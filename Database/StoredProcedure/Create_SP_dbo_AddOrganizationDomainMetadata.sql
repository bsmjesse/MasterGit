
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'[dbo].[AddOrganizationDomainMetadata]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AddOrganizationDomainMetadata]
GO

CREATE PROCEDURE [dbo].[AddOrganizationDomainMetadata]
@OrganizationId int,
@UserId int,
@DomainId int,
@MetadataName nvarchar(200)

 AS 

 INSERT INTO dbo.DomainMetadata (OrganizationId, DomainId, MetadataValue) Values (@OrganizationId, @DomainId, @MetadataName)


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO







