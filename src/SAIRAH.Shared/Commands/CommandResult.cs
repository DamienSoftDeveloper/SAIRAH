using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Shared.Commands
{
    public sealed class CommandResult
    {
        public bool Success { get; init; }

        public string Message { get; init; } = string.Empty;

        public static CommandResult Ok(string message)
        {
            return new CommandResult
            {
                Success = true,
                Message = message
            };
        }

        public static CommandResult Fail(string message)
        {
            return new CommandResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
