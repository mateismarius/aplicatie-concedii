using Microsoft.EntityFrameworkCore;
using Utilitar.Areas.Identity.Data;
using Utilitar.Converters;
using Utilitar.Models;

namespace Utilitar.AppContext
{
    public class UtilitiesContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public UtilitiesContext(DbContextOptions<UtilitiesContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        



        public DbSet<DayOffType>? DayOffTypes { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<FreeDay>? FreeDays { get; set; }
        public DbSet<FreeDay>? LegallDays { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<PCA>? PCurtiDeApels { get; set; }
        public DbSet<PTribunal>? PTribunals { get; set; }
        public DbSet<PJudecatorie>? PJudecatories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //DayOffType configuration

            modelBuilder.Entity<DayOffType>()
                    .Property(s => s.Name)
                    .HasMaxLength(50)
                    .IsRequired();

            modelBuilder.Entity<DayOffType>()
                    .Property(s => s.IsPaid)
                    .HasDefaultValue(true)
                    .IsRequired();

            // Employee configurations
            modelBuilder.Entity<Employee>()
                    .Property(s => s.FirstName)
                    .HasMaxLength(50)
                    .IsRequired();

            modelBuilder.Entity<Employee>()
                    .Property(s => s.LastName)
                    .HasMaxLength(50)
                    .IsRequired();

            modelBuilder.Entity<Employee>()
                    .Property(s => s.PermanentLocation)
                    .HasMaxLength(100)
                    .IsRequired();

            modelBuilder.Entity<Employee>()
                    .Property(s => s.IsDelegate)
                    .HasDefaultValue(false)
                    .IsRequired();

            modelBuilder.Entity<Employee>()
                    .Property(s => s.PermanentLocation)
                    .HasMaxLength(20)
                    .IsRequired();

            // FreeDay configurations
            modelBuilder.Entity<FreeDay>()
                    .Property(s => s.RequestNumber)
                    .HasMaxLength(50)
                    .IsRequired();

            modelBuilder.Entity<FreeDay>()
                    .HasOne<Employee>(s => s.Employee)
                    .WithMany(g => g.FreeDays)
                    .HasForeignKey(k => k.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FreeDay>()
                    .HasOne<DayOffType>(s => s.DayOffType)
                    .WithMany(g => g.FreeDays)
                    .HasForeignKey(k => k.DayOffTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Utilitar.Models.LegallDay>? LegallDay { get; set; }

        
    }
}
