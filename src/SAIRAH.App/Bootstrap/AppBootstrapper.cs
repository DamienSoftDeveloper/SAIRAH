using SAIRAH.Commands.Abstractions;
using SAIRAH.Commands.Files;
using SAIRAH.Commands.System;
using SAIRAH.Commands.Workflows;
using SAIRAH.Core.Parsing;
using SAIRAH.Core.Routing;
using SAIRAH.Core.Services;
using SAIRAH.Security.Services;
using SAIRAH.Storage.Logging;
using SAIRAH.Storage.Workflows;

namespace SAIRAH.App.Bootstrap
{
    public static class AppBootstrapper
    {
        public static AssistantEngine CreateAssistantEngine()
        {
            ICommandParser parser = new BasicCommandParser();

            IPermissionService permissionService = new PermissionService();

            CommandRouter router = new CommandRouter(
                Array.Empty<IAssistantCommand>(),
                permissionService);

            IAssistantLogger logger = new FileAssistantLogger();

            AssistantEngine assistantEngine = new AssistantEngine(parser, router, logger);

            IWorkflowRepository workflowRepository = new JsonWorkflowRepository();

            router.AddCommand(new OpenApplicationCommand());
            router.AddCommand(new OpenFolderCommand());
            router.AddCommand(new OpenWebsiteCommand());
            router.AddCommand(new CreateFolderCommand());
            router.AddCommand(new RunWorkflowCommand(workflowRepository, assistantEngine));
            router.AddCommand(new WorkflowCommandService(workflowRepository));

            return assistantEngine;
        }
    }
}
