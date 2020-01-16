////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.WorkflowEngine
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

using System;
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Exceptions;

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
        /// Returns a <see cref="WorkflowExecutionResult"/> object
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
        public static WorkflowExecutionResult TransitionTo(this IWorkflowSubject workflowSubject,
            string transitionName,
            IServiceProvider serviceProvider)
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
