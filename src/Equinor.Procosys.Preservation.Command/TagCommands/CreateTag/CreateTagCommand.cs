﻿using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommand : IRequest<Result<List<int>>>
    {
        public CreateTagCommand(
            IEnumerable<string> tagNos,
            string projectName,
            int stepId,
            IEnumerable<Requirement> requirements,
            string remark)
        {
            TagNos = tagNos ?? new List<string>();
            ProjectName = projectName;
            StepId = stepId;
            Requirements = requirements ?? new List<Requirement>();
            Remark = remark;
        }

        public IEnumerable<string> TagNos { get; }
        public string ProjectName { get; }
        public int StepId { get; }
        public IEnumerable<Requirement> Requirements { get; }
        public string Remark { get; }
    }
}
