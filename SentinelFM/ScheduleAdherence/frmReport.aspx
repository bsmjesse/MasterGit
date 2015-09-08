<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReport.aspx.cs" Inherits="ScheduleAdherence_frmReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" id="ng-app" ng-app="saReportApp" xmlns:ng="http://angularjs.org">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title>Schedule Adherence Report</title>
    <link rel="stylesheet" type="text/css" href="../GlobalStyle.css" />
    <link href="css/bootstrap-combined.min.css" rel="stylesheet" type="text/css" />
    <link href="css/ng-grid.css" rel="stylesheet" type="text/css" />
    <link href="css/ng-table.min.css" rel="stylesheet" type="text/css" />
    <link href="css/buttonbar.css" rel="stylesheet" type="text/css" />
    <link href="css/complexlist.css?v=1.2" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
	<style type="text/css">
	    .ItemOdd {
            background-color: White;
        }
        .ItemEven {
            background-color: Beige;
        }
	    .Item_Changed
	    {
	        background-color:Red;
	    }
        .Item_Late
        {
	        background-color:#ff9999;
        }
        .Item_Early
        {
	        background-color:#9999ff;
        }
        .PageSelected
        {
            font-weight:bold;
        }
        
/*!        #div_options
        {
            position:fixed;
            left:10px;
            top:30px;
        }
        
        #div_Data
        {
            position:relative;
            left:10px;
            top:120px;
        }
*/
        #tbOptions input,select
        {
            width:150px;
        }
	</style>
    <script src="Lib/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Lib/jquery-ui.js" type="text/javascript"></script>
    <%--<script src="http://code.jquery.com/jquery-1.9.1.js"></script>--%>
    <%--<script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>--%>
    <%--<script src="Lib/angular.js" type="text/javascript"></script>--%>   
    <script src="Lib/angular_1.08.js" type="text/javascript"></script>
    <script src="Lib/ng-table.js?1.0" type="text/javascript"></script>   
    <script src="Lib/ui-bootstrap-custom-tpls-0.6.0.js" type="text/javascript"></script>
    <script src="Lib/taffy-min.js" type="text/javascript"></script>
    <script src="JS/ReportApp.js?v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
    <script src="JS/ReportController.js?v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
    <script type="text/javascript">
        document.createElement('datepicker-popup-wrap');
        document.createElement('datepicker');
        var DefaultRoute = "<%=DefaultRoute %>";
        var DefaultVehicle = "<%=DefaultVehicle %>";
        var DefaultStation = "<%=DefaultStation %>";

   </script>
