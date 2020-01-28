using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewBISReports.Models.ImportVisitor
{
    public interface IFormFile
    {
        string ContentType { get; }
        string ContentDisposition { get; }
        IHeaderDictionary Headers { get; }
        long Length { get; }
        string Name { get; }
        string FileName { get; }
        Stream OpenReadStream();
        void CopyTo(Stream target);
        Task CopyToAsync(Stream target, CancellationToken cancellationToken);
    }

    #region Enumeration
    /// <summary>
    /// Enumeração com os tipos de relatórios.
    /// </summary>
    public enum DATATYPE
    {
        DT_IMPORTVISITOR = 0,
    }
    #endregion

    public class ImportVisitorModel
    {
        #region Variables
        /// <summary>
        /// Tipo do relatório.
        /// </summary>
        public DATATYPE Type { get; set; }

        /// <summary>
        /// ID do cliente.
        /// </summary>
        public string CLIENTID { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retorna o tipo do relatório.
        /// </summary>
        /// <param name="type">Descrição do tipo do relatório.</param>
        /// <returns></returns>
        public static DATATYPE GetDataType(string type)
        {
            DATATYPE retval = DATATYPE.DT_IMPORTVISITOR;

            switch (type)
            {
                case "0":
                    retval = DATATYPE.DT_IMPORTVISITOR;
                    break;
            }

            return retval;
        }

        /// <summary>
        /// Retorna a descrição do tipos de relatório.
        /// </summary>
        /// <param name="type">Tipo do relatório.</param>
        /// <returns></returns>
        public static string GetTypeDescription(DATATYPE type)
        {
            string retval = "Dados dos Visitantes";

            switch (type)
            {
                case DATATYPE.DT_IMPORTVISITOR:
                    retval = "Dados dos Visitantes";
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Retorna a descrição do tipos de relatório.
        /// </summary>
        /// <param name="type">Tipo do relatório.</param>
        /// <returns></returns>
        public static string GetTypeComments(DATATYPE type)
        {
            string retval = "importação dos dados dos visitantes";

            switch (type)
            {
                case DATATYPE.DT_IMPORTVISITOR:
                    retval = "importação dos dados dos visitantes";
                    break;
            }
            return retval;
        }
        #endregion
    }
}
