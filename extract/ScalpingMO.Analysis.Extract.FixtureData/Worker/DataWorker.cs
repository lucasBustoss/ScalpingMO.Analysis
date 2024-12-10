using Microsoft.Playwright;
using ScalpingMO.Analysis.Extract.FixtureData.Data;
using ScalpingMO.Analysis.Extract.FixtureData.Data.Betfair;
using ScalpingMO.Analysis.Extract.FixtureData.Models;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.BetfairAPI.Scrapper;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response;
using ScalpingMO.Analysis.Extract.FixtureData.Models.FootballAPI.Response.Odds;
using ScalpingMO.Analysis.Extract.FixtureData.Models.Radar;

namespace ScalpingMO.Analysis.Extract.FixtureData.Worker
{
    public class DataWorker
    {
        #region Radar

        private IPage _page;
        private IBrowser _browser; 
        Queue<RadarInfo> _comments;
        private ILocator _commentBox;

        #endregion

        #region FootballApi

        private Queue<ReferenceOddInfo> _referenceOdds;
        private int _thresholdExtract = 200;
        private int _thresholdSave = 500;

        #endregion

        #region Betfair

        private BetfairAPIService _betfairAPI;
        private BetfairScrapper _betfairScrapper;
        private BetfairEventResponse _event;
        private Queue<BetfairScrapperMatch> _betfairScrapperOdds;
        private BetfairScrapperMatch _betfairScrapperMatch;
        private string _matchName;
        private string _eventId;
        private string _matchId;

        #endregion

        private Fixture _fixture;
        private MongoDBService _mongoDB;
        private bool _matchFinished = false;

        /// <summary>
        /// Parametro pra teste. Depois de tudo desenvolvido vou remvoer
        /// </summary>
        private int _tries = 0;
        private int _triesThreshold = 100000;

        public DataWorker(MongoDBService mongoDB, BetfairAPIService betfairApi, IBrowser browser)
        {
            _mongoDB = mongoDB;
            _betfairAPI = betfairApi;
            _browser = browser;
        }

        public async Task Execute(Fixture fixture)
        {
            _fixture = fixture;

            string matchName = $"{fixture.HomeTeamName} x {fixture.AwayTeamName}";
            Console.WriteLine($"Iniciando a extração dos dados do jogo {matchName}");

            await InitializeBetfairApi(fixture.BetfairId.Value);
            await InitializeScraperAsync(fixture.RadarUrl);
            InitializeFootballApi();

            Task getCommentTask = ExecuteGetCommentAsync();
            Task betfairApiTask = ExecuteBetfairApiAsync();
            Task footballApiTask = ExecuteFootballApiAsync();

            Task saveRadarCommentsTask = ExecuteSaveRadarCommentsAsync();
            Task saveReferenceOddTask = ExecuteSaveReferenceOddAsync();
            Task saveBetfairOddTask = ExecuteSaveBetfairOddsAsync();

            await Task.WhenAll(getCommentTask, betfairApiTask, footballApiTask, saveRadarCommentsTask, saveReferenceOddTask, saveBetfairOddTask);

            await CloseScraperAsync();

            Console.WriteLine($"Fim da extração dos dados do jogo {matchName}.");
        }

        #region Private methods

        #region Tasks

        private async Task ExecuteGetCommentAsync()
        {
            while (!_matchFinished && _tries < _triesThreshold)
            {
                await GetCommentAsync();
                await Task.Delay(1000);
            }
        }

        private async Task ExecuteBetfairApiAsync()
        {
            while (!_matchFinished && _tries < _triesThreshold)
            {
                BetfairScrapperMatch match = new BetfairScrapperMatch(_event.Event.Name, Convert.ToInt32(_event.Event.Id), _fixture.MarketId);
                await GetBetfairScrapperOdds(match);

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
                    BetfairScrapperMatch match = _betfairScrapperOdds.Dequeue();
                    _mongoDB.SaveBetfairScrapperMatch(match);
                }

                await Task.Delay(_thresholdSave);
            }
        }

        #endregion

        #region Scrapper

        #region Initialize 

        public async Task InitializeScraperAsync(string url)
        {
            _comments = new Queue<RadarInfo>();
            await StartDriverAsync();

            // Navegar para a URL
            await _page.GotoAsync(url);

            var balloonButton = _page.Locator("//*[@data-action='commentaries']/a");
            await balloonButton.WaitForAsync(new LocatorWaitForOptions
            {
                Timeout = 10000
            });

            await balloonButton.ClickAsync();
        }

        private async Task StartDriverAsync()
        {
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        private async Task CloseScraperAsync()
        {
            await _browser.CloseAsync();
        }

        #endregion

        #region Task 

        public async Task GetCommentAsync()
        {
            try
            {
                _commentBox = _page.Locator("//*[@id='box_commentaries']");
                await _commentBox.WaitForAsync(new LocatorWaitForOptions
                {
                    Timeout = 10000
                });

                var listComments = await _commentBox.Locator("ul > li").AllAsync();

                if (listComments != null && listComments.Any())
                {
                    var top5Comments = listComments.Take(6);

                    foreach (var comment in top5Comments)
                    {
                        // Usando o seletor CSS em vez de XPath
                        var minuteElement = await comment.Locator("span.time > span.minute").TextContentAsync();
                        var minute = minuteElement.Replace("'", "");

                        if (minute.Contains(" + "))
                        {
                            int minuteTreat = Convert.ToInt32(minute.Split(" + ")[0]);
                            int extraTime = Convert.ToInt32(minute.Split(" + ")[1]);

                            minute = Convert.ToString(minuteTreat + extraTime);
                        }

                        var secondsElement = await comment.Locator("span.time > span.seconds").TextContentAsync();
                        string seconds = secondsElement.Replace("'", "");
                        var description = await comment.Locator("span.comment_data").TextContentAsync();

                        var time = !string.IsNullOrEmpty(minute) && !string.IsNullOrEmpty(seconds) ? $"{minute}:{seconds}" : "-";

                        if (time == ":")
                        {
                            if (description == "Full Time")
                                time = "90:00";
                            else
                                time = "45:00";
                        }

                        var existsComment = _comments.FirstOrDefault(c => c.Minute == time && c.Description == description);

                        if (existsComment == null)
                        {
                            RadarInfo commentModel = new RadarInfo(_fixture.WilliamHillId.Value, time, description);
                            _comments.Enqueue(commentModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter os comentários: {ex.Message}");
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
            _betfairScrapperOdds = new Queue<BetfairScrapperMatch>();

            _betfairScrapper = new BetfairScrapper(_mongoDB, _betfairAPI, _browser, _event.Event.Name, _event.Event.Id, _fixture.MarketId);
            await _betfairScrapper.StartDriver();
        }

        #endregion

        #region Task

        private async Task GetBetfairScrapperOdds(BetfairScrapperMatch match)
        {
            await _betfairScrapper.ScrapAsync(match);
            _betfairScrapperOdds.Enqueue(match);
        }

        #endregion

        #endregion

        #endregion
    }
}
