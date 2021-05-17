using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Models;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;

namespace MIF.VU.PJach.McElieceSecurity.Statistics
{
    public class FlwcStatistics : IStatistics
    {
        private readonly ILogger _logger;

        public FlwcStatistics(ILogger logger)
        {
            _logger = logger;
        }

        public void PrintStatistics(string data)
        {
            var statisticsList = JsonConvert.DeserializeObject<List<FlwcAttackStatisticsEntry>>(data);

            double averageTime = 0;
            double averageInitialGaussianEliminationTime = 0;
            double averageTotalIterationsCount = 0;
            double averageFinalGaussianEliminationTime = 0;

            foreach (var statisticsEntry in statisticsList)
            {
                averageTime += statisticsEntry.SpentTime;
                averageInitialGaussianEliminationTime += statisticsEntry.InitialGaussianEliminationTime;
                averageFinalGaussianEliminationTime += statisticsEntry.FinalGaussianEliminationTime;
                averageTotalIterationsCount += statisticsEntry.TotalIterationsCount;
            }

            var statisticsCount = statisticsList.Count;
            _logger.Debug($"Average total spent time:{averageTime / statisticsCount} ms" + System.Environment.NewLine);
            _logger.Debug($"Average total iterations:{averageTotalIterationsCount / statisticsCount}" + System.Environment.NewLine);
            _logger.Debug($"Average initial Gausian elimination time:{averageInitialGaussianEliminationTime / statisticsCount} ms" + System.Environment.NewLine);
            _logger.Debug($"Average final Gaussian elimination time:{averageFinalGaussianEliminationTime / statisticsCount} ms" + System.Environment.NewLine);
        }
    }
}