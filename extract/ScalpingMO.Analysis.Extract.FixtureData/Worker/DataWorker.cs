using ScalpingMO.Analysis.Extract.FixtureData.Data;
using ScalpingMO.Analysis.Extract.FixtureData.Models;

namespace ScalpingMO.Analysis.Extract.FixtureData.Worker
{
    public class DataWorker
    {
        private MongoDBService _mongoDB;
        private bool _matchFinished = false;

        /// <summary>
        /// Parametro pra teste. Depois de tudo desenvolvido vou remvoer
        /// </summary>
        private int _tries = 0;

        public DataWorker()
        {
            _mongoDB = new MongoDBService();
        }

        public void Execute(Fixture fixture)
        {
            string matchName = $"{fixture.HomeTeamName} x {fixture.AwayTeamName}";
            
            Console.WriteLine($"Iniciando a extração dos dados do jogo {matchName}");
            
            while (!_matchFinished && _tries < 10)
            {
                _tries++;

                Thread.Sleep(200);
            }

            Console.WriteLine($"Fim da extração dos dados do jogo {matchName}.");
        }
    }
}
