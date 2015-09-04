var geozonesstore;
var geozonegrid;
var landmarksstore;
var landmarkgrid;
var mapframe = "nmapframe";
var geozonegridloaded = false;
var landmarkgridloaded = false;
var tabs;
var northmappanel;

var historystore;
var historyPageStore;
var historygrid;
var historyGridForm;
var IconTypeName = "circle";

var historyStopStore;
var historyStopGrid;

var historyTripStore;
var historyTripGrid;

var historyIni = false;
var historyIniVehicleId = '';
var historyVehicleStoreLoaded = false;
var historyFleetId = '';
var historyFleetName = '';

var historyAddressStore;
var historyAddressGrid;
var historyAddressResetField;
var historyAddressFleetId;
var historyAddressFleetName;

var historyDateFrom;
var historyTimeFrom
var historyDateTo;
var historyTimeTo;

var organizationHierarchy;
var historyOrganizationHierarchy;

var fleetButton;
var historyFleetButton;
var clearSearchBtn

var mainstore;
var pagesize = 10000;

 var historyVehicleStore;
 var historyHiddenFleet;
 var fleetWin;

 var historyTripsNum = 0;

 var AllHistoryRecords = [];

 var gridStatus = 'expanded'; 
 var gridOriginalHeight;
 var gridOriginalWidth;
 var historyPagerDoc = '';
 var FromClosestVehicles = false;

 var mapStatus = 'collapse';
 var mapGridOriginalHeight;
 var mapGridOriginalWidth;

 var loadingMask;
 var searchingMask;

 String.prototype.trim = function () {
     return this.replace(/^\s+|\s+$/g, '');
 }

 Ext.override(Ext.grid.View, { enableTextSelection: true });
 Ext.override(Ext.menu.Menu, {
     hideMode: 'display'
 });
 
Ext.Loader.setConfig(
{
    enabled: true
}
);
Ext.Loader.setPath('Ext.ux', './extjs/examples/ux');
Ext.Loader.setPath('Ext.ux.exporter', './sencha/Ext.ux.Exporter'); // Only the Ext.ux.exporter.* classes will be searched in ./something/exporter'
Ext.require([
'Ext.window.*',
'Ext.ux.grid.FiltersFeature',
'Ext.ux.AspWebAjaxProxy',
'Ext.ux.form.MultiSelect',
'Ext.ux.exporter.Exporter.*',
	 'Ext.ux.exporter.excelFormatter.*',
	 'Ext.ux.exporter.csvFormatter.*',
	 'Ext.ux.exporter.Button.*'
]);

Ext.define('VehicleList',
   {
       extend: 'Ext.data.Model',
       fields: [
       //'BoxId',
      {
      name: 'BoxId', type: 'int'
  },
      'Description',
      'StreetAddress',
      {
          name: 'OriginDateTime', type: 'date', dateFormat: 'c'
      }
      ,
       //      {
       //          name: 'convertedDate', type: 'date'// , dateFormat : 'c'
       //         , defaultValue: '', convert: function (value, record) {
       //             var localDateStr;
       //             localDateStr = record.get('OriginDateTime').toLocaleString();
       //             var dt = Ext.Date.parse(localDateStr, 'F-d-y g:i:s A');
       //             // March - 15 - 12 12 : 41 : 27 PM for mozilla
       //             if (dt == undefined) {
       //                 var currentDate = record.get('OriginDateTime');
       //                 // Wed Dec 07 2011 10 : 58 : 19 GMT - 0500 (Eastern Standard Time) for all other browsers
       //                 dt = new Date(currentDate);
       //             }
       //             return dt;
       //         }
       //      }
       //      ,
      {
      name: 'convertedDisplayDate',
      convert: function (value, record) {
          var currentDate = record.get('OriginDateTime');
          var dt = new Date(currentDate);
          var newDt = Ext.Date.format(dt, userdateformat);
          return newDt;
      }
  }
      ,
      'Speed',
      'BoxArmed',
      'VehicleStatus',
      'PTO',
      'History',
      'VehicleId',
      'LicensePlate',
      'Latitude',
      'Longitude'
       //      {
       //          name: 'Latitude', type: 'float'
       //      }
       //      ,
       //      {
       //          name: 'Longitude', type: 'float'
       //      }
      ,
      'IconTypeName',
      'PretripResult',
      'ReeferLastAlarm',
       //      {
       //          name: 'CustomSpeed', type: 'int'
       //      }
       //      ,
       //'CustomSpeed',
      {
      name: 'CustomSpeed', type: 'int'
  },
      'MyHeading',
      'icon',
      'Driver',
      'ImagePath',
      'VehicleTypeName',
      'ExtraInfo', 'SensorMask',
     { name: 'isSelected', type: 'int', convert: function (value, record) { return 0; } }
     , 'ReeferState'
     , 'Micro'
     , 'ControllerType'
     , 'ReeferPower'
     , 'PowerOnOff'
     , 'ModeOfOp'
     , 'SDoor'
     , 'AFAX'
     , 'RF_Setpt'
     , 'RF_Ret'
     , 'RF_Dis'
     , 'RF_SensorProbe'
     , 'RF_Amb'
     , 'RF_Setpt2'
     , 'RF_Ret2'
     , 'RF_Dis2'
     , 'RF_SensorProbe2'
     , 'RF_Setpt3'
     , 'RF_Ret3'
     , 'RF_Dis3'
     , 'RF_SensorProbe3'
     , 'FuelLevel'
     , 'FuelLevelGallon'
     , 'RPM'
     , 'BatteryVoltage'
     , 'EngineHours'
     , 'TetherOnOff'
      ],
       idProperty: 'BoxId'
   }
   );

   
Ext.define('HistoryListModel',
   {
       extend: 'Ext.data.Model',
       fields: [
      //'BoxId', 
      {
        name: 'BoxId', type: 'int'
      },
      'VehicleId', 'LicensePlate', 'Description', 'DateTimeReceived', 'DclId', 'BoxMsgInTypeId', 'BoxMsgInTypeName','BoxProtocolTypeId',
      'BoxProtocolTypeName','CommInfo1','CommInfo2','ValidGps','Latitude','Longitude','Heading','SensorMask','CustomProp','BlobDataSize','SequenceNum',
      'StreetAddress',
      //'Speed',
      {
        name: 'Speed', type: 'int',
        convert: function (value, record) {
              if(value >=0 || value < 0)
                return value * 1;
              else
                return -1;
          }
      },
      'BoxArmed','MsgDirection','Acknowledged','Scheduled','MsgDetails','MyDateTime','MyHeading','dgKey','CustomUrl','HistoryInfoId','chkBoxShow',
      {
          name: 'OriginDateTime', type: 'date', dateFormat: 'c'
      }
      ,'icon'
//      ,
//      {
//          name: 'convertedDate', type: 'date'// , dateFormat : 'c'
//         , defaultValue: '', convert: function (value, record) {
//             var localDateStr;
//             localDateStr = record.get('OriginDateTime').toLocaleString();
//             var dt = Ext.Date.parse(localDateStr, 'F-d-y g:i:s A');
//             // March - 15 - 12 12 : 41 : 27 PM for mozilla
//             if (dt == undefined) {
//                 var currentDate = record.get('OriginDateTime');
//                 // Wed Dec 07 2011 10 : 58 : 19 GMT - 0500 (Eastern Standard Time) for all other browsers
//                 dt = new Date(currentDate);
//             }
//             return dt;
//         }
//      }      
      ]
   }
   );

  Ext.define('HistoryAddressModel',
   {
       extend: 'Ext.data.Model',
       fields: [       
          {
              name: 'BoxId', type: 'int'
          },
          'VehicleId', 'LicensePlate', 'Description', 'FleetId'
      ]
   }
   );

//Ext.define('FleetList',
//   {
//       extend: 'Ext.data.Model',
//       fields: [
//      'OrganizationName',
//      'FleetId',
//      'FleetName',
//      'Description',
//      'IconTypeName',
//      'MyHeading'
//      ]
//   }
//   );

Ext.define('HistoryVehicleList',
       {
           extend: 'Ext.data.Model',
           fields: [
              'LicensePlate',
              'BoxId',
              'VehicleId',
              'VinNum',
              'MakeName',
              'ModelName',
              'VehicleTypeName',
              'StateProvince',
              'ModelYear',
              'Color',
              'Description',
              'FleetId',
              'FleetName'
          ]
       }
   );

Ext.define('HistoryCommModeList',
       {
           extend: 'Ext.data.Model',
           fields: [
              'CommModeName',
              'DclId'
          ]
       }
   );

Ext.define('HistoryMessageModel',
       {
           extend: 'Ext.data.Model',
           fields: [
              'BoxMsgInTypeId',
              'BoxMsgInTypeName'
          ]
       }
   );

Ext.define('HistoryGridModel',
   {
       extend: 'Ext.data.Model',
       fields: [
          'BoxId',
          'VehicleId',
          'LicensePlate',
          'Description',
          'DateTimeReceived',
          'DclId',
          'BoxMsgInTypeId',
          'BoxMsgInTypeName',
          'BoxProtocolTypeId',
          'BoxProtocolTypeName',
          'CommInfo1',
          'CommInfo2',
          'ValidGps',
          'Latitude',
          'Longitude',
          'Heading',
          'SensorMask',
          'CustomProp',
          'BlobDataSize',
          'SequenceNum',
          'StreetAddress',
          'OriginDateTime',
          'Speed',
          'BoxArmed',
          'MsgDirection',
          'Acknowledged',
          'Scheduled',
          'MsgDetails',
          'MyDateTime',
          'MyHeading',
          'dgKey',
          'CustomUrl',
          'chkBoxShow'
          ]
   }
   );

Ext.define('HistoryStopModel',
   {
       extend: 'Ext.data.Model',
       fields: [
           'StopIndex',
           'BoxId',
            {
                name: 'ArrivalDateTime', type: 'date'// , dateFormat : 'c'
                , defaultValue: '', convert: function (value, record) {
                    var dt = Ext.Date.parse(value, userdateformat); //12/21/2012 5:55:51 PM
                    return dt;
                }
            }
             ,
            'Location',
            {
                name: 'DepartureDateTime', type: 'date', //dateFormat: 'c'
                defaultValue: '', convert: function (value, record) {
                    var dt = Ext.Date.parse(value, userdateformat); //12/21/2012 5:55:51 PM
                    return dt;
                }
            },
            'StopDuration',
            'Remarks',
            'Latitude',
            'Longitude',
            'StopDurationVal',
            'VehicleId',
            'IsLandmark',
            'chkBoxShow'
      ]
   }
   );

