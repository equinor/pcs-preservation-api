﻿using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTestsBase : TestBase
    {
        protected readonly string KnownAreaCode = "A";
        protected readonly string KnownDisciplineCode = "D";
        protected int TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
        protected int TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
        protected int TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
        protected int StepIdUnderTest;
        protected int StandardTagActionIdUnderTest;
        protected int SiteAreaTagActionIdUnderTest;
        protected int InitialTagsCount;
        protected int StandardTagAttachmentIdUnderTest;
        protected int SiteAreaTagAttachmentIdUnderTest;
        protected int StandardTagActionAttachmentIdUnderTest;
        protected int SiteAreaTagActionAttachmentIdUnderTest;

        protected TestFile FileToBeUploaded = new TestFile("test file content", "file.txt");

        [TestInitialize]
        public async Task TestInitialize()
        {
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            InitialTagsCount = result.MaxAvailable;
            Assert.IsTrue(InitialTagsCount > 0, "Didn't find any tags at startup. Bad test setup");
            Assert.AreEqual(InitialTagsCount, result.Tags.Count);
            TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted
                = TestFactory.KnownTestData.TagId_ForStandardTagReadyForBulkPreserve_NotStarted;
            TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started
                = TestFactory.KnownTestData.TagId_ForStandardTagWithAttachmentRequirement_Started;
            TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted
                = TestFactory.KnownTestData.TagId_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
            StepIdUnderTest = TestFactory.KnownTestData.StepIds.First();
            StandardTagActionIdUnderTest = TestFactory.KnownTestData.StandardTagActionIds.First();
            SiteAreaTagActionIdUnderTest = TestFactory.KnownTestData.SiteAreaTagActionIds.First();
            StandardTagAttachmentIdUnderTest = TestFactory.KnownTestData.StandardTagAttachmentIds.First();
            SiteAreaTagAttachmentIdUnderTest = TestFactory.KnownTestData.SiteAreaTagAttachmentIds.First();
            StandardTagActionAttachmentIdUnderTest = TestFactory.KnownTestData.StandardTagActionAttachmentIds.First();
            SiteAreaTagActionAttachmentIdUnderTest = TestFactory.KnownTestData.SiteAreaTagActionAttachmentIds.First();

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
    }
}
