USE [hzFortknox]
GO
/****** Object:  StoredProcedure [dbo].[spCreditosPessoa]    Script Date: 02/02/2021 15:28:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spREL_CreditosPessoaMultiArea] 
	@cmpAREAID as varchar(2000), 
	@cmpCredito as int, 
	@DataInicio as date,
	@cmpPERSID as nchar(16)
with encryption
AS
BEGIN
	declare
	@Sql as varchar(4000),
	@Filtro as varchar(4000)
set @Sql='SELECT  
	case when persclass <> ''V'' then [acedb].[bsuser].PERSONS.PERSNO else passportno collate SQL_Latin1_General_CP1_CI_AS end as ''CPF'',      
	[acedb].[bsuser].PERSONS.FIRSTNAME + '' ''+ [acedb].[bsuser].PERSONS.LASTNAME as ''Nome'', 
	hzFortknox.dbo.Creditos.cmpCredito as "Créditos",  
	[acedb].[bsuser].AREAS.NAME as ''Área'', 
	hzFortknox.dbo.Creditos.cmpUltimoAcesso as ''Último Acesso''					 
FROM            hzFortknox.dbo.Creditos 
						 INNER JOIN [acedb].[bsuser].AREAS ON hzFortknox.dbo.Creditos.cmpAREAID = [acedb].[bsuser].AREAS.AREAID 
						 INNER JOIN [acedb].[bsuser].PERSONS ON hzFortknox.dbo.Creditos.cmpPersID = [acedb].[bsuser].PERSONS.PERSID  
						 left outer join [acedb].[bsuser].visitors vis on vis.persid = [acedb].[bsuser].PERSONS.PERSID  '

SET @Filtro=''	

IF @cmpAREAID IS NOT NULL 
SET @Filtro=' hzFortknox.dbo.Creditos.cmpAREAID in (' + @cmpAREAID + ')' 


IF @cmpCredito IS NOT NULL
BEGIN
	IF @Filtro <> ''
	begin
		SET @Filtro =@Filtro + ' AND '
	end
	SET @Filtro = @Filtro + ' hzFortknox.dbo.Creditos.cmpCredito=' + CAST(@cmpCredito As varchar(20))
END

IF @DataInicio IS NOT NULL
BEGIN
	IF @Filtro <> '' SET @Filtro =@Filtro + ' AND '
	SET @Filtro = @Filtro + ' hzFortknox.dbo.Creditos.cmpUltimoAcesso >=''' + convert(varchar, @DataInicio, 103) + ''''
END
IF @cmpPERSID IS NOT NULL
BEGIN
	IF @Filtro <> '' SET @Filtro =@Filtro + ' AND '
	SET @Filtro = @Filtro + ' hzFortknox.dbo.Creditos.cmpPersID =''' + @cmpPERSID + ''''
END



IF @Filtro <> ''
SET @Sql= @Sql + ' WHERE ' + @Filtro



--SELECT @Sql

EXEC (@Sql)

		
END
