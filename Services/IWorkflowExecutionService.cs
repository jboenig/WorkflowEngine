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

namespace Headway.WorkflowEngine.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWorkflowExecutionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="workflowName"></param>
        /// <returns></returns>
        WorkflowExecutionResult StartWorkflow(WorkflowItem item, string workflowName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="transitionName"></param>
        /// <returns></returns>
        WorkflowExecutionResult TransitionTo(WorkflowItem item, string transitionName);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class WorkflowExecutionServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowTransitionService"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static WorkflowExecutionResult StartWorkflow(this IWorkflowExecutionService workflowTransitionService,
            WorkflowItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var workflowName = item.WorkflowName;
            if (string.IsNullOrEmpty(workflowName))
            {
                var msg = $"{item} is not associated with a workflow";
                throw new InvalidOperationException(msg);
            }

            return workflowTransitionService.StartWorkflow(item, workflowName);
        }
    }
}
