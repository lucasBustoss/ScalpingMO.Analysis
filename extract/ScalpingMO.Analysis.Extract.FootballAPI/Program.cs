using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.FootballAPI.Worker;

namespace ScalpingMO.Analysis.Extract.FootballAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())  // Diretório base onde o appsettings está
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // Carrega o appsettings.json
               .Build();

            string url = config["FootballAPI:Url"];
            string apiKey = config["FootballAPI:ApiKey"];
            string apiHost = config["FootballAPI:ApiHost"];

            DataWorker worker = new DataWorker(url, apiKey, apiHost);
            worker.Execute();
        }
    }
}
