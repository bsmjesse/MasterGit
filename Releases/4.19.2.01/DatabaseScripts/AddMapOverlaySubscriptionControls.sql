INSERT INTO vlfUserGroup(UserGroupId, UserGroupName, SecurityLevel, OrganizationId, ParentUserGroupId)
VALUES(121, 'BSM Developers', 1, 480, 1) 
GO

DECLARE	@ControlName varchar(50),
	@Description varchar(255),
	@FormID int,
	@ControlURL varchar(300),
	@ControlIsActive bit,
	@ControlId int
	
SET @ControlName = 'divMapSubscription';
SET @Description = 'Map Subscription';
SET @FormID = 13;
SET @ControlURL = 'Configuration/frmOrganizationSettings.aspx';
SET @ControlIsActive = 1;

SET @ControlId = 94

INSERT INTO vlfGuiControls 
	(ControlId,
	ControlName,
	[Description],
	FormID,
	ControlURL,
	ControlIsActive)
	--ParentControlId)
VALUES 
	(@ControlId,
	@ControlName,
	@Description,
	@FormID,
	@ControlURL,
	@ControlIsActive)
	--@ParentControlId)

INSERT INTO	vlfGuiControlsLang
(	
	ControlId,
	LanguageID,
	ControlDescription
)
VALUES 
(
	@ControlId,
	1,
	@Description
)

INSERT INTO vlfGroupSecurity (UserGroupId, OperationId, OperationType)
VALUES(121, 94, 3)

SET @ControlName = 'divOverlaySubscription';
SET @Description = 'Overlay Subscription';
SET @FormID = 13;
SET @ControlURL = 'Configuration/frmOrganizationSettings.aspx';
SET @ControlIsActive = 1;

SET @ControlId = 95

INSERT INTO vlfGuiControls 
	(ControlId,
	ControlName,
	[Description],
	FormID,
	ControlURL,
	ControlIsActive)
	--ParentControlId)
VALUES 
	(@ControlId,
	@ControlName,
	@Description,
	@FormID,
	@ControlURL,
	@ControlIsActive)
	--@ParentControlId)

INSERT INTO	vlfGuiControlsLang
(	
	ControlId,
	LanguageID,
	ControlDescription
)
VALUES 
(
	@ControlId,
	1,
	@Description
)

INSERT INTO vlfGroupSecurity (UserGroupId, OperationId, OperationType)
VALUES(121, 95, 3)

SET @ControlName = 'divAllOrganizationOverlayLayersSetting';
SET @Description = 'All Organization Overlay Layers Setting';
SET @FormID = 13;
SET @ControlURL = 'Configuration/frmOrganizationSettings.aspx';
SET @ControlIsActive = 1;

SET @ControlId = 96

INSERT INTO vlfGuiControls 
	(ControlId,
	ControlName,
	[Description],
	FormID,
	ControlURL,
	ControlIsActive)
	--ParentControlId)
VALUES 
	(@ControlId,
	@ControlName,
	@Description,
	@FormID,
	@ControlURL,
	@ControlIsActive)
	--@ParentControlId)

INSERT INTO	vlfGuiControlsLang
(	
	ControlId,
	LanguageID,
	ControlDescription
)
VALUES 
(
	@ControlId,
	1,
	@Description
)

INSERT INTO vlfGroupSecurity (UserGroupId, OperationId, OperationType)
VALUES(121, 96, 3)
					
