<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AllRoutes.aspx.cs" Inherits="ServiceAssignment_AllRoutes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>All routes</title>
    <link rel="stylesheet" href="Scripts/media/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="Scripts/media/css/demo_table.css" />  
    <link rel="stylesheet" href="Scripts/media/css/TableTools.css" />   
    <link rel="stylesheet" href="Scripts/media/gh/gh-buttons.css" /> 
  <script type="text/javascript" src="Scripts/ui/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Scripts/ui/jquery-migrate-1.1.0.js"></script>
  <script type="text/javascript" src="Scripts/ui/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/ckeditor/ckeditor.js"></script>
    <script src="Scripts/media/js/jquery.dataTables.js" type="text/javascript"></script>
    <script type="text/javascript" charset="utf-8" src="Scripts/media/js/ZeroClipboard.js"></script>
	<script type="text/javascript" charset="utf-8" src="Scripts/media/js/TableTools.js"></script>   
    <script src="Scripts/Allroutes.js" type="text/javascript"></script>
    <script type="text/javascript">
        g(document).ready(function() {
            g("#tabs").tabs();
        });
    </script>
    
    <style type="text/css">
    body { font-size: 62.5%; }
    label, input { display:inline; }
    input.text { margin-bottom:12px; width:95%; padding: .4em; }
    fieldset { padding:0; border:0; margin-top:25px; }
    h1 { font-size: 1.2em; margin: .6em 0; }
    div#users-contain { width: 350px; margin: 20px 0; }
    div#users-contain table { margin: 1em 0; border-collapse: collapse; width: 100%; }
    div#users-contain table td, div#users-contain table th { border: 1px solid #eee; padding: .6em 10px; text-align: left; }
    .ui-dialog .ui-state-error { padding: .3em;}
    .validateTips { border: 1px solid transparent; padding: 0.3em; }  
    .ui-assignment { padding: .3em;}  
   
    .ui-tooltip {
        width: 210px;
    }
        .include {
            color: green;
            font-weight: bold;
        }
        .exclude {
            color: red;
            font-weight: bold;
        }
        .sample {
            color:green;
        }
  </style>
</head>
<body>
                        
    <form id="form1" runat="server">
        <div id="tabs" style="width: 1000px; height:300px;">
        <ul>
            <li><a href="#tabs-1">Routes</a></li>
            <li><a href="#tabs-2"><span id="serviceNamePlaceholder">Assignment</span> history</a></li>            
        </ul>
        <div id="tabs-1">           
            <table id="tblAllRoutes" style="width:950px;float:left;">
        <thead>
        <tr>
            <th>Route name</th>
            <th>Last modified by</th>
            <th>Last modified time</th>
            <th>Status</th>
            <th>Is assigned</th>
            <th>Operation</th>
        </tr>
            </thead>
        <tbody>
            <tr>
			<td colspan="6" class="dataTables_empty">Loading data from server</td>
		    </tr>
        </tbody>
    </table>            
   </div>
        <div id="tabs-2">
            <div style="width:700px;float:left;">
            <table id="tblAssignmentHistory" style="width:700px;float:left;">
        <thead>
        <tr>
            <th>Vehicle name</th>
            <th>Last modified by</th>
            <th>Created time</th>
            <th>Last modified time</th>
            <th>Deleted</th>            
        </tr>
            </thead>
        <tbody>
            <tr>
			<td colspan="5" class="dataTables_empty">Please select a service</td>
		    </tr>
        </tbody>
    </table>
                </div>
        </div>
    </div>
 </form>
    
</body>
</html>
