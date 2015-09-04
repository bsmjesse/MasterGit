<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHistDataGridExtended.aspx.cs"
    Inherits="SentinelFM.History_New_frmHistDataGridExtended" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        .RadGrid_Simple .rgHoveredRow td
        {
            border-color: #b7dbff #b7dbff !important;
        }
        .RadGrid_Simple .rgHoveredRow
        {
            background-color: #b7dbff !important;
            background: #b7dbff !important;
        }
        
        .FiltMenuCss
        {
            height: 90% !important;
            scrollbar-face-color: #B5C9DD;
            overflow-y: auto !important;
            overflow-x: hidden !important;
        }
        
        .GridButton a
        {
            font-size: 11px;
            margin: 1px;
            color: Blue !important;
        }
        
        .rgAdvPart
        {
            display: none;
        }
        .rgPager td
        {
            padding-top: 1px !important;
            padding-bottom: 1px !important;
        }
        
        .GridRowStyle td
        {
            padding-top: 1px !important;
            padding-bottom: 1px !important;
        }
        
        .radToolBarMenuCss ul li a span, .radToolBarMenuCss ul li span label
        {
            font-size: 11px !important;
            font-family: Verdana !important;
            color: White !important;
            text-decoration: underline !important;
            padding-top: 2px !important;
        }
    </style>
    <script language="JavaScript">
