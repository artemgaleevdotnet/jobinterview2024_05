using System.CommandLine;

internal interface ICliCommand
{
    Command InitializeCommand(CancellationToken cancellationToken);
}