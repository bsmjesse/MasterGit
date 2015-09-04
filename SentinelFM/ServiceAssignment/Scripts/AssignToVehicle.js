var g = jQuery.noConflict();
var dTable = null;
var sourceType = "Vehicle";
g(document).ready(function () {

    InitialAssignmentsList(false);
    
    g('#txtInput').val('');
    RenderService(objectName, 'inputsList');
    g('#txtRules').val('');
    RenderConfiguredService(serviceName, 'servicesList');
    
    g('#closeWindow').click(function () {
        closeWindow();
    });

    g("#dialog-assignment").dialog({
        autoOpen: false,
        height: 480,
        width: 520,
        modal: true,
        buttons: {
            "Assign": function () {
                if (AssignToService()) {
                    g(this).dialog("close");
                }                
            },
            Cancel: function () {
                g(this).dialog("close");
            }
        },
        close: function () {
            g(this).dialog("close");
        }
    });
    
    g("#searchField").keyup(function () {
        /* Filter on the column (the index) of this element */
        dTable.fnFilter(this.value, g("#selectColumn").val());
    });
    
    g('#btnAssignment').click(function () {
        g('#txtInput').val(objectName);
        g('#txtRules').val(serviceName);
        g("#dialog-assignment").dialog("open");
    });
        

    g("#allRoutes tfoot th").each(function (i) {
        if (i == 1 && objectName != '' && objectName != null) {
            this.innerHTML = fnCreateSelect(objectName);
            dTable.fnFilter(objectName, 0);
        } 
        else if (i == 0 && serviceName != '' && serviceName != null) {
            this.innerHTML = fnCreateSelect(serviceName);
            dTable.fnFilter(serviceName, 1);
        } else {
            this.innerHTML = fnCreateSelect('');
        }
        
        g('input', this).keyup(function () {
            var columnCount = i;
            if (i == 0) {
                columnCount = 1;
            } else if(i == 1) {
                columnCount = 0;
            }
            else if (i == 2 && hgiUser == "0") {
                columnCount = 4;
            } else if (i == 5 && hgiUser == "1") {
                columnCount = 6;
            }
            dTable.fnFilter(g(this).val(), columnCount);
        });
    });

    g('#fleetsList').change(function () {
        g("#vehiclesList").empty();
        var selectedFleetId = g('#fleetsList').val();
        g.ajax({
            url: "AssignmentForm.aspx/GetFleetVehicles",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: "{ 'fleetId': '" + selectedFleetId + "'}",
            success: function (data) {                
                if (data.d != "Failed") {
                    var jsonData = jQuery.parseJSON(data.d);
                    g.each(jsonData, function (key, val) {
                        var inHTML = '<option value="' + key + '">' + val + '</option>';
                        g('#vehiclesList').append(inHTML);
                    });
                }                
            }
        });
    });
    
    g('#txtRules').keyup(function () {
        var inputVal = g('#txtRules').val();
        RenderConfiguredService(inputVal, 'servicesList');
    });

    g('#txtInput').keyup(function() {
        var inputVal = g('#txtInput').val();
        RenderService(inputVal, 'inputsList');
    });

    g('#btnUnassigned').click(function() {
        InitialAssignmentsList(true);
    });

    g('#searchService').keyup(function () {
        var inputVal = g('#searchService').val();
        InjectConfiguredServices(inputVal);
    });

    g('#btnFleet').click(function() {
        sourceType = "fleet";       
        InitialAssignmentsList();
        g('#objectName').html("Fleet Name");
        g('#txtInput').val('');
        RenderService('', 'inputsList');
        g('#txtRules').val('');
        g('.objectholder').html(sourceType);
        RenderConfiguredService('', 'servicesList');
    });
    
    g('#btnVehicle').click(function () {
        sourceType = "vehicle";        
        InitialAssignmentsList();
        g('#objectName').html("Vehicle Name");
        g('#txtInput').val('');
        RenderService('', 'inputsList');
        g('#txtRules').val('');
        g('.objectholder').html(sourceType);
        RenderConfiguredService('', 'servicesList');
    });
    
    g('#btnLandmark').click(function () {
        sourceType = "landmark";
        var columName = "Landmark Name";        
        if (serviceId == 4) {
            sourceType = "route";
            columName = "Route Name";            
        }
        InitialAssignmentsList();
        g('#objectName').html(columName);
        g('#txtInput').val('');
        RenderService('', 'inputsList');
        g('#txtRules').val('');
        g('.objectholder').html(sourceType);
        RenderConfiguredService('', 'servicesList');
    });
    g("#dialog-report").dialog({
        autoOpen: false,
        height: 680,
        width: 1200,
        modal: true,
        appendTo: "form",
        buttons: {
            Cancel: function () {
                g(this).dialog("close");
            }
        }
    });

    g('#serviceIdUl li').click(function() {
        g('#btnServiceContainer').text(g(this).text());
        //g('#btnServiceContainer').parent().toggleClass("btn btn-success dropdown-toggle");
    });
    
    g(document).on('click', '#serviceUl li.serviceItem', function () {
        g('#btnConfiguredServiceContainer').text(g(this).text());        
    });
});

