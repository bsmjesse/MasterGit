
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[DeleteOrganizationDomainMetadata]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteOrganizationDomainMetadata]
GO

CREATE PROCEDURE [dbo].[DeleteOrganizationDomainMetadata]
@OrganizationId int,
@UserId int,
@DomainMetadataId bigint

 AS 

IF (@DomainMetadataId > 0)
	BEGIN
		Delete From dbo.DomainMetadata
		Where	OrganizationId=@OrganizationId AND 
				DomainMetadataId=@DomainMetadataId AND 
				NOT EXISTS (Select DMM.DomainMetadataId
							From dbo.DomainMetadataMapping DMM (NOLOCK)
							Where DMM.DomainMetadataId = @DomainMetadataId)
	END


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

