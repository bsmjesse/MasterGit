USE [SentinelFM]
GO

/****** Object:  StoredProcedure [dbo].[GetOrganizationSensor ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[GetOrganizationSensor] (@organizationid int)
as
Begin
SELECT distinct SensorName FROM vlfBoxSensorsCfg WHERE BoxId in (SELECT  [BoxId]
FROM [SentinelFM].[dbo].[vlfBox]
where [OrganizationId]=@organizationid) and SensorName Not Like '%Unused%' and SensorName Not Like '%Unsued%'and  SensorName Not Like '%Unse%' and SensorName Not Like '%nused%' and SensorName Not Like '%Unsued%'and SensorName Not Like '%Unuse%' and SensorName Not Like '%Not Used%' 
End
GO