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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Factories;
using Headway.WorkflowEngine.Services;
using Headway.WorkflowEngine.Implementations;
using Headway.WorkflowEngine.UnitTests.MockData;

namespace WorkflowEngine.UnitTests
{
    [TestClass]
    public class WorkflowTransitionTests
    {
        private IKernel kernel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<IWorkflowByNameResolver>().To<JsonResourceWorkflowByNameResolver>().
                WithConstructorArgument("svcProvider", this.kernel).
                WithConstructorArgument("assembly", this.GetType().Assembly);
            this.kernel.Bind<IWorkflowItemTemplateResolver>().To<JsonResourceWorkflowItemTemplateResolver>().
                WithConstructorArgument("svcProvider", this.kernel).
                WithConstructorArgument("assembly", this.GetType().Assembly);
            this.kernel.Bind<ISerializerConfigService>().To<StandardSerializerConfigService>();
            this.kernel.Bind<IMetadataProvider>().To<StandardMetadataProvider>();
            this.kernel.Bind<IWorkflowItemFactory>().To<StandardWorkflowItemFactory>();
            this.kernel.Bind<IWorkflowExecutionService>().To<StandardWorkflowExecutionService>();
            this.kernel.Bind<IServiceProvider>().ToConstant(kernel);
        }

        [TestMethod]
        public void TransitionTest1()
        {
            var workflowItemFactory = this.kernel.GetService(typeof(IWorkflowItemFactory)) as IWorkflowItemFactory;

            var workflowItem = workflowItemFactory.CreateWorkflowItem("MockData.Templates.Test1Template", null);
            Assert.IsNotNull(workflowItem);

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var resExec = workflowExecutionSvc.StartWorkflow(workflowItem);
            Assert.IsTrue(resExec.IsSuccess);

            var resTransition = workflowExecutionSvc.TransitionTo(workflowItem, "Start");
            Assert.IsTrue(resTransition.IsSuccess);
        }

        [TestMethod]
        public void TransitionToWhenTest()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);

            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null) as MyWorkflowItem;
            Assert.IsNotNull(workflowItem);

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var resStartWorkflow = workflowExecutionSvc.StartWorkflow(workflowItem, "MockData.Workflows.Test1Workflow");
            Assert.IsTrue(resStartWorkflow.IsSuccess);

            var resStartTransition = workflowExecutionSvc.TransitionTo(workflowItem, "Start");
            Assert.IsTrue(resStartTransition.IsSuccess);

            var resNeedMoreInfoTransition = workflowExecutionSvc.TransitionTo(workflowItem, "Need More Info");
            Assert.IsTrue(resNeedMoreInfoTransition.IsSuccess);

            Assert.AreEqual(workflowItem.CurrentState, "Reviewing");
            Assert.AreEqual(workflowItem.Info, "Dude");
        }

        [TestMethod]
        public void TransitionOnboardingTest1()
        {
            var workflowItemFactory = this.kernel.GetService(typeof(IWorkflowItemFactory)) as IWorkflowItemFactory;

            var workflowItem = workflowItemFactory.CreateWorkflowItem("MockData.Templates.NewEmployeeOnboardingTaskTemplate", null);
            Assert.IsNotNull(workflowItem);

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var resStartWorkflow = workflowExecutionSvc.StartWorkflow(workflowItem);
            Assert.IsTrue(resStartWorkflow.IsSuccess);

            var resTransition1 = workflowExecutionSvc.TransitionTo(workflowItem, "Create Employee Records");
            Assert.IsTrue(resTransition1.IsSuccess);
        }
    }
}
