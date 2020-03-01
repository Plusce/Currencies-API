using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Autofac;
using Currencies.Api.Infrastructure.DependencyInjection;
using Currencies.Api.Infrastructure.Middleware;
using Currencies.App.UseCases.GetExchangeRate;
using Currencies.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Currencies.Api
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
            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddMediatR(Assembly.GetAssembly(typeof(GetExchangeRateQueryHandler)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Currencies API", Version = "v1" });
                c.IncludeXmlComments(GetXmlDocumentationFilePath(Assembly.GetExecutingAssembly()));
                c.IncludeXmlComments(GetXmlDocumentationFilePath(typeof(GetExchangeRateQuery).Assembly));
            });

            var connectionString = Configuration.GetValue<string>("ConnectionString");
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetXmlDocumentationFilePath(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            var xmlDocumentationFileName = $"{assemblyName}.XML";
            var xmlDocumentationFilePath = Path.Combine(AppContext.BaseDirectory, xmlDocumentationFileName);
            return xmlDocumentationFilePath;
        }
    }
}
