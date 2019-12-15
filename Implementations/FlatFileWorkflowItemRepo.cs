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
    /// store and retrieve <see cref="WorkflowItem"/> objects.
    /// </summary>
    public sealed class FlatFileWorkflowItemRepo : FlatFileRepo<WorkflowItem>,
        IWorkflowItemByIdResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serializerConfigSvc"></param>
        /// <param name="svcProvider"></param>
        public FlatFileWorkflowItemRepo(string filePath,
              ISerializerConfigService serializerConfigSvc,
              IServiceProvider svcProvider) :
            base(filePath, serializerConfigSvc, svcProvider)
        {
        }

        /// <summary>
        /// Resolves an object given a key value.
        /// </summary>
        /// <param name="key">Key value</param>
        /// <returns>Object matching the given key value.</returns>
        public WorkflowItem Resolve(object key)
        {
            return (from wi in this.GetQueryable()
                    where wi.PrimaryKey == key
                    select wi).FirstOrDefault();
        }
    }
}
