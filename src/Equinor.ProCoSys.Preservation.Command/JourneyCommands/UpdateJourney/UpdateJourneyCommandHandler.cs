using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateJourney
{
    public class UpdateJourneyCommandHandler : IRequestHandler<UpdateJourneyCommand, Result<string>>
    {
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectApiService _projectApiService;
        private readonly IPlantProvider _plantProvider;

        public UpdateJourneyCommandHandler(IJourneyRepository journeyRepository, IUnitOfWork unitOfWork, IProjectRepository projectRepository, IProjectApiService projectApiService, IPlantProvider plantProvider)
        {
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
            _projectApiService = projectApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<string>> Handle(UpdateJourneyCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);

            journey.Title = request.Title;
            journey.SetRowVersion(request.RowVersion);
            if((journey.Project == null || journey.Project.Name != request.ProjectName) && request.ProjectName != null)
            {
                var project = await _projectRepository.GetProjectOnlyByNameAsync(request.ProjectName);
                if (project == null)
                {
                    project = await ImportProjectAsync(request.ProjectName);
                    if (project == null)
                    {
                        return new NotFoundResult<string>($"Project with name {request.ProjectName} not found");
                    }
                }
                journey.Project = project;
            } 
            else if (request.ProjectName == null && journey.Project != null)
            {
                journey.Project = null;
            }   
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(journey.RowVersion.ConvertToString());
        }
        
        private async Task<Project> ImportProjectAsync(string projectName)
        {
            var mainProject = await _projectApiService.TryGetProjectAsync(_plantProvider.Plant, projectName);
            if (mainProject == null)
            {
                return null;
            }

            var project = new Project(_plantProvider.Plant, projectName, mainProject.Description, mainProject.ProCoSysGuid);
            _projectRepository.Add(project);
            return project;
        }
    }
}
