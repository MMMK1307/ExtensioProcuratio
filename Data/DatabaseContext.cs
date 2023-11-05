using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        builder.Entity<ProjectModel>()
            .Property(e => e.Id)
            .HasConversion(
            projectId => projectId.Value,
            value => new ProjectId(value));

        builder.Entity<ProjectAssociatesModel>()
            .Property(e => e.ProjectId)
            .HasConversion(
            projectId => projectId.Value,
            value => new ProjectId(value));

        base.OnModelCreating(builder);
    }
}