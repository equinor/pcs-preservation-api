﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Command.TagCommands.SetStep;
using Equinor.Procosys.Preservation.Query.TagAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    [ApiController]
    [Route("Tags")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            var result = await _mediator.Send(new AllTagsQuery());
            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTag([FromBody] CreateTagDto dto)
        {
            var result = await _mediator.Send(
                new CreateTagCommand(
                    dto.TagNo,
                    dto.ProjectNo,
                    dto.StepId,
                    dto.Requirements.Select(r =>
                        new Requirement(r.RequirementDefinitionId, r.Interval))));
            return this.FromResult(result);
        }

        [HttpPost("{id}/SetStep")]
        public async Task<IActionResult> SetStep([FromRoute] int id, [FromBody] SetStepDto dto)
        {
            var result = await _mediator.Send(new SetStepCommand(id, dto.StepId));
            return this.FromResult(result);
        }
    }
}
