using Microsoft.AspNetCore.Http;
using NewBISReports.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Excecao
{
    public class ExcecaoModel
    {
        #region Variables
        /// <summary>
        /// Matrícula da pessoa.
        /// </summary>
        public string Persno { get; set; }
        /// <summary>
        /// String de pesquisa da pessoa.
        /// </summary>
        public string NAMESEARCH { get; set; }
        /// <summary>
        /// ID de pessoa para bloqueio.
        /// </summary>
        public string PERSIDBLOCK { get; set; }
        /// <summary>
        /// PERSID da pessoa, caso exista.
        /// </summary>
        public string PERSID { get; set; }
        /// <summary>
        /// Datta do início do bloqueio.
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// Data do término do bloqueio.
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// Tipo da pesquisa das pessoas.
        /// </summary>
        public SEARCHPERSONS SearchPersonsType { get; set; }
        /// <summary>
        /// Nome do arquivo com a formatação para importação.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Coleção das fotos selecionadas.
        /// </summary>
        public IFormFile[] CSVFile { get; set; }
        public List<Persons> personsExce { get; set; }
        #endregion
    }
}
