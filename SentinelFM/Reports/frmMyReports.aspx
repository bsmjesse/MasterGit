<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMyReports.aspx.cs" Inherits="SentinelFM.Reports_frmMyReports" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="UserControl/frmRepository.ascx" TagName="frmRepository" TagPrefix="uc1" %>
<%@ Register Src="UserControl/ViewReport.ascx" TagName="ViewReport" TagPrefix="uc2" %>
<%@ Register src="UserControl/frmScheduleReportList.ascx" tagname="ViewReport" tagprefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Services>
            <asp:ServiceReference Path="ReportWebService.asmx" />
        </Services>
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" 
        meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1">
        <AjaxSettings>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <script type="text/javascript">

        function onTabSelected(sender, args) {
            if (args.get_tab().get_value() == "2") {
                RefreshPage_ViewReport();
            }
            if (args.get_tab().get_value() == "1") rebindGrid(false);
            if (args.get_tab().get_value() == "3") frmScheduleReportListToolBar();

            $telerik.$('#frmMyReports_modify').attr("src", "");
        }

        function onTabSelecting(sender, args) {
            if (args.get_tab().get_pageViewID()) {
                args.get_tab().set_postBack(false);
            }
        }

        function DateSelected(sender, args) {
        }  


    </script>
    <telerik:RadTabStrip ID="RadTabStrip1" SelectedIndex="0" runat="server" MultiPageID="RadMultiPage1"
        Skin="Hay" meta:resourcekey="RadTabStrip1Resource1" OnClientTabSelected="onTabSelected">
        <Tabs>
            <telerik:RadTab runat="server" Value="0" Text="Create Report" PageViewID="Report"
                meta:resourcekey="RadTabResource1">
            </telerik:RadTab>
            <telerik:RadTab runat="server" Value="1" Text="Repository" PageViewID="Repository"
                meta:resourcekey="RadTabResource2">
            </telerik:RadTab>
                <telerik:RadTab runat="server" Text="Scheduled Report" Value= "3" PageViewID="ScheduledReport" meta:resourcekey="RadTabResource4" >
                </telerik:RadTab>
            <telerik:RadTab runat="server" Value="2" Text="View" PageViewID="View" meta:resourcekey="RadTabResource3" Enabled ="false">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0"  CssClass="multiPage"
        meta:resourcekey="RadMultiPage1Resource1" >
        <telerik:RadPageView ID="Report" runat="server" PageViewID="Report" meta:resourcekey="RadPageView1Resource1" Height="92%"
            Selected="True">
            <center>
                <table width="95%">
                    <tr align="center" style="height: 40px;">
                        <td>
                            <telerik:RadComboBox ID="ddlReport" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged"
                                Skin="Hay" meta:resourcekey="ddlReportResource1">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Standard Reports" meta:resourcekey="ddlReportItemResource1"
                                        Value="0" runat="server" Owner=""></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Text="Extended Reports" Visible="False" meta:resourcekey="ddlReportItemResource2"
                                        Value="1" runat="server" Owner=""></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Text="My Reports" meta:resourcekey="ddlReportItemResource3"
                                        Value="2" Selected="True" runat="server" Owner=""></telerik:RadComboBoxItem>
                                </Items>
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr align="center">
                        <td align="center">
                            <asp:Panel ID="pnlMyReport" runat="server" Width="100%" 
                                meta:resourcekey="pnlMyReportResource1" >
                                <table>
                                    <tr>
                                        <td>
                                            <telerik:RadGrid runat="server" ID="gdMyReports" AllowPaging="True" AllowSorting="True"
                                                PageSize="15" Skin="Hay" AutoGenerateColumns="False"  
                                                AllowFilteringByColumn="True" GridLines="None" oninit="gdMyReports_Init" FilterItemStyle-VerticalAlign ="Top"
                                                meta:resourcekey="gdMyReportsResource1" onitemcreated="gdScheduleReports_ItemCreated" OnPreRender="Grid_PreRend"  >
                                                <ClientSettings EnableRowHoverStyle="True">
                                                    <ClientEvents OnCommand="gdMyReports_Command" OnRowDataBound="gdMyReports_RowDataBound" />
                                                </ClientSettings>
                                                <MasterTableView ClientDataKeyNames="UserReportId" DataKeyNames="UserReportId">
                                                    <CommandItemSettings ExportToPdfText="Export to Pdf" />
                                                    <Columns>
                                                        <telerik:GridBoundColumn DataField="Name" HeaderText="Name"  FilterControlWidth ="150px"
                                                            meta:resourceKey="GridBoundColumnResource1" UniqueName="Name">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description"   FilterControlWidth ="150px"
                                                            meta:resourceKey="GridBoundColumnResource2" UniqueName="Description">
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridDateTimeColumn AllowFiltering="true" DataField="DateFrom"  FilterControlWidth ="110px"
                                                            DataFormatString="{0:M/d/yyyy HH:mm}" HeaderText="From" 
                                                            meta:resourceKey="GridBoundColumnResource3" 
                                                            UniqueName="From">
                                                            <FilterTemplate></FilterTemplate>
                                                            <ItemStyle Width ="100px" />
                                                        </telerik:GridDateTimeColumn>
                                                        <telerik:GridDateTimeColumn AllowFiltering="true" AllowSorting="true" FilterControlWidth ="110px"
                                                            DataField="DateTo" DataFormatString="{0:M/d/yyyy HH:mm}" HeaderText="To" 
                                                            meta:resourceKey="GridBoundColumnResource4" UniqueName="To" >
                                                            <ItemStyle Width ="100px" />
                                                        </telerik:GridDateTimeColumn>
                                                        <telerik:GridHyperLinkColumn AllowFiltering="False"  
                                                            DataTextField="UserReportId" HeaderText="Execute" 
                                                            meta:resourceKey="GridBoundColumnResource5" NavigateUrl="#" 
                                                             UniqueName="Execute">
                                                        </telerik:GridHyperLinkColumn>
                                                        <telerik:GridHyperLinkColumn AllowFiltering="False"   
                                                            DataTextField="Category" HeaderText="Modify" 
                                                            meta:resourceKey="GridBoundColumnResource6" NavigateUrl="#" UniqueName="Modify">
                                                        </telerik:GridHyperLinkColumn>
                                                        <telerik:GridHyperLinkColumn AllowFiltering="False" 
                                                            DataTextField="UserReportId" HeaderText="Delete" 
                                                            meta:resourceKey="GridBoundColumnResource7" NavigateUrl="#" UniqueName="Delete">
                                                        </telerik:GridHyperLinkColumn>
                                                    </Columns>
                                                    <PagerStyle Mode="NextPrevAndNumeric" />
                                                        <HeaderStyle CssClass="RadGridtblHeader" />
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </center>
            <input type="hidden" id="hid_Reporsitory_ReportDelete"  title="-1" />            
        </telerik:RadPageView>
        <telerik:RadPageView ID="Repository" runat="server" PageViewID="Repository" meta:resourcekey="RadPageView2Resource1" Height="92%">
            <center>
                <uc1:frmRepository ID="frmRepository1" runat="server" />
            </center>
        </telerik:RadPageView>
        <telerik:RadPageView ID="View" runat="server" PageViewID="View" meta:resourcekey="RadPageView3Resource1">
            <center>
                <uc2:ViewReport ID="ViewReport1" runat="server" />
            </center>
        </telerik:RadPageView>

        <telerik:RadPageView ID="Modify" runat="server" PageViewID="Modify" meta:resourcekey="RadPageView4Resource1">
            <center>
                <table style="width :90%; height:90%">
                    <tr align ="center" style ="height:20px" > 
                     <td align="right"  width ="100px" ></td>
                      <td align ="center" width="80%"  >
                          <span    id="frmMyReports_loading" style=" background-color:Green; color:White; font-weight:bold; width :120px; height:15px; font-size:14px "    ><%= LoadingText %> </span>
                      </td>
                      <td align="center"   width ="100px" >
                           <a href ="#" onclick ="javascript:return Backto_Create()" ><%= BackTextResource%></a>
                      </td>
                    </tr>
                    <tr align ="center" valign="top">
                    <td colspan ="3">
                        <iframe id="frmMyReports_modify" src=""  style ="border-width:0px" onload="frmMyReports_closeIndicator()"    ></iframe>
                    </td>
                    </tr>
                </table>

                <script type="text/javascript">
                    //Refresh iframe window
                    function RefreshPage_frmMyReports_modify() {

                        var height = $telerik.$(document).height();
                        var width = $telerik.$(document).width();
                        $telerik.$('#frmMyReports_modify').height("95%")
                        $telerik.$('#frmMyReports_modify').width("100%");
                    }

                    //Iframe loaded event
                    function frmMyReports_closeIndicator() {

                        $telerik.$('#frmMyReports_loading').fadeOut();
                    }
                </script>

            </center>
        </telerik:RadPageView>
         <telerik:RadPageView ID="ScheduledReport" runat="server" PageViewID="ScheduledReport" meta:resourcekey="RadPageView5Resource1" Height="92%">
                <center>
                <uc3:ViewReport ID="ScheduleReport" runat="server"  />
                </center>
            </telerik:RadPageView>
    </telerik:RadMultiPage>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" 
        Behavior="Default" InitialBehavior="None" 
        meta:resourcekey="RadWindowManager1Resource1">
        <Windows>
            <telerik:RadWindow ID="UserListDialog" runat="server" Skin="Hay" DestroyOClose="true"
                ReloadOnShow="false" ShowContentDuringLoad="false" Modal="true" VisibleStatusbar="false"
                VisibleTitlebar="true" Animation="Fade" AnimationDuration="1000" 
                meta:resourcekey="UserListDialogResource1" />
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            //<![CDATA[
            //Bind Excute, Delete and Modify column
            function checkDataForView_myreport() {
                var masterTable = $find("<%=gdMyReports.ClientID%>").get_masterTableView();
                var count = masterTable.get_dataItems().length;
                var item;
                for (var i = 0; i < count; i++) {
                    item = masterTable.get_dataItems()[i];
                    var key = item.getDataKeyValue("UserReportId");

                    var executeReport = masterTable.getCellByColumnUniqueName(item, "Execute");
                    $telerik.$(executeReport).attr('tag', key);
                    $telerik.$(executeReport).unbind();
                    $telerik.$(executeReport).bind("click", function () { return Executemyreport(this) });
                    $telerik.$(executeReport).html("<a href='#' ><%= ExecuteText %></a>");

                    var modifyReport = masterTable.getCellByColumnUniqueName(item, "Modify");
                    var category = $telerik.$(modifyReport).text();
                    $telerik.$(modifyReport).attr('tag', key + ":" + category);
                    $telerik.$(modifyReport).unbind();
                    $telerik.$(modifyReport).bind("click", function () { return Modifymyreport(this) });
                    $telerik.$(modifyReport).html("<a href='#' ><%= ModifyText %></a>");

                    var DeleteReport = masterTable.getCellByColumnUniqueName(item, "Delete");
                    $telerik.$(DeleteReport).attr('tag', key);
                    $telerik.$(DeleteReport).unbind();
                    $telerik.$(DeleteReport).bind("click", function () { return Deletemyreport(this) });
                    $telerik.$(DeleteReport).html("<img src='../images/delete.gif'> </img> ");
                }
            }

            //Modify user Report
            function Modifymyreport(ctl) {
                var tag = $telerik.$(ctl).attr('tag');
                var strs = tag.split(":");
                var category = "";
                var id="";
                if (strs[1] != null) category = $telerik.$.trim(strs[1]);
                if (strs[0] != null) id = $telerik.$.trim(strs[0]);
                $telerik.$('#frmMyReports_modify').attr("src", "");
                if (category == "0") $telerik.$('#frmMyReports_modify').attr("src", "frmReports_new.aspx?id=" + id + "&isMyReport=1");
                else $telerik.$('#frmMyReports_modify').attr("src", "frmReportMasterExtended_new.aspx?id=" + id + "&isMyReport=1");
                var pageView = $find("<%= RadMultiPage1.ClientID %>").findPageViewByID("Modify").select();
                var tabStrip = $find("<%= RadTabStrip1.ClientID %>");
                tabStrip.findTabByValue("0").set_selected(false);
                RefreshPage_frmMyReports_modify();
                $telerik.$('#frmMyReports_loading').fadeIn();
            }
            //Execute Report
            function Executemyreport(ctl) {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= gdMyReports.ClientID %>");
                var id = $telerik.$(ctl).attr('tag');
                id = parseInt(id);
                var postData = "{'UserReportID':'" + id + "', 'PageName':'<%=  Page.GetType().Name %>'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "ReportWebService.asmx/ExcuteUserReport",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d == '<%= ReportWebService.return_fake_user  %>') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                        }

                        if (data.d == '<%= ReportWebService.return_no_login %>') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '<%= ReportWebService.return_fail  %>') {
                            alert('<%= GetLocalResourceObject("ExcuteFailed") %>');
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                        }
                        if (data.d == '<%= ReportWebService.return_success %>') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                            var tabStrip = $find("<%= RadTabStrip1.ClientID %>");
                            tabStrip.findTabByValue("1").select();
                        }

                    },
                    error: function (request, status, error) {
                        alert('<%= GetLocalResourceObject("ExcuteFailed") %>');
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                        //alert(request.responseText);
                    }

                });
                return false;
               
            }

            //Delete record
            function Deletemyreport(ctl) {
                //$telerik.$('#report_delete_img').remove();
                //$telerik.$(ctl).parent().append("<img id='report_delete_img' src='Images/loading5.gif' />");
                $find("<%= LoadingPanel1.ClientID %>").show("<%= gdMyReports.ClientID %>");
                var id = $telerik.$(ctl).attr('tag');
                id = parseInt(id);
                var postData = "{'UserReportID':'" + id + "', 'PageName':'<%=  Page.GetType().Name %>'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "ReportWebService.asmx/DeleteUserReportByID",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d == '<%= ReportWebService.return_fake_user  %>') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                        }
                        if (data.d == '<%= ReportWebService.return_no_login %>') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '<%= ReportWebService.return_fail  %>') {
                            alert('<%= GetLocalResourceObject("Deletefailed") %>');
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                        }
                        if (data.d == '<%= ReportWebService.return_success %>') {
                            rebindGrid_myreport(true);
                            //$find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                            alert("Deleted successfully");
                        }

                    },
                    error: function (request, status, error) {
                        alert('<%= GetLocalResourceObject("Deletefailed") %>');
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
                        alert(request.responseText);
                    }

                });
                return false;
            }

            //Rebind the grid for delete and refresh
            function rebindGrid_myreport(isForDelete) {
                var masterTable = $find("<%=gdMyReports.ClientID%>").get_masterTableView();
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
                        ReportWebService.GetUserReportData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid_myreport, frmMyReports_loadfailed);
                    }
                    else {

                        if (currentPageIndex * pageSize >= itemCount) {
                            if (currentPageIndex >= 1) {
                                currentPageIndex = currentPageIndex - 1;
                            }
                        }

                        $telerik.$('#hid_Reporsitory_ReportDelete').attr("title", currentPageIndex);
                        masterTable.set_virtualItemCount(itemCount);
                    }
                }
                else {
                    masterTable.fireCommand("Page", "Prev");
                }
            }

            //Click back in modidy user report page
            function Backto_Create() {
                var tabStrip = $find("<%= RadTabStrip1.ClientID %>");
                tabStrip.findTabByValue("0").select();
                return false;
            }

            //Click excute button in modify page
            function frmMyReports_Backto_Repository() {
                var tabStrip = $find("<%= RadTabStrip1.ClientID %>");
                tabStrip.findTabByValue("1").select();
                return false;
            }

            //Click excute and update button in modify page
            function frmMyReports_Backto_Repository_u() {
                var tabStrip = $find("<%= RadTabStrip1.ClientID %>");
                tabStrip.findTabByValue("1").select();
                rebindGrid_myreport(false);
                return false;
            }

            function updateGrid_myreport(result) {
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
                }

                var tableView = $find("<%= gdMyReports.ClientID %>").get_masterTableView();
                tableView.set_dataSource(result);
                tableView.dataBind();
                checkDataForView_myreport();
                $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
            }

            function frmMyReports_loadfailed() {
                alert("<%= LoadFailed%>");
                $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMyReports.ClientID %>");
            }

            function updateVirtualItemCount_myreport(result) {
                var tableView = $find("<%= gdMyReports.ClientID %>").get_masterTableView();
                tableView.set_virtualItemCount(result);
            }

            function gdMyReports_Command(sender, args) {
                args.set_cancel(true);
                var pageSize = sender.get_masterTableView().get_pageSize();

                var sortExpressions = sender.get_masterTableView().get_sortExpressions();
                var filterExpressions = sender.get_masterTableView().get_filterExpressions();
                var isShowloadingpanel = true;
                //For delete
                if ($telerik.$('#hid_Reporsitory_ReportDelete').attr('title') != "-1") {
                    var p_index = $telerik.$('#hid_Reporsitory_ReportDelete').attr('title');
                    $telerik.$('#hid_Reporsitory_ReportDelete').attr('title', "-1");
                    p_index = parseInt(p_index);
                    sender.get_masterTableView().set_currentPageIndex(p_index);
                    isShowloadingpanel = false;
                }

                if (isShowloadingpanel == true) $find("<%= LoadingPanel1.ClientID %>").show("<%= gdMyReports.ClientID %>");

                var currentPageIndex = sender.get_masterTableView().get_currentPageIndex();
                if (args.get_commandName() == "Filter")
                    currentPageIndex = 0;
                var sortExpressionsAsSQL = sortExpressions.toString();
                var filterExpressionsAsSQL = filterExpressions.toString();
                ReportWebService.GetUserReportData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid_myreport, frmMyReports_loadfailed);

                if (args.get_commandName() == "Filter") {
                    ReportWebService.GetUserReportCount(filterExpressions.toList(), updateVirtualItemCount_myreport, frmMyReports_loadfailed);
                }
            }
            function gdMyReports_RowDataBound(sender, args) {
                return;
                //var radTextBox1 = args.get_item().findControl("LastName"); // find control
                //radTextBox1.set_value(args.get_dataItem()["LastName"]);
            }

            function pageLoad(sender, eventArgs) {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= gdMyReports.ClientID %>");
                var tableView = $find("<%= gdMyReports.ClientID %>").get_masterTableView();
                ReportWebService.GetUserReportData(0, tableView.get_pageSize(),
                tableView.get_sortExpressions().toString(), tableView.get_filterExpressions().toList(),
                    updateGrid_myreport, frmMyReports_loadfailed);
                ReportWebService.GetUserReportCount(tableView.get_filterExpressions().toList(), updateVirtualItemCount_myreport, frmMyReports_loadfailed);
            }

            function frmMyReports_Login() {
                $telerik.$(location).attr('href', "../login.aspx");
            }

            //]]>
        </script>
    </telerik:RadCodeBlock>
    </form>
    <script type="text/javascript">
        //************************************************************************************************************
        //                                    Refresh repository page
        //************************************************************************************************************
        var reportIntervalID = setInterval(reflashRepository, 20000);
        function reflashRepository() {
            var pageView = $find("<%= RadMultiPage1.ClientID %>").findPageViewByID("Repository");
            if (pageView.get_selected() == true) { rebindGrid(false); };

        }

    </script>
</body>
</html>
