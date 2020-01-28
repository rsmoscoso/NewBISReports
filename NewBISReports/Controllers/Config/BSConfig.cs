using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;

namespace NewBISReports.Controllers.Config
{
    #region Enumeration
    /// <summary>
    /// Tipo do sistema de relatórios:
    /// Simple = apenas os relatórios do BIS em tempo real
    /// Full - relatórios do BIS e da tabela de Acessos.
    /// </summary>
    public enum SYSTEM_TYPE
    {
        SIMPLE = 0,
        FULL = 1
    }

    #endregion

    /// <summary>
    /// Classe com as configurações do aplicativo.
    /// </summary>
    public class BSConfig
    {
        #region Variables
        /// <summary>
        /// Nome da emprea de configuração padrão.
        /// </summary>
        public string DefaultName { get; set; }
        /// <summary>
        /// Cor de fundo.
        /// </summary>
        public string BackColor { get; set; }
        /// <summary>
        /// Cor da letra.
        /// </summary>
        public string ForeColor { get; set; }
        /// <summary>
        /// Intensidade do negrito.
        /// </summary>
        public string FontWeight { get; set; }
        /// <summary>
        /// Caminho da logomarca.
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// Se há controle de refeições.
        /// </summary>
        public bool Meal { get; set; }
        /// <summary>
        /// Diretório do BIS.
        /// </summary>
        public string BisPath { get; set; }
        /// <summary>
        /// Tipo do sistema de relatórios.
        /// </summary>
        public SYSTEM_TYPE SystemType { get; set; }
        /// <summary>
        /// Prefixo da AddressTag. Se for nulo, o padrão é "ControleAcesso.".
        /// </summary>
        public string AddressTagPrefix { get; set; }
        /// <summary>
        /// Sufixo da AddressTag. Se for nulo, o padrão é ".Evento".
        /// </summary>
        public string AddressTagSufix { get; set; }
        /// <summary>
        /// Parâmetro utilizado para a pesquisa no relatório pela unidade. O 
        /// padrão é: TagBISServer + '.' + descrição da unidade.
        /// </summary>
        public string TagBISServer { get; set; }
        /// <summary>
        /// IP do servidor RESTApi.
        /// </summary>
        public string RestServer { get; set; }
        /// <summary>
        /// Porta de comunição do servidor RESTApi.
        /// </summary>
        public string RestPort { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="defaultname">Nome da configuração padrão.</param>
        /// <param name="backcolor">Cor de fundo.</param>
        /// <param name="forecolor">Cor da letra.</param>
        /// <param name="fontweight">Intensidade do negrito.</param>
        /// <param name="imagepath">Caminho da logomarca.</param>
        /// <param name="meal">Se há controle de refeições.</param>
        /// <param name="bispath">Diretório do BIS.</param>
        /// <param name="rpttype">Tipo do sistema.</param>
        /// <param name="systemtype">Tipo do sistema de relatórios.</param>
        /// <param name="addresstagprefix">Prefixo da AddressTag. Se for nulo, o padrão é "ControleAcesso."</param>
        /// <param name="addresstagsufix">Sufixo da AddressTag. Se for nulo, o padrão é ".Evento".</param>
        /// <param name="tagbisserver">Parâmetro utilizado para a pesquisa no relatório pela unidade. O 
        /// padrão é: TagBISServer + '.' + descrição da unidade.</param>
        /// <param name="restserver">IP do servidor RESTApi.</param>
        /// <param name="restport">Porta do servidor RESTApi.</param>
        public BSConfig(string defaultname, string backcolor, string forecolor, string fontweight, string imagepath, string meal, string bispath, string systemtype, string addresstagprefix,
            string addresstagsufix, string tagbisserver, string restserver, string restport)
        {
            this.DefaultName = defaultname;
            this.BackColor = !String.IsNullOrEmpty(backcolor) ? backcolor : "black";
            this.ForeColor = !String.IsNullOrEmpty(forecolor) ? forecolor : "white";
            this.FontWeight = !String.IsNullOrEmpty(fontweight) ? fontweight : "bold";
            this.ImagePath = !String.IsNullOrEmpty(imagepath) ? imagepath : "";
            this.Meal = !String.IsNullOrEmpty(meal) ? bool.Parse(meal) : false;
            this.BisPath = !String.IsNullOrEmpty(bispath) ? bispath : @"c:\mgts";
            this.SystemType = !String.IsNullOrEmpty(systemtype) ? (systemtype == "1" ? SYSTEM_TYPE.FULL : SYSTEM_TYPE.SIMPLE) : SYSTEM_TYPE.FULL;
            this.AddressTagPrefix = !String.IsNullOrEmpty(addresstagprefix) ? addresstagprefix : "ControleAcesso.Devices.";
            this.AddressTagSufix = !String.IsNullOrEmpty(addresstagsufix) ? addresstagsufix : ".Evento";
            this.TagBISServer = !String.IsNullOrEmpty(tagbisserver) ? tagbisserver : "BIS";
            this.RestServer = !String.IsNullOrEmpty(restserver) ? restserver : "localhost";
            this.RestPort = !String.IsNullOrEmpty(restport) ? restport : "9090";
        }
        #endregion
    }
}
