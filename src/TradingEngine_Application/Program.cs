using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TradingEngineServer.Core;

using var engine = TradingEngineHostBuilder.BuildTradingEngine();
TradingEngineServiceProvider.ServiceProvider = engine.Services;
{
    using var scope = TradingEngineServiceProvider.ServiceProvider.CreateScope();
    await engine.RunAsync(default).ConfigureAwait(false);
}