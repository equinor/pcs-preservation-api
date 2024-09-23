﻿using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagRequirementEventEventHelper : ICreateEventHelper<TagRequirement>
{
    private readonly IPersonRepository _personRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IRequirementTypeRepository _requirementTypeRepository;
    private readonly ITagProjectId _tagProjectId;

    public CreateTagRequirementEventEventHelper(IPersonRepository personRepository, IProjectRepository projectRepository, IRequirementTypeRepository requirementTypeRepository, ITagProjectId tagProjectId)
    {
        _personRepository = personRepository;
        _projectRepository = projectRepository;
        _requirementTypeRepository = requirementTypeRepository;
        _tagProjectId = tagProjectId;
    }

    public async Task<IIntegrationEvent> CreateEvent(TagRequirement entity)
    {
        var tag = await _projectRepository.GetTagByTagRequirementGuidAsync(entity.Guid);
        var projectId = await _tagProjectId.Retrieve(tag);
        var project = await _projectRepository.GetByIdAsync(projectId);
        var requirementDefinitions = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(entity.RequirementDefinitionId);

        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new TagRequirementEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            ProjectName = project.Name,
            IntervalWeeks = entity.IntervalWeeks,
            Usage = entity.Usage.ToString(),
            NextDueTimeUtc = entity.NextDueTimeUtc,
            IsVoided = entity.IsVoided,
            IsInUse = entity.IsInUse,
            RequirementDefinitionGuid = requirementDefinitions.Guid,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            ReadyToBePreserved = entity.ReadyToBePreserved
        };
    }
}
