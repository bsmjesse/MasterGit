var format = 'image/png';
var geoserver = 'http://geomap.sentinelfm.com:9090/geoserver/wms';

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
var searchMarkerPopup;
var undolist = [];
var changeslist = [];
var previousPointSets;
var currentEditFeature = null;
var currentEditPopup = null;
var maxVehiclesOnMap = parent.maxVehiclesOnMap;
var vehicleClickControl;
var dblclick

var geoLandmarkFeatures = [];
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

var ClosestVehiclesRadius = 500;
var ClosestVehiclesNumOfVehicles = 100;
//var SearchHistoryDate = 
var SearchHistoryTime = '11:00';

var yesterday = new Date(); yesterday.setDate(yesterday.getDate() - 1);
var yesterdayyyyy = yesterday.getFullYear().toString();
var yesterdaymm = (yesterday.getMonth()+1).toString(); // getMonth() is zero-based
var yesterdaydd = yesterday.getDate().toString();
//var SearchHistoryDate = (yesterdaydd[1] ? yesterdaydd : "0" + yesterdaydd[0]) + "/" + (yesterdaymm[1] ? yesterdaymm : "0" + yesterdaymm[0]) + "/" + yesterdayyyyy;
var SearchHistoryDate = (yesterdaymm[1] ? yesterdaymm : "0" + yesterdaymm[0]) + "/" + (yesterdaydd[1] ? yesterdaydd : "0" + yesterdaydd[0]) + "/" + yesterdayyyyy;
var SearchHistoryRadius = 2000;
var SearchHistoryMinutes = 60;

var AllHistoryRecords = [];

