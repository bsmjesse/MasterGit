
ALTER TABLE [dbo].[vlfVehicleInfo] 
	ADD [OperationalState] [int] NULL,
		[Notes] [nvarchar](250) NULL
GO

-- * OperationalState column valid values are:
--   - Available = 100
--   - Unavailable = 200



























