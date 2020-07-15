﻿using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class GetPreservationRecordQuery : IRequest<Result<PreservationRecordDto>>, ITagQueryRequest
    {
        public GetPreservationRecordQuery(int tagId, int tagTagRequirementId, Guid preservationRecordGuid)
        {
            TagId = tagId;
            TagRequirementId = tagTagRequirementId;
            PreservationRecordGuid = preservationRecordGuid;
        }

        public int TagId { get; }
        public int TagRequirementId { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
