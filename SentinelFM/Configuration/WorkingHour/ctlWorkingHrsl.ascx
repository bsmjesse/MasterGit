<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlWorkingHrsl.ascx.cs" Inherits="Configuration_WorkingHour_ctlWorkingHrsl" %>
<table id="tblDriverAssgn" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
    width: 190px; position: relative; top: 0px; height: 22px">
    <tr>
        <td style="width: 250px">
            <asp:Button ID="cmdWorkingHrs" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/WorkingHour/frmWorkingHrs.aspx"
                CssClass="confbutton" Text="After Hours Alerts" Width="150px" meta:resourcekey="cmdContactsResource1" />
        </td>
        <td style="width: 250px">
            <asp:Button ID="cmdReport" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/WorkingHour/frmWorkingHrsRpt.aspx"
                CssClass="confbutton" Text="Report" Width="150px" meta:resourcekey="cmdPlansResource1" />
        </td>
        <td style="width: 250px">
            <asp:Button ID="cmdSupress" runat="server" CausesValidation="False" PostBackUrl="~/Configuration/WorkingHour/frmWorkingHrsSuppressed.aspx"
                CssClass="confbutton" Text="Suppressed Email" Width="150px" meta:resourcekey="cmdPlansResource1" />
        </td>
    </tr>
</table>