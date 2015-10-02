
function BsmVE(type) {

    //VE control
    this.map = null;
    this.maptype = type;
    this.geowidthtrigger = 0.5;
    this.landwidthtrigger = 0.8;
    this.lonwidth = 1000;
    this.showBounds = false;
    this.bounds = null;
    this.lastGeozoneBounds = null;
    this.lastLandmarksBounds = null;
    this.geozonesInBounds = false;
    this.landmarkInBounds = false;
    this.imgdir = "images";
    
    
    //persisted user prefs
    //  ->> params = zoom, mapstyle, labels, fleet, land, geo, inset map, traffic, radius...
    params = new Array(3, "r", true, true, false, false, false, false, false);

    //user controls
    this.zoombox = null;
    
    //layers
    this.fleetlayer = null;
    this.fleetEnabled = true;
    
    this.landlayer = null;
    this.geolayer = null;
    this.findlayer = null;
    this.poilayer = null;
    this.draglayer = null;
    this.boundslayer = null;
    this.drawlayer = null;
    
    //constants
    this.earthRadiusInMiles = 3956.0;
    this.earthRadiusInKilometers = 6367.0;
    this.earthRadius = this.earthRadiusInKilometers;

    //other
    this.zoomlevel = 1;
    //this.currentLandmark = null;
    //this.currentGeoZone = null;
    this.dragop = false;
    this.dragXY = null;
    this.ring = null;
    this.trafficEnabled = false;
    this.insetMapEnabled = false;
    
    //custom events
    this.onFleetVisibilityChanged = null; 
    this.onTrafficVisibilityChanged = null; 
    this.onInsetMapVisibilityChanged = null; 
    this.onGeozonesAvailableChange = null;
    this.onLandmarksAvailableChange = null;
    this.onDrawStarted = null;
    this.onDrawEnded = null;
    this.onDrawChanged = null;
    this.onDrawCleared = null;
    this.onDrawSave = null;
    this.onDrawSubmit = null;
    this.onDrawClose = null;
    this.onDrawSetPoints = null;
    this.setview = false;
    
    //constructor
    this.loadMap = function() {
        try {
            //alert("maptype:" + type);

            this.onFleetVisibilityChanged = new YAHOO.util.CustomEvent('FleetVisibilityChange');
            this.onTrafficVisibilityChanged = new YAHOO.util.CustomEvent('TrafficVisibilityChange');
            this.onInsetMapVisibilityChanged = new YAHOO.util.CustomEvent('InsetMapVisibilityChange');
            this.onGeozonesAvailableChange = new YAHOO.util.CustomEvent('GeozonesAvailableChange');
            this.onLandmarksAvailableChange = new YAHOO.util.CustomEvent('LandmarksAvailableChange');
            this.onDrawStarted = new YAHOO.util.CustomEvent('DrawStarted');
            this.onDrawEnded = new YAHOO.util.CustomEvent('DrawEnded');
            this.onDrawChanged = new YAHOO.util.CustomEvent('DrawingChanged');
            this.onDrawCleared = new YAHOO.util.CustomEvent('DrawingCleared');
            this.onDrawSave = new YAHOO.util.CustomEvent('DrawingSave');
            this.onDrawSubmit = new YAHOO.util.CustomEvent('DrawingSubmit');
            this.onDrawClose = new YAHOO.util.CustomEvent('DrawingClose');
            this.onDrawSetPoints = new YAHOO.util.CustomEvent('DrawSetPoints');
            
            this.getUserValues();

            this.initializeVE(); 
            
        }
        catch(e) {
            alert("loadMap error: -> " + e.description);
        }
            
     }
    
     this.setUserInterface = function() {
         try {
                 this.showFleet(this.fleetEnabled);
                this.showTraffic(this.trafficEnabled);
                this.showInsetMap(this.insetMapEnabled);
            }
            catch(e) {
                alert("setUserInterface error: -> " + e.description);
            }
        
     }
    
    //creates the new VE object and loads the tiles
    this.initializeVE = function() {
        try {
            var options = new VEMapOptions();
            options.EnableDashboardLabels = params[2];
            var cpin = new VELatLong(45.95115, -97.20703);
            this.map = new VEMap('vex');
            
            if (this.map == null)
                return;
                
            //this.map.SetDashboardSize(VEDashboardSize.Normal);
            //this.map.HideDashboard();
            this.map.EnableShapeDisplayThreshold(false);
            
            this.map.SetClientToken(token);
            //alert("Token is set: " + token);
            
            this.map.LoadMap(cpin, params[0], params[1], false, VEMapMode.Mode2D, true, options);
                      
            var distanceUnit = VEDistanceUnit.Kilometers;
            if (this.unitOfMes == "Mi") {
                distanceUnit = VEDistanceUnit.Miles;
                this.earthRadius = this.earthRadiusInMiles;               
            }
            this.map.SetScaleBarDistanceUnit(distanceUnit);

            this.createLayers();
            
            this.addMapControls();
            
            this.attachMapEvents();
            
            this.setZoomLevel(7);

            if (this.maptype == "Map" || this.maptype == "History" || this.maptype == "Playback") {
                this.fleetlayer.DeleteAllShapes();
                this.addAvlShapes();
            }
            else if (this.maptype == "Landmark")
                this.setLandmarkData(landmark, false);
            else if (this.maptype == "GeoZone")
                this.setGeoZoneData(geoZone, false);
            else if (this.maptype == "Landmarks_GeoZones")
            {
                 this.setLandmarkData(landmark, false);
                 this.setGeoZoneData(geoZone, false);
            }
        }
        catch(e) {
            alert("createVeMap error: -> " + e.description);
        }
    }
    
    
    //intializes map event handlers
    this.attachMapEvents = function() {
//            this.map.AttachEvent("onchangeview", this.mapStyleChange);
            this.map.AttachEvent("onclick", mapClicked);
//            this.map.AttachEvent("oninitmode", this.mapInit);
//            this.map.AttachEvent("onmouseup",this.mapMouseUp);
//            this.map.AttachEvent("onmousedown",this.mapMouseDown);
            this.map.AttachEvent("onstartpan", mapStartPan);
            this.map.AttachEvent("onendpan", mapEndPan);
            this.map.AttachEvent("onstartzoom", mapStartZoom);
            this.map.AttachEvent("onendzoom", mapEndZoom);
//            this.map.AttachEvent("ontokenerror", this.mapError);
//            this.map.AttachEvent("ontokenexpired", this.mapError);
            this.map.AttachEvent("onerror", mapError);
            this.map.AttachEvent("onresize", mapResize);
    }   
    
    //create and add user controls
    this.addMapControls = function() {
        this.map.AddControl(this.addZoomLevelDisplay());
    }
    
///////////////////////////////////////////////////////////////////////////////////////////////////             LAYERS

    this.createLayers = function() {
      try {
            if (this.fleetlayer == null) {
                this.fleetlayer = new VEShapeLayer();
                this.fleetlayer.SetTitle("Fleet Assets Layer");
                this.map.AddShapeLayer(this.fleetlayer);
            }
            if (this.landlayer == null) {
                this.landlayer = new VEShapeLayer();
                this.landlayer.SetTitle("Landmark Layer");
                //landlayer.Hide();
                this.map.AddShapeLayer(this.landlayer);
            }            
            if (this.geolayer == null) {
                this.geolayer = new VEShapeLayer();
                this.geolayer.SetTitle("GeoZone Layer");
                //geolayer.Hide();
                this.map.AddShapeLayer(this.geolayer);
            }
            if (this.findlayer == null) {
                this.findlayer = new VEShapeLayer();
                this.findlayer.SetTitle("Find Layer");
                //findlayer.Hide();
                this.map.AddShapeLayer(this.findlayer);
            }
            if (this.poilayer == null) {
                this.poilayer = new VEShapeLayer();
                this.poilayer.SetTitle("GeoZone Layer");
                //poilayer.Hide();
                this.map.AddShapeLayer(this.poilayer);
            }
            if (this.draglayer == null) {
                this.draglayer = new VEShapeLayer();
                this.draglayer.SetTitle("Drag Op Layer");
                this.map.AddShapeLayer(this.draglayer);
            }
            if (this.boundslayer == null) {
                this.boundslayer = new VEShapeLayer();
                this.boundslayer.SetTitle("Drag Op Layer");
                this.map.AddShapeLayer(this.boundslayer);
            }
            if (this.drawlayer == null) {
                this.drawlayer = new VEShapeLayer();
                this.drawlayer.SetTitle("Draw Layer");
                this.map.AddShapeLayer(this.drawlayer);
            }                          
        } 
        catch (e) {
            alert("createLayers error:" + e.description);
        }    
    
    }

    this.clearDragLayer = function() {
        this.draglayer.DeleteAllShapes();
    }
    
///////////////////////////////////////////////////////////////////////////////////////////////////             USER CONTROLS

    //provides user feedback on current zoom level
    this.addZoomLevelDisplay = function() {
        try {
            var el = document.createElement("div");
            el.style.top = "125px";
            el.id = "zoombox";
            el.className = "zoombox";
            this.zoombox = el;
            return el;
        } 
        catch (e) {
            alert("addZoomLevelDisplay error:" + e.description);
        }
    }
    
    this.setZoomLevelDisplay = function() {
        try {
            
            this.getBounds();
            this.zoombox.innerHTML = this.zoomlevel;
//                                     + "<br>" + 
//                                    this.lonwidth + "<br>" + 
//                                    this.bounds.TopLeftLatLong.Latitude + ", " + 
//                                    this.bounds.TopLeftLatLong.Longitude + "<br>" + 
//                                    this.bounds.BottomRightLatLong.Latitude + ", " + 
//                                    this.bounds.BottomRightLatLong.Longitude;
        } 
        catch (e) {
            alert("setZoomLevelDisplay error:" + e.description);
        }    
    }

///////////////////////////////////////////////////////////////////////////////////////////////////             EVENT HANDLERS
    
    this.toggleFleetVisibility = function() {
        this.fleetEnabled = !this.fleetEnabled;
        this.showFleet(this.fleetEnabled);
    }
    
    this.showFleet = function(enable) {
        if (enable == true || enable == "true")
            this.fleetlayer.Show();
        else 
            this.fleetlayer.Hide();
            
        params[3] = this.fleetEnabled;
        this.setUserValues();     
        this.onFleetVisibilityChanged.fire();
    }
    
    this.toggleInsetMapVisibility = function() {
        this.insetMapEnabled = !this.insetMapEnabled;
        this.showInsetMap(this.insetMapEnabled);
    }

        
    this.showInsetMap = function(enable) {
        if (enable  == true  || enable == "true")
            this.map.ShowMiniMap();
        else 
            this.map.HideMiniMap();
            
        params[6] =  this.insetMapEnabled;
        this.setUserValues();     
        this.onInsetMapVisibilityChanged.fire();
    }
    
    
    this.toggleTrafficVisibility = function() {
        this.trafficEnabled = !this.trafficEnabled;
        this.showTraffic(this.trafficEnabled);
    }

    this.showTraffic = function(enable) {
        if(enable  == true || enable == "true") {
            this.map.LoadTraffic(true);
            //ve control errors out on resize so disable it for the beta version
            //this.map.ShowTrafficLegend();
            //this.map.SetTrafficLegendText("Traffic Legend");
        }
        else
             this.map.ClearTraffic();
        params[7] = this.trafficEnabled;
        this.setUserValues();     
        this.onTrafficVisibilityChanged.fire();
    }
    
    this.zoomToObject = function(elementID) {
        try {
            var shape = this.map.GetShapeByID(elementID);
             if (shape != null) 
                this.map.SetCenterAndZoom(shape.GetIconAnchor(), 15);
        } 
        catch (e) {
            alert("zoomToObject error:" + e.description);
        }      
    }
    
    this.showDetail = function(elementID, shift) {
        try {
            var shape = this.map.GetShapeByID(elementID);
            if (shape != null) {
                var name = shape.GetTitle();
                var pos = name.indexOf(">") + 1;
                name = name.substr(pos, 100);
                pos = name.indexOf("<");
                name = name.substr(0, pos);
			    var layer = shape.GetShapeLayer().GetTitle();  
		        switch (layer) {
                    case "GeoZone Layer":  
                        this.showGeoZoneDetail(name, shift);
                    break;
                    case "Landmark Layer":
                        this.showLandmarkDetail(name, shift);
                    break;
                }
			}
        } 
        catch (e) {
            alert("showDetail error:" + e.description);
        }        
    }
       
    this.setZoomLevel = function(level) {
        try {           
            this.zoomlevel = level;
            this.setZoomLevelDisplay();
        } 
        catch (e) {
            alert("setZoomLevel error:" + e.description);
        }
    }
    
    this.drawBounds = function() {
        try {    
            this.getBounds();       
            if (this.showBounds) {
                this.boundslayer.DeleteAllShapes();
                var points = new Array();
                points.push(this.bounds.TopLeftLatLong);
                points.push(new VELatLong(this.bounds.TopLeftLatLong.Latitude, this.bounds.BottomRightLatLong.Longitude));
                points.push(this.bounds.BottomRightLatLong);
                points.push(new VELatLong(this.bounds.BottomRightLatLong.Latitude, this.bounds.TopLeftLatLong.Longitude));
                points.push(this.bounds.TopLeftLatLong);
                var shape = new VEShape(VEShapeType.Polyline, points);
                var sicon = "<img src='" + this.imgdir + "/trans.gif' />";
                shape.SetCustomIcon(sicon);
                shape.SetLineColor(new VEColor(255, 0, 0, 1));
                shape.SetLineWidth(1);
                shape.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
                shape.SetZIndex(800, 800);
                this.boundslayer.AddShape(shape);
            }
        } 
        catch (e) {
            alert("drawBounds error:" + e.description);
        }
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////             LAT LONG

    this.getBounds = function() {
        this.bounds = this.map.GetMapView();
    }
    
    this.isInBounds = function() {
        try {
            this.getBounds();
            this.lonwidth = this.bounds.BottomRightLatLong.Longitude - this.bounds.TopLeftLatLong.Longitude;
            this.setZoomLevelDisplay();           
            this.isLandmarksInBounds();
            this.isGeozonesInBounds();
        } 
        catch (e) {
            alert("isInBounds error:" + e.description);
        }    
    }
    
    this.isLandmarksInBounds = function() {
        this.landmarkInBounds = (this.lonwidth < this.landwidthtrigger);
        this.onLandmarksAvailableChange.fire();
    }
 
 
     this.isGeozonesInBounds = function() {
        this.geozonesInBounds = (this.lonwidth < this.geowidthtrigger);
        this.onGeozonesAvailableChange.fire();
     }
       
    
///////////////////////////////////////////////////////////////////////////////////////////////////             FLEETS

   this.addAvlShapes = function() {
        //alert (avls);
        if (avls.length == 0) 
            return;
        try {
            //<tr><td class="caption">Unit:</td><td class="value">##UNIT##</td></tr>
            var avlinfos = avls.split('|');
            //        var pointline = new Array();
            if (avlinfos.length > 500)
		        alert("Map performance may be affected with more 500 positions plotted");
            for (var i = 0; i < avlinfos.length - 1; i++) {
                if (avlinfos[i].length == 0) 
                    break;
                try {  
                    //map "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}|", (int)model, description, latitude, longitude, timestamp, address, status, speed, duration, iconName);
                    //his "{0};{1};{2};{3};{4};{5};{6};{7};{8}|",     (int)model, description, latitude, longitude, timestamp, address, messageType, speed, iconName
                    var avlinfo = avlinfos[i].split('^');
                    var point = new VELatLong(avlinfo[2], avlinfo[3]);
                    var pin = new VEShape(VEShapeType.Pushpin, point);
                    pin.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
                    var td = this.GetTimeDifferenceString(avlinfo[4]);
                    
                    if (this.maptype == "Map") 
                        var image = "" + this.imgdir + "/" + avlinfo[9]; //+".gif";
                    else 
                         var image = "" + this.imgdir + "/" + avlinfo[8]; //+".gif";
                    image = image.replace(".gif", ".png");
                    
                    var shape = "Circle";
                    if (image.indexOf("Square") > -1)
                        shape = "Square";
                    else if (image.indexOf("Diamond") > -1)
                        shape = "Diamond";


                    var color = "#00DD00";
                    
                    switch (avlinfo[6])
                    {
                        case "Untethered":
                        case "Untethered*":
                            color = "#dd00dd";
                            image = this.imgdir + "/" + "Untethered" + shape + ".png";
                        break;
                        case "Tethered":
                        case "Tethered*":
                            color = "#00dddd";
                            image = this.imgdir + "/" + "Tethered" + shape + ".png";
                        break;
                        case "Ignition ON":
                        case "Ignition ON*":
                        case "parked":
                        case "parked*":
                        case "Parked":
                        case "Parked*":
                            color = "#DD0000";
                        break;
                        case "power save":
                        case "power save*":
                        case "Power Save":
                        case "Power Save*":
                            color = "#777777";
                            image = this.imgdir + "/" + "Grey" + shape + ".png";
                            break;
                        case "Idling":
                        case "Idling*":
                        case "idling":
                        case "idling*":                           
//                            var dursplit = td.split(' ');
//                            var dur = parseInt(dursplit[0]);
//                            if (  ( td.indexOf("days") > -1 ) || ( td.indexOf("hrs") > -1 ) || ( ( td.indexOf("mins") > -1 ) &&  (dur > 15) ) )
//                            {
//                                avlinfo[6] = "Excessive Idling";
//                                color = "#FF9900";
//                                image = this.imgdir + "/" + "ExtendedIdleCircle.png";
//                            }
//                            else
//                            {
                                color = "#EEEE00";
                                image = this.imgdir + "/" + "Idle" + shape + ".png";
                            // }
                            break;
                        case "extended idling":
                        case "extended idling*":
                            color = "#FF9900";
                            image = this.imgdir + "/" + "ExtendedIdle" + shape + ".png";
                            break;
                        case "N/A":
                        case "N/A*":
                            color = "#000000";
                            image = this.imgdir + "/" + "Unknown" + shape + ".png";
                            break;                                                    
                    } 

                    if (this.maptype == "History" || this.maptype == "Playback")
                        var pinhtml = '<div class="descbody"><img alt="pin" src="##IMG##" /></div>';
                    else                       
                        var pinhtml = '<div class="descbody"><img class="imgx" alt="pin" src="##IMG##" /><div class="newdescbody"><div class="newdesc" style="background-color:'+color+';"><div class="newdesc1"><nobr>##UNIT##</nobr></div><div class="newdesc2"><nobr>##DIFF##</nobr></div></div></div></div>';
                    
                    pinhtml = pinhtml.replace("##IMG##", image);
                    pinhtml = pinhtml.replace("##UNIT##", avlinfo[1]);
                    pinhtml = pinhtml.replace("##DIFF##", td);


                    this.fleetlayer.AddShape(pin);
                    pin = this.fleetlayer.GetShapeByIndex(this.fleetlayer.GetShapeCount() - 1);                   
                    var id = "'"+pin.GetID()+"'";
                   

                    pin.SetTitle("<div class='infotitle'>" + avlinfo[1] + "</div><div class='status' style='background-color:"+color+";'><b>" + avlinfo[6] + "</b><br><i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + td +  "</i></div>");
                    
                    var avldesc = '<table cellpadding="1" cellspacing="1" border="0"><tr><td class="caption">Timestamp:</td><td class="value">##TIMESTAMP##</td></tr><tr><td class="caption">Address:</td><td class="value">##ADDRESS##</td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Zoom to location" href="javascript:void(0);" onclick="zoomMe('+id+');">Zoom to Location</a></td></tr><tr><td class="caption">Latitude:</td><td class="value">##LAT##</td></tr><tr><td class="caption">Longitude:</td><td class="value">##LON##</td></tr><tr><td class="caption">Speed:</td><td class="value">##SPEED##</td></tr><tr><td class="caption">Status:</td><td class="value">##STATUS##</td></tr><tr><td>&nbsp;</td><td style="text-align:right;"><img src="##LOGO##" /></td></tr></table>';
                    var avld = avldesc;

                    avld = avld.replace("##LOGO##", image);
                    avld = avld.replace("##UNIT##", avlinfo[1]);
                    avld = avld.replace("##LAT##", avlinfo[2]);
                    avld = avld.replace("##LON##", avlinfo[3]);

                    avld = avld.replace("##TIMESTAMP##", avlinfo[4]);
                    avld = avld.replace("##ADDRESS##", avlinfo[5]);
                    avld = avld.replace("##STATUS##", avlinfo[6]); // + "<br>" + image);
                    avld = avld.replace("##SPEED##", avlinfo[7]);

                    
                    pin.SetDescription(avld);
                    pin.SetCustomIcon(pinhtml);
                    

                    //               if(maptype=="History") 
                    //                   pointline.push(new VELatLong(avlinfo[2],avlinfo[3]));
                
                } 
                catch (e) {
                    alert("addAvlShapes shape creation error:" + e.description + "\r\n" + avlinfo);
                }
            }
       
        } 
        catch (e) {
            alert("addAvlShapes error:" + e.description + "\r\n" + avlinfos);
        }
    }

///////////////////////////////////////////////////////////////////////////////////////////////////             LANDMARKS


    this.setLandmarkData = function(value, persist) {
        if (value == null || value == "")
            return;
        if (persist)
            this.createCookie("landmarks", value);           
        this.landlayer.Show();
        this.addLandmarkShapes(value);
    }

    this.addLandmarkShapes = function(value)
    {
        try {
            var xlm = new Array();
            xlm = value.split("|");
            //{0};{1};{2};{3};{4};{5}|", (int)model, description, latitude, longitude,radius,iconName

            for (var i = 0; i < xlm.length; i++) {
                try {
                    if (xlm[i].length == 0) 
                        break;
                    var lminfo = xlm[i].split('^');
                    var point = new VELatLong(lminfo[2], lminfo[3]);
                    var pin = new VEShape(VEShapeType.Pushpin, point);
                    
                    this.landlayer.AddShape(pin);
                    pin = this.landlayer.GetShapeByIndex(this.landlayer.GetShapeCount() - 1);
                    var id = "'"+pin.GetID()+"'";
                    
                    var image = this.imgdir + "/" + lminfo[5].replace(".gif", ".ico");
                    var lmdesc = '<table cellpadding="1" cellspacing="1" border="0"><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Zoom to location" href="javascript:void(0);" onclick="zoomMe('+id+');">Zoom to Location</a></td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Zoom and show radius" href="javascript:void(0);" onclick="zoomLandmark('+id+', false);">Zoom and show radius</a></td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Show radius" href="javascript:void(0);" onclick="zoomLandmark('+id+', true);">Show radius</a></td></tr><tr><td>&nbsp;</td><td style="text-align:right;"><img src="'+image+'" /></td></tr></table>';

                    //var cicon = new VECustomIconSpecification();
                    var pinhtml = "<div style='padding-top:6px;padding-left:6px;'><img src='" + image + "' /></div>"
                    pin.SetTitle("<div class='infotitle'>" + lminfo[1] + "</div>");
                    pin.SetDescription(lmdesc);
                    pin.SetCustomIcon(pinhtml);
                    pin.SetZIndex(900, 900);
                    pin.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
                    //lmShapes.push(pin);
                } 
                catch (e) {
                    alert("addLandmarkShapes error:" + e.description + "\r\n" + xlm[i]);
                }                }
        } 
        catch (e) {
            alert("addLandmarkShapes error:" + e.description);
        }        
    
    }
    
    this.showLandmarkDetail = function(name, shift) {
        try {
		    if (this.maptype == "Landmark" || this.maptype == "Landmarks_GeoZones")
     		    var lm = landmark;
	        else
                var lm = this.readCookie("landmarks");
		    //alert(lm);
            if (lm == null || lm == "") { 
                alert("No Landmark data");
			    return;
            }
            var xlm = new Array();
            xlm = lm.split("|");
            
            var lmShapes = new Array();
            
            for (var i = 0; i < xlm.length; i++) {
                try {
                    if (xlm[i].length == 0) 
                        break;
                    var lminfo = xlm[i].split('^');
                    if (lminfo[1] == name) {
					    found = true;									
                        var radius = lminfo[4]; //50;
                        var point = new VELatLong(lminfo[2], lminfo[3]);
                        var zoom = true;
                        if (shift) 
                            zoom = false;
                        this.viewRadius(point, radius, true, zoom);
                        this.viewRadius(point, radius, false, zoom);
                        return;
                    }
                } 
                catch (e) {
                
                }                         
            }
		    if (!found)
			    alert("No Landmark data");
        } 
        catch (e) {
            alert("showLandmarkDetail error:" + e.description);
        } 								    
    }


    this.viewRadius = function(point, radius, setsize, setzoomlevel) {
        try {
            if (radius > 0) { //do some calcs for visiblity of rings...
                ring = this.createCircle(point, radius / 1000);
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
                this.draglayer.AddShape(ring);
                if (setsize && setzoomlevel) 
                    this.map.SetMapView(this.draglayer.GetBoundingRectangle());
            }
        } 
        catch (e) {
            alert("viewRadius error:" + e.description);
        }
    }
    
///////////////////////////////////////////////////////////////////////////////////////////////////             GEOZONES

    this.setGeoZoneData = function(value, persist) {
        if (value == null || value == "")
            return;
        if (persist) 
            this.createCookie("geozones", value);           
        this.geolayer.Show();
        this.addGeoZoneShapes(value);
    }

    this.addGeoZoneShapes = function(value) {
        try {
		    if (this.maptype == "GeoZone" || this.maptype == "Landmarks_GeoZones")
     		    var geozones = geoZone;
	        else {
	            if (value != "")
	                var geozones = value;
                else
                    var geozones = this.readCookie("geozones");
            }
            if (geozones == null || geozones == "") { 
                alert("No Geozone data");
			    return;
            }
            
	           
            var xlg = new Array();
            xlg = geozones.split("|");
            //"{0};{1};{2};{3};{4};{5}|", (int)model, description, numPoints, pointList, type, severity
            
            //var geoShapes = new Array();
            for (var i = 0; i < xlg.length; i++) {
                try {
                    if (xlg[i].length == 0) 
                        break;
                    var geoinfo = xlg[i].split('^');
                    var numpoints = geoinfo[2];
                    var index = 3;
                    var point = new VELatLong(geoinfo[index], geoinfo[index + 1]);
                    var pin = new VEShape(VEShapeType.Pushpin, point);
                    
                    
                    this.geolayer.AddShape(pin);
                    pin = this.geolayer.GetShapeByIndex(this.geolayer.GetShapeCount() - 1);
                    var id = "'"+pin.GetID()+"'";
                    
                   // var lmdesc = '<table cellpadding="1" cellspacing="1" border="0"><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Zoom to location" href="javascript:void(0);" onclick="zoomMe('+id+');">Zoom to Location</a></td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Zoom and show radius" href="javascript:void(0);" onclick="zoomLandmark('+id+', false);">Zoom and show radius</a></td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Show radius" href="javascript:void(0);" onclick="zoomLandmark('+id+', true);">Show radius</a></td></tr></table>';


                    index = index + (numpoints * 2);
                    var type = "";
                    if (geoinfo[index] == 2) 
                        type = "_in";
                    else 
                        if (geoinfo[index] == 1) 
                            type = "_out";
                    var cicon = new VECustomIconSpecification();
                    var image = "" + this.imgdir + "/geozone" + type + ".gif";
                    cicon.Image = image;

                    var geodesc = '<table cellpadding="0" cellspacing="0" border="0"><tr><td class="caption">GeoZone Type:</td><td class="value">##TYPE##</td></tr><tr><td class="caption">Severity:</td><td class="value">##SEVERITY##</td></tr><tr><td class="caption">Points:</td><td class="value">##POINTS##</td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Zoom to location" href="javascript:void(0);" onclick="zoomMe('+id+');">Zoom to Location</a></td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Zoom and show Geofence" href="javascript:void(0);" onclick="zoomGeozone('+id+', false);">Zoom and show Geofence</a></td></tr><tr><td class="caption">&nbsp;</td><td class="value"><a alt="Show Geofence" href="javascript:void(0);" onclick="zoomGeozone('+id+', true);">Show Geofence</a></td></tr><tr><td>&nbsp;</td><td style="text-align:right;"><img src="'+image+'" /></td></tr></table>';
                    
                    pin.SetCustomIcon(cicon);
                    var geod = geodesc;
				    var gtype = geoinfo[index++];
				    var gstype = "In/Out";
				    if (gtype == 2)
					    gstype = "In";
				    else if (gtype == 1)
					    gstype = "Out";
				    var gsev =  geoinfo[index];
				    var gssev = "No Alarm";
				    if (gsev == 1)
					    gssev = "Notify";
				    else if (gsev == 2)
					    gssev = "Warning";
				    else if (gsev == 3)
					    gssev = "Critical";
                    geod = geod.replace("##TYPE##", gstype);
                    geod = geod.replace("##SEVERITY##",gssev);
                    geod = geod.replace("##POINTS##", geoinfo[2]);
                    pin.SetTitle("<div class='infotitle'>" + geoinfo[1] + "</div>");
                    pin.SetDescription(geod);
                    pin.SetAltitudeMode(VEAltitudeMode.RelativeToGround);
                    pin.SetZIndex(875, 875);
                    //geoShapes.push(pin);
                    if (setview && i == 0) 
                        var geoinfox = geoinfo;
                        
                } 
               catch (e) {
                   //alert("Create GeoZone Shape:" + e.description + "\r\n" + xlg[i]);
                }
            }
            //this.geolayer.AddShape(geoShapes);
            
            if (this.setview) {
                this.viewGeoZone(geoinfox, true, false);
                this.viewGeoZone(geoinfox, false, false);
            }
        } 
        catch (e) {
            alert("addGeoZoneShapes error:" + e.description);
        }        
    }
        
    this.showGeoZoneDetail = function(name, shift, setview) {
        try {
            if (this.maptype == "GeoZone" || this.maptype == "Landmarks_GeoZones")
        	    var lm = geoZone;
	        else 
			    var lm = this.readCookie("geozones");

		    if (lm == null || lm == "") 
		    {
		        alert("No GeoZone data");
                return;
            }
            var xlm = new Array();
            xlm = lm.split("|");
            var lmShapes = new Array();

            for (var i = 0; i < xlm.length; i++) {
                try {
                    if (xlm[i].length == 0) 
                        break;
                    var lminfo = xlm[i].split('^');
                    if (lminfo[1] == name) {
					    found = true;
                        var zoom = true;
                        if (shift == true) 
                            zoom = false;
                        this.viewGeoZone(lminfo, true, zoom);
                        this.viewGeoZone(lminfo, false, zoom);
                        return;
                    }
                } 
                catch (e) {
                }                         
            }
		    if (!found) 
			    alert("No Geozone data");
        } 
        catch (e) {
            alert("showGeoZoneDetail error:" + e.description);
        } 								    
    }
        
    this.viewGeoZone = function(geoinfo, setsize, setzoomlevel) {
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
            this.draglayer.AddShape(shape);
            if (setsize && setzoomlevel) 
                this.map.SetMapView(this.draglayer.GetBoundingRectangle());        
        } 
        catch (e) {
            //alert("showGeoZoneDetail error:" + e.description);
        }             
    }

///////////////////////////////////////////////////////////////////////////////////////////////////             COOKIES



    this.createCookie = function(name, value){
        var expires = "";
	if(navigator.userAgent.indexOf('MSIE')>=0) {
        	var len = document.cookie.length;
        	var current = this.readCookie(name);
        	if (current == null)
            		current = "";
        	var space = len - current.length;
        	if (space + value.length > 4000)
       		{
            		//alert("Current " + name + " items cannot be saved in cache.");
            		//alert("Previously loaded " + name + " have been cleared from cache"); 
	    		//return;
	    		if (confirm("Cache space for " + name + " data has been exhausted,\r\ndo you wish to clear old values?")) {
				//do nothing
	    		} else 
				return;
        	}
		else
	   		value = value + current;
	}
        document.cookie = name + "=" + value + ";" + expires + ";path=/";
    }

    this.readCookie = function(name) {
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

    this.eraseCookie = function(name) {
        this.createCookie(name, "", -1);
    }

///////////////////////////////////////////////////////////////////////////////////////////////////             RESIZING

    //resizes VE div to parent frame size
    this.frameResize = function() {
        try {   

            var width = document.documentElement.clientWidth - 20;
            
            var height = document.documentElement.clientHeight - 20;
            document.getElementById("vex").style.width = width + "px";
            document.getElementById("vex").style.height = height + "px";

            this.adjustView();

            this.map.Resize(width, height);
        } 
        catch(e) {
            alert("frameResize error: -> " + e.description);
        }            
    }
 

    
    //rescales & centers viewport to include the current set of vehicles
    this.adjustView = function() {
        try {
            if (this.fleetlayer) {
                var count = this.fleetlayer.GetShapeCount();
                if (count == 0) {
                    this.setZoomLevel(7);
                    return;
                }
                else {
                    this.map.SetMapView(this.fleetlayer.GetBoundingRectangle());
                    if (count == 1) 
                    this.setZoomLevel(7);
                    else 
                       this.setZoomLevel(this.zoomlevel + 2);
                       
                    this.setZoomLevelDisplay();
                       
                }
            }
            else
                this.setZoomLevel(7);
            this.drawBounds();
                
        } 
        catch (e) {
            alert("adjustView error:" + e.description);
        }    
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////             FIND
    
    this.conductFind = function(searchparam) {
        try {
            if (searchparam == undefined || searchparam == null || searchparam ==  "") {
                alert("Search value cannot be blank!");
                return;
            }
            //VEMap.Find(what, where, findType, shapeLayer, startIndex, numberOfResults, showResults, createResults, useDefaultDisambiguation, setBestMapView, callback);
            results = this.map.Find(null, searchparam, null, null, 0, 20, false, false, true, true, searchResults);
        } 
        catch (e) {
            alert("conductFind error:" + e.description);
        }
    }

    this.displayResults = function(layer, resultsArray, places, hasMore, veErrorMessage) {
        //alert(resultsArray.length + "\r\n" + places.length);
        try {
            this.findlayer.DeleteAllShapes();
            for (var i = 0; i < places.length; i++) {
                var pin = new VEShape(VEShapeType.Pushpin, places[i].LatLong);
                var pinhtml = "<div style='padding-top:6px;padding-left:6px;'><img src='" + this.imgdir + "/pin.gif' /></div>"
                pin.SetTitle("<div class='infotitle'>" + places[i].Name + "</div>");
                pin.SetCustomIcon(pinhtml);
                this.findlayer.AddShape(pin);
                var id = pin.GetID();
                this.map.ShowInfoBox(id);
                
            }
        } 
        catch (e) {
            alert("searchResults error:" + e.description);
        }
    }
    
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////             USER PARAMS
    
    
    //gets user preferences saved in cookie for persistance during postbacks
    this.getUserValues = function() {
        try {
            var cookie = this.readCookie("ve");
            if (cookie == null || cookie == "") 
                this.createCookie("bsmve", params.join(","));
            else 
                params = cookie.split(",");
                    
        //  ->> params = zoom, mapstyle, labels, fleet, land, geo, inset map, traffic, radius...

            this.fleetEnabled = params[3];
            this.trafficEnabled = params[7];
            this.insetMapEnabled = params[6];
        } 
        catch (e) {
            alert("getUserValues error:" + e.description);
        }    
    }

    this.setUserValues = function() {
        try {
            this.createCookie("ve", params.join(","));       
        } 
        catch (e) {
            alert("setUserValues error:" + e.description);
        }    
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////             UTILITY

    this.GetTimeDifferenceString = function(value)
    {
        var now = new Date();
        var timestamp = new Date(value);
        var diff= now.getTime() - timestamp.getTime();
        var minutes = 1000*60;
        var hours = minutes*60;
        var days = hours*24;
        var years = days*365;
        
        var y = diff/years;
        if (y >= 1)
            return Math.floor(y) + " yrs"; 
        else
        {
            var d = diff/days;
            if (d > 3)
                return Math.floor(d) + " days"; 
            else
            {
                var h = diff/hours;
                if (h > 3)
                    return Math.floor(h) + " hrs"; 
                else
                {
                    var i = diff/minutes;
                    if (i > 2)
                        return Math.floor(i) + " mins"; 
                    else
                       return Math.floor(diff) + " secs"; 
                }
            }
        
        }       
    }
    
    this.createCircle = function(point, radius) {
        var latitude = this.toRadian(point.Latitude);
        var longitude = this.toRadian(point.Longitude);
        var d = parseFloat(radius) / this.earthRadius;
        var plotPoints = new Array();
        for (var i = 0; i <= 360;) {
            var currentRadian = this.toRadian(i);
            var latitudeRadians = Math.asin(Math.sin(latitude) * Math.cos(d) + Math.cos(latitude) * Math.sin(d) * Math.cos(currentRadian));
            var longitudeRadians = longitude + Math.atan2(Math.sin(currentRadian) * Math.sin(d) * Math.cos(latitude), Math.cos(d) - Math.sin(latitude) * Math.sin(latitudeRadians));
            plotPoints.push(new VELatLong(this.toDegrees(latitudeRadians), this.toDegrees(longitudeRadians)));
            i = i + 5;
        }
        return new VEShape(VEShapeType.Polyline, plotPoints);
    }

    this.toDegrees = function(radians) {
        return radians * 180 / Math.PI;
    }

    this.toRadian = function(value) {
        return value * (Math.PI / 180);
    }

    this.diffRadian = function(value1, value2) {
        return this.toRadian(value2) - this.toRadian(value1);
    }

    this.calculateDistance = function(latitude1, longitude1, latitude2, longitude2, radius) {
        return radius * 2 * Math.asin(Math.min(1, Math.sqrt((Math.pow(Math.sin((this.diffRadian(latitude1, latitude2)) / 2.0), 2.0) + Math.cos(this.toRadian(latitude1)) * Math.cos(this.toRadian(latitude2)) * Math.pow(Math.sin((this.diffRadian(longitude1, longitude2)) / 2.0), 2.0)))));
    }
    
    
        ///////////////////////////////////////////////////////////////////////////////////////////////////             MAP DRAW
        
        
        xOffset = 8;
        yOffset = 12;
        this.geotype = "Rectangle";
        this.geopoints = null;
        this.submitvalue = ""; 
        this.geoband = null;
        this.drawlayer = null;
        this.drawing = false;
        this.drawingNotSaved = false;
        this.key = 16;
        this.submit = false;
        this.spin = null;

    
        this.startDraw = function() {
            this.drawing = true;
            try {
                this.onDrawStarted.fire();   
                this.submitvalue = null;             
                this.drawlayer.DeleteAllShapes();
                this.geopoints = null;
                this.map.vemapcontrol.EnableGeoCommunity(true);
                setCursor(true);
                this.map.AttachEvent("onclick", mapClickDraw);
            } 
            catch (e) {
                alert("startDraw error:" + e.description);
            } 
        }
        
        this.clearDraw = function() {
            try {
                this.submitvalue = null;             
                this.geopoints = null;
                this.drawlayer.DeleteAllShapes();
                this.endDraw();
                this.onDrawCleared.fire();
            } 
            catch (err) {
                alert("clearDraw error:" + e.description);
            }
        }        
        
        this.endDraw = function() {
            this.drawing = false;
            try {
                this.onDrawEnded.fire();                
                this.map.vemapcontrol.EnableGeoCommunity(false);
                setCursor(false);
                this.map.DetachEvent("onmousemove", mapMouseMove);
                if (this.geopoints != null) {
                    if ((this.geotype == "Rectangle" && this.geopoints.length == 2) 
                        || (this.geotype == "Polygon" && this.geopoints.length > 2) 
                        || (this.geotype == "Landmark" && this.geopoints.length == 1)) 
                        this.onDrawChanged.fire();
                }
                this.map.DetachEvent("onclick", mapClickDraw);
            } 
            catch (e) {
                alert("endDraw error:" + e.description);
            }
        }        
        
        this.saveDraw = function() {
            try {
                this.onDrawSave.fire();
                var values = "";
                for (var i = 0; i < this.geopoints.length; i++) {
                    values = values + this.geopoints[i].Latitude + ",";
                    values = values + this.geopoints[i].Longitude + ",";
                }
                this.submitvalue = values.substr(0, values.length - 1);
                this.onDrawSubmit.fire(); 
            } 
            catch (e) {
                alert("saveDraw error:" + e.description);
            }
        }        
        
        this.closeDraw = function() {
            try {
                this.onDrawClose.fire();
            } 
            catch (e) {
            } // alert("SaveDraw:"+err.description);}
        }

        this.setDrawType = function(value) {
            this.geotype = value;
            this.clearDraw();
        }  
        
        this.draw = function(mapX, mapY) {
            try {
                if (!this.drawing)
                    return;
                
                var bandpoints = new Array();
                if (this.geopoints == null) {
                    this.geopoints = new Array();
                    this.map.AttachEvent("onmousemove", mapMouseMove);
                }
                if (this.geopoints.length >= 10) {
                    alert("GeoZones can contain a maximum of 10 points. \r\nPlease simplify your point set");
                    return;
                }
                var xy = this.map.PixelToLatLong(new VEPixel(mapX + xOffset, mapY + yOffset));
                this.geopoints.push(xy);
                if (this.geotype == "Landmark") {
                    var xyz = this.map.PixelToLatLong(new VEPixel(mapX + xOffset + 5, mapY + yOffset + 5));
                    var cpin = new VEShape(VEShapeType.Pushpin, new VELatLong(xyz.Latitude, xyz.Longitude));
                    var cicon = new VECustomIconSpecification();
                    cicon.ForeColor = new VEColor(255, 0, 0, 1.0);
                    cicon.TextContent = this.geopoints.length;
                    cicon.TextFont = "Arial";
                    cicon.TextSize = 9;
                    cicon.TextOffset = new VEPixel(20, 0);
                    cicon.Image = "" + this.imgdir + "/Landmark.ico";
                    cpin.SetCustomIcon(cicon);
                    this.drawlayer.AddShape(cpin);
                    this.map.DetachEvent("onmousemove", mapMouseMove);
                    this.endDraw();
                    return;
                }
                else {
                    if (this.geopoints.length == 1) {
                        bandpoints.push(xy);
                        bandpoints.push(xy);
                        this.geoband = new VEShape(VEShapeType.Polyline, bandpoints);
                        this.geoband.SetLineColor(new VEColor(255, 0, 0, 1));
                        this.geoband.SetLineWidth(1);
                        this.geoband.HideIcon();
                        this.drawlayer.AddShape(this.geoband);
                    }
                    if (this.geotype == "Rectangle") {
                        if (this.geopoints.length == 1) {
                            bandpoints.push(xy);
                            bandpoints.push(xy);
                            bandpoints.push(xy);
                            bandpoints.push(xy);
                            bandpoints.push(xy);
                            this.geoband.SetPoints(bandpoints);
                            var xyz = this.map.PixelToLatLong(new VEPixel(mapX + xOffset + 5, mapY + yOffset + 5));
                            this.spin = new VEShape(VEShapeType.Pushpin, new VELatLong(xyz.Latitude, xyz.Longitude));
                            var cicon = new VECustomIconSpecification();
                            cicon.ForeColor = new VEColor(255, 0, 0, 1.0);
                            cicon.TextContent = this.geopoints.length;
                            cicon.TextFont = "Arial";
                            cicon.TextSize = 9;
                            cicon.TextOffset = new VEPixel(20, 0);
                            cicon.Image = "" + this.imgdir + "/StartGeoZone.ico";
                            this.spin.SetCustomIcon(cicon);
                            this.drawlayer.AddShape(this.spin);
                        }
                        else 
                            if (this.geopoints.length >= 2) {
                                this.drawlayer.DeleteShape(this.spin);
                                this.endDraw();
                            }
                        
                    }
                    else 
                        if (this.geotype == "Polygon") { //Poly
                            if (this.geopoints.length == 1) {
                                var xyz = this.map.PixelToLatLong(new VEPixel(mapX + xOffset + 5, mapY + yOffset + 5));
                                this.spin = new VEShape(VEShapeType.Pushpin, new VELatLong(xyz.Latitude, xyz.Longitude));
                                var cicon = new VECustomIconSpecification();
                                cicon.ForeColor = new VEColor(255, 0, 0, 1.0);
                                cicon.TextContent = this.geopoints.length;
                                cicon.TextFont = "Arial";
                                cicon.TextSize = 9;
                                cicon.TextOffset = new VEPixel(20, 0);
                                cicon.Image = "" + this.imgdir + "/StartGeoZone.ico";
                                this.spin.SetCustomIcon(cicon);
                                this.drawlayer.AddShape(this.spin);
                            }
                            else 
                                if (this.geopoints.length > 1 && this.geopoints.length < 10) {
                                    this.drawlayer.DeleteShape(this.spin);
                                    this.geoband.SetPoints(this.geopoints);
                                    bandpoints = this.geoband.GetPoints();
                                    bandpoints.push(bandpoints[0]);
                                    this.geoband.SetPoints(bandpoints);
                                }
                                if (this.geopoints.length > 2)
                                    this.onDrawChanged.fire();

                                
                        }
                }
                if (this.geopoints.length >= 2) {
                    this.map.DetachEvent("onmousemove", mapMouseMove);
                }
                this.onDrawSetPoints.fire();
            } 
            catch (err) {
                this.map.DetachEvent("onmousemove", mapMouseMove);
            } //alert("MapMouseMove:"+mapevent.error); }   
        }      
        
        this.setMousePoint = function(mapX, mapY) {
            try {
                var bandpoints = null;
                var xy = this.map.PixelToLatLong(new VEPixel(mapX + xOffset, mapY + yOffset));
                if (this.geotype == "Rectangle") {
                    if (this.geopoints.length >= 2) 
                        return;
                    bandpoints = this.geoband.GetPoints();
                    bandpoints[1] = new VELatLong(bandpoints[1].Latitude, xy.Longitude);
                    bandpoints[2] = new VELatLong(xy.Latitude, xy.Longitude);
                    bandpoints[3] = new VELatLong(xy.Latitude, bandpoints[3].Longitude);
                    if (bandpoints != null) 
                        this.geoband.SetPoints(bandpoints);
                }
            } 
            catch (err) {
                this.map.DetachEvent("onmousemove", mapMouseMove);
            } //alert("MapMouseMove:"+mapevent.error); }        
        }
        

this.zoomMe = function(info)
{
    alert("zoom request\r\n"+info);
}

        
        ///////////////////////////////////////////////////////////////////////////////////////////////////             Default process entry point



    this.loadMap();
}

