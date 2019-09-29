using System;
using System.Collections.Generic;
using System.Text;

using Headway.Dynamo.Metadata;
using Headway.WorkflowEngine;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal class WorkflowItemImpl : Headway.WorkflowEngine.WorkflowItem
    {
        private string id;
        private string workflowName;
        private WorkflowItemType workflowItemType;

        public WorkflowItemImpl(WorkflowItemType workflowItemType,
            string id,
            string workflowName)
        {
            this.workflowItemType = workflowItemType;
            this.id = id;
            this.workflowName = workflowName;
        }

        /// <summary>
        /// Gets the unique identifier for this
        /// <see cref="WorkflowItem"/>.
        /// </summary>
        public override string Id
        {
            get { return this.id; }
            set { throw new NotImplementedException(); }
        }

        public override string WorkflowName
        {
            get { return this.workflowName; }
            set { throw new NotImplementedException(); }
        }

        public override WorkflowItemType ItemType
        {
            get
            {
                return this.workflowItemType;
            }
        }

        public override string CurrentState
        {
            get;
            set;
        }

        public string Info
        {
            get;
            set;
        }

        public override object GetContextObject(IServiceProvider svcProvider)
        {
            return this;
        }

        public override void OnTransitionedTo(WorkflowTransition transition)
        {
        }

        public override void OnTransitioningTo(WorkflowTransition transition)
        {
        }
    }
}
