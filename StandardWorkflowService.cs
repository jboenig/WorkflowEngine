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
    public sealed class StandardWorkflowService : IWorkflowService
    {
        private IServiceProvider serviceProvider;
        private IWorkflowItemTemplateResolver workflowItemTemplateResolver;
        private IWorkflowByNameResolver workflowByNameResolver;

        public StandardWorkflowService(IServiceProvider svcProvider)
        {
            if (svcProvider == null)
            {
                throw new ArgumentNullException(nameof(svcProvider));
            }
            this.serviceProvider = svcProvider;

            this.workflowItemTemplateResolver = this.serviceProvider.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            if (this.workflowItemTemplateResolver == null)
            {
                throw new ServiceNotFoundException(typeof(IWorkflowItemTemplateResolver));
            }

            this.workflowByNameResolver = this.serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            if (this.workflowByNameResolver == null)
            {
                throw new ServiceNotFoundException(typeof(IWorkflowByNameResolver));
            }
        }

        public WorkflowItem CreateWorkflowItem(string templateName, object context)
        {
            var workflowItemTemplate = this.workflowItemTemplateResolver.Resolve(templateName);
            if (workflowItemTemplate == null)
            {
                var msg = string.Format("Workflow item template {0} not found", templateName);
                throw new InvalidOperationException(msg);
            }

             return workflowItemTemplate.CreateInstance(this.serviceProvider, context);
        }

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
