using Microsoft.Extensions.Options;
using System.Numerics;
using System.Text;

namespace altium.filegenerator
{
    internal sealed class FileGenerator : IFileGenerator
    {
        private readonly FileGenereatorSettings _options;

        public FileGenerator(IOptions<FileGenereatorSettings> options)
        {
            _options = options.Value;
        }

        public async Task GenerateFile(string filePath, BigInteger fileSizeInBytes, CancellationToken cancellationToken)
        {
            Random random = new();

            BigInteger bytesWritten = 0;

            using StreamWriter writer = new(filePath);

            while (bytesWritten < fileSizeInBytes)
            {
                // minumum 2 symbols reserved for number and end of line
                int stringLength = random.Next(_options.Delimiter.Length + 2, _options.MaxStringSize);

                if (bytesWritten + stringLength < fileSizeInBytes)
                {
                    var randomString = GenerateRandomString(random, stringLength);

                    await writer.WriteAsync(randomString, cancellationToken);

                    bytesWritten += Encoding.UTF8.GetByteCount(randomString.ToString());
                }
                else
                {
                    BigInteger remainingBytes = fileSizeInBytes - bytesWritten;

                    var randomString = GenerateRandomString(random, remainingBytes);

                    randomString.Length--; // minus end of file 

                    await writer.WriteAsync(randomString, cancellationToken);

                    bytesWritten += remainingBytes;
                }
            }
        }

        private StringBuilder GenerateRandomString(Random random, BigInteger byteCount)
        {
            var sb = new StringBuilder();

            sb.Append(byteCount);
            sb.Append(_options.Delimiter);

            long currentByteCount = 0;

            if (currentByteCount < byteCount)
            {
                while (currentByteCount < byteCount)
                {
                    char randomChar = _options.Chars[random.Next(_options.Chars.Length)];
                    sb.Append(randomChar);

                    currentByteCount = Encoding.UTF8.GetByteCount(sb.ToString());
                }
            }

            sb.Length--; // minus end of line
            sb.Append(Environment.NewLine);

            return sb;
        }
    }
}