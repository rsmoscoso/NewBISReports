using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NewBISReports.Models.Reports
{
    public class RPTBS_Acedb
    {
        #region SOLAR
        #endregion

        #region Functions
        /// <summary>
        /// Retorna os QRCodes dos visitantes.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="cmpDocumento">Documento do Visitante.</param>
        /// <param name="cmpNoVisitante">Nome do Visitante.</param>
        /// <returns></returns>
        public static DataTable LoadReaderAuthorization(DatabaseContext dbcontext, string clientid, string[] authid)
        {
            try
            {
                bool bWhere = false;
                string sql = "select AuthName = auth.shortname, Description = dev.description, DisplayText = dev.displaytext, Type = dev.type from bsuser.RCPPERAUTH rcp " +
                    "inner join bsuser.authorizations auth on auth.authid = rcp.authid inner join bsuser.devicegroups devgrp on devgrp.devgroupid = rcp.devgroupid " +
                    "inner join bsuser.DEVICESINGROUP devin on devin.devgroupid = devgrp.devgroupid inner join bsuser.devices dev on dev.id = devin.devid where dev.type = 'wie1' ";

                if (authid != null && authid.Length > 0)
                {
                    string where = " and auth.authid in (";
                    foreach (string id in authid)
                        where += "'" + id + "',";
                    sql += where.Substring(0, where.Length - 1) + ")";
                    bWhere = true;
                }

                if (!String.IsNullOrEmpty(clientid))
                    sql += " and auth.clientid = '" + clientid + "'";

                sql += " order by auth.shortname";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna os QRCodes dos visitantes.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="cmpDocumento">Documento do Visitante.</param>
        /// <param name="cmpNoVisitante">Nome do Visitante.</param>
        /// <returns></returns>
        public static DataTable LoadVisitorQRCode(DatabaseContext dbcontext, string cmpNoVisitante)
        {
            try
            {
                string sql = "select Autorizador = cmpNomeAutorizador, Nome = cmpNomeVisitante, Documento = cmpDocumento, NCartao = cmpCardNo, Email = cmpEmail, " +
                    " Data = convert(varchar, cmpDataHora, 103) + ' ' + convert(varchar, cmpDataHora, 108) from hzFortknox.dbo.tblLogQRCODE ";

                if (!String.IsNullOrEmpty(cmpNoVisitante))
                    sql += String.Format(" where cmpNomeVisitante = '{0}'", cmpNoVisitante);

                sql += " order by Nome";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Retorna as autorizações das pessoas.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static DataTable LoadPersonAuths(DatabaseContext dbcontext, string clientid, string[] authid)
        {
            try
            {
                string sql = "select Documento = persno, Nome = firstname + ' ' + lastname, Unidade = cli.Name, Autorizacao = auth.shortname from bsuser.persons per " +
                    "inner join bsuser.clients cli on per.clientid = cli.clientid inner join bsuser.authperperson authper on authper.persid = per.persid " +
                    "inner join bsuser.authorizations auth on auth.authid = authper.authid where persclass = 'E' and per.status = 1";

                if (!String.IsNullOrEmpty(clientid))
                    sql += String.Format(" and cli.clientid = '{0}'", clientid);

                if (authid != null && authid.Length > 0)
                {
                    string and = " and auth.authid in (";
                    foreach (string auth in authid)
                        and += "'" + auth + "',";
                    and = and.Substring(0, and.Length - 1) + ")";
                    sql += and;
                }
                sql += " order by cli.Name, Nome";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna os perfis das pessoas.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static DataTable LoadPersonProfiles(DatabaseContext dbcontext, string clientid, string[] authid)
        {
            try
            {
                string sql = "select Documento = persno, Nome = firstname + ' ' + lastname, Unidade = cli.Name, Perfil = authpro.shortname from bsuser.authperprofile perpro " +
                    "inner join bsuser.authprofiles pro on perpro.PROFILEID = pro.PROFILEID inner join bsuser.AUTHPERPERSON aper on aper.AUTHID = perpro.AUTHID " +
                    "inner join bsuser.PERSONS per on aper.PERSID = per.persid inner join bsuser.clients cli on per.clientid = cli.clientid " +
                    "inner join bsuser.authprofiles authpro on perpro.PROFILEID = authpro.PROFILEID " +
                    "where persclass = 'E' and per.status = 1";

                if (!String.IsNullOrEmpty(clientid))
                    sql += String.Format(" and cli.clientid = '{0}'", clientid);

                sql += " order by cli.Name, Nome";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna as autorizações das pessoas.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static DataTable LoadAllLocked(DatabaseContext dbcontext, string clientid)
        {
            try
            {
                string sql = "select Documento = persno, Nome = firstname + ' ' + lastname, Motivo = causeoflock,  " +
                    "DataInicio = right(lockedfrom, 2) + '/' + substring(lockedfrom, 5, 2) + '/' + left(lockedfrom, 4),  " +
                    "DataTermino = right(lockeduntil, 2) + '/' + substring(lockeduntil, 5, 2) + '/' + left(lockeduntil, 4), cli.Name  from bsuser.persons per " +
                    "inner join bsuser.lockouts loc on per.persid = loc.persid " +
                    "inner join bsuser.clients cli on cli.clientid = per.clientid where per.status = 1 ";

                if (!String.IsNullOrEmpty(clientid))
                    sql += String.Format(" and cli.clientid = '{0}'", clientid);

                sql += " order by Nome, cli.Name";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna as autorizações das pessoas.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static DataTable LoadAllVisitors(DatabaseContext dbcontext, string clientid)
        {
            try
            {
                string sql = "select Documento = passportno, Nome = firstname + ' ' + lastname from bsuser.persons per  " +
                    "inner join bsuser.visitors vis on vis.persid = per.persid " +
                    "where per.status = 1 ";

                if (!String.IsNullOrEmpty(clientid))
                    sql += String.Format(" and cli.clientid = '{0}'", clientid);

                sql += " order by Nome, cli.Name";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna as pessoas por edifício no MJ.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="company">Nome da empresa.</param>
        /// <returns></returns>
        public static DataTable LoadAllPerson(DatabaseContext dbcontext, BSRPTFields reportfields, List<BSRPTCustomFields> customfields, string persno, string[] persclassid, string clientid, string[] company)
        {
            try
            {
                bool bBio = false;
                string cmpno = "";
                string sql = "select Nome = isnull(firstname, '') + ' ' + isnull(lastname, ''), TipoPessoa = case when persclass = 'E' then 'FUNCIONARIO' else 'VISITANTE' end, " +
                    " UltimoAcesso = convert(varchar, accesstime, 103) + ' ' + convert(varchar, accesstime, 108), " +
                    " Bloqueado = case when lock.persid is null then 'Não Bloqueado' else lock.causeoflock end, " +
                    " LimiteBloqueio = case when lock.persid is null then 'Não Bloqueado' else convert(varchar, lockeduntil, 103) end, ";

                string sqlfields = "";
                foreach (PropertyInfo p in reportfields.GetType().GetProperties())
                {
                    var value = p.GetValue(reportfields);

                    if (value != null)
                    {
                        if (p.Name.Equals("DATEOFBIRTH"))
                            sqlfields += "convert(date," + p.Name + ", 103) as " + value.ToString() + ",";
                        else if (p.Name.Equals("STREETHOUSENO") || p.Name.Equals("CITY"))
                            sqlfields += "per." + p.Name + " as " + value.ToString() + ",";
                        else if (p.Name.Equals("NAME"))
                            sqlfields += "cli." + p.Name + " as " + value.ToString() + ",";
                        else if (p.Name.Equals("CODEDATA"))
                            sqlfields += "convert(int, convert(varbinary(4)," + p.Name + ")) as " + value.ToString() + ",";
                        else if (p.Name.Equals("IDENTIFICATIONMODE"))
                        {
                            sqlfields += value.ToString() + " = case when identificationmode = 1 then 'NENHUM' when identificationmode =  2 then 'BIOMETRIA' when identificationmode =  3 then 'CARTAO' when identificationmode =  4 then 'BIOMETRIA E CARTAO' end,";
                            sqlfields += "BIOMETRIA = case when not bio.persid is null then 'SIM' else 'NAO' end,";
                            bBio = true;
                        }
                        else
                            sqlfields += p.Name + " as " + value.ToString() + ",";
                    }
                }
                sqlfields = sqlfields.Substring(0, sqlfields.Length - 1);

                if (customfields != null)
                {
                    foreach (BSRPTCustomFields custom in customfields)
                        sqlfields += "," + custom.LABEL + " =  max(case when label = '" + custom.LABEL + "' then value else '' end)";
                }

                sql += sqlfields + " from bsuser.persons per left outer join bsuser.companies cmp on per.companyid = cmp.companyid " +
                    " left outer join bsuser.cards cd on per.persid = cd.persid inner join bsuser.clients cli on cli.clientid = per.clientid " +
                    " left outer join bsuser.currentaccessstate cursta on cursta.persid = per.persid " +
                    " left outer join bsuser.lockouts lock on lock.persid = per.persid ";

                if (customfields != null)
                {
                    if (customfields != null && customfields.Count > 0)
                        sql += "left outer join bsuser.ADDITIONALFIELDS ad on per.persid = ad.persid left outer join bsuser.additionalfielddescriptors ac on ac.id = ad.fielddescid ";
                }

                if (bBio)
                    sql += "left outer join bsuser.biodata bio on bio.persid = per.persid ";

                sql += " where per.status = 1 and cd.status = 1 and persclass = 'E' ";

                if (!string.IsNullOrEmpty(persno))
                    sql += " and per.persid = '" + persno + "'";
                else
                {
                    if (!String.IsNullOrEmpty(clientid))
                        sql += " and per.clientid = '" + clientid + "'";

                    if (persclassid != null && persclassid.Length > 0)
                    {
                        string where = "";
                        cmpno = "";
                        foreach (string no in persclassid)
                            where += "'" + no + "', ";
                        cmpno += where.Substring(0, where.Length - 2);

                        sql += " and persclassid in (" + cmpno + ")";
                    }

                    if (company != null && company.Length > 0)
                    {
                        string where = "";
                        cmpno = "";
                        foreach (string no in company)
                            where += "'" + no + "', ";
                        cmpno += where.Substring(0, where.Length - 2);

                        sql += " and companyno in (" + cmpno + ")";
                    }
                }

                if (customfields != null && customfields.Count > 0)
                {
                    sql += " group by ";
                    foreach (PropertyInfo p in reportfields.GetType().GetProperties())
                    {
                        if (p.Name.Equals("STREETHOUSENO") || p.Name.Equals("CITY"))
                            sql += p.GetValue(reportfields) != null ? "per." + p.Name + "," : "";
                        else if (p.Name.Equals("NAME"))
                            sql += p.GetValue(reportfields) != null ? "cli." + p.Name + "," : "";
                        else
                            sql += p.GetValue(reportfields) != null ? p.Name + "," : "";
                    }
                    sql += "FIRSTNAME, LASTNAME, PERSCLASS, ACCESSTIME, lock.PERSID, CAUSEOFLOCK, LOCKEDUNTIL";
                    if (bBio)
                        sql += ",bio.persid";
                }

                sql += " order by Nome";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna as pessoas sem crachá.
        /// Caso a pesquisa seja por cliente, a variável clientid deve ser preenchida.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static DataTable LoadNoBadge(DatabaseContext dbcontext, string clientid)
        {
            try
            {
                string sql = "select description, p.persid, persno, idnumber, Nome = ISNULL(firstname, '') + ' ' + ISNULL(lastname, '') from bsuser.cards c " +
                    "right outer join bsuser.persons p on c.persid = p.persid " +
                    "inner join bsuser.clients cli on cli.clientid = p.clientid where persclass = 'E' and c.persid is null and p.status = 1";
                if (!String.IsNullOrEmpty(clientid))
                    sql += " and p.clientid = '" + clientid + "'";
                sql += " order by description";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna as pessoas sem uso de crachás por um período de tempo.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="qtd">N. de dias sem uso de crachás. </param>
        /// <returns></returns>
        public static DataTable LoadBadgeNoUse(DatabaseContext dbcontext, string qtd)
        {
            try
            {
                string sql = String.Format("set dateformat 'dmy' exec spREL_RetornaDiasSemUso {0}", qtd);

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna as pessoas sem fotografia.
        /// Caso a pesquisa seja por cliente, a variável clientid deve ser preenchida.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID do cliente.</param>
        /// <returns></returns>
        public static DataTable LoadPhotos(DatabaseContext dbcontext, string clientid)
        {
            try
            {
                string sql = "select p.persid, description, persno, idnumber, Nome = ISNULL(firstname, '') + ' ' + ISNULL(lastname, '') from bsuser.persons p " +
                    "inner join bsuser.clients c on c.clientid = p.clientid where persclass = 'E'";
                if (!String.IsNullOrEmpty(clientid))
                    sql += " and p.clientid = '" + clientid + "'";
                sql += " order by description";

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gera o arquivo com as pessoas sem foto.
        /// </summary>
        /// <param name="table">Tabela com as pessoas sem foto.</param>
        /// <param name="bispath">Caminho do BIS.</param>
        /// <returns>Retorna um array de bytes com o arquivo.</returns>
        public static byte[] SaveFile(DataTable table, string bispath)
        {
            MemoryStream fs = new MemoryStream();
            TextWriter w = new StreamWriter(fs);
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    if (!System.IO.File.Exists(bispath + @"AccessEngine\Cardholderimages\" + row["persid"].ToString().Trim() + ".jpg"))
                    {
                        w.WriteLine(row["description"].ToString() + ";" + row["persno"].ToString() + ";" + row["idnumber"].ToString() + ";" + row["nome"].ToString());
                    }
                }
                w.Flush();
                fs.Flush();

                return fs.ToArray();
            }
            catch (Exception ex)
            {
                if (w != null)
                {
                    w.Close();
                    fs.Close();
                    w = null;
                    fs = null;
                }
                throw new Exception(ex.Message);
            }
            finally
            {
                w.Close();
                fs.Close();
                w = null;
                fs = null;
            }
        }
        #endregion
    }
}
