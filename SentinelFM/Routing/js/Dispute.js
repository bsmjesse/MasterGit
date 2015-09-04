var g = jQuery.noConflict();

var correctionPoints = Array();
g(document).ready(function () {
   
    g('#showAll').click(function () {
        ShowDisputePoints("all");
    });
    g('#showDisputed').click(function () {
        ShowDisputePoints("Disputed");
    });
    g('#showDismissed').click(function () {
        ShowDisputePoints("Dismissed");
    });
    g('#showAccepted').click(function () {
        ShowDisputePoints("Accepted");
    });
    g('#showSegements').click(function () {
        g("#dialog-routeSegments").dialog("open");
        InitialRouteSegmentsList();
    });
    g("#dialog-disputes").dialog({
        autoOpen: false,
        height: 650,
        width: 1200,
        modal: true,
        buttons: {           
            Cancel: function () {
                g(this).dialog("close");
            }
        },
        close: function () {
            g(this).dialog("close");
        }
    });
    g('#dialog-routeSegments').dialog({
        autoOpen: false,
        height: 500,
        width: 1200,
        modal: true,
        buttons: {
            Cancel: function () {
                g(this).dialog("close");
            }
        },
        close: function () {
            g(this).dialog("close");
        }
    });

    g('#chkDropSegments').click(function () {
        if (g('#chkDropSegments').prop('checked')) {
            OSRM.G.SpeedDispute.addWmsLayer();
        } else {
            OSRM.G.SpeedDispute.removeWmsLayer();
        }        
    });

});

function FixPoint(lat, lon, mid) {
    CleanUpInfo();
    OSRM.G.SpeedDispute.CleanDispute();
    OSRM.G.SpeedDispute.ZoomInForFixing(lat, lon);    
    if (mid != undefined) {
        OSRM.G.SpeedDispute.OpenPopup(mid);
        if (g("#dialog-disputes").dialog("isOpen") === true) {
            g("#dialog-disputes").dialog("close");
        }        
    }
}

