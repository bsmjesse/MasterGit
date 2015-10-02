
CREATE TABLE [dbo].[Domain] (
		[DomainId] [int] NOT NULL,
		[Name] [nvarchar](60) NOT NULL,
		CONSTRAINT [PK_Domain] PRIMARY KEY CLUSTERED 
		(
			[DomainId] ASC
		)  ON [PRIMARY],
		CONSTRAINT IX_Domain_UQ_Name UNIQUE NONCLUSTERED ([Name])		
	) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DomainMetadata] (
		[DomainMetadataId] [bigint] IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
		[OrganizationId] [int] NULL,
		[DomainId] [int] NOT NULL,
		[MetadataValue] [nvarchar](200) NOT NULL,
		[MetadataIdentifier] [nvarchar](50) NULL,
		[InActive] [bit] NULL,
		CONSTRAINT [PK_DomainMetadata] PRIMARY KEY CLUSTERED 
		(
			[DomainMetadataId] ASC
		)  ON [PRIMARY],
		CONSTRAINT [FK_DomainMetadata_vlfOrganization] FOREIGN KEY 
		(
			[OrganizationId]
		) REFERENCES [dbo].[vlfOrganization] (
			[OrganizationId]
		),
		CONSTRAINT [FK_DomainMetadata_Domain] FOREIGN KEY 
		(
			[DomainId]
		) REFERENCES [dbo].[Domain] (
			[DomainId]
		)  
		
	) ON [PRIMARY]
GO

-- Add a Unique constraint
BEGIN TRANSACTION
if not exists (select * from dbo.sysobjects where name='IX_DomainMetadata_UQ_MetadataValueWithinOrgDomain')
ALTER TABLE dbo.DomainMetadata ADD CONSTRAINT
	IX_DomainMetadata_UQ_MetadataValueWithinOrgDomain UNIQUE NONCLUSTERED 
	(
		OrganizationId,
		DomainId,
		MetadataValue
	) ON [PRIMARY]
GO
COMMIT


CREATE TABLE [dbo].[DomainMetadataMapping] (
		[DomainMetadataMappingId] [bigint] IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
		[DomainMetadataId] [bigint] NOT NULL,
		[RecordId] [bigint] NOT NULL,
		CONSTRAINT [PK_DomainMetadataMapping] PRIMARY KEY CLUSTERED 
		(
			[DomainMetadataMappingId] ASC
		)  ON [PRIMARY],
		CONSTRAINT [FK_DomainMetadataMapping_DomainMetadata] FOREIGN KEY 
		(
			[DomainMetadataId]
		) REFERENCES [dbo].[DomainMetadata] (
			[DomainMetadataId]
		)  
		
	) ON [PRIMARY]
GO








