﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve
{
    public class BulkPreserveCommandHandler : IRequestHandler<BulkPreserveCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ITimeService _timeService;

        public BulkPreserveCommandHandler(
            IProjectRepository projectRepository,
            IPersonRepository personRepository,
            ITimeService timeService,
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider)
        {
            _projectRepository = projectRepository;
            _personRepository = personRepository;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<Unit>> Handle(BulkPreserveCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsByTagIdsAsync(request.TagIds);
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUser());

            foreach (var tag in tags)
            {
                tag.BulkPreserve(_timeService.GetCurrentTimeUtc(), currentUser);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
