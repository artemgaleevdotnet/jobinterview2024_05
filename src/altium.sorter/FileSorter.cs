namespace altium.sorter
{
    internal sealed class FileSorter : IFileSorter
    {
        private const int ChunkSize = 1000000;
        private readonly ParsedStringComparer _comparator;

        public FileSorter()
        {
            _comparator = new ParsedStringComparer();
        }

        public async Task SortFile(string inputFilePath, string outputFilePath, CancellationToken cancellationToken = default)
        {
            var tempFiles = new List<string>();

            using StreamReader inputFile = new(inputFilePath);

            SortedList<string, int> chunk = new(_comparator);
            int index = 0;

            while (!inputFile.EndOfStream)
            {
                string? line = await inputFile.ReadLineAsync();

                if (line == null) continue;

                if (!chunk.TryAdd(line, 0))
                {
                    chunk[line]++;
                }

                if (chunk.Count < ChunkSize && !inputFile.EndOfStream)
                {
                    continue;
                }

                string tempFilePath = $"temp_{index}.txt";

                tempFiles.Add(tempFilePath);

                await WriteChunkToFile(chunk, tempFilePath, cancellationToken);

                chunk.Clear();

                index++;
            }
            
            await MergeTempFiles(tempFiles, outputFilePath, cancellationToken);

            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        private async Task WriteChunkToFile(SortedList<string, int> chunk, string filePath, CancellationToken cancellationToken)
        {
            using var outputFile = new StreamWriter(filePath);

            foreach (var line in chunk)
            {
                for (int i = 0; i <= line.Value; i++)
                {
                    await outputFile.WriteLineAsync(line.Key.ToArray(), cancellationToken);
                }
            }
        }

        private async Task MergeTempFiles(List<string> tempFiles, string outputFilePath, CancellationToken cancellationToken)
        {
            using var outputFile = new StreamWriter(outputFilePath);

            var readers = tempFiles.Select(tempFile => new StreamReader(tempFile)).ToArray();

            var currentLines = new string[readers.Length];

            for (int i = 0; i < readers.Length; i++)
            {
                if (!readers[i].EndOfStream)
                {
                    currentLines[i] = await readers[i].ReadLineAsync();
                }
            }

            while (currentLines.Any(currentLine => currentLine!= null))
            {
                int minIndex = IndexOfMin(currentLines);

                await outputFile.WriteLineAsync(currentLines[minIndex].ToArray(), cancellationToken);

                if (!readers[minIndex].EndOfStream)
                {
                    currentLines[minIndex] = await readers[minIndex].ReadLineAsync();
                }
                else
                {
                    currentLines[minIndex] = null!;
                }
            }

            foreach (var reader in readers)
            {
                reader.Close();
            }
        }

        private int IndexOfMin(string[] list)
        {
            int minIndex = 0;

            for (int i = 1; i < list.Length; i++)
            {
                if (list[i] == null) continue;

                if (list[minIndex] == null)
                {
                    minIndex = i;

                    continue;
                }

                if (_comparator.Compare(list[i], list[minIndex]) < 0)
                {
                    minIndex = i;
                }
            }

            return minIndex;
        }
    }
}