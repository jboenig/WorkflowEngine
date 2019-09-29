using System;
using Headway.Dynamo.Commands;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Result codes that can occur when an attempt is made to
    /// transition an <see cref="IWorkflowSubject"/> from one
    /// state to another.
    /// </summary>
    public enum WorkflowTransitionResultCode
    {
        /// <summary>
        /// Transition successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Transition not allowed based on failed prerequisite conditions.
        /// </summary>
        NotAllowed = 1,

        /// <summary>
        /// Indicates that at least one action failed.
        /// </summary>
        ActionFailed = 2,

        /// <summary>
        /// Indicates that an exception occurred during the transition
        /// </summary>
        Exception = 3
    }

    /// <summary>
    /// Encapsulates results that can occur an attempt is made to
    /// to transition an <see cref="IWorkflowSubject"/> from one
    /// state to another.
    /// </summary>
    public class WorkflowTransitionResult
    {
        /// <summary>
        /// Constructs a <see cref="WorkflowTransitionResult"/> given a result
        /// code and a description.
        /// </summary>
        /// <param name="resultCode">Code describing the result.</param>
        /// <param name="description">Text description of the result.</param>
        public WorkflowTransitionResult(WorkflowTransitionResultCode resultCode, string description = null)
        {
            this.ActionResult = CommandResult.Success;
            this.ResultCode = resultCode;
            this.Description = description;
        }

        /// <summary>
        /// Constructs a <see cref="WorkflowTransitionResult"/> given an
        /// exception object.
        /// </summary>
        /// <param name="ex">Exception that occurred during the transition.</param>
        public WorkflowTransitionResult(Exception ex)
        {
            this.ActionResult = CommandResult.Success;
            this.ResultCode = WorkflowTransitionResultCode.Exception;
            this.Description = ex.Message;
        }

        /// <summary>
        /// Constructs a <see cref="WorkflowTransitionResult"/> given an
        /// exception object.
        /// </summary>
        /// <param name="actionRes">
        /// Result of the last command executed.
        /// </param>
        public WorkflowTransitionResult(CommandResult actionRes)
        {
            if (actionRes == null)
            {
                this.ResultCode = WorkflowTransitionResultCode.Success;
                this.ActionResult = CommandResult.Success;
            }
            else
            {
                if (actionRes.IsSuccess)
                {
                    this.ResultCode = WorkflowTransitionResultCode.Success;
                }
                else
                {
                    this.ResultCode = WorkflowTransitionResultCode.ActionFailed;
                }
                this.Description = actionRes.Description;
                this.ActionResult = actionRes;
            }
        }

        /// <summary>
        /// Gets the result code.
        /// </summary>
        public WorkflowTransitionResultCode ResultCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Exception that occurred during a transition.
        /// </summary>
        /// <remarks>
        /// Null if no exception occurred.
        /// </remarks>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="CommandResult"/> from the
        /// actions executed.
        /// </summary>
        /// <remarks>
        /// If a failure occurs, this should be the first
        /// error encountered, since processing stops when
        /// an error occurs.
        /// </remarks>
        public CommandResult ActionResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a flag indicating whether or not the transition was successful.
        /// </summary>
        public bool IsSuccess
        {
            get { return (this.ResultCode == WorkflowTransitionResultCode.Success); }
        }

        /// <summary>
        /// Singleton object used to indicate success.
        /// </summary>
        public static WorkflowTransitionResult Success = new WorkflowTransitionResult(WorkflowTransitionResultCode.Success);
    }
}
