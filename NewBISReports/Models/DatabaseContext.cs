using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models
{
    public class DatabaseContext : DbContext
    {
        #region Variables
        /// <summary>
        /// String de conexão com o banco de ados.
        /// </summary>
        public string connectionstring { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Carrega uma tabela baseada na string SQL.
        /// </summary>
        /// <param name="dbcontext">Conexão com base de dados.</param>
        /// <param name="sql">String com SQL.</param>
        /// <returns></returns>
        public DataTable LoadDatatable(DatabaseContext dbcontext, string sql)
        {
            try
            {
                var dt = new DataTable();
                var conn = dbcontext.Database.GetDbConnection();
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retorna o IP ou nome do servidor de banco de dados.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <returns></returns>
        public string GetHost()
        {
            int pos = -1;
            string retval = this.Database.GetDbConnection().DataSource;

            if ((pos = retval.IndexOf(@"\")) > -1)
                retval = retval.Substring(0, pos);

            return retval;
        }
        #endregion

        #region Events
        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="options"></param>
        public DatabaseContext(string connectionstring) : base()
        {
            this.connectionstring = connectionstring;
            this.Database.SetCommandTimeout(6000);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(this.connectionstring);
        #endregion
    }
}
