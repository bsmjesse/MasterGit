﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="JasperReports_Default" %>

<%@ Reference Page="~/JasperReports/Default2.aspx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Analytic Report</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="bower_components/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="bower_components/jquery-ui/themes/smoothness/jquery-ui.css" />
    <link href="bower_components/multiple-select/multiple-select.css" rel="stylesheet" />
    <style type="text/css">
        .glyphicon {
            margin-right: 10px;
        }

        .panel-body {
            padding: 0px;
        }

            .panel-body table tr td {
                padding-left: 15px;
            }

            .panel-body .table {
                margin-bottom: 0px;
            }

        .reportContainer {
            height: 95vh;
        }

        .dashboardControl {
            /*display: inline-block;*/
            float: left;
            position: relative;
            padding-right: 6px;
        }

        #filter-title {
            display: inline-block;
            margin: 0 auto;
            text-align: center;
            font-size: 18px;
        }

        .cssFilter {
            display: inline-block;
            vertical-align: top;
        }

    </style>


    <script type="text/javascript">
        var SERVER_URL = "http://10.10.110.122:8080/jasperserver-pro";
        var token = '<%=Token%>';// ; '645CB5DCC552F852C789A08788A3C2EF'
        var biPublicDashboard = '<%=BiPublicDashboard%>';
        var biPublicReports = '<%=BiPublicReports%>';
        var biPublicAdHoc = '<%=BiPublicAdHoc%>';
        var biOrganizationDashboard = '<%=BiOrganizationDashboard%>';
        var biOrganizationReports = '<%=BiOrganizationReports%>';
        var biDemo = '<%=BiDemo%>';

        function disableEnterKey(e) {
            var key;
            if (window.event)
                key = window.event.keyCode; //IE
            else
                key = e.which; //firefox      

            return (key != 13);
        }

        function getFleetData() {
            return <%=FLEET_DATA%>;
        }

        function getDaysOfWeek() {
            return [[{ groupName: 'Weekday', data: [{ id: 1, title: "Monday" }, { id: 2, title: "Tuesday" }, { id: 3, title: "Wednesday" }, { id: 4, title: "Thursday" }, { id: 5, title: "Friday" }] }], [{ groupName: 'Weekend', data: [{ id: 6, title: "Saturday" }, { id: 7, title: "Sunday" }] }]];
        }

        function getDriver() {
            //return [[{ id: 1, title: "Franklin" }, { id: 2, title: "Tom" }, { id: 3, title: "John" }, { id: 4, title: "Angela" }, { id: 5, title: "William" }, { id: 6, title: "Georage" }]];
            return <%=DRIVER_DATA%>;
        }

        function getInfractionCategory() {
            return [[{ id: "Alarm", title: "Alarm" }, { id: "Diagnostic", title: "Diagnostic" }, { id: "Diagnostic:(Custom)", title: "Diagnostic:(Custom)" }, { id: "DTC", title: "DTC" }, { id: "Violation", title: "Violation" }, { id: "Violation:(Custom)", title: "Violation:(Custom)" }]];
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">

        <div class="container-fluid">
            <div class="row">

                <!-- Menu -->
                <div class="col-sm-2 col-md-2">
                    <div class="panel-group" id="accordion">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseOne"><span class="glyphicon glyphicon-folder-close"></span>Views</a>
                                </h4>
                            </div>
                            <div id="collapseOne" class="panel-collapse collapse">
                                <div class="panel-body">
                                    <table class="table" id="tblViews">
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseTwo"><span class="glyphicon glyphicon-th"></span>Dashboards</a>
                                </h4>
                            </div>
                            <div id="collapseTwo" class="panel-collapse collapse in">
                                <div class="panel-body">
                                    <table class="table" id="tblDashboards">
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseFour"><span class="glyphicon glyphicon-file"></span>Reports</a>
                                </h4>
                            </div>
                            <div id="collapseFour" class="panel-collapse collapse">
                                <div class="panel-body">
                                    <table class="table" id="tblReports">
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseFive"><span class="glyphicon glyphicon-file"></span>Demos</a>
                                </h4>
                            </div>
                            <div id="collapseFive" class="panel-collapse collapse">
                                <div class="panel-body">
                                    <table class="table" id="tblDemos">
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- End of Menu -->

                
                <div class="col-sm-10 col-md-10 reportContainer">
                    <div class="row">
                        <div class="well col-xs-12">
                            <div id="filter-title"></div>
                            <button type="button" class="btn btn-default dropdown-toggle btn-xs" data-toggle="collapse" data-target="#dashboard-filters">
                                Filters<span class="caret"></span>
                            </button>
                            <div class="form-horizontal">
                                <div id="dashboard-filters" class="collapse">

                                    <div id="productFamilySelector">
                                    </div>
                                    <button type="button" class="btn btn-default" onclick="runDashboardWithAllParams()">Apply</button>

                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divContainer" class="reportContainer"></div>
                    <div id="divReportView" class="reportContainer" style="display: none;">
                        <iframe id="reportView" name="reportView" src="" frameborder="0" style="overflow: hidden; height: 100%; width: 100%; position: absolute;" height="100%" width="100%"></iframe>
                    </div>

                </div>


            </div>
        </div>
    </form>
    <script src="bower_components/jquery/dist/jquery.min.js"></script>
    <script src="bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
    <script src="bower_components/underscore/underscore-min.js"></script>
    <script src="bower_components/jquery-ui/jquery-ui.min.js"></script>
    <script src="bower_components/moment/min/moment.min.js"></script>
    <script src="bower_components/string/dist/string.min.js"></script>
    <script src="js/jquery.multiple.select.js"></script>
    <script src="js/bsm.js"></script>
    <script src="js/controls.js"></script>
    <script src="js/visualize.js?_opt=false"></script>
    <script src="js/jasper2.js"></script>
</body>
</html>