<!--
        var clientMapData;
        ns = (document.layers) ? true : false
        ie = (document.all) ? true : false
        var agt = navigator.userAgent.toLowerCase();

        function HistoryInfo(dgKey) {
            var mypage = '../frmHistDetails.aspx?dgKey=' + dgKey
            var myname = '';
            var w = 580;
            var h = 660;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=yes,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }




        function FullScreenGrid() {
            var mypage = '../../Map/frmBigDetailsFrame.htm'
            var myname = 'Data';
            var w = 900;
            var h = 700;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            return false;
        }

        function DrawHistoryMap() {
            try {
               if (clientMapData != null)
               parent.frmHisMap.ShowHistoryMap(clientMapData);
           } 
           catch(err){
               setTimeout("DrawHistoryMap()", 1000);
           }
        }

        function SensorsInfo(LicensePlate) {

            var mypage = '../../Map/frmSensorMain.aspx?LicensePlate=' + LicensePlate;
            var myname = 'Sensors';
            var w = 460;
            var h = 720;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            return false;
        }

        function HistorySearch() {
            var mypage = '../frmHistorySearch.aspx'
            var myname = 'HistorySearch';
            var w = 560;
            var h = 120;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function ResizeGrid() {
            location.href = "?clientWidth=" + document.body.clientWidth + "&clientHeight=" + document.body.clientHeight;
        }

//-->
    </script>
</head>
<body id="body" leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    <form id="frmHistory" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
 <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" OnAjaxRequest="RadAjaxManager1_AjaxRequest" ClientEvents-OnRequestStart="requestStart" ClientEvents-OnResponseEnd="responseEnd"
         EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboGridPaging">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgHistoryDetails" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dgStops" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dgTrips" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cmdMapSelected">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgHistoryDetails" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dgStops" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dgTrips" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="radContextExportMenu">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgHistoryDetails" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dgStops" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="dgTrips" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgHistoryDetails">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgHistoryDetails" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="dgStops">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgStops" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="dgTrips">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgTrips" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll"  />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <table id="Table1" style="left: 4px; width: 99%; height: 100%;" cellspacing="0" cellpadding="0"
        border="0">
        <tr>
            <td align="left">
                <table class="hbg" cellpadding="0" cellspacing="0"  width="100%">
                    <tr>
                        <td align="left">
                           <table>
                             <tr>
                        <td>
                            <asp:Button ID="cmdMapSelected" runat="server" CssClass="combutton" OnClientClick="return MapIt(this); "
                                Text="Map It" meta:resourcekey="cmdMapIt" Width="150px" />
                        </td>
                        <td>
                            <asp:Button ID="cmdVehicleGrid" runat="server" CssClass="combutton" OnClientClick="javascript:return FullScreenGrid()"
                                Text="Vehicle Grid" meta:resourcekey="cmdVehicleGrid" Width="150px" />
                        </td>
                        <td>
                            <asp:Button ID="cmdSendCommand" runat="server" CssClass="combutton" Text="Send Command" OnClientClick="return false;"
                                meta:resourcekey="cmdSendCommand" Width="150px" />
                        </td>
                        <td>
                            <asp:Button ID="cmdSearch" OnClientClick="javascript:HistorySearch();" runat="server"
                                CssClass="combutton" Text="Search" Width="150px" Visible="False" />
                        </td>

                        <td>
                            <asp:Button ID="cmdResizeGrid" runat="server" Text="Resize Grid" OnClientClick="javascript:ResizeGrid()"
                                CssClass="combutton" meta:resourcekey="cmdResizeGridResource1" Visible="False" />
                        </td>
                             </tr>
                           </table>
                        </td>

                                        <td align="right">
                                            <table>
                                                <tr>
                                                    <td valign="middle" >
                                                       <asp:LinkButton ID="imgExport" runat="server" OnClientClick ="javascript:return showExport(event);"  Text="Export" ForeColor="White" CssClass="RegularText" Font-Underline="true" meta:resourcekey="imgExportResource1" />
                                                    </td>

                                                    <td >
                                                        &nbsp;
                                                    </td>

                                                    <td valign="middle">
                            <asp:Label ID="lblPageSize" runat="server" CssClass="formtext" Text="Items per Page:" ForeColor="White"
                                meta:resourcekey="lblPageSizeResource1"></asp:Label>
                                                    </td>
                                                    <td valign="bottom">
                            <asp:DropDownList ID="cboGridPaging" runat="server" AutoPostBack="True" CssClass="RegularText"
                                OnSelectedIndexChanged="cboGridPaging_SelectedIndexChanged" meta:resourcekey="cboGridPagingResource1">
                                <asp:ListItem Value="999" meta:resourcekey="ListItemResource1">All</asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource2">15</asp:ListItem>
                                <asp:ListItem Selected="True" meta:resourcekey="ListItemResource3">25</asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource4">50</asp:ListItem>
                            </asp:DropDownList>

                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td align="right" valign="middle" >
                                                        <nobr>
                                <asp:ImageButton ID="imgFilter" runat="server" OnClientClick ="javascript:return showFilterItem();" ImageUrl="~/images/filter.gif" />
                                <asp:LinkButton ID="hplFilter" runat="server" CssClass="RegularText" ForeColor="White"  OnClientClick ="javascript:return showFilterItem();" Text="Show Filter" meta:resourcekey="hplFilterResource1" Font-Underline="true"></asp:LinkButton>
                                </nobr>
                                                    </td>
                                                    <td width="15px">
                                                       &nbsp;
                                                    </td>

                                                </tr>
                                            </table>
                                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <asp:Panel ID="pnlAll" runat="server" >
                <div id="HistoryGrid">
                    <telerik:RadGrid ID="dgHistoryDetails"  Width="100%" Style="width: 100% !important"
                        runat="server" Skin="Simple" AllowPaging="true" meta:resourcekey="dgHistoryDetailsResource2"
                        AllowSorting="True" AutoGenerateColumns="False" GridLines="Both" FilterItemStyle-HorizontalAlign="Left"
                        AllowFilteringByColumn="True" 
                        onneeddatasource="dgHistoryDetails_NeedDataSource" 
                        onitemdatabound="dgHistoryDetails_ItemDataBound"
                        OnInit="Grid_Init" 
                        onitemcreated="Grid_ItemCreated" onitemcommand="dgHistoryDetails_ItemCommand" 
                        >
                        <GroupingSettings CaseSensitive="false" />
                        <ExportSettings  OpenInNewWindow ="true" ExportOnlyData="true" IgnorePaging="true" ></ExportSettings>
                        <MasterTableView DataKeyNames="dgKey" ClientDataKeyNames="dgKey">
                            <CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
                            <Columns>
                                <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="selectCheckBox" meta:resourcekey="GridCheckBoxColumnResource1">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkSelectAllHistoryDetails" runat="server"></asp:CheckBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelectHistoryDetails" runat="server"></asp:CheckBox>
                                    </ItemTemplate>
                                    <ItemStyle Width="30px" HorizontalAlign="Center" />
                                    <HeaderStyle Width="30px" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_BoxId %>'
                                    DataField="BoxId" DataType="System.Int32" UniqueName="BoxId">
                                    <ItemStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Vehicle %>'
                                    DataField="Description" DataType="System.String" UniqueName="Description">

                                </telerik:GridBoundColumn>
                                <telerik:GridDateTimeColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_DateTime %>' DataField="OriginDateTime"  UniqueName="MyDateTime">
                                    <ItemStyle Width="160px" />
                                    <HeaderStyle Width="160px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Address %>'
                                    DataField="StreetAddress" UniqueName="StreetAddress">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Speed %>'
                                    DataField="Speed"  UniqueName="Speed" SortExpression="Speed_1" DataType="System.Double" >
                                    <ItemStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Speed %>' Visible="false"
                                    DataField="Speed_1"  UniqueName="Speed_1" SortExpression="Speed_1" DataType="System.Double" >
                                </telerik:GridBoundColumn>

                                <telerik:GridTemplateColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_MsgTypeName %>'
                                    DataField="BoxMsgInTypeName" UniqueName="BoxMsgInTypeName">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>'
                                            Text='<%# DataBinder.Eval(Container.DataItem, "BoxMsgInTypeName") %>' meta:resourcekey="HyperLink_SiteResource1"></asp:HyperLink>
                                    </ItemTemplate>
                                    <ItemStyle Width="140px" />
                                    <HeaderStyle Width="140px" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_MsgDetails %>'
                                    DataField="MsgDetails" UniqueName="MsgDetails">
                                    <ItemStyle Width="110px" />
                                    <HeaderStyle Width="110px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Ack %>'
                                    DataField="Acknowledged" UniqueName="Ack">
                                    <ItemStyle Width="80px" />
                                    <HeaderStyle Width="80px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Latitude" DataField="Latitude" UniqueName="Latitude"
                                    Visible="false">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Longitude" DataField="Longitude" UniqueName="Longitude"
                                    Visible="false">
                                </telerik:GridBoundColumn>
                            </Columns>
                            <HeaderStyle HorizontalAlign="Left" ForeColor="White" CssClass="RadGridtblHeader" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                            <AlternatingItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                        </MasterTableView>
                        <FilterItemStyle HorizontalAlign="Left"></FilterItemStyle>
                        <ClientSettings>
                            <Scrolling AllowScroll="true" UseStaticHeaders="true"  SaveScrollPosition="false" />
                            <ClientEvents OnGridCreated="GridCreated" OnColumnResized="ColumnResized" />
                            <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="True">
                            </Resizing>
                        </ClientSettings>
                        <FilterMenu CssClass="FiltMenuCss">
                        </FilterMenu>
                    </telerik:RadGrid>
                    <telerik:RadGrid ID="dgStops" runat="server" Width="100%" AllowSorting="True"
                        Style="width: 100% !important" AutoGenerateColumns="False" 
                        GridLines="Both" FilterItemStyle-HorizontalAlign="Left"
                        AllowFilteringByColumn="True" Skin="Simple" AllowPaging="true" 
                        meta:resourcekey="dgStopsResource2" onitemdatabound="dgStops_ItemDataBound" 
                        onneeddatasource="dgStops_NeedDataSource"
                        OnInit="Grid_Init" 
                        onitemcreated="Grid_ItemCreated"

                        >
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView DataKeyNames="StopIndex" ClientDataKeyNames="StopIndex">
                            <Columns>
                                <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="selectCheckBox" meta:resourcekey="GridCheckBoxColumnResource1">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkSelectAllStops" runat="server"></asp:CheckBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelectStops" runat="server"></asp:CheckBox>
                                    </ItemTemplate>
                                    <ItemStyle Width="30px" HorizontalAlign="Center" />
                                    <HeaderStyle Width="30px" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridDateTimeColumn HeaderText='<%$ Resources:dgStopsTitle_Arrival %>' DataField="ArrivalDateTime"
                                     UniqueName="ArrivalDateTime">
                                    <ItemStyle Width="155px" />
                                    <HeaderStyle Width="155px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgStopsTitle_Address %>' DataField="Location"
                                    UniqueName="Location">
                                </telerik:GridBoundColumn>
                                <telerik:GridDateTimeColumn HeaderText='<%$ Resources:dgStopsTitle_Departure %>' DataField="DepartureDateTime"
                                     UniqueName="DepartureDateTime">
                                    <ItemStyle Width="155px" />
                                    <HeaderStyle Width="155px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgStopsTitle_Duration %>' DataField="StopDuration"
                                    UniqueName="StopDuration">
                                    <ItemStyle Width="110px" />
                                    <HeaderStyle Width="110px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText='<%$ Resources:dgStopsTitle_Status %>' DataField="Remarks"
                                    UniqueName="Remarks">
                                    <ItemStyle Width="100px" />
                                    <HeaderStyle Width="100px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Latitude" DataField="Latitude" UniqueName="Latitude"
                                    Visible="False">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Longitude" DataField="Longitude" UniqueName="Longitude"
                                    Visible="False">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="StopDurationVal" DataField="StopDurationVal"
                                    UniqueName="StopDurationVal" Visible="False">
                                </telerik:GridBoundColumn>
                            </Columns>
                            <HeaderStyle HorizontalAlign="Left" ForeColor="White" CssClass="RadGridtblHeader" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                            <AlternatingItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                        </MasterTableView>
                        <FilterItemStyle HorizontalAlign="Left"></FilterItemStyle>
                        <ClientSettings>
                            <Scrolling AllowScroll="true" UseStaticHeaders="true"  SaveScrollPosition="false" />
                            <ClientEvents OnGridCreated="GridCreated" OnColumnResized="ColumnResized" />
                            <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="True">
                            </Resizing>
                        </ClientSettings>
                        <FilterMenu CssClass="FiltMenuCss">
                        </FilterMenu>
                    </telerik:RadGrid>
                    <telerik:RadGrid ID="dgTrips"  Width="100%" runat="server" Style="width: 100% !important"
                        AutoGenerateColumns="False" GridLines="Both" FilterItemStyle-HorizontalAlign="Left"
                        AllowFilteringByColumn="True" Skin="Simple" AllowPaging="true" 
                        onneeddatasource="dgTrips_NeedDataSource"
                        OnInit="Grid_Init" 
                        onitemcreated="Grid_ItemCreated" AutoGenerateHierarchy="true" ondetailtabledatabind="dgTrips_DetailTableDataBind"
                        OnItemDataBound="dgTrips__ItemDataBound"
                        >
                        <GroupingSettings CaseSensitive="false" />
                        <MasterTableView DataKeyNames="TripId" ClientDataKeyNames="TripId">
                            <Columns>
                                <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText='<%$ Resources:dgTrips_Description %>'>
                                    <ItemStyle Width="100px" />
                                    <HeaderStyle Width="100px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridDateTimeColumn DataField="DepartureTime" UniqueName="DepartureTime" HeaderText='<%$ Resources:dgTrips_Departure %>'  > 
                                    <ItemStyle Width="155px" />
                                    <HeaderStyle Width="155px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridDateTimeColumn DataField="ArrivalTime" UniqueName="ArrivalTime" HeaderText='<%$ Resources:dgTrips_Arrival %>' >
                                    <ItemStyle Width="155px" />
                                    <HeaderStyle Width="155px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridBoundColumn DataField="_From" UniqueName="_From" HeaderText='<%$ Resources:dgTrips_From %>' >
                                    <ItemStyle Width="155px" />
                                    <HeaderStyle Width="155px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="_To" UniqueName="_To" HeaderText='<%$ Resources:dgTrips_To %>' >
                                    <ItemStyle Width="155px" />
                                    <HeaderStyle Width="155px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="Duration" UniqueName="Duration" HeaderText='<%$ Resources:dgTrips_Duration %>'>
                                    <ItemStyle Width="100px" />
                                    <HeaderStyle Width="100px" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="FuelConsumed" UniqueName="FuelConsumed" HeaderText='<%$ Resources:dgTrips_FuelConsumed %>'>
                                    <ItemStyle Width="100px" />
                                    <HeaderStyle Width="100px" />
                                </telerik:GridBoundColumn>
                            </Columns>
                            <DetailTables >
                                <telerik:GridTableView   AllowSorting="True" AllowFilteringByColumn="false" Name="Trips"
                                    AutoGenerateColumns="False" GridLines="Both" SkinID="Simple"  >
                                    <Columns >
                                        <telerik:GridTemplateColumn Resizable="true" AllowFiltering="false" UniqueName="selectCheckBox"
                                            meta:resourcekey="GridCheckBoxColumnResource1">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkSelectAllTrips" runat="server" onclick="chkSelectAllTrips_Click(this)" ></asp:CheckBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelectTrips" runat="server"></asp:CheckBox>
                                                <asp:HiddenField ID="hiddgKey" runat="server" Value='<%# Bind("dgKey") %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="30px" HorizontalAlign="Center" />
                                            <HeaderStyle Width="30px" />
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_BoxId %>'
                                            DataField="BoxId" DataType="System.Int32" UniqueName="BoxId" Resizable="true">
                                            <ItemStyle Width="80px" />
                                            <HeaderStyle Width="80px" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Vehicle %>'
                                            DataField="Description" DataType="System.String" UniqueName="Description" Resizable="true">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridDateTimeColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_DateTime %>' DataFormatString="<%$ Resources:dgHistoryDetailsDateTimeFormat %>"
                                            DataField="OriginDateTime"  UniqueName="MyDateTime"
                                            Resizable="true">
                                            <ItemStyle Width="160px" />
                                            <HeaderStyle Width="160px" />
                                        </telerik:GridDateTimeColumn>
                                        <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Address %>'
                                            DataField="StreetAddress" UniqueName="StreetAddress" Resizable="true">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Speed %>'
                                            DataField="Speed" DataType="System.Double" UniqueName="Speed" Resizable="true" SortExpression="Speed_1" >
                                            <ItemStyle Width="80px" />
                                            <HeaderStyle Width="80px" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_MsgTypeName %>'
                                            DataField="BoxMsgInTypeName" UniqueName="BoxMsgInTypeName" Resizable="true">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>'
                                                    Text='<%# DataBinder.Eval(Container.DataItem, "BoxMsgInTypeName") %>' meta:resourcekey="HyperLink_SiteResource1"></asp:HyperLink>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_MsgDetails %>'
                                            DataField="MsgDetails" UniqueName="MsgDetails" Resizable="true">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn HeaderText='<%$ Resources:dgHistoryDetailsTitle_Ack %>'
                                            DataField="Acknowledged" UniqueName="Ack" Resizable="true">
                                            <ItemStyle Width="60px" />
                                            <HeaderStyle Width="60px" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn HeaderText="Latitude" DataField="Latitude" UniqueName="Latitude"
                                            Visible="false">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn HeaderText="Longitude" DataField="Longitude" UniqueName="Longitude"
                                            Visible="False">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                    <HeaderStyle HorizontalAlign="Left" ForeColor="White" CssClass="RadGridtblHeader" />
                                    <ItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                                    <AlternatingItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                                </telerik:GridTableView>
                            </DetailTables>

                            <HeaderStyle HorizontalAlign="Left" ForeColor="White" CssClass="RadGridtblHeader" />
                            <ItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                            <AlternatingItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                        </MasterTableView>
                        <FilterItemStyle HorizontalAlign="Left"></FilterItemStyle>
                        <ClientSettings>
                            <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="false" />
                            <ClientEvents OnGridCreated="GridCreated" OnColumnResized="ColumnResized" />
                            <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="True">
                            </Resizing>
                        </ClientSettings>
                        <FilterMenu CssClass="FiltMenuCss">
                        </FilterMenu>
                    </telerik:RadGrid>
                    <asp:HiddenField ID="hidFilter" runat="server" />
                </div>
                </asp:Panel>
            </td>
        </tr>
    </table>
 <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var CurrentGridClientID = "";
            var CurrentGridID = "";
            $telerik.$(window).resize(function () {
                SetVehicleGridHeight();
                setTimeout("SetGridFilterWidth()", 200);
            });

            function ColumnResized(sender, eventArgs) {
                SetVehicleGridHeight();
                setTimeout("SetGridFilterWidth()", 1000);

            }

            function SetVehicleGridHeight() {
                SetGridFilterWidth();
                var fleetHeight = $telerik.$("#toolBarControls").height();
                var headerHeight = $telerik.$(".rgHeaderDiv").height();
                var footHeight = $telerik.$(".rgPager").parent().height();
                $telerik.$(".rgDataDiv").height($telerik.$(window).height() - headerHeight - footHeight - fleetHeight - 30);

                if ($telerik.$(".rgDataDiv")[0].scrollHeight > $telerik.$(".rgDataDiv")[0].clientHeight) //check if scroll bar exists
                {
                    var scrollWidth = 20;
                    $telerik.$("table[id^='" + CurrentGridID + "']").each(function () {
                        var id = $telerik.$(this).attr("id").toLowerCase();
                        if (id.substring(id.length - 5, id.length) != "pager") {
                            if (id.indexOf('calendar') <= 0) {
                                var thisClass = $telerik.$(this).attr("class");
                                if (thisClass == null || thisClass.indexOf('rgDetailTable') < 0)
                                    $telerik.$(this).width($telerik.$(window).width() - scrollWidth);
                                else $telerik.$(this).width($telerik.$(window).width() - scrollWidth - 50);
                            }
                        }
                    })
                    $telerik.$(".rgHeaderDiv").width($telerik.$(window).width() - scrollWidth);
                }
                else {
                    $telerik.$("table[id^='" + CurrentGridID + "']").each(function (){
                        var id = $telerik.$(this).attr("id").toLowerCase();
                        if (id.indexOf('calendar') <= 0) {
                            var thisClass = $telerik.$(this).attr("class");
                            if (thisClass == null || thisClass.indexOf('rgDetailTable') < 0)
                                $telerik.$(this).width($telerik.$(window).width() - 3);
                            else $telerik.$(this).width($telerik.$(window).width() - 3 - 50);    
                      }
                    
                    });
                }
                $telerik.$("#" + CurrentGridClientID ).width($telerik.$(window).width());
                $telerik.$("#toolBarControls").width($telerik.$(window).width());
            }

            function SetGridFilterWidth() {
                $telerik.$(".rgFilterBox[type='text']").each(function () {
                    if ($telerik.$(this).css("visibility") != "hidden") {
                        var buttonWidth = 0;
                        if ($telerik.$(this).next("[type='submit']").length > 0) {
                            buttonWidth = $telerik.$(this).next("[type='submit']").width();
                        }
                        if ($telerik.$(this).next("[type='button']").length > 0) {
                            buttonWidth = $telerik.$(this).next("[type='button']").width();
                        }

                        if (buttonWidth > 0) {
                            $telerik.$(this).width($telerik.$(this).parent().width() - buttonWidth - 10);
                        }
                        else {
                            $telerik.$(this).width($telerik.$(this).parent().width() - 50);
                        }
                    }
                })

                //Calendar filter
                $telerik.$(".rgFilterRow").find("input[id$='_dateInput_text']").each(function () {
                    try {
                        var parentTd = $telerik.$(this).parents("table:first").parents("td:first");
                        if (parentTd.children(":first")[0].tagName.toLowerCase() != 'nobr') parentTd.wrapInner('<nobr />');
                    }
                    catch (err) { };
                }); 

            }


            function GridCreated() {
                if ($find("<%=dgHistoryDetails.ClientID%>")) {
                    CurrentGridClientID = "<%=dgHistoryDetails.ClientID%>";
                    CurrentGridID = "dgHistoryDetails";
                }
                if ($find("<%=dgStops.ClientID%>")) {
                    CurrentGridClientID = "<%=dgStops.ClientID%>";
                    CurrentGridID = "dgStops";
                }
                if ($find("<%=dgTrips.ClientID%>")) {
                    CurrentGridClientID = "<%=dgTrips.ClientID%>";
                    CurrentGridID = "dgTrips";
                }

                SetFilterWhenCreated();
                SetVehicleGridHeight();
                SetFiltMenuSeparatorLine();
            }



            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find(CurrentGridClientID).get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find(CurrentGridClientID).get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                SetVehicleGridHeight();
                return false;
            }

            function SetFilterWhenCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find(CurrentGridClientID).get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $find(CurrentGridClientID).get_masterTableView().showFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }



            function SetFiltMenuSeparatorLine() {
                $telerik.$(".FiltMenuCss").find("span.rmText").filter(function () {
                    return ($telerik.$(this).text() == '<%= HttpContext.GetGlobalResourceObject("Const", "RadGrid_ClearAllFilters").ToString() %>')
                }).parent().css("border-bottom", "1px solid gray");
            }

            function ShowErrorMessage(errMsg) {
                parent.parent.parent.ShowfrmShowMessage(errMsg);
            }

            function chkSelectAllHistoryDetails_Click(ctl) {
                var isChecked = $telerik.$(ctl).attr('checked');

                $telerik.$("#<%= dgHistoryDetails.ClientID %>").find("input:checkbox[id$='chkSelectHistoryDetails']").attr("checked", isChecked);
            }
            function chkSelectAllStops_Click(ctl) {
                var isChecked = $telerik.$(ctl).attr('checked');

                $telerik.$("#<%= dgStops.ClientID %>").find("input:checkbox[id$='chkSelectStops']").attr("checked", isChecked);
            }

            function chkSelectAllTrips_Click(ctl) {
                var isChecked = $telerik.$(ctl).attr('checked');
                $telerik.$(ctl).parents("table:first").find("input:checkbox[id$='chkSelectTrips']").attr("checked", isChecked);
            }
            
            function MapIt(ctl) {
                $telerik.$("#imgAjaxLoading_MapIt").remove();
                $telerik.$(ctl).after("<img id='imgAjaxLoading_MapIt' src='../../images/loading5.gif'>");
                $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("MapIt");
            }

            function showExport(e) {
                var contextMenu = $find("<%= radContextExportMenu.ClientID %>");
                contextMenu.show(e);
            }

            function itemClicked(sender, args) {
                var value = args.get_item().get_value();
                if (value == "Nothing") {
                    return false;
                }
                if (value == "ClearAllFilters") {
                    $telerik.$(".rgFilterBox[type='text']").val("");
                    $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("ClearAllFilters");
                    $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAll.ClientID %>");
                    $find("<%= LoadingPanel1.ClientID %>").show("<%= pnlAll.ClientID %>");
                }
            }

            function GenerateGridsDate(vehiclePlate) {
                $telerik.$("#<%= cmdSendCommand.ClientID %>").bind("click", function () {
                    eval("SensorsInfo('" + vehiclePlate + "')");
                    return false;
                });
                $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("GenerateGridsDate");
                $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAll.ClientID %>");
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnlAll.ClientID %>");
            }

            function requestStart(sender, args) {
                if (args.get_eventTarget().indexOf("radContextExportMenu") >= 0) {
                    args.set_enableAjax(false);
                }
            }

            function responseEnd(sender, eventArgs) {
                $telerik.$("#imgAjaxLoading_MapIt").remove();
                $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnlAll.ClientID %>");
            }

        </script>
    </telerik:RadCodeBlock>
        <Telerik:RadContextMenu id="radContextExportMenu" runat="server"  
        meta:resourcekey="RadContextExportMenuResource1" onitemclick="radContextExportMenu_ItemClick"
        >
                <Items>
                    <Telerik:RadMenuItem Text="Export To PDF" Value="pdf"  meta:resourcekey="RadContextExportMenuItemResource1" />
                    <Telerik:RadMenuItem Text="Export To Excel" value="excel" meta:resourcekey="RadContextExportMenuItemResource2" /> 
                    <Telerik:RadMenuItem Text="Export To Word" Value="word" meta:resourcekey="RadContextExportMenuItemResource3" />
                    <Telerik:RadMenuItem Text="Export To CSV" Value="csv"  meta:resourcekey="RadContextExportMenuItemResource4" />
                </Items>
    </Telerik:RadContextMenu>

    </form>
</body>
</html>
