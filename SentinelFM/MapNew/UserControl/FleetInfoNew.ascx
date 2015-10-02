<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FleetInfoNew.ascx.cs" Inherits="SentinelFM.MapNew_UserControl_FleetInfoNew" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" 
        OnAjaxRequest="RadAjaxManager1_AjaxRequest" EnableAJAX="true" ClientEvents-OnRequestStart="requestStart">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="dgFleetInfo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgFleetInfo" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboGridPaging">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgFleetInfo" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="radContextExportMenu">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgFleetInfo" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgFleetInfo" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div>
        <table id="Table1" style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0"
            border="0">
            <tr>
                <td valign="top" align="left">
                    <table id="toolBarControls" width="100%" cellspacing="0" cellpadding="0" border="0">
                        <!-- ID important don't change -->
                        <tr>
                            <td>
                                <table cellspacing="0" width="100%" cellpadding="0" border="0" class="hbg">
                                    <tr>
                                        <td>
                                            <telerik:RadToolBar runat="server" ID="radToolBarMenu" EnableRoundedCorners="false"
                                                OnClientButtonClicked="radToolBarMenuButtonClicked" EnableShadows="false" SkinID="" Skin="" CssClass="radToolBarMenuCss"  >
                                                <Items>
                                                    <telerik:RadToolBarButton Text="barFleet">
                                                        <ItemTemplate>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="Label1"  Text="Fleet:" ForeColor="White" runat="server" CssClass="RegularText"
                                                                            meta:resourcekey="MenuItemResource3"></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="242px"
                                                                            DataTextField="FleetName" DataValueField="FleetId" BackColor="White" meta:resourcekey="cboFleetResource1">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ItemTemplate>
                                                    </telerik:RadToolBarButton>
                                                    <telerik:RadToolBarButton Text="Update Position" Value="UpdatePosition" 
                                                        meta:resourcekey="MenuItemResource4" >
                                                    </telerik:RadToolBarButton>
                                                    <telerik:RadToolBarButton Text="Map It" Value="MapIt" meta:resourcekey="MenuItemResource5">
                                                    </telerik:RadToolBarButton>
                                                    <telerik:RadToolBarButton Text="Search" Value="Search" meta:resourcekey="MenuItemResource6">
                                                    </telerik:RadToolBarButton>
                                                    <telerik:RadToolBarButton Value="AutoRefresh" >
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkAutoRefresh" runat="server" Text="Auto Refresh" Font-Underline="true" 
                                                                meta:resourcekey="MenuItemResource8" onclick="javascript:return chkAutoUpdateChanged(this)" />
                                                        </ItemTemplate>
                                                    </telerik:RadToolBarButton>
                                                    <telerik:RadToolBarButton Text="Full Screen Grid" Value="Full Screen Grid" 
                                                        meta:resourcekey="MenuItemResource11" NavigateUrl="javascript: FullScreenGrid()">
                                                    </telerik:RadToolBarButton>
                                                </Items>
                                            </telerik:RadToolBar>
                                        </td>
                                        <td valign="bottom">
                                            <table id="tblWait" style="display: none;">
                                                <tr>
                                                    <td align="center">
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="formtext">
                                                        <asp:Label ID="lblUpdatingPositionMessage" Font-Bold="true" runat="server" meta:resourcekey="lblUpdatingPositionMessageResource1"
                                                            ForeColor="White" Text="Updating Position..."></asp:Label>
                                                    </td>
                                                    <td align="center">
                                                        <asp:Image ID="imgWait" runat="server" Height="10px" ImageUrl="~/images/prgBar.gif"
                                                            meta:resourcekey="imgWaitResource1"></asp:Image>
                                                    </td>
                                                    <td style="width: 10px" align="center">
                                                        &nbsp;
                                                    </td>
                                                    <td align="center">
                                                        <asp:Button ID="cmdCancelUpdatePos" runat="server" CssClass="combutton" Text="Cancel"
                                                            OnClientClick="javascript:CancelUpdatePos();return false;" meta:resourcekey="cmdCancelUpdatePosResource1">
                                                        </asp:Button>
                                                    </td>
                                                    <td style="width: 10px" align="center">
                                                        &nbsp;
                                                    </td>
                                                    <td align="center">
                                                        <asp:Label ID="lblUpdatePosition" runat="server" CssClass="formtext" meta:resourcekey="lblUpdatePositionResource1"
                                                            ForeColor="White" Font-Bold="true"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="right">
                                            <table>
                                                <tr>
                                                    <td valign="middle" >
                                                       <asp:LinkButton ID="imgExport" runat="server" OnClientClick ="javascript:return showExport(event);"  Text="Export" ForeColor="White" CssClass="RegularText" Font-Underline="true"/>
                                                    </td>

                                                    <td >
                                                        &nbsp;
                                                    </td>

                                                    <td valign="middle">
                                                        <asp:Label ID="lblPageSize" runat="server" CssClass="RegularText" Text="Items per Page:"
                                                            meta:resourcekey="lblPageSizeResource1" ForeColor="White"></asp:Label>
                                                    </td>
                                                    <td valign="bottom">
                                                        <asp:DropDownList ID="cboGridPaging" CssClass="RegularText" runat="server" AutoPostBack="True"
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
                    </table>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <div id="divDetails">
                        <telerik:RadGrid ID="dgFleetInfo" runat="server" Width="100%" AllowSorting="True"
                            Style="width: 100% !important" AutoGenerateColumns="False" 
                            GridLines="Both" FilterItemStyle-HorizontalAlign="Left"
                            AllowFilteringByColumn="True" OnItemDataBound="dgFleetInfo_ItemDataBound" OnNeedDataSource="dgFleetInfo_NeedDataSource"
                            Skin="Simple" AllowPaging="true" meta:resourcekey="dgFleetInfoResource1" OnItemCommand="dgFleetInfo_ItemCommand"
                            OnSortCommand="dgFleetInfo_SortCommand" OnInit="dgFleetInfo_Init" 
                            onitemcreated="dgFleetInfo_ItemCreated" >
                             <GroupingSettings CaseSensitive="false" /> 
                            <ExportSettings  OpenInNewWindow ="true" ExportOnlyData="true" IgnorePaging="true" ></ExportSettings>
                            <MasterTableView DataKeyNames="VehicleId" ClientDataKeyNames="VehicleId" AllowMultiColumnSorting="true"  >
                                <PagerStyle Mode="NextPrevAndNumeric" />
                                <Columns>
                                    <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="selectCheckBox" meta:resourcekey="GridCheckBoxColumnResource1">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkSelectAllVehicles" runat="server"></asp:CheckBox>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelectVehicle" runat="server"></asp:CheckBox>
                                        </ItemTemplate>
                                        <ItemStyle Width="30px" HorizontalAlign="Center" />
                                        <HeaderStyle Width="30px" />
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn HeaderText='<%$ Resources:dgFleetInfo_BoxId %>' DataField="BoxId"
                                        DataType="System.Int32" UniqueName="BoxId" meta:resourcekey="GridBoundColumnResource1">
                                        <ItemStyle Width="100px" />
                                        <HeaderStyle Width="100px" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridHyperLinkColumn HeaderText='<%$ Resources:dgFleetInfo_Description %>'
                                        SortExpression="Description" UniqueName="Description" DataTextField="Description"
                                        meta:resourcekey="GridHyperLinkColumnResource1">
                                    </telerik:GridHyperLinkColumn>
                                    <telerik:GridBoundColumn HeaderText='<%$ Resources:dgFleetInfo_StreetAddress %>'
                                        DataField="StreetAddress" UniqueName="StreetAddress" meta:resourcekey="GridBoundColumnResource2">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridDateTimeColumn HeaderText='<%$ Resources:dgFleetInfo_MyDateTime %>'
                                        DataField="OriginDateTime" UniqueName="MyDateTime" DataType="System.DateTime"
                                        meta:resourcekey="GridBoundColumnResource3" PickerType="None">
                                        <ItemStyle Width="155px" />
                                        <HeaderStyle Width="155px" />
                                    </telerik:GridDateTimeColumn>
                                    <telerik:GridBoundColumn HeaderText='<%$ Resources:dgFleetInfo_CustomSpeed %>' DataField="CustomSpeed"
                                        UniqueName="CustomSpeed" meta:resourcekey="GridBoundColumnResource4">
                                        <ItemStyle Width="110px" />
                                        <HeaderStyle Width="110px" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridCheckBoxColumn HeaderText='<%$ Resources:dgFleetInfo_BoxArmed %>' DataField="BoxArmed"
                                        UniqueName="chkArmed" DataType="System.Boolean" ReadOnly="true" meta:resourcekey="GridCheckBoxColumnResource2">
                                        <ItemStyle Width="60px" />
                                        <HeaderStyle Width="60px" />
                                    </telerik:GridCheckBoxColumn>
                                    <telerik:GridBoundColumn HeaderText='<%$ Resources:dgFleetInfo_VehicleStatus %>'
                                        DataField="VehicleStatus" UniqueName="VehicleStatus" meta:resourcekey="GridBoundColumnResource5">
                                        <ItemStyle Width="100px" />
                                        <HeaderStyle Width="100px" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridCheckBoxColumn HeaderText="chkBoxShow" DataField="chkBoxShow" UniqueName="chkBoxShow"
                                        Visible="False" meta:resourcekey="GridCheckBoxColumnResource3">
                                    </telerik:GridCheckBoxColumn>
                                    <telerik:GridBoundColumn HeaderText="VehicleId" DataField="VehicleId" UniqueName="VehicleId"
                                        Visible="False" meta:resourcekey="GridBoundColumnResource6">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridCheckBoxColumn HeaderText="PTO" DataField="chkPTO" ReadOnly="true" DataType="System.Boolean"
                                        meta:resourcekey="GridCheckBoxColumnResource4">
                                        <ItemStyle Width="60px" />
                                        <HeaderStyle Width="60px" />
                                    </telerik:GridCheckBoxColumn>
                                    <telerik:GridBoundColumn HeaderText="LSD" UniqueName="LSD" DataField="LSD" DataType="System.String"
                                        meta:resourcekey="GridBoundColumnResource7">
                                        <ItemStyle Width="100px" />
                                        <HeaderStyle Width="100px" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridButtonColumn HeaderText='<%$ Resources:dgFleetInfo_ViewHistory %>' Display="true"
                                        Text='<%$ Resources:dgFleetInfo_ViewHistory %>' UniqueName="History" meta:resourcekey="GridButtonColumnResource1"
                                        CommandArgument="History" ButtonType="LinkButton">
                                        <ItemStyle Width="50px" HorizontalAlign="Center" CssClass="GridButton" />
                                        <HeaderStyle Width="50px" />
                                    </telerik:GridButtonColumn>
                                </Columns>
                                <HeaderStyle HorizontalAlign="Left" ForeColor="White" CssClass="RadGridtblHeader" />
                                <ItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                                <AlternatingItemStyle HorizontalAlign="Left" Font-Size="11px" Wrap="true" CssClass="GridRowStyle" />
                                <FooterStyle Font-Size="11px" />
                            </MasterTableView>
                            <FilterItemStyle HorizontalAlign="Left"></FilterItemStyle>
                            <ClientSettings>
                                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="200px" SaveScrollPosition="false" />
                                <ClientEvents OnGridCreated="GridCreated" OnColumnResized="ColumnResized" />
                                <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="True">
                                </Resizing>
                            </ClientSettings>
                            <FilterMenu CssClass="FiltMenuCss">
                            </FilterMenu>
                        </telerik:RadGrid>
                        <asp:HiddenField ID="hidFilter" runat="server" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
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
    
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var timeout_handles = null;
            var timeout_Update = null;
            function FullScreenGrid() {
                var mypage = '../Map/frmBigDetailsFrame.htm'
                var myname = 'Data';
                var w = screen.width - 20;
                var h = screen.height - 80;
                winprops = 'height=' + h + ',width=' + w + ',top=0,left=0,location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1'

                win = window.open(mypage, myname, winprops)
                if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            }

            function Search() {
                var mypage = '../Map/frmmapvehiclesearch.aspx'
                var w = 840;
                var h = 500;
                parent.parent.frmMain_Top_ViewWindow(mypage, w,h);
            }

            function SetReloadSetTimeOut() {
                AutoReloadDetails();
                timeout_handles = window.setTimeout('SetReloadSetTimeOut()',  <%=  AutoRefreshTimer%> );
            }

            function ClearReloadSetTimeOut()
            {
                if (timeout_handles != null) window.clearTimeout(timeout_handles);
            }

            function AutoReloadDetails() {
                $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("Rebind");
            }

		   function VehicleInfoWindow(VehicleId) { 
					var mypage='../Map/frmVehicleDescription.aspx?VehicleId='+VehicleId
					var myname='';
					var w=330;
					var h=450;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
                    parent.parent.frmMain_Top_ViewWindow(mypage, w,h);
					//winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					//win = window.open(mypage, myname, winprops) 
					//if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}

            function SensorsInfo(LicensePlate) {
                var mypage = '../Map/frmSensorMain.aspx?LicensePlate=' + LicensePlate
                var myname = 'Sensors';
                var w = 550;
                var h = 650;
                var winl = (screen.width - w) / 2;
                var wint = (screen.height - h) / 2;
                //winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
                //win = window.open(mypage, myname, winprops)
                parent.parent.frmMain_Top_ViewWindow(mypage, w,h);
                //if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            }
            $telerik.$(document).ready(function () {
                //Bind event
                $telerik.$("#<%= BarItemcboFleet.ClientID %>").bind("change", FleetSelectedIndxChanged);
                //Display error message
                if ('<%= ErrorMessage %>' != '' )
                {
                     parent.parent.ShowfrmShowMessage('<%= ErrorMessage %>');
                }
            });


            function FleetSelectedIndxChanged()
            {
                 $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("RebindSelectedFleet");
            }
            
            $telerik.$(window).resize(function () {
                SetVehicleGridHeight();
                setTimeout("SetGridFilterWidth()", 1000);
            });

            function ColumnResized(sender, eventArgs)
            {
                SetVehicleGridHeight();
                setTimeout("SetGridFilterWidth()", 1500);

            }

            function SetVehicleGridHeight() {
                //return;
                SetGridFilterWidth();
                //var scrollWidth = $telerik.$("#<%= dgFleetInfo.ClientID %>").width() - $telerik.$("table[id^='dgFleetInfo']").width();
                //if (scrollWidth <= 20) scrollWidth = 20;

                //$telerik.$("#<%= dgFleetInfo.ClientID %>").css("overflow", "hidden");
                var fleetHeight = $telerik.$("#toolBarControls").height();
                var headerHeight = $telerik.$(".rgHeaderDiv").height();
                var footHeight = $telerik.$(".rgPager").parent().height();
                var FleetInfoHeight = $telerik.$(window).height() - $telerik.$("#MiddleFrameTop").height();
                $telerik.$(".rgDataDiv").height(FleetInfoHeight - headerHeight - footHeight - fleetHeight);

                if ($telerik.$(".rgDataDiv")[0].scrollHeight > $telerik.$(".rgDataDiv")[0].clientHeight) //check if scroll bar exists
                {
                    var scrollWidth = 20;
                    $telerik.$("table[id^='dgFleetInfo']").each(function () {
                            var id = $telerik.$(this).attr("id").toLowerCase();
                            if (id.substring(id.length - 5, id.length) != "pager") {
                                $telerik.$(this).width($telerik.$(window).width() - scrollWidth);
                            }
                        })
                    $telerik.$(".rgHeaderDiv").width($telerik.$(window).width() - scrollWidth);
                }
                else $telerik.$("table[id^='dgFleetInfo']").width($telerik.$(window).width() - 3);
                $telerik.$("#<%= dgFleetInfo.ClientID %>").width($telerik.$(window).width());
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

            }


            function GridCreated() {
                //document.getElementById('<%= dgFleetInfo.ClientID%>_GridData').scrollTop = 0;
                SetFilterWhenCreated();
                SetVehicleGridHeight();
                SetFiltMenuSeparatorLine();
            }
             function chkSelectAllVehicles_Click() {
                  $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("SelectAllVehicles");
            }

            function chkSelectVehicle_Click(itemID, ctl) {
                var masterTable = $find("<%=dgFleetInfo.ClientID%>").get_masterTableView();
                var item = masterTable.get_dataItems()[itemID];
                var vehicleID = item.getDataKeyValue("VehicleId");

                var isChecked = $telerik.$(ctl).is(":checked");
                var fleetID = $telerik.$("#<%= BarItemcboFleet.ClientID %>").val();
                var postData = "{'fleetID':'" + fleetID + 
                               "', 'vehicleID':'" + vehicleID +
                               "', 'isChecked':'" + isChecked +
                               "'}";
                $telerik.$(ctl).hide();
                $telerik.$(ctl).parent().append("<img id='imgAjaxLoading_checkVehicle' src='../images/loading5.gif' width='15px' height = '15px' />");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmFleetInfoNew.aspx/SelectVehicle",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            $telerik.$(ctl).parent().find("#imgAjaxLoading_checkVehicle").remove();
                            $telerik.$(ctl).show();
                            if (isChecked == false) $telerik.$("input:checkbox[id$='chkSelectAllVehicles']").attr("checked", false);
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $telerik.$(ctl).parent().find("#imgAjaxLoading_checkVehicle").remove();
                            $telerik.$(ctl).show();
                            $telerik.$(ctl).attr("checked", !isChecked);
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $telerik.$(ctl).parent().find("#imgAjaxLoading_checkVehicle").remove();
                        $telerik.$(ctl).show();
                        $telerik.$(ctl).attr("checked", !isChecked);
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }

            function radToolBarMenuButtonClicked(sender, args)
            {
                if (args.get_item().get_value() == "MapIt") //Map it
                {
                    MapIt(args.get_item().get_element());
                }
                if (args.get_item().get_value() == "UpdatePosition") //UpdatePosition
                {
                    UpdatePosition(args.get_item().get_element());
                }
                if (args.get_item().get_value() == "LSD") //UpdatePosition
                {
                    LSD();
                }
                if (args.get_item().get_value() == "Search") //UpdatePosition
                {
                    Search();
                }
           }

            function MapIt(ctl)
            {
                var fleetID = $telerik.$("#<%= BarItemcboFleet.ClientID %>").val();
                if (fleetID == "-1" || fleetID == "") 
                {
                   alert('<%=  (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet")%>');
                   return;
                }
                var vehicleIDs = FindCheckedVehicles();
                var postData = "{'vehicleIDs':'," + vehicleIDs +  ",'}";
                $telerik.$("#imgAjaxLoading_MapIt").remove();
                $telerik.$(ctl).after("<img id='imgAjaxLoading_MapIt' src='../images/loading5.gif' width='15px' height = '15px' />");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmFleetInfoNew.aspx/MapIt",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            $telerik.$("#imgAjaxLoading_MapIt").remove();
                            if (vehicleIDs == '')
                            {
                                alert('<%=  (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle")%>');
                            }
                            else
                            {
                               if (data.d)
                               {
                                  var retData = eval(data.d);
                                  clientMapData = retData[0];
                                  if (retData.length == 2 )
                                  {
                                    if (retData[1] != '')
                                      parent.parent.ShowfrmShowMessage('<%=  (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle1") %>' 
                                                                              + ':' + retData[1]  +  
                                                                              '<%=  (string)base.GetLocalResourceObject("sn_MessageText_PositionInfoForVehicle2") %>');
                                  }
                                  ShowMultipleAssets(clientMapData);
                               }
                            }
                            //AutoReloadDetails();
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $telerik.$("#imgAjaxLoading_MapIt").remove();
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $telerik.$("#imgAjaxLoading_MapIt").remove();
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });

            }

            function UpdatePosition(ctl)
            {
                var fleetID = $telerik.$("#<%= BarItemcboFleet.ClientID %>").val();
                if (fleetID == "-1" || fleetID == "") 
                {
                   alert('<%=  (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet")%>');
                   return;
                }
                var vehicleIDs = FindCheckedVehicles();

                var postData = "{'vehicleIDs':'," + vehicleIDs +  ",'}";
                $telerik.$("#imgAjaxLoading_UpdatePosition").remove();
                $telerik.$(ctl).after("<img id='imgAjaxLoading_UpdatePosition' src='../images/loading5.gif' width='15px' height = '15px' />");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmFleetInfoNew.aspx/UpdatePosition",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            $telerik.$("#imgAjaxLoading_UpdatePosition").remove();
                            if (vehicleIDs == '')
                            {
                                alert('<%=  (string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle")%>');
                            }
                            else
                            {
                               if (data.d && data.d != "<%= NoShowString %>" )
                               {
                                  $find("<%=radToolBarMenu.ClientID %>").set_visible(false);
                                  $telerik.$("#tblWait").show();
                                  $telerik.$("#<%= lblUpdatePosition.ClientID %>").html(data.d);
                                  UpdatePositionResult();
                               }
                            }
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $telerik.$("#imgAjaxLoading_UpdatePosition").remove();
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $telerik.$("#imgAjaxLoading_UpdatePosition").remove();
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });

            }

            function FindCheckedVehicles()
            {
                var masterTable = $find("<%=dgFleetInfo.ClientID%>").get_masterTableView();
                var count = masterTable.get_dataItems().length;
                var item;
                var vehicleIDs = '';
                for (var i = 0; i < count; i++) {
                    item = masterTable.get_dataItems()[i];
                    var key = item.getDataKeyValue("VehicleId");
                    var cell = masterTable.getCellByColumnUniqueName(item, "selectCheckBox")
                    //var checkBox = cell.getElementsByTagName("INPUT")[0]; 
                    var checkBox = $telerik.$(cell).find("[name$='chkSelectVehicle']")[0];
                    if (checkBox && checkBox.checked)  
                    {
                            if (vehicleIDs == '') vehicleIDs = key;
                            else vehicleIDs = vehicleIDs + "," + key;
                     }
                 }
                 return vehicleIDs;
            }

            //called from frmtimerposition.aspx
            function UpdatePositionResult()
            {
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmFleetInfoNew.aspx/UpdatePositionResult",
                    data: null,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                           if (data.d.length >= 2)
                           {
                              if (data.d.substring(0,2) == '<%= start_characters_begin %>')
                              {
                                 window.setTimeout('UpdatePositionResult()',data.d.substring(2) );
                              }
                              else
                              { 
                                 window.clearTimeout();
                                 $telerik.$("#tblWait").hide();
                                 $find("<%=radToolBarMenu.ClientID %>").set_visible(true);
                                 RebindGridAndMap();

                                 if (data.d.length > 2)
                                 parent.parent.ShowfrmShowMessage(data.d.substring(2));
                              }
                           }
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        window.clearTimeout();
                        $telerik.$("#tblWait").hide();
                        $find("<%=radToolBarMenu.ClientID %>").set_visible(true);

                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }

            function CancelUpdatePos()
            {
                var ctl = "#<%= cmdCancelUpdatePos.ClientID %>"
                $telerik.$("#imgAjaxLoading_CancelUpdatePos").remove();
                $telerik.$(ctl).after("<img id='imgAjaxLoading_CancelUpdatePos' src='../images/loading5.gif' width='15px' height = '15px' />");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmFleetInfoNew.aspx/CancelUpdatePos",
                    data: null,
                    dataType: "json",
                    success: function (data) {
                        $telerik.$("#imgAjaxLoading_CancelUpdatePos").remove();
                        if (data.d != '-1' && data.d != "0") {
                           $telerik.$("#tblWait").hide();
                           $find("<%=radToolBarMenu.ClientID %>").set_visible(true);
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= errorCancel%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $telerik.$("#imgAjaxLoading_CancelUpdatePos").remove();
                        alert("<%= errorCancel%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
            }


            function chkAutoUpdateChanged(ctl)
            {
               var isChecked = $telerik.$(ctl).is(":checked");
               if (isChecked )
               {
                    var fleetID = $telerik.$("#<%= BarItemcboFleet.ClientID %>").val();
                    if (fleetID == "-1" || fleetID == "") 
                    {
                       alert('<%=  (string)base.GetLocalResourceObject("sn_MessageText_SelectFleet")%>');
                       ClearReloadSetTimeOut();
                       return false;
                    }
               }
               $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("AutoRefresh");
               return true;
            }

            function LSD()
            {
                $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("LSD");
            }

            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgFleetInfo.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=dgFleetInfo.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                SetVehicleGridHeight();
                return false;
            }

            function SetFilterWhenCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgFleetInfo.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $find('<%=dgFleetInfo.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }

            function History_Click(itemID, ctl) {
                var masterTable = $find("<%=dgFleetInfo.ClientID%>").get_masterTableView();
                var item = masterTable.get_dataItems()[itemID];
                var vehicleID = item.getDataKeyValue("VehicleId");
                var vehicleIDs = FindCheckedVehicles();
                var postData = "{'vehicleID':'" + vehicleID + 
                               "', 'vehicleIDs':'" + vehicleIDs + 
                               "'}";
                $telerik.$(ctl).hide();
                $telerik.$(ctl).parent().append("<img id='imgAjaxLoading_history' src='../images/loading5.gif' width='15px' height = '15px' />");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmFleetInfoNew.aspx/History",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            parent.main.window.location='../History/frmhistmain.aspx';
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $telerik.$(ctl).parent().find("#imgAjaxLoading_history").remove();
                            $telerik.$(ctl).show();
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $telerik.$(ctl).parent().find("#imgAjaxLoading_history").remove();
                        $telerik.$(ctl).show();
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
                return false;
            }

            function itemClicked(sender, args) {
                    var value  = args.get_item().get_value();
                    if (value == "Nothing")
                    {
                        return false;
                    }
                    if (value == "ClearAllFilters")
                    {
                        $telerik.$(".rgFilterBox[type='text']").val("");                
                        $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("ClearAllFilters");
                        
                        //$get("hidFilterStatus").value = args.get_item().get_attributes().getAttribute("realCommand");
                    }
             }

             function SetFiltMenuSeparatorLine()
             {
                $telerik.$(".FiltMenuCss").find("span.rmText").filter(function (){
                    return ($telerik.$(this).text()== '<%= HttpContext.GetGlobalResourceObject("Const", "RadGrid_ClearAllFilters").ToString() %>')
                }).parent().css("border-bottom","1px solid gray");
             }

             function showExport(e)
             {
                 var contextMenu = $find("<%= radContextExportMenu.ClientID %>");
                 contextMenu.show(e);
             }
             
             function RebindGridAndMap()
             {
                 $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("RebindGridAndMap");
             }

             function requestStart(sender, args)
             {
                 if (args.get_eventTarget().indexOf("radContextExportMenu") >= 0)
                 {
                     args.set_enableAjax(false);
                 }
             }

        </script>
    </telerik:RadCodeBlock>
