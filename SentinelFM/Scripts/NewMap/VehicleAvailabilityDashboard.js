Ext.onReady(function () {

    var qsParameters = Ext.urlDecode(window.location.search.substring(1));

    Ext.define('ManagerVehiclesAvailabilityModel',
    {
        extend: 'Ext.data.Model',
        fields:
        [
            'ManagerName',
            {
                name: 'NumberOfAvailable', type: 'int'
            },
            {
                name: 'NumberOfUnavailable', type: 'int'
            },
            {
                name: 'PercAvailable', type: 'float'
            },
            {
                name: 'PercUnavailable', type: 'float'
            },
            {
                name: 'Total', type: 'int'
            }
        ]
    });

    ManagerVehiclesAvailableStore = new Ext.data.Store(
    {
        model: 'ManagerVehiclesAvailabilityModel',
        autoLoad: true,
        proxy:
        {
            type: 'ajax',
            url: './NewMapGeozoneLandmark.asmx/GetManagerVehiclesAvailability?FleetId='+qsParameters.FleetId,
            timeout: 120000,
            reader:
            {
                type: 'xml',
                root: 'Fleet',
                record: 'ManagerVehiclesAvailability',
                totalProperty: 'totalCount'
            }
        },
        listeners: {
            'load': function () {
                //loadingMask.hide();
            }
        }
            
    });

    var vehiclePager = new Ext.PagingToolbar(
    {
        id: 'vehiclePager',
        store: ManagerVehiclesAvailableStore,
        displayInfo: true,
        displayMsg: 'Displaying records {0} - {1} of {2}',
        emptyMsg: "No records to display",
        listeners: {
            //beforechange: function () {
            //    vehicleListStore.proxy.extraParams = { /*fleetId: FleetId,*/   t: VoltageThreshold };
            //    loadingMask.show();
            //},

            //change: function (thisd, params) {
            //    //loadingMask.hide();
            //    //if (!filteron && typeof (params) != 'undefined') {
            //    //    currentpage = params.currentPage;
            //    //}                        
            //}
        }
    });

    var ManagerVehiclesAvailableGrid = Ext.create('Ext.grid.Panel', {
        id: 'ManagerVehiclesAvailableGrid',
        enableColumnHide: true,
        enableSorting: false,
        closable: false,
        collapsible: false,
        resizable: false,
        width: 425,
        height: 400,
        maxWidth: window.screen.width - 5,
        maxHeight: 1000,
        title: '',
        store: ManagerVehiclesAvailableStore,
        //features: [batteryfilters],
        renderTo: 'divForMangerGrid',//Ext.getBody(),                
        viewConfig: {
            emptyText: 'No records to display',
            useMsg: false
        },
        columns: [
            {
                text: 'Manager',
                align: 'left',
                width: 150,
                //filterable: true,
                //renderer: function (value, p, record) {
                //    return Ext.String.format('<a href="BatteryVoltageGraph.aspx?fleetId={0}&vehicleId={1}">{2}</a>', fleetId, Ext.String.escape(record.data['VehicleId']), value);
                //},
                dataIndex: 'ManagerName',
                sortable: true
            },
            {
                text: 'Available (%)',
                filterable: false,
                //xtype: 'datecolumn',
                //format: userdateformat,
                align: 'right',
                width: 95,
                dataIndex: 'PercAvailable',
                filterable: false,
                sortable: true,
                renderer: function (value, p, record) {                    
                    return value.toFixed(2);                    
                }
            },
            {
                text: 'Unavailable (%)',
                filterable: false,
                align: 'right',
                width: 95,
                dataIndex: 'PercUnavailable',
                //filterable: true,
                sortable: true,
                hidden: false
                ,renderer: function (value, p, record) {                    
                    return value.toFixed(2);                    
                }
            },
            {
                text: 'Total',
                align: 'right',
                width: 60,
                dataIndex: 'Total',
                filterable: false,
                sortable: true
                //,renderer: function (value, p, record) {
                //    if (value > 0)
                //        return value.toFixed(2);
                //    else
                //        return '';
                //}
            }
        ],
        //dockedItems:
        //[
        //    {
        //        xtype: 'toolbar',
        //        dock: 'top',
        //        items: [
        //            //selVoltageThreshold, 
        //            {
        //                icon: 'preview.png',
        //                cls: 'x-btn-text-icon',
        //                text: 'Export',
        //                menu: exportMenu
        //            }
        //        ]
        //    }

        //],
        bbar: vehiclePager
    });

    //Ext.chart.theme.White = Ext.extend(Ext.chart.theme.Base, {
    //    constructor: function () {
    //        Ext.chart.theme.White.superclass.constructor.call(this, {
    //            axis: {
    //                stroke: 'rgb(8,69,148)',
    //                'stroke-width': 1
    //            },
    //            axisLabel: {
    //                fill: 'rgb(8,69,148)',
    //                font: '12px Arial',
    //                'font-family': '"Arial',
    //                spacing: 2,
    //                padding: 5,
    //                renderer: function (v) { return v; }
    //            },
    //            axisTitle: {
    //                font: 'bold 18px Arial'
    //            }
    //        });
    //    }
    //});

    //var chart = Ext.create('Ext.chart.Chart', {
    //    id: 'chartCmp',
    //    xtype: 'chart',
    //    animate: true,
    //    shadow: true,
    //    store: ManagerVehiclesAvailableStore,
    //    //renderTo: Ext.get("divForMangerChart"),
    //    width: 700,
    //    //height: 500,
    //    axes: [{
    //        type: 'Numeric',
    //        position: 'bottom',
    //        fields: ['PercAvailable'],
    //        label: {
    //            renderer: Ext.util.Format.numberRenderer('0,0')
    //        },
    //        title: 'Percentage Available',
    //        grid: true,
    //        minimum: 0
    //    }, {
    //        type: 'Category',
    //        position: 'left',
    //        fields: ['ManagerName'],
    //        title: 'Managers'
    //    }],
    //    theme: 'White',
    //    background: {
    //        gradient: {
    //            id: 'backgroundGradient',
    //            angle: 45,
    //            stops: {
    //                0: {
    //                    color: '#ffffff'
    //                },
    //                100: {
    //                    color: '#eaf1f8'
    //                }
    //            }
    //        }
    //    },
    //    series: [{
    //        type: 'bar',
    //        axis: 'bottom',
    //        highlight: true,
    //        tips: {
    //            trackMouse: true,
    //            width: 140,
    //            height: 40,
    //            renderer: function (storeItem, item) {
    //                this.setTitle('Available: ' + storeItem.get('NumAvailable') + ', Unavailable: ' + storeItem.get('NumUnavailable'));
    //            }
    //        },
    //        label: {
    //            display: 'insideEnd',
    //            field: 'data1',
    //            renderer: Ext.util.Format.numberRenderer('0'),
    //            orientation: 'horizontal',
    //            color: '#333',
    //            'text-anchor': 'middle',
    //            contrast: true
    //        },
    //        xField: 'ManagerName',
    //        yField: ['PercAvailable'],
    //        renderer: function (sprite, record, attr, index, store) {
    //            var colours = ['rgb(49, 149, 0)', 'rgb(249, 153, 0)', 'rgb(213, 70, 121)'];
    //            var color = colours[0]; //'#088A08';

    //            if (record.get('PercAvailable') < 75) {
    //                color = colours[2];
    //            } else if (record.get('PercAvailable') < 90) {
    //                color = colours[1];
    //            }

    //            return Ext.apply(attr, {
    //                fill: color
    //            });
    //        }
    //    }]//,
    //    //legend: {
    //    //    position: 'right'
    //    //}
    //});

    //var legendPanel = Ext.create('Ext.Panel', {
    //    name: 'historyTimeFrom',
    //    width: 240,
    //    border: 0,
    //    height: 600,
    //    html: '<div class="chartlegend">' +
    //            '<table border="0" cellspacing="10">' +
    //                '<tr><td class="lessthan75" width="100">&nbsp;</td><td>&lt; 75%</td></tr>' +
    //                '<tr><td class="blank" width="100" colspan="2" height="2"></td></tr>' +
    //                '<tr><td class="between75and90" width="100">&nbsp;</td><td>&gt;= 75% &amp; &lt; 90% </td></tr>' +
    //                '<tr><td class="blank" width="100" colspan="2" height="2"></td></tr>' +
    //                '<tr><td class="greaterthan90" width="100">&nbsp;</td><td>&gt;= 90%</td></tr>' +                    
    //                '</table>    ' +
    //        '</div>'
    //});

    //var chartPanel = Ext.create('widget.panel', {
    //    width: 580,
    //    height: 700,
    //    //title: "Vehicle Availability for '" + FleetName + "' fleet by Manager",
    //    header: false,
    //    //renderTo: "divForMangerChart",
    //    layout: 'fit',
    //    items: [chart]
        
    //});

    //panelOuter = Ext.create('Ext.form.Panel', {
    //    width: 830,
    //    minHeight: 400,
    //    title: "Vehicle Availability for '" + FleetName + "' fleet by Manager",
    //    renderTo: "divForMangerChart",
    //    layout: 'column',
    //    items: [chartPanel, legendPanel]        
    //});

});