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
using Newtonsoft.Json;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Metadata;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Base class for items that participate in workflow.
    /// </summary>
    [JsonObject]
    public class WorkflowItem : DynamoObject, IWorkflowSubject, IPrimaryKeyAccessor
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
        public object PrimaryKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the fully qualified name of the <see cref="Workflow"/> associated with
        /// this object.
        /// </summary>
        public string WorkflowName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of workflow current state this object is in.
        /// </summary>
        public string CurrentState
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
        public virtual object GetContextObject(IServiceProvider svcProvider)
        {
            return this;
        }

        /// <summary>
        /// Called after this item is started in a workflow.
        /// </summary>
        /// <param name="workflow">
        /// Workflow this item has started
        /// </param>
        public virtual void OnStarted(Workflow workflow)
        {
            this.LastTransition = null;
        }

        /// <summary>
        /// Called before this object is transitioned to a new state.
        /// </summary>
        /// <param name="workflow">
        /// Workflow that contains the transition
        /// </param>
        /// <param name="transition">
        /// Transition executed
        /// </param>
        public virtual void OnTransitioningTo(Workflow workflow, WorkflowTransition transition)
        {
        }

        /// <summary>
        /// Called after this object is transitioned to a new state.
        /// </summary>
        /// <param name="workflow">
        /// Workflow that contains the transition
        /// </param>
        /// <param name="transition">
        /// Transition executed
        /// </param>
        public virtual void OnTransitionedTo(Workflow workflow, WorkflowTransition transition)
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
    }
}
