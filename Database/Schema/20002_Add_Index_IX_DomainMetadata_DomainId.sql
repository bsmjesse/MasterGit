

BEGIN TRANSACTION
IF NOT EXISTS (SELECT * FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[dbo].[DomainMetadata]') AND name = N'IX_DomainMetadata_DomainId')

	CREATE NONCLUSTERED INDEX IX_DomainMetadata_DomainId
	ON dbo.DomainMetadata 
	(
		[DomainId] ASC
	) ON [PRIMARY]

GO
COMMIT



































