
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[UpdateOrganizationDomainMetadata]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateOrganizationDomainMetadata]
GO

CREATE PROCEDURE [dbo].[UpdateOrganizationDomainMetadata]
@OrganizationId int,
@UserId int,
@DomainMetadataId bigint,
@NewMetadataName nvarchar(200)

 AS 

 Update dbo.DomainMetadata 
 Set MetadataValue = @NewMetadataName
 Where OrganizationId=@OrganizationId AND DomainMetadataId=@DomainMetadataId

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO







