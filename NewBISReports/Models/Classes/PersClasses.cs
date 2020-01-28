using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    /// <summary>
    /// Classe que gerencia a tabela Persclasses.
    /// </summary>
    public class PersClasses
    {
        #region Variables
        /// <summary>
        /// ID do tipo da pessoa.
        /// </summary>
        public string PERSCLASSID { get; set; }

        /// <summary>
        /// Descrição do tipo da pessoa.
        /// </summary>
        public string DisplayTextCustomer { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retornaos tipos de pessoa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <returns></returns>
        public static List<PersClasses> GetPersClasses(DatabaseContext dbcontext)
        {
            List<PersClasses> persons = new List<PersClasses>();
            try
            {
                string sql = "select PERSCLASSID, DisplayTextCustomer from bsuser.persclasses order by displaytextcustomer";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        persons = GlobalFunctions.ConvertDataTable<PersClasses>(table);
                }
                return persons;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retornaos tipos de pessoa baseado na pesquisa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="displaytextcustomer">Descrição do tipo de pessoa.</param>
        /// <returns></returns>
        public static List<PersClasses> GetPersClasses(DatabaseContext dbcontext, string displaytextcustomer)
        {
            List<PersClasses> persons = new List<PersClasses>();
            try
            {
                string sql = string.Format("select PERSCLASSID, DisplayTextCustomer from bsuser.persclasses where displaytextcustomer like '%{0}%' order by displaytextcustomer",
                    displaytextcustomer);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        persons = GlobalFunctions.ConvertDataTable<PersClasses>(table);
                }
                return persons;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna a descrição do tipo de pessoa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="persclassid">ID do tipo de pessoa.</param>
        /// <returns></returns>
        public static string GetDisplayTextCustomer(DatabaseContext dbcontext, string persclassid)
        {
            string retval = "";
            try
            {
                string sql = string.Format("select DisplayTextCustomer from bsuser.persclasses where persclassid = '{0}'", persclassid);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null && table.Rows.Count > 0)
                        retval = table.Rows[0]["displaytextcustomer"].ToString();
                }
                return retval;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
