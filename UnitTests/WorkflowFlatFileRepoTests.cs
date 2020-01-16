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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Implementations;
using Headway.WorkflowEngine.Repository;

namespace WorkflowEngine.UnitTests
{
    [TestClass]
    public class WorkflowFlatFileRepoTests
    {
        private IKernel kernel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<IServiceProvider>().ToConstant(this.kernel);
            this.kernel.Bind<IWorkflowRepository>().To<FlatFileWorkflowRepo>();
            this.kernel.Bind<ISerializerConfigService>().To<StandardSerializerConfigService>();
            this.kernel.Bind<IMetadataProvider>().To<StandardMetadataProvider>();
        }

        [TestMethod]
        public void CreateAndSaveWorkflowTest1()
        {
            var workflow = new Workflow("Test.Foo");
            workflow.InitialState = new WorkflowState("Reviewing");
            workflow.States.Add(new WorkflowState("Complete"));
            workflow.InitialState.Transitions.Add(new WorkflowTransition("Approve", "Complete"));

            var workflowRepo = this.kernel.GetService(typeof(IWorkflowRepository)) as IWorkflowRepository;
            workflowRepo.Add(workflow);
            workflowRepo.SaveChanges();

            var resWorkflow = workflowRepo.Resolve("Test.Foo");
            Assert.IsNotNull(resWorkflow);
        }
    }
}
