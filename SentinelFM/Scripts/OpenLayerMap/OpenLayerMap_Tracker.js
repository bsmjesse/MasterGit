// JavaScript Document
var format = 'image/png';
var geoserver = 'http://geomap.sentinelfm.com:9090/geoserver/wms';
var arcgisLayer;

var map, layer, newl, markers, currentMarkerIndex = 0;
var currentMarkers = new Array();
var finishedLastTimer = false;
var timer_is_on = 0;
var size, offset, icon, allVehicles, myWinID;
var infocontrols, highlightlayer;
var rootpath = top.location.href.match(/^(http.+\/)[^\/]+$/)[1];
var userdateformat = getSenchaDateFormat();

var winparent = window.opener;
if (winparent == null) {
    winparent = window.parent;
}

var overlaysettings = winparent.overlaysettings;

OpenLayers.ProxyHost = "/proxy/?url=";

interval = interval / 2;
OpenLayers.IMAGE_RELOAD_ATTEMPTS = 3;

var qsParm = new Array();

function getSenchaDateFormat() {
    if (userDate == 'dd/MM/yyyy')
        userDate = 'd/m/Y';
    else if (userDate == 'd/M/yyyy')
        userDate = 'j/n/Y';
    else if (userDate == 'dd/MM/yy')
        userDate = 'd/m/y';
    else if (userDate == 'd/M/yy')
        userDate = 'j/n/y';
    else if (userDate == 'd MMM yyyy')
        userDate = 'j M Y';
    else if (userDate == 'MM/dd/yyyy')
        userDate = 'm/d/Y';
    else if (userDate == 'M/d/yyyy')
        userDate = 'n/j/Y';
    else if (userDate == 'MM/dd/yy')
        userDate = 'm/d/y';
    else if (userDate == 'M/d/yy')
        userDate = 'n/j/y';
    else if (userDate == 'MMMM d yy')
        userDate = 'M j y';
    else if (userDate == 'yyyy/MM/dd')
        userDate = 'Y/m/d';
    if (userTime == "hh:mm:ss tt")
        userTime = "h:i:s A";
    else
        userTime = "H:i:s";
    return userDate + " " + userTime;
}

function qs()
{
   var query = window.location.search.substring(1);
   if (query.indexOf("Id") != - 1)
   {
      if (query.indexOf('&') != - 1)
      {
         var parms = query.split('&');
         for (var i = 0; i < parms.length; i ++ )
         {
            var pos = parms[i].indexOf('=');
            if (pos > 0)
            {
               var key = parms[i].substring(0, pos);
               var val = parms[i].substring(pos + 1);
               qsParm[key] = val;
            }
         }
      }
      else
      {
         var pos = query.indexOf('=');
         // alert(pos);
         if (pos > 0)
         {
            var key = query.substring(0, pos);
            // alert(key);
            var val = query.substring(pos + 1);
            // alert(val);
            qsParm[key] = val;
         }
      }
   }
}

