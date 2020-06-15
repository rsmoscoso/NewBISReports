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
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using NewBISReports.Models.Autorizacao;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewBISReports.Models.Administracao;
using System.Security.Claims;

namespace NewBISReports.Controllers
{
    //todas as páginas necessitarão de login, exceto a de login
    public class AdministracaoController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly  UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public string EmpresaDefault {get; set;}

        [TempData]
        public string ErrorMessage { get; set; }

        public AdministracaoController(SignInManager<ApplicationUser> signInManager,      
                                 IdentityErrorDescriber describer,
                                 UserManager<ApplicationUser> userManager,
                                 IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            ErrorDescriber = describer ?? throw new ArgumentNullException(nameof(describer));
            _configuration = configuration;
            //Salva a empresa configurada para facilitar a recuperação de parametros
            EmpresaDefault = _configuration.GetSection("Default")["Name"];

        }

        [HttpGet,Authorize("AcessoAdmin")]
        public  IActionResult CreateUser()
        {
            //inicializa a ViewModel
            var vm = new CreateUserViewModel();

            //transforma a lista de propriedades em uma selectlist
            vm.UserTypes = typeof(Claims).GetFields(BindingFlags.Static | BindingFlags.Public)
                                 .Where(x => x.IsLiteral && !x.IsInitOnly)
                                 .Select(x => new SelectListItem{
                Value =  x.GetValue(null).ToString(),
                Text =  x.GetValue(null).ToString()
            }).ToList();
            

            return View(vm);
        }
        [HttpGet,Authorize("CriarNovaSenha")]
        public async Task<IActionResult> ChangePassword(string Id)
        {
            //preenche a viewmodel com o Nome do usuário, e seu Id no banco, para voltar pelo post
            var vm = new ChangePasswordViewModel(); 
            var user = await _userManager.FindByIdAsync(Id);
            vm.Id = user.Id;
            vm.FullName = user.FullName;

            return View(vm);
        }

        
        [HttpGet,Authorize("AcessoAdmin")]
        public async Task<IActionResult> ResetUserPassword()
        {
            //preenche a viewmodel com o Nome do usuário, e seu Id no banco, para voltar pelo post
            var vm = new ResetPasswordViewModel(); 
            //recupera todos os usuarios
            vm.Users = await _userManager.Users.Select(x => new SelectListItem{
                Text = x.FullName,
                Value = x.Id
            }).ToListAsync();

            return View(vm);
        }

        [HttpPost,Authorize("AcessoAdmin")]
        public async Task<IActionResult> ResetUserPassword(ResetPasswordViewModel vm)
        {
            if(ModelState.IsValid && vm.Id != "0")
            {            
                //recupera o usuario do banco
                var user = await _userManager.FindByIdAsync(vm.Id);

                //aplica a senha padrão
                var newPassword = _configuration.GetSection(EmpresaDefault)["userDefualtPassword"];
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
                var Result= await _userManager.UpdateAsync(user);
                if(Result.Succeeded)
                {
                    //adiciona a claim de mudança de senha
                    var pwdClaim = new Claim("MustChangePassword","true");
                    await _userManager.AddClaimAsync(user, pwdClaim);
                    //derruba o login do usuario para que ele faça o login novamente
                    await _userManager.UpdateSecurityStampAsync(user);
                    return RedirectToAction(nameof(Resultado), new{ Message = "Senha do usuário \""+user.FullName+"\" reiniciada com sucesso para: \""+ newPassword+"\". O usuário deverá criar uma nova senha no próximo acesso."}); //Redireciona para rota padrão (Home/Index)
                }else{
                    return RedirectToAction(nameof(Resultado), new{ Message = "Erro: \""+Result.Errors.ToString()+"\" Entre em contato com o Administrador e relate esta mensagem." }); 
                }
            }else{
                //preenche a viewmodel com o Nome do usuário, e seu Id no banco, para voltar pelo post
                //recupera todos os usuarios
                vm.Users = await _userManager.Users.Select(x => new SelectListItem{
                    Text = x.FullName,
                    Value = x.Id
                }).ToListAsync();

                ModelState.AddModelError(string.Empty,"Por favor, selecione um Usuário");
                return View(vm);
            }

        }

        [HttpPost,Authorize("CriarNovaSenha")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            //recupera o usuario do banco
            var user = await _userManager.FindByIdAsync(vm.Id);

            //aplica a nova senha
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, vm.ConfirmPassword);
            var Result= await _userManager.UpdateAsync(user);
            if(Result.Succeeded)
            {
                //retira a claim de MustChangePassword
                var pwdClaim = new Claim("MustChangePassword","true");
                await _userManager.RemoveClaimAsync(user, pwdClaim);
                 //derruba o login do usuario para que ele faça o login novamente
                await _userManager.UpdateSecurityStampAsync(user);
                return RedirectToAction(nameof(Resultado), new{ Message = "Senha Modificada com Sucesso, realize o login com a nova senha"}); //Redireciona para rota padrão (Home/Index)
            }else{
                 return RedirectToAction(nameof(Resultado), new{ Message = "Erro: \""+Result.Errors.ToString()+"\" Entre em contato com o Administrador e relate esta mensagem." }); 
            }
        }

        [HttpGet,AllowAnonymous]
        public IActionResult Resultado(string Message)
        {
            ViewBag.Message = Message;
            return View();
        }
        

        [HttpPost,Authorize("AcessoAdmin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel newUser )
        {
            //aqui criar uma nova senha para o usuário, colocar a senha padrão no appssetings de cada cliente
            //retornar em uma tela a senha criada
            //usar o campo de lockout enable para reset de senha no priomeiro acesso
            if (ModelState.IsValid)
            {
                 var user = new ApplicationUser { 
                     //retira o username da string do email antes da @
                     UserName = newUser.Email.Split('@',StringSplitOptions.None)[0],
                     Email = newUser.Email, 
                     FullName = newUser.UserName,
                     }; //Cria um usuário com os dados vindos do front end
                 //Cria o usuário com a senha padrão para a Empresa
                var newPassword = _configuration.GetSection(EmpresaDefault)["userDefualtPassword"];
                try{
                    var result = await _userManager.CreateAsync(user, newPassword); 
                    if (result.Succeeded)
                    {              
                        //adiciona a Claim ao usuário     
                        var nameClaim = new Claim(newUser.UserType,newUser.UserType);
                        var pwdClaim = new Claim("MustChangePassword","true");
                        await _userManager.AddClaimAsync(user, nameClaim);
                        //o usuario deve criar uma senha em seu primeiro login
                        await _userManager.AddClaimAsync(user, pwdClaim);

                        return RedirectToAction(nameof(Resultado), new{ Message = "Usuário "+ user.FullName + " criado com sucesso, com a senha: \""+ newPassword +"\". Ela deverá ser trocada em seu primeiro acesso."}); //Redireciona para rota padrão (Home/Index)
                    }else{
                        return RedirectToAction(nameof(Resultado), new{ Message = "Erro: \""+result.Errors.ToString()+"\" Entre em contato com o Administrador e relate esta mensagem." }); 
                    }
                } catch(Exception ex){
                    return RedirectToAction(nameof(Resultado), new{ Message = "Erro: \""+ex.Message+"\" Entre em contato com o Administrador e relate esta mensagem." }); 
                }
            }else{
                return RedirectToAction(nameof(Resultado), new{ Message = "Algo deu errado! Tente novamente mais tarde"}); //Redireciona para rota padrão (Home/Index)
            }
        }

    }
}