
Ext.require(['Ext.data.*', 'Ext.ux.AspWebAjaxProxy', 'Ext.form.*']);
Ext.require('Ext.chart.*');
Ext.require('Ext.layout.container.Fit');



Ext.onReady(function () {



    Ext.apply(Ext.form.field.VTypes, {
        daterange: function (val, field) {
            var date = field.parseDate(val);

            if (!date) {
                return false;
            }
            if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
                var start = field.up('form').down('#' + field.startDateField);
                start.setMaxValue(date);
                start.validate();
                this.dateRangeMax = date;
            }
            else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
                var end = field.up('form').down('#' + field.endDateField);
                end.setMinValue(date);
                end.validate();
                this.dateRangeMin = date;
            }
            /*
            * Always return true since we're only using this vtype to set the
            * min/max allowed values (these are tested for after the vtype test)
            */
            return true;
        },

        daterangeText: 'Start date must be less than end date',

        password: function (val, field) {
            if (field.initialPassField) {
                var pwd = field.up('form').down('#' + field.initialPassField);
                return (val == pwd.getValue());
            }
            return true;
        },

        passwordText: 'Passwords do not match'
    });



    Ext.define('ahaData',
   {
       extend: 'Ext.data.Model',
       fields: ['FleetName', 'Counter']
   }
   );



    Ext.define('GData',
               {
                   extend: 'Ext.data.Model',
                   fields: ['name', 'data']
               }
               );


    var storeAHA = new Ext.data.Store(
   {
       proxy: new Ext.ux.AspWebAjaxProxy(
      {
          autoload: false,
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
                        TopFleets: 10,
                        TopHours: 50
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



    var storeIdling = new Ext.data.Store(
   {
       autoload: false,
       proxy: new Ext.ux.AspWebAjaxProxy(
      {
          url: './dashboardService.asmx/LoadIdlingSummary',
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
                        FleetId: 2611,
                        TopHours: 50
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






    idlingConfig = {
        flex: 1,
        xtype: 'chart',
        animate: true,
        shadow: true,
        autoload: false,
        store: storeIdling,
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
            }



        }]
    };




    ahaConfig = {
        flex: 1,
        xtype: 'chart',
        animate: true,
        shadow: true,
        autoload: false,
        store: storeAHA,
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
    };






    //create a grid that will list the dataset items.
    gridFleetConfig = {
        flex: 1,
        xtype: 'grid',

        autoload: false,
        store: storeFleet,
        columnLines: false,
        columns: [
            {
                id: 'name',
                text: 'Fleet',
                width: 220,
                sortable: true,
                dataIndex: 'name'

            }
        ]

    };




        dateConfig = {

            xtype: 'panel',
            frame: true,
            title: 'Range',
            bodyPadding: '5px 0px 0',
            width: 350,
            height: 130,
            fieldDefaults: {
                labelWidth: 125,
                msgTarget: 'side',
                autoFitErrors: false
            },
            defaults: {
                width: 200
            },
            
            items: [
            {
                xtype: 'datefield',
                format: "m/d/Y",
                fieldLabel: 'Start Date',
                name: 'startdt',
                id: 'startdt',
//                vtype: 'daterange',
                endDateField: 'enddt' // id of the end date field
            },
            {
                xtype: 'datefield',
                format: "m/d/Y",
                fieldLabel: 'End Date',
                name: 'enddt',
                id: 'enddt',
//                vtype: 'daterange',
                startDateField: 'startdt' // id of the start date field
            },

             {
                xtype: 'combo',
                        fieldLabel: 'Top',
                        name: 'top',
                        store: ['10', '20', '30'],
                        triggerAction: 'all'


             }

            
        ]

        };



    var mainPanel = Ext.create('widget.panel',
   {
       width: 1224,
       height: 800,
       renderTo: Ext.getBody(),
       layout: 'fit',
       border: false,
       items: [{
           xtype: 'panel',
           layout: {
               type: 'hbox',
               align: 'stretch'
           },
           items: [
           
            {
                flex: 1,
                width: 230,
               layout: 'fit',
               xtype: 'container',
               layout: {
                   type: 'vbox',
                   align: 'stretch'
               },
               items: [dateConfig,
                    gridFleetConfig
                ]
           },
           
           
           {
               flex: 1,
               layout: 'fit',
               xtype: 'container',
               layout: {
                   type: 'vbox',
                   align: 'stretch'
               },
               items: [
                    ahaConfig,
                    idlingConfig
                ]
           }, {
               flex: 1,
               layout: 'fit',
               xtype: 'container',
               layout: {
                   type: 'vbox',
                   align: 'stretch'
               },
               items: [
                    ahaConfig,
                    ahaConfig
                ]
           }]
       }]
   }
   );

    storeAHA.load({
        extraParams: { TopFleets: 10, TopHours: 50 }
    });

    storeIdling.load({
        extraParams: { FleetId: 2611, TopHours: 50 }
    });


    storeFleet.load();


}
);