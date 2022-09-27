using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Reports
{
    public class RPTCECNC
    {
        private readonly ArvoreOpcoes _arvoreOpcoes;

        public RPTCECNC(ArvoreOpcoes arvoreOpcoes)
        {
            _arvoreOpcoes = arvoreOpcoes;
        }


        /// <summary>
        /// Retorna os QRCodes dos visitantes.
        /// </summary>
        /// <param name="dbcontext">Conexão com o banco de dados.</param>
        /// <param name="cmpDocumento">Documento do Visitante.</param>
        /// <param name="cmpNoVisitante">Nome do Visitante.</param>
        /// <returns></returns>
        public DataTable LoadAcessos(DatabaseContext dbcontext, string datestart, string dateend)
        {
            try
            {
                bool bWhere = false;
                string sql = String.Format("set dateformat 'dmy' select Data = EventTime, Local = EventObjectName, Nome = CardUserName, NCartao = CardUserNumber, Documento = document, Torre, Pavimento, Empresa, TipoUsuario from Horizon.dbo.tblAcessosDelta where EventTime >= '{0}' and EventTime <= '{1}' order by EventTime",
                    datestart, dateend);

                return dbcontext.LoadDatatable(dbcontext, sql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
