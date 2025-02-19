﻿using HzBISCommands;
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
    public class Clients : BSClientsInfo
    {
        #region Variables
        public string ExternID { get; set; }
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
        public static List<BSClientsInfo> GetClients(DatabaseContext dbcontext, string description)
        {
            List<BSClientsInfo> retval = new List<BSClientsInfo>();
            try
            {
                string sql = String.Format("select CLIENTID, DESCRIPTION from bsuser.clients where description like '%{0}%' order by description", description);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSClientsInfo>(table);
                }
                return retval;
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
        public static List<BSClientsInfo> BSClientsInfo(DatabaseContext dbcontext)
        {
            List<BSClientsInfo> retval = new List<BSClientsInfo>();
            try
            {
                string sql = String.Format("select CLIENTID, Description from bsuser.clients order by description");
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSClientsInfo>(table);
                }
                return retval;
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
        public static List<BSClientsInfo> GetClients(DatabaseContext dbcontext)
        {
            List<BSClientsInfo> retval = new List<BSClientsInfo>();
            try
            {
                string sql = String.Format("select CLIENTID, DESCRIPTION, NAME from bsuser.clients order by description");
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSClientsInfo>(table);
                }
                return retval;
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
        public static BSClientsInfo GetClientsClass(DatabaseContext dbcontext, string clientid)
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
                        retval.DESCRIPTION = table.Rows[0]["description"].ToString();
                        retval.ExternID = table.Rows[0]["externid"].ToString();
                        retval.NAME = table.Rows[0]["Name"].ToString();
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
