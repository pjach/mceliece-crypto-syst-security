using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Models;
using MIF.VU.PJach.McElieceSecurity.Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace MIF.VU.PJach.McElieceSecurity.Attacks
{
    public class RelatedMessageAttacks
    {
        private readonly Randomizer randomizer = Randomizer.Instance;
        private readonly Stopwatch stopwatch = new Stopwatch();

        public StatisticsEntry FailureUnderMessageResendAttack(int errorVectorWeight, Vector<float> interceptedVector1,
          Vector<float> interceptedVector2, Matrix<float> publicKey, Vector<float> errorVector1, Vector<float> errorVector2)
        {
            var L0Set = new List<int>();
            var L1Set = new List<int>();
            var statisticsEntry = new StatisticsEntry();

            stopwatch.Restart();
            stopwatch.Start();
            CalculationHelper.CalculateLocationSets(L0Set, L1Set, interceptedVector1, interceptedVector2);

            int rowCount = publicKey.RowCount;

            int totalIterationsCount = 1;
            int linearlyIndependentColumnsGuesses = 0;
            int numberOfSuccessfulGaussianEliminations = 0;
            while (true)
            {
                linearlyIndependentColumnsGuesses++;
                var randomIndicess = CalculationHelper.GenerateKnumberOfRandomColumnsIndices(rowCount, L0Set, randomizer);
                stopwatch.Stop();
                var areThereAnyGarbledIndices = CalculationHelper.AreThereAnyGarbledIndices(randomIndicess, errorVector1, errorVector2);
                if (areThereAnyGarbledIndices == false)
                {
                    statisticsEntry.GuessesUntilErrorFreeColumnsSelected.Add(linearlyIndependentColumnsGuesses);
                    linearlyIndependentColumnsGuesses = 0;
                }
                stopwatch.Start();
                var temporaryMatrix = CalculationHelper.ConstructTemporaryMatrix(randomIndicess,
                                                           publicKey, interceptedVector1);
                var IsEliminableByGaussian = CalculationHelper.IsEliminableByGaussian(temporaryMatrix);

                if (IsEliminableByGaussian)
                {
                    numberOfSuccessfulGaussianEliminations++;

                    var messageCandidate = CalculationHelper.MultipyMatrixWithVector(publicKey,
                                                       temporaryMatrix.Column(rowCount));
                    var hammingDistance = CalculationHelper.GetHammingDistance(interceptedVector1, messageCandidate);

                    if (hammingDistance == errorVectorWeight)
                    {
                        stopwatch.Stop();
                        statisticsEntry.SpentTime = stopwatch.ElapsedMilliseconds / 1000;
                        statisticsEntry.TotalIterationsCount = totalIterationsCount;
                        statisticsEntry.NumberOfSuccessfulGaussianEliminations = numberOfSuccessfulGaussianEliminations;
                        statisticsEntry.L0SetCount = L0Set.Count;
                        statisticsEntry.L1SetCount = L1Set.Count;
                        return statisticsEntry;
                    }
                }

                totalIterationsCount++;
            }
        }

        public StatisticsEntry PrepareDataAndAttemptResendAttack(Vector<float> vectorMessage, int amountOfErrors, Matrix<float> publicKey)
        {
            var errorFreeVector = CalculationHelper.MultipyMatrixWithVector(publicKey, vectorMessage);

            var errorVector1 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);
            var errorVector2 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);

            var interceptedVector1 = CalculationHelper.AddVectorMod2(errorFreeVector, errorVector1);
            var interceptedVector2 = CalculationHelper.AddVectorMod2(errorFreeVector, errorVector2);

            return FailureUnderMessageResendAttack(amountOfErrors, interceptedVector1, interceptedVector2, publicKey, errorVector1, errorVector2);
        }

        public StatisticsEntry PrepareDataAndAttemptRelatedAttack(Vector<float> messageVector1, Vector<float> messageVector2,
                                                                                 int amountOfErrors, Matrix<float> publicKey)
        {
            var messagesSum = CalculationHelper.AddVectorMod2(messageVector1, messageVector2);

            var errorFreeVector1 = CalculationHelper.MultipyMatrixWithVector(publicKey, messageVector1);
            var errorFreeVector2 = CalculationHelper.MultipyMatrixWithVector(publicKey, messageVector2);
            var thirdVector = CalculationHelper.MultipyMatrixWithVector(publicKey, messagesSum);

            var errorVector1 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);
            var errorVector2 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);

            var interceptedVector1 = CalculationHelper.AddVectorMod2(errorFreeVector1, errorVector1);
            var interceptedVector2 = CalculationHelper.AddVectorMod2(errorFreeVector2, errorVector2);
            var preSum = CalculationHelper.AddVectorMod2(interceptedVector2, thirdVector);

            return FailureUnderMessageResendAttack(amountOfErrors, interceptedVector1, preSum, publicKey, errorVector1, errorVector2);
        }
    }
}