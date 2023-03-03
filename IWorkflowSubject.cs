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

namespace Headway.WorkflowEngine;

/// <summary>
/// Interface to objects that can move through
/// a <see cref="Workflow"/>.
/// </summary>
/// <remarks>
/// This interface provides the minimal set of properties and
/// methods needed to apply workflow transitions.
/// </remarks>
public interface IWorkflowSubject
{
    /// <summary>
    /// Gets or sets the fully qualified name of the
    /// <see cref="Workflow"/> associated with this object.
    /// </summary>
    string WorkflowName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the name of workflow current state this object is in.
    /// </summary>
    string CurrentState
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the context object.
    /// </summary>
    /// <param name="svcProvider">Reference to service provider</param>
    /// <returns>Context object</returns>
    object GetContextObject(IServiceProvider svcProvider);

    /// <summary>
    /// Called after this item is started in a workflow.
    /// </summary>
    /// <param name="workflow">
    /// Workflow this subject has started
    /// </param>
    Task OnStarted(Workflow workflow);

    /// <summary>
    /// Called before this object is transitioned to a new state.
    /// </summary>
    /// <param name="transition"></param>
    /// <param name="workflow">
    /// Workflow that contains the transition
    /// </param>
    Task OnTransitioningTo(Workflow workflow, WorkflowTransition transition);

    /// <summary>
    /// Called after this object is transitioned to a new state.
    /// </summary>
    /// <param name="workflow">
    /// Workflow that contains the transition
    /// </param>
    /// <param name="transition"></param>
    Task OnTransitionedTo(Workflow workflow, WorkflowTransition transition);
}
