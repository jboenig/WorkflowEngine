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
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

using Headway.Dynamo.Runtime;
using Headway.Dynamo.Commands;
using Headway.WorkflowEngine.Exceptions;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Encapsulates a workflow is a set of states and transitions
    /// that can be applied to a <see cref="IWorkflowSubject"/>.
    /// </summary>
    public sealed class Workflow : INamedObject, IPrimaryKeyAccessor
    {
        #region Member Variables

        private string fullName;
        private string displayName;
        private WorkflowState initialState;
        private readonly HashSet<WorkflowState> states;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        private Workflow()
        {
            this.states = new HashSet<WorkflowState>(new WorkflowState.EqualityComparer());
        }

        /// <summary>
        /// Constructs a <see cref="Workflow"/> given a
        /// fully qualified name.
        /// </summary>
        /// <param name="fullName">
        /// Fully qualified name of the workflow.
        /// </param>
        public Workflow(string fullName)
        {
            this.fullName = fullName;
            this.states = new HashSet<WorkflowState>(new WorkflowState.EqualityComparer());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the workflow.
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get { return NameHelpers.GetName(this.fullName); }
        }

        /// <summary>
        /// Gets the display name of the workflow.
        /// </summary>        
        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        /// <summary>
        /// Gets the namespace of the workflow.
        /// </summary>
        [JsonIgnore]
        public string Namespace
        {
            get { return NameHelpers.GetNamespace(this.fullName); }
        }

        /// <summary>
        /// Gets the fullname of the workflow.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
            set { this.fullName = value; }
        }

        /// <summary>
        /// Gets the primary key for this object.
        /// </summary>
        public object PrimaryKey
        {
            get { return this.FullName; }
        }

        /// <summary>
        /// Gets the initial state in this workflow.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value defines the state that workflow subjects
        /// should start in.
        /// </para>
        /// <para>
        /// The value assigned to this property is automatically
        /// added to the <see cref="Workflow.States"/> collection.
        /// </para>
        /// </remarks>
        public WorkflowState InitialState
        {
            get
            {
                return this.initialState;
            }
            set
            {
                if (this.initialState != null)
                {
                    this.states.Remove(this.initialState);
                }

                this.initialState = value;

                if (this.initialState != null)
                {
                    this.states.Add(this.initialState);
                }
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="WorkflowState"/> objects
        /// contained by this <see cref="Workflow"/>.
        /// </summary>
        public ICollection<WorkflowState> States
        {
            get { return this.states; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Searches the <see cref="Workflow.States"/> collection
        /// for a <see cref="WorkflowState"/> matching the specified
        /// name.
        /// </summary>
        /// <param name="stateName">Name of state to find.</param>
        /// <returns>
        /// Returns the <see cref="WorkflowState"/> matching the
        /// specified name or null if no matching state is found.
        /// </returns>
        public WorkflowState FindStateByName(string stateName)
        {
            return (from s in this.States
                    where s.Name == stateName
                    select s).FirstOrDefault();
        }

        /// <summary>
        /// Sets an <see cref="IWorkflowSubject"/> object to the initial
        /// state defined by this workflow.
        /// </summary>
        /// <param name="workflowSubject">
        /// Workflow subject to transition to initialize.
        /// </param>
        /// <param name="serviceProvider">
        /// Interface to service provider.
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowTransitionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        /// <remarks>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when workflowSubject is null.
        /// </exception>
        /// <exception cref="InitialStateNotFoundException">
        /// Thrown when the workflow has no initial state defined.
        /// </exception>
        public WorkflowTransitionResult LoadInitialState(IWorkflowSubject workflowSubject, IServiceProvider serviceProvider)
        {
            ///////////////////////////////////////////////////////////////////
            // Check arguments
            if (workflowSubject == null)
            {
                throw new ArgumentNullException("workflowSubject");
            }

            var initialState = this.InitialState;
            if (initialState == null)
            {
                throw new InitialStateNotFoundException();
            }

            ///////////////////////////////////////////////////////////////////
            // Execute EnterAction associated with the INITIAL state
            initialState.ExecuteEnterAction(serviceProvider, workflowSubject.GetContextObject(serviceProvider));

            ///////////////////////////////////////////////////////////////////
            // Set the current state of the workflow subject
            // to the INITIAL state
            workflowSubject.CurrentState = initialState.Name;

            return WorkflowTransitionResult.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowSubject"></param>
        /// <param name="transitionName"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public WorkflowTransitionResult IsTransitionToAllowed(IWorkflowSubject workflowSubject, string transitionName, IServiceProvider serviceProvider)
        {
            ///////////////////////////////////////////////////////////////////
            // Get the FROM state
            var currentStateName = workflowSubject.CurrentState;
            if (string.IsNullOrEmpty(currentStateName))
            {
                throw new StateNotFoundException(this, currentStateName);
            }

            var fromState = this.FindStateByName(currentStateName);
            if (fromState == null)
            {
                throw new StateNotFoundException(this, currentStateName);
            }

            var transition = fromState.GetTransition(transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException(fromState, transitionName);
            }
            ///////////////////////////////////////////////////////////////////
            // Check to see if the transition is allowed
            if (!transition.IsAllowed(serviceProvider, workflowSubject.GetContextObject(serviceProvider)))
            {
                return new WorkflowTransitionResult(WorkflowTransitionResultCode.NotAllowed, string.Format("Transition {0} not allowed {1}", transitionName, transition.ConditionErrorMessage));
            }

            return new WorkflowTransitionResult(WorkflowTransitionResultCode.Success);
        }

        /// <summary>
        /// Transitions the specified workflow subject to a
        /// new state along a given <see cref="WorkflowTransition"/>.
        /// </summary>
        /// <param name="workflowSubject">
        /// Workflow subject to transition to a new state.
        /// </param>
        /// <param name="transitionName">
        /// Name of the <see cref="WorkflowTransition"/> to follow to
        /// the new state.
        /// </param>
        /// <param name="serviceProvider">
        /// Interface to service provider.
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowTransitionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        /// <remarks>
        /// The transition is only allowed if the
        /// <see cref="WorkflowTransition.Condition"/> evaluates to true.
        /// The <see cref="WorkflowState.ExitAction"/>,
        /// <see cref="WorkflowTransition.Action"/>, and
        /// <see cref="WorkflowState.EnterAction"/> are all fired
        /// along the way.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when workflowSubject or transitionName is null.
        /// </exception>
        /// <exception cref="StateNotFoundException">
        /// Thrown when the current state of the workflow subject cannot be
        /// found in the workflow.
        /// </exception>
        /// <exception cref="TransitionNotFoundException">
        /// Thrown when the specified transitionName cannot be found
        /// in the FROM state.
        /// </exception>
        /// <exception cref="ActionFailedException">
        /// Thrown when an action fails exiting a state, transitioning,
        /// or entering a state.
        /// </exception>
        public WorkflowTransitionResult TransitionTo(IWorkflowSubject workflowSubject, string transitionName, IServiceProvider serviceProvider)
        {
            WorkflowTransitionResult res = WorkflowTransitionResult.Success;

            ///////////////////////////////////////////////////////////////////
            // Check arguments
            if (workflowSubject == null)
            {
                throw new ArgumentNullException("workflowSubject");
            }

            if (string.IsNullOrEmpty(transitionName))
            {
                throw new ArgumentNullException("transitionName");
            }

            ///////////////////////////////////////////////////////////////////
            // Get the FROM state
            var currentStateName = workflowSubject.CurrentState;
            if (string.IsNullOrEmpty(currentStateName))
            {
                throw new StateNotFoundException(this, currentStateName);
            }

            var fromState = this.FindStateByName(currentStateName);
            if (fromState == null)
            {
                throw new StateNotFoundException(this, currentStateName);
            }

            ///////////////////////////////////////////////////////////////////
            // Get the transition
            var transition = fromState.GetTransition(transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException(fromState, transitionName);
            }

            ///////////////////////////////////////////////////////////////////
            // Check to see if the transition is allowed
            if (!transition.IsAllowed(serviceProvider, workflowSubject.GetContextObject(serviceProvider)))
            {
                return new WorkflowTransitionResult(
                    WorkflowTransitionResultCode.NotAllowed,
                    string.Format("Transition {0} not allowed {1}", transitionName, transition.ConditionErrorMessage));
            }           

            ///////////////////////////////////////////////////////////////////
            // Get the TO state
            var toState = (from s in this.States
                           where s.Name == transition.ToStateName
                           select s).FirstOrDefault();
            if (toState == null)
            {
                throw new StateNotFoundException(this, transition.ToStateName);
            }

            try
            {
                ///////////////////////////////////////////////////////////////////
                // Fire the pre-transition notification on the workflow subject
                workflowSubject.OnTransitioningTo(transition);

                CommandResult actionRes;

                ///////////////////////////////////////////////////////////////////
                // Execute ExitAction associated with the FROM state
                var exitActionTask = fromState.ExecuteExitAction(serviceProvider, workflowSubject.GetContextObject(serviceProvider));
                exitActionTask.RunSynchronously();
                actionRes = exitActionTask.Result;

                if (actionRes.IsSuccess)
                {
                    ///////////////////////////////////////////////////////////////////
                    // Execute Action associated with the transition
                    var transitionActionTask = transition.ExecuteAction(serviceProvider, workflowSubject.GetContextObject(serviceProvider));
                    transitionActionTask.RunSynchronously();
                    actionRes = transitionActionTask.Result;
                }

                if (actionRes.IsSuccess)
                {
                    ///////////////////////////////////////////////////////////////////
                    // Fire the post-transition notification on the workflow subject
                    workflowSubject.OnTransitionedTo(transition);
                }

                if (actionRes.IsSuccess)
                {
                    ///////////////////////////////////////////////////////////////////
                    // Set the current state of the workflow subject to the new state
                    workflowSubject.CurrentState = toState.Name;
                }

                if (actionRes.IsSuccess)
                {
                    ///////////////////////////////////////////////////////////////////
                    // Execute EnterAction associated with the TO state
                    var enterActionTask = toState.ExecuteEnterAction(serviceProvider, workflowSubject.GetContextObject(serviceProvider));
                    enterActionTask.RunSynchronously();
                    actionRes = enterActionTask.Result;
                }

                if (!actionRes.IsSuccess)
                {
                    res = new WorkflowTransitionResult(actionRes);
                }
            }
            catch (Exception ex)
            {
                res = new WorkflowTransitionResult(ex);
            }

            return res;
        }

        /// <summary>
        /// Check for disconnects between states and transitions.
        /// </summary>
        /// <returns>
        /// A <see cref="WorkflowAnalysis"/> object containing counters
        /// and information about errors found during analysis.
        /// </returns>
        public WorkflowAnalysis Analyze()
        {
            var res = new WorkflowAnalysis();
            var visitedStates = new List<WorkflowState>();
            this.Analyze(this.InitialState, visitedStates, res);
            return res;
        }

        #endregion

        #region Implementation

        private void Analyze(WorkflowState workflowState, ICollection<WorkflowState> visitedStates, WorkflowAnalysis res)
        {
            if (visitedStates.Contains(workflowState))
            {
                return;
            }

            visitedStates.Add(workflowState);
            res.StateCount = res.StateCount + 1;

            foreach (var transition in workflowState.Transitions)
            {
                res.TransitionCount = res.TransitionCount + 1;

                var toState = this.FindStateByName(transition.ToStateName);
                if (toState == null)
                {
                    res.AddError(new StateNotFoundException(this, transition.ToStateName));
                }
                else
                {
                    this.Analyze(toState, visitedStates, res);
                }
            }
        }

        #endregion
    }
}
