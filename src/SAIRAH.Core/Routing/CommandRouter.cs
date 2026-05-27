using SAIRAH.Commands.Abstractions;
using SAIRAH.Security.Services;
using SAIRAH.Shared.Commands;
using SAIRAH.Shared.Permissions;

namespace SAIRAH.Core.Routing
{
    public sealed class CommandRouter
    {
        private readonly List<IAssistantCommand> _commands;
        private readonly IPermissionService _permissionService;

        public CommandRouter(
            IEnumerable<IAssistantCommand> commands,
            IPermissionService permissionService)
        {
            _commands = commands.ToList();
            _permissionService = permissionService;
        }

        public void AddCommand(IAssistantCommand command)
        {
            _commands.Add(command);
        }

        public async Task<CommandResult> ExecuteAsync(CommandRequest request)
        {
            IAssistantCommand? command = _commands.FirstOrDefault(c => c.CanHandle(request));

            if (command == null)
            {
                return CommandResult.Fail("I don't know how to handle that command yet.");
            }

            PermissionDecision decision = _permissionService.CheckPermission(command, request);

            if (decision == PermissionDecision.Deny)
            {
                return CommandResult.Fail("That action is blocked by SAIRAH's permission system.");
            }

            if (decision == PermissionDecision.Ask)
            {
                return CommandResult.Fail("This action requires confirmation. Confirmation UI will be added later.");
            }

            return await command.ExecuteAsync(request);
        }
    }
}

