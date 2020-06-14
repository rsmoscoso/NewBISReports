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

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(vm.EmailOrLogin, vm.Password, vm.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);                   
                }
                //Não estamos utilizando estas funcionalidades
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = vm.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToPage("./Lockout");
                //}
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
        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}