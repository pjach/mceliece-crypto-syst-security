using McEliece.Cryptosystem.Security.Contracts;
using System.IO;

namespace McEliece.Cryptosystem.Security.Utilities
{
    public class FileWriter : IFileWriter
    {
        public void WriteToFile(string data, string name)
        {
            string path = PathHelper.GetPath(name);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, data);
        }
    }
}