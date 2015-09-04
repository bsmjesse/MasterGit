
IF OBJECT_ID('tempdb..#TempOrg') IS NOT NULL
DROP TABLE #TempOrg

Select Distinct L.OrganizationId Into #TempOrg  
From dbo.vlfLandmark L (NOLOCK) 
Order By L.OrganizationId Asc

Declare @OrgId int	
Declare @NewOrOldCategoryId int 
Declare @CatIdentityOutput Table (CatId int)

While EXISTS(SELECT * From #TempOrg)
Begin

BEGIN TRANSACTION

BEGIN TRY

    Select Top 1 @OrgId = OrganizationId From #TempOrg

	-- Filter LandmarkCategory only for this Organization
	IF OBJECT_ID('tempdb..#TempLMC') IS NOT NULL
	DROP TABLE #TempLMC
	
	Select DMM.DomainMetadataId, DMM.RecordId INTO #TempLMC
	From dbo.DomainMetadataMapping DMM (NOLOCK) 
			Inner join dbo.DomainMetadata DM (NOLOCK) ON DMM.DomainMetadataId = DM.DomainMetadataId
	Where DM.DomainId = 1 AND DM.OrganizationId = @OrgId 
	

	-- Get all the landmark which are not assigned to any category
	IF OBJECT_ID('tempdb..#TempLM') IS NOT NULL
	DROP TABLE #TempLM

	Select L.LandmarkId INTO #TempLM
	From dbo.vlfLandmark L (NOLOCK)
	Where L.OrganizationId = @OrgId AND 
		NOT EXISTS
		(
			Select 1 FROM #TempLMC TLMC Where TLMC.RecordId = L.LandmarkId
		)


    -- Process the record
    Set @NewOrOldCategoryId = 0
    Select @NewOrOldCategoryId = MC.DomainMetadataId From [dbo].[DomainMetadata] MC
    Where MC.MetadataValue = 'Generic' and MC.DomainId=1 and MC.OrganizationId = @OrgId	
	
	IF (@NewOrOldCategoryId = 0) -- Not exists, then insert default category
	BEGIN
		INSERT INTO [dbo].[DomainMetadata] (OrganizationId, DomainId, MetadataValue)
			OUTPUT INSERTED.DomainMetadataId INTO @CatIdentityOutput
		VALUES (@OrgId, 1,  N'Generic');
		SELECT @NewOrOldCategoryId = (SELECT CatId FROM @CatIdentityOutput)
		DELETE From @CatIdentityOutput
	END

	-- Insert Landmark with 'Generic' category
	INSERT INTO [dbo].[DomainMetadataMapping]
			(DomainMetadataId, RecordId)
			SELECT @NewOrOldCategoryId, LandmarkId From #TempLM
			   
    -- Remove the record
    Delete #TempOrg Where OrganizationId = @OrgId

END TRY

BEGIN CATCH
    --returns the complete original error message as a result set
    SELECT @OrgId as OrganizationId,
         ERROR_NUMBER() AS ErrorNumber
        ,ERROR_SEVERITY() AS ErrorSeverity
        ,ERROR_STATE() AS ErrorState
        ,ERROR_PROCEDURE() AS ErrorProcedure
        ,ERROR_LINE() AS ErrorLine
        ,ERROR_MESSAGE() AS ErrorMessage

	IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

	
	-- Remove the record
    Delete #TempOrg Where OrganizationId = @OrgId
END CATCH

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;


End -- While loop

IF OBJECT_ID('tempdb..#TempLMC') IS NOT NULL
	DROP TABLE #TempLMC
IF OBJECT_ID('tempdb..#TempLM') IS NOT NULL
	DROP TABLE #TempLM
IF OBJECT_ID('tempdb..#TempOrg') IS NOT NULL
    DROP TABLE #TempOrg



