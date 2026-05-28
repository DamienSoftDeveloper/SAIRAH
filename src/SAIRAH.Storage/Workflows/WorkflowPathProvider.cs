
namespace SAIRAH.Storage.Workflows
{
    public sealed class WorkflowPathProvider
    {
        private const string AppName = "SAIRAH";

        public string UserDataRoot { get; }
        public string UserWorkflowsPath { get; }
        public string? TemplateWorkflowsPath { get; }

        public WorkflowPathProvider()
        {
            UserDataRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppName,
                "Data"
            );

            UserWorkflowsPath = Path.Combine(UserDataRoot, "Workflows");
            TemplateWorkflowsPath = FindTemplateWorkflowFolder();
        }

        public void EnsureUserWorkflowFolderExists()
        {
            Directory.CreateDirectory(UserDataRoot);
            Directory.CreateDirectory(UserWorkflowsPath);
        }

        public bool UserWorkflowFolderHasWorkflows()
        {
            EnsureUserWorkflowFolderExists();

            return Directory
                .EnumerateFiles(UserWorkflowsPath, "*.json", SearchOption.TopDirectoryOnly)
                .Any();
        }

        public bool TemplateFolderHasWorkflows()
        {
            return TemplateWorkflowsPath is not null
                   && Directory.Exists(TemplateWorkflowsPath)
                   && Directory.EnumerateFiles(TemplateWorkflowsPath, "*.json", SearchOption.TopDirectoryOnly).Any();
        }

        private static string? FindTemplateWorkflowFolder()
        {
            string baseDirectory = AppContext.BaseDirectory;
            DirectoryInfo? current = new(baseDirectory);

            while (current is not null)
            {
                string lowercaseCandidate = Path.Combine(current.FullName, "data", "WorkflowTemplates");
                string uppercaseCandidate = Path.Combine(current.FullName, "Data", "WorkflowTemplates");

                if (Directory.Exists(lowercaseCandidate))
                    return lowercaseCandidate;

                if (Directory.Exists(uppercaseCandidate))
                    return uppercaseCandidate;

                current = current.Parent;
            }

            return null;
        }
    }
}
