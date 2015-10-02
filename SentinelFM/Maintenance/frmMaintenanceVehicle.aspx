<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceVehicle.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceVehicle" Theme="TelerikControl"  %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register Src="../UserControl/FleetOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
         EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnClose" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="gdMCCMaintenances" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="combMccGroup" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboFleet" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
             <telerik:AjaxSetting AjaxControlID="pnlFleetVehicle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="gdSource" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="gdDest" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnl" runat ="server" >
<table cellpadding="0" cellspacing="0" >
       <tr>
          <td align="left" style="padding-left:5px" >
            <table cellpadding="0" cellspacing="0" >
              <tr>
            <td align="left"  >
                <asp:Label ID="lblMccGroup" runat="server"  CssClass="formtext" Width="100px" meta:resourcekey="lblMccGroupResource1"
                    Text="Group:"></asp:Label>
           <%-- </td>
            <td  align="left"  style="width:70px">--%>
                <telerik:RadComboBox ID="combMccGroup" runat="server" CssClass="RegularText" Width="258px"
                    DataTextField="MccName" DataValueField="MccId" meta:resourcekey="combMccGroupResource1"
                    Skin="Hay"   
                    AutoPostBack = "true" MaxHeight ="200px" onselectedindexchanged="combMccGroup_SelectedIndexChanged" 
                    >
                </telerik:RadComboBox>
            </td>
            <td>
                            &nbsp;
            </td>
            <td>
                 <asp:Button ID="btnAdd" Text="Vehicle Assignment"  runat="server" CommandName="Modify"
                       CssClass="combutton" Width="180px" 
                     meta:resourcekey="btnAddNewMCCMaintenanceResource1" 
                     OnClientClick="if (!GetSelectedVehicle(true)) return false;" 
                     onclick="btnAdd_Click" />

            </td>
            </tr>
                <tr>

                    <td  align="left">               
                <asp:Panel ID="pnlFleetVehicle"  runat="server" style="display:none">
                <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                </asp:Panel>  
            </td>
                    
                    <td></td>
                    <td></td>
                </tr>
            </table>
          </td>
        </tr>
   <tr>
      <td>
       <Sentinel:Grid AutoGenerateColumns="False" ID="gdMCCMaintenances" 
              AllowAutomaticDeletes="false" IsAutoResize="true" 
                                                        AllowAutomaticInserts="false" 
              AllowSorting="true" AllowAutomaticUpdates="false"
                                                        AllowPaging="True" 
              runat="server" Skin="Hay" GridLines="None" Width="960px" OnNeedDataSource="gdMCCMaintenances_NeedDataSource"
                                                        
                                                         AllowFilteringByColumn="true" 
              FilterItemStyle-HorizontalAlign="Left" PageSize="20" 
              onitemcommand="gdMCCMaintenances_ItemCommand" 
                                                        >
                                                        <GroupingSettings CaseSensitive="false" />
                                                        <MasterTableView DataKeyNames="MaintenanceId" ClientDataKeyNames="MaintenanceId"
                                                            CommandItemDisplay="Top">
                                                            <Columns>
