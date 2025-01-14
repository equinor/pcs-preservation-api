﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

public interface IDomainToIntegrationEventConverter<T> where T : IDomainEvent
{
    Task<IEnumerable<IIntegrationEvent>> Convert(T domainEvent);
}
