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
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

using Headway.Dynamo.Metadata;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Metadata.Reflection;
using Headway.WorkflowEngine.UnitTests.MockData;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Implementations;
using Headway.Dynamo.Repository;
using Headway.Dynamo.Repository.FlatFileRepo;

namespace WorkflowEngine.UnitTests
{
    [TestClass]
    public class WorkflowItemRepoTests
    {
        private IKernel kernel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<IServiceProvider>().ToConstant(this.kernel);
            this.kernel.Bind<IWorkflowByNameResolver>().To<JsonResourceWorkflowByNameResolver>().
                WithConstructorArgument("svcProvider", this.kernel).
                WithConstructorArgument("assembly", this.GetType().Assembly);
            this.kernel.Bind<IWorkflowItemTemplateResolver>().To<JsonResourceWorkflowItemTemplateResolver>().
                WithConstructorArgument("svcProvider", this.kernel).
                WithConstructorArgument("assembly", this.GetType().Assembly);
            this.kernel.Bind<ISerializerConfigService>().To<StandardSerializerConfigService>();
            this.kernel.Bind<IMetadataProvider>().To<StandardMetadataProvider>();
            this.kernel.Bind<IObjectRepository<WorkflowItem>>()
                .To<FlatFileWorkflowItemRepo>()
                .WithConstructorArgument("filePath", "workflowitems.json");
        }

        [TestMethod]
        public void CreateAndSaveWorkflowItemTest1()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);
            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null) as MyWorkflowItem;
            Assert.IsNotNull(workflowItem);
            workflowItem.Info = "Hello world";

            var workflowItemRepo = this.kernel.GetService(typeof(IObjectRepository<WorkflowItem>)) as IObjectRepository<WorkflowItem>;

            workflowItemRepo.Add(workflowItem);
            workflowItemRepo.SaveChanges();
        }
    }
}
