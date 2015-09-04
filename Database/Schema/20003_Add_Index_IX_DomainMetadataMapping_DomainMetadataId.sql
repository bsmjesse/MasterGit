
BEGIN TRANSACTION
IF NOT EXISTS (SELECT * FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[dbo].[DomainMetadataMapping]') AND name = N'IX_DomainMetadataMapping_DomainMetadataId')

	CREATE NONCLUSTERED INDEX IX_DomainMetadataMapping_DomainMetadataId
	ON dbo.DomainMetadataMapping 
	(
		[DomainMetadataId] ASC
	) ON [PRIMARY]

GO
COMMIT

