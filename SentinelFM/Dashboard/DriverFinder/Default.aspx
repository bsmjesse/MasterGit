<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SentinelFM.DriverFinder_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Map</title>
    <link rel="stylesheet" href="Content/theme/default/style.css" type="text/css" />
    <link rel="stylesheet" href="Content/style.css" type="text/css" />
    <link rel="stylesheet" href="Content/DataTables-1.9.2/media/css/demo_table.css" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Scripts/OpenLayers.js" type="text/javascript"></script>    
    <script src="Scripts/DataTables-1.9.2/media/js/jquery.dataTables.js" type="text/javascript"></script>   
    <script src="Scripts/Map1.js" type="text/javascript"></script> 
</head>
<body onload="init()">
    <div style="position: absolute; display:none;height:200px;width:200px;border:3px solid green;z-index: 0;" id="popup">Hi</div> 
    <h1 id="title">Find Vehicles/Drivers</h1>
        

<div id="map" class="smallmap"></div>

    <br />
<div id="searchFormOnPage"></div>
<br/>

<form id="form1" runat="server">
   

    <div>
   
    <div id="errorMsg" runat="server"></div>
    </div>
    </form>
    
    
    <table id="DriverInfoTable" cellpadding="0" cellspacing="0" border="0" class="display">
        <thead>
            <tr>
        <th>UnitId</th>
        <th>Description</th>
        <th>Vehicle Type</th>
        <th>Skills</th>
        <th>Driver Name</th>
        <th>Address</th>
        <th>Distance</th>
        <th>History</th>
            </tr>
        </thead>
        <tbody>            
        </tbody>
        
    </table>
</body>
</html>
