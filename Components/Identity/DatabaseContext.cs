using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Components.Identity
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext() { }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }

    public class ApplicationUser : IdentityUser
    {
        public string CountryCode { get; set; }
        public string MobileNumber { get; set; }
        public string DeviceToken { get; set; }
        public DateTime DateRegistration { get; set; }
    }
}