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
        private BetfairScrapperMatch _match;

        private string _username = "lucasbustoss";
        private string _password = "simSenhor0182@";
        private string _secretCodeTwoFactor = "LACUV35V5VEB5PFU";

        private string _urlLogin = "https://identitysso.betfair.com/view/login?product=vendor&url=https%3A%2F%2Fidentitysso.betfair.com%2Fview%2Fvendor-login%3Fresponse_type%3Dcode%26redirect_uri%3Dauthenticate%26client_id%3D118466";

        private string _baseUrlMarket = "https://software.layback.me/market?";
        private string _urlMarket;

        public BetfairScrapper(MongoDBService mongoDb, BetfairAPIService betfairApi, BetfairScrapperMatch match)
        {
            _mongoDb = mongoDb;
            _credentials = mongoDb.GetBetfairCredentials();
            _match = match;

            _mongoDb.SaveBetfairScrapperMatch(match);

            StartDriver();
            _betfairApi = betfairApi;

            _urlMarket = GetUrlMarket(match.EventId.ToString(), match.MarketId);
            _driver.Navigate().GoToUrl(_urlMarket);

            Thread.Sleep(6000);
        }

        public void Scrap(BetfairScrapperExecution execution)
        {
            Console.WriteLine($"Inicio odds jogo {_match.MatchName} {DateTime.UtcNow}");
            string minute = "";

            try
            {
                IWebElement minuteElement = _driver.FindElement(By.XPath("(//p[contains(@style, 'color: rgb(0, 210, 0)')])[2]"));
                minute = minuteElement.GetAttribute("innerHTML").Replace("'", "");
                execution.Minute = minute;
            }
            catch
            {

            }

            GetLadderInfo(_match, execution, minute);

            Console.WriteLine($"Fim odds jogo {_match.MatchName} {DateTime.UtcNow}");
        }

        #region Private methods

        private void StartDriver()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var chromeOptions = new ChromeOptions();
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            chromeDriverService.EnableVerboseLogging = false;
            chromeDriverService.HideCommandPromptWindow = true;  // Oculta a janela de comando no Windows

            if (environment == "Docker")
            {
                chromeOptions.AddArgument("--headless");
                chromeOptions.AddArgument("--no-sandbox");
                chromeOptions.AddArgument("--disable-dev-shm-usage");
                chromeOptions.AddArgument("--disable-gpu");
                chromeOptions.AddArgument("--disable-software-rasterizer");
                chromeOptions.AddArgument("--disable-extensions");
                chromeOptions.AddArgument("--disable-infobars");
                chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
                chromeOptions.AddArgument("--window-size=1920,1080");  // Define um tamanho padrão de janela
            }

            _driver = new ChromeDriver(chromeDriverService, chromeOptions);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));

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

                Parallel.For(0, Math.Min(teams.Count, ladders.Count), new ParallelOptions { MaxDegreeOfParallelism = 8 }, i =>
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
                ", ladder);


                // Iterar sobre a coleção retornada
                foreach (var data in oddsData)
                {
                    var dict = (IDictionary<string, object>)data;
                    string odd = dict["odd"]?.ToString();
                    string oddClass = dict["oddClass"]?.ToString();
                    var buttons = (ReadOnlyCollection<object>)dict["buttons"];

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
