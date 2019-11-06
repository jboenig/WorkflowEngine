using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

using Headway.Dynamo.Runtime;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Metadata;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.UnitTests.MockData;
using Headway.WorkflowEngine.Resolvers;

namespace WorkflowEngine.UnitTests
{
    [TestClass]
    public class WorkflowItemTemplateTests
    {
        private IKernel kernel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<IServiceProvider>().ToConstant(this.kernel);
            this.kernel.Bind<IWorkflowByNameResolver>().To<WorkflowByNameResolver>();
            this.kernel.Bind<IWorkflowItemTemplateResolver>().To<WorkflowItemTemplateResolver>();
            this.kernel.Bind<IWorkflowItemTypeResolver>().To<WorkflowItemTypeResolver>();
            this.kernel.Bind<ISerializerConfigService>().To<StandardSerializerConfigService>();
            this.kernel.Bind<IMetadataProvider>().To<StandardMetadataProvider>();
        }

        [TestMethod]
        public void CreateWorkflowItemTest1()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);
            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null);
            Assert.IsNotNull(workflowItem);
            Assert.AreEqual(workflowItem.ItemType.DisplayName, "Issue");
            Assert.AreEqual(workflowItem.ItemType.Abbreviation, "ISS");
        }
    }
}
