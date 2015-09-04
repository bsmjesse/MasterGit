<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OpenLayerMap.aspx.cs" Inherits="SentinelFM.OpenLayerMap" %>

<html>
  <head>
    <title></title>
    <!--<link rel="stylesheet" href="openlayers/theme/default/style.css" type="text/css" />-->

       <link rel="stylesheet" href="Openlayers/OpenLayers-2.13.1/theme/default/style.css?v=20140812" type="text/css">
        <link rel="stylesheet" href="maps/style.css" type="text/css">
        <link rel="stylesheet" type="text/css" href="sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
        <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />

    <% if (sn.SelectedLanguage.Contains("fr"))
       { %>
    <script type="text/javascript" src="Openlayers/OpenLayers-2.11/lib/OpenLayers/Lang/fr.js"></script>
    <% } %>
    

    <style type="text/css">
          html, body {
              height: 100%;
              margin:0px 0px;
          }
          #map {
              width: 100%;
              height: 100%;
              border: 1px solid black;
          }
          
          .olControlAttribution {
            bottom: 1.5em !important;
          }
    </style>
      <script type="text/javascript">
          var orgid = '<%=sn.User.OrganizationId %>';
          var interval = '<%=sn.User.GeneralRefreshFrequency %>';
          var userDate ='<%=sn.User.DateFormat %>';
          var userTime ='<%=sn.User.TimeFormat %>';
      </script>

      <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
      <script type="text/javascript" src="sencha/extjs-4.1.0/bootstrap.js"></script>
      <script type="text/javascript" src="Openlayers/OpenLayers-2.11/OpenLayers.js"></script>
      <script src="./Scripts/OpenLayerMap/OpenLayerMap_tracker.js?v=18"></script>
      <script type="text/javascript" src="Openlayers/navteqlayer.js"></script>
      <script src="./openlayers/lib/OpenLayers/Layer/ArcGISCache.js" type="text/javascript"></script>
      <script src="./Scripts/OpenLayerMap/utils/arcgislayers.js"></script>  
      <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>   
      <script type="text/javascript" src="./Scripts/OpenLayerMap/utils/cloudmade.js"></script>
       
      <script type="text/javascript">
          var map;
          var vectors, labels;
          var CurrentBoxID;
          var CurrentIcon;
          var IsCustomeIcon;

        function initmap() {
            var maxExtent = new OpenLayers.Bounds(-20037508, -20037508, 20037508, 20037508),
            restrictedExtent = maxExtent.clone(),
            maxResolution = 126543.0339;

            <% if(sn.SelectedLanguage.Contains("fr")) { %>
            OpenLayers.Lang.setCode("fr");
            <% } %>

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
                         }),
                     new OpenLayers.Control.ScaleLine(),
                     new OpenLayers.Control.MousePosition(),
                     new OpenLayers.Control.KeyboardDefaults(),
                     new OpenLayers.Control.Attribution()
                  ]
              };

            var style = new OpenLayers.Style({
                fillColor: "#ffcc66",
                fillOpacity: "0.3",
                strokeColor: "#cc6633",
                strokeWidth: "1",
                strokeOpacity: 0.8,
                strokeDashstyle: "solid"
            });

            var labelstyle = new OpenLayers.Style({
                pointRadius: "0",
                    
                label: "${name}",
                labelAlign: "cm",
                //labelPadding: "0",
                labelBackgroundColor: "#ffffff",
                labelBorderColor: "#cfcfcf",
                labelBorderSize: "1px",
                fontColor: "black",
                fontSize: "12px",
                fontFamily: "Courier New, monospace",
                fontWeight: "bold",
                labelYOffset: "-12"
                    
            });

            var renderer = OpenLayers.Util.getParameters(window.location.href).renderer;
            renderer = (renderer) ? [renderer] : OpenLayers.Layer.Vector.prototype.renderers;

            vectors = new OpenLayers.Layer.Vector("GeoAsset", {
                styleMap: new OpenLayers.StyleMap({
                    "default": style,
                    "select": {
                        fillColor: "#8aeeef",
                        strokeColor: "#32a8a9"
                    }
                }),
                renderers: renderer, visibility: true, displayInLayerSwitcher: false
            });

            labels = new OpenLayers.Layer.Vector("GeoLabel", {
                styleMap: new OpenLayers.StyleMap({
                    "default": labelstyle
                }),
                renderers: renderer, visibility: true, displayInLayerSwitcher: false
            });

            map = new OpenLayers.Map('map', options);
            map.addLayers([vectors, labels]);

            var Navteq = new OpenLayers.Layer.Navteq("Navteq", "normal.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
            var NavteqHybrid = new OpenLayers.Layer.Navteq("Navteq Hybrid", "hybrid.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
            map.addLayers([Navteq, NavteqHybrid]);

            try {

                // markers = new OpenLayers.Layer.Markers("Vehicles");
                markers = new OpenLayers.Layer.Markers("Vehicles",
                {
                    displayInLayerSwitcher: false,
                    isBaseLayer: false
                }
                );

                size = new OpenLayers.Size(8, 16);
                offset = new OpenLayers.Pixel(-(size.w / 2), -size.h);

                if (!map.getCenter()) {
                    // map.zoomToMaxExtent();
                    map.setCenter(transformCoords(-72.872559, 46.767074), 4);
                }
                // map.addControl(new OpenLayers.Control.LayerSwitcher());
                qs();
                myWinID = qsParm["WinId"];
                // var p = parent;

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
                            if (!timer_is_on) {
                                timer_is_on = 1;
                                finishedLastTimer = true;
                                CheckAndUpdateMap();
                            }
                        }
                    }
                }
            }
            catch (err) {
                //alert(err);
            }

            if (!map.getCenter()) {
                map.zoomToMaxExtent();
                map.setCenter(transformCoords(-100.42, 43.73), 3);                
            }
    
        }
        
          function CheckAndUpdateMap()
          {
              var i = 0;

            try
                { 
                var winparent = window.opener;
                if (winparent == null)
                {
                    winparent = window.parent;
                }
                if (winparent != null && allVehicles.length > 0)
                {
                    if (CurrentBoxID == null || CurrentBoxID =='')
                    {
                        for (i = 0; i < allVehicles.length; i ++ )
                        {
                            //var updatedData = winparent.GetWinTrackData(allVehicles[i].BoxId);
                            CurrentBoxID = allVehicles[i].BoxId;
                            CurrentIcon = allVehicles[i].icon;
                            if ( allVehicles[i].ImagePath != "" &&  allVehicles[i].ImagePath != null)
                                IsCustomeIcon = true;
                            else
                                IsCustomeIcon = false;     
                        }      
                    }
                    GetVehicleInfo(CurrentBoxID, CurrentIcon, IsCustomeIcon);
                }
                     
                timers = setTimeout("CheckAndUpdateMap()", interval);
                }
                catch (err)
                {
                    //alert(err);
                    clearTimeout(timers);
                    timer_is_on = 0;
                }
          }

        function transformCoords(lon, lat) {
            return new OpenLayers.LonLat(lon, lat)
               .transform(
                   new OpenLayers.Projection("EPSG:4326"),
                   map.getProjectionObject()
               );
        }

    </script>
    
    <!--<script src="./Openlayers/OpenLayers.js"></script>-->
    
    <script type="text/javascript">
         var ISSECURE = <%=ISSECURE.ToString().ToLower() %>;         
    </script>
  </head>
  <body onload="initmap()">
    <div id="map" class="largemap"></div>
  </body>
</html>
