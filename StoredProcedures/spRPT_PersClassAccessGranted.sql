alter procedure [dbo].[spRPT_PersClassAccessGranted]
	@persid varchar(50) = null,
	@startdate datetime,
	@enddate datetime,
	@clientid varchar(50) = null,
	@devices varchar(1000) = null,
	@company varchar(1000) = null,
	@persclassid varchar(1000) = null,
	@accesstype varchar(50)
with
	encryption
as

declare @sql varchar(5000)  
declare @where varchar(5000)  = ' where '
declare @server varchar(100)

/*** Lógica de filtro de resultados
	Como não é possível usar o "LogEventValueType.eventValueName" para filtrar mais de um parâmetro,
	do segundo em diante deve-se filtrar em um select por fora.

	Assim, o passo a oasso de gerar a string de consulta é:
	1-Determina-se o numero de filtros que dependem de "LogEventValueType.eventValueName", ou seja mutuamente exclusivos
	2-Escolhe-se o filtro interno, ou seja que será executado no Select mais central
	3- Se houver mais filtros além do usado no passo 2, constrói-se o select por fora com os filtros extra
***/
--detectar se há mais de um filtro
declare @num_filtros smallint = 0

--uma flag para cada tipo de filtro mutuamente exclusivo
declare @filtro_Persid BIT = 0
declare @filtro_UnidadedaPessoa BIT = 0
declare @filtro_Empresa BIT = 0

--avalia filtro de persid
if (not @persid is null)
begin
	set @filtro_Persid = 1
	set @num_filtros = @num_filtros + 1
end

--avalia filtro de UnidadePEssoa (client)
--filtro de client e device agora é dissociado
if (not @clientid is null) --and @devices is null and @devices != 'Common')
begin
	set @filtro_UnidadedaPessoa = 1
	set @num_filtros = @num_filtros+1
end

--avalia filtro de Empresa
if (not @company is null)
begin
	set @filtro_Empresa = 1
	set @num_filtros = @num_filtros+1
end

set @server = convert(varchar, SERVERPROPERTY('MachineName')) + '\BIS_ACE'

/* Encapsulamos a SP spRT_AccessGranted em um outro select para só depois fazer joins com a instância ACE
   Assim, apenas o filtro de perclassid é feito no select mais externo
*/
set @sql = 'SELECT DataAcesso, Leitor, Nome, x.Persid, Cardno, 
Empresa, 
TipoAcesso
,CPF = case when persclass <> ''V'' then x.CPF else passportno collate SQL_Latin1_General_CP1_CI_AS end
,UnidadedaPessoa 
FROM( 
	SELECT 
LogEvent.ID,
DataAcesso = convert(varchar, eventCreationtime, 103) + '' '' +   convert(varchar, eventCreationtime, 108), 
Leitor = AddressTag,
CONCAT(MAX(CASE WHEN LogEventValueType.eventValueName = ''FIRSTNAME'' THEN stringValue + '' '' ELSE NULL END), MAX(CASE WHEN LogEventValueType.eventValueName = ''NAME'' THEN stringValue ELSE NULL END)) AS Nome,  
MAX(CASE WHEN LogEventValueType.eventValueName = ''PERSID'' THEN stringValue ELSE NULL END) AS Persid,    
MAX(CASE WHEN LogEventValueType.eventValueName = ''CARDNO'' THEN stringValue ELSE NULL END) AS Cardno,   
MAX(CASE WHEN LogEventValueType.eventValueName = ''COMPANY'' THEN stringValue ELSE NULL END) AS Empresa,   
MAX(CASE WHEN LogEventValueType.eventValueName = ''KURZTEXT'' THEN stringValue ELSE NULL END) AS TipoAcesso,   
MAX(CASE WHEN LogEventValueType.eventValueName = ''PERSNO'' THEN stringValue ELSE NULL END) AS CPF,
MAX(CASE WHEN LogEventValueType.eventValueName = ''ORTSPFAD'' THEN stringValue ELSE NULL END) AS UnidadedaPessoa from LogEventValue
inner join LogEvent2Value on LogEvent2Value.valueId = LogEventValue.Id
inner join LogEvent on LogEvent.Id = LogEvent2Value.eventId
inner join LogState on BISEventLog..LogState.Id = LogEvent.stateId 
inner join LogEventValueType on LogEventValueType.Id = LogEventValue.eventTypeId 
inner join LogEventType on LogEventType.ID = LogEvent.eventTypeId 
inner join LogAddress on LogAddress.ID = LogEvent.AddressID '

set @where = @where + ' LogEventType.ID = 1 and LogState.stateNumber in (' + @accesstype + ') '


if (not @devices is null)
	set @where = @where + ' and AddressTag in (' + @devices + ')'

	--construção do select interno dentro da string do "where"
set @where = @where + ' and LogEvent.ID in (select LogEvent.Id from LogEventValueType
 inner join LogEventValue on LogEventValue.eventTypeId = LogEventValueType.Id
 inner join LogEvent2Value on LogEvent2Value.valueId = LogEventValue.Id
 inner join LogEvent on LogEvent.Id = LogEvent2Value.eventId '
 
set @where = @where + ' where eventCreationTime >= ''' + convert(varchar, @startdate) + ''' and eventCreationTime <= ''' + convert(varchar, @enddate) + ''''
 

--utiliza apenas um filtro dentro do select interno, avaliado por ordem de prioridade, por isso são avaliados com elseif
--filtro de pessoa
if(@filtro_Persid = 1)
	set @where = @where + ' and eventValueName = ''PERSID'' and stringValue = ''' + @persid + ''''
---filtro de UnidadePessoa (Client) 
else if (@filtro_UnidadedaPessoa = 1)
begin
	set @where = @where + ' and LogEventValueType.eventValueName = ''ORTSPFAD'' and stringValue like ''%' + @clientid + '%'''
	set @filtro_UnidadedaPessoa = 0
end
--filtro de Empresa
else if (@filtro_Empresa = 1)
begin
	set @where = @where + ' and eventValueName = ''COMPANY'' and stringValue collate SQL_Latin1_General_CP1_CI_AS in (' + @company + ')'
	set @filtro_Empresa = 0
end
set @where = @where + ')'

set @where = @where + ' group by LogEvent.ID, AddressTag, eventCreationtime' 

--fechamento do select intermediário e inicio dos joins ocm a instancia ACE e dos filtros externos
set @where = @where +  ') x 
inner join [' + @server + '].acedb.bsuser.persons per on per.persid collate SQL_Latin1_General_CP1_CI_AS = x.Persid 
inner join [' + @server + '].acedb.bsuser.persclasses perclass on perclass.persclassid = per.persclassid  
left outer join [' + @server + '].acedb.bsuser.visitors vis on vis.persid = per.persid 
where'

--filtro de Persclassid., tipo de pessoa
set @where = @where + ' per.persclassid in (' + @persclassid + ')'

--filtro de UnidadePessoa (Client) 
if (@filtro_UnidadedaPessoa = 1)
	set @where = @where + ' and UnidadedaPessoa like ''%' + @clientid + '%'''
--filtro de Empresa
if (@filtro_Empresa = 1)
	set @where = @where + ' and Empresa collate SQL_Latin1_General_CP1_CI_AS in (' + @company + ')'

--acrescenta o order by
set @where = @where + ' order by DataAcesso'




exec (@sql + @where)
