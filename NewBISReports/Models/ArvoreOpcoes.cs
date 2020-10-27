
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;



namespace NewBISReports.Models
{
    ///<summary>
    ///Utilizada para construir o menu de relatórios na barra lateral esquerda do site
    ///via appsettings.json, evitando assim, condicionais por cliente direto no código do _layout.cshtml
    ///</summary>
    public class ArvoreOpcoes
    {

        #region propriedades para construção de menu
        //menu raiz
        public bool AdministracaoRaiz { get; set; }
        public bool AdicionarUsuarios { get; set; }
        public bool AlterarSenhas { get; set; }
        public bool RemoverUsurios { get; set; }
        //menu raiz
        public bool EventosDeAcessoRaiz { get; set; }
        public bool EventosDeAcesso { get; set; }
        public bool AcessosAnaliticosGeral { get; set; }
        public bool TabelaRefeicoes { get; set; }
        public bool DashboardTotalRefeicoes { get; set; }
        public bool TotalDeRefeicoes { get; set; }
        public bool ExportarRefeicoes { get; set; }
        //Menu Raiz
        public bool OperacionaisRaiz { get; set; }
        public bool Excessao { get; set; }
        public bool Banheiro { get; set; }
        public bool PessoasSemFotografia { get; set; }
        public bool PessoasSemCracha { get; set; }
        public bool TempoSemUsoDoCracha { get; set; }
        //Menu Raiz
        public bool AdministrativosRaiz { get; set; }
        public bool Pessoas { get; set; }
        public bool PerfilDasPessoas { get; set; }
        public bool AutorizacoesDasPessoas { get; set; }
        public bool LeitoresPorAutorizacoes { get; set; }
        public bool PessoasBloqueadas { get; set; }
        public bool TodosOsVisitantes { get; set; }
        public bool PessoasPorArea { get; set; }
        //Menu Raiz
        public bool VisitantesRaiz { get; set; }
        public bool QrCodeDosVisitantes { get; set; }
        public bool ImportarVisitantes { get; set; }

        #endregion

        #region outras propriedades de configuração
        public bool UseLogin { get; set; }
        public bool Meal { get; set; }

        //Customização de nome de "pessoas", solicitado pela FortKnox
        public string PersonsLabel { get; set; }
        //mostrar ou não cmapo de documento, solicitado pela FortKnox
        public bool ShowDocumentSearch { get; set; }
        //colunas não desejadas no relatório analytics_granted_BIS
        public List<string> RemoverColunasAlanyticsGranted { get; set; }
        //Formato data e hora
        public string FormatoDataHora { get; set; }
        #endregion


        //propriedades para injeção de depedencia
        private IConfiguration _configuration { get; }




