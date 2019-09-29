using Headway.Dynamo.Runtime;

namespace Headway.WorkflowEngine
{
    public interface IWorkflowItemByIdResolver : IObjectResolver<string, WorkflowItem>
    {
    }
}
