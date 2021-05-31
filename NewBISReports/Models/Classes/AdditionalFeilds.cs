using HzBISCommands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    public class CustomFields : BSAdditionalFieldInfo
    {
        /// <summary>
        /// Retorna os campos adicionais da pessoa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <returns></returns>
        public static List<BSAdditionalFieldInfo> GetCustomFields(DatabaseContext dbcontext, string persid)
        {
            List<BSAdditionalFieldInfo> retval = new List<BSAdditionalFieldInfo>();
            try
            {
                string sql = String.Format("select addf.ID, LABEL, VALUE from bsuser.persons per inner join bsuser.ADDITIONALFIELDS addf on addf.persid = per.persid " +
                    "inner join bsuser.ADDITIONALFIELDDESCRIPTORS descf on descf.id = addf.fielddescid where per.persid = '{0}'", persid);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSAdditionalFieldInfo>(table);
                }
                return retval;
            }
            catch
            {
                return null;
            }
        }
    }
}
