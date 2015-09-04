var ReeferPretripPageStore;
var ReeferPretripGrid;
var ReeferPretripPager;

var ReeferImpactPageStore;
var ReeferImpactGrid;
var ReeferImpactPager;

function IniReeferPretrip() {
    var pretripLoaded = false;
    var impactLoaded = false;
    var ReeferPretripPagesize = 1000;
    var ReeferImpactPagesize = 1000;
    var dateformat = 'd/m/Y h:i a';

    var threeMonthEarlier = new Date();
    threeMonthEarlier.setMonth(threeMonthEarlier.getMonth() - 3);

    // Impact
    Ext.define('ReeferImpactModel',
    {
        extend: 'Ext.data.Model',
        fields: [
            'ReeferNum',
            {
                name: 'OriginDateTime', type: 'date', dateFormat: 'c'
            }
            , 'StreetAddress', 'ImpactType', 'PeakG', 'DeltaV', 'Speed', 'MoveStatus'
        ]
    });

    ReeferImpactPageStore = Ext.create('Ext.data.Store',
       {
           //buffered: true,
           pageSize: ReeferImpactPagesize, //HistoryPagesize,
           storeId: 'ReeferImpactPageStore',
           model: 'ReeferImpactModel',
           autoLoad: false,

           proxy: {
               type: 'ajax',
               url: './historynew/historyservices_Reefer.aspx?st=gethistoryreeferimpact',
               timeout: 600000,
               reader: {
                   type: 'xml',
                   root: 'ReeferImpactDataset',
                   record: 'ReeferImpact',
                   totalProperty: 'totalCount'
               }
           }
           , listeners:
            {
                'load': function (store, records, options) {

                    try {
                        //iniVehicleGridPopup();
                        loadingMask.hide();
                        ReeferPretripGrid.hide();
                        ReeferImpactGrid.show();
                        reeferAdvanceFormFieldContainer.hide();
                    }
                    catch (err) {
                        loadingMask.hide();
                        ReeferPretripGrid.hide();
                        ReeferImpactGrid.show();
                    }
                }
                ,
                scope: this
            }
       });

    var exportReeferImpactToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportReeferImpactToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = ReeferImpactGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "reeferimpacts";
               document.getElementById('formatter').value = "csv";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

    var exportReeferImpactToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportReeferImpactToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = ReeferImpactGrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "reeferImpact";
               document.getElementById('formatter').value = "excel2003";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

    var exportReeferImpactToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportReeferImpactToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = ReeferImpactGrid;
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
               document.getElementById('filename').value = "reeferimpacts";
               document.getElementById('formatter').value = "excel2007";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

    var reeferImpactExportMenu = Ext.create('Ext.menu.Menu');
    reeferImpactExportMenu.add(exportReeferImpactToCsvButton, exportReeferImpactToExcel2003Button, exportReeferImpactToExcel2007Button);

    ReeferImpactPager = new Ext.PagingToolbar(
    {
        store: ReeferImpactPageStore,
        displayInfo: true,
        displayMsg: 'Displaying Impact {0} - {1} of {2}',
        emptyMsg: "No Impact to display",
        listeners: {
            beforechange: function (b, page, o) {

                try {

                    var historyDateTimeFrom = new Date();

                    historyDateTimeFrom.setHours(historyDateTimeFrom.getHours() - ReeferDashboardDefaultTimeRange - 2);
                    var historyDateTimeTo = new Date();

                    var ReeferAdvanceDateFrom = TwoDigits(reeferAdvanceDateFrom.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateFrom.value.getDate()) + "/" + reeferAdvanceDateFrom.value.getFullYear().toString();
                    var ReeferAdvanceTimeFrom = TwoDigits(reeferAdvanceTimeFrom.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeFrom.value.getMinutes());

                    var ReeferAdvanceDateTo = TwoDigits(reeferAdvanceDateTo.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateTo.value.getDate()) + "/" + reeferAdvanceDateTo.value.getFullYear().toString();
                    var ReeferAdvanceTimeTo = TwoDigits(reeferAdvanceTimeTo.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeTo.value.getMinutes());

                    var fleetId;
                    if (LoadVehiclesBasedOn == 'fleet') {
                        fleetId = DefaultFleetID;
                    }
                    else {
                        fleetId = DefaultOrganizationHierarchyFleetId;
                    }

                    ReeferImpactPageStore.proxy.extraParams = {
                        historyFleet: fleetId,
                        historyDateFrom: ReeferAdvanceDateFrom,
                        historyTimeFrom: ReeferAdvanceTimeFrom,
                        historyDateTo: ReeferAdvanceDateTo,
                        historyTimeTo: ReeferAdvanceTimeTo,
                        limit: ReeferImpactPagesize,
                        fromsession: 1
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

    ReeferImpactGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'ReeferImpactGrid',
       enableColumnHide: true,
       title: '',
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
       store: ReeferImpactPageStore,
       collapsible: false,
       animCollapse: false,
       split: true,
       hidden: true,
       anchor: '-10, -45',
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
            , {
                id: 'ImpactOriginDateTime',
                //stateId: 'stDescription',
                text: 'Date Time',
                align: 'left',
                width: 150,
                xtype: 'datecolumn',
                format: userdateformat, //dateformat,
                dataIndex: 'OriginDateTime',
                filterable: false,
                sortable: false
            }
            , {
                id: 'ImpactStreetAddress',
                //stateId: 'stDescription',
                text: 'Location',
                align: 'left',
                width: 150,
                dataIndex: 'StreetAddress',
                filterable: false,
                sortable: false
            }
            , {
                id: 'ImpactType',
                //stateId: 'stDescription',
                text: 'Impact Type',
                align: 'left',
                width: 150,
                dataIndex: 'ImpactType',
                filterable: false,
                sortable: false
            }
            , {
                id: 'ImpactPeakG',
                //stateId: 'stDescription',
                text: 'Peak G (m/s<sup>2</sup>)',
                align: 'left',
                width: 150,
                dataIndex: 'PeakG',
                filterable: false,
                sortable: false
            }
            , {
                id: 'ImpactDeltaV',
                //stateId: 'stDescription',
                text: 'Delta V (m/s<sup>2</sup>)',
                align: 'left',
                width: 150,
                dataIndex: 'DeltaV',
                filterable: false,
                sortable: false
            }
            , {
                id: 'ImpactSpeed',
                //stateId: 'stDescription',
                text: 'Speed (km/s)',
                align: 'left',
                width: 150,
                dataIndex: 'Speed',
                filterable: false,
                sortable: false
            }
            , {
                id: 'ImpactMoveStatus',
                //stateId: 'stDescription',
                text: 'Move Status',
                align: 'left',
                width: 150,
                dataIndex: 'MoveStatus',
                filterable: false,
                sortable: false
            }

        ]
        , dockedItems: {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                'Impact',
                { icon: 'preview.png',
                    cls: 'x-btn-text-icon',
                    text: 'Export',
                    menu: reeferImpactExportMenu
                }
           ]
        }
       , bbar: ReeferImpactPager       
   });

    
    //   Advance Search Submit
    var btnReeferImpactSubmit = Ext.create('Ext.Button', {
        text: 'View Impact',
        cls: 'cmbfonts',
        //margin: '10 auto',
        style: { margin: '10px 0 10px 10px' },
        width: 100,
        handler: function () {
            try {

                /*
                if (pretripLoaded)
                return;
                */

                loadingMask.show();
                
                impactLoaded = true;
                var historyDateTimeFrom = new Date();
                //historyDateTimeFrom = historyDateTimeFrom.addHours(-ReeferDashboardDefaultTimeRange);

                historyDateTimeFrom.setHours(historyDateTimeFrom.getHours() - ReeferDashboardDefaultTimeRange - 2);
                var historyDateTimeTo = new Date();

                var ReeferAdvanceDateFrom = TwoDigits(reeferAdvanceDateFrom.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateFrom.value.getDate()) + "/" + reeferAdvanceDateFrom.value.getFullYear().toString();
                var ReeferAdvanceTimeFrom = TwoDigits(reeferAdvanceTimeFrom.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeFrom.value.getMinutes());

                var ReeferAdvanceDateTo = TwoDigits(reeferAdvanceDateTo.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateTo.value.getDate()) + "/" + reeferAdvanceDateTo.value.getFullYear().toString();
                var ReeferAdvanceTimeTo = TwoDigits(reeferAdvanceTimeTo.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeTo.value.getMinutes());

                var fleetId;
                if (LoadVehiclesBasedOn == 'fleet') {
                    fleetId = DefaultFleetID;
                }
                else {
                    fleetId = DefaultOrganizationHierarchyFleetId;
                }
                ReeferImpactPageStore.load(
                {
                    params:
                    {
                        historyFleet: fleetId,
                        historyDateFrom: ReeferAdvanceDateFrom,
                        historyTimeFrom: ReeferAdvanceTimeFrom,
                        historyDateTo: ReeferAdvanceDateTo,
                        historyTimeTo: ReeferAdvanceTimeTo,
                        limit: ReeferImpactPagesize,
                        fromsession: 0
                    }
                });
            }
            catch (err) {
                loadingMask.hide();
                ReeferImpactGrid.show();
            }
        }
    });

    
    // Pretrip

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
            , 'PretripResult', 'BatteryVolt', 'FuelLevel', 'AlarmDesc'
        ]
     });

    ReeferPretripPageStore = Ext.create('Ext.data.Store',
       {
           //buffered: true,
           pageSize: ReeferPretripPagesize,//HistoryPagesize,
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
                        //reeferPrtForm.setTitle("Event");

                        loadingMask.hide();
                        ReeferImpactGrid.hide();
                        ReeferPretripGrid.show();
                        reeferAdvanceFormFieldContainer.hide();
                        setTimeout(function () { iniVehicleGridPopup(); }, 1000);
                    }
                    catch (err) {
                        loadingMask.hide();
                        ReeferImpactGrid.hide();
                        ReeferPretripGrid.show();
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
               document.getElementById('filename').value = "reeferPretrip";
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
               document.getElementById('filename').value = "reeferPretrip";
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

                   var ReeferAdvanceDateFrom = TwoDigits(reeferAdvanceDateFrom.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateFrom.value.getDate()) + "/" + reeferAdvanceDateFrom.value.getFullYear().toString();
                   var ReeferAdvanceTimeFrom = TwoDigits(reeferAdvanceTimeFrom.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeFrom.value.getMinutes());

                   var ReeferAdvanceDateTo = TwoDigits(reeferAdvanceDateTo.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateTo.value.getDate()) + "/" + reeferAdvanceDateTo.value.getFullYear().toString();
                   var ReeferAdvanceTimeTo = TwoDigits(reeferAdvanceTimeTo.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeTo.value.getMinutes());

                   var fleetId;
                   if (LoadVehiclesBasedOn == 'fleet') {
                       fleetId = DefaultFleetID;
                   }
                   else {
                       fleetId = DefaultOrganizationHierarchyFleetId;
                   }

                   loadingMask.show();

                   ReeferPretripPageStore.proxy.extraParams = {
                       historyFleet: fleetId,
                       historyDateFrom: ReeferAdvanceDateFrom,
                       historyTimeFrom: ReeferAdvanceTimeFrom,
                       historyDateTo: ReeferAdvanceDateTo,
                       historyTimeTo: ReeferAdvanceTimeTo,
                       limit: ReeferPretripPagesize,
                       fromsession: 1
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
       title: '',
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
       collapsible: false,
       animCollapse: false,
       split: true,
       anchor: '-10, -45',
       hidden: true,
       //features: [filters],
       //stateId: 'stateVGrid',
       stateful: false, // state should be preserved

       columns: [
            {
                id: 'reeferPretrip',
                text: '',
                align: 'left',
                width: 30,
                filterable: false,
                dataIndex: 'PretripResult',
                renderer: function (value, p, record) {
                    if (value == 'Pass')
                        return '<img src="images/reefer_ptpass.png" />';
                    else if (value == 'Fail') {
                        var sTitle = 'PreTrip Result For Asset: ' + Ext.String.format(record.data['ReeferNum']) + '<br /><br />' + 'Overall Result:' + value;
                        return Ext.String.format('<a href="javascript:void(0);" rel="bootstrapreeferpretripresultpopover" data-mouseover="false" data-popup="false" class="withajaxpopover" data-content="loading..." data-html="true" data-title="{2}" data-poload="{1}" data-placement="right" data-container="body" ><img src="images/reefer_ptfail.png" /></a>', value, record.data['AlarmDesc'], sTitle);                        
                    }
                    else
                        return "";
                },
                sortable: false
            },
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
            , {
                id: 'LocalRemote',
                //stateId: 'stDescription',
                text: 'Source',
                align: 'left',
                width: 100,
                dataIndex: 'LocalRemote',
                filterable: false,
                sortable: false
            }
            , {
                id: 'InitiatedTime',
                //stateId: 'stDescription',
                text: 'Initiated Time',
                align: 'left',
                width: 150,
                xtype: 'datecolumn',
                format: userdateformat, //dateformat,
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
                format: userdateformat, //dateformat,
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
                    if (value == "Pass")
                        return value;
                    else if (value == "Fail") {

                        var sTitle = 'PreTrip Result For Asset: ' + Ext.String.format(record.data['ReeferNum']) + '</br></br>' + 'Overall Result:' + value;
                        return Ext.String.format('<a href="javascript:void(0);" rel="bootstrapreeferpretripresultpopover" data-mouseover="false" data-popup="false" class="withajaxpopover" data-content="loading..." data-html="true" data-title="{2}" data-poload="{1}" data-placement="right" data-container="body" >{0}</a>', value, record.data['AlarmDesc'], sTitle);
                    }
                    else {
                        return "";
                    }
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
                text: 'Fuel Level (%)',
                align: 'left',
                width: 150,
                dataIndex: 'FuelLevel',
                filterable: false,
                sortable: false
            }

        ]
        , dockedItems: {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                'Pre-trip',
                { icon: 'preview.png',
                    cls: 'x-btn-text-icon',
                    text: 'Export',
                    menu: reeferPretripExportMenu
                }
           ]
        }
       , bbar: ReeferPretripPager

   });
       
    //tabs.add(ReeferPretripGrid);

    //  Date Time Range

    //  Advance Search
   var btnReeferAdvanceSearch = Ext.create('Ext.Button',
       {
           text: 'Advanced Search',
           id: 'btmReeferPrtSearch',
           tooltip: 'Search Reefer PreTrip',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'centre',
           width: 150,
           handler: function () {
               try {
                   if (reeferAdvanceFormFieldContainer.isHidden()) {

                       //ReeferPretripGrid.show();
                       reeferAdvanceFormFieldContainer.show();
                   }

                   else {
                       //ReeferPretripGrid.hide();
                       reeferAdvanceFormFieldContainer.hide();
                   }


               }
               catch (err) {
               }
           }
       }
       );

       
       //   Advance Search Submit
       var btnReeferPrtSubmit = Ext.create('Ext.Button', {
           text: 'View Pre-trip',
           cls: 'cmbfonts',
           //margin: '10 auto',
           style: { margin: '10px 0 10px 10px' },
           width: 100,
           handler: function () {
               try {

                   /*
                   if (pretripLoaded)
                   return;
                   */
                   
                   loadingMask.show();
                   
                   pretripLoaded = true;

                   var ReeferAdvanceDateFrom = TwoDigits(reeferAdvanceDateFrom.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateFrom.value.getDate()) + "/" + reeferAdvanceDateFrom.value.getFullYear().toString();
                   var ReeferAdvanceTimeFrom = TwoDigits(reeferAdvanceTimeFrom.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeFrom.value.getMinutes());

                   var ReeferAdvanceDateTo = TwoDigits(reeferAdvanceDateTo.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateTo.value.getDate()) + "/" + reeferAdvanceDateTo.value.getFullYear().toString();
                   var ReeferAdvanceTimeTo = TwoDigits(reeferAdvanceTimeTo.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeTo.value.getMinutes());

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
                        historyDateFrom: ReeferAdvanceDateFrom,
                        historyTimeFrom: ReeferAdvanceTimeFrom,
                        historyDateTo: ReeferAdvanceDateTo,
                        historyTimeTo: ReeferAdvanceTimeTo,
                        limit: ReeferPretripPagesize,
                        fromsession: 0
                    }
                });
               }
               catch (err) {
                   loadingMask.hide();
                   ReeferPretripGrid.show();
               }
           }
       });


       reeferAdvanceDateFrom = Ext.create('Ext.form.field.Date', {
        anchor: '100%',
        labelWidth: 50,
        width: 190,
        maxWidth: 190,
        fieldLabel: 'From',
        name: 'pretripDateFrom',
        format: 'm/d/Y',
        //value: new Date()
        value: threeMonthEarlier
    });
    reeferAdvanceTimeFrom = Ext.create('Ext.form.field.Time', {
        name: 'pretripTimeFrom',
        fieldLabel: '',
        labelWidth: 0,
        minValue: '12 AM',
        maxValue: '11:45 PM',
        increment: 15,
        value: '12:00 AM',
        width: 100,
        maxWidth: 100,
        margin: '0 0 0 10',
        editable: false
    });
    reeferAdvanceDateTo = Ext.create('Ext.form.field.Date', {
        anchor: '100%',
        labelWidth: 50,
        width: 190,
        maxWidth: 190,
        fieldLabel: 'To',
        name: 'pretripDateTo',
        format: 'm/d/Y',
        //value: (new Date()).getDate() + 1
        value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1)
    });
    reeferAdvanceTimeTo = Ext.create('Ext.form.field.Time', {
        name: 'pretripTimeTo',
        fieldLabel: '',
        labelWidth: 0,
        minValue: '12 AM',
        maxValue: '11:45 PM',
        increment: 15,
        value: '12:00 AM',
        width: 100,
        maxWidth: 100,
        margin: '0 0 0 10',
        editable: false
    });

    var reeferAdvanceDateTimeContainer = Ext.create('Ext.Panel', {
        //title: 'Messages',
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:0;border:0;background-color:transparent;',
        style: { margin: '10px 0 10px 10px' },
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
           reeferAdvanceDateFrom,
           reeferAdvanceTimeFrom,
           reeferAdvanceDateTo,
           reeferAdvanceTimeTo]
    });
    var reeferAdvanceFormFieldContainer = Ext.create('Ext.Panel', {
        //title: 'Messages',
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:0;border:0;background-color:transparent;',
        width: 320,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: false,
        defaultType: 'textfield',
        items: [reeferAdvanceDateTimeContainer]
    });

    var reeferPrtForm = Ext.create('Ext.form.Panel', {
        title: 'Events',
        labelWidth: 50, // label settings here cascade unless overridden
        //url: './historynew/historyservices_Reefer.aspx?st=gethistoryrecords&reeferScreenName=reefer',
        frame: true,
        bodyStyle: 'padding:5px 5px 0;',
        width: 320,
        layout: 'anchor', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: false,
        defaultType: 'textfield',
        items: [btnReeferAdvanceSearch, btnReeferPrtSubmit, btnReeferImpactSubmit, reeferAdvanceFormFieldContainer, ReeferPretripGrid, ReeferImpactGrid]
        , listeners: {
           'activate': function (grid, eOpts) {
               if (pretripLoaded)
                   return;

               //reeferPrtForm.setTitle("Loading...");
               loadingMask.show();

               pretripLoaded = true;

               var ReeferAdvanceDateFrom = TwoDigits(reeferAdvanceDateFrom.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateFrom.value.getDate()) + "/" + reeferAdvanceDateFrom.value.getFullYear().toString();
               var ReeferAdvanceTimeFrom = TwoDigits(reeferAdvanceTimeFrom.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeFrom.value.getMinutes());

               var ReeferAdvanceDateTo = TwoDigits(reeferAdvanceDateTo.value.getMonth() + 1) + "/" + TwoDigits(reeferAdvanceDateTo.value.getDate()) + "/" + reeferAdvanceDateTo.value.getFullYear().toString();
               var ReeferAdvanceTimeTo = TwoDigits(reeferAdvanceTimeTo.value.getHours()) + ":" + TwoDigits(reeferAdvanceTimeTo.value.getMinutes());

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
                        historyDateFrom: ReeferAdvanceDateFrom,
                        historyTimeFrom: ReeferAdvanceTimeFrom,
                        historyDateTo: ReeferAdvanceDateTo,
                        historyTimeTo: ReeferAdvanceTimeTo,
                        limit: ReeferPretripPagesize
                    }
                });
           }
       }
    });

    //tabs.add(reeferPrtForm);
    tabs.add(reeferPrtForm);
}