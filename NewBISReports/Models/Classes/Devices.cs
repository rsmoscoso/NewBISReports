using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    public class Devices
    {
        #region Variables
        /// <summary>
        /// ID do Despositivo.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Descrição do dispositivo.
        /// </summary>
        public string DESCRIPTION { get; set; }
        /// <summary>
        /// Texto para o log do dispositivo.
        /// </summary>
        public string DISPLAYTEXT { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retorna os dispositivos.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="clientid">ID da unidade.</param>
        /// <param name="description">Descrição do dispositivo.</param>
        /// <returns></returns>
        public static List<Devices> GetDevices(DatabaseContext dbcontext, string clientid, string description)
        {
            List<Devices> devices = new List<Devices>();
            try
            {
                string sql = "select * from bsuser.devices where clientid = '" + clientid + "' and description like '%" + description + "%' and type = 'wie1' order by description";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                    {
                        devices = GlobalFunctions.ConvertDataTable<Devices>(table);
                        Devices d = new Devices();
                        d.ID = "0";
                        d.DESCRIPTION = "TODOS";
                        d.DISPLAYTEXT = "TODOS";
                        devices.Add(d);
                        devices.Sort((x, y) => x.ID.CompareTo(y.ID));
                    }
                }
                return devices;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna os dispositivos.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="deviceid">Lista dos IDs dos dispositivos.</param>
        /// <returns></returns>
        public static string[] GetDevices(DatabaseContext dbcontext, string[] deviceid)
        {
            List<string> devices = new List<string>();
            try
            {
                string sql = "select displaytext from bsuser.devices where id ";
                if (deviceid != null && deviceid.Length > 0)
                {
                    string devid = "in (";
                    foreach (string id in deviceid)
                        devid += "'" + id + "',";
                    sql += devid.Substring(0, devid.Length - 1) + ")";
                }

                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                    {
                        foreach (DataRow row in table.Rows)
                            devices.Add(row["displaytext"].ToString());
                    }
                }
                return devices.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna os dados dos dispositivos.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="deviceid">ID do device.</param>
        /// <returns></returns>
        public static Devices GetDevicesClass(DatabaseContext dbcontext, string deviceid)
        {
            Devices retval = new Devices();
            try
            {
                string sql = "select * from bsuser.devices where id = '" + deviceid + "'";
                using (DataTable table = dbcontext.LoadDatatable(dbcontext, sql))
                {
                    if (table != null)
                    {
                        retval.ID = table.Rows[0]["id"].ToString();
                        retval.DESCRIPTION = table.Rows[0]["description"].ToString();
                        retval.DISPLAYTEXT = table.Rows[0]["displaytext"].ToString();
                    }
                }
                return retval;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
