// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Masking.Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Linq;
using Serilog.Sinks.File;
namespace Tranglo1.CustomerIdentity.IdentityServer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var BUILD_VERSION = Environment.GetEnvironmentVariable("BUILD_VERSION");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Destructure.ByMaskingProperties(x =>
                {
                    x.PropertyNames.Add("Password");
                    x.PropertyNames.Add("Token");
                    x.PropertyNames.Add("Cookie");
                    x.PropertyNames.Add("ApiKey");
                    x.PropertyNames.Add("LoginId");
                    x.PropertyNames.Add("Email");
                    x.PropertyNames.Add("Emails");
                    x.PropertyNames.Add("EmailAddress");
                    x.Mask = "******";
                })
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                /*
                .WriteTo.File(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level} {CorrelationId}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    path: "logs/identityserver.log",
                    buffered: true, 
                    rollingInterval: RollingInterval.Hour,
                    flushToDiskInterval: TimeSpan.FromSeconds(10)
                )
                */
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level} {CorrelationId}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            Log.Information($"environment = {environment}");
            Log.Information($"BUILD_VERSION : {BUILD_VERSION}");

            try
            {
                var seed = args.Contains("/seed");
                if (seed)
                {
                    args = args.Except(new[] { "/seed" }).ToArray();
                }

                var host = CreateHostBuilder(args).Build();


                if (seed)
                {
                    Log.Information("Seeding database...");
                    var config = host.Services.GetRequiredService<IConfiguration>();
                    var connectionString = config.GetConnectionString("DefaultConnection");
                    //SeedData.EnsureSeedData(connectionString);
                    Log.Information("Done seeding database.");
                    return 0;
                }

                Log.Information("Starting host...");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                 .UseSerilog((builder, cfg) =>
                 {
                     cfg.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                         .WriteTo.Console(
                             theme: AnsiConsoleTheme.Code,
                             outputTemplate: "[{Timestamp:HH:mm:ss} {Level} {CorrelationId}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
                         );
                         /*
                         .WriteTo.File(

                            outputTemplate: "[{Timestamp:HH:mm:ss} {Level} {CorrelationId}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                            //path: "logs/identityserver.log",
                            buffered: true,
                            rollingInterval: RollingInterval.Hour,
                            flushToDiskInterval: TimeSpan.FromSeconds(10)
                         );
                         */
                     cfg.ReadFrom.Configuration(builder.Configuration);

                 })
                .ConfigureWebHostDefaults(configure =>
                {
                    configure.UseStartup<Startup>();

                    configure.ConfigureKestrel(options =>
                    {
                        // Disable "Server: Kestrel"
                        options.AddServerHeader = false;
                    });
                })
                .ConfigureHostConfiguration(webBuilder => {
                    webBuilder.AddCommandLine(args);
                });
              
    }
}