using ExtensioProcuratio.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExtensioProcuratio.Data;

public class EpDbContext : IdentityDbContext<ApplicationUser>
{
    public EpDbContext(DbContextOptions<EpDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
