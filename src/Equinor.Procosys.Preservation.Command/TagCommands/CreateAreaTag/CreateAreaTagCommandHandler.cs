﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Project;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag
{
    public class CreateAreaTagCommandHandler : IRequestHandler<CreateAreaTagCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IProjectApiService _projectApiService;
        private readonly IDisciplineApiService _disciplineApiService;
        private readonly IAreaApiService _areaApiService;

        public CreateAreaTagCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IProjectApiService projectApiService,
            IDisciplineApiService disciplineApiService,
            IAreaApiService areaApiService)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _projectApiService = projectApiService;
            _disciplineApiService = disciplineApiService;
            _areaApiService = areaApiService;
        }

        public async Task<Result<int>> Handle(CreateAreaTagCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByNameAsync(request.ProjectName);
            
            if (project == null)
            {
                project = await CreateProjectAsync(request.ProjectName);
                if (project == null)
                {
                    return new NotFoundResult<int>($"Project with name {request.ProjectName} not found");
                }
            }

            var areaTagToAdd = await CreateAreaTagAsync(project, request);

            if (!string.IsNullOrEmpty(request.AreaCode) && !await FillAreaDataAsync(areaTagToAdd, request.AreaCode))
            {
                return new NotFoundResult<int>($"Area with code {request.AreaCode} not found");
            }

            if (!await FillDisciplineDataAsync(areaTagToAdd, request.DisciplineCode))
            {
                return new NotFoundResult<int>($"Discipline with code {request.DisciplineCode} not found");
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(areaTagToAdd.Id);
        }

        private async Task<bool> FillDisciplineDataAsync(Tag tag, string disciplineCode)
        {
            var discipline = await _disciplineApiService.GetDiscipline(_plantProvider.Plant, disciplineCode);
            if (discipline == null)
            {
                return false;
            }
            tag.SetDiscipline(disciplineCode, discipline.Description);
            return true;
        }

        private async Task<bool> FillAreaDataAsync(Tag tag, string areaCode)
        {
            var area = await _areaApiService.GetArea(_plantProvider.Plant, areaCode);
            if (area == null)
            {
                return false;
            }
            tag.SetArea(areaCode, area.Description);
            return true;
        }

        private async Task<Tag> CreateAreaTagAsync(Project project, CreateAreaTagCommand request)
        {
            var reqDefIds = request.Requirements.Select(r => r.RequirementDefinitionId).ToList();
            var reqDefs = await _requirementTypeRepository.GetRequirementDefinitionsByIdsAsync(reqDefIds);

            var requirements = new List<TagRequirement>();
            foreach (var requirement in request.Requirements)
            {
                var reqDef = reqDefs.Single(rd => rd.Id == requirement.RequirementDefinitionId);
                requirements.Add(new TagRequirement(_plantProvider.Plant, requirement.IntervalWeeks, reqDef));
            }

            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);
            var tag = new Tag(
                _plantProvider.Plant,
                request.TagType,
                request.GetTagNo(),
                request.Description,
                step,
                requirements)
            {
                Remark = request.Remark,
                StorageArea = request.StorageArea
            };
            project.AddTag(tag);
            return tag;
        }

        private async Task<Project> CreateProjectAsync(string projectName)
        {
            var mainProject = await _projectApiService.GetProject(_plantProvider.Plant, projectName);
            if (mainProject == null)
            {
                return null;
            }

            var project = new Project(_plantProvider.Plant, projectName, mainProject.Description);
            _projectRepository.Add(project);
            return project;
        }
    }
}
