using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

using Headway.Dynamo.Runtime;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.UnitTests.MockData;
using Headway.WorkflowEngine.Resolvers;

namespace WorkflowEngine.UnitTests
{
    [TestClass]
    public class WorkflowTransitionTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IWorkflowByNameResolver>().To<WorkflowByNameResolver>();
            kernel.Bind<IWorkflowItemTemplateResolver>().To<WorkflowItemTemplateResolver>();
            kernel.Bind<IWorkflowItemTypeResolver>().To<WorkflowItemTypeResolver>();
            kernel.Bind<IWorkflowService>().To<StandardWorkflowService>();
            kernel.Bind<IServiceProvider>().ToConstant(kernel);
            this.serviceProvider = kernel;

            //var serviceContainer = new ServiceContainer();
            //serviceContainer.AddService(typeof(IWorkflowByNameResolver), new WorkflowByNameResolver());
            //serviceContainer.AddService(typeof(IWorkflowItemTemplateResolver), new WorkflowItemTemplateResolver());
            //serviceContainer.AddService(typeof(IWorkflowItemTypeResolver), new WorkflowItemTypeResolver());
            //            serviceContainer.AddService(typeof(IObjectResolver<PrimaryKeyValue, Workflow>), new WorkflowByPKResolver());
            //this.serviceProvider = serviceContainer;
        }

        [TestMethod]
        public void TransitionTest1()
        {
            var workflowService = this.serviceProvider.GetService(typeof(IWorkflowService)) as IWorkflowService;

            var workflowItem = workflowService.CreateWorkflowItem("MockData.Templates.Test1Template", null);
            Assert.IsNotNull(workflowItem);
            Assert.AreEqual(workflowItem.ItemType.DisplayName, "Issue");
            Assert.AreEqual(workflowItem.ItemType.Abbreviation, "ISS");

            var workflowResolver = this.serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            Assert.IsNotNull(workflowResolver);
            var workflow = workflowResolver.Resolve(workflowItem.WorkflowName);
            Assert.IsNotNull(workflow);

            var res = workflow.TransitionTo(workflowItem, "Start", this.serviceProvider);
            Assert.IsTrue(res.IsSuccess);
        }

        [TestMethod]
        public void TransitionToWhenTest()
        {
            var workflowItemTemplateResolver = this.serviceProvider.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);

            var workflowItem = workflowItemTemplate.CreateInstance(this.serviceProvider, null) as WorkflowItemImpl;
            Assert.IsNotNull(workflowItem);
            Assert.AreEqual(workflowItem.ItemType.DisplayName, "Issue");
            Assert.AreEqual(workflowItem.ItemType.Abbreviation, "ISS");

            var workflowResolver = this.serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            Assert.IsNotNull(workflowResolver);
            var workflow = workflowResolver.Resolve(workflowItem.WorkflowName);
            Assert.IsNotNull(workflow);

            var resStartTransition = workflow.TransitionTo(workflowItem, "Start", this.serviceProvider);
            Assert.IsTrue(resStartTransition.IsSuccess);

            var resNeedMoreInfoTransition = workflow.TransitionTo(workflowItem, "Need More Info", this.serviceProvider);
            Assert.IsTrue(resNeedMoreInfoTransition.IsSuccess);

            Assert.AreEqual(workflowItem.CurrentState, "Reviewing");
            Assert.AreEqual(workflowItem.Info, "Dude");
        }

        [TestMethod]
        public void TransitionOnboardingTest1()
        {
            var workflowService = this.serviceProvider.GetService(typeof(IWorkflowService)) as IWorkflowService;

            var workflowItem = workflowService.CreateWorkflowItem("MockData.Templates.NewEmployeeOnboardingTaskTemplate", null);
            Assert.IsNotNull(workflowItem);

            var workflowResolver = this.serviceProvider.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            Assert.IsNotNull(workflowResolver);
            var workflow = workflowResolver.Resolve(workflowItem.WorkflowName);
            Assert.IsNotNull(workflow);

            var resTransition1 = workflow.TransitionTo(workflowItem, "Create Employee Records", this.serviceProvider);
            Assert.IsTrue(resTransition1.IsSuccess);
        }
    }
}
