﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTags;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTags
{
    [TestClass]
    public class GetTagsQueryHandlerTests : ReadOnlyTestsBase
    {
        private string _projectName = "PX";
        private string _journeyTitle1 = "J1";
        private string _journeyTitle2 = "J2";
        private string _mode1 = "M1";
        private string _resp1 = "R1";
        private string _mode2 = "M2";
        private string _resp2 = "R2";
        private string _reqType1Code = "ROT";
        private string _reqType2Code = "AREA";
        private int _intervalWeeks = 2;
        private GetTagsQuery _query;
        private string _stdTagPrefix = "StdTagNo";
        private string _siteTagPrefix = "SiteTagNo";
        private string _callOffPrefix = "CO";
        private string _disciplinePrefix = "DI";
        private string _mcPkgPrefix = "MC";
        private string _commPkgPrefix = "COMM";
        private string _poPrefix = "PO";
        private string _tagFunctionPrefix = "TF";
        private Sorting _sorting;
        private Filter _filter;
        private Paging _paging;
        private int _journeyId1;
        private int _step1Onjourney1Id;
        private int _reqType1Id;
        private int _resp1Id;
        private int _mode1Id;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider))
            {
                AddPerson(context, _currentUserOid, "Ole", "Lukkøye");

                var projectPx = AddProject(context, _projectName, "Project description");
                var projectAnother = AddProject(context, "Another", "Project description");

                var mode1 = AddMode(context, _mode1);
                _mode1Id = mode1.Id;
                var responsible1 = AddResponsible(context, _resp1);
                _resp1Id = responsible1.Id;
                var mode2 = AddMode(context, _mode2);
                var responsible2 = AddResponsible(context, _resp2);

                var journey1With2Steps = AddJourneyWithStep(context, _journeyTitle1, mode1, responsible1);
                _journeyId1 = journey1With2Steps.Id;
                var journey2With1Steps = AddJourneyWithStep(context, _journeyTitle2, mode1, responsible1);
                var step1OnJourney2 = journey2With1Steps.Steps.ElementAt(0);
                var step1OnJourney1 = journey1With2Steps.Steps.ElementAt(0);
                _step1Onjourney1Id = step1OnJourney1.Id;

                var step2OnJourney1 = new Step(TestPlant, mode2, responsible2);

                journey1With2Steps.AddStep(step2OnJourney1);
                context.SaveChanges();

                var reqType1 = AddRequirementTypeWith1DefWithoutField(context, _reqType1Code, "D1");
                _reqType1Id = reqType1.Id;
                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.Standard,
                        $"{_stdTagPrefix}-{i}",
                        "Description",
                        "AreaCode",
                        $"{_callOffPrefix}-{i}",
                        $"{_disciplinePrefix}-{i}",
                        $"{_mcPkgPrefix}-{i}",
                        $"{_commPkgPrefix}-{i}",
                        $"{_poPrefix}-{i}",
                        "Remark",
                        $"{_tagFunctionPrefix}-{i}",
                        step1OnJourney1,
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType1.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectPx.AddTag(tag);
                }

                var reqType2 = AddRequirementTypeWith1DefWithoutField(context, _reqType2Code, "D2");
                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.SiteArea,
                        $"{_siteTagPrefix}-{i}",
                        "Description",
                        "AreaCode",
                        $"{_callOffPrefix}-{i}",
                        $"{_disciplinePrefix}-{i}",
                        $"{_mcPkgPrefix}-{i}",
                        $"{_commPkgPrefix}-{i}",
                        $"{_poPrefix}-{i}",
                        "Remark",
                        $"{_tagFunctionPrefix}-{i}",
                        step1OnJourney2,
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType2.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectPx.AddTag(tag);
                }

                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.Standard,
                        $"Another-{i}",
                        "Description",
                        "AreaCode",
                        "Calloff",
                        "DisciplineCode",
                        "McPkgNo",
                        "CommPkgNo",
                        "PurchaseOrderNo",
                        "Remark",
                        "TagFunctionCode",
                        journey1With2Steps.Steps.ElementAt(0),
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType1.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectAnother.AddTag(tag);
                }
                context.SaveChanges();

                _sorting = new Sorting(SortingDirection.Asc, SortingColumn.TagNo);
                _filter = new Filter(_projectName, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                _paging = new Paging(0, 50);
                _query = new GetTagsQuery(_sorting, _filter, _paging);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectNumber()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                // 30 tags added in setup, but 20 of them in project PX
                Assert.AreEqual(20, result.Data.MaxAvailable);
                Assert.AreEqual(20, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnPage()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var paging = new Paging(0, 5);
                var result = await dut.Handle(new GetTagsQuery(_sorting, _filter, paging), default);

                // 30 tags added in setup, but 20 of them in project PX
                Assert.AreEqual(20, result.Data.MaxAvailable);
                Assert.AreEqual(5, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectDto()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.TagNo.StartsWith(_stdTagPrefix));
                var tag = context.Tags.Single(t => t.Id == tagDto.Id);
                Assert.AreEqual(tag.AreaCode, tagDto.AreaCode);
                Assert.AreEqual(tag.Calloff, tagDto.CalloffNo);
                Assert.AreEqual(tag.CommPkgNo, tagDto.CommPkgNo);
                Assert.AreEqual(tag.DisciplineCode, tagDto.DisciplineCode);
                Assert.AreEqual(tag.Id, tagDto.Id);
                Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
                Assert.AreEqual(tag.McPkgNo, tagDto.McPkgNo);
                Assert.AreEqual(_mode1, tagDto.Mode);
                Assert.AreEqual(_resp1, tagDto.ResponsibleCode);
                Assert.AreEqual(tag.Description, tagDto.Description);
                Assert.AreEqual(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);

                Assert.AreEqual(tag.Status, tagDto.Status);
                Assert.AreEqual(tag.TagFunctionCode, tagDto.TagFunctionCode);
                Assert.AreEqual(tag.TagNo, tagDto.TagNo);

                Assert.AreEqual(_reqType1Code, tagDto.Requirements.First().RequirementTypeCode);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnDueInfo_WhenPreservationNotStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted);
                var requirementDto = tagDto.Requirements.First();

                Assert.IsFalse(requirementDto.NextDueTimeUtc.HasValue);
                Assert.IsFalse(requirementDto.NextDueWeeks.HasValue);
                Assert.IsNull(requirementDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.NotStarted, tagDto.Status);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnDueInfo_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active);
                var requirementDto = tagDto.Requirements.First();

                Assert.IsTrue(requirementDto.NextDueTimeUtc.HasValue);
                Assert.AreEqual(_intervalWeeks, requirementDto.NextDueWeeks);
                Assert.IsNotNull(requirementDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.Active, tagDto.Status);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnNoElements_WhenThereIsNoTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var filter = new Filter("NO", null, null, null, null, null, null, null, null, null, null, null, null, null, null);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnTagNo()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var tagNoStartsWith = $"{_stdTagPrefix}-0";
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, null, null, tagNoStartsWith, null, null, null, null);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(1, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.TagNo.StartsWith(tagNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnCommPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var commPkgNoStartsWith = $"{_commPkgPrefix}-0";
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, null, null, null, commPkgNoStartsWith, null, null, null);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(2, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.CommPkgNo.StartsWith(commPkgNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnMcPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var mcPkgNoStartsWith = $"{_mcPkgPrefix}-0";
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, null, null, null, null, mcPkgNoStartsWith, null, null);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(2, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.McPkgNo.StartsWith(mcPkgNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnPurchaseOrder()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var purchaseOrderNoStartsWith = $"{_poPrefix}-0";
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, null, null, null, null, null, purchaseOrderNoStartsWith, null);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(2, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.PurchaseOrderNo.StartsWith(purchaseOrderNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnCallOff()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var callOffStartsWith = $"{_callOffPrefix}-0";
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, null, null, null, null, null, null, callOffStartsWith);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(2, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.CalloffNo.StartsWith(callOffStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnPreservationStatus()
        {
            var filter = new Filter(_projectName, null, PreservationStatus.Active, null, null, null, null, null, null, null, null, null, null, null, null);
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }
            
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(20, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(PreservationStatus.Active, tag.Status);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnRequirementType()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var filter = new Filter(_projectName, null, null, new List<int>{_reqType1Id}, null, null, null, null, null, null, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(_reqType1Code, tag.Requirements.Single().RequirementTypeCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnDisciplineCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var diCode = $"{_disciplinePrefix}-0";
                var filter = new Filter(_projectName, null, null, null, new List<string>{diCode}, null, null, null, null, null, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(2, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(diCode, tag.DisciplineCode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnResponsible()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var filter = new Filter(_projectName, null, null, null, null, new List<int>{_resp1Id}, null, null, null, null, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(20, tags.Count); // 20 because responsible is the same on first step on both journeys
                foreach (var tag in tags)
                {
                    Assert.AreEqual(_resp1, tag.ResponsibleCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnTagFunctionCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var tfCode = $"{_tagFunctionPrefix}-0";
                var filter = new Filter(_projectName, null, null, null, null, null, new List<string>{tfCode}, null, null, null, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(2, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(tfCode, tag.TagFunctionCode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnMode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var filter = new Filter(_projectName, null, null, null, null, null, null, new List<int>{_mode1Id}, null, null, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(20, tags.Count); // 20 because mode is the same on first step on both journeys
                foreach (var tag in tags)
                {
                    Assert.AreEqual(_mode1, tag.Mode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnJourney()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, new List<int>{_journeyId1}, null, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnStep()
        {
            IEnumerable<int> tagIdsToTransfer;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, null, new List<int>{_step1Onjourney1Id}, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                tagIdsToTransfer = tags.Select(t => t.Id).Take(5);
            }

            StartPreservationOnAllTags();
            TransferTags(tagIdsToTransfer);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var filter = new Filter(_projectName, null, null, null, null, null, null, null, null, new List<int>{_step1Onjourney1Id}, null, null, null, null, null);
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(5, tags.Count);
            }
        }

        private void TransferTags(IEnumerable<int> tagIds)
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                foreach (var tagId in tagIds)
                {
                    var tag = context.Tags.Single(t => t.Id == tagId);
                    var journey = context.Journeys.Include(j => j.Steps).Single(j => j.Steps.Any(s => s.Id == tag.StepId));
                    tag.Transfer(journey);
                }
                context.SaveChanges();
            }
        }

        private void StartPreservationOnAllTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var tags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).ToList();
                tags.ForEach(t => t.StartPreservation());
                context.SaveChanges();
            }
        }
    }
}
