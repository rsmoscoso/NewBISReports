using NewBISReports.Models.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static NewBISReports.Models.Classes.LogEvent;

namespace NewBISReports.Models.Reports
{
    /// <summary>
    /// Classe com as pesquisas dos eventos.
    /// </summary>
    public class RPTBS_Analytics
    {
        #region Analytics
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
        public static DataTable GetEventsBosch(DatabaseContext dbcontext, DatabaseContext dbcontextACE, string start, string end, LOGEVENT_STATE state, LOGEVENT_VALUETYPE type, string clientexternalid,
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
                        cli = Clients.GetClientsClass(dbcontextACE, reports.CLIENTID);
                        clientexternalid = cli.ExternID;
                    }
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

                string sql = "";

                if (String.IsNullOrEmpty(persclass))
                {
                    sql = String.Format("set dateformat 'dmy' exec BISEventLog..spRPT_AccessGrantedWithCode  {0}, '{1}', '{2}', {3}, {4}, {5}, {6}",
                        String.IsNullOrEmpty(stringvalue) ? "null" : "'" + stringvalue + "'", start, end,
                        String.IsNullOrEmpty(reports.CLIENTID) ? "null" : "'" + tagbisserver + description + "'",
                        String.IsNullOrEmpty(devid) ? "null" : "'" + devid + "'",
                        String.IsNullOrEmpty(cmpno) ? "null" : "'" + cmpno + "'",
                        ((int)accesstype).ToString());
                }
                else
                {
                    sql = String.Format("set dateformat 'dmy' exec BISEventLog..spRPT_PersClassAccessGranted  {0}, '{1}', '{2}', {3}, {4}, {5}, '{6}'",
                    String.IsNullOrEmpty(stringvalue) ? "null" : "'" + stringvalue + "'", start, end,
                    String.IsNullOrEmpty(reports.CLIENTID) ? "null" : "'" + tagbisserver + description + "'",
                    String.IsNullOrEmpty(devid) ? "null" : "'" + devid + "'",
                    String.IsNullOrEmpty(cmpno) ? "null" : "'" + cmpno + "'",
                    String.IsNullOrEmpty(persclass) ? "null" : persclass);
                }

                StreamWriter w = new StreamWriter("erro.txt", true);
                w.WriteLine(sql);
                w.Close();
                w = null;

                return GlobalFunctions.RemoveTrash(dbcontext.LoadDatatable(dbcontext, sql), null);
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
        public static DataTable GetCountBath(DatabaseContext dbcontext)
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
        public static DataTable LoadTotalMeal(DatabaseContext dbcontext, string start, string end, string clientid)
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
        public static DataTable GetMealBosch(DatabaseContext dbcontext, DatabaseContext dbcontextACE, MEALTYPE type, string startdate, string enddate, string tagbisserver, string clientid)
        {
            try
            {
                string sql = String.Format("set dateformat 'dmy' select CPF, Nome, Empresa, Data = convert(varchar, data, 103) + ' ' + convert(varchar, data, 108), TipoRefeicao, EnderecoAcesso, Divisao from HzBIS..tblAcessos where data >= '{0}' and data <= '{1}'", startdate, enddate);

                if (!String.IsNullOrEmpty(clientid) && clientid != "0")
                    sql += " and divisao = '" + tagbisserver + "." + clientid + "'";

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
    }
}
