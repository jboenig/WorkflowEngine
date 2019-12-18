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
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.UnitTests.MockData;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Implementations;
using Headway.Dynamo.Repository;
using Headway.Dynamo.Repository.FlatFileRepo;

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
            this.kernel.Bind<IWorkflowByNameResolver>().To<FlatFileWorkflowRepo>()
                .WithConstructorArgument("filePath", "workflows.json");
            this.kernel.Bind<IObjectRepository<Workflow>>().To<FlatFileWorkflowRepo>()
                .WithConstructorArgument("filePath", "workflows.json");
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

            var workflowRepo = this.kernel.GetService(typeof(IObjectRepository<Workflow>)) as IObjectRepository<Workflow>;
            workflowRepo.Add(workflow);
            workflowRepo.SaveChanges();
            var workflowByNameResolver = this.kernel.GetService(typeof(IWorkflowByNameResolver)) as IWorkflowByNameResolver;

            var resWorkflow = workflowByNameResolver.Resolve("Test.Foo");
            Assert.IsNotNull(resWorkflow);
        }
    }
}
