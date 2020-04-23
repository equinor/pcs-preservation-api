﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ContentRestrictionsChecker : IContentRestrictionsChecker
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public ContentRestrictionsChecker(ICurrentUserProvider currentUserProvider) => _currentUserProvider = currentUserProvider;

        public bool HasCurrentUserExplicitNoRestrictions()
        {
            var claimWithContentRestriction = GetContentRestrictionClaims(_currentUserProvider.CurrentUser().Claims);

            // the rule for saying that a user do not have any restriction, is that user has one and only one restriction with value %
            return claimWithContentRestriction.Count == 1 && HasContentRestrictionClaim(claimWithContentRestriction, ClaimsTransformation.NoRestrictions);
        }

        public bool HasCurrentUserExplicitAccessToContent(string responsibleCode)
        {
            if (string.IsNullOrEmpty(responsibleCode))
            {
                throw new ArgumentNullException(nameof(responsibleCode));
            }
            
            var claimWithContentRestriction = GetContentRestrictionClaims(_currentUserProvider.CurrentUser().Claims);
            return HasContentRestrictionClaim(claimWithContentRestriction, responsibleCode);
        }

        private bool HasContentRestrictionClaim(IEnumerable<Claim> claims, string responsibleCode)
        {
            var contentRestrictionClaimValue = ClaimsTransformation.GetContentRestrictionClaimValue(responsibleCode);
            return claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == contentRestrictionClaimValue);
        }

        private List<Claim> GetContentRestrictionClaims(IEnumerable<Claim> claims)
            => claims.Where(c =>
                    c.Type == ClaimTypes.UserData &&
                    c.Value.StartsWith(ClaimsTransformation.ContentRestrictionPrefix))
                .ToList();
    }
}
