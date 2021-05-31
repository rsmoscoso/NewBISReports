using HzBISCommands;
using NewBISReports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public async Task<bool> Salvar(BSPersonsInfo PessOrigem) //Salva um pessoa no banco
        {
            try
            {
                //Tratamento de erro para nome vazio ou nome incompleto ou com numeros e caracters especiais e nome preferido
                if (String.IsNullOrEmpty(PessOrigem.NOME))
                {
                    throw new Exception("Escreva o nome do funcionário!");
                }
                else if (Regex.IsMatch(PessOrigem.NOME, (@"[^a-zA-Z ]")))
                {
                    throw new Exception("Escreva o nome do funcionário sem numeros ou caracteres especiais!");
                }
                string[] nome = PessOrigem.NOME.Split(' ');
                if (String.IsNullOrEmpty(PessOrigem.NOME.Substring(nome[0].Length)))
                {
                    throw new Exception("Escreva o nome e o sobrenome do funcionário!");
                }
                if (PessOrigem.PERSNO.Length > 16)
                {
                    throw new Exception("Nº registro do funcionário tem mais que 16 numeros");
                }
                PessOrigem = Validar(PessOrigem);
                string response = await _bisClient.SavePerson(PessOrigem);
                if (string.IsNullOrEmpty(response) == false && response.Length > 7)
                {
                    if (response.IndexOf("cartão") == -1) { throw new Exception(response); }

                }
                return true;
            }
            catch (Exception ex)
            {

                throw new Exception("Não foi possível salvar esta pessoa!");
            }
        }

        public async Task<BSPersonsInfo> Load(string PersID) //Encontra a pessoa no BIS e retorna seus dados
        {
            try
            {
                var person = await _bisClient.GetPerson(PersID);
                //if (person.PERSID == null) { throw new Exception("Pessoa não encontrada!"); }
                return (person);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar a pessoa:");
            }
        }

        public async Task<BSPersonsInfo> Get(string _PersID) //carrega a pessoa pelo ser id com seus campos
        {
            try
            {
                var personsInfo = new BSPersonsInfo();
                personsInfo = await Load(_PersID);
                //Agora ao invés de ter uma model para cada solicitação ao banco, todas são feitas aqui, sem precisar criar uma model ou objeto para isso
                //Pessoa.Perfis = await GetProfInfo();
                //Pessoa.PersClasses = await GetClasses();
                //Pessoa.Empresas = await GetComps();
                //Pessoa.Pessoa.CARDS = await GetCards(Pessoa.Pessoa.PERSID);
                //Pessoa.AutorizacoesPessoa = await GetPersonAuth(Pessoa.Pessoa.PERSID);
                //Pessoa.Autorizacoes = await GetAllAuth();
                //Pessoa.Unidades = await GetUnits();
                //Pessoa.Perfil = new BSProfilesInfo();

                //Pessoa.autoPessoasLst = GetAllAuth().Result;
                //ValidarPessoa(personsInfo); //Valida a pessoa


                //HzBISCommands.BSCompaniesInfo emp = Pessoa.Empresas.Find(x => x.COMPANYID == Pessoa.Pessoa.COMPANYID);
                //if (emp != null) { Pessoa.Pessoa.COMPANYNO = emp.COMPANYNO; } //Pega a empresa da pessoa

                //var auto = new mdAuthorization();
                //Pessoa.Autorizacoes.Insert(0, auto);

                //if (Pessoa.Pessoa.AUTHORIZATIONS == null) { Pessoa.Pessoa.AUTHORIZATIONS = new List<BSAuthorizationInfo>(); }
                ////Diogo - inserir cartaoNovo?
                //Pessoa.AutorizacaoNova = new mdAuthorization();

                return personsInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar a pessoa!");
            }
        }

        public async Task<BSPersonsInfo> Novo(BSPersonsInfo PessOrigem) //Salva um pessoa no banco
        {

            return new BSPersonsInfo();
        }


        #region Generals
        public BSPersonsInfo Validar(BSPersonsInfo PessOrigem) //Método que valida uma pessoa a ser cadastrada
        {
            string[] nome = PessOrigem.NOME.Split(' '); //Verifica se a pessoa contem nome e sobrenome
            PessOrigem.FIRSTNAME = nome[0].Trim(); //E separa os dois
            PessOrigem.LASTNAME = PessOrigem.NOME.Substring(nome[0].Length).Trim();
            return PessOrigem;
        }
        #endregion
    }
}
