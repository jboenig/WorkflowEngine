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
using Headway.Dynamo.Exceptions;
using Headway.WorkflowEngine.UnitTests.MockData;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.Resolvers;
using Headway.WorkflowEngine.Implementations;
using Headway.Dynamo.Repository;

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
            workflowItem.PrimaryKey = Guid.NewGuid();
            workflowItem.Info = "Hello world";

            var workflowItemRepo = this.kernel.GetService(typeof(IObjectRepository<WorkflowItem>)) as IObjectRepository<WorkflowItem>;

            workflowItemRepo.Add(workflowItem);
            workflowItemRepo.SaveChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateKeyException))]
        public void AddDupWorkflowItemTest1()
        {
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);
            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null) as MyWorkflowItem;
            Assert.IsNotNull(workflowItem);
            workflowItem.PrimaryKey = Guid.NewGuid();
            workflowItem.Info = "Hello world";

            var workflowItemRepo = this.kernel.GetService(typeof(IObjectRepository<WorkflowItem>)) as IObjectRepository<WorkflowItem>;

            workflowItemRepo.Add(workflowItem);
            workflowItemRepo.Add(workflowItem);
        }
    }
}
