create procedure spREL_TotalMeal
@datei varchar(10), 
@datee varchar(10) , 
@clientid varchar(500)  
with encryption as

declare @dateini varchar(50)  
declare @dateend varchar(50)  
declare @timeini varchar(20)  
declare @timeend varchar(20)  
declare @sql varchar(5000)  
  
set @dateini = @datei + ' 00:00:00'  
set @dateend = @datee + ' 04:00:59'  

set dateformat 'dmy' select dia, [Desejum], [Almoço], [Jantar], [Ceia] from (  
select dia, cmpDcType, total = count(*) from HzBIS..tblAcessos  
inner join HzBIS..tblMealType on HzBIS..tblMealType.cmpDcType = HzBIS..tblAcessos.TipoRefeicao  
where data >= @dateini and data <= @dateend and TipoRefeicao <> 'Acesso Regular' and HzBIS..tblAcessos.clientid = @clientid
group by cmpDcType, dia) sq  
PIVOT (sum(total) FOR cmpDcType in ([Desejum], [Almoço], [Jantar], [Ceia])) totalmeal 


