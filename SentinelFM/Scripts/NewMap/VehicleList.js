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
var historyMessageStore;
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
var historyVehicles;

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

var mapStatus = hideMapByDefault ? 'collapse' : 'expanded';
var mapGridOriginalHeight;
var mapGridOriginalWidth;

var loadingMask;
var searchingMask;

var btnVehicleonoff;
var btnHistoryReplay;

var replayPanel;
var historyReplaySlider;
var btnHistoryPlayPlay;
var btnHistoryPlayPause;
var txtHistoryReplayDelayTime;

var historyGraphWindow;
var dataHistoryGraphSpeed = [];
var dataHistoryGraphRPM = [];
var dataHistoryGraphRoadSpeed = [];
var selModel;
var slectedboxidarray = [];
var allGridColumns = { BoxId: 1, Description: 2, StreetAddress: 3, OriginDateTime: 4, Speed: 5, BoxArmed: 6, VehicleStatus: 7, PTO: 8, History: 9, VehicleId: 10, LicensePlate: 11, Latitude: 12, Longitude: 13, IconTypeName: 14, MyHeading: 15, icon: 16, Driver: 17, ImagePath: 18, ConfiguredNum: 19, DriverCardNumber: 20, Field1: 21, Field2: 22, Field3: 23, Field4: 24, Field5: 25, ModelYear: 26, MakeName: 27, ModelName: 28, VehicleTypeName: 29, VinNum: 30, ManagerName: 31, ManagerEmployeeId: 32, StateProvince: 33, Color: 34, EngineHours: 35, Odometer: 36, RouteAssigned: 37, DateTimeReceived: 38, DclId: 39, BoxMsgInTypeId: 40, BoxMsgInTypeName: 41, BoxProtocolTypeId: 42, BoxProtocolTypeName: 43, CommInfo1: 44, CommInfo2: 45, ValidGps: 46, Heading: 47, SensorMask: 48, CustomProp: 49, BlobDataSize: 50, SequenceNum: 51, Acknowledged: 52, Scheduled: 53, MsgDetails: 54, MyDateTime: 55, dgKey: 56, CustomUrl: 57, HistoryInfoId: 58, chkBoxShow: 59, CP_Fuel: 60, CP_Odometer: 61, CP_RPM: 62, CP_FLIP: 63, CP_FLIS: 64, CP_SeatBelt: 65, CP_MIL: 66, CP_CLT: 67, CP_EOT: 68, CP_EOP: 69, RPM: 70, DriverHIDCard: 71, LastIgnOnBatV: 72, LastIgnOffBatV: 73, CustomConfig: 74, NearestLandmark: 75, SAP_number: 76 };
var SaveGridSizeToCookie = false;
var cookiedefaultmapview;

var VehicleGridInSearchMode = false;

var selectionon = false;
var originSelectionon = false;

var vehiclerunner;
var alarmrunner;
var messagerunner;
var delayrunner;

var ExtraVehicleForLandmark;

var fromAutoSync = false;
var vehicleAutoSyncStopped = false;
var vehicleTimeOutId;

var currentpage;

Ext.define('Override.menu.Menu', {
    override: 'Ext.menu.Menu',

    compatibility: '4',

    onMouseLeave: function (e) {
        var me = this;

        // If the mouseleave was into the active submenu, do not dismiss
        if (me.activeChild) {
            if (e.within(me.activeChild.el, true)) {
                return;
            }
        }
        me.deactivateActiveItem();
        if (me.disabled) {
            return;
        }
        me.fireEvent('mouseleave', me, e);
    }
});

//Ext.create('Ext.toolbar.Toolbar', {
//    renderTo: Ext.getBody(),
//    items: [{
//        text: 'Button w/ Menu',
//        menu: [{
//            xtype: 'combobox',
//            hideLabel: true,
//            store: ['Arizona'],
//            typeAhead: true,
//            queryMode: 'local',
//            triggerAction: 'all',
//            emptyText: 'Select a state...',
//            selectOnFocus: true,
//            width: 135,
//            iconCls: 'no-icon'
//        }, // A Field in a Menu
//        {
//            text: 'I like Ext',
//            checked: true
//        }, '-', {
//            text: 'Radio Options',
//            menu: { // <-- submenu by nested config object
//                items: [
//                // stick any markup in a menu
//                '<b class="menu-title">Choose a Theme</b>', {
//                    text: 'Aero Glass',
//                    checked: true,
//                    group: 'theme'
//                }, {
//                    text: 'Vista Black',
//                    checked: false,
//                    group: 'theme'
//                }, {
//                    text: 'Gray Theme',
//                    checked: false,
//                    group: 'theme'
//                }, {
//                    text: 'Default Theme',
//                    checked: false,
//                    group: 'theme'
//                }]
//            }
//        }, {
//            text: 'Choose a Date',
//            iconCls: 'calendar',
//            menu: [{
//                xtype: 'datepicker'
//            }]
//        }, {
//            text: 'Choose a Color',
//            menu: [, {
//                xtype: 'colorpicker'
//            }]
//        }]
//    }, {
//        text: 'Users',
//        menu: {
//            xtype: 'menu',
//            plain: true,
//            items: {
//                xtype: 'buttongroup',
//                title: 'User options',
//                columns: 2,
//                defaults: {
//                    xtype: 'button',
//                    scale: 'large',
//                    iconAlign: 'left'
//                },
//                items: [{
//                    text: 'User<br/>manager',
//                    width: 90,
//                    displayText: 'User manager'
//                }, {
//                    tooltip: 'Add user',
//                    width: 40,
//                    displayText: 'Add user'
//                }, {
//                    colspan: 2,
//                    text: 'Import',
//                    scale: 'small',
//                    width: 130
//                }, {
//                    colspan: 2,
//                    text: 'Who is online?',
//                    scale: 'small',
//                    width: 130
//                }]
//            }
//        }
//    }]
//});

var baseColumns = [];
var defaultViewColumns = [];
var dashboardViewColumns = [];
var vehicleGridColumns = [];

var currentView = 'default';

var landmarkCategory;
var availabilityChartBtn;

var recordUpdater;

var col_vgSpeed = Ext.create('Ext.grid.column.Column', {
    id: 'vgSpeed',
    text: ResvgSpeedText, //'Speed',
    align: 'left',
    width: 50,
    dataIndex: 'CustomSpeed',
    filterable: true,
    sortable: true,
    hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Speed) - 1] == "y") ? false : true
});

Ext.override(Ext.grid.View, { enableTextSelection: true });
Ext.override(Ext.menu.Menu, {
    hideMode: 'display'
});

Ext.Loader.setConfig(
{
    enabled: true
	, disableCaching: false
}
);
Ext.Loader.setPath('Ext.ux', './extjs/examples/ux');
Ext.Loader.setPath('Ext.ux.exporter', './sencha/Ext.ux.Exporter'); // Only the Ext.ux.exporter.* classes will be searched in ./something/exporter'
//Ext.Loader.setPath('Ext.ux.container', './sencha/Ext.ux.container');
Ext.require([
'Ext.window.*',
'Ext.ux.grid.FiltersFeature',
'Ext.ux.AspWebAjaxProxy',
'Ext.ux.form.MultiSelect',
'Ext.ux.exporter.Exporter.*',
	 'Ext.ux.exporter.excelFormatter.*',
	 'Ext.ux.exporter.csvFormatter.*',
	 'Ext.ux.exporter.Button.*'
     //,'Ext.ux.container.ButtonSegment.*'
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
      'StreetAddress', 'LatLon',
      {
          name: 'OriginDateTime', type: 'date', dateFormat: 'c'
      }
      ,
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
      'Longitude',
      'IconTypeName',
       {
          name: 'CustomSpeed', type: 'int'
      },
      'MyHeading',
      'icon',
      'Driver',
      'ImagePath',
      'ConfiguredNum',
      'DriverCardNumber',
     'Field1',
     'Field2',
     'Field3',
     'Field4',
     'Field5',
     {
         name: 'ModelYear', type: 'int'
     },
     'MakeName',
     'ModelName',
     'VehicleTypeName',
     'VinNum',
     'ManagerName',
     'ManagerEmployeeId',
     'StateProvince',
     'Color',
     'EngineHours',
     'Odometer',
     { name: 'LastIgnOnBatV', type: 'float' },
     { name: 'LastIgnOffBatV', type: 'float' },
     { name: 'LastBatV', type: 'float' },
      {
          //name: 'IsRouteAssigned', convert: function (value, record) { if (record.get('ConfiguredNum') * 1 == 0) return 'No'; else return 'Yes'; }
          name: 'IsRouteAssigned', convert: function (value, record) { if (record.get('ConfiguredNum') * 1 == 0) return ResRouteAssignedNo; else return ResRouteAssignedYes; }
      },
     { name: 'isSelected', type: 'int', convert: function (value, record) { return 0; } },
     'OperationalState',
     'OperationalStateName',
     'OperationalStateNotes',
     { name: 'DurationInLandmarkMin', type: 'int' },
     { name: 'LandmarkID', type: 'int' },
     'LandmarkName'
     , 'CustomConfig'
     , 'VehicleDeviceStatusID'
     , 'NearestLandmark'
     , 'SAP_number'
     , {
         name: 'LandmarkInDateTime', type: 'date', dateFormat: 'c'
     }
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
      'VehicleId', 'LicensePlate', 'Description',
      {
          name: 'DateTimeReceived', type: 'date', dateFormat: 'c'
      }
      , 'DclId', 'BoxMsgInTypeId', 'BoxMsgInTypeName', 'BoxProtocolTypeId',
      'BoxProtocolTypeName', 'CommInfo1', 'CommInfo2', 'ValidGps', 'Latitude', 'Longitude', 'Heading', 'SensorMask', 'CustomProp', 'BlobDataSize', 'SequenceNum',
      'StreetAddress',
       'Speed',
      'BoxArmed', 'MsgDirection', 'Acknowledged', 'Scheduled', 'MsgDetails', 'MyDateTime', 'MyHeading', 'dgKey', 'CustomUrl', 'HistoryInfoId', 'chkBoxShow',
      {
          name: 'OriginDateTime', type: 'date', dateFormat: 'c'
      }
      , 'icon', 'CP_Fuel', 'CP_Odometer', 'CP_RPM', 'CP_FLIP', 'CP_FLIS', 'CP_SeatBelt', 'CP_MIL', 'CP_CLT', 'CP_EOT', 'CP_EOP',
      'RPM',
      'RoadSpeed',
      'Driver',
      'DriverHIDCard',
      'PTO',
      { name: 'custSpeed', type: 'int' },
      'TripColor',
      {
          name: 'TimeDifference', type: 'int'
      }
      , 'LatLon'
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
          {
              name: 'OriginDateTime', type: 'date', dateFormat: 'c'
          },
          'VehicleId', 'LicensePlate', 'Description', 'FleetId'
       ]
   }
   );

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
                name: 'ArrivalDateTime', type: 'date', dateFormat: userdateformat
                , defaultValue: '', convert: function (value, record) {
                    var dt = Ext.Date.parse(value, 'n/j/Y g:i:s A'); //12/21/2012 5:55:51 PM
                    return dt;
                }
            }
             ,
            'Location',
            {
                name: 'DepartureDateTime', type: 'date', dateFormat: userdateformat,
                defaultValue: '', convert: function (value, record) {
                    var dt = Ext.Date.parse(value, 'n/j/Y g:i:s A'); //12/21/2012 5:55:51 PM
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
            'Driver',
            'DriverHIDCard',
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
            'Driver',
            'DriverHIDCard'
       ]
   }
   );


Ext.define('DashboardModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'VehicleID' },
        { name: 'OperationalStateName', type: 'string' },
        { name: 'Notes', type: 'string' },
        { name: 'TimeInLandmark', type: 'int' },
        { name: 'DurationInLandmarkMin', type: 'int' },
        { name: 'LandmarkInDateTime', type: 'date', dateFormat: 'c' }

    ]
});

var testStore = Ext.create('Ext.data.Store', {
    model: 'DashboardModel'//,
    , autoLoad: false
    , listeners:
    {
        'load': function (store, records, options) {
            try {
                vehiclegrid.getView().refresh();
            }
            catch (err) {
            }

        },
        scope: this
    }
    , proxy:
    {
        type: 'ajax',
        url: 'Vehicles.aspx?QueryType=ListVehiclesInLandmarksForDashboard',
        timeout: 120000,
        reader:
        {
            type: 'xml',
            root: 'Vehicle',
            record: 'VehicleList'
        }
    }
});



