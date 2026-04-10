// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using CorrelationId;
using CorrelationId.Abstractions;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using EntityFrameworkCore.SqlServer.TemporalTable.Extensions;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Design;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using IdentityServer4;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Tranglo1.Common.Cache;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.AspNetCore.Builder;
using Tranglo1.CustomerIdentity.IdentityServer.Common.EventHandlers;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Exceptions;
using Tranglo1.CustomerIdentity.IdentityServer.Common.ModelBinder;
using Tranglo1.CustomerIdentity.IdentityServer.Data;
using Tranglo1.CustomerIdentity.IdentityServer.DependencyInjection;
using Tranglo1.CustomerIdentity.IdentityServer.Extensions;
using Tranglo1.CustomerIdentity.IdentityServer.Hubs;
using Tranglo1.CustomerIdentity.IdentityServer.Infrastructure.Persistance;
using Tranglo1.CustomerIdentity.IdentityServer.Infrastructure.Swagger;
using Tranglo1.CustomerIdentity.IdentityServer.Providers;
using Tranglo1.CustomerIdentity.IdentityServer.Security;
using Tranglo1.CustomerIdentity.IdentityServer.Services;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Notification;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha;
using Tranglo1.CustomerIdentity.IdentityServer.Services.SignalR;
using Tranglo1.CustomerIdentity.IdentityServer.Services.UserProfile;
using Tranglo1.CustomerIdentity.Infrastructure.Event;
using Tranglo1.CustomerIdentity.Infrastructure.Identity;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.DocumentStorage.EntityFramework.DBContexts;
using Tranglo1.CustomerIdentity.Infrastructure.DependencyInjection;

[assembly: InternalsVisibleTo("Tranglo1.CustomerIdentity.Registration.Test")]

