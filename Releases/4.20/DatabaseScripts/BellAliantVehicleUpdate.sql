DECLARE @BellOrganizationId Int
SET @BellOrganizationId = 1000110

DECLARE @ID int
DECLARE @VehicleDescription Varchar(100)
DECLARE @Field1 Varchar(100)
DECLARE @Field2 Varchar(100)
DECLARE @MakeModelId INT
DECLARE @VehicleType varchar(100)
DECLARE @VehicleTypeId smallint
DECLARE @VinNum varchar(40)
DECLARE @VinNumTemp varchar(40)
DECLARE @ModelYear smallint
DECLARE @VehicleWeight int
DECLARE @Color varchar(20)
DECLARE @ManagerName varchar(100)
DECLARE @NodeCode varchar(100)
DECLARE @VehicleId bigint
DECLARE @Bell_Vehicle_Id bigint
DECLARE @Bell_Vehicle_vehicleid varchar
DECLARE @Prefix varchar(10)
DECLARE @FleetId int
DECLARE @customeridfk INT
DECLARE @LicensePlate varchar(50)
DECLARE @Manufacturer varchar(100)
DECLARE @MakeId INT
DECLARE @ModelNumber varchar(100)
DECLARE @ModelId INT

DECLARE @FuelType varchar(100)
DECLARE @FuelTypeId INT

DECLARE @CountFoundSentinelFM INT;
DECLARE @CountFoundBell INT;

DECLARE @LogMsg Varchar(500)
SET @LogMsg = ''

DECLARE @ParentNodeCode Varchar(100)
DECLARE @NodeName Varchar(200)
DECLARE @ParentId INT


SET @MakeModelId = 0;
SET @VehicleTypeId = 1;
SET @Color = '';
SET @CountFoundSentinelFM = 1;

--SET @VehicleId = 49736
--SET @VehicleId = 44589

DECLARE vehicle_Cursor CURSOR FOR
               select VehicleId from vlfVehicleInfo 
               WHERE OrganizationId = @BellOrganizationId
