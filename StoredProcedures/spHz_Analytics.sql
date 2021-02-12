USE [BISEventLog]
GO
/****** Object:  StoredProcedure [dbo].[spHz_Analytics]    Script Date: 10/02/2021 10:51:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[spHz_Analytics]  
@type smallint  
as  
  
declare @data date   
declare @dateini varchar(50)    
declare @dateend varchar(50)    
declare @timeini varchar(20)    
declare @timeend varchar(20)  
declare @eventid int  
declare @nome varchar(100)  
declare @company varchar(100)  
declare @localacesso varchar(100)  
declare @tiporefeicao varchar(50)  
declare @entsia varchar(5)  
declare @dataacesso datetime  
declare @cpf varchar(100)  
declare @unidade varchar(100)  
declare @clientid varchar(50)
declare @DataTemp as date
set @data = convert(date, getdate()) 
--set @DataTemp= convert(date, DATEFROMPARTS (2021,02,02))  
--set @data = convert(date, @DataTemp) 
    --select @DataTemp
select @timeini = cmpTmTypeIni, @timeend = cmpTmTypeFin from HzBIS..tblMealType    
where cmpNuType = @type    
    
if (@type < 4)    
 set @dateini = convert(varchar, getdate(), 103) + ' ' + @timeini    
else    
 set @dateini = convert(varchar, dateadd(DAY, -1, getdate()), 103) + ' ' + @timeini    
    
set @dateend = convert(varchar, getdate(), 103) + ' ' + @timeend    
 --adicionado o tipo 99, que na tabela de horários deve contemplar todo o período do de café da manhã à ceia na teabela. 
 -- edeve ser programado um job para este tipo uma vez ao dia, de preferência umas 3h após o valor do horário final, 
 -- de forma que a tblRefeições receba eventos que tenham sido propagados para  o BisEventLog com atraso.
 if(@type=99)
 begin
	set @dateini = convert(varchar, dateadd(DAY, -1, getdate()), 103) + ' ' + @timeini    
    
set @dateend = convert(varchar, getdate(), 103) + ' ' + @timeend

select @dateini
select @dateend
 end 

declare o cursor for    
SELECT LogEvent.ID, eventcreationtime, TipoRefeicao = dbo.fnHZ_ReturnMealTypeName(convert(varchar, eventCreationTime, 108), addresstag),  
ClientID = dbo.fnHZ_GetClientID(addresstag),  
CONCAT(MAX(CASE WHEN LogEventValueType.eventValueName = 'FIRSTNAME' THEN stringValue + ' ' ELSE NULL END),     
MAX(CASE WHEN LogEventValueType.eventValueName = 'NAME' THEN stringValue ELSE NULL END)) AS Nome,    
MAX(CASE WHEN LogEventValueType.eventValueName = 'COMPANY' THEN stringValue ELSE NULL END) AS Empresa,     
MAX(CASE WHEN LogEventValueType.eventValueName = 'KURZTEXT' THEN stringValue ELSE NULL END) AS TipoAcesso,     
MAX(CASE WHEN LogEventValueType.eventValueName = 'PERSNO' THEN stringValue ELSE NULL END) AS CPF,  
MAX(CASE WHEN LogEventValueType.eventValueName = 'ORTSPFAD' THEN stringValue ELSE NULL END) AS UnidadedaPessoa from LogEventValue  
inner join LogEvent2Value on LogEvent2Value.valueId = LogEventValue.Id  
inner join LogEvent on LogEvent.Id = LogEvent2Value.eventId  
inner join LogState on BISEventLog..LogState.Id = LogEvent.stateId   
inner join LogEventValueType on LogEventValueType.Id = LogEventValue.eventTypeId   
inner join LogEventType on LogEventType.ID = LogEvent.eventTypeId   
inner join LogAddress on LogAddress.ID = LogEvent.AddressID   
where LogEventType.ID = 1 and LogState.stateNumber in (4101) and addresstag in (select devicenamelogevent collate SQL_Latin1_General_CP1_CI_AS from HzBIS..tblAMCRefeicao)   
and LogEvent.ID in (select LogEvent.Id from LogEventValueType  
     inner join LogEventValue on LogEventValue.eventTypeId = LogEventValueType.Id  
     inner join LogEvent2Value on LogEvent2Value.valueId = LogEventValue.Id  
     inner join LogEvent on LogEvent.Id = LogEvent2Value.eventId      
     where eventCreationTime >= @dateini and eventCreationTime <= @dateend)  
group by LogEvent.ID, addresstag, eventCreationtime order by eventCreationtime  
  
open o    
    
fetch next from o    
into  @eventid, @dataacesso, @tiporefeicao, @clientid, @nome, @company, @localacesso, @cpf, @unidade  
    
while @@FETCH_STATUS = 0    
begin    	
	if not exists(select idevent from HzBIS..tblAcessos where idevent = @eventid)    
		insert into HzBIS..tblAcessos values (@eventid, @unidade, @nome, @cpf, @dataacesso, @localacesso, @tiporefeicao, @company, datepart(day, @dataacesso), datepart(month, @dataacesso), datepart(year, @dataacesso), @clientid)    
    
fetch next from o    
into  @eventid, @dataacesso, @tiporefeicao, @clientid, @nome, @company, @localacesso, @cpf, @unidade  
     
end    
    
close o    
deallocate o    