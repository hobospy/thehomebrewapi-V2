using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using thehomebrewapi.Contexts;
using thehomebrewapi.Services;

namespace thehomebrewapi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            const string PROBLEM_JSON = "application/problems+json";

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            })
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions( setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        // Create a problem details object
                        var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                            context.HttpContext,
                            context.ModelState);

                        // Add additional info not added by default
                        problemDetails.Detail = "See the errors field for details.";
                        problemDetails.Instance = context.HttpContext.Request.Path;

                        var actionExecutingContext =
                            context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                        // If there are modelstate errros and all arguments were correctly
                        // found/parsed we're dealing with validation errors
                        if ((context.ModelState.ErrorCount > 0) && 
                            (actionExecutingContext?.ActionArguments.Count ==
                            context.ActionDescriptor.Parameters.Count))
                        {
                            problemDetails.Type = "insert http link here for help page";
                            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                            problemDetails.Title = "One or more validation errors occurred.";

                            return new UnprocessableEntityObjectResult(problemDetails)
                            {
                                ContentTypes = { PROBLEM_JSON }
                            };
                        }

                        // If one of the arguments wasn't correctly found / couldn't be parsed
                        // we're deailing with null/unparseable input
                        problemDetails.Status = StatusCodes.Status400BadRequest;
                        problemDetails.Title = "One or more errors on input occurred.";

                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = { PROBLEM_JSON }
                        };
                    };
                });

            services.Configure<MvcOptions>(config =>
           {
               var newtonsoftJsonOutputFormatter = config.OutputFormatters
               .OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();

               if (newtonsoftJsonOutputFormatter != null)
               {
                   newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add(_configuration["homeBrewApiMediaTypes:hateoas"]);
               }
           });

            // Register PropertyMappingService
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            var connectionString = _configuration["connectionStrings:homeBrewDBConnectionString"];
            services.AddDbContext<HomeBrewContext>(o =>
            {
                o.UseSqlServer(connectionString);
            });

            services.AddScoped<IHomeBrewRepository, HomeBrewRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault occurred.  Try again later.");
                    });
                });
            }

            app.UseStatusCodePages();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
