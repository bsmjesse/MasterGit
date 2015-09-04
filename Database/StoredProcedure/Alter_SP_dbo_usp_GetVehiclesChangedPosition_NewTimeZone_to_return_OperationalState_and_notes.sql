

USE [SentinelFM]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetVehiclesChangedPosition_NewTimeZone]    Script Date: 04/06/2015 1:46:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER procedure [dbo].[usp_GetVehiclesChangedPosition_NewTimeZone] (@fleetId int,@userId int,@language varchar(50),@lastchecked datetime)  
as  
/*  
DECLARE @StartTime datetime  
SET @StartTime = GETDATE()  
declare @userId int, @fleetId int  
set @userId = 962  
set @fleetId = 464  
  
USE [SentinelFM]  
GO  
  
DECLARE @return_value int  
  
EXEC @return_value = [dbo].[usp_GetVehiclesChangedPosition]  
  @fleetId = 464,  
  @userId = 962,  
  @language = N'en',  
  @lastchecked = N'2011-12-14 09:43:29.000'  
  
SELECT 'Return Value' = @return_value  
  
GO  
  
  
*/  
 DECLARE @ResolveLandmark int, @Timezone float, @Unit real, @addressNA varchar(32),  
   @positionExpiredMinutes int,@organizationId int  
 SET @Timezone = ISNULL(dbo.GetTimeZoneDayLight_NewTimeZone(@userId), 0)  
-- print @Timezone  
 SET @Unit = ISNULL(dbo.GetRealUserPreference(@userId, 0), 1)  
-- print @Unit  
 SET @positionExpiredMinutes = ISNULL(dbo.GetRealUserPreference(@userId, 16), 43200)  -- by default is one minute  
-- print @positionExpiredMinutes  
 SET @ResolveLandmark = ISNULL(dbo.GetRealUserPreference(@userId, 6), 0)  
-- print @ResolveLandmark  
 SET @addressNA = 'Address resolution in progress'  
  
    SET @organizationId = (SELECT  OrganizationId FROM vlfUser (NOLOCK) WHERE userid = @userid)  
  
--DECLARE @orgName int  
--SELECT TOP 1 @orgId=OrganizationId  
--FROM dbo.vlfUser WITH(NOLOCK)  
--INNER JOIN  
--  
--WHERE  UserId=@userId  
  
 SELECT  vlfBox.BoxId,  
   vlfVehicleInfo.VehicleId,   
   vlfVehicleAssignment.LicensePlate,   
  CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(minute, (@Timezone * 60), vlfBox.LastCommunicatedDateTime) END AS OriginDateTime,  
  ISNULL(vlfBox.LastLatitude, 0) AS Latitude,  
  ISNULL(vlfBox.LastLongitude, 0) AS Longitude,  
  ISNULL(vlfBox.LastSpeed, 0) AS Speed,  
  CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit, 1) END AS CustomSpeed,  
  dbo.ConvertHeading(vlfBox.LastSpeed ,ISNULL(vlfBox.LastHeading, 0)) AS MyHeading,  
        --ISNULL(CASE WHEN @ResolveLandmark = 0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END, @addressNA) AS StreetAddress,  
        ISNULL(CASE WHEN @ResolveLandmark = 0 then  case when vlfBox.LastStreetAddress='' then @addressNA else  vlfBox.LastStreetAddress end ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then case when vlfBox.LastStreetAddress='' then @addressNA else vlfBox.LastStreetAddress end ELSE vlfBox.NearestLandmark END END, @addressNA) AS StreetAddress,  
    
