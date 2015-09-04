/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
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

    var bd = Ext.getBody(),
        form = false,
        rec = false,
        selectedStoreItem = false,
    //performs the highlight of an item in the bar series
        selectItem = function (storeItem) {
            var name = storeItem.get('VehicleDescription'),
                series = barChart.series.get(0),
                i, items, l;

            series.highlight = true;
            series.unHighlightItem();
            series.cleanHighlights();
            for (i = 0, items = series.items, l = items.length; i < l; i++) {
                if (name == items[i].storeItem.get('VehicleDescription')) {
                    selectedStoreItem = items[i].storeItem;
                    series.highlightItem(items[i]);
                    break;
                }
            }
            series.highlight = false;
        },
    //updates a record modified via the form
        updateRecord = function (rec) {
            var name, series, i, l, items, json = [{
                'Name': 'Description',
                'Data': rec.get('Description')
            }, {
                'Name': 'ServicePerc',
                'Data': rec.get('ServicePerc')
            }, {
                'Name': 'Growth %',
                'Data': rec.get('growth %')
            }, {
                'Name': 'Product %',
                'Data': rec.get('product %')
            }, {
                'Name': 'Market %',
                'Data': rec.get('market %')
            }];

            selectItem(rec);
        },
        createListeners = function () {
            return {
                // buffer so we don't refire while the user is still typing
                buffer: 200,
                change: function (field, newValue, oldValue, listener) {
                    form.updateRecord(rec);
                    updateRecord(rec);
                }
            };
        };

    //    // sample static data for the store
    //    var myData = [
    //        ['3m Co'],
    //        ['Alcoa Inc'],
    //        ['Altria Group Inc'],
    //        ['American Express Company'],
    //        ['American International Group, Inc.'],
    //        ['AT&T Inc'],
    //        ['Boeing Co.'],
    //        ['Caterpillar Inc.'],
    //        ['Citigroup, Inc.'],
    //        ['E.I. du Pont de Nemours and Company'],
    //        ['Exxon Mobil Corp'],
    //        ['General Electric Company'],
    //        ['General Motors Corporation'],
    //        ['Hewlett-Packard Co'],
    //        ['Honeywell Intl Inc'],
    //        ['Intel Corporation'],
    //        ['International Business Machines'],
    //        ['Johnson & Johnson'],
    //        ['JP Morgan & Chase & Co'],
    //        ['McDonald\'s Corporation'],
    //        ['Merck & Co., Inc.'],
    //        ['Microsoft Corporation'],
    //        ['Pfizer Inc'],
    //        ['The Coca-Cola Company'],
    //        ['The Home Depot, Inc.'],
    //        ['The Procter & Gamble Company'],
    //        ['United Technologies Corporation'],
    //        ['Verizon Communications'],
    //        ['Wal-Mart Stores, Inc.']
    //    ];
    //    
    //    for (var i = 0, l = myData.length, rand = Math.random; i < l; i++) {
    //        var data = myData[i];
    //        data[1] = ((rand() * 10000) >> 0) / 100;
    //        data[2] = ((rand() * 10000) >> 0) / 100;
    //        data[3] = ((rand() * 10000) >> 0) / 100;
    //        data[4] = ((rand() * 10000) >> 0) / 100;
    //        data[5] = ((rand() * 10000) >> 0) / 100;
    //    }




    //     var myData ='<%=strGridData %>'

    //    //create data store to be shared among the grid and bar series.
    //    var ds = Ext.create('Ext.data.ArrayStore', {
    //        fields: [
    //            {name: 'VehicleDescription'},
    //            {name: 'Description'},
    //            {name: 'ServicePerc', type: 'float'}
    //       
    //        ],
    //        data: myData
    //    });





    //create a grid that will list the dataset items.
    var gridPanel = Ext.create('Ext.grid.Panel', {
        id: 'maintenance-form',
        store: store2,
        columnLines: true,
        columns: [
            {
                id: 'VehicleDescription',
                text: 'Vehicle',
                width: 110,
                sortable: true,
                dataIndex: 'VehicleDescription'
            },
            {
                text: 'Maintenance',
                width: 190,
                sortable: true,
                dataIndex: 'Description'


            },
            {
                text: '%',
                width: 60,
                sortable: true,
                dataIndex: 'ServicePerc',
                renderer: perc
            }
        ],

        listeners: {
            selectionchange: function (model, records) {
                var json, name, i, l, items, series, fields;
                if (records[0]) {
                    rec = records[0];
                    form = form || this.up('form').getForm();
                    fields = form.getFields();
                    // prevent change events from firing
                    fields.each(function (field) {
                        field.suspendEvents();
                    });
                    form.loadRecord(rec);

                    fields.each(function (field) {
                        field.resumeEvents();
                    });
                }
            }
        }
    });

    //create a bar series to be at the top of the panel.
    var barChart = Ext.create('widget.panel', {
        width: 400,
        height: 320,
        frame: false,
        renderTo: Ext.getBody(),
        layout: 'fit',

        items: {
            xtype: 'chart',
            id: 'chartCmp',
            animate: true,
            store: store1,
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

                        if (obj.storeItem.data['name'] == 'Over 90') {
                            store2.filter('StatusId', 1);
                        }
                        if (obj.storeItem.data['name'] == 'Over 95') {
                            store2.filter('StatusId', 3);
                        }
                        if (obj.storeItem.data['name'] == 'Overdue') {
                            store2.filter('StatusId',4);
                        }
                    }
                }

            }]
        }
    });

    /*
    * Here is where we create the Form
    */
    var gridForm = Ext.create('Ext.form.Panel', {
        frame: false ,
        bodyPadding: 0,
        width: 380,
        height: 700,
        split: true,
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
                border: false,
                margin: '0 0 0 0',
                items: [barChart]
            },
            {

                layout: { type: 'hbox', align: 'stretch' },
                flex: 1,
                border: false,
                margin: '0 0 0 0',
                bodyStyle: 'background-color: transparent',

                items: [gridPanel]
            }],
        renderTo: bd
    });

    var gp = Ext.getCmp('maintenance-form');
});
