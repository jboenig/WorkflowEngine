using System;

namespace Headway.WorkflowEngine.Exceptions
{
    /// <summary>
    /// Base class for workflow exceptions.
    /// </summary>
    public abstract class WorkflowException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WorkflowException()
        {
        }

        /// <summary>
        /// Construct from a message and inner exception.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public WorkflowException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
