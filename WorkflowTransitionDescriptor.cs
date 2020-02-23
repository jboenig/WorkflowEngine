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

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Describes a <see cref="WorkflowTransition"/> in the
    /// context of a particular user and <see cref="IWorkflowSubject"/>.
    /// </summary>
    public sealed class WorkflowTransitionDescriptor
    {
        /// <summary>
        /// Gets or sets the name of the transition.
        /// </summary>
        [JsonProperty("transitionName")]
        public string TransitionName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the description of the transition.
        /// </summary>
        [JsonProperty("description")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the
        /// transition is allowed in the given context.
        /// </summary>
        [JsonProperty("isAllowed")]
        public bool IsAllowed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or set the reason the transition is not allowed.
        /// </summary>
        [JsonProperty("reason")]
        public string Reason
        {
            get;
            set;
        }
    }
}
