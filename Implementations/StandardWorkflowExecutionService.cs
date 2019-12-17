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
using Headway.WorkflowEngine.Services;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Exceptions;

namespace Headway.WorkflowEngine.Implementations
{
    /// <summary>
    /// Base implementation of the <see cref="IWorkflowExecutionService"/>
    /// service.
    /// </summary>
    public sealed class StandardWorkflowExecutionService : IWorkflowExecutionService
    {
        private IServiceProvider serviceProvider;
        private IWorkflowByNameResolver workflowByNameResolver;

        /// <summary>
        /// Constructs a <see cref="StandardWorkflowExecutionService"/>
        /// given a service provider
        /// </summary>
        /// <param name="svcProvider">
        /// Service provider
        /// </param>
        public StandardWorkflowExecutionService(IServiceProvider svcProvider)
        {
            if (svcProvider == null)
            {
                throw new ArgumentNullException(nameof(svcProvider));
            }
            this.serviceProvider = svcProvider;

            this.workflowByNameResolver = this.serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            if (this.workflowByNameResolver == null)
            {
                throw new ServiceNotFoundException(typeof(IWorkflowByNameResolver));
            }
        }

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
        public WorkflowExecutionResult StartWorkflow(IWorkflowSubject workflowSubject, string workflowName)
        {
            if (workflowSubject == null)
            {
                throw new ArgumentNullException(nameof(workflowSubject));
            }

            if (string.IsNullOrEmpty(workflowName))
            {
                throw new ArgumentNullException(nameof(workflowName));
            }

            var workflow = this.workflowByNameResolver.Resolve(workflowName);
            if (workflow == null)
            {
                throw new WorkflowNotFoundException(workflowName);
            }

            return workflow.Start(workflowSubject, this.serviceProvider);
        }

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
        public WorkflowExecutionResult TransitionTo(IWorkflowSubject workflowSubject, string transitionName)
        {
            if (workflowSubject == null)
            {
                throw new ArgumentNullException(nameof(workflowSubject));
            }

            var workflow = this.workflowByNameResolver.Resolve(workflowSubject.WorkflowName);
            if (workflow == null)
            {
                throw new WorkflowNotFoundException(workflowSubject.WorkflowName);
            }

            return workflow.TransitionTo(workflowSubject, transitionName, this.serviceProvider);
        }
    }
}
