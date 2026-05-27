using SAIRAH.Commands.Abstractions;
using SAIRAH.Commands.Files;
using SAIRAH.Commands.System;
using SAIRAH.Core.Parsing;
using SAIRAH.Core.Routing;
using SAIRAH.Core.Services;
using SAIRAH.Security.Services;
using SAIRAH.Storage.Logging;

ICommandParser parser = new BasicCommandParser();

IAssistantCommand[] commands =
[
    new OpenApplicationCommand(),
    new OpenFolderCommand(),
    new OpenWebsiteCommand(),
    new CreateFolderCommand()
];

IPermissionService permissionService = new PermissionService();

CommandRouter router = new CommandRouter(commands, permissionService);
IAssistantLogger logger = new FileAssistantLogger();

AssistantEngine assistant = new AssistantEngine(parser, router, logger);

Console.WriteLine("SAIRAH Core Test");
Console.WriteLine("Secure Artificial Intelligence Responsive Assistant Hub");
Console.WriteLine("Type a command, or type 'exit' to close.");
Console.WriteLine();

while (true)
{
    Console.Write("> ");

    string? input = Console.ReadLine();

    if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    var result = await assistant.HandleInputAsync(input ?? string.Empty);

    Console.WriteLine($"SAIRAH: {result.Message}");
}