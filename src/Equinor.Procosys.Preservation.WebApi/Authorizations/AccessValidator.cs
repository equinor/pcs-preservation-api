﻿using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query;
using MediatR;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class AccessValidator : IAccessValidator
    {
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly IContentRestrictionsChecker _contentRestrictionsChecker;
        private readonly IProjectHelper _projectHelper;

        public AccessValidator(
            IProjectAccessChecker projectAccessChecker,
            IProjectHelper projectHelper,
            IContentRestrictionsChecker contentRestrictionsChecker)
        {
            _projectAccessChecker = projectAccessChecker;
            _projectHelper = projectHelper;
            _contentRestrictionsChecker = contentRestrictionsChecker;
        }

        public async Task<bool> ValidateAsync<TRequest>(TRequest request) where TRequest : IBaseRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request is IProjectRequest projectRequest)
            {
                return HasCurrentUserAccessToProject(projectRequest);
            }

            if (request is ITagCommandRequest tagCommandRequest)
            {
                return await HasCurrentUserAccessToProjectAsync(tagCommandRequest);
            }

            if (request is ITagQueryRequest tagQueryRequest)
            {
                return await HasCurrentUserAccessToProjectAsync(tagQueryRequest) && 
                    await HasCurrentUserAccessToContentAsync(tagQueryRequest);
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToContentAsync(ITagQueryRequest tagQueryRequest)
        {
            if (_contentRestrictionsChecker.HasCurrentUserAnyRestrictions())
            {
                var responsibleCode = "todo"; // Todo
                return _contentRestrictionsChecker.HasCurrentUserExplicitAccessToContent(responsibleCode);
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToProjectAsync(ITagCommandRequest tagCommandRequest)
            => await HasCurrentUserAccessToProjectAsync(tagCommandRequest.TagId);

        private async Task<bool> HasCurrentUserAccessToProjectAsync(ITagQueryRequest tagQueryRequest)
            => await HasCurrentUserAccessToProjectAsync(tagQueryRequest.TagId);

        private async Task<bool> HasCurrentUserAccessToProjectAsync(int tagId)
        {
            var projectName = await _projectHelper.GetProjectNameFromTagIdAsync(tagId);
            return _projectAccessChecker.HasCurrentUserAccessToProject(projectName);
        }

        private bool HasCurrentUserAccessToProject(IProjectRequest projectRequest)
            => _projectAccessChecker.HasCurrentUserAccessToProject(projectRequest.ProjectName);
    }
}
