using SAIRAH.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Core.Parsing
{
    public interface ICommandParser
    {
        CommandRequest Parse(string input);
    }
}
