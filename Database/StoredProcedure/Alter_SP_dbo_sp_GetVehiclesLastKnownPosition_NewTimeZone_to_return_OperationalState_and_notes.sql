

USE [SentinelFM]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetVehiclesLastKnownPosition_NewTimeZone]    Script Date: 04/06/2015 1:33:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Alter procedure [dbo].[sp_GetVehiclesLastKnownPosition_NewTimeZone] (@fleetId int,@userId int,@language varchar(50))    
as    
/*    
DECLARE @StartTime datetime    
SET @StartTime = GETDATE()    
declare @userId int, @fleetId int    
set @userId = 962    
set @fleetId = 464    
sp_GetVehiclesLastKnownPosition 19699,13627,'en'    
*/    
      DECLARE @ResolveLandmark int, @Timezone float, @Unit real, @addressNA varchar(32),    
                  @positionExpiredMinutes int, @organizationId int    
      SET @Timezone = ISNULL(dbo.GetTimeZoneDayLight_NewTimeZone(@userId), 0)    
      SET @Unit = ISNULL(dbo.GetRealUserPreference(@userId, 0), 1)    
      SET @positionExpiredMinutes = ISNULL(dbo.GetRealUserPreference(@userId, 16), 43200)  -- by default is one minute    
      SET @ResolveLandmark = ISNULL(dbo.GetRealUserPreference(@userId, 6), 0)    
      SET @addressNA = 'Address resolution in progress'    
     SET @organizationId = (SELECT TOP 1 OrganizationId FROM vlfFleet (NOLOCK) WHERE FleetId = @fleetId)    
      /*            
      -- Insert LIRR Info in evtNotifiaction    
     IF (@organizationId = 1000051)    
    
      BEGIN    
          EXEC SentinelFM.dbo.usp_Driver_Sign_On_Failure_Notification_Insert @organizationId,@fleetId,@positionExpiredMinutes,@Timezone,@language    
      END    
*/    
            SELECT  vlfBox.BoxId,    
                  vlfVehicleInfo.VehicleId,     
                  vlfVehicleAssignment.LicensePlate,     
            CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(minute, (@Timezone *60), vlfBox.LastCommunicatedDateTime) END AS OriginDateTime,    
            ISNULL(vlfBox.LastLatitude, 0) AS Latitude,    
            ISNULL(vlfBox.LastLongitude, 0) AS Longitude,    
            ISNULL(vlfBox.LastSpeed, 0) AS Speed,    
            CASE WHEN vlfBox.LastSpeed IS NULL then 0 ELSE ROUND(vlfBox.LastSpeed * @Unit, 1) END AS CustomSpeed,    
            dbo.ConvertHeading(vlfBox.LastSpeed ,ISNULL(vlfBox.LastHeading, 0)) AS MyHeading,    
        --ISNULL(CASE WHEN @ResolveLandmark = 0 then vlfBox.LastStreetAddress ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then vlfBox.LastStreetAddress ELSE vlfBox.NearestLandmark END END, @addressNA) AS StreetAddress,    
        ISNULL(CASE WHEN @ResolveLandmark = 0 then  case when ltrim(rtrim(vlfBox.LastStreetAddress))='' then @addressNA else  vlfBox.LastStreetAddress end ELSE CASE WHEN vlfBox.NearestLandmark IS NULL then case when ltrim(rtrim(vlfBox.LastStreetAddress))=
  
'' then @addressNA else vlfBox.LastStreetAddress end ELSE vlfBox.NearestLandmark END END, @addressNA) AS StreetAddress,    
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
            'javascript:var w =SensorsInfo(''' + vlfVehicleAssignment.LicensePlate+''')'  as CustomUrl,    
            -1 as ProtocolId,    
            CASE WHEN vlfBox.LastCommunicatedDateTime IS NULL then '' ELSE DATEADD(minute, (@Timezone *60), vlfBox.LastCommunicatedDateTime) END AS LastCommunicatedDateTime,    
            ISNULL(vlfBox.LastSensorMask, 0) AS SensorMask,    
            ISNULL(vlfBox.LSD, '') AS LSD,    
            CASE WHEN CHARINDEX('Tethered',vlfBox.CustomProp2)>0 THEN    
                        ''    
            ELSE    
                  ISNULL(LTRIM(RTRIM(vlfBox.CustomProp2)),'')    
            END AS ExtraInfo    
            --,ISNULL(dbo.vlfDriver.FirstName,'') +' '+ ISNULL(dbo.vlfDriver.LastName,'') as Driver    
            ,CASE WHEN     
                  @organizationId IN (1000051) AND (ISNULL(dbo.VehicleStatus (    
                        @positionExpiredMinutes,    
                        vlfBox.BoxId,     
                        vlfBox.LastSpeed,     
                        vlfVehicleInfo.VehicleTypeId,    
                        vlfBox.LastStatusSensor,    
                        vlfBox.LastCommunicatedDateTime,    
                        vlfBox.LastValidDateTime,    
                        vlfBox.Dormant,@language,vlfbox.LastSensorMask),'')='Moving') AND ISNULL(dbo.vlfDriver.FirstName,'') +' '+ ISNULL(dbo.vlfDriver.LastName,'')=' ' THEN     
                        'DRIVER SIGN ON FAILURE'    
            ELSE ISNULL(dbo.vlfDriver.FirstName,'') +' '+ ISNULL(dbo.vlfDriver.LastName,'') END as Driver    
         --,dbo.GetTripDriver('',vlfVehicleInfo.VehicleId,vlfBox.LastCommunicatedDateTime,dateadd(mi,1, vlfBox.LastCommunicatedDateTime )) as Driver    
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
                
            FROM vlfBox with (nolock) INNER JOIN vlfVehicleAssignment ON vlfBox.BoxId = vlfVehicleAssignment.BoxId     
            INNER JOIN vlfFleetVehicles with (nolock) ON vlfVehicleAssignment.VehicleId = vlfFleetVehicles.VehicleId     
            INNER JOIN vlfFleet with (nolock) ON  vlfFleetVehicles.FleetId = vlfFleet.FleetId    
            INNER JOIN vlfVehicleInfo with (nolock) ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId  -- AND vlfFleetVehicles.VehicleId = vlfVehicleInfo.VehicleId     
            INNER JOIN vlfIconType with (nolock) ON vlfVehicleInfo.IconTypeId = vlfIconType.IconTypeId     
            LEFT JOIN vlfDriverAssignment with (nolock) ON vlfDriverAssignment.VehicleId=vlfVehicleInfo.VehicleId    
            LEFT JOIN vlfDriver with (nolock) ON vlfDriverAssignment.DriverId=vlfDriver.DriverId    
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
                
            ORDER BY vlfVehicleInfo.[Description]    
            --ORDER BY vlfBox.boxid    
    
    
    
GO