var SearchAddressOriginHeight = 66;

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
                                else if (currentVehicle.ImagePath != '') {
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

function initMap(layerInfo1) {
    var renderer;
    try {
        if (parent.ifShowArcgis && !ISSECURE) {

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
         new OpenLayers.Control.LayerSwitcher(
         {
             'ascending': true
         }
         ),
         new OpenLayers.Control.ScaleLine(),
         new OpenLayers.Control.MousePosition(),
         new OpenLayers.Control.KeyboardDefaults()
         ]
      }
      ;

        // create OSM layer
     if (parent.ifShowTheme1 && !ISSECURE)
            var mapnik = new OpenLayers.Layer.OSM("Theme 1");

     if (parent.ifShowTheme2 && !ISSECURE) {
            var cloudmade = new OpenLayers.Layer.CloudMade("Theme 2",
              {
                  key: '8ee2a50541944fb9bcedded5165f09d9'
              }
              );
        }

        if (parent.ifShowGoogleStreets) {
            var gmap = new OpenLayers.Layer.Google(
          "Google Streets",
          {
              sphericalMercator: true
          }
          );
        }

        if (parent.ifShowGoogleHybrid) {
            var ghyb = new OpenLayers.Layer.Google(
          "Google Hybrid",
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
      if (parent.ifShowBingRoads && !ISSECURE) {
            var veroad = new OpenLayers.Layer.VirtualEarth(
          "Bing Roads",
          {
              'type': VEMapStyle.Road, sphericalMercator: true
          }
          );
        }
        
      if (parent.ifShowBingHybrid && !ISSECURE) {
            var vehyb = new OpenLayers.Layer.VirtualEarth(
          "Bing Hybrid",
          {
              'type': VEMapStyle.Hybrid, sphericalMercator: true
          }
          );
      }

      if (parent.ifShowAerial && !ISSECURE) {
          var aerial = new OpenLayers.Layer.Yahoo(
	        "Aerial",
	        { 'type': YAHOO_MAP_HYB, sphericalMercator: true }
	        ); 
      }



      if (parent.overlaysettings.nexradweatherradar && !ISSECURE) {
            // Weather Radar Overlay Layer
            var nexrad = new OpenLayers.Layer.WMS(
              "Nexrad Weather Radar",
              "http://mesonet.agron.iastate.edu/cgi-bin/wms/nexrad/n0r.cgi?",
              {
                  layers: "nexrad-n0r", transparent: "TRUE", format: 'image/png'
              }
              ,
              {
                  isBaseLayer: false, buffer: 0, singleTile: false, opacity: 0.5, visibility: parent.overlaysettings.nexradweatherradarVisibility
              }
          );
        }

          if (parent.overlaysettings.water && !ISSECURE) {
            var oiltrax_poi_water = new OpenLayers.Layer.WMS("Water",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_water', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.waterVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
          );
        }
          if (parent.overlaysettings.wellsite && !ISSECURE) {
            var oiltrax_poi_wellsite = new OpenLayers.Layer.WMS("Wellsite",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_wellsite', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.wellsiteVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
        }
          if (parent.overlaysettings.batteries && !ISSECURE) {
            var oiltrax_poi_batteries = new OpenLayers.Layer.WMS("Batteries",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_batteries', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.batteriesVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
        }

          if (parent.overlaysettings.otherfacilities && !ISSECURE) {
            var oiltrax_poi_otherfacilities = new OpenLayers.Layer.WMS("Other Facilities",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_otherfacilities', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.otherfacilitiesVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
        }

          if (parent.overlaysettings.sasktellsd && !ISSECURE) {
            var oiltrax_sasklsd = new OpenLayers.Layer.WMS("Sasktel LSD",
              geoserver,
              {
                  'layers': 'oiltrax:sask_lsd', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.sasktellsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
           );
        }
          if (parent.overlaysettings.manbslsd && !ISSECURE) {
            var oiltrax_manbclsd = new OpenLayers.Layer.WMS("Man-BC LSD",
              geoserver,
              {
                  'layers': 'oiltrax:manbc_lsd', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.manbslsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
          );
        }
          if (parent.overlaysettings.albertalsd && !ISSECURE) {
            var oiltrax_albertalsd = new OpenLayers.Layer.WMS("Alberta LSD",
              geoserver,
              {
                  'layers': 'oiltrax:alberta_lsd', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.albertalsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
           );
        }

          if (parent.overlaysettings.trails && !ISSECURE) {
            var oiltrax_trails = new OpenLayers.Layer.WMS("Trails",
              geoserver,
              {
                  'layers': 'oiltrax:trail', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.trailsVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
          );
        }
          if (parent.overlaysettings.additionalroads && !ISSECURE) {
            var oiltrax_goat = new OpenLayers.Layer.WMS("Additional Roads",
              geoserver,
              {
                  'layers': 'oiltrax:goat', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.additionalroadsVisibility, opacity: 1, singleTile: true, ratio: 1
              }
           );
        }
          if (parent.overlaysettings.additionalroads2 && !ISSECURE) {
            var oiltrax_goat2 = new OpenLayers.Layer.WMS("Additional Roads 2",
              geoserver,
              {
                  'layers': 'oiltrax:goat2', transparent: true, format: format
              }
              ,
              {
                  visibility: parent.overlaysettings.additionalroads2Visibility, opacity: 1, singleTile: true, ratio: 1
              }
          );
          }

          if (parent.overlaysettings.CNRailMilePosts && !ISSECURE) {
              var cnrail_mileposts = new OpenLayers.Layer.WMS("CNRail MilePost",
                geoserver,
                { 'layers': 'cnrail:cnrail_mileposts', transparent: true, format: format },
                { visibility: parent.overlaysettings.CNRailMilePostsVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
          }

          if (parent.overlaysettings.CNRailMaintenanceOfWay && !ISSECURE) {
              var cnrail_maintenanceofway = new OpenLayers.Layer.WMS("CNRail Maintenance Of Way",
                geoserver,
                { 'layers': 'cnrail:cnrail_mow', transparent: true, format: format },
                { visibility: parent.overlaysettings.CNRailMaintenanceOfWayVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );

          }

          if (parent.overlaysettings.UPRailNetwork && !ISSECURE) {
              var uprail_main = new OpenLayers.Layer.WMS("UPRail Network",
                geoserver,
                { 'layers': 'uprail:up_main', transparent: true, format: format },
                { visibility: parent.overlaysettings.UPRailNetworkVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
          }

          if (parent.overlaysettings.UPRailMilePosts && !ISSECURE) {
              var uprail_mileposts = new OpenLayers.Layer.WMS("UPRail MilePost",
                geoserver,
                { 'layers': 'uprail:uprail_mileposts', transparent: true, format: format },
                { visibility: parent.overlaysettings.UPRailMilePostsVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
          }

          if (parent.overlaysettings.UPRailRightOfWay && !ISSECURE) {
              var UPRailRightOfWay = new OpenLayers.Layer.WMS("UPRail Right Of Way",
                geoserver,
                { 'layers': 'uprail:right_of_way', transparent: true, format: format },
                { visibility: parent.overlaysettings.UPRailRightOfWayVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );
          }

          if (parent.overlaysettings.UPRailPoly && !ISSECURE) {
              var UPRailPoly = new OpenLayers.Layer.WMS("UPRail Poly",
                geoserver,
                { 'layers': 'uprail:uprail_polygon', transparent: true, format: format },
                { visibility: parent.overlaysettings.UPRailPolyVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );
          }

          if (parent.overlaysettings.OFSCTrails && !ISSECURE) {
              //OFSC Trails layer
              var ofsc_trails = new OpenLayers.Layer.WMS("OFSC Trails",
                geoserver,
                { 'layers': 'ofsc:OFSC_trails', transparent: true, format: format },
                { visibility: parent.overlaysettings.OFSCTrailsVisibility, opacity: 1.0, singleTile: true, ratio: 1 }
            );
          }

          if (parent.overlaysettings.CityWards && !ISSECURE) {
              var toronto_wards = new OpenLayers.Layer.WMS("City Wards",
                geoserver,
                { 'layers': 'toronto:city_wards', transparent: true, format: format },
                { visibility: parent.overlaysettings.CityWardsVisibility, opacity: 1.0, singleTile: true, ratio: 1 }
            );                
          }

          if (parent.overlaysettings.TDSBSchools && !ISSECURE) {
              var toronto_tdsbschools = new OpenLayers.Layer.WMS("TDSB Schools",
                geoserver,
                { 'layers': 'toronto:TDSBSchool', transparent: true, format: format },
                { visibility: parent.overlaysettings.TDSBSchoolsVisibility, opacity: 1.0, singleTile: true, ratio: 1 }
            );
          }

          if (parent.overlaysettings.BNSFRailway && !ISSECURE) {
              //BNSF Layers
              var bnsf_railway = new OpenLayers.Layer.WMS("BNSF Railway",
                geoserver,
                { 'layers': 'bnsf:railway', transparent: true, format: format },
                { visibility: parent.overlaysettings.BNSFRailwayVisibility, opacity: 0.7, singleTile: true, ratio: 1 }
            );
          }

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
             title: 'Identify features by clicking',
             layers: [parent.overlaysettings.water ? oiltrax_poi_water : ""],
             queryVisible: true
         }
         ),
          hover: new OpenLayers.Control.WMSGetFeatureInfo(
         {
             url: geoserver,
             title: 'Identify features by clicking',
             layers: [parent.overlaysettings.water ? oiltrax_poi_water : ""],
             hover: true,
             queryVisible: true
         }
         )
      }
      ;


//     if (parent.VehicleClustering) {
//     }
//     else if (parent.overlaysettings.vehicleDrivers) {
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
//         visibility: parent.overlaysettings.vehicleDriversVisibility
//         });
//     }
//     else if (parent.overlaysettings.vehiclenames) {
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
//            visibility: parent.overlaysettings.vehiclenamesVisibility
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
            strokeDashstyle: "${strokeDashstyle}"
        }, {
            context: {
                width: function (feature) {
                    return (feature.cluster) ? 2 : 1;
                },
                radius: function (feature) {
                    var pix = 2;
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
            renderers: renderer, visibility: parent.overlaysettings.geoassetVisibility
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
            renderers: renderer, visibility: parent.overlaysettings.geoassetVisibility
        });

        var searchMarkersStyle = new OpenLayers.Style({
            externalGraphic: "../images/locator.png",
            graphicWidth: "40",
            graphicHeight: "40"
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

        if (parent.ifShowTheme1 && !ISSECURE)
            map.addLayer(mapnik);
        if (parent.ifShowTheme2 && !ISSECURE)
            map.addLayer(cloudmade);
        if (parent.ifShowArcgis && !ISSECURE)
            map.addLayer(arcgisLayer);
        if (parent.ifShowGoogleStreets)
            map.addLayer(gmap);
        if (parent.ifShowGoogleHybrid)
            map.addLayer(ghyb);
        if (parent.ifShowBingRoads && !ISSECURE)
            map.addLayer(veroad);
        if (parent.ifShowBingHybrid && !ISSECURE)
            map.addLayer(vehyb);
        if (parent.ifShowAerial && !ISSECURE)
            map.addLayer(aerial);


        if (parent.ifShowNavteq && !ISSECURE) {
            //'https: //maps.nlp.nokia.com/maptiler/v2/maptile/newest/normal.day';
            var Navteq = new OpenLayers.Layer.XYZ(
                "Navteq",
                [
                    "https://maps.nlp.nokia.com/maptiler/v2/maptile/newest/normal.day/${z}/${x}/${y}/256/png8?app_id=v5HljYEynPujgUUkNmny&token=x14y1MmQaoSerjNQKGsABw"
                ],
                {
                    attribution: "&copy; 2013 Nokia</span>&nbsp;<a href='http://maps.nokia.com/services/terms' target='_blank' title='Terms of Use' style='color:#333;text-decoration: underline;'>Terms of Use</a></div> <img src='http://api.maps.nokia.com/2.2.4/assets/ovi/mapsapi/by_here.png' border='0'>"
                    /*,transitionEffect: "resize"*/
                }
            );
            map.addLayer(Navteq);
        }

        /*var map = new OpenLayers.Map({
            div: "map",
            projection: "EPSG:900913",
            layers: [
        new OpenLayers.Layer.XYZ(
            "HereMap",
            [
                "MAP_TILE_BASE_URL/${z}/${x}/${y}/256/png8?lg=ENG&app_id=YOUR_APP_ID&token=YOUR_TOKEN"
            ],
            {
                attribution: "&copy; 2013 Nokia</span>&nbsp;<a href='http://maps.nokia.com/services/terms' target='_blank' title='Terms of Use' style='color:#333;text-decoration: underline;'>Terms of Use</a></div> <img src='http://api.maps.nokia.com/2.2.4/assets/ovi/mapsapi/by_here.png' border='0'>",
                transitionEffect: "resize"
            }
        )
    ],
            center: [0, 0],
            zoom: 1
        });*/

        

        if (map.layers.length > 1 && map.layers[0].name.toLowerCase().indexOf("google") != -1) {            
            map.setBaseLayer(map.layers[1]);
            map.setBaseLayer(map.layers[0]);
        }
        
        map.addLayer(vectors);
        map.addLayer(searchMarker);
        map.addLayer(searchArea);
        //map.addLayer(workorderLayer);

        //trafficLayer.setMap(map.getLayer('gmap').mapObject);

        if (!map.getCenter()) {
            map.zoomToMaxExtent();
            //map.setCenter(transformCoords(-72.872559, 46.767074), 8);
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
            var fcmq_sentiers = new OpenLayers.Layer.WMS("FCMQ Sentiers No",
            geoserver,
            { 'layers': 'fcmq:NoSentiersQC', transparent: true, format: format },
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
            if (parent.overlaysettings.water && !ISSECURE) {
                map.addLayer(oiltrax_poi_water);
            }
            if (parent.overlaysettings.wellsite && !ISSECURE) {
                map.addLayer(oiltrax_poi_wellsite);
            }
            if (parent.overlaysettings.batteries && !ISSECURE) {
                map.addLayer(oiltrax_poi_batteries);
            }
            if (parent.overlaysettings.otherfacilities && !ISSECURE) {
                map.addLayer(oiltrax_poi_otherfacilities);
            }
            if (parent.overlaysettings.sasktellsd && !ISSECURE) {
                map.addLayer(oiltrax_sasklsd);
            }
            if (parent.overlaysettings.albertalsd && !ISSECURE) {
                map.addLayer(oiltrax_albertalsd);
            }
            if (parent.overlaysettings.manbslsd && !ISSECURE) {
                map.addLayer(oiltrax_manbclsd);
            }
            if (parent.overlaysettings.trails && !ISSECURE) {
                map.addLayer(oiltrax_trails);
            }
            if (parent.overlaysettings.additionalroads && !ISSECURE) {
                map.addLayer(oiltrax_goat);
            }
            if (parent.overlaysettings.additionalroads2 && !ISSECURE) {
                map.addLayer(oiltrax_goat2);
            }
            if (parent.overlaysettings.nexradweatherradar && !ISSECURE) {
                map.addLayer(nexrad);
            }
            if (parent.overlaysettings.CNRailMilePosts && !ISSECURE) {
                map.addLayer(cnrail_mileposts);
            }
            if (parent.overlaysettings.CNRailMaintenanceOfWay && !ISSECURE) {
                map.addLayer(cnrail_maintenanceofway);
            }

            if (parent.overlaysettings.UPRailNetwork && !ISSECURE) {
                map.addLayer(uprail_main);                
            }

            if (parent.overlaysettings.UPRailMilePosts && !ISSECURE) {
                map.addLayer(uprail_mileposts);
            }

            if (parent.overlaysettings.UPRailRightOfWay && !ISSECURE) {
                map.addLayer(UPRailRightOfWay);
            }

            if (parent.overlaysettings.UPRailPoly && !ISSECURE) {
                map.addLayer(UPRailPoly);
            }

            if (parent.overlaysettings.OFSCTrails && !ISSECURE) {
              map.addLayer(ofsc_trails);
            }

            if (parent.overlaysettings.CityWards && !ISSECURE) {
                map.addLayer(toronto_wards);                
            }

            if (parent.overlaysettings.TDSBSchools && !ISSECURE) {
                map.addLayer(toronto_tdsbschools);
            }

            if (parent.overlaysettings.BNSFRailway && !ISSECURE) {
                map.addLayer(bnsf_railway);
            }

            map.addLayers([
            // oiltrax_polygon,
         highlightLayer]);

            /*if (parent.overlaysettings.vehiclenames) {
            map.addLayer(vehiclenamesLayer);
            }*/
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

                        if (parent.overlaysettings.vehicleDrivers) {
                            return feature.attributes.Driver;
                        }
                        else if (parent.overlaysettings.vehiclenames) {
                            return feature.attributes.Description;
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
                        if (parent.overlaysettings.vehicleDrivers) {
                            return feature.attributes.Driver;
                        }
                        else if (parent.overlaysettings.vehiclenames) {
                            return feature.attributes.Description;
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
            strategies: parent.VehicleClustering ? [markerstrategy]: [],
            displayInLayerSwitcher: false,
            isBaseLayer: false,
            styleMap: new OpenLayers.StyleMap({
                "default": parent.VehicleClustering ? clusteringMarkersStyle : markersStyle
            }),
            renderers: markersRenderer, visibility: true

        });


        map.addLayer(markers);

//        if (parent.VehicleClustering) {
//        }
//        else if (parent.overlaysettings.vehicleDrivers) {
//            map.addLayer(vehicleDriversLayer);
//        }
//        else if (parent.overlaysettings.vehiclenames) {
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

        vehicleClickControl = new OpenLayers.Control.SelectFeature(
                [vectors, markers, histories, searchMarker],
                {
                    onSelect: onVehicleClick,
                    autoActivate: true,
                    toggle: true, 
                    clickout:false
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
                map.moveTo(lastlonlat, map.getZoom()+1);
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
            if (parent.overlaysettings.geoasset) {
                DrawGeozone_NewTZ();
                DrawLandmark_NewTZ();
            }
        }
        catch (err) {
        }
        //End

        DrawWorkorder();

        if (!parent.mapAssets)
            map.zoomTo(4);
    }
    catch (err) {
    }


    function createGezoneLandmark(event) {
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
                var distance = point1.distanceTo(point2);
                if(distance > polygonRadius)
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
                var area = event.feature.geometry.getArea();
                var radius = 0.565352 * Math.sqrt(area);
                content = "<h6 style=\"margin:0 0 10px 10px;border-bottom:1px solid #cccccc\">New circle type landmark</h6>";
                content += "<div style='font-size:1em'><form id='polygonform'>" +
                            "<input type='hidden' id='isNew' name='isNew' value='1' />" +
                            "<input type='hidden' id='txtX' name='txtX' value='" + txtX + "' />" +
                            "<input type='hidden' id='txtY' name='txtY' value='" + txtY + "' />" +
                            "<input type='hidden' id='lstAddOptions' name='lstAddOptions' value='0' />" +
                                '<table class="landmarkfield landmarkmainoptions" cellspacing="0" cellpadding="0" border="0" style="margin-left:10px;">' +
                                '<tr><td style="padding-top:10px;"><table border="0">' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px; width: 139px;">' +
                                '            <span id="lblLandmarkNameTitle" class="formtext">Landmark Name (*):</span>' +
                                '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkName" type="text" id="txtLandmarkName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblContactNameTitle" class="formtext">Contact Name:</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtContactName" type="text" id="txtContactName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblLandmarkDescriptionTitle" class="formtext">Landmark Description:</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkDesc" type="text" id="txtLandmarkDesc" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblPhoneTitle" class="formtext">Phone :</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtPhone" type="text" id="txtPhone" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 15px;">' +
                                '            <span id="lblRadiusTitle" class="formtext">Radius</span>' +
                                '            (' +
                                '            <span id="lblUnit">m</span>):' +
                                '            <span id="valRadius" style="color:Red;visibility:hidden;">*</span><span id="valRangeRadius" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" height="15">' +
                                '            <input name="txtRadius" type="text" id="txtRadius" class="formtext bsmforminput" style="width:173px;" value="' + radius.toFixed(0) + '" /></td>' +
                                '    </tr>';
                if (ShowCallTimer) {
                    content += '    <tr>' +
                                '        <td class="formtext" style="height: 30px;">' +
                                '            <span id="lblCallTimerTitle" class="formtext">Timeout: </span></td>' +
                                '        <td class="formtext" height="30">' + CallTimerSelections + '</td>' +
                                '    </tr>';
                }
                content += '   <tr>' +
                                '       <td colspan="2" class="formtext" style="height: 30px;">' +
                                '           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" /><span style="margin-left:3px;">Private</span>' +
                                '           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" checked /><span style="margin-left:3px;">Public</span>' +
                                '       </td>' +
                                '   </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '           <a href=\'javascript:void(0)\' onclick=\'landmarkformmoreoptions();\'>More Options</a>';
                if (ShowMapHistorySearch) {
                    content += '           <a href=\'javascript:void(0)\' onclick=\'landmarkhistorysearch();\'>Search History</a>';
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
                                '            <span id="lblEmailTitle" class="formtext">Email:</span>' +
                                '            </td>' +
                                '        <td>' +
                                '            <input name="txtEmail" type="text" id="txtEmail" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr style="display:none;">' +
                                '        <td class="style4" style="height: 30px">' +
                                '            <span id="lblPhone" class="formtext"> Phone:</span>' +
                                '                                        <span id="RegularExpressionValidator1" class="formtext" style="color:Red;visibility:hidden;">*</span>' +
                                '                        </td>' +
                                '        <td>' +
                                '            <input name="txtPhoneSMS" type="text" id="txtPhoneSMS" disabled="disabled" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblTimeZoneTitle" class="formtext">Time Zone:</span>' +
                                '                        </td>' +
                                '        <td>' +
                                '            <select name="cboTimeZone" id="cboTimeZone" class="RegularText" style="width:168px;">' +
			                    '                <option value="-12">GMT-12</option>' +
			                    '                <option value="-11">GMT-11</option>' +
			                    '                <option value="-10">GMT-10</option>' +
			                    '                <option value="-9">GMT-9</option>' +
			                    '                <option value="-8">GMT-8</option>' +
			                    '                <option value="-7">GMT-7</option>' +
			                    '                <option value="-6">GMT-6</option>' +
			                    '                <option value="-5">GMT-5</option>' +
			                    '                <option value="-4">GMT-4</option>' +
			                    '                <option value="-3">GMT-3</option>' +
			                    '                <option value="-2">GMT-2</option>' +
			                    '                <option value="-1">GMT-1</option>' +
			                    '                <option value="0">GMT</option>' +
			                    '                <option value="1">GMT+1</option>' +
			                    '                <option value="2">GMT+2</option>' +
			                    '                <option value="3">GMT+3</option>' +
			                    '                <option value="4">GMT+4</option>' +
			                    '                <option value="5">GMT+5</option>' +
			                    '                <option value="6">GMT+6</option>' +
			                    '                <option value="7">GMT+7</option>' +
			                    '                <option value="8">GMT+8</option>' +
			                    '                <option value="9">GMT+9</option>' +
			                    '                <option value="10">GMT+10</option>' +
			                    '                <option value="11">GMT+11</option>' +
			                    '                <option value="12">GMT+12</option>' +
			                    '                <option value="13">GMT+13</option>' +
		                        '            </select></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2">' +
                                '                                    <span id="lblMultipleEmails" class="formtext">*Multiple email addresses Must be Separated by semicolon or comma</span>' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td style="height: 30px" colspan="2">' +
                                '            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" /><label for="chkDayLight">Automatically adjust for daylight savings time</label></span>' +
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
                                '           <a href=\'javascript:void(0)\' onclick=\'geozonelandmarkformmainoptions();\'>Back</a>' +
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
                                "       <div id='SearchHistoryTitle' style='font-weight: bold; color: green;margin:10px 0;'>Search History:</div>" +
                                "       <div><span style='font-weight: bold; color: green;'>Date:</span> <input type='text' size='10' id='SearchHistoryDate' />" +
                                "            <span style='font-weight: bold; color: green;'>Time:</span> <select id='SearchHistoryTime'>" + createTimeOptions() + "</select></div>" +
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>Radius:</span> <input type='text' id='SearchHistoryRadius' value='" + radius.toFixed(0) + "' /> m </div>" +
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>Time Range:</span> <span style='font-weight: bold; color: green;'>+/-</span> <select id='SearchHistoryMinutes'><option value='10'>10</option><option value='30'>30</option><option selected value='60'>60</option></select> Minutes &nbsp; " +
                                //"       <div style='margin-top: 5px;'><a href='javascript:void(0)' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");'><img src='../images/searchicon.png' /> Search</a></div>" +
                                "       <div style='margin-top: 5px;'><input type='button' class='kd-button' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");' value='Search' /></div>" + 
                                "       <div id='searchAddressMessage' style='color:red;display:none;'>* Radius cannot exceed 2000m</div>" +
                                "       <div style='margin-top: 30px;'><a href='javascript:void(0)' onclick='geozonelandmarkformmainoptions();'>Back</a></div>" +
                                "   </div>" +
                                "</td></tr></table>";
                }

                content +=      "<div style='font-size: 1em;margin-top:10px;'><input type='button' class='kd-button' onclick='savePolygon_NewTZ(this, popup, selectedFeature);' value='Save' /> <input type='button' class='kd-button' value='Cancel' onclick='cancelPolygon(popup, selectedFeature);' /></div>" +
                            "</form></div>";
            }
            else {
                content = "<h6 style=\"margin:0 0 10px 10px;border-bottom:1px solid #cccccc\">New Geozone/Landmark</h6>";
                content += "<div style='font-size:1em'><form id='polygonform'>" +
                            "<input type='hidden' id='isNew' name='isNew' value='1' />" +
                            "<input type='hidden' id='pointSets' name='pointSets' value='" + pointSets + "' />" +
                            "<input type='hidden' id='txtX' name='txtX' value='" + txtX + "' />" +
                            "<input type='hidden' id='txtY' name='txtY' value='" + txtY + "' />" +
                            "<input type='hidden' id='txtRadius' name='txtRadius' value='-1' />" +
                            "<input id='lstAddOptions_0' type='radio' onclick='$(\".geozonefield\").hide();$(\".landmarkfield\").show();$(\".geozonelandmarkmoreoption\").hide();' value='0' name='lstAddOptions' checked /><label for='lstAddOptions_0' style='margin-left:3px;'>Landmark</label>" +
                            "<input id='lstAddOptions_0' type='radio' onclick='$(\".landmarkfield\").hide();$(\".geozonefield\").show();$(\".geozonelandmarkmoreoption\").hide();' value='1' name='lstAddOptions' style='margin-left:20px;' /><label for='lstAddOptions_0' style='margin-left:3px;'>Geozone</label>" +
                                '<table class="landmarkfield landmarkmainoptions" cellspacing="0" cellpadding="0" border="0" style="margin-left:10px;">' +
                                '<tr><td style="padding-top:10px;"><table border="0">' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px; width: 139px;">' +
                                '            <span id="lblLandmarkNameTitle" class="formtext">Landmark Name (*):</span>' +
                                '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkName" type="text" id="txtLandmarkName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblContactNameTitle" class="formtext">Contact Name:</span></td>' +
                                '        <td class="formtext" style="height: 22px">' +
                                '            <input name="txtContactName" type="text" id="txtContactName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblLandmarkDescriptionTitle" class="formtext">Landmark Description:</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtLandmarkDesc" type="text" id="txtLandmarkDesc" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblPhoneTitle" class="formtext">Phone :</span></td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <input name="txtPhone" type="text" id="txtPhone" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>';
                if (ShowCallTimer) {
                    content += '    <tr>' +
                                '        <td class="formtext" style="height: 30px;">' +
                                '            <span id="lblCallTimerTitle" class="formtext">Timeout: </span></td>' +
                                '        <td class="formtext" height="30">' + CallTimerSelections + '</td>' +
                                '    </tr>';
                }
                content += '   <tr>' +
                                '       <td colspan="2" class="formtext" height="15" style="height: 30px;">' +
                                '           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" /><span style="margin-left:3px;">Private</span>' +
                                '           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" checked /><span style="margin-left:3px;">Public</span>' +
                                '       </td>' +
                                '   </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '           <a href=\'javascript:void(0)\' onclick=\'landmarkformmoreoptions();\'>More Options</a>';
                if (ShowMapHistorySearch) {
                    content += '           <a href=\'javascript:void(0)\' onclick=\'landmarkhistorysearch();\'>Search History</a>';
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
                                '            <span id="lblGeozoneNameTitle" class="formtext">GeoZone Name (*):</span>' +
                                '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                                '        <td class="formtext" style="height: 22px">' +
                                '            <input name="txtGeoZoneName" type="text" id="txtGeoZoneName" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblDirectionTitle" class="formtext">Direction:</span></td>' +
                                '        <td class="formtext" style="height: 22px">' +
                                '            <select name="cboDirection" id="cboDirection" class="formtext" style="width:175px;">' +
			                    '                <option selected="selected" value="0">Disable</option>' +
			                    '                <option value="1">Out</option>' +
			                    '                <option value="2">In</option>' +
			                    '                <option value="3">InOut</option>' +
		                        '            </select></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblGeozoneDescriptionTitle" class="formtext">GeoZone Description:</span></td>' +
                                '        <td class="formtext" style="height: 20px">' +
                                '            <input name="txtGeoZoneDesc" type="text" id="txtGeoZoneDesc" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblDefaultSeverityTitle" class="formtext">Default Severity:</span>' +
                                '        </td>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <select name="cboGeoZoneSeverity" onchange="" id="cboGeoZoneSeverity" class="formtext" DESIGNTIMEDRAGDROP="451" style="width:175px;">' +
			                    '                <option selected="selected" value="0">NoAlarm</option>' +
			                    '                <option value="1">Notify</option>' +
			                    '                <option value="2">Warning</option>' +
			                    '                <option value="3">Critical</option>' +
		                        '            </select></td>' +
                                '    </tr>' +
                                '   <tr>' +
                                '       <td colspan="2" class="formtext" style="height: 30px;">' +
                                '           <input id="lstPublicPrivate" type="radio" value="0" name="lstGeozonePublicPrivate" /><span style="margin-left:3px;">Private</span>' +
                                '           <input id="lstPublicPrivate" type="radio" value="1" name="lstGeozonePublicPrivate" style="margin-left:20px;" checked /><span style="margin-left:3px;">Public</span>' +
                                '       </td>' +
                                '   </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="2" class="formtext" height="15">' +
                                '           <a href=\'javascript:void(0)\' onclick=\'geozoneformmoreoptions();\'>More Options</a>' +
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
                                '            <span id="lblEmailTitle" class="formtext">Email:</span>' +
                                '            </td>' +
                                '        <td style="height: 30px">' +
                                '            <input name="txtEmail" type="text" id="txtEmail" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '   </tr>' +
                                '   <tr style="display:none;">' +
                                '        <td class="style4" style="height: 30px">' +
                                '            <span id="lblPhone" class="formtext"> Phone:</span>' +
                                '                                        <span id="RegularExpressionValidator1" class="formtext" style="color:Red;visibility:hidden;">*</span>' +
                                '                        </td>' +
                                '        <td style="height: 30px">' +
                                '            <input name="txtPhoneSMS" type="text" id="txtPhoneSMS" disabled="disabled" class="formtext bsmforminput" style="width:173px;" /></td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td class="formtext" style="height: 30px">' +
                                '            <span id="lblTimeZoneTitle" class="formtext">Time Zone:</span>' +
                                '                        </td>' +
                                '        <td style="height: 30px">' +
                                '            <select name="cboTimeZone" id="cboTimeZone" class="RegularText" style="width:168px;">' +
			                    '                <option value="-12">GMT-12</option>' +
			                    '                <option value="-11">GMT-11</option>' +
			                    '                <option value="-10">GMT-10</option>' +
			                    '                <option value="-9">GMT-9</option>' +
			                    '                <option value="-8">GMT-8</option>' +
			                    '                <option value="-7">GMT-7</option>' +
			                    '                <option value="-6">GMT-6</option>' +
			                    '                <option value="-5">GMT-5</option>' +
			                    '                <option value="-4">GMT-4</option>' +
			                    '                <option value="-3">GMT-3</option>' +
			                    '                <option value="-2">GMT-2</option>' +
			                    '                <option value="-1">GMT-1</option>' +
			                    '                <option value="0">GMT</option>' +
			                    '                <option value="1">GMT+1</option>' +
			                    '                <option value="2">GMT+2</option>' +
			                    '                <option value="3">GMT+3</option>' +
			                    '                <option value="4">GMT+4</option>' +
			                    '                <option value="5">GMT+5</option>' +
			                    '                <option value="6">GMT+6</option>' +
			                    '                <option value="7">GMT+7</option>' +
			                    '                <option value="8">GMT+8</option>' +
			                    '                <option value="9">GMT+9</option>' +
			                    '                <option value="10">GMT+10</option>' +
			                    '                <option value="11">GMT+11</option>' +
			                    '                <option value="12">GMT+12</option>' +
			                    '                <option value="13">GMT+13</option>' +
		                        '            </select></td>' +
                                '        <td class="style4">' +
                                '            &nbsp;</td>' +
                                '        <td>' +
                                '            &nbsp;</td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td colspan="4">' +
                                '                                    <span id="lblMultipleEmails" class="formtext">*Multiple email addresses Must be Separated by semicolon or comma</span>' +
                                '        </td>' +
                                '    </tr>' +
                                '    <tr>' +
                                '        <td style="height: 9px" colspan="4">' +
                                '            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" /><label for="chkDayLight">Automatically adjust for daylight savings time</label></span>' +
                                '            <table class="geozonefield" cellspacing="0" cellpadding="0" border="0" style="display:none;">' +
                                '                <tr>' +
                                '                    <td>' +
                                '                        <input id="chkCritical" type="checkbox" name="chkCritical" /><label for="chkCritical">Critical Alarm</label></td>' +
                                '                </tr>' +
                                '                <tr>' +
                                '                    <td>' +
                                '                        <input id="chkWarning" type="checkbox" name="chkWarning" /><label for="chkWarning">Warning Alarm</label></td>' +
                                '                </tr>' +
                                '                <tr>' +
                                '                    <td>' +
                                '                        <input id="chkNotify" type="checkbox" name="chkNotify" /><label for="chkNotify">Notify Alarm</label></td>' +
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
                                '           <a href=\'javascript:void(0)\' onclick=\'geozonelandmarkformmainoptions();\'>Back</a>' +
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
                                "       <div id='SearchHistoryTitle' style='font-weight: bold; color: green;margin:10px 0;'>Search History:</div>" +
                                "       <div><span style='font-weight: bold; color: green;'>Date:</span> <input type='text' size='10' id='SearchHistoryDate' />" +
                                "            <span style='font-weight: bold; color: green;'>Time:</span> <select id='SearchHistoryTime'>" + createTimeOptions() + "</select></div>" +
                                "       <div style='margin-top: 5px;display:none;'><span style='font-weight: bold; color: green;'>Radius:</span> <input type='text' id='SearchHistoryRadius' value='" + polygonRadius.toFixed(0) + "' /> m </div>" +
                                "<input type='hidden' id='mapSearchPointSets' name='mapSearchPointSets' value='" + mapSearchPointSets + "' />" +
                                "       <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>Time Range:</span> <span style='font-weight: bold; color: green;'>+/-</span> <select id='SearchHistoryMinutes'><option value='10'>10</option><option value='30'>30</option><option selected value='60'>60</option></select> Minutes &nbsp; " +
                                //"       <a href='javascript:void(0)' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");'><img src='../images/searchicon.png' /></a>" +
                                "       <div style='margin-top: 5px;'><input type='button' class='kd-button' onclick='searchHistoryAddress(" + txtX.toFixed(5) + "," + txtY.toFixed(5) + ");' value='Search' /></div>" +
                                "       <div id='searchAddressMessage' style='color:red;display:none;'>* Radius cannot exceed 2000m</div>" +
                                "       <div style='margin-top: 30px;'><a href='javascript:void(0)' onclick='geozonelandmarkformmainoptions();'>Back</a></div>" +
                                "   </div>" +
                                "</td></tr></table>";
                }

                 content +=     "<div style='font-size: 1em;margin-top:10px;'><input type='button' class='kd-button' onclick='savePolygon_NewTZ(this, popup, selectedFeature);' value='Save' /> <input type='button' class='kd-button' value='Cancel' onclick='cancelPolygon(popup, selectedFeature);' /></div>" +
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
            $('#messagebar').html(event.feature.attributes.count + " Geozone/Landmark(s). Zoom in for details.").show().delay(3000).fadeOut(1000);
            return false;
        }
        if (event.feature.fid.split(":::")[0] == "LandmarkCircle" && document.getElementById("modifyToggle").checked == true) {
            $('#messagebar').html("Please use Edit button to modify Landmark Circle.").show().delay(2000).fadeOut(1000);
            return false;
        }
        if (event.feature.fid.split(":::")[0] == "Geozone" && document.getElementById("modifyToggle").checked == true && event.feature.Assigned == 1) {
            $('#messagebar').html("This Geozone is assigned to a box and it's not editable.").show().delay(2000).fadeOut(1000);
            return false;
        }
    }

    function beforeModified(event) {
        if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {

            if (event.feature.fid.split(":::")[0] == "LandmarkCircle") {
                return;
            }

            var c = event.feature.geometry.components[0];
            previousPointSets = "";
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
                pushToUndolist(otype, oid, pointSets);
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
                vectors.addFeatures(geoLandmarkFeatures);
            }
            else if (zLevel < 14 && clusterDisabled) {
                clusterDisabled = false;
                strategy.activate();
                vectors.removeAllFeatures();
                vectors.addFeatures(geoLandmarkFeatures);
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

    map.removePopup(popup);
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


                if (r.isNew == 1) geoLandmarkFeatures.push(feature);
                try { selectControl.unselect(selectedFeature); }
                catch (e) { }
            }
            else {
                //alert("Failed to create the Landmark/Geozone");
                if (r.message == "")
                    $('#messagebar').html("Failed to create the Landmark/Geozone").show().delay(2000).fadeOut(1000);
                else
                    $('#messagebar').html(r.message).show().delay(2000).fadeOut(1000);

                vectors.removeFeatures(feature);
            }

            if (r.objectType == "Geozone" && parent.geozonegridloaded) {
                parent.loadGeozones();
            }
            else if ((r.objectType == "Landmark" || r.objectType == "LandmarkCircle") && parent.landmarkgridloaded) {
                parent.loadLandmarks();
            }
        },
        error: function (msg) {
            alert('failure');
            vectors.removeFeatures(feature);
        }
    });

    currentEditFeature = null;
    currentEditPopup = null;

}

// Changes for TimeZone Feature end

function savePolygon(b, popup, feature) {
    var f = $(b).closest("form").serialize();
    
    map.removePopup(popup);
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


                if (r.isNew == 1) geoLandmarkFeatures.push(feature);
                try { selectControl.unselect(selectedFeature); }
                catch (e) { }
            }
            else {
                //alert("Failed to create the Landmark/Geozone");
                if (r.message == "")
                    $('#messagebar').html("Failed to create the Landmark/Geozone").show().delay(2000).fadeOut(1000);
                else
                    $('#messagebar').html(r.message).show().delay(2000).fadeOut(1000);

                vectors.removeFeatures(feature);
            }

            if (r.objectType == "Geozone" && parent.geozonegridloaded) {
                parent.loadGeozones();
            }
            else if ((r.objectType == "Landmark" || r.objectType == "LandmarkCircle") && parent.landmarkgridloaded) {
                parent.loadLandmarks();
            }
        },
        error: function (msg) {
            alert('failure');
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

                if (otype == "Geozone" && parent.geozonegridloaded) {
                    parent.loadGeozones();
                }
                else if ((otype == "Landmark" || otype == "LandmarkCircle") && parent.landmarkgridloaded) {
                    parent.loadLandmarks();
                }
            }
            else {
                alert("Failed to delete the Landmark/Geozone");
            }
        },
        error: function (msg) {
            alert('failure');
        }
    });
}

function deleteThisPolygon(b, popup, feature) {
    var f = $(b).closest("form");
    var otype = f.find('#geoassettype').val();
    var oid = f.find('#geoassetname').val();
    deletePolygon(otype, oid, popup, feature);
}

function pushToUndolist(otype, oid, pointSets) {
    var item = { otype: otype, oid: oid, pointSets: previousPointSets };
    undolist.push(item);
    if (undolist.length > 0) {
        $('#undonum').html(undolist.length);
        $('#undobar').show();
    }

    var itemForSave = { otype: otype, oid: oid, pointSets: pointSets };

    var addedtochangeslist = false;
    for (var i = 0; i < changeslist.length; i++) {
        if (changeslist[i].otype == otype && changeslist[i].oid == oid) {
            changeslist[i].pointSets = pointSets;
            addedtochangeslist = true;
        }
    }
    if (!addedtochangeslist)
        changeslist.push(itemForSave);
}

// Changes for TimeZone Feature start
function editPolygon_NewTZ(otype, oid, pointSets) {
    //Edit polygon
    $('#undolink').hide();
    $('#undosaving').show();

    $.ajax({
        type: 'POST',
        url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/EditGeozoneLandmark_NewTZ',
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
                alert("Failed to edit the landmark");
            }
        },
        error: function (msg) {
            //$('#undosaving').hide();
            //$('#undolink').show();
            $('#savechanges').show();
            alert('failure');
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
                alert("Failed to edit the landmark");
            }
        },
        error: function (msg) {
            //$('#undosaving').hide();
            //$('#undolink').show();
            $('#savechanges').show();
            alert('failure');
        }
    });

}
// Changes for TimeZone Feature start
//By Devin Begin
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
                                    }

                                }
                                catch (err) { }
                            }
                            catch (err) { }
                        }
                        geozoneLoaded = true;
                        if (geoLandmarkFeatures.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(geoLandmarkFeatures);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (response) {
                alert("Failed to load geozones. " + response.toString());
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
                                    }

                                }
                                catch (err) { }
                            }
                            catch (err) { }
                        }
                        geozoneLoaded = true;
                        if (geoLandmarkFeatures.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(geoLandmarkFeatures);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (response) {
                alert("Failed to load geozones. " + response.toString());
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
                                var feature;
                                if (radius > 0) {
                                    var point = new OpenLayers.LonLat(landmarkCollection[landmark_i].lon, landmarkCollection[landmark_i].lat);
                                    point.transform(proj, map.getProjectionObject());
                                    var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
                                    var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, radius, 40, 0);
                                    feature = new OpenLayers.Feature.Vector(
                                                circle
                                             );
                                    feature.fid = "LandmarkCircle:::" + landmarkCollection[landmark_i].LandmarkName;

                                    feature.attributes = { Public: landmarkCollection[landmark_i].Public };
                                    //geoLandmarkFeatures.push(feature);


                                }
                                else {
                                    try {

                                        landmarkstring = landmarkCollection[landmark_i].coords;

                                        if (landmarkstring.length > 60) {
                                            feature = new OpenLayers.Format.WKT(in_options).read(landmarkstring);

                                            feature.fid = "Landmark:::" + landmarkCollection[landmark_i].LandmarkName;
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
                                    geoLandmarkFeatures.push(feature);
                                }

                            }
                            catch (err) { }
                        }
                        landmarkLoaded = true;
                        if (geoLandmarkFeatures.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(geoLandmarkFeatures);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (msg) {
                alert('Failed to load landmarks.');
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
                                var feature;
                                if (radius > 0) {
                                    var point = new OpenLayers.LonLat(landmarkCollection[landmark_i].lon, landmarkCollection[landmark_i].lat);
                                    point.transform(proj, map.getProjectionObject());
                                    var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
                                    var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, radius, 40, 0);
                                    feature = new OpenLayers.Feature.Vector(
                                                circle
                                             );
                                    feature.fid = "LandmarkCircle:::" + landmarkCollection[landmark_i].LandmarkName;

                                    feature.attributes = { Public: landmarkCollection[landmark_i].Public };
                                    //geoLandmarkFeatures.push(feature);


                                }
                                else {
                                    try {

                                        landmarkstring = landmarkCollection[landmark_i].coords;
                                        
                                        if (landmarkstring.length > 60) {
                                            feature = new OpenLayers.Format.WKT(in_options).read(landmarkstring);

                                            feature.fid = "Landmark:::" + landmarkCollection[landmark_i].LandmarkName;
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
                                    geoLandmarkFeatures.push(feature);
                                }

                            }
                            catch (err) { }
                        }
                        landmarkLoaded = true;
                        if (geoLandmarkFeatures.length > 0) {
                            vectors.removeAllFeatures();
                            vectors.addFeatures(geoLandmarkFeatures);
                        }
                    }
                }
                catch (err) { }

            },
            error: function (msg) {
                alert('Failed to load landmarks.');
            }
        });

    }
    catch (err) {
        var s = err;
    }
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
    var msie = ua.indexOf("MSIE ")
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
//        else if (parent.overlaysettings.vehicleDrivers) {
//            vehicleDriversLayer.destroyFeatures();
//            vehicleDriversLayer.addFeatures(vehicleDrivers);
//            vehicleDriversLayer.redraw();
//        }
//        else if (parent.overlaysettings.vehiclenames) {
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
        if (currentPopups[i].id != "polygonformpopup")
            map.removePopup(currentPopups[i]);
    }
}

