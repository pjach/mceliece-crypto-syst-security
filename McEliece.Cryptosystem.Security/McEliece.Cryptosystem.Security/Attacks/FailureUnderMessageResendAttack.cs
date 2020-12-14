using MathNet.Numerics.LinearAlgebra;
using matrixcEliece.Cryptosystem.Security.Utilities;
using System;
using System.Collections.Generic;

namespace McEliece.Cryptosystem.Security.Attacks
{
    public static class FailureUnderMessageResendAttack
    {
        public static bool AttemptAnAttack(Vector<float> messageVector, Vector<float> interceptedVector1,
                                        Vector<float> interceptedVector2, Matrix<float> publicKey)
        {
            int errorVectorWeight = Calculation.GetHammingDistance(messageVector,
                  Calculation.MultipyMatrixWithVector(publicKey, messageVector));
            var L0Set = new List<int>();
            var L1Set = new List<int>();

            Calculation.CalculateLocationSets(L0Set, L1Set, interceptedVector1, interceptedVector2);

            int rowCount = publicKey.RowCount;

            int iteration = 1;
            while (true)
            {
                Console.WriteLine(iteration + " iteration" + Environment.NewLine);
                var randomIndicess = Calculation.GenerateKnumberOfRandomColumnsIndices(rowCount, L0Set);
                var temporaryMatrix = Calculation.ConstructTemporaryMatrix(randomIndicess,
                                                           publicKey, interceptedVector1);
                var IsEliminableByGaussian = Calculation.IsEliminableByGaussian(temporaryMatrix);
                if (IsEliminableByGaussian)
                {
                    var messageCandidate = Calculation.MultipyMatrixWithVector(publicKey,
                                                       temporaryMatrix.Column(rowCount));
                    var hammingDistance = Calculation.GetHammingDistance(messageVector, messageCandidate);
                    if (hammingDistance == errorVectorWeight)
                    {
                        Console.WriteLine("Found!");
                        return true;
                    }
                }

                iteration++;
            }
        }
    }
}