var map = null;
var maptype = "Map";
var findControl = null;
var maincontrol = null;
var controlBox = null;
var controlBoxExpanded = true;
//params = zoom, mapstyle, labels, fleet, land, geo, inset map, traffic, radius...
var params = new Array(3, "r", true, true, false, false, false, false, false);
var fleetlayer = null;
var landlayer = null;
var geolayer = null;
var findlayer = null;
var EarthRadiusInMiles = 3956.0;
var EarthRadiusInKilometers = 6367.0;
var EarthRadius = EarthRadiusInKilometers;
var draglayer = null;
var dragop = false;
var dragXY = null;
var ring = null;
var vexwidth = 0;
var vexheight = 0;
var controltop = 0;
var debugBox = null;

function GetMap(type){
    try {
        //alert("maptype:" + type);
        if (type != null) 
            maptype = type;
        switch (maptype) {
            case "Map":
            case "History":
                controltop = "297";
                break;
            case "Landmark":
                controltop = "490";
                break;
            case "GeoZone":
            case "Landmark_GeoZone":
                controltop = "535";
        }
        GetFrameWidth();
        GetFrameHeight();
        GetUserValues();
        var options = new VEMapOptions();
        options.EnableDashboardLabels = params[2];
        var cpin = new VELatLong(45.95115, -97.20703);
        map = new VEMap('vex');
        //map.SetDashboardSize(VEDashboardSize.Normal);
        //map.HideDashboard();
        map.EnableShapeDisplayThreshold(false);
        //map.SetClientToken(token);
        map.LoadMap(cpin, params[0], params[1], false, VEMapMode.Mode2D, true, options);
        //FrameResize();
        CreateLayers();
        var distanceUnit = VEDistanceUnit.Kilometers;
        if (unitOfMes != undefined && unitOfMes == "Mi") {
            distanceUnit = VEDistanceUnit.Miles;
            EarthRadius = EarthRadiusInMiles;
        }
        map.SetScaleBarDistanceUnit(distanceUnit);
        map.AttachEvent("onchangeview", MapStyleChange);
        map.AttachEvent("onclick", MapClicked);
        //        map.AttachEvent("oninitmode", MapInit);
        //        map.AttachEvent("onmouseup",MapMouseUp);
        //        map.AttachEvent("onmousedown",MapMouseDown);
        map.AttachEvent("onstartpan", MapStartPan);
        map.AttachEvent("onstartzoom", MapStartZoom);
        map.AttachEvent("ontokenerror", MapError);
        map.AttachEvent("ontokenexpired", MapError);
        map.AttachEvent("onerror", MapError);
        
        //map.AddControl(AddDebugBox());
        
        map.AddControl(AddControlBox());
        map.AddControl(AddFlexControlBox());
        map.AddControl(AddFinder());
        
        
        SetShowInsetMap();
        
        var pin = new VEShape(VEShapeType.Pushpin, cpin);
        var pinhtml = "<img src='../veicons/trans.gif' />";
        pin.SetCustomIcon(pinhtml);
        map.AddShape(pin);
        map.SetCenterAndZoom(pin.GetIconAnchor(), 3);
        
        setTimeout("MapFleet();", 500);
        setTimeout("MapLandmarks();", 1000);
        setTimeout("MapGeozones();", 1500);
        
        
        
    } 
    catch (err) {
        alert("GetMap:" + err.description);
    }
}


function MapInit(mapevent){
    alert("map init: ");
}

function MapError(mapevent){
    alert("map error: " + mapevent.error);
}

function CreateLayers(){
    try {
        if (geolayer == null) {
            geolayer = new VEShapeLayer();
            geolayer.SetTitle("Geozone Layer");
            //geolayer.Hide();
            map.AddShapeLayer(geolayer);
        }
        if (landlayer == null) {
            landlayer = new VEShapeLayer();
            landlayer.SetTitle("Landmark Layer");
            //landlayer.Hide();
            map.AddShapeLayer(landlayer);
        }
        if (fleetlayer == null) {
            fleetlayer = new VEShapeLayer();
            fleetlayer.SetTitle("Fleet Assets Layer");
            map.AddShapeLayer(fleetlayer);
        }
        if (draglayer == null) {
            draglayer = new VEShapeLayer();
            draglayer.SetTitle("Drag Op Layer");
            map.AddShapeLayer(draglayer);
        }
    } 
    catch (err) {
        alert("CreateLayers:" + err.description);
    }
}

function MapFleet(){
    try {
    
        switch (maptype) {
            case "Map":
                CreateAvlShapes();
                SetShowTraffic();
                break;
            case "History":
                CreateAvlShapes();
                break;
            case "Landmark":
            case "GeoZone":
                GetDrawMap();
                return;                break;
            case "Landmark_GeoZone":
                return;                break;
        }
        if (fleetlayer.GetShapeCount() > 0) 
            AdjustView();
        else 
            map.SetZoomLevel(3);
        ShowFind();
        SetShowFleet();
    } 
    catch (err) {
        alert("MapFleet:" + err.description);
    }
}

