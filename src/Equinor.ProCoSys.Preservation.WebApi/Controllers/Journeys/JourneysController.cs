using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.DeleteJourney;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.DeleteStep;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.DuplicateJourney;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.SwapSteps;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UnvoidJourney;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UnvoidStep;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.VoidJourney;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.VoidStep;
using Equinor.ProCoSys.Preservation.Query.GetAllJourneys;
using Equinor.ProCoSys.Preservation.Query.GetJourneyById;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceResult.ApiExtensions;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Journeys
{
    [ApiController]
    [Route("Journeys")]
    public class JourneysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JourneysController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet]
        public async Task<ActionResult<List<JourneyDto>>> GetJourneys(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromQuery] bool includeVoided = false,
            [FromQuery] string projectName = null)
        {
            var result = await _mediator.Send(new GetAllJourneysQuery(includeVoided, projectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_READ)]
        [HttpGet("{id}")]
        public async Task<ActionResult<JourneyDetailsDto>> GetJourney(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromQuery] bool includeVoided = false)
        {
            var result = await _mediator.Send(new GetJourneyByIdQuery(id, includeVoided));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost]
        public async Task<ActionResult<int>> AddJourney(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromBody] CreateJourneyDto dto)
        {
            var result = await _mediator.Send(new CreateJourneyCommand(dto.Title));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateJourney(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] UpdateJourneyDto dto)
        {
            var result = await _mediator.Send(new UpdateJourneyCommand(id, dto.Title, dto.RowVersion, dto.ProjectName));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPut("{id}/Duplicate")]
        public async Task<ActionResult<int>> DuplicateJourney(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id)
        {
            var result = await _mediator.Send(new DuplicateJourneyCommand(id));
            return this.FromResult(result);
        }
        
        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Void")]
        public async Task<ActionResult<string>> VoidJourney(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] VoidJourneyDto dto)
        {
            var result = await _mediator.Send(new VoidJourneyCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Unvoid")]
        public async Task<ActionResult<string>> UnvoidJourney(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] UnvoidJourneyDto dto)
        {
            var result = await _mediator.Send(new UnvoidJourneyCommand(id, dto.RowVersion));

            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_DELETE)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteJourney(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromBody] DeleteJourneyDto dto)
        {
            var result = await _mediator.Send(new DeleteJourneyCommand(id, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_CREATE)]
        [HttpPost("{id}/AddStep")]
        public async Task<ActionResult<int>> AddStep(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] CreateStepDto dto)
        {
            var result = await _mediator.Send(new CreateStepCommand(
                id,
                dto.Title,
                dto.ModeId,
                dto.ResponsibleCode,
                dto.AutoTransferMethod));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/Steps/{stepId}")]
        public async Task<ActionResult<string>> UpdateStep(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int stepId,
            [FromBody] UpdateStepDto dto)
        {
            var command = new UpdateStepCommand(
                id,
                stepId,
                dto.ModeId,
                dto.ResponsibleCode,
                dto.Title,
                dto.AutoTransferMethod,
                dto.RowVersion);
            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_DELETE)]
        [HttpDelete("{id}/Steps/{stepId}")]
        public async Task<ActionResult> DeleteStep(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            string plant,
            [FromRoute] int id,
            [FromRoute] int stepId,
            [FromBody] DeleteStepDto dto)
        {
            var result = await _mediator.Send(new DeleteStepCommand(id, stepId, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Steps/{stepId}/Void")]
        public async Task<ActionResult<string>> VoidStep(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int stepId,
            [FromBody] VoidStepDto dto)
        {
            var result = await _mediator.Send(new VoidStepCommand(id, stepId, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_VOIDUNVOID)]
        [HttpPut("{id}/Steps/{stepId}/Unvoid")]
        public async Task<ActionResult<string>> UnvoidStep(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromRoute] int stepId,
            [FromBody] UnvoidStepDto dto)
        {
            var result = await _mediator.Send(new UnvoidStepCommand(id, stepId, dto.RowVersion));
            return this.FromResult(result);
        }

        [Authorize(Roles = Permissions.LIBRARY_PRESERVATION_WRITE)]
        [HttpPut("{id}/Steps/SwapSteps")]
        public async Task<ActionResult<List<StepIdAndRowVersion>>> SwapSteps(
            [FromHeader(Name = CurrentPlantMiddleware.PlantHeader)]
            [Required]
            [StringLength(PlantEntityBase.PlantLengthMax, MinimumLength = PlantEntityBase.PlantLengthMin)]
            string plant,
            [FromRoute] int id,
            [FromBody] PairedStepIdWithRowVersionDto pairedStepsDto)
        {
            var command = new SwapStepsCommand(
                id, pairedStepsDto.StepA.Id, 
                pairedStepsDto.StepA.RowVersion, 
                pairedStepsDto.StepB.Id,
                pairedStepsDto.StepB.RowVersion
                );
            var result = await _mediator.Send(command);
            return this.FromResult(result);
        }
    }
}
