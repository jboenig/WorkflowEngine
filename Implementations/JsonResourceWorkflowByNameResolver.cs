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

using Headway.WorkflowEngine.Resolvers;
using System;
using Headway.Dynamo.Serialization;

namespace Headway.WorkflowEngine.Implementations
{
    /// <summary>
    /// Implements <see cref="IWorkflowByNameResolver"/> by retrieving
    /// <see cref="Workflow"/> objects from an assembly resource and
    /// parsing as JSON.
    /// </summary>
    public sealed class JsonResourceWorkflowByNameResolver : IWorkflowByNameResolver
    {
        private JsonResourceObjectResolver<Workflow> jsonResourceWorkflowResolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svcProvider"></param>
        /// <param name="assembly"></param>
        public JsonResourceWorkflowByNameResolver(IServiceProvider svcProvider,
            System.Reflection.Assembly assembly)
        {
            this.jsonResourceWorkflowResolver = new JsonResourceObjectResolver<Workflow>(svcProvider, assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Workflow Resolve(string key)
        {
            return this.jsonResourceWorkflowResolver.Resolve(key);
        }
    }
}
