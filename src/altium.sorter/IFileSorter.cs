namespace altium.sorter
{
    public interface IFileSorter
    {
        Task SortFile(string inputFilePath, string outputFilePath, CancellationToken cancellationToken = default);
    }
}