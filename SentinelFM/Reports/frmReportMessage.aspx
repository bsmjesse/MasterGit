<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportMessage.aspx.cs" Inherits="Reports_frmReportMessage" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" >
        html, body
        {
            margin: 0;
            padding: 0;
            width:292px;
            height: 162px;
            background-color:White ;
        }
        #frmReportMessage_winpop
        {
            height: 162px;
        }
        #frmReportMessage_winpop .title
        {
            height: 20px;
            line-height: 18px;
            background: Green;
            font-weight: bold;
            text-align: center;
            font-size: 12px;
            color:White;
        }
         #frmReportMessage_winpop .title a
         {
             color:White;
         }
        .close
        {
            position: absolute;
            right: 18px;
            color: #FFF;
            cursor: pointer;
        }
    </style>
</head>
<body style="background :white">
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
     <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1">
     </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1_GridResource1" />
    <div style ="vertical-align:top; " >
    <center>
    <table id="frmReportMessage_winpop" >
       <tr valign ="top" >
         <td>
           <div class="title" style="background-color:<%= TitleBackColor%>"> <a id="frmReportMessage_btn"  href ="#" onclick ="return frmReportMessage_readAllClick()"  ><%= ReadingText %></a>
                   <span class="close" onclick="javascript:parent.frmMain_Top_Msg_pop()">X</span></div>
        </td>
       </tr>
       <tr>
           <td align ="center">
             <telerik:RadGrid AutoGenerateColumns="False" ID="gdReportsMessage" runat="server"
                Skin="Hay"
                Width="280px" ShowHeader ="False"  Font-Size ="10px" BorderWidth ="0px"  >
                <MasterTableView >
                    <CommandItemSettings ExportToPdfText="Export to Pdf" />
                    <Columns>
                        <telerik:GridBoundColumn DataField="MessageName"  
                             UniqueName="MessageName">
                             <ItemStyle Width = "130px"   VerticalAlign ="Top"  Font-Size="11px" />
                             
                        </telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="MessageDateTime"   DataFormatString="{0:MMM dd yy HH:mm}" 
                             UniqueName="MessageDateTime">
                             <ItemStyle   VerticalAlign ="Top"  Width = "80px" Font-Size="11px"  />
                        </telerik:GridDateTimeColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="true">
                    <ClientEvents  OnRowDataBound="frmReportMessage_RowDataBound"  OnCommand="frmReportMessage_Command" />
                    <Scrolling AllowScroll="true" ScrollHeight ="135px" UseStaticHeaders ="true"  /> 
                </ClientSettings>
            </telerik:RadGrid>
            <asp:HiddenField ID="hidReportType" runat = "server" Value ="" />
            <asp:HiddenField ID="hidMessageType" runat = "server" Value ="" />
           </td>
       </tr>
    </table>
    </center>
    </div>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            //<![CDATA[
            //Bind Excute, Delete and Modify column

            function frmReportMessage_Command(sender, args) { }

            function updateGrid_frmReportMessage(result) {
                if (result == null) {
                }
                var tableView = $find("<%= gdReportsMessage.ClientID %>").get_masterTableView();
                tableView.set_dataSource(result);
                tableView.dataBind();
                $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdReportsMessage.ClientID %>");
            }

            function frmReportMessage_loadfailed() {
                $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdReportsMessage.ClientID %>");
            }

            //Click I have read all link
            function frmReportMessage_readAllClick() {
                var reportIDs = $telerik.$.trim($telerik.$('#<%=  hidReportType.ClientID %>').val());
                var messageIDs = $telerik.$.trim($telerik.$('#<%=  hidMessageType.ClientID %>').val());
                var postData = "{'reportRepositoryIDs':'" + reportIDs + "', 'messageIDs':'" + messageIDs  + "'}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "ReportWebService.asmx/UpdateReportRepositoryReadStatusByIDString",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d == '<%= ReportWebService.return_no_login %>') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '<%= ReportWebService.return_success %>') {
                            parent.frmMain_Top_Call_GetData();
                        }
                    },
                    error: function (request, status, error) {
                    }

                });

            }

            ///Ajax Call UpdateReportRepositoryReadStatusByID 
            function frmReportMessage_Call_UpdateRead(messageID, messageType) {
                try {
                    if (messageType == '<%=  Report_Type %>') {
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
                                    parent.frmMain_Top_Call_GetData();
                                }
                            },
                            error: function (request, status, error) {
                            }

                        });
                    }
                }
                catch (ex) { }
            }

            function frmReportMessage_RowDataBound(sender, args) {
                var file = args.get_dataItem()["MessagePath"];
                var messageID = args.get_dataItem()["MessageID"];
                var messageType = args.get_dataItem()["MessageType"];
                var messageName = args.get_dataItem()["MessageName"];
                var view = args.get_item().get_cell("MessageName");

                //Set current grid IDs to hidden field
                if (messageType == '<%=  Report_Type %>') {
                    var report_Type = $telerik.$.trim($telerik.$('#<%=  hidReportType.ClientID %>').val());
                    if (report_Type == '') report_Type = messageID
                    else report_Type = report_Type + "," + messageID
                    $telerik.$('#<%=  hidReportType.ClientID %>').val(report_Type);
                }

                if (messageType == '<%=  Message_Type %>') {
                    var message_Type = $telerik.$.trim($telerik.$('#<%=  hidMessageType.ClientID %>').val());
                    if (message_Type == '') message_Type = messageID
                    else message_Type = message_Type + "," + messageID
                    $telerik.$('#<%=  hidMessageType.ClientID %>').val(message_Type);
                }
                var isPDF = false;
                if (file.length > 4) {
                    if (file.toLowerCase().substr(file.length - 4) == '.pdf') isPDF = true;
                }

                if (isPDF) {
                    $telerik.$(view).unbind();
                    $telerik.$(view).bind("click", function () { frmReportMessage_Call_UpdateRead(messageID, messageType); parent.frmMain_Top_ViewDoc(messageID, messageType); return false; });

                    $telerik.$(view).html("<a href='#' a>" + messageName + "</a>");
                }
                else {
                    $telerik.$(view).unbind();
                    $telerik.$(view).bind("click", function () { frmReportMessage_Call_UpdateRead(messageID, messageType); return true; });
                    $telerik.$(view).html("<a href='" + file + "' target='blank' >" + messageName + "</a>");
                }

                $telerik.$(view).css("text-align", "left");
                var messageDatetime = args.get_item().get_cell("MessageDateTime");
                $telerik.$(messageDatetime).css("text-align", "left");

            }

            function pageLoad(sender, eventArgs) {
                $find("<%= LoadingPanel1.ClientID %>").show("<%= gdReportsMessage.ClientID %>");
                ReportWebService.GetReportRepositoryDataForMessage(updateGrid_frmReportMessage, frmReportMessage_loadfailed);
            }


            //]]>
        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