<telerik:GridTemplateColumn  UniqueName="SelectSource" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:CheckBox ID="btnAssign" runat="server"  meta:resourcekey="gvServicesAssignResource5" onclick="return MCCMaintenancesCheck(this)" Checked="true" /></ItemTemplate>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkAssignAllMcc" runat="server"  meta:resourcekey="gvServicesAssignAllResource5" 
                                        onclick="return MCCMaintenancesCheckAll(this)" Checked="true" /></HeaderTemplate>
                                <ItemStyle Width="60px" HorizontalAlign="Center"></ItemStyle>
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                                                                <telerik:GridBoundColumn HeaderText="Description" UniqueName="Description" SortExpression="Description"
                                                                    meta:resourcekey="gdMCCMaintenancesDescriptionResource1" AllowFiltering="true"
                                                                    DataField="Description">
                                                                    <ItemStyle Width="200px" />
                                                                    <HeaderStyle Width="200px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridTemplateColumn HeaderText="Operation Type" UniqueName="OperationType" SortExpression="OperationType"
                                                                    meta:resourcekey="gdMCCMaintenancesOperationTypeResource1" AllowFiltering="true"
                                                                    DataField="OperationType">
                                                                    <ItemTemplate>
                                                                        <asp:Label id="lblOperationType"  runat="server" Text='<%# Eval("OperationType") %>' ></asp:Label>
                                                                        <asp:HiddenField id="hidOperationType"  runat="server" Value='<%# Eval("OperationTypeID") %>' ></asp:HiddenField>
                                                                        <asp:HiddenField id="hidInterval"  runat="server" Value='<%# Eval("Interval") %>' ></asp:HiddenField>
                                                                        <asp:HiddenField id="hidFixDate"  runat="server" Value='<%# Eval("FixedDate") %>' ></asp:HiddenField>
                                                                        <asp:HiddenField id="hidTimespanId"  runat="server" Value='<%# Eval("TimespanId") %>' ></asp:HiddenField>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridBoundColumn HeaderText="Notification Type" UniqueName="NotificationType"
                                                                    DataField="NotificationDescription" SortExpression="NotificationDescription"
                                                                    meta:resourcekey="gdMCCMaintenancesNotificationTypeResource1" AllowFiltering="true"
                                                                    >
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Frequency" UniqueName="FrequencyName" SortExpression="FrequencyName"
                                                                    meta:resourcekey="gdMCCMaintenancesFrequencyIDResource1" AllowFiltering="true"
                                                                    DataField="FrequencyName" >
                                                                    <ItemStyle Width="100px" />
                                                                    <HeaderStyle Width="100px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Interval" UniqueName="Interval" SortExpression="IntervalDesc "
                                                                    meta:resourcekey="gdMCCMaintenancesIntervalResource1" AllowFiltering="true" DataField="IntervalDesc "
                                                                    >
                                                                    <ItemStyle Width="120px" />
                                                                    <HeaderStyle Width="120px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Fixed Date" Visible="false" UniqueName="FixedServiceDate" SortExpression="FixedServiceDate" DataField="FixedServiceDate" meta:resourcekey="gdMCCMaintenancesFixedFixedServiceDateResource1" >
                                                                    <ItemStyle Width="80px" />
                                                                    <HeaderStyle Width="80px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn HeaderText="Fixed Date" UniqueName="FixedServiceDate_1" SortExpression="FixedServiceDate_1" DataField="FixedServiceDate_1" meta:resourcekey="gdMCCMaintenancesFixedServiceDate_1eResource1" >
                                                                    <ItemStyle Width="80px" />
                                                                    <HeaderStyle Width="80px" />
                                                                </telerik:GridBoundColumn>
                                                            </Columns>
                                                            <HeaderStyle CssClass="RadGridtblHeader" />
                                                            <CommandItemTemplate>
                                                                <table id="tblCustomerCommand" runat="server" width="100%">
                                                                    <tr>
                                                                        <td align="left">
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </CommandItemTemplate>
                                                        </MasterTableView>
                                                        
                                                        <ClientSettings>
                                                                            <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="false"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="false">
                    </Resizing>

                                                        </ClientSettings>
                                                    </Sentinel:Grid>
      </td>
   </tr>
   <tr>
      <td align="left" >
            <table  cellpadding="0" cellspacing="0" width="100%">
               <tr>
               <td align="left">
                        <asp:Panel ID="pnlVehicle" runat ="server" Visible="true" style="padding-left:5px" >
               <table id="tblVehicle" cellpadding="0" cellspacing="0" >
                 <tr>
                             <td align="left" >
                <asp:Label ID="lblFleet" runat="server" CssClass="formtext" Width="103px" meta:resourcekey="lblFleetResource1"
                    Text="Fleet:"></asp:Label>
            </td>
            <td  align="left">
                <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px"
                    DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                    Skin="Hay"  onselectedindexchanged="cboFleet_SelectedIndexChanged"
                    AutoPostBack = "true" MaxHeight ="200px" 
                    >
                </telerik:RadComboBox>
            </td>
            <td>
            </td>
            <td align="left">
             
               <asp:Button ID="btnClose" runat="server" Text ="Close1"  CssClass="combutton" Visible="false"
                    meta:resourcekey="btnCloseResource1" onclick="btnClose_Click" Width="180px"  />
            </td>
                 </tr>
               </table>
               </asp:Panel>
               </td>
               </tr>
               <tr>
                  <td colspan="3">
                     <table>
                         <tr>
                            <td>
 <Sentinel:Grid ID="gdSource" runat="server" AllowSorting="true"  
                    Width="500px"  IsAutoResize="true" PageSize="20" 
                    EnableTheming="true" VerticalGridNum="2"  AllowPaging="True" AutoGenerateColumns="False"
                    meta:resourcekey="gvServicesSourceResource1" 
                    AllowFilteringByColumn="true" FilterItemStyle-HorizontalAlign="Left"
                    OnNeedDataSource="gdSource_NeedDataSource" 
                    onitemcommand="gvServicesSource_ItemCommand" onitemdatabound="gdSource_ItemDataBound">
                     <GroupingSettings CaseSensitive="false" />
                    <MasterTableView DataKeyNames="VehicleId" ClientDataKeyNames="VehicleId" CommandItemDisplay="Top">
                        <Columns>
                            <telerik:GridTemplateColumn  UniqueName="SelectSource" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:Button ID="btnAssign" runat="server" Text="Assign" meta:resourcekey="gvServicesAssignResource5a" Width="105px" CommandName="Assign" CssClass="formtext" /></ItemTemplate>
                                <HeaderTemplate>
                                    <asp:Button ID="btnAssignAll" runat="server" Text="Assign All" meta:resourcekey="gvServicesAssignAllResource5a" Width="105px" CommandName="AssignAll" CssClass="formtext"
                                         /></HeaderTemplate>
                                <ItemStyle Width="100px" HorizontalAlign="Center"></ItemStyle>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="VehicleId" UniqueName="Description" SortExpression="Description"
                                meta:resourcekey="gdMCCMaintenancesDescriptionResource1" AllowFiltering="true"
                                DataField="Description">
                            </telerik:GridBoundColumn>

                            <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="LastService" HeaderText="Last Service Value and Date" >
                               <ItemTemplate>
                                    <telerik:RadGrid ID="dgLastService" runat="server" AutoGenerateColumns="False" 
                                        GridLines="None" Skin="Simple" ShowHeader="false" Width="295px" onitemdatabound="dgLastService_ItemDataBound" >
                                        <MasterTableView DataKeyNames="MaintenanceId" ClientDataKeyNames="MaintenanceId" 
                                                            GroupLoadMode="Server">
                                            <Columns>
                                                <telerik:GridBoundColumn HeaderText="Description" UniqueName="Description" 
                                                    DataField="Description">
                                                    <ItemStyle Width="80px" HorizontalAlign="Left"  />
                                                </telerik:GridBoundColumn>

                                                <telerik:GridTemplateColumn HeaderText="Odo\Hrs" DataField="value" 
                                                    UniqueName="CurrentVal">
                                                    <ItemTemplate>
                                                           <telerik:RadNumericTextBox Width="70px" id="txtOdoHrsLast" runat="server" CssClass="formtext"  Value="0"  >
                                                              <NumberFormat DecimalDigits="0"  />
                                                           </telerik:RadNumericTextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="80px" HorizontalAlign="Left"  />
                                                    <HeaderStyle Width="80px" />
                                                </telerik:GridTemplateColumn>

                                                <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="ColcalValueLast" HeaderText="Date"  DataField="lstdate" >
                                                    <ItemTemplate>
                                                        <telerik:RadDatePicker ID="calValueLast" runat="server"  DateInput-EmptyMessage="" Width="100px" 
                                                            MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US"  >
                                                            <Calendar ID="Calendar1" Width="120px" runat="server" >
                                                                <SpecialDays>
                                                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday"  />
                                                                </SpecialDays>
                                                            </Calendar>
                                                            <DateInput ID="DateInput1"  Culture="en-US"  runat="server" DateFormat='<%# sn.User.DateFormat %>' DisplayDateFormat='<%# sn.User.DateFormat %>' LabelCssClass="" >
                                                                 <ClientEvents  OnValueChanged="InputCalValueChanged"  OnError = "onErrorInputValue" OnValueChanging="SaveInitalInputCalValue"  />
                                                            </DateInput>
                                                        </telerik:RadDatePicker>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                            <ItemStyle HorizontalAlign="Left" />
                                            <AlternatingItemStyle HorizontalAlign="Left" />
                                            <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                        </MasterTableView>
                                        <ClientSettings EnableRowHoverStyle="false">
                                        </ClientSettings>
                                    </telerik:RadGrid>
                               </ItemTemplate>
                               <ItemStyle Width="300px"  CssClass="SubRadGridItem" />
                               <HeaderStyle  Width="300px"  />
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="ColcalValue" HeaderText="Start Date"  >
                               <ItemTemplate>
                               <telerik:RadDatePicker ID="calValue" runat="server"  DateInput-EmptyMessage="" Width="100px"
                                    MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US" >
                                    <Calendar ID="Calendar1" Width="120px" runat="server" >
                                        <SpecialDays>
                                           <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                        </SpecialDays>

                                    </Calendar>
                                    <DateInput ID="DateInput1"  Culture="en-US"  runat="server" DateFormat='<%# sn.User.DateFormat %>' DisplayDateFormat='<%# sn.User.DateFormat %>' LabelCssClass="" >
                                      <ClientEvents  OnValueChanged="InputCalValueChanged"  OnError = "onErrorInputValue" OnValueChanging="SaveInitalInputCalValue"  />
                                    </DateInput>
                                    
                                </telerik:RadDatePicker>

                               </ItemTemplate>
                               <ItemStyle Width="110px" />
                               <HeaderStyle  Width="110px"  />
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <ItemStyle VerticalAlign="Top" />
                        <AlternatingItemStyle VerticalAlign="Top" />
                        <HeaderStyle CssClass="RadGridtblHeader" ForeColor="White" />
                        <CommandItemTemplate>
                            <table id="tblCustomerCommand" runat="server" width="100%">
                                <tr>
                                    <td> <b>
                                        <asp:Label ID="lblUnassignedMaintenances" CssClass="formtext" runat="server" Text="Unassigned Vehicles"
                                            meta:resourcekey="lblUnassignedMaintenancesResource1"></asp:Label>
                                         </b>
                                    </td>
                                </tr>
                            </table>
                        </CommandItemTemplate>
                    </MasterTableView><ClientSettings>
                        <Selecting AllowRowSelect="false" />
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true"  />
                    </ClientSettings>
                </Sentinel:Grid>                         
                   </td>
                   </tr>
                   <tr>
                   <td>
 <Sentinel:Grid ID="gdDest" runat="server" AllowSorting="true"  
                    Width="400px"  IsAutoResize="true"
                    EnableTheming="true" VerticalGridNum="2"  AllowPaging="True" AutoGenerateColumns="False" PageSize="20" 
                    meta:resourcekey="gvServicesSourceResource1" 
                    AllowFilteringByColumn="true" FilterItemStyle-HorizontalAlign="Left"
                    OnNeedDataSource="gvDest_NeedDataSource" 
                    onitemcommand="gvDest_ItemCommand">
                     <GroupingSettings CaseSensitive="false" />
                    <MasterTableView DataKeyNames="VehicleId" ClientDataKeyNames="VehicleId" CommandItemDisplay="Top">
                        <Columns>
                            <telerik:GridTemplateColumn  UniqueName="SelectSource" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:Button ID="btnAssign" runat="server" Text="Unassign" meta:resourcekey="gvServicesAssignResource5" Width="105px" CommandName="UnAssign" CssClass="formtext" /></ItemTemplate>
                                <HeaderTemplate>
                                    <asp:Button ID="btnAssignAll" runat="server" Text="Unassign All" meta:resourcekey="gvServicesAssignAllResource5" Width="105px" CommandName="UnAssignAll" CssClass="formtext" 
                                         /></HeaderTemplate>
                                <ItemStyle Width="120px" HorizontalAlign="Center"></ItemStyle>
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Vehicle" UniqueName="Description" SortExpression="Description"
                                meta:resourcekey="gdMCCMaintenancesDescriptionResource1" AllowFiltering="true"
                                DataField="Description">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <ItemStyle VerticalAlign="Top" />
                        <AlternatingItemStyle VerticalAlign="Top" />
                        <HeaderStyle CssClass="RadGridtblHeader" ForeColor="White" />
                        <CommandItemTemplate>
                            <table id="tblCustomerCommand" runat="server" width="100%">
                                <tr>
                                    <td> <b>
                                        <asp:Label ID="lblUnassignedMaintenances" CssClass="formtext" runat="server" Text="Assigned Vehicles"
                                            meta:resourcekey="lblAsignedMaintenancesResource1"></asp:Label>
                                         </b>
                                    </td>
                                </tr>
                            </table>
                        </CommandItemTemplate>
                    </MasterTableView><ClientSettings>
                        <Selecting AllowRowSelect="false" />
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true"   />
                    </ClientSettings>
                </Sentinel:Grid>      
                   </td>
                         </tr>
                     </table>
                  </td>
               </tr>
            </table>
      </td>
   </tr>
       <tr>
           <td align="left">
               &nbsp;</td>
       </tr>
       <tr>
           <td align="left">
               &nbsp;</td>
       </tr>
