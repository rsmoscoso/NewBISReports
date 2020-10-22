alter procedure [dbo].[spRPT_PersClassAccessGranted_testeLibra]
	@persid varchar(50) = null,
	@startdate datetime,
	@enddate datetime,
	@clientid varchar(50) = null,
	@devices varchar(1000) = null,
	@company varchar(1000) = null,
	@persclassid varchar(1000) = null,
	@accesstype varchar(50),
	@dateformat varchar(50) = 'dmy'
with
	encryption
as

declare @sql varchar(5000)  
declare @where varchar(5000)  
declare @server varchar(100)
declare @codigoData varchar(50)
declare @codigoHora varchar(100)

--determinação do código do formato da data
set @codigoData = CASE 
	when @dateformat = 'dmy' then  '103' --dd/mm/yyyy
	when @dateformat = 'mdy' then  '101'  --mm/dd/yyyy
	else '103'
End;
--determinação do codigo do formato da hora
set @codigoHora = CASE
	when @dateformat = 'dmy' then 'convert(varchar, eventCreationtime, 108)' --hh:mm:ss (24h)
	when @dateformat = 'mdy' then 'RIGHT(convert(varchar, eventCreationtime, 22), 11)' --mm/dd/yy hh:mi:ss AM (or PM) -> os 11 ultimos caracteres
	else 'convert(varchar, eventCreationtime, 108)'
End;

set @server = convert(varchar, SERVERPROPERTY('MachineName')) + '\BIS_ACE'
set @where = ' where '


set @sql = 'set dateformat '+ @dateformat+' SELECT 
LogEvent.ID,
DataAcesso = convert(varchar, eventCreationtime, '+ @codigoData +') + '' '' +  '+ @codigoHora +', 
Nome = firstname + '' '' + isnull(lastname, ''''), 
Empresa = CompanyNO, 
TipoAcesso = AddressTag,
CPF = case when persclass <> ''V'' then persno else passportno end,
Unidade = cli.Name 
from LogEventValue
inner join LogEvent2Value on LogEvent2Value.valueId = LogEventValue.Id
inner join LogEvent on LogEvent.Id = LogEvent2Value.eventId
inner join LogState on BISEventLog..LogState.Id = LogEvent.stateId 
inner join LogEventValueType on LogEventValueType.Id = LogEventValue.eventTypeId 
inner join LogEventType on LogEventType.ID = LogEvent.eventTypeId 
inner join LogAddress on LogAddress.ID = LogEvent.AddressID 
inner join [' + @server + '].acedb.bsuser.persons per on per.persid collate SQL_Latin1_General_CP1_CI_AS = stringValue 
inner join [' + @server + '].acedb.bsuser.persclasses perclass on perclass.persclassid = per.persclassid 
left outer join [' + @server + '].acedb.bsuser.companies cmp on cmp.companyid = per.companyid 
left outer join [' + @server + '].acedb.bsuser.visitors vis on vis.persid = per.persid 
inner join [' + @server + '].acedb.bsuser.clients cli on cli.clientid = per.clientid '

set @where = @where + ' LogEventType.ID = 1 and LogState.stateNumber in (' + @accesstype + ') and eventValueName = ''PERSID'''
--set @where = @where + ' LogEventType.ID = 1 and LogState.stateNumber in (' + @accesstype + ')'

if (not @devices is null)
	set @where = @where + ' and AddressTag in (' + @devices + ')'

if (not @persclassid is null)
	set @where = @where + ' and per.persclassid in (' + @persclassid + ')'
	
set @where = @where + ' and LogEvent.ID in (select LogEvent.Id from LogEventValueType
 inner join LogEventValue on LogEventValue.eventTypeId = LogEventValueType.Id
 inner join LogEvent2Value on LogEvent2Value.valueId = LogEventValue.Id
 inner join LogEvent on LogEvent.Id = LogEvent2Value.eventId '
 
 set @where = @where + ' where eventCreationTime >= ''' + convert(varchar, @startdate) + ''' and eventCreationTime <= ''' + convert(varchar, @enddate) + ''''
 
/* se há pessoas selecionadas, prioridade na pesquisa */
if (not @persid is null)
	set @where = @where + ' and eventValueName = ''PERSID'' and stringValue = ''' + @persid + ''''

else if (not @clientid is null and @devices is null)
	set @where = @where + ' and LogEventValueType.eventValueName = ''ORTSPFAD'' and stringValue like ''%' + @clientid + '%'''
 

/*
 * Se empresa for preenchida e os devices + unidades não,
 * pesquisa apenas pela empresa ou
 * se empresa e unidades forem preenchidos, pesquisa pelos empresa pois
 * o primeiro select pesquisa a unidade */
else if (not @company is null and @devices is null and @clientid is null)
	set @where = @where + ' and eventValueName = ''COMPANY'' and stringValue collate SQL_Latin1_General_CP1_CI_AS in (' + @company + ')'

set @where = @where + ')'

set @where = @where + ' order by eventCreationtime, Nome'

exec (@sql + @where)
