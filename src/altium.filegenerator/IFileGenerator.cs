using System.Numerics;

namespace altium.filegenerator
{
    public interface IFileGenerator
    {
        public Task GenerateFile(string filename, BigInteger fileSizeInBytes, CancellationToken cancellationToken);
    }
}