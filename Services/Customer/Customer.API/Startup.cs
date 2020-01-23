using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Abstractions;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBusAzure;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBusRabbitMQ;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Client;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation.Impl;
using XJeunot.PhysicalStoreApps.Services.Customer.API.IntegrationEvents.EventHandling;
using XJeunot.PhysicalStoreApps.Services.Customer.API.IntegrationEvents.Events;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            /*
             * Add Authentication.
             */
            services.AddMvcCore()
                .AddAuthorization()
                .AddNewtonsoftJson();
            services
                .AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetSection("AuthenticationBearer:Authority").Value;
                    options.RequireHttpsMetadata = true;
                    options.ApiName = Configuration.GetSection("AuthenticationBearer:ApiName").Value;
                    options.ApiSecret = Configuration.GetSection("AuthenticationBearer:ApiSecret").Value;
                });

            /*
             * EventBus Setup.
             */
            if (Configuration["ClusterMode"] == Program.CLUSTER_AZURE)
            {
                /*
                 * Azure Service Bus.
                 */
                services.AddSingleton<IAzureServiceBusPersisterConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<AzureServiceBusPersisterConnection>>();

                    var serviceBusConnectionString = Configuration.GetSection("Azure:EventBusConnection").Value;
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                    if ((Configuration.GetSection("Azure:EventBusEntityPath").Exists()) &&
                        (!string.IsNullOrEmpty(Configuration.GetSection("Azure:EventBusEntityPath").Value)))
                    {
                        serviceBusConnection.EntityPath = Configuration.GetSection("Azure:EventBusEntityPath").Value;
                    }

                    return new AzureServiceBusPersisterConnection(serviceBusConnection, logger);
                });
                services.AddSingleton<IEventBus, EventBusAzureServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IAzureServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusAzureServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    var subscriptionClientName = Configuration.GetSection("Azure:SubscriptionClientName").Value;

                    return new EventBusAzureServiceBus(serviceBusPersisterConnection, logger,
                        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
                });
            }
            else
            {
                /*
                 * RabbitMq.
                 */
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory() { HostName = Configuration.GetSection("RabbitMqBus:Connection").Value };
                    factory.UserName = Configuration.GetSection("RabbitMqBus:User").Value;
                    factory.Password = Configuration.GetSection("RabbitMqBus:Password").Value;

                    var retryCount = 5;
                    if ((Configuration.GetSection("RabbitMqBus:RetryCount").Exists()) &&
                        (!string.IsNullOrEmpty(Configuration.GetSection("RabbitMqBus:RetryCount").Value)))
                        retryCount = int.Parse(Configuration.GetSection("RabbitMqBus:RetryCount").Value);

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQConnexion = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusAboManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;
                    if ((Configuration.GetSection("RabbitMqBus:RetryCount").Exists()) &&
                        (!string.IsNullOrEmpty(Configuration.GetSection("RabbitMqBus:RetryCount").Value)))
                        retryCount = int.Parse(Configuration.GetSection("RabbitMqBus:RetryCount").Value);

                    var queueName = Configuration.GetSection("RabbitMqBus:QueueName").Value;

                    return new EventBusRabbitMQ(rabbitMQConnexion, logger, iLifetimeScope,
                        eventBusAboManager, queueName, retryCount);
                });
            }
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<CheckOutEventHandler>();

            /*
             * MongoDB Setup.
             */
            services.Configure<MongoDbConfig>(Options =>
            {
                Options.IsAzure = (Configuration["ClusterMode"] == Program.CLUSTER_AZURE);

                Options.ConnectionString = Configuration.GetSection("MongoDbConfig:ConnectionString").Value;
                Options.Datebase = Configuration.GetSection("MongoDbConfig:Datebase").Value;

                Options.User = Configuration.GetSection("MongoDbConfig:User").Value;
                Options.Password = Configuration.GetSection("MongoDbConfig:Password").Value;
            });
            services.AddSingleton<IMongoDbClient>(sp =>
            {
                ILogger<MongoDbClient> logger = sp.GetRequiredService<ILogger<MongoDbClient>>();
                IOptions<MongoDbConfig> options = sp.GetRequiredService<IOptions<MongoDbConfig>>();
                return new MongoDbClient(options, logger);
            });
            services.AddSingleton<ICustomerServices>(sp =>
            {
                return new CustomerServices(sp.GetRequiredService<IMongoDbClient>());
            });

            /*
             * Custom Validator Setup.
             */
            services.AddSingleton<ICustomerFlowValid>(sp => { return new CustomerFlowValid(); });

            /*
             * Register the Swagger generator.
             */
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer.API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetSection("AuthenticationBearer:Authority").Value}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration.GetSection("AuthenticationBearer:Authority").Value}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "customer", "Access Customer.API" }
                            }
                        }
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new[]
                        {
                            "customer"
                        }
                    }
                });
            });

            /*
             * Configuration de AutoFac.
             * Inversion de contrôle.
             */
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            /*
             * Use Authentication.
             */
            app.UseAuthentication();

            app.UseHttpsRedirection();

            // Configuration bus Subcription.
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<CheckOutEvent, CheckOutEventHandler>();
            eventBus.ActivateConsumerChannel();

            // Swagger.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer.API V1");
            });
        }
    }
}