        //construtor lê do appsettings.json e popula as propriedades.
        public ArvoreOpcoes(IConfiguration configuration)
        {
            _configuration = configuration;

            //Populando os itens um a um

            //retorna o nome do cliente
            string nomeCliente = _configuration.GetSection("Default")["Name"];

            //retorna se o sistema de login está ativo ou não
            UseLogin = bool.Parse(_configuration.GetSection(nomeCliente)["useLogin"]);

            //retorna se o sistema de controle de refeições deve ser utilziado ou não
            Meal = bool.Parse(_configuration.GetSection(nomeCliente)["Meal"]);

            //Retorna o "label" de pessoas no filtro 
            PersonsLabel = _configuration.GetSection(nomeCliente)["PersonsLabel"];

            //Mostra ou não a busca por documento
            //retorna se o sistema de controle de refeições deve ser utilziado ou não
            ShowDocumentSearch = bool.Parse(_configuration.GetSection(nomeCliente)["ShowDocumentSearch"]);

            //omitir colunas do relatório Access Granted
            var remArray = _configuration.GetSection(nomeCliente + ":RemoverColunasAccessGranted").Get<string[]>();
            RemoverColunasAlanyticsGranted = remArray == null ? new List<string>() : remArray.ToList();

            //formato da data e hora FormatoDataHora
            FormatoDataHora = _configuration.GetSection(nomeCliente)["FormatoDataHora"];
            //faz uma espécie de assert, se não for os casos esperados, força pt - BR para não quebrar o Front - end
            switch (FormatoDataHora)
            {
                case "pt-BR":
                       break;
                case "en":
                    break;
                default:
                 //força pt-BR
                    FormatoDataHora = "pt-BR";
                    break;
            }

            //começa pelo Login - avalia apenas se useLogin for verdadeiro
            if (UseLogin)
            {
                //avaliar quais itens do menu de login estarão disponíveis
                AdicionarUsuarios = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administracaoRaiz:adicionarUsuarios"]);
                AlterarSenhas = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administracaoRaiz:alterarSenhas"]);
                RemoverUsurios = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administracaoRaiz:removerUsurios"]);

            }
            else
            {
                AdicionarUsuarios = false;
                AlterarSenhas = false;
                RemoverUsurios = false;
            }
            //avalia se deve adicionar a raiz
            AdministracaoRaiz = (AdicionarUsuarios || AlterarSenhas || RemoverUsurios);

            //menu Eventos de acesso
            EventosDeAcesso = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:eventosDeAcessoRaiz:eventosDeAcesso"]);

            //o restante só é levado em consideração se a oopção "Meal" for true
            if (Meal)
            {
                AcessosAnaliticosGeral = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:eventosDeAcessoRaiz:acessosAnaliticosGeral"]);
                TabelaRefeicoes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:eventosDeAcessoRaiz:tabelaRefeicoes"]);
                DashboardTotalRefeicoes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:eventosDeAcessoRaiz:dashboardTotalRefeicoes"]);
                TotalDeRefeicoes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:eventosDeAcessoRaiz:totalDeRefeicoes"]);
                ExportarRefeicoes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:eventosDeAcessoRaiz:exportarRefeicoes"]);
            }
            else
            {
                AcessosAnaliticosGeral = false;
                TabelaRefeicoes = false;
                DashboardTotalRefeicoes = false;
                TotalDeRefeicoes = false;
                ExportarRefeicoes = false;
            }
            //avalia se adiciona a raiz
            EventosDeAcessoRaiz = (EventosDeAcesso || AcessosAnaliticosGeral || TabelaRefeicoes || DashboardTotalRefeicoes || TotalDeRefeicoes || ExportarRefeicoes);

            //menu Operacionais
            Excessao = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:operacionaisRaiz:excessao"]);
            Banheiro = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:operacionaisRaiz:banheiro"]);
            PessoasSemFotografia = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:operacionaisRaiz:pessoasSemFotografia"]);
            PessoasSemCracha = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:operacionaisRaiz:pessoasSemCracha"]);
            TempoSemUsoDoCracha = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:operacionaisRaiz:tempoSemUsoDoCracha"]);
            //avalia se adiciona a raiz
            OperacionaisRaiz = (Excessao || Banheiro || PessoasSemFotografia || PessoasSemCracha || TempoSemUsoDoCracha);

            //menu Administrativos
            Pessoas = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administrativosRaiz:Pessoas"]);
            PerfilDasPessoas = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administrativosRaiz:perfilDasPessoas"]);
            AutorizacoesDasPessoas = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administrativosRaiz:autorizacoesDasPessoas"]);
            LeitoresPorAutorizacoes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administrativosRaiz:leitoresPorAutorizacoes"]);
            PessoasBloqueadas = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administrativosRaiz:pessoasBloqueadas"]);
            TodosOsVisitantes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administrativosRaiz:todosOsVisitantes"]);
            AdministrativosRaiz = (Pessoas || PerfilDasPessoas || AutorizacoesDasPessoas || LeitoresPorAutorizacoes || PessoasBloqueadas || TodosOsVisitantes);
            PessoasPorArea = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:administrativosRaiz:pessoasporArea"]);

            //menu visitantes
            QrCodeDosVisitantes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:visitantesRaiz:qrCodeDosVisitantes"]);
            ImportarVisitantes = bool.Parse(_configuration.GetSection(nomeCliente)["arvoreOpcoes:visitantesRaiz:importarVisitantes"]);
            VisitantesRaiz = (QrCodeDosVisitantes || ImportarVisitantes);


        }

    }
}