</head>
<body ng-controller="ReportCtrl" class="formtext">
    <form id="form1" runat="server">
	<table id="tblCommands" cellSpacing="0" cellPadding="0" border="0">
		<TR ng-show="DefaultSearch.RouteName == '' && DefaultSearch.VehicleName == '' && DefaultSearch.StationName == ''">
			<TD>
                <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdSchedule" runat="server" CausesValidation="False" CssClass="confbutton" 
                                Text="Schedules" PostBackUrl="Index.aspx"/>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdStation" runat="server" CssClass="confbutton" CausesValidation="False" Text="Stations/Depots"
                                 PostBackUrl="frmStationMap.aspx"  >
                            </asp:Button>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdReport" runat="server" Text="Report" CssClass="confbutton selectedbutton"
                                CausesValidation="False"  >
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
    <div id="div_options">
       <table id="tbOptions">
        <tr>
            <td colspan="4">
                <div ng-show="CanSearch()" class="buttonbar">
                    <a href="" ng-click="View()" class="imgbutton">
                        <span class="imgbuttonicon imgbuttonedit">View</span>
                    </a> 
                    <a href="" ng-show="CanSave()" ng-click="Save()" class="imgbutton">
                        <span class="imgbuttonicon imgbuttonSave">Save</span>
                    </a> 
                    <a href="" ng-show="CanExport()" class="imgbutton" 
                        ng-mousedown="generateCSV()" ng-href="{{CSVText}}" download="ScheduleAdherence.csv">
                        <span class="imgbuttonicon imgbuttonSave" >Export to CSV</span>
                    </a> 
                </div>
            </td>
        </tr>
        <tr>
            <th>Condition:</th>
            <td>Start Date:</td>
            <td style="width:150px">
                <input type="text" datepicker-popup="mediumDate" ng-model="StartDate" is-open="StartOpened"  />
            </td>
            <td>End Date:</td>
            <td colspan="5">
                <input type="text" datepicker-popup="mediumDate" ng-model="EndDate" is-open="EndOpened"/>
            </td>
        </tr>
        <tr>
            <th>Filter:</th>
            <td>Depot:</td>
            <td style="width:150px"><select ng-model="SearchRoute.Depot" ng-options="c.Name for c in DepotList"></select></td>
            <td>Route:</td>
            <td style="width:100px">
                <span ng-show="DefaultSearch.RouteName != ''">{{DefaultSearch.RouteName}}</span>
                <input ng-hide="DefaultSearch.RouteName != ''" type="text" ng-model="SearchRoute.RouteName" placeholder="Route name"/>
            </td>
            <td>Vehicle:</td>
            <td style="width:100px">
                <span ng-show="DefaultSearch.VehicleName != ''">{{DefaultSearch.VehicleName}}</span>
                <div ng-hide="DefaultSearch.VehicleName != ''" class="dropdown">
                <input type="text" class="dropdown-toggle" ng-model="SearchRoute.VehicleName" placeholder="Vehicle name"/>
                <ul class="dropdown-menu">
                <li ng-repeat="v in VehicleList | filter:VehicleCompare | orderBy:'s.Description'">
                    <a href="" ng-click="SearchRoute.VehicleName = v.Description">{{v.Description}}</a>
                </li>
                </ul>
                </div>
            </td>
            <td>Station:</td>
            <td style="width:100px">
                <span ng-show="DefaultSearch.StationName != ''">{{DefaultSearch.StationName}}</span>
                <div ng-hide="DefaultSearch.StationName != ''" class="dropdown">
                <input type="text" class="dropdown-toggle" ng-model="SearchRoute.StationName" placeholder="Station name or number" style="width:160px"/>
                <ul class="dropdown-menu">
                <li ng-repeat="s in StationList | filter:StationCompare | orderBy:'Name'">
                    <a href="" ng-click="SearchRoute.StationName = s.Name">{{s.StationNumber}} {{s.Name}}</a>
                </li>
                </ul>
                </div>
            </td>
            <td ng-hide="true">Delivery Status:</td>
            <td ng-hide="true"><select ng-model="DeliveryStatus" ng-options="c.Name for c in StatusList"></select></td>
        </tr>
    </table>
       <div>
        <input ng-model="IsEditMode" type="checkbox" /> Edit Mode
        <span ng-show="IsEditMode"><input ng-model="IsAutoSave" type="checkbox" /> Auto Save</span>   
        <span class="Item_Early"> Early </span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="Item_Late"> Late </span>
    </div>
    </div>
    <div id="div_Data">
    <div ng-show="Status=='Init' && !CanSearch()">Initialling data....</div>
    <div ng-show="Status=='NoData'">No Data.</div>
    <div ng-show="Status=='Loading'">Loading....</div>
    <div ng-show="Status=='Data'" style="width:3000px">
    <table ng-table="tableParams" class="table" template-pagination="custom/pager" >
        <tr class="tableRow gridtext" ng-class-odd="'ItemOdd'" ng-class-even="'ItemEven'" ng-init="baseTabIndex = $index * 10"
            ng-repeat="item in $data">
            <td data-title="'Depot'" sortable="'Route.RSCDepot.Name'">{{item.Route.RSCDepot.Name}}</td>
            <td data-title="'Route'" sortable="'Route.Name'">{{item.Route.Name}}</td>
            <td data-title="'Vehicle'" sortable="'Route.VehicleName'">{{item.Route.VehicleName}}</td>
            <td data-title="'Station No'" sortable="'Station.StationNo'">{{item.Station.StationNo}}</td>
            <td data-title="'Station Name'" sortable="'Station.StationName'">{{item.Station.StationName}}</td>
            <td data-title="'Depot Depart Sched'">{{item.Route.RSCSchedule_Departure | date:'medium'}}</td>
            <td data-title="'Depot Depart Actual'" ng-class="{Item_Late:item.Route.RSCDeparture_StatusId==1, Item_Early:item.Route.RSCDeparture_StatusId==2}">
                {{item.Route.RSCActual_Departure | date:'medium'}}
            </td>
            <td data-title="'Depot Depart Reason'" ng-class="{Item_Changed:item.Route.IsDeparture_Changed}">
                <span ng-hide="IsEditMode">{{item.Route.RSCDeparture_CodeDescription}}</span>
                <div ng-show="IsEditMode" class="dropdown" >
                    <input type="text" tabindex="{{baseTabIndex + 1}}" class="dropdown-toggle" style="width:100px"
                        ng-Blur="RSCDepartCodeChanged(item.Route.RouteId)" 
                        ng-model="item.Route.RSCDeparture_CodeDescription" />
                    <ul class="dropdown-menu">
                    <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                        <a href="" ng-click="SetRSCDepartCode(item.Route.RouteId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                    </li>
                    </ul>
                </div>
             </td>
            <td data-title="'Station Arrival Sched'">{{item.Station.Schedule_Arrival | date:'medium'}}</td>
            <td data-title="'Station Arrival Actual'"ng-class="{Item_Late:item.Station.Arrival_StatusId==1, Item_Early:item.Station.Arrival_StatusId==2}">
                {{item.Station.Actual_Arrival | date:'medium'}}
            </td>
            <td data-title="'Station Arrival Reason'" ng-class="{Item_Changed:item.Station.IsArrival_Changed}">
                <span ng-hide="IsEditMode">{{item.Station.Arrival_CodeDescription}}</span>
                <div ng-show="IsEditMode" class="dropdown" >
                    <input type="text" tabindex="{{baseTabIndex + 2}}" class="dropdown-toggle" style="width:100px"
                        ng-Blur="ArrivalCodeChanged(item.Route.RouteId, item.Station.RouteStationId)" 
                        ng-model="item.Station.Arrival_CodeDescription" />
                    <ul class="dropdown-menu">
                    <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                        <a href="" ng-click="SetArrivalCode(item.Route.RouteId, item.Station.RouteStationId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                    </li>
                    </ul>
                </div>
            </td>
            <td data-title="'Station Depart Sched'">{{item.Station.Schedule_Departure | date:'medium'}}</td>
            <td data-title="'Station Depart Actual'" ng-class="{Item_Late:item.Station.Departure_StatusId==1, Item_Early:item.Station.Departure_StatusId==2}" >
                {{item.Station.Actual_Departure | date:'medium'}}
            </td>
            <td data-title="'Station Depart Reason'" ng-class="{Item_Changed:item.Station.IsDeparture_Changed}" >
                <span ng-hide="IsEditMode">{{item.Station.Departure_CodeDescription}}</span>
                <div ng-show="IsEditMode" class="dropdown" >
                    <input type="text" tabindex="{{baseTabIndex + 3}}" class="dropdown-toggle" style="width:100px"
                        ng-Blur="DepartCodeChanged(item.Route.RouteId, item.Station.RouteStationId)" 
                        ng-model="item.Station.Departure_CodeDescription" />
                    <ul class="dropdown-menu">
                    <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                        <a href="" ng-click="SetDepartCode(item.Route.RouteId, item.Station.RouteStationId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                    </li>
                    </ul>
                </div>
            </td>
            <td data-title="'Depot Arrival Sched'">{{item.Route.RSCSchedule_Arrival | date:'medium'}}</td>
            <td data-title="'Depot Arrival Actual'" ng-class="{Item_Late:item.Route.RSCArrival_StatusId==1, Item_Early:item.Route.RSCArrival_StatusId==2}">
                {{item.Route.RSCActual_Arrival | date:'medium'}}
            </td>
            <td data-title="'Depot Arrival Reason'" ng-class="{Item_Changed:item.Route.IsArrival_Changed}" >
                <span ng-hide="IsEditMode">{{item.Route.RSCArrival_CodeDescription}}</span>
                <div ng-show="IsEditMode" class="dropdown" >
                    <input type="text" tabindex="{{baseTabIndex + 4}}" class="dropdown-toggle" style="width:100px"
                        ng-Blur="RSCArrivalCodeChanged(item.Route.RouteId)" 
                        ng-model="item.Route.RSCArrival_CodeDescription" />
                    <ul class="dropdown-menu">
                    <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                        <a href="" ng-click="SetRSCArrivalCode(item.Route.RouteId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                    </li>
                    </ul>
                </div>
            </td>
            <td data-title="'Route Duration'">
                <span ng-hide="item.Route.RSCActual_Arrival == null || item.Route.RSCActual_Departure == null">{{item.Route.RSCActual_Arrival.getTime()-item.Route.RSCActual_Departure.getTime() | TimeString}}</span>               
            </td>
            <td data-title="'Depot Comments'" ng-class="{Item_Changed:item.Route.IsDescription_Changed}">
                <span ng-hide="IsEditMode">{{item.Route.Description}}</span>
                <input ng-show="IsEditMode" type="text" tabindex="{{baseTabIndex + 5}}" 
                    ng-Blur="RSCDescriptionChanged(item.Route.RouteId)" 
                    ng-model="item.Route.Description" />
            </td>
            <td data-title="'Station Comments'" ng-class="{Item_Changed:item.Station.IsDescription_Changed}">
                <span ng-hide="IsEditMode">{{item.Station.Description}}</span>
                <input ng-show="IsEditMode" type="text" tabindex="{{baseTabIndex + 6}}" 
                    ng-Blur="StationDescriptionChanged(item.Route.RouteId, item.Station.RouteStationId)" 
                    ng-model="item.Station.Description" />
            </td>
        </tr>
    </table>
    <script type="text/ng-template" id="custom/pager">
