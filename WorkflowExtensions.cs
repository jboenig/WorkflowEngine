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
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Extension methods for the <see cref="IWorkflowSubject"/> interface.
    /// </summary>
    public static class WorkflowSubjectExtensions
    {
        /// <summary>
        /// Transitions the workflow subject to a new state along the
        /// specified transition.
        /// </summary>
        /// <param name="serviceProvider">
        /// Interface to service provider.
        /// </param>
        /// <param name="workflowSubject">
        /// Workflow subject to transition to a new state.
        /// </param>
        /// <param name="transitionName">
        /// Name of transition to follow.
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowTransitionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when workflowSubject or transitionName is null.
        /// </exception>
        /// <exception cref="StateNotFoundException">
        /// Thrown when the current state of the workflow subject cannot be
        /// found in the workflow.
        /// </exception>
        /// <exception cref="TransitionNotFoundException">
        /// Thrown when the specified transitionName cannot be found
        /// in the FROM state.
        /// </exception>
        /// <exception cref="ActionFailedException">
        /// Thrown when an action fails exiting a state, transitioning,
        /// or entering a state.
        /// </exception>
        public static WorkflowTransitionResult TransitionTo(this IWorkflowSubject workflowSubject, string transitionName, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                throw new ArgumentNullException(nameof(transitionName));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var workflowName = workflowSubject.WorkflowName;
            if (string.IsNullOrEmpty(workflowName))
            {
                throw new InvalidOperationException("No workflow associated with this workflow subject");
            }

            var workflowResolver = serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            if (workflowResolver == null)
            {
                throw new ServiceNotFoundException(typeof(IWorkflowByNameResolver));
            }

            var workflow = workflowResolver.Resolve(workflowName);
            if (workflow == null)
            {
                var msg = string.Format("Workflow {0} not found", workflowName);
                throw new InvalidOperationException(msg);
            }

            return workflow.TransitionTo(workflowSubject, transitionName, serviceProvider);
        }
    }
}
