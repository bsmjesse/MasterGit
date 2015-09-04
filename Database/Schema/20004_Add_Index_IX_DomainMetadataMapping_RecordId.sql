

BEGIN TRANSACTION
IF NOT EXISTS (SELECT * FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[dbo].[DomainMetadataMapping]') AND name = N'IX_DomainMetadataMapping_RecordId')

	CREATE NONCLUSTERED INDEX IX_DomainMetadataMapping_RecordId
	ON dbo.DomainMetadataMapping 
	(
		[RecordId] ASC
	) ON [PRIMARY]

GO
COMMIT



