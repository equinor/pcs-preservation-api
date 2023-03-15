﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagVoidedEventHandler : INotificationHandler<TagVoidedEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public TagVoidedEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(TagVoidedEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.TagVoided;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
