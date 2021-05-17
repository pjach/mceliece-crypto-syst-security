using MathNet.Numerics.LinearAlgebra;
using System.Linq;

namespace MIF.VU.PJach.McElieceSecurity.Utilities.Helpers
{
    public static class VectorHelper
    {
        public static int GetVectorWeight(Vector<float> vector)
        {
            return (int)vector.Sum();
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

        public static Vector<float> AddVectorMod2(Vector<float> vector1, Vector<float> vector2)
        {
            var finalVector = Vector<float>.Build.Dense(vector1.Count, 0);

            for (int i = 0; i < vector1.Count; i++)
            {
                finalVector[i] = CalculationHelper.AddBinaryNumbers(vector1[i], vector2[i]);
            }

            return finalVector;
        }
    }
}