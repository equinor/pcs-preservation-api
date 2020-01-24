﻿using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class AllTagsInProjectQuery : IRequest<Result<IEnumerable<TagDto>>>
    {
        public AllTagsInProjectQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