function MapLandmarks(){
    LoadLandmarks();
    if (maptype == "Landmark_GeoZone" || maptype == "Landmark") 
        landlayer.Show();
    else 
        if (maptype == "Map" || maptype == "History") 
            SetShowLandmarks();
}

function MapGeozones(){
    LoadGeozones();
    if (maptype == "Landmark_GeoZone" || maptype == "GeoZone") 
        geolayer.Show();
    else 
        if (maptype == "Map" || maptype == "History") 
            SetShowGeozones();
    
    if (maptype == "Landmark_GeoZone") 
        AdjustViewGeozoneLandmarks();
}

function CreateAvlShapes(){
    //alert (avls);
    if (avls.length == 0) 
        return;
    var avlShapes = new Array();
    fleetlayer.DeleteAllShapes();
    try {
        //<tr><td class="caption">Unit:</td><td class="value">##UNIT##</td></tr>
        var avldesc = '<table cellpadding="0" cellspacing="0" border="0"><tr><td class="caption">Timestamp:</td><td class="value">##TIMESTAMP##</td></tr><tr><td class="caption">Address:</td><td class="value">##ADDRESS##</td></tr><tr><td class="caption">Speed:</td><td class="value">##SPEED##</td></tr><tr><td class="caption">Status:</td><td class="value">##STATUS##</td></tr><tr><td>&nbsp;</td><td style="text-align:right;"><img src="../veicons/###LOGO###" /></td></tr></table>';
        var avlinfos = avls.split('|');
        //        var pointline = new Array();
        for (var i = 0; i < avlinfos.length - 1; i++) {
            if (avlinfos[i].length == 0) 
                break;
            try {
                //map "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}|", (int)model, description, latitude, longitude, timestamp, address, status, speed, duration, iconName);
                //his "{0};{1};{2};{3};{4};{5};{6};{7};{8}|",     (int)model, description, latitude, longitude, timestamp, address, messageType, speed, iconName
                var avlinfo = avlinfos[i].split(';');
                var point = new VELatLong(avlinfo[2], avlinfo[3]);
                var pin = new VEShape(VEShapeType.Pushpin, point);
                pin.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
                var cicon = new VECustomIconSpecification();
                var avld = avldesc;
                if (maptype == "Map") {
                    cicon.ForeColor = new VEColor(255, 0, 0, 1.0);
                    cicon.TextContent = avlinfo[1];
                    cicon.TextFont = "Arial";
                    cicon.TextSize = 8;
                    cicon.TextOffset = new VEPixel(20, 0);
                }
                avld = avld.replace("##LOGO##", logo);
                avld = avld.replace("##UNIT##", avlinfo[9]);
                avld = avld.replace("##TIMESTAMP##", avlinfo[4]);
                avld = avld.replace("##ADDRESS##", avlinfo[5]);
                avld = avld.replace("##STATUS##", avlinfo[6]);
                if (maptype == "Map") {
                    cicon.Image = "../veicons/" + avlinfo[9];//+".gif";
                }
                else {
                    cicon.Image = "../veicons/" + avlinfo[8];//+".gif";
                }
                avld = avld.replace("##SPEED##", avlinfo[7]);
                cicon.Image = cicon.Image.replace(".gif", ".ico");
                pin.SetTitle("<div class='infotitle'>" + avlinfo[1] + "</div>");
                pin.SetDescription(avld);
                pin.SetCustomIcon(cicon);
                avlShapes.push(pin);
                
                //               if(maptype=="History") 
                //                   pointline.push(new VELatLong(avlinfo[2],avlinfo[3]));
            
            } 
            catch (err) {
                alert("Create AVL Shape:" + err.description + "\r\n" + avlinfo);
            }
        }
        try {
            fleetlayer.AddShape(avlShapes);
        } 
        catch (err) {
            alert("Load shapes to fleet layer:" + err.description + "\r\n" + pointline.length);
        }
        //        try {
        //            if(maptype=="History") {
        //                //alert("isHistory points:"+pointline.length);
        //                var avlline=new VEShape(VEShapeType.Polyline,pointline);
        //                avlline.SetLineColor(new VEColor(255,0,0,1));
        //                avlline.SetLineWidth(1);
        //                avlline.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
        //                avlline.SetZIndex(800,800);
        //                fleetlayer.AddShape(avlline);
        //            }
        //        } catch(err) { alert("Create History trail:"+err.description+"\r\n"+pointline.length);}
    
    
    } 
    catch (err) {
        alert("Create AVL Shapes:" + err.description + "\r\n" + avlinfos);
    }
}

