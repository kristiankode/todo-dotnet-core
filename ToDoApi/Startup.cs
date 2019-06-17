using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoApi.Dao;
using ToDoApi.Models;
using ToDoApi.Repository;

namespace ToDoApi
{
    public class Startup
    {
        private const string AllowLocalhostCors = "AllowLocalhost";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase());

            // Add framework services.
            services.AddMvc();
            services.AddCors(options =>
            {
                options.AddPolicy(AllowLocalhostCors, 
                    builder => { builder.WithOrigins("http://localhost:3000"); });
            });

            services.AddSingleton<ITodoRepository, TodoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddConsole(LogLevel.Warning);
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(AllowLocalhostCors);

                var repository = app.ApplicationServices.GetService<ITodoRepository>();
                InitializeDatabase(repository);
            }

            app.UseMvc();
        }

        public static Todo GetTestTodo()
        {
            return new Todo
            {
                Name = "Test Todo 1"
            };
        }

        private static void InitializeDatabase(ITodoRepository repo)
        {
        }
    }
}