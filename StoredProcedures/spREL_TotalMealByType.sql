create procedure spREL_TotalMeal
    @datei varchar(10),
    @datee varchar(10) ,
    @clientid varchar(500),
    @dateformat varchar(50) = 'dmy'
with
    encryption
as

declare @dateini varchar(50)
declare @dateend varchar(50)
declare @timeini varchar(20)
declare @timeend varchar(20)
declare @sql varchar(5000)
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

--precisão de minutos vem do frontend agora
--set @dateini = @datei + ' 00:00:00'  
--set @dateend = @datee + ' 04:00:59'  

set dateformat @dateFormat
select Dia = (convert(varchar, eventCreationtime, @codigoData) + ' ' +   @codigoHora ), [Desejum], [Almo�o], [Jantar], [Ceia]
from (  
select dia, cmpDcType, total = count(*)
    from HzBIS..tblAcessos
        inner join HzBIS..tblMealType on HzBIS..tblMealType.cmpDcType = HzBIS..tblAcessos.TipoRefeicao
    where data >= @dateini and data <= @dateend and TipoRefeicao <> 'Acesso Regular' and HzBIS..tblAcessos.clientid = @clientid
    group by cmpDcType, dia) sq  
PIVOT (sum(total) FOR cmpDcType in ([Desejum], [Almo�o], [Jantar], [Ceia])) totalmeal 


