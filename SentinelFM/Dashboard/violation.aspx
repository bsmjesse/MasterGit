<%@ Page Language="C#" AutoEventWireup="true" CodeFile="violation.aspx.cs" Inherits="SentinelFM.DashBoard_violation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <!-- ExtJS -->
  
   <link rel="stylesheet" type="text/css" href="extjs/resources/css/ext-all-gray.css" />
    <script type="text/javascript" src="extjs/bootstrap.js"></script>
    <!-- Shared -->
    <link rel="stylesheet" type="text/css" href="shared/example.css" />


        
     <script type="text/javascript">
        Ext.require(['Ext.data.*']);

        Ext.onReady(function () {

            window.generateGraphData = function () {
                 var data = [];
                   <%=strGraphData %>
                return data;
            };

           window.generateGridData = function () {
                var dataGrid = [];
                   <%=strGridData %>
                return dataGrid;
            };

           window.generateFleetData = function () {
                var fleetList = [];
                   <%=strFleets %>
                return fleetList;
            };
           
           
           
            window.store1 = Ext.create('Ext.data.JsonStore', {
                fields: ['name', 'data'],
                data: generateGraphData()
            });
         
           
        window.store2 = Ext.create('Ext.data.JsonStore', {
            fields: [
                {name: 'description'},
                {name: 'Speeding'},
                {name: 'extBraking'},
                {name: 'extAcc'},
                
                {name: 'Total'}
                
            ],
            data: generateGridData()
        });



//            window.store3 = Ext.create('Ext.data.JsonStore', {
//            fields: [
//                {name: 'FleetId'},
//                {name: 'FleetName'}
//                
//            ],
//            data: generateFleetData()
//        });





        });

     </script>

     <script type="text/javascript" src="classes/violation.js"></script>
  
    
</head>
<body  style="margin: -10px">
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
