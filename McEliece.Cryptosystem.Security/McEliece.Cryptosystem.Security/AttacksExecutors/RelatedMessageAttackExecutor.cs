using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Attacks;
using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Models;
using MIF.VU.PJach.McElieceSecurity.Utilities;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Configuration;

namespace MIF.VU.PJach.McElieceSecurity.AttacksExecutors
{
    public class RelatedMessageAttackExecutor
    {
        private IFileWriter fileWriter;
        private RelatedMessageAttacks messageAttacks = new RelatedMessageAttacks();
        private IList<StatisticsEntry> relatedMessageStatistics;
        private IList<StatisticsEntry> resendMessageStatistics;
        private Matrix<float> PublicKey;
        private int ErrorVectorWeight;
        private readonly ILogger _logger;

        public RelatedMessageAttackExecutor(IFileWriter fileWriter, IFileReader fileReader, ILogger logger)
        {
            _logger = logger;
            this.fileWriter = fileWriter;
            relatedMessageStatistics = new List<StatisticsEntry>();
            resendMessageStatistics = new List<StatisticsEntry>();
            var publicKeyData = fileReader.ReadFromFile(ConfigurationManager.AppSettings["public_matrix_file_name"]);
            PublicKey = Converter.ConvertToMatrix(publicKeyData);
            var errorVectorWeightValue = ConfigurationManager.AppSettings["error_vector_weight"];
            ErrorVectorWeight = int.Parse(errorVectorWeightValue);
        }

        public RelatedMessageAttackExecutor(IFileWriter fileWriter, string relatedMessageStatistics,
                             ILogger logger, IFileReader fileReader, string resendMessageStatistics)
        {
            _logger = logger;
            this.fileWriter = fileWriter;
            this.relatedMessageStatistics = JsonConvert.DeserializeObject<List<StatisticsEntry>>(relatedMessageStatistics);
            this.resendMessageStatistics = JsonConvert.DeserializeObject<List<StatisticsEntry>>(resendMessageStatistics);
            var publicKeyData = fileReader.ReadFromFile(ConfigurationManager.AppSettings["public_matrix_file_name"]);
            PublicKey = Converter.ConvertToMatrix(publicKeyData);
            var errorVectorWeightValue = ConfigurationManager.AppSettings["error_vector_weight"];
            ErrorVectorWeight = int.Parse(errorVectorWeightValue);
        }

        public void ExecuteRelatedMessageAttack(int amountOfAttacks)
        {
            for (int i = 0; i < amountOfAttacks; i++)
            {
                relatedMessageStatistics.Add(messageAttacks.PrepareDataAndAttemptRelatedAttack(ErrorVectorWeight, PublicKey));
            }
            fileWriter.WriteToFile(JsonConvert.SerializeObject(relatedMessageStatistics), "RelatedMessageAttackStatistics");
        }

        public void ExecuteResendMessageAttack(int amountOfAttacks)
        {
            for (int i = 0; i < amountOfAttacks; i++)
            {
                resendMessageStatistics.Add(messageAttacks.PrepareDataAndAttemptResendAttack(ErrorVectorWeight, PublicKey));
            }
            fileWriter.WriteToFile(JsonConvert.SerializeObject(relatedMessageStatistics), "ResendMessageAttackStatistics");
        }

        public void ExecuteBothAttacks(int amountOfAttacks)
        {
            for (int i = 0; i < amountOfAttacks; i++)
            {
                resendMessageStatistics.Add(messageAttacks.PrepareDataAndAttemptResendAttack(ErrorVectorWeight, PublicKey));
                relatedMessageStatistics.Add(messageAttacks.PrepareDataAndAttemptRelatedAttack(ErrorVectorWeight, PublicKey));
            }
            fileWriter.WriteToFile(JsonConvert.SerializeObject(relatedMessageStatistics), "ResendMessageAttackStatistics.txt");
            fileWriter.WriteToFile(JsonConvert.SerializeObject(resendMessageStatistics), "RelatedMessageAttackStatistics.txt");
        }
    }
}