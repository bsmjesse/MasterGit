<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmFleetInfoNew.aspx.cs" Inherits="SentinelFM.MapNew_frmMap" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="UserControl/AlarmRotatingClient.ascx" TagName="AlarmRotatingClient"
    TagPrefix="uc1" %>
<%@ Register Src="UserControl/MessageRotatingClient.ascx" TagName="MessageRotatingClient"
    TagPrefix="uc2" %>
<%@ Register Src="UserControl/Message.ascx" TagName="Message" TagPrefix="uc3" %>
<%@ Register Src="UserControl/Map.ascx" TagName="Map" TagPrefix="uc4" %>
<%@ Register src="UserControl/FleetInfoNew.ascx" tagname="FleetInfoNew" tagprefix="uc5" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Map</title>
    <link href="Styles/MapGrid.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/css/Map/sentinel.telogis.css" rel="stylesheet" type="text/css" />
    <link href="Styles/AlarmAndMessage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.4.1.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.geobase.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.dictionary_en.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.skins.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.map.js" language="javascript"></script>
    <script type ="text/javascript" >
        var clientMapData = "";
    </script>
    <style type="text/css" >
         html, body
        {
            margin: 0;
            padding: 0;
            height : 100%;
            overflow: hidden;
        }
    </style>
</head>
<body scroll="no">
    <form id="FleetForm" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" EnablePageMethods ="true">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <div id="MiddleFrame" style="width: 100%; height: 100%">
        <div id="MiddleFrameTop">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="frmVehicleMapfrmVehicleMap" style="height: 337px; width: 100%">
                        <uc4:Map ID="Map1" runat="server" />
                    </td>
                    <td id="frmAlermAndMessage" style="vertical-align: top;">
                        <table border="0" width="220px" cellpadding="0" cellspacing="0" style="margin-left:5px; margin-right:5px ">
                            <tr>
                                <td >
                                    <div id="frmAlarmRotating" class="AlarmRotatingClient"  >
                                       <% if (isAlarm)
                                          {%>
                                       <uc1:AlarmRotatingClient ID="AlarmRotatingClient1" runat="server" /> 
                                       <%} %>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div id="frmMessageRotating" class="MessageRotatingClient" style="margin-top:5px"  >
                                        <% if (isMessage)
                                          {%>
                                           <uc2:MessageRotatingClient ID="MessageRotatingClient1" runat="server" />
                                        <%} %>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <div id="frmMessage" >
                                        <uc3:Message ID="Message1" runat="server" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="frmFleetInfo">
            <uc5:FleetInfoNew ID="ctlFleetInfoNew" runat="server" />
        </div>
    </div>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            $(document).ready(function () {
                
            })

            function NewWindow(mypage, myname, w, h) {
                var winl = (screen.width - w) / 2;
                var wint = (screen.height - h) / 2;
                winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
                win = window.open(mypage, myname, winprops)
                if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            }
            
            function RefreshMap() {
                //todo
            }

            function RefreshGrid() {
                //todo
            }

            $(window).resize(function () {
                //alert($telerik.$(window).width() - 220);
                //$telerik.$("#Map1").width($telerik.$(window).width() - 220);
                SetMapSize($telerik.$("#frmVehicleMapfrmVehicleMap").height(), $telerik.$(window).width() - 220);
            });
            var isRunSetTopFrameHeigh = false;
            function SetTopFrameHeigh() {
                var isAlarm = '<%= isAlarm %>'.toLowerCase();
                var isMessage = '<%= isMessage %>'.toLowerCase();
                var alarmAndMsgHeight = $("#frmVehicleMapfrmVehicleMap").height() - 40 - $("#frmMessage").height();
                if (isAlarm == 'true' && isMessage == 'true') {
                    $("#frmAlarmRotating").height(alarmAndMsgHeight / 2);
                    $("#frmMessageRotating").height(alarmAndMsgHeight / 2);
                }
                if (isAlarm == 'true' && isMessage == 'false') {
                    alarmAndMsgHeight = alarmAndMsgHeight + 20;
                    $("#frmAlarmRotating").height(alarmAndMsgHeight);
                }
                if (isAlarm == 'false' && isMessage == 'true') {
                    alarmAndMsgHeight = alarmAndMsgHeight + 20;
                    $("#frmMessageRotating").height(alarmAndMsgHeight);
                }
            }


        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
