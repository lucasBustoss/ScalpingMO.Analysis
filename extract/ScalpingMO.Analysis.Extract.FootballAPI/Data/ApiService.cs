using ScalpingMO.Analysis.Extract.FootballAPI.Models;
using ScalpingMO.Analysis.Extract.FootballAPI.Models.Response;
using System.Text.Json;

namespace ScalpingMO.Analysis.Extract.FootballAPI.Data
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly MongoDBService _mongoDB;

        public ApiService(string url, string apiKey, string apiHost)
        {
            _mongoDB = new MongoDBService();

            _httpClient = new HttpClient() { BaseAddress = new Uri(url) };
            _httpClient.DefaultRequestHeaders.Add("X-RapidApi-Key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidApi-Host", apiHost);
        }

        public async Task<List<FixtureResponse>> GetFixtures(string date)
        {
            if (!CheckRateLimit())
            {
                Console.WriteLine("FootballAPI rate exceeded");
                return null;
            }

            HttpResponseMessage request = await _httpClient.GetAsync($"fixtures?date={date}");

            if (request.IsSuccessStatusCode)
            {
                UpdateRateLimit(request);

                string response = await request.Content.ReadAsStringAsync();
                ApiBaseResponse<FixtureResponse> responseData = JsonSerializer.Deserialize<ApiBaseResponse<FixtureResponse>>(response);

                if (responseData == null)
                    return null;

                return responseData.Response.ToList();
            }

            return null;
        }

        public async Task<List<OddsResponse>> GetLiveOdds()
        {
            if (!CheckRateLimit())
            {
                Console.WriteLine("FootballAPI rate exceeded");
                return null;
            }

            HttpResponseMessage request = await _httpClient.GetAsync($"/odds/live");

            if (request.IsSuccessStatusCode)
            {
                UpdateRateLimit(request);
                
                string response = await request.Content.ReadAsStringAsync();
                ApiBaseResponse<OddsResponse> responseData = JsonSerializer.Deserialize<ApiBaseResponse<OddsResponse>>(response);

                if (responseData == null)
                    return null;

                return responseData.Response.ToList();
            }

            return null;
        }

        #region Private methods

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

        #endregion
    }
}