Ext.define('HistoryTripModel',
   {
       extend: 'Ext.data.Model',
       fields: [
           'VehicleId',
           'TripId',
           'Description',
            {
                name: 'DepartureTime', type: 'date', dateFormat: 'c'
            },
            {
                name: 'ArrivalTime', type: 'date', dateFormat: 'c'
            },
            '_From',
            '_To',
            'Duration',
            'StartOdometer',
            'Distance',
            'StopDuration',
            'IdlingTime',
            'FuelConsumed',
            'MaxSpeed',
            'TotalHarshAcceleration',
            'TotalHarshBraking',
            'TotalExtremeAcceleration',
            'TotalExtremeBraking',
            'TotalSeatBelt',
            'TotalSpeeding',
            'LicensePlate',
            'VIN',
            'Color',
            'Make',
            'Model',
            'Year',
            'BoxId',
            'Driver'
      ]
   }
   );


   Ext.onReady(function () {
       //   Ext.tip.QuickTipManager.init();
       // setup the state provider, all state information will be saved to a cookie
       // Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));
       Ext.QuickTips.init();

       // setup the state provider, all state information will be saved to a cookie
       //Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));
       Ext.state.Manager.setProvider(new Ext.state.CookieProvider({
           expires: new Date(new Date().getTime() + (1000 * 60 * 60 * 24 * 30)) // 30 days
       }));

       var selectedVehIds = "";
       var lastProxyFinished = false;
       var mapLoading = false;
       var currentSelected = "";
       var wincounter = 0;
       var proxyTimeOut = 120000;
       var fleetDefaultText = 'All Vehicles...';
       var taskRunning = false;
       var mapHTML = '<iframe scrolling="no" src="./MapNew/OpenLayerMaps_Reefer.aspx"';
       var mapStyle = 'style="Height:100%; width:100%;  border:0;margin:0px"';

       var nframeadded = false, sframeadded = false, eframeadded = false;
       loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." });
       searchingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Searching..." });
       // Default

       var ActiveTabs =
   {
       Alarms: 0,
       Vehicles: 1,
       Messages: 2
   };

       var LoadStates =
   {
       GridLoading: 0,
       MapLoading: 1,
       AlarmLoading: 2,
       FleetsLoading: 3,
       MainStoreLoading: 4,
       GettingUpdates: 5,
       GettingAlarmUpdates: 6,
       MappingVehicles: 7,
       MainStoreLoaded: 8,
       AlarmsLoaded: 9,
       MessagesLoading: 10,
       MessagesLoaded: 11,
       GettingMessageUpdates: 6
   };

       var currentState = LoadStates.FleetsLoading;
       var activetab = ActiveTabs.Vehicles;

       //    var pagesize = 2;
       var alarminterval = 5000;
       var messageinterval = 5000;
       var dateformat = userdateformat;
       var initialData = "";
       var soundPresent = false;
       // var newPosition = 10;
       var statusColorString = '#00C000';

       var sensorPage = './Map/frmSensorMain.aspx?LicensePlate=';
       var historyPage = './History/frmhistmain_new.aspx?VehicleId=';

       var template = '<span style="color:{0};">{1}</span>';

       var selModel = Ext.create('Ext.selection.CheckboxModel',
   {
       checkOnly: true,
       enableKeyNav: false,
       listeners:
      {
          selectionchange: function (selModel, selections) {
              try {
                  //vehiclegrid.down('#finditmenu').setDisabled(selectedVehicleBoxId < 0);
                  vehiclegrid.down('#trackitmenu').setDisabled(selectedVehicleBoxId < 0);
                  vehiclegrid.down('#streetViewMenu').setDisabled(selectedVehicleBoxId < 0);
                  //vehiclegrid.down('#streetView2Button').setDisabled(selections.length == 0);               
                  vehiclegrid.down('#updatePositionMenu').setDisabled(selections.length == 0);
                  vehiclegrid.down('#clearAllMenu').setDisabled(selections.length == 0);
                  vehiclegrid.down('#GetReeferMenu').setDisabled(selections.length == 0);
                  mainstore.each(function (record, idx) {
                      //do whatever you want with the record
                      record.set('isSelected', 0);
                  });

                  //if (!firstLoad) {
                  var zoommap = firstLoad;
                  try {
                      //mapSelections(selections, zoommap);
                      mapSelections(selections, true);
                      ////////////////////////////////////////////////////////////////////////////
                  }
                  catch (err) { alert(err); }
                  //}
                  firstLoad = false;

              }
              catch (err) {
              }
          }
      }
   }
   );
       var maxtrymapselection = 0;
       function mapSelections(selections, zoommap) {
           try {
               var el = document.getElementById(mapframe).contentWindow;

               if (typeof el.allVehicles != "undefined")
                   el.allVehicles.length = 0;
               //               if (el.parent.VehicleClustering) {
               //               }
               //               else if (el.parent.overlaysettings.vehicleDrivers)
               //                   el.vehicleDriversLayer.destroyFeatures();
               //               else if (el.parent.overlaysettings.vehiclenames)
               //                   el.vehiclenamesLayer.destroyFeatures();

               //el.markers.clearMarkers();
               el.markers.removeAllFeatures();
               if (el.parent.VehicleClustering) {
                   el.markerstrategy.clearCache();
               }
               el.vehicleFeatures = [];

               /////////////// reload vehicles on map /////////////////////////////////////
               if (selections.length > 0) {
                   var mapJsonData = new Array();
                   Ext.each(selections, function (exirecord) {
                       exirecord.data.isSelected = 1;
                       var newIcon = "";
                       var today = new Date();
                       var posExpireDate = new Date();
                       posExpireDate.setTime(today.getTime() - (60 * PositionExpiredTime * 1000));

                       exirecord.data.icon = getIcon(exirecord, posExpireDate);
                       mapJsonData.push(exirecord.data);
                   }
                    );
                   currentState = LoadStates.MappingVehicles;

                   if (mapJsonData.length > 0) {
                       mapVehicles(true, mapJsonData, true, false, zoommap);
                   }
               }
           }
           catch (err) {
               if (maxtrymapselection < 10) {
                   maxtrymapselection++;
                   setTimeout(function () { mapSelections(selections); }, 1000);
               }
           }
       }


       function mapVehicles(map, vehiclesData, isInitial, zoomVehicles, zoomtomap) {
           zoomtomap = typeof zoomVehicles !== 'undefined' ? zoomtomap : true;
           try {
               if (map == true) {
                   if (zoomVehicles) {
                       mapSelecteds(vehiclesData, mapframe);
                   }
                   else {
                       if (isInitial) {
                           ShowMapFrameData(vehiclesData, true, mapframe, zoomtomap);
                       }
                       else {
                           ShowMapFrameData(vehiclesData, false, mapframe, zoomtomap);
                       }
                   }
               }
           }
           catch (err) {
           }
       }

       function mapHistories(selections, isInitial, s, stopHistory) {
           zoomtomap = true;

           try {
               var mapHistoryJsonData = new Array();
               if (s) {
                   selections = Ext.Array.sort(selections, function (r1, r2) {
                       if (stopHistory) {
                           if (r1.data.StopIndex < r2.data.StopIndex)
                               return 1;
                           if (r1.data.StopIndex > r2.data.StopIndex)
                               return -1;
                           return 0;
                       }
                       else {
                           if (r1.data.OriginDateTime < r2.data.OriginDateTime)
                               return 1;
                           if (r1.data.OriginDateTime > r2.data.OriginDateTime)
                               return -1;
                           return 0;
                       }

                   });
               }
               if (selections.length > 0) {

                   Ext.each(selections, function (exirecord) {

                       if (isNumber(exirecord.data.Latitude) && isNumber(exirecord.data.Longitude) && (exirecord.data.Latitude != 0 || exirecord.data.Longitude != 0)) {

                           /*var today = new Date();
                           var posExpireDate = new Date();
                           posExpireDate.setTime(today.getTime() - (60 * PositionExpiredTime * 1000));*/

                           if (exirecord.data.icon == undefined || exirecord.data.icon == '') {
                               var newIcon = "";
                               if (stopHistory) {
                                   if (exirecord.data.Remarks == 'Idling')
                                       newIcon = "../Bigicons/Idle.ico";
                                   else if (exirecord.data.StopDurationVal / 60 < 15)
                                       newIcon = "../Bigicons/Stop_3.ico";
                                   else if (exirecord.data.StopDurationVal / 60 < 60)
                                       newIcon = "../Bigicons/Stop_15.ico";
                                   else
                                       newIcon = "../Bigicons/Stop_60.ico";

                                   var dthis = new Date(exirecord.data.ArrivalDateTime);
                                   var newDthis = Ext.Date.format(dthis, userdateformat);
                                   exirecord.data.convertedArrivalDisplayDate = newDthis;
                               }
                               else {
                                   if (exirecord.data.Speed != 0) {
                                       newIcon = "Green" + IconTypeName + exirecord.data.MyHeading + ".ico";
                                   }
                                   else {
                                       newIcon = "Red" + IconTypeName + ".ico";
                                   }
                               }
                               exirecord.data.icon = newIcon;
                           }

                           var dt = new Date(exirecord.data.OriginDateTime);
                           var newDt = Ext.Date.format(dt, userdateformat);
                           exirecord.data.convertedDisplayDate = newDt;


                           mapHistoryJsonData.push(exirecord.data);
                       }

                   }
                        );

               }

               if (mapHistoryJsonData.length > 0) {
                   if (isInitial) {
                       ShowHistoryMapFrameData(mapHistoryJsonData, true, mapframe, zoomtomap);
                   }
                   else {
                       ShowHistoryMapFrameData(mapHistoryJsonData, false, mapframe, zoomtomap);
                   }
               }
               else {
                   removeHistoriesOnMap(mapframe);
               }

           }
           catch (err) {
           }
       }



       var trackpanel = Ext.create('Ext.Panel',
   {
       id: 'trackpanel',
       autoHeight: true,
       titleCollapse: true,
       unstyled: true,
       layout: 'fit',
       border: false,
       width: '100%',
       maxWidth: window.screen.width,
       html: '<iframe id="trackwindow" name="trackwindow" src="./OpenLayerMap.aspx" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',
       margins: '0 0 0 0',
       autoScroll: true
   }
   );

       Ext.define('UpdatePositonData',
   {
       extend: 'Ext.data.Model',
       fields: ['mesg']
   }
   );

       function onUpdatePositionReceived(operation) {
           try {

               var data = Ext.decode(operation.response.responseText);
               // process server response here
               if (data.d != '-1' && data.d != "0") {
                   var retData;
                   if (data.d) {
                       retData = eval(data.d);
                       Ext.MessageBox.show(
               {
                   title: 'UpdatePosition Command Status',
                   msg: retData,
                   buttons: Ext.MessageBox.OK,
                   icon: Ext.MessageBox.INFO
               }
               );

                   }
               }
           }
           catch (err) {
           }
       }

       function doMin() {
           this.collapse(false);
           this.alignTo(document.body, 'bl-bl');
       }

       var mapit = Ext.create('Ext.Button',
   {
       text: 'FindIt',
       id: 'mapitButton',
       //    renderTo : Ext.getBody(),
       tooltip: 'Map the selected vehicle',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       disabled: true,
       handler: function () {
           // console.log("selected " + selectedVehIds);
           try {
               mapLoading = true;

               if (selectedVehicleBoxId > 0) {
                   var gridindex = 0;
                   vehiclegrid.getStore().each(function (record) {
                       if (record.data.BoxId == selectedVehicleBoxId) {
                           if (!vehiclegrid.getSelectionModel().isSelected(gridindex))
                               vehiclegrid.getSelectionModel().select(gridindex, true, false);
                           return false;
                       }
                       gridindex++;

                   });

                   var selectedBoxs = new Array();
                   selectedBoxs.push(selectedVehicleData);
                   mapSelecteds(selectedBoxs, "nmapframe");
               }
               mapLoading = false;
           }
           catch (err) {
           }
       }
   }
   );

       var updatePosition = Ext.create('Ext.Button',
   {
       text: 'Update Position',
       id: 'updatePositionButton',
       tooltip: 'Map selected vehicle on map',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       disabled: true,
       handler: function () {
           try {
               var currentTicked = vehiclegrid.getSelectionModel().getSelection();
               var selectedBoxs = "";
               Ext.each(currentTicked, function (selectedRec, i) {
                   selectedBoxs = selectedBoxs + "," + selectedRec.data.BoxId + ",";
               }
            );
               var mapstore = new Ext.data.Store(
            {
                proxy: new Ext.ux.AspWebAjaxProxy(
               {
                   //
                   url: './Vehicles_Reefer.aspx/UpdatePosition',
                   timeout: proxyTimeOut,
                   actionMethods:
                  {
                      create: 'POST',
                      destroy: 'DELETE',
                      read: 'POST',
                      update: 'POST'
                  }
                  ,
                   extraParams:
                  {
                      boxIDs: selectedBoxs
                  }
                  ,
                   reader:
                  {
                      type: 'json',
                      // model : 'VehicleList'
                      model: 'UpdatePositonData'
                  }
                  ,
                   headers:
                  {
                      'Content-Type': 'application/json; charset=utf-8'
                  }
               }
               )
            }
            );

               var operation = new Ext.data.Operation(
            {
                action: 'read'
            }
            );
               mapstore.proxy.read(operation, onUpdatePositionReceived, mapstore);
           }
           catch (err) {
           }
       }
   }
   );

       var feedback = Ext.create('Ext.Button',
   {
       text: 'Feedback',
       id: 'feedbackButton',
       tooltip: 'Want to improve our map please provide feedback..',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               var feedbackURL = "./Feedback.aspx";
               var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + feedbackURL + '"></iframe>';
               openWindow('Map<sup>beta</sup> Feedback', urlToLoad, 720, 360);
           }
           catch (err) {
           }
       }
   }
   );


       var clearall = Ext.create('Ext.Button',
   {
       text: 'ClearAll',
       id: 'clearAllButton',
       tooltip: 'Clear all selected vehicles',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               vehiclegrid.getSelectionModel().deselectAll(false);
           }
           catch (err) {
           }
       }
   }
   );

       var findvehiclesdrivers = Ext.create('Ext.Button',
   {
       text: 'Find Vehicles/Drivers',
       id: 'findVehiclesDriversButton',
       tooltip: 'Find Vehicles/Drivers',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               var url = "./DriverFinder/Default.aspx";

               window.open(url);
           }
           catch (err) {
           }
       }
   }
   );

       var vehicleLegendWin;

       var legend = Ext.create('Ext.Button',
   {
       text: 'Legend',
       id: 'legendButton',
       tooltip: 'Legend of Date/Time Color',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               if (!vehicleLegendWin) {
                   var legendURL = "./Legend.aspx";
                   var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + legendURL + '"></iframe>';
                   vehicleLegendWin = openWindow('Map Legend', urlToLoad, 400, 220);
               }
               else {
                   if (vehicleLegendWin.isVisible()) {
                       vehicleLegendWin.hide();
                   } else {
                       vehicleLegendWin.show();
                   }
               }

           }
           catch (err) {
           }
       }
   }
   );

       organizationHierarchy = Ext.create('Ext.Button',
   {
       text: DefaultOrganizationHierarchyFleetName == '' ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName,
       id: 'organizationHierarchyButton',
       tooltip: 'Organization Hierarchy',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               $('#organizationHierarchyTree').show();
               if (!vehicletreeviewIni) {
                   vehicletreeviewIni = true;
                   $("#MySplitter").splitter({
                       type: "v",
                       outline: true,
                       minLeft: 100, sizeLeft: 280, minRight: 100,
                       resizeToWidth: true,
                       cookie: "vsplitter",
                       accessKey: 'I'
                   });

                   inifiletree(OrganizationHierarchyPath);

                   $('#vehiclelisttbl').tablesorter();
                   $('#vehiclelisttbl').colResizable({ headerOnly: true });
               }
           }
           catch (err) {
           }
       }
   }
   );

       fleetButton = Ext.create('Ext.Button',
   {
       text: DefaultFleetName == '' ? DefaultFleetID : DefaultFleetName,
       id: 'fleetButton',
       tooltip: 'Select a fleet',
       cls: 'cmbfonts',
       icon: 'preview.png',
       textAlign: 'left',
       handler: function () {
           try {
               var url = "./Widgets/fleet.aspx?fleetId=" + SelectedFleetId + '&f=fleetButton';
               var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
               //if (fleetWin==undefined)
               fleetWin = openWindow('Select a fleet', urlToLoad, 400, 150);
               //else
               //{
               //     fleetWin.show();
               //}
           }
           catch (err) {
           }
       }
   }
   );

       historyOrganizationHierarchy = Ext.create('Ext.Button',
   {
       text: DefaultOrganizationHierarchyFleetName == '' ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName,
       id: 'historyOrganizationHierarchyButton',
       tooltip: 'Organization Hierarchy',
       cls: 'cmbfonts',
       textAlign: 'left',
       width: 230,
       style: { marginLeft: '1px' },
       handler: function () {
           try {
               var mypage = '../../Widgets/OrganizationHierarchy.aspx?nodecode=' + HistoryOrganizationHierarchyNodeCode;
               var myname = 'OrganizationHierarchy';
               var w = 740;
               var h = 440;
               var winl = (screen.width - w) / 2;
               var wint = (screen.height - h) / 2;
               winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
               win = window.open(mypage, myname, winprops)
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
               return false;
           }
           catch (err) {
           }
       }
   }
   );

       historyFleetButton = Ext.create('Ext.Button',
   {
       text: SelectedFleetName == '' ? SelectedFleetId : SelectedFleetName,
       id: 'historyFleetButton',
       tooltip: 'Select a fleet',
       cls: 'cmbfonts',
       icon: 'preview.png',
       textAlign: 'left',
       width: 230,
       style: { marginLeft: '5px' },
       handler: function () {
           try {
               var url = "./Widgets/fleet.aspx?fleetId=" + historyFleetId + '&f=historyFleetButton';
               var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
               //if (fleetWin==undefined)
               fleetWin = openWindow('Select a fleet', urlToLoad, 400, 150);
               //else
               //{
               //     fleetWin.show();
               //}  
           }
           catch (err) {
           }
       }
   }
   );

       var labelonoff = Ext.create('Ext.Button',
   {
       text: ifShowClusteredVehicleLabel ? 'Hide Label' : 'Show Label',
       id: 'labelonoffButton',
       tooltip: 'Hide/Show Label',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               if (ifShowClusteredVehicleLabel) {
                   ifShowClusteredVehicleLabel = false;
                   labelonoff.setText('Show Label');
               }
               else {
                   ifShowClusteredVehicleLabel = true;
                   labelonoff.setText('Hide Label');
               }
               redrawVehicleMarkers();
           }
           catch (err) {
           }
       }
   }
   );

       clearSearchBtn = Ext.create('Ext.Button',
   {
       text: 'Clear Search',
       id: 'clearSearchBtn',
       tooltip: 'Hide/Show Label',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       hidden: true,
       handler: function () {
           try {
               var fleetId;
               if (LoadVehiclesBasedOn == 'fleet') {
                   fleetId = DefaultFleetID;
               }
               else {
                   fleetId = DefaultOrganizationHierarchyFleetId;
               }

               loadingMask.show();

               mainstore.load(
                {
                    params:
                    {
                        QueryType: 'GetfleetPosition',
                        fleetID: fleetId,
                        start: 0,
                        limit: VehicleListPagesize
                    }
                });
               clearSearchBtn.hide();

               /* clear map markers */
               var el = document.getElementById(mapframe);
               if (el.contentWindow) {
                   el.contentWindow.removeMarkersOnMap();
                   el.contentWindow.searchArea.removeAllFeatures();
               }
               else if (el.contentDocument) {
                   el.contentDocument.removeMarkersOnMap();
                   el.contentDocument.searchArea.removeAllFeatures();
               }

           }
           catch (err) {
           }
       }
   }
   );


       var sendmessage = Ext.create('Ext.Button',
   {
       text: 'Send Message',
       id: 'sendmessageButton',
       tooltip: 'Send Message',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               /*var legendURL = "./Legend.aspx";
               var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + legendURL + '"></iframe>';
               openWindow('Map<sup>beta</sup> Legend', urlToLoad, 400, 220);
               NewMessageWindow();*/

               var mypage = './Messages/frmNewMessageMain.aspx'
               var myname = '';
               var w = 560;
               var h = 560;
               var winl = (screen.width - w) / 2;
               var wint = (screen.height - h) / 2;
               winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
               win = window.open(mypage, myname, winprops)
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }


           }
           catch (err) {
           }
       }
   }
   );

       function openWindow(wintitle, winURL, winWidth, winHeight) {
           var win = new Ext.Window(
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
          html: winURL,
          closeAction: 'hide'
      }
      );
           win.show();
           return win;
       }


       var streetView = Ext.create('Ext.Button',
   {
       text: 'Street view',
       id: 'streetViewButton',
       tooltip: 'Google street view',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       disabled: true,
       handler: function () {
           try {
               var currentTicked = vehiclegrid.getSelectionModel().getSelection();
               if (currentTicked.length > 1) {
                   Ext.MessageBox.alert('Streetview', ' Please select only 1 vehicle for street view.');
               }
               else {

                   var selectedBoxs = new Array();
                   Ext.each(currentTicked, function (selectedRec, i) {
                       selectedBoxs.push(selectedRec.data);
                   }
               );
                   var winurl = "./StreetView.aspx?WinId=" + wincounter;
                   var htmlNewWin = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + winurl + '"></iframe>';
                   SetWinTrackData2(selectedBoxs);
                   openWindow('Street view', htmlNewWin, 1000, 480);
                   wincounter++;
               }
           }
           catch (err) {
           }
       }
   }
   );


       var trackit = Ext.create('Ext.Button',
   {
       text: 'TrackIt',
       id: 'trackitButton',
       //    renderTo : Ext.getBody(),
       tooltip: 'Track selected vehicle on separate map',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       disabled: true,
       handler: function () {
           try {
               var currentTicked = vehiclegrid.getSelectionModel().getSelection();
               var selectedBoxs = new Array();
               Ext.each(currentTicked, function (selectedRec, i) {
                   var newIcon = "";
                   var today = new Date();
                   var posExpireDate = new Date();
                   posExpireDate.setTime(today.getTime() - (60 * PositionExpiredTime * 1000));

                   selectedRec.data.icon = getIcon(selectedRec, posExpireDate);
                   selectedBoxs.push(selectedRec.data);
               }
            );

               SetWinTrackData2(selectedBoxs);
               var winurl = "./OpenLayerMap.aspx?WinId=" + wincounter;
               var htmlNewWin = '<iframe scrolling="no" src="' + winurl + '" style="Height:100%; width:100%;  border:0;margin:0px"></iframe>';
               openWindow('Track Vehicles', htmlNewWin, 600, 480);
               wincounter++;
           }
           catch (err) {

           }
       }
   }
   );

       var demomap = Ext.create('Ext.Button',
   {
       text: 'Demo open layer map',
       id: 'demomapButton',
       tooltip: 'Demo open layer map without tracking vehicle',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       disabled: false,
       handler: function () {
           try {
               var winurl = "./OpenLayerMap.aspx";

               var htmlNewWin = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + winurl + '"></iframe>';

               var win = Ext.create('Ext.Window',
            {
                title: 'Track Vehicles',
                width: 1000,
                maxWidth: window.screen.width,
                maxHeight: window.screen.height,
                height: 600,
                layout: 'fit',
                plain: true,
                maximizable: 'true',
                minimizable: 'true',
                resizable: 'true',
                closable: true,
                html: htmlNewWin
            }
            ).show();
           }
           catch (err) {
           }
       }
   }
   );

       var searchMap = Ext.create('Ext.Button',
   {
       text: 'Search',
       id: 'searchMapButton',
       tooltip: 'Search',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               searchwindow(mapframe);
           }
           catch (err) {
           }
       }
   }
   );

       var filters =
   {
       ftype: 'filters',
       local: true,   // defaults to false (remote filtering)
       filters: [
      {
          type: 'string',
          dataIndex: 'BoxId'
      }
      ,
      {
          type: 'string',
          dataIndex: 'Description'
      }
      ,
      {
          type: 'string',
          dataIndex: 'StreetAddress'
      }
      ,
      {
          type: 'string',
          dataIndex: 'VehicleStatus'
      }
      ,
      {
          type: 'int',
          dataIndex: 'CustomSpeed'
      }
      ,
      {
          type: 'date',
          dataIndex: 'OriginDateTime'
      }
      ,
      {
          type: 'boolean',
          dataIndex: 'BoxArmed'
      }
      ]
   }
   ;

       Ext.define('Alarm',
   {
       extend: 'Ext.data.Model',
       fields: [
      'AlarmId',
      {
          name: 'TimeCreated', type: 'date', dateFormat: 'c'
      }
      ,
      'AlarmState',
      'AlarmLevel',
      'vehicleDescription',
      'AlarmDescription'
      ],
       idProperty: 'AlarmId'
   }
   );

       Ext.define('Message',
   {
       extend: 'Ext.data.Model',
       fields: [
      'MessageId',
      {
          name: 'MsgDateTime', type: 'date', dateFormat: 'c'
      },
      'MsgKey',
      'VehicleId',
      'peripheralId',
      'MsgTypeId',
      'checksumId',
      'UserId',
      'Description',
      'MsgBody',
      'Acknowledged'
      ],
       idProperty: 'MessageId'
   }
   );
       // create the Data Store
       var alarmsstore = Ext.create('Ext.data.Store',
   {
       model: 'Alarm',
       pageSize: pagesize,
       autosync: false,
       autoLoad: false,
       storeId: 'AlarmsStore',
       proxy:
      {
          type: 'ajax',
          url: './Map/frmAlarmRotatingServerCall_XML.aspx',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'Alarm',
             record: 'AllUserAlarmsInfo'
         }
      }
      ,
       listeners:
      {
          'load': function (store, records, options) {
              currentState = LoadStates.AlarmsLoaded;

              var alms = store.getRange();
              var newAlarms = 0;
              Ext.each(alms, function (modrecord, i) {
                  if (modrecord.data.AlarmState == "New")
                      newAlarms++;
              });

              var divwidth = 25;
              if (newAlarms >= 10 && newAlarms < 100)
                  divwidth = 30;
              else if (newAlarms >= 100)
                  divwidth = 35;

              if (!alarmgrid.isVisible() && newAlarms > 0)
                  alarmgrid.setTitle("Alarms <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg blinking'>(" + newAlarms + ")</span></div>");
              else
                  alarmgrid.setTitle("Alarms <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg'>(" + newAlarms + ")</span></div>");
          }
         ,
          scope: this
      }
      , sorters: [
      {
          // property : 'OriginDateTime',
          property: 'TimeCreated',
          direction: 'DESC'
      }
      ]
   }
   );

       var messagesstore = Ext.create('Ext.data.Store',
   {
       model: 'Message',
       pageSize: pagesize,
       autosync: false,
       autoLoad: false,
       storeId: 'MessagesStore',
       proxy:
      {
          type: 'ajax',
          url: './Map/frmMessageRotatingServer_XML.aspx',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'Message',
             record: 'MessageInfo'
         }
      }
      ,
       listeners:
      {
          'load': function (store, records, options) {
              currentState = LoadStates.MessagesLoaded;

              var msgs = store.getRange();
              var unreadMsg = 0;
              Ext.each(msgs, function (modrecord, i) {
                  if (modrecord.data.Acknowledged == "N/A" || modrecord.data.Acknowledged.toLowerCase() == "unread")
                      unreadMsg++;
              });

              var divwidth = 25;
              if (unreadMsg >= 10 && unreadMsg < 100)
                  divwidth = 30;
              else if (unreadMsg >= 100)
                  divwidth = 35;

              if (!messagegrid.isVisible() && unreadMsg > 0)
                  messagegrid.setTitle("Messages <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg blinking'>(" + unreadMsg + ")</span></div>");
              else
                  messagegrid.setTitle("Messages <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg'>(" + unreadMsg + ")</span></div>");
          }
         ,
          scope: this
      }
      , sorters: [
      {
          // property : 'OriginDateTime',
          property: 'MsgDateTime',
          direction: 'DESC'
      }
      ]
   }
   );

       /*var geolandmarksdata = { 'items': [
       { 'type': 'Lisa', "name": "lisa@simpsons.com", "f": "555-111-1224" },
       { 'type': 'Bart', "name": "bart@simpsons.com", "f": "555-222-1234" }
       ]
       };*/
       //var geolandmarksdata = { 'items': [] };
       var geolandmarksstore = Ext.create('Ext.data.Store',
   {
       //model: 'Message',
       pageSize: pagesize,
       //pageSize: 100,
       //autosync: false,
       //autoLoad: false,
       storeId: 'geolandmarksstore',
       fields: ['type', 'name', 'f']
       ,
       autoLoad: false,
       proxy: {
           type: 'memory',
           reader: {
               type: 'json',
               root: 'items',
               totalProperty: 'total'
           }
       }
   });

       geozonesstore = Ext.create('Ext.data.Store',
   {
       //model: 'Message',
       pageSize: pagesize,
       //pageSize: 100,
       //autosync: false,
       //autoLoad: false,
       storeId: 'geozonesstore',
       fields: ['type', 'name', 'f', 'desc', 'direction', 'SeverityName', 'id']
       ,
       autoLoad: false,
       proxy: {
           type: 'memory',
           reader: {
               type: 'json',
               root: 'items',
               totalProperty: 'total'
           }
       }
   });

       landmarksstore = Ext.create('Ext.data.Store',
   {
       //model: 'Message',
       pageSize: pagesize,
       //pageSize: 100,
       //autosync: false,
       //autoLoad: false,
       storeId: 'landmarksstore',
       fields: ['type', 'name', 'f', 'desc', 'StreetAddress', 'Email', 'ContactPhoneNum', 'radius']
       ,
       autoLoad: false,
       proxy: {
           type: 'memory',
           reader: {
               type: 'json',
               root: 'items',
               totalProperty: 'total'
           }
       }
   });


       function mapassets(records) {
           var mapJsonData = new Array();
           if (mapAssets) {
               Ext.each(records, function (exirecord) {
                   var newIcon = "";
                   var today = new Date();
                   var posExpireDate = new Date();
                   posExpireDate.setTime(today.getTime() - (60 * PositionExpiredTime * 1000));

                   exirecord.data.icon = getIcon(exirecord, posExpireDate);
                   mapJsonData.push(exirecord.data);

               }
               );
           }

           currentState = LoadStates.MappingVehicles;

           if (mapJsonData.length > 0 && mapAssets) {
               mapVehicles(true, mapJsonData, true, false);
               firstLoad = true;
               vehiclegrid.getSelectionModel().selectAll(false);
           }
           else {
               firstLoad = false;
               $('.x-grid-hd-checker-on').removeClass('x-grid-hd-checker-on');
           }
       }

       // create the Data Store
       var sortingParam = {};
       mainstore = Ext.create('Ext.data.Store',
   {
       model: 'VehicleList',
       pageSize: VehicleListPagesize,
       // buffered : true,
       // purgePageCount : 0,
       autoLoad: false,
       autosync: false,
       storeId: 'Vehicles',
       listeners:
      {
          'load': function (store, records, options) {

              try {
                  if (store.getCount() > 0) {
                      currentState = LoadStates.GridLoading;

                      if (mapAssets || FromClosestVehicles) {
                          //setTimeout(function () { mapassets(records); }, 5000);

                          //firstLoad = false;
                          FromClosestVehicles = false;
                          setTimeout(function () { vehiclegrid.getSelectionModel().selectAll(true) }, 2000);

                      }
                      else {
                          firstLoad = false;
                          $('.x-grid-hd-checker-on').removeClass('x-grid-hd-checker-on');
                      }
                      if (!taskRunning) {
                          taskRunning = true;
                          if (ShowAlarmTab) alarmsstore.load();
                          messagesstore.load();
                          vehiclerunner.start(vehicletask);
                          if (ShowAlarmTab) alarmrunner.start(alarmtask);
                          //messagerunner.start(messagetask);
                          loadingMask.hide();
                      }
                  }
                  else {
                      FromClosestVehicles = false;
                      removeMarkersOnMap();
                      currentState = LoadStates.GridLoading;

                      if (!taskRunning) {
                          taskRunning = true;
                          if (ShowAlarmTab) alarmsstore.load();
                          messagesstore.load();
                          vehiclerunner.start(vehicletask);
                          if (ShowAlarmTab) alarmrunner.start(alarmtask);
                          messagerunner.start(messagetask);
                          loadingMask.hide();
                      }
                  }
                  lastProxyFinished = true;

                  //iniVehicleGridPopup();

              }
              catch (err) {
              }
              currentState = LoadStates.MainStoreLoaded;
          }
         ,
          scope: this
      }
      ,
       proxy:
      {
          type: 'ajax',
          url: 'Vehicles_Reefer.aspx',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'Fleet',
             record: 'VehiclesLastKnownPositionInformation',
             totalProperty: 'totalCount'
         }
      }
      , sorters: [
      {
          property: 'OriginDateTime',
          direction: 'DESC'
      }
      ],
       sort: function (sorters) {
           sorters = sorters || { property: "OriginDateTime", direction: "DESC" };
           if (sorters.direction == undefined || sorters.property == undefined) {
               sorters = { property: "OriginDateTime", direction: "DESC" };
           }
           var mod = sorters.direction.toUpperCase() == "DESC" ? -1 : 1;
           this.sorters.clear();
           this.sorters.add(sorters);
           this.doSort(function (a, b) {
               if (a.get('isSelected') == 1 && b.get('isSelected') != 1)
                   return -1;
               else if (a.get('isSelected') != 1 && b.get('isSelected') == 1)
                   return 1;

               var a_prop = a.get(sorters.property),
                b_prop = b.get(sorters.property);

               return (a_prop < b_prop ? -1 : 1) * mod;

           });
       }
   }
   );

       historystore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: HistoryPagesize,
       storeId: 'historystore',
       model: 'HistoryListModel',
       //fields: ['BoxId', 'VehicleId', 'LicensePlate', 'Description', 'DateTimeReceived', 'DclId', 'BoxMsgInTypeId', 'BoxMsgInTypeName','BoxProtocolTypeId','BoxProtocolTypeName','CommInfo1','CommInfo2','ValidGps','Latitude','Longitude'
       //         ,'Heading','SensorMask','CustomProp','BlobDataSize','SequenceNum','StreetAddress','OriginDateTime','Speed','BoxArmed','MsgDirection','Acknowledged','Scheduled','MsgDetails','MyDateTime','MyHeading','dgKey','CustomUrl','chkBoxShow'],
       //fields: ['BoxId', 'Description'],
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  AllHistoryRecords = records;
                  setTimeout(function () { mapHistories(records, true, false, false); }, 500);
                  historygrid.getSelectionModel().selectAll(false);
                  $('#historiescount').html(records.length);
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy: {
           type: 'memory',
           reader: {
               type: 'xml',
               root: 'MsgInHistory',
               record: 'VehicleStatusHistory',
               totalProperty: 'totalCount'
           }
       }
   });

       historyPageStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: HistoryPagesize,
       storeId: 'historyPageStore',
       model: 'HistoryListModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  /*mapHistories(records, true, false, false);
                  historygrid.getSelectionModel().selectAll(false);
                  $('#historiescount').html(records.length);*/
                  //historygrid.getDockedItems('pagingtoolbar')[0].doRefresh();
                  //historyPager.doRefresh();
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy: {
           type: 'memory',
           reader: {
               type: 'xml',
               root: 'MsgInHistory',
               record: 'VehicleStatusHistory',
               totalProperty: 'totalCount'
           }
       }
   });

       historyStopStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'historyStopStore',
       model: 'HistoryStopModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  mapHistories(records, true, false, true);
                  historyStopGrid.getSelectionModel().selectAll(false);

                  $('#stophistoriescount').html(records.length);
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy: {
           type: 'memory',
           reader: {
               type: 'xml',
               root: 'DsStopReport',
               record: 'StopData'
               //,totalProperty: 'totalCount'
           }
       }
   });

       historyTripStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'historyTripStore',
       model: 'HistoryTripModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  //mapHistories(records, true, false, true);
                  //historyTripGrid.getSelectionModel().selectAll(false);

                  //$('#stophistoriescount').html(records.length);
                  historyTripsNum = records.length;
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy: {
           type: 'memory',
           reader: {
               type: 'xml',
               root: 'dstTripSummaryPerVehicle',
               record: 'Table'
               //,totalProperty: 'totalCount'
           }
       }
   });

       historyAddressStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'historyaddressstore',
       model: 'HistoryAddressModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  searchingMask.hide();
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy:
      {
          type: 'ajax',
          url: 'Vehicles_Reefer.aspx?QueryType=searchHistoryAddress',
          timeout: 600000,
          reader:
         {
             type: 'xml',
             root: 'NewDataSet',
             record: 'Table'//,
             //totalProperty: 'totalCount'
         }
      }
   });
       var currentpage = null;
       var vehiclePager = new Ext.PagingToolbar(
   {
       id: 'vehiclePager',
       store: mainstore,
       displayInfo: true,
       displayMsg: 'Displaying vehicles {0} - {1} of {2}',
       emptyMsg: "No vehicles to display",
       listeners: {
           beforechange: function () {
               selModel.suspendEvents(true);
               var selFleet = (LoadVehiclesBasedOn == 'fleet') ? SelectedFleetId : DefaultOrganizationHierarchyFleetId;
               //mainstore.proxy.extraParams = { QueryType: 'GetfleetPosition', fleetID: selFleet };
               mainstore.proxy.extraParams = { QueryType: 'GetFilteredFleet', filters: currentFilters };

               loadingMask.show();
           },

           change: function (thisd, params) {
               loadingMask.hide();
               if (typeof (params) != 'undefined') {
                   currentpage = params.currentPage;
               }
               
               selModel.resumeEvents();
           }
       }

   }
   );

       var alarmsPager = new Ext.PagingToolbar(
   {
       store: alarmsstore,
       displayInfo: true,
       displayMsg: 'Displaying alarms {0} - {1} of {2}',
       emptyMsg: "No alarms to display"// ,

   }
   );

       var messagesPager = new Ext.PagingToolbar(
   {
       store: messagesstore,
       displayInfo: true,
       displayMsg: 'Displaying messages {0} - {1} of {2}',
       emptyMsg: "No messages to display"// ,

   }
   );
       var historyPager = new Ext.PagingToolbar(
   {
       store: historyPageStore,
       displayInfo: true,
       displayMsg: 'Displaying histories {0} - {1} of {2}',
       emptyMsg: "No histories to display",
       listeners: {
           beforechange: function (b, page, o) {
               /*var selFleet = (LoadVehiclesBasedOn == 'fleet') ? SelectedFleetId : DefaultOrganizationHierarchyFleetId;
               mainstore.proxy.extraParams = { QueryType: 'GetfleetPosition', fleetID: selFleet };

               loadingMask.show();*/

               try {
                   if (historyPagerDoc != '') {
                       historyPagerDoc = '';
                       return;
                   }
                   if (historyVehicles.getValue() == null || historyVehicles.getValue() == '-1') {
                       Ext.Msg.alert('Oops', 'Please select a vehicle...');
                       return;
                   }

                   historygrid.getView().emptyText = 'loading...';

                   //var form = btnSubmit.up('form').getForm();
                   var form = historyForm.getForm();
                   if (form.isValid()) {
                       form.submit({
                           url: './historynew/historyservices_Reefer.aspx?fromsession=1&st=gethistoryrecords&start=' + (page - 1) * HistoryPagesize + '&limit=' + HistoryPagesize,
                           success: function (form, action) {
                               historygrid.getView().emptyText = 'No History to display';
                               var d = action.result.data;
                               d = d.replace(/\u003c/g, "<").replace("\u003e", ">");

                               if (action.result.iconTypeName != "") IconTypeName = action.result.iconTypeName;
                               var doc;
                               if (window.ActiveXObject) {         //IE
                                   var doc = new ActiveXObject("Microsoft.XMLDOM");
                                   doc.async = "false";
                                   doc.loadXML(d);
                               } else {                             //Mozilla
                                   var doc = new DOMParser().parseFromString(d, "text/xml");
                               }
                               historyPageStore.loadRawData(doc);
                               historyForm.hide();
                           },
                           failure: function (form, action) {
                               historygrid.getView().emptyText = 'No History to display';

                               if (action.result && action.result.msg && action.result.msg != '')
                                   Ext.Msg.alert('Failed', action.result.msg);
                               else
                                   Ext.Msg.alert('Failed', 'some error');
                               
                           }
                       });
                   }
               }
               catch (err) {
               }
           },

           change: function () {
               //loadingMask.hide();
           }
       }

   }
   );

       var recordUpdater = Ext.create('Ext.data.Store',
   {
       model: 'VehicleList',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  if (store.getCount() > 0) {
                      currentState = LoadStates.GettingUpdates;
                      var modified = store.getRange();
                      // var recToDelete = new Array();
                      var newRecToMap = new Array();
                      var counter = 0;
                      Ext.each(modified, function (modrecord, i) {
                          var newIcon = "";
                          var today = new Date();
                          var posExpireDate = new Date();
                          posExpireDate.setTime(today.getTime() - (60 * PositionExpiredTime * 1000));

                          modrecord.data.icon = getIcon(modrecord, posExpireDate);

                          var tests = mainstore.findExact('BoxId', modrecord.data.BoxId, 0);
                          if (tests != -1) {
                              modrecord.data.isSelected = mainstore.getAt(tests).data.isSelected;
                              mainstore.getAt(tests).data = modrecord.data;
                              newRecToMap.push(modrecord.data);
                          }
                      }
                  );

                      currentState = LoadStates.MainStoreLoading;
                      mainstore.sort('OriginDateTime', 'DESC');
                      currentState = LoadStates.MappingVehicles;
                      mapVehicles(true, newRecToMap, false, '');

                      //var row = $(Ext.get(vehiclegrid.getView().getNode(gridindex)).dom);
                      //row.children("td").attr("style", "border-right: 0 !important; border-left: 0 !important");

                      if (selectedVehicleBoxId > 0) {
                          var gridindex = 0;
                          vehiclegrid.getStore().each(function (record) {
                              if (record.data.BoxId == selectedVehicleBoxId) {
                                  var row = $(Ext.get(vehiclegrid.getView().getNode(gridindex)).dom);

                                  row.children("td").addClass("highlightgrid");
                                  row.children("td").attr("style", "background-color: #ACFA97 !important");
                                  $(".x-date-time").attr("style", "background-color: white");

                                  return false;
                              }
                              gridindex++;

                          });
                      }

                      iniVehicleGridPopup();
                  }
              }
              catch (err) {
                  // console.log("Error in recordUpdater " + err);                  
              }
              currentState = LoadStates.MainStoreLoaded;
          }
      }
      ,
       proxy:
      {
          // load using HTTP
          type: 'ajax',
          url: 'Vehicles_Reefer.aspx',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'Fleet',
             record: 'VehiclesLastKnownPositionInformation'
         }
      }
   }
   );


       var vehicletask =
   {
       run: function () {
           try {
               if (!mainstore.isLoading() && !recordUpdater.isLoading()) {
                   if (IsSyncOn) {
                       currentState = LoadStates.GettingUpdates;
                       recordUpdater.removeAll(true);
                       recordUpdater.load(
                  {
                      params:
                     {
                         QueryType: 'GetVehiclePosition',
                         filters: currentFilters,
                         sorting: sortingParam,
                         start: (currentpage - 1) * pagesize,
                         limit: pagesize
                     }
                  }
                  );
                   }
               }
           }
           catch (err) {
           }
       }
      ,
       interval: parseInt(vehinterval) // 5 second
   }
       var vehiclerunner = new Ext.util.TaskRunner();

       // create the Data Store
       var alarmupdater = Ext.create('Ext.data.Store',
   {
       model: 'Alarm',
       autoLoad: false,
       proxy:
      {
          // load using HTTP
          type: 'ajax',
          url: './Map/frmAlarmRotatingServerCall_XML.aspx',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'Alarm',
             record: 'AllUserAlarmsInfo'
         }
      }
      ,
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  if (store.getCount() > 0) {
                      currentState = LoadStates.GettingAlarmUpdates;
                      var modified = store.getRange();
                      var newRecords = new Array();
                      var newAlarms = 0;
                      Ext.each(modified, function (modrecord, i) {
                          var tests = alarmsstore.findExact('AlarmId', modrecord.data.AlarmId, 0);
                          if (tests == -1) {
                              newRecords.push(modrecord);
                          }
                          if (modrecord.data.AlarmState == "New")
                              newAlarms++;
                      }
                      );
                      if (newRecords.length > 0) {
                          alarmsstore.insert(alarmsstore.getCount() + 1, newRecords);
                          alarmsstore.sort('TimeCreated', 'DESC');

                          var divwidth = 25;
                          if (newAlarms >= 10 && newAlarms < 100)
                              divwidth = 30;
                          else if (newAlarms >= 100)
                              divwidth = 35;

                          if (!alarmgrid.isVisible())
                              alarmgrid.setTitle("Alarms <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg blinking'>(" + newAlarms + ")</span></div>");
                          else
                              alarmgrid.setTitle("Alarms <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg'>(" + newAlarms + ")</span></div>");
                      }
                      alarmsPager.updateInfo();
                  }
              }
              catch (err) {
              }
              currentState = LoadStates.AlarmsLoaded;
          }
      }
   }
   );

       var alarmsDone = true;
       var alarmtask =
   {
       run: function () {
           try {
               if (!alarmsstore.isLoading() && !alarmupdater.isLoading()) {
                   currentState = LoadStates.GettingAlarmUpdates;
                   alarmupdater.removeAll(true);
                   alarmupdater.load();
               }

           }
           catch (err) {
           }
       }
      ,
       interval: parseInt(alarminterval) // 5 second
   }

       var alarmrunner = new Ext.util.TaskRunner();


       //////// messages
       // create the Data Store
       var messageupdater = Ext.create('Ext.data.Store',
   {
       model: 'Message',
       autoLoad: false,
       proxy:
      {
          // load using HTTP
          type: 'ajax',
          url: './Map/frmMessageRotatingServer_XML.aspx',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'Message',
             record: 'MessageInfo'
         }
      }
      ,
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  if (store.getCount() > 0) {
                      currentState = LoadStates.GettingMessageUpdates;
                      var modified = store.getRange();
                      var newRecords = new Array();
                      var unreadMsg = 0;
                      Ext.each(modified, function (modrecord, i) {
                          var tests = messagesstore.findExact('MessageId', modrecord.data.MessageId, 0);
                          if (tests == -1) {
                              newRecords.push(modrecord);
                          }
                          if (modrecord.data.Acknowledged == "N/A")
                              unreadMsg++;
                      }
                      );

                      if (newRecords.length > 0) {
                          messagesstore.insert(messagesstore.getCount() + 1, newRecords);
                          messagesstore.sort('MsgDateTime', 'DESC');

                          var divwidth = 25;
                          if (unreadMsg >= 10 && unreadMsg < 100)
                              divwidth = 30;
                          else if (unreadMsg >= 100)
                              divwidth = 35;

                          if (!messagegrid.isVisible())
                              messagegrid.setTitle("Messages <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg blinking'>(" + unreadMsg + ")</span></div>");
                          else
                              messagegrid.setTitle("Messages <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg'>(" + unreadMsg + ")</span></div>");
                      }
                      messagesPager.updateInfo();
                  }
              }
              catch (err) {
              }
              currentState = LoadStates.MessagesLoaded;
          }
      }
   }
   );

       var messagesDone = true;
       var messagetask =
   {
       run: function () {
           try {
               if (!messagesstore.isLoading() && !messageupdater.isLoading()) {
                   currentState = LoadStates.GettingMessageUpdates;
                   messageupdater.removeAll(true);
                   messageupdater.load();
               }

           }
           catch (err) {
           }
       }
      ,
       interval: parseInt(messageinterval) // 5 second
   }

       var messagerunner = new Ext.util.TaskRunner();

       northmappanel = Ext.create('Ext.Panel',
   {
       //region: defaultMapView,
       region: 'center',
       id: 'nmappanel',
       /*split: true,
       titleCollapse: true,
       autoScroll: true,
       border: false,
       height: window.screen.height / 2,
       width: window.screen.width / 2,
       autoHeight: true,
       collapsible: true,
       animCollapse: true,*/
       minHeight: 0,
       minSize: 0,
       html: mapHTML + ' id="nmapframe" name="nmapframe" ' + mapStyle + '></iframe>',
       listeners:
      {
          'afterrender': function () {
              if (LoadVehiclesBasedOn == 'fleet') {
                  //fleetstore.load();
                  mainstore.load(
                   {
                       params:
                      {
                          QueryType: 'GetfleetPosition',
                          fleetID: DefaultFleetID,
                          start: 0,
                          limit: VehicleListPagesize
                      }
                   }
                   );
              }
              else {
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

          }
      }
   }
   );

       var scrollMenu = Ext.create('Ext.menu.Menu');

       var exportMenu = Ext.create('Ext.menu.Menu');
       var historyExportMenu = Ext.create('Ext.menu.Menu');
       var historyStopExportMenu = Ext.create('Ext.menu.Menu');
       var historyTripExportMenu = Ext.create('Ext.menu.Menu');
       var historyAddressExportMenu = Ext.create('Ext.menu.Menu');

       var finditmenu = {
           id: 'finditmenu',
           text: 'FindIt',
           disabled: true,
           tooltip: 'Map the selected vehicle',
           iconCls: 'map',
           cls: 'cmbfonts',
           handler: finditonmap
       }

       var trackitmenu =
       {
           text: 'TrackIt',
           id: 'trackitmenu',
           tooltip: 'Track selected vehicle on separate map',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'left',
           disabled: true,
           handler: function () {
               try {
                   var selectedBoxs = new Array();
                   if (selectedVehicleBoxId > 0) {
                       var newIcon = "";
                       var today = new Date();
                       var posExpireDate = new Date();
                       posExpireDate.setTime(today.getTime() - (60 * PositionExpiredTime * 1000));

                       selectedVehicleData.icon = getIcon(selectedVehicleData, posExpireDate);

                       selectedBoxs.push(selectedVehicleData);
                   }

                   SetWinTrackData2(selectedBoxs);
                   var winurl = "./OpenLayerMap.aspx?WinId=" + wincounter;
                   var htmlNewWin = '<iframe scrolling="no" src="' + winurl + '" style="Height:100%; width:100%;  border:0;margin:0px"></iframe>';
                   openWindow('Track Vehicles', htmlNewWin, 600, 480);
                   wincounter++;
               }
               catch (err) {

               }
           }
       };

       var streetViewMenu =
       {
           text: 'Street view',
           id: 'streetViewMenu',
           tooltip: 'Google street view',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'left',
           disabled: true,
           handler: function () {
               try {
                   if (selectedVehicleBoxId > 0) {
                       var selectedBoxs = new Array();
                       selectedBoxs.push(selectedVehicleData);

                       var winurl = "./StreetView.aspx?WinId=" + wincounter;
                       var htmlNewWin = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + winurl + '"></iframe>';
                       SetWinTrackData2(selectedBoxs);
                       openWindow('Street view', htmlNewWin, 1000, 480);
                       wincounter++;
                   }
                   else {
                       Ext.MessageBox.alert('Streetview', ' Please select a vehicle for street view.');
                   }

               }
               catch (err) {
               }
           }
       };

       var updatePositionMenu =
       {
           text: 'Update Position',
           id: 'updatePositionMenu',
           tooltip: 'Map selected vehicle on map',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'left',
           disabled: true,
           handler: function () {
               try {
                   var currentTicked = vehiclegrid.getSelectionModel().getSelection();
                   var selectedBoxs = "";
                   Ext.each(currentTicked, function (selectedRec, i) {
                       selectedBoxs = selectedBoxs + "," + selectedRec.data.BoxId + ",";
                   }
                );
                   var mapstore = new Ext.data.Store(
                {
                    proxy: new Ext.ux.AspWebAjaxProxy(
                   {
                       //
                       url: './Vehicles_Reefer.aspx/UpdatePosition',
                       timeout: proxyTimeOut,
                       actionMethods:
                      {
                          create: 'POST',
                          destroy: 'DELETE',
                          read: 'POST',
                          update: 'POST'
                      }
                      ,
                       extraParams:
                      {
                          boxIDs: selectedBoxs
                      }
                      ,
                       reader:
                      {
                          type: 'json',
                          // model : 'VehicleList'
                          model: 'UpdatePositonData'
                      }
                      ,
                       headers:
                      {
                          'Content-Type': 'application/json; charset=utf-8'
                      }
                   }
                   )
                }
                );

                   var operation = new Ext.data.Operation(
                {
                    action: 'read'
                }
                );
                   mapstore.proxy.read(operation, onUpdatePositionReceived, mapstore);
               }
               catch (err) {
               }
           }
       };

       var GetReeferMenu =
 {
     text: 'Reefer Commands',
     id: 'GetReeferMenu',
     //: ResGetBoxStatusMenuTool,
     iconCls: 'map',
     cls: 'cmbfonts',
     textAlign: 'left',
     disabled: true,
     handler: function () {
         try {
             var currentTicked = vehiclegrid.getSelectionModel().getSelection();
             var selectedBoxs = ",";
             Ext.each(currentTicked, function (selectedRec, i) {
                 selectedBoxs = selectedBoxs + selectedRec.data.BoxId + ",";
             });
             var postdata = {"boxid":selectedBoxs};
             $.ajax({
                 type: 'POST',
                 url: 'Vehicles_Reefer.aspx?QueryType=GetSensorCommand',
                 data: postdata,
                 async: false,
                 success: function (msg) { },
                 error: function (msg) {}
             });
             var myname = 'Sensors';
             var mypage = './Map/frmSensorMain.aspx?LicensePlate=Multi';
             var w = 525;
             var h = 720;
             var winl = (screen.width - w) / 2;
             var wint = (screen.height - h) / 2;
             winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
             win = window.open(mypage, myname, winprops)
             if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }             
         }
         catch (err) {
         }
     }
 };

       var clearallMenu =
       {
           text: 'ClearAll',
           id: 'clearAllMenu',
           tooltip: 'Clear all selected vehicles',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'left',
           handler: function () {
               try {
                   vehiclegrid.getSelectionModel().deselectAll(false);
               }
               catch (err) {
               }
           }
       }

       var exportToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               //var component = vehiclegrid;
               //var config = {};
               //var formatter = 'json'

               //var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               ////var ed = eval('(' + data + ')');
               ////alert(ed.Header);
               ////alert(data);
               //var id, frame, form, hidden, callback;

               //frame = Ext.fly('exportframe').dom;
               //frame.src = Ext.SSL_SECURE_URL;

               //form = Ext.fly('exportform').dom;

               //document.getElementById('exportdata').value = encodeURIComponent(data);
               //document.getElementById('filename').value = "vehicles";
               //document.getElementById('formatter').value = "csv";
               ////alert('ok');
               //form.submit();

               loadingMask.show();
               vehiclerunner.stopAll();
               vehiclerunner.tasks.length = 0;
               var sortingp = "";
               for (prop in sortingParam) {
                   sortingp += sortingParam[prop] + ',';
               }
               var filterp = "";
               for (prop in currentFilters) {
                   filterp += currentFilters[prop] + ',';
               }
               var columnsp = "";
               Ext.each(vehiclegrid.columns, function (col, index) {
                   if (index == 0)
                       return;
                   if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned" && col.id != "vgReeferHistorys") {
                       if (col.text == "Vin #")
                           columnsp += 'Vin' + ':' + col.dataIndex + ',';
                       else if(col.text == 'Reefer #')
                           columnsp += 'Reefer' + ':' + col.dataIndex + ',';
                       else
                           columnsp += col.text + ':' + col.dataIndex + ',';
                   }
               });
               columnsp = columnsp.substring(0, columnsp.length - 1);
               var form = Ext.create('Ext.form.Panel', {
                   xtype: 'form',
                   itemId: 'uploadForm',
                   hidden: true,
                   standardSubmit: true,
                   method: 'post',
                   url: 'Vehicles_Reefer.aspx?QueryType=GetFilteredFleet&formattype=csv&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
               });

               form.getForm().submit();
               vehiclerunner.start(vehicletask);
               loadingMask.hide();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               //var component = vehiclegrid;
               //var config = {};
               //var formatter = 'json'

               //var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               ////var ed = eval('(' + data + ')');
               ////alert(ed.Header);
               ////alert(data);
               //var id, frame, form, hidden, callback;

               //frame = Ext.fly('exportframe').dom;
               //frame.src = Ext.SSL_SECURE_URL;

               //form = Ext.fly('exportform').dom;

               //document.getElementById('exportdata').value = encodeURIComponent(data);
               //document.getElementById('filename').value = "vehicles";
               //document.getElementById('formatter').value = "excel2003";
               ////alert('ok');
               //form.submit();

               loadingMask.show();
               vehiclerunner.stopAll();
               vehiclerunner.tasks.length = 0;
               var sortingp = "";
               for (prop in sortingParam) {
                   sortingp += sortingParam[prop] + ',';
               }
               var filterp = "";
               for (prop in currentFilters) {
                   filterp += currentFilters[prop] + ',';
               }
               var columnsp = "";
               Ext.each(vehiclegrid.columns, function (col, index) {
                   if (index == 0)
                       return;
                   if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned" && col.id != "vgReeferHistorys") {
                       if (col.text == "Vin #")
                           columnsp += 'Vin' + ':' + col.dataIndex + ',';
                       else if (col.text == 'Reefer #')
                           columnsp += 'Reefer' + ':' + col.dataIndex + ',';
                       else
                           columnsp += col.text + ':' + col.dataIndex + ',';
                   }
               });
               columnsp = columnsp.substring(0, columnsp.length - 1);
               var form = Ext.create('Ext.form.Panel', {
                   xtype: 'form',
                   itemId: 'uploadForm',
                   hidden: true,
                   standardSubmit: true,
                   method: 'post',
                   url: 'Vehicles_Reefer.aspx?QueryType=GetFilteredFleet&formattype=excel2003&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
               });

               form.getForm().submit();
               vehiclerunner.start(vehicletask);
               loadingMask.hide();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               //var component = vehiclegrid;
               //var config = {};
               //var formatter = 'json'

               //var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               ////var ed = eval('(' + data + ')');
               ////alert(ed.Header);
               ////alert(data);
               //var id, frame, form, hidden, callback;

               //frame = Ext.fly('exportframe').dom;
               //frame.src = Ext.SSL_SECURE_URL;

               //form = Ext.fly('exportform').dom;

               //document.getElementById('exportdata').value = encodeURIComponent(data);
               //document.getElementById('filename').value = "vehicles";
               //document.getElementById('formatter').value = "excel2007";
               ////alert('ok');
               //form.submit();

               loadingMask.show();
               vehiclerunner.stopAll();
               vehiclerunner.tasks.length = 0;
               var sortingp = "";
               for (prop in sortingParam) {
                   sortingp += sortingParam[prop] + ',';
               }
               var filterp = "";
               for (prop in currentFilters) {
                   filterp += currentFilters[prop] + ',';
               }
               var columnsp = "";
               Ext.each(vehiclegrid.columns, function (col, index) {
                   if (index == 0)
                       return;
                   if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned" && col.id != "vgReeferHistorys") {
                       if (col.text == "Vin #")
                           columnsp += 'Vin' + ':' + col.dataIndex + ',';
                       else if (col.text == 'Reefer #')
                           columnsp += 'Reefer' + ':' + col.dataIndex + ',';
                       else
                           columnsp += col.text + ':' + col.dataIndex + ',';
                   }
               });
               columnsp = columnsp.substring(0, columnsp.length - 1);
               var form = Ext.create('Ext.form.Panel', {
                   xtype: 'form',
                   itemId: 'uploadForm',
                   hidden: true,
                   standardSubmit: true,
                   method: 'post',
                   url: 'Vehicles.aspx?QueryType=GetFilteredFleet&formattype=excel2007&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
               });

               form.getForm().submit();
               vehiclerunner.start(vehicletask);
               loadingMask.hide();

           }
           catch (err) {
               alert(err);
           }
       }
   }

       var exportHistoryToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportHistoryToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historygrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "csv";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportHistoryToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historygrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2003";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportHistoryToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historygrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               //var ed = eval('(' + data + ')');
               //alert(ed.Header);
               //alert(data);
               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2007";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

       var exportHistoryStopToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportHistoryStopToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyStopGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "csv";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryStopToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportHistoryStopToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyStopGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2003";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryStopToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportHistoryStopToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyStopGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               //var ed = eval('(' + data + ')');
               //alert(ed.Header);
               //alert(data);
               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2007";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

       var exportHistoryTripToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportHistoryTripToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyTripGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "csv";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryTripToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportHistoryTripToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyTripGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2003";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryTripToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportHistoryTripToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyTripGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               //var ed = eval('(' + data + ')');
               //alert(ed.Header);
               //alert(data);
               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2007";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

       var exportHistoryAddressToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportHisotryAddressToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyAddressGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "csv";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryAddressToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportHistoryAddressToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyAddressGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2003";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


       var exportHistoryAddressToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportHistoryAddressToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = historyAddressGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               //var ed = eval('(' + data + ')');
               //alert(ed.Header);
               //alert(data);
               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2007";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

       exportMenu.add(exportToCsvButton, exportToExcel2003Button, exportToExcel2007Button);
       historyExportMenu.add(exportHistoryToCsvButton, exportHistoryToExcel2003Button, exportHistoryToExcel2007Button);
       historyStopExportMenu.add(exportHistoryStopToCsvButton, exportHistoryStopToExcel2003Button, exportHistoryStopToExcel2007Button);
       historyTripExportMenu.add(exportHistoryTripToCsvButton, exportHistoryTripToExcel2003Button, exportHistoryTripToExcel2007Button);
       historyAddressExportMenu.add(exportHistoryAddressToCsvButton, exportHistoryAddressToExcel2003Button, exportHistoryAddressToExcel2007Button);


       scrollMenu.add(/*finditmenu, */trackitmenu, streetViewMenu, updatePositionMenu, GetReeferMenu, clearallMenu);

       var vehicleGridColumns = [];
       vehicleGridColumns.push({
           id: 'vgPretrip',
           text: 'PT',
           align: 'left',
           width: 30,
           filterable: false,
           dataIndex: 'PretripResult',
           renderer: function (value, p, record) {
               if (value == '1')
                   return '<img src="images/reefer_ptpass.png" />';
               else if (value == '-1')
                   return '';
               else {
                   var sTitle = 'Last PreTrip Result For Asset: ' + Ext.String.format(record.data['Description']) + '</br></br>' + 'Overall Result: Fail';
                   return Ext.String.format('<a href="javascript:void(0);" rel="bootstrapreeferpretripresultpopover" data-mouseover="false" data-popup="false" class="withajaxpopover" data-content="loading..." data-html="true" data-title="{1}" data-poload="{0}" data-placement="right" data-container="#vehicleInfoPopover" ><img src="images/reefer_ptfail.png" /></a>', value, sTitle);
               }
           },
           sortable: false
       });

       vehicleGridColumns.push({
           id: 'vgLastAlarm',
           text: 'LA',
           align: 'left',
           width: 30,
           dataIndex: 'ReeferLastAlarm',
           renderer: function (value, p, record) {
               if (value == 'true')
                   return '<img src="images/reefer_alarm.png" />';
               else
                   return '';
           },
           filterable: false,
           sortable: false
       });

       vehicleGridColumns.push({
           id: 'vgReeferSN',
           //stateId: 'stDescription',
           text: 'Reefer #',
           align: 'left',
           width: 90,
           renderer: function (value, p, record) {
               var vehicleId = record.get('VehicleId');
               var returnstring = Ext.String.format('<a href="javascript:void(0);" rel="" data-mouseover="false" data-popup="false" class="withajaxpopover" data-content="loading..." data-html="true" data-poload="./MapNew/frmGetVehicleInfo.aspx?LicensePlate={0}" data-title="{1}" data-placement="right" data-container="body" OnClick="SensorInfoWindow(\'{0}\')">{1}</a>', Ext.String.escape(record.data['LicensePlate']), value);

               return returnstring;
           },
           dataIndex: 'Description',
           filterable: true,
           sortable: true
       });

       vehicleGridColumns.push({
           id: 'vgReeferHistorys',
           //stateId: 'stDescription',
           text: '&nbsp;',
           align: 'left',
           width: 100,
           renderer: function (value, p, record) {
               var returnstring = '<a href="javascript:void(0);" title="Command History" alt="Command History" OnClick="showCommandHistoryTab(\'' + value + '\');">CH</a>';
               returnstring += ' &nbsp; <a href="javascript:void(0);" title="Reefer History" alt="Reefer History" OnClick="showReeferHistoryTab(\'' + value + '\');">RH</a>';
               if (HistoryEnabled)
                   returnstring += ' &nbsp; <a href="javascript:void(0);" OnClick="showHistoryTab(\'' + value + '\');">History</a>';

               return returnstring;
           },
           dataIndex: 'VehicleId',
           filterable: false,
           sortable: false
       });

       vehicleGridColumns.push({
           id: 'vgBoxId',
           //stateId: 'stDescription',
           text: 'Box Id',
           align: 'left',
           width: 100,
           hidden: true,
           dataIndex: 'BoxId',
           filterable: true,
           sortable: true
       });

       vehicleGridColumns.push({
           id: 'vgDateTime',
           //stateId: 'stDateTime',
           text: 'Last Contact',
           align: 'left',
           width: 120,
           xtype: 'datecolumn',
           format: dateformat, //dateformat,
           dataIndex: 'OriginDateTime',
           filterable: true,
           sortable: true,
           tdCls: 'x-date-time'
       });

       vehicleGridColumns.push({
           id: 'vgMicro',
           //stateId: 'stDateTime',
           text: 'Micro',
           align: 'left',
           width: 60,
           filterable: false,
           dataIndex: 'Micro',
           sortable: false
       });

       vehicleGridColumns.push({
           id: 'vgControllerType',
           text: 'Controller',
           align: 'left',
           width: 60,
           dataIndex: 'ControllerType'/*,
           renderer: function (value, p, record) {
               if (record.data['VehicleTypeName'].trim() == 'Dry Car') //Dry Car
                   return '-';
               var ps = getValueByKey('RD_STA', value);

               var returnvalue = '';
               if (ps != '') {
                   if ((ps & 15) == 1) {    // Device Type = 1 (TK)              
                       var controllerTypeId = getValueByKey('RD_CT', value);
                       returnvalue = getControllerTypeById(controllerTypeId);
                   }
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgAddress',
           //stateId: 'stAddress',
           text: 'Location',
           align: 'left',
           width: 280,
           dataIndex: 'StreetAddress',
           filterable: true,
           sortable: false
       });

       vehicleGridColumns.push({
           id: 'vgTether',
           text: 'Tether',
           align: 'left',
           width: 60,
           hidden: true,
           dataIndex: 'TetherOnOff'/*,
           renderer: function (value) {
               return GetTetherOnOff(value);
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgPower',
           text: 'Power',
           align: 'left',
           width: 50,
           dataIndex: 'PowerOnOff',
           renderer: function (value, p, record) {
               //if (GetTetherOnOff(value) == 'Off' && record.data['VehicleTypeName'].trim() != 'Dry Car') //Dry Car
               //    return '-';

               //var ps = getValueByKey('RD_STA', value);

               //var returnvalue = '-';
               //if (ps != '') {
               //    if ((ps & 15) == 1) {    // Device Type = 1 (TK)              
               //        var controllerTypeId = getValueByKey('RD_CT', value);
               //        if (',9,11,12,13,14,15,16,17,19,20,'.indexOf(',' + controllerTypeId + ',') >= 0) {

               //            if (ps & 64 > 0)
               //                returnvalue = "On";
               //            else
               //                returnvalue = "Off";
               //        }
               //    }
               //}

               //var poweronoff = GetReeferOnOff(record);
               //var color = returnvalue == poweronoff ? "#000000" : "#666666";
               //return '<span style="color:' + color + '">' + poweronoff + '</span>';
               ////return returnvalue;
               if (record.data['ReeferPower'].trim() == "-" && record.data['Micro'].trim() != "DC")
                   return "-";
               var color = value == record.data['ReeferPower'].trim() ? "#000000" : "#666666";
               return '<span style="color:' + color + '">' + value + '</span>';
           }
       });

       vehicleGridColumns.push({
           id: 'vgReeferState',
           text: 'ReeferState',
           align: 'left',
           width: 120,
           dataIndex: 'ReeferState'
       });

       vehicleGridColumns.push({
           id: 'vgModeOfOp',
           text: 'Mode of Op.',
           align: 'left',
           width: 80,
           dataIndex: 'ModeOfOp'/*,
           renderer: function (value, p, record) {
               if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                   return '-';

               var ps = getValueByKey('RD_ZONE1', value);
               var returnvalue = '';
               if (ps != '') {
                   var v = getValueByKeyWithSeperatorEqualSign('OM', ps, ',', ':');
                   if (v != '') {
                       var mode = (v & 16) >> 4;
                       if (mode == 1)
                           returnvalue = 'Continuous';
                       else
                           returnvalue = 'Cycle Sentry';
                   }
               }
               return returnvalue;
               //return getValueByKey('SENSOR_NUM', value);
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgSetpt',
           text: 'Setpt.',
           align: 'left',
           width: 60,
           dataIndex: 'RF_Setpt'/*,
           renderer: function (value, p, record) {
               if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                   return '-';

               var ps = getValueByKey('RD_ZONE1', value);
               var returnvalue = '';
               if (ps != '') {
                   var v = getValueByKeyWithSeperatorEqualSign('TSP', ps, ',', ':');
                   if (v != '')
                       returnvalue = (v * 1).toFixed(0);
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgRet',
           text: 'Ret.',
           align: 'left',
           width: 60,
           dataIndex: 'RF_Ret'/*,
           renderer: function (value, p, record) {
               if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                   return '-';

               var ps = getValueByKey('RD_ZONE1', value);
               var returnvalue = '';
               if (ps != '') {
                   var v = getValueByKeyWithSeperatorEqualSign('RAT1', ps, ',', ':');
                   if (v != '')
                       returnvalue = (v * 1).toFixed(2);
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgDis',
           text: 'Dis.',
           align: 'left',
           width: 60,
           dataIndex: 'RF_Dis'/*,
           renderer: function (value, p, record) {
               if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                   return '-';

               var ps = getValueByKey('RD_ZONE1', value);
               var returnvalue = '';
               if (ps != '') {
                   var v = getValueByKeyWithSeperatorEqualSign('SDT1', ps, ',', ':');
                   if (v != '')
                       returnvalue = (v * 1).toFixed(2);
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgSensorProbe',  //Sensor Probe, evaporator coil temperature
           text: 'SenPro.',
           align: 'left',
           width: 60,
           dataIndex: 'RF_SensorProbe'/*,
           renderer: function (value, p, record) {
               if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                   return '-';

               var ps = getValueByKey('RD_ZONE1', value);
               var returnvalue = '';
               if (ps != '') {
                   var v = getValueByKeyWithSeperatorEqualSign('ECT', ps, ',', ':'); // ECT is currently copy of SpareTemp (RD_STS1) 2014-05-30
                   if (v != '') {
                       v = v * 1;
                       //returnvalue = (v * 1).toFixed(2);
                       if (v >= 3276.7 || v <= -3276.8) {
                           returnvalue = '-';
                       }
                       else {
                           returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                       }
                   }
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgAmb',  //Ambient Temperature
           text: 'Amb.',
           align: 'left',
           width: 60,
           dataIndex: 'RF_Amb'/*,
           renderer: function (value, p, record) {

               if (record.data['VehicleTypeName'].trim() == 'Dry Car') //Dry Car
               {
                   var analog1 = getValueByKey('Analog1', record.data['ExtraInfo']);
                   var analog2 = getValueByKey('Analog2', record.data['ExtraInfo']);

                   if (analog1 != "" && analog2 != "" && !(analog1 == 0 && analog2 == 0)) {
                       var ADC = (analog2 * 256) + (analog1 * 1);
                       var temp = Math.floor((ADC - 1120) / 5);
                       var returnvalue = temp * 9 / 5 + 32;
                       return (returnvalue * 1).toFixed(0);
                   }
                   else
                       return "-";
               }
               else {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_AMT', value);
                   var returnvalue = '';
                   if (ps != '') {
                       returnvalue = (ps * 1).toFixed(0);
                   }
                   return returnvalue;
               }
           }*/
       });

       if (DisplayZone2_3Temperatures == '1') {
           vehicleGridColumns.push({
               id: 'vgSetpt2',
               text: 'Setpt2.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_Setpt2'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE2', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('TSP', ps, ',', ':');
                       if (v != '')
                           returnvalue = (v * 1).toFixed(0);
                   }
                   return returnvalue;
               }*/
           });

           vehicleGridColumns.push({
               id: 'vgRet2',
               text: 'Ret2.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_Ret2'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE2', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('RAT1', ps, ',', ':');
                       if (v != '')
                           returnvalue = (v * 1).toFixed(2);
                   }
                   return returnvalue;
               }*/
           });

           vehicleGridColumns.push({
               id: 'vgDis2',
               text: 'Dis2.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_Dis2'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE2', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('SDT1', ps, ',', ':');
                       if (v != '')
                           returnvalue = (v * 1).toFixed(2);
                   }
                   return returnvalue;
               }*/
           });

           vehicleGridColumns.push({
               id: 'vgSensorProbe2',  //Sensor Probe, evaporator coil temperature
               text: 'SenPro2.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_SensorProbe2'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE2', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('ECT', ps, ',', ':'); // ECT is currently copy of SpareTemp (RD_STS1) 2014-05-30
                       if (v != '') {
                           v = v * 1;
                           //returnvalue = (v * 1).toFixed(2);
                           if (v >= 3276.7 || v <= -3276.8) {
                               returnvalue = '-';
                           }
                           else {
                               returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                           }
                       }
                   }
                   return returnvalue;
               }*/
           });

           vehicleGridColumns.push({
               id: 'vgSetpt3',
               text: 'Setpt3.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_Setpt3'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE3', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('TSP', ps, ',', ':');
                       if (v != '')
                           returnvalue = (v * 1).toFixed(0);
                   }
                   return returnvalue;
               }*/
           });

           vehicleGridColumns.push({
               id: 'vgRet3',
               text: 'Ret3.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_Ret3'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE3', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('RAT1', ps, ',', ':');
                       if (v != '')
                           returnvalue = (v * 1).toFixed(2);
                   }
                   return returnvalue;
               }*/
           });

           vehicleGridColumns.push({
               id: 'vgDis3',
               text: 'Dis3.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_Dis3'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE3', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('SDT1', ps, ',', ':');
                       if (v != '')
                           returnvalue = (v * 1).toFixed(2);
                   }
                   return returnvalue;
               }*/
           });

           vehicleGridColumns.push({
               id: 'vgSensorProbe3',  //Sensor Probe, evaporator coil temperature
               text: 'SenPro3.',
               align: 'left',
               width: 60,
               dataIndex: 'RF_SensorProbe3'/*,
               renderer: function (value, p, record) {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE3', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('ECT', ps, ',', ':'); // ECT is currently copy of SpareTemp (RD_STS1) 2014-05-30
                       if (v != '') {
                           v = v * 1;
                           //returnvalue = (v * 1).toFixed(2);
                           if (v >= 3276.7 || v <= -3276.8) {
                               returnvalue = '-';
                           }
                           else {
                               returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                           }
                       }
                   }
                   return returnvalue;
               }*/
           });           

       }

       vehicleGridColumns.push({
           id: 'vgFuelLevel',
           text: 'Fuel Level (%)',
           align: 'left',
           width: 80,
           dataIndex: 'FuelLevel',
           hidden: true/*,
           renderer: function (value, p, record) {
               //if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
               //    return '-';

               var ps = getValueByKey('RD_FUEL', value);
               var returnvalue = '';
               if (ps != '') {
                   var v = ps * 1;
                   if (v <= 100)
                       returnvalue = v.toFixed(0);
                   else
                       returnvalue = 'Invalid';
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgFuelLevelGallon',
           text: 'Fuel Level (gal)',
           align: 'left',
           width: 80,
           dataIndex: 'FuelLevelGallon'/*,
           renderer: function (value, p, record) {
               //if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
               //    return '-';

               var ps = getValueByKey('RD_FUEL', value);
               var returnvalue = '';
               if (ps != '') {
                   var v = ps * 1;
                   if (v <= 100) {
                       returnvalue = (v * 450 / 100).toFixed(0); //based on 450 gallon tank size
                   }
                   else
                       returnvalue = 'Invalid';
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgAFAX',
           text: 'AFAX',
           align: 'left',
           width: 80,
           dataIndex: 'AFAX'/*,
           renderer: function (value, p, record) {
               if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                   return '-';

               var ps = getValueByKey('RD_ZONE1', value);
               var returnvalue = '-';
               if (ps != '') {
                   var v = getValueByKeyWithSeperatorEqualSign('OM', ps, ',', ':');

                   var controllerTypeId = getValueByKey('RD_CT', value);
                   if (',14,15,16,17,19,20,21,22,'.indexOf(',' + controllerTypeId + ',') >= 0) {
                       if (v != '') {
                           var door = v & 1;

                           if (door == 1)
                               returnvalue = 'Open';
                           else
                               returnvalue = 'Closed';
                       }
                   }

               }
               return returnvalue;
               //return getValueByKey('SENSOR_NUM', value);
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgDoor',
           text: 'S-Door',
           align: 'left',
           width: 80,
           dataIndex: 'SDoor'/*,
           renderer: function (value, p, record) {
               if (record.data['VehicleTypeName'].trim() == 'Dry Car') //Dry Car
               {
                   var ps = getValueByKey('RD_STA', value);

                   var poweronoff = GetReeferOnOff(record);
                   return poweronoff == "On" ? "Open" : "Closed";
               }
               else {
                   if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                       return '-';

                   var ps = getValueByKey('RD_ZONE1', value);
                   var returnvalue = '';
                   if (ps != '') {
                       var v = getValueByKeyWithSeperatorEqualSign('OM', ps, ',', ':');
                       if (v != '') {
                           var door = (v & 4) >> 2;

                           if (door == 1)
                               returnvalue = 'Open';
                           else
                               returnvalue = 'Closed';
                       }
                   }
                   return returnvalue;
                   //return getValueByKey('SENSOR_NUM', value);
               }
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgRPM',
           text: 'RPM',
           align: 'left',
           width: 50,
           dataIndex: 'RPM'/*,
           renderer: function (value, p, record) {
               if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
                   return '-';

               var ps = getValueByKey('RD_STA', value);

               var returnvalue = '';
               if (ps != '') {
                   if ((ps & 15) == 1) {    // Device Type = 1 (TK)              
                       var controllerTypeId = getValueByKey('RD_CT', value);
                       if (',9,11,12,13,14,15,16,17,19,20,'.indexOf(',' + controllerTypeId + ',') >= 0)
                           returnvalue = getValueByKey('RD_RPM', value);
                   }
               }
               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgBatteryVoltage',
           text: 'BatteryV',
           align: 'left',
           width: 60,
           dataIndex: 'BatteryVoltage'/*,
           renderer: function (value, p, record) {
               //if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
               //    return 'N/A';

               var ps = getValueByKey('RD_BATTERY', value);
               var returnvalue = '';

               if (ps != '') {
                   returnvalue = (ps * 1).toFixed(2);
               }

               return returnvalue;
           }*/
       });

       vehicleGridColumns.push({
           id: 'vgEngineHours',
           text: 'Eng. Hrs',
           align: 'left',
           width: 60,
           dataIndex: 'EngineHours'/*,
           renderer: function (value, p, record) {
               //if (GetTetherOnOff(value) == 'Off' || GetReeferOnOff(record) == 'Off')
               //    return '-';

               var ps = getValueByKey('RD_EH', value);
               var returnvalue = '';
               if (ps != '') {
                   returnvalue = (ps * 1).toFixed(0);
               }
               return returnvalue;
           }*/
       });

       var currentFilters = new Object();
       var filteron = false;

       vehiclegrid = Ext.create('Ext.grid.Panel',
   {
       id: 'vehiclesgrid',
       enableColumnHide: true,
       title: 'Status',
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width - 5,
       maxHeight: window.screen.height,
       enableSorting: true,
       closable: false,
       columnLines: true,
       width: window.screen.width,
       autoHeight: true,
       store: mainstore,
       collapsible: true,
       animCollapse: false,
       split: true,
       features: [filters],
       stateId: 'stateVGrid',
       stateful: false, // state should be preserved

       columns: vehicleGridColumns
        , dockedItems: [
      {
          xtype: 'toolbar',
          dock: 'top',
          items: [LoadVehiclesBasedOn == 'fleet' ? fleetButton : organizationHierarchy,

				 '-',
				 {
				     icon: 'preview.png',
				     cls: 'x-btn-text-icon',
				     text: 'Actions',
				     menu: scrollMenu
				 },
				 searchMap, // exportButton,
				 {
				 itemId: 'AutoSync',
				 boxLabel: 'AutoSync',
				 boxLabelCls: 'cmbfonts',
				 xtype: 'checkboxfield',
				 checked: IsSyncOn,
				 tooltip: 'Refresh the map and grid automatically',
				 handler: function () {
				     IsSyncOn = !IsSyncOn;
				 }
				 // IsSyncOn
	}, /*feedback, */, showDriverFinderButton ? findvehiclesdrivers : null, legend, /*VehicleClustering ? */labelonoff/* : null*/
          /*, {
          xtype: 'exporterbutton', //exportbutton
          text: 'Export Grid Data',
          formatter: "csv",
          swfPath: './sencha/Ext.ux.Exporter/downloadify.swf',
          downloadImage: './sencha/Ext.ux.Exporter/download.png',
          downloadName: 'vehicles',
          separator: ","
          //  store: store
          }*/
         , { icon: 'preview.png',
             cls: 'x-btn-text-icon',
             text: 'Export',
             menu: exportMenu
         }
         , clearSearchBtn
          //         , {
          //             xtype: 'exportermenu', //exportbutton
          //             text: 'Export Grid Data',
          //             formatter: "csv",
          //             swfPath: './sencha/Ext.ux.Exporter/downloadify.swf',
          //             downloadImage: './sencha/Ext.ux.Exporter/download.png',
          //             downloadName: 'vehicles',
          //             separator: ","
          //             //  store: store
          //         }
         ]
      }
      ]
      , selModel: selModel
      , listeners: {
          'cellclick': function (grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {

              if (cellIndex != 0) {
                  $(".highlightgrid").attr("style", "background-color: white");
                  $(".highlightgrid").removeClass("highlightgrid");
                  $(tr).children("td").addClass("highlightgrid");
                  $(tr).children("td").attr("style", "background-color: #ACFA97 !important");
                  $(".x-date-time").attr("style", "background-color: white");

                  selectedVehicleBoxId = record.data.BoxId;
                  selectedVehicleData = record.data;

                  //vehiclegrid.down('#finditmenu').setDisabled(false);
                  vehiclegrid.down('#trackitmenu').setDisabled(false);
                  vehiclegrid.down('#streetViewMenu').setDisabled(false);
              }
          },
          'celldblclick': function (grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {

              if (cellIndex != 0) {
                  $(".highlightgrid").attr("style", "background-color: white");
                  $(".highlightgrid").removeClass("highlightgrid");
                  $(tr).children("td").addClass("highlightgrid");
                  $(tr).children("td").attr("style", "background-color: #ACFA97 !important");
                  $(".x-date-time").attr("style", "background-color: white");

                  selectedVehicleBoxId = record.data.BoxId;
                  selectedVehicleData = record.data;

                  //vehiclegrid.down('#finditmenu').setDisabled(false);
                  vehiclegrid.down('#trackitmenu').setDisabled(false);
                  vehiclegrid.down('#streetViewMenu').setDisabled(false);

                  //$('#mapitButton-btnEl').click();
                  findit();
              }
          },
          filterupdate: function () {              
              vehiclegrid.filters.deferredUpdate.cancel();
              var filtersdata;
              var stringvalue;
              if (typeof (vehiclegrid) != 'undefined' && vehiclegrid.filters.filters.length > 0) {
                  filtersdata = vehiclegrid.filters.getMenuFilter();
                  if (filtersdata != null) {
                      filtercolvalue = filtersdata.dataIndex;
                      if (filtersdata.active == true) {
                          if (typeof (filtersdata.getValue()) != 'undefined') {
                              if (filtersdata.type == "int") {
                                  stringvalue = "type int";
                                  var fvalue = filtersdata.getValue();
                                  if (typeof (fvalue["eq"]) != 'undefined')
                                      stringvalue = stringvalue + " eq " + fvalue["eq"].toString();
                                  else {
                                      if (typeof (fvalue["lt"]) != 'undefined')
                                          stringvalue = stringvalue + " lt " + fvalue["lt"].toString();
                                      if (typeof (fvalue["gt"]) != 'undefined')
                                          stringvalue = stringvalue + " gt " + fvalue["gt"].toString();
                                  }
                              }
                              else if (filtersdata.type == "float") {
                                  stringvalue = "type float";
                                  var fvalue = filtersdata.getValue();
                                  if (typeof (fvalue["eq"]) != 'undefined')
                                      stringvalue = stringvalue + " eq " + fvalue["eq"].toString();
                                  else {
                                      if (typeof (fvalue["lt"]) != 'undefined')
                                          stringvalue = stringvalue + " lt " + fvalue["lt"].toString();
                                      if (typeof (fvalue["gt"]) != 'undefined')
                                          stringvalue = stringvalue + " gt " + fvalue["gt"].toString();
                                  }
                              }
                              else if (filtersdata.type == "date") {
                                  stringvalue = "type date";
                                  var fvalue = filtersdata.getValue();

                                  if (typeof (fvalue["on"]) != 'undefined') {
                                      var dt = new Date(fvalue["on"]);
                                      var newDt = Ext.Date.format(dt, userdateformat);
                                      stringvalue = stringvalue + " on " + newDt.toString();
                                  }
                                  else {
                                      if (typeof (fvalue["before"]) != 'undefined') {
                                          var dt = new Date(fvalue["before"]);
                                          var newDt = Ext.Date.format(dt, userdateformat);
                                          stringvalue = stringvalue + " before " + newDt.toString();
                                      }
                                      if (typeof (fvalue["after"]) != 'undefined') {
                                          var dt = new Date(fvalue["after"]);
                                          var newDt = Ext.Date.format(dt, userdateformat);
                                          stringvalue = stringvalue + " after " + newDt.toString();
                                      }
                                  }
                              }
                              else {
                                  stringvalue = filtersdata.getValue().toString();
                              }
                              if (typeof (stringvalue) != 'undefined' && stringvalue.length != 0 && (stringvalue != "" || stringvalue != null)) {
                                  filteron = true;
                                  currentFilters[filtercolvalue] = filtercolvalue + ":" + stringvalue;
                                  var selFleet = (LoadVehiclesBasedOn == 'fleet') ? SelectedFleetId : DefaultOrganizationHierarchyFleetId;
                                  mainstore.currentPage = 1;
                                  loadingMask.show();
                                  mainstore.load({
                                      params:
                                      {
                                          QueryType: 'GetFilteredFleet',
                                          sorting: sortingParam,
                                          filters: currentFilters
                                      },
                                      callback: function (records, operation, success) {
                                          loadingMask.hide();                                          
                                      },
                                      scope: this
                                  });
                              }
                              else {
                                  delete currentFilters[filtercolvalue];
                                  if (Object.keys(currentFilters).length == 0) {
                                      filteron = false;
                                      mainstore.currentPage = currentpage;
                                      loadingMask.show();
                                      mainstore.load({
                                          params:
                                          {
                                              QueryType: 'GetFilteredFleet',
                                              sorting: sortingParam
                                          },
                                          callback: function (records, operation, success) {
                                              loadingMask.hide();                                              
                                          },
                                          scope: this
                                      });
                                  }
                                  else {
                                      loadingMask.show();
                                      mainstore.load({
                                          params:
                                          {
                                              QueryType: 'GetFilteredFleet',
                                              sorting: sortingParam,
                                              filters: currentFilters
                                          },
                                          callback: function (records, operation, success) {
                                              loadingMask.hide();                                              
                                          },
                                          scope: this
                                      });
                                  }
                              }
                          }
                          else {
                              delete currentFilters[filtercolvalue];
                              if (Object.keys(currentFilters).length == 0) {
                                  filteron = false;
                                  mainstore.currentPage = currentpage;
                                  loadingMask.show();
                                  mainstore.load({
                                      params:
                                      {
                                          QueryType: 'GetFilteredFleet',
                                          sorting: sortingParam
                                      },
                                      callback: function (records, operation, success) {
                                          loadingMask.hide();                                          
                                      },
                                      scope: this
                                  });
                              }
                              else {
                                  loadingMask.show();
                                  mainstore.load({
                                      params:
                                      {
                                          QueryType: 'GetFilteredFleet',
                                          sorting: sortingParam,
                                          filters: currentFilters
                                      },
                                      callback: function (records, operation, success) {
                                          loadingMask.hide();                                          
                                      },
                                      scope: this
                                  });
                              }
                          }
                      }
                      else {
                          delete currentFilters[filtercolvalue];
                          if (Object.keys(currentFilters).length == 0) {
                              filteron = false;
                              mainstore.currentPage = currentpage;
                              loadingMask.show();
                              mainstore.load({
                                  params:
                                  {
                                      QueryType: 'GetFilteredFleet',
                                      sorting: sortingParam
                                  },
                                  callback: function (records, operation, success) {
                                      loadingMask.hide();                                      
                                  },
                                  scope: this
                              });
                          }
                          else {
                              loadingMask.show();
                              mainstore.load({
                                  params:
                                  {
                                      QueryType: 'GetFilteredFleet',
                                      sorting: sortingParam,
                                      filters: currentFilters
                                  },
                                  callback: function (records, operation, success) {
                                      loadingMask.hide();                                      
                                  },
                                  scope: this
                              });
                          }
                      }

                  }
                  else {
                      currentFilters = {};
                      filteron = false;
                      mainstore.currentPage = currentpage;
                      loadingMask.show();
                      mainstore.load({
                          params:
                                  {
                                      QueryType: 'GetFilteredFleet',
                                      sorting: sortingParam

                                  },
                          callback: function (records, operation, success) {
                              loadingMask.hide();                              
                          },
                          scope: this
                      });
                  }
              }
          }
      }
      ,
   viewConfig: {
       stripeRows: false,
       emptyText: 'No vehicles to display',
       useMsg: false,
       getRowClass: function (record, index) {
           var d = ((new Date()).getTime() - record.get('OriginDateTime').getTime()) / 1000 / 60 / 60;    // hours

           if (d < 24)
               return 'withinlastday';
           else if (d < 48)
               return 'withinlast2days';
           else if (d < 72)
               return 'withinlast3days';
           else if (d < 168)
               return 'withinlast7days';
           else
               return 'morethan7days';
       }
   },
   // paging bar on the bottom
   bbar: vehiclePager
}
   );

       var alarmgrid = Ext.create('Ext.grid.Panel',
   {
       id: 'alarmgrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width,
       maxHeight: window.screen.height,
       stateful: true,

       closable: false,
       enableColumnHide: false,
       enableSorting: false,
       closable: false,
       width: window.screen.width,
       autoHeight: true,
       title: 'Alarms',
       store: alarmsstore,
       columnLines: true,
       stateId: 'stateAGrid',
       viewConfig:
      {
          emptyText: 'No alarms to display',
          useMsg: false,
          getRowClass: function (rec, rowIdx, params, store) {
              if (rec.get('AlarmDescription').indexOf("CIA") != -1) {
                  return 'grid-row-red';
              }
              if (rec.get('AlarmDescription').indexOf("VIA") != -1) {
                  return 'grid-row-yellow';
              }
          }
      }
      ,
       columns: [
      {
          text: 'Number',
          align: 'left',
          width: 80,
          renderer: function (value) {
              return Ext.String.format('<a href="javascript:void(0);" OnClick="NewAlarmWindow({0})">{1}</a>', value, value);
          }
         ,
          dataIndex: 'AlarmId',
          sortable: false
      }
      ,
      {
          text: 'Alarm Time',
          align: 'left',
          width: 120,
          xtype: 'datecolumn',
          format: userdateformat,
          dataIndex: 'TimeCreated',
          sortable: false
      }
      ,
      {
          text: 'Alarm Priority',
          align: 'left',
          width: 80,
          dataIndex: 'AlarmLevel',
          sortable: false
      }
      ,
      {
          text: 'Alarm Description',
          align: 'left',
          width: 450,
          renderer: function (value) {
              if (value.indexOf("CIA") != -1 && soundPresent != true) {
                  soundPresent = true;
                  return Ext.String.format('{0} <object><embed src="../../sounds/FireAlarm.wav" hidden="true" autostart="True" loop="true" type="audio/wav" pluginspage="https://www.apple.com/quicktime/download/" /></object>', value);
              }
              else {
                  return value;
              }
          }
         ,
          dataIndex: 'AlarmDescription',
          sortable: false
      }
      ,
      {
          text: 'Vehicle Description',
          align: 'left',
          width: 120,
          dataIndex: 'vehicleDescription',
          sortable: false
      }
      ]
      , listeners: {
          'activate': function (grid, eOpts) {
              $('.alarmtabtitleunreadmsg').show();
              $('.alarmtabtitleunreadmsg').removeClass('blinking');
          }
      }
      ,
       // paging bar on the bottom
       bbar: alarmsPager
   }
   );

       var messagegrid = Ext.create('Ext.grid.Panel',
   {
       id: 'messagegrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width,
       maxHeight: window.screen.height,
       stateful: true,

       closable: false,
       enableColumnHide: false,
       enableSorting: false,
       closable: false,
       width: window.screen.width,
       autoHeight: true,
       title: 'Messages',
       store: messagesstore,
       columnLines: true,
       stateId: 'stateMGrid',
       viewConfig:
      {
          emptyText: 'No messages to display',
          useMsg: false
      }
      ,
       columns: [
      {
          text: 'MessageId',
          align: 'left',
          width: 80,
          renderer: function (value, p, record) {

              var MsgKey = Ext.String.escape(record.data['MsgKey']);
              return Ext.String.format('<a href="javascript:void(0);" OnClick="NewMessageWindow(\'{0}\')">{1}</a>', MsgKey, value);
          }
         ,
          dataIndex: 'MessageId',
          sortable: false
      }
      ,
      {
          text: 'Date/Time',
          align: 'left',
          width: 120,
          xtype: 'datecolumn',
          format: userdateformat, //dateformat,
          dataIndex: 'MsgDateTime',
          sortable: false
      }
      ,
      {
          text: 'From',
          align: 'left',
          width: 150,
          dataIndex: 'Description',
          sortable: false
      }
      ,
      {
          text: 'Message Body',
          align: 'left',
          width: 200,
          dataIndex: 'MsgBody',
          sortable: false
      }
      ,
      {
          text: 'Acknowledged',
          align: 'left',
          width: 120,
          dataIndex: 'Acknowledged',
          sortable: false
      }
      ]
      , dockedItems: [
          {
              xtype: 'toolbar',
              dock: 'top',
              items: [
                sendmessage
             ]
          }
      ]
      , listeners: {
          'activate': function (grid, eOpts) {
              $('.messagetabtitleunreadmsg').show();
              $('.messagetabtitleunreadmsg').removeClass('blinking');
          }
      }
      ,
       // paging bar on the bottom
       bbar: messagesPager
   }
   );

       geolandmarkgrid = Ext.create('Ext.grid.Panel',
   {
       id: 'geolandmarkgrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width,
       maxHeight: window.screen.height,
       stateful: true,

       closable: false,
       enableColumnHide: false,
       enableSorting: false,
       closable: false,
       width: window.screen.width,
       autoHeight: true,
       title: 'Geozone/Landmarks',
       store: geolandmarksstore,
       columnLines: true,
       stateId: 'stateLandmarkGrid',
       viewConfig:
      {
          emptyText: 'No Geozone/Landmarks to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Name', dataIndex: 'name' },
        { header: 'Type', dataIndex: 'type' }
       /*,
       { header: '',
       renderer: function (value, p, record) {
       //var MsgKey = Ext.String.escape(record.data['MsgKey']);                
       return '<a href="javascript:void(0);" onclick="return confirm(\'Are you sure you want to delete?\');"><img border="0" src="../images/delete.gif"></a>';
       }
       }*/
      ],


       listeners: {
           'activate': function (grid, eOpts) {
               geolandmarkgrid.setTitle("Loading...");

               setTimeout(function () { loadGeozoneLandmarks(); }, 100);


           },
           'cellclick': function (grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {

               $(".highlightgrid").attr("style", "");
               $(".highlightgrid").removeClass("highlightgrid");
               $(tr).children("td").addClass("highlightgrid");
               $(tr).children("td").attr("style", "background-color: #BBCCFF !important");

               var el = document.getElementById(mapframe).contentWindow;
               el.map.zoomToExtent(record.data.f.geometry.getBounds(), closest = true);
               var currentZoom = el.map.getZoom();
               el.map.zoomTo(currentZoom - 2);

               el.onFeatureSelect(record.data.f);
           }
       },
       bbar: ['->', 'Total Geozone/Landmarks: <span id="geolandmarkcount" style="margin-right:20px;">0</span>']
   }
   );
       var vehiclegeozoneassignment = Ext.create('Ext.Button',
   {
       text: 'Vehicle-Geozone Assignment',
       id: 'vehiclegeozoneassignmentButton',
       tooltip: 'Vehicle-Geozone Assignment',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               /*var legendURL = "./GeoZone_Landmarks/frmVehicleGeoZone.aspx";
               var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + legendURL + '"></iframe>';
               openWindow('Vehicle-Geozone Assignment', urlToLoad, 1010, 660);*/

               var mypage = './GeoZone_Landmarks/frmVehicleGeoZone.aspx'
               var myname = '';
               var w = 1010;
               var h = 660;
               var winl = (screen.width - w) / 2;
               var wint = (screen.height - h) / 2;
               winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
               win = window.open(mypage, myname, winprops)
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }


           }
           catch (err) {
           }
       }
   }
   );

       geozonegrid = Ext.create('Ext.grid.Panel',
   {
       id: 'geozonegrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width,
       maxHeight: window.screen.height,

       closable: false,
       enableColumnHide: false,
       enableSorting: true,
       width: window.screen.width,
       autoHeight: true,
       title: 'Geozones',
       store: geozonesstore,
       columnLines: true,
       stateId: 'stateGeozoneGrid',

       enableColumnHide: true,
       stateful: false,
       width: window.screen.width,
       collapsible: true,
       animCollapse: true,
       split: true,
       features: [filters],

       viewConfig:
      {
          emptyText: 'No Geozones to display',
          useMsg: false
      }
      ,
       columns: [
        {
            header: 'Geozone',
            dataIndex: 'name',
            align: 'left',
            width: 170,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            text: 'Description',
            align: 'left',
            width: 270,
            dataIndex: 'desc',
            filterable: true,
            sortable: true,
            hidden: true
        },
        {
            header: 'Direction',
            dataIndex: 'direction',
            align: 'left',
            width: 70,
            filterable: true,
            sortable: true,
            hidden: false
        },
        { header: 'Severity', dataIndex: 'SeverityName' },
        { header: '',
            width: 120,
            renderer: function (value, p, record) {
                //var url = "./GeoZone_Landmarks/frmViewVehicleGeozones.aspx?geozoneId=" + value;
                //var urlToLoad = '<iframe width=\\\'100%\\\' height=\\\'100%\\\' frameborder=\\\'0\\\' scrolling=\\\'no\\\' src=\\\'' + url + '\\\'></iframe>';                
                //return Ext.String.format('<a href="javascript:void(0);" OnClick="Ext.openWindow(\'Current Assignment\', \'{1}\', 400, 220)">Current Assignment</a>', value, urlToLoad);

                return Ext.String.format('<a href="javascript:void(0);" OnClick="GetGeozoneCurrentAssignment({0});">Current Assignment</a>', value);
            },
            dataIndex: 'id'
        }
      ],
       listeners: {
           'activate': function (grid, eOpts) {
               if (!geozonegridloaded) {
                   geozonegrid.setTitle("Loading...");
                   setTimeout(function () { loadGeozones(); }, 100);
               }
           },
           'cellclick': function (grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {
               $(".highlightgrid").attr("style", "");
               $(".highlightgrid").removeClass("highlightgrid");
               $(tr).children("td").addClass("highlightgrid");
               $(tr).children("td").attr("style", "background-color: #BBCCFF !important");
           },
           'celldblclick': function (grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {
               var el = document.getElementById(mapframe).contentWindow;
               el.map.zoomToExtent(record.data.f.geometry.getBounds(), closest = true);
               var currentZoom = el.map.getZoom();
               el.map.zoomTo(currentZoom - 2);

               el.onFeatureSelect(record.data.f);
           }
       },
       dockedItems: [
          {
              xtype: 'toolbar',
              dock: 'top',
              items: [
                vehiclegeozoneassignment
             ]
          }
        ]
       ,
       bbar: ['->', 'Total Geozones: <span id="geozonecount" style="margin-right:20px;">0</span>']
   }
   );

       landmarkgrid = Ext.create('Ext.grid.Panel',
   {
       id: 'landmarkgrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width,
       maxHeight: window.screen.height,
       stateful: true,

       closable: false,
       enableColumnHide: false,
       enableSorting: false,
       closable: false,
       width: window.screen.width,
       autoHeight: true,
       title: 'Landmarks',
       store: landmarksstore,
       columnLines: true,
       stateId: 'stateLGrid',

       enableColumnHide: true,
       stateful: false,
       width: window.screen.width,
       collapsible: true,
       animCollapse: true,
       split: true,
       features: [filters],

       viewConfig:
      {
          emptyText: 'No Landmarks to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Landmark', dataIndex: 'name', align: 'left',
            width: 170,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        { header: 'Description', dataIndex: 'desc', align: 'left',
            width: 120,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: true
        },
        { header: 'Street Address', dataIndex: 'StreetAddress', align: 'left',
            width: 270,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        { header: 'Email', dataIndex: 'Email', align: 'left',
            width: 100,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        { header: 'Contact Phone', dataIndex: 'ContactPhoneNum', align: 'left',
            width: 90,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        { header: 'Radius (m)', dataIndex: 'radius', align: 'right',
            width: 70,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        }
      ],
       listeners: {
           'activate': function (grid, eOpts) {
               if (!landmarkgridloaded) {
                   landmarkgrid.setTitle("Loading...");
                   setTimeout(function () { loadLandmarks(); }, 100);
               }
           },
           'cellclick': function (grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {

               $(".highlightgrid").attr("style", "");
               $(".highlightgrid").removeClass("highlightgrid");
               $(tr).children("td").addClass("highlightgrid");
               $(tr).children("td").attr("style", "background-color: #BBCCFF !important");
           },
           'celldblclick': function (grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {
               var el = document.getElementById(mapframe).contentWindow;
               el.map.zoomToExtent(record.data.f.geometry.getBounds(), closest = true);
               var currentZoom = el.map.getZoom();
               el.map.zoomTo(currentZoom - 2);

               el.onFeatureSelect(record.data.f);
           }
       }
       ,
       bbar: ['->', 'Total Landmarks: <span id="landmarkcount" style="margin-right:20px;">0</span>']
   }
   );

       var geozonelandmarktabs = Ext.create('Ext.tab.Panel',
   {
       region: 'center', // a center region is ALWAYS required for border layout
       deferredRender: false,
       title: 'Geozone/Landmarks',
       activeTab: 0,     // first tab initially active
       items: [geozonegrid, landmarkgrid]

   }
   );

       var selHistoryModel = Ext.create('Ext.selection.CheckboxModel',
   {
       checkOnly: true,
       enableKeyNav: false,
       listeners:
      {
          selectionchange: function (selModel, selections) {
              try {


              }
              catch (err) {
              }
          }
      }
   }
   );

       var selHistoryStopModel = Ext.create('Ext.selection.CheckboxModel',
   {
       checkOnly: true,
       enableKeyNav: false,
       listeners:
      {
          selectionchange: function (selModel, selections) {
              try {


              }
              catch (err) {
              }
          }
      }
   }
   );


       /*
       *
       * Code of history grid with form
       *
       */


       /*
       * Here is where we create the Form
       */
       historyDateFrom = Ext.create('Ext.form.field.Date', {
           anchor: '100%',
           labelWidth: 50,
           maxWidth: 190,
           fieldLabel: 'From',
           name: 'historyDateFrom',
           format: userDate,
           value: new Date()
       });
       historyTimeFrom = Ext.create('Ext.form.field.Time', {
           name: 'historyTimeFrom',
           fieldLabel: '',
           labelWidth: 0,
           minValue: '12 AM',
           maxValue: '11:45 PM',
           increment: 15,
           value: '12:00 AM',
           maxWidth: 100,
           margin: '0 0 0 10',
           editable: false
       });
       historyDateTo = Ext.create('Ext.form.field.Date', {
           anchor: '100%',
           labelWidth: 50,
           maxWidth: 190,
           fieldLabel: 'To',
           name: 'historyDateTo',
           format: userDate,
           //value: (new Date()).getDate() + 1
           value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1)
       });
       historyTimeTo = Ext.create('Ext.form.field.Time', {
           name: 'historyTimeTo',
           fieldLabel: '',
           labelWidth: 0,
           minValue: '12 AM',
           maxValue: '11:45 PM',
           increment: 15,
           value: '12:00 AM',
           maxWidth: 100,
           margin: '0 0 0 10',
           editable: false
       });

       var historyDateTimeContainer = Ext.create('Ext.Panel', {
           //title: 'Messages',
           labelWidth: 0,
           border: 0,
           frame: false,
           bodyStyle: 'padding:0;border:0;background-color:transparent;',
           width: 300,
           layout: 'column', // arrange fieldsets side by side
           defaults: {
               width: 240,
               labelWidth: 90
           },
           //margin: '10px 0',
           header: false,
           defaultType: 'textfield',
           items: [
           historyDateFrom,
           historyTimeFrom,
           historyDateTo,
           historyTimeTo]
       });

       var btnHistorySearch = Ext.create('Ext.Button',
       {
           text: 'Advanced Search',
           id: 'btmHistorySearch',
           tooltip: 'Search History',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'left',
           handler: function () {
               try {
                   if (historyForm.isHidden())
                       historyForm.show();
                   else
                       historyForm.hide();

               }
               catch (err) {
               }
           }
       }
       );

       var btnHistoryMapit = Ext.create('Ext.Button',
       {
           text: 'Map It',
           id: 'btnHistoryMapit',
           tooltip: 'Map the selected history records',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           handler: function () {
               try {
                   var historytype = historyType.getValue();
                   if (historytype == 0) {
                       selections = historygrid.getSelectionModel().getSelection();
                       mapHistories(selections, true, true, false);
                   }
                   else if (historytype == 1 || historytype == 2 || historytype == 3) {
                       selections = historyStopGrid.getSelectionModel().getSelection();
                       mapHistories(selections, true, true, true);
                   }
                   else if (historytype == 4) {
                       var selections = [];
                       for (itrip = 0; itrip < historyTripsNum; itrip++) {
                           var innerGrid = Ext.getCmp('hidtoryDetailsGrid' + itrip);
                           if (innerGrid) {
                               var ss = innerGrid.getSelectionModel().getSelection();
                               selections = selections.concat(ss);
                           }
                       }
                       //var innerGrid = Ext.getCmp('hidtoryDetailsGrid' + '0');
                       //var selections = innerGrid.getSelectionModel().getSelection();
                       mapHistories(selections, true, true, false);
                   }

               }
               catch (err) {
                   var i = 0;
               }
           }
       }
       );

       var winMapLegend;

       var btnHistoryLegend = Ext.create('Ext.Button',
       {
           text: 'Map Legend',
           id: 'btnHistoryLegend',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           hidden: true,
           handler: function () {
               try {
                   if (!winMapLegend) {
                       var legendURL = "./History/frmhistoryMapsoluteLegend.aspx?f=1";
                       var urlToLoad = '<iframe style="background-color:white;" width="100%" height="100%" frameborder="0" scrolling="no" src="' + legendURL + '"></iframe>';
                       winMapLegend = openWindow('Map Legend', urlToLoad, 300, 230);
                   }
                   else {
                       if (winMapLegend.isVisible()) {
                           winMapLegend.hide();
                       } else {
                           winMapLegend.show();
                       }
                   }
               }
               catch (err) {
               }
           }
       }
       );

       var btnHistoryMapAll = Ext.create('Ext.Button',
       {
           text: 'Map All',
           id: 'btnHistoryMapAll',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           hidden: false,
           handler: function () {
               try {
                   if (AllHistoryRecords.length > 0)
                       mapHistories(AllHistoryRecords, true, false, false);
               }
               catch (err) {
               }
           }
       }
       );

       var historyType_values = [
            [0, 'Vehicle Path'],
            [1, 'Stop and Idle Sequence'],
            [2, 'Stop Sequence'],
            [3, 'Idle Sequence'],
            [4, 'Trip Report']
        ];

       var historyType_store = new Ext.data.SimpleStore({
           fields: ['number', 'histype'],
           data: historyType_values
       });


       var historyType = new Ext.form.ComboBox({
           name: 'historyType',
           fieldLabel: 'Type',
           hiddenName: 'historyType',
           store: historyType_store,
           displayField: 'histype',
           valueField: 'number',
           value: 0,
           labelWidth: 50,
           width: 300,
           typeAhead: true,
           mode: 'local',
           triggerAction: 'all',
           emptyText: 'Choose number...',
           selectOnFocus: true,
           editable: false,
           listeners:
             {
                 scope: this,
                 'select': function (combo, value) {
                     try {
                         var selectedtype = combo.getValue();
                         if (selectedtype == 0) {
                             btnHistoryMapAll.show();
                             btnHistoryLegend.hide();
                             historytabs.setActiveTab(historyMessageForm);
                         }
                         else if (selectedtype == 1 || selectedtype == 2 || selectedtype == 3) {
                             btnHistoryLegend.show();
                             btnHistoryMapAll.hide();
                             historytabs.setActiveTab(historyMessageForm);
                         }
                         else {
                             btnHistoryLegend.hide();
                             btnHistoryMapAll.hide();
                             historytabs.setActiveTab(historyTripRadios);
                         }
                     }
                     catch (err) {
                     }
                 }
             }
       });

       historyHiddenFleet = Ext.create('Ext.form.field.Hidden',
            {
                name: 'historyFleet',
                value: LoadVehiclesBasedOn == 'fleet' ? SelectedFleetId : DefaultOrganizationHierarchyFleetId
            }
        );

       //        var historyfleets = Ext.create('Ext.form.ComboBox',
       //       {
       //           name: 'historyFleet',
       //           store: 'FleetStore',
       //           displayField: 'FleetName',
       //           valueField: 'FleetId',
       //           typeAhead: true,
       //           fieldStyle: 'cmbfonts',
       //           labelCls: 'cmbLabel',
       //           queryMode: 'local',
       //           triggerAction: 'all',
       //           fieldLabel: ' Fleet',
       //           emptyText: fleetDefaultText,
       //           tooltip: 'Select group of vehicles to show',
       //           selectOnFocus: true,
       //           width: 300,
       //           labelWidth: 50,
       //           editable: false,
       //           listeners:
       //          {
       //              scope: this,
       //              'select': function (combo, value) {
       //                 //alert('selec changed');
       //                 var selFleet = combo.getValue();
       //                 try {
       //                    historyVehicleStore.load(
       //                        {
       //                            params:
       //                            {
       //                                fleetID: selFleet
       //                            }
       //                        }
       //                    );
       //                 }
       //                 catch(err){
       //                 }                  
       //              },
       //              'afterrender': function () {
       //                  //historyVehicleStore.load();
       //              }
       //          }
       //       }
       //       );

       historyVehicleStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryVehicleList',
               autoLoad: false,
               storeId: 'historyVehicleStore',
               listeners:
               {
                   'load': function (xstore, records, options) {

                       var u = Ext.create('HistoryVehicleList', {
                           VehicleId: '-1',
                           Description: 'Select a Vehicle'
                       });
                       xstore.insert(0, u);
                       u = Ext.create('HistoryVehicleList', {
                           VehicleId: '0',
                           Description: 'Entire Fleet'
                       });
                       xstore.insert(1, u);

                       if (historyIniVehicleId != '') {
                           historyVehicles.setValue(historyIniVehicleId);
                           historyIniVehicleId = '';
                       }
                       else
                           historyVehicles.setValue('-1');

                       historyVehicleStoreLoaded = true;
                   }
               },
               proxy:
                  {
                      // load using HTTP
                      type: 'ajax',
                      url: './historynew/historyservices_Reefer.aspx',
                      timeout: proxyTimeOut,
                      reader:
                     {
                         type: 'xml',
                         root: 'Fleet',
                         record: 'VehiclesInformation'
                     }
                  }
           }
       );

       var historyVehicles = Ext.create('Ext.form.ComboBox',
       {
           name: 'historyVehicle',
           store: 'historyVehicleStore',
           displayField: 'Description',
           valueField: 'VehicleId',
           typeAhead: true,
           fieldStyle: 'cmbfonts',
           labelCls: 'cmbLabel',
           queryMode: 'local',
           triggerAction: 'all',
           fieldLabel: ' Vehicle',
           //emptyText: fleetDefaultText,
           emptyText: 'Loading...',
           tooltip: 'Select a vehicles',
           selectOnFocus: true,
           width: 300,
           labelWidth: 50,
           editable: false,
           listeners:
          {
              scope: this,
              'select': function (combo, value) {
                  //alert('selec changed');
                  var selVehicle = combo.getValue();
                  try {
                      historyCommModeStore.load(
                        {
                            params:
                            {
                                vehicleID: selVehicle
                            }
                        }
                    );
                  }
                  catch (err) {
                  }

              },
              'afterrender': function () {
                  //fleetstore.load();
                  //alert('afterrender');

                  if (LoadVehiclesBasedOn == 'fleet') {
                      //alert('fleet:' + historyHiddenFleet.getValue());
                      historyVehicleStore.load(
                        {
                            params:
                                {
                                    fleetID: SelectedFleetId
                                }
                        }
                      );
                  }
                  else {
                      historyVehicleStore.load(
                            {
                                params:
                                {
                                    //fleetID: HistoryOrganizationHierarchyFleetId
                                    fleetId: DefaultOrganizationHierarchyFleetId
                                }
                            }
                        );
                  }
              }
          }
       }
       );

       var historyCommModeStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryCommModeList',
               autoLoad: false,
               storeId: 'historyCommModeStore',
               listeners:
               {
                   'load': function (xstore, records, options) {

                       var u = Ext.create('HistoryCommModeList', {
                           DclId: '-1',
                           CommModeName: 'ALL'
                       });
                       xstore.insert(0, u);

                       historyCommModes.setValue('-1');
                   }
               },
               proxy:
                  {
                      // load using HTTP
                      type: 'ajax',
                      url: './historynew/historyservices_Reefer.aspx?st=getcommmode',
                      timeout: proxyTimeOut,
                      reader:
                     {
                         type: 'xml',
                         root: 'Box',
                         record: 'BoxConfigInfo'
                     }
                  }
           }
       );
       var historyCommModes = Ext.create('Ext.form.ComboBox',
       {
           name: 'historyCommMode',
           store: 'historyCommModeStore',
           displayField: 'CommModeName',
           valueField: 'DclId',
           typeAhead: true,
           fieldStyle: 'cmbfonts',
           labelCls: 'cmbLabel',
           queryMode: 'local',
           triggerAction: 'all',
           fieldLabel: ' Comm',
           emptyText: 'Loading...',
           tooltip: 'Select a Comm Mode',
           selectOnFocus: true,
           width: 300,
           labelWidth: 50,
           editable: false,
           listeners:
          {
              scope: this,
              'select': function (combo, value) {
                  try {

                  }
                  catch (err) {
                  }

              },
              'afterrender': function () {
                  //fleetstore.load();
                  //alert('afterrender');
                  historyCommModeStore.load();
              }
          }
       }
       );

       var historyMessageCheckBox = Ext.create('Ext.form.field.Checkbox',
           {
               boxLabel: 'Last message only',
               name: 'lastmessageonly',
               inputValue: '1',
               id: 'checkbox1',
               border: 0
           }
       );

       var historyMessageStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryMessageModel',
               autoLoad: false,
               storeId: 'historyMessageStore',
               listeners:
               {
                   'load': function (xstore, records, options) {
                       var u = Ext.create('HistoryMessageModel', {
                           BoxMsgInTypeId: '-1',
                           BoxMsgInTypeName: 'All Messages'
                       });
                       xstore.insert(0, u);

                       historyMessageList.setValue('-1');
                   }
               },
               proxy:
                  {
                      // load using HTTP
                      type: 'ajax',
                      url: './historynew/historyservices_Reefer.aspx?st=GetMessageList',
                      timeout: proxyTimeOut,
                      reader:
                     {
                         type: 'xml',
                         root: 'Box',
                         record: 'BoxMsgTypes'
                     }
                  }
           }
       );

       var historyMessageList = Ext.create('Ext.ux.form.MultiSelect',
            {
                anchor: '100%',
                msgTarget: 'side',
                fieldLabel: '',
                name: 'historyMessageList',
                width: 280,
                allowBlank: true,
                valueField: 'BoxMsgInTypeId',
                displayField: 'BoxMsgInTypeName',
                height: 120,
                emptyText: '',
                // minSelections: 2,
                // maxSelections: 3,

                store: historyMessageStore,
                ddReorder: false,
                listeners:
                  {
                      scope: this,
                      'afterrender': function () {
                          historyMessageStore.load();
                      }
                  }
            }
        );

       var historyMessageForm = Ext.create('Ext.Panel', {
           title: 'Messages',
           labelWidth: 50, // label settings here cascade unless overridden
           frame: true,
           bodyStyle: 'padding:5px 5px 0;',
           width: 550,
           layout: 'column', // arrange fieldsets side by side
           defaults: {
               width: 240,
               labelWidth: 90
           },
           //margin: '10px 0',
           header: true,
           defaultType: 'textfield',
           items: [historyMessageCheckBox, historyMessageList]
       });

       var historyLocationText = Ext.create('Ext.form.field.Text', {
           name: 'historyLocation',
           labelWidth: 50,
           fieldLabel: 'Address',
           allowBlank: true  // requires a non-empty value
       }
        );

       var historyByLocation = Ext.create('Ext.form.field.Hidden', {
           name: 'historyByLocation',
           value: '0'
       }
        );

       var historyLoactionForm = Ext.create('Ext.Panel', {
           id: 'historyLoactionForm',
           title: 'Location',
           labelWidth: 50, // label settings here cascade unless overridden
           frame: true,
           bodyStyle: 'padding:5px 5px 0;',
           width: 550,
           layout: 'column', // arrange fieldsets side by side
           defaults: {
               width: 240,
               labelWidth: 90
           },
           //margin: '10px 0',
           header: true,
           defaultType: 'textfield',
           items: [historyLocationText]
       });


       var historyTripRadios = Ext.create('Ext.Panel', {
           title: 'Trip',
           labelWidth: 0,
           border: 0,
           frame: true,
           bodyStyle: 'padding:10;border:0;background-color:transparent;',
           width: 310,
           layout: 'column', // arrange fieldsets side by side
           defaults: {
               width: 240,
               labelWidth: 90
           },
           //margin: '10px 0',
           header: false,
           defaultType: 'radiofield',
           items: [{ xtype: 'component', html: 'Calculate Trips based on:', cls: '' },
                    {
                        boxLabel: 'Ignition',
                        name: 'historytrip',
                        inputValue: '3',
                        id: 'historytrip1',
                        checked: true
                    }, {
                        boxLabel: 'Tractor Power',
                        name: 'historytrip',
                        inputValue: '11',
                        id: 'historytrip2'
                    }, {
                        boxLabel: 'PTO',
                        name: 'historytrip',
                        inputValue: '8',
                        id: 'historytrip3'
                    }]
       });

       var btnSubmit = Ext.create('Ext.Button', {
           text: 'View',
           cls: 'cmbfonts',
           //margin: '10 auto',
           style: { margin: '10px 0 10px 55px' },
           width: 100,
           handler: function () {
               try {
                   if (historyVehicles.getValue() == null || historyVehicles.getValue() == '-1') {
                       Ext.Msg.alert('Oops', 'Please select a vehicle...');
                       return;
                   }
                   //var form = this.up('form').getForm();
                   var form = historyForm.getForm();
                   var historytype = historyType.getValue();

                   if (historytype == 0) {
                       historyGridForm.remove(historyStopGrid, false);
                       historyGridForm.remove(historyTripGrid, false);
                       historystore.removeAll();
                       historyPageStore.removeAll();
                       historyGridForm.add(historygrid);
                   }
                   else if (historytype == 1 || historytype == 2 || historytype == 3) {
                       historyGridForm.remove(historygrid, false);
                       historyGridForm.remove(historyTripGrid, false);
                       historyStopStore.removeAll();
                       historyGridForm.add(historyStopGrid);
                   }
                   else if (historytype == 4) {
                       historyGridForm.remove(historygrid, false);
                       historyGridForm.remove(historyStopGrid, false);
                       historyTripStore.removeAll();
                       historyGridForm.add(historyTripGrid);
                   }
                   historyGridForm.doLayout();

                   if (form.isValid()) {
                       loadingMask.show();
                       form.submit({
                           url: './historynew/historyservices_Reefer.aspx?st=gethistoryrecords&start=0&limit=' + HistoryPagesize,
                           timeout: 1800,
                           success: function (form, action) {
                               try {
                                   var d;

                                   d = action.result.data;
                                   d = d.replace(/\u003c/g, "<").replace("\u003e", ">");

                                   if (action.result.iconTypeName != "") IconTypeName = action.result.iconTypeName;
                                   var doc;
                                   if (window.ActiveXObject) {         //IE
                                       var doc = new ActiveXObject("Microsoft.XMLDOM");
                                       doc.async = "false";
                                       doc.loadXML(d);
                                   } else {                             //Mozilla
                                       var doc = new DOMParser().parseFromString(d, "text/xml");
                                   }

                                   if (historytype == 0) {
                                       var wholedata = action.result.wholedata;
                                       wholedata = wholedata.replace(/\u003c/g, "<").replace("\u003e", ">");

                                       var docwholedata;
                                       if (window.ActiveXObject) {         //IE
                                           docwholedata = new ActiveXObject("Microsoft.XMLDOM");
                                           docwholedata.async = "false";
                                           docwholedata.loadXML(wholedata);
                                       } else {                             //Mozilla
                                           docwholedata = new DOMParser().parseFromString(wholedata, "text/xml");
                                       }

                                       historystore.loadRawData(docwholedata);
                                       historyPagerDoc = '1';
                                       historyPager.moveFirst();
                                       historyPageStore.loadRawData(doc);
                                   }
                                   else if (historytype == 1 || historytype == 2 || historytype == 3) {
                                       historyStopStore.loadRawData(doc);
                                   }
                                   else if (historytype == 4) {
                                       historyTripStore.loadRawData(doc);

                                       var tripdetails = action.result.tripdata;
                                       tripdetails = tripdetails.replace(/\u003c/g, "<").replace("\u003e", ">");

                                       var doctripdata;
                                       if (window.ActiveXObject) {         //IE
                                           doctripdata = new ActiveXObject("Microsoft.XMLDOM");
                                           doctripdata.async = "false";
                                           doctripdata.loadXML(tripdetails);
                                       } else {                             //Mozilla
                                           doctripdata = new DOMParser().parseFromString(tripdetails, "text/xml");
                                       }

                                       historystore.loadRawData(doctripdata);
                                   }

                                   //historystore.loadRawData(doc);
                                   historyForm.hide();
                                   loadingMask.hide();
                               }
                               catch (error) {
                                   historyForm.hide();
                                   loadingMask.hide();
                               }

                           },
                           failure: function (form, action) {
                               loadingMask.hide();
                               if (action.result && action.result.msg && action.result.msg != '')
                                   Ext.Msg.alert('Failed', action.result.msg);
                               else
                                   Ext.Msg.alert('Failed', 'some error');                               
                           }
                       });
                   }
               }
               catch (err) {
               }
           }
       });

       var txtButtonTitle = Ext.create('Ext.draw.Text', {
           text: LoadVehiclesBasedOn == 'fleet' ? 'Fleet:' : 'Hierarchy:',
           width: LoadVehiclesBasedOn == 'fleet' ? 50 : 55,
           height: 20
       });

       var historyFormFieldContainer = Ext.create('Ext.Panel', {
           //title: 'Messages',
           labelWidth: 0,
           border: 0,
           frame: false,
           bodyStyle: 'padding:0;border:0;background-color:transparent;',
           width: 310,
           layout: 'column', // arrange fieldsets side by side
           defaults: {
               width: 240,
               labelWidth: 90
           },
           //margin: '10px 0',
           header: false,
           defaultType: 'textfield',
           items: [historyDateTimeContainer, historyType, historyHiddenFleet,
                txtButtonTitle,
                LoadVehiclesBasedOn == 'fleet' ? historyFleetButton : historyOrganizationHierarchy,
                historyVehicles, historyCommModes/*, btnSubmit*/]
       });

       var historytabs = Ext.create('Ext.tab.Panel',
       {
           region: 'center', // a center region is ALWAYS required for border layout
           deferredRender: false,
           titleCollapse: false,
           autoScroll: true,
           border: false,
           width: 300,
           height: 180,

           autoHeight: true,
           collapsible: false,
           animCollapse: true,
           autoDestroy: false,
           activeTab: 0,     // first tab initially active
           items: [historyMessageForm, historyLoactionForm, historyTripRadios],
           listeners:
            {
                tabchange: function (tabPanel, newCard, oldCard, eOpts) {
                    if (newCard.id == 'historyLoactionForm') {
                        historyByLocation.setValue('1');
                    }
                    else {
                        historyByLocation.setValue('0');
                    }
                }
            }

       }
       );

       var historyForm = Ext.create('Ext.form.Panel', {
           title: '',
           labelWidth: 50, // label settings here cascade unless overridden
           url: './historynew/historyservices_Reefer.aspx?st=gethistoryrecords',
           frame: true,
           bodyStyle: 'padding:5px 5px 0;',
           width: 630,
           layout: 'column', // arrange fieldsets side by side
           defaults: {
               width: 240,
               labelWidth: 90
           },
           //margin: '10px 0',
           header: false,
           defaultType: 'textfield',
           items: [historyByLocation, historyFormFieldContainer, historytabs]
       });

       historyGridForm = Ext.create('Ext.Panel', {
           id: 'historyGridForm',
           frame: false,
           border: 0,
           title: 'History',
           header: false,
           bodyPadding: 0,
           margin: '5px',
           width: 750,
           closable: true,
           autoHeight: true,
           autoScroll: true,
           layout: 'anchor',    // Specifies that the items will now be arranged in columns

           fieldDefaults: {
               labelAlign: 'left',
               msgTarget: 'side'
           },

           items: [btnHistorySearch, btnHistoryMapit, btnHistoryMapAll, btnSubmit, btnHistoryLegend, historyForm]
           , listeners: {
               'activate': function (grid, eOpts) {

                   if (historyAddressResetField) {
                       historyAddressResetField = false;
                       SelectedFleetId = historyAddressFleetId;
                       if (historyAddressFleetId == historyHiddenFleet.getValue() && historyVehicleStoreLoaded) {

                           historyVehicles.setValue(historyIniVehicleId);
                           historyIniVehicleId = '';
                       }
                       else if (historyVehicleStoreLoaded) {
                           historyVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: historyAddressFleetId
                                    }
                                }
                            );
                       }

                       if (LoadVehiclesBasedOn == 'fleet') {
                           historyFleetButton.setText('All Vehicles');
                           historyHiddenFleet.setValue(historyAddressFleetId);
                       }
                       else {
                           historyOrganizationHierarchy.setText('All Vehicles');
                           historyHiddenFleet.setValue(historyAddressFleetId);
                       }

                       historyForm.show();
                       removeHistoriesOnMap(mapframe);
                       historystore.removeAll();
                       historyPageStore.removeAll();
                       historyStopStore.removeAll();

                       return;
                   }
                   if (historyIni) {
                       historyIni = false;

                       if (LoadVehiclesBasedOn == 'fleet') {
                           //historyFleetId = SelectedFleetId;
                           //historyFleetName = SelectedFleetName;
                           historyFleetButton.setText(SelectedFleetName);
                           historyHiddenFleet.setValue(SelectedFleetId);
                       }
                       else {
                           //HistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
                           //HistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
                           historyOrganizationHierarchy.setText(organizationHierarchy.getText());
                           historyHiddenFleet.setValue(DefaultOrganizationHierarchyFleetId);
                       }


                       historyForm.show();
                       removeHistoriesOnMap(mapframe);
                       historystore.removeAll();
                       historyPageStore.removeAll();
                       historyStopStore.removeAll();
                   }
                   if (historyIniVehicleId != '' && historyVehicleStoreLoaded) {
                       if (LoadVehiclesBasedOn == 'fleet') {
                           if (historyFleetId == SelectedFleetId) {
                               historyVehicles.setValue(historyIniVehicleId);
                               historyIniVehicleId = '';
                           }
                           else {
                               historyFleetId = SelectedFleetId;
                               historyFleetName = SelectedFleetName;
                               historyFleetButton.setText(historyFleetName);
                               historyHiddenFleet.setValue(historyFleetId);

                               historyVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: historyFleetId
                                    }
                                }
                            );
                           }
                       }
                       else {
                           if (HistoryOrganizationHierarchyFleetId == DefaultOrganizationHierarchyFleetId) {
                               historyVehicles.setValue(historyIniVehicleId);
                               historyIniVehicleId = '';
                           }
                           else {
                               HistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
                               HistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
                               historyOrganizationHierarchy.setText(organizationHierarchy.getText());
                               historyHiddenFleet.setValue(HistoryOrganizationHierarchyFleetId);

                               historyVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: HistoryOrganizationHierarchyFleetId
                                    }
                                }
                            );
                           }
                       }
                   }
               },
               'close': function (panel, eOpts) {
                   removeHistoriesOnMap(mapframe);
                   historystore.removeAll();
                   historyPageStore.removeAll();
                   historyStopStore.removeAll();
               }
           }
       });

       historygrid = Ext.create('Ext.grid.Panel',
   {
       id: 'historygrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       //maxWidth: window.screen.width - 50,
       //maxHeight: window.screen.height,
       anchor: '-10, -45',
       autoHeight: true,
       stateful: true,
       closable: false,
       enableColumnHide: false,
       enableSorting: false,

       title: '',
       store: historyPageStore,
       columnLines: true,
       stateId: 'stateHistoryGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       selModel: selHistoryModel,

       viewConfig:
      {
          emptyText: 'No History to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Unit ID', dataIndex: 'BoxId', align: 'left',
            width: 70,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'Vehicle', dataIndex: 'Description', align: 'left',
            width: 120,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Date/Time', dataIndex: 'OriginDateTime', align: 'left',
            align: 'left',
            width: 150,
            xtype: 'datecolumn',
            format: userdateformat, //'d/m/Y h:i:s a',
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Address', dataIndex: 'StreetAddress', align: 'left',
            align: 'left',
            width: 230,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Speed', dataIndex: 'Speed', align: 'left',
            align: 'left',
            width: 50,
            renderer: function (value, p, record) {
                if (value == -1)
                    return 'N/A';
                else
                    return value;
            },
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Message', dataIndex: 'BoxMsgInTypeName', align: 'left',
            renderer: function (value, p, record, ri) {
                //return '<a href="javascript:var w =HistoryInfo(' + ri + ')">' + value + '</a>';
                return '<a href="' + record.data.CustomUrl + '">' + value + '</a>';
            },
            align: 'left',
            width: 130,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            //header: 'MsgDetails', dataIndex: 'CustomProp', align: 'left',
            header: 'MsgDetails', dataIndex: 'MsgDetails', align: 'left',
            align: 'left',
            width: 130,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Ack', dataIndex: 'Acknowledged', align: 'left',
            align: 'left',
            width: 50,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        }
      ]
       ,
       dockedItems: {
           xtype: 'toolbar',
           dock: 'top',
           items: [
            { icon: 'preview.png',
                cls: 'x-btn-text-icon',
                text: 'Export',
                menu: historyExportMenu
            }
           ]
       },
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       bbar: historyPager
   }
   );

       historyStopGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'historyStopGrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width - 50,
       maxHeight: window.screen.height,

       autoHeight: true,
       stateful: true,
       closable: false,
       enableColumnHide: false,
       enableSorting: false,

       title: '',
       store: historyStopStore,
       columnLines: true,
       stateId: 'stateHistoryStopGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       selModel: selHistoryStopModel,

       viewConfig:
      {
          emptyText: 'No History to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Arrival', dataIndex: 'ArrivalDateTime', align: 'left',
            xtype: 'datecolumn',
            format: userdateformat,
            width: 150,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Address', dataIndex: 'Location',
            align: 'left',
            width: 220,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        { header: 'Departure', dataIndex: 'DepartureDateTime', align: 'left',
            xtype: 'datecolumn',
            format: 'd/m/Y h:i:s a',
            width: 150,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Duration', dataIndex: 'StopDuration',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Status', dataIndex: 'Remarks',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        }
      ],
       dockedItems: {
           xtype: 'toolbar',
           dock: 'top',
           items: [
        { icon: 'preview.png',
            cls: 'x-btn-text-icon',
            text: 'Export',
            menu: historyStopExportMenu
        }
       ]
       },
       bbar: ['->', 'Total Histories: <span id="stophistoriescount" style="margin-right:20px;">0</span>']
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //bbar: historyPager
   }
   );

       historyTripGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'historyTripGrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width - 50,
       //maxHeight: window.screen.height,

       autoHeight: true,
       stateful: true,
       closable: false,
       enableColumnHide: false,
       enableSorting: false,

       title: '',
       store: historyTripStore,
       columnLines: true,
       stateId: 'stateHistoryTripGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       selModel: {
           selType: 'cellmodel'
       },
       plugins: [{
           ptype: 'rowexpander',
           rowBodyTpl: [
                '<div id="tripsummary{TripId}">',
                '</div>'
            ]
       }],

       viewConfig:
      {
          emptyText: 'No History to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Description', dataIndex: 'Description', align: 'left',
            width: 150,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'Departure', dataIndex: 'DepartureTime', align: 'left',
            xtype: 'datecolumn',
            format: userdateformat,
            width: 150,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'Arrival', dataIndex: 'ArrivalTime', align: 'left',
            xtype: 'datecolumn',
            format: userdateformat,
            width: 150,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'From', dataIndex: '_From', align: 'left',
            width: 200,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'To', dataIndex: '_To', align: 'left',
            width: 200,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Duration', dataIndex: 'Duration',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Fuel Consumed', dataIndex: 'FuelConsumed',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        }
      ],
       dockedItems: {
           xtype: 'toolbar',
           dock: 'top',
           items: [
        { icon: 'preview.png',
            cls: 'x-btn-text-icon',
            text: 'Export',
            menu: historyTripExportMenu
        }
       ]
       }
       //,bbar: ['->', 'Total Histories: <span id="stophistoriescount" style="margin-right:20px;">0</span>']
       //,bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //,bbar: historyPager
   }
   );

       historyAddressGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'historyAddressGrid',
       enableColumnHide: true,
       title: 'Search Result',
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width - 5,
       maxHeight: window.screen.height,
       enableSorting: true,
       closable: true,
       columnLines: true,
       width: window.screen.width,
       autoHeight: true,
       store: historyAddressStore,
       collapsible: false,
       animCollapse: false,
       split: true,

       columns: [
              {
                  id: 'haUnitID',
                  //stateId: 'stUnitId',
                  text: 'UnitID',
                  align: 'left',
                  width: 70,
                  dataIndex: 'BoxId',
                  filterable: true,
                  sortable: true,
                  // flex : 1,
                  hidden: false
              },
              {
                  id: 'haDescription',
                  //stateId: 'stDescription',
                  text: 'Description',
                  align: 'left',
                  width: 150,
                  dataIndex: 'Description',
                  filterable: true,
                  sortable: true
              },

              {
                  id: 'haDateTime',
                  //stateId: 'stDateTime',
                  text: 'Date/Time',
                  align: 'left',
                  width: 120,
                  xtype: 'datecolumn',
                  format: userdateformat, //dateformat,
                  //dataIndex: 'OriginDateTime',
                  renderer: function (value) {
                      return SearchHistoryDateTime;
                  },
                  filterable: false,
                  sortable: false,
                  tdCls: 'x-date-time'
              }
              , {
                  id: 'haDetails',
                  text: 'Details',
                  align: 'left',
                  width: 90,
                  dataIndex: 'VehicleId',
                  renderer: function (value, p, record) {
                      return '<a href="javascript:void(0);" OnClick="showHistoryTab(\'' + value + '\', true, ' + record.data.FleetId + ');">Details</a>';
                  }
                 ,
                  dataIndex: 'VehicleId',
                  filterable: false,
                  sortable: false
              }
        ],
       listeners:
              {
                  'close': function () {
                      try {
                          /* clear map markers */
                          var el = document.getElementById(mapframe);
                          if (el.contentWindow) {
                              //el.contentWindow.removeMarkersOnMap();
                              el.contentWindow.searchArea.removeAllFeatures();
                          }
                          else if (el.contentDocument) {
                              //el.contentDocument.removeMarkersOnMap();
                              el.contentDocument.searchArea.removeAllFeatures();
                          }
                      }
                      catch (err) {
                      }
                  }
                 ,
                  scope: this
              }
      , viewConfig: {
          stripeRows: false,
          emptyText: 'No vehicles to display',
          useMsg: false
      },
       dockedItems: {
           xtype: 'toolbar',
           dock: 'top',
           items: [
        { icon: 'preview.png',
            cls: 'x-btn-text-icon',
            text: 'Export',
            menu: historyAddressExportMenu
        }
       ]
       }
   }
   );


       historyTripGrid.view.on('expandBody', function (rowNode, record, expandRow, eOpts) {
           displayInnerGrid(record.get('TripId'), record);
       });

       historyTripGrid.view.on('collapsebody', function (rowNode, record, expandRow, eOpts) {
           destroyInnerGrid(record);
       });

       function displayInnerGrid(renderId, record) {

           var parent = document.getElementById('tripsummary' + record.get('TripId'));
           var child = parent.firstChild;
           if (child != null) {
               child.style.display = "";
               return;
           }

           //Model for the inside grid store
           Ext.define('HistoryTripDetailsModel',
           {
               extend: 'Ext.data.Model',
               fields: [
                  'BoxId', 'VehicleId', 'LicensePlate', 'Description', 'DateTimeReceived', 'DclId', 'BoxMsgInTypeId', 'BoxMsgInTypeName', 'BoxProtocolTypeId', 'BoxProtocolTypeName',
                  'CommInfo1', 'CommInfo2', 'ValidGps', 'Latitude', 'Longitude', 'Heading', 'SensorMask', 'CustomProp', 'BlobDataSize', 'SequenceNum', 'StreetAddress',
                  'OriginDateTime',
               //'Speed', 
                   {
                   name: 'Speed', type: 'int',
                   convert: function (value, record) {
                       if (value >= 0 || value < 0)
                           return value * 1;
                       else
                           return -1;
                   }
               },
                  'BoxArmed', 'MsgDirection', 'Acknowledged', 'Scheduled', 'MsgDetails', 'MyDateTime', 'MyHeading', 'dgKey', 'CustomUrl', 'chkBoxShow'
                  ]
           }
           );

           var historyDetailsStore = Ext.create('Ext.data.Store',
           {
               //buffered: true,
               pageSize: 10000,
               model: 'HistoryTripDetailsModel',
               autoLoad: false,
               listeners:
              {
                  'load': function (store, records, options) {
                      try {
                          /*mapHistories(records, true, false, false);
                          historygrid.getSelectionModel().selectAll(false);
                          $('#historiescount').html(records.length);*/
                          $('#detailshistoriescount' + renderId).html(records.length);
                      }
                      catch (err) {
                      }
                  }
                 ,
                  scope: this
              },

               proxy: {
                   type: 'memory',
                   reader: {
                       type: 'xml',
                       root: 'HistoryTripDetailed',
                       record: 'TripDetails',
                       totalProperty: 'totalCount'
                   }
               }
           });

           var insideSelModel = Ext.create('Ext.selection.CheckboxModel',
		   {
		       checkOnly: true,
		       enableKeyNav: false,
		       listeners:
			  {
			      selectionchange: function (selModel, selections) {
			          try {


			          }
			          catch (err) {
			          }
			      }
			  }
		   }
		   );

           historyDetailsGrid = Ext.create('Ext.grid.Panel',
           {
               id: 'hidtoryDetailsGrid' + renderId,
               animCollapse: false,
               autoLoad: false,
               autoScroll: true,
               loadMask: true,
               maxWidth: window.screen.width - 50,
               //maxHeight: window.screen.height,

               autoHeight: true,
               stateful: true,
               closable: false,
               enableColumnHide: false,
               enableSorting: false,

               title: '',
               store: historyDetailsStore,
               columnLines: true,
               stateId: 'stateHistoryGrid',

               enableColumnHide: true,
               stateful: false,

               collapsible: false,
               animCollapse: true,
               split: true,
               features: [filters],
               margin: '0',
               selModel: insideSelModel,
               renderTo: 'tripsummary' + renderId,
               //enableColumnResize: true,
               //bubbleEvents: ['add', 'remove', 'columnresize'],

               viewConfig:
              {
                  emptyText: 'No History to display',
                  useMsg: false
              }
              ,
               columns: [
                { header: 'Unit ID', dataIndex: 'BoxId', align: 'left',
                    width: 60,
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                },
                { header: 'Vehicle', dataIndex: 'Description', align: 'left',
                    width: 120,
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                },
                {
                    header: 'Date/Time', dataIndex: 'OriginDateTime', align: 'left',
                    align: 'left',
                    width: 150,
                    xtype: 'datecolumn',
                    format: userdateformat, //'d/m/Y h:i:s a',
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                },
                {
                    header: 'Address', dataIndex: 'StreetAddress', align: 'left',
                    align: 'left',
                    width: 280,
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                },
                {
                    header: 'Speed', dataIndex: 'Speed', align: 'left',
                    align: 'left',
                    width: 50,
                    renderer: function (value, p, record) {
                        if (value == -1)
                            return 'N/A';
                        else
                            return value;
                    },
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                },
                {
                    header: 'Message', dataIndex: 'BoxMsgInTypeName', align: 'left',
                    renderer: function (value, p, record, ri) {
                        //return '<a href="javascript:var w =HistoryInfo(' + ri + ')">' + value + '</a>';
                        return '<a href="' + record.data.CustomUrl + '">' + value + '</a>';
                    },
                    align: 'left',
                    width: 140,
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                },
                {
                    //header: 'MsgDetails', dataIndex: 'CustomProp', align: 'left',
                    header: 'MsgDetails', dataIndex: 'MsgDetails', align: 'left',
                    align: 'left',
                    width: 110,
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                },
                {
                    header: 'Ack', dataIndex: 'Acknowledged', align: 'left',
                    align: 'left',
                    width: 50,
                    filterable: true,
                    sortable: true,
                    // flex : 1,
                    hidden: false
                }
              ]
               ,
               bbar: ['->', 'Total Histories: <span id="detailshistoriescount' + renderId + '" style="margin-right:20px;">0</span>']
               //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
               //bbar: historyPager
           }
           );

           var form = historyForm.getForm();

           form.submit({
               url: './historynew/historyservices_Reefer.aspx?fromsession=1&st=gettripdetails&tripId=' + renderId,
               success: function (form, action) {
                   historyDetailsGrid.getView().emptyText = 'No History to display';
                   var d = action.result.data;
                   d = d.replace(/\u003c/g, "<").replace("\u003e", ">");
                   //alert(d);
                   //if (action.result.iconTypeName != "") IconTypeName = action.result.iconTypeName;
                   var doc;
                   if (window.ActiveXObject) {         //IE
                       var doc = new ActiveXObject("Microsoft.XMLDOM");
                       doc.async = "false";
                       doc.loadXML(d);
                   } else {                             //Mozilla
                       var doc = new DOMParser().parseFromString(d, "text/xml");
                   }
                   historyDetailsStore.loadRawData(doc);
                   //historyForm.hide();
               },
               failure: function (form, action) {
                   historygrid.getView().emptyText = 'No History to display';
                   if (action.result && action.result.msg && action.result.msg != '')
                       Ext.Msg.alert('Failed', action.result.msg);
                   else
                       Ext.Msg.alert('Failed', 'some error');                   
               }
           });

           historyDetailsGrid.getEl().swallowEvent([
					'mousedown', 'mouseup', 'click',
					'contextmenu', 'mouseover', 'mouseout',
					'dblclick', 'mousemove'
				]);


       }

       function destroyInnerGrid(record) {

           var parent = document.getElementById('tripsummary' + record.get('TripId'));
           var child = parent.firstChild;

           var innerGrid = Ext.getCmp('hidtoryDetailsGrid' + record.get('TripId'));
           var selections = innerGrid.getSelectionModel().getSelection();
           //alert('you selected: ' + selections.length);

           child.style.display = "none";

       }

       /*
       *
       * Code of history grid with form Finished
       *
       */

       /*var PreTrip = Ext.create('Ext.Panel', {
       labelWidth: 0,
       border: 0,
       frame: false,
       bodyStyle: 'padding:3px;border:0;background-color:transparent;',
       width: 800,
       id: "PreTripTab",
       layout: {
       type: 'column',
       margin: 10
       }, // arrange fieldsets side by side
       defaults: {
       width: 800,
       labelWidth: 90
       },
       header: false,
       title: 'Pre-Trip',
       defaultType: 'textfield'
       });*/

       mapGridOriginalHeight = getDocHeight() - (window.screen.height / 2);
       mapGridOriginalWidth = getDocWidth() - (window.screen.width / 2);

       tabs = Ext.create('Ext.tab.Panel',
   {
       id: 'tabs',
       region: defaultMapView,
       split: true,
       titleCollapse: false,
       header: false,
       autoScroll: true,
       border: false,
       //height: window.screen.height / 2,
       //width: window.screen.width / 2,
       height: $(document).height(), //getDocHeight() - (window.screen.height / 2),
       width: $(document).width(), //getDocWidth() - (window.screen.width / 2),
       autoHeight: true,
       collapsible: true,
       animCollapse: true,
       deferredRender: false,
       activeTab: 0,     // first tab initially active
       autoDestroy: false,
       items: [vehiclegrid, ShowAlarmTab ? alarmgrid : null/*, geozonelandmarktabs*/],
       listeners:
      {
          afterrender: function () {
              try {
                  $('.x-collapse-el').hide();
                  var s = '<div class=\'togglemap\'><a href=\'javascript:void(0);\' onclick="toggleMap();"><img src=\'images/menutogglemaphandler.png\'></a></div>';
                  $('#tabs').parent().prepend(s);
                  var s = '<div class=\'collapsebutton\'><a href=\'javascript:void(0);\' onclick="toggleGrid();"><img src=\'images/menuhandler.png\'></a></div>';
                  $('#tabs').parent().prepend(s);
                  $('#tabs-body').css('overflow', 'hidden');

                  //if (ViolationTabAtNewMapPage)
                  //    IniViolations();
                  //IniReeferImpact();
                  //IniReeferMaintenance();
                  IniReeferPretrip();
                  IniReeferCommandHistory();                  
                  //IniReeferAlarm();
                  IniReeferReeferHistory();
              }
              catch (err) { alert(err); }
          }
      }

   }
   );


       //Devin Added
       try {
           if (DispatchOrganizationId == 480 && false) {
               var DispatchTab = Ext.create('Ext.Panel', {
                   labelWidth: 0,
                   border: 0,
                   frame: false,
                   bodyStyle: 'padding:3px;border:0;background-color:transparent;',
                   width: 800,
                   id: "DispatchTab",
                   layout: {
                       type: 'column',
                       margin: 10
                   }, // arrange fieldsets side by side
                   defaults: {
                       width: 800,
                       labelWidth: 90
                   },
                   header: false,
                   title: 'Dispatch',
                   defaultType: 'textfield'
               });


               tabs.add(DispatchTab);
               tabs.items.each(function (i) {
                   if (i.id == "DispatchTab") {
                       i.tab.on('click', function (el, e) {
                           e.stopEvent();

                           var ant_w = 800;
                           var ant_h = 800;
                           var ant_left = (screen.width / 2) - (ant_w / 2);
                           var ant_top = (screen.height / 2) - (ant_h / 2);

                           window.open('../../Ant/Ant.html', 'Dispatch', 'width=' + ant_w + ',height=' + ant_h + ',left=' + ant_left + ',top=' + ant_top + ',screenX=0,screenY=100');
                       });
                   }
               });
           }

       }
       catch (err) { }

       var viewport = Ext.create('Ext.Viewport',
   {
       layout: 'border',
       border: false,
       items: [northmappanel, tabs],
       listeners:
      {
          afterlayout: function (o, layout, eOpts) {
              if (defaultMapView == 'west') {
                  if (gridStatus == 'expanded') {
                      $('.collapsebutton').css("top", 7);
                      $('.collapsebutton').css("left", tabs.getWidth() - 50);

                      $('.togglemap').css("top", 7);
                      $('.togglemap').css("left", tabs.getWidth() - 80);
                  }

              }
              else if (defaultMapView == 'south') {
                  $('.collapsebutton').css("top", northmappanel.getHeight() + 10);
                  $('.collapsebutton').css("left", tabs.getWidth() - 50);

                  $('.togglemap').css("top", northmappanel.getHeight() + 10);
                  $('.togglemap').css("left", tabs.getWidth() - 80);
              }
              else if (defaultMapView == 'north') {
                  $('.collapsebutton').css("top", 7);
                  $('.collapsebutton').css("left", tabs.getWidth() - 50);

                  $('.togglemap').css("top", 7);
                  $('.togglemap').css("left", tabs.getWidth() - 80);
              }
          }
      }
   }
   );

       function onItemClick(item) {
           alert('Menu Click, You clicked the "' + item.text + '" menu item.');
       }

       function loadGeozoneLandmarks() {
           var el = document.getElementById(mapframe).contentWindow;

           if (el.geozoneLoaded && el.landmarkLoaded) {
               var landmarks = [];
               for (i = 0; i < el.geoLandmarkFeatures.length; i++) {
                   var ss = el.geoLandmarkFeatures[i].fid.split(':::');
                   d = { 'type': ss[0], 'name': ss[1], 'f': el.geoLandmarkFeatures[i] };
                   landmarks.push(d);
               }

               geolandmarksstore.loadData(landmarks);
               geolandmarkgrid.setTitle("Geozone/Landmarks");
               $('#geolandmarkcount').html(landmarks.length);
           }
           else {
               setTimeout(function () { loadGeozoneLandmarks(); }, 500);
           }
       }


       loadingMask.show();

       function getIcon(exirecord, posExpireDate) {
           var newIcon;
           if (exirecord.data.ImagePath != "") {    // custom icon

               if (exirecord.data.OriginDateTime < posExpireDate) {
                   newIcon = "Grey"; // +bicon;
               }
               else {
                   if (exirecord.data.CustomSpeed != 0) {
                       newIcon = "Green"; // +bicon;
                   }
                   else {
                       newIcon = "Red"; // +bicon;
                   }
               }

               var bicon = exirecord.data.ImagePath.replace("\\", "/");
               var cicon = '';
               if (bicon.split("/").length > 1) {
                   //bicon = bicon.split("/")[0];
                   newIcon = bicon.split("/")[0] + "/" + newIcon + bicon.split("/")[1];
               }
               else
                   newIcon = newIcon + bicon;
           }
           else {
               if (exirecord.data.OriginDateTime < posExpireDate) {
                   newIcon = "Grey" + exirecord.data.IconTypeName + ".ico";
               }
               else {
                   if (exirecord.data.CustomSpeed != 0) {
                       newIcon = "Green" + exirecord.data.IconTypeName + exirecord.data.MyHeading + ".ico";
                   }
                   else {
                       newIcon = "Red" + exirecord.data.IconTypeName + ".ico";
                   }
               }
           }
           return newIcon;
       }
   }
);




   Ext.PagingToolbar.override({
       updateInfo: function () {
           var me = this,
            displayItem = me.child('#displayItem'),
            store = me.store,
            pageData = me.getPageData(),
            count, msg;

           if (displayItem) {

               count = store.getCount();
               if (count === 0) {
                   msg = me.emptyMsg;
               } else {
                   msg = Ext.String.format(
                    me.displayMsg,
                    pageData.fromRecord,
                    pageData.toRecord,
                    pageData.total
                );
               }
               displayItem.setText(msg);
               me.doComponentLayout();
           }

       }
   });


   function loadGeozones() {
       var el = document.getElementById(mapframe).contentWindow;

       if (el.geozoneLoaded) {
           var geozones = [];
           for (i = 0; i < el.geoLandmarkFeatures.length; i++) {
               var ss = el.geoLandmarkFeatures[i].fid.split(':::');
               if (ss[0] == "Geozone") {
                   d = { 'type': ss[0], 'name': ss[1], 'f': el.geoLandmarkFeatures[i], 'desc': el.geoLandmarkFeatures[i].GeoDescription, 'direction': el.geoLandmarkFeatures[i].GeoDirection,
                       'SeverityName': el.geoLandmarkFeatures[i].SeverityName,
                       'id': el.geoLandmarkFeatures[i].GeozoneID
                   };
                   geozones.push(d);
               }
           }
           geozonesstore.loadData(geozones);
           //geolandmarksstore.loadData(geolandmarksdata);

           geozonegrid.setTitle("Geozones");

           $('#geozonecount').html(geozones.length);
           geozonegridloaded = true;
       }
       else {
           setTimeout(function () { loadGeozones(); }, 500);
       }
   }

   function loadLandmarks() {
       var el = document.getElementById(mapframe).contentWindow;

       if (el.landmarkLoaded) {
           var landmarks = [];
           for (i = 0; i < el.geoLandmarkFeatures.length; i++) {
               var ss = el.geoLandmarkFeatures[i].fid.split(':::');
               if (ss[0] == "Landmark" || ss[0] == "LandmarkCircle") {
                   d = { 'type': ss[0], 'name': ss[1], 'f': el.geoLandmarkFeatures[i],
                       'desc': el.geoLandmarkFeatures[i].landmarkDescription,
                       'StreetAddress': el.geoLandmarkFeatures[i].StreetAddress,
                       'Email': el.geoLandmarkFeatures[i].Email,
                       'ContactPhoneNum': el.geoLandmarkFeatures[i].ContactPhoneNum,
                       'radius': el.geoLandmarkFeatures[i].radius
                   };
                   landmarks.push(d);
               }
           }
           landmarksstore.loadData(landmarks);
           //geolandmarksstore.loadData(geolandmarksdata);

           landmarkgrid.setTitle("Landmarks");

           $('#landmarkcount').html(landmarks.length);
           landmarkgridloaded = true;
       }
       else {
           setTimeout(function () { loadLandmarks(); }, 500);
       }
   }

   function finditonmap() {
       findit(selectedVehicleBoxId, false);
   }

   function findit(finditvehicleBoxId, clearall) {
       
       if (finditvehicleBoxId == undefined) {
           finditvehicleBoxId = selectedVehicleBoxId;
       }
       if (clearall == undefined)
           clearall = false;
       
       try {
           mapLoading = true;

           if (finditvehicleBoxId > 0) {
               var gridindex = 0;
               if(clearall)
                   vehiclegrid.getSelectionModel().deselectAll(false);
               vehiclegrid.getStore().each(function (record) {
                   if (record.data.BoxId == finditvehicleBoxId) {
                       selectedVehicleData = record.data;
                       if (!vehiclegrid.getSelectionModel().isSelected(gridindex))
                           vehiclegrid.getSelectionModel().select(gridindex, true, false);
                       return false;
                   }
                   gridindex++;

               });

               var selectedBoxs = new Array();
               selectedBoxs.push(selectedVehicleData);
               mapSelecteds(selectedBoxs, "nmapframe");
           }
           mapLoading = false;
       }
       catch (err) {
       }
       return false;
   }

   function getDocHeight() {
       var D = document;
       return Math.max(
        Math.max(D.body.scrollHeight, D.documentElement.scrollHeight),
        Math.max(D.body.offsetHeight, D.documentElement.offsetHeight),
        Math.max(D.body.clientHeight, D.documentElement.clientHeight)
    );
   }

   function getDocWidth() {
       var D = document;
       return Math.max(
        Math.max(D.body.scrollWidth, D.documentElement.scrollWidth),
        Math.max(D.body.offsetWidth, D.documentElement.offsetWidth),
        Math.max(D.body.clientWidth, D.documentElement.clientWidth)
    );
   }

   function getBaseLayer() {
       var el = document.getElementById(mapframe).contentWindow;
       return el.map.baseLayer.name;
   }


   function showHistoryTab(VehicleId, setDateTime, fleetId) {
       historyIniVehicleId = VehicleId;
       historyIni = true;
       //tabs.add(historygrid);
       //tabs.setActiveTab(historygrid);
       if (setDateTime) {
           historyAddressResetField = true;
           historyAddressFleetId = fleetId;
           //historyAddressFleetName = 'test';
           
           var d = SearchHistoryDateTime.split(' ')[0].split('/');
           var t = SearchHistoryDateTime.split(' ')[1].split(':');
           var nd = new Date(d[2], parseInt(d[0]) - 1, d[1], t[0], t[1]); 
           var newDateTimeFrom = new Date(nd.getTime() - SearchHistoryTimeRange * 60000);
           var newDateTimeTo = new Date(nd.getTime() + SearchHistoryTimeRange * 60000);

           historyDateFrom.setValue((newDateTimeFrom.getMonth() + 1) + '/' + newDateTimeFrom.getDate() + '/' + newDateTimeFrom.getFullYear());
           historyTimeFrom.setValue(formatAMPM(newDateTimeFrom));
           historyDateTo.setValue((newDateTimeTo.getMonth() + 1) + '/' + newDateTimeTo.getDate() + '/' + newDateTimeTo.getFullYear());
           historyTimeTo.setValue(formatAMPM(newDateTimeTo));

           //value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1)
       }
       tabs.add(historyGridForm);
       tabs.setActiveTab(historyGridForm);
   }

   function isNumber(n) {
      return !isNaN(parseFloat(n)) && isFinite(n);
  }

  function formatAMPM(date) {
      var hours = date.getHours();
      var minutes = date.getMinutes();
      var ampm = hours >= 12 ? 'PM' : 'AM';
      hours = hours % 12;
      hours = hours ? hours : 12; // the hour '0' should be '12'
      minutes = minutes < 10 ? '0' + minutes : minutes;
      var strTime = hours + ':' + minutes + ' ' + ampm;
      return strTime;
  }




