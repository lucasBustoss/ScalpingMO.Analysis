using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds;
using System.Text.Json;

namespace ScalpingMO.Analysis.Extract.FixtureData.Data
{
    public class FootballAPIService
    {
        private MongoDBService _mongoDB;
        private readonly HttpClient _httpClient;

        public FootballAPIService(MongoDBService mongoDB, string url, string apiKey, string apiHost)
        {
            _mongoDB = mongoDB;

            _httpClient = new HttpClient() { BaseAddress = new Uri(url) };
            _httpClient.DefaultRequestHeaders.Add("X-RapidApi-Key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidApi-Host", apiHost);
        }

        public async Task GetLiveOdds()
        {
            if (!CheckRateLimit())
            {
                Console.WriteLine("FootballAPI rate exceeded");
                return;
            }

            HttpResponseMessage request = await _httpClient.GetAsync($"odds/live?bet=59");

            if (request.IsSuccessStatusCode)
            {
                UpdateRateLimit(request);

                string response = await request.Content.ReadAsStringAsync();
                _mongoDB.SaveOddResponseRaw(response);
            }
        }

        private void UpdateRateLimit(HttpResponseMessage request)
        {
            IEnumerable<string> limit;
            IEnumerable<string> remaining;
            IEnumerable<string> reset;

            if (request.Headers.TryGetValues("x-ratelimit-requests-limit", out limit) &&
                request.Headers.TryGetValues("x-ratelimit-requests-remaining", out remaining) &&
                request.Headers.TryGetValues("x-ratelimit-requests-reset", out reset))
            {
                RateLimit rateLimit = new RateLimit(limit.FirstOrDefault(), remaining.FirstOrDefault(), reset.FirstOrDefault());
                _mongoDB.UpdateRateLimit(rateLimit);
            }
        }

        private bool CheckRateLimit()
        {
            RateLimit rate = _mongoDB.GetRateLimit();

            if (rate == null)
                return true;

            bool isResetTimePassed = rate.LastRequest.AddSeconds(rate.RequestsReset) < DateTime.UtcNow;

            bool hasRemainingRequests = rate.RequestsRemaining > 0;

            return isResetTimePassed || hasRemainingRequests;
        }
    }
}
