using SAIRAH.Commands.Abstractions;
using SAIRAH.Shared.Commands;
using System.Diagnostics;

namespace SAIRAH.Commands.System
{
    public sealed class OpenWebsiteCommand : IAssistantCommand
    {
        public string Name => "Open Website";

        public CommandRiskLevel RiskLevel => CommandRiskLevel.Low;

        public bool CanHandle(CommandRequest request)
        {
            return request.Intent == "open_website";
        }

        public Task<CommandResult> ExecuteAsync(CommandRequest request)
        {
            if (!request.Parameters.TryGetValue("url", out string? url) ||
                string.IsNullOrWhiteSpace(url))
            {
                return Task.FromResult(CommandResult.Fail("No website was provided."));
            }

            string finalUrl = NormalizeUrl(url);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = finalUrl,
                    UseShellExecute = true
                });

                return Task.FromResult(CommandResult.Ok($"Opened website: {finalUrl}"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(CommandResult.Fail($"Failed to open website. {ex.Message}"));
            }
        }

        private static string NormalizeUrl(string url)
        {
            string cleaned = url.Trim();

            return cleaned.ToLowerInvariant() switch
            {
                "google" => "https://www.google.com",
                "youtube" => "https://www.youtube.com",
                "github" => "https://github.com",
                "chatgpt" => "https://chatgpt.com",
                _ when cleaned.StartsWith("http://", StringComparison.OrdinalIgnoreCase) => cleaned,
                _ when cleaned.StartsWith("https://", StringComparison.OrdinalIgnoreCase) => cleaned,
                _ => $"https://{cleaned}"
            };
        }
    }
}
