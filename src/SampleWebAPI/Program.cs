
using Diginsight.Diagnostics;
using Diginsight;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Diginsight.SmartCache.Externalization.ServiceBus;
using Diginsight.SmartCache;
using SampleWebAPI.Configuration;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Diginsight.SmartCache.Externalization.Http;

namespace SampleWebAPI;

public class Program
{
    private static readonly string SmartCacheServiceBusSubscriptionName = Guid.NewGuid().ToString("N");
    public static void Main(string[] args)
    {
        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));
        Observability.LoggerFactory = observabilityManager.LoggerFactory;

        WebApplication app;
        using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;
            var environment = builder.Environment;
            var loggerFactory = Observability.LoggerFactory;

            // Add logging and opentelemetry
            services.AddObservability(configuration, environment, out IOpenTelemetryOptions openTelemetryOptions);
            observabilityManager.AttachTo(services);

            services.ConfigureClassAware<ConcurrencyOptions>(configuration.GetSection("AppSettings"))
                .DynamicallyConfigureClassAware<ConcurrencyOptions>()
                .VolatilelyConfigureClassAware<ConcurrencyOptions>();

            services.ConfigureClassAware<SmartCacheCoreOptions>(configuration.GetSection("Diginsight:SmartCache"));

            var smartCacheBuilder = services
                .AddSmartCache(configuration, environment, loggerFactory)
                .AddHttp();

            IConfigurationSection smartCacheServiceBusConfiguration = configuration.GetSection("Diginsight:SmartCache:ServiceBus");
            if (!string.IsNullOrEmpty(smartCacheServiceBusConfiguration[nameof(SmartCacheServiceBusOptions.ConnectionString)]) &&
                !string.IsNullOrEmpty(smartCacheServiceBusConfiguration[nameof(SmartCacheServiceBusOptions.TopicName)]))
            {
                smartCacheBuilder.SetServiceBusCompanion(
                    static (c, _) =>
                    {
                        IConfiguration sbc = c.GetSection("Diginsight:SmartCache:ServiceBus");
                        return !string.IsNullOrEmpty(sbc[nameof(SmartCacheServiceBusOptions.ConnectionString)])
                            && !string.IsNullOrEmpty(sbc[nameof(SmartCacheServiceBusOptions.TopicName)]);
                    },
                    sbo =>
                    {
                        configuration.GetSection("Diginsight:SmartCache:ServiceBus").Bind(sbo);
                        sbo.SubscriptionName = SmartCacheServiceBusSubscriptionName;
                    });
            }
            services.TryAddSingleton<ICacheKeyProvider, MyCacheKeyProvider>();
            //services.TryAddSingleton<IActivityTagger, ActivityTagger>();

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            builder.Host.UseDiginsightServiceProvider(true);
            app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }

        app.Run();
    }
}
