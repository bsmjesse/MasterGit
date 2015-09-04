<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReeferMap.aspx.cs" Inherits="SentinelFM.ReeferMap" Async="true" EnableViewState="false" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Vehicle List</title>

    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <%--<link rel="stylesheet" type="text/css" href="./sencha/extjs-4.1.0/resources/css/ext-all.css" />--%>

    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/examples/ux/grid/css/RangeMenu.css" />
    <link href="./bootstrap/css/bootstrapscope.css" rel="stylesheet" media="screen" />   

    <style type="text/css">
 
        .x-menu-item img.preview-right, .preview-right {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-right.gif);
        }
        .x-menu-item img.preview-bottom, .preview-bottom {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-bottom.gif);
        }
        .x-menu-item img.preview-hide, .preview-hide {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-hide.gif);
        }

        .x-menu-item img.preview-top, .preview-top {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-top.gif);
        }

        #reading-menu .x-menu-item-checked {
            border: 1px dotted #a3bae9 !important;
            background: #DFE8F6;
            padding: 0;
            margin: 0;
        }


        .grid-row-red
        {
          background-color: red;
        }
        .grid-row-yellow
        {
          background-color: yellow;
        }
 
        .x-grid3-scroller {overflow-y: scroll;}
 
        .cmbLabel
        {
            float:left;z-index:2;position:relative;            
            font:normal 12px tahoma, arial, verdana, sans-serif !important;
            -moz-user-select:none;-khtml-user-select:none;-webkit-user-select:ignore;cursor:default
        }       

        .x-panel-body
        {            
            border-top:0px solid #fafad2 !important;
            border-bottom:0px solid #fafad2 !important;
            font:normal 12px tahoma, arial, verdana, sans-serif !important
        }   
        .cmbfonts
        {
            font:normal 12px tahoma, arial, verdana, sans-serif !important
        }
        .map {
                background: url('./Styles/SentinelFM/resources/icons/map.png') no-repeat 0 0 !important;
            }
            
        .x-livesearch-match {
            font-weight: bold;
            background-color: yellow;
        }

        .x-toolbar .x-form-cb-wrap {
            line-height: 22px;
        }
        .x-toolbar .x-form-checkbox {
            vertical-align: 0;
        }
        
        .x-grid-with-col-lines .x-grid-cell {border-right: 0;}
        
        .withinlastday .x-date-time {background-color: #7BB273 !important;}
        .withinlast2days .x-date-time {background-color: #EFD700 !important;}
        .withinlast3days .x-date-time {background-color: #FFA64A !important;}
        .withinlast7days .x-date-time {background-color: #DE7973 !important;}
        .morethan7days .x-date-time {background-color: #637DA5 !important;}
        
        .collapsebutton 
        {
            position:absolute;
            float:right;
            margin-right: 20px;
            margin-top: 0;
            z-index: 1000;
        }

        .togglemap 
        {
            position:absolute;
            float:right;
            margin-right: 120px;
            margin-top: 0;
            z-index: 1000;
        }
        
        
    </style>
    
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>     
    <script type="text/javascript" src="./sencha/<%=ExtjsVersion %>/ext-all.js"></script>  
    <script type="text/javascript" src="./sencha/rowexpander.js"></script>  
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/downloadify.min.js"></script>
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/Button.js"></script>
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/ExportMenu.js"></script>
    <script type="text/javascript" src="./bootstrap/js/bootstrap.min.js"></script>     
    <script type="text/javascript">
  //rraj added for date time format issue
        var userDate ='<%=sn.User.DateFormat %>';
        var userTime ='<%=sn.User.TimeFormat %>';
        function getSenchaDateFormat()
        {
            if(userDate == 'dd/MM/yyyy')
                userDate = 'd/m/Y';
            else if(userDate == 'd/M/yyyy')
                userDate ='j/n/Y';
            else if(userDate == 'dd/MM/yy')
                userDate ='d/m/y';
            else if(userDate == 'd/M/yy')
                userDate ='j/n/y';
            else if(userDate == 'd MMM yyyy')
                userDate ='j M Y';
            else if(userDate == 'MM/dd/yyyy')
                userDate ='m/d/Y';
            else if(userDate == 'M/d/yyyy')
                userDate = 'n/j/Y';
            else if(userDate == 'MM/dd/yy')
                userDate = 'm/d/y';
            else if(userDate == 'M/d/yy')
                userDate = 'n/j/y';
            else if(userDate == 'MMMM d yy')
                userDate = 'M j y';
            else if(userDate == 'yyyy/MM/dd')
                userDate = 'Y/m/d';
            if(userTime =="hh:mm:ss tt")
                userTime="h:i:s A";
            else
                userTime="H:i:s";
            return userDate+" "+userTime;
        }
        var userdateformat = getSenchaDateFormat();
        //--------------------------------------


     var mapAssets = <%=mapAssets.ToString().ToLower() %>;
     var maxVehiclesOnMap = <%=maxVehiclesOnMap%>;
     var selectedVehicleBoxId = -1;
     var selectedVehicleData;
     var firstLoad = true;
     var ifShowTheme1 = <%=ifShowTheme1.ToString().ToLower() %>;
     var ifShowTheme2 = <%=ifShowTheme2.ToString().ToLower() %>;
     var ifShowArcgis = <%=ifShowArcgis.ToString().ToLower() %>;
     var ifShowGoogleStreets = <%=ifShowGoogleStreets.ToString().ToLower() %>;
     var ifShowGoogleHybrid = <%=ifShowGoogleHybrid.ToString().ToLower() %>;
     var ifShowBingRoads = <%=ifShowBingRoads.ToString().ToLower() %>;
     var ifShowBingHybrid = <%=ifShowBingHybrid.ToString().ToLower() %>;
     var ifShowNavteq = <%=ifShowNavteq.ToString().ToLower() %>;
     var overlaysettings = <%=overlays==""?"''":overlays %>;
     var defaultMapView = "<%=defaultMapView %>";
     var VehicleClustering = <%=VehicleClustering.ToString().ToLower() %>;
     var VehicleClusteringDistance = <%=VehicleClusteringDistance %>;
     var VehicleClusteringThreshold = <%=VehicleClusteringThreshold %>;
     var showDriverFinderButton = <%=showDriverFinderButton.ToString().ToLower() %>;
     var showDriverColumn = <%=showDriverColumn.ToString().ToLower() %>;
     var vehiclegrid;
     var zoomtogeozone = true;
     var ifShowClusteredVehicleLabel = false;

     var VehicleListPagesize = <%=PageSize %>; 
     var HistoryPagesize = 5000;//<%=HistoryPageSize.ToString() %>;
     //var HistoryPagesize = 10;
     var IE8orUnder = <%=IE8orUnder.ToString().ToLower() %>;

     //Devin Added
     var DispatchOrganizationId = <%=sn.User.OrganizationId %>;

     var ShowAlarmTab = <%=ShowAlarmTab.ToString().ToLower() %>;
     //ShowAlarmTab = false;

     if (overlaysettings.vehicleDrivers) {
        if(overlaysettings.vehicleDriversVisibility) ifShowClusteredVehicleLabel = true;        
     }
     else if (overlaysettings.vehiclenames) {
        if(overlaysettings.vehiclenamesVisibility) ifShowClusteredVehicleLabel = true;
     }

     if(defaultMapView == "north")
        defaultMapView = "south";
     else if(defaultMapView == "south")
        defaultMapView = "north";
     else if(defaultMapView =="east")
        defaultMapView = "west";
     else if(defaultMapView == "west")
        defaultMapView = "east";

     var LoadVehiclesBasedOn = '<%=LoadVehiclesBasedOn %>';
     var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
     var DefaultOrganizationHierarchyFleetName = '<%=DefaultOrganizationHierarchyFleetName %>';
     var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
     var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
     var HistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
     var HistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

     var CommandHistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
     var CommandHistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

     var ReeferHistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
     var ReeferHistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

     var HistoryEnabled = <%=HistoryEnabled.ToString().ToLower() %>;

     var ViolationTabAtNewMapPage = <%=ViolationTabAtNewMapPage.ToString().ToLower() %>;
     
	 var currentMapFrame='';

	 var ReeferDashboardDefaultTimeRange = '<%=ReeferDashboardDefaultTimeRange %>';

        var DisplayZone2_3Temperatures = '<%=DisplayZone2_3Temperatures%>';

        var TemperatureType = '<%=TemperatureType%>';

         var vehinterval ='<%=sn.User.GeneralRefreshFrequency %>';
         var PositionExpiredTime= '<%=sn.User.PositionExpiredTime %>';
         var DefaultFleetID='<%=sn.User.DefaultFleet %>';  
         var DefaultFleetName = '<%=DefaultFleetName %>';  
         
         var SelectedFleetId = DefaultFleetID;
         var SelectedFleetName = DefaultFleetName; 
         //var mapReloadedFromSearch='<%=sn.Map.ReloadMap%>';

         var IsSyncOn = false;
         var allVehicles;
         var allHistories;
         var windowsUpdating = false;
         var allTrackerWindows = new Array();
         var wincounter = 0;
         var openMapURL="./OpenLayerMap.aspx?WinId=";

         if (vehinterval > 0) {
             IsSyncOn = true;
         }

         function GetWinInitialTrackData(winID) {
             if (allTrackerWindows[winID])
                 return allTrackerWindows[winID].winData;
             else
                 return "";
         }

         function GetWinTrackData(vehicleID) {
             var result;
             var j = 0;             
                 for (j = 0; j < allVehicles.length; j++) {
                     if (allVehicles[j].BoxId == vehicleID) {
                     //.id == vehicleID) {
                         result=allVehicles[j];
                         break;
                     }
                 }
                 return result;
         }

         function GetTelogisMapData() {
             /*while (true) {
                 if (allVehicles) {
                     if (allVehicles != "" && allVehicles != null) {
                         break;
                     }
                 }
             }*/
             return allVehicles;
         }

         function SetWinTrackData(data) {
             winTracker = new Object();
             winTracker.winId = "Tracker:" + wincounter;
             winTracker.winData = data;
             allTrackerWindows[wincounter] = winTracker;
             NewWindow(openMapURL,wincounter);
             wincounter++;
         }

//         function updateVehiclePosition(VehIds)
//         {
//             alert("You cliecked " + VehIds);
//         }
         function SetWinTrackData2(data) {
             winTracker = new Object();
             winTracker.winId = "Tracker:" + wincounter;
             winTracker.winData = data;
             allTrackerWindows[wincounter] = winTracker;
             //NewWindow(openMapURL,wincounter);
             wincounter++;
         }

         function searchwindow(mapframe) {
			currentMapFrame=mapframe;
					var mypage='./Map/mapSearch.aspx'
					var myname='Search';
					var w=840;
					var h=500;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,toolbar=0,menubar=0,scrollbars=1,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
            }
//         function ZoomVehicles(mapData) {             
//             var el = document.getElementById("mapframe");
//             var IframeLoaded = false;
//             var maxAttempt = 10;
//             var currentAttempt = 0;
//             while (!IframeLoaded && currentAttempt < maxAttempt) {
//                 try {
//                     currentAttempt = currentAttempt + 1;
//                     if (el.contentWindow) {
//                         el.contentWindow.zoomMap(mapData);
//                     }
//                     else if (el.contentDocument) {
//                         el.contentDocument.zoomMap(mapData);
//                     }
//                     IframeLoaded = true;
//                 } catch (err) {
//                     IframeLoaded = false;
//                 }
//             }
//         }

         function mapSelecteds(selectedBoxs,mapframe)
            {               
              var el = document.getElementById(mapframe);             
              if(el)
              {
                 var IframeLoaded = false;
                 var maxAttempt = 5;
                 var currentAttempt = 0;
                 while (!IframeLoaded && currentAttempt < maxAttempt) {
                     try {
                         currentAttempt = currentAttempt + 1;
                         if (el.contentWindow) {
                             el.contentWindow.zoomMap(selectedBoxs);
                         }
                         else if (el.contentDocument) {
                             el.contentDocument.zoomMap(selectedBoxs);
                         }
                         IframeLoaded = true;
                     } catch (err) {
                         IframeLoaded = false;
                     }
                 }  
               }      
            }

       	    function mapSelectedVehIDs(vehID)
            {             
                var vehIDs = [];
                vehID=new String(vehID);
                //var vehIDs=selectedVehIDs.split(',');                
                try
                {
                    var boxids=new Array(); 
                    if(vehID.indexOf(',')>0)
                    {
                        vehIDs=vehID.split(',');
                        selectSelectedVehIDs(vehIDs);
                         var i = 0, j = 0;
                        for (i = 0; i < vehIDs.length; i++) {
                         for (j = 0; j < allVehicles.length; j++) {
                            if (allVehicles[j].VehicleId == vehIDs[i]) { 
                              boxids.push(allVehicles[j]);
                              break;
                            }
                         }
                        }
                    }
                    else
                    {  
                        vehIDs.push(vehID);
                        selectSelectedVehIDs(vehIDs);       
                        var i = 0, j = 0;
                    
                        for (j = 0; j < allVehicles.length; j++) {
                            if (allVehicles[j].VehicleId == vehID) { 
                                boxids.push(allVehicles[j]);
                                break;
                            }
                        }
                    
                    }
                    if(currentMapFrame!='')
                    {
                        mapSelecteds(boxids,currentMapFrame);
                    }
                    else
                    {
                        mapSelecteds(boxids,'nmapframe');
                    }
                 }
                 catch(err)
                 {
                    console.log("Error: " + err.message);
                 }
            }

            function selectSelectedVehIDs(vehIDs) {
               try {
                   mapLoading = true;

                   for (i = 0; i < vehIDs.length; i++) {
                        var gridindex = 0;
                        vehiclegrid.getStore().each(function (record) {                            
                           if (record.data.VehicleId == vehIDs[i]) {                                
                               if (!vehiclegrid.getSelectionModel().isSelected(gridindex))
                                   vehiclegrid.getSelectionModel().select(gridindex, true, false);                               
                           }
                           gridindex++;

                       });
                   }
                   
                   mapLoading = false;
               }
               catch (err) {
                alert(err);
               }               
           }

  function MapAlarm(vehicles)
             {
                  var el = document.getElementById("nmapframe");
                   var IframeLoaded = false;
                 var maxAttempt = 5;
                 var currentAttempt = 0;
                 while (!IframeLoaded && currentAttempt < maxAttempt) {
                     try {
                         currentAttempt = currentAttempt + 1;
                         if (el.contentWindow) {
                          el.contentWindow.zoomMapAlarms(vehicles);
                         }
                         else if (el.contentDocument) {
                          el.contentDocument.zoomMapAlarms(vehicles);
                         }
                           IframeLoaded = true;
                     } catch (err) {
                         //console.log("Error in mapping alarms " + err.message);
                         IframeLoaded = false;
                     }
                     }

                        el = document.getElementById("smapframe");
                    IframeLoaded = false;
                 currentAttempt = 0;
                 while (!IframeLoaded && currentAttempt < maxAttempt) {
                     try {
                         currentAttempt = currentAttempt + 1;
                         if (el.contentWindow) {
                          el.contentWindow.zoomMapAlarms(vehicles);
                         }
                         else if (el.contentDocument) {
                          el.contentDocument.zoomMapAlarms(vehicles);
                         }
                           IframeLoaded = true;
                     } catch (err) {
                         //console.log("Error in mapping alarms " + err.message);
                         IframeLoaded = false;
                     }
                     }

                        el = document.getElementById("emapframe");
                    IframeLoaded = false;
                 currentAttempt = 0;
                 while (!IframeLoaded && currentAttempt < maxAttempt) {
                     try {
                         currentAttempt = currentAttempt + 1;
                         if (el.contentWindow) {
                          el.contentWindow.zoomMapAlarms(vehicles);
                         }
                         else if (el.contentDocument) {
                          el.contentDocument.zoomMapAlarms(vehicles);
                         }
                           IframeLoaded = true;
                     } catch (err) {
                         //console.log("Error in mapping alarms " + err.message);
                         IframeLoaded = false;
                     }
                     }
             }

        function ShowMapFrameData(mapData, isInitialMapRequest,mapframe, zoomVehicles) {        
            zoomVehicles = typeof zoomVehicles !== 'undefined' ? zoomVehicles : true;
            
             if (isInitialMapRequest) {
                 allVehicles = mapData;
             }
             else {
                 var i = 0, j = 0;
                 for (i = 0; i < mapData.length; i++) {
                     for (j = 0; j < allVehicles.length; j++) {
                         if (allVehicles[j].BoxId == mapData[i].BoxId) {
                         //(allVehicles[j].id == mapData[i].id) {
                             allVehicles[j] = mapData[i];
                             break;
                         }
                     }
                 }
                }
                 var el = document.getElementById(mapframe);
                 var IframeLoaded = false;
                 var maxAttempt = 5;
                 var currentAttempt = 0;
                 while (!IframeLoaded && currentAttempt < maxAttempt) {
                     try {
                         currentAttempt = currentAttempt + 1;
                         if (el.contentWindow) {
                                 if (isInitialMapRequest) {
                                    el.contentWindow.ShowMultipleAssets(mapData, zoomVehicles);                                     
                                }
                                else {
                             el.contentWindow.UpdateMultipleAssets(mapData);
                                }
                         }
                         else if (el.contentDocument) {
                             if (isInitialMapRequest) {
                                 el.contentDocument.ShowMultipleAssets(mapData, zoomVehicles);                                  
                              }
                              else {
                             el.contentDocument.UpdateMultipleAssets(mapData);
                             }
                         }
                         IframeLoaded = true;
                     } catch (err) {
                         IframeLoaded = false;
                     }
                 }
             
         }

         function ShowHistoryMapFrameData(mapData,isInitialMapRequest,mapframe, zoomHistories) {        
            zoomHistories = typeof zoomHistories !== 'undefined' ? zoomHistories : true; 
             
            allHistories = mapData;
             
            var el = document.getElementById(mapframe);
            var IframeLoaded = false;
            var maxAttempt = 5;
            var currentAttempt = 0;
            while (!IframeLoaded && currentAttempt < maxAttempt) {
                try {
                    currentAttempt = currentAttempt + 1;
                    if (el.contentWindow) {
                        if (isInitialMapRequest) {
                            el.contentWindow.ShowHistories(mapData, zoomHistories);                                     
                        }
                        else {
                            el.contentWindow.UpdateHistories(mapData);
                        }
                        el.contentWindow.removeAllPopups();
                    }
                    else if (el.contentDocument) {
                        if (isInitialMapRequest) {
                            el.contentDocument.ShowHistories(mapData, zoomHistories);                                  
                        }
                        else {
                            el.contentDocument.UpdateHistories(mapData);
                        }
                        el.contentDocument.removeAllPopups();
                    }
                    IframeLoaded = true;
                } catch (err) {
                    IframeLoaded = false;
                }
            }
             
         }

//         function ShowMapFrameData(mapData, isInitialMapRequest) {

//             if (isInitialMapRequest) {
//                 allVehicles = mapData;
//             }
//             else {
//                 var i = 0, j = 0;
//                 for (i = 0; i < mapData.length; i++) {
//                     for (j = 0; j < allVehicles.length; j++) {
//                         if (allVehicles[j].id == mapData[i].id) {
//                             allVehicles[j] = mapData[i];
//                             break;
//                         }
//                     }
//                 }

//                 var el = document.getElementById("mapframe");
//                 var IframeLoaded = false;
//                 var maxAttempt = 5;
//                 var currentAttempt = 0;
//                 while (!IframeLoaded && currentAttempt < maxAttempt) {
//                     try {
//                         currentAttempt = currentAttempt + 1;
//                         if (el.contentWindow) {
//                             //    if (isInitialMapRequest) {
//                             //       el.contentWindow.ShowMultipleAssets(mapData);                             
//                             //   }
//                             //   else {
//                             el.contentWindow.UpdateMultipleAssets(mapData);
//                             //   }
//                         }
//                         else if (el.contentDocument) {
//                             //if (isInitialMapRequest) {
//                             //    el.contentDocument.ShowMultipleAssets(mapData);                            
//                             // }
//                             // else {
//                             el.contentDocument.UpdateMultipleAssets(mapData);
//                             //}
//                         }
//                         IframeLoaded = true;
//                     } catch (err) {
//                         IframeLoaded = false;
//                     }
//                 }
//             }
//         }

         function removeMapWindow(winID) {
             try {
                 allTrackerWindows.splice(winID, 1);
             } catch (err) {
             }
         }

         function NewWindow(url,data) {
             var mypage = url + data;
             win = window.open(mypage)
             if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
         }

           function NewAlarmWindow(AlarmId) {
		    var target = "<%=alarmDetailPage%>";
                    var mypage = target + '?AlarmId=' + AlarmId;                    
					var myname='AlarmInfo';
					var w=<%=windowWidth%>;
					var h=<%=windowHeight%>;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
	        }

            function NewMessageWindow(MsgKey) {
		    var target = "<%=messageDetailPage%>";
                    var mypage = target + '?MsgKey=' + MsgKey;                    
					var myname='MessageInfo';
					var w=426;
					var h=400;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
	        }

//        function SensorInfoWindow(LicencePlate) {
//                var mypage = './Map/frmSensorMain.aspx?LicensePlate=' + LicencePlate;
//             var myname = 'Sensor Information';
//             var w = 535;
//             var h = 605;
//             var winl = (screen.width - w) / 2;
//             var wint = (screen.height - h) / 2;
//             winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
//             win = window.open(mypage, myname, '');
//             if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
//         }

               function SensorInfoWindow(LicensePlate) {
					var mypage='./Map/frmSensorMain.aspx?LicensePlate='+LicensePlate
					var myname='Sensors';
					var w=525;
					var h=720;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
            }

            function removeMarkersOnMap()
            {
                 var el = document.getElementById("mapframe");
                 var IframeLoaded = false;
                 var maxAttempt = 5;
                 var currentAttempt = 0;
                 while (!IframeLoaded && currentAttempt < maxAttempt) {
                     try {
                         currentAttempt = currentAttempt + 1;
                         if (el.contentWindow) {                                
                             el.contentWindow.removeMarkersOnMap();                                
                         }
                         else if (el.contentDocument) {
                             el.contentDocument.removeMarkersOnMap();                             
                         }
                         IframeLoaded = true;
                     } catch (err) {
                         IframeLoaded = false;
                     }
                 }
            }

            function removeHistoriesOnMap(mf)
            {
                 var el = document.getElementById(mf);
                 var IframeLoaded = false;
                 var maxAttempt = 5;
                 var currentAttempt = 0;
                 while (!IframeLoaded && currentAttempt < maxAttempt) {
                     try {
                         currentAttempt = currentAttempt + 1;
                         if (el.contentWindow) {                                
                             el.contentWindow.removeHistoriesOnMap(); 
                             el.contentWindow.removeAllPopups();
                         }
                         else if (el.contentDocument) {
                             el.contentDocument.removeHistoriesOnMap();
                             el.contentDocument.removeAllPopups();
                         }
                         $('#historiescount').html('0');
                         IframeLoaded = true;
                     } catch (err) {
                         IframeLoaded = false;
                     }
                 }
            }

         /*function VehicleInfoWindow(VehicleId) {
             var mypage = './frmVehicleDescription.aspx?VehicleId=' + VehicleId;
             var myname = 'Vehicle Description';
             var w = 440;
             var h = 700;
             var winl = (screen.width - w) / 2;
             var wint = (screen.height - h) / 2;
             winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + ',location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
             win = window.open(mypage, myname, winprops)
             if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
         }
         function NewAlarmWindow(AlarmId) {

		            var target = "frmAlarmInfo_G4S.aspx";
                    var mypage = target + '?AlarmId=' + AlarmId;                    
					var myname='AlarmInfo';
					var winl = (screen.width) / 2;
					var wint = (screen.height) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+',location=0,status=0,scrollbars=0,toolbar=0,menubar=0' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
	}
           
         function HistoryWindow(boxid) {
             var mypage = './History/frmhistmain_new.aspx?VehicleId=' + boxid;                    
					var myname='HistoryDetail';
					var w=1000;
					var h=1000;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+',location=0,status=0,scrollbars=0,toolbar=0,menubar=0' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
	}*/
    var geozoneCurrentAssignmentWin;
    function openNewWindow(wintitle, winURL, winWidth, winHeight) {
        win = new Ext.Window(
          {
              title: wintitle,
              width: winWidth,
              height: winHeight,
              layout: 'fit',
              maxWidth: window.screen.width,
              maxHeight: window.screen.height,
              maximizable: 'true',
              minimizable: 'true',
              resizable: 'true',
              closable: true,
              border: false,
              html: winURL
          }
          );

        win.show();        
    }

    function blinks(hide) {
        if(hide==1) {
            $('.blinking').show();
                hide = 0;
        }
    else { 
        $('.blinking').hide();
        hide = 1;
    }
        setTimeout("blinks("+hide+")",1000);
    }

    function GetGeozoneCurrentAssignment(geozoneId)
    {
        //$('#geozoneCurrentAssignmentWin').close();
        if(geozoneCurrentAssignmentWin)
            geozoneCurrentAssignmentWin.close();

        var url = "./GeoZone_Landmarks/frmViewVehicleGeozones.aspx?geozoneId=" + geozoneId;
        var urlToLoad = '<iframe id="geozoneCurrentAssignmentWin" name="geozoneCurrentAssignmentWin" width="100%" height="100%" frameborder="0" scrolling="yes" src="' + url + '"></iframe>';
        //openNewWindow('Current Assignment', urlToLoad, 500, 320);
        geozoneCurrentAssignmentWin = new Ext.Window(
          {
              title: 'Current Assignment',
              width: 500,
              height: 320,
              layout: 'fit',
              maxWidth: window.screen.width,
              maxHeight: window.screen.height,
              maximizable: 'true',
              minimizable: 'true',
              resizable: 'true',
              closable: true,
              border: false,
              html: urlToLoad
          }
          );

        geozoneCurrentAssignmentWin.show();
    }

    function redrawVehicleMarkers(){
        var el = document.getElementById(mapframe).contentWindow;

        el.markers.removeAllFeatures();
        if (el.parent.VehicleClustering) {
            el.markerstrategy.clearCache();
        }
        el.markers.addFeatures(el.vehicleFeatures);
    }

    function iniVehicleGridPopup() {
        $('#vehicleInfoPopover .popover').remove();
        
        $('[rel=bootstrapvehiclegridpopover]').bind({
            'mouseenter':function(el) {        
                //alert(el.clientY + ', ' + $(document).height());
                var clientY = el.clientY;
                var documentHeight = $(document).height();
                var placement = 'right';
                if(clientY < documentHeight / 3)
                    placement = 'bottom';
                else if(clientY > documentHeight * 2 / 3)
                    placement = 'top';

                var e=$(this);
                //e.unbind('mouseenter');
                e.attr('data-mouseover', 'true');
                $.get(e.data('poload'),function(d) {
                    //e.popover({content: (itimes++) + ' ' + d}).popover('show');
                    if(e.attr('data-mouseover') == 'false')
                        return;
                    e.popover({
                        content: function() {
                            return d;
                        },
                        placement: placement
                    }).popover('show');
                    e.attr('data-popup', 'true');
                });
            },
            'mouseleave': function(e) {
                var e=$(this);
                e.attr('data-mouseover', 'false');
                if(e.attr('data-popup') == 'false')
                        return;
                e.popover('hide');
            }
         });

        $('[rel=bootstrapreeferpretripresultpopover]').bind({
            'mouseenter':function(el) {        
                //alert(el.clientY + ', ' + $(document).height());
                var clientY = el.clientY;
                var documentHeight = $(document).height();
                var placement = 'right';
                if(clientY < documentHeight / 3)
                    placement = 'bottom';
                else if(clientY > documentHeight * 2 / 3)
                    placement = 'top';

                var e=$(this);
                //e.unbind('mouseenter');
                e.attr('data-mouseover', 'true');
                
                /*$.get(e.data('poload'),function(d) {
                    //e.popover({content: (itimes++) + ' ' + d}).popover('show');
                    if(e.attr('data-mouseover') == 'false')
                        return;
                    e.popover({
                        content: function() {
                            return d;
                        },
                        placement: placement
                    }).popover('show');
                    e.attr('data-popup', 'true');
                });*/
                e.popover({
                        content: function() {
                            return e.data('poload');
                        },
                        placement: placement
                    }).popover('show');
                e.attr('data-popup', 'true');
            },
            'mouseleave': function(e) {
                var e=$(this);
                e.attr('data-mouseover', 'false');
                if(e.attr('data-popup') == 'false')
                        return;
                e.popover('hide');
            }
         });
    }
    
 </script>
<script src="Scripts/NewMap/VehicleList_Reefer.js?v=<%=LastUpdatedVehicleListJs %>" type="text/javascript"></script>
<script src="Scripts/NewMap/ReeferImpact.js?v=<%=LastUpdatedReeferImpactJs %>" type="text/javascript"></script>
<script src="Scripts/NewMap/ReeferMaintenance.js?v=<%=LastUpdatedReeferMaintenanceJs %>" type="text/javascript"></script>
<script src="Scripts/NewMap/ReeferEvent.js?v=<%=LastUpdatedReeferEventJs %>" type="text/javascript"></script>
<script src="Scripts/NewMap/ReeferAlarm.js?v=<%=LastUpdatedReeferAlarmJs %>" type="text/javascript"></script>
<script src="Scripts/NewMap/ReeferCommandHistory.js?v=<%=LastUpdatedReeferCommandHistoryJs %>" type="text/javascript"></script>
<script src="Scripts/NewMap/ReeferReeferHistory.js?v=<%=LastUpdatedReeferReeferHistoryJs %>" type="text/javascript"></script>
<script type="text/javascript">

     blinks(1);

</script>
<%if (LoadVehiclesBasedOn == "hierarchy")
  {%>
    <script type="text/javascript" src="scripts/jquery.cookie.js"></script>
    <script src="reports/jqueryFileTree.js" type="text/javascript"></script>
    <script src="reports/splitter.js" type="text/javascript"></script>
    <script src="scripts/tablesorter/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script src="scripts/colResizable-1.3.min.js" type="text/javascript"></script>
    <script src="scripts/json2.js" type="text/javascript"></script>
    <link rel="stylesheet" href="scripts/tablesorter/themes/report/style.css" type="text/css" />
	<link href="reports/jqueryFileTree.css" rel="stylesheet" type="text/css" media="screen" />

    <style type="text/css">
        .style1
        {
            width: 45px;
        }
        
        .LeftPane {
            background: none repeat scroll 0 0 #FFFFFF;
            border-color: #BBBBBB #FFFFFF #FFFFFF #BBBBBB;
            border-style: solid;
            border-width: 1px;
            height: 300px;
            overflow: scroll;
            padding: 5px;
            width: 300px;
        }
        
        #vehicletreeview, #vehicledetails
        {
            padding: 0 5px;
        }
        
        #vehicledetails
        {
            /*width:450px;*/
            margin-right:16px;
        }
        
        /*
         * Splitter container. Set this to the desired width and height
         * of the combined left and right panes. In this example, the
         * height is fixed and the width is the full width of the body,
         * less the margin on the splitter itself.
         */
        #MySplitter {
	        height: 300px;
	        margin: 5px 40px 5px 0;
	        border: 4px solid #bdb;
	        /* No padding allowed */
        }
        /*
         * Left-side element of the splitter. Use pixel units for the
         * min-width and max-width; the splitter plugin parses them to
         * determine the splitter movement limits. Set the width to
         * the desired initial width of the element; the plugin changes
         * the width of this element dynamically.
         */
        #LeftPane {
	        background: #efe;
	        overflow: auto;
	        /* No margin or border allowed */
        }
        /*
         * Right-side element of the splitter.
         */
        #RightPane {
	        background: #f8fff8;
	        overflow: auto;
	        /* No margin or border allowed */
        }
        /* 
         * Splitter bar style; the .active class is added when the
         * mouse is over the splitter or the splitter is focused
         * via the keyboard taborder or an accessKey. 
         */
        #MySplitter .vsplitbar {
	        width: 6px;
	        background: #aca url(images/vgrabber.gif) no-repeat center;
        }
        #MySplitter .vsplitbar.active {
	        background: #da8 url(images/vgrabber.gif) no-repeat center;
	        opacity: 0.7;
        }
        
        ul.jqueryFileTree A
        {
            text-align:left;
        }
        
        #organizationHierarchyTree
        {
            position:absolute;
            width:700px;
            height: 400px;
            left: 20px;
            top: 60px;
            z-index: 200;
            background-color:White;
            border: 2px solid #cccccc;
            padding: 10px;
        }
        
        .kd-button {
            -moz-transition: all 0.218s ease 0s;
            background-color: #F5F5F5;
            background-image: -moz-linear-gradient(center top , #F5F5F5, #F1F1F1);
            /*border: 1px solid rgba(0, 0, 0, 0.1);*/
            border: 1px solid #D8D8D8;
            border-radius: 2px 2px 2px 2px;
    
            color: #444444;
            display: inline-block;
            font-size: 100%;
            font-family: arial,​sans-serif;
            font-weight: bold;
            height: 27px;
            line-height: 27px;
            min-width: 54px;
            padding: 0 8px;
            text-align: center;
        }

        .kd-button:hover {
            -moz-transition: all 0s ease 0s;
            background-color: #F8F8F8;
            background-image: -moz-linear-gradient(center top , #F8F8F8, #F1F1F1);
            border: 1px solid #C6C6C6;
            box-shadow: 0 1px 1px rgba(0, 0, 0, 0.1);
            color: #333333;
            text-decoration: none;
        }

        .kd-button-disabled, .kd-button-disabled:hover {
            -moz-transition: all 0.218s ease 0s !important;
            background-color: #F5F5F5 !important;
            background-image: -moz-linear-gradient(center top , #F5F5F5, #F1F1F1) !important;
            border: 1px solid #D8D8D8 !important;
            border-radius: 2px 2px 2px 2px !important;
            color: #cccccc !important;
            font-weight: normal !important;
            box-shadow: none !important;        
        }
        </style>

        <script language="javascript">
			<!--
            OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
            var vehicletreeviewIni = false;
            var selectedOrganizationHierarchyNodeCode = '';
            var selectedOrganizationHierarchyFleetName = ''

            function inifiletree(inipath) {
                $('#vehicletreeview').fileTree({ root: '', script: 'reports/vehicleListTree.asmx/FetchVehicleList', expanded: inipath,
                    expandSpeed: 200, collapseSpeed: 200, vehicledetails: 'vehicledetails',
                    highlightVehicleSelection: false,
                    searchScript: 'reports/vehicleListTree.asmx/SearchOrganizationHierarchy'
                },
                /*
                * Call back function when you click left pane tree folder.
                */
                        function (NodeCode, fleetId, FleetName) {
                            //$('#OrganizationHierarchyNodeCode').val(NodeCode);
                            //$('#OrganizationHierarchyBoxId').val('');
                            selectedOrganizationHierarchyNodeCode = NodeCode;
                            selectedOrganizationHierarchyFleetName = FleetName;
                            TempSelectedOrganizationHierarchyFleetId = fleetId;
                        },

                /*
                * Call back function when you click right pane vehicle list.
                */
                        function (BoxId) {
                            //alert('BoxId: ' + BoxId);
                            //$('#OrganizationHierarchyBoxId').val(BoxId);
                        }
                    );
            }

            function applyOrganizationHierarchy() {
                organizationHierarchy.setText(selectedOrganizationHierarchyFleetName);
                $('#organizationHierarchyTree').hide();
                DefaultOrganizationHierarchyFleetId = TempSelectedOrganizationHierarchyFleetId;
                DefaultOrganizationHierarchyNodeCode = selectedOrganizationHierarchyNodeCode;
                mainstore.load(
                           {
                               params:
                              {
                                  QueryType: 'GetfleetPosition',
                                  fleetID: DefaultOrganizationHierarchyFleetId,
                                  start: 0,
                                  limit: VehicleListPagesize
                              }
                           }
                       );
            }

            function cancelOrganizationHierarchy() {
                $('#organizationHierarchyTree').hide();
                TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
            }
            
			//-->
    </script>
<%} %>
</head>
<body> 

<%if (LoadVehiclesBasedOn == "hierarchy")
  {%>
<div id="organizationHierarchyTree" style="display:none;">
    <div id="ohsearchbar" class="formtext"><asp:Label ID="Label8" runat="server" CssClass="tableheading" Text="Search: "></asp:Label>
        <input type="text" id="ohsearchbox" class="ohsearch" />
        <a href="javascript:void(0);" onclick="onsearchbtnclicked('reports/vehicleListTree.asmx/SearchOrganizationHierarchy');"><img src="../images/searchicon.png" border="0" /></a>
        <asp:Label ID="Label10" runat="server" style="color:#666666;display:none" Text="(Type in at least 3 characters to search)"></asp:Label>
    </div>
    <div id="ohsearchresult">
        <div id="ohsearchresulttitle">
            Search Result: <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">Close</a>
        </div>
        <div id="ohsearchresultlist">
            <ul></ul>
        </div>
    </div>
    <div id="MySplitter">
                                                   
        <div id="LeftPane">
		    <div id="vehicletreeview" class="demo"></div>		                                                
        </div>     
        <div id="RightPane">
            <div id="vehicledetails">
                <table cellspacing="0" class="vehiclelisttbl tablesorter" id="vehiclelisttbl">
                    <thead>
                    <tr>
                        <th>Vehicle</th>
                                                                                                                                                                                                     
                    </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div> 
    <div>
        <input type="button" class="kd-button" onclick="applyOrganizationHierarchy();" id="Button1" value="OK" />
        <input type="button" class="kd-button" onclick="cancelOrganizationHierarchy();" id="Button2" value="Cancel" />
    </div>
</div> 
<%} %>

    <iframe id="exportframe" name="exportframe" style="display:none"></iframe>
    <form id="exportform" method="post" target="exportframe" action="./MapNew/frmExportData.aspx">
        <input type="hidden" id="exportdata" name="exportdata" value="" />
        <input type="hidden" id="filename" name="filename" value="" />
        <input type="hidden" id="formatter" name="formatter" value="" />
    </form>

    <div id="vehicleInfoPopover"></div>
<!-- Google Tag Manager -->
<noscript><iframe src="//www.googletagmanager.com/ns.html?id=GTM-K49R9G"
height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>
<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
})(window,document,'script','dataLayer','GTM-K49R9G');</script>
<!-- End Google Tag Manager -->
</body>
</html>
