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

using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Factories;

namespace Headway.WorkflowEngine.Implementations;

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
