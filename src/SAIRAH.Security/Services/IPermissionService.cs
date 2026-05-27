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
    public interface IPermissionService
    {
        PermissionDecision CheckPermission(IAssistantCommand command, CommandRequest request);
    }
}
