using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;

namespace ScalpingMO.Analysis.Extract.FixtureData.Data.Betfair
{
    public class BetfairScrapper
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private MongoDBService _mongoDb;
        private BetfairCookie _credentials;
        private BetfairAPIService _betfairApi;

        private string _username = "lucasbustoss";
        private string _password = "simSenhor0182@";
        private string _secretCodeTwoFactor = "LACUV35V5VEB5PFU";

        private string _urlLogin = "https://identitysso.betfair.com/view/login?product=vendor&url=https%3A%2F%2Fidentitysso.betfair.com%2Fview%2Fvendor-login%3Fresponse_type%3Dcode%26redirect_uri%3Dauthenticate%26client_id%3D118466";

        private string _baseUrlMarket = "https://software.layback.me/market?";
        private string _urlMarket;

        public BetfairScrapper(MongoDBService mongoDb, BetfairAPIService betfairApi)
        {
            _mongoDb = mongoDb;
            _credentials = mongoDb.GetBetfairCredentials();

            StartDriver();
            _betfairApi = betfairApi;
            //Login();

        }

        public async Task Scrap(string eventId, string marketId)
        {
            BetfairEventResponse betfairEvent = await GetBetfairEvent(eventId);

            if (betfairEvent == null)
                return;

            _urlMarket = GetUrlMarket(eventId, marketId);
            _driver.Navigate().GoToUrl(_urlMarket);

            Thread.Sleep(6000);
            BetfairScrapperMatch match = new BetfairScrapperMatch(betfairEvent.Event.Name, Convert.ToInt32(eventId), marketId);
            _mongoDb.SaveBetfairScrapperMatch(match);

            int tries = 0;

            while (tries < 5)
            {
                Console.WriteLine($"Inicio odds {DateTime.UtcNow}");

                BetfairScrapperExecution execution = new BetfairScrapperExecution();
                IWebElement minuteElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("(//p[contains(@style, 'color: rgb(0, 210, 0)')])[2]")));
                string minute = minuteElement.GetAttribute("innerHTML").Replace("'", "");
                
                GetLadderInfo(match, execution, minute);
                _mongoDb.SaveBetfairScrapperExecutionInMatch(Convert.ToInt32(eventId), execution);
                
                Console.WriteLine($"Fim odds {DateTime.UtcNow}");

                if (minute == "Intervalo")
                    Thread.Sleep(60000);
                else
                    Thread.Sleep(2000);