function AddControlBox(){
    try {
        var el = document.createElement("div");
        el.style.top = controltop + "px";
        el.id = "controlbox";
        el.className = "mapcontrolactivator";
        var a = document.createElement("a");
        a.href = "javascript:void(0);";
        var i = document.createElement("img");
        i.alt = "Expand/Contract Toolbar";
        i.border = 0;
        i.src = "../veicons/" + logo;
        a.appendChild(i);
        try {
            a.addEventListener("click", CollapseToolbar, false);
        } 
        catch (err) {
            a.attachEvent('onclick', CollapseToolbar);
        }
        el.appendChild(a);
        controlBox = el;
        
        return el;
    } 
    catch (err) {
        alert("AddControlBox:" + err.description);
    }
}

function AddDebugBox(){
    try {
        var el = document.createElement("div");
        el.id = "debug";
        debugBox = el;
        return el;
    } 
    catch (err) {
        alert("AddDebugBox:" + err.description);
    }
}

function AddFlexControlBox(){
    try {
        var el = document.createElement("div");
        el.id = "maincontrol";
        el.className = "mapcontrolbox";
        el.style.left = "21px";
        el.style.top = controltop + "px";
        switch (maptype) {
            case "Map":
                el.appendChild(AddFleetCheckBox());
                el.appendChild(AddLandmarkCheckBox());
                el.appendChild(AddGeozoneCheckBox());
                el.appendChild(AddTrafficCheckBox());
                el.appendChild(AddFinderCheckBox());
                break;
            case "History":
                el.appendChild(AddFleetCheckBox());
                el.appendChild(AddLandmarkCheckBox());
                el.appendChild(AddGeozoneCheckBox());
                break;
            case "Landmark":
            case "GeoZone":
            case "Landmark_GeoZone":
                break;
        }
        el.appendChild(AddInsetMapCheckBox());
        maincontrol = el;
        return el;
    } 
    catch (err) {
        alert("AddFlexControlBox:" + err.description);
    }
}

function AddFleetCheckBox(){
    try {
        var s = document.createElement("span");
        var cb = document.createElement("input");
        cb.id = "mbfleet";
        cb.type = "checkbox";
        cb.defaultChecked = true;
        cb.title = "Controls the visibility of the Fleet layer";
        try {
            cb.addEventListener("click", SetShowFleet, false);
        } 
        catch (err) {
            cb.attachEvent('onclick', SetShowFleet);
        }
        s.appendChild(cb);
        s.appendChild(document.createTextNode('Fleet'));
        var i = document.createElement("img");
        i.border = 0;
        i.src = "../veicons/trans.gif";
        s.appendChild(i);
        return s;
    } 
    catch (err) {
        alert("AddFleetCheckBox:" + err.description);
    }
}

function AddLandmarkCheckBox(){
    try {
        var s = document.createElement("span");
        if (landmarksactive != undefined && landmarksactive) {
        
            var cb = document.createElement("input");
            cb.id = "mbland";
            cb.type = "checkbox";
            cb.defaultChecked = false; //params[4]=="true";
            cb.title = "Controls the visibility of the Landmark layer";
            try {
                cb.addEventListener("click", SetShowLandmarks, false);
            } 
            catch (err) {
                cb.attachEvent('onclick', SetShowLandmarks);
            }
            s.appendChild(cb);
            s.appendChild(document.createTextNode('Landmarks'));
            var i = document.createElement("img");
            i.border = 0;
            i.src = "../veicons/trans.gif";
            s.appendChild(i);
        }
        return s;
    } 
    catch (err) {
        alert("AddLandmarkCheckBox:" + err.description);
    }
}

function AddGeozoneCheckBox(){
    try {
        var s = document.createElement("span");
        if (geozonesactive != undefined && geozonesactive) {
            var cb = document.createElement("input");
            cb.id = "mbgeo";
            cb.type = "checkbox";
            cb.defaultChecked = false; //params[5]=="true";
            cb.title = "Controls the visibility of the Geozone layer";
            try {
                cb.addEventListener("click", SetShowGeozones, false);
            } 
            catch (err) {
                cb.attachEvent('onclick', SetShowGeozones);
            }
            s.appendChild(cb);
            s.appendChild(document.createTextNode('Geozones'));
            var i = document.createElement("img");
            i.border = 0;
            i.src = "../veicons/trans.gif";
            s.appendChild(i);
        }
        return s;
    } 
    catch (err) {
        alert("AddGeozoneCheckBox:" + err.description);
    }
}

function AddTrafficCheckBox(){
    try {
        var s = document.createElement("span");
        var cbt = document.createElement("input");
        cbt.id = "mbtraffic";
        cbt.type = "checkbox";
        cbt.defaultChecked = params[7] == "true";
        cbt.title = "Controls the visibility of Traffic Data";
        try {
            cbt.addEventListener("click", SetShowTraffic, false);
        } 
        catch (err) {
            cbt.attachEvent('onclick', SetShowTraffic);
        }
        s.appendChild(cbt);
        s.appendChild(document.createTextNode('Traffic'));
        var i = document.createElement("img");
        i.border = 0;
        i.src = "../veicons/trans.gif";
        s.appendChild(i);
        return s;
    } 
    catch (err) {
        alert("AddTrafficCheckBox:" + err.description);
    }
}


