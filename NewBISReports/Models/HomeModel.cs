using HzBISCommands;
using Microsoft.AspNetCore.Http;
using NewBISReports.Models.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models
{
    #region Enumeration
    public enum ACCESSTYPE
    {
        GRANTED = 4101,
        UNKNOWN = 4109,
        NOTGRANTED = 4112,
        CARDBLOCKED = 4111,
        CARDNOTYETVALID = 4119,
        CARDEXPIRED = 4120

    }

    /// <summary>
    /// Enumeração com os tipos de relatórios.
    /// </summary>
    public enum REPORTTYPE
    {
        //Diogo - adicionando uma "landing page"
        RPT_LANDINGPAGE = 0,
        //Diogo - Precisei trocar o valor desta enum, pois a número "0" é exclusiva do Id default
        RPT_ANALYTICGRANTEDBIS = 99,
        RPT_ANALYTICMEAL = 1,
        RPT_EXPORTMEAL = 2,
        RPT_PHOTOS = 3,
        RPT_BADGES = 4,
        RPT_TOTALMEAL = 5,
        RPT_ANALYTICSGENERAL = 6,
        RPT_ANALYTICSMEALBIS = 7,
        RPT_TOTALBADGES = 8,
        RPT_JOBS = 9,
        RPT_PERSONSAUTHORIZATIONS = 10,
        RPT_BADGENOUSE = 11,
        RPT_DASHBOARDMEAL = 12,
        RPT_EXCEPTION = 13,
        RPT_TOTALMEALGRAPH = 14,
        RPT_ALLLOCKOUT = 15,
        RPT_ALLVISITORS = 16,
        RPT_PERSONGENERAL = 17, // Relatório específido do MJ para a pesquisa por edifício.
        RPT_LOGQRCODE = 18, // Relatório específico FORTKNOX
        RPT_READERAUTHORIZATION = 19,
        RPT_COUNTBATH = 20,
        RPT_PERSONSPROFILES = 21,
        RPT_PERSONSAREA = 22,
        RPT_PERSONSINSIDEAREA = 23,
        RPT_CREDITS = 24,
        RPT_INTEGRACAOWFMBIS = 25,
        RPT_CHANGECLIENTID = 26,
        RPT_ANALYTICGRANTEDAMS = 27

    }

    /// <summary>
    /// Enumeração para o tipo de pesquisa das pessoas.
    /// </summary>
    public enum SEARCHPERSONS
    {
        SEARCHPERSONS_CARD = 0,
        SEARCHPERSONS_DOCUMENT = 1,
        SEARCHPERSONS_NAME = 2,
        SEARCHPERSONS_NAMEVISITOR = 3,
        SEARCHPERSONS_PERSNO = 4,
        SEARCHPERSONS_PERSID = 5,
        SEARCHPERSONS_PASSPORTNO = 6
    }

    /// <summary>
    /// Tipo da Refeição
    /// </summary>

    public enum MEALTYPE
    {
        MEALTYPE_TODOS = 0,
        MEALTYPE_DESEJUM = 1,
        MEALTYPE_ALMOCO = 2,
        MEALTYPE_JANTAR = 3,
        MEALTYPE_CEIA = 4
    }
    #endregion

    public class HomeModel
    {
        /// <summary>
        /// Data Inicial do relatório.
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Data final do relatório.
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Matrícula da pessoa.
        /// </summary>
        public string PERSNO { get; set; }

        /// <summary>
        /// Divisão do BIS.
        /// </summary>
        public string CLIENTID { get; set; }

        /// <summary>
        /// ID do dispositivo.
        /// </summary>
        public string[] DEVICEID { get; set; }

        /// <summary>
        /// ID das autorizações.
        /// </summary>
        public string[] AUTHID { get; set; }

        /// <summary>
        /// Tipo da Pessoa.
        /// </summary>
        public string[] PERSCLASSID { get; set; }

        /// <summary>
        /// Sigla da empresa.
        /// </summary>
        public string[] CompanyNO { get; set; }

        /// <summary>
        /// Tipo da pesquisa das pessoas.
        /// </summary>
        public SEARCHPERSONS SearchPersonsType { get; set; }

        /// <summary>
        /// Tipo da Refeição
        /// </summary>
        public MEALTYPE MealType { get; set; }
        /// <summary>
        /// String de pesquisa da pessoa.
        /// </summary>
        public string NAMESEARCH { get; set; }

        /// <summary>
        /// String de pesquisa da empresa.
        /// </summary>
        public string COMPANYNOSEARCH { get; set; }

        /// <summary>
        /// String de pesquisa da leitora.
        /// </summary>
        public string DEVICESEARCH { get; set; }
        /// <summary>
        /// Listagem das pessoas para pesquisa.
        /// </summary>
        public string LISTPERSONS { get; set; }

        public string EXCELBUTTON { get; set; }

        /// <summary>
        /// Coleção com as informações do acesso.
        /// </summary>
        public List<LogEvent> Acessos { get; set; }
        /// <summary>
        /// Coleção com a totalização das refeições.
        /// </summary>
        public List<TotalMeal> Meals { get; set; }

        public List<IntegracaoWFMBIS> WFM { get; set; }
        /// <summary>
        /// Tipo do relatório.
        /// </summary>
        public REPORTTYPE Type { get; set; }
        /// <summary>
        /// N. de dias para o relatório de tempo de uso do crachá.
        /// </summary>
        public string NDays { get; set; }
        /// <summary>
        /// Tipo do relatório de acesso.
        /// </summary>
        public ACCESSTYPE AccessType { get; set; }
        /// <summary>
        /// ID da área.
        /// </summary>
        public string[] AREAID { get; set; }
        /// <summary>
        /// Flag para o relatórios de todos dentro da área.
        /// </summary>
        public bool ALLINSIDE { get; set; }
        public List<BSCompaniesInfo> Companies { get; set; }
        public List<BSClientsInfo> Clients { get; set; }
        public string USERRE { get; set; } 
        public string USERPASSWORD { get; set; }
        public string PERSID { get; set; }

        #region Functions
        /// <summary>
        /// Retorna o tipo do relatório.
        /// </summary>
        /// <param name="type">Descrição do tipo do relatório.</param>
        /// <returns></returns>
        public static REPORTTYPE GetReportType(string type)
        {
            
            REPORTTYPE retval = REPORTTYPE.RPT_ANALYTICGRANTEDBIS;

            switch (type)
            {
                case "1":
                    retval = REPORTTYPE.RPT_ANALYTICMEAL;
                    break;
                case "6":
                    retval = REPORTTYPE.RPT_ANALYTICSGENERAL;
                    break;
                case "0":
                    retval = REPORTTYPE.RPT_ANALYTICGRANTEDBIS;
                    break;
                case "4":
                    retval = REPORTTYPE.RPT_BADGES;
                    break;
                case "2":
                    retval = REPORTTYPE.RPT_EXPORTMEAL;
                    break;
                case "3":
                    retval = REPORTTYPE.RPT_PHOTOS;
                    break;
                case "5":
                    retval = REPORTTYPE.RPT_TOTALMEAL;
                    break;
                case "7":
                    retval = REPORTTYPE.RPT_ANALYTICSMEALBIS;
                    break;
                case "8":
                    retval = REPORTTYPE.RPT_TOTALBADGES;
                    break;
                case "9":
                    retval = REPORTTYPE.RPT_JOBS;
                    break;
                case "10":
                    retval = REPORTTYPE.RPT_PERSONSAUTHORIZATIONS;
                    break;
                case "11":
                    retval = REPORTTYPE.RPT_BADGENOUSE;
                    break;
                case "12":
                    retval = REPORTTYPE.RPT_DASHBOARDMEAL;
                    break;
                case "13":
                    retval = REPORTTYPE.RPT_EXCEPTION;
                    break;
                case "14":
                    retval = REPORTTYPE.RPT_TOTALMEALGRAPH;
                    break;
                case "15":
                    retval = REPORTTYPE.RPT_ALLLOCKOUT;
                    break;
                case "16":
                    retval = REPORTTYPE.RPT_ALLVISITORS;
                    break;
                case "17":
                    retval = REPORTTYPE.RPT_PERSONGENERAL;
                    break;
                case "18":
                    retval = REPORTTYPE.RPT_LOGQRCODE;
                    break;
                case "19":
                    retval = REPORTTYPE.RPT_READERAUTHORIZATION;
                    break;
                case "20":
                    retval = REPORTTYPE.RPT_COUNTBATH;
                    break;
                case "21":
                    retval = REPORTTYPE.RPT_PERSONSPROFILES;
                    break;
                case "22":
                    retval = REPORTTYPE.RPT_PERSONSAREA;
                    break;
                case "23":
                    retval = REPORTTYPE.RPT_PERSONSINSIDEAREA;
                    break;
                case "24":
                    retval = REPORTTYPE.RPT_CREDITS;
                    break;
                case "25":
                    retval = REPORTTYPE.RPT_INTEGRACAOWFMBIS;
                    break;
                case "27":
                    retval = REPORTTYPE.RPT_ANALYTICGRANTEDAMS;
                    break;
                //Diogo - adicionando uma landing page
                case "99":
                    retval = REPORTTYPE.RPT_LANDINGPAGE;
                    break;
            }

            return retval;
        }

        /// <summary>
        /// Retorna a descrição do tipos de relatório.
        /// </summary>
        /// <param name="type">Tipo do relatório.</param>
        /// <returns></returns>
        public static string GetTypeDescription(REPORTTYPE type)
        {
            string retval = "Analítico de Acessos";

            switch (type)
            {
                case REPORTTYPE.RPT_ANALYTICMEAL:
                    retval = "Analítico Refeição";
                    break;
                case REPORTTYPE.RPT_ANALYTICSGENERAL:
                    retval = "Analítico Geral - Solar";
                    break;
                case REPORTTYPE.RPT_ANALYTICGRANTEDBIS:
                    retval = "Analítico de Acessos em Tempo Real";
                    break;
                case REPORTTYPE.RPT_BADGES:
                    retval = "Pessoas que não Possuem Crachás";
                    break;
                case REPORTTYPE.RPT_EXPORTMEAL:
                    retval = "Exporta as Refeições Servidas";
                    break;
                case REPORTTYPE.RPT_PHOTOS:
                    retval = "Pessoas que Não Possuem Fotografia";
                    break;
                case REPORTTYPE.RPT_TOTALMEAL:
                    retval = "Total de Refeições Servidas";
                    break;
                case REPORTTYPE.RPT_ANALYTICSMEALBIS:
                    retval = "Anatítico das Refeições Servidas";
                    break;
                case REPORTTYPE.RPT_TOTALBADGES:
                    retval = "Pessoas com Excesso de Crachás";
                    break;
                case REPORTTYPE.RPT_JOBS:
                    retval = "Cargos Cadastrados";
                    break;
                case REPORTTYPE.RPT_PERSONSAUTHORIZATIONS:
                    retval = "Autorizações das Pessoas";
                    break;
                case REPORTTYPE.RPT_BADGENOUSE:
                    retval = "Pessoas Sem Uso de Crachá por Período Determinado";
                    break;
                case REPORTTYPE.RPT_DASHBOARDMEAL:
                    retval = "Dashboard das Refeições Servidas";
                    break;
                case REPORTTYPE.RPT_EXCEPTION:
                    retval = "Exceções para o Acesso das Pessoas";
                    break;
                case REPORTTYPE.RPT_TOTALMEALGRAPH:
                    retval = "Dashboard do Total de Refeições";
                    break;
                case REPORTTYPE.RPT_ALLLOCKOUT:
                    retval = "Pessoas Bloqueadas";
                    break;
                case REPORTTYPE.RPT_ALLVISITORS:
                    retval = "Visitantes";
                    break;
                case REPORTTYPE.RPT_PERSONGENERAL:
                    retval = "Pessoas em Geral";
                    break;
                case REPORTTYPE.RPT_LOGQRCODE:
                    retval = "Visitantes QRCode";
                    break;
                case REPORTTYPE.RPT_READERAUTHORIZATION:
                    retval = "Leitores por Autorizações";
                    break;
                case REPORTTYPE.RPT_COUNTBATH:
                    retval = "Contagem de Pessoas";
                    break;
                case REPORTTYPE.RPT_PERSONSPROFILES:
                    retval = "Perfis de Pessoas";
                    break;
                case REPORTTYPE.RPT_PERSONSAREA:
                    retval = "Pessoas por Área";
                    break;
                case REPORTTYPE.RPT_PERSONSINSIDEAREA:
                    retval = "Pessoas dentro da Unidade";
                    break;
                case REPORTTYPE.RPT_CREDITS:
                    retval = "Créditos Disponíveis";
                    break;
                case REPORTTYPE.RPT_INTEGRACAOWFMBIS:
                    retval = "Integração WFM x BIS";
                    break;
                case REPORTTYPE.RPT_ANALYTICGRANTEDAMS:
                    retval = "Relatório de Eventos do AMS";
                    break;

            }
            return retval;
        }

        /// <summary>
        /// Retorna a descrição do tipos de relatório.
        /// </summary>
        /// <param name="type">Tipo do relatório.</param>
        /// <returns></returns>
        public static string GetTypeComments(REPORTTYPE type)
        {
            string retval = "acessos permitidos";

            switch (type)
            {
                case REPORTTYPE.RPT_ANALYTICMEAL:
                    retval = "Analítico Refeição";
                    break;
                case REPORTTYPE.RPT_ANALYTICSGENERAL:
                    retval = "acessos permitidos geral - Solar";
                    break;
                case REPORTTYPE.RPT_ANALYTICGRANTEDBIS:
                    retval = "acessos permitidos - BIS";
                    break;
                case REPORTTYPE.RPT_BADGES:
                    retval = "pessoas que não possuem crachás";
                    break;
                case REPORTTYPE.RPT_EXPORTMEAL:
                    retval = "exporta as refeições servidas - BIS";
                    break;
                case REPORTTYPE.RPT_PHOTOS:
                    retval = "pessoas que não possuem fotografia cadastrada";
                    break;
                case REPORTTYPE.RPT_TOTALMEAL:
                    retval = "total de refeições servidas por tipo de refeição - BIS";
                    break;
                case REPORTTYPE.RPT_ANALYTICSMEALBIS:
                    retval = "relatórios com as refeições servidas - BIS";
                    break;
                case REPORTTYPE.RPT_TOTALBADGES:
                    retval = "Pessoas com Excesso de Crachás";
                    break;
                case REPORTTYPE.RPT_JOBS:
                    retval = "Cargos Cadastrados";
                    break;
                case REPORTTYPE.RPT_PERSONSAUTHORIZATIONS:
                    retval = "autorizações das pessoas";
                    break;
                case REPORTTYPE.RPT_BADGENOUSE:
                    retval = "pessoas sem uso do crachá por um período";
                    break;
                case REPORTTYPE.RPT_DASHBOARDMEAL:
                    retval = "dashboard com as refeições servidas - BIS";
                    break;
                case REPORTTYPE.RPT_EXCEPTION:
                    retval = "cadastro das exceções de acesso das pessoas";
                    break;
                case REPORTTYPE.RPT_TOTALMEALGRAPH:
                    retval = "quantidade total de refeições";
                    break;
                case REPORTTYPE.RPT_ALLLOCKOUT:
                    retval = "relatório das pessoas bloqueadas";
                    break;
                case REPORTTYPE.RPT_ALLVISITORS:
                    retval = "relatório de todos os visitantes ativos";
                    break;
                case REPORTTYPE.RPT_PERSONGENERAL:
                    retval = "relatório das pessoas";
                    break;
                case REPORTTYPE.RPT_LOGQRCODE:
                    retval = "relatório dos visitantes por QR Code";
                    break;
                case REPORTTYPE.RPT_READERAUTHORIZATION:
                    retval = "relatório dos leitores por autorização";
                    break;
                case REPORTTYPE.RPT_COUNTBATH:
                    retval = "relatório da contagem das pessoas";
                    break;
                case REPORTTYPE.RPT_PERSONSPROFILES:
                    retval = "relatório dos perfis das pessoas das pessoas";
                    break;
                case REPORTTYPE.RPT_PERSONSAREA:
                    retval = "relatório das pessoas por área";
                    break;
                case REPORTTYPE.RPT_PERSONSINSIDEAREA:
                    retval = "relatório das pessoas dentro da unidade";
                    break;
                case REPORTTYPE.RPT_CREDITS:
                    retval = "Relatório de créditos restantes das pessoas";
                    break;
                case REPORTTYPE.RPT_INTEGRACAOWFMBIS:
                    retval = "Relatório da integração WFM x BIS";
                    break;
                case REPORTTYPE.RPT_ANALYTICGRANTEDAMS:
                    retval = "Relatório de Eventos do AMS";
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Gera o aqruivo de exportação das refeições.
        /// </summary>
        public bool ExportMeal(DataTable table, string filename)
        {
            StreamWriter writer = null;
            try
            {
                if (table != null && table.Rows.Count > 0)
                {
                    writer = new StreamWriter(filename);
                    foreach (DataRow r in table.Rows)
                    {
                        DateTime date = DateTime.Parse(r["Data"].ToString());

                        writer.WriteLine(r["matricula"].ToString().Trim() + date.ToShortTimeString().ToString().Replace(":", "").Substring(0, 4) +
                            date.ToShortDateString().ToString().Replace("/", "") + "100012");
                    }
                    writer.Close();
                    writer = null;
                }

                return true;
            }
            catch
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }

                return false;
            }
        }
        #endregion
    }
}
