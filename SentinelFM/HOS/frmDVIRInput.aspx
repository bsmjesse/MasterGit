<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmDVIRInput.aspx.cs" Inherits="HOS_frmDVIRInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all.css" />
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
    <style>
         .chkgrp {height:30px;}
    </style>
</head>
<body>
    <form id="form1" runat="server" >
    <div id="insInput">
        <table cellspacing="10">
            <tr>
                <td colspan="2" id="inspectionGroup">Inspection Type: 
                    <select onchange="changeInspectionType()" id="inspectionType" name="inspectionGroup">
                        <option value="55" selected>Canada Truck / Trailer Inspection</option>
                        <option value="56">Canada Bus Inspection</option>
                        <option value="57">USA Inspection</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr width="100%" />
                </td>
            </tr>
            <tr>
                <td style="white-space:nowrap">
                    <table>
                        <tr>
                            <td><span id="spanDate"></span></td>
                            <td>&nbsp;</td>
                            <td><span id="spanTime"></span></td>
                        </tr>
                    </table>
                </td>
                <td><input type="radio" name="insType" value="1" checked>Pre-Trip &nbsp;<input type="radio" name="insType" value="2">Post-Trip</td>
            </tr>
            <tr>
                <td><span id="spanDriver"></span></td>
                <td>Driver's Signature Present: <input type="checkbox" value="1" id="signed" name="signed" /></td>
            </tr>
            <tr>
                <td><span id="spanVehicle"></span></td>
                <td><span id="spanTrailer"></span></td>
            </tr>
            <tr>
                <td>Odometer: <input id="odometer" name="odometer" /></td>
                <td></td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr width="100%" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    Inspection List:<br />
                    <span id="spanInspectionItems"></span>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr width="100%" />
                </td>
            </tr>
            <tr>
                <td><input type="radio" name="satisfied" value="1" checked>Satisfied to Drive</td>
                <td><input type="radio" name="satisfied" value="0" >Unsitisfied to Drive</td>
            </tr>
            <tr>
                <td colspan="2">
                    Remarks:<br />
                        <textarea rows="5" cols="100" id="remark" name="remark"></textarea> 
                </td>
            </tr>
            <tr>
                <td><input type="checkbox" value="1" id="defectsCorrected" name="defectsCorrected" /> Defects Corrected</td>
                <td style="white-space:nowrap"><input type="checkbox" value="1" id="defectsSignedRepairer" name="defectsSignedRepairer" /> Defects signed off by Mechanic / Repairer</td>
            </tr>
            <tr>
                <td style="white-space:nowrap"><input type="checkbox" value="1" id="defectsNoNeedCorrect" name="defectsNoNeedCorrect" /> Defects Need Not Be Corrected</td>
                <td><input type="checkbox" value="1" id="defectsSignedDriver" name="defectsSignedDriver" /> Defects signed off by Driver</td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr width="100%" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align:center">
                    <input type="button" value ="Submit" onclick="SaveInspection()" />
                    <input type="button" value ="Cancel" onclick="window.location.reload()" />
                </td>
            </tr>
        </table>
    </div>
    </form>
        <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />   
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>    
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext/adapter/ext/ext-base.js"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>
    <script type="text/javascript">
        function getQueryStringValue(key) {
            return unescape(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + escape(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
        }

        function toLower(text) {
            if (text == null) {
                return null;
            } else {
                return text.toString().toLowerCase();
            }
        }

        function makeDateFields() {

            $("#spanDate").each(function (i, obj) {
                obj.innerHTML = "";
            });

            $("#spanTime").each(function (i, obj) {
                obj.innerHTML = "";
            });

            dateControl = new Ext.form.DateField({
                fieldLabel: '',
                format: 'd/m/Y',
                allowBlank: false,
                renderTo: spanDate,
                fieldLabel: 'Date',
                labelWidth: 30,
                width: 150,
                name: 'insDate',
                id: 'insDateId',
                value: ''
            });

            timeControl = new Ext.form.field.Time({
                renderTo: 'spanTime',
                allowBlank: false,
                name: 'insTime',
                format: 'H:i', 
                fieldLabel: 'Time',
                minValue: '0:00',
                maxValue: '23:59',
                anchor: '100%',
                labelWidth: 30,
                width: 100,
                value: '00:00'
            });
        }

        function displayVehicleList(list, type) {

            var listName = null;
            var spanId = null;
            var labelText = null;
            var lableId = null;
            var allowblank = null;

            if (type == 4) {
                listName = 'trailer';
                spanId = 'spanTrailer';
                labelText = 'Trailer';
                lableId = 'trailerId';
                allowblank = true;
            } else {
                listName = 'vehicle';
                spanId = 'spanVehicle';
                labelText = 'Vehicle';
                lableId = 'vehicleId';
                allowblank = false;
            }

            $("#" + spanId)[0].innerHTML = "";
            var simpleCombo = Ext.create('Ext.form.field.ComboBox', {
                id: listName,
                queryMode: "local",
                fieldLabel: labelText,
                validateOnChange: true,
                displayField: 'display',
                valueField: 'VehicleId',
                value: '',
                hiddenName: lableId,
                width: 260,
                listWidth : 320,
                labelWidth: 40,
                queryMode: 'local',
                lazyRender: true,
                typeAhead: true,
                typeAheadDelay: 250,
                triggerAction: 'all',
                minChars: 3,
                queryDelay: 1000,
                allowBlank: allowblank,
                name: listName,
                matchFieldWidth: false,
                renderTo: spanId,
                multiSelect: false,
                filterOnLoad: true,
                // prevent expand performance issue
                filters: [                  // prevent expand performance issue
                  function (item) {          // prevent expand performance issue
                      return item.index < 10; // prevent expand performance issue
                  }                         // prevent expand performance issue
                ],                         // prevent expand performance issue
                activeFilter: true, // workaround to allow me.clearFilter() inside Ext.form.field.ComboBox
                store: new Ext.data.SimpleStore({
                    fields: ['VehicleId', 'Description', 'LicensePlate', {
                        name: 'display',
                        convert: function (v, rec) {
                            return rec.data.Description + ' - ' + rec.data.LicensePlate;
                        }
                    }],
                    noCache: false, // allow browser cache
                    data: list
                }),
                enableKeyEvents: true,
                listeners: {
                    afterrender: function (combo) {
                        combo.expand();
                        combo.collapse();
                        combo.reset();
                    },
                    keydown: function (combo, event) {
                        if (event.getKey() == 18) {
                            var store = this.store;
                            //store.suspendEvents();
                            combo.collapse();
                            store.clearFilter();
                            store.filter({
                                property: 'display',
                                anyMatch: true,
                                value: this.getValue(),
                                caseSensitive: false
                            });
                            combo.expand();
                        }
                    }
                }
            });

        }

        function getVehicleLists(VehicleType) { //1-Truck, 2-Bus, 3-Truck and Bus, 4-Trailer
            var listData = null;
            if (VehicleType == 1) {
                if (truckTrailerListData != null) {
                    listData = truckTrailerListData;
                }
            } else if (VehicleType == 2) {
                if (busListData != null) {
                    listData = truckTrailerListData;
                }
            } else if (VehicleType == 3) {
                if (usVehicleListData != null) {
                    listData = truckTrailerListData;
                }
            } else if (VehicleType == 4) {
                if (trailerListData != null) {
                    return;
                }
            }

            if (listData != null) {
                displayVehicleList(listData, VehicleType);
                return;
            }


            Ext.Ajax.request({
                url: 'frmDVIRInput.aspx/GetVehicles',
                params: { type: VehicleType },
                method: 'GET',
                async: false,
                headers: { 'Content-Type': 'application/json' },
                success: function (conn, response, options, eOpts) {
                    var result = Ext.decode(conn.responseText).d;
                    if (VehicleType == 1) {
                        truckTrailerListData = result;
                    } else if (VehicleType == 2) {
                        busListData = result;
                    } else if (VehicleType == 3) {
                        usVehicleListData = result;
                    } else if (VehicleType == 4) {
                        trailerListData = result;
                    }
                    displayVehicleList(result, VehicleType);
                },
                failure: function (conn, response, options, eOpts) {
                    $("divImage").val = "";
                }
            });

        }

        function getInspectionItems(instype) { //55,56,57
            var listData = null;
            if (instype == 55) {
                if (truckTrailerInspectionData != null) {
                    listData = truckTrailerInspectionData;
                }
            } else if (instype == 56) {
                if (busInspectionData != null) {
                    listData = busInspectionData;
                }
            } else if (instype == 57) {
                if (usVehicleInspectionData != null) {
                    listData = usVehicleInspectionData;
                }
            } else {
                return;
            }

            if (listData != null) {
                displayInspectionItemList(listData);
                return;
            }


            Ext.Ajax.request({
                url: 'frmDVIRInput.aspx/GetInspectionItems',
                params: { groupId: instype },
                method: 'GET',
                async: false,
                headers: { 'Content-Type': 'application/json' },
                success: function (conn, response, options, eOpts) {
                    var result = Ext.decode(conn.responseText).d;

                    if (instype == 55) {
                        truckTrailerInspectionData = result;
                    } else if (instype == 56) {
                        busInspectionData = result;
                    } else if (instype == 57) {
                        usVehicleInspectionData = result;
                    } else {
                        return;
                    }

                    displayInspectionItemList(result);
                },
                failure: function (conn, response, options, eOpts) {
                    $("divImage").val = "";
                }
            });

        }


        function displayInspectionItemList(listdata) {

            var Inspection_CheckBoxArray = []; //temp stakeholder store
            var Inspection_Store = new Ext.data.ArrayStore({
                fields: ['Id', 'Text'],
                //data: [['1', 'item1'], ['2', 'item2'], ['3', 'item3']],
                data: listdata,
                listeners: {
                    load: function (t, records, options) {
                        for (var i = 0; i < records.length; i++) {
                            Inspection_CheckBoxArray.push({ 
                                inputValue: records[i].data.Id, boxLabel: "" + (i + 1) + ". " + records[i].data.Text, name: 'defects', cls:'chkgrp'
                            });
                            // alert(records[i].data.Text);
                        }
                    }
                }
            });

            $("#spanInspectionItems")[0].innerHTML = "";
            var checkboxes = new Ext.form.CheckboxGroup({
                id: 'inspections',
                name: 'insItems',
                allowBlank: true,
                columns: 4,
                items: Inspection_CheckBoxArray,
                renderTo: 'spanInspectionItems',
                vertical: true
            });
       
        }

        function getDrivers() {
            
            if (driverListData != null) {
                return;
            }
            
            Ext.Ajax.request({
                url: 'frmDVIRInput.aspx/GetDrivers',
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                async: false,
                success: function (conn, response, options, eOpts) {
                    var result = Ext.decode(conn.responseText).d;
                    driverListData = result;
                    displayDriverList(result);
                },
                failure: function (conn, response, options, eOpts) {
                    $("divImage").val = "";
                }
            });

        }

        function displayDriverList(lst) {
            $("#spanDriver").each(function (i, obj) {
                obj.innerHTML = "";
            });

            driverControl = Ext.create('Ext.form.field.ComboBox', {
                id: 'driverlist',
                fieldLabel: 'Driver',
                displayField: 'display',
                valueField: 'DriverId',
                value: '',
                hiddenName: 'driverId',
                width: 260,
                labelWidth: 40,
                queryMode: 'local',
                lazyRender: false,
                typeAhead: true,
                typeAheadDelay: 250,
                triggerAction: 'all',
                minChars: 3,
                name: 'driver',
                renderTo: 'spanDriver',
                multiSelect: false,
                allowBlank: false,
                filterOnLoad: true,         // prevent expand performance issue
                filters: [                  // prevent expand performance issue
                  function(item) {          // prevent expand performance issue
                      return item.index < 10; // prevent expand performance issue
                  }                         // prevent expand performance issue
                ],                           // prevent expand performance issue
                activeFilter: true, // workaround to allow me.clearFilter() inside Ext.form.field.ComboBox
                store: new Ext.data.SimpleStore({
                    fields: ['DriverId', 'EmployeeId', 'DriverName', {
                        name: 'display',
                        convert: function (v, rec) {
                            return rec.data.EmployeeId + ' - ' + rec.data.DriverName;
                        }
                    }],
                    noCache: false, // allow browser cache
                    data: lst
                }),
                enableKeyEvents: true,
                queryMode: 'local',
                listeners: {
                    afterrender: function (combo) {
                        combo.expand();
                        combo.collapse();
                        combo.reset();
                    },
                    keydown: function (combo, event) {
                        
                        if (event.getKey() == 18) {
                            var store = this.store;
                            combo.collapse();
                            //store.suspendEvents();
                            store.clearFilter();
//                            store.resumeEvents();
                            store.filter({
                                property: 'display',
                                anyMatch: true,
                                value: this.getValue(),
                                caseSensitive: false
                            });
                            combo.expand();
                        }
                    }
                }
            });

        }

        var driverListData = null;
        var truckTrailerListData = null;
        var busListData = null;
        var usVehicleListData = null;
        var trailerListData = null;
        var truckTrailerInspectionData
        var busInspectionData = null;
        var usVehicleInspectionData = null;

        var dateControl = null;
        var timeControl = null;
        var driverControl = null;

        var fileName = null;
        var folder = null;
        var formPanel = null;

        Ext.onReady(function () {
            fileName = getQueryStringValue("fileName");
            folder = getQueryStringValue("folder");
            
            formPanel = Ext.create('Ext.Panel', {
                type: 'hbox',
                width: '100%',
                layout: {
                    align: 'stretch',
                    pack: 'center',
                    type: 'vbox',
                    matchFieldWidth: true
                },
                ContentEl: 'insInput'
            });

            clearForm();

            window.initialForm = function (mfolder, mfileName) {
                folder = mfolder;
                fileName = mfileName;
                document.title = mfileName;
                clearForm();
            }

        });

        function changeInspectionType() {
            clearForm();
            dateControl.focus();
        }

        function clearForm() {
            var inspectionType = $('#inspectionType')[0].value;
            makeDateFields();
            getDrivers()
            showVehicleLists();
            $("#odometer").val("");
            $("#odometer").val("");
            $("input[name=satisfied]").val("1");

            $("#defectsCorrected").checked = false;
            $("#defectsSignedRepairer").checked = false;
            $("#defectsNoNeedCorrect").checked = false;
            $("#defectsSignedDriver").checked = false;

            $("#remark").val("");

            getInspectionItems($('#inspectionType').val);

            this.form1.reset();

            $('#inspectionType')[0].value = inspectionType;

            dateControl.focus();
        }

        function showVehicleLists() {
            var inspectionType = $("#inspectionType")[0].value;
            if (inspectionType == 55) {
                getVehicleLists(1);
                getVehicleLists(4);
            } else if (inspectionType == 56) {
                getVehicleLists(2);
                getVehicleLists(4);
            } else {
                getVehicleLists(3);
                getVehicleLists(4);
            }

            getInspectionItems(inspectionType);
        }

        function isEmpty(o){
            return (!o||o.length==0);
        }

        function is_int(value) {
            for (i = 0 ; i < value.length ; i++) {
                if ((value.charAt(i) < '0') || (value.charAt(i) > '9')) return false
            }
            return true;
        }

        function checkOverwrite(data) {
            var ret = false;
            Ext.Ajax.request({
                url: 'frmDVIRInput.aspx/existingInsepection',
                params: {
                    driverId: data.driverId,
                    date: Ext.encode(data.insDate),
                    dateFormat: Ext.encode('dd/MM/yyyy'),
                    time: Ext.encode(data.insTime)
                },
                method: 'GET',
                async: false,
                headers: { 'Content-Type': 'application/json' },
                success: function (conn, response, options, eOpts) {
                    var result = Ext.decode(conn.responseText).d;
                    ret = processPPC(result);
                },
                failure: function (conn, response, options, eOpts) {
                    var message = Ext.decode(conn.responseText).Message;
                    alert(message);
                    ret = false;
                }
            });
            return ret;
        }

        function processPPC(result) {
            if (!result || result == '') {
                return true;
            }
            result=result.toLowerCase();
            if (result == 'ocr') {
                return confirm("An inspection from paper log already exists, do you want to overwrite?");
            } else if (result == 'phone') {
                alert('An inspection from mobile already exists, you cannot overwrite');
                return false;
            } else {
                alert('An inspection from device already exists, you cannot overwrite');
                return false;
            }
        }

        function isValid(data) {
            var message ="";
            if (isEmpty(data.insDate)) {
                message += "Please enter valid Date.\r\n";
            }
            if (isEmpty(data.insTime)) {
                message += "Please enter valid Time.\r\n";
            }
            if (isEmpty(data.driverId)) {
                message += "Please enter valid Driver.\r\n";
            }
            if (isEmpty(data.vehicleId)) {
                message += "Please enter valid Vehicle.\r\n";
            }
            if (!is_int(data.odometer)) {
                message += "Please enter Odometer as integer.\r\n";
            }
            if (isEmpty(message)) {
                return true;
            } else {
                alert(message);
                return false;
            }
        }

        function SaveInspection() {
            var form = $("#form1")[0];
            var formData = $("#form1").serializeArray();
            var o = {};
            $.each(formData, function () {
                if (o[this.name] !== undefined) {
                    if (!o[this.name].push) {
                        o[this.name] = [o[this.name]];
                    }
                    o[this.name].push(this.value || '');
                } else {
                    o[this.name] = this.value || '';
                }
            });

            if (isValid(o)==true && checkOverwrite(o)==true) {

                var group = Ext.getCmp('inspections');
                var checkedArray = group.query('[checked="true"]');
                var insItems = [];
                for (var key in checkedArray) {
                    insItems.push(checkedArray[key].inputValue);
                }
                o.insItems = insItems;

                var params = Ext.Object.fromQueryString(window.location.search);
                o["folder"] = folder;
                o["fileName"] = fileName;

                Ext.Ajax.request({

                    url: 'frmDVIRInput.aspx/SaveInspection',
                    method: 'POST',
                    jsonData: { obj: Ext.encode(o) },
                    success: function (conn, response, options, eOpts) {
                        var result = Ext.decode(conn.responseText).d;

                        opener.window.removeFileFromList(encodeURI(fileName));
                    },
                    failure: function (conn, response, options, eOpts) {
                        var message = Ext.decode(conn.responseText).Message;
                        alert(message);
                    }

                });
            }
        }

    </script>
</body>
</html>