function AddFinderCheckBox(){
    try {
        var s = document.createElement("span");
        var cbt = document.createElement("input");
        cbt.id = "mbfindx";
        cbt.type = "checkbox";
        cbt.Checked = "false";
        cbt.title = "Controls the visibility of finder control";
        try {
            cbt.addEventListener("click", ShowFind, false);
        } 
        catch (err) {
            cbt.attachEvent('onclick', ShowFind);
        }
        s.appendChild(cbt);
        s.appendChild(document.createTextNode('Find'));
        var i = document.createElement("img");
        i.border = 0;
        i.src = "../veicons/trans.gif";
        s.appendChild(i);
        return s;
    } 
    catch (err) {
        alert("AddFinderCheckBox:" + err.description);
    }
}

function AddInsetMapCheckBox(){
    try {
        var s = document.createElement("span");
        var cb = document.createElement("input");
        cb.id = "mbmini";
        cb.type = "checkbox";
        cb.defaultChecked = params[6] == "true";
        cb.title = "Controls the visibility of the Inset Map";
        try {
            cb.addEventListener("click", SetShowInsetMap, false);
        } 
        catch (err) {
            cb.attachEvent('onclick', SetShowInsetMap);
        }
        s.appendChild(cb);
        s.appendChild(document.createTextNode('Inset Map'));
        var i = document.createElement("img");
        i.border = 0;
        i.src = "../veicons/trans.gif";
        s.appendChild(i);
        return s;
    } 
    catch (err) {
        alert("AddInsetMapCheckBox:" + err.description);
    }
}

function AddFinder(){
    try {
        var el2 = document.createElement("div");
        el2.id = "findcontrol";
        el2.className = "mapfindbox";
        el2.style.left = "21px";
        el2.style.top = (controltop - 28) + "px";
        el2.style.visibility = "hidden";
        var cbi17 = document.createElement("img");
        cbi17.border = 0;
        cbi17.src = "../veicons/trans.gif";
        el2.appendChild(cbi17);
        var cbfb = document.createElement("input");
        cbfb.id = "mbfindbox";
        cbfb.className = "minput";
        cbfb.type = "text";
        cbfb.title = "Show Criteria";
        el2.appendChild(cbfb);
        var cbi6 = document.createElement("img");
        cbi6.border = 0;
        cbi6.src = "../veicons/trans.gif";
        el2.appendChild(cbi6);
        var cbfbx = document.createElement("input");
        cbfbx.id = "mbfind";
        cbfbx.type = "button";
        cbfbx.value = "Find";
        //cbfbx.title="Perform search based on specified criteria";
        try {
            cbfbx.addEventListener("click", DoFind, false);
        } 
        catch (err) {
            cbfbx.attachEvent('onclick', DoFind);
        }
        el2.appendChild(cbfbx);
        var cbi7 = document.createElement("img");
        cbi7.border = 0;
        cbi7.src = "../veicons/trans.gif";
        el2.appendChild(cbi7);
        return el2;
    } 
    catch (err) {
        alert("AddFinder:" + err.description);
    }
}

function CollapseToolbar(){
    if (controlBoxExpanded && controlBox != null) {
        controlBoxExpanded = false;
        map.HideControl(maincontrol);
        //        if (maptype=="Map"||maptype=="History")
        //            map.HideControl(drawcontrol);
    }
    else {
        controlBoxExpanded = true;
        map.ShowControl(maincontrol);
        //        if (maptype=="Map"||maptype=="History")
        //            map.ShowControl(drawcontrol);
    }
}

function Get(){
    var rect = draglayer.GetBoundingRectangle();
    if (rect == null) 
        alert("nope");
    else {
        var rectstr = "";
        rectstr = "topleft:" + rect.TopLeftLatLong.Latitude + "," + rect.TopLeftLatLong.Longitude + "\r\n";
        rectstr = rectstr + "bottomright:" + rect.BottomRightLatLong.Latitude + "," + rect.BottomRightLatLong.Longitude;
        alert(rectstr);
    }
}

function SetShowInsetMap(){
    params[6] = document.getElementById("mbmini").checked;
    SetUserValues();
    ShowInsetMap();
}

function ShowInsetMap(){
    if (params[6] == true) 
        map.ShowMiniMap();
    else 
        map.HideMiniMap();
}

function SetShowFleet(){
    params[3] = document.getElementById("mbfleet").checked;
    SetUserValues();
    ShowFleet();
}

function ShowFleet(){
    if (params[3] == true) 
        fleetlayer.Show();
    else 
        fleetlayer.Hide();
}

function SetShowLandmarks(){
    params[4] = document.getElementById("mbland").checked;
    SetUserValues();
    ShowLandmarks();
}

function ShowLandmarks(){
    if (landmarksactive == false) {
        landlayer.Hide();
        return;
    }
    if (params[4] == true) 
        landlayer.Show();
    else 
        landlayer.Hide();
}

