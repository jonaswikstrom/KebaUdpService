using System;
using System.Threading;
using KebaUdpService.MessageHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KebaUdpService
{

    public class Application
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger(typeof(Application));

            try
            {
                var mqttClient = serviceProvider.GetService<IMqttClient>();
                mqttClient.Connect();

                serviceProvider.GetServices<IKebaMessageHandler>();
                var kebaConnector = serviceProvider.GetService<IKebaConnector>();
                kebaConnector.Connect();

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }

            logger.LogInformation("Application exits...");


        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add logging
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole()
                .AddDebug());

            serviceCollection.AddLogging();

            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IMqttClient, MqttClient>();
            serviceCollection.AddSingleton<IKebaConnector>(p =>
                new KebaConnector(p.GetRequiredService<ILoggerFactory>(), "192.168.1.122", 7090));
            serviceCollection.AddSingleton<IKebaMessageHandler, EPresMessageHandler>();
            serviceCollection.AddSingleton<IKebaMessageHandler, Report2MessageHandler>();
            serviceCollection.AddSingleton<IKebaMessageHandler, Report3MessageHandler>();
            serviceCollection.AddSingleton<IKebaMessageHandler, PlugStateMessageHandler>();
            serviceCollection.AddSingleton<IKebaMessageHandler, ChargingStateMessageHandler>();
        }
    }




}
