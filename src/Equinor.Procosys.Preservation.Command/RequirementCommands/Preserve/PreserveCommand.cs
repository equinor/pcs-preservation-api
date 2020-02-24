﻿using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>
    {
        public PreserveCommand(int tagId, int requirementId)
        {
            TagId = tagId;
            RequirementId = requirementId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
    }
}
