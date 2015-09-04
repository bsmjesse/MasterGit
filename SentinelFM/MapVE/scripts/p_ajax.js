

function GetLandmarksFromService(uid, sid, id, ulat, ulon, blat, blon) {
    //BSM.VE.Ajax.AjaxService.GetLandmarksWithBoundaries(uid, sid, id, ulat, ulon, blat, blon, LandmarksSucceededCallback);
    var params = { "act": "landmarks", "ulat": ulat, "ulon": ulon, "blat": blat, "blon": blon };
    new Ajax.Request('mapService.aspx', {
        evalJS: false,
        parameters: params,
        onComplete: function(transport) {
            LandmarksSucceededCallback(transport);
        }
    });

}

function GetGeoZonesFromService(uid, sid, id, ulat, ulon, blat, blon) {
    //BSM.VE.Ajax.AjaxService.GetGeozonesWithBoundaries(uid, sid, id, ulat, ulon, blat, blon, GeoZonesSucceededCallback);
    var params = { "act": "geozones", "ulat": ulat, "ulon": ulon, "blat": blat, "blon": blon };
    new Ajax.Request('mapService.aspx', {
        evalJS: false,
        parameters: params,
        onComplete: function(transport) {
            GeoZonesSucceededCallback(transport);
        }
    });
}

function GetPoisFromService(uid, sid, id, ulat, ulon, blat, blon) {
    //BSM.VE.Ajax.AjaxService.GetPoisWithBoundaries(uid, sid, id, ulat, ulon, blat, blon, PoisSucceededCallback);
    var params = { "act": "pois", "ulat": ulat, "ulon": ulon, "blat": blat, "blon": blon };
    new Ajax.Request('mapService.aspx', {
        evalJS: false,
        parameters: params,
        onComplete: function(transport) {
            PoisSucceededCallback(transport);
        }
    });
}

function LandmarksSucceededCallback(transport) {
    mapscreen.setLandmarkData(transport.responseText, true);
}

function GeoZonesSucceededCallback(transport) {
    mapscreen.setGeoZoneData(transport.responseText, true);
}

function PoisSucceededCallback(transport) {
    mapscreen.setPoiData(transport.responseText);
}

function FailedCallback(error) {
    alert(error);
}

