using AnalyzerService;
using AnalyzerService.Abstraction;
using ImageHandler;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ImageHandler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddSingleton<IAnalyzerService, ComputerVisionAnalyzerService>();
        }
    }
}