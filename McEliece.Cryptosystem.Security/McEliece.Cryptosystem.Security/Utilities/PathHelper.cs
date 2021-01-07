using System.IO;
using System.Reflection;

namespace McEliece.Cryptosystem.Security.Utilities
{
    public static class PathHelper
    {
        public static string GetPath(string fileName)
        {
            string data = string.Empty;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int index = path.LastIndexOf("bin");
            path = path.Substring(0, index);
            return Path.Combine(path, "Data", fileName);
        }
    }
}