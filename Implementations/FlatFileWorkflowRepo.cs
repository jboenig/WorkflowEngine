﻿////////////////////////////////////////////////////////////////////////////////
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
using System.Linq;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.Implementation;
using Headway.WorkflowEngine.Repository;

namespace Headway.WorkflowEngine.Implementations
{
    /// <summary>
    /// Implements a <see cref="FlatFileRepo{TObject}"/> to
    /// store and retrieve <see cref="Workflow"/> objects.
    /// </summary>
    public sealed class FlatFileWorkflowRepo : JsonFileRepo<Workflow>,
        IWorkflowRepository
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
        /// 
        /// </summary>
        /// <param name="serializerConfigSvc"></param>
        /// <param name="svcProvider"></param>
        public FlatFileWorkflowRepo(ISerializerConfigService serializerConfigSvc,
              IServiceProvider svcProvider) :
            base(serializerConfigSvc, svcProvider)
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
