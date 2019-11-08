using System;
using System.Runtime.Serialization;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    [Serializable]
    internal class WorkflowItemImpl : WorkflowItem
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
            set { this.id = value; }
        }

        public override string WorkflowName
        {
            get { return this.workflowName; }
            set { this.workflowName = value; }
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

        #region Serialization

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialiation info</param>
        /// <param name="context">Streaming context</param>
        /// <remarks>
        /// Deserializes the given SerializationInfo into a new
        /// instance of this class.
        /// </remarks>
        protected WorkflowItemImpl(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion
    }
}
