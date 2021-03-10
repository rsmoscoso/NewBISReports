
using System;
using HzBISCommands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewBISReports.Services
{
    public interface IBisApiRestAccessClient
    {
        Task<BSPersonsInfo> GetPerson(string persno);
        Task<BSPersonsSearch> GetPersonsList(string name);
        Task<HzBISCommands.BSAuthorizationInfo> GetPersonAuthorizations(string persid);
        Task<HzBISCommands.BSAuthorizationInfo> GetAuthorizations();
        Task<BSPersonsCard> GetPersonCards(string persid);
        Task<BSClientsInfo> GetUnits();
        Task<BSCompaniesInfo> GetCompanies();
        Task<List<BSProfilesInfo>> GetProfile();
        Task<BSProfilesInfo> GetPersonsProfile(string persid);
        Task<BSPersClassessInfo> GetPersonsClass();
        Task<BSLoginInfo> Login(string user, string password);
        Task<BSPersonsInfo> Exist(string persno);
        Task<string> SavePerson(BSPersonsInfo person);
        Task<string> AddPersonsCard(BSPersonsCard card);
        Task<string> AddPersonsAuth(BSAuthorizationInfo auth);
        Task<string> RemovePersonsCard(BSPersonsCard card);
        Task<string> RemovePersonsAuth(BSAuthorizationInfo auth);
        
    }


    //TODO: atualizar tudo para filtrar por entidade, e por conseguinte, abrir a key e secret adequada para a requisição
    //talvez fazer uma tabela singleton para armazenar os tokens por entidade.
    public class BisApiRestAccessClient : IBisApiRestAccessClient
    {
        private readonly HttpClient _client;
        private readonly ApiClientBase _apiClientBase;

        //Construtor da classe. As injeções de dependencias são  feitas pelo conteiner injetor de dependencias, vindas do startup
        //a partir do serviço criado pelo Doigo  (AddBisRestApiAccess)
        public BisApiRestAccessClient(HttpClient client, ApiClientBase apiClientBase)
        {
            _client = client;
            _apiClientBase = apiClientBase;
        }
        //Métodos GET
        public async Task<BSPersonsInfo> GetPerson(string persno)
        {
            //Recupera os dados de uma pessoa no BIS
            var retorno = await _apiClientBase.GetAsync<BSPersonsInfo>(_client, "/api/BSPersons/GetPersons/LOADPERSON/" + persno, null);
            return retorno;
        }
        public async Task<BSPersonsSearch> GetPersonsList(string name)
        {
            //Usado para fazer pesquisas de pessoas no BIS
            var retorno = await _apiClientBase.GetAsync<BSPersonsSearch>(_client, "/api/BSPersons/GetPersons/LOADPERSONSQL/" + name, null);
            return retorno;
        }
        public async Task<HzBISCommands.BSAuthorizationInfo> GetAuthorizations()
        {
            //Pega as autotizações existentes no BIS.
            var retorno = await _apiClientBase.GetAsync<HzBISCommands.BSAuthorizationInfo>(_client, "/api/BSTables/GetTables/LOADAUTHORIZATIONSQL/null", null);
            return retorno;
        }
        public async Task<HzBISCommands.BSAuthorizationInfo> GetPersonAuthorizations(string persid)
        {
            //Pega as autorizações de uma determinada pessoa no BIS
            var retorno = await _apiClientBase.GetAsync<HzBISCommands.BSAuthorizationInfo>(_client, "/api/BSPersons/GetPersons/LOADAUTHORIZATIONSQL/"+persid, null);
            return retorno;
        }
        public async Task<BSPersonsCard> GetPersonCards(string persid)
        {
            //Pega os cartões de uma determinada pessoa no BIS
            var retorno = await _apiClientBase.GetAsync<BSPersonsCard>(_client, "/api/BSPersons/GetPersons/LOADPERSONCARD/" + persid, null);
            return retorno;
        }
        public async Task<BSClientsInfo> GetUnits()
        {
            //Pega as unidades do BIS
            var retorno = await _apiClientBase.GetAsync<BSClientsInfo> (_client, "/api/BSTables/GetTables/LOADCLIENTSQL/null", null);
            return retorno;
        }
        public async Task<BSCompaniesInfo> GetCompanies()
        {
            //Pega as empresas cadastradas no BIS
            var retorno = await _apiClientBase.GetAsync<BSCompaniesInfo>(_client, "/api/BSTables/GetTables/LOADCOMPANYSQL/null", null);
            return retorno;
        }
        public async Task<List<BSProfilesInfo>> GetProfile()
        {
            //Pega os perfis cadastrados no BIS
            var retorno = await _apiClientBase.GetAsync<List<BSProfilesInfo>>(_client, "/api/BSTables/GetTables/LOADPROFILESQL/null", null);
            return retorno;
        }
        public async Task<BSProfilesInfo> GetPersonsProfile(string persid)
        {
            //Pega os perfis de uma determinada pessoa
            var retorno = await _apiClientBase.GetAsync<BSProfilesInfo>(_client, "/api/BSTables/GetTables/LOADAUTHPROFILESQL/" + persid, null);
            return retorno;
        }
        public async Task<BSPersonsInfo> Exist(string persno)
        {
            //Verifica se uma pessoa já existe (esta cadastrada) no BIS
            var retorno = await _apiClientBase.GetAsync<BSPersonsInfo>(_client, "/api/BSPersons/GetPersons/LOADPERSON/" + persno, null);
            return retorno;
        }
        public async Task<BSPersClassessInfo> GetPersonsClass()
        {
            //Pega as classes de pessoas do BIS
            var retorno = await _apiClientBase.GetAsync<BSPersClassessInfo>(_client,"/api/BSTables/GetTables/LOADPERSCLASSESSQL/null", null);
            return retorno;
        }
        public async Task<BSLoginInfo> Login(string user, string password)
        {
            //Efetua no login no BIS
            var retorno = await _apiClientBase.GetAsync<BSLoginInfo>(_client, "/api/BSAction/CheckUser" + "/" + user + "/" + password, null);
            return retorno;
        }
        //Métodos POST
        public async Task<string> SavePerson(BSPersonsInfo person)
        {
            //Salva um pessoa no BIS (Nova ou existente fazendo atualização dos dados)
            var retorno = await _apiClientBase.PostAsync<BSPersonsInfo>(_client, "/api/BSPersons/SavePersons/",null,person);
            return retorno;
        }
        public async Task<string> AddPersonsCard(BSPersonsCard card)
        {
            //Adiciona um cartão
            var retorno = await _apiClientBase.PostAsync<BSPersonsCard>(_client, "/api/BSPersons/AddPersonCard/", null, card);
            return retorno;
        }
        public async Task<string> AddPersonsAuth(BSAuthorizationInfo auth)
        {
            //Adiciona um autorização para uma pessoa
            var retorno = await _apiClientBase.PostAsync<BSAuthorizationInfo>(_client, "/api/BSPersons/AddAuthorization/", null, auth);
            return retorno;
        }
        public async Task<string> RemovePersonsCard(BSPersonsCard card)
        {
            //Remove um cartão de uma pessoa
            var retorno = await _apiClientBase.PostAsync<BSPersonsCard>(_client, "/api/BSPersons/RemovePersonCard/", null, card);
            return retorno;
        }
        public async Task<string> RemovePersonsAuth(BSAuthorizationInfo auth)
        {
            //Remove uma autorização de uma pessoa
            var retorno = await _apiClientBase.PostAsync<BSAuthorizationInfo>(_client, "/api/BSPersons/RemoveAuthorization/", null, auth);
            return retorno;
        }
    }
}