function SetShowGeozones(){
    params[5] = document.getElementById("mbgeo").checked;
    SetUserValues();
    ShowGeozones();
}

function ShowGeozones(){
    if (geozonesactive == false) {
        geolayer.Hide();
        return;
    }
    if (params[5] == true) 
        geolayer.Show();
    else 
        geolayer.Hide();
}

function SetShowTraffic(){
    params[7] = document.getElementById("mbtraffic").checked;
    SetUserValues();
    ShowTraffic();
}

function ShowTraffic(){
    if (params[7] == true) 
        LoadTraffic();
    else 
        ClearTraffic();
}

function DoFind(){
    try {
        if (document.getElementById("mbfindbox") == undefined) 
            return;
        var ms = document.getElementById("mbfindbox");
        //VEMap.Find(what, where, findType, shapeLayer, startIndex, numberOfResults, showResults, createResults, useDefaultDisambiguation, setBestMapView, callback);
        results = map.Find(null, ms.value, null, null, 0, 20, false, false, true, true, SearchResults);
    } 
    catch (e) {
        alert(e.message);
    }
}

function SearchResults(layer, resultsArray, places, hasMore, veErrorMessage){
    alert(resultsArray.length + "\r\n" + places.length);
    if (findlayer == null) {
        findlayer = new VEShapeLayer();
        findlayer.SetTitle = "Find Assets Layer";
        map.AddShapeLayer(findlayer);
    }
    findlayer.DeleteAllShapes();
    for (var i = 0; i < places.length; i++) {
        var pin = new VEShape(VEShapeType.Pushpin, places[i].LatLong);
        pin.SetTitle(place[i].Name);
        findlayer.AddShape(pin);
    }
}

function GetMapInfo(){
    var center = map.GetCenter();
    var zoom = map.GetZoomLevel();
    alert("Center: " + center.Latitude + "," + center.Longitude + "\r\nZoom:" + zoom + "\r\n:" + zoom);
}

function CreateCookie(name, value){
    var expires = "";
    document.cookie = name + "=" + value + expires + ";path=/";
}

function ReadCookie(name){
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') 
            c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) 
            return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function EraseCookie(name){
    createCookie(name, "", -1);
}

function GetUserValues(){
    try {
        var cookie = ReadCookie("bsmve");
        if (cookie == null || cookie == "") 
            CreateCookie("bsmve", params.join(","));
        else 
            params = cookie.split(",");
    } 
    catch (err) {
        alert("GetUserValues:" + err.description);
    }
}

function SetUserValues(){
    try {
        CreateCookie("bsmve", params.join(","));
    } 
    catch (err) {
        alert("SetUserValues:" + err.description);
    }
}

function MapStyleChange(mapevent){
    try {
        if (params[0] == mapevent.zoomLevel && params[1] == mapevent.mapStyle) 
            return;
        params[0] = mapevent.zoomLevel
        params[1] = mapevent.mapStyle;
        SetUserValues();
    } 
    catch (err) {
    }
}

function MapClicked(mapevent){
    try {
        var shape = map.GetShapeByID(mapevent.elementID);
        if (mapevent.leftMouseButton) {
            if (shape != null) 
                map.SetCenterAndZoom(shape.GetIconAnchor(), 15);
        }
        else 
            if (mapevent.rightMouseButton) {
                if (shape != null) {
                    var name = shape.GetTitle();
                    var pos = name.indexOf(">") + 1;
                    name = name.substr(pos, 100);
                    pos = name.indexOf("<");
                    name = name.substr(0, pos);
                    //alert (name);
                    switch (shape.GetShapeLayer().GetTitle()) {
                        case "Geozone Layer":
                            var lm = ReadCookie("bsmgeozones");
                            if (lm == null || lm == "") 
                                return;
                            var xlm = new Array();
                            xlm = lm.split("|");
                            var lmShapes = new Array();
                            for (var i = 0; i < xlm.length; i++) {
                                try {
                                    if (xlm[i].length == 0) 
                                        break;
                                    var lminfo = xlm[i].split('$');
                                    if (lminfo[1] == name) {
                                        var zoom = true;
                                        if (mapevent.shiftKey == true) 
                                            zoom = false;
                                        ViewGeozone(lminfo, true, zoom);
                                        ViewGeozone(lminfo, false, zoom);
                                        return;
                                    }
                                } 
                                catch (err) {
                                } //alert("MapClicked:G:"+err.description);}                         
                            }
                            break;
                        case "Landmark Layer":
                            var lm = ReadCookie("bsmlandmarks");
                            if (lm == null || lm == "") 
                                return;
                            var xlm = new Array();
                            xlm = lm.split("|");
                            var lmShapes = new Array();
                            for (var i = 0; i < xlm.length; i++) {
                                try {
                                    if (xlm[i].length == 0) 
                                        break;
                                    var lminfo = xlm[i].split('$');
                                    if (lminfo[1] == name) {
                                        var radius = lminfo[4]; //50;
                                        var point = new VELatLong(lminfo[2], lminfo[3]);
                                        var zoom = true;
                                        if (mapevent.shiftKey == true) 
                                            zoom = false;
                                        ViewRadius(point, radius, true, zoom);
                                        ViewRadius(point, radius, false, zoom);
                                        
                                        return;
                                    }
                                } 
                                catch (err) {
                                } //alert("MapClicked:L:"+err.description);}                         
                            }
                            break;
                            
                    }
                //alert(shape.GetShapeLayer().GetTitle() + "\r\n" + shape.GetTitle());
                }
            }
    } 
    catch (err) {
        alert("mouseclick:" + mapevent.error);
    }
}

