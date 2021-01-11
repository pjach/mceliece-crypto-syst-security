﻿using McEliece.Cryptosystem.Security.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace McEliece.Cryptosystem.Security.Statistics
{
    public class FailureUnderRelatedMessageStatistics
    {
        public void CalculateAndPrintStatistics(string data)
        {
            var statisticsList = JsonConvert.DeserializeObject<List<StatisticsEntry>>(data);

            long averageTime = 0;
            int averageL0SetSize = 0;
            int averageTotalIterations = 0;
            int averageSuccessfulEliminations = 0;
            var totalGuessesUntilFoundErrorFreeColumnsSum = new List<int>();
            int averageTotalGuessesUntilFound;
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

            averageTotalGuessesUntilFound = totalGuessesUntilFoundErrorFreeColumnsSum
                .Take(totalGuessesUntilFoundErrorFreeColumnsSum.Count)
                .Sum()
                / totalGuessesUntilFoundErrorFreeColumnsSum.Count;

            System.Console.WriteLine("Average guesses until error-free columns were selected for each L0 set size");
            foreach (AverageEntry average in finalL0Averages)
            {
                average.Average = average.Sum / average.Count;
                System.Console.WriteLine(average.L0SetCount + ", " + average.Average);
            }

            System.Console.WriteLine("Average Time:" + System.Environment.NewLine + averageTime);
            System.Console.WriteLine("Average total iterations:" + System.Environment.NewLine
                + averageTotalIterations);
            System.Console.WriteLine("Average L0 set size:" + System.Environment.NewLine
                + averageL0SetSize);
            System.Console.WriteLine("Average successful eliminations:" + System.Environment.NewLine
                + averageSuccessfulEliminations);
            System.Console.WriteLine("Average guesses until error free columns were selected:"
                + System.Environment.NewLine + averageTotalGuessesUntilFound);

            System.Console.WriteLine("Density of occurrencies and the percentage"
                + " of total iterations across L0 set count:");

            foreach (AverageEntry average in finalL0Averages)
            {
                float percentage = (float)average.L0SetCountOccurrences / (float)statisticsList.Count * 100;
                System.Console.WriteLine(average.L0SetCount + ": " + average.L0SetCountOccurrences
                    + ", " + percentage + "%");
            }
        }
    }
}