using Headway.Dynamo.Runtime;

namespace Headway.WorkflowEngine.Resolvers
{
    /// <summary>
    /// Resolves <see cref="Workflow"/> objects given
    /// a unique name as a key.
    /// </summary>
    public interface IWorkflowByNameResolver : IObjectResolver<string, Workflow>
    {
    }
}
