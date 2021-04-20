using Combinatorics.Collections;
using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Models;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MIF.VU.PJach.McElieceSecurity.Utilities
{
    public static class CalculationHelper
    {
        public static Matrix<float> ApplyGaussianEliminationForAColumn(Matrix<float> publicMatrix, ColumnAndRowIndices columnAndRowIndices)
        {
            int rowLength = publicMatrix.Row(0).Count;

            for (int row = 0; row < publicMatrix.RowCount; row++)
            {
                if (row != columnAndRowIndices.RowIndex)
                {
                    if (publicMatrix[row, columnAndRowIndices.ColumnIndex] == 1)
                    {
                        for (int i = 0; i < rowLength; i++)
                        {
                            publicMatrix[row, i] = AddBinaryNumbers(publicMatrix[row, i],
                                publicMatrix[columnAndRowIndices.RowIndex, i]);
                        }
                    }
                }
            }

            return publicMatrix;
        }

        public static int GetNonZeroIndexFromVector(Vector<float> vector, Randomizer randomizer)
        {
            while (true)
            {
                var randomIndex = randomizer.Next(vector.Count);
                if (vector[randomIndex] == 1)
                {
                    return randomIndex;
                }
            }
        }

        public static bool IsZeroVector(Vector<float> vector)
        {
            return !vector.Any(zero => zero.Equals(1));
        }

        public static int GenerateRandomIndex(IList<int> list, Randomizer randomizer)
        {
            return randomizer.Next(list.Count);
        }

        public static ColumnAndRowIndices GetIndexOfApplicableMatrixColumn(Matrix<float> publicMatrix, IList<int> informationSet, IList<int> zSet,
            Randomizer randomizer)
        {
            var randomIndexFromZSet = CalculationHelper.GenerateRandomIndex(zSet, randomizer);
            while (true)
            {
                if (IsZeroVector(publicMatrix.Column(zSet[randomIndexFromZSet])))
                {
                    randomIndexFromZSet = CalculationHelper.GenerateRandomIndex(zSet, randomizer);
                }
                else
                {
                    break;
                }
            }
            var randomIndexForInformationSet = CalculationHelper.GetNonZeroIndexFromVector(
                publicMatrix.Column(zSet[randomIndexFromZSet]), randomizer);

            var valueForChange = informationSet[randomIndexForInformationSet];
            informationSet[randomIndexForInformationSet] = zSet[randomIndexFromZSet];
            zSet[randomIndexFromZSet] = valueForChange;

            return new ColumnAndRowIndices()
            {
                ColumnIndex = informationSet[randomIndexForInformationSet],
                RowIndex = randomIndexForInformationSet
            };
        }

        public static int GetVectorWeight(Vector<float> vector)
        {
            return (int)vector.Sum();
        }

        public static string GetHashTableKeyForGISD(IList<int> indices, Vector<float> vector)
        {
            return string.Join("", indices.Select(index => vector[index]));
        }

        public static Vector<float> MultipyMatrixWithVector(Matrix<float> matrix, Vector<float> vector)
        {
            var finalVectorSize = matrix.Row(0).Count;
            var finalVector = Vector<float>.Build.Dense(finalVectorSize);

            for (int i = 0; i < finalVectorSize; i++)
            {
                var temporaryVector = matrix.Column(i);
                float sum = 0;
                for (int j = 0; j < temporaryVector.Count; j++)
                {
                    sum = AddBinaryNumbers(sum, temporaryVector[j] * vector[j]);
                }

                finalVector[i] = sum;
            }

            return finalVector;
        }

        public static float AddBinaryNumbers(float firstNumber, float secondNumber)
        {
            return (firstNumber + secondNumber) % 2;
        }

        public static bool IsEliminableByGaussian(Matrix<float> matrix)
        {
            int rowCount = matrix.RowCount;
            int rowLength = matrix.Row(0).Count;
            for (int row = 0; row < rowCount; row++)
            {
                //Checking whether diagonal member is 1
                //and making it if not by swapping rows.
                //If not, main cycle is stopped.
                bool isEmptyColumn = true;
                if (matrix[row, row] == 0)
                {
                    for (int nonZeroRow = row + 1; nonZeroRow < rowCount; nonZeroRow++)
                    {
                        if (matrix[nonZeroRow, row] != 0)
                        {
                            isEmptyColumn = false;
                            //swap the rows
                            float[] temporary = new float[rowLength];
                            for (int i = 0; i < rowLength; i++)
                            {
                                temporary[i] = matrix[nonZeroRow, i];
                                matrix[nonZeroRow, i] = matrix[row, i];
                                matrix[row, i] = temporary[i];
                            }
                            break;
                        }
                    }
                    if (isEmptyColumn)
                    {
                        return false;
                    }
                }

                //Elimination to the bottom
                for (int nonZeroRow = row + 1; nonZeroRow < rowCount; nonZeroRow++)
                {
                    if (matrix[nonZeroRow, row] != 0)
                    {
                        //Add two rows to eliminate 1(value)
                        for (int i = 0; i < rowLength; i++)
                        {
                            matrix[nonZeroRow, i] = AddBinaryNumbers(matrix[nonZeroRow, i],
                                                                   matrix[row, i]);
                        }
                    }
                }
                //Elimination to the top
                for (int nonZeroRow = row - 1; nonZeroRow >= 0; nonZeroRow--)
                {
                    if (matrix[nonZeroRow, row] != 0)
                    {
                        //Add two rows to eliminate 1(value)
                        for (int i = 0; i < rowLength; i++)
                        {
                            matrix[nonZeroRow, i] = AddBinaryNumbers(matrix[nonZeroRow, i],
                                                                   matrix[row, i]);
                        }
                    }
                }
            }

            return true;
        }

        public static Matrix<float> GaussianEliminationForGISD(Matrix<float> matrix, IList<int> indices, Randomizer randomizer, ILogger logger)
        {
            int rowCount = indices.Count;
            int rowLength = matrix.Row(0).Count;
            for (int row = 0; row < rowCount; row++)
            {
            //Checking whether diagonal member is 1
            //and making it if not by swapping rows.
            //If not, main cycle is stopped.
            Search:
                bool isEmptyColumn = true;
                if (matrix[row, indices[row]] == 0)
                {
                    for (int nonZeroRow = row + 1; nonZeroRow < rowCount; nonZeroRow++)
                    {
                        if (matrix[nonZeroRow, indices[row]] != 0)
                        {
                            isEmptyColumn = false;
                            //swap the rows
                            float[] temporary = new float[rowLength];
                            for (int i = 0; i < rowLength; i++)
                            {
                                temporary[i] = matrix[nonZeroRow, i];
                                matrix[nonZeroRow, i] = matrix[row, i];
                                matrix[row, i] = temporary[i];
                            }
                            break;
                        }
                    }
                    if (isEmptyColumn)
                    {
                        indices[row] = GenerateRandomIndexForGISD(indices, matrix.ColumnCount - 1, randomizer);

                        goto Search;
                    }
                }
                //Elimination to the bottom
                for (int nonZeroRow = row + 1; nonZeroRow < rowCount; nonZeroRow++)
                {
                    if (matrix[nonZeroRow, indices[row]] != 0)
                    {
                        //Add two rows to eliminate 1(value)
                        for (int i = 0; i < rowLength; i++)
                        {
                            matrix[nonZeroRow, i] = AddBinaryNumbers(matrix[nonZeroRow, i],
                                                                   matrix[row, i]);
                        }
                    }
                }
                //Elimination to the top
                for (int nonZeroRow = row - 1; nonZeroRow >= 0; nonZeroRow--)
                {
                    if (matrix[nonZeroRow, indices[row]] != 0)
                    {
                        //Add two rows to eliminate 1(value)
                        for (int i = 0; i < rowLength; i++)
                        {
                            matrix[nonZeroRow, i] = AddBinaryNumbers(matrix[nonZeroRow, i],
                                                                   matrix[row, i]);
                        }
                    }
                }
            }

            return matrix;
        }

        public static IList<int> CloneList(IList<int> listToBeCloned)
        {
            return listToBeCloned.Select(item => item).ToList();
        }

        public static IList<int> GenerateRandomIndices(int maxValue, int numberOfElements, Randomizer randomizer)
        {
            var randomIndicesList = new List<int>();

            for (int i = 0; i < numberOfElements; i++)
            {
                try
                {
                    int randomIndex = randomizer.Next(0, maxValue);
                    if (randomIndicesList.Contains(randomIndex))
                    {
                        i--;
                    }
                    else
                    {
                        randomIndicesList.Add(randomIndex);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    i--;
                }
            }

            return randomIndicesList;
        }

        public static int GetHammingDistance(Vector<float> firstVector, Vector<float> secondVector)
        {
            int hammingDistance = 0;
            for (int i = 0; i < firstVector.Count; i++)
            {
                if (!firstVector[i].Equals(secondVector[i]))
                {
                    hammingDistance++;
                }
            }
            return hammingDistance;
        }

        public static int GenerateRandomIndexForGISD(IList<int> indices, int maxValue, Randomizer randomizer)
        {
            while (true)
            {
                try
                {
                    int randomIndex = randomizer.Next(0, maxValue);
                    if (!indices.Contains(randomIndex))
                    {
                        return randomIndex;
                    }
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        public static void CalculateLocationSets(IList L0Set, IList L1Set, Vector<float> firstCipheredVector,
                                                                          Vector<float> secondCipheredVector)
        {
            for (int i = 0; i < firstCipheredVector.Count; i++)
            {
                if (AddBinaryNumbers(firstCipheredVector[i], secondCipheredVector[i]).Equals(0))
                {
                    L0Set.Add(i);
                }
                else
                {
                    L1Set.Add(i);
                }
            }
        }

        public static IList<int> GenerateKnumberOfRandomColumnsIndices(int k, IList<int> L0Set, Randomizer randomizer)
        {
            var columnIndices = new List<int>();
            var usedColumnsIndices = new ArrayList();

            for (int i = 0; i < k; i++)
            {
                try
                {
                    int randomIndex = randomizer.Next(0, L0Set.Count - 1);
                    if (usedColumnsIndices.Contains(randomIndex))
                    {
                        i--;
                    }
                    else
                    {
                        columnIndices.Add(L0Set[randomIndex]);
                        usedColumnsIndices.Add(randomIndex);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    i--;
                }
            }

            return columnIndices;
        }

        public static Matrix<float> ConstructTemporaryMatrix(IList<int> indices, Matrix<float> publicMatrix,
                                                                          Vector<float> cipheredVector)
        {
            var indicesCount = indices.Count;
            var temporaryMatrix = Matrix<float>.Build.Dense(indicesCount, indicesCount, 0);
            var temporaryVectorArray = new float[indices.Count];

            for (int i = 0; i < indicesCount; i++)
            {
                temporaryVectorArray[i] = cipheredVector[indices[i]];
                temporaryMatrix.SetColumn(i, publicMatrix.Column(indices[i]));
            }
            var prefinal = temporaryMatrix.Transpose();
            var temporaryVector = Vector<float>.Build.DenseOfArray(temporaryVectorArray);
            var finalMatrix = prefinal.InsertColumn(indicesCount, temporaryVector);

            return finalMatrix;
        }

        public static Matrix<float> ConstructTemporaryMatrixForGISD(Matrix<float> publicMatrix,
                                                                  Vector<float> cipheredVector)
        {
            var temporaryMatrix = Matrix<float>.Build.Dense(publicMatrix.RowCount, publicMatrix.ColumnCount, 0);

            for (int i = 0; i < publicMatrix.RowCount; i++)
            {
                temporaryMatrix.SetRow(i, publicMatrix.Row(i));
            }
            //var prefinal = temporaryMatrix.Transpose();
            //var temporaryVector = Vector<float>.Build.DenseOfArray(temporaryVectorArray);
            //var finalMatrix = prefinal.InsertColumn(indicesCount, temporaryVector);

            //return finalMatrix;
            return temporaryMatrix.InsertRow(publicMatrix.RowCount, cipheredVector);
        }

        public static ConstructionOfZMatrixResult ConstructMatrixFromIndices(Matrix<float> matrix, IList<int> indices, IList<int> lSet)
        {
            var result = new ConstructionOfZMatrixResult(Matrix<float>.Build.Dense(matrix.RowCount, indices.Count, 0));

            for (int i = 0; i < indices.Count; i++)
            {
                result.ZMatrix.SetColumn(i, matrix.Column(indices[i]));
                if (lSet.Contains(indices[i]))
                {
                    result.LIndicesEquivalent.Add(i);
                }
            }
            return result;
        }

        public static Vector<float> GenerateErrorVector(int length, int amountOfErrors, Randomizer randomizer)
        {
            var vector = Vector<float>.Build.Dense(length, 0);

            for (int i = 0; i < amountOfErrors; i++)
            {
                int randomIndex = randomizer.Next(0, length - 1);
                if (vector[randomIndex] != 0)
                {
                    i--;
                }
                vector[randomIndex] = 1;
            }

            return vector;
        }

        public static Vector<float> GenerateRandomVector(int length, Randomizer randomizer)
        {
            var vector = Vector<float>.Build.Dense(length, 0);

            for (int i = 0; i < length; i++)
            {
                vector[i] = randomizer.Next(0, 2);
            }

            return vector;
        }

        public static Vector<float> AddVectorMod2(Vector<float> vector1, Vector<float> vector2)
        {
            var finalVector = Vector<float>.Build.Dense(vector1.Count, 0);

            for (int i = 0; i < vector1.Count; i++)
            {
                finalVector[i] = AddBinaryNumbers(vector1[i], vector2[i]);
            }

            return finalVector;
        }

        public static bool AreThereAnyGarbledIndices(IList<int> randomIndices, Vector<float> errorVector1, Vector<float> errorVector2)
        {
            foreach (int index in randomIndices)
            {
                if (errorVector1[index] == 1 || errorVector2[index] == 1) return true;
            }

            return false;
        }

        public static IList<int> CalculateZSet(IList<int> informationSet, int sizeOfNSet)
        {
            var zSet = new List<int>();

            for (int i = 0; i < sizeOfNSet; i++)
            {
                if (!informationSet.Contains(i))
                {
                    zSet.Add(i);
                }
            }

            return zSet;
        }
    }
}