/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/


Ext.require('Ext.chart.*');
Ext.require('Ext.layout.container.Fit');

Ext.onReady(function () {
    var panel1 = Ext.create('widget.panel', {
        width: 370,
        height: 340,
        
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
                title: '# of Vehicles',
                grid: true,
                minimum: 0
            }, {
                type: 'Category',
                position: 'bottom',
                fields: ['name'],
                title: 'HeartBeat',
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