function MapMouseUp(mapevent){
    try {
        if (mapevent.leftMouseButton && mapevent.shiftKey) {
            map.vemapcontrol.EnableGeoCommunity(false);
            var endXY = map.PixelToLatLong(new VEPixel(mapevent.mapX, mapevent.mapY));
            var lat = dragXY.Latitude - ((dragXY.Latitude - endXY.Latitude) / 2);
            var lon = dragXY.Longitude - ((dragXY.Longitude - endXY.Longitude) / 2);
            //alert(lat + "\r\n" + lon + "\r\n" + dragXY.Latitude + "\r\n" + dragXY.Longitude + "\r\n" + endXY.Latitude + "\r\n" + endXY.Longitude + "\r\n");
            var cpin = new VEShape(VEShapeType.Pushpin, new VELatLong(lat, lon));
            var cicon = new VECustomIconSpecification();
            cicon.ForeColor = new VEColor(255, 0, 0, 1.0);
            cicon.TextContent = "Click here to center view";
            cicon.TextFont = "Arial";
            cicon.TextSize = 9;
            cicon.TextOffset = new VEPixel(20, 0);
            cicon.Image = "../icons/GeoZoneCentr.ico";
            cpin.SetCustomIcon(cicon);
            draglayer.AddShape(cpin);
        }
    } 
    catch (err) {
        alert("mouseup:" + mapevent.error);
    }
    
    //AdjustView(draglayer);
}

function MapMouseDown(mapevent){
    try {
        if (mapevent.leftMouseButton && mapevent.shiftKey) {
            map.vemapcontrol.EnableGeoCommunity(true);
            draglayer.DeleteAllShapes();
            dragXY = map.PixelToLatLong(new VEPixel(mapevent.mapX, mapevent.mapY));
        }
    } 
    catch (err) {
        alert("mousedown:" + mapevent.error);
    }
}

function MapStartPan(mapevent){
    try {
        draglayer.DeleteAllShapes();
    } 
    catch (err) {
        alert("startpan:" + mapevent.error);
    }
}

function MapStartZoom(mapevent){
    try {
        draglayer.DeleteAllShapes();
    } 
    catch (err) {
        alert("startzoom:" + mapevent.error);
    }
}

function AdjustView(){
    try {
        if (fleetlayer) {
            var count = fleetlayer.GetShapeCount();
            if (count == 0) 
                return;
            map.SetMapView(fleetlayer.GetBoundingRectangle());
            if (count == 1) {
                map.SetZoomLevel(7);
            }
            else 
                map.SetZoomLevel(map.GetZoomLevel() - 1);
        }
    } 
    catch (err) {
        alert("AdjustView:" + err.description);
    }
}

function AdjustViewGeozoneLandmarks(){
    try {
        map.SetMapView(landlayer.GetBoundingRectangle());
        if (landlayer.GetShapeCount() <= 1 | geolayer.GetShapeCount() <= 1) {
            map.SetZoomLevel(7);
        }
    } 
    catch (err) {
        alert("AdjustViewGeozoneLandmarks:" + err.description);
    }
}


