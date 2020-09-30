alter procedure [dbo].[spRPT_AccessGranted]
@persid varchar(50) = null,
@startdate datetime,  
@enddate datetime,
@clientid varchar(50) = null,  
@devices varchar(1000) = null,
@company varchar(1000) = null
with encryption as

declare @sql varchar(5000)  
declare @where varchar(5000)  
declare @server varchar(100)

set @server = convert(varchar, SERVERPROPERTY('MachineName')) + '\BIS_ACE'
set @where = ' where '

set @sql = 'SELECT LogEvent.ID, DataAcesso = convert(varchar, eventCreationtime, 103) + '' '' +   
convert(varchar, eventCreationtime, 108),   
CONCAT(MAX(CASE WHEN LogEventValueType.eventValueName = ''FIRSTNAME'' THEN stringValue + '' '' ELSE NULL END),   
MAX(CASE WHEN LogEventValueType.eventValueName = ''NAME'' THEN stringValue ELSE NULL END)) AS Nome,  
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

set @where = @where + ' LogEventType.ID = 1 and LogState.stateNumber in (4101) '

if (not @devices is null)
	set @where = @where + ' and AddressTag in (' + @devices + ')'
	
set @where = @where + ' and LogEvent.ID in (select LogEvent.Id from LogEventValueType
 inner join LogEventValue on LogEventValue.eventTypeId = LogEventValueType.Id
 inner join LogEvent2Value on LogEvent2Value.valueId = LogEventValue.Id
 inner join LogEvent on LogEvent.Id = LogEvent2Value.eventId '
 
 set @where = @where + ' where eventCreationTime >= ''' + convert(varchar, @startdate) + ''' and eventCreationTime <= ''' + convert(varchar, @enddate) + ''''
 
 if (not @clientid is null and @devices is null and @devices != 'Common')
	set @where = @where + ' and LogEventValueType.eventValueName = ''ORTSPFAD'' and stringValue like ''%' + @clientid + '%'''
 
/* se há pessoas selecionadas, prioridade na pesquisa */
if (not @persid is null)
	set @where = @where + ' and eventValueName = ''PERSID'' and stringValue = ''' + @persid + ''''

/*
 * Se empresa for preenchida e os devices + unidades não,
 * pesquisa apenas pela empresa ou
 * se empresa e unidades forem preenchidos, pesquisa pelos empresa pois
 * o primeiro select pesquisa a unidade */
else if (not @company is null and @devices is null and @clientid is null)
	set @where = @where + ' and eventValueName = ''COMPANY'' and stringValue collate SQL_Latin1_General_CP1_CI_AS in (' + @company + ')'

set @where = @where + ')'

set @where = @where + ' group by LogEvent.ID, eventCreationtime order by eventCreationtime'

exec (@sql + @where)
--select @sql + @where
