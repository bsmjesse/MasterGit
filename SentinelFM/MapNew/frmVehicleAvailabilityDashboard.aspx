<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleAvailabilityDashboard.aspx.cs" 
    Inherits="SentinelFM.frmVehicleAvailabilityDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />   

    <style type="text/css">
       .divStyle {
             /*float: left;*/
             display: inline-block;
        }
       .divStyleForSpacing {
             width: 30px;
             float: left;

        }
       .lessthan75 { background-color: rgb(213, 70, 121) }
       .between75and90 { background-color: rgb(249, 153, 0) }
       .greaterthan90 { background-color: rgb(49, 149, 0) }
       .chartlegend {
           border: 0;
           margin-top:280px;
           margin-left:10px;
           border: 1px solid #cccccc;
           padding: 5px;
       }

       .chartlegend td { 
            padding: 2px;
        }
       
     </style>

    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>    
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>
    <script type="text/javascript" src="../Scripts/NewMap/VehicleAvailabilityDashboard.js?v=20150723"></script>

    <script type="text/javascript">
        var panelOuter;
        var FleetName = '<%=FleetName%>';
        var ManagerVehiclesAvailableStore;

        Ext.require('Ext.chart.*');
        Ext.require(['Ext.layout.container.Fit', 'Ext.window.MessageBox']);
        Ext.require('Ext.data.*');
        Ext.require([
            'Ext.grid.*',
            'Ext.toolbar.Paging'
        ]);


        Ext.onReady(function () {

            Ext.define('Ext.ux.AspWebAjaxProxy', {
                extend: 'Ext.data.proxy.Ajax',
                require: 'Ext.data',

                buildRequest: function (operation) {
                    var params = Ext.applyIf(operation.params || {}, this.extraParams || {}),
                                            request;
                    params = Ext.applyIf(params, this.getParams(params, operation));
                    if (operation.id && !params.id) {
                        params.id = operation.id;
                    }

                    params = Ext.JSON.encode(params);

                    request = Ext.create('Ext.data.Request', {
                        params: params,
                        action: operation.action,
                        records: operation.records,
                        operation: operation,
                        url: operation.url
                    });
                    request.url = this.buildUrl(request);
                    operation.request = request;
                    return request;
                }
            });


            //************************
            Ext.define('DashboardPieModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'OperationState', type: 'int' },
                    { name: 'OperationStateDisplayText', type: 'string' },
                    { name: 'PercentageDisplayText', type: 'string' },
                    { name: 'FleetName', type: 'string' },
                    { name: 'Percentage', type: 'int' },
                    { name: 'NumberOfVehicles', type: 'int' }
                ]
            });


            var qsParametersObject = Ext.urlDecode(window.location.search.substring(1));

            var titleForSelectedFleet = "Undefined";

            var pieStoreForSelectedFleet = new Ext.data.Store(
            {
                proxy: new Ext.ux.AspWebAjaxProxy({
                    url: './NewMapGeozoneLandmark.asmx/GetVehicleAvailabilityByFleetForPieChart',
                    actionMethods: {
                        read: 'POST'
                    },
                    reader: {
                        type: 'json',
                        model: 'DashboardPieModel',
                        root: 'd'
                    },
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8'
                    },
                    timeout: 120000,
                    extraParams: {
                        FleetId: qsParametersObject.FleetId
                    }
                })
                , listeners:
                {
                    'load': function (store, records, options) {
                        try {
                            //var firstPieModel = pieStoreForSelectedFleet.first();
                            //if (firstPieModel) {
                            //    titleForSelectedFleet = firstPieModel.data.FleetName;

                            //    panelForSelectedFleet.setTitle('Vehicle Availability - Fleet: ' + firstPieModel.data.FleetName);
                            //    panelOuter.setTitle("Vehicle Availability for '" + firstPieModel.data.FleetName + "' fleet by Manager");
                            //}
                            ManagerVehiclesAvailableStore.load();
                        }
                        catch (err) {
                        }

                    },
                    scope: this
                },
                autoLoad: true
            });

            var chartForSelectedFleet = Ext.create('Ext.chart.Chart', {
                    xtype: 'chart',
                    id: 'chartCmp',
                    animate: true,
                    store: pieStoreForSelectedFleet,
                    shadow: true,
                    legend: {
                        position: 'right'
                    },
                    insetPadding: 20,
                    theme: 'Base:gradients',
                    series: [{
                        type: 'pie',
                        colorSet: ['#088A08', '#B40404'],
                        field: 'NumberOfVehicles',
                        showInLegend: false,
                        tips: {
                            trackMouse: true,
                            width: 140,
                            height: 28,
                            renderer: function (storeItem, item) {
                                this.setTitle(storeItem.get('OperationStateDisplayText') + ': ' + storeItem.get('NumberOfVehicles') + ' vehicles');
                            }
                        },
                        highlight: {
                            segment: {
                                margin: 20
                            }
                        },
                        label: {
                            field: 'PercentageDisplayText',
                            display: 'rotate',
                            contrast: true,
                            font: '14px Arial'
                        }
                    }]
                });


            var panelForSelectedFleet = Ext.create('widget.panel', {
                width: 400,
                height: 400,
                title: 'Vehicle Availability - Fleet: ' + FleetName,
                renderTo: "divForSelectedFleetChart",
                layout: 'fit',
                items: [chartForSelectedFleet]
            });

        });


    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <%--<div id="divForAllFleetChart" class="divStyle"></div> --%>
        <%--<div class="divStyle">&nbsp;&nbsp;&nbsp;</div>  --%>
        <div id="divForSelectedFleetChart" class="divStyle"></div>        
        <div id="divForMangerGrid" class="divStyle"></div>
        <div id="divForMangerChart" class="divStyle"></div>
    </div>
    </form>
</body>
</html>
