<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pie.aspx.cs" Inherits="SentinelFM.DBoard_Pie" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="extjs/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="shared/example.css" />
    <script type="text/javascript" src="extjs/bootstrap.js"></script>
    <!--<script type="text/javascript" src="example-data.js"></script>-->
    


    
  <script type="text/javascript">
        Ext.require(['Ext.data.*']);

        Ext.onReady(function () {

            window.generateData = function () {
                var data = [];
                   <%=strData %>
                return data;
            };


            window.store1 = Ext.create('Ext.data.JsonStore', {
                fields: ['name', 'data'],
                data: generateData()
            });




        });

     </script>

     <script type="text/javascript" src="classes/pie.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
