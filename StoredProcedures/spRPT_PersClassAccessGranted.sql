create procedure [dbo].[spRPT_PersClassAccessGranted]
@persid varchar(50) = null,
@startdate datetime,  
@enddate datetime,
@clientid varchar(50) = null,  
@devices varchar(1000) = null,
@company varchar(1000) = null,
@persclassid varchar(1000) = null
with encryption as

declare @sql varchar(5000)  
declare @where varchar(5000)  
declare @server varchar(100)

set @server = convert(varchar, SERVERPROPERTY('MachineName')) + '\BIS_ACE'
set @where = ' where '

set @sql = 'SELECT LogEvent.ID, DataAcesso = convert(varchar, eventCreationtime, 103) + '' '' +   
convert(varchar, eventCreationtime, 108), Nome = firstname + '' '' + isnull(lastname, ''''), 
Empresa = CompanyNO, CPF = case when persclass = ''E'' then persno else passportno end,
Unidade = cli.Name, TipoAcesso = AddressTag from LogEventValue
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

set @where = @where + ' LogEventType.ID = 1 and LogState.stateNumber in (4101) and eventValueName = ''PERSID'''

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