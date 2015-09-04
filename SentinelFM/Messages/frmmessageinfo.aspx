<%@ Reference Page="~/map/frmmessageinfo.aspx" %>

<%@ Page Language="c#" Inherits="SentinelFM.Messages.frmMessageInfo" CodeFile="frmMessageInfo.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Message Info</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="frmMessageInfoForm" method="post" runat="server">
        <table id="Table1" style="border-right: gray thin solid; border-top: gray thin solid;
            z-index: 101; left: 6px; border-left: gray thin solid; width: 321px; border-bottom: gray thin solid;
            position: absolute; top: 5px; height: 312px" cellspacing="0" cellpadding="0"
            width="321" border="0">
            <tr>
                <td class="BigFormText" style="width: 120px" height="5">
                </td>
                <td class="Regulartext" align="left" height="5">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblTimeSentTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeSentTitleResource1"
                        Text="Time sent:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblTimeCreated" runat="server" CssClass="RegularText" meta:resourcekey="lblTimeCreatedResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="BigFormText" style="width: 120px" height="10">
                </td>
                <td align="left" height="10">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1"
                        Text="From:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblFrom" runat="server" CssClass="RegularText" meta:resourcekey="lblFromResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="BigFormText" style="width: 120px" height="10">
                </td>
                <td class="Regulartext" align="left" height="10">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1"
                        Text="To:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblTo" runat="server" CssClass="RegularText" meta:resourcekey="lblToResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="BigFormText" style="width: 120px; height: 19px" height="19">
                </td>
                <td class="Regulartext" align="left" height="19" style="height: 19px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblLicensePlateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLicensePlateTitleResource1"
                        Text="License Plate:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:Label ID="lblLicensePlate" runat="server" CssClass="BigRegularText" meta:resourcekey="lblLicensePlateResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                </td>
                <td class="Regulartext" align="left">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblMessageTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMessageTitleResource1"
                        Text="Message:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="RegularText" Width="190px"
                        TextMode="MultiLine" ReadOnly="True" Height="71px" BorderStyle="Solid" BackColor="WhiteSmoke"
                        BorderColor="Gainsboro" meta:resourcekey="txtMessageResource1"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                </td>
                <td class="Regulartext" align="left">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                    &nbsp;<asp:Label ID="lblResponseTitle" runat="server" CssClass="formtext" meta:resourcekey="lblResponseTitleResource1"
                        Text="Response:"></asp:Label></td>
                <td class="Regulartext" align="left">
                    <asp:TextBox ID="txtResponse" runat="server" CssClass="RegularText" Width="190px"
                        TextMode="MultiLine" ReadOnly="True" Height="34px" BorderStyle="Solid" BackColor="WhiteSmoke"
                        BorderColor="Gainsboro" meta:resourcekey="txtResponseResource1"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 120px; height: 10px">
                </td>
                <td style="width: 181px; height: 10px">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px; height: 14px">
                    &nbsp;<asp:Label ID="lblResponseDateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblResponseDateTitleResource1"
                        Text="Response Date:"></asp:Label></td>
                <td class="Regulartext" align="left" style="height: 14px">
                    <asp:Label ID="lblResponseDate" runat="server" CssClass="RegularText" meta:resourcekey="lblResponseDateResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="BigFormText" style="width: 120px" height="10">
                </td>
                <td class="Regulartext" align="left" height="10">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="10">
                    &nbsp;<asp:Label ID="lblAcknowledgedTitle" runat="server" CssClass="formtext" meta:resourcekey="lblAcknowledgedTitleResource1"
                        Text="Acknowledged:"></asp:Label></td>
                <td class="Regulartext" align="left" height="10">
                    <asp:Label ID="lblAcknowledged" runat="server" CssClass="RegularText" meta:resourcekey="lblAcknowledgedResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px; height: 14px" height="14">
                </td>
                <td class="Regulartext" style="height: 14px" align="left" height="14">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="10">
                    &nbsp;<asp:Label ID="lblReadTitle" runat="server" CssClass="formtext" meta:resourcekey="lblReadTitleResource1"
                        Text="Read:"></asp:Label></td>
                <td class="Regulartext" align="left" height="10">
                    <asp:Label ID="lblRead" runat="server" CssClass="RegularText" meta:resourcekey="lblReadResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="10">
                </td>
                <td class="Regulartext" align="left" height="10">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="10">
                    &nbsp;<asp:Label ID="lblStreetAddressTitle" runat="server" CssClass="formtext" meta:resourcekey="lblStreetAddressTitleResource1"
                        Text="Street Address:"></asp:Label></td>
                <td class="Regulartext" align="left" height="10">
                    <asp:Label ID="lblStreetAddress" runat="server" CssClass="RegularText" meta:resourcekey="lblStreetAddressResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px" height="10">
                </td>
                <td class="Regulartext" align="left" height="10">
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 120px">
                </td>
                <td align="center">
                    <asp:Button ID="cmdClose" runat="server" Height="19px" CssClass="combutton" OnClientClick="window.close()"
                        Text="Close" meta:resourcekey="cmdCloseResource1" /></td>
                <%--<input class="combutton" id="cmdClose" style="height: 19px" onclick="window.close()"
                        type="button" value="Close" name="cmdCancel">--%>
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
