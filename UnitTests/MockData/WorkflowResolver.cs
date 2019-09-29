using System;
using Headway.Dynamo.Serialization;
using Headway.WorkflowEngine.Resolvers;
using Headway.Dynamo.Runtime;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal sealed class WorkflowByNameResolver : IWorkflowByNameResolver
    {
        private JsonResourceObjectResolver<Workflow> jsonResourceWorkflowResolver;

        public WorkflowByNameResolver()
        {
            this.jsonResourceWorkflowResolver = new JsonResourceObjectResolver<Workflow>(this.GetType().Assembly);
        }

        public Workflow Resolve(string key)
        {
            return this.jsonResourceWorkflowResolver.Resolve(key);
        }
    }

#if false
    internal sealed class WorkflowByPKResolver : IObjectResolver<PrimaryKeyValue, Workflow>
    {
        private JsonResourceObjectResolver<Workflow> jsonResourceWorkflowResolver;

        public WorkflowByPKResolver()
        {
            this.jsonResourceWorkflowResolver = new JsonResourceObjectResolver<Workflow>(this.GetType().Assembly);
        }

        public Workflow Resolve(PrimaryKeyValue key)
        {
            return this.jsonResourceWorkflowResolver.Resolve((string)key.Value);
        }
    }
#endif
}
