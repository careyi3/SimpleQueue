using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleQueue.Server.Services;
using SimpleQueue.Server.DataAccess;
using SimpleQueue.Common.DataAccess;
using SimpleQueue.Server.MemoryQueue;

namespace SimpleQueue.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ApplicationDbContextFactory>();
            services.AddSingleton<QueueManager>();
            services.AddTransient<QueueService>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("SimpleQueue.Server"))
            );
            services.AddCors(options =>
                options.AddPolicy("cors", b => b.WithOrigins("http://localhost:5001"))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("cors");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Setup(app);
        }

        private void Setup(IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<QueueManager>().Setup();
        }
    }
}
