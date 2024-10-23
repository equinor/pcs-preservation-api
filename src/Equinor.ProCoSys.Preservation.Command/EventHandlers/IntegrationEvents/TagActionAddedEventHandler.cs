﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagActionAddedEventHandler : INotificationHandler<TagActionAddedEvent>
{
    private readonly ICreateChildEventHelper<Tag, Action, ActionEvent> _createTagEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public TagActionAddedEventHandler(
        ICreateChildEventHelper<Tag, Action, ActionEvent> createTagEventHelper,
        IIntegrationEventPublisher integrationEventPublisher)
    {
        _createTagEventHelper = createTagEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task Handle(TagActionAddedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = await _createTagEventHelper.CreateEvent(notification.Entity, notification.Action);
        await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
