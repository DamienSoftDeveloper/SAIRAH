using SAIRAH.Commands.Abstractions;
using SAIRAH.Shared.Commands;
using SAIRAH.Shared.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Security.Services
{
    public sealed class PermissionService : IPermissionService
    {
        public PermissionDecision CheckPermission(IAssistantCommand command, CommandRequest request)
        {
            return command.RiskLevel switch
            {
                CommandRiskLevel.Safe => PermissionDecision.Allow,
                CommandRiskLevel.Low => PermissionDecision.Allow,
                CommandRiskLevel.Medium => PermissionDecision.Ask,
                CommandRiskLevel.High => PermissionDecision.Ask,
                CommandRiskLevel.Critical => PermissionDecision.Deny,
                _ => PermissionDecision.Deny
            };
        }
    }
}

