/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/

//--Start bar
/*
Ext.require('Ext.chart.*');
Ext.require('Ext.layout.container.Fit');

Ext.onReady(function () {
    var panel1 = Ext.create('widget.panel', {
        width: 800,
        height: 400,
        title: 'Maintenace DTC codes',
        renderTo: Ext.getBody(),
        layout: 'fit',
      
        items: {
            xtype: 'chart',
            animate: true,
            shadow: true,
            store: store1,
            axes: [{
                type: 'Numeric',
                position: 'left',
                fields: ['data'],
                title: '# of DTC',
                grid: true,
                minimum: 0
            }, {
                type: 'Category',
                position: 'bottom',
                fields: ['name'],
                title: 'Vehicles',
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
                    renderer: function(storeItem, item) {
                        this.setTitle(storeItem.get('name') + '<br />' + storeItem.get('data'));
                    }
                },
                style: {
                    fill: '#38B8BF'
                }
            }]
        }
    });
});

*/
// End bar


Ext.require('Ext.chart.*');
Ext.require('Ext.layout.container.Fit');




Ext.onReady(function () {


    var dateMenu = Ext.create('Ext.menu.DatePicker', {
        handler: function (dp, date) {
            Ext.Msg.alert('Date Selected', 'You choose {0}.', Ext.Date.format(date, 'M j, Y'));

        }
    });


    var panel1 = Ext.create('widget.panel', {
        width: 500,
        height: 270,
        renderTo: Ext.getBody(),
        layout: 'fit',
//        tbar: [{
//                xtype: 'combo',
//                id: 'daysCmb',
//                fieldLabel: 'Days',
//                labelWidth: 50,
//                hiddenName: 'ddi_days',
//                emptyText: 'Last 7 days',
//                store:
//          new Ext.data.SimpleStore({
//              fields: ['alpha2code', 'name'],
//              data: [
//              ["7", "7 Days"], ["3", "3 Day"], ["1", "Yesterday"]]
//          }), // end of Ext.data.SimpleStore
//                displayField: 'name',
//                valueField: 'alpha2code',
//                selectOnFocus: true,
//                mode: 'local',
//                typeAhead: true,
//                editable: false,
//                triggerAction: 'all',
//                value: '7',
//                listeners:
//                    { select: { fn: function (combo, value) {
//                        
//                                 //alert(this.value);
//                         }
//                    }
//            }
//                 },



//                 {
//                     xtype: 'combo',
//                     id: 'topCmb',
//                     fieldLabel: 'Fleets',
//                     labelWidth: 50,
//                     hiddenName: 'ddi_top',
//                     emptyText: 'Top 10 ',
//                     store:
//          new Ext.data.SimpleStore({
//              fields: ['alpha2code', 'name'],
//              data: [
//              ["10", "Top 10"], ["5", "Top 5"]]
//          }), // end of Ext.data.SimpleStore
//                     displayField: 'name',
//                     valueField: 'alpha2code',
//                     selectOnFocus: true,
//                     mode: 'local',
//                     typeAhead: true,
//                     editable: false,
//                     triggerAction: 'all',
//                     value: '10',
//                     listeners:
//                    { select: { fn: function (combo, value) {

//                       // alert(this.value);
//                    }
//                    }
//                    }
//                 }
//        
//        ],
        items: {
            xtype: 'chart',
            animate: true,
            shadow: true,
            store: store1,
            axes: [{
                type: 'Numeric',
                position: 'bottom',
                fields: ['data'],
                title: '# of Alarms',
                grid: true,
                minimum: 0
            }, {
                type: 'Category',
                position: 'left',
                fields: ['name'],
                title: 'Fleet'

            }],
            series: [{
                type: 'bar',
                axis: 'bottom',
                highlight: true,
                tips: {
                    trackMouse: true,
                    width: 140,
                    height: 38,
                    renderer: function (storeItem, item) {
                        this.setTitle(storeItem.get('name') + ': ' + storeItem.get('data') + ' views');
                    }
                },
                label: {
                    display: 'insideEnd',
                    field: 'data',
                    renderer: Ext.util.Format.numberRenderer('0'),
                    orientation: 'horizontal',
                    color: '#333',
                    'text-anchor': 'middle'
                },
                xField: 'name',
                yField: ['data']
            }]
        }
    });
});