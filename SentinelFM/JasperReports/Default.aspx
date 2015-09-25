<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="JasperReports_Default" %>

<%@ Reference Page="~/JasperReports/Default2.aspx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Analytic Report</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <!-- bower:css -->
    <link rel="stylesheet" href="bower_components/bootstrap-daterangepicker/daterangepicker.css" />
    <link rel="stylesheet" href="bower_components/select2/dist/css/select2.css" />
    <!-- endbower -->
    <link href="bower_components/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="bower_components/jquery-ui/themes/smoothness/jquery-ui.css" />
    <link href="js/multiple-select/multiple-select.css" rel="stylesheet" />

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


</head>
<body>
    <form id="form1" runat="server">

        <div class="container-fluid">
            <div class="row">

                <!-- Menu -->
                <div class="col-sm-3 col-xs-3 col-md-2 col-md-2">
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


                <div class="col-sm-9 col-xs-9 col-md-10 reportContainer">
                    <div class="row">
                        <div class="well col-xs-12">
                            <div id="filter-title"></div>
                            <button id="dashboard-filters-toggle" type="button" class="btn btn-default dropdown-toggle btn-xs" data-toggle="collapse" data-target="#dashboard-filters" onclick="BSM.Controls.onToggle()">
                                Filters<span class="caret"></span>
                            </button>
                            <div id="dashboard-filters" class="collapse">

                                <div id="productFamilySelector" class="form-horizontal">
                                </div>
                                <br />
                                <button type="button" class="btn btn-default" data-toggle="collapse" data-target="#dashboard-filters" onclick="BSM.jasper.runDashboardWithAllParams()">Apply</button>

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
        <!-- bower:js -->
        <script src="bower_components/es5-shim/es5-shim.js"></script>
        <script src="bower_components/jquery/dist/jquery.js"></script>
        <script src="bower_components/bootstrap/dist/js/bootstrap.js"></script>
        <script src="bower_components/moment/moment.js"></script>
        <script src="bower_components/bootstrap-daterangepicker/daterangepicker.js"></script>
        <script src="bower_components/json3/lib/json3.js"></script>
        <script src="bower_components/select2/dist/js/select2.js"></script>
        <script src="bower_components/string/dist/string.min.js"></script>
        <!-- endbower -->
        <script src="js/bsm.js"></script>
        <script type="text/javascript">
            var SERVER_URL = "http://10.10.110.122:8080/jasperserver-pro";
            var token = '<%=Token%>';// ; '645CB5DCC552F852C789A08788A3C2EF'
            var biPublicDashboard = '<%=BiPublicDashboard%>';
            var biPublicReports = '<%=BiPublicReports%>';
            var biPublicAdHoc = '<%=BiPublicAdHoc%>';
            var biOrganizationDashboard = '<%=BiOrganizationDashboard%>';
            var biOrganizationReports = '<%=BiOrganizationReports%>';
            var biDemo = '<%=BiDemo%>';
            BSM.drivers = <%=DRIVER_DATA%>;

        </script>
    </form>

    <script src="js/visualize.js?_opt=false"></script>


    <script src="js/controls.js"></script>
    <script src="js/jasper2.js"></script>
    <script src="js/multiple-select/jquery.multiple.select.js"></script>

    <script>

    </script>
</body>
</html>
