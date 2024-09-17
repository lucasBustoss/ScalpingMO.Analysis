using FuzzySharp;
using ScalpingMO.Analysis.Analysis.ConsolidateData.Worker;
using SimMetrics.Net.Metric;

namespace ScalpingMO.Analysis.Analysis.ConsolidateData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataWorker worker = new DataWorker();
            worker.Execute();
        }
    }
}
