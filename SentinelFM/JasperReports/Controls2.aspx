<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Controls2.aspx.cs" Inherits="JasperReports_Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Analytic Report</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta charset="utf-8">  
  <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
  <link href="js/multiple-select/multiple-select.css" rel="stylesheet" />
  <style type="text/css">
    body {
	    font-family: "Trebuchet MS", "Helvetica", "Arial",  "Verdana", "sans-serif";
	    font-size: 70.5%;
    }
    
    .form-group {
        margin-top: 10px;
    }

    .form-control {clear:both;display:block;}
    
    .ms-drop {z-index: 10031;}
  </style>

  <script src="//code.jquery.com/jquery-1.10.2.js"></script>
  <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>

    <script src="js/controls.js"></script>
    <script type="text/javascript">
        var SERVER_URL = "http://dev.sentinelfm.com";
        var token = "";

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
            return [[{groupName:'Weekday', data: [{ id: 1, title: "Monday" }, { id: 2, title: "Tuesday" }, { id: 3, title: "Wednesday" }, { id: 4, title: "Thursday"  }, { id: 5, title: "Friday"}]}], [{groupName: 'Weekend', data: [{ id: 6, title: "Saturday" }, { id: 7, title: "Sunday" }]}]];
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
        <div class="container">
            <div class="row">
                <div class="col-sm-2 col-md-2">
       
                </div>
                <div class="col-sm-10 col-md-10 reportContainer">
                    <div class="well">
                        
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
    <script type='text/javascript' src="js/multiple-select/jquery.multiple.select.js"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            $('#dashboard-controls').on({
                "click": function (e) {
                    e.stopPropagation();
                }
            });
            BSM.Controls.scanControls();
        });
    </script>
</body>
</html>
