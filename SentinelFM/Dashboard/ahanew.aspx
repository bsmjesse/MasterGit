<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ahanew.aspx.cs" Inherits="DashBoard_ahanew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <link rel="stylesheet" type="text/css" href="../extjs/resources/css/ext-all-gray.css" />
<%--    <link rel="stylesheet" type="text/css" href="shared/example.css" />--%>
    <script type="text/javascript" src="../extjs/ext-all.js"></script>
    <script type="text/javascript" src="extjs/AspWebAjaxProxy.js"></script>
     <script type="text/javascript">
         Ext.require([
    'Ext.form.*',
    'Ext.data.*',
    'Ext.chart.*',
    'Ext.grid.Panel',
    'Ext.layout.container.Column'
]);


          var selValue=10;

          Ext.onReady(function () {
          
              Ext.define('ahaData',
   {
       extend: 'Ext.data.Model',
       fields: ['FleetName', 'Counter']
   }
   );





   var cboHrs = Ext.create('Ext.form.field.ComboBox',
   {
       fieldLabel: 'For last',
       labelWidth: 50,
       width: 160, 
       hiddenName: 'ddi_hours',
       store:
         new Ext.data.SimpleStore(
         {
             fields: ['alpha2code', 'name'],
             data: [
            ["24", "24 Hrs"], ["36", "36 Hrs"], ["48", "48 Hrs"], ["72", "72 Hrs"]]
         }
         ), // end of Ext.data.SimpleStore
       displayField: 'name',
       valueField: 'alpha2code',
       selectOnFocus: true,
       mode: 'local',
       typeAhead: true,
       editable: false,
       triggerAction: 'all',
       value: '72',
       listeners:
         {
             select:
            {
                fn: function (combo, value) {
                    store1.load(
                  {

                      params:
                     {
                         TopFleets: cbofleets.value,
                         TopHours: combo.getValue()
                     }
                  }
                  );

                    // mainstore.sort('OriginDateTime', 'DESC');
                }
            }
         }

   });





   var cbofleets = Ext.create('Ext.form.field.ComboBox',
   {


       fieldLabel: 'For top',
       labelWidth: 50,
       width: 160, 
       hiddenName: 'ddi_top',
       store:
         new Ext.data.SimpleStore(
         {
             fields: ['alpha2code', 'name'],
             data: [
            ["5", "5 Fleets"], ["10", "10 Fleets"], ["15", "15 Fleets"], ["25", "25 Fleets"]]

         }
         ), // end of Ext.data.SimpleStore
       displayField: 'name',
       valueField: 'alpha2code',
       selectOnFocus: true,
       mode: 'local',
       typeAhead: true,
       editable: false,
       triggerAction: 'all',
       value: '25',
       listeners:
         {
             select:
            {
                fn: function (combo, value) {
                    store1.load(
                  {

                      params:
                     {
                         TopFleets: combo.getValue(),
                         TopHours: cboHrs.value
                     }
                  }
                  );

                    // mainstore.sort('OriginDateTime', 'DESC');
                }
            }
         }

   });





              var store1 = new Ext.data.Store(
   {
       autoload: false,
       proxy: new Ext.ux.AspWebAjaxProxy(
      {
          url: './dashboardService.asmx/LoadAHA',
          timeout: 120000,

          actionMethods:
         {
             create: 'POST',
             destroy: 'DELETE',
             read: 'POST',
             update: 'POST'
         }
         ,

         extraParams:
                    {
                        TopFleets: cbofleets.value,
                        TopHours: cboHrs.value
                    }
         ,


          reader:
         {
             type: 'json',
             model: 'ahaData',
             root: 'd'


         }
         ,
          headers:
         {
             'Content-Type': 'application/json; charset=utf-8'
         }
      }
      )

   }
   );



 var cmdReport = Ext.create('Ext.button.Button', {
     text: 'View Details',

     handler: function () {
         window.open('reportViewer.aspx?reportId=4&fleetId=' + cbofleets.value + '&data=' + cboHrs.value, 'Report', 'left=20,top=20,width=700,height=700,toolbar=1,resizable=1');

     }
 });





              var panel1 = Ext.create('widget.panel',
   {
       width: 400,
       height: 327,
       renderTo: Ext.getBody(),
       layout: 'fit',
       tbar: [cboHrs, cbofleets, { xtype: 'tbfill' },cmdReport],
       items:
      {
          xtype: 'chart',
          animate: true,
          shadow: true,
          autoload: false,
          store: store1,
          axes: [
         {
             type: 'Numeric',
             position: 'bottom',
             fields: ['Counter'],
             title: '# of Alarms',
             grid: true,
             minimum: 0
         }
         ,
         {
             type: 'Category',
             position: 'left',
             fields: ['FleetName'],
             title: 'Fleet'

         }
         ],
          series: [
         {
             type: 'bar',
             axis: 'bottom',
             highlight: true,
             tips:
            {
                trackMouse: true,
                width: 140,
                height: 38,
                renderer: function (storeItem, item) {
                    this.setTitle(storeItem.get('FleetName') + ': ' + storeItem.get('Counter') + ' views');
                }
            }
            ,
             label:
            {
                display: 'insideEnd',
                field: 'Counter',
                renderer: Ext.util.Format.numberRenderer('0'),
                orientation: 'horizontal',
                color: '#333',
                'text-anchor': 'middle'
            }
            ,
             xField: 'FleetName',
             yField: 'Counter'
         }
         ]
      }
   }
   );

              store1.load(
                {
                    extraParams:
                    {
                        TopFleets: cbofleets.value,
                        TopHours: cboHrs.value
                    }

                }

                 );


          }
);

     </script>



</head>
<body >
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
