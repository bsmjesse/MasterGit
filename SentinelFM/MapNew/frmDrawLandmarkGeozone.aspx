<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmDrawLandmarkGeozone.aspx.cs" Inherits="SentinelFM.MapNew_frmDrawLandmarkGeozone" meta:resourcekey="PageResource1" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="../openlayers/theme/default/style.css" type="text/css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />

    <link rel="stylesheet" href="//code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="../Openlayers/OpenLayers-2.11/OpenLayers.js"></script>
    <% if (sn.SelectedLanguage.Contains("fr"))
       { %>
    <script type="text/javascript" src="../Openlayers/OpenLayers-2.11/lib/OpenLayers/Lang/fr.js"></script>
    <% } %>
    <script type="text/javascript" src="../Openlayers/navteqlayer.js"></script>
    
    <%--<script type="text/javascript" src="../Scripts/jquery-1.4.1.js" language="javascript"></script>--%>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="//code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <script type="text/javascript" src="../sencha/extjs-4.1.0/bootstrap.js"></script>
    <script type="text/javascript" src="//maps.googleapis.com/maps/api/js?v=3&client=gme-bsmwirelessinc&sensor=true&libraries=places"></script>

    <script type="text/javascript">
        var tooltip_Pan_Map = "<%=tooltip_Pan_Map %>";
        var tooltip_Add_a_landmark_Circle = "<%=tooltip_Add_a_landmark_Circle %>";
        var tooltip_Draw_a_Polygon = "<%=tooltip_Draw_a_Polygon %>";
        var tooltip_Modify_feature = "<%=tooltip_Modify_feature %>";
        var MapSearchData = "<%=MapSearchData %>";
    </script>

    <script type="text/javascript" src="mapmenus.js?v=2013101701"></script>
      

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
            bottom: 0.5em !important;
        }
          
        .olbsmtoolbar
        {
            border: 0; z-index: 1000; position: absolute; top: 10px; left: 65px; width: <%= (OBJECT_TYPE=="Geozone"? "72": "95") %>px;
        }
        
        #toolbar .toolbar-transparent
        {
            border:0;
            background-color: #B5B2AE !important;
            background-image: none;                        
        }
        
        .combutton {
            background-color: #F5F5F5;
            border-color: #808080;
            border-width: 1px;
            color: #000000;
            font-size: 11px;
            padding: 1px 5px;
            text-decoration: none;
            width: 105px;
        }
        
        .maptoolbarselected {
            background: none repeat scroll 0 0 #FFFFFF !important;
        }
        
        .message
        {
            background: none repeat scroll 0 0 #F9EDBE;
            border: 1px solid #F0C36D !important;
            border-radius: 2px 2px 2px 2px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
            font-size: 95%;
            line-height: 29px;
            padding-left: 16px;
            padding-right: 16px;
        }
        
        .maptoolbarcircletoggle
        {
            <%= (OBJECT_TYPE=="Geozone"? "display:none;": "") %>
        }

        .mapsearchtoolbar
        {
            border: 0; z-index: 1000; position: absolute; top: 8px; left: <%= (OBJECT_TYPE=="Geozone"? "150": "175") %>px; width: 500px; height: 35px;
        }
        .mapsearchtoolbar input
        {
            margin-top: 1px; margin-left: 5px;           
        }

        #searchAddressBtnW 
        {
            display: inline;
            margin: 1px 1px 0 0;
            vertical-align: top;
            padding: 0;
        }
        
        .searchAddressBtn 
        {
            background-color: #4A8BF5;
            /*background-image: -moz-linear-gradient(center top , #4D90FE, #4787ED);*/
            border: 1px solid #3079ED;
            color: #FFFFFF !important;
            margin: 0;
            -moz-user-select: none;
            border-radius: 2px 2px 2px 2px;
            cursor: default !important;
            display: inline-block;
            font-weight: bold;
            height: 29px;
            line-height: 29px;
            min-width: 54px;
            padding: 0 8px;
            text-align: center;
            text-decoration: none !important;
        }
        
        /*.searchAddressBtn:focus {
            border: 1px solid #4D90FE;
            box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.5) inset;
            outline: medium none;
        }*/
        
        .searchAddressBtn:hover {
            background-color: #3F83F1;
            /*background-image: -moz-linear-gradient(center top , #4D90FE, #357AE8);*/
        }
        
        .searchAddressBtnI 
        {
            display: inline-block;
            height: 13px;
            margin: 7px 19px;
            width: 14px;
            background-image: url("../images/search.png");
            background-repeat: no-repeat;
        }

        .ui-autocomplete {z-index: 9999 !important;}
        .ui-autocomplete-loading {
            background: white url('http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.2/themes/smoothness/images/ui-anim_basic_16x16.gif') right center no-repeat;
        }

        .bsmforminput
        {
            border: 1px solid #809DB9;
            padding: 3px;
        }

        .bsmforminput:focus
        {
            border: 2px solid #DDBB77;
        }

        .kd-button {
            -moz-transition: all 0.218s ease 0s;
            background-color: #F5F5F5;
            background-image: -moz-linear-gradient(center top , #F5F5F5, #F1F1F1);
            /*border: 1px solid rgba(0, 0, 0, 0.1);*/
            border: 1px solid #D8D8D8;
            border-radius: 2px 2px 2px 2px;
    
            color: #444444;
            display: inline-block;
            font-size: 100%;
            font-family: arial,​sans-serif;
            font-weight: bold;
            height: 27px;
            line-height: 27px;
            min-width: 54px;
            padding: 0 8px;
            text-align: center;
        }

        .kd-button:hover {
            -moz-transition: all 0s ease 0s;
            background-color: #F8F8F8;
            background-image: -moz-linear-gradient(center top , #F8F8F8, #F1F1F1);
            border: 1px solid #C6C6C6;
            box-shadow: 0 1px 1px rgba(0, 0, 0, 0.1);
            color: #333333;
            text-decoration: none;
        }

        .kd-button-disabled, .kd-button-disabled:hover {
            -moz-transition: all 0.218s ease 0s !important;
            background-color: #F5F5F5 !important;
            background-image: -moz-linear-gradient(center top , #F5F5F5, #F1F1F1) !important;
            border: 1px solid #D8D8D8 !important;
            border-radius: 2px 2px 2px 2px !important;
            color: #cccccc !important;
            font-weight: normal !important;
            box-shadow: none !important;        
        }
    </style>

    <script type="text/javascript">
        var map;
        var vectors, labels;
        var mycontrols, newfeature;
        var undolist = [];
        var changeslist = [];
        var alwaysnew = false;

        var DRAW_OBJECT =  <%=DRAW_OBJECT %>;
        var searchMarker;
        var searchLat;
        var searchLon;
        
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
                     //new OpenLayers.Control.MousePosition(),
                     //new OpenLayers.Control.KeyboardDefaults(),
                     new OpenLayers.Control.Attribution()
                 ]
              };

            var renderer = OpenLayers.Util.getParameters(window.location.href).renderer;
            renderer = (renderer) ? [renderer] : OpenLayers.Layer.Vector.prototype.renderers;

            var style = new OpenLayers.Style({
                    pointRadius: "4",
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

            var markersStyle = new OpenLayers.Style({
                externalGraphic: "../New20x20/RedCircle.ico",
                graphicWidth: "20",
                graphicHeight: "20"                
            });

            markers = new OpenLayers.Layer.Vector("Markers", {
                displayInLayerSwitcher: true,
                isBaseLayer: false,
                styleMap: new OpenLayers.StyleMap({
                    "default": markersStyle
                }),
                renderers: renderer, visibility: true

            });

            map = new OpenLayers.Map('map', options);
            map.addLayers([vectors, labels, markers]);
            map.addLayer(searchMarker);

            var markerClickControl = new OpenLayers.Control.SelectFeature(
                [searchMarker],
                {
                    onSelect: onMarkerClick,
                    autoActivate: true,
                    toggle: true,
                    clickout: false
                }
            );

            /*markerClickControl.handlers['feature'].stopDown = false;
            markerClickControl.handlers['feature'].stopUp = false;
            markerClickControl.handlers['feature'].stopClick = false;

            map.addControl(markerClickControl);*/
            
            function onMarkerClick(feature) {
                
                var position = new OpenLayers.LonLat(feature.geometry.x, feature.geometry.y);
                var content = "<div style='font-size:12px;margin:10px;'>" + feature.attributes.address + "</div>";
                //content += "<div style='margin-top: 20px;'><a href='javascript:void(0)' onclick='parent.getClosestVehicles(" + feature.attributes.lon + "," + feature.attributes.lat + ");map.removePopup(searchMarkerPopup);'>Closest Vehicles</a> &nbsp; <a href='javascript:void(0)'>History</a></div>";
                
    
                searchMarkerPopup = new OpenLayers.Popup.FramedCloud("addressMarkerPopup", position,
                                            new OpenLayers.Size(100, 50),
                                            content,
                                            null, true, null);

                searchMarkerPopup.autoSize = true;   
                markerClickControl.unselect(feature);
                
                map.addPopup(searchMarkerPopup, true);                
                
            }

            /*
                Map Drawing Controls

            */

            var onFeatureSelect = function (feature) {
                if (feature.attributes && feature.attributes.markerType && feature.attributes.markerType == 'searchMarker')
                {
                    //alert(feature.attributes.address);
                    onMarkerClick(feature);
                    return;
                }
            };
            var onFeatureUnselect = function (feature) { };

            //selectControl = new OpenLayers.Control.SelectFeature([vectors,searchMarker], { onSelect: onFeatureSelect, onUnselect: onFeatureUnselect });
            selectControl = new OpenLayers.Control.SelectFeature(
                [vectors, searchMarker],
                {
                    onSelect: onMarkerClick,
                    autoActivate: true,
                    toggle: true,
                    clickout: false
                }
            );

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

            mycontrols.modify.mode = OpenLayers.Control.ModifyFeature.RESHAPE;

            document.getElementById('noneToggle').checked = true;

            searchMarker.events.fallThrough = true;
            vectors.events.fallThrough = true;

            /* End of Map Drawing Controls*/

            var Navteq = new OpenLayers.Layer.Navteq("Navteq", "normal.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
            var NavteqHybrid = new OpenLayers.Layer.Navteq("Navteq Hybrid", "hybrid.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
            map.addLayers([Navteq, NavteqHybrid]);

            drawObject();

            vectors.events.on({
                "beforefeaturemodified": beforeModified,
                "featuremodified": modified,
                "beforefeatureselected": beforefeatureselected,
                
                //"afterfeaturemodified": report,
                //"vertexmodified": report,
                //"sketchmodified": report,
                //"sketchstarted": onSketchstarted,
                "sketchcomplete": onSketchcomplete
            });

            if (!map.getCenter()) {
                map.zoomToMaxExtent();
                map.setCenter(transformCoords(-100.42, 43.73), 3);                
            }
            
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
            else if(MapSearchData != '')
            {
                
                var _ps = MapSearchData.split(",");
                if(_ps.length > 1)
                {

                    var markerpoint = new OpenLayers.LonLat(_ps[1], _ps[0]);
                    markerpoint.transform(proj, map.getProjectionObject());

                    var marker = new OpenLayers.Feature.Vector(new OpenLayers.Geometry.Point(markerpoint.lon, markerpoint.lat));

                    markers.removeAllFeatures();
                    markers.addFeatures([marker]);
                    map.setCenter(markerpoint, 16);
                }
            }
        }

        function toggleControl(element) {
            for (key in mycontrols) {
                var control = mycontrols[key];
                if (element.value == key && element.checked) {
                    control.activate();
                } else {
                    control.deactivate();
                }
            }
            if (element.value == 'none') {
                selectControl.activate();                
            }
        }

        function onSketchstarted(event) {
            
        }

        function onSketchcomplete(event) {
//            if (newfeature)
//                vectors.removeFeatures(newfeature);
            vectors.removeAllFeatures();

            newfeature = event.feature;

            if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {
                
                var pointSets = getPointSets(event.feature);
                savePoints(event.feature, pointSets);

                var r = -1;
                if (document.getElementById('circleToggle').checked == true) 
                {
                    //r = 0.565352 * Math.sqrt(event.feature.geometry.getArea());                
                    r = 0.565352 * Math.sqrt(event.feature.geometry.getGeodesicArea(new OpenLayers.Projection("EPSG:900913")));
                }
                
                event.feature.attributes = {
                    name: "newobjectfromscratch",
                    type: DRAW_OBJECT.type,
                    radius: r
                };

                alwaysnew = true;
                $('#undobar').hide();
                undolist = [];
            }
        }

        function getPointSets(feature)
        {
            var pointSets = "";
            var c = feature.geometry.components[0];
            
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

            return pointSets;
        }

        function savePoints(feature, pointSets)
        {
            var latlon = feature.geometry.getBounds().getCenterLonLat();
            latlon.transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));

            $('#<%=latitude.ClientID %>').val(latlon.lat);
            $('#<%=longitude.ClientID %>').val(latlon.lon);

            //alert(pointSets);
            $('#<%=pointSets.ClientID %>').val(pointSets);
            if (document.getElementById('circleToggle').checked == true) 
            {
                $('#<%=isCircle.ClientID %>').val("1");
                $('#<%=radius.ClientID %>').val(0.565352 * Math.sqrt(feature.geometry.getGeodesicArea(new OpenLayers.Projection("EPSG:900913"))));                
            }
            else
            {
                $('#<%=isCircle.ClientID %>').val("0");                
            }

            $('#<%=cmdSave.ClientID %>').show();
        }

        function undopolygon() {

            if (undolist.length == 0) {
                $('#undobar').hide();
                $('#<%=cmdSave.ClientID %>').hide();
                return;
            }
            
            var lastdo = undolist[undolist.length - 1];
    
            for (var i = 0; i < vectors.features.length; i++) {
                var feature = vectors.features[i];
    
                if (encodeURI(feature.attributes.name) == encodeURI(lastdo.name)) {
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

                    var modifiedfeature = new OpenLayers.Feature.Vector(
                                                OpenLayers.Geometry.fromWKT(polygonstring)
                                         );
                    //var fid = feature.fid;
                    //newfeature.fid = fid;

                    modifiedfeature.attributes = {
                        name: lastdo.name,
                        type: DRAW_OBJECT.type,
                        radius: -1
                    };

                    vectors.addFeatures([modifiedfeature]);

                    savePoints(modifiedfeature, decodeURI(lastdo.pointSets)); 

                    //geoLandmarkFeatures.push(newfeature);
                    

                    undolist.splice(undolist.length - 1, 1);
                    $('#undonum').html(undolist.length);

                    

                    break;
                }
            }
            

            if (undolist.length == 0) {
                $('#undobar').hide();
                
                if(!alwaysnew) 
                {
                    $('#<%=cmdSave.ClientID %>').hide();                    
                }
            }
        }

        /*function savechanges() {
            $('#savechanges').hide();
            $('#undolink').hide();
            $('#undosaving').show();
            $('#noneToggle').click();

            for (var i = 0; i < changeslist.length; i++) {
                editPolygon(changeslist[i].otype, changeslist[i].oid, changeslist[i].pointSets);
            }
            changeslist = [];
            undolist = [];
            $('#undobar').hide();
            $('#messagebar').html("The change(s) has been saved.").show().delay(2000).fadeOut(1000);
        }*/

        function cancelchanges() {
            if (undolist.length == 0) {
                $('#undobar').hide();
                if(!alwaysnew) $('#<%=cmdSave.ClientID %>').hide();
                return;
            }

            while (undolist.length > 0) {
                undopolygon();
            }    

            $('#undobar').hide();

            if(!alwaysnew) 
            {
                $('#<%=cmdSave.ClientID %>').hide();
            }
        }

        //function pushToUndolist(otype, oid, pointSets) {
        function pushToUndolist(name, pointSets) {
            //var item = { otype: otype, oid: oid, pointSets: previousPointSets };
            var item = { name: name, pointSets: previousPointSets };
            undolist.push(item);
            if (undolist.length > 0) {
                $('#undonum').html(undolist.length);
                $('#undobar').show();
            }

            var itemForSave = { name: name, pointSets: pointSets };

            var addedtochangeslist = false;
            for (var i = 0; i < changeslist.length; i++) {
                //if (changeslist[i].otype == otype && changeslist[i].oid == oid) {
                //    changeslist[i].pointSets = pointSets;
                    addedtochangeslist = true;
                //}
            }
            if (!addedtochangeslist)
                changeslist.push(itemForSave);
        }

        function beforefeatureselected(event) {
            if (event.feature.attributes.radius * 1 > 0 && document.getElementById("modifyToggle").checked == true) {
                $('#messagebar').html("Could not modify circle, please re-draw it.").show().delay(2000).fadeOut(1000);
                return false;
            }            
        }

        function beforeModified(event) {
            if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {

                //if (event.feature.fid.split(":::")[0] == "LandmarkCircle") {
                //    return;
                //}
                if(event.feature.attributes.radius * 1 > 0)
                    return;
                previousPointSets = encodeURI(getPointSets(event.feature));
            }
        }

        function modified(event) {
            if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {
                
                var pointSets = getPointSets(event.feature);
                savePoints(event.feature, pointSets);                

                pointSets = encodeURI(pointSets);

                $('#undosaving').hide();
                $('#undolink').show();
                $('#savechanges').show();
                pushToUndolist(event.feature.attributes.name, pointSets);
                previousPointSets = pointSets;
                $('#undobar').show();                
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
        }

        function gotoPointByReference(reference) {
            $.ajax({
                url: "../Widgets/GeoService.aspx?reference=" + reference + "&language=en&action=getCoord",
                dataType: "json",
                type: "GET",
                success: function (data) {                    
                    searchLat = data.result.geometry.location.lat * 1;
                    searchLon = data.result.geometry.location.lng * 1;
                    gotoSearchPoint(searchLon, searchLat);
                },
                error: function (error) {
                    console.log(error);
                }
            });
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

        $( document ).ready(function() {
            <%=JAVASCRIPT %>
            $('#txtSearchAddress').autocomplete({
                source: function (request, response) {
                    searchLat = 0;
                    searchLon = 0;
                
                    $.ajax({
                        url: "../Widgets/GeoService.aspx?input=" + request.term + "&language=en&action=autocomplete&geoserver=&layernames=GoogleAddress",                    
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
        });
        
    </script>

    
</head>
<body onload="initmap()">
    <form id="form1" runat="server">
    <asp:HiddenField ID="pointSets" runat="server" />
    <asp:HiddenField ID="isCircle" runat="server" />
    <asp:HiddenField ID="radius" runat="server" Value="-1" />
    <asp:HiddenField ID="latitude" runat="server" Value="0" />
    <asp:HiddenField ID="longitude" runat="server" Value="0" />
    <div id="toolbar" class="olbsmtoolbar"></div>
    <div id="mapsearchtoolbar" class="mapsearchtoolbar"><input type="text" style="width:300px;height:26px;" class="formtext bsmforminput" id="txtSearchAddress" name="txtSearchAddress" placeholder="" />
        <div id="searchAddressBtnW">
            <button id="searchAddressBtn" class="searchAddressBtn" onclick="searchAddress();">
                <span class="searchAddressBtnI"></span>
            </button>
        </div>
    </div>
    <div>
        <table border="0" cellpadding="0" cellspacing="0" style="width:100%; height:100%">
            <tr valign="top">
                <td id="frmVehicleMapfrmVehicleMap"  style="width:800px; height:500px"   >
                    <div id="map"></div>
                </td>
            </tr>
            <tr align="left">
                <td>
                    <span style="margin-left: 5px; font-family: verdana; font-size: 11px;"></span>
                </td>
            </tr>
            <tr align="center">
                <td>
                    <br />
                    <asp:Button ID="cmdCLoseWindow"  class="combutton" OnClientClick="window.close()"   runat="server" Text="Close" meta:resourcekey="cmdCLoseWindowResource1" /> 
                    <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" style="display:none;" meta:resourcekey="cmdSaveResource1" onclick="cmdSave_Click">
        </asp:Button>                                  
                </td>
            </tr>
        </table>    
    </div>

    <div id="undobar" style="border: 0; z-index: 1000; position: absolute; top: 40px; left: 65px; width: 300px; display:none" class="message">
        <asp:Label ID="Label1" runat="server" Text="The shape has been changed. " 
            meta:resourcekey="Label1Resource1"></asp:Label><span id="undolink"><a href="javascript:void(0)" onclick="undopolygon();">
        <asp:Label ID="Label2" runat="server" Text="Undo" 
            meta:resourcekey="Label2Resource1"></asp:Label></a></span> (<span id="undonum">1</span>)<span id="undosaving" style="margin-left:10px;">Saving...</span><span id="savechanges"> <a href="javascript:void(0)" onclick="cancelchanges();">
        <asp:Label ID="Label3" runat="server" Text="Cancel" 
            meta:resourcekey="Label3Resource1"></asp:Label></a></span></div>
    <div id="messagebar" style="border: 0; z-index: 1000; position: absolute; top: 10px; left: 220px; width: 300px; display:none" class="message"></div>

    <div id="controls" style="display:none">
        <ul id="controlToggle">
            <li>
                <input type="radio" name="type" value="none" id="noneToggle" onclick="toggleControl(this);" checked="checked" />
                <label for="noneToggle">navigate</label>
            </li>
            <li>
                <input type="radio" name="type" value="select" id="selectToggle" onclick="toggleControl(this);" />
                <label for="selectToggle">select polygon on click</label>
            </li>            
            <li>
                <input type="radio" name="type" value="circle" id="circleToggle" onclick="toggleControl(this);" />
                <label for="pointToggle">draw circle</label>
            </li>
            <li>
                <input type="radio" name="type" value="polygon" id="polygonToggle" onclick="toggleControl(this);" />
                <label for="polygonToggle">draw polygon</label>
            </li>
            <li>
                <input type="radio" name="type" value="modify" id="modifyToggle" onclick="toggleControl(this);" />
                <label for="modifyToggle">modify feature</label>                
            </li>           
        </ul>
    </div>

    </form>
</body>
</html>