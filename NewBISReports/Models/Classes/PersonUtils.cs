using HzBISCommands;
using NewBISReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    public class PersonUtils
    {
        private readonly IBisApiRestAccessClient _bisClient;
        //Injeção de dependencia para usar a API criada pelo Diogo para fazer solicitações HTTP
        public PersonUtils(IBisApiRestAccessClient bisClient)
        {
            _bisClient = bisClient;
        }

        public async Task<bool> Novo(BSPersonsInfo PessOrigem) //Salva um pessoa no banco
        {

            return true;
        }
    }
}
