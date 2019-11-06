using System;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Template used to create and initialize new instances of
    /// <see cref="IWorkflowSubject"/> objects.
    /// </summary>
    public class WorkflowItemTemplate : IObjectFactory<WorkflowItem>
    {
        /// <summary>
        /// Full name reference to the metadata for the
        /// <see cref="WorkflowItem"/> objects created by
        /// this template.
        /// </summary>
        public string ItemTypeFullName
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the workflow to associate with new instances
        /// of <see cref="IWorkflowSubject"/> created by this
        /// template.
        /// </summary>
        public string WorkflowName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the initial workflow state for items
        /// created by this template.
        /// </summary>
        public string InitialState
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of an <see cref="IWorkflowSubject"/>
        /// object for the given context.
        /// </summary>
        /// <param name="svcProvider">Reference to service provider</param>
        /// <param name="context">
        /// Context object data to associated with the new workflow subject
        /// </param>
        /// <returns>
        /// An instance of <see cref="IWorkflowSubject"/>.
        /// </returns>
        public WorkflowItem CreateInstance(IServiceProvider svcProvider,
            object context)
        {
            if (svcProvider == null)
            {
                throw new ArgumentNullException(nameof(IServiceProvider));
            }

            var itemTypeResolver = svcProvider.GetService(typeof(IWorkflowItemTypeResolver)) as IWorkflowItemTypeResolver;
            if (itemTypeResolver == null)
            {
                throw new ServiceNotFoundException(typeof(IWorkflowItemTypeResolver));
            }

            var workflowResolver = svcProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            if (workflowResolver == null)
            {
                throw new ServiceNotFoundException(typeof(IWorkflowByNameResolver));
            }

            // Resolve the workflow subject metadata type information
            var itemType = itemTypeResolver.Resolve(this.ItemTypeFullName);
            if (itemType == null)
            {
                // TODO: throw better exception
                throw new InvalidOperationException();
            }

            // Resolve the workflow
            var workflow = workflowResolver.Resolve(this.WorkflowName);
            if (workflow == null)
            {
                // TODO: throw better exception
                throw new InvalidOperationException();
            }

            var workflowItemObjectType = itemType.ObjectType;
            if (workflowItemObjectType == null)
            {
                // TODO: throw better exception
                throw new InvalidOperationException();
            }

            string itemId = Guid.NewGuid().ToString();

            // Instantiate new workflow item and pass workflow as constructor parameter
            WorkflowItem workflowItem = workflowItemObjectType.CreateInstance<WorkflowItem>(svcProvider, itemType, itemId, workflow.FullName);

            // Initialize new instance
            this.InitWorkflowItem(workflowItem);

            return workflowItem;
        }

        /// <summary>
        /// Initializes newly created <see cref="WorkflowItem"/> objects
        /// created by this template.
        /// </summary>
        /// <param name="workflowItem">
        /// <see cref="WorkflowItem"/> object to initialize.
        /// </param>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// <description>
        /// The following properties are already initialized when this method
        /// is invoked
        /// </description>
        /// </listheader>
        /// <item>
        /// <description><see cref="WorkflowItem.Workflow"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="WorkflowItem.ItemType"/></description>
        /// </item>
        /// </list>
        /// </remarks>
        protected virtual void InitWorkflowItem(WorkflowItem workflowItem)
        {
            workflowItem.CurrentState = this.InitialState;
        }
    }
}
