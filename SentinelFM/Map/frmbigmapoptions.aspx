<%@ Page Language="c#" Inherits="SentinelFM.Map.frmBigMapOptions" CodeFile="frmBigMapOptions.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

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
				pWin.location.href='frmBigMap.aspx'
			}

		//-->
    </script>

</head>
<body onload="setParent()" onunload="reloadParent()">
    <form method="post" runat="server">
        <table class="formtext" id="Table1" style="border-right: gray 1px outset; border-top: gray 1px outset;
            z-index: 101; left: 7px; border-left: gray 1px outset; width: 204px; border-bottom: gray 1px outset;
            position: absolute; top: 6px; height: 186px" cellspacing="0" cellpadding="0"
            width="204" border="0">
            <tr>
                <td class="heading" align="center">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowVehicleName" runat="server" CssClass="formtext" Text="Show Vehicle name"
                        Checked="True" meta:resourcekey="chkShowVehicleNameResource1"></asp:CheckBox></td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowLandmark" runat="server" CssClass="formtext" Text="Show Landmark"
                        Checked="True" AutoPostBack="True" OnCheckedChanged="chkShowLandmark_CheckedChanged" meta:resourcekey="chkShowLandmarkResource1">
                    </asp:CheckBox></td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkShowLandmarkname" runat="server" CssClass="formtext" Text="Show Landmark name"
                        Checked="True" AutoPostBack="True" OnCheckedChanged="chkShowLandmarkname_CheckedChanged" meta:resourcekey="chkShowLandmarknameResource1">
                    </asp:CheckBox></td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td height="5">
                </td>
            </tr>
            <tr>
                <td align="left">
                    &nbsp;&nbsp;&nbsp;
                    <table id="Table2" style="width: 214px; height: 19px" cellspacing="0" cellpadding="0"
                        width="214" border="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1">
                                </asp:Button></td>
                            <td>
                                <asp:Button runat="server" ID="cmdClose" CssClass="combutton" OnClientClick="window.close()"
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
    </form>
</body>
</html>
