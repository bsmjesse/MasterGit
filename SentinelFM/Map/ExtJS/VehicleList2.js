
Ext.Loader.setConfig(
{
   enabled : true
}
);
Ext.Loader.setPath('Ext.ux', './ExtJS/src/ux');
Ext.require([
'Ext.window.Window',
'Ext.grid.*',
'Ext.data.*',
'Ext.ux.grid.FiltersFeature',
'Ext.toolbar.Paging',
'Ext.selection.CheckboxModel',
'Ext.button.*',
'Ext.util.*',
'Ext.grid.PagingScroller',
'Ext.ux.AspWebAjaxProxy'
]);
Ext.Loader.setPath('Ext.app', './ExtJS/examples/portal/classes');
Ext.require(['Ext.app.GridPortlet']);
Ext.onReady(function ()
{
   Ext.tip.QuickTipManager.init();


   // setup the state provider, all state information will be saved to a cookie
   Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));

   var pagesize = 10000;
   // var isAlarmTabActive = false;
   // var isVehicleTabActive = true;
   // var dateformat = 'm/d/Y h:i a'; // 2011 - 12 - 06T11 : 50 : 19 - 05 : 00
   var selectedVehIds = "";
   var lastProxyFinished = false;
   var mapLoading = false;
   var currentSelected = "";
   var wincounter = 0;

   var ActiveTabs =
   {
      Alarms : 0,
      Vehicles : 1
   }

   var LoadStates =
   {
      GridLoading : 0,
      MapLoading : 1,
      AlarmLoading : 2,
      FleetsLoading : 3,
      MainStoreLoading : 4,
      GettingUpdates : 5,
      MappingVehicles : 6,
      MainStoreLoaded : 7,
      AlarmsLoaded: 8
   }
   var currentState = LoadStates.FleetsLoading;
   var activetab = ActiveTabs.Vehicles;

   //    var pagesize = 2;
   var alarminterval = 5000;
   var dateformat = 'd/m/Y h:i a';
   var initialData = "";
   var soundPresent = false;
   // var newPosition = 10;
   var statusColorString = '#00C000';

   var sensorPage = './Map/frmSensorMain.aspx?LicensePlate=';
   var historyPage = './History/frmhistmain_new.aspx?VehicleId=';

   Ext.define('VehicleList',
   {
      extend : 'Ext.data.Model',
      fields : [
      'BoxId',
      'Description',
      'StreetAddress',
      {
         name : 'OriginDateTime', type : 'date', dateFormat : 'c'
      }
      ,
      'Speed',
      'BoxArmed',
      'VehicleStatus',
      'PTO',
      'History',
      'VehicleId',
      'LicensePlate',
      'CustomSpeed'
      ],
      idProperty : 'BoxId'
   }
   );

   Ext.define('FleetList',
   {
      extend : 'Ext.data.Model',
      fields : [
      'OrganizationName',
      'FleetId',
      'FleetName',
      'Description'
      ]
   }
   );

   Ext.define('MapDataModel',
   {
      extend : 'Ext.data.Model',
      fields : [
      'BoxId',
      'Description',
      'StreetAddress',
      {
         name : 'OriginDateTime', type : 'date', dateFormat : 'c'
      }
      ,
      'Speed',
      'BoxArmed',
      'VehicleStatus',
      'PTO',
      'History',
      'VehicleId',
      'LicensePlate'
      ],
      idProperty : 'BoxId'
   }
   );


   var template = '<span style="color:{0};">{1}</span>';

   var selModel = Ext.create('Ext.selection.CheckboxModel',
   {
      listeners :
      {
         selectionchange : function(selModel, selections)
         {
            try
            {              
               // vehiclegrid.down('#updatePositionButton').setDisabled(selections.length == 0);

               if(selections.length > 0)
               {
                  selectedVehIds = "',";

                  vehiclegrid.down('#mapitButton').setDisabled(selections.length == 0);
                  vehiclegrid.down('#trackitButton').setDisabled(selections.length == 0);
                  Ext.each(selections, function(selectrec, i)
                  {
                     selectedVehIds = selectedVehIds + selectrec.data.VehicleId + ",";
                  }
                  );
                  selectedVehIds = selectedVehIds +  "\'";

               }
            }
            catch(err)
            {
               // console.log("Error " + err);
            }
         }
      }
   }
   );


   Ext.define('MapData',
   {
      extend : 'Ext.data.Model',
      fields : ['id', 'date', 'lat', 'lon', 'desc', 'icon']
   }
   );

   function onMapDataReceived(operation)
   {
      try
      {
         mapLoading = true;
         var data = Ext.decode(operation.response.responseText);
         // process server response here
         if (data.d != '-1' && data.d != "0")
         {
            var retData;
            if (data.d)
            {
               retData = eval(data.d);
               currentSelected = retData;
               // console.log("Json data2:" + (data.d));
               ShowMapFrameData(retData, true);
               mapLoading = false;
            }
         }
      }
      catch(err)
      {
         mapLoading = false;
      }
   }

   function onMapDataUpdate(operation)
   {
      try
      {
         mapLoading = true;
         var data = Ext.decode(operation.response.responseText);
         // process server response here
         if (data.d != '-1' && data.d != "0")
         {
            var retData;
            if (data.d)
            {
               retData = eval(data.d);
               currentSelected = retData;
               // console.log("Json data2:" + (data.d));
               ShowMapFrameData(retData, false);
               mapLoading = false;
            }
         }
      }
      catch(err)
      {
         mapLoading = false;
      }
   }

   function mapVehicles(map, vehicleIDs, isInitial, mapJsonData, zoomVehicles)
   {
      try
      {
         if(map == true)
         {
            if (vehicleIDs == '')
            {
               alert('<%=  GetScriptEscapeString((string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle")) %>');
            }
            else
            {
               if(mapJsonData != '')
               {
                  ShowMapFrameData(mapJsonData, isInitial);
               }
               else
               {
                  var mapstore = new Ext.data.Store(
                  {
                     proxy : new Ext.ux.AspWebAjaxProxy(
                     {
                        //
                        url : './Map/New/FleetInfo.aspx/MapIt2',
                        timeout : 120000,
                        actionMethods :
                        {
                           create : 'POST',
                           destroy : 'DELETE',
                           read : 'POST',
                           update : 'POST'
                        }
                        ,
                        extraParams :
                        {
                           vehicleIDs : vehicleIDs
                        }
                        ,
                        reader :
                        {
                           type : 'json',
                           model : 'MapData'
                        }
                        ,
                        headers :
                        {
                           'Content-Type' : 'application/json; charset=utf-8'
                        }
                     }
                     )
                  }
                  );

                  var operation = new Ext.data.Operation(
                  {
                     action : 'read'
                  }
                  );
                  if(zoomVehicles)
                  {
                     mapstore.proxy.read(operation, onZoomDataUpdate, mapstore);
                  }
                  else
                  {
                     if(isInitial)
                     {
                        mapstore.proxy.read(operation, onMapDataReceived, mapstore);
                     }
                     else
                     {
                        // console.log('Grid updated');
                        mapstore.proxy.read(operation, onMapDataUpdate, mapstore);
                     }
                  }
               }
            }
         }
      }
      catch(err)
      {
         mapLoading = false;
      }
   }
   function onZoomDataUpdate(operation)
   {
      try
      {
         mapLoading = true;
         var data = Ext.decode(operation.response.responseText);
         // process server response here
         if (data.d != '-1' && data.d != "0")
         {
            var retData;
            if (data.d)
            {
               retData = eval(data.d);
               // console.log("Json data2:" + (data.d));
               ZoomVehicles(retData, false);
               mapLoading = false;
            }
         }
      }
      catch(err)
      {
         mapLoading = false;
      }
   }


   var trackpanel = Ext.create('Ext.Panel',
   {
      id : 'trackpanel',
      autoHeight : true,
      titleCollapse : true,
      unstyled : true,
      layout : 'fit',
      border : false,
      width : '100%',
      maxWidth : window.screen.width,
      html : '<iframe id="trackwindow" name="trackwindow" src="./OpenLayerMap.aspx" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',
      // html : '<iframe id="trackwindow" name="trackwindow" src="./map/ExtJS/OpenLayerMap.html" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',
      margins : '0 0 0 0',
      autoScroll : true
   }
   );

   function onUpdatePositionReceived(operation)
   {
      try
      {
         var data = Ext.decode(operation.response.responseText);
         // process server response here
         if (data.d != '-1' && data.d != "0")
         {
            var retData;
            if (data.d)
            {
               mapLoading = true;
               retData = eval(data.d);
               mapLoading = false;
            }
         }
      }
      catch(err)
      {
      }
   }
      function doMin() {
          this.collapse(false);
          this.alignTo(document.body, 'bl-bl');
        }

   function onTrackDataReceived(operation)
   {
      try
      {
         var data = Ext.decode(operation.response.responseText);
         // process server response here
         if (data.d != '-1' && data.d != "0")
         {
            var retData;
            if (data.d)
            {
               retData = eval(data.d);

               // SetWinTrackData(retData);
               var winurl = "./OpenLayerMap.aspx?WinId=" + wincounter;
               // "./map/ExtJS/OpenLayerMap.html?WinId=" + wincounter;
               //
               // './Maps/oiltrax.html';
               //
               var htmlNewWin = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + winurl + '"></iframe>';

               var win = Ext.create('Ext.Window',
               {
                  title : 'Track Vehicles',
                  width : '600',
                  height : '480',
                  layout : 'fit',
                  maxWidth : window.screen.width,
                  maxHeight : window.screen.height,
                  maximizable : 'true',
                  minimizable : 'true',
                  resizable : 'true',
                  // bodyStyle : 'padding: 5px;',
                  closable : true,
                  html :  htmlNewWin
                  /* , items : [{
                  title : 'Vehicles',
                  autoLoad : {url : winurl, scripts : true}
                  }] */
               }
               );
               SetWinTrackData2(retData);
               win.on('minimize', doMin, win);
               win.show();
               wincounter ++ ;
            }
         }
      }
      catch(err)
      {
      }
   }
   var mapit = Ext.create('Ext.Button',
   {
      text : 'FindIt',
      id : 'mapitButton',
      //    renderTo : Ext.getBody(),
      tooltip : 'Find selected vehicle on map',
      iconCls : 'map',
      cls : 'cmbfonts',
      textAlign : 'left',
      disabled : true ,
      handler : function()
      {
         //console.log("selected " + selectedVehIds);
         try
         {
            mapLoading = true;
            mapVehicles(true, selectedVehIds, true, '', true);
            mapLoading = false;
         }
         catch(err)
         {
            mapLoading = false;
         }
      }
   }
   );

   var updatePosition = Ext.create('Ext.Button',
   {
      text : 'Update Position',
      id : 'updatePositionButton',
      //    renderTo : Ext.getBody(),
      tooltip : 'Map selected vehicle on map',
      iconCls : 'map',
      cls : 'cmbfonts',
      textAlign : 'left',
      disabled : true ,
      handler : function()
      {
         // console.log("selected " + selectedVehIds);
         try
         {
            var mapstore = new Ext.data.Store(
            {
               proxy : new Ext.ux.AspWebAjaxProxy(
               {
                  //
                  url : './Map/New/FleetInfo.aspx/UpdatePosition',
                  timeout : 120000,
                  actionMethods :
                  {
                     create : 'POST',
                     destroy : 'DELETE',
                     read : 'POST',
                     update : 'POST'
                  }
                  ,
                  extraParams :
                  {
                     vehicleIDs : selectedVehIds
                  }
                  ,
                  reader :
                  {
                     type : 'json',
                     model : 'MapData'
                  }
                  ,
                  headers :
                  {
                     'Content-Type' : 'application/json; charset=utf-8'
                  }
               }
               )
            }
            );

            var operation = new Ext.data.Operation(
            {
               action : 'read'
            }
            );
            mapstore.proxy.read(operation, onUpdatePositionReceived, mapstore);
         }
         catch(err)
         {
         }
      }
   }
   );

   var trackit = Ext.create('Ext.Button',
   {
      text : 'TrackIt',
      id : 'trackitButton',
      //    renderTo : Ext.getBody(),
      tooltip : 'Track selected vehicle on separate map',
      iconCls : 'map',
      cls : 'cmbfonts',
      textAlign : 'left',
      disabled : true ,
      handler : function()
      {
         try
         {
            var mapstore = new Ext.data.Store(
            {
               proxy : new Ext.ux.AspWebAjaxProxy(
               {
                  //
                  url : './Map/New/FleetInfo.aspx/MapIt2',
                  timeout : 120000,
                  actionMethods :
                  {
                     create : 'POST',
                     destroy : 'DELETE',
                     read : 'POST',
                     update : 'POST'
                  }
                  ,
                  extraParams :
                  {
                     vehicleIDs : selectedVehIds
                  }
                  ,
                  reader :
                  {
                     type : 'json',
                     model : 'MapData'
                  }
                  ,
                  headers :
                  {
                     'Content-Type' : 'application/json; charset=utf-8'
                  }
               }
               )
            }
            );

            var operation = new Ext.data.Operation(
            {
               action : 'read'
            }
            );
            mapstore.proxy.read(operation, onTrackDataReceived, mapstore);
         }
         catch(err)
         {
         }
      }
   }
   );

   var demomap = Ext.create('Ext.Button',
   {
      text : 'Demo open layer map',
      id : 'demomapButton',
      //    renderTo : Ext.getBody(),
      tooltip : 'Demo open layer map without tracking vehicle',
      iconCls : 'map',
      cls : 'cmbfonts',
      textAlign : 'left',
      disabled : false ,
      handler : function()
      {
         try
         {
            var winurl = "./OpenLayerMap.aspx";
            // "./map/ExtJS/OpenLayerMap.html";
            //
            var htmlNewWin = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + winurl + '"></iframe>';

            var win = Ext.create('Ext.Window',
            {
               title : 'Track Vehicles',
               width : '1000',
               maxWidth : window.screen.width,
               maxHeight : window.screen.height,
               height : '600',
               layout : 'fit',
               maximizable : 'true',
               minimizable : 'true',
               resizable : 'true',
               // bodyStyle : 'padding: 5px;',
               closable : true,
               html :  htmlNewWin
            }
            ).show();
         }
         catch(err)
         {
         }
      }
   }
   );

   var searchMap = Ext.create('Ext.Button',
   {
      text : 'SearchMap',
      id : 'searchMapButton',
      //    renderTo : Ext.getBody(),
      tooltip : 'Search anything on map',
      iconCls : 'map',
      cls : 'cmbfonts',
      textAlign : 'left',
      disabled : true ,
      handler : function()
      {
         try
         {
            //console.log("selected " + selectedVehIds);
         }
         catch(err)
         {
         }
      }
   }
   );

   var filters =
   {
      ftype : 'filters',
      local : true,   // defaults to false (remote filtering)
      filters : [
      {
         type : 'string',
         dataIndex : 'BoxId'
      }
      ,
      {
         type : 'string',
         dataIndex : 'Description'
      }
      ,
      {
         type : 'string',
         dataIndex : 'StreetAddress'
      }
      ,
      {
         type : 'string',
         dataIndex : 'VehicleStatus'
      }
      ,
      {
         type : 'date',
         dataIndex : 'OriginDateTime'
      }
      ,
      {
         type : 'boolean',
         dataIndex : 'BoxArmed'
      }
      ]
   }
   ;

   // create the Data Store
   var mainstore = Ext.create('Ext.data.Store',
   {
      model : 'VehicleList',
      pageSize : pagesize,
      // buffered : true,
      // purgePageCount : 0,
      autoLoad : false,
      autosync : false,
      storeId : 'Vehicles',
      listeners :
      {
         'load' : function(store, records, options)
         {
            try
            {
               currentState = LoadStates.GridLoading;
               if (navigator.appName.indexOf("Microsoft") != - 1)
               {
                  dateformat = 'dd/mm/yyyy HH:MM tt';
               }
               // console.info('Height: ' + (window.screen.height / 3));
               mainstore.sort('OriginDateTime', 'DESC');
               // console.info('Main store loaded');

               var mainRecords = mainstore.getRange();
               selectedVehIds = "',";
               var AllJsonData = "";
               Ext.each(mainRecords, function(exirecord)
               {
                  selectedVehIds = selectedVehIds + exirecord.data.VehicleId + ",";
               }
               );
               selectedVehIds = selectedVehIds +  "\'";
               currentState = LoadStates.MappingVehicles;
               mapVehicles(true, selectedVehIds, true, '', false);
               lastProxyFinished = true;
            }
            catch(err)
            {

            }
            currentState = LoadStates.MainStoreLoaded;
         }
         ,
         scope : this
      }
      ,
      proxy :
      {
         // load using HTTP
         type : 'ajax',
         url : 'Vehicles.aspx',
         timeout : 120000,
         reader :
         {
            type : 'xml',
            root : 'Fleet',
            record : 'VehiclesLastKnownPositionInformation'
         }
         /*  , params : {
         QueryType : 'GetVehiclePosition'
         } */
      }
      , sorters : [
      {
         property : 'OriginDateTime',
         direction : 'DESC'
      }
      ]
   }
   );

   var gridPager = new Ext.PagingToolbar(
   {
      // Ext.create('Ext.PagingToolbar', {
      store : mainstore,
      displayInfo : true,
      displayMsg : 'Displaying vehicles {0} - {1} of {2}',
      emptyMsg : "No vehicles to display"// ,

   }
   );

 // create the Data Store
   var fleetstore = Ext.create('Ext.data.Store',
   {
      model : 'FleetList',
      autoLoad : false,
      storeId : 'FleetStore',

      listeners :
      {
         'load' : function(store, records, options)
         {
            try
            {
               currentState = LoadStates.MainStoreLoading;
               mainstore.load(
               {
                  params :
                  {
                     QueryType : 'GetVehiclePosition',
                     start : 0,
                     limit : pagesize
                  }
               }
               );
            }
            catch(err)
            {
            }
         }
         ,
         scope : this
      }
      ,
      // buffered : true,
      proxy :
      {
         // load using HTTP
         type : 'ajax',
         url : 'Vehicles.aspx?QueryType=GetAllFleets',
         timeout : 120000,
         reader :
         {
            type : 'xml',
            root : 'Fleet',
            record : 'FleetsInformation'
         }
      }
   }
   );
   // add a combobox to the toolbar
   var fleets = Ext.create('Ext.form.field.ComboBox',
   {
      //        hideLabel : true,
      store : 'FleetStore',
      displayField : 'FleetName',
      valueField : 'FleetId',
      // listClass : 'cmbfonts',
      typeAhead : true,
      //        fieldCls : 'cmbfonts',
      fieldStyle :  'cmbfonts',
      labelCls : 'cmbLabel',
      mode : 'local',
      triggerAction : 'all',
      fieldLabel : ' Fleets ',
      emptyText : 'All Vehicles...',
      tooltip : 'Select group of vehicles to show',
      selectOnFocus : true,
      width : 300,
      labelWidth : 40,
      listeners :
      {
         scope : this,
         'select' : function(combo, value)
         {
            // console.log("Fleet changed." + combo.getValue());
            currentState = LoadStates.MainStoreLoading;
            try
            {
               mainstore.load(
               {
                  params :
                  {
                     QueryType : 'GetVehiclePosition',
                     fleetID : combo.getValue(),
                     start : 0,
                     limit : pagesize
                  }
               }
               );
               mainstore.sort('OriginDateTime', 'DESC');
            }
            catch(err)
            {
            }
            currentState = LoadStates.MainStoreLoaded;
         }
      }
   }
   );


   var vehiclegrid = Ext.create('Ext.grid.Panel',
   {
      id : 'vehiclesgrid',
      enableColumnHide : true,
      title : 'Vehicles',
      autoLoad : false,
      // split : true,
      // unstyled : true,
      // sm,
      // autoScroll : true,
      frame : true,

      animCollapse : false,
      // renderTo : 'vehiclesgrid',
      // region : 'south',
      maxWidth : window.screen.width,
      maxHeight : window.screen.height,
      enableSorting : true,
      stateful : true,
      stateId : 'stateVehicleGrid',
      closable : false,
      columnLines : true,
      //        resizable : true,
      width : window.screen.width - 10,
      // height : 300,
      autoheight : true,
      //      maxHeight : window.screen.height - 50,
      store : mainstore,
      features : [filters],
      viewConfig :
      {
         emptyText : 'No vehicles to display',
         useMsg : false
      }
      ,
      columns : [
      /*{
         xtype : 'rownumberer',
         width : 50,
         sortable : false
      }
      ,*/
      {
         text : 'UnitID',
         align : 'left',
         width : 70,
         dataIndex : 'BoxId',
         filterable : true,
         sortable : true,
         // flex : 1,
         hidden : false
      }
      ,
      {
         text : 'Description',
         align : 'left',
         width : 150,
         renderer : function (value, p, record)
         {
            // return Ext.String.format('<a href="#" OnClick="NewWindow(\'{0}\',\'{1}\')">{2}</a>', sensorPage, Ext.String.escape(record.data['LicensePlate']), value);
            return Ext.String.format('<a href="#" OnClick="SensorInfoWindow(\'{0}\')">{1}</a>', Ext.String.escape(record.data['LicensePlate']), value);
         }
         ,
         dataIndex : 'Description',
         filterable : true,
         sortable : true
      }
      ,
      {
         text : 'Status',
         align : 'left',
         width : 120,
         renderer : function (value)
         {
            var fontColor = "black";
            if(value.indexOf("Parked") != - 1)
            {
               fontColor = "red";
            }
            else if(value.indexOf("Idling") != - 1)
            {
               fontColor = "orange";
            }
            else if(value.indexOf("Moving") != - 1)
            {
               fontColor = "green";
            }

            return Ext.String.format(template, fontColor, value);
         }
         ,
         dataIndex : 'VehicleStatus',
         filterable : true,
         sortable : true
      }
      ,
      {
         text : 'Speed',
         align : 'left',
         width : 50,
         dataIndex : 'CustomSpeed',
         filterable : true,
         sortable : true
      }
      ,
      {
         text : 'Date/Time',
         align : 'left',
         width : 120,
         xtype : 'datecolumn',
         format : dateformat,
         dataIndex : 'OriginDateTime',
         filterable : true,
         sortable : true
      }
      ,
      {
         text : 'Address',
         align : 'left',
         width : 300,
         dataIndex : 'StreetAddress',
         filterable : true,
         sortable : true
      }
      ,
      {
         text : 'Armed',
         align : 'left',
         width : 40,
         dataIndex : 'BoxArmed',
         filterable : true,
         sortable : true
      }
      ,
      {
         text : 'History',
         align : 'left',
         width : 90,
         renderer : function (value)
         {
            return Ext.String.format('<a href="#" OnClick="NewWindow(\'{0}\',{1})">History</a>', historyPage, Ext.String.format(value));
         }
         ,
         dataIndex : 'VehicleId',
         filterable : false,
         sortable : true
      }
      ]
      , dockedItems : [
      {
         xtype : 'toolbar',
         dock : 'top',
         items : [fleets,
         '-', mapit,
         '-', trackit,
         // '-', demomap,
         // '-', updatePosition,
         '-',
         {
            itemId : 'AutoSync',
            boxLabel : 'AutoSync',
            boxLabelCls : 'cmbfonts',
            xtype : 'checkboxfield',
            checked   : IsSyncOn,
            tooltip : 'Refresh the map and grid automatically',
            handler : function()
            {
               IsSyncOn = ! IsSyncOn;
            }
            // IsSyncOn
         }
         ]
      }
      ], selModel : selModel
      ,
      // paging bar on the bottom
      bbar : gridPager
   }
   );

  
   // var sm = Ext.create('Ext.selection.CheckboxModel');
   var recordUpdater = Ext.create('Ext.data.Store',
   {
      model : 'VehicleList',
      autoLoad : false,
      listeners :
      {
         'load' : function(store, records, options)
         {
            try
            {
               if(store.getCount() > 0)
               {
                  currentState = LoadStates.GettingUpdates;
                  var modified = store.getRange();
                  var recToDelete = new Array();
                  var counter = 0;
                  changedVehIds = "',";
                  // var currentTicked = vehiclegrid.getSelectionModel().getSelection();
                  //                Ext.each(currentTicked, function(selectedRec, i)
                  //               {
                  //                 console.log("Selected " + selectedRec.data.BoxId);
                  //               }
                  //               );
                  var newSelected = new Array();
                  var newSelectedIdx = 0;
                  // newSelectedIdx = mainstore.getCount() + 1; // We will insert all new records at the end so any selected record which was clared will be added by this index
                  Ext.each(modified, function(modrecord, i)
                  {
                     var tests = mainstore.findRecord('BoxId', modrecord.data.BoxId, 0, false, false, true);
                     if(vehiclegrid.getSelectionModel().isSelected(tests))
                     {
                        newSelected[newSelectedIdx] = mainstore.getCount() + 1 + counter;
                        //console.info('Selected ' + modrecord.data.BoxId);
                        newSelectedIdx = newSelectedIdx + 1;
                     }
                     recToDelete[counter] = tests;
                     counter = counter + 1;
                     changedVehIds = changedVehIds + tests.data.VehicleId + ",";

                     // );
                  }
                  );
                  changedVehIds = changedVehIds +  "\'";
                  // console.info('Vehilces moved' +   selectedVehIds);
                  // mapVehicles(true, selectedVehIds, false, '');
                  currentState = LoadStates.MappingVehicles;
                  mapVehicles(true, changedVehIds, false, '', false);
                  currentState = LoadStates.MainStoreLoading;
                  mainstore.remove(recToDelete);
                  // console.info('Records removed' + (counter - 1));
                  mainstore.insert(mainstore.getCount() + 1, modified);
                  // console.info('Records added');
                  mainstore.sort('OriginDateTime', 'DESC');
                  vehiclegrid.getSelectionModel().select(newSelected, true);                  
                  // , updatedData);
                  // mainstore.sort();
               }
            }
            catch(err)
            {
            }
            currentState = LoadStates.MainStoreLoaded;
         }
      }
      ,
      proxy :
      {
         // load using HTTP
         type : 'ajax',
         url : 'Vehicles.aspx',
         timeout : 120000,
         reader :
         {
            type : 'xml',
            root : 'Fleet',
            record : 'VehiclesLastKnownPositionInformation'
         }
      }
   }
   );
   // gridPager.refresh.hideParent = true;
   // gridPager.refresh.hide();

   Ext.define('Alarm',
   {
      extend : 'Ext.data.Model',
      fields : [
      'AlarmId',
      {
         name : 'TimeCreated', type : 'date', dateFormat : 'c'
      }
      ,
      'AlarmLevel',
      'vehicleDescription',
      'AlarmDescription'
      ]
   }
   );

   // create the Data Store
   var alarmsstore = Ext.create('Ext.data.Store',
   {
      model : 'Alarm',
      autoLoad : false,
      proxy :
      {
         // load using HTTP
         type : 'ajax',
         url : './Map/frmAlarmRotatingServerCall_XML.aspx',
         timeout : 120000,
         reader :
         {
            type : 'xml',
            root : 'Alarm',
            record : 'AllUserAlarmsInfo'
         }
      }
   }
   );

   var alarmgrid = Ext.create('Ext.grid.Panel',
   {
      id : 'alarmgrid',
      titleCollapse : true,
      enableColumnHide : false,
      enableSorting : false,
      closable : false,
      // collapsible : false,
      resizable : false,
      stateful : true,
      width : 800,
      height : 300,
      title : 'Alarms',
      store : alarmsstore,
      stateId : 'stateAlarmsGrid',
      // renderTo : 'alarmsgrid',
      // titleCollapse : true,
      columnLines : true,
      // renderBody(),
      // Ext.getBody(),
      viewConfig :
      {
         emptyText : 'No alarms to display',
         useMsg : false,
         getRowClass : function (rec, rowIdx, params, store)
         {
            if (rec.get('AlarmDescription').indexOf("CIA") != - 1)
            {
               return 'grid-row-red';
            }
            if (rec.get('AlarmDescription').indexOf("VIA") != - 1)
            {
               return 'grid-row-yellow';
            }
         }
      }
      ,
      columns : [
      {
         text : 'Number',
         align : 'left',
         width : 80,
         renderer : function (value)
         {
            return Ext.String.format('<a href="#" OnClick="NewAlarmWindow({0})">{1}</a>', value, value);
         }
         ,
         dataIndex : 'AlarmId',
         sortable : false
      }
      ,
      {
         text : 'Alarm Time',
         align : 'left',
         width : 120,
         xtype : 'datecolumn',
         format : dateformat,
         dataIndex : 'TimeCreated',
         sortable : false
      }
      ,
      {
         text : 'Alarm Priority',
         align : 'left',
         width : 80,
         dataIndex : 'AlarmLevel',
         sortable : false
      }
      ,
      {
         text : 'Alarm Description',
         align : 'left',
         width : 120,
         renderer : function (value)
         {
            if (value.indexOf("CIA") != - 1 && soundPresent != true)
            {
               soundPresent = true;
               return Ext.String.format('{0} <object><embed src="./sounds/FireAlarm.wav" hidden="true" autostart="True" loop="true" type="audio/wav" pluginspage="http://www.apple.com/quicktime/      download/" /></object>', value);
            }
            else
            {
               return value;
            }
         }
         ,
         dataIndex : 'AlarmDescription',
         sortable : false
      }
      ,
      {
         text : 'Vehicle Description',
         align : 'left',
         width : 120,
         dataIndex : 'vehicleDescription',
         sortable : false
      }
      ]
   }
   );

   var tabs = Ext.createWidget('tabpanel',
   {

      region : 'south',
      width : '100%',
      layout : 'fit',
      plain : true,
      height : 300,
      defaults :
      {
         autoScroll : true
      }
      ,
      collapsible : true,
      animCollapse : true,
      split : true,
      // autoDestroy : false,
      activeTab : 0,
      listeners :
      {
         tabchange : function(tabPanel, newTab)
         {
            try
            {
               if(newTab.id == 'alarmgrid')
               {
                  // isAlarmTabActive = true;
                  // isVehicleTabActive = false;
                  activetab = ActiveTabs.Alarms;
                  currentState = LoadStates.AlarmsLoaded;
               }
               else if(newTab.id == 'vehiclesgrid')
               {
                  // isAlarmTabActive = false;
                  // isVehicleTabActive = true;
                  activetab = ActiveTabs.Vehicles;
                  currentState = LoadStates.MainStoreLoaded;
               }
            }
            catch(err)
            {
            }
         }
      }
      ,
      items : [vehiclegrid, alarmgrid]

   }
   );

   var centerpanel = Ext.create('Ext.Panel',
   {
      region : 'center',
      //      region : 'north',
      id : 'centerwin',
      // title : 'MapView',
      // autoHeight : true,
      titleCollapse : true,
      // split : true,
      // unstyled : true,
      layout : 'fit',
      border : false,
      // height : '50%',
      height : 900,
      // anchor : 'window.screen.height/2',
      width : '100%',
      maxWidth : window.screen.width - 2,
      html : '<iframe id="mapframe" name="mapframe" src="./MapNew/OpenLayerMaps.aspx" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',      
      listeners :
      {
         'afterrender' : function()
         {
           //console.log(' I am done with map');
           fleetstore.load();
         }
      }
   }
   );


   var viewport = Ext.create('Ext.Viewport',
   {
      layout : 'border',
      border : false,
      items : [centerpanel, tabs]
      //     , renderTo :       renderBody()
   }
   );

   //var alarmsDone = true;
   var alarmtask =
   {
      run : function ()
      {
         if(alarmsstore.isLoading() != true && activetab == ActiveTabs.Alarms && currentState == LoadStates.AlarmsLoaded)
         {
            try
            {
               //alarmsDone = false;
               currentState=LoadStates.AlarmLoading;
               var operation = new Ext.data.Operation(
               {
                  action : 'read'
               }
               );
               alarmsstore.proxy.read(operation, this.onProxyLoad, alarmsstore);
            }
            catch(err)
            {
            }
            //                 console.log('Shehul');
         }
      }
      ,
      onProxyLoad : function(operation)
      {
         try
         {
            var me = this,
            resultSet = operation.getResultSet(),
            records = operation.getRecords(),
            successful = operation.wasSuccessful();

            alarmsstore.loading = false;
            soundPresent = false;
            if (resultSet)
            {
               alarmsstore.totalCount = resultSet.total;


               if(initialData == "")
               {
                  initialData = operation.response.responseText;
                  //                         console.info('Data have not changed');
                  //                        console.info('Initial load verify');
               }
               else
               {
                  if(initialData.length != operation.response.responseText.length)
                  {
                     alarmsstore.load();
                     //                          console.info('Data changed');
                     initialData = operation.response.responseText;
                  }
                  else
                  {
                     //                          console.info('Data have not changed');

                  }
               }
            }
            //alarmsDone = true;
         }
         catch(err)
         {
         }
         currentState = LoadStates.AlarmsLoaded;
      }
      ,
      interval : alarminterval // 5 second
   }

   var alarmrunner = new Ext.util.TaskRunner();

   alarmrunner.start(alarmtask);

   var vehicletask =
   {
      run : function ()
      {
         if( ! mainstore.isLoading() && ! mapLoading  && activetab == ActiveTabs.Vehicles && currentState == LoadStates.MainStoreLoaded)
         {
            // if(lastProxyFinished == true)
            // {
            if(IsSyncOn)
            {
               try
               {
                  currentState = LoadStates.GettingUpdates;
                  // console.info('Map Refresh frequency is : ' + interval);
                  recordUpdater.removeAll(true);
                  // lastProxyFinished = false;
                  recordUpdater.load(
                  {
                     params :
                     {
                        QueryType : 'GetVehiclePosition',
                        start : 0,
                        limit : pagesize
                     }
                  }
                  );
               }
               catch(err)
               {
               }
            }
         }
      }
      ,
      interval : interval // 5 second
   }
   var vehiclerunner = new Ext.util.TaskRunner();

   vehiclerunner.start(vehicletask);
}
);
