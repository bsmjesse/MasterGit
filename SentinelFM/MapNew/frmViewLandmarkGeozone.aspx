<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmViewLandmarkGeozone.aspx.cs" Inherits="SentinelFM.MapNew_frmViewLandmarkGeozone" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="../openlayers/theme/default/style.css" type="text/css" />
    
    <script type="text/javascript" src="../Openlayers/OpenLayers-2.11/OpenLayers.js"></script>
    <script type="text/javascript" src="../Openlayers/navteqlayer.js"></script>

    <% if (sn.SelectedLanguage.Contains("fr"))
       { %>
    <script type="text/javascript" src="../Openlayers/OpenLayers-2.11/lib/OpenLayers/Lang/fr.js"></script>
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
        var map;
        var vectors, labels;
        var DRAW_OBJECT = <%=DRAW_OBJECT %>;

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
            
            drawObject();

            if (!map.getCenter()) {
                map.zoomToMaxExtent();
                map.setCenter(transformCoords(-100.42, 43.73), 3);                
            }

            /*var showpopup = function(e) {
                
                try {
                    
                    var position = e.feature.geometry.getBounds().getCenterLonLat();
                    //position = e.feature.attributes.location;
                    var popupcontent = e.feature.attributes.type + ': ' + e.feature.attributes.name;
                    if(e.feature.attributes.radius > 0) popupcontent += '<br />Radius: ' + e.feature.attributes.radius;
                    var popup = new OpenLayers.Popup.FramedCloud("featurePopup", position,
                                    new OpenLayers.Size(100, 100),
                                    popupcontent,
                                    null, true);
    

                    map.addPopup(popup, true);

                    
                }
                catch(err){}
            };

            var report = function(e) {}
            
            var highlightCtrl = new OpenLayers.Control.SelectFeature(vectors, {
                hover: true,
                highlightOnly: true,
                renderIntent: "temporary",
                eventListeners: {
                    beforefeaturehighlighted: showpopup,
                    featurehighlighted: report,
                    featureunhighlighted: report
                }
            });

            map.addControl(highlightCtrl);
            highlightCtrl.activate();
            */

            
        }

        function transformCoords(lon, lat) {
            return new OpenLayers.LonLat(lon, lat)
               .transform(
                   new OpenLayers.Projection("EPSG:4326"),
                   map.getProjectionObject()
               );
        }

        function drawObject()
        {
            
            var proj = new OpenLayers.Projection("EPSG:4326");
            var in_options = {
                'internalProjection': map.baseLayer.projection,
                'externalProjection': proj
            };
            var feature;
            
            if (DRAW_OBJECT.radius > 0) {
                var point = new OpenLayers.LonLat(DRAW_OBJECT.lon, DRAW_OBJECT.lat);
                point.transform(proj, map.getProjectionObject());
                var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
                var circle = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, DRAW_OBJECT.radius / Math.cos(point.lat * (Math.PI / 180)), 40, 0);
                feature = new OpenLayers.Feature.Vector(circle);
            }
            else {
                try {
                    coords = DRAW_OBJECT.coords;                    
                    feature = new OpenLayers.Format.WKT(in_options).read(coords);
                }
                catch (err) { }
            }
            if (feature) {                
                
                var x, y;
                var maxX = 0;
                var maxY = 0;
                var minX = 0;
                var minY = 0;
                
                for(var i=0;i<feature.geometry.getVertices().length;i++)
                {
                    x = feature.geometry.getVertices()[i].x;
                    y = feature.geometry.getVertices()[i].y;
                    
                    if(minX == 0 || x < minX)
                        minX = x;
                    if(maxY == 0 || y < minY)
                        minY = y;
                    if(maxX == 0 || x > maxX)
                        maxX = x;
                    if(maxY == 0 || y > maxY)
                        maxY = y;
                }

                //alert(minX + ', ' + minY + ', ' + maxX + ', ' + maxY);

                var point = new OpenLayers.Geometry.Point((minX + maxX) / 2, minY);
                var label = new OpenLayers.Feature.Vector(point);

                feature.attributes = {
                    name: DRAW_OBJECT.desc,
                    type: DRAW_OBJECT.type,
                    radius: DRAW_OBJECT.radius,
                    location: new OpenLayers.LonLat((minX + maxX) / 2, minY)
                };

                label.attributes = {
                    name: DRAW_OBJECT.desc
                };

                vectors.removeAllFeatures();
                vectors.addFeatures([feature]);

                labels.removeAllFeatures();
                labels.addFeatures([label]);

                map.zoomToExtent(feature.geometry.getBounds(), true);
                var currentZoom = map.getZoom();
                map.zoomTo(currentZoom - 1);
            }
        }
        
        
    </script>
</head>
<body onload="initmap()">
    <form id="form1" runat="server">
    <div>
        <table border="0" cellpadding="0" cellspacing="0" style="width:100%; height:100%">
            <tr valign="top">
                <td id="frmVehicleMapfrmVehicleMap"  style="width:800px; height:500px"   >
                    <div id="map"></div>
                </td>
            </tr>
            <tr align="left">
                <td>
                    <span style="margin-left: 5px; font-family: verdana; font-size: 11px;"><%=CAPTION%></span>
                </td>
            </tr>
            <tr align="center">
                <td>
                    <br />
                    <asp:Button ID="cmdCLoseWindow"  class="combutton" OnClientClick="window.close()"   runat="server" Text="Close1" meta:resourcekey="cmdCLoseWindowResource1" />                                   
                </td>
            </tr>
        </table>    
    </div>
    </form>
</body>
</html>
