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

Ext.define('OverlayVisibilityObject', {
    extend: 'Ext.data.Model',
    fields: ['name', 'layerId', 'visibility']
});

Ext.define('MapOverlayObject', {
    extend: 'Ext.data.Model',
    fields: ['name', 'layerId']
});

Ext.onReady(function () {
    var defaultOverlaysData = eval($('#defaultOverlaysHidden').val());
    var defaultAvailableOverlaysData = eval($('#defaultAvailableOverlaysHidden').val());

    var premiumOverlaysData = eval($('#premiumOverlaysHidden').val());
    var premiumAvailableOverlaysData = eval($('#premiumAvailableOverlaysHidden').val());

    var overlayVisibilityData = eval($('#overlayVisibilityDataHidden').val());

    // create the data store
    var defaultAvailableGridStore = Ext.create('Ext.data.Store', {
        model: 'MapOverlayObject',
        data: defaultAvailableOverlaysData
    });

    var premiumAvailableGridStore = Ext.create('Ext.data.Store', {
        model: 'MapOverlayObject',
        data: premiumAvailableOverlaysData
    });

    var overlayVisibilityGridStore = Ext.create('Ext.data.Store', {
        model: 'OverlayVisibilityObject',
        data: overlayVisibilityData
    });


    // Column Model shortcut array
    var columns = [
        { text: "Overlay Name", flex: 1, sortable: true, dataIndex: 'name' }
    ];

    // declare the source Grid
    var defaultAvailableGrid = Ext.create('Ext.grid.Panel', {
        multiSelect: true,
        viewConfig: {
            plugins: AllOrganizationOverlayLayersSettingreadonly ? null : {
                ptype: 'gridviewdragdrop',
                dragGroup: 'defaultAvailableGridDDGroup',
                dropGroup: 'defaultOverlaysGridDDGroup'
            },
            listeners: AllOrganizationOverlayLayersSettingreadonly ? null : {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    defaultOverlaysGridStore.insert(defaultOverlaysGridStore.getCount(), record);
                    defaultAvailableGridStore.remove(record);
                }
            }
        },
        store: defaultAvailableGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Available Overlays',
        margins: '0 2 0 0',
        hideHeaders: true
    });

    var premiumAvailableGrid = Ext.create('Ext.grid.Panel', {
        multiSelect: true,
        viewConfig: {
            plugins: {
                ptype: 'gridviewdragdrop',
                dragGroup: 'premiumAvailableGridDDGroup',
                dropGroup: 'premiumOverlaysGridDDGroup'
            },
            listeners: {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    premiumOverlaysGridStore.insert(premiumOverlaysGridStore.getCount(), record);
                    premiumAvailableGridStore.remove(record);
                }
            }
        },
        store: premiumAvailableGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Available Overlays',
        margins: '0 2 0 0',
        hideHeaders: true
    });

    var defaultOverlaysGridStore = Ext.create('Ext.data.Store', {
        model: 'MapOverlayObject',
        data: defaultOverlaysData
    });

    var premiumOverlaysGridStore = Ext.create('Ext.data.Store', {
        model: 'MapOverlayObject',
        data: premiumOverlaysData
    });

    // create the destination Grid
    var defaultOverlaysGrid = Ext.create('Ext.grid.Panel', {
        viewConfig: {
            plugins: AllOrganizationOverlayLayersSettingreadonly ? null : {
                ptype: 'gridviewdragdrop',
                dragGroup: 'defaultOverlaysGridDDGroup',
                dropGroup: 'defaultAvailableGridDDGroup'
            },
            listeners: AllOrganizationOverlayLayersSettingreadonly ? null : {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    defaultAvailableGridStore.insert(defaultAvailableGridStore.getCount(), record);
                    defaultOverlaysGridStore.remove(record);
                }
            }
        },
        store: defaultOverlaysGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Default Subscribed Overlays',
        margins: '0 0 0 3',
        hideHeaders: true
    });


    // create the destination Grid
    var premiumOverlaysGrid = Ext.create('Ext.grid.Panel', {
        viewConfig: {
            plugins: {
                ptype: 'gridviewdragdrop',
                dragGroup: 'premiumOverlaysGridDDGroup',
                dropGroup: 'premiumAvailableGridDDGroup'
            },
            listeners: {
                drop: function (node, data, dropRec, dropPosition) {
                    var dropOn = dropRec ? ' ' + dropPosition + ' ' + dropRec.get('name') : ' on empty view';
                },
                itemdblclick: function (thisview, record, item, index, e, eOpts) {
                    premiumAvailableGridStore.insert(premiumAvailableGridStore.getCount(), record);
                    premiumOverlaysGridStore.remove(record);
                }
            }
        },
        store: premiumOverlaysGridStore,
        columns: columns,
        stripeRows: true,
        title: 'Premium Subscribed Overlays',
        margins: '0 0 0 3',
        hideHeaders: true
    });

    //Simple 'border layout' panel to house both grids
    var displayPanel = Ext.create('Ext.Panel', {
        width: 650,
        height: 250,
        collapsible: true,
        title: '<div class="panelTitle">All Organizations Overlay Layers Setting (Default)' + (AllOrganizationOverlayLayersSettingreadonly ? ' - Read Only':'') + '</div><div class="panelSubTitle">Set/Unset default overlay layers for <span style="font-weight:bold;">ALL</span> clients by drag-and-drop or double clicking the layers\' name.</div>',
        layout: {
            type: 'hbox',
            align: 'stretch',
            padding: 5
        },
        renderTo: 'defaultOverlayPanel',
        defaults: { flex: 1 }, //auto stretch
        items: [
            defaultAvailableGrid,
            defaultOverlaysGrid
        ]
    });

    //Simple 'border layout' panel to house both grids
    var premiumDisplayPanel = Ext.create('Ext.Panel', {
        width: 650,
        height: 250,
        collapsible: true,
        title: '<div class="panelTitle">Premium Overlay Layers Setting</div><div class="panelSubTitle">Set/Unset premium overlay layers for this client only by drag-and-drop or double clicking the layers\' name.</div>',
        layout: {
            type: 'hbox',
            align: 'stretch',
            padding: 5
        },
        renderTo: 'premiumOverlayPanel',
        defaults: { flex: 1 }, //auto stretch
        items: [
            premiumAvailableGrid,
            premiumOverlaysGrid
        ]
    });

    var visibilitygrid = Ext.create('Ext.grid.Panel',
   {
       id: 'visibilitygrid',
       renderTo: 'overlayVisibilityPanel',
       enableColumnHide: true,
       title: '<div class="panelTitle">Overlay Visibility Setting</div><div class="panelSubTitle">Check On/Off to set if the overlay is visible. This setting is for <span style="font-weight:bold;">ALL</span> clients.</div>',
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       enableSorting: true,
       stateful: false,
       closable: false,
       columnLines: true,
       autoHeight: true,
       store: overlayVisibilityGridStore,
       collapsible: true,
       animCollapse: true,
       split: true,
       stateId: 'stateVGrid',
       width: 650,
       viewConfig:
      {
          emptyText: 'No overlays to display',
          useMsg: false
      }
      ,
       columns: [
      {
          text: 'Overlay Name',
          align: 'left',
          width: 320,
          dataIndex: 'name',
          filterable: true,
          sortable: true,
          hidden: false
      },
      {
          text: 'Visibility',
          align: 'left',
          width: 320,
          renderer: function (value, p, record) {
              var c = '';
              if(record.data['visibility']=='True') c=' checked ';
              var s = '<input type="Checkbox" id="visibility' + record.data['layerId'] + '" class="visibilitychk" layerId="' + record.data['layerId'] + '" ' + c + ' />';
              return s;
          },
          dataIndex: 'visibility',
          filterable: true,
          sortable: true,
          hidden: false,
          flex: 1
      }
      ]
   });

    Ext.create('Ext.Button', {
        renderTo: 'overlayVisibilityPanel',
        text: 'Save',
        cls: 'cmbfonts',
        margin: '10 0 30 0',
        handler: function () {
            try {
                SaveOverlaysSetting(defaultOverlaysGrid, premiumOverlaysGrid);
            }
            catch (err) {
            }
        }
    });
});

