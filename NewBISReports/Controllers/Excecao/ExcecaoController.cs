using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NewBISReports.Controllers.Config;
using NewBISReports.Models;
using NewBISReports.Models.Classes;
using NewBISReports.Models.Excecao;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace NewBISReports.Controllers.Excecao
{
    [Authorize("AcessoUsuario")]
    public class ExcecaoController : Controller
    {
        #region Variables
        /// <summary>
        /// Conexão com a instância de dados do BIS.
        /// </summary>
        private DatabaseContext contextACE { get; set; }
        /// <summary>
        /// Conexão com a instância de dados do BIS.
        /// </summary>
        private DatabaseContext contextSolar { get; set; }
        /// <summary>
        /// Coleção das pessoas.
        /// </summary>
        private List<Persons> persons { get; set; }
        /// <summary>
        /// Coleção das pessoas.
        /// </summary>
        private List<Persons> personsexcep { get; set; }
        /// <summary>
        /// Chave com características do cliente.
        /// </summary>
        private BSConfig config { get; set; }
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        private void persist()
        {
            ViewBag.Persons = JsonConvert.DeserializeObject(TempData["Persons"].ToString());
            ViewBag.PersonsExcep = JsonConvert.DeserializeObject(TempData["PersonsExcep"].ToString());
            ViewBag.ConfigSection = JsonConvert.DeserializeObject(TempData["ConfigSection"].ToString());

            TempData["Persons"] = JsonConvert.SerializeObject(ViewBag.Persons);
            TempData["PersonsExcep"] = JsonConvert.SerializeObject(ViewBag.PersonsExcep);
            ViewBag.ConfigSection = this.config;
            TempData["ConfigSection"] = JsonConvert.SerializeObject(ViewBag.ConfigSection);
            TempData.Keep();

            ViewBag.Persons = JsonConvert.DeserializeObject(TempData["Persons"].ToString());
            ViewBag.PersonsExcep = JsonConvert.DeserializeObject(TempData["PersonsExcep"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Import(ExcecaoModel excecaoModel)
        {
            //foreach (IFormFile file in excecaoModel.FileName)
            //{
            //    var filepath = String.Format("{0}/ImportCSV/{1}.csv", this._hostingEnvironment.WebRootPath, "foto", file.FileName);

            //    using (FileStream fs = System.IO.File.Create(filepath))
            //    {
            //        using (var memoryStream = new MemoryStream())
            //        {
            //            await file.CopyToAsync(memoryStream);
            //            byte[] bt = memoryStream.ToArray();

            //            await file.CopyToAsync(fs);
            //            fs.Flush();
            //        }
            //    }
            //}

            return View();
        }
        /// <summary>
        /// Pesquisa as pessoas.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        [HttpPost]
        public IActionResult searchPersons(ExcecaoModel reports)
        {
            try
            {
                var persons = new List<Persons>();

                persons = Persons.GetPersons(this.contextACE, Models.SEARCHPERSONS.SEARCHPERSONS_NAME, reports.NAMESEARCH);

                ModelState["NAMESEARCH"].RawValue = "";

                ViewBag.Persons = persons;
                TempData["Persons"] = JsonConvert.SerializeObject(ViewBag.Persons);
                this.persist();

                return View("Index", reports);
            }
            catch
            {
                return View("Index", reports);
            }
        }

        /// <summary>
        /// Pesquisa as pessoas.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        [HttpPost]
        public IActionResult New(ExcecaoModel reports)
        {
            try
            {
                tblBlockExcecao.New(this.contextSolar, reports);
                this.persist();

                return RedirectToAction("Index", reports);
            }
            catch
            {
                return View("Index", reports);
            }
        }

        /// <summary>
        /// Pesquisa as pessoas.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        [HttpPost]
        public IActionResult Delete(ExcecaoModel reports)
        {
            try
            {
                if (!String.IsNullOrEmpty(reports.PERSID))
                    tblBlockExcecao.Delete(this.contextSolar, reports.PERSID);
                this.persist();

                return RedirectToAction("Index", reports);
            }
            catch
            {
                return View("Index", reports);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                this.personsexcep = tblBlockExcecao.LoadExceptions(this.contextACE, this.contextACE.GetHost().IndexOf("forsrp") > -1 ? "hzSolar" : "hzRH");
                ViewBag.PersonsExcep = this.personsexcep;
                TempData["PersonsExcep"] = JsonConvert.SerializeObject(ViewBag.PersonsExcep);
                ViewBag.Persons = new List<Persons>();
                TempData["Persons"] = JsonConvert.SerializeObject(ViewBag.Persons);
                ViewBag.ConfigSection = this.config;
                TempData["ConfigSection"] = JsonConvert.SerializeObject(ViewBag.ConfigSection);
                TempData.Keep();

                ViewBag.PersonsExcep = JsonConvert.DeserializeObject(TempData["PersonsExcep"].ToString());
                ViewBag.Persons = JsonConvert.DeserializeObject(TempData["Persons"].ToString());
                return View("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Index(ExcecaoModel reports)
        {
            try
            {
                using (DataTable table = tblBlockExcecao.LoadExceptionsDt(this.contextACE, reports.PERSID, this.contextACE.GetHost().IndexOf("forsrp") > -1 ? "hzSolar" : "hzRH"))
                {

                    reports.StartDate = table.Rows[0]["cmpDtInicio"].ToString();
                    reports.EndDate = table.Rows[0]["cmpDtTermino"].ToString();
                    //this.searchPersons(table.Rows[0]["persid"].ToString());
                    this.persist();
                }

                return View("Index", reports);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="configuration">Referência às configurações em appsettings.json.</param>
        public ExcecaoController(IConfiguration configuration, IHostingEnvironment env)
        {
            this._hostingEnvironment = env;
            this.contextACE = new DatabaseContext(configuration.GetConnectionString("BIS_ACE"));
            this.contextSolar = new DatabaseContext(configuration.GetConnectionString("SOLAR"));
            string defaultsettings = configuration.GetSection("Default")["Name"];
            this.config = new BSConfig(defaultsettings, configuration.GetSection(defaultsettings)["BackColor"], configuration.GetSection(defaultsettings)["ForeColor"],
                configuration.GetSection(defaultsettings)["FontWeight"], configuration.GetSection(defaultsettings)["ImagePath"],
                configuration.GetSection(defaultsettings)["Meal"], configuration.GetSection(defaultsettings)["BisPath"], configuration.GetSection(defaultsettings)["SystemType"],
                                configuration.GetSection(defaultsettings)["AddressTagPrefix"], configuration.GetSection(defaultsettings)["AddressTagSufix"], configuration.GetSection(defaultsettings)["TagBISServer"],
                    configuration.GetSection(defaultsettings)["RestServer"], configuration.GetSection(defaultsettings)["RestPort"], configuration.GetSection(defaultsettings)["OutSideArea"],
                    configuration.GetSection(defaultsettings)["WFMServer"]);
        }

    }
}