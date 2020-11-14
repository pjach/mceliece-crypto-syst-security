using McEliece.Cryptosystem.Security.Utilities;
using System.Configuration;

namespace McEliece.Cryptosystem.Security
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string data = FileReader.ReadFromFile(ConfigurationManager.AppSettings["matrix_file_name"]);
            var matrix = Converter.ConvertToMatrix(data);

            System.Console.WriteLine(matrix.ToString());
        }
    }
}