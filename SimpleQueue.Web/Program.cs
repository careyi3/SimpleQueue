using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SimpleQueue.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(new string[] { "http://localhost:5001" });
                });
    }
}
