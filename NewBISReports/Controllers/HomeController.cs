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

namespace NewBISReports.Controllers
{
    [Authorize("AcessoUsuario")]
    public class HomeController : Controller
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
        /// Coleção dos tipos de pessoa.
        /// </summary>
        private List<PersClasses> persclassid { get; set; }
        /// <summary>
        /// Coleção das pessoas.
        /// </summary>
        private List<Persons> persons { get; set; }
        /// <summary>
        /// Coleção das empresas.
        /// </summary>
        public List<Company> companies { get; set; }
        /// <summary>
        /// Coleção das autorizações.
        /// </summary>
        public List<Authorizations> authorizations { get; set; }
        /// <summary>
        /// Coleção dos dispositivos.
        /// </summary>
        public List<Devices> devices { get; set; }

        /// <summary>
        /// Chave com características do cliente.
        /// </summary>
        private BSConfig config { get; set; }
        /// <summary>
        /// Lista das colunas para o relatório de pessoas.
        /// </summary>
        private BSRPTFields ReportFields { get; set; }
        /// <summary>
        /// Lista das colunas customizáveis para o relatório de pessoas.
        /// </summary>
        private List<BSRPTCustomFields> CustomFields { get; set; }
        /// <summary>
        /// Nome da área externa para o relatório de áreas.
        /// </summary>
        private string AreaExterna { get; set; }
        #endregion

        #region Injecao de dependencia
        private readonly ArvoreOpcoes _arvoreopcoes;
        private readonly DateTimeConverter _dateTimeConverter;

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
                {
                    //deve-se montar a opção todos toda vez
                    var _clients = Clients.GetClients(this.contextACE);
                    _clients.Insert(0, new Clients { CLIENTID = "", Description = "TODOS" });
                    ViewBag.Clients = _clients;
                }

                if (TempData["Areas"] != null)
                    ViewBag.Areas = JsonConvert.DeserializeObject(TempData["Areas"].ToString());
                else
                {
                    //deve-se montar a opção todos toda vez
                    var _areas = Areas.GetAreas(this.contextACE);
                    _areas.Insert(0, new Areas { AREAID = "", NAME = "TODOS" });
                    ViewBag.Areas = _areas;
                }

                if (TempData["Authorizations"] != null)
                    ViewBag.Authorizations = JsonConvert.DeserializeObject(TempData["Authorizations"].ToString());
                else
                    ViewBag.Authorizations = Authorizations.GetAuthorizations(this.contextACE, null);

                if (TempData["Company"] != null)
                    ViewBag.Company = JsonConvert.DeserializeObject(TempData["Company"].ToString());
                else
                    ViewBag.Company = Company.GetCompanies(this.contextACE);

                if (TempData["Persons"] != null)
                    ViewBag.Persons = JsonConvert.DeserializeObject(TempData["Persons"].ToString());

                if (TempData["Company"] != null)
                    ViewBag.Company = JsonConvert.DeserializeObject(TempData["Company"].ToString());

                if (TempData["Persclassid"] != null)
                    ViewBag.Persclassid = JsonConvert.DeserializeObject(TempData["Persclassid"].ToString());
                else
                    ViewBag.Persclassid = PersClasses.GetPersClasses(this.contextACE);

                if (TempData["Devices"] != null)
                    ViewBag.Devices = JsonConvert.DeserializeObject(TempData["Devices"].ToString());

                if (TempData["Type"] != null)
                    ViewBag.Type = TempData["Type"];
                if (TempData["ConfigSection"] != null)
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

