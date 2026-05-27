using SAIRAH.Shared.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIRAH.Storage.Workflows
{

    public interface IWorkflowRepository
    {
        Task<IReadOnlyList<WorkflowDefinition>> GetAllAsync();

        Task<WorkflowDefinition?> GetByNameAsync(string workflowName);

        Task SaveAsync(WorkflowDefinition workflow);
    }
}
