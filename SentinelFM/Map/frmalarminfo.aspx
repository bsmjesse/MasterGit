<%@ Page Language="c#" Inherits="SentinelFM.frmAlarmInfo" CodeFile="frmAlarmInfo.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Alarm Info</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    

    <script language="javascript">
	<!--
			function onblur(event) 
			{
		window.focus();
		}


	//-->
    </script>

</head>
<body onblur="window.focus()" onunload="window.opener.document.location.reload()">
    <form method="post" runat="server">
        <table id="Table1" style="border-right: gray 4px double; border-top: gray 4px double;
            z-index: 101; left: 4px; border-left: gray 4px double; width: 370px; border-bottom: gray 4px double;
            position: absolute; top: 3px; height: 446px" cellspacing="0" cellpadding="0"
            width="370" border="0">
            <tr>
                <td class="formtext" style="width: 181px; height: 18px">
                    &nbsp;<asp:Label ID="lblAlarmDescriptionTitle" runat="server" CssClass="formtext"
                        meta:resourcekey="lblAlarmDescriptionTitleResource1" Text="Alarm Description:"></asp:Label></td>
                <td class="BigText" style="height: 18px" align="left">
                    
                    <asp:TextBox ID="lblAlarmDesc" runat="server" CssClass="regulartext" 
                        Height="53px" ReadOnly="True" TextMode="MultiLine" Width="100%" ></asp:TextBox>
                    
                    </td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                    &nbsp;<asp:Label ID="lblTimeCreatedTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeCreatedTitleResource1" Text="Time Created:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblTimeCreated" runat="server" CssClass="regulartext" meta:resourcekey="lblTimeCreatedResource1"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                    &nbsp;<asp:Label ID="lblTimeAcceptedTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeAcceptedTitleResource1" Text="Time Accepted:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblTimeAccepted" runat="server" CssClass="regulartext" meta:resourcekey="lblTimeAcceptedResource1"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px; height: 18px">
                    &nbsp;<asp:Label ID="lblOperatorNameTitle" runat="server" CssClass="formtext" meta:resourcekey="lblOperatorNameTitleResource1" Text="Operator Name:"></asp:Label></td>
                <td class="Regulartext" style="height: 18px" align="left">
                    <asp:Label ID="lblOperatorName" runat="server" CssClass="regulartext" meta:resourcekey="lblOperatorNameResource1"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px; height: 23px">
                    &nbsp;<asp:Label ID="lblAlarmStateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblAlarmStateTitleResource1" Text="Alarm State:"></asp:Label></td>
                <td class="formtext" style="height: 23px" align="left">
                    <asp:Label ID="lblAlarmState" runat="server" CssClass="regulartext" meta:resourcekey="lblAlarmStateResource1"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px; height: 23px">
                    &nbsp;<asp:Label ID="lblAlarmSeverity" runat="server" CssClass="formtext" meta:resourcekey="lblAlarmSeverityResource1" Text="Alarm Severity:"></asp:Label></td>
                <td class="formtext" style="height: 23px" align="left">
                    <asp:Label ID="lblAlarmLevel" runat="server" CssClass="regulartext" meta:resourcekey="lblAlarmLevelResource1"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                    &nbsp;<asp:Label ID="lblVehicleDescriptionTitle" runat="server" CssClass="formtext"
                        meta:resourcekey="lblVehicleDescriptionTitleResource1" Text="Vehicle Description:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblVehicleDesc" runat="server" CssClass="regulartext" meta:resourcekey="lblVehicleDescResource1"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                    &nbsp;<asp:Label ID="lblStreetAddressTitle" runat="server" CssClass="formtext" meta:resourcekey="lblStreetAddressTitleResource1" Text="Street Address:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblStreetAddress" runat="server" CssClass="regulartext" meta:resourcekey="lblStreetAddressResource1"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 181px; height: 25px" colspan=2 >
                    &nbsp;
                    <asp:Label ID="lblVehicleId" runat="server" Visible="False" meta:resourcekey="lblVehicleIdResource1"></asp:Label></td>
            </tr>
            <tr>
                <td colspan=2>
                    <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Width="100%" 
                        Enabled="False"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                </td>
                <td align="left">
                    <asp:Button ID="cmdAcceptAll" runat="server" CssClass="combutton" Text="Accept all vehicles Alarms"
                        Width="32px" Visible="False" OnClick="cmdAcceptAll_Click" meta:resourcekey="cmdAcceptAllResource1">
                    </asp:Button></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                    &nbsp;
                    <asp:Button ID="cmdCloseAll" runat="server" CssClass="combutton" Width="154px" Text="Close all alarms"
                        OnClick="cmdCloseAll_Click" meta:resourcekey="cmdCloseAllResource1"></asp:Button></td>
                <td align="left">
                    <asp:Button ID="cmdAccept" runat="server" CssClass="combutton" Text="Accept Alarm"
                        Width="154px" OnClick="cmdAccept_Click" meta:resourcekey="cmdAcceptResource1"></asp:Button>&nbsp;</td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                    &nbsp;
                    <asp:Button ID="cmdCloseAlarm" runat="server" CssClass="combutton" Width="154px"
                        Text="Close Alarm" OnClick="cmdCloseAlarm_Click" meta:resourcekey="cmdCloseAlarmResource1">
                    </asp:Button></td>
                <td align="left">
                    <asp:Button ID="cmdMapIt" runat="server" CssClass="combutton" Width="154px" Text="Map It"
                        CommandName="36" OnClick="cmdMapIt_Click" meta:resourcekey="cmdMapItResource1"></asp:Button></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 181px">
                    &nbsp;</td>
                <td align="left">
                    <asp:Button runat="server" CssClass="combutton" ID="cmdClose" Width="154px" Height="19px"
                        OnClientClick="window.close()" Text="Exit" meta:resourcekey="cmdCloseResource1" /><%--<input class="combutton" id="cmdClose"
                            style="width: 154px; height: 19px" onclick="window.close()" type="button" value="Exit"
                            name="cmdCancel">--%>&nbsp;</td>
            </tr>
        </table>
        <asp:Label ID="lblMessage" Style="z-index: 102; left: 55px; position: absolute; top: 455px"
            runat="server" CssClass="errortext" Width="271px" Height="26px" meta:resourcekey="lblMessageResource1"></asp:Label>
        
    </form>
</body>
</html>
