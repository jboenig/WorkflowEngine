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
