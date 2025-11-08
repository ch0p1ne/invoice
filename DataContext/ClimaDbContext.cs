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

        // Junction tables for many-to-many relationship
        public DbSet<UserRole> UsersRoles { get; set; }
        public DbSet<FactureExamen> FacturesExamens { get; set; }
        private string sourceNameLocal= "pc-thomas";
        private string sourceNameProd= "DESKTOP-RVJHVJ";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer($"Data Source={sourceNameLocal}\\SQLEXPRESS;database=CLIMAG;Trusted_Connection=True;TrustServerCertificate=True");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Account_name)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasMany<Role>(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRole>();

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
        }
    }
}
