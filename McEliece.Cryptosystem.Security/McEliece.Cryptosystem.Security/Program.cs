using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Statistics;
using MIF.VU.PJach.McElieceSecurity.Utilities;
using System.Configuration;

namespace MIF.VU.PJach.McElieceSecurity
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IFileReader fileReader = new FileReader();
            IFileWriter fileWriter = new FileWriter();

            string relatedAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["related_statistics"]);
            string resendAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["resend_statistics"]);
            string errorVectorWeightValue = ConfigurationManager
                            .AppSettings["error_vector_weight"];

            var errorVectorWeight = int.Parse(errorVectorWeightValue);

            var statistics = new RelatedMessageStatistics();

            System.Console.WriteLine("Failure under related message attack statistics");
            statistics.CalculateAndPrintStatistics(relatedAttackData);

            System.Console.WriteLine("Failure under resend message attack statistics");
            statistics.CalculateAndPrintStatistics(resendAttackData);

            string publicKeyData = fileReader.ReadFromFile(ConfigurationManager
                                       .AppSettings["public_matrix_file_name"]);
            string messageData1 = fileReader.ReadFromFile(ConfigurationManager
                                        .AppSettings["message_vector_name1"]);
            string messageData2 = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["message_vector_name2"]);

            /*var publicKey = Converter.ConvertToMatrix(publicKeyData);
            var messageVector1 = Converter.ConvertToVector(messageData1);
            var messageVector2 = Converter.ConvertToVector(messageData2);

            var attack = new RelatedMessageAttacks();

            for (int i = 1; i <= 100; i++)
            {
                var relatedMessageEntry = attack.PrepareDataAndAttemptRelatedAttack(messageVector1, messageVector2,
                                                                                     errorVectorWeight, publicKey);
                System.Console.WriteLine(i + ". iteration" + System.Environment.NewLine + "  " + relatedMessageEntry.SpentTime
                    + ", " + relatedMessageEntry.TotalIterationsCount + ", " + DateTime.Now);
                var resendMessageEntry = attack.PrepareDataAndAttemptResendAttack(messageVector1,
                                                                   errorVectorWeight, publicKey);
                System.Console.WriteLine("  " + resendMessageEntry.SpentTime + ", "
                    + resendMessageEntry.TotalIterationsCount);
                relatedStatistics.Add(relatedMessageEntry);
                resendStatistics.Add(resendMessageEntry);
            }
            fileWriter.WriteToFile(JsonConvert.SerializeObject(relatedStatistics), "RelatedMessageAttackStatisticss.txt");
            fileWriter.WriteToFile(JsonConvert.SerializeObject(resendStatistics), "ResendMessageAttackStatisticss.txt");
            */
        }
    }
}