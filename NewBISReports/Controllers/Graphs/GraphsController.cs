using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NewBISReports.Controllers.Config;
using NewBISReports.Models;
using NewBISReports.Models.Classes;
using NewBISReports.Models.Reports;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;


namespace NewBISReports.Controllers.Graphs
{
    [Authorize("AcessoUsuario")]
    public class GraphsController : Controller
    {
        #region Variables
        /// <summary>
        /// Conexão com a instância de eventos do BIS.
        /// </summary>
        private DatabaseContext contextBIS { get; set; }

        /// <summary>
        /// Conexão com a instância de dados do BIS.
        /// </summary>
        private DatabaseContext contextACE { get; set; }

        /// <summary>
        /// Coleção das unidades.
        /// </summary>
        private List<Clients> clients { get; set; }

        /// <summary>
        /// Chave com características do cliente.
        /// </summary>
        private BSConfig config { get; set; }
        #endregion

        #region injeções de dependencia
        /// <summary>
        /// classe RPTBS_Analytics
        /// </summary>
        private readonly RPTBS_Analytics _rptsAnalytics;
        #endregion

        #region Functions
        /// <summary>
        /// Mantem a conexão com o banco de dados.
        /// </summary>
        /// <param name="type">Tipo da coleção.</param>
        private bool persisTempData()
        {
            try
            {
                if (TempData["Clients"] != null)
                    ViewBag.Clients = JsonConvert.DeserializeObject(TempData["Clients"].ToString());
                else
                    ViewBag.Clients = Clients.GetClients(this.contextACE);

                if (TempData["Type"] != null)
                    ViewBag.Type = TempData["Type"];

                ViewBag.ConfigSection = JsonConvert.DeserializeObject(TempData["ConfigSection"].ToString());
                TempData.Keep();

                return true;
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> Persist");
                w.Close();
                w = null;


                return false;
            }
        }
        #endregion

        public IActionResult ExecPage(HomeModel reports)
        {
            try
            {
                List<LogEvent> acessos = new List<LogEvent>();

                TempData["Type"] = reports.Type;

                this.persisTempData();

                if (reports.Type == REPORTTYPE.RPT_TOTALMEALGRAPH)
                {
                    List<TotalMeal> meals = new List<TotalMeal>();
                    using (DataTable table = _rptsAnalytics.LoadTotalMeal(this.contextBIS, reports.StartDate, reports.EndDate,
                        reports.CLIENTID))
                    {
                        if (table != null)
                            meals = GlobalFunctions.ConvertDataTable<TotalMeal>(table);
                        reports.Meals = meals;
                        return View("Index", reports);
                    }
                }
                return View("Index", reports);

            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> ExecPage");
                w.Close();
                w = null;
                return View("Index", reports);
            }
        }

        [HttpGet]
        public IActionResult Index(REPORTTYPE type)
        {
            this.clients = Clients.GetClients(this.contextACE);
            this.clients.Insert(0, new Clients { CLIENTID = "", Description = "TODOS" });
            TempData["Clients"] = JsonConvert.SerializeObject(this.clients);
            TempData["ConfigSection"] = JsonConvert.SerializeObject(this.config);
            TempData["Type"] = type;
            TempData.Keep();

            this.persisTempData();

            return View();
        }

        public GraphsController(IConfiguration configuration,
                                RPTBS_Analytics rptsAnalytics)
        {
            try
            {
                _rptsAnalytics = rptsAnalytics;
                this.contextBIS = new DatabaseContext(configuration.GetConnectionString("BIS"));
                this.contextACE = new DatabaseContext(configuration.GetConnectionString("BIS_ACE"));
                string defaultsettings = configuration.GetSection("Default")["Name"];
                this.config = new BSConfig(defaultsettings, configuration.GetSection(defaultsettings)["BackColor"], configuration.GetSection(defaultsettings)["ForeColor"],
                    configuration.GetSection(defaultsettings)["FontWeight"], configuration.GetSection(defaultsettings)["ImagePath"],
                    configuration.GetSection(defaultsettings)["Meal"], configuration.GetSection(defaultsettings)["BisPath"], configuration.GetSection(defaultsettings)["SystemType"],
                    configuration.GetSection(defaultsettings)["AddressTagPrefix"], configuration.GetSection(defaultsettings)["AddressTagSufix"], configuration.GetSection(defaultsettings)["TagBISServer"],
                    configuration.GetSection(defaultsettings)["RestServer"], configuration.GetSection(defaultsettings)["RestPort"], configuration.GetSection(defaultsettings)["OutSideArea"]);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> Home Controller");
                w.Close();
                w = null;
            }
        }

    }
}