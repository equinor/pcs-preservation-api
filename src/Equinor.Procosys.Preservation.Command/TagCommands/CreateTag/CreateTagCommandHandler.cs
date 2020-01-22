﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;
using Equinor.Procosys.Preservation.MainApi.Tag;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagApiService _tagApiService;

        public CreateTagCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagApiService tagApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagApiService = tagApiService;
        }

        public async Task<Result<int>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            if (step == null)
            {
                return new NotFoundResult<int>(Strings.EntityNotFound(nameof(Step), request.StepId));
            }

            var requirements = new List<TagRequirement>();
            foreach (var requirement in request.Requirements)
            {
                var requirementDefinition =
                    await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirement.RequirementDefinitionId);

                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, requirementDefinition));
            }

            var tagDetails = await _tagApiService.GetTagDetails(_plantProvider.Plant, request.ProjectName, request.TagNo);

            var project = await _projectRepository.GetByNameAsync(request.ProjectName);
            if (project == null)
            {
                project = new Project(_plantProvider.Plant, request.ProjectName, tagDetails.ProjectDescription);
                _projectRepository.Add(project);
            }

            var tagToAdd = new Tag(
                _plantProvider.Plant,
                request.TagNo,
                tagDetails.Description,
                tagDetails.AreaCode,
                tagDetails.CallOffNo,
                tagDetails.DisciplineCode,
                tagDetails.McPkgNo,
                tagDetails.CommPkgNo,
                tagDetails.PurchaseOrderNo,
                tagDetails.TagFunctionCode,
                step,
                requirements);
            
            project.AddTag(tagToAdd);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(tagToAdd.Id);
        }
    }
}
