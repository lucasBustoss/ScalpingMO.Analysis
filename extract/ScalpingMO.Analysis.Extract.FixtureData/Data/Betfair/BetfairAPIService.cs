using System.Text.Json;
using System.Text;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Request;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response.Error;

namespace ScalpingMO.Analysis.Extract.FixtureData.Data.Betfair
{
    public class BetfairAPIService
    {
        private HttpClient _betfairClient;
        private string _token;

        public BetfairAPIService(string url, string appKey, string token)
        {
            _betfairClient = new HttpClient() { BaseAddress = new Uri(url) };
            _betfairClient.DefaultRequestHeaders.Add("X-Application", appKey);
            _betfairClient.DefaultRequestHeaders.Add("X-Authentication", token);
            _betfairClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _betfairClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        public async Task<List<BetfairEventResponse>> GetEvents(string eventId)
        {
            BetfairEventsRequest listEvents = new BetfairEventsRequest();
            listEvents.Filter.EventTypeIds = ["1"];
            listEvents.Filter.EventIds = [eventId];
            HttpContent content = new StringContent(JsonSerializer.Serialize(listEvents), Encoding.UTF8, "application/json");

            List<BetfairEventResponse> events = null;

            HttpResponseMessage request = await _betfairClient.PostAsync("listEvents/", content);

            if (request.IsSuccessStatusCode)
            {
                string response = await request.Content.ReadAsStringAsync();
                events = JsonSerializer.Deserialize<List<BetfairEventResponse>>(response);

                if (events == null)
                    return null;

                return events;
            }
            else
            {
                return await CheckErrorBetfair(() => GetEvents(eventId), request);
            }
        }

        public async Task<BetfairMarketBookResponse> GetOdds(string marketId)
        {
            BetfairMarketBookRequest listBooksBody = new BetfairMarketBookRequest()
            {
                PriceProjection = new PriceProjection() { PriceData = ["EX_ALL_OFFERS", "EX_TRADED"] },
                MarketIds = [marketId]
            };

            HttpContent content = new StringContent(JsonSerializer.Serialize(listBooksBody), Encoding.UTF8, "application/json");

            List<BetfairMarketBookResponse> markets = null;

            HttpResponseMessage request = await _betfairClient.PostAsync("listMarketBook/", content);

            if (request.IsSuccessStatusCode)
            {
                //Console.WriteLine("ok");
                string response = await request.Content.ReadAsStringAsync();
                markets = JsonSerializer.Deserialize<List<BetfairMarketBookResponse>>(response);

                if (markets == null)
                    return null;

                BetfairMarketBookResponse market = markets.FirstOrDefault();

                if (market.LastMatchedTime > DateTime.UtcNow.AddSeconds(-4))
                    return market;

                return null;
            }
            else
            {
                return await CheckErrorBetfair(() => GetOdds(marketId), request);
            }
        }

        #region Private methods

        private async Task Login()
        {
            //bool tokenIsValid = await ValidateToken();

            //if (!tokenIsValid)
            //{
            //    string twoFactor = TwoFactor.GenerateTwoFactorAuthCode(_configuration.SecretTwoFactorCode);

            //    var formAuth = new Dictionary<string, string>
            //    {
            //        { "username",  _configuration.Username},
            //        { "password", $"{_configuration.Password}{twoFactor}" },
            //    };

            //    HttpContent content = new FormUrlEncodedContent(formAuth);
            //    HttpResponseMessage request = await _authClient.PostAsync("", content);

            //    if (request.IsSuccessStatusCode)
            //    {
            //        string response = await request.Content.ReadAsStringAsync();
            //        BetfairLoginResponse responseObject = JsonSerializer.Deserialize<BetfairLoginResponse>(response);

            //        if (responseObject != null)
            //        {
            //            _betfairClient.DefaultRequestHeaders.Add("X-Authentication", responseObject.Token);
            //            _token = responseObject.Token;
            //        }
            //    }
            //}
        }

        private async Task<T> CheckErrorBetfair<T>(Func<Task<T>> method, HttpResponseMessage request)
        {
            string errorResponse = await request.Content.ReadAsStringAsync();
            BetfairErrorResponse error = JsonSerializer.Deserialize<BetfairErrorResponse>(errorResponse);

            if (error.Code == "ANGX-0003")
            {
                Console.WriteLine("Sessão inválida. Fazendo login e tentando novamente.");
                await Login();
                return await method();
            }
            else
            {
                throw new Exception($"Erro ao buscar mercados na Betfair. Código {error.Code}");
            }
        }

        #endregion
    }
}
