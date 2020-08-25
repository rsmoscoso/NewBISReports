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
using Microsoft.EntityFrameworkCore;
using NewBISReports.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using NewBISReports.Models.Autorizacao;
using NewBISReports.Models;

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

            //Adiciona as configurações de Login
            //verifica para qual cliente está sendo configurado
            var nomeCliente = Configuration["Default:Name"];
            //Verifica se o módulo de login estará habilitado
            var isLogin = Configuration[nomeCliente+":useLogin"];            
            //Diogo - Adicionando Identity para uso com o banco hzLogin 
            //TODO  -> adicionando Identity apontando para uma classe vazia de userStore, com policies inócuas,
            // para ficar configurável se a pessoa quer ou não utilizar o login de forma compatível
            if(isLogin =="true"){
                string hzLoginConnectionString = Configuration.GetConnectionString("DbContextHzLogin");
                services.AddDbContext<DbContexHzLogin>((optBuilder) =>
                {
                    optBuilder.UseSqlServer(hzLoginConnectionString);
                });

                // services.AddIdentity<ApplicationUser, IdentityRole>()
                // .AddEntityFrameworkStores<DbContexHzLogin>()
                // .AddDefaultTokenProviders();
                services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<DbContexHzLogin>()
                .AddDefaultTokenProviders();
            }else{
                //Faz um override na classe UserStore
                services.AddScoped<IUserStore<ApplicationUser>, EmptyUserStore>();
                services.AddScoped<IRoleStore<IdentityRole>, EmptyRoleStore>();
                //Adiciona um Identity "dummy" sem qualquer banco de dados atrelado
                services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddSignInManager();
            }

            services.AddHttpContextAccessor();


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
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.";
                options.User.RequireUniqueEmail = false;
            });

            //Permite deslogar um usuario instantaneamente trocando a securitystamp dele
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;   
            });

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            })
            .AddIdentityCookies();

            //TODO: construir as rotas de login/logout/accessdenied
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Autorizacao/LoginAsync";
                options.LogoutPath = $"/Autorizacao/LogoutAsync";
                //no accessdenied, devemos testar se há a claim de troca de senha MustChangePassword, e redirecionar para a troca de senha
                options.AccessDeniedPath = $"/Autorizacao/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

            //políticas de acesso: Apenas Admin, User e Anonymous

            //Politicas básicas 
            services.AddAuthorization(options =>{
                //As duas controlam se o usuario precisa trocar a senha
                //Quando a política é apenas para admin acessar
                options.AddPolicy("AcessoAdmin", pB => pB.RequireAssertion(c =>(c.User.HasClaim(x => x.Type == Claims.Admin) && !c.User.HasClaim(x => x.Type == "MustChangePassword"))));
                //Partes acessíveis pelos usuários ou admins
                options.AddPolicy("AcessoUsuario", pB => pB.RequireAssertion(c => (c.User.HasClaim(x => x.Type == Claims.Admin) || c.User.HasClaim(x => x.Type == Claims.Usuario)) && !c.User.HasClaim(x => x.Type == "MustChangePassword")));
                //reset de senha
                options.AddPolicy("CriarNovaSenha", pB => pB.RequireAssertion(c => (c.User.HasClaim(x => x.Type == Claims.Admin) || c.User.HasClaim(x => x.Type == Claims.Usuario)) && c.User.HasClaim(x => x.Type == "MustChangePassword")));
            });

            //Politica de filtro global para usuários que necessitam trocar a senha


            //habilia o middleware mvc, com filtro de allowanony mous se o login estiver desabilitado
            if(isLogin == "true"){
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            }else{
                services.AddMvc(opts =>
                {
                    //filtro global de allowanonymous
                    opts.Filters.Add(new AllowAnonymousFilter());
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            }
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

            //ria o Singleton de criação de menu lateral do site
            services.AddSingleton<ArvoreOpcoes>();

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

            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}