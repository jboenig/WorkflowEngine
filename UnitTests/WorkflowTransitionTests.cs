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

using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task TransitionTest1()
        {
            var workflowItemFactory = this.kernel.GetService(typeof(IWorkflowItemFactory)) as IWorkflowItemFactory;

            var workflowItem = workflowItemFactory.CreateWorkflowItem("MockData.Templates.Test1Template", null);
            Assert.IsNotNull(workflowItem);

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var resExec = await workflowExecutionSvc.StartWorkflow(workflowItem);
            Assert.IsTrue(resExec.IsSuccess);

            var resTransition = await workflowExecutionSvc.TransitionTo(workflowItem, "Start");
            Assert.IsTrue(resTransition.IsSuccess);
        }

        [TestMethod]
        public void GetAllTransitionsTest()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);

            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null) as MyWorkflowItem;
            Assert.IsNotNull(workflowItem);
            workflowItem.CurrentState = "Reviewing";

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var allTransitions = workflowExecutionSvc.GetAllTransitions(workflowItem);
            Assert.AreEqual(allTransitions.Count(), 2);
        }

        [TestMethod]
        public void GetAllTransitionNamesTest()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);

            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null) as MyWorkflowItem;
            Assert.IsNotNull(workflowItem);
            workflowItem.CurrentState = "Reviewing";

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var allTransitionNames = workflowExecutionSvc.GetAllTransitionNames(workflowItem).ToList();
            Assert.AreEqual(allTransitionNames.Count, 2);
        }

        [TestMethod]
        public async Task TransitionToWhenTest()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);

            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null) as MyWorkflowItem;
            Assert.IsNotNull(workflowItem);

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var resStartWorkflow = await workflowExecutionSvc.StartWorkflow(workflowItem, "MockData.Workflows.Test1Workflow");
            Assert.IsTrue(resStartWorkflow.IsSuccess);

            var resStartTransition = await workflowExecutionSvc.TransitionTo(workflowItem, "Start");
            Assert.IsTrue(resStartTransition.IsSuccess);

            var resNeedMoreInfoTransition = await workflowExecutionSvc.TransitionTo(workflowItem, "Need More Info");
            Assert.IsTrue(resNeedMoreInfoTransition.IsSuccess);

            Assert.AreEqual(workflowItem.CurrentState, "Reviewing");
            Assert.AreEqual(workflowItem.Info, "Dude");
        }

        [TestMethod]
        public async Task TransitionOnboardingTest1()
        {
            var workflowItemFactory = this.kernel.GetService(typeof(IWorkflowItemFactory)) as IWorkflowItemFactory;

            var workflowItem = workflowItemFactory.CreateWorkflowItem("MockData.Templates.NewEmployeeOnboardingTaskTemplate", null);
            Assert.IsNotNull(workflowItem);

            var workflowExecutionSvc = this.kernel.GetService(typeof(IWorkflowExecutionService)) as IWorkflowExecutionService;
            Assert.IsNotNull(workflowExecutionSvc);

            var resStartWorkflow = await workflowExecutionSvc.StartWorkflow(workflowItem);
            Assert.IsTrue(resStartWorkflow.IsSuccess);

            var resTransition1 = await workflowExecutionSvc.TransitionTo(workflowItem, "Create Employee Records");
            Assert.IsTrue(resTransition1.IsSuccess);
        }
    }
}