        #region ACEDB Functions
        /// <summary>
        /// Pesquisa as pessoas.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        [HttpPost]
        [Route("/Home/searchPersonsAjax", Name = "searchPersonsAjax")]
        public IActionResult searchPersonsAjax([FromBody] NameSearch pesquisa)
        {
            try
            {
                var persons = new List<Persons>();

                if (pesquisa.searchField != null || (pesquisa.PersClassIdArray != null && pesquisa.PersClassIdArray.Length > 0))
                    persons = Persons.GetPersons(this.contextACE, pesquisa.SearchType, null, pesquisa.PersClassIdArray, pesquisa.searchField);
                return new JsonResult(persons);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> searchpersons");
                w.Close();
                w = null;
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Pesquisa as empresas.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult searchCompanies(HomeModel model)
        {
            try
            {
                var companies = new List<Company>();

                if (model.COMPANYNOSEARCH != null)
                    companies = Company.GetCompanies(this.contextACE, model.COMPANYNOSEARCH);

                ModelState["COMPANYNOSEARCH"].RawValue = "";

                ViewBag.Companies = companies;
                TempData["Company"] = JsonConvert.SerializeObject(ViewBag.Companies);
                TempData.Keep();

                this.persisTempData();
                //diogo se não colocar o type, ele sempre vai pular para o relatorio padrão
                //return View("Index", model.Type);
                return RedirectToAction(nameof(Index), new { type = model.Type, mensagemErro = "" });
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> searchcompanies");
                w.Close();
                w = null;

                // return View("Index");
                return RedirectToAction(nameof(Index), new { type = model.Type, mensagemErro = ex.Message });
            }
        }

        /// <summary>
        /// Pesquisa as pessoas.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        public void searchDevices(HomeModel reports)
        {
            try
            {
                var devices = new List<Devices>();

                if (reports.CLIENTID != null || reports.DEVICESEARCH != null)
                    devices = Devices.GetDevices(this.contextACE, reports.CLIENTID, reports.DEVICESEARCH);

                TempData["Devices"] = JsonConvert.SerializeObject(devices);

                this.persisTempData();
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> searchDevices");
                w.Close();
                w = null;
            }
        }

        [HttpPost]
        [Route("/Home/searchDevicesAjax", Name = "searchDevicesAjax")]
        public IActionResult searchDevicesAjax([FromBody] DeviceSearch pesquisa)
        {
            try
            {
                var devices = new List<Devices>();

                if (pesquisa.ClientId != null || pesquisa.DeviceToSearch != null)
                    devices = Devices.GetDevices(this.contextACE, pesquisa.ClientId, pesquisa.DeviceToSearch);

                return new JsonResult(devices);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> searchDevices");
                w.Close();
                w = null;
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        /// <summary>
        /// Pesquisa as autorizações.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        public void searchAuthorizations(HomeModel reports)
        {
            try
            {
                var auth = new List<Authorizations>();

                if (reports.CLIENTID != null || reports.DEVICESEARCH != null)
                    auth = Authorizations.GetAuthorizations(this.contextACE, reports.CLIENTID);

                TempData["Authorizations"] = JsonConvert.SerializeObject(auth); ;

                this.persisTempData();
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> searchAuthorizations");
                w.Close();
                w = null;
            }
        }
        #endregion

        #region BISEventLog Functions
        /// <summary>
        /// Retorna os eventos da tabela tblAcessos.
        /// </summary>
        /// <param name="reports">Parâmetros da pesquisa.</param>
        /// <returns></returns>
        private DataTable getAnalytics(HomeModel reports)
        {
            try
            {
                List<Persons> pers = new List<Persons>();
                if (!String.IsNullOrEmpty(reports.PERSNO))
                {
                    pers = Persons.GetPersons(this.contextACE, SEARCHPERSONS.SEARCHPERSONS_PERSID, reports.PERSNO);
                    reports.PERSNO = pers[0].Documento;
                }

                List<string> displaytextcustomer = new List<string>();
                if (reports.PERSCLASSID != null && reports.PERSCLASSID.Length > 0)
                {
                    foreach (string id in reports.PERSCLASSID)
                    {
                        displaytextcustomer.Add(PersClasses.GetDisplayTextCustomer(this.contextACE, id));

                    }
                }

                return RPTAnalytics.LoadAnalyticsSolar(this.contextBIS, reports.StartDate, reports.EndDate,
                    reports.PERSNO, reports.CLIENTID, displaytextcustomer, reports.CompanyNO, reports.DEVICEID, reports.Type == REPORTTYPE.RPT_ANALYTICSGENERAL ? "G" : "R");

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna os eventos da tabela tblAcessos.
        /// </summary>
        /// <param name="reports">Parâmetros da pesquisa.</param>
        /// <returns></returns>
        private DataTable getCountBath(HomeModel reports)
        {
            try
            {
                return RPTBS_Analytics.GetCountBath(this.contextBIS);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Retorna os eventos dos BIS.
        /// </summary>
        /// <param name="reports">Parâmetros da pesquisa.</param>
        /// <param name="addresstagprefix">Prefixo da AddressTag. Se for nulo, o padrão é "ControleAcesso.".</param>
        /// <param name="addresstagsufix">Sufixo da AddressTag. Se for nulo, o padrão é ".Evento".</param>
        /// <param name="meal">Se true, retorna os eventos das catracas de refeitório.</param>
        /// <returns></returns>
        private DataTable getBISEvents(HomeModel reports, string addresstagprefix, string addresstagsufix, bool meal)
        {
            try
            {
                LOGEVENT_VALUETYPE evtType = LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_PERSID;
                Clients cli = new Clients();
                string[] devices = null;
                if (!String.IsNullOrEmpty(reports.CLIENTID) && reports.CLIENTID != "0")
                    cli = Clients.GetClientsClass(this.contextACE, reports.CLIENTID);
                if (reports.DEVICEID != null && reports.DEVICEID.Length > 0)
                    devices = Devices.GetDevices(this.contextACE, reports.DEVICEID);
                if (reports.SearchPersonsType == SEARCHPERSONS.SEARCHPERSONS_CARD)
                    evtType = LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_CARDNO;

                if (String.IsNullOrEmpty(addresstagprefix))
                    addresstagprefix = "ControleAcesso.Devices.";
                if (String.IsNullOrEmpty(addresstagsufix))
                    addresstagprefix = ".Evento";
                //Diogo - A data agora já vem formatada do frontend com hora minuto
                //return RPTBS_Analytics.GetEventsBosch(this.contextBIS, this.contextACE, reports.StartDate + " 00:00:00", reports.EndDate + " 23:59:59", LogEvent.LOGEVENT_STATE.LOGEVENTSTATE_ACCESSGRANTED,
                return RPTBS_Analytics.GetEventsBosch(this.contextBIS, this.contextACE, reports.StartDate + ":00", reports.EndDate + ":59", LogEvent.LOGEVENT_STATE.LOGEVENTSTATE_ACCESSGRANTED,
                    evtType, "", cli.Name, reports.CompanyNO, reports.DEVICEID, devices, reports.PERSCLASSID,
                    String.IsNullOrEmpty(reports.LISTPERSONS) ? (reports.SearchPersonsType == SEARCHPERSONS.SEARCHPERSONS_CARD ? reports.NAMESEARCH : reports.PERSNO) : reports.LISTPERSONS, meal, reports,
                    this.config.TagBISServer, addresstagprefix, addresstagsufix, reports.AccessType);

            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> getBiSEVents");
                w.Close();
                w = null;


                return null;
            }
        }
        #endregion
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExecPage(HomeModel reports)
        {
            try
            {
                List<LogEvent> acessos = new List<LogEvent>();

                TempData["Type"] = reports.Type;

                this.persisTempData();

                if (reports.Type == REPORTTYPE.RPT_TOTALMEAL)
                {
                    List<TotalMeal> meals = new List<TotalMeal>();
                    using (DataTable table = RPTBS_Analytics.LoadTotalMeal(this.contextBIS, reports.StartDate, reports.EndDate,
                        reports.CLIENTID))
                    {
                        if (table != null)
                            meals = GlobalFunctions.ConvertDataTable<TotalMeal>(table);
                        reports.Meals = meals;
                        //return View("Index", reports);
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_ANALYTICSGENERAL)
                {
                    using (DataTable table = this.getAnalytics(reports))
                    {
                        if (table != null)
                            acessos = GlobalFunctions.ConvertDataTable<LogEvent>(table);
                    }
                    reports.Acessos = acessos;
                    //return View("Index", reports);

                }
                else if (reports.Type == REPORTTYPE.RPT_COUNTBATH)
                {
                    using (DataTable table = this.getCountBath(reports))
                    {
                        if (table != null)
                            acessos = GlobalFunctions.ConvertDataTable<LogEvent>(table);
                    }
                    reports.Acessos = acessos;
                    //return View("Index", reports);

                }
                else if (reports.Type == REPORTTYPE.RPT_EXPORTMEAL)
                {
                    using (System.Data.DataTable table = RPTAnalytics.LoadMeals(this.contextBIS, "exportmeal.txt", reports.StartDate, reports.EndDate))
                    {
                        string filename = "c:\\Horizon\\exportmeal.txt";
                        if (reports.ExportMeal(table, filename))
                        {
                            byte[] fileBytes = System.IO.File.ReadAllBytes(filename);
                            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                        }
                    }
                    //return View("Index", reports);
                }

                // return View("Index", reports);
                return RedirectToAction(nameof(Index), new { type = reports.Type });

            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> ExecPage");
                w.Close();
                w = null;
                //return View("Index", reports);
                return RedirectToAction(nameof(Index), new { type = reports.Type, mensagemErro = ex.Message });
            }
        }

        /// <summary>
        /// Executa a pesquisa dos eventos para uma planilha Excel.
        /// </summary>
        /// <param name="reports">Parâmetros da pesquisa.</param>
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ExcelPage(HomeModel reports)
        {
            try
            {
                byte[] filebytes = null;
                List<LogEvent> acessos = new List<LogEvent>();

                this.persisTempData();

                //Diogo - Converte as strings de data e hora para PTBR
                if (!String.IsNullOrEmpty(reports.StartDate))
                {
                    reports.StartDate = _dateTimeConverter.ToPtBR(reports.StartDate);
                }
                if (!String.IsNullOrEmpty(reports.EndDate))
                {
                    reports.EndDate = _dateTimeConverter.ToPtBR(reports.EndDate);
                }

                if (reports.Type == REPORTTYPE.RPT_ANALYTICGRANTEDBIS)
                {
                    using (DataTable table = this.getBISEvents(reports, config.AddressTagPrefix, config.AddressTagSufix, false))
                    {

                        //Remover colunas não desejadas
                        //basta configurar na propriedade "RemoverColunasAlanyticsGranted" do appssettings.json
                        if (_arvoreopcoes.RemoverColunasAlanyticsGranted.Count > 0)
                        {
                            //
                            foreach (string ColName in _arvoreopcoes.RemoverColunasAlanyticsGranted)
                            {
                                if (table.Columns.Contains(ColName))
                                    table.Columns.Remove(ColName);
                            }
                        }
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\bisevents.xlsx", "Orion", "Analytics", _dateTimeConverter)) != null)
                        {
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\bisevents.xlsx");
                        }
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_ANALYTICSGENERAL)
                {
                    using (DataTable table = this.getAnalytics(reports))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\bismeals.xlsx", "Orion", "Meal", _dateTimeConverter)) != null)
                        {
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\bismeals.xlsx");
                        }
                    }

                }
                else if (reports.Type == REPORTTYPE.RPT_ANALYTICSMEALBIS)
                {
                    string divisao = Clients.GetClientDescription(this.contextACE, reports.CLIENTID);
                    //Diogo - único relatório em que se passa apenas data                  
                    using (DataTable table = RPTBS_Analytics.GetMealBosch(this.contextBIS, this.contextACE, reports.MealType, reports.StartDate + " 00:00:00", reports.EndDate + " 23:59:59", config.TagBISServer, divisao))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\bismeals.xlsx", "Orion", "Meal", _dateTimeConverter)) != null)
                        {
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\bismeals.xlsx");
                        }
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_PHOTOS)
                {
                    using (DataTable table = RPTBS_Acedb.LoadPhotos(this.contextACE, reports.CLIENTID))
                    {
                        filebytes = RPTBS_Acedb.SaveFile(table, this.config.BisPath);

                        return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\reportphoto.txt");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_BADGES)
                {
                    using (DataTable table = RPTBS_Acedb.LoadNoBadge(this.contextACE, reports.CLIENTID))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\reportphoto.xlsx", "Orion", "Crachas", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\reportphoto.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_LOGQRCODE)
                {
                    string visitorname = Visitors.GetVisitorName(this.contextACE, reports.PERSNO);

                    //DatabaseContext ctext = new DatabaseContext("Data Source=(local); UID=sa; Password=S$iG3L9a@n; Database=hzFortknox;Connection Timeout=300; MultipleActiveResultSets='true'");

                    using (DataTable table = RPTBS_Acedb.LoadVisitorQRCode(this.contextACE, visitorname))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\visqrcode.xlsx", "Orion", "QRCode", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\visqrcode.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_READERAUTHORIZATION)
                {
                    using (DataTable table = RPTBS_Acedb.LoadReaderAuthorization(this.contextACE, reports.CLIENTID, reports.AUTHID))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\ReaderAuth.xlsx", "Orion", "ReaderAuth", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\ReaderAuth.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_BADGENOUSE)
                {
                    using (System.Data.DataTable table = RPTBS_Acedb.LoadBadgeNoUse(this.contextACE, reports.NDays))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\reportbadgesnouse.xlsx", "Orion", "Crachas", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\reportbadges.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_PERSONSPROFILES)
                {
                    using (System.Data.DataTable table = RPTBS_Acedb.LoadPersonProfiles(this.contextACE, reports.CLIENTID, reports.AUTHID))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\personprofile.xlsx", "Orion", "Profiles", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\personprofile.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_PERSONSAUTHORIZATIONS)
                {
                    using (System.Data.DataTable table = RPTBS_Acedb.LoadPersonAuths(this.contextACE, reports.CLIENTID, reports.AUTHID))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\personauth.xlsx", "Orion", "Crachas", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\personauth.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_ALLLOCKOUT)
                {
                    using (System.Data.DataTable table = RPTBS_Acedb.LoadAllLocked(this.contextACE, reports.CLIENTID))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\peoplelockout.xlsx", "Orion", "Locked", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\peoplelockout.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_ALLVISITORS)
                {
                    using (System.Data.DataTable table = RPTBS_Acedb.LoadAllVisitors(this.contextACE, reports.CLIENTID))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\visitors.xlsx", "Orion", "Visitors", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\visitors.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_PERSONSAREA)
                {
                    using (System.Data.DataTable table = RPTBS_Acedb.LoadPersonsArea(this.contextACE, reports.AREAID, this.AreaExterna))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\area.xlsx", "Orion", "Area", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\area.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_PERSONGENERAL)
                {
                    using (System.Data.DataTable table = RPTBS_Acedb.LoadAllPerson(this.contextACE, this.ReportFields, this.CustomFields, reports.PERSNO, reports.PERSCLASSID, reports.CLIENTID, reports.CompanyNO))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\people.xlsx", "Orion", "PEOPLE", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\people.xlsx");
                    }
                }
                else if (reports.Type == REPORTTYPE.RPT_TOTALMEAL)
                {
                    List<TotalMeal> meals = new List<TotalMeal>();
                    using (DataTable table = RPTBS_Analytics.LoadTotalMeal(this.contextBIS, reports.StartDate, reports.EndDate,
                        reports.CLIENTID))
                    {
                        if ((filebytes = GlobalFunctions.SaveExcel(table, @"c:\\horizon\\reporttotalmeal.xlsx", "Orion", "Crachas", _dateTimeConverter)) != null)
                            return File(filebytes, System.Net.Mime.MediaTypeNames.Application.Octet, "c:\\horizon\\reporttotalmeal.xlsx");
                    }
                }

                reports.Acessos = acessos;
                this.persisTempData();

                //Diogo - Adicionando mensagem de alerta, caso nenhum arquivo seja retornado, se chegar aqui sem erros
                return RedirectToAction(nameof(Index), new { type = reports.Type, mensagemErro = "Nenhum registro encontrado" });
                //return View("Index", reports);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> ExcelPage");
                w.Close();
                w = null;
                //adicionando verbosidade de erros e recarregando o formulário corretamente
                //return View("Index", reports);
                return RedirectToAction(nameof(Index), new { type = reports.Type, mensagemErro = ex.Message });
            }
        }

        /// <summary>
        /// Evento de geração de relatório de acordo com as opções do usuário.
        /// srsclasses - ação do botão de pesquisa do tipo de pessoa;
        /// srcname - ação do botão de pesquisa a pessoa.
        /// </summary>
        /// <param name="reports">Classe com os dados da pesquisa.</param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Index(HomeModel reports)
        {
            try
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine("Index...");
                w.Close();
                w = null;

                if (Request.Form["CLIENTID"].Count == 1)
                {
                    this.searchDevices(reports);
                    if (reports.Type == REPORTTYPE.RPT_READERAUTHORIZATION)
                        this.searchAuthorizations(reports);
                    if (reports.Type == REPORTTYPE.RPT_PERSONSAUTHORIZATIONS)
                        this.searchAuthorizations(reports);
                }

                return View("Index", reports);
                //return RedirectToAction(nameof(Index), new{type=reports.Type});
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> Index POST");
                w.Close();
                w = null;

                // return View("Index", reports);
                return RedirectToAction(nameof(Index), new { type = reports.Type, mensagemErro = ex.Message });
            }
        }
        //Diogo Adicionando Landing page
        [HttpGet]
        public IActionResult Landing()
        {
            TempData["ConfigSection"] = JsonConvert.SerializeObject(this.config);
            TempData["Type"] = REPORTTYPE.RPT_LANDINGPAGE;
            TempData.Keep();

            this.persisTempData();
            return View();
        }

        [HttpGet]
        public IActionResult Index(REPORTTYPE type, string mensagemErro)
        {

            try
            {
                //Diogo - Removi as inicializações, pois o persistTempData já controla a integridade das inicializações
                this.persisTempData();

                //diogo - adicionando uma Landing Page
                if (type == REPORTTYPE.RPT_LANDINGPAGE)
                {
                    return RedirectToAction(nameof(Landing));
                }
                else
                {
                    //adicionando a mensagem de erro propagada
                    if (mensagemErro != null)
                    {
                        ViewBag.MensagemErro = mensagemErro;
                    }
                    ViewBag.Type = type;
                    return View();
                }

            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> Index GET");
                w.Close();
                w = null;


                return View();
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public HomeController(IConfiguration configuration,
                                ArvoreOpcoes arvoreOpcoes,
                                DateTimeConverter dateTimeConverter)
        {
            //Classe que contempla opções do appssetings
            //TODO: mover as configurações dentro do Try abaixo para dentro del
            _arvoreopcoes = arvoreOpcoes;
            _dateTimeConverter = dateTimeConverter;
            try
            {
                this.contextBIS = new DatabaseContext(configuration.GetConnectionString("BIS"));
                this.contextACE = new DatabaseContext(configuration.GetConnectionString("BIS_ACE"));

                if (configuration.GetSection("Report").GetChildren() != null)
                {
                    foreach (ConfigurationSection config in configuration.GetSection("Report").GetChildren())
                    {
                        if (config.Key.Equals("ALLRECORDS"))
                            this.ReportFields = (BSRPTFields)JsonConvert.DeserializeObject<BSRPTFields>(configuration.GetSection("Report")["ALLRECORDS"].ToString());
                        else if (config.Key.Equals("CUSTOMFIELDS"))
                            this.CustomFields = (List<BSRPTCustomFields>)JsonConvert.DeserializeObject<List<BSRPTCustomFields>>(configuration.GetSection("Report")["CUSTOMFIELDS"].ToString());
                    }
                }

                string defaultsettings = configuration.GetSection("Default")["Name"];
                this.config = new BSConfig(defaultsettings, configuration.GetSection(defaultsettings)["BackColor"], configuration.GetSection(defaultsettings)["ForeColor"],
                    configuration.GetSection(defaultsettings)["FontWeight"], configuration.GetSection(defaultsettings)["ImagePath"],
                    configuration.GetSection(defaultsettings)["Meal"], configuration.GetSection(defaultsettings)["BisPath"], configuration.GetSection(defaultsettings)["SystemType"],
                    configuration.GetSection(defaultsettings)["AddressTagPrefix"], configuration.GetSection(defaultsettings)["AddressTagSufix"], configuration.GetSection(defaultsettings)["TagBISServer"],
                    configuration.GetSection(defaultsettings)["RestServer"], configuration.GetSection(defaultsettings)["RestPort"], configuration.GetSection(defaultsettings)["OutSideArea"]);
                this.AreaExterna = configuration.GetSection(defaultsettings)["OutSideArea"];
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