function LoadGeozones(){
    try {
        if (geozonesactive != undefined) {
            //alert("geozonesactive:"+geozonesactive+"\r\geozones:"+geozones+"\r\nmaptype:"+maptype+"\r\nedit:"+edit);
            if (!geozonesactive) 
                return;
            var setview = false;
            if (geozones != null && geozones.length > 0) {
                geozones = geozones.replace(/;/g, "$");
                if (maptype == "Map" || maptype == "History") 
                    CreateCookie("bsmgeozones", geozones);
                else 
                    if (maptype == "Landmark_GeoZone") 
                        setview = false;
                    else 
                        setview = true;
            }
            else 
                geozones = ReadCookie("bsmgeozones");
            if (geozones == null || geozones == "") 
                return;
            //alert(geozones);
            var xlg = new Array();
            xlg = geozones.split("|");
            //"{0};{1};{2};{3};{4};{5}|", (int)model, description, numPoints, pointList, type, severity
            var geoShapes = new Array();
            var geodesc = '<table cellpadding="0" cellspacing="0" border="0"><tr><td class="caption">Geozone Type:</td><td class="value">##TYPE##</td></tr><tr><td class="caption">Severity:</td><td class="value">##SEVERITY##</td></tr><tr><td class="caption">Points:</td><td class="value">##POINTS##</td></tr><tr><td>&nbsp;</td><td style="text-align:right;"><img src="../veicons/' + logo + '" /></td></tr></table>';
            for (var i = 0; i < xlg.length; i++) {
                try {
                    if (xlg[i].length == 0) 
                        break;
                    var geoinfo = xlg[i].split('$');
                    var numpoints = geoinfo[2];
                    var index = 3;
                    var point = new VELatLong(geoinfo[index], geoinfo[index + 1]);
                    var pin = new VEShape(VEShapeType.Pushpin, point);
                    index = index + (numpoints * 2);
                    var type = "";
                    if (geoinfo[index] == 2) 
                        type = "_in";
                    else 
                        if (geoinfo[index] == 1) 
                            type = "_out";
                    var cicon = new VECustomIconSpecification();
                    cicon.Image = "../veicons/geozone" + type + ".gif";
                    pin.SetCustomIcon(cicon);
                    var geod = geodesc;
                    geod = geod.replace("##TYPE##", geoinfo[index++]);
                    geod = geod.replace("##SEVERITY##", geoinfo[index]);
                    geod = geod.replace("##POINTS##", geoinfo[2]);
                    pin.SetTitle("<div class='infotitle'>" + geoinfo[1] + "</div>");
                    pin.SetDescription(geod);
                    pin.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
                    pin.SetZIndex(875, 875);
                    geoShapes.push(pin);
                    if (setview && i == 0) 
                        var geoinfox = geoinfo;
                    
                } 
                catch (err) {
                    alert("Create Geozone Shape:" + err.description + "\r\n" + xlg[i]);
                }
            }
            geolayer.AddShape(geoShapes);
            if (setview) {
                //map.SetCenterAndZoom(geoShapes[0].GetIconAnchor(), 15);
                ViewGeozone(geoinfox, true);
                ViewGeozone(geoinfox, false);
                
                //map.SetMapView(draglayer.GetBoundingRectangle());
            }
        }
    } 
    catch (err) {
        alert("LoadGeozones:" + err.description);
    }
}

function LoadLandmarks(){
    try {
        if (landmarksactive != undefined) {
            //alert("landmarksactive:"+landmarksactive+"\r\nlandmarks:"+landmarks+"\r\nmaptype:"+maptype);//+"\r\nedit:"+edit);
            var setview = false;
            if (!landmarksactive) 
                return;
            if (landmarks != null && landmarks.length > 0) {
                landmarks = landmarks.replace(/;/g, "$");
                if (maptype == "Map" || maptype == "History") 
                    CreateCookie("bsmlandmarks", landmarks);
                else 
                    if (maptype == "Landmark_GeoZone") 
                        setview = false;
                    else 
                        setview = true;
            }
            else 
                landmarks = ReadCookie("bsmlandmarks");
            if (landmarks == null || landmarks == "") 
                return;
            var xlm = new Array();
            xlm = landmarks.split("|");
            //{0};{1};{2};{3};{4};{5}|", (int)model, description, latitude, longitude,radius,iconName
            var lmShapes = new Array();
            //var rShapes=new Array();
            for (var i = 0; i < xlm.length; i++) {
                try {
                    if (xlm[i].length == 0) 
                        break;
                    var lminfo = xlm[i].split('$');
                    var point = new VELatLong(lminfo[2], lminfo[3]);
                    var pin = new VEShape(VEShapeType.Pushpin, point);
                    //var cicon = new VECustomIconSpecification();
                    var pinhtml = "<div style='padding-top:6px;padding-left:6px;'><img src='../veicons/" + lminfo[5].replace(".gif", ".ico") + "' /></div>"
                    //cicon.Image = "../veicons/" + lminfo[5];//+".gif";
                    //cicon.Image = cicon.Image.replace(".gif", ".ico");
                    //cicon.ImageOffset = new VEPixel(128,128);
                    pin.SetTitle("<div class='infotitle'>" + lminfo[1] + "</div>");
                    //pin.SetDescription(lminfo[1]);
                    pin.SetCustomIcon(pinhtml); //cicon);
                    pin.SetZIndex(900, 900);
                    pin.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
                    lmShapes.push(pin);
                } 
                catch (err) {
                    alert("Create Landmark Shape:" + err.description + "\r\n" + xlm[i]);
                }
            }
            //alert(lmShapes.length);
            
            landlayer.AddShape(lmShapes);
            if (setview) 
                map.SetCenterAndZoom(lmShapes[0].GetIconAnchor(), 15);
            
        }
    } 
    catch (err) {
        alert("LoadLandmarks:" + err.description);
    }
}

