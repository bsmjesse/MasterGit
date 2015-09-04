/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
Ext.require([
    'Ext.grid.*',
    'Ext.data.*',
    'Ext.dd.*'
]);

Ext.define('MapLayerObject', {
    extend: 'Ext.data.Model',
    fields: ['name', 'layerId', 'description']
});

Ext.onReady(function () {
    var defaultLayersData = eval($('#defaultLayersHidden').val());
    var defaultAvailableLayersData = eval($('#defaultAvailableLayersHidden').val());

    var premiumLayersData = eval($('#premiumLayersHidden').val());
    var premiumAvailableLayersData = eval($('#premiumAvailableLayersHidden').val());

    // create the data store
    var defaultAvailableGridStore = Ext.create('Ext.data.Store', {
        model: 'MapLayerObject',
        data: defaultAvailableLayersData
    });

    var premiumAvailableGridStore = Ext.create('Ext.data.Store', {
        model: 'MapLayerObject',
        data: premiumAvailableLayersData
    });


    // Column Model shortcut array
    var columns = [
        { text: "Layer Name", flex: 1, sortable: true, dataIndex: 'description' }
    ];

    // declare the source Grid
    var defaultAvailableGrid = Ext.create('Ext.grid.Panel', {
        multiSelect: true,
        viewConfig: {
            plugins: {
                ptype: 'gridviewdragdrop',
                dragGroup: 'defaultAvailableGridDDGroup',
                dropGroup: 'defaultLayersGridDDGroup'
            },
            listeners: {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    defaultLayersGridStore.insert(defaultLayersGridStore.getCount(), record);
                    defaultAvailableGridStore.remove(record);
                }
            }
        },
        store: defaultAvailableGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Available Layers',
        margins: '0 2 0 0',
        hideHeaders: true
    });

    var premiumAvailableGrid = Ext.create('Ext.grid.Panel', {
        multiSelect: true,
        viewConfig: {
            plugins: {
                ptype: 'gridviewdragdrop',
                dragGroup: 'premiumAvailableGridDDGroup',
                dropGroup: 'premiumLayersGridDDGroup'
            },
            listeners: {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    premiumLayersGridStore.insert(premiumLayersGridStore.getCount(), record);
                    premiumAvailableGridStore.remove(record);
                }
            }
        },
        store: premiumAvailableGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Available Layers',
        margins: '0 2 0 0',
        hideHeaders: true
    });

    var defaultLayersGridStore = Ext.create('Ext.data.Store', {
        model: 'MapLayerObject',
        data: defaultLayersData
    });

    var premiumLayersGridStore = Ext.create('Ext.data.Store', {
        model: 'MapLayerObject',
        data: premiumLayersData
    });

    // create the destination Grid
    var defaultLayersGrid = Ext.create('Ext.grid.Panel', {
        viewConfig: {
            plugins: {
                ptype: 'gridviewdragdrop',
                dragGroup: 'defaultLayersGridDDGroup',
                dropGroup: 'defaultAvailableGridDDGroup'
            },
            listeners: {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    defaultAvailableGridStore.insert(defaultAvailableGridStore.getCount(), record);
                    defaultLayersGridStore.remove(record);
                }
            }
        },
        store: defaultLayersGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Default Subscribed Layers',
        margins: '0 0 0 3',
        hideHeaders: true
    });


    // create the destination Grid
    var premiumLayersGrid = Ext.create('Ext.grid.Panel', {
        viewConfig: {
            plugins: {
                ptype: 'gridviewdragdrop',
                dragGroup: 'premiumLayersGridDDGroup',
                dropGroup: 'premiumAvailableGridDDGroup'
            },
            listeners: {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    premiumAvailableGridStore.insert(premiumAvailableGridStore.getCount(), record);
                    premiumLayersGridStore.remove(record);
                }
            }
        },
        store: premiumLayersGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Premium Subscribed Layers',
        margins: '0 0 0 3',
        hideHeaders: true
    });

    //Simple 'border layout' panel to house both grids
    var displayPanel = Ext.create('Ext.Panel', {
        width: 650,
        height: 250,
        collapsible: true,
        title: '<div class="panelTitle">All Organizations Base Layers Setting (Default)</div><div class="panelSubTitle">Set/Unset default base layers for <span style="font-weight:bold;">ALL</span> clients by drag-and-drop or double clicking the layers\' name.</div>',
        layout: {
            type: 'hbox',
            align: 'stretch',
            padding: 5
        },
        renderTo: 'defaultLayerPanel',
        defaults: { flex: 1 }, //auto stretch
        items: [
            defaultAvailableGrid,
            defaultLayersGrid
        ]
    });

    //Simple 'border layout' panel to house both grids
    var premiumDisplayPanel = Ext.create('Ext.Panel', {
        width: 650,
        height: 250,
        collapsible: true,
        title: '<div class="panelTitle">Premium Base Layers Setting</div><div class="panelSubTitle">Set/Unset premium base layers for this client only by drag-and-drop or double clicking the layers\' name.</div>',
        layout: {
            type: 'hbox',
            align: 'stretch',
            padding: 5
        },
        renderTo: 'premiumLayerPanel',
        defaults: { flex: 1 }, //auto stretch
        items: [
            premiumAvailableGrid,
            premiumLayersGrid
        ]
    });

    var textVehicleGridPageSize = Ext.create('Ext.form.field.Number', {
        anchor: '100%',
        name: 'vehicleGridPageSize',
        fieldLabel: 'Vehicle Grid',
        value: VehicleGridPageSize,
        maxValue: 1000,
        minValue: 10,
        height: 30,
        maxHeight: 30,
        style: 'margin:20px 0 0 30px;',
        labelWidth: 70,
        labelStyle: 'margin: 5px 0 0 0;'
    });

    var textHistoryGridPageSize = Ext.create('Ext.form.field.Number', {
        anchor: '100%',
        name: 'historyGridPageSize',
        fieldLabel: 'History Grid',
        value: HistoryGridPageSize,
        maxValue: 2000,
        minValue: 10,
        height: 30,
        maxHeight: 30,
        style: 'margin:20px 0 0 30px;',
        labelWidth: 70,
        labelStyle: 'margin: 5px 0 0 0;'
    });

    var textHistoryGridNormalPageSize = Ext.create('Ext.form.field.Number', {
        anchor: '100%',
        name: 'historyGridNormalPageSize',
        fieldLabel: 'History Grid (Other Browsers)',
        value: HistoryGridNormalPageSize,
        maxValue: 2000,
        minValue: 10,
        height: 30,
        maxHeight: 30,
        style: 'margin:20px 0 0 30px;',
        labelWidth: 180,
        labelStyle: 'margin: 5px 0 0 0;'
    });

    var gridPageSizePanel = Ext.create('Ext.Panel', {
        width: 650,
        height: 160,
        collapsible: true,
        title: '<div class="panelTitle">Grid PageSize Setting</div><div class="panelSubTitle">Set PageSize</div>',
        layout: {
            type: 'column',
            align: 'stretch',
            padding: 5
        },
        renderTo: 'gridPageSize',
        defaults: { flex: 0 }, //auto stretch
        items: [
            textVehicleGridPageSize,
            textHistoryGridPageSize//,
            //textHistoryGridNormalPageSize
        ]
    });

    Ext.create('Ext.Button', {
        renderTo: 'btnSave',
        text: 'Save',
        cls: 'cmbfonts',
        margin: '10 0 0 0',
        handler: function () {
            try {
                if (!textVehicleGridPageSize.isValid() || !textHistoryGridPageSize.isValid()) {
                    return;
                }
                SaveLayersSetting(defaultLayersGrid, premiumLayersGrid);
            }
            catch (err) {
            }
        }
    });
});

