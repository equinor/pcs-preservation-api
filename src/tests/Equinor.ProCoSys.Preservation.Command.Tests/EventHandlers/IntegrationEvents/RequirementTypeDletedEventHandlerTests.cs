﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class RequirementTypeDeletedEventHandlerTests
{
    private const string TestPlant = "PCS$PlantA";
    private RequirementTypeDeletedEventHandler _dut;
    private IIntegrationEvent _publishedEvent;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvent = e);

        _publishedEvent = null;

        _dut = new RequirementTypeDeletedEventHandler(mockPublisher.Object);
    }

    [TestMethod]
    public async Task Handle_ShouldSendIntegrationEvent()
    {
        // Arrange
        var requirementType = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
        var domainEvent = new DeletedEvent<RequirementType>(requirementType);

        // Act
        await _dut.Handle(domainEvent, default);

        // Assert
        Assert.IsInstanceOfType<RequirementTypeDeleteEvent>(_publishedEvent);
    }
}
