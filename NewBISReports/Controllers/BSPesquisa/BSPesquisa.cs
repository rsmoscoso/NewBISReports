using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NewBISReports.Controllers.Config;
using NewBISReports.Models;
using NewBISReports.Models.Classes;
using NewBISReports.Models.Reports;
using Newtonsoft.Json;
using static NewBISReports.Models.Classes.LogEvent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using NewBISReports.Models.BSPesquisa;

namespace NewBISReports.Controllers.BSPesquisa
{
    public class BSPesquisa : Controller
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

        public IActionResult eventPesquisa(BSPesquisaModel bsPesquisa)
        {
            try
            {
                if (bsPesquisa.Nome == null) //Verifica se um nome foi digitado
                {
                    throw new Exception("Digite um nome para pesquisar!");
                }

                bsPesquisa.lstPessoa = Persons.GetPersons(this.contextACE, bsPesquisa.SearchPersonsType, bsPesquisa.Nome); //Passa a lista de pessoas encontradas para a view
                return View("Index", bsPesquisa);
            }
            catch (Exception ex) //Caso não encontre nenhuma mostra uma modal e passa uma lista vazia
            {
                bsPesquisa.lstPessoa = new List<Persons>();
                ViewBag.Erro = ex.Message;
                ViewBag.Alerta = null;
                //LogFile.LogMessageToFile("Erro na Pesquisa:" + ex.Message);
                return View("Index", bsPesquisa);
            }
        }

        #region Events
        public IActionResult Index()
        {
            return View("Index", new BSPesquisaModel());
        }

        public BSPesquisa(IConfiguration configuration,
                        ArvoreOpcoes arvoreOpcoes,
                        DateTimeConverter dateTimeConverter)
        {
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
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> Home Controller");
                w.Close();
                w = null;
            }
        }
        #endregion
    }
}
