using AutoMapper;
using CorrelationId;
using CorrelationId.Abstractions;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Tranglo1.Onboarding.Domain.DomainServices;
using Tranglo1.Onboarding.Application.Common.Exceptions;
using Tranglo1.Onboarding.Application.Infrastructure.Swagger;
using Tranglo1.Onboarding.Application.Managers;
using Tranglo1.Onboarding.Application.MediatR.Behaviours;
using Tranglo1.Onboarding.Application.Services;
using Tranglo1.Onboarding.Application.Services.AuditLog;
using Tranglo1.Onboarding.Application.Services.Notification;
using Tranglo1.Onboarding.Infrastructure.Event;
using Tranglo1.Onboarding.Infrastructure.Persistence;
using Tranglo1.Onboarding.Infrastructure.DependencyInjection;
using Tranglo1.DocumentStorage.EntityFramework.DBContexts;
using Tranglo1.Onboarding.Application.DependencyInjection;

namespace Tranglo1.Onboarding.API
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks().AddSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                });

            services.Configure<MvcOptions>(configure =>
            {
                configure.InputFormatters.RemoveType<NewtonsoftJsonPatchInputFormatter>();
                configure.OutputFormatters.RemoveType<StringOutputFormatter>();
                configure.ReturnHttpNotAcceptable = true;

                var jsonOutputFormatter = configure.OutputFormatters
                    .OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                jsonOutputFormatter?.RemoveMediaType("text/json");
                jsonOutputFormatter?.RemoveMediaType("application/*+json");
            });

            // DbContexts
            services.AddDbContext<BusinessProfileDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddDbContext<ScreeningDBContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddDbContext<RBADBContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddDbContext<RBARequisitionDBContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

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

            services.AddDbContext<KYCPartnerStatusRequisitionDbContext>((provider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                options.EnableDetailedErrors(true);
            });

            services.AddHttpContextAccessor();

            // AutoMapper - scan both API and Application assemblies
            var onboardingAssembly = typeof(OnboardingApplicationAssemblyMarker).Assembly;
            services.AddAutoMapper(typeof(Startup), onboardingAssembly);

            // MediatR
            services.AddScoped<IEventDispatcher, MediatREventDispatcher>();
            services.AddMediatR(onboardingAssembly)
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));

            // FluentValidation
            services.AddValidatorsFromAssembly(onboardingAssembly);

            services.Configure<MvcOptions>(options =>
            {
                NewtonsoftJsonOutputFormatter newtonsoftJsonOutputFormatter =
                    options.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault() ??
                    throw new ArgumentNullException(nameof(newtonsoftJsonOutputFormatter));
                newtonsoftJsonOutputFormatter
                    .SupportedMediaTypes
                    .Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/problem+json"));
            });

            services.AddHttpClient();

            // Application services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ResetMFASuccessfulEmailSender>();
            services.AddSingleton<RegisterMfaSuccessfulEmailSender>();
            services.AddScoped<BusinessProfileService>();
            services.AddScoped<ComplianceScreeningService>();
            services.AddScoped<PartnerService>();
            services.AddScoped<IntegrationManager>();
            services.AddScoped<RBAService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddOnboardingApplicationServices();

            // Storage manager
            services.AddStorageManager(config =>
            {
                config.FileTablePath = Configuration.GetValue<string>("FileTablePath");
                config.TempTablePath = Configuration.GetValue<string>("TempFilePath");
            });

            services.AddSqlServerUnitOfWork(Configuration.GetConnectionString("DefaultConnection"));

            // Approval workflow engines
            services.AddDefaultApprovalManager<Tranglo1.Onboarding.Domain.Entities.Requisition.PartnerKYCStatusRequisition>()
                .ConfigureApprovalWorkflowEngine<Tranglo1.Onboarding.Domain.Entities.Requisition.PartnerKYCStatusRequisition>(approvalConfiguration =>
                {
                    approvalConfiguration.MaxLevelRequired = 2;
                    approvalConfiguration.AllowedToBypassLowerApprovalLevel = true;
                    approvalConfiguration.IsLocked = false;
                }).AddUserIdentityContext<Tranglo1.Onboarding.Application.Managers.KYCApproval.RequisitionUserIdentityContext>()
                .AddDbContext<Tranglo1.Onboarding.Domain.Entities.Requisition.PartnerKYCStatusRequisition, KYCPartnerStatusRequisitionDbContext>();

            services.AddDefaultApprovalManager<Tranglo1.Onboarding.Domain.Entities.RBAAggregate.Requisitions.RBARequisition>()
                .ConfigureApprovalWorkflowEngine<Tranglo1.Onboarding.Domain.Entities.RBAAggregate.Requisitions.RBARequisition>(approvalConfiguration =>
                {
                    approvalConfiguration.MaxLevelRequired = 1;
                    approvalConfiguration.AllowedToBypassLowerApprovalLevel = true;
                    approvalConfiguration.IsLocked = false;
                }).AddUserIdentityContext<Tranglo1.Onboarding.Application.Managers.KYCApproval.RequisitionUserIdentityContext>()
                .AddDbContext<Tranglo1.Onboarding.Domain.Entities.RBAAggregate.Requisitions.RBARequisition, RBARequisitionDBContext>();

            // globally define app's root folder for template resolution (Xslt resolution)
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
                    var correlationAccessor = context.RequestServices.GetService<ICorrelationContextAccessor>();
                    return correlationAccessor.CorrelationContext.CorrelationId;
                };
            });

            // Authentication - validate tokens from IdentityServer
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("IdentityServerUri");
                    options.RequireHttpsMetadata = !Environment.IsDevelopment();
                    options.SaveToken = true;
                });

            ConfigureSwagger(services);

            services.AddDefaultCorrelationId(options =>
            {
                options.AddToLoggingScope = true;
                options.IncludeInResponse = false;
            });

            services.AddHttpClient("")
                .AddCorrelationIdForwarding();
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddVersionedApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = "'v'V";
            });

            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.ReportApiVersions = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
            });

            var versionExplorer = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();

                foreach (var apiVersion in versionExplorer.ApiVersionDescriptions)
                {
                    c.SwaggerDoc($"v{apiVersion.ApiVersion}",
                        new OpenApiInfo { Title = "Onboarding API", Version = $"v{apiVersion.ApiVersion}" });
                }

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });

                c.OperationFilter<FileOperationFilter>();
                c.OperationFilter<SwaggerDefaultValues>();
                c.DocumentFilter<VersionPathDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });
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
            app.UseSerilogRequestLogging();
            app.UseProblemDetails();

            app.UseCors(builder => builder
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true)
                .AllowAnyHeader()
                .AllowAnyMethod());

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersion in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint(
                        $"/swagger/v{apiVersion.ApiVersion}/swagger.json",
                        $"Onboarding API v{apiVersion.ApiVersion}");
                }
                c.DisplayRequestDuration();
            });

            app.UseSqlServerUnitOfWork();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("api/v1/healthcheck");
            });
        }
    }
}
