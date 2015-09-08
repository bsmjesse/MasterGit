
var SelectedFleetName;
var DefaultOrganizationHierarchyFleetId;
var DefaultOrganizationHierarchyNodeCode;
var MutipleUserHierarchyAssignment;
var PreferOrganizationHierarchyNodeCode;
var DefaultOrganizationHierarchyFleetName;
var ScheventId;
var proxyTimeOut = 120000;
var SummaryGridPagesize = 10;
var DetailGridPagesize = 10;
var Pagesize = 100;
var FleetPagesize = 20;
var VehiclePagesize = 20;
var table;
var DateFrom;
var TimeFrom;
var SchTimeFrom;
var SchTimeTo;
var DateTo;
var TimeTo;
var DateBtn;
var timeTest12 = /^([0-1][0-9]):([0-5][0-9]):([0-5][0-9])(\s[A|P]M)$/i;
var timeTest24 = /^([0-1][0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$/i;
var ViolationboxGroup;
var VisiblecolumnsboxGroup;
var DefaultEventSelectedCol = '';
var DefaultViolationSelectedCol = '';
var sendNoOfDays = false;
var VisibleCols;
var FleetboxGroup;
var DetailGridStore;
var loadingMask;
var sendingMask;
var firstTimeload = true;
var defaultFleetName = "";
var defaultVehicleName = "";
var sortingParam = {};
var searchColumns = "";
var timetest;
var timeText;
var FleetBtn;
var VehicleBtn;
var DefaultSelectedId;
var selectedFleet = [];
var DeSelectedIndex = [];
var DeSelectedIndexVehicle = [];
var DeSelectedFleetId = [];
var selectedFleetId = [];
var selectedVehicle = [];
var DeSelectedVehicleId = [];
var selectedVehicleId = [];
var DeselectedVehicleId = [];
var allSelected = false;
var allSelectedVehicle = false;
var selectAllVehicleFlag = false;
var currentPg = 0;
var onlyOnce = 0;
var DataPresent = 0;
var count = '1';
var headerChecked = 0;
var selectedEvent = [];
var selectedEventIndex = [];
var st = 1;
var toggle = 1;
var EventSelected = false;
var DatabaseColID = [];
var SenchaColID = [];
var SenchaColName = [];
var SchedulerColumnsGroup;
var SelectedFleetNames = [];
var nDefaultFleetIndex = -1;
var Select = false;
var DeSelect = true;
var bFleetSelectionMode = Select;
DatabaseColID.push(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27);
SenchaColID.push('UnitID', 'LicensePlate', 'VehicleDescription', 'VehicleMake', 'VehicleModel', 'VINNumber', 'address', 'DriverName', 'ServiceName', 'ServiceType', 'DateTime', 'EventType', 'Driver_Class', 'Vehicle_Class', 'Field1', 'Field2', 'Field3', 'Field4', 'Field5', 'Note', 'ManagerId', 'ManagerName', 'color', 'LandmarkName', 'FleetName', 'bscid', 'EmployeeId');
SenchaColName.push('Unit ID', 'License Plate', 'Vehicle Description', 'Vehicle Make', 'Vehicle Model', 'Vin Number', 'Address', 'Driver Name', 'Service Name', 'Service Type', 'Date Time', 'Event Type', 'Driver Class', 'Vehicle Class', 'Field 1', 'Field 2', 'Field 3', 'Field 4', 'Field 5', 'Notes', 'Manager Employee', 'Manager Name', 'Color', 'Landmark Name', 'Fleet Name', 'BSCID', 'Employee ID');
Ext.Loader.setConfig({
    enabled: true,
    disableCaching: false
});
Ext.Loader.setPath('Ext.ux', './extjs/examples/ux');
Ext.Loader.setPath('Ext.ux.exporter', './sencha/Ext.ux.Exporter');
Ext.require([
'Ext.window.*',
'Ext.ux.grid.FiltersFeature',
'Ext.ux.AspWebAjaxProxy',
'Ext.ux.form.MultiSelect',
'Ext.ux.exporter.Exporter.*',
     'Ext.ux.exporter.excelFormatter.*',
     'Ext.ux.exporter.csvFormatter.*',
     'Ext.ux.exporter.Button.*'
]);
Ext.define('VehicleList', {
    extend: 'Ext.data.Model',
    fields: [
        'LicensePlate',
        'BoxId',
        'VehicleId',
        'VinNum',
        'MakeName',
        'ModelName',
        'VehicleTypeName',
        'StateProvince',
        'ModelYear',
        'Color',
        'Description',
        'FleetId',
        'FleetName'

    ]
});
Ext.define('EventModel', {
    extend: 'Ext.data.Model',
    fields: [
        'EventTypeID',
        'Description'
    ]
});
Ext.define('User', {
    extend: 'Ext.data.Model',
    fields: ['BoxId', 'Description', 'Driver']
});
Ext.define('FleetModel', {
    extend: 'Ext.data.Model',
    fields: ['FleetName', 'FleetId']
});
Ext.define('VehicleModel', {
    extend: 'Ext.data.Model',
    fields: ['VehicleName', 'VehicleId']
});
Ext.define('EventGridModel', {
    extend: 'Ext.data.Model',
    fields: [
            { name: 'EventID', type: 'int' }
            , 'ServiceName'
            , 'ServiceType'
            , 'BoxID'
            , 'VehicleID'
            , 'LicensePlate'
            , 'VehicleDescription'
            , 'VinNumber'
            , 'MakeName'
            , 'ModelName'
            , 'AssignmentDate'
            , 'OrganizationID'
            , 'DriverID'
            , 'DriverName'
            , { name: 'NumberOfEvents', type: 'int' }
            , { name: 'StDate', type: 'date', dateFormat: 'c' }
            , 'TableType'
            , { name: 'rowNo', type: 'int' }
            , 'Address'
            , 'Driver_Class'
            , 'Vehicle_Class'
            , 'Field1'
            , 'Field2'
            , 'Field3'
            , 'Field4'
            , 'Field5'
            , 'Notes'
            , 'ManagerId'
            , 'ManagerName'
            , 'color'
            , 'LandmarkName'
            , 'CostCenter'
            , 'bscid'
            , 'EmployeeId'
    ]
});


// custom Vtype for vtype:'time'
Ext.apply(Ext.form.field.VTypes, {
    //  vtype validation function
    time: function (val, field) {
        if (timetest.test(val)) {
            DateBtn.setText("From: " + DateFrom.rawValue + " " + TimeFrom.value + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: " + DateTo.rawValue + " " + TimeTo.value);
            return true;
        }
    },
    // vtype Text property: The error text to display when the validation function returns false
    timeText: 'Not a valid time.',
    // vtype Mask property: The keystroke filter mask
    timeMask: /[\d\s:amp]/i
});
var oldvalue = "";
Ext.define('Ext.ux.CustomSpinner', {
    extend: 'Ext.form.field.Spinner',
    alias: 'widget.customspinner',
    onSpinUp: function () {
        var me = this;
        if (!me.readOnly) {
            if (userTime.indexOf('A') > -1) {
                var splitedtime = me.getValue().split(':');
                var currenttt = splitedtime[2].split(' ')[1];
                if (me.getValue() !== '' && (parseInt(splitedtime[0]) <= 12)) {
                    if (splitedtime[1] == '00') {
                        splitedtime[1] = '45';
                        if (parseInt(splitedtime[0]) == 12) {
                            if (currenttt == 'AM')
                                currenttt = 'PM';
                            else
                                currenttt = 'AM';
                        }
                        splitedtime[0] = (parseInt(splitedtime[0]) - 1) + '';
                    }
                    else if (splitedtime[1] == '15') {
                        if (parseInt(splitedtime[0]) == 1)
                            splitedtime[0] = '12';
                        splitedtime[1] = (parseInt(splitedtime[1]) - me.step) + '';
                    }
                    else
                        splitedtime[1] = (parseInt(splitedtime[1]) - me.step) + '';
                    if (parseInt(splitedtime[0]) < 10) {
                        if (splitedtime[0].indexOf('0') == -1 || splitedtime[0] == '0')
                            splitedtime[0] = '0' + splitedtime[0];
                    }
                    if (parseInt(splitedtime[1]) < 10) {
                        if (splitedtime[1].indexOf('0') == -1 || splitedtime[1] == '0')
                            splitedtime[1] = '0' + splitedtime[1];
                    }
                    me.setValue(splitedtime[0] + ':' + splitedtime[1] + ':00 ' + currenttt);
                    DateBtn.setText("From: " + DateFrom.rawValue + " " + TimeFrom.value + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: " + DateTo.rawValue + " " + TimeTo.value);
                }
            }
            else {
                var splitedtime = me.getValue().split(':');
                if (me.getValue() !== '' && (parseInt(splitedtime[0]) <= 23)) {
                    if (splitedtime[1] == '00') {
                        splitedtime[1] = '45';
                        if (splitedtime[0] == '00')
                            splitedtime[0] = '23';
                        else
                            splitedtime[0] = (parseInt(splitedtime[0]) - 1) + '';
                    }
                    else
                        splitedtime[1] = (parseInt(splitedtime[1]) - me.step) + '';
                    if (parseInt(splitedtime[0]) < 10) {
                        if (splitedtime[0].indexOf('0') == -1 || splitedtime[0] == '0')
                            splitedtime[0] = '0' + splitedtime[0];
                    }
                    if (parseInt(splitedtime[1]) < 10) {
                        if (splitedtime[1].indexOf('0') == -1 || splitedtime[1] == '0')
                            splitedtime[1] = '0' + splitedtime[1];
                    }
                    me.setValue(splitedtime[0] + ':' + splitedtime[1] + ':00');
                    DateBtn.setText("From: " + DateFrom.rawValue + " " + TimeFrom.value + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: " + DateTo.rawValue + " " + TimeTo.value);
                }
            }
        }
    },
    onSpinDown: function () {
        var me = this;
        if (!me.readOnly) {
            if (userTime.indexOf('A') > -1) {
                var splitedtime = me.getValue().split(':');
                var currenttt = splitedtime[2].split(' ')[1];
                if (me.getValue() !== '' && (parseInt(splitedtime[0]) <= 12)) {
                    if (splitedtime[1] == '45') {
                        splitedtime[1] = '00';
                        if (parseInt(splitedtime[0]) == 11) {
                            if (currenttt == 'AM')
                                currenttt = 'PM';
                            else
                                currenttt = 'AM';
                        }
                        if (parseInt(splitedtime[0]) == 12)
                            splitedtime[0] = '01';
                        else
                            splitedtime[0] = parseInt(splitedtime[0]) + 1 + '';
                    }
                    else
                        splitedtime[1] = (parseInt(splitedtime[1]) + me.step) + '';
                    if (parseInt(splitedtime[0]) < 10) {
                        if (splitedtime[0].indexOf('0') == -1 || splitedtime[0] == '0')
                            splitedtime[0] = '0' + splitedtime[0];
                    }
                    if (parseInt(splitedtime[1]) < 10) {
                        if (splitedtime[1].indexOf('0') == -1 || splitedtime[1] == '0')
                            splitedtime[1] = '0' + splitedtime[1];
                    }
                    me.setValue(splitedtime[0] + ':' + splitedtime[1] + ':00 ' + currenttt);
                    DateBtn.setText("From: " + DateFrom.rawValue + " " + TimeFrom.value + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: " + DateTo.rawValue + " " + TimeTo.value);
                }
            }
            else {
                var splitedtime = me.getValue().split(':');
                if (me.getValue() !== '' && (parseInt(splitedtime[0]) <= 23)) {
                    if (splitedtime[1] == '45') {
                        splitedtime[1] = '00';
                        if (splitedtime[0] == '23')
                            splitedtime[0] = '00';
                        else
                            splitedtime[0] = (parseInt(splitedtime[0]) + 1) + '';
                    }
                    else
                        splitedtime[1] = (parseInt(splitedtime[1]) + me.step) + '';
                    if (parseInt(splitedtime[0]) < 10) {
                        if (splitedtime[0].indexOf('0') == -1 || splitedtime[0] == '0')
                            splitedtime[0] = '0' + splitedtime[0];
                    }
                    if (parseInt(splitedtime[1]) < 10) {
                        if (splitedtime[1].indexOf('0') == -1 || splitedtime[1] == '0')
                            splitedtime[1] = '0' + splitedtime[1];
                    }
                    me.setValue(splitedtime[0] + ':' + splitedtime[1] + ':00');
                    DateBtn.setText("From: " + DateFrom.rawValue + " " + TimeFrom.value + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: " + DateTo.rawValue + " " + TimeTo.value);
                }
            }
        }
    }
});


Ext.onReady(function () {


    if (userTime.indexOf('A') > -1)
        timetest = timeTest12;
    else
        timetest = timeTest24;

    if (userTime.indexOf('A') > -1)
        timeText = "hh:mm:ss tt";
    else
        timeText = "HH:mm:ss";

    loadingMask = new Ext.LoadMask(Ext.getBody(), {
        msg: "Loading..."
    });

    sendingMask = new Ext.LoadMask(Ext.getBody(), {
        msg: "Sending..."
    });

    //DateTime container
    DateFrom = Ext.create('Ext.form.field.Date', {
        name: 'DateFrom',
        id: 'DateFrom',
        fieldLabel: '',
        labelWidth: 0,
        format: userDate,
        value: new Date(),
        width: 140,
        maxWidth: 140,
        listeners: {
            'change': function (me) {

                if (this.getSubmitValue() > DateTo.getValue())
                    Ext.Msg.alert("Warning", 'To date can not be less then From date');
            }
        }
    });
    DateTo = Ext.create('Ext.form.field.Date', {
        name: 'DateTo',
        id: 'DateTo',
        fieldLabel: '',
        labelWidth: 0,
        format: userDate,
        value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1),
        width: 140,
        maxWidth: 140,
        listeners: {
            'change': function (me) {
                if (this.getSubmitValue() < DateTo.getValue())
                    Ext.Msg.alert("Warning", 'To date can not be less then From date');
            }
        }
    });
    TimeFrom = Ext.create('Ext.ux.CustomSpinner', {
        name: 'TimeFrom',
        fieldLabel: 'Start Time',
        //cls:'fa fa-arrow-circle-o-right',
        labelWidth: 70,
        editable: true,
        enableKeyEvents: true,
        emptyText: timeText,
        value: '12:00:00 AM',
        vtype: 'time',
        step: 15
    });
    TimeTo = Ext.create('Ext.ux.CustomSpinner', {
        name: 'TimeTo',
        fieldLabel: 'End Time',
        labelWidth: 70,
        editable: true,
        enableKeyEvents: true,
        emptyText: timeText,
        value: '12:00:00 AM',
        vtype: 'time',
        step: 15
    });
    //Vehicle Grid
    var VehicleStore = Ext.create('Ext.data.Store', {
        model: 'VehicleModel',
        autoLoad: false,
        storeId: 'VehicleStore',
        pageSize: VehiclePagesize,
        proxy:
          {
              type: 'ajax',
              url: './Event.aspx?QueryType=GetVehicle',
              timeout: proxyTimeOut,
              reader:
             {
                 type: 'xml',
                 root: 'DocumentElement',
                 record: 'Vehicle',
                 messageProperty: 'message',
                 totalProperty: 'totalCount',


             }
          }
    });

    var VehicleGridselModel = Ext.create('Ext.selection.CheckboxModel', {
        checkOnly: true,
        enableKeyNav: false,
        multipageSelection: {},
        listeners:
        {
            selectionchange: function (selModel, selections) {
                if (allSelectedVehicle) {

                    VehicleStore.each(function (record, idx) {

                        if (DeselectedVehicleId.indexOf(record.data.VehicleId) == -1) {

                            //  Select This Record  
                            //var selected = record.index - (VehicleStore.currentPage - 1) * VehiclePagesize;
                            //VehicleGridselModel.select(selected, true, true);
                            var rowIdx = VehicleStore.find('VehicleId', record.data.VehicleId);
                            if (rowIdx > -1) {
                                VehicleGridselModel.select(rowIdx, true, true);
                            }

                            // Update SelectedVehicle Array
                            if (selectedVehicle.indexOf(record.index) == -1) {
                                selectedVehicle.push(record.index);
                            }

                            // Update SelectedVehicle Array
                            if (selectedVehicleId.indexOf(record.data.VehicleId) == -1)
                                selectedVehicleId.push(record.data.VehicleId);
                        }
                    });
                   
                    return;
                }

                //if (Object.keys(selections).length == 0)
                //    VehicleStore.each(function (record, idx) {
                //        if (selectedVehicle.indexOf(record.index) > -1) {
                //            var selected = record.index - (VehicleStore.currentPage - 1) * VehiclePagesize;
                //            VehicleGridselModel.select(selected, true, true);
                //        }
                //    });
                //if (Object.keys(selections).length == 0)
                Ext.each(selectedVehicleId, function (val, index) {
                    var rowIndex = VehicleStore.find('VehicleId', val);
                    if (rowIndex > -1)
                    {
                        VehicleGridselModel.select(rowIndex, true, true);
                    }
                    
                });
                if (allSelectedVehicle) {
                    Ext.each(DeselectedVehicleId, function (val, index) {
                        var rowIndex2 = VehicleStore.find('VehicleId', val);
                        if (rowIndex2 > -1) {
                            VehicleGridselModel.deselect(rowIndex2, true, true);
                        }

                    });
                }
                

            },
            deselect: function (selectionModel, record, index, eOpts) {


                //if (defaultVehicleName != "" && defaultVehicleName != null) {
                //    var tests = VehicleGrid.getStore().findExact('VehicleName', defaultVehicleName, 0);
                //    if (tests != -1) {
                //        VehicleGridselModel.deselect(tests, true, true);
                //    }
                //}
                //defaultVehicleName = "";
                if (selectedVehicleId.indexOf(record.index) > -1) {
                    selectedVehicle.splice(selectedVehicle.indexOf(record.index), 1);
                }

                if (selectedVehicleId.indexOf(record.data.VehicleId) > -1) {                    
                    selectedVehicleId.splice(selectedVehicleId.indexOf(record.data.VehicleId), 1);
                }
                if (DeselectedVehicleId.indexOf(record.data.VehicleId) == -1) {                    
                    DeselectedVehicleId.push(record.data.VehicleId);
                }

                if (DeSelectedIndexVehicle.indexOf(record.index) == -1) {
                    DeSelectedIndexVehicle.push(record.index);
                }

                if (selectedVehicleId.length > 1)
                    VehicleBtn.setText("Vehicle: <br/>Multiple Vehicles");
                else if (selectedVehicleId.length == 1) {
                    var row = VehicleGrid.getSelectionModel().getSelection()[0];
                    VehicleBtn.setText("Vehicle: <br/>" + row.get('VehicleName'));
                }
                    //VehicleBtn.setText("Vehicle: <br/>" + record.data.VehicleName);
                else
                    VehicleBtn.setText("Vehicle: <br/>" + 'Select a Vehicle');
            },
            select: function (selectionModel, record, index, eOpts) {

                if (selectedVehicle.indexOf(record.index) == -1) {
                    selectedVehicle.push(record.index);
                }

                if (selectedVehicleId.indexOf(record.data.VehicleId) == -1)
                    selectedVehicleId.push(record.data.VehicleId);

                if (DeSelectedIndexVehicle.indexOf(record.index) > -1) {
                    DeSelectedIndexVehicle.splice(DeSelectedIndexVehicle.indexOf(record.index), 1);
                }

                if (DeselectedVehicleId.indexOf(record.data.VehicleId) > -1) {
                    DeselectedVehicleId.splice(DeselectedVehicleId.indexOf(recordDeselectedVehicleId), 1);
                }

                if (selectedVehicleId.length > 1)
                    VehicleBtn.setText("Vehicle: <br/>Multiple Vehicles");
                else if (selectedVehicleId.length == 1)
                    VehicleBtn.setText("Vehicle: <br/>" + record.data.VehicleName);
                else
                    VehicleBtn.setText("Vehicle: <br/>" + 'Select a Vehicle');
            }
        }
    });
    var VehiclePager = new Ext.PagingToolbar({
        id: 'vehiclePager',
        store: 'VehicleStore',
        displayInfo: false,
        displayMsg: 'Displaying Records {0} - {1} of {2}',
        emptyMsg: "No Records to display",
        listeners: {
            beforechange: function (b, page, o) {
                var fleetids = "";
                if (allSelected)
                    fleetids = ',-1,'
                else {

                    Ext.each(selectedFleetId, function (val, index) {
                        fleetids += ',' + val;
                    });
                    fleetids += ',';
                }
                
                VehicleStore.proxy.extraParams = {
                    pageno: page,
                    start: page - 1,
                    SearchVehicle: Ext.getCmp('searchVehicleId').getValue(),
                    Columns: searchColumns,
                    limit: VehiclePagesize,
                    FleetFirstTime: false,
                    Fleet: fleetids

                }
            },
            change: function (thisd, params) {
                //if (defaultVehicleName != "" && defaultVehicleName != null) {
                //    var tests = VehicleGrid.getStore().findExact('VehicleName', defaultVehicleName , 0);
                //    if (tests != -1) {
                //        VehicleGridselModel.select(tests, true, true);
                //    }
                //}
                Ext.each(selectedVehicleId, function (val, index) {
                    var rowIndex = VehicleStore.find('VehicleId', val);
                    if (rowIndex > -1) {
                        VehicleGridselModel.select(rowIndex, true, true);
                    }

                });
                if (allSelectedVehicle) {
                    Ext.each(DeselectedVehicleId, function (val, index) {
                        var rowIndex2 = VehicleStore.find('VehicleId', val);
                        if (rowIndex2 > -1) {
                            VehicleGridselModel.deselect(rowIndex2, true, true);
                        }

                    });
                }
                //VehicleStore.each(function (record, idx) {
                //    if (selectedVehicle.indexOf(record.index) > -1) {
                //        var selected = record.index - (VehicleStore.currentPage - 1) * VehiclePagesize;
                //        VehicleGridselModel.select(selected, true, true);
                //    }
                //});
            }
        }
    });
    var VehicleGrid = Ext.create('Ext.grid.Panel', {
        hideHeaders: true,
        xtype: 'exampleGrid',
        id: 'VehicleGrid',
        align: 'Right',
        autoLoad: false,
        store: 'VehicleStore',
        width: 260,
        height: 220,
        autoScroll: true,
        selModel: VehicleGridselModel,
        viewConfig: {
            stripeRows: false,
            forceFit: true
        },

        columns: [{
            dataIndex: 'VehicleId',
            cls: 'RemoveLine',
            filterable: false,
            flex: 1,
            sortable: true,
            hidden: true
        },{
            dataIndex: 'VehicleName',
            cls: 'RemoveLine',
            filterable: false,
            flex: 1,
            sortable: true,
            hidden: false
        }],
        tbar: ['Search', {
            xtype: 'textfield',
            name: 'searchVehicle',
            id: 'searchVehicleId',
            hideLabel: true,
            width: 200,
            listeners: {
                specialkey: function (field, event, options) {
                    if (event.getKey() == event.ENTER) {
                        
                        var fleetids = "";

                        //  If De-Selected Fleet IDs are to be sent to Web Method
                        if (bFleetSelectionMode == DeSelect) {

                            if (DeSelectedFleetId.length == 0)
                                fleetids = ',-1,';

                            else {
                                Ext.each(DeSelectedFleetId, function (val, index) {
                                    fleetids += ',' + val;
                                });

                                fleetids += ',';
                            }

                        }

                            //  Else, If Selected Fleet IDs are to be sent to Web Method. Server side has the logic to get actual FleetIDs 
                        else if (bFleetSelectionMode == Select) {

                            if (allSelected && DeSelectedFleetId.length == 0)
                                fleetids = ',-1,';
                            else {

                                Ext.each(selectedFleetId, function (val, index) {
                                    fleetids += ',' + val;
                                });

                                fleetids += ',';
                            }

                        }

                        if (fleetids == ",") {
                            Ext.Msg.alert("Warning", 'Please select a fleet');
                            SummaryGridStore.loadData([], false);
                            return;
                        }
                        
                        VehicleStore.load(
                            {
                                params:
                                {
                                    SearchVehicle: field.value,
                                    Columns: searchColumns,
                                    pageno:1,
                                    start: 0,
                                    limit: VehiclePagesize,
                                    FleetFirstTime: false,
                                    Fleet: fleetids,
                                    ExcludeFleetIDs: bFleetSelectionMode
                                },
                                callback: function (records, operation, success) {
                                    if (field.value != null && field.value != "")
                                        VehicleStore.totalCount = records.length;
                                    VehiclePager.bindStore(VehicleStore);
                                    VehiclePager.onLoad();
                                    VehiclePager.moveFirst();
                                    
                                }
                            });
                    }
                }
            }
        }
        ],
        bbar: VehiclePager
    });

    function setFleetButtonText() {
        if (bFleetSelectionMode == Select) {
            if (selectedFleetId.length > 1)
                FleetBtn.setText("Fleet: <br/>" + selectedFleetId.length.toString() + " Fleets");
            else if (selectedFleetId.length == 1) {
                FleetBtn.setText("Fleet: <br/>" + SelectedFleetNames[0]);
            }

            else
                FleetBtn.setText("Fleet: <br/>" + 'Select a Fleet');
        }
        else if (bFleetSelectionMode == DeSelect) {

            if (DeSelectedFleetId.length == Fleetstore.totalCount)
                FleetBtn.setText("Fleet: <br/>" + 'Select a Fleet');

            else if ((Fleetstore.totalCount - DeSelectedFleetId.length) == 1)
                FleetBtn.setText("Fleet: <br/>" + SelectedFleetNames[0]);

            else {
                FleetBtn.setText("Fleet: <br/>" + (Fleetstore.totalCount - DeSelectedFleetId.length).toString() + " Fleets");
            }

        }
    }

    //Fleet Grid
    var Fleetstore = Ext.create('Ext.data.Store', {
        model: 'FleetModel',
        autoLoad: false,
        storeId: 'Fleetstore',
        pageSize: FleetPagesize,
        proxy:
          {
              type: 'ajax',
              url: './Event.aspx?QueryType=GetFleet',
              timeout: proxyTimeOut,
              reader:
             {
                 type: 'xml',
                 root: 'DocumentElement',
                 record: 'Fleet',
                 messageProperty: 'message',
                 totalProperty: 'totalCount',
                

             }
          }
    });

    var FleetGridselModel = Ext.create('Ext.selection.CheckboxModel', {
        checkOnly: true,
        enableKeyNav: false,
        multipageSelection: {},
        listeners:
        {
            selectionchange: function (selModel, selections) {

                setFleetButtonText();

                if (allSelected) {

                    Fleetstore.each(function (record, idx) {

                        if (DeSelectedIndex.indexOf(record.index) == -1) {

                            //  Select This Record  
                            var selected = record.index - (Fleetstore.currentPage - 1) * FleetPagesize;
                            FleetGridselModel.select(selected, true, true);

                            // Update SelectedFleet Array
                            if (selectedFleet.indexOf(record.index) == -1) {
                                selectedFleet.push(record.index);
                            }

                            // Update SelectedFleet Array
                            if (selectedFleetId.indexOf(record.data.FleetId) == -1)
                                selectedFleetId.push(record.data.FleetId);

                            if (SelectedFleetNames.indexOf(record.data.FleetName) == -1) {
                                SelectedFleetNames.push(record.data.FleetName);
                            }
                        }

                    });

                    return;
                }

                if (Object.keys(selections).length == 0)
                    Fleetstore.each(function (record, idx) {
                        if (selectedFleet.indexOf(record.index) > -1) {
                            var selected = record.index - (Fleetstore.currentPage - 1) * FleetPagesize;
                            FleetGridselModel.select(selected, true, true);
                        }
                    });

            },
            deselect: function (selectionModel, record, index, eOpts) {

                if (SelectedFleetNames.indexOf(record.data.FleetName) > -1) {
                    SelectedFleetNames.splice(SelectedFleetNames.indexOf(record.data.FleetName), 1);
                }

                if (defaultFleetName != "" && defaultFleetName != null && record.data.FleetName == defaultFleetName) {
                    defaultFleetName = "";
                }

                if (selectedFleetId.indexOf(record.data.FleetId) > -1) {
                    selectedFleetId.splice(selectedFleetId.indexOf(record.data.FleetId), 1);
                }

                if (selectedFleet.indexOf(record.index) > -1) {
                    selectedFleet.splice(selectedFleet.indexOf(record.index), 1);
                }

                if (DeSelectedIndex.indexOf(record.index) == -1) {
                    DeSelectedIndex.push(record.index);
                }

                if (DeSelectedFleetId.indexOf(record.data.FleetId) == -1) {
                    DeSelectedFleetId.push(record.data.FleetId);
                }

                //if (selectedFleetId.length > 1)
                //    FleetBtn.setText("Fleet: <br/>Multi Fleets");
                //else if (selectedFleetId.length == 1) {
                //    FleetBtn.setText("Fleet: <br/>" + SelectedFleetNames[0]);
                //}

                //else
                //    FleetBtn.setText("Fleet: <br/>" + 'Select a Fleet');
            },
            select: function (selectionModel, record, index, eOpts) {

                if (SelectedFleetNames.indexOf(record.data.FleetName) == -1) {
                    SelectedFleetNames.push(record.data.FleetName);
                }

                if (selectedFleet.indexOf(record.index) == -1) {
                    selectedFleet.push(record.index);

                    if (selectedFleetId.indexOf(record.data.FleetId) == -1)
                        selectedFleetId.push(record.data.FleetId);
                }

                if (DeSelectedIndex.indexOf(record.index) > -1) {
                    DeSelectedIndex.splice(DeSelectedIndex.indexOf(record.index), 1);
                }

                if (DeSelectedFleetId.indexOf(record.data.FleetId) > -1) {
                    DeSelectedFleetId.splice(DeSelectedFleetId.indexOf(record.data.FleetId), 1);
                }


                //if (selectedFleetId.length > 1)
                //    FleetBtn.setText("Fleet: <br/>Multi Fleets");
                //else if (selectedFleetId.length == 1)
                //    FleetBtn.setText("Fleet: <br/>" + record.data.FleetName);
                //else
                //    FleetBtn.setText("Fleet: <br/>" + 'Select a Fleet');
            }
        }
    });

    var FleetPager = new Ext.PagingToolbar({
        id: 'FleetPager',
        store: 'Fleetstore',
        displayInfo: false,
        displayMsg: 'Displaying Records {0} - {1} of {2}',
        emptyMsg: "No Records to display",
        listeners: {
            beforechange: function (b, page, o) {
                Fleetstore.proxy.extraParams = {
                    pageno: page,
                    start: page - 1,
                    limit: FleetPagesize
                }
            },
            change: function (thisd, params) {
                if (defaultFleetName != "" && defaultFleetName != null) {
                    var tests = FleetGrid.getStore().findExact('FleetName', defaultFleetName, 0);
                    if (tests != -1) {
                        FleetGridselModel.select(tests, true, true);
                    }
                }
                                            
                Fleetstore.each(function (record, idx) {
                    if (selectedFleet.indexOf(record.index) > -1) {
                        var selected = record.index - (Fleetstore.currentPage - 1) * FleetPagesize;
                        FleetGridselModel.select(selected, true, true);
                    }
                });

                
            }
        }
    });
    var FleetGrid = Ext.create('Ext.grid.Panel', {
        hideHeaders: true,
        xtype: 'exampleGrid',
        id: 'FleetGrid',
        align: 'Right',
        autoLoad: false,
        store: 'Fleetstore',
        width: 260,
        height: 220,
        autoScroll: true,
        selModel: FleetGridselModel,
        viewConfig: {
            stripeRows: false,
            forceFit: true
        },
        
        columns: [{
            dataIndex: 'FleetName',
            cls: 'RemoveLine',
            filterable: false,
            flex: 1,
            sortable: true,
            hidden: false
        }],
        tbar: ['Search', {
            xtype: 'textfield',
            name: 'searchFleet',
            id: 'searchFleet',
            hideLabel: true,
            width: 200,
            listeners: {
                specialkey: function (field, event, options) {
                    if (event.getKey() == event.ENTER) {
                        Fleetstore.load(
                            {
                                params:
                                {
                                    Search: field.value,
                                    Columns: searchColumns,
                                    start: 0,
                                    limit: FleetPagesize
                                },
                                callback: function (records, operation, success) {
                                    if (field.value != null && field.value != "")
                                        Fleetstore.totalCount = records.length;
                                    FleetPager.bindStore(Fleetstore);
                                    FleetPager.onLoad();
                                   
                                }
                            });
                    }
                }
            }
        }
        ],
        bbar: FleetPager
    });


    var selectedDates = { "From": new Date().toDateString(), "To": new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1).toDateString() };
    var myDatePicker = {
        xtype: 'datepicker',
        handler: function (picker, date) {
            if (Object.keys(selectedDates).length == 1) {
                var stringdate = date.toDateString();
                if (new Date(date.toDateString()) >= new Date(selectedDates["From"])) {
                    selectedDates["To"] = date.toDateString();
                }
                else {
                    selectedDates = {};
                    selectedDates["From"] = date.toDateString();
                }
            }
            else if (date.toDateString() == selectedDates["To"]) {
                selectedDates["To"] = date.toDateString();
            }
            else {
                selectedDates = {};
                selectedDates["From"] = date.toDateString();
            }
            var btntext = "";
            if (typeof (selectedDates["From"]) != 'undefined') {
                ViolationChkboxList
                DateFrom.setValue(new Date(selectedDates["From"]));
                btntext = "From: " + DateFrom.rawValue + " " + TimeFrom.value;
                DateBtn.setWidth(200);
            }
            if (typeof (selectedDates["To"]) != 'undefined') {
                DateTo.setValue(new Date(selectedDates["To"]));
                btntext = "From: " + DateFrom.rawValue + " " + TimeFrom.value + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: " + DateTo.rawValue + " " + TimeTo.value;
            }
            else
                btntext += "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: Please select To  date";
            DateBtn.setText(btntext);
            DateBtn.setWidth(200);
            this.cells.each(function (item) {
                var date = new Date(item.dom.firstChild.dateValue).toDateString();
                if (new Date(selectedDates["From"]) <= new Date(date) && new Date(selectedDates["To"]) >= new Date(date)) {
                    var getclass = item.dom.firstChild.getAttribute('class');
                    item.addCls('x-datepicker-selected');
                }
            })
        },
        listeners: {
            'onShow': function () {
                this.cells.each(function (item) {
                    var date = new Date(item.dom.firstChild.dateValue).toDateString();
                    if (new Date(selectedDates["From"]) <= new Date(date) && new Date(selectedDates["To"]) >= new Date(date)) {
                        var getclass = item.dom.firstChild.getAttribute('class');
                        item.addCls('x-datepicker-selected');
                    }
                })
            },
            'selectToday': function () {
                this.cells.each(function (item) {
                    var date = new Date(item.dom.firstChild.dateValue).toDateString();
                    if (new Date(selectedDates["From"]) <= new Date(date) && new Date(selectedDates["To"]) >= new Date(date)) {
                        var getclass = item.dom.firstChild.getAttribute('class');
                        item.addCls('x-datepicker-selected');
                    }
                })
            },
            'selectedUpdate': function () {
                this.cells.each(function (item) {
                    var date = new Date(item.dom.firstChild.dateValue).toDateString();
                    if (new Date(selectedDates["From"]) <= new Date(date) && new Date(selectedDates["To"]) >= new Date(date)) {
                        var getclass = item.dom.firstChild.getAttribute('class');
                        item.addCls('x-datepicker-selected');
                    }
                })
            },
            'highlightitem': function () {
                this.cells.each(function (item) {
                    var date = new Date(item.dom.firstChild.dateValue).toDateString();
                    if (new Date(selectedDates["From"]) <= new Date(date) && new Date(selectedDates["To"]) >= new Date(date)) {
                        var getclass = item.dom.firstChild.getAttribute('class');
                        item.addCls('x-datepicker-selected');
                    }
                })
            },
            'boxready': function () {
                this.cells.each(function (item) {
                    var date = new Date(item.dom.firstChild.dateValue).toDateString();
                    if (new Date(selectedDates["From"]) <= new Date(date) && new Date(selectedDates["To"]) >= new Date(date)) {
                        var getclass = item.dom.firstChild.getAttribute('class');
                        item.addCls('x-datepicker-selected');
                    }
                })
            },
            'afterrender': function () {
                var me = this;
                me.updateLayout();
                me.cells.each(function (item) {
                    var date = new Date(item.dom.firstChild.dateValue).toDateString();
                    if (new Date(selectedDates["From"]) <= new Date(date) && new Date(selectedDates["To"]) >= new Date(date)) {
                        var getclass = item.dom.firstChild.getAttribute('class');
                        item.addCls('x-datepicker-selected');
                    }
                })
            }
        }
    };

    var DateMenu = Ext.create('Ext.menu.Menu', {
        plain: true,
        floating: true,
        column: 1,
        width: 200,
        items: [myDatePicker, TimeFrom, TimeTo,
            {
                text: 'Generate',
                cls: 'no-icon-menu x-btn-default-small',
                iconCls: 'fa fa-sign-out fa-2x',
                style: { textAlign: 'center' },
                handler: function () {
                    
                    onGenerateBtnClick();
                }
            }

        ]
    });
    var VehicleMenu = Ext.create('Ext.menu.Menu', {
        plain: true,
        floating: true,
        layout: 'column',
        width: 270,
        items: [VehicleGrid,

           {
               text: 'Select All',
               // xtype:'button',
               cls: 'no-icon-menu x-btn-default-small',
               style: { textAlign: 'center', width: '50%' },
               hideOnClick: false,
               handler: function () {
                   VehicleGridselModel.selectAll();
                   allSelectedVehicle = true;
                   DeSelectedIndexVehicle = [];
                   DeselectedVehicleId = [];
                   selectAllVehicleFlag = false;
               }
           },

            {
                text: 'Deselect All',
                cls: 'no-icon-menu x-btn-default-small',
                style: { textAlign: 'center', width: '50%' },
                hideOnClick: false, handler: function () {
                    VehicleGridselModel.deselectAll();
                    selectedVehicle = [];
                    selectedVehicleId = [];
                    DeSelectedIndexVehicle = [];
                    DeselectedVehicleId = [];
                    selectAllVehicleFlag = false;
                    VehicleBtn.setText("Vehicle: <br/>" + 'Select a Vehicle');
                    defaultVehicleName = "";
                    allSelectedVehicle = false;
                }
            },
            '',
            
            {
                //generate Button for Fleet
                text: 'Generate',
                cls: 'no-icon-menu x-btn-default-small',
                style: { textAlign: 'center', width: '100%' },
                handler: function () {
                    selectedEvent = [];
                    selectedEventIndex = [];
                    //handler Code
                    if (typeof (selectedDates["To"]) == 'undefined') {
                        Ext.Msg.alert("Warning", 'Please select To  date');
                        return;
                    }
                    var str = TimeFrom.value;
                    var result = str.match(timetest);

                    if (result == null) {
                        Ext.Msg.alert("Warning", 'Invalid time format');
                        if (userTime.indexOf('A') > -1)
                            Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 12:00:00 AM');
                        else
                            Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 00:00:00');
                        SummaryGridStore.loadData([], false);
                        return;
                    }
                    str = TimeTo.value;
                    result = str.match(timetest);
                    if (result == null) {
                        Ext.Msg.alert("Warning", 'Invalid time format');
                        if (userTime.indexOf('A') > -1)
                            Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 12:00:00 AM');
                        else
                            Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 00:00:00');
                        SummaryGridStore.loadData([], false);
                        return;
                    }
                    if (DateFrom.rawValue + " " + TimeFrom.value == DateTo.rawValue + " " + TimeTo.value) {
                        Ext.Msg.alert("Warning", 'From datetime and To datetime can\'t be same');
                        SummaryGridStore.loadData([], false);
                        return;
                    }
                    var fleetids = "";
                    if (allSelected)
                        fleetids = ',-1,'
                    else {

                        Ext.each(selectedFleetId, function (val, index) {
                            fleetids += ',' + val;
                        });
                        fleetids += ',';
                    }
                    if (fleetids == ",") {
                        Ext.Msg.alert("Warning", 'Please select a fleet');
                        SummaryGridStore.loadData([], false);
                        return;
                    }
                    var vehicleids = "";
                    if (allSelectedVehicle) {
                        if (DeSelectedIndexVehicle.length == 0) {
                            vehicleids = ',-1,'
                            selectAllVehicleFlag = false;
                        }
                        else {
                            Ext.each(DeselectedVehicleId, function (val, index) {
                               vehicleids += val + ',';
                            });
                            vehicleids = vehicleids.substring(0, vehicleids.length - 1);
                            selectAllVehicleFlag = true;
                        }
                    }
                    else {
                        Ext.each(selectedVehicleId, function (val, index) {
                            vehicleids += ',' + val;
                        });
                        vehicleids += ',';
                        selectAllVehicleFlag = false;
                    }


                    if (vehicleids == ",") {
                        Ext.Msg.alert("Warning", 'Please select a Vehicle');
                        SummaryGridStore.loadData([], false);
                        return;
                    }
                    var eventids = ViolationboxGroup.getValue();
                    if (Object.keys(eventids).length == 0) {
                        Ext.Msg.alert("Warning", 'Please select a Event');
                        SummaryGridStore.loadData([], false);
                        return;
                    }


                    if (table == "Event") {
                        eventCol = '';
                    }
                    else {
                        violationCol = '';
                    }

                    for (var i = 0; i < SenchaColID.length; i++) {
                        if (Ext.getCmp(SenchaColID[i].toString().trim()).hidden == false) {
                            if (table == "Event") {
                                eventCol += ((DatabaseColID[i]).toString() + ',');
                            }
                            else {
                                violationCol += ((DatabaseColID[i]).toString() + ',');
                            }

                        }
                    }

                    ShowHideColumnPreference();
                    loadingMask.show();
                    SummaryGridStore.load(
                        {
                            params:
                            {
                                Action: table,
                                FromDB: 'true',
                                VehicleIds: vehicleids,
                                startDate: DateFrom.rawValue + " " + TimeFrom.value,
                                EndDate: DateTo.rawValue + " " + TimeTo.value,
                                Events: ViolationboxGroup.getValue(),
                                Search: Ext.getCmp('searchField').getValue(),
                                Columns: searchColumns,
                                SortBy: sortingParam,
                                start: 0,
                                limit: Pagesize,
                                FirstTime: 'true'
                                , SelectAllVehicleFlag: selectAllVehicleFlag
                            },
                            callback: function (records, operation, success) {
                                if (records != null) {
                                    SummaryGridStore.totalCount = SummaryGridStore.totalCount;
                                    SummaryGridStore.currentPage = 1;
                                    DataPresent = 1;
                                    onlyOnce = 0;
                                    count = '1';
                                    st = 1;
                                    currentPg = 1;
                                    if (currentPg == 1 && st == 1)
                                    { SummaryGridPager.doRefresh(); st = 0; }
                                    //Ext.getCmp('loadDataBuffer').show();
                                    Ext.getCmp('MoreRecordsGenerate').hide();
                                    if (SummaryGridStore.totalCount >= recordsToFetch) {
                                        Ext.getCmp('MoreRecordsGenerate').show();
                                    }

                                }
                                else {
                                    DataPresent = 0;
                                    SummaryGridStore.totalCount = 0;
                                    SummaryGridStore.currentPage = 0;
                                }

                                loadingMask.hide();
                                if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                    Ext.Msg.alert("Warning", operation.resultSet.message);
                                    if (records == null)
                                        SummaryGridStore.loadData([], false);
                                }
                                if (operation.exception && operation.error.statusText == "communication failure") {
                                    Ext.Msg.alert("Warning", "Server timeout");
                                    if (records == null)
                                        SummaryGridStore.loadData([], false);
                                }
                            }
                        });
                }
            }
        ]


    });
    var FleetMenu = Ext.create('Ext.menu.Menu', {
        plain: true,
        floating: true,
        layout: 'column',
        width: 270,
        items: [FleetGrid,

           {
               text: 'Select All',
               // xtype:'button',
               cls: 'no-icon-menu x-btn-default-small',
               style: { textAlign: 'center', width: '50%' },
               hideOnClick: false,
               handler: function () {
                   FleetGridselModel.selectAll();
                   allSelected = true;
                   DeSelectedIndex = [];
                   SelectedFleetNames = [];
                   bFleetSelectionMode = DeSelect;
                   setFleetButtonText();
               }
           },

            {
                text: 'Deselect All',
                cls: 'no-icon-menu x-btn-default-small',
                style: { textAlign: 'center', width: '50%' },
                hideOnClick: false, handler: function () {
                    FleetGridselModel.deselectAll();
                    selectedFleet = [];
                    selectedFleetId = [];
                    DeSelectedIndex = [];
                    FleetBtn.setText("Fleet: <br/>" + 'Select a Fleet');
                    defaultFleetName = "";
                    allSelected = false;
                    SelectedFleetNames = [];
                    bFleetSelectionMode = Select;
                    setFleetButtonText();
                }
            },
            '',
            {
                //Ok Button for Vehicle
                text: 'OK',
                cls: 'no-icon-menu x-btn-default-small',
                style: { textAlign: 'center', width: '50%' },
                handler: function () {
                    selectedEvent = [];
                    selectedEventIndex = [];
                    //handler Code 
                    allSelectedVehicle = false;
                    selectedVehicleId = [];
                    selectedVehicle = [];
                    DeselectedVehicleId = [];
                    selectAllVehicleFlag = false;
                    
                    var fleetids = "";

                    //  If De-Selected Fleet IDs are to be sent to Web Method
                    if (bFleetSelectionMode == DeSelect) {

                        if (DeSelectedFleetId.length == 0)
                            fleetids = ',-1,';

                        else {
                            Ext.each(DeSelectedFleetId, function (val, index) {
                                fleetids += ',' + val;
                            });

                            fleetids += ',';
                        }

                    }

                        //  Else, If Selected Fleet IDs are to be sent to Web Method. Server side has the logic to get actual FleetIDs 
                    else if (bFleetSelectionMode == Select) {

                        if (allSelected && DeSelectedFleetId.length == 0)
                            fleetids = ',-1,';
                        else {

                            Ext.each(selectedFleetId, function (val, index) {
                                fleetids += ',' + val;
                            });

                            fleetids += ',';
                        }

                    }

                    if (fleetids == ",") {
                        Ext.Msg.alert("Warning", 'Please select a fleet');
                        SummaryGridStore.loadData([], false);
                        return;
                    }
                   
                    VehicleStore.load(
                    {
                        params:
                            {
                                start: 0,
                                limit: VehiclePagesize,
                                Fleet: fleetids,
                                FleetFirstTime: true,
                                ExcludeFleetIDs: bFleetSelectionMode
                        
                            },
                        callback: function (records, operation, success) {
                   
                            if (records != null) {
                                VehicleStore.totalCount = VehicleStore.totalCount;
                                VehicleStore.currentPage = 1;
                                VehicleBtn.setText("Vehicle: <br/>" + 'Select a Vehicle');
                        
                            }
                            else {
                        
                                VehicleStore.totalCount = 0;
                                VehicleStore.currentPage = 0;
                            }

                    
                            if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                Ext.Msg.alert("Warning", operation.resultSet.message);
                                if (records == null)
                                    VehicleStore.loadData([], false);
                            }
                            if (operation.exception && operation.error.statusText == "communication failure") {
                                Ext.Msg.alert("Warning", "Server timeout");
                                if (records == null)
                                    VehicleStore.loadData([], false);
                            }

                        }
                    });
                   

                    
                }
            },
            {
                //generate Button for Fleet
                text: 'Generate',
                cls: 'no-icon-menu x-btn-default-small',
                style: { textAlign: 'center', width: '50%' },
                handler: function () {
                    
                    onGenerateBtnClick();
                }
            }
        ]


    });

    var TableGroup = new Ext.form.CheckboxGroup({
        id: 'TableGroup',
        xtype: 'checkboxgroup',
        width: 160,
        border: true,
        style: {
            overflow: "auto"
        },
        columns: 1,
        items: [{
            boxLabel: 'Events',
            name: 'Events',
            inputValue: '1',
            id: 'checkbox1',
            style: { marginLeft: '3px' }
        }, {
            boxLabel: 'Violations',
            name: 'Violations',
            style: { marginTop: '10px' },
            inputValue: '2',
            checked: true,
            id: 'checkbox2',
            style: { marginLeft: '3px' }
        }],
        listeners:
        {
            change: function (field, newValue, oldValue, eOpts) {
                if (Object.keys(oldValue).length == 1) {
                    if (oldValue["Events"] == "1") {
                        table = 'Violation';
                       
                        Ext.getCmp('SummaryGrid').columns[1].setVisible(true);
                        Ext.getCmp('checkbox1').setValue(0);
                    }
                    else if (oldValue["Violations"] == "2") {
                        table = 'Event';
                        Ext.getCmp('SummaryGrid').columns[1].setVisible(true);
                        
                        Ext.getCmp('checkbox2').setValue(0);
                    }
                }
                if (table == undefined)
                    table = 'select an Event Category';
                TableBtn.setText("Event Category: <br/>" + table);


                ViolationStore.load({
                    params:
            {
                Action: table
            },
                    callback: function (records, operation, success) {
                        firstLoad();
                    }
                });

            },

        }

    });
    var TableChkboxList = Ext.create('Ext.form.Panel', {
        width: 210,
        height: 70,
        style: { marginLeft: '2px' },
        autoScroll: true,
        bodyPadding: 2,
        items: [TableGroup]
    });
    var TableMenu = Ext.create('Ext.menu.Menu', {
        plain: true,
        floating: true,
        layout: 'column',
        width: 170,
        items: [TableChkboxList]
    });

   
    VisiblecolumnsboxGroup = new Ext.form.CheckboxGroup({
       id: 'VisiblecolumnsboxGroup',
        xtype: 'checkboxgroup',
        border: true,
        columns: 1,
        vertical: true,
        items: [],
        columns: 1
    });
   
    ViolationboxGroup = new Ext.form.CheckboxGroup({
        id: 'ViolationboxGroup',
        xtype: 'checkboxgroup',
        border: true,

        columns: 1,
        listeners:
        {
            change: function (field, newValue, oldValue, eOpts) {
                EventBtn.setText(table + ": <br/>" + Object.keys(newValue).length + "/" + ViolationboxGroup.getBoxes().length);
            }
        }
    });
    var ViolationStore = Ext.create('Ext.data.Store', {
        model: 'EventModel',
        autoLoad: false,
        storeId: 'ViolationStore',
        listeners:
        {
            'load': function (store, records, options) {
                if (records == null)
                    return;
                var i = 0;
                // var sz = ViolationboxGroup.getSize();
                ViolationboxGroup.removeAll();
                for (var i = 0; i < records.length; i++) {
                    var cb = Ext.create('Ext.form.field.Checkbox', {
                        boxLabel: records[i].data.Description,
                        inputValue: records[i].data.EventTypeID,
                        id: records[i].data.EventTypeID
                        
                    }
                    );
                   
                    try {
                        ViolationboxGroup.add(cb);
                    }
                    catch (err) {
                        alert(err)
                    }

                }
                if (defaultSelectedEvents == 'all') {

                    var checkBoxes = ViolationboxGroup.getBoxes();
                    
                    Ext.Array.each(checkBoxes, function (checkbox) {
                        checkbox.setValue(1);
                    });
                    Ext.Array.each(checkBoxes, function (checkbox) {
                        checkbox.setValue(0);
                    });
                }
                else {
                    var checkBoxes = FleetboxGroup.getBoxes();
                    Ext.Array.each(checkBoxes, function (checkbox) {
                        if (defaultSelectedEvents.indexOf(checkbox.id) > -1)
                            checkbox.setValue(1);
                    });
                }
            }
        },
        proxy:
           {
               type: 'ajax',
               url: './Event.aspx?QueryType=GetViolationList',
               timeout: proxyTimeOut,
               reader:
              {
                  type: 'xml',
                  root: 'NewDataSet',
                  record: 'Table'
              }
           }
    });

   
    var VisibleColumnsChkboxList = Ext.create('Ext.form.Panel', {
        width: 210,
        height: 290,
        style: { marginLeft: '2px' },
        autoScroll: true,
        bodyPadding: 2,      
        items: [VisiblecolumnsboxGroup, 
        ]
    });
   
    var ViolationChkboxList = Ext.create('Ext.form.Panel', {
        width: 210,
        height: 220,
        style: { marginLeft: '2px' },
        autoScroll: true,
        bodyPadding: 2,
        items: [ViolationboxGroup]
    });
    var EventMenu = Ext.create('Ext.menu.Menu', {
        plain: true,
        floating: true,
        layout: 'column',
        width: 220,
        items: [ViolationChkboxList,
            {
                //xtype:'button',
                //cls: 'fa fa-print',
                text: 'Select All',
                cls: 'no-icon-menu x-btn-default-small cmbfonts',
                style: { textAlign: 'center', width: '50%' },
                hideOnClick: false,
                handler: function () {
                    var chkboxs = ViolationboxGroup.getBoxes()
                    Ext.Array.each(chkboxs, function (chkbox) {
                        chkbox.setValue(1);
                    });
                }
            },

            {
                text: 'Deselect All',
                //cls: 'startbutton',
                cls: 'no-icon-menu x-btn-default-small',
                style: { textAlign: 'center', width: '50%' },
                hideOnClick: false,
                handler: function () {
                    var chkboxs = ViolationboxGroup.getBoxes()
                    Ext.Array.each(chkboxs, function (chkbox) {
                        chkbox.setValue(0);
                    });
                }
            },
            {
                //generate Button for dateTime
                text: 'Generate',
                cls: 'no-icon-menu x-btn-default-small',
                style: { textAlign: 'center', width: '100%' },
                handler: function () {
                    
                    onGenerateBtnClick();

                    }
                }           
        ]
    });
    DateBtn = Ext.create('Ext.Button', {
        text: 'From: ',

        width: 100,
        cls: '  x-btn-over-calender  x-btn-split-right-calender menutBtn  ',
        handler: function () { },
        menu: DateMenu
    });
    VehicleBtn = Ext.create('Ext.Button', {
        text: 'Vehicle: <br/> &nbsp;',
        width: 100,
        cls: '  x-btn-over-transport  x-btn-split-right-transport menutBtn',
        handler: function () {
            if (LoadVehiclesBasedOn != 'fleet') {
                //selectedEvent = [];
                //selectedEventIndex = [];
                //handler Code 
                //allSelectedVehicle = false;
                //selectedVehicleId = [];
                //selectedVehicle = [];
                //DeselectedVehicleId = [];
                //selectAllVehicleFlag = false;

                var fleetids = "";
                if (allSelected)
                    fleetids = ',-1,'
                else {

                Ext.each(selectedFleetId, function (val, index) {
                    fleetids += ',' + val;
                });
                fleetids += ',';
                }
                if (fleetids == ",") {
                    Ext.Msg.alert("Warning", 'Please select a fleet');
                    SummaryGridStore.loadData([], false);
                    return;
                }

                VehicleStore.load(
        {
            params:
                {
                    start: 0,
                    limit: VehiclePagesize,
                    Fleet: fleetids,
                    FleetFirstTime: true,
                    ExcludeFleetIDs: bFleetSelectionMode
                },
            callback: function (records, operation, success) {

                if (records != null) {
                    VehicleStore.totalCount = VehicleStore.totalCount;
                    VehicleStore.currentPage = 1;
                    if (selectedVehicleId.length > 1)
                        VehicleBtn.setText("Vehicle: <br/>" + 'Multiple Vehicle');
                    else if (selectedVehicleId.length == 1) {
                        var row = VehicleGrid.getSelectionModel().getSelection()[0];
                        VehicleBtn.setText("Vehicle: <br/>" + row.get('VehicleName'));
                    }
                    else                       
                        VehicleBtn.setText("Vehicle: <br/>" + 'Select a Vehicle');
                }
                else {

                    VehicleStore.totalCount = 0;
                    VehicleStore.currentPage = 0;
                }


                if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                    Ext.Msg.alert("Warning", operation.resultSet.message);
                    if (records == null)
                        VehicleStore.loadData([], false);
                }
                if (operation.exception && operation.error.statusText == "communication failure") {
                    Ext.Msg.alert("Warning", "Server timeout");
                    if (records == null)
                        VehicleStore.loadData([], false);
                }

            }
        });
            }
        },
        menu: VehicleMenu
    });
    FleetBtn = Ext.create('Ext.Button', {
        text: 'Fleet: <br/> &nbsp;',
        

        width: 100,
        cls: '  x-btn-over-transport  x-btn-split-right-transport menutBtn',
        //listeners: {
        //    arrowClick: function () {
        handler: function () {

            if (LoadVehiclesBasedOn != 'fleet') {
                this.menu.hide();
                try {
                    var mypage = './Widgets/OrganizationHierarchy.aspx?nodecode=' + DefaultOrganizationHierarchyNodeCode + '&loadVehicle=0';
                    if (MutipleUserHierarchyAssignment) {
                        mypage = mypage + "&m=1&f=0&rootNodecode=";
                    }
                    var myname = 'OrganizationHierarchy';
                    var w = 740;
                    var h = 440;
                    var winl = (screen.width - w) / 2;
                    var wint = (screen.height - h) / 2;
                    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
                    win = window.open(mypage, myname, winprops)
                    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
                    return false;
                }
                catch (err) {
                }
            }
        },
        //},
        menu: FleetMenu
    });
    var TableBtn = Ext.create('Ext.Button', {
        text: 'Event Category: <br/> &nbsp;',
        cls: 'x-btn-over-operation  x-btn-split-right-operation menutBtn',
        width: 120,
        config: {
            ui: 'plain',
            style: 'background-color:transparent;'
        },
        handler: function () { },
        menu: TableMenu
    });
    var EventBtn = Ext.create('Ext.Button', {
        text: 'Events: <br/> &nbsp;',
        width: 100,
        cls: ' cmbfonts x-btn-over-event  x-btn-split-right-event menutBtn  ',
        handler: function () { },
        menu: EventMenu
    });

    function onGenerateBtnClick() {

        selectedEvent = [];
        selectedEventIndex = [];
        if (typeof (selectedDates["To"]) == 'undefined') {
            Ext.Msg.alert("Warning", 'Please select To  date');
            return;
        }
        var str = TimeFrom.value;
        var result = str.match(timetest);
        if (result == null) {
            Ext.Msg.alert("Warning", 'Invalid time format');
            if (userTime.indexOf('A') > -1)
                Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 12:00:00 AM');
            else
                Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 00:00:00');
            SummaryGridStore.loadData([], false);
            return;
        }
        str = TimeTo.value;
        result = str.match(timetest);
        if (result == null) {
            Ext.Msg.alert("Warning", 'Invalid time format');
            if (userTime.indexOf('A') > -1)
                Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 12:00:00 AM');
            else
                Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 00:00:00');
            SummaryGridStore.loadData([], false);
            return;
        }
        if (DateFrom.rawValue + " " + TimeFrom.value == DateTo.rawValue + " " + TimeTo.value) {
            Ext.Msg.alert("Warning", 'From datetime and To datetime can\'t be same');
            SummaryGridStore.loadData([], false);
            return;
        }

        var vehicleids = "";
        if (allSelectedVehicle) {
            if (DeSelectedIndexVehicle.length == 0) {
                vehicleids = ',-1,'
                selectAllVehicleFlag = false;
            }
            else {
                Ext.each(DeselectedVehicleId, function (val, index) {
                    vehicleids += val + ',';
                });
                vehicleids = vehicleids.substring(0, vehicleids.length - 1);
                selectAllVehicleFlag = true;
            }
        }
        else {
            Ext.each(selectedVehicleId, function (val, index) {
                vehicleids += ',' + val;
            });
            vehicleids += ',';
            selectAllVehicleFlag = false;
        }


        if (vehicleids == ",") {
            Ext.Msg.alert("Warning", 'Please select a Vehicle');
            SummaryGridStore.loadData([], false);
            return;
        }

        var eventids = ViolationboxGroup.getValue();
        if (Object.keys(eventids).length == 0) {
            Ext.Msg.alert("Warning", 'Please select a Event');
            SummaryGridStore.loadData([], false);
            return;
        }

        if (table == "Event") {
            eventCol = '';
        }
        else {
            violationCol = '';
        }

        for (var i = 0; i < SenchaColID.length; i++) {
            if (Ext.getCmp(SenchaColID[i].toString().trim()).hidden == false) {
                if (table == "Event") {
                    eventCol += ((DatabaseColID[i]).toString() + ',');
                }
                else {
                    violationCol += ((DatabaseColID[i]).toString() + ',');
                }

            }
        }

        //show hide column as per user preference
        ShowHideColumnPreference();

        loadingMask.show();
        SummaryGridStore.load(
                        {
                            params:
                            {
                                Action: table,
                                FromDB: 'true',
                                VehicleIds: vehicleids,
                                startDate: DateFrom.rawValue + " " + TimeFrom.value,
                                EndDate: DateTo.rawValue + " " + TimeTo.value,
                                Events: ViolationboxGroup.getValue(),
                                Search: Ext.getCmp('searchField').getValue(),
                                Columns: searchColumns,
                                SortBy: sortingParam,
                                start: 0,
                                limit: Pagesize,
                                FirstTime: 'true',
                                SelectAllVehicleFlag: selectAllVehicleFlag
                            },
                            callback: function (records, operation, success) {

                                if (records != null) {
                                    SummaryGridStore.totalCount = SummaryGridStore.totalCount;
                                    SummaryGridStore.currentPage = 1;
                                    DataPresent = 1;
                                    onlyOnce = 0;
                                    count = '1';
                                    st = 1;
                                    currentPg = 1;
                                    if (currentPg == 1 && st == 1)
                                    { SummaryGridPager.doRefresh(); st = 0; }
                                    Ext.getCmp('MoreRecordsGenerate').hide();
                                    if (SummaryGridStore.totalCount >= recordsToFetch) {
                                        Ext.getCmp('MoreRecordsGenerate').show();
                                    }


                                    //Ext.getCmp('loadDataBuffer').show();



                                }
                                else {
                                    DataPresent = 0;
                                    SummaryGridStore.totalCount = 0;
                                    SummaryGridStore.currentPage = 0;
                                }

                                loadingMask.hide();
                                if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                    Ext.Msg.alert("Warning", operation.resultSet.message);
                                    if (records == null)
                                        SummaryGridStore.loadData([], false);
                                }
                                if (operation.exception && operation.error.statusText == "communication failure") {
                                    Ext.Msg.alert("Warning", "Server timeout");
                                    if (records == null)
                                        SummaryGridStore.loadData([], false);
                                }
                            }

            });

    };

    var GenerateBtn = Ext.create('Ext.Button', {
        textAlign: 'center',
        cls: 'generateBtn fa fa-download fa-2x',
        height: 36,
        handler: function () {

            onGenerateBtnClick();
        }
    });

    var themeMenu = Ext.create('Ext.menu.Menu', {
        items: [{
            text: 'Theme 1',
            id: 'ssgray1',
            cls: 'cmbfonts',
            textAlign: 'left',
            handler: function () {
                Ext.util.CSS.swapStyleSheet('ssgray', "Scripts/css/" + "EventStyle.css");
            }
        },
        {
            text: 'Theme 2',
            id: 'ssgray2',
            cls: 'cmbfonts',
            textAlign: 'left',
            handler: function () {
                Ext.util.CSS.swapStyleSheet('ssgray', "Scripts/css/" + "EventStyle2.css");
            }
        },
         {
             text: 'Theme 3',
             id: 'ssgray3',
             cls: 'cmbfonts',
             textAlign: 'left',
             handler: function () {
                 Ext.util.CSS.swapStyleSheet('ssgray', "Scripts/css/" + "EventStyle3.css");
             }
         },
         {
             text: 'Theme 4',
             id: 'ssgray4',
             cls: 'cmbfonts',
             textAlign: 'left',
             handler: function () {
                 Ext.util.CSS.swapStyleSheet('ssgray', "Scripts/css/" + "EventStyle4.css");
             }
         },
         {
             text: 'Theme 5',
             id: 'ssgray5',
             cls: 'cmbfonts',
             textAlign: 'left',
             handler: function () {
                 Ext.util.CSS.swapStyleSheet('ssgray', "Scripts/css/" + "EventStyle5.css");
             }
         }
        ]
    });
    var exportMenu = Ext.create('Ext.menu.Menu', {
        items: [{
            text: 'Export To CSV',
            id: 'exportToCsvButton',
            iconCls: 'map',

            cls: 'cmbfonts',
            textAlign: 'left',
            handler: function () {
                var selectedRecord = "";
                for (var prop in selectedEventIndex) {
                    selectedRecord += selectedEventIndex[prop] + ",";
                }
                var columnsp = "";
                Ext.each(SummaryGrid.columns, function (col, index) {
                    if (index == 0)
                        return;
                    else if (col.hidden == false)
                        columnsp += col.text + ':' + col.dataIndex + ',';
                });
                columnsp = columnsp.substring(0, columnsp.length - 1);
                var postdata = { selected: selectedRecord, SortBy: sortingParam.property + " " + sortingParam.direction, SearchBy: Ext.getCmp('searchField').getValue(), searchIn: searchColumns, action: 'Export', format: 'CSV', columnsList: columnsp, HeaderChecked: headerChecked, tableType: table };
                var form = Ext.create('Ext.form.Panel', {
                    xtype: 'form',
                    itemId: 'uploadForm',
                    standardSubmit: true,
                    method: 'POST',
                    url: './Event.aspx?QueryType=shareDatatable'
                });
                form.getForm().submit({ params: postdata });
            }
        },
        {
            text: 'Export To Excel2003',
            id: 'exportToExcel2003Button',
            iconCls: 'map',
            cls: 'cmbfonts',
            textAlign: 'left',
            handler: function () {
                var selectedRecord = "";
                for (var prop in selectedEventIndex) {
                    selectedRecord += selectedEventIndex[prop] + ",";
                }
                var columnsp = "";
                Ext.each(SummaryGrid.columns, function (col, index) {
                    if (index == 0)
                        return;
                    else if (col.hidden == false)
                        columnsp += col.text + ':' + col.dataIndex + ',';
                });
                columnsp = columnsp.substring(0, columnsp.length - 1);
                var postdata = { selected: selectedRecord, SortBy: sortingParam.property + " " + sortingParam.direction, SearchBy: Ext.getCmp('searchField').getValue(), searchIn: searchColumns, action: 'Export', format: 'Excel2003', columnsList: columnsp, HeaderChecked: headerChecked, tableType: table };
                var form = Ext.create('Ext.form.Panel', {
                    xtype: 'form',
                    itemId: 'uploadForm',
                    standardSubmit: true,
                    method: 'POST',
                    url: './Event.aspx?QueryType=shareDatatable'
                });
                form.getForm().submit({ params: postdata });
            }
        },
        {
            text: 'Export To Excel2007',
            id: 'exportToExcel2007Button',
            iconCls: 'map',
            cls: 'cmbfonts',
            textAlign: 'left',
            handler: function () {
                var selectedRecord = "";
                for (var prop in selectedEventIndex) {
                    selectedRecord += selectedEventIndex[prop] + ",";
                }
                var columnsp = "";
                Ext.each(SummaryGrid.columns, function (col, index) {
                    if (index == 0)
                        return;
                    else if (col.hidden == false)
                        columnsp += col.text + ':' + col.dataIndex + ',';
                });
                columnsp = columnsp.substring(0, columnsp.length - 1);
                var postdata = { selected: selectedRecord, SortBy: sortingParam.property + " " + sortingParam.direction, SearchBy: Ext.getCmp('searchField').getValue(), searchIn: searchColumns, action: 'Export', format: 'Excel2007', columnsList: columnsp, HeaderChecked: headerChecked, tableType: table };
                var form = Ext.create('Ext.form.Panel', {
                    xtype: 'form',
                    itemId: 'uploadForm',
                    standardSubmit: true,
                    method: 'POST',
                    url: './Event.aspx?QueryType=shareDatatable'
                });
                form.getForm().submit({ params: postdata });
            }
        },
        {
            text: 'Export To PDF',
            id: 'exportToEPDFButton',
            iconCls: 'map',
            cls: 'cmbfonts',
            textAlign: 'left',
            handler: function () {
                var selectedRecord = "";
                for (var prop in selectedEventIndex) {
                    selectedRecord += selectedEventIndex[prop] + ",";
                }
                var columnsp = "";
                Ext.each(SummaryGrid.columns, function (col, index) {
                    if (index == 0)
                        return;
                    else if (col.hidden == false)
                        columnsp += col.text + ':' + col.dataIndex + ',';
                });
                columnsp = columnsp.substring(0, columnsp.length - 1);
                var postdata = { selected: selectedRecord, SortBy: sortingParam.property + " " + sortingParam.direction, SearchBy: Ext.getCmp('searchField').getValue(), searchIn: searchColumns, action: 'Export', format: 'PDF', columnsList: columnsp, HeaderChecked: headerChecked, tableType: table };
                var form = Ext.create('Ext.form.Panel', {
                    xtype: 'form',
                    itemId: 'uploadForm',
                    standardSubmit: true,
                    method: 'POST',
                    url: './Event.aspx?QueryType=shareDatatable'
                });
                form.getForm().submit({ params: postdata });
            }
        }]
    });
    var formatCombo = new Ext.form.ComboBox({
        fieldLabel: 'Format',
        hiddenName: 'Format',
        store: new Ext.data.SimpleStore({
            data: [
                ['CSV', 'CSV'],
                ['Excel2003', 'Excel2003'],
                ['Excel2007', 'Excel2007'],
                ['PDF', 'PDF']
            ],
            id: 'formatcombo',
            fields: ['value', 'text']
        }),
        valueField: 'value',
        displayField: 'text',
        triggerAction: 'all',
        editable: false,
        listeners:
            {
                'afterrender': function () {
                    this.setValue('CSV');
                }
            }
    });
   

    var SearchForm = Ext.create('Ext.form.Panel', {
        labelWidth: 50,
        id: 'searchFrm',
        //height: '100px',
        baseCls: 'searchBar',
        width: window.screen.width - 20,
        layout: 'column',
        defaults: {
            labelWidth: 90
        },
        defaultType: 'textfield',
        items: [
            //{

            //    xtype: 'button',
            //    id: 'ToggleBtn',
            //    cls: 'fa fa-th fa-2x eventBtn',
            //    menu: themeMenu

            //},
                {
                    xtype: 'label',
                    id: 'eventLabel',
                    text: 'Events',
                    cls: 'eventLabel'
                },
                {
                    xtype: 'textfield',
                    name: 'searchField',
                    id: 'searchField',
                    hideLabel: true,
                    width: 250,
                    height: 28,
                    borderRadius: 25,
                    emptyText: 'Search',
                    cls: 'textarea searchTextfield',
                    listeners: {
                        specialkey: function (field, event, options) {
                            if (event.getKey() == event.ENTER) {
                                var selcb = Ext.getCmp('SGridSearchMenuCBG').getValue();
                                var selcolumns = '';
                                if (Object.keys(selcb).length > 0) {
                                    for (var prop in selcb) {
                                        if (prop.indexOf('Details') > -1)
                                            selcolumns += "Details,";
                                        else
                                            selcolumns += selcb[prop] + ",";
                                    }
                                }
                                searchColumns = selcolumns;
                                loadingMask.show();
                                var fleetids = "";
                                //if (allSelected)
                                //    fleetids = ',-1,'
                                //else {

                                    Ext.each(selectedFleetId, function (val, index) {
                                        fleetids += ',' + val;
                                    });
                                    fleetids += ',';
                                //}
                                    var vehicleids = "";
                                    if (allSelectedVehicle) {
                                        if (DeSelectedIndexVehicle.length == 0) {
                                            vehicleids = ',-1,'
                                            selectAllVehicleFlag = false;
                                        }
                                        else {
                                            Ext.each(DeselectedVehicleId, function (val, index) {
                                               vehicleids += val + ',';
                                            });
                                            vehicleids = vehicleids.substring(0, vehicleids.length - 1);
                                            selectAllVehicleFlag = true;
                                        }
                                    }
                                    else {
                                        Ext.each(selectedVehicleId, function (val, index) {
                                            vehicleids += ',' + val;
                                        });
                                        vehicleids += ',';
                                        selectAllVehicleFlag = false;
                                    }
           
                                if (vehicleids == ",") {
                                    Ext.Msg.alert("Warning", 'Please select a Vehicle');
                                    SummaryGridStore.loadData([], false);
                                    return;
                                }
                                
                                SummaryGridStore.load(
                                    {

                                        params:
                                        {
                                            Action: table,
                                            VehicleIds: vehicleids,
                                            startDate: DateFrom.rawValue + " " + TimeFrom.value,
                                            EndDate: DateTo.rawValue + " " + TimeTo.value,
                                            Events: ViolationboxGroup.getValue(),
                                            Search: field.value,
                                            Columns: searchColumns,
                                            SortBy: sortingParam,
                                            start: SummaryGridStore.currentPage - 1,
                                            limit: Pagesize,
                                            SelectAllVehicleFlag : selectAllVehicleFlag

                                        },
                                        callback: function (records, operation, success) {
                                            if (field.value != null && field.value != "") {
                                                if (records != null) {
                                                    SummaryGridStore.totalCount = SummaryGridStore.totalCount;
                                                    SummaryGridStore.currentPage = 1;
                                                }
                                                else {
                                                    SummaryGridStore.totalCount = 0;
                                                    SummaryGridStore.currentPage = 0;
                                                }
                                            }
                                            SummaryGridPager.bindStore(SummaryGridStore);
                                            SummaryGridPager.onLoad();
                                            loadingMask.hide();
                                            if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                                Ext.Msg.alert("Warning", operation.resultSet.message);
                                                if (records == null)
                                                    SummaryGridStore.loadData([], false);
                                            }

                                        }
                                    });
                                
                            }
                        }
                    }
                },
             {
                 xtype: 'button',
                 id: 'searchbtn',
                 cls: 'searchButton fa fa-search fa-2x',
                 menu: new Ext.menu.Menu({
                     id: 'SGridSearchMenuitems',
                     plain: true,
                     items: [{
                         xtype: 'checkboxgroup',
                         id: 'SGridSearchMenuCBG',
                         columns: 1,
                         vertical: true,
                         items: []
                     }]
                 })
             },

        {
            xtype: 'button',
            cls: 'fa fa-file-text fa-2x eventBtn',
            menu: exportMenu
        },
                 {
                     xtype: 'button',
                     // text: 'Print',
                     id: 'Printbtn',
                     cls: 'fa fa-print fa-2x eventBtn',
                     

                     listeners: {
                         click: function () {
                             var selectedRecord = "";
                             for (var prop in selectedEventIndex) {
                                 selectedRecord += selectedEventIndex[prop] + ",";
                             }
                             var columnsp = "";
                             Ext.each(SummaryGrid.columns, function (col, index) {
                                 if (index == 0)
                                     return;
                                 else if (col.hidden == false)
                                     columnsp += col.text + ':' + col.dataIndex + ',';
                             });
                             columnsp = columnsp.substring(0, columnsp.length - 1);
                             var postdata = { selected: selectedRecord, SortBy: sortingParam.property + " " + sortingParam.direction, SearchBy: Ext.getCmp('searchField').getValue(), searchIn: searchColumns, action: 'Print', format: 'PDF', columnsList: columnsp, HeaderChecked: headerChecked, tableType: table };
                             Ext.Ajax.request({
                                 url: './Event.aspx?QueryType=shareDatatable',
                                 params: postdata,
                                 success: function (response) {
                                     var text = response.responseText;
                                     if (text.indexOf('Failed') == -1) {
                                         window.open('./TempReports/' + text, "_blank");
                                     }
                                     else {
                                         Ext.Msg.alert("Message", text);
                                     }
                                 }
                             });
                         }
                     }
                 },
                 {
                     xtype: 'button',
                     //text: 'Email',
                     id: 'Emailbtn',
                     cls: 'eventBtn fa fa-envelope fa-2x',
                     menu: new Ext.menu.Menu({
                         id: 'SGridEmailMenuitems',
                         plain: true,
                         items: [formatCombo, {
                             xtype: 'textfield',
                             name: 'EmailAddress',
                             id: 'EmailAddress',
                             fieldLabel: 'Email Address',
                             width: 270,
                             emptyText: 'Example:jone@bsmwireless.com'
                         },
                         {
                             xtype: 'button',
                             text: 'Send',
                             id: 'Sendbtn',
                             listeners: {
                                 click: function () {
                                     var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                                     if (re.test(Ext.getCmp("EmailAddress").getValue())) {
                                         var selectedRecord = "";
                                         for (var prop in selectedEventIndex) {
                                             selectedRecord += selectedEventIndex[prop] + ",";
                                         }
                                         var columnsp = "";
                                         Ext.each(SummaryGrid.columns, function (col, index) {
                                             if (index == 0)
                                                 return;
                                             else if (col.hidden == false)
                                                 columnsp += col.text + ':' + col.dataIndex + ',';
                                         });
                                         columnsp = columnsp.substring(0, columnsp.length - 1);
                                         var postdata = { selected: selectedRecord, SortBy: sortingParam.property + " " + sortingParam.direction, SearchBy: Ext.getCmp('searchField').getValue(), searchIn: searchColumns, action: 'Email', format: formatCombo.getValue(), address: Ext.getCmp('EmailAddress').getValue(), columnsList: columnsp,  tableType: table };
                                         sendingMask.show();
                                         Ext.Ajax.request({
                                             url: './Event.aspx?QueryType=shareDatatable',
                                             params: postdata,
                                             success: function (response) {
                                                 var text = response.responseText;
                                                 Ext.Msg.alert("Message", text);
                                                 sendingMask.hide();
                                             },
                                             failure: function (response) {
                                                 sendingMask.hide();
                                             }
                                         });
                                     }
                                     else
                                         alert('Invalid Email');
                                 }
                             }
                         }]
                     }),
                     listeners: {
                         click: function () { }
                     }
                 }


                 , {

                     xtype: 'button',
                     //text: 'Schedule',
                     id: 'Schedulebtn',
                     cls: 'fa fa-clock-o fa-2x eventBtn',
                     listeners: {
                         click: function () {
                             //handler Code
                             if (typeof (selectedDates["To"]) == 'undefined') {
                                 Ext.Msg.alert("Warning", 'Please select To  date');
                                 return;
                             }
                             var str = TimeFrom.value;
                             var result = str.match(timetest);
                             if (result == null) {
                                 Ext.Msg.alert("Warning", 'Invalid time format');
                                 if (userTime.indexOf('A') > -1)
                                     Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 12:00:00 AM');
                                 else
                                     Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 00:00:00');
                                 SummaryGridStore.loadData([], false);
                                 return;
                             }
                             str = TimeTo.value;
                             result = str.match(timetest);
                             if (result == null) {
                                 Ext.Msg.alert("Warning", 'Invalid time format');
                                 if (userTime.indexOf('A') > -1)
                                     Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 12:00:00 AM');
                                 else
                                     Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 00:00:00');
                                 SummaryGridStore.loadData([], false);
                                 return;
                             }
                             if (DateFrom.rawValue + " " + TimeFrom.value == DateTo.rawValue + " " + TimeTo.value) {
                                 Ext.Msg.alert("Warning", 'From datetime and To datetime can\'t be same');
                                 SummaryGridStore.loadData([], false);
                                 return;
                             }
                             
                             var fleetids = "";
                             if (allSelected)
                                 fleetids = ',-1,'
                             else {

                                 Ext.each(selectedFleetId, function (val, index) {
                                     fleetids += ',' + val;
                                 });
                                 fleetids += ',';
                             }
                             if (fleetids == ",") {
                                 Ext.Msg.alert("Warning", 'Please select a fleet');
                                 SummaryGridStore.loadData([], false);
                                 return;
                             }
                             if (fleetids == ",,") {
                                 Ext.Msg.alert("Warning", 'No vehicle for Selected fleet.');
                                 SummaryGridStore.loadData([], false);
                                 return;
                             }
                             var vehicleids = "";
                             if (allSelectedVehicle) {
                                 if (DeSelectedIndexVehicle.length == 0) {
                                     vehicleids = ',-1,'
                                     selectAllVehicleFlag = false;
                                 }
                                 else {
                                     Ext.each(DeselectedVehicleId, function (val, index) {
                                        vehicleids += val + ',';
                                     });
                                     vehicleids = vehicleids.substring(0, vehicleids.length - 1);
                                     selectAllVehicleFlag = true;
                                 }
                             }
                             else {
                                 Ext.each(selectedVehicleId, function (val, index) {
                                     vehicleids += ',' + val;
                                 });
                                 vehicleids += ',';
                                 selectAllVehicleFlag = false;
                             }


                             if (vehicleids == ",") {
                                 Ext.Msg.alert("Warning", 'Please select a Vehicle');
                                 SummaryGridStore.loadData([], false);
                                 return;
                             }
                             var eventids = ViolationboxGroup.getValue();
                             if (Object.keys(eventids).length == 0) {
                                 Ext.Msg.alert("Warning", 'Please select a Event');
                                 SummaryGridStore.loadData([], false);
                                 return;
                             }
                             var eventid = ViolationboxGroup.getValue();
                            
                             var x = 0;
                             if (ScheventId.length > 0) {
                                 for (i = 0; i < ScheventId.length; i++) {
                                     if (x == 0) {
                                         for (var prop in eventid) {
                                             if (eventid.hasOwnProperty(prop))
                                                 if (ScheventId[i] == eventid[prop]) {
                                                     //Commented to hide Schedule Start Time Control
                                                     // Ext.getCmp('SchTimeFrom').show();
                                                     x = 1;
                                                     break;
                                                 }
                                                 else {
                                                     Ext.getCmp('SchTimeFrom').hide();
                                                 }

                                         }
                                     }
                                 
                                 }
                             }
                             else {
                                 Ext.getCmp('SchTimeFrom').hide();
                             }

                             // Set Report Columns to currently visible ones
                             var chkboxs = SchedulerColumnsGroup.getBoxes();
                             var visibleReportColumns = '';
                             for (var i = 0; i < SenchaColID.length; i++) {
                                 if (Ext.getCmp(SenchaColID[i].toString().trim()).hidden == false) {

                                     visibleReportColumns += (SenchaColName[i] + ', ');
                                     chkboxs[i].setValue(1);
                                 }
                                 else {
                                     chkboxs[i].setValue(0);
                                 }
                             }
                             
                             //var y = x.Repl(',');
                             Schedulepopup.show();
                             //Ext.getCmp('SchedulepopupId').body.setStyle('background-color', organizationColor);
                         }
                     }
                 },
                  
                 {
                     xtype: 'button',                     
                     id: 'SetColumnPreference',
                     cls: 'fa fa-sliders fa-2x eventBtn',
                     listeners: {
                         click: function () {
                             var column;
                             if (table == "Event") {
                                 column = eventCol.split(',');
                             }
                             else {                                
                                 column = violationCol.split(',');                                 
                             }
                             var chkboxs = VisiblecolumnsboxGroup.getBoxes()
                             Ext.Array.each(chkboxs, function (chkbox) {                               
                                 for (var i = 0; i < column.length; i++) {
                                     if (SenchaColName[column[i] - 1] == chkbox.id) {
                                         chkbox.setValue(1);
                                         break;
                                     }
                                     else {
                                         chkbox.setValue(0);
                                     }
                                 }
                               
                             });

                             Ext.getCmp('EventViolationCombo').setValue(table);
                             ColumnPreferencePopup.show();                             
                         }
                     }
                 },
                
                
                 {
                     xtype: 'button',
                     cls: 'eventBtn fa fa-angle-double-down fa-2x',
                     id: 'MoreRecordsGenerate',
                     listeners: {
                         click: function () {
                             loadingMask.show();
                             var fleetids = "";
                             if (allSelected)
                                 fleetids = ',-1,'
                             else {
                                 Ext.each(selectedFleetId, function (val, index) {
                                     fleetids += ',' + val;
                                 });
                                 fleetids += ',';
                             }
                             var vehicleids = "";
                             if (allSelectedVehicle) {
                                 if (DeSelectedIndexVehicle.length == 0) {
                                     vehicleids = ',-1,'
                                     selectAllVehicleFlag = false;
                                 }
                                 else {
                                     Ext.each(DeselectedVehicleId, function (val, index) {
                                        vehicleids += val + ',';
                                     });
                                     vehicleids = vehicleids.substring(0, vehicleids.length - 1);
                                     selectAllVehicleFlag = true;
                                 }
                             }
                             else {
                                 Ext.each(selectedVehicleId, function (val, index) {
                                     vehicleids += ',' + val;
                                 });
                                 vehicleids += ',';
                                 selectAllVehicleFlag = false;
                             }
                             SummaryGridStore.load(
                                                             {
                                                                 params:
                                                                  {
                                                                      Action: table,
                                                                      VehicleIds: vehicleids,
                                                                      startDate: DateFrom.rawValue + " " + TimeFrom.value,
                                                                      EndDate: DateTo.rawValue + " " + TimeTo.value,
                                                                      Events: ViolationboxGroup.getValue(),
                                                                      Search: Ext.getCmp('searchField').getValue(),
                                                                      Columns: searchColumns,
                                                                      SortBy: sortingParam,                                                                     
                                                                      start: (SummaryGridStore.totalCount / Pagesize),
                                                                      limit: Pagesize,
                                                                      FirstTime: false,
                                                                      FetchMoreRecords: true,
                                                                      SelectAllVehicleFlag: selectAllVehicleFlag,
                                                                      FromDB: 'true'

                                                                  },
                                                             
                                                                callback: function (records, operation, success) {                                                                                                                          
                                                                    loadingMask.hide();
                                                                    SummaryGridPager.doRefresh();
                                                                    if (records.length != 0) {
                                                                        var parser = new DOMParser();
                                                                        var response = operation.response.responseText;
                                                                        var xmlDoc = parser.parseFromString(response, "text/xml");
                                                                        var EventsFetched = xmlDoc.getElementsByTagName("EventsFetched")[0].innerHTML;
                                                                        if (EventsFetched != recordsToFetch) {
                                                                            if (table == "Event") {
                                                                                Ext.MessageBox.show({
                                                                                    title: "Fetch More Records",
                                                                                    msg: "ALL Events have been fetched.There are no more records.",
                                                                                    icon: Ext.MessageBox.INFO,

                                                                                });
                                                                            }
                                                                            else {

                                                                                Ext.MessageBox.show({
                                                                                    title: "Fetch More Records",
                                                                                    msg: "ALL Violations have been fetched.There are no more records.",
                                                                                    icon: Ext.MessageBox.INFO,

                                                                                });

                                                                            }

                                                                            hide_msg();
                                                                            Ext.getCmp('MoreRecordsGenerate').hide();
                                                                        }
                                                                    }

                                                                     
                                                                                                                                        
                                                                     if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                                                        
                                                                         Ext.Msg.alert("Warning", operation.resultSet.message);
                                                                        
                                                                        
                                                                         if (records == null)
                                                                             SummaryGridStore.loadData([], false);
                                                                     }

                                                                 }
                                                             }
                             );
}
                     }
                 },

                


            GenerateBtn, VehicleBtn, FleetBtn, DateBtn, EventBtn, TableBtn

        ],

    });

   

    function hide_msg() {
        Ext.defer(function () {
           Ext.MessageBox.hide();
        }, 4000);
    }


    var SummaryGridStore = Ext.create('Ext.data.Store', {
        model: 'EventGridModel',
        autoLoad: false,
        sortOnLoad: false,
        storeId: 'SummaryGridStore',
        pageSize: Pagesize,
        proxy:
           {
               type: 'ajax',
               url: './Event.aspx?QueryType=GetEvents',
               timeout: proxyTimeOut,
               reader:
              {
                  type: 'xml',
                  root: 'DocumentElement',
                  record: 'Table',
                  messageProperty: 'message',
                  totalProperty: 'totalCount'

              }
             
           },
        sort: function (sorters) {
            if (sorters != undefined && sorters.direction != undefined && sorters.property != undefined) {
                sortingParam = {};
                sortingParam.property = sorters.property;
                sortingParam.direction = sorters.direction;
            }
            else if (sortingParam == undefined || sortingParam.direction == undefined || sortingParam.property == undefined) {
                return;
            }
            this.sorters.clear();
            this.sorters.add(sortingParam);
            loadingMask.show();
            var fleetids = "";
            if (allSelected)
                fleetids = ',-1,'
            else {
                Ext.each(selectedFleetId, function (val, index) {
                    fleetids += ',' + val;
                });
                fleetids += ',';
            }
            var vehicleids = "";
            if (allSelectedVehicle) {
                if (DeSelectedIndexVehicle.length == 0) {
                    vehicleids = ',-1,'
                    selectAllVehicleFlag = false;
                }
                else {
                    Ext.each(DeselectedVehicleId, function (val, index) {
                       vehicleids += val + ',';
                    });
                    vehicleids = vehicleids.substring(0, vehicleids.length - 1);
                    selectAllVehicleFlag = true;
                }
            }
            else {
                Ext.each(selectedVehicleId, function (val, index) {
                    vehicleids += ',' + val;
                });
                vehicleids += ',';
                selectAllVehicleFlag = false;
            }
           
            SummaryGridStore.load(
                                 {
                                     params:
                                      {
                                          Action: table,
                                          VehicleIds: vehicleids,
                                          startDate: DateFrom.rawValue + " " + TimeFrom.value,
                                          EndDate: DateTo.rawValue + " " + TimeTo.value,
                                          Events: ViolationboxGroup.getValue(),
                                          Search: Ext.getCmp('searchField').getValue(),
                                          Columns: searchColumns,
                                          SortBy: sortingParam,
                                          start: SummaryGridStore.currentPage - 1,
                                          limit: Pagesize
                                          , SelectAllVehicleFlag: selectAllVehicleFlag

                                      },
                                     callback: function (records, operation, success) {
                                         loadingMask.hide();
                                         if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
                                             Ext.Msg.alert("Warning", operation.resultSet.message);
                                             if (records == null)
                                                 SummaryGridStore.loadData([], false);
                                         }

                                     }
                                 });
        }
    });


    var SummaryGridselModel = Ext.create('Ext.selection.CheckboxModel', {
        checkOnly: true,
        //cls: 'checkbx',
        align: 'top',
        enableKeyNav: false,
        listeners:
        {
            selectionchange: function (selModel, selections) {
                var checkedHd = this.view.headerCt.child('gridcolumn[isCheckerHd]').el.hasCls(Ext.baseCSSPrefix + 'grid-hd-checker-on');
                if (checkedHd) {
                    headerChecked = 1;
                    selModel.selectAll();
                    selectedEvent = [];
                    return;
                }
                else { headerChecked = 0; }
                if (Object.keys(selections).length == 0)
                    SummaryGridStore.each(function (record, idx) {
                        if (selectedEvent.indexOf(record.data.EventID + '-' + record.data.BoxID) > -1) {
                            SummaryGridselModel.select(idx, true, true);
                        }
                    });
            },
            deselect: function (selectionModel, record, index, eOpts) {
                if (selectedEvent.indexOf(record.data.EventID + '-' + record.data.BoxID) > -1) {
                    selectedEvent.splice(selectedEvent.indexOf(record.data.EventID + '-' + record.data.BoxID), 1);
                }

                if (selectedEventIndex.indexOf(record.index) > -1) {
                    selectedEventIndex.splice(selectedEventIndex.indexOf(record.index), 1);
                }
            },
            select: function (selectionModel, record, index, eOpts) {
                if (selectedEvent.indexOf(record.data.EventID + '-' + record.data.BoxID) == -1) {
                    selectedEvent.push(record.data.EventID + '-' + record.data.BoxID);
                }
                if (selectedEventIndex.indexOf(record.index) == -1) {
                    selectedEventIndex.push(record.index);
                }
            }
        }
    });


    var SummaryGridPager = new Ext.PagingToolbar({
        id: 'SummaryGridPager',
        store: 'SummaryGridStore',
        pageSize: Pagesize,
        dock: 'top',
        displayInfo: true,
        displayMsg: 'Displaying Records {0} - {1} of {2}',
        emptyMsg: "No Records to display",
        listeners: {
            beforechange: function (b, page, o) {
                // currentPg = page;
                var fleetids = "";
                if (allSelected)
                    fleetids = ',-1,'
                else {

                    Ext.each(selectedFleetId, function (val, index) {
                        fleetids += ',' + val;
                    });
                    fleetids += ',';
                }
                //var vehicleids = "";
                //if (allSelectedVehicle) {
                //    vehicleids = ',-1,'
                //}
                //else {
                //     Ext.each(selectedVehicleId, function (val, index) {
                //        vehicleids += ',' + val;
                //    });
                //    vehicleids += ',';
                //}
                var vehicleids = "";
                if (allSelectedVehicle) {
                    if (DeSelectedIndexVehicle.length == 0) {
                        vehicleids = ',-1,'
                        selectAllVehicleFlag = false;
                    }
                    else {
                        Ext.each(DeselectedVehicleId, function (val, index) {
                           vehicleids += val + ',';
                        });
                        vehicleids = vehicleids.substring(0, vehicleids.length - 1);
                        selectAllVehicleFlag = true;
                    }
                }
                else {
                    Ext.each(selectedVehicleId, function (val, index) {
                        vehicleids += ',' + val;
                    });
                    vehicleids += ',';
                    selectAllVehicleFlag = false;
                }


                if (vehicleids == ",") {
                    Ext.Msg.alert("Warning", 'Please select a Vehicle');
                    SummaryGridStore.loadData([], false);
                    return;
                }
                loadingMask.show();
                SummaryGridStore.proxy.extraParams = (
               {

                   Action: table,
                   FromDB: 'true',
                   VehicleIds: vehicleids,
                   startDate: DateFrom.rawValue + " " + TimeFrom.value,
                   EndDate: DateTo.rawValue + " " + TimeTo.value,
                   Events: ViolationboxGroup.getValue(),
                   Search: Ext.getCmp('searchField').getValue(),
                   Columns: searchColumns,
                   SortBy: sortingParam,
                   start: page - 1,
                   limit: Pagesize,
                   FirstTime: 'false',
                   SelectAllVehicleFlag: selectAllVehicleFlag

               });


            },
            change: function (pagingToolBar, changeEvent) {
                loadingMask.hide();
                pagingToolBar.start = SummaryGridStore.currentPage;

                //Remember selection on page change
                SummaryGridStore.each(function (record, idx) {
                    if (selectedEvent.indexOf(record.data.EventID + '-' + record.data.BoxID) > -1) {
                        SummaryGridselModel.select(idx, true, true);
                    }
                });
            }
        }


    });
    var SummaryGrid = Ext.create('Ext.grid.Panel', {
        id: 'SummaryGrid',
        autoLoad: false,
        store: 'SummaryGridStore',
        width: window.screen.width - 20,
        height: window.top.innerHeight - 140,
        autoScroll: true,
        maxHeight: window.screen.height,
        cls: 'x-grid-header-ct x-column-header extra-alt child-row x-grid-with-col-lines x-grid-cell summarygrd',
        selModel: SummaryGridselModel,
        viewConfig: {
            forceFit: true            
        },
        columns: [
        {
            header: 'Unit ID',
            dataIndex: 'BoxID',
            id: 'UnitID',
            //flex: 2,
            width:100,
            filterable: true,
            sortable: true,
            hidden: true,
            
            
        },
        {
            header: 'License Plate',
            dataIndex: 'LicensePlate',
            id: 'LicensePlate',
            //flex: 3,
            width: 120,
            renderer: function (val, meta, record, rowIndex) {
                //return '<a href="javascript:void(0);" OnClick="">' + record.data['MakeName'] + "/" + record.data['ModelName'] + "<br/>" + record.data['LicensePlate'] + "<br/>" + record.data['VehicleDescription'] + '</a>';
                return record.data['LicensePlate'];//+ "; " + record.data['VehicleDescription'];
            },
            filterable: true,
            sortable: true,
            
            hidden: true,
            
        },
         {
             header: 'Vehicle Description',
             dataIndex: 'VehicleDescription',
             id: 'VehicleDescription',
             //flex: 4,
             width: 150,
             
             renderer: function (val, meta, record, rowIndex) {
                 //return '<a href="javascript:void(0);" OnClick="">' + record.data['MakeName'] + "/" + record.data['ModelName'] + "<br/>" + record.data['LicensePlate'] + "<br/>" + record.data['VehicleDescription'] + '</a>';
                 return record.data['VehicleDescription'];
             },
             filterable: true,
             sortable: true,
             hidden: true,
             
         },
         {
             header: 'Vehicle Make',
             dataIndex: 'MakeName',
             id:'VehicleMake',
             //flex: 3,
             width: 120,
             renderer: function (val, meta, record, rowIndex) {
                 //return '<a href="javascript:void(0);" OnClick="">' + record.data['MakeName'] + "/" + record.data['ModelName'] + "<br/>" + record.data['LicensePlate'] + "<br/>" + record.data['VehicleDescription'] + '</a>';
                 return record.data['MakeName'];
             },
             filterable: true,
             sortable: true,
             hidden: true,
             
         },
          {
              header: 'Vehicle Model',
              dataIndex: 'ModelName',
              id:'VehicleModel',
              //flex: 3,
              width: 120,
              renderer: function (val, meta, record, rowIndex) {
                  //return '<a href="javascript:void(0);" OnClick="">' + record.data['MakeName'] + "/" + record.data['ModelName'] + "<br/>" + record.data['LicensePlate'] + "<br/>" + record.data['VehicleDescription'] + '</a>';
                  return record.data['ModelName'];
              },
              filterable: true,
              sortable: true,
              hidden: true,
              
          },
           {
               header: 'Vin Number',
               dataIndex: 'VinNumber',
               id: 'VINNumber',
               //flex: 3,
               width: 170,
               filterable: true,
               sortable: true,               
               hidden: true,
               
           },
           {
               header: 'Address',
               dataIndex: 'Address',
               id:'address',
               //flex: 4, 
               width: 400,
               filterable: true,
               sortable: true,
               hidden: true,
               
           },
        {
            header: 'Driver Name',
            text: 'DriverName',
            id: 'DriverName',
            //flex: 3,
           width: 180,
            dataIndex: 'DriverName',
            filterable: true,
            sortable: true,
            hidden: true,
            
        },
        {
            header: 'Service Name',
            dataIndex: 'ServiceName',
            id: 'ServiceName',
            //flex: 4,
            width: 200,
            filterable: true,
            sortable: true,
            hidden: true,
            
        },
        {
            header: 'Service Type',
            dataIndex: 'ServiceType',
            //flex: 4,
            width: 200,
            id:'ServiceType',
            filterable: true,
            sortable: true,
            hidden: true,
            
        },
        {
            header: 'Date Time',
            text: 'Date/Time',
            id:'DateTime',
            //flex: 4,
            width: 180,
            xtype: 'datecolumn',
            format: userdateformat,
            dataIndex: 'StDate',
            filterable: true,
            sortable: true,
            tdCls: 'x-date-time',
            hidden: true,
            
        },
        {
            header: 'Event Type',
            dataIndex: 'TableType',           
            id: 'EventType',
           // flex: 2,
            width: 100,
            filterable: true,
            sortable: true,
            hidden: true,
            
        },
        {
            header: 'Driver Class',
            dataIndex: 'Driver_Class',
            id: 'Driver_Class',
            //flex: 2,
            width: 140,
            filterable: true,
            sortable: true,
            hidden: true,
            
            
        },
        {
            header: 'Vehicle Class',
            dataIndex: 'Vehicle_Class',
            id: 'Vehicle_Class',
            //flex: 2,
            width: 140,
            filterable: true,
            sortable: true,
            hidden: true,
            
            
        },
        {
            header: 'Field 1',
            dataIndex: 'Field1',
            id: 'Field1',
            // flex: 2,
            width: 130,
            filterable: true,
            sortable: true,
            hidden: true,
            
            
        },
        
        {
            header: 'Field 2',
            dataIndex: 'Field2',
            id: 'Field2',
            //flex: 2,
            width: 130,
            filterable: true,
            sortable: true,
            hidden: true,
            
            
        },
        {
            header: 'Field 3',
            dataIndex: 'Field3',
            id: 'Field3',
            width: 130,
            //flex: 2,
            filterable: true,
            sortable: true,
            hidden: true,
            
            
        },
         {
             header: 'Field 4',
             dataIndex: 'Field4',
             id: 'Field4',
             //flex: 2,
             width: 130,
             filterable: true,
             sortable: true,
             hidden: true,
             
             
         },
          {
              header: 'Field 5',
              dataIndex: 'Field5',
              id: 'Field5',
              width: 200,
              //flex: 2,
              filterable: true,
              sortable: true,
              hidden: true,
              
              
          },
        {
        header: 'Notes',
        dataIndex: 'Notes',
        id: 'Note',
           // flex: 2,
        width: 130,
            filterable: true,
            sortable: true,
            hidden: true,
            
              
        },
        {
            header: 'Manager Employee',
            dataIndex: 'ManagerId',
            id: 'ManagerId',
            // flex: 2,
            width: 170,
            filterable: true,
            sortable: true,
            hidden: true,

        },
        {
            header: 'Manager Name',
            dataIndex: 'ManagerName',
            id: 'ManagerName',
            // flex: 2,
            width: 170,
            filterable: true,
            sortable: true,
            hidden: true,

        },
        {
            header: 'Color',
            dataIndex: 'color',
            id: 'color',
            // flex: 2,
            width: 170,
            filterable: true,
            sortable: true,
            hidden: true,

        },
           {
               header: 'Landmark Name',
               dataIndex: 'LandmarkName',
               id: 'LandmarkName',
               //flex: 3,
               width: 170,
               filterable: true,
               sortable: true,
               hidden: true,

           }, 
        {
        header: 'Fleet Name',
        dataIndex: 'CostCenter',
        id: 'FleetName',
        //flex: 3,
        width: 170,
        filterable: true,
        sortable: true,
        hidden: true,

        }
        ,{
        header: 'Employee ID',
        dataIndex: 'EmployeeId',
        id: 'EmployeeId',
        //flex: 3,
        width: 170,
        filterable: true,
        sortable: true,
        hidden: true,

        },
        {
        header: 'BSCID',
        dataIndex: 'bscid',
        id: 'bscid',
        //flex: 3,
        width: 170,
        filterable: true,
        sortable: true,
        hidden: true,

    }
        ],

        bbar: SummaryGridPager,
        listeners: {
            'afterrender': function () {
               
                Ext.each(this.columns, function (col, index) {                    
                        
                    if (col.hidden && col.text != '&#160;') {
                        var cb = Ext.create('Ext.form.field.Checkbox', {
                            boxLabel: col.text,
                            inputValue: col.dataIndex,
                            id: col.text,
                            checked: true
                        });
                        Ext.getCmp('SGridSearchMenuCBG').add(cb);
                    }

                                      
                    if (col.hidden && col.text != '&#160;') {
                        var cb = Ext.create('Ext.form.field.Checkbox', {
                            boxLabel: col.text,
                            inputValue: col.dataIndex,
                            id: col.text,
                        });
                        
                        VisiblecolumnsboxGroup.add(cb);
                    }
                   
                    
                });
            },
   
            }
    });

    SchTimeFrom = Ext.create('Ext.ux.CustomSpinner', {
        name: 'SchTimeFrom',
        fieldLabel: 'Schedule Start Time',
        id: 'SchTimeFrom',
        //format: 'H:i',
        editable: true,
        enableKeyEvents: true,
        emptyText: timeText,
        value: '12:00:00 AM',
        vtype: 'time',
        step: 15,
        hidden: true

    });
    
    SchTimeTo = Ext.create('Ext.ux.CustomSpinner', {
        name: 'TimeTo',

        fieldLabel: 'Schedule End Time',
        
        editable: true,
        enableKeyEvents: true,
        emptyText: timeText,
        value: '12:00:00 AM',
        vtype: 'time',
        step: 15,
        hidden: true
    });

   
    var ColumnPreferencePopup = Ext.create('Ext.form.Panel', {
        id: 'ColumnPreferencePopupId',
        title: 'Column Preferences',
        width: 250,     
        layout: 'column', 
        cls: 'schedulePopupBorder',
        modal: true,
        floating: true,
        draggable: true,
        centered: true,
        closable: true,
        closeAction: 'hide',       
       
        items: [{
                         xtype: 'fieldset',
                         title: 'Event Category',
                         id: 'EventCategory',
                         width: 230,
                         cls: 'ColumnPreferenceEventCategory',
                         items: [{
                             xtype: 'combobox',                             
                             name: 'EventViolation',
                             width: 210,
                             id: 'EventViolationCombo',
                             editable: false,                            
                             store: ['Event', 'Violation'],  
                             listeners: {
                                 change: function (combo, newValue, oldValue) {
                                     var column;                            
                             
                                     if (newValue == 'Event')
                                     {
                                         column = eventCol.split(',');                               
                                     }
                                     else {                                 
                                         column = violationCol.split(',');                                
                                     }

                                     var chkboxs = VisiblecolumnsboxGroup.getBoxes()
                                     Ext.Array.each(chkboxs, function (chkbox) {                                                                      

                                         for (var i = 0; i < column.length; i++) {
                                             if (SenchaColName[column[i] - 1] == chkbox.id) {
                                                 chkbox.setValue(1);
                                                 break;
                                             }
                                             else {
                                                 chkbox.setValue(0);
                                             }
                                         }
                         
                                     })

                             
                                 }
                             }
                         
                     }]

                     },
                     {
                         xtype: 'fieldset',
                         title: 'List Of Columns',
                         id: 'ListOfColumns',
                         width: 230,
                         cls: 'ColumnPreferenceEventCategory',
                         items: [
                             VisibleColumnsChkboxList,
                             
                     
               {
                   text: 'Select All',
                   xtype: 'button',              
                   cls: 'ColumnPreferenceSelectAll',
                   hideOnClick: false,
                 
                   handler: function () {
                       var chkboxs = VisiblecolumnsboxGroup.getBoxes()
                       Ext.Array.each(chkboxs, function (chkbox) {
                           chkbox.setValue(1);
                       });
                   }},        

            {
                text: 'Deselect All',
                xtype: 'button',
                cls: 'ColumnPreferenceDeSelectAll',
                hideOnClick: false,
                handler: function () {
                    var chkboxs = VisiblecolumnsboxGroup.getBoxes()
                    Ext.Array.each(chkboxs, function (chkbox) {
                        chkbox.setValue(0);
                    });
                }

            },
            {
                text: 'Reset',
                xtype: 'button',
                cls: 'ColumnPreferenceVisibleColumn',
                hideOnClick: false,
                handler: function () {
                    var chkboxs = VisiblecolumnsboxGroup.getBoxes();
                    for (var i = 0; i < SenchaColID.length; i++) {
                        if (Ext.getCmp(SenchaColID[i].toString().trim()).hidden == false) {

                            chkboxs[i].setValue(1);
                        }
                        else {
                            chkboxs[i].setValue(0);
                        }
                    }
                }

            }]},
         {
             text: 'Save',
             xtype: 'button',
             cls: 'ColumnPreferenceSavetBtn',
             hideOnClick: false,
             handler: function () {
                  
                 DefaultEventSelectedCol = "";
                 DefaultViolationSelectedCol = "";
                 var chkboxs = VisiblecolumnsboxGroup.getBoxes();
                 for (var i = 0; i < chkboxs.length; i++) {
                     if(chkboxs[i].checked == true)
                     {
                         if (table == "Event") {
                             DefaultEventSelectedCol += (DatabaseColID[i]).toString() + ',';
                         }
                         else {
                             DefaultViolationSelectedCol += (DatabaseColID[i]).toString() + ',';
                         }
                     }
                 }
                 var sSelectedColumnPreference = '';
                 if (table == "Event") {
                     sSelectedColumnPreference = DefaultEventSelectedCol;
                     eventCol = DefaultEventSelectedCol;
                 }
                 else {
                     sSelectedColumnPreference = DefaultViolationSelectedCol;
                     violationCol = DefaultViolationSelectedCol;
                 }

                 var postdata = { SelectedColumnPreference: sSelectedColumnPreference, Operation: table };
                 Ext.Ajax.request({
                     params: postdata,
                     url: './Event.aspx?QueryType=SetDefaultColumnPreferences',
                     success: function (form, action) {
                         Ext.Msg.alert("Status", 'Column Preferences saved Successfully ');
                         ColumnPreferencePopup.hide();
                        
                     },
                     failure: function (form, action) {
                         Ext.Msg.alert("Warning", 'Error in saving column preferences');
                     }
                 });
                 
             }

         },
          {
              text: 'Save & Apply',
              xtype: 'button',              
              hideOnClick: false,
              cls: ' ColumnPreferenceSaveandApplytBtn',

             
                  handler: function () {
                      
                          DefaultEventSelectedCol = "";
                          DefaultViolationSelectedCol = "";
                          var chkboxs = VisiblecolumnsboxGroup.getBoxes();
                          for (var i = 0; i < chkboxs.length; i++) {
                              if (chkboxs[i].checked == true) {
                                  if (table == "Event") {
                                      DefaultEventSelectedCol += (DatabaseColID[i]).toString() + ',';
                                  }
                                  else {
                                      DefaultViolationSelectedCol += (DatabaseColID[i]).toString() + ',';
                                  }
                              }
                          }
                          var sSelectedColumnPreference = '';
                          if (table == "Event") {
                              sSelectedColumnPreference = DefaultEventSelectedCol;
                              eventCol = DefaultEventSelectedCol;
                          }
                          else {
                              sSelectedColumnPreference = DefaultViolationSelectedCol;
                              violationCol = DefaultViolationSelectedCol;
                          }

                          var postdata = { SelectedColumnPreference: sSelectedColumnPreference, Operation: table };
                          Ext.Ajax.request({
                              params: postdata,
                              url: './Event.aspx?QueryType=SetDefaultColumnPreferences',
                              success: function (form, action) {
                                  Ext.Msg.alert("Status", 'Column Preferences Saved and Applied Successfully ');
                                  ShowHideColumnPreference();
                                  ColumnPreferencePopup.hide();

                              },
                              failure: function (form, action) {
                                  Ext.Msg.alert("Warning", 'Error in saving and applying column preferences');
                              }
                          });
                      
                  },
                  //click: function () {
                  //    ShowHideColumnPreference();
                  //    ColumnPreferencePopup.hide();

                  //},

              
          },
         {
             text: 'Cancel',
             xtype: 'button',            
             hideOnClick: false,
             cls: ' ColumnPreferenceCancelBtn ',

             listeners: {                
                 click: function () {
                     
                     ColumnPreferencePopup.hide();

                 },

             },
            

         } ]        

    });
  
   
    SchedulerColumnsGroup = new Ext.form.CheckboxGroup({
        id: 'SchedulerColumnsGroup',
        xtype: 'checkboxgroup',
        border: true,

        columns: 1,
        listeners:
        {
            change: function (field, newValue, oldValue, eOpts) {
               // EventBtn.setText(table + ": <br/>" + Object.keys(newValue).length + "/" + SchedulerColumnsGroup.getBoxes().length);
            }
        }
    });

    SchedulerColumnsGroup.removeAll();
    for (i = 0; i < SenchaColName.length; i++) {

        var cb = Ext.create('Ext.form.field.Checkbox', {
            boxLabel: SenchaColName[i].toString(),
            inputValue: DatabaseColID[i].toString(),
            id: 'SchedulerColumnsGroup_' + SenchaColID[i].toString()

        }
                    );

        try {
            SchedulerColumnsGroup.add(cb);
        }
        catch (err) {
            alert(err)
        }
    }

    var SchedulerColumnsChkboxList = Ext.create('Ext.form.Panel', {
        width: 200,
        height: 357,
        style: { marginLeft: '2px' },
        autoScroll: true,
        bodyPadding: 2,
        items: [SchedulerColumnsGroup]
    });
    var lineconfig = {
        xtype: 'box',
        autoEl: {
            tag: 'div',
            style: 'line-height:1px; font-size: 1px;margin-bottom:4px',          
        }
    };


    var ScheduleReportPopUp = Ext.create('Ext.form.Panel', {
        id: 'ScheduleReportPopUpId',
        width: 700,
        //layout: 'column',
        layout: 'column',
        items: [{
            xtype: 'panel',
            //title: 'Report Parameters',
            height: 500,
            columnWidth: 0.5,
            items: [{
                xtype: 'fieldset',
                //xtype: 'radiogroup',
                columns: 4,
                title: 'Frequency',
                id: 'repetition',
                width: 300,
                cls: 'scheduleRadio scheduleCheckbox scheduleSection ',
                border: true,
                items: [{
                    xtype: 'radiogroup',
                    id: 'radiogroupfield',
                    items: [{
                        xtype: 'radiofield',
                        boxLabel: 'Once',
                        name: 'repeat',
                        checked: true,
                        inputValue: '0'
                    },
                            {
                                xtype: 'radiofield',
                                boxLabel: 'Daily',
                                name: 'repeat',
                                inputValue: '1'
                            },
                            {
                                xtype: 'radiofield',
                                boxLabel: 'Weekly',
                                name: 'repeat',
                                inputValue: '2'
                            },
                            {
                                xtype: 'radiofield',
                                boxLabel: 'Monthly',
                                name: 'repeat',
                                inputValue: '3'
                            }],
                    listeners: {
                        change: function (item, state) {
                            if (state.repeat == "0") {
                                Ext.getCmp('End_date').hide();
                                Ext.getCmp('Monthly').hide();
                                Ext.getCmp('Weekly').hide();
                            }
                            else {
                                Ext.getCmp('End_date').show();
                                Ext.getCmp('Monthly').hide();
                                Ext.getCmp('Weekly').hide();
                            }
                            if (state.repeat == "2") {
                                Ext.getCmp('Weekly').show();
                                Ext.getCmp('Monthly').hide();
                            }
                            if (state.repeat == "3") {
                                Ext.getCmp('Monthly').show();
                                Ext.getCmp('Weekly').hide();
                            }
                        }
                    }
                },



            {
                xtype: 'fieldset',
                id: 'Monthly',
                width: 278,

                cls: 'scheduleCombo',
                hidden: true,
                items: [{
                    xtype: 'combobox',
                    fieldLabel: 'Every',
                    name: 'Monthly',
                    id: 'MonthlyCombo',
                    editable: false,
                    store: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31'],
                    value: '1'
                }]
            },
            {
                xtype: 'fieldset',
                id: 'Weekly',
                width: 278,

                cls: 'scheduleCombo',
                hidden: true,
                items: [{
                    xtype: 'combobox',
                    fieldLabel: 'Every',
                    name: 'Weekly',
                    id: 'WeeklyCombo',
                    editable: false,
                    store: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
                    value: 'Monday'
                }]
            }]
            },
         {
             xtype: 'fieldset',
             title: 'Export Format',
             id: 'ExportFormat',
             width: 300,
             cls: 'scheduleDelivery scheduleCheckbox scheduleSection',
             items: [{
                 xtype: 'radiogroup',
                 columns: 3,
                 id: 'ScheduleExportFormat',
                 width: 290,
                 border: false,
                 items: [{
                     xtype: 'radiofield',
                     boxLabel: 'PDF',
                     name: 'export',
                     checked: true,
                     inputValue: '0'
                 },
                 {
                     xtype: 'radiofield',
                     boxLabel: 'Excel',
                     name: 'export',
                     inputValue: '1'
                 }, {
                     xtype: 'radiofield',
                     boxLabel: 'Word',
                     name: 'export',
                     inputValue: '2'
                 }]
             }]
         },
        {
            xtype: 'fieldset',
            title: 'Delivery Method',
            id: 'DeliveryM',
            width: 300,
            cls: 'scheduleDelivery scheduleCheckbox scheduleSection',
            items: [{
                xtype: 'radiogroup',
                columns: 2,
                id: 'Delivery',
                width: 290,
                border: false,
                items: [{
                    xtype: 'radiofield',
                    boxLabel: 'To Email',
                    name: 'delivery',
                    checked: true,
                    inputValue: '0',

                    handler: function (field, value) {
                        if (value) {
                            Ext.getCmp('ScheduleEmail').show();
                            Ext.getCmp('ScheduleEmail').enable();
                            Ext.getCmp('myFieldId').hide();
                            Ext.getCmp('myFieldId').disable();
                            Ext.getCmp('lblSuggestionMultipleEmail').show();
                            Ext.getCmp('lblSuggestionMultipleEmail').enable();
                            Ext.getCmp('lblSuggestionEmailLength').show();
                            Ext.getCmp('lblSuggestionEmailLength').enable();

                        }

                    }


                },
                {
                    xtype: 'radiofield',
                    boxLabel: 'Store to Disk',
                    name: 'delivery',
                    inputValue: '1',

                    handler: function (field, value) {
                        if (value) {
                            Ext.getCmp('ScheduleEmail').hide();
                            Ext.getCmp('ScheduleEmail').disable();
                            Ext.getCmp('myFieldId').show();
                            Ext.getCmp('myFieldId').enable();
                            Ext.getCmp('statusId').hide();
                            Ext.getCmp('lblSuggestionMultipleEmail').hide();
                            Ext.getCmp('lblSuggestionMultipleEmail').disable();
                            Ext.getCmp('lblSuggestionEmailLength').hide();
                            Ext.getCmp('lblSuggestionEmailLength').disable();

                        }

                    }


                }],

            },
            {
                xtype: 'textfield',
                name: 'Email',
                id: 'ScheduleEmail',
                fieldLabel: 'Email',
                cls: 'scheduleEmailTextField',
                emptyText: 'Example:jone@bsmwireless.com'

            },
            {
                xtype: 'label',
                id: 'myFieldId',
                text: 'Reports Would Be Saved Locally On The Server.',
                cls: 'scheduleStoreToDiskLabel'
            },
            {
                xtype: 'label',
                id: 'lblSuggestionMultipleEmail',
                text: 'Multiple email should be separated by comma (,) or semicolon (;).',
                cls: 'scheduleStoreToDiskLabel'
            },lineconfig,
                {
                    xtype: 'label',
                    id: 'lblSuggestionEmailLength',
                    text: 'Email text should not exceed by 8000 characters.',
                    cls: 'scheduleStoreToDiskLabel'
                }


            ]
        },
        {
            xtype: 'fieldset',
            title: 'Report Life Time',
            id: 'Life',
            width: 300,
            cls: 'scheduleFieldset scheduleSection',
            items: [{
                xtype: 'datefield',
                anchor: '100%',
                fieldLabel: 'Start',
                name: 'Start_date',
                id: 'Start_date',
                format: userDate,
                value: new Date(),
                minValue: new Date()
            },
            {
                xtype: 'datefield',
                anchor: '100%',
                fieldLabel: 'End',
                name: 'End_date',
                id: 'End_date',
                format: userDate,
                value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1),
                minValue: new Date(),
                hidden: true
            }, SchTimeFrom

            ]
        },


            ]
        },
        {
            xtype: 'panel',
            //title: 'Report Columns',
            height: 500,
            columnWidth: 0.5,
            items: [{
                xtype: 'fieldset',
                title: 'Columns On The Report',
                id: 'SchedulerReportColumns',
                width: 220,
                height: 440,
                cls: 'SchedulerColumnFieldset scheduleSection',
                items: [SchedulerColumnsChkboxList,
                {
                    xtype: 'button',
                    cls: 'SchedulerColumnSelectAll',
                    text: 'Select All',
                    
                    hideOnClick: false,
                    handler: function () {
                        var chkboxs = SchedulerColumnsGroup.getBoxes();
                        Ext.Array.each(chkboxs, function (chkbox) {
                            chkbox.setValue(1);
                        });
                    }
                },

                {
                    xtype: 'button',
                    text: 'Deselect All',
                    cls: 'SchedulerColumnDeSelectAll',
                   
                    hideOnClick: false,
                    handler: function () {
                        var chkboxs = SchedulerColumnsGroup.getBoxes();
                        Ext.Array.each(chkboxs, function (chkbox) {
                            chkbox.setValue(0);
                        });
                    }
                },
                {
                    xtype: 'button',
                    text: 'Reset',
                    cls: 'SchedulerColumnVisibleColumn',
                    
                    hideOnClick: false,
                    handler: function () {

                        var chkboxs = SchedulerColumnsGroup.getBoxes();
                        var visibleReportColumns = '';
                        for (var i = 0; i < SenchaColID.length; i++) {
                            if (Ext.getCmp(SenchaColID[i].toString().trim()).hidden == false) {

                                visibleReportColumns += (SenchaColName[i] + ', ');
                                chkboxs[i].setValue(1);
                            }
                            else {
                                chkboxs[i].setValue(0);
                            }
                        }
                    }
                }

                ]
            }],
        }
        ]
    });

    var SubmitSchedulerSection = Ext.create('Ext.form.Panel', {
        id: 'SubmitSchedulerSectionId',
        width: 700,
        //layout: 'column',
        layout: 'column',
        items: [{
            xtype: 'button',
            text: 'Submit',
            id: 'ScheduleSubmit',
            cls:'scheduleSubmitBtn',
                
            handler: function () {

                var fType = Ext.getCmp('radiogroupfield').getValue();
                var DType = Ext.getCmp('Delivery').getValue();

                if (DType.delivery == '0' && Ext.getCmp('ScheduleEmail').getValue() == '') {
                    Ext.Msg.alert("Warning", 'Please Enter Email Id');
                    return;
                }
                var str = SchTimeFrom.value;
                var result = str.match(timetest);
                result = '23:00:00';

                if (result == null) {
                    Ext.Msg.alert("Warning", 'Invalid time format');
                    if (userTime.indexOf('A') > -1)
                        Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 12:00:00 AM');
                    else
                        Ext.Msg.alert("Warning", 'Invalid time format,Valid format is 00:00:00');

                    return;
                }

                if (Ext.getCmp('ScheduleEmail').getValue() != '') {
                    var email = Ext.getCmp('ScheduleEmail').getValue();
                    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                    if(email.length > 8000)
                    {
                        Ext.Msg.alert("Warning", 'Multiple email should be separated by comma (,) or semicolon (;) and Email text should not exceed by 8000 characters');
                        return;
                    }
                    if (email.indexOf(';') > -1) {
                        email = email.replace(/;/g, ",");
                    }
                    if (email.indexOf(',') > -1) {

                        if (email != '') {
                            result = email.split(',');

                            for (var i = 0; i < result.length; i++) {
                                if (result[i] != '') {

                                    if (!re.test(result[i])) {
                                        Ext.Msg.alert("Warning", 'Please Enter a Valid Email Id;  EmailId ' + result[i] + ' is invalid');
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else {
                        if (!re.test(Ext.getCmp("ScheduleEmail").getValue())) {
                            Ext.Msg.alert("Warning", 'Multiple email should be separated by comma (,) or semicolon (;)');
                            return;
                        }
                    }

                }


                var sExportFormatVal = Ext.getCmp('ScheduleExportFormat').getValue();
                var sExportFormat = "";
                if (sExportFormatVal.export == 0) {
                    sExportFormat = "PDF";
                }
                else if (sExportFormatVal.export == 1) {
                    sExportFormat = "Excel";
                }
                else {
                    sExportFormat = "Word";
                }
                var sDate = Ext.Date.format(Ext.getCmp('Start_date').getValue(), userDate);
                var sEventCategory = "";

                if (Ext.getCmp('checkbox1').getValue()) {
                    sEventCategory = 'Event';

                }
                if (Ext.getCmp('checkbox2').getValue()) {
                    sEventCategory = 'Violation';

                }

                //if (table == "Event") {
                //    eventCol = '';
                //}
                //else {
                //    violationCol = '';
                //}

                //var sVisibleColumns = '';
                //for (var i = 0; i < SenchaColID.length; i++) {
                //    if (Ext.getCmp(SenchaColID[i].toString().trim()).hidden == false) {
                //        if (table == "Event") {
                //            sVisibleColumns += (SenchaColID[i] + ',');
                //        }
                //        else {
                //            sVisibleColumns += (SenchaColID[i] + ',');
                //        }

                //    }
                //}
                //sVisibleColumns = sVisibleColumns.substring(0, sVisibleColumns.length - 1);

                var sReportColumns = '';
                //var SReportColumnNames = '';
                var chkboxs = SchedulerColumnsGroup.getBoxes();
                var i = 0;
                Ext.Array.each(chkboxs, function (chkbox) {
                    if (chkbox.checked == true) {
                        sReportColumns += (SenchaColID[i] + ',');
                        //SReportColumnNames += (SenchaColName[i] + ',');
                    }
                    i++;
                });

                if ((sReportColumns.trim().length == 0) || sReportColumns == undefined) {
                    Ext.Msg.alert("Warning", 'Please Select Atleast One Column');
                    return;
                }

                sReportColumns = sReportColumns.substring(0, sReportColumns.length - 1);

                var fleetids = "";
                if (allSelected)
                    fleetids = '-1'
                else {

                    Ext.each(selectedFleetId, function (val, index) {
                        fleetids += ',' + val;

                    });

                    var FirstCommaIndex = fleetids.indexOf(',');
                    if (FirstCommaIndex == 0) {
                        fleetids = fleetids.substring(1, fleetids.length);
                    }



                }
                //var noOfDaysCheck = ViolationboxGroup.getValue();
                //for (var i = 0; i < Object.keys(noOfDaysCheck).length; i++) {
                //    if (Object.keys(noOfDaysCheck)[i].inputValue == '-2') {
                //        sendNoOfDays = true;
                //        break;
                //    }
                //    else
                //        sendNoOfDays = false;
                //}
               
                var postdata = {

                    VisibleColumns: sReportColumns, ExportFormat: sExportFormat, EventCategory: sEventCategory, FleetIds: fleetids, SchedStart: sDate, SchedEnd: Ext.Date.format(Ext.getCmp('End_date').getValue(), userDate), FreqType: fType.repeat, FreqParamM: Ext.getCmp('MonthlyCombo').getValue(), FreqParamW: Ext.getCmp('WeeklyCombo').getValue(), Email: email, DeliveryType: DType.delivery, FromDate: DateFrom.rawValue + " " + TimeFrom.value, ToDate: DateTo.rawValue + " " + TimeTo.value, SchedStartTime: SchTimeFrom.value, SchEventIds: ViolationboxGroup.getValue()
                };


                Ext.Ajax.request({
                    params: postdata,
                    url: './Event.aspx?QueryType=scheduleReport',
                    success: function (form, action) {
                        Ext.Msg.alert("Status", 'Data Submitted Successfully ');
                        Schedulepopup.hide();
                        Ext.getCmp('ScheduleEmail').setValue('');
                        Ext.getCmp('Start_date').setValue(new Date());
                        Ext.getCmp('End_date').setValue(new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1));
                        Ext.getCmp('SchTimeFrom').setValue('12:00:00 AM');

                    },
                    failure: function (form, action) {
                        Ext.Msg.alert("Warning", 'Error in submitting data');
                    }
                });

            }
        },
        {
            xtype: 'button',
            text: 'Close',
            id: 'ScheduleClose',
            cls: 'scheduleCloseBtn',

            listeners: {
                click: function () {
                    Schedulepopup.hide();

                },

            },

        },
        {
            xtype: 'label',
            id: 'statusId',
            text: 'Unable To Schedule Requested Report',
            cls: 'scheduleValidationLabel'
        }]
    });

    var Schedulepopup = Ext.create('Ext.form.Panel', {
        id: 'SchedulepopupId',
        title: 'Schedule Report',
        width: 600,
        //layout: 'column',
        layout: 'column',

        //bodyStyle: { "background-color": "#B8B8B8", "border": "10px solid #F2F2F4" },
        cls: 'schedulePopupBorder ',

        modal: true,
        floating: true,
        draggable: true,
        centered: true,
        closable: true,
        closeAction: 'hide',
        items: [ScheduleReportPopUp, SubmitSchedulerSection]


    });
    
    Ext.getCmp('MoreRecordsGenerate').hide();
    Ext.getCmp('statusId').hide();

    Ext.getCmp('ScheduleEmail').show();
    Ext.getCmp('ScheduleEmail').enable();
    Ext.getCmp('myFieldId').hide();
    Ext.getCmp('myFieldId').disable();
    Ext.getCmp('lblSuggestionMultipleEmail').show();
    Ext.getCmp('lblSuggestionMultipleEmail').enable();
    Ext.getCmp('lblSuggestionEmailLength').show();
    Ext.getCmp('lblSuggestionEmailLength').enable();
    
    if (userTime.indexOf('A') > -1) {
        TimeFrom.setValue('12:00:00 AM');
        TimeTo.setValue('12:00:00 AM');
    }
    else {
        TimeFrom.setValue('00:00:00');
        TimeTo.setValue('00:00:00');
    }
    DateBtn.setText("From: " + DateFrom.rawValue + " " + TimeFrom.value + "<br/>&nbsp;&nbsp;&nbsp;&nbsp;To: " + DateTo.rawValue + " " + TimeTo.value);
    DateBtn.setWidth(150);
    SearchForm.render('form-ct');
    SearchForm.doLayout();
    SummaryGrid.render('form-ct');
    SummaryGrid.doLayout();
    if (LoadVehiclesBasedOn == 'fleet') {
        FleetBtn.setWidth(100);
        Fleetstore.load(
            {
                params:
                    {
                        start: 0,
                        limit: FleetPagesize
                    },
                callback: function (records, operation, success) {
                    if (success == true)
                        defaultFleetName = operation.resultSet.message;
                    if (SelectedFleetNames.indexOf(defaultFleetName) == -1) {
                        SelectedFleetNames.push(defaultFleetName);
                    }

                    var parser = new DOMParser();
                    var response = operation.response.responseText;
                    var xmlDoc = parser.parseFromString(response, "text/xml");
                    nDefaultFleetIndex = xmlDoc.getElementsByTagName("DefaultFleetIndex")[0].innerHTML;

                    // Update SelectedFleet Array
                    if (selectedFleet.indexOf(nDefaultFleetIndex) == -1) {
                        selectedFleet.push(nDefaultFleetIndex);
                    }
                    
                    // Update SelectedFleetId Array
                    if (defaultSelectedFleetid != null && defaultSelectedFleetid != "")
                        selectedFleetId.push(defaultSelectedFleetid);
                    if (selectedFleetId.length > 1)
                        FleetBtn.setText("Fleet: <br/>Multi Fleets");
                    else if (selectedFleetId.length == 1)
                        FleetBtn.setText("Fleet: <br/>" + defaultFleetName);
                    else
                        FleetBtn.setText("Fleet: <br/>" + 'Select a Fleet');
                    if (Ext.getCmp('checkbox1').getValue()) {
                        table = 'Event';
                       
                        //Ext.getCmp('checkbox2').setValue(0);
                    }
                    if (Ext.getCmp('checkbox2').getValue()) {
                        table = 'Violation';
                        
                        //Ext.getCmp('checkbox1').setValue(0);
                    }
                    ViolationStore.load({
                        params:
                {
                    Action: table
                },
                        callback: function (records, operation, success) {
                            if (firstTimeload)
                                firstLoad();
                        }
                    });

                    
                    if (defaultFleetName != "" && defaultFleetName != null) {

                        DefaultSelectedId = FleetGrid.getStore().findExact('FleetName', defaultFleetName, 0);
                        if (DefaultSelectedId != -1) {
                            FleetGridselModel.select(DefaultSelectedId, true, true);
                        }
                    }
                }
            });
    }
    else {
        FleetBtn.setWidth(150);
        selectedFleetId = [];
        selectedFleetId.push(DefaultOrganizationHierarchyFleetId);
        FleetBtn.setText("Fleet: <br/>" + DefaultOrganizationHierarchyFleetName);
        if (Ext.getCmp('checkbox1').getValue()) {
            table = 'Event';
            
            //Ext.getCmp('checkbox2').setValue(0);
        }
        if (Ext.getCmp('checkbox2').getValue()) {
            table = 'Violation';
            
            //Ext.getCmp('checkbox1').setValue(0);
        }
        ViolationStore.load({
            params:
                {
                    Action: table
                },
            callback: function (records, operation, success) {
                if (firstTimeload)
                    firstLoad();
            }
        });
    }
    //load default vehicle list
    if (LoadVehiclesBasedOn == 'fleet') {
        //selectedEvent = [];
        //selectedEventIndex = [];
        //handler Code 
        allSelectedVehicle = false;
        selectedVehicleId = [];
        selectedVehicle = [];
        DeselectedVehicleId = [];
        selectAllVehicleFlag = false;

       

        VehicleStore.load(
{
    params:
        {
            start: 0,
            limit: VehiclePagesize,
            Fleet: defaultFleet,
            FleetFirstTime: true,
            ExcludeFleetIDs: bFleetSelectionMode
            
        },
    callback: function (records, operation, success) {

        if (records != null) {
            VehicleStore.totalCount = VehicleStore.totalCount;
            VehicleStore.currentPage = 1;
            VehicleBtn.setText("Vehicle: <br/>" + 'Select a Vehicle');

        }
        else {

            VehicleStore.totalCount = 0;
            VehicleStore.currentPage = 0;
        }


        if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
            Ext.Msg.alert("Warning", operation.resultSet.message);
            if (records == null)
                VehicleStore.loadData([], false);
        }
        if (operation.exception && operation.error.statusText == "communication failure") {
            Ext.Msg.alert("Warning", "Server timeout");
            if (records == null)
                VehicleStore.loadData([], false);
        }

    }
});
    }
    else {
        allSelectedVehicle = false;
        selectedVehicleId = [];
        selectedVehicle = [];
        DeselectedVehicleId = [];
        selectAllVehicleFlag = false;



        VehicleStore.load(
{
    params:
        {
            start: 0,
            limit: VehiclePagesize,
            Fleet: DefaultOrganizationHierarchyFleetId,
            FleetFirstTime: true,
            ExcludeFleetIDs: bFleetSelectionMode
        },
    callback: function (records, operation, success) {

        if (records != null) {
            VehicleStore.totalCount = VehicleStore.totalCount;
            VehicleStore.currentPage = 1;
            VehicleBtn.setText("Vehicle: <br/>" + 'Select a Vehicle');

        }
        else {

            VehicleStore.totalCount = 0;
            VehicleStore.currentPage = 0;
        }


        if (operation.resultSet != undefined && operation.resultSet.message != undefined && operation.resultSet.message != "") {
            Ext.Msg.alert("Warning", operation.resultSet.message);
            if (records == null)
                VehicleStore.loadData([], false);
        }
        if (operation.exception && operation.error.statusText == "communication failure") {
            Ext.Msg.alert("Warning", "Server timeout");
            if (records == null)
                VehicleStore.loadData([], false);
        }

    }
});
    }

    function firstLoad() {

        firstTimeload = false;
        if (Ext.getCmp('checkbox1').getValue()) {
            table = 'Event';
           
            Ext.getCmp('checkbox2').setValue(0);
            
        }
        if (Ext.getCmp('checkbox2').getValue()) {
            table = 'Violation';
            
            Ext.getCmp('checkbox1').setValue(0);
            
         
        }
        if (table == undefined)
            table = 'select an Event Category';
        TableBtn.setText("Event Category: <br/>" + table)
        var fleetids = "";
        if (allSelected)
            fleetids = ',-1,'
        else {           
            Ext.each(selectedFleetId, function (val, index) {
                fleetids += ',' + val;
            });
            fleetids += ',';
        }

      
        ShowHideColumnPreference();

    }


    var updateClock = function () {

        if (DataPresent == 1) {
            //if (count == '0')
            //{ Ext.getCmp('loadDataBuffer').hide(); }
            //if (onlyOnce == 0) {
            if (table == 'Event') {
                if (count != '0') {
                    $.ajax({
                        type: 'GET',
                        url: './Event.aspx?QueryType=FillBufferRecursivelyEvent',
                        dataType: 'xml',
                        //data: postdata,
                        async: true,
                        success: function (msg, xml) {
                            count = $(xml.responseText).find("message").text();
                            //if (count == '0')
                            //{ Ext.getCmp('loadDataBuffer').hide(); }
                        },
                        error: function (msg) {
                            //alert('e');
                        }
                    });
                }
            }
            else if (table == 'Violation') {
                if (count != '0') {
                    $.ajax({
                        type: 'GET',
                        url: './Event.aspx?QueryType=FillBufferRecursivelyViolation',
                        dataType: 'xml',
                        //data: postdata,
                        async: true,
                        success: function (msg, result, xml) {
                            count = $(xml.responseText).find("message").text();
                            //if(count == '0')
                            //{ Ext.getCmp('loadDataBuffer').hide(); }

                        },
                        error: function (msg, result) {
                            //alert('e');
                        }
                    });
                }

            }

            //}
        }




    }
    var task = {
        run: updateClock,
        interval: 15000 //2 second
    }
    var runner = new Ext.util.TaskRunner();
    //runner.start(task);

});


function openWindow(wintitle, winURL, winWidth, winHeight) {
    var win = new Ext.Window({
        title: wintitle,
        width: winWidth,
        height: winHeight,
        layout: 'fit',
        maxWidth: window.screen.width,
        maxHeight: window.screen.height,
        maximizable: 'true',
        minimizable: 'true',
        resizable: 'true',
        closable: true,
        border: false,
        html: winURL,
        closeAction: 'hide'
    });
    win.show();
    return win;
}

function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName) {
    if (hierarchyBtnReference == 'vehiclelist') {
        selectedOrganizationHierarchyNodeCode = nodecode;
        selectedOrganizationHierarchyFleetName = fleetName;
        TempSelectedOrganizationHierarchyFleetId = fleetId;

        applyOrganizationHierarchy();
    }
    else if (hierarchyBtnReference == 'history') {
        HistoryOrganizationHierarchyNodeCode = nodecode;
        historyHiddenFleet.setValue(fleetId);

        historyOrganizationHierarchy.setText(fleetName);
        //loadingMask.show();
        historyVehicleStore.load(
        {
            params:
            {
                fleetID: fleetId
            }
        });
    }

}

