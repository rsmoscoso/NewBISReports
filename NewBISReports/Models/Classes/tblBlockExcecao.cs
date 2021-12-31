using NewBISReports.Models.Excecao;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    /// <summary>
    /// Classe que gerencia a tabela tblExcecao.
    /// </summary>
    public class tblBlockExcecao
    {
        #region Variables
        /// <summary>
        /// Persid da pessoa com exceção no bloqueio.
        /// </summary>
        public string PERSID { get; set; }
        /// <summary>
        /// Data do início do bloqueio.
        /// </summary>
        public string cmpDtInicio { get; set; }
        /// <summary>
        /// Data do término do bloqueio.
        /// </summary>
        public string cmpDtTermino { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retorna a tabela com todos as exceções baseadas na pessoa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="perisd">ID da pessoa.</param>
        /// <param name="databasename">Nome do banco de dados.</param>
        /// <returns></returns>
        public static DataTable LoadExceptionsDt(DatabaseContext dbcontext, string persid, string databasename)
        {
            try
            {
                return dbcontext.LoadDatatable(dbcontext, "select bl.persid, Nome = firstname + ' ' + lastname, cmpDtInicio = convert(varchar, cmpDtInicio, 103), " +
                    "cmpDtTermino = convert(varchar, cmpDtTermino, 103) from " + databasename + "..tblblockexcecao bl " +
                    "inner join acedb.bsuser.persons p on bl.persid = p.persid where p.persid = '" + persid + "'");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetPersid(DatabaseContext dbcontext, string persno)
        {
            string persid = null;
            try
            {
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, String.Format("select persid from bsuser.persons where persno = '{0}'", persno)))
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        persid = table.Rows[0]["persid"].ToString();
                    }
                }

                return persid;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool Save(DatabaseContext dbcontext, string persid, string cmpdtinicio, string cmpdttermino)
        {
            try
            {
                DbCommand cmd = dbcontext.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = String.Format("set dateformat 'dmy' exec HzRH..spIncluirBlockExcecao '{0}', '{1}', '{2}'", persid, 
                    cmpdtinicio, cmpdttermino);
                cmd.CommandType = System.Data.CommandType.Text;

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// Retorna a tabela com todos as exceções.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="databasename">Nome do banco de dados.</param>
        /// <returns></returns>
        public static List<Persons> LoadExceptions(DatabaseContext dbcontext, string databasename)
        {
            List<Persons> retval = new List<Persons>();
            try
            {
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, "select Persid = bl.persid, Nome = firstname + ' ' + lastname, cmpDtInicio = convert(varchar, cmpDtInicio, 103) + ' ' " +
                    "convert(varchar, cmpDtInicio, 108), cmpDtTermino = convert(varchar, cmpDtTermino, 103) + ' ' + convert(varchar, cmpDtTermino, 108) from " + databasename + "..tblblockexcecao bl " +
                    "inner join acedb.bsuser.persons p on bl.persid = p.persid order by Nome"))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<Persons>(table);
                }

                return retval;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Persons> LoadExceptions(DatabaseContext dbcontext, string databasename, string data)
        {
            List<Persons> retval = new List<Persons>();
            try
            {
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, String.Format("select Persid = bl.persid, Nome = isnull(firstname, '') + ' ' + isnull(lastname, ''), cmpDtInicio = convert(varchar, cmpDtInicio, 103) + ' ' + " +
                    "convert(varchar, cmpDtInicio, 108), cmpDtTermino = convert(varchar, cmpDtTermino, 103) + ' ' + convert(varchar, cmpDtTermino, 108) from {0}..tblblockexcecao bl " +
                    "inner join acedb.bsuser.persons p on bl.persid = p.persid where convert(varchar, cmpDtInclusao, 103) = convert(varchar, getdate(), 103) order by Nome", databasename)))
                { 
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<Persons>(table);
                }

                return retval;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Exclui uma exceção existente.
        /// </summary>
        /// <param name="context">Conexão com o banco de dados.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <returns></returns>
        public static bool Delete(DatabaseContext context, string persid)
        {
            try
            {
                DbCommand cmd = context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "spExcluirBlockExcecao";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@persid", SqlDbType.VarChar) { Value = persid });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adiciona uma nova exceção.
        /// </summary>
        /// <param name="context">Conexão com o banco de dados.</param>
        /// <param name="model">Classe com os parâmetros da exceção.</param>
        /// <returns></returns>
        public static bool New(DatabaseContext context, ExcecaoModel model)
        {
            try
            {
                DbCommand cmd = context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "spIncluirBlockExcecao";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@persid", SqlDbType.VarChar) { Value = model.PERSIDBLOCK });
                cmd.Parameters.Add(new SqlParameter("@dataini", SqlDbType.VarChar) { Value = model.StartDate });
                cmd.Parameters.Add(new SqlParameter("@dataend", SqlDbType.VarChar) { Value = model.EndDate });

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
