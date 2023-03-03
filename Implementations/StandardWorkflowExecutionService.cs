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
using Headway.WorkflowEngine.Services;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Exceptions;

namespace Headway.WorkflowEngine.Implementations;

/// <summary>
/// Base implementation of the <see cref="IWorkflowExecutionService"/>
/// service.
/// </summary>
public sealed class StandardWorkflowExecutionService : IWorkflowExecutionService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IWorkflowByNameResolver workflowByNameResolver;

    /// <summary>
    /// Constructs a <see cref="StandardWorkflowExecutionService"/>
    /// given a service provider
    /// </summary>
    /// <param name="svcProvider">
    /// Service provider
    /// </param>
    public StandardWorkflowExecutionService(IServiceProvider svcProvider)
    {
        this.serviceProvider = svcProvider ?? throw new ArgumentNullException(nameof(svcProvider));

        this.workflowByNameResolver = this.serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
        if (this.workflowByNameResolver == null)
        {
            throw new ServiceNotFoundException(typeof(IWorkflowByNameResolver));
        }
    }

    /// <summary>
    /// Starts execution of an <see cref="IWorkflowSubject"/> object
    /// in the specified workflow.
    /// </summary>
    /// <param name="workflowSubject">
    /// <see cref="IWorkflowSubject"/> object to start in the
    /// workflow
    /// </param>
    /// <param name="workflowName">
    /// Fully-qualified name of the workflow to execute
    /// </param>
    /// <returns>
    /// Returns a <see cref="WorkflowTransitionResult"/> object
    /// that encapsulates the result of the operation.
    /// </returns>
    public async Task<WorkflowTransitionResult> StartWorkflow(IWorkflowSubject workflowSubject, string workflowName)
    {
        if (workflowSubject == null)
        {
            throw new ArgumentNullException(nameof(workflowSubject));
        }

        if (string.IsNullOrEmpty(workflowName))
        {
            throw new ArgumentNullException(nameof(workflowName));
        }

        var workflow = this.workflowByNameResolver.Resolve(workflowName);
        if (workflow == null)
        {
            throw new WorkflowNotFoundException(workflowName);
        }

        return await workflow.Start(workflowSubject, this.serviceProvider);
    }

    /// <summary>
    /// Transitions the specified workflow subject to a
    /// new state along a given transition.
    /// </summary>
    /// <param name="workflowSubject">
    /// <see cref="IWorkflowSubject"/> object to transition
    /// </param>
    /// <param name="transitionName">
    /// Name of transition to execute
    /// </param>
    /// <returns>
    /// Returns a <see cref="WorkflowTransitionResult"/> object
    /// that encapsulates the result of the operation.
    /// </returns>
    public async Task<WorkflowTransitionResult> TransitionTo(IWorkflowSubject workflowSubject, string transitionName)
    {
        if (workflowSubject == null)
        {
            throw new ArgumentNullException(nameof(workflowSubject));
        }

        var workflow = this.workflowByNameResolver.Resolve(workflowSubject.WorkflowName);
        if (workflow == null)
        {
            throw new WorkflowNotFoundException(workflowSubject.WorkflowName);
        }

        return await workflow.TransitionTo(workflowSubject, transitionName, this.serviceProvider);
    }

    /// <summary>
    /// Gets the collection of all transitions that are available
    /// for the given <see cref="IWorkflowSubject"/> from its
    /// <see cref="IWorkflowSubject.CurrentState"/>.
    /// </summary>
    /// <param name="workflowSubject">
    /// Workflow subject to get transitions for
    /// </param>
    /// <returns>
    /// Collection of <see cref="WorkflowTransition"/> objects
    /// that can be taken from the current state of the given
    /// <see cref="IWorkflowSubject"/> object.
    /// </returns>
    public IEnumerable<WorkflowTransition> GetAllTransitions(IWorkflowSubject workflowSubject)
    {
        if (workflowSubject == null)
        {
            throw new ArgumentNullException(nameof(workflowSubject));
        }

        var workflow = this.workflowByNameResolver.Resolve(workflowSubject.WorkflowName);
        if (workflow == null)
        {
            throw new WorkflowNotFoundException(workflowSubject.WorkflowName);
        }

        return workflow.GetAllTransitions(workflowSubject, this.serviceProvider);
    }

    /// <summary>
    /// Gets the collection of allowed transitions that are available
    /// for the given <see cref="IWorkflowSubject"/> from its
    /// <see cref="IWorkflowSubject.CurrentState"/>.
    /// </summary>
    /// <param name="workflowSubject">
    /// Workflow subject to get transitions for
    /// </param>
    /// <returns>
    /// Collection of <see cref="WorkflowTransition"/> objects
    /// that can be taken from the current state of the given
    /// <see cref="IWorkflowSubject"/> object.
    /// </returns>
    public IEnumerable<WorkflowTransition> GetAllowedTransitions(IWorkflowSubject workflowSubject)
    {
        if (workflowSubject == null)
        {
            throw new ArgumentNullException(nameof(workflowSubject));
        }

        var workflow = this.workflowByNameResolver.Resolve(workflowSubject.WorkflowName);
        if (workflow == null)
        {
            throw new WorkflowNotFoundException(workflowSubject.WorkflowName);
        }

        return workflow.GetAllowedTransitions(workflowSubject, this.serviceProvider);
    }

    /// <summary>
    /// Gets the current <see cref="WorkflowExecutionContext"/> for the specified
    /// <see cref="IWorkflowSubject"/>.
    /// </summary>
    /// <param name="workflowSubject">
    /// Workflow subject to get execution frame for
    /// </param>
    /// <returns>
    /// Returns a <see cref="WorkflowExecutionContext"/> that describes the state of execution
    /// for the given <see cref="IWorkflowSubject"/>.
    /// </returns>
    public WorkflowExecutionContext GetCurrentExecutionFrame(IWorkflowSubject workflowSubject)
    {
        if (workflowSubject == null)
        {
            throw new ArgumentNullException(nameof(workflowSubject));
        }

        var workflow = this.workflowByNameResolver.Resolve(workflowSubject.WorkflowName);
        if (workflow == null)
        {
            throw new WorkflowNotFoundException(workflowSubject.WorkflowName);
        }

        return workflow.GetExecutionFrame(workflowSubject, this.serviceProvider);
    }
}
