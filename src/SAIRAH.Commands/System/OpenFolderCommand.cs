using SAIRAH.Commands.Abstractions;
using SAIRAH.Shared.Commands;
using System.Diagnostics;

namespace SAIRAH.Commands.System
{
    public sealed class OpenFolderCommand : IAssistantCommand
    {
        public string Name => "Open Folder";

        public CommandRiskLevel RiskLevel => CommandRiskLevel.Low;

        public bool CanHandle(CommandRequest request)
        {
            return request.Intent == "open_folder";
        }

        public Task<CommandResult> ExecuteAsync(CommandRequest request)
        {
            if (!request.Parameters.TryGetValue("folder", out string? folderName) ||
                string.IsNullOrWhiteSpace(folderName))
            {
                return Task.FromResult(CommandResult.Fail("No folder name was provided."));
            }

            string? folderPath = ResolveKnownFolder(folderName);

            if (folderPath == null)
            {
                return Task.FromResult(CommandResult.Fail($"I don't know the folder '{folderName}' yet."));
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = folderPath,
                    UseShellExecute = true
                });

                return Task.FromResult(CommandResult.Ok($"Opened {folderName}."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(CommandResult.Fail($"Failed to open {folderName}. {ex.Message}"));
            }
        }

        private static string? ResolveKnownFolder(string folderName)
        {
            string key = folderName.Trim().ToLowerInvariant();

            return key switch
            {
                "desktop" => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "documents" => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "downloads" => Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"),
                "pictures" => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "music" => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                "videos" => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                _ => null
            };
        }
    }
}
