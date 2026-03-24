using Microsoft.Extensions.DependencyInjection;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.Infrastructure.DependencyInjection
{
    public static class IdentityInfrastructureExtensions
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            // Identity-specific repositories
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();
            services.AddScoped<ITrangloRoleRepository, TrangloRoleRepository>();
            services.AddScoped<IExternalUserRoleRepository, ExternalUserRoleRepository>();
            services.AddScoped<ISignUpCodeRepository, SignUpCodeRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<ICountrySettingRepository, CountrySettingRepository>();

            // Cross-domain service (bridges Identity -> Onboarding via Contracts)
            services.AddScoped<IStaffEntityQueryService, StaffEntityQueryService>();

            return services;
        }
    }
}
