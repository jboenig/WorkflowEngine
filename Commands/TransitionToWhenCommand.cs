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
using System.Threading.Tasks;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Conditions;
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine.Commands
{
    /// <summary>
    /// This command is designed to be used in a workflow to
    /// invoke a transition when a specified condition is true.
    /// </summary>
    public sealed class TransitionToWhenCommand : Command
    {
        /// <summary>
        /// Gets or sets the name of the transition to use.
        /// </summary>
        public string TransitionName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Condition"/> that determines
        /// if the transition will occur.
        /// </summary>
        public Condition Condition
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns a <see cref="CommandResult"/> object that describes
        /// the result.
        /// </returns>
        /// <remarks>
        /// The <see cref="TransitionToWhenCommand.Condition"/> is evaluated and
        /// if true, the transition is applied.
        /// </remarks>
        public override Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
        {
            return new Task<CommandResult>(() =>
            {
                var workflowSubject = context as IWorkflowSubject;
                if (workflowSubject == null)
                {
                    var msg = string.Format("Context object must implement {0}", nameof(IWorkflowSubject));
                    throw new ArgumentException(msg, nameof(context));
                }

                var workflowResolver = serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
                if (workflowResolver == null)
                {
                    throw new ServiceNotFoundException(typeof(IWorkflowByNameResolver));
                }

                var workflow = workflowResolver.Resolve(workflowSubject.WorkflowName);
                if (workflow == null)
                {
                    var msg = string.Format("Workflow {0} not found", workflowSubject.WorkflowName);
                    throw new InvalidOperationException(msg);
                }

                if (string.IsNullOrEmpty(this.TransitionName))
                {
                    var msg = string.Format("TransitionName cannot be null");
                    throw new InvalidOperationException(msg);
                }

                CommandResult commandRes = CommandResult.Success;

                if (this.Condition == null || this.Condition.Evaluate(serviceProvider, context))
                {
                    // Apply the transition
                    var workflowTransitionRes = workflow.TransitionTo(workflowSubject, this.TransitionName, serviceProvider);
                    if (!workflowTransitionRes.IsSuccess)
                    {
                        commandRes = new BoolCommandResult(false, workflowTransitionRes.Description);
                    }
                }

                return commandRes;
            });
        }
    }
}
