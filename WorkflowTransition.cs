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
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        private string description;
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
        [JsonProperty("name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets the description of the state.
        /// </summary>
        [JsonProperty("description")]
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Gets the name of the state this transition goes to.
        /// </summary>
        [JsonProperty("toStateName")]
        public string ToStateName
        {
            get { return this.toStateName; }
        }

        /// <summary>
        /// Gets the <see cref="Condition"/> that determines if
        /// this transition is allowed in the current context.
        /// state.
        /// </summary>
        [JsonProperty("condition")]
        public Condition Condition
        {
            get { return this.isAllowed; }
            set { this.isAllowed = value; }
        }

        /// <summary>
        /// Gets the <see cref="Command"/> that is executed
        /// when this transition is taken.
        /// </summary>
        [JsonProperty("action")]
        public Command Action
        {
            get { return this.action; }
            set { this.action = value; }
        }

        /// <summary>
        /// Gets or sets the prompt presented to the user prior
        /// to applying this transition.
        /// </summary>
        [JsonProperty("userPrompt")]
        public string UserPrompt
        {
            get { return this.userPrompt; }
            set { this.userPrompt = value; }
        }

        /// <summary>
        /// Gets or sets the prompt view presented to the user prior
        /// to applying this transition.
        /// </summary>
        [JsonProperty("userPromptView")]
        public string UserPromptView
        {
            get { return this.userPromptView; }
            set { this.userPromptView = value; }
        }

        /// <summary>
        /// Gets or sets the condition evaluation message
        /// to applying this transition.
        /// </summary>
        [JsonProperty("conditionErrorMessage")]
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
                    isAllowed = Task.Run(() => condition.Evaluate(serviceProvider, context)).GetAwaiter().GetResult();
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
        /// If <see cref="WorkflowTransition.Action"/> is null then this
        /// method does nothing.
        /// </remarks>
        /// <exception cref="ActionFailedException">
        /// Thrown if execution of the <see cref="Command"/> assigned to the
        /// action fails for any reason.
        /// </exception>
        public async Task<CommandResult> ExecuteAction(IServiceProvider serviceProvider, object context)
        {
            var action = this.Action;
            if (action != null)
            {
                return await action.Execute(serviceProvider, context);
            }

            return CommandResult.Success;
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
