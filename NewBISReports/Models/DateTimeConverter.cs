
using System;
using System.Globalization;

namespace NewBISReports.Models
{
    ///<summary>
    ///Utilizada para normalizar os formatos de data e Hora, para que as consultas SQL recebam sempre o formato 
    /// Portugês do brasil (dd/MM/yyyy hh:mm:ss), bem como a conversão desejada no excel de saída
    ///</summary>
    public class DateTimeConverter
    {
        private readonly ArvoreOpcoes _arvoreOpcoes;
        public DateTimeConverter(ArvoreOpcoes arvoreOpcoes)
        {
            _arvoreOpcoes = arvoreOpcoes;
        }

        /// <summary>
        /// Converte a data em Pt-BR para o formatop especificado na arvoreOpcoes
        /// </summary>
        /// <param name="dateTimeRaw"></param>
        /// <returns>String de data e hora no formato especificado na arvore de opcoes</returns>
        public string FromPtBR(string dateTimeRaw)
        {
            switch (_arvoreOpcoes.FormatoDataHora)
            {
                case "pt-BR":
                    return (dateTimeRaw);
                case "en":
                    //Converte a string em um objeto DAteTime
                    DateTime enDateTime = DateTime.ParseExact(dateTimeRaw,
                                                "dd/MM/yyyy HH:mm",
                                                CultureInfo.InvariantCulture);
                    //Converte devolta para en
                    string value = enDateTime.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    return (value);
                default:
                    return (dateTimeRaw);
            }

        }

        /// <summary>
        /// Converte a data em Pt-BR para o formatop especificado na arvoreOpcoes
        /// </summary>
        /// <param name="dateTimeRaw"></param>
        /// <returns>String de data e hora no formato especificado na arvore de opcoes</returns>
        public string FromPtBRWithSeconds(string dateTimeRaw)
        {
            switch (_arvoreOpcoes.FormatoDataHora)
            {
                case "pt-BR":
                    return (dateTimeRaw);
                case "en":
                    //Converte a string em um objeto DAteTime
                    DateTime enDateTime = DateTime.ParseExact(dateTimeRaw,
                                                "dd/MM/yyyy HH:mm:ss",
                                                CultureInfo.InvariantCulture);
                    //Converte devolta para en
                    string value = enDateTime.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    return (value);
                default:
                    return (dateTimeRaw);
            }

        }
        /// <summary>
        /// Converte a data no formato especificado na arvoreOpcoes para em Pt-BR
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>String de data e hora no em pt-BR</returns>
        public string ToPtBR(string dateTime)
        {
            switch (_arvoreOpcoes.FormatoDataHora)
            {
                case "pt-BR":
                    return (dateTime);
                case "en":
                    //Converte a string em um objeto DAteTime
                    DateTime enDateTime = DateTime.ParseExact(dateTime,
                                                "MM/dd/yyyy hh:mm tt",
                                                CultureInfo.InvariantCulture);
                    //Converte devolta para Pt-BR
                    string value = enDateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    return (value);
                default:
                    return (dateTime);
            }
        }
    }
}