GO

CREATE TABLE [dbo].[vlfFleetUserGroup](
	[FleetId] [int] NOT NULL,
	[UserGroupId] [int] NOT NULL,
 CONSTRAINT [PK_vlfFleetUserGroup] PRIMARY KEY CLUSTERED 
(
	[FleetId] ASC,
	[UserGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: November 2013
-- Description:	Adds User Group settings
-- =============================================
ALTER PROCEDURE [dbo].[usp_UserGroupSettings_Add]
	@CheckboxValuesParams varchar(8000),
	@CheckboxReportsValuesParams varchar(8000) = '',
	@CheckboxCommandsValuesParams varchar(8000) = '',
	@FleetIDs varchar(8000) = '',
	@OperationType int, 
	@UserGroupName varchar(255),
	@OrganizationId int,
	@ParentUserGroupId int,
	@UserId int,
	@UserGroupId smallint OUTPUT
AS

BEGIN TRANSACTION

DECLARE @SecurityLevel smallint, @OperationTypeControls int, @OperationTypeReports int, @OperationTypeCommands int, @OldParentUserGroupId int

SET @SecurityLevel = (SELECT SecurityLevel FROM vlfUserGroup WHERE UserGroupId = @ParentUserGroupId)
--set new UserGroupId 
SET @UserGroupId = (SELECT MAX(UserGroupId) FROM vlfUserGroup) + 1
SET @OperationTypeControls = 3 -- Gui controls
SET @OperationTypeReports = 4  -- Reports
SET @OperationTypeCommands = 2  -- Commands

SET @OldParentUserGroupId = (SELECT ParentUserGroupId FROM vlfUserGroup WHERE UserGroupId = @ParentUserGroupId)
IF @OldParentUserGroupId IS NOT NULL
	SET @ParentUserGroupId = @OldParentUserGroupId
	
--add new User Group
INSERT INTO vlfUserGroup(UserGroupId, UserGroupName, SecurityLevel, OrganizationId, ParentUserGroupId)
VALUES(@UserGroupId, @UserGroupName, @SecurityLevel, @OrganizationId, @ParentUserGroupId)  

--add group control settings
IF @CheckboxValuesParams <> ''
BEGIN
	INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
	SELECT @UserGroupId, SplitValue, @OperationTypeControls 
	FROM Split_Max(@CheckboxValuesParams, ';')
END

--add group report settings
IF @CheckboxReportsValuesParams <> ''
BEGIN
	INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
	SELECT @UserGroupId, SplitValue, @OperationTypeReports 
	FROM Split_Max(@CheckboxReportsValuesParams, ';')
END

--add group command settings
IF @CheckboxCommandsValuesParams <> ''
BEGIN
	INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
	SELECT @UserGroupId, SplitValue, @OperationTypeCommands 
	FROM Split_Max(@CheckboxCommandsValuesParams, ';')
END
BEGIN
	--add from parent group 2 - Command
	INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
	SELECT @UserGroupId, OperationId, OperationType 
	FROM vlfGroupSecurity 
	WHERE UserGroupId = 2 AND OperationType = @OperationTypeCommands --@ParentUserGroupId
END

--add from parent group 0 - System, 1 - Output, 5 - ???, 6 - WebMethod
INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
SELECT @UserGroupId, OperationId, OperationType 
FROM vlfGroupSecurity 
WHERE UserGroupId = 2 AND OperationType IN (0, 1, 5, 6) --@ParentUserGroupId

--add Fleet/UserGroup settings
IF @FleetIDs <> ''
BEGIN
	INSERT INTO vlfFleetUserGroup(FleetId, UserGroupId)
	SELECT SplitValue, @UserGroupId 
	FROM Split_Max(@FleetIDs, ';')
	WHERE SplitValue > 0
END

IF @@ERROR = 0
	COMMIT TRANSACTION
ELSE
	ROLLBACK TRANSACTION
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: November 2013
-- Description:	Updates User Group settings
-- =============================================
ALTER PROCEDURE [dbo].[usp_UserGroupSettings_Update]
	@UserGroupId int,
	@CheckboxValuesParams varchar(8000),
	@CheckboxReportsValuesParams varchar(8000) = '',
	@CheckboxCommandsValuesParams varchar(8000) = '',
	@FleetIDs varchar(8000) = '',
	@OperationType int, -- 1-output, 2-command, 3-Gui controls, 4-reports
	@UserId int,
	@UserGroupName varchar(255) = ''	
AS

DECLARE @ParentUserGroupId int, @OperationTypeControls int, @OperationTypeReports int, @OperationTypeCommands int
DECLARE @tmp_UsergroupFleets TABLE
(FleetId int)
DECLARE @tmp_UsergroupOldFleets TABLE
(FleetId int)
DECLARE @tmp_UsersAssignedToUsergroup TABLE
(UserId int)

SET @ParentUserGroupId = (SELECT ParentUserGroupId FROM vlfUserGroup WITH(NOLOCK) WHERE UserGroupId = @UserGroupId)
SET @OperationTypeControls = 3 -- Gui controls
SET @OperationTypeReports = 4  -- Reports
SET @OperationTypeCommands = 2  -- Commands

BEGIN TRANSACTION

INSERT INTO @tmp_UsergroupOldFleets (FleetId)
SELECT FleetId 
FROM vlfFleetUserGroup
WHERE UserGroupId = @UserGroupId

-- get Users assigned to selected UserGroup
INSERT INTO @tmp_UsersAssignedToUsergroup (UserId)
SELECT UserId FROM vlfUserGroupAssignment
WHERE UserGroupId = @UserGroupId
		
--modification of base group is not allowed
--IF @ParentUserGroupId IS NOT NULL
--BEGIN
	--delete group settings
	DELETE FROM vlfGroupSecurity
	WHERE UserGroupId=@UserGroupId 
		AND OperationType IN (@OperationTypeControls, @OperationTypeReports, @OperationTypeCommands)
	
	--update group control settings
	IF @CheckboxValuesParams <> ''
	BEGIN
		INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
		SELECT @UserGroupId, SplitValue, @OperationTypeControls 
		FROM Split_Max(@CheckboxValuesParams, ';')
	END
	
	--update group report settings
	IF @CheckboxReportsValuesParams <> ''
	BEGIN
		INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
		SELECT @UserGroupId, SplitValue, @OperationTypeReports 
		FROM Split_Max(@CheckboxReportsValuesParams, ';')
	END
	
	--add group command settings
	IF @CheckboxCommandsValuesParams <> ''
	BEGIN		
		INSERT INTO vlfGroupSecurity(UserGroupId, OperationId, OperationType)
		SELECT @UserGroupId, SplitValue, @OperationTypeCommands 
		FROM Split_Max(@CheckboxCommandsValuesParams, ';')
	END

	IF @UserGroupName <> ''
	BEGIN
		UPDATE vlfUserGroup
		SET UserGroupName = @UserGroupName
		WHERE UserGroupId = @UserGroupId
	END
	
	--delete Fleet/UserGroup settings first
	DELETE FROM vlfFleetUserGroup
	WHERE UserGroupId=@UserGroupId 
	
	-- remove Fleets from User that belongs to removed UserGroups
	IF EXISTS (SELECT * FROM @tmp_UsergroupOldFleets) AND EXISTS (SELECT * FROM @tmp_UsersAssignedToUsergroup) 
	BEGIN
		DELETE FROM vlfFleetUsers
		WHERE FleetId IN (SELECT FleetId FROM @tmp_UsergroupOldFleets)
			AND UserId IN (SELECT UserId FROM @tmp_UsersAssignedToUsergroup)
			AND UserId NOT IN (SELECT vlfFleetUsers.UserId FROM vlfFleetUsers INNER JOIN
									vlfUserGroupAssignment ON vlfUserGroupAssignment.UserId = vlfFleetUsers.UserId INNER JOIN
									vlfFleetUserGroup ON vlfFleetUserGroup.UserGroupId = vlfUserGroupAssignment.UserGroupId
								WHERE vlfFleetUserGroup.UserGroupId <> @UserGroupId and vlfFleetUserGroup.FleetId in (SELECT FleetId FROM @tmp_UsergroupOldFleets))  
	END
	
	--add Fleet/UserGroup settings
	IF @FleetIDs <> ''
	BEGIN
		-- get Fleets for selected UserGroup		
		INSERT INTO @tmp_UsergroupFleets (FleetId)
		SELECT SplitValue 
		FROM Split_Max(@FleetIDs, ';')
		WHERE SplitValue > 0
		
		-- add Fleets for selected UserGroups
		IF EXISTS (SELECT * FROM @tmp_UsergroupFleets) AND EXISTS (SELECT * FROM @tmp_UsersAssignedToUsergroup) 
		BEGIN
			INSERT INTO vlfFleetUsers (UserId, FleetId)
			SELECT u.UserId, t.FleetId
			FROM @tmp_UsergroupFleets t CROSS JOIN
				@tmp_UsersAssignedToUsergroup u LEFT OUTER JOIN
				vlfFleetUsers f ON t.fleetid=f.fleetid AND f.userid=u.UserId
			WHERE f.fleetid IS NULL AND t.fleetid IS NOT NULL 
		END
	
		INSERT INTO vlfFleetUserGroup(FleetId, UserGroupId)
		SELECT SplitValue, @UserGroupId 
		FROM Split_Max(@FleetIDs, ';')
		WHERE SplitValue > 0
	END
--END

IF @@ERROR = 0
	COMMIT TRANSACTION
ELSE
	ROLLBACK TRANSACTION
	
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: July 2014
-- Description:	Gets Fleets assigned to User Group
-- =============================================
CREATE PROCEDURE [dbo].[usp_FleetsByUserGroup_Get]
	@UserGroupId int,
	@FleetType varchar(50)
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT vlfFleetUserGroup.FleetId,
		vlfFleet.NodeCode,
		vlfFleet.FleetName
FROM vlfFleetUserGroup with(nolock) INNER JOIN
	vlfFleet ON vlfFleet.FleetId = vlfFleetUserGroup.FleetId
WHERE vlfFleetUserGroup.UserGroupId = @UserGroupId
		AND ISNULL(FleetType, '') = @FleetType
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: February 2014
-- Description:	Updates User Group assignment
-- =============================================
ALTER PROCEDURE [dbo].[usp_UserGroupAssignmnet_Update]
	@UserId int,
	@UsergroupsParams varchar(8000)	
AS

BEGIN TRANSACTION

DECLARE @tmp_SelectedUserGroups TABLE
(UserGropId int)

DECLARE @tmp_DeletedUserGroups TABLE
(UserGropId int)

DECLARE @tmp_DefaultFleets TABLE
(FleetId int)

DECLARE @tmp_FleetsToDelete TABLE
(FleetId int)

-- get selected UserGroups
INSERT INTO @tmp_SelectedUserGroups (UserGropId)
SELECT  SplitValue
FROM Split_Max(@UsergroupsParams, ';')
WHERE SplitValue > 0

-- get UserGroups that were removed from User
INSERT INTO @tmp_DeletedUserGroups (UserGropId)
SELECT UserGroupId 
FROM vlfUserGroupAssignment 
WHERE UserId = @UserId 
		AND UserGroupId NOT IN (SELECT UserGropId FROM @tmp_SelectedUserGroups)

-- get Fleets for selected UserGroups		
INSERT INTO @tmp_DefaultFleets (FleetId)
SELECT DISTINCT FUG.FleetId 
FROM vlfFleetUserGroup FUG INNER JOIN
	@tmp_SelectedUserGroups SG ON SG.UserGropId = FUG.UserGroupId

-- get Fleets for removed UserGroups
INSERT INTO @tmp_FleetsToDelete (FleetId)
SELECT DISTINCT FUG.FleetId 
FROM vlfFleetUserGroup FUG INNER JOIN
	@tmp_DeletedUserGroups DG ON DG.UserGropId = FUG.UserGroupId
		
--delete user group assignments
DELETE FROM vlfUserGroupAssignment
WHERE UserId=@UserId 

--add user group assignments
IF @UsergroupsParams <> ''
BEGIN
	INSERT INTO vlfUserGroupAssignment(UserId, UserGroupId)
	SELECT @UserId, SplitValue
	FROM Split_Max(@UsergroupsParams, ';')
	WHERE SplitValue > 0
END

-- remove Fleets from User that belongs to removed UserGroups
DELETE FROM vlfFleetUsers
WHERE UserId = @UserId 
		AND FleetId IN (SELECT FleetId FROM @tmp_FleetsToDelete)

-- add Fleets for selected UserGroups
IF EXISTS (SELECT * FROM @tmp_DefaultFleets) 
BEGIN
	INSERT INTO vlfFleetUsers (UserId, FleetId)
	SELECT @UserId, t.FleetId
	FROM @tmp_DefaultFleets t
	LEFT OUTER JOIN vlfFleetUsers f
	ON t.fleetid=f.fleetid AND f.userid=@UserId
	WHERE f.fleetid IS NULL AND t.fleetid IS NOT NULL
END

IF @@ERROR = 0
	COMMIT TRANSACTION
ELSE
	ROLLBACK TRANSACTION
	
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: July 2014
-- Description:	Gets Fleets by OrganizationId and FleetType
-- =============================================
CREATE PROCEDURE [dbo].[usp_FleetsInfoByOrganizationIdAndType_Get]
	@OrganizationId int,
	@FleetType varchar(50) = ''
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT vlfFleet.FleetId, 
		vlfFleet.FleetName, 
		vlfFleet.[Description]
FROM vlfFleet with(nolock)
WHERE vlfFleet.OrganizationId = @OrganizationId 
		AND ISNULL(FleetType, '') = @FleetType
ORDER BY vlfFleet.FleetName
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: July 2014
-- Description:	Deletes User to UserGroup assignment
-- =============================================
CREATE PROCEDURE [dbo].[usp_UserGroupUserAssignment_Delete]
	@UserGroupId smallint,
	@UserId int
AS

BEGIN TRANSACTION
/*
DELETE FROM vlfFleetUsers
WHERE UserId = @UserId 
		AND FleetId IN (SELECT FleetId FROM vlfFleetUserGroup WHERE UserGroupID = @UserGroupId)
		AND FleetId NOT IN (SELECT FleetId FROM vlfFleetUserGroup WHERE UserGroupID <> @UserGroupId)
*/
		
DELETE FROM vlfUserGroupAssignment
WHERE UserGroupId = @UserGroupId
	AND UserId = @UserId
				
IF @@ERROR = 0
	COMMIT TRANSACTION
ELSE
	ROLLBACK TRANSACTION
GO
ALTER TABLE vlfOrganizationServices ADD SendEmail bit not null default 1
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: May 2014
-- Description:	Gets organization services
-- =============================================
ALTER PROCEDURE [dbo].[usp_OrganizationService_Add]
	@OrganizationID int,
	@ServiceID int,
	@UserID int,
	@SendEmail bit = 1,
	@OrganizationServiceID int OUTPUT
AS

DECLARE @TDate datetime, @FleetID int, @ServiceConfigID int
SET @TDate = GETDATE()

BEGIN TRANSACTION

SET @FleetID = (SELECT TOP 1 FleetId FROM vlfFleet WITH(NOLOCK) 
					WHERE (FleetName='All Vehicles' OR FleetName='Tous les véhicules' OR FleetName='Tous les vehicules')
						AND OrganizationId=@OrganizationID)

IF NOT EXISTS(SELECT * FROM vlfOrganizationServices WITH(NOLOCK) 
				WHERE OrganizationID=@OrganizationID AND ServiceID=@ServiceID)
BEGIN
	INSERT INTO vlfOrganizationServices (OrganizationID, ServiceID, [Status], SendEmail)
	VALUES (@OrganizationID, @ServiceID, 1, @SendEmail)

	SET @OrganizationServiceID = SCOPE_IDENTITY()
	
	INSERT INTO vlfOrganizationServiceHistory (OrganizationServiceID, ServiceStartDate, UserIDEnableService)
	VALUES (@OrganizationServiceID, @TDate, @UserID)
END
ELSE
BEGIN
	SET @OrganizationServiceID = (SELECT OrganizationServiceID
									FROM vlfOrganizationServices
									WHERE ServiceID = @ServiceID
										AND OrganizationID = @OrganizationID)
	IF @OrganizationServiceID IS NOT NULL
	BEGIN
		UPDATE vlfOrganizationServices
		SET [Status] = 1,
			SendEmail = @SendEmail
		WHERE OrganizationServiceID = @OrganizationServiceID
		
		IF NOT EXISTS(SELECT * FROM vlfOrganizationServiceHistory 
						WHERE OrganizationServiceID = @OrganizationServiceID AND ServiceEndDate IS NULL)
		BEGIN
			INSERT INTO vlfOrganizationServiceHistory (OrganizationServiceID, ServiceStartDate, UserIDEnableService)
			VALUES (@OrganizationServiceID, @TDate, @UserID)
		END
	END
	
END

IF @ServiceID = 2 AND @FleetID IS NOT NULL
BEGIN
	IF NOT EXISTS(SELECT * FROM vlfServiceConfigurations WITH(NOLOCK) 
					WHERE ServiceID = @ServiceID
						--AND ServiceConfigName = 'SpeedOver10'
						AND OrganizationID = @OrganizationID)
	BEGIN
		INSERT INTO vlfServiceConfigurations (ServiceID, ServiceConfigName, OrganizationID, IsActive, RulesApplied, CreatedDate, UserID)
		VALUES (@ServiceID, 'SpeedOver10', @OrganizationID, 1, 'PostSpeed = 1;OverSpeed > 10;Metric = 3;', @TDate, @UserID) 
		
		SET @ServiceConfigID = SCOPE_IDENTITY()
		
		INSERT INTO vlfServiceAssignments (ServiceConfigID, [Objects], ObjectID, Stdate, Inclusive, Deleted)
		VALUES (@ServiceConfigID, 'Fleet', @FleetID, @TDate, 1, 0)
	END
	--ELSE
	--BEGIN
	--	UPDATE vlfServiceConfigurations
	--	SET IsActive = 1,
	--		UserID = @UserID
	--	WHERE ServiceID = @ServiceID
	--		AND ServiceConfigName = 'SpeedOver10'
	--		AND OrganizationID = @OrganizationID
	--END
END

IF @@ERROR = 0
BEGIN
	COMMIT TRANSACTION
END
ELSE
BEGIN
	ROLLBACK TRANSACTION
	SET @OrganizationServiceID = 0
END
GO
-- =============================================
-- Author:		Gene Pachkevitch
-- Create date: November 2013
-- Description:	Gets User Groups allowed to view by current User
-- =============================================
ALTER PROCEDURE [dbo].[usp_UserGroupsByUser_Get]
	@UserId int,
	@AllOrganizationGroups bit = 1
AS

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

DECLARE @OrganizationId int
DECLARE @UserSecurityLevel int, @NewSecurityModel varchar(50)
                               
SET @OrganizationId = (SELECT OrganizationId FROM vlfUser WITH(NOLOCK) WHERE UserId=@UserId)
SET @UserSecurityLevel = (SELECT MIN(vlfUserGroup.SecurityLevel) 
							FROM vlfUserGroup WITH(NOLOCK) INNER JOIN
								vlfUserGroupAssignment WITH(NOLOCK) ON vlfUserGroupAssignment.UserGroupId = vlfUserGroup.UserGroupId INNER JOIN 
								vlfUser WITH(NOLOCK) ON vlfUser.UserId = vlfUserGroupAssignment.UserId
							WHERE vlfUser.UserId=@UserId) 

SET @NewSecurityModel = (SELECT TOP 1 PreferenceValue   
	FROM vlfOrganizationSettings with(nolock)  
	WHERE OrgPreferenceId = 25 AND OrganizationId = @OrganizationId)  
IF @NewSecurityModel IS NULL  
	SET @NewSecurityModel = 'false'
	
IF @AllOrganizationGroups = 1
BEGIN
	--system groups that assigned to organization users
	SELECT DISTINCT vlfUserGroup.UserGroupId,
			vlfUserGroup.UserGroupName
	FROM vlfUserGroup WITH(NOLOCK) INNER JOIN
		vlfUserGroupAssignment WITH(NOLOCK) ON vlfUserGroupAssignment.UserGroupId = vlfUserGroup.UserGroupId INNER JOIN
		vlfUser WITH(NOLOCK) ON vlfUser.UserId = vlfUserGroupAssignment.UserId 
	WHERE vlfUserGroup.SecurityLevel >= @UserSecurityLevel 
			AND vlfUser.OrganizationId = @OrganizationId
			AND vlfUserGroup.OrganizationId IS NULL
			AND vlfUserGroup.UserGroupId NOT IN (1, 14) --system admin
	UNION
	
	SELECT DISTINCT vlfUserGroup.UserGroupId,
			vlfUserGroup.UserGroupName
	FROM vlfUserGroup WITH(NOLOCK)
	WHERE vlfUserGroup.SecurityLevel >= @UserSecurityLevel 
			AND vlfUserGroup.OrganizationId IS NULL
			AND vlfUserGroup.UserGroupId IN (2, 3, 4) --Security Administrator, Security High, Security Low
			AND @NewSecurityModel = 'false'
	UNION
	
	--organization groups
	SELECT DISTINCT vlfUserGroup.UserGroupId,
			vlfUserGroup.UserGroupName
	FROM vlfUserGroup WITH(NOLOCK) 
	WHERE vlfUserGroup.SecurityLevel >= @UserSecurityLevel 
			AND vlfUserGroup.OrganizationId = @OrganizationId
			AND vlfUserGroup.UserGroupId NOT IN (1, 14) --system admin
	ORDER BY vlfUserGroup.UserGroupName
END
ELSE
BEGIN
	--system groups that assigned to organization users
	SELECT DISTINCT vlfUserGroup.UserGroupId,
			vlfUserGroup.UserGroupName
	FROM vlfUserGroup WITH(NOLOCK) INNER JOIN
		vlfUserGroupAssignment WITH(NOLOCK) ON vlfUserGroupAssignment.UserGroupId = vlfUserGroup.UserGroupId INNER JOIN
		vlfUser WITH(NOLOCK) ON vlfUser.UserId = vlfUserGroupAssignment.UserId 
	WHERE vlfUserGroup.SecurityLevel >= @UserSecurityLevel 
			AND vlfUser.OrganizationId = @OrganizationId
			AND vlfUserGroup.OrganizationId IS NULL
			AND vlfUserGroup.UserGroupId NOT IN (1, 14)
			
	UNION
	SELECT DISTINCT vlfUserGroup.UserGroupId,
			vlfUserGroup.UserGroupName
	FROM vlfUserGroup WITH(NOLOCK) 
	WHERE vlfUserGroup.SecurityLevel >= @UserSecurityLevel 
			AND vlfUserGroup.OrganizationId IS NULL
			AND vlfUserGroup.UserGroupId IN (2, 3, 4) --Security Administrator, Security High, Security Low
			AND @NewSecurityModel = 'false'
	ORDER BY vlfUserGroup.UserGroupName
	
END
GO

INSERT INTO vlfOrganizationSettingsTypes(OrgPreferenceId, OrgPreferenceName)
VALUES (25, 'New Security Model')

INSERT into vlfOrganizationSettings(PreferenceValue, OrgPreferenceId, OrganizationId)
VALUES ('true', 25, 480)  

GO

DECLARE @UserGroupId smallint
SET @UserGroupId = 121

INSERT INTO vlfUserGroup(UserGroupId, UserGroupName, SecurityLevel, OrganizationId, ParentUserGroupId)
VALUES(@UserGroupId, 'BSM Developers', 1, 480, 1) 
GO

update vlfReportTypes
set ReportGroupID = 1
WHERE id in (10, 30,
33,
34,
36,
46,
47,
48,
49,
78,
106,
107,
109,
125,
132,
246,
288)

GO

update vlfReportTypes
set ReportGroupID = 2
WHERE id in (2,
3,
7,
13,
14,
15,
17,
19,
21,
23,
24,
25,
26,
27,
28,
29,
35,
37,
38,
39,
40,
41,
43,
50,
54,
67,
70,
71,
72,
75,
108,
120,
131,
133,
144,
156,
157,
175,
237,
243,
272,
274,
276,
278,
283,
294,
295,
296)

GO

--UPDATE vlfUserGroup
--SET UserGroupName = UserGroupName + ' *'
--WHERE OrganizationId IS NULL
	
--GO