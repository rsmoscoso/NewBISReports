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
ALTER PROCEDURE [dbo].[spCreditosPessoa] 
	@cmpAREAID as varchar(16), 
	@cmpCredito as int, 
	@DataInicio as date,
	@cmpPERSID as nchar(16)
AS
BEGIN
	declare
	@Sql as varchar(2000),
	@Filtro as varchar(2000)

	set @Sql='SELECT        hzFortknox.dbo.Creditos.cmpIDCreditos, hzFortknox.dbo.Creditos.cmpPersID, hzFortknox.dbo.Creditos.cmpAREAID, 
	hzFortknox.dbo.Creditos.cmpCredito, hzFortknox.dbo.Creditos.cmpUltimoAcesso, [acedb].[bsuser].AREAS.NAME, 
                         [acedb].[bsuser].PERSONS.PERSNO, [acedb].[bsuser].PERSONS.FIRSTNAME,[acedb].[bsuser].PERSONS.LASTNAME
FROM            hzFortknox.dbo.Creditos INNER JOIN
                         [acedb].[bsuser].AREAS ON hzFortknox.dbo.Creditos.cmpAREAID = [acedb].[bsuser].AREAS.AREAID INNER JOIN
                         [acedb].[bsuser].PERSONS ON hzFortknox.dbo.Creditos.cmpPersID = [acedb].[bsuser].PERSONS.PERSID  '

SET @Filtro=''	

IF @cmpAREAID IS NOT NULL 
SET @Filtro=' hzFortknox.dbo.Creditos.cmpAREAID=''' + @cmpAREAID + ''''


IF @cmpCredito IS NOT NULL
BEGIN
	IF @Filtro <> ''
	begin
		SET @Filtro =@Filtro + ' AND '
	end
	SET @Filtro = @Filtro + ' hzFortknox.dbo.Creditos.cmpCredito=' + CAST(@cmpCredito As varchar(20))
END
select @Filtro as filtro
IF @DataInicio IS NOT NULL
BEGIN
	IF @Filtro <> '' SET @Filtro =@Filtro + ' AND '
	SET @Filtro = @Filtro + ' hzFortknox.dbo.Creditos.cmpUltimoAcesso >=''' + CAST(@DataInicio AS VARCHAR(10)) + ''''
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
