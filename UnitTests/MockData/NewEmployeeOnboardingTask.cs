namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal class NewEmployeeOnboardingTask : WorkflowItemImpl
    {
        public NewEmployeeOnboardingTask(WorkflowItemType workflowItemType,
            string id,
            string workflowName) :
            base(workflowItemType, id, workflowName)
        {
        }
    }
}
