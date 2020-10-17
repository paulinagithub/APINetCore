using System;
using Microsoft.EntityFrameworkCore;

namespace RestWebApiPumoxGmgH.Models
{
    public partial class CompanyDBContext : DbContext
    {
        public CompanyDBContext(DbContextOptions<CompanyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.CompanyName).IsRequired();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.Idcompany).HasColumnName("IDCompany");

                entity.Property(e => e.JobTitle).IsRequired();

                entity.Property(e => e.LastName).IsRequired();

                entity.HasOne(d => d.IdcompanyNavigation)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.Idcompany)
                    .HasConstraintName("FK__Employee__IDComp__398D8EEE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
