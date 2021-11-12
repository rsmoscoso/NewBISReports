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
    /// Classe com as funções de pesquisa analítica dos acesso.
    /// </summary>
    public class RPTAnalytics
    {
        #region Functions SQL
        /// <summary>
        /// Retorna a string sql para a pesquisa dos eventos.
        /// Para o evento de acesso, no caso de depósito do cartão na urna, quando a opção
        /// de baixa automática está ativada, o evento gerado é o 4202 ao invés do 4101.
        /// O evento 4202 é de exclusão de cartão. O que garante o evento de acesso é o authgroupid = 10.
        /// </summary>
        /// <param name="start">Data inicial da pesquisa.</param>
        /// <param name="end">Data final da pesquisa.</param>
        /// <param name="state">Tipo do estado do relatório.</param>
        /// <param name="type">Tipo da varíavel da pesquisa (PERSID, PERSNO...)</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static string GetSQLEvent(string start, string end, LOGEVENT_STATE state, LOGEVENT_VALUETYPE type)
        {
            return String.Format("select LogEvent.ID from BISEventLog..LogEvent inner join BISEventLog..LogAddress on BISEventLog..LogEvent.addressId = BISEventLog..LogAddress.ID " +
                    "inner join BISEventLog..LogState on BISEventLog..LogState.Id = BISEventLog..LogEvent.stateId inner join BISEventLog..LogEventType on BISEventLog..LogEventType.ID = BISEventLog..LogEvent.eventTypeId " +
                    "inner join BISEventLog..LogEvent2Value on BISEventLog..LogEvent2Value.eventId = BISEventLog..LogEvent.ID " +
                    "inner join BISEventLog..LogEventValue with(index(IdxeventTypeIdstringValue)) on BISEventLog..LogEventValue.Id = BISEventLog..LogEvent2Value.valueId " +
                    "inner join BISEventLog..LogEventValueType on BISEventLog..LogEventValueType.ID = BISEventLog..LogEventValue.eventTypeId " +
                    "inner join BISEventLog..LogDivision on BISEventLog..LogDivision.ID = BISEventLog..LogEvent.divisionId " +
                    "where eventCreationTime >= '{0}' and eventCreationTime <= '{1}' and LogEventType.ID = 1 and stateNumber in ({2}) and LogEventValueType.eventValueName = '{3}'", start, end,
                    (state == LOGEVENT_STATE.LOGEVENTSTATE_ACCESSGRANTED ? ((int)state).ToString() + ",4202" : ((int)state).ToString()), LogEvent.GetValueType(type));
        }

        /// <summary>
        /// Retorna a condição para a pesquisa das refeições.
        /// </summary>
        /// <returns></returns>4101
        public static string GetSQLEventMeal()
        {
            return " and AddressTag collate SQL_Latin1_General_CP1_CI_AS in (select devicenamelogevent from HzBIS..tblAMCRefeicao)";
        }

        #endregion
        #region Functions LogEvent
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
        /// <param name="stringvalue">Desrição do item da pesquisa.</param>
        /// <param name="server">Servidor da instância do BIS_ACE.</param>
        /// <returns></returns>
        public static DataTable GetEventsBoschVisitor(DatabaseContext dbcontext, string start, string end, LOGEVENT_STATE state, LOGEVENT_VALUETYPE type, string clientexternalid,
            string description, string[] company, string[] deviceid, string stringvalue, string server)
        {
            try
            {
                string devid = "";
                string cmpno = "";
                if (deviceid != null && deviceid.Length > 0)
                {
                    if (deviceid != null && deviceid.Length > 0)
                    {
                        string where = "";
                        devid = "(";
                        foreach (string id in deviceid)
                            where += "AddressTag like ''%" + id + "%'' or ";
                        devid += where.Substring(0, where.Length - 3) + ")";
                    }
                }

                if (company != null && company.Length > 0)
                {
                    string where = "";
                    cmpno = "(";
                    foreach (string no in company)
                        where += "stringValue = ''" + no + "'' or ";
                    cmpno += where.Substring(0, where.Length - 3) + ")";
                }

                string sql = String.Format("set dateformat 'dmy' exec BISEventLog..spRPT_VisitorAccessGranted  {0}, '{1}', '{2}', {3}, {4}, {5}",
                    String.IsNullOrEmpty(stringvalue) ? "null" : "'" + stringvalue + "'", start, end,
                    String.IsNullOrEmpty(clientexternalid) ? "null" : "'" + clientexternalid + "'",
                    String.IsNullOrEmpty(devid) ? "null" : "'" + devid + "'",
                    String.IsNullOrEmpty(cmpno) ? "null" : "'" + cmpno + "'",
                    server);

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion


        #region Functions tblAcessos
        /// <summary>
        /// Gera arquivo com os dados das refeições das catracas do refeitório.
        /// Os dados são apenas dos funcionários.
        /// </summary>
        /// <param name="connection">Conexão com o banco de dados.</param>
        /// <param name="filename">Nome do arquivo.</param>
        /// <param name="startdate">Data inicial da pesquisa.</param>
        /// <param name="enddate">Data Final da pesquisa.</param>
        /// <returns>Retorna a Datatable com os dados.</returns>
        public static DataTable LoadMeals(DatabaseContext dbcontext, string filename, string startdate, string enddate)
        {
            try
            {
                string sql = "set dateformat 'dmy' select Matricula, data from HzBIS..tblAcessos " +
                    "where data >= '" + startdate + "' and data <= '" + enddate + "'  and TipoRefeicao <> 'Acesso Regular'";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna o relatório analítico da tabela tblAcessos.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="start">Data inicial do relatório.</param>
        /// <param name="end">Data final do relatório.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <param name="tipoempregado">Tipo da pessoa.</param>
        /// <param name="companyno">Nome da empresa.</param>
        /// <param name="deviceid">Lista de IDs do dispositivo.</param>
        /// <param name="type">Tipo do analítico: R - apenas refeição.</param>
        /// <returns></returns>
        public static DataTable LoadAnalyticsSolar(DatabaseContext dbcontext, string start, string end, string persid, string clientid, List<string> tipoempregado, string[] company, string[] deviceid, string type)
        {
            try
            {
                string sql = String.Format("set dateformat 'dmy' select Divisao, Nome, CPF, " +
                    "DataAcesso = convert(varchar, data, 103) + ' ' + convert(varchar, data, 108),  " +
                    "EnderecoAcesso, TipoRefeicao, Empresa from HzBIS..tblAcessos where data >= '{0}' and data <= '{1}'",
                    //Diogo - alteração no formato da data, já vem com hora e minuto do frontend
                    //start + " 00:00:00", end + " 23:59:59");
                    start + " 00:00:00", end + " 23:59:59");

                if (!String.IsNullOrEmpty(clientid) && clientid != "0")
                    sql += " and clientid = '" + clientid + "'";

                if (company != null && company.Length > 0)
                {
                    string devid = " and (";
                    foreach (string id in company)
                        devid += "empresa = '" + id + "' or ";
                    sql += devid.Substring(0, devid.Length - 3) + ")";
                }

                if (deviceid != null && deviceid.Length > 0)
                {
                    string devid = "in (";
                    foreach (string id in deviceid)
                        devid += "'" + id + "',";
                    sql += devid.Substring(0, devid.Length - 1) + ")";
                }
                if (tipoempregado != null && tipoempregado.Count > 0)
                {
                    string devid = " and TipoEmpregado in (";
                    foreach (string id in tipoempregado)
                        devid += "'" + id + "',";
                    sql += devid.Substring(0, devid.Length - 1) + ")";
                }
                if (!String.IsNullOrEmpty(persid))
                    sql += " and cpf = '" + persid + "'";
                if (type == "R")
                    sql += " and EnderecoAcesso in (select devicenamelogevent from HzBIS..tblAMCRefeicao)";
                sql += " order by data, nome";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna o relatório analítico da tabela tblAcessos.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="start">Data inicial do relatório.</param>
        /// <param name="end">Data final do relatório.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <param name="companyno">Nome da empresa.</param>
        /// <param name="deviceid">Lista de IDs do dispositivo.</param>
        /// <param name="type">Tipo do analítico: R - apenas refeição.</param>
        /// <returns></returns>
        public static DataTable LoadAnalytics(DatabaseContext dbcontext, string start, string end, string persid, string clientid, string companyno, string[] deviceid, string type)
        {
            try
            {
                string sql = String.Format("set dateformat 'dmy' select LocalAcesso, Data, Nome, Documento, TipoPessoa, TipoAcesso, Empresa from HzBIS..tblAcess where data >= '{0}' and data <= '{1}'",
                    start + " 00:00:00", end + " 23:59:59");

                if (!String.IsNullOrEmpty(clientid) && clientid != "0")
                    sql += " and clientid = '" + clientid + "'";
                if (!String.IsNullOrEmpty(companyno))
                    sql += " and empresa = '" + companyno + "'";
                if (deviceid != null && deviceid.Length > 0)
                {
                    string devid = "in (";
                    foreach (string id in deviceid)
                        devid += "'" + id + "',";
                    sql += devid.Substring(0, devid.Length - 1) + ")";
                }
                if (!String.IsNullOrEmpty(persid))
                    sql += " and documento = '" + persid + "'";
                if (type == "R")
                    sql += " and LocalAcesso in (select devicenamelogevent from HzBIS..tblAMCRefeicao)";
                sql += " order by data, nome";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch
            {
                return null;
            }
        }

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
        #endregion
    }
}
