using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using invoice.Models;

namespace invoice.Context
{
    public class ClimaDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<FactureAvoir> FactureAvoirs { get; set; }
        public DbSet<Examen> Examens { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medecin> Medecins { get; set; }
        public DbSet<GreetingMessage> GreetingMessages { get; set; }
        public DbSet<Assurance> Assurances { get; set; }
        public DbSet<PrixHomologue> PrixHomologues { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        // Junction tables for many-to-many relationship
        public DbSet<FactureExamen> FacturesExamens { get; set; }
        public DbSet<FactureConsultation> FacturesConsultations { get; set; }
        public DbSet<RolePermission> RolesPermissions { get; set; }
        private string sourceNameLocal= "pc-thomas";
        private string sourceNameProd= "DESKTOP-RVJHVJS";
        private string sourceNameProd2= "DESKTOP-HPOEPLL";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer($"Data Source={sourceNameProd2}\\sqlexpress;database=CLIMAG;Trusted_Connection=True;TrustServerCertificate=True");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Account_name)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasOne<Role>(u => u.Role)
                .WithMany(r => r.Users);

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Role_name)
                .IsUnique();

            modelBuilder.Entity<Facture>()
                .HasIndex(f => f.Reference)
                .IsUnique();
            modelBuilder.Entity<Facture>()
                .HasOne(f => f.User)
                .WithMany(u => u.Factures);
            modelBuilder.Entity<Facture>()
                .HasOne(f => f.Patient)
                .WithMany(p => p.Factures);
            modelBuilder.Entity<Facture>()
                .HasMany<Examen>(f => f.Examens)
                .WithMany(e => e.Factures)
                .UsingEntity<FactureExamen>();
            modelBuilder.Entity<Facture>()
                .HasMany<Consultation>(f => f.Consultations)
                .WithMany(c => c.Factures)
                .UsingEntity<FactureConsultation>();
            modelBuilder.Entity<Facture>()
                .Property(f => f.Css)
                .HasDefaultValue(0.00);
            modelBuilder.Entity<Facture>()
                .Property(f => f.ESCOMPT)
                .HasDefaultValue(0.00);

            modelBuilder.Entity<FactureAvoir>()
                .HasIndex(fa => fa.Reference)
                .IsUnique();
            modelBuilder.Entity<FactureAvoir>()
                .HasOne(fa => fa.Facture)
                .WithOne(f => f.FactureAvoir);

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.AssuranceNumber)
                .IsUnique();
            modelBuilder.Entity<Patient>()
                .HasOne<Assurance>(p => p.Assurance)
                .WithMany(a => a.Patients);

            modelBuilder.Entity<Assurance>()
                .HasIndex(a => a.Compagny)
                .IsUnique();

            modelBuilder.Entity<FactureConsultation>()
                .HasOne(fc => fc.Medecin)
                .WithMany(m => m.FacturesConsultations);

            // Configure la table de jonction RolePermission
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
