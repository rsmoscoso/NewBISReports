USE [acedb]
GO
/****** Object:  StoredProcedure [dbo].[spRetornarUltimoAcesso]    Script Date: 01/02/2020 12:33:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
 * Procedimento que retorna as informações do último acesso das pessoas.
 *
 * interval - intervalo de tempo em SEGUNDOS. O valor deve ser maior que 0.
 * area - nome das areas separadas por virgula ou null se forem todas

 */
ALTER procedure [dbo].[spRetornarUltimoAcesso]
@interval int = 1,
@Area	varchar(500) = null
with encryption as

declare @sql varchar(5000)

-- Se o valor for nulo ou menor que 0, determina 1 segundo
if (@interval is null or @interval < 0)
	set @interval = 1

set @sql = 'set dateformat ''dmy'' SELECT cur.PERSID AS Id, ISNULL(per.FIRSTNAME, '''') + '' '' + ISNULL(per.LASTNAME, '''') AS Nome, per.PERSNO AS Documento, ''Andar'' AS Andar, are.NAME AS Area, DATEADD(MILLISECOND, DATEDIFF(MILLISECOND, GETUTCDATE(), 
								 GETDATE()), cur.ACCESSTIME) AS DataAcessoLOCAL, cur.ACCESSTIME AS DataAcessoUTC, bsuser.ADDITIONALFIELDDESCRIPTORS.LABEL, bsuser.ADDITIONALFIELDS.VALUE, per.PersClass as TipoPessoa
		FROM            bsuser.CURRENTACCESSSTATE AS cur INNER JOIN
								 bsuser.AREAS AS are ON cur.AREAID = are.AREAID INNER JOIN
								 bsuser.PERSONS AS per ON per.PERSID = cur.PERSID INNER JOIN
								 bsuser.ADDITIONALFIELDS ON per.PERSID = bsuser.ADDITIONALFIELDS.PERSID INNER JOIN
								 bsuser.ADDITIONALFIELDDESCRIPTORS ON bsuser.ADDITIONALFIELDS.FIELDDESCID = bsuser.ADDITIONALFIELDDESCRIPTORS.ID '

set @sql = @sql + ' where ACCESSTIME >= DATEADD(SECOND, ' + convert(varchar, @interval) + ' * (-1), DATEADD(MILLISECOND, DATEDIFF(MILLISECOND,GETDATE(), GETUTCDATE()), GETDATE())) and Label = ''Andar'''

if not @Area is null
	--Modificado por diogo 31/01/2020
	--set @sql = @sql + ' and are.Name in (select data from dbo.split(' + @Area + ', '',''))'
	set @sql = @sql + ' and are.Name in (select data from dbo.split('''+@Area+''', '',''))'
	--Fim  31/01/2020

exec (@sql)
