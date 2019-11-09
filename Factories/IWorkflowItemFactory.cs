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

namespace Headway.WorkflowEngine.Factories
{
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
}
