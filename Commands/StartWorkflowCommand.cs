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

using Headway.Dynamo.Commands;
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Services;
using Headway.Dynamo.Runtime;

namespace Headway.WorkflowEngine.Commands;

/// <summary>
/// Starts or executes a workflow.
/// </summary>
/// <remarks>
/// <para>
/// This command executes a workflow. Executing a workflow requires a reference
/// to the <see cref="IWorkflowExecutionService"/>, an <see cref="IWorkflowSubject"/>
/// object, and the name of the workflow.
/// </para>
/// <para>
/// The workflow subject can either be a property of the context object passed
/// to the <see cref="StartWorkflowCommand.Execute(IServiceProvider, object)"/>
/// method or it can be the context object itself. If no property name
/// is specified in the <see cref="StartWorkflowCommand.WorkflowSubjectPropertyName"/>
/// property, then the context object is cast to <see cref="IWorkflowSubject"/>.
/// </para>
/// </remarks>
public sealed class StartWorkflowCommand : Command
{
    /// <summary>
    /// Gets or sets the name of the workflow to execute.
    /// </summary>
    public string WorkflowName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the name of the property on the context object that
    /// stores the <see cref="IWorkflowSubject"/> object that the workflow
    /// execution applies to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this property is specified, then it must resolve to the name of
    /// a property on the context object passed to the
    /// <see cref="StartWorkflowCommand.Execute(IServiceProvider, object)"/>
    /// method. In addition, the value of the property must be of type
    /// <see cref="IWorkflowSubject"/>.
    /// </para>
    /// <para>
    /// If this property is null or whitespace, then the context object
    /// passed to the <see cref="StartWorkflowCommand.Execute(IServiceProvider, object)"/>
    /// method will be cast to <see cref="IWorkflowSubject"/> in order to
    /// get the workflow subject.
    /// </para>
    /// </remarks>
    public string WorkflowSubjectPropertyName
    {
        get;
        set;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="serviceProvider">
    /// Reference to service provider
    /// </param>
    /// <param name="context">
    /// Context object
    /// </param>
    /// <returns>Returns a <see cref="CommandResult"/></returns>
    /// <exception cref="ServiceNotFoundException">
    /// Throws this exception of the <see cref="IWorkflowExecutionService"/>
    /// is not found
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Throws this exception of the context object is null
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// Throws this exception if it cannot resolve the workflow subject
    /// </exception>
    public override async Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
    {
        var workflowExecutionService = serviceProvider.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
        if (workflowExecutionService == null)
            throw new ServiceNotFoundException(typeof(IWorkflowExecutionService));

        if (string.IsNullOrWhiteSpace(WorkflowName))
            throw new InvalidDataException($"Workflow name must be specified for {this.GetType().Name} commands");

        if (context == null)
            throw new ArgumentNullException(nameof(context), "Context object must be provided in order to get workflow subject");

        IWorkflowSubject workflowSubject = null;

        if (!string.IsNullOrWhiteSpace(this.WorkflowSubjectPropertyName))
        {
            // Get workflow subject from context as a property
            workflowSubject = PropertyResolver.GetPropertyValue<IWorkflowSubject>(context, this.WorkflowSubjectPropertyName);
        }
        else
        {
            workflowSubject = context as IWorkflowSubject;
        }

        if (workflowSubject == null)
            throw new InvalidDataException($"No workflow subject provided for {this.GetType().Name} command");

        var workflowRes = await workflowExecutionService.StartWorkflow(workflowSubject, this.WorkflowName);
        return new WorkflowTransitionCommandResult(workflowRes);
    }
}

