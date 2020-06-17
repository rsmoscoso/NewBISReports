
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NewBISReports.Models.Administracao{

    public class ChangePasswordViewModel{

    //Propriedades de HttpGet
    //Lista de usuários para troca de senha

        public string FullName { get; set; }
        public string Id {get; set;}

        [Required(ErrorMessage = "Digite a senha!")]
        [StringLength(100, ErrorMessage = "A senha deve ter no mínimo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação da senha não são iguais.")]
        public string ConfirmPassword { get; set; }


    }
 }