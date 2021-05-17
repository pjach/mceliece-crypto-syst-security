using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Attacks;
using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Models;
using MIF.VU.PJach.McElieceSecurity.Utilities.Helpers;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MIF.VU.PJach.McElieceSecurity.AttacksExecutors
{
    public class FlwcAttackExecutor
    {
        private IFileWriter _fileWriter;
        private FLWCAttack _flwcAttack;
        private IList<FlwcAttackStatisticsEntry> FlwcAttackStatistics;
        private Matrix<float> PublicKey;
        private int ErrorVectorWeight;
        private readonly ILogger _logger;

        public FlwcAttackExecutor(IFileWriter fileWriter, IFileReader fileReader, ILogger logger, bool extendStatisticsFile)
        {
            _fileWriter = fileWriter;
            if (extendStatisticsFile)
            {
                var statisticsJson = fileReader.ReadFromFile(ConfigurationManager.AppSettings["flwc_statistics"]);
                FlwcAttackStatistics = JsonConvert.DeserializeObject<List<FlwcAttackStatisticsEntry>>(statisticsJson);
            }
            else
            {
                FlwcAttackStatistics = new List<FlwcAttackStatisticsEntry>();
            }
            var publicKeyData = fileReader.ReadFromFile(ConfigurationManager.AppSettings["public_matrix_file_name"]);
            PublicKey = MatrixHelper.ConvertToMatrix(publicKeyData);
            var errorVectorWeightValue = ConfigurationManager.AppSettings["error_vector_weight"];
            ErrorVectorWeight = int.Parse(errorVectorWeightValue);
            _logger = logger;
            _flwcAttack = new FLWCAttack(logger);
        }

        public void ExecuteAttack(int amountOfAttacks, int parameterP, int paramaterSigma)
        {
            for (int i = 0; i < amountOfAttacks; i++)
            {
                FlwcAttackStatistics.Add(_flwcAttack.PrepareAndExecuteFlwcAttack(PublicKey, parameterP, paramaterSigma, ErrorVectorWeight));
                _logger.Debug($"FLWC attack {i + 1} finished. {amountOfAttacks - (i + 1)} left. Count of statistics items: {FlwcAttackStatistics.Count}. The time: {DateTime.Now}");
            }
            _fileWriter.WriteToFile(JsonConvert.SerializeObject(FlwcAttackStatistics), ConfigurationManager.AppSettings["flwc_statistics"]);
        }
    }
}