function SaveLayersSetting(defaultLayersGrid, premiumLayersGrid) {
    var selectedDefaultLayers = "";
    var selectedPremiumLayers = "";    
    
    if (defaultLayersGrid.getStore().getCount() > 0) {
        
        defaultLayersGrid.getStore().each(function (record) {
            selectedDefaultLayers += record.data.layerId + ',';
        });
        selectedDefaultLayers = selectedDefaultLayers.substring(0, selectedDefaultLayers.length - 1);        
    }
    if (premiumLayersGrid.getStore().getCount() > 0) {

        premiumLayersGrid.getStore().each(function (record) {
            selectedPremiumLayers += record.data.layerId + ',';
        });
        selectedPremiumLayers = selectedPremiumLayers.substring(0, selectedPremiumLayers.length - 1);
    }

    //if (selectedDefaultLayers == "") selectedDefaultLayers = "No default layer selected";
    //if (selectedPremiumLayers == "") selectedPremiumLayers = "No premium layer selected";

    $('#selectedDefaultLayers').val(selectedDefaultLayers);
    $('#selectedPremiumLayers').val(selectedPremiumLayers);

    setTargetname('mapsubscription');

    $('#formMapsubscription').submit();
}

function setTargetname(n) {
    $('#targetName').val(n);
}

