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
            this.kernel.Bind<IWorkflowTransitionService>().To<StandardWorkflowTransitionService>();
            this.kernel.Bind<IServiceProvider>().ToConstant(kernel);
        }

        [TestMethod]
        public void TransitionTest1()
        {
            var workflowItemFactory = this.kernel.GetService(typeof(IWorkflowItemFactory)) as IWorkflowItemFactory;

            var workflowItem = workflowItemFactory.CreateWorkflowItem("MockData.Templates.Test1Template", null);
            Assert.IsNotNull(workflowItem);

            var workflowResolver = this.kernel.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            Assert.IsNotNull(workflowResolver);
            var workflow = workflowResolver.Resolve(workflowItem.WorkflowName);
            Assert.IsNotNull(workflow);

            var res = workflow.TransitionTo(workflowItem, "Start", this.kernel);
            Assert.IsTrue(res.IsSuccess);
        }

        [TestMethod]
        public void TransitionToWhenTest()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);

            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null) as MyWorkflowItem;
            Assert.IsNotNull(workflowItem);

            var workflowResolver = this.kernel.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            Assert.IsNotNull(workflowResolver);
            var workflow = workflowResolver.Resolve(workflowItem.WorkflowName);
            Assert.IsNotNull(workflow);

            var resStartTransition = workflow.TransitionTo(workflowItem, "Start", this.kernel);
            Assert.IsTrue(resStartTransition.IsSuccess);

            var resNeedMoreInfoTransition = workflow.TransitionTo(workflowItem, "Need More Info", this.kernel);
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

            var workflowResolver = this.kernel.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;
            Assert.IsNotNull(workflowResolver);
            var workflow = workflowResolver.Resolve(workflowItem.WorkflowName);
            Assert.IsNotNull(workflow);

            var resTransition1 = workflow.TransitionTo(workflowItem, "Create Employee Records", this.kernel);
            Assert.IsTrue(resTransition1.IsSuccess);
        }
    }
}
