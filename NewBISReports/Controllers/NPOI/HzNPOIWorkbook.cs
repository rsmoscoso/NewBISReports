using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Data.Common;
using System.Data;

namespace NewBISReports
{
    /// <summary>
    /// Classe que gerencia o workbook da classe NPOI.
    /// </summary>
    public class HzNPOIWorkbook
    {
        #region variables
        public IWorkbook WorkBoook { get; set; }
        /// <summary>
        /// Nome do arquivo excel.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Planilha do excel.
        /// </summary>
        private ISheet Sheet { get; set; }
        /// <summary>
        /// Nome da planilha.
        /// </summary>        
        private string SheetName { get; set; }
        /// <summary>
        /// Fonte do cabeçalho.
        /// </summary>
        public HzNPOIFont FontTitle { get; set; }
        /// <summary>
        /// Fonte das células.
        /// </summary>
        public HzNPOIFont FontBody { get; set; }
        /// <summary>
        /// Estilo do cabeçalho.
        /// </summary>
        public ICellStyle StyleTitle { get; set; }
        /// <summary>
        /// Estilo da fonte do texto.
        /// </summary>
        public ICellStyle StyleBody { get; set; }
        /// <summary>
        /// Linha corrente do excel.
        /// </summary>
        private IRow Row { get; set; }
        /// <summary>
        /// Número da célula corrente.
        /// </summary>
        private int nCell { get; set; }
        /// <summary>
        /// Número da linha do excel.
        /// </summary>
        private int nRow { get; set; }
        /// <summary>
        /// Quantidade de planilhas no arquivo excel.
        /// </summary>
        private int nSheet { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Cria a planilha Excel para o relatório.
        /// </summary>
        /// <param name="sheetname">Nome da planliha.</param>
        /// <returns></returns>
        public bool CreateExcel(string sheetname)
        {
            try
            {
                this.Sheet = this.WorkBoook.CreateSheet(this.SheetName = sheetname);
                this.nRow = 0;
                this.nSheet = 0;

                this.FontTitle = new HzNPOIFont(this, HzNPOIFont.HZNPOIFONT_NAME.CALIBRI, 10);
                this.FontTitle.SetBold(true);
                this.StyleTitle = this.WorkBoook.CreateCellStyle();
                this.StyleTitle.SetFont(this.FontTitle.GetFont());

                this.FontBody = new HzNPOIFont(this, HzNPOIFont.HZNPOIFONT_NAME.CALIBRI, 10);
                this.StyleBody = this.WorkBoook.CreateCellStyle();
                this.StyleBody.SetFont(this.FontBody.GetFont());
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Escreve uma célula do cabeçalho.
        /// </summary>
        /// <param name="name">Descrição do cabeçalho.</param>
        /// <returns></returns>
        public bool WriteHeader(string name)
        {
            try
            {
                if (this.Row == null)
                {
                    this.Row = this.Sheet.CreateRow(this.nRow++);
                    this.nCell = 0;
                }
                ICell cell = this.Row.CreateCell(this.nCell++);
                cell.CellStyle = this.StyleTitle;
                cell.SetCellValue(name);
                this.Sheet.SetColumnWidth(this.nCell - 1, this.FontTitle.GetFontSize() * name.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Escreve o conteúdo da célula.
        /// </summary>
        /// <param name="content">Conteúdo da célula.</param>
        /// <returns></returns>
        public bool WriteBodyCell(bool newrow, string content)
        {
            try
            {
                if (newrow)
                {
                    this.Row = this.Sheet.CreateRow(this.nRow++);
                    this.nCell = 0;
                }
                ICell cell = this.Row.CreateCell(this.nCell++);
                cell.CellStyle = this.StyleBody;
                cell.SetCellValue(content);
                this.Sheet.SetColumnWidth(this.nCell - 1, this.FontBody.GetFontSize() * content.Length);

                if (this.nRow > 65000)
                {
                    this.nRow = 0;
                    this.Sheet = this.WorkBoook.CreateSheet(this.SheetName + (this.nSheet++).ToString());
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// Salva o arquivo em disco.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Save(string filename)
        {
            try
            {
                this.FileName = filename;
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);

                FileStream fs = new FileStream(filename, FileMode.CreateNew);
                this.WorkBoook.Write(fs);
                fs.Close();
                this.WorkBoook.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Adiciona uma planilha ao documento.
        /// </summary>
        /// <param name="name">Nome da planilha.</param>
        /// <returns></returns>
        public ISheet AddSheet(string name)
        {
            try
            {
                return this.WorkBoook.CreateSheet(name);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Export Functions
        /// <summary>
        /// Exporta os dados para excel baseados em um DBDATAREADER.
        /// </summary>
        /// <param name="reader">Coleção com os dados.</param>
        /// <param name="company">Nome da empresa no arquivo excel.</param>
        /// <param name="sheet">Nome da planilha.</param>
        /// <returns></returns>
        public static HzNPOIWorkbook Export(DbDataReader reader, string company, string sheet)
        {
            HzNPOIWorkbook wb = null;
            try
            {
                if (reader.HasRows)
                {
                    wb = new HzNPOIWorkbook(company, sheet);
                    if (wb.CreateExcel(sheet))
                    {
                        for (int col = 0; col < reader.FieldCount; ++col)
                            wb.WriteHeader(reader.GetName(col));
                    }
                    while (reader.Read())
                    {
                        for (int col = 0; col < reader.FieldCount; ++col)
                        {
                            wb.WriteBodyCell(col == 0, reader[col].ToString());
                        }
                    }
                }

                return wb;
            }
            catch 
            {
                return null;
            }
        }

        /// <summary>
        /// Exporta os dados para excel baseados em uma DATATABLE.
        /// </summary>
        /// <param name="table">Coleção com os dados.</param>
        /// <param name="company">Nome da empresa no arquivo excel.</param>
        /// <param name="sheet">Nome da planilha.</param>
        /// <returns></returns>
        public static HzNPOIWorkbook Export(DataTable table, string company, string sheet)
        {
            HzNPOIWorkbook wb = null;
            try
            {
                if (table != null && table.Rows.Count > 0)
                {
                    wb = new HzNPOIWorkbook(company, sheet);
                    if (wb.CreateExcel(sheet))
                    {
                        foreach(DataColumn column in table.Columns)
                            wb.WriteHeader(column.Caption);
                    }

                    foreach(DataRow row in table.Rows)
                    {
                        for (int col = 0; col < table.Columns.Count; ++col)
                            wb.WriteBodyCell(col == 0, row[col].ToString());
                    }
                }

                return wb;
            }
            catch (Exception ex) 
            {
                StreamWriter writer = new StreamWriter("ErroExcel.txt");
                writer.WriteLine(ex.Message);
                writer.Close();
                writer = null;
                return null;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Evento de criação da classe.
        /// </summary>
        /// <param name="company">Nome da companhia.</param>
        /// <param name="subject">Nome do relatório.</param>
        public HzNPOIWorkbook(string company, string subject)
        {
            this.WorkBoook = new XSSFWorkbook();
            this.FileName = "report";
            //this.DocumentSummaryInformation = PropertySetFactory.CreateDocumentSummaryInformation();
            //this.DocumentSummaryInformation.Company = company;
            //this.SummaryInformation = PropertySetFactory.CreateSummaryInformation();
            //this.SummaryInformation.Subject = subject;
        }
        #endregion
    }
}
