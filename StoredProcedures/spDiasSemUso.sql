create procedure spREL_RetornaDiasSemUso 
@days smallint,
@name varchar(100) = null
WITH ENCRYPTION AS

declare @sql varchar(5000)

set @sql = 'set dateformat ''dmy'' select Documento = persno, Nome = ISNULL(firstname, '''') + '' '' + ISNULL(lastname, ''''), 
UltimoAceso = convert(varchar, accesstime, 103) + '' '' + 
convert(varchar, accesstime, 108), Empresa = cmp.name, 
Tipo = case when persclass = ''E'' then ''Funcionario'' else ''Visitante'' end,
NCartao = cardno, StatusCartao = case when cd.status = 1 then ''Ativo'' else ''Inativo'' end,
StatusPessoa = case when per.status = 1 then ''Ativo'' else ''Inativo'' end,
Bloqueado = case when lock.persid is null then ''Não Bloqueado'' else lock.causeoflock end,
LimiteBloqueio = case when lock.persid is null then ''Não Bloqueado'' else convert(varchar, lockeduntil, 103) end from bsuser.currentaccessstate access
inner join bsuser.persons per on access.persid = per.persid
inner join bsuser.cards cd on cd.persid = per.persid
left outer join bsuser.lockouts lock on lock.persid = per.persid
left outer join bsuser.companies cmp on cmp.companyid = per.companyid
where datediff(day, accesstime, getdate()) > ' + convert(varchar, @days)

if (not @name is null)
 set @sql = @sql + ' and (cmp.companyno = ''' + @name + ''' or cmp.name = ''' + @name + ''')' 
 
set @sql = @sql + ' order by accesstime desc'

exec (@sql)