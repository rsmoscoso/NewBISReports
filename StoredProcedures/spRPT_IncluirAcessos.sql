USE [BISEventLog]
GO
/****** Object:  StoredProcedure [dbo].[spRPT_IncluirAcessos]    Script Date: 21/05/2020 20:22:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[spRPT_IncluirAcessos]
	@DataInicio datetime,
	@DataFim	datetime
AS

BEGIN
declare 
@IdEvent int

set dateformat dmy


 
set @IdEvent= (Select Max([IDEVENT]) FROM [HzBIS].[dbo].[tblAcessos])
if (@IdEvent is null)
set @IdEvent=0

--select @IdEvent

insert into [HzBIS].[dbo].[tblAcessos] (IDEVENT,Data,EnderecoAcesso,PersID) 
SELECT     LogEvent.ID,   LogEvent.eventCreationTime,LogAddress.AddressTag, LogEventValue.stringValue
FROM            LogEventValue INNER JOIN
                         LogEvent2Value ON LogEventValue.Id = LogEvent2Value.valueId INNER JOIN
                         LogEvent ON LogEvent2Value.eventId = LogEvent.ID INNER JOIN
                         LogEventType ON LogEvent.eventTypeId = LogEventType.ID INNER JOIN
                         LogState ON LogEvent.stateId = LogState.Id INNER JOIN
                         LogAddress ON LogEvent.addressId = LogAddress.ID
WHERE        (LogEventValue.eventTypeId = 21) 
AND (LogEvent.eventCreationTime >= @DataInicio) AND (LogEvent.eventCreationTime <= @DataFim) AND (LogEventType.ID = 1) AND 
                         (LogState.stateNumber = 4101) and LogEvent.ID > @IdEvent
END

--exec [SVRBISCETENCO\BIS_ACE].[hzFortknox].[dbo].spRPTAtualizarAcessos
