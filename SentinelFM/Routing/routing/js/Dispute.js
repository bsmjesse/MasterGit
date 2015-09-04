var g = jQuery.noConflict();

var correctionPoints = Array();
g(document).ready(function () {
   
    g('#showDisputes').click(function () {
        OSRM.G.SpeedDispute.DisplayRouteMarkers();
        g.ajax({
            url: "Main.aspx/GetDisputePoints",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            type: "POST",
            data: "{'organizationId':'0'}",
            success: function (data) {
                if (data.d != "Failed") {
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

    });

});

function FixPoint(lat, lon) {
    CleanUpInfo();
    OSRM.G.SpeedDispute.CleanDispute();
    OSRM.G.SpeedDispute.ZoomInForFixing(lat, lon);
}

function DisplayCorrectedArea(correctid) {        
    g.ajax({
        url: "Main.aspx/GetCorrectRoute",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        type: "POST",
        data: "{'routeId':'" + correctid + "'}",
        success: function (data) {
            if (data.d != "Failed") {
                var jsonData = jQuery.parseJSON(data.d);
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
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function CleanUpInfo() {
    CleanUpAllBoxes();
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
        data: "{'pointId':'" + pointId + "'}",
        success: function (data) {
            if (data.d == "Failed") {
                alert("You cannot dismiss the dispute, please contact with administrator.");
            } else {
                OSRM.G.SpeedDispute.removeMarker(pointId);
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
        } catch (err) {
            console.log(err.message);
        }
    });
}