using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateJourneyEventHelper : ICreateEventHelper<Journey, JourneyEvent>
{
    private readonly IPersonRepository _personRepository;

    public CreateJourneyEventHelper(IPersonRepository personRepository) => _personRepository = personRepository;

    public async Task<JourneyEvent> CreateEvent(Journey entity)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new JourneyEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            Title = entity.Title,
            IsVoided = entity.IsVoided,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            Project = entity.Project,
        };
    }
}
