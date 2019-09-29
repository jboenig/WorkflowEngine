using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Headway.Dynamo.Metadata;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Metadata.Reflection;
using Headway.WorkflowEngine.UnitTests.MockData;
using Headway.WorkflowEngine;
using Headway.WorkflowEngine.Resolvers;
using Headway.Dynamo.Repository.FlatFileRepo;

namespace WorkflowEngine.UnitTests
{
    [TestClass]
    public class WorkflowItemRepoTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceContainer = new ServiceContainer();
            var workflowByNameResolver = new WorkflowByNameResolver();
            serviceContainer.AddService(typeof(IWorkflowByNameResolver), workflowByNameResolver);
            serviceContainer.AddService(typeof(IObjectResolver<string, Workflow>), workflowByNameResolver);
            serviceContainer.AddService(typeof(IWorkflowItemTemplateResolver), new WorkflowItemTemplateResolver());
            serviceContainer.AddService(typeof(IWorkflowItemTypeResolver), new WorkflowItemTypeResolver());
            var converterService = new JsonConverterService();
            serviceContainer.AddService(typeof(IJsonConverterService), converterService);
            serviceContainer.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(converterService));
            serviceContainer.AddService(typeof(IMetadataProvider), new StandardMetadataProvider());
            this.serviceProvider = serviceContainer;
        }

        [TestMethod]
        public void CreateAndSaveWorkflowItemTest1()
        {
            var metadataProvider = this.serviceProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
            var workflowItemTemplateResolver = this.serviceProvider.GetService(typeof(IWorkflowItemTemplateResolver)) as IWorkflowItemTemplateResolver;
            var workflowItemTemplate = workflowItemTemplateResolver.Resolve("MockData.Templates.Test1Template");
            Assert.IsNotNull(workflowItemTemplate);
            var workflowItem = workflowItemTemplate.CreateInstance(this.serviceProvider, null);
            Assert.IsNotNull(workflowItem);
            Assert.AreEqual(workflowItem.ItemType.DisplayName, "Issue");
            Assert.AreEqual(workflowItem.ItemType.Abbreviation, "ISS");

            var workflowItemTypeInfo = metadataProvider.GetDataType<ObjectType>(typeof(WorkflowItem));
            var repo = new FlatFileRepo<WorkflowItem>(
                workflowItemTypeInfo,
                "workflowitems.json",
                this.serviceProvider);

            repo.Add(workflowItem);
            repo.SaveChanges();
        }
    }
}
