using System;
using Demo_API.CosmosDB;
using Demo_API.Data;
using Demo_API.Wikipedia;
using Microsoft.AspNetCore.Mvc;

namespace Demo_API
{
	public class Startup
	{

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSingleton<IDataBaseConnection>(
                new CosmosDBConnection(this.Configuration.GetSection("CosmosDB").GetValue<string>("ConnectionString")));

            services.AddSingleton<IDataEnrichment>(
                new WikipediaDataEnrichment(this.Configuration.GetSection("Wikipedia").GetValue<string>("uri")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(); 
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
	
}

