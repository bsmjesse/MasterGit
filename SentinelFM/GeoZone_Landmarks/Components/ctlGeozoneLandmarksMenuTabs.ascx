<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlGeozoneLandmarksMenuTabs.ascx.cs" Inherits="SentinelFM.Components.GeoZone_Landmarks_Components_ctlGeozoneLandmarksMenuTabs" %>
<table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td>
            <asp:Button ID="cmdLandmark" runat="server" Text="Landmarks" CssClass="confbutton"
                CausesValidation="False" meta:resourcekey="cmdLandmarkResource1" OnClick="cmdLandmark_Click" CommandName="102"></asp:Button></td>
        <td>
            <asp:Button ID="cmdGeoZone" runat="server" Text="GeoZones" CssClass="confbutton"
                CausesValidation="False" OnClick="cmdGeoZone_Click" meta:resourcekey="cmdGeoZoneResource1" CommandName="103"></asp:Button></td>
        <td style="width: 7px">
            <asp:Button ID="cmdVehicleGeoZone" runat="server" Text="Vehicle-GeoZone Assignment"
                CssClass="confbutton" CausesValidation="False" Width="192px"
                OnClick="cmdVehicleGeoZone_Click" meta:resourcekey="cmdVehicleGeoZoneResource1" CommandName="104"></asp:Button></td>
        <td>
            <asp:Button ID="cmdWaypoints" runat="server" Text="Waypoints"
                CssClass="confbutton" CausesValidation="False" Visible="False"
                CommandName="8" Width="192px"
                OnClick="cmdWaypoints_Click" meta:resourcekey="cmdWaypointsResource1"></asp:Button>
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdMap" runat="server" Text="Map" CssClass="confbutton" CausesValidation="False"
                OnClick="cmdMap_Click" meta:resourcekey="cmdMapResource1" CommandName="105"></asp:Button></td>
        <td>
            <asp:Button ID="cmdFleetGeozone" runat="server" Text="Fleet-GeoZone Assignment"
                CssClass="confbutton" CausesValidation="False" Width="192px" Visible="true"
                OnClick="cmdFleetGeozone_Click"
                meta:resourcekey="cmdFleetGeozoneResource1" CommandName="106"></asp:Button>
        </td>
        <td>
            <asp:Button ID="cmdFleetLandmark" runat="server"
                Text="Fleet-Landmark Assignment" CssClass="confbutton" CausesValidation="false"
                Width="192px" Visible="true"
                OnClick="cmdFleetLandmark_Click"
                meta:resourcekey="cmdFleetLandmarkResource1" CommandName="107"></asp:Button>
        </td>
        <td></td>
    </tr>
</table>
