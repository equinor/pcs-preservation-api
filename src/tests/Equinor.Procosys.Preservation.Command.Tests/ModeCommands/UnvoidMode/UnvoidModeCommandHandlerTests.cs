﻿using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.UnvoidMode;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.UnvoidMode
{
    [TestClass]
    public class UnvoidModeCommandHandlerTests : CommandHandlerTestsBase
    {
        private Mode _mode;
        private UnvoidModeCommand _command;
        private UnvoidModeCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var modeId = 1;
            var modeRepositoryMock = new Mock<IModeRepository>();

            _mode = new Mode(TestPlant, "ModeTitle");
            _mode.Void();
            modeRepositoryMock
                .Setup(r => r.GetByIdAsync(modeId))
                .Returns(Task.FromResult(_mode));

            _command = new UnvoidModeCommand(modeId, _rowVersion);

            _dut = new UnvoidModeCommandHandler(
                modeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidModeCommand_ShouldUnvoidMode()
        {
            // Arrange
            Assert.IsTrue(_mode.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_mode.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidModeCommand_ShouldSetAndReturnRowVersion()
        {
            await _dut.Handle(_command, default);
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _mode.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUnvoidModeCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