</table>
</asp:Panel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >
        function MCCMaintenancesCheckAll(ctl) {
            var isChecked = $telerik.$(ctl).attr("checked");
            $telerik.$("#<%=gdMCCMaintenances.ClientID %>").find("input:checkbox").attr("checked", isChecked); ;
            ReBindVehicles();
        }

        var isWorngInput = false;
        function onErrorInputValue(sender, eventArgs) {
            isWorngInput = true;
            eventArgs.set_cancel(true);
        }

        function SaveInitalInputCalValue(sender, eventArgs) {
            var selectText = "1";
            $telerik.$(sender.get_element()).parent().find("input[type!='hidden']").each(function () {
                if ($telerik.$(this).css("visibility").toLowerCase() != 'hidden') {
                    selectText = $telerik.$(this).val();
                }
            })
            if (isWorngInput || selectText == '') {
                alert('<%= inputCalError %>');
                eventArgs.set_cancel(true);
            }
            isWorngInput = false;
        }


        function GetSelectedVehicle(isShow) {
            var selectedVehicle = 0;
            var tableView = $find("<%= gdMCCMaintenances.ClientID %>").get_masterTableView();
            var count = tableView.get_dataItems().length;
            for (var i = 0; i < count; i++) {
                var item = tableView.get_dataItems()[i];
                var selectCell = tableView.getCellByColumnUniqueName(item, "SelectSource")
                if ($telerik.$(selectCell).find("input:checkbox[id$='btnAssign']").attr("checked") == true) {
                    selectedVehicle = selectedVehicle + 1;
                }
            }

            if (selectedVehicle == 0) {
                if (isShow) {
                    alert('<%= selectMaintenance%>')
                }
                return false;
            }

            return true;
        }

        function MCCMaintenancesCheck(ctl) {
            var isChecked = $telerik.$(ctl).attr("checked");
            if (!isChecked) $telerik.$("#<%=  gdMCCMaintenances.ClientID%>").find("[ID$='chkAssignAllMcc']").attr("checked", isChecked);
            ReBindVehicles();
        }

        function ReBindVehicles() {
            if (!GetSelectedVehicle(false)) {
                if ($telerik.$("#<%= pnlVehicle.ClientID %>").length > 0) {
                    $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("closevehicles");
                }
                return
            }
            if ($telerik.$("#<%= pnlVehicle.ClientID %>").length > 0) {
                $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("ReBindVehicles");
            }
        }

        function InputCalValueChanged(sender, eventArgs) {

        }

    </script>
</telerik:RadCodeBlock>
</asp:Content>
