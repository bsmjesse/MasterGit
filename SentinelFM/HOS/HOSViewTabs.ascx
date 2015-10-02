<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HOSViewTabs.ascx.cs" Inherits="HOS_HOSViewTabs" %>
<table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td style="width: 180px">
            <asp:Button ID="cmdLogs" runat="server" Text="Log Sheet"
                CausesValidation="False" CssClass="confbutton"
                meta:resourcekey="cmdLogsResource"
                PostBackUrl="~/HOS/frmManagingHOS.aspx"
                Width="180px" CommandName="84" />
        </td>
        <td style="width: 180px">
            <asp:Button ID="cmdFuel" runat="server" Text="Fuel Form" CssClass="confbutton" CausesValidation="False" Visible="true"
                meta:resourcekey="cmdFuelResource" PostBackUrl="~/HOS/frmHOSFuel.aspx"
                Width="180px" CommandName="85"></asp:Button>
        </td>
        <td style="width: 180px">
            <asp:Button ID="cmdHistory" runat="server" Text="History" CssClass="confbutton" CausesValidation="False" Visible="true"
                meta:resourcekey="cmdHistoryResource" PostBackUrl="~/HOS/frmHOSHistory.aspx"
                Width="180px" CommandName="86"></asp:Button>
        </td>
        <td style="width: 0px">
            <asp:Button ID="cmdInputDuty" Visible="false" runat="server" Text="Input Log" CssClass="confbutton" CausesValidation="False"
                meta:resourcekey="cmdFuelResource" PostBackUrl="~/HOS/frmHOSInputLog.aspx"
                Width="180px"></asp:Button>
        </td>
        <td style="width: 0px">
            <asp:Button ID="cmdDynamicForms" Visible="false" runat="server" Text="Dynamic Forms" CssClass="confbutton" CausesValidation="False"
                meta:resourcekey="cmdDynamicFormsResource" PostBackUrl="~/HOS/ViewDynamicForm.aspx"
                Width="180px" CommandName="87"></asp:Button>
        </td>
        <td style="width: 0px">
            <asp:Button ID="cmdVehicleInspection" Visible="false" runat="server" Text="Vehicle Inspection Form" CssClass="confbutton" CausesValidation="False"
                meta:resourcekey="cmdDynamicFormsResource" PostBackUrl="~/HOS/frmVehicleInspection.aspx"
                Width="180px" CommandName="87"></asp:Button>
        </td>
    </tr>
</table>
