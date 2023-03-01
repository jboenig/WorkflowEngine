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
        public override async Task<CommandResult> ExecuteAsync(IServiceProvider serviceProvider, object context)
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

            if (this.Condition == null || await this.Condition.EvaluateAsync(serviceProvider, context))
            {
                // Apply the transition
                var workflowTransitionRes = await workflow.TransitionTo(workflowSubject, this.TransitionName, serviceProvider);
                if (!workflowTransitionRes.IsSuccess)
                {
                    commandRes = new BoolCommandResult(false, workflowTransitionRes.Description);
                }
            }

            return commandRes;
        }
    }
}
