using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    /// <summary>
    /// Classe com a estrutura para a totalizaçao das refeições.
    /// </summary>
    public class TotalMeal
    {
        #region Variables
        /// <summary>
        /// Total de refeições.
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// Unidade das refeições.
        /// </summary>
        public string Divisao { get; set; }
        /// <summary>
        /// Tipo da Refeição: desejum, almoço, jantar, ceia.
        /// </summary>
        public string TipoRefeicao { get; set; }
        /// <summary>
        /// Tipo do empregado.
        /// </summary>
        public string TipoEmpregado { get; set; }
        /// <summary>
        /// Data da refeição.
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// Data da refeição.
        /// </summary>
        public int Dia { get; set; }
        /// <summary>
        /// Total por tipo: Empregado.
        /// </summary>
        public int Empregado { get; set; }
        /// <summary>
        /// Total por tipo: Master Terceirizado.
        /// </summary>
        public int MasterT { get; set; }
        /// <summary>
        /// Total por tipo: Master Provisório.
        /// </summary>
        public int MasterP { get; set; }
        /// <summary>
        /// Total por tipo: Visitante.
        /// </summary>
        public int Visitante { get; set; }
        /// <summary>
        /// Total por tipo: todos os outros tipos de pessoa.
        /// </summary>
        public int Coca { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        public TotalMeal()
        {
        }
        #endregion
    }
}
