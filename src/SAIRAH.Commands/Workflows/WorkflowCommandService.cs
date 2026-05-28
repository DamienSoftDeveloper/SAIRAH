using SAIRAH.Commands.Abstractions;
using SAIRAH.Shared.Commands;
using SAIRAH.Shared.Workflows;
using SAIRAH.Storage.Workflows;
using System.Collections;
using System.Text;

namespace SAIRAH.Commands.Workflows
{
    public sealed class WorkflowCommandService : IAssistantCommand
    {
        private readonly IWorkflowRepository _workflowRepository;

        public string Name => "workflow";

        // If your enum does not have Low, use whatever your project uses:
        // Safe, None, LowRisk, etc.
        public CommandRiskLevel RiskLevel => CommandRiskLevel.Low;

        public WorkflowCommandService(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public bool CanHandle(CommandRequest request)
        {
            string input = GetRequestInput(request);
            return CanHandleInput(input);
        }

        public async Task<CommandResult> ExecuteAsync(CommandRequest request)
        {
            string input = GetRequestInput(request);
            string message = await HandleInputAsync(input);

            return CreateSuccessResult(message);
        }

        private static bool CanHandleInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string command = input.Trim();

            return command.Equals("list workflows", StringComparison.OrdinalIgnoreCase)
                   || command.Equals("reload workflows", StringComparison.OrdinalIgnoreCase)
                   || command.StartsWith("show workflow ", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string> HandleInputAsync(string input)
        {
            string command = input.Trim();

            if (command.Equals("list workflows", StringComparison.OrdinalIgnoreCase))
            {
                return await ListWorkflowsAsync();
            }

            if (command.Equals("reload workflows", StringComparison.OrdinalIgnoreCase))
            {
                return await ReloadWorkflowsAsync();
            }

            if (command.StartsWith("show workflow ", StringComparison.OrdinalIgnoreCase))
            {
                string workflowName = command["show workflow ".Length..].Trim();
                return await ShowWorkflowAsync(workflowName);
            }

            return "I could not understand that workflow command.";
        }

        private async Task<string> ListWorkflowsAsync()
        {
            IReadOnlyList<WorkflowDefinition> workflows = await _workflowRepository.GetAllAsync();

            if (workflows.Count == 0)
            {
                return "I could not find any workflows yet.";
            }

            StringBuilder builder = new();

            builder.AppendLine($"I found {workflows.Count} workflow(s):");

            foreach (WorkflowDefinition workflow in workflows)
            {
                string? description = ReadTextProperty(workflow, "Description");

                if (string.IsNullOrWhiteSpace(description))
                {
                    builder.AppendLine($"- {workflow.Name}");
                }
                else
                {
                    builder.AppendLine($"- {workflow.Name}: {description}");
                }
            }

            return builder.ToString().Trim();
        }

        private async Task<string> ReloadWorkflowsAsync()
        {
            IReadOnlyList<WorkflowDefinition> workflows = await _workflowRepository.GetAllAsync();

            return $"Workflows reloaded. I found {workflows.Count} workflow(s).";
        }

        private async Task<string> ShowWorkflowAsync(string workflowName)
        {
            if (string.IsNullOrWhiteSpace(workflowName))
            {
                return "Please provide a workflow name. Example: show workflow open-notepad";
            }

            WorkflowDefinition? workflow = await _workflowRepository.GetByNameAsync(workflowName);

            if (workflow == null)
            {
                return $"I could not find a workflow called '{workflowName}'.";
            }

            StringBuilder builder = new();

            builder.AppendLine($"Workflow: {workflow.Name}");

            string? description = ReadTextProperty(workflow, "Description");

            if (!string.IsNullOrWhiteSpace(description))
            {
                builder.AppendLine($"Description: {description}");
            }

            object? stepsObject = workflow
                .GetType()
                .GetProperty("Steps")
                ?.GetValue(workflow);

            if (stepsObject is not IEnumerable steps)
            {
                builder.AppendLine();
                builder.AppendLine("Steps: No steps found.");
                return builder.ToString().Trim();
            }

            builder.AppendLine();
            builder.AppendLine("Steps:");

            int index = 1;

            foreach (object? step in steps)
            {
                if (step == null)
                {
                    continue;
                }

                builder.AppendLine($"{index}. {BuildStepSummary(step)}");
                index++;
            }

            if (index == 1)
            {
                builder.AppendLine("No steps found.");
            }

            return builder.ToString().Trim();
        }

        private static string BuildStepSummary(object? step)
        {
            if (step == null)
            {
                return "Unknown step";
            }

            string? name = ReadTextProperty(step, "Name", "Title", "Description");
            string? action = ReadTextProperty(step, "Action", "Type");
            string? target = ReadTextProperty(step, "Target", "Command", "Path", "Value");

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(target))
            {
                return $"{name} -> {target}";
            }

            if (!string.IsNullOrWhiteSpace(action) && !string.IsNullOrWhiteSpace(target))
            {
                return $"{action} -> {target}";
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                return action;
            }

            return step.ToString() ?? "Unknown step";
        }

        private static string? ReadTextProperty(object? obj, params string[] propertyNames)
        {
            if (obj == null)
            {
                return null;
            }

            Type type = obj.GetType();

            foreach (string propertyName in propertyNames)
            {
                object? value = type.GetProperty(propertyName)?.GetValue(obj);

                if (value == null)
                {
                    continue;
                }

                string? text = value.ToString();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }

            return null;
        }

        private static string GetRequestInput(CommandRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            // This makes it flexible in case your CommandRequest property is called
            // Input, RawInput, Text, Message, etc.
            string[] possiblePropertyNames =
            {
                "Input",
                "RawInput",
                "Text",
                "Message",
                "Command",
                "CommandText",
                "UserInput",
                "Content"
            };

            Type type = request.GetType();

            foreach (string propertyName in possiblePropertyNames)
            {
                object? value = type.GetProperty(propertyName)?.GetValue(request);

                if (value is string text && !string.IsNullOrWhiteSpace(text))
                {
                    return text.Trim();
                }
            }

            return request.ToString()?.Trim() ?? string.Empty;
        }

        private static CommandResult CreateSuccessResult(string message)
        {
            // Use this if your CommandResult has a Success factory method.
            return CommandResult.Ok(message);

            // If your CommandResult does NOT have Success(message),
            // replace the line above with the same return style used by your other commands.
            //
            // Example alternative:
            //
            // return new CommandResult
            // {
            //     Message = message,
            //     WasSuccessful = true
            // };
        }
    }
}