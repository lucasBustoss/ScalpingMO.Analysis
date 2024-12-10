using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;
using System.Text.Json;
using Cookie = Microsoft.Playwright.Cookie;

namespace ScalpingMO.Analysis.Extract.FixtureData.Data.Betfair
{
    public class BetfairScrapper
    {
        private IPage _page;
        private IBrowser _browser;
        private MongoDBService _mongoDb;
        private BetfairCookie _credentials;
        private BetfairAPIService _betfairApi;

        private string _username = "carolbetfair111";
        private string _password = "simSenhor0182@";
        private string _secretCodeTwoFactor = "NML23LV6FH2GDLSG";

        private string _urlLogin = "https://identitysso.betfair.com/view/login?product=vendor&url=https%3A%2F%2Fidentitysso.betfair.com%2Fview%2Fvendor-login%3Fresponse_type%3Dcode%26redirect_uri%3Dauthenticate%26client_id%3D118466";

        private string _baseUrlMarket = "https://software.layback.me/market?";
        private string _urlMarket;

        public BetfairScrapper(MongoDBService mongoDb, BetfairAPIService betfairApi, IBrowser browser, string matchName, string eventId, string marketId)
        {
            _mongoDb = mongoDb;
            _credentials = mongoDb.GetBetfairCredentials();
            _browser = browser;

            _betfairApi = betfairApi;

            _urlMarket = GetUrlMarket(eventId, marketId);
        }

        public async Task StartDriver()
        {
            var context = await _browser.NewContextAsync();

            // Navegar até a URL e configurar os cookies
            _page = await context.NewPageAsync();
            await _page.GotoAsync("https://software.layback.me/");

            await SetCookiesAsync(context);

            await _page.GotoAsync("https://software.layback.me/");
            await Task.Delay(10000);

            var currentUrl = _page.Url;

            if (currentUrl == "https://software.layback.me/login?tokenExpired=true" || currentUrl == "https://software.layback.me/login")
            {
                Console.WriteLine("cai no login");
                await LoginAsync();
            }
            else
                Console.WriteLine("não cai no login");


            await _page.GotoAsync(_urlMarket);
            await Task.Delay(3000);
        }

        public async Task ScrapAsync(BetfairScrapperMatch match)
        {
            Console.WriteLine($"Inicio odds jogo {match.MatchName} {DateTime.UtcNow}");
            string minute = "";

            try
            {
                // Usando Playwright para localizar o elemento do minuto
                var minuteElement = await _page.QuerySelectorAsync("(//p[contains(@style, 'color: rgb(0, 210, 0)')])[2]");
                if (minuteElement != null)
                {
                    minute = (await minuteElement.InnerHTMLAsync()).Replace("'", "");
                    match.Minute = minute;
                }
            }
            catch
            {
                Console.WriteLine("Erro ao buscar o minuto.");
            }

            await GetLadderInfoAsync(match, minute);

            Console.WriteLine($"Fim odds jogo {match.MatchName} {DateTime.UtcNow}");
        }

        #region Private methods

        private async Task SetCookiesAsync(IBrowserContext context)
        {
            string decodedAccessToken = Uri.UnescapeDataString(_credentials?.AccessToken ?? "");

            // Adicionando cookies usando o contexto do navegador, com domínio e path explícitos
            await context.AddCookiesAsync(new[]
            {
                new Cookie { Name = "layback.accessToken", Value = decodedAccessToken, Domain = "software.layback.me", Path = "/" },
                new Cookie { Name = "layback.exchange", Value = _credentials?.Exchange ?? "", Domain = "software.layback.me", Path = "/" },
                new Cookie { Name = "layback.refreshToken", Value = _credentials?.RefreshToken ?? "", Domain = "software.layback.me", Path = "/" },
                new Cookie { Name = "layback.source", Value = _credentials?.Source ?? "", Domain = "software.layback.me", Path = "/" }
            });
        }


