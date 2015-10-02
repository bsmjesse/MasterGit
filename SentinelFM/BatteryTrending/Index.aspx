<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="SentinelFM.BatteryTrending_Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Battery Trending</title>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>
    <script type="text/javascript"> 
        var vehiclegrid;
        var pagesize = 100;
        var sortingParam;
        var loadingMask;
        var firstLoad = false;

        var JSON = JSON || {};
        // implement JSON.stringify serialization
        JSON.stringify = JSON.stringify || function(obj) {
            var t = typeof (obj);
            if (t != "object" || obj === null) {
                // simple data type
                if (t == "string")
                    obj = '"' + obj + '"';
                return String(obj);
            } else {
                // recurse array or object
                var n, v, json = [], arr = (obj && obj.constructor == Array);
                for (n in obj) {
                    v = obj[n];
                    t = typeof (v);
                    if (t == "string")
                        v = '"' + v + '"';
                    else if (t == "object" && v !== null)
                        v = JSON.stringify(v);
                    json.push((arr ? "" : '"' + n + '":') + String(v));
                }
                return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
            }
        };

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
	        'Ext.ux.exporter.Button.*',
            'Ext.chart.*',
            'Ext.layout.container.Fit'
        ]);

        Ext.onReady(function () {

            Ext.define('BatterySummaryModel',
            {
                extend: 'Ext.data.Model',
                fields: 
                [                
                    'Category',
                    {
                        name: 'Total', type: 'int'
                    }
                ]
            });

            window.store1 = Ext.create('Ext.data.Store', {
                model: 'BatterySummaryModel',
                autoLoad: true,
                proxy:
                {
                    type: 'ajax',
                    url: 'BatteryServices.aspx?QueryType=GetBatterySummaryByFleetId&fleetId='+fleetId,
                    timeout: 120000,
                    reader:
                    {
                        type: 'xml',
                        root: 'FleetBattery',
                        record: 'BatterySummaryInfo',
                        totalProperty: 'totalCount'
                    }
                },
                listeners: {
                    'load': function () {
                        //loadingMask.hide();
                    }
                }
            });
        
            var donut = false,
                panel1 = Ext.create('widget.panel', {
                    width: 800,
                    height: 180,
                    title: '',
                    //renderTo: 'divBatteryPie',//Ext.getBody(),
                    layout: 'fit',
                    margin: '0 0 0 0',
                    items: {
                        xtype: 'chart',
                        id: 'chartCmp',
                        animate: true,
                        store: store1,
                        shadow: false,
                        legend: {
                            position: 'right'
                        },
                        insetPadding: 20,
                        theme: 'Base:gradients',
                        series: [{
                            type: 'pie',
                            getLegendColor: function (index) {
                                return ["#ff0000", "#ffa500", "#008000"][index % 3];
                            },
                            renderer: function (sprite, record, attr, index, store) {
                                return Ext.apply(attr, {
                                    fill: ["#ff0000", "#ffa500", "#008000"][index % 3]
                                });
                            },
                            style: {
                                cursor: 'pointer'                                
                            },
                            title: ['< 11 v', '11 - 12.5 v', '>= 12.5 v'],
                            field: 'Total',
                            showInLegend: true,
                            donut: donut,
                            //tips: {
                            //    trackMouse: true,
                            //    width: 140,
                            //    height: 28,
                            //    renderer: function (storeItem, item) {
                            //        //calculate percentage.
                            //        var total = 0;
                            //        store1.each(function (rec) {
                            //            total += rec.get('Total');
                            //        });
                            //        this.setTitle(storeItem.get('name') + ': ' + Math.round(storeItem.get('Total') / total * 100) + '%');
                            //    }
                            //},
                            //highlight: {
                            //    segment: {
                            //        margin: 5
                            //    }
                            //},
                            label: {
                                field: 'Category',
                                display: 'middle',
                                contrast: true,
                                font: '18px Arial'
                                ,renderer: function (itemName) {
                                    //calculate percentage.
                                    //this.setTitle(storeItem.get('name') + ': ' + Math.round(storeItem.get('Total') / total * 100) + '%');
                                    var total = 0;
                                    var v = 0;
                                    store1.each(function (rec) {
                                        var name = rec.get('Category');
                                        if (name == itemName) {
                                            v = rec.get('Total');
                                        }
                                        total += rec.get('Total');
                                    });
                                    return Math.round(v / total * 100).toFixed(0) + '%';
                                }
                            },
                            listeners: {
                                itemmousedown: function (obj) {                                    
                                    //location.href = "BatteryVehicleList.aspx?fleetId=" + fleetId + "&t=" + obj.storeItem.data['Category'];
                                    VoltageThreshold = obj.storeItem.data['Category'];
                                    vehicleListStore.removeAll(true);
                                    vehicleListStore.currentPage = 1;
                                    vehicleListStore.proxy.extraParams.t = VoltageThreshold;
                                    loadingMask.show();
                                    vehicleListStore.load({
                                        params:
                                        {
                                            start: 0,
                                            limit: pagesize
                                            //fleetId: FleetId,
                                            //t: VoltageThreshold                           
                                        }
                                    });
                                }
                            }
                        }]
                    }
                });

            loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." 
            });
            loadingMask.show();
            window.generateThresholdData = function () {
                var data = [];
                data.push({
                    name: 'red',
                    display: '< 11'
                });

                data.push({
                    name: 'orange',
                    display: '11 - 12.5'
                });

                data.push({
                    name: 'green',
                    display: '>= 12.5'
                });

                return data;
            };

            Ext.define('VehicleList',
            {
                extend: 'Ext.data.Model',
                fields: [                    
                    'VehicleId', 'Description', 
                    {
                        name: 'LastUpdateDatetime', type: 'date', dateFormat: 'c'
                    },
                    { name: 'B1', type: 'float'}, {name: 'B2', type: 'float'}, {name: 'B3', type: 'float'}, 
                    {name: 'B4', type: 'float'}, {name: 'B5', type: 'float'},  {name: 'B6', type: 'float'}, {name: 'B7', type: 'float'}]
            });

            window.vehicleListStore = Ext.create('Ext.data.Store', {
                //fields: ['description', 'datetime', 'ignOn', 'ignOff', 'firstReading', 'secondReading', 'thirdReading', 'sleepReading', 'heartBeat'],
                model: 'VehicleList',
                pageSize: pagesize,
                storeId: 'vehicleListStore',  
                remoteSort: true,
                autoLoad: true,
                proxy:
                {
                    type: 'ajax',
                    url: './BatteryServices.aspx?QueryType=GetLastKnownBatteryByFleet_NewTZ&fleetId=' + fleetId,
                    timeout: 120000,
                    reader:
                    {
                        type: 'xml',
                        root: 'Fleet',
                        record: 'VehiclesBatteryInfo',
                        totalProperty: 'totalCount'
                    },
                    extraParams:{
                        t: VoltageThreshold
                    }
                },
                sorters: [
                  {
                      property: 'LastUpdateDatetime',
                      direction: 'DESC'
                  }
                ],
                listeners: {
                    'load': function () {
                        loadingMask.hide();
                    },
                    'beforeload': function() {
                        loadingMask.show();
                    }
                }
            });

            var vehiclePager = new Ext.PagingToolbar(
            {
                id: 'vehiclePager',
                store: vehicleListStore,
                displayInfo: true,
                displayMsg: ResVehiclePagerDisplayMsg, //'Displaying vehicles {0} - {1} of {2}',
                emptyMsg: ResVehiclePagerEmptyMsg, //"No vehicles to display",
                listeners: {
                    beforechange: function () {                        
                        vehicleListStore.proxy.extraParams = { /*fleetId: FleetId,*/   t: VoltageThreshold     };
                        loadingMask.show();
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
                local: false,   // defaults to false (remote filtering)
                encode: true,
                filters: [
                    {
                        type: 'string',
                        dataIndex: 'Description'
                    },
                    {
                        type: 'string',
                        dataIndex: 'LastUpdateDatetime'
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
                   exportHandler('csv');               
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
                    exportHandler('excel2003');
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
                    exportHandler('excel2007');
                }
            }

            function exportHandler(fileformat)
            {
                try {
                    var filtersdata = vehiclegrid.filters.buildQuery(vehiclegrid.filters.getFilterData()).filter;
                    if(filtersdata == undefined) 
                        filtersdata = "";
                    else
                        filtersdata = encodeURIComponent(filtersdata);

                    var sorters = vehicleListStore.sorters;
                    var sortingp = "";                    
                    if (sorters != undefined && sorters.items != undefined && sorters.items.length > 0 && sorters.items[0].direction != undefined && sorters.items[0].property != undefined) {
                        sortingp = encodeURIComponent(JSON.stringify(sorters.items));                        
                    }

                    var columnsp = "";
                    Ext.each(vehiclegrid.columns, function (col, index) {
                        if(!col.hidden)
                            columnsp += col.text + ':' + col.dataIndex + ',';
                        
                    });
                    columnsp = columnsp.substring(0, columnsp.length - 1);
                    
                    var form = Ext.create('Ext.form.Panel', {
                        xtype: 'form',
                        itemId: 'uploadForm',
                        hidden: true,
                        standardSubmit: true,
                        url: './BatteryServices.aspx?GetLastKnownBatteryByFleet_NewTZ&fleetId=' + fleetId + '&formattype=' + fileformat + '&operation=Export&sort=' + sortingp + '&filter=' + filtersdata + '&t=' + VoltageThreshold + '&columns=' + columnsp
                    });               
                    form.getForm().submit({ target: 'exportframe' });
                }
                catch (err) {
                    alert(err);
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
                maxHeight: 300,
                title: '',
                store: vehicleListStore,
                features: [batteryfilters],
                //renderTo: 'vehiclegrid',//Ext.getBody(),                
                viewConfig: {
                    emptyText: 'No vehicles to display',
                    useMsg: false                    
                },
                columns: [
                    {
                        text: 'Vehicle',
                        align: 'left',
                        width: 150,
                        filterable: true,
                        renderer: function (value, p, record) {
                            return Ext.String.format('<a href="BatteryVoltageGraph.aspx?fleetId={0}&vehicleId={1}">{2}</a>', fleetId, Ext.String.escape(record.data['VehicleId']), value);
                        },
                        dataIndex: 'Description',
                        sortable: true
                    },
                    {
                        text: 'Date/Time',
                        filterable: false,
                        xtype: 'datecolumn',
                        format: userdateformat,
                        align: 'left',
                        width: 135,                        
                        dataIndex: 'LastUpdateDatetime',
                        filterable: false,
                        sortable: false
                    },
                    {
                        text: 'B1(v)',
                        filterable: false,
                        align: 'right',
                        width: 80,
                        dataIndex: 'B1',
                        filterable: true,
                        sortable: true,
                        hidden: false,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        text: 'B2(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'B2',
                        filterable: true,
                        sortable: true,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        text: 'B3(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'B3',
                        filterable: true,
                        sortable: true,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        text: 'B4(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'B4',
                        filterable: true,
                        sortable: true,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        text: 'B5(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'B5',
                        filterable: true,
                        sortable: true,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        text: 'B6(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'B6',
                        filterable: true,
                        sortable: true,
                        renderer: function (value, p, record) {
                            if(value>0)
                                return value.toFixed(2);
                            else
                                return '';
                        }
                    },
                    {
                        text: 'B7(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'B7',
                        filterable: true,
                        sortable: true,
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
                            //selVoltageThreshold, 
                            { icon: 'preview.png',
                                cls: 'x-btn-text-icon',
                                text: 'Export',
                                menu: exportMenu
                            }
                        ]
                    }
                   
                ],                
                bbar: vehiclePager
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
                maxHeight: 480,//window.screen.height,
                closable: true,
                autoHeight: true,
                autoScroll: true,
                layout: 'anchor',    // Specifies that the items will now be arranged in columns
                renderTo: 'divBattery',//Ext.getBody(),
                fieldDefaults: {
                    labelAlign: 'left',
                    msgTarget: 'side'
                },
                items: [panel1, vehiclegrid]
            });
        });

    </script>
</head>
<body>
    <script type="text/javascript">
        var VoltageThreshold = '';
        var fleetId = <%=FleetId%>;
        var ResVehiclePagerDisplayMsg = "Displaying vehicles {0} - {1} of {2}";
        var ResVehiclePagerEmptyMsg = "No vehicles to display";

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
    </script>
    <h1 style="margin:10px;">Battery distribution for Selected fleet (<%=FleetName %>)</h1>
    <form id="form1" runat="server">
    <div id="divBattery" style="height: 480px;">
    
    </div>
    </form>
    <div style="height:20px;padding:10px;">
        <%--Please select region to see vehicles.--%>
    </div>
    <iframe id="exportframe" name="exportframe" style="display:none"></iframe>
</body>
</html>
