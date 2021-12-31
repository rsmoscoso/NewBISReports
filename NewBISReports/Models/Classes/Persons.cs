using HzBISCommands;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    /// <summary>
    /// Classe que gerencia a tabela Persons.
    /// </summary>
    public class Persons
    {
        #region Variables
        /// <summary>
        /// PERSID da pessoa.
        /// </summary>
        public string Persid { get; set; }
        /// <summary>
        /// Primeiro nome da pessoa.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Sobrenome da pessoa.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Persno da pessoa.
        /// </summary>
        public string Persno { get; set; }
        /// <summary>
        /// Nome da Pessoa.
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// Tipo da pessoa.
        /// </summary>
        public string PersClass { get; set; }
        /// <summary>
        /// Documento da pessoa.
        /// </summary>
        public string Documento { get; set; }
        /// <summary>
        /// CPF da pessoa.
        /// </summary>
        public string CPF { get; set; }
        /// <summary>
        /// Data de aniversário.
        /// </summary>
        public string DataofBirth { get; set; }
        public string cmpDtInicio { get; set; }
        public string cmpDtTermino { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retorna a pessoa através do PERSID.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <returns></returns>
        public static BSPersonsInfo GetPersonsPERSID(DatabaseContext dbcontext, string persid)
        {
            BSPersonsInfo persons = new BSPersonsInfo();
            try
            {
                string sql = String.Format("select * from bsuser.persons where persid = '{0}'", persid);
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    
                    if (table != null)
                    {
                        persons = GlobalFunctions.ConvertDataTable<BSPersonsInfo>(table)[0];
                        persons.NOME = String.Format("{0} {1}", persons.FIRSTNAME.Trim(), persons.LASTNAME.Trim());
                        persons.CARDS = Cards.GetCards(dbcontext, persid);
                        persons.AUTHORIZATIONS = Authorizations.GetAuthorizationsPERSID(dbcontext, persid);
                        persons.CUSTOMFIELDS = CustomFields.GetCustomFields(dbcontext, persid);
                        if (persons.CUSTOMFIELDS != null && persons.CUSTOMFIELDS.Count > 0)
                        {
                            foreach(BSAdditionalFieldInfo addfield in persons.CUSTOMFIELDS)
                            {
                                if (addfield.LABEL.ToLower().Equals("cpf"))
                                    persons.CPF = addfield.VALUE.ToString();
                                else if (addfield.LABEL.ToLower().Equals("uf"))
                                    persons.UF = addfield.VALUE.ToString();
                            }
                        }
                    }
                }
                return persons;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna a coleção com as pessoas pesquisadas no BIS.
        /// As pessoas podem ser Pessoas ou Visitantes.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="searchtype">Tipo de pesquisa pra pessoa.</param>
        /// <param name="clientid">ID da unidade.</param>
        /// <param name="persclassid">ID do tipo da pessoa.</param>
        /// <param name="companyid">ID da empresa.</param>
        /// <param name="search">Filtro da pesquisa.</param>
        /// <returns></returns>
        public static List<Persons> GetPersons(DatabaseContext dbcontext, SEARCHPERSONS searchtype, string clientid, string[] persclassid, string search)
        {
            List<Persons> persons = new List<Persons>();
            try
            {
                string sql = "";

                // apenas pesquisa pessoas com cartões cadastrados.
                if (searchtype == SEARCHPERSONS.SEARCHPERSONS_PERSNO || searchtype == SEARCHPERSONS.SEARCHPERSONS_NAME)
                //fix de bug de filtro de funcionarios externos e vigilantes                
                    //sql = "select distinct Persid = per.persid, Documento = persno, Nome = firstname + ' ' + lastname from bsuser.persons per left outer join bsuser.cards cd on cd.persid = per.persid where per.status = 1 and persclass = 'E' and cd.datedeleted is null ";
                    sql = "select distinct Persid = per.persid, Documento = persno, Nome = firstname + ' ' + lastname from bsuser.persons per left outer join bsuser.cards cd on cd.persid = per.persid where per.status = 1 "+ (persclassid == null || persclassid.Length < 1 ? " and persclass = 'E' " : null ) + "and cd.datedeleted is null ";
                else if (searchtype == SEARCHPERSONS.SEARCHPERSONS_PASSPORTNO || searchtype == SEARCHPERSONS.SEARCHPERSONS_NAMEVISITOR)
                    sql = "select Persid = per.persid, Documento = passportno, Nome = firstname + ' ' + lastname from bsuser.persons per" +
                        " inner join bsuser.visitors vis on vis.persid = per.persid where per.status = 1 and persclass = 'V'";

                if (!String.IsNullOrEmpty(search))
                {
                    if (searchtype == SEARCHPERSONS.SEARCHPERSONS_PERSNO)
                        sql += " and (persno like '%" + search + "%' or idnumber like '%" + search + "%')";
                    else if (searchtype == SEARCHPERSONS.SEARCHPERSONS_PASSPORTNO)
                        sql += " and (passportno like '%" + search + "%')";
                    else if (searchtype == SEARCHPERSONS.SEARCHPERSONS_NAME || searchtype == SEARCHPERSONS.SEARCHPERSONS_NAMEVISITOR)
                    {
                        string fn = "";
                        string ln = "";
                        GlobalFunctions.ParseName(search, out fn, out ln);
                        if (!String.IsNullOrEmpty(fn) && !String.IsNullOrEmpty(ln))
                            sql += " and (firstname like '%" + fn + "%' and (lastname like '%" + ln + "%'))";
                        else if (!String.IsNullOrEmpty(fn) && String.IsNullOrEmpty(ln))
                            sql += " and (firstname like '%" + fn + "%' or (lastname like '%" + fn + "%'))";
                    }
                }
                //fix de bug de filtro de funcionarios externos e vigilantes 
                //if (persclassid != null && persclassid.Length > 0)
                if (persclassid != null && persclassid.Length > 0 &&
                    (searchtype == SEARCHPERSONS.SEARCHPERSONS_PERSNO || searchtype == SEARCHPERSONS.SEARCHPERSONS_NAME))
                {
                    if (persclassid != null && persclassid.Length > 0)
                    {
                        string devid = " and (";
                        foreach (string id in persclassid)
                            devid += "per.persclassid = '" + id + "' or ";
                        sql += devid.Substring(0, devid.Length - 3) + ")";
                    }
                }


                if (!String.IsNullOrEmpty(clientid))
                    sql += " and per.clientid = '" + clientid[0] + "'";

                sql += " order by Nome";

                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        persons = GlobalFunctions.ConvertDataTable<Persons>(table);
                }
                return persons;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna a coleção com as pessoas pesquisadas no BIS.
        /// As pessoas podem ser Pessoas ou Visitantes.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="search">Filtro da pesquisa.</param>
        /// <returns></returns>
        public static List<Persons> GetPersons(DatabaseContext dbcontext, SEARCHPERSONS searchtype, string search)
        {
            List<Persons> persons = new List<Persons>();
            try
            {
                string sql = "";

                if (searchtype == SEARCHPERSONS.SEARCHPERSONS_PERSNO || searchtype == SEARCHPERSONS.SEARCHPERSONS_NAME)
                    sql = "select Persid = per.persid, Documento = persno, Nome = firstname + ' ' + lastname from bsuser.persons per  where per.status = 1";
                else if (searchtype == SEARCHPERSONS.SEARCHPERSONS_DOCUMENT || searchtype == SEARCHPERSONS.SEARCHPERSONS_NAMEVISITOR)
                    sql = "select Persid = per.persid, Documento = persno, Nome = firstname + ' ' + lastname from bsuser.persons per" +
                        "inner join bsuser.visitors vis on vis.persid = per.persid where per.status = 1";

                if (!String.IsNullOrEmpty(search))
                {
                    if (searchtype == SEARCHPERSONS.SEARCHPERSONS_PERSNO)
                        sql += " and (persno like '%" + search + "%' or idnumber like '%" + search + "%')";
                    else if (searchtype == SEARCHPERSONS.SEARCHPERSONS_DOCUMENT)
                        sql += " and (passportno like '%" + search + "%')";
                    else if (searchtype == SEARCHPERSONS.SEARCHPERSONS_NAME || searchtype == SEARCHPERSONS.SEARCHPERSONS_NAMEVISITOR)
                    {
                        string fn = "";
                        string ln = "";
                        GlobalFunctions.ParseName(search, out fn, out ln);
                        if (!String.IsNullOrEmpty(fn) && !String.IsNullOrEmpty(ln))
                            sql += " and (firstname like '%" + fn + "%' and (lastname like '%" + ln + "%'))";
                        else if (!String.IsNullOrEmpty(fn) && String.IsNullOrEmpty(ln))
                            sql += " and (firstname like '%" + fn + "%' or (lastname like '%" + fn + "%'))";
                    }
                }

                sql += " order by Nome";

                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                        persons = GlobalFunctions.ConvertDataTable<Persons>(table);
                }
                return persons;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Verifica a existência da photo da pessoa.
        /// </summary>
        /// <param name="path">Local onde se encontram os arquivos das fotos. O carácter / deve ser o último.</param>
        /// <param name="persid">ID da pessoa.</param>
        /// <returns></returns>
        public static bool ExistPhoto(string path, string persid)
        {
            try
            {
                return File.Exists(path + persid + ".jpg");
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public Persons()
        {
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="persno">Persno da pessoa.</param>
        /// <param name="nome">Nome da pessoa.</param>
        public Persons(string persno, string nome)
        {
            this.Persno = persno;
            this.Nome = nome;
        }
        #endregion
    }
}
