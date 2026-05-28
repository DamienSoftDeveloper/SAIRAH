using SAIRAH.Shared.Workflows;
using System.Text.Json;

namespace SAIRAH.Storage.Workflows
{
    public sealed class JsonWorkflowRepository : IWorkflowRepository
    {
        private readonly WorkflowPathProvider _pathProvider;
        private readonly DefaultWorkflowSeeder _defaultWorkflowSeeder;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public JsonWorkflowRepository()
            : this(CreateDefaultPathProvider())
        {
        }

        private JsonWorkflowRepository(WorkflowPathProvider pathProvider)
            : this(pathProvider, new DefaultWorkflowSeeder(pathProvider))
        {
        }

        public JsonWorkflowRepository(
            WorkflowPathProvider pathProvider,
            DefaultWorkflowSeeder defaultWorkflowSeeder)
        {
            _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
            _defaultWorkflowSeeder = defaultWorkflowSeeder ?? throw new ArgumentNullException(nameof(defaultWorkflowSeeder));

            EnsureReady();
        }

        public async Task<IReadOnlyList<WorkflowDefinition>> GetAllAsync()
        {
            EnsureReady();

            string[] files = Directory.GetFiles(
                _pathProvider.UserWorkflowsPath,
                "*.json",
                SearchOption.TopDirectoryOnly);

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
                    // Later we can connect this to SAIRAH.Storage.Logging.
                    // For now, invalid workflow files are skipped so SAIRAH does not crash.
                }
            }

            return workflows
                .OrderBy(workflow => workflow.Name)
                .ToList();
        }

        public async Task<WorkflowDefinition?> GetByNameAsync(string workflowName)
        {
            if (string.IsNullOrWhiteSpace(workflowName))
            {
                return null;
            }

            string searchName = workflowName.Trim();

            if (searchName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                searchName = Path.GetFileNameWithoutExtension(searchName);
            }

            IReadOnlyList<WorkflowDefinition> workflows = await GetAllAsync();

            return workflows.FirstOrDefault(workflow =>
                MatchesWorkflowName(workflow, searchName));
        }

        public async Task SaveAsync(WorkflowDefinition workflow)
        {
            if (workflow == null)
            {
                throw new ArgumentNullException(nameof(workflow));
            }

            if (string.IsNullOrWhiteSpace(workflow.Name))
            {
                throw new InvalidOperationException("Workflow name cannot be empty.");
            }

            EnsureReady();

            string safeFileName = MakeSafeFileName(workflow.Name);
            string path = Path.Combine(_pathProvider.UserWorkflowsPath, $"{safeFileName}.json");

            string json = JsonSerializer.Serialize(workflow, _jsonOptions);

            await File.WriteAllTextAsync(path, json);
        }

        public string GetWorkflowDirectoryPath()
        {
            EnsureReady();
            return _pathProvider.UserWorkflowsPath;
        }

        private void EnsureReady()
        {
            _pathProvider.EnsureUserWorkflowFolderExists();
            _defaultWorkflowSeeder.SeedIfNeeded();
        }

        private static WorkflowPathProvider CreateDefaultPathProvider()
        {
            return new WorkflowPathProvider();
        }

        private static bool MatchesWorkflowName(WorkflowDefinition workflow, string searchName)
        {
            if (workflow == null || string.IsNullOrWhiteSpace(searchName))
            {
                return false;
            }

            string workflowName = workflow.Name.Trim();

            return string.Equals(workflowName, searchName, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(MakeSafeFileName(workflowName), MakeSafeFileName(searchName), StringComparison.OrdinalIgnoreCase);
        }

        private static string MakeSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "workflow";
            }

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar, '-');
            }

            return name
                .Trim()
                .Replace(' ', '-')
                .ToLowerInvariant();
        }
    }
}
