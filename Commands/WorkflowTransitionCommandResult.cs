using Headway.Dynamo.Commands;

namespace Headway.WorkflowEngine.Commands
{
    /// <summary>
    /// Implements the <see cref="CommandResult"/> abstract class
    /// for a <see cref="WorkflowTransitionCommandResult"/> object.
    /// </summary>
    /// <remarks>This class is a simple wrapper</remarks>
    public sealed class WorkflowTransitionCommandResult : CommandResult
    {
        private readonly WorkflowTransitionResult workflowRes;

        /// <summary>
        /// Constructs a <see cref="WorkflowTransitionCommandResult"/>
        /// given a <see cref="WorkflowTransitionResult"/>
        /// </summary>
        /// <param name="workflowRes">
        /// The <see cref="WorkflowTransitionResult"/> object that this
        /// object wraps.
        /// </param>
        public WorkflowTransitionCommandResult(WorkflowTransitionResult workflowRes)
        {
            this.workflowRes = workflowRes;
        }

        /// <summary>
        /// Gets the description of the result
        /// </summary>
        public override string Description
        {
            get { return workflowRes.Description; }
        }

        /// <summary>
        /// Gets a flag indicating whether or not the command
        /// was successful.
        /// </summary>
        public override bool IsSuccess
        {
            get { return workflowRes.IsSuccess; }
        }
    }
}
