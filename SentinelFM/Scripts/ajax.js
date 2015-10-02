

function GetLandmarksFromService(id, ulat, ulon, blat, blon)
{
    BSM.VE.Ajax.AjaxService.GetLandmarksWithBoundaries(id, ulat, ulon, blat, blon, LandmarksSucceededCallback);
}

function GetGeoZonesFromService(id, ulat, ulon, blat, blon)
{
    BSM.VE.Ajax.AjaxService.GetGeozonesWithBoundaries(id, ulat, ulon, blat, blon, GeoZonesSucceededCallback);
}

function GetPoisFromService(id, ulat, ulon, blat, blon)
{
    BSM.VE.Ajax.AjaxService.GetPoisWithBoundaries(id, ulat, ulon, blat, blon, PoisSucceededCallback);
}

function LandmarksSucceededCallback(result, eventArgs)
{
    parent.window.frames["framefooter"].cancelWait();
    mapscreen.setLandmarkData(result, true);
}

function GeoZonesSucceededCallback(result, eventArgs)
{
    parent.window.frames["framefooter"].cancelWait();
    mapscreen.setGeoZoneData(result, true);
}

function PoisSucceededCallback(result, eventArgs)
{
    parent.window.frames["framefooter"].cancelWait();
    mapscreen.setPoiData(result);
}

function FailedCallback(error)
{
    alert(error);
}

if (typeof(Sys) !== "undefined") Sys.Application.notifyScriptLoaded();
