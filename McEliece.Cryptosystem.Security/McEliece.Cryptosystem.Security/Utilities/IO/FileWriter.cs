using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Utilities.Helpers;
using System.IO;

namespace MIF.VU.PJach.McElieceSecurity.Utilities.IO
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