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
var hidImageloadingUnitHtml = "<div id='hidImageloadingUnit' style='width:100px;display:inline'><span style ='color:blue' >Sending...</span><image src ='../images/loading2.gif'  ></image></div>";
var hidImageDumpUnitHtml = "<div id='hidImageDumpUnit' style='width:100px;display:inline'><span style ='color:blue'>Sending...</span><image src ='../images/loading2.gif'  ></image></div>";
var hidImageMessageUnitHtml = "<div id='hidImageMessageUnit' style='width:100px;display:inline'><span style ='color:blue'>Sending...</span><image src ='../images/loading2.gif'  ></image></div>";


var GetSelectedVehicles = function (checkboxVechicleType) {
    var selectBoxids = "";
    var retValue = new Array();
    $('input:checkbox[id^="' + CheckboxVechicleId + checkboxVechicleType + '"]').each(function () {
        if (this.checked) {
            if (selectBoxids == "")
                selectBoxids = $(this).val();
            else selectBoxids = selectBoxids + "," + $(this).val();

            var vehicleid = $(this).attr('id').replace(CheckboxVechicleId, SpanVechicleId);
            retValue.push($('#' + vehicleid).text());
        }
    });
    if (selectBoxids != "")
       retValue.push(selectBoxids);
    return retValue;
}

var SendLoadingTimer = function (interval) {
    if (interval > 0)
    setTimeout(function () {
        CheckSendingData("1");
    }, interval);
    else CheckSendingData("1");
};

var SendDumpTimer = function (interval) {
    if (interval > 0)
    setTimeout(function () {
        CheckSendingData("2");
    }, interval);
    else CheckSendingData("2");
};


var SendEmgencyTimer = function (interval) {
    if (interval > 0)
    setTimeout(function () {
        CheckSendingData("3");
    }, interval);
    else CheckSendingData("3");

};

function ResetAllControls(checkboxVechicleType) {
    $('input:checkbox[id^="' + CheckboxVechicleId + checkboxVechicleType + '"]').each(function () {
        if (this.checked) {
            $(this).attr('checked', false);
        }
    });

}

function CheckSendingData(sendingType)
{

    Ext.Ajax.request({
        url: "AntLoadData.aspx/CheckSendingData",
        method: 'POST',
        jsonData: {
            sendingType: sendingType
        },
        success: function (result, request) {
            var ret = Ext.JSON.decode(result.responseText).d;
            if (ret == "-1") {
                window.open('../Login.aspx', '_top')
                return;
            }
            if (ret == "0") {
                if (sendingType == "1") {
                    SendLoadingTimer(500);
                }
                if (sendingType == "2") {
                    SendDumpTimer(500);
                }
                if (sendingType == "3") {
                    SendEmgencyTimer(500)
                }
                return;
            }

            if (ret.length > 1) {
                if (sendingType == "1") {
                    $(LoadingSendButton).attr('disabled', '');
                    //Ext.getCmp('hidImageloadingUnit').getEl().hide();
                    $('#hidImageloadingUnit').hide();
                    ResetAllControls(CheckboxVechicleType.Loading);
                    ShowResultWindow(eval(ret));
                }
                if (sendingType == "2") {
                    $(DumpSendButton).attr('disabled', '');
                    //Ext.getCmp('hidImageloadingUnit').getEl().hide();
                    $('#hidImageDumpUnit').hide();
                    ResetAllControls(CheckboxVechicleType.Dump);
                    ShowResultWindow(eval(ret));
                }
                if (sendingType == "3") {
                    $("#MessageSendButton").attr('disabled', '');
                    //Ext.getCmp('hidImageloadingUnit').getEl().hide();
                    $('#hidImageMessageUnit').hide();
                    ResetAllControls(CheckboxVechicleType.Message);
                    ShowResultWindow(eval(ret));

                }

                //var retVehicles = eval(ret);

                return;
            }
        },
        failure: function () {
            if (sendingType == "1") {
                SendLoadingTimer(500);
            }
            if (sendingType == "2") {
                SendDumpTimer(500);
            }
            if (sendingType == "3") {
                SendEmgencyTimer(500)
            }

        }
    });
}

var CheckboxVechicleType = {};

            CheckboxVechicleType.Loading = "_1_";

            CheckboxVechicleType.Dump = "_2_"

            CheckboxVechicleType.Message = "_3_";
var CheckboxVechicleId = "checkbox_vehicleid_";
var SpanVechicleId = "span_vehicleid_";
var CheckVehicle = function(ctl, checkboxVechicleType)
{
//alert(checkboxVechicleType);
    if ($(ctl).attr("id") == CheckboxVechicleId + checkboxVechicleType + "0")
    {
        $('input:checkbox[id^="' + CheckboxVechicleId + checkboxVechicleType + '"]').attr('checked', $(ctl).attr("checked"));
    }
    else
    {
        if ($(ctl).attr("checked") == false)
        {
           $('input:checkbox[id^="' + CheckboxVechicleId + checkboxVechicleType + '0"]').attr('checked', false);
        }
    }
}
var FleetComboMessage;

