using System;

using Headway.Dynamo.Commands;

namespace Headway.WorkflowEngine.Exceptions
{
    /// <summary>
    /// Exception thrown when an action fails during workflow execution.
    /// </summary>
    public sealed class ActionFailedException : WorkflowException
    {
        public ActionFailedException(string message)
        {
        }

        public ActionFailedException(string message, Exception innerException)
        {
        }
    }
}