<div class="pagination ng-table-pagination"> 
    <span>Show 
        <select size="1" style="width:100px">
            <option value="5" ng-click="params.count(5)">5</option>
            <option value="10" ng-click="params.count(10)">10</option>
            <option value="25" ng-click="params.count(25)">25</option>
            <option value="50" ng-click="params.count(50)">50</option>
            <option value="100" ng-click="params.count(100)">100</option>
        </select> entries
    </span>
	<span ng-class="{'disabled': !page.active}" ng-repeat="page in pages" ng-switch="page.type"> 
		<a ng-switch-when="first" ng-class="{PageSelected:page.number == params.page()}" ng-click="params.page(page.number)" href="">
			<span ng-bind="page.number"></span>
		</a> 
		<a ng-switch-when="page" ng-class="{PageSelected:page.number == params.page()}" ng-click="params.page(page.number)" href="">
			<span ng-bind="page.number"></span>
		</a> 
		<a ng-switch-when="more" ng-class="{PageSelected:page.number == params.page()}" ng-click="params.page(page.number)" href="">&#8230;</a> 
		<a ng-switch-when="last" ng-class="{PageSelected:page.number == params.page()}" ng-click="params.page(page.number)" href="">
			<span ng-bind="page.number"></span>
		</a> 
	</span> 
