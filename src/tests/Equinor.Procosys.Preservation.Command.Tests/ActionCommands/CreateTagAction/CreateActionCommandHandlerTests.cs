﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.CreateAction
{
    [TestClass]
    public class CreateActionCommandHandlerTests : CommandHandlerTestsBase
    {
        private int _tagId = 2;
        private string _title = "ActionTitle";
        private readonly string _description = "ActionDescription";
        private DateTime _dueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);

        private TagRequirement _requirement;
        private Tag _tag;
        private CreateActionCommand _command;
        private CreateActionCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private int _rdId1 = 17;
        private int _intervalWeeks = 2;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var _rdMock = new Mock<RequirementDefinition>();
            _rdMock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _requirement = new TagRequirement(TestPlant, _intervalWeeks, _rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement> { _requirement });

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(_tag));

            _command = new CreateActionCommand(_tagId, _title, _description, _dueTimeUtc);

            _dut = new CreateActionCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object
                );
        }

        [TestMethod]
        public async Task HandlingCreateModeCommand_ShouldAddActionToTag()
        {
            Assert.IsTrue(_tag.Actions.Count == 0);

            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsTrue(_tag.Actions.Count == 1);
        }

        [TestMethod]
        public async Task HandlingCreateActionCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