                tries++;
            }
        }

        #region Private methods

        private async Task<BetfairEventResponse> GetBetfairEvent(string eventId)
        {
            List<BetfairEventResponse> events = await _betfairApi.GetEvents(eventId);

            if (events != null && events.Count > 0)
                return events.FirstOrDefault();

            return null;
        }

        private void StartDriver()
        {
            string enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var chromeOptions = new ChromeOptions();

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.SuppressInitialDiagnosticInformation = true;  // Suprime logs de inicialização
            chromeDriverService.EnableVerboseLogging = false;  // Desativa logs detalhados
            chromeDriverService.HideCommandPromptWindow = true;  // Oculta a janela de comando (somente no Windows)

            if (enviroment == "Docker")
            {
                chromeOptions.AddArgument("--headless");  // Para rodar no Docker sem interface gráfica
                chromeOptions.AddArgument("--no-sandbox");  // Recomendado para Docker
                chromeOptions.AddArgument("--disable-dev-shm-usage");  // Evita problemas com espaço limitado
            }

            _driver = new ChromeDriver(chromeDriverService, chromeOptions);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _driver.Navigate().GoToUrl("https://software.layback.me/");

            SetCookies();

            _driver.Navigate().GoToUrl("https://software.layback.me/");

            Thread.Sleep(10000);

            if (_driver.Url == "https://software.layback.me/login?tokenExpired=true" || _driver.Url == "https://software.layback.me/login")
            {
                Console.WriteLine("cai no login");
                Login();
            }
            else
            {
                Console.WriteLine("não cai no login");
            }
        }

        private void SetCookies()
        {
            string decodedAccessToken = HttpUtility.UrlDecode(_credentials != null ? _credentials.AccessToken : "");
            Cookie accessToken = new Cookie("layback.accessToken", decodedAccessToken);
            Cookie exchange = new Cookie("layback.exchange", _credentials != null ? _credentials.Exchange : "");
            Cookie refreshToken = new Cookie("layback.refreshToken", _credentials != null ? _credentials.RefreshToken : "");
            Cookie source = new Cookie("layback.source", _credentials != null ? _credentials.Source : "");

            // Adicione os cookies
            _driver.Manage().Cookies.AddCookie(accessToken);
            _driver.Manage().Cookies.AddCookie(exchange);
            _driver.Manage().Cookies.AddCookie(refreshToken);
            _driver.Manage().Cookies.AddCookie(source);
        }

        private void Login()
        {
            _driver.Navigate().GoToUrl(_urlLogin);
            AllowCookies();

            IWebElement usernameInput = _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("username")));
            IWebElement passwordInput = _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("password")));

            if (usernameInput != null)
                usernameInput.SendKeys(_username);

            if (passwordInput != null)
            {
                string twoFactorCode = TwoFactor.GenerateTwoFactorAuthCode(_secretCodeTwoFactor);
                passwordInput.SendKeys($"{_password}{twoFactorCode}");
            }

            IWebElement loginButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("login")));
            loginButton.Click();

            Thread.Sleep(3000);

            SaveCookies();
        }

        private void AllowCookies()
        {
            IWebElement buttonAllowCookies = _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("onetrust-accept-btn-handler")));

            if (buttonAllowCookies != null)
                buttonAllowCookies.Click();
        }

        private void SaveCookies()
        {
            List<Cookie> cookies = _driver.Manage().Cookies.AllCookies.ToList();
            string accessToken = "";
            string refreshToken = "";
            string source = "";
            string exchange = "";

            foreach (Cookie cookie in cookies)
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

            BetfairCookie betfairCookie = new BetfairCookie(accessToken, refreshToken, source, exchange);
            _mongoDb.SaveBetfairCredentials(betfairCookie);
        }

        private string GetUrlMarket(string eventId, string marketId)
        {
            return $"{_baseUrlMarket}eventId={eventId}&marketId={marketId}";
        }

        private void GetLadderInfo(BetfairScrapperMatch match, BetfairScrapperExecution execution, string minute)
        {
            try
            {
                string homeTeamName = match.MatchName.Split(" v ")[0];
                string awayTeamName = match.MatchName.Split(" v ")[1];

                // Capturar separadamente os times e os ladders
                List<IWebElement> teams = _driver.FindElements(By.CssSelector("span.ml-1.block.truncate.text-xs")).ToList();
                List<IWebElement> ladders = _driver.FindElements(By.CssSelector("div.mb-1.flex.w-full.flex-col")).ToList();

                if (teams.Count == 0 || ladders.Count == 0)
                {
                    Console.WriteLine("Nenhum elemento encontrado para teams ou ladders.");
                    return;
                }

                // Iterar apenas sobre os elementos encontrados em paralelo
                Parallel.For(0, Math.Min(teams.Count, ladders.Count), i =>
                {
                    IWebElement team = teams[i];
                    IWebElement ladder = ladders[i];

                    BetfairScrapperTeamOdd teamOdd = new BetfairScrapperTeamOdd(minute);
                    teamOdd.Odds = GetOddsFromLadderWithJs(ladder);

                    string teamName = team.GetAttribute("innerHTML");

                    if (teamName.Trim() == homeTeamName.Trim())
                        execution.HomeOdds = teamOdd;
                    else if (teamName.Trim() == awayTeamName.Trim())
                        execution.AwayOdds = teamOdd;
                    else
                        execution.DrawOdds = teamOdd;
                });
            }
            catch
            {
                Console.WriteLine("Erro para buscar as odds");
            }
        }

        private List<BetfairScrapperOdd> GetOddsFromLadderWithJs(IWebElement ladder)
        {
            try
            {
                List<BetfairScrapperOdd> betfairOdds = new List<BetfairScrapperOdd>();

                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;

                var oddsData = (ReadOnlyCollection<object>)js.ExecuteScript(@"
                    var secondDivChild = arguments[0].querySelector('div:nth-child(2)');
                    if (!secondDivChild) return [];

                    // Captura apenas as divs filhas diretas
                    var oddsElements = Array.from(secondDivChild.children);
                    var result = [];
                    oddsElements.forEach(function(element) {
                        var oddElement = element.querySelector('div > p');
                        var oddValue = oddElement ? oddElement.textContent.trim() : null;
                        var oddClass = oddElement ? oddElement.className : null;

                        // Capturar textContent para garantir que o texto dos botões seja extraído
                        var buttons = Array.from(element.querySelectorAll('div > button')).map(button => button.textContent.trim());
                        result.push({ odd: oddValue, oddClass: oddClass, buttons: buttons });
                    });
                    return result;
                ", ladder);

                // Iterar sobre a coleção retornada
                foreach (var data in oddsData)
                {
                    // Cada item é um dicionário que contém "odd", "oddClass", e "buttons"
                    var dict = (IDictionary<string, object>)data;
                    string odd = dict["odd"]?.ToString();
                    string oddClass = dict["oddClass"]?.ToString();
                    var buttons = (ReadOnlyCollection<object>)dict["buttons"];

                    if (string.IsNullOrEmpty(odd)) 
                        continue;
                    
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
