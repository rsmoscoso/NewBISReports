using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;


namespace NewBISReports
{
    /// <summary>
    /// Classe que gerencia as células da planilha.
    /// </summary>
    public class HzNPOICell
    {
        #region Variables
        /// <summary>
        /// Planilha atual.
        /// </summary>
        private ISheet currentsheet { get; set; }
        /// <summary>
        /// Célula da planilha.
        /// </summary>
        private ICell cell { get; set; }
        /// <summary>
        /// Fonte da célula.
        /// </summary>
        public HzNPOIFont Font { get; set; }
        /// <summary>
        /// Estilo da célula de acordo com a fonte.
        /// </summary>
        private ICellStyle Style { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retorna a célula corrente.
        /// </summary>
        /// <returns></returns>
        public ICell GetCell()
        {
            return this.cell;
        }

        /// <summary>
        /// Adiciona uma célula na linha corrente.
        /// </summary>
        /// <param name="row">Linha da planilha.</param>
        /// <param name="ncell">Índice da célula.</param>
        public void AddCell(IRow row, int ncell)
        {
            this.cell = row.CreateCell(ncell);
        }
        #endregion

        #region Events
        /// <summary>
        /// Construtora da classe.
        /// </summary>
        /// <param name="sheet">Planilha atual.</param>
        /// <param name="font">Fonte da célula.</param>
        /// <param name="style">Estilo da célula.</param>
        public HzNPOICell(ISheet sheet, HzNPOIFont font, ICellStyle style)
        {
            this.currentsheet = sheet;
            this.Font = font;
        }
        #endregion
    }
}
