using System;
using Newtonsoft.Json;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal sealed class WorkflowJsonConverter : ObjectRefJsonConverter<string, Workflow>
    {
    }

    internal sealed class JsonConverterService : IJsonConverterService
    {
        public JsonConverter[] GetConverters(ObjectType objType)
        {
            return new JsonConverter[]
            {
                new WorkflowJsonConverter()
            };
        }
    }
}
