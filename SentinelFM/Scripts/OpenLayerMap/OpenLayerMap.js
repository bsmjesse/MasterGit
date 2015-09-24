var format = 'image/png';
//var geoserver;

var map, layer, newl, markers, currentMarkerIndex = 0, arcgisLayer, histories;
var currentMarkers = new Array();
var currentHistories = new Array();
var finishedLastTimer = false;
var timer_is_on = 0;
var size, offset, icon, allVehicles, myWinID;
var infocontrols, highlightlayer, vehiclenamesLayer, vehicleDriversLayer;
var interval = '<%=sn.User.GeneralRefreshFrequency %>';
var vehicleNames = new Array();
var vehicleDrivers = new Array();
var rootpath = top.location.href.match(/^(http.+\/)[^\/]+$/)[1];
var vectors;
var searchMarker;
var searchArea;
var workorderLayer;
var selectedPopup;
var contextPopup;
var searchMarkerPopup;
var undolist = [];
var changeslist = [];
var previousPointSets;
var previousCenterPoint;
var previousRadius;
var currentEditFeature = null;
var currentEditPopup = null;
var maxVehiclesOnMap = parent.maxVehiclesOnMap;
var vehicleClickControl;
var dblclick

var geoLandmarkFeatures = [];
var visibleGeoLandmarkFeature = [];
var vehicleFeatures = [];
var workorderFeatures = [];
var strategy;
var markerstrategy;

var clusterDisabled = false;

var vehiclePopup;
var polygonPopup;
var polygonPopupHeight;
var donotclosevehiclepopups = false;
var geozoneLoaded = false;
var landmarkLoaded = false;

var ClosestVehiclesRadius = 100;
var ClosestVehiclesNumOfVehicles = 100;
//var SearchHistoryDate = 
var SearchHistoryTime = '11:00';
var dateformat = DefaultUserDate;
var datePickerFormat;

var yesterday = new Date(); yesterday.setDate(yesterday.getDate() - 1);
var yesterdayyyyy = yesterday.getFullYear().toString();
var yesterdaymm = (yesterday.getMonth() + 1).toString(); // getMonth() is zero-based
var yesterdaydd = yesterday.getDate().toString();
var SearchHistoryDate;
if (dateformat == 'mm/dd/yy') {
    SearchHistoryDate = (yesterdaymm[1] ? yesterdaymm : "0" + yesterdaymm[0]) + "/" + (yesterdaydd[1] ? yesterdaydd : "0" + yesterdaydd[0]) + "/" + yesterdayyyyy;
    datePickerFormat = 'mm/dd/yy';
}
else {
    SearchHistoryDate = (yesterdaydd[1] ? yesterdaydd : "0" + yesterdaydd[0]) + "/" + (yesterdaymm[1] ? yesterdaymm : "0" + yesterdaymm[0]) + "/" + yesterdayyyyy;
    datePickerFormat = 'dd/mm/yy';
}
//var SearchHistoryDate = (yesterdaydd[1] ? yesterdaydd : "0" + yesterdaydd[0]) + "/" + (yesterdaymm[1] ? yesterdaymm : "0" + yesterdaymm[0]) + "/" + yesterdayyyyy;
//var SearchHistoryDate = (yesterdaymm[1] ? yesterdaymm : "0" + yesterdaymm[0]) + "/" + (yesterdaydd[1] ? yesterdaydd : "0" + yesterdaydd[0]) + "/" + yesterdayyyyy;
var SearchHistoryRadius = 2000;
var SearchHistoryMinutes = 60;

var AllHistoryRecords = [];

var SearchAddressOriginHeight = 66;
var mycontrols;

var HistoryReplayTimeout;

var LandmarkDrawMode = '';
var RedrawLandmarkId = 0;
var RedrawLandmarkName = '';

OpenLayers.ProxyHost = "/proxy/?url=";

interval = interval / 2;
OpenLayers.IMAGE_RELOAD_ATTEMPTS = 3;

var JSON = JSON || {};
// implement JSON.stringify serialization
JSON.stringify = JSON.stringify || function(obj) {
    var t = typeof (obj);
    if (t != "object" || obj === null) {
        // simple data type
        if (t == "string")
            obj = '"' + obj + '"';
        return String(obj);
    } else {
        // recurse array or object
        var n, v, json = [], arr = (obj && obj.constructor == Array);
        for (n in obj) {
            v = obj[n];
            t = typeof (v);
            if (t == "string")
                v = '"' + v + '"';
            else if (t == "object" && v !== null)
                v = JSON.stringify(v);
            json.push((arr ? "" : '"' + n + '":') + String(v));
        }
        return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
    }
};

var qsParm = new Array();
function qs() {
    var query = window.location.search.substring(1);
    if (query.indexOf("Id") != -1) {
        if (query.indexOf('&') != -1) {
            var parms = query.split('&');
            for (var i = 0; i < parms.length; i++) {
                var pos = parms[i].indexOf('=');
                if (pos > 0) {
                    var key = parms[i].substring(0, pos);
                    var val = parms[i].substring(pos + 1);
                    qsParm[key] = val;
                }
            }
        }
        else {
            var pos = query.indexOf('=');
            if (pos > 0) {
                var key = query.substring(0, pos);
                var val = query.substring(pos + 1);
                qsParm[key] = val;
            }
        }
    }
}

function init() {
    /*var jsonp_url = layerURL + '?f=json&pretty=true&callback=?';
    $.getJSON(jsonp_url, function (data) {
        initMap(data);
    }
   );*/
    initMap();
}


function zoomMapAlarms(fleet) {
    try {
        if (fleet != null) {
            fleet = eval(fleet);
            if (fleet != null && fleet != "") {
                var bounds = new OpenLayers.Bounds();
                for (i = 0; i < fleet.length; i++) {
                    try {
                        for (j = 0; j < allVehicles.length; j++) {
                            if (allVehicles[j].BoxId == fleet[i].BoxId) {
                                currentVehicle = fleet[i];
                                var newLoc = transformCoords(currentVehicle.Longitude, currentVehicle.Latitude);
                                var vicon = new OpenLayers.Icon("../New20x20/" + "RedCircle.ico");
                                var viconUrl = "../New20x20/" + "RedCircle.ico";

                                if (currentVehicle.BoxId == '27492') {
                                    var size = new OpenLayers.Size(48, 97);
                                    vicon = new OpenLayers.Icon("../New20x20/" + "tower.png", size);
                                    viconUrl = "../New20x20/" + "tower.png";
                                }
                                else if (currentVehicle.ImagePath != '' && currentVehicle.ImagePath != undefined) {
                                    vicon = new OpenLayers.Icon("../images/CustomIcon/" + currentVehicle.icon);
                                    var viconUrl = "../images/CustomIcon/" + currentVehicle.icon;
                                }
                                //markers.removeMarker(currentMarkers[j]);
                                //var marker = new OpenLayers.Marker(newLoc, vicon);

                                markers.removeFeatures([currentMarkers[j]]);

                                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                                var marker = new OpenLayers.Feature.Vector(point);
                                /*var popupContent = "<h6>" + currentVehicle.Description + "<br />" +
                                    currentVehicle.StreetAddress +
                                    "<br />Time: " + currentVehicle.convertedDisplayDate +
                                    "<br />Heading: " + currentVehicle.MyHeading +
                                    "<br />Speed: " + currentVehicle.CustomSpeed +
                                    "<br />Status: " + currentVehicle.VehicleStatus +
                                    "<br /><div style='height:25px;margin-top:5px;' id='popup" + currentVehicle.LicensePlate + "'></div>";*/
                                var popupContent = getPopupContent(currentVehicle);

                                marker.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc, Driver: currentVehicle.Driver, Description: currentVehicle.vehicleDescription };
                                markers.addFeatures([marker]);

                                /*var eventObj = new Object();
                                eventObj.icon = vicon;
                                eventObj.location = newLoc;
                                eventObj.LicensePlate = currentVehicle.LicensePlate;
                                eventObj.featureHTML = "<h6>" + currentVehicle.vehicleDescription + "</h6>" +
                                    currentVehicle.StreetAddress;
                                            eventObj.featureHTML = eventObj.featureHTML +
                                    "<br />Alarm Time: " + currentVehicle.TimeCreated;
                                            eventObj.featureHTML = eventObj.featureHTML +
                                    "<br />Heading: " + currentVehicle.Heading +
                                    "<br />Speed: " + currentVehicle.Speed +
                                    "<br />Alarm Status: " + currentVehicle.AlarmState +
                                    "<br /><div style='height:25px;margin-top:5px;' id='popup" + currentVehicle.LicensePlate + "'></div>";
                                //                        marker.events.register('mouseover', eventObj, showVehiclePopups);
                                //                        marker.events.register('mouseout', eventObj, closeVehiclePopups);
                                marker.events.register('mousedown', eventObj, showVehiclePopups);
                                markers.addMarker(marker);*/
                                currentMarkers[j] = marker;
                                allVehicles[j] = currentVehicle;

                                var proj = new OpenLayers.Projection("EPSG:4326");
                                var point = new OpenLayers.LonLat(currentVehicle.Longitude, currentVehicle.Latitude);
                                point.transform(proj, map.getProjectionObject());

                                vehicleNames[j] = new OpenLayers.Feature.Vector(
                                    new OpenLayers.Geometry.Point(point.lon, point.lat), { name: currentVehicle.vehicleDescription });

                                vehicleDrivers[j] = new OpenLayers.Feature.Vector(
                                    new OpenLayers.Geometry.Point(point.lon, point.lat), { name: currentVehicle.Driver });



                            }
                        }
                        bounds.extend(transformCoords(fleet[i].Longitude, fleet[i].Latitude));
                    }
                    catch (err1) {
                        var test = err.Message;
                    }
                }

                //                if (parent.VehicleClustering) {
                //                }
                //                else if (parent.overlaysettings.vehicleDrivers) {
                //                    vehicleDriversLayer.destroyFeatures();
                //                    vehicleDriversLayer.addFeatures(vehicleDrivers);
                //                    vehicleDriversLayer.redraw();
                //                }
                //                else if (parent.overlaysettings.vehiclenames) {
                //                    vehiclenamesLayer.destroyFeatures();
                //                    vehiclenamesLayer.addFeatures(vehicleNames);
                //                    vehiclenamesLayer.redraw();
                //                }

                map.zoomToExtent(bounds, true);
                var currentZoom = map.getZoom();
                map.zoomTo(currentZoom - 2);
            }
        }
    }
    catch (err) {
    }
}

var firstTimeSetBaseLayer = true;

function mapBaseLayerChanged(event) {
    if (firstTimeSetBaseLayer) {
        firstTimeSetBaseLayer = false;
        return;
    }

    var CookieDate = new Date;
    CookieDate.setFullYear(CookieDate.getFullYear() + 1);
    document.cookie =  orgid + 'DefaultBaseLayer=' + event.layer.name + '; expires=' + CookieDate.toGMTString() + ';';    
}

