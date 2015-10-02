
-- Mantis Ticket: 0013643: CN - Add 2 Types of CN Landmarks in 2 Categories
--===========================================================================

--  Move landmarks to 'CN Vendor' category if the name starts with '-V*****'
Update dbo.DomainMetadataMapping
	Set DomainMetadataId = (Select DomainMetadataId From dbo.DomainMetadata Where OrganizationId=123 And DomainId=1 And MetadataValue = 'CN Vendor')
Where RecordId IN (
						Select L.LandmarkId 
						From		vlfLandmark L
						Where		L.OrganizationId=123
									And L.LandmarkName Like '-V[0-9][0-9][0-9][0-9][0-9]%'
				  )

--  Move landmarks to 'CN Installers (Costa)' category if the name starts with '-' and not like '-V*****'
Update dbo.DomainMetadataMapping
	Set DomainMetadataId =	(Select DomainMetadataId From dbo.DomainMetadata Where OrganizationId=123 And DomainId=1 And MetadataValue = 'CN Installers (Costa)')
Where RecordId IN (
						Select		L.LandmarkId
						From		dbo.vlfLandmark L
						Where		L.OrganizationId=123
									and L.LandmarkName like '-%' and  L.LandmarkName not like  '-V[0-9][0-9][0-9][0-9][0-9]%'
				  )
























