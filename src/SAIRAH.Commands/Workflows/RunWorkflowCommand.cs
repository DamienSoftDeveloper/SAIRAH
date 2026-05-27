using SAIRAH.Commands.Abstractions;
using SAIRAH.Shared.Commands;
using SAIRAH.Storage.Workflows;

namespace SAIRAH.Commands.Workflows
{
    public sealed class RunWorkflowCommand : IAssistantCommand
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly ICommandExecutor _commandExecutor;

        public RunWorkflowCommand(
            IWorkflowRepository workflowRepository,
            ICommandExecutor commandExecutor)
        {
            _workflowRepository = workflowRepository;
            _commandExecutor = commandExecutor;
        }

        public string Name => "Run Workflow";

        public CommandRiskLevel RiskLevel => CommandRiskLevel.Low;

        public bool CanHandle(CommandRequest request)
        {
            return request.Intent == "run_workflow";
        }

        public async Task<CommandResult> ExecuteAsync(CommandRequest request)
        {
            if (!request.Parameters.TryGetValue("name", out string? workflowName) ||
                string.IsNullOrWhiteSpace(workflowName))
            {
                return CommandResult.Fail("No workflow name was provided.");
            }

            var workflow = await _workflowRepository.GetByNameAsync(workflowName);

            if (workflow == null)
            {
                return CommandResult.Fail($"I could not find a workflow called '{workflowName}'.");
            }

            if (workflow.Steps.Count == 0)
            {
                return CommandResult.Fail($"Workflow '{workflow.Name}' has no steps.");
            }

            List<string> messages = new();

            foreach (var step in workflow.Steps)
            {
                if (string.IsNullOrWhiteSpace(step.Command))
                {
                    continue;
                }

                CommandResult stepResult = await _commandExecutor.ExecuteRawAsync(step.Command);

                messages.Add($"{step.Command} → {stepResult.Message}");

                if (!stepResult.Success)
                {
                    return CommandResult.Fail(
                        $"Workflow '{workflow.Name}' stopped because a step failed.\n" +
                        string.Join(Environment.NewLine, messages));
                }
            }

            return CommandResult.Ok(
                $"Workflow '{workflow.Name}' completed.\n" +
                string.Join(Environment.NewLine, messages));
        }
    }
}