</div> 
    </script>
    </div>
    </div>
<%--     <dl class="exlist" ng-show="Status=='Data'"  ng-init="baseTabIndex = $index * 100"
        ng-repeat="route in RouteList | filter:SearchRoute:strict | orderBy:'-RSCSchedule_Departure'">
        <dt class="exlist-head">
 			<img ng-src='{{route.IsExpansion | ListOpenIcon}}' ng-click="route.IsExpansion = !route.IsExpansion" ng-init="route.IsExpansion = IsExpansion" alt="expand" style="cursor:hand" />            
            <span class="exlist-head-title">Route({{route.Name}}) for Vehicle({{route.VehicleName}}) at {{route.BaseDate | date:'mediumDate'}}. </span>
        </dt>
        <dd class="exlist-body" ng-class="{exlist_hidden:!route.IsExpansion}">
            <table border="1">
                <tr>
                    <th></th>
                    <th>{{route.RSCDepot.Name}}</th>
                    <th ng-repeat="s in route.StationList | filter:SearchStation:strict| orderBy:'Schedule_Arrival'">
                        {{s.StationName}}
                    </th>
                    <th>{{route.RSCDepot.Name}}</th>
                </tr>
                <tr>
                    <th align="left">Schedule Arrival DateTime:</th>
                    <td rowspan="3"></td>
                    <td ng-repeat="s in route.StationList | filter:SearchStation:strict| orderBy:'Schedule_Arrival'"> 
                        {{s.Schedule_Arrival | date:'medium'}}
                    </td>
                    <td>{{route.RSCSchedule_Arrival | date:'medium'}}</td>
                </tr>
                <tr>
                    <th align="left">Actual Arrival DateTime:</th>
                    <td ng-repeat="s in route.StationList| filter:SearchStation:strict | orderBy:'Schedule_Arrival'" 
                        ng-class="{Item_Late:s.RSCArrival_StatusId==1, Item_Early:s.RSCArrival_StatusId==2}"> 
                        {{s.Actual_Arrival | date:'medium'}}
                    </td>
                    <td ng-class="{Item_Late:route.RSCArrival_StatusId==1, Item_Early:route.RSCArrival_StatusId==2}">{{route.RSCActual_Arrival | date:'medium'}}</td>
                </tr>
                <tr>
                    <th align="left">Arrival Reason Code:</th>
                    <td ng-repeat="s in route.StationList | filter:SearchStation:strict| orderBy:'Schedule_Arrival'" ng-init="childIndex = $index" ng-class="{Item_Changed:s.IsArrival_Changed}"> 
                        <div class="dropdown" >
                          <input type="text" tabindex="{{baseTabIndex + childIndex * 3 + 3}}" class="dropdown-toggle" ng-Blur="ArrivalCodeChanged(route.RouteId, s.RouteStationId)" 
                            ng-model="s.Arrival_CodeDescription" />
                          <ul class="dropdown-menu">
                            <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                              <a href="" ng-click="SetArrivalCode(route.RouteId, s.RouteStationId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                            </li>
                          </ul>
                        </div>
                    </td>
                    <td ng-class="{Item_Changed:route.IsArrival_Changed}">
                        <div class="dropdown" >
                          <input type="text" class="dropdown-toggle" ng-Blur="RSCArrivalCodeChanged(route.RouteId)" tabindex="{{baseTabIndex + route.StationList.length * 3 + 3}}" ng-model="route.RSCArrival_CodeDescription" />
                          <ul class="dropdown-menu">
                            <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                              <a href="" ng-click="SetRSCArrivalCode(route.RouteId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                            </li>
                          </ul>
                        </div>
                    </td>
               </tr>
                <tr>
                    <th align="left">Schedule Departure DateTime:</th>
                    <td>{{route.RSCSchedule_Departure | date:'medium'}}</td>
                    <td ng-repeat="s in route.StationList | filter:SearchStation:strict| orderBy:'Schedule_Arrival'"> 
                        {{s.Schedule_Departure | date:'medium'}}
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <th align="left">Actual Departure DateTime:</th>
                    <td ng-class="{Item_Late:route.RSCDeparture_StatusId==1, Item_Early:route.RSCDeparture_StatusId==2}">{{route.RSCActual_Departure | date:'medium'}}</td>
                    <td ng-repeat="s in route.StationList | filter:SearchStation:strict| orderBy:'Schedule_Arrival'"
                        ng-class="{Item_Late:s.Departure_StatusId==1, Item_Early:s.Departure_StatusId==2}"> 
                        {{s.Actual_Departure | date:'medium'}}
                    </td>
                    <td rowspan="2"></td>
                </tr>
                <tr>
                    <th align="left">Departure Reason Code:</th>
                    <td ng-class="{Item_Changed:route.IsDeparture_Changed}" >
                        <div class="dropdown" >
                          <input type="text" tabindex="{{baseTabIndex + 1}}" class="dropdown-toggle" ng-Blur="RSCDepartCodeChanged(route.RouteId)" ng-model="route.RSCDeparture_CodeDescription" />
                          <ul class="dropdown-menu">
                            <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                              <a href="" ng-click="SetRSCDepartCode(route.RouteId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                            </li>
                          </ul>
                        </div>
                    </td>
                    <td ng-repeat="s in route.StationList | filter:SearchStation:strict| orderBy:'Schedule_Arrival'" ng-init="childIndex = $index" ng-class="{Item_Changed:s.IsDeparture_Changed}"> 
                        <div class="dropdown">
                          <input type="text" tabindex="{{baseTabIndex + childIndex * 3 + 4}}" class="dropdown-toggle" ng-Blur="DepartCodeChanged(route.RouteId, s.RouteStationId)" 
                            ng-model="s.Departure_CodeDescription" />
                          <ul class="dropdown-menu">
                            <li ng-repeat="code in ReasonList | orderBy:'ReasonCode'">
                              <a href="" ng-click="SetDepartCode(route.RouteId, s.RouteStationId, code.ReasonCodeId)">{{code.ReasonCode}}&nbsp;{{code.Description}}</a>
                            </li>
                          </ul>
                        </div>
                    </td>
                </tr>
                <tr>
                    <th align="left">Description:</th>
                    <td ng-class="{Item_Changed:route.IsDescription_Changed}"><input type="text" tabindex="{{baseTabIndex + 2}}" ng-Blur="RSCDescriptionChanged(route.RouteId)" ng-model="route.Description" /></td>
                    <td ng-repeat="s in route.StationList | filter:SearchStation:strict| orderBy:'Schedule_Arrival'" ng-init="childIndex = $index" ng-class="{Item_Changed:s.IsDescription_Changed}"> 
                        <input type="text" ng-Blur="StationDescriptionChanged(route.RouteId, s.RouteStationId)" tabindex="{{baseTabIndex + childIndex * 3 + 5}}" ng-model="s.Description" />
                    </td>
                    <td ng-class="{Item_Changed:route.IsDescription_Changed}"><input type="text" ng-Blur="RSCDescriptionChanged(route.RouteId)" tabindex="{{baseTabIndex + route.StationList.length * 3 + 4}}" ng-model="route.Description" /></td>
               </tr>
            </table>
        </dd>
    </dl>--%>
    </form>
</body>
</html>
