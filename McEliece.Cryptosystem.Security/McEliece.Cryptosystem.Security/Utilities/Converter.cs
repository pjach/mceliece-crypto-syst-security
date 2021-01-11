using MathNet.Numerics.LinearAlgebra;

namespace MIF.VU.PJach.McElieceSecurity.Utilities
{
    public static class Converter
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

        public static Vector<float> ConvertToVector(string data)
        {
            string[] textVectorArray = data.Split(", ");

            float[] arrayVector = new float[textVectorArray.Length];

            for (int i = 0; i < textVectorArray.Length; i++)
            {
                arrayVector[i] = float.Parse(textVectorArray[i]);
            }

            return Vector<float>.Build.DenseOfArray(arrayVector);
        }
    }
}