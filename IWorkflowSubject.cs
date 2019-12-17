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

namespace Headway.WorkflowEngine
{
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
        void OnStarted(Workflow workflow);

        /// <summary>
        /// Called before this object is transitioned to a new state.
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="workflow">
        /// Workflow that contains the transition
        /// </param>
        void OnTransitioningTo(Workflow workflow, WorkflowTransition transition);

        /// <summary>
        /// Called after this object is transitioned to a new state.
        /// </summary>
        /// <param name="workflow">
        /// Workflow that contains the transition
        /// </param>
        /// <param name="transition"></param>
        void OnTransitionedTo(Workflow workflow, WorkflowTransition transition);
    }
}
