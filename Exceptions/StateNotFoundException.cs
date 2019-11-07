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

using System;

namespace Headway.WorkflowEngine.Exceptions
{
    /// <summary>
    /// Exception thrown when an expected state cannot be found
    /// in a workflow.
    /// </summary>
    public sealed class StateNotFoundException : WorkflowException
    {
        private Workflow workflow;
        private string stateName;

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
}
