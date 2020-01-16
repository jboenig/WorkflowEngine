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
