using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NewBISReports.Services
{
    /// <summary>
    /// Provides extension methods for configuring the DI container
    /// </summary>`
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddBisRestApiAccess(this IServiceCollection services, IConfiguration Configuration)
        {

            //Classe base de consumo de APIs
            services.AddTransient<ApiClientBase>();

            //Banco de dados extendido para o infraSpeak
            //Configração básica das requisições da API
            services.AddHttpClient<IBisApiRestAccessClient, BisApiRestAccessClient>(client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("Servico").GetSection("Endereco").Value);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            

            return services;
        }
    }

}


