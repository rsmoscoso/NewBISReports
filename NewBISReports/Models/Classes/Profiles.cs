using HzBISCommands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    public class Profiles
    {
        /// <summary>
        /// Pesquisa os perfis no BIS.
        /// </summary>
        /// <param name="connection">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <param name="errormessage">Mensagem de erro.</param>
        /// <returns>Retorna Datatable com os dados das pessoas. Se houver erro,
        /// a propriedade ErrorMessage é preenchida.</returns>
        public static List<BSProfilesInfo> GetProfiles(DatabaseContext dbcontext, string clientid)
        {
            try
            {
                List<BSProfilesInfo> retval = null;

                string sql = "select * from bsuser.authprofiles";
                if (!String.IsNullOrEmpty(clientid))
                    sql += String.Format(" where clientid = '{0}'", clientid);

                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSProfilesInfo>(table);
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
