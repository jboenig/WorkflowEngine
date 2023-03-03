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

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Exceptions;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine;

/// <summary>
/// Template used to create and initialize new instances of
/// <see cref="WorkflowItem"/> objects.
/// </summary>
/// <remarks>
/// This class is a factory for <see cref="WorkflowItem"/> objects.
/// </remarks>
public sealed class WorkflowItemTemplate : IObjectFactory<WorkflowItem>
{
    private string objectTypeFullName;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public WorkflowItemTemplate()
    {
    }

    /// <summary>
    /// Gets or sets the full name of the object type.
    /// </summary>
    [JsonProperty("objectTypeFullName")]
    private string ObjectTypeFullName
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
        get;
        set;
    }

    /// <summary>
    /// Name of the workflow to associate with new instances
    /// of <see cref="WorkflowItem"/> created by this
    /// template.
    /// </summary>
    /// <remarks>
    /// This property is optional.  Binding a <see cref="WorkflowItem"/>
    /// to a workflow can be done later when the workflow
    /// is started.
    /// </remarks>
    [JsonProperty("workflowName")]
    public string WorkflowName
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
            // Initialize new instance
            workflowItem.WorkflowName = this.WorkflowName;

            var objInit = workflowItem as IObjectInit;
            if (objInit != null)
            {
                objInit.Init(svcProvider);
            }
        }

        return workflowItem;
    }

    #region Serialization

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        var svcProvider = context.Context as IServiceProvider;
        if (svcProvider != null)
        {
            var metadataProvider = svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            if (metadataProvider == null)
            {
                throw new ServiceNotFoundException(typeof(IMetadataProvider));
            }
            this.ObjectType = metadataProvider.GetDataType<ObjectType>(this.ObjectTypeFullName);
        }
    }

    #endregion
}
