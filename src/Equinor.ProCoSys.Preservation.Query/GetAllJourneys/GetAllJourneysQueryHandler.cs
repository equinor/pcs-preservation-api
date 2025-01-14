using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Query.ModeAggregate;
using Equinor.ProCoSys.Preservation.Query.ResponsibleAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetAllJourneys
{
    public class GetAllJourneysQueryHandler : IRequestHandler<GetAllJourneysQuery, Result<IEnumerable<JourneyDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetAllJourneysQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<IEnumerable<JourneyDto>>> Handle(GetAllJourneysQuery request,
            CancellationToken cancellationToken)
        {
            var journeys = await (from j in _context.QuerySet<Journey>().Include(j => j.Steps).Include(j => j.Project)
                select j).ToListAsync(cancellationToken);

            var modeIds = journeys.SelectMany(j => j.Steps).Select(x => x.ModeId).Distinct();
            var responsibleIds = journeys.SelectMany(j => j.Steps).Select(x => x.ResponsibleId).Distinct();

            var modes = await (from m in _context.QuerySet<Mode>()
                where modeIds.Contains(m.Id)
                select m).ToListAsync(cancellationToken);

            var responsibles = await (from r in _context.QuerySet<Responsible>()
                where responsibleIds.Contains(r.Id)
                select r).ToListAsync(cancellationToken);

            var journeyDtos =
                journeys.Where(j => (!j.IsVoided || request.IncludeVoided))
                    .Where(j => request.ProjectName == null || j.Project?.Name == null || j.Project.Name == request.ProjectName)
                    .Select(j => new JourneyDto(
                        j.Id,
                        j.Title,
                        j.IsVoided,
                        j.OrderedSteps().Where(s => !s.IsVoided || request.IncludeVoided)
                            .Select(s =>
                            {
                                var modeDto = modes
                                    .Where(m => m.Id == s.ModeId)
                                    .Select(m => new ModeDto(m.Id, m.Title, m.IsVoided, m.ForSupplier, true, m.RowVersion.ConvertToString()))
                                    .Single();
                                var responsibleDto = responsibles
                                    .Where(r => r.Id == s.ResponsibleId)
                                    .Select(r => new ResponsibleDto(r.Id, r.Code, r.Description,
                                        r.RowVersion.ConvertToString()))
                                    .Single();
                                return new StepDto(
                                    s.Id,
                                    s.Title,
                                    s.IsVoided,
                                    modeDto, 
                                    responsibleDto,
                                    s.AutoTransferMethod,
                                    s.RowVersion.ConvertToString());
                            }),
                        j.Project != null ? new JourneyDto.JourneyProjectDto(j.Project.Id, j.Project.Name, j.Project.Description) : null, 
                        j.RowVersion.ConvertToString()));

            return new SuccessResult<IEnumerable<JourneyDto>>(journeyDtos);
        }
    }
}
