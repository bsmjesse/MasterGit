<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmAssignmentList.aspx.cs"
    Inherits="SentinelFM.Configuration_frmAssignmentList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="ctlEquipmentMenu.ascx" TagName="ctlEquipmentMenu" TagPrefix="uc2" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .equipmentTable
        {
            padding: 1px !important;
        }
    </style>
</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        meta:resourcekey="RadAjaxManager1Resource1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdVehicle" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gdVehicle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdVehicle" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdVehicle" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="text-align: left; width: 900px">
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicles" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="table" style="height: 550px;
                                    width: 990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px;
                                                margin-top: 5px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table5" style="z-index: 101; width: 190px; position: relative; top: 0px;
                                                                        height: 22px" cellspacing="0" cellpadding="0" border="0">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Button ID="cmdVehicleInfo" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                                    Text="Vehicle Info" CommandName="4" Width="112px" OnClick="cmdVehicleInfo_Click"
                                                                                    meta:resourcekey="cmdVehicleInfoResource1"></asp:Button>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Button ID="cmdAlarms" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                                    OnClick="cmdAlarms_Click" Text="Sensors/Alarms/Messages" CommandName="5" Width="176px"
                                                                                    meta:resourcekey="cmdAlarmsResource1"></asp:Button>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Button ID="cmdOutputs" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                                    Text="Outputs" CommandName="6" OnClick="cmdOutputs_Click" meta:resourcekey="cmdOutputsResource1">
                                                                                </asp:Button>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Button ID="cmdFleetVehicle" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                                    Text="Fleet-Vehicle Assignment" CommandName="7" Width="169px" OnClick="cmdFleetVehicle_Click"
                                                                                    meta:resourcekey="cmdFleetVehicleResource1"></asp:Button>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Button ID="btnEquipmentAssignment" runat="server" Width="168px" CausesValidation="False"
                                                                                    OnClientClick="javascript:return false;" Text="Equipment Assignment" CssClass="selectedbutton"
                                                                                    meta:resourcekey="btnEquipmentAssignmentResource1"></asp:Button>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760"
                                                                        style="padding-bottom: 5px">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td align="center" style="padding-top: 3px">
                                                                                            <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px"
                                                                                                DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                                                                                                Skin="Hay" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" AutoPostBack="true"
                                                                                                MaxHeight="200px">
                                                                                            </telerik:RadComboBox>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td valign="top">
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 2px; height: 495px">
                                                                                                <tr>
                                                                                                    <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                                                                        <telerik:RadGrid ID="gdVehicle" Width="900px" runat="server" AutoGenerateColumns="False"
                                                                                                            AllowSorting="True" AllowPaging="True" PageSize="15" GridLines="None" ShowGroupPanel="false"
                                                                                                            AllowFilteringByColumn="true" FilterItemStyle-HorizontalAlign="Left" Skin="Hay"
                                                                                                            OnNeedDataSource="gdVehicle_NeedDataSource" OnItemDataBound="gdVehicle_ItemDataBound">
                                                                                                            <GroupingSettings CaseSensitive="false" />   
                                                                                                            <MasterTableView DataKeyNames="VehicleId" CommandItemDisplay="Top" ClientDataKeyNames="VehicleId"
                                                                                                                GroupLoadMode="Server">
                                                                                                                <Columns>
                                                                                                                    <telerik:GridBoundColumn DataField="Description" HeaderText="Vehicle" SortExpression="Description"
                                                                                                                        UniqueName="Description">
                                                                                                                        <ItemStyle Width="200px" VerticalAlign="Top" />
                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                    <telerik:GridBoundColumn DataField="BoxId" HeaderText="Box ID" SortExpression="BoxId"
                                                                                                                        UniqueName="BoxId">
                                                                                                                        <ItemStyle Width="80px" VerticalAlign="Top" />
                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                    <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Assignments">
                                                                                                                        <ItemTemplate>
                                                                                                                            <telerik:RadGrid ID="gdAssignment" runat="server" AutoGenerateColumns="False" GridLines="None"
                                                                                                                                OnDeleteCommand="gdAssignment_DeleteCommand" OnNeedDataSource="gdAssignment_NeedDataSource"
                                                                                                                                Skin="Simple" OnItemDataBound="gdAssignment_ItemDataBound" ShowHeader="false">
                                                                                                                                <MasterTableView DataKeyNames="AssignmentId" GroupLoadMode="Server">
                                                                                                                                    <Columns>
                                                                                                                                        <telerik:GridBoundColumn DataField="EquipmentDescription" HeaderText="Equipment"
                                                                                                                                            SortExpression="EquipmentDescription" UniqueName="EquipmentDescription">
                                                                                                                                            <ItemStyle Width="250px" />
                                                                                                                                        </telerik:GridBoundColumn>
                                                                                                                                        <telerik:GridBoundColumn DataField="MediaDescription" HeaderText="Media" SortExpression="MediaDescription"
                                                                                                                                            UniqueName="MediaDescription">
                                                                                                                                            <ItemStyle Width="250px" />
                                                                                                                                        </telerik:GridBoundColumn>
                                                                                                                                        <telerik:GridTemplateColumn>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="../../images/edit.gif" ToolTip="Modify Media Values" />
                                                                                                                                            </ItemTemplate>
                                                                                                                                            <ItemStyle HorizontalAlign="Center" />
                                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                                        <telerik:GridButtonColumn ConfirmText="Delete this assignment?" ConfirmDialogType="Classic"
                                                                                                                                            ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                                                                            UniqueName="DeleteColumn" meta:resourcekey="GridButtonColumnDeleteResource1"
                                                                                                                                            ImageUrl="../../images/delete.gif">
                                                                                                                                            <ItemStyle HorizontalAlign="Center" />
                                                                                                                                        </telerik:GridButtonColumn>
                                                                                                                                    </Columns>
                                                                                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                                                                                    <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                                    <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                                </MasterTableView>
                                                                                                                                <ClientSettings EnableRowHoverStyle="false">
                                                                                                                                </ClientSettings>
                                                                                                                            </telerik:RadGrid>
                                                                                                                        </ItemTemplate>
                                                                                                                        <HeaderTemplate>
                                                                                                                            <norbr>
                                                                                                                                     <asp:Label ID="lblEquipmentText" Width="255px" style="font:12px/16px 'segoe ui',arial,sans-serif;" ForeColor="White" runat="server" Text ="Equipment" meta:resourcekey="lblEquipmentTextResource1" ></asp:Label>
                                                                                                                                    <asp:Label ID="lblMedia" runat="server"  Width="240px" style="font:12px/16px 'segoe ui',arial,sans-serif;" ForeColor="White" Text ="Media" meta:resourcekey="lblMediaResource1" ></asp:Label>
                                                                                                                          </norbr>
                                                                                                                        </HeaderTemplate>
                                                                                                                        <ItemStyle Width="600px" CssClass="equipmentTable" />
                                                                                                                        <HeaderStyle />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                </Columns>
                                                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                                                                <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader" />
                                                                                                                <CommandItemTemplate>
                                                                                                                    <table width="100%">
                                                                                                                        <tr>
                                                                                                                            <td align="left">
                                                                                                                                <asp:Button ID="btAddNew" runat="server" Text="Add New Assignment" OnClientClick="return ShowInsertForm();" Width="130px" CssClass="combutton"  />
                                                                                                                            </td>
                                                                                                                            <td align="right">
                                                                                                                                <nobr>
                                                                                                                                <asp:ImageButton ID="imgFilter" runat="server" OnClientClick ="javascript:return showFilterItem();" ImageUrl="~/images/filter.gif" />
                                                                                                                                <asp:LinkButton ID="hplFilter" runat="server"  OnClientClick ="javascript:return showFilterItem();" Text="Show Filter" meta:resourcekey="hplFilterResource1" Font-Underline="true"></asp:LinkButton>
                                                                                                                                </nobr>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </CommandItemTemplate>
                                                                                                            </MasterTableView>
                                                                                                            <ClientSettings>
                                                                                                                <ClientEvents OnGridCreated="GridCreated" />
                                                                                                            </ClientSettings>
                                                                                                        </telerik:RadGrid>
                                                                                                        <asp:HiddenField ID="hidFilter" runat="server" />
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow ID="AddAssignment" runat="server" Title="Add Assignment" Height="330px"
                Width="780px" ReloadOnShow="true" ShowContentDuringLoad="false" Skin="Hay" Modal="true"
                meta:resourcekey="UserListDialogResource1" VisibleStatusbar="false" VisibleTitlebar="false" />
            <telerik:RadWindow ID="EditAssignment" runat="server" Title="Edit Assignment" Height="350px"
                Width="580px" ReloadOnShow="true" ShowContentDuringLoad="false" Skin="Hay" Modal="true"
                meta:resourcekey="EditAssignmentResource1" VisibleStatusbar="false" VisibleTitlebar="false" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ShowInsertForm() {
                var url = "frmAssignment.aspx";
                try {
                    var cboFleet = $find("<%= cboFleet.ClientID %>");
                    if (cboFleet.get_selectedIndex() > 0) {
                        url = url + "?f=" + cboFleet.get_value();
                    }
                }
                catch (err) { }
                window.radopen(url, "AddAssignment");
                return false;
            }
            function refreshGrid(arg) {
                if (arg) {
                    $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("Rebind");
                }
            }

            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdVehicle.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=gdVehicle.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdVehicle.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }
        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
