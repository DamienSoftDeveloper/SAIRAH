using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAIRAH.Shared.Commands;


namespace SAIRAH.Commands.Abstractions
{
    public interface IAssistantCommand
    {
        string Name { get; }

        CommandRiskLevel RiskLevel { get; }

        bool CanHandle(CommandRequest request);

        Task<CommandResult> ExecuteAsync(CommandRequest request);
    }
}
