using System;

namespace Headway.WorkflowEngine.Exceptions
{
    /// <summary>
    /// Exception thrown when an expected state cannot be found
    /// in a workflow.
    /// </summary>
    public sealed class StateNotFoundException : WorkflowException
    {
        private Workflow workflow;
        private string stateName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflow"></param>
        /// <param name="stateName"></param>
        public StateNotFoundException(Workflow workflow, string stateName)
        {
            this.workflow = workflow;
            this.stateName = stateName;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                var msg = string.Format("State named {0} does not existing in the {1} workflow", this.stateName, this.workflow.FullName);
                throw new InvalidOperationException(msg);
            }
        }
    }
}
