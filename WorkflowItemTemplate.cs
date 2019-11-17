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
using Newtonsoft.Json;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Exceptions;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Template used to create and initialize new instances of
    /// <see cref="WorkflowItem"/> objects.
    /// </summary>
    public class WorkflowItemTemplate : IObjectFactory<WorkflowItem>
    {
        private string objectTypeFullName;
        private ObjectType objectType;
        private IMetadataProvider metadataProvider;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WorkflowItemTemplate()
        {
        }

        /// <summary>
        /// Gets or sets the full name of the object type.
        /// </summary>
        public string ObjectTypeFullName
        {
            get
            {
                if (this.ObjectType != null)
                {
                    return this.ObjectType.FullName;
                }
                return this.objectTypeFullName;
            }
            set
            {
                this.objectTypeFullName = value;
                this.ObjectType = null;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Dynamo.Metadata.ObjectType"/> metadata
        /// for this <see cref="WorkflowItemTemplate"/>.
        /// </summary>
        [JsonIgnore]
        public ObjectType ObjectType
        {
            get
            {
                if (this.objectType == null)
                {
                    if (this.objectTypeFullName != null)
                    {
                        // Resolve object type
                        this.objectType = this.metadataProvider.GetDataType<ObjectType>(this.objectTypeFullName);
                    }
                }
                return this.objectType;
            }
            set
            {
                this.objectType = value;
                if (this.objectType != null)
                {
                    this.objectTypeFullName = this.objectType.FullName;
                }
            }
        }

        /// <summary>
        /// Name of the workflow to associate with new instances
        /// of <see cref="WorkflowItem"/> created by this
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
        /// Creates a new instance of an <see cref="WorkflowItem"/>
        /// object for the given context.
        /// </summary>
        /// <param name="svcProvider">Reference to service provider</param>
        /// <param name="context">
        /// Context object data to associated with the new workflow subject
        /// </param>
        /// <returns>
        /// An instance of <see cref="WorkflowItem"/>.
        /// </returns>
        public WorkflowItem CreateInstance(IServiceProvider svcProvider,
            object context)
        {
            if (svcProvider == null)
            {
                throw new ArgumentNullException(nameof(IServiceProvider));
            }

            var workflowResolver = svcProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            if (workflowResolver == null)
            {
                throw new ServiceNotFoundException(typeof(IWorkflowByNameResolver));
            }

            // Resolve the workflow
            var workflow = workflowResolver.Resolve(this.WorkflowName);
            if (workflow == null)
            {
                throw new WorkflowNotFoundException(this.WorkflowName);
            }

            // Resolve the ObjectType metadata
            var workflowItemObjectType = this.ObjectType;
            if (workflowItemObjectType == null)
            {
                throw new DataTypeNotFound(this.ObjectTypeFullName);
            }

            // Instantiate new workflow item
            var workflowItem = workflowItemObjectType.CreateInstance<WorkflowItem>(svcProvider);
            if (workflowItem != null)
            {
                workflowItem.WorkflowName = this.WorkflowName;
                workflowItem.CurrentState = this.InitialState;

                // Initialize new instance
                this.InitWorkflowItem(workflowItem);
            }

            return workflowItem;
        }

        /// <summary>
        /// Initializes newly created <see cref="WorkflowItem"/> objects
        /// created by this template.
        /// </summary>
        /// <param name="workflowItem">
        /// <see cref="WorkflowItem"/> object to initialize.
        /// </param>
        /// <description>
        /// The following properties are already initialized when this method
        /// is invoked
        /// </description>
        protected virtual void InitWorkflowItem(WorkflowItem workflowItem)
        {
        }

        #region Serialization

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            var svcProvider = context.Context as IServiceProvider;
            if (svcProvider != null)
            {
                this.metadataProvider = svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            }
        }

        #endregion
    }
}
