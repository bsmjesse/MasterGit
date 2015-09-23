<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default3.aspx.cs" Inherits="JasperReports_Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Analytic Report</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="http://netdna.bootstrapcdn.com/bootstrap/3.0.3/css/bootstrap.min.css" rel="stylesheet" />
    <style type="text/css">
        body {
            margin-top: 50px;
        }

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

        .container {
            width: 100%;
        }

        .reportContainer {
            height: 640px;
        }

        #productFamilySelector {
            display: inline-block;
        }

        .dashboardControl {
            display: inline-block;
            float: left;
            position: relative;
            padding-right: 6px;
        }
    </style>
    <%--<script src="http://code.jquery.com/jquery-1.11.1.min.js"></script>--%>
    <%--<script src="http://netdna.bootstrapcdn.com/bootstrap/3.0.3/js/bootstrap.min.js"></script>--%>
    <script type='text/javascript' src="js/jquery-2.1.4.min.js"></script>
    <script type='text/javascript' src="js/bootstrap/js/bootstrap.min.js"></script>
    <script type='text/javascript' src="js/underscore-min.js"></script>
    <script src="js/visualize.js?_opt=false"></script>
    <script src="js/jasper2.js"></script>
    <script type="text/javascript">
        var SERVER_URL = "http://10.10.110.122:8080/jasperserver-pro";
        var token = '<%=Token%>'; //'645CB5DCC552F852C789A08788A3C2EF';//;
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="productFamilySelector">
            <div class="form-group">
                <input type="text" id="Fleet_Selector" class="form-control floating-label" placeholder="Fleet_Selector" />
            </div>
            <div class="form-group">
                <input type="text" id="StartDate" class="form-control floating-label" placeholder="StartDate" />
            </div>
            <div class="form-group">
                <input type="text" id="EndDate" class="form-control floating-label" placeholder="EndDate" />
            </div>
            <div class="form-group">
                <input type="text" id="Select_Days_of_Week" class="form-control floating-label" placeholder="Select_Days_of_Week" />
            </div>

            <div class="form-group">
                <input type="text" id="Driver_Select" class="form-control floating-label" placeholder="Driver_Select" />
            </div>
            <div class="form-group">
                <input type="text" id="Title" class="form-control floating-label" placeholder="Title" />
            </div>
            <div class="form-group">
                <input type="text" id="Infraction_Category" class="form-control floating-label" placeholder="Infraction_Category" />
            </div>
            <div class="form-group">
                <input type="text" id="Infractions_List" class="form-control floating-label" placeholder="Infractions_List" />
            </div>
            <div class="form-group">
                <input type="text" id="Vehicle_Selector" class="form-control floating-label" placeholder="Vehicle_Selector" />
            </div>
            <button type="button" class="btn btn-default" onclick="runDashboardWithAllParams()">Apply</button>
        </div>

        <div class="container">
            <div class="row">
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
                <div class="col-sm-10 col-md-10 reportContainer">

                    <div id="divContainer"></div>
                    <div id="divReportView" style="display: none;">
                        <iframe id="reportView" name="reportView" src="" frameborder="0" style="overflow: hidden; height: 95%; width: 95%; position: absolute;" height="95%" width="95%"></iframe>
                    </div>

                </div>


            </div>
        </div>
    </form>
    <script type="text/javascript" src="js/controls.js"></script>
</body>
</html>
