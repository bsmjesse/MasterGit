<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmStationMap.aspx.cs" Inherits="ScheduleAdherence_frmStationMap" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" id="ng-app" ng-app="saStationApp" xmlns:ng="http://angularjs.org">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../GlobalStyle.css" />
    <link rel="stylesheet" href="../openlayers/theme/default/style.css" type="text/css">
    <link rel="stylesheet" href="../maps/style.css" type="text/css">
    <link href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" rel="stylesheet"type="text/css" />
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <link href="css/complexlist.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        html, body {
            height: 100%;
            margin:0px 0px;
        }
 	    #div_map
	    {
            position: fixed;
            left: 300px;
            top: 50px;
        }
       #stationMap {              
              overflow:hidden;
              border: 1px solid #999999;
        }
        #toolbar .toolbar-transparent
        {
            border:0;
            background-color: #B5B2AE !important;
            background-image: none;                        
        }
        .olbsmtoolbar
        {
            border: 0; z-index: 1000; position: absolute; top: 10px; left: 15px; width: 75px;
        }
        .mapsearchtoolbar
        {
            border: 0; z-index: 1000; position: absolute; top: 8px; left: 144px; width: 450px; height: 35px;
        }
        #searchAddressBtnW 
        {
            display: inline-block;
            margin: 1px 15px 0 10px;
            vertical-align: top;
            padding: 0;
        }
        
        .searchAddressBtn 
        {
            background-color: #4D90FE;
            background-image: -moz-linear-gradient(center top , #4D90FE, #4787ED);
            border: 1px solid #3079ED;
            color: #FFFFFF !important;
            margin: 0;
            -moz-user-select: none;
            border-radius: 2px 2px 2px 2px;
            cursor: default !important;
            display: inline-block;
            font-weight: bold;
            height: 29px;
            line-height: 29px;
            min-width: 54px;
            padding: 0 8px;
            text-align: center;
            text-decoration: none !important;
        }
        .searchAddressBtnI 
        {
            display: inline-block;
            height: 13px;
            margin: 7px 19px;
            width: 14px;
            background-image: url("../images/search.png");
        }
         .olControlPanZoomBar 
        {
            top: 50px !important;
            right: 50px !important;
            left: auto !important;
        }
        .maptoolbarselected
        {
            background: none repeat scroll 0 0 #ffffff !important;
        }
    </style>
    <script type="text/javascript" src="../sencha/extjs-4.1.0/bootstrap.js"></script>
    <script type="text/javascript" src="JS/MapMenus.js?v=<%=DateTime.Now.Ticks %>"></script>
    <script src="Lib/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Lib/jquery-ui.js" type="text/javascript"></script>
    <script src="../Openlayers/OpenLayers-2.11/OpenLayers.js" type="text/javascript"></script>
    <script src="../Openlayers/navteqlayer.js" type="text/javascript"></script>
    <%if (ifShowArcgis)  { %>
        <script src="../Openlayers/OpenLayers-2.11/lib/OpenLayers/Layer/ArcGISCache.js" type="text/javascript"></script>
        <script src="../Scripts/OpenLayerMap/utils/arcgislayers.js" type="text/javascript"></script>
    <%} %>
     <%if (!ISSECURE && (ifShowBingRoads || ifShowBingHybrid))
       { %>
        <script src='http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.1'></script> 
     <%} %>
    <script type="text/javascript" src="../Scripts/OpenLayerMap/utils/cloudmade.js"></script>
    <script type="text/javascript" src="<%=ISSECURE ? "https" : "http" %>://maps.googleapis.com/maps/api/js?v=3&client=gme-bsmwirelessinc&sensor=true&libraries=places"></script>
    <%--<script src="Lib/angular_1.08.js" type="text/javascript"></script>--%>   
    <script src="Lib/angular.js" type="text/javascript"></script>   
    <script src="Lib/ui-bootstrap-custom-tpls-0.6.0.js" type="text/javascript"></script>
    <script type="text/javascript" src="JS/StationMapApp.js?v=<%=DateTime.Now.Ticks %>"></script>
    <script type="text/javascript" src="JS/StationMap.js?v=<%=DateTime.Now.Ticks %>"></script>
    <script type="text/javascript">
     var ISSECURE = <%=ISSECURE.ToString().ToLower() %>;   
     var ifShowTheme1 = <%=ifShowTheme1.ToString().ToLower() %>;
     var ifShowTheme2 = <%=ifShowTheme2.ToString().ToLower() %>;
     var ifShowArcgis = <%=ifShowArcgis.ToString().ToLower() %>;
     var ifShowGoogleStreets = <%=ifShowGoogleStreets.ToString().ToLower() %>;
     var ifShowGoogleHybrid = <%=ifShowGoogleHybrid.ToString().ToLower() %>;
     var ifShowBingRoads = <%=ifShowBingRoads.ToString().ToLower() %>;
     var ifShowBingHybrid = <%=ifShowBingHybrid.ToString().ToLower() %>;
     var ifShowNavteq = <%=ifShowNavteq.ToString().ToLower() %>;
     var ifShowNavteqHybrid = <%=ifShowNavteqHybrid.ToString().ToLower() %>;

    function SetLayout()
    {
        var top_height = 45;
        var head_height = 80;
        var win_width = $(window).width();
        var win_height = $(window).height();
        if (win_height < 300)
            win_height = 300;
        if (win_width < 600)
            win_width = 600;
        $("#div_stationList").height(win_height - top_height);
        var div_height = Math.floor(win_height - top_height - 5);
        $("#stationMap").height(div_height-10);
        $("#stationMap").width(win_width - 300);
        $("#div_map").css("top",top_height);
    }

        <%
            if (sn.MapCenter != null && sn.MapCenter != "" && sn.MapZoomLevel != null && sn.MapZoomLevel != ""){
        %>
                DefaultMapCenter = new OpenLayers.LonLat(<% = sn.MapCenter %>);
                DefaultMapZoomLevel = <% = sn.MapZoomLevel %>;
        <%} %>
        $(function() {
            //Devin Added for default Map View
            SetLayout();
            if (map != null){
            //fresh map
                if (DefaultMapCenter != null &&
                         DefaultMapZoomLevel != null) {
                    map.setCenter(DefaultMapCenter, DefaultMapZoomLevel+1);
                    map.setCenter(DefaultMapCenter, DefaultMapZoomLevel);
                }
                else
                    map.zoomTo(4);
            }
            //InitMap();
            $(window).resize(function () {
                if (window.console) console.log('win_height=');
                SetLayout();
            });
        });
</script>
</head>
<body ng-controller="StationCtrl" >
    <form id="form1" runat="server">
	<table id="tblCommands" cellSpacing="0" cellPadding="0" border="0">
		<TR>
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
                            <asp:Button ID="cmdStation" runat="server" CssClass="confbutton selectedbutton" CausesValidation="False" Text="Stations/Depots"
                                PostBackUrl="frmStationMap.aspx"  >
                            </asp:Button>
                        </td>
                        <td style="width: 7px">
                            <asp:Button ID="cmdReport" runat="server" Text="Report" CssClass="confbutton"
                                CausesValidation="False" PostBackUrl="frmReport.aspx"  >
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
    <div>
    <div style="display:none">
        <input type="button" id="bt_EditCancel" value="EditCancel" ng-click="EditCancel()" />
        <input type="button" id="bt_SaveDepot" value="SaveDepot" ng-click="SaveDepot()" />
        <input type="button" id="bt_SaveStation" value="SaveStation" ng-click="SaveStation()" />
        <input type="button" id="bt_DeleteDepot" value="DeleteDepot" ng-click="DeleteDepot()" />
        <input type="button" id="bt_DeleteStation" value="DeleteStation" ng-click="DeleteStation()" />
    </div>
    <div id="div_stationList" style="float:left; overflow: auto; width:300px;">
    <dl class="exlist">
        <dt class="exlist-head">
            <img ng-init="DepotListOpen = true" ng-src='{{DepotListOpen | ListOpenIcon}}' ng-click="DepotListOpen=!DepotListOpen" alt="expand" style="cursor:hand" />            
            Depots
            <span ng-show="DepotListOpen">
            <input type="text" ng-model="Search.Depot" placeholder="Depot name or number" style="width:150px"/>
            </span>
        </dt>
        <dd ng-show="DepotListOpen" class="exlist-body">
            <div ng-show="DepotStatus=='NoData'">No Data.</div>
            <div ng-show="DepotStatus=='Loading'">Loading....</div>
           <dl ng-show="DepotStatus=='Data'" class="exlist" ng-click="SelectedItem({{depot.StationId}})" ng-class="{exlist_current:depot.IsSelected}"
                ng-repeat="depot in DepotList | filter:DepotCompare">
                <dt class="exlist-head">
                    <img ng-init="depot.IsOpen = false" ng-src='{{depot.IsOpen | ListOpenIcon}}' ng-click="depot.IsOpen=!depot.IsOpen" alt="expand" style="cursor:hand" />            
                    {{depot.StationNumber}} {{depot.Name}}
                </dt>
                <dd ng-show="depot.IsOpen" class="exlist-body"">
                <table>
                    <tr>
                        <td>Radius(m):</td>
                        <td>{{depot.radius}}</td>
                    </tr>
                    <tr>
                        <td>Contact:</td>
                        <td>{{depot.ContractName}}</td>
                    </tr>
                    <tr>
                        <td>Phone:</td>
                        <td>{{depot.PhoneNumber}}</td>
                    </tr>
                </table>
                </dd>
            </dl>
        </dd>
    </dl>
    <dl class="exlist">
        <dt class="exlist-head">
            <img ng-init="StationListOpen = true" ng-src='{{StationListOpen | ListOpenIcon}}' ng-click="StationListOpen=!StationListOpen" alt="expand" style="cursor:hand" />            
            Stations
            <span ng-show="StationListOpen">
            <input type="text" ng-model="Search.Station" placeholder="Station name or number" style="width:150px"/>
            </span>
        </dt>
        <dd ng-show="StationListOpen" class="exlist-body">
            <div ng-show="StationStatus=='NoData'">No Data.</div>
            <div ng-show="StationStatus=='Loading'">Loading....</div>
            <dl ng-show="StationStatus=='Data'" class="exlist"  ng-click="SelectedItem({{station.StationId}})" ng-class="{exlist_current:station.IsSelected}"
                ng-repeat="station in StationList | filter:StationCompare">
                <dt class="exlist-head">
                    <img ng-init="station.IsOpen = false" ng-src='{{station.IsOpen | ListOpenIcon}}' ng-click="station.IsOpen=!station.IsOpen" alt="expand" style="cursor:hand" />            
                    {{station.StationNumber}} {{station.Name}}
                </dt>
                <dd ng-show="station.IsOpen" class="exlist-body"">
                <table>
                    <tr>
                        <td>Radius(m):</td>
                        <td>{{station.radius}}</td>
                    </tr>
                    <tr>
                        <td>Contact:</td>
                        <td>{{station.ContractName}}</td>
                    </tr>
                    <tr>
                        <td>Phone:</td>
                        <td>{{station.PhoneNumber}}</td>
                    </tr>
                </table>
                </dd>
            </dl>
        </dd>
    </dl>
    </div>
    <div id="div_map">
        <div id="toolbar" class="olbsmtoolbar"></div>
        <div id="mapsearchtoolbar" class="mapsearchtoolbar">
            <input type="text" style="width:300px;height:26px;" class="formtext bsmforminput" id="txtSearchAddress" name="txtSearchAddress" placeholder="Enter a location"/> 
            <div id="searchAddressBtnW">
                <button id="searchAddressBtn" class="searchAddressBtn" ng-click="SearchAddress()" onclick="return false;">
                    <span class="searchAddressBtnI"></span>
                </button>
            </div>
        </div>
        <div id="stationMap" class="largemap"></div>
    </div>
    <div id="map_canvas"></div>
    </div>
    </form>
</body>
</html>
