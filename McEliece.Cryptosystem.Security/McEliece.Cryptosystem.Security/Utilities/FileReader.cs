using MIF.VU.PJach.McElieceSecurity.Contracts;
using System;
using System.IO;

namespace MIF.VU.PJach.McElieceSecurity.Utilities
{
    public class FileReader : IFileReader
    {
        public string ReadFromFile(string name)
        {
            string data = string.Empty;
            string path = PathHelper.GetPath(name);

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