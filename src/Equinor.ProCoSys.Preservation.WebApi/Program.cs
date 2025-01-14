using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

namespace Equinor.ProCoSys.Preservation.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var settings = config.Build();
                    var azConfig = settings.GetValue<bool>("UseAzureAppConfiguration");
                    if (azConfig)
                    {
                        config.AddAzureAppConfiguration(options =>
                        {
                            var configAddressString = settings["AppConfiguration:BaseAddress"];
                            var configAddress = new Uri(configAddressString!);
                            
                            options.Connect(configAddress, new DefaultAzureCredential())
                                .ConfigureKeyVault(kv =>
                                {
                                    kv.SetCredential(new DefaultAzureCredential());
                                })
                                .Select(KeyFilter.Any)
                                .Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName)
                                .ConfigureRefresh(refreshOptions =>
                                {
                                    refreshOptions.Register("Sentinel", true);
                                    refreshOptions.SetRefreshInterval(TimeSpan.FromSeconds(30));
                                });
                        });
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.Limits.MaxRequestBodySize = null;
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
