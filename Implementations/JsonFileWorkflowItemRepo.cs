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

using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.Implementation;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine.Implementations;

/// <summary>
/// Implements a <see cref="JsonFileRepo{TObject}"/> to
/// store and retrieve <see cref="WorkflowItem"/> objects.
/// </summary>
public sealed class JsonFileWorkflowItemRepo : JsonFileRepo<WorkflowItem>,
    IWorkflowItemByIdResolver
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="serializerConfigSvc"></param>
    /// <param name="svcProvider"></param>
    public JsonFileWorkflowItemRepo(string filePath,
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
