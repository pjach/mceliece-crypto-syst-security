using Combinatorics.Collections;
using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Models;
using MIF.VU.PJach.McElieceSecurity.Utilities;
using MIF.VU.PJach.McElieceSecurity.Utilities.Helpers;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MIF.VU.PJach.McElieceSecurity.Attacks
{
    public class FLWCAttack
    {
        private readonly Randomizer Randomizer = Randomizer.Instance;
        private readonly Stopwatch MainStopwatch = new Stopwatch();
        private readonly Stopwatch PeriodicStopwatch = new Stopwatch();
        private readonly ILogger _logger;

        public FLWCAttack(ILogger logger)
        {
            _logger = logger;
        }

        public FlwcAttackStatisticsEntry PrepareAndExecuteFlwcAttack(Matrix<float> publicMatrix, int parameterP, int parameterSigma, int weightOfVector)
        {
            var randomVector = RandomnessHelper.GenerateRandomVector(publicMatrix.RowCount, Randomizer.Instance);
            var errorFreeVector = CalculationHelper.MultipyMatrixWithVector(publicMatrix, randomVector);
            var errorVector = RandomnessHelper.GenerateErrorVector(publicMatrix.ColumnCount, weightOfVector, Randomizer.Instance);
            var finalSumVector = VectorHelper.AddVectorMod2(errorFreeVector, errorVector);

            var finalMatrix = publicMatrix.InsertRow(publicMatrix.RowCount, finalSumVector);

            return FlwcAttack(publicMatrix, finalMatrix, parameterP, parameterSigma, weightOfVector, finalSumVector);
        }

        public FlwcAttackStatisticsEntry FlwcAttack(Matrix<float> notAlteredPublicMatrix, Matrix<float> publicMatrix, int parameterP, int paramaterSigma, int weightOfErrorVector, Vector<float> receivedVector)
        {
            var result = new FlwcAttackStatisticsEntry();

            MainStopwatch.Restart();
            //Generating information set
            var informationSet = RandomnessHelper.GenerateRandomIndices(publicMatrix.ColumnCount - 1, publicMatrix.RowCount, Randomizer);

            //Applying Gaussian elimination for public matrix
            PeriodicStopwatch.Restart();
            var appliedGausianForPublicMatrix = MatrixHelper.GaussianEliminationForFLWC(publicMatrix, informationSet, Randomizer);
            PeriodicStopwatch.Stop();
            //result.InitialGaussianEliminationTime = PeriodicStopwatch.ElapsedMilliseconds / 1000;
            result.InitialGaussianEliminationTime = PeriodicStopwatch.ElapsedMilliseconds;

            //Forming a z set
            var zSet = CalculationHelper.CalculateZSet(informationSet, publicMatrix.ColumnCount);

            var counter = 0;
            while (true)
            {
                //Generating L set
                var lSet = RandomnessHelper.GenerateKnumberOfRandomColumnsIndices(paramaterSigma, zSet, Randomizer);
                //Forming a z matrix and updated L set
                var constructionOfZMatrixResult = MatrixHelper.ConstructMatrixFromIndices(appliedGausianForPublicMatrix, zSet, lSet);
                var zMatrix = constructionOfZMatrixResult.ZMatrix;
                lSet = constructionOfZMatrixResult.LIndicesEquivalent;

                //Generating a list of random indices for shuffling information set
                var randomIndicesForInformationSet = RandomnessHelper.GenerateRandomIndices(informationSet.Count - 1,
                                                                                    informationSet.Count / 2, Randomizer);

                var informationSet1 = new List<int>();
                var informationSet2 = new List<int>();
                var zSet1 = new List<int>();
                var zSet2 = new List<int>();

                //Populating information set 1
                for (int i = 0; i < informationSet.Count / 2; i++)
                {
                    informationSet2.Add(informationSet[randomIndicesForInformationSet[i]]);
                    zSet2.Add(informationSet.IndexOf(informationSet[randomIndicesForInformationSet[i]]));
                }

                //Populating information set 2
                for (int i = 0; i < informationSet.Count; i++)
                {
                    if (!informationSet2.Contains(informationSet[i]))
                    {
                        informationSet1.Add(informationSet[i]);
                        zSet1.Add(i);
                    }
                }

                var hashTable1 = new Dictionary<string, List<ZMatrixRowsAndTheirIndices>>();
                var hashTable2 = new Dictionary<string, List<ZMatrixRowsAndTheirIndices>>();

                var combinationsForZ1 = new Combinations<int>(zSet1, parameterP);
                var combinationsForZ2 = new Combinations<int>(zSet2, parameterP);

                //Populating hash table 1
                foreach (IList<int> combinations in combinationsForZ1)
                {
                    if (parameterP == 1)
                    {
                        if (hashTable1.TryGetValue(CalculationHelper.GetHashTableKeyForFLWC(lSet, zMatrix.Row(combinations[0])), out var list))
                        {
                            list.Add(new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = zMatrix.Row(combinations[0]),
                                ZMatrixRowsIndices = combinations
                            });
                        }
                        else
                        {
                            hashTable1.Add(CalculationHelper.GetHashTableKeyForFLWC(lSet, zMatrix.Row(combinations[0])), new List<ZMatrixRowsAndTheirIndices>()
                        {
                            new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = zMatrix.Row(combinations[0]),
                                ZMatrixRowsIndices = combinations
                            }
                        });
                        }
                    }
                    else
                    {
                        var preSum = VectorHelper.AddVectorMod2(zMatrix.Row(combinations[0]), zMatrix.Row(combinations[1]));
                        for (int i = 2; i < parameterP; i++)
                        {
                            preSum = VectorHelper.AddVectorMod2(preSum, zMatrix.Row(combinations[i]));
                        }
                        if (hashTable1.TryGetValue(CalculationHelper.GetHashTableKeyForFLWC(lSet, preSum), out var list))
                        {
                            list.Add(new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = preSum,
                                ZMatrixRowsIndices = combinations
                            });
                        }
                        else
                        {
                            hashTable1.Add(CalculationHelper.GetHashTableKeyForFLWC(lSet, preSum), new List<ZMatrixRowsAndTheirIndices>()
                        {
                            new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = preSum,
                                ZMatrixRowsIndices = combinations
                            }
                        });
                        }
                    }
                }
                //Populating hash table 2
                foreach (IList<int> combinations in combinationsForZ2)
                {
                    if (parameterP == 1)
                    {
                        if (hashTable2.TryGetValue(CalculationHelper.GetHashTableKeyForFLWC(lSet, zMatrix.Row(combinations[0])), out var list))
                        {
                            list.Add(new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = zMatrix.Row(combinations[0]),
                                ZMatrixRowsIndices = combinations
                            });
                        }
                        else
                        {
                            hashTable2.Add(CalculationHelper.GetHashTableKeyForFLWC(lSet, zMatrix.Row(combinations[0])), new List<ZMatrixRowsAndTheirIndices>()
                        {
                            new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = zMatrix.Row(combinations[0]),
                                ZMatrixRowsIndices = combinations
                            }
                        });
                        }
                    }
                    else
                    {
                        var preSum = VectorHelper.AddVectorMod2(zMatrix.Row(combinations[0]), zMatrix.Row(combinations[1]));
                        for (int i = 2; i < parameterP; i++)
                        {
                            preSum = VectorHelper.AddVectorMod2(preSum, zMatrix.Row(combinations[i]));
                        }
                        if (hashTable2.TryGetValue(CalculationHelper.GetHashTableKeyForFLWC(lSet, preSum), out var list))
                        {
                            list.Add(new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = preSum,
                                ZMatrixRowsIndices = combinations
                            });
                        }
                        else
                        {
                            hashTable2.Add(CalculationHelper.GetHashTableKeyForFLWC(lSet, preSum), new List<ZMatrixRowsAndTheirIndices>()
                        {
                            new ZMatrixRowsAndTheirIndices()
                            {
                                ZMatrixRow = preSum,
                                ZMatrixRowsIndices = combinations
                            }
                        });
                        }
                    }
                }

                var hashTable1Keys = hashTable1.Keys;
                var hashTable2Keys = hashTable2.Keys;
                var conditionWeight = weightOfErrorVector - 2 * parameterP;

                foreach (var key in hashTable1Keys)
                {
                    if (hashTable2Keys.Contains(key))
                    {
                        var hashTable1KeyVectors = hashTable1.GetValueOrDefault(key);
                        var hashTable2KeyVectors = hashTable2.GetValueOrDefault(key);
                        foreach (var zMatrixRowsAndTheirIndices1 in hashTable1KeyVectors)
                        {
                            foreach (var zMatrixRowsAndTheirIndices2 in hashTable2KeyVectors)
                            {
                                var sum = VectorHelper.AddVectorMod2(zMatrixRowsAndTheirIndices1.ZMatrixRow,
                                     zMatrixRowsAndTheirIndices2.ZMatrixRow);

                                if (VectorHelper.GetVectorWeight(sum) == conditionWeight)
                                {
                                    var finalListOfRowsIndices = new List<int>();
                                    finalListOfRowsIndices.AddRange(zMatrixRowsAndTheirIndices1.ZMatrixRowsIndices);
                                    finalListOfRowsIndices.AddRange(zMatrixRowsAndTheirIndices2.ZMatrixRowsIndices);
                                    var guessedErrorVector = VectorHelper.AddVectorMod2(
                                            appliedGausianForPublicMatrix.Row(finalListOfRowsIndices[0]),
                                            appliedGausianForPublicMatrix.Row(finalListOfRowsIndices[1]));

                                    if (finalListOfRowsIndices.Count > 2)
                                    {
                                        for (int i = 2; i < finalListOfRowsIndices.Count; i++)
                                        {
                                            guessedErrorVector = VectorHelper.AddVectorMod2(
                                                guessedErrorVector,
                                                appliedGausianForPublicMatrix.Row(finalListOfRowsIndices[i]));
                                        }
                                    }
                                    var errorFreeVector = VectorHelper.AddVectorMod2(guessedErrorVector, receivedVector);
                                    informationSet.RemoveAt(0);
                                    var matrixCopy = Matrix<float>.Build.Dense(notAlteredPublicMatrix.RowCount, notAlteredPublicMatrix.ColumnCount);
                                    notAlteredPublicMatrix.CopyTo(matrixCopy);
                                    MatrixHelper.GaussianEliminationForFLWC(matrixCopy, informationSet, Randomizer);
                                    var finalSolution = MatrixHelper.ConstructTemporaryMatrix(informationSet, notAlteredPublicMatrix, errorFreeVector);
                                    PeriodicStopwatch.Restart();
                                    var gaussianElimination = MatrixHelper.IsEliminableByGaussian(finalSolution);

                                    PeriodicStopwatch.Stop();
                                    MainStopwatch.Stop();
                                    //Matrix's equation answer is the original message.
                                    //Saving statistics and stopping the attack

                                    //result.SpentTime = MainStopwatch.ElapsedMilliseconds / 1000;
                                    result.SpentTime = MainStopwatch.ElapsedMilliseconds;
                                    result.TotalIterationsCount = counter + 1;
                                    //result.FinalGaussianEliminationTime = PeriodicStopwatch.ElapsedMilliseconds / 1000;
                                    result.FinalGaussianEliminationTime = PeriodicStopwatch.ElapsedMilliseconds;

                                    return result;
                                }
                            }
                        }
                    }
                }

                //Guessing of error vector was not successfull.
                //Trying again.
                var indexForChange = CalculationHelper.GetIndexOfApplicableMatrixColumn(appliedGausianForPublicMatrix, informationSet, zSet,
                                                                                                               Randomizer);
                appliedGausianForPublicMatrix = MatrixHelper.ApplyGaussianEliminationForAColumn(appliedGausianForPublicMatrix, indexForChange);

                counter++;
            }
        }
    }
}