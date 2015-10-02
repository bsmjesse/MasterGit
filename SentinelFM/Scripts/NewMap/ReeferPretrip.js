var ReeferPretripPageStore;
var ReeferPretripGrid;
var ReeferPretripPager;

function IniReeferPretrip() {
    var pretripLoaded = false;
    var ReeferPretripPagesize = 15;
    var dateformat = 'd/m/Y h:i a';
    Ext.define('ReeferPretripModel',
    {
        extend: 'Ext.data.Model',
        fields: [
            'ReeferNum', 'LocalRemote',
            {
                name: 'InitiatedTime', type: 'date', dateFormat: 'c'
            }
            ,{
                name: 'CompletedTime', type: 'date', dateFormat: 'c'
            }
            , 'PretripResult', 'BatteryVolt', 'FuelLevel', 'Alarms'
        ]
     });

     ReeferPretripPageStore = Ext.create('Ext.data.Store',
       {
           //buffered: true,
           pageSize: HistoryPagesize,
           storeId: 'ReeferPretripPageStore',
           model: 'ReeferPretripModel',
           autoLoad: false,
       
           proxy: {
               type: 'ajax',
               url: './historynew/historyservices_Reefer.aspx?st=gethistoryreeferpretrip',
               timeout: 600000,
               reader: {
                   type: 'xml',
                   root: 'ReeferPretripDataset',
                   record: 'ReeferPretrip',
                   totalProperty: 'totalCount'
               }
           }
           , listeners:
            {
                'load': function (store, records, options) {

                    try {
                        ReeferPretripGrid.setTitle("Pretrip");
                        iniVehicleGridPopup();
                    }
                    catch (err) {
                    }
                }
                ,
                scope: this
            }
        });

        var exportReeferPretripToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportReeferPretripToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = ReeferPretripGrid;
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


        var exportReeferPretripToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportReeferPretripToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = ReeferPretripGrid;
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


        var exportReeferPretripToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportReeferPretripToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = ReeferPretripGrid;
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

        var reeferPretripExportMenu = Ext.create('Ext.menu.Menu');

        reeferPretripExportMenu.add(exportReeferPretripToCsvButton, exportReeferPretripToExcel2003Button, exportReeferPretripToExcel2007Button);

    ReeferPretripPager = new Ext.PagingToolbar(
    {
       store: ReeferPretripPageStore,
       displayInfo: true,
       displayMsg: 'Displaying Pre-trip {0} - {1} of {2}',
       emptyMsg: "No Pre-trip to display",
       listeners: {
           beforechange: function (b, page, o) {

               try {
                   /*if (historyPagerDoc != '') {
                       historyPagerDoc = '';
                       return;
                   }

                   historygrid.getView().emptyText = 'loading...';

                   var fleetId;
                   if (LoadVehiclesBasedOn == 'fleet') {
                       fleetId = DefaultFleetID;
                   }
                   else {
                       fleetId = DefaultOrganizationHierarchyFleetId;
                   }*/

                   ReeferPretripGrid.setTitle("Loading...");

                   var historyDateTimeFrom = new Date();
                   
                   historyDateTimeFrom.setHours(historyDateTimeFrom.getHours() - ReeferDashboardDefaultTimeRange - 2);
                   var historyDateTimeTo = new Date();

                   ReeferPretripDateFrom = TwoDigits(historyDateTimeFrom.getMonth() + 1) + "/" + TwoDigits(historyDateTimeFrom.getDate()) + "/" + historyDateTimeFrom.getFullYear().toString();
                   ReeferPretripTimeFrom = TwoDigits(historyDateTimeFrom.getHours()) + ":" + TwoDigits(historyDateTimeFrom.getMinutes());

                   ReeferPretripDateTo = TwoDigits(historyDateTimeTo.getMonth() + 1) + "/" + TwoDigits(historyDateTimeTo.getDate()) + "/" + historyDateTimeTo.getFullYear().toString();
                   ReeferPretripTimeTo = TwoDigits(historyDateTimeTo.getHours()) + ":" + TwoDigits(historyDateTimeTo.getMinutes());

                   var fleetId;
                   if (LoadVehiclesBasedOn == 'fleet') {
                       fleetId = DefaultFleetID;
                   }
                   else {
                       fleetId = DefaultOrganizationHierarchyFleetId;
                   }

                   ReeferPretripPageStore.proxy.extraParams = {
                       historyFleet: fleetId,
                       historyDateFrom: ReeferPretripDateFrom,
                       historyTimeFrom: ReeferPretripTimeFrom,
                       historyDateTo: ReeferPretripDateTo,
                       historyTimeTo: ReeferPretripTimeTo,
                       limit: ReeferPretripPagesize,
                       fromsession: 0
                   };
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

   ReeferPretripGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'ReeferPretripGrid',
       enableColumnHide: true,
       title: 'Pretrip',
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
       store: ReeferPretripPageStore,
       collapsible: true,
       animCollapse: false,
       split: true,
       //features: [filters],
       //stateId: 'stateVGrid',
       stateful: false, // state should be preserved

       columns: [
            {
                id: 'ReeferNum',
                //stateId: 'stAddress',
                text: 'Reefer #',
                align: 'left',
                width: 100,
                dataIndex: 'ReeferNum',
                filterable: true,
                sortable: false
                /*renderer: function (value, p, record) {
                    return 'in development.';
                }*/
            }
            /*,{
                id: 'LocalRemote',
               //stateId: 'stDescription',
                text: 'Local / Remote',
               align: 'left',
               width: 150,
               dataIndex: 'LocalRemote',
               filterable: false,
               sortable: false
           }*/
            , {
                id: 'InitiatedTime',
                //stateId: 'stDescription',
                text: 'Initiated Time',
                align: 'left',
                width: 150,
                xtype: 'datecolumn',
                format: userdateformat,//dateformat,
                dataIndex: 'InitiatedTime',
                filterable: false,
                sortable: false
            }
            , {
                id: 'CompletedTime',
                //stateId: 'stDescription',
                text: 'Completed Time',
                align: 'left',
                width: 150,
                xtype: 'datecolumn',
                format: userdateformat,//dateformat,
                dataIndex: 'CompletedTime',
                filterable: false,
                sortable: false
            }
            , {
                id: 'PretripResult',
                //stateId: 'stDescription',
                text: 'Result',
                align: 'left',
                width: 150,
                dataIndex: 'PretripResult',
                filterable: false,
                sortable: false,
                renderer: function (value, p, record) {
                  //var vehicleId = record.get('VehicleId');
                  if(value == "Pass")
                    return value;
                  else
                      return Ext.String.format('<a href="javascript:void(0);" rel="bootstrapreeferpretripresultpopover" data-mouseover="false" data-popup="false" class="withajaxpopover" data-content="loading..." data-html="true" data-title="{0}" data-poload="{1}" data-placement="right" data-container="body" >{0}</a>', value, record.data['Alarms']);
              }
            }
            , {
                id: 'BatteryVolt',
                //stateId: 'stDescription',
                text: 'BatteryVolt',
                align: 'left',
                width: 150,
                dataIndex: 'BatteryVolt',
                filterable: false,
                sortable: false
            }
            , {
                id: 'FuelLevel',
                //stateId: 'stDescription',
                text: 'Fuel Level',
                align: 'left',
                width: 150,
                dataIndex: 'FuelLevel',
                filterable: false,
                sortable: false
            }

        ]
        ,dockedItems: {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                { icon: 'preview.png',
                    cls: 'x-btn-text-icon',
                    text: 'Export',
                    menu: reeferPretripExportMenu
                }
           ]
        }
       , bbar: ReeferPretripPager
       , listeners: {
           'activate': function (grid, eOpts) {
               if (pretripLoaded)
                   return;

               ReeferPretripGrid.setTitle("Loading...");

               pretripLoaded = true;
               var historyDateTimeFrom = new Date();
               //historyDateTimeFrom = historyDateTimeFrom.addHours(-ReeferDashboardDefaultTimeRange);

               historyDateTimeFrom.setHours(historyDateTimeFrom.getHours() - ReeferDashboardDefaultTimeRange - 2);
               var historyDateTimeTo = new Date();

               ReeferPretripDateFrom = TwoDigits(historyDateTimeFrom.getMonth() + 1) + "/" + TwoDigits(historyDateTimeFrom.getDate()) + "/" + historyDateTimeFrom.getFullYear().toString();
               ReeferPretripTimeFrom = TwoDigits(historyDateTimeFrom.getHours()) + ":" + TwoDigits(historyDateTimeFrom.getMinutes());

               ReeferPretripDateTo = TwoDigits(historyDateTimeTo.getMonth() + 1) + "/" + TwoDigits(historyDateTimeTo.getDate()) + "/" + historyDateTimeTo.getFullYear().toString();
               ReeferPretripTimeTo = TwoDigits(historyDateTimeTo.getHours()) + ":" + TwoDigits(historyDateTimeTo.getMinutes());

               var fleetId;
               if (LoadVehiclesBasedOn == 'fleet') {
                   fleetId = DefaultFleetID;
               }
               else {
                   fleetId = DefaultOrganizationHierarchyFleetId;
               }
               ReeferPretripPageStore.load(
                {
                    params:
                    {
                        historyFleet: fleetId,
                        historyDateFrom: ReeferPretripDateFrom,
                        historyTimeFrom: ReeferPretripTimeFrom,
                        historyDateTo: ReeferPretripDateTo,
                        historyTimeTo: ReeferPretripTimeTo,
                        limit: ReeferPretripPagesize
                    }
                });
           }
       }
   });

    tabs.add(ReeferPretripGrid); 
}