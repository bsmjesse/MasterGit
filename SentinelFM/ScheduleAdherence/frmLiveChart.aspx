<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmLiveChart.aspx.cs" Inherits="ScheduleAdherence_frmLiveChart" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" ng-app="saLiveChartApp">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../GlobalStyle.css" />
    <link href="css/bootstrap-combined.min.css" rel="stylesheet" type="text/css" />
    <link href="css/buttonbar.css" rel="stylesheet" type="text/css" />
    <link href="css/complexlist.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <script src="Lib/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Lib/jquery-ui.js" type="text/javascript"></script>
    <script src="Lib/angular_1.08.js" type="text/javascript"></script>   
    <script src="Lib/ui-bootstrap-custom-tpls-0.6.0.js" type="text/javascript"></script>
    <script src="JS/ng-google-chart.js" type="text/javascript"></script>
    <script src="Lib/taffy-min.js" type="text/javascript"></script>
    <script src="js/LiveChartApp.js" type="text/javascript"></script>
    <script src="JS/LiveChartController.js" type="text/javascript"></script>
    <script type="text/javascript">
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
                            <asp:Button ID="cmdSchedule" runat="server" CausesValidation="False" CssClass="confbutton" Text="Schedules" PostBackUrl="Index.aspx"/>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdStation" runat="server" CssClass="confbutton" CausesValidation="False" Text="Stations/Depot"
                                 PostBackUrl="frmStationList.aspx"  >
                            </asp:Button>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdReport" runat="server" Text="Report" CssClass="confbutton"
                                CausesValidation="False" PostBackUrl="frmReport.aspx" >
                            </asp:Button>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdChart" runat="server" Text="Live Chart" CssClass="confbutton selectedbutton"
                                CausesValidation="False" PostBackUrl="frmLiveChart.aspx">
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
    <div ng-controller="LiveChartCtrl" >
    <table class="formtext">
        <tr>
            <td>Start Date:</td>
            <td>
                <input type="text" datepicker-popup="mediumDate" ng-model="StartDate" placeholder="Enter a start date" is-open="StartOpened"/>
            </td>
            <td>End Date:</td>
            <td>
                <input type="text" datepicker-popup="mediumDate" ng-model="EndDate" placeholder="Enter a end date" is-open="EndOpened" />
            </td>
            <td colspan="2">
                <div class="buttonbar">
                    <a href="" ng-show="CanSearch()" ng-click="View()" class="imgbutton">
                        <span class="imgbuttonicon imgbuttonedit">View</span>
                    </a> 
                </div>
            </td>
        </tr>
        <tr>
            <td>Route:</td>
            <td><input type="text" ng-model="SearchRoute.Name" placeholder="Enter a route name"/></td>
            <td>Vehicle:</td>
            <td>
                <div class="dropdown">
                <input type="text" class="dropdown-toggle" ng-model="Search.VehicleName" placeholder="Enter a vehicle name"/>
                <ul class="dropdown-menu">
                <li ng-repeat="v in VehicleList | filter:VehicleCompare | orderBy:'s.Description'">
                    <a href="" ng-click="Search.Vehicle = v">{{v.Description}}</a>
                </li>
                </ul>
                </div>
            </td>
            <td>Station:</td>
            <td>
                <div class="dropdown">
                <input type="text" class="dropdown-toggle" ng-model="Search.StationName" placeholder="Enter a station name"/>
                <ul class="dropdown-menu">
                <li ng-repeat="s in StationList | filter:StationCompare | orderBy:'Name'">
                    <a href="" ng-click="Search.Station = s">{{s.Name}}</a>
                </li>
                </ul>
                </div>
            </td>
        </tr>
