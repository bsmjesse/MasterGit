/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
Ext.define('Ext.app.PiePortlet', {

    extend: 'Ext.panel.Panel',
    alias: 'widget.pieportlet',


   requires: [
        'Ext.data.JsonStore',
        'Ext.chart.*',
        'Ext.layout.container.Fit',
    ],


Ext.onReady(function () {
    store1.loadData(generateData(6, 20));

    var donut = false,
        panel1 = Ext.create('widget.panel', {
        width: 800,
        height: 600,
        title: 'Semester Trends',
        renderTo: Ext.getBody(),
        layout: 'fit',
        tbar: [{
            text: 'Reload Data',
            handler: function() {
                store1.loadData(generateData(6, 20));
            }
        }, {
            enableToggle: true,
            pressed: false,
            text: 'Donut',
            toggleHandler: function(btn, pressed) {
                var chart = Ext.getCmp('chartCmp');
                chart.series.first().donut = pressed ? 35 : false;
                chart.refresh();
            }
        }],
        items: {
            xtype: 'chart',
            id: 'chartCmp',
            animate: true,
            store: store1,
            shadow: true,
            legend: {
                position: 'right'
            },
            insetPadding: 60,
            theme: 'Base:gradients',
            series: [{
                type: 'pie',
                field: 'data',
                showInLegend: true,
                donut: donut,
                tips: {
                  trackMouse: true,
                  width: 140,
                  height: 28,
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
        }
    });
});
   
});

