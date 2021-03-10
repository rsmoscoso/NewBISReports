using HzBISCommands;
using NewBISReports.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.BSPesquisa
{
    public class BSPesquisaModel
    {
        public string Nome { get; set; }
        public List<Persons> lstPessoa { get; set; }
        public SEARCHPERSONS SearchPersonsType { get; set; }

        public BSPesquisaModel()
        {
            this.lstPessoa = new List<Persons>();
        }
    }
}
