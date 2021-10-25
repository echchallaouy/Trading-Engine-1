using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;
using TradingEngineServer.Exchange;

namespace TradingEngineServer.Core
{
    public sealed class TradingEngineHostBuilder
    {
        public static IHost BuildTradingEngine()
            => Host.CreateDefaultBuilder().ConfigureServices((hostContext, services)
                =>
                {
                    // Start with configurations.
                    services.AddOptions();
                    services.Configure<TradingEngineServerConfiguration>(hostContext.Configuration.GetSection(nameof(TradingEngineServerConfiguration)));
                    services.Configure<LoggerConfiguration>(hostContext.Configuration.GetSection(nameof(LoggerConfiguration)));
                    services.Configure<TradingExchangeConfiguration>(hostContext.Configuration.GetSection(nameof(TradingExchangeConfiguration)));

                    // Add singleton objects.
                    services.AddSingleton<ITextLogger, TextLogger>();
                    services.AddSingleton<ITradingEngine, TradingEngineServer>();
                    services.AddSingleton<ITradingExchange, TradingExchange>();

                    // Add hosted service.
                    services.AddHostedService<TradingEngineServer>();
                }).Build();
    }
}
