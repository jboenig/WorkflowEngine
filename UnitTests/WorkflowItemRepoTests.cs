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
            this.kernel.Bind<IWorkflowItemTypeResolver>().To<JsonResourceWorkflowItemTypeResolver>().
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
            var metadataProvider = this.kernel.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            var workflowItemTemplateResolver = this.kernel.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);
            var workflowItem = workflowItemTemplate.CreateInstance(this.kernel, null);
            Assert.IsNotNull(workflowItem);
            Assert.AreEqual(workflowItem.ItemType.DisplayName, "Issue");
            Assert.AreEqual(workflowItem.ItemType.Abbreviation, "ISS");

            var workflowItemRepo = this.kernel.GetService(typeof(IObjectRepository<WorkflowItem>)) as IObjectRepository<WorkflowItem>;

            workflowItemRepo.Add(workflowItem);
            workflowItemRepo.SaveChanges();
        }
    }
}
