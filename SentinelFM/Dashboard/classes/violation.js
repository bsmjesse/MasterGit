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


    function renderIdling(val) { return '<a href="idlingDetails.aspx?v=' + val + '" target="_blank" >' + val + '</a>'; } 


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
        },
    //updates a record modified via the form
        updateRecord = function (rec) {
            var name, series, i, l, items, json = [{
                'Name': 'Description',
                'Data': rec.get('description')
            }, {
                'Name': 'Idling Hours',
                'Data': rec.get('idling')
            }, {
                'Name': 'Total Engine Hours',
                'Data': rec.get('engine')
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




    //create a grid that will list the dataset items.
    var gridPanel = Ext.create('Ext.grid.Panel', {
        id: 'violation-form',
        store: store2,
        columnLines: true,
        columns: [
            {
                id: 'description',
                text: 'Vehicle',
                width: 100,
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
                text: 'Ext.Braking',
                width: 70,
                sortable: true,
                dataIndex: 'extBraking'
            }
            ,
            {
                text: 'Ext.Acc',
                width: 60,
                sortable: true,
                dataIndex: 'extAcc'


            },

            {
                text: 'Total',
                width: 60,
                sortable: true,
                dataIndex: 'Total'

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

        renderTo: Ext.getBody(),
        layout: 'fit',
        //        tbar: [{
        //                     xtype: 'combo',
        //                     id: 'topCmb',
        //                     fieldLabel: 'Fleets',
        //                     labelWidth: 50,
        //                     hiddenName: 'ddi_top',
        //                     //emptyText: 'Top 10 ',
        //                     store: store3, 
        //                     displayField: 'FleetName',
        //                     valueField: 'FleetId',
        //                     selectOnFocus: true,
        //                    // mode: 'local',
        //                     typeAhead: true,
        //                     editable: false,
        //                     triggerAction: 'all'//,
        //                     //value: '10',
        ////                     listeners:
        ////                    { select: { fn: function (combo, value) {

        ////                        alert(this.value);
        ////                    }
        ////                    }
        ////                    }
        //                 }

        //        ],
        items: {
            xtype: 'chart',
            animate: true,
            shadow: true,
            store: store1,
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

    var gp = Ext.getCmp('violation-form');
});
