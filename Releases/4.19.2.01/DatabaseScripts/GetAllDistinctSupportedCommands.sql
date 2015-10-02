USE [SentinelFM]
GO
/****** Object:  StoredProcedure [dbo].[GetAllDistinctSupportedCommands]    Script Date: 09/16/2014 14:30:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[GetAllDistinctSupportedCommands] ( @boxid varchar(max), @userid int)
as
Begin
SELECT DISTINCT vlfBoxCmdOutType.BoxCmdOutTypeId,
 vlfBoxCmdOutType.BoxCmdOutTypeName,
  vlfBoxProtocolTypeCmdOutType.Rules,
   vlfBoxProtocolType.BoxProtocolTypeId,
    vlfBoxProtocolType.BoxProtocolTypeName,
     vlfFirmwareChannels.ChPriority
      FROM vlfBoxProtocolTypeCmdOutType
       INNER JOIN vlfUserGroupAssignment 
       INNER JOIN vlfGroupSecurity ON vlfUserGroupAssignment.UserGroupId = vlfGroupSecurity.UserGroupId
        ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfGroupSecurity.OperationId 
        INNER JOIN vlfBoxCmdOutType ON vlfBoxProtocolTypeCmdOutType.BoxCmdOutTypeId = vlfBoxCmdOutType.BoxCmdOutTypeId 
        INNER JOIN vlfBoxProtocolType ON vlfBoxProtocolTypeCmdOutType.BoxProtocolTypeId = vlfBoxProtocolType.BoxProtocolTypeId 
        INNER JOIN vlfFirmwareChannelReference 
        INNER JOIN vlfBox with (nolock) ON vlfFirmwareChannelReference.FwChId = vlfBox.FwChId 
        INNER JOIN  vlfFirmwareChannels ON vlfFirmwareChannelReference.FwChId = vlfFirmwareChannels.FwChId 
        INNER JOIN vlfChannels ON vlfFirmwareChannels.ChId = vlfChannels.ChId ON vlfBoxProtocolType.BoxProtocolTypeId = vlfChannels.BoxProtocolTypeId 
        WHERE Charindex(','+cast(vlfBox.BoxId as varchar)+',', @boxid) > 0  AND 
        vlfUserGroupAssignment.UserId =@userid
        AND vlfGroupSecurity.OperationType = 2
        AND vlfBoxProtocolTypeCmdOutType.Visible = 1 
        ORDER BY vlfBoxCmdOutType.BoxCmdOutTypeName, vlfFirmwareChannels.ChPriority
End
