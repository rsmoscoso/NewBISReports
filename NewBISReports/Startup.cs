using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace NewBISReports
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
            var cultureInfo = new CultureInfo("pt-BR");
            cultureInfo.NumberFormat.CurrencySymbol = "R$";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //        services.AddIdentity<ApplicationUser, IdentityRole>()
            //.AddEntityFrameworkStores<DbContextFinanceiro>()
            //.AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false; //Senha deve conter numeros?
                options.Password.RequireLowercase = false; //Senha deve conter letras minusculas?
                options.Password.RequireNonAlphanumeric = false; //Senha deve conter caracteres alpha-numéricos?
                options.Password.RequireUppercase = false; //Senha deve conter letras maiusculas?
                options.Password.RequiredLength = 6; //Tamanho minimo
                options.Password.RequiredUniqueChars = 1; //Numeros de caracteres unicos na senha (para nao repetir o mesmo caractere muitas vezes)

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                //// Cookie settings
                //options.Cookie.Name = "auth_cookie";W
                //options.Cookie.SameSite = SameSiteMode.None;
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = new PathString("/Autenticacao/Login");
                options.LogoutPath = new PathString("/Autenticacao/Login");
                options.AccessDeniedPath = new PathString("/Autenticacao/AcessoNegado");

                //options.SlidingExpiration = true;
            });





            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //encaminhamento de cabeçalho https para não perder o host de destino quando houver o 
            //desempacotamento SSL no proxy reverso
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                //options.KnownProxies.Add(IPAddress.Parse("10.20.30.115"));
            });

            services.AddLogging();
            services.AddCors();
//            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePages();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}