<%--        <tr>
            <td colspan="6">
            Station Late Arrival Buffer:<input type="text" ng-model="BuffSetting.StationLateArrival" style="width:50px" />
            Station Late Departure Buffer:<input type="text" ng-model="BuffSetting.StationLateDeparture" style="width:50px" />
            Depot Late Arrival Buffer:<input type="text" ng-model="BuffSetting.DepotLateArrival" style="width:50px" />
            Depot Late Departure Buffer:<input type="text" ng-model="BuffSetting.DeportLateDeparture" style="width:50px" />
            <br />
            Station Early Arrival Buffer:<input type="text" ng-model="BuffSetting.StationEarlyArrival" style="width:50px" />
            Station Early Departure Buffer:<input type="text" ng-model="BuffSetting.StationEarlyDeparture" style="width:50px" />
            Depot Early Arrival Buffer:<input type="text" ng-model="BuffSetting.DeportEarlyArrival" style="width:50px" />
            Depot Early Departure Buffer:<input type="text" ng-model="BuffSetting.DeportEarlyDeparture" style="width:50px" />
            </td>
        </tr>--%>
    </table>
    <div style="float:left; overflow: auto;">
    <dl class="exlist">
        <dt class="exlist-head">
            <img ng-init="SummaryDepotOpen = true" ng-src='{{SummaryDepotOpen | ListOpenIcon}}' ng-click="SummaryDepotOpen=!SummaryDepotOpen" alt="expand" style="cursor:hand" />            
            Summary By Depot
        </dt>
        <dd ng-show="SummaryDepotOpen" class="exlist-body"">
            <dl class="exlist">
                <dt class="exlist-head">
                    <img ng-init="Summary_stArrivalOpen = true" ng-src='{{Summary_stArrivalOpen | ListOpenIcon}}' ng-click="Summary_stArrivalOpen=!Summary_stArrivalOpen" alt="expand" style="cursor:hand" />            
                    Station Arrival
                </dt>
                <dd ng-show="Summary_stArrivalOpen" class="exlist-body">
                    <dl class="exlist">
                        <dt class="exlist-head exlist_Schedule" ng-click="AddView(1, 1, 0)">All</dt>
                        <dt class="exlist-head exlist_Schedule" ng-click="AddView(1, 1, depot.StationId)" ng-repeat="depot in DepotList">{{depot.Name}}</dt>
                    </dl>
                </dd>
                <dt class="exlist-head">
                    <img ng-init="Summary_stDepartureOpen = true" ng-src='{{Summary_stDepartureOpen | ListOpenIcon}}' ng-click="Summary_stDepartureOpen=!Summary_stDepartureOpen" alt="expand" style="cursor:hand" />            
                    Station Departure
                </dt>
                <dd ng-show="Summary_stDepartureOpen" class="exlist-body">
                    <dl class="exlist">
                        <dt class="exlist-head exlist_Schedule" ng-click="AddView(1, 2, 0)">All</dt>
                        <dt class="exlist-head exlist_Schedule" ng-click="AddView(1, 2, depot.StationId)" ng-repeat="depot in DepotList">{{depot.Name}}</dt>
                    </dl>
                </dd>
                <dt class="exlist-head">
                    <img ng-init="Summary_deArrivalOpen = true" ng-src='{{Summary_deArrivalOpen | ListOpenIcon}}' ng-click="Summary_deArrivalOpen=!Summary_deArrivalOpen" alt="expand" style="cursor:hand" />            
                    Depot Arrival
                 </dt>
                <dd ng-show="Summary_deArrivalOpen" class="exlist-body">
                    <dl class="exlist">
                        <dt class="exlist-head exlist_Schedule">All</dt>
                        <dt class="exlist-head exlist_Schedule" ng-repeat="depot in DepotList">{{depot.Name}}</dt>
                    </dl>
                </dd>
                <dt class="exlist-head">
                    <img ng-init="Summary_deDepartureOpen = true" ng-src='{{Summary_deDepartureOpen | ListOpenIcon}}' ng-click="Summary_deDepartureOpen=!Summary_deDepartureOpen" alt="expand" style="cursor:hand" />            
                    Depot Departure
                </dt>
                <dd ng-show="Summary_deDepartureOpen" class="exlist-body">
                    <dl class="exlist">
                        <dt class="exlist-head exlist_Schedule">All</dt>
                        <dt class="exlist-head exlist_Schedule" ng-repeat="depot in DepotList">{{depot.Name}}</dt>
                    </dl>
                </dd>
            </dl>
        </dd>
        <dt class="exlist-head exlist_Schedule">Summary By Station</dt>
        <dt class="exlist-head exlist_Schedule">Summary By Vehicle</dt>
    </div>
    <div style="float:left; margin:10px;">
    <tabset>
        <tab ng-repeat="tab in ViewList" active="tab.active">
            <tab-heading>
                <table>
                    <tr><td align="right" ></td></tr>
                </table>
                <span>{{tab.Title}}</span>
                <span style="float: right">X</span>
            </tab-heading>
            <div ng-include="tab.URL"></div>
        </tab>
    </tabset>
    </div>
    </div>
    </form>
</body>
</html>