function RenderService(inputVal, renderbox) {
    g.ajax({
        url: "AssignmentForm.aspx/GetServices",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: "{ 'input': '" + inputVal + "','lookfor':'" + sourceType + "'}",
        success: function (data) {
            if (data.d != "Failed") {
                g("#" + renderbox).empty();
                var jsonData = jQuery.parseJSON(data.d);
                g.each(jsonData, function (key, val) {
                    var inHTML = '<option value="' + key + '">' + val + '</option>';
                    g('#' + renderbox).append(inHTML);
                });
            }
        }
    });
}

function RenderConfiguredService(inputVal, renderbox) {    
    g.ajax({
        url: "AssignmentForm.aspx/GetConfiguredServices",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: "{ 'input': '" + inputVal + "','lookfor':'" + lookfor + "', 'serviceIdStr':'0'}",
        success: function (data) {
            if (data.d != "Failed") {
                g("#" + renderbox).empty();
                var jsonData = jQuery.parseJSON(data.d);
                g.each(jsonData, function (key, val) {
                    var inHTML = '<option value="' + key + '">' + val + '</option>';
                    g('#' + renderbox).append(inHTML);
                });
            }
        }
    });
}

function closeWindow() {
    window.open('', '_parent', '');
    window.close();
}

function AssignToService() {
    if (g('#servicesList').val() == null || g('#servicesList').val() == '') {
        alert("Please choose one service at least");
        return false;
    }
    if (g('#inputsList').val() == null || g('#inputsList').val() == '') {
        alert("Please choose one object at least");
        return false;
    }
    var selectedServices = g('#servicesList').val().join(',');
    var selectedObjects = g('#inputsList').val().join(',');
    g.ajax({
        url: "AssignmentForm.aspx/SaveServiceAssignment",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        dataType: "json",
        data: "{ 'servicesStr': '" + selectedServices + "', 'vehiclesStr': '" + selectedObjects + "', 'objects':'" + sourceType + "' }",
        error: function (xhr, status, error) {
            alert(xhr.responseText);          
        },
        success: function(data) {
            if (data.d == "Success") {
                InitialAssignmentsList();
                alert("Your service assignments have been saved");
            } else {
                alert("Cannot save your service assignments, please contact with customer service.");
            }             
        }
    });
    return true;
}

function DeleteAssignment(serviceId, vehicleId) {
    var yes = confirm("Do you want to delete the route Assignment?");
    if (yes) {
        g.ajax({
            url: "AssignmentForm.aspx/DeleteAssignment",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            dataType: "json",
            data: "{ 'serviceId': '" + serviceId + "', 'vehicleId': '" + vehicleId + "' }",
            error: function (xhr, status, error) {
                alert(xhr.responseText);
            },
            success: function (data) {
                if (data.d == "Success") {
                    InitialAssignmentsList();                    
                } else {
                    alert("Cannot delete your service assignment, please contact with customer service.");
                }
            }
        });
    }
   
}

