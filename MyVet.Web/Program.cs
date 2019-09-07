using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MyVet.Web.Data;

namespace MyVet.Web
{
    public class Program
    {
        /*original: el codigo orinal fue sustituido para poder insertar informacion de prueba en la base de datos 
        por medio de la clase seedDb.  Esto asegura que las tablas de la base de datos ya tenga algunos ejemplos para trabajar*/
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();    *original
            var host = CreateWebHostBuilder(args).Build();
            Runseeding(host);
            host.Run();
        }
        private static void Runseeding(IWebHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
                using (var scope=scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<SeedDb>();
                seeder.SeedAsync().Wait();
            } 
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();  //*original

        
    }
}
