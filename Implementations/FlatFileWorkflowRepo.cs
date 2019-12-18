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
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFileRepo;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine.Implementations
{
    /// <summary>
    /// Implements a <see cref="FlatFileRepo{TObject}"/> to
    /// store and retrieve <see cref="Workflow"/> objects.
    /// </summary>
    public sealed class FlatFileWorkflowRepo : FlatFileRepo<Workflow>,
        IWorkflowByNameResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serializerConfigSvc"></param>
        /// <param name="svcProvider"></param>
        public FlatFileWorkflowRepo(string filePath,
              ISerializerConfigService serializerConfigSvc,
              IServiceProvider svcProvider) :
            base(filePath, serializerConfigSvc, svcProvider)
        {
        }

        /// <summary>
        /// Resolves a <see cref="Workflow"/> object given a
        /// fully-qualified workflow name.
        /// </summary>
        /// <param name="workflowName">Fully qualified name of the workflow</param>
        /// <returns>Workflow matching the given workflow name.</returns>
        public Workflow Resolve(string workflowName)
        {
            return (from w in this.GetQueryable()
                    where w.FullName == workflowName
                    select w).FirstOrDefault();
        }
    }
}
