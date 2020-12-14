using System;
using System.IO;
using System.Reflection;

namespace McEliece.Cryptosystem.Security.Utilities
{
    public static class FileReader
    {
        public static string ReadFromFile(string name)
        {
            string data = string.Empty;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int index = path.LastIndexOf("bin");
            path = path.Substring(0, index);
            path = Path.Combine(path, "Data", name);

            if (File.Exists(path))
            {
                StreamReader Textfile = new StreamReader(path);
                string line;

                while ((line = Textfile.ReadLine()) != null)
                {
                    data = String.Concat(data, line);
                }

                Textfile.Close();
            }

            return data;
        }
    }
}