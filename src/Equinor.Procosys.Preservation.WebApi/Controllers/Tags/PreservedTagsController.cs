﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Command.TagCommands.SetStep;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Query.ProjectAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    /// <summary>
    /// Handles requests that deal with preservation tags
    /// </summary>
    [ApiController]
    [Route("Tags/Preserved")]
    public class PreservedTagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PreservedTagsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTagsInProject([FromQuery] string projectName)    
        {
            var result = await _mediator.Send(new GetAllTagsInProjectQuery(projectName));
            return this.FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateTag([FromBody] CreateTagDto dto)
        {
            var requirements = dto.Requirements?
                .Select(r =>
                    new Requirement(r.RequirementDefinitionId, r.IntervalWeeks));
            var result = await _mediator.Send(
                new CreateTagCommand(
                    dto.TagNos,
                    dto.ProjectName,
                    dto.StepId,
                    requirements,
                    dto.Remark));
            return this.FromResult(result);
        }

        [HttpPut("{id}/SetStep")]
        public async Task<ActionResult> SetStep([FromRoute] int id, [FromBody] SetStepDto dto)
        {
            var result = await _mediator.Send(new SetStepCommand(id, dto.StepId));
            return this.FromResult(result);
        }

        [HttpPut("{id}/StartPreservation")]
        public async Task<IActionResult> StartPreservation([FromRoute] int tagId)
        {
            var result = await _mediator.Send(new StartPreservationCommand(new List<int>{tagId}));
            return this.FromResult(result);
        }

        [HttpPut("StartPreservation")]
        public async Task<IActionResult> StartPreservation([FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new StartPreservationCommand(tagIds));
            return this.FromResult(result);
        }

        [HttpPut("{id}/Preserve")]
        public async Task<IActionResult> Preserve([FromRoute] int tagId)
        {
            var result = await _mediator.Send(new PreserveCommand(new List<int>{tagId}, false));
            return this.FromResult(result);
        }

        [HttpPut("Preserve")]
        public async Task<IActionResult> Preserve([FromBody] List<int> tagIds)
        {
            var result = await _mediator.Send(new PreserveCommand(tagIds, true));
            return this.FromResult(result);
        }
    }
}