function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName)
{            
    /*$('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
    $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
    $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();*/
    HistoryOrganizationHierarchyNodeCode = nodecode;
    historyOrganizationHierarchy.setText(fleetName);
    HistoryOrganizationHierarchyFleetId = fleetId;

    try {
        historyVehicleStore.load(
            {
                params:
                {
                    fleetID: HistoryOrganizationHierarchyFleetId
                }
            }
        );
    }
    catch(err){
    }           

}

function HistoryInfo(dgKey) {
    var mypage = './History/frmHistDetails.aspx?dgKey=' + dgKey
    var myname = '';
    var w = 580;
    var h = 660;
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=yes,'
    win = window.open(mypage, myname, winprops)
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}

function OnFleetSelect(c, fleetId, fleetName, caller){
    if(c)
    {
        if (caller == 'fleetButton') {
            fleetButton.setText(fleetName);

            SelectedFleetId = fleetId;
            SelectedFleetName = fleetName;

            mainstore.load(
                {
                    params:
                    {
                        QueryType: 'GetfleetPosition',
                        fleetID: fleetId,
                        start: 0,
                        limit: VehicleListPagesize
                    }
                }
            );
        }
        else if (caller == 'violationFleetButton') {
            ViolationOnFleetSelect(fleetId, fleetName);
        }
        else {
            historyFleetId = fleetId;
            historyFleetName = fleetName;
            historyFleetButton.setText(fleetName);
            historyHiddenFleet.setValue(fleetId);

            historyVehicleStore.load(
                {
                    params:
                    {
                        fleetID: historyFleetId
                    }
                }
            );
        }
    }
    fleetWin.hide();
}

