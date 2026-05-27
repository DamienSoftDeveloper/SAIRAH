using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Shared.Commands
{
    public sealed class CommandRequest
    {
        public string RawInput { get; init; } = string.Empty;

        public string Intent { get; init; } = string.Empty;

        public Dictionary<string, string> Parameters { get; init; } = new();
    }
}
