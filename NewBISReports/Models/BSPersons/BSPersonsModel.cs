using HzBISCommands;
using NewBISReports.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.BSPersons
{

    public class BSPersonsModel
    {
        public enum _Operacao { Editar, Novo, Salvar, AddCartao, AddAutorizacao, ExcluirCartao, ExcluirAutorizacao }

        public _Operacao Operacao;
        public BSPersonsInfo Pessoa { get; set; }
        public List<BSClientsInfo> Unidades { get; set; }
        public List<BSAuthorizationInfo> AutorizacoesPessoa { get; set; }
        public List<BSCompaniesInfo> Empresas { get; set; }
        public List<BSProfilesInfo> Perfis { get; set; }
        public List<BSPersClassessInfo> PersClasses { get; set; }
        public BSProfilesInfo Perfil { get; set; }
        public string[] SelectedAutorizacao { set; get; }
        public BSAuthorizationInfo AutorizacaoNova { get; set; }
        public BSCardsInfo CartaoNovo { get; set; }
        public string CPF { get; set; }
        public string UF { get; set; }

        public string Aniversario { get; set; }
        public BSPersonsModel()
        {
            this.Pessoa = new BSPersonsInfo();
            this.Unidades = new List<BSClientsInfo>();
            this.AutorizacaoNova = new BSAuthorizationInfo();
            this.AutorizacoesPessoa = new List<BSAuthorizationInfo>();
            this.Empresas = new List<BSCompaniesInfo>();
            this.Perfis = new List<BSProfilesInfo>();
        }
    }
}
