<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlContactMenu.ascx.cs" Inherits="Configuration_Contact_ctlContactMenu" %>
<table id="tblDriverAssgn" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
    width: 190px; position: relative; top: 0px; height: 22px">
    <tr>
        <td style="width: 250px">
            <asp:Button ID="cmdContacts" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/Contact/frmContacts.aspx"
                CssClass="confbutton" Text="Contacts" Width="112px" meta:resourcekey="cmdContactsResource1" />
        </td>
        <td style="width: 250px">
            <asp:Button ID="cmdPlans" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/Contact/frmContactPlans.aspx"
                CssClass="confbutton" Text="Emergency Plans" Width="112px" meta:resourcekey="cmdPlansResource1" />
        </td>
    </tr>
</table>