// JavaScript Document
/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
//http://mapfish.org/doc/tutorials/extjs.html
var mainstore;
var vehiclegrid;
var barPanel;
var proxyTimeOut = 120000;
//var dateformat = 'm/d/Y h:i a';
//var mapHTML = '<iframe scrolling="no" src="./frmManagingExtHOS.aspx?QueryType=DownloadFile"';

//var mapHTML = '<iframe scrolling="no" src=""'; //./frmManagingExtHOS.aspx?QueryType=DownloadFile&FileName=\\201307\\e19b4fc8-ed2b-4d24-a9fa-209c622ad057.pdf
var mapStyle = 'style="Height:100%; width:100%;  border:0;margin:0px"';
var mapHTML = '<iframe scrolling="no" src=""' + ' id="frPdfPanel' + '" name="frPdfPanel' + '" ' + mapStyle + '></iframe>'

var loadingMask;
var barChart;
var checkboxselection;
var framePanel;
var logsheetgrid;
var inspectiongrid;
var tabs;
var logTab;
var insTab;
var iDriverID = '';
var iDriverName = '';
var headerChecked = 0;
var selectedLogSheetFileName = [];
//var selectedLogSheetFileName = [];
var selectedLogSheetEventIndex = [];
var gridPageSize = 25;
var MaximumSelectCapacity = 100;
//var selectedInspectionSheetFileName = [];
var selectedInspectionSheetFileName = [];
var selectedInspectionSheetEventIndex = [];

var allSelected = false;
var buttonName = "";

var historyDateFrom;
var historyDateTo;
var inspectionDateFrom;
var inspectionDateTo;

var btnViewDriverLogsheet;
var btnViewDriverInspection;
var btnViewAllDriverLogsheet;
var btnViewAllDriverInspection;
var btnPrintInspectionMultipleSheet;
var btnPrintMultipleSheet;

var fleetButton;
var fleetWin;

//Export Related
var VehicleGridInSearchMode = false;
var LogSheetGridInSearchMode = false;
var InspectionGridInSearchMode = false;

var SearchedDriverName = '';

//Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setConfig(
{
   enabled : true//,
   //disableCaching: true
}
);

//Ext.Loader.setPath('Ext.ux', '../../SentinelFM/ExtJS/src/ux');
Ext.Loader.setPath('Ext.ux', '../sencha/extjs-4.1.0/examples/ux');
Ext.Loader.setPath('Ext.ux.exporter', '../sencha/Ext.ux.Exporter'); // Only the Ext.ux.exporter.* classes will be searched in ./something/exporter'

//Peiya
//Ext.Loader.setPath('Ext.ux', './extjs/examples/ux');
Ext.require([
'Ext.chart.*',
'Ext.grid.*',
'Ext.data.*',
'Ext.util.*',
'Ext.date.*',
//'Ext.ux.grid.FiltersFeature',
'Ext.ux.grid.FiltersFeature',
'Ext.layout.container.Fit',
'Ext.grid.PagingScroller',
'Ext.ux.exporter.Exporter.*'
]);

