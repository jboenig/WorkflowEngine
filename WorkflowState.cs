using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Headway.Dynamo;
using Headway.Dynamo.Conditions;
using Headway.Dynamo.Commands;
using Headway.WorkflowEngine.Exceptions;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Encapsulates a state in a <see cref="Workflow"/>.
    /// </summary>
    public sealed class WorkflowState
    {
        #region Member Variables

        private string name;
        private readonly HashSet<WorkflowTransition> transitions;
        private Condition canEnter;
        private Command enterAction;
        private Command exitAction;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        private WorkflowState()
        {
            this.transitions = new HashSet<WorkflowTransition>(new WorkflowTransition.EqualityComparer());
        }

        /// <summary>
        /// Constructs a <see cref="WorkflowState"/> given a name.
        /// </summary>
        public WorkflowState(string name)
        {
            this.name = name;
            this.transitions = new HashSet<WorkflowTransition>(new WorkflowTransition.EqualityComparer());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the state.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets the collection of transitions from this state to
        /// other states.
        /// </summary>
        public ICollection<WorkflowTransition> Transitions
        {
            get { return this.transitions; }
        }

        /// <summary>
        /// Gets the <see cref="Condition"/> that determines if
        /// this state can be entered in the current context.
        /// </summary>
        public Condition CanEnter
        {
            get { return this.canEnter; }
            set { this.canEnter = value; }
        }

        /// <summary>
        /// Gets the <see cref="Command"/> that is executed
        /// when this state is entered.
        /// </summary>
        public Command EnterAction
        {
            get { return this.enterAction; }
            set { this.enterAction = value; }
        }

        /// <summary>
        /// Gets the <see cref="Command"/> that is executed
        /// when this state is exited.
        /// </summary>
        public Command ExitAction
        {
            get { return this.exitAction; }
            set { this.exitAction = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a <see cref="Workflowtransition"/> by name.
        /// </summary>
        /// <param name="transitionName">
        /// Name of transition to retrieve.
        /// </param>
        /// <returns>
        /// Returns the <see cref="WorkflowTransition"/> object matching
        /// the specified name or null if no transition with the given
        /// name exists in this state.
        /// </returns>
        public WorkflowTransition GetTransition(string transitionName)
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                throw new ArgumentNullException("transitionName");
            }

            return (from t in this.Transitions
                    where t.Name == transitionName
                    select t).FirstOrDefault();
        }

        /// <summary>
        /// Executes the enter action for this state if there is one.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider.</param>
        /// <param name="context">Context object for the command.</param>
        /// <remarks>
        /// If <see cref="WorkflowState.EnterAction"/> is null then this
        /// method does nothing.
        /// </remarks>
        /// <exception cref="ActionFailedException">
        /// Thrown if execution of the <see cref="Command"/> used for the enter
        /// action fails for any reason.
        /// </exception>
        public Task<CommandResult> ExecuteEnterAction(IServiceProvider serviceProvider, object context)
        {
            var enterAction = this.EnterAction;
            if (enterAction != null)
            {
                return enterAction.Execute(serviceProvider, context);
            }

            return new Task<CommandResult>(() =>
            {
                return CommandResult.Success;
            });
        }

        /// <summary>
        /// Executes the exit action for this state if there is one.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider.</param>
        /// <param name="context">Context object for the command.</param>
        /// <remarks>
        /// If <see cref="WorkflowState.ExitAction"/> is null then this
        /// method does nothing.
        /// </remarks>
        /// <exception cref="ActionFailedException">
        /// Thrown if execution of the <see cref="Command"/> used for the exit
        /// action fails for any reason.
        /// </exception>
        public Task<CommandResult> ExecuteExitAction(IServiceProvider serviceProvider, object context)
        {
            var exitAction = this.ExitAction;
            if (exitAction != null)
            {
                return exitAction.Execute(serviceProvider, context);
            }

            return new Task<CommandResult>(() =>
            {
                return CommandResult.Success;
            });
        }

        #endregion

        #region Equality Comparer Class

        /// <summary>
        /// 
        /// </summary>
        public sealed class EqualityComparer : IEqualityComparer<WorkflowState>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(WorkflowState x, WorkflowState y)
            {
                return (x.Name.CompareTo(y.Name) == 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(WorkflowState obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #endregion
    }
}
