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

using Newtonsoft.Json;

namespace Headway.WorkflowEngine;

/// <summary>
/// This class encpasulates a snapshot of the current state of workflow execution
/// for a <see cref="IWorkflowSubject"/>.
/// </summary>
/// <remarks>
/// This class provides information about available transitions that are in
/// the context of the given <see cref="IWorkflowSubject"/>. Specifically, it
/// flags each transition as allowed or not allowed and includes information
/// explaining why transitions are not allow.
/// </remarks>
public sealed class WorkflowExecutionContext
{
    internal static WorkflowExecutionContext Create(IWorkflowSubject subject,
        IEnumerable<WorkflowTransitionDescriptor> nextTransitions)
    {
        var workflowExecutionFrame = new WorkflowExecutionContext()
        {
            Subject = subject,
            NextTransitions = nextTransitions
        };
        return workflowExecutionFrame;
    }

    private WorkflowExecutionContext()
    {
    }

    /// <summary>
    /// Gets the <see cref="IWorkflowSubject"/> described by this
    /// execution frame.
    /// </summary>
    [JsonIgnore]
    public IWorkflowSubject Subject
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the collection of transitions from the current state in the
    /// context of the <see cref="IWorkflowSubject"/>.
    /// </summary>
    [JsonProperty("nextTransitions")]
    public IEnumerable<WorkflowTransitionDescriptor> NextTransitions
    {
        get;
        private set;
    }
}
