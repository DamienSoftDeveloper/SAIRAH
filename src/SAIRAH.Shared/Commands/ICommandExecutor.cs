using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Shared.Commands
{
    public interface ICommandExecutor
    {
        Task<CommandResult> ExecuteRawAsync(string input);
    }
}