Ext.onReady(function () {

    Ext.tip.QuickTipManager.init();

    loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." });
    loadingMask.show();

    function fromUTC(inputDate) {
        // Convert the date from the UTC date
        var timezones = Ext.Date.getGMTOffset(inputDate, true).split(":");
        var timezoneHour = 60 * Number(timezones[0]);
        var timezoneminutes = Number(timezones[1]);
        var timezoneoffset = timezoneHour + timezoneminutes;
        return Ext.Date.add(inputDate, Ext.Date.MINUTE, timezoneoffset);
        //-inputDate.getTimezoneOffset());

    }

    // setup the state provider, all state information will be saved to a cookie
    Ext.state.Manager.setProvider(Ext.create('Ext.state.CookieProvider'));   
    //var dateformat = 'm/d/Y h:i a'; // 2011 - 12 - 06T11 : 50 : 19 - 05 : 00
    var dateformat = userdateformat; // 2011 - 12 - 06T11 : 50 : 19 - 05 : 00
    var interval = 60000;
    var IsSyncOn = true;

    function UTCStringToDate(dtStr, format) {
       
        var dt = Ext.Date.parse(dtStr, "Y-m-dTH:i:s.uP");
        //Ext.Date.parseDate(dtStr, format);
        if (dt == undefined) return ''; // or whatever you want to do
        dt = fromUTC(dt);
        dt = Ext.Date.format(dt, format);
        return dt;
        //dt.fromUTC();
    }

    function ExportToExcel(iGrid, fType) {
        try {
            var columnsp = "";
            var path = "";
            //alert(iGrid.id);
            if (iGrid.id == 'vehiclesgrid') {
                Ext.each(iGrid.columns, function (col, index) {
                    if (index == 9)
                        return;
                    columnsp += col.text + ':' + col.dataIndex + ',';
                });
                columnsp = columnsp.substring(0, columnsp.length - 1);
                path = '../HOS/HOSData.aspx?QueryType=ExportDriverStatus&formattype=' + fType + '&operation=Export' + '&columns=' + columnsp;
            }
            else if (iGrid.id == 'logsheetgrid') {
                Ext.each(iGrid.columns, function (col, index) {
                    //if (index == 9)
                    //    return;
                    columnsp += col.text + ':' + col.dataIndex + ',';
                });
                columnsp = columnsp.substring(0, columnsp.length - 1);
                var fDate = Ext.Date.format(historyDateFrom.value, userdateformat);//'m-d-Y g:i A'
                var tDate = Ext.Date.format(historyDateTo.value, userdateformat);//'m-d-Y g:i A'
                path = './frmManagingExtHOS.aspx?QueryType=ExportDriverLogSheet&formattype=' + fType + '&operation=Export' + '&columns=' + columnsp + '&fromDate=' + fDate + '&toDate=' + tDate + '&driverID=' + iDriverID;
            }

            else if (iGrid.id == 'inspectiongrid') {
                Ext.each(iGrid.columns, function (col, index) {
                    //if (index == 9)
                    //    return;
                    columnsp += col.text + ':' + col.dataIndex + ',';
                });
                columnsp = columnsp.substring(0, columnsp.length - 1);
                var fDate = Ext.Date.format(inspectionDateFrom.value, userdateformat);//'m-d-Y g:i A'
                var tDate = Ext.Date.format(inspectionDateTo.value, userdateformat);//'m-d-Y g:i A'
                path = './frmManagingExtHOS.aspx?QueryType=ExportDriverInspectionSheet&formattype=' + fType + '&operation=Export' + '&columns=' + columnsp + '&fromDate=' + fDate + '&toDate=' + tDate + '&driverID=' + iDriverID;
            }
            else {
                return;
            }

            var form = Ext.create('Ext.form.Panel', {
                xtype: 'form',
                itemId: 'uploadForm',
                hidden: true,
                standardSubmit: true,
                method: 'post',
                url: path
            });
            form.getForm().submit();
        }
        catch (err) {
            alert(err);
        }
    }

    Ext.define('DriverInfo',
   {
       extend: 'Ext.data.Model',
       fields: [
        { name: 'DriverID', type: 'int' },
        { name: 'DriverName', type: 'string' },
        { name: 'DisplayDate', type: 'date', dateFormat: 'c' },
       //, defaultValue: '', convert: function(v){return UTCStringToDate(v, dateformat);} 
       //"Y-m-d H:i:s");} 
       //},
        {name: 'LastUpdate', type: 'date', dateFormat: 'c'
       // , defaultValue: '', convert: function (v) { return UTCStringToDate(v, dateformat); }
    },
       //{name: 'BoxId', type: 'int'},
        {name: 'TotalHours', type: 'float' },
        { name: 'DrivenToday', type: 'float' },
        { name: 'HoursAvilable', type: 'float' },
        { name: 'Cycle', type: 'string' },
        { name: 'Signed', type: 'bool' },
        { name: 'InspectorSigned', type: 'bool' },
        { name: 'VehilceID', type: 'string' },
        { name: 'Status', type: 'string' },
        { name: 'Position', type: 'string' },
        { name: 'FleetId', type: 'string' }
       //{ name: 'BarChart', type: 'string' }
      ]//, uses: ['Ext.ux.exporter.Exporter']
   }
   );


    //FLEET HIRARCHY BUTTON
    fleetButton = Ext.create('Ext.Button',
{
    text: DefaultFleetName == '' ? DefaultFleetID : DefaultFleetName,
    id: 'fleetButton',
    tooltip: 'Select a fleet', //'Select a fleet', //ResSelectFleet
    //cls: 'cmbfonts',
    cls: 'x-btn-text-icon',
    icon: '../preview.png',
    //iconCls: 'map',
    textAlign: 'left',
    handler: function () {
        try {
            var url = "../Widgets/fleet.aspx?fleetId=" + SelectedFleetId + '&f=fleetButton';
            var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
           
            //if (fleetWin==undefined)
            fleetWin = openWindow(ResfleetButtonOpenwindowMessage, urlToLoad, 400, 150); //openWindow('Select a fleet', urlToLoad, 400, 150);
            //alert(ResfleetButtonOpenwindowMessage);
        }
        catch (err) {
            alert(err);
        }
    }
});
    //ORGANIZATION HIRARCHY BUTTON
    organizationHierarchy = Ext.create('Ext.Button',
{
    text: DefaultOrganizationHierarchyFleetName == '' ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName,
    id: 'organizationHierarchyButton',
    tooltip: 'Organization Hierarchy',
    cls: 'cmbfonts',
    textAlign: 'left',
    handler: function () {
        try {
            onOrganizationHierarchyNodeCodeClick();
        }
        catch (err) {
        }
    }
});
    //ORGANIZATION HIRARCHY BUTTON
    organizationHierarchy = Ext.create('Ext.Button',
{
    text: DefaultOrganizationHierarchyFleetName == '' ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName,
    id: 'organizationHierarchyButton',
    tooltip: 'Organization Hierarchy',
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

    //BUTTON: SEARCH
    var txtDriverName = Ext.create('Ext.form.field.Text', {
        id: 'txtDriverName',
        fieldLabel: 'Driver Name',
        tooltip: 'Enter driver name to search',
        width: 300,
        enableKeyEvents: true,
        listeners: {
            'keypress': function (field, event) {
                if (event.getKey() == event.ENTER) {
                    try {
                        SearchedDriverName = txtDriverName.getValue();
                        mainstore.currentPage = 1;
                        barPanel.setActiveTab(0);
                        mainstore.load({
                            params: {
                                driverName: txtDriverName.getValue(),
                                start: 0,
                                limit: DriverListPagesize
                            }
                        }
                        );
                        return;
                    }
                    catch (err) {
                        alert(err);
                    }
                }
            }
        }
    });
    var btnSearchDriver = Ext.create('Ext.Button', {
        icon: '../images/searchicon.png',
        cls: 'x-btn-text-icon',
        //margin: '0 0 0 5',
        //width: 100,
        //height: 56,
        id: 'btnSearchDriver',
        handler: function () {
            try {
                mainstore.currentPage = 1;
                barPanel.setActiveTab(0);
                loadingMask.show();
                SearchedDriverName = txtDriverName.getValue();
                mainstore.load({
                    params: {
                        driverName: txtDriverName.getValue(),
                        start: 0,
                        limit: DriverListPagesize
                    }
                }
                        );
                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    var btnClearSearch = Ext.create('Ext.Button', {
        icon: '../images/close.png',
        id: 'btnClearSearch',
        handler: function () {
            try {
                txtDriverName.setValue('');
                barPanel.setActiveTab(0);
                loadingMask.show();
                //mainstore.load({
                //    callback: function (records, operation, success) {
                //       alert(operation.response.responseText);
                //    }
                barChart.surface.removeAll();
                mainstore.currentPage = 1;
                SearchedDriverName = '';
                mainstore.load({
                    params: {
                        driverName: '',
                        start: 0,
                        limit: DriverListPagesize
                    }
                });
                //Ext.getCmp('barPanel').redraw();
                //Ext.getCmp('barPanel').refresh();

                //Ext.getCmp('barPanel').refresh(); 
                //                mainstore.load({
                //                    params: {
                //                        driverName: ''
                //                    }
                //                }
                //                        );
                return;

                //});
                //$('#barPanel-body. x-hide-visibility').removeClass(' x-hide-visibility'); 
                //alert($('#barPanel-body').find('.x-hide-visibility').length);
                //$('#barPanel-body').find('.x-hide-visibility').removeClass('x-hide-visibility');
                //alert($('#barPanel-body').find('.x-hide-visibility').length);

                //alert($('#barPanel-body').find('.x-hide-visibility').length);
                //$('#barPanel-body').find('.x-hide-visibility').replaceWith('');
                //alert($('#barPanel-body').find('.x-hide-visibility').length);
                //return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    var btnExportStatus = Ext.create('Ext.Button', {
        text: 'Export',
        id: 'btnExportStatus',
        handler: function () {
            try {
                alert('Export Status');
                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    //BUTTON: SEARCH


    //DRIVER INFO MODEL *************************************************************************
    Ext.define('DriverList',
    {
        extend: 'Ext.data.Model',
        fields: [
        { name: 'driverid', type: 'int' },
        { name: 'drivername', type: 'string' }
        ]
    }
    );
    //************************************************************************************

    //GRID LOGSHEET MODEL *************************************************************************
    Ext.define('GridLogsheetList',
   {
       extend: 'Ext.data.Model',
       fields: [
      { name: 'refid', type: 'string' },
       //{ name: 'drivername', type: 'string' },
      {name: 'date', type: 'date', dateFormat:"Y-m-dTH:i:s", defaultValue: '' }, //, convert: function (v) { return UTCStringToDate(v, 'm/d/Y h:i a'); } },
      {name: 'filename', type: 'string' },
      { name: 'inspection', type: 'string' },
      { name: 'offduty', type: 'float' },
      { name: 'sleeping', type: 'float' },
      { name: 'driving', type: 'float' },
      { name: 'onduty', type: 'float' },
      { name: 'drivername', type: 'string' },
      { name: 'FleetId', type: 'string' }
      ]
   }
   );

    //GRID INSPECTION MODEL *************************************************************************
    Ext.define('GridInspectionList',
   {
       extend: 'Ext.data.Model',
       fields: [
      { name: 'InsTime', type: 'date', dateFormat: 'Y-m-dTH:i:s', defaultValue: '' }, //, convert: function (v) { return UTCStringToDate(v, 'm/d/Y h:i a'); } },
      {name: 'trip', type: 'string' },
      { name: 'filename', type: 'string' },
      { name: 'defect', type: 'string' },
      { name: 'DriverName', type: 'string' },
      { name: 'VehicleID', type: 'string' },
      { name: 'Odometer', type: 'string' },
      { name: 'FleetId', type: 'string' },
      { name: 'image', type: 'string' } //Devin Added on 2014-08-29
      ]
   }
   );

    //************************************************************************************

    var todayDate = Ext.util.Format.date(new Date(), 'dmY');
    //(new Date(2012,1,2),'dmY');
    //(new Date(), 'd/m/Y');
    //Date(2012,1,2));
    //, 'd/m/Y');
    //Ext.Date.add(new Date(), Ext.Date.DAY, -4), 'dmY');//new Date(); 
    var barTitleDate = Ext.util.Format.date(new Date(), 'd/m/Y');
    //(2012,1,2), 'd/m/Y');
    //Ext.Date.add(new Date(), Ext.Date.DAY, -4), 'd/m/Y');//new Date(); 
    var barTitle = 'Driver\'s Hours of Service ' + barTitleDate;
    // + ' -- US 60/7 Cycle';

    var PieStore = Ext.create('Ext.data.JsonStore', {
        //model : 'DriverStatus'
        //,
        fields: ['name', 'val']
    });


    /*var*/ mainstore = Ext.create('Ext.data.Store',
   {
       model: 'DriverInfo',
       autoLoad: true,
       autosync: false,
       filterOnLoad: false,
       pageSize: DriverListPagesize,//35,
       // remoteSort : true,
       // buffered : true,
       storeId: 'DriverInfos',
       //sorters: ['LastUpdate'],
       sorters: [{ property: 'LastUpdate', direction: 'DESC'}],
       //direction: 'DESC',
       //groupField: 'LastUpdate',
       //groupDir: 'DESC',
       proxy:
      {
          // load using HTTP
          type: 'ajax',
          url: 'HOSData.aspx?QueryType=GetDriverStatus',
          extraParams: {
              FleetId: LoadVehiclesBasedOn == 'fleet' ? '' : DefaultOrganizationHierarchyFleetId
          },
          reader: {
              type: 'xml',
              root: 'HOS_DriverDashBoard',
              record: 'DriverDashBoard',
              totalProperty: 'totalCount'
          }
      },
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  var mainRecords = store.getRange();

                  //GridStore.loadData(mainRecords);
                  //alert(store.getCount());
                  //console.log("Count" + store.getCount());
                  //var data = [];
                  var barData = [];
                  //var MoreThan20 = 0, mayViolate = 0, alreadyViolated = 0;
                  //alert(operation.request.proxy.reader.rawData);
                  Ext.each(mainRecords, function (currecord) {
                      //console.log("inside now");
                      var varDate = Ext.util.Format.date(currecord.data.DisplayDate, 'dmY');
                      //console.log(todayDate);
                      //console.log(varDate);                
                      var barRecord = currecord;

                      barRecord.data.HoursUsed = currecord.data.DrivenToday;
                      barData.push(barRecord);
                  });

                  //$('#barPanel-body .x-hide-visibility').removeClass('x-hide-visibility');
              } catch (err) {
                  //console.log("Error in loading main store.. " + err.message);
              }
          }
          //,scope : this
      }
       //proxy :ajProxy     
   });

    // PAGING Left Grid
    var vehiclePager = new Ext.PagingToolbar(
   {
       store: mainstore,
       displayInfo: true,
       displayMsg: 'Displaying drivers {0} - {1} of {2}',
       emptyMsg: "No drivers to display",
       listeners: {
           beforechange: function () {
               mainstore.proxy.extraParams = { FleetId: LoadVehiclesBasedOn == 'fleet' ? SelectedFleetId : DefaultOrganizationHierarchyFleetId, driverName: SearchedDriverName };
               loadingMask.show();
           },

           change: function () {
               loadingMask.hide();
           }
       }
   }
   );
    // PAGING Left Grid

    var exportMenu = Ext.create('Ext.menu.Menu');

    var exportToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       //handler: Peter(vehiclegrid, 'csv')
       handler: function () {
           try {
               if (VehicleGridInSearchMode) {
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
                   //document.getElementById('filename').value = "DriverStatus";
                   //document.getElementById('formatter').value = "csv";
                   ////alert('ok');
                   //form.submit();
                   alert('VehicleGridInSearchMode:' + VehicleGridInSearchMode);
               }
               else {
                   ExportToExcel(vehiclegrid, 'csv');
               }
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
               if (VehicleGridInSearchMode) {
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
                   //document.getElementById('filename').value = "DriverStatus";
                   //document.getElementById('formatter').value = "excel2003";
                   ////alert('ok');
                   //form.submit();
                   alert('VehicleGridInSearchMode:' + VehicleGridInSearchMode);
               }
               else {
                   ExportToExcel(vehiclegrid, 'excel2003');
               }
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
               if (VehicleGridInSearchMode) {
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
                   //document.getElementById('filename').value = "DriverStatus";
                   //document.getElementById('formatter').value = "excel2007";
                   ////alert('ok');
                   //form.submit();
                   alert('VehicleGridInSearchMode:' + VehicleGridInSearchMode);
               }
               else {
                   ExportToExcel(vehiclegrid, 'excel2007');
               }
           }
           catch (err) {
               alert(err);
           }
       }
   }


    exportMenu.add(exportToCsvButton, exportToExcel2003Button, exportToExcel2007Button);

    var drvfilters =
   {
       ftype: 'filters',
       local: true,   // defaults to false (remote filtering)
       filters: [
          {
              type: 'int',
              dataIndex: 'DriverID'
          },
          {
              type: 'string',
              dataIndex: 'DriverName'
          },
          {
              type: 'string',
              dataIndex: 'VehilceID'
          },
          {
              type: 'date',
              dataIndex: 'LastUpdate'
          }
       ]
   }

    /*var*/vehiclegrid = Ext.create('Ext.grid.Panel',
   {
       id: 'vehiclesgrid',
       width: 505, //window.screen.width / 3, //'42%',
       //height: 600,
       region: 'west',
       enableColumnHide: false,
       collapsible: true,
       animCollapse: true,
       split: true,
       //selModel : selModel,
       title: 'Driver Status',
       // sm,
       //autoScroll : true,
       //renderTo: Ext.getBody(),
       // frame : true,
       enableSorting: true,
       verticalScrollerType: 'paginggridscroller',
       //  loadMask : false,
       // disableSelection : true,
       //    invalidateScrollerOnRefresh : false,
       //        titleCollapse : true,
       closable: false,
       //        collapsible : true,
       columnLines: true,
       //        resizable : true,
       //height : 100,
       //        title : 'VehicleList',
       store: mainstore,
       // store,
       //     renderTo : Ext.getBody(),
       features: [drvfilters],
       viewConfig:
      {
          emptyText: 'No driver data to display',
          // trackOver : false,
          useMsg: false
      }
      ,
       bbar: vehiclePager,
       columns: [
      {
          text: 'DriverID',
          align: 'left',
          width: 60,
          dataIndex: 'DriverID',
          filterable: true,
          sortable: true//,
          //          renderer: function (value) {
          //              return '<a href="#" OnClick="alert(\'' + value + '\');">' + value + '</a>';
          //}
      },

      {
          text: 'DriverName',
          align: 'left',
          width: 321,
          dataIndex: 'DriverName',
          filterable : true,
          sortable: true,
          //hidden: isCompact,
          //renderer: function (value) {
          //return '<a href="#" OnClick="alert(\'' + value + '\');">' + value + '</a>';
          //renderer: function (value, p, record) {
          //return Ext.String.format('<a href="#" OnClick="SensorInfoWindow(\'{0}\')">{1}</a>', Ext.String.escape(record.data['LicensePlate']), value);
          //return Ext.String.format('<a href="#" OnClick="alert(\'{0}\')">{1}</a>', record.data['DriverID'] + '?', value);
          renderer: function (value, p, record) {
              //return '<a href="#" OnClick="ShowFrmManagingHOSWindow()">' + value + '</a>';
              //return Ext.String.format('<a href="#" OnClick="ShowFrmManagingHOSWindow(\'{0}\')">{1}</a>', value, value);

              //return Ext.String.format('<a href="#" OnClick="addNewTab(\'{0}\',{1});">{2}</a>', value, record.data['DriverID'], value); // +'   ' + Ext.String.format('<a href="javascript:void(0);" OnClick="alert(\'Peiya\')"><img src="../images/Edit.gif" border="0"></a>') + '   ' + Ext.String.format('<a href="javascript:void(0);" OnClick="alert(\'Salman\')"><img src="../images/right.gif" border="0"></a>');
              //return Ext.String.format('<a href="#" onmouseover="alert(\'L.Display\')" onmouseout="alert(\'L.Hidden\')"  OnClick="addNewTab(\'{0}\',{1});">{2}</a>', value, record.data['DriverID'], value);

              //return Ext.String.format('<a href="#" OnClick="addNewTab(\'{0}\',{1});">{2}</a>', value, record.data['DriverID'], value) + '   ' + Ext.String.format('<a href="#" OnClick="alert(\'addNewLogTab for({0})\');">{1}</a>', record.data['DriverID'], 'Log') + '   ' + Ext.String.format('<a href="#" OnClick="alert(\'addNewInsTab for({0})\');">{1}</a>', record.data['DriverID'], 'Ins');

              //return Ext.String.format('<a href="#" onmouseover="showLI(true)" onmouseout="showLI(false)"  OnClick="addNewTab(\'{0}\',{1});">{2}</a>', value, record.data['DriverID'], value);
              //return Ext.String.format('{0} <span onmouseover="showLI(\'true\',\'{1}\')"></span>', value, record.data['DriverID']);

              //return Ext.String.format('<span onmouseover="showLI(\'true\',\'{0}\');">{1}</span>', record.data['DriverID'], value);
              //return Ext.String.format('<span onmouseover="<a href="#" OnClick="alert(\'LogTab for(1403)\');">Log</a>">{1}</span>', record.data['DriverID'], value);
              //return Ext.String.format('<span onmouseover="<a href="#" OnClick="alert(\'LogTab for(1403)\');">Log</a>">{1}</span>', record.data['DriverID'], value);
              return Ext.String.format('<span onmouseover="showLI({0});" onmouseout="hideLI({0});">{2} <span style="display:none;" id="LI{0}"><a href="javascript:void(0)" onclick="OpenLog({0},\'{1}\');">Log</a> <a href="javascript:void(0)" onclick="OpenIns({0},\'{1}\');">Ins</a><span></span>', record.data['DriverID'], record.data['DriverName'], value);
          }
      },
       
      {
      text: 'VehicleID',
      align: 'left',
      width: 90,
      dataIndex: 'VehilceID',
      filterable : true,
      sortable: true
  },
      {
          text: 'Status',
          align: 'left',
          width: 80,
          dataIndex: 'Status',
          //filterable : true,
          sortable: true
      },
      {
          text: 'Date/Time',
          align: 'left',
          width: 120,
          xtype: 'datecolumn',
          format: userdateformat,
          dataIndex: 'LastUpdate',
          filterable: true,
          sortable: true,
        

      },
      {
          text: 'Hours Used Today',
          align: 'left',
          width: 80,
          dataIndex: 'DrivenToday',
          //filterable : true,
          sortable: true
      },
      {
          text: 'Total Hours in Cycle',
          align: 'left',
          width: 80,
          dataIndex: 'TotalHours',
          //filterable : true,
          sortable: true
      },
      {
          text: 'Hours Available in Cycle',
          align: 'left',
          width: 80,
          dataIndex: 'HoursAvilable',
          //filterable : true,
          sortable: true
      },
      {
          text: 'Cycle',
          align: 'left',
          width: 70,
          dataIndex: 'Cycle',
          //filterable : true,
          sortable: true
      }
      ,
      {
          text: 'Position',
          align: 'left',
          width: 123,
          dataIndex: 'Position',
          hidden: true //set false for debug
      }

      ],
       dockedItems: [
      {
          xtype: 'toolbar',
          dock: 'top',
          items: [LoadVehiclesBasedOn == 'fleet' ? fleetButton : organizationHierarchy, txtDriverName, btnSearchDriver, btnClearSearch/*, btnExportStatus*/
          , { icon: '../preview.png',
              cls: 'x-btn-text-icon',
              text: 'Export',
              menu: exportMenu
          }
          ]
      }
      ],

       listeners:
            {
                'collapse': function (p, eOpts) {
                    framePanel.setWidth(document.body.clientWidth - 456);
                },
                'expand': function (p, eOpts) {
                    framePanel.setWidth(document.body.clientWidth - vehiclegrid.width - 430);
                }
            },
       filterupdate: function () {
           if (VehicleGridInSearchMode) {
               alert('VehicleGridInSearchMode:' + VehicleGridInSearchMode);
           }
           else {
               alert('VehicleGridInSearchMode:' + VehicleGridInSearchMode);
           }
       }
   }
   );

    var donut = false,
        piePanel = Ext.create('widget.panel', {
            width: '45%',
            height: '50%',
            //maxHeight: '400',
            //region: 'south',
            title: 'Overview of driver\'s hours of service violation',
            region: 'south',
            split: true,
            collapsible: true,
            animCollapse: true,
            //renderTo: Ext.getBody(),
            layout: 'fit',
            items: {
                xtype: 'chart',
                id: 'chartCmp',
                animate: true,
                store: PieStore,
                shadow: true,
                legend: {
                    position: 'right'
                },
                insetPadding: 60,
                theme: 'Base:gradients',
                series: [{
                    type: 'pie',
                    field: 'val',
                    showInLegend: true,
                    donut: 35,
                    tips: {
                        trackMouse: true,
                        width: 140,
                        height: 45,
                        renderer: function (storeItem, item) {
                            //calculate percentage.
                            var total = 0;
                            PieStore.each(function (rec) {
                                total += rec.get('val');
                            });
                            this.setTitle(storeItem.get('name') + ': ' + Math.round(storeItem.get('val') / total * 100) + '%');
                        }
                    },
                    highlight: {
                        segment: {
                            margin: 20
                        }
                    }
                }]
            }
        });

    barChart = Ext.create('Ext.chart.Chart', {
        animate: true,
        shadow: true,
        //store: BarStore,
        store: mainstore,
        legend: {
            position: 'right'
        },
        title: "Bar Chart",
        axes: [{
            type: 'Numeric',
            position: 'bottom',
            fields: ['DrivenToday', 'TotalHours', 'HoursAvilable'],
            title: 'Hours',
            grid: true//,
            //roundToDecimal: false
        }, {
            type: 'Category',
            position: 'left',
            fields: ['DriverName'],
            title: 'Drivers'
        }],
        series: [{
            type: 'bar',
            axis: 'top',
            xField: 'DriverName',
            yField: ['DrivenToday', 'TotalHours', 'HoursAvilable'],
            stacked: true,
            tips: {
                trackMouse: true,
                width: 300,
                height: 40,
                renderer: function (storeItem, item) {
                    this.setTitle(storeItem.get('DriverName') + ': ' + String(item.value[1]));
                }
            }
        }]
    });

    // iFRAME
    framePanel = Ext.create('Ext.Panel',
       {
           id: 'frPanel',
           //width: /*window.screen.width*/document.body.clientWidth - vehiclegrid.width - historyFormGridContainer.width - 5, /*window.screen.width / 2.30*/ //590, * 475-422
           width: vehiclegrid.width,
           //height: document.body.clientHeight - 56, //window.screen.height - 272,// / 1.3366, //808, // getIFHeight()
           height: document.body.clientHeight,
           //minHeight: 150,
           border: 0,
           region: 'east',
           //hidden:true//,
           //html: mapHTML + ' id="frPdfPanel' + currentIndexTab + '" name="frPdfPanel' + currentIndexTab + '" ' + mapStyle + '></iframe>'
           //html: mapHTML + ' id="frPdfPanel' + '" name="frPdfPanel' + '" ' + mapStyle + '></iframe>'
           html: mapHTML
       }
       );

    //framePanels.push(framePanel); //CMT
    //    //iFRAME

    barPanel = Ext.create('Ext.tab.Panel', {
        //width: '50%',
        //height: '50%',
        width: '100%',
        id: 'barPanel',
        height: 400,
        //maxHeight: '80%',
        title: barTitle,
        //region: 'center',
        region: 'center',
        collapsible: true,
        animCollapse: true,
        split: true,
        //renderTo: Ext.getBody(),
        layout: 'fit',
        items: [barChart, framePanel]
    });

    var eastPanel = Ext.create('Ext.Panel',
   {
       region: 'center',
       //width: (window.screen.width - (window.screen.width / 3)),
       width: (document.body.clientWidth - (document.body.clientWidth / 3)),
       //      region:'north',
       id: 'centerwin',
       // autoHeight : true,
       titleCollapse: true,
       split: true,
       unstyled: true,
       layout: 'border',
       //      items : [piePanel,barPanel],
       items: [barPanel],
       border: false

   });

    //WEST LOG
    //DATETIME/BTN
    /*var*/historyDateFrom = Ext.create('Ext.form.field.Date', {
        //anchor: '100%',
        labelWidth: 50,
        maxWidth: 160,
        width: 160,
        fieldLabel: 'From',
        name: 'historyDateFrom',
        format: userDate,
        //value: new Date()
        value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() - 14)
    });

    /*var*/historyDateTo = Ext.create('Ext.form.field.Date', {
        //anchor: '100%',
        labelWidth: 50,
        maxWidth: 160,
        width: 160,
        fieldLabel: 'To',
        name: 'historyDateTo',
        format: userDate,
        //value: (new Date()).getDate() + 1
        //value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate()+1)
        value: new Date()//,
        //margin: '0 0 0 10'//{top:0, right:0, bottom:0, left:10}
        //disabledDates: [disableDT()]
    });

    //BUTTON: PANEL
    /*var*/btnViewDriverLogsheet = Ext.create('Ext.Button', {
        //text: 'Logsheet(s)',
        icon: '../images/searchicon.png',
        cls: 'x-btn-text-icon',
        hidden: true,
        //margin: '0 0 0 5',
        //width: 87,
        id: 'ViewDriverLogsheets',
        handler: function () {
            try {
                //Ext.Msg.alert('Oops2', 'View Driver Logsheets...');
                //alert(historyDateTo.getValue() - historyDateFrom.getValue());
                if (historyDateTo.getValue() - historyDateFrom.getValue() > 7776000000) {//Convert values to -/+ days and return value (24*60*60*1000)>>30 days  2592000000 ms
                    Ext.Function.defer(function () {
                        alert('You cannot select more than 90 days.');
                    }, 100);
                    return;
                }

                loadPDFFile("");
                barPanel.setActiveTab(0);
                //alert('2');
                logsheetgrid.show();
                //alert('3');
                loadingMask.show();
                //alert('4');

                //alert(historyDateFrom.getValue());
                //alert(historyDateTo.getValue());
                //alert(iDriverID);
                logsheetgrid.setTitle('Logs: ' + '<span style="color:darkolivegreen">' + iDriverName + '</span>');
                framePanel.setTitle('Report (Logs): ' + '<span style="color:darkolivegreen">' + iDriverName + '</span>');

                GridLogsheetStore.load({
                    params: {
                        fromDate: historyDateFrom.value,
                        toDate: historyDateTo.value,
                        driverId: iDriverID,
                        FleetId: SelectedFleetId
                    }
                }
                        );
                //alert('?? ' + iDriverID + '->' + isNaN(iDriverID));
                //if (typeof driverId == 'number' && isNaN(iDriverID) == false) {
                if (iDriverID != '') {
                    logsheetgrid.columns[2].setVisible(false);
                    inspectiongrid.columns[3].setVisible(false);
                }
                else {
                    logsheetgrid.columns[2].setVisible(true);
                    inspectiongrid.columns[3].setVisible(true);
                }

                //iDriverID = '';
                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    /*var*/btnViewAllDriverLogsheet = Ext.create('Ext.Button', {
        text: 'Load All Logsheet(s)',
        icon: '../images/searchicon.png',
        cls: 'x-btn-text-icon',
        id: 'ViewAllDriverLogsheets',
        handler: function () {
            iDriverID = '';
            iDriverName = '';
            try {
                //alert(historyDateTo.getValue() - historyDateFrom.getValue());
                if (historyDateTo.getValue() - historyDateFrom.getValue() > 7776000000) {//Convert values to -/+ days and return value (24*60*60*1000)>>30 days  2592000000 ms
                    Ext.Function.defer(function () {
                        alert('You cannot select more than 90 days.');
                    }, 100);
                    return;
                }

                loadPDFFile("");
                barPanel.setActiveTab(0);
                logsheetgrid.show();
                loadingMask.show();

                logsheetgrid.setTitle('Logs: ' + '<span style="color:darkolivegreen">All Drivers</span>');
                framePanel.setTitle('Report (Logs): ' + '<span style="color:darkolivegreen">All Drivers</span>');

                GridLogsheetStore.load({
                    params: {
                        fromDate: historyDateFrom.value,
                        toDate: historyDateTo.value,
                        driverId: iDriverID,
                        FleetId: SelectedFleetId
                    }
                }
                        );
                //alert('?? ' + iDriverID + '->' + isNaN(iDriverID));
                //if (typeof driverId == 'number' && isNaN(iDriverID) == false) {
                if (iDriverID != '') {
                    logsheetgrid.columns[2].setVisible(false);
                    btnViewDriverLogsheet.setVisible(true);
                    btnViewDriverInspection.setVisible(true);
                }
                else {
                    logsheetgrid.columns[2].setVisible(true);
                    btnViewDriverLogsheet.setVisible(false);
                    btnViewDriverInspection.setVisible(false);
                }

                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    var btnClearLogSearch = Ext.create('Ext.Button', {
        icon: '../images/close.png',
        id: 'btnClearLogSearch',
        handler: function () {
            try {
                loadPDFFile("");
                barPanel.setActiveTab(0);

                loadingMask.show();
                GridLogsheetStore.load();
                loadingMask.hide();

                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });


    // GRID
    GridLogsheetStore = Ext.create('Ext.data.Store',
   {
       model: 'GridLogsheetList',
       autoLoad: false,
       autosync: false,
       pageSize: 25,
       // remoteSort : true,
       // buffered : true,
       storeId: 'GridLogsheetStore',
       sorters: [{ property: 'date', direction: 'DESC'}],
       proxy:
      {
          // load using HTTP
          type: 'ajax',
          url: 'frmManagingExtHOS.aspx?QueryType=GetDriverLogsheetList',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'NewDataSet',
             record: 'GetReportLogSheet', //'GetReportLogSheet_ByDriver'
             totalProperty: 'totalCount'
         }
      },
       listeners:
      {
          'load': function () {
              loadingMask.hide();
          }
         ,
          scope: this
      }
   });

    var filters =
   {
       ftype: 'filters',
       local: true,   // defaults to false (remote filtering)
       filters: [
          {
              type: 'string',
              dataIndex: 'refid'
          },
          {
              type: 'string',
              dataIndex: 'DriverName'
          }
      ]
   }
   ;

    //var loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." });
    // PAGING Logsheet Grid
    var logsheetPager = new Ext.PagingToolbar(
   {
       store: GridLogsheetStore,
       //id: 'logsheetPager' + currentIndexTab,
       displayInfo: true,
       displayMsg: 'Displaying logsheets {0} - {1} of {2}',
       emptyMsg: "No logsheet(s) to display",
       listeners: {
           beforechange: function () {
               //new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." }).show();
               loadingMask.show();
               //GridLogsheetStore.proxy.extraParams = { /*QueryType: 'GetDriverLogsheetList',*/fromDate: historyDateFrom.getValue(), toDate: historyDateTo.getValue(), driverId: id };
               GridLogsheetStore.proxy.extraParams = { /*QueryType: 'GetDriverLogsheetList',*/fromDate: historyDateFrom.value, toDate: historyDateTo.value, driverId: iDriverID };
           },

           change: function (pagingToolBar, changeEvent) {
               loadingMask.hide();
               pagingToolBar.start = GridLogsheetStore.currentPage;

               //Remember selection on page change
               GridLogsheetStore.each(function (record, idx) {
                   var tempLogGridFilename = record.data.filename.replace(/\\/g, '\\\\');
                   if (selectedLogSheetFileName.indexOf(tempLogGridFilename) > -1) {
                       LogSheetGridselModel.select(idx, true, true);
                   }
                   
               });
           }
       }
   });
               
           
    // Inspection Grid Check box Model

    var InspectionSheetGridselModel = Ext.create('Ext.selection.CheckboxModel', {
        checkOnly: true,
        enableKeyNav: false,
        multipageSelection: {},
        listeners:
        {
            selectionchange: function (selModel, selections) {
                var isChecked = this.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                if (isChecked && selections.length < 1) {
                    this.view.headerCt.child('gridcolumn[isCheckerHd]').el.removeCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                    this.clearSelections();
                }
                   
                if (Object.keys(selections).length == 0)
                    GridInspectionStore.each(function (record, idx) {
                        var tempInsGridFilename = record.data.filename.replace(/\\/g, '\\\\');
                        if (selectedInspectionSheetFileName.indexOf(tempInsGridFilename) > -1) {
                            var selected = record.index - (GridInspectionStore.currentPage - 1) * gridPageSize;
                            InspectionSheetGridselModel.select(selected, true, true);
                        }
                    });
            },

            deselect: function (selectionModel, record, index, eOpts) {
                if (record.data.filename != "") {
                    if (selectedInspectionSheetFileName.indexOf(record.data.filename.replace(/\\/g, '\\\\')) > -1) {
                        selectedInspectionSheetFileName.splice(selectedInspectionSheetFileName.indexOf(record.data.filename.replace(/\\/g, '\\\\')), 1);
                    }

                    if (selectedInspectionSheetEventIndex.indexOf(record.index) > -1) {
                        selectedInspectionSheetEventIndex.splice(selectedInspectionSheetEventIndex.indexOf(record.index), 1);
                    }
                }
            },
           
            select: function (selectionModel, record, index, eOpts) {
                if (record.data.filename != "") {

                    if (selectedInspectionSheetFileName.indexOf(record.data.filename) == -1) {
                        selectedInspectionSheetFileName.push(record.data.filename.replace(/\\/g, '\\\\'));
                    }
                    if (selectedInspectionSheetEventIndex.indexOf(record.index) == -1) {
                        selectedInspectionSheetEventIndex.push(record.index);
                    }
                }
            }
        }
    });


    //Logsheet Grid

    var LogSheetGridselModel = Ext.create('Ext.selection.CheckboxModel', {
        checkOnly: true,
        enableKeyNav: false,        
        listeners:
        {
            selectionchange: function (selModel, selections)
            {
                var isChecked = this.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                if (isChecked && selections.length < 1) {
                    this.view.headerCt.child('gridcolumn[isCheckerHd]').el.removeCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                    this.clearSelections();
                }

                if (Object.keys(selections).length == 0)
                    GridLogsheetStore.each(function (record, idx)
                    {
                        var tempFilename = record.data.filename.replace(/\\/g, '\\\\');
                        if (selectedLogSheetFileName.indexOf(record.data.tempFilename) > -1)
                        {                           
                            var selected = record.index - (GridLogsheetStore.currentPage - 1) * gridPageSize;
                            LogSheetGridselModel.select(selected, true, true);
                        }
                    });
            },

            deselect: function (selectionModel, record, index, eOpts) {                
                if (record.data.filename != "")
                {
                    if (selectedLogSheetFileName.indexOf(record.data.filename.replace(/\\/g, '\\\\')) > -1) {
                        selectedLogSheetFileName.splice(selectedLogSheetFileName.indexOf(record.data.filename.replace(/\\/g, '\\\\')), 1);
                    }
               
                    if (selectedLogSheetEventIndex.indexOf(record.index) > -1) {
                        selectedLogSheetEventIndex.splice(selectedLogSheetEventIndex.indexOf(record.index), 1);
                    }
                }
            },
            select: function (selectionModel, record, index, eOpts) {
                if (record.data.filename != "") {
                    if (selectedLogSheetFileName.indexOf(record.data.filename) == -1) {
                        selectedLogSheetFileName.push(record.data.filename.replace(/\\/g, '\\\\'));
                    }
                    if (selectedLogSheetEventIndex.indexOf(record.index) == -1) {
                        selectedLogSheetEventIndex.push(record.index);
                    }

                }
            }
        }
    });
   

    //BUTTON: Print Logsheet Button
       btnPrintMultipleSheet = Ext.create('Ext.Button', {
        text: 'Merge Pdf',
        id: 'btnPrintMultipleSheet',
        handler: function () {
            try {
                  buttonName = "Logsheet";
                  if (headerChecked == 1)
                  {
                      if (GridLogsheetStore.totalCount <= MaximumSelectCapacity)
                      {
                          allSelected = true;                          
                          loadMultiPDFFile(selectedLogSheetFileName, allSelected, buttonName)
                      }
                      else
                          alert('Please select less than ' + MaximumSelectCapacity);
                  }
                  else
                    {
                      if (selectedLogSheetFileName.length != 0)
                      {
                          if (selectedLogSheetFileName.length <= MaximumSelectCapacity) {
                              loadMultiPDFFile(selectedLogSheetFileName, allSelected, buttonName);
                              return;
                          }
                          else
                              alert('Please select less than ' + MaximumSelectCapacity);
                      }
                      else {
                            if (LogSheetGridselModel.selected.length != 0)
                            {
                               for (var i = 0; i < LogSheetGridselModel.selected.length; i++) 
                                {
                                  selectedLogSheetFileName.push(LogSheetGridselModel.selected.items[i].data.filename);
                                  //.replace(/\\/g, '\\\\'));
                                }
                                loadMultiPDFFile(selectedLogSheetFileName, allSelected, buttonName)
                             }                          
                            else
                              alert('Please select check box for print sheet');
                           }

                     }
               
                

            }
            catch (err) {
                alert(err);
            }
        }
    });

    //BUTTON: Print Inspection Button

      btnPrintInspectionMultipleSheet = Ext.create('Ext.Button', {
        text: 'Merge Pdf',
        id: 'btnPrintInspectionMultipleSheet',
        handler: function () {
            try {
                buttonName = "Inspectionsheet";
                if (headerChecked == 1)
                {
                    if (GridInspectionStore.totalCount <= MaximumSelectCapacity)
                    {
                        allSelected = true;
                        loadMultiPDFFile(selectedInspectionSheetFileName, allSelected, buttonName)
                    }
                    else
                        alert('Please select less than ' + MaximumSelectCapacity);
                }
                else
                {
                    if (selectedInspectionSheetFileName.length != 0)
                    {
                        if (selectedInspectionSheetFileName.length <= MaximumSelectCapacity)
                        {
                            loadMultiPDFFile(selectedInspectionSheetFileName, allSelected, buttonName);
                            return;
                        }
                        else
                            alert('Please select less than ' + MaximumSelectCapacity);
                    }
                    else
                        alert('Please select check box for print sheet');
                }
               
            }
            catch (err) {                
                alert(err);
            }
        }
    });



    //BUTTON: EXPORT
    var btnExportLogsheet = Ext.create('Ext.Button', {
        text: 'Export',
        id: 'btnExportLogsheet',
        handler: function () {
            try {
                alert('Export Logsheet: ' + driver);
                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    //BUTTON: EXPORT

    var exportLogsheetMenu = Ext.create('Ext.menu.Menu');

    var exportLSToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportLSToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               ExportToExcel(logsheetgrid, 'csv');
           }
           catch (err) {
               alert(err);
           }
       }
   }

    
    var exportLSToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportLSToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               ExportToExcel(logsheetgrid, 'excel2003');
           }
           catch (err) {
               alert(err);
           }
       }
   }


    var exportLSToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportLSToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               ExportToExcel(logsheetgrid, 'excel2007');
           }
           catch (err) {
               alert(err);
           }
       }
   }

    exportLogsheetMenu.add(exportLSToCsvButton, exportLSToExcel2003Button, exportLSToExcel2007Button);
    logsheetgrid = Ext.create('Ext.grid.Panel',
   {
       id: 'logsheetgrid',
       width: vehiclegrid.width, //getLGWidth()
       //height: /*vehiclegrid.height - 50, //*/window.screen.height / 1.42668, //757,
       //height: document.body.clientHeight - 80, //getLGHeight(),
       //maxWidth: window.screen.width,
       //maxHeight: 750,
       //maxHeight: document.body.clientHeight/*window.screen.height*/,
       //width: window.screen.width,
       //autoHeight: true,
       //split: true,
       //region: 'west',
       //style: 'margin:0 auto;margin-top:5px;',
       enableColumnHide: false,
       //collapsible: true,
       animCollapse: true,
       split: true,
       title: 'Log', //'Driver Logsheet(s)',
       //headerPosition: 'left',
       enableSorting: true,
       verticalScrollerType: 'paginggridscroller',
       closable: false,
       columnLines: true,
       store: GridLogsheetStore,
       features: [filters],
       selModel: LogSheetGridselModel,
       //hidden: true,
       autoScroll: true,
       //layout: 'fit',
       //autoScroll: true,
       viewConfig:
      {
          emptyText: 'No driver logsheet to display',
          useMsg: false,
          forceFit: true
      }
      ,
       bbar: logsheetPager,
       columns: [
         {
             text: 'ID',
             align: 'left',
             width: 70,
             dataIndex: 'refid',
             sortable: true,
             renderer: function (value, p, record) {
                 //return '<a href="#" OnClick="alert(\'' + value + '\');">' + value + '</a>';
                 //return Ext.String.format('<a href="#" OnClick="alert(\'{0}\');">{1}</a>', record.data['filename'].replace(/\\/g,'\\\\'), value);
                 return Ext.String.format('<a href="#" OnClick="loadPDFFile(\'{0}\');">{1}</a>', record.data['filename'].replace(/\\/g, '\\\\'), value);
             }
         },

         {
             text: 'Date',
             align: 'left',
             width: 70,
             xtype: 'datecolumn',
             format: userdateformat,
             dataIndex: 'date',
             filterable: true,
             sortable: true
         },

         {
             text: 'Driver Name',
             align: 'left',
             width: 100,
             dataIndex: 'drivername',
             filterable: true,
             sortable: true
         },

         {
             text: 'On Duty',
             align: 'left',
             width: 50,
             dataIndex: 'onduty',
             sortable: true
         },
         {
             text: 'Off Duty',
             align: 'left',
             width: 50,
             dataIndex: 'offduty',
             sortable: true
         },
         {
             text: 'Driving',
             align: 'left',
             width: 45,
             dataIndex: 'driving',
             sortable: true
         },
         {
             text: 'Sleeper',
             align: 'left',
             width: 45,
             dataIndex: 'sleeping',
             sortable: true
         }
      ],
       dockedItems: [
      {
          xtype: 'toolbar',
          dock: 'top',
          items: [historyDateFrom, historyDateTo, btnViewDriverLogsheet, btnViewAllDriverLogsheet, btnClearLogSearch,btnPrintMultipleSheet /*btnExportLogsheet*/
          //Ext.String.format('<a href="#" OnClick="loadPDFFile(\'{0}\',' + currentIndexTab + ');">{1}</a>', record.data['filename'].replace(/\\/g, '\\\\'), value);
          , { icon: '../preview.png',
              cls: 'x-btn-text-icon',
              text: 'Export',
              menu: exportLogsheetMenu
          }]
      }
      ],

       listeners: {
           'select': function (k, record, index, eOpts) {
               //alert(index);
               loadPDFFile(record.data['filename'].replace(/\\/g, '\\\\'));
               //Ext.String.format('<a href="#" OnClick="loadPDFFile(\'{0}\',' + currentIndexTab + ');">{1}</a>', record.data['filename'].replace(/\\/g, '\\\\'), value);
           }
       }
   }
   );


    //WEST INS
    //DATETIME/BTN
    /*var*/inspectionDateFrom = Ext.create('Ext.form.field.Date', {
        //anchor: '100%',
        labelWidth: 50,
        maxWidth: 160,
        width: 160,
        fieldLabel: 'From',
        name: 'inspectionDateFrom',
        format: userDate,
        //value: new Date()
        value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() - 30)
    });

    /*var*/inspectionDateTo = Ext.create('Ext.form.field.Date', {
        //anchor: '100%',
        labelWidth: 50,
        maxWidth: 160,
        width: 160,
        fieldLabel: 'To',
        name: 'inspectionDateTo',
        format: userDate,
        //value: (new Date()).getDate() + 1
        //value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate()+1)
        value: new Date()//,
        //margin: '0 0 0 10'//{top:0, right:0, bottom:0, left:10}
        //disabledDates: [disableDT()]
    });

    //BUTTON: PANEL
    /*var*/btnViewDriverInspection = Ext.create('Ext.Button', {
        //text: 'Logsheet(s)',
        icon: '../images/searchicon.png',
        cls: 'x-btn-text-icon',
        //margin: '0 0 0 5',
        //width: 87,
        id: 'ViewDriverInspections',
        hidden: true,
        handler: function () {
            try {
                //Ext.Msg.alert('Oops2', 'View Driver Logsheets...');
                //alert(historyDateTo.getValue() - historyDateFrom.getValue());
                if (inspectionDateTo.getValue() - inspectionDateFrom.getValue() > 7776000000) {//Convert values to -/+ days and return value (24*60*60*1000)>>30 days  2592000000 ms
                    Ext.Function.defer(function () {
                        alert('You cannot select more than 90 days.');
                    }, 100);
                    return;
                }

                loadPDFFile("");
                barPanel.setActiveTab(0);
                //alert('2');
                inspectiongrid.show();
                //alert('3');
                loadingMask.show();
                //alert('4');

                //alert(historyDateFrom.getValue());
                //alert(historyDateTo.getValue());
                //alert(iDriverID);
                inspectiongrid.setTitle('Inspections: ' + '<span style="color:darkolivegreen">' + iDriverName + '</span>');
                framePanel.setTitle('Report (Inspections): ' + '<span style="color:darkolivegreen">' + iDriverName + '</span>');

                GridInspectionStore.load({
                    params: {
                        fromDate: inspectionDateFrom.value,
                        toDate: inspectionDateTo.value,
                        driverId: iDriverID,
                        FleetId: SelectedFleetId
                    }
                }
                        );

                if (iDriverID != '') {
                    inspectiongrid.columns[3].setVisible(false);
                    logsheetgrid.columns[2].setVisible(false);
                }
                else {
                    inspectiongrid.columns[3].setVisible(true);
                    logsheetgrid.columns[2].setVisible(true);
                }

                //iDriverID = '';
                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    /*var*/btnViewAllDriverInspection = Ext.create('Ext.Button', {
        text: 'Load All Inspection(s)',
        icon: '../images/searchicon.png',
        cls: 'x-btn-text-icon',
        id: 'ViewAllDriverInspections',
        handler: function () {
            iDriverID = '';
            iDriverName = '';
            try {
                //alert(historyDateTo.getValue() - historyDateFrom.getValue());
                if (historyDateTo.getValue() - historyDateFrom.getValue() > 7776000000) {//Convert values to -/+ days and return value (24*60*60*1000)>>30 days  2592000000 ms
                    Ext.Function.defer(function () {
                        alert('You cannot select more than 90 days.');
                    }, 100);
                    return;
                }

                loadPDFFile("");
                barPanel.setActiveTab(0);
                inspectiongrid.show();
                loadingMask.show();

                inspectiongrid.setTitle('Inspections: ' + '<span style="color:darkolivegreen">All Drivers</span>');
                framePanel.setTitle('Report (Inspections): ' + '<span style="color:darkolivegreen">All Drivers</span>');

                GridInspectionStore.load({
                    params: {
                        fromDate: inspectionDateFrom.value,
                        toDate: inspectionDateTo.value,
                        driverId: iDriverID,
                        FleetId: SelectedFleetId
                    }
                }
                        );
                //alert('?? ' + iDriverID + '->' + isNaN(iDriverID));
                //if (typeof driverId == 'number' && isNaN(iDriverID) == false) {
                if (iDriverID != '') {
                    inspectiongrid.columns[3].setVisible(false);
                    btnViewDriverInspection.setVisible(true);
                    btnViewDriverLogsheet.setVisible(true);
                }
                else {
                    inspectiongrid.columns[3].setVisible(true);
                    btnViewDriverInspection.setVisible(false);
                    btnViewDriverLogsheet.setVisible(false);
                }

                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    var btnClearInsSearch = Ext.create('Ext.Button', {
        icon: '../images/close.png',
        id: 'btnClearInsSearch',
        handler: function () {
            try {
                loadPDFFile("");
                barPanel.setActiveTab(2);

                loadingMask.show();
                GridInspectionStore.load();
                loadingMask.hide();

                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    // GRID
    GridInspectionStore = Ext.create('Ext.data.Store',
   {
       model: 'GridInspectionList',
       autoLoad: false,
       autosync: false,
       pageSize: 25,
       // remoteSort : true,
       // buffered : true,
       storeId: 'GridInspectionStore',
       sorters: [{ property: 'InsTime', direction: 'DESC'}],
       proxy:
      {
          // load using HTTP
          type: 'ajax',
          url: 'frmManagingExtHOS.aspx?QueryType=GetDriverInspectionList',
          timeout: proxyTimeOut,
          reader:
         {
             type: 'xml',
             root: 'NewDataSet',
             record: 'GetReportInspectionsSheet', //'GetReportLogSheet'
             totalProperty: 'totalCount'
         }
      },
       listeners:
      {
          'load': function () {
              loadingMask.hide();
          }
         ,
          scope: this
      }
   });

    var ins_filters =
   {
       ftype: 'filters',
       local: true,   // defaults to false (remote filtering)
       filters: [
           {
               type: 'string',
               dataIndex: 'trip'
           },
           {
              type: 'string',
              dataIndex: 'filename'
          },
          {
              type: 'string',
              dataIndex: 'DriverName'
          },
          {
              type: 'string',
              dataIndex: 'VehilceID'
          }
      ]
   }
   ;

    //var loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." });
    // PAGING Logsheet Grid
    var inspectionPager = new Ext.PagingToolbar(
   {
       store: GridInspectionStore,
       //id: 'inspectionPager' + currentIndexTab,
       displayInfo: true,
       displayMsg: 'Displaying inspections {0} - {1} of {2}',
       emptyMsg: "No inspection(s) to display",
       listeners: {
           beforechange: function () {
               new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." }).show();
               //GridInspectionStore.proxy.extraParams = { /*QueryType: 'GetDriverLogsheetList',*/fromDate: inspectionDateFrom.getValue(), toDate: inspectionDateTo.getValue(), driverId: id };
               GridInspectionStore.proxy.extraParams = { /*QueryType: 'GetDriverLogsheetList',*/fromDate: inspectionDateFrom.value, toDate: inspectionDateTo.value, driverId: iDriverID };
           },

           change: function (pagingToolBar, changeEvent) {
               new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." }).hide();
               //loadingMask.hide();
               pagingToolBar.start = GridInspectionStore.currentPage;

               //Remember selection on page change
               GridInspectionStore.each(function (record, idx) {
                   var tempFilenameIns = record.data.filename.replace(/\\/g, '\\\\');
                   if (selectedInspectionSheetFileName.indexOf(tempFilenameIns) > -1) {
                       InspectionSheetGridselModel.select(idx, true, true);
                   }
               });
           }
       }
   });

         
    // PAGING Logsheet Grid

    //BUTTON: EXPORT
    var btnExportInspection = Ext.create('Ext.Button', {
        text: 'Export',
        id: 'btnExportInspection',
        handler: function () {
            try {
                alert('Export Inspection: ' + driver);
                return;
            }
            catch (err) {
                alert(err);
            }
        }
    });

    //BUTTON: EXPORT

    var exportInspectionMenu = Ext.create('Ext.menu.Menu');

    var exportINSToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportINSToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               ExportToExcel(inspectiongrid, 'csv');
           }
           catch (err) {
               alert(err);
           }
       }
   }


    var exportINSToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportINSToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               ExportToExcel(inspectiongrid, 'excel2003');
           }
           catch (err) {
               alert(err);
           }
       }
   }


    var exportINSToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportINSToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {
               ExportToExcel(inspectiongrid, 'excel2007');
           }
           catch (err) {
               alert(err);
           }
       }
   }

    exportInspectionMenu.add(exportINSToCsvButton, exportINSToExcel2003Button, exportINSToExcel2007Button);
    inspectiongrid = Ext.create('Ext.grid.Panel',
   {
       id: 'inspectiongrid',
       width: vehiclegrid.width, //getLGWidth()
       //height: /*vehiclegrid.height - 50, //*/window.screen.height / 1.42668, //757,
       //height: document.body.clientHeight - 80, //getLGHeight(),
       //maxWidth: window.screen.width,
       //maxHeight: 750,
       //maxHeight: document.body.clientHeight/*window.screen.height*/,
       //width: window.screen.width,
       //autoHeight: true,
       //split: true,
       //region: 'west',
       //style: 'margin:0 auto;margin-top:5px;',
       enableColumnHide: false,
       //collapsible: true,
       animCollapse: true,
       split: true,
       title: 'Ins', //'Driver Logsheet(s)',
       //headerPosition: 'left',
       enableSorting: true,
       verticalScrollerType: 'paginggridscroller',
       closable: false,
       columnLines: true,
       store: GridInspectionStore,
       features: [ins_filters],
       //hidden: true,
       autoScroll: true,
       selModel:InspectionSheetGridselModel,
       //layout: 'fit',
       //autoScroll: true,
       viewConfig:
      {
          emptyText: 'No driver inspection to display',
          useMsg: false,
          forceFit: true
      }
      ,
       bbar: inspectionPager,
       columns: [
         {
             text: 'Flag',
             align: 'center',
             width: 40,
             dataIndex: 'defect',
             filterable: true,
             sortable: true,
             renderer: function (value, p, record) {
                 //return '<a href="#" OnClick="alert(\'' + value + '\');">' + value + '</a>';
                 //return Ext.String.format('<a href="#" OnClick="alert(\'{0}\');">{1}</a>', record.data['filename'].replace(/\\/g,'\\\\'), value);
                 //return Ext.String.format('<a href="#" OnClick="loadPDFFile(\'{0}\');">{1}</a>', record.data['filename'].replace(/\\/g, '\\\\'), value);
                 if (value == '0')
                     return '<img src="../images/reefer_ptpass.png" />';
                 else if (value == '1')
                     return '<img src="../images/reefer_ptfail.png" />';
             }
         },

         {
             text: 'Inspection',
             align: 'left',
             width: 88,
             dataIndex: 'trip',
             filterable: true,
             sortable: true,
             renderer: function (value, p, record) {
                 return getValueByKey(value);
             }
         },

         {
             text: 'Date/Time',
             align: 'left',
             width: 70,
             xtype: 'datecolumn',
             format: userdateformat,
             dataIndex: 'InsTime',
             filterable: true,
             sortable: true
         },

         {
             text: 'Driver Name',
             align: 'left',
             width: 100,
             dataIndex: 'DriverName',
             filterable: true,
             sortable: true
         },
         {
             text: 'Vehicle ID',
             align: 'left',
             width: 60,
             dataIndex: 'VehicleID',
             filterable: true,
             sortable: true
         },
         {
             text: 'Odometer',
             align: 'left',
             width: 55,
             dataIndex: 'Odometer',
             sortable: true
         }, //Devin added image on 2014-08-29
         {
                      text: 'Image',
                      align: 'left',
                      width: 55,
                      dataIndex: 'image',
                      renderer: function (value, p, record) {
                          return viewInspectionImage(value);
                      }
         }
      ],
       dockedItems: [
      {
          xtype: 'toolbar',
          dock: 'top',
          items: [inspectionDateFrom, inspectionDateTo, btnViewDriverInspection, btnViewAllDriverInspection, btnClearInsSearch,btnPrintInspectionMultipleSheet
          , { icon: '../preview.png',
              cls: 'x-btn-text-icon',
              text: 'Export',
              menu: exportInspectionMenu
          }]
      }
      ],

       listeners: {
           'select': function (k, record, index, eOpts) {
               //alert(index);
               loadPDFFile(record.data['filename'].replace(/\\/g, '\\\\'));
               //Ext.String.format('<a href="#" OnClick="loadPDFFile(\'{0}\',' + currentIndexTab + ');">{1}</a>', record.data['filename'].replace(/\\/g, '\\\\'), value);
           }
       }
   }
   );
    //*********************************************************************************************************************************************

    //WEST TABS
    tabs = Ext.create('Ext.tab.Panel',
   {
       id: 'tabs',
       region: 'west',
       split: true,
       titleCollapse: false,
       header: false,
       autoScroll: true,
       border: false,
       //height: window.screen.height / 2,
       //width: window.screen.width / 2,
       height: 543, //getDocHeight() - (window.screen.height / 2),
       width: '45%', //getDocWidth() - (window.screen.width / 2),
       autoHeight: true,
       collapsible: true,
       animCollapse: true,
       deferredRender: false,
       activeTab: 0,     // first tab initially active
       autoDestroy: false,
       items: [vehiclegrid, logsheetgrid, inspectiongrid]
   }
   );

    var viewport = Ext.create('Ext.Viewport',
     {
         layout: 'border',
         border: false,
         //items: [vehiclegrid, eastPanel]
         items: [tabs, eastPanel]
         //     , renderTo :       renderBody()
     }
   );

    var vehicletask =
   {
       run: function () {
           if (mainstore.isLoading() != true) {
               if (IsSyncOn) {
                   mainstore.load();
                   //alert('loaded');
               }
           }
       }
       //      ,
       //       interval: interval // 5 second
   }
    var runner = new Ext.util.TaskRunner();

    runner.start(vehicletask);
});

function getValueByKey(s) {
    //s = s.toLowerCase();
    var arr = s.split(',');
    var result = '';
    
    for(i=0;i<arr.length;i++) {

        var arrN = arr[i].split('#');
        if (arrN.length == 4) {
            var color;
            if (arrN[3] == 1)
                color = "red";
            else
                color = "blue";
            result += "<a href='javascript:void(0)' onclick='loadPDFFile(\"" + arrN[2].replace(/\\/g, '\\\\') + "\")' style='color: " + color + "'>" + arrN[0] + " " + arrN[1] + "</a><br />";
        }
    }

    return result;
}

//Devin added for inspection image on 2014-08-29
function viewInspectionImage(s) {
    var result = '';
    var color;
    if (s.length > 0) {
    	color = "blue";
    	result = "<a href='javascript:void(0)' onclick='viewInspectionImageWin(\"" + s + "\")' style='color: " + color + "'>View</a>";
    	return result;
    }
}
function viewInspectionImageWin(queystr) {
    var mypage = '../hos/frmViewImage.aspx?' + queystr
    var myname = '';
    var w = 800;
    var h = 800;
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,'
    win = window.open(mypage, myname, winprops)
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}
function loadPDFFile(filename) {
    barPanel.setActiveTab(1);
    //framePanel.setTitle(token);
    // Google Chrome
    
    document.getElementById('frPdfPanel').style.height = (document.body.clientHeight - 56) + 'px';

    if (filename == "") {
        document.getElementById('frPdfPanel').src = "";
    }
    else {
        document.getElementById('frPdfPanel').src = "./frmManagingExtHOS.aspx?QueryType=DownloadFile&FileName=" + filename;
    }
}

function loadMultiPDFFile(filename, allSelected, buttonName) {
    barPanel.setActiveTab(1);
    //framePanel.setTitle(token);
    // Google Chrome

    document.getElementById('frPdfPanel').style.height = (document.body.clientHeight - 56) + 'px';

    if (filename.length == 0 && allSelected == "false") {
        document.getElementById('frPdfPanel').src = "";
    }
    else {
        document.getElementById('frPdfPanel').src = "./frmManagingExtHOS.aspx?QueryType=MultiDownloadFile&FileName=" + filename + "&headerChecked=" + allSelected + "&buttonName=" + buttonName;
    }
}

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

function showLI(driverId) {
    $('#LI' + driverId).show();
}

function hideLI(driverId) {
    $('#LI' + driverId).hide();
}

function OpenLog(driverId, driverName) {
    //alert('Log :' + driverId);
//    alert(historyDateFrom.value);
    //    alert(historyDateTo.value);

    iDriverID = driverId;
    iDriverName = driverName;

    if (document.getElementById('frPdfPanel') != null) {
        barPanel.setActiveTab(0);
        document.getElementById('frPdfPanel').src = "";
    }
    tabs.setActiveTab(1);

    logsheetgrid.setTitle('Logs: ' + '<span style="color:darkolivegreen">' + driverName + '</span>');
    framePanel.setTitle('Report (Logs): ' + '<span style="color:darkolivegreen">' + driverName + '</span>');
    //    alert(historyDateFrom.value);
    //    alert(historyDateTo.value);
    GridLogsheetStore.removeAll();
    loadingMask.show();
    GridLogsheetStore.load({
        params: {
            fromDate: historyDateFrom.value,
            toDate: historyDateTo.value,
            driverId: driverId
        }
    });
    
    //if(driverId
    //logsheetgrid.columns[2].setVisible(false);

    
    //if (typeof driverId == 'number' || isNaN(driverId) == false) {
    //if (driverId != ''){
    if (typeof driverId == 'number') {
        logsheetgrid.columns[2].setVisible(false);
        btnViewDriverLogsheet.setVisible(true);
        btnViewDriverInspection.setVisible(true);
    }
    else {
        logsheetgrid.columns[2].setVisible(true);
        btnViewDriverLogsheet.setVisible(false);
        btnViewDriverInspection.setVisible(false);
    }
}

function OpenIns(driverId, driverName) {

    iDriverID = driverId;
    iDriverName = driverName;

    if (document.getElementById('frPdfPanel') != null) {
        barPanel.setActiveTab(0);
        document.getElementById('frPdfPanel').src = "";
    }
    tabs.setActiveTab(2);

    inspectiongrid.setTitle('Inspections: ' + '<span style="color:darkolivegreen">' + driverName + '</span>');
    framePanel.setTitle('Report (Inspections): ' + '<span style="color:darkolivegreen">' + driverName + '</span>');
    //    alert(inspectionDateFrom.value);
    //    alert(inspectionDateTo.value);
    
    GridInspectionStore.removeAll();
    loadingMask.show();
    GridInspectionStore.load({
        params: {
            fromDate: inspectionDateFrom.value,
            toDate: inspectionDateTo.value,
            driverId: driverId
        }
    });

    if (typeof driverId == 'number') {
        inspectiongrid.columns[3].setVisible(false);
        btnViewDriverLogsheet.setVisible(true);
        btnViewDriverInspection.setVisible(true);
    }
    else {
        inspectiongrid.columns[3].setVisible(true);
        btnViewDriverLogsheet.setVisible(false);
        btnViewDriverInspection.setVisible(false);
    }
}

function OnFleetSelect(c, fleetId, fleetName, caller) {
    if (c) {
        if (caller == 'fleetButton') {
            fleetButton.setText(fleetName);
            //alert('1. ' + fleetName);
            SelectedFleetId = fleetId;
            SelectedFleetName = fleetName;
            
            try{
                mainstore.currentPage = 1;
                loadingMask.show();
                mainstore.load(
                    {
                        params:
                        {
                            //QueryType: 'GetDriverStatus',
                            FleetId: SelectedFleetId,
                            start: 0,
                            limit: DriverListPagesize
                        }
                    });
                //alert('F: ' + fleetId);
            }
            catch (err) {
                alert(err);
            }
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
    fleetWin.hide();
}



//http://www.sencha.com/forum/showthread.php?196418-extjs-chart-in-extjs-grid
//http://skirtlesden.com/ux/component-column
//how to render bar chart in grid using extjs
