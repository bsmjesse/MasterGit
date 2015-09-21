<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="JasperReports_Default" %>
<%@ Reference Page="~/JasperReports/Default2.aspx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Analytic Report</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
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

        .container {            
            width: 100%; 
            float:left;           
        }

        .col-sm-2 {            
            height: 95vh;
            overflow-y : auto;
            padding-right:5px;            
        }
        .col-md-10 {
            height: 100%;
            padding-left:5px;
        }
        .reportContainer {
            height: 95vh;
        }
        #productFamilySelector {
            /*display: inline-block;*/
            width: 100%;
        }
        .dashboardControl {
            /*display: inline-block;*/
            float: left;
            position: relative;
            padding-right: 6px; 
        }
        .well{
            text-align:center;
            vertical-align:middle;            
            margin-bottom:10px;
            height:30px;
            padding:2px;
        }
        #divTitle{
            display: inline-block;
            margin: 0 auto;
            text-align:center;
            font-size:18px;
        }
        .cssFilter{
            display: inline-block;
            vertical-align:top;
           
        }
        #btnFilter{
             float:right;
        }

        .mydropdown{
            width:300px;
        }

        .ui-datepicker {
            font-family: "Trebuchet MS", "Helvetica", "Arial",  "Verdana", "sans-serif";
	        font-size: 90.5%;
        }
    </style>
    <%--<script src="http://code.jquery.com/jquery-1.11.1.min.js"></script>--%>
    <%--<script src="http://netdna.bootstrapcdn.com/bootstrap/3.0.3/js/bootstrap.min.js"></script>--%>
    <script type='text/javascript' src="js/jquery-2.1.4.min.js"></script>
    <script type='text/javascript' src="js/bootstrap/js/bootstrap.min.js"></script>
    <script type='text/javascript' src="js/underscore-min.js"></script>
    <script src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    
    <script src="js/controls.js"></script>
    <script src="js/visualize.js?_opt=false"></script>
    <script src="js/jasper2.js"></script>
    <script type="text/javascript">
        var SERVER_URL = "http://bi.sentinelfm.com/jasperserver-pro";
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

        $(document).ready(function () {
            $('#productFamilySelector').on({
                "click": function (e) {
                    e.stopPropagation();
                }
            });
            //BSM.Controls.scanControls();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        

        <!-- Modal -->
        <div id="myModal" class="modal fade" role="dialog">
            <div class="modal-dialog">

                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h4 class="modal-title">Filters</h4>
                    </div>
                    <div class="modal-body">
                        
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal" onclick="runDashboardWithAllParams()">Apply</button>
                    </div>
                </div>

            </div>
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
                        
                    <div class="well">
                        <div id="divTitle"></div>
                        <div class="btn-group" id="btnFilter" style="display: none;">
                            <button type="button" class="btn btn-default dropdown-toggle btn-xs" data-toggle="dropdown">
                                Filters<span class="caret"></span>

                            </button>
                            <ul class="dropdown-menu pull-right" role="menu">
                                <li>
                                    <div class="container mydropdown">
                                        <div id="productFamilySelector">
                                        </div>
                                        <button type="button" class="btn btn-default" onclick="runDashboardWithAllParams()">Apply</button>
                                    </div>

                                </li>
                            </ul>
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

    <script type='text/javascript' src="js/multiple-select/jquery.multiple.select.js"></script>    
</body>
</html>
