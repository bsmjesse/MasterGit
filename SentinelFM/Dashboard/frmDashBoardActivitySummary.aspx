<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeFile="frmDashBoardActivitySummary.aspx.cs"
    Inherits="SentinelFM.Dashboard_frmActivitySummary" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        body
        {
            font-family: Tahoma;
            font-size: 9pt;
        }
        .d5B_c
        {
            height: 150px;
            width: 380px;
            overflow-y: scroll;
        }
        .d5B_cc
        {
            border: 1px solid black;
        }
        .d5B_sb
        {
            width: 250px;
            overflow: visible;
        }
        .d5B_r
        {
        }
        .d5B_cap
        {
            color: #000000;
            font-weight: bold;
            width: 90px;
            text-align: right;
        }
        .d5B_val
        {
            border: 1px solid black;
            border-bottom: 1px solid silver;
            border-right: 1px solid silver;
            background-image: url(a/i/segment.png);
            background-repeat: repeat-x;
            color: #ffffff;
            font-weight: bold;
            height: 16px;
            overflow: hidden;
            width: 250px;
        }
        .d5B_cel
        {
            overflow: hidden;
            color: #000000;
            background-image: url(a/i/bar16_15.png);
            background-repeat: repeat-x;
            text-align: right;
            text-indent: 5px;
            font-size: 6pt;
            font-weight: bold;
            vertical-align: text-bottom;
            height: 16px;
            padding-top: 3px;
            cursor: pointer;
        }
        .d5B_hi
        {
            overflow: hidden;
            position: relative;
            left: -1px;
            top: -11px;
            color: #ffffff;
        }
        .d5B_h
        {
            width: 100%;
            height: 24px;
            overflow: hidden;
            text-indent: 5px;
            font-weight: bold;
        }
        .d5B_hcl, .d5B_hcr
        {
            width: 100%;
            color: #000000;
            background-color: #e3e3e3;
            background-image: url(a/i/bar_24_15.png);
            background-repeat: repeat-x;
        }
        .d5B_hcr
        {
            text-align: right;
            width: 20px;
        }
    </style>

    <script type="text/javascript">

        setTimeout('location.reload(true)',480000);

        var d5Bpre = "d5B_";
        var barWidth = 250;
        
        

        function createStackedBar(index, arr, pid) {
            var scale = parseFloat(arr[1]);
            var colors = arr[2];
            var values = arr[3];
            for (var caption in values) { }
            var d = document.createElement("div");
            d.className = "d5B_sb";
            var t = document.createElement("table");
            t.setAttribute("cellpadding", "1");
            t.setAttribute("cellspacing", "1");
            var tdc = document.createElement("td");
            var dcap = document.createElement("div");
            dcap.id = pid + "_c" + index;
            dcap.className = "d5B_cap";
            dcap.innerHTML = caption;
            tdc.appendChild(dcap);
            var tdv = document.createElement("td");
            var dval = document.createElement("div");
            dval.id = pid + "_v" + index;
            dval.className = "d5B_val";
            dval.style.width = barWidth + "px";
            var xt = document.createElement("table");
            xt.setAttribute("cellpadding", "0");
            xt.setAttribute("cellspacing", "0");
            var xtr = document.createElement("tr");
            var a = 0;
            for (var x in values) {
                var varr = values[x];
                for (var a = 0; a < varr.length; a++) {
                    var w = ((barWidth / 100) * parseFloat(varr[a][1]));
                    w *= scale;
                    var xtdv = document.createElement("td");
                    var di = document.createElement("div");
                    di.className = "d5B_cel";
                    di.id = pid + "_v" + index + "_i" + a;
                    di.style.width = w + "px";
                    xtdv.style.backgroundColor = colors[a];
                    di.innerHTML = varr[a][0];
                    di.title = varr[a][1];
                    var di2 = document.createElement("div");
                    di2.className = "d5B_hi";
                    di2.id = pid + "_v" + index + "_h" + a;
                    di2.style.width = w + "px";
                    di2.innerHTML = varr[a][0];
                    di.appendChild(di2);
                    xtdv.appendChild(di);
                    xtr.appendChild(xtdv);
                }
            }
            xt.appendChild(xtr);
            dval.appendChild(xt);
            tdv.appendChild(dval);
            var tr = document.createElement("tr");
            tr.appendChild(tdc);
            tr.appendChild(tdv);
            t.appendChild(tr);
            d.appendChild(t);
            return d;
        };

        function loadBars() {
            try{
            var graphCount = d5B_gda.length;
            for (var a = 0; a < graphCount; a++) {
                var p = document.getElementById("d5B_" + a);
                p.appendChild(createStackedBar(a, d5B_gda[a], p.id));
            }
            }
            catch(e) {}
        };

        function d5B_sel(o) {
            var sid = o.id.split('_');
            var index = sid[sid.length - 1];
            d5B_gda[index][1] = parseFloat(o[o.selectedIndex].value);
            var p = document.getElementById("d5B_" + index);
            p.innerHTML = "";
            p.appendChild(createStackedBar(index, d5B_gda[index], p.id));
        }

    </script>

    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
