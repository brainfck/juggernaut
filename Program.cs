using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace juggernaut
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            if(args.Length > 0)
                await serviceProvider.GetService<App>().Run(args[0]);
            else
                await serviceProvider.GetService<App>().Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            
            services.AddSingleton(config);
            services.AddSingleton<IImageDownloader, ImageDownloader>();
            services.AddTransient<App>();

            services.AddLogging(configure => configure.AddConsole())
                .AddTransient<ImageDownloader>();

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, true);

            return builder.Build();
        }
    }
}
