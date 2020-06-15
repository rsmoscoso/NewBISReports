
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NewBISReports.Models.Administracao{

    public class ResetPasswordViewModel{

    //Propriedades de HttpGet
    //Lista de usuários para troca de senha
    public List<SelectListItem> Users { get; set; }
    public string Id {get;set;}

    }
 }