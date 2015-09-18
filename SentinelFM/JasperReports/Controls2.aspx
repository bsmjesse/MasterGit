<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Controls2.aspx.cs" Inherits="JasperReports_Default2" %>

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
                    <div class="well">
                        <div id="divTitle">Engine Hours Analysis</div>
                        <div class="btn-group" id="btnFilter" style="">
                            <button type="button" class="btn btn-default dropdown-toggle btn-xs" data-toggle="dropdown" aria-expanded="false">
                                Filters<span class="caret"></span>

                            </button>
                            <ul class="dropdown-menu pull-right" role="menu" id="dashboard-controls">
                                <li>
                                    <div class="container mydropdown">
                                        <div id="productFamilySelector">
                                            <div class="form-group">
                                                <label for="Fleet_Selector">Fleet_Selector</label>
                                                <input type="text" id="Fleet_Selector" class="form-control" />
                                            </div>
                                            <div class="form-group">
                                                <label for="StartDate">StartDate</label>
                                                <input type="text" id="StartDate" class="form-control" />
                                            </div>
                                            <div class="form-group">
                                                <label for="EndDate">EndDate</label>
                                                <input type="text" id="EndDate" class="form-control" />
                                            </div>
                                            <div class="form-group">
                                                <label for="Select_Days_of_Week">Select_Days_of_Week</label>
                                                <input type="text" id="Select_Days_of_Week" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label for="Driver_Select">Driver_Select</label>
                                            <input type="text" id="Driver_Select" class="form-control" /></div>
                                        <div class="form-group">
                                            <label for="Title">Title</label>
                                            <input type="text" id="Title" class="form-control" /></div>
                                        <div class="form-group">
                                            <label for="Infraction_Category">Infraction_Category</label>
                                            <input type="text" id="Infraction_Category" class="form-control" /></div>
                                        <div class="form-group">
                                            <label for="Infractions_List">Infractions_List</label>
                                            <input type="text" id="Infractions_List" class="form-control" /></div>
                                        <div class="form-group">
                                            <label for="Vehicle_Selector">Vehicle_Selector</label>
                                            <input type="text" id="Vehicle_Selector" class="form-control" /></div>
                                        <button type="button" class="btn btn-default" onclick="runDashboardWithAllParams()">Apply</button>
                                    </div>

                                </li>
                            </ul>
                        </div>

                    </div>
                    <div id="divContainer"></div>
                    <div id="divReportView" style="display: none;">
                        <iframe id="reportView" name="reportView" src="" frameborder="0" style="overflow: hidden; height: 95%; width: 95%; position: absolute;" height="95%" width="95%"></iframe>
                    </div>

                </div>


            </div>
        </div>
    </form>
</body>
</html>