function SaveOverlaysSetting(defaultOverlaysGrid, premiumOverlaysGrid) {
    var selectedDefaultOverlays = "";
    var selectedPremiumOverlays = "";
    var visibleOverlays = "";

    if (defaultOverlaysGrid.getStore().getCount() > 0) {

        defaultOverlaysGrid.getStore().each(function (record) {
            selectedDefaultOverlays += record.data.layerId + ',';
        });
        selectedDefaultOverlays = selectedDefaultOverlays.substring(0, selectedDefaultOverlays.length - 1);        
    }
    if (premiumOverlaysGrid.getStore().getCount() > 0) {

        premiumOverlaysGrid.getStore().each(function (record) {
            selectedPremiumOverlays += record.data.layerId + ',';
        });
        selectedPremiumOverlays = selectedPremiumOverlays.substring(0, selectedPremiumOverlays.length - 1);
    }

    //if (selectedDefaultOverlays == "") selectedDefaultOverlays = "No default Overlay selected";
    //if (selectedPremiumOverlays == "") selectedPremiumOverlays = "No premium Overlay selected";

    $('#selectedDefaultOverlays').val(selectedDefaultOverlays);
    $('#selectedPremiumOverlays').val(selectedPremiumOverlays);

    $('.visibilitychk').each(function () {
        if ($(this).is(':checked'))
            visibleOverlays += $(this).attr('layerId') + ',';
    });

    if (visibleOverlays != "")
        visibleOverlays = visibleOverlays.substring(0, visibleOverlays.length - 1);
    else
        visibleOverlays = '-1';

    $('#selectedVisibleOverlays').val(visibleOverlays);
    setTargetname('overlaysubscription');

    $('#formMapsubscription').submit();
}

function setTargetname(n) {
    $('#targetName').val(n);
}