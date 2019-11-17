﻿////////////////////////////////////////////////////////////////////////////////
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
    /// Exception thrown when an action fails during workflow execution.
    /// </summary>
    public sealed class ActionFailedException : WorkflowException
    {
        /// <summary>
        /// Constructs an <see cref="ActionFailedException"/>
        /// given a message.
        /// </summary>
        /// <param name="message">
        /// Message to associate with this exception.
        /// </param>
        public ActionFailedException(string message)
        {
        }

        /// <summary>
        /// Constructs an <see cref="ActionFailedException"/>
        /// given a message and inner exception.
        /// </summary>
        /// <param name="message">
        /// Message to associate with this exception.
        /// </param>
        /// <param name="innerException">
        /// Exception that caused the action to fail.
        /// </param>
        public ActionFailedException(string message, Exception innerException)
        {
        }
    }
}
