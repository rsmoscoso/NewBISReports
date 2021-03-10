using HzBISCommands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace NewBISReports.Models.Classes
{
    public class Company : BSCompaniesInfo
    {
        #region Variables
        /// <summary>
        /// ID do tipo da pessoa.
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// Apelido da Empresa.
        /// </summary>
        public string CompanyNO { get; set; }
        /// <summary>
        /// Descrição do tipo da pessoa.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retornaos tipos de pessoa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <returns></returns>
        public static List<BSCompaniesInfo> GetCompanies(DatabaseContext dbcontext)
        {
            List<BSCompaniesInfo> retval = new List<BSCompaniesInfo>();
            try
            {
                string sql = "select CompanyID, CompanyNO, Name from bsuser.companies order by Name";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSCompaniesInfo>(table);
                }
                return retval;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna as empresas baseadas na pesquisa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="Name">Nome da empresa.</param>
        /// <returns></returns>
        public static List<Company> GetCompanies(DatabaseContext dbcontext, string Name)
        {
            List<Company> companies = new List<Company>();
            try
            {
                string sql = string.Format("select CompanyID, CompanyNO, Name from bsuser.Companies where Name like '%{0}%' order by Name",
                    Name);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        companies = GlobalFunctions.ConvertDataTable<Company>(table);
                }
                return companies;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna a empresa da pessoa.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <returns></returns>
        public static BSCompaniesInfo GetCompaniesPERSID(DatabaseContext dbcontext, string persid)
        {
            BSCompaniesInfo retval = new BSCompaniesInfo();
            try
            {
                string sql = string.Format("select COMPANYID, COMPANYNO, NAME from bsuser.Companies where persid = '{0}'",
                    persid);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        retval = GlobalFunctions.ConvertDataTable<BSCompaniesInfo>(table)[0];
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