-- Import Organization Hierarchy From AliantView For Bell

-- Actuall we need to run step 2 first to manually add BellAll as the root.

DECLARE @BellOrganizationId Int
DECLARE @customeridfk int
DECLARE @hgiUserId Int
DECLARE @Prefix varchar(10)
DECLARE @RootNodeCode varchar(10)

--1                              Bell Canada
--4                              Bell Technical Solutions
--23                           Aliant (ignore this)
--43                           Expertech
--180                         Bell Aliant Central
--38							Bell Mobility


SET @BellOrganizationId = 1000110

--SET @customeridfk = 1
--SET @Prefix = ''

--SET @customeridfk = 4
--SET @Prefix = 'BTS_'

--SET @customeridfk = 43
--SET @Prefix = 'BExp_'

SET @customeridfk = 23
SET @Prefix = ''
SET @RootNodeCode = 'BellAliant'

--SET @customeridfk = 38
--SET @Prefix = 'BM_'
--SET @RootNodeCode = 'BellMobili'

SET @hgiUserId = 23190


--BEGIN TRANSACTION;
--BEGIN TRY

	-- Insert to vlfOrganizationHierarchy

	INSERT INTO vlfOrganizationHierarchy (
			OrganizationId
			, NodeCode
			, NodeName
			, IsParent
			, ParentNodeCode
			, [Description]
		)
	SELECT @BellOrganizationId, cc.id, @Prefix + cc.cost_center + ' - ' + ISNULL(cclr.cost_center_desc, ''), 0, 
		case when cc.cost_center_parent is null THEN @RootNodeCode else CONVERT(VARCHAR, cc.cost_center_parent) END as cost_center_parent, cc.cost_center 
	FROM [192.168.9.71].AliantView.dbo.cost_center  cc
		LEFT JOIN [192.168.9.71].AliantView.dbo.cost_center_language_ref cclr ON cc.id = cclr.cost_centeridfk
	WHERE cc.customeridfk = @customeridfk and cc.active=1 and cclr.languageidfk = 1

	-- Insert new fleet record
	DECLARE @fleetId int
	SELECT @fleetId = MAX(FleetId) FROM dbo.vlfFleet

	INSERT INTO dbo.vlfFleet (FleetId, FleetName, OrganizationId, [Description], FleetType, NodeCode) 
	SELECT 
	(ROW_NUMBER() OVER(ORDER BY cc.id DESC) + @fleetId) AS Row
	, @Prefix + cc.cost_center + ' - ' + ISNULL(cclr.cost_center_desc, ''), @BellOrganizationId, @Prefix +cc.cost_center +  '-' + ISNULL(cclr.cost_center_desc, ''), 'oh', cc.id
	FROM [192.168.9.71].AliantView.dbo.cost_center cc
		LEFT JOIN [192.168.9.71].AliantView.dbo.cost_center_language_ref cclr ON cc.id = cclr.cost_centeridfk
	WHERE cc.customeridfk = @customeridfk and cclr.languageidfk = 1
	
	
--END TRY
--BEGIN CATCH
--	print 'error'
--    IF @@TRANCOUNT > 0
--        ROLLBACK TRANSACTION;
--END CATCH;

--IF @@TRANCOUNT > 0
--	COMMIT TRANSACTION;

-- Need to call SP: HierarchyParentSetup
exec HierarchyParentSetup @BellOrganizationId, 0

-- Update Hierarchy Path and flat table
EXEC UpdateOrganizationHierarchyFlat @BellOrganizationId

--SELECT TOP 100 * FROM AliantView.dbo.cost_center
    



