using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Headway.Dynamo.Runtime;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Conditions;
using Headway.WorkflowEngine.Exceptions;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Encapsulates a transition from one <see cref="WorkflowState"/>
    /// to another.
    /// </summary>
    public sealed class WorkflowTransition
    {
        #region Member Variables

        private string name;
        private string toStateName;
        private Condition isAllowed;
        private Command action;
        private string userPrompt;
        private string userPromptView;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        private WorkflowTransition()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="toStateName"></param>
        public WorkflowTransition(string name, string toStateName)
        {
            this.name = name;
            this.toStateName = toStateName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the transition.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets the name of the state this transition goes to.
        /// </summary>
        public string ToStateName
        {
            get { return this.toStateName; }
        }

        /// <summary>
        /// Gets the <see cref="Condition"/> that determines if
        /// this transition is allowed in the current context.
        /// state.
        /// </summary>
        public Condition Condition
        {
            get { return this.isAllowed; }
            set { this.isAllowed = value; }
        }

        /// <summary>
        /// Gets the <see cref="Command"/> that is executed
        /// when this transition is taken.
        /// </summary>
        public Command Action
        {
            get { return this.action; }
            set { this.action = value; }
        }

        /// <summary>
        /// Gets or sets the prompt presented to the user prior
        /// to applying this transition.
        /// </summary>
        public string UserPrompt
        {
            get { return this.userPrompt; }
            set { this.userPrompt = value; }
        }

        /// <summary>
        /// Gets or sets the prompt view presented to the user prior
        /// to applying this transition.
        /// </summary>
        public string UserPromptView
        {
            get { return this.userPromptView; }
            set { this.userPromptView = value; }
        }

        /// <summary>
        /// Gets or sets the condition evaluation message
        /// to applying this transition.
        /// </summary>
        public string ConditionErrorMessage
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsAllowed(IServiceProvider serviceProvider, object context)
        {
            var isAllowed = true;

            var condition = this.Condition;
            if (condition != null)
            {
                try
                {
                    isAllowed = condition.Evaluate(serviceProvider, context);
                }
                catch (Exception ex)
                {
                    isAllowed = false;
                    this.ConditionErrorMessage = ex.Message;
                }
            }

            return isAllowed;
        }

        /// <summary>
        /// Executes the action assigned to the <see cref="WorkflowTransition.Action"/>
        /// property.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider.</param>
        /// <param name="context"></param>
        /// <remarks>
        /// If <see cref="WorkflowState.Action"/> is null then this
        /// method does nothing.
        /// </remarks>
        /// <exception cref="ActionFailedException">
        /// Thrown if execution of the <see cref="Command"/> assigned to the
        /// action fails for any reason.
        /// </exception>
        public Task<CommandResult> ExecuteAction(IServiceProvider serviceProvider, object context)
        {
            var action = this.Action;
            if (action != null)
            {
                return action.Execute(serviceProvider, context);
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
        public sealed class EqualityComparer : IEqualityComparer<WorkflowTransition>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(WorkflowTransition x, WorkflowTransition y)
            {
                return (x.Name.CompareTo(y.Name) == 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(WorkflowTransition obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
    }
}
