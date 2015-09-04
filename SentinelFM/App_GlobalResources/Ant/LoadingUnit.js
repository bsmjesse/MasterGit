Ext.Loader.setConfig({
    enabled: true
});
Ext.Loader.setPath('Ext.ux', '../extjs/examples/ux');

Ext.require([
    'Ext.grid.*',
    'Ext.data.*',
    'Ext.util.*',
    'Ext.state.*',
    'Ext.form.*',
    'Ext.ux.CheckColumn',
    'Ext.form.field.ComboBox',
    'Ext.form.FieldSet',
    'Ext.tip.QuickTipManager'
]);

Ext.onReady(function () {
    // Define our data model
    // Define the model for a State
    Ext.define('Fleet', {
        extend: 'Ext.data.Model',
        fields: [
            { type: 'string', name: 'FleetId' },
            { type: 'string', name: 'FleetName' }
        ]
    });

    // The data for all states
    var FleetData = [
        { "FleetId": "1", "FleetName": "Fleet 1" },
        { "FleetId": "2", "FleetName": "Fleet 2" },
        { "FleetId": "3", "FleetName": "Fleet 3" }
    ];

    // The data store holding the states; shared by each of the ComboBox examples below
    var FleetStore = Ext.create('Ext.data.Store', {
        model: 'Fleet',
        data: FleetData
    });

    // Simple ComboBox using the data store
    var FleetCombo = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Select a fleet',
        renderTo: 'FleetCombo',
        displayField: 'FleetName',
        width: 250,
        labelWidth: 90,
        store: FleetStore,
        queryMode: 'local',
        typeAhead: true
    });


    var FleetComboDump = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Select a fleet',
        renderTo: 'FleetComboDump',
        displayField: 'FleetName',
        width: 250,
        labelWidth: 90,
        store: FleetStore,
        queryMode: 'local',
        typeAhead: true
    });


    Ext.define('Vehicle', {
        extend: 'Ext.data.Model',
        fields: [
            { type: 'string', name: 'VehicleId' },
            { type: 'string', name: 'VehicleName' }
        ]
    });

    // The data for all states
    var VehicleData = [
        { "VehicleId": "1", "VehicleName": "Vehicle 1" },
        { "VehicleId": "2", "VehicleName": "Vehicle 2" },
        { "VehicleId": "3", "VehicleName": "Vehicle 3" }
    ];

    // The data store holding the states; shared by each of the ComboBox examples below
    var VehicleStore = Ext.create('Ext.data.Store', {
        model: 'Vehicle',
        data: VehicleData
    });

    // Simple ComboBox using the data store
    var VehicleCombo = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Select a vehicle',
        renderTo: 'VehicleCombo',
        displayField: 'VehicleName',
        width: 250,
        labelWidth: 90,
        store: VehicleStore,
        queryMode: 'local',
        typeAhead: true
    });

    var VehicleComboDump = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Select a vehicle',
        renderTo: 'VehicleComboDump',
        displayField: 'VehicleName',
        width: 250,
        labelWidth: 90,
        store: VehicleStore,
        queryMode: 'local',
        typeAhead: true
    });



    //Loading Function
    Ext.define('Loading', {
        extend: 'Ext.data.Model',
        fields: [
            'unit',
            'area',
            'id',
            { name: 'unit', type: 'string' },
            { name: 'area', type: 'string' },
            { name: 'id', type: 'int' }
        ]
    });

    // Generate Loading data
    var LoadingData = [];

    LoadingData.push({
        unit: '2753',
        area: 'area 1'
    });
    LoadingData.push({
        unit: '2754',
        area: 'area 2'
    });

    // create the Data Store
    var LoadingStore = Ext.create('Ext.data.Store', {
        // destroy the store if the grid is destroyed
        autoDestroy: true,
        model: 'Loading',
        proxy: {
            type: 'memory'
        },
        data: LoadingData,
        sorters: [{
            property: 'start',
            direction: 'ASC'
        }]
    });

    var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 1,
        autoCancel: false
    });

    // create the grid and specify what field you want
    // to use for the editor at each column.
    var LoadingGrid = Ext.create('Ext.grid.Panel', {
        store: LoadingStore,
        columns: [{
            header: 'Loading Unit',
            dataIndex: 'unit',
            width: 250,
            editor: {
                // defaults to textfield if no xtype is supplied
                allowBlank: false
            }
        }, {
            header: 'Loading Area',
            dataIndex: 'area',
            width: 250,
            editor: {
                allowBlank: false
            }
        }],
        renderTo: 'loadingGrid',
        width: 550,
        height: 500,
        title: 'Loading Unit Location',
        frame: true,
        tbar: [{
            text: 'Add Loading Location',
            iconCls: 'add',
            handler: function () {
                rowEditing.cancelEdit();

                // Create a record instance through the ModelManager
                var r = Ext.ModelManager.create({
                    unit: '',
                    area: ''
                }, 'Loading');

                LoadingStore.insert(0, r);
                rowEditing.startEdit(0, 0);
            }
        }, {
            itemId: 'removeLoadingLocation',
            text: 'Remove Loading Location',
            iconCls: 'remove',
            handler: function () {
                if (confirm("Are you sure you want to remove?")) {
                    var sm = LoadingGrid.getSelectionModel();
                    rowEditing.cancelEdit();
                    LoadingStore.remove(sm.getSelection());
                    sm.select(0);
                }
            },
            disabled: true
        }, {
            itemId: 'Send',
            text: 'Send',
            iconCls: 'send',
            handler: function () {
                if (confirm("Are you sure you want to send?")) {

                }
            },
            disabled: false
        }],
        plugins: [rowEditing],
        listeners: {
            'selectionchange': function (view, records) {
                LoadingGrid.down('#removeLoadingLocation').setDisabled(!records.length);
            }
        }
    });


    //Dump Function
    Ext.define('Dump', {
        extend: 'Ext.data.Model',
        fields: [
            'Dump',
            'id',
            { name: 'dump', type: 'string' },
            { name: 'id', type: 'int' }
        ]
    });

    // Generate Loading data
    var DumpData = [];

    DumpData.push({
        dump: 'Dump 2753',
        id: '1'
    });

    DumpData.push({
        dump: 'Dump 2754',
        id: '2'
    });

    // create the Data Store
    var DumpStore = Ext.create('Ext.data.Store', {
        // destroy the store if the grid is destroyed
        autoDestroy: true,
        model: 'Dump',
        proxy: {
            type: 'memory'
        },
        data: DumpData,
        sorters: [{
            property: 'start',
            direction: 'ASC'
        }]
    });

    var DumprowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 1,
        autoCancel: false
    });

    // create the grid and specify what field you want
    // to use for the editor at each column.
    var DumpGrid = Ext.create('Ext.grid.Panel', {
        store: DumpStore,
        columns: [{
            header: 'Dump Location',
            dataIndex: 'dump',
            width: 400,
            editor: {
                // defaults to textfield if no xtype is supplied
                allowBlank: false
            }
        }],
        renderTo: 'dumpGrid',
        width: 550,
        height: 500,
        title: 'Dump Location',
        frame: true,
        tbar: [{
            text: 'Add Dump Location',
            iconCls: 'add',
            handler: function () {
                DumprowEditing.cancelEdit();

                // Create a record instance through the ModelManager
                var r = Ext.ModelManager.create({
                    dump: ''
                }, 'Dump');

                DumpStore.insert(0, r);
                DumprowEditing.startEdit(0, 0);
            }
        }, {
            itemId: 'removeDumpLocation',
            text: 'Remove Dump Location',
            iconCls: 'remove',
            handler: function () {
                if (confirm("Are you sure you want to remove?")) {
                    var sm = DumpGrid.getSelectionModel();
                    DumprowEditing.cancelEdit();
                    DumpStore.remove(sm.getSelection());
                    sm.select(0);
                }
            },
            disabled: true
        }, {
            itemId: 'Send',
            text: 'Send',
            iconCls: 'send',
            handler: function () {
                if (confirm("Are you sure you want to send?")) {

                }
            }
        }],
        plugins: [DumprowEditing],
        listeners: {
            'selectionchange': function (view, records) {
                DumpGrid.down('#removeDumpLocation').setDisabled(!records.length);
            }
        }
    });

    var tabs = Ext.create('Ext.tab.Panel', {
        renderTo: 'tabs1',
        width: 600,
        height: 600,
        activeTab: 0,
        defaults: {
            bodyPadding: 10
        },
        items: [DumpGrid],
        listeners:
            {
                tabchange: function (tabPanel, newCard, oldCard, eOpts) {
                    //if (newCard.id == '1') {
                    //    historyByLocation.setValue('1');
                    //}
                    //else {
                    //    historyByLocation.setValue('0');
                    //}

                    if (newCard.id == 'id2') {
                        //Ext.get('dump').
                        //document.getElementById('dump').display = 'block';
                        document.getElementById('dump').style.display = '';
                    }
                }
            }
    });

    // var tabPanel = Ext.getCmp('myTabPanel');
    //var tabToHide = Ext.getCmp('myTab');

});