var selectedfleetId = '';
Ext.onReady(function () {
    // Define our data model
    // Define the model for a State
    var proxyTimeOut = 120000;
    Ext.define('Fleet', {
        extend: 'Ext.data.Model',
        fields: [
            { type: 'string', name: 'FleetId' },
            { type: 'string', name: 'FleetName' }
        ]
    });


    // The data store holding the states; shared by each of the ComboBox examples below
    var FleetStore = Ext.create('Ext.data.Store', {
        model: 'Fleet',
        proxy:
        {
            // load using HTTP
            type: 'ajax',
            filterOnLoad: true,
            url: '../Vehicles.aspx?QueryType=GetAllFleets',
            timeout: proxyTimeOut,
            reader:
             {
                 type: 'xml',
                 root: 'Fleet',
                 record: 'FleetsInformation'
             }
        },

        listeners: { 'load': function () {
            FleetStore.filterBy(function (record) {
                return (record.data['FleetName'] != 'All Vehicles');
            });
        }
        }
    });



    // Simple ComboBox using the data store
    var FleetCombo = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Fleet',
        displayField: 'FleetName',
        valueField: 'FleetId',
        value: 'Select a fleet',
        width: 280,
        labelWidth: 50,
        store: FleetStore,
        queryMode: 'local',
        triggerAction: 'all',
        typeAhead: true,
        listeners:
        {
            scope: this,
            'select': function (combo, value) {
                try {
                    var selFleet = combo.getValue();
                    selectedfleetId = selFleet;
                    var url = 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId;
                    LoadingStore.getProxy().url = url; //modify your URL
                    LoadingStore.load();
                    LoadingGrid.getView().refresh();
                    VehicleCombo.clearValue();
                }
                catch (err) {
                    alert("Failed to load data.");
                }
            },
            'afterrender': function () {
                FleetStore.load();
            }

        }
    });


    var FleetComboDump = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Fleet',
        displayField: 'FleetName',
        valueField: 'FleetId',
        value: 'Select a fleet',
        width: 280,
        labelWidth: 50,
        store: FleetStore,
        queryMode: 'local',
        typeAhead: true,
        listeners:
        {
            scope: this,
            'select': function (combo, value) {
                try {
                    var selFleet = combo.getValue();
                    selectedfleetId = selFleet;
                    var url = 'AntLoadData.aspx?type=GetAllDumpLocations&fleetId=' + selectedfleetId;
                    DumpStore.getProxy().url = url; //modify your URL
                    DumpStore.load();
                    DumpGrid.getView().refresh();

                    var url = 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId;
                    DumpVehicleStore.getProxy().url = url; //modify your URL
                    DumpVehicleStore.load();
                    VehicleComboDump.clearValue();

                }
                catch (err) {
                    alert("Failed to load data.");
                }
            },
            'afterrender': function () {
                FleetStore.load();
            }

        }
    });

    FleetComboMessage = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Fleet',
        displayField: 'FleetName',
        valueField: 'FleetId',
        value: 'Select a fleet',
        width: 280,
        labelWidth: 50,
        store: FleetStore,
        queryMode: 'local',
        typeAhead: true,
        listeners:
        {
            scope: this,
            'select': function (combo, value) {
                try {
                    var selFleet = combo.getValue();
                    selectedfleetId = selFleet;
                    var url = 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId;
                    MessageVehicleStore.getProxy().url = url; //modify your URL
                    MessageVehicleStore.load();
                    VehicleComboMessage.clearValue();

                }
                catch (err) {
                    alert("Failed to load data.");
                }
            },
            'afterrender': function () {
                FleetStore.load();
            }

        }
    });


    //Loading Function
    Ext.define('Loading', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'Boxid', type: 'int' },
            { name: 'Location', type: 'string' },
            { name: 'VehicleID', type: 'int' },
            { name: 'Description', type: 'string' },
            { name: 'eId', type: 'int' }
        ]
    });



    // The data store holding the states; shared by each of the ComboBox examples below
    var VehicleStore = Ext.create('Ext.data.Store', {
        model: 'Loading',
        proxy:
        {
            // load using HTTP
            type: 'ajax',
            filterOnLoad: true,
            //url: 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId,
            timeout: proxyTimeOut,
            reader:
             {
                 type: 'xml',
                 root: 'DocumentElement',
                 record: 'LoadingUnit'
             }
        }

    });


    var MessageVehicleStore = Ext.create('Ext.data.Store', {
        // destroy the store if the grid is destroyed
        autoDestroy: true,
        model: 'Loading',
        sorters: [{
            property: 'start',
            direction: 'ASC'
        }],
        proxy:
        {
            // load using HTTP
            type: 'ajax',
            url: 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId + "&type=3",
            timeout: proxyTimeOut,
            reader:
             {
                 type: 'xml',
                 root: 'DocumentElement',
                 record: 'LoadingUnit'
             }
        },

        listeners: { 'load': function () {

            //VehicleComboMessage.store.removeAll();
            //VehicleComboMessage.store.add(MessageVehicleStore.getRange());
        }
        }

    });


    var DumpVehicleStore = Ext.create('Ext.data.Store', {
        // destroy the store if the grid is destroyed
        autoDestroy: true,
        model: 'Loading',
        sorters: [{
            property: 'start',
            direction: 'ASC'
        }],
        proxy:
        {
            // load using HTTP
            type: 'ajax',
            url: 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId + "&type=2",
            timeout: proxyTimeOut,
            reader:
             {
                 type: 'xml',
                 root: 'DocumentElement',
                 record: 'LoadingUnit'
             }
        },

        listeners: { 'load': function () {
            DumpVehicleStore.filterBy(function (record) {
                return (record.data['eId'] == 1);
            });
            //VehicleComboDump.store = "DumpVehicleStore";
            //VehicleComboDump.store.removeAll();
            //VehicleComboDump.store.add(DumpVehicleStore.getRange());
        }
        }

    });

    // create the Data Store
    var LoadingStore = Ext.create('Ext.data.Store', {
        // destroy the store if the grid is destroyed
        autoDestroy: true,
        model: 'Loading',
        sorters: [{
            property: 'start',
            direction: 'ASC'
        }],
        proxy:
        {
            // load using HTTP
            type: 'ajax',
            url: 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId + "&type=1",
            timeout: proxyTimeOut,
            reader:
             {
                 type: 'xml',
                 root: 'DocumentElement',
                 record: 'LoadingUnit'
             }
        },

        listeners: { 'load': function () {
            VehicleCombo.store.removeAll();
            VehicleCombo.store.add(LoadingStore.getRange());
            LoadingStore.filterBy(function (record) {
                return (record.data['eId'] != 1 && record.data['Description'] != 'All Vehicles');
            });
        }
        }

    });

    var VehicleComboMessage = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Vehicle',
        displayField: 'Description',
        valueField: 'Boxid',
        emptyText: 'Select a vehilce',
        maxHeight: 350,
        width: 280,
        labelWidth: 50,

        //multiSelect: true,
        //forceSelection: true,

        editable: true,
        store: MessageVehicleStore,
        queryMode: 'local',
        triggerAction: 'all',
        //typeAhead: true,
        selectOnFocus: true,
        //tpl: '<div style="height:300px; overflow-y:scroll; border:1px solid;"><tpl for="." ><div class="x-combo-list-item" ><span style="margin-bottom:2cm"><input id="' + CheckboxVechicleId + CheckboxVechicleType.Loading + '{Boxid}" onclick="CheckVehicle(this,\'' + CheckboxVechicleType.Loading + '\')" type="checkbox" value="{Boxid}" /></span><span style="padding-left:4px" >{Description}</span></div></tpl></div>'
        tpl: '<tpl for="." ><div class="x-combo-list-item" onmouseover="this.style.backgroundColor=\'#DEEBF7\'" onmouseout="this.style.backgroundColor=\'#ffffff\'"><span style="margin-bottom:2cm"><input id="' + CheckboxVechicleId + CheckboxVechicleType.Message + '{Boxid}" onclick="CheckVehicle(this,\'' + CheckboxVechicleType.Message + '\')" type="checkbox" value="{Boxid}" /></span><span  id="' + SpanVechicleId + CheckboxVechicleType.Message + '{Boxid}" style="padding-left:4px;cursor: pointer;"   >{Description}</span></div></tpl>'
    });

    // Simple ComboBox using the data store
    var VehicleCombo = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Vehicle',
        displayField: 'Description',
        valueField: 'Boxid',
        emptyText: 'Select a vehilce',
        maxHeight: 350,
        width: 280,
        labelWidth: 50,

        //multiSelect: true,
        //forceSelection: true,

        editable: true,
        store: VehicleStore,
        queryMode: 'local',
        triggerAction: 'all',
        //typeAhead: true,
        selectOnFocus: true,
        //tpl: '<div style="height:300px; overflow-y:scroll; border:1px solid;"><tpl for="." ><div class="x-combo-list-item" ><span style="margin-bottom:2cm"><input id="' + CheckboxVechicleId + CheckboxVechicleType.Loading + '{Boxid}" onclick="CheckVehicle(this,\'' + CheckboxVechicleType.Loading + '\')" type="checkbox" value="{Boxid}" /></span><span style="padding-left:4px" >{Description}</span></div></tpl></div>'
        tpl: '<tpl for="." ><div class="x-combo-list-item" onmouseover="this.style.backgroundColor=\'#DEEBF7\'" onmouseout="this.style.backgroundColor=\'#ffffff\'"><span style="margin-bottom:2cm"><input id="' + CheckboxVechicleId + CheckboxVechicleType.Loading + '{Boxid}" onclick="CheckVehicle(this,\'' + CheckboxVechicleType.Loading + '\')" type="checkbox" value="{Boxid}" /></span><span  id="' + SpanVechicleId + CheckboxVechicleType.Loading + '{Boxid}" style="padding-left:4px;cursor: pointer;"   >{Description}</span></div></tpl>'
        //        listConfig: {
        //            getInnerTpl: function () {
        //            return '<div class="x-combo-list-item"><span style="margin-bottom:2cm "><input id="' + CheckboxVechicleId + CheckboxVechicleType.Loading + '{Boxid}" onclick="CheckVehicle(this,\'' + CheckboxVechicleType.Loading + '\')" type="checkbox" value="{Boxid}" /></span><span style="padding-left:4px" >{Description}</span></div>';
        //                //return '<div ><img class="chkCombo-default-icon chkCombo"></img> {Description} </div>';
        //            }
        //}
    });



    var VehicleComboDump = Ext.create('Ext.form.field.ComboBox', {
        fieldLabel: 'Vehicle',
        displayField: 'Description',
        valueField: 'Boxid',
        emptyText: 'Select a vehilce',
        maxHeight: 350,
        width: 280,
        labelWidth: 50,

        //multiSelect: true,
        //forceSelection: true,

        editable: true,
        store: DumpVehicleStore,
        queryMode: 'local',
        triggerAction: 'all',
        //typeAhead: true,
        selectOnFocus: true,
        //tpl: '<div style="height:300px; overflow-y:scroll; border:1px solid;"><tpl for="." ><div class="x-combo-list-item" ><span style="margin-bottom:2cm"><input id="' + CheckboxVechicleId + CheckboxVechicleType.Loading + '{Boxid}" onclick="CheckVehicle(this,\'' + CheckboxVechicleType.Loading + '\')" type="checkbox" value="{Boxid}" /></span><span style="padding-left:4px" >{Description}</span></div></tpl></div>'
        tpl: '<tpl for="." ><div class="x-combo-list-item" onmouseover="this.style.backgroundColor=\'#DEEBF7\'" onmouseout="this.style.backgroundColor=\'#ffffff\'"><span style="margin-bottom:2cm"><input id="' + CheckboxVechicleId + CheckboxVechicleType.Dump + '{Boxid}" onclick="CheckVehicle(this,\'' + CheckboxVechicleType.Dump + '\')" type="checkbox" value="{Boxid}" /></span><span  id="' + SpanVechicleId + CheckboxVechicleType.Dump + '{Boxid}" style="padding-left:4px;cursor: pointer;"   >{Description}</span></div></tpl>'
    });


    var LoadingSendButton = Ext.create('Ext.Button', {
        text: 'Send',
        width: 100,
        id: "LoadingSendButton",
        listeners: {
            click: function () {
                // this == the button, as we are in the local scope
                var selectBoxIds = GetSelectedVehicles(CheckboxVechicleType.Loading);
                if (selectBoxIds.length == 0) {
                    //if (VehicleCombo.getValue().length <= 0) {
                    alert("Please select vehicle(s).");
                    return;
                }
                var fleetId = FleetCombo.getValue();

                Ext.Ajax.request({
                    url: "AntLoadData.aspx/SendSyncRequest",
                    method: 'POST',
                    jsonData: {
                        boxids: selectBoxIds,
                        fleetId: fleetId
                    },
                    success: function (result, request) {
                        var ret = Ext.JSON.decode(result.responseText).d;
                        if (ret == "-1") {
                            window.open('../Login.aspx', '_top')
                            return;
                        }
                        if (ret == "0") {
                            alert("Failed to send.");
                            return;
                        }
                        if (ret == "1") {
                            //Ext.getCmp('hidImageloadingUnit').getEl().show();
                            $('#hidImageloadingUnit').show();
                            $("#LoadingSendButton").attr('disabled', 'disabled');
                            SendLoadingTimer(0);
                            return;
                        }
                        //store.load();
                    },
                    failure: function () {
                        //Ext.Msg.alert("Alert", "Failed to update.");
                        alert("Failed to send.");
                    }
                });
            }
        }
    });


    var DumpSendButton = Ext.create('Ext.Button', {
        text: 'Send',
        width: 100,
        id: "DumpSendButton",
        listeners: {
            click: function () {
                // this == the button, as we are in the local scope
                var selectBoxIds = GetSelectedVehicles(CheckboxVechicleType.Dump);
                if (selectBoxIds.length == 0) {
                    //if (VehicleCombo.getValue().length <= 0) {
                    alert("Please select vehicle(s).");
                    return;
                }
                var fleetId = FleetComboDump.getValue();

                Ext.Ajax.request({
                    url: "AntLoadData.aspx/SendSyncDumpRequest",
                    method: 'POST',
                    jsonData: {
                        boxids: selectBoxIds,
                        fleetId: fleetId
                    },
                    success: function (result, request) {
                        var ret = Ext.JSON.decode(result.responseText).d;
                        if (ret == "-1") {
                            window.open('../Login.aspx', '_top')
                            return;
                        }
                        if (ret == "0") {
                            alert("Failed to send.");
                            return;
                        }
                        if (ret == "1") {
                            //Ext.getCmp('hidImageloadingUnit').getEl().show();
                            $('#hidImageDumpUnit').show();
                            $("#DumpSendButton").attr('disabled', 'disabled');
                            SendDumpTimer(0);
                            return;
                        }
                        //store.load();
                    },
                    failure: function (e) {
                        //Ext.Msg.alert("Alert", "Failed to update.");
                        alert("Failed to send.");
                    }
                });
            }
        }
    });
    var FleetVehicle = Ext.create('Ext.Panel', {
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:3px;border:0;background-color:transparent;',
        width: 800,
        layout: {
            type: 'column',
            margin: 20
        }, // arrange fieldsets side by side
        defaults: {
            width: 800,
            labelWidth: 90
        },
        header: false,
        defaultType: 'textfield',
        items: [{ xtype: FleetCombo }, VehicleCombo, LoadingSendButton
        ]
    });


    var FleetVehicleDump = Ext.create('Ext.Panel', {
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:3px;border:0;background-color:transparent;',
        width: 800,
        layout: {
            type: 'column',
            margin: 10
        }, // arrange fieldsets side by side
        defaults: {
            width: 800,
            labelWidth: 90
        },
        header: false,
        defaultType: 'textfield',
        items: [FleetComboDump, VehicleComboDump, DumpSendButton]
    });

    var FleetVehicleMessage = Ext.create('Ext.Panel', {
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:3px;border:0;background-color:transparent;',
        width: 800,
        layout: {
            type: 'column',
            margin: 10
        }, // arrange fieldsets side by side
        defaults: {
            width: 800,
            labelWidth: 90
        },
        header: false,
        defaultType: 'textfield',
        items: [FleetComboMessage, VehicleComboMessage]
    });


    var MessageForm = Ext.create('Ext.Panel', {
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:3px;border:0;background-color:transparent;',
        width: 800,
        id: "MessageForm",
        layout: {
            type: 'column',
            margin: 10
        }, // arrange fieldsets side by side
        defaults: {
            width: 800,
            labelWidth: 90
        },
        header: false,
        title: 'Panic Alert',
        defaultType: 'textfield',
        items: [FleetVehicleMessage,
          {
              xtype: 'panel',
              width: 800,
              height: 500,
              border: false,
              autoScroll: true,
              loader: {
                  autoLoad: true,
                  url: 'Emergency.html'
              }
          }
]
    });


    var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 1,
        autoCancel: false,
        pluginId: 'rowEditing'
    });
    rowEditing.on({
        scope: this,
        edit: function (roweditor, event) {
            //alert(roweditor.record);
            var loc = event.record.data["Location"];
            var veh = event.record.data["VehicleID"];
            var box = event.record.data["Boxid"];
            if (loc != undefined && loc != null) {
                loc = $.trim(loc);
                event.record.data["Location"] = loc;
                Ext.Ajax.request({
                    url: "AntLoadData.aspx/UpdateLoadingUnit",
                    method: 'POST',
                    jsonData: {
                        box: box,
                        vehicle: veh,
                        location: loc
                    },
                    success: function (result, request) {
                        var ret = Ext.JSON.decode(result.responseText).d;
                        if (ret == "-1")
                            window.open('../Login.aspx', '_top')
                        if (ret == "0") {
                            event.record.reject();
                            alert("Failed to update.");
                        }
                        if (ret == "1") {
                            alert("Updated successfully.");
                        }
                        //store.load();
                    },
                    failure: function () {
                        //Ext.Msg.alert("Alert", "Failed to update.");
                        event.record.reject();
                        alert("Failed to update.");
                    }
                });

            }
        }
        //      afteredit: function(roweditor, changes, record, rowIndex) {
        //        var a= "1";
        //        //your save logic here - might look something like this:
        //        //Ext.Ajax.request({
        //        //  url   : record.phantom ? '/users' : '/users/' + record.get('user_id'),
        //        //  method: record.phantom ? 'POST'   : 'PUT',
        //        //  params: changes,
        //         // success: function() {
        //            //post-processing here - this might include reloading the grid if there are calculated fields
        //          //}
        //        //});
        //      }
    });


    // create the grid and specify what field you want
    // to use for the editor at each column.
    var LoadingGrid = Ext.create('Ext.grid.Panel', {
        store: LoadingStore,
        id: 'LoadingGrid',
        columns: [{
            header: 'Loading Unit',
            dataIndex: 'Description',
            width: 250,
            editor: {
                // defaults to textfield if no xtype is supplied
                allowBlank: false
            }
        }, {
            header: 'Loading Area',
            dataIndex: 'Location',
            width: 250,
            editor: {
                allowBlank: true
            }
        }],
        width: 800,
        height: 600,
        title: 'Loading Unit Location',
        frame: true,

        dockedItems: {
            dock: 'top',
            items: [FleetVehicle]
        },
        tbar: [
        /*{
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
        }, 
        */
        //        {
        //        itemId: 'Send',
        //        text: 'Send',
        //        iconCls: 'send',
        //        handler: function () {
        //            if (confirm("Are you sure you want to send?")) {

        //            }
        //        },
        //        disabled: false
        //    }
 ],
        plugins: [rowEditing],
        listeners: {
            'selectionchange': function (view, records) {
                try {
                    LoadingGrid.down('#removeLoadingLocation').setDisabled(!records.length);
                }
                catch (err) { }
            }
        }
    });

    LoadingGrid.on('beforeedit', function (e) {
        LoadingGrid.getPlugin('rowEditing').editor.form.findField('Description').disable();
    });

    //Dump Function
    Ext.define('Dump', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'DumpLocation', type: 'string' },
            { name: 'Id', type: 'int' }
        ]
    });


    // create the Data Store
    var DumpStore = Ext.create('Ext.data.Store', {
        // destroy the store if the grid is destroyed
        autoDestroy: true,
        model: 'Dump',
        proxy:
    {
        // load using HTTP
        type: 'ajax',
        url: 'AntLoadData.aspx?type=GetAllDumpLocations&fleetId=' + selectedfleetId,
        timeout: proxyTimeOut,
        reader:
            {
                type: 'xml',
                root: 'DocumentElement',
                record: 'DumpingLocation'
            }
    },
        sorters: [{
            property: 'start',
            direction: 'ASC'
        }]
    });

    var DumprowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 0,
        autoCancel: true,
        pluginId: 'DumprowEditing'
    });


    DumprowEditing.on({
        scope: this,
        canceledit: function (roweditor, event) {
            var Id = event.record.data["Id"];
            if (Id == -1) DumpStore.removeAt(0);
            if (DumpStore.getAt(0).get('Id') == -1) DumpStore.removeAt(0);
        },
        beforeedit: function (roweditor, event) {
            var Id = event.record.data["Id"];
            if (Id > 0) {
                if (DumpStore.getAt(0).get('Id') == -1) DumpStore.removeAt(0);
            }
        },

        edit: function (roweditor, event) {
            //alert(roweditor.record);
            var Id = event.record.data["Id"];
            var fleetId = FleetComboDump.getValue();
            if (isNaN(fleetId)) {
                alert("Please select a fleet.");
                return;
            }
            var DumpLocation = event.record.data["DumpLocation"];
            if (DumpLocation != undefined && DumpLocation != null) {
                if ($.trim(DumpLocation) == '') {
                    alert('Dump Location is required');
                    return;
                }
                DumpLocation = $.trim(DumpLocation);
                event.record.data["DumpLocation"] = DumpLocation;
                Ext.Ajax.request({
                    url: "AntLoadData.aspx/UpdateDumpLocation",
                    method: 'POST',
                    jsonData: {
                        Id: Id,
                        Fleet: fleetId,
                        location: DumpLocation,
                        IsDelete: false
                    },
                    success: function (result, request) {
                        var ret = Ext.JSON.decode(result.responseText).d;
                        if (ret == "-1")
                            window.open('../Login.aspx', '_top')
                        if (ret == "0") {
                            event.record.reject();
                            alert("Failed to update.");
                        }
                        if (ret == "1") {
                            alert("Updated successfully.");
                            if (Id == -1) {
                                try {
                                    var url = 'AntLoadData.aspx?type=GetAllDumpLocations&fleetId=' + fleetId;
                                    DumpStore.getProxy().url = url; //modify your URL
                                    DumpStore.load();
                                    DumpGrid.getView().refresh();

                                    //var url = 'AntLoadData.aspx?type=GetAllLoadingUnits&fleetId=' + selectedfleetId;
                                    //DumpVehicleStore.getProxy().url = url; //modify your URL
                                    //DumpVehicleStore.load();
                                    //VehicleComboDump.clearValue();

                                }
                                catch (err) {
                                    alert("Failed to load data.");
                                }
                            }
                        }
                        //store.load();
                    },
                    failure: function (e) {
                        //Ext.Msg.alert("Alert", "Failed to update.");
                        event.record.reject();
                        alert("Failed to update.");
                    }
                });
            }
        }
        //      afteredit: function(roweditor, changes, record, rowIndex) {
        //        var a= "1";
        //        //your save logic here - might look something like this:
        //        //Ext.Ajax.request({
        //        //  url   : record.phantom ? '/users' : '/users/' + record.get('user_id'),
        //        //  method: record.phantom ? 'POST'   : 'PUT',
        //        //  params: changes,
        //         // success: function() {
        //            //post-processing here - this might include reloading the grid if there are calculated fields
        //          //}
        //        //});
        //      }
    });
    // create the grid and specify what field you want
    // to use for the editor at each column.
    var DumpGrid = Ext.create('Ext.grid.Panel', {
        store: DumpStore,
        id: 'DumpGrid',
        columns: [{
            header: 'Dump Location',
            dataIndex: 'DumpLocation',
            width: 400,
            editor: {
                // defaults to textfield if no xtype is supplied
                allowBlank: true
            }
        }],
        width: 550,
        height: 500,
        title: 'Dump Location',
        frame: true,
        dockedItems: {
            dock: 'top',
            items: [FleetVehicleDump]
        },
        tbar: [{
            text: 'Add Dump Location',
            iconCls: 'add',
            handler: function () {
                DumprowEditing.cancelEdit();

                // Create a record instance through the ModelManager
                var r = Ext.ModelManager.create({
                    Id: -1,
                    DumpLocation: ''
                }, 'Dump');

                DumpStore.insert(0, r);
                DumprowEditing.startEdit(0, 0);
            }
        }, {
            itemId: 'removeDumpLocation',
            text: 'Remove Dump Location',
            iconCls: 'remove',
            handler: function () {
                if (confirm("Are you sure you want to remove selected record?")) {
                    var sm = DumpGrid.getSelectionModel();
                    DumprowEditing.cancelEdit();
                    var Id = sm.getSelection()[0].data["Id"]
                    var fleetId = FleetComboDump.getValue();
                    var DumpLocation = sm.getSelection()[0].data["DumpLocation"];
                    if (isNaN(fleetId)) {
                        alert("Please select a fleet.");
                        return;
                    }
                    if (Id != undefined && Id != null) {
                        Ext.Ajax.request({
                            url: "AntLoadData.aspx/UpdateDumpLocation",
                            method: 'POST',
                            jsonData: {
                                Id: Id,
                                Fleet: fleetId,
                                location: DumpLocation,
                                IsDelete: true
                            },
                            success: function (result, request) {
                                var ret = Ext.JSON.decode(result.responseText).d;
                                if (ret == "-1")
                                    window.open('../Login.aspx', '_top')
                                if (ret == "0") {
                                    event.record.reject();
                                    alert("Failed to delete.");
                                }
                                if (ret == "1") {
                                    alert("Deleted successfully.");
                                    DumpStore.remove(sm.getSelection());
                                    sm.select(0)
                                }
                                //store.load();
                            },
                            failure: function (e) {
                                //Ext.Msg.alert("Alert", "Failed to update.");
                                event.record.reject();
                                alert("Failed to delete.");
                            }
                        });
                    }


                }
            },
            disabled: true
        }],
        plugins: [DumprowEditing],
        listeners: {
            'selectionchange': function (view, records) {
                try {
                    DumpGrid.down('#removeDumpLocation').setDisabled(!records.length);
                }
                catch (e) { }
            }
        }
    });


    var myForm = new Ext.form.FormPanel({
        //title: "Basic Form",
        width: 425,
        frame: true,
        items: [
            new Ext.form.TextField({
                id: "to",
                fieldLabel: "To",
                width: 275,
                allowBlank: false,
                blankText: "Please enter a to address",
                readOnly: false
            }),
            new Ext.form.TextField({
                id: "subject",
                fieldLabel: "Subject",
                width: 275,
                allowBlank: false,
                blankText: "Please enter a subject address",
                readOnly: true
            }),
        ],
        buttons: [
            { text: "Cancel" },
            { text: "Save" }
        ]
    });

    var sendWindow = new Ext.Window({
        id: 'id_sendWindow',
        title: 'Send',
        closable: true,
        width: 750,
        height: 380,
        plain: true,
        layout: 'fit',
        items: myForm
    });

    var DispatchTab = Ext.create('Ext.Panel', {
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:3px;border:0;background-color:transparent;',
        width: 800,
        id: "DispatchTab",
        layout: {
            type: 'column',
            margin: 10
        }, // arrange fieldsets side by side
        defaults: {
            width: 800,
            labelWidth: 90
        },
        header: false,
        title: 'Dispatch',
        defaultType: 'textfield'

    });

    var tabs = Ext.createWidget('tabpanel', {
        renderTo: 'tabs1',
        width: 800,
        height: 700,
        activeTab: 0,
        defaults: {
            bodyPadding: 10
        },
        items: [LoadingGrid, DumpGrid, MessageForm],
        listeners:
            {
                //                render: function () {
                //                    this.items.each(function (i) {
                //                        i.tab.on('click', function (el, e) {
                //                            if (i.id == "DispatchTab") {
                //                                e.stopEvent();
                //                                window.open('../Ant/Ant.html', 'Dispatch', 'width=800,height=800,left=0,top=100,screenX=0,screenY=100');
                //                            }
                //                        });
                //                    });
                //                },
                tabchange: function (tabPanel, newCard, oldCard, eOpts) {
                    //if (newCard.id == '1') {
                    //    historyByLocation.setValue('1');
                    //}
                    //else {
                    //    historyByLocation.setValue('0');
                    //}
                    if (newCard.id == "DumpGrid") {
                        if ($("#DumpSendButton").next().attr('id') != "hidImageDumpUnit") {
                            $("#DumpSendButton").after(hidImageDumpUnitHtml);
                            $("#hidImageDumpUnit").hide();
                        }
                    }
                    if (newCard.id == "MessageForm") {
                        if ($("#MessageSendButton").next().attr('id') != "hidImageMessageUnit") {
                            $("#MessageSendButton").after(hidImageMessageUnitHtml);
                            $("#hidImageMessageUnit").hide();
                        }
                        SetMessageButtonClick();

                    }



                }
            }
    });
    //tabs.add(DispatchTab);
    //    tabs.items.each(function (i) {
    //        if (i.id == "DispatchTab") {
    //            i.tab.on('click', function (el, e) {
    //                e.stopEvent();
    //                window.open('../Ant/Ant.html', 'Dispatch', 'width=800,height=800,left=0,top=100,screenX=0,screenY=100');
    //            });
    //        }
    //    });
    //    // var tabPanel = Ext.getCmp('myTabPanel');
    //var tabToHide = Ext.getCmp('myTab');
    //getCmp('hidImageloadingUnit').getEl().hide();
    $("#LoadingSendButton").after(hidImageloadingUnitHtml);
    $("#hidImageloadingUnit").hide();


});

