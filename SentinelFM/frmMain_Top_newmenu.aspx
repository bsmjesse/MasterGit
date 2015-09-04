<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMain_Top_newmenu.aspx.cs" Inherits="frmMain_Top_newmenu" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="UserControl/ShowMessage.ascx" TagName="ShowMessage" TagPrefix="uc2" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <TITLE>Fleet Management & Security</TITLE>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style type="text/css">
        html, body
        {
            margin: 0;
            padding: 0;
  height: 100%;
  overflow: hidden;

            
        }
        #frmMain_TopPopUp
        {
            width: 300px;
            height: 0px;
            position: absolute;
            right: 0;
            bottom: 0;
            border: 1px solid #666;
            margin: 0;
            padding: 1px;
            overflow: auto;
            
            
            display: none;
            z-index: 200;
        }
        
        
        
    </style>
</head>
<body scroll="no">
    <form id="form1" runat="server" style="height:100%;">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Services>
            <asp:ServiceReference Path="ReportWebService.asmx" />
            <asp:ServiceReference Path="~/CustomReport/CustomReportsService.asmx" />
        </Services>

        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            var myheigh1 = 182;
            var pageName = '';

            function frmMain_KeepAlive() {
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "Configuration/KeepAlive.asmx/IsSessionAlive",
                    data: '',
                    dataType: "json",
                    success: function (data) {

                    },
                    error: function (request, status, error) {

                    }

                });
            };
            ///Ajax Call GetReportRepositoryDataForMessage
            function frmMain_Top_Call_GetData() {
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "Reports/ReportWebService.asmx/GetReportRepositoryCountForMessage",
                    data: '',
                    dataType: "json",
                    success: function (data) {
                        if (data.d == 0) {
                            var MsgPop = $telerik.$("#frmMain_TopPopUp");
                            var popH = parseInt(MsgPop.height());
                            if (popH > 0) { frmMain_Top_Msg_pop(); }
                        }
                        else {
                            var MsgPop = $telerik.$("#frmMain_TopPopUp");
                            var popH = parseInt(MsgPop.height());
                            if (popH > 0) { MsgPop[0].contentWindow.pageLoad(); }
                            else {
                                MsgPop.attr("src", "Reports/frmReportMessage.aspx?rmd=" + Math.random());
                                frmMain_Top_Msg_pop();
                            }
                        }
                    },
                    error: function (request, status, error) {
                    }

                });

            }
            function pageLoad(sender, eventArgs) {
                //setTimeout("frmMain_Top_SetupAjaxCall()", 1000);
                setInterval("frmMain_KeepAlive()", 60 * 1000);
            }

            function frmMain_Top_Msg_pop() {

                var MsgPop = $telerik.$("#frmMain_TopPopUp");
                var popH = parseInt(MsgPop.height());
                if (popH == 0) {
                    MsgPop.css("display", "block");
                    show = setInterval("frmMain_Top_changeH('up')", 2);
                }
                else {
                    hide = setInterval("frmMain_Top_changeH('down')", 2);
                }
            }
            function frmMain_Top_changeH(str) {
                var MsgPop = $telerik.$("#frmMain_TopPopUp");
                var popH = parseInt(MsgPop.css("height"));
                var myheigh = myheigh1;
                if (str == "up") {
                    if (popH <= myheigh) {
                        MsgPop.css("height", (popH + 4).toString() + "px");
                    }
                    else {
                        clearInterval(show);
                    }
                }
                if (str == "down") {
                    if (popH >= 4) {
                        MsgPop.css("height", (popH - 4).toString() + "px");
                    }
                    else {
                        clearInterval(hide);
                        MsgPop.hide();
                    }
                }
            }

            //for stetup Ajax Call
            function frmMain_Top_SetupAjaxCall() {
                return;
                frmMain_Top_Call_GetData();
                //setInterval("frmMain_Top_Call_GetData()", 60*1000 * 10);
                setInterval("frmMain_Top_Call_GetData()", 60 * 1000);
            }

            //for display document
            function frmMain_Top_ViewDoc(messageID, messageType) {
                if (messageType == '<%= Report_Type%>') {
                    var url = "Reports/frmViewDoc.aspx?id=" + messageID + "&rnd=" + Math.random();
                    var oWnd = $find("<%=ViewDocument.ClientID%>");
                    var height = $telerik.$(document).height() * 0.8;
                    var width = $telerik.$(document).width() * 0.8;
                    oWnd.setSize(width, height);
                    window.radopen(url, "ViewDocument");
                }

            }


            function ShowfrmShowMessage(msg) {
                ShowMessage_SetTxtMessage(msg)
                $find('<%= RadWindowContentTemplate.ClientID%>').show();
            }
            function ClosefrmShowMessage() {
                $find('<%= RadWindowContentTemplate.ClientID%>').close();
                return false;
            }

            //for display window
            function frmMain_Top_ViewWindow(url, width, height) {
                var oWnd = $find("<%=ViewDocument.ClientID%>");
                oWnd.setSize(width, height);
                window.radopen(url, "ViewDocument");
            }

            function OnClientShow(radWindow) {
                radWindow.Center();
            }


            function ReloadUrlForfrmFleetInfo(url) {
                try {
                    var myPage = $telerik.$($telerik.$($telerik.$("#TopFrame").contents()[0]).find("Frame[name='main']")[0].contentWindow.document).find("Frame[name='frmFleetInfo']")[0].contentWindow;
                    var locUrl = myPage.document.location.toString().toLowerCase();
                    if (locUrl.indexOf(url.toLowerCase()) > 0) {
                        //alert(myPage.document.location);
                        //myPage.document.location = myPage.document.location;
                        myPage.AutoReloadDetails();
                    }
                }
                catch (err) { alert(err.Message) }

                //                       try
                //                       {
                //                       alert($telerik.$($telerik.$($telerik.$("#TopFrame")[0].contentDocument).find("[name='main']")[0].contentDocument).find("[name='frmFleetInfo']")[0].contentDocument.location);
                //                       }
                //                       catch(err){}

                //                       try {
                //                           alert(top.frames('TopFrame').frames('main').frames('frmFleetInfo').location);
                //                       }
                //                       catch (err) { }
            }
        </script>
    </telerik:RadCodeBlock>
    <div style="height:100%;">
        <iframe src="frmMain_newmenu.htm" id="TopFrame" style="Height:100%; width:100%;  border:0;margin:0px"  ></iframe>
        <iframe id="frmMain_TopPopUp"  ></iframe>
    </div>

        <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
            <Windows>
                <telerik:RadWindow ID="ViewDocument" runat="server" Skin="Simple" DestroyOnClose="false"  
                    ReloadOnShow="true" ShowContentDuringLoad="false"  VisibleStatusbar="false" Behaviors="Maximize,Close,Move" 
                    VisibleTitlebar="true"  Height = "600px" OnClientShow="OnClientShow"  Width ="800px"
                      />

                <telerik:RadWindow ID="RadWindowContentTemplate"  VisibleStatusbar="false" 
                      Width="480px" Height="280px" runat="server" ReloadOnShow="true" Behaviors="Close,Move" 
                    ShowContentDuringLoad="false" Skin="Simple" Modal="true" >
                    <ContentTemplate>
                        <uc2:ShowMessage ID="ShowMessage1" runat="server" />
                    </ContentTemplate>
                </telerik:RadWindow>

            </Windows>
        </telerik:RadWindowManager>

    </form>
</body>
</html>
