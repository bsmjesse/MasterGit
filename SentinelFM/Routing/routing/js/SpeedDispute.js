var g = jQuery.noConflict();
function SpeedDispute(myMap) {
    this.map = myMap;
    this.marker = null;
    this.markersCollection = new Array();
    this.pointscollection = "";
    this.clusterMarkers = L.markerClusterGroup();
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
        if (a.correctionid != null && a.correctionid != "" && a.correctionid != "0") {
            marker.setIcon(L.AwesomeMarkers.icon({ icon: 'cog', prefix: 'fa', markerColor: 'green' }));
        } else {
            marker.setIcon(L.AwesomeMarkers.icon({ icon: 'cog', prefix: 'fa', markerColor: 'red' }));
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
              '<div>Speed Dispute Ticket</div>' +
              '<form id="inputform" name="inputform" class="well">' +
             
              '<label><strong>Point: ' + jsonData.latitude  + ',' + jsonData.longitude + '</strong> </label>' +
              '<label><strong><font color="green">Suggested speed: ' + jsonData.speed + (jsonData.metric == "1" ? 'kms/hr' : 'mph') + '</font></strong> </label>' +
            '<label><strong><font color="red">' + errorSpeed + '</font></strong> </label>' +
            '<label><strong>' + driverName + '</strong> </label>' +
             '<label><strong>Notes: ' + notes + '</strong> </label>' +
            '<label><strong>' + (parseInt(jsonData.correctionid) > 0 ? '<a href=\"javascript:DisplayCorrectedArea(\'' + jsonData.correctionid + '\');\">Corrected</a>&nbsp;&nbsp;&nbsp;<a href=\"#\" onclick=\"DeletePoint(\'' + jsonData.id + '\');\">Dismiss</a>' : '<a href=\"javascript:FixPoint(' + jsonData.latitude + ', ' + jsonData.longitude + ');\">Correct</a>&nbsp;&nbsp;&nbsp;<a href=\"#\" onclick=\"DeletePoint(\'' + jsonData.id + '\');\">Dismiss</a>') + '</strong> </label>' +
              '</div>' +
              '</form>' +
               '</div>';
        return body;
    };

    this.InitilaizeClick = function() {
        this.map.on("click", this.MapClick);
    };

    this.MapClick = function (e) {
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

    this.removeMarker = function(pid) {
        this.map.removeLayer(this.markersCollection[pid]);
        this.clusterMarkers.removeLayer(this.markersCollection[pid]);
        delete this.markersCollection[pid];
    };

    this.changeMarkerColor = function (pid) {
        console.log(this.markersCollection.length);
        this.markersCollection[pid].setIcon(L.AwesomeMarkers.icon({ icon: 'info', prefix: 'fa', markerColor: 'green' }));
    };    
    
};

