using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Models;
using System.Collections.Generic;

namespace MIF.VU.PJach.McElieceSecurity.Utilities.Helpers
{
    public static class MatrixHelper
    {
        public static Matrix<float> ConvertToMatrix(string data)
        {
            string[] rows = data.Split("; ");
            float[,] matrixArray = new float[rows.Length, rows[0].Split(' ').Length];

            string[] row = new string[rows[0].Length];

            for (int i = 0; i < rows.Length; i++)
            {
                row = rows[i].Split(" ");
                for (int j = 0; j < row.Length; j++)
                {
                    matrixArray[i, j] = float.Parse(row[j]);
                }
            }
            return Matrix<float>.Build.DenseOfArray(matrixArray);
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
                            matrix[nonZeroRow, i] = CalculationHelper.AddBinaryNumbers(matrix[nonZeroRow, i],
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
                            matrix[nonZeroRow, i] = CalculationHelper.AddBinaryNumbers(matrix[nonZeroRow, i],
                                                                   matrix[row, i]);
                        }
                    }
                }
            }

            return true;
        }

        public static Matrix<float> GaussianEliminationForFLWC(Matrix<float> matrix, IList<int> indices, Randomizer randomizer)
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
                        indices[row] = RandomnessHelper.GenerateRandomIndexForFLWC(indices, matrix.ColumnCount - 1, randomizer);

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
                            matrix[nonZeroRow, i] = CalculationHelper.AddBinaryNumbers(matrix[nonZeroRow, i],
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
                            matrix[nonZeroRow, i] = CalculationHelper.AddBinaryNumbers(matrix[nonZeroRow, i],
                                                                   matrix[row, i]);
                        }
                    }
                }
            }

            return matrix;
        }

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
                            publicMatrix[row, i] = CalculationHelper.AddBinaryNumbers(publicMatrix[row, i],
                                publicMatrix[columnAndRowIndices.RowIndex, i]);
                        }
                    }
                }
            }

            return publicMatrix;
        }
    }
}