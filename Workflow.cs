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
        /// Gets or sets the primary key for this object.
        /// </summary>
        [JsonIgnore]
        public object PrimaryKey
        {
            get { return this.FullName; }
            set { this.FullName = value.ToString(); }
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
        /// Starts execution of an <see cref="IWorkflowSubject"/> object
        /// in this workflow.
        /// </summary>
        /// <param name="workflowSubject">
        /// Workflow subject to start in this workflow.
        /// </param>
        /// <param name="serviceProvider">
        /// Interface to service provider.
        /// </param>
        /// <returns>
        /// Returns a <see cref="WorkflowExecutionResult"/> object
        /// that encapsulates the result of the operation.
        /// </returns>
        /// <remarks>
        /// This method associates the <see cref="IWorkflowSubject"/> with
        /// this workflow, sets the <see cref="IWorkflowSubject.CurrentState"/>
        /// to the <see cref="Workflow.InitialState"/> of this workflow, and
        /// executes the enter action of the initial state.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when workflowSubject is null.
        /// </exception>
        /// <exception cref="InitialStateNotFoundException">
        /// Thrown when the workflow has no initial state defined.
        /// </exception>
        public WorkflowExecutionResult Start(IWorkflowSubject workflowSubject, IServiceProvider serviceProvider)
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
            // Assign the name of the workflow to the object
            workflowSubject.WorkflowName = this.FullName;

            ///////////////////////////////////////////////////////////////////
            // Set the current state of the workflow subject
            // to the INITIAL state
            workflowSubject.CurrentState = initialState.Name;

            ///////////////////////////////////////////////////////////////////
            // Invoke OnStarted callback
            workflowSubject.OnStarted(this);

            return WorkflowExecutionResult.Success;
        }

        /// <summary>
        /// Determines if the specified transition is allowed
        /// for the workflow subject.
        /// </summary>
        /// <param name="workflowSubject">
        /// Workflow subject to test
        /// </param>
        /// <param name="transitionName">
        /// Transition to test
        /// </param>
        /// <param name="serviceProvider">
        /// Service provider
        /// </param>
        /// <returns>
        /// Returns true if the transition is allowed, otherwise
        /// returns false
        /// </returns>
        /// <remarks>
        /// Checks condition associated with the transition against
        /// the context provided by the workflow subject.
        /// </remarks>
        public WorkflowExecutionResult IsTransitionToAllowed(IWorkflowSubject workflowSubject,
            string transitionName,
            IServiceProvider serviceProvider)
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
                return new WorkflowExecutionResult(WorkflowExecutionResultCode.NotAllowed, string.Format("Transition {0} not allowed {1}", transitionName, transition.ConditionErrorMessage));
            }

            return new WorkflowExecutionResult(WorkflowExecutionResultCode.Success);
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
        /// Returns a <see cref="WorkflowExecutionResult"/> object
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
        public WorkflowExecutionResult TransitionTo(IWorkflowSubject workflowSubject, string transitionName, IServiceProvider serviceProvider)
        {
            WorkflowExecutionResult res = WorkflowExecutionResult.Success;

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
                return new WorkflowExecutionResult(
                    WorkflowExecutionResultCode.NotAllowed,
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
                workflowSubject.OnTransitioningTo(this, transition);

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
                    workflowSubject.OnTransitionedTo(this, transition);
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
                    res = new WorkflowExecutionResult(actionRes);
                }
            }
            catch (Exception ex)
            {
                res = new WorkflowExecutionResult(ex);
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
