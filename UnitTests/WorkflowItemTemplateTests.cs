using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Headway.Dynamo.Runtime;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.UnitTests.MockData;
using Headway.WorkflowEngine.Resolvers;

namespace WorkflowEngine.UnitTests
{
    [TestClass]
    public class WorkflowItemTemplateTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceContainer = new ServiceContainer();
            serviceContainer.AddService(typeof(IWorkflowByNameResolver), new WorkflowByNameResolver());
            serviceContainer.AddService(typeof(IWorkflowItemTemplateResolver), new WorkflowItemTemplateResolver());
            serviceContainer.AddService(typeof(IWorkflowItemTypeResolver), new WorkflowItemTypeResolver());
//            serviceContainer.AddService(typeof(IObjectResolver<PrimaryKeyValue, Workflow>), new WorkflowByPKResolver());
            this.serviceProvider = serviceContainer;
        }

        [TestMethod]
        public void CreateWorkflowItemTest1()
        {
            var workflowItemTemplateResolver = this.serviceProvider.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);
            var workflowItem = workflowItemTemplate.CreateInstance(this.serviceProvider, null);
            Assert.IsNotNull(workflowItem);
            Assert.AreEqual(workflowItem.ItemType.DisplayName, "Issue");
            Assert.AreEqual(workflowItem.ItemType.Abbreviation, "ISS");
        }
    }
}
