////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.WorkflowEngine.
//
// Headway.WorkflowEngine is free software: you can redistribute it and/or
// modify it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
//
// Headway.WorkflowEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR PARTICULAR PURPOSE. See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with Headway.WorkflowEngine. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using System;
using Headway.Dynamo.Commands;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Result codes that can occur when an attempt is made to
    /// start or transition an <see cref="IWorkflowSubject"/> from
    /// one state to another.
    /// </summary>
    public enum WorkflowExecutionResultCode
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
    public class WorkflowExecutionResult
    {
        /// <summary>
        /// Constructs a <see cref="WorkflowExecutionResult"/> given a result
        /// code and a description.
        /// </summary>
        /// <param name="resultCode">Code describing the result.</param>
        /// <param name="description">Text description of the result.</param>
        public WorkflowExecutionResult(WorkflowExecutionResultCode resultCode, string description = null)
        {
            this.ActionResult = CommandResult.Success;
            this.ResultCode = resultCode;
            this.Description = description;
        }

        /// <summary>
        /// Constructs a <see cref="WorkflowExecutionResult"/> given an
        /// exception object.
        /// </summary>
        /// <param name="ex">Exception that occurred during the transition.</param>
        public WorkflowExecutionResult(Exception ex)
        {
            this.ActionResult = CommandResult.Success;
            this.ResultCode = WorkflowExecutionResultCode.Exception;
            this.Description = ex.Message;
        }

        /// <summary>
        /// Constructs a <see cref="WorkflowExecutionResult"/> given an
        /// exception object.
        /// </summary>
        /// <param name="actionRes">
        /// Result of the last command executed.
        /// </param>
        public WorkflowExecutionResult(CommandResult actionRes)
        {
            if (actionRes == null)
            {
                this.ResultCode = WorkflowExecutionResultCode.Success;
                this.ActionResult = CommandResult.Success;
            }
            else
            {
                if (actionRes.IsSuccess)
                {
                    this.ResultCode = WorkflowExecutionResultCode.Success;
                }
                else
                {
                    this.ResultCode = WorkflowExecutionResultCode.ActionFailed;
                }
                this.Description = actionRes.Description;
                this.ActionResult = actionRes;
            }
        }

        /// <summary>
        /// Gets the result code.
        /// </summary>
        public WorkflowExecutionResultCode ResultCode
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
            get { return (this.ResultCode == WorkflowExecutionResultCode.Success); }
        }

        /// <summary>
        /// Singleton object used to indicate success.
        /// </summary>
        public static WorkflowExecutionResult Success = new WorkflowExecutionResult(WorkflowExecutionResultCode.Success);
    }
}
