using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Shared.Workflows
{
    public sealed class WorkflowDefinition
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<WorkflowStep> Steps { get; set; } = new();
    }
}