namespace Tranglo1.CustomerIdentity.IdentityServer
{
    class EnhancedDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            //services.AddEntityFrameworkSqlServer();
            //services.RegisterTemporalTablesForDatabase();
            services.AddSingleton<ICSharpMigrationOperationGenerator, TemporalCSharpMigrationOperationGenerator>();
        }
    }
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private void ConfigureDataProtectionKeys(IServiceCollection services)
        {
            // Add a DbContext to store your Database Keys
            services.AddDbContext<ProtectionKeyDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
                //options.EnableSensitiveDataLogging(true);

            });

            X509Certificate2 _SignCert = new X509Certificate2("Identity.pfx");

            // using Microsoft.AspNetCore.DataProtection;
            services.AddDataProtection(options =>
            {
                options.ApplicationDiscriminator = "Tranglo.IdentityServer";
            })
            .SetApplicationName("Tranglo.IdentityServer")
            .PersistKeysToDbContext<ProtectionKeyDbContext>()
            .ProtectKeysWithCertificate(_SignCert);
        }

        public void ConfigureServices(IServiceCollection services)
        {

            //for DEBUG only
            IdentityModelEventSource.ShowPII = false;
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //services.AddApplication(Configuration);
            //services.AddInfrastructure(Configuration);
            services.AddHealthChecks().AddSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);

            services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            })
            .AddCookieTempDataProvider(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            /*
            services.PostConfigure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context => new UnprocessableEntityObjectResult(context);
            });
            */


            //We configure MvcOptions after
            //  .AddNewtonsoftJson() and 
            //  .AddXmlSerializerFormatters()
            services.Configure<MvcOptions>(configure =>
            {
                configure.InputFormatters.RemoveType<NewtonsoftJsonPatchInputFormatter>();
                configure.OutputFormatters.RemoveType<StringOutputFormatter>();

                configure.ReturnHttpNotAcceptable = true;

                var jsonOutputFormatter = configure.OutputFormatters
                    .OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                jsonOutputFormatter?.RemoveMediaType("text/json");
                jsonOutputFormatter?.RemoveMediaType("application/*+json");

                var xmlOutputFormatter = configure.OutputFormatters
                    .OfType<XmlSerializerOutputFormatter>().FirstOrDefault();
                xmlOutputFormatter?.RemoveMediaType("text/xml");

                var jsonInputFormatter = configure.InputFormatters
                    .OfType<NewtonsoftJsonInputFormatter>().FirstOrDefault();
                jsonInputFormatter?.RemoveMediaType("text/json");
                jsonInputFormatter?.RemoveMediaType("application/*+json");

                var xmlInputFormatter = configure.InputFormatters
                    .OfType<XmlSerializerInputFormatter>().FirstOrDefault();
                xmlInputFormatter?.RemoveMediaType("text/xml");
                xmlInputFormatter?.RemoveMediaType("application/*+xml");
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddDbContext<BusinessProfileDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
                //options.EnableSensitiveDataLogging(true);
                //options.LogTo(Console.WriteLine);
            });
            services.AddDbContext<ApplicationUserDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
                //options.EnableSensitiveDataLogging(true);
                //options.LogTo(Console.WriteLine);
            }, ServiceLifetime.Transient);
            services.AddDbContext<TrangloRoleDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Transient);


            services.AddDbContext<AuditLogDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });


            services.AddDbContext<DocumentStorageDBContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddDbContext<PartnerDBContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddDbContext<ExternalUserRoleDbContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddDbContext<SignUpCodeDBContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddDbContext<OTPDbContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });


            services.AddDbContext<CountrySettingDbContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });
            services.AddEntityFrameworkSqlServer();
            services.RegisterTemporalTablesForDatabase();

            //https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-3.1&tabs=visual-studio
            ConfigureDataProtectionKeys(services);

            services.AddHttpClient();
            ConfigureRecaptchaKeys(services);

            ConfigureSwagger(services);
            /*
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            */
            //ConfigureAuthentication(services);

            //services.AddTransient<IProfileService, CustomProfileService>();


            services.AddHttpContextAccessor();
            //services.AddAutoMapper(c => c.AddProfile<DtoToCommandProfile>(), typeof(Startup));

            services.AddAuthorization(configure =>
            {
                configure.AddPolicy(AuthenticationPolicies.InternalOnlyPolicy, policy =>
                {
                    policy.Combine(AuthenticationPolicies.CreateInternalUserOnlyPolicy());
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                });

                configure.AddPolicy(AuthenticationPolicies.ExternalOnlyPolicy, policy =>
                {
                    policy.Combine(AuthenticationPolicies.CreateExternalUserOnlyPolicy());
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                });

                configure.AddPolicy(AuthenticationPolicies.InternalOrExternalPolicy, policy =>
                {
                    policy.Combine(AuthenticationPolicies.CreateInternalOrExternalUserPolicy());
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                });

                configure.AddPolicy(AuthenticationPolicies.ExternalBusinessOnlyPolicy, policy =>
                {
                    policy.Combine(AuthenticationPolicies.CreateExternalBusinessUserOnlyPolicy());
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                });
            });

            var _ExpireTimeSpan = TimeSpan.FromMinutes(15);
            var _CookieExpiration = Configuration.GetSection("CookieOptions:ExpireTimeSpanInMinutes").Value;
            if (int.TryParse(_CookieExpiration, out var _Minutes))
            {
                if (_Minutes > 0)
                {
                    _ExpireTimeSpan = TimeSpan.FromMinutes(_Minutes);
                }
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;

            })
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = new PathString("/Account/Login");
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };

                o.Cookie.HttpOnly = true;
                o.Cookie.SameSite = SameSiteMode.Strict;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.IsEssential = true;
                o.SlidingExpiration = false;
                o.ExpireTimeSpan = _ExpireTimeSpan;
            })
            .AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                o.Cookie.HttpOnly = true;
                o.Cookie.SameSite = SameSiteMode.Strict;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.IsEssential = true;
                o.SlidingExpiration = false;
            })
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
                };

                o.Cookie.HttpOnly = true;
                o.Cookie.SameSite = SameSiteMode.Strict;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.IsEssential = true;
                o.SlidingExpiration = false;
                o.ExpireTimeSpan = _ExpireTimeSpan;
            })
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                o.Cookie.HttpOnly = true;
                o.Cookie.SameSite = SameSiteMode.Strict;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                o.Cookie.IsEssential = true;
                o.SlidingExpiration = false;
            })
            .AddLocalApi(IdentityServerConstants.LocalApi.AuthenticationScheme, configureOptions =>
            {
                //Use this when running IdentityServer and API at the same machine
                configureOptions.SaveToken = true;
                configureOptions.ForwardSignIn = IdentityConstants.ApplicationScheme;
            });

            var ldapServer = Configuration.GetSection("LdapConfiguration").Get<LdapConfiguration>();
            services.AddLdapPasswordValidator(ldapServer, serverConnection =>
            {
                serverConnection.UseSsl();
            });

            ConfigurationOptions options = new ConfigurationOptions()
            {
                EndPoints = {
                    { Configuration.GetValue<string>("RedisConfig:ConnectionUrl"), Configuration.GetValue<int>("RedisConfig:Port")},
                },
                Password = Configuration.GetValue<string>("RedisConfig:Password"),
                AbortOnConnectFail = false
            };
            services.AddSingleton<IRedisCacheManager>(o => new RedisCacheManager(options));

            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();
            services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<ApplicationUser>>();

            services.AddIdentityInfrastructure();

            services.AddStorageManager(config =>
            {
                config.FileTablePath = Configuration.GetValue<string>("FileTablePath");
                config.TempTablePath = Configuration.GetValue<string>("TempFilePath");
            });


            Lockout lockout = Configuration.GetSection("Lockout").Get<Lockout>();


            //Override default identity unique username checking
            services.AddTransient<IUserValidator<ApplicationUser>, TrangloUserValidator<ApplicationUser>>();
            services.AddIdentityCore<ApplicationUser>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";

                option.SignIn.RequireConfirmedEmail = true;
                option.SignIn.RequireConfirmedAccount = true;

                //Password must contain at least 8 characters, 1 lower case, 1 upper case, 1 numeric and 1 symbol
                option.Password.RequiredLength = 8;
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireDigit = true;
                option.Password.RequiredUniqueChars = 1;

                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(90);
                option.Lockout.MaxFailedAccessAttempts = lockout.MaxFailedAccessAttempts;
                option.Lockout.AllowedForNewUsers = true;

                //Map custom token provider
                option.Tokens.PasswordResetTokenProvider = "CustomResetPasswordDataProtectorTokenProvider";
            })
            .AddUserStore<TrangloUserStore>()
            .AddPersonalDataProtection<DefaultLookupProtector, DefaultLookupProtectorKeyRing>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<CustomResetPasswordTokenProvider<ApplicationUser>>("CustomResetPasswordDataProtectorTokenProvider")
            .AddTokenProvider<InvitationTokenProvider<ApplicationUser>>("InvitationDataProtectorTokenProvider")
            .AddSignInManager<TrangloSignInManager>()
            .AddUserManager<TrangloUserManager>();

            services.AddScoped<ILookupNormalizer, LowerInvariantLookupNormalizer>();

            //This is the cert to sign the JWT. When go live, we need to cert that 
            //is issued to identity server (ie: subject = identity.tranglo.com) <-- prove this!
            X509Certificate2 _SignCert = new X509Certificate2("Identity.pfx");

            //Note:
            //  Default endpoints can be found at Constants.ProtocolRoutePaths.cs
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;

                options.Endpoints.EnableTokenEndpoint = true; //Default is true

                options.IssuerUri = Configuration.GetValue<string>("IdentityServerUri"); //If not specifiy then this value will be inferred from request.

                options.LowerCaseIssuerUri = true;

            })

                //Let identity server can use UserManager<> to authenticate request
                .AddAspNetIdentity<ApplicationUser>()

                //Using email in token request with IdentityServer4
                //.AddResourceOwnerValidator<OwnerPasswordValidatorService<ApplicationUser>>()

                //configure database to keep identity server's setting
                //.AddConfigurationStore(option =>
                //{
                //    option.ConfigureDbContext = ctx =>
                //    {
                //        ctx.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                //            b =>
                //            {
                //                b.MigrationsAssembly(migrationsAssembly);
                //                b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                //            });
                //    };

                //    option.DefaultSchema = "configuration";

                //})

                //this is about tables that store issued/used tokens....
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            });

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
                    options.DefaultSchema = "operation";
                })
                .AddSigningCredential(_SignCert)
                .AddProfileService<TrangloUserProfileService>()
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryIdentityResources(Config.IdentityResources);


            //enable scalfolding pages for identity module
            services.AddRazorPages();
            services.AddAutoMapper(typeof(Startup));

            /*Add all Application related DI*/
            services.AddScoped<IEventDispatcher, MediatREventDispatcher>();

            //Note:
            //	MediatR does not support ordering for IPipelineBehavior yet as the time when 
            //	preparing this demo (https://github.com/jbogard/MediatR/pull/509)
            //	By default, ordering for IPipelineBehavior will depends on their registration sequence.
            services.AddMediatR(Assembly.GetExecutingAssembly())
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(UserAccessControlBehaviour<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            //.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            //Turn off due to Controller returning 406
            services.Configure<MvcOptions>(
            options =>
            {
                NewtonsoftJsonOutputFormatter newtonsoftJsonOutputFormatter =
                    options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault() ??
                    throw new ArgumentNullException(nameof(newtonsoftJsonOutputFormatter));

                // add media types that we want to support by default
                newtonsoftJsonOutputFormatter
                    .SupportedMediaTypes
                    .Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/problem+json"));
            });


            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-IdentityServers/implement-http-call-retries-exponential-backoff-polly
            services.AddHttpClient();

            services.AddSingleton<ICapchaValidator, ReCapchaValidator>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ResetMFASuccessfulEmailSender>();
            services.AddSingleton<RegisterMfaSuccessfulEmailSender>();
            services.AddScoped<BusinessProfileService>();
            services.AddScoped<ComplianceScreeningService>();
            services.AddScoped<PartnerService>();

            services.AddScoped<ApplicationUserService>();
            services.AddHttpContextAccessor();

            //UAC Related
            services.AddUserAccessControl()
                .FindPermissionsInAssemblyAndCsv(typeof(Startup), "PermissionInfos")
                .ConfigurePermissionAccessControlStore(option =>
                {
                    option.ConfigureDbContext = ctx =>
                    {
                        ctx.UseSqlServer(Configuration.GetConnectionString("UACConnection"),
                            b =>
                            {
                                b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                                b.MigrationsAssembly(migrationsAssembly);
                            });
                    };
                    option.DefaultSchema = "uac";
                })
                .ImportDefaultAssignmentsFromCsv(typeof(Startup), "PermissionAssignments")
                .ImportDefaultPortalFromCsv(typeof(Startup), "Portals");

            services.AddHttpIdentityContext();
            services.AddScoped<IAuditLogManager, AuditLogManager>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddBusinessProfileContext();
            services.AddPartnerCodeContext();
            services.AddTrangloEntityContext();
            services.AddRoleCodeContext();
            services.AddSqlServerUnitOfWork(Configuration.GetConnectionString("DefaultConnection"));

            // globally define app's root folder for template resolution (Xslt resolution)
            // Refactor remove AppDomain
            AppDomain.CurrentDomain.SetData("ContentRootPath", Environment.ContentRootPath);
            AppDomain.CurrentDomain.SetData("WebRootPath", Environment.WebRootPath);

            services.AddProblemDetails(setup =>
            {
                setup.IncludeExceptionDetails = (ctx, env) => Environment.IsDevelopment();

                setup.Map<ForbiddenException>(exception => new ProblemDetails
                {
                    Title = exception.Title,
                    Detail = exception.Message,
                    Status = StatusCodes.Status403Forbidden,
                    Type = exception.Type
                });

                setup.Map<NotFoundException>(exception => new ProblemDetails
                {
                    Title = exception.Title,
                    Status = StatusCodes.Status404NotFound,
                    Type = exception.Type
                });

                setup.GetTraceId = context =>
                {
                    var _CorrelationIdAccessor = context.RequestServices.GetService<ICorrelationContextAccessor>();
                    return _CorrelationIdAccessor.CorrelationContext.CorrelationId;
                };
            });



            // Configure SignalR
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, SignalrUserLoginProvider>();
            services.AddScoped<SignalRMessageService>();
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            var authorizationUrl = Configuration.GetValue<string>("IdentityServerUri") + "/connect/authorize";
            var tokenUrl = Configuration.GetValue<string>("IdentityServerUri") + "/connect/token";

            services.AddVersionedApiExplorer(setupAction =>
            {
                //means a "v" followed with major + minor version (represented by uppercase "V")
                //Source: https://github.com/Microsoft/aspnet-api-versioning/wiki/Version-Format
                setupAction.GroupNameFormat = "'v'V";
            });

            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.ReportApiVersions = true;
                setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);

                //client specify api version by header
                //setupAction.ApiVersionReader = new HeaderApiVersionReader("api-version");

                //client specify api version by media type
                //setupAction.ApiVersionReader = new MediaTypeApiVersionReader();
            });

            //Note: The following line must be after services.AddVersionedApiExplorer(...)
            var _VersionExplorer = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                //Enable all attributes from Swashbuckle.AspNetCore.Annotations package.
                c.EnableAnnotations();

                foreach (var apiVersion in _VersionExplorer.ApiVersionDescriptions)
                {
                    c.SwaggerDoc($"v{apiVersion.ApiVersion}",
                        new OpenApiInfo { Title = "Customer Identity API", Version = $"v{apiVersion.ApiVersion}" });
                }

                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer Identity Login API", Version = "v1" });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(authorizationUrl),
                            TokenUrl = new Uri(tokenUrl),

                            Scopes =
                                {
                                    { IdentityServerConstants.StandardScopes.OpenId, "Basic scope" },
                                    { IdentityServerConstants.StandardScopes.OfflineAccess, "Needed for refresh token" },
                                    { IdentityServerConstants.StandardScopes.Profile,
                                        "name, family_name, given_name, middle_name, nickname, " +
                                        "preferred_username, profile, picture, website, gender, birthdate, " +
                                        "zoneinfo, locale, and updated_at."
                                    },
                                    {
                                        IdentityServerConstants.StandardScopes.Email,
                                        "This scope value requests access to the email and email_verified Claims."
                                    },
                                    { "business_profile" , "Company name of the business profile" },
                                    { "tranglo_profile" , "Company name of the Tranglo" },
                                    /*
                                    {"api1.read", "Deprecated"},
                                    {"IdentityServerApi", "Deprecated"},
                                    {"dynamicroles", "Deprecated"},
                                    {"company_name", "Deprecated. Use business_profile"},
                                    {"types", "Deprecated"},
                                    {"userid", "Deprecated. Use business_profile"},
                                    {"roles", "Deprecated" },
                                    {"account_status", "Deprecated. Use business_profile"},
                                    */
                                }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme(){
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference()
                            {
                                 Id = "oauth2", Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });

                //c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.OperationFilter<FileOperationFilter>();

                //This will help to fill in the default version for api-version parameter
                //in swagger UI
                c.OperationFilter<SwaggerDefaultValues>();
                c.DocumentFilter<VersionPathDocumentFilter>();
                // c.OperationFilter<SecurityRequirementsOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new PortlModelBinderProvider());
            });

            services.AddDefaultCorrelationId(options =>
            {
                options.AddToLoggingScope = true;
                options.IncludeInResponse = false;
            });

            services.AddHttpClient("")
                .AddCorrelationIdForwarding();
        }

        private void ConfigureRecaptchaKeys(IServiceCollection services)
        {
            RecaptchaKey recaptchaKey = new RecaptchaKey();
            Configuration.GetSection("RecaptchaKey").Bind(recaptchaKey);

            //Create singleton from instance
            services.AddSingleton<RecaptchaKey>(recaptchaKey);
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All,
                RequireHeaderSymmetry = false,
                ForwardLimit = null,
            };

            forwardOptions.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Any, 0));

            app.UseForwardedHeaders(forwardOptions);

            app.UseCorrelationId();
            app.UseSerilogRequestLogging(opts
                               => opts.EnrichDiagnosticContext = (diagnosticsContext, httpContext) =>
                               {
                                   var request = httpContext.Request;

                                   diagnosticsContext.Set("Custom Header value", request.Headers["custom-header-value"]);
                               });

            app.UseProblemDetails();

            app.UseCors(builder => builder
                   .AllowCredentials()
                   .SetIsOriginAllowed(origin => true)
                   .AllowAnyHeader()
                   .AllowAnyMethod());

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            //Internally this will call app.UseAuthentication() for us
            app.UseIdentityServer();
            //app.UseAuthentication(); //Testing to fix 404 signin-oidc admin routing in master branch
            app.UseAuthorization();

            if (Environment.IsDevelopmentEnv() || Environment.IsQA())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    foreach (var apiVersion in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        c.SwaggerEndpoint(
                            $"{Configuration["Swagger:BasePath"]}/swagger/v{apiVersion.ApiVersion}/swagger.json",
                            $"Customer Identity API v{apiVersion.ApiVersion}");
                    }

                    //c.SwaggerEndpoint($"{Configuration["Swagger:BasePath"]}/swagger/v1/swagger.json", "Customer Identity API V1");
                    c.DisplayRequestDuration();
                    //// Enable when deploy to server
                    //c.RoutePrefix = string.Empty;

                    c.OAuthClientId("connect-client-local");

                    c.OAuthScopes(
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       IdentityServerConstants.StandardScopes.Email,
                       "business_profile",
                       IdentityServerConstants.StandardScopes.OfflineAccess
                    );
                    c.OAuthUsePkce();

                });
            }

            app.UseFileUploadSecurity();

            app.UseSqlServerUnitOfWork();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("api/v1/healthcheck");
                // signalr hub mapping
                endpoints.MapHub<UserLogOffHub>("hub/user-log-off");
            });
        }
    }
}