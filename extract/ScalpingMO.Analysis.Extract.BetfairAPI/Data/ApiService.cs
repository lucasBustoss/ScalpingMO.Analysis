using ScalpingMO.Analysis.Extract.BetfairAPI.Models;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models.Request;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response;
using ScalpingMO.Analysis.Extract.BetfairAPI.Models.Response.Error;
using System.Text;
using System.Text.Json;

namespace ScalpingMO.Analysis.Extract.BetfairAPI.Data
{
    public class ApiService
    {
        private string _token = "U6CyhuH4jMKG65Q5sUjijoxTT67IZKOlXhdEvjdzReo=";
        private BetfairConfiguration _configuration;
        private HttpClient _betfairClient;
        private HttpClient _authClient;

        public ApiService(BetfairConfiguration configuration)
        {
            _configuration = configuration;
            
            _betfairClient = new HttpClient() { BaseAddress = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/") };
            _betfairClient.DefaultRequestHeaders.Add("X-Application", configuration.ApiKey);
            _betfairClient.DefaultRequestHeaders.Add("X-Authentication", _token);
            _betfairClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _betfairClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            _authClient = new HttpClient() { BaseAddress = new Uri("https://identitysso.betfair.com/api/login") };
            _authClient.DefaultRequestHeaders.Add("X-Application", configuration.ApiKey);
            _authClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _authClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
        }

        public async Task<List<Fixture>> GetFixtures()
        {
            List<Fixture> matches = new List<Fixture>();
            
            List<BetfairEventResponse> betfairEvents = await GetEvents();

            foreach (BetfairEventResponse betfairEvent in betfairEvents)
            {
                if (betfairEvent.Event.Name == "Specials")
                    continue;

                BetfairMarketCatalogueResponse betfairMarket = await GetMarketCatalogue(betfairEvent.Event.Id);
                string homeTeamName = "";
                string awayTeamName = "";

                try
                {
                    string eventName = betfairEvent.Event.Name;
                    homeTeamName = eventName.Split(" v ")[0];
                    awayTeamName = eventName.Split(" v ")[1];
                }
                catch
                {
                    continue;
                }

                int id = Convert.ToInt32(betfairEvent.Event.Id);
                DateTime date = Convert.ToDateTime(betfairEvent.Event.OpenDate);
                string marketId = betfairMarket != null ? betfairMarket.MarketId : null;

                Fixture match = new Fixture(id, date, marketId, homeTeamName, awayTeamName);
                matches.Add(match);
            }

            return matches;
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

        private async Task<bool> ValidateToken()
        {
            try
            {
                Dictionary<string, object> body = new Dictionary<string, object>
                {
                    { "filter", new Dictionary<string, object>() }
                };
                HttpContent content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                HttpResponseMessage request = await _betfairClient.PostAsync("listCountries/", content);

                if (request.IsSuccessStatusCode)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<BetfairEventResponse>> GetEvents()
        {
            BetfairEventsRequest listEvents = new BetfairEventsRequest();
            listEvents.Filter.EventTypeIds = ["1"];
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
                return await CheckErrorBetfair(GetEvents, request);
            }
        }

        private async Task<BetfairMarketCatalogueResponse> GetMarketCatalogue(string eventId)
        {
            try
            {
                BetfairListMarketCatalogueRequest listMarketCatalogues = new BetfairListMarketCatalogueRequest()
                {
                    Filter = new BetfairFilter() { EventIds = new List<string> { eventId }, MarketTypeCodes = ["MATCH_ODDS"] },
                    MaxResults = 1,
                    CurrencyCode = "BRL"
                };

                HttpContent content = new StringContent(JsonSerializer.Serialize(listMarketCatalogues), Encoding.UTF8, "application/json");

                List<BetfairMarketCatalogueResponse> markets = null;

                HttpResponseMessage request = await _betfairClient.PostAsync("listMarketCatalogue/", content);

                if (request.IsSuccessStatusCode)
                {
                    string response = await request.Content.ReadAsStringAsync();
                    markets = JsonSerializer.Deserialize<List<BetfairMarketCatalogueResponse>>(response);

                    if (markets == null)
                        return null;

                    return markets.FirstOrDefault();
                }
                else
                {
                    return await CheckErrorBetfair(() => GetMarketCatalogue(eventId), request);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
