using System;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Interface to objects that can move through
    /// a <see cref="Workflow"/>.
    /// </summary>
    /// <remarks>
    /// This interface provides the minimal set of properties and
    /// methods needed to apply workflow transitions.
    /// </remarks>
    public interface IWorkflowSubject
    {
        /// <summary>
        /// Gets the <see cref="Workflow"/> associated with this object.
        /// </summary>
//        Workflow Workflow
//        {
//            get;
//        }

        /// <summary>
        /// Gets the fully qualified name of the <see cref="Workflow"/> associated with
        /// this object.
        /// </summary>
        string WorkflowName
        {
            get;
        }

        /// <summary>
        /// Gets the name of workflow current state this object is in.
        /// </summary>
        string CurrentState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the context object.
        /// </summary>
        /// <param name="svcProvider">Reference to service provider</param>
        /// <returns>Context object</returns>
        object GetContextObject(IServiceProvider svcProvider);

        /// <summary>
        /// Called before this object is transitioned to a new state.
        /// </summary>
        /// <param name="transition"></param>
        void OnTransitioningTo(WorkflowTransition transition);

        /// <summary>
        /// Called after this object is transitioned to a new state.
        /// </summary>
        /// <param name="transition"></param>
        void OnTransitionedTo(WorkflowTransition transition);
    }
}
