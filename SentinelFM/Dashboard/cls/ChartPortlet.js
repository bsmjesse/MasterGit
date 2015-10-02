/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
Ext.define('Ext.app.ChartPortlet', {

    extend: 'Ext.panel.Panel',
    alias: 'widget.chartportlet',

    requires: [
        'Ext.data.JsonStore',
        'Ext.chart.theme.Base',
        'Ext.chart.series.Series',
        'Ext.chart.series.Line',
        'Ext.chart.axis.Numeric'
    ],

    generateData: function(){
        var data = [{
                name: 'x',
                djia: 10000,
                sp500: 1100
            }],
            i;
        for (i = 1; i < 50; i++) {
            data.push({
                name: 'x',
                sp500: data[i - 1].sp500 + ((Math.floor(Math.random() * 2) % 2) ? -1 : 1) * Math.floor(Math.random() * 7),
                djia: data[i - 1].djia + ((Math.floor(Math.random() * 2) % 2) ? -1 : 1) * Math.floor(Math.random() * 7)
            });
        }
        return data;
    },

    initComponent: function () {



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
                         TopFleets: cbofleets.value,
                         TopHours: combo.getValue()
                     }
                  }
                  );

                    // mainstore.sort('OriginDateTime', 'DESC');
                }
            }
         }

   }





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
            ["5", "5 Fleets"], ["10", "10 Fleets"], ["15", "15 Fleets"]]

         }
         ), // end of Ext.data.SimpleStore
       displayField: 'name',
       valueField: 'alpha2code',
       selectOnFocus: true,
       mode: 'local',
       typeAhead: true,
       editable: false,
       triggerAction: 'all',
       value: '10',
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

   }





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





              var panel1 = Ext.create('widget.panel',
   {
       width: 500,
       height: 270,
       renderTo: Ext.getBody(),
       layout: 'fit',
       tbar: [cboHrs, cbofleets],
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



        Ext.apply(this, {
            layout: 'fit',
            width: 600,
            height: 300,
            items: {
                xtype: 'chart',
                animate: false,
                shadow: false,
                store: Ext.create('Ext.data.JsonStore', {
                    fields: ['name', 'sp500', 'djia'],
                    data: this.generateData()
                }),
                legend: {
                    position: 'bottom'
                },
                axes: [{
                    type: 'Numeric',
                    position: 'left',
                    fields: ['djia'],
                    title: 'Dow Jones Average',
                    label: {
                        font: '11px Arial'
                    }
                }, {
                    type: 'Numeric',
                    position: 'right',
                    grid: false,
                    fields: ['sp500'],
                    title: 'S&P 500',
                    label: {
                            font: '11px Arial'
                        }
                }],
                series: [{
                    type: 'line',
                    lineWidth: 1,
                    showMarkers: false,
                    fill: true,
                    axis: 'left',
                    xField: 'name',
                    yField: 'djia',
                    style: {
                        'stroke-width': 1
                    }
                }, {
                    type: 'line',
                    lineWidth: 1,
                    showMarkers: false,
                    axis: 'right',
                    xField: 'name',
                    yField: 'sp500',
                    style: {
                        'stroke-width': 1
                    }
                }]
            }
        });

        this.callParent(arguments);
    }
});

