// JavaScript Document
// JavaScript Document

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
//  'Ext.toolbar.Paging',
//'Ext.toolbar.Paging',
//'Ext.ux.RowExpander',
'Ext.selection.CheckboxModel',
'Ext.button.*',
'Ext.ModelManager',
    'Ext.tip.QuickTipManager',
'Ext.util.*',
//'Ext.grid.PagingScroller',
//'Ext.ux.SimpleIFrame',
'Ext.ux.AspWebAjaxProxy'
]);
//Ext.Loader.setPath('Ext.app', './ExtJS/examples/portal/classes');
//Ext.require(['Ext.app.GridPortlet']);
//Ext.Loader.setPath('Ext.util', './Map/ExtJS/util');
//Ext.require(['Ext.util.JSON']);
Ext.onReady(function ()
{

   var initialData = "";
   var pagesize=10000;
   var dateformat = 'm/d/Y h:i a'; // 2011 - 12 - 06T11 : 50 : 19 - 05 : 00
   var selectedVehIds = "";
   var lastProxyFinished = false;
   var currentSelected="";
   var wincounter=0;
   var sensorPage='./Map/frmSensorMain.aspx?LicensePlate=';
   var historyPage='./History/frmhistmain_new.aspx?VehicleId=';
   
   Ext.tip.QuickTipManager.init();
   // setup the state provider, all state information will be saved to a cookie
   Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));

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
      'LicensePlate'
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
   // create the Data Store
   var fleetstore = Ext.create('Ext.data.Store',
   {
      model : 'FleetList',
      autoLoad : true,
      storeId : 'FleetStore',
      // buffered : true,
      proxy :
      {
         // load using HTTP
         type : 'ajax',
         url : 'Vehicles.aspx?QueryType=GetAllFleets',
         reader :
         {
            type : 'xml',
            root : 'Fleet',
            record : 'FleetsInformation'
         }
      }
   }
   );


   var sm = Ext.create('Ext.selection.CheckboxModel');


   // create the Data Store
   var mainstore = Ext.create('Ext.data.Store',
   {
      model : 'VehicleList',
      pageSize: pagesize,
      //buffered: true,
      //purgePageCount: 0,
      autoLoad : false,
      autosync : false,
      storeId : 'Vehicles',
      listeners :
      {
         'load' : function(store, records, options)
         {
            if (navigator.appName.indexOf("Microsoft") != - 1)
            { 
             dateformat = 'dd/mm/yyyy HH:MM tt';
            }
            //console.info('Height: ' + (window.screen.height/3));
            mainstore.sort('OriginDateTime', 'DESC');
            // console.info('Main store loaded');

            var mainRecords = mainstore.getRange();
            selectedVehIds = "',";
            var AllJsonData="";
            Ext.each(mainRecords, function(exirecord)
            {
               selectedVehIds = selectedVehIds + exirecord.data.VehicleId + ",";
            }
            );
            selectedVehIds = selectedVehIds +  "\'";
            mapVehicles(true, selectedVehIds,true,'',false);
            lastProxyFinished=true;
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
            mainstore.load(
            {
               params :
               {
                  QueryType : 'GetVehiclePosition',
                  fleetID : combo.getValue(),
                  start: 0, 
                  limit: pagesize
               }
            }
            );
            mainstore.sort('OriginDateTime', 'DESC');

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



   var recordUpdater = Ext.create('Ext.data.Store',
   {
      model : 'VehicleList',
      autoLoad : false,
      listeners :
      {
         'load' : function(store, records, options)
         {
            if(store.getCount() > 0)
            {
               var modified = store.getRange();
               var recToDelete = new Array();
               var counter = 0;
               selectedVehIds = "',";

               Ext.each(modified, function(modrecord, i)
               {
                  var tests = mainstore.findRecord('BoxId', modrecord.data.BoxId, 0, false, false, true);
                  recToDelete[counter] = tests;
                  counter = counter + 1;
                  selectedVehIds = selectedVehIds + tests.data.VehicleId + ",";
               }
               );
               selectedVehIds = selectedVehIds +  "\'";
               //console.info('Vehilces moved' +   selectedVehIds);
               //mapVehicles(true, selectedVehIds,false,'');
               mapVehicles(true, selectedVehIds,false,'',false);
               
               mainstore.remove(recToDelete);
               // console.info('Records removed' + (counter - 1));
               mainstore.insert(mainstore.getCount() + 1, modified);
               // console.info('Records added');
               mainstore.sort('OriginDateTime', 'DESC');

              
               //,updatedData);
               // mainstore.sort();
            }
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

   function renderGridBody()
   {
      
//      if (document.all)
//      return 'document.body';
//      else if (document.getElementById)
//      return 'vehiclesgrid';
//      else if (document.layers)
      return 'vehiclesgrid';
   }



   var template = '<span style="color:{0};">{1}</span>';

   var selModel = Ext.create('Ext.selection.CheckboxModel',
   {
      listeners :
      {
         selectionchange : function(selModel, selections)
         {
            selectedVehIds = "',";
            vehiclegrid.down('#mapitButton').setDisabled(selections.length == 0);
            vehiclegrid.down('#trackitButton').setDisabled(selections.length == 0);
            if(selections.length != 0)
            {
               Ext.each(selections, function(selectrec, i)
               {
                  selectedVehIds = selectedVehIds + selectrec.data.VehicleId + ",";
               }
               );
               selectedVehIds = selectedVehIds +  "\'";

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
      // var datas = Ext.decode(operation.response.responseText);
      // console.log(datas);
      // console.log('Shehul');
      var data = Ext.decode(operation.response.responseText);
      // process server response here
      if (data.d != '-1' && data.d != "0")
      {
         var retData;
         if (data.d)
         {
            retData = eval(data.d);
            currentSelected=retData;
            //console.log("Json data2:" + (data.d));
            ShowMapFrameData(retData,true);
         }
      }
   }

function onMapDataUpdate(operation)
   {
      // var datas = Ext.decode(operation.response.responseText);
      // console.log(datas);
      // console.log('Shehul');
      var data = Ext.decode(operation.response.responseText);
      // process server response here
      if (data.d != '-1' && data.d != "0")
      {
         var retData;
         if (data.d)
         {
            retData = eval(data.d);
            currentSelected=retData;
            //console.log("Json data2:" + (data.d));
            ShowMapFrameData(retData,false);
         }
      }
   }

   function mapVehicles(map, vehicleIDs,isInitial,mapJsonData,zoomVehicles)
   {

      if(map == true)
      {
         if (vehicleIDs == '')
         {
            alert('<%=  GetScriptEscapeString((string)base.GetLocalResourceObject("sn_MessageText_SelectVehicle")) %>');
         }
         else
         {
            if(mapJsonData!='') 
            {
                ShowMapFrameData(mapJsonData,isInitial);
            }
            else
            {
                var mapstore = new Ext.data.Store(
                {
                   proxy : new Ext.ux.AspWebAjaxProxy(
                   {
                      //
                      url : './Map/New/FleetInfo.aspx/MapIt2',
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
                        //console.log('Grid updated');
                        mapstore.proxy.read(operation, onMapDataUpdate, mapstore);                   
                    }
                }
            }
         }
      }
   }
   function onZoomDataUpdate(operation)
   {
      // var datas = Ext.decode(operation.response.responseText);
      // console.log(datas);
      // console.log('Shehul');
      var data = Ext.decode(operation.response.responseText);
      // process server response here
      if (data.d != '-1' && data.d != "0")
      {
         var retData;
         if (data.d)
         {
            retData = eval(data.d);
            //console.log("Json data2:" + (data.d));
            ZoomVehicles(retData,false);
         }
      }
   }
   

   var trackpanel = Ext.create('Ext.Panel',
   {
//      region:'north',
      id : 'trackpanel',
      // title : 'MapView',
      autoHeight : true,
      titleCollapse : true,
      //split : true,
      unstyled : true,
      layout : 'fit',
      border : false,
//      height : '50%',
      // anchor : 'window.screen.height/2',
      width : '100%',
      maxWidth : window.screen.width - 2,
      html: '<iframe id="trackwindow" name="trackwindow" src="./map/new/OpenLayerMap.aspx" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',                    
      margins : '0 0 0 0',
      autoScroll : true
   }
   );
   

   
 function onTrackDataReceived(operation)
   {
      var data = Ext.decode(operation.response.responseText);
      // process server response here
      if (data.d != '-1' && data.d != "0")
      {
         var retData;
         if (data.d)
         {
            retData = eval(data.d);

            //SetWinTrackData(retData);
            var winurl="./OpenLayerMap.aspx?WinId=" + wincounter;
            //'./Maps/oiltrax.html';
            //
            var htmlNewWin='<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + winurl + '"></iframe>';
            
           var win= Ext.create('Ext.Window', {
            title: 'Track Vehicles',
            width: '900',
            height: '500',            
            layout: 'fit',            
            maximizable: 'true',
            minimizable: 'true',
            resizable: 'true',
            //bodyStyle: 'padding: 5px;',
            closable: true,
            html:  htmlNewWin           
            /*,items: [{            
                    title: 'Vehicles',
                	autoLoad: {url: winurl, scripts:true}             
            }]*/
            }).show();    
            SetWinTrackData2(retData);
            wincounter++;
         }         
      }
   }
   var mapit = Ext.create('Ext.Button',
   {
      text : 'MapIt',
      id : 'mapitButton',
      //    renderTo : Ext.getBody(),
      tooltip : 'Map selected vehicle on map',
      iconCls : 'map',
      cls : 'cmbfonts',
      textAlign : 'left',
      disabled : true ,
      handler : function()
      {
        //console.log("selected " + selectedVehIds); 
        mapVehicles(true, selectedVehIds,true,'',true);       
      }
   }
   );

  var trackit = Ext.create('Ext.Button',
   {
      text : 'Track selected',
      id : 'trackitButton',
      //    renderTo : Ext.getBody(),
      tooltip : 'Track selected vehicle on separate map',
      iconCls : 'map',
      cls : 'cmbfonts',
      textAlign : 'left',
      disabled : true ,
      handler : function()
      {
        var mapstore = new Ext.data.Store(
                {
                   proxy : new Ext.ux.AspWebAjaxProxy(
                   {
                      //
                      url : './Map/New/FleetInfo.aspx/MapIt2',
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
   }
   );

   var vehiclegrid = Ext.create('Ext.grid.Panel',
   {
      id : 'vehiclesgrid',
      enableColumnHide : false,      
      title : 'Vehicles',
      split : true,
      // unstyled : true,
      // sm,
      //autoScroll : true,
      //frame : true,
      collapsible : true,
      //renderTo: Ext.getBody(),
//       verticalScroller: {
//            xtype: 'paginggridscroller'
//            //,activePrefetch: false
//        },
      animCollapse : false,
      region: 'south',      
      maxWidth : window.screen.width - 2,
      maxHeight : window.screen.height - 2,
      enableSorting : true,
      stateful: true,
      stateId: 'stateVehicleGrid',
      //invalidateScrollerOnRefresh: false,
      //verticalScrollerType: 'paginggridscroller',
      //  loadMask : false,
//      disableSelection : true,
      //titleCollapse : true,
      closable : false,
      //        collapsible : true,
      columnLines : true,
      //        resizable : true,
      width : 1200,
      height : 300,
//      maxHeight: window.screen.height - 50,
      store : mainstore,
      features : [filters],
      viewConfig :
      {
         emptyText : 'No vehicles to display',         
         useMsg : false
      }
      ,
      columns : [
     /* {
            xtype: 'rownumberer',
            width: 50,
            sortable: false
        },*/
      {
         text : 'UnitID',
         align : 'left',
         width : 70,
         dataIndex : 'BoxId',
         filterable : true,
         sortable : true,
         //flex: 1,
         hidden : false
      }
      ,
      {
         text : 'Description',
         align : 'left',
         width : 150,
         renderer : function (value, p, record)
         {
            //return Ext.String.format('<a href="#" OnClick="NewWindow(\'{0}\',\'{1}\')">{2}</a>',sensorPage,Ext.String.escape(record.data['LicensePlate']), value);
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
         dataIndex : 'Speed',
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
            return Ext.String.format('<a href="#" OnClick="NewWindow(\'{0}\',{1})">History</a>', historyPage,Ext.String.format(value));
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
         '-', mapit, '-',trackit,'-',
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
      ],selModel : selModel
      /*,
      // paging bar on the bottom
        bbar: Ext.create('Ext.PagingToolbar', {
            store: mainstore,
            displayInfo: true,
            displayMsg: 'Displaying vehicles {0} - {1} of {2}',
            emptyMsg: "No vehicles to display",
            /*items:[
                '-', {
                text: 'Show Preview',
                pressed: pluginExpanded,
                enableToggle: true,
                toggleHandler: function(btn, pressed) {
                    var preview = Ext.getCmp('gv').getPlugin('preview');
                    preview.toggleExpanded(pressed);
                }
            }]
        })*/
   }
   );


   var Navigation = Ext.create('Ext.Panel',
   {
      // contentEl : 'west',
      title : 'Navigation',
      iconCls : 'nav' //
   }
   );


   var settings = Ext.create('Ext.Panel',
   {
      title : 'Settings',
      html : '<p></p>',
      iconCls : 'settings'
   }
   );

   var Information = Ext.create('Ext.Panel',
   {
      title : 'Information',
      html : '<p></p>',
      iconCls : 'info'
   }
   );

   var westpanel = Ext.create('Ext.Panel',
   {
      region : 'west',
      stateId : 'navigation-panel',
      id : 'west-panel', // see Ext.getCmp() below
      title : 'LiveGPS West Side',
      split : true,
      width : 200,
      minWidth : 175,
      maxWidth : 400,
      collapsible : true,
      animCollapse : true,
      margins : '0 0 0 5',
      layout : 'accordion',
      items : [Navigation, settings, Information]
   }
   );

   var eastpanel = Ext.create('Ext.Panel',
   {
      region : 'east',
      id : 'east-panel',
      title : 'East Side',
      animCollapse : true,
      collapsible : true,
      split : true,
      width : 225, // give east and west regions a width
      minWidth : 175,
      maxWidth : 400,
      margins : '0 0 0 0'
   }
   );

   var northpanel = Ext.create('Ext.Panel',
   {
      region : 'north',
      height : 32, // give north and south regions a height
      autoEl :
      {
         tag : 'div',
         html : '<p>GPSWeb</p>'
      }
   }
   );

 

   var infowindow = Ext.create('Ext.Window',
   {
      xtype : 'window',
      closable : false,
      minimizable : true,
      title : 'Info Window',
      height : 200,
      width : 200,
      constrain : true,
      html : '',
      itemId : 'center-window',
      minimize : function()
      {
         this.floatParent.down('button#toggleCw').toggle();
      }
   }
   );


    var centerpanel = Ext.create('Ext.Panel',
   {
      region : 'center',
//      region:'north',
      id : 'centerwin',
      // title : 'MapView',
      //autoHeight : true,
      titleCollapse : true,
      //split : true,
      unstyled : true,
      layout : 'fit',
      border : false,
      height : window.screen.height,
      // anchor : 'window.screen.height/2',
      width : '100%',
      maxWidth : window.screen.width - 2,
      maxHeight: window.screen.height - 2,
      //html : '<iframe id="mapframe" name="mapframe" src="./OpenLayerMap.aspx" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',
      //html: '<iframe id="mapframe" name="mapframe" src="./Messages/frmAlarms.aspx" width="100%" height="100%" scrolling="no"></iframe>',
      html : '<iframe id="mapframe" name="mapframe" src="./MapNew/vehiclemap.aspx" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',
      // '<iframe id="mapframe" name="mapframe" src="./map.html" width="100%" height="100%" frameborder="0" scrolling="no"></iframe>',
      margins : '0 0 0 0',
      autoScroll : true
   }
   );




   var viewport = Ext.create('Ext.Viewport',
   {
      layout : 'border',
      border : false,
      items : [centerpanel,vehiclegrid]
//     , renderTo :       renderBody()
   }
   );


   mainstore.load(
   {
      params :
      {
         QueryType : 'GetVehiclePosition',
         start: 0, 
         limit: pagesize
      }
   }
   );

   var vehicletask =
   {
      run : function ()
      {
         if(mainstore.isLoading() != true)
         {
           //if(lastProxyFinished==true)
           //{
            if(IsSyncOn)
            {
               // console.info('Map Refresh frequency is : ' + interval);
               recordUpdater.removeAll(true);
               //lastProxyFinished=false;
               recordUpdater.load(
               {
                  params :
                  {
                     QueryType : 'GetVehiclePosition',
                     start: 0, 
                     limit: pagesize
                  }
               }
               );
            }
           //}
           //else
           //{
            //lastProxyFinished=true;
           //}
         }
      }
      ,
      interval : interval // 5 second
   }
   var runner = new Ext.util.TaskRunner();

   runner.start(vehicletask);

}
);