function removeAllPopups() {
    var currentPopups = map.popups;
    for (i = 0; i < currentPopups.length; i++) {
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
                else if (currentVehicle.ImagePath != '') {
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
                            if ((currentVehicle.convertedDisplayDate >= '01/01/2000 00:00 am') && currentVehicle.Longitude != 0 && currentVehicle.Latitude != 0) {
                                bounds.extend(newLoc);
                            }
                        }
                        catch (ex) {
                        }
                    }
                }

                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                var marker = new OpenLayers.Feature.Vector(point);
                /*var popupContent = "<h6>" + currentVehicle.Description + "<br />";
                if ($.trim(currentVehicle.Driver) != '')
                    popupContent = popupContent + 'Driver: ' + currentVehicle.Driver + '<br />';
                popupContent = popupContent +
                    currentVehicle.StreetAddress +
                    "<br />Time: " + currentVehicle.convertedDisplayDate +
                    "<br />Heading: " + currentVehicle.MyHeading +
                    "<br />Speed: " + currentVehicle.CustomSpeed +
                    "<br />Status: " + currentVehicle.VehicleStatus +
                    "<br /><div style='height:25px;margin-top:5px;' id='popup" + currentVehicle.LicensePlate + "'></div>";*/
                var popupContent = getPopupContent(currentVehicle);

                marker.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc, BoxId: currentVehicle.BoxId, Driver: currentVehicle.Driver, Description: currentVehicle.Description };

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
//            else if (parent.overlaysettings.vehicleDrivers) {
//                vehicleDriversLayer.destroyFeatures();
//                vehicleDriversLayer.addFeatures(vehicleDrivers);
//                vehicleDriversLayer.redraw();
//            }
//            else if (parent.overlaysettings.vehiclenames) {
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
            for (i = 0; i < historyRecords.length; i++) {
                //if (i > 9 || i<8) continue;
                currentHistory = historyRecords[i];
                var vicon = new OpenLayers.Icon(rootpath+"New20x20/" + currentHistory.icon);
                var viconUrl = rootpath+"New20x20/" + currentHistory.icon;
                
                if (currentHistory.BoxId == '27492') {
                    var size = new OpenLayers.Size(48, 97);
                    vicon = new OpenLayers.Icon(rootpath+"New20x20/" + "tower.png", size);
                    viconUrl = rootpath+"New20x20/" + "tower.png";
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
                
                points.push(point);
                var historyMarker = new OpenLayers.Feature.Vector(point);
                
                //var popupContent = getHistoryPopupContent(currentHistory);

                //historyMarker.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc, BoxId: currentHistory.BoxId, Description: currentHistory.Description };
                historyMarker.attributes = { vicon: vicon, icon: viconUrl, popupContent: '', location: newLoc, BoxId: currentHistory.BoxId, Description: currentHistory.Description, historyRecordsIndex: i };

                //currentHistories[currentHistoriesIndex] = historyMarker;
                //currentHistoriesIndex++;

                historyFeatures.push(historyMarker);                
            }

            map.addLayer(histories);

            
            /*
             * Draw path
             */
            map.addControl(new OpenLayers.Control.DrawFeature(histories, OpenLayers.Handler.Path));
            
            var line = new OpenLayers.Geometry.LineString(points);
            
            var style = {
                strokeColor: '#0000ff',
                strokeOpacity: 0.4,
                strokeWidth: 8
            };

            var lineFeature = new OpenLayers.Feature.Vector(line, null, style);
            lineFeature.noclickevent = true;
            histories.addFeatures([lineFeature]);
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
                                else if (currentVehicle.ImagePath != '') {
                                    vicon = new OpenLayers.Icon("../images/CustomIcon/" + currentVehicle.icon);
                                    var viconUrl = "../images/CustomIcon/" + currentVehicle.icon;
                                }

                                //markers.removeFeatures([currentMarkers[j]]);

                                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                                var marker = new OpenLayers.Feature.Vector(point);

                                var popupContent = getPopupContent(currentVehicle);

                                marker.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc, BoxId: currentVehicle.BoxId, Driver: currentVehicle.Driver, Description: currentVehicle.Description };
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
                                else if (currentVehicle.ImagePath != '') {
                                    vicon = new OpenLayers.Icon("../images/CustomIcon/" + currentVehicle.icon);
                                    var viconUrl = "../images/CustomIcon/" + currentVehicle.icon;
                                }

                                markers.removeFeatures([currentMarkers[j]]);

                                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                                var marker = new OpenLayers.Feature.Vector(point);

                                var popupContent = getPopupContent(currentVehicle);

                                marker.attributes = { vicon: vicon, icon: viconUrl, popupContent: popupContent, location: newLoc, BoxId: currentVehicle.BoxId, Driver: currentVehicle.Driver, Description: currentVehicle.Description };
                                markers.addFeatures([marker]);

                                currentMarkers[j] = marker;
                                allVehicles[j] = currentVehicle;

