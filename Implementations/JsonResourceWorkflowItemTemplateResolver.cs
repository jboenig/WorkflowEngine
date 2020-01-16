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
        /// <param name="svcProvider"></param>
        public JsonResourceWorkflowItemTemplateResolver(IServiceProvider svcProvider)
        {
            this.jsonResourceTemplateResolver = new JsonResourceObjectResolver<WorkflowItemTemplate>(svcProvider);
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
