using System;
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Factories;

namespace Headway.WorkflowEngine.Implementations
{
    /// <summary>
    /// Implements <see cref="IWorkflowItemFactory"/> based on
    /// templates supplied by the injected <see cref="IWorkflowItemTemplateResolver"/>
    /// service.
    /// </summary>
    public sealed class StandardWorkflowItemFactory : IWorkflowItemFactory
    {
        private IServiceProvider serviceProvider;
        private IWorkflowItemTemplateResolver workflowItemTemplateResolver;

        /// <summary>
        /// Constructs a <see cref="StandardWorkflowItemFactory"/> given
        /// a reference to an IServiceProvider.
        /// </summary>
        /// <param name="svcProvider">
        /// Service provider to supply services during item creation.
        /// </param>
        public StandardWorkflowItemFactory(IServiceProvider svcProvider)
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
        }

        /// <summary>
        /// Creates a new <see cref="WorkflowItem"/> based on the name
        /// of a <see cref="WorkflowItemTemplate"/>.
        /// </summary>
        /// <param name="templateName">
        /// Name of template to use to create the item.
        /// </param>
        /// <param name="context">
        /// Context object to associate with the new item
        /// </param>
        /// <returns>
        /// Returns a new <see cref="WorkflowItem"/>
        /// </returns>
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
    }
}
