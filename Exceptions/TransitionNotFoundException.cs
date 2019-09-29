using System;

namespace Headway.WorkflowEngine.Exceptions
{
    /// <summary>
    /// Exception thrown when an expected transition cannot be
    /// found in a state.
    /// </summary>
    public sealed class TransitionNotFoundException : WorkflowException
    {
        private WorkflowState state;
        private string transitionName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="transitionName"></param>
        public TransitionNotFoundException(WorkflowState state, string transitionName)
        {
            this.state = state;
            this.transitionName = transitionName;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                var msg = string.Format("Transition named {0} does not existing in the {1} state", this.transitionName, this.state.Name);
                throw new InvalidOperationException(msg);
            }
        }
    }
}
