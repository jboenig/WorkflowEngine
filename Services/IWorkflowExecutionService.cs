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

namespace Headway.WorkflowEngine.Services
{
    /// <summary>
    /// Interface to service that executes workflows on
    /// against <see cref="IWorkflowSubject"/> objects.
    /// </summary>
    public interface IWorkflowExecutionService
    {
        /// <summary>
        /// Starts execution of an <see cref="IWorkflowSubject"/> object
        /// in the specified workflow.
        /// </summary>
        /// <param name="workflowSubject">
        /// <see cref="IWorkflowSubject"/> object to start in the
        /// workflow
        /// </param>
        /// <param name="workflowName">
        /// Fully-qualified name of the workflow to execute
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowExecutionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        WorkflowExecutionResult StartWorkflow(IWorkflowSubject workflowSubject, string workflowName);

        /// <summary>
        /// Transitions the specified workflow subject to a
        /// new state along a given transition.
        /// </summary>
        /// <param name="workflowSubject">
        /// <see cref="IWorkflowSubject"/> object to transition
        /// </param>
        /// <param name="transitionName">
        /// Name of transition to execute
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowExecutionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        WorkflowExecutionResult TransitionTo(IWorkflowSubject workflowSubject, string transitionName);
    }

    /// <summary>
    /// Extension methods for the <see cref="IWorkflowExecutionService"/>
    /// interface.
    /// </summary>
    public static class WorkflowExecutionServiceExtensions
    {
        /// <summary>
        /// Starts execution of an <see cref="IWorkflowSubject"/> object
        /// in the workflow associated with the object.
        /// </summary>
        /// <param name="workflowTransitionService"></param>
        /// <param name="workflowSubject"></param>
        /// <returns>
        /// Returns a <see cref="WorkflowExecutionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        /// <remarks>
        /// This is a short-cut method for <see cref="IWorkflowSubject"/>
        /// objects that already have a value assigned to
        /// <see cref="IWorkflowSubject.WorkflowName"/>.
        /// </remarks>
        public static WorkflowExecutionResult StartWorkflow(this IWorkflowExecutionService workflowTransitionService,
            IWorkflowSubject workflowSubject)
        {
            if (workflowSubject == null)
            {
                throw new ArgumentNullException(nameof(workflowSubject));
            }

            var workflowName = workflowSubject.WorkflowName;
            if (string.IsNullOrEmpty(workflowName))
            {
                var msg = $"{workflowSubject} is not associated with a workflow";
                throw new InvalidOperationException(msg);
            }

            return workflowTransitionService.StartWorkflow(workflowSubject, workflowName);
        }
    }
}