</head>
<body leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0" onload="loadBars();">
    <form id="form1" runat="server">
    <div>
        <fieldset style="margin: 0px 0px 5px 0px; padding: 0px 0px 0px 0px">
            <table>
                <tr>
                    <td>
                        <asp:Label CssClass="formtext" ID="lblLastUpdatedCaption" runat="server" Text="Updated:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label CssClass="formtext" ID="lblLastUpdated" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="optView" runat="server" CssClass="formtext" RepeatDirection="Horizontal"
                            OnSelectedIndexChanged="optView_SelectedIndexChanged" AutoPostBack="True">
                            <asp:ListItem Value="0" Selected="True">Grid</asp:ListItem>
                            <asp:ListItem Value="1">Graph</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
        </fieldset>
        <ISWebGrid:WebGrid ID="dgActivitySummary" runat="server" Height="90%" UseDefaultStyle="true"
            Width="100%" OnInitializeDataSource="dgActivitySummary_InitializeDataSource"
            OnChartImageProcessing="dgActivitySummary_ChartImageProcessing">
            <RootTable>
                <Columns>
                    <ISWebGrid:WebGridColumn Caption="Fleet" DataMember="fleetName" Name="fleetName">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="# Vehicles" DataMember="TotalVehicles" Name="TotalVehicles">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="# Trips" DataMember="TripCounter" Name="TripCounter">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Avg. Active Vehicles" DataMember="AverageActiveVehicles"
                        Name="AverageActiveVehicles">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Duration (Min)" DataMember="TotalDurationMin" Name="TotalDurationMin">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Service (Min)" DataMember="TotalServiceMin" Name="TotalServiceMin">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Engine On (Min)" DataMember="EngineOnMin" Name="EngineOnMin">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Idling (Min)" DataMember="IdlingTimeMin" Name="IdlingTimeMin">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="PTO On (Min)" DataMember="PTOOnMin" Name="PTOOnMin">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Distance (Km)" DataMember="DistanceKm" Name="DistanceKm">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Fuel Consumed (L)" DataMember="FuelConsumed" Name="FuelConsumed">
                    </ISWebGrid:WebGridColumn>
                </Columns>
            </RootTable>
            <LayoutSettings AutoHeight="true">
            </LayoutSettings>
            <ChartDataCollection>
                <ISWebGrid:ChartPivotDataConfig DataMember="EngineOnMin" AutoCalc="Sum" DataType="System.Int32" />
            </ChartDataCollection>
            <ChartSeriesCollection>
                <ISWebGrid:ChartPivotFilterConfig DataMember="fleetName" />
            </ChartSeriesCollection>
            <ChartSettings Depth="20" ChartType="Bar">
            </ChartSettings>
            <ChartInteractiveUI RibbonState="Hidden">
                <ChartTypeRibbon>
                    <SmoothLineType Enabled="False" />
                    <DoughnutType Enabled="False" />
                    <BarType Enabled="False" />
                    <PieType Enabled="False" />
                    <LineType Enabled="False" />
                    <StepLineType Enabled="False" />
                    <ColumnType Enabled="False" />
                </ChartTypeRibbon>
            </ChartInteractiveUI>
        </ISWebGrid:WebGrid>
    </div>
    <asp:Literal ID="ScriptValues" runat="server"></asp:Literal>
    </form>
</body>
</html>