function init()
{
   try
   {

      var maxExtent = new OpenLayers.Bounds( - 20037508, - 20037508, 20037508, 20037508),
      restrictedExtent = maxExtent.clone(),
      maxResolution = 126543.0339;

      var options =
      {
         projection : new OpenLayers.Projection("EPSG:900913"),
         displayProjection : new OpenLayers.Projection("EPSG:4326"),
         units : "m",
         numZoomLevels : 20,
         maxResolution : maxResolution,
         maxExtent : maxExtent,
         restrictedExtent : restrictedExtent/* ,
         controls : [
         new OpenLayers.Control.Navigation(),
         new OpenLayers.Control.PanZoomBar(),
         new OpenLayers.Control.LayerSwitcher(
         {
         'ascending' : true
         }
         ),
         new OpenLayers.Control.ScaleLine(),
         new OpenLayers.Control.MousePosition(),
         new OpenLayers.Control.KeyboardDefaults()
         ] */
      }
      ;

      // map = new OpenLayers.Map('map');
      map = new OpenLayers.Map('map', options);

      map.addControl(new OpenLayers.Control.Navigation());
      map.addControl(new OpenLayers.Control.LayerSwitcher(
      {
         'ascending' : true
      }
      ));
      map.addControl(new OpenLayers.Control.ScaleLine());
      map.addControl(new OpenLayers.Control.MousePosition());
      map.addControl(new OpenLayers.Control.KeyboardDefaults());

      /*
      // create OSM layer
      var mapnik = new OpenLayers.Layer.OSM("Theme 1");

      var ghyb = new OpenLayers.Layer.Google(
      "Google Hybrid",
      {
         type : google.maps.MapTypeId.HYBRID, sphericalMercator : true, numZoomLevels : 20
      }
      );
      // create OSM layer
      //var mapnik = new OpenLayers.Layer.OSM("Base Map");
      // create OAM layer
      //   var oam = new OpenLayers.Layer.XYZ(
      //   "OpenAerialMap",
      //   "http://tile.openaerialmap.org/tiles/1.0.0/openaerialmap-900913/${z}/${x}/${y}.png",
      //   {
      //      sphericalMercator : true
      //   }
      //   );
      // create OSM layer
      //   var osmarender = new OpenLayers.Layer.OSM(
      //   "OpenStreetMap (Tiles@Home)",
      //   "http://tah.openstreetmap.org/Tiles/tile/${z}/${x}/${y}.png"
      //   );

      //    create Google Mercator layers
      //   var gmap = new OpenLayers.Layer.Google(
      //   "Google Streets",
      //   {
      //      sphericalMercator : true
      //   }
      //   );
      //   var gsat = new OpenLayers.Layer.Google(
      //   "Google Satellite",
      //   {
      //      type : G_SATELLITE_MAP, sphericalMercator : true, numZoomLevels : 22
      //   }
      //   );
      //   var ghyb = new OpenLayers.Layer.Google(
      //   "Google Hybrid",
      //   {
      //      type : G_HYBRID_MAP, sphericalMercator : true
      //   }
      //   );

      //      var ghyb = new OpenLayers.Layer.Google(
      //      "Google Hybrid",
      //      {
      //         type : google.maps.MapTypeId.HYBRID, sphericalMercator : true, numZoomLevels : 20
      //      }
      //      );

      // create Virtual Earth layers
      //      var veroad = new OpenLayers.Layer.VirtualEarth(
      //      "Virtual Earth Roads",
      //      {
      //         'type' : VEMapStyle.Road, sphericalMercator : true
      //      }
      //      );
      //      var veaer = new OpenLayers.Layer.VirtualEarth(
      //      "Virtual Earth Aerial",
      //      {
      //         'type' : VEMapStyle.Aerial, sphericalMercator : true
      //      }
      //      );
      //      var vehyb = new OpenLayers.Layer.VirtualEarth(
      //      "Virtual Earth Hybrid",
      //      {
      //         'type' : VEMapStyle.Hybrid, sphericalMercator : true
      //      }
      //      );

      // Weather Radar Overlay Layer
      var nexrad = new OpenLayers.Layer.WMS(
      "Nexrad Weather Radar",
      "http://mesonet.agron.iastate.edu/cgi-bin/wms/nexrad/n0r.cgi?",
      {
         layers : "nexrad-n0r", transparent : "TRUE", format : 'image/png'
      }
      ,
      {
         isBaseLayer : false, buffer : 0, singleTile : false, opacity : 0.5, visibility : false
      }
      );

      var oiltrax_poi_water = new OpenLayers.Layer.WMS("Water",
      geoserver,
      {
         'layers' : 'oiltrax:oiltrax_poi_water', transparent : true, format : format
      }
      ,
      {
         visibility : true, opacity : 1.0, singleTile : true, ratio : 1
      }
      );
      var oiltrax_poi_wellsite = new OpenLayers.Layer.WMS("Wellsite",
      geoserver,
      {
         'layers' : 'oiltrax:oiltrax_poi_wellsite', transparent : true, format : format
      }
      ,
      {
         visibility : true, opacity : 1.0, singleTile : true, ratio : 1
      }
      );
      var oiltrax_poi_batteries = new OpenLayers.Layer.WMS("Batteries",
      geoserver,
      {
         'layers' : 'oiltrax:oiltrax_poi_batteries', transparent : true, format : format
      }
      ,
      {
         visibility : true, opacity : 1.0, singleTile : true, ratio : 1
      }
      );

      var oiltrax_poi_otherfacilities = new OpenLayers.Layer.WMS("Other Facilities",
      geoserver,
      {
         'layers' : 'oiltrax:oiltrax_poi_otherfacilities', transparent : true, format : format
      }
      ,
      {
         visibility : true, opacity : 1.0, singleTile : true, ratio : 1
      }
      );

      var oiltrax_sasklsd = new OpenLayers.Layer.WMS("Sasktel LSD",
      geoserver,
      {
         'layers' : 'oiltrax:sask_lsd', transparent : true, format : format
      }
      ,
      {
         visibility : true, opacity : 0.5, singleTile : true, ratio : 1
      }
      );
      var oiltrax_manbclsd = new OpenLayers.Layer.WMS("Man-BC LSD",
      geoserver,
      {
         'layers' : 'oiltrax:manbc_lsd', transparent : true, format : format
      }
      ,
      {
         visibility : true, opacity : 0.5, singleTile : true, ratio : 1
      }
      );
      var oiltrax_albertalsd = new OpenLayers.Layer.WMS("Alberta LSD",
      geoserver,
      {
         'layers' : 'oiltrax:alberta_lsd', transparent : true, format : format
      }
      ,
      {
         visibility : true, opacity : 0.5, singleTile : true, ratio : 1
      }
      );

      var oiltrax_polygon = new OpenLayers.Layer.WMS("Polygon",
      geoserver,
      {
         'layers' : 'oiltrax:oiltrax_polygon', transparent : true, format : format
      }
      ,
      {
         visibility : false, opacity : 0.5, singleTile : true, ratio : 1
      }
      );*/

      /*// create Yahoo layer
      var yahoo = new OpenLayers.Layer.Yahoo(
      "Yahoo Street",
      { sphericalMercator : true }
      );
      var yahoosat = new OpenLayers.Layer.Yahoo(
      "Yahoo Satellite",
      { 'type' : YAHOO_MAP_SAT, sphericalMercator : true }
      );
      var yahoohyb = new OpenLayers.Layer.Yahoo(
      "Yahoo Hybrid",
      { 'type' : YAHOO_MAP_HYB, sphericalMercator : true }
      ); */

      //   var jpl_wms = new OpenLayers.Layer.WMS("NASA Global Mosaic",
      //   "http://wms.jpl.nasa.gov/wms.cgi",
      //   {
      //      layers : "modis,global_mosaic"
      //   }
      //   );
      // allVehicles = p.frames["trackwindow"].ShowTrackVehicleData("", false);

      // map = new OpenLayers.Map('map', options);
      //   layer = new OpenLayers.Layer.WMS("OpenLayers WMS",
      //   "http://vmap0.tiles.osgeo.org/wms/vmap0",
      //   {
      //      layers : 'basic'
      //   }
      //   );


      // layer = new OpenLayers.Layer.OSM();
      var apiKey = "AjfXbh4yW0IPCW0jQaXRxfZwLdAIGJjXk0v_OhoZo2gs6rygaryV1aCihwPQ4F9l";
      // var map;

      /* map.addLayers([gmap, gsat, ghyb,
      mapnik, osmarender,
      veroad, veaer, vehyb,
      // yahoo, yahoosat, yahoohyb,
      oam, jpl_wms, layer, nexrad
      , oiltrax_poi_water,
      oiltrax_poi_wellsite,
      oiltrax_poi_batteries,
      oiltrax_poi_otherfacilities
      // ,
      // wms, vector,
      ]); */

      highlightLayer = new OpenLayers.Layer.Vector("Highlighted Features",
      {
         displayInLayerSwitcher : false,
         isBaseLayer : false
      }
      );

      infoControls =
      {
         click : new OpenLayers.Control.WMSGetFeatureInfo(
         {
            url : geoserver,
            title : 'Identify features by clicking',
            layers : [oiltrax_poi_water],
            queryVisible : true
         }
         ),
         hover : new OpenLayers.Control.WMSGetFeatureInfo(
         {
            url : geoserver,
            title : 'Identify features by clicking',
            layers : [oiltrax_poi_water],
            hover : true,
            queryVisible : true
         }
         )
      }
      ;
      /*
      map.addLayers([
      mapnik,
      // osmarender,
      // gmap, gsat, ghyb,
      // veroad, veaer, vehyb,
      ghyb// ,
      // yahoo, yahoosat, yahoohyb, oam, jpl_wms,

      ]);


      // }
      // var orgid = '<%=sn.User.OrganizationId %>';
      if (orgid == 727)
      {
         var fcmq_tq = new OpenLayers.Layer.WMS("TQ (blue)",
         geoserver,
         {
            'layers' : 'fcmq:TQ_4326', transparent : true, format : format
         }
         ,
         {
            visibility : true, opacity : 1.0, singleTile : true, ratio : 1
         }
         );
         var fcmq_local = new OpenLayers.Layer.WMS("Local (orange)",
         geoserver,
         {
            'layers' : 'fcmq:Local_4326', transparent : true, format : format
         }
         ,
         {
            visibility : true, opacity : 1.0, singleTile : true, ratio : 1
         }
         );
         var fcmq_n = new OpenLayers.Layer.WMS("N (pink)",
         geoserver,
         {
            'layers' : 'fcmq:N_4326', transparent : true, format : format
         }
         ,
         {
            visibility : true, opacity : 1.0, singleTile : true, ratio : 1
         }
         );
         var fcmq_reg = new OpenLayers.Layer.WMS("Reg (green)",
         geoserver,
         {
            'layers' : 'fcmq:REG_4326', transparent : true, format : format
         }
         ,
         {
            visibility : true, opacity : 1.0, singleTile : true, ratio : 1
         }
         );
         var fcmq_nsentiers = new OpenLayers.Layer.WMS("Trails No",
         geoserver,
         {
            'layers' : 'fcmq:nsentiers_4326', transparent : true, format : format
         }
         ,
         {
            visibility : true, opacity : 1.0, singleTile : true, ratio : 1
         }
         );
         var fcmq_nsentiers_reg = new OpenLayers.Layer.WMS("Trails No",
         geoserver,
         {
            'layers' : 'fcmq:NSentiers_REG', transparent : true, format : format
         }
         ,
         {
            visibility : true, opacity : 1.0, singleTile : true, ratio : 1
         }
         );
         var fcmq_nsentiers_tq = new OpenLayers.Layer.WMS("Trails No",
         geoserver,
         {
            'layers' : 'fcmq:NSentiers_TQ', transparent : true, format : format
         }
         ,
         {
            visibility : true, opacity : 1.0, singleTile : true, ratio : 1
         }
         );

         map.addLayers([fcmq_tq,
         fcmq_local,
         fcmq_n,
         fcmq_reg,
         fcmq_nsentiers_tq,
         fcmq_nsentiers_reg]);
      }
      else
      {
         map.addLayers([oiltrax_poi_water,
         oiltrax_poi_wellsite,
         oiltrax_poi_batteries,
         oiltrax_poi_otherfacilities,
         oiltrax_sasklsd,
         oiltrax_albertalsd,
         oiltrax_manbclsd,
         // oiltrax_polygon,
         nexrad,
         highlightLayer]);
      }*/

    var resolutions = [];

    for (var i = 0; i < layerInfo.tileInfo.lods.length;
      i++) {
        resolutions.push(layerInfo.tileInfo.lods[i].resolution);
    }

    var layerMaxExtent = new OpenLayers.Bounds(
      layerInfo.fullExtent.xmin,
      layerInfo.fullExtent.ymin,
      layerInfo.fullExtent.xmax,
      layerInfo.fullExtent.ymax
    );


    // create OSM layer
    if (winparent.ifShowTheme1 && !ISSECURE)
        var mapnik = new OpenLayers.Layer.OSM("Theme 1");

    if (winparent.ifShowTheme2 && !ISSECURE) {
        var cloudmade = new OpenLayers.Layer.CloudMade("Theme 2",
              {
                  key: '8ee2a50541944fb9bcedded5165f09d9'
              }
              );
          }
          
          if (winparent.ifShowArcgis && !ISSECURE) {
              
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
      
          if (winparent.ifShowGoogleStreets) {
        var gmap = new OpenLayers.Layer.Google(
          "Google Streets",
          {
              sphericalMercator: true
          }
          );
    }

      if (winparent.ifShowGoogleHybrid) {
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
      if (winparent.ifShowBingRoads && !ISSECURE) {
        var veroad = new OpenLayers.Layer.VirtualEarth(
          "Bing Roads",
          {
              'type': VEMapStyle.Road, sphericalMercator: true
          }
          );
    }

      if (winparent.ifShowBingHybrid && !ISSECURE) {
        var vehyb = new OpenLayers.Layer.VirtualEarth(
          "Bing Hybrid",
          {
              'type': VEMapStyle.Hybrid, sphericalMercator: true
          }
          );
    }

    if (overlaysettings.nexradweatherradar && !ISSECURE) {
        // Weather Radar Overlay Layer
        var nexrad = new OpenLayers.Layer.WMS(
              "Nexrad Weather Radar",
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

    if (overlaysettings.water && !ISSECURE) {
        var oiltrax_poi_water = new OpenLayers.Layer.WMS("Water",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_water', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.waterVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
          );
    }
    if (overlaysettings.wellsite && !ISSECURE) {
        var oiltrax_poi_wellsite = new OpenLayers.Layer.WMS("Wellsite",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_wellsite', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.wellsiteVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
    }
    if (overlaysettings.batteries && !ISSECURE) {
        var oiltrax_poi_batteries = new OpenLayers.Layer.WMS("Batteries",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_batteries', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.batteriesVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
    }

    if (overlaysettings.otherfacilities && !ISSECURE) {
        var oiltrax_poi_otherfacilities = new OpenLayers.Layer.WMS("Other Facilities",
              geoserver,
              {
                  'layers': 'oiltrax:oiltrax_poi_otherfacilities', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.otherfacilitiesVisibility, opacity: 1.0, singleTile: true, ratio: 1
              }
           );
    }

    if (overlaysettings.sasktellsd && !ISSECURE) {
        var oiltrax_sasklsd = new OpenLayers.Layer.WMS("Sasktel LSD",
              geoserver,
              {
                  'layers': 'oiltrax:sask_lsd', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.sasktellsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
           );
    }
    if (overlaysettings.manbslsd && !ISSECURE) {
        var oiltrax_manbclsd = new OpenLayers.Layer.WMS("Man-BC LSD",
              geoserver,
              {
                  'layers': 'oiltrax:manbc_lsd', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.manbslsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
          );
    }
    if (overlaysettings.albertalsd && !ISSECURE) {
        var oiltrax_albertalsd = new OpenLayers.Layer.WMS("Alberta LSD",
              geoserver,
              {
                  'layers': 'oiltrax:alberta_lsd', transparent: true, format: format
              }
              ,
              {
                  visibility: overlaysettings.albertalsdVisibility, opacity: 0.5, singleTile: true, ratio: 1
              }
           );
    }

    if (overlaysettings.trails && !ISSECURE) {
        var oiltrax_trails = new OpenLayers.Layer.WMS("Trails",
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
    if (overlaysettings.additionalroads && !ISSECURE) {
        var oiltrax_goat = new OpenLayers.Layer.WMS("Additional Roads",
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
    if (overlaysettings.additionalroads2 && !ISSECURE) {
        var oiltrax_goat2 = new OpenLayers.Layer.WMS("Additional Roads 2",
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

    if (overlaysettings.CNRailMilePosts && !ISSECURE) {
        var cnrail_mileposts = new OpenLayers.Layer.WMS("CNRail MilePost",
                geoserver,
                { 'layers': 'cnrail:cnrail_mileposts', transparent: true, format: format },
                { visibility: overlaysettings.CNRailMilePostsVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
    }

    if (overlaysettings.UPRailNetwork && !ISSECURE) {
        var uprail_main = new OpenLayers.Layer.WMS("UPRail Network",
                geoserver,
                { 'layers': 'uprail:up_main', transparent: true, format: format },
                { visibility: overlaysettings.UPRailNetworkVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
    }

    if (overlaysettings.UPRailMilePosts && !ISSECURE) {
        var uprail_mileposts = new OpenLayers.Layer.WMS("UPRail MilePost",
                geoserver,
                { 'layers': 'uprail:uprail_mileposts', transparent: true, format: format },
                { visibility: overlaysettings.UPRailMilePostsVisibility, opacity: 1.0, singleTile: false, ratio: 1 }
            );
    }

    if (overlaysettings.UPRailRightOfWay && !ISSECURE) {
        var UPRailRightOfWay = new OpenLayers.Layer.WMS("UPRail Right Of Way",
                geoserver,
                { 'layers': 'uprail:right_of_way', transparent: true, format: format },
                { visibility: overlaysettings.UPRailRightOfWayVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );
    }

    if (overlaysettings.UPRailPoly && !ISSECURE) {
        var UPRailPoly = new OpenLayers.Layer.WMS("UPRail Poly",
                geoserver,
                { 'layers': 'uprail:uprail_polygon', transparent: true, format: format },
                { visibility: overlaysettings.UPRailPolyVisibility, opacity: 0.5, singleTile: false, ratio: 1 }
            );
    }

    if (winparent.ifShowTheme1 && !ISSECURE) {
        map.addLayer(mapnik);
        if (winparent.getBaseLayer() == "Theme 1") map.setBaseLayer(mapnik);
    }
    if (winparent.ifShowTheme2 && !ISSECURE) {
        map.addLayer(cloudmade);
        if (winparent.getBaseLayer() == "Theme 2") map.setBaseLayer(cloudmade);
    }
    if (winparent.ifShowArcgis && !ISSECURE) {
        map.addLayer(arcgisLayer);
        if (winparent.getBaseLayer() == "Arcgis") map.setBaseLayer(arcgisLayer);
    }
    if (winparent.ifShowGoogleStreets) {
        map.addLayer(gmap);
        if (winparent.getBaseLayer() == "Google Streets") map.setBaseLayer(gmap);
    }
    if (winparent.ifShowGoogleHybrid) {
        map.addLayer(ghyb);
        if (winparent.getBaseLayer() == "Google Hybrid") map.setBaseLayer(ghyb);
    }
    if (winparent.ifShowBingRoads && !ISSECURE) {
        map.addLayer(veroad);
        if (winparent.getBaseLayer() == "Bing Roads") map.setBaseLayer(veroad);
    }
    if (winparent.ifShowBingHybrid && !ISSECURE) {
        map.addLayer(vehyb);
        if (winparent.getBaseLayer() == "Bing Hybrid") map.setBaseLayer(vehyb);
    }

    if (orgid == 727) {
        var fcmq_tq = new OpenLayers.Layer.WMS("TQ (blue)",
         geoserver,
         {
             'layers': 'fcmq:TQ_4326', transparent: true, format: format
         }
         ,
         {
             visibility: true, opacity: 1.0, singleTile: true, ratio: 1
         }
         );
        var fcmq_local = new OpenLayers.Layer.WMS("Local (orange)",
         geoserver,
         {
             'layers': 'fcmq:Local_4326', transparent: true, format: format
         }
         ,
         {
             visibility: true, opacity: 1.0, singleTile: true, ratio: 1
         }
         );
        var fcmq_n = new OpenLayers.Layer.WMS("N (pink)",
         geoserver,
         {
             'layers': 'fcmq:N_4326', transparent: true, format: format
         }
         ,
         {
             visibility: true, opacity: 1.0, singleTile: true, ratio: 1
         }
         );
        var fcmq_reg = new OpenLayers.Layer.WMS("Reg (green)",
         geoserver,
         {
             'layers': 'fcmq:REG_4326', transparent: true, format: format
         }
         ,
         {
             visibility: true, opacity: 1.0, singleTile: true, ratio: 1
         }
         );
        var fcmq_nsentiers = new OpenLayers.Layer.WMS("Trails No",
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
         );






        map.addLayers([fcmq_tq,
         fcmq_local,
         fcmq_n,
         fcmq_reg,
         fcmq_nsentiers_tq,
         fcmq_nsentiers_reg]);
        
    }
    else {
        if (overlaysettings.water && !ISSECURE) {
            map.addLayer(oiltrax_poi_water);
        }
        if (overlaysettings.wellsite && !ISSECURE) {
            map.addLayer(oiltrax_poi_wellsite);
        }
        if (overlaysettings.batteries && !ISSECURE) {
            map.addLayer(oiltrax_poi_batteries);
        }
        if (overlaysettings.otherfacilities && !ISSECURE) {
            map.addLayer(oiltrax_poi_otherfacilities);
        }
        if (overlaysettings.sasktellsd && !ISSECURE) {
            map.addLayer(oiltrax_sasklsd);
        }
        if (overlaysettings.albertalsd && !ISSECURE) {
            map.addLayer(oiltrax_albertalsd);
        }
        if (overlaysettings.manbslsd && !ISSECURE) {
            map.addLayer(oiltrax_manbclsd);
        }
        if (overlaysettings.trails && !ISSECURE) {
            map.addLayer(oiltrax_trails);
        }
        if (overlaysettings.additionalroads && !ISSECURE) {
            map.addLayer(oiltrax_goat);
        }
        if (overlaysettings.additionalroads2 && !ISSECURE) {
            map.addLayer(oiltrax_goat2);
        }
        if (overlaysettings.nexradweatherradar && !ISSECURE) {
            map.addLayer(nexrad);
        }
        if (overlaysettings.CNRailMilePosts && !ISSECURE) {
            map.addLayer(cnrail_mileposts);
        }

        if (overlaysettings.UPRailNetwork && !ISSECURE) {
            map.addLayer(uprail_main);
        }

        if (overlaysettings.UPRailMilePosts && !ISSECURE) {
            map.addLayer(uprail_mileposts);
        }

        if (overlaysettings.UPRailRightOfWay && !ISSECURE) {
            map.addLayer(UPRailRightOfWay);
        }

        if (overlaysettings.UPRailPoly && !ISSECURE) {
            map.addLayer(UPRailPoly);
        }

        map.addLayers([
        // oiltrax_polygon,
         highlightLayer]);
        
    }


      for (var i in infoControls)
      {
         infoControls[i].events.register("getfeatureinfo", this, showInfo);
         map.addControl(infoControls[i]);
      }

      // map.setCenter(new OpenLayers.LonLat(0, 0), 0);
      // Google.v3 uses EPSG : 900913 as projection, so we have to
      // transform our coordinates



      // markers = new OpenLayers.Layer.Markers("Vehicles");
      markers = new OpenLayers.Layer.Markers("Vehicles",
      {
         displayInLayerSwitcher : false,
         isBaseLayer : false
      }
      );

      size = new OpenLayers.Size(8, 16);
      offset = new OpenLayers.Pixel( - (size.w / 2), - size.h);

      // if ( ! map.getCenter()) {
      // map.zoomToMaxExtent();
      if ( ! map.getCenter())
      {
         // map.zoomToMaxExtent();
         map.setCenter(transformCoords( - 72.872559, 46.767074), 4);
      }
      // map.addControl(new OpenLayers.Control.LayerSwitcher());
      qs();
      myWinID = qsParm["WinId"];
      // var p = parent;
      
      var didntLoadFrmParent = false;
      if (winparent != null)
      {
         if (myWinID && myWinID != "")
         {
            allVehicles = winparent.GetWinInitialTrackData(myWinID);
         }
         else
         {
            if ( ! allVehicles || allVehicles == "")
            {
               allVehicles = winparent.GetTelogisMapData();
               didntLoadFrmParent = true;
            }
         }
         if (allVehicles && (didntLoadFrmParent == true || myWinID != ""))
         {
            if (allVehicles != "")
            {
               ShowMultipleAssets(allVehicles);
               if ( ! timer_is_on)
               {
                  timer_is_on = 1;
                  finishedLastTimer = true;
                  CheckAndUpdateMap();
               }
            }
         }
      }
      // map.zoomToMaxExtent();
   }
   catch(err)
   {
      // alert(err);
   }

}

function msieversion()
{
   var ua = window.navigator.userAgent
   var msie = ua.indexOf("MSIE ")
   if (msie > 0)      // If Internet Explorer, return version number
   return parseInt(ua.substring(msie + 5, ua.indexOf(".", msie)))
   else                 // If another browser, return 0
   return 0

}

//         function getVehicles() {
//             // var brVersion = msieversion();
//             // if (brVersion == 0 || brVersion > 7) {
//             try {
//                 var parent = window.opener;
//                 if (parent == null) {
//                     parent = window.parent;
//                 }
//                 if (parent != null) {
//                     var AllVehicles = parent.GetTelogisMapData();
//                     if (AllVehicles) {
//                         if (AllVehicles != "" && AllVehicles != null) {
//                             // ShowMultipleAssets(AllVehicles);
//                             return AllVehicles;
//                         }
//                         else {
//                             setTimeout("getVehicles()", interval);
//                         }
//                     }
//                     else {
//                         setTimeout("getVehicles()", interval);
//                     }
//                 }
//             } catch (err) {
//                 setTimeout("getVehicles()", interval);
//             }
//             // }
//             // else {
//                 // Need to do in future
//             // }
//         }

function showInfo(evt)
{
   if (evt.features && evt.features.length)
   {
      highlightLayer.destroyFeatures();
      highlightLayer.addFeatures(evt.features);
      highlightLayer.redraw();
   }
   else
   {
      $('responseText').innerHTML = evt.text;
   }
}

function zoomMap(fleet)
{
   try
   {
      if (fleet != null)
      {
         fleet = eval(fleet);
         if (fleet != null && fleet != "")
         {
            // if (fleet.length > 1) {
            var bounds = new OpenLayers.Bounds();
            for (i = 0; i < fleet.length; i ++ )
            {
               try
               {
                  bounds.extend(transformCoords(fleet[i].Longitude, fleet[i].Latitude));
                  // transformCoords(fleet[i].lon, fleet[i].lat));
               }
               catch (err1)
               {
                  // console.log(err1);
                  var test = err.Message;
                  // alert(err.Message);
               }
            }
            // var bbox = bounds.toBBOX();
            map.zoomToExtent(bounds, true);
            // }
            // else
            // {
            var currentZoom = map.getZoom();
            // getZoomForExtent(bounds, true);
            // console.log(currentZoom);
            map.zoomTo(currentZoom - 2);
            // }
         }
      }
   }
   catch (err)
   {
      // console.log(err);
      var test = err.Message;
   }
}
// function selectFeatureDelegate(fleet) {
//        function selectFeature3()
//        {
//            popup = new OpenLayers.Popup.FramedCloud("featurePopup", transformCoords(this.fleet.lon, this.fleet.lat),
//            new OpenLayers.Size(100, 100),
//            "<h2>" + this.fleet.desc + "</h2>" +
//            this.fleet.addr +
//            "<br />Time: " + this.fleet.date +
//            "<br />Heading: " + this.fleet.head +
//            "<br />Speed: " + this.fleet.spd +
//            "<br />Status: " + this.fleet.stat,
//            this.icon, true, null);
//            map.addPopup(popup, true);
//            //
//        }
function showVehiclePopups()
{
   popup = new OpenLayers.Popup.FramedCloud("featurePopup", this.location,
   new OpenLayers.Size(100, 100),
   this.featureHTML,
   this.icon, true, null);
   map.addPopup(popup, true);
   //
}

function closeVehiclePopups()
{
   var currentPopups = map.popups;
   for(i = 0; i < currentPopups.length; i ++ )
   {
      map.removePopup(currentPopups[i]);
      // currentPopups[i].destroy();
   }
}
//        function selectFeature2() {
//            map.addPopup(this.popup);
//            // , true);
//            //
//        }
function ShowMultipleAssets(fleet)
{
   if (fleet != null && fleet != "")
   {
      var i = 0;
      allVehicles = fleet;
      var bounds = new OpenLayers.Bounds();
      for (i = 0; i < fleet.length; i ++ )
      {
         var currentVehicle = fleet[i];
         var vicon;
         if (currentVehicle.ImagePath != "" && currentVehicle.ImagePath != null)
             vicon = new OpenLayers.Icon("images/CustomIcon/" + currentVehicle.icon);
         else
             vicon = new OpenLayers.Icon("./New20x20/" + currentVehicle.icon);
         if (currentVehicle.BoxId=='27492') {
            vicon = new OpenLayers.Icon("./New20x20/" + "tower.gif");  
         }
         // , size, offset);
         var newLoc = transformCoords(currentVehicle.Longitude, currentVehicle.Latitude);
         // transformCoords(currentVehicle.lon, currentVehicle.lat);
         if (fleet.length == 1)
         {
            map.setCenter(newLoc, 14);
         }
         else
         {
            bounds.extend(newLoc);
         }
         var marker = new OpenLayers.Marker(newLoc, vicon);

         var eventObj = new Object();
         eventObj.icon = vicon;
         eventObj.location = newLoc;
         // eventObj.fleet = currentVehicle;
         eventObj.featureHTML = "<h2>" + currentVehicle.Description + "</h2>" +
         // + currentVehicle.desc + "</h2>" +
         // currentVehicle.addr +
         currentVehicle.StreetAddress +
         "<br />Time: " + currentVehicle.convertedDisplayDate +
         // + currentVehicle.date +
         "<br />Heading: " + currentVehicle.MyHeading +
         // + currentVehicle.head +
         "<br />Speed: " + currentVehicle.CustomSpeed +
         // + currentVehicle.spd +
         "<br />Status: " + currentVehicle.VehicleStatus;
         // + currentVehicle.stat;
         marker.events.register('mousedown', eventObj, showVehiclePopups);
         // marker.events.register('mouseover', eventObj, showVehiclePopups);
         // marker.events.register('mouseout', eventObj, closeVehiclePopups);
         // var markerObject = new Object();
         // markerObject.id = currentVehicle.id;
         // markerObject.marker = marker;
         currentMarkers[currentMarkerIndex] = marker;
         currentMarkerIndex ++ ;
         markers.addMarker(marker);
      }
      map.addLayer(markers);
      if (fleet.length > 1)
      {
         // var newMapCenter = bounds.getCenterLonLat();
         map.zoomToExtent(bounds, true);
         map.setCenter(bounds.getCenterLonLat());
      }
   }
}

function removeMapWindow()
{
   var winparent = window.opener;
   if (winparent == null)
   {
      winparent = window.parent;
   }
   if (winparent != null)
   {
      if (myWinID && myWinID != "")
      {
         winparent.removeMapWindow(myWinID);
      }
   }
}

function transformCoords(lon, lat)
{
   return new OpenLayers.LonLat(lon, lat)
   .transform(
   new OpenLayers.Projection("EPSG:4326"),
   map.getProjectionObject()
   );
}

function getPixelValues(lon, lat)
{
   var latLong = new OpenLayers.LonLat(lon, lat).transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
   return new OpenLayers.Pixel(latLong.lon, latLong.lat);
}

function ResetTimer()
{
   finishedLastTimer = false;
   CheckAndUpdateMap();
}

var timers;


function sortNumber(a, b)
{
   return a - b;
}

function GetVehicleInfo(BoxID, Icon, IsCustomeIcon) {
    try {
        $.ajax({
            type: 'POST',
            url: rootpath + 'Vehicles.aspx?QueryType=getBoxInfo&BoxId=' + BoxID,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {
                UpdateAssetLocation(msg, Icon, IsCustomeIcon);
            },
            error: function (msg) {
                //alert('failed to fetch the data.');
            }
        });
    }
    catch (err) {
        //alert(err);
    }
}

function UpdateAssetLocation(asset, icon, custom)
{
   try
   {
      if (asset != null && asset != "")
      {
         var i = 0, j = 0;
         var bounds = new OpenLayers.Bounds();
        
        try
        {
            currentVehicle = asset;
            var newLoc = transformCoords(currentVehicle.Longitude, currentVehicle.Latitude);

            var vicon;
            
            if (custom)
                vicon = new OpenLayers.Icon("./images/CustomIcon/" + icon);
            else
                vicon = new OpenLayers.Icon("./New20x20/" + icon);
            
            if(currentVehicle.BoxId=='27492') {
                vicon = new OpenLayers.Icon("./New20x20/" + "tower.gif");  
            }

            //if (asset.length == 1) {
            map.setCenter(newLoc, 14);
                //map.zoomToMaxExtent();
            //}
            //else
            //{
            //    bounds.extend(newLoc);
            //}

            // currentMarkers[currentMarkerIndex] = marker;
            // currentMarkerIndex ++ ;

            markers.removeMarker(currentMarkers[j]);
            // markers.markers[j].destroy();
            var marker = new OpenLayers.Marker(newLoc, vicon);

            var eventObj = new Object();
            eventObj.icon = vicon;
            eventObj.location = newLoc;
            var dTime = new Date(currentVehicle.OriginDateTime);
            //alert(userdateformat);
            // eventObj.asset = currentVehicle; 
            // eventObj.asset = currentVehicle;
            eventObj.featureHTML = "<h2>" + currentVehicle.Description + "</h2>" +
            // + currentVehicle.desc + "</h2>" +
            // currentVehicle.addr +
            "Driver: " + currentVehicle.Driver +
            "<br />" + currentVehicle.StreetAddress +
            "<br />Time: " + (Ext.Date.format(dTime, userdateformat)) +
            // + currentVehicle.date +
            "<br />Heading: " + currentVehicle.MyHeading +
            // + currentVehicle.head +
            "<br />Speed: " + currentVehicle.CustomSpeed +
            // + currentVehicle.spd +
            "<br />Status: " + currentVehicle.VehicleStatus;
            // + currentVehicle.stat;
            //                  eventObj.featureHTML = "<h2>" + currentVehicle.desc + "</h2>" +
            //                  currentVehicle.addr +
            //                  "<br />Time: " + currentVehicle.date +
            //                  "<br />Heading: " + currentVehicle.head +
            //                  "<br />Speed: " + currentVehicle.spd +
            //                  "<br />Status: " + currentVehicle.stat;
            marker.events.register('mousedown', eventObj, showVehiclePopups);
            // marker.events.register('mouseover', eventObj, showVehiclePopups);
            // marker.events.register('mouseout', eventObj, closeVehiclePopups);
            markers.addMarker(marker);
            currentMarkers[j] = marker;
            // markers.markers[j] = marker;
            allVehicles[j] = currentVehicle;
        }
        catch (err)
        {
            //var test = err.Message;
            //alert(err);
        }
                  
              
         closeVehiclePopups();
         if (asset.length > 1)
         {
            map.zoomToExtent(bounds, true);
            map.setCenter(bounds.getCenterLonLat());
         }
      }
   }
   catch(err)
   {
       alert("error in UpdateAssetLocation " + err.message);
   }
}
