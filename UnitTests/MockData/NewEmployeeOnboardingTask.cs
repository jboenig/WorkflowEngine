namespace Headway.WorkflowEngine.UnitTests.MockData
{
    internal class NewEmployeeOnboardingTask : WorkflowItem
    {
        public NewEmployeeOnboardingTask()
        {
        }

        public NewEmployeeOnboardingTask(Headway.Dynamo.Metadata.ObjectType objType) :
             base(objType)
        {
        }
    }
}
