using System;
using Headway.Dynamo.Serialization;
using Headway.WorkflowEngine.Resolvers;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal sealed class WorkflowItemTypeResolver : IWorkflowItemTypeResolver
    {
        private JsonResourceObjectResolver<WorkflowItemType> jsonResourceItemTypeResolver;

        public WorkflowItemTypeResolver(IServiceProvider svcProvider)
        {
            this.jsonResourceItemTypeResolver = new JsonResourceObjectResolver<WorkflowItemType>(svcProvider, this.GetType().Assembly);
        }

        public WorkflowItemType Resolve(string key)
        {
            return this.jsonResourceItemTypeResolver.Resolve(key);
        }
    }
}
