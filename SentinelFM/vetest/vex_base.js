/**
 * @author mkroeze
 */
var params = new Array(3, "r", true);
var map = null;
var errorVisibility = 3;
var maptype = null;
var EarthRadiusInMiles = 3956.0;
var EarthRadiusInKilometers = 6367.0;
var EarthRadius = EarthRadiusInKilometers;


function GetMap(type){
    try {
        if (type != null) 
            maptype = type;
        var cpin = new VELatLong(45.95115, -97.20703);
        var distanceUnit = VEDistanceUnit.Kilometers;
        if (typeof(unitOfMes) != "undefined" && unitOfMes == "Mi") {
            distanceUnit = VEDistanceUnit.Miles;
            EarthRadius = EarthRadiusInMiles;
        }
        LoadMap(cpin, distanceUnit);
    } 
    catch (err) {
        SetError("GetMap", 0, err);
    }
}

function LoadMap(centerpoint, distanceUnit){
    try {
        map = new VEMap('vex');
        map.EnableShapeDisplayThreshold(false);
        map.LoadMap(centerpoint, params[0], params[1], false, VEMapMode.Mode2D, true);
        map.SetScaleBarDistanceUnit(distanceUnit);
        map.AttachEvent("onchangeview", MapStyleChange);
        //        map.AttachEvent("onclick",MapClicked);
        map.AttachEvent("oninitmode", MapInit);
        //        map.AttachEvent("onmouseup",MapMouseUp);
        //        map.AttachEvent("onmousedown",MapMouseDown);
        map.AttachEvent("onstartpan",MapStartPan);
        map.AttachEvent("onendpan",MapEndPan);
        map.AttachEvent("onstartzoom",MapStartZoom);
        map.AttachEvent("onendzoom",MapEndZoom);
        //        map.AttachEvent("ontokenerror",MapError);
        //        map.AttachEvent("ontokenexpired",MapError);
        map.AttachEvent("onerror", MapError);
    } 
    catch (err) {
        SetError("LoadMap", 0, err);
    }
}

function SetError(method, level, err){
    if (level <= errorVisibility) 
        alert(method + ":" + err.description);
}

function MapError(mapevent){
    SetError("MapError", 0, mapevent.error)
}

function MapStyleChange(mapevent){
    //alert(mapevent.eventName);
}

function MapInit(mapevent){
    //alert(mapevent.eventName);
    
}



function MapStartZoom(mapevent){
	StartPerformanceCounter();
}


function MapEndZoom(mapevent){
	EndPerformanceCounter();
}

function MapStartPan(mapevent){
	StartPerformanceCounter();
}


function MapEndPan(mapevent){
	EndPerformanceCounter();
}

