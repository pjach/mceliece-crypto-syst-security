﻿using MathNet.Numerics.LinearAlgebra;

namespace McEliece.Cryptosystem.Security.Utilities
{
    public static class Converter
    {
        public static Matrix<float> ConvertToMatrix(string data)
        {
            string[] rows = data.Split("; ");
            float[,] matrixArray = new float[rows[0].Length, rows[0].Split(' ').Length];

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
    }
}