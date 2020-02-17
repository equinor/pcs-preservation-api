﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagDetails
{
    [TestClass]
    public class GetTagDetailsQueryHandlerTests
    {
        readonly string _schema = "PCS$TEST";
        Mock<IEventDispatcher> _eventDispatcherMock;
        Mock<IPlantProvider> _plantProviderMock;

        [TestInitialize]
        public void Setup()
        {
            _eventDispatcherMock = new Mock<IEventDispatcher>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(x => x.Plant).Returns(_schema);
        }

        [TestMethod]
        public async Task Handler_ReturnsTagDetails()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using (var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var responsible = new Responsible(_schema, "Responsible");
                context.Responsibles.Add(responsible);
                context.SaveChanges();

                var mode = new Mode(_schema, "Mode");
                context.Modes.Add(mode);
                context.SaveChanges();

                var step = new Step(_schema, mode, context.Responsibles.First());
                var journey = new Journey(_schema, "Journey");
                journey.AddStep(step);
                context.Journeys.Add(journey);
                context.SaveChanges();

                var requirementDefinition = new RequirementDefinition(_schema, "RequirementDefinition", 2, 1);
                context.RequirementDefinitions.Add(requirementDefinition);
                context.SaveChanges();

                var tag = new Tag(_schema, "TagNo", "Description", "AreaCode", "Calloff", "DisciplineCode", "McPkgNo", "CommPkgNo", "PurchaseOrderNo", "Remark", "TagFunctionCode", step, new List<Requirement> { new Requirement(_schema, 2, requirementDefinition) });
                tag.StartPreservation(new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc));
                context.Tags.Add(tag);
                context.SaveChanges();
            }

            using (var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagDetailsQuery(1);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                Assert.AreEqual("AreaCode", dto.AreaCode);
                Assert.AreEqual("CommPkgNo", dto.CommPkgNo);
                Assert.AreEqual("Description", dto.Description);
                Assert.AreEqual(1, dto.Id);
                Assert.AreEqual("Journey", dto.JourneyTitle);
                Assert.AreEqual("McPkgNo", dto.McPkgNo);
                Assert.AreEqual("Mode", dto.Mode);
                Assert.AreEqual("PurchaseOrderNo", dto.PurchaseOrderNo);
                Assert.AreEqual("Responsible", dto.ResponsibleName);
                Assert.AreEqual(PreservationStatus.Active, dto.Status);
                Assert.AreEqual("TagNo", dto.TagNo);
                Assert.AreEqual(TagType.Standard, dto.TagType);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object);
            var query = new GetTagDetailsQuery(1);
            var dut = new GetTagDetailsQueryHandler(context);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }
    }
}
