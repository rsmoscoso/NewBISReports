using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewBISReports.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using NewBISReports.Models.Autorizacao;

namespace NewBISReports
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //criar banco automaticamente caso não exista e caso tenha login habilitado
            //Ler configurações do appsettings aqui
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            //verifica para qual cliente está sendo configurado
            var nomeCliente = config["Default:Name"];

            //Verifica se o módulo de login estará habilitado
            var isLogin = config[nomeCliente+":useLogin"];
            if(isLogin == "true"){
                //recupera a senha inicial do usuario admin
                var adminPassword = config[nomeCliente+":adminDefaultPassword"];
                //cria o WebHost se assegurando de que o banco de login existe
                var host = CreateWebHostBuilder(args).Build();
                //cria o banco se ele não existir
                await CreateDbIfNotExists(host, adminPassword);
                host.Run();
            }else{
                CreateWebHostBuilder(args).Build().Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        
        private static async Task CreateDbIfNotExists(IWebHost host, string adminPassword)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DbContexHzLogin>();
                    //context.Database.Migrate();
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                    return;
                }

                try{
                    //se a criação/existencia do banco estiver assegurado, cehcar se o usuário admin já existe
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    //verifica se já existe
                    var adminUser = await userManager.FindByNameAsync( "admin");
                    if (adminUser == null){
                        //caso não exisata criar
                        var newAdminUser = new IdentityUser { UserName = "admin", Email = "admin@admin" }; 
                        var result = await userManager.CreateAsync(newAdminUser,adminPassword);
                        if (result.Succeeded)
                        {
                            //e atribuir a Claim de Admin             
                            var adminClaim = new Claim(Claims.Admin,"Admin");
                            await userManager.AddClaimAsync(newAdminUser, adminClaim);
                            StreamWriter w = new StreamWriter("erro.txt", true);
                            w.WriteLine("Usuario Admin criado com sucesso");
                            w.Close();
                            w = null;                       
                        }                
                    }                
                }
                catch{
                    StreamWriter w = new StreamWriter("erro.txt", true);
                    w.WriteLine("Usuario não pôde ser criado");
                    w.Close();
                    w = null;
                }

            }
        }
    }
}