function showPreviewWindow(htmlData) {

    var previewWindow = new Ext.Window({
        title: "Send Result",
        width: 600,
        id: 'previewWindow',
        autoHeight: true,
        html: htmlData,
        modal: true,
        y: 150,
        listeners: {
            beforeclose: function () {
                searchVisible = false;
            }
//            afterrender: {
//                fn: function (win) {
//                    var f = win.down('#prefForm');
//                    f.doLayout();
//                    var h = f.body.dom.scrollHeight;
//                    if (f.getHeight() > h)
//                        h = f.getHeight();
//                    win.setHeight(h + 61);
//                    win.center();
//                },
//                single: true
//            }
        },
        buttons: [
                 {
                     text: 'Close', handler: function () {
                         previewWindow.close();
                     }
                 }
                ]
    });

    previewWindow.show(this);
}


function ShowResultWindow(resultVehicles) {
    var str = '<table cellspacing="1" width="600px" cellpadding="3" border="1" style="background-color:White;border-color:White;border-width:2px;border-style:Ridge;font-size:12px" ><tr><td style ="font-weight:bold;width:400px">Vehicle</td><td style ="font-weight:bold;width:400px">Status</td></tr>';

    for (var index = 0; index < resultVehicles.length; index++) {
         var color = "style='color:green;'";
         if (resultVehicles["Status"] == "Pending")
             color = "style='color:blue;'";
         if (resultVehicles["Status"] == "Fail")
             color = "style='color:red;'";

         str = str + "<tr>" + "<td>" + resultVehicles[index]["Vehicle"] + "</td>" 
                   +"<td " + color + ">" + resultVehicles[index]["Status"] + "</td>" + 
                     "</tr>";
     }
     str = str + "</table>";
     showPreviewWindow(str);
 }

 function SetMessageButtonClick() {
     $("#MessageSendButton").unbind('click');
     $("#MessageSendButton").click(function () {
         var selectBoxIds = GetSelectedVehicles(CheckboxVechicleType.Message);
         if (selectBoxIds.length == 0) {
             //if (VehicleCombo.getValue().length <= 0) {
             alert("Please select vehicle(s).");
             return;
         }
         var fleetId = FleetComboMessage.getValue();
         var txtmessage = $.trim($("#txtMessage").val());
         if (txtmessage == "") {
             alert("Please input message.");
             return;
         }

         var schPeriod = $("#cboSchPeriod").val();
         var schInterval = $("#cboSchInterval").val();

         Ext.Ajax.request({
             url: "AntLoadData.aspx/SendMessageRequest",
             method: 'POST',
             jsonData: {
                 boxids: selectBoxIds,
                 fleetId: fleetId,
                 message: txtmessage,
                 schPeriod: schPeriod,
                 schInterval: schInterval
             },
             success: function (result, request) {
                 var ret = Ext.JSON.decode(result.responseText).d;
                 if (ret == "-1") {
                     window.open('../Login.aspx', '_top')
                     return;
                 }
                 if (ret == "0") {
                     alert("Failed to send.");
                     return;
                 }
                 if (ret == "1") {
                     //Ext.getCmp('hidImageloadingUnit').getEl().show();
                     $('#hidImageMessageUnit').show();
                     $("#MessageSendButton").attr('disabled', 'disabled');
                     SendEmgencyTimer(0);
                     return;
                 }
                 //store.load();
             },
             failure: function () {
                 //Ext.Msg.alert("Alert", "Failed to update.");
                 alert("Failed to send.");
             }
         });
     });
 }