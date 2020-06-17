using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Autorizacao
{
    public class LoginModel
    {
        [Required]
        public string EmailOrLogin { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Lembrar?")]
        public bool RememberMe { get; set; }

        public string CaminhoIconeEmpresa {get; set;}

    }
}


