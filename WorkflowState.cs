////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.WorkflowEngine
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using Headway.Dynamo.Conditions;
using Headway.Dynamo.Commands;
using Headway.WorkflowEngine.Exceptions;

namespace Headway.WorkflowEngine;

/// <summary>
/// Encapsulates a state in a <see cref="Workflow"/>.
/// </summary>
public sealed class WorkflowState
{
    #region Member Variables

    private string name;
    private string description;
    private readonly HashSet<WorkflowTransition> transitions;
    private Condition canEnter;
    private Command enterAction;
    private Command exitAction;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    private WorkflowState()
    {
        this.transitions = new HashSet<WorkflowTransition>(new WorkflowTransition.EqualityComparer());
    }

    /// <summary>
    /// Constructs a <see cref="WorkflowState"/> given a name.
    /// </summary>
    public WorkflowState(string name)
    {
        this.name = name;
        this.transitions = new HashSet<WorkflowTransition>(new WorkflowTransition.EqualityComparer());
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the name of the state.
    /// </summary>
    [JsonProperty("name")]
    public string Name
    {
        get { return this.name; }
        set { this.name = value; }
    }

    /// <summary>
    /// Gets the description of the state.
    /// </summary>
    [JsonProperty("description")]
    public string Description
    {
        get { return this.description; }
        set { this.description = value; }
    }

    /// <summary>
    /// Gets the collection of transitions from this state to
    /// other states.
    /// </summary>
    [JsonProperty("transitions")]
    public ICollection<WorkflowTransition> Transitions
    {
        get { return this.transitions; }
    }

    /// <summary>
    /// Gets the <see cref="Condition"/> that determines if
    /// this state can be entered in the current context.
    /// </summary>
    [JsonProperty("canEnter")]
    public Condition CanEnter
    {
        get { return this.canEnter; }
        set { this.canEnter = value; }
    }

    /// <summary>
    /// Gets the <see cref="Command"/> that is executed
    /// when this state is entered.
    /// </summary>
    [JsonProperty("enterAction")]
    public Command EnterAction
    {
        get { return this.enterAction; }
        set { this.enterAction = value; }
    }

    /// <summary>
    /// Gets the <see cref="Command"/> that is executed
    /// when this state is exited.
    /// </summary>
    [JsonProperty("exitAction")]
    public Command ExitAction
    {
        get { return this.exitAction; }
        set { this.exitAction = value; }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets a <see cref="WorkflowTransition"/> by name.
    /// </summary>
    /// <param name="transitionName">
    /// Name of transition to retrieve.
    /// </param>
    /// <returns>
    /// Returns the <see cref="WorkflowTransition"/> object matching
    /// the specified name or null if no transition with the given
    /// name exists in this state.
    /// </returns>
    public WorkflowTransition GetTransition(string transitionName)
    {
        if (string.IsNullOrEmpty(transitionName))
        {
            throw new ArgumentNullException("transitionName");
        }

        return (from t in this.Transitions
                where t.Name == transitionName
                select t).FirstOrDefault();
    }

    /// <summary>
    /// Executes the enter action for this state if there is one.
    /// </summary>
    /// <param name="serviceProvider">Interface to service provider.</param>
    /// <param name="context">Context object for the command.</param>
    /// <remarks>
    /// If <see cref="WorkflowState.EnterAction"/> is null then this
    /// method does nothing.
    /// </remarks>
    /// <exception cref="ActionFailedException">
    /// Thrown if execution of the <see cref="Command"/> used for the enter
    /// action fails for any reason.
    /// </exception>
    public async Task<CommandResult> ExecuteEnterAction(IServiceProvider serviceProvider, object context)
    {
        var enterAction = this.EnterAction;
        if (enterAction != null)
        {
            return await enterAction.Execute(serviceProvider, context);
        }
        return CommandResult.Success;
    }

    /// <summary>
    /// Executes the exit action for this state if there is one.
    /// </summary>
    /// <param name="serviceProvider">Interface to service provider.</param>
    /// <param name="context">Context object for the command.</param>
    /// <remarks>
    /// If <see cref="WorkflowState.ExitAction"/> is null then this
    /// method does nothing.
    /// </remarks>
    /// <exception cref="ActionFailedException">
    /// Thrown if execution of the <see cref="Command"/> used for the exit
    /// action fails for any reason.
    /// </exception>
    public async Task<CommandResult> ExecuteExitAction(IServiceProvider serviceProvider, object context)
    {
        var exitAction = this.ExitAction;
        if (exitAction != null)
        {
            return await exitAction.Execute(serviceProvider, context);
        }
        return CommandResult.Success;
    }

    #endregion

    #region Equality Comparer Class

    /// <summary>
    /// 
    /// </summary>
    public sealed class EqualityComparer : IEqualityComparer<WorkflowState>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(WorkflowState x, WorkflowState y)
        {
            return (x.Name.CompareTo(y.Name) == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(WorkflowState obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    #endregion
}
