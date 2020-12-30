alter procedure [dbo].[spRPT_AccessGranted]
	@persid varchar(50) = null,
	@startdate datetime,
	@enddate datetime,
	@clientid varchar(50) = null,
	@devices varchar(1000) = null,
	@company varchar(1000) = null,
	@accesstype varchar(50)
with
	encryption
as

declare @sql varchar(5000)  = ' ';
declare @where varchar(5000)  = ' where '
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
if (not @clientid is null and @devices is null and @devices != 'Common')
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

--se houver mais de um filtro, já construir o SELECT mais externo. As colunas devem estar com os msmos nomes das do select intermediário
if(@num_filtros > 1)
	set @sql = 'SELECT DataAcesso, Leitor, Nome, ID, Persid, Cardno ,Empresa, TipoAcesso, CPF, UnidadedaPessoa from ('

--construção do select intermediário
set @sql = @sql + 'SELECT 
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
--filtro de UnidadePessoa (Client) 
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

--se não houver mais filtros fecha a string com order by, caso contrário coloca os filtros restantes e o order by no select externo
if(@num_filtros <=1)
	set @where = @where + ' order by eventCreationtime'
else
begin
	--fechamento do select externo e inicio dos filtros externos
	set @where = @where +  ') x 
	where'

	--flag para controlar a adiçaõ do "and" se houver mais de um filtro por fora
	declare @flag_and BIT = 0

	--filtro de UnidadePessoa (Client) 
	if (@filtro_UnidadedaPessoa = 1)
	begin
		if (@flag_and = 1)
			set @where = @where + ' and'
		set @where = @where + ' UnidadedaPessoa like ''%' + @clientid + '%'''
		--marca que já teve um filtro, os proximos deverão adicionar "and" na frente
		set @flag_and = 1
	end
	--filtro de Empresa
	if (@filtro_Empresa = 1)
	 begin
		if (@flag_and = 1)
			set @where = @where + ' and'
		set @where = @where + ' Empresa collate SQL_Latin1_General_CP1_CI_AS in (' + @company + ')'
		--marca que já teve um filtro, os proximos deverão adicionar "and" na frente
		set @flag_and = 1
	end

	--acrescenta o order by
	set @where = @where + ' order by DataAcesso'

end
exec (@sql + @where)