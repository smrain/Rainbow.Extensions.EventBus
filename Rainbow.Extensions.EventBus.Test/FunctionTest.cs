using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Rainbow.Extensions.EventBus.Abstractions;
using Rainbow.Extensions.EventBus.Abstractions.Events;
using Rainbow.Extensions.EventBus.RabbitMQ;
using Serilog;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Rainbow.Extensions.EventBus.Test
{
    public class FunctionTest
    {
        public IConfiguration Configuration { get; private set; }

        public virtual IServiceProvider ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .CreateLogger();


            var serviceProvider = new ServiceCollection()
                .AddSingleton(Configuration)
                .AddLogging(x => x.SetMinimumLevel(LogLevel.Trace))
                .AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = Configuration["EventBusConnection"],
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                    {
                        factory.UserName = Configuration["EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                    {
                        factory.Password = Configuration["EventBusPassword"];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                })
                .AddEventBus(Configuration);
                //.BuildServiceProvider();

            //configure autofac
            var container = new ContainerBuilder();
            container.Populate(serviceProvider);

            container.RegisterModule(new ApplicationModule());

            return new AutofacServiceProvider(container.Build());
        }


        [Fact]
        void TestEventBus()
        {
            IServiceProvider provider = this.ConfigureServices();

            var eventBus = provider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<TestIntegrationEvent, IIntegrationEventHandler<TestIntegrationEvent>>();


            var @event = new TestIntegrationEvent("²âÊÔÊÂ¼þ");

            eventBus.Publish(@event);
        }

    }

    public class TestIntegrationEvent : IntegrationEvent
    {
        public string Remark { get; }

        public TestIntegrationEvent(string remark) => Remark = remark;
    }

    public class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public async Task Handle(TestIntegrationEvent @event)
        {
            Log.Debug(@event.Remark);

            await Task.CompletedTask;
        }
    }
   
    
    public class ApplicationModule : Autofac.Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(TestIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }


    internal static class MiddlewareExtensionsMethods
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }

}
