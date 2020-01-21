﻿using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommand : IRequest<Result<int>>
    {
        public CreateTagCommand(string tagNo, string projectNo, int stepId, IEnumerable<Requirement> requirements)
        {
            TagNo = tagNo;
            ProjectNo = projectNo;
            StepId = stepId;
            Requirements = requirements ?? new List<Requirement>();
        }

        public string TagNo { get; }
        public string ProjectNo { get; }
        public int StepId { get; }
        public IEnumerable<Requirement> Requirements { get; }
    }
}