function toggleGrid() {
    if (defaultMapView == 'west') {
        if (gridStatus == 'expanded') {
            gridStatus = 'collapse';
            gridOriginalWidth = tabs.getWidth();
            //tabs.hide();
            tabs.setWidth(1);
            $('.collapsebutton').css("left", 30);
            $('.collapsebutton').css("top", 0);

            $('.togglemap').css("left", 0);
            $('.togglemap').css("top", 0);
        }
        else {
            gridStatus = 'expanded';
            //tabs.show();
            tabs.setWidth(gridOriginalWidth);
            $('.collapsebutton').css("left", tabs.getWidth() - 50);
            $('.collapsebutton').css("top", 5);

            $('.togglemap').css("left", tabs.getWidth() - 80);
            $('.togglemap').css("top", 5);
        }        
    }
    else {

        if (gridStatus == 'expanded') {
            gridStatus = 'collapse';
            gridOriginalHeight = tabs.getHeight();
            tabs.setHeight(25);
        }
        else {
            gridStatus = 'expanded';
            tabs.setHeight(gridOriginalHeight);
        }
    }
}

function toggleMap() {
    if (defaultMapView == 'west') {
        if (mapStatus == 'expanded') {
            mapStatus = 'collapse';
            mapGridOriginalWidth = tabs.getWidth();
            //tabs.hide();
            tabs.setWidth($(document).width());

            $('.collapsebutton').css("left", tabs.getWidth() - 50);
            $('.collapsebutton').css("top", 5);
            $('.togglemap').css("left", tabs.getWidth() - 80);
            $('.togglemap').css("top", 5);
        }
        else {
            mapStatus = 'expanded';
            //tabs.show();
            tabs.setWidth(mapGridOriginalWidth);

            $('.collapsebutton').css("left", tabs.getWidth() - 50);
            $('.collapsebutton').css("top", 5);
            $('.togglemap').css("left", tabs.getWidth() - 80);
            $('.togglemap').css("top", 5);
        }
    }
    else {
        if (mapStatus == 'expanded') {
            mapStatus = 'collapse';
            mapGridOriginalHeight = tabs.getHeight();
            tabs.setHeight($(document).height());
        }
        else {
            mapStatus = 'expanded';
            tabs.setHeight(mapGridOriginalHeight);
        }
        //tabs.setHeight($(document).height());
    }
}