function InitialAssignmentsList(unassigned) {
   dTable = g('#allRoutes').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,        
        "sAjaxSource": "AssignmentManager.aspx",        
        "oLanguage": {
            "sSearch": "Search all columns:"
        },
        "bAutoWidth": false,
        "aoColumns": [
            { "sType": "text" },
            { "sType": "text" },
            { "sType": "text", "bVisible":  true },
            { "sType": "text", "bVisible": true },
            { "sType": "text" },
            { "sType": "text" },
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {            
            aoData.push({ "name": "searchCriteria", "value": sourceType });
            aoData.push({ "name": "sCriterialDedicate", "value": "1" });            
            aoData.push({ "name": "serviceId", "value": serviceId });
            if (unassigned) {
                aoData.push({ "name": "unassigned", "value": "1" });
            }
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
                if (json.iTotalRecords > 7) {
                    g('#dialog-assignments').css("height", 480);
                } else {
                    g('#dialog-assignments').css("height", 350);
                }
            });
        },
        "aLengthMenu": [10, 25, 50, 100, 500, 1000, 2000],
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "Scripts/media/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy",
                {
                    "sExtends": "csv",
                    "sTitle": "Assignment",
                    "sFileName": "serviceassignment.csv",
                    "mColumns": [0, 1, 2, 3, 4, 5]
                },
                {
                    "sExtends": "xls",
                    "sTitle": "Assignment",
                    "sFileName": "serviceassignment.xls",
                    "mColumns": [0, 1, 2, 3, 4, 5]
                },
                {
                    "sExtends": "pdf",
                    "sTitle": "Assignment",
                    "sFileName": "serviceassignment.pdf",
                    "mColumns": [0, 1, 2, 3, 4, 5]
                }
            ]
        }
   });
    /****
   g("#allRoutes tfoot th").each(function (i) {
       var columnCount = i;
       if (i == 3) {
           columnCount = 4;
       } else if (i == 4) {
           columnCount = 6;
       }
       if (g('input', this).val() != undefined) {
           dTable.fnFilter(g('input', this).val(), columnCount);
       }       
    });
    ***/
    //g("#dialog-assignments").css('height', 350);
}


function fnCreateSelect(iniData) {
    
    return '<input type="text" value="' + iniData + '" />';
}

function AddService(configName) {
    RenderConfiguredService(configName, "servicesList");
    g("#dialog-assignment").dialog("open");
}

function InitializeReport(searchFor, searchField, serviceConfigId, serviceConfigName) {
    var fileName = serviceConfigName + "-" + searchFor + "-" + searchField;
    g('#dialog-report').dialog('open');
    g('#tblReport').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "ViolationsReport.aspx",
        "bAutoWidth": false,
        "aoColumns": [
            { "sType": "text" },
            { "sType": "text" },
            { "bSortable": false },
            { "sType": "text" },
            { "bSortable": false, "bVisible":  false },
            { "bSortable": false, "bVisible": false },
            { "sType": "text", "bVisible": serviceId == 2 },
            { "sType": "text" },
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "Scripts/media/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy",
                {
                    "sExtends": "csv",
                    "sTitle": "ExceptionsReport",
                    "sFileName": "ExceptionsReport-" + fileName + ".csv"
                },
                {
                    "sExtends": "xls",
                    "sTitle": "ExceptionsReport",
                    "sFileName": "ExceptionsReport-" + fileName + ".xls"
                },
                {
                    "sExtends": "pdf",
                    "sTitle": "ExceptionsReport",
                    "sFileName": "ExceptionsReport-" + fileName + ".pdf"
                },
                "print"
            ]
        },
        "aLengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],
        "fnServerData": function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "searchFor", "value": searchFor });
            aoData.push({ "name": "searchField", "value": searchField });
            aoData.push({ "name": "serviceConfigId", "value": serviceConfigId });
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
                if (json.iTotalRecords > 7) {
                    g('#dialog-assignments').css("height", 480);
                } else {
                    g('#dialog-assignments').css("height", 350);
                }
            });
        }
    });
}

function SelectService(selectedServiceId) {    
    serviceId = selectedServiceId;
    g('#ServiceId').val(serviceId);
    InjectConfiguredServices();
    InitialAssignmentsList("");
}

function InjectConfiguredServices(inputVal) {    
    g.ajax({
        url: "AssignmentForm.aspx/GetConfiguredServices",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: "{ 'input': '" + inputVal + "','lookfor':'0', 'serviceIdStr':'" + serviceId + "'}",
        success: function (data) {
            g('.serviceItem').remove();
            if (data.d != "Failed") {                
                var jsonData = jQuery.parseJSON(data.d);
                g.each(jsonData, function (key, val) {
                    var inHTML = '<li class="serviceItem"><a tabindex="-1" href="#">' + val + '</a></li>';
                    g('#serviceUl').append(inHTML);
                });
            }
        }
    });
}

function CreateAssignment() {
    DeleteAllValue();
    g('#btnAddRule').click();
    g('#deleteService').val('');
    var myButtons = {
        "Create new rule": function () {
            CreateExpression();
            if (ValidateForm()) {
                g('#action').val("Save");
                g('#ServiceConfigForm').submit();
            }

        },
        Cancel: function () {
            DeleteAllValue();
            g(this).dialog("close");
        }
    };
    g("#dialog-modal").dialog('option', 'buttons', myButtons).dialog("open");
}

function exportData(exportFormat) {
    alert(exportFormat);
}