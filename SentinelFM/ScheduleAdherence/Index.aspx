<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="ScheduleAdherence_Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" id="ng-app" xmlns:ng="http://angularjs.org" ng-app="saScheduleApp">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title>Schedule Adherence</title>
    <link rel="stylesheet" type="text/css" href="../GlobalStyle.css" />
    <link href="css/bootstrap-combined.min.css" rel="stylesheet" type="text/css" />
    <link href="css/buttonbar.css" rel="stylesheet" type="text/css" />
    <link href="css/complexlist.css?v=1.3" rel="stylesheet" type="text/css" media="screen" />
    <link href="~/GlobalStyle.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
	<style type="text/css">
	    .ItemOdd {
            background-color: White;
        }
        .ItemEven {
            background-color: Beige;
        }
	    .ErrorMessage
	    {
	        color:Red;
	    }
	    #div_Route
	    {
            position: fixed;
            left: 300px;
            top: 50px;
            overflow-y: auto;
            padding-left:20px;
        }
	    #div_Station
	    {
            position: fixed;
            left: 300px;
            top: 250px;
            overflow-y: auto;
            padding-left:20px;
        }
	</style>
    <script src="Lib/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Lib/jquery-ui.js" type="text/javascript"></script>
    <script src="Lib/angular.js" type="text/javascript"></script>   
    <%--<script src="Lib/angular_1.08.js" type="text/javascript"></script>--%>   
    <script src="Lib/ui-bootstrap-custom-tpls-0.6.0.js" type="text/javascript"></script>
    <script src="js/ScheduleApp.js?v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
    <script src="JS/DepotController.js?v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
    <script src="JS/ScheduleGroupController.js?v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
    <script src="JS/SARouteController.js?v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
    <script src="JS/SARouteStationController.js?v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
    <script type="text/javascript">
        function SetLayout() {
            var top_height = 45;
            var head_height = 80;
            var w = angular.element(window);
            var win_width = $(window).width();
            var win_height = w.height();
            if (window.console) console.log('win_height=' + win_height);
            if (win_height < 300)
                win_height = 300;
            if (win_width < 600)
                win_width = 600;

            $("#div_Route").width(win_width - 300 - 15);
            $("#div_Station").width(win_width - 300 - 15);
            var div_height = Math.floor((win_height - top_height - 5) / 2);
            $("#div_Schedule").height(win_height - top_height);
            $("#div_Route").height(div_height);
            $("#div_Station").height(div_height);
            $("#div_Route").css("top",top_height);
            $("#div_Station").css("top", top_height + div_height);
        }
        $(function () {
            SetLayout();
            $("#div_Schedule").width(300);
            $(window).resize(function () {
                if (window.console) console.log('win_height=');
                SetLayout();
            });
        }); 
        document.createElement('datepicker-popup-wrap');
        document.createElement('datepicker');
    </script>
</head>
<body class="formtext">
    <form id="form1" runat="server">
	<table id="tblCommands" cellSpacing="0" cellPadding="0" border="0">
		<TR>
			<TD>
                <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdSchedule" runat="server" CausesValidation="False" CssClass="confbutton selectedbutton" Text="Schedules"/>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdStation" runat="server" CssClass="confbutton" CausesValidation="False" Text="Stations/Depots"
                                 PostBackUrl="frmStationMap.aspx"  >
                            </asp:Button>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdReport" runat="server" Text="Report" CssClass="confbutton"
                                CausesValidation="False" PostBackUrl="frmReport.aspx" >
                            </asp:Button>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdReasonCode" runat="server" CssClass="confbutton" CausesValidation="False" Text="Setting" 
                               PostBackUrl="frmReasonCodeList.aspx">
                            </asp:Button>
                        </td>
                    </tr>
                </table>
			</TD>
		</TR>
        <tr style="height:20px"><td></td></tr>
     </table>
    <div ng-controller="SAAppCtrl" >
        <div id="div_Schedule" style="float:left; overflow: auto;">
            <div ng-include="DepotURL"></div>
            <div id="Dialog_Group" ng-include="GroupEditURL" onload="DialogGroupLoaded()" title="Edit Schedule Group"></div>
            <div id="Dialog_GroupCopy" ng-include="GroupCopyURL" onload="DialogGroupCopyLoaded()" title="Copy a Schedule Group"></div>
            <div id="Dialog_ScheduleCopy" ng-include="ScheduleCopyURL" onload="DialogScheduleCopyLoaded()" title="Copy a Schedule"></div>
            <div id="Dialog_GroupImport" ng-include="GroupImportURL" onload="DialogGroupImportLoaded()" title="Import Schedule Group"></div>
            <div ng-include="ScheduleGroupURL">
        </div>
        <div>
            <div id="div_Route">
                <div id="Dialog_Route" ng-include="RouteEditURL" onload="DialogRouteLoaded()" title="Edit a Route"></div>
                <div ng-include="RouteListURL"></div>
            </div>
            <div id="div_Station">
                <div id="Dialog_RStation" ng-include="RStationEditURL" onload="DialogRStationLoaded()" title="Edit a Route Station"></div>
                <div ng-include="RouteStationListURL"></div>
            </div>
        </div>
</form>
</body>
</html>
