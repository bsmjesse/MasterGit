USE [SentinelFM]
GO

/****** Object:  StoredProcedure [dbo].[GetOrganizationSensor] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[GetOrganizationSensorPreference] (@organizationid int)
as
Begin
SELECT distinct SensorName
FROM [SentinelFM].[dbo].[vlfOrganizationSensorPreference]
where [OrganizationId]=@organizationid
End
GO