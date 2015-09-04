
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[ListOrganizationDomainMetadata]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ListOrganizationDomainMetadata]
GO


CREATE PROCEDURE [dbo].[ListOrganizationDomainMetadata]
@OrganizationId int = 0,
@UserId int = 0,
@DomainId int = 0
 AS 

Select DM.DomainMetadataId, DM.MetadataValue, DM.MetadataIdentifier
From dbo.DomainMetadata DM (NOLOCK) 
Where OrganizationId=@OrganizationId And DM.DomainId=@DomainId And ISNULL(InActive,0) <> 1
Order By DM.MetadataValue ASC


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

