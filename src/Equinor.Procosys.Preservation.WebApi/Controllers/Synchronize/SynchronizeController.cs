﻿using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Synchronize
{
    [ApiController]
    [Route("Synchronize")]
    public class SynchronizeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SynchronizeController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.PRESERVATION_PLAN_CREATE)]
        [HttpPut("Projects")]
        public async Task<ActionResult> Projects(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant)
        {
            var result = await _mediator.Send(new SyncProjectsCommand());
            return this.FromResult(result);
        }
    }
}
