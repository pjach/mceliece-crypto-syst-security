using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Attacks;
using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Models;
using MIF.VU.PJach.McElieceSecurity.Utilities;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MIF.VU.PJach.McElieceSecurity.AttacksExecutors
{
    public class GisdAttackExecutor
    {
        private IFileWriter _fileWriter;
        private GISDAttack _gisdAttack;
        private IList<GisdAttackStatisticsEntry> GisdAttackStatistics;
        private Matrix<float> PublicKey;
        private int ErrorVectorWeight;
        private readonly ILogger _logger;

        public GisdAttackExecutor(IFileWriter fileWriter, IFileReader fileReader, ILogger logger, bool extendStatisticsFile)
        {
            _fileWriter = fileWriter;
            if (extendStatisticsFile)
            {
                var statisticsJson = fileReader.ReadFromFile(ConfigurationManager.AppSettings["gisd_statistics"]);
                GisdAttackStatistics = JsonConvert.DeserializeObject<List<GisdAttackStatisticsEntry>>(statisticsJson);
            }
            else
            {
                GisdAttackStatistics = new List<GisdAttackStatisticsEntry>();
            }
            var publicKeyData = fileReader.ReadFromFile(ConfigurationManager.AppSettings["public_matrix_file_name"]);
            PublicKey = Converter.ConvertToMatrix(publicKeyData);
            var errorVectorWeightValue = ConfigurationManager.AppSettings["error_vector_weight"];
            ErrorVectorWeight = int.Parse(errorVectorWeightValue);
            _logger = logger;
            _gisdAttack = new GISDAttack(logger);
        }

        public void ExecuteAttack(int amountOfAttacks, int parameterP, int paramaterSigma)
        {
            for (int i = 0; i < amountOfAttacks; i++)
            {
                GisdAttackStatistics.Add(_gisdAttack.PrepareAndExecuteGisdAttack(PublicKey, parameterP, paramaterSigma, ErrorVectorWeight));
                _logger.Debug($"GISD attack {i + 1} finished. {amountOfAttacks - (i+1)} left. Count of statistics items: {GisdAttackStatistics.Count}. The time: {DateTime.Now}");
            }
            _fileWriter.WriteToFile(JsonConvert.SerializeObject(GisdAttackStatistics), "GISDAttackStatisticsTEST.txt");
        }
    }
}