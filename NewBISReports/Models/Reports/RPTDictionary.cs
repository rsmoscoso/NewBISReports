using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Reports
{
    /// <summary>
    /// Relação entre coluna do banco de dados e alias para exibição.
    /// </summary>
    public class RPTDictionary
    {
        #region Variables
        /// <summary>
        /// Nome da coluna no banco de dados.
        /// </summary>
        public string Column { get; set; }
        /// <summary>
        /// Alias da coluna para exibição.
        /// </summary>
        public string Alias { get; set; }
        #endregion

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="column">Nome da coluna do banco de dados.</param>
        /// <param name="alias">Alias para a exibição.</param>
        public RPTDictionary(string column, string alias)
        {
            this.Column = column;
            this.Alias = alias;
        }
    }
}
