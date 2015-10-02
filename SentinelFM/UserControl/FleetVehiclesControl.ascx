<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FleetVehiclesControl.ascx.cs" Inherits="SentinelFM.UserControl_FleetVehiclesControl" %>
    <table style="background-color:<%=BackColor%>;width: <%=Width%>; height:<%=Height%>; border:<%=Border%>" cellpadding="2">
        <tr>
            <td style="width: 85px">
                <asp:Label ID="lblFleets" runat="server" Text="Fleet:" Width="80px" meta:resourcekey="lblFleetsResource1"></asp:Label>
            </td>
            <td style="width: 165px">
                <asp:DropDownList runat="server" ID="ddlFleets" DataTextField="FleetName" DataValueField="FleetId" AutoPostBack="True" Width="160px" OnSelectedIndexChanged="ddlFleets_SelectedIndexChanged" meta:resourcekey="ddlFleetsResource1"></asp:DropDownList>
            </td>
            <td style="width: 85px">
                <asp:Label ID="lblVehicles" runat="server" Text="Vehicle:" Width="80px" meta:resourcekey="lblVehiclesResource1"></asp:Label>
            </td>
            <td style="width: 165px">
                <asp:DropDownList runat="server" ID="ddlVehicles" Enabled="False" DataValueField="VehicleId" DataTextField="Description" Width="160px" OnSelectedIndexChanged="ddlVehicles_SelectedIndexChanged" meta:resourcekey="ddlVehiclesResource1" AutoPostBack="True"></asp:DropDownList>
            </td>
        </tr>
    </table>
