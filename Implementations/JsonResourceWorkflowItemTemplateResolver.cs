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
using System.Collections.Generic;
using Headway.Dynamo.Serialization;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class JsonResourceWorkflowItemTemplateResolver : IWorkflowItemTemplateResolver
    {
        private Dictionary<string, WorkflowItemTemplate> templates = new Dictionary<string, WorkflowItemTemplate>();
        private JsonResourceObjectResolver<WorkflowItemTemplate> jsonResourceTemplateResolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svcProvider"></param>
        /// <param name="assembly"></param>
        public JsonResourceWorkflowItemTemplateResolver(IServiceProvider svcProvider,
            System.Reflection.Assembly assembly)
        {
            this.jsonResourceTemplateResolver = new JsonResourceObjectResolver<WorkflowItemTemplate>(svcProvider, assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public WorkflowItemTemplate Resolve(string key)
        {
            return this.jsonResourceTemplateResolver.Resolve(key);
        }
    }
}
