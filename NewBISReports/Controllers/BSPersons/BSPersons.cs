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

        private PersonUtils personsUtils { get; set; }
        public async Task<IActionResult>Novo()
        {
            await this.personsUtils.Novo(new BSPersonsInfo());
            return View(new BSPersonsModel());
        }
        public IActionResult Index(string persid)
        {
            BSPersonsModel model = new BSPersonsModel();
            try
            {
                persid = "00134CE73E627A3C";
                model.Empresas = Company.GetCompanies(this.contextACE);
                model.Perfis = Profiles.GetProfiles(this.contextACE, null);
                model.Pessoa = Persons.GetPersonsPERSID(this.contextACE, persid);
                model.Unidades = Clients.GetAllClients(this.contextACE);
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
