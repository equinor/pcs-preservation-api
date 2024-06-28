﻿namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IIntegrationEvent
{
    // The entity Guid the event was published for
    Guid Guid { get; }
    Guid ProCoSysGuid { get; }
    string Plant { get; }
    string ProjectName { get; }
}