function initMap(layerInfo1) {
    //if (ISSECURE) {
        //geoserver = 'https://geomap.sentinelfm.com/geoserver/wms'; //production
        //geoserver = 'https://ugeomap.sentinelfm.com/geoserver/wms'; //UAT
        //geoserver = 'https://geodev.sentinelfm.com/geoserver/wms';  //staging
        //geoserver = 'https://geomap1.sentinelfm.com/geoserver/wms'; //preproduction
        //geoserver = 'https://dgeomap.sentinelfm.com/geoserver/wms'; //DEV
    //}
    //else {
        //geoserver = 'http://geomap.sentinelfm.com:9090/geoserver/wms'; //production
        //geoserver = 'http://ugeomap.sentinelfm.com:9090/geoserver/wms'; //UAT
        //geoserver = 'http://geodev.sentinelfm.com:9090/geoserver/wms';  //staging
        //geoserver = 'http://geomap1.sentinelfm.com:9090/geoserver/wms'; //preproduction
        //geoserver = 'http://dgeomap.sentinelfm.com:9090/geoserver/wms'; //DEV
    //}

    var renderer;
    try {
        if (basemapSettings.Arcgis && !ISSECURE) {

            var layerMaxExtent = new OpenLayers.Bounds(
              layerInfo.fullExtent.xmin,
              layerInfo.fullExtent.ymin,
              layerInfo.fullExtent.xmax,
              layerInfo.fullExtent.ymax
              );

            var resolutions = [];

            for (var i = 0; i < layerInfo.tileInfo.lods.length; i++) {
                resolutions.push(layerInfo.tileInfo.lods[i].resolution);
            }
        
            arcgisLayer = new OpenLayers.Layer.ArcGISCache("Arcgis",
              "http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer",
              {
                  isBaseLayer: true,

                  resolutions: resolutions,
                  tileSize: new OpenLayers.Size(layerInfo.tileInfo.cols, layerInfo.tileInfo.rows),
                  tileOrigin: new OpenLayers.LonLat(layerInfo.tileInfo.origin.x, layerInfo.tileInfo.origin.y),
                  maxExtent: layerMaxExtent,
                  projection: 'EPSG:' + layerInfo.spatialReference.wkid
              }
            );
        }

        var maxExtent = new OpenLayers.Bounds(-20037508, -20037508, 20037508, 20037508),
      restrictedExtent = maxExtent.clone(),
      maxResolution = 126543.0339;

        var options =
      {
          projection: new OpenLayers.Projection("EPSG:900913"),
          displayProjection: new OpenLayers.Projection("EPSG:4326"),
          units: "m",
          numZoomLevels: 20,
          maxResolution: maxResolution,
          maxExtent: maxExtent,
          restrictedExtent: restrictedExtent,
          controls: [
         new OpenLayers.Control.Navigation(),
         new OpenLayers.Control.PanZoomBar(),
         //new OpenLayers.Control.PanZoom(),
         new OpenLayers.Control.MyCustomLayerSwitcher(
         {
             'ascending': true
         }
         ),
         new OpenLayers.Control.ScaleLine({ 'geodesic': true }),
         new OpenLayers.Control.MousePosition(),
         new OpenLayers.Control.KeyboardDefaults(),
		 new OpenLayers.Control.Navigation(
            {dragPanOptions: {enableKinetic: false}}
        )
         ],
          eventListeners: {
              "changebaselayer": mapBaseLayerChanged
          }
      }
      ;

        // create OSM layer
//     if (ifShowTheme1 && !ISSECURE)
//         var mapnik = new OpenLayers.Layer.OSM("Theme 1");
     if (basemapSettings.Theme1 && !ISSECURE)
         var mapnik = new OpenLayers.Layer.OSM(basemapSettings.Theme1Description);//"Theme 1");

     if (basemapSettings.Theme2 && !ISSECURE) {
            var cloudmade = new OpenLayers.Layer.CloudMade(basemapSettings.Theme2Description,//"Theme 2",
              {
                  key: '8ee2a50541944fb9bcedded5165f09d9'
              }
              );
        }

          //if (ifShowGoogleStreets) {
          if (basemapSettings.GoogleStreets) {
            var gmap = new OpenLayers.Layer.Google(
              basemapSettings.GoogleStreetsDescription, //"Google Streets",
              {
                  sphericalMercator: true
              });            
        }

          if (basemapSettings.GoogleHybrid) {
            var ghyb = new OpenLayers.Layer.Google(
          basemapSettings.GoogleHybridDescription,//"Google Hybrid",
          {
              type: google.maps.MapTypeId.HYBRID, sphericalMercator: true, numZoomLevels: 20, disableTilt: true
          }
          );
            /*var ghyb = new OpenLayers.Layer.Google(
                "Google Hybrid",
                { type: G_HYBRID_MAP, sphericalMercator: true }
            );*/
        }

        // create Virtual Earth layers
      if (basemapSettings.BingRoads && !ISSECURE) {
            var veroad = new OpenLayers.Layer.VirtualEarth(
          basemapSettings.BingRoadsDescription,//"Bing Roads",
          {
              'type': VEMapStyle.Road, sphericalMercator: true
          }
          );
        }

      if (basemapSettings.BingHybrid && !ISSECURE) {
            var vehyb = new OpenLayers.Layer.VirtualEarth(
          basemapSettings.BingHybridDescription,//"Bing Hybrid",
          {
              'type': VEMapStyle.Hybrid, sphericalMercator: true
          }
          );
        }

        //if (ifShowAerial && !ISSECURE) {
        //    var aerial = new OpenLayers.Layer.Yahoo(
        //      "Aerial",
        //      { 'type': YAHOO_MAP_HYB, sphericalMercator: true }
        //      ); 
        //}


      var layerspath = "";

      if (overlaysettings.nexradweatherradar && !ISSECURE) {
            // Weather Radar Overlay Layer
            var nexrad = new OpenLayers.Layer.WMS(
              overlaysettings.nexradweatherradarDescription,//"Nexrad Weather Radar",
              "http://mesonet.agron.iastate.edu/cgi-bin/wms/nexrad/n0r.cgi?",
              {
                  layers: "nexrad-n0r", transparent: "TRUE", format: 'image/png'
              }
              ,
              {
                  isBaseLayer: false, buffer: 0, singleTile: false, opacity: 0.5, visibility: overlaysettings.nexradweatherradarVisibility
              }
          );
        }

          if (overlaysettings.water) {              
              layerspath = "oiltrax:oiltrax_poi_water";
            var oiltrax_poi_water = new OpenLayers.Layer.WMS(overlaysettings.waterDescription,//"Water",
              geoserver,
              {
                  'layers': layerspath, transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.waterVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
          );
            $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.waterDescription + '<br />');

        }
          if (overlaysettings.wellsite) {
              layerspath = "oiltrax:oiltrax_poi_wellsite";
            var oiltrax_poi_wellsite = new OpenLayers.Layer.WMS(overlaysettings.wellsiteDescription,//"Wellsite",
              geoserver,
              {
                  'layers': layerspath, transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.wellsiteVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
            $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.wellsiteDescription + '<br />');
        }
          if (overlaysettings.batteries) {
              layerspath = "oiltrax:oiltrax_poi_batteries";
            var oiltrax_poi_batteries = new OpenLayers.Layer.WMS(overlaysettings.batteriesDescription,//"Batteries",
              geoserver,
              {
                  'layers': layerspath, transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.batteriesVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
            $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.batteriesDescription + '<br />');
        }

          if (overlaysettings.otherfacilities) {
              layerspath = "oiltrax:oiltrax_poi_otherfacilities";
            var oiltrax_poi_otherfacilities = new OpenLayers.Layer.WMS(overlaysettings.otherfacilitiesDescription,//"Other Facilities",
              geoserver,
              {
                  'layers': layerspath, transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.otherfacilitiesVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
            $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.otherfacilitiesDescription + '<br />');
        }

          if (overlaysettings.sasktellsd) {
              layerspath = "oiltrax:sask_lsd";
            var oiltrax_sasklsd = new OpenLayers.Layer.WMS(overlaysettings.sasktellsdDescription,//"Sasktel LSD",
              geoserver,
              {
                  'layers': layerspath, transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.sasktellsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
           );
            $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.sasktellsdDescription + '<br />');
        }
          if (overlaysettings.manbslsd) {
              layerspath = "oiltrax:manbc_lsd";
            var oiltrax_manbclsd = new OpenLayers.Layer.WMS(overlaysettings.manbslsdDescription,//"Man-BC LSD",
              geoserver,
              {
                  'layers': layerspath, transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.manbslsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
          );
            $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.manbslsdDescription + '<br />');
        }
          if (overlaysettings.albertalsd) {
              layerspath = "oiltrax:alberta_lsd";
            var oiltrax_albertalsd = new OpenLayers.Layer.WMS(overlaysettings.albertalsdDescription,//"Alberta LSD",
              geoserver,
              {
                  'layers': 'oiltrax:alberta_lsd', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.albertalsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
           );
            $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.albertalsdDescription + '<br />');
        }

          if (overlaysettings.trails) {
            var oiltrax_trails = new OpenLayers.Layer.WMS(overlaysettings.trailsDescription,//"Trails",
              geoserver,
              {
                  'layers': 'oiltrax:trail', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.trailsVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
          );
        }
          if (overlaysettings.additionalroads) {
            var oiltrax_goat = new OpenLayers.Layer.WMS(overlaysettings.additionalroadsDescription,//"Additional Roads",
              geoserver,
              {
                  'layers': 'oiltrax:goat', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.additionalroadsVisibility, opacity: 1, singleTile: true, ratio: 1
              }
           );
        }
          if (overlaysettings.additionalroads2) {
            var oiltrax_goat2 = new OpenLayers.Layer.WMS(overlaysettings.additionalroads2Description,//"Additional Roads 2",
              geoserver,
              {
                  'layers': 'oiltrax:goat2', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.additionalroads2Visibility, opacity: 1, singleTile: true, ratio: 1
              }
          );
          }

          if (overlaysettings.CNRailMilePosts) {
              var cnrail_mileposts = new OpenLayers.Layer.WMS(overlaysettings.CNRailMilePostsDescription,//"CNRail MilePost",
                geoserver,
                { 'layers': 'cnrail:cnrail_mileposts', transparent: true, format: format },
                { visibility: overlaysettings.CNRailMilePostsVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
          }

          if (overlaysettings.CNRailMaintenanceOfWay) {
              var cnrail_maintenanceofway = new OpenLayers.Layer.WMS(overlaysettings.CNRailMaintenanceOfWayDescription,//"CNRail Maintenance Of Way",
                geoserver,
                { 'layers': 'cnrail:cnrail_mow', transparent: true, format: format },
                { visibility: overlaysettings.CNRailMaintenanceOfWayVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );

          }

          if (overlaysettings.UPRailNetwork) {
              var uprail_main = new OpenLayers.Layer.WMS(overlaysettings.UPRailNetworkDescription,//"UPRail Network",
                geoserver,
                { 'layers': 'uprail:up_main', transparent: true, format: format },
                { visibility: overlaysettings.UPRailNetworkVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
          }

          if (overlaysettings.UPRailMilePosts) {
              var uprail_mileposts = new OpenLayers.Layer.WMS(overlaysettings.UPRailMilePostsDescription,//"UPRail MilePost",
                geoserver,
                { 'layers': 'uprail:uprail_mileposts', transparent: true, format: format },
                { visibility: overlaysettings.UPRailMilePostsVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
          }

          if (overlaysettings.UPRailRightOfWay) {
              var UPRailRightOfWay = new OpenLayers.Layer.WMS(overlaysettings.UPRailRightOfWayDescription,//"UPRail Right Of Way",
                geoserver,
                { 'layers': 'uprail:right_of_way', transparent: true, format: format },
                { visibility: overlaysettings.UPRailRightOfWayVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );
          }

          if (overlaysettings.UPRailPoly) {
              var UPRailPoly = new OpenLayers.Layer.WMS(overlaysettings.UPRailPolyDescription,//"UPRail Poly",
                geoserver,
                { 'layers': 'uprail:uprail_polygon', transparent: true, format: format },
                { visibility: overlaysettings.UPRailPolyVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );
          }

          if (overlaysettings.OFSCTrails) {
              //OFSC Trails layer
              var ofsc_trails = new OpenLayers.Layer.WMS(overlaysettings.OFSCTrailsDescription,//"OFSC Trails",
                geoserver,
                { 'layers': 'ofsc:OFSC_trails', transparent: true, format: format },
                { visibility: overlaysettings.OFSCTrailsVisibility, opacity: 1.0, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.CityWards) {
              layerspath = "toronto:city_wards";
              var toronto_wards = new OpenLayers.Layer.WMS(overlaysettings.CityWardsDescription,//"City Wards",
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.CityWardsVisibility, opacity: 1.0, singleTile: true, ratio: 1 }
            );
              $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.CityWardsDescription + '<br />');
          }

          if (overlaysettings.TDSBSchools) {
              layerspath = "toronto:TDSBSchool";
              var toronto_tdsbschools = new OpenLayers.Layer.WMS(overlaysettings.TDSBSchoolsDescription,//"TDSB Schools",
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.TDSBSchoolsVisibility, opacity: 1.0, singleTile: true, ratio: 1 }
            );
              $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.TDSBSchoolsDescription + '<br />');
          }

          if (overlaysettings.BNSFRailway) {
              //BNSF Layers
              var bnsf_railway = new OpenLayers.Layer.WMS(overlaysettings.BNSFRailwayDescription,//"BNSF Railway",
                geoserver,
                { 'layers': 'bnsf:railway', transparent: true, format: format },
                { visibility: overlaysettings.BNSFRailwayVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.MiltonParksAndFacilities) {
              //Milton Layers
              layerspath = "milton:PARKS_FACILITY1";
              var MiltonParksAndFacilities = new OpenLayers.Layer.WMS(overlaysettings.MiltonParksAndFacilitiesDescription,//"Milton Parks and Facilities",
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.MiltonParksAndFacilitiesVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
              $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.MiltonParksAndFacilitiesDescription + '<br />');
          }

          if (overlaysettings.MiltonAddressPoints) {
              //Milton Layers
              layerspath = "milton:MILTON_ADDRESS_PTS";
              var MiltonAddressPoints = new OpenLayers.Layer.WMS(overlaysettings.MiltonAddressPointsDescription,//"Milton Address Points",
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.MiltonAddressPointsVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
              $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.MiltonAddressPointsDescription + '<br />');
          }



          if (overlaysettings.MiltonRoads) {
              //Milton Layers
              layerspath = "milton:Milton_Roads";
              var MiltonRoads = new OpenLayers.Layer.WMS(overlaysettings.MiltonRoadsDescription,//"Milton Roads",
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.MiltonRoadsVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
              $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.MiltonRoadsDescription + '<br />');
          }

          if (overlaysettings.MiltonRail) {
              //Milton Layers
              var MiltonRail = new OpenLayers.Layer.WMS(overlaysettings.MiltonRailDescription,//"Milton Rail",
                geoserver,
                { 'layers': 'milton:RAIL', transparent: true, format: format },
                { visibility: overlaysettings.MiltonRailVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.NewBrunswicLocalTrails) {
              var NewBrunswicLocalTrails = new OpenLayers.Layer.WMS(overlaysettings.NewBrunswicLocalTrailsDescription,//"New Brunswic Local Trails",
                geoserver,
                { 'layers': 'nbfsc:LOCAL_TRAILS_ONLY', transparent: true, format: format },
                { visibility: overlaysettings.NewBrunswicLocalTrailsVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.NewBrunswicProvincialTrails) {
              var NewBrunswicProvincialTrails = new OpenLayers.Layer.WMS(overlaysettings.NewBrunswicProvincialTrailsDescription,//"New Brunswic Provincial Trails",
                geoserver,
                { 'layers': 'nbfsc:PROVINCIAL_TRAILS_ONLY', transparent: true, format: format },
                { visibility: overlaysettings.NewBrunswicProvincialTrailsVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.TrailsNumbers) {
              var TrailsNumbers = new OpenLayers.Layer.WMS(overlaysettings.TrailsNumbersDescription,//"Trails Numbers",
                geoserver,
                { 'layers': 'fcmq:TrailsNumbers_15112013', transparent: true, format: format },
                { visibility: overlaysettings.TrailsNumbersVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.TrailsTQ) {
              var TrailsTQ = new OpenLayers.Layer.WMS(overlaysettings.TrailsTQDescription,//"Trails TQ",
                geoserver,
                { 'layers': 'fcmq:TrailsTQ_15112013', transparent: true, format: format },
                { visibility: overlaysettings.TrailsTQVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.lirr_stations) {
              layerspath = "Lirr:lirr_stations";
              var lirr_stations = new OpenLayers.Layer.WMS(overlaysettings.lirr_stationsDescription,//"LIRR Stations",
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.lirr_stationsVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
              $('#searchAddressOptionsOverlays').append('<input type="checkbox" name="SearchAddressBy[]" value="' + layerspath + '" /> ' + overlaysettings.lirr_stationsDescription + '<br />');
          }

          if (overlaysettings.lirr_tracks) {
              var lirr_tracks = new OpenLayers.Layer.WMS(overlaysettings.lirr_tracksDescription,//"LIRR Tracks",
                geoserver,
                { 'layers': 'Lirr:lirr_tracks', transparent: true, format: format },
                { visibility: overlaysettings.lirr_tracksVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.postedspeedreports) {
              var postedspeedreports = new OpenLayers.Layer.WMS(overlaysettings.postedspeedreportsDescription, //"Posted Speed Reports",
                //'http://geomap1.sentinelfm.com:9090/geoserver/wms',
                geoserver,
                { 'layers': 'topp:bsm-dispute', transparent: true, format: format, viewparams: 'organizationid:' + orgid },
                { visibility: overlaysettings.postedspeedreportsVisibility, opacity: 0.7, singleTile: true, ratio: 1 }                
            );
          }

          if (overlaysettings.Limitesclubs) {
              layerspath = "fcmq:Club_motoneige";
              var Limitesclubs = new OpenLayers.Layer.WMS(overlaysettings.LimitesclubsDescription,//"Limites Clubs",
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.LimitesclubsVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );              
          }

          if (overlaysettings.bnsfoffroad) {
              layerspath = "bnsf:bnsf_offroad_2015";
              var bnsfoffroad = new OpenLayers.Layer.WMS(overlaysettings.bnsfoffroadDescription,
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.bnsfoffroadVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }
          if (overlaysettings.FCMQSentiersNOReg) {
              layerspath = "fcmq:FCMQ_Sentiers_NO_reg";
              var FCMQSentiersNOReg = new OpenLayers.Layer.WMS(overlaysettings.FCMQSentiersNORegDescription,
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.FCMQSentiersNORegVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.Fermes) {
              layerspath = "fcmq:Fermes";
              var Fermes = new OpenLayers.Layer.WMS(overlaysettings.FermesDescription,
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.FermesVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

          if (overlaysettings.VoiesNavigables) {
              layerspath = "fcmq:Voies_navigables";
              var VoiesNavigables = new OpenLayers.Layer.WMS(overlaysettings.VoiesNavigablesDescription,
                geoserver,
                { 'layers': layerspath, transparent: true, format: format },
                { visibility: overlaysettings.VoiesNavigablesVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }


          if ($('#searchAddressOptionsOverlays').html().trim() == '')
              $('#searchAddressOptionsOverlays').append('<br />You don\'t have searchable overlays.');
        map = new OpenLayers.Map('map', options);
        var apiKey = "AjfXbh4yW0IPCW0jQaXRxfZwLdAIGJjXk0v_OhoZo2gs6rygaryV1aCihwPQ4F9l";

        highlightLayer = new OpenLayers.Layer.Vector("Highlighted Features",
      {
          displayInLayerSwitcher: false,
          isBaseLayer: false
      }
      );

        infoControls =
      {
          click: new OpenLayers.Control.WMSGetFeatureInfo(
         {
             url: geoserver,
             title: WMSInfoControlTitle, //res //title: 'Identify features by clicking',
             layers: [overlaysettings.water ? oiltrax_poi_water : ""],
             queryVisible: true
         }
         ),
          hover: new OpenLayers.Control.WMSGetFeatureInfo(
         {
             url: geoserver,
             title: WMSInfoControlTitle, //res //title: 'Identify features by clicking',
             layers: [overlaysettings.water ? oiltrax_poi_water : ""],
             hover: true,
             queryVisible: true
         }
         )
      }
        ;


        //     if (parent.VehicleClustering) {
        //     }
        //     else if (overlaysettings.vehicleDrivers) {
        //         vehicleDriversLayer = new OpenLayers.Layer.Vector("Vehicle Drivers", {
        //             styleMap: new OpenLayers.StyleMap({ 'default': {
        //                 label: "${name}",
        //                 labelPadding: "3px",
        //                 labelBackgroundColor: "#ffffff",
        //                 labelBorderColor: "#cfcfcf",
        //                 labelBorderSize: "1px",
        //                 fontColor: "black",
        //                 fontSize: "12px",
        //                 fontFamily: "Courier New, monospace",
        //                 fontWeight: "bold",
        //                 labelYOffset: "-17"
        //             }
        //             }),
        //         visibility: overlaysettings.vehicleDriversVisibility
        //         });
        //     }
        //     else if (overlaysettings.vehiclenames) {
        //         vehiclenamesLayer = new OpenLayers.Layer.Vector("Vehicle Names", {
        //                styleMap: new OpenLayers.StyleMap({ 'default': {
        //                    label: "${name}",
        //                    labelPadding: "3px",
        //                    labelBackgroundColor: "#ffffff",
        //                    labelBorderColor: "#cfcfcf",
        //                    labelBorderSize: "1px",
        //                    fontColor: "black",
        //                    fontSize: "12px",
        //                    fontFamily: "Courier New, monospace",
        //                    fontWeight: "bold",
        //                    labelYOffset: "-17"
        //                }
        //                }),
        //            visibility: overlaysettings.vehiclenamesVisibility
        //            });
        //        }

        // allow testing of specific renderers via "?renderer=Canvas", etc
        renderer = OpenLayers.Util.getParameters(window.location.href).renderer;
        renderer = (renderer) ? [renderer] : OpenLayers.Layer.Vector.prototype.renderers;
        //renderer = (renderer) ? [renderer] : ['Canvas', 'SVG', 'VML'];

        var style = new OpenLayers.Style({
            pointRadius: "${radius}",
            //fillColor: "#ffcc66",
            fillColor: "${fillcolor}",
            fillOpacity: "${opacity}",
            strokeColor: "#cc6633",
            strokeWidth: "${width}",
            strokeOpacity: 0.8,
            strokeDashstyle: "${strokeDashstyle}",
            title: '${title}'
        }, {
            context: {
                width: function (feature) {
                    return (feature.cluster) ? 2 : 1;
                },
                radius: function (feature) {
                    var pix = 5;
                    if (feature.cluster) {
                        pix = Math.min(feature.attributes.count, 7) - 1;
                    }
                    return pix;
                },
                fillcolor: function (feature) {
                    return (feature.cluster) ? '#cc6633' : '#ffcc66';
                },
                opacity: function (feature) {
                    return feature.cluster ? 0.8 : 0.3;
                },
                strokeDashstyle: function (feature) {
                    return feature.fid == null ? "solid" : (feature.fid.split(":::")[0] == "Geozone" ? "solid" : "dash");
                },
                title: function (feature) {
                    if (feature.cluster || feature.fid == null)
                        return '';

                    if (feature.fid.split(":::").length < 2)
                        return '';                    
                    
                    return feature.fid.split(":::")[1];
                }
            }
        });
        strategy = new OpenLayers.Strategy.Cluster();
        strategy.distance = 40;
        strategy.threshold = 3;

        vectors = new OpenLayers.Layer.Vector("GeoAsset Layer", {
            strategies: [strategy],
            styleMap: new OpenLayers.StyleMap({
                "default": style,
                "select": {
                    fillColor: "#8aeeef",
                    strokeColor: "#32a8a9"
                }
            }),
            renderers: renderer, visibility: overlaysettings.geoassetVisibility
        });

        var routeGeometryStyle = new OpenLayers.Style({
            fillColor: "#57007F",
            strokeColor: "#470068",
            fillOpacity: "0.8",
            strokeWidth: 1,
            externalGraphic: "../New20x20/alarmRedCircleFlash.gif",
            graphicWidth: "20",
            graphicHeight: "20",
            label: "${name}",
            //label: "test",
            labelPadding: "3px",
            labelBackgroundColor: "#ffffff",
            labelBorderColor: "#cfcfcf",
            labelBorderSize: "1px",
            fontColor: "black",
            fontSize: "12px",
            fontFamily: "Courier New, monospace",
            fontWeight: "bold",
            labelYOffset: "-22"
        }, {
            context: {
                name: function (feature) {
                    if (feature.attributes && feature.attributes.Description)
                        return feature.attributes.Description;
                    else {
                        return "";
                    }
                }
            }
        });

        routeGeometry = new OpenLayers.Layer.Vector("Route Geometry Layer", {
            displayInLayerSwitcher: false,
            styleMap: new OpenLayers.StyleMap({
                //"default": style,
                "default": routeGeometryStyle
            }),
            renderers: renderer, visibility: true
        });

        var searchAreaStyle = new OpenLayers.Style({
            //pointRadius: "${radius}",
            //fillColor: "#ffcc66",
            fillColor: "#8AFF38",
            fillOpacity: "0.5",
            strokeColor: "#1CFF60",
            strokeWidth: "1",
            strokeOpacity: 0.8,
            strokeDashstyle: "solid"
        });

        searchArea = new OpenLayers.Layer.Vector("Search Area", {
            //strategies: [strategy],
            displayInLayerSwitcher: false,
            styleMap: new OpenLayers.StyleMap({
                "default": searchAreaStyle,
                "select": {
                    fillColor: "#8aeeef",
                    strokeColor: "#32a8a9"
                }
            }),
            renderers: renderer, visibility: true
        });

        var searchMarkersStyle = new OpenLayers.Style({
            externalGraphic: "../images/locator.png",
            graphicWidth: 40,
            graphicHeight: 40
            , graphicYOffset: -33
            , graphicXOffset: -10
        });

        searchMarker = new OpenLayers.Layer.Vector("Search Layer", {
            //strategies: [strategy],
            displayInLayerSwitcher: false,
            styleMap: new OpenLayers.StyleMap({
                "default": searchMarkersStyle,
                "select": {
                    fillColor: "#8aeeef",
                    strokeColor: "#32a8a9"
                }
            }),
            renderers: renderer
        });

        var workOrderStyle = new OpenLayers.Style({
            pointRadius: "${radius}",
            //fillColor: "#ffcc66",
            fillColor: "#04B4AE",
            fillOpacity: "0.5",
            strokeColor: "#0B4C5F",
            strokeWidth: "1",
            strokeOpacity: 0.8,
            strokeDashstyle: "solid"
        }, {
            context: {
                radius: function (feature) {
                    var pix = 2;
                    if (feature.cluster) {
                        pix = Math.min(feature.attributes.count, 7) - 1;
                    }
                    return feature.radius;
                }
            }
        });
        workorderLayer = new OpenLayers.Layer.Vector("Workorder Layer", {
            styleMap: new OpenLayers.StyleMap({
                "default": workOrderStyle,
                "select": {
                    fillColor: "#04B4AE",
                    strokeColor: "#0B4C5F"
                }
            }),
            renderers: renderer, visibility: true
        });

        //var trafficLayer = new google.maps.TrafficLayer();
        //gmap.id = 'gmap';

        if (basemapSettings.Navteq) {
            var Navteq = new OpenLayers.Layer.Navteq(basemapSettings.NavteqDescription, /*"Navteq",*/"normal.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
            map.addLayer(Navteq);
        }

        if (basemapSettings.NavteqHybrid) {
            var NavteqHybrid = new OpenLayers.Layer.Navteq(basemapSettings.NavteqHybridDescription, /*"Navteq Hybrid",*/"hybrid.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
            map.addLayer(NavteqHybrid);
        }

        if (basemapSettings.Theme1 && !ISSECURE)
            map.addLayer(mapnik);
        if (basemapSettings.Theme2 && !ISSECURE)
            map.addLayer(cloudmade);
        if (basemapSettings.Arcgis && !ISSECURE)
            map.addLayer(arcgisLayer);
        if (basemapSettings.GoogleStreets)
            map.addLayer(gmap);
        if (basemapSettings.GoogleHybrid)
            map.addLayer(ghyb);
        if (basemapSettings.BingRoads && !ISSECURE)
            map.addLayer(veroad);
        if (basemapSettings.BingHybrid && !ISSECURE)
            map.addLayer(vehyb);


        //if (map.layers.length > 1 && map.layers[0].name.toLowerCase().indexOf("google") != -1) {            
        //    map.setBaseLayer(map.layers[1]);
        //    map.setBaseLayer(map.layers[0]);
        //}
        
        var defaultBaseLayerName = readCookie(orgid + 'DefaultBaseLayer');
        var defaultBaseLayer = map.getLayersByName(defaultBaseLayerName);
        if(defaultBaseLayer.length>0)
        {
            map.setBaseLayer(defaultBaseLayer[0]);
        }
        
        map.addLayer(vectors);
        map.addLayer(searchMarker);
        map.addLayer(searchArea);

        //map.addLayer(workorderLayer);

        //trafficLayer.setMap(map.getLayer('gmap').mapObject);
        //Moved By Rohit
        if (DefaultMapCenter != null) {
            if (DefaultMapZoomLevel != null)
                map.setCenter(DefaultMapCenter, DefaultMapZoomLevel);
            else
                map.setCenter(DefaultMapCenter, 8);
        }
        else
            map.setCenter(transformCoords(-100.42, 43.73), 8);

        if (!map.getCenter()) {
            map.zoomToMaxExtent();
            map.setCenter(transformCoords(-100.42, 43.73), 8);
        }
        if (orgid == 727) {
            var fcmq_tq = new OpenLayers.Layer.WMS("TQ (blue)",
            geoserver,
            { 'layers': 'fcmq:TQ_Trails', transparent: true, format: format },
            { visibility: true, opacity: 1.0, singleTile: true, ratio: 1 }
        );
            var fcmq_local = new OpenLayers.Layer.WMS("Local (orange)",
            geoserver,
            { 'layers': 'fcmq:Local_Trails', transparent: true, format: format },
            { visibility: true, opacity: 1.0, singleTile: true, ratio: 1 }
        );
            var fcmq_localn = new OpenLayers.Layer.WMS("LocalN (pink)",
            geoserver,
            { 'layers': 'fcmq:LocalN_Trails', transparent: true, format: format },
            { visibility: true, opacity: 1.0, singleTile: true, ratio: 1 }
        );

            var fcmq_reg = new OpenLayers.Layer.WMS("Reg (green)",
            geoserver,
            { 'layers': 'fcmq:REG_Trails', transparent: true, format: format },
            { visibility: true, opacity: 1.0, singleTile: true, ratio: 1 }
        );

            /*var fcmq_nsentiers = new OpenLayers.Layer.WMS("Trails No",
         geoserver,
         {
             'layers': 'fcmq:nsentiers_4326', transparent: true, format: format
         }
         ,
         {
             visibility: true, opacity: 1.0, singleTile: true, ratio: 1
         }
         );
            var fcmq_nsentiers_reg = new OpenLayers.Layer.WMS("Trails No",
         geoserver,
         {
             'layers': 'fcmq:NSentiers_REG', transparent: true, format: format
         }
         ,
         {
             visibility: true, opacity: 1.0, singleTile: true, ratio: 1
         }
         );
            var fcmq_nsentiers_tq = new OpenLayers.Layer.WMS("Trails No",
         geoserver,
         {
             'layers': 'fcmq:NSentiers_TQ', transparent: true, format: format
         }
         ,
         {
             visibility: true, opacity: 1.0, singleTile: true, ratio: 1
         }
         );*/
            var fcmq_sentiers = new OpenLayers.Layer.WMS("FCMQ Sentiers NO TQ",
            geoserver,
            { 'layers': 'fcmq:FCMQ_Sentiers_NO_TQ', transparent: true, format: format },
            { visibility: true, opacity: 1.0, singleTile: true, ratio: 1 }
        );

            map.addLayers([fcmq_tq,
         fcmq_local,
         fcmq_localn,
         fcmq_reg,
         fcmq_sentiers]);

            /*if (parent.overlaysettings.vehiclenames) {
            map.addLayer(vehiclenamesLayer);
            }*/
        }
        else {
            if (overlaysettings.water) {
                map.addLayer(oiltrax_poi_water);
            }
            if (overlaysettings.wellsite) {
                map.addLayer(oiltrax_poi_wellsite);
            }
            if (overlaysettings.batteries) {
                map.addLayer(oiltrax_poi_batteries);
            }
            if (overlaysettings.otherfacilities) {
                map.addLayer(oiltrax_poi_otherfacilities);
            }
            if (overlaysettings.sasktellsd) {
                map.addLayer(oiltrax_sasklsd);
            }
            if (overlaysettings.albertalsd) {
                map.addLayer(oiltrax_albertalsd);
            }
            if (overlaysettings.manbslsd) {
                map.addLayer(oiltrax_manbclsd);
            }
            if (overlaysettings.trails) {
                map.addLayer(oiltrax_trails);
            }
            if (overlaysettings.additionalroads) {
                map.addLayer(oiltrax_goat);
            }
            if (overlaysettings.additionalroads2) {
                map.addLayer(oiltrax_goat2);
            }
            if (overlaysettings.nexradweatherradar && !ISSECURE) {
                map.addLayer(nexrad);
            }
            if (overlaysettings.CNRailMilePosts) {
                map.addLayer(cnrail_mileposts);
            }
            if (overlaysettings.CNRailMaintenanceOfWay) {
                map.addLayer(cnrail_maintenanceofway);
            }

            if (overlaysettings.UPRailNetwork) {
                map.addLayer(uprail_main);                
            }

            if (overlaysettings.UPRailMilePosts) {
                map.addLayer(uprail_mileposts);
            }

            if (overlaysettings.UPRailRightOfWay) {
                map.addLayer(UPRailRightOfWay);
            }

            if (overlaysettings.UPRailPoly) {
                map.addLayer(UPRailPoly);
            }

            if (overlaysettings.OFSCTrails) {
              map.addLayer(ofsc_trails);
            }

            if (overlaysettings.CityWards) {
                map.addLayer(toronto_wards);                
            }

            if (overlaysettings.TDSBSchools) {
                map.addLayer(toronto_tdsbschools);
            }

            if (overlaysettings.BNSFRailway) {
                map.addLayer(bnsf_railway);
            }

            if (overlaysettings.MiltonParksAndFacilities) {
                map.addLayer(MiltonParksAndFacilities);
            }

            if (overlaysettings.MiltonAddressPoints) {
                map.addLayer(MiltonAddressPoints);
            }

            if (overlaysettings.MiltonRoads) {
                map.addLayer(MiltonRoads);
            }

            if (overlaysettings.MiltonRail) {
                map.addLayer(MiltonRail);
            }

            if (overlaysettings.NewBrunswicLocalTrails) {
                map.addLayer(NewBrunswicLocalTrails);
            }

            if (overlaysettings.NewBrunswicProvincialTrails) {
                map.addLayer(NewBrunswicProvincialTrails);
            }



            map.addLayers([
            // oiltrax_polygon,
         highlightLayer]);

            /*if (overlaysettings.vehiclenames) {
            map.addLayer(vehiclenamesLayer);
            }*/
        }

        if (overlaysettings.TrailsNumbers) {
            map.addLayer(TrailsNumbers);
        }

        if (overlaysettings.TrailsTQ) {
            map.addLayer(TrailsTQ);
        }

        //if (overlaysettings.lirr_stations && !ISSECURE) {
        if (overlaysettings.lirr_stations) {
            map.addLayer(lirr_stations);
        }

        //if (overlaysettings.lirr_tracks && !ISSECURE) {
        if (overlaysettings.lirr_tracks) {
            map.addLayer(lirr_tracks);
        }
        if (overlaysettings.postedspeedreports) {
            map.addLayer(postedspeedreports);
        }

        if (overlaysettings.Limitesclubs) {
            map.addLayer(Limitesclubs);
        }

        if (overlaysettings.bnsfoffroad) {
            map.addLayer(bnsfoffroad);
        }
        if (overlaysettings.FCMQSentiersNOReg) {
            map.addLayer(FCMQSentiersNOReg);
        }

        if (overlaysettings.Fermes) {
            map.addLayer(Fermes);
        }

        if (overlaysettings.VoiesNavigables) {
            map.addLayer(VoiesNavigables);
        }
        
        for (var i in infoControls) {
            infoControls[i].events.register("getfeatureinfo", this, showInfo);
            map.addControl(infoControls[i]);
        }
        //alert(parent.VehicleClustering);
        markerstrategy = new OpenLayers.Strategy.Cluster();
        markerstrategy.distance = parent.VehicleClusteringDistance; // 40;
        markerstrategy.threshold = parent.VehicleClusteringThreshold; // 5;

        // allow testing of specific renderers via "?renderer=Canvas", etc
        markersRenderer = OpenLayers.Util.getParameters(window.location.href).renderer;
        markersRenderer = (markersRenderer) ? [markersRenderer] : OpenLayers.Layer.Vector.prototype.renderers;

        var markersStyle = new OpenLayers.Style({
            externalGraphic: "${icon}",
            graphicWidth: "${graphicWidth}",
            graphicHeight: "${graphicHeight}",
            label: "${name}",
            labelPadding: "3px",
            labelBackgroundColor: "#ffffff",
            labelBorderColor: "#cfcfcf",
            labelBorderSize: "1px",
            fontColor: "black",
            fontSize: "12px",
            fontFamily: "Courier New, monospace",
            fontWeight: "bold",
            labelYOffset: "-22"
        }, {
            context: {
                icon: function (feature) {
                    if (feature.cluster)
                        return "../images/vehicleclustering.png";
                    else
                        return feature.attributes.icon;
                },
                graphicWidth: function (feature) {
                    if (feature.cluster)
                        return 18;
                    else
                        return 20;
                },
                graphicHeight: function (feature) {
                    if (feature.cluster)
                        return 15;
                    else
                        return 20;
                },
                name: function (feature) {
                    if (!parent.ifShowClusteredVehicleLabel) {
                        return "";
                    }
                    else {
                        if (overlaysettings.vehicleDrivers && $.trim(feature.attributes.Driver) != '' && typeof feature.attributes.Driver != 'object') {
                            return feature.attributes.Driver;
                        }
                        else if (overlaysettings.vehiclenames) {
                            return feature.attributes.Description;
                        }
                        else {
                            return "";
                        }
                    }

                }
            }
        });

        var clusteringMarkersStyle = new OpenLayers.Style({
            externalGraphic: "${icon}",
            graphicWidth: "${graphicWidth}",
            graphicHeight: "${graphicHeight}",
            label: "${name}",
            labelPadding: "3px",
            labelBackgroundColor: "#ffffff",
            labelBorderColor: "#cfcfcf",
            labelBorderSize: "1px",
            fontColor: "black",
            fontSize: "12px",
            fontFamily: "Courier New, monospace",
            fontWeight: "bold",
            labelYOffset: "-22"
        }, {
            context: {
                icon: function (feature) {
                    if (feature.cluster)
                        return "../images/vehicleclustering.png";
                    else
                        return feature.attributes.icon;
                },
                graphicWidth: function (feature) {
                    if (feature.cluster)
                        return 32;
                    else
                        return 20;
                },
                graphicHeight: function (feature) {
                    if (feature.cluster)
                        return 32;
                    else
                        return 20;
                },
                name: function (feature) {
                    if (feature.cluster) {
                        return "";
                        //return feature.attributes.count;
                    }
                    else if (!parent.ifShowClusteredVehicleLabel) {
                        return "";
                    }
                    else {
                        if (overlaysettings.vehicleDrivers && $.trim(feature.attributes.Driver) != '' && typeof feature.attributes.Driver != 'object') {
                            return feature.attributes.Driver;
                        }
                        else if (overlaysettings.vehiclenames) {
                            return feature.attributes.Description;
                        }
                        else {
                            return "";
                        }
                    }
                } /*,
                bgcolor: function (feature) {
                    if (feature.cluster) {
                        return "transparent";
                    }
                    else {
                        return "#ffffff";
                    }
                },
                bordercolor: function (feature) {
                    if (feature.cluster) {
                        return "transparent";
                    }
                    else {
                        return "#cfcfcf";
                    }
                },
                bordersize: function (feature) {
                    if (feature.cluster) {
                        return 0;
                    }
                    else {
                        return "1px";
                    }
                },
                yoffset: function (feature) {
                    if (feature.cluster) {
                        if ($.browser.msie) {
                            return "9";
                        }
                        else {
                            return "7";
                        }
                    }
                    else {
                        return "-19";
                    }
                },
                fontcolor: function (feature) {
                    if (feature.cluster) {
                        return "white";
                    }
                    else {
                        return "black";
                    }
                },
                labelpadding: function (feature) {
                    if (feature.cluster) {
                        return "0";
                    }
                    else {
                        return "3px";
                    }
                }*/
            }
        });

        markers = new OpenLayers.Layer.Vector("Vehicles", {
            strategies: parent.VehicleClustering ? [markerstrategy] : [],
            displayInLayerSwitcher: false,
            isBaseLayer: false,
            styleMap: new OpenLayers.StyleMap({
                "default": parent.VehicleClustering ? clusteringMarkersStyle : markersStyle
            }),
            renderers: markersRenderer, visibility: true

        });


        map.addLayer(markers);
        map.setLayerIndex(markers, 99);

        //        if (parent.VehicleClustering) {
        //        }
        //        else if (overlaysettings.vehicleDrivers) {
        //            map.addLayer(vehicleDriversLayer);
        //        }
        //        else if (overlaysettings.vehiclenames) {
        //            map.addLayer(vehiclenamesLayer);
        //        }


        // allow testing of specific renderers via "?renderer=Canvas", etc
        historiesRenderer = OpenLayers.Util.getParameters(window.location.href).renderer;
        historiesRenderer = (historiesRenderer) ? [historiesRenderer] : OpenLayers.Layer.Vector.prototype.renderers;

        var historiesStyle = new OpenLayers.Style({
            externalGraphic: "${icon}",
            graphicWidth: 20,
            graphicHeight: 20
        }, {
            context: {
                icon: function (feature) {
                    return feature.attributes.icon;
                }
            }
        });

        histories = new OpenLayers.Layer.Vector("History", {
            displayInLayerSwitcher: false,
            isBaseLayer: false,
            styleMap: new OpenLayers.StyleMap({
                "default": historiesStyle
            }),
            renderers: historiesRenderer, visibility: true

        });


        map.addLayer(histories);

        map.addLayer(routeGeometry);
        map.setLayerIndex(routeGeometry, 1999);

        vehicleClickControl = new OpenLayers.Control.SelectFeature(
                [vectors, markers, histories, searchMarker, routeGeometry],
                {
                    onSelect: onVehicleClick,
                    autoActivate: true,
                    toggle: true,
                    clickout: false
                }
            );

        vehicleClickControl.handlers['feature'].stopDown = false;
        vehicleClickControl.handlers['feature'].stopUp = false;
        vehicleClickControl.handlers['feature'].stopClick = false;

        map.addControl(vehicleClickControl);

        vectors.events.fallThrough = true;
        markers.events.fallThrough = true;
        searchMarker.events.fallThrough = true;

        var DblclickFeature = OpenLayers.Class(OpenLayers.Control, {
            initialize: function (layer, options) {
                OpenLayers.Control.prototype.initialize.apply(this, [options]);
                this.handler = new OpenLayers.Handler.Feature(this, layer, {
                    dblclick: this.dblclick
                });
            }
        });

        dblclick = new DblclickFeature(vectors, {
            dblclick: function (evt) {
                isdoubleclick = true;
                var lastlonlat = map.getLonLatFromPixel(
                   (map.getControlsByClass("OpenLayers.Control.MousePosition")[0]).lastXy
                 );
                map.moveTo(lastlonlat, map.getZoom() + 1);
            }
        });

        map.addControl(dblclick);
        dblclick.activate();

        dblclick.handler.stopClick = false;
        dblclick.handler.stopDown = false;
        dblclick.handler.stopUp = false;

        size = new OpenLayers.Size(10, 20);
        offset = new OpenLayers.Pixel(-(size.w / 2), -size.h);

        qs();
        myWinID = qsParm["WinId"];

        var winparent = window.opener;
        if (winparent == null) {
            winparent = window.parent;
        }
        var didntLoadFrmParent = false;
        if (winparent != null) {
            if (myWinID && myWinID != "") {
                allVehicles = winparent.GetWinInitialTrackData(myWinID);
            }
            else {
                if (!allVehicles || allVehicles == "") {
                    allVehicles = winparent.GetTelogisMapData();
                    didntLoadFrmParent = true;
                }
            }
            if (allVehicles && (didntLoadFrmParent == true || myWinID != "")) {
                if (allVehicles != "") {
                    ShowMultipleAssets(allVehicles);
                }
            }
        }

        //by Devin Begin
        try {
            if (overlaysettings.geoasset) {
                DrawGeozone_NewTZ();
                DrawLandmark_NewTZ();
            }
        }
        catch (err) {
        }
        //End

        DrawWorkorder();

        //Devin added for default map view
        //Commented and Moved above by Rohit
//        if (!parent.mapAssets) {
//            if (DefaultMapCenter != null &&
//                 DefaultMapZoomLevel != null) {
//                map.setCenter(DefaultMapCenter, DefaultMapZoomLevel);
//            }
//            else
//               map.zoomTo(4);
//        }
    }
    catch (err) {
    }


    function createGezoneLandmark(event) {
        if (LandmarkDrawMode == 'redraw') {
            clearEditForm();
            //selectedFeature = event.feature;
            event.feature.fid = "Landmark:::Landmark" + (new Date()).getTime();
            currentEditFeature = event.feature;
            $('#saveredrawchanges').show();
            return;
        }
        if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {
            clearEditForm();

            var latlon = event.feature.geometry.getBounds().getCenterLonLat();
            latlon.transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
            centerPoint = latlon.lat + "," + latlon.lon;
            txtX = latlon.lon;
            txtY = latlon.lat;
            latlon.transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));
            
            var c = event.feature.geometry.components[0];
            var pointSets = "";
            var mapSearchPointSets = "";
            var centerPoint = "";
            var polygonRadius = 0;
            var point1 = new OpenLayers.Geometry.Point(latlon.lon, latlon.lat);
            for (var i = 0; i < c.components.length - 1; i++) {                
                var point2 = new OpenLayers.Geometry.Point(c.components[i].x, c.components[i].y);
                //var distance = point1.distanceTo(point2);
                var line = new OpenLayers.Geometry.LineString([point1, point2]);
                var distance = line.getGeodesicLength(new OpenLayers.Projection("EPSG:900913"));

                if (distance > polygonRadius)
                    polygonRadius = distance;


                c.components[i].transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));

                var x = c.components[i].x;
                var y = c.components[i].y;
                
                if (pointSets == "")
                    pointSets = y + "|" + x;
                else
                    pointSets = pointSets + "," + y + "|" + x;

                if (mapSearchPointSets == "")
                    mapSearchPointSets = x.toFixed(5) + " " + y.toFixed(5);
                else
                    mapSearchPointSets = mapSearchPointSets + ", " + x.toFixed(5) + " " + y.toFixed(5);

                c.components[i].transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));
            }

            if (c.components.length > 0) {
                mapSearchPointSets = mapSearchPointSets + ', ' + mapSearchPointSets.split(', ')[0];
            }
            
            var isCircle = false;
            if (document.getElementById('circleToggle').checked == true) isCircle = true;

            var content = "test";
            var popupheight = 330;

            if (isCircle) {
                popupheight = 340;
                var area = event.feature.geometry.getGeodesicArea(new OpenLayers.Projection("EPSG:900913"));
                var radius = 0.565352 * Math.sqrt(area);
                content = "<h6 style=\"margin:0 0 10px 10px;border-bottom:1px solid #cccccc\">" + h6CircleContentText; //res  //content = "<h6 style=\"margin:0 0 10px 10px;border-bottom:1px solid #cccccc\">New circle type landmark</h6>";
                content += "<div style='font-size:1em'><form id='polygonform'>" +
                            "<input type='hidden' id='isNew' name='isNew' value='1' />" +
                            "<input type='hidden' id='txtX' name='txtX' value='" + txtX + "' />" +
                            "<input type='hidden' id='txtY' name='txtY' value='" + txtY + "' />" +
                            "<input type='hidden' id='lstAddOptions' name='lstAddOptions' value='0' />" +
                                '<table class="landmarkfield landmarkmainoptions" cellspacing="0" cellpadding="0" border="0" style="margin-left:10px;">' +
                                '<tr><td style="padding-top:10px;"><table border="0">' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px; width: 139px;">' +
                                '            <span id="lblLandmarkNameTitle" class="formtext">' + lblLandmarkNameTitle + '</span>' + //res //'            <span id="lblLandmarkNameTitle" class="formtext">Landmark Name (*):</span>' +
                                '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkName" type="text" id="txtLandmarkName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblContactNameTitle" class="formtext">' + lblContactNameTitle + '</span></td>' + //res  //'            <span id="lblContactNameTitle" class="formtext">Contact Name:</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtContactName" type="text" id="txtContactName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblLandmarkDescriptionTitle" class="formtext">' + lblLandmarkDescriptionTitle + '</span></td>' + //res  //'            <span id="lblLandmarkDescriptionTitle" class="formtext">Landmark Description:</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkDesc" type="text" id="txtLandmarkDesc" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblPhoneTitle" class="formtext">' + lblPhoneTitle + '</span></td>' + //res   //'            <span id="lblPhoneTitle" class="formtext">Phone :</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtPhone" type="text" id="txtPhone" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 20px;">' +
                                '            <span id="lblRadiusTitle" class="formtext">' + lblRadiusTitle + '</span>' + //res    //'            <span id="lblRadiusTitle" class="formtext">Radius</span>' +
                                '            (' +
                                '            <span id="lblUnit">m</span>):' +
                                '            <span id="valRadius" style="color:Red;visibility:hidden;">*</span><span id="valRangeRadius" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" height="15">' +
                                '            <input name="txtRadius" type="text" id="txtRadius" class="formtext bsmforminput" style="width:173px;" value="' + radius.toFixed(0) + '" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 20px;">' +
                                '            <span id="lblCategoryTitle" class="formtext">' + Res_CategoryTitle + '</span></td>' + //res    //Category                                
                                '        <td class="formtext" height="15">' + buildCategoryDropDown() +
                                '        </td>' +
                                '    </tr>' ;
                content += '   <tr>' +
                                '       <td colspan="2" class="formtext" style="height: 30px;">' +
                                '           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" checked /><span style="margin-left:3px;">' + lstPublicPrivate_PrivateText + '</span>';  //res  //'           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" /><span style="margin-left:3px;">Private</span>' +
                                    if (ShowPublicLandmarkOption) {
                                        content += '          <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;"  /><span style="margin-left:3px;">' + lstPublicPrivate_PublicText + '</span>'; //res    //'           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" checked /><span style="margin-left:3px;">Public</span>' +
                                    }
              

                               content += '       </td>' +
                                '   </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '           <a href=\'javascript:void(0)\' onclick=\'landmarkformmoreoptions();\'>' + alandmarkformmoreoptionsText + '</a>'; //res   //'           <a href=\'javascript:void(0)\' onclick=\'landmarkformmoreoptions();\'>More Options</a>';
                if (ShowMapHistorySearch) {
                    content += '           <a href=\'javascript:void(0)\' onclick=\'landmarkhistorysearch();\'>' + lnklandmarkhistorysearchText + '</a>'; //res
                }
                content += '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '</table></td></tr></table>' +
                                '<table id="Table4" class="geozonelandmarkmoreoption" style="border:0; display:none;" cellspacing="0" cellpadding="0">' +
                                '<tr><td valign="top" style="padding:10px;vertical-align: top;"><table border="0">' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblEmailTitle" class="formtext">' + lblEmailTitle + '</span>' + //res    //'            <span id="lblEmailTitle" class="formtext">Email:</span>' +
                                '            </td>' +
                                '        <td>' +
                                '            <input name="txtEmail" type="text" id="txtEmail" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr style="display:none;">' +
                                '        <td class="style4" style="height: 30px">' +
                                '            <span id="lblPhone" class="formtext"> ' + lblPhoneTitle + '</span>' + //res    //'            <span id="lblPhone" class="formtext"> Phone:</span>' +
                                '                                        <span id="RegularExpressionValidator1" class="formtext" style="color:Red;visibility:hidden;">*</span>' +
                                '                        </td>' +
                                '        <td>' +
                                '            <input name="txtPhoneSMS" type="text" id="txtPhoneSMS" disabled="disabled" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblTimeZoneTitle" class="formtext">' + lblTimeZoneTitle + '</span>' + //res   //'            <span id="lblTimeZoneTitle" class="formtext">Time Zone:</span>' +
                                '                        </td>' +
                                '        <td>' +
                                '            <select name="cboTimeZone" id="cboTimeZone" class="RegularText" style="width:168px;">' +
			                    '                <option value="-12">' + cboTimeZoneGMT + '-12</option>' + //res //'                <option value="-12">GMT-12</option>' +
			                    '                <option value="-11">' + cboTimeZoneGMT + '-11</option>' + //res //'                <option value="-11">GMT-11</option>' +
			                    '                <option value="-10">' + cboTimeZoneGMT + '-10</option>' + //res //'                <option value="-10">GMT-10</option>' +
			                    '                <option value="-9">' + cboTimeZoneGMT + '-9</option>' + //res   //'                <option value="-9">GMT-9</option>' +
			                    '                <option value="-8">' + cboTimeZoneGMT + '-8</option>' + //res   //'                <option value="-8">GMT-8</option>' +
			                    '                <option value="-7">' + cboTimeZoneGMT + '-7</option>' + //res   //'                <option value="-7">GMT-7</option>' +
			                    '                <option value="-6">' + cboTimeZoneGMT + '-6</option>' + //res   //'                <option value="-6">GMT-6</option>' +
			                    '                <option value="-5">' + cboTimeZoneGMT + '-5</option>' + //res   //'                <option value="-5">GMT-5</option>' +
			                    '                <option value="-4">' + cboTimeZoneGMT + '-4</option>' + //res   //'                <option value="-4">GMT-4</option>' +
			                    '                <option value="-3">' + cboTimeZoneGMT + '-3</option>' + //res   //'                <option value="-3">GMT-3</option>' +
			                    '                <option value="-2">' + cboTimeZoneGMT + '-2</option>' + //res   //'                <option value="-2">GMT-2</option>' +
			                    '                <option value="-1">' + cboTimeZoneGMT + '-1</option>' + //res   //'                <option value="-1">GMT-1</option>' +
			                    '                <option value="0">' + cboTimeZoneGMT + '</option>' + //res      //'                <option value="0">GMT</option>' +
			                    '                <option value="1">' + cboTimeZoneGMT + '+1</option>' + //res    //'                <option value="1">GMT+1</option>' +
			                    '                <option value="2">' + cboTimeZoneGMT + '+2</option>' + //res    //'                <option value="2">GMT+2</option>' +
			                    '                <option value="3">' + cboTimeZoneGMT + '+3</option>' + //res    //'                <option value="3">GMT+3</option>' +
			                    '                <option value="4">' + cboTimeZoneGMT + '+4</option>' + //res    //'                <option value="4">GMT+4</option>' +
			                    '                <option value="5">' + cboTimeZoneGMT + '+5</option>' + //res    //'                <option value="5">GMT+5</option>' +
			                    '                <option value="6">' + cboTimeZoneGMT + '+6</option>' + //res    //'                <option value="6">GMT+6</option>' +
			                    '                <option value="7">' + cboTimeZoneGMT + '+7</option>' + //res    //'                <option value="7">GMT+7</option>' +
			                    '                <option value="8">' + cboTimeZoneGMT + '+8</option>' + //res    //'                <option value="8">GMT+8</option>' +
			                    '                <option value="9">' + cboTimeZoneGMT + '+9</option>' + //res    //'                <option value="9">GMT+9</option>' +
			                    '                <option value="10">' + cboTimeZoneGMT + '+10</option>' + //res  //'                <option value="10">GMT+10</option>' +
			                    '                <option value="11">' + cboTimeZoneGMT + '+11</option>' + //res  //'                <option value="11">GMT+11</option>' +
			                    '                <option value="12">' + cboTimeZoneGMT + '+12</option>' + //res  //'                <option value="12">GMT+12</option>' +
			                    '                <option value="13">' + cboTimeZoneGMT + '+13</option>' + //res  //'                <option value="13">GMT+13</option>' +
		                        '            </select></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2">' +
                                '                                    <span id="lblMultipleEmails" class="formtext">' + lblMultipleEmailsText + '</span>' + //res  //'                                    <span id="lblMultipleEmails" class="formtext">*Multiple email addresses Must be Separated by semicolon or comma</span>' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td style="height: 30px" colspan="2">' +
                                '            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" /><label for="chkDayLight">' + chkDayLightText + '</label></span>' + //res  //'            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" /><label for="chkDayLight">Automatically adjust for daylight savings time</label></span>' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td height="5">' +
                                '        </td>' +
                                '        <td height="5">' +
                                '        </td>' +
                                '   </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '           <a href=\'javascript:void(0)\' onclick=\'geozonelandmarkformmainoptions();\'>' + lnkBackText + '</a>' + //res  //'           <a href=\'javascript:void(0)\' onclick=\'geozonelandmarkformmainoptions();\'>Back</a>' +
                                '        </td>' +
                                '    </tr>' +
                                '   <tr>' +
                                '        <td height="5" class="style4">' +
                                '        </td>' +
                                '        <td height="5">' +
                                '        </td>' +
                                '    </tr>' +
                                '</table></td></tr></table>';
                if (ShowMapHistorySearch) {
                    content += '<table class="geozonelandmarksearchhistory" style="border:0; display:none;" cellspacing="0" cellpadding="0">' +
                                '<tr><td valign="top" style="padding:10px;vertical-align: top;">' +
                                "   <div id='SearchHistoryCriterias' class='SearchAddressFields' style='height: 230px;'>" +
                                "       <div id='SearchHistoryTitle' style='font-weight: bold; color: green;margin:10px 0;'>" + SearchHistoryTitle + "</div>" + //res
                                "       <div><span style='font-weight: bold; color: green;'>" + SearchHistoryDateText + "</span> <input type='text' size='10' id='SearchHistoryDate' />" + //res
                                "            <span style='font-weight: bold; color: green;'>" + SearchHistoryTimeText + "</span> <select id='SearchHistoryTime'>" + createTimeOptions() + "</select></div>" + //res
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>" + vehicleText + "</span> <input type='text' id='SearchHistoryRadius' value='" + radius.toFixed(0) + "' /> m </div>" + //res lblRadiusTitle
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>" + SearchHistoryTimeRangeText + "</span> <span style='font-weight: bold; color: green;'>+/-</span> <select id='SearchHistoryMinutes'> " + buildOptions(isExtended) + " </select>" + SearchHistoryMinutesText + " &nbsp; " + //res
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>" + Res_By + ": </span><select id='SearchHistoryBy'><option value='1'>" + Res_SearchHistoryFleetHierarchy + "</option><option value='2'>" + Res_SelectedVehicles + "</option></select></div>" +
                                //"       <div style='margin-top: 5px;'><a href='javascript:void(0)' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");'><img src='../images/searchicon.png' /> Search</a></div>" +
                                "       <div style='margin-top: 5px;'><input type='button' class='kd-button' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");' value=" + SearchText + " /></div>" + //res
                                "       <div id='searchAddressMessage' style='color:red;display:none;'>" + searchAddressMessageText + "</div>" + //res
                                "       <div style='margin-top: 30px;'><a href='javascript:void(0)' onclick='geozonelandmarkformmainoptions();'>" + lnkBackText + "</a></div>" + //res
                                "   </div>" +
                                "</td></tr></table>";
                }

                content += "<div style='font-size: 1em;margin-top:10px;'><input type='button' class='kd-button' onclick='savePolygon_NewTZ(this, popup, selectedFeature);' value=" + SaveText + " /> <input type='button' class='kd-button' value=" + CancelText + " onclick='cancelPolygon(popup, selectedFeature);' /></div>" + //res //content +=      "<div style='font-size: 1em;margin-top:10px;'><input type='button' class='kd-button' onclick='savePolygon(this, popup, selectedFeature);' value='Save' /> <input type='button' class='kd-button' value='Cancel' onclick='cancelPolygon(popup, selectedFeature);' /></div>" +
                            "</form></div>";
            }
            else {
                content = "<h6 style=\"margin:0 0 10px 10px;border-bottom:1px solid #cccccc\">" + h6NewGeozoneLandmarlText + "</h6>"; //res   //content = "<h6 style=\"margin:0 0 10px 10px;border-bottom:1px solid #cccccc\">New Geozone/Landmark</h6>";
                content += "<div style='font-size:1em'><form id='polygonform'>" +
                            "<input type='hidden' id='isNew' name='isNew' value='1' />" +
                            "<input type='hidden' id='pointSets' name='pointSets' value='" + pointSets + "' />" +
                            "<input type='hidden' id='txtX' name='txtX' value='" + txtX + "' />" +
                            "<input type='hidden' id='txtY' name='txtY' value='" + txtY + "' />" +
                            "<input type='hidden' id='txtRadius' name='txtRadius' value='-1' />" +
                            "<input id='lstAddOptions_0' type='radio' onclick='$(\".geozonefield\").hide();$(\".landmarkfield\").show();$(\".geozonelandmarkmoreoption\").hide();' value='0' name='lstAddOptions' checked /><label for='lstAddOptions_0' style='margin-left:3px;'>" + lstAddOptionsLandmarkText + "</label>" + //res  //"<input id='lstAddOptions_0' type='radio' onclick='$(\".geozonefield\").hide();$(\".landmarkfield\").show();$(\".geozonelandmarkmoreoption\").hide();' value='0' name='lstAddOptions' checked /><label for='lstAddOptions_0' style='margin-left:3px;'>Landmark</label>" +
                            "<input id='lstAddOptions_0' type='radio' onclick='$(\".landmarkfield\").hide();$(\".geozonefield\").show();$(\".geozonelandmarkmoreoption\").hide();' value='1' name='lstAddOptions' style='margin-left:20px;' /><label for='lstAddOptions_0' style='margin-left:3px;'>" + lbllstAddOptionsGeozoneText + "</label>" + //res   //"<input id='lstAddOptions_0' type='radio' onclick='$(\".landmarkfield\").hide();$(\".geozonefield\").show();$(\".geozonelandmarkmoreoption\").hide();' value='1' name='lstAddOptions' style='margin-left:20px;' /><label for='lstAddOptions_0' style='margin-left:3px;'>Geozone</label>" +
                                '<table class="landmarkfield landmarkmainoptions" cellspacing="0" cellpadding="0" border="0" style="margin-left:10px;">' +
                                '<tr><td style="padding-top:10px;"><table border="0">' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px; width: 139px;">' +
                                '            <span id="lblLandmarkNameTitle" class="formtext">' + lblLandmarkNameTitle + '</span>' + //res  //'            <span id="lblLandmarkNameTitle" class="formtext">Landmark Name (*):</span>' +
                                '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkName" type="text" id="txtLandmarkName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblContactNameTitle" class="formtext">' + lblContactNameTitle + '</span></td>' + //res  //'            <span id="lblContactNameTitle" class="formtext">Contact Name:</span></td>' +
                                '        <td class="formtext" style="height: 22px">' +
                                '            <input name="txtContactName" type="text" id="txtContactName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblLandmarkDescriptionTitle" class="formtext">' + lblLandmarkDescriptionTitle + '</span></td>' + //res    //'            <span id="lblLandmarkDescriptionTitle" class="formtext">Landmark Description:</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkDesc" type="text" id="txtLandmarkDesc" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblPhoneTitle" class="formtext">' + lblPhoneTitle + '</span></td>' + //res   //'            <span id="lblPhoneTitle" class="formtext">Phone :</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtPhone" type="text" id="txtPhone" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 20px;">' +
                                '            <span id="lblCategoryTitle" class="formtext">' + Res_CategoryTitle + '</span></td>' + //res    //Category                                
                                '        <td class="formtext" height="15">' + buildCategoryDropDown() +
                                '        </td>' +
                                '    </tr>';
                content += '   <tr>' +
                               '       <td colspan="2" class="formtext" height="15" style="height: 30px;">' +
                               '           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" /><span style="margin-left:3px;">' + lstPublicPrivate_PrivateText + '</span>'; //res   //'           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" /><span style="margin-left:3px;">Private</span>' +
                if (ShowPublicLandmarkOption) {
                    content += '           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" checked /><span style="margin-left:3px;">' + lstPublicPrivate_PublicText + '</span>'; //res  //'           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" checked /><span style="margin-left:3px;">Public</span>' +
                }
                content += '       </td>' +
                           '   </tr>' +
                           '    <tr>' +
                           '        <td colspan="2" class="formtext" height="15">' +
                           '        </td>' +
                           '    </tr>' +
                           '    <tr>' +
                           '        <td colspan="2" class="formtext" height="15">' +
                           '           <a href=\'javascript:void(0)\' onclick=\'landmarkformmoreoptions();\'>' + alandmarkformmoreoptionsText + '</a>'; //res   //'           <a href=\'javascript:void(0)\' onclick=\'landmarkformmoreoptions();\'>More Options</a>';
                if (ShowMapHistorySearch) {
                    content += '           <a href=\'javascript:void(0)\' onclick=\'landmarkhistorysearch();\'>' + lnklandmarkhistorysearchText + '</a>'; //res
                }
                content += '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '</table></td></tr></table>' +
                                '<table class="geozonefield geozonemainoptions" cellspacing="0" cellpadding="0" style="margin-left:10px;display:none;" border="0">' +
                                '<tr><td style="padding-top:10px;"><table border="0">' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblGeozoneNameTitle" class="formtext">' + lblGeozoneNameTitle + '</span>' + //res   //'            <span id="lblGeozoneNameTitle" class="formtext">GeoZone Name (*):</span>' +
                                '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" style="height: 22px">' +
                                '            <input name="txtGeoZoneName" type="text" id="txtGeoZoneName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblDirectionTitle" class="formtext">' + lblDirectionTitle + '</span></td>' + //res   //'            <span id="lblDirectionTitle" class="formtext">Direction:</span></td>' +
                                '        <td class="formtext" style="height: 22px">' +
                                '            <select name="cboDirection" id="cboDirection" class="formtext" style="width:175px;">' +
			                    '                <option selected="selected" value="0">' + cboDirection_DisableText + '</option>' + //res   //'                <option selected="selected" value="0">Disable</option>' +
			                    '                <option value="1">' + cboDirection_OutText + '</option>' + //res   //'                <option value="1">Out</option>' +
			                    '                <option value="2">' + cboDirection_InText + '</option>' + //res    //'                <option value="2">In</option>' +
			                    '                <option value="3">' + cboDirection_InOutText + '</option>' + //res //'                <option value="3">InOut</option>' +
		                        '            </select></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblGeozoneDescriptionTitle" class="formtext">' + lblGeozoneDescriptionTitle + '</span></td>' + //res  //'            <span id="lblGeozoneDescriptionTitle" class="formtext">GeoZone Description:</span></td>' +
                                '        <td class="formtext" style="height: 20px">' +
                                '            <input name="txtGeoZoneDesc" type="text" id="txtGeoZoneDesc" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblDefaultSeverityTitle" class="formtext">' + lblDefaultSeverityTitle + '</span>' + //res  //'            <span id="lblDefaultSeverityTitle" class="formtext">Default Severity:</span>' +
                                '        </td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <select name="cboGeoZoneSeverity" onchange="" id="cboGeoZoneSeverity" class="formtext" DESIGNTIMEDRAGDROP="451" style="width:175px;">' +
			                    '                <option selected="selected" value="0">' + cboGeoZoneSeverity_NoAlarmText + '</option>' + //res   //'                <option selected="selected" value="0">NoAlarm</option>' +
			                    '                <option value="1">' + cboGeoZoneSeverity_NotifyText + '</option>' + //res   //'                <option value="1">Notify</option>' +
			                    '                <option value="2">' + cboGeoZoneSeverity_WarningText + '</option>' + //res  //'                <option value="2">Warning</option>' +
			                    '                <option value="3">' + cboGeoZoneSeverity_CriticalText + '</option>' + //res //'                <option value="3">Critical</option>' +
		                        '            </select></td>' +
                                '    </tr>' +
                                '   <tr>' +
                                '       <td colspan="2" class="formtext" style="height: 30px;">' +
                                '           <input id="lstPublicPrivate" type="radio" value="0" name="lstGeozonePublicPrivate" checked /><span style="margin-left:3px;">' + lstPublicPrivate_PrivateText + '</span>'; //res  //'           <input id="lstPublicPrivate" type="radio" value="0" name="lstGeozonePublicPrivate" /><span style="margin-left:3px;">Private</span>' +


                                if (ShowPublicGeoZoneOption) {
                                     content += '           <input id="lstPublicPrivate" type="radio" value="1" name="lstGeozonePublicPrivate" style="margin-left:20px;"  /><span style="margin-left:3px;">' + lstPublicPrivate_PublicText + '</span>'; //res  //'           <input id="lstPublicPrivate" type="radio" value="1" name="lstGeozonePublicPrivate" style="margin-left:20px;" checked /><span style="margin-left:3px;">Public</span>' +
                                 }
                
                                 content += '      </td>' +
                                '   </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '           <a href=\'javascript:void(0)\' onclick=\'geozoneformmoreoptions();\'>' + alandmarkformmoreoptionsText + '</a>' + //res  //'           <a href=\'javascript:void(0)\' onclick=\'geozoneformmoreoptions();\'>More Options</a>' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '</table></td></tr></table>' +
                                '<table class="geozonelandmarkmoreoption" id="Table4" style="border:0;display:none" cellspacing="0" cellpadding="0">' +
                                '<tr><td valign="top" style="padding:10px;vertical-align: top;"><table border="0">' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblEmailTitle" class="formtext">' + lblEmailTitle + '</span>' + //res   //'            <span id="lblEmailTitle" class="formtext">Email:</span>' +
                                '            </td>' +
                                '        <td style="height: 30px">' +
                                '            <input name="txtEmail" type="text" id="txtEmail" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr style="display:none;">' +
                                '        <td class="style4" style="height: 30px">' +
                                '            <span id="lblPhone" class="formtext">' + lblPhoneTitle + '</span>' + //res      //'            <span id="lblPhone" class="formtext"> Phone:</span>' +
                                '                                        <span id="RegularExpressionValidator1" class="formtext" style="color:Red;visibility:hidden;">*</span>' +
                                '                        </td>' +
                                '        <td style="height: 30px">' +
                                '            <input name="txtPhoneSMS" type="text" id="txtPhoneSMS" disabled="disabled" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblTimeZoneTitle" class="formtext">' + lblTimeZoneTitle + '</span>' + //res   //'            <span id="lblTimeZoneTitle" class="formtext">Time Zone:</span>' +
                                '                        </td>' +
                                '        <td style="height: 30px">' +
                                '            <select name="cboTimeZone" id="cboTimeZone" class="RegularText" style="width:168px;">' +
			                    '                <option value="-12">' + cboTimeZoneGMT + '-12</option>' + //res     //'                <option value="-12">GMT-12</option>' +
			                    '                <option value="-11">' + cboTimeZoneGMT + '-11</option>' + //res     //'                <option value="-11">GMT-11</option>' +
			                    '                <option value="-10">' + cboTimeZoneGMT + '-10</option>' + //res     //'                <option value="-10">GMT-10</option>' +
			                    '                <option value="-9">' + cboTimeZoneGMT + '-9</option>' + //res       //'                <option value="-9">GMT-9</option>' +
			                    '                <option value="-8">' + cboTimeZoneGMT + '-8</option>' + //res       //'                <option value="-8">GMT-8</option>' +
			                    '                <option value="-7">' + cboTimeZoneGMT + '-7</option>' + //res       //'                <option value="-7">GMT-7</option>' +
			                    '                <option value="-6">' + cboTimeZoneGMT + '-6</option>' + //res       //'                <option value="-6">GMT-6</option>' +
			                    '                <option value="-5">' + cboTimeZoneGMT + '-5</option>' + //res       //'                <option value="-5">GMT-5</option>' +
			                    '                <option value="-4">' + cboTimeZoneGMT + '-4</option>' + //res       //'                <option value="-4">GMT-4</option>' +
			                    '                <option value="-3">' + cboTimeZoneGMT + '-3</option>' + //res       //'                <option value="-3">GMT-3</option>' +
			                    '                <option value="-2">' + cboTimeZoneGMT + '-2</option>' + //res       //'                <option value="-2">GMT-2</option>' +
			                    '                <option value="-1">' + cboTimeZoneGMT + '-1</option>' + //res       //'                <option value="-1">GMT-1</option>' +
			                    '                <option value="0">' + cboTimeZoneGMT + '</option>' + //res          //'                <option value="0">GMT</option>' +
			                    '                <option value="1">' + cboTimeZoneGMT + '+1</option>' + //res        //'                <option value="1">GMT+1</option>' +
			                    '                <option value="2">' + cboTimeZoneGMT + '+2</option>' + //res        //'                <option value="2">GMT+2</option>' +
			                    '                <option value="3">' + cboTimeZoneGMT + '+3</option>' + //res        //'                <option value="3">GMT+3</option>' +
			                    '                <option value="4">' + cboTimeZoneGMT + '+4</option>' + //res        //'                <option value="4">GMT+4</option>' +
			                    '                <option value="5">' + cboTimeZoneGMT + '+5</option>' + //res        //'                <option value="5">GMT+5</option>' +
			                    '                <option value="6">' + cboTimeZoneGMT + '+6</option>' + //res        //'                <option value="6">GMT+6</option>' +
			                    '                <option value="7">' + cboTimeZoneGMT + '+7</option>' + //res        //'                <option value="7">GMT+7</option>' +
			                    '                <option value="8">' + cboTimeZoneGMT + '+8</option>' + //res        //'                <option value="8">GMT+8</option>' +
			                    '                <option value="9">' + cboTimeZoneGMT + '+9</option>' + //res        //'                <option value="9">GMT+9</option>' +
			                    '                <option value="10">' + cboTimeZoneGMT + '+10</option>' + //res      //'                <option value="10">GMT+10</option>' +
			                    '                <option value="11">' + cboTimeZoneGMT + '+11</option>' + //res      //'                <option value="11">GMT+11</option>' +
			                    '                <option value="12">' + cboTimeZoneGMT + '+12</option>' + //res      //'                <option value="12">GMT+12</option>' +
			                    '                <option value="13">' + cboTimeZoneGMT + '+13</option>' + //res      //'                <option value="13">GMT+13</option>' +
		                        '            </select></td>' +
                                '        <td class="style4">' +
                                '            &nbsp;</td>' +
                                '        <td>' +
                                '            &nbsp;</td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="4">' +
                                '                                    <span id="lblMultipleEmails" class="formtext">' + lblMultipleEmailsText + '</span>' + //res    //'                                    <span id="lblMultipleEmails" class="formtext">*Multiple email addresses Must be Separated by semicolon or comma</span>' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td style="height: 9px" colspan="4">' +
                                '            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" /><label for="chkDayLight">' + chkDayLightText + '</label></span>' + //res   //'            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" /><label for="chkDayLight">Automatically adjust for daylight savings time</label></span>' +
                                '            <table class="geozonefield" cellspacing="0" cellpadding="0" border="0" style="display:none;">' +
                                '                <tr>' +
                                '                    <td>' +
                                '                        <input id="chkCritical" type="checkbox" name="chkCritical" /><label for="chkCritical">' + chkCriticalText + '</label></td>' + //res    //'                        <input id="chkCritical" type="checkbox" name="chkCritical" /><label for="chkCritical">Critical Alarm</label></td>' +
                                '                </tr>' +
                                '                <tr>' +
                                '                    <td>' +
                                '                        <input id="chkWarning" type="checkbox" name="chkWarning" /><label for="chkWarning">' + chkWarningText + '</label></td>' + //res       //'                        <input id="chkWarning" type="checkbox" name="chkWarning" /><label for="chkWarning">Warning Alarm</label></td>' +
                                '                </tr>' +
                                '                <tr>' +
                                '                    <td>' +
                                '                        <input id="chkNotify" type="checkbox" name="chkNotify" /><label for="chkNotify">' + chkNotifyText + '</label></td>' + //res          //'                        <input id="chkNotify" type="checkbox" name="chkNotify" /><label for="chkNotify">Notify Alarm</label></td>' +
                                '                </tr>' +
                                '            </table>' +
                                '        </td>' +
                                '    </tr>' +
                                 '    <tr>' +
                                '        <td height="5">' +
                                '        </td>' +
                                '        <td height="5">' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '           <a href=\'javascript:void(0)\' onclick=\'geozonelandmarkformmainoptions();\'>' + lnkBackText + '</a>' + //res     //'           <a href=\'javascript:void(0)\' onclick=\'geozonelandmarkformmainoptions();\'>Back</a>' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td height="5">' +
                                '        </td>' +
                                '        <td height="5">' +
                                '        </td>' +
                                '    </tr>' +
                                '</table></td></tr></table>';

                if (ShowMapHistorySearch) {

                    content += '<table class="geozonelandmarksearchhistory" style="border:0; display:none;" cellspacing="0" cellpadding="0">' +
                                '<tr><td valign="top" style="padding:10px;vertical-align: top;">' +
                                "   <div id='SearchHistoryCriterias' class='SearchAddressFields' style='height: 206px;'>" +
                                "       <div id='SearchHistoryTitle' style='font-weight: bold; color: green;margin:10px 0;'>" + SearchHistoryTitle + "</div>" + //res
                                "       <div><span style='font-weight: bold; color: green;'>" + SearchHistoryDateText + "</span> <input type='text' size='10' id='SearchHistoryDate' />" + //res
                                "            <span style='font-weight: bold; color: green;'>" + SearchHistoryTimeText + "</span> <select id='SearchHistoryTime'>" + createTimeOptions() + "</select></div>" + //res
                                "       <div style='margin-top: 5px;display:none;'><span style='font-weight: bold; color: green;'>" + vehicleText + "</span> <input type='text' id='SearchHistoryRadius' value='" + polygonRadius.toFixed(0) + "' /> m </div>" + //res
                                "<input type='hidden' id='mapSearchPointSets' name='mapSearchPointSets' value='" + mapSearchPointSets + "' />" +
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>" + SearchHistoryTimeRangeText + "</span> <span style='font-weight: bold; color: green;'>+/-</span> <select id='SearchHistoryMinutes'> " + buildOptions(isExtended) + " </select> " + SearchHistoryMinutesText + " &nbsp; " + //res
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>" + Res_By + ": </span><select id='SearchHistoryBy'><option value='1'>" + Res_SearchHistoryFleetHierarchy + "</option><option value='2'>" + Res_SelectedVehicles + "</option></select></div>" +
                                //"       <a href='javascript:void(0)' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");'><img src='../images/searchicon.png' /></a>" +
                                "       <div style='margin-top: 5px;'><input type='button' class='kd-button' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");' value=" + SearchText + " /></div>" + //res value='Search'
                                "       <div id='searchAddressMessage' style='color:red;display:none;'>" + searchAddressMessageText + "</div>" + //res
                                "       <div style='margin-top: 30px;'><a href='javascript:void(0)' onclick='geozonelandmarkformmainoptions();'>" + lnkBackText + "</a></div>" + //res
                                "   </div>" +
                                "</td></tr></table>";
                }

                content += "<div style='font-size: 1em;margin-top:10px;'><input type='button' class='kd-button' onclick='savePolygon_NewTZ(this, popup, selectedFeature);' value=" + SaveText + " /> <input type='button' class='kd-button' value=" + CancelText + " onclick='cancelPolygon(popup, selectedFeature);' /></div>" + //res  //content +=     "<div style='font-size: 1em;margin-top:10px;'><input type='button' class='kd-button' onclick='savePolygon(this, popup, selectedFeature);' value='Save' /> <input type='button' class='kd-button' value='Cancel' onclick='cancelPolygon(popup, selectedFeature);' /></div>" +
                            "</form></div>";
            }

            selectedFeature = event.feature;

            //geoLandmarkFeatures.push(selectedFeature);
            //vectors.removeFeatures(geoLandmarkFeatures.features);
            //vectors.addFeatures(geoLandmarkFeatures);

            popup = new OpenLayers.Popup.FramedCloud("polygonformpopup",
                                    event.feature.geometry.getBounds().getCenterLonLat(),
                                    null,
                                    content,
                                    null, true, onCreateGezoneLandmarkClose);
            event.feature.popup = popup;
            selectedPopup = popup;
            map.addPopup(popup);

            popup.setSize(new OpenLayers.Size(400, popupheight));

            currentEditFeature = selectedFeature;
            currentEditPopup = popup;

        }
    }

    function beforefeatureselected(event) {
        if (event.feature.cluster) {
            $('#messagebar').html(event.feature.attributes.count + messagebarGeoLandZoomInText).show().delay(3000).fadeOut(1000); //res //$('#messagebar').html(event.feature.attributes.count + " Geozone/Landmark(s). Zoom in for details.").show().delay(3000).fadeOut(1000);
            return false;
        }
        if (event.feature.fid.split(":::")[0] == "LandmarkCircle" && document.getElementById("modifyToggle").checked == true) {
            $('#messagebar').html(messagebarEditLandmarkCircleText).show().delay(2000).fadeOut(1000); //res  //$('#messagebar').html("Please use Edit button to modify Landmark Circle.").show().delay(2000).fadeOut(1000);
            return false;
        }
        if (event.feature.fid.split(":::")[0] == "Geozone" && document.getElementById("modifyToggle").checked == true && event.feature.Assigned == 1) {
            $('#messagebar').html(messagebarGeozoneNonEditableText).show().delay(2000).fadeOut(1000); //res   //$('#messagebar').html("This Geozone is assigned to a box and it's not editable.").show().delay(2000).fadeOut(1000);
            return false;
        }
    }

    function beforeModified(event) {
        if (event.feature.fid.split(":::")[0] == "LandmarkCircle") {
            mycontrols.modify.mode = OpenLayers.Control.ModifyFeature.RESIZE | OpenLayers.Control.ModifyFeature.DRAG;
        }
        else if (event.feature.fid.split(":::")[0] == "Geozone" && document.getElementById("modifyToggle").checked == true && event.feature.Assigned == 1) {
            $('#messagebar').html(messagebarGeozoneNonEditableText).show().delay(2000).fadeOut(1000); //res   //$('#messagebar').html("This Geozone is assigned to a box and it's not editable.").show().delay(2000).fadeOut(1000);
            return false;
        }
        else {
            mycontrols.modify.mode = OpenLayers.Control.ModifyFeature.RESHAPE;
        }
        if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {

            var c = event.feature.geometry.components[0];
            previousPointSets = "";
            previousCenterPoint = "";
            previousRadius = 0;
            for (var i = 0; i < c.components.length - 1; i++) {
                c.components[i].transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));

                var x = c.components[i].x;
                var y = c.components[i].y;

                if (previousPointSets == "")
                    previousPointSets = y + "|" + x;
                else
                    previousPointSets = previousPointSets + "," + y + "|" + x;

                c.components[i].transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));
            }

            var oinfo = event.feature.fid.split(":::");
            var otype;
            var oid;
            if (oinfo.length > 0) {
                otype = encodeURI(oinfo[0]);
                oid = encodeURI(oinfo[1]);
                previousPointSets = encodeURI(previousPointSets);
            }

            if (event.feature.fid.split(":::")[0] == "LandmarkCircle") {
                var latlon = event.feature.geometry.getBounds().getCenterLonLat();
                latlon.transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
                previousCenterPoint = latlon.lat + "," + latlon.lon;
                latlon.transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));

                var area = event.feature.geometry.getGeodesicArea(new OpenLayers.Projection("EPSG:900913"));
                previousRadius = (0.565352 * Math.sqrt(area)).toFixed(0);
            }
        }
    }

    function modified(event) {
        if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {
            
            var c = event.feature.geometry.components[0];
            var pointSets = "";
            var centerPoint = "";
            for (var i = 0; i < c.components.length - 1; i++) {
                c.components[i].transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));

                var x = c.components[i].x;
                var y = c.components[i].y;

                if (pointSets == "")
                    pointSets = y + "|" + x;
                else
                    pointSets = pointSets + "," + y + "|" + x;

                c.components[i].transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));
            }

            var radius = '';
            if (event.feature.fid.split(":::")[0] == "LandmarkCircle") {
                var latlon = event.feature.geometry.getBounds().getCenterLonLat();
                latlon.transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
                centerPoint = latlon.lat + "," + latlon.lon;
                latlon.transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));
                
                var area = event.feature.geometry.getGeodesicArea(new OpenLayers.Projection("EPSG:900913"));
                radius = (0.565352 * Math.sqrt(area)).toFixed(0);
            }

            var oinfo = event.feature.fid.split(":::");
            var otype;
            var oid;
            if (oinfo.length > 0) {
                otype = encodeURI(oinfo[0]);
                oid = encodeURI(oinfo[1]);

                pointSets = encodeURI(pointSets);

                $('#undosaving').hide();
                $('#undolink').show();
                $('#savechanges').show();
                pushToUndolist(otype, oid, pointSets, centerPoint, radius);
                previousCenterPoint = centerPoint;
                previousRadius = radius;
                previousPointSets = pointSets;
                $('#undobar').show();

                //editPolygon(otype, oid, pointSets, false);
            }
        }
    }

    function onCreateGezoneLandmarkClose(evt) {
        map.removePopup(selectedPopup);
        vectors.removeFeatures(selectedFeature);
    }

    vectors.events.on({
        "beforefeaturemodified": beforeModified,
        "featuremodified": modified,
        "beforefeatureselected": beforefeatureselected,
        //"afterfeaturemodified": report,
        //"vertexmodified": report,
        //"sketchmodified": report,
        //"sketchstarted": report,
        "sketchcomplete": createGezoneLandmark
    });


    selectControl = new OpenLayers.Control.SelectFeature(vectors, { onSelect: onFeatureSelect, onUnselect: onFeatureUnselect });
    
    circleOptions = { sides: 40 };

    mycontrols = {
        select: selectControl,
        point: new OpenLayers.Control.DrawFeature(vectors, OpenLayers.Handler.Point),
        circle: new OpenLayers.Control.DrawFeature(vectors, OpenLayers.Handler.RegularPolygon, { handlerOptions: { sides: 40} }),
        line: new OpenLayers.Control.DrawFeature(vectors, OpenLayers.Handler.Path),
        polygon: new OpenLayers.Control.DrawFeature(vectors, OpenLayers.Handler.Polygon),
        regular: new OpenLayers.Control.DrawFeature(vectors,
                            OpenLayers.Handler.RegularPolygon,
                            { handlerOptions: { sides: 5} }
                            ),
        modify: new OpenLayers.Control.ModifyFeature(vectors)
    };

    for (var key in mycontrols) {
        map.addControl(mycontrols[key]);
    }

    document.getElementById('noneToggle').checked = true;

    map.events.register('zoomend', this, function (event) {
        if ($('#noneToggle').attr('checked')) {
            var zLevel = map.getZoom();
            
            if (zLevel >= 14 && !clusterDisabled) {
                clusterDisabled = true;
                strategy.deactivate();
                vectors.removeAllFeatures();
                vectors.addFeatures(visibleGeoLandmarkFeature);
            }
            else if (zLevel < 14 && clusterDisabled) {
                clusterDisabled = false;
                strategy.activate();
                vectors.removeAllFeatures();
                vectors.addFeatures(visibleGeoLandmarkFeature);
            }

            //strategy.activate();
            //markers.removeAllFeatures();
            //markers.addFeatures(vehicleFeatures);
        }

    });

    /*var layerswitcher = map.getControlsByClass("OpenLayers.Control.LayerSwitcher")[0];

    //layerswitcher.events.register("mouseout", layerswitcher, function () { alert('mouse out')});
    $('#' + layerswitcher.id).mouseout(function () {
        alert('mouse out');
    });*/


    // Get control of the right-click event:
    document.getElementById('map').oncontextmenu = function (e) {

        e = e ? e : window.event;

        try {
            var target = e.target || e.srcElement;
            if ($(target).parents('.olPopup').length > 0) return;
        }
        catch (ex) { }
        var position = map.getLonLatFromViewPortPx(new OpenLayers.Pixel(e.clientX - 1, e.clientY - 1));
        var proj = new OpenLayers.Projection("EPSG:4326");
        position.transform(map.getProjectionObject(), proj);

        var lat = position.lat;
        var lon = position.lon;

        position.transform(proj, map.getProjectionObject());

        if (contextPopup)
            map.removePopup(contextPopup);

        if (searchMarkerPopup)
            map.removePopup(searchMarkerPopup);

        var content = '<div id="contextMenuStreetInfo"><div id="contextMenuStreetAddress">Loading address...</div>';
        content += '<div>Road Speed: <span id="contextPostedSpeed">... </span>';
        content += ', Function Class: <span id="contextFunctionClass">... </span>';
        content += ', Speed Category: <span id="contextSpeedCategory">... </span></div>';
        content += '<div style="color: #999999">' + lat.toFixed(5) + ', ' + lon.toFixed(5) + '</div></div>';
        content += PointPopupForm(lon.toFixed(5), lat.toFixed(5));

        contextPopup = new OpenLayers.Popup.Anchored("contextmenu",
                   position,
                   new OpenLayers.Size(420, 110),
                   content,
                   null, true, null);

        contextPopup.setBorder("1px solid #cccccc");

        searchMarkerPopup = contextPopup;

        map.addPopup(contextPopup);

        $('#ClosestVehiclesRadius').val(ClosestVehiclesRadius);
        $('#ClosestVehiclesNumOfVehicles').val(ClosestVehiclesNumOfVehicles);
        $('#SearchHistoryTime').val(SearchHistoryTime);
        $('#SearchHistoryDate').val(SearchHistoryDate);
        $('#SearchHistoryRadius').val(SearchHistoryRadius);
        $('#SearchHistoryMinutes').val(SearchHistoryMinutes);

        $.ajax({
            type: 'POST',
            url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetGeoInfo',
            data: JSON.stringify({ lon: lon, lat: lat }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var r = eval('(' + msg.d + ')');
                $('#contextMenuStreetAddress').html(r.StreetAddress);
                $('#contextPostedSpeed').html(r.RoadSpeed);
                $('#contextFunctionClass').html(r.FunctionClass);
                $('#contextSpeedCategory').html(r.SpeedCategory);
            },
            error: function (msg) {
                alert(alertErrorText); //res
            }
        });
        if (e.preventDefault) e.preventDefault(); // For non-IE browsers.
        else return false; // For IE browsers.
    };

}

function cancelPolygon(popup, feature) {
    map.removePopup(popup);
    vectors.removeFeatures(feature);

    currentEditFeature = null;
    currentEditPopup = null;
}

function cancelEditPolygon(popup, feature) {
    map.removePopup(popup);
    selectControl.unselect(selectedFeature);
}

// Changes for TimeZone Feature start
function savePolygon_NewTZ(b, popup, feature) {

    var f = $(b).closest("form").serialize();

    //map.removePopup(popup);
    $.ajax({
        type: 'GET',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/AddNewGeozoneLandmark_NewTZ',
        //data: $(b).closest("form").serialize(),
        //data: $(b).closest("form").find("input[value][id != '__VIEWSTATE'][id != '__EVENTVALIDATION'], select, textarea").serialize(),
        data: $(b).closest("form").find("input[id != '__VIEWSTATE'][id != '__EVENTVALIDATION'], select, textarea").serialize(),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            var r = eval('(' + msg.d + ')');
            if (r.status == 200) {
                //alert(r.message);
                $('#messagebar').html(r.message).show().delay(2000).fadeOut(1000);
                feature.fid = r.objectType + ":::" + r.geozonelandmarkname;
                //alert(r.objectType + ':' + r.isPublic);
                feature.attributes.Public = r.isPublic;

                feature.GeoDirection = $(b).closest("form").find('#cboDirection option:selected').text();
                feature.SeverityName = $(b).closest("form").find('#cboGeoZoneSeverity option:selected').text();

                if (r.objectType == "Landmark" || r.objectType == "LandmarkCircle") {
                    feature.CategoryId = $(b).closest("form").find('#ddlCategory option:selected').val();
                    feature.CategoryName = $(b).closest("form").find('#ddlCategory option:selected').text();
                }

                if (r.isNew == 1) {
                    geoLandmarkFeatures.push(feature);
                    visibleGeoLandmarkFeature.push(feature);
                }
                try { selectControl.unselect(selectedFeature); }
                catch (e) { }
                map.removePopup(popup);
                btnMapToolPan.fireEvent('click', btnMapToolPan);
            }
            else {
                //alert("Failed to create the Landmark/Geozone");
                if (r.message == "")
                    alert(messagebarSavePolygonFailedText);
                else
                    alert(r.message);

                //vectors.removeFeatures(feature);
            }

            if (r.objectType == "Geozone" && parent.geozonegridloaded) {
                parent.loadGeozones();
            }
            else if ((r.objectType == "Landmark" || r.objectType == "LandmarkCircle") && parent.landmarkgridloaded) {
                parent.loadLandmarks();
            }

            //map.removePopup(popup);
        },
        error: function (msg) {
            //map.removePopup(popup);
            alert(alertErrorText); //res
            vectors.removeFeatures(feature);
        }
    });

    currentEditFeature = null;
    currentEditPopup = null;

}


// Changes for TimeZone Feature end

function savePolygon(b, popup, feature) {
    
    var f = $(b).closest("form").serialize();
    
    //map.removePopup(popup);
    $.ajax({
        type: 'GET',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/AddNewGeozoneLandmark',
        //data: $(b).closest("form").serialize(),
        //data: $(b).closest("form").find("input[value][id != '__VIEWSTATE'][id != '__EVENTVALIDATION'], select, textarea").serialize(),
        data: $(b).closest("form").find("input[id != '__VIEWSTATE'][id != '__EVENTVALIDATION'], select, textarea").serialize(),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            
            var r = eval('(' + msg.d + ')');
            if (r.status == 200) {
                //alert(r.message);
                $('#messagebar').html(r.message).show().delay(2000).fadeOut(1000);
                feature.fid = r.objectType + ":::" + r.geozonelandmarkname;
                //alert(r.objectType + ':' + r.isPublic);
                feature.attributes.Public = r.isPublic;

                feature.GeoDirection = $(b).closest("form").find('#cboDirection option:selected').text();
                feature.SeverityName = $(b).closest("form").find('#cboGeoZoneSeverity option:selected').text();

                if (r.objectType == "Landmark" || r.objectType == "LandmarkCircle")
                {
                    feature.CategoryId = $(b).closest("form").find('#ddlCategory option:selected').val();
                    feature.CategoryName = $(b).closest("form").find('#ddlCategory option:selected').text();
                }

                if (r.isNew == 1) {
                    geoLandmarkFeatures.push(feature);
                    visibleGeoLandmarkFeature.push(feature);
                }
                try { selectControl.unselect(selectedFeature); }
                catch (e) { }
                map.removePopup(popup);
            }
            else {
                //alert("Failed to create the Landmark/Geozone");
                if (r.message == "")
                    alert(messagebarSavePolygonFailedText);                    
                else
                    alert(r.message);                    

                //vectors.removeFeatures(feature);
            }

            if (r.objectType == "Geozone" && parent.geozonegridloaded) {
                parent.loadGeozones();
            }
            else if ((r.objectType == "Landmark" || r.objectType == "LandmarkCircle") && parent.landmarkgridloaded) {
                parent.loadLandmarks();
            }

            //map.removePopup(popup);
        },
        error: function (msg) {
            //map.removePopup(popup);
            alert(alertErrorText); //res
            vectors.removeFeatures(feature);
        }
    });

    currentEditFeature = null;
    currentEditPopup = null;

}

function clearEditForm() {
    if (currentEditPopup != null)
        map.removePopup(currentEditPopup);

    if (currentEditFeature != null)
        vectors.removeFeatures(currentEditFeature);

    currentEditFeature = null;
    currentEditPopup = null;
}

function deletePolygon(otype, oid, popup, feature) {

    $.ajax({
        type: 'POST',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/DeleteGeozoneLandmark',
        data: JSON.stringify({ otype: otype, oid: oid }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            var r = eval('(' + msg.d + ')');
            if (r.status == 200) {
                map.removePopup(popup);
                vectors.removeFeatures(feature);
                geoLandmarkFeatures.remove(feature);
                visibleGeoLandmarkFeature.remove(feature);

                if (otype == "Geozone" && parent.geozonegridloaded) {
                    parent.loadGeozones();
                }
                else if ((otype == "Landmark" || otype == "LandmarkCircle") && parent.landmarkgridloaded) {
                    parent.loadLandmarks();
                }
                $('#messagebar').html(r.message).show().delay(2000).fadeOut(1000);
            }
            else {
                alert(alertDeletePolygonFailedText); //res
            }
        },
        error: function (msg) {
            alert(alertErrorText); //res
        }
    });
}

function deleteThisPolygon(b, popup, feature) {
    var r = confirm(Res_DeleteConfirmation);
    if (r == false) {
        return;
    }

    var f = $(b).closest("form");
    var otype = f.find('#geoassettype').val();
    var oid = f.find('#geoassetname').val();
    deletePolygon(otype, oid, popup, feature);
}

function pushToUndolist(otype, oid, pointSets, centerpoint, radius) {
    if (centerpoint == undefined) centerpoint = '';
    if (radius == undefined) radius = '';

    var item = { otype: otype, oid: oid, pointSets: previousPointSets, centerpoint: previousCenterPoint, radius: previousRadius };
    undolist.push(item);
    if (undolist.length > 0) {
        $('#undonum').html(undolist.length);
        $('#undobar').show();
    }

    var itemForSave = { otype: otype, oid: oid, pointSets: pointSets, centerpoint: centerpoint, radius: radius };

    var addedtochangeslist = false;
    for (var i = 0; i < changeslist.length; i++) {
        if (changeslist[i].otype == otype && changeslist[i].oid == oid) {
            changeslist[i].pointSets = pointSets;
            changeslist[i].centerpoint = centerpoint;
            changeslist[i].radius = radius;
            addedtochangeslist = true;
        }
    }
    if (!addedtochangeslist)
        changeslist.push(itemForSave);
}

// Changes for TimeZone Feature start

function editPolygon_NewTZ(otype, oid, pointSets, centerpoint, radius) {
    //Edit polygon
    $('#undolink').hide();
    $('#undosaving').show();

    $.ajax({
        type: 'POST',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/EditGeozoneLandmark_NewTZ',
        data: JSON.stringify({ otype: otype, oid: oid, pointSets: pointSets, centerpoint: centerpoint, newradius: radius }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            var r = eval('(' + msg.d + ')');
            if (r.status == 200) {

            }
            else {
                //$('#undosaving').hide();
                //$('#undolink').show();
                alert(alertEditPolygonFailedText); //res
            }
        },
        error: function (msg) {
            //$('#undosaving').hide();
            //$('#undolink').show();
            $('#savechanges').show();
            alert(alertErrorText); //res
        }
    });

}

// Changes for TimeZone Feature end

function editPolygon(otype, oid, pointSets) {
    //Edit polygon
    $('#undolink').hide();
    $('#undosaving').show();

    $.ajax({
        type: 'POST',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/EditGeozoneLandmark',
        data: JSON.stringify({ otype: otype, oid: oid, pointSets: pointSets }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            var r = eval('(' + msg.d + ')');
            if (r.status == 200) {

            }
            else {
                //$('#undosaving').hide();
                //$('#undolink').show();
                alert(alertEditPolygonFailedText); //res
            }
        },
        error: function (msg) {
            //$('#undosaving').hide();
            //$('#undolink').show();
            $('#savechanges').show();
            alert(alertErrorText); //res
        }
    });

}


// Changes for TimeZone Feature start
function DrawGeozone_NewTZ() {
    try {
        $.ajax({
            type: 'GET',
            url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetAllGoezons_NewTZ',
            contentType: "application/json; charset=utf-8",
            async: true,
            success: function (response) {
                try {
                    var resultMessage = response.d;

                    if (resultMessage == "1") geozoneLoaded = true;

                    if (resultMessage != "-1" && resultMessage != "1" && resultMessage != "0") {
                        var proj = new OpenLayers.Projection("EPSG:4326");
                        var geoZonesCollection = eval("(" + resultMessage + ")");

                        for (var geoZone_i = 0; geoZone_i < geoZonesCollection.length; geoZone_i++) {
                            try {
                                var geozonestring = '';
                                var geoZonesPoints = eval(geoZonesCollection[geoZone_i].coords);
                                try {
                                    if (geoZonesPoints.length > 0) {
                                        for (var geoZone_j = 0; geoZone_j < geoZonesPoints.length; geoZone_j++) {
                                            var point = new OpenLayers.LonLat(geoZonesPoints[geoZone_j][1], geoZonesPoints[geoZone_j][0]);
                                            point.transform(proj, map.getProjectionObject());
                                            if (geozonestring == '')
                                                geozonestring = point.lon.toString() + " " + point.lat.toString();
                                            else
                                                geozonestring = geozonestring + ", " + point.lon.toString() + " " + point.lat.toString();
                                        }
                                        geozonestring = "POLYGON((" + geozonestring + "))";

                                        var feature = new OpenLayers.Feature.Vector(
                                            OpenLayers.Geometry.fromWKT(
                                            geozonestring
                                             )
                                         );
                                        feature.fid = "Geozone:::" + geoZonesCollection[geoZone_i].geozoneName;
                                        //feature.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc };
                                        feature.Assigned = geoZonesCollection[geoZone_i].Assigned;
                                        feature.GeoDescription = geoZonesCollection[geoZone_i].desc;
                                        feature.GeoDirection = geoZonesCollection[geoZone_i].Direction;
                                        feature.SeverityName = geoZonesCollection[geoZone_i].SeverityName;
                                        feature.GeozoneID = geoZonesCollection[geoZone_i].id;
                                        feature.attributes = { Public: geoZonesCollection[geoZone_i].Public };
                                        //alert("assigned?" + feature.Assigned);
                                        //alert(geoZonesCollection[geoZone_i].Assigned);
                                        //vectors.addFeatures([feature]);
                                        geoLandmarkFeatures.push(feature);
                                        visibleGeoLandmarkFeature.push(feature);
                                    }

                                }
                                catch (err) { }
                            }
                            catch (err) { }
                        }
                        geozoneLoaded = true;
                        if (visibleGeoLandmarkFeature.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(visibleGeoLandmarkFeature);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (response) {
                alert(alertGeozoneLoadFailedText + response.toString()); //res
            }
        });

    }
    catch (err) {
        var s = err;
    }
}


// Changes for TimeZone Feature end
//By Devin Begin
function DrawGeozone() {
    try {
        $.ajax({
            type: 'GET',
            url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetAllGoezons',
            contentType: "application/json; charset=utf-8",
            async: true,
            success: function (response) {
                try {
                    var resultMessage = response.d;

                    if (resultMessage == "1") geozoneLoaded = true;

                    if (resultMessage != "-1" && resultMessage != "1" && resultMessage != "0") {
                        var proj = new OpenLayers.Projection("EPSG:4326");
                        var geoZonesCollection = eval("(" + resultMessage + ")");

                        for (var geoZone_i = 0; geoZone_i < geoZonesCollection.length; geoZone_i++) {
                            try {
                                var geozonestring = '';
                                var geoZonesPoints = eval(geoZonesCollection[geoZone_i].coords);
                                try {
                                    if (geoZonesPoints.length > 0) {
                                        for (var geoZone_j = 0; geoZone_j < geoZonesPoints.length; geoZone_j++) {
                                            var point = new OpenLayers.LonLat(geoZonesPoints[geoZone_j][1], geoZonesPoints[geoZone_j][0]);
                                            point.transform(proj, map.getProjectionObject());
                                            if (geozonestring == '')
                                                geozonestring = point.lon.toString() + " " + point.lat.toString();
                                            else
                                                geozonestring = geozonestring + ", " + point.lon.toString() + " " + point.lat.toString();
                                        }
                                        geozonestring = "POLYGON((" + geozonestring + "))";

                                        var feature = new OpenLayers.Feature.Vector(
                                            OpenLayers.Geometry.fromWKT(
                                            geozonestring
                                             )
                                         );
                                        feature.fid = "Geozone:::" + geoZonesCollection[geoZone_i].geozoneName;
                                        //feature.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc };
                                        feature.Assigned = geoZonesCollection[geoZone_i].Assigned;
                                        feature.GeoDescription = geoZonesCollection[geoZone_i].desc;
                                        feature.GeoDirection = geoZonesCollection[geoZone_i].Direction;
                                        feature.SeverityName = geoZonesCollection[geoZone_i].SeverityName;
                                        feature.GeozoneID = geoZonesCollection[geoZone_i].id;
                                        feature.attributes = { Public: geoZonesCollection[geoZone_i].Public };
                                        //alert("assigned?" + feature.Assigned);
                                        //alert(geoZonesCollection[geoZone_i].Assigned);
                                        //vectors.addFeatures([feature]);
                                        geoLandmarkFeatures.push(feature);
                                        visibleGeoLandmarkFeature.push(feature);
                                    }

                                }
                                catch (err) { }
                            }
                            catch (err) { }
                        }
                        geozoneLoaded = true;
                        if (visibleGeoLandmarkFeature.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(visibleGeoLandmarkFeature);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (response) {
                alert(alertGeozoneLoadFailedText + response.toString()); //res
            }
        });

    }
    catch (err) {
        var s = err;
    }
}
//End

// Changes for TimeZone Feature start

function DrawLandmark_NewTZ() {
    try {
        $.ajax({
            type: 'GET',
            url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetAllLandmarks_NewTZ',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                try {
                    var landmarkCollection = eval('(' + msg.d + ')');
                    var noresult = false;
                    if (landmarkCollection.status != undefined)
                        noresult = true;

                    if (!noresult) {
                        var proj = new OpenLayers.Projection("EPSG:4326");
                        var in_options = {
                            'internalProjection': map.baseLayer.projection,
                            'externalProjection': proj
                        };

                        for (var landmark_i = 0; landmark_i < landmarkCollection.length; landmark_i++) {
                            try {
                                var landmarkstring = '';
                                var radius = parseInt(landmarkCollection[landmark_i].radius);
                                var feature = null;
                                if (radius > 0) {
                                    var point = new OpenLayers.LonLat(landmarkCollection[landmark_i].lon, landmarkCollection[landmark_i].lat);
                                    point.transform(proj, map.getProjectionObject());
                                    var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
                                    var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, radius / Math.cos(landmarkCollection[landmark_i].lat * (Math.PI / 180)), 40, 0);
                                    feature = new OpenLayers.Feature.Vector(
                                                circle
                                             );
                                    feature.fid = "LandmarkCircle:::" + landmarkCollection[landmark_i].LandmarkName; //res tbd

                                    feature.attributes = { Public: landmarkCollection[landmark_i].Public };
                                    //geoLandmarkFeatures.push(feature);


                                }
                                else {
                                    try {

                                        landmarkstring = landmarkCollection[landmark_i].coords;

                                        if (landmarkstring.length > 60) {
                                            feature = new OpenLayers.Format.WKT(in_options).read(landmarkstring);

                                            feature.fid = "Landmark:::" + landmarkCollection[landmark_i].LandmarkName; //res tbd
                                            feature.attributes = { Public: landmarkCollection[landmark_i].Public };
                                        }
                                        //geoLandmarkFeatures.push(feature);                                  

                                    }
                                    catch (err) { }
                                }
                                if (feature) {
                                    feature.landmarkDescription = landmarkCollection[landmark_i].desc;
                                    feature.StreetAddress = landmarkCollection[landmark_i].StreetAddress;
                                    feature.Email = landmarkCollection[landmark_i].Email;
                                    feature.ContactPhoneNum = landmarkCollection[landmark_i].ContactPhoneNum;
                                    feature.radius = landmarkCollection[landmark_i].radius;
                                    feature.CategoryName = landmarkCollection[landmark_i].CategoryName;
                                    feature.CategoryId = landmarkCollection[landmark_i].CategoryId;
                                    feature.LandmarkId = landmarkCollection[landmark_i].LandmarkId;
                                    geoLandmarkFeatures.push(feature);
                                    visibleGeoLandmarkFeature.push(feature);
                                }

                            }
                            catch (err) { }
                        }
                        landmarkLoaded = true;
                        if (visibleGeoLandmarkFeature.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(visibleGeoLandmarkFeature);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (msg) {
                alert(alertLandmarkLoadFailedText); //res
            }
        });

    }
    catch (err) {
        var s = err;
    }
}

// Changes for TimeZone Feature end

function DrawLandmark() {
    try {
        $.ajax({
            type: 'GET',
            url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetAllLandmarks',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                try {
                    var landmarkCollection = eval('(' + msg.d + ')');
                    var noresult = false;
                    if (landmarkCollection.status != undefined)
                        noresult = true;

                    if (!noresult) {
                        var proj = new OpenLayers.Projection("EPSG:4326");
                        var in_options = {
                            'internalProjection': map.baseLayer.projection,
                            'externalProjection': proj
                        };

                        for (var landmark_i = 0; landmark_i < landmarkCollection.length; landmark_i++) {
                            try {
                                var landmarkstring = '';
                                var radius = parseInt(landmarkCollection[landmark_i].radius);
                                var feature = null;
                                if (radius > 0) {
                                    var point = new OpenLayers.LonLat(landmarkCollection[landmark_i].lon, landmarkCollection[landmark_i].lat);
                                    point.transform(proj, map.getProjectionObject());
                                    var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
                                    var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, radius / Math.cos(landmarkCollection[landmark_i].lat * (Math.PI / 180)), 40, 0);
                                    feature = new OpenLayers.Feature.Vector(
                                                circle
                                             );
                                    feature.fid = "LandmarkCircle:::" + landmarkCollection[landmark_i].LandmarkName; //res tbd

                                    feature.attributes = { Public: landmarkCollection[landmark_i].Public };
                                    //geoLandmarkFeatures.push(feature);


                                }
                                else {
                                    try {

                                        landmarkstring = landmarkCollection[landmark_i].coords;
                                        
                                        if (landmarkstring.length > 60) {
                                            feature = new OpenLayers.Format.WKT(in_options).read(landmarkstring);

                                            feature.fid = "Landmark:::" + landmarkCollection[landmark_i].LandmarkName; //res tbd
                                            feature.attributes = { Public: landmarkCollection[landmark_i].Public }; 
                                        }
                                        //geoLandmarkFeatures.push(feature);                                  

                                    }
                                    catch (err) { }
                                }
                                if (feature) {
                                    feature.landmarkDescription = landmarkCollection[landmark_i].desc;
                                    feature.StreetAddress = landmarkCollection[landmark_i].StreetAddress;
                                    feature.Email = landmarkCollection[landmark_i].Email;
                                    feature.ContactPhoneNum = landmarkCollection[landmark_i].ContactPhoneNum;
                                    feature.radius = landmarkCollection[landmark_i].radius;
                                    feature.CategoryName = landmarkCollection[landmark_i].CategoryName;
                                    feature.CategoryId = landmarkCollection[landmark_i].CategoryId;
                                    feature.LandmarkId = landmarkCollection[landmark_i].LandmarkId;
                                    geoLandmarkFeatures.push(feature);
                                    visibleGeoLandmarkFeature.push(feature);
                                }

                            }
                            catch (err) { }
                        }
                        landmarkLoaded = true;
                        if (visibleGeoLandmarkFeature.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(visibleGeoLandmarkFeature);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (msg) {
                alert(alertLandmarkLoadFailedText); //res
            }
        });

    }
    catch (err) {
        var s = err;
    }
}

function showAlarm(AlarmVehicleDescription, AlarmLon, AlarmLat, landmarkId) {
    //alert('l: ' + landmarkId);
    try {
        var arr = landmarkId.split(",");
        routeGeometry.removeAllFeatures();

        var bounds = new OpenLayers.Bounds();

        var newLoc = transformCoords(AlarmLon, AlarmLat);
        var vicon = new OpenLayers.Icon("../New20x20/" + "RedCircle.ico");
        var viconUrl = "../New20x20/" + "RedCircle.ico";
        
        var routeGeometryFeatures = [];
        var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
        var marker = new OpenLayers.Feature.Vector(point);

        //bounds.extend(newLoc.lon, newLoc.lat);
        bounds.extend(newLoc);
        
        marker.attributes = { vicon: vicon, icon: viconUrl, location: newLoc, Description: AlarmVehicleDescription };
        routeGeometryFeatures.push(marker);

        if (landmarkId != 0) {
            $.ajax({
                type: 'GET',
                //url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetLandmarkGeometry?landmarkId=' + landmarkId,
                url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetLandmarkGeometry',
                data: { landmarkId: '"' + landmarkId + '"' },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    try {
                        var landmarkCollection = eval('(' + msg.d + ')');




                        /*for (i = 0; i < fleet.length; i++) {
                        try {
                        bounds.extend(transformCoords(fleet[i].Longitude, fleet[i].Latitude));
                        }
                        catch (err1) {
                        }
                        }*/
                        //map.zoomToExtent(bounds, true);

                        for (var i = 0; i < landmarkCollection.length; i++) {
                            //var lg = eval('(' + msg.d + ')');
                            var lg = eval('(' + landmarkCollection[i] + ')');

                            if (lg.type == 'Polygon' && lg.coordinates.length > 0) {

                                var polygonstring = 'POLYGON((' + lg.coordinates[0].join(';').replace(/,/g, ' ').replace(/;/g, ',') + '))';
                                var proj = new OpenLayers.Projection("EPSG:4326");
                                var in_options = {
                                    'internalProjection': map.baseLayer.projection,
                                    'externalProjection': proj
                                };

                                feature = new OpenLayers.Format.WKT(in_options).read(polygonstring);

                                if (feature) {
                                    routeGeometryFeatures.push(feature);
                                    bounds.extend(feature.geometry.getBounds());
                                }

                            }
                        }

                        if (routeGeometryFeatures.length > 0) {
                            routeGeometry.addFeatures(routeGeometryFeatures);
                            map.zoomToExtent(bounds, true);
                            map.zoomTo(map.getZoom() - 1);
                        }

                    }
                    catch (err) { }

                },
                error: function (msg) {
                    alert('Failed to load Landmark Geometry.');
                }
            });
        }
        else {
            if (routeGeometryFeatures.length > 0) {
                routeGeometry.addFeatures(routeGeometryFeatures);
                map.zoomToExtent(bounds, true);
                map.zoomTo(map.getZoom() - 1);
            }
        }
        

    }
    catch (err) {
        var s = err;
    }
}

function mapRoute(routeId, serviceConfigId) {
    //alert('l: ' + landmarkId);
    try {
        //var arr = landmarkId.split(",");
        routeGeometry.removeAllFeatures();

        var bounds = new OpenLayers.Bounds();
        var routeGeometryFeatures = [];

        /*var newLoc = transformCoords(AlarmLon, AlarmLat);
        var vicon = new OpenLayers.Icon("../New20x20/" + "RedCircle.ico");
        var viconUrl = "../New20x20/" + "RedCircle.ico";

        
        var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
        var marker = new OpenLayers.Feature.Vector(point);

        //bounds.extend(newLoc.lon, newLoc.lat);
        bounds.extend(newLoc);

        marker.attributes = { vicon: vicon, icon: viconUrl, location: newLoc, Description: AlarmVehicleDescription };
        routeGeometryFeatures.push(marker);
        */
        if (routeId != 0) {
            $.ajax({
                type: 'GET',
                //url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetLandmarkGeometry?landmarkId=' + landmarkId,
                url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/GetLandmarkGeometryWithVehicles',
                data: { landmarkId: '"' + routeId + '"', serviceConfigId: '"' + serviceConfigId + '"' },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    try {

                        var landmarkCollection = eval('(' + msg.d + ')');



                        /*for (i = 0; i < fleet.length; i++) {
                        try {
                        bounds.extend(transformCoords(fleet[i].Longitude, fleet[i].Latitude));
                        }
                        catch (err1) {
                        }
                        }*/
                        //map.zoomToExtent(bounds, true);



                        for (var i = 0; i < landmarkCollection.length; i++) {
                            //var lg = eval('(' + msg.d + ')');
                            //alert(landmarkCollection[i].Vehicles[0].lon);

                            ////////////////////////////////////
                            var markers = [];
                            for (var j = 0; j < landmarkCollection[i].Vehicles.length; j++) {

                                var newLoc = transformCoords(landmarkCollection[i].Vehicles[j].lon, landmarkCollection[i].Vehicles[j].lat);
                                var vicon = new OpenLayers.Icon("../New20x20/" + "RedCircle.ico");
                                var viconUrl = "../New20x20/" + "RedCircle.ico";


                                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                                var marker = new OpenLayers.Feature.Vector(point);

                                //bounds.extend(newLoc.lon, newLoc.lat);
                                //bounds.extend(newLoc);

                                marker.attributes = { vicon: vicon, icon: viconUrl, location: newLoc, Description: landmarkCollection[i].Vehicles[j].Description };
                                markers.push(marker);
                            }
                            
                            //routeGeometryFeatures.push(markers)
                            routeGeometry.addFeatures(markers);

                            //////////////////////////////////////////

                            var lg = eval('(' + landmarkCollection[i].RoutePoints + ')');

                            if (lg.type == 'Polygon' && lg.coordinates.length > 0) {

                                var polygonstring = 'POLYGON((' + lg.coordinates[0].join(';').replace(/,/g, ' ').replace(/;/g, ',') + '))';

                                var proj = new OpenLayers.Projection("EPSG:4326");
                                var in_options = {
                                    'internalProjection': map.baseLayer.projection,
                                    'externalProjection': proj
                                };

                                feature = new OpenLayers.Format.WKT(in_options).read(polygonstring);

                                if (feature) {
                                    routeGeometryFeatures.push(feature);
                                    bounds.extend(feature.geometry.getBounds());
                                }

                            }
                        }

                        if (routeGeometryFeatures.length > 0) {
                            routeGeometry.addFeatures(routeGeometryFeatures);
                            map.zoomToExtent(bounds, true);
                            map.zoomTo(map.getZoom() - 1);
                        }

                    }
                    catch (err) { alert(err); }

                },
                error: function (msg) {
                    alert('Failed to load Landmark Geometry.');
                }
            });
        }
        else {
            if (routeGeometryFeatures.length > 0) {
                routeGeometry.addFeatures(routeGeometryFeatures);
                map.zoomToExtent(bounds, true);
                map.zoomTo(map.getZoom() - 1);
            }
        }


    }
    catch (err) {
        var s = err;
    }
}

function clearLandmarkGeometry() {
    routeGeometry.removeAllFeatures();
}

function DrawWorkorder() {
    /*try {
        $.ajax({
            type: 'GET',
            url: rootpath + 'MapNew/AddNewMapGoezone.asmx/GetAllLandmarks',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                try {
                    var landmarkCollection = eval('(' + msg.d + ')');
                    var noresult = false;
                    if (landmarkCollection.status != undefined)
                        noresult = true;

                    if (!noresult) {
                        var proj = new OpenLayers.Projection("EPSG:4326");

                        for (var landmark_i = 0; landmark_i < landmarkCollection.length; landmark_i++) {
                            try {
                                var landmarkstring = '';
                                var radius = parseInt(landmarkCollection[landmark_i].radius);
                                if (radius > 0) {
                                    var point = new OpenLayers.LonLat(landmarkCollection[landmark_i].lon, landmarkCollection[landmark_i].lat);
                                    point.transform(proj, map.getProjectionObject());
                                    var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
                                    var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, radius, 40, 0);
                                    var feature = new OpenLayers.Feature.Vector(
                                                circle
                                             );
                                    feature.fid = "LandmarkCircle:::" + landmarkCollection[landmark_i].LandmarkName;
                                    //vectors.addFeatures([feature]);
                                    workorderFeatures.push(feature);

                                    //circle.fid = "LandmarkCircle:::" + landmarkCollection[landmark_i].LandmarkName;
                                    //vectors.addFeatures([circle]);
                                }
                                else {
                                    var landmarkPoints = eval(landmarkCollection[landmark_i].coords);
                                    try {
                                        if (landmarkPoints.length > 0) {
                                            for (var landmark_j = 0; landmark_j < landmarkPoints.length; landmark_j++) {
                                                var point = new OpenLayers.LonLat(landmarkPoints[landmark_j][1], landmarkPoints[landmark_j][0]);
                                                point.transform(proj, map.getProjectionObject());
                                                if (landmarkstring == '')
                                                    landmarkstring = point.lon.toString() + " " + point.lat.toString();
                                                else
                                                    landmarkstring = landmarkstring + ", " + point.lon.toString() + " " + point.lat.toString();
                                            }
                                            landmarkstring = "POLYGON((" + landmarkstring + "))";

                                            var feature = new OpenLayers.Feature.Vector(
                                                OpenLayers.Geometry.fromWKT(landmarkstring)
                                             );
                                            feature.fid = "Landmark:::" + landmarkCollection[landmark_i].LandmarkName;
                                            //vectors.addFeatures([feature]);
                                            workorderFeatures.push(feature);
                                        }

                                    }
                                    catch (err) { }
                                }

                            }
                            catch (err) { }
                        }
                        if (workorderFeatures.length > 0) {
                            workorderLayer.removeAllFeatures();
                            workorderLayer.addFeatures(workorderFeatures);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (msg) {
                alert('Failed to load workorders.');
            }
        });

    }
    catch (err) {
        var s = err;
    }*/
}

function msieversion() {
    var ua = window.navigator.userAgent
    var msie = ua.indexOf("MSIE ") // res tbd
    if (msie > 0)      // If Internet Explorer, return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf(".", msie)))
    else                 // If another browser, return 0
        return 0
}


function showInfo(evt) {
    if (evt.features && evt.features.length) {
        highlightLayer.destroyFeatures();
        highlightLayer.addFeatures(evt.features);
        highlightLayer.redraw();

//        if (parent.VehicleClustering) {
//        }
        //        else if (overlaysettings.vehicleDrivers) {
//            vehicleDriversLayer.destroyFeatures();
//            vehicleDriversLayer.addFeatures(vehicleDrivers);
//            vehicleDriversLayer.redraw();
//        }
        //        else if (overlaysettings.vehiclenames) {
//            vehiclenamesLayer.destroyFeatures();
//            vehiclenamesLayer.addFeatures(vehiclenames);
//            vehiclenamesLayer.redraw();
//        }
    }
    else {
        $('responseText').innerHTML = evt.text;
    }
}

function zoomMap(fleet) {
    try {
        if (fleet != null) {
            fleet = eval(fleet);
            if (fleet != null && fleet != "") {
                var bounds = new OpenLayers.Bounds();
                for (i = 0; i < fleet.length; i++) {
                    try {
                        bounds.extend(transformCoords(fleet[i].Longitude, fleet[i].Latitude));
                    }
                    catch (err1) {
                    }
                }
                map.zoomToExtent(bounds, true);                
                if (fleet.length == 1)
                    map.zoomTo(14);
                else
                    map.zoomTo(map.getZoom() - 2);
                
            }
        }
    }
    catch (err) {
    }
}
function showVehiclePopups() {
    popup = new OpenLayers.Popup.FramedCloud("featurePopup", this.location,
   new OpenLayers.Size(100, 100),
   this.featureHTML,
   this.icon, true, null);
    map.addPopup(popup, true);
    var element = document.getElementById("popup" + this.LicensePlate);
    var lcPlate = this.LicensePlate;
    /*Ext.create('Ext.Button', {
    renderTo: element,
    text: 'Vehicle Info',
    cls: 'cmbfonts',
    handler: function () {
    try {
    var url = "./Map/frmSensorMain.aspx?LicensePlate=" + lcPlate;
    var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
    var h = $(parent.document).height();
    parent.openNewWindow('Vehicle Info', urlToLoad, 560, h - 20);
    }
    catch (err) {
    }
    }
    });

    Ext.create('Ext.Button', {
    renderTo: element,
    text: 'New Message',
    cls: 'cmbfonts',
    handler: function () {
    try {
    var url = "./Messages/frmNewMessageMain.aspx?LicensePlate=" + lcPlate;
    var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
    parent.openNewWindow('New Message', urlToLoad, 560, 640);
    }
    catch (err) {
    }
    }
    });*/

}

function closeVehiclePopups() {
    if (donotclosevehiclepopups) return;
    var currentPopups = map.popups;
    for (i = 0; i < currentPopups.length; i++) {
        if (currentPopups[i].id != "polygonformpopup" && currentPopups[i].id != "addressMarkerPopup" && currentPopups[i].id != "contextmenu")
            map.removePopup(currentPopups[i]);
    }
}

function removeAllPopups() {
    var currentPopups = map.popups;
    for (i = 0; i < currentPopups.length; i++) {
        map.removePopup(currentPopups[i]);
    }
}

function removePoloygonPopups() {
    var currentPopups = map.popups;
    for (i = 0; i < currentPopups.length; i++) {
        if (currentPopups[i].id == "polygonformpopup")
            map.removePopup(currentPopups[i]);
    }
}

function removeMarkersOnMap() {
    currentMarkers = new Array();
    if (markers) {
        markers.removeAllFeatures();
        if (map.getLayerIndex(markers) != -1) {
            map.removeLayer(markers);
        }
    }
}

function removeHistoriesOnMap() {
    currentHistories = new Array();
    if (histories) {
        histories.removeAllFeatures();
        if (map.getLayerIndex(histories) != -1) {
            map.removeLayer(histories);
        }
    }
}

function ShowMultipleAssets(fleet, zoomVehicles) {
    zoomVehicles = typeof zoomVehicles !== 'undefined' ? zoomVehicles : true;

    try {
        if (fleet != null && fleet != "") {
            var i = 0;
            allVehicles = fleet;
            var bounds = new OpenLayers.Bounds();
            currentMarkers = new Array();
            currentMarkerIndex = 0;
            if (markers && map) {
                markers.removeAllFeatures();
                if (map.getLayerIndex(markers) != -1) {
                    map.removeLayer(markers);
                }
            }
            vehicleFeatures = [];
            for (i = 0; i < fleet.length; i++) {
                var currentVehicle = fleet[i];
                var vicon = new OpenLayers.Icon("../New20x20/" + currentVehicle.icon);
                var viconUrl = "../New20x20/" + currentVehicle.icon;
                if (currentVehicle.BoxId == '27492') {
                    var size = new OpenLayers.Size(48, 97);
                    vicon = new OpenLayers.Icon("../New20x20/" + "tower.png", size);
                    viconUrl = "../New20x20/" + "tower.png";
                }
                else if (currentVehicle.ImagePath != '' && currentVehicle.ImagePath != undefined) {
                    vicon = new OpenLayers.Icon("../images/CustomIcon/" + currentVehicle.icon);
                    var viconUrl = "../images/CustomIcon/" + currentVehicle.icon;
                }
                var newLoc = transformCoords(currentVehicle.Longitude, currentVehicle.Latitude);
                if (zoomVehicles) {
                    if (fleet.length == 1) {
                        map.setCenter(newLoc, 14);
                    }
                    else {
                        try {
                            if ((Ext.Date.format(currentVehicle.OriginDateTime, parent.userdateformat) >= Ext.Date.format('01/01/2000 00:00 am', parent.userdateformat)) && currentVehicle.Longitude != 0 && currentVehicle.Latitude != 0) {
                                bounds.extend(newLoc);
                            }
                        }
                        catch (ex) {
                        }
                    }
                }

                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                var marker = new OpenLayers.Feature.Vector(point);
                
                //var popupContent = getPopupContent(currentVehicle);

                marker.attributes = { vicon: vicon, icon: viconUrl, location: newLoc, BoxId: currentVehicle.BoxId, Driver: currentVehicle.Driver, Description: currentVehicle.Description };

                currentMarkers[currentMarkerIndex] = marker;
                currentMarkerIndex++;
                //vectorLayer.addFeatures([feature]);
                vehicleFeatures.push(marker);

                //var marker = new OpenLayers.Marker(newLoc, vicon);

                /*var eventObj = new Object();
                eventObj.icon = vicon;
                eventObj.location = newLoc;
                eventObj.LicensePlate = currentVehicle.LicensePlate;
                eventObj.featureHTML = "<h6>" + currentVehicle.Description + "<br />" +                
                    currentVehicle.StreetAddress +
                    "<br />Time: " + currentVehicle.convertedDisplayDate +
                    "<br />Heading: " + currentVehicle.MyHeading +
                    "<br />Speed: " + currentVehicle.CustomSpeed +
                    "<br />Status: " + currentVehicle.VehicleStatus +
                    "<br /><div style='height:25px;margin-top:5px;' id='popup" + currentVehicle.LicensePlate + "'></div>";
                marker.events.register('mousedown', eventObj, showVehiclePopups);
                currentMarkers[currentMarkerIndex] = marker;
                currentMarkerIndex++;*/
                //markers.addMarker(marker);

                var proj = new OpenLayers.Projection("EPSG:4326");
                var point = new OpenLayers.LonLat(currentVehicle.Longitude, currentVehicle.Latitude);
                point.transform(proj, map.getProjectionObject());
                vehicleNames[i] = new OpenLayers.Feature.Vector(
                        new OpenLayers.Geometry.Point(point.lon, point.lat), { name: currentVehicle.Description });
                vehicleDrivers[i] = new OpenLayers.Feature.Vector(
                        new OpenLayers.Geometry.Point(point.lon, point.lat), { name: currentVehicle.Driver });
            }

            //map.addLayer(vectors);
            map.addLayer(markers);

            markers.addFeatures(vehicleFeatures);

//            if (parent.VehicleClustering) {
//            }
            //            else if (overlaysettings.vehicleDrivers) {
//                vehicleDriversLayer.destroyFeatures();
//                vehicleDriversLayer.addFeatures(vehicleDrivers);
//                vehicleDriversLayer.redraw();
//            }
            //            else if (overlaysettings.vehiclenames) {
//                vehiclenamesLayer.destroyFeatures();
//                vehiclenamesLayer.addFeatures(vehicleNames);
//                vehiclenamesLayer.redraw();
//            }

            if (fleet.length > 1 && zoomVehicles) {
                map.zoomToExtent(bounds, true);
                map.setCenter(bounds.getCenterLonLat());
                var currentZoom = map.getZoom();
                map.zoomTo(currentZoom - 1);
            }

            /*vehicleClickControl = new OpenLayers.Control.SelectFeature(
                [vectors, markers],
                {
                    onSelect: onVehicleClick,
                    autoActivate: true
                }
            );

            map.addControl(vehicleClickControl);*/
            
        }
    }
    catch (err) {
    }
}
var ShowHistoryVehicleIcons = true;
function ShowHistories(historyRecords, zoomHistories) {
    zoomHistories = typeof zoomHistories !== 'undefined' ? zoomHistories : true;
    AllHistoryRecords = historyRecords;

    try {
        if (historyRecords != null && historyRecords != "") {
            var i = 0;
            allHistories = historyRecords;
            var bounds = new OpenLayers.Bounds();
            currentHistories = new Array();
            currentHistoriesIndex = 0;
            if (histories && map) {
                histories.removeAllFeatures();
                if (map.getLayerIndex(histories) != -1) {
                    map.removeLayer(histories);
                }
            }
            var historyFeatures = [];
            var currentHistory;
            
            var points = new Array();
            var lastpoint = null;
            var lastColor = null;
            //Edited by rohit mittal
            for (i = (historyRecords.length - 1) ; i >= 0; i--) {
                //if (i > 9 || i<8) continue;
                currentHistory = historyRecords[i];
                var vicon = new OpenLayers.Icon(rootpath + "New20x20/" + currentHistory.icon);
                var viconUrl = rootpath + "New20x20/" + currentHistory.icon;

                if (currentHistory.BoxId == '27492') {
                    var size = new OpenLayers.Size(48, 97);
                    vicon = new OpenLayers.Icon(rootpath + "New20x20/" + "tower.png", size);
                    viconUrl = rootpath + "New20x20/" + "tower.png";
                }
                
                var newLoc = transformCoords(currentHistory.Longitude, currentHistory.Latitude);
                if (zoomHistories) {
                    if (historyRecords.length == 1) {
                        map.setCenter(newLoc, 14);
                    }
                    else {
                        try {
                            if (currentHistory.Longitude != 0 && currentHistory.Latitude != 0) {
                                bounds.extend(newLoc);
                            }
                        }
                        catch (ex) {
                        }
                    }
                }

                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                //added by rohit mittal
                if (lastpoint == null) {
                    lastpoint = point;
                    lastColor = currentHistory.TripColor;
                }
                else {
                    var style;
                    if (ShowMultiColor) {
                        var color = lastColor;
                        style = {
                            strokeColor: color,
                            strokeOpacity: 1.0,
                            strokeWidth: 4
                        };
                    }
                    else {
                        style = {
                            strokeColor: "#0000ff",
                            strokeOpacity: 1.0,
                            strokeWidth: 4
                        };
                    }
                    var lineFeature = new OpenLayers.Feature.Vector(new OpenLayers.Geometry.LineString([lastpoint, point]), null, style);
                    lineFeature.noclickevent = true;
                    histories.addFeatures([lineFeature]);
                    lastpoint = point;
                    lastColor = currentHistory.TripColor;
                }
                points.push(point);
                var historyMarker = new OpenLayers.Feature.Vector(point);
                
                //var popupContent = getHistoryPopupContent(currentHistory);

                //historyMarker.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc, BoxId: currentHistory.BoxId, Description: currentHistory.Description };
                historyMarker.attributes = { vicon: vicon, icon: viconUrl, popupContent: '', location: newLoc, BoxId: currentHistory.BoxId, Description: currentHistory.Description, historyRecordsIndex: i, Driver: currentHistory.Driver };

                    //currentHistories[currentHistoriesIndex] = historyMarker;
                    //currentHistoriesIndex++;
                    if (ShowHistoryVehicleIcons) 
                     historyFeatures.push(historyMarker);
                }
            

            map.addLayer(histories);

            
            /*
             * Draw path
             */
            map.addControl(new OpenLayers.Control.DrawFeature(histories, OpenLayers.Handler.Path));
            

            /*
            * Draw path End
            */

            histories.addFeatures(historyFeatures);
            //markers.addFeatures(historyFeatures);

            if (historyRecords.length > 1 && zoomHistories) {
                map.zoomToExtent(bounds, true);
                map.setCenter(bounds.getCenterLonLat());
                var currentZoom = map.getZoom();
                map.zoomTo(currentZoom - 1);
            }
            /*else if (zoomHistories) {
                map.zoomToExtent(bounds, true);
                map.setCenter(bounds.getCenterLonLat());
                map.zoomTo(14);
            }*/
        }
    }
    catch (err) {
    }
}

var CurrentHistoryPlayIndex = 0;
var HistoryPlayPaused = true;
var HistoryReplayDelayBaseTime = 200;
var HistoryReplayDelayTime = 200;
function ReplayHistories(historyRecords) {
    //AllHistoryRecords = historyRecords;
    //currentIndex = typeof currentIndex !== 'undefined' ? currentIndex : historyRecords.length - 1;

    // Initialise Replay panel
    if (CurrentHistoryPlayIndex == 0 && historyRecords.length > 0) {
        parent.historyReplaySlider.setMinValue(1);
        parent.historyReplaySlider.setMaxValue(historyRecords.length);
    }

    parent.historyReplaySlider.setValue(CurrentHistoryPlayIndex + 1, false);
    parent.SetHistoryGraphHighlight(CurrentHistoryPlayIndex);

    try {
        if (historyRecords != null && historyRecords != "") {
            var i = 0;
            allHistories = historyRecords;
            var bounds = new OpenLayers.Bounds();
            currentHistories = new Array();
            currentHistoriesIndex = 0;
            
            var historyFeatures = [];
            var currentHistory;

            var points = new Array();
            
            //for (i = 0; i < historyRecords.length; i++) {
            //for (i = historyRecords.length - 1; i >= 0; i--) {
            //var i = currentIndex;
            var i = historyRecords.length - 1 - CurrentHistoryPlayIndex;
            //if (i > 9 || i<8) continue;
            currentHistory = historyRecords[i];
            var vicon = new OpenLayers.Icon(rootpath + "New20x20/" + currentHistory.icon);
            var viconUrl = rootpath + "New20x20/" + currentHistory.icon;

            if (currentHistory.BoxId == '27492') {
                var size = new OpenLayers.Size(48, 97);
                vicon = new OpenLayers.Icon(rootpath + "New20x20/" + "tower.png", size);
                viconUrl = rootpath + "New20x20/" + "tower.png";
            }

            var newLoc = transformCoords(currentHistory.Longitude, currentHistory.Latitude);

            var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);

            points.push(point);
            var historyMarker = new OpenLayers.Feature.Vector(point);

            historyMarker.attributes = { vicon: vicon, icon: viconUrl, popupContent: '', location: newLoc, BoxId: currentHistory.BoxId, Description: currentHistory.Description, historyRecordsIndex: i };

                histories.removeAllFeatures();
                historyFeatures = []
                historyFeatures.push(historyMarker);
                histories.addFeatures(historyFeatures);
            //}

            //histories.addFeatures(historyFeatures);
            
            /*if (historyRecords.length > 1 && zoomHistories) {
                map.zoomToExtent(bounds, true);
                map.setCenter(bounds.getCenterLonLat());
                var currentZoom = map.getZoom();
                map.zoomTo(currentZoom - 1);
            }*/

                i--;
                CurrentHistoryPlayIndex++;
                if (CurrentHistoryPlayIndex >= historyRecords.length)
                    CurrentHistoryPlayIndex = 0;

                if (i >= 0 && !HistoryPlayPaused) {
                    //var delaytime = Math.abs(historyRecords[i + 1].OriginDateTime.getTime() - historyRecords[i].OriginDateTime.getTime()) / HistoryReplayDelayTime;
                    var delaytime = HistoryReplayDelayTime;
                    HistoryReplayTimeout = window.setTimeout(function () {
                        ReplayHistories(historyRecords) 
                        //parent.historyReplaySlider.setValue(CurrentHistoryPlayIndex + 1, false);
                    }, delaytime);
                }
                else {
                    HistoryPlayPaused = true;
                    parent.HistoryPlayPaused = true;
                    //parent.btnHistoryReplay.setText(parent.Res_HistoryReplayReplay); // 'Replay'
                    parent.btnHistoryPlayPlay.show();
                    parent.btnHistoryPlayPause.hide();
                }
            
        }
    }
    catch (err) {
    }
}

function clearHistoryReplayTimeout() {
    clearTimeout(HistoryReplayTimeout);
}


//Edited by Rohit Mittal
function getColors(sensorMask) {
    var bit11;
    var bit12;
    var bit13;
    try {
        bit11 = IsBitOn(sensorMask, 11);
        bit12 = !IsBitOn(sensorMask, 12);
        bit13 = !IsBitOn(sensorMask, 13);
        if (bit11 && (bit12 || bit13)) {
            return "#0000ff"; //blue
        }
        if (!bit11 && !bit12 && !bit13) {
            return "#008000"; //green
        }
        if (!bit11 && (bit12 || bit13)) {
            return "#ff69b4"; //pink
        }
        if (bit11 && !bit12 && !bit13) {
            return "#47260F"; //brown
        }
        else {
            return "#0000ff"; //blue
        }
    }
    catch (err) {
        return "#0000ff"; //blue
    }
}

function IsBitOn(sensorMask, bit) {
    try {
        return ((parseInt(sensorMask) & Math.pow(2, bit - 1)) == Math.pow(2, bit - 1));
    }
    catch (err) {
        return false;
    }
}
function removeMapWindow() {
    var winparent = window.opener;
    if (winparent == null) {
        winparent = window.parent;
    }
    if (winparent != null) {
        if (myWinID && myWinID != "") {
            winparent.removeMapWindow(myWinID);
        }
    }
}

function transformCoords(lon, lat) {
    return new OpenLayers.LonLat(lon, lat)
   .transform(
   new OpenLayers.Projection("EPSG:4326"),
   map.getProjectionObject()
   );
}

function getPixelValues(lon, lat) {
    var latLong = new OpenLayers.LonLat(lon, lat).transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
    return new OpenLayers.Pixel(latLong.lon, latLong.lat);
}

function sortNumber(a, b) {
    return a - b;
}

function UpdateMultipleAssets(fleet) {
    
    try {
        if (fleet != null && fleet != "" && allVehicles.length > 0) {
            var i = 0, j = 0;
            
            if (parent.VehicleClustering) {
                
                if (markers && map) {
                    
                    markers.removeAllFeatures();
                    if (map.getLayerIndex(markers) != -1) {
                        map.removeLayer(markers);
                    }
                }
                for (i = 0; i < fleet.length; i++) {
                    for (j = 0; j < vehicleFeatures.length; j++) {
                        if (vehicleFeatures[j].attributes.BoxId == fleet[i].BoxId) {
                            try {
                                currentVehicle = fleet[i];
                                var newLoc = transformCoords(currentVehicle.Longitude, currentVehicle.Latitude);
                                var vicon = new OpenLayers.Icon("../New20x20/" + currentVehicle.icon);
                                var viconUrl = "../New20x20/" + currentVehicle.icon;
                                if (currentVehicle.BoxId == '27492') {
                                    var size = new OpenLayers.Size(48, 97);
                                    vicon = new OpenLayers.Icon("../New20x20/" + "tower.png", size);
                                    viconUrl = "../New20x20/" + "tower.png";
                                }
                                else if (currentVehicle.ImagePath != '' && currentVehicle.ImagePath != undefined) {
                                    vicon = new OpenLayers.Icon("../images/CustomIcon/" + currentVehicle.icon);
                                    var viconUrl = "../images/CustomIcon/" + currentVehicle.icon;
                                }

                                //markers.removeFeatures([currentMarkers[j]]);

                                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                                var marker = new OpenLayers.Feature.Vector(point);

                                marker.attributes = { vicon: vicon, icon: viconUrl, location: newLoc, BoxId: currentVehicle.BoxId, Driver: currentVehicle.Driver, Description: currentVehicle.Description };
                                vehicleFeatures[j] = marker;
                                //markers.addFeatures([marker]);

                                currentMarkers[j] = marker;
                                //allVehicles[j] = currentVehicle;
                                
                            }
                            catch (err) {
                            }
                            break;
                        }
                    }
                }

                map.addLayer(markers);
                markers.addFeatures(vehicleFeatures);
            }
            else {
                for (i = 0; i < fleet.length; i++) {
                    for (j = 0; j < allVehicles.length; j++) {
                        if (allVehicles[j].BoxId == fleet[i].BoxId) {
                            try {
                                currentVehicle = fleet[i];
                                var newLoc = transformCoords(currentVehicle.Longitude, currentVehicle.Latitude);
                                var vicon = new OpenLayers.Icon("../New20x20/" + currentVehicle.icon);
                                var viconUrl = "../New20x20/" + currentVehicle.icon;
                                if (currentVehicle.BoxId == '27492') {
                                    var size = new OpenLayers.Size(48, 97);
                                    vicon = new OpenLayers.Icon("../New20x20/" + "tower.png", size);
                                    viconUrl = "../New20x20/" + "tower.png";
                                }
                                else if (currentVehicle.ImagePath != '' && currentVehicle.ImagePath != undefined) {
                                    vicon = new OpenLayers.Icon("../images/CustomIcon/" + currentVehicle.icon);
                                    var viconUrl = "../images/CustomIcon/" + currentVehicle.icon;
                                }

                                markers.removeFeatures([currentMarkers[j]]);

                                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                                var marker = new OpenLayers.Feature.Vector(point);

                                marker.attributes = { vicon: vicon, icon: viconUrl, location: newLoc, BoxId: currentVehicle.BoxId, Driver: currentVehicle.Driver, Description: currentVehicle.Description };
                                markers.addFeatures([marker]);

                                currentMarkers[j] = marker;
                                allVehicles[j] = currentVehicle;

//                                if (parent.VehicleClustering) {
//                                }
                                //                                else if (overlaysettings.vehicleDrivers) {
//                                    var proj = new OpenLayers.Projection("EPSG:4326");
//                                    var point = new OpenLayers.LonLat(currentVehicle.Longitude, currentVehicle.Latitude);
//                                    point.transform(proj, map.getProjectionObject());

//                                    var lonlat = vehicleDriversLayer.features[j].geometry.getBounds().getCenterLonLat();

//                                    var dx = point.lon - lonlat.lon;
//                                    var dy = point.lat - lonlat.lat;

//                                    if (dx != 0 || dy != 0) {
//                                        vehicleDriversLayer.features[j].geometry.move(dx, dy);
//                                        vehicleDriversLayer.features[j].layer.drawFeature(vehicleDriversLayer.features[j]);
//                                    }
//                                }
                                //                                else if (overlaysettings.vehiclenames) {
//                                    var proj = new OpenLayers.Projection("EPSG:4326");
//                                    var point = new OpenLayers.LonLat(currentVehicle.Longitude, currentVehicle.Latitude);
//                                    point.transform(proj, map.getProjectionObject());

//                                    var lonlat = vehiclenamesLayer.features[j].geometry.getBounds().getCenterLonLat();

//                                    var dx = point.lon - lonlat.lon;
//                                    var dy = point.lat - lonlat.lat;

//                                    if (dx != 0 || dy != 0) {
//                                        vehiclenamesLayer.features[j].geometry.move(dx, dy);
//                                        vehiclenamesLayer.features[j].layer.drawFeature(vehiclenamesLayer.features[j]);
//                                    }
//                                }
                            }
                            catch (err) {
                            }
                            break;
                        }
                    }

                }
            }

            closeVehiclePopups();
        }
    }
    catch (err) {
    }
}

function openWindow(wintitle, winURL, winWidth, winHeight) {

    var win = new Ext.Window(

      {

          title: wintitle,

          width: winWidth,

          height: winHeight,

          layout: 'fit',

          maxWidth: window.screen.width,

          maxHeight: window.screen.height,

          maximizable: 'true',

          minimizable: 'true',

          resizable: 'true',

          closable: true,

          border: false,

          html: winURL

      }

      );

    win.show();

}


function onPopupClose(evt) {
    selectControl.unselect(selectedFeature);    
}

var currentVehiclePopupFeature;
var currentVehiclePopupIndex;
var VehiclePopupPageSize = 10;


var isdoubleclick = false;
function onVehicleClick(feature) {
    
    if (feature.attributes && feature.attributes.markerType && feature.attributes.markerType == 'searchMarker')
    {
        //alert(feature.attributes.address);
        onSearchMarkerClick(feature);
        return;
    }

    if (feature.noclickevent) {
        vehicleClickControl.unselect(feature);
        return;
    }
    if (feature.fid == null) {
        onVehicleClickAction(feature);
    }
    else {
        window.setTimeout(function () { onVehicleClickAction(feature) }, 300);
    }
}

function onVehicleClickAction(feature) {
    if (isdoubleclick) {
        isdoubleclick = false;
        return;
    }
    if (feature.fid != null) {      // not clustered Geozone/Landmark
        onFeatureSelect(feature);
        vehicleClickControl.unselect(feature);
        return;
    }
    parent.vehiclerunner.stopAll();
    parent.alarmrunner.stopAll();
    parent.messagerunner.stopAll();
    var content;
    var position;
    if (feature.cluster) {
        currentVehiclePopupFeature = feature;
        position = new OpenLayers.LonLat(feature.geometry.x, feature.geometry.y);
        content = "<h6 style='margin-bottom:10px;'>" + feature.attributes.count + h6featureVehiclesText + "</h6>"; //res   //content = "<h6 style='margin-bottom:10px;'>" + feature.attributes.count + " Vehicles.</h6>";
        content = content + "<div id='vehiclelist'>";
        var i = 0;
       while ((i < VehiclePopupPageSize && i < feature.cluster.length)) {
            var markerscontent = "";
            $.ajax({
                type: 'POST',
                url: rootpath + 'Vehicles.aspx?QueryType=getBoxInfo&BoxId=' + feature.cluster[i].attributes.BoxId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    markerscontent = getPopupContent(msg);
                },
                error: function (msg) {
                    //content = "failed to fetch the data.";
                }
            });
            content = content + "<div class='singleVehiclePopupContent'>" + markerscontent + "</div>";
            i++;
        }
        currentVehiclePopupIndex = i;
        content = content + "</div>";

        if (feature.attributes.count > VehiclePopupPageSize) {
            content = content + "<div id='vehicleViewMoreLink'><a href='javascript:void(0)' onclick='loadmoreclustervehicles()'>" + vehicleViewMoreLinkText + "</a></div>"; //res    //content = content + "<div id='vehicleViewMoreLink'><a href='javascript:void(0)' onclick='loadmoreclustervehicles()'>View More</a></div>";
        }
    }
    else {
        if (feature.attributes.popupContent != undefined && feature.attributes.popupContent != '') {

            if (ShowFields && feature.attributes.Equipment == undefined) {
                $.ajax({
                    type: 'POST',
                    url: rootpath + 'Vehicles.aspx?QueryType=getVehicleInfo_NewTZ&vehicleId=' + feature.attributes.VehicleId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (msg) {
                        feature.attributes.Equipment = msg.Field1;
                        feature.attributes.TeamLeaderName = msg.Field2;
                        feature.attributes.Listname = msg.Field3;

                        var $c = $(feature.attributes.popupContent);
                        $c.find('#Equipment' + feature.attributes.VehicleId).html(feature.attributes.Equipment);
                        $c.find('#TeamLeaderName' + feature.attributes.VehicleId).html(feature.attributes.TeamLeaderName);
                        $c.find('#Listname' + feature.attributes.VehicleId).html(feature.attributes.Listname);
                        $c.find('.vehiclefields').css('display', '');
                        feature.attributes.popupContent = $c.html();
                    },
                    error: function (msg) {
                        //content = "failed to fetch the data.";
                    }
                });

            }
            content = feature.attributes.popupContent;
        }
        else if (feature.attributes.historyRecordsIndex != undefined) {
            content = getHistoryPopupContent(AllHistoryRecords[feature.attributes.historyRecordsIndex]);
            feature.attributes.popupContent = content;
        }
        else if (feature.attributes.BoxId != undefined)
        {
            $.ajax({
                type: 'POST',
                url: rootpath + 'Vehicles.aspx?QueryType=getBoxInfo&BoxId=' + feature.attributes.BoxId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {                    
                    content = getPopupContent(msg);
                },
                error: function (msg) {
                    //content = "failed to fetch the data.";
                }
            });
        }
        else {
            content = 'n/a';
        }

        position = feature.attributes.location;
    }
    vehiclePopup = new OpenLayers.Popup.FramedCloud("featurePopup", position,
                                new OpenLayers.Size(100, 100),
                                content,
                                feature.attributes.vicon, true, onVehiclePopupClose);
    vehicleClickControl.unselect(feature);

    map.addPopup(vehiclePopup, true);

    var c = map.getControlsByClass("OpenLayers.Control.KeyboardDefaults");
    c[0].deactivate();
    if (parseInt(parent.alarminterval) > parseInt(parent.vehinterval))
        parent.delayrunner.delay(parseInt(parent.alarminterval))
    else
        parent.delayrunner.delay(parseInt(parent.vehinterval))
}

function onSearchMarkerClick(feature) {
    //alert(feature.geometry.x);
    var position = new OpenLayers.LonLat(feature.geometry.x, feature.geometry.y);
    var content = "<div style='font-weight:bold;'>" + feature.attributes.address + "</div>";
    //content += "<div style='margin-top: 20px;'><a href='javascript:void(0)' onclick='parent.getClosestVehicles(" + feature.attributes.lon + "," + feature.attributes.lat + ");map.removePopup(searchMarkerPopup);'>Closest Vehicles</a> &nbsp; <a href='javascript:void(0)'>History</a></div>";
    content += PointPopupForm(feature.attributes.lon.toFixed(5), feature.attributes.lat.toFixed(5));
    
    searchMarkerPopup = new OpenLayers.Popup.FramedCloud("addressMarkerPopup", position,
                                new OpenLayers.Size(100, 50),
                                content,
                                null, true, null);

    searchMarkerPopup.autoSize = true;
    
    vehicleClickControl.unselect(feature);

    map.addPopup(searchMarkerPopup, true);

    $('#ClosestVehiclesRadius').val(ClosestVehiclesRadius);
    $('#ClosestVehiclesNumOfVehicles').val(ClosestVehiclesNumOfVehicles);
    $('#SearchHistoryTime').val(SearchHistoryTime);
    $('#SearchHistoryDate').val(SearchHistoryDate);
    $('#SearchHistoryRadius').val(SearchHistoryRadius);
    $('#SearchHistoryMinutes').val(SearchHistoryMinutes);
}

function PointPopupForm(lon, lat) {
    var content = "<div style='margin-top: 20px;'><a href='javascript:void(0)' onclick='toggleSearchAddressCriterias(this, \"ClosestVehiclesCriterias\", 300, 50);'>" + lnkClosestVehicleText + "</a> &nbsp; "; //res
    if (ShowMapHistorySearch) {
        content += "<a href='javascript:void(0)' onclick='toggleSearchAddressCriterias(this, \"SearchHistoryCriterias\", 400, 70);'>" + lnkShowMapHistoryText + "</a>"; //res
    }
    content += "</div>";
    content += "<div style='margin-top: 5px; display:none;border-top: 1px solid #cccccc;padding-top: 5px;' id='SearchAddressForms'>";
    content += "<div style='display:none' id='ClosestVehiclesCriterias' class='SearchAddressFields'>";
    content += " <span style='font-weight: bold; color: green;'>" + vehicleText + "</span> <select id='ClosestVehiclesRadius' /><option value='100'>100</option><option value='500'>500</option><option value='1000'>1000</option><option value='2000'>2000</option></select> m "; //res
    content += " <span style='font-weight: bold; color: green;'>" + ClosestVehiclesNumOfVehiclesText + "</span> <select id='ClosestVehiclesNumOfVehicles' /><option value='10'>10</option><option value='20'>20</option><option value='50'>50</option><option value='100' selected>100</option></select>"; //res
    content += " <a href='javascript:void(0)' onclick='getClosestVehicles(" + lon + "," + lat + ");'><img src='../images/searchicon.png' /></a>";
    content += "</div>";
    content += "<div style='display:none' id='SearchHistoryCriterias' class='SearchAddressFields'>";
    content += " <div><span style='font-weight: bold; color: green;'>" + SearchHistoryDateText + "</span> <input type='text' size='10' id='SearchHistoryDate' />"; //res
    content += " <span style='font-weight: bold; color: green;'>" + SearchHistoryTimeText + "</span> <select id='SearchHistoryTime'>" + createTimeOptions() + "</select>"; //res
    content += " <span style='font-weight: bold; color: green;'>" + Res_By + ": </span><select id='SearchHistoryBy'><option value='1'>" + Res_SearchHistoryFleetHierarchy + "</option><option value='2'>" + Res_SelectedVehicles + "</option></select>";
    content += "</div>";
    content += " <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>" + vehicleText + "</span> <select id='SearchHistoryRadius' /><option value='100'>100</option><option value='500'>500</option><option value='1000'>1000</option><option selected value='2000'>2000</option></select> m "; //res
    content += " <span style='font-weight: bold; color: green;'>+/-</span> <select id='SearchHistoryMinutes'> " + buildOptions(isExtended) + " </select> " + SearchHistoryMinutesText + " &nbsp; "; // res
    //content += " <span style='font-weight: bold; color: green;'>Vehicles:</span> <select id='ClosestVehiclesNumOfVehicles' /><option value='10'>10</option><option value='20' selected>20</option><option value='50'>50</option><option value='100'>100</option></select>";
    content += " <a href='javascript:void(0)' onclick='searchHistoryAddress(" + lon + "," + lat + ");'><img src='../images/searchicon.png' /></a>";
    content += " </div>";
    content += "</div>";
    content += "</div>";
    
    return content;
}

function createTimeOptions() {
    var s = '';
    for (var i = 0; i < 24; i++) {
        var hour = i < 10 ? '0' + i : i;
        s += "<option value='" + hour + ":00'>" + hour + ":00</option>";
        s += "<option value='" + hour + ":15'>" + hour + ":15</option>";
        s += "<option value='" + hour + ":30'>" + hour + ":30</option>";
        s += "<option value='" + hour + ":45'>" + hour + ":45</option>";
    }
    return s;
}

function toggleSearchAddressCriterias(o, fieldname, width, height) {
    var cdiv = searchMarkerPopup.contentDiv;
    var w = $(cdiv).outerWidth();
    w = w < width ? width : w;

    SearchAddressOriginHeight = $('#SearchAddressForms').is(":visible") ? SearchAddressOriginHeight : $(cdiv).outerHeight();
    
    var h = ($('#SearchAddressForms').is(":visible") && $('#' + fieldname).is(":visible")) ? SearchAddressOriginHeight : SearchAddressOriginHeight + height;
    //alert(h);
    if ($('#' + fieldname).is(":visible")) {
        $('.SearchAddressFields').hide();
        $('#SearchAddressForms').hide();        
    }
    else {
        $('.SearchAddressFields').hide();
        $('#' + fieldname).show();
        $('#SearchAddressForms').show();
        //alert($("#SearchHistoryDate"));
        //alert($("#SearchHistoryDate"));
        //$("#txtSearchAddress").datepicker();
        $("#SearchHistoryDate").datepicker({
            showButtonPanel: true,
            //changeMonth : true,
            //changeYear : true,
            dateFormat: DefaultUserDate,
            closeText: closeText//res tbd this was 'Close' earlier
        });        
    }

    searchMarkerPopup.setSize(new OpenLayers.Size(w, h));
}


function getClosestVehicles(lon, lat) {
    ClosestVehiclesRadius = $('#ClosestVehiclesRadius').val();
    ClosestVehiclesNumOfVehicles = $('#ClosestVehiclesNumOfVehicles').val();

    var proj = new OpenLayers.Projection("EPSG:4326");
    var point = new OpenLayers.LonLat(lon, lat);
    point.transform(proj, map.getProjectionObject());
    var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
    var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, ClosestVehiclesRadius / Math.cos(lat * (Math.PI / 180)), 40, 0);
    feature = new OpenLayers.Feature.Vector(circle);
    searchArea.removeAllFeatures();
    searchArea.addFeatures([feature]);

    parent.getClosestVehicles(lon, lat, ClosestVehiclesRadius, ClosestVehiclesNumOfVehicles); 
    map.removePopup(searchMarkerPopup);
}

function getVehiclesInLandmark(landmarkId, popup) {
    parent.getVehiclesInLandmark(landmarkId);
    map.removePopup(popup);
    
}

function searchHistoryAddress(lon, lat) {
    SearchHistoryRadius = $('#SearchHistoryRadius').val();
    SearchHistoryMinutes = $('#SearchHistoryMinutes').val();
    //ClosestVehiclesNumOfVehicles = $('#ClosestVehiclesNumOfVehicles').val();
    SearchHistoryTime = $('#SearchHistoryTime').val();
    SearchHistoryDate = $('#SearchHistoryDate').val();
    mapSearchPointSets = $('#mapSearchPointSets').val();
    SearchHistoryByOption = $('#SearchHistoryBy').val();
    
    if ((mapSearchPointSets == '' || mapSearchPointSets == null) && SearchHistoryRadius > 2000) {
        $('#searchAddressMessage').show();
        return;
    }
    $('#searchAddressMessage').hide();

    if (mapSearchPointSets == '' || mapSearchPointSets == null) {
        var proj = new OpenLayers.Projection("EPSG:4326");
        var point = new OpenLayers.LonLat(lon, lat);
        point.transform(proj, map.getProjectionObject());
        var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
        var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, SearchHistoryRadius / Math.cos(lat * (Math.PI / 180)), 40, 0);
        feature = new OpenLayers.Feature.Vector(circle);
        
    }
    else {
        var mapsearchPolygonString = 'POLYGON((' + mapSearchPointSets + '))';
        
        var proj = new OpenLayers.Projection("EPSG:4326");
        var in_options = {
            'internalProjection': map.baseLayer.projection,
            'externalProjection': proj
        };
        feature = new OpenLayers.Format.WKT(in_options).read(mapsearchPolygonString);              
    }
    
    searchArea.removeAllFeatures();
    searchArea.addFeatures([feature]);

    parent.searchHistoryAddress(lon, lat, SearchHistoryDate + ' ' + SearchHistoryTime, SearchHistoryRadius, SearchHistoryMinutes, mapSearchPointSets, SearchHistoryByOption);
    map.removePopup(searchMarkerPopup);
}

function onVehiclePopupClose(evt) {
    var c = map.getControlsByClass("OpenLayers.Control.KeyboardDefaults");
    c[0].activate();
    donotclosevehiclepopups = false;
    closeVehiclePopups();
}

function loadmoreclustervehicles() {
    //alert(currentVehiclePopupFeature);
    parent.vehiclerunner.stopAll();
    parent.alarmrunner.stopAll();
    parent.messagerunner.stopAll();
    if (currentVehiclePopupFeature && currentVehiclePopupFeature.cluster) {
        var content = "";
        var startIndex = currentVehiclePopupIndex;
      
        while ((currentVehiclePopupIndex < startIndex + VehiclePopupPageSize && currentVehiclePopupIndex < currentVehiclePopupFeature.attributes.count)) {
            var markerscontent = "";
            $.ajax({
                type: 'POST',
                url: rootpath + 'Vehicles.aspx?QueryType=getBoxInfo&BoxId=' + currentVehiclePopupFeature.cluster[currentVehiclePopupIndex].attributes.BoxId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    markerscontent = getPopupContent(msg);
                },
                error: function (msg) {
                    //content = "failed to fetch the data.";
                }
            });
            content = content + "<div class='singleVehiclePopupContent'>" + markerscontent + "</div>";
            currentVehiclePopupIndex++;
        }
        $('#vehiclelist').append(content);
        if (currentVehiclePopupIndex >= currentVehiclePopupFeature.cluster.length)
            $('#vehicleViewMoreLink').hide();
        else
            $('#vehicleViewMoreLink').show();
    }
    if (parseInt(parent.alarminterval) > parseInt(parent.vehinterval))
        parent.delayrunner.delay(parseInt(parent.alarminterval))
    else
        parent.delayrunner.delay(parseInt(parent.vehinterval))
}

function onFeatureSelect(feature) {
    var latlon = feature.geometry.getBounds().getCenterLonLat();
    latlon.transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
    centerPoint = latlon.lat + "," + latlon.lon;
    txtX = latlon.lon;
    txtY = latlon.lat;
    latlon.transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));

    //alert(txtX + ',' + txtY);
    
    selectedFeature = feature;
    var popupheight;
    var content = "<div style='font-size:.8em'>" + onFeatureSelectContentText + "</div>"; //res    //var content = "<div style='font-size:.8em'>New Landmark or Geozone</div>";
    if (feature.fid.split(":::")[0] == "Landmark" || feature.fid.split(":::")[0] == "LandmarkCircle") {
        //popupheight = 280;
        polygonPopupHeight = 280;
        if (feature.fid.split(":::")[0] == "LandmarkCircle") polygonPopupHeight = 310;
        var oid = feature.fid.split(":::")[1];
        
        $.ajax({
            type: 'POST',
            url: rootpath + 'MapNew/landmarkEditForm.aspx?lon=' + txtX + '&lat=' + txtY,  //url: rootpath + 'MapNew/landmarkEditForm.aspx',
            data: { landmarkName: oid, showheader: false, geotype: 'landmark' },
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            dataType: "html",
            async: false,
            success: function (msg) {
                var c = feature.geometry.components[0];

                var isPublic = feature.attributes.Public == 'true';
                var pointSets = "";                
                content = msg;
            },
            error: function (msg) {
                content = errorContentFailedDataFetchText; //res
            }
        });
    }
    else if (feature.fid.split(":::")[0] == "Geozone") { //res tbd
        //popupheight = 300;
        polygonPopupHeight = 270;
        var oid = feature.fid.split(":::")[1];
        var Assigned = feature.Assigned;
        $.ajax({
            type: 'POST',
            url: rootpath + 'MapNew/AddNewMapGoezone.asmx/GetGeozoneById_NewTZ',
            data: JSON.stringify({ oid: oid }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {
                var c = feature.geometry.components[0];
                var pointSets = "";
                var isPublic = feature.attributes.Public == 'true';
                for (var i = 0; i < c.components.length - 1; i++) {
                    c.components[i].transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));

                    var x = c.components[i].x;
                    var y = c.components[i].y;
                    //alert("x=" + x + ", y=" + y);
                    if (pointSets == "")
                        pointSets = y + "|" + x;
                    else
                        pointSets = pointSets + "," + y + "|" + x;

                    c.components[i].transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));
                }

                var r = eval('(' + msg.d + ')');
                content = buildEditFormString("Geozone", r, pointSets, Assigned, isPublic);
                //content = r.GeozoneId + "<br />";
                //content = content + "name:" + r.GeozoneName + " Description:" + r.Description;
            },
            error: function (msg) {
                content = errorContentFailedDataFetchText; //res
            }
        });
    }

    removeAllPopups();

    polygonPopup = new OpenLayers.Popup.FramedCloud("polygonformpopup",
                                     feature.geometry.getBounds().getCenterLonLat(),
                                     null,
                                     content,
                                     null, true, onPopupClose);
    feature.popup = polygonPopup;    
    map.addPopup(polygonPopup);

    var c = map.getControlsByClass("OpenLayers.Control.KeyboardDefaults");
    c[0].deactivate();
    //popup.setSize(new OpenLayers.Size(400, popupheight));
}

function onFeatureUnselect(feature) {
    try {        
        map.removePopup(feature.popup);
        feature.popup.destroy();
        feature.popup = null;
    }
    catch (e) { }
}

function update() {
    // reset modification mode
    mycontrols.modify.mode = OpenLayers.Control.ModifyFeature.RESHAPE;
    var rotate = document.getElementById("rotate").checked;
    if (rotate) {
        mycontrols.modify.mode |= OpenLayers.Control.ModifyFeature.ROTATE;
    }
    var resize = document.getElementById("resize").checked;
    if (resize) {
        mycontrols.modify.mode |= OpenLayers.Control.ModifyFeature.RESIZE;
        var keepAspectRatio = document.getElementById("keepAspectRatio").checked;
        if (keepAspectRatio) {
            mycontrols.modify.mode &= ~OpenLayers.Control.ModifyFeature.RESHAPE;
        }
    }
    var drag = document.getElementById("drag").checked;
    if (drag) {
        mycontrols.modify.mode |= OpenLayers.Control.ModifyFeature.DRAG;
    }
    if (rotate || drag) {
        mycontrols.modify.mode &= ~OpenLayers.Control.ModifyFeature.RESHAPE;
    }
    var sides = parseInt(document.getElementById("sides").value);
    sides = Math.max(3, isNaN(sides) ? 0 : sides);
    mycontrols.regular.handler.sides = sides;
    var irregular = document.getElementById("irregular").checked;
    mycontrols.regular.handler.irregular = irregular;
}

function toggleControl(element) {
    if (element.value == 'none') {
        strategy.activate();
        vectors.removeFeatures(visibleGeoLandmarkFeature.features);
        vectors.addFeatures(visibleGeoLandmarkFeature);

        vehicleClickControl.activate();
        dblclick.deactivate();
        dblclick.activate();

        var c = map.getControlsByClass("OpenLayers.Control.KeyboardDefaults");
        c[0].activate();
    }
    else {
        strategy.deactivate();        
        vehicleClickControl.deactivate();
        
        vectors.removeAllFeatures();
        vectors.addFeatures(visibleGeoLandmarkFeature);

        var c = map.getControlsByClass("OpenLayers.Control.KeyboardDefaults");
        c[0].deactivate();
    }
    for (key in mycontrols) {
        var control = mycontrols[key];
        if (element.value == key && element.checked) {
            control.activate();
        } else {
            control.deactivate();
        }
    }
}

function undopolygon() {

    if (undolist.length == 0) {
        $('#undobar').hide();
        return;
    }
    setChangeslist();
    var lastdo = undolist[undolist.length - 1];
    
    for (var i = 0; i < vectors.features.length; i++) {
        var feature = vectors.features[i];
        
        if (feature.cluster)
            continue;

        if (feature.fid == null || feature.geometry == null)
            continue;

        var featuretype = feature.fid.split(":::")[0];
        var featureid = feature.fid.split(":::")[1];

        if (encodeURI(featuretype) == lastdo.otype && encodeURI(featureid) == lastdo.oid) {
            //selectControl.unselect(vectors.features[i]);

            var proj = new OpenLayers.Projection("EPSG:4326");
            var previousToggle;
            //if ($('#modifyToggle').attr('checked')) {
            //    previousToggle = "modifyToggle";
            //    $('#noneToggle').click();
            //}

            polygonPoints = lastdo.pointSets.split(",");
            var polygonstring = "";
            for (var j = 0; j < polygonPoints.length; j++) {
                latlon = polygonPoints[j].split("%7C");
                var point = new OpenLayers.LonLat(latlon[1], latlon[0]);
                point.transform(proj, map.getProjectionObject());
                if (polygonstring == '')
                    polygonstring = point.lon.toString() + " " + point.lat.toString();
                else
                    polygonstring = polygonstring + ", " + point.lon.toString() + " " + point.lat.toString();
            }
            polygonstring = "POLYGON((" + polygonstring + "))";
            //feature.geometry = polygonstring;
            //feature.move(feature.geometry.getBounds().getCenterLonLat());

            vectors.features[i].destroy();

            var newfeature = new OpenLayers.Feature.Vector(
                                        OpenLayers.Geometry.fromWKT(polygonstring)
                                 );
            var fid = feature.fid;
            newfeature.fid = fid;
            vectors.addFeatures([newfeature]);

            geoLandmarkFeatures.push(newfeature);
            visibleGeoLandmarkFeature.push(newfeature);
            //vectors.eraseFeatures(vectors.features[i]);
            //vectors.features.push(newfeature);
            //vectors.drawFeature(newfeature);

            /*vectors.features[i] = newfeature;
            vectors.redraw();
            vectors.refresh();*/

            //editPolygon(lastdo.otype, lastdo.oid, lastdo.pointSets, true);

            undolist.splice(undolist.length - 1, 1);
            $('#undonum').html(undolist.length);

            //if (previousToggle == "modifyToggle") {
            //    document.getElementById("modifyToggle").checked = true;
            //    toggleControl(document.getElementById("modifyToggle"));
            //}

            break;
        }
    }
    
    if (undolist.length == 0) {
        $('#undobar').hide();
    }
}

function setChangeslist() {
    
    changeslist = [];
    for (var i = undolist.length - 1; i >= 0; i--) {
        var addedtochangeslist = false;
        for (var j = 0; j < changeslist.length; j++) {
            if (changeslist[j].otype == undolist[i].otype && changeslist[j].oid == undolist[i].oid) {
                addedtochangeslist = true;
            }
        }
        if (!addedtochangeslist) {    
            changeslist.push(undolist[i]);
        }
    }
}

function savechanges() {
    $('#savechanges').hide();
    $('#undolink').hide();
    $('#undosaving').show();
    $('#noneToggle').click();

    for (var i = 0; i < changeslist.length; i++) {
        editPolygon_NewTZ(changeslist[i].otype, changeslist[i].oid, changeslist[i].pointSets, changeslist[i].centerpoint, changeslist[i].radius);
    }
    changeslist = [];
    undolist = [];
    $('#undobar').hide();
    $('#messagebar').html(messagebarSaveChangesText).show().delay(2000).fadeOut(1000); //res
    btnMapToolPan.fireEvent('click', btnMapToolPan);
}

function cancelchanges() {
    if (undolist.length == 0) {
        $('#undobar').hide();
        return;
    }
    
    while (undolist.length > 0) {
        undopolygon();
    }    

    $('#undobar').hide();
}

function buildEditFormString(otype, data, pointSets, assigned, isPublic) {
    
    var lstAddOptions = 0;
    var oid;
    if (otype == "Geozone")
        oid = data.GeozoneId;

    if (otype == "Geozone") lstAddOptions = 1;
    var formtitle;
    var infotitle;
    if (otype == "Landmark") {
        formtitle = geozoneLandmarkFormTitle; //res     //formtitle = "Edit Landmark";
        infotitle = geozoneLandmarkInfoTitle + data.LandmarkName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;"); //res   //infotitle = "Landmark: " + data.LandmarkName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;");
    }
    else {
        formtitle = geozoneDefaultFormTitle; //res   //formtitle = "Edit Geozone";
        infotitle = geozoneDefaultInfoTitle + data.GeozoneName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;"); //res   //infotitle = "Geozone: " + data.GeozoneName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;");
    }
    var infocontent = "<h6 style=\"margin-left:10px;\">" + infotitle + "</h6>";
    var disabled = assigned == 1 ? ' disabled ' : '';

    infocontent += '<div style="margin:10px 0 0 20px;"><a href="javascript:void(0)" onclick="showeditpolygonform();">' + ResEdit + '</a></div>'   //infocontent += '<div style="margin:10px 0 0 20px;"><a href="javascript:void(0)" onclick="showeditpolygonform();">Edit</a></div>'

    var content = "<h6 style=\"margin-left:10px;border-bottom:1px solid #cccccc\">" + formtitle + "</h6>";
    content += "<div style='font-size:1em'><form id='editpolygonform'>" +
                    "<input type='hidden' id='lstAddOptions' name='lstAddOptions' value='" + lstAddOptions + "' />" +
                    "<input type='hidden' id='oid' name='oid' value='" + oid + "' />" +
                    "<input type='hidden' id='isNew' name='isNew' value='0' />" +
                    "<input type='hidden' id='pointSets' name='pointSets' value='" + pointSets + "' />";
    if (otype == "Landmark") {
        
        content = content +
                        "<input type='hidden' id='txtX' name='txtX' value='" + data.Longitude + "' />" +
                        "<input type='hidden' id='txtY' name='txtY' value='" + data.Latitude + "' />" +
                        "<input type='hidden' id='oldLandmarkName' name='oldLandmarkName' value='" + data.LandmarkName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + "' />" +
                        '<table class="landmarkfield editformmainoptions" cellspacing="0" cellpadding="0" border="0" style=\"margin-left:10px;\">' +
                        '<tr><td style="padding-top:10px;"><table border="0">' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 25px; width: 139px;">' +
                        '            <span id="lblLandmarkNameTitle" class="formtext">' + lblLandmarkNameTitle + '</span>' + //res
                        '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <input name="txtLandmarkName" type="text" id="txtLandmarkName" class="formtext bsmforminput" style="width:173px;" value="' + data.LandmarkName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <span id="lblContactNameTitle" class="formtext">' + lblContactNameTitle + '</span></td>' + //res
                        '        <td class="formtext" style="height: 25px">' +
                        '            <input name="txtContactName" type="text" id="txtContactName" class="formtext bsmforminput" style="width:173px;" value="' + data.ContactPersonName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +                        
                        '    <tr>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <span id="lblLandmarkDescriptionTitle" class="formtext">' + lblLandmarkDescriptionTitle + '</span></td>' + //res
                        '        <td class="formtext">' +
                        '            <input name="txtLandmarkDesc" type="text" id="txtLandmarkDesc" class="formtext bsmforminput" style="width:173px;" value="' + data.Description.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <span id="lblPhoneTitle" class="formtext">' + lblPhoneTitle + '</span></td>' + //res
                        '        <td class="formtext">' +
                        '            <input name="txtPhone" type="text" id="txtPhone" class="formtext bsmforminput" style="width:173px;" value="' + data.ContactPhoneNum.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>';
        if (data.Radius > 0) {
            content = content +
                        '    <tr>' +
                        '        <td class="formtext" height="25" style="height: 15px;">' +
                        '            <span id="lblRadiusTitle" class="formtext">' + lblRadiusTitle + '</span>' + //res
                        '            (' +
                        '            <span id="lblUnit">m</span>):' +
                        '            <span id="valRadius" style="color:Red;visibility:hidden;">*</span><span id="valRangeRadius" style="color:Red;visibility:hidden;">*</span></td>' +
                        '        <td class="formtext" height="25">' +
                        '            <input name="txtRadius" type="text" id="txtRadius" class="formtext bsmforminput" style="width:173px;" value="' + data.Radius + '" /></td>' +
                        '    </tr>';
        }
        content = content +
                        '   <tr>' +
                        '       <td colspan="2" class="formtext" height="25" style="height: 25px;">' +
                        '           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" ' + (isPublic ? '' : 'checked') + ' /><span style="margin-left:3px;">' + lstPublicPrivate_PrivateText + '</span>' + //res   //'           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" ' + (isPublic ? '' : 'checked') + ' /><span style="margin-left:3px;">Private</span>' +
                        '           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" ' + (isPublic ? 'checked' : '') + ' /><span style="margin-left:3px;">' + lstPublicPrivate_PublicText + '</span>' + //res   //'           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" ' + (isPublic ? 'checked' : '') + ' /><span style="margin-left:3px;">Public</span>' +
                        '       </td>' +
                        '   </tr>';
        content = content +
                        '    <tr>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td colspan="2" class="formtext" height="15">' +
                        '           <a href=\'javascript:void(0)\' onclick=\'editformmoreoptions();\'>' + alandmarkformmoreoptionsText + '</a> &nbsp; ' + //res
                        //'           <a href=\'javascript:void(0)\' onclick=\'editformextendedattributes();\'>Extended Attributes</a>' +
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '    </tr>' +
                        '</table></td></tr></table>';
    }
    if (otype == "Geozone") {
        content = content +
                        '<table class="geozonefield editformmainoptions" cellspacing="0" cellpadding="0" style="height: 93px;margin-left:10px;" border="0">' +
                        '<tr><td style="padding-top:10px;"><table border="0">' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblGeozoneNameTitle" class="formtext">' + lblGeozoneNameTitle + '</span>' + //res
                        '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <input name="txtGeoZoneName" type="text" id="txtGeoZoneName" class="formtext bsmforminput" style="width:173px;" value="' + data.GeozoneName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblDirectionTitle" class="formtext">' + lblDirectionTitle + '</span></td>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <select name="cboDirection" id="cboDirection" class="formtext" style="width:175px;" ' + disabled + '>' +
			            '                <option ' + (data.Direction == 0 ? ' selected="selected"' : '') + ' value="0">' + cboDirection_DisableText + '</option>' + //res    //'                <option ' + (data.Direction == 0 ? ' selected="selected"' : '') + ' value="0">Disable</option>' +
			            '                <option ' + (data.Direction == 1 ? ' selected="selected"' : '') + ' value="1">' + cboDirection_OutText + '</option>' + //res     //'                <option ' + (data.Direction == 1 ? ' selected="selected"' : '') + ' value="1">Out</option>' +
			            '                <option ' + (data.Direction == 2 ? ' selected="selected"' : '') + ' value="2">' + cboDirection_InText + '</option>' + //res     //'                <option ' + (data.Direction == 2 ? ' selected="selected"' : '') + ' value="2">In</option>' +
			            '                <option ' + (data.Direction == 3 ? ' selected="selected"' : '') + ' value="3">' + cboDirection_InOutText + '</option>' + //res  //'                <option ' + (data.Direction == 3 ? ' selected="selected"' : '') + ' value="3">InOut</option>' +
		                '            </select></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblGeozoneDescriptionTitle" class="formtext">' + lblGeozoneDescriptionTitle + '</span></td>' + //res
                        '        <td class="formtext" style="height: 25px">' +
                        '            <input name="txtGeoZoneDesc" type="text" id="txtGeoZoneDesc" class="formtext bsmforminput" value="' + data.Description.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" style="width:173px;" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblDefaultSeverityTitle" class="formtext">' + lblDefaultSeverityTitle + '</span>' + //res
                        '        </td>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <select name="cboGeoZoneSeverity" onchange="" id="cboGeoZoneSeverity" class="formtext" DESIGNTIMEDRAGDROP="451" style="width:175px;" >' +
			            '                <option ' + (data.SeverityId == 0 ? ' selected="selected"' : '') + ' value="0">' + cboGeoZoneSeverity_NoAlarmText + '</option>' + //res    //'                <option ' + (data.SeverityId == 0 ? ' selected="selected"' : '') + ' value="0">NoAlarm</option>' +
			            '                <option ' + (data.SeverityId == 1 ? ' selected="selected"' : '') + ' value="1">' + cboGeoZoneSeverity_NotifyText + '</option>' + //res    //'                <option ' + (data.SeverityId == 1 ? ' selected="selected"' : '') + ' value="1">Notify</option>' +
			            '                <option ' + (data.SeverityId == 2 ? ' selected="selected"' : '') + ' value="2">' + cboGeoZoneSeverity_WarningText + '</option>' + //res   //'                <option ' + (data.SeverityId == 2 ? ' selected="selected"' : '') + ' value="2">Warning</option>' +
			            '                <option ' + (data.SeverityId == 3 ? ' selected="selected"' : '') + ' value="3">' + cboGeoZoneSeverity_CriticalText + '</option>' + //res  //'                <option ' + (data.SeverityId == 3 ? ' selected="selected"' : '') + ' value="3">Critical</option>' +
		                '            </select></td>' +
                        '    </tr>';
        content = content +
                        '   <tr>' +
                        '       <td colspan="2" class="formtext" height="25" style="height: 25px;">' +
                        '           <input id="lstPublicPrivate" type="radio" value="0" name="lstGeozonePublicPrivate" ' + (isPublic ? '' : ' checked ') + ' /><span style="margin-left:3px;">' + lstPublicPrivate_PrivateText + '</span>' + //res    //'           <input id="lstPublicPrivate" type="radio" value="0" name="lstGeozonePublicPrivate" ' + (isPublic ? '' : ' checked ') + ' /><span style="margin-left:3px;">Private</span>' +
                        '           <input id="lstPublicPrivate" type="radio" value="1" name="lstGeozonePublicPrivate" style="margin-left:20px;" ' + (isPublic ? ' checked ' : '') + ' /><span style="margin-left:3px;">' + lstPublicPrivate_PublicText + '</span>' + //res   //'           <input id="lstPublicPrivate" type="radio" value="1" name="lstGeozonePublicPrivate" style="margin-left:20px;" ' + (isPublic ? ' checked ' : '') + ' /><span style="margin-left:3px;">Public</span>' +
                        '       </td>' +
                        '   </tr>';
        content = content +
                        '    <tr>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '    <tr>' +
                        '        <td colspan="2" class="formtext" height="15">' +
                        '           <a href=\'javascript:void(0)\' onclick=\'editformmoreoptions();\'>' + alandmarkformmoreoptionsText + '</a>' + //res    //'           <a href=\'javascript:void(0)\' onclick=\'editformmoreoptions();\'>More Options</a>' +
                        '        </td>' +
                        '    </tr>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '    </tr>' +
                        '</table></td></tr></table>';
    }
    content = content +
                        '<table class="editformmoreoptions" style="display:none" id="Table4" style="border:1px solid black;" cellspacing="0" cellpadding="0" >' +
                        '<tr><td valign="top" style="padding:10px;vertical-align: top;"><table border="0">' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblEmailTitle" class="formtext">' + lblEmailTitle + '</span>' + //res
                        '            </td>' +
                        '        <td style="height: 30px">' +
                        '            <input name="txtEmail" type="text" id="txtEmail" class="formtext bsmforminput" value="' + data.Email + '" style="width:173px;" /></td>' +
                        '    </tr>' +
                        '    <tr style="display:none;">' +
                        '        <td class="style4" style="height: 30px">' +
                        '            <span id="lblPhone" class="formtext">' + lblPhoneTitle + '</span>' + //res
                        '                                        <span id="RegularExpressionValidator1" class="formtext" style="color:Red;visibility:hidden;">*</span>' +
                        '                        </td>' +
                        '        <td style="height: 30px">' +
                        '            <input name="txtPhoneSMS" type="text" id="txtPhoneSMS" disabled="disabled" class="formtext bsmforminput" value="' + data.Phone + '" style="width:173px;" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblTimeZoneTitle" class="formtext">' + lblTimeZoneTitle + '</span>' + //res
                        '                        </td>' +
                        '        <td style="height: 30px">' +
                        '            <select name="cboTimeZone" id="cboTimeZone" class="RegularText" style="width:168px;">' +
			            '                <option ' + (data.TimeZone == -12 ? ' selected="selected"' : '') + ' value="-12">' + cboTimeZoneGMT + '-12</option>' + //res   //'                <option ' + (data.TimeZone == -12 ? ' selected="selected"' : '') + ' value="-12">GMT-12</option>' +
			            '                <option ' + (data.TimeZone == -11 ? ' selected="selected"' : '') + ' value="-11">' + cboTimeZoneGMT + '-11</option>' + //res   //'                <option ' + (data.TimeZone == -11 ? ' selected="selected"' : '') + ' value="-11">GMT-11</option>' +
			            '                <option ' + (data.TimeZone == -10 ? ' selected="selected"' : '') + ' value="-10">' + cboTimeZoneGMT + '-10</option>' + //res   //'                <option ' + (data.TimeZone == -10 ? ' selected="selected"' : '') + ' value="-10">GMT-10</option>' +
			            '                <option ' + (data.TimeZone == -9 ? ' selected="selected"' : '') + ' value="-9">' + cboTimeZoneGMT + '-9</option>' + //res      //'                <option ' + (data.TimeZone == -9 ? ' selected="selected"' : '') + ' value="-9">GMT-9</option>' +
			            '                <option ' + (data.TimeZone == -8 ? ' selected="selected"' : '') + ' value="-8">' + cboTimeZoneGMT + '-8</option>' + //res     //'                <option ' + (data.TimeZone == -8 ? ' selected="selected"' : '') + ' value="-8">GMT-8</option>' +
			            '                <option ' + (data.TimeZone == -7 ? ' selected="selected"' : '') + ' value="-7">' + cboTimeZoneGMT + '-7</option>' + //res     //'                <option ' + (data.TimeZone == -7 ? ' selected="selected"' : '') + ' value="-7">GMT-7</option>' +
			            '                <option ' + (data.TimeZone == -6 ? ' selected="selected"' : '') + ' value="-6">' + cboTimeZoneGMT + '-6</option>' + //res     //'                <option ' + (data.TimeZone == -6 ? ' selected="selected"' : '') + ' value="-6">GMT-6</option>' +
			            '                <option ' + (data.TimeZone == -5 ? ' selected="selected"' : '') + ' value="-5">' + cboTimeZoneGMT + '-5</option>' + //res     //'                <option ' + (data.TimeZone == -5 ? ' selected="selected"' : '') + ' value="-5">GMT-5</option>' +
			            '                <option ' + (data.TimeZone == -4 ? ' selected="selected"' : '') + ' value="-4">' + cboTimeZoneGMT + '-4</option>' + //res     //'                <option ' + (data.TimeZone == -4 ? ' selected="selected"' : '') + ' value="-4">GMT-4</option>' +
			            '                <option ' + (data.TimeZone == -3 ? ' selected="selected"' : '') + ' value="-3">' + cboTimeZoneGMT + '-3</option>' +
			            '                <option ' + (data.TimeZone == -2 ? ' selected="selected"' : '') + ' value="-2">' + cboTimeZoneGMT + '-2</option>' +
			            '                <option ' + (data.TimeZone == -1 ? ' selected="selected"' : '') + ' value="-1">' + cboTimeZoneGMT + '-1</option>' +
			            '                <option ' + (data.TimeZone == 0 ? ' selected="selected"' : '') + ' value="0">' + cboTimeZoneGMT + '</option>' +
			            '                <option ' + (data.TimeZone == 1 ? ' selected="selected"' : '') + ' value="1">' + cboTimeZoneGMT + '+1</option>' +
			            '                <option ' + (data.TimeZone == 2 ? ' selected="selected"' : '') + ' value="2">' + cboTimeZoneGMT + '+2</option>' +
			            '                <option ' + (data.TimeZone == 3 ? ' selected="selected"' : '') + ' value="3">' + cboTimeZoneGMT + '+3</option>' +
			            '                <option ' + (data.TimeZone == 4 ? ' selected="selected"' : '') + ' value="4">' + cboTimeZoneGMT + '+4</option>' +
			            '                <option ' + (data.TimeZone == 5 ? ' selected="selected"' : '') + ' value="5">' + cboTimeZoneGMT + '+5</option>' +
			            '                <option ' + (data.TimeZone == 6 ? ' selected="selected"' : '') + ' value="6">' + cboTimeZoneGMT + '+6</option>' +
			            '                <option ' + (data.TimeZone == 7 ? ' selected="selected"' : '') + ' value="7">' + cboTimeZoneGMT + '+7</option>' +
			            '                <option ' + (data.TimeZone == 8 ? ' selected="selected"' : '') + ' value="8">' + cboTimeZoneGMT + '+8</option>' +
			            '                <option ' + (data.TimeZone == 9 ? ' selected="selected"' : '') + ' value="9">' + cboTimeZoneGMT + '+9</option>' +
			            '                <option ' + (data.TimeZone == 10 ? ' selected="selected"' : '') + ' value="10">' + cboTimeZoneGMT + '+10</option>' +
			            '                <option ' + (data.TimeZone == 11 ? ' selected="selected"' : '') + ' value="11">' + cboTimeZoneGMT + '+11</option>' +
			            '                <option ' + (data.TimeZone == 12 ? ' selected="selected"' : '') + ' value="12">' + cboTimeZoneGMT + '+12</option>' +
			            '                <option ' + (data.TimeZone == 13 ? ' selected="selected"' : '') + ' value="13">' + cboTimeZoneGMT + '+13</option>' + //res
		                '            </select></td>' +
                        '        <td class="style4" style="height: 30px">' +
                        '            &nbsp;</td>' +
                        '        <td style="height: 30px">' +
                        '            &nbsp;</td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td colspan="2" style="height: 30px">' +
                        '                                    <span id="lblMultipleEmails" class="formtext">' + lblMultipleEmailsText + '</span>' + //res
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td style="height: 30px" colspan="2">' +
                        '            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" ' + (data.AutoAdjustDayLightSaving == true ? ' checked="checked"' : '') + ' /><label for="chkDayLight">' + chkDayLightText + '</label></span>'; //res   //'            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" ' + (data.AutoAdjustDayLightSaving == true ? ' checked="checked"' : '') + ' /><label for="chkDayLight">Automatically adjust for daylight savings time</label></span>';
    if (otype == "Geozone") {
        content = content +
                        '            <table class="geozonefield" cellspacing="0" cellpadding="0" border="0">' +
                        '                <tr>' +
                        '                    <td>' +
                        '                        <input id="chkCritical" type="checkbox" name="chkCritical" ' + (data.Critical == true ? ' checked="checked"' : '') + ' /><label for="chkCritical">' + chkCriticalText + '</label></td>' + //res
                        '                </tr>' +
                        '                <tr>' +
                        '                    <td>' +
                        '                        <input id="chkWarning" type="checkbox" name="chkWarning" ' + (data.Warning == true ? ' checked="checked"' : '') + ' /><label for="chkWarning">' + chkWarningText + '</label></td>' + //res
                        '                </tr>' +
                        '                <tr>' +
                        '                    <td>' +
                        '                        <input id="chkNotify" type="checkbox" name="chkNotify" ' + (data.Notify == true ? ' checked="checked"' : '') + ' /><label for="chkNotify">' + chkNotifyText + '</label></td>' + //res
                        '                </tr>' +
                        '            </table>';
    }


    content = content +
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td height="5" class="style4">' +
                        '        </td>' +
                        '        <td height="5">' +
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td colspan="2" class="formtext" height="15">' +
                        '           <a href=\'javascript:void(0)\' onclick=\'editformmainoptions();\'>' + lnkBackText + '</a>' + //res
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td height="5" class="style4">' +
                        '        </td>' +
                        '        <td height="5">' +
                        '        </td>' +
                        '    </tr>' +
                        '</table></td></tr></table>';
    var deletebuttonDisabled = "";
    if (assigned == 1) {
        deletebuttonDisabled = "disabled=\"disabled\"";
    }

    deletebutton = "<input " + deletebuttonDisabled + " type='button' class='kd-button" + (assigned == 1 ? " kd-button-disabled" : "") + "' onclick='deletePolygon(\"" + otype + "\",\"" + (otype == "Geozone" ? data.GeozoneId : escape(data.LandmarkName)) + "\", polygonPopup, selectedFeature);' value='" + btnDeletePolygonText + "'/>"; //res  //deletebutton = "<input " + deletebuttonDisabled + " type='button' class='kd-button" + (assigned == 1 ? " kd-button-disabled" : "") + "' onclick='deletePolygon(\"" + otype + "\",\"" + (otype == "Geozone" ? data.GeozoneId : escape(data.LandmarkName)) + "\", polygonPopup, selectedFeature);' value='Delete!' />";

    content = content +
                    "<div style='font-size: 1em;margin-top:10px;'>" + deletebutton + " <input type='button' class='kd-button' onclick='savePolygon_NewTZ(this, polygonPopup, selectedFeature);' value=" + SaveText + " style='margin-left:20px;' /> <input type='button' class='kd-button' value=" + CancelText + " onclick='cancelEditPolygon(polygonPopup, selectedFeature);' /></div>" + //res values Save and Cancel
                    "</form></div>";
    return '<div id="polygoninfo">' + infocontent + '</div>' + '<div id="polygoneditform" style="display:none;">' + content + '</div>';
}

function getPopupContent(cv) {
    
    /*if (ShowFields && cv.Equipment == undefined) {
        $.ajax({
            type: 'POST',
            url: rootpath + 'Vehicles.aspx?QueryType=getVehicleInfo&vehicleId=' + cv.VehicleId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {                
                cv.Equipment = msg.Field1;
                cv.TeamLeaderName = msg.Field2;
                cv.Listname = msg.Field3;
            },
            error: function (msg) {
                //content = "failed to fetch the data.";
            }
        });      
        
    }*/
    var popupContent = "<div class='popupContent'><a href='javascript:void(0)' onclick='setMapToCenter(" + cv.Longitude + "," + cv.Latitude + ");'><h6>" + cv.Description + "</a><br />";
    if ($.trim(cv.Driver) != '')
        popupContent = popupContent + cvDriverText + cv.Driver + '<br />'; //res
    var dTime = new Date(cv.OriginDateTime);
    //cv.Description = "test's\"";
    popupContent = popupContent +
                    cv.StreetAddress +

                    "<br />" + SearchHistoryTimeText + (Ext.Date.format(dTime, parent.userdateformat)) + //res
                    "<br />" + cvMyHeadingText + cv.MyHeading + //res
                    "<br />" + cvCustomSpeedText + cv.CustomSpeed + //res
                    "<br />" + cvVehicleStatusText + cv.VehicleStatus + //res
                    "<br />Color:" + cv.Color;  //res
    for (i = 0; i < parent.showsensor.split(',').length; i++) {
        if (parent.showsensor.split(',')[i] != null && parent.showsensor.split(',')[i] != "") {
            if(cv[parent.showsensor.split(',')[i]] !=undefined)
            popupContent += "<br />" + parent.showsensor.split(',')[i] + ": " + cv[parent.showsensor.split(',')[i]]
        }
    }

    if (ShowFields) {
        popupContent += "<span class='vehiclefields'>";
        popupContent += "<br />" + cvEquipmentText + "<span id='Equipment'>" + cv.Field1 + "</span>"; //res
        popupContent += "<br />" + cvTeamLeaderNameText + "<span id='TeamLeaderName'>" + cv.Field2 + "</span>"; //res
        popupContent += "<br />" + cvListNameText + "<span id='Listname'>" + cv.Field3 + "</span>"; //res
        popupContent += "</span>";
    }
    if (parent.ShowDashboardView && cv.OperationalStateName != undefined) {
        popupContent += "<br />Vehicle State: <span id='spanOperationalStateName'>" + cv.OperationalStateName + "</span>";
    }

    popupContent += "<br /></h6>";
    popupContent += "<div class='popupwizard'>" +
                    "<a href='javascript:void(0)' onclick='loadPopupsendmessage( " + cv.BoxId + ")'>" + lnkloadPopupsendmessageText + "</a>"; //res

    popupContent += " | <a href='javascript:void(0)' onclick='parent.historyMessageStore.load();parent.showHistoryTab(" + cv.VehicleId + ")'>" + lnkShowMapHistoryText + "</a>"; //res
    if (ShowRouteAssignment) {
        popupContent += " | <a href='javascript:void(0)' onclick='RouteAssignmentWindow(" + cv.VehicleId + ", \"" + cv.Description.replace(/\'/g, "[singlequote]").replace(/\"/g, "\\\"") + "\")'>" + lnkRouteAssignmentText + "</a>"; //res
    }
    if (parent.ShowDashboardView) {
        //alert(cv.LandmarkID);
        popupContent += " | <a href='javascript:void(0)' onclick='VehicleLandmarkEditWindow(" + cv.VehicleId + ", " + cv.BoxId + ")'>Edit</a>";
    }
    popupContent += "</div>" +
                    "<div id='wizardclosebar" + cv.BoxId + "' class='wizardclosebarcls' style='display: none;'><div id='wizardclosebar-closebutton'><a href='javascript:void(0)' onclick='closeWizard(" + cv.BoxId + ");'><img src='" + rootpath + "images/close.png' /></a></div></div>" +
                    "<div id='popupsendmessage" + cv.BoxId + "' class='popupsendmessagecls' style='display: none;'></div>";
    if (parent.ShowDashboardView) {
        //$('#saveVehicleOperationalStateStatus').hide();
        popupContent += "<div id='vehicleOperationalStateClosebar" + cv.BoxId + "' class='vehicleOperationalStateClosebarcls' style='display: none;'><div id='vehicleOperationalStateclosebar-closebutton'><a href='javascript:void(0)' onclick='closeVehicleOperationalState(" + cv.BoxId + ");' style='display:none;'><img src='" + rootpath + "images/close.png' /></a></div></div>" +
                    "<div id='vehicleOperationalStateEdit" + cv.BoxId + "' class='vehicleOperationalStateEditcls' style='display: none;'>" +
                    "<form id='vehicleOperationalStateFrm' name='vehicleOperationalStateFrm' method='post'>" +
                        "<input type='hidden' id='vehicleId' name='vehicleId' value='" + cv.VehicleId + "' />" +
                        "<input type='hidden' id='boxId' name='boxId' value='" + cv.BoxId + "' />" +
                        "<input type='hidden' id='landmarkId' name='landmarkId' value='" + cv.LandmarkID + "' />" +
                        "<table border=0 style='margin-top:10px;' cellpadding='5'>";
        if (cv.LandmarkID != undefined && cv.LandmarkID != null && cv.LandmarkID * 1 > 0) {
            popupContent += " <tr id='trOperationalStateServiceName' style='" + (cv.ServiceConfigurations.length==0 ? "display:none;":"") + "'>" +
                            "   <td height='30'>Service Name: </td><td><select id='OperationalStateServiceConfigId' name='OperationalStateServiceConfigId' onchange='getLandmarkDuration(" + cv.VehicleId + "," + cv.LandmarkID + "," + cv.BoxId + ")'>";
            for (var iservice = 0; iservice < cv.ServiceConfigurations.length; iservice = iservice + 2)
            {
                popupContent += "<option value='" + cv.ServiceConfigurations[iservice] + "'>" + cv.ServiceConfigurations[iservice + 1] + "</option>";
            }
            popupContent += "                                    </select></td>" +
                            " </tr>";

            popupContent += " <tr id='trLandmarkDuration' style='" + (cv.OperationalState == "200" || cv.ServiceConfigurations.length == 0 ? "display:none;" : "") + "'>" +
                        "   <td height='30'>Postpone Duration: </td><td><select id='LandmarkDuration' name='LandmarkDuration'>" +
                        "                                       <option value='0' " + (cv.LandmarkDuration == 0 ? "selected":"") + ">0</option>" +
                        "                                       <option value='12' " + (cv.LandmarkDuration == 12 ? "selected" : "") + ">12</option>" +
                        "                                       <option value='24' " + (cv.LandmarkDuration == 24 ? "selected" : "") + ">24</option>" +
                        "                                       <option value='36' " + (cv.LandmarkDuration == 36 ? "selected" : "") + ">36</option>" +
                        "                                       <option value='48' " + (cv.LandmarkDuration == 48 ? "selected" : "") + ">48</option>" +
                        "                                       <option value='60' " + (cv.LandmarkDuration == 60 ? "selected" : "") + ">60</option>" +
                        "                                       <option value='72' " + (cv.LandmarkDuration == 72 ? "selected" : "") + ">72</option>" +
                        "                                    </select> Hours</td>" +
                        " </tr>";
        }
        popupContent += " <tr><td height='20'>Vehicle State: </td><td><select id='OperationalState' name='OperationalState' onchange='onOperationalStateChange()'>" +
                        "                                       <option value='100' " + (cv.OperationalState == "100" ? "Selected" : "") + ">Available</option>" +
                        "                                       <option value='200' " + (cv.OperationalState == "200" ? "Selected" : "") + ">Unavailable</option>" +
                        "                                    </select></td>" +
                        " </tr>" +
                       
                        " <tr>" +
                        "   <td height='20' valign='top' style='vertical-align: top;'>Notes: </td><td><textarea id='OperationalStateNotes' name='OperationalStateNotes' rows='2' cols='30'>" + cv.OperationalStateNotes + "</textarea>" +
                        "                                    </td>" +
                        " </tr>" +
                        " <tr style='" + (cv.LandmarkDuration == -1 ? "": "display:none;") + "' id='trOperationalStateMessage'>" +
                        "   <td height='20' colspan='2' valign='top' style='vertical-align: top;'><span style='color:red;'>Could not get duration</span></td>" +
                        " </tr>" +
                        " <tr>" +
                        "   <td height='40' colspan='2' style='vertical-align: bottom;align: right;'>" +
                        //"       <input id='cmdSaveOperationState' class='kd-button' type='button' value='Save' name='cmdSaveOperationState' onclick='SaveOperationState(this);' /> <input id='cmdCancelOperationState' class='kd-button' type='button' value='Cancel' name='cmdCancelOperationState' onclick='closeVehicleOperationalState(" + cv.BoxId + ");' />" +
                        "       <input id='cmdSaveOperationState' class='kd-button' type='button' value='Save' name='cmdSaveOperationState' onclick='saveVehicleOperationalState(this, "+ cv.VehicleId + ", " + cv.BoxId + ");' /> <input id='cmdCancelOperationState' class='kd-button' type='button' value='Cancel' name='cmdCancelOperationState' onclick='closeVehicleOperationalState(" + cv.BoxId + ");' />" +
                        "   </td>" +
                        " </tr> " +                        
                        " </table>" +
                    "</form>" +
                    "</div>";
    }
    popupContent += "</div>";
    //alert(popupContent);
    return popupContent;
}

var savingVehicleOperationalState = false;
function saveVehicleOperationalState(b, vehicleId, boxId) {
    if (savingVehicleOperationalState)
        return '';
    var f = $(b).closest("form").serialize();

    //if ($('#txtMessage').val() == '') {
    //    $('#commandStatus').css('color', 'red').html('Message cannot be empty.').show();
    //    return;
    //}

    $.ajax({
        type: 'GET',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/SaveVehicleOperationalState',
        data: $(b).closest("form").serialize(),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            savingVehicleOperationalState = false;
            //$('#saveVehicleOperationalStateStatus').css('color', 'green').html('Successfully Saved!').show();
            closeVehicleOperationalState(boxId);
            if (parent.currentView == 'dashboard') {
                ////parent.Ext.getCmp('vehiclePager').doRefresh();
                //parent.recordUpdater.removeAll(true);
                //parent.loadingMask.show();
                //parent.recordUpdater.load();
                //alert($('#boxId').val() + ',' + $('#OperationalState option:selected').text());
                parent.updateOperationalState($('#boxId').val(), $('#OperationalState').val(), $('#OperationalState option:selected').text(), $('#OperationalStateNotes').val());
                $('#spanOperationalStateName').html($('#OperationalState option:selected').text());
            }
        },
        error: function (msg) {
            savingVehicleOperationalState = false;
            alert('failure');
        }
    });
}

function getLandmarkDuration(vehicleId, landmarkId, boxId)
{
    var serviceConfigId = $('#OperationalStateServiceConfigId').val();
    $.ajax({
        type: 'GET',
        url: rootpath + 'Vehicles.aspx?QueryType=GetLandmarkDuration',
        data: { "serviceConfigId": serviceConfigId, "vehicleId": vehicleId, "landmarkId": landmarkId, "boxId": boxId },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {
            if (msg.status == 200) {
                if (msg.landmarkDuration == -1) {
                    $('#LandmarkDuration').val(0);
                    $('#trOperationalStateMessage').show();
                }
                else {
                    $('#LandmarkDuration').val(msg.landmarkDuration);
                    $('#trOperationalStateMessage').hide();
                }
            }
            
        },
        error: function (msg) {
            alert('failure');
        }
    });
}

function onOperationalStateChange() {    
    var length = $('#OperationalStateServiceConfigId').children('option').length;    
    if ($('#OperationalState').val() == '100' && length > 0)
        $('#trLandmarkDuration').show();
    else
        $('#trLandmarkDuration').hide();
}

function getHistoryPopupContent(cv) {
    var popupContent;
    //alert('popupcontent' + cv.StopIndex);
    if (cv.StopIndex) {
        popupContent = '<b>[' + cv.StopIndex + ']</b>: ' + cv.Remarks + ' (' + cv.StopDuration + ') ' + cv.convertedArrivalDisplayDate + '<br /><b>' + cvLocationTitle + '</b> ' + cv.Location; //res
    }
    else {
        popupContent = "<div class='popupContent'><a href='javascript:void(0)' onclick='setMapToCenter(" + cv.Longitude + "," + cv.Latitude + ");'><h6>" + cv.Description + "</a><br />";

        popupContent = popupContent +
                    "<br />Driver: " + cv.Driver +
                    "<br />" + cv.StreetAddress +
                    "<br />" + SearchHistoryTimeText + cv.convertedDisplayDate + //res
                    "<br />" + cvMyHeadingText + cv.MyHeading + //res
                    "<br />" + cvCustomSpeedText + (cv.Speed == -1 ? 'N/A' : cv.Speed) + //res
                    "<br /><br /><a href='javascript:var w =HistoryInfo(" + cv.HistoryInfoId + ")'>" + cvHistoryInfoIdTitle + "</a>" + //res                    
                    "<br /></h6>";
        popupContent += "</div>";
    }

    return popupContent;
}

function setMapToCenter(lon, lat) {
    donotclosevehiclepopups = false;
    closeVehiclePopups();
    var newLoc = transformCoords(lon, lat);
    map.setCenter(newLoc, 17);    
}

function loadPopupsendmessage(BoxId) {
    closeVehicleOperationalState(BoxId);
    donotclosevehiclepopups = true;
    if ($('#popupsendmessage' + BoxId).html() != '') return;
    closeAllWizard();
    if ($('#popupsendmessage' + BoxId).html() == '') {
        $('#popupsendmessage' + BoxId).load("../Widgets/SendMessage.aspx?BoxId=" + BoxId, function () {            
            var w = vehiclePopup.size.w > 428 ? vehiclePopup.size.w - 48 : 380;
            var h;
            
            if ($('.popupContent').length == 1)
                h = vehiclePopup.size.h + 120;
            else
                h = vehiclePopup.size.h - 60;

            $('#popupsendmessage' + BoxId).show();
            $('#wizardclosebar' + BoxId).show();
            vehiclePopup.setSize(new OpenLayers.Size(w, h));
            if ($('.popupContent').length == 1) vehiclePopup.panIntoView();
        });        
    }    
}

function VehicleLandmarkEditWindow(VehicleId, BoxId) {
    closeWizard(BoxId);
    donotclosevehiclepopups = true;
    var w = vehiclePopup.size.w > 428 ? vehiclePopup.size.w - 48 : 380;
    var h;

    if ($('.popupContent').length == 1)
        h = vehiclePopup.size.h + 120;
    else
        h = vehiclePopup.size.h - 60;

    $('#vehicleOperationalStateClosebar' + BoxId).show();
    $('#vehicleOperationalStateEdit' + BoxId).show();
    vehiclePopup.setSize(new OpenLayers.Size(w, h));
    if ($('.popupContent').length == 1) vehiclePopup.panIntoView();

}

function closeWizard(BoxId) {
    donotclosevehiclepopups = false;
    $('#popupsendmessage' + BoxId).html('');
    $('#popupsendmessage' + BoxId).hide();
    $('#wizardclosebar' + BoxId).hide();
    vehiclePopup.updateSize();
}

function closeVehicleOperationalState(BoxId) {
    donotclosevehiclepopups = false;    
    $('#vehicleOperationalStateClosebar' + BoxId).hide();
    $('#vehicleOperationalStateEdit' + BoxId).hide();
    vehiclePopup.updateSize();
}

function closeAllWizard() {
    $('.popupsendmessagecls').html('');
    $('.popupsendmessagecls').hide();
    $('.wizardclosebarcls').hide();
    /*if ($.browser.msie) {
        vehiclePopup.updateSize();
    }*/
}

function landmarkformmoreoptions() {
    $(".landmarkmainoptions").hide();
    $(".geozonelandmarkmoreoption").show();
}

function geozoneformmoreoptions() {
    $(".geozonemainoptions").hide();
    $(".geozonelandmarkmoreoption").show();
}

function geozonelandmarkformmainoptions() {
    
    if ($('#lstAddOptions_0:checked').val() == 1) //geozone
    {
        $(".geozonelandmarkmoreoption").hide();
        $(".geozonemainoptions").show();

    }
    else {
        $(".geozonelandmarkmoreoption").hide();
        $(".landmarkmainoptions").show();
    }
    $(".geozonelandmarksearchhistory").hide();
}

function landmarkhistorysearch() {
    $(".landmarkmainoptions").hide();
    $(".geozonelandmarksearchhistory").show();

    $("#SearchHistoryDate").datepicker({
        showButtonPanel: true,
        //changeMonth : true,
        //changeYear : true,
        dateFormat: userDate,
        closeText: closeText//res tbd
    });  

    $('#SearchHistoryTime').val(SearchHistoryTime);
    $('#SearchHistoryDate').val(SearchHistoryDate);
}

function searchAddress() {
    var geocoder = new google.maps.Geocoder();
    var _address = $('#txtSearchAddress').val();
    if (_address.search(/([0-9.-]+).+?([0-9.-]+)/i) >= 0 && _address.search(/([a-zA-Z]+)/i) < 0) {  // if it's already in lon, lat, don't call google api
        _address = _address.trim();
        searchLat = _address.split(/[\s,]+/)[0] * 1;
        searchLon = _address.split(/[\s,]+/)[1] * 1;
        
        gotoSearchPoint(searchLon, searchLat);
    }
    else if (searchLat != 0 && searchLon != 0) {
        gotoSearchPoint(searchLon, searchLat);
    }
    else {

        geocoder.geocode({ "address": $('#txtSearchAddress').val() }, function (results, status) {
            //addressTryTimes++;
            if (status == google.maps.GeocoderStatus.OK) {
                searchLat = results[0].geometry.location.lat();
                searchLon = results[0].geometry.location.lng();
                
                gotoSearchPoint(searchLon, searchLat);
            }
            else {
                $('#searchmessage').html(searchmessageHtmlText).show().delay(2000).fadeOut(1000); //res
            }
        });
    }
}

function gotoSearchPoint(_Lon, _Lat) {
    searchMarker.removeAllFeatures();
    var newLoc = transformCoords(_Lon, _Lat);
    map.setCenter(newLoc, 16);

    var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
    var marker = new OpenLayers.Feature.Vector(point);
    //var popupContent = getPopupContent(currentVehicle);
    marker.attributes = { markerType: 'searchMarker', address: $('#txtSearchAddress').val(), lon: _Lon, lat: _Lat };

    searchMarker.addFeatures([marker]);

    onSearchMarkerClick(marker);
}

function IniGoogleAutoComplete() {
    /*
    //Google auto completion    
    var mapOptions = {
        center: new google.maps.LatLng(43.67746, -79.5850766666667),
        zoom: 13,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var cmap = new google.maps.Map(document.getElementById('map_canvas'),
          mapOptions);
    var input = document.getElementById('txtSearchAddress');
    $('#map_canvas').css("display", "none");
    autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.bindTo('bounds', cmap);

    //var infowindow = new google.maps.InfoWindow();    

    google.maps.event.addListener(autocomplete, 'place_changed', function () {

        //infowindow.close();

        //input.className = '';
        var place = autocomplete.getPlace();

        if (!place.geometry) {
            // Inform the user that the place was not found and return.
            //input.className = 'notfound';
            //alert("The address cannot be found");
            searchLat = 0;
            searchLon = 0;
            return;
        }
        var txt = $('#txtSearchAddress').val();
        var numb = txt.match(/\d/g);
        if (numb == null) {
            //alert("The address is not normal street address format, the information might not be displayed correctly.");
        }
        searchLat = place.geometry.location.lat();
        searchLon = place.geometry.location.lng();

    });
    */

    $('#txtSearchAddress').autocomplete({
        source: function (request, response) {
            searchLat = 0;
            searchLon = 0;
            var layernames = getSearchAddressOptions();
            //for (var i = 0; i < map.layers.length; i++) {
            //    var _layer = map.layers[i];
                
            //    if (!_layer.isBaseLayer && _layer.displayInLayerSwitcher && _layer.visibility && _layer.params && _layer.params.LAYERS)
            //    {
            //        layernames += _layer.params.LAYERS + ";";
            //    }
            //}
            $.ajax({
                url: "../Widgets/GeoService.aspx?input=" + request.term + "&language=" + SelectedLanguage + "&action=autocomplete&geoserver=" + geoserver+"&layernames=" + layernames,
                //url: "../Routing/SentinelService.aspx?input=" + request.term + "&language=" + SelectedLanguage + "&action=autocomplete&geoserver=" + geoserver + "&layernames=" + layernames,
                dataType: "json",
                type: "GET",
                success: function (data) {
                    //alert(data.predictions);
                    /****/
                    response($.map(data.predictions, function (item) {
                        return {
                            label: item.description,//item.name + (item.adminName1 ? ", " + item.adminName1 : "") + ", " + item.countryName,
                            value: item.value,
                            reference: item.reference
                        };
                    }));
                    /***/
                },
                error: function (error) {
                    //alert(error);
                    return "";
                }
            });
           
        },
        //source: availableTags,
        focus: function (event, ui) {
            $(this).val(ui.item.value);
        },
        minLength: 4,
        select: function (event, ui) {
            if (ui.item.reference.indexOf(',') > -1) {
                searchLat = ui.item.reference.split(/[\s,]+/)[0] * 1;
                searchLon = ui.item.reference.split(/[\s,]+/)[1] * 1;

                gotoSearchPoint(searchLon, searchLat);
            }
            else {
                searchReference = ui.item.reference;
                gotoPointByReference(ui.item.reference);                
            }
            //console.log(ui.item.reference);
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });
    

}

function gotoPointByReference(reference) {
    $.ajax({
        url: "../Widgets/GeoService.aspx?reference=" + reference + "&language=" + SelectedLanguage + "&action=getCoord",
        //url: "../Routing/SentinelService.aspx?reference=" + reference + "&language=" + SelectedLanguage + "&action=getCoord",
        dataType: "json",
        type: "GET",
        success: function (data) {
            /***/
            //var newCoord = data.result.geometry.location.lat + "," + data.result.geometry.location.lng;
            //AutocompleteRender(id, newCoord);
            //UpdateWayPointsCollection(ui.item.value, newCoord);
            //$('.input-delete').css("visibility", "visible");
            //if (alreadyKnownPointsCollection == "") {
            //    alreadyKnownPointsCollection = newCoord;
            //} else {
            //    alreadyKnownPointsCollection += ";" + newCoord;
            //}                        
            searchLat = data.result.geometry.location.lat * 1;
            searchLon = data.result.geometry.location.lng * 1;
            gotoSearchPoint(searchLon, searchLat);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function distanceBetweenPoints(latlng1, latlng2) {
    var Geographic = new OpenLayers.Projection("EPSG:4326");
    var Mercator = new OpenLayers.Projection("EPSG:900913");

    var point1 = new OpenLayers.Geometry.Point(latlng1.lon, latlng1.lat).transform(Geographic, Mercator);
    var point2 = new OpenLayers.Geometry.Point(latlng2.lon, latlng2.lat).transform(Geographic, Mercator);
    return point1.distanceTo(point2);
}


Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) != -1) {
            this.splice(ax, 1);
        }
    }
    return this;
}


function RouteAssignmentWindow(vehicleId, vehicleDescription) {
    vehicleDescription = vehicleDescription.replace(/\[singlequote\]/g, "'");
    var mypage = '../ServiceAssignment/AssignmentForm.aspx?vehicleId=' + vehicleId + '&vehicleName=' + vehicleDescription;
    var myname = 'Service Assignment';// res tbd
    var w = 850;
    var h = 500;
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
    win = window.open(mypage, myname, winprops)
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}

function HistoryInfo(dgKey) {
    var mypage = '../History/frmHistDetails.aspx?dgKey=' + dgKey
    var myname = '';
    var w = 580;
    var h = 660;
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=yes,'
    win = window.open(mypage, myname, winprops)
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}

function buildOptions(isExtended) {
    var retStr = "<option value='10'>10</option><option value='30'>30</option><option selected value='60'>60</option>";
    if (isExtended) {
        retStr += "<option value='120'>120</option> <option value='180'>180</option> <option value='240'>240</option> <option value='300'>300</option>";
        retStr += "<option value='360'>360</option> <option value='420'>420</option> <option value='480'>480</option> <option value='540'>540</option>";
        retStr += "<option value='600'>600</option> <option value='660'>660</option> <option value='720'>720</option>";
    }
    return retStr;
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return '';
}

OpenLayers.Style.createLiteral = function(value, context, feature, property) {
    if (typeof value == "string" && value.indexOf("${") != -1) {
        value = OpenLayers.String.format(value, context, [feature, property]);
        value = (isNaN(value) || !value || value.indexOf('0')==0) ? value : parseFloat(value);
    }
    return value;
};

var geozoneLayerChecked = null;
var landmarkLayerChecked = null;

OpenLayers.Control.MyCustomLayerSwitcher =
    OpenLayers.Class(OpenLayers.Control.LayerSwitcher,{
        redraw: function (){
            //if the state hasn't changed since last redraw, no need
            // to do anything. Just return the existing div.
            if (!this.checkRedraw()) {
                //alert('no redraw');
                return this.div;
            }
            
            //clear out previous layers
            this.clearLayersArray("base");
            this.clearLayersArray("data");

            var containsOverlays = false;
            var containsBaseLayers = false;

            // Save state -- for checking layer if the map state changed.
            // We save this before redrawing, because in the process of redrawing
            // we will trigger more visibility changes, and we want to not redraw
            // and enter an infinite loop.
            var len = this.map.layers.length;
            this.layerStates = new Array(len);
            for (var i = 0; i < len; i++) {
                var layer = this.map.layers[i];
                this.layerStates[i] = {
                    'name': layer.name,
                    'visibility': layer.visibility,
                    'inRange': layer.inRange,
                    'id': layer.id
                };
            }

            var layers = this.map.layers.slice();
            if (!this.ascending) { layers.reverse(); }
            for (var i = 0, len = layers.length; i < len; i++) {
                var layer = layers[i];
                var baseLayer = layer.isBaseLayer;

                if (layer.displayInLayerSwitcher && layer.name != "GeoAsset Layer") {
                    if (baseLayer) {
                        containsBaseLayers = true;
                    } else {
                        containsOverlays = true;
                    }

                    var checked = (baseLayer) ? (layer == this.map.baseLayer)
                                              : layer.getVisibility();
                    var inputId = OpenLayers.Util.createUniqueID(this.id + "_input_");
                    var inputName = (baseLayer) ? this.id + "_baseLayers" : layer.name;
                    var inputValue = layer.name;
                    var displayText = layer.name;

                    this.AddItemToLayerSwitcher(baseLayer, this, layer, checked, inputId, inputName, inputValue, displayText, true, '', '');
                    }
                else if (layer.displayInLayerSwitcher && layer.name == "GeoAsset Layer") {

                    containsOverlays = true;
                    
                    // only check a baselayer if it is *the* baselayer, check data
                    //  layers if they are visible
                    var checked = layer.getVisibility();
                    if (geozoneLayerChecked == null) geozoneLayerChecked = layer.getVisibility();

                    var inputId = OpenLayers.Util.createUniqueID(
                            this.id + "_input_"
                        );
                    this.AddItemToLayerSwitcher(baseLayer, this, layer, geozoneLayerChecked, inputId, "_Geozones_", "_Geozones_", 'Geozones', false, '', '',
                        function (o, reverseCheck) {
                            geozoneLayerChecked = reverseCheck ? !o.checked : o.checked;
                            ToggleGeoAsset();
                        }
                    );                    

                    if (landmarkLayerChecked == null) landmarkLayerChecked = layer.getVisibility();                    

                    inputId = OpenLayers.Util.createUniqueID(
                            this.id + "_input_"
                        );
                    
                    this.AddItemToLayerSwitcher(baseLayer, this, layer, landmarkLayerChecked, inputId, "_Landmarks_", "_Landmarks_", 'Landmarks', false, 'landmark', '',
                        function (o, reverseCheck) {
                            landmarkLayerChecked = reverseCheck ? !o.checked : o.checked;
                            $('input:checkbox.landmarkcategory').prop('checked', landmarkLayerChecked);
                            ToggleGeoAsset();
                    }
                    );

                    for (var j = 0; j < CategoryList.length; j++) {
                        if (CategoryList[j][1] <= 0) continue;
                        inputId = OpenLayers.Util.createUniqueID(
                                this.id + "_input_category_" + CategoryList[j][1]
                            );

                        this.AddItemToLayerSwitcher(baseLayer, this, layer, CategoryList[j][2], inputId, "_Landmarks_category" + CategoryList[j][1], "_Landmarks_category" + CategoryList[j][1], CategoryList[j][0], false, 'landmarkcategory', CategoryList[j][1],
                            function (o, reverseCheck) {
                                landmarkLayerChecked = reverseCheck ? !o.checked : o.checked;
                                $(o).prop('checked', landmarkLayerChecked);
                                if (!landmarkLayerChecked)
                                {
                                    $('input:checkbox.landmark').prop('checked', landmarkLayerChecked);
                                }
                                else
                                {
                                    if ($("input:checkbox.landmarkcategory:not(:checked)").length == 0) {
                                        $('input:checkbox.landmark').prop('checked', true);
                                        landmarkLayerChecked = true;
                                    }
                                    else
                                        landmarkLayerChecked = false;
                                }
                                ToggleGeoAsset();
                            }
                        );
                    }
                }
                    }

            // if no overlays, dont display the overlay label
            this.dataLbl.style.display = (containsOverlays) ? "" : "none";

            // if no baselayers, dont display the baselayer label
            this.baseLbl.style.display = (containsBaseLayers) ? "" : "none";

            return this.div;

        },
        AddItemToLayerSwitcher: function (baseLayer, o, layer, checked, inputId, inputName, inputValue, displayText, pushToGroupArray, c, categoryId, oncheckboxclick) {
                    // create input element
            var inputElem = document.createElement("input");
                        // The input shall have an id attribute so we can use
                        // labels to interact with them.
                    inputElem.id = inputId;
            inputElem.name = inputName;//(baseLayer) ? o.id + "_baseLayers" : layer.name;
            inputElem.type = (baseLayer) ? "radio" : "checkbox";
            inputElem.value = inputValue;//layer.name;
            inputElem.checked = checked;
            inputElem.defaultChecked = checked;
            inputElem.className = "olButton " + c;
                    inputElem._layer = layer.id;
            inputElem._layerSwitcher = o.id;
            if (categoryId != '')
                inputElem.setAttribute("categoryId", categoryId);

                    if (!baseLayer && !layer.inRange) {
                        inputElem.disabled = true;
                    }

            //if (oncheckboxclick && typeof oncheckboxclick === 'function') {
            //    inputElem.onclick = function () { oncheckboxclick(this, true); };
            //}

                    // create span
                    var labelSpan = document.createElement("label");
                    // this isn't the DOM attribute 'for', but an arbitrary name we
                    // use to find the appropriate input element in <onButtonClick>
                    labelSpan["for"] = inputElem.id;
                    OpenLayers.Element.addClass(labelSpan, "labelSpan olButton");
                    labelSpan._layer = layer.id;
            labelSpan._layerSwitcher = o.id;
                    if (!baseLayer && !layer.inRange) {
                        labelSpan.style.color = "gray";
                    }
            labelSpan.innerHTML = displayText;//layer.name;
            labelSpan.style.verticalAlign = (baseLayer) ? "bottom"
                                                        : "baseline";

            if (oncheckboxclick && typeof oncheckboxclick === 'function') {
                inputElem.onclick = function () { oncheckboxclick(this, true); };

                labelSpan.onclick = function () {
                    oncheckboxclick(document.getElementById(inputId), false);
                };
            }

                    // create line break
                    var br = document.createElement("br");


            if (pushToGroupArray) {
                var groupArray = (baseLayer) ? o.baseLayers
                                             : o.dataLayers;
                groupArray.push({
                    'layer': layer,
                    'inputElem': inputElem,
                    'labelSpan': labelSpan
                });
            }

            var groupDiv = (baseLayer) ? o.baseLayersDiv
                                       : o.dataLayersDiv;
                    groupDiv.appendChild(inputElem);
                    groupDiv.appendChild(labelSpan);
                    groupDiv.appendChild(br);
        },
        CLASSNAME: "OpenLayers.Control.MyCustomLayerSwitcher"
    });

function ToggleGeoAsset() {

    removePoloygonPopups();
    var selectedLandmarkCategories = [];

    $("input:checkbox.landmarkcategory").each(function () {
        var cid = $(this).attr("CategoryId");
        var checked = $(this).prop("checked");
        if(checked)
            selectedLandmarkCategories.push(cid);

        for (var i = 0; i < CategoryList.length; i++) {
            if(CategoryList[i][1] == cid)
            {
                CategoryList[i][2] = checked ? 1 : 0;
                continue;
            }
        }
    });

    visibleGeoLandmarkFeature = [];
    for (var i = 0; i < geoLandmarkFeatures.length; i++)
    {
        var feature = geoLandmarkFeatures[i];

        if ( feature.fid.split(":::")[0] == "LandmarkCircle" || feature.fid.split(":::")[0] == "Landmark") {
            if (selectedLandmarkCategories.indexOf(feature.CategoryId) > -1)
                visibleGeoLandmarkFeature.push(feature);            
        }
        else if (geozoneLayerChecked && feature.fid.split(":::")[0] == "Geozone") {
            visibleGeoLandmarkFeature.push(feature);
        }           

    }
        
    vectors.removeAllFeatures();
    if (visibleGeoLandmarkFeature.length > 0) {
        vectors.addFeatures(visibleGeoLandmarkFeature);
    }

    vectors.redraw();
}

function buildCategoryDropDown(){
    var s = '<select name="ddlCategory" id="ddlCategory" class="formtext bsmforminput" style="width:173px;">';
    for(var i=0;i<CategoryList.length;i++)
    {
        s += '<option value="' + CategoryList[i][1] + '">' + CategoryList[i][0] + '</option>'
    }
    s += '</select>';
    return s;
}

function redrawLandmark(landMarkId, popup, geoassetname) {
    
    LandmarkDrawMode = 'redraw';
    RedrawLandmarkId = landMarkId;
    RedrawLandmarkName = $('#' + geoassetname).val();
    $('#redrawLandmarkName').html(RedrawLandmarkName);
    map.removePopup(popup);
    selectControl.unselect(selectedFeature);
    
    btnMapToolDrawPolygon.fireEvent('click', btnMapToolDrawPolygon);
    
    $('#redrawbar').show();
}

function cancelredraw() {
    LandmarkDrawMode = '';
    RedrawLandmarkId = 0;
    RedrawLandmarkName = '';
    $('#saveredrawchanges').hide();
    $('#redrawbar').hide();
    clearEditForm();
    btnMapToolPan.fireEvent('click', btnMapToolPan);
}

function saveredrawchanges() {
    var landmarkType = 'Landmark';
    if (document.getElementById('circleToggle').checked == true)
        landmarkType = 'LandmarkCircle';

    var c = currentEditFeature.geometry.components[0];
    var pointSets = "";
    var centerPoint = "";

    if (landmarkType == 'Landmark') {
        for (var i = 0; i < c.components.length - 1; i++) {
            c.components[i].transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));

            var x = c.components[i].x;
            var y = c.components[i].y;

            if (pointSets == "")
                pointSets = y + "|" + x;
            else
                pointSets = pointSets + "," + y + "|" + x;

            c.components[i].transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));
        }
    }

    var radius = '';

    var latlon = currentEditFeature.geometry.getBounds().getCenterLonLat();
    latlon.transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
    centerPoint = latlon.lat + "," + latlon.lon;
    latlon.transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));

    if (landmarkType == "LandmarkCircle") {
        var area = currentEditFeature.geometry.getGeodesicArea(new OpenLayers.Projection("EPSG:900913"));
        radius = (0.565352 * Math.sqrt(area)).toFixed(0);
    }

    $.ajax({
        type: 'POST',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/EditGeozoneLandmark_NewTZ',
        data: JSON.stringify({ otype: encodeURI(landmarkType), oid: encodeURI(RedrawLandmarkName), pointSets: pointSets, centerpoint: centerPoint, newradius: radius }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            var r = eval('(' + msg.d + ')');
            if (r.status == 200) {
                
                currentEditFeature.fid = landmarkType + ":::" + RedrawLandmarkName;
                currentEditFeature.CategoryId = selectedFeature.CategoryId;
                currentEditFeature.CategoryName = selectedFeature.CategoryName;
                
                vectors.removeFeatures(selectedFeature);

                geoLandmarkFeatures = removeElementFromArray(geoLandmarkFeatures, selectedFeature);
                visibleGeoLandmarkFeature = removeElementFromArray(visibleGeoLandmarkFeature, selectedFeature);

                geoLandmarkFeatures.push(currentEditFeature);
                visibleGeoLandmarkFeature.push(currentEditFeature);
                currentEditFeature = null;

                LandmarkDrawMode = '';
                RedrawLandmarkId = 0;
                RedrawLandmarkName = '';
                $('#saveredrawchanges').hide();
                $('#redrawbar').hide();

                $('#messagebar').html("Sucessfully redraw the landmark.").show().delay(3000).fadeOut(1000);

                btnMapToolPan.fireEvent('click', btnMapToolPan);
            }
            else {
                alert('Failed to save the changes.'); //res
            }
        },
        error: function (msg) {            
            alert(msg); //res
        }
    });

}

function removeElementFromArray(arr, val) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] === val) {
            arr.splice(i, 1);
            i--;
        }
    }
    return arr;
}

