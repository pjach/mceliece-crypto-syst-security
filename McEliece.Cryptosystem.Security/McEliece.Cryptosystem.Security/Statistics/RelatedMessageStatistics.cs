using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Models;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace MIF.VU.PJach.McElieceSecurity.Statistics
{
    public class RelatedMessageStatistics
    {
        private readonly ILogger _logger;

        public RelatedMessageStatistics(ILogger logger)
        {
            _logger = logger;
        }

        public void CalculateAndPrintStatistics(string data)
        {
            var statisticsList = JsonConvert.DeserializeObject<List<StatisticsEntry>>(data);

            double averageTime = 0;
            double averageL0SetSize = 0;
            double averageTotalIterations = 0;
            double averageSuccessfulEliminations = 0;
            var totalGuessesUntilFoundErrorFreeColumnsSum = new List<int>();
            double averageTotalGuessesUntilFound;
            var L0setSizes = new List<int>();
            var L0Averages = new List<AverageEntry>();

            for (int i = 0; i < statisticsList.Count; i++)
            {
                averageTime += statisticsList[i].SpentTime;

                averageTotalIterations += statisticsList[i].TotalIterationsCount;

                averageL0SetSize += statisticsList[i].L0SetCount;

                averageSuccessfulEliminations += statisticsList[i].NumberOfSuccessfulGaussianEliminations;

                totalGuessesUntilFoundErrorFreeColumnsSum
                    .AddRange(statisticsList[i].GuessesUntilErrorFreeColumnsSelected);

                if (!L0setSizes.Contains(statisticsList[i].L0SetCount))
                {
                    L0setSizes.Add(statisticsList[i].L0SetCount);
                    L0Averages.Add(new AverageEntry
                    {
                        Count = 0,
                        L0SetCount = statisticsList[i].L0SetCount,
                        Sum = 0,
                        L0SetCountOccurrences = 0
                    });
                }
            }

            for (int i = 0; i < statisticsList.Count; i++)
            {
                var entr1 = L0Averages.Where(e => e.L0SetCount == statisticsList[i].L0SetCount).First();

                entr1.L0SetCountOccurrences++;

                entr1.Count += statisticsList[i].GuessesUntilErrorFreeColumnsSelected.Count;

                entr1.Sum += statisticsList[i].GuessesUntilErrorFreeColumnsSelected
                    .Take(statisticsList[i].GuessesUntilErrorFreeColumnsSelected.Count)
                    .Sum();
            }

            var finalL0Averages = L0Averages.OrderBy(e => e.L0SetCount);

            averageTime = averageTime / statisticsList.Count;

            averageTotalIterations = averageTotalIterations / statisticsList.Count;

            averageL0SetSize = averageL0SetSize / statisticsList.Count;

            averageSuccessfulEliminations = averageSuccessfulEliminations / statisticsList.Count;

            averageTotalGuessesUntilFound = (double)totalGuessesUntilFoundErrorFreeColumnsSum
                .Take(totalGuessesUntilFoundErrorFreeColumnsSum.Count)
                .Sum()
                / totalGuessesUntilFoundErrorFreeColumnsSum.Count;
            _logger.Debug("Average guesses until error-free columns were selected for each L0 set size");
            foreach (AverageEntry average in finalL0Averages)
            {
                average.Average = (double)average.Sum / average.Count;
                _logger.Debug(average.L0SetCount + ", " + average.Average);
            }

            _logger.Debug("Average Time:" + System.Environment.NewLine + averageTime);
            _logger.Debug("Average total iterations:" + System.Environment.NewLine
                + averageTotalIterations);
            _logger.Debug("Average L0 set size:" + System.Environment.NewLine
                + averageL0SetSize);
            _logger.Debug("Average successful eliminations:" + System.Environment.NewLine
                + averageSuccessfulEliminations);
            _logger.Debug("Average guesses until error free columns were selected:"
                + System.Environment.NewLine + averageTotalGuessesUntilFound);

            _logger.Debug("Density of occurrencies and the percentage"
                + " of total iterations across L0 set count:");

            foreach (AverageEntry average in finalL0Averages)
            {
                float percentage = (float)average.L0SetCountOccurrences / (float)statisticsList.Count * 100;
                _logger.Debug(average.L0SetCount + ": " + average.L0SetCountOccurrences
                    + ", " + percentage + "%");
            }
        }

        public void CalculateAndPrintStatistics(List<StatisticsEntry> statisticsList)
        {

            double averageTime = 0;
            double averageL0SetSize = 0;
            double averageTotalIterations = 0;
            double averageSuccessfulEliminations = 0;
            var totalGuessesUntilFoundErrorFreeColumnsSum = new List<int>();
            double averageTotalGuessesUntilFound;
            var L0setSizes = new List<int>();
            var L0Averages = new List<AverageEntry>();

            for (int i = 0; i < statisticsList.Count; i++)
            {
                averageTime += statisticsList[i].SpentTime;

                averageTotalIterations += statisticsList[i].TotalIterationsCount;

                averageL0SetSize += statisticsList[i].L0SetCount;

                averageSuccessfulEliminations += statisticsList[i].NumberOfSuccessfulGaussianEliminations;

                totalGuessesUntilFoundErrorFreeColumnsSum
                    .AddRange(statisticsList[i].GuessesUntilErrorFreeColumnsSelected);

                if (!L0setSizes.Contains(statisticsList[i].L0SetCount))
                {
                    L0setSizes.Add(statisticsList[i].L0SetCount);
                    L0Averages.Add(new AverageEntry
                    {
                        Count = 0,
                        L0SetCount = statisticsList[i].L0SetCount,
                        Sum = 0,
                        L0SetCountOccurrences = 0
                    });
                }
            }

            for (int i = 0; i < statisticsList.Count; i++)
            {
                var entr1 = L0Averages.Where(e => e.L0SetCount == statisticsList[i].L0SetCount).First();

                entr1.L0SetCountOccurrences++;

                entr1.Count += statisticsList[i].GuessesUntilErrorFreeColumnsSelected.Count;

                entr1.Sum += statisticsList[i].GuessesUntilErrorFreeColumnsSelected
                    .Take(statisticsList[i].GuessesUntilErrorFreeColumnsSelected.Count)
                    .Sum();
            }

            var finalL0Averages = L0Averages.OrderBy(e => e.L0SetCount);

            averageTime = averageTime / statisticsList.Count;

            averageTotalIterations = averageTotalIterations / statisticsList.Count;

            averageL0SetSize = averageL0SetSize / statisticsList.Count;

            averageSuccessfulEliminations = averageSuccessfulEliminations / statisticsList.Count;

            averageTotalGuessesUntilFound = (double)totalGuessesUntilFoundErrorFreeColumnsSum
                .Take(totalGuessesUntilFoundErrorFreeColumnsSum.Count)
                .Sum()
                / totalGuessesUntilFoundErrorFreeColumnsSum.Count;

            _logger.Debug("Average guesses until error-free columns were selected for each L0 set size");
            foreach (AverageEntry average in finalL0Averages)
            {
                average.Average = (double)average.Sum / average.Count;
                _logger.Debug(average.L0SetCount + ", " + average.Average);
            }

            _logger.Debug("Average Time:" + System.Environment.NewLine + averageTime);
            _logger.Debug("Average total iterations:" + System.Environment.NewLine
                + averageTotalIterations);
            _logger.Debug("Average L0 set size:" + System.Environment.NewLine
                + averageL0SetSize);
            _logger.Debug("Average successful eliminations:" + System.Environment.NewLine
                + averageSuccessfulEliminations);
            _logger.Debug("Average guesses until error free columns were selected:"
                + System.Environment.NewLine + averageTotalGuessesUntilFound);

            _logger.Debug("Density of occurrencies and the percentage"
                + " of total iterations across L0 set count:");

            foreach (AverageEntry average in finalL0Averages)
            {
                float percentage = (float)average.L0SetCountOccurrences / (float)statisticsList.Count * 100;
                _logger.Debug(average.L0SetCount + ": " + average.L0SetCountOccurrences
                    + ", " + percentage + "%");
            }
        }
    }
}