using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    public class Areas
    {
        #region Variables
        /// <summary>
        /// ID da área.
        /// </summary>
        public string AREAID { get; set; }
        /// <summary>
        /// Nome da área.
        /// </summary>
        public string NAME { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retornaos tipos de autorizações.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <returns></returns>
        public static List<Areas> GetAreas(DatabaseContext dbcontext)
        {
            List<Areas> areas = new List<Areas>();
            try
            {
                string sql = "select AREAID, NAME from bsuser.areas order by name";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        areas = GlobalFunctions.ConvertDataTable<Areas>(table);
                }
                return areas;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
