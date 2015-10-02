USE [SentinelFM]
GO

/****** Object:  Table [dbo].[vlfOrganizationSensorPreference] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[vlfOrganizationSensorPreference](
	[OrganizationId] [int] NOT NULL,
	[SensorName] VARCHAR(30) NOT NULL
 CONSTRAINT [PK_vlfOrganizationSensorPreference] PRIMARY KEY CLUSTERED 
(
	[OrganizationId] ASC,
	[SensorName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[vlfOrganizationSensorPreference]  WITH CHECK ADD  CONSTRAINT [FK_vlfOrganizationSensorPreference_vlfOrganization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[vlfOrganization] ([OrganizationId])
GO

ALTER TABLE [dbo].[vlfOrganizationSensorPreference] CHECK CONSTRAINT [FK_vlfOrganizationSensorPreference_vlfOrganization]
GO


