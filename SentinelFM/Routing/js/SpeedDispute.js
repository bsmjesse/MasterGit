var g = jQuery.noConflict();
function SpeedDispute(myMap) {
    this.map = myMap;
    this.marker = null;
    this.markersCollection = new Array();
    this.pointscollection = "";
    this.clusterMarkers = L.markerClusterGroup();
    this.mywms = L.tileLayer.wms('http://geomap1.sentinelfm.com:9090/geoserver/wms', {
        layers: 'topp:bsm-dispute',
        format: 'image/png',
        transparent: true,
        viewparams: 'organizationid:' + OSRM.G.OrganizationId
        },
        { 'opacity': 1, 'isBaseLayer': false, 'visibility': false });
    this.__construct = function() {
        
    }();
    this.DisplayRouteMarkers = function (jsonData) {        
        this.map.setView([43.67746, -79.5850766666667], 4);
       // this.marker = L.marker([jsonData.latitude, jsonData.longitude]).addTo(this.map);
       // this.marker.bindPopup(this.GetForm(jsonData)).openPopup();
    };
    
    this.AddPointToClusterMarkers = function (a) {
        if (this.markersCollection[a.id] != undefined) {
            this.removeMarker(a.id);
        }
        var title = this.GetForm(a);
        var marker = L.marker(new L.LatLng(a.latitude, a.longitude), { title: title });
        if (parseInt(a.deleted) == 1) {
            marker.setIcon(L.AwesomeMarkers.icon({ icon: 'cog', prefix: 'fa', markerColor: 'red' }));
        }
        else if (parseInt(a.deleted) < 1 && parseInt(a.correctionid) == 0) {
            marker.setIcon(L.AwesomeMarkers.icon({ icon: 'cog', prefix: 'fa', markerColor: 'orange' }));
        } else if (a.correctionid != null && a.correctionid != "" && a.correctionid != "0") {
            marker.setIcon(L.AwesomeMarkers.icon({ icon: 'cog', prefix: 'fa', markerColor: 'green' }));
        }

        marker.bindPopup(title);
        this.clusterMarkers.addLayer(marker);
        this.markersCollection[a.id] = marker;      
    };


    this.RenderCluster = function() {
        this.map.addLayer(this.clusterMarkers);
    };

    this.ZoomInForFixing = function(lat, lon) {
        this.map.setView([lat, lon], 20);
    };

    this.OpenPopup = function (mid) {
        var m = this.markersCollection[mid];        
        g('#disputeId').val(mid);
        g('#disputeComment').val('');
        g('#txtComment').val('');
        this.clusterMarkers.zoomToShowLayer(m, function () {            
            m.openPopup();
        });        
    };

    this.GetForm = function (jsonData) {        
        var errorSpeed = "", driverName = "", notes = "";        
        if (jsonData.notes != "" && jsonData.notes != null) {
            var strs = jsonData.notes.split('||');
            if(strs[0] != undefined)
                errorSpeed = strs[0];
            
            if(strs[1] != undefined)
                driverName = strs[1];
            
            if(strs[2] != undefined)
                notes = strs[2];
        }
        var body = '<div class="boostrap">' +
            '<div>Speed Dispute Ticket: ' + jsonData.notificationid + '</div>' +
            '<form id="inputform" name="inputform" class="well">' +
            '<label><strong>Point: ' + jsonData.latitude + ',' + jsonData.longitude + '</strong> </label>' +
            '<label><strong><font color="green">Claimed speed: ' + jsonData.speed + '</font></strong> </label>' +
            '<label><strong><font color="red">Post speed: ' + errorSpeed + '</font></strong> </label>' +
            '<label><strong>Driver name: ' + driverName + '</strong> </label>' +
            '<label><strong>Notes: ' + notes + '</strong> </label>' +
            '<label><strong>Dispute Comment:</strong></label> <textarea id=\"txtComment\" name=\"txtComment\">' + jsonData.comments + '</textarea> ';
        if (parseInt(jsonData.deleted) < 1) {
            body += '<label><strong>' + (parseInt(jsonData.correctionid) > 0 ? '<a href=\"javascript:DisplayCorrectedArea(\'' + jsonData.correctionid + '\');\">Display</a>&nbsp;&nbsp;&nbsp;<a href=\"#\" onclick=\"DeletePoint(\'' + jsonData.id + '\');\">Dismiss</a>' : '<a href=\"javascript:FixPoint(' + jsonData.latitude + ', ' + jsonData.longitude + ', ' + jsonData.id + ');\">Display</a>&nbsp;&nbsp;&nbsp;<a href=\"#\" onclick=\"DeletePoint(\'' + jsonData.id + '\');\">Dismiss</a>') + '</strong> </label>';
        } else {
            body += '<label><strong>' + (parseInt(jsonData.correctionid) > 0 ? '<a href=\"javascript:DisplayCorrectedArea(\'' + jsonData.correctionid + '\');\">Display</a>' : '<a href=\"javascript:FixPoint(' + jsonData.latitude + ', ' + jsonData.longitude + ', ' + jsonData.id + ');\">Display</a>') + '</strong> </label>';
        }            
              body += '</div>' +
              '</form>' +
               '</div>';
        return body;
    };

    this.InitilaizeClick = function() {
        this.map.on("click", this.MapClick);
    };

    this.MapClick = function (e) {
        OSRM.G.IsDblClicked = true;
        if (this.pointscollection == "") {
            this.pointscollection = alreadyKnownPointsCollection;
        } else {
            if (alreadyKnownPointsCollection != "") {
                this.pointscollection += ";" + alreadyKnownPointsCollection;
            }
            
        }        
        alreadyKnownPointsCollection = "";
        if (this.pointscollection == "") {
            this.pointscollection = e.latlng.lat + "," + e.latlng.lng;
        } else {
            this.pointscollection += ';' + e.latlng.lat + "," + e.latlng.lng;
        }        
        ReRenderWayPoints(this.pointscollection, true);
        RenderRoute();
        
    };

    this.CleanDispute = function() {
        this.pointscollection = "";
    };

    this.CleanDisputeByPoint = function (deletePoint) {
        this.pointscollection = this.pointscollection.replace(deletePoint, "");
        if (this.pointscollection.indexOf(";;") > -1) {
            this.pointscollection = this.pointscollection.replace(";;", ";");
        }
    };

    this.removeMarker = function(pid) {
        this.map.removeLayer(this.markersCollection[pid]);
        this.clusterMarkers.removeLayer(this.markersCollection[pid]);
        delete this.markersCollection[pid];
    };

    this.changeMarkerColor = function (pid) {
        console.log(this.markersCollection.length);
        this.markersCollection[pid].setIcon(L.AwesomeMarkers.icon({ icon: 'info', prefix: 'fa', markerColor: 'green' }));
    };

    this.resetPoints = function () {
        var myPoints = OSRM.Routing.getSortedPoints();
        this.CleanDispute();
        addressesCollection = "";
        suggestedPoints = {};
        if (myPoints != null) {
            for (var i = 0; i < myPoints.length; i++) {
                if (this.pointscollection == '') {
                    this.pointscollection = myPoints[i].getLat() + ',' + myPoints[i].getLng();
                } else {
                    this.pointscollection += ';' + myPoints[i].getLat() + ',' + myPoints[i].getLng();
                }

            }
        }
    };

    this.drawBuffer = function(jsonstr) {
        var route = [];
        g.each(jsonstr, function(key, val) {
            g.each(val, function(k1, v1) {
                route.push(new L.LatLng(v1[1], v1[0]));
            });
        });
        var polygon = new L.Polygon(route);
        this.map.addLayer(polygon);
    };

    this.clearLayers = function() {

    };

    this.removeLayer = function() {
        this.clusterMarkers.clearLayers();
    };

    this.addWmsLayer = function() {        
        this.mywms.addTo(this.map).bringToFront();
    };
    this.removeWmsLayer = function() {
        this.map.removeLayer(this.mywms);
    }

};;