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
            //Calculates L0 and L1 sets
            CalculationHelper.CalculateLocationSets(L0Set, L1Set, interceptedVector1, interceptedVector2);

            int rowCount = publicKey.RowCount;

            int totalIterationsCount = 1;
            int linearlyIndependentColumnsGuesses = 0;
            int numberOfSuccessfulGaussianEliminations = 0;

            while (true)
            {
                linearlyIndependentColumnsGuesses++;
                //Generates random indices for the attack
                var randomIndicess = CalculationHelper.GenerateKnumberOfRandomColumnsIndices(rowCount, L0Set, randomizer);
                //Stops the stopwatch, checks whether at least one of the chosen indices
                //were garbled by the error vector, records it
                stopwatch.Stop();
                var areThereAnyGarbledIndices = CalculationHelper.AreThereAnyGarbledIndices(randomIndicess, errorVector1, errorVector2);
                if (areThereAnyGarbledIndices == false)
                {
                    statisticsEntry.GuessesUntilErrorFreeColumnsSelected.Add(linearlyIndependentColumnsGuesses);
                    linearlyIndependentColumnsGuesses = 0;
                }
                stopwatch.Start();
                //Builds temporary matrix for the Gaussian elimination
                var temporaryMatrix = CalculationHelper.ConstructTemporaryMatrix(randomIndicess,
                                                           publicKey, interceptedVector1);
                //Executes Gaussian elimination algorithm
                var IsEliminableByGaussian = CalculationHelper.IsEliminableByGaussian(temporaryMatrix);

                if (IsEliminableByGaussian)
                {
                    //Gaussian elimination was successful
                    numberOfSuccessfulGaussianEliminations++;

                    //Matrix's equation answer is multiplied with public matrix
                    //as a candidate to the original message
                    var messageCandidate = CalculationHelper.MultipyMatrixWithVector(publicKey,
                                                       temporaryMatrix.Column(rowCount));
                    //Checking the Hamming distance between the candidate and ciphered vector
                    var hammingDistance = CalculationHelper.GetHammingDistance(interceptedVector1, messageCandidate);

                    if (hammingDistance == errorVectorWeight)
                    {
                        //Matrix's equation answer is the original message.
                        //Saving statistics and stopping the attack
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

        public StatisticsEntry PrepareDataAndAttemptResendAttack(int amountOfErrors, Matrix<float> publicKey)
        {
            //Generating random original message
            var messageVector = CalculationHelper.GenerateRandomVector(publicKey.ColumnCount, randomizer);

            //Encrypting original message
            var errorFreeVector = CalculationHelper.MultipyMatrixWithVector(publicKey, messageVector);

            //Generating error vectors
            var errorVector1 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);
            var errorVector2 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);

            //Adding error vectors to the encrypted messages
            var interceptedVector1 = CalculationHelper.AddVectorMod2(errorFreeVector, errorVector1);
            var interceptedVector2 = CalculationHelper.AddVectorMod2(errorFreeVector, errorVector2);

            //Initiate message resend attack
            return FailureUnderMessageResendAttack(amountOfErrors, interceptedVector1, interceptedVector2, publicKey, errorVector1, errorVector2);
        }

        public StatisticsEntry PrepareDataAndAttemptRelatedAttack(int amountOfErrors, Matrix<float> publicKey)
        {
            //Generating random original messages
            var messageVector1 = CalculationHelper.GenerateRandomVector(publicKey.ColumnCount, randomizer);
            var messageVector2 = CalculationHelper.GenerateRandomVector(publicKey.ColumnCount, randomizer);

            //Counting the sum of the original messages
            var messagesSum = CalculationHelper.AddVectorMod2(messageVector1, messageVector2);

            //Encrypting original messages
            var errorFreeVector1 = CalculationHelper.MultipyMatrixWithVector(publicKey, messageVector1);
            var errorFreeVector2 = CalculationHelper.MultipyMatrixWithVector(publicKey, messageVector2);

            //Encrypting original messages sum
            var sumVector = CalculationHelper.MultipyMatrixWithVector(publicKey, messagesSum);

            //Generating error vectors
            var errorVector1 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);
            var errorVector2 = CalculationHelper.GenerateErrorVector(publicKey.ColumnCount, amountOfErrors, randomizer);

            //Adding error vectors to the encrypted messages
            var interceptedVector1 = CalculationHelper.AddVectorMod2(errorFreeVector1, errorVector1);
            var interceptedVector2 = CalculationHelper.AddVectorMod2(errorFreeVector2, errorVector2);

            //Meeting related message attack conditions
            var finalSumVector = CalculationHelper.AddVectorMod2(interceptedVector2, sumVector);

            //Executing related message attack
            return FailureUnderMessageResendAttack(amountOfErrors, interceptedVector1, finalSumVector, publicKey, errorVector1, errorVector2);
        }
    }
}