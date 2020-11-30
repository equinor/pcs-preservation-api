﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTestsBase : TestBase
    {
        protected readonly string KnownAreaCode = "A";
        protected readonly string KnownDisciplineCode = "D";

        protected int InitialTagsCount;
        
        protected int TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
        protected int TagIdUnderTest_ForStandardTagWithInfoRequirement_Started;
        protected int TagIdUnderTest_ForStandardTagWithCbRequirement_Started;
        protected int TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
        protected int TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;

        protected int TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
        protected int TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments;

        protected JourneyDto JourneyWithTags;

        protected TestFile FileToBeUploaded = new TestFile("test file content", "file.txt");

        [TestInitialize]
        public async Task TestInitialize()
        {
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            InitialTagsCount = result.MaxAvailable;
            Assert.IsTrue(InitialTagsCount > 0, "Bad test setup: Didn't find any tags at startup");
            Assert.AreEqual(InitialTagsCount, result.Tags.Count);

            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(LibraryAdminClient(TestFactory.PlantWithAccess));
            JourneyWithTags = journeys.Single(j => j.Title == KnownTestData.JourneyWithTags);

            TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted
                = TestFactory.KnownTestData.TagId_ForStandardTagReadyForBulkPreserve_NotStarted;
            TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started
                = TestFactory.KnownTestData.TagId_ForStandardTagWithAttachmentRequirement_Started;
            TagIdUnderTest_ForStandardTagWithInfoRequirement_Started
                = TestFactory.KnownTestData.TagId_ForStandardTagWithInfoRequirement_Started;
            TagIdUnderTest_ForStandardTagWithCbRequirement_Started
                = TestFactory.KnownTestData.TagId_ForStandardTagWithCbRequirement_Started;
            TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted
                = TestFactory.KnownTestData.TagId_ForSiteAreaTagReadyForBulkPreserve_NotStarted;

            TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments
                = TestFactory.KnownTestData.TagId_ForStandardTagWithAttachmentsAndActionAttachments;
            TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments
                = TestFactory.KnownTestData.TagId_ForSiteAreaTagWithAttachmentsAndActionAttachments;

            TestFactory
                .DisciplineApiServiceMock
                .Setup(service => service.TryGetDisciplineAsync(TestFactory.PlantWithAccess, KnownDisciplineCode))
                .Returns(Task.FromResult(new ProcosysDiscipline
                {
                    Code = KnownDisciplineCode, Description = $"{KnownDisciplineCode} - Description"
                }));

            TestFactory
                .AreaApiServiceMock
                .Setup(service => service.TryGetAreaAsync(TestFactory.PlantWithAccess, KnownAreaCode))
                .Returns(Task.FromResult(new ProcosysArea
                {
                    Code = KnownAreaCode, Description = $"{KnownAreaCode} - Description"
                }));
        }

        protected async Task<int> CreateRequirementDefinitionAsync(HttpClient client)
        {
            var reqTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(client);
            var newReqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                client, reqTypes.First().Id, Guid.NewGuid().ToString());
            return newReqDefId;
        }
    }
}
