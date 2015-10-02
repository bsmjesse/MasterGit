/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
Ext.Loader.setConfig(
    {
        enabled: true
    });
Ext.Loader.setPath('Ext.ux', '../extjs/examples/ux');
Ext.require([
    'Ext.grid.*',
    'Ext.ux.grid.FiltersFeature',
    'Ext.data.*',
    'Ext.dd.*'
]);

Ext.onReady(function () {
    var vehicleEmailFilters =
    {
        ftype: 'filters',
        local: true,
        filters: [
            {
                type: 'string',
                dataIndex: 'LicensePlate'
            },
            {
                type: 'string',
                dataIndex: 'Description'
            },
            {
                type: 'string',
                dataIndex: 'Email'
            },
            {
                type: 'string',
                dataIndex: 'VinNum'
            },
            {
                type: 'int',
                dataIndex: 'BoxId'
            }
        ]
    };

    var costCenterEmailFilters =
    {
        ftype: 'filters',
        local: true,
        filters: [
            {
                type: 'string',
                dataIndex: 'FleetName'
            },
            {
                type: 'string',
                dataIndex: 'Email'
            }
        ]
    };

    var scheduledReportsFilters =
    {
        ftype: 'filters',
        local: true,
        filters: [
            {
                type: 'int',
                dataIndex: 'ReportID'
            },
            {
                type: 'string',
                dataIndex: 'GuiName'
            },
            {
                type: 'date',
                dataIndex: 'DateFrom'
            },
            {
                type: 'date',
                dataIndex: 'DateTo'
            },
            {
                type: 'string',
                dataIndex: 'VehicleDescription'
            },
            {
                type: 'string',
                dataIndex: 'FleetName'
            }
            ,
            {
                type: 'string',
                dataIndex: 'Status'
            },
            {
                type: 'string',
                dataIndex: 'Email'
            }

        ]
    };

    var textEmail = Ext.create('Ext.form.field.Text', {
        anchor: '100%',
        name: 'Email',
        vtype: 'email',
        fieldLabel: 'Email',
        allowBlank: false,
        validateOnChange: false,
        validateOnBlur: false,
        height: 30,
        maxHeight: 30,
        width: 250,
        style: 'margin:10px 0 0 30px;',
        labelWidth: 30,
        labelStyle: 'margin: 5px 0 0 0;',
       
        listeners: {
            specialkey: function (field, event, options) {
                try {
                    if (event.getKey() == event.ENTER) {

                        loadingMask.show();
                        vehicleEmailStoreLoading = true;
                        costCenterEmailStoreLoading = true;
                        scheduledReportsStoreLoading = true;
                        costCenterEmailStore.load(
                            {
                                params:
                                {
                                    st: 'GetCostCenterFleetEmailsByEmail',
                                    email: textEmail.getValue()
                                    //,fleetID: selFleet
                                    //,start: 0
                                    //,limit: pagesize
                                },
                                callback: function (records, operation, success) {
                                    loadingMask.hide();
                                    if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                        Ext.Msg.alert("Warning", operation.resultSet.message);
                                        if (records == null)
                                            costCenterEmailStore.loadData([], false);
                                    }
                                }
                            });

                        scheduledReportsStore.load(
                            {
                                params:
                                 {
                                     st: 'GetScheduledReportsByEmail',
                                     email: textEmail.getValue()
                                     //,fleetID: selFleet
                                     //,start: 0
                                     //,limit: pagesize
                                 },
                                callback: function (records, operation, success) {
                                    loadingMask.hide();
                                    if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                        Ext.Msg.alert("Warning", operation.resultSet.message);
                                        if (records == null)
                                            scheduledReportsStore.loadData([], false);
                                    }
                                }
                            });

                        vehicleEmailStore.load(
                          {
                              params:
                               {
                                   st: 'GetVehicleEmailsByEmail',
                                   email: textEmail.getValue()
                                   //,fleetID: selFleet
                                   //,start: 0
                                   //,limit: pagesize
                               },
                              callback: function (records, operation, success) {
                                  loadingMask.hide();
                                  if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                      Ext.Msg.alert("Warning", operation.resultSet.message);
                                      if (records == null)
                                          vehicleEmailStore.loadData([], false);
                                  }
                              }
                          });



                    }
                }
                catch (err) {
                    alert(err);
                }
            }
        }
    });

    var vehicleEmailStoreLoading = false;
    var costCenterEmailStoreLoading = false;
    var scheduledReportsStoreLoading = false;

    var loadingMask = new Ext.LoadMask(Ext.getBody(), { msg: "Loading..." });

    var btnSearchByEmail = Ext.create('Ext.Button', {
        text: 'Search',
        cls: 'cmbfonts',
        margin: '15 0 0 10',
        handler: function () {
            try {
                if (!textEmail.validate())
                    return;

                loadingMask.show();

                vehicleEmailStoreLoading = true;
                costCenterEmailStoreLoading = true;
                scheduledReportsStoreLoading = true;

                vehicleEmailStore.load(
                {
                    params:
                    {
                        st: 'GetVehicleEmailsByEmail',
                        email: textEmail.getValue()
                        //,fleetID: selFleet
                        //,start: 0
                        //,limit: pagesize
                    }
                });

                costCenterEmailStore.load(
                {
                    params:
                    {
                        st: 'GetCostCenterFleetEmailsByEmail',
                        email: textEmail.getValue()
                        //,fleetID: selFleet
                        //,start: 0
                        //,limit: pagesize
                    }
                });

                scheduledReportsStore.load(
                {
                    params:
                    {
                        st: 'GetScheduledReportsByEmail',
                        email: textEmail.getValue()
                        //,fleetID: selFleet
                        //,start: 0
                        //,limit: pagesize
                    }
                });
            }
            catch (err) {
                alert(err);
            }
        }
    });

    var searchByEmailPanel = Ext.create('Ext.Panel', {
        width: 650,
        height: 60,
        header: true,
        title: 'Search By Email',
        frame: true,
        bodyStyle: 'padding:5px 5px 0;',
        //bodyStyle: 'background:transparent;',
        layout: {
            type: 'column',
            align: 'stretch',
            padding: 5
        },

        defaults: { flex: 0 }, //auto stretch
        items: [
            textEmail, btnSearchByEmail
        ]
    });

    var search_by_vehicle_type_values = [
            [0, 'Description'],
            [1, 'License Plate'],
            [2, 'Box ID'],
            [3, 'VIN Number']
        ];

    var search_by_vehicle_type_store = new Ext.data.SimpleStore({
        fields: ['number', 'vehicletype'],
        data: search_by_vehicle_type_values
    });


    var vehicleType = new Ext.form.ComboBox({
        name: 'vehicleType',
        fieldLabel: '',
        hiddenName: 'vehicleType',
        store: search_by_vehicle_type_store,
        displayField: 'vehicletype',
        valueField: 'number',
        value: 0,
        labelWidth: 50,
        width: 100,
        typeAhead: true,
        mode: 'local',
        triggerAction: 'all',
        emptyText: 'Choose number...',
        selectOnFocus: true,
        editable: false,
        style: 'margin:10px 0 0 30px;'
    });

    var textVehicle = Ext.create('Ext.form.field.Text', {
        anchor: '100%',
        name: 'textVehicle',
        fieldLabel: '',
        allowBlank: false,
        validateOnChange: false,
        validateOnBlur: false,
        height: 30,
        maxHeight: 30,
        width: 250,
        style: 'margin:10px 0 0 10px;',
        labelWidth: 30,
        labelStyle: 'margin: 5px 0 0 0;',     
       
        listeners: { 
            specialkey: function (field, event, options) {
                try{
                    if (event.getKey() == event.ENTER) {                   
                        loadingMask.show();
                        var vehicleTypeValue = vehicleType.getValue();
                        var textSearchParam = textVehicle.getValue();
                        vehicleEmailStoreLoading = true;
                        costCenterEmailStoreLoading = true;
                        scheduledReportsStoreLoading = true;
                        vehicleEmailStore.load(
                         {
                             params:
                             {
                                 st: 'GetVehicleEmailsByVehicle',
                                 vehicleTypeValue: vehicleTypeValue,
                                 textSearchParam: textSearchParam
                                 //,fleetID: selFleet
                                 //,start: 0
                                 //,limit: pagesize
                             },
               
                             callback: function (records, operation, success) {                                
                                 loadingMask.hide();
                                 if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                     Ext.Msg.alert("Warning", operation.resultSet.message);
                                     if (records == null)
                                         vehicleEmailStore.loadData([], false);
                                 }
                             }
                         });

                        costCenterEmailStore.load(
                        {
                            params:
                            {
                                st: 'GetCostCenterFleetEmailsByVehicle',
                                vehicleTypeValue: vehicleTypeValue,
                                textSearchParam: textSearchParam
                                //,fleetID: selFleet
                                //,start: 0
                                //,limit: pagesize
                            },
                            callback: function (records, operation, success) {
                                loadingMask.hide();
                                if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                    Ext.Msg.alert("Warning", operation.resultSet.message);
                                    if (records == null)
                                        costCenterEmailStore.loadData([], false);
                                }
                            }
                        });

                        scheduledReportsStore.load(
                         {
                             params:
                              {
                                  st: 'GetScheduledReportsByVehicle',
                                  vehicleTypeValue: vehicleTypeValue,
                                  textSearchParam: textSearchParam
                                  //,fleetID: selFleet
                                  //,start: 0
                                  //,limit: pagesize
                              },
                             callback: function (records, operation, success) {
                                 loadingMask.hide();
                                 if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                     Ext.Msg.alert("Warning", operation.resultSet.message);
                                     if (records == null)
                                         scheduledReportsStore.loadData([], false);
                                 }
                             }
                         });
                    }
                }
                catch (err) {
                    alert(err);
                }
            }
        }
    });

    var btnSearchByVehicle = Ext.create('Ext.Button', {
        text: 'Search',
        cls: 'cmbfonts',
        margin: '15 0 0 10',
        handler: function () {
            try {
                if (!textVehicle.validate())
                    return;

                var vehicleTypeValue = vehicleType.getValue();
                var textSearchParam = textVehicle.getValue();

                loadingMask.show();

                vehicleEmailStoreLoading = true;
                costCenterEmailStoreLoading = true;
                scheduledReportsStoreLoading = true;

                vehicleEmailStore.load(
                {
                    params:
                    {
                        st: 'GetVehicleEmailsByVehicle',
                        vehicleTypeValue: vehicleTypeValue,
                        textSearchParam: textSearchParam
                        //,fleetID: selFleet
                        //,start: 0
                        //,limit: pagesize
                    }
                });

                costCenterEmailStore.load(
                {
                    params:
                    {
                        st: 'GetCostCenterFleetEmailsByVehicle',
                        vehicleTypeValue: vehicleTypeValue,
                        textSearchParam: textSearchParam
                        //,fleetID: selFleet
                        //,start: 0
                        //,limit: pagesize
                    }
                });

                scheduledReportsStore.load(
                {
                    params:
                    {
                        st: 'GetScheduledReportsByVehicle',
                        vehicleTypeValue: vehicleTypeValue,
                        textSearchParam: textSearchParam
                        //,fleetID: selFleet
                        //,start: 0
                        //,limit: pagesize
                    }
                });
            }
            catch (err) {
                alert(err);
            }
        }
    });

    var searchByVehiclePanel = Ext.create('Ext.Panel', {
        width: 650,
        height: 60,
        header: true,
        title: 'Search By Vehicle',

        frame: true,
        bodyStyle: 'padding:5px 5px 0;',
        //bodyStyle: 'background:transparent;',
        layout: {
            type: 'column',
            align: 'stretch',
            padding: 5
        },

        defaults: { flex: 0 }, //auto stretch
        items: [
            vehicleType, textVehicle, btnSearchByVehicle
        ]
    });

    var searchCriteriaPanel = Ext.create('Ext.tab.Panel',
       {
           region: 'center', // a center region is ALWAYS required for border layout
           deferredRender: false,
           titleCollapse: false,
           autoScroll: true,
           border: false,
           width: 650,
           height: 100,
           renderTo: 'searchCriteriaPanel',

           autoHeight: true,
           collapsible: false,
           animCollapse: true,
           autoDestroy: false,
           activeTab: 0,     // first tab initially active
           items: [searchByEmailPanel, searchByVehiclePanel]
       });

    // Vehicle-Email
    Ext.define('vehicleEmailObject', {
        extend: 'Ext.data.Model',
        fields: ['LicensePlate', 'Description', 'Email', 'BoxId', 'VinNum']
    });

    var vehicleEmailStore = Ext.create('Ext.data.Store', {
        model: 'vehicleEmailObject',
        autosync: false,
        autoLoad: false,
        storeId: 'vehicleEmailStore',
        proxy:
        {
            type: 'ajax',
            url: './ScheduledReportsService.aspx',
            timeout: 12000,
            reader:
            {
                type: 'xml',
                root: 'VehicleEmailsDataSet',
                record: 'VehicleEmails'
            }
        },
        listeners:
        {
            'load': function (store, records, options) {
                vehicleEmailStoreLoading = false;
                if (!vehicleEmailStoreLoading && !costCenterEmailStoreLoading && !scheduledReportsStoreLoading) {
                    loadingMask.hide();
                }
            }
        }
    });

    var vehicleEmailGrid = Ext.create('Ext.grid.Panel', {
        width: 650,
        store: vehicleEmailStore,
        emptyText: 'No records to display',
        features: [vehicleEmailFilters],
        columns: [
            {
                text: "Vehicle", sortable: false, dataIndex: 'Description', width: 100
            },
            {
                text: "License Plate", sortable: false, dataIndex: 'LicensePlate', width: 100
            },
            {
                text: "Vin Num", sortable: false, dataIndex: 'VinNum', width: 200
            },
            {
                text: "Box Id", sortable: false, dataIndex: 'BoxId', width: 70
            },
            {
                text: "Email", flex: 1, sortable: false, dataIndex: 'Email', width: 230
            }
        ],
        stripeRows: true,
        title: 'Vehicles',
        margins: '0 2 0 0',
        renderTo: 'vehicleEmailPanel',
        collapsible: true
    });

    // Cost Center/Fleet-Email
    Ext.define('costCenterEmailObject', {
        extend: 'Ext.data.Model',
        fields: ['FleetName', 'Email', 'FleetType']
    });

    var costCenterEmailStore = Ext.create('Ext.data.Store', {
        model: 'costCenterEmailObject',
        autosync: false,
        autoLoad: false,
        storeId: 'costCenterEmailStore',
        proxy:
        {
            type: 'ajax',
            url: './ScheduledReportsService.aspx',
            timeout: 12000,
            reader:
            {
                type: 'xml',
                root: 'ScheduledReportsCostCenterFleetEmailDataSet',
                record: 'ScheduledReportsCostCenterFleetEmail'
            }
        },
        listeners:
        {
            'load': function (store, records, options) {
                costCenterEmailStoreLoading = false;
                if (!vehicleEmailStoreLoading && !costCenterEmailStoreLoading && !scheduledReportsStoreLoading) {
                    loadingMask.hide();
                }
            }
        }
    });

    var costCenterEmailGrid = Ext.create('Ext.grid.Panel', {
        width: 650,
        store: costCenterEmailStore,
        emptyText: 'No records to display',
        features: [costCenterEmailFilters],
        columns: [
            {
                text: "Coster Center/Fleet", flex: 1, sortable: false, dataIndex: 'FleetName', width: 330
            },
            {
                text: "Type", sortable: false, dataIndex: 'FleetType', width: 120,
                renderer: function (value) {
                    if (value == 'oh')
                        return 'Cost Center';
                    else
                        return 'Fleet';
                }
            },
            {
                text: "Email", flex: 1, sortable: false, dataIndex: 'Email', width: 200
            }
        ],
        stripeRows: true,
        title: 'Cost Center/Fleet',
        margins: '0 2 0 0',
        renderTo: 'costCenterEmailPanel',
        collapsible: true
    });

    // Scheduled Reports
    Ext.define('scheduledReportsObject', {
        extend: 'Ext.data.Model',
        fields: ['ReportID', 'GuiName',
            {
                name: 'DateFrom', type: 'date', dateFormat: 'c'
            },
            {
                name: 'DateTo', type: 'date', dateFormat: 'c'
            }, 'VehicleDescription', 'FleetName', 'Status', 'Email']
    });

    var scheduledReportsStore = Ext.create('Ext.data.Store', {
        model: 'scheduledReportsObject',
        autosync: false,
        autoLoad: false,
        storeId: 'scheduledReportsStore',
        proxy:
        {
            type: 'ajax',
            url: './ScheduledReportsService.aspx',
            timeout: 12000,
            reader:
            {
                type: 'xml',
                root: 'ScheduledReportsDataSet',
                record: 'ScheduledReports'
            }
        },
        listeners:
        {
            'load': function (store, records, options) {
                scheduledReportsStoreLoading = false;
                if (!vehicleEmailStoreLoading && !costCenterEmailStoreLoading && !scheduledReportsStoreLoading) {
                    loadingMask.hide();
                }
            }
        }
    });

    var scheduledReportsGrid = Ext.create('Ext.grid.Panel', {
        width: 1130,
        store: scheduledReportsStore,
        emptyText: 'No records to display',
        features: [scheduledReportsFilters],
        columns: [
            {
                text: "ReportID", sortable: false, dataIndex: 'ReportID', width: 70
            },
            {
                text: "Report", flex: 1, sortable: false, dataIndex: 'GuiName', width: 120
            },
            {
                text: "From", sortable: false, dataIndex: 'DateFrom', format: 'd/m/Y h:i a', xtype: 'datecolumn', width: 120
            },
            {
                text: "To", sortable: false, dataIndex: 'DateTo', format: 'd/m/Y h:i a', xtype: 'datecolumn', width: 120
            },
            {
                text: "Vehicle", sortable: false, dataIndex: 'VehicleDescription', width: 70
            },
            {
                text: "Fleet", sortable: false, dataIndex: 'FleetName', width: 200
            },
            {
                text: "Status", sortable: false, dataIndex: 'Status', width: 60
            },
            {
                text: "Email", sortable: false, dataIndex: 'Email', width: 160
            }
        ],
        stripeRows: true,
        title: 'Scheduled Reports',
        margins: '0 2 0 0',
        renderTo: 'scheduledReportsPanel',
        collapsible: true
    });


});