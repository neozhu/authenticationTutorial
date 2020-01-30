using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace identityserver.Models
{
    public class StoreDbContext:IdentityDbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options):base(options:options)
        {
        }
    }
}
