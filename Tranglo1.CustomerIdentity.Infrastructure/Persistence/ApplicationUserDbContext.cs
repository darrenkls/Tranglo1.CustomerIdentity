using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.CustomerUserVerification;
using Tranglo1.CustomerIdentity.Domain.Entities.BusinessProfileAggregate.Verification;
using Tranglo1.CustomerIdentity.Domain.Entities.Meta;
using Tranglo1.CustomerIdentity.Domain.Entities.MFA;
using Tranglo1.CustomerIdentity.Infrastructure.EntityTypeConfiguration;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using UserType = Tranglo1.CustomerIdentity.Domain.Entities.UserType;

namespace Tranglo1.CustomerIdentity.Infrastructure.Persistence
{
	public class ApplicationUserDbContext : BaseDbContext//, IApplicationUserDbContext
	{
		public const string DEFAULT_SCHEMA = "dbo";
		public const string META_SCHEMA = "meta";
		public const string HISTORY_SCHEMA = "history";

		//entities
		public DbSet<Solution> Solutions { get; set; }
		public DbSet<UserType> UserTypes { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<AuthorityLevel> AuthorityLevels { get; set; }
		public DbSet<RoleStatus> RoleStatus { get; set; }
		public DbSet<ExternalUserRoleStatus> ExternalUserRoleStatuses { get; set; }
		public DbSet<SystemEnvironment> SystemEnvironments { get; set; }
		public DbSet<AccountStatus> AccountStatus { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<CustomerUserRegistration> CustomerUserRegistrations { get; set; }
		public DbSet<CustomerUser> CustomerUsers { get; set; }
		public DbSet<TrangloStaff> TrangloStaffs { get; set; }
		public DbSet<ApplicationUserClaim> ApplicationUserClaims { get; set; }
		public DbSet<Gender> Genders { get; set; }
		public DbSet<ActionOperation> ActionOperations { get; set; }
		public DbSet<IndividualProfile> IndividualProfiles { get; set; }
		public DbSet<Invitation> Invitations { get; set; }
        public DbSet<TrangloStaffAssignment> TrangloStaffAssignments { get; set; }
		public DbSet<CompanyUserAccountStatus> CompanyUserAccountStatus { get; set; }
		public DbSet<CompanyUserBlockStatus> CompanyUserBlockStatus { get; set; }
		public DbSet<ServicesOffered> ServicesOffered { get; set; }
		public DbSet<TrangloDepartment> TrangloDepartment { get; set; }
		public DbSet<TrangloEntity> TrangloEntity { get; set; }
		public DbSet<Domain.Entities.Environment> Environments { get; set; }
		public DbSet<TrangloStaffEntityAssignment> TrangloStaffEntityAssignment { get; set; }
		public DbSet<EntityType> EntityTypes { get; set; }
		public DbSet<RelationshipTieUp> RelationshipTieUps { get; set; }

		public DbSet<IncorporationCompanyType> IncorporationCompanyTypes { get; set; }

		public DbSet<CustomerType> CustomerTypes { get; set; }

		public DbSet<ServiceType>ServiceTypes { get; set; }
		public DbSet<CollectionTier> CollectionTiers { get; set; }
        public DbSet<MFA> MFA { get; set; }
		public DbSet<MFAEmailOTP> MFAEmailOTPs { get; set; }
		public DbSet<MFAResetRequest> MFAResetRequests { get; set; }




        public ApplicationUserDbContext(
			DbContextOptions<ApplicationUserDbContext> options,
			IUnitOfWork unitOfWorkContext,
			IEventDispatcher dispatcher, IIdentityContext identityContext)
			: base(options, dispatcher, unitOfWorkContext, identityContext)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
			//builder.ApplyConfiguration(new CountryEntityTypeConfiguration());
			//builder.ApplyConfiguration(new SolutionEntityTypeConfiguration());
			//builder.ApplyConfiguration(new UserTypeEntityTypeConfiguration());
			//builder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
			//builder.ApplyConfiguration(new SystemEnvironmentEntityTypeConfiguration());
			//builder.ApplyConfiguration(new AccountStatusEntityTypeConfiguration());
			//builder.ApplyConfiguration(new ApplicationUserEntityTypeConfiguration());
			//builder.ApplyConfiguration(new CustomerUserEntityTypeConfiguration());
			//builder.ApplyConfiguration(new TrangloStaffEntityTypeConfiguration());
			//builder.ApplyConfiguration(new ApplicationUserClaimEntityTypeConfiguration());
			//builder.ApplyConfiguration(new GenderEntityTypeConfiguration());
			//builder.ApplyConfiguration(new IndividualProfileEntityTypeConfiguration());
		}

	}
}

