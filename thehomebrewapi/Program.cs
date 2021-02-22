using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;
using System.IO;
using thehomebrewapi.Contexts;

namespace thehomebrewapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder
                .ConfigureNLog("nlog.config")
                .GetCurrentClassLogger();
            try
            {
                logger.Info("Initializing application ...");
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetService<HomeBrewContext>();

                        // for demo purposes, delete the database and migrate on startup so
                        // we can start with a clean slate
#if DEBUG
                        context.Database.EnsureDeleted();
                        context.Database.Migrate();
#endif
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex, "An error occurred while migrating the database.");
                        throw;
                    }
                }

                // run the web app
                host.Run();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Application stopped because of exception.");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false)
                                .AddJsonFile("hostsettings.json", optional: true)
                                .AddCommandLine(args)
                                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:5000");
                    webBuilder.UseConfiguration(config);
                    webBuilder.UseStartup<Startup>().UseNLog();
                });

            //Host.CreateDefaultBuilder(args)
            //    .ConfigureWebHostDefaults(webBuilder =>
            //    {
            //        webBuilder.UseStartup<Startup>().UseNLog();
            //    });
        }
    }
}
