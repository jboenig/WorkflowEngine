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

namespace Headway.WorkflowEngine.Factories;

/// <summary>
/// Interface to factory that creates <see cref="WorkflowItem"/>
/// objects.
/// </summary>
public interface IWorkflowItemFactory
{
    /// <summary>
    /// Creates a <see cref="WorkflowItem"/> based on the specified
    /// template.
    /// </summary>
    /// <param name="templateName">
    /// Name of template to use to generate new <see cref="WorkflowItem"/>
    /// </param>
    /// <param name="context">
    /// Reference to context object.
    /// </param>
    /// <returns>
    /// New instance of a <see cref="WorkflowItem"/> based on specified
    /// template and context data.
    /// </returns>
    WorkflowItem CreateWorkflowItem(string templateName, object context);
}