function DisplayCorrectedArea(correctid, pid) {        
    g.ajax({
        url: "Main.aspx/GetCorrectRoute",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: "POST",
        data: "{'routeId':'" + correctid + "', 'pid':'" + pid + "'}",
        success: function (data) {
            if (data.d != "Failed" && data.d != "" && data.d != null) {
                
                /****
                CleanUpInfo();               
                customizedUrl = jsonData.routelink;
                RenderWayPoints(jsonData.waypoints);
                RenderRoute();
                g('#routeName').val(jsonData.routename);
                g('#routeBuffer').val(jsonData.buffer);
                g('#txtName').val(jsonData.contactperson);
                g('#txtEmail').val(g.trim(jsonData.email));
                g('#txtPhone').val(jsonData.phone);
                g('#chkSpeed').prop('checked', true);
                g('#correctSpeed').val(jsonData.speed);
                g("label[for='correctSpeed']").show();
                g('#correctSpeed').show();                
                g('#txtDescription').val(jsonData.description);
                g('#txtWayPoints').val(jsonData.waypoints);
                g('#txtQueueString').val(jsonData.routelink);                
                g('#correctionid').val(correctid);
                if (g("#dialog-disputes").dialog("isOpen") === true) {
                    g("#dialog-disputes").dialog("close");
                }  
                ***/
                if (g("#dialog-routeSegments").dialog("isOpen")) {
                    g("#dialog-routeSegments").dialog("close");
                }
                RenderSegment(data, false);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function RenderSegment(data, urldecode) {
    var jsonData = null;
    if (urldecode == 'true') {
        data = decodeURIComponent(data);     
        jsonData = jQuery.parseJSON(data);
    } else {
        jsonData = jQuery.parseJSON(data.d);  
    }
    CleanUpInfo();
    customizedUrl = jsonData.routelink;
    RenderWayPoints(jsonData.waypoints);    
    RenderRoute();
    BuildAlreadyKnownCollection(jsonData.waypoints);
    g('#routeName').val(jsonData.routename);
    g('#routeBuffer').val(jsonData.buffer);
    g('#txtName').val(jsonData.contactperson);
    g('#txtEmail').val(g.trim(jsonData.email));
    g('#txtPhone').val(jsonData.phone);
    g('#chkSpeed').prop('checked', true);
    g('#correctSpeed').val(jsonData.speed);
    g("label[for='correctSpeed']").show();
    g('#correctSpeed').show();    
    if (jsonData.organizationid == "0") {
        g('#gblSegment').prop('checked', true);
    }
    g("label[for='gblSegment']").show();
    g('#gblSegment').show();

    
    g('#txtDescription').val(jsonData.description);
    g('#txtWayPoints').val(jsonData.waypoints);
    g('#txtQueueString').val(jsonData.routelink);
    g('#correctionid').val(jsonData.id);
    if (g("#dialog-disputes").dialog("isOpen") === true) {
        g("#dialog-disputes").dialog("close");
    }
}

function CleanUpInfo() {
    CleanUpAllBoxes();
    OSRM.G.SpeedDispute.CleanDispute();
    OSRM.G.route.reset();
    OSRM.G.markers.reset();
    pointsCount = 0;
    suggestedPoints = {};
    alreadyKnownPointsCollection = "";
    addressesCollection = "";
    g('#information-box-header').html('');
    g('#information-box').html('');
    OSRM.G.NavTeqResponse = '';
    CleanupForm();
}

function DeletePoint(pointId) {
    var r = confirm("Are you sure to delete this dispute?");
    if (r == false) {       
        return false;
    }
   
    g.ajax({
        url: "Main.aspx/DeleteDisputePoint",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: "POST",
        data: "{'pointId':'" + pointId + "', 'comment':'" + g('#txtComment').val() + "'}",
        success: function (data) {
            if (data.d == "Failed") {
                alert("You cannot dismiss the dispute, please contact with administrator.");
            } else {
                OSRM.G.SpeedDispute.removeMarker(pointId);
                InitialDisputePointsList();
                alert("You have successfully deleted the dispute.");
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function ChangeMarkers(affectedMarkers) {
    g.each(affectedMarkers, function (key, val) {
        try {
            OSRM.G.SpeedDispute.AddPointToClusterMarkers(val);
            g('#correctionid').val(val.correctionid);
        } catch (err) {
            console.log(err.message);
        }
    });
}

function InitialDisputePointsList(filter) {
    //g.fn.dataTableExt.oStdClasses.sStripeOdd = '';
    //g.fn.dataTableExt.oStdClasses.sStripeEven = '';
    var fileName = "SpeedDisputePoints";
    g('#tblDisputePoints').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "GetDisputePointsForGrid.aspx",        
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "js/media/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy",
                {
                    "sExtends": "csv",
                    "sTitle": "DisputedPoints",
                    "sFileName": fileName + ".csv",
                    "mColumns": [0, 1, 2, 3, 4, 5, 6, 7, 8]
                },
                {
                    "sExtends": "xls",
                    "sTitle": "DisputedPoints",
                    "sFileName": fileName + ".xls",
                    "mColumns": [0, 1, 2, 3, 4, 5, 6, 7, 8]
                },
                {
                    "sExtends": "pdf",
                    "sTitle": "DisputedPoints",
                    "sFileName": fileName + ".pdf",
                    "mColumns": [0, 1, 2, 3, 4, 5, 6, 7, 8]
                }
            ]
        },
        "bAutoWidth": false,
        "aaSorting": [[0, "desc"]],
        "aoColumns": [
            { "sType": "text" },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": true },
            { "bSortable": true },
            { "sType": "text" },
            { "bSortable": false }

        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "filter", "value": filter });           
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);               
            });
        }
    });
}

function InitialRouteSegmentsList() {
    
    var fileName = "RouteSegmentsList";
    g('#tblRouteSegments').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "GetSegments.aspx",
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "js/media/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy",
                {
                    "sExtends": "csv",
                    "sTitle": "DisputedPoints",
                    "sFileName": fileName + ".csv",
                    "mColumns": [0, 1, 2, 3, 4, 5, 6, 7, 8]
                },
                {
                    "sExtends": "xls",
                    "sTitle": "DisputedPoints",
                    "sFileName": fileName + ".xls",
                    "mColumns": [0, 1, 2, 3, 4, 5, 6, 7, 8]
                },
                {
                    "sExtends": "pdf",
                    "sTitle": "DisputedPoints",
                    "sFileName": fileName + ".pdf",
                    "mColumns": [0, 1, 2, 3, 4, 5, 6, 7, 8]
                }
            ]
        },
        "bAutoWidth": false,
        "aaSorting": [[0, "desc"]],
        "aoColumns": [
            { "sType": "text" },
            { "bSortable": false },
            { "bSortable": true },
            { "bSortable": false },
            { "bSortable": true },
            { "bSortable": false },
            { "bSortable": true },
            { "bSortable": false }           
        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {           
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
            });
        }
    });
}

function ShowDisputePoints(filter) {
    OSRM.G.SpeedDispute.DisplayRouteMarkers();
    g.ajax({
        url: "Main.aspx/GetDisputePoints",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: "POST",
        data: "{'organizationId':'0', 'filter':'" + filter + "'}",
        success: function (data) {
            if (data.d != "Failed") {
                OSRM.G.SpeedDispute.removeLayer();
                var jsonData = jQuery.parseJSON(data.d);
                g.each(jsonData, function (key, val) {
                    correctionPoints.push(val);
                    OSRM.G.SpeedDispute.AddPointToClusterMarkers(val);
                });
                //OSRM.G.SpeedDispute.DisplayClusterMarkers(correctionPoints);
                OSRM.G.SpeedDispute.RenderCluster();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
    g("#dialog-disputes").dialog("open");
    InitialDisputePointsList(filter);
    g(document).on('keyup', 'textarea[id^=txtComment]', function (event) {
        g('#disputeComment').val(g('#txtComment').val());
    });
}