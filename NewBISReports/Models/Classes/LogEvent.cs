using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBISReports.Models.Classes
{
    /// <summary>
    /// Classe com os eventos gerados no banco BISEventLog.
    /// </summary>
    public class LogEvent
    {
        #region Enumeration
        /// <summary>
        /// Estados de evento.
        /// </summary>
        public enum LOGEVENT_STATE
        {
            LOGEVENTSTATE_ACCESSGRANTED = 4101,
            LOGEVENTSTATE_ACCESSDENIED = 4112,
            LOGEVENTSTATE_ACCESSINVALID = 4110,
            LOGEVENTSTATE_ACCESSBLOCKED = 4111,
            LOGEVENTSTATE_ACCESSUNKNOWN = 4109
        }

        /// <summary>
        /// Tipos de evento.
        /// </summary>
        public enum LOGEVENT_VALUETYPE
        {
            LOGEVENTVALUETYPE_PERSID = 0,
            LOGEVENTVALUETYPE_PERSNO = 1,
            LOGEVENTVALUETYPE_CARDNO = 2,
            LOGEVENTVALUETYPE_FIRSTNAME = 3,
            LOGEVENTVALUETYPE_LASTNAME = 4,
            LOGEVENTVALUETYPE_COMPANY = 5
        }
        #endregion

        #region Variables
        /// <summary>
        /// Matrícula ou documento da pessoa.
        /// </summary>
        public string Matricula { get; set; }
        /// <summary>
        /// Nome da pessoa.
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// Data do evento.
        /// </summary>
        public string DataAcesso { get; set; }
        /// <summary>
        /// Unidade onde houve o evento.
        /// </summary>
        public string LocalAcesso { get; set; }
        /// <summary>
        /// Device onde houve o evento.
        /// </summary>
        public string EnderecoAcesso { get; set; }
        /// <summary>
        /// Nome da empresa da pessoa, caso exista.
        /// </summary>
        public string Empresa { get; set; }
        /// <summary>
        /// CPF da Pessoa.
        /// </summary>
        public string CPF { get; set; }
        ///// <summary>
        ///// Primeiro nome da pessoa.
        ///// </summary>
        //public string Firstname { get; set; }
        ///// <summary>
        ///// Sobrenome da pessoa.
        ///// </summary>
        //public string Lastname { get; set; }
        /// <summary>
        /// Descrição do local do acesso.
        /// </summary>
        public string TipoAcesso { get; set; }
        public Int32 Dia { get; set; }
        public Int32 Total { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Retorna a descrição do tipo do valor para a string de pesquisa.
        /// </summary>
        /// <param name="valuetype">Enumeração do tipo do valor.</param>
        /// <returns></returns>
        public static string GetValueType(LOGEVENT_VALUETYPE valuetype)
        {
            string retval = "PERSID";
            switch (valuetype)
            {
                case LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_PERSID:
                    retval = "PERSID";
                    break;

                case LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_PERSNO:
                    retval = "PERSNO";
                    break;

                case LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_CARDNO:
                    retval = "CARDNO";
                    break;

                case LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_FIRSTNAME:
                    retval = "FIRSTNAME";
                    break;

                case LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_LASTNAME:
                    retval = "LASTNAME";
                    break;

                case LOGEVENT_VALUETYPE.LOGEVENTVALUETYPE_COMPANY:
                    retval = "COMPANY";
                    break;
            }

            return retval;
        }
        #endregion
    }
}