        private async Task LoginAsync()
        {
            await _page.GotoAsync(_urlLogin);
            await AllowCookiesAsync();

            // Aguardando o elemento estar disponível e inserindo os valores
            var usernameInput = await _page.WaitForSelectorAsync("#username", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await usernameInput.FillAsync(_username);

            var passwordInput = await _page.WaitForSelectorAsync("#password", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            if (passwordInput != null)
            {
                string twoFactorCode = TwoFactor.GenerateTwoFactorAuthCode(_secretCodeTwoFactor);
                await passwordInput.FillAsync($"{_password}{twoFactorCode}");
            }

            var loginButton = await _page.WaitForSelectorAsync("#login", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
            await loginButton.ClickAsync();

            await Task.Delay(3000); // Substituindo Thread.Sleep

            await SaveCookiesAsync();
        }

        private async Task AllowCookiesAsync()
        {
            var buttonAllowCookies = await _page.WaitForSelectorAsync("#onetrust-accept-btn-handler", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            if (buttonAllowCookies != null)
            {
                await buttonAllowCookies.ClickAsync();
            }
        }

        private async Task SaveCookiesAsync()
        {
            // Obtém todos os cookies do contexto atual da página
            var cookies = await _page.Context.CookiesAsync(new[] { "https://software.layback.me" });

            string accessToken = "";
            string refreshToken = "";
            string source = "";
            string exchange = "";

            // Loop através dos cookies e atribui os valores apropriados
            foreach (var cookie in cookies)
            {
                if (cookie.Name == "layback.accessToken")
                    accessToken = cookie.Value;

                if (cookie.Name == "layback.refreshToken")
                    refreshToken = cookie.Value;

                if (cookie.Name == "layback.source")
                    source = cookie.Value;

                if (cookie.Name == "layback.exchange")
                    exchange = cookie.Value;
            }

            // Cria o objeto com os valores dos cookies e salva no MongoDB
            BetfairCookie betfairCookie = new BetfairCookie(accessToken, refreshToken, source, exchange);
            _mongoDb.SaveBetfairCredentials(betfairCookie);
        }

        private string GetUrlMarket(string eventId, string marketId)
        {
            return $"{_baseUrlMarket}eventId={eventId}&marketId={marketId}";
        }

        private async Task GetLadderInfoAsync(BetfairScrapperMatch match, string minute)
        {
            try
            {
                string homeTeamName = match.MatchName.Split(" v ")[0];
                string awayTeamName = match.MatchName.Split(" v ")[1];

                // Capturar separadamente os times e os ladders com Playwright
                var teams = await _page.QuerySelectorAllAsync("span.ml-1.block.truncate.text-xs");
                var ladders = await _page.QuerySelectorAllAsync("div.mb-1.flex.w-full.flex-col");

                if (teams.Count == 0 || ladders.Count == 0)
                {
                    Console.WriteLine("Nenhum elemento encontrado para teams ou ladders.");
                    return;
                }

                // Usando paralelismo para processar odds
                await Task.WhenAll(teams.Zip(ladders, async (team, ladder) =>
                {
                    List<BetfairScrapperOdd> odds = await GetOddsFromLadderWithJsAsync(ladder);

                    string teamName = await team.InnerHTMLAsync();

                    if (teamName.Trim() == homeTeamName.Trim())
                        match.HomeOdds = odds;
                    else if (teamName.Trim() == awayTeamName.Trim())
                        match.AwayOdds = odds;
                    else
                        match.DrawOdds = odds;
                }));
            }
            catch
            {
                Console.WriteLine("Erro para buscar as odds.");
            }
        }

        private async Task<List<BetfairScrapperOdd>> GetOddsFromLadderWithJsAsync(IElementHandle ladder)
        {
            try
            {
                List<BetfairScrapperOdd> betfairOdds = new List<BetfairScrapperOdd>();

                var oddsData = await _page.EvaluateAsync<JsonElement>(@"(ladder) => {
                    var secondDivChild = ladder.querySelector('div:nth-child(2)');
                    if (!secondDivChild) return [];

                    // Captura apenas as divs filhas diretas e limita a quantidade de elementos buscados
                    var oddsElements = secondDivChild.children;
                    var result = [];

                    for (var i = 0; i < oddsElements.length; i++) {
                        var element = oddsElements[i];

                        // Captura o elemento <p> da odd
                        var oddElement = element.querySelector('p');
                        if (!oddElement) continue;

                        var oddValue = oddElement.textContent.trim();
                        var oddClass = oddElement.className;

                        // Capturar textContent para garantir que o texto dos botões seja extraído
                        var buttons = [];
                        var buttonElements = element.querySelectorAll('button');
                        for (var j = 0; j < buttonElements.length; j++) {
                            buttons.push(buttonElements[j].textContent.trim());
                        }

                        result.push({ odd: oddValue, oddClass: oddClass, buttons: buttons });
                    }

                    return result;
                }", ladder);

                // Iterar sobre a coleção retornada
                foreach (var element in oddsData.EnumerateArray())
                {
                    string odd = element.GetProperty("odd").GetString();
                    string oddClass = element.GetProperty("oddClass").GetString();
                    var buttons = element.GetProperty("buttons").EnumerateArray().Select(b => b.GetString()).ToList();

                    if (string.IsNullOrEmpty(odd)) continue;

                    bool isLastCorrespondence = !string.IsNullOrEmpty(oddClass) && oddClass.Contains("bg-white");

                    string availableToLay = buttons.Count > 2 ? buttons[1].ToString() : "";
                    string availableToBack = buttons.Count > 2 ? buttons[2].ToString() : "";

                    // Criar e adicionar o modelo de odds
                    BetfairScrapperOdd oddModel = new BetfairScrapperOdd(odd, availableToLay, availableToBack, isLastCorrespondence);
                    betfairOdds.Add(oddModel);
                }

                return betfairOdds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
