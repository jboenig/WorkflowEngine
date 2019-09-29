namespace Headway.WorkflowEngine
{
    public interface IWorkflowService
    {
        WorkflowItem CreateWorkflowItem(string templateName, object context);

        WorkflowTransitionResult TransitionTo(WorkflowItem item, string transitionName);
    }
}
