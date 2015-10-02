<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomerReportMenu.ascx.cs" Inherits="Configuration_CustomerReport_CustomerReportMenu" %>
<table id="tblDriverAssgn" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
    width: 190px; position: relative; top: 0px; height: 22px">
    <tr>
        <td style="width: 250px">
            <asp:Button ID="cmpEmail" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/CustomerReport/CustomerReportEmail.aspx"
                CssClass="confbutton" Text="Emails" Width="112px" meta:resourcekey="cmpEmailResource1" />
        </td>
        <td style="width: 250px">
            <asp:Button ID="cmpEmailBody" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/CustomerReport/CustomerReportEmailBody.aspx"
                CssClass="confbutton" Text="Email Body" Width="112px" meta:resourcekey="cmpEmailBodyResource1" />
        </td>

    </tr>
</table>