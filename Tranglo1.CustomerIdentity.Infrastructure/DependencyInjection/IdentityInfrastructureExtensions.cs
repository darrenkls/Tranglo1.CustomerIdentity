using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Compliance;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Watchlist;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Commons;
using Tranglo1.CustomerIdentity.Infrastructure.ExternalServices;
using Tranglo1.CustomerIdentity.Infrastructure.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Services;

namespace Tranglo1.CustomerIdentity.Infrastructure.DependencyInjection
{
    public static class IdentityInfrastructureExtensions
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddScoped<IBusinessProfileRepository, BusinessProfileRepository>();
            services.AddScoped<IScreeningRepository, ScreeningRepository>();
            services.AddScoped<IRBARepository, RBARepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<ITrangloRoleRepository, TrangloRoleRepository>();
            services.AddScoped<IExternalUserRoleRepository, ExternalUserRoleRepository>();
            services.AddScoped<ISignUpCodeRepository, SignUpCodeRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<ICountrySettingRepository, CountrySettingRepository>();
            services.AddScoped<CsvExporter>();

            services.AddTransient<LoggingHandler<ComplianceExternalService>>();
            services.AddHttpClient<IComplianceExternalService, ComplianceExternalService>()
                .ConfigureHttpClient(e =>
                {
                    string baseUrl = configuration.GetValue<string>("ComplianceScreeningAPI");
                    string apiKey = configuration.GetValue<string>("ComplianceScreeningAPIKey");

                    e.BaseAddress = new Uri(baseUrl);
                    e.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TIKAPI", apiKey);

                    e.DefaultRequestHeaders.UserAgent.Clear();
                    e.DefaultRequestHeaders.UserAgent.ParseAdd($"ComplianceExternalService-{hostEnvironment.EnvironmentName}/1.0");

                    // The following settings are available in .NET 5.0 or greater
                    #if NET5_0_OR_GREATER
                        e.DefaultRequestVersion = new Version(2, 0);
                        e.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                    #endif
                })
                .AddHttpMessageHandler<LoggingHandler<ComplianceExternalService>>()
                .AddPolicyHandler(GetRetryPolicy());

            services.AddTransient<LoggingHandler<WatchlistNotificationExternalService>>();
            services.AddHttpClient<IWatchlistNotificationExternalService, WatchlistNotificationExternalService>()
                .ConfigureHttpClient(e =>
                {
                    string baseUrl = configuration.GetValue<string>("IdentityServerUri");
                    string apiKey = configuration.GetValue<string>("ApiKeyValue");

                    e.BaseAddress = new Uri(baseUrl);
                    e.DefaultRequestHeaders.Add("X-T1-API-Key", apiKey);

                    e.DefaultRequestHeaders.UserAgent.Clear();
                    e.DefaultRequestHeaders.UserAgent.ParseAdd($"WatchlistNotificationExternalService-{hostEnvironment.EnvironmentName}/1.0");

                    // The following settings are available in .NET 5.0 or greater
                    #if NET5_0_OR_GREATER
                        e.DefaultRequestVersion = new Version(2, 0);
                        e.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                    #endif
                })
                .AddHttpMessageHandler<LoggingHandler<WatchlistNotificationExternalService>>()
                .AddPolicyHandler(GetRetryPolicy());

            return services;
        }

        #region Private Helper Methods
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // network errors + 5xx + 408
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"Retry {retryAttempt} after {timespan}. " +
                                          $"Reason: {(outcome.Exception != null ? outcome.Exception.Message : outcome.Result.StatusCode.ToString())}");
                    });
        } 
        #endregion Private Helper Methods
    }
}
