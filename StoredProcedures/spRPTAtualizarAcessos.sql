USE [hzFortknox]
GO
/****** Object:  StoredProcedure [dbo].[spRPTAtualizarAcessos]    Script Date: 21/05/2020 20:58:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[spRPTAtualizarAcessos] 
	
AS
BEGIN
	

UPDATE [SVRBISCETENCO\BIS].[hzBIS].[dbo].[tblAcessos]
SET [ClientID] = dbo.fnHZ_GetClientID([tblAcessos].EnderecoAcesso), 
    [Divisao] = dbo.fnHZ_GetClientNome([tblAcessos].EnderecoAcesso),
	DEVICEID= dbo.fnHZ_GetDeviceID([tblAcessos].EnderecoAcesso),
		LocalAcesso = dbo.fnHZ_GetClientNome(tblAcessos.EnderecoAcesso)
FROM (
    SELECT [IDEVENT], [EnderecoAcesso]
    FROM  [SVRBISCETENCO\BIS].[HzBIS].[dbo].[tblAcessos] where [ClientID] is null) as Acesso
WHERE 
    Acesso.[IDEVENT] = [tblAcessos].[IDEVENT] and   ( tblAcessos.nome is null)

UPDATE [SVRBISCETENCO\BIS].[HzBIS].[dbo].[tblAcessos]
SET [TipoEmpregado] = Pessoa.DISPLAYTEXT, 
    NOME = Pessoa.nome,
	CPF=Pessoa.PERSNO,
	[Empresa]=Pessoa.COMPANYNO

FROM (
SELECT acedb.bsuser.PERSCLASSES.DISPLAYTEXT, acedb.bsuser.PERSONS.FIRSTNAME + ' ' + acedb.bsuser.PERSONS.LASTNAME AS nome, acedb.bsuser.PERSONS.PERSNO, acedb.bsuser.PERSONS.PERSID, acedb.bsuser.COMPANIES.COMPANYNO
FROM    acedb.bsuser.PERSONS INNER JOIN
                  acedb.bsuser.PERSCLASSES ON acedb.bsuser.PERSONS.PERSCLASSID = acedb.bsuser.PERSCLASSES.PERSCLASSID LEFT OUTER JOIN
                  acedb.bsuser.COMPANIES ON acedb.bsuser.PERSONS.COMPANYID = acedb.bsuser.COMPANIES.COMPANYID) as Pessoa
where Pessoa.PERSID=[tblAcessos].PERSID  and   ( tblAcessos.nome is null)


--DELETE FROM [172.16.0.98\BIS].[HzBIS].[dbo].[tblAcessos]
END
