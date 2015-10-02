<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FleetVehicle.ascx.cs"
    Inherits="SentinelFM.UserControl_FleetVehicle" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table >
    <tr >
        <td align="left" >
            <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                Text="Fleet:" />
        </td>
        <td align="left" >
            <telerik:RadComboBox ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" Width="257px" 
                Filter="Contains" MarkFirstMatch="True" 
                ChangeTextOnKeyBoardNavigation="False" Skin="Hay"
                MaxHeight="400px" CausesValidation = "False"
            />
        </td>
        <td align="left" >
            <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleNameResource1"
                Text=" Vehicle:"  />
        </td>
        <td align="left">
            <telerik:RadComboBox ID="cboVehicle" runat="server" CssClass="RegularText" DataTextField="Description"
                DataValueField="VehicleId" meta:resourcekey="cboVehicleResource1" Width="256px" 
                Filter="Contains" MarkFirstMatch="True" 
                ChangeTextOnKeyBoardNavigation="False" Skin="Hay"
                MaxHeight="400px" CausesValidation = "False"
            />
        </td>
    </tr>
</table>
