--- Manually add root node for Bell
USE SentinelFM
DECLARE @BellOrganizationId Int
DECLARE @hgiUserId Int
DECLARE @NodeCode varchar(10)
DECLARE @NodeName varchar(100)

SET @BellOrganizationId = 1000110
SET @hgiUserId = 23190
--SET @NodeCode = 'BellAll'
--SET @NodeName = 'Bell (All)'

--SET @NodeCode = 'BellCanada'
--SET @NodeName = 'Bell Canada'

--SET @NodeCode = 'BellTS'
--SET @NodeName = 'Bell Technical Solutions'

--SET @NodeCode = 'BellExpert'
--SET @NodeName = 'Customer Expertech'

SET @NodeCode = 'BellAliant'
SET @NodeName = 'Bell Aliant'

--SET @NodeCode = 'BellMobili'
--SET @NodeName = 'Bell Mobility'

BEGIN TRANSACTION;
BEGIN TRY
	DECLARE @fleetId int
	SELECT @fleetId = fleetId FROM vlfFleet WHERE OrganizationId=@BellOrganizationId AND NodeCode = @NodeCode AND FleetType='oh'
	DELETE vlfFleetUsers WHERE FleetId=@fleetId AND UserId=@hgiUserId
	
	DELETE vlfFleetVehicles WHERE FleetId IN (
		SELECT FleetId FROM vlfFleet WHERE FleetType='oh' AND OrganizationId=@BellOrganizationId
	)

	DELETE vlfFleet WHERE FleetType='oh' AND OrganizationId=@BellOrganizationId;
	DELETE vlfOrganizationHierarchy WHERE OrganizationId=@BellOrganizationId;

	INSERT INTO vlfOrganizationHierarchy (
			OrganizationId
			, NodeCode
			, NodeName
			, IsParent
			, ParentNodeCode
			, [Description]
		)
	VALUES( @BellOrganizationId, @NodeCode, @NodeName, 1, NULL, @NodeName)

	-- Insert new fleet record
	SELECT @fleetId = MAX(FleetId) + 1 FROM dbo.vlfFleet

	INSERT INTO dbo.vlfFleet (FleetId, FleetName, OrganizationId, [Description], FleetType, NodeCode) 
	VALUES( @fleetId, @NodeName, @BellOrganizationId, @NodeName, 'oh', @NodeCode)

	-- Assign Fleet To Hgi User
	IF @NodeCode = 'BellAliant'
	BEGIN
		SELECT @fleetId = fleetId FROM vlfFleet WHERE OrganizationId=@BellOrganizationId AND FleetType='oh' AND NodeCode=@NodeCode
		IF Not exists (SELECT * FROM vlfFleetUsers WHERE FleetId=@fleetId AND UserId=@hgiUserId)
			INSERT INTO vlfFleetUsers (FleetId, UserId) 
			VALUES(@fleetId, @hgiUserId)
	END
	

END TRY
BEGIN CATCH
	print 'error'
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION;
    