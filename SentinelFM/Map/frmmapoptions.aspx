<%@ Page Language="c#" Inherits="SentinelFM.Map.frmMapOptions" CodeFile="frmMapOptions.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Map Options</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script language="javascript">
		<!--
			var pWin
			function setParent(){
				pWin=top.window.opener
			}
			function reloadParent(){  
				pWin.location.href='frmVehicleMap.aspx'
				 
			}

		//-->
    </script>

</head>
<body onload="setParent()">
    <form id="frmMapOptionsForm" method="post" runat="server">
        <table class="formtext" id="Table1" style="border-right: gray 1px solid; border-top: gray 1px solid;
            z-index: 101; left: 8px; border-left: gray 1px solid; width: 221px; border-bottom: gray 1px solid;
            position: absolute; top: 8px; height: 188px" cellspacing="0" cellpadding="0"
            width="221" border="0">
            <tr>
                <td class="heading" align="center">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowVehicleName" runat="server" CssClass="formtext" Text="Show Vehicle names."
                        Checked="True" meta:resourcekey="chkShowVehicleNameResource1"></asp:CheckBox></td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowLandmark" runat="server" CssClass="formtext" Text="Show Landmarks Icons."
                        Checked="True" AutoPostBack="True" 
                        OnCheckedChanged="chkShowLandmark_CheckedChanged" 
                        meta:resourcekey="chkShowLandmarkResource1">
                    </asp:CheckBox></td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowLandmarkname" runat="server" CssClass="formtext" Text="Show Landmark  names."
                        Checked="True" AutoPostBack="True" OnCheckedChanged="chkShowLandmarkname_CheckedChanged" meta:resourcekey="chkShowLandmarknameResource1">
                    </asp:CheckBox></td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td height="5"><asp:CheckBox ID="chkShowGeoZones" runat="server" 
                        CssClass="formtext" Text="Show GeoZones"
                        Checked="True" AutoPostBack="True" 
                        meta:resourcekey="chkShowGeoZonesResource1" >
                        
                        </asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td height="5">
                </td>
            </tr>
            <tr>
                <td style="height: 5px">
                    <asp:CheckBox ID="chkShowLandmarkRadius" runat="server" AutoPostBack="True" 
                        Checked="True" CssClass="formtext"
                        Text="Show Landmark Radius" 
                        oncheckedchanged="chkShowLandmarkRadius_CheckedChanged" 
                        meta:resourcekey="chkShowLandmarkRadiusResource1" /></td>
            </tr>
            <tr>
                <td align="left">
                    &nbsp;&nbsp;&nbsp;
                    <table id="Table2" style="height: 19px" cellspacing="0" cellpadding="0" width="100%"
                        border="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1">
                                </asp:Button></td>
                            <td>
                                <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="window.close()"
                                    Text="Close" meta:resourcekey="cmdCloseResource1" /><%--<input class="combutton" id="cmdClose" onclick="window.close()" type="button"
                                        value="Close" name="cmdClose">--%></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" height="5">
                </td>
            </tr>
        </table>
        &nbsp;
    </form>
</body>
</html>