//                                if (parent.VehicleClustering) {
//                                }
//                                else if (parent.overlaysettings.vehicleDrivers) {
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
//                                else if (parent.overlaysettings.vehiclenames) {
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
    var content;
    var position;
    if (feature.cluster) {
        currentVehiclePopupFeature = feature;
        position = new OpenLayers.LonLat(feature.geometry.x, feature.geometry.y);
        content = "<h6 style='margin-bottom:10px;'>" + feature.attributes.count + " Vehicles.</h6>";
        content = content + "<div id='vehiclelist'>";
        for (i = 0; i < VehiclePopupPageSize && i < feature.attributes.count; i++) {
            content = content + "<div class='singleVehiclePopupContent'>" + feature.cluster[i].attributes.popupContent + "</div>";
        }
        currentVehiclePopupIndex = i;
        content = content + "</div>";

        if (feature.attributes.count > VehiclePopupPageSize) {
            content = content + "<div id='vehicleViewMoreLink'><a href='javascript:void(0)' onclick='loadmoreclustervehicles()'>View More</a></div>";
        }
    }
    else {
        if (feature.attributes.popupContent != '')
            content = feature.attributes.popupContent;
        else if (feature.attributes.historyRecordsIndex != undefined) {
            content = getHistoryPopupContent(AllHistoryRecords[feature.attributes.historyRecordsIndex]);
            feature.attributes.popupContent = content;
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
}

function onSearchMarkerClick(feature) {
    //alert(feature.geometry.x);
    var position = new OpenLayers.LonLat(feature.geometry.x, feature.geometry.y);
    var content = "<div style='font-weight:bold;'>" + feature.attributes.address + "</div>";
    //content += "<div style='margin-top: 20px;'><a href='javascript:void(0)' onclick='parent.getClosestVehicles(" + feature.attributes.lon + "," + feature.attributes.lat + ");map.removePopup(searchMarkerPopup);'>Closest Vehicles</a> &nbsp; <a href='javascript:void(0)'>History</a></div>";
    content += "<div style='margin-top: 20px;'><a href='javascript:void(0)' onclick='toggleSearchAddressCriterias(this, \"ClosestVehiclesCriterias\", 300, 50);'>Closest Vehicles</a> &nbsp; ";
    if (ShowMapHistorySearch) {
        content += "<a href='javascript:void(0)' onclick='toggleSearchAddressCriterias(this, \"SearchHistoryCriterias\", 300, 70);'>History</a>";
    }
    content += "</div>";
    content += "<div style='margin-top: 5px; display:none;border-top: 1px solid #cccccc;padding-top: 5px;' id='SearchAddressForms'>";
    content += "<div style='display:none' id='ClosestVehiclesCriterias' class='SearchAddressFields'>";
    content += " <span style='font-weight: bold; color: green;'>Radius:</span> <select id='ClosestVehiclesRadius' /><option value='500'>500</option><option value='1000'>1000</option><option value='2000'>2000</option></select> m ";
    content += " <span style='font-weight: bold; color: green;'>Vehicles:</span> <select id='ClosestVehiclesNumOfVehicles' /><option value='10'>10</option><option value='20'>20</option><option value='50'>50</option><option value='100' selected>100</option></select>";
    content += " <a href='javascript:void(0)' onclick='getClosestVehicles(" + feature.attributes.lon + "," + feature.attributes.lat + ");'><img src='../images/searchicon.png' /></a>";
    content += "</div>";
    content += "<div style='display:none' id='SearchHistoryCriterias' class='SearchAddressFields'>";    
    content += " <div><span style='font-weight: bold; color: green;'>Date:</span> <input type='text' size='10' id='SearchHistoryDate' />";
    content += " <span style='font-weight: bold; color: green;'>Time:</span> <select id='SearchHistoryTime'>" + createTimeOptions() + "</select></div>";
    content += " <div style='margin-top: 5px;'><span style='font-weight: bold; color: green;'>Radius:</span> <select id='SearchHistoryRadius' /><option value='500'>500</option><option value='1000'>1000</option><option selected value='2000'>2000</option></select> m ";
    content += " <span style='font-weight: bold; color: green;'>+/-</span> <select id='SearchHistoryMinutes'><option value='10'>10</option><option value='30'>30</option><option selected value='60'>60</option></select> Minutes &nbsp; ";
    //content += " <span style='font-weight: bold; color: green;'>Vehicles:</span> <select id='ClosestVehiclesNumOfVehicles' /><option value='10'>10</option><option value='20' selected>20</option><option value='50'>50</option><option value='100'>100</option></select>";
    content += " <a href='javascript:void(0)' onclick='searchHistoryAddress(" + feature.attributes.lon.toFixed(5) + "," + feature.attributes.lat.toFixed(5) + ");'><img src='../images/searchicon.png' /></a>";
    content += " </div>";
    content += "</div>";
    content += "</div>";
    
    searchMarkerPopup = new OpenLayers.Popup.FramedCloud("addressMarkerPopup", position,
                                new OpenLayers.Size(100, 50),
                                content,
                                null, true, onVehiclePopupClose);

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
            dateFormat: 'mm/dd/yy',
            closeText: 'Close'
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
    var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, ClosestVehiclesRadius, 40, 0);
    feature = new OpenLayers.Feature.Vector(  circle  );
    searchArea.removeAllFeatures();
    searchArea.addFeatures([feature]);

    parent.getClosestVehicles(lon, lat, ClosestVehiclesRadius, ClosestVehiclesNumOfVehicles); 
    map.removePopup(searchMarkerPopup);
}

