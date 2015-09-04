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
    'Ext.tab.*',
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
        id: 'idling-form',
       // renderTo: Ext.getBody(),
        layout: 'fit',
        store: store2,
        columnLines: true,
        columns: [
            {
                id: 'description',
                text: 'Vehicle',
                width: 120,
                sortable: true,
                dataIndex: 'description'
                
            },
            {
                text: 'Engine Hrs.',
                width: 75,
                sortable: true,
                dataIndex: 'engine'
            }
            ,
            {
                text: 'Idling Hrs.',
                width: 75,
                sortable: true,
                dataIndex: 'idling'
               

            },

             {
                 text: 'Perc',
                 width: 55,
                 sortable: true,
                 dataIndex: 'perc',
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
        width: 300,
        height: 250,
       
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
            id: 'chartCmp',
            animate: true,
            store: store1,
            shadow: true,
            
            legend: {
                position: 'right'
            },
            insetPadding: 20,
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
                        margin: 2
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

    /*
    var gridForm = Ext.create('Ext.form.Panel', {
        frame: true,
        bodyPadding: 0,
        width: 420,
        height: 650,

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
                margin: '0 0 1 0',
                items: [barChart]
            },
            {

                layout: { type: 'hbox', align: 'stretch' },
                flex: 1,
                border: false,
                bodyStyle: 'background-color: transparent',

                items: [gridPanel]
            }],
        renderTo: bd
    });
    */



    var tabs = Ext.createWidget('tabpanel', {
        renderTo: 'tabs1',
        width: 303,
        activeTab: 0,
        defaults: {
            bodyPadding: 0
        },
        items: [{
            title: 'Graph',
            items: [barChart]
        }, {
           
            title: 'Details',
            items: [gridPanel]
        
        }]
    });



   //var gp = Ext.getCmp('idling-form');
});
