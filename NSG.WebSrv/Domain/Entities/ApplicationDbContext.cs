using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NSG.WebSrv.Domain.Entities
{
    public class ApplicationDbContext :
        IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>, IDb_Context
    {
        public object _options;
        // No database provider has been configured for this DbContext. A provider can be configured by overriding the DbContext.OnConfiguring method or by using AddDbContext on the application service provider. If AddDbContext is used, then also ensure that your DbContext type accepts a DbContextOptions<TContext> object in its constructor and passes it to the base constructor for DbContext.
        // solution:  : base(options)
        // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.0#the-identity-model
        //
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _options = options;
        }
        //
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //
            modelBuilder.Entity<ApplicationUser>((item) =>
            {
                item.Property(u => u.Id).HasMaxLength(128);
                item.HasMany(u => u.UserServers).WithOne(u => u.User)
                    .HasForeignKey(u => u.Id).OnDelete(DeleteBehavior.Cascade);
                item.HasOne(u => u.Company).WithMany(c => c.Users)
                    .HasForeignKey(u => u.CompanyId).OnDelete(DeleteBehavior.Restrict);
                // Each User can have many UserLogins
                item.HasMany(e => e.Logins).WithOne()
                    .HasForeignKey(ul => ul.UserId).IsRequired();
                // Each User can have many UserTokens
                item.HasMany(e => e.Tokens).WithOne()
                    .HasForeignKey(ut => ut.UserId).IsRequired();
            });
            //
            modelBuilder.Entity<ApplicationRole>((item) =>
            {
                item.Property(u => u.Id).HasMaxLength(128);
                // Each Role can have many entries in the UserRole join table
                item.HasMany(e => e.UserRoles).WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId).IsRequired();
            });
            //
            // ApplicationUserRole
            // InvalidOperationException: The entity type 'IdentityUserRole<string>' requires a primary key to be defined.
            // Microsoft.EntityFrameworkCore.Infrastructure.ModelValidator.ValidateNonNullPrimaryKeys(IModel model)
            modelBuilder.Entity<ApplicationUserRole>((item) =>
            {
                item.Property(ur => ur.UserId).HasMaxLength(128);
                item.Property(ur => ur.RoleId).HasMaxLength(128);
                item.HasKey(ur => new { ur.UserId, ur.RoleId });
                item.HasOne(ur => ur.Role).WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId).IsRequired();
                item.HasOne(ur => ur.User).WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId).IsRequired();
            });
            // ApplicationUserClaims
            modelBuilder.Entity<IdentityUserClaim<int>>()
                .Property(u => u.UserId).HasMaxLength(128);
            // ApplicationUserLogins
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .Property(u => u.UserId).HasMaxLength(128);
            // ApplicationUserTokens
            modelBuilder.Entity<IdentityUserToken<string>>()
                .Property(u => u.UserId).HasMaxLength(128);
            // AspNetRoleClaims
            modelBuilder.Entity<IdentityRoleClaim<int>>()
                .Property(u => u.RoleId).HasMaxLength(128);
            //
            modelBuilder.Entity<ApplicationUserServer>((item) =>
            {
                item.Property(us => us.Id).HasMaxLength(128);
                item.HasKey(us => new { us.Id, us.ServerId });
                item.HasOne<Server>(srv => srv.Server)
                    .WithMany(s => s.UserServers).HasForeignKey(srv => srv.ServerId).IsRequired();
                item.HasOne<ApplicationUser>(usr => usr.User)
                    .WithMany(u => u.UserServers).HasForeignKey(usr => usr.Id).IsRequired();
            });
            //
            modelBuilder.Entity<Company>((item) =>
            {
                item.HasKey(c => c.CompanyId);
                item.HasMany(u => u.Servers).WithOne(s => s.Company).HasForeignKey(s => s.CompanyId);
                item.HasIndex(c => c.CompanyShortName).IsUnique()
                    .HasName("Idx_Companies_ShortName");
            });
            //
            modelBuilder.Entity<Server>((item) =>
            {
                item.HasKey(s => s.ServerId);
                item.HasMany(u => u.UserServers).WithOne(u => u.Server).HasForeignKey(u => u.ServerId);
                item.HasOne(c => c.Company).WithMany(s => s.Servers)
                    .HasForeignKey(s => s.CompanyId).OnDelete(DeleteBehavior.Restrict);
                // index
                item.HasIndex(s => s.ServerShortName).IsUnique()
                    .HasName("Idx_AspNetServers_ShortName");
            });
            //
            modelBuilder.Entity<IncidentType>((item) =>
            {
                item.HasKey(it => it.IncidentTypeId);
                item.HasMany(it => it.NetworkLogs).WithOne(l => l.IncidentType)
                    .HasForeignKey(it => it.IncidentTypeId).OnDelete(DeleteBehavior.Restrict);
                // index
                item.HasIndex(i => i.IncidentTypeShortDesc)
                    .IsUnique().HasName("Idx_IncidentType_ShortDesc");
            });
            //
            modelBuilder.Entity<NetworkLog>((item) =>
            {
                item.HasKey(nl => nl.NetworkLogId);
                item.HasOne(nl => nl.IncidentType).WithMany(it => it.NetworkLogs)
                    .HasForeignKey(nl => nl.IncidentTypeId).OnDelete(DeleteBehavior.Restrict);
                item.HasOne(nl => nl.Server).WithMany(s => s.NetworkLogs)
                    .HasForeignKey(nl => nl.ServerId).OnDelete(DeleteBehavior.Restrict);
            });
            //
            modelBuilder.Entity<NIC>((item) =>
            {
                item.HasKey(n => n.NIC_Id);
                item.HasMany(n => n.Incidents).WithOne(i => i.NIC)
                    .HasForeignKey(n => n.NIC_Id).OnDelete(DeleteBehavior.Restrict);
            });
            //
            modelBuilder.Entity<EmailTemplate>((item) =>
            {
                item.HasKey(et => new { et.CompanyId, et.IncidentTypeId });
                // Company Company { get; set; }
                item.HasOne(nl => nl.Company).WithMany(c => c.EmailTemplates)
                    .HasForeignKey(nl => nl.CompanyId).OnDelete(DeleteBehavior.Restrict);
                item.HasOne(nl => nl.IncidentType).WithMany(et => et.EmailTemplates)
                    .HasForeignKey(nl => nl.IncidentTypeId).OnDelete(DeleteBehavior.Restrict);
            });
            //
            modelBuilder.Entity<Incident>((item) =>
            {
                item.HasKey(i => i.IncidentId);
                item.HasOne(i => i.NIC).WithMany(n => n.Incidents)
                    .HasForeignKey(i => i.NIC_Id).OnDelete(DeleteBehavior.Restrict);
                item.HasOne(i => i.Server).WithMany(s => s.Incidents)
                    .HasForeignKey(i => i.ServerId).OnDelete(DeleteBehavior.Restrict);
            });
            //
            modelBuilder.Entity<NoteType>((item) =>
            {
                item.HasKey(nt => nt.NoteTypeId);
                item.HasMany(nt => nt.IncidentNotes).WithOne(n => n.NoteType)
                    .HasForeignKey(nt => nt.IncidentNoteId).OnDelete(DeleteBehavior.Restrict);
                // index
                item.HasIndex(nt => nt.NoteTypeShortDesc)
                    .IsUnique().HasName("Idx_NoteType_ShortDesc");
            });
            //
            modelBuilder.Entity<IncidentNote>((item) =>
            {
                item.HasKey(n => n.IncidentNoteId);
                item.HasOne(n => n.NoteType).WithMany(nt => nt.IncidentNotes)
                    .HasForeignKey(n => n.NoteTypeId).OnDelete(DeleteBehavior.Restrict);
            });
            //
            modelBuilder.Entity<IncidentIncidentNote>((item) =>
            {
                item.HasKey(iin => new { iin.IncidentId, iin.IncidentNoteId });
                item.HasOne(iin => iin.IncidentNote).WithMany(n => n.IncidentIncidentNotes)
                    .HasForeignKey(iin => iin.IncidentNoteId).OnDelete(DeleteBehavior.Cascade);
                item.HasOne(iin => iin.Incident).WithMany(i => i.IncidentIncidentNotes)
                    .HasForeignKey(iin => iin.IncidentId).OnDelete(DeleteBehavior.Cascade);
            });
            //
        }
        //
        // public virtual DbSet<ApplicationRole> Roles { get; set; }
        // public virtual DbSet<ApplicationUserRole> UserRoles { get; set; }
        // support
        public virtual DbSet<LogData> Logs { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<ApplicationUserServer> UserServers { get; set; }
        // types
        public virtual DbSet<IncidentType> IncidentTypes { get; set; }
        public virtual DbSet<NIC> NICs { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<NoteType> NoteTypes { get; set; }
        // incidents
        public virtual DbSet<NetworkLog> NetworkLogs { get; set; }
        public virtual DbSet<Incident> Incidents { get; set; }
        public virtual DbSet<IncidentNote> IncidentNotes { get; set; }
        public virtual DbSet<IncidentIncidentNote> IncidentIncidentNotes { get; set; }
        //
    }
}
