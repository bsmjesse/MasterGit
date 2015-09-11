<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleAvailabilityDashboard.aspx.cs" 
    Inherits="SentinelFM.frmVehicleAvailabilityDashboard" meta:resourcekey="PageResource1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />   

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>  
    <script type="text/javascript" src="../Scripts/highcharts/highcharts.js"></script>

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

       body {
           padding: 0;
           margin: 0;
       }
       
     </style>

   <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>    
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="../Scripts/NewMap/VehicleAvailabilityDashboard.js?v=20150723"></script>

    <script type="text/javascript">
        var JSON = JSON || {};
        // implement JSON.stringify serialization
        JSON.stringify = JSON.stringify || function (obj) {
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

        var FleetId = <% = FleetId.ToString() %>;
        var LandmarkCategoryId = <% = LandmarkCategoryId.ToString() %>;
        var NodeCode = '<%=NodeCode%>';
        var MutipleUserHierarchyAssignment = false;

        var panelOuter;
        var FleetName = '<%=FleetName%>';
        var ManagerVehiclesAvailableStore;

        $(function () {
            
            $('#divForSelectedFleetChart').highcharts({
                colors: ['#088A08', '#B40404'],
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie',
                    events: {
                        load: function () {                            
                            var series = this.series[0];
                            var piechart = this;
                            $.ajax({
                                type: 'POST',
                                url: './NewMapGeozoneLandmark.asmx/GetVehicleAvailabilityByFleetForPieChart',
                                data: JSON.stringify({ FleetId: FleetId }),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                async: true,
                                success: function (msg) {
                                    series.setData(msg.d);

                                    var total = msg.d[0].NumberOfVehicles + msg.d[1].NumberOfVehicles;
                                    
                                    var text = piechart.renderer.text(
                                          'Total: ' + total + ' vehicles',
                                          piechart.plotLeft,
                                          piechart.plotTop + 5                                              
                                      ).attr({
                                          zIndex: 5
                                      }).css({
                                          color: '#333',
                                          fontSize: '12px'
                                      }).add();
                                }                                
                            });                            
                        }
                    }
                },
                title: {
                    text: 'Vehicle Availability - Fleet: ' + FleetName,
                    style: {font: 'bold 14px "Trebuchet MS", Verdana, sans-serif'}
                },
                tooltip: {
                    pointFormat: '{point.y:.0f} vehicles'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: ' {point.percentage:.1f} %',
                            distance: -30,
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
                    }
                },
                series: [{
                    name: "Brands",
                    colorByPoint: true,
                    data: []                    
                }],
                credits: {
                    enabled: false
                }
                
            });

            $('#divForMangerChart').highcharts({
                chart: {
                    type: 'bar',
                    //height: 12 * 30 + 120,
                    events: {
                        load: function () {                            
                            var series = this.series;
                            var managerchart = this;
                            $.ajax({
                                type: 'POST',
                                url: './NewMapGeozoneLandmark.asmx/GetManagerVehiclesAvailabilityJSON',
                                data: JSON.stringify({ FleetId: FleetId }),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                async: true,
                                success: function (msg) {
                                    seriesData = eval('(' + msg.d + ')');
                                    
                                    series[0].setData(seriesData.series1);
                                    series[1].setData(seriesData.series2);
                                    series[2].setData(seriesData.series3);
                                    managerchart.setSize(600, (seriesData.series1.length + seriesData.series2.length + seriesData.series3.length) * 25 + 150);
                                    managerchart.redraw();                                    
                                }                                
                            });                            
                            
                        }
                    }
                },                
                title: {
                    text: "Vehicle Availability for '" + FleetName + "' fleet by Manager",
                    style: {font: 'bold 14px "Trebuchet MS", Verdana, sans-serif'}
                },
                xAxis: {
                    title: {
                        text: 'Managers',
                        style: { font: 'bold 14px Arial;' }
                    },
                    type: 'category'
                },
                yAxis: {
                    title: {
                        text: 'Percentage Available',
                        style: { font: 'bold 14px Arial;' }
                    },
                    min: 0, max: 100

                },
                legend: {
                    enabled: true
                },
                plotOptions: {                    
                    bar: {
                        cursor: 'pointer',
                        stacking: 'normal'
                    }
                },

                tooltip: {
                    headerFormat: '',
                    pointFormat: '<span style="font-size:11px">{point.name}:</span> <span style="color:{point.color}">{point.y:.2f}%</span><br><span style="font-size:11px">Available:{point.numAvailable}, Unavailable:{point.numUnavailable}, Total: {point.numTotal}</span><br/>'
                },

                series: [{
                    name: '>=90%', color: '#319500'
                },
                {
                    name: '>= 75% & < 90%', color: '#F99900',
                    data: []
                },
                {
                    name: '< 75%', color: '#D54679',
                    data: []
                }
                ],
                credits: {
                    enabled: false
                }

            });
        });

        //Ext.require('Ext.chart.*');
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
            
        });

        function onFleetChanged(o) {
            var _fleetId = $('#<%=cboFleet.ClientID%>').val();
            window.location.href = "frmvehicleavailabilitydashboard.aspx?FleetId=" + _fleetId + "&LandmarkCategoryId=" + LandmarkCategoryId;
        }

        function onOrganizationHierarchyNodeCodeClick() {
            var mypage = '../Widgets/OrganizationHierarchy.aspx?nodecode=' + NodeCode + '&loadVehicle=0&sl=1';
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0&rootNodecode=";
            }
            var myname = 'OrganizationHierarchy';
            var w = 740;
            var h = 440;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            hierarchyBtnReference = 'vehiclelist';
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName) {
            window.location.href = "/MapNew/frmvehicleavailabilitydashboard.aspx?FleetId=" + fleetId + "&LandmarkCategoryId=" + LandmarkCategoryId;
                
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr id="trBasedOnNormalFleet" runat="server">
                <td style="width: 669px; padding: 10px;" align="left">
                    <table id="Table3" cellspacing="0" cellpadding="0" align="left" border="0">
	                    <tr>
		                    <td><asp:label id="lblFleet" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Fleet:"></asp:label> </td>
		                    <td><asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" AutoPostBack="False" DataValueField="FleetId"
				                    DataTextField="FleetName" OnChange="onFleetChanged(this)" meta:resourcekey="cboFleetResource1"></asp:dropdownlist></td>
	                    </tr>
                    </table>
                </td>
            </tr>
            <tr id="trBasedOnHierarchyFleet" visible="false" runat="server">
                <td style="width: 669px; padding: 10px;" align="center">
                <table id="Table11" cellspacing="0" cellpadding="0" align="left" border="0">
	                <tr>
                            <td class="style1">
                                <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" 
                                    Text=" Hierarchy Node:" meta:resourcekey="lblOhTitleResource1"  /></td>
                            <td>
                                <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                    CssClass="combutton" Width="200px" 
                                    OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                    meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                            </td>
                        </tr>
                </table>
                </td>
            </tr>
        </table>
    <div>
        <div id="divForSelectedFleetChart" class="divStyle" style="width:400px;height:400px;"></div>        
        <div id="divForMangerGrid" class="divStyle"></div>
        <div id="divForMangerChart" class="divStyle"></div>
    </div>
    </form>
</body>
</html>
