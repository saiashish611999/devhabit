using devhabit.api.Entities;
using Microsoft.EntityFrameworkCore;

namespace devhabit.api.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
{
    // Add-Migration Add_Habits -o Migrations/Application
    public DbSet<Habit> Habits { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Application);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}


