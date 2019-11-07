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
    /// Exception thrown when an expected transition cannot be
    /// found in a state.
    /// </summary>
    public sealed class TransitionNotFoundException : WorkflowException
    {
        private WorkflowState state;
        private string transitionName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="transitionName"></param>
        public TransitionNotFoundException(WorkflowState state, string transitionName)
        {
            this.state = state;
            this.transitionName = transitionName;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                var msg = string.Format("Transition named {0} does not existing in the {1} state", this.transitionName, this.state.Name);
                throw new InvalidOperationException(msg);
            }
        }
    }
}
