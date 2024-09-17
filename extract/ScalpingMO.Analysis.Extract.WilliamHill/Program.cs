using Microsoft.Extensions.Configuration;
using ScalpingMO.Analysis.Extract.WilliamHill.Worker;

namespace ScalpingMO.Analysis.Extract.WilliamHill
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())  // Diretório base onde o appsettings está
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // Carrega o appsettings.json
               .Build();

            string url = config["WilliamHill:Url"];

            DataWorker worker = new DataWorker(url);
            worker.Execute();
        }
    }
}
