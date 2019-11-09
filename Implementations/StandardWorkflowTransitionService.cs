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

namespace Headway.WorkflowEngine.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StandardWorkflowTransitionService : IWorkflowTransitionService
    {
        private IServiceProvider serviceProvider;
        private IWorkflowByNameResolver workflowByNameResolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svcProvider"></param>
        public StandardWorkflowTransitionService(IServiceProvider svcProvider)
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
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="transitionName"></param>
        /// <returns></returns>
        public WorkflowTransitionResult TransitionTo(WorkflowItem item, string transitionName)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var workflow = this.workflowByNameResolver.Resolve(item.WorkflowName);
            if (workflow == null)
            {
                var msg = string.Format("Workflow {0} not found", item.WorkflowName);
            }

            return workflow.TransitionTo(item, transitionName, this.serviceProvider);
        }
    }
}
