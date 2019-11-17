using System;
using Newtonsoft.Json;
using Headway.Dynamo.Serialization;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal sealed class WorkflowJsonConverter : ObjectRefJsonConverter<string, Workflow>
    {
    }

    internal sealed class JsonConverterService : IJsonConverterService
    {
        public JsonConverter[] GetConverters(Type objType)
        {
            return new JsonConverter[]
            {
                new WorkflowJsonConverter()
            };
        }
    }
}