function searchHistoryAddress(lon, lat) {
    SearchHistoryRadius = $('#SearchHistoryRadius').val();
    SearchHistoryMinutes = $('#SearchHistoryMinutes').val();
    //ClosestVehiclesNumOfVehicles = $('#ClosestVehiclesNumOfVehicles').val();
    SearchHistoryTime = $('#SearchHistoryTime').val();
    SearchHistoryDate = $('#SearchHistoryDate').val();
    mapSearchPointSets = $('#mapSearchPointSets').val();

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
        var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, SearchHistoryRadius, 40, 0);
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

    parent.searchHistoryAddress(lon, lat, SearchHistoryDate + ' ' + SearchHistoryTime, SearchHistoryRadius, SearchHistoryMinutes, mapSearchPointSets);
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
    if (currentVehiclePopupFeature && currentVehiclePopupFeature.cluster) {
        var content = "";
        var startIndex = currentVehiclePopupIndex;
        for (; currentVehiclePopupIndex < startIndex + VehiclePopupPageSize && currentVehiclePopupIndex < currentVehiclePopupFeature.attributes.count; currentVehiclePopupIndex++) {
            content = content + "<div class='singleVehiclePopupContent'>" + currentVehiclePopupFeature.cluster[currentVehiclePopupIndex].attributes.popupContent + "</div>";
        }
        $('#vehiclelist').append(content);
        if (currentVehiclePopupIndex >= currentVehiclePopupFeature.attributes.count)
            $('#vehicleViewMoreLink').hide();
        else
            $('#vehicleViewMoreLink').show();
    }
}

