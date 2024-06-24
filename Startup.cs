using ImageHandler;
using System;
using AnalyzerService.Abstraction
//type of startup object
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
