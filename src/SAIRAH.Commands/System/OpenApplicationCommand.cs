using SAIRAH.Commands.Abstractions;
using SAIRAH.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Commands.System
{
    public sealed class OpenApplicationCommand : IAssistantCommand
    {
        public string Name => "Open Application";

        public CommandRiskLevel RiskLevel => CommandRiskLevel.Low;

        public bool CanHandle(CommandRequest request)
        {
            return request.Intent == "open_app";
        }

        public Task<CommandResult> ExecuteAsync(CommandRequest request)
        {
            if (!request.Parameters.TryGetValue("app", out string? appName) ||
                string.IsNullOrWhiteSpace(appName))
            {
                return Task.FromResult(CommandResult.Fail("No application name was provided."));
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = appName,
                    UseShellExecute = true
                });

                return Task.FromResult(CommandResult.Ok($"Opened {appName}."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(CommandResult.Fail($"Failed to open {appName}. {ex.Message}"));
            }
        }
    }
}
