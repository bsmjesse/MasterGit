<%@ Page Language="C#" AutoEventWireup="true" CodeFile="m.aspx.cs" Inherits="DashBoard_idlingnew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="stylesheet" type="text/css" href="extjs/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="shared/example.css" />
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


              //use a renderer for values in the data view.
            function perc(v) {
                return v + '%';
            }



                      Ext.define('MItem',
               {
                   extend: 'Ext.data.Model',
                   fields: ['VehicleDescription', 'Description', 'ServicePerc', 'StatusId']
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





             var cboHrs = Ext.create('Ext.form.field.ComboBox',
   {
       fieldLabel: 'For last',
       labelWidth: 50,
       width: 130,
       hiddenName: 'ddi_hours',
       store:
         new Ext.data.SimpleStore(
         {
             fields: ['alpha2code', 'name'],
             data: [
            ["24", "24 Hours"], ["36", "36 Hours"], ["48", "48 Hours"]]
         }
         ), // end of Ext.data.SimpleStore
       displayField: 'name',
       valueField: 'alpha2code',
       selectOnFocus: true,
       mode: 'local',
       typeAhead: true,
       editable: false,
       triggerAction: 'all',
       value: '36',
       listeners:
         {
             select:
            {
                fn: function (combo, value) {

                    store1.load(
                  {

                      params:
                     {
                         FleetId: cbofleets.value
                         
                     }
                  }
                  );


                    store2.load(
                  {

                      params:
                     {
                         FleetId: cbofleets.value
                         
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
       width: 225,
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
       emptyText:'All Vehicles ',
       listeners:
         {
             select:
            {
                fn: function (combo, value) {

                  

                    store1.load(
                  {

                      params:
                     {
                         FleetId: combo.getValue()
                         
                     }
                  }
                  );


                    store2.load(
                  {

                      params:
                     {
                         FleetId: combo.getValue()
                         
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
          url: './dashboardService.asmx/LoadMaintenanceDetails',
          timeout: 520000,

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
                        FleetId: cbofleets.value
                        
                    }
         ,


          reader:
         {
             type: 'json',
             model: 'MItem',
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
          url: './dashboardService.asmx/LoadMaintenanceSummary',
          timeout: 520000,

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
                        FleetId: cbofleets.value
                        
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
                 id: 'idling-form',
                 store: store1,
                 columnLines: true,
            columns: [
         
            {
                text: 'Maintenance',
                width: 190,
                sortable: true,
                dataIndex: 'Description'


            }
        ]


 });

            //create a bar series to be at the top of the panel.
    var barChart = Ext.create('widget.panel', {
        width: 400,
        height: 320,

        frame: false,

        renderTo: Ext.getBody(),
        layout: 'fit',
        tbar: [cboHrs, cbofleets],
        items: {
            xtype: 'chart',
            id: 'chartCmp',
            animate: true,
            store: store2,
            shadow: true,
            legend: {
                position: 'right'
            },
            insetPadding: 30,
            theme: 'Base:gradients',
            series: [{
                type: 'pie',
                field: 'data',
                showInLegend: false,

                tips: {
                    trackMouse: true,
                    width: 140,
                    height: 38,
                    renderer: function (storeItem, item) {
                        this.setTitle(storeItem.get('name') + '<br />' + storeItem.get('data'));
                    }
                },
                highlight: {
                    segment: {
                        margin: 20
                    }
                },
                label: {
                    field: 'name',
                    display: 'rotate',
                    contrast: true,
                    font: '18px Arial'
                },

                listeners: {
                    'itemmouseup': function (obj) {

                        store2.clearFilter();

                        //                        if (obj.storeItem.data['name'] == 'Idling') {
                        //                            store2.filter('StatusId', 1);
                        //                        }
                        //                        if (obj.storeItem.data['name'] == 'EngineHrs') {
                        //                            store2.filter('StatusId', 0);
                        //                        }

                    }
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
                 width: 370,
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
                height: 300,
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
                        FleetId: cbofleets.value
                     
                    }

                }

                 );




             store2.load(
                {
                    extraParams:
                    {
                        FleetId: cbofleets.value
                        
                    }

                }

                 );



             var gp = Ext.getCmp('idling-form');
         });


     </script>
</head>
<body style="margin: -10px">
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
