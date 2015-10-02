<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BatteryVoltageGraph.aspx.cs" Inherits="SentinelFM.BatteryTrending_BatteryVoltageGraph" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Battery Trending</title>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />
    
    <link rel="stylesheet" type="text/css" href="../Scripts/jqchart/css/jquery.jqChart.css" />
    <link rel="stylesheet" type="text/css" href="../Scripts/jqchart/css/jquery.jqRangeSlider.css" />
    <link rel="stylesheet" type="text/css" href="../Scripts/jqchart/themes/smoothness/jquery-ui-1.8.21.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script> 
    <script src="../Scripts/jqchart/js/jquery.mousewheel.js" type="text/javascript"></script>
    <script src="../Scripts/jqchart/js/jquery.jqChart.min.js" type="text/javascript"></script>
    <script src="../Scripts/jqchart/js/jquery.jqRangeSlider.min.js" type="text/javascript"></script>
    <!--[if IE]><script lang="javascript" type="text/javascript" src="../Scripts/jqchart/js/excanvas.js?v=2014032107"></script><![endif]-->
       
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/downloadify.min.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Button.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/ExportMenu.js"></script>

    <script type="text/javascript">        
        var vehiclegrid;
        var pagesize = 10000;
        var batteryGridForm;
        var batteryGraphWindow;
        var loadingMask;

        var dataBatteryIgnOn = [];
        var dataBatteryIgnOff = [];
        var dataBatteryFirstReading = [];
        var dataBatterySecondReading = [];
        var dataBatteryThirdReading = [];
        var dataBatterySleepReading = [];
        var dataBatteryHeartbeat = [];
        var batteryDateFrom;
        var batteryDateTo;

        var graphIgnOnReading = true;
        var graphIgnOffReading = true;
        var graphFirstReading = true;
        var graphSecondReading = true;
        var graphThirdReading = true;
        var graphSleepReading = true;
        var graphHeartbeat = true;

        var listVehicleStoreLoaded = false;

        Ext.Loader.setConfig({enabled: true});
        Ext.Loader.setPath('Ext.ux', '../extjs/examples/ux');
        Ext.Loader.setPath('Ext.ux.exporter', '../sencha/Ext.ux.Exporter'); // Only the Ext.ux.exporter.* classes will be searched in ./something/exporter'
        Ext.require([
            'Ext.grid.*',
            'Ext.data.*',
            'Ext.ux.grid.FiltersFeature',
            'Ext.util.*',
            'Ext.toolbar.Paging',
            'Ext.ux.PreviewPlugin',
            'Ext.ModelManager',
            'Ext.tip.QuickTipManager',
            'Ext.ux.exporter.Exporter.*',
	        'Ext.ux.exporter.excelFormatter.*',
	        'Ext.ux.exporter.csvFormatter.*',
	        'Ext.ux.exporter.Button.*'
        ]);

        Ext.onReady(function () {
            loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." });
            window.generateMessageTypeData = function () {
                var data = [];
                data.push({
                    name: 'All',
                    display: 'All Messages'
                });

                data.push({
                    name: 'IgnOn',
                    display: 'Ign On Reading'
                });

                data.push({
                    name: 'IgnOff',
                    display: 'Ign Off Reading'
                });

                data.push({
                    name: 'First',
                    display: 'First Reading'
                });

                data.push({
                    name: 'Second',
                    display: 'Second Reading'
                });

                data.push({
                    name: 'Third',
                    display: 'Third Reading'
                });

                data.push({
                    name: 'Sleep',
                    display: 'Sleep Reading'
                });

                data.push({
                    name: 'Heartbeat',
                    display: 'Heartbeat'
                });

                return data;
            };

            Ext.define('BatteryTrending',
            {
                extend: 'Ext.data.Model',
                fields: [                    
                    'VehicleId', 'Description', 'CostCenter',
                    {
                        name: 'Datetime', type: 'date', dateFormat: 'c'
                    }, 'CostCenter',
                    {name: 'IgnOnReading', type: 'float'}, {name: 'IgnOffReading', type: 'float'}, {name: 'FirstReading', type: 'float'}, 
                    {name: 'SecondReading', type: 'float'}, {name: 'ThirdReading', type: 'float'}, {name: 'SleepReading', type: 'float'}, {name: 'Heartbeat', type: 'float'}]
            });

            window.BatteryTrendingStore = Ext.create('Ext.data.Store', {
                model: 'BatteryTrending',
                pageSize: pagesize,
                storeId: 'BatteryTrendingStore',                
                proxy:
                {
                    type: 'ajax',
                    url: './BatteryServices.aspx?QueryType=GetBatteryTrendingByVehicleId_NewTZ&fleetId=' + fleetId,
                    timeout: 120000,
                    reader:
                    {
                        type: 'xml',
                        root: 'Vehicle',
                        record: 'BatteryTrendingInfo',
                        totalProperty: 'totalCount'
                    },
                    extraParams:{
                        vehicleId: vehicleId
                    }
                },
                listeners:
                {
                    'load': function (store, records, options) {                        
                        var msgs = store.getRange();
                        dataBatteryIgnOn = [];
                        dataBatteryIgnOff = [];
                        dataBatteryFirstReading = [];
                        dataBatterySecondReading = [];
                        dataBatteryThirdReading = [];
                        dataBatterySleepReading = [];
                        dataBatteryHeartbeat = [];
                        Ext.each(msgs, function (modrecord, i) {
                            if($.isNumeric(modrecord.data.IgnOnReading) && modrecord.data.IgnOnReading > 0)
                            {
                                var p = [modrecord.data.Datetime, modrecord.data.IgnOnReading * 1];
                                dataBatteryIgnOn.push(p);
                            }
                            if($.isNumeric(modrecord.data.IgnOffReading) && modrecord.data.IgnOffReading > 0)
                            {
                                var p = [modrecord.data.Datetime, modrecord.data.IgnOffReading * 1];
                                dataBatteryIgnOff.push(p);
                            }
                            if($.isNumeric(modrecord.data.FirstReading) && modrecord.data.FirstReading > 0)
                            {
                                var p = [modrecord.data.Datetime, modrecord.data.FirstReading * 1];
                                dataBatteryFirstReading.push(p);
                            }
                            if($.isNumeric(modrecord.data.SecondReading) && modrecord.data.SecondReading > 0)
                            {
                                var p = [modrecord.data.Datetime, modrecord.data.SecondReading * 1];
                                dataBatterySecondReading.push(p);
                            }
                            if($.isNumeric(modrecord.data.ThirdReading) && modrecord.data.ThirdReading > 0)
                            {
                                var p = [modrecord.data.Datetime, modrecord.data.ThirdReading * 1];
                                dataBatteryThirdReading.push(p);
                            }
                            if($.isNumeric(modrecord.data.SleepReading) && modrecord.data.SleepReading > 0)
                            {
                                var p = [modrecord.data.Datetime, modrecord.data.SleepReading * 1];
                                dataBatterySleepReading.push(p);
                            }
                            if($.isNumeric(modrecord.data.Heartbeat) && modrecord.data.Heartbeat > 0)
                            {
                                var p = [modrecord.data.Datetime, modrecord.data.Heartbeat * 1];
                                dataBatteryHeartbeat.push(p);
                            }
                            
                            //a();
                        });

                        if(msgs.length > 0)
                            drawBatteryGraph();
                        else
                        {
                            var series = $('#divBatteryGraph').jqChart('option', 'series');  
                            if(series && series.length > 0)
                            {
                                //series = [];
                                series.splice(0, series.length);
                                $('#divBatteryGraph').jqChart('update');
                            }

                        }
                        loadingMask.hide();
                    }
                }
            });

            window.MessageType_store = Ext.create('Ext.data.JsonStore', {
                fields: ['name', 'display'],
                autoLoad: false,
                data: generateMessageTypeData()
            });

            Ext.define('VehicleListModel',
            {
                extend: 'Ext.data.Model',
                fields: [
                    'VehicleId',
                    'Description'                    
                ]
            }
            );

            var listVehicleStore = Ext.create('Ext.data.Store',
            {
                model: 'VehicleListModel',
                autoLoad: false,
                storeId: 'listVehicleStore',
                listeners:
                {
                    'load': function (xstore, records, options) {

                        var u = Ext.create('VehicleListModel', {
                            VehicleId: '-1',
                            Description: 'Select a Vehicle'
                        });
                        xstore.insert(0, u);
                        
                        //if (historyIniVehicleId != '') {
                        //    historyVehicles.setValue(historyIniVehicleId);
                        //    historyIniVehicleId = '';
                        //}
                        //else
                        //    historyVehicles.setValue('-1');

                        listVehicleStoreLoaded = true;
                    }
                },
                proxy:
                    {
                        // load using HTTP
                        type: 'ajax',
                        url: '../historynew/historyservices.aspx',
                        timeout: 120000,
                        reader:
                        {
                            type: 'xml',
                            root: 'Fleet',
                            record: 'VehiclesInformation'
                        }
                    }
            }
            );

            var listVehicles = Ext.create('Ext.form.ComboBox',
            {
                name: 'listVehicles',
                store: 'listVehicleStore',
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
                editable: true,
                enableKeyEvents: true,
                listeners:
                {
                    scope: this,
                    'keyup': function () {
                        if(!listVehicleStoreLoaded)
                        {
                            
                            listVehicleStore.load(
                            {
                                params:
                                {
                                    fleetId: fleetId
                                }
                            });
                            
                        }
                    }
                }
            }
            );

            var selMessageType = new Ext.form.ComboBox({
                name: 'selMessageType',
                fieldLabel: '', //'Voltage Threshold',
                hiddenName: 'selMessageType',
                store: MessageType_store,
                displayField: 'display',
                valueField: 'name',
                value: 'All',
                labelWidth: 0,
                width: 120,
                typeAhead: true,
                queryMode: 'local',
                triggerAction: 'all',
                emptyText: 'Choose Threshold...',
                selectOnFocus: true,
                editable: false,
                listeners:
                {
                    scope: this,
                    'select': function (combo, records) {  
                        return;                        
                    }
                }
            });

            batteryDateFrom = Ext.create('Ext.form.field.Date', {
                anchor: '100%',
                labelWidth: 30,
                maxWidth: 150,
                height: 25,
                style: { margin: '0 0 10px 10px' },
                fieldLabel: 'From',
                id: 'batteryDateFrom',
                name: 'batteryDateFrom',
                format: userDate,
                //value: new Date()
                value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() - 10)
            });
            
            batteryDateTo = Ext.create('Ext.form.field.Date', {
                anchor: '100%',
                labelWidth: 30,
                maxWidth: 150,
                height: 25,
                style: { margin: '0 0 10px 10px' },
                fieldLabel: 'To',
                id: 'batteryDateTo',
                name: 'batteryDateTo',
                format: userDate,
                //value: (new Date()).getDate() + 1
                value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1)
            });
            
            var batteryDateTimeContainer = Ext.create('Ext.Panel', {
                //title: 'Messages',
                labelWidth: 0,
                border: 0,
                frame: false,
                bodyStyle: 'padding:0;border:0;background-color:transparent;',
                width: 320,
                height: 25,
                layout: 'column', // arrange fieldsets side by side
                defaults: {
                    width: 240,
                    labelWidth: 90
                },
                //margin: '10px 0',
                header: false,
                defaultType: 'textfield',
                items: [
                    batteryDateFrom,
                    batteryDateTo
                ]
            });

            var btnSubmit = Ext.create('Ext.Button', {
                text: 'Update',
                cls: 'cmbfonts',
                //margin: '10 auto',
                style: { margin: '0 0 10px 10px' },
                width: 100,
                handler: function () {
                    try {
                        
                        loadingMask.show();
                        BatteryTrendingStore.load(
                            {
                                params: {
                                    dateTimeFrom: Ext.Date.format(batteryDateFrom.getValue(), userDate),
                                    dateTimeTo: Ext.Date.format(batteryDateTo.getValue(), userDate),
                                    vehicleId: listVehicles.getValue()
                                }
                            });
                    }
                    catch (err) {
                    }
                }
            });

            var vehiclePager = new Ext.PagingToolbar(
            {
                id: 'vehiclePager',
                store: BatteryTrendingStore,
                displayInfo: true,
                displayMsg: ResVehiclePagerDisplayMsg, //'Displaying vehicles {0} - {1} of {2}',
                emptyMsg: ResVehiclePagerEmptyMsg, //"No vehicles to display",
                listeners: {
                    beforechange: function () {                        
                        BatteryTrendingStore.proxy.extraParams = { /*fleetId: FleetId,   t: VoltageThreshold*/     };
                        //loadingMask.show();
                    },

                    change: function (thisd, params) {
                        //loadingMask.hide();
                        //if (!filteron && typeof (params) != 'undefined') {
                        //    currentpage = params.currentPage;
                        //}                        
                    }
                }
            });

            var batteryfilters =
            {
                ftype: 'filters',
                local: true,   // defaults to false (remote filtering)
                filters: [
                    {
                        type: 'string',
                        dataIndex: 'description'
                    },
                    {
                        type: 'string',
                        dataIndex: 'datetime'
                    }
                ]
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
                catch (err) {
                    alert(err);
                }
            }
        }

            var exportMenu = Ext.create('Ext.menu.Menu');
            exportMenu.add(exportToCsvButton, exportToExcel2003Button, exportToExcel2007Button);

            
            vehiclegrid = Ext.create('Ext.grid.Panel', {
                id: 'vehiclegrid',
                enableColumnHide: true,
                enableSorting: false,
                closable: false,
                collapsible: false,
                resizable: false,
                //width: 525,
                //height: 300,
                maxWidth: window.screen.width - 5,
                maxHeight: 270,
                title: '',
                store: BatteryTrendingStore,
                features: [batteryfilters],                
                viewConfig: {
                    emptyText: 'No records to display',
                    useMsg: false                    
                },
                columns: [
                    {
                        id: 'vgCostCenter',
                        text: 'Cost Center',
                        align: 'left',
                        width: 130,
                        filterable: false,
                        dataIndex: 'CostCenter',
                        sortable: false
                    },
                    {
                        id: 'vgDatetime',
                        text: 'Date/Time',
                        filterable: false,
                        xtype: 'datecolumn',
                        format: userdateformat,
                        align: 'left',
                        width: 135,                        
                        dataIndex: 'Datetime',
                        filterable: false,
                        sortable: false
                    },
                    {
                        id: 'vgIgnOnReading',
                        text: 'Ign On',
                        filterable: false,
                        align: 'right',
                        width: 70,
                        dataIndex: 'IgnOnReading',
                        filterable: true,
                        sortable: false,
                        hidden: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        id: 'vgIgnOffReading',
                        text: 'Ign Off',
                        align: 'right',
                        width: 70,
                        dataIndex: 'IgnOffReading',
                        filterable: true,
                        sortable: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        id: 'vgFirstReading',
                        text: 'Battery Value(v)',
                        align: 'right',
                        width: 100,
                        dataIndex: 'FirstReading',
                        filterable: true,
                        sortable: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        id: 'vgSecondReading',
                        text: 'Battery Value(v)',
                        align: 'right',
                        width: 100,
                        dataIndex: 'SecondReading',
                        filterable: true,
                        sortable: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        id: 'vgThirdReading',
                        text: 'Battery Value(v)',
                        align: 'right',
                        width: 100,
                        dataIndex: 'ThirdReading',
                        filterable: true,
                        sortable: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        id: 'vgSleepReading',
                        text: 'Sleep Reading',
                        align: 'right',
                        width: 80,
                        dataIndex: 'SleepReading',
                        filterable: true,
                        sortable: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        id: 'vgHeartbeat',
                        text: 'Heartbeat',
                        align: 'right',
                        width: 70,
                        dataIndex: 'Heartbeat',
                        filterable: true,
                        sortable: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    }
                ],
                dockedItems: 
                [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            { icon: 'preview.png',
                                cls: 'x-btn-text-icon',
                                text: 'Export',
                                menu: exportMenu
                            }
                        ]
                    }
                   
                ],
                listeners:
                {
                    'afterrender': function () {
                        BatteryTrendingStore.load(
                            {
                                params: {
                                    dateTimeFrom: Ext.Date.format(batteryDateFrom.getValue(), userDate),
                                    dateTimeTo: Ext.Date.format(batteryDateTo.getValue(), userDate)
                                }
                            });

                        var u = Ext.create('VehicleListModel', {
                            VehicleId: vehicleId,                    
                            Description: VehicleDescription
                        });
                        listVehicles.getStore().insert(0, u);
                        listVehicles.setValue(vehicleId);
                    },
                    'columnhide': function(a, b, c) {
                        redrawGraph(a, b);
                    },
                    'columnshow': function(a, b, c) {
                        redrawGraph(a, b);
                    }
                },
                // paging bar on the bottom
                bbar: vehiclePager
            });

            batteryGraphWindow = Ext.create('Ext.panel.Panel', {
                title: '',
                labelWidth: 50, // label settings here cascade unless overridden
                frame: true,
                //bodyStyle: 'padding:5px 5px 0;margin:0 0 10px 0;',
                margin: '0 10 10 10',
                hidden: false,
                height: 180,
                width: 830,
                html: '<div id="divBatteryGraph"></div>'
            });

            var batteryForm = Ext.create('Ext.Panel', {
                id: 'batteryForm',
                frame: false,
                border: 0,
                //title: ResHistory, //'History',
                header: false,
                bodyPadding: 0,
                margin: '5px',
                width: 800,
                closable: true,
                autoHeight: true,
                autoScroll: true,
                layout: 'column',    // Specifies that the items will now be arranged in columns

                fieldDefaults: {
                    labelAlign: 'left',
                    msgTarget: 'side'
                },
                items: [listVehicles, /*selMessageType,*/ batteryDateTimeContainer, btnSubmit]
            });

            batteryGridForm = Ext.create('Ext.Panel', {
                id: 'batteryGridForm',
                frame: false,
                border: 0,
                title: 'Battery Trending',
                header: false,
                bodyPadding: 0,
                margin: '5px',
                //width: 750,
                maxWidth: window.screen.width - 5,
                maxHeight: 510,//window.screen.height,
                closable: true,
                autoHeight: true,
                autoScroll: true,
                layout: 'anchor',    // Specifies that the items will now be arranged in columns
                renderTo: 'divGridForm',//Ext.getBody(),
                fieldDefaults: {
                    labelAlign: 'left',
                    msgTarget: 'side'
                },
                items: [batteryForm, batteryGraphWindow, vehiclegrid]
            });

        });

        function drawBatteryGraph() {
            
            $(document).ready(function () {
                var background = {
                    type: 'linearGradient',
                    x0: 0,
                    y0: 0,
                    x1: 0,
                    y1: 1,
                    colorStops: [{ offset: 0, color: '#d2e6c9' },
                       { offset: 1, color: 'white'}]
                };

                $('#divBatteryGraph').jqChart({
                    //width: window.screen.width - 70,
                    height: 180,
                    //title: { text: 'Battery Graph', font: '12px' },
                    axes: [
                              {
                                  type: 'dateTime',
                                  location: 'bottom',
                                  zoomEnabled: true,
                                  labels: { visible: false }
                              },
                              {
                                  name: 'y1',
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
                    series: []
                });

                var series = $('#divBatteryGraph').jqChart('option', 'series');
                
                if(graphIgnOnReading && dataBatteryIgnOn.length > 0)
                {
                    var newSeries = {
                        type: 'line',
                        title: 'Ign On Reading',
                        data: dataBatteryIgnOn
                    };
                    series.push(newSeries);
                }

                if(graphIgnOffReading && dataBatteryIgnOff.length > 0)
                {
                    var newSeries = {
                        type: 'line',
                        title: 'Ign Off Reading',
                        data: dataBatteryIgnOff
                    };
                    series.push(newSeries);
                }

                if(graphFirstReading && dataBatteryFirstReading.length > 0)
                {
                    var newSeries = {
                        type: 'line',
                        title: 'First Reading',
                        data: dataBatteryFirstReading
                    };
                    series.push(newSeries);
                }

                if(graphSecondReading && dataBatterySecondReading.length > 0)
                {
                    var newSeries = {
                        type: 'line',
                        title: 'Second Reading',
                        data: dataBatterySecondReading
                    };
                    series.push(newSeries);
                }

                if(graphThirdReading && dataBatteryThirdReading.length > 0)
                {
                    var newSeries = {
                        type: 'line',
                        title: 'Third Reading',
                        data: dataBatteryThirdReading
                    };
                    series.push(newSeries);
                }

                if(graphSleepReading && dataBatterySleepReading.length > 0)
                {
                    var newSeries = {
                        type: 'line',
                        title: 'Sleep Reading',
                        data: dataBatterySleepReading
                    };
                    series.push(newSeries);
                }

                if(graphHeartbeat && dataBatteryHeartbeat.length > 0)
                {
                    var newSeries = {
                        type: 'line',
                        title: 'Heartbeat',
                        data: dataBatteryHeartbeat
                    };
                    series.push(newSeries);
                }

                $('#divBatteryGraph').jqChart('update');

            });
        }

        function redrawGraph(a, b){
            
            if($.inArray(b.id, ["vgIgnOnReading", "vgIgnOffReading", "vgFirstReading", "vgSecondReading", "vgThirdReading", "vgSleepReading", "vgHeartbeat"]) >= 0)
            {
                Ext.each(vehiclegrid.columns, function (col, index) {  
                    var series = $('#divBatteryGraph').jqChart('option', 'series');

                    if(col.id == "vgIgnOnReading")
                        graphIgnOnReading = !col.hidden;
                    else if(col.id == "vgIgnOffReading")
                        graphIgnOffReading = !col.hidden;
                    else if(col.id == "vgFirstReading")
                        graphFirstReading = !col.hidden;
                    else if(col.id == "vgSecondReading")
                        graphSecondReading = !col.hidden;
                    else if(col.id == "vgThirdReading")
                        graphThirdReading = !col.hidden;
                    else if(col.id == "vgSleepReading")
                        graphSleepReading = !col.hidden;
                    else if(col.id == "vgHeartbeat")
                        graphHeartbeat = !col.hidden;                    
                });

                drawBatteryGraph();
            }
            
        }
    </script>

</head>
<body>
    <script type="text/javascript">
        var fleetId = <%=FleetId%>;
        var vehicleId = <%=VehicleId%>;

        var ResVehiclePagerDisplayMsg = "Displaying records {0} - {1} of {2}";
        var ResVehiclePagerEmptyMsg = "No records to display";

        var userDate ='<%=sn.User.DateFormat %>';
        var userTime ='<%=sn.User.TimeFormat %>';
        var VehicleDescription = "<%=VehicleDescription%>";
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
    </script>
    
    <div id="divGridForm" style="height: 510px;"></div>
    <div style="height:20px;padding:10px;">
        <a href="javascript:void(0)" onclick="window.history.back();">Back</a>
    </div>
    <iframe id="exportframe" name="exportframe" style="display:none"></iframe>
    <form id="exportform" method="post" target="exportframe" action="../MapNew/frmExportData.aspx">
        <input type="hidden" id="exportdata" name="exportdata" value="" />
        <input type="hidden" id="filename" name="filename" value="" />
        <input type="hidden" id="formatter" name="formatter" value="" />
    </form>
    
</body>
</html>
