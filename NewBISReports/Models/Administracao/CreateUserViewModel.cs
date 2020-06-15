
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NewBISReports.Models.Administracao{

    public class CreateUserViewModel{

    //Propriedades de HttpGet
    //Lista de usuários para troca de senha
    public List<SelectListItem> UserTypes { get; set; }

    //Propriedades de HttpPost
    //Novo Usuário
        [Required(ErrorMessage ="Email é obrigatório!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Digite o nome de usuário!")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        // [Required(ErrorMessage = "Digite a senha!")]
        // [StringLength(100, ErrorMessage = "A senha deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        // [DataType(DataType.Password)]
        // [Display(Name = "Password")]
        // public string Password { get; set; }
        
        // [DataType(DataType.Password)]
        // [Display(Name = "Confirm password")]
        // [Compare("Password", ErrorMessage = "A senha e a confirmação da senha não são iguais.")]
        // public string ConfirmPassword { get; set; }
        public string UserType {get;set;}

    }
 }