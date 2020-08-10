﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter;
using Equinor.Procosys.Preservation.Query.GetAllSavedFilters;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Persons
{
    [ApiController]
    [Route("Persons")]
    public class PersonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonsController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.PRESERVATION_CREATE)]
        [HttpPost("/SavedFilter")]
        public async Task<ActionResult<int>> CreateSavedFilter(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromBody] CreateSavedFilterDto dto)
        {
            var result = await _mediator.Send(new CreateSavedFilterCommand(dto.ProjectName, dto.Title, dto.Criteria, dto.DefaultFilter));
            return this.FromResult(result);
        }
         
        [Authorize(Roles = Permissions.PRESERVATION_READ)]
        [HttpGet("/SavedFilters")]
        public async Task<ActionResult<List<SavedFilterDto>>> GetAllSavedFilters(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant)
        {
            var result = await _mediator.Send(new GetAllSavedFiltersQuery());
            return this.FromResult(result);
        }
    }
}
