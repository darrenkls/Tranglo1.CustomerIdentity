using Microsoft.EntityFrameworkCore;
using System.Linq;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
    public class MigrationDbContext : BaseDbContext
    {

        public MigrationDbContext(DbContextOptions<MigrationDbContext> options)
            : base(options, null, null, null)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Seed();

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ExternalUserRoleStatus> ExternalUserRoleStatuses { get; set; }
        public DbSet<SystemEnvironment> SystemEnvironments { get; set; }
        public DbSet<AccountStatus> AccountStatus { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CustomerUser> CustomerUsers { get; set; }
        public DbSet<CustomerUserRegistration> CustomerUserRegistrations { get; set; }
        public DbSet<TrangloStaff> TrangloStaffs { get; set; }
        public DbSet<ApplicationUserClaim> ApplicationUserClaims { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<IndividualProfile> IndividualProfiles { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<ScreeningTypeList> ScreeningTypeList { get; set; }
        public DbSet<BusinessNature> BusinessNatures { get; set; }
        public DbSet<ReviewResult> ReviewResults { get; set; }
        public DbSet<WorkflowStatus> WorkflowStatuses { get; set; }
        public DbSet<KYCStatus> KYCStatuses { get; set; }
        public DbSet<IDType> IDTypes { get; set; }
        public DbSet<BusinessProfile> BusinessProfiles { get; set; }
        public DbSet<CustomerUserBusinessProfile> CustomerUserBusinessProfiles { get; set; }
        public DbSet<CustomerUserBusinessProfileRole> CustomerUserBusinessProfileRoles { get; set; }
        public DbSet<LicenseInformation> LicenseInformations { get; set; }
        public DbSet<COInformation> COInformations { get; set; }
        public DbSet<ScreeningInput> Screening { get; set; }
        public DbSet<Title> Titles { get; set; }
    }
}
