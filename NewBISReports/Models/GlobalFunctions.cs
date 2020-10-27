using HzBISCommands;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewBISReports.Models
{
    public class SKServer
    {
        public string Server { get; set; }
        public int Port { get; set; }
    }

    /// <summary>
    /// Classe com funções genéricas.
    /// </summary>
    public class GlobalFunctions
    {
        #region Datatable
        /// <summary>
        /// Gets a Inverted DataTable
        /// </summary>
        /// <param name="table">Provided DataTable</param>
        /// <param name="columnX">X Axis Column</param>
        /// <param name="columnY">Y Axis Column</param>
        /// <param name="columnZ">Z Axis Column (values)</param>
        /// <param name="columnsToIgnore">Whether to ignore some column, it must be 
        /// provided here</param>
        /// <param name="nullValue">null Values to be filled</param> 
        /// <returns>C# Pivot Table Method  - Felipe Sabino</returns>
        public static DataTable GetInversedDataTable(DataTable table, string columnX,
             string columnY, string columnZ, string nullValue, bool sumValues)
        {
            //Create a DataTable to Return
            DataTable returnTable = new DataTable();

            if (columnX == "")
                columnX = table.Columns[0].ColumnName;

            //Add a Column at the beginning of the table
            returnTable.Columns.Add(columnY);


            //Read all DISTINCT values from columnX Column in the provided DataTale
            List<string> columnXValues = new List<string>();

            foreach (DataRow dr in table.Rows)
            {

                string columnXTemp = dr[columnX].ToString();
                if (!columnXValues.Contains(columnXTemp))
                {
                    //Read each row value, if it's different from others provided, add to 
                    //the list of values and creates a new Column with its value.
                    columnXValues.Add(columnXTemp);
                    returnTable.Columns.Add(columnXTemp);
                }
            }

            //Verify if Y and Z Axis columns re provided
            if (columnY != "" && columnZ != "")
            {
                //Read DISTINCT Values for Y Axis Column
                List<string> columnYValues = new List<string>();

                foreach (DataRow dr in table.Rows)
                {
                    if (!columnYValues.Contains(dr[columnY].ToString()))
                        columnYValues.Add(dr[columnY].ToString());
                }

                //Loop all Column Y Distinct Value
                foreach (string columnYValue in columnYValues)
                {
                    //Creates a new Row
                    DataRow drReturn = returnTable.NewRow();
                    drReturn[0] = columnYValue;
                    //foreach column Y value, The rows are selected distincted
                    DataRow[] rows = table.Select(columnY + "='" + columnYValue + "'");

                    //Read each row to fill the DataTable
                    foreach (DataRow dr in rows)
                    {
                        string rowColumnTitle = dr[columnX].ToString();

                        //Read each column to fill the DataTable
                        foreach (DataColumn dc in returnTable.Columns)
                        {
                            if (dc.ColumnName == rowColumnTitle)
                            {
                                //If Sum of Values is True it try to perform a Sum
                                //If sum is not possible due to value types, the value 
                                // displayed is the last one read
                                if (sumValues)
                                {
                                    try
                                    {
                                        drReturn[rowColumnTitle] =
                                             Convert.ToDecimal(drReturn[rowColumnTitle]) +
                                             Convert.ToDecimal(dr[columnZ]);
                                    }
                                    catch
                                    {
                                        drReturn[rowColumnTitle] = dr[columnZ];
                                    }
                                }
                                else
                                {
                                    drReturn[rowColumnTitle] = dr[columnZ];
                                }
                            }
                        }
                    }
                    returnTable.Rows.Add(drReturn);
                }
            }
            else
            {
                throw new Exception("The columns to perform inversion are not provided");
            }

            //if a nullValue is provided, fill the datable with it
            if (nullValue != "")
            {
                foreach (DataRow dr in returnTable.Rows)
                {
                    foreach (DataColumn dc in returnTable.Columns)
                    {
                        if (dr[dc.ColumnName].ToString() == "")
                            dr[dc.ColumnName] = nullValue;
                    }
                }
            }

            return returnTable;
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Serialização de uma tabela.
        /// </summary>
        /// <typeparam name="T">Classe para serialização.</typeparam>
        /// <param name="table">Tabela a ser serializada.</param>
        /// <returns></returns>
        public static List<T> ConvertDataTable<T>(DataTable table)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        /// <summary>
        /// Retorna um item da tabela.
        /// </summary>
        /// <typeparam name="T">Classe serializada.</typeparam>
        /// <param name="row">Linha da tabela.</param>
        /// <returns></returns>
        public static T GetItem<T>(DataRow row)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            object objvalue = "";

            foreach (DataColumn column in row.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToLower() == column.ColumnName.ToLower())
                    {
                        if (row[column.ColumnName] is System.DBNull)
                        {
                            if (column.DataType == Type.GetType("System.String"))
                                objvalue = "";
                            else
                                objvalue = 0;
                        }
                        else
                            objvalue = row[column.ColumnName];
                        pro.SetValue(obj, objvalue, null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
        #endregion

        #region Excel
        /// <summary>
        /// Retorna array de bytes, padrão Excel, para o envio do arquivo para o cliente.
        /// </summary>
        /// <param name="table">Tabela para a gravação do Excel.</param>
        /// <param name="filename">Nome do arquivo Excel.</param>
        /// <param name="company">Nome da empresa para o arquivo Excel.</param>
        /// <param name="sheet">Nome da planilha.</param>
        /// <returns></returns>
        public static byte[] SaveExcel(DataTable table, string filename, string company, string sheet, DateTimeConverter _dateTimeConverter)
        {
            try
            {
                byte[] retval = null;
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                //Diogo - converte a coluna de "DataAcesso" para o formato de data especificado
                if (table.Columns.Contains("DataAcesso"))
                {
                    //tira o atributo readonly da coluna
                    table.Columns["DataAcesso"].ReadOnly = false;
                    //itera entre todas as linhas0
                    foreach (DataRow dr in table.Rows)
                    {
                        dr.BeginEdit();
                        dr["DataAcesso"] = _dateTimeConverter.FromPtBRWithSeconds(dr["DataAcesso"].ToString());
                        dr.EndEdit();
                    }
                    //confirma todas as alterações feitas
                    table.AcceptChanges();

                }

                HzNPOIWorkbook wb = HzNPOIWorkbook.Export(table, company, sheet);
                if (wb != null && wb.Save(filename))
                    retval = System.IO.File.ReadAllBytes(filename);
                return retval;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region General
        /// <summary>
        /// Retorna o nome e o sobrenome do servidor.
        /// </summary>
        /// <param name="nom_servidor">Nome do servidor no Oracle.</param>
        /// <param name="firstname">Primeiro nome.</param>
        /// <param name="lastname">Sobrenome completo.</param>
        public static void ParseName(string nom_servidor, out string firstname, out string lastname)
        {
            firstname = "";
            lastname = "";
            try
            {
                int i = nom_servidor.IndexOf(' ');
                if (i > -1)
                {
                    firstname = nom_servidor.Substring(0, i);
                    lastname = nom_servidor.Substring(i + 1, nom_servidor.Length - (i + 1));
                }
                else
                    firstname = nom_servidor;
            }
            catch
            {
            }
        }
        #endregion

        #region Database
        /// <summary>
        /// Remove qualquer inconsitência no relatório
        /// gerada pela falta de uma pessoa no evento.
        /// </summary>
        /// <param name="table">Tabela com o resultado da pesquisa.</param>
        /// <returns></returns>
        public static DataTable RemoveTrash(DataTable table, string filter)
        {
            try
            {
                if (table != null && table.Rows.Count > 0)
                {
                    DataView view = table.DefaultView;
                    view.RowFilter = "Persid <> ''" + (String.IsNullOrEmpty(filter) ? "" : " and " + filter);
                    table = view.ToTable();
                    if (table.Columns.Contains("Persid"))
                        table.Columns.Remove("Persid");
                    if (table.Columns.Contains("ID"))
                        table.Columns.Remove("ID");
                    if (table.Columns.Contains("ClientID"))
                        table.Columns.Remove("ClientID");
                }
                return table;
            }
            catch
            {
                return table;
            }
        }
        #endregion

        #region Socket
        /// <summary>
        /// Envia o comando JSON para o servidor de integração.
        /// </summary>
        /// <param name="server">Servidor de integração.</param>
        /// <param name="port">Porta do socket de servidor de integração.</param>
        /// <param name="json">JSON com as características do comando.</param>
        /// <returns>Retorna o JSON do objeto correspondente ao comando JSON.</returns>
        public static string SendJson(SKServer skserver, string json)
        {
            string retval = "";
            try
            {
                //client.SendMessage(json + '\n');

                System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(skserver.Server, skserver.Port);
                if (tcp.Connected)
                {
                    tcp.SendTimeout = -1;
                    Socket socket = tcp.Client;
                    socket.ReceiveTimeout = -1;
                    socket.Send(Encoding.ASCII.GetBytes(json + "\n"));
                    byte[] bt = new byte[1000000];
                    socket.Receive(bt);
                    socket.Close();
                    socket.Close();
                    tcp.Close();

                    retval = Encoding.ASCII.GetString(bt).Replace("\0", "");
                }

                return retval;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Import
        /// <summary>
        /// Retorna coleção com o cabeçalho da importação de acordo
        /// com o separador selecionado.
        /// </summary>
        /// <param name="line">Primeira linha do arquivo.</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetReaders(string line)
        {
            try
            {
                Dictionary<string, int> retval = new Dictionary<string, int>();
                string[] headers = line.Split(';');
                int i = 0;
                foreach (string head in headers)
                {
                    retval.Add(head, i++);
                }

                return retval;
            }
            catch
            {
                return null;
            }
        }

        public static List<BSVisitorsInfo> ReadExcelVisitor(string filename)
        {
            StreamReader reader = null;
            string line = "";
            int count = 0;
            int countproperties = 0;
            Dictionary<string, int> headers = new Dictionary<string, int>();
            List<BSVisitorsInfo> retval = new List<BSVisitorsInfo>();

            try
            {
                reader = new StreamReader(filename);
                while ((line = reader.ReadLine()) != null)
                {
                    if (count++ == 0)
                        headers = GlobalFunctions.GetReaders(line);
                    else
                    {
                        string[] data = line.Split(';');
                        BSVisitorsInfo properties = new BSVisitorsInfo();
                        foreach (PropertyInfo p in ((BSVisitorsInfo)properties).GetType().GetProperties())
                        {
                            countproperties++;
                            for (int i = 0; i < headers.Count; ++i)
                            {
                                try
                                {
                                    if (p.Name.ToLower().Trim().Equals(headers.ElementAt(i).Key.ToLower().Trim()))
                                    {
                                        if (p.PropertyType == Type.GetType("System.DateTime"))
                                        {
                                            DateTime dt = DateTime.Now;
                                            DateTime.TryParse(data[i], System.Globalization.CultureInfo.GetCultureInfo("pt-BR"),
                                                System.Globalization.DateTimeStyles.None,
                                                out dt);
                                            p.SetValue(properties, dt);
                                        }
                                        else
                                            p.SetValue(properties, data[i], null);
                                    }
                                    else
                                    {
                                        // Todas as autorizações são lidas na primeira propriedade.
                                        if (!String.IsNullOrEmpty(data[i].Trim()) && headers.ElementAt(i).Key.ToLower().IndexOf("autorizacao") > -1 && countproperties == 1)
                                        {
                                            BSAuthorizationInfo binfo = new BSAuthorizationInfo();
                                            binfo.SHORTNAME = data[i];
                                            properties.AUTHORIZATIONS.Add(binfo);

                                        }
                                    }
                                }
                                catch (Exception EX)
                                {
                                    // MessageBox.Show(EX.Message);
                                }
                            }
                        }

                        retval.Add(properties);
                    }
                }

                reader.Close();
                reader = null;

                return retval;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                return null;
            }
        }
        #endregion
    }
}
