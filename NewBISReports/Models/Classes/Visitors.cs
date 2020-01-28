using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    /// <summary>
    /// Classe que gerencia a tabela Visitors.
    /// </summary>
    public class Visitors
    {
        /// <summary>
        /// Retorna o nome do visitante.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <returns></returns>
        public static string GetVisitorName(DatabaseContext dbcontext, string persid)
        {
            string retval = "";
            try
            {
                string sql = String.Format("select Nome = firstname + ' ' + lastname from bsuser.persons where persid = '{0}'", persid);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null && table.Rows.Count > 0)
                        retval = table.Rows[0]["Nome"].ToString();
                }
                return retval;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
