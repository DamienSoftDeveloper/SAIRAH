using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAIRAH.Shared.Workflows;
using System.Text.Json;

namespace SAIRAH.Storage.Workflows
{
    public sealed class JsonWorkflowRepository : IWorkflowRepository
    {
        private readonly string _workflowDirectory;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public JsonWorkflowRepository()
        {
            _workflowDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SAIRAH",
                "workflows");

            Directory.CreateDirectory(_workflowDirectory);
        }

        public async Task<IReadOnlyList<WorkflowDefinition>> GetAllAsync()
        {
            string[] files = Directory.GetFiles(_workflowDirectory, "*.json");

            List<WorkflowDefinition> workflows = new();

            foreach (string file in files)
            {
                try
                {
                    string json = await File.ReadAllTextAsync(file);
                    WorkflowDefinition? workflow = JsonSerializer.Deserialize<WorkflowDefinition>(json, _jsonOptions);

                    if (workflow != null)
                    {
                        workflows.Add(workflow);
                    }
                }
                catch
                {
                    // Later we can log invalid workflow files.
                }
            }

            return workflows;
        }

        public async Task<WorkflowDefinition?> GetByNameAsync(string workflowName)
        {
            IReadOnlyList<WorkflowDefinition> workflows = await GetAllAsync();

            return workflows.FirstOrDefault(w =>
                string.Equals(w.Name, workflowName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task SaveAsync(WorkflowDefinition workflow)
        {
            if (string.IsNullOrWhiteSpace(workflow.Name))
            {
                throw new InvalidOperationException("Workflow name cannot be empty.");
            }

            string safeFileName = MakeSafeFileName(workflow.Name);
            string path = Path.Combine(_workflowDirectory, $"{safeFileName}.json");

            string json = JsonSerializer.Serialize(workflow, _jsonOptions);

            await File.WriteAllTextAsync(path, json);
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar, '-');
            }

            return name.Trim().Replace(' ', '-').ToLowerInvariant();
        }
    }
}
