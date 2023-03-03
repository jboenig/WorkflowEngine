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

namespace Headway.WorkflowEngine.Exceptions;

/// <summary>
/// Exception thrown when an expected state cannot be found
/// in a workflow.
/// </summary>
public sealed class StateNotFoundException : WorkflowException
{
    private readonly Workflow workflow;
    private readonly string stateName;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="workflow"></param>
    /// <param name="stateName"></param>
    public StateNotFoundException(Workflow workflow, string stateName)
    {
        this.workflow = workflow;
        this.stateName = stateName;
    }

    /// <summary>
    /// 
    /// </summary>
    public override string Message
    {
        get
        {
            var msg = string.Format("State named {0} does not existing in the {1} workflow", this.stateName, this.workflow.FullName);
            throw new InvalidOperationException(msg);
        }
    }
}