function applyOrganizationHierarchy() {
    FleetBtn.setText("Fleet: <br/>" + selectedOrganizationHierarchyFleetName);
    $('#organizationHierarchyTree').hide();
    DefaultOrganizationHierarchyFleetId = TempSelectedOrganizationHierarchyFleetId;
    DefaultOrganizationHierarchyNodeCode = selectedOrganizationHierarchyNodeCode;
    selectedFleetId = [];
    if (DefaultOrganizationHierarchyFleetId.indexOf(',') > -1)
        selectedFleetId = DefaultOrganizationHierarchyFleetId.split(',');
    else
        selectedFleetId.push(DefaultOrganizationHierarchyFleetId);
}


function ShowHideColumnPreference() {
    

    Ext.getCmp('UnitID').hide();
    Ext.getCmp('LicensePlate').hide();
    Ext.getCmp('VehicleDescription').hide();
    Ext.getCmp('VehicleMake').hide();
    Ext.getCmp('VehicleModel').hide();
    Ext.getCmp('VINNumber').hide();
    Ext.getCmp('address').hide();
    Ext.getCmp('DriverName').hide();
    Ext.getCmp('ServiceName').hide();
    Ext.getCmp('ServiceType').hide();
    Ext.getCmp('DateTime').hide();
    Ext.getCmp('EventType').hide();
    Ext.getCmp('Driver_Class').hide();
    Ext.getCmp('Vehicle_Class').hide();
    Ext.getCmp('Field1').hide();
    Ext.getCmp('Field2').hide();
    Ext.getCmp('Field3').hide();
    Ext.getCmp('Field4').hide();
    Ext.getCmp('Field5').hide();
    Ext.getCmp('Note').hide();
    Ext.getCmp('ManagerId').hide();
    Ext.getCmp('ManagerName').hide();
    Ext.getCmp('color').hide();
    Ext.getCmp('LandmarkName').hide();
    Ext.getCmp('FleetName').hide();
    Ext.getCmp('EmployeeId').hide();
    Ext.getCmp('bscid').hide();
    var col
    if (table == "Event") {
        col = eventCol.split(',');
    }
    else {
        col = violationCol.split(',');
    }
    for (var i = 0; i < col.length; i++) {
        switch (col[i]) {
            case '1':
                Ext.getCmp('UnitID').show();
               
                break;

            case '2':
                Ext.getCmp('LicensePlate').show();
               
                break;
            case '3':
                Ext.getCmp('VehicleDescription').show();
                
                break;
            case '4':
                Ext.getCmp('VehicleMake').show();

                break;
            case '5':
                Ext.getCmp('VehicleModel').show();
                
                break;
            case '6':
                Ext.getCmp('VINNumber').show();
               
                break;
            case '7':
                Ext.getCmp('address').show();
                
                break;
            case '8':
                Ext.getCmp('DriverName').show();
               
                break;
            case '9':
                Ext.getCmp('ServiceName').show();
               
                break;
            case '10':
                Ext.getCmp('ServiceType').show();
                
                break;
            case '11':
                Ext.getCmp('DateTime').show();
                
                break;
            case '12':
                Ext.getCmp('EventType').show();
                
                break;
            case '13':
                Ext.getCmp('Driver_Class').show();
                
                break;
            case '14':
                Ext.getCmp('Vehicle_Class').show();
                  
                break;
            case '15':
                Ext.getCmp('Field1').show();
               
                break;
            case '16':
                Ext.getCmp('Field2').show();
               
                break;
            case '17':
                Ext.getCmp('Field3').show();
                
                break;
            case '18':
                Ext.getCmp('Field4').show();
               
                break;
            case '19':
                Ext.getCmp('Field5').show();
                
                break;
            case '20':
                Ext.getCmp('Note').show();
                
                break;
            case '21':
                Ext.getCmp('ManagerId').show();

                break;
            case '22':
                Ext.getCmp('ManagerName').show();

                break;
            case '23':
                Ext.getCmp('color').show();

                break;
            case '24':
                Ext.getCmp('LandmarkName').show();

                break;
            case '25':
                Ext.getCmp('FleetName').show();

                break;
            case '26':
                Ext.getCmp('bscid').show();
                break;
            case '27':
                Ext.getCmp('EmployeeId').show();
                break;
            default:
                break;

        }

        
    }


}

//For avoiding Next- sibling error for IE- 10 
(function () {

    // kill repeat to save bytes
    var detachedDiv = document.createElement('div');

    Ext.define('Ext.override.dom.Helper', {
        override: 'Ext.dom.AbstractHelper',

        ieTable: function (depth, openingTags, htmlContent, closingTags) {
            detachedDiv.innerHTML = [openingTags, htmlContent, closingTags].join('');

            var i = -1,
                el = detachedDiv,
                ns;
            while (++i < depth) {
                el = el.firstChild;
            }
            // If the result is multiple siblings, then encapsulate them into one fragment.
            ns = el.nextSibling;

            if (ns) {
                el = document.createDocumentFragment();
                while (ns) {
                    el.appendChild(ns);
                    ns = ns.nextSibling;
                }
            }
            return el;
        }

    }, function () {
        Ext.ns('Ext.core');
        Ext.DomHelper = Ext.core.DomHelper = new this;
    });

}());



