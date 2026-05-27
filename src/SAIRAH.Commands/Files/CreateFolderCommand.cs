using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::SAIRAH.Commands.Abstractions;
using global::SAIRAH.Shared.Commands;

namespace SAIRAH.Commands.Files
{
    public sealed class CreateFolderCommand : IAssistantCommand
    {
        public string Name => "Create Folder";

        public CommandRiskLevel RiskLevel => CommandRiskLevel.Low;

        public bool CanHandle(CommandRequest request)
        {
            return request.Intent == "create_folder";
        }

        public Task<CommandResult> ExecuteAsync(CommandRequest request)
        {
            if (!request.Parameters.TryGetValue("name", out string? folderName) ||
                string.IsNullOrWhiteSpace(folderName))
            {
                return Task.FromResult(CommandResult.Fail("No folder name was provided."));
            }

            request.Parameters.TryGetValue("location", out string? locationName);

            string? locationPath = ResolveLocation(locationName ?? "desktop");

            if (locationPath == null)
            {
                return Task.FromResult(CommandResult.Fail($"I don't know the location '{locationName}'."));
            }

            string folderPath = Path.Combine(locationPath, folderName);

            try
            {
                if (Directory.Exists(folderPath))
                {
                    return Task.FromResult(CommandResult.Ok($"Folder already exists: {folderPath}"));
                }

                Directory.CreateDirectory(folderPath);

                return Task.FromResult(CommandResult.Ok($"Created folder: {folderPath}"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(CommandResult.Fail($"Failed to create folder. {ex.Message}"));
            }
        }

        private static string? ResolveLocation(string locationName)
        {
            string key = locationName.Trim().ToLowerInvariant();

            return key switch
            {
                "desktop" => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "documents" => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "downloads" => Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"),
                _ => null
            };
        }
    }
}
