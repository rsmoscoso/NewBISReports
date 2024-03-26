using NewBISReports.Models.Classes;
using NewBISReports.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static NewBISReports.Models.Classes.LogEvent;
using HzBISCommands;
using HzBISCommands.Events;
using NPOI.XSSF.Streaming.Values;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace NewBISReports.Models.Reports
{
    /// <summary>
    /// Classe com as pesquisas dos eventos.
    /// </summary>
    public class RPTBS_Analytics
    {
        private readonly ArvoreOpcoes _arvoreOpcoes;

        public RPTBS_Analytics(ArvoreOpcoes arvoreOpcoes)
        {
            _arvoreOpcoes = arvoreOpcoes;
        }

        private BSEvents setEventValue(BSEvents evt, string name, string value)
        {
            switch (name.ToLower())
            {
                case "persid":
                    evt.Persid = value;
                    break;
                case "persno":
                    evt.Persno = value;
                    break;
                case "name":
                    evt.Name = value;
                    break;
                case "firstname":
                    evt.FirstName = value;
                    break;
                case "company":
                    evt.Company = value;
                    break;
                case "customcode":
                    evt.CustomCode = value;
                    break;
                case "areaname":
                    evt.AreaName = value;
                    break;
                case "kurztext":
                    evt.Kurztext = value;
                    break;
                case "cardid":
                    evt.CardID = value;
                    break;
                case "cardno":
                    evt.CardNO = value;
                    break;
            }

            return evt;
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        #region Analytics
        public DataTable GetDataAcesso(DatabaseContext dbcontext, string start, string end)
        {
            try
            {
                string sql = String.Format("set dateformat 'dmy' select dtper.persid, re = persno, nome, primeiraData = convert(varchar, cmpDtFirst, 103), primeiraHora = convert(varchar, cmpDtFirst, 108), LocalPrimeira = LocalFirst, " +
                    "UltimaData = convert(varchar, cmpDtLast, 103), UltimaHora = convert(varchar, cmpDtLast, 108), LocalUltima = LocalLast from tblDataAcessoPERSID dtper " +
                    "inner join HzBIS..tblDataAcesso dt on dt.cmpCoDataAcesso = dtper.cmpCoDataAcesso inner join [kofbrclubis02\\bis_ace_2021].acedb.bsuser.persons per on per.persid = dtper.persid " +
                    "where cmpDtFirst >= '{0}' and cmpDtFirst <= '{1}'", start, end);

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erroDataAcesso.txt", true);
                w.WriteLine(ex.Message + " --> GetDataAcesso");
                w.Close();
                w = null;

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna os eventos, de acordo com o estado, da tabela LogEvent.
        /// O parâmetro deve ser o ID da pessoa no BIS (PERSID).
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="start">Data inicial da pesquisa.</param>
        /// <param name="end">Data final da pesquisa.</param>
        /// <param name="state">Tipo do estado do relatório.</param>
        /// <param name="type">Tipo da varíavel da pesquisa (PERSID, PERSNO...)</param>
        /// <param name="clientexternalid">ID externo da uniade relativa à Pessoa.</param>
        /// <param name="description">Nome da unidade.</param>
        /// <param name="company">Lista com os nomes das empresas.</param>
        /// <param name="deviceid">Lista da coluna Displaytext do dispositivo</param>
        /// <param name="persclassid">Lista com os tipos de pessoa.</param>
        /// <param name="stringvalue">Desrição do item da pesquisa.</param>
        /// <param name="meal">Verifica os eventos de refeição.</param>
        /// <param name="server">Servidor da instância BIS_ACE.</param>
        /// <returns></returns>
        public DataTable GetEventsBosch(DatabaseContext dbcontext, DatabaseContext dbcontextACE, string start, string end, LOGEVENT_STATE state, LOGEVENT_VALUETYPE type, string clientexternalid,
            string description, string[] company, string[] deviceid, string[] addresstag, string[] persclassid, string stringvalue, bool meal, HomeModel reports, string tagbisserver,
            string addresstagprefix, string addresstagsufix, ACCESSTYPE accesstype, string defaultconfig)
        {
            try
            {
                //string server = this.contextACE.GetHost();
                string devid = "";
                string cmpno = "";
                string persclass = "";

                //variável da nomenclatura da divisão no LogDivision.
                //Mantida para futura análise.
                //tagbisserver += "." 
                tagbisserver = "";

                //if (reports.DEVICEID != null && reports.DEVICEID.Length > 0)
                //{
                //    if (deviceid != null && deviceid.Length > 0)
                //    {
                //        string where = "";
                //        devid = "";
                //        foreach (string id in deviceid)
                //            where += "''" + id + "'', ";
                //        devid += where.Substring(0, where.Length - 2);
                //    }
                //}

                if (addresstag != null && addresstag.Length > 0)
                {
                    string where = "";
                    devid = "";
                    foreach (string name in addresstag)
                        where += "''" + addresstagprefix + name + addresstagsufix + "'', ";
                    devid += where.Substring(0, where.Length - 2);
                }

                if (!String.IsNullOrEmpty(reports.CLIENTID))
                {
                    Clients cli = new Clients();
                    if (!String.IsNullOrEmpty(reports.CLIENTID) && reports.CLIENTID != "0")
                    {
                        cli = (Clients)Clients.GetClientsClass(dbcontextACE, reports.CLIENTID);
                        clientexternalid = cli.ExternID;
                    }

                    //if (reports.CLIENTID.Equals("001301AE6C269AC1"))
                    //    reports.CLIENTID = "0013A26FC15BC5B6";
                    //else if (reports.CLIENTID.Equals("0013A26FC15BC5B6"))
                    //    reports.CLIENTID = "001301AE6C269AC1";
                }

                if (company != null && company.Length > 0)
                {
                    string where = "";
                    cmpno = "";
                    foreach (string no in company)
                        where += "''" + no + "'', ";
                    cmpno += where.Substring(0, where.Length - 2);
                }

                if (persclassid != null && persclassid.Length > 0)
                {
                    string where = "";
                    persclass = "";
                    foreach (string no in persclassid)
                        where += "''" + no + "'',";
                    persclass += where.Substring(0, where.Length - 2) + "'";
                }

                List<BSEvents> events = new List<BSEvents>();
                string sql = null;
                if (defaultconfig.ToLower().Trim().Equals("solar"))
                {
                    sql = String.Format("set dateformat 'dmy' select LogEvent.ID, stringValue, eventValueName, AddressTag, eventCreationTime from BISEventLog..LogEventValue inner join BISEventLog..LogEvent2Value on BISEventLog..LogEvent2Value.valueId = BISEventLog..LogEventValue.Id " +
                        "inner join BISEventLog..LogEvent on BISEventLog..LogEvent.Id = BISEventLog..LogEvent2Value.eventId inner join BISEventLog..LogState on BISEventLog..LogState.Id = BISEventLog..LogEvent.stateId " +
                        "inner join BISEventLog..LogEventValueType on BISEventLog..LogEventValueType.Id = BISEventLog..LogEventValue.eventTypeId " +
                        "inner join BISEventLog..LogEventType on BISEventLog..LogEventType.ID = BISEventLog..LogEvent.eventTypeId inner join BISEventLog..LogAddress on BISEventLog..LogAddress.ID = BISEventLog..LogEvent.AddressID " +
                        "where stateNumber = 4101 and LogEventType.ID = 1 and eventCreationTime >= '{0}' and eventCreationTime <= '{1}' " +
                        "and eventValueName in ('PERSID', 'PERSNO', 'NAME', 'FIRSTNAME', 'COMPANY', 'CUSTOMCODE', 'AREANAME', 'Kurztext', 'CARDID', 'CARDNO', 'Badge ID') order by BISEventLog..LogEvent.ID", start, end);

                    using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                    {
                        if (table != null && table.Rows.Count > 0)
                        {
                            int id = 0;
                            BSEvents evt = null;
                            string stringValue = null;
                            string eventValueName = null;
                            foreach (DataRow row in table.Rows)
                            {
                                eventValueName = row["eventValueName"].ToString();
                                stringValue = row["stringValue"].ToString();
                                if (id != int.Parse(row["id"].ToString()))
                                {
                                    if (evt != null)
                                        events.Add(evt);
                                    evt = new BSEvents();
                                    evt.ID = int.Parse(row["id"].ToString());
                                    evt.EventCreationTime = DateTime.Parse(row["EventCreationTime"].ToString());
                                    evt.AddressTag = row["addresstag"].ToString();
                                    id = int.Parse(row["id"].ToString());
                                }

                                evt = setEventValue(evt, eventValueName, stringValue);
                            }
                        }
                    }
                }
                else
                {
                    sql = null;
                    if (String.IsNullOrEmpty(persclass))
                    {
                        sql = String.Format("set dateformat 'dmy' exec BISEventLog.." + _arvoreOpcoes.SpAccessGranted + " {0}, '{1}', '{2}', {3}, {4}, {5}, {6}",
                            String.IsNullOrEmpty(stringvalue) ? "null" : "'" + stringvalue + "'", start, end,
                            String.IsNullOrEmpty(reports.CLIENTID) ? "null" : "'" + reports.CLIENTID + "'",
                            String.IsNullOrEmpty(devid) ? "null" : "'" + devid + "'",
                            String.IsNullOrEmpty(cmpno) ? "null" : "'" + cmpno + "'",
                            ((int)accesstype).ToString());
                    }
                    else
                    {
                        sql = String.Format("set dateformat 'dmy' exec BISEventLog.." + _arvoreOpcoes.SpPersClassAccessGranted + "  {0}, '{1}', '{2}', {3}, {4}, {5}, '{6}', {7}",
                        String.IsNullOrEmpty(stringvalue) ? "null" : "'" + stringvalue + "'",
                        start,
                        end,
                        String.IsNullOrEmpty(reports.CLIENTID) ? "null" : "'" + tagbisserver + description + "'",
                        String.IsNullOrEmpty(devid) ? "null" : "'" + devid + "'",
                        String.IsNullOrEmpty(cmpno) ? "null" : "'" + cmpno + "'",
                        String.IsNullOrEmpty(persclass) ? "null" : persclass,
                       ((int)accesstype).ToString());
                    }
                }

                StreamWriter w = new StreamWriter("sp.txt", true);
                w.WriteLine(sql);
                w.Close();
                w = null;

                return defaultconfig.ToLower().Trim().Equals("solar") ? ToDataTable<BSEvents>(events) :
                dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> GetEventsBosch");
                w.Close();
                w = null;

                throw new Exception(ex.Message);
            }
        }

        public DataTable GetEventsBoschAMS(DatabaseContext dbcontext, DatabaseContext dbcontextACE, string start, string end, LOGEVENT_STATE state, LOGEVENT_VALUETYPE type, string clientexternalid,
    string description, string[] company, string[] deviceid, string[] addresstag, string[] persclassid, string stringvalue, bool meal, HomeModel reports, string tagbisserver,
    string addresstagprefix, string addresstagsufix, ACCESSTYPE accesstype)
        {
            try
            {
                //string server = this.contextACE.GetHost();
                string devid = "";
                string cmpno = "";
                string persclass = "";

                //variável da nomenclatura da divisão no LogDivision.
                //Mantida para futura análise.
                //tagbisserver += "." 
                tagbisserver = "";

                //if (reports.DEVICEID != null && reports.DEVICEID.Length > 0)
                //{
                //    if (deviceid != null && deviceid.Length > 0)
                //    {
                //        string where = "";
                //        devid = "";
                //        foreach (string id in deviceid)
                //            where += "''" + id + "'', ";
                //        devid += where.Substring(0, where.Length - 2);
                //    }
                //}

                //if (addresstag != null && addresstag.Length > 0)
                //{
                //    string where = "";
                //    devid = "";
                //    foreach (string name in addresstag)
                //        where += "''" + addresstagprefix + name + addresstagsufix + "'', ";
                //    devid += where.Substring(0, where.Length - 2);
                //}

                //if (!String.IsNullOrEmpty(reports.CLIENTID))
                //{
                //    Clients cli = new Clients();
                //    if (!String.IsNullOrEmpty(reports.CLIENTID) && reports.CLIENTID != "0")
                //    {
                //        cli = (Clients)Clients.GetClientsClass(dbcontextACE, reports.CLIENTID);
                //        clientexternalid = cli.ExternID;
                //    }
                //}

                //if (company != null && company.Length > 0)
                //{
                //    string where = "";
                //    cmpno = "";
                //    foreach (string no in company)
                //        where += "''" + no + "'', ";
                //    cmpno += where.Substring(0, where.Length - 2);
                //}

                //if (persclassid != null && persclassid.Length > 0)
                //{
                //    string where = "";
                //    persclass = "";
                //    foreach (string no in persclassid)
                //        where += "''" + no + "'',";
                //    persclass += where.Substring(0, where.Length - 2) + "'";
                //}

                string sql = "";
                if (reports.AccessType != ACCESSTYPE.TIMEOUT)
                {
                    sql = String.Format("set dateformat 'dmy' select * from [Bosch.EventDb].dbo.vwAcessosGeral where data >= '{0}' and data <= '{1}' order by data",
                                        start, end);
                }
                else
                {
                    sql = String.Format("set dateformat 'dmy' select * from [Bosch.EventDb].dbo.vwEventosGeral where data >= '{0}' and data <= '{1}' and NEvento = 16777991 order by data",
                                        start, end);
                }


                StreamWriter w = new StreamWriter("AMS.txt", true);
                w.WriteLine(sql);
                w.Close();
                w = null;

                //if (String.IsNullOrEmpty(persclass))
                //{
                //    sql = String.Format("set dateformat 'dmy' exec BISEventLog.." + _arvoreOpcoes.SpAccessGranted + " {0}, '{1}', '{2}', {3}, {4}, {5}, {6}",
                //        String.IsNullOrEmpty(stringvalue) ? "null" : "'" + stringvalue + "'", start, end,
                //        String.IsNullOrEmpty(reports.CLIENTID) ? "null" : "'" + tagbisserver + description + "'",
                //        String.IsNullOrEmpty(devid) ? "null" : "'" + devid + "'",
                //        String.IsNullOrEmpty(cmpno) ? "null" : "'" + cmpno + "'",
                //        ((int)accesstype).ToString());
                //}
                //else
                //{
                //    sql = String.Format("set dateformat 'dmy' exec BISEventLog.." + _arvoreOpcoes.SpPersClassAccessGranted + "  {0}, '{1}', '{2}', {3}, {4}, {5}, '{6}', {7}",
                //    String.IsNullOrEmpty(stringvalue) ? "null" : "'" + stringvalue + "'",
                //    start,
                //    end,
                //    String.IsNullOrEmpty(reports.CLIENTID) ? "null" : "'" + tagbisserver + description + "'",
                //    String.IsNullOrEmpty(devid) ? "null" : "'" + devid + "'",
                //    String.IsNullOrEmpty(cmpno) ? "null" : "'" + cmpno + "'",
                //    String.IsNullOrEmpty(persclass) ? "null" : persclass,
                //   ((int)accesstype).ToString());
                //}

                //StreamWriter w = new StreamWriter("erro.txt", true);
                //w.WriteLine(sql);
                //w.Close();
                //w = null;

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> GetEventsBosch");
                w.Close();
                w = null;

                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region Functions Count
        /// <summary>
        /// Gera arquivo com os dados das refeições das catracas do refeitório.
        /// Os dados são apenas dos funcionários.
        /// </summary>
        /// <param name="connection">Conexão com o banco de dados.</param>
        /// <param name="filename">Nome do arquivo.</param>
        /// <param name="startdate">Data inicial da pesquisa.</param>
        /// <param name="enddate">Data Final da pesquisa.</param>
        /// <returns>Retorna a Datatable com os dados.</returns>
        public DataTable GetCountBath(DatabaseContext dbcontext)
        {
            try
            {
                string sql = "set dateformat 'dmy' select Dia = datepart(day, eventcreationtime), Total = count(*) / 2 from BISEventLog..LogEvent " +
                    "inner join BISEventLog..LogState on BISEventLog..LogState.Id = BISEventLog..LogEvent.stateId " +
                    "inner join BISEventLog..LogEventType on BISEventLog..LogEventType.ID = BISEventLog..LogEvent.eventTypeId  " +
                    "where LogEventType.ID = 1 and LogState.stateNumber in (4171) and eventCreationTime >= '01/10/2019' and eventCreationTime <= getdate() " +
                    "group by datepart(day, eventcreationtime)";

                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(sql);
                w.Close();
                w = null;

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Meal
        /// <summary>
        /// Retorna o relatório com o total das refeições.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="start">Data inicial do relatório.</param>
        /// <param name="end">Data final do relatório.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public  DataTable LoadTotalMeal(DatabaseContext dbcontext, string start, string end, string clientid)
        {
            try
            {
                string sql = String.Format("set dateformat 'dmy' exec BISEventLog..spREL_TotalMeal '{0}', '{1}', '{2}'", start, end, clientid);
                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna as refeições de acordo com os parâmetros.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="type">Tipo da refeição de acordo com MEALTYPE.</param>
        /// <param name="startdate">Início da pesquisa.</param>
        /// <param name="enddate">Fim da pesquisa.</param>
        /// <param name="clientid">ID do cliente para pesquisa.</param>
        /// <returns>Retorna a tabela com as refeições.</returns>
        public DataTable GetMealBosch(DatabaseContext dbcontext, DatabaseContext dbcontextACE, string defaultconfig, MEALTYPE type, string startdate, string enddate, string tagbisserver, string clientid)
        {
            try
            {
                string sql = "set dateformat 'dmy' select CPF, Nome, Empresa, Data = convert(varchar, data, 103) + ' ' + convert(varchar, data, 108), TipoRefeicao, EnderecoAcesso, Divisao ";

                if (defaultconfig.ToLower().Trim().Equals("solar"))
                    sql += ", re , Cargo = job, CCusto = costcentre, Departamento = department, Unidade = centraloffice, Local = nationality, TipoPessoa, Autorizador = AttendantName, " +
                    "AutorizadorCC = AttendantCostCentre, ValorRefeicao, Ticket, SiteCode, Cardno, TipoRefeicaoExtra, DtRefeicaoExtra = convert(varchar, DataRefeicaoExtra, 103), AutorizadorRefeicaoExtra, CCRefeicaoExtra ";
                else if (defaultconfig.ToLower().Trim().Equals("femsa"))
                    sql += ", CodigoCC, DescricaoCC, AreaRH ";

                sql += String.Format(" from HzBIS..tblAcessos where data >= '{0}' and data <= '{1}'", startdate, enddate);

                if (!String.IsNullOrEmpty(clientid) && clientid != "0")
                    sql += " and clientid = '" + clientid + "'";

                if (type != MEALTYPE.MEALTYPE_TODOS)
                {
                    switch (type)
                    {
                        case MEALTYPE.MEALTYPE_ALMOCO:
                            sql += " and TipoRefeicao = 'Almoço'";
                            break;
                        case MEALTYPE.MEALTYPE_CEIA:
                            sql += " and TipoRefeicao = 'Ceia'";
                            break;
                        case MEALTYPE.MEALTYPE_DESEJUM:
                            sql += " and TipoRefeicao = 'Desejum'";
                            break;
                        case MEALTYPE.MEALTYPE_JANTAR:
                            sql += " and TipoRefeicao = 'Jantar'";
                            break;
                    }
                }

                sql += " order by data, nome";
                StreamWriter w = new StreamWriter("SQLMeal.txt", true);
                w.WriteLine(sql);
                w.Close();
                w = null;

                DataTable table = dbcontext.LoadDatatable(dbcontext, sql);

                w = new StreamWriter("SQLMealTable.txt", true);
                w.WriteLine(table == null || table.Rows.Count < 1 ? "SEM REGISTRO" : table.Rows.Count.ToString());
                w.Close();
                w = null;

                return table;
            }
            catch (Exception ex)
            {
                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(ex.Message + " --> GetEventsBosch");
                w.Close();
                w = null;

                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