Ext.onReady(function () {
    Ext.QuickTips.init();

    // setup the state provider, all state information will be saved to a cookie
    Ext.state.Manager.setProvider(new Ext.state.CookieProvider({
        expires: new Date(new Date().getTime() + (1000 * 60 * 60 * 24 * 30)) // 30 days
    }));

    var selectedVehIds = "";
    var lastProxyFinished = false;
    var mapLoading = false;
    var currentSelected = "";
    var wincounter = 0;
    var proxyTimeOut = 120000;
    var fleetDefaultText = ResfleetDefaultText; //'All Vehicles...';
    var taskRunning = false;
    var mapHTML = '<iframe scrolling="no" src="./MapNew/OpenLayerMaps.aspx"';
    var mapStyle = 'style="Height:100%; width:100%;  border:0;margin:0px"';

    var nframeadded = false, sframeadded = false, eframeadded = false;
    loadingMask = new Ext.LoadMask(Ext.getBody(), {
        msg: ResloadingMaskMessage//"Loading..."
    });
    searchingMask = new Ext.LoadMask(Ext.getBody(), {
        msg: RessearchingMaskMessage//"Searching..."
    });
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
    var stopVehicleSync = function() {
        vehicleAutoSyncStopped = true;
        if (vehicleTimeOutId) {
            clearTimeout(vehicleTimeOutId);
        }
    }

    var currentState = LoadStates.FleetsLoading;
    var activetab = ActiveTabs.Vehicles;
    var dateformat = userdateformat;
    var initialData = "";
    var soundPresent = false;
    var statusColorString = '#00C000';

    var sensorPage = './Map/frmSensorMain.aspx?LicensePlate=';
    var historyPage = './History/frmhistmain_new.aspx?VehicleId=';

    var template = '<span style="color:{0};">{1}</span>';

    //Added by Rohit for selection of Units
    var paged = false;

    selModel = Ext.create('Ext.selection.CheckboxModel',
{
    checkOnly: true,
    enableKeyNav: false,
    listeners:
   {
       selectionchange: function (selModel, selections) {
           try {
               //vehiclerunner.stopAll();
               stopVehicleSync();

               alarmrunner.stopAll();
               messagerunner.stopAll();

               vehiclegrid.down('#trackitmenu').setDisabled(selectedVehicleBoxId < 0);
               vehiclegrid.down('#streetViewMenu').setDisabled(selectedVehicleBoxId < 0);
               vehiclegrid.down('#updatePositionMenu').setDisabled(selections.length == 0);
               vehiclegrid.down('#clearAllMenu').setDisabled(selections.length == 0);
               if (ShowDashboardView) {
                   vehiclegrid.down('#vehicleStatusUpdate').setDisabled(selections.length == 0);
               }

               var checkedHd = this.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
               if (checkedHd)
                   selectionon = true;
               else
                   selectionon = false;

               if (selectionon) {
                   slectedboxidarray.length = 0;
               }

               mainstore.each(function (record, idx) {
                   record.set('isSelected', 0);
               });

               if (selections.length > 0 && !selectionon) {
                   Ext.each(selections, function (record) {
                       if (slectedboxidarray.indexOf(record.data.BoxId) == -1)
                           slectedboxidarray.push(record.data.BoxId);
                       var tests = mainstore.findExact('BoxId', record.data.BoxId, 0);
                       if (tests != -1) {
                           mainstore.getAt(tests).data.isSelected = 1;
                       }
                   });
                   if (!reselect) {
                       mainstore.each(function (record, idx) {
                           if (record.data.isSelected != 1 && slectedboxidarray.indexOf(record.data.BoxId) > -1) {
                               slectedboxidarray.splice(slectedboxidarray.indexOf(record.data.BoxId), 1);
                           }
                       });
                   }
               }

               if (selections.length == 0 && !paged) {
                   mainstore.each(function (record, idx) {
                       if (slectedboxidarray.length > 0 && slectedboxidarray.indexOf(record.data.BoxId) > -1)
                           slectedboxidarray.splice(slectedboxidarray.indexOf(record.data.BoxId), 1);
                   });
               }



               //if (!firstLoad) {
               var zoommap = firstLoad;
               var allvehicleonmap = false;

               try {
                   //mapSelections(selections, zoommap);
                   if ((!paged || currentpage == 1) && selections.length > 0) {
                       if (selectionon && !VehicleGridInSearchMode) {
                           allvehicleonmap = true;
                           var postdata = "filters=";
                           for (var propt in currentFilters) {
                               postdata += currentFilters[propt] + ",";
                           }
                           //alert(fromAutoSync);
                           $.ajax({
                               type: 'GET',
                               url: 'Vehicles.aspx?QueryType=GetAllFleetForMap&data=' + (fromAutoSync ? 'new' : 'all') + '&_dc=' + (new Date()).getTime(),
                               dataType: 'json',
                               data: postdata,
                               async: false,
                               success: function (msg) {
                                   var records = [];
                                   if (msg.Fleet.VehiclesLastKnownPositionInformation.length != undefined)
                                       records = msg.Fleet.VehiclesLastKnownPositionInformation;
                                   else
                                       records.push(msg.Fleet.VehiclesLastKnownPositionInformation);

                                   if (!fromAutoSync) {
                                       var el = document.getElementById(mapframe).contentWindow;
                                       if (typeof el.allVehicles != "undefined")
                                           el.allVehicles.length = 0;
                                       if (el.markers != undefined) {
                                           el.markers.removeAllFeatures();
                                           if (el.parent.VehicleClustering) {
                                               el.markerstrategy.clearCache();
                                           }
                                       }
                                       if (el.vehicleFeatures != undefined)
                                           el.vehicleFeatures = [];
                                   }
                                   if (records.length > 0) {
                                       if (fromAutoSync)
                                           mapVehicles(true, records, false, false, false);
                                       else if (reselect == true)
                                           mapVehicles(true, records, true, false, false);
                                       else
                                           mapVehicles(true, records, true, false, true);
                                   }
                                   fromAutoSync = false;
                                   delayrunner.delay(parseInt(vehinterval));

                               },
                               error: function (msg) {

                               }
                           });
                       }
                       else {
                           if (reselect == true)
                               mapSelections(selections, false);
                           else
                               mapSelections(selections, true);
                       }
                   }
                   else if (!fromAutoSync && selections.length == 0) {
                       mapSelections(selections, true);
                   }
                   else if (!paged && selections.length == 0) {
                       mapSelections(selections, true);
                   }
                   else if (paged && selections.length == 0) {
                   }
                   else if (paged && selections.length > 0 && !selectionon) {
                       if (reselect == true)
                           mapSelections(selections, false);
                       else
                           mapSelections(selections, true);
                   }
                   if (reselect == true && selections.length == 0 && paged && (!selectionon || !fromAutoSync))
                       mapSelections(selections, false);
                   if (!(selections.length == 0 && paged)) {
                       paged = false;
                   }
                   reselect = false;
                   ////////////////////////////////////////////////////////////////////////////
               }
               catch (err) { alert(err); }
               //}
               firstLoad = false;

               //vehiclegrid.getView().refresh();
               if (!allvehicleonmap) {
                   delayrunner.delay(parseInt(vehinterval));
               }
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
                    if (!ShowRetiredVehicles && exirecord.data.VehicleDeviceStatusID == "3")
                        return;
                    if (exirecord.data.Latitude == 0 || exirecord.data.Latitude == 90 || exirecord.data.Latitude == -90 || exirecord.data.Longitude == 0 || exirecord.data.Longitude == 90 || exirecord.data.Longitude == -90)
                        return;

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
                        if (exirecord.data.PTO != undefined && exirecord.data.PTO == 'On') {
                            newIcon = "Blue" + IconTypeName + ".ico";;
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
                title: ResUpdatePositionCommandStatus, //'UpdatePosition Command Status',
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
    text: ResmapitButtonText, //'FindIt',
    id: 'mapitButton',
    //    renderTo : Ext.getBody(),
    tooltip: ResMapItButMapTheSelectedVehicle, //'Map the selected vehicle',
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

    var batteryTrendingBtn = BatteryTredingEnabled ? Ext.create('Ext.Button',
{
    text: ResBatteryTrendingBtnText, //'Battery',
    id: 'batteryTrendingBtn',
    tooltip: ResBatteryTrendingBtnText, //'Battery',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    hidden: false,
    handler: function () {
        try {
            var selFleet = (LoadVehiclesBasedOn == 'fleet') ? SelectedFleetId : DefaultOrganizationHierarchyFleetId;
            var mypage = './BatteryTrending/Index.aspx?fleetId=' + selFleet;

            var myname = ResBatteryTrendingBtnText; //'Battery';
            myname = '';
            var w = 900;
            var h = 555;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1';
            win = window.open(mypage, myname, winprops);
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }

        }
        catch (err) {
        }
    }
}
) : null;

    var updatePosition = Ext.create('Ext.Button',
{
    text: ResupdatePositionButtonText, //'Update Position',
    id: 'updatePositionButton',
    tooltip: ResUpdatePositionButtonToolTip, //'Map selected vehicle on map',
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
                url: './Vehicles.aspx/UpdatePosition',
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
    text: ResFeedbackButtonText, //'Feedback',
    id: 'feedbackButton',
    tooltip: ResWantToImproveOurMap, //'Want to improve our map please provide feedback..',
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
    text: ResClearAllButtonText, //'ClearAll',
    id: 'clearAllButton',
    tooltip: ResClearAllButtonToolTip, //'Clear all selected vehicles',
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
    text: ResFindVehiclesDriversButtonText, //'Find Vehicles/Drivers',
    id: 'findVehiclesDriversButton',
    tooltip: ResFindVehiclesDrivers, //'Find Vehicles/Drivers',
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
    text: ResLegendButtonText, //'Legend',
    id: 'legendButton',
    tooltip: ResLegendDateTimeColor, // 'Legend of Date/Time Color',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            if (!vehicleLegendWin) {
                var legendURL = "./Legend.aspx";
                var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + legendURL + '"></iframe>';
                vehicleLegendWin = openWindow(ResbtnHistoryLegendText/*'Map Legend'*/, urlToLoad, 400, 220);
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
    tooltip: ResOrganizationHierarchy, //'Organization Hierarchy',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            onOrganizationHierarchyNodeCodeClick();
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
    tooltip: ResSelectFleet, //'Select a fleet',
    cls: 'cmbfonts',
    icon: 'preview.png',
    textAlign: 'left',
    handler: function () {
        try {
            var url = "./Widgets/fleet.aspx?fleetId=" + SelectedFleetId + '&f=fleetButton';
            var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
            //if (fleetWin==undefined)
            fleetWin = openWindow(ResfleetButtonOpenwindowMessage, urlToLoad, 400, 150); //openWindow('Select a fleet', urlToLoad, 400, 150);
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
    tooltip: ResHistoryOrganizationHierarchyButtonToolTip, //'Organization Hierarchy',
    cls: 'cmbfonts',
    textAlign: 'left',
    width: 230,
    style: { marginLeft: '1px' },
    handler: function () {
        try {
            var mypage = '../../Widgets/OrganizationHierarchy.aspx?nodecode=' + HistoryOrganizationHierarchyNodeCode;
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0&rootNodecode=" + PreferOrganizationHierarchyNodeCode;
            }
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
    tooltip: ResHistoryFleetButtonSelectfleet, //'Select a fleet',
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
            fleetWin = openWindow(ReshistoryFleetButtonOpenwindowMessage, urlToLoad, 400, 150); //openWindow('Select a fleet', urlToLoad, 400, 150);
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
    text: ifShowClusteredVehicleLabel ? ReslabelonoffButtonlabelonoffHideLabel : ReslabelonoffButtonlabelonoffShowLabel, //'Hide Label' : 'Show Label',
    id: 'labelonoffButton',
    tooltip: ResLabelonoffButtonHideShowLabel, //'Hide/Show Label',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            if (ifShowClusteredVehicleLabel) {
                ifShowClusteredVehicleLabel = false;
                labelonoff.setText(ReslabelonoffButtonlabelonoffShowLabel); //labelonoff.setText('Show Label');
            }
            else {
                ifShowClusteredVehicleLabel = true;
                labelonoff.setText(ReslabelonoffButtonlabelonoffHideLabel); //labelonoff.setText('Hide Label');
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
    text: ResClearSearchBtnText, //'Clear Search',
    id: 'clearSearchBtn',
    tooltip: ResClearSearchBtnHideShowLabel, //'Hide/Show Label',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    hidden: true,
    handler: function () {
        try {
            if (!originSelectionon && selectionon)
                vehiclegrid.getSelectionModel().deselectAll(true);
            else if (originSelectionon && !selectionon)
                vehiclegrid.getSelectionModel().selectAll(true);

            var fleetId;
            if (LoadVehiclesBasedOn == 'fleet') {
                fleetId = DefaultFleetID;
            }
            else {
                fleetId = DefaultOrganizationHierarchyFleetId;
            }

            loadingMask.show();

            VehicleGridInSearchMode = false;

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
    text: ResSendmessageButtonText, //'Send Message',
    id: 'sendmessageButton',
    tooltip: ResSendmessageButtonSendMessage, //'Send Message',
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

    var allRoutesBtn = Ext.create('Ext.Button',
{
    text: ResAllRoutesBtnText, //'All Routes',
    id: 'allRoutesBtn',
    tooltip: ResAllRoutesBtnText, //'All Routes',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    hidden: false,
    handler: function () {
        try {
            var mypage = './ServiceAssignment/AllRoutes.aspx?objectname=&routeName=&service=route';
            //var mypage='http://preprod.sentinelfm.com/ServiceAssignment/AssignmentForm.aspx?vehicleName=' + encodeURIComponent(vehicleDescription.replace(/&singlequote;/g, "\'")) + '&routeName=';
            var myname = ResAllRoutesBtnText; //'All Routes';
            myname = '';
            var w = 825;
            var h = 520;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }

        }
        catch (err) {
        }
    }
}
);

    //var landmarkCategory_values = [
    //        [0, ResHistoryTypeValues_0 /*'Vehicle Path'*/],
    //        [1, ResHistoryTypeValues_1/*'Stop and Idle Sequence'*/],
    //        [2, ResHistoryTypeValues_2/*'Stop Sequence'*/],
    //        [3, ResHistoryTypeValues_3/*'Idle Sequence'*/],
    //        [4, ResHistoryTypeValues_4/*'Trip Report'*/]
    //];

    var landmarkCategory_store = new Ext.data.SimpleStore({
        fields: ['CategoryName', 'CategoryId'],
        data: CategoryList
    });


    landmarkCategory = new Ext.form.ComboBox({
        name: 'landmarkCategory_values',
        fieldLabel: 'Landmark Category', //'Type',
        //hiddenName: 'historyType',
        store: landmarkCategory_store,
        displayField: 'CategoryName',
        valueField: 'CategoryId',
        value: 0,
        labelWidth: 110,
        width: 300,
        typeAhead: true,
        mode: 'local',
        triggerAction: 'all',
        emptyText: '', //'Choose number...',
        selectOnFocus: true,
        editable: false,
        hidden: true,
        listeners:
          {
              scope: this,
              'select': function (combo) {
                  var cat_id = combo.getValue();
                  if (cat_id == 0)
                      return;
                  //alert(combo.getValue());
                  // testStore.load(
                  //{
                  //    params:
                  //    {
                  //        landmarkCategoryId: cat_id
                  //    }
                  //});

                  if (!VehicleGridInSearchMode) {
                      if (!paged || currentpage == 1) {
                          if (selectionon && !VehicleGridInSearchMode) {
                              fromAutoSync = true;
                              //alert('fromAutoSync:' + fromAutoSync);
                          }
                      }

                      currentState = LoadStates.GettingUpdates;
                      recordUpdater.removeAll(true);
                      loadingMask.show();
                      recordUpdater.load({
                          params:
                        {
                            QueryType: 'GetVehiclePosition',
                            filters: currentFilters,
                            sorting: sortingParam,
                            start: (currentpage - 1) * VehicleListPagesize,
                            limit: VehicleListPagesize,
                            mergeData: 'VehiclesInLandmarks',
                            landmarkCategoryId: cat_id
                        }
                      });
                  }
              }
          }
    });

    var availabilityChartBtn = Ext.create('Ext.Button',
{
    text: 'Availability Chart',
    id: 'availabilityChartBtn',
    tooltip: 'Availability Chart',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    hidden: true,
    handler: function () {
        try {
            var selFleet = (LoadVehiclesBasedOn == 'fleet') ? SelectedFleetId : DefaultOrganizationHierarchyFleetId;
            var mypage = './MapNew/frmVehicleAvailabilityDashboard.aspx?FleetId=' + selFleet + '&LandmarkCategoryId=' + landmarkCategory.getValue();
            var myname = 'Availability Chart';
            myname = '';
            var w = 925;
            var h = 620;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }

        }
        catch (err) {
        }
    }
}
);



    var defaultViewBtn = Ext.create('Ext.Button',
{
    text: 'Default View',
    id: 'defaultViewBtn',
    tooltip: '',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    //disabled: true,
    allowDepress: false,
    pressed: true,
    toggleGroup: 'vehiclegridViewGroup',
    handler: function () {
        try {
            //vehiclegrid.removeColumn('Position');
            ShowDefaultView();
        }
        catch (err) {
            alert(err);
        }
    }
}
);

    var dashboardBtn = Ext.create('Ext.Button',
{
    text: 'Dashboard',
    id: 'dashboardBtn',
    tooltip: '',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    allowDepress: false,
    pressed: false,
    toggleGroup: 'vehiclegridViewGroup',
    handler: function () {
        try {
            //vehiclegrid.removeColumn('Position');
            ShowDashboard();
        }
        catch (err) {
            alert(err);
        }
    }
}
);

    //var segmentbtn = Ext.create('Ext.ux.container.ButtonSegment', {
    //    style: 'margin-top:15px',
    //    activeItem: 1,
    //    defaults: {
    //        scale: 'medium'
    //    },
    //    items: [{
    //        text: 'Default View'
    //    }, {
    //        text: 'Dashboard'
    //    }],
    //    listeners:
    //	{
    //	    change: function (btn, item) {
    //	        alert(btn.text);
    //	    },
    //	    scope: this
    //	}
    //});

    function openWindow(wintitle, winURL, winWidth, winHeight) {
        return openGeneralPopupWindow(wintitle, winURL, winWidth, winHeight, 'hide');
    }

    function openGeneralPopupWindow(wintitle, winURL, winWidth, winHeight, closeAction, winId) {
        if (winId != undefined && Ext.getCmp(winId))
            return;

        var win = new Ext.Window(
        {
            id: winId == undefined ? null : winId,
            title: wintitle,
            width: winWidth,
            height: winHeight,
            layout: 'fit',
            maxWidth: window.screen.width,
            maxHeight: window.screen.height,
            maximizable: true,
            minimizable: false,
            //maximized: winId == 'winBulkVehicleStateUpdate' ? true : false,
            resizable: 'true',
            closable: true,
            border: false,
            html: winURL,
            closeAction: closeAction//'hide'
        });
        win.show();
        return win;
    }

    function openTrackItWindow(wintitle, winURL, winWidth, winHeight) {
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
       closeAction: 'destroy'
   }
   );
        win.show();
        return win;
    }

    var streetView = Ext.create('Ext.Button',
{
    text: ResStreetViewButtonText, //'Street view',
    id: 'streetViewButton',
    tooltip: ResStreetViewButtonGoogleStreetView, //'Google street view',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    disabled: true,
    handler: function () {
        try {
            var currentTicked = vehiclegrid.getSelectionModel().getSelection();
            if (currentTicked.length > 1) {
                Ext.MessageBox.alert(ResStreetviewAlertTitle, ResStreetviewAlertMessage); // Ext.MessageBox.alert('Streetview', ' Please select only 1 vehicle for street view.');
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
                openWindow(ResstreetViewButtonOpenwindowMessage, htmlNewWin, 1000, 480); //openWindow('Street view', htmlNewWin, 1000, 480);
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
    text: ResTrackitButtonText, //'TrackIt',
    id: 'trackitButton',
    //    renderTo : Ext.getBody(),
    tooltip: ResTrackitButtonToolTip, //'Track selected vehicle on separate map',
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
                /*if (selectedRec.data.ImagePath != "") {    // custom icon
                var bicon = selectedRec.data.ImagePath;
                if (selectedRec.data.OriginDateTime < posExpireDate) {
                newIcon = "Grey" + bicon;
                }
                else {
                if (selectedRec.data.CustomSpeed != 0) {
                newIcon = "Green" + bicon;
                }
                else {
                newIcon = "Red" + bicon;
                }
                }
                }
                else {
                if (selectedRec.data.OriginDateTime < posExpireDate) {
                newIcon = "Grey" + selectedRec.data.IconTypeName + ".ico";
                }
                else {
                if (selectedRec.data.CustomSpeed != 0) {
                newIcon = "Green" + selectedRec.data.IconTypeName + selectedRec.data.MyHeading + ".ico";
                }
                else {
                newIcon = "Red" + selectedRec.data.IconTypeName + ".ico";
                }
                }
                }
                selectedRec.data.icon = newIcon;*/
                selectedRec.data.icon = getIcon(selectedRec, posExpireDate);
                selectedBoxs.push(selectedRec.data);
            }
         );

            SetWinTrackData2(selectedBoxs);
            var winurl = "./OpenLayerMap.aspx?WinId=" + wincounter;
            var htmlNewWin = '<iframe scrolling="no" src="' + winurl + '" style="Height:100%; width:100%;  border:0;margin:0px"></iframe>';
            openWindow(RestrackitButtonOpenwindowMessage, htmlNewWin, 600, 480); //openWindow('Track Vehicles', htmlNewWin, 600, 480);
            wincounter++;
        }
        catch (err) {

        }
    }
}
);

    var demomap = Ext.create('Ext.Button',
{
    text: ResDemomapButtonText, //'Demo open layer map',
    id: 'demomapButton',
    tooltip: ResDemomapButtonToolTip, //'Demo open layer map without tracking vehicle',
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
             title: ResTrackVehicles, //'Track Vehicles',
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
    text: ResSearchMapButtonText, //'Search',
    id: 'searchMapButton',
    tooltip: ResSearch, //'Search',
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
       type: 'int',
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
   },
   {
       type: 'string',
       dataIndex: 'OperationalStateName'
   },
   {
       type: 'string',
       dataIndex: 'OperationalStateNotes'
   },
   {
       type: 'int',
       dataIndex: 'DurationInLandmarkMin'
   },
   {
       type: 'string',
       dataIndex: 'LandmarkName'
   },
   {
       type: 'date',
       dataIndex: 'LandmarkInDateTime'
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
               alarmgrid.setTitle(ResAlarms + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg blinking'>(" + newAlarms + ")</span></div>");
           else
               alarmgrid.setTitle(ResAlarms + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg'>(" + newAlarms + ")</span></div>");
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
               messagegrid.setTitle(ResMessages + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg blinking'>(" + unreadMsg + ")</span></div>");
           else
               messagegrid.setTitle(ResMessages + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg'>(" + unreadMsg + ")</span></div>");
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
    fields: ['type', 'name', 'f', 'desc', 'StreetAddress', 'Email', 'ContactPhoneNum', 'radius', 'CategoryName']
    ,
    autoLoad: false,
    listeners:
   {
       'datachanged': function (store) {
           store.totalCount = store.count();
           landmarksPager.onLoad();
       },
       scope: this
   },
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
                /*if (exirecord.data.ImagePath != "") {    // custom icon
                var bicon = exirecord.data.ImagePath;
                if (exirecord.data.OriginDateTime < posExpireDate) {
                newIcon = "Grey" + bicon;
                }
                else {
                if (exirecord.data.CustomSpeed != 0) {
                newIcon = "Green" + bicon;
                }
                else {
                newIcon = "Red" + bicon;
                }
                }
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

                exirecord.data.icon = newIcon;*/
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

    function vehicleAutoSync() {
        try {
            if (!mainstore.isLoading() && !recordUpdater.isLoading()) {
                if (IsSyncOn && !VehicleGridInSearchMode) {
                    //alert('paged:' + paged + ',currentpage:' + currentpage + ',selectionon:' + selectionon + ',VehicleGridInSearchMode:' + VehicleGridInSearchMode)
                    if (!paged || currentpage == 1) {
                        if (selectionon && !VehicleGridInSearchMode) {
                            fromAutoSync = true;
                            //alert('fromAutoSync:' + fromAutoSync);
                        }
                    }

                    currentState = LoadStates.GettingUpdates;
                    recordUpdater.removeAll(true);
                    var mergeData = '';
                    var cat_id = 0;
                    if (currentView == 'dashboard') {
                        cat_id = landmarkCategory.getValue();
                        mergeData = 'VehiclesInLandmarks';
                    }

                    recordUpdater.load({
                        params:
                        {
                            QueryType: 'GetVehiclePosition',
                            filters: currentFilters,
                            sorting: sortingParam,
                            start: (currentpage - 1) * VehicleListPagesize,
                            limit: VehicleListPagesize,
                            mergeData: mergeData,
                            landmarkCategoryId: cat_id
                        }
                    });
                }
            }
        } catch (err) {
        }

        if (!vehicleAutoSyncStopped) {
            vehicleTimeOutId = setTimeout(function () { vehicleAutoSync(); }, parseInt(vehinterval == '0' ? '60000' : vehinterval));
        }

    }

    // create the Data Store
    //Added by Rohit Mittal for sorting
    var sortingParam;
    mainstore = Ext.create('Ext.data.Store',
{
    model: 'VehicleList',
    pageSize: VehicleListPagesize,
    // buffered : true,
    // purgePageCount : 0,
    autoLoad: false,
    autosync: false,
    storeId: 'Vehicles',
    sortOnLoad: false,
    listeners:
   {
       'load': function (store, records, options) {
           try {
               if (store.getCount() > 0) {
                   currentState = LoadStates.GridLoading;
                   if (mapAssets || FromClosestVehicles) {
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
                       if (ShowMessageTab) messagesstore.load();
                       //vehiclerunner.start(vehicletask);
                       vehicleAutoSyncStopped = false;
                       vehicleAutoSync();
                       if (ShowAlarmTab) alarmrunner.start(alarmtask);
                       if (ShowMessageTab) messagerunner.start(messagetask);
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
                       if (ShowMessageTab) messagesstore.load();
                       //vehiclerunner.start(vehicletask);
                       vehicleAutoSyncStopped = false;
                       vehicleAutoSync();
                       if (ShowAlarmTab) alarmrunner.start(alarmtask);
                       if (ShowMessageTab) messagerunner.start(messagetask);
                       loadingMask.hide();
                   }
               }
               lastProxyFinished = true;

               //iniVehicleGridPopup();
               setTimeout(function () { iniVehicleGridPopup(); }, 2000)

           }
           catch (err) {
           }
           currentState = LoadStates.MainStoreLoaded;
       },

       scope: this
   },
    proxy:
   {
       type: 'ajax',
       url: 'Vehicles.aspx',
       timeout: proxyTimeOut,
       reader:
      {
          type: 'xml',
          root: 'Fleet',
          record: 'VehiclesLastKnownPositionInformation',
          totalProperty: 'totalCount'
      }
   },
    sorters: [
   {
       property: 'OriginDateTime',
       direction: 'DESC'
   }
    ],
    sort: function (sorters) {
        if (sorters != undefined && sorters.direction != undefined && sorters.property != undefined) {
            sortingParam = {};
            sortingParam.property = sorters.property;
            sortingParam.direction = sorters.direction;
        }
        else if (sortingParam == undefined || sortingParam.direction == undefined || sortingParam.property == undefined) {
            //sortingParam = { property: "OriginDateTime", direction: "DESC" };
            return;
        }
        this.sorters.clear();
        this.sorters.add(sortingParam);
        var startpa;
        if (currentpage != null)
            startpa = (currentpage - 1) * VehicleListPagesize;
        else
            startpa = 0;
        if (!firstLoad && !VehicleGridInSearchMode) {
            try {
                loadingMask.show();
                var checkedHd = vehiclegrid.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                paged = true;
                mainstore.load({
                    params:
                         {
                             QueryType: 'GetFilteredFleet',
                             filters: currentFilters,
                             sorting: sortingParam,
                             start: startpa,
                             limit: VehicleListPagesize
                         },
                    callback: function (records, operation, success) {
                        loadingMask.hide();
                        if (checkedHd) {
                            selModel.selectAll(true);
                        }
                        else {
                            var newRecToMap = new Array();
                            for (var i = 0; i < slectedboxidarray.length; i++) {
                                var tests = mainstore.findExact('BoxId', slectedboxidarray[i], 0);
                                if (tests != -1) {
                                    reselect = true;
                                    var record = mainstore.getAt(tests);
                                    record.data.isSelected = 1;
                                    vehiclegrid.getSelectionModel().select(tests, true);
                                    newRecToMap.push(mainstore.getAt(tests).data);
                                }
                            }
                            vehiclegrid.reconfigure(mainstore);
                            mapVehicles(true, newRecToMap, false, '');
                        }
                    },
                    scope: this
                });
            }
            catch (err) {
            }
        }
    }
});

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

    var hissortingParam;
    historyPageStore = Ext.create('Ext.data.Store',
{
    //buffered: true,
    pageSize: HistoryPagesize,
    storeId: 'historyPageStore',
    model: 'HistoryListModel',
    autoLoad: false,
    sortOnLoad: false,
    listeners:
   {
       'load': function (store, records, options) {
           try {
               /*mapHistories(records, true, false, false);
               historygrid.getSelectionModel().selectAll(false);
               $('#historiescount').html(records.length);*/
               //historygrid.getDockedItems('pagingtoolbar')[0].doRefresh();
               //historyPager.doRefresh();

               if (HGI && $('[rel=bootstrapHoverPopover]').length > 0)
                   setTimeout(function () { iniBootstrapHoverPopover(); }, 2000);
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
    },
    sort: function (sorters) {
        if (sorters != undefined && sorters.direction != undefined && sorters.property != undefined) {
            hissortingParam = {}
            hissortingParam.property = sorters.property;
            hissortingParam.direction = sorters.direction;
        }
        else if (hissortingParam == undefined || hissortingParam.direction == undefined || hissortingParam.property == undefined) {
            return;
        }
        if (hissortingParam.property == "Speed")
            hissortingParam.property = "custSpeed";
        var mod = hissortingParam.direction.toUpperCase() == "DESC" ? -1 : 1;
        this.sorters.clear();
        this.sorters.add(hissortingParam);
        this.doSort(function (a, b) {
            return;
        });
        var startpa;
        if (hiscurrentpage != null)
            startpa = (hiscurrentpage - 1) * HistoryPagesize;
        else
            startpa = 0;
        try {
            loadingMask.show();
            var tempStore = Ext.create('Ext.data.Store', {
                model: 'HistoryListModel',
                sortOnLoad: false,
                pageSize: HistoryPagesize,
                proxy: {
                    type: 'ajax',
                    url: './historynew/historyservices.aspx',
                    reader: {
                        type: 'xml',
                        root: 'MsgInHistory',
                        record: 'VehicleStatusHistory',
                        totalProperty: 'totalCount'
                    }
                }
            });
            tempStore.load({
                params:
                     {
                         st: 'GetFilteredRecord',
                         filters: hiscurrentFilters,
                         sorting: hissortingParam,
                         start: startpa,
                         limit: HistoryPagesize
                     },
                callback: function (records, operation, success) {
                    historygrid.getStore().loadRecords(records);
                    loadingMask.hide();
                },
                scope: this
            });
        }
        catch (err) {
            loadingMask.hide();
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
       url: 'Vehicles.aspx?QueryType=searchHistoryAddress',
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
    displayMsg: ResVehiclePagerDisplayMsg, //'Displaying vehicles {0} - {1} of {2}',
    emptyMsg: ResVehiclePagerEmptyMsg, //"No vehicles to display",
    listeners: {
        beforechange: function () {
            //Added by Rohit Mittal
            selModel.suspendEvents(true);
            slectedboxidarray.length = 0;
            var selFleet = (LoadVehiclesBasedOn == 'fleet') ? SelectedFleetId : DefaultOrganizationHierarchyFleetId;
            //mainstore.proxy.extraParams = { QueryType: 'GetfleetPosition', fleetID: selFleet };
            mainstore.proxy.extraParams = { QueryType: 'GetFilteredFleet', filters: currentFilters, sorting: sortingParam };

            loadingMask.show();
        },

        change: function (thisd, params) {
            loadingMask.hide();
            if (typeof (params) != 'undefined') {
                currentpage = params.currentPage;
            }
            //Added by Rohit Mittal for selection of Units
            if (selectionon) {
                paged = true;
                selModel.selectAll(true);
            }
            selModel.resumeEvents();
        }
    }
});

    var alarmsPager = new Ext.PagingToolbar(
{
    store: alarmsstore,
    displayInfo: true,
    displayMsg: ResAlarmsPagerdisplayMsg, //'Displaying alarms {0} - {1} of {2}',
    emptyMsg: ResAlarmsPageremptyMsg //"No alarms to display"// ,

}
);

    var messagesPager = new Ext.PagingToolbar(
{
    store: messagesstore,
    displayInfo: true,
    displayMsg: ResMessagesPagerdisplayMsg, //'Displaying messages {0} - {1} of {2}',
    emptyMsg: ResMessagesPageremptyMsg //"No messages to display"// ,

}
);

    var hiscurrentpage = null;
    var historyPager = new Ext.PagingToolbar(
{
    store: historyPageStore,
    displayInfo: true,
    displayMsg: ResHistoryPagerdisplayMsg, //'Displaying histories {0} - {1} of {2}',
    emptyMsg: ResHistoryPageremptyMsg, //"No histories to display",
    listeners: {
        beforechange: function (b, page, o) {
            try {
                loadingMask.show();
                var tempStore = Ext.create('Ext.data.Store', {
                    model: 'HistoryListModel',
                    pageSize: HistoryPagesize,
                    sortOnLoad: false,
                    proxy: {
                        type: 'ajax',
                        url: './historynew/historyservices.aspx',
                        reader: {
                            type: 'xml',
                            root: 'MsgInHistory',
                            record: 'VehicleStatusHistory',
                            totalProperty: 'totalCount'
                        }
                    }
                });
                tempStore.currentPage = page;
                tempStore.load({
                    params:
                         {
                             st: 'GetFilteredRecord',
                             filters: hiscurrentFilters,
                             sorting: hissortingParam,
                             start: (page - 1) * HistoryPagesize,
                             limit: HistoryPagesize
                         },
                    callback: function (records, operation, success) {
                        historygrid.getStore().loadRecords(records);
                        historygrid.down('pagingtoolbar').bindStore(tempStore);
                        historygrid.down('pagingtoolbar').onLoad();
                        loadingMask.hide();
                    },
                    scope: this
                });
            }
            catch (err) {
                loadingMask.hide();
            }
        },

        change: function (thisd, params) {
            //loadingMask.hide();
            if (typeof (params) != 'undefined') {
                hiscurrentpage = params.currentPage;
            }
        }
    }

}
);

    var reselect = false;
    recordUpdater = Ext.create('Ext.data.Store', {
        model: 'VehicleList',
        autoLoad: false,
        listeners: {
            'load': function (store, records, options) {
                if (mapAssets == true) {
                    mapAssets = false;
                    return;
                }
                if (records == null) {
                    return;
                }
                paged = true;
                mainstore.loadRecords(records);
                mainstore.totalCount = recordUpdater.totalCount;
                vehiclegrid.down('pagingtoolbar').bindStore(mainstore);
                vehiclegrid.down('pagingtoolbar').onLoad();
                var newRecToMap = new Array();
                var checkedHd = vehiclegrid.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                if (checkedHd) {
                    //reselect = true;
                    selModel.selectAll(true);
                }
                else {
                    var iniMap = false;
                    for (var i = 0; i < slectedboxidarray.length; i++) {
                        var tests = mainstore.findExact('BoxId', slectedboxidarray[i], 0);
                        if (tests != -1) {
                            reselect = true;
                            var record = mainstore.getAt(tests);
                            record.data.isSelected = 1;
                            vehiclegrid.getStore().remove(record);
                            vehiclegrid.getStore().insert(0, record);
                            vehiclegrid.getSelectionModel().select(0, true);
                            newRecToMap.push(mainstore.getAt(0).data);
                        }
                        else if (ExtraVehicleForLandmark && ExtraVehicleForLandmark.length) {
                            for (var j = 0; j < ExtraVehicleForLandmark.length; j++) {
                                if (slectedboxidarray[i] == ExtraVehicleForLandmark[j].BoxId * 1) {
                                    iniMap = true;
                                    ExtraVehicleForLandmark[j].BoxId = ExtraVehicleForLandmark[j].BoxId * 1;
                                    newRecToMap.push(ExtraVehicleForLandmark[j]);
                                }
                            }
                        }
                    }

                    vehiclegrid.reconfigure(mainstore);
                    mapVehicles(true, newRecToMap, iniMap, false, false);
                }
                vehiclegrid.down('pagingtoolbar').bindStore(vehiclegrid.getStore());
                vehiclegrid.down('pagingtoolbar').onLoad();
            }
        },
        proxy: {
            type: 'ajax',
            url: 'Vehicles.aspx',
            timeout: proxyTimeOut,
            reader: {
                type: 'xml',
                root: 'Fleet',
                record: 'VehiclesLastKnownPositionInformation',
                totalProperty: 'totalCount'
            }
        },
        sorters: [
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
    });

    //var vehicletask = {
    //    run: function () {
    //        try {
    //            if (!mainstore.isLoading() && !recordUpdater.isLoading()) {
    //                if (IsSyncOn && !VehicleGridInSearchMode) {
    //                    //alert('paged:' + paged + ',currentpage:' + currentpage + ',selectionon:' + selectionon + ',VehicleGridInSearchMode:' + VehicleGridInSearchMode)
    //                    if (!paged || currentpage == 1) {
    //                        if (selectionon && !VehicleGridInSearchMode) {
    //                            fromAutoSync = true;
    //                            //alert('fromAutoSync:' + fromAutoSync);
    //                        }
    //                    }

    //                    currentState = LoadStates.GettingUpdates;
    //                    recordUpdater.removeAll(true);
    //                    var mergeData = '';
    //                    var cat_id = 0;
    //                    if (currentView == 'dashboard') {
    //                        cat_id = landmarkCategory.getValue();
    //                        mergeData = 'VehiclesInLandmarks';
    //                    }

    //                    recordUpdater.load({
    //                        params:
    //                      {
    //                          QueryType: 'GetVehiclePosition',
    //                          filters: currentFilters,
    //                          sorting: sortingParam,
    //                          start: (currentpage - 1) * VehicleListPagesize,
    //                          limit: VehicleListPagesize,
    //                          mergeData: mergeData,
    //                          landmarkCategoryId: cat_id
    //                      }
    //                    });
    //                }
    //            }
    //        }
    //        catch (err) { }
    //    },
    //    interval: parseInt(vehinterval == '0' ? '60000' : vehinterval) // 5 second
    //}

    //function vehicleAutoSync() {
    //    try {
    //        if (!mainstore.isLoading() && !recordUpdater.isLoading()) {
    //            if (IsSyncOn && !VehicleGridInSearchMode) {
    //                //alert('paged:' + paged + ',currentpage:' + currentpage + ',selectionon:' + selectionon + ',VehicleGridInSearchMode:' + VehicleGridInSearchMode)
    //                if (!paged || currentpage == 1) {
    //                    if (selectionon && !VehicleGridInSearchMode) {
    //                        fromAutoSync = true;
    //                        //alert('fromAutoSync:' + fromAutoSync);
    //                    }
    //                }

    //                currentState = LoadStates.GettingUpdates;
    //                recordUpdater.removeAll(true);
    //                var mergeData = '';
    //                var cat_id = 0;
    //                if (currentView == 'dashboard') {
    //                    cat_id = landmarkCategory.getValue();
    //                    mergeData = 'VehiclesInLandmarks';
    //                }

    //                recordUpdater.load({
    //                    params:
    //                    {
    //                        QueryType: 'GetVehiclePosition',
    //                        filters: currentFilters,
    //                        sorting: sortingParam,
    //                        start: (currentpage - 1) * VehicleListPagesize,
    //                        limit: VehicleListPagesize,
    //                        mergeData: mergeData,
    //                        landmarkCategoryId: cat_id
    //                    }
    //                });
    //            }
    //        }
    //    } catch (err) {
    //    }

    //    if (!vehicleAutoSyncStopped) {
    //        setTimeout(function() { vehicleAutoSync(); }, parseInt(vehinterval == '0' ? '60000' : vehinterval));
    //    }

    //}

    vehiclerunner = new Ext.util.TaskRunner();

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
                           alarmgrid.setTitle(ResAlarms + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg blinking'>(" + newAlarms + ")</span></div>");
                       else
                           alarmgrid.setTitle(ResAlarms + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='alarmtabtitleunreadmsg'>(" + newAlarms + ")</span></div>");
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

    alarmrunner = new Ext.util.TaskRunner();


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
                           messagegrid.setTitle(ResMessages + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg blinking'>(" + unreadMsg + ")</span></div>");
                       else
                           messagegrid.setTitle(ResMessages + " <div style='display:inline-block;width:" + divwidth + "px;'><span class='messagetabtitleunreadmsg'>(" + unreadMsg + ")</span></div>");
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

    messagerunner = new Ext.util.TaskRunner();

    delayrunner = new Ext.util.DelayedTask(function () {
        //vehiclerunner.start(vehicletask);
        vehicleAutoSyncStopped = false;
        vehicleAutoSync();
        if (ShowAlarmTab) alarmrunner.start(alarmtask);
        if (ShowMessageTab) messagerunner.start(messagetask);
    });
    //////////////////////////////////////////////////////////////////

    function InitMainstoreData() {
        if (!OpenlayerMapsPageLoaded)
            setTimeout(function () { InitMainstoreData(); }, 100);
        else {
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
           InitMainstoreData();
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
        text: ResFinditmenuText, //'FindIt',
        disabled: true,
        tooltip: ResMaptheselectedvehicle, //'Map the selected vehicle',
        iconCls: 'map',
        cls: 'cmbfonts',
        handler: finditonmap
    }

    var trackitmenu =
    {
        text: ResTrackitmenuText, //'TrackIt',
        id: 'trackitmenu',
        tooltip: ResTrackSelectedVehicleOnSeparateMap, //'Track selected vehicle on separate map',
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
                    //if (selectedVehicleData.data.OriginDateTime < posExpireDate) {
                    //    newIcon = "Grey" + selectedVehicleData.data.IconTypeName + ".ico";
                    //}
                    //else {
                    //    if (selectedVehicleData.data.CustomSpeed != 0) {
                    //    newIcon = "Green" + selectedVehicleData.data.IconTypeName + selectedVehicleData.data.MyHeading + ".ico";
                    //}
                    //else {
                    //    newIcon = "Red" + selectedVehicleData.data.IconTypeName + ".ico";
                    //}
                    //}
                    //selectedVehicleData.icon = newIcon;//
                    newIcon = getIcon(selectedVehicleData, posExpireDate);
                    selectedVehicleData.data.icon = newIcon;

                    selectedBoxs.push(selectedVehicleData.data);
                }

                SetWinTrackData2(selectedBoxs);
                var winurl = "./OpenLayerMap.aspx?WinId=" + wincounter;
                var htmlNewWin = '<iframe scrolling="no" src="' + winurl + '" style="Height:100%; width:100%;  border:0;margin:0px"></iframe>';
                openTrackItWindow(RestrackitmenuOpenwindowMessage + ': ' + selectedVehicleData.data.Description, htmlNewWin, 600, 480); //openWindow('Track Vehicles', htmlNewWin, 600, 480);
                wincounter++;
            }
            catch (err) {
                //alert(err);
            }
        }
    };

    var streetViewMenu =
    {
        text: ResStreetViewMenuText, // 'Street view',
        id: 'streetViewMenu',
        tooltip: ResGoogleStreetView, //'Google street view',
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
                    openWindow(ResstreetViewMenuOpenwindowMessage, htmlNewWin, 1000, 480); //openWindow('Street view', htmlNewWin, 1000, 480);
                    wincounter++;
                }
                else {
                    Ext.MessageBox.alert(ResStreetViewMenualertTitle, ResStreetViewMenualertMessage); //Ext.MessageBox.alert('Streetview', ' Please select a vehicle for street view.');
                }


                /*var currentTicked = vehiclegrid.getSelectionModel().getSelection();
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
                }*/
            }
            catch (err) {
            }
        }
    };

    var updatePositionMenu =
    {
        text: ResUpdatePositionMenuText, //'Update Position',
        id: 'updatePositionMenu',
        tooltip: ResMapSelectedVehicleOnMap, //'Map selected vehicle on map',
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
                    url: './Vehicles.aspx/UpdatePosition',
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

    var clearAllMenu =
    {
        text: ResClearAllMenuText, //'ClearAll',
        id: 'clearAllMenu',
        tooltip: ResClearAllSelectedVehicles, //'Clear all selected vehicles',
        iconCls: 'map',
        cls: 'cmbfonts',
        textAlign: 'left',
        disabled: true,
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
    text: ResExportToCsvButtonText, //'Export To CSV',
    id: 'exportToCsvButton',
    tooltip: ResExport, // 'Export',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            if (VehicleGridInSearchMode) {
                var component = vehiclegrid;
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
            else {
                loadingMask.show();
                //vehiclerunner.stopAll();
                //vehiclerunner.tasks.length = 0;
                stopVehicleSync();
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
                    if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned") {
                        if (col.text == "Vin #")
                            columnsp += 'Vin' + ':' + col.dataIndex + ',';
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
                    url: 'Vehicles.aspx?QueryType=GetFilteredFleet&formattype=csv&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
                });

                form.getForm().submit();
                //vehiclerunner.start(vehicletask);
                vehicleAutoSyncStopped = false;
                vehicleAutoSync();
                loadingMask.hide();
            }

        }
        catch (err) {
            //vehiclerunner.start(vehicletask);
            vehicleAutoSyncStopped = false;
            vehicleAutoSync();
            loadingMask.hide();
            alert(err);
        }
    }
}


    var exportToExcel2003Button =
{
    text: ResExportToExcel2003ButtonText, //'Export To Excel2003',
    id: 'exportToExcel2003Button',
    tooltip: ResExport, //'Export',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            if (VehicleGridInSearchMode) {
                var component = vehiclegrid;
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
            else {
                loadingMask.show();
                //vehiclerunner.stopAll();
                //vehiclerunner.tasks.length = 0;
                stopVehicleSync();
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
                    if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned") {
                        if (col.text == "Vin #")
                            columnsp += 'Vin' + ':' + col.dataIndex + ',';
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
                    url: 'Vehicles.aspx?QueryType=GetFilteredFleet&formattype=excel2003&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
                });

                form.getForm().submit();
                //vehiclerunner.start(vehicletask);
                vehicleAutoSyncStopped = false;
                vehicleAutoSync();
                loadingMask.hide();
            }

        }
        catch (err) {
            //vehiclerunner.start(vehicletask);
            vehicleAutoSyncStopped = false;
            vehicleAutoSync();
            loadingMask.hide();
            alert(err);
        }
    }
}


    var exportToExcel2007Button =
{
    text: ResExportToExcel2007ButtonText, //'Export To Excel2007',
    id: 'exportToExcel2007Button',
    tooltip: ResExport, // 'Export',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            if (VehicleGridInSearchMode) {
                var component = vehiclegrid;
                var config = {};
                var formatter = 'json'

                var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

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
            else {
                loadingMask.show();
                //vehiclerunner.stopAll();
                //vehiclerunner.tasks.length = 0;
                stopVehicleSync();

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
                    if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned") {
                        if (col.text == "Vin #")
                            columnsp += 'Vin' + ':' + col.dataIndex + ',';
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
                //vehiclerunner.start(vehicletask);
                vehicleAutoSyncStopped = false;
                vehicleAutoSync();
                loadingMask.hide();

            }
        }
        catch (err) {
            //vehiclerunner.start(vehicletask);
            vehicleAutoSyncStopped = false;
            vehicleAutoSync();
            loadingMask.hide();
            alert(err);
        }
    }
}

    var exportHistoryToCsvButton =
{
    text: ResExportHistoryToCsvButtonText, //'Export To CSV',
    id: 'exportHistoryToCsvButton',
    tooltip: ResExport, //'Export',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            loadingMask.show();
            var sortingp = "";
            for (prop in hissortingParam) {
                sortingp += hissortingParam[prop] + ',';
            }
            var filterp = "";
            for (prop in hiscurrentFilters) {
                filterp += hiscurrentFilters[prop] + ',';
            }
            var columnsp = "";
            Ext.each(historygrid.columns, function (col, index) {
                if (index == 0)
                    return;
                if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned") {
                    if (col.text == "Vin #")
                        columnsp += 'Vin' + ':' + col.dataIndex + ';';
                    else
                        columnsp += col.text + ':' + col.dataIndex + ';';
                }
            });
            columnsp = columnsp.substring(0, columnsp.length - 1);
            var form = Ext.create('Ext.form.Panel', {
                xtype: 'form',
                itemId: 'hisuploadForm',
                hidden: true,
                standardSubmit: true,
                method: 'post',
                url: './historynew/historyservices.aspx?st=GetFilteredRecord&formattype=csv&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
            });
            form.getForm().submit();
            loadingMask.hide();
        }
        catch (err) {
            loadingMask.hide();
            Ext.Msg.alert("Error", err);

        }
    }
}


    var exportHistoryToExcel2003Button =
{
    text: ResExportHistoryToExcel2003ButtonText, //'Export To Excel2003',
    id: 'exportHistoryToExcel2003Button',
    tooltip: ResExport, //'Export',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            loadingMask.show();
            var sortingp = "";
            for (prop in hissortingParam) {
                sortingp += hissortingParam[prop] + ',';
            }
            var filterp = "";
            for (prop in hiscurrentFilters) {
                filterp += hiscurrentFilters[prop] + ',';
            }
            var columnsp = "";
            Ext.each(historygrid.columns, function (col, index) {
                if (index == 0)
                    return;
                if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned") {
                    if (col.text == "Vin #")
                        columnsp += 'Vin' + ':' + col.dataIndex + ';';
                    else
                        columnsp += col.text + ':' + col.dataIndex + ';';
                }
            });
            columnsp = columnsp.substring(0, columnsp.length - 1);
            var form = Ext.create('Ext.form.Panel', {
                xtype: 'form',
                itemId: 'hisuploadForm',
                hidden: true,
                standardSubmit: true,
                method: 'post',
                url: './historynew/historyservices.aspx?st=GetFilteredRecord&formattype=excel2003&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
            });
            form.getForm().submit();
            loadingMask.hide();
        }
        catch (err) {
            loadingMask.hide();
            Ext.Msg.alert("Error", err);

        }
    }
}


    var exportHistoryToExcel2007Button =
{
    text: ResExportHistoryToExcel2007ButtonText, //'Export To Excel2007',
    id: 'exportHistoryToExcel2007Button',
    tooltip: ResExport, //'Export',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            loadingMask.show();
            var sortingp = "";
            for (prop in hissortingParam) {
                sortingp += hissortingParam[prop] + ',';
            }
            var filterp = "";
            for (prop in hiscurrentFilters) {
                filterp += hiscurrentFilters[prop] + ',';
            }
            var columnsp = "";
            Ext.each(historygrid.columns, function (col, index) {
                if (index == 0)
                    return;
                if (!col.hidden && col.id != "vgHistory" && col.id != "vgIsRouteAssigned") {
                    if (col.text == "Vin #")
                        columnsp += 'Vin' + ':' + col.dataIndex + ';';
                    else
                        columnsp += col.text + ':' + col.dataIndex + ';';
                }
            });
            columnsp = columnsp.substring(0, columnsp.length - 1);
            var form = Ext.create('Ext.form.Panel', {
                xtype: 'form',
                itemId: 'hisuploadForm',
                hidden: true,
                standardSubmit: true,
                method: 'post',
                url: './historynew/historyservices.aspx?st=GetFilteredRecord&formattype=excel2007&operation=Export&sorting=' + sortingp + '&filters=' + filterp + '&columns=' + columnsp
            });
            form.getForm().submit();
            loadingMask.hide();
        }
        catch (err) {
            loadingMask.hide();
            Ext.Msg.alert("Error", err);

        }
    }
}

    var exportHistoryStopToCsvButton =
{
    text: ResExportHistoryStopToCsvButtonText, //'Export To CSV',
    id: 'exportHistoryStopToCsvButton',
    tooltip: ResExport, //'Export',
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
    text: ResExportHistoryStopToExcel2003ButtonText, //'Export To Excel2003',
    id: 'exportHistoryStopToExcel2003Button',
    tooltip: ResExport, //'Export',
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
    text: ResExportHistoryStopToExcel2007ButtonText, //'Export To Excel2007',
    id: 'exportHistoryStopToExcel2007Button',
    tooltip: ResExport, //'Export',
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
    text: ResExportHistoryTripToCsvButtonText, //'Export To CSV',
    id: 'exportHistoryTripToCsvButton',
    tooltip: ResExport, //'Export',
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
    text: ResExportHistoryTripToExcel2003ButtonText, //'Export To Excel2003',
    id: 'exportHistoryTripToExcel2003Button',
    tooltip: ResExport, //'Export',
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
            form.submit();

        }
        catch (err) {
            alert(err);
        }
    }
}


    var exportHistoryTripToExcel2007Button =
{
    text: ResExportHistoryTripToExcel2007ButtonText, //'Export To Excel2007',
    id: 'exportHistoryTripToExcel2007Button',
    tooltip: ResExport, //'Export',
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
            form.submit();

        }
        catch (err) {
            alert(err);
        }
    }
}

    var exportHistoryAddressToCsvButton =
{
    text: ResExportHisotryAddressToCsvButtonText, //'Export To CSV',
    id: 'exportHisotryAddressToCsvButton',
    tooltip: ResExport, //'Export',
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
            form.submit();

        }
        catch (err) {
            alert(err);
        }
    }
}


    var exportHistoryAddressToExcel2003Button =
{
    text: ResExportHistoryAddressToExcel2003ButtonText, //'Export To Excel2003',
    id: 'exportHistoryAddressToExcel2003Button',
    tooltip: ResExport, //'Export',
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
            form.submit();

        }
        catch (err) {
            alert(err);
        }
    }
}

    var exportHistoryAddressToExcel2007Button =
{
    text: ResExportHistoryAddressToExcel2007ButtonText, //'Export To Excel2007',
    id: 'exportHistoryAddressToExcel2007Button',
    tooltip: ResExport, //'Export',
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


    scrollMenu.add(/*finditmenu, */trackitmenu, streetViewMenu, updatePositionMenu, clearAllMenu);

    var vehicleStatusUpdate;
    if (ShowDashboardView) {
        vehicleStatusUpdate =
        {
            text: "Change Vehicle Status",
            id: 'vehicleStatusUpdate',
            tooltip: "Change Vehicle Status",
            iconCls: 'map',
            cls: 'cmbfonts',
            textAlign: 'left',
            disabled: true,
            handler: function () {
                try {
                    var currentTicked = vehiclegrid.getSelectionModel().getSelection();
                    var selectedVehicles = "";
                    var numAvailable = 0;
                    var numUnavailable = 0;
                    Ext.each(currentTicked, function (selectedRec, i) {
                        selectedVehicles = selectedVehicles + "," + selectedRec.data.VehicleId + ",";
                        if (selectedRec.data.OperationalState * 1 == 100)
                            numAvailable++;
                        else
                            numUnavailable++;
                    });
                    var defaultState = numUnavailable >= numAvailable ? 100 : 200;
                    var winurl = "./Widgets/BulkVehicleStateUpdate.aspx?defaultState=" + defaultState + "&selectedVehicles=" + selectedVehicles;
                    var htmlNewWin = '<iframe scrolling="no" src="' + winurl + '" style="Height:100%; width:100%;  border:0;margin:0px"></iframe>';
                    openGeneralPopupWindow("Change Vehicle Status", htmlNewWin, 900, 430, 'destroy', 'winBulkVehicleStateUpdate');
                    wincounter++;
                }
                catch (err) {
                }
            }
        }
        scrollMenu.add(vehicleStatusUpdate);
    }

    function buildBaseColumns() {
        if (vgrid.indexOf("," + allGridColumns.BoxId + ",") > -1 || vgrid == "all") {
            var col_vgUnitID = Ext.create('Ext.grid.column.Column', {
                id: 'vgUnitID',
                text: ResvgUnitIDText, //'UnitID',
                align: 'left',
                width: 70,
                dataIndex: 'BoxId',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.BoxId) - 1] == "y") ? false : true
            });

            vehicleGridColumns.push(col_vgUnitID);
            baseColumns.push(col_vgUnitID);
        }
        if (vgrid.indexOf("," + allGridColumns.Driver + ",") > -1 || vgrid == "all") {
            var col_vgDriver = Ext.create('Ext.grid.column.Column', {
                id: 'vgDriver',
                text: ResvgDriverText, //'Driver',
                align: 'left',
                width: 70,
                dataIndex: 'Driver',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Driver) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgDriver);
            baseColumns.push(col_vgDriver);
        }
        if (vgrid.indexOf("," + allGridColumns.Description + ",") > -1 || vgrid == "all") {
            var col_vgDescription = Ext.create('Ext.grid.column.Column', {
                id: 'vgDescription',
                text: ResvgDescriptionText, //'Description',
                align: 'left',
                width: 150,
                renderer: function (value, p, record) {
                    return Ext.String.format('<a href="javascript:void(0);" rel="bootstrapvehiclegridpopover" data-mouseover="false" data-popup="false" class="withajaxpopover" data-html="true" data-poload="./MapNew/frmGetVehicleInfo.aspx?LicensePlate={0}" data-title="{1} <button type=\'button\' class=\'close\' onclick=\'closebootstrappopover(this);hasVehiclePopover=false;\'>&times;</button>" data-placement="right" data-container="#vehicleInfoPopover" OnClick="SensorInfoWindow(\'{0}\')">{1}</a>', Ext.String.escape(record.data['LicensePlate']), value);
                },
                dataIndex: 'Description',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Description) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgDescription);
            baseColumns.push(col_vgDescription);
        }
        if (vgrid.indexOf("," + allGridColumns.VehicleStatus + ",") > -1 || vgrid == "all") {
            var col_vgStatus = Ext.create('Ext.grid.column.Column', {
                id: 'vgStatus',
                text: ResvgStatusText, //'Status',
                align: 'left',
                width: 120,
                renderer: function (value) {
                    var fontColor = "black";
                    if (value.indexOf("Parked") != -1 || value.indexOf("Stationn") != -1) {
                        fontColor = "red";
                    }
                    else if (value.indexOf("Idling") != -1 || value.indexOf("Moteur au ralenti") != -1) {
                        fontColor = "orange";
                    }
                    else if (value.indexOf("Moving") != -1 || value.indexOf("En mouvement") != -1) {
                        fontColor = "green";
                    }
                    return Ext.String.format(template, fontColor, value);
                },
                dataIndex: 'VehicleStatus',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.VehicleStatus) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgStatus);
            baseColumns.push(col_vgStatus);
        }

        if (vgrid.indexOf("," + allGridColumns.OriginDateTime + ",") > -1 || vgrid == "all") {
            var col_vgDateTime = Ext.create('Ext.grid.column.Column', {
                id: 'vgDateTime',
                text: ResvgDateTimeText, //'Date/Time',
                align: 'left',
                width: 135,
                //xtype: 'datecolumn',
                //format: userdateformat, //dateformat,
                dataIndex: 'OriginDateTime',
                filterable: true,
                sortable: true,
                tdCls: 'x-date-time',
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.OriginDateTime) - 1] == "y") ? false : true,
                renderer: function (value, p, record) {
                    return Ext.Date.format(value, userdateformat);
                }
            });
            vehicleGridColumns.push(col_vgDateTime);
            baseColumns.push(col_vgDateTime);
        }
    }

    function buildDefaultViewColumns() {

        baseColumns = [];
        buildBaseColumns();

        for (var i = 0; i < baseColumns.length; i++) {
            defaultViewColumns.push(baseColumns[i]);
        }

        if (vgrid.indexOf("," + allGridColumns.Speed + ",") > -1 || vgrid == "all") {
            var col_vgSpeed = Ext.create('Ext.grid.column.Column', {
                id: 'vgSpeed',
                text: ResvgSpeedText, //'Speed',
                align: 'left',
                width: 50,
                dataIndex: 'CustomSpeed',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Speed) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgSpeed);
            defaultViewColumns.push(col_vgSpeed);
        }

        if (vgrid.indexOf("," + allGridColumns.StreetAddress + ",") > -1 || vgrid == "all") {
            var col_vgPosition = Ext.create('Ext.grid.column.Column', {
                id: 'vgPosition',
                text: 'Position',
                align: 'left',
                width: 180,
                dataIndex: 'LatLon',
                renderer: function (value, p, record) {
                    return Ext.String.format('<a href="javascript:void(0);" OnClick="GotoMapPosition({0},{1})">{0},{1}</a>', record.data['Latitude'], record.data['Longitude']);
                },
                filterable: false,
                sortable: false,
                hidden: true
            });
            vehicleGridColumns.push(col_vgPosition);
            defaultViewColumns.push(col_vgPosition);

            var col_vgAddress = Ext.create('Ext.grid.column.Column', {
                id: 'vgAddress',
                text: ResvgAddressText, //'Address',
                align: 'left',
                width: 300,
                dataIndex: 'StreetAddress',
                renderer: function (value, p, record) {
                    if (value == 'Resolve Address' || value == 'Address resolution in progress')
                        return '<a href="javascript:void(0);" alt="Resolve" title="' + ResResolveAddressToolTips + '"  OnClick="ResolveAddress(this, ' + record.data['BoxId'] + ',' + record.data['Latitude'] + ',' + record.data['Longitude'] + ')">' + ResAddressresolutioninprogress + '</a>';//, Ext.String.escape(record.data['BoxId']), Ext.String.escape(record.data['Latitude']), Ext.String.escape(record.data['Longitude']), value);
                    else
                        return value;
                },
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.StreetAddress) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgAddress);
            defaultViewColumns.push(col_vgAddress);
        }
        if (vgrid.indexOf("," + allGridColumns.NearestLandmark + ",") > -1 || vgrid == "all") {
            var col_vgNearestLandmark = Ext.create('Ext.grid.column.Column', {
                id: 'vgNearestLandmark',
                text: ResLandmarks,
                align: 'left',
                width: 90,
                dataIndex: 'NearestLandmark',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.NearestLandmark) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgNearestLandmark);
            defaultViewColumns.push(col_vgNearestLandmark);
        }
        if (vgrid.indexOf("," + allGridColumns.BoxArmed + ",") > -1 || vgrid == "all") {
            var col_vgArmed = Ext.create('Ext.grid.column.Column', {
                id: 'vgArmed',
                text: ResvgArmedText, //'Armed',
                align: 'left',
                width: 40,
                dataIndex: 'BoxArmed',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.BoxArmed) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgArmed);
            defaultViewColumns.push(col_vgArmed);
        }
        if (vgrid.indexOf("," + allGridColumns.History + ",") > -1 || vgrid == "all") {
            var col_vgHistory = Ext.create('Ext.grid.column.Column', {
                id: 'vgHistory',
                text: ResvgHistoryText, //'History',
                align: 'left',
                width: 90,
                renderer: function (value) {
                    if (HistoryEnabled)
                        return '<a href="javascript:void(0);" OnClick="historyMessageStore.load();showHistoryTab(\'' + value + '\');">' + ResHistoryLinkText + '</a>';
                    else
                        return '';
                },
                dataIndex: 'VehicleId',
                filterable: false,
                sortable: false,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.History) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgHistory);
            defaultViewColumns.push(col_vgHistory);
        }
        if (vgrid.indexOf("," + allGridColumns.DriverCardNumber + ",") > -1 || vgrid == "all") {
            var col_vgDriverCardNumber = Ext.create('Ext.grid.column.Column', {
                id: 'vgDriverCardNumber',
                text: ResvgDriverCardNumberText,
                align: 'left',
                width: 120,
                dataIndex: 'DriverCardNumber',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.DriverCardNumber) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgDriverCardNumber);
            defaultViewColumns.push(col_vgDriverCardNumber);
        }
        if (vgrid.indexOf("," + allGridColumns.Field1 + ",") > -1 || vgrid == "all") {
            var col_vgField1 = Ext.create('Ext.grid.column.Column', {
                id: 'vgField1',
                text: ResvgField1Text, //'Driver',
                align: 'left',
                width: 70,
                dataIndex: 'Field1',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Field1) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgField1);
            defaultViewColumns.push(col_vgField1);
        }
        if (vgrid.indexOf("," + allGridColumns.Field2 + ",") > -1 || vgrid == "all") {
            var col_vgField2 = Ext.create('Ext.grid.column.Column', {
                id: 'vgField2',
                text: ResvgField2Text, //'Driver',
                align: 'left',
                width: 70,
                dataIndex: 'Field2',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Field2) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgField2);
            defaultViewColumns.push(col_vgField2);
        }
        if (vgrid.indexOf("," + allGridColumns.Field3 + ",") > -1 || vgrid == "all") {
            var col_vgField3 = Ext.create('Ext.grid.column.Column', {
                id: 'vgField3',
                text: ResvgField3Text, //'Driver',
                align: 'left',
                width: 70,
                dataIndex: 'Field3',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Field3) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgField3);
            defaultViewColumns.push(col_vgField3);
        }
        if (vgrid.indexOf("," + allGridColumns.Field4 + ",") > -1 || vgrid == "all") {
            var col_vgField4 = Ext.create('Ext.grid.column.Column', {
                id: 'vgField4',
                text: ResvgField4Text, //'Driver',
                align: 'left',
                width: 70,
                dataIndex: 'Field4',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Field4) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgField4);
            defaultViewColumns.push(col_vgField4);
        }
        if (vgrid.indexOf("," + allGridColumns.Field5 + ",") > -1 || vgrid == "all") {
            var col_vgField5 = Ext.create('Ext.grid.column.Column', {
                id: 'vgField5',
                text: ResvgField5Text, //'Driver',
                align: 'left',
                width: 70,
                dataIndex: 'Field5',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Field5) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgField5);
            defaultViewColumns.push(col_vgField5);
        }
        if (vgrid.indexOf("," + allGridColumns.ModelYear + ",") > -1 || vgrid == "all") {
            var col_vgModelYear = Ext.create('Ext.grid.column.Column', {
                id: 'vgModelYear',
                text: ResvgModelYearText, //'Driver',
                align: 'left',
                width: 50,
                dataIndex: 'ModelYear',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.ModelYear) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgModelYear);
            defaultViewColumns.push(col_vgModelYear);
        }
        if (vgrid.indexOf("," + allGridColumns.MakeName + ",") > -1 || vgrid == "all") {
            var col_vgMakeName = Ext.create('Ext.grid.column.Column', {
                id: 'vgMakeName',
                text: ResvgMakeNameText, //'Driver',
                align: 'left',
                width: 50,
                dataIndex: 'MakeName',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.MakeName) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgMakeName);
            defaultViewColumns.push(col_vgMakeName);
        }
        if (vgrid.indexOf("," + allGridColumns.ModelName + ",") > -1 || vgrid == "all") {
            var col_vgModelName = Ext.create('Ext.grid.column.Column', {
                id: 'vgModelName',
                text: ResvgModelNameText, //'Driver',
                align: 'left',
                width: 50,
                dataIndex: 'ModelName',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.ModelName) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgModelName);
            defaultViewColumns.push(col_vgModelName);
        }
        if (vgrid.indexOf("," + allGridColumns.VehicleTypeName + ",") > -1 || vgrid == "all") {
            var col_vgVehicleTypeName = Ext.create('Ext.grid.column.Column', {
                id: 'vgVehicleTypeName',
                text: ResvgVehicleTypeNameText, //'Driver',
                align: 'left',
                width: 80,
                dataIndex: 'VehicleTypeName',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.VehicleTypeName) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgVehicleTypeName);
            defaultViewColumns.push(col_vgVehicleTypeName);
        }
        if (vgrid.indexOf("," + allGridColumns.LicensePlate + ",") > -1 || vgrid == "all") {
            var col_vgLicensePlate = Ext.create('Ext.grid.column.Column', {
                id: 'vgLicensePlate',
                text: ResvgLicensePlateText, //'Driver',
                align: 'left',
                width: 75,
                dataIndex: 'LicensePlate',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.LicensePlate) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgLicensePlate);
            defaultViewColumns.push(col_vgLicensePlate);
        }
        if (vgrid.indexOf("," + allGridColumns.VinNum + ",") > -1 || vgrid == "all") {
            var col_vgVinNum = Ext.create('Ext.grid.column.Column', {
                id: 'vgVinNum',
                text: ResvgVinNumText, //'Driver',
                align: 'left',
                width: 50,
                dataIndex: 'VinNum',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.VinNum) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgVinNum);
            defaultViewColumns.push(col_vgVinNum);
        }
        if (vgrid.indexOf("," + allGridColumns.ManagerName + ",") > -1 || vgrid == "all") {
            var col_vgManagerName = Ext.create('Ext.grid.column.Column', {
                id: 'vgManagerName',
                text: ResvgManagerNameText, //'Driver',
                align: 'left',
                width: 90,
                dataIndex: 'ManagerName',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.ManagerName) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgManagerName);
            defaultViewColumns.push(col_vgManagerName);
        }
        if (vgrid.indexOf("," + allGridColumns.ManagerEmployeeId + ",") > -1 || vgrid == "all") {
            var col_vgManagerEmployeeId = Ext.create('Ext.grid.column.Column', {
                id: 'vgManagerEmployeeId',
                text: ResvgManagerEmployeeIdText, //'Driver',
                align: 'left',
                width: 100,
                dataIndex: 'ManagerEmployeeId',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.ManagerEmployeeId) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgManagerEmployeeId);
            defaultViewColumns.push(col_vgManagerEmployeeId);
        }
        if (vgrid.indexOf("," + allGridColumns.StateProvince + ",") > -1 || vgrid == "all") {
            var col_vgStateProvince = Ext.create('Ext.grid.column.Column', {
                id: 'vgStateProvince',
                text: ResvgStateProvinceText, //'Driver',
                align: 'left',
                width: 90,
                dataIndex: 'StateProvince',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.StateProvince) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgStateProvince);
            defaultViewColumns.push(col_vgStateProvince);
        }
        if (vgrid.indexOf("," + allGridColumns.Color + ",") > -1 || vgrid == "all") {
            var col_vgColor = Ext.create('Ext.grid.column.Column', {
                id: 'vgColor',
                text: 'Color', //'Color'
                align: 'left',
                width: 90,
                dataIndex: 'Color',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Color) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgColor);
            defaultViewColumns.push(col_vgColor);
        }
        if (vgrid.indexOf("," + allGridColumns.PTO + ",") > -1 || vgrid == "all") {
            var col_vgPTO = Ext.create('Ext.grid.column.Column', {
                id: 'vgPTO',
                text: "PTO", //'PTO',
                align: 'left',
                width: 40,
                dataIndex: 'PTO',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.PTO) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgPTO);
            defaultViewColumns.push(col_vgPTO);
        }

        if (ShowEngineHours) {
            if (vgrid.indexOf("," + allGridColumns.EngineHours + ",") > -1 || vgrid == "all") {
                var col_vgEngineHours = Ext.create('Ext.grid.column.Column', {
                    id: 'vgEngineHours',
                    text: ResvgEngineHoursText,
                    align: 'left',
                    width: 80,
                    dataIndex: 'EngineHours',
                    filterable: true,
                    sortable: true,
                    hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.EngineHours) - 1] == "y") ? false : true
                });
                vehicleGridColumns.push(col_vgEngineHours);
                defaultViewColumns.push(col_vgEngineHours);
            }
        }
        if (ShowOdometer) {
            if (vgrid.indexOf("," + allGridColumns.Odometer + ",") > -1 || vgrid == "all") {
                var col_vgOdometer = Ext.create('Ext.grid.column.Column', {
                    id: 'vgOdometer',
                    text: ResvgOdometerText,
                    align: 'left',
                    width: 80,
                    dataIndex: 'Odometer',
                    filterable: true,
                    sortable: true,
                    hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.Odometer) - 1] == "y") ? false : true
                });
                vehicleGridColumns.push(col_vgOdometer);
                defaultViewColumns.push(col_vgOdometer);
            }
        }
        if (vgrid.indexOf("," + allGridColumns.IsRouteAssigned + ",") > -1 || vgrid == "all" || DispatchOrganizationId == 480 || DispatchOrganizationId == 999991) {
            var col_vgIsRouteAssigned = Ext.create('Ext.grid.column.Column', {
                id: 'vgIsRouteAssigned',
                text: ResvgIsRouteAssignedText, //'Route Assigned',
                align: 'left',
                width: 90,
                dataIndex: 'IsRouteAssigned',
                renderer: function (value, p, record) {
                    return Ext.String.format('<a href="javascript:void(0);" OnClick="RouteWindow(\'{0}\')">{1}</a>', Ext.String.escape(record.data['Description']).replace(/\'/g, "&singlequote;"), value);
                },
                filterable: true,
                sortable: false,
                hidden: (VGridActive == "" || DispatchOrganizationId == 480 || DispatchOrganizationId == 999991 || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.IsRouteAssigned) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgIsRouteAssigned);
            defaultViewColumns.push(col_vgIsRouteAssigned);
        }

        //if (vgrid.indexOf("," + allGridColumns.LastIgnOnBatV + ",") > -1 || vgrid == "all") {
        if (BatteryTredingEnabled) {
            var col_vgLastIgnOnBatV = Ext.create('Ext.grid.column.Column', {
                id: 'vgLastIgnOnBatV',
                text: "Battery On(v)", //'Ign On Reading(V)',
                align: 'left',
                width: 80,
                dataIndex: 'LastIgnOnBatV',
                filterable: true,
                sortable: true,
                renderer: function (value, p, record) {
                    if (value == 0)
                        return value;
                    else if (isNumber(value))
                        return (value * 1).toFixed(2);
                    return "";
                },
                //hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.LastIgnOnBatV) - 1] == "y") ? false : true
                hidden: false
            });
            vehicleGridColumns.push(col_vgLastIgnOnBatV);
            defaultViewColumns.push(col_vgLastIgnOnBatV);
        }

        //if (vgrid.indexOf("," + allGridColumns.LastIgnOffBatV + ",") > -1 || vgrid == "all") {
        if (BatteryTredingEnabled) {
            var col_vgLastIgnOffBatV = Ext.create('Ext.grid.column.Column', {
                id: 'vgLastIgnOffBatV',
                text: "Battery Off(v)", //'Vehicle Off Reading(V)',
                align: 'left',
                width: 115,
                dataIndex: 'LastIgnOffBatV',
                filterable: true,
                sortable: true,
                renderer: function (value, p, record) {
                    if (value == 0)
                        return value;
                    else if (isNumber(value))
                        return (value * 1).toFixed(2);
                    return "";
                },
                //hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.LastIgnOffBatV) - 1] == "y") ? false : true
                hidden: false
            });
            vehicleGridColumns.push(col_vgLastIgnOffBatV);
            defaultViewColumns.push(col_vgLastIgnOffBatV);
        }

        //if (vgrid.indexOf("," + allGridColumns.LastIgnOffBatV + ",") > -1 || vgrid == "all") {
        /*if (BatteryTredingEnabled) {
            vehicleGridColumns.push({
                id: 'vgLastBatV',
                text: "Last Battery(v)",
                align: 'left',
                width: 120,
                dataIndex: 'LastBatV',
                filterable: true,
                sortable: true,
                renderer: function (value, p, record) {
                    if (value == 0)
                        return value;
                    else if (isNumber(value))
                        return (value * 1).toFixed(2);
                    return "";
                },
                hidden: false
            });
        }*/

        // if (vgrid.indexOf("," + allGridColumns.CustomConfig + ",") > -1 || vgrid == "all") {
        if (DispatchOrganizationId == 123) { // Hard-coded MDT Serial column only for CN. Mantis # 15936
            var col_vgCustomConfig = Ext.create('Ext.grid.column.Column', {
                id: 'vgCustomConfig',
                text: "MDT Serial", //'PPCID',
                align: 'left',
                width: 140,
                dataIndex: 'CustomConfig',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.CustomConfig) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgCustomConfig);
            defaultViewColumns.push(col_vgCustomConfig);
        }

        if (vgrid.indexOf("," + allGridColumns.SAP_number + ",") > -1 || vgrid == "all") {
            var col_vgSAP_number = Ext.create('Ext.grid.column.Column', {
                id: 'vgSAP_number',
                text: 'BSC',
                align: 'left',
                width: 90,
                dataIndex: 'SAP_number',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.SAP_number) - 1] == "y") ? false : true
            });
            vehicleGridColumns.push(col_vgSAP_number);
            defaultViewColumns.push(col_vgSAP_number);
        }
    }

    function buildDashboardViewColumns() {

        baseColumns = [];
        buildBaseColumns();

        for (var i = 0; i < baseColumns.length; i++) {
            dashboardViewColumns.push(baseColumns[i]);
        }

        var col_vgOperationalState = Ext.create('Ext.grid.column.Column', {
            id: 'vgDashboardOperationalState',
            text: 'Vehicle State',
            align: 'left',
            width: 140,
            dataIndex: 'OperationalStateName',
            filterable: true,
            sortable: true,
            hidden: false
        });

        dashboardViewColumns.push(col_vgOperationalState);

        var col_vgDashboardNotes = Ext.create('Ext.grid.column.Column', {
            id: 'vgDashboardNotes',
            text: 'Notes',
            align: 'left',
            width: 270,
            dataIndex: 'OperationalStateNotes',
            filterable: true,
            sortable: true,
            hidden: false
        });

        dashboardViewColumns.push(col_vgDashboardNotes);

        var col_vgLandmarkInDateTime = Ext.create('Ext.grid.column.Column', {
            id: 'vgLandmarkInDateTime',
            text: 'In DateTime',
            align: 'left',
            width: 150,
            dataIndex: 'LandmarkInDateTime',
            filterable: true,
            sortable: true,
            hidden: true,
            renderer: function (value, p, record) {
                if (value == '' || value == null) {
                    return '';
                }
                return Ext.Date.format(value, userdateformat);
            }
        });
        dashboardViewColumns.push(col_vgLandmarkInDateTime);

        var col_vgDashboardTimeInLandmark = Ext.create('Ext.grid.column.Column', {
            id: 'vgDashboardTimeInLandmark',
            text: 'Time Inside (hours)',
            align: 'left',
            width: 150,
            dataIndex: 'DurationInLandmarkMin',
            filterable: true,
            sortable: true,
            hidden: false,
            renderer: function (value, p, record) {

                if (value == 0 && record.get('LandmarkName') != '')
                    return 0;
                else if (value == 0)
                    return '';
                else
                    return value;
            }
        });

        dashboardViewColumns.push(col_vgDashboardTimeInLandmark);

        //function build_col_vgDashboardLandmarkName() {
        //    return Ext.create('Ext.grid.column.Column', {
        //        id: 'vgDashboardLandmarkName',
        //        text: 'Landmark Name',
        //        align: 'left',
        //        width: 140,
        //        dataIndex: 'LandmarkName',
        //        filterable: false,
        //        sortable: false,
        //        hidden: false//,
        //        //renderer: function (value) {
        //        //    //var rec = testStore.getById(value);
        //        //    //return rec ? rec.get('LandmarkName') : '';
        //        //    var i = testStore.findExact('VehicleID', value, 0);
        //        //    if (i != -1) {
        //        //        return testStore.getAt(i).get('LandmarkName');
        //        //    }
        //        //    else
        //        //        return '';
        //        //}
        //    });
        //}

        var col_vgDashboardLandmarkName = Ext.create('Ext.grid.column.Column', {
            id: 'vgDashboardLandmarkName',
            text: 'Location',
            align: 'left',
            width: 250,
            dataIndex: 'LandmarkName',
            filterable: true,
            sortable: true,
            hidden: false,
            renderer: function (value, p, record) {
                if (value == '')
                    return '';

                var s = Ext.String.format('<a href="javascript:void(0);" OnClick="gotoLandmark(\'{0}\')">{1}</a>', record.data['LandmarkID'], value);
                return s;
            }
        });

        dashboardViewColumns.push(col_vgDashboardLandmarkName);

        if (vgrid.indexOf("," + allGridColumns.StreetAddress + ",") > -1 || vgrid == "all") {
            var col_vgAddress = Ext.create('Ext.grid.column.Column', {
                id: 'vgAddress',
                text: ResvgAddressText, //'Address',
                align: 'left',
                width: 300,
                dataIndex: 'StreetAddress',
                renderer: function (value, p, record) {
                    if (value == 'Resolve Address' || value == 'Address resolution in progress')
                        return '<a href="javascript:void(0);" alt="Resolve" title="' + ResResolveAddressToolTips + '"  OnClick="ResolveAddress(this, ' + record.data['BoxId'] + ',' + record.data['Latitude'] + ',' + record.data['Longitude'] + ')">' + ResAddressresolutioninprogress + '</a>';//, Ext.String.escape(record.data['BoxId']), Ext.String.escape(record.data['Latitude']), Ext.String.escape(record.data['Longitude']), value);
                    else
                        return value;
                },
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.StreetAddress) - 1] == "y") ? false : true
            });
            dashboardViewColumns.push(col_vgAddress);
        }

        if (vgrid.indexOf("," + allGridColumns.ManagerName + ",") > -1 || vgrid == "all") {
            var col_vgManagerNameDashboard = Ext.create('Ext.grid.column.Column', {
                id: 'vgManagerName',
                text: ResvgManagerNameText,
                align: 'left',
                width: 90,
                dataIndex: 'ManagerName',
                filterable: true,
                sortable: true,
                hidden: (VGridActive == "" || VGridActive.split(",")[vgrid.split(",").indexOf("" + allGridColumns.ManagerName) - 1] == "y") ? false : true
            });
            dashboardViewColumns.push(col_vgManagerNameDashboard);
        }
    }

    buildBaseColumns();
    buildDefaultViewColumns();

    function ShowDashboard() {

        if (currentView == 'dashboard')
            return;

        //vehiclegrid.headerCt.removeAll();

        // Insert DashboardView columns

        clearVehiceSelections();

        vehiclegrid.filters.addFilters(filters.filters);
        currentFilters = {};

        dashboardViewColumns = [];
        buildDashboardViewColumns();

        vehiclegrid.reconfigure(vehiclegrid.getStore(), dashboardViewColumns);

        currentView = 'dashboard';

        landmarkCategory.show();
        availabilityChartBtn.show();

        vehiclegrid.getView().refresh();

        //dashboardBtn.disable();
        //defaultViewBtn.enable();

        recordUpdater.load({
            params:
          {
              QueryType: 'GetVehiclePosition',
              filters: currentFilters,
              sorting: sortingParam,
              start: (currentpage - 1) * VehicleListPagesize,
              limit: VehicleListPagesize
          }
        });
    }

    function ShowDefaultView() {

        if (currentView == 'default')
            return;

        // Remove DashboardView Columns
        //for (var _dashboardColumn in dashboardViewColumns) {
        //    vehiclegrid.headerCt.remove(dashboardViewColumns[_dashboardColumn].getId());
        //}
        //vehiclegrid.headerCt.removeAll();

        clearVehiceSelections();

        vehiclegrid.filters.addFilters(filters.filters);
        currentFilters = {};

        defaultViewColumns = [];
        buildDefaultViewColumns();

        // Insert defaultView columns
        //for (var _defaultColumn in defaultViewColumns) {
        //    vehiclegrid.headerCt.insert(vehiclegrid.columns.length, defaultViewColumns[_defaultColumn]);
        //}
        vehiclegrid.reconfigure(vehiclegrid.getStore(), defaultViewColumns);

        currentView = 'default';
        landmarkCategory.hide();
        availabilityChartBtn.hide();

        vehiclegrid.getView().refresh();

        //dashboardBtn.enable();
        //defaultViewBtn.disable();

        recordUpdater.load({
            params:
          {
              QueryType: 'GetVehiclePosition',
              filters: currentFilters,
              sorting: sortingParam,
              start: (currentpage - 1) * VehicleListPagesize,
              limit: VehicleListPagesize
          }
        });
    }

    function clearVehiceSelections() {
        try {
            vehiclegrid.getSelectionModel().deselectAll(false);
            var el = document.getElementById(mapframe).contentWindow;
            el.closeVehiclePopups();
        }
        catch (err) { }
    }

    function clone(obj) {
        if (obj === null || typeof (obj) !== 'object')
            return obj;

        var temp = obj.constructor(); // changed

        for (var key in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, key)) {
                temp[key] = clone(obj[key]);
            }
        }
        return temp;
    }

    var currentFilters = new Object();
    var filteron = false;
    vehiclegrid = Ext.create('Ext.grid.Panel',
{
    id: 'vehiclesgrid',
    enableColumnHide: true,
    title: ResVehicles, //'Vehicles',
    autoLoad: false,
    autoScroll: true,
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
    stateful: true, // state should be preserved
    //stateEvents: ['columnresize', 'columnmove', 'show', 'hide'],

    /*viewConfig:
    {
    stripeRows: false,
    emptyText: 'No vehicles to display',
    useMsg: false
    }
    ,*/
    columns: vehicleGridColumns
     , dockedItems: [
   {
       xtype: 'toolbar',
       dock: 'top',
       items: [LoadVehiclesBasedOn == 'fleet' ? fleetButton : organizationHierarchy,
       /*{
       xtype: 'cycle',
       text: 'Reading Pane',
       prependText: 'Map: ',
       showText: true,
       scope: this,
       changeHandler: readingPaneChange,
       hidden: true,
       menu:
       {
       id: 'reading-menu',
       items: [
       {
       text: 'Top',
       checked: defaultMapView == "south" ? true : false,
       iconCls: 'preview-top'
       }
       ,
       {
       text: 'Bottom',
       checked: defaultMapView == "north" ? true : false,
       iconCls: 'preview-bottom'
       }
       ,
       {
       text: 'Right',
       checked: defaultMapView == "west" ? true : false,
       iconCls: 'preview-right'
       }
       ,
       {
       text: 'Hide',
       iconCls: 'preview-hide'
       }
       ]
       }
       }
       ,*/
              '-',
              {
                  icon: 'preview.png',
                  cls: 'x-btn-text-icon',
                  text: ResdockedItemsActionsText, //'Actions',
                  menu: scrollMenu
              },
              searchMap, // exportButton,
              {
                  itemId: 'AutoSync',
                  boxLabel: 'AutoSync',
                  boxLabelCls: 'cmbfonts',
                  xtype: 'checkboxfield',
                  disabled: vehinterval == 0 ? true : false,
                  checked: IsSyncOn,
                  tooltip: ResRefreshTheMapAndGridAutomatically, //'Refresh the map and grid automatically',
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
      , {
          icon: 'preview.png',
          cls: 'x-btn-text-icon',
          text: ResdockedItemsExportText, //'Export',
          menu: exportMenu
      }
      , clearSearchBtn
      , ShowRouteAssignment ? allRoutesBtn : null
      , batteryTrendingBtn
      , ShowDashboardView ? landmarkCategory : null
      , ShowDashboardView ? availabilityChartBtn : null
      , { xtype: 'tbfill' }
      , ShowDashboardView ? defaultViewBtn : null
      , ShowDashboardView ? dashboardBtn : null
      //, segmentbtn
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
               selectedVehicleData = record; //record.data

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
           if (VehicleGridInSearchMode)
               return;
           var checkedHd = vehiclegrid.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
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
                                       if (checkedHd) {
                                           selModel.selectAll(true);
                                       }
                                       else
                                           slectedboxidarray.length = 0;
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
                                           slectedboxidarray.length = 0;
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
                                           if (checkedHd) {
                                               selModel.selectAll(true);
                                           }
                                           else
                                               slectedboxidarray.length = 0;
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
                                       slectedboxidarray.length = 0;
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
                                       if (checkedHd) {
                                           selModel.selectAll(true);
                                       }
                                       else
                                           slectedboxidarray.length = 0;
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
                                   slectedboxidarray.length = 0;
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
                                   if (checkedHd) {
                                       selModel.selectAll(true);
                                   }
                                   else
                                       slectedboxidarray.length = 0;
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
                           slectedboxidarray.length = 0;
                       },
                       scope: this
                   });
               }
           }
       }
   }
   ,
    viewConfig: {
        loadMask: false,
        stripeRows: false,
        emptyText: ResVehiclePagerEmptyMsg, //'No vehicles to display',
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

    var AMfilters = {
        ftype: 'filters',
        encode: false,
        local: true
    };
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
    enableColumnHide: true,
    enableSorting: true,
    features: [AMfilters],
    closable: false,
    width: window.screen.width,
    autoHeight: true,
    title: ResAlarms, //'Alarms',
    store: alarmsstore,
    columnLines: true,
    stateId: 'stateAGrid',
    viewConfig:
   {
       emptyText: ResalarmgridEmptyText, //'No alarms to display',
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
       text: ResalarmgridcolumnsAlarmId, //'Number',
       align: 'left',
       width: 80,
       renderer: function (value) {
           return Ext.String.format('<a href="javascript:void(0);" OnClick="NewAlarmWindow({0})">{1}</a>', value, value);
       }
      ,
       dataIndex: 'AlarmId',
       sortable: true,
       filter: {
           type: 'int'
       }
   }
   ,
   {
       text: ResalarmgridcolumnsTimeCreated, //'Alarm Time',
       align: 'left',
       width: 135,
       xtype: 'datecolumn',
       format: userdateformat, //dateformat,
       dataIndex: 'TimeCreated',
       sortable: true,
       filter: {
           type: 'date'
       }
   }
   ,
   {
       text: ResalarmgridcolumnsAlarmLevel, // 'Alarm Priority',
       align: 'left',
       width: 80,
       dataIndex: 'AlarmLevel',
       sortable: true,
       filter: {
           type: 'string'
       }
   }
   ,
   {
       text: ResalarmgridcolumnsAlarmDescription, //'Alarm Description',
       align: 'left',
       width: 120,
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
       sortable: true,
       filter: {
           type: 'string'
       }
   }
   ,
   {
       text: ResalarmgridcolumnsvehicleDescription, // 'Vehicle Description',
       align: 'left',
       width: 120,
       dataIndex: 'vehicleDescription',
       sortable: true,
       filter: {
           type: 'string'
       }
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
    enableColumnHide: true,
    enableSorting: true,
    features: [AMfilters],
    closable: false,
    width: window.screen.width,
    autoHeight: true,
    title: ResMessages, //'Messages',
    store: messagesstore,
    columnLines: true,
    stateId: 'stateMGrid',
    viewConfig:
   {
       emptyText: ResmessagegridemptyText, //'No messages to display',
       useMsg: false
   }
   ,
    columns: [
   {
       text: ResmessagegridcolumnsMessageId, //'MessageId',
       align: 'left',
       width: 80,
       renderer: function (value, p, record) {

           var MsgKey = Ext.String.escape(record.data['MsgKey']);
           return Ext.String.format('<a href="javascript:void(0);" OnClick="NewMessageWindow(\'{0}\')">{1}</a>', MsgKey, value);
       }
      ,
       dataIndex: 'MessageId',
       sortable: true,
       filter: {
           type: 'int'
       }
   }
   ,
   {
       text: ResmessagegridcolumnsMsgDateTime, //'Date/Time',
       align: 'left',
       width: 135,
       xtype: 'datecolumn',
       format: userdateformat, //dateformat,
       dataIndex: 'MsgDateTime',
       sortable: true,
       filter: {
           type: 'date'
       }
   }
   ,
   {
       text: ResmessagegridcolumnsDescription, // 'From',
       align: 'left',
       width: 150,
       dataIndex: 'Description',
       sortable: true,
       filter: {
           type: 'string'
       }
   }
   ,
   {
       text: ResmessagegridcolumnsMsgBody, //'Message Body',
       align: 'left',
       width: 200,
       dataIndex: 'MsgBody',
       sortable: true,
       filter: {
           type: 'string'
       }
   }
   ,
   {
       text: ResmessagegridcolumnsAcknowledged, //'Acknowledged',
       align: 'left',
       width: 120,
       dataIndex: 'Acknowledged',
       sortable: true,
       filter: {
           type: 'string'
       }
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
    enableSorting: true,
    closable: false,
    width: window.screen.width,
    autoHeight: true,
    title: ResGeozoneLandmarks, //'Geozone/Landmarks',
    store: geolandmarksstore,
    columnLines: true,
    stateId: 'stateLandmarkGrid',
    viewConfig:
   {
       emptyText: ResgeolandmarkgridemptyText, //'No Geozone/Landmarks to display',
       useMsg: false
   }
   ,
    columns: [
     {
         header: Resgeolandmarkgridcolumnsname, //'Name',
         dataIndex: 'name'
     },
     {
         header: ResgeolandmarkgridcolumnsType, //'Type',
         dataIndex: 'type'
     }
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
            geolandmarkgrid.setTitle(ResgeolandmarkgridsetTitle); //geolandmarkgrid.setTitle("Loading...");

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
    //bbar: ['->', 'Total Geozone/Landmarks: <span id="geolandmarkcount" style="margin-right:20px;">0</span>']
    bbar: ['->', 'Total ' + ResGeozoneLandmarks + ': <span id="geolandmarkcount" style="margin-right:20px;">0</span>']
}
);
    var vehiclegeozoneassignment = Ext.create('Ext.Button',
{
    text: ResvehiclegeozoneassignmentButtonText, //'Vehicle-Geozone Assignment',
    id: 'vehiclegeozoneassignmentButton',
    tooltip: ResVehicleGeozoneAssignment, //'Vehicle-Geozone Assignment',
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
    title: ResGeozones, //'Geozones',
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
       emptyText: ResgeozonegridemptyText, //'No Geozones to display',
       useMsg: false
   }
   ,
    columns: [
     {
         header: ResgeozonegridcolumnsGeozone, //'Geozone'
         dataIndex: 'name',
         align: 'left',
         width: 170,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         text: Resgeozonegridcolumnsdesc, //'Description',
         align: 'left',
         width: 270,
         dataIndex: 'desc',
         filterable: true,
         sortable: true,
         hidden: true
     },
     {
         header: ResgeozonegridcolumnsDirection, //'Direction',
         dataIndex: 'direction',
         align: 'left',
         width: 70,
         filterable: true,
         sortable: true,
         hidden: false
     },
     {
         header: ResgeozonegridcolumnsSeverityName, //'Severity',
         dataIndex: 'SeverityName',
         filterable: true,
         sortable: true
     },
     {
         header: '',
         width: 120,
         renderer: function (value, p, record) {
             //var url = "./GeoZone_Landmarks/frmViewVehicleGeozones.aspx?geozoneId=" + value;
             //var urlToLoad = '<iframe width=\\\'100%\\\' height=\\\'100%\\\' frameborder=\\\'0\\\' scrolling=\\\'no\\\' src=\\\'' + url + '\\\'></iframe>';
             //return Ext.String.format('<a href="javascript:void(0);" OnClick="Ext.openWindow(\'Current Assignment\', \'{1}\', 400, 220)">Current Assignment</a>', value, urlToLoad);

             //return Ext.String.format('<a href="javascript:void(0);" OnClick="GetGeozoneCurrentAssignment({0});">Current Assignment</a>', value);
             return Ext.String.format('<a href="javascript:void(0);" OnClick="GetGeozoneCurrentAssignment({0});">' + ResgeozonegridcolumnsCurrentAssignment + '</a>', value);
         },
         dataIndex: 'id',
         filterable: true,
         sortable: true
     }
    ],
    listeners: {
        'activate': function (grid, eOpts) {
            if (!geozonegridloaded) {
                geozonegrid.setTitle(ReshistorygridemptyText2/*"Loading..."*/);
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
    //bbar: ['->', 'Total Geozones: <span id="geozonecount" style="margin-right:20px;">0</span>']
    bbar: ['->', 'Total ' + ResGeozones + ': <span id="geozonecount" style="margin-right:20px;">0</span>']
}
);

    var landmarksPager = new Ext.PagingToolbar(
    {
        store: landmarksstore,
        displayInfo: true,
        displayMsg: 'Total ' + ResLandmarks + ': {2}',//'Displaying alarms {0} - {1} of {2}',
        emptyMsg: 'Total ' + ResLandmarks + ': 0'
    });

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
    title: ResLandmarks, //'Landmarks',
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
       emptyText: ReslandmarkgridemptyText, // 'No Landmarks to display',
       useMsg: false
   }
   ,
    columns: [
     {
         header: Reslandmarkgridname, //'Landmark',
         dataIndex: 'name', align: 'left',
         width: 170,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: Reslandmarkgriddesc, //'Description',
         dataIndex: 'desc', align: 'left',
         width: 120,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: true
     },
     {
         header: ReslandmarkgridStreetAddress, //'Street Address',
         dataIndex: 'StreetAddress', align: 'left',
         width: 270,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReslandmarkgridEmail, //'Email',
         dataIndex: 'Email', align: 'left',
         width: 100,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReslandmarkgridContactPhoneNum, // 'Contact Phone',
         dataIndex: 'ContactPhoneNum', align: 'left',
         width: 90,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: Reslandmarkgridradius, // 'Radius (m)',
         dataIndex: 'radius', align: 'right',
         width: 70,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReslandmarkgridCategoryName, //'Category',
         dataIndex: 'CategoryName', align: 'left',
         width: 200,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     }
    ],
    listeners: {
        'activate': function (grid, eOpts) {
            if (!landmarkgridloaded) {
                landmarkgrid.setTitle(ResgeolandmarkgridsetTitle/*"Loading..."*/);
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
    //bbar: ['->', 'Total Landmarks: <span id="landmarkcount" style="margin-right:20px;">0</span>']
    bbar: landmarksPager//['->', 'Total ' + ResLandmarks + ': <span id="landmarkcount" style="margin-right:20px;">0</span>']
}
);

    var geozonelandmarktabs = Ext.create('Ext.tab.Panel',
{
    region: 'center', // a center region is ALWAYS required for border layout
    deferredRender: false,
    title: ResGeozoneLandmarks, //'Geozone/Landmarks',
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
               var isChecked = this.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
               if (isChecked && selections.length < 1) {
                   this.view.headerCt.child('gridcolumn[isCheckerHd]').el.removeCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                   this.clearSelections();
               }
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
        fieldLabel: ReshistoryDateFromfieldLabel, //'From',
        name: 'historyDateFrom',
        format: userDate,
        value: new Date()
    });
    historyTimeFrom = Ext.create('Ext.form.field.Time', {
        name: 'historyTimeFrom',
        fieldLabel: '',
        format: userTime,
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
        fieldLabel: ReshistoryDateTofieldLabel, //'To',
        name: 'historyDateTo',
        format: userDate,
        //value: (new Date()).getDate() + 1
        value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1)
    });
    historyTimeTo = Ext.create('Ext.form.field.Time', {
        name: 'historyTimeTo',
        fieldLabel: '',
        format: userTime,
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
        text: ResbtmHistorySearchText, //'Advanced Search',
        id: 'btmHistorySearch',
        tooltip: ResSearchHistory, //'Search History',
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
        text: ResbtnHistoryMapitText, //'Map It',
        id: 'btnHistoryMapit',
        tooltip: ResMapTheSelectedHistoryRecords, //'Map the selected history records',
        iconCls: 'map',
        margin: '0 5',
        cls: 'cmbfonts',
        textAlign: 'left',
        handler: function () {
            try {
                ResetHistoryReplay();

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
        text: ResbtnHistoryLegendText, //'Map Legend',
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
        text: ResbtnHistoryMapAllText, //'Map All',
        id: 'btnHistoryMapAll',
        iconCls: 'map',
        margin: '0 5',
        cls: 'cmbfonts',
        textAlign: 'left',
        hidden: false,
        handler: function () {
            try {
                ResetHistoryReplay();
                if (AllHistoryRecords.length > 0)
                    mapHistories(AllHistoryRecords, true, false, false);
            }
            catch (err) {
            }
        }
    }
    );

    var historyTripColorsWin;
    var historyTripColors = Ext.create('Ext.Button',
{
    text: ResHisTripColorsButtonText, //'Trip Colors',
    id: 'TripColorsButton',
    tooltip: ResHisTripColors, // 'Legend of Trip Colors',
    iconCls: 'map',
    margin: '0 5',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            if (!historyTripColorsWin) {
                var legendURL = "./TripColors.aspx";
                var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + legendURL + '"></iframe>';
                historyTripColorsWin = openWindow(ResHisTripColorsButtonText, urlToLoad, 500, 190);
            }
            else {
                if (historyTripColorsWin.isVisible()) {
                    historyTripColorsWin.hide();
                } else {
                    historyTripColorsWin.show();
                }
            }
        }
        catch (err) {
        }
    }
});

    btnVehicleonoff = Ext.create('Ext.Button',
{
    text: ifShowVehicleIcon ? ResbtnVehicleonoffHideDetailsText : ResbtnVehicleonoffShowDetailsText, //'Hide Details' : 'Show Details',
    id: 'labelVehicleonoffButton',
    tooltip: ResbtnVehicleonoffTooltipText, //'Hide/Show Vehicles',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {

            if (ifShowVehicleIcon) {
                ifShowVehicleIcon = false;
                btnVehicleonoff.setText(ResbtnVehicleonoffShowDetailsText/*'Show Details'*/);
            }
            else {
                ifShowVehicleIcon = true;
                btnVehicleonoff.setText(ResbtnVehicleonoffHideDetailsText/*'Hide Details'*/);
            }

            OriginIfShowVehicleIcon = ifShowVehicleIcon;

            redrawHistoryVehicleMarkers();
        }
        catch (err) {
        }
    }
}
);

    var btnHistorySendCommand = Ext.create('Ext.Button',
   {
       text: ResbtnHistorySendCMDText, //'Send Command',
       id: 'btnHistorySendCommand',
       tooltip: ResSendCMDHistoryRecords,
       iconCls: 'map',
       margin: '0 5',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               if (historyVehicles.getValue() != null || historyVehicles.getValue().trim() != "") {
                   var test = mainstore.findExact('VehicleId', historyVehicles.getValue(), 0);
                   var testvalue = mainstore.getAt(test).data.LicensePlate;
                   SensorInfoWindow(testvalue)
               }
           }
           catch (err) {

           }
       }
   });

    btnHistoryReplay = Ext.create('Ext.Button',
{
    text: 'Replay',
    id: 'labelHistoryReplay',
    //tooltip: Res_historyReplayTooltip, //'History Replay',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    margin: '0 5',
    handler: function () {
        try {

            if (ifShowVehicleIcon) {
                ifShowVehicleIcon = false;
                btnVehicleonoff.setText(ResbtnVehicleonoffShowDetailsText/*'Show Details'*/);

                redrawHistoryVehicleMarkers();
            }

            //               if (HistoryPlayPaused) {
            //                   HistoryPlayPaused = false;
            //                   btnHistoryReplay.setText(Res_historyReplayStop); // 'Stop'
            //               }
            //               else {
            //                   HistoryPlayPaused = true;
            //                   btnHistoryReplay.setText(Res_HistoryReplayReplay); // 'Replay'
            //               }

            HistoryPlayPaused = false;
            btnHistoryReplay.setDisabled(true);
            btnHistoryPlayPlay.hide();
            btnHistoryPlayPause.show();

            replayHistoryVehicleMarkers();

            replayPanel.show();
        }
        catch (err) {
        }
    }
}
);

    var btnHistoryGraph = Ext.create('Ext.Button',
{
    text: 'Graph',
    id: 'labelHistoryGraph',
    //tooltip: Res_historyReplayTooltip, //'History Replay',
    iconCls: 'map',
    cls: 'cmbfonts',
    textAlign: 'left',
    margin: '0 5',
    handler: function () {
        try {
            if (historyGraphWindow.isHidden())
                historyGraphWindow.show();
            else
                historyGraphWindow.hide();
        }
        catch (err) {
        }
    }
}
);


    var historyType_values = [
         [0, ResHistoryTypeValues_0 /*'Vehicle Path'*/],
         [1, ResHistoryTypeValues_1/*'Stop and Idle Sequence'*/],
         [2, ResHistoryTypeValues_2/*'Stop Sequence'*/],
         [3, ResHistoryTypeValues_3/*'Idle Sequence'*/],
         [4, ResHistoryTypeValues_4/*'Trip Report'*/]
    ];

    var historyType_store = new Ext.data.SimpleStore({
        fields: ['number', 'histype'],
        data: historyType_values
    });


    var historyType = new Ext.form.ComboBox({
        name: 'historyType',
        fieldLabel: ReshistoryTypefieldLabel, //'Type',
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
        emptyText: ReshistoryTypeemptyText, //'Choose number...',
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
                        Description: ResHistoryVehicleListDescription1//'Select a Vehicle'
                    });
                    xstore.insert(0, u);
                    //                       u = Ext.create('HistoryVehicleList', {
                    //                           VehicleId: '0',
                    //                           Description: ResHistoryVehicleListDescription2//'Entire Fleet'
                    //                       });
                    //                       xstore.insert(1, u);

                    if (historyIniVehicleId != '') {
                        historyVehicles.setValue(historyIniVehicleId);
                        historyIniVehicleId = '';
                    }

                    historyVehicleStoreLoaded = true;
                }
            },
            proxy:
               {
                   // load using HTTP
                   type: 'ajax',
                   url: './historynew/historyservices.aspx',
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

    historyVehicles = Ext.create('Ext.form.ComboBox',
    {
        name: 'historyVehicle',
        store: 'historyVehicleStore',
        displayField: ReshistoryVehiclesdisplayField, //'Description',
        valueField: 'VehicleId',
        typeAhead: true,
        fieldStyle: 'cmbfonts',
        labelCls: 'cmbLabel',
        queryMode: 'local',
        triggerAction: 'all',
        fieldLabel: ReshistoryVehiclesfieldLabel, //' Vehicle',
        emptyText: ReshistoryVehiclesEmptyText, //'Loading...',
        tooltip: ResSelectVehicles, //'Select a vehicles',
        selectOnFocus: true,
        width: 300,
        labelWidth: 50,
        editable: true,
        enableKeyEvents: true,
        lastQuery: '',
        listeners:
       {
           scope: this,
           'select': function (combo, value) {
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
           'keyup': function () {
               if (!historyVehicleStoreLoaded) {
                   if (LoadVehiclesBasedOn == 'fleet') {
                       historyVehicleStore.load(
                         {
                             params:
                                 {
                                     fleetID: SelectedFleetId
                                 }
                         });
                   }
                   else {
                       historyVehicleStore.load(
                             {
                                 params:
                                 {
                                     fleetId: DefaultOrganizationHierarchyFleetId
                                 }
                             });
                   }
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
                        CommModeName: ResHistoryCommModeNameText//'ALL'
                    });
                    xstore.insert(0, u);

                    historyCommModes.setValue('-1');
                }
            },
            proxy:
               {
                   // load using HTTP
                   type: 'ajax',
                   url: './historynew/historyservices.aspx?st=getcommmode',
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
        emptyText: ReshistoryCommModesEmptyText, //'Loading...',
        tooltip: ResSelectCommMode, //'Select a Comm Mode',
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
            boxLabel: ReshistoryMessageCheckBoxboxLabel, //'Last message only',
            name: 'lastmessageonly',
            inputValue: '1',
            id: 'checkbox1',
            border: 0
        }
    );

    historyMessageStore = Ext.create('Ext.data.Store',
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
                   url: './historynew/historyservices.aspx?st=GetMessageList',
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
             ddReorder: false
         }
     );

    var historyMessageForm = Ext.create('Ext.Panel', {
        title: ResMessages, //'Messages',
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
        fieldLabel: ResHistoryLocationText, //'Address',
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
        title: ResLocation, //'Location',
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
        title: ResTrip, //'Trip',
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
        items: [{
            xtype: 'component', html: ReshistoryTripRadioshtml, //'Calculate Trips based on:',
            cls: ''
        },
                 {
                     boxLabel: ReshistoryTripRadiosboxLabel1, //'Ignition',
                     name: 'historytrip',
                     inputValue: '3',
                     id: 'historytrip1',
                     checked: true
                 }, {
                     boxLabel: ReshistoryTripRadiosboxLabel2, // 'Tractor Power',
                     name: 'historytrip',
                     inputValue: '11',
                     id: 'historytrip2'
                 }, {
                     boxLabel: ReshistoryTripRadiosboxLabel3, // 'PTO',
                     name: 'historytrip',
                     inputValue: '8',
                     id: 'historytrip3'
                 }]
    });

    var btnSubmit = Ext.create('Ext.Button', {
        text: ResSubmitButtonText, //'View',
        cls: 'cmbfonts',
        //margin: '10 auto',
        style: { margin: '10px 0 10px 55px' },
        width: 100,
        handler: function () {
            try {
                if (historyVehicles.getValue() == null || historyVehicles.getValue() == '-1') {
                    Ext.Msg.alert('Oops', ResbtnSubmitMessage); //Ext.Msg.alert('Oops', 'Please select a vehicle...');
                    return;
                }
                //var form = this.up('form').getForm();
                var form = historyForm.getForm();
                var historytype = historyType.getValue();
                historyPageStore.currentPage = 1;
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

                    ResetHistoryReplay();
                    loadingMask.show();
                    form.submit({
                        url: './historynew/historyservices.aspx?st=gethistoryrecords&start=0&limit=' + HistoryPagesize,
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
                                    //Commented By Rohit
                                    // historyPager.moveFirst();
                                    historyPageStore.loadRawData(doc);
                                    historygrid.down('pagingtoolbar').bindStore(historygrid.getStore());
                                    historygrid.down('pagingtoolbar').onLoad();
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
                                    historygrid.down('pagingtoolbar').bindStore(historygrid.getStore());
                                    historygrid.down('pagingtoolbar').onLoad();
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
                            Ext.Msg.alert(ResbtnSubmitAlertTitle, action.result.msg ? action.result.msg : 'Unknown Error');
                        }
                    });
                }
            }
            catch (err) {
            }
        }
    });

    var txtButtonTitle = Ext.create('Ext.draw.Text', {
        text: LoadVehiclesBasedOn == 'fleet' ? ResTxtButtonTitleFleetText : ResTxtButtonTitleHierarchyText, //'Fleet:' : 'Hierarchy:',
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

    historyReplaySlider = Ext.create('Ext.slider.Single', {
        width: 214,
        minValue: 0,
        hideLabel: true,
        useTips: true,
        maxValue: 100,
        tipText: function (thumb) {
            var currentDateTime = GetHistoryCurrentDateTime(thumb.value);
            return Ext.String.format('{0}', Ext.Date.format(currentDateTime, userdateformat));
        },
        listeners: {
            changecomplete: function (slider, newValue, thumb, eOpts) {
                replayHistoryVehicleMarkers(newValue);
            }
        }
    });

    btnHistoryPlayPlay = Ext.create('Ext.Button',
    {
        text: '',
        id: 'labelHistoryPlayPlay',
        //tooltip: Res_historyReplayTooltip, //'History Replay',
        iconCls: 'history-icons replayPlay',
        overCls: 'history-icons-hover',
        hidden: true,
        cls: 'cmbfonts',
        textAlign: 'left',
        width: 22,
        margin: '0 0 0 10',
        handler: function () {
            try {
                btnHistoryPlayPlay.hide();
                btnHistoryPlayPause.show();
                HistoryPlayPaused = false;
                replayHistoryVehicleMarkers();

            }
            catch (err) {
            }
        }
    }
    );

    btnHistoryPlayPause = Ext.create('Ext.Button',
    {
        text: '',
        id: 'labelHistoryPlayPause',
        //tooltip: Res_historyReplayTooltip, //'History Replay',
        iconCls: 'history-icons replayPause',
        overCls: 'history-icons-hover',
        cls: 'cmbfonts',
        textAlign: 'left',
        width: 22,
        margin: '0 0 0 10',
        handler: function () {
            try {
                //               if (HistoryPlayPaused) {
                //                   HistoryPlayPaused = false;
                //                   btnHistoryReplay.setText(Res_historyReplayStop); // 'Stop'
                //               }
                //               else {
                //                   HistoryPlayPaused = true;
                //                   btnHistoryReplay.setText(Res_HistoryReplayReplay); // 'Replay'
                //               }

                btnHistoryPlayPlay.show();
                btnHistoryPlayPause.hide();
                HistoryPlayPaused = true;
                replayHistoryVehicleMarkers();

            }
            catch (err) {
            }
        }
    }
    );

    var historyPlaySpeedButtons = Ext.create('Ext.Container', {
        items: [
         {
             xtype: 'button',
             enableToggle: true,
             text: '1/4 x',
             toggleGroup: 'ratings',
             width: 39,
             margin: '0 2 0 10',
             allowDepress: false,
             handler: function () {
                 try {
                     setHistoryReplayDelayTime(4);
                 }
                 catch (err) {
                 }
             }
         }, {
             xtype: 'button',
             enableToggle: true,
             text: '1/2 x',
             toggleGroup: 'ratings',
             width: 39,
             margin: '0 2 0 2',
             allowDepress: false,
             handler: function () {
                 try {
                     setHistoryReplayDelayTime(2);
                 }
                 catch (err) {
                 }
             }
         }, {
             xtype: 'button',
             enableToggle: true,
             text: '1 x',
             toggleGroup: 'ratings',
             width: 39,
             margin: '0 2 0 2',
             pressed: true,
             allowDepress: false,
             handler: function () {
                 try {
                     setHistoryReplayDelayTime(1);
                 }
                 catch (err) {
                 }
             }
         }, {
             xtype: 'button',
             enableToggle: true,
             text: '2 x',
             toggleGroup: 'ratings',
             width: 39,
             margin: '0 2 0 2',
             allowDepress: false,
             handler: function () {
                 try {
                     setHistoryReplayDelayTime(0.5);
                 }
                 catch (err) {
                 }
             }
         }, {
             xtype: 'button',
             enableToggle: true,
             text: '4 x',
             toggleGroup: 'ratings',
             width: 39,
             margin: '0 2 0 2',
             allowDepress: false,
             handler: function () {
                 try {
                     setHistoryReplayDelayTime(0.25);
                 }
                 catch (err) {
                 }
             }
         }
        ]
    });

    txtHistoryReplayDelayTime = Ext.create('Ext.form.field.Text',
    {
        width: 60,
        margin: '0 0 0 20',
        readOnly: true,
        border: false,
        value: '200ms'
    });

    var btnHistoryPlayIncrease = Ext.create('Ext.Button',
    {
        text: '',
        id: 'labelHistoryPlayIncrease',
        //tooltip: Res_historyReplayTooltip, //'History Replay',
        iconCls: 'history-icons replayIncrease',
        overCls: 'history-icons-hover',
        cls: 'cmbfonts',
        textAlign: 'left',
        width: 22,
        margin: '0 0 0 0',
        handler: function () {
            try {

                //txtHistoryReplayDelayTime.setValue('250ms');
                adjustHistoryReplayDelayTime(50);
            }
            catch (err) {
            }
        }
    }
    );

    var btnHistoryPlayDecrease = Ext.create('Ext.Button',
    {
        text: '',
        id: 'labelHistoryPlayDecrease',
        //tooltip: Res_historyReplayTooltip, //'History Replay',
        iconCls: 'history-icons replayDecrease',
        overCls: 'history-icons-hover',
        cls: 'cmbfonts',
        textAlign: 'left',
        width: 22,
        margin: '0 0 0 0',
        handler: function () {
            try {

                //txtHistoryReplayDelayTime.setValue('150ms');
                adjustHistoryReplayDelayTime(-50);
            }
            catch (err) {
            }
        }
    }
    );

    var btnHistoryPlayClose = Ext.create('Ext.Button',
    {
        text: '',
        id: 'labelHistoryPlayClose',
        //tooltip: Res_historyReplayTooltip, //'History Replay',
        iconCls: 'history-icons replayClose',
        overCls: 'history-icons-hover',
        cls: 'cmbfonts',
        textAlign: 'left',
        width: 22,
        margin: '0 0 0 20',
        handler: function () {
            try {

                btnHistoryPlayPlay.show();
                btnHistoryPlayPause.hide();
                HistoryPlayPaused = true;
                replayHistoryVehicleMarkers();
                replayPanel.hide();
                btnHistoryReplay.setDisabled(false);
            }
            catch (err) {
            }
        }
    }
    );

    replayPanel = Ext.create('Ext.form.Panel', {
        title: '',
        labelWidth: 50, // label settings here cascade unless overridden
        frame: true,
        //bodyStyle: 'padding:5px 5px 0;margin:0 0 10px 0;',
        margin: '0 10 10 10',
        hidden: true,
        width: 430,//530,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: false,
        defaultType: 'textfield',
        items: [historyReplaySlider, btnHistoryPlayPlay, btnHistoryPlayPause, /*historyPlaySpeedButtons,*/txtHistoryReplayDelayTime, btnHistoryPlayIncrease, btnHistoryPlayDecrease, btnHistoryPlayClose]
    });

    var historyGraphWidth = window.screen.width - 70;

    historyGraphWindow = Ext.create('Ext.panel.Panel', {
        title: '',
        labelWidth: 50, // label settings here cascade unless overridden
        frame: true,
        //bodyStyle: 'padding:5px 5px 0;margin:0 0 10px 0;',
        margin: '0 10 10 10',
        hidden: true,
        height: 150,
        width: historyGraphWidth, //830,
        html: '<div id="divHistoryGraph"></div>',
        listeners: {
            afterrender: function () {
                try {
                    historyGraphWindow.setWidth(defaultMapView == 'west' ? (historyGraphWidth - northmappanel.getWidth()) : historyGraphWidth);
                }
                catch (err) { }
            }
           , beforeshow: function (p, eOpts) {
               if (dataHistoryGraphSpeed.length == 0) {
                   //initialise graph data
                   dataHistoryGraphSpeed = GetHistoryGraphData();
               }
               if (dataHistoryGraphRPM.length == 0) {
                   dataHistoryGraphRPM = GetHistoryGraphRPM();
               }
               if (dataHistoryGraphRoadSpeed.length == 0) {
                   dataHistoryGraphRoadSpeed = GetHistoryGraphRoadSpeed();
               }
               $(document).ready(function () {
                   var background = {
                       type: 'linearGradient',
                       x0: 0,
                       y0: 0,
                       x1: 0,
                       y1: 1,
                       colorStops: [{ offset: 0, color: '#d2e6c9' },
                          { offset: 1, color: 'white' }]
                   };

                   $('#divHistoryGraph').jqChart({
                       width: (defaultMapView == 'west' ? (historyGraphWidth - northmappanel.getWidth()) : historyGraphWidth), //830,
                       height: 150,
                       title: { text: 'Speed Graph', font: '12px' },
                       axes: [
                                 {
                                     type: 'category',
                                     location: 'bottom',
                                     zoomEnabled: true,
                                     labels: { visible: false }
                                 },
                                 {
                                     name: 'y1',
                                     location: 'left'
                                 },
                                 {
                                     name: 'y2',
                                     location: 'left'
                                 }
                       ],
                       border: { strokeStyle: '#6ba851' },
                       background: background,
                       tooltips: {
                           type: 'shared'
                       },
                       crosshairs: {
                           enabled: true,
                           hLine: false,
                           vLine: { strokeStyle: '#cc0a0c' }
                       },
                       series: [
                                     {
                                         type: 'line',
                                         title: 'Vehicle Speed',
                                         axisY: 'y1',
                                         data: dataHistoryGraphSpeed
                                     }
                                     , {
                                         type: 'line',
                                         title: 'RPM',
                                         axisY: 'y2',
                                         data: dataHistoryGraphRPM
                                     }
                                     , {
                                         type: 'line',
                                         title: 'Road Speed',
                                         axisY: 'y1',
                                         data: dataHistoryGraphRoadSpeed
                                     }
                       ]
                   });
               });
           }
        }

    });

    var historyForm = Ext.create('Ext.form.Panel', {
        title: '',
        labelWidth: 50, // label settings here cascade unless overridden
        url: './historynew/historyservices.aspx?st=gethistoryrecords',
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
        title: ResHistory, //'History',
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
        //Edited by Rohit Mittal for Trip Colors button
        items: [btnHistorySearch, btnHistoryMapit, btnHistoryMapAll, btnHistorySendCommand, ShowMultiColor ? historyTripColors : null, btnVehicleonoff, btnHistoryReplay, btnSubmit, btnHistoryGraph, btnHistoryLegend, replayPanel, historyGraphWindow, historyForm]
        , listeners: {
            'activate': function (grid, eOpts) {
                if (historyIniVehicleId != '') {
                    var testvalue;
                    Ext.each(mainstore.data.items, function (item, index) {
                        if (index < 0)
                            return false;
                        if (item.data.VehicleId == historyIniVehicleId) {
                            testvalue = item.data.Description
                            if (historyVehicles.getStore().findExact('VehicleId', historyIniVehicleId, 0) == -1) {
                                var u = Ext.create('HistoryVehicleList', {
                                    VehicleId: historyIniVehicleId,
                                    Description: testvalue
                                });
                                historyVehicles.getStore().insert(0, u);
                            }
                            return false;
                        }
                    });
                    if (historyVehicles.getStore().data.length <= 0) {
                        if (LoadVehiclesBasedOn == 'fleet') {
                            historyVehicleStore.load(
                              {
                                  params:
                                      {
                                          fleetID: SelectedFleetId
                                      }
                              });
                        }
                        else {
                            historyVehicleStore.load(
                                  {
                                      params:
                                      {
                                          fleetId: DefaultOrganizationHierarchyFleetId
                                      }
                                  });
                        }
                    }
                    historyVehicles.setValue(historyIniVehicleId);
                }
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
                        historyFleetButton.setText(ReshistoryFleetButtonsetText); //historyFleetButton.setText('All Vehicles');
                        historyHiddenFleet.setValue(historyAddressFleetId);
                    }
                    else {
                        historyOrganizationHierarchy.setText(ReshistoryFleetButtonsetText); //historyOrganizationHierarchy.setText('All Vehicles');
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

    var historyGridColumns = [
     {
         header: ReshistorygridBoxId, //'Unit ID',
         dataIndex: 'BoxId', align: 'left',
         width: 70,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistorygridDescription, // 'Vehicle',
         dataIndex: 'Description', align: 'left',
         width: 120,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistorygridOriginDateTime, //'Date/Time',
         dataIndex: 'OriginDateTime', align: 'left',
         align: 'left',
         width: 150,
         xtype: 'datecolumn',
         format: userdateformat, //'d/m/Y h:i:s a',
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false,
         tdCls: 'x-date-time',
         renderer: function (value, p, record) {
             return Ext.String.format('<div title="Received Date/Time: {0}" href="javascript:void(0);" style="width:100%;display:inline-block;overflow:hidden;" rel="bootstrapHoverPopover" data-html="true" data-container="body" data-content="{0}" data-html="true" data-title="Received Date/Time" data-placement="right" data-container="body">{1}</div>', Ext.Date.format(record.data.DateTimeReceived, userdateformat/*'d/m/Y h:i:s a'*/), Ext.Date.format(value, userdateformat/*'d/m/Y h:i:s a'*/));
         }
     },
     {
         header: ReshistorygridStreetAddress, //'Address',
         dataIndex: 'StreetAddress', align: 'left',
         align: 'left',
         width: 230,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false,
         renderer: function (value, p, record) {
             if (record.data.ValidGps * 1 == 0 || record.data.ValidGps * 1 == 1)
                 return value;
             else
                 return '---';
         }
     },
     {
         header: 'Lat, Lon',
         dataIndex: 'LatLon', align: 'left',
         align: 'left',
         width: 130,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: true/*,
            renderer: function (value, p, record) {
                if (record.data.ValidGps * 1 == 0)
                    return Ext.String.format('{0}, {1}', record.data.Latitude, record.data.Longitude);
                else
                    return ResTextNA;
            }*/
     },
     {
         header: ResValidGPS,//'Valid GPS',
         dataIndex: 'ValidGps', align: 'left',
         align: 'left',
         width: 60,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: true,
         renderer: function (value, p, record) {
             if (value * 1 == 0)
                 return ResTextTrue;//'true';
             else if (value * 1 == 1)
                 return ResTextFalse;//'false';
             else
                 return ResTextNA;//'N/A';
         }
     },
     {
         header: ReshistorygridSpeed, //'Speed',
         dataIndex: 'Speed', align: 'left',
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
         header: ReshistorygridBoxMsgInTypeName, //'Message',
         dataIndex: 'BoxMsgInTypeName', align: 'left',
         renderer: function (value, p, record, ri) {
             //return '<a href="javascript:var w =HistoryInfo(' + ri + ')">' + value + '</a>';
             var r = '<a href="' + record.data.CustomUrl + '" style="float:left;">' + value + '</a>';
             if (record.data.MsgDetails.indexOf("Video Available") >= 0)
                 r += " <a style='float:right;' href='javascript:void(0)' onclick='VideoViewer(" + record.data.BoxId + ", \"" + Ext.Date.format(record.data.OriginDateTime, userdateformat) + "\" )'>V</a>";
             return r;
         },
         align: 'left',
         width: 130,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         //header: 'MsgDetails', dataIndex: 'CustomProp', align: 'left',
         header: ReshistorygridMsgDetails, //'MsgDetails',
         dataIndex: 'MsgDetails', align: 'left',
         align: 'left',
         width: 130,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false,
         renderer: function (value, p, record) {
             if (HGI)
                 return Ext.String.format('<div href="javascript:void(0);" style="width:100%;display:inline-block;overflow:hidden;" rel="bootstrapHoverPopover" data-html="true" data-container="body" data-content="[{0}]" data-html="true" data-title="Custom Prop" data-placement="right" data-container="body">{1}</div>', record.data.CustomProp, value);
             else
                 return value;
         }
     },
     {
         header: ResAcknowledged, //'Ack',
         dataIndex: 'Acknowledged', align: 'left',
         align: 'left',
         width: 50,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },

             {
                 id: 'vgHisDriver',
                 text: 'Driver Name',
                 align: 'left',
                 width: 90,
                 dataIndex: 'Driver',
                 filterable: true,
                 sortable: true
             },
             {
                 id: 'vgHisDriverHIDCard',
                 text: 'Driver ID',
                 align: 'left',
                 width: 130,
                 dataIndex: 'DriverHIDCard',
                 filterable: true,
                 sortable: true
             },
            {
                id: 'vgHisRPM',
                text: 'RPM',
                align: 'right',
                width: 60,
                dataIndex: 'RPM',
                filterable: true,
                sortable: true
            },
            {
                id: 'vgHisPTO',
                text: 'PTO',
                align: 'left',
                width: 40,
                dataIndex: 'PTO',
                filterable: true,
                sortable: true
            }


    ];

    // 'CP_Fuel', 'CP_Odometer', 'CP_RPM', 'CP_FLIP', 'CP_FLIS', 'CP_SeatBelt', 'CP_MIL', 'CP_CLT', 'CP_EOT', 'CP_EOP'
    if (ShowHistoryDetails) {
        historyGridColumns.push(
             {
                 id: 'vgHisCPFuel',
                 text: 'Fuel',
                 align: 'right',
                 width: 60,
                 dataIndex: 'CP_Fuel',
                 filterable: true,
                 sortable: true
             },
            {
                id: 'vgHisCPOdometer',
                text: 'Odometer',
                align: 'right',
                width: 70,
                dataIndex: 'CP_Odometer',
                filterable: true,
                sortable: true
            },
        //               {
        //                   id: 'vgHisCPRPM',
        //                   text: 'RPM',
        //                   align: 'right',
        //                   width: 60,
        //                   dataIndex: 'CP_RPM',
        //                   filterable: true,
        //                   sortable: true
        //               },
            {
                id: 'vgHisCPFLIP',
                text: 'Fuel Level P. (%)',
                align: 'right',
                width: 100,
                dataIndex: 'CP_FLIP',
                filterable: true,
                sortable: true
            },
            {
                id: 'vgHisCPFLIS',
                text: 'Fuel Level S. (%)',
                align: 'right',
                width: 100,
                dataIndex: 'CP_FLIS',
                filterable: true,
                sortable: true
            },
            {
                id: 'vgHisCPCLT',
                text: 'Coolant Temp. (&#176C)',
                align: 'right',
                width: 100,
                dataIndex: 'CP_CLT',
                filterable: true,
                sortable: true
            },
            {
                id: 'vgHisCPEOT',
                text: 'Eng Oil Temp. (&#176C)',
                align: 'right',
                width: 110,
                dataIndex: 'CP_EOT',
                filterable: true,
                sortable: true
            },
            {
                id: 'vgHisCPEOP',
                text: 'Eng Oil Pres (kPa)',
                align: 'right',
                width: 110,
                dataIndex: 'CP_EOP',
                filterable: true,
                sortable: true
            },
            {
                id: 'vgHisCPSeatBelt',
                text: 'Seat Belt',
                align: 'left',
                width: 90,
                dataIndex: 'CP_SeatBelt',
                filterable: true,
                sortable: true
            },
            {
                id: 'vgHisCPMIL',
                text: 'MIL',
                align: 'left',
                width: 90,
                dataIndex: 'CP_MIL',
                filterable: true,
                sortable: true
            }
        );
    }

    var hiscurrentFilters = new Object();
    var hisfilteron = false;
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
    enableColumnHide: true,
    enableSorting: true,
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
    columns: historyGridColumns,
    viewConfig:
   {
       emptyText: ReshistorygridEmptyText, //'No History to display',
       useMsg: false,
       getRowClass: function (record, index) {
           var d = record.get('TimeDifference') * 1;    // hours
           //alert(d);
           if (d > 0)
               return 'latereceived';
           else
               return '';
       }
   },
    dockedItems: {
        xtype: 'toolbar',
        dock: 'top',
        items: [
     {
         icon: 'preview.png',
         cls: 'x-btn-text-icon',
         text: ResExport, //'Export',
         menu: historyExportMenu
     }
        ]
    },
    listeners: {
        //Added by Rohit Mittal
        filterupdate: function () {
            historygrid.filters.deferredUpdate.cancel();
            var filtersdata;
            var stringvalue;
            var tempStore = Ext.create('Ext.data.Store', {
                model: 'HistoryListModel',
                pageSize: HistoryPagesize,
                proxy: {
                    type: 'ajax',
                    url: './historynew/historyservices.aspx',
                    reader: {
                        type: 'xml',
                        root: 'MsgInHistory',
                        record: 'VehicleStatusHistory',
                        totalProperty: 'totalCount'
                    }
                }
            });
            if (typeof (historygrid) != 'undefined' && historygrid.filters.filters.length > 0) {
                filtersdata = historygrid.filters.getMenuFilter();
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
                                hisfilteron = true;
                                hiscurrentFilters[filtercolvalue] = filtercolvalue + ":" + stringvalue;
                                historyPageStore.currentPage = 1;
                                loadingMask.show();
                                tempStore.load({
                                    params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam,
                                   filters: hiscurrentFilters
                               },
                                    callback: function (records, operation, success) {
                                        historygrid.getStore().loadRecords(records);
                                        historygrid.down('pagingtoolbar').bindStore(tempStore);
                                        historygrid.down('pagingtoolbar').onLoad();
                                        loadingMask.hide();
                                    },
                                    scope: this
                                });
                            }
                            else {
                                delete hiscurrentFilters[filtercolvalue];
                                if (Object.keys(hiscurrentFilters).length == 0) {
                                    hisfilteron = false;
                                    tempStore.currentPage = hiscurrentpage;
                                    loadingMask.show();
                                    tempStore.load({
                                        params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam
                               },
                                        callback: function (records, operation, success) {
                                            historygrid.getStore().loadRecords(records);
                                            historygrid.down('pagingtoolbar').bindStore(tempStore);
                                            historygrid.down('pagingtoolbar').onLoad();
                                            loadingMask.hide();
                                        },
                                        scope: this
                                    });
                                }
                                else {
                                    loadingMask.show();
                                    tempStore.load({
                                        params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam,
                                   filters: hiscurrentFilters
                               },
                                        callback: function (records, operation, success) {
                                            historygrid.getStore().loadRecords(records);
                                            historygrid.down('pagingtoolbar').bindStore(tempStore);
                                            historygrid.down('pagingtoolbar').onLoad();
                                            loadingMask.hide();
                                        },
                                        scope: this
                                    });
                                }
                            }
                        }
                        else {
                            delete hiscurrentFilters[filtercolvalue];
                            if (Object.keys(hiscurrentFilters).length == 0) {
                                hisfilteron = false;
                                tempStore.currentPage = hiscurrentpage;
                                loadingMask.show();
                                tempStore.load({
                                    params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam
                               },
                                    callback: function (records, operation, success) {
                                        historygrid.getStore().loadRecords(records);
                                        historygrid.down('pagingtoolbar').bindStore(tempStore);
                                        historygrid.down('pagingtoolbar').onLoad();
                                        loadingMask.hide();
                                    },
                                    scope: this
                                });
                            }
                            else {
                                loadingMask.show();
                                tempStore.load({
                                    params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam,
                                   filters: hiscurrentFilters
                               },
                                    callback: function (records, operation, success) {
                                        historygrid.getStore().loadRecords(records);
                                        historygrid.down('pagingtoolbar').bindStore(tempStore);
                                        historygrid.down('pagingtoolbar').onLoad();
                                        loadingMask.hide();
                                    },
                                    scope: this
                                });
                            }
                        }
                    }
                    else {
                        delete hiscurrentFilters[filtercolvalue];
                        if (Object.keys(hiscurrentFilters).length == 0) {
                            hisfilteron = false;
                            tempStore.currentPage = hiscurrentpage;
                            loadingMask.show();
                            tempStore.load({
                                params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam
                               },
                                callback: function (records, operation, success) {
                                    historygrid.getStore().loadRecords(records);
                                    historygrid.down('pagingtoolbar').bindStore(tempStore);
                                    historygrid.down('pagingtoolbar').onLoad();
                                    loadingMask.hide();
                                },
                                scope: this
                            });
                        }
                        else {
                            loadingMask.show();
                            tempStore.load({
                                params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam,
                                   filters: hiscurrentFilters
                               },
                                callback: function (records, operation, success) {
                                    historygrid.getStore().loadRecords(records);
                                    historygrid.down('pagingtoolbar').bindStore(tempStore);
                                    historygrid.down('pagingtoolbar').onLoad();
                                    loadingMask.hide();
                                },
                                scope: this
                            });
                        }
                    }

                }
                else {
                    hiscurrentFilters = {};
                    hisfilteron = false;
                    tempStore.currentPage = hiscurrentpage;
                    loadingMask.show();
                    tempStore.load({
                        params:
                               {
                                   st: 'GetFilteredRecord',
                                   sorting: hissortingParam

                               },
                        callback: function (records, operation, success) {
                            historygrid.getStore().loadRecords(records);
                            historygrid.down('pagingtoolbar').bindStore(tempStore);
                            historygrid.down('pagingtoolbar').onLoad();
                            loadingMask.hide();
                        },
                        scope: this
                    });
                }
            }
        }
    },
    bbar: historyPager
});

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
       emptyText: ReshistoryStopGridEmptyText, //'No History to display',
       useMsg: false
   }
   ,
    columns: [
     {
         header: ReshistoryStopGridArrivalDateTime, //'Arrival',
         dataIndex: 'ArrivalDateTime', align: 'left',
         xtype: 'datecolumn',
         format: userdateformat, //'d/m/Y h:i:s a',
         width: 150,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryStopGridLocation, //'Address',
         dataIndex: 'Location',
         align: 'left',
         width: 220,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryStopGridDepartureDateTime, //'Departure',
         dataIndex: 'DepartureDateTime', align: 'left',
         xtype: 'datecolumn',
         format: userdateformat, //'d/m/Y h:i:s a',
         width: 150,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryStopGridStopDuration, //'Duration',
         dataIndex: 'StopDuration',
         align: 'left',
         width: 80,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryStopGridRemarks, //'Status',
         dataIndex: 'Remarks',
         align: 'left',
         width: 80,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: 'Driver Name',
         dataIndex: 'Driver',
         align: 'left',
         width: 80,
         filterable: true,
         sortable: true,
         // flex : 1,
         hidden: false
     },
     {
         header: 'Driver ID',
         dataIndex: 'DriverHIDCard',
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
     {
         icon: 'preview.png',
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
       emptyText: ReshistoryTripGridEmptyText, //'No History to display',
       useMsg: false
   }
   ,
    columns: [
     {
         header: ReshistoryTripGridDescription, //'Description',
         dataIndex: 'Description', align: 'left',
         width: 150,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryTripGridDepartureTime, //'Departure',
         dataIndex: 'DepartureTime', align: 'left',
         xtype: 'datecolumn',
         format: userdateformat, //'d/m/Y h:i:s a',
         width: 150,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryTripGridArrivalTime, //'Arrival',
         dataIndex: 'ArrivalTime', align: 'left',
         xtype: 'datecolumn',
         format: userdateformat, //'d/m/Y h:i:s a',
         width: 150,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryTripGrid_From, //'From',
         dataIndex: '_From', align: 'left',
         width: 200,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryTripGrid_To, //'To',
         dataIndex: '_To', align: 'left',
         width: 200,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryTripGridDuration, //'Duration',
         dataIndex: 'Duration',
         align: 'left',
         width: 80,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: ReshistoryTripGridFuelConsumed, //'Fuel Consumed',
         dataIndex: 'FuelConsumed',
         align: 'left',
         width: 80,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: 'Driver Name',
         dataIndex: 'Driver',
         align: 'left',
         width: 80,
         filterable: true,
         sortable: false,
         // flex : 1,
         hidden: false
     },
     {
         header: 'Driver ID',
         dataIndex: 'DriverHIDCard',
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
     {
         icon: 'preview.png',
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
    title: ResSearchResult, //'Search Result',
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
               text: ReshistoryAddressGridhaUnitID, //'UnitID',
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
               text: ReshistoryAddressGridhaDescription, //'Description',
               align: 'left',
               width: 150,
               dataIndex: 'Description',
               filterable: true,
               sortable: true
           },

           {
               id: 'haDateTime',
               //stateId: 'stDateTime',
               text: ReshistoryAddressGridhaDateTime, //'Date/Time',
               align: 'left',
               width: 135,
               xtype: 'datecolumn',
               format: userdateformat, //dateformat,
               dataIndex: 'OriginDateTime',
               filterable: false,
               sortable: false,
               tdCls: 'x-date-time'
           }
           , {
               id: 'haDetails',
               text: ReshistoryAddressGridhaDetails, //'Details',
               align: 'left',
               width: 90,
               dataIndex: 'VehicleId',
               renderer: function (value, p, record) {
                   return '<a href="javascript:void(0);" OnClick="historyMessageStore.load();showHistoryTab(\'' + value + '\', true, ' + record.data.FleetId + ');">Details</a>';
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
       emptyText: ResVehiclePagerEmptyMsg, //'No vehicles to display',
       useMsg: false
   },
    dockedItems: {
        xtype: 'toolbar',
        dock: 'top',
        items: [
     {
         icon: 'preview.png',
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
               {
                   name: 'OriginDateTime', type: 'date', dateFormat: 'c'
               },
               //'OriginDateTime',
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
               emptyText: ReshidtoryDetailsGridEmptyText, //'No History to display',
               useMsg: false
           }
           ,
            columns: [
             {
                 header: ReshidtoryDetailsGridBoxId, //'Unit ID',
                 dataIndex: 'BoxId', align: 'left',
                 width: 60,
                 filterable: true,
                 sortable: true,
                 // flex : 1,
                 hidden: false
             },
             {
                 header: ReshidtoryDetailsGridDescription, //'Vehicle',
                 dataIndex: 'Description', align: 'left',
                 width: 120,
                 filterable: true,
                 sortable: true,
                 // flex : 1,
                 hidden: false
             },
             {
                 header: ReshidtoryDetailsGridOriginDateTime, //'Date/Time',
                 dataIndex: 'OriginDateTime', align: 'left',
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
                 header: ReshidtoryDetailsGridStreetAddress, //'Address',
                 dataIndex: 'StreetAddress', align: 'left',
                 align: 'left',
                 width: 280,
                 filterable: true,
                 sortable: true,
                 // flex : 1,
                 hidden: false
             },
             {
                 header: ReshidtoryDetailsGridSpeed, //'Speed',
                 dataIndex: 'Speed', align: 'left',
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
                 header: ReshidtoryDetailsGridBoxMsgInTypeName, //'Message',
                 dataIndex: 'BoxMsgInTypeName', align: 'left',
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
                 header: ReshidtoryDetailsGridMsgDetails, //'MsgDetails',
                 dataIndex: 'MsgDetails', align: 'left',
                 align: 'left',
                 width: 110,
                 filterable: true,
                 sortable: true,
                 // flex : 1,
                 hidden: false
             },
             {
                 header: ReshidtoryDetailsGridAcknowledged, //'Ack',
                 dataIndex: 'Acknowledged', align: 'left',
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
            url: './historynew/historyservices.aspx?fromsession=1&st=gettripdetails&tripId=' + renderId,
            success: function (form, action) {
                historyDetailsGrid.getView().emptyText = ReshistoryFormEmptyText; //'No History to display';
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
                historygrid.getView().emptyText = ReshistoryFormEmptyText, //'No History to display';
                //Ext.Msg.alert(ReshistoryFormAlertTitle, ReshistoryFormAlertMessage); //Ext.Msg.alert('Failed', 'some error');
     Ext.Msg.alert(ReshistoryFormAlertTitle, "Too much data requested. Please reduce your search criteria."); //Ext.Msg.alert('Failed', 'some error');
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

    var cookieheight = readCookie(DispatchOrganizationId + 'mapGridDefaultHeight');
    var cookiewidth = readCookie(DispatchOrganizationId + 'mapGridDefaultWidth');
    cookiedefaultmapview = readCookie(DispatchOrganizationId + 'DefaultMapView');

    var hideGridDefault = false;
    if (cookiedefaultmapview != '') {
        if (cookiedefaultmapview == 'normal')
            hideMapByDefault = false;
        else if (cookiedefaultmapview == 'FullGridView') {
            hideMapByDefault = true;
            mapStatus = 'collapse';
        }
        else if (cookiedefaultmapview == 'FullMapView') {
            hideGridDefault = true;
            hideMapByDefault = false;
            gridStatus = 'collapse';
        }
    }

    mapGridOriginalHeight = cookieheight != '' ? cookieheight * 1 : getDocHeight() - (window.screen.height / 2);
    mapGridOriginalWidth = cookiewidth != '' ? cookiewidth * 1 : getDocWidth() - (window.screen.width / 2);

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
    //height: hideMapByDefault ? $(document).height() : mapGridOriginalHeight, //getDocHeight() - (window.screen.height / 2),
    //width: hideMapByDefault ? $(document).width() : mapGridOriginalWidth, //getDocWidth() - (window.screen.width / 2),
    height: hideMapByDefault ? $(document).height() : (hideGridDefault ? (defaultMapView == 'west' ? mapGridOriginalHeight : 25) : mapGridOriginalHeight),
    width: hideMapByDefault ? $(document).width() : (hideGridDefault ? (defaultMapView == 'west' ? 1 : mapGridOriginalWidth) : mapGridOriginalWidth),
    autoHeight: true,
    collapsible: true,
    animCollapse: true,
    deferredRender: false,
    activeTab: 0,     // first tab initially active
    autoDestroy: false,
    items: [vehiclegrid, ShowAlarmTab ? alarmgrid : null, ShowMessageTab ? messagegrid : null, geozonelandmarktabs],
    listeners:
   {
       afterrender: function () {
           try {
               $('.x-collapse-el').hide();
               var s = '<div class=\'togglemap\'><a href=\'javascript:void(0);\' onclick="toggleMap();"><img src=\'images/menutogglemaphandler.png\'></a></div>';
               $('#tabs').parent().prepend(s);
               s = '<div class=\'collapsebutton\'><a href=\'javascript:void(0);\' onclick="toggleGrid();"><img src=\'images/menuhandler.png\' alt=\'hide/show grid\' title=\'hide/show grid\'></a></div>';
               $('#tabs').parent().prepend(s);
               //s = '<div class=\'expandgridbutton\'><a href=\'javascript:void(0);\' onclick="toggleMap();"><img src=\'images/menumaphandler.png\' style=\'border:1px solid #666666\' alt=\'hide/show map\' title=\'hide/show map\'></a></div>';
               //$('#tabs').parent().prepend(s);
               $('#tabs-body').css('overflow', 'hidden');

               /*if (defaultMapView == 'west') {
               $('.collapsebutton').css("top", 7);
               $('.collapsebutton').css("left", tabs.getWidth() - 50);
               }
               else if (defaultMapView == 'south') {
               $('.collapsebutton').css("top", northmappanel.getHeight() + 10);
               $('.collapsebutton').css("left", tabs.getWidth() - 50);
               }
               else if (defaultMapView == 'north') {
               $('.collapsebutton').css("top", 7);
               $('.collapsebutton').css("left", tabs.getWidth() - 50);
               }*/

               //if (ViolationTabAtNewMapPage)
               //    IniViolations();
           }
           catch (err) { alert(err); }
       },
       resize: function (o, width, height, oldWidth, oldHeight, eOpts) {
           if (!SaveGridSizeToCookie) {
               SaveGridSizeToCookie = true;
               return;
           }

           var CookieDate = new Date;
           CookieDate.setFullYear(CookieDate.getFullYear() + 1);
           if (defaultMapView == 'west' || defaultMapView == 'east')
               document.cookie = DispatchOrganizationId + 'mapGridDefaultWidth=' + width + '; expires=' + CookieDate.toGMTString() + ';';
           else
               document.cookie = DispatchOrganizationId + 'mapGridDefaultHeight=' + height + '; expires=' + CookieDate.toGMTString() + ';';

           document.cookie = DispatchOrganizationId + 'DefaultMapView=normal; expires=' + CookieDate.toGMTString() + ';';

           cookiedefaultmapview = readCookie(DispatchOrganizationId + 'DefaultMapView');
           gridStatus = 'expanded';
           mapStatus = 'expanded';
       }
   }

}
);

    //Devin Added
    try {
        if (DispatchOrganizationId == 480) {
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
                title: ResDispatch, //'Dispatch',
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

                        window.open('../../Ant/Ant.html', ResDispatch, 'width=' + ant_w + ',height=' + ant_h + ',left=' + ant_left + ',top=' + ant_top + ',screenX=0,screenY=100');
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

               if (cookiedefaultmapview == 'FullMapView') {
                   $('.collapsebutton').css("left", 30);
                   $('.collapsebutton').css("top", 0);

                   $('.togglemap').css("left", 0);
                   $('.togglemap').css("top", 0);
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
            geolandmarkgrid.setTitle(ResGeozoneLandmarks);
            $('#geolandmarkcount').html(landmarks.length);
        }
        else {
            setTimeout(function () { loadGeozoneLandmarks(); }, 500);
        }
    }

    loadingMask.show();

    function getIcon(exirecord, posExpireDate) {
        var newIcon;
        if (exirecord.data.ImagePath != "" && exirecord.data.ImagePath != null) {    // custom icon
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
                if (exirecord.data.VehicleStatus.indexOf("Idling") > -1 || exirecord.data.VehicleStatus.indexOf("Moteur au ralenti") > -1) {
                    newIcon = "Orange";
                }
                if (exirecord.data.PTO == 'On') {
                    newIcon = "Blue";
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
                if (exirecord.data.VehicleStatus.indexOf("Idling") > -1 || exirecord.data.VehicleStatus.indexOf("Moteur au ralenti") > -1) {
                    newIcon = "Orange" + exirecord.data.IconTypeName + ".ico";
                }
                if (exirecord.data.PTO == 'On') {
                    newIcon = "Blue" + exirecord.data.IconTypeName + ".ico";
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
                d = {
                    'type': ss[0], 'name': ss[1], 'f': el.geoLandmarkFeatures[i], 'desc': el.geoLandmarkFeatures[i].GeoDescription, 'direction': el.geoLandmarkFeatures[i].GeoDirection,
                    'SeverityName': el.geoLandmarkFeatures[i].SeverityName,
                    'id': el.geoLandmarkFeatures[i].GeozoneID
                };
                geozones.push(d);
            }
        }
        geozonesstore.loadData(geozones);
        //geolandmarksstore.loadData(geolandmarksdata);

        geozonegrid.setTitle(ResGeozones/*"Geozones"*/);

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
                d = {
                    'type': ss[0], 'name': ss[1], 'f': el.geoLandmarkFeatures[i],
                    'desc': el.geoLandmarkFeatures[i].landmarkDescription,
                    'StreetAddress': el.geoLandmarkFeatures[i].StreetAddress,
                    'Email': el.geoLandmarkFeatures[i].Email,
                    'ContactPhoneNum': el.geoLandmarkFeatures[i].ContactPhoneNum,
                    'radius': el.geoLandmarkFeatures[i].radius,
                    'CategoryName': el.geoLandmarkFeatures[i].CategoryName
                };
                landmarks.push(d);
            }
        }
        landmarksstore.loadRawData(landmarks);
        //geolandmarksstore.loadData(geolandmarksdata);

        landmarkgrid.setTitle(ResLandmarks/*"Landmarks"*/);

        //$('#landmarkcount').html(landmarks.length);
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
            if (clearall)
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
        var nd = Ext.Date.parse(SearchHistoryDateTime, userDate + ' H:i'); //new Date(d[2], parseInt(d[0]) - 1, d[1], t[0], t[1]);
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
    historyGridForm.doLayout();
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime
    if (userTime == "h:i:s A") {
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12;
        strTime = hours + ':' + minutes + ' ' + ampm;
    }
    else
        strTime = hours + ':' + minutes;
    return strTime;
}




function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName) {

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
    catch (err) {
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

function OnFleetSelect(c, fleetId, fleetName, caller) {
    fleetWin.hide();
    if (c) {
        if (caller == 'fleetButton') {
            fleetButton.setText(fleetName);

            SelectedFleetId = fleetId;
            SelectedFleetName = fleetName;
            var el = document.getElementById(mapframe).contentWindow;
            if (typeof el.allVehicles != "undefined")
                el.allVehicles.length = 0;
            el.markers.removeAllFeatures();
            if (el.parent.VehicleClustering) {
                el.markerstrategy.clearCache();
            }
            el.vehicleFeatures = [];
            slectedboxidarray.length = 0;
            mainstore.currentPage = 1;

            var mergeData = '';
            var cat_id = 0;
            if (currentView == 'dashboard') {
                cat_id = landmarkCategory.getValue();
                mergeData = 'VehiclesInLandmarks';
            }

            loadingMask.show();

            mainstore.load(
                {
                    params:
                    {
                        QueryType: 'GetfleetPosition',
                        fleetID: fleetId,
                        start: 0,
                        limit: VehicleListPagesize,
                        mergeData: mergeData,
                        landmarkCategoryId: cat_id
                    }
                });
            vehiclegrid.down('pagingtoolbar').bindStore(vehiclegrid.getStore());
            vehiclegrid.down('pagingtoolbar').onLoad();
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
                    },
                    callback: function (records, operation, success) {
                        historyVehicles.setValue('-1');
                    }
                }
            );
        }
    }

}

function toggleGrid() {
    SaveGridSizeToCookie = false;
    mapStatus = 'expanded';
    if (defaultMapView == 'west') {

        if (gridStatus == 'expanded') {
            gridStatus = 'collapse';
            if (cookiedefaultmapview != 'FullGridView')
                mapGridOriginalWidth = tabs.getWidth();
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
            tabs.setWidth(mapGridOriginalWidth);
            $('.collapsebutton').css("left", tabs.getWidth() - 50);
            $('.collapsebutton').css("top", 5);

            $('.togglemap').css("left", tabs.getWidth() - 80);
            $('.togglemap').css("top", 5);
        }
    }
    else {

        if (gridStatus == 'expanded') {
            gridStatus = 'collapse';
            if (cookiedefaultmapview != 'FullGridView')
                mapGridOriginalHeight = tabs.getHeight();

            tabs.setHeight(25);
        }
        else {
            gridStatus = 'expanded';
            tabs.setHeight(mapGridOriginalHeight);
        }
    }

    var CookieDate = new Date;
    CookieDate.setFullYear(CookieDate.getFullYear() + 1);

    if (gridStatus == 'expanded') {
        document.cookie = DispatchOrganizationId + 'DefaultMapView=normal; expires=' + CookieDate.toGMTString() + ';';
        cookiedefaultmapview = "normal";
    }
    else {
        document.cookie = DispatchOrganizationId + 'DefaultMapView=FullMapView; expires=' + CookieDate.toGMTString() + ';';
        cookiedefaultmapview = "FullMapView";
    }
}

function toggleMap() {
    SaveGridSizeToCookie = false;
    gridStatus = 'expanded';
    if (defaultMapView == 'west') {
        if (mapStatus == 'expanded') {
            mapStatus = 'collapse';
            if (cookiedefaultmapview != 'FullMapView')
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
            if (cookiedefaultmapview != 'FullMapView')
                mapGridOriginalHeight = tabs.getHeight();
            tabs.setHeight($(document).height());
        }
        else {
            mapStatus = 'expanded';
            tabs.setHeight(mapGridOriginalHeight);
        }
        //tabs.setHeight($(document).height());
    }

    var CookieDate = new Date;
    CookieDate.setFullYear(CookieDate.getFullYear() + 1);
    if (mapStatus == 'expanded') {
        document.cookie = DispatchOrganizationId + 'DefaultMapView=normal; expires=' + CookieDate.toGMTString() + ';';
        cookiedefaultmapview = "normal";
    }
    else {
        document.cookie = DispatchOrganizationId + 'DefaultMapView=FullGridView; expires=' + CookieDate.toGMTString() + ';';
        cookiedefaultmapview = "FullGridView";
    }
}

var popupWindow;

function openPopupWindow(wintitle, winURL, winWidth, winHeight) {
    try {
        popupWindow.close();
    }
    catch (err) { }
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
        fleetId = DefaultFleetID;
    }
    else {
        fleetId = DefaultOrganizationHierarchyFleetId;
    }

    loadingMask.show();
    FromClosestVehicles = true;
    VehicleGridInSearchMode = true;
    originSelectionon = selectionon;
    mainstore.load(
    {
        params:
        {
            QueryType: 'getClosestVehicles',
            fleetID: fleetId,
            lon: lon,
            lat: lat,
            radius: radius,
            numofvehicles: numofvehicles
        }
    });

    clearSearchBtn.show();

}

function getVehiclesInLandmark(landmarkId) {
    //alert(lon + ',' + lat);
    var fleetId;
    if (LoadVehiclesBasedOn == 'fleet') {
        fleetId = DefaultFleetID;
    }
    else {
        fleetId = DefaultOrganizationHierarchyFleetId;
    }

    loadingMask.show();
    FromClosestVehicles = true;
    VehicleGridInSearchMode = true;
    originSelectionon = selectionon;
    mainstore.load(
    {
        params:
        {
            QueryType: 'getVehiclesInLandmark',
            landmarkId: landmarkId,
            fleetID: fleetId
        }
    });

    clearSearchBtn.show();

}

var SearchHistoryDateTime;
var SearchHistoryTimeRange;

function searchHistoryAddress(lon, lat, searchDateTime, radius, minutes, mapSearchPointSets, SearchHistoryBy) {
    if (SearchHistoryBy == undefined)
        SearchHistoryBy = 0;
    var SearchFleetId = '';
    if (SearchHistoryBy == 1)
        SearchFleetId = (LoadVehiclesBasedOn == 'fleet') ? SelectedFleetId : DefaultOrganizationHierarchyFleetId;
    var SearchBoxIds = '';
    if (SearchHistoryBy == 2) {
        var currentTicked = vehiclegrid.getSelectionModel().getSelection();
        Ext.each(currentTicked, function (selectedRec, i) {
            if (SearchBoxIds != '') SearchBoxIds = SearchBoxIds + ',';
            SearchBoxIds = SearchBoxIds + selectedRec.data.BoxId;
        });
    }

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
            mapSearchPointSets: mapSearchPointSets,
            FleetIds: SearchFleetId,
            BoxIds: SearchBoxIds
        }
    });
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

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return '';
}

function ResolveAddress(o, boxId, lat, lon) {
    var thiscell = $(o).parent();
    thiscell.html(ResResolving);

    $.ajax({
        type: 'GET',
        url: 'Vehicles.aspx?QueryType=resolveaddress&boxId=' + boxId + '&lat=' + lat + '&lon=' + lon,
        dataType: 'text',
        async: true,
        success: function (msg) {
            thiscell.html(msg);
        },
        error: function (msg) {

        }
    });

}

function gotoLandmark(landmarkId) {
    //landmarkName = landmarkName.split(' || ')[0];
    var el = document.getElementById(mapframe).contentWindow;

    if (el.landmarkLoaded) {

        for (i = 0; i < el.geoLandmarkFeatures.length; i++) {
            if (el.geoLandmarkFeatures[i].LandmarkId == landmarkId) {
                el.map.zoomToExtent(el.geoLandmarkFeatures[i].geometry.getBounds(), closest = true);
                getVehilcesByLandmarkIdAndMapIt(landmarkId);
                break;
            }
        }

    }
    else {
        setTimeout(function () { gotoLandmark(landmarkId); }, 500);
    }
}



function getVehilcesByLandmarkIdAndMapIt(landmarkId) {
    $.ajax({
        type: 'GET',
        url: 'Vehicles.aspx?QueryType=getVehilcesByLandmarkId&landmarkId=' + landmarkId,
        dataType: 'json',
        //data: postdata,
        async: false,
        success: function (msg) {
            ExtraVehicleForLandmark = [];
            var records = [];
            if (msg.Fleet.VehiclesLastKnownPositionInformation.length != undefined) {
                //ExtraVehicleForLandmark = msg.Fleet.VehiclesLastKnownPositionInformation;
                ExtraVehicleForLandmark = clone(msg.Fleet.VehiclesLastKnownPositionInformation);
                records = msg.Fleet.VehiclesLastKnownPositionInformation;
            }
            else {
                //ExtraVehicleForLandmark.push(msg.Fleet.VehiclesLastKnownPositionInformation);
                ExtraVehicleForLandmark.push(clone(msg.Fleet.VehiclesLastKnownPositionInformation));
                records.push(msg.Fleet.VehiclesLastKnownPositionInformation);
            }

            if (ExtraVehicleForLandmark.length > 0) {
                try {
                    vehiclegrid.getSelectionModel().deselectAll(false);
                    var el = document.getElementById(mapframe).contentWindow;
                    el.closeVehiclePopups();
                }
                catch (err) { }

                var boxIds = [];
                slectedboxidarray.length = 0;
                for (var i = 0; i < ExtraVehicleForLandmark.length; i++) {
                    boxIds.push(ExtraVehicleForLandmark[i].BoxId * 1);
                    slectedboxidarray.push(ExtraVehicleForLandmark[i].BoxId * 1);
                }

                var gridindex = 0;
                vehiclegrid.getStore().each(function (record) {
                    if ($.inArray(record.data.BoxId, boxIds) >= 0) {
                        if (!vehiclegrid.getSelectionModel().isSelected(gridindex))
                            vehiclegrid.getSelectionModel().select(gridindex, true, true);
                        return false;
                    }
                    gridindex++;

                });

                mapVehicles(true, records, true, false, false);
            }

        },
        error: function (msg) {

        }
    });

}

function clone(obj) {
    if (obj === null || typeof (obj) !== 'object' || 'isActiveClone' in obj)
        return obj;

    var temp = obj.constructor(); // changed

    for (var key in obj) {
        if (Object.prototype.hasOwnProperty.call(obj, key)) {
            obj['isActiveClone'] = null;
            temp[key] = clone(obj[key]);
            delete obj['isActiveClone'];
        }
    }

    return temp;
}

function updateOperationalState(boxId, operationalState, operationalStateName, operationalStateNotes) {

    mainstore.each(function (record, idx) {
        val = record.get('BoxId');
        if (val == boxId) {
            record.set('OperationalStateName', operationalStateName);
            record.set('OperationalStateNotes', operationalStateNotes);
            record.commit();
        }
    });

    vehiclegrid.getView().refresh();
}

function updateOperationalStateByVehicleId(vehicleId, operationalState, operationalStateNotes) {

    mainstore.each(function (record, idx) {
        val = record.get('VehicleId');
        var operationalStateName = (operationalState * 1 == 100) ? 'Available' : 'Unavailable';
        if (val == vehicleId) {
            record.set('OperationalStateName', operationalStateName);
            record.set('OperationalStateNotes', operationalStateNotes);
            record.commit();
        }
    });

    vehiclegrid.getView().refresh();
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
