using McEliece.Cryptosystem.Security.Attacks;
using McEliece.Cryptosystem.Security.Utilities;
using System.Configuration;

namespace McEliece.Cryptosystem.Security
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string publicKeyData = FileReader.ReadFromFile(ConfigurationManager
                                       .AppSettings["public_matrix_file_name"]);
            string interceptedMessage1 = FileReader.ReadFromFile(ConfigurationManager
                                          .AppSettings["first_ciphered_vector_name"]);
            string interceptedMessage2 = FileReader.ReadFromFile(ConfigurationManager
                                         .AppSettings["second_ciphered_vector_name"]);
            string messageData = FileReader.ReadFromFile(ConfigurationManager
                                        .AppSettings["message_vector_name"]);

            var publicKey = Converter.ConvertToMatrix(publicKeyData);
            var interceptedVector1 = Converter.ConvertToVector(interceptedMessage1);
            var interceptedVector2 = Converter.ConvertToVector(interceptedMessage2);
            var messageVector = Converter.ConvertToVector(messageData);

            FailureUnderMessageResendAttack.AttemptAnAttack(messageVector, interceptedVector1,
                                                               interceptedVector2, publicKey);
        }
    }
}