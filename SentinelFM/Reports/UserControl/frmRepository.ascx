<%@ Control Language="C#" AutoEventWireup="true" CodeFile="frmRepository.ascx.cs"
    Inherits="SentinelFM.Reports_UserControl_frmRepository" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<table style ="margin-top:0px; margin-bottom:10px">
   <tr style="height:20px">
      <td align ="center"   >
            <span    id="img_Reporsitory_loading" style="display:none; background-color:Green; color:White; font-weight:bold; width :120px; height:15px; font-size:14px "    ><%= RefreshingResource%> </span>
      </td>
   </tr>
   <tr>
      <td>
 
            <telerik:RadGrid AutoGenerateColumns="False" ID="gdRepository"  PageSize="15" 
                AllowFilteringByColumn="True" AllowPaging="True"  
                AllowSorting="True" runat="server" Skin="Hay" GridLines="None" 
                meta:resourcekey="gdRepositoryResource1" oninit="gdRepository_Init" 
                FilterItemStyle-VerticalAlign ="Top" 
                onitemcreated="gdScheduleReports_ItemCreated" 
                onprerender="gdRepository_PreRender" >
               
                <MasterTableView DataKeyNames="ReportRepositoryId" ClientDataKeyNames="ReportRepositoryId">
          
<CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
          
                    <Columns>
                    
                       <telerik:GridBoundColumn DataField ="GuiName" HeaderText ="Report Name" 
                            meta:resourcekey="GridBoundColumnResource1"  FilterControlWidth ="150px" >
                          <ItemStyle Width ="300px" />
                       </telerik:GridBoundColumn>
                       <telerik:GridBoundColumn DataField ="KeyValues" 
                            meta:resourcekey="GridBoundColumnKeyValues"  FilterControlWidth ="150px" >
                         
                       </telerik:GridBoundColumn>

                       <telerik:GridBoundColumn DataField ="Requested" HeaderText ="Require Date" 
                            DataFormatString="{0:M/d/yyyy HH:mm:ss}"
                            meta:resourcekey="GridBoundColumnResource2" >

                        <HeaderStyle Font-Underline="false" Width ="170px"/>
                        <FilterTemplate >
                        <telerik:RadPanelBar runat="server" ID="RadPanelRequired"  Width ="100%"  >
                          <Items>
                           <telerik:RadPanelItem   Text="<%$ Resources:FilterTextResource %>" Expanded="false" Width ="100%"  >
                                <Items >
                                   <telerik:RadPanelItem >
                                          <ItemTemplate   >
                                          <center>
                                              <table id="RepositoryRequiredFilter" style="border-width:0px; border-color:Green;" border="0px" width ="100%"  cellpadding ="0" cellspacing ="0" >
                            
                            <tr>
                             <td>
                          <asp:Label ID="lblRequiredFrom" runat ="server" Text="<%$ Resources:GridFilterFromSource %>"  ></asp:Label>
                             </td>
                             <td>
                            <telerik:RadDatePicker ID="FromRequiredPicker" runat="server" Width="95px"   Skin ="Hay"  Font-Size ="10px" 
                                 >
                                 <DateInput Culture="en-US" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass=""  />
                                 </telerik:RadDatePicker>
                             </td>
                             </tr>
                             <tr>
                             <td>
                            <asp:Label ID="lblRequiredTo" runat ="server" Text="<%$ Resources:GridFilterToSource %>" > </asp:Label>
                            </td>
                            <td>
                            <telerik:RadDatePicker ID="ToRequiredPicker" runat="server" Width="95px"  Skin ="Hay" Font-Size ="10px" 
                                 >
                                 <DateInput Culture="en-US" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" />
                                 </telerik:RadDatePicker>
                               </td>
                               </tr>
                               <tr>
                                  <td colspan ="2" align ="center" >
                                     <asp:Button id="btnRequiredFilter" Text="<%$ Resources:GridFilterSubmitSource %>"  runat ="server" OnClientClick ="return RepositoryRequiredFilter()" />
                                  </td>
                               </tr>
                               </table>
                                          </center>
                                          </ItemTemplate>
                                   </telerik:RadPanelItem>
                                </Items>
                           </telerik:RadPanelItem>
                           </Items>
                           </telerik:RadPanelBar>
                        <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">

                                <script type="text/javascript">
                                    function RepositoryRequiredFilter() {

                                        var tableView = $find("<%# ((GridItem)(Container.Parent)).OwnerTableView.ClientID %>");
                                        var FromPicker = $find('<%# ((RadPanelBar)(((GridFilteringItem)Container).FindControl("RadPanelRequired"))).Items[0].Items[0].FindControl("FromRequiredPicker").ClientID %>');
                                        var ToPicker = $find('<%# ((RadPanelBar)(((GridFilteringItem)Container).FindControl("RadPanelRequired"))).Items[0].Items[0].FindControl("ToRequiredPicker").ClientID %>');
                                        var fromDate = FromPicker.get_selectedDate();
                                        var toDate = ToPicker.get_selectedDate();

                                        var dateFromInput = FromPicker.get_dateInput();
                                        var dateToInput = ToPicker.get_dateInput();

                                        var fromDateStr = '<%= DateTimeFilterMin%>';
                                        var toDateStr = '<%=DateTimeFilterMax %>';
                                        if (fromDate != null) fromDateStr = dateFromInput.get_dateFormatInfo().FormatDate(fromDate, "yyyyMMdd");
                                        if (toDate != null) toDateStr = dateToInput.get_dateFormatInfo().FormatDate(toDate, "yyyyMMdd");
                                        tableView.filter("Requested", fromDateStr + "#" + toDateStr + "<%= DateTimeFilterFlage%>", "Between");
                                    }
                                </script>

                            </telerik:RadScriptBlock>
                        </FilterTemplate>
                       </telerik:GridBoundColumn>
                       <telerik:GridBoundColumn DataField ="Completed" HeaderText ="Completed Date" 
                            DataFormatString="{0:M/d/yyyy HH:mm:ss}" 
                            meta:resourcekey="GridBoundColumnResource3" >
                        <HeaderStyle Font-Underline="false" Width ="170px"/>
                        <FilterTemplate >
                        <telerik:RadPanelBar runat="server" ID="RadPanelCompleted"  Width ="100%" >
                          <Items>
                           <telerik:RadPanelItem   Text="<%$ Resources:FilterTextResource %>" Expanded="false" Width ="100%"  >
                                <Items >
                                   <telerik:RadPanelItem >
                                          <ItemTemplate   >
                                          <center>
                                              <table id="RepositoryCompletedFilter" style="border-width:0px; border-color:Green;" border="0px" width ="100%"  cellpadding ="0" cellspacing ="0" >
                            
                            <tr>
                             <td>
                          <asp:Label ID="lblcompletedFrom" runat ="server" Text="<%$ Resources:GridFilterFromSource %>" ></asp:Label>
                             </td>
                             <td>
                            <telerik:RadDatePicker ID="FromcompletedPicker" runat="server" Width="95px"   Skin ="Hay"  Font-Size ="10px" 
                                 >
                                 <DateInput Culture="en-US" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" />
                                 </telerik:RadDatePicker>
                             </td>
                             </tr>
                             <tr>
                             <td>
                            <asp:Label ID="lblcompletedTo" runat ="server" Text="<%$ Resources:GridFilterToSource %>" ></asp:Label>
                            </td>
                            <td>
                            <telerik:RadDatePicker ID="TocompletedPicker" runat="server" Width="95px"  Skin ="Hay" Font-Size ="10px" 
                                 >
                                 <DateInput Culture="en-US" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" />
                                 </telerik:RadDatePicker>
                               </td>
                               </tr>
                               <tr>
                                  <td colspan ="2" align ="center" >
                                     <asp:Button id="btncompletedFilter" Text="<%$ Resources:GridFilterSubmitSource %>" runat ="server" OnClientClick ="return RepositorycompletedFilter()" />
                                  </td>
                               </tr>
                               </table>
                                          </center>
                                          </ItemTemplate>
                                   </telerik:RadPanelItem>
                                </Items>
                           </telerik:RadPanelItem>
                           </Items>
                           </telerik:RadPanelBar>
                        <telerik:RadScriptBlock ID="RadScriptBlock2" runat="server">

                                <script type="text/javascript">

                                    function RepositorycompletedFilter() {

                                        var tableView = $find("<%# ((GridItem)(Container.Parent)).OwnerTableView.ClientID %>");
                                        var FromPicker = $find('<%# ((RadPanelBar)(((GridFilteringItem)Container).FindControl("RadPanelCompleted"))).Items[0].Items[0].FindControl("FromcompletedPicker").ClientID %>');
                                        var ToPicker = $find('<%# ((RadPanelBar)(((GridFilteringItem)Container).FindControl("RadPanelCompleted"))).Items[0].Items[0].FindControl("TocompletedPicker").ClientID %>');
                                        var fromDate = FromPicker.get_selectedDate();
                                        var toDate = ToPicker.get_selectedDate();

                                        var dateFromInput = FromPicker.get_dateInput();
                                        var dateToInput = ToPicker.get_dateInput();

                                        var fromDateStr = '<%= DateTimeFilterMin%>';
                                        var toDateStr = '<%=DateTimeFilterMax %>';
                                        if (fromDate != null) fromDateStr = dateFromInput.get_dateFormatInfo().FormatDate(fromDate, "yyyyMMdd");
                                        if (toDate != null) toDateStr = dateToInput.get_dateFormatInfo().FormatDate(toDate, "yyyyMMdd");
                                        tableView.filter("Completed", fromDateStr + "#" + toDateStr + "<%= DateTimeFilterFlage%>", "Between");
                                    }
                                </script>

                            </telerik:RadScriptBlock>
                        </FilterTemplate>

                       </telerik:GridBoundColumn>
                       <telerik:GridHyperLinkColumn HeaderText="View"  meta:resourcekey="GridBoundColumnResource4" AllowFiltering = "false"  NavigateUrl="#" UniqueName ="View"  SortExpression="Completed" DataTextField ="Path" >
                       </telerik:GridHyperLinkColumn>
                       <telerik:GridHyperLinkColumn HeaderText="Delete" meta:resourcekey="GridBoundColumnResource5" AllowFiltering = "false"  NavigateUrl="#" UniqueName ="Delete"  DataTextField ="ReportRepositoryId" >
                       </telerik:GridHyperLinkColumn>
                    </Columns>
                    <HeaderStyle CssClass="RadGridtblHeader" />
                </MasterTableView>
                <PagerStyle Mode="NextPrevAndNumeric" />
                <ClientSettings EnableRowHoverStyle="true">
                <ClientEvents OnCommand="gdRepository_Command" OnRowDataBound="gdRepository_RowDataBound" />
            </ClientSettings>

            </telerik:RadGrid>   
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1_Grid" runat="server" 
            Skin="Hay" meta:resourcekey="LoadingPanel1_GridResource1"  />
       <input type="hidden" id="hid_Reporsitory_ReportDelete"  title="-1" />            
       <input type="hidden" id="hid_Reporsitory_ReportRefresh"  title="-1" />            
       <asp:HiddenField  id="hid_Reporsitory_RunCount" runat ="server"  Value="-1" />      
      </td>
   </tr>
   
   

