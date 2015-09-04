/**
 * @author mkroeze
 */
var clusterCount = 10;
var individualSeperation = 0.0002;
var clusterSeperation = 0.01;
var shapeCount = 20;
var startTime = null;
var endTime = null;
var timeControl = null;

function DrawDefaultLayerShapes(){
    try {
        map.DeleteAllShapes();
        StartPerformanceCounter();
        var point = new VELatLong(45.95115, -97.20703);
        var pins = new Array();
        var indies = 0;
        for (var i = 0; i < shapeCount; i++) {
            /*
             var x = point.Latitude *= individualSeperation;
             var y = point.Longitude *= individualSeperation;
             indies++;
             if (indies >= clusterCount) {
             x *= clusterSeperation;
             y *= clusterSeperation;
             indies = 0;
             }
             point = new VELatLong(x, y);
             */
            var pin = new VEShape(VEShapeType.Pushpin, point);
            pins.push(pin);
        }
        map.AddShape(pins);
    } 
    catch (err) {
        SetError("DrawDefaultLayerShapes", 0, err)
    }
    finally {
        EndPerformanceCounter();
    
    }
}

function AddUserBox(){
    try {
        var el = document.createElement("div");
        el.style.top = "200px";
        el.style.width = "200px";
        //el.style.backgroundColor="white";
        //el.style.height="200px";
        el.appendChild(AddSelectorBox());
        el.appendChild(document.createElement("br"));
        el.appendChild(AddDurationBox());
        map.AddControl(el);
    } 
    catch (err) {
        SetError("AddUserBox", 0, err)
    }
}


function AddSelectorBox(){
    var el = null;
    try {
        el = document.createElement("span");
        ela = document.createTextNode("Shape Count:");
        el.appendChild(ela);
        elb = document.createElement("select");
        elb.id = "selector"
        try {
            elb.addEventListener("onchange", GetCount, false);
        } 
        catch (err) {
            elb.attachEvent('onchange', GetCount);
        }
        var countArray = new Array(20, 50, 100, 250, 500, 750, 1000, 1500, 2000, 3000, 4000, 5000, 10000);
        for (var i = 0; i < countArray.length; i++) {
            var elx = document.createElement("option");
            elx.value = countArray[i];
            var ely = document.createTextNode(countArray[i]);
            elx.appendChild(ely);
            elb.appendChild(elx);
        }
        el.appendChild(elb);
    } 
    catch (err) {
        SetError("AddSelectorBox", 0, err)
    }
    finally {
        return el;
    }
}

function AddDurationBox(){
    var el = null;
    try {
        el = document.createElement("span");
        ela = document.createTextNode("Performance:");
        el.appendChild(ela);
        elb = document.createElement("input");
        elb.id = "timer"
        elb.type = "text"
        value = "";
        timeControl = elb;
        el.appendChild(elb);
    } 
    catch (err) {
        SetError("AddDurationBox", 0, err)
    }
    finally {
        return el;
    }
}

function GetCount(){
    try {
        var el = document.getElementById("selector");
        shapeCount = el.value;
        //alert(shapeCount);
        DrawDefaultLayerShapes();
    } 
    catch (err) {
        SetError("GetCount", 0, err)
    }
}

function StartPerformanceCounter(){
    startTime = new Date();
}

function EndPerformanceCounter(){
    endTime = new Date();
    var durationInSeconds = endTime.getSeconds() - startTime.getSeconds();
    var durationInMilliseconds = endTime.getMilliseconds() - startTime.getMilliseconds();
    var duration = (durationInSeconds * 1000) + durationInMilliseconds;
    timeControl.value = duration + " ms";
}
