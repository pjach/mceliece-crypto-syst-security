namespace McEliece.Cryptosystem.Security.Contracts
{
    public interface IFileWriter
    {
        public void WriteToFile(string data, string name);
    }
}