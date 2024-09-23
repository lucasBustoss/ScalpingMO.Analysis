using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ScalpingMO.Analysis.Extract.FixtureData.Data;
using ScalpingMO.Analysis.Extract.FixtureData.Data.Betfair;
using ScalpingMO.Analysis.Extract.FixtureData.Models;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds;
using ScalpingMO.Analysis.Extract.FixtureData.Models.Radar;
using SeleniumExtras.WaitHelpers;

namespace ScalpingMO.Analysis.Extract.FixtureData.Worker
{
    public class DataWorker
    {
        #region Radar

        private IWebDriver _driver;
        IWebElement _commentBox;
        Queue<RadarInfo> _comments;

        #endregion

        #region FootballApi

        private Queue<ReferenceOddInfo> _referenceOdds;
        private int _thresholdExtract = 200;
        private int _thresholdSave = 500;
        private WebDriverWait _wait;

        #endregion

        #region Betfair

        private BetfairAPIService _betfairAPI;
        private BetfairScrapper _betfairScrapper;
        private BetfairEventResponse _event;
        private Queue<BetfairScrapperExecution> _betfairScrapperOdds;
        private BetfairScrapperMatch _betfairScrapperMatch;

        #endregion

        private Fixture _fixture;
        private MongoDBService _mongoDB;
        private bool _matchFinished = false;

        /// <summary>
        /// Parametro pra teste. Depois de tudo desenvolvido vou remvoer
        /// </summary>
        private int _tries = 0;
        private int _triesThreshold = 100000;

        public DataWorker(MongoDBService mongoDB, BetfairAPIService betfairApi)
        {
            _mongoDB = mongoDB;
            _betfairAPI = betfairApi;
        }

        public async Task Execute(Fixture fixture)
        {
            _fixture = fixture;

            string matchName = $"{fixture.HomeTeamName} x {fixture.AwayTeamName}";
            Console.WriteLine($"Iniciando a extração dos dados do jogo {matchName}");

            await InitializeBetfairApi(fixture.BetfairId.Value);
            InitializeScrapper(fixture.RadarUrl);
            InitializeFootballApi();

            Task getCommentTask = ExecuteGetCommentAsync();
            Task betfairApiTask = ExecuteBetfairApiAsync();
            Task footballApiTask = ExecuteFootballApiAsync();

            Task saveRadarCommentsTask = ExecuteSaveRadarCommentsAsync();
            Task saveReferenceOddTask = ExecuteSaveReferenceOddAsync();
            Task saveBetfairOddTask = ExecuteSaveBetfairOddsAsync();

            await Task.WhenAll(getCommentTask, betfairApiTask, footballApiTask, saveRadarCommentsTask, saveReferenceOddTask, saveBetfairOddTask);

            CloseScrapper();

            Console.WriteLine($"Fim da extração dos dados do jogo {matchName}.");
        }

        #region Private methods

        #region Tasks

        private async Task ExecuteGetCommentAsync()
        {
            while (!_matchFinished && _tries < _triesThreshold)
            {
                GetComment();
                await Task.Delay(1000);
            }
        }

        private async Task ExecuteBetfairApiAsync()
        {
            while (!_matchFinished && _tries < _triesThreshold)
            {
                BetfairScrapperExecution execution = new BetfairScrapperExecution();
                GetBetfairScrapperOdds(execution);

                await Task.Delay(500);
            }
        }

        private async Task ExecuteFootballApiAsync()
        {
            while (!_matchFinished && _tries < _triesThreshold)
            {
                GetReferenceOdd();
                _tries++;
                await Task.Delay(_thresholdExtract);
            }
        }

        private async Task ExecuteSaveRadarCommentsAsync()
        {
            while ((!_matchFinished && _tries < _triesThreshold) || _comments.Count > 0)
            {
                if (_comments.Count > 0)
                {
                    RadarInfo info = _comments.Dequeue();
                    _mongoDB.SaveRadarInfo(info);
                }

                await Task.Delay(_thresholdSave);
            }
        }

        private async Task ExecuteSaveReferenceOddAsync()
        {
            while ((!_matchFinished && _tries < _triesThreshold) || _referenceOdds.Count > 0)
            {
                if (_referenceOdds.Count > 0)
                {
                    ReferenceOddInfo info = _referenceOdds.Dequeue();
                    _mongoDB.SaveReferenceOddInfo(info);
                }

                await Task.Delay(_thresholdSave);
            }
        }

        private async Task ExecuteSaveBetfairOddsAsync()
        {
            while ((!_matchFinished && _tries < _triesThreshold) || _betfairScrapperOdds.Count > 0)
            {
                if (_betfairScrapperOdds.Count > 0)
                {
                    BetfairScrapperExecution execution = _betfairScrapperOdds.Dequeue();
                    _mongoDB.SaveBetfairScrapperExecutionInMatch(_betfairScrapperMatch.EventId, execution);
                }

                await Task.Delay(_thresholdSave);
            }
        }

        #endregion

        #region Scrapper

        #region Initialize 

        private void InitializeScrapper(string url)
        {
            _comments = new Queue<RadarInfo>();
            StartDriver();

            _driver.Navigate().GoToUrl(url);

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            IWebElement balloonButton = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-action=\"commentaries\"]/a")));
            balloonButton.Click();

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
        }

        private void CloseScrapper()
        {
            _driver.Quit();
        }

        #endregion

        #region Task 

        private void GetComment()
        {
            try
            {
                _commentBox = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"box_commentaries\"]")));
                IEnumerable<IWebElement> listComments = _commentBox.FindElements(By.XPath("ul/li"));

                if (listComments != null && listComments.Count() > 0)
                {
                    IEnumerable<IWebElement> top5Comments = listComments.Take(6);

                    foreach (IWebElement comment in top5Comments)
                    {
                        string minute = comment.FindElement(By.XPath("span[@class=\"time\"]/span[@class=\"minute\"]")).GetAttribute("innerHTML").Replace("'", "");

                        if (minute.Contains(" + "))
                        {
                            int minuteTreat = Convert.ToInt32(minute.Split(" + ")[0]);
                            int extraTime = Convert.ToInt32(minute.Split(" + ")[1]);

                            minute = Convert.ToString(minuteTreat + extraTime);
                        }

                        string seconds = comment.FindElement(By.XPath("span[@class=\"time\"]/span[@class=\"seconds\"]")).GetAttribute("innerHTML").Replace("'", "");

                        string description = comment.FindElement(By.XPath("span[@class=\"comment_data\"]")).GetAttribute("innerHTML");

                        string time = minute != null && seconds != null ? $"{minute}:{seconds}" : "-";

                        if (time == ":")
                        {
                            if (description == "Full Time")
                                time = "90:00";
                            else
                                time = "45:00";
                        }

                        RadarInfo existsComment = _comments.Where(c => c.Minute == time && c.Description == description).FirstOrDefault();

                        if (existsComment == null)
                        {
                            RadarInfo commentModel = new RadarInfo(_fixture.WilliamHillId.Value, time, description);
                            _comments.Enqueue(commentModel);
                        }
                    }

                }
            }
            catch
            {

            }
        }

        #endregion

        #endregion

        #region FootballAPI

        #region Initialize

        private void InitializeFootballApi()
        {
            _referenceOdds = new Queue<ReferenceOddInfo>();
        }

        #endregion

        #region Task

        private void GetReferenceOdd()
        {
            OddsResponse odds = _mongoDB.GetFixtureOdd(_fixture.FootballApiId);

            if (odds == null)
                return;

            ReferenceOddInfo oddInfo = new ReferenceOddInfo(odds.Fixture, odds.Odds);

            ReferenceOddInfo existsOdd = _referenceOdds.Where(f =>
                f.FootballApiId == oddInfo.FootballApiId &&
                f.Minute == oddInfo.Minute &&
                f.HomeOdd == oddInfo.HomeOdd &&
                f.DrawOdd == oddInfo.DrawOdd &&
                f.AwayOdd == oddInfo.AwayOdd).FirstOrDefault();

            if (existsOdd == null)
                _referenceOdds.Enqueue(oddInfo);
        }

        #endregion

        #endregion

        #region Betfair

        #region Initialize

        private async Task InitializeBetfairApi(int eventId)
        {
            _event = await _betfairAPI.GetEvent(eventId.ToString());

            _betfairScrapperMatch = new BetfairScrapperMatch(_event.Event.Name, Convert.ToInt32(_event.Event.Id), _fixture.MarketId);
            _betfairScrapper = new BetfairScrapper(_mongoDB, _betfairAPI, _betfairScrapperMatch);
            _betfairScrapperOdds = new Queue<BetfairScrapperExecution>();
        }

        #endregion

        #region Task

        private void GetBetfairScrapperOdds(BetfairScrapperExecution execution)
        {
            _betfairScrapper.Scrap(execution);
            _betfairScrapperOdds.Enqueue(execution);
        }

        #endregion

        #endregion

        #endregion
    }
}
