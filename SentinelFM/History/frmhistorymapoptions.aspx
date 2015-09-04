<%@ Page Language="c#" Inherits="SentinelFM.History.frmHistoryMapOptions" CodeFile="frmHistoryMapOptions.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Map Options</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0"/>
    <meta name="CODE_LANGUAGE" content="C#"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
    <script language="javascript" type="text/javascript">
		<!--
            var pWin;
			function setParent(){
			    pWin = top.window.opener;
            }
			
			function reloadParent(){
			    pWin.location.href = 'frmHistMap.aspx';
			}
		//-->
    </script>
</head>
<body onload="setParent()">
    <form id="frmHistoryMapOptionsForm" method="post" runat="server">
        <table class="tableDoubleBorder" id="Table1" style="z-index: 101; left: 8px;position: absolute; top: 8px; height: 186px" cellspacing="0" cellpadding="0"
            width="204" border="0">
            <tr>
                <td class="heading" align="center">
                    <asp:CheckBox ID="chkShowVehicleName" runat="server" CssClass="formtext" Text="Show Vehicle name"
                        Checked="True" Visible="False" meta:resourcekey="chkShowVehicleNameResource1"/>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowLandmark" runat="server" CssClass="formtext" Text="Show Landmarks Icon."
                        Checked="True" AutoPostBack="True" 
                        OnCheckedChanged="chkShowLandmark_CheckedChanged" 
                        meta:resourcekey="chkShowLandmarkResource1"/>
               </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowLandmarkname" runat="server" CssClass="formtext" Text="Show Landmark names."
                        Checked="True" AutoPostBack="True" OnCheckedChanged="chkShowLandmarkname_CheckedChanged" 
                        meta:resourcekey="chkShowLandmarknameResource1"/>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowBreadCrumb" runat="server" Text="Show BreadCrumb Trail."
                        CssClass="formtext" meta:resourcekey="chkShowBreadCrumbResource1"/>
                    <asp:CheckBox ID="chkShowStopSq" runat="server" Text="Show Stop Sequence No." CssClass="formtext"
                        Visible="False" meta:resourcekey="chkShowStopSqResource1"/>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td height="5">
                    <asp:CheckBox ID="chkShowGeoZones" runat="server" AutoPostBack="True" Checked="True"
                        CssClass="formtext" Text="Show GeoZones"/>
                </td>
            </tr>
            <tr>
                <td height="5">
                </td>
            </tr>
            <tr>
                <td height="5">
                    <asp:CheckBox ID="chkShowLandmarkRadius" runat="server" AutoPostBack="True" Checked="True"
                        CssClass="formtext" Text="Show Landmark Radius" 
                        oncheckedchanged="chkShowLandmarkRadius_CheckedChanged"/>
                </td>
            </tr>
            <tr>
                <td align="left">
                    &nbsp;&nbsp;&nbsp;
                    <table id="Table2" style="width: 212px; height: 19px" cellspacing="0" cellpadding="0" width="212" border="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1"/>
                            </td>
                            <td>
                                <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="window.close()"
                                    Text="Close" meta:resourcekey="cmdCloseResource1" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" height="5">
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
