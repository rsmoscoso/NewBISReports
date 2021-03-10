using HzBISCommands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    public class Cards : BSCardsInfo
    {
        /// <summary>
        /// Retorna os cartões ativsos da pessoa.
        /// </summary>
        /// <param name="dbcontext">Objeto do banco de dados.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <returns></returns>
        public static List<BSCardsInfo> GetCards(DatabaseContext dbcontext, string persid)
        {
            List<BSCardsInfo> retval = null;
            try
            {
                string sql = String.Format("select CODEDATA = convert(varchar, cast(convert(varbinary(4), codedata) as int)), CARDNO = convert(varchar, cast(convert(varbinary(8), codedata) as int)), STATUS, CLIENTID, PERSID, CARDID " +
                    "from bsuser.cards where persid = '{0}' and status = 1", persid);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSCardsInfo>(table);
                }

                return retval;
            }
            catch (Exception ex)
            {

                return retval;
            }
        }
    }
}
