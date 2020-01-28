using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;

namespace NewBISReports
{
    /// <summary>
    /// Classe que gerencia as fontes do NPOI.
    /// </summary>
    public class HzNPOIFont
    {
        #region Enum
        /// <summary>
        /// Enumeração dos tipos de fonte.
        /// </summary>
        public enum HZNPOIFONT_NAME
        {
            ARIAL = 0,
            CALIBRI = 1,
            TIMESNEWROMAN = 2
        }
        #endregion

        #region Variables
        /// <summary>
        /// Conversão de pontos para pixels.
        /// </summary>
        public static short FONTRATIO = 1;
        /// <summary>
        /// Workbook do NPOI.
        /// </summary>
        public HzNPOIWorkbook WorkBook { get; set; }
        /// <summary>
        /// Fonte da planilha.
        /// </summary>
        private NPOI.SS.UserModel.IFont font { get; set; }
        /// <summary>
        /// Nome da fonte.
        /// </summary>
        public HZNPOIFONT_NAME Name { get; set; }
        /// <summary>
        /// Tamanho da fonte em pontos.
        /// </summary>
        public short Size { get; set; }
        /// <summary>
        /// Determina se a fonte é negrito ou não.
        /// </summary>
        private bool Bold { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retorna a fonte corrente da planilha.
        /// </summary>
        /// <returns></returns>
        public NPOI.SS.UserModel.IFont GetFont()
        {
            return this.font;
        }

        /// <summary>
        /// Retorna o tamanho da fonte de acordo com seu tamanhao.
        /// </summary>
        /// <returns></returns>
        public int GetFontSize()
        {
            return (int)this.font.FontHeight;
        }

        /// <summary>
        /// Habilita/Desabilita negrito da fonte.
        /// </summary>
        /// <param name="bold"></param>
        public void SetBold(bool bold)
        {
            if (this.font != null)
                this.font.Boldweight = bold ? (short)NPOI.SS.UserModel.FontBoldWeight.Bold : (short)NPOI.SS.UserModel.FontBoldWeight.Normal;
        }
        /// <summary>
        /// Retorna o nome da fonte em carcater.
        /// </summary>
        /// <param name="fontname">Nome da fonte.</param>
        /// <returns></returns>
        public string GetFontName(HZNPOIFONT_NAME fontname)
        {
            if (fontname == HZNPOIFONT_NAME.ARIAL)
                return "ARIAL";
            else if (fontname == HZNPOIFONT_NAME.TIMESNEWROMAN)
                return "TIMES NEW ROMAN";
            else
                return "CALIBRI";
        }
        #endregion

        #region Events
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="workbook">Workbook da planilha.</param>
        /// <param name="fontname">Nome da fonte.</param>
        /// <param name="size">Tamanho da fonte em pontos.</param>
        public HzNPOIFont(HzNPOIWorkbook workbook, HZNPOIFONT_NAME fontname, short size)             
        {
            this.WorkBook = workbook;
            this.Name = fontname;
            this.Size = size;
            this.font = this.WorkBook.WorkBoook.CreateFont();
            //this.font.FontName = this.WorkBook.WorkBoook.GetFontName(fontname);
            this.font.FontHeight = (short)(HzNPOIFont.FONTRATIO * this.Size);
        }
        #endregion
    }
}
