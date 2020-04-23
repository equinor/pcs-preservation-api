﻿using System;
using System.Security.Claims;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUser(); // todo rename to GetCurrentUserOid
        Guid? TryGetCurrentUserOid();
        bool IsCurrentUserAuthenticated();
        ClaimsPrincipal CurrentUser(); // todo rename to GetCurrentUser
    }
}
