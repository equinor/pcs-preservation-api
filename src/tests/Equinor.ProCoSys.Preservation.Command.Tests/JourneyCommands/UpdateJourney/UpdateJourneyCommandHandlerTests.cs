using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.UpdateJourney
{
    [TestClass]
    public class UpdateJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "JourneyTitleOld";
        private readonly string _newTitle = "JourneyTitleNew";
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateJourneyCommand _command;
        private UpdateJourneyCommand _addProjectCommand;
        private UpdateJourneyCommand _removeProjectCommand;
        private UpdateJourneyCommandHandler _dut;
        private Journey _journey;
        private Journey _journeyWithProject;
        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var testJourneyId = 1;
            var testJourneyWithProjectId = 2;
            var testProjectName = "ProjectName";
            var journeyRepositoryMock = new Mock<IJourneyRepository>();
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var mainApiProjectServiceMock  = new Mock<IProjectApiService>();
            
            var project = new Project(TestPlant, testProjectName, "ProjectDescription", Guid.NewGuid());
            
            _journey = new Journey(TestPlant, _oldTitle);
            _journeyWithProject = new Journey(TestPlant, _oldTitle, project);
            
            journeyRepositoryMock.Setup(j => j.GetByIdAsync(testJourneyId))
                .Returns(Task.FromResult(_journey));
            journeyRepositoryMock.Setup(j => j.GetByIdAsync(testJourneyWithProjectId))
                .Returns(Task.FromResult(_journeyWithProject));
            
            projectRepositoryMock.Setup(p => p.GetProjectOnlyByNameAsync(testProjectName))
                .Returns(Task.FromResult(project));
            
            _command = new UpdateJourneyCommand(testJourneyId, _newTitle, _rowVersion);
            _addProjectCommand = new UpdateJourneyCommand(testJourneyId, _newTitle, _rowVersion, project.Name);
            _removeProjectCommand = new UpdateJourneyCommand(testJourneyWithProjectId, _newTitle, _rowVersion);

            _dut = new UpdateJourneyCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object, projectRepositoryMock.Object, mainApiProjectServiceMock.Object, PlantProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldUpdateJourney()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _journey.Title);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_newTitle, _journey.Title);
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _journey.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
        
        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldAddProject()
        {
            // Act
            await _dut.Handle(_addProjectCommand, default);

            // Assert
            Assert.IsNotNull(_journey.Project);
            Assert.AreEqual(_addProjectCommand.ProjectName, _journey.Project.Name);
        }
        
        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldRemoveProject()
        {
            // Act
            await _dut.Handle(_removeProjectCommand, default);
            
            // Assert
            Assert.IsNull(_journey.Project);
        }
    }
}
