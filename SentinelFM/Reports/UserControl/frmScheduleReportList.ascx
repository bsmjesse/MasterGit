<%@ Control Language="C#" AutoEventWireup="true" CodeFile="frmScheduleReportList.ascx.cs"
    Inherits="SentinelFM.Reports_UserControl_frmScheduleReportList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table width="98%">
    <tr>
        <td align="center">
            <telerik:RadToolBar runat="server" ID="RadToolBarSchedule" EnableRoundedCorners="True"
                EnableShadows="True" 
                OnClientButtonClicked="frmScheduleReportListToolBarClicked" 
                meta:resourcekey="RadToolBarScheduleResource1">
                <Items>
                    <telerik:RadToolBarButton Text="Scheduled Reports" CheckOnClick="true" meta:resourcekey="RadToolBarScheduleItemResource1"
                        Value="0" Checked="true" Group="Align">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton Text="Emailed/Stored Reports" CheckOnClick="true" Value="1"
                        Group="Align" meta:resourcekey="RadToolBarScheduleItemResource2">
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadGrid AutoGenerateColumns="False" ID="gdScheduleReports" PageSize="15"
                AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" runat="server"
                Skin="Hay" GridLines="None" meta:resourcekey="gdReportsResource1" FilterItemStyle-VerticalAlign="Top"
                OnInit="gdScheduleReports_Init" OnItemCreated="gdScheduleReports_ItemCreated" OnPreRender="Grid_PreRend"
                Width="99%">
                <MasterTableView DataKeyNames="ReportID" ClientDataKeyNames="ReportID">
                    <CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="ReportID" HeaderText='Report ID' meta:resourcekey="GridBoundColumnResource1" AllowFiltering="false">
                            <HeaderStyle Width="50px" />
                            <ItemStyle Width="50px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="GuiName" HeaderText='Report' meta:resourcekey="GridBoundColumnResource2" SortExpression="GuiName" UniqueName="GuiName">
                            <HeaderStyle Width="120px" />
                            <ItemStyle Width="110px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="DateFrom" HeaderText='From' DataFormatString="{0:MMM dd yyyy HH:mm}" meta:resourcekey="GridBoundColumnResource3">
                            <HeaderStyle Width="120px" />
                            <ItemStyle Width="110px" />
                        </telerik:GridDateTimeColumn>
                        <telerik:GridDateTimeColumn DataField="DateTo" HeaderText='To' DataFormatString="{0:MMM dd yyyy HH:mm}" meta:resourcekey="GridBoundColumnResource4">
                            <HeaderStyle Width="120px" />
                            <ItemStyle Width="110px" />
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="FleetName" HeaderText='Fleet' meta:resourcekey="GridBoundColumnResource5" DataType="System.String">
                            <HeaderStyle Width="60px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="StartScheduledDate" HeaderText='Scheduled Date' DataFormatString="{0:MMM dd yyyy HH:mm}" meta:resourcekey="GridBoundColumnResource6">
                            <HeaderStyle Width="120px" />
                            <ItemStyle Width="110px"  />
                        </telerik:GridDateTimeColumn>
                        <telerik:GridDateTimeColumn DataField="EndScheduledDate" HeaderText='End Scheduled' DataFormatString="{0:MMM dd yyyy HH:mm}" meta:resourcekey="GridBoundColumnResource7">
                            <HeaderStyle Width="120px" />
                            <ItemStyle Width="110px" />
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="Status" HeaderText='Status' meta:resourcekey="GridBoundColumnResource8" DataType="System.String" UniqueName="Status">
                            <ItemStyle Width="30px" />
                            <HeaderStyle Width="30px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DeliveryMethodType" HeaderText="Emailed To / Stored on Disk" meta:resourcekey="GridBoundColumnResource9">
                            <ItemStyle Width="100px" />
                            <HeaderStyle Width="100px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="UserName" HeaderText="User" FilterControlWidth ="100px" Visible="false">
                            <ItemStyle Width="110px" />
                            <HeaderStyle Width="100px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridHyperLinkColumn HeaderText="Edit" AllowFiltering="false" Visible="false" UniqueName="Edit" DataTextField="ReportID" meta:resourcekey="GridBoundColumnResource10">
                            <HeaderStyle Width="40px" />
                            <ItemStyle Width="40px" />
                        </telerik:GridHyperLinkColumn>
                        <telerik:GridHyperLinkColumn HeaderText="Delete" AllowFiltering="false" NavigateUrl="#" UniqueName="Delete" DataTextField="ReportID" meta:resourcekey="GridBoundColumnResource11">
                            <HeaderStyle Width="40px" />
                            <ItemStyle Width="40px" />
                        </telerik:GridHyperLinkColumn>
                    </Columns>
                </MasterTableView>
                <PagerStyle Mode="NextPrevAndNumeric" />
                <ClientSettings EnableRowHoverStyle="true">
                    <ClientEvents OnCommand="gdScheduleReports_Command" OnRowDataBound="gdScheduleReports_RowDataBound" />
                </ClientSettings>
                <HeaderStyle CssClass="RadGridtblHeader" />
            </telerik:RadGrid>
            <telerik:RadAjaxLoadingPanel ID="LoadingPanel1_Grid" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1_GridResource1" />
            <input type="hidden" id="hid_Schedule_ReportDelete" title="-1" />
            <input type="hidden" id="hid_Schedule_ReportRefresh" title="-1" />
            <asp:HiddenField ID="hid_Schedule_RunCount" runat="server" Value="-1" />
            <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
                <script type="text/javascript">
                    //<![CDATA[
                    //Bind Delete column
                    function checkScheduleReportsForDelete() {
                        var masterTable = $find("<%=gdScheduleReports.ClientID%>").get_masterTableView();
                        var count = masterTable.get_dataItems().length;
                        var item;
                        for (var i = 0; i < count; i++) {
                            item = masterTable.get_dataItems()[i];
                            var status = $telerik.$(masterTable.getCellByColumnUniqueName(item, "Status")).text();
                            try {
                                if (status.toLowerCase().indexOf("error") >= 0) $telerik.$(item.get_element()).css("color", "red");
                            } catch (err) { }

                            var key = item.getDataKeyValue("ReportID");

                            var Delete = masterTable.getCellByColumnUniqueName(item, "Delete");
                            $telerik.$(Delete).attr('tag', key);
                            $telerik.$(Delete).unbind();
                            $telerik.$(Delete).bind("click", function () { return DeleteScheduleReports(this) });
                            $telerik.$(Delete).html("<img src='../images/delete.gif'> </img> ");

                            var Edit = masterTable.getCellByColumnUniqueName(item, "Edit");
                            $telerik.$(Edit).attr('tag', key);
                            $telerik.$(Edit).unbind();
                            $telerik.$(Edit).bind("click", function () { return EditScheduleReports(this) });
                            $telerik.$(Edit).html("<img src='../images/Edit.gif'> </img> ");

                            var view = masterTable.getCellByColumnUniqueName(item, "GuiName");
                            $telerik.$(view).attr('tag', key);
                            $telerik.$(view).unbind();
                            $telerik.$(view).bind("click", function () { return frmScheduleReportListViewReport(this); }); //return ViewScheduleFile(this)
                            $telerik.$(view).html("<a href='#' >" + $telerik.$(view).text() + "</a>");

                        }
                    }

                    //View report detail
                    function frmScheduleReportListViewReport(ctl) {
                        var ReportID = $telerik.$(ctl).attr('tag');
                        $telerik.$('#<%= hid_ScheduleFile_ReportID.ClientID %>').val(ReportID);
                        //Have to run GetScheduleReportFileCount
                        $telerik.$('#<%= hid_ScheduleFile_RunCount.ClientID%>').val("-1");
                        rebindScheduleFileGrid(false);
                        var toolBar = $find("<%= RadToolBarSchedule.ClientID %>");
                        toolBar.findItemByValue("1").enable();
                        toolBar.findItemByValue("1").click();
                    }
                    //Delete record
                    function DeleteScheduleReports(ctl) {
                        //$telerik.$('#report_delete_img').remove();
                        //$telerik.$(ctl).parent().append("<img id='report_delete_img' src='Images/loading5.gif' />");
                        if (confirm('<%= GetLocalResourceObject("Text_ConfirmDelete")%>') == false) return
                        var id = -1;
                        var postData = "";
                        try {
                            id = $telerik.$(ctl).attr('tag');
                            //id = $telerik.$.trim(id);
                            postData = "{'ReportId':'" + id + "', 'PageName':'<%=  Page.GetType().Name %>'}";
                        }
                        catch (err) {}
                        $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= gdScheduleReports.ClientID %>");
                        $telerik.$.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: "ReportWebService.asmx/DeleteScheduledReportByID",
                            data: postData,
                            dataType: "json",
                            success: function (data) {
                                if (data.d == '<%= ReportWebService.return_fake_user  %>') {
                                    $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdScheduleReports.ClientID %>");
                                }
                                if (data.d == '<%= ReportWebService.return_no_login %>') {
                                    top.document.all('TopFrame').cols = '0,*';
                                    window.open('../Login.aspx', '_top')
                                }
                                if (data.d == '<%= ReportWebService.return_fail  %>') {
                                    alert('<%= GetLocalResourceObject("Deletefailed") %>');
                                    $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdScheduleReports.ClientID %>");
                                }
                                if (data.d == '<%= ReportWebService.return_success %>') {
                                    rebindScheduleGrid(true);
                                    alert("Deleted successfully");
                                    //$find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdScheduleReports.ClientID %>");
                                }

                            },
                            error: function (request, status, error) {
                                alert('<%= GetLocalResourceObject("Deletefailed") %>');
                                $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdScheduleReports.ClientID %>");
                            }

                        });
                        return false;
                    }

                    // Edit record
                    function EditScheduleReports(ctl) {
                        // Distroy current opened dialog window
                        window.radopen(null, "UserListDialog");
                        // Open a new dialog window
                        var url = "../ReportsScheduling/frmReportSchedulerEdit.aspx?ScheduleID=" + $telerik.$(ctl).attr('tag');
                        var oWnd = window.radopen(url, "UserListDialog");
                        oWnd.Center();
                    }

                    //Rebind the grid for delete and refresh
                    function rebindScheduleGrid(isForDelete) {
                        var masterTable = $find("<%=gdScheduleReports.ClientID%>").get_masterTableView();
                        var pageSize = masterTable.get_pageSize();

                        var sortExpressions = masterTable.get_sortExpressions();
                        var filterExpressions = masterTable.get_filterExpressions();

                        var currentPageIndex = masterTable.get_currentPageIndex();

                        var sortExpressionsAsSQL = sortExpressions.toString();
                        var filterExpressionsAsSQL = filterExpressions.toString();
                        var itemCount = 0;
                        if (isForDelete == true) {
                            itemCount = masterTable.get_virtualItemCount() - 1;
                            if (currentPageIndex == 0) {
                                masterTable.set_virtualItemCount(itemCount);
                                ReportWebService.GetScheduleReportData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateScheduleReportsGrid, frmScheduleReportList_loadfailed);
                            }
                            else {

                                if (currentPageIndex * pageSize >= itemCount) {
                                    if (currentPageIndex >= 1) {
                                        currentPageIndex = currentPageIndex - 1;
                                    }
                                }

                                $telerik.$('#hid_Schedule_ReportDelete').attr("title", currentPageIndex);
                                masterTable.set_virtualItemCount(itemCount);
                            }
                        }
                        else {
                            $telerik.$('#hid_Schedule_ReportRefresh').attr("title", currentPageIndex);
                            //alert($telerik.$('#hid_Schedule_ReportRefresh').attr("title", currentPageIndex));
                            //ReportWebService.GetReportScheduleReportsData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid_fresh);
                            masterTable.fireCommand("Page", "Prev");
                        }

                    }

                    function pageLoad_ScheduleReports(sender, eventArgs) {
                        $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= gdScheduleReports.ClientID %>");
                        var tableView = $find("<%= gdScheduleReports.ClientID %>").get_masterTableView();
                        ReportWebService.GetScheduleReportData(0, tableView.get_pageSize(),
                tableView.get_sortExpressions().toString(), tableView.get_filterExpressions().toList(),
                    updateScheduleReportsGrid, frmScheduleReportList_loadfailed);

                        ReportWebService.GetScheduleReportCount(tableView.get_filterExpressions().toList(), updateScheduleReportsVirtualItemCount, frmScheduleReportList_loadfailed);
                    }

                    function updateScheduleReportsGrid(result) {
                        if (result == null) {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top');
                        }
                        for (var index = 0; index < result.length; index++) {
                            var tmpDat = result[index]["DateFromStr"];
                            result[index]["DateFrom"] =
                            new Date(parseInt(tmpDat.substring(0, 4), 10),
                            parseInt(tmpDat.substring(4, 6), 10) - 1,
                            parseInt(tmpDat.substring(6, 8), 10),
                            parseInt(tmpDat.substring(8, 10), 10),
                            parseInt(tmpDat.substring(10, 12), 10), 0);

                            tmpDat = result[index]["DateToStr"];
                            result[index]["DateTo"] =
                            new Date(parseInt(tmpDat.substring(0, 4), 10),
                            parseInt(tmpDat.substring(4, 6), 10) - 1,
                            parseInt(tmpDat.substring(6, 8), 10),
                            parseInt(tmpDat.substring(8, 10), 10),
                            parseInt(tmpDat.substring(10, 12), 10), 0);

                            tmpDat = result[index]["StartScheduledDateStr"];
                            result[index]["StartScheduledDate"] =
                            new Date(parseInt(tmpDat.substring(0, 4), 10),
                            parseInt(tmpDat.substring(4, 6), 10) - 1,
                            parseInt(tmpDat.substring(6, 8), 10),
                            parseInt(tmpDat.substring(8, 10), 10),
                            parseInt(tmpDat.substring(10, 12), 10), 0);

                            tmpDat = result[index]["EndScheduledDateStr"];
                            result[index]["EndScheduledDate"] =
                            new Date(parseInt(tmpDat.substring(0, 4), 10),
                            parseInt(tmpDat.substring(4, 6), 10) - 1,
                            parseInt(tmpDat.substring(6, 8), 10),
                            parseInt(tmpDat.substring(8, 10), 10),
                            parseInt(tmpDat.substring(10, 12), 10), 0);

                        }

                        var tableView = $find("<%= gdScheduleReports.ClientID %>").get_masterTableView();
                        tableView.set_dataSource(result);
                        tableView.dataBind();
                        checkScheduleReportsForDelete();
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdScheduleReports.ClientID %>");
                    }

                    function frmScheduleReportList_loadfailed() {
                        alert("<%= LoadFailed%>");
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdScheduleReports.ClientID %>");
                    }

                    function updateScheduleReportsVirtualItemCount(result) {
                        var tableView = $find("<%= gdScheduleReports.ClientID %>").get_masterTableView();
                        tableView.set_virtualItemCount(result);
                        $telerik.$('#<%= hid_Schedule_RunCount.ClientID%>').val("0");
                    }

                    function gdScheduleReports_Command(sender, args) {
                        args.set_cancel(true);
                        var pageSize = sender.get_masterTableView().get_pageSize();

                        var sortExpressions = sender.get_masterTableView().get_sortExpressions();
                        var filterExpressions = sender.get_masterTableView().get_filterExpressions();
                        var isFresh = false;
                        //For delete
                        if ($telerik.$('#hid_Schedule_ReportDelete').attr('title') != "-1") {
                            var p_index = $telerik.$('#hid_Schedule_ReportDelete').attr('title');
                            $telerik.$('#hid_Schedule_ReportDelete').attr('title', "-1");
                            p_index = parseInt(p_index);
                            sender.get_masterTableView().set_currentPageIndex(p_index);
                        }

                        //For refresh
                        if ($telerik.$('#hid_Schedule_ReportRefresh').attr("title") != "-1") {
                            var p_index = $telerik.$('#hid_Schedule_ReportRefresh').attr('title');
                            $telerik.$('#hid_Schedule_ReportRefresh').attr('title', "-1");
                            p_index = parseInt(p_index);
                            sender.get_masterTableView().set_currentPageIndex(p_index);
                            isFresh = true;
                        }
                        $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= gdScheduleReports.ClientID %>");

                        var currentPageIndex = sender.get_masterTableView().get_currentPageIndex();
                        if (args.get_commandName() == "Filter")
                            currentPageIndex = 0;
                        var sortExpressionsAsSQL = sortExpressions.toString();
                        var filterExpressionsAsSQL = filterExpressions.toString();
                        if (isFresh == true) {
                            ReportWebService.GetScheduleReportData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateScheduleReportsGrid, frmScheduleReportList_loadfailed);
                            if ($telerik.$('#<%= hid_Schedule_RunCount.ClientID%>').val() == "-1")
                                ReportWebService.GetScheduleReportCount(filterExpressions.toList(), updateScheduleReportsVirtualItemCount, frmScheduleReportList_loadfailed);
                        }
                        else {
                            ReportWebService.GetScheduleReportData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateScheduleReportsGrid, frmScheduleReportList_loadfailed);
                        }
                        if (args.get_commandName() == "Filter") {
                            ReportWebService.GetScheduleReportCount(filterExpressions.toList(), updateScheduleReportsVirtualItemCount, frmScheduleReportList_loadfailed);
                        }
                    }
                    function gdScheduleReports_RowDataBound(sender, args) {
                        return;
                        //var radTextBox1 = args.get_item().findControl("LastName"); // find control
                        //radTextBox1.set_value(args.get_dataItem()["LastName"]);
                    }
                    function frmScheduleReportListToolBar() {
                        var toolBar = $find("<%= RadToolBarSchedule.ClientID %>");
                        var ReportID = $telerik.$('#<%= hid_ScheduleFile_ReportID.ClientID %>').val();
                        if (ReportID == '-1') {
                            $telerik.$("#<%= gdScheduleReports.ClientID %>").show();
                            $telerik.$("#<%= dgStoredreports.ClientID %>").hide();
                            toolBar.findItemByValue("0").set_checked(true);
                            toolBar.findItemByValue("1").disable();
                            
                        }
                        if ($find("<%= gdScheduleReports.ClientID %>").get_visible()) {
                            toolBar.findItemByValue("0").set_checked(true);
                            $telerik.$('#<%= hid_Schedule_RunCount.ClientID%>').val("-1");
                            rebindScheduleGrid(false);
                        }
                        else {
                            toolBar.findItemByValue("1").set_checked(true);
                            $telerik.$('#<%= hid_ScheduleFile_RunCount.ClientID%>').val("-1");
                            rebindScheduleFileGrid(false);
                        }
                    }
                    function frmScheduleReportListToolBarClicked(sender, args) {
                        var button = args.get_item();

                        if (button.get_value() == "1") {

                            $telerik.$("#<%= gdScheduleReports.ClientID %>").hide();

                            $telerik.$("#<%= dgStoredreports.ClientID %>").show();
                            //If have not run before
                            if ($telerik.$('#<%= hid_ScheduleFile_RunData.ClientID%>').val() == "-1") {
                                rebindScheduleFileGrid(false);
                            }

                        }
                        if (button.get_value() == "0") {
                            $telerik.$("#<%= gdScheduleReports.ClientID %>").show();

                            $telerik.$("#<%= dgStoredreports.ClientID %>").hide();
                        }

                    }

                    // Declare modal window
                    var modalWindow = {
                        parent: "body",
                        windowId: null,
                        content: null,
                        width: null,
                        height: null,
                        close: function () {
                            $(".modal-window").remove();
                            $(".modal-overlay").remove();
                        },
                        open: function () {
                            var modal = "";
                            modal += "<div class=\"modal-overlay\"></div>";
                            modal += "<div id=\"" + this.windowId + "\" class=\"modal-window\" style=\"width:" + this.width + "px; height:" + this.height + "px; margin-top:-" + (this.height / 2) + "px; margin-left:-" + (this.width / 2) + "px;\">";
                            modal += this.content;
                            modal += "</div>";
                            $(this.parent).append(modal);
                            $(".modal-window").append("<a class=\"close-window\"></a>");
                            $(".close-window").click(function () { modalWindow.close(); });
                            $(".modal-overlay").click(function () { modalWindow.close(); });
                        }
                    };

                    // Initial modal window
                    var openMyModal = function (source) {
                        modalWindow.windowId = "myModal";
                        modalWindow.width = 480;
                        modalWindow.height = 600;
                        modalWindow.content = "<iframe width='480' height='405' frameborder='0' scrolling='no' allowtransparency='true' src='" + source + "'></iframe>";
                        modalWindow.open();
                    };

                //]]>
                </script>
                <script id="Initial" type="text/javascript" language="javascript">
                </script>
            </telerik:RadCodeBlock>
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadGrid AutoGenerateColumns="False" ID="dgStoredreports" PageSize="15" Style="display: none"
                AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" runat="server"
                Skin="Hay" GridLines="None" meta:resourcekey="gdReportsResource1" FilterItemStyle-VerticalAlign="Top"
                OnInit="gdScheduleReports_Init" OnItemCreated="gdScheduleReports_ItemCreated" OnPreRender="Grid_PreRend"
                Width="99%">
                <MasterTableView DataKeyNames="RowID" ClientDataKeyNames="RowID">
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Report" DataField="GuiName" meta:resourcekey="GridBoundColumnResourceFile1"
                            UniqueName="GuiName">
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="DateCreated" DataFormatString="{0:MMM dd yyyy HH:mm}" FilterControlWidth ="100px"
                            HeaderText="Date Created" meta:resourcekey="GridBoundColumnResourceFile2">
                            <ItemStyle Width="250px" />
                            <HeaderStyle Width="250px" />
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="DeliveryMethod" Visible="false" HeaderText="DeliveryMethod Date"
                            meta:resourcekey="GridBoundColumnResourceFile3">
                        </telerik:GridBoundColumn>
                        <telerik:GridHyperLinkColumn HeaderText="View" meta:resourcekey="GridBoundColumnResourceFile4"
                            AllowFiltering="false" NavigateUrl="#" UniqueName="View" DataTextField="ReportFileName">
                            <ItemStyle Width="50px" />
                            <HeaderStyle Width="50px" />
                        </telerik:GridHyperLinkColumn>
                        <telerik:GridHyperLinkColumn HeaderText="" AllowFiltering="false" NavigateUrl="#" 
                            UniqueName="Delete" DataTextField="DeliveryMethod" meta:resourcekey="GridBoundColumnResourceFile5">
                        </telerik:GridHyperLinkColumn>
                    </Columns>
                </MasterTableView>
                <PagerStyle Mode="NextPrevAndNumeric" />
                <ClientSettings EnableRowHoverStyle="true">
                    <ClientEvents OnCommand="gdScheduleReportsFile_Command" OnRowDataBound="gdScheduleReportsFile_RowDataBound" />
                </ClientSettings>
                <HeaderStyle CssClass="RadGridtblHeader" />
            </telerik:RadGrid>
            <input type="hidden" id="hid_Schedule_ReportFileDelete" title="-1" />
            <input type="hidden" id="hid_Schedule_ReportFileRefresh" title="-1" />
            <asp:HiddenField ID="hid_ScheduleFile_RunCount" runat="server" Value="-1" />
            <asp:HiddenField ID="hid_ScheduleFile_ReportID" runat="server" Value="-1" />
            <asp:HiddenField ID="hid_ScheduleFile_RunData" runat="server" Value="-1" />
            <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
                <script type="text/javascript">
            //<![CDATA[
                    //*********************************For File Grid *********************/
                    //Bind Delete column
                    function checkScheduleReportsFileForDelete() {
                        var masterTable = $find("<%=dgStoredreports.ClientID%>").get_masterTableView();
                        var count = masterTable.get_dataItems().length;
                        var item;
                        for (var i = 0; i < count; i++) {
                            item = masterTable.get_dataItems()[i];
                            var Delete = masterTable.getCellByColumnUniqueName(item, "Delete");
                            var typeID = 0;
                            try {
                                if ($telerik.$.trim($telerik.$(Delete).text()) != '') typeID = parseFloat($telerik.$.trim($telerik.$(Delete).text()));
                            }
                            catch (err) { }
                            if (typeID != 0 && !isNaN(typeID)) {
                                var key = item.getDataKeyValue("RowID");
                                $telerik.$(Delete).attr('tag', key);
                                $telerik.$(Delete).unbind();
                                $telerik.$(Delete).bind("click", function () { return DeleteScheduleReportsFile(this) });
                                $telerik.$(Delete).html("<img src='../images/delete.gif'> </img> ");
                            }
                            else {
                                $telerik.$(Delete).text('');
                                $telerik.$(Delete).unbind();
                            }

                            var view = masterTable.getCellByColumnUniqueName(item, "View");
                            var file = $telerik.$.trim($telerik.$(view).text());
                            if ('https:' == document.location.protocol) file = file.replace('http://', 'https://');
                            if (file != null && file != '') {
                                var isPDF = false;
                                if (file.length > 4) {
                                    if (file.toLowerCase().substr(file.length - 4) == '.pdf') isPDF = true;
                                }

                                if (isPDF) {
                                    $telerik.$(view).attr('tag', file);
                                    $telerik.$(view).unbind();
                                    $telerik.$(view).bind("click", function () { return ViewScheduleReportsFile(this) }); //return ViewScheduleFile(this)
                                    $telerik.$(view).html("<a href='#' ><%= ViewText%> </a>");
                                }
                                else {
                                    $telerik.$(view).unbind();
                                    $telerik.$(view).html("<a href='" + file + "' ><%= DownloadText %> </a>");

                                }
                            }
                            else $telerik.$(view).html('');
                        }
                    }

                    function ViewScheduleReportsFile(ctl) {
                        var file = $telerik.$(ctl).attr('tag').toString();
                        file = file + "?#zoom=100";
                        var tabStrip = $find("<%= RadTabStripClientID %>");
                        tabStrip.findTabByValue("2").select();

                        $telerik.$('#ViewRepor_iframe_Report').attr("src", file);
                        $telerik.$('#<%= ViewRepor_Hidden_Msg %>').attr("value", file)


                        $telerik.$("#ViewReport_loading").css("display", "inline");
                        RefreshPage_ViewReport();
                        setTimeout(stateChangeFirefox, 10000)
                        return false;
                    }

                    //Delete record
                    function DeleteScheduleReportsFile(ctl) {
                        if (confirm('<%= GetLocalResourceObject("Text_ConfirmDelete")%>') == false) return
                        var id = -1;
                        var postData = "";
                        var ReportID = "-1";
                        try {
                            id = $telerik.$(ctl).attr('tag');
                            ReportID = $telerik.$('#<%= hid_ScheduleFile_ReportID.ClientID %>').val();
                            postData = "{'ReportId':'" + ReportID + "','RowId':'" + id + "', 'PageName':'<%=  Page.GetType().Name %>'}";
                        }
                        catch (error) { }
                        $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= dgStoredreports.ClientID %>");
                        $telerik.$.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: "ReportWebService.asmx/DeleteScheduledReportFileByID",
                            data: postData,
                            dataType: "json",
                            success: function (data) {
                                if (data.d == '<%= ReportWebService.return_fake_user  %>') {
                                    $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= dgStoredreports.ClientID %>");
                                }
                                if (data.d == '<%= ReportWebService.return_no_login %>') {
                                    top.document.all('TopFrame').cols = '0,*';
                                    window.open('../Login.aspx', '_top')
                                }
                                if (data.d == '<%= ReportWebService.return_fail  %>') {
                                    alert('<%= GetLocalResourceObject("Deletefailed") %>');
                                    $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= dgStoredreports.ClientID %>");
                                }
                                if (data.d == '<%= ReportWebService.return_success %>') {
                                    rebindScheduleFileGrid(true);
                                    //$find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= dgStoredreports.ClientID %>");
                                }

                            },
                            error: function (request, status, error) {
                                alert('<%= GetLocalResourceObject("Deletefailed") %>');
                                $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= dgStoredreports.ClientID %>");
                            }

                        });
                        return false;
                    }

                    //Rebind the grid for delete and refresh
                    function rebindScheduleFileGrid(isForDelete) {
                        var ReportID = $telerik.$('#<%= hid_ScheduleFile_ReportID.ClientID %>').val();
                        var masterTable = $find("<%=dgStoredreports.ClientID%>").get_masterTableView();
                        var pageSize = masterTable.get_pageSize();
                        var sortExpressions = masterTable.get_sortExpressions();
                        var filterExpressions = masterTable.get_filterExpressions();

                        var currentPageIndex = masterTable.get_currentPageIndex();

                        var sortExpressionsAsSQL = sortExpressions.toString();
                        var filterExpressionsAsSQL = filterExpressions.toString();
                        var itemCount = 0;
                        if (isForDelete == true) {
                            itemCount = masterTable.get_virtualItemCount() - 1;
                            if (currentPageIndex == 0) {
                                masterTable.set_virtualItemCount(itemCount);
                                ReportWebService.GetScheduleReportFileData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), ReportID, updateScheduleReportsFileGrid, frmScheduleReportListFile_loadfailed);
                            }
                            else {

                                if (currentPageIndex * pageSize >= itemCount) {
                                    if (currentPageIndex >= 1) {
                                        currentPageIndex = currentPageIndex - 1;
                                    }
                                }

                                $telerik.$('#hid_Schedule_ReportFileDelete').attr("title", currentPageIndex);
                                masterTable.set_virtualItemCount(itemCount);
                            }
                        }
                        else {
                            $telerik.$('#hid_Schedule_ReportFileRefresh').attr("title", currentPageIndex);
                            //alert($telerik.$('#hid_Schedule_ReportFileRefresh').attr("title", currentPageIndex));
                            //ReportWebService.GetReportScheduleReportsData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid_fresh);
                            masterTable.fireCommand("Page", "Prev");
                        }

                    }

                    function pageLoad_ScheduleReportsFile(sender, eventArgs) {
                        var ReportID = $telerik.$('#<%= hid_ScheduleFile_ReportID.ClientID %>').val();
                        $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= dgStoredreports.ClientID %>");
                        var tableView = $find("<%= dgStoredreports.ClientID %>").get_masterTableView();
                        ReportWebService.GetScheduleReportFileData(0, tableView.get_pageSize(),
                tableView.get_sortExpressions().toString(), tableView.get_filterExpressions().toList(), ReportID, updateScheduleReportsFileGrid, frmScheduleReportListFile_loadfailed);

                        ReportWebService.GetScheduleReportFileCount(tableView.get_filterExpressions().toList(), ReportID, updateScheduleReportsFileVirtualItemCount, frmScheduleReportListFile_loadfailed);
                    }

                    function updateScheduleReportsFileGrid(result) {
                        if (result == null) {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top');
                        }
                        var tableView = $find("<%= dgStoredreports.ClientID %>").get_masterTableView();
                        tableView.set_dataSource(result);
                        tableView.dataBind();
                        checkScheduleReportsFileForDelete();
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= dgStoredreports.ClientID %>");
                        $telerik.$('#<%= hid_ScheduleFile_RunData.ClientID%>').val("0");
                    }

                    function frmScheduleReportListFile_loadfailed() {
                        alert("<%= LoadFailed%>");
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= dgStoredreports.ClientID %>");
                    }

                    function updateScheduleReportsFileVirtualItemCount(result) {
                        var tableView = $find("<%= dgStoredreports.ClientID %>").get_masterTableView();
                        tableView.set_virtualItemCount(result);
                        $telerik.$('#<%= hid_ScheduleFile_RunCount.ClientID%>').val("0");
                    }

                    function gdScheduleReportsFile_Command(sender, args) {
                        var ReportID = $telerik.$('#<%= hid_ScheduleFile_ReportID.ClientID %>').val();
                        args.set_cancel(true);
                        var pageSize = sender.get_masterTableView().get_pageSize();
                        var sortExpressions = sender.get_masterTableView().get_sortExpressions();
                        var filterExpressions = sender.get_masterTableView().get_filterExpressions();
                        var isFresh = false;
                        //For delete
                        if ($telerik.$('#hid_Schedule_ReportFileDelete').attr('title') != "-1") {
                            var p_index = $telerik.$('#hid_Schedule_ReportFileDelete').attr('title');
                            $telerik.$('#hid_Schedule_ReportFileDelete').attr('title', "-1");
                            p_index = parseInt(p_index);
                            sender.get_masterTableView().set_currentPageIndex(p_index);
                        }

                        //For refresh
                        if ($telerik.$('#hid_Schedule_ReportFileRefresh').attr("title") != "-1") {
                            var p_index = $telerik.$('#hid_Schedule_ReportFileRefresh').attr('title');
                            $telerik.$('#hid_Schedule_ReportFileRefresh').attr('title', "-1");
                            p_index = parseInt(p_index);
                            sender.get_masterTableView().set_currentPageIndex(p_index);
                            isFresh = true;
                        }
                        $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= dgStoredreports.ClientID %>");

                        var currentPageIndex = sender.get_masterTableView().get_currentPageIndex();
                        if (args.get_commandName() == "Filter")
                            currentPageIndex = 0;
                        var sortExpressionsAsSQL = sortExpressions.toString();
                        var filterExpressionsAsSQL = filterExpressions.toString();
                        if (isFresh == true) {
                            ReportWebService.GetScheduleReportFileData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), ReportID, updateScheduleReportsFileGrid, frmScheduleReportListFile_loadfailed);
                            if ($telerik.$('#<%= hid_ScheduleFile_RunCount.ClientID%>').val() == "-1")
                                ReportWebService.GetScheduleReportFileCount(filterExpressions.toList(), ReportID, updateScheduleReportsFileVirtualItemCount, frmScheduleReportListFile_loadfailed);
                        }
                        else {
                            ReportWebService.GetScheduleReportFileData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), ReportID, updateScheduleReportsFileGrid, frmScheduleReportListFile_loadfailed);
                        }
                        if (args.get_commandName() == "Filter") {
                            ReportWebService.GetScheduleReportFileCount(filterExpressions.toList(), ReportID, updateScheduleReportsFileVirtualItemCount, frmScheduleReportListFile_loadfailed);
                        }
                    }
                    function gdScheduleReportsFile_RowDataBound(sender, args) {
                        return;
                        //var radTextBox1 = args.get_item().findControl("LastName"); // find control
                        //radTextBox1.set_value(args.get_dataItem()["LastName"]);
                    }

            //]]>            
                </script>
            </telerik:RadCodeBlock>
        </td>
    </tr>
</table>
