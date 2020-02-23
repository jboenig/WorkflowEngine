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
using System.Linq;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets the collection of all transitions that are available
        /// for the given <see cref="IWorkflowSubject"/> from its
        /// <see cref="IWorkflowSubject.CurrentState"/>.
        /// </summary>
        /// <param name="workflowSubject">
        /// Workflow subject to get transitions for
        /// </param>
        /// <returns>
        /// Collection of <see cref="WorkflowTransition"/> objects
        /// that can be taken from the current state of the given
        /// <see cref="IWorkflowSubject"/> object.
        /// </returns>
        IEnumerable<WorkflowTransition> GetAllTransitions(IWorkflowSubject workflowSubject);

        /// <summary>
        /// Gets the collection of allowed transitions that are available
        /// for the given <see cref="IWorkflowSubject"/> from its
        /// <see cref="IWorkflowSubject.CurrentState"/>.
        /// </summary>
        /// <param name="workflowSubject">
        /// Workflow subject to get transitions for
        /// </param>
        /// <returns>
        /// Collection of <see cref="WorkflowTransition"/> objects
        /// that can be taken from the current state of the given
        /// <see cref="IWorkflowSubject"/> object.
        /// </returns>
        IEnumerable<WorkflowTransition> GetAllowedTransitions(IWorkflowSubject workflowSubject);

        /// <summary>
        /// Gets the current <see cref="WorkflowExecutionFrame"/> for the specified
        /// <see cref="IWorkflowSubject"/>.
        /// </summary>
        /// <param name="workflowSubject">
        /// Workflow subject to get execution frame for
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowExecutionFrame"/> that describes the state of execution
        /// for the given <see cref="IWorkflowSubject"/>.
        /// </returns>
        WorkflowExecutionFrame GetCurrentExecutionFrame(IWorkflowSubject workflowSubject);
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
        /// <param name="workflowExeService">
        /// Reference to the <see cref="IWorkflowExecutionService"/>
        /// to call
        /// </param>
        /// <param name="workflowSubject">
        /// <see cref="IWorkflowSubject"/> to start in the workflow
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowExecutionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        /// <remarks>
        /// This is a short-cut method for <see cref="IWorkflowSubject"/>
        /// objects that already have a value assigned to
        /// <see cref="IWorkflowSubject.WorkflowName"/>.
        /// </remarks>
        public static WorkflowExecutionResult StartWorkflow(this IWorkflowExecutionService workflowExeService,
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

            return workflowExeService.StartWorkflow(workflowSubject, workflowName);
        }

        /// <summary>
        /// Gets the collection of all transition names that are available
        /// for the given <see cref="IWorkflowSubject"/> from its
        /// <see cref="IWorkflowSubject.CurrentState"/>.
        /// </summary>
        /// <param name="workflowExeService">
        /// Reference to the <see cref="IWorkflowExecutionService"/>
        /// to call
        /// </param>
        /// <param name="workflowSubject">
        /// Workflow subject to get transitions for
        /// </param>
        /// <returns>
        /// Returns a collection of transition names
        /// </returns>
        public static IEnumerable<string> GetAllTransitionNames(this IWorkflowExecutionService workflowExeService,
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

            return workflowExeService.GetAllTransitions(workflowSubject)
                .Select(t => t.Name);
        }

        /// <summary>
        /// Gets the collection of allowed transition names that are available
        /// for the given <see cref="IWorkflowSubject"/> from its
        /// <see cref="IWorkflowSubject.CurrentState"/>.
        /// </summary>
        /// <param name="workflowExeService">
        /// Reference to the <see cref="IWorkflowExecutionService"/>
        /// to call
        /// </param>
        /// <param name="workflowSubject">
        /// Workflow subject to get transitions for
        /// </param>
        /// <returns>
        /// Returns a collection of transition names
        /// </returns>
        public static IEnumerable<string> GetAllowedTransitionNames(this IWorkflowExecutionService workflowExeService,
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

            return workflowExeService.GetAllowedTransitions(workflowSubject)
                .Select(t => t.Name);
        }
    }
}
