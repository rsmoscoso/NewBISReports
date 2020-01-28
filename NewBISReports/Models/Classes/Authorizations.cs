using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    public class Authorizations
    {
        #region Variables
        /// <summary>
        /// ID do tipo da pessoa.
        /// </summary>
        public string AuthID { get; set; }
        /// <summary>
        /// Apelido da Empresa.
        /// </summary>
        public string Shortname { get; set; }
        /// <summary>
        /// Descrição do tipo da pessoa.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retornaos tipos de autorizações.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <returns></returns>
        public static List<Authorizations> GetAuthorizations(DatabaseContext dbcontext)
        {
            List<Authorizations> persons = new List<Authorizations>();
            try
            {
                string sql = "select AuthID, Shortname, Name from bsuser.authorizations order by shortname";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        persons = GlobalFunctions.ConvertDataTable<Authorizations>(table);
                }
                return persons;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Retorna as autorizações baseadas na pesquisa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static List<Authorizations> GetAuthorizations(DatabaseContext dbcontext, string clientid)
        {
            List<Authorizations> companies = new List<Authorizations>();
            try
            {
                string sql = "select AuthID, Shortname, Name from bsuser.Authorizations";

                if (!String.IsNullOrEmpty(clientid))
                    sql += " where clientid = '" + clientid + "'";

                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        companies = GlobalFunctions.ConvertDataTable<Authorizations>(table);
                }
                return companies;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}