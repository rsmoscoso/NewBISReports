using HzBISACELib.SQL;
using HzBISCommands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NewBISReports.Controllers.Config;
using NewBISReports.Models;
using NewBISReports.Models.BSPersons;
using NewBISReports.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Controllers.BSPersons
{
    public class BSPersons : Controller
    {

        #region Variables
        /// <summary>
        /// Conexão com a instância de dados do BIS.
        /// </summary>
        private DatabaseContext contextACE { get; set; }
        /// <summary>
        /// Coleção das pessoas.
        /// </summary>
        private List<Persons> persons { get; set; }
        /// <summary>
        /// Chave com características do cliente.
        /// </summary>
        private BSConfig config { get; set; }
        #endregion


        #region Injecao de dependencia
        private readonly ArvoreOpcoes _arvoreopcoes;
        private readonly DateTimeConverter _dateTimeConverter;
        #endregion

        #region Functions
        /// <summary>
        /// Carrega as tabelas auxiliares para as pessoas.
        /// </summary>
        /// <param name="model">Modelo de Pessoas.</param>
        private BSPersonsModel loadAllTables(BSPersonsModel model)
        {
            try
            {
                model.Empresas = Company.GetCompanies(this.contextACE);
                model.Perfis = Profiles.GetProfiles(this.contextACE, null);
                model.Unidades = Clients.GetClients(this.contextACE);
                model.PersClasses = PersClasses.GetPersClasses(this.contextACE);

                return model;
            }
            catch (Exception ex)
            {
                return model;
            }
        }
        #endregion

        private PersonUtils personsUtils { get; set; }
        public async Task<IActionResult>Novo()
        {

            return View("Index", this.loadAllTables(new BSPersonsModel()));
        }
        public async Task<IActionResult> Salvar(BSPersonsModel model)
        {
            model.Pessoa.CUSTOMFIELDS.Add(new BSAdditionalFieldInfo("CPF", "STRING", model.CPF));
            model.Pessoa.CUSTOMFIELDS.Add(new BSAdditionalFieldInfo("UF", "STRING", model.UF));
            await this.personsUtils.Salvar(model.Pessoa);
            model.Pessoa = await this.personsUtils.Get(model.Pessoa.PERSID);
            return View("Index", model);
        }
        public IActionResult Index(string persid)
        {
            BSPersonsModel model = new BSPersonsModel();
            try
            {
                persid = "0013805967BD6C2B";
                model = this.loadAllTables(model);
                model.Pessoa = Persons.GetPersonsPERSID(this.contextACE, persid);
                model.CPF = model.Pessoa.CUSTOMFIELDS.Find(d => d.LABEL.ToLower().Equals("cpf")).VALUE.ToString();
                model.UF = model.Pessoa.CUSTOMFIELDS.Find(d => d.LABEL.ToLower().Equals("uf")).VALUE.ToString();
                model.Aniversario = (model.Pessoa == null || model.Pessoa.DATEOFBIRTH.Equals(DateTime.MinValue)) ? "" : model.Pessoa.DATEOFBIRTH.ToShortDateString();
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        public BSPersons(IConfiguration configuration,
                ArvoreOpcoes arvoreOpcoes,
                DateTimeConverter dateTimeConverter,
                PersonUtils personsUtils)
        {
            this.personsUtils = personsUtils;

            //Classe que contempla opções do appssetings
            //TODO: mover as configurações dentro do Try abaixo para dentro del
            _arvoreopcoes = arvoreOpcoes;
            _dateTimeConverter = dateTimeConverter;
            try
            {
                this.contextACE = new DatabaseContext(configuration.GetConnectionString("BIS_ACE"));

                string defaultsettings = configuration.GetSection("Default")["Name"];
                this.config = new BSConfig(defaultsettings, configuration.GetSection(defaultsettings)["BackColor"], configuration.GetSection(defaultsettings)["ForeColor"],
                    configuration.GetSection(defaultsettings)["FontWeight"], configuration.GetSection(defaultsettings)["ImagePath"],
                    configuration.GetSection(defaultsettings)["Meal"], configuration.GetSection(defaultsettings)["BisPath"], configuration.GetSection(defaultsettings)["SystemType"],
                    configuration.GetSection(defaultsettings)["AddressTagPrefix"], configuration.GetSection(defaultsettings)["AddressTagSufix"], configuration.GetSection(defaultsettings)["TagBISServer"],
                    configuration.GetSection(defaultsettings)["RestServer"], configuration.GetSection(defaultsettings)["RestPort"], configuration.GetSection(defaultsettings)["OutSideArea"],
                    configuration.GetSection(defaultsettings)["WFMServer"]);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