----  CASE WHEN vlfBox.LastStreetAddress='' then  
----   @addressNA  
----  ELSE   
----   ISNULL(vlfBox.LastStreetAddress,  
----    CASE WHEN @ResolveLandmark <> 0 then   
----     CASE WHEN vlfBox.NearestLandmark IS NULL then   
----       vlfBox.LastStreetAddress  
----     ELSE   
----      vlfBox.NearestLandmark   
----     END  
----    ELSE  
----     @addressNA  
----    END)  
----  END AS StreetAddress,  
  ISNULL(vlfVehicleInfo.[Description], '') AS [Description],  
  CASE WHEN vlfBox.BoxArmed = 0 then 'false' ELSE 'true' END AS BoxArmed,  
  IconTypeName,  
     case when @organizationId IN (18) then  
     ISNULL(dbo.udf_AssetStatus (  
                        vlfBox.BoxId,   
      @positionExpiredMinutes,  
      @language),'')   
   else  
  
    ISNULL(dbo.VehicleStatus (  
                        @positionExpiredMinutes,  
                        vlfBox.BoxId,   
                        vlfBox.LastSpeed,   
                        vlfVehicleInfo.VehicleTypeId,  
                        vlfBox.LastStatusSensor,  
                        vlfBox.LastCommunicatedDateTime,  
                        vlfBox.LastValidDateTime,  
                        vlfBox.Dormant,@language,vlfbox.LastSensorMask),'') end as VehicleStatus,  
        'false' as chkBoxShow,  
  'false' as Updated,  
  'javascript:var w =SensorsInfo(''' + vlfVehicleAssignment.LicensePlate+''')' as CustomUrl,  
  -1 as ProtocolId,  
  CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(minute, (@Timezone * 60), vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime,  
  ISNULL(vlfBox.LastSensorMask, 0) AS SensorMask,  
  ISNULL(vlfBox.LSD, '') AS LSD,  
  CASE WHEN CHARINDEX('Tethered',vlfBox.CustomProp2)>0 THEN  
    ''  
  ELSE  
    ISNULL(LTRIM(RTRIM(vlfBox.CustomProp2)),'')  
  END AS ExtraInfo  
    --,dbo.GetTripDriver('',vlfVehicleInfo.VehicleId,vlfBox.LastCommunicatedDateTime,dateadd(mi,1, vlfBox.LastCommunicatedDateTime )) as Driver  
  ,ISNULL(dbo.vlfDriver.FirstName,'') +' '+ ISNULL(dbo.vlfDriver.LastName,'') as Driver  
  ,ISNULL(vlfVehicleInfo.ImagePath, '') AS ImagePath  
  ,ISNULL(va.num, 0) AS ConfiguredNum  
  ,ISNULL(vlfDriver.KeyFobId,'') as DriverCardNumber  
  --,dbo.GetTripDriverKeyFobId('',vlfVehicleInfo.VehicleId,vlfBox.LastCommunicatedDateTime,dateadd(mi,1, vlfBox.LastCommunicatedDateTime )) as DriverCardNumber  
     ,ISNULL(vlfVehicleInfo.Field1,'') as Field1  
  ,ISNULL(vlfVehicleInfo.Field2,'') as Field2  
  ,ISNULL(vlfVehicleInfo.Field3,'') as Field3  
  ,ISNULL(vlfVehicleInfo.Field4,'') as Field4  
  ,ISNULL(vlfVehicleInfo.Field5,'') as Field5  
  ,ModelYear  
  ,MakeName  
  ,ModelName  
  ,VehicleTypeName  
  ,VinNum  
  ,ManagerName as ManagerName  
  ,ManagerId as ManagerEmployeeId  
  ,StateProvince  
  ,ISNULL(Color,'') as Color  
  ,ISNULL(LastEngineHour,0) as EngineHours  
  ,ISNULL(BoxOdometer,0)* @Unit as Odometer  
  , CASE WHEN vlfBox.OrganizationID= 1000047 THEN dbo.udb_GetLastPreTripState(vlfBox.BoxId) ELSE NULL END AS PretripResult  
  , CASE WHEN vlfBox.OrganizationID= 1000047 THEN dbo.[udb_GetLastReeferAlarmExist](vlfBox.BoxId) ELSE NULL END AS ReeferLastAlarm  
  , LastIgnOnBatV  
  , LastIgnOffBatV  
  , LastBatV  
  , IsNull(vlfVehicleInfo.OperationalState, 100) As OperationalState 
  , IsNull(vlfVehicleInfo.Notes,'') AS OperationalStateNotes
--, ISNULL(LTRIM(RTRIM(vlfBox.CustomProp2)),'') AS ExtraInfo  
--  vlfVehicleInfo.IconTypeId,   
--  CASE WHEN vlfBox.LastStatusDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.LastStatusDateTime) END AS LastStatusDateTime,  
--  GeoFenceEnabled,  
--  ISNULL(LastStatusSensor, -1) AS LastStatusSensor,  
--  ISNULL(LastStatusSpeed, -1) AS LastStatusSpeed,  
--        CASE WHEN vlfBox.PrevStatusDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.PrevStatusDateTime) END AS PrevStatusDateTime,  
--  ISNULL(PrevStatusSensor, -1) AS PrevStatusSensor,  
--  ISNULL(PrevStatusSpeed, -1) AS PrevStatusSpeed,  
--  vlfFirmware.FwTypeId,  
--  ISNULL(Dormant, 0) AS Dormant  
--        CASE WHEN vlfBox.DormantDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.DormantDateTime) END AS DormantDateTime,  
--  ISNULL(vlfVehicleType.VehicleTypeName, '') AS VehicleTypeName,  
--  CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime,  
--      vlfVehicleType.VehicleTypeId  
    
  FROM vlfBox with(nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId   
  INNER JOIN vlfFleetVehicles with(nolock) ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId   
  INNER JOIN vlfFleet with(nolock) ON  vlfFleetVehicles.FleetId = vlfFleet.FleetId  
  INNER JOIN vlfVehicleInfo with(nolock) ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId  -- AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId   
  INNER JOIN vlfIconType with(nolock) ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId   
  LEFT JOIN vlfDriverAssignment with (nolock) ON vlfDriverAssignment.VehicleId=vlfVehicleInfo.VehicleId  
  LEFT JOIN vlfDriver with (nolock) ON vlfDriverAssignment.DriverId=vlfDriver.DriverId  
--  INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId   
--  INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId   
--  INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId    
  LEFT JOIN (SELECT a.ObjectID, count(a.ObjectID) as num  
     FROM vlfServiceAssignments a WITH(NOLOCK)   
     INNER JOIN vlfServiceConfigurations c WITH(NOLOCK) ON  
     a.ServiceConfigID = c.ServiceConfigID  
     WHERE c.ServiceID = 4 AND a.Objects = 'Vehicle' AND a.Deleted=0  
     GROUP BY a.ObjectID  
    ) va ON va.ObjectID = vlfVehicleInfo.VehicleId   
  
  LEFT JOIN dbo.vlfMakeModel on vlfVehicleInfo.MakeModelId=dbo.vlfMakeModel.MakeModelId  
  LEFT JOIN dbo.vlfMake on vlfMake.MakeId=dbo.vlfMakeModel.MakeId  
  LEFT JOIN dbo.vlfModel on vlfModel.ModelId=dbo.vlfMakeModel.ModelId  
  LEFT JOIN vlfVehicleType ON vlfVehicleType.VehicleTypeId=vlfVehicleInfo.VehicleTypeId  
  WHERE vlfFleet.FleetId = @fleetId  
  AND   
  (vlfBox.LastCommunicatedDateTime>=@lastchecked  
   OR vlfBox.Modified_Datetime>=@lastchecked  
  --vlfBox.LastValidDateTime IS NULL or   
  --DATEADD(hour, @Timezone, vlfBox.LastCommunicatedDateTime)>=@lastchecked  
  --DATEDIFF(ss,DATEADD(hour, @Timezone, vlfBox.LastCommunicatedDateTime),DATEADD(hour, @Timezone,@lastchecked))>=0  
  --DATEDIFF(ss,LastCommunicatedDateTime,@lastchecked)>=0  
    
  )  
  ORDER BY 4 DESC  
--vlfVehicleInfo.[Description]  
  
------DECLARE @UpdatedData TABLE   
------(  
------BoxId int,  
------lastValidDateTime datetime  
------)  
------  
------INSERT INTO @UpdatedData  
------SELECT vlfMsgInHst.BoxId,Max(vlfMsgInHst.DateTimeReceived) As lastValidDateTime  
--------vlfVehicleInfo.VehicleId,vlfVehicleAssignment.LicensePlate,  
--------,ISNULL(vlfVehicleInfo.[Description], '') AS [Description],  
--------CASE WHEN vlfMsgInHst.IsArmed = 0 then 'false' ELSE 'true' END AS BoxArmed,  
--------'javascript:var w =SensorsInfo(''' + vlfVehicleAssignment.LicensePlate+''')' as CustomUrl,  
--------ISNULL(vlfMsgInHst.LSD, '') AS LSD,vlfVehicleInfo.VehicleTypeId,vlfIconType.IconTypeName,vlfBox.Dormant  
------FROM vlfMsgInHst with(nolock)  
------INNER JOIN vlfVehicleAssignment with(nolock)  
------ON vlfMsgInHst.BoxId = vlfVehicleAssignment.BoxId  
------INNER JOIN vlfFleetVehicles with(nolock)   
------ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId   
------INNER JOIN vlfFleet with(nolock)   
------ON  vlfFleetVehicles.FleetId = vlfFleet.FleetId  
--------INNER JOIN vlfBox with(nolock)  
--------ON  vlfMsgInHst.BoxId=vlfBox.BoxId  
---------INNER JOIN vlfVehicleInfo with(nolock)   
--------ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId  -- AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId   
--------INNER JOIN vlfIconType with(nolock)  
--------ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId   
--------INNER JOIN vlfBox with(nolock)  
--------ON vlfBox.BoxId=vlfMsgInHst.BoxId  
------WHERE vlfFleet.FleetId = @fleetId  
------AND vlfMsgInHst.DateTimeReceived IS NOT NULL  
------  AND   
------  (  
------   DATEDIFF(ss,@lastchecked,vlfMsgInHst.DateTimeReceived) >0  
------  --DATEDIFF(ss,DATEADD(hour, @Timezone, vlfMsgInHst.DateTimeReceived),@lastchecked) >0  
------  --vlfBox.LastValidDateTime IS NULL or   
------  --DATEADD(hour, @Timezone, vlfMsgInHst.DateTimeReceived)>=@lastchecked  
------  )  
------GROUP BY vlfMsgInHst.BoxId  
------  
------  
------SELECT DISTINCT vlfMsgInHst.BoxId,  
------   vlfVehicleInfo.VehicleId,   
------   vlfVehicleAssignment.LicensePlate,   
------  CASE WHEN a.LastValidDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, a.LastValidDateTime) END AS OriginDateTime,  
------  ISNULL(vlfMsgInHst.Latitude, 0) AS Latitude,  
------  ISNULL(vlfMsgInHst.Longitude, 0) AS Longitude,  
------  ISNULL(vlfMsgInHst.Speed, 0) AS Speed,  
------  CASE WHEN vlfMsgInHst.Speed IS NULL then 0 ELSE ROUND(vlfMsgInHst.Speed * @Unit, 1) END AS CustomSpeed,  
------  dbo.ConvertHeading(vlfMsgInHst.Speed ,ISNULL(vlfMsgInHst.Heading, 0)) AS MyHeading,  
------        --ISNULL(CASE WHEN @ResolveLandmark = 0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END, @addressNA) AS StreetAddress,  
------        ISNULL(vlfMsgInHst.StreetAddress, @addressNA) AS StreetAddress,  
------  ISNULL(vlfVehicleInfo.[Description], '') AS [Description],  
------  CASE WHEN vlfMsgInHst.IsArmed = 0 then 'false' ELSE 'true' END AS BoxArmed,  
------  IconTypeName,  
------   dbo.VehicleStatus (  
------    @positionExpiredMinutes,  
------    vlfMsgInHst.BoxId,   
------    vlfMsgInHst.Speed,   
------    vlfVehicleInfo.VehicleTypeId,  
------    vlfMsgInHst.SensorMask,  
------    vlfMsgInHst.DateTimeReceived,  
------    a.lastValidDateTime,  
------    vlfBox.Dormant,@language,vlfbox.LastSensorMask) as VehicleStatus,  
------        'false' as chkBoxShow,  
------  'false' as Updated,  
------  'javascript:var w =SensorsInfo(''' + vlfVehicleAssignment.LicensePlate+''')' as CustomUrl,  
------  -1 as ProtocolId,  
------  CASE WHEN vlfMsgInHst.DateTimeReceived IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfMsgInHst.DateTimeReceived) END AS LastCommunicatedDateTime,  
------  ISNULL(vlfMsgInHst.SensorMask, 0) AS SensorMask,  
------  ISNULL(vlfMsgInHst.LSD, '') AS LSD  
--------  vlfVehicleInfo.IconTypeId,   
--------  CASE WHEN vlfBox.LastStatusDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.LastStatusDateTime) END AS LastStatusDateTime,  
--------  GeoFenceEnabled,  
--------  ISNULL(LastStatusSensor, -1) AS LastStatusSensor,  
--------  ISNULL(LastStatusSpeed, -1) AS LastStatusSpeed,  
--------        CASE WHEN vlfBox.PrevStatusDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.PrevStatusDateTime) END AS PrevStatusDateTime,  
--------  ISNULL(PrevStatusSensor, -1) AS PrevStatusSensor,  
--------  ISNULL(PrevStatusSpeed, -1) AS PrevStatusSpeed,  
--------  vlfFirmware.FwTypeId,  
--------  ISNULL(Dormant, 0) AS Dormant  
--------        CASE WHEN vlfBox.DormantDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.DormantDateTime) END AS DormantDateTime,  
--------  ISNULL(vlfVehicleType.VehicleTypeName, '') AS VehicleTypeName,  
--------  CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(hour, @Timezone, vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime,  
--------      vlfVehicleType.VehicleTypeId  
------    
------  FROM   
------@UpdatedData as a  
------INNER JOIN  
------dbo.vlfMsgInHst WITH(NOLOCK)  
------ON a.BoxId=vlfMsgInHst.BoxId  
------AND a.lastValidDateTime=vlfMsgInHst.DateTimeReceived  
------INNER JOIN  
------vlfBox with(nolock)  
------ON vlfBox.BoxId=a.BoxID  
------ INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId   
------  INNER JOIN vlfFleetVehicles with(nolock) ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId   
------  INNER JOIN vlfFleet with(nolock) ON  vlfFleetVehicles.FleetId = vlfFleet.FleetId  
------  INNER JOIN vlfVehicleInfo with(nolock) ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId  -- AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId   
------  INNER JOIN vlfIconType with(nolock) ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId   
--------  INNER JOIN vlfVehicleType ON vlfVehicleInfo.VehicleTypeId = vlfVehicleType.VehicleTypeId   
--------  INNER JOIN vlfFirmwareChannels ON vlfBox.FwChId = vlfFirmwareChannels.FwChId   
--------  INNER JOIN vlfFirmware ON vlfFirmwareChannels.FwId = vlfFirmware.FwId     
------  WHERE vlfFleet.FleetId = @fleetId  
------    
------  ORDER BY vlfVehicleInfo.[Description]  
  
  
  
  
  
GO