</table>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            //<![CDATA[
            //Bind View and Delete column
            function checkDataForView() {
                var masterTable = $find("<%=gdRepository.ClientID%>").get_masterTableView();
                var count = masterTable.get_dataItems().length;
                var item;
                for (var i = 0; i < count; i++) {
                    item = masterTable.get_dataItems()[i];
                    var key = item.getDataKeyValue("ReportRepositoryId");
                    var completed = masterTable.getCellByColumnUniqueName(item, "Completed")
                    var keyvalue = masterTable.getCellByColumnUniqueName(item, "KeyValues")
                    var view = masterTable.getCellByColumnUniqueName(item, "View");
                    cellValue = $telerik.$(completed).text();
                    var keyvalueData = $telerik.$(keyvalue).text();

                    var isTimeout = false;
                    try {
                        var requestedDate = $telerik.$(masterTable.getCellByColumnUniqueName(item, "Requested")).text();
                        var requestedDateArray = requestedDate.split('/');
                        var i_month = parseInt(requestedDateArray[0]);
                        var i_day = parseInt(requestedDateArray[1]);
                        var i_year = requestedDateArray[2].substring(0, 4);
                        var i_time = requestedDateArray[2].substring(5).split(':');
                        var i_hour = parseInt(i_time[0]);
                        var i_minute = parseInt(i_time[1]);
                        var i_second = parseInt(i_time[2]);

                        var requestDateTime = new Date(new Date(i_year, i_month - 1, i_day, i_hour, i_minute, i_second).getTime() + 86400000 * 7);
                        if (requestDateTime < new Date())
                            isTimeout = true
                    }
                    catch (er) { }


                    if (cellValue) cellValue = $telerik.$.trim(cellValue);
                    if (cellValue != '&nbsp;' && cellValue != '' && cellValue != null) {
                        if ($telerik.$.trim($telerik.$(view).text()) == '') {
                            $telerik.$(view).attr('tag', key);
                            //$telerik.$(view).text("<%= PendingText%>");
                            if (!isTimeout) {
                                if (keyvalueData.indexOf("NoData") > 0)
                                    $telerik.$(view).text("No Data");
                                else $telerik.$(view).text("<%= PendingText%>");
                            }
                            else $telerik.$(view).text("<%= TimeOutText%>");


                        }
                        else {

                            var isPDF = false;
                            var file = $telerik.$.trim($telerik.$(view).text());
                            if ('https:' == document.location.protocol) file = file.replace('http://', 'https://');
                            if (file.length > 4) {
                                if (file.toLowerCase().substr(file.length - 4) == '.pdf') isPDF = true;
                            }
                            $telerik.$(view).attr('key', key);
                            if (isPDF) {
                                $telerik.$(view).attr('tag', file);
                                $telerik.$(view).unbind();
                                $telerik.$(view).bind("click", function () { frmRepository_Call_UpdateRead(this); return ViewDoc(this) });
                                $telerik.$(view).html("<a href='#' ><%= ViewText%> </a>");
                            }
                            else {
                                $telerik.$(view).unbind();
                                $telerik.$(view).attr('file', file);
                                $telerik.$(view).bind("click", function () { frmRepository_Call_UpdateRead_download(this); return false; });
                                $telerik.$(view).html("<a href='#' ><%= DownloadText %> </a>");
                            }
                        }
                    }
                    else {
                        $telerik.$(view).attr('tag', key);
                        if (!isTimeout) {
                            if (keyvalueData.indexOf("NoData") > 0)
                                $telerik.$(view).text("No Data");
                            else   $telerik.$(view).text("<%= PendingText%>");
                        }
                        else $telerik.$(view).text("<%= TimeOutText%>");
                        //$telerik.$(view).text("<%= PendingText%>");
                    }

                    var Delete = masterTable.getCellByColumnUniqueName(item, "Delete");
                    $telerik.$(Delete).attr('tag', key);
                    $telerik.$(Delete).unbind();
                    $telerik.$(Delete).bind("click", function () { return DeleteReportRepository(this) });
                    $telerik.$(Delete).html("<img src='../images/delete.gif'> </img> ");
                }
            }
            function frmRepository_Call_UpdateRead_download(ctl) {
                try {
                    document.getElementById('iframedownload').src = $telerik.$(ctl).attr('file');
                    //window.location.href = $telerik.$(ctl).attr('file');
                    frmRepository_Call_UpdateRead(ctl);
                }
                catch (ex) { }
            }
            ///Ajax Call UpdateReportRepositoryReadStatusByID 
            function frmRepository_Call_UpdateRead(ctl) {
                try {
                        var messageID = $telerik.$(ctl).attr('key').toString();
                        var postData = "{'reportRepositoryID':'" + messageID + "'}";
                        $telerik.$.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: "ReportWebService.asmx/UpdateReportRepositoryReadStatusByID",
                            data: postData,
                            dataType: "json",
                            success: function (data) {
                                if (data.d == '<%= ReportWebService.return_no_login %>') {
                                    top.document.all('TopFrame').cols = '0,*';
                                    window.open('../Login.aspx', '_top')
                                }
                                if (data.d == '<%= ReportWebService.return_success %>') {
                                    parent.parent.frmMain_Top_Call_GetData();
                                }
                            },
                            error: function (request, status, error) {
                            }

                        });
                }
                catch (ex) { }
            }


            //View Report Document
            function ViewDoc(ctl) {
                var file = $telerik.$(ctl).attr('tag').toString();
                file = file + "?#zoom=100";
                if ('<%= isSecurity %>' == '1') file = file.replace("http://", "https://");
                var tabStrip = $find("<%= RadTabStripClientID %>");
                tabStrip.findTabByValue("2").select();

                $telerik.$('#ViewRepor_iframe_Report').attr("src", file);
                $telerik.$('#<%= ViewRepor_Hidden_Msg %>').attr("value", file)

                RefreshPage_ViewReport();
                $telerik.$("#ViewReport_loading").css("display","inline");
                setTimeout(stateChangeFirefox, 10000)
                return false;
            }

            //Delete record
            function DeleteReportRepository(ctl) {
                //$telerik.$('#report_delete_img').remove();
                //$telerik.$(ctl).parent().append("<img id='report_delete_img' src='Images/loading5.gif' />");
                $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= gdRepository.ClientID %>");
                var id = $telerik.$(ctl).attr('tag');
                id = parseInt(id);
                var postData = "{'ReportRepositoryID':'" + id + "', 'PageName':'<%=  Page.GetType().Name %>'}";

                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "ReportWebService.asmx/DeleteReportRepositoryByID",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d == '<%= ReportWebService.return_fake_user  %>') {
                            $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdRepository.ClientID %>");
                        }
                        if (data.d == '<%= ReportWebService.return_no_login %>') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '<%= ReportWebService.return_fail  %>') {
                            alert('<%= GetLocalResourceObject("Deletefailed") %>');
                            $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdRepository.ClientID %>");
                        }
                        if (data.d == '<%= ReportWebService.return_success %>') {
                            rebindGrid_call(true);
                            alert("Deleted successfully");
                            //$find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdRepository.ClientID %>");
                        }

                    },
                    error: function (request, status, error) {
                        alert('<%= GetLocalResourceObject("Deletefailed") %>');
                        $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdRepository.ClientID %>");
                        //alert(request.responseText);
                    }

                });
                return false;
            }

            function rebindGrid(isForDelete) {
                $telerik.$('#<%= hid_Reporsitory_RunCount.ClientID%>').val("-1");
                rebindGrid_call(isForDelete);
            }
            //Rebind the grid for delete and refresh
            function rebindGrid_call(isForDelete) {
                var masterTable = $find("<%=gdRepository.ClientID%>").get_masterTableView();
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
                        ReportWebService.GetReportRepositoryData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid, frmRepository_loadfailed);
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
                    $telerik.$('#img_Reporsitory_loading').fadeIn('slow');
                    $telerik.$('#hid_Reporsitory_ReportRefresh').attr("title", currentPageIndex);
                    //alert($telerik.$('#hid_Reporsitory_ReportRefresh').attr("title", currentPageIndex));
                    //ReportWebService.GetReportRepositoryData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid_fresh);
                    masterTable.fireCommand("Page", "Prev");
                }
                
            }

            function pageLoad_repository(sender, eventArgs) {
                $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= gdRepository.ClientID %>");
                var tableView = $find("<%= gdRepository.ClientID %>").get_masterTableView();
                ReportWebService.GetReportRepositoryData(0, tableView.get_pageSize(),
                tableView.get_sortExpressions().toString(), tableView.get_filterExpressions().toList(),
                    updateGrid, frmRepository_loadfailed);

                ReportWebService.GetReportRepositoryCount(tableView.get_filterExpressions().toList(), updateVirtualItemCount, frmRepository_loadfailed);
            }

            function updateGrid_fresh(result) {
                if (result == null) {
                    top.document.all('TopFrame').cols = '0,*';
                    window.open('../Login.aspx', '_top');
                }
                for (var index = 0; index < result.length; index++) {
                    var tmpDat = result[index]["RequestedStr"];
                    result[index]["Requested"] =
                            new Date(parseInt(tmpDat.substring(0, 4), 10),
                            parseInt(tmpDat.substring(4, 6), 10) - 1,
                            parseInt(tmpDat.substring(6, 8), 10),
                            parseInt(tmpDat.substring(8, 10), 10),
                            parseInt(tmpDat.substring(10, 12), 10), parseInt(tmpDat.substring(12, 14), 10));

                    tmpDat = result[index]["CompletedStr"];
                    if (tmpDat != "") {
                        result[index]["Completed"] =
                            new Date(parseInt(tmpDat.substring(0, 4), 10),
                            parseInt(tmpDat.substring(4, 6), 10) - 1,
                            parseInt(tmpDat.substring(6, 8), 10),
                            parseInt(tmpDat.substring(8, 10), 10),
                            parseInt(tmpDat.substring(10, 12), 10), parseInt(tmpDat.substring(12, 14), 10));
                    }
                }

                var tableView = $find("<%= gdRepository.ClientID %>").get_masterTableView();
                tableView.set_dataSource(result);
                tableView.dataBind();
                checkDataForView();
                $telerik.$('#img_Reporsitory_loading').fadeOut('slow');
            }

            function updateGrid(result) {
                if (result == null) {
                    top.document.all('TopFrame').cols = '0,*';
                    window.open('../Login.aspx', '_top');
                }

                var tableView = $find("<%= gdRepository.ClientID %>").get_masterTableView();
                tableView.set_dataSource(result);
                tableView.dataBind();
                checkDataForView();
                $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdRepository.ClientID %>");
            }

            function frmRepository_loadfailed() {
                alert("<%= LoadFailed%>");
                $find("<%= LoadingPanel1_Grid.ClientID %>").hide("<%= gdRepository.ClientID %>");
                $telerik.$('#img_Reporsitory_loading').fadeOut('slow');
            }

            function updateVirtualItemCount(result) {
                var tableView = $find("<%= gdRepository.ClientID %>").get_masterTableView();
                tableView.set_virtualItemCount(result);
                $telerik.$('#<%= hid_Reporsitory_RunCount.ClientID%>').val("0");
            }

            function gdRepository_Command(sender, args) {
                args.set_cancel(true);  
                var pageSize = sender.get_masterTableView().get_pageSize();

                var sortExpressions = sender.get_masterTableView().get_sortExpressions();
                var filterExpressions = sender.get_masterTableView().get_filterExpressions();
                var isShowloadingpanel = true;
                var isFresh = false;
                //For delete
                if ($telerik.$('#hid_Reporsitory_ReportDelete').attr('title') != "-1") {
                    var p_index = $telerik.$('#hid_Reporsitory_ReportDelete').attr('title');
                    $telerik.$('#hid_Reporsitory_ReportDelete').attr('title', "-1");
                    p_index = parseInt(p_index);
                    sender.get_masterTableView().set_currentPageIndex(p_index);
                    isShowloadingpanel = false;
                }

                //For refresh
                if ($telerik.$('#hid_Reporsitory_ReportRefresh').attr("title") != "-1") {
                    var p_index = $telerik.$('#hid_Reporsitory_ReportRefresh').attr('title');
                    $telerik.$('#hid_Reporsitory_ReportRefresh').attr('title', "-1");
                    p_index = parseInt(p_index);
                    sender.get_masterTableView().set_currentPageIndex(p_index);
                    isShowloadingpanel = false;
                    isFresh = true;
                }
                if (isShowloadingpanel == true) $find("<%= LoadingPanel1_Grid.ClientID %>").show("<%= gdRepository.ClientID %>");

                var currentPageIndex = sender.get_masterTableView().get_currentPageIndex();
                if (args.get_commandName() == "Filter")
                    currentPageIndex = 0;
                var sortExpressionsAsSQL = sortExpressions.toString();
                var filterExpressionsAsSQL = filterExpressions.toString();
                if (isFresh == true) {
                    ReportWebService.GetReportRepositoryData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid_fresh, frmRepository_loadfailed);
                    if ($telerik.$('#<%= hid_Reporsitory_RunCount.ClientID%>').val() == "-1" )
                        ReportWebService.GetReportRepositoryCount(filterExpressions.toList(), updateVirtualItemCount, frmRepository_loadfailed);
                }
                else {
                    ReportWebService.GetReportRepositoryData(currentPageIndex * pageSize, pageSize, sortExpressionsAsSQL, filterExpressions.toList(), updateGrid, frmRepository_loadfailed);
                }
                if (args.get_commandName() == "Filter") {
                    ReportWebService.GetReportRepositoryCount(filterExpressions.toList(), updateVirtualItemCount, frmRepository_loadfailed);
                }
            }
            function gdRepository_RowDataBound(sender, args) {
                return;
                //var radTextBox1 = args.get_item().findControl("LastName"); // find control
                //radTextBox1.set_value(args.get_dataItem()["LastName"]);
            }

            //]]>
        </script>

    </telerik:RadCodeBlock>

    <iframe src='' id='iframedownload' name='iframedownload' width="0" height="0"></iframe> 