function ViewRadius(point, radius, setsize, setzoomlevel){
    //alert("ViewRadius:"+radius);
    try {
        if (radius > 0) { //do some calcs for visiblity of rings...
            ring = CreateCircle(point, radius / 1000);
            ring.HideIcon();
            ring.SetLineColor(new VEColor(255, 255, 255, 1));
            //            var minzl = 1;
            //            var maxzl = 21;
            //            ring.SetMaxZoomLevel(maxzl);
            //            ring.SetMinZoomLevel(minzl);
            ring.SetLineColor(new VEColor(255, 0, 0, 1));
            ring.SetLineWidth(1);
            ring.SetZIndex(850, 850);
            ring.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
            draglayer.AddShape(ring);
            if (setsize && setzoomlevel) 
                map.SetMapView(draglayer.GetBoundingRectangle());
        }
    } 
    catch (err) {
        alert("ViewRadius:" + err.description);
    }
}

function ViewGeozone(geoinfo, setsize, setzoomlevel){
    //alert("ViewGeozone");
    //alert(geoinfo.length);
    
    try {
        var points = new Array();
        var numpoints = geoinfo[2];
        var index = 3;
        if (numpoints == 2) { //rectangle
            for (var x = 0; x < numpoints; x++) {
                points.push(new VELatLong(geoinfo[index], geoinfo[index + 1]));
                index++;
                points.push(new VELatLong(geoinfo[index], geoinfo[index + 1]));
                index++;
            }
            points[1] = new VELatLong(points[0].Latitude, points[2].Longitude);
            points[3] = new VELatLong(points[2].Latitude, points[0].Longitude);
        }
        else {
            for (var x = 0; x < numpoints; x++) {
                var point = new VELatLong(geoinfo[index], geoinfo[index + 1]);
                points.push(point);
                index = index + 2;
            }
        }
        points.push(points[0]); //make our loop
        var shape = new VEShape(VEShapeType.Polyline, points);
        shape.SetLineColor(new VEColor(255, 0, 0, 1));
        shape.SetLineWidth(1);
        shape.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
        shape.SetZIndex(800, 800);
        draglayer.AddShape(shape);
        if (setsize && setzoomlevel) 
            map.SetMapView(draglayer.GetBoundingRectangle());
    } 
    catch (err) {
    } //alert("ViewGeozone:"+err.description+"\r\nme:"+mapevent.error);}   
}

function LoadTraffic(){
    map.LoadTraffic(true);
    map.ShowTrafficLegend(65, 30);
    map.SetTrafficLegendText("Traffic");
}

function ClearTraffic(){
    map.ClearTraffic();
}

function ShowFind(){
    var finder = document.getElementById("mbfindx");
    if (finder != null && findcontrol != null) {
        if (finder.checked) 
            map.ShowControl(findcontrol);
        else 
            map.HideControl(findcontrol);
    }
}

function CreateCircle(point, radius){
    var latitude = ToRadian(point.Latitude);
    var longitude = ToRadian(point.Longitude);
    var d = parseFloat(radius) / EarthRadius;
    var plotPoints = new Array();
    for (var i = 0; i <= 360;) {
        var currentRadian = ToRadian(i);
        var latitudeRadians = Math.asin(Math.sin(latitude) * Math.cos(d) + Math.cos(latitude) * Math.sin(d) * Math.cos(currentRadian));
        var longitudeRadians = longitude + Math.atan2(Math.sin(currentRadian) * Math.sin(d) * Math.cos(latitude), Math.cos(d) - Math.sin(latitude) * Math.sin(latitudeRadians));
        plotPoints.push(new VELatLong(ToDegrees(latitudeRadians), ToDegrees(longitudeRadians)));
        i = i + 5;
    }
    return new VEShape(VEShapeType.Polyline, plotPoints);
}

function ToDegrees(radians){
    return radians * 180 / Math.PI;
}

function ToRadian(value){
    return value * (Math.PI / 180);
}

function DiffRadian(value1, value2){
    return ToRadian(value2) - ToRadian(value1);
}

function CalculateDistance(latitude1, longitude1, latitude2, longitude2, radius){
    return radius * 2 * Math.asin(Math.min(1, Math.sqrt((Math.pow(Math.sin((DiffRadian(latitude1, latitude2)) / 2.0), 2.0) + Math.cos(ToRadian(latitude1)) * Math.cos(ToRadian(latitude2)) * Math.pow(Math.sin((DiffRadian(longitude1, longitude2)) / 2.0), 2.0)))));
}

function FrameResize(){
    GetFrameWidth();
    GetFrameHeight();
    
	//alert("resize");
	
    if (map != null) {
        map.Resize(vexwidth, vexheight);
        AdjustView();
    }
    
    if (debugBox != null) 
        debugBox.innerHTML = "width:" +
        vexwidth +
        "<br>" +
        "height:" +
        vexheight;
    
}

function GetFrameHeight(){
    try {
        vexheight = document.body.clientHeight;
    } 
    catch (err) {
    }
}

function GetFrameWidth(){
    try {
        vexwidth = document.body.clientWidth;
    } 
    catch (err) {
    }
}

//function IncludeDrawFunctions() {
//    try {
//        var e=document.createElement('script');
//        e.setAttribute('src','vexdata.js');
//        document.body.appendChild(e);
//}

