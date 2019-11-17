////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.WorkflowEngine.
//
// Headway.WorkflowEngine is free software: you can redistribute it and/or
// modify it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
//
// Headway.WorkflowEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR PARTICULAR PURPOSE. See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with Headway.WorkflowEngine. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;
using Headway.Dynamo.Metadata;

namespace Headway.WorkflowEngine.UnitTests.MockData
{
    [Serializable]
    public sealed class MyWorkflowItem : WorkflowItem
    {
        public MyWorkflowItem()
        {
        }

        public MyWorkflowItem(ObjectType objType) :
            base(objType)
        {
        }

        public string Info
        {
            get;
            set;
        }

        #region Serialization

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialiation info</param>
        /// <param name="context">Streaming context</param>
        /// <remarks>
        /// Deserializes the given SerializationInfo into a new
        /// instance of this class.
        /// </remarks>
        private MyWorkflowItem(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        #endregion
    }
}
