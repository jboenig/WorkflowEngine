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
using System.Runtime.Serialization;
using Headway.Dynamo.Metadata;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Base class for items that participate in workflow.
    /// </summary>
    public abstract class WorkflowItem : Dynamo.Runtime.Dynamo, IWorkflowSubject
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WorkflowItem()
        {
        }

        /// <summary>
        /// Constructs a <see cref="WorkflowItem"/> given
        /// an <see cref="ObjectType"/>
        /// </summary>
        /// <param name="objType">
        /// Object metadata for this <see cref="WorkflowItem"/>
        /// </param>
        public WorkflowItem(ObjectType objType) :
            base(objType)
        {
        }

        /// <summary>
        /// Gets the unique identifier for this
        /// <see cref="WorkflowItem"/>.
        /// </summary>
        public abstract string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the fully qualified name of the <see cref="Workflow"/> associated with
        /// this object.
        /// </summary>
        public abstract string WorkflowName
        {
            get;
            set;
        }

        /// <summary>
        /// Contains metadata for the workflow item.
        /// </summary>
        public abstract WorkflowItemType ItemType
        {
            get;
        }

        /// <summary>
        /// Gets the name of workflow current state this object is in.
        /// </summary>
        public abstract string CurrentState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the last transition that occurred
        /// on this <see cref="WorkflowItem"/>.
        /// </summary>
        public string LastTransition
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the context object.
        /// </summary>
        /// <param name="svcProvider">Reference to service provider</param>
        /// <returns>Context object</returns>
        public abstract object GetContextObject(IServiceProvider svcProvider);

        /// <summary>
        /// Called before this object is transitioned to a new state.
        /// </summary>
        /// <param name="transition"></param>
        public abstract void OnTransitioningTo(WorkflowTransition transition);

        /// <summary>
        /// Called after this object is transitioned to a new state.
        /// </summary>
        /// <param name="transition"></param>
        public virtual void OnTransitionedTo(WorkflowTransition transition)
        {
            if (transition != null)
            {
                this.LastTransition = transition.Name;
            }
            else
            {
                this.LastTransition = null;
            }
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
        protected WorkflowItem(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        #endregion
    }
}
