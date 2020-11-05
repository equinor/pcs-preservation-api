﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using FluentValidation;

namespace Equinor.Procosys.Preservation.Command.TagCommands.DuplicateAreaTag
{
    public class DuplicateAreaTagCommandValidator : AbstractValidator<DuplicateAreaTagCommand>
    {
        // todo unit test
        public DuplicateAreaTagCommandValidator(ITagValidator tagValidator, IProjectValidator projectValidator)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(command => command)
                .MustAsync((command, token) => NotBeAClosedProjectForTagAsync(command.TagId, token))
                .WithMessage(command => $"Project for tag is closed! Tag={command.TagId}")
                .MustAsync((command, token) => BeAnExistingSourceTagAsync(command.TagId, token))
                .WithMessage(command => $"Tag doesn't exist! Tag={command.TagId}")
                .MustAsync((command, token) => NotBeAnExistingTagWithinProjectAsync(command.GetTagNo(), command.TagId, token))
                .WithMessage(command => $"Tag already exists in scope for project! Tag={command.GetTagNo()}")
                .MustAsync((command, token) => BeSameTagTypeAsSourceTagAsync(command.TagId, command.TagType, token))
                .WithMessage(command => $"Tag type must be same as for source tag! Tag={command.TagType}");

            async Task<bool> NotBeAClosedProjectForTagAsync(int tagId, CancellationToken token)
                => !await projectValidator.IsClosedForTagAsync(tagId, token);
        
            async Task<bool> BeAnExistingSourceTagAsync(int tagId, CancellationToken token)
                => await tagValidator.ExistsAsync(tagId, token);

            async Task<bool> NotBeAnExistingTagWithinProjectAsync(string tagNo, int tagId, CancellationToken token)
                => !await tagValidator.ExistsAsync(tagNo, tagId, token);

            async Task<bool> BeSameTagTypeAsSourceTagAsync(int tagId, TagType tagType, CancellationToken token)
                => !await tagValidator.VerifyTagTypeAsync(tagId, tagType, token);
        }
    }
}