function onFeatureSelect(feature) {
    selectedFeature = feature;
    var popupheight;
    var content = "<div style='font-size:.8em'>New Landmark or Geozone</div>";
    if (feature.fid.split(":::")[0] == "Landmark" || feature.fid.split(":::")[0] == "LandmarkCircle") {
        //popupheight = 280;
        polygonPopupHeight = 280;
        if (feature.fid.split(":::")[0] == "LandmarkCircle") polygonPopupHeight = 310;
        var oid = feature.fid.split(":::")[1];
        
        $.ajax({
            type: 'POST',
            url: rootpath + 'MapNew/landmarkEditForm.aspx',
            data: { landmarkName: oid, showheader: false, geotype:'landmark' },
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
                content = "failed to fetch the data.";
            }
        });
    }
    else if (feature.fid.split(":::")[0] == "Geozone") {
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
                content = "failed to fetch the data.";
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
        vectors.removeFeatures(geoLandmarkFeatures.features);
        vectors.addFeatures(geoLandmarkFeatures);

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
        vectors.addFeatures(geoLandmarkFeatures);

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
        if (!addedtochangeslist)
            changeslist.push(undolist[i]);
    }
}

function savechanges() {
    $('#savechanges').hide();
    $('#undolink').hide();
    $('#undosaving').show();
    $('#noneToggle').click();

    for (var i = 0; i < changeslist.length; i++) {
        editPolygon_NewTZ(changeslist[i].otype, changeslist[i].oid, changeslist[i].pointSets);
    }
    changeslist = [];
    undolist = [];
    $('#undobar').hide();
    $('#messagebar').html("The change(s) has been saved.").show().delay(2000).fadeOut(1000);
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
        formtitle = "Edit Landmark";
        infotitle = "Landmark: " + data.LandmarkName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;");
    }
    else {
        formtitle = "Edit Geozone";
        infotitle = "Geozone: " + data.GeozoneName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;");
    }
    var infocontent = "<h6 style=\"margin-left:10px;\">" + infotitle + "</h6>";
    var disabled = assigned == 1 ? ' disabled ' : '';
     
    infocontent += '<div style="margin:10px 0 0 20px;"><a href="javascript:void(0)" onclick="showeditpolygonform();">Edit</a></div>'

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
                        '            <span id="lblLandmarkNameTitle" class="formtext">Landmark Name (*):</span>' +
                        '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <input name="txtLandmarkName" type="text" id="txtLandmarkName" class="formtext bsmforminput" style="width:173px;" value="' + data.LandmarkName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <span id="lblContactNameTitle" class="formtext">Contact Name:</span></td>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <input name="txtContactName" type="text" id="txtContactName" class="formtext bsmforminput" style="width:173px;" value="' + data.ContactPersonName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +                        
                        '    <tr>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <span id="lblLandmarkDescriptionTitle" class="formtext">Landmark Description:</span></td>' +
                        '        <td class="formtext">' +
                        '            <input name="txtLandmarkDesc" type="text" id="txtLandmarkDesc" class="formtext bsmforminput" style="width:173px;" value="' + data.Description.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <span id="lblPhoneTitle" class="formtext">Phone :</span></td>' +
                        '        <td class="formtext">' +
                        '            <input name="txtPhone" type="text" id="txtPhone" class="formtext bsmforminput" style="width:173px;" value="' + data.ContactPhoneNum.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>';
        if (data.Radius > 0) {
            content = content +
                        '    <tr>' +
                        '        <td class="formtext" height="25" style="height: 15px;">' +
                        '            <span id="lblRadiusTitle" class="formtext">Radius</span>' +
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
                        '           <input id="lstPublicPrivate" type="radio" value="0" name="lstPublicPrivate" ' + (isPublic ? '' : 'checked') + ' /><span style="margin-left:3px;">Private</span>' +
                        '           <input id="lstPublicPrivate" type="radio" value="1" name="lstPublicPrivate" style="margin-left:20px;" ' + (isPublic ? 'checked' : '') + ' /><span style="margin-left:3px;">Public</span>' +
                        '       </td>' +                                
                        '   </tr>' ;
        content = content +
                        '    <tr>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '        <td class="formtext" height="15">' +
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td colspan="2" class="formtext" height="15">' +
                        '           <a href=\'javascript:void(0)\' onclick=\'editformmoreoptions();\'>More Options</a> &nbsp; ' +
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
                        '            <span id="lblGeozoneNameTitle" class="formtext">GeoZone Name (*):</span>' +
                        '            <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <input name="txtGeoZoneName" type="text" id="txtGeoZoneName" class="formtext bsmforminput" style="width:173px;" value="' + data.GeozoneName.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblDirectionTitle" class="formtext">Direction:</span></td>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <select name="cboDirection" id="cboDirection" class="formtext" style="width:175px;" ' + disabled + '>' +
			            '                <option ' + (data.Direction == 0 ? ' selected="selected"' : '') + ' value="0">Disable</option>' +
			            '                <option ' + (data.Direction == 1 ? ' selected="selected"' : '') + ' value="1">Out</option>' +
			            '                <option ' + (data.Direction == 2 ? ' selected="selected"' : '') + ' value="2">In</option>' +
			            '                <option ' + (data.Direction == 3 ? ' selected="selected"' : '') + ' value="3">InOut</option>' +
		                '            </select></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblGeozoneDescriptionTitle" class="formtext">GeoZone Description:</span></td>' +
                        '        <td class="formtext" style="height: 25px">' +
                        '            <input name="txtGeoZoneDesc" type="text" id="txtGeoZoneDesc" class="formtext bsmforminput" value="' + data.Description.replace(/\'/g, '&#39;').replace(/\"/g, "&quot;") + '" style="width:173px;" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblDefaultSeverityTitle" class="formtext">Default Severity:</span>' +
                        '        </td>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <select name="cboGeoZoneSeverity" onchange="" id="cboGeoZoneSeverity" class="formtext" DESIGNTIMEDRAGDROP="451" style="width:175px;" >' +
			            '                <option ' + (data.SeverityId == 0 ? ' selected="selected"' : '') + ' value="0">NoAlarm</option>' +
			            '                <option ' + (data.SeverityId == 1 ? ' selected="selected"' : '') + ' value="1">Notify</option>' +
			            '                <option ' + (data.SeverityId == 2 ? ' selected="selected"' : '') + ' value="2">Warning</option>' +
			            '                <option ' + (data.SeverityId == 3 ? ' selected="selected"' : '') + ' value="3">Critical</option>' +
		                '            </select></td>' +
                        '    </tr>';
        content = content +
                        '   <tr>' +
                        '       <td colspan="2" class="formtext" height="25" style="height: 25px;">' +
                        '           <input id="lstPublicPrivate" type="radio" value="0" name="lstGeozonePublicPrivate" ' + (isPublic ? '' : ' checked ') + ' /><span style="margin-left:3px;">Private</span>' +
                        '           <input id="lstPublicPrivate" type="radio" value="1" name="lstGeozonePublicPrivate" style="margin-left:20px;" ' + (isPublic ? ' checked ' : '') + ' /><span style="margin-left:3px;">Public</span>' +
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
                        '           <a href=\'javascript:void(0)\' onclick=\'editformmoreoptions();\'>More Options</a>' +
                        (ShowAssignToFleet ? ' <a href=\'javascript:void(0)\' onclick=\'assignGeozoneToFleet(' + data.GeozoneId + ');\'>Assign to fleet</a> ' : ' ') + 
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
                        '            <span id="lblEmailTitle" class="formtext">Email:</span>' +
                        '            </td>' +
                        '        <td style="height: 30px">' +
                        '            <input name="txtEmail" type="text" id="txtEmail" class="formtext bsmforminput" value="' + data.Email + '" style="width:173px;" /></td>' +
                        '    </tr>' +
                        '    <tr style="display:none;">' +
                        '        <td class="style4" style="height: 30px">' +
                        '            <span id="lblPhone" class="formtext"> Phone:</span>' +
                        '                                        <span id="RegularExpressionValidator1" class="formtext" style="color:Red;visibility:hidden;">*</span>' +
                        '                        </td>' +
                        '        <td style="height: 30px">' +
                        '            <input name="txtPhoneSMS" type="text" id="txtPhoneSMS" disabled="disabled" class="formtext bsmforminput" value="' + data.Phone + '" style="width:173px;" /></td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td class="formtext" style="height: 30px">' +
                        '            <span id="lblTimeZoneTitle" class="formtext">Time Zone:</span>' +
                        '                        </td>' +
                        '        <td style="height: 30px">' +
                        '            <select name="cboTimeZone" id="cboTimeZone" class="RegularText" style="width:168px;">' +
			            '                <option ' + (data.TimeZone == -12 ? ' selected="selected"' : '') + ' value="-12">GMT-12</option>' +
			            '                <option ' + (data.TimeZone == -11 ? ' selected="selected"' : '') + ' value="-11">GMT-11</option>' +
			            '                <option ' + (data.TimeZone == -10 ? ' selected="selected"' : '') + ' value="-10">GMT-10</option>' +
			            '                <option ' + (data.TimeZone == -9 ? ' selected="selected"' : '') + ' value="-9">GMT-9</option>' +
			            '                <option ' + (data.TimeZone == -8 ? ' selected="selected"' : '') + ' value="-8">GMT-8</option>' +
			            '                <option ' + (data.TimeZone == -7 ? ' selected="selected"' : '') + ' value="-7">GMT-7</option>' +
			            '                <option ' + (data.TimeZone == -6 ? ' selected="selected"' : '') + ' value="-6">GMT-6</option>' +
			            '                <option ' + (data.TimeZone == -5 ? ' selected="selected"' : '') + ' value="-5">GMT-5</option>' +
			            '                <option ' + (data.TimeZone == -4 ? ' selected="selected"' : '') + ' value="-4">GMT-4</option>' +
			            '                <option ' + (data.TimeZone == -3 ? ' selected="selected"' : '') + ' value="-3">GMT-3</option>' +
			            '                <option ' + (data.TimeZone == -2 ? ' selected="selected"' : '') + ' value="-2">GMT-2</option>' +
			            '                <option ' + (data.TimeZone == -1 ? ' selected="selected"' : '') + ' value="-1">GMT-1</option>' +
			            '                <option ' + (data.TimeZone == 0 ? ' selected="selected"' : '') + ' value="0">GMT</option>' +
			            '                <option ' + (data.TimeZone == 1 ? ' selected="selected"' : '') + ' value="1">GMT+1</option>' +
			            '                <option ' + (data.TimeZone == 2 ? ' selected="selected"' : '') + ' value="2">GMT+2</option>' +
			            '                <option ' + (data.TimeZone == 3 ? ' selected="selected"' : '') + ' value="3">GMT+3</option>' +
			            '                <option ' + (data.TimeZone == 4 ? ' selected="selected"' : '') + ' value="4">GMT+4</option>' +
			            '                <option ' + (data.TimeZone == 5 ? ' selected="selected"' : '') + ' value="5">GMT+5</option>' +
			            '                <option ' + (data.TimeZone == 6 ? ' selected="selected"' : '') + ' value="6">GMT+6</option>' +
			            '                <option ' + (data.TimeZone == 7 ? ' selected="selected"' : '') + ' value="7">GMT+7</option>' +
			            '                <option ' + (data.TimeZone == 8 ? ' selected="selected"' : '') + ' value="8">GMT+8</option>' +
			            '                <option ' + (data.TimeZone == 9 ? ' selected="selected"' : '') + ' value="9">GMT+9</option>' +
			            '                <option ' + (data.TimeZone == 10 ? ' selected="selected"' : '') + ' value="10">GMT+10</option>' +
			            '                <option ' + (data.TimeZone == 11 ? ' selected="selected"' : '') + ' value="11">GMT+11</option>' +
			            '                <option ' + (data.TimeZone == 12 ? ' selected="selected"' : '') + ' value="12">GMT+12</option>' +
			            '                <option ' + (data.TimeZone == 13 ? ' selected="selected"' : '') + ' value="13">GMT+13</option>' +
		                '            </select></td>' +
                        '        <td class="style4" style="height: 30px">' +
                        '            &nbsp;</td>' +
                        '        <td style="height: 30px">' +
                        '            &nbsp;</td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td colspan="2" style="height: 30px">' +
                        '                                    <span id="lblMultipleEmails" class="formtext">*Multiple email addresses Must be Separated by semicolon or comma</span>' +
                        '        </td>' +
                        '    </tr>' +
                        '    <tr>' +
                        '        <td style="height: 30px" colspan="2">' +
                        '            <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" ' + (data.AutoAdjustDayLightSaving == true ? ' checked="checked"' : '') + ' /><label for="chkDayLight">Automatically adjust for daylight savings time</label></span>';
    if (otype == "Geozone") {
        content = content +
                        '            <table class="geozonefield" cellspacing="0" cellpadding="0" border="0">' +
                        '                <tr>' +
                        '                    <td>' +
                        '                        <input id="chkCritical" type="checkbox" name="chkCritical" ' + (data.Critical == true ? ' checked="checked"' : '') + ' /><label for="chkCritical">Critical Alarm</label></td>' +
                        '                </tr>' +
                        '                <tr>' +
                        '                    <td>' +
                        '                        <input id="chkWarning" type="checkbox" name="chkWarning" ' + (data.Warning == true ? ' checked="checked"' : '') + ' /><label for="chkWarning">Warning Alarm</label></td>' +
                        '                </tr>' +
                        '                <tr>' +
                        '                    <td>' +
                        '                        <input id="chkNotify" type="checkbox" name="chkNotify" ' + (data.Notify == true ? ' checked="checked"' : '') + ' /><label for="chkNotify">Notify Alarm</label></td>' +
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
                        '           <a href=\'javascript:void(0)\' onclick=\'editformmainoptions();\'>Back</a>' +
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

    deletebutton = "<input " + deletebuttonDisabled + " type='button' class='kd-button" + (assigned == 1 ? " kd-button-disabled" : "") + "' onclick='deletePolygon(\"" + otype + "\",\"" + (otype == "Geozone" ? data.GeozoneId : escape(data.LandmarkName)) + "\", polygonPopup, selectedFeature);' value='Delete!' />";

    content = content +
                    "<div style='font-size: 1em;margin-top:10px;'>" + deletebutton + " <input type='button' class='kd-button' onclick='savePolygon_NewTZ(this, polygonPopup, selectedFeature);' value='Save' style='margin-left:20px;' /> <input type='button' class='kd-button' value='Cancel' onclick='cancelEditPolygon(polygonPopup, selectedFeature);' /></div>" +
                    "</form></div>";
    return '<div id="polygoninfo">' + infocontent + '</div>' + '<div id="polygoneditform" style="display:none;">' + content + '</div>';
}

function getPopupContent(cv) {    
    var popupContent = "<div class='popupContent'><a href='javascript:void(0)' onclick='setMapToCenter(" + cv.Longitude + "," + cv.Latitude + ");'><h6>" + cv.Description + "</a><br />";
    if ($.trim(cv.Driver) != '')
        popupContent = popupContent + 'Driver: ' + cv.Driver + '<br />';
    //cv.Description = "test's\"";
    popupContent = popupContent +
                    cv.StreetAddress +
                    "<br />Time: " + cv.convertedDisplayDate +
                    "<br />Heading: " + cv.MyHeading +
                    "<br />Speed: " + cv.CustomSpeed +
                    "<br />Status: " + cv.VehicleStatus +
                    "<br /></h6>";
    popupContent += "<div class='popupwizard'>" +
                    "<a href='javascript:void(0)' onclick='loadPopupsendmessage( " + cv.BoxId + ")'>Send Message</a>";
    popupContent += " | <a href='javascript:void(0)' onclick='parent.showHistoryTab(" + cv.VehicleId + ")'>History</a>";
    if (ShowRouteAssignment) {
        popupContent += " | <a href='javascript:void(0)' onclick='RouteAssignmentWindow(" + cv.VehicleId + ", \"" + cv.Description.replace(/\'/g, "[singlequote]").replace(/\"/g, "\\\"") + "\")'>Route Assignment</a>";
    }
    popupContent += "</div>" +
                    "<div id='wizardclosebar" + cv.BoxId + "' class='wizardclosebarcls' style='display: none;'><div id='wizardclosebar-closebutton'><a href='javascript:void(0)' onclick='closeWizard(" + cv.BoxId + ");'><img src='" + rootpath + "images/close.png' /></a></div></div>" +
                    "<div id='popupsendmessage" + cv.BoxId + "' class='popupsendmessagecls' style='display: none;'></div>";
    popupContent += "</div>";
    //alert(popupContent);
    return popupContent;
}

function getHistoryPopupContent(cv) {
    var popupContent;
    //alert('popupcontent' + cv.StopIndex);
    if (cv.StopIndex) {
        popupContent = '<b>[' + cv.StopIndex + ']</b>: ' + cv.Remarks + ' (' + cv.StopDuration + ') ' + cv.convertedArrivalDisplayDate + '<br /><b>Address:</b> ' + cv.Location;
    }
    else {
        popupContent = "<div class='popupContent'><a href='javascript:void(0)' onclick='setMapToCenter(" + cv.Longitude + "," + cv.Latitude + ");'><h6>" + cv.Description + "</a><br />";

        popupContent = popupContent +
                    cv.StreetAddress +
                    "<br />Time: " + cv.convertedDisplayDate +
                    "<br />Heading: " + cv.MyHeading +
                    "<br />Speed: " + (cv.Speed == -1 ? 'N/A' : cv.Speed) +
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

function closeWizard(BoxId) {
    donotclosevehiclepopups = false;
    $('#popupsendmessage' + BoxId).html('');
    $('#popupsendmessage' + BoxId).hide();
    $('#wizardclosebar' + BoxId).hide();
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
        dateFormat: 'mm/dd/yy',
        closeText: 'Close'
    });  

    $('#SearchHistoryTime').val(SearchHistoryTime);
    $('#SearchHistoryDate').val(SearchHistoryDate);
}

function searchAddress() {
    var geocoder = new google.maps.Geocoder();
    searchMarker.removeAllFeatures();
    geocoder.geocode({ "address": $('#txtSearchAddress').val() }, function (results, status) {
        //addressTryTimes++;
        if (status == google.maps.GeocoderStatus.OK) {
            searchLat = results[0].geometry.location.lat();
            searchLon = results[0].geometry.location.lng();
            //alert(searchLat + ',' + searchLon);
            var newLoc = transformCoords(searchLon, searchLat);
            map.setCenter(newLoc, 16);

            var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
            var marker = new OpenLayers.Feature.Vector(point);
            //var popupContent = getPopupContent(currentVehicle);

            marker.attributes = { markerType: 'searchMarker', address: $('#txtSearchAddress').val(), lon: searchLon, lat: searchLat };

            searchMarker.addFeatures([marker]);

            onSearchMarkerClick(marker);
        }
        else {
            $('#searchmessage').html('Invalid address').show().delay(2000).fadeOut(1000);
        }
    });
}

function IniGoogleAutoComplete() {

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

    //var autocomplete = new google.maps.places.Autocomplete($("#txtSearchAddress")[0], {});

    //google.maps.event.addListener(autocomplete, 'place_changed', function () {
    //    var place = autocomplete.getPlace();
    //    //console.log(place.address_components);
    //});

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
    var myname = 'Service Assignment';
	var w=425;
	var h=220;
	var winl = (screen.width - w) / 2; 
	var wint = (screen.height - h) / 2; 
	winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
	win = window.open(mypage, myname, winprops) 
	if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
}