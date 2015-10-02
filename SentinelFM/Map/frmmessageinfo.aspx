<%@ Page Language="c#" Inherits="SentinelFM.frmMessageInfo" CodeFile="frmMessageInfo.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Message Info</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script language="javascript">
			<!--
				function onblur(event) 
				{
					window.focus();
				}
			//-->
    </script>

</head>
<body onblur="window.focus()" >
    <form method="post" runat="server">
        <table id="Table1" style="border-right: gray thin solid; border-top: gray thin solid;
            z-index: 101; left: 6px; border-left: gray thin solid; width: 410px; border-bottom: gray thin solid;
            position: absolute; top: 5px; height: 312px" cellspacing="0" cellpadding="0"
            width="410" border="0">
            <tr>
                <td class="formtext" style="width: 120px" height="5">
                </td>
                <td class="Regulartext" align="left" height="5">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblTimeSentTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeSentTitleResource1">Time sent:</asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblTimeCreated" runat="server" CssClass="BigText" meta:resourcekey="lblTimeCreatedResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="30">
                </td>
                <td class="Regulartext" align="left" height="30">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblMessageTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMessageTitleResource1">Message:</asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="formtext" Width="99%" TextMode="MultiLine"
                        ReadOnly="True" Height="71px" BorderStyle="Solid" BackColor="WhiteSmoke" 
                        BorderColor="Gainsboro" meta:resourcekey="txtMessageResource1"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 120px; height: 20px">
                </td>
                <td style="width: 181px; height: 30px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1">From:</asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblFrom" runat="server" CssClass="BigText" meta:resourcekey="lblFromResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="30">
                </td>
                <td class="Regulartext" align="left" height="30">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1">To:</asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblTo" runat="server" CssClass="BigText" meta:resourcekey="lblToResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="30">
                </td>
                <td class="Regulartext" align="left" height="30">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblLicensePlateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLicensePlateTitleResource1">LicensePlate:</asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblLicensePlate" runat="server" CssClass="BigText" meta:resourcekey="lblLicensePlateResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="boldtext" style="width: 120px; height: 30px">
                </td>
                <td class="Regulartext" align="left">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblBoxIdTitle" runat="server" CssClass="formtext" meta:resourcekey="lblBoxIdTitleResource1">BoxId:</asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblBoxId" runat="server" CssClass="BigText" meta:resourcekey="lblBoxIdResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px; height: 21px">
                </td>
                <td class="Regulartext" style="height: 21px" align="left">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px; height: 28px">
                    &nbsp;<asp:Label ID="lblStreetAddressTitle" runat="server" CssClass="formtext" meta:resourcekey="lblStreetAddressTitleResource1">Street Address:</asp:Label></td>
                <td class="Regulartext" style="height: 28px" align="left">
                    <asp:Label ID="lblStreetAddress" runat="server" Height="15px" Width="190px" CssClass="BigText" meta:resourcekey="lblStreetAddressResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px; height: 6px">
                </td>
                <td class="Regulartext" style="height: 6px" align="left">
                </td>
            </tr>
            <tr>
                <td class="formtext" align="center" colspan="2">
                    <asp:Button ID="cmdMarkAsRead" runat="server" CssClass="combutton" Text="Mark as Read"
                        OnClick="cmdMarkAsRead_Click" meta:resourcekey="cmdMarkAsReadResource1"></asp:Button> &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button runat="server" CssClass="combutton" ID="cmdClose" Height="19px" OnClientClick="window.close()"
                        Text="Close" meta:resourcekey="cmdCloseResource1" /><%--<input class="combutton" id="cmdClose" style="height: 19px" onclick="window.close()"
                            type="button" value="Close" name="cmdCancel">--%></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="5">
                </td>
                <td align="right" height="5">
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
