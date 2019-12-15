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

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Defines different modes for viewing workflow subjects.
    /// </summary>
    public enum WorkflowViewModes
    {
        /// <summary>
        /// Read-only summary view mode
        /// </summary>
        SummaryView,

        /// <summary>
        /// Read-only detail view mode
        /// </summary>
        DetailView,

        /// <summary>
        /// Editable summary view mode
        /// </summary>
        SummaryEdit,

        /// <summary>
        /// Editable detail view mode
        /// </summary>
        DetailEdit,

        /// <summary>
        /// List item view mode
        /// </summary>
        ListItem
    }
}
