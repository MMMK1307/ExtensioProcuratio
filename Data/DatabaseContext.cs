using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExtensioProcuratio.Data;

public class DatabaseContext : IdentityDbContext<ApplicationUser>
{
#nullable disable

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectModel> Project { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ProjectAssociatesModel> ProjectAssociates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}