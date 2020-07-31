using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using NewBISReports.Models.Autorizacao;
using Microsoft.Extensions.Configuration;
using NewBISReports.Models;

namespace NewBISReports.Controllers
{
    //todas as páginas necessitarão de login, exceto a de login
    [AllowAnonymous]
    public class AutorizacaoController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly  UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public AutorizacaoController(SignInManager<ApplicationUser> signInManager,      
                                 IdentityErrorDescriber describer,
                                 UserManager<ApplicationUser> userManager,
                                 IConfiguration configuration
                               
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            ErrorDescriber = describer ?? throw new ArgumentNullException(nameof(describer));
            _configuration = configuration;

        }

        [HttpGet]
        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            //Registrar Url de retorno interna
            returnUrl = returnUrl ?? Url.Content("~/");

            TempData["ReturnUrl"] = returnUrl;

            var vm = new LoginModel();
            //verifica para qual cliente está sendo configurado
            var nomeCliente  = _configuration.GetSection("Default")["Name"];
            vm.CaminhoIconeEmpresa = _configuration.GetSection(nomeCliente)["ImagePath"]; 

            await Task.CompletedTask;
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(CancellationToken cancellationToken, LoginModel vm, string returnUrl = null)
        {
            //var _userStore = new AdLdapUserStore(_ldapOptions, ErrorDescriber);
            if (ModelState.IsValid)
            {
                //redirecionamento local
                returnUrl = returnUrl ?? Url.Content("~/");

                //Para realizar login com email ou username, basta dividir a string
                //Confiamos que o formulário só deixou criar usuarios com email bem formado, então basta fazer um split na @
                var userName = vm.EmailOrLogin.Split('@',StringSplitOptions.None)[0];
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(userName, vm.Password, vm.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //Testar se a senha deve ser modificada MustChangePassword
                    if (User.HasClaim(x => x.Type =="MustChangePassword")){
                        
                        var user = await _userManager.FindByNameAsync(vm.EmailOrLogin);
                        return RedirectToAction("ChangePassword","Administracao", new {Id = user.Id});
                    }else{
                        //Operação normal
                        return LocalRedirect(returnUrl); 
                    }
                  
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário ou Senha Inválidos");
                    return View(vm);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(vm);
        }

        
        public async Task<IActionResult> LogoutAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        //Acesso Negado
        
        public async Task<IActionResult> AccessDenied()
        {
            //testa se o erro foi simplesmente porcausa de claim, e re-direciona para a troca de senha de forma apropriada
            if (User.HasClaim(x => x.Type =="MustChangePassword")){
                        
                ApplicationUser user = await _userManager.GetUserAsync(User);
                return RedirectToAction("ChangePassword","Administracao", new {Id = user.Id});
            }
            return View();
        }
    }
}