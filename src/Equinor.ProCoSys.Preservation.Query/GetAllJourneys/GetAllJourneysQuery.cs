using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetAllJourneys
{
    public class GetAllJourneysQuery : IRequest<Result<IEnumerable<JourneyDto>>>
    {
        public GetAllJourneysQuery(bool includeVoided, string projectName) => (IncludeVoided, ProjectName) = (includeVoided, projectName);

        public bool IncludeVoided { get; }
        public string ProjectName { get; }
    }
}
