<%@ Page Language="C#" AutoEventWireup="true" CodeFile="violationnew.aspx.cs" Inherits="DashBoard_violationnew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="extjs/resources/css/ext-all-gray.css" />
   <%-- <link rel="stylesheet" type="text/css" href="shared/example.css" />--%>
    <script type="text/javascript" src="extjs/bootstrap.js"></script>
    <script type="text/javascript" src="extjs/AspWebAjaxProxy.js"></script>
     <script type="text/javascript">
         Ext.require([
    'Ext.form.*',
    'Ext.data.*',
    'Ext.chart.*',
    'Ext.grid.Panel',
    'Ext.layout.container.Column'
]);


         Ext.onReady(function () {


             Ext.define('VData',
               {
                   extend: 'Ext.data.Model',
                   fields: ['description', 'harshBraking', 'harshAcc', 'Speeding', 'TotalHarshBraking', 'TotalHarshAcceleration', 'TotalSpeeding', 'Total']
               }
               );




             Ext.define('GData',
               {
                   extend: 'Ext.data.Model',
                   fields: ['name', 'data']
               }
               );


             //use a renderer for values in the data view.
             function perc(v) {
                 return v + '%';
             }





             var storeFleet = new Ext.data.Store(
   {
       autoload: false,
       proxy: new Ext.ux.AspWebAjaxProxy(
      {
          url: './dashboardService.asmx/LoadFleets',
          timeout: 120000,

          actionMethods:
         {
             create: 'POST',
             destroy: 'DELETE',
             read: 'POST',
             update: 'POST'
         }
         ,

          reader:
         {
             type: 'json',
             model: 'GData',
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




             var bd = Ext.getBody(),
        form = false,
        rec = false,
        selectedStoreItem = false,
             //performs the highlight of an item in the bar series
        selectItem = function (storeItem) {
            var name = storeItem.get('description'),
                series = barChart.series.get(0),
                i, items, l;

            series.highlight = true;
            series.unHighlightItem();
            series.cleanHighlights();
            for (i = 0, items = series.items, l = items.length; i < l; i++) {
                if (name == items[i].storeItem.get('description')) {
                    selectedStoreItem = items[i].storeItem;
                    series.highlightItem(items[i]);
                    break;
                }
            }
            series.highlight = false;
        }



        var cmdReport =Ext.create('Ext.button.Button', {
    text: 'View Details',
    
    handler: function() {
       window.open('reportViewer.aspx?reportId=1&fleetId='+cbofleets.value+'&data='+cboHrs.value,'Report','left=20,top=20,width=700,height=700,toolbar=1,resizable=1');

    }
});



             var cboHrs = Ext.create('Ext.form.field.ComboBox',
   {
       fieldLabel: 'For last',
       labelWidth: 50,
       width: 120,
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
                         FleetId: cbofleets.value,
                         TopHours: combo.getValue()
                     }
                  }
                  );


                    store2.load(
                  {

                      params:
                     {
                         FleetId: cbofleets.value,
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

        autoSelect: false,
       fieldLabel: 'Fleet',
       labelWidth: 35,
       width: 155,
       hiddenName: 'ddi_top',
       store: storeFleet,
       displayField: 'name',
       valueField: 'data',
       selectOnFocus: true,
       mode: 'local',
       typeAhead: true,
       editable: false,
       triggerAction: 'all',
       value:<%=strDefaultFleet %>,
       emptyText:'<%=strDefaultFleetName %>',
       listeners:
         {
             select:
            {
                fn: function (combo, value) {

                  

                    store1.load(
                  {

                      params:
                     {
                         FleetId: combo.getValue(),
                         TopHours: cboHrs.value
                     }
                  }
                  );


                    store2.load(
                  {

                      params:
                     {
                         FleetId: combo.getValue(),
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
          url: './dashboardService.asmx/LoadViolationsDetails',
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
                        FleetId: cbofleets.value,
                        TopHours: cboHrs.value
                    }
         ,


          reader:
         {
             type: 'json',
             model: 'VData',
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






   var store2 = new Ext.data.Store(
   {
       autoload: false ,
       proxy: new Ext.ux.AspWebAjaxProxy(
      {
          url: './dashboardService.asmx/LoadViolationsSummary',
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
                        FleetId: cbofleets.value,
                        TopHours: cboHrs.value
                    }
         ,


          reader:
         {
             type: 'json',
             model: 'GData',
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







             //create a grid that will list the dataset items.
             var gridPanel = Ext.create('Ext.grid.Panel', {
                 id: 'violation-form',
                 store: store1,
                 columnLines: true,
                 columns: [
            {
                id: 'description',
                text: 'Vehicle',
                width: 105,
                sortable: true,
                dataIndex: 'description'

            },

                {
                    text: 'Speeding',
                    width: 60,
                    sortable: true,
                    dataIndex: 'Speeding'


                },

            {
                text: 'Harsh.Braking',
                width: 70,
                sortable: true,
                dataIndex: 'harshBraking'
            }
            ,
            {
                text: 'Harsh.Acc',
                width: 60,
                sortable: true,
                dataIndex: 'harshAcc'


            },

            {
                text: 'Total',
                width: 60,
                sortable: true,
                dataIndex: 'Total'

            }

        ]


             });

             //create a bar series to be at the top of the panel.
             var barChart = Ext.create('widget.panel', {
                

                 renderTo: Ext.getBody(),
                 layout: 'fit',
                 tbar: [cboHrs, cbofleets,{ xtype: 'tbfill' },cmdReport],
                 items: {
                     xtype: 'chart',
                     animate: true,
                     shadow: true,
                     store: store2,
                     axes: [{
                         type: 'Numeric',
                         position: 'left',
                         fields: ['data'],
                         title: 'Violations',
                         grid: true,
                         minimum: 0
                     }, {
                         type: 'Category',
                         position: 'bottom',
                         fields: ['name'],
                         title: '',
                         label: {
                             rotate: {
                                 degrees: 270
                             }
                         }
                     }],
                     series: [{
                         type: 'column',
                         axis: 'left',
                         gutter: 80,
                         xField: 'name',
                         yField: ['data'],
                         tips: {
                             trackMouse: true,
                             width: 74,
                             height: 38,
                             renderer: function (storeItem, item) {
                                 this.setTitle(storeItem.get('name') + '<br />' + storeItem.get('data'));
                             }
                         },
                         style: {
                             fill: '#38B8BF'
                         }
                     }]
                 }
             });

             /*
             * Here is where we create the Form
             */
             var gridForm = Ext.create('Ext.form.Panel', {
                 frame: false,
                 bodyPadding: 0,
                 width: 382,
                 height: 640,

                 fieldDefaults: {
                     labelAlign: 'left',
                     msgTarget: 'side'
                 },

                 layout: {
                     type: 'vbox',
                     align: 'stretch'
                 },

                 items: [
            {
                height: 328,
                layout: 'fit',
                margin: '0 0 0 0',
                border: false,
                items: [barChart]
            },
            {

                layout: { type: 'hbox', align: 'stretch' },
                flex: 1,
                margin: '0 0 0 0',
                border: false,
                bodyStyle: 'background-color: transparent',

                items: [gridPanel]
            }],
                 renderTo: bd
             });


             storeFleet.load();


             store1.load(
                {
                    extraParams:
                    {
                        FleetId: cbofleets.value,
                        TopHours: cboHrs.value
                    }

                }

                 );




             store2.load(
                {
                    extraParams:
                    {
                        FleetId: cbofleets.value,
                        TopHours: cboHrs.value
                    }

                }

                 );



             var gp = Ext.getCmp('violation-form');
         });


     </script>
</head>
<body >
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
