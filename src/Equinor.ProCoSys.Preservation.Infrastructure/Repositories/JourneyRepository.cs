using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(PreservationContext context)
            : base(context, context.Journeys, context.Journeys.Include(j => j.Steps).Include(j => j.Project))
        {
        }

        public Task<Step> GetStepByStepIdAsync(int stepId)
            => DefaultQuery
                .SelectMany(j => j.Steps)
                .SingleOrDefaultAsync(s => s.Id == stepId);


        public Task<List<Journey>> GetJourneysByStepIdsAsync(IEnumerable<int> stepIds)
            => DefaultQuery
                .Where(journey => journey.Steps.Any(s => stepIds.Contains(s.Id)))
                .ToListAsync();

        public Task<List<Journey>> GetJourneysWithAutoTransferStepsAsync(AutoTransferMethod autoTransferMethod)
            => DefaultQuery
                .Where(journey => journey.Steps.Any(s => s.AutoTransferMethod == autoTransferMethod))
                .ToListAsync();

        public void RemoveStep(Step step) => _context.Steps.Remove(step);

        public override void Remove(Journey journey)
        {
            foreach (var step in journey.Steps)
            {
                _context.Steps.Remove(step);
            }
            base.Remove(journey);
        }
    }
}
