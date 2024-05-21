using altium.filegenerator;
using altium.sorter;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

internal class Program
{
    private static Task Main(string[] args)
    {
        Console.WriteLine("Altium test task.");

        var rootCommand = new RootCommand
        {
            Name = "altium-test",
            Description = "Large File Utility"
        };

        var commands = Configure().GetServices<ICliCommand>();

        foreach (var command in commands)
        {
            rootCommand.AddCommand(command.InitializeCommand(default));
        }

        return rootCommand.InvokeAsync(args);
    }

    private static IServiceProvider Configure()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFileGenerator();
        serviceCollection.AddFileSorter();

        serviceCollection.AddSingleton<ICliCommand, GenerateCommand>();
        serviceCollection.AddSingleton<ICliCommand, SortCommand>();

        return serviceCollection.BuildServiceProvider();
    }
}
