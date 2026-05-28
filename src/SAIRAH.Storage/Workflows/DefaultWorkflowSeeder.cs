
namespace SAIRAH.Storage.Workflows
{
    public sealed class DefaultWorkflowSeeder
    {
        private readonly WorkflowPathProvider _pathProvider;

        public DefaultWorkflowSeeder(WorkflowPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public int SeedIfNeeded()
        {
            _pathProvider.EnsureUserWorkflowFolderExists();

            if (_pathProvider.UserWorkflowFolderHasWorkflows())
                return 0;

            if (!_pathProvider.TemplateFolderHasWorkflows())
                return 0;

            int copiedCount = 0;

            foreach (string sourceFile in Directory.EnumerateFiles(_pathProvider.TemplateWorkflowsPath!, "*.json"))
            {
                string fileName = Path.GetFileName(sourceFile);
                string destinationFile = Path.Combine(_pathProvider.UserWorkflowsPath, fileName);

                if (File.Exists(destinationFile))
                    continue;

                File.Copy(sourceFile, destinationFile);
                copiedCount++;
            }

            return copiedCount;
        }

      
    }
}


