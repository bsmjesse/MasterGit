<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BatteryVehicleList.aspx.cs" Inherits="SentinelFM.BatteryTrending_BatteryVehilceList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Battery Trending</title>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />
    
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/downloadify.min.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Button.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/ExportMenu.js"></script>
    
    <script type="text/javascript">        
        var vehiclegrid;
        var pagesize = 5;
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
	        'Ext.ux.exporter.Button.*'
        ]);

        Ext.onReady(function () {
            loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." 
            });
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
                        name: 'Datetime', type: 'date', dateFormat: 'c'
                    },
                    { name: 'IgnOnReading', type: 'float'}, {name: 'IgnOffReading', type: 'float'}, {name: 'FirstReading', type: 'float'}, 
                    {name: 'SecondReading', type: 'float'}, {name: 'ThirdReading', type: 'float'},  {name: 'SleepReading', type: 'float'}, {name: 'Heartbeat', type: 'float'}]
            });

            window.store1 = Ext.create('Ext.data.Store', {
                //fields: ['description', 'datetime', 'ignOn', 'ignOff', 'firstReading', 'secondReading', 'thirdReading', 'sleepReading', 'heartBeat'],
                model: 'VehicleList',
                pageSize: pagesize,
                storeId: 'store1',  
                remoteSort: true,
                proxy:
                {
                    type: 'ajax',
                    url: './BatteryServices.aspx?QueryType=GetCurrentOrLastTripBatteryByFleet&fleetId=' + FleetId,
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
                      property: 'Datetime',
                      direction: 'DESC'
                  }
                ]
            });

            window.VoltageThreshold_store = Ext.create('Ext.data.JsonStore', {
                fields: ['name', 'display'],
                autoLoad: false,
                data: generateThresholdData()
            });

            var selVoltageThreshold = new Ext.form.ComboBox({
                name: 'selVoltageThreshold',
                fieldLabel: 'Voltage Threshold', //'Voltage Threshold',
                hiddenName: 'selVoltageThreshold',
                store: VoltageThreshold_store,
                displayField: 'display',
                valueField: 'name',
                value: VoltageThreshold,
                labelWidth: 100,
                width: 250,
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
                          VoltageThreshold = records[0].data.name;
                          store1.removeAll(true);
                          store1.currentPage = 1;
                          store1.proxy.extraParams.t = VoltageThreshold;
                          store1.load({
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
            });

            var vehiclePager = new Ext.PagingToolbar(
            {
                id: 'vehiclePager',
                store: store1,
                displayInfo: true,
                displayMsg: ResVehiclePagerDisplayMsg, //'Displaying vehicles {0} - {1} of {2}',
                emptyMsg: ResVehiclePagerEmptyMsg, //"No vehicles to display",
                listeners: {
                    beforechange: function () {                        
                        store1.proxy.extraParams = { /*fleetId: FleetId,*/   t: VoltageThreshold     };                        
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
                        dataIndex: 'Datetime'
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

                    var sorters = store1.sorters;
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
                        url: './BatteryServices.aspx?GetCurrentOrLastTripBatteryByFleet&fleetId=' + FleetId + '&formattype=' + fileformat + '&operation=Export&sort=' + sortingp + '&filter=' + filtersdata + '&t=' + VoltageThreshold + '&columns=' + columnsp
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
                maxHeight: 520,
                title: '',
                store: store1,
                features: [batteryfilters],
                renderTo: 'vehiclegrid',//Ext.getBody(),                
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
                            return Ext.String.format('<a href="BatteryVoltageGraph.aspx?fleetId={0}&vehicleId={1}">{2}</a>', FleetId, Ext.String.escape(record.data['VehicleId']), value);
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
                        dataIndex: 'Datetime',
                        filterable: false,
                        sortable: false
                    },
                    {
                        text: 'Ign On Reading',
                        filterable: false,
                        align: 'right',
                        width: 90,
                        dataIndex: 'IgnOnReading',
                        filterable: true,
                        sortable: true,
						hidden: true
                    },
                    {
                        text: 'Ign Off Reading',
                        align: 'right',
                        width: 90,
                        dataIndex: 'IgnOffReading',
                        filterable: true,
                        sortable: true
                    },
                    {
                        text: 'Battery Value(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'FirstReading',
                        filterable: true,
                        sortable: true
                    },
                    {
                        text: 'Battery Value(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'SecondReading',
                        filterable: true,
                        sortable: true
                    },
                    {
                        text: 'Battery Value(v)',
                        align: 'right',
                        width: 80,
                        dataIndex: 'ThirdReading',
                        filterable: true,
                        sortable: true
                    },
                    {
                        text: 'Sleep Reading',
                        align: 'right',
                        width: 80,
                        dataIndex: 'SleepReading',
                        filterable: true,
                        sortable: true
                    },
                    {
                        text: 'Heartbeat',
                        align: 'right',
                        width: 70,
                        dataIndex: 'Heartbeat',
                        filterable: true,
                        sortable: true
                    }
                ],
                dockedItems: 
                [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            selVoltageThreshold, 
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
                        store1.load();
                    }/*,
                    'filterupdate': function () {
                        var filtersdata = vehiclegrid.filters.buildQuery(vehiclegrid.filters.getFilterData()).filter;
                        alert(filtersdata);
                    }*/
                },
                // paging bar on the bottom
                bbar: vehiclePager
            });

        });
    </script>
</head>
<body>
    <script type="text/javascript">
        var VoltageThreshold = '<%=VoltageThreshold %>';
        var FleetId = <%=FleetId%>;
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
    <div id="vehiclegrid" style="height:520px;">
    
    </div>
    <div style="height:20px;padding:10px;">
        <a href="Index.aspx?fleetId=<%=FleetId %>">Back</a>
    </div>

    <iframe id="exportframe" name="exportframe" style="display:none"></iframe>
    <form id="exportform" method="post" target="exportframe" action="../MapNew/frmExportData.aspx">
        <input type="hidden" id="exportdata" name="exportdata" value="" />
        <input type="hidden" id="filename" name="filename" value="" />
        <input type="hidden" id="formatter" name="formatter" value="" />
    </form>
</body>
</html>
