using System;
using System.Runtime.Serialization;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Reflection;
using Newtonsoft.Json;

namespace Headway.WorkflowEngine
{
    /// <summary>
    /// Provides metadata for a type of workflow item.
    /// </summary>
    public class WorkflowItemType
    {
        private string objectTypeFullName;
        private ObjectType objectType;
        private IMetadataProvider metadataProvider;

        public WorkflowItemType(IMetadataProvider metadataProvider)
        {
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Gets or sets the fully qualified name of this
        /// <see cref="WorkflowItemType"/> object.
        /// </summary>
        public string FullName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the display name used to refer to this type
        /// of workflow item (e.g. Issue, Ticket,
        /// Case, Incident, ...)
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the abbreviated name used to refer to this type
        /// of workflow item (e.g. ISS, TCKT,
        /// CASE, INCD, ...)
        /// </summary>
        public string Abbreviation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the full name of the object type.
        /// </summary>
        public string ObjectTypeFullName
        {
            get
            {
                if (this.ObjectType != null)
                {
                    return this.ObjectType.FullName;
                }
                return this.objectTypeFullName;
            }
            set
            {
                this.objectTypeFullName = value;
                this.ObjectType = null;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ObjectType"/> metadata
        /// for this <see cref="WorkflowItemType"/>.
        /// </summary>
        [JsonIgnore]
        public ObjectType ObjectType
        {
            get
            {
                if (this.objectType == null)
                {
                    if (this.objectTypeFullName != null)
                    {
                        // Resolve object type
                        this.objectType = this.metadataProvider.GetDataType<ObjectType>(this.objectTypeFullName);
                    }
                }
                return this.objectType;
            }
            set
            {
                this.objectType = value;
                if (this.objectType != null)
                {
                    this.objectTypeFullName = this.objectType.FullName;
                }
            }
        }

        #region Serialization

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            var svcProvider = context.Context as IServiceProvider;
            if (svcProvider != null)
            {
                this.metadataProvider = svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            }
        }

        #endregion

        /// <summary>
        /// Returns the logical name of the view used to
        /// render the given workflow item.
        /// </summary>
        /// <param name="viewMode">Mode of view to return</param>
        /// <returns>
        /// Name of the view to use for this type of workflow item
        /// and the specified workflow state and context.
        /// </returns>
        //public abstract string GetViewName(WorkflowViewModes viewMode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewMode"></param>
        /// <param name="viewName"></param>
        //public abstract void SetViewName(WorkflowViewModes viewMode, string viewName);
    }
}
