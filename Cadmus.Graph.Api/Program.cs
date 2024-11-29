using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Serilog;
using Cadmus.Graph.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Cadmus.Graph.Ef.PgSql;
using Cadmus.Graph.Ef;
using Scalar.AspNetCore;

namespace Cadmus.Graph.Api;

public static class Program
{
    private static void DumpEnvironmentVars()
    {
        Console.WriteLine("ENVIRONMENT VARIABLES:");
        IDictionary dct = Environment.GetEnvironmentVariables();
        List<string> keys = new();
        var enumerator = dct.GetEnumerator();
        while (enumerator.MoveNext())
        {
            keys.Add(((DictionaryEntry)enumerator.Current).Key.ToString()!);
        }

        foreach (string key in keys.OrderBy(s => s))
            Console.WriteLine($"{key} = {dct[key]}");
    }

    public static void Main(string[] args)
    {
        // setup logger: see instructions at:
        // https://github.com/datalust/dotnet6-serilog-example

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        Log.Information("Starting up");

        try
        {
            Log.Information("Starting Cadmus Graph API host");
            DumpEnvironmentVars();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            // more config sources
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddCommandLine(args);

            // add services to the container
            builder.Services.AddScoped<IGraphRepository>(provider =>
            {
                var config = provider.GetService<IConfiguration>()!;
                string cst = config.GetConnectionString("Template")!;
                var repository = new EfPgSqlGraphRepository();
                repository.Configure(new EfGraphRepositoryOptions
                {
                    ConnectionString = string.Format(cst,
                        config.GetValue<string>("DatabaseName") ?? "cadmus-graph")
                });
                return repository;
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();

            // setup log (config is read from appsettings.json under Serilog)
            builder.Host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .WriteTo.Console()
//#if DEBUG
//                    .WriteTo.File("cadmus-graph-log.txt", rollingInterval: RollingInterval.Day)
//#endif
                .ReadFrom.Configuration(ctx.Configuration));

            // seed service
            // https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-3/
            builder.Services.AddHostedService<DatabaseSeedService>();

            WebApplication app = builder.Build();

            // middleware
            app.UseCors(builder =>
            {
                // open everything, we don't care, that's a playground
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
            app.UseSerilogRequestLogging();

            // configure the HTTP request pipeline
            app.UseAuthorization();
            app.MapControllers();
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Cadmus API")
                       .WithPreferredScheme("Bearer");
            });

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }
}