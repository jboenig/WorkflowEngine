using System;

namespace Headway.WorkflowEngine.Exceptions
{
    /// <summary>
    /// Exception thrown when a workflow has no initial
    /// state defined.
    /// </summary>
    public sealed class InitialStateNotFoundException : WorkflowException
    {
        /// <summary>
        /// 
        /// </summary>
        public InitialStateNotFoundException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                var msg = "Workflow has no initial state defined";
                return msg;
            }
        }
    }
}
