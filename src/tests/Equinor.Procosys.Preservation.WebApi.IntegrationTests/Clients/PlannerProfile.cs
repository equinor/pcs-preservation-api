﻿using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    // Authenticated client with necessary roles to Create and Update in Scope
    public static class PlannerProfile
    {
        public static Profile Tokens
            => new Profile
            {
                Oid = "00000000-0000-0000-0000-000000000002", 
                FullName = "Pernilla Planner"
            };

        public static IList<string> ProCoSysPermissions
        {
            get
            {
                var proCoSysPermissions = new List<string>
                {
                    Permissions.PRESERVATION_PLAN_CREATE,
                    Permissions.PRESERVATION_PLAN_DELETE,
                    Permissions.PRESERVATION_PLAN_VOIDUNVOID,
                    Permissions.PRESERVATION_PLAN_WRITE
                };
                proCoSysPermissions.AddRange(PreserverProfile.ProCoSysPermissions);
                return proCoSysPermissions;
            }
        }
    }
}