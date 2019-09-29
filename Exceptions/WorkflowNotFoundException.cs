using System;

namespace Headway.WorkflowEngine.Exceptions
{
    public sealed class WorkflowNotFoundException : WorkflowException
    {
        public WorkflowNotFoundException(string workflowFullName) :
            base(string.Format("Workflow {0} not found", workflowFullName), null)
        {
        }
    }
}