OPEN vehicle_Cursor;
FETCH NEXT FROM vehicle_Cursor into @VehicleId
WHILE @@FETCH_STATUS = 0
   BEGIN

      SET @Bell_Vehicle_Id = NULL;
      SET @VehicleDescription = NULL;
      SET @VinNum = NULL;
      SET @LogMsg = ''
      SET @CountFoundSentinelFM = 1;
      SET @CountFoundBell = 1;
      SET @Bell_Vehicle_vehicleid = '';
      
      -- Match by field1 and field2
      SELECT @Field1 = ltrim(rtrim(Field1)), @Field2 = ltrim(rtrim(Field2)), @VehicleDescription = Description, @VinNum = ltrim(rtrim(VinNum))
      FROM vlfVehicleInfo WHERE OrganizationId=@BellOrganizationId AND VehicleId = @VehicleId
      IF @Field1 IS NOT NULL AND @Field1 <> '' AND @Field2 IS NOT NULL AND @Field2 <> ''
      BEGIN
         SELECT @CountFoundSentinelFM = COUNT(VehicleId) FROM vlfVehicleInfo 
         WHERE OrganizationId = @BellOrganizationId --AND Field1 = @Field1 AND Field2 = @Field2;
                        AND Description = @Field1
         IF @CountFoundSentinelFM = 1
         BEGIN
            SELECT @Bell_Vehicle_Id = id FROM [192.168.9.71].AliantView.dbo.Vehicle 
            WHERE  vehicleid = @Field1 AND CAST(customeridfk AS varchar(10)) = @Field2
         END
      END
      
      -- Match by vin num
      IF @Bell_Vehicle_Id IS NULL AND @VinNum IS NOT NULL AND @VinNum <> ''
      BEGIN
         SELECT @CountFoundBell = COUNT(id) FROM [192.168.9.71].AliantView.dbo.Vehicle WHERE serial_no = @VinNum
         IF @CountFoundBell = 1
         BEGIN
            SELECT @Bell_Vehicle_Id = id FROM [192.168.9.71].AliantView.dbo.Vehicle 
            WHERE  serial_no = @VinNum
         END
      END

      -- Last try, match by vehicle Description - vehicleid without customer code, like 051723
      IF @Bell_Vehicle_Id IS NULL
      BEGIN
         IF @Bell_Vehicle_vehicleid  IS NULL OR @Bell_Vehicle_vehicleid = ''
         BEGIN
            SELECT @CountFoundSentinelFM = COUNT(VehicleId) FROM vlfVehicleInfo 
            WHERE OrganizationId = @BellOrganizationId AND Description = @VehicleDescription;
            SELECT @CountFoundBell = COUNT(id) FROM [192.168.9.71].AliantView.dbo.Vehicle WHERE vehicleid = @VehicleDescription

            IF @CountFoundSentinelFM = 1 AND @CountFoundBell = 1
            BEGIN
               SELECT @Bell_Vehicle_Id = id FROM [192.168.9.71].AliantView.dbo.Vehicle 
               WHERE  vehicleid = @VehicleDescription
            END
         END
         ELSE
         BEGIN   -- if its description is format: BTS051723 and didn't find it, we'll remvoe BTS and try to match by 051723 only.
            SELECT @CountFoundSentinelFM = COUNT(VehicleId) FROM vlfVehicleInfo 
            WHERE OrganizationId = @BellOrganizationId AND Description = @Bell_Vehicle_vehicleid;
            SELECT @CountFoundBell = COUNT(id) FROM [192.168.9.71].AliantView.dbo.Vehicle WHERE vehicleid = @Bell_Vehicle_vehicleid

            IF @CountFoundSentinelFM = 1 AND @CountFoundBell = 1
            BEGIN
               SELECT @Bell_Vehicle_Id = id FROM [192.168.9.71].AliantView.dbo.Vehicle 
               WHERE  vehicleid = @Bell_Vehicle_vehicleid
            END
         END
      END

      -- We gave up, could match the vehicle in Bell
      

      -- If we found duplicated vehicle in SentinelFM, we'll do nothing and log it. because we don't know which vehciel we should match.
      IF @CountFoundSentinelFM > 1
      BEGIN
         SET @LogMsg = 'Duplicated Vehicle Found In SentinelFM, SentinelFM VehicleId: ' + CAST(@VehicleId AS varchar) + ', Description: ' + @VehicleDescription;             
         INSERT INTO dbo.vlfVehicleUploadLog (UserId, OrganizationId, Description, OriginDT)
         VALUES( 0, @BellOrganizationId, @LogMsg, GETDATE());
      END
      -- If we found duplicated vehicle in Bell, we'll do nothing and log it. because we don't know which vehciel we should match.
      ELSE IF @CountFoundBell > 1
      BEGIN
         SET @LogMsg = 'Duplicated Vehicle Found In Bell Aliant, SentinelFM VehicleId: ' + CAST(@VehicleId AS varchar) + ', Description: ' + @VehicleDescription;       
         INSERT INTO dbo.vlfVehicleUploadLog (UserId, OrganizationId, Description, OriginDT)
         VALUES( 0, @BellOrganizationId, @LogMsg, GETDATE());
      END
      ELSE IF @Bell_Vehicle_Id IS NULL
      BEGIN
         SET @LogMsg = 'No Vehicle Found In Bell Aliant, SentinelFM VehicleId: ' + CAST(@VehicleId AS varchar) + ', Description: ' + @VehicleDescription;                      
         INSERT INTO dbo.vlfVehicleUploadLog (UserId, OrganizationId, Description, OriginDT)
         VALUES( 0, @BellOrganizationId, @LogMsg, GETDATE());
      END

      
      -- Finally we'll do something
      IF @LogMsg = ''
      BEGIN
                     
         SELECT @VinNum = v.serial_no
               , @ModelYear = vehmodel_year 
               --, @VehicleDescription = CAST(v.vehicleid AS varchar(10)) + '_' + CAST(v.customeridfk AS varchar(10))
               --, @VehicleDescription = 
               --            CASE WHEN v.customeridfk= 1 THEN 'BEL' + v.vehicleid
               --                           WHEN v.customeridfk= 4 THEN 'BTS' + v.vehicleid
               --                           WHEN v.customeridfk= 43 THEN 'EXP' + v.vehicleid
               --                           WHEN v.customeridfk= 180 THEN 'BAC' + v.vehicleid
               --                           ELSE NULL
               --            END       
               , @VehicleDescription = v.vehicleid           
               , @Field1 = v.vehicleid
               , @Field2 = CAST(v.customeridfk AS varchar(10))
               , @ManagerName = v.supervisor_name
               , @NodeCode = CAST(v.cost_centeridfk AS varchar(100))
               , @VehicleWeight = 
                      case when v.gvwr >= v.gcwr AND v.gvwr >= v.curbweight then v.gvwr
                                     when v.gcwr >= v.gvwr AND v.gcwr >= v.curbweight then v.gcwr
                                     else v.curbweight
                      end
               , @FuelType = 
                      CASE WHEN fuel_type_refidfk = 1 THEN 'Compress Natural Gas'
							 WHEN fuel_type_refidfk = 2 THEN 'Diesel'
							 WHEN fuel_type_refidfk = 3 THEN 'Electric'
							 WHEN fuel_type_refidfk = 4 THEN 'Propane'
							 WHEN fuel_type_refidfk = 5 THEN 'Unleaded'
							 WHEN fuel_type_refidfk = 6 THEN 'Unleaded/Electric'
							 WHEN fuel_type_refidfk = 7 THEN 'Miscellaneous'
							 WHEN fuel_type_refidfk = 8 THEN 'Composite'
							 WHEN fuel_type_refidfk = 9 THEN 'Aviation Gas'
							 WHEN fuel_type_refidfk = 10 THEN 'Jet Fuel'
							 WHEN fuel_type_refidfk = 11 THEN 'N/A'
							 ELSE NULL
                      END
               , @Color = 
                      CASE WHEN color_refidfk = 1 THEN 'BEIGE'
                             WHEN color_refidfk = 2 THEN 'BLACK'
                             WHEN color_refidfk = 3 THEN 'BLUE'
                             WHEN color_refidfk = 4 THEN 'BROWN'
                             WHEN color_refidfk = 5 THEN 'BURGUNDY'
                             WHEN color_refidfk = 6 THEN 'Gold'
                             WHEN color_refidfk = 7 THEN 'GREEN'
                             WHEN color_refidfk = 8 THEN 'GREY'
                             WHEN color_refidfk = 9 THEN 'GREY/BLUE'
                             WHEN color_refidfk = 10 THEN 'LIGHT BLUE'
                             WHEN color_refidfk = 11 THEN 'n/a'
                             WHEN color_refidfk = 12 THEN 'orange'
                             WHEN color_refidfk = 13 THEN 'Purple'
                             WHEN color_refidfk = 14 THEN 'red'
                             WHEN color_refidfk = 15 THEN 'sauge'
                             WHEN color_refidfk = 16 THEN 'SILVER'
                             WHEN color_refidfk = 17 THEN 'Tan'
                             WHEN color_refidfk = 18 THEN 'teal'
                             WHEN color_refidfk = 19 THEN 'TITANIUM'
                             WHEN color_refidfk = 20 THEN 'violet'
                             WHEN color_refidfk = 21 THEN 'WHITE'
                             WHEN color_refidfk = 22 THEN 'YELLOW'
                             --WHEN color_refidfk = 23 THEN ''
                             --WHEN color_refidfk = 24 THEN ''
                             WHEN color_refidfk = 25 THEN 'Aluminium'
                             WHEN color_refidfk = 26 THEN 'PEARL'
                             WHEN color_refidfk = 27 THEN 'METAL'
                             ELSE ''
                      END
               , @LicensePlate = v.license_plate_no
               , @Manufacturer = mlr.manufacturer_desc
               , @ModelNumber = vmr.[vehmodel_desc]
               , @VehicleType = 
                      CASE
							 WHEN vm.vehtype_refidfk = 1 THEN 'C01A Cars & Sport Utility Vehicles'
							 WHEN vm.vehtype_refidfk = 2 THEN 'C02A Passenger'
							 WHEN vm.vehtype_refidfk = 3 THEN 'C02B Work Application'
							 WHEN vm.vehtype_refidfk = 4 THEN 'C03A Passenger'
							 WHEN vm.vehtype_refidfk = 5 THEN 'C03B Work Application Compact'
							 WHEN vm.vehtype_refidfk = 6 THEN 'C03C Work Application Heavy'
							 WHEN vm.vehtype_refidfk = 7 THEN 'C04A Parcel delivery vans'
							 WHEN vm.vehtype_refidfk = 8 THEN 'C05A Step vans'
							 WHEN vm.vehtype_refidfk = 9 THEN 'C06A Regular Drive'
							 WHEN vm.vehtype_refidfk = 10 THEN 'C06B 4x4'
							 WHEN vm.vehtype_refidfk = 11 THEN 'C07A Regular Cab'
							 WHEN vm.vehtype_refidfk = 12 THEN 'C07B Extended Cab'
							 WHEN vm.vehtype_refidfk = 13 THEN 'C07C Cab & Chassis'
							 WHEN vm.vehtype_refidfk = 14 THEN 'C08A Line Trucks'
							 WHEN vm.vehtype_refidfk = 15 THEN 'C08B Diggers'
							 WHEN vm.vehtype_refidfk = 16 THEN 'C08C Aerial Devices'
							 WHEN vm.vehtype_refidfk = 17 THEN 'C09A Highway tractors'
							 WHEN vm.vehtype_refidfk = 18 THEN 'C10A Construction Trailers'
							 WHEN vm.vehtype_refidfk = 19 THEN 'C11A'
							 WHEN vm.vehtype_refidfk = 20 THEN 'C12A Trailers'
							 WHEN vm.vehtype_refidfk = 21 THEN 'C12B Semi-trailers'
							 WHEN vm.vehtype_refidfk = 22 THEN 'C13A Equipment Trailers'
							 WHEN vm.vehtype_refidfk = 23 THEN 'C14A Trenchers, Plows & Loaders'
							 WHEN vm.vehtype_refidfk = 24 THEN 'C15A All Terrain Vehicles'
							 WHEN vm.vehtype_refidfk = 25 THEN 'C15B Snowmobiles'
							 WHEN vm.vehtype_refidfk = 26 THEN 'C16A Fork Lifts'
							 WHEN vm.vehtype_refidfk = 27 THEN 'C04B Aerial Device'
							 WHEN vm.vehtype_refidfk = 28 THEN 'C07D Aerial Device'
							 WHEN vm.vehtype_refidfk = 29 THEN 'C07E Crew cab'
							 WHEN vm.vehtype_refidfk = 30 THEN 'C03D Aerial Device'
							 WHEN vm.vehtype_refidfk = 31 THEN 'N/A'                                                                                                         
                      END
         FROM [192.168.9.71].AliantView.dbo.Vehicle v
                inner join [192.168.9.71].AliantView.dbo.vehmodel_ref vm on vm.id=v.vehmodel_refidfk
                inner join [192.168.9.71].AliantView.dbo.[vehmodel_language_ref] vmr on vm.id=vmr.vehmodel_refidfk
                inner join [192.168.9.71].AliantView.dbo.manufacturer_language_ref mlr on vm.manufacturer_refidfk=mlr.manufacturer_refidfk
         WHERE v.id = @Bell_Vehicle_Id
                and  vmr.languageidfk=1
                and mlr.languageidfk=1
      END

      IF EXISTS (SELECT * FROM vlfVehicleInfo WHERE VinNum=@VinNum AND VehicleId <> @VehicleId)
      BEGIN
         SET @LogMsg = 'VinNum Already Exists. SentinelFM VehicleId: ' + CAST(@VehicleId AS varchar) + ', Description: ' + @VehicleDescription + ', VinNum:' + @VinNum
         INSERT INTO dbo.vlfVehicleUploadLog (UserId, OrganizationId, Description, OriginDT)
         VALUES( 0, @BellOrganizationId, @LogMsg, GETDATE());
      END

      IF @LogMsg = ''
      BEGIN

         IF @VinNum IS NULL
         BEGIN
            SELECT @VinNum = VinNum FROM vlfVehicleInfo WHERE VehicleId = @VehicleId
         END

         IF @VehicleDescription IS NULL
         BEGIN
            SELECT @VehicleDescription = Description FROM vlfVehicleInfo WHERE VehicleId = @VehicleId
         END

         -- Get OR Update Fuel Type
         SET @FuelTypeId = NULL
         IF (@FuelType <> '' AND @FuelType IS NOT NULL)
         BEGIN
            SELECT @FuelTypeId = FuelTypeID FROM vlfFuelCategory WHERE OrganizationID=@BellOrganizationId AND FuelType=@FuelType
            IF (@FuelTypeId IS NULL OR @FuelTypeId=0)
            BEGIN
              INSERT INTO vlfFuelCategory (OrganizationID, FuelType, GHGCategory, GHGCategoryDesc, CO2Factor)
               VALUES(@BellOrganizationId, @FuelType, '0', '0', 0)

               SELECT @FuelTypeId = FuelTypeID FROM vlfFuelCategory WHERE OrganizationID=@BellOrganizationId AND FuelType=@FuelType
            END
         END

         -- Get Or Update Make
         SET @MakeId = 0
         IF (@Manufacturer <> '')
         BEGIN
            SELECT @MakeId = MakeId FROM vlfMake WHERE MakeName = @Manufacturer
            IF (@MakeId IS NULL OR @MakeId=0)
            BEGIN
               SELECT @MakeId = MAX(MakeId) + 1 FROM vlfMake --WHERE MakeId <> 999
               INSERT INTO vlfMake (MakeId, MakeName)
               VALUES(@MakeId, @Manufacturer)                                                                     
            END
         END

         -- Get OR Update Model
         SET @ModelId = 0
         IF (@ModelNumber <> '')
         BEGIN
            SELECT @ModelId = ModelId FROM vlfModel WHERE ModelName = @ModelNumber
            IF (@ModelId IS NULL OR @ModelId=0)
            BEGIN
               SELECT @ModelId = MAX(ModelId) + 1 FROM vlfModel
               INSERT INTO vlfModel (ModelId, ModelName)
               VALUES(@ModelId, @ModelNumber)                                                                  
            END
         END

         -- GET OR SET MakeModel
         SET @MakeModelId = 0
         IF (@ModelId > 0 AND @MakeId > 0)
         BEGIN
            SELECT @MakeModelId = MakeModelId FROM vlfMakeModel WHERE MakeId = @MakeId AND ModelId = @ModelId
            IF (@MakeModelId IS NULL OR @MakeModelId=0)
            BEGIN                                                               
               INSERT INTO vlfMakeModel (MakeId, ModelId, FuelFactor)
               VALUES(@MakeId, @ModelId, 1)
               SELECT @MakeModelId = MakeModelId FROM vlfMakeModel WHERE MakeId = @MakeId AND ModelId = @ModelId
            END
         END

         -- GET OR SET VehicleType
         SET @VehicleTypeId = 0
         IF (@VehicleType <> '')
         BEGIN
            SELECT @VehicleTypeId = VehicleTypeId FROM vlfVehicleType WHERE VehicleTypeName = @VehicleType
            IF (@VehicleTypeId IS NULL OR @VehicleTypeId=0)
            BEGIN
               SELECT @VehicleTypeId = MAX(VehicleTypeId) + 1 FROM vlfVehicleType
               INSERT INTO vlfVehicleType (VehicleTypeId, VehicleTypeName)
               VALUES(@VehicleTypeId, @VehicleType)                                                             
            END
         END
         
         
         UPDATE vlfVehicleInfo SET Description = @VehicleDescription
               , Field1 = @Field1, Field2 = @Field2, ManagerName = @ManagerName, ModelYear = @ModelYear, VinNum = @VinNum, VehicleWeight = @VehicleWeight
               , FuelTypeId = @FuelTypeId, Color = @Color, MakeModelId = @MakeModelId, VehicleTypeId = @VehicleTypeId
         WHERE OrganizationId = @BellOrganizationId AND VehicleId = @VehicleId

         IF @LicensePlate IS NULL
         BEGIN
            SET @LogMsg = 'No License Plate found in Bell Aliant. SentinelFM VehicleId: ' + CAST(@VehicleId AS varchar) + ', Description: ' + @VehicleDescription
            INSERT INTO dbo.vlfVehicleUploadLog (UserId, OrganizationId, Description, OriginDT)
            VALUES( 0, @BellOrganizationId, @LogMsg, GETDATE());
         END
         ELSE IF EXISTS( SELECT * FROM vlfVehicleAssignment WHERE LicensePlate = @LicensePlate AND VehicleId <> @VehicleId)
         BEGIN
            SET @LogMsg = 'Existing License Plate found in SentinelFM. SentinelFM VehicleId: ' + CAST(@VehicleId AS varchar) + ', Description: ' + @VehicleDescription
            INSERT INTO dbo.vlfVehicleUploadLog (UserId, OrganizationId, Description, OriginDT)
            VALUES( 0, @BellOrganizationId, @LogMsg, GETDATE());
         END
         ELSE
         BEGIN
            UPDATE vlfVehicleAssignment SET LicensePlate = @LicensePlate
            WHERE VehicleId = @VehicleId
         END

                              ---- If we could not find the nodecode in Sentinelfm, we will insert it to SentinelFM
                              IF NOT EXISTS (
                                             SELECT ID FROM vlfOrganizationHierarchy WHERE NodeCode = @NodeCode AND OrganizationId = @BellOrganizationId
                              )
                              BEGIN
                                             SET @Prefix = '';
                  /*CASE WHEN @customeridfk = 1 THEN ''
						  WHEN @customeridfk = 4 THEN 'BTS_'
						  WHEN @customeridfk = 43 THEN 'BExp_'
						  WHEN @customeridfk = 180 THEN 'BellAC_' 
						  WHEN @customeridfk = 38 THEN 'BM_' 						  
						  ELSE ''
                  END*/

                                             SET @ParentNodeCode = '';
                                             SET @NodeName = '';
                                             SELECT @ParentNodeCode = cc.cost_center_parent, @NodeName = @Prefix + cc.cost_center + ' - ' + ISNULL(cclr.cost_center_desc, '')
                                             FROM [192.168.9.71].AliantView.dbo.cost_center cc
												LEFT JOIN [192.168.9.71].AliantView.dbo.cost_center_language_ref cclr ON cc.id = cclr.cost_centeridfk
                                             WHERE cc.id = @NodeCode and cclr.languageidfk = 1

                                             SET @ParentId = NULL;
                                             SELECT @ParentId = ID FROM vlfOrganizationHierarchy WHERE OrganizationId = @BellOrganizationId AND NodeCode = @ParentNodeCode

                                             IF (@ParentId IS NOT NULL AND @ParentId <> 0 
                                                                           AND NOT EXISTS (SELECT * FROM vlfOrganizationHierarchy WHERE NodeName=@NodeName AND OrganizationId=@BellOrganizationId)
                                                                           AND NOT EXISTS (SELECT * FROM vlfFleet WHERE FleetName = @NodeName AND OrganizationId = @BellOrganizationId)
                                             )
                                             BEGIN
                                                            EXEC [dbo].OrganizationHierarchyAddNodeCode @BellOrganizationId,@NodeCode,@NodeName,@ParentNodeCode,'',@ParentId,0
                                             END
                              END
                              -----------------------------------------------------------------------------
         

         -- Bell's vehicle only assigned to one cost center

         IF NOT EXISTS (
                  SELECT ID FROM vlfOrganizationHierarchyAssignment 
                  WHERE OrganizationId=@BellOrganizationId
                        AND NodeCode = @NodeCode AND ObjectTableName = 'vlfVehicleInfo_VehicleId' AND ObjectId = CAST(@VehicleId AS varchar(50))
                        AND ( EffectiveTo IS NULL OR EffectiveTo > GETDATE())
         ) AND @VehicleId > 0
         BEGIN
            --print 'vehicle hierarchy fleet assignment';
            --print 'OrganizationId:' + cast(@BellOrganizationId as varchar(20)) + '; nodecode:' + @NodeCode + '; VehicleId:' + CAST(@VehicleId AS varchar(50));
            --print 'vinNum: ' + @VinNum + '; vinNumTemp:' + @VinNumTemp

            /*SELECT * FROM vlfOrganizationHierarchyAssignment 
                                          WHERE OrganizationId=@BellOrganizationId
                                                                        AND NodeCode = @NodeCode AND ObjectTableName = 'vlfVehicleInfo_VehicleId' AND ObjectId = CAST(@VehicleId AS varchar(50))
                                                                        AND ( EffectiveTo IS NULL OR EffectiveTo > GETDATE())
            */
            -- If this vehicle assigned to another cost center we set it expired                                

            INSERT INTO vlfOrganizationHierarchyAssignmentHistory (OrganizationId, NodeCode, ObjectTableName, ObjectId, EffectiveFrom, EffectiveTo)
            SELECT OrganizationId, NodeCode, ObjectTableName, ObjectId, EffectiveFrom, GETDATE()
            FROM vlfOrganizationHierarchyAssignment
            WHERE OrganizationId=@BellOrganizationId AND ( EffectiveTo IS NULL  OR EffectiveTo > GETDATE())
                           AND ObjectTableName = 'vlfVehicleInfo_VehicleId' AND ObjectId = CAST(@VehicleId AS varchar(50))

            DELETE vlfOrganizationHierarchyAssignment
            WHERE OrganizationId=@BellOrganizationId AND ( EffectiveTo IS NULL  OR EffectiveTo > GETDATE())
                           AND ObjectTableName = 'vlfVehicleInfo_VehicleId' AND ObjectId = CAST(@VehicleId AS varchar(50))

            INSERT INTO vlfOrganizationHierarchyAssignment (
                           OrganizationId
                           , NodeCode
                           , ObjectTableName
                           , ObjectId
                           , EffectiveFrom                 
            )
            VALUES (
                           @BellOrganizationId,
                           @NodeCode,
                           'vlfVehicleInfo_VehicleId',
                           CAST(@VehicleId AS varchar(50)),
                           GETDATE()
            )
                        
         END

                              IF EXISTS (SELECT * FROM vlfOrganizationHierarchy WHERE NodeCode=@NodeCode AND OrganizationId = @BellOrganizationId)
                              BEGIN
                                             -- delete old vehicle-fleet assignment
                                             DELETE FROM vlfFleetVehicles 
                                              WHERE FleetId IN ( SELECT FleetId FROM vlfFleet WHERE OrganizationId=@BellOrganizationId AND FleetType='oh') 
                                                                                                           AND VehicleId = @VehicleId

                                             -- Assign the vehicle to all the parent fleets
                        
                                              /*IF NOT EXISTS ( SELECT * FROM vlfFleetVehicles WHERE FleetId=@FleetId AND VehicleId = @VehicleId)
                                                                                                         INSERT INTO vlfFleetVehicles (FleetId, VehicleId) VALUES(@FleetId, @VehicleId)
                                             */
                                             INSERT INTO vlfFleetVehicles (FleetId, VehicleId)
                                             SELECT f.FleetId, @VehicleId 
                                              FROM vlfFleet f
                                                            INNER JOIN vlfOrganizationHierarchy oh ON f.NodeCode = oh.NodeCode
                                                            INNER JOIN vlfOrganizationHierarchyFlat ohf ON oh.NodeCode = ohf.ParentNodeCode
                                             WHERE f.OrganizationId = @BellOrganizationId AND oh.OrganizationId = @BellOrganizationId AND ohf.OrganizationId = @BellOrganizationId
                                                               AND ohf.ChildNodeCode = @NodeCode
                                                               AND f.FleetId NOT IN (SELECT FleetId FROM vlfFleetVehicles WHERE VehicleId=@VehicleId) 
                               END   
                     
      END
      FETCH NEXT FROM vehicle_Cursor into @VehicleId
   END
CLOSE vehicle_Cursor;
DEALLOCATE vehicle_Cursor;
