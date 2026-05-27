using SAIRAH.Shared.Workflows;

namespace SAIRAH.Storage.Workflows
{
    public sealed class DefaultWorkflowSeeder
    {
        private readonly IWorkflowRepository _workflowRepository;

        public DefaultWorkflowSeeder(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public async Task SeedAsync()
        {
            await SeedWorkflowAsync(new WorkflowDefinition
            {
                Name = "dev mode",
                Description = "Basic development workspace test workflow.",
                Steps =
                [
                    new WorkflowStep { Command = "open notepad" },
                new WorkflowStep { Command = "open website github" },
                new WorkflowStep { Command = "open folder documents" }
                ]
            });

            await SeedWorkflowAsync(new WorkflowDefinition
            {
                Name = "web mode",
                Description = "Opens common web tools.",
                Steps =
                [
                    new WorkflowStep { Command = "open website google" },
                new WorkflowStep { Command = "open website youtube" },
                new WorkflowStep { Command = "open website github" }
                ]
            });

            await SeedWorkflowAsync(new WorkflowDefinition
            {
                Name = "sairah dev",
                Description = "Opens common tools for developing SAIRAH.",
                Steps =
                [
                    new WorkflowStep { Command = "open website github" },
                new WorkflowStep { Command = "open folder documents" }
                ]
            });
        }

        private async Task SeedWorkflowAsync(WorkflowDefinition workflow)
        {
            WorkflowDefinition? existingWorkflow = await _workflowRepository.GetByNameAsync(workflow.Name);

            if (existingWorkflow != null)
            {
                return;
            }

            await _workflowRepository.SaveAsync(workflow);
        }
    }
}

