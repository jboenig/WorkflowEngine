using System.Collections.Generic;
using Headway.WorkflowEngine.Resolvers;
using Headway.Dynamo.Serialization;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal sealed class WorkflowItemTemplateResolver : IWorkflowItemTemplateResolver
    {
        private Dictionary<string, WorkflowItemTemplate> templates = new Dictionary<string, WorkflowItemTemplate>();
        private JsonResourceObjectResolver<WorkflowItemTemplate> jsonResourceTemplateResolver;

        public WorkflowItemTemplateResolver()
        {
            this.jsonResourceTemplateResolver = new JsonResourceObjectResolver<WorkflowItemTemplate>(this.GetType().Assembly);
        }

        public WorkflowItemTemplate Resolve(string key)
        {
            return this.jsonResourceTemplateResolver.Resolve(key);
        }
    }
}
