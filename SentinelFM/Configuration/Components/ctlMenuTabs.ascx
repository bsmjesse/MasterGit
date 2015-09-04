<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlMenuTabs.ascx.cs" Inherits="SentinelFM.Components.Configuration_Components_ctlMenuTabs" %>
<table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
    <tr>
        <td>
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdOrganization" runat="server" CausesValidation="False" CssClass="confbutton"
                meta:resourcekey="cmdDriverResource2" PostBackUrl="~/Configuration/frmOrganizationSettings.aspx"
                Text="Organization" CommandName="48" />
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdFleets" runat="server" Text="Fleets" CssClass="confbutton" CausesValidation="False"
                CommandName="1" meta:resourcekey="cmdFleetsResource1" OnClick="cmdFleets_Click">
            </asp:Button>
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdVehicles" runat="server" Text="Vehicles" CssClass="confbutton"
                CausesValidation="False" CommandName="2" OnClick="cmdVehicles_Click" meta:resourcekey="cmdVehiclesResource2">
            </asp:Button>
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdEquipment" runat="server" Text="Equipment" CssClass="confbutton"
                CausesValidation="False" OnClick="cmdEquipment_Click" meta:resourcekey="cmdEquipmentResource2">
            </asp:Button>
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdUsers" runat="server" CommandName="22" CausesValidation="False"
                CssClass="confbutton" Text="Users" OnClick="cmdUsers_Click" meta:resourcekey="cmdUsersResource2">
            </asp:Button>
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdDriver" runat="server" CausesValidation="False" CssClass="confbutton" CommandName="55"
                Text="Drivers" PostBackUrl="~/Configuration/frmdrivers.aspx" meta:resourcekey="cmdDriverResource1">
            </asp:Button>
        </td>
        <td style="width: 7px">
            <asp:Button ID="cmdScheduledTasks" runat="server" CausesValidation="False" CssClass="confbutton" CommandName="56"
                Text="Scheduled Tasks" meta:resourcekey="cmdScheduledTasksResource1" OnClick="cmdScheduledTasks_Click"
                Width="150px" />
        </td>
        <td>
            <asp:Button ID="btnPolicies" runat="server" CausesValidation="False" CssClass="confbutton"
                Visible="False" Text="Policies" PostBackUrl="~/Configuration/frmpolicies.aspx"
                meta:resourcekey="btnPoliciesResource1" />
        </td>
        <td>
            <asp:Button ID="btnPanicManagerment" runat="server" CausesValidation="False" CssClass="confbutton" CommandName="57"
                Visible="true" Text="Panic Management" PostBackUrl="~/Configuration/Contact/frmContacts.aspx"
                meta:resourcekey="btnPanicManagermentResource1" Width="125px" />
        </td>
        <td>
            <asp:Button ID="btnMaintenance" runat="server" CausesValidation="False" CssClass="confbutton" CommandName="58"
                Visible="false" Text="Maintenance ▼" OnClientClick="return ClickAdminInConfiguration(this);"
                Width="120px" />
            
        </td>
        <td>
            <asp:Button ID="btnWorkinghrs" runat="server" CausesValidation="False" CssClass="confbutton" CommandName="59"
                Visible="true" Text="After Hours Alerts" PostBackUrl="~/Configuration/WorkingHour/frmWorkingHrs.aspx"
                Width="120px" />
            
        </td>
        <td>
            <asp:Button ID="btnCustomReport" runat="server" CausesValidation="False" CssClass="confbutton"  CommandName="60"
                Visible="true" Text="Custom Report" PostBackUrl="~/Configuration/CustomerReport/CustomerReportEmail.aspx"
                Width="120px" />
            
        </td>
        <td>
            <asp:Button ID="btnHOS" runat="server" CausesValidation="False" CssClass="confbutton"  CommandName="47"
                Visible="true" Text="HOS" PostBackUrl="~/HOS/frmEmailAddress.aspx"
                Width="120px" />
            
        </td>
        <td>
            <asp:Button ID="btnServiceAssigment" runat="server" CausesValidation="False" CssClass="confbutton"  CommandName="72"
                Visible="true" Text="Alert assignments" meta:resourcekey="btnServiceAssigmentResource1" Width="140px"  OnClick="cmdServiceAssignment_Click" />  
        </td>
    </tr>
</table>
        <div  id='ui-slider-MaintenanceAdm-for-screen' style=' width:220px; background:#fdf9e5; border:1px solid black;display:none;text-align: left;padding: 5px 5px 5px 5px;position: absolute;z-index:99999;'>
             <div style='border: solid 1px black;    padding:2px'>
                <table>
                   <tr>
                    <td>
                        <a href='<%= GetMaintenanceUrl("NotificationPolicy") %>'  ><span class="rmText" >Notification
                        Policy</span></a>
                     </td>
                   </tr>
                   <tr>
                     <td >
                        <a href='<%= GetMaintenanceUrl("Services") %>' ><span class="rmText">
                            Services</span></a>
                     </td>
                   </tr>
                   <tr>
                     <td>
                         <a href='<%= GetMaintenanceUrl("MCCGroup") %>' ><span class="rmText">
                                MCC Group</span></a>
                     </td>
                   </tr>
                   <tr>
                     <td >
                         <a href='<%= GetMaintenanceUrl("MCCMaintenances") %>' ><span class="rmText">
                                    MCC Maintenances</span></a>
                     </td>
                   </tr>
                   <tr>
                     <td ><a href='<%= GetMaintenanceUrl("MCCNotificationType") %>' ><span class="rmText"
                                       >MCC Notification Type</span></a>
                     </td>
                   </tr>
                   <tr>
                    <td >
                     <a href='<%= GetMaintenanceUrl("MCCAssignment") %>'
                                            ><span class="rmText">MCC Group Maintenances Assignment </span></a>
                     </td>
                   </tr>
                   <tr>
                     <td ><a href="#" onclick="alert('To be built');return false;" ><span class="rmText">Vehicle Maintenances
                        Assignment</span></a>
                        </td>
                </tr>
                </table>
            </div>
        </div>

<script type="text/javascript">
    function ClickAdminInConfiguration(ctl) {
        var clickAdminInConfigurationScipt =
        "var uiCtl = telerik('#ui-slider-MaintenanceAdm-for-screen'); " +
        "uiCtl.unbind('blur');" +
        "var position = telerik(ctl).position();" +
        "var x = telerik(ctl).parents('td:first').offset().left - 8;" +
        "var y = telerik(ctl).parents('table:first').offset().top + telerik(ctl).parents('table:first').height() - 3;" +
        "uiCtl.css('top', y + 'px').css('left', x + 'px');" +
        "uiCtl.focusout(function() { " +
             "var uiCtl = telerik('#ui-slider-MaintenanceAdm-for-screen'); " +
              "uiCtl.fadeOut(200);" +
        "});" +
        "uiCtl.slideDown(500, function(){" +
             "var uiCtl = telerik('#ui-slider-MaintenanceAdm-for-screen'); " +
        //"uiCtl[0].focus();" +
              "uiCtl.find('a')[0].focus();" +
        "});"

        if ('<%= isJquery %>' == '1') {
            eval(clickAdminInConfigurationScipt.replace(/telerik/g, "$"));
        }
        else {
            eval(clickAdminInConfigurationScipt.replace(/telerik/g, "$telerik.$"));
        }
        return false;
    }


</script>
