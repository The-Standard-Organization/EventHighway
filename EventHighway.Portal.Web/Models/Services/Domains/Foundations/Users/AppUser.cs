// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Identity;

namespace EventHighway.Portal.Web.Models.Services.Domains.Foundations.Users
{
    public class AppUser : IdentityUser<Guid>
    {
        // Soft-delete / disable flag (Spec Section 11.9): a disabled account is locked out and
        // hidden from normal use but retained, preferred over a hard delete.
        public bool IsDisabled { get; set; }
    }
}
