using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    /// <summary>
    /// Classe que gerencia a tabela Clients.
    /// </summary>
    public class Clients
    {
        #region Variables
        /// <summary>
        /// ID do cliente.
        /// </summary>
        public string CLIENTID { get; set; }
        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ID extero do cliente.
        /// </summary>
        public string ExternID { get; set; }
        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string Name { get; set; }
        #endregion


        #region Functions
        /// <summary>
        /// Retorna a descrição do cliente.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static string GetClientDescription(DatabaseContext dbcontext, string clientid)
        {
            string retval = "";
            try
            {
                string sql = "select Description from bsuser.clients where clientid = '" + clientid + "'";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        retval = table.Rows[0]["description"].ToString();
                    }
                }
                return retval = !String.IsNullOrEmpty(retval) ? retval : "";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna o nome do cliente.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static string GetClientName(DatabaseContext dbcontext, string clientid)
        {
            string retval = "";
            try
            {
                string sql = "select Name from bsuser.clients where clientid = '" + clientid + "'";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        retval = table.Rows[0]["Name"].ToString();
                    }
                }
                return retval = !String.IsNullOrEmpty(retval) ? retval : "";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna os clientes..
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="description">Nome do cliente para pesquisa.</param>
        /// <returns></returns>
        public static List<Clients> GetClients(DatabaseContext dbcontext, string description)
        {
            List<Clients> persons = new List<Clients>();
            try
            {
                string sql = String.Format("select CLIENTID, Description from bsuser.clients where description like '%{0}%' order by description", description);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        persons = GlobalFunctions.ConvertDataTable<Clients>(table);
                }
                return persons;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna os clientes..
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <returns></returns>
        public static List<Clients> GetClients(DatabaseContext dbcontext)
        {
            List<Clients> persons = new List<Clients>();
            try
            {
                string sql = String.Format("select CLIENTID, Description from bsuser.clients order by description");
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        persons = GlobalFunctions.ConvertDataTable<Clients>(table);
                }
                return persons;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna os dados do cliente.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static Clients GetClientsClass(DatabaseContext dbcontext, string clientid)
        {
            Clients retval = new Clients();
            try
            {
                string sql = "select * from bsuser.clients where clientid = '" + clientid + "'";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                    {
                        retval.CLIENTID = table.Rows[0]["clientid"].ToString();
                        retval.Description = table.Rows[0]["description"].ToString();
                        retval.ExternID = table.Rows[0]["externid"].ToString();
                        retval.Name = table.Rows[0]["Name"].ToString();
                    }
                }
                return retval;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