var popupWindow;

function openPopupWindow(wintitle, winURL, winWidth, winHeight) {
    try {
        popupWindow.close();
    }
    catch(err){}
    var win = new Ext.Window(
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
      popupWindow = win;
  }

function getClosestVehicles(lon, lat, radius, numofvehicles) {
    //alert(lon + ',' + lat);
    var fleetId;
    if (LoadVehiclesBasedOn == 'fleet') {
        fleetId = DefaultOrganizationHierarchyFleetId;
    }
    else {
        fleetId = DefaultOrganizationHierarchyFleetId;
    }

    loadingMask.show();
    FromClosestVehicles = true;
    mainstore.load(
    {
        params:
        {
            QueryType: 'getClosestVehicles',
            fleetID: DefaultFleetID,
            lon: lon,
            lat: lat,
            radius: radius,
            numofvehicles: numofvehicles   
        }
    });

    clearSearchBtn.show();

}

var SearchHistoryDateTime;
var SearchHistoryTimeRange;

function searchHistoryAddress(lon, lat, searchDateTime, radius, minutes, mapSearchPointSets) {
    SearchHistoryDateTime = searchDateTime;
    SearchHistoryTimeRange = minutes;
    searchingMask.show();
    tabs.add(historyAddressGrid);
    tabs.setActiveTab(historyAddressGrid);
    historyAddressStore.load({
        params:
        {
            lon: lon,
            lat: lat,
            SearchHistoryDateTime: SearchHistoryDateTime,
            SearchHistoryTimeRange: SearchHistoryTimeRange,
            radius: radius,
            mapSearchPointSets: mapSearchPointSets
        }
    });
}

function getValueByKey(key, s) {
    //s = s.toLowerCase();
    //key = key.toLowerCase() + '=';
    key = key + '=';

    var n = s.indexOf(key);
    var m;

    if (n < 0)
        return '';

    if (n == 0) {
        m = s.indexOf(";");
        return s.substring(n + key.length, m);
    }

    n = s.indexOf(";" + key);
    m = s.indexOf(";", n + 1);
    return s.substring(n + key.length + 1, m);

}

function getValueByKeyWithSeperatorEqualSign(key, s, separator, equalSign) {
    key = key + equalSign;

    var n = s.indexOf(key);
    var m;

    if (n < 0)
        return '';

    if (n == 0) {
        m = s.indexOf(separator);
        return s.substring(n + key.length, m);
    }

    n = s.indexOf(separator + key);
    m = s.indexOf(separator, n + 1);
    return s.substring(n + key.length + 1, m);
}

function getControllerTypeById(controllerTypeId) {
    var c = 'N/A';

    if (controllerTypeId == '')
        return c;
    
    switch (controllerTypeId * 1) {
        case 0:
            c = 'Invalid';
            break;
        case 1:
            c = 'MP4';
            break;
        case 2:
            c = 'MP5';
            break;
        case 3:
            c = 'MP6';
            break;
        case 4:
            c = 'TG6';
            break;
        case 5:
            c = 'TTMT';
            break;
        case 6:
            c = 'DAS';
            break;
        case 7:
            c = 'TCI';
            break;
        case 8:
            c = 'MPT';
            break;
        case 9:
            c = 'SR2';
            break;
        case 10:
            c = 'N/A';
            break;
        case 11:
            c = 'SR2 M/T';
            break;
        case 12:
            c = 'SR2 Truck';
            break;
        case 13:
            c = 'SR2 Truck M/T';
            break;
        case 14:
            c = 'SR3';
            break;
        case 15:
            c = 'SR3 MT';
            break;
        case 16:
            c = 'SR3 ST Truck';
            break;
        case 17:
            c = 'SR3 MT Truck';
            break;
        case 18:
            c = 'DAS IV';
            break;
        case 19:
            c = 'SR4 ST';
            break;
        case 20:
            c = 'SR4 MT';
            break;
        case 21:
            c = 'SR4 ST Truck';
            break;
        case 22:
            c = 'SR4 MT Truck';
            break;
        case 23:
            c = 'Cryo Trailer';
            break;
        case 24:
            c = 'Cryo Truck';
            break;
        default:
            c = 'N/A';
            break;

    }
    return c;
}

function GetTetherOnOff(value) {
    var ps = getValueByKey('Power', value);
    if (ps.toLowerCase() == 'sleepmode') {
        return 'Off';
    }
    ps = getValueByKey('RD_STA', value);
    var returnvalue = 'Off';
    if (ps != '') {
        if (((ps & 16) >> 4) == 1)
            returnvalue = 'On';
    }
    return returnvalue;
}

function GetReeferOnOff(record) {
    var sensorMask = record.data['SensorMask'];
    //alert(sensorMask);
    if (record.data['VehicleStatus'] == 'Ignition Off')
        return 'Off';
    if (sensorMask == '')
        return 'Off';
    if((sensorMask & 4) >> 2 == 1)
        return "On";
    else
        return "Off";
}
