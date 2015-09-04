

// telogis API classes
var Size = Telogis.GeoBase.Size;
var DistanceUnit = Telogis.GeoBase.DistanceUnit;
var Map = Telogis.GeoBase.Widgets.Map;
var Dock = Telogis.GeoBase.Widgets.Dock;
var DockSkin = Telogis.GeoBase.Widgets.DockSkin;
var Scale = Telogis.GeoBase.Widgets.Scale;
var Slider = Telogis.GeoBase.Widgets.Slider;
var SliderSkin = Telogis.GeoBase.Widgets.SliderSkin;
var Point = Telogis.GeoBase.Point;
var MapControl = Telogis.GeoBase.Widgets.MapControl;
var ObjectLayer = Telogis.GeoBase.MapLayers.ObjectLayer;
var TileLayer = Telogis.GeoBase.MapLayers.TileLayer;
var RouteLayer = Telogis.GeoBase.MapLayers.RouteLayer;
var ImageObject = Telogis.GeoBase.MapLayers.ImageObject;
var LatLon = Telogis.GeoBase.LatLon;
var Balloon = Telogis.GeoBase.MapLayers.Balloon;
var BalloonSkin = Telogis.GeoBase.MapLayers.BalloonSkin;
var MathUtil = Telogis.GeoBase.MathUtil;
var BoundingBox = Telogis.GeoBase.BoundingBox;
var Color = Telogis.GeoBase.Color;
var DivObject = Telogis.GeoBase.MapLayers.DivObject;
var PolygonFence = Telogis.GeoBase.MapLayers.PolygonFence;
var Rectangle = Telogis.GeoBase.Rectangle;
var CanvasLayer = Telogis.GeoBase.MapLayers.CanvasLayer;
var RegionShadeLayer = Telogis.GeoBase.MapLayers.RegionShadeLayer;
var FenceCollection = Telogis.GeoBase.MapLayers.FenceCollection;
var CircleFence = Telogis.GeoBase.MapLayers.CircleFence;
var Canvas = Telogis.GeoBase.Canvas;
var isUpdateCanvas = true;
var isUpdateGeozoneCanvas = true;
var isUpdateLandmarkCanvas = true;
var IsFrenchCulture = false;

var MapLandMarkText = "";
var MapLandMarkRadiusText = "";
if (IsFrenchCulture) {
    MapLandMarkText = Dictionary.landmark_fr;
    MapLandMarkRadiusText = Dictionary.radius_fr;
}
else {
    MapLandMarkText = Dictionary.landmark;
    MapLandMarkRadiusText = Dictionary.radius;
}

var CircleMarkerLayer = CanvasLayer.define(
                function (canvas) {
                    try {
                        if (!this._circles) this._circles = [];
                        for (var i = 0; i < this._points.length; i++) {


                            if (!this._circles[i]) {

                                this._circles[i] = canvas.circle();

                                // since circle() actually returns an ellipse, we adjust radiusX and radiusY properties instead
                                // of a radius.


                                // all redraw calls:

                                // when setFillColor, setLineColor or setLineWidth is called on the layer, it merely adjusts the
                                // internal record of these properties. It is up to the derived draw callback (this function!) to
                                // apply these to the actual primitives, as illustrated below.

                                this._circles[i].fillColor = new Color(0x00, 0x00, 0x00, 0.2);
                                this._circles[i].lineColor = new Color(0x00, 0x00, 0x00, 0.8);
                                this._circles[i].lineWidth = 1;

                                this._circles[i].center = this.map.getXY(this._points[i]);
                                var northXY = this.map.getXY(MathUtil.displace(this._points[i], this._radius[i], 0, DistanceUnit.METERS));
                                var radius = this._circles[i].center.y - northXY.y;
                                this._circles[i].radiusX = radius;
                                this._circles[i].radiusY = radius;
                            }
                        }
                        if (isUpdateCanvas) canvas.update();
                    }
                    catch (err) {
                        var s = '';
                    }
                }
            );
CircleMarkerLayer.prototype.addPoint = function (loc) { this._points.push(loc); };
CircleMarkerLayer.prototype.addRadius = function (radius) { this._radius.push(radius); };
CircleMarkerLayer.prototype._points = [];
CircleMarkerLayer.prototype._radius = [];


var PolygonMarkerLayer = CanvasLayer.define(
                function (canvas) {
                    try {
                        if (!this._polygon) this._polygon = [];
                        for (var i = 0; i < this._points.length; i++) {
                            if (!this._polygon[i]) {
                                var coords = [];
                                //for (var coordIndex = 0; coordIndex < this._points[i].length; coordIndex++) {
                                    //try {
                                    //var lat_1 = parseFloat(this._points[i][coordIndex][0]);
                                    //var lon_1 = parseFloat(this._points[i][coordIndex][1]);
                                    //coords.push(this.map.getXY(new LatLon(lat_1, lon_1)));
                                    //}
                                    //catch (err) {
                                    //}
                                //}
                                //coords.push(this._points[i][coordIndex]);
                                this._polygon[i] = canvas.polyline(this._points[i], true);

                                this._polygon[i].fillColor = new Color(0x00, 0x00, 0x00, 0.2);
                                this._polygon[i].lineColor = new Color(0x00, 0x00, 0x00, 0.8);
                                this._polygon[i].lineWidth = 1;

                            }
                        }
                        if (isUpdateGeozoneCanvas) canvas.update();
                    }
                    catch (err) {
                    }
                }
            );
PolygonMarkerLayer.prototype.addPoint = function (locs) { this._points.push(locs); };
PolygonMarkerLayer.prototype._points = [];

var LandmarkPolygonMarkerLayer = CanvasLayer.define(
                function (canvas) {
                    try {
                        if (!this._polygon) this._polygon = [];
                        for (var i = 0; i < this._points.length; i++) {
                            if (!this._polygon[i]) {
                                var coords = [];
                                this._polygon[i] = canvas.polyline(this._points[i], true);

                                this._polygon[i].fillColor = new Color(0x00, 0x00, 0x00, 0.2);
                                this._polygon[i].lineColor = new Color(0x00, 0x00, 0x00, 0.8);
                                this._polygon[i].lineWidth = 1;

                            }
                        }
                        if (isUpdateLandmarkCanvas) canvas.update();
                    }
                    catch (err) {
                    }
                }
            );
LandmarkPolygonMarkerLayer.prototype.addPoint = function (locs) { this._points.push(locs); };
LandmarkPolygonMarkerLayer.prototype._points = [];

// Sentinel namespace
Sentinel:Sentinel = function () {

    return {

        // Sentinel.Constants namespace
        Constants: function () {
            return {
                // pan step length (in pixels)
                PAN_SENSITIVITY: 100,
                // default zoom index when mapping a single asset
                ASSET_DEFAULT_ZOOM_INDEX: 15,
                // icon mapping for asset status
                ASSET_ICONS: {
                    Moving: "images/dots/triangle.gif"
                },
                // icon mapping for landmark types
                LANDMARK_ICONS: {
                    0: "images/dots/landmarks/0.gif"
                },
                // icon mapping for geozone types
                GEOZONE_ICONS: {
                    0: "images/dots/landmarks/0.gif"
                },
                // z-indices for layers
                LAYER_ZINDEX: {
                    LANDMARK_CIRCLE: 40,
                    LANDMARK: 50,
                    GEOZONE: 40,
                    GEOZONE_LABEL: 50,
                    TRAILS: 50,
                    ASSET: 55,
                    DRAWING: 60,
                    BALLONLABEL: 70
                },
                // multiplier for calculation of the geoasset cache area
                GEOASSET_CACHE_AREA_FACTOR: 3.0,
                // threshold zoom index for hiding details
                HIGH_ZOOM_INDEX: 8,
                // event definitions for the EventDispatcher
                EVENTS: {
                    SHOW_ASSET: "SHOW_ASSET",
                    SHOW_ADDRESS: "SHOW_ADDRESS",
                    SHOW_TRIP: "SHOW_TRIP",
                    CREATE_LANDMARK: "CREATE_LANDMARK",
                    SEARCH: "SEARCH",
                    SHOW_ALARM: "SHOW_ALARM",
                    GEOZONE_EDIT_SAVED: "GEOZONE_EDIT_SAVED",
                    GEOZONE_EDIT_CANCELLED: "GEOZONE_EDIT_CANCELLED"
                },
                // geozone form factors
                GEOZONE_FORM_FACTOR: {
                    RECTANGLE: "RECTANGLE",
                    CIRCLE: "CIRCLE",
                    POLYGON: "POLYGON"
                },
                // maximum number of points per polygon
                GEOZONE_POLYGON_MAX_POINTS: 10
            };
        } (),


        Map: function (container, eventDispatcher, options) {
            ///
            /// <summary>
            ///     Creates a map and places it inside the container (DIV) in parameter.
            /// </summary>
            /// <param name="container">The container where the map will be inserted (a DIV tag).</param>
            /// <param name="eventDispatcher" type="EventDispatcher">An event dispatcher to which the map
            ///     will bind to be notified of events in the web page.</param>
            /// <param name="options">
            ///     A map that contains options to customize the behavior of the map:
            ///         1: initialPosition - A tuple containing the lat/lon coordinate to center the map on at startup
            /// </param>
            ///

            if (IsFrenchCulture) {
                MapLandMarkText = Dictionary.landmark_fr;
                MapLandMarkRadiusText = Dictionary.radius_fr;
            }
            else {
                MapLandMarkText = Dictionary.landmark;
                MapLandMarkRadiusText = Dictionary.radius;
            }

            var local = this; // create local reference usable in closures

            // setup initial size and automatic resize callbacks
            this._getContainerSize = function () {
                height = container.parentNode.clientHeight;
                width = container.parentNode.clientWidth;
                return new Size(width, height);
            }

            // initialize map and bind it to the container
            var map = new Map({
                id: container.id,
                dragBehavior: Map.DRAG_PAN,
                //size: this._getContainerSize(),
                center: new LatLon(options['initialPosition'][0], options['initialPosition'][1]),
                rightDragBehavior: Map.DRAG_ZOOM
            });

            this.map = map;
            //map.setPanCursor("pointer");
            //map.setPanningCursor("hand");

            //Devin
            var TriplatLonLocations;
            this.TriplatLonLocations = TriplatLonLocations;


            // private members
            this._geozoneVisible = true;
            this._landmarkVisible = true;

            // disabled until a fix is found for IE
            //            Util.addEvent(window, "resize", function () {
            //                map.setSize(this._getContainerSize());
            //            });

            // setup satellite imagery

            //Devin

            this.tiles = new TileLayer({
                id: 'tileLayer',
                map: this.map,
                tileConfig: { satellite: false }
            });

            // setup layers

            this.assetLayer = new ObjectLayer({
                id: "assetLayer",
                map: this.map,
                zIndex: Sentinel.Constants.LAYER_ZINDEX.ASSET
            });

            this.historyLayer = new RouteLayer({
                id: "historyLayer",
                map: this.map,
                zIndex: Sentinel.Constants.LAYER_ZINDEX.TRAILS,
                lineColor: new Color(0x00, 0x00, 0x00, 0.75)
            });

            this.landmarkLayer = new ObjectLayer({
                id: "landmarkLayer",
                map: this.map,
                zIndex: Sentinel.Constants.LAYER_ZINDEX.LANDMARK
            });

            this.pushpinLayer = new ObjectLayer({ id: "pushpin_layer", map: this.map });

            this.geozoneLabelLayer = new ObjectLayer({
                id: "geozoneLabelLayer",
                map: this.map,
                zIndex: Sentinel.Constants.LAYER_ZINDEX.GEOZONE_LABEL
            });

            // layer collections are not bound to the map, so we must create a handle for future reference
            this.geozoneFenceCollection = new FenceCollection({
                id: "geozoneFenceCollection"
            });

            this.landmarkFenceCollection = new FenceCollection({
                id: "landmarkFenceCollection"
            });
            this.landmarkFenceCollection.add(this.landmarkLayer);

            // add the labels layer
            this.geozoneFenceCollection.add(this.geozoneLabelLayer);

            // create functions for missing features of the FenceCollection object
            this.geozoneFenceCollection.show = function () {
                for (var i in this._layers) {
                    this._layers[i].setVisibility(1);
                }
                //map.setHeight(map.getHeight() + 1);
                //map.setHeight(map.getHeight() - 1);
            };

            this.geozoneFenceCollection.hide = function () {
                for (var i in this._layers) {
                    this._layers[i].setVisibility(0);
                }
                // map.setHeight(map.getHeight() + 1); //force fresh map
                //map.setHeight(map.getHeight() - 1);
                //map.setSize(new Telogis.GeoBase.Size(300, 300));
            };

            this.landmarkFenceCollection.show = function () {

                for (var i in this._layers) {
                    if (this._layers[i].id == "landmarkLayer") {
                        // label layers gets cleared only, not destroyed
                        this._layers[i].setVisibility(1);
                    }
                    //this._layers[i].setVisibility(1);
                }
            };

            this.landmarkFenceCollection.hide = function () {
                for (var i in this._layers) {
                    this._layers[i].setVisibility(0);
                }
            };


            this.geozoneFenceCollection.clear = function () {
                for (var i in this._layers) {
                    if (this._layers[i].id == "geozoneLabelLayer") {
                        // label layers gets cleared only, not destroyed
                        this._layers[i].clear();
                    } else {
                        // all other layers are destroyed
                        this._layers[i].destroy();
                    }
                }
                // clear the list and keep only the geozoneLabelLayer
                this._layers = [map.geozoneLabelLayer];
            };

            this.landmarkFenceCollection.clear = function () {
                for (var i in this._layers) {
                    if (this._layers[i].id == "landmarkLayer") {
                        // label layers gets cleared only, not destroyed
                        //this._layers[i].clear();
                    } else {
                        // all other layers are destroyed
                        this._layers[i].destroy();
                    }
                }
                // clear the list and keep only the geozoneLabelLayer
                this._layers = [map.landmarkLayer];
            };

            this.drawingLayer = null; /*new Layer({
                id: "drawingLayer",
                map: this.map,
                zIndex: Sentinel.Constants.LAYER_ZINDEX.DRAWING
            });*/

            // add map controls to the map

            this.map.frame.appendChild(document.getElementById('topControlArea'));
            this.map.frame.appendChild(document.getElementById('panControlPanel'));
            this.map.frame.appendChild(document.getElementById('topMenu'));
            //this.map.frame.appendChild(document.getElementById('drawingToolsPanel'));

            this.slider = new Slider({
                id: "zoomSlider",
                skin: SliderSkin.translucentVertical
            })
            //this.slider.elem.style.top = 60; // offset the slider just below the pan buttons control

            this.slider.setPosition(new Point(10, 90));

            this.leftDock = new Dock({
                id: "leftDock",
                align: "left",
                map: this.map,
                items: [this.slider]
            });

            // contextual menu

            var loc = new LatLon();
            var landmarkloc = new LatLon();
            var createlandmarkloc = new LatLon();
            var pos = new Point();

            var landmarkPoints = new Array();
            var geozonePoints = new Array();

            var contextualMenu = new ContextMenu();
            $(contextualMenu.elem).width("150px");

            var _rightclick_txt = Dictionary.createLandmark;
            if (IsFrenchCulture) _rightclick_txt = Dictionary.createLandmark_fr;
            contextualMenu.appendEntry(_rightclick_txt, true, function () {
                eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.CREATE_LANDMARK, { lat: loc.lat, lon: loc.lon });
                //devin
                //AddLandmarkWindow();
            });
            _rightclick_txt = Dictionary.search;
            if (IsFrenchCulture) _rightclick_txt = Dictionary.search_fr;
            contextualMenu.appendEntry(_rightclick_txt, true, function () {
                eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.SEARCH, { lat: loc.lat, lon: loc.lon });
                //devin
                //AddLandmarkWindow();
            });

            /*
            examples:
            menu.appendEntry ('<span class="hotkey">A</span>ppend Route Stop',            true, function () {Routing.append  (loc);});
            menu.appendSeparator ();
            */

            // ************************************************************************************
            // input events handling

            this.map.Click.append(function (e) {
                contextualMenu.hide();
            });

            this.map.MouseDown.append(function (e) {
                pos = local.map.mouseXY(e);
            });

            this.map.RightClick.append(function (e) {
                var previousPos = pos;
                pos = local.map.mouseXY(e);
                //loc = local.map.mouseLatLon(e);
                createlandmarkloc = local.map.mouseLatLon(e);
                if (previousPos != null && previousPos.x == pos.x && previousPos.y == pos.y) {
                    contextualMenu.show(e);
                } // else, ignoring event because the mouse was dragged
            });

            // ************************************************************************************
            // prevent mouse events in control panels from propagating to underlying map

            $(".mapControlPanel").click(function (e) {
                e.stopPropagation();
            });

            $(".mapControlPanel").dblclick(function (e) {
                e.stopPropagation();
            });

            // ************************************************************************************
            // helper functions for the map object

            this.map.getVisibleBoundingBox = function () {
                // calculate visible area
                var upperLeft = this.getLatLon(new Point(0, 0));
                var lowerRight = this.getLatLon(new Point(this.getWidth(), this.getHeight()));
                return new BoundingBox(upperLeft, lowerRight);
            };


            // ************************************************************************************
            // map behavior support properties

            // recenter button callback
            this.map.previousAction = new function () { };

            // geoassets cache support
            this.outerBound = this.map.getVisibleBoundingBox();
            this.previousOuterBound = new BoundingBox(new LatLon(0, 0, new LatLon(0, 0)));
            this.previousZoomIndex = this.map.getZoomIndex();


            // ************************************************************************************
            // map behavior support functions

            this.map.Update.append(function () {

                // process zoom level
                // if zoom level changed
                var currentZoomIndex = local.map.getZoomIndex();
                if (currentZoomIndex != local.previousZoomIndex) {
                    if (currentZoomIndex > local.previousZoomIndex) {
                        // zoomed IN
                        if (currentZoomIndex >= Sentinel.Constants.HIGH_ZOOM_INDEX) {
                            // fell below details threshold
                            //console.log("Showing details: high zoom index");
                            $(map.elem).removeClass("lowZoomIndex");
                        }
                    } else {
                        // zoomed OUT
                        if (currentZoomIndex < Sentinel.Constants.HIGH_ZOOM_INDEX) {
                            // raised above details threshold
                            //console.log("Hiding details: low zoom index");
                            $(local.map.elem).addClass("lowZoomIndex");
                        }
                    }
                    // update previous index
                    local.previousZoomIndex = currentZoomIndex;
                }

                // process area
                var visibleBoundingBox = local.map.getVisibleBoundingBox();
                if (local.outerBound.contains(visibleBoundingBox)) {
                    return; // nothing to do, return fast
                } else {
                    //console.log("Moved outside cached area, calculating bounds");
                    visibleBoundingBox.inflateBy(Sentinel.Constants.GEOASSET_CACHE_AREA_FACTOR);
                    local.previousOuterBound = local.outerBound
                    local.outerBound = visibleBoundingBox;
                    local.refreshGeoAssetCache();
                }

            });

            this.refreshGeoAssetCache = function () {
                /// <summary>
                ///     Refresh cache of "geo assets" for the region covered by the current viewport
                /// </summary>

                //console.log("Refreshing geoassets cache");

                var boundingBox = this.outerBound;
                var norBoundingBox = this.previousOuterBound;

                // prepare args
                // note: coordinate parameters look strange because there's a discrepancy between the web service
                // and the bounding box mappings: one expects NW/SE corners, and the other expects NE/SW

                // TODO replace SID, userId and orgId
                // TODO this is quite ugly, should find a better way to pass parameters to the web service
                var args = "{" +
                    "userId: " + 123 + "," +
                    "SID: \"" + "123" + "\"," +
                    "orgId: " + 480 + "," +
                    "topleftlat: " + boundingBox.getNE().lat + ", " +
                    "topleftlong: " + boundingBox.getSW().lon + ", " +
                    "bottomrightlat: " + boundingBox.getSW().lat + ", " +
                    "bottomrightlong: " + boundingBox.getNE().lon + ", " +
                    "topleftlatnor: " + norBoundingBox.getNE().lat + ", " +
                    "topleftlongnor: " + norBoundingBox.getSW().lon + ", " +
                    "bottomrightlatnor: " + norBoundingBox.getSW().lat + ", " +
                    "bottomrightlongnor: " + norBoundingBox.getNE().lon + "}";

                var loadingLabel = document.createElement("div");

            };


            // ************************************************************************************
            // add custom functions

            this.setSizeXY = function (height, width) {
                /// <summary>
                ///     Changes the size of the map viewport.
                /// </summary>

                this.map.setSize(new Size(height, width));
            };

            this.panRight = function () {
                /// <summary>
                ///     Pans the map right-ward.
                /// </summary>
                this.map.panBy(new Point(-Sentinel.Constants.PAN_SENSITIVITY, 0), false);
            };

            this.panLeft = function () {
                /// <summary>
                ///     Pans the map left-ward.
                /// </summary>
                this.map.panBy(new Point(Sentinel.Constants.PAN_SENSITIVITY, 0), false);
            };

            this.panUp = function () {
                /// <summary>
                ///     Pans the map up-ward.
                /// </summary>

                this.map.panBy(new Point(0, Sentinel.Constants.PAN_SENSITIVITY), false);
            };

            this.panDown = function () {
                /// <summary>
                ///     Pans the map down-ward.
                /// </summary>

                this.map.panBy(new Point(0, -Sentinel.Constants.PAN_SENSITIVITY), false);
            };

            this.satelliteImagery = function (enabled) {
                /// <summary>
                ///     Controls visibility of satellite imagery.
                /// </summary>

                this.map.tileLayer.reconfigureTiles({ satellite: enabled });
            };

            this.enableLayer = function (layerId, enable) {
                /// <summary>
                ///     Enable/disable visibility of a layer.
                /// </summary>
                if (layerId == "landmarkLayer") {
                    this.setLandmarkVisibility(enable);
                    return;
                }
                var layer = this[layerId];
                if (enable) {
                    layer.show();
                } else {
                    layer.hide();
                }
            };

            this.setRecenterAction = function (actionFunction) {
                /// <summary>
                ///     Set the action to execute when the Recenter button is pressed.
                /// </summary>

                // set action
                this.previousAction = actionFunction;
                // enable recenter button
                //devin
                document.getElementById("recenterButton").removeAttribute("disabled");
            }

            this.clearRecenterAction = function () {
                /// <summary>
                ///      Clear the Recenter button action.
                /// </summary>

                // clear action
                this.previousAction = null;
                // disable recenter button
                document.getElementById("recenterButton").setAttribute("disabled", "disabled");
            }

            this.recenter = function () {
                /// <summary>
                ///     Re-launch the previous "map it" action.
                /// </summary>

                if (this.previousAction) {
                    try {
                        //this[this.previousAction[0]].call(previousAction[1]);
                        this.previousAction();
                    } catch (e) {
                        console.error("Failed to execute previous action", e);
                    }
                }
            };

            this.showLandmark = function (landmarks) {
                /// <summary>
                ///     Add landmarks on the map.
                /// </summary>

                this.landmarkLayer.clear();

                if (landmarks == null || landmarks.length == 0) {
                    return;
                }

                for (var i in landmarks) {

                    var landmark = landmarks[i];

                    try {
                        var balloonInfo =
                            "<span class=\"balloonBlock\">" +
                            "<p><B>" + MapLandMarkText + "</B>" + landmark.desc + "</p>" +
                            "</span>";

                        //var iconAndLabel = "<div class=\"elementIcon\"><img src=\"" +
                        //    Sentinel.Constants.LANDMARK_ICONS[landmark.type] + "\"/><span class=\"text\">" + landmark.desc + "</span></div>";
                        //devin

                        var iconAndLabel = "<img src=\"" +
                            landmark.icon + "\"/>";

                        //                        if (landmark.show == "0")
                        //                            iconAndLabel = "<div class=\"elementIcon\"><img src=\"" +
                        //                            landmark.icon + "\"/></div>";
                        var obj = new DivObject({
                            id: "landmarkLayer_" + landmark.id,
                            anchorPoint: new Point(0.5, 0.5),
                            dragEnabled: false,
                            layer: this.landmarkLayer,
                            location: new LatLon(parseFloat(landmark.lat), parseFloat(landmark.lon)),
                            balloonConfig: {
                                content: balloonInfo,
                                skin: BalloonSkin.standard,
                                behavior: Balloon.HOVER_ACTIVE
                            },
                            innerHTML: iconAndLabel,
                            size: new Size(16, 16) // size of the icon only
                        });

                    } catch (e) {
                        console.error("Failed to display landmark, cause: " + e.message);
                    }
                }

            };
            //Devin
            this.showAddress = function (addr) {
                map.setZoomIndex(Sentinel.Constants.ASSET_DEFAULT_ZOOM_INDEX, false);
                this.assetLayer.clear();
                var latLonLocations = new Array();
                var latLon = new LatLon(parseFloat(addr.lat), parseFloat(addr.lon));
                var iconAndLabel = "";
                if (addr.desc == '') {
                    iconAndLabel = "<img src=\"../Bigicons/" +
                            "RedCircle.ico" + "\"/>";
                }
                else {
                    iconAndLabel = "<div class=\"elementIcon\"><img src=\"../Bigicons/" +
                            "RedCircle.ico" + "\"/><span class=\"text\">" + addr.desc + "</span></div>";
                }
                try {

                    new DivObject({
                        id: "assetLayer_" + addr.id,
                        anchorPoint: new Point(0.5, 0.5),
                        dragEnabled: false,
                        layer: this.assetLayer,
                        location: latLon,
                        innerHTML: iconAndLabel,
                        size: new Size(16, 16) // size of the icon only
                    });
                    latLonLocations.push(latLon);
                    this.centerMap(latLonLocations);
                }
                catch (e) {
                    console.error("Failed to display Address, cause: " + e.message);
                }

            }

            this.showAsset = function (fleet) {
                /// <summary>
                ///     Adds assets on the map, re-centering the map on the elements.
                /// </summary>
                map.setZoomIndex(Sentinel.Constants.ASSET_DEFAULT_ZOOM_INDEX, false);
                if (fleet == null || fleet.length == 0) {
                    return;
                }
                this.historyLayer.setPoints([]);
                this.assetLayer.clear();

                var latLonLocations = new Array();

                this.assetLayer.startBulkAdd(); // tell layer (potentially) a lot of items will be added

                for (var i in fleet) {

                    var vehicle = fleet[i];

                    //console.log("Adding an asset");

                    try {


                        var headingStr = '';
                        try {
                            if (vehicle.head.substring(0, 1) >= "0" && vehicle.head.substring(0, 1) <= "9")
                               headingStr = Dictionary['heading_' + MathUtil.bearingToCompass(vehicle.head)];
                            else
                               headingStr = Dictionary['heading_' + vehicle.head];

                        }
                        catch (ex) { }

                        var balloonInfo = '';

                        if (!IsFrenchCulture) {
                            balloonInfo = "<span class=\"balloonBlock\">" +
                            /*"<p>" + vehicle.desc + "</p>" +*/
                            "<table cellspacing='0' cellpadding='0' >" +
                            "<tr><td><em>" + Dictionary['vehicle'] + ":</em> " + vehicle.desc + "</td></tr>" +
                            "<tr><td>" + vehicle.addr + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['time'] + ":</em> " + vehicle.date + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['heading'] + ":</em> " + headingStr + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['speed'] + ":</em> " + vehicle.spd + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['status'] + ":</em> " + vehicle.stat + "</td></tr>" +
                            "</table>" +
                            "</span>";
                        }
                        else {
                            balloonInfo = "<span class=\"balloonBlock\">" +
                            /*"<p>" + vehicle.desc + "</p>" +*/
                            "<table cellspacing='0' cellpadding='0' >" +
                            "<tr><td><em>" + Dictionary['vehicle_fr'] + ":</em> " + vehicle.desc + "</td></tr>" +
                            "<tr><td>" + vehicle.addr + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['time_fr'] + ":</em> " + vehicle.date + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['heading_fr'] + ":</em> " + headingStr + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['speed_fr'] + ":</em> " + vehicle.spd + "</td></tr>" +
                            "<tr><td><em>" + Dictionary['status_fr'] + ":</em> " + vehicle.stat + "</td></tr>" +
                            "</table>" +
                            "</span>";
                        }
                        //devin
                        var latLon = new LatLon(parseFloat(vehicle.lat), parseFloat(vehicle.lon));
                        latLonLocations.push(latLon);

                        var iconAndLabel = "";
                        if (vehicle.desc != '')
                            iconAndLabel = "<div class=\"elementIcon\"><table cellspacing='0' cellpadding='0' ><tr valign='middle'><td>" +
                            "<img src=\"../Bigicons/" +
                            vehicle.icon + "\"/></td><td style='color:black' class='vehicledash'><b>-</b></td><td align='left'><span class=\"vehicletext\" >" + vehicle.desc + "</span></td></tr></table></div>";
                        else
                            iconAndLabel = "<img src=\"../Bigicons/" +
                                vehicle.icon + "\"/>";

                        new DivObject({
                            id: "assetLayer_" + vehicle.id,
                            anchorPoint: new Point(0.5, 0.5),
                            dragEnabled: false,
                            layer: this.assetLayer,
                            location: latLon,
                            balloonConfig: {
                                content: balloonInfo,
                                skin: BalloonSkin.standard,
                                behavior: Balloon.HOVER_ACTIVE
                            },
                            innerHTML: iconAndLabel,
                            size: new Size(16, 16) // size of the icon only
                        });

                    } catch (e) {
                        console.error("Failed to display asset, cause: " + e.message);
                    }

                }

                this.assetLayer.endBulkAdd(); // done adding items

                // center the map on the items just added to the map
                this.centerMap(latLonLocations);
            };

            this.BeginshowTrip = function () {
                this.assetLayer.clear();
                this.loc = new Array();
                this.historyLayer.setPoints([]);
                this.assetLayer.startBulkAdd(); // tell layer (potentially) a lot of items will be added
            };

            this.showTrip = function (trip) {
                /// <summary>
                ///     Show a trip on the map, adding a trail between each point (optional).
                /// </summary>
                if (trip == null || trip.length == 0) {
                    return;
                }


                //var boxId = trip.id;
                //var vehicleDesc = trip.desc;
                //var points = trip.points;

                var isTrail = false;
                //console.log("Adding a trip");

                for (var i in trip) {

                    var point = trip[i];
                    if (point == null) continue;
                    try {

                        var headingStr = Dictionary['heading_' + MathUtil.bearingToCompass(point.head)];
                        var statStr = '';
                        var balloonInfo =
                            "<span class=\"balloonBlock\">" + point.toolTip + "</span>";

                        //                            "<table cellspacing='0' cellpadding='0' >" +
                        //                            "<tr><td>" + point.addr + "</td></tr>" +
                        //                            "<tr><td><em>" + Dictionary['time'] + ":</em> " + point.date + "</td></tr>" +
                        //                            "<tr><td><em>" + Dictionary['heading'] + ":</em> " + headingStr + "</td></tr>" +
                        //                            "<tr><td><em>" + Dictionary['speed'] + ":</em> " + point.spd + "</td></tr>";
                        //                        if (point.stat != "N") balloonInfo = balloonInfo +
                        //                            "<tr><td><em>" + Dictionary['status'] + ":</em> " + point.stat + "</td></tr>";
                        //                        balloonInfo = balloonInfo +   "</table>" +               "</span>";

                        var latLon = new LatLon(parseFloat(point.lat), parseFloat(point.lon));
                        this.loc.push(latLon);

                        var iconAndLabel = "<div class=\"elementIcon\"><img src=\"../Bigicons/" +
                            point.icon + "\"/></div>";

                        if (point.trail == 1) isTrail = true;

                        new DivObject({
                            id: "assetLayer_" + point.id + "_" + i,
                            anchorPoint: new Point(0.5, 0.5),
                            dragEnabled: false,
                            layer: this.assetLayer,
                            location: latLon,
                            balloonConfig: {
                                content: balloonInfo,
                                skin: BalloonSkin.standard,
                                behavior: Balloon.HOVER_ACTIVE
                            },
                            innerHTML: iconAndLabel,
                            size: new Size(16, 16) // size of the icon only
                        });

                    } catch (e) {
                        console.error("Failed to display asset, cause: " + e.message);
                    }

                }



            };

            this.EndshowTrip = function () {
                map.setZoomIndex(Sentinel.Constants.ASSET_DEFAULT_ZOOM_INDEX, false);
                if (this.loc != null && this.loc.length > 0) {
                    this.historyLayer.setPoints(this.loc);
                }
                this.centerMap(this.loc);
                this.assetLayer.endBulkAdd(); // done adding items
                $("#divMapLoadingText").fadeOut();
            };

            this.centerMap = function (latLonLocations) {
                /// <summary>
                ///     Center map around the locations in parameter.
                /// </summary>

                if (latLonLocations == null || latLonLocations.length == 0) {
                    return;
                }

                var focusOnItemsFunction = null;

                if (latLonLocations.length == 1) {
                    focusOnItemsFunction = function () {
                        map.setCenter(latLonLocations[0], true);
                        map.setZoomIndex(Sentinel.Constants.ASSET_DEFAULT_ZOOM_INDEX, false);
                    };
                } else {
                    // create bounding box by selecting the first 2 points
                    var boundingBox = new BoundingBox(latLonLocations[0], latLonLocations[1]);
                    // then stretch the bounding box using the other points
                    for (var i = 2; i < latLonLocations.length; i++) {
                        boundingBox.add(latLonLocations[i]);
                    }
                    focusOnItemsFunction = function () {
                        // zoom the view close on the bounding box
                        map.zoomTo(boundingBox);
                    };
                }

                // move the map to show the items added to the map
                focusOnItemsFunction();

                // set function for recenter button
                this.setRecenterAction(focusOnItemsFunction);

            }

            this.showGeozone = function (geozones) {
                /// <summary>
                ///     Show geozones on map.
                /// </summary>
                if (geozones == null || geozones.length == 0) {
                    return;
                }

                // remove all geofences
                this.geozoneFenceCollection.clear();

                for (var i in geozones) {

                    var geozone = geozones[i];
                    var newZone = null;

                    // prevent from showing a geozone that is being edited
                    if (this.drawingContext != null && this.drawingContext.editingGeozoneId != null) {
                        if (this.drawingContext.editingGeozoneId == geozone.id) {
                            continue;
                        }
                    }

                    if (geozone.lat != 'N' && geozone.lon != 'N') {
                        var centerLatLon = new LatLon(parseFloat(geozone.lat), parseFloat(geozone.lon));

                        var balloonInfo =
                        "<span class=\"balloonBlock\">" +
                        "<B>Geozone: </B>" + geozone.desc + "" +
                        /*                        "<p><em>" + Dictionary['time'] + ":</em> " + point.date + "</p>" +
                        "<p><em>" + Dictionary['heading'] + ":</em> " + headingStr + "</p>" +
                        "<p><em>" + Dictionary['speed'] + ":</em> " + point.spd + "</p>" +*/
                        "</span>";

                        //                        var iconAndLabel = "<div class=\"elementIcon\"><img src=\"" +
                        //                        Sentinel.Constants.GEOZONE_ICONS[geozone.type] + "\"/><span class=\"text\">" + geozone.desc + "</span></div>";
                        var iconAndLabel = "<img src=\"" +
                            Sentinel.Constants.GEOZONE_ICONS[geozone.type] + "\"/>";
                        var label = new DivObject({
                            id: "geozoneLabelLayer_" + geozone.id,
                            anchorPoint: new Point(0.5, 0.5),
                            dragEnabled: false,
                            layer: this.geozoneLabelLayer,
                            location: centerLatLon,
                            balloonConfig: {
                                content: balloonInfo,
                                skin: BalloonSkin.standard,
                                behavior: Balloon.HOVER_ACTIVE
                            },
                            innerHTML: iconAndLabel,
                            size: new Size(16, 16) // size of the icon only
                        });
                    }
                    geozone.coords = eval(geozone.coords);
                    if (geozone.coords.length == 1) {
                        // circular geozone


                        var radius = MathUtil.distance(centerLatLon,
                            new LatLon(parseFloat(geozone.coords[0][0]), parseFloat(geozone.coords[0][1])), DistanceUnit.METERS);

                        newZone = new CircleFence({
                            id: "geozone_" + geozone.id,
                            parent: this.map,
                            center: centerLatLon,
                            radius: radius,
                            units: DistanceUnit.METERS,
                            zIndex: Sentinel.Constants.LAYER_ZINDEX.GEOZONE
                        });

                    } else if (geozone.coords.length == 2) {
                        // rectangular geozone

                        newZone = new RegionShadeLayer({
                            id: "geozone_" + geozone.id,
                            parent: this.map,
                            nw: new LatLon(parseFloat(geozone.coords[0][0]), parseFloat(geozone.coords[0][1])),
                            se: new LatLon(parseFloat(geozone.coords[1][0]), parseFloat(geozone.coords[1][1])),
                            zIndex: Sentinel.Constants.LAYER_ZINDEX.GEOZONE
                        });

                    } else if (geozone.coords.length > 2) {
                        // polygonal geozone

                        var points = [];
                        for (j in geozone.coords) {
                            points.push(new LatLon(parseFloat(geozone.coords[j][0]), parseFloat(geozone.coords[j][1])));
                        }

                        newZone = new PolygonFence({
                            id: "geozone_" + geozone.id,
                            parent: this.map,
                            points: points,
                            zIndex: Sentinel.Constants.LAYER_ZINDEX.GEOZONE
                        });
                    }
                    // set the initial visibility of the geozone
                    if (newZone != null) {
                        newZone.setVisibility(this._geozoneVisible ? 1 : 0);

                        this.geozoneFenceCollection.add(newZone);
                    }

                }


            };

            this.showGeoAsset = function (geoassets) {
                /// <summary>
                ///     Show geoassets on map.
                /// </summary>

                var geozones = [];
                var landmarks = [];

                var geoasset = null;
                for (var i in geoassets) {
                    geoasset = geoassets[i];
                    if (geoasset.type == 0) {
                        landmarks.push(geoasset);
                    } else if (geoasset.type == 1) {
                        geozones.push(geoasset);
                    } else {
                        console.warn("Undefined geoasset type: " + geoasset.type, geoasset);
                    }
                }

                this.showGeozone(geozones);
                this.showLandmark(landmarks);
            };

            this.setGeozoneVisibility = function (visible) {
                /// <summary>
                ///     Set visibility of geozones on map.
                /// </summary>

                this._geozoneVisible = visible;
                if (visible) {
                    this.geozoneFenceCollection.show();
                } else {
                    this.geozoneFenceCollection.hide();
                }
            };

            this.setLandmarkVisibility = function (visible) {
                /// <summary>
                ///     Set visibility of geozones on map.
                /// </summary>
                this._landmarkVisible = visible;
                if (visible) {
                    this.landmarkFenceCollection.show();
                } else {
                    this.landmarkFenceCollection.hide();
                }
            };

            this.enableLabels = function (enabled) {
                /// <summary>
                ///     Set visibility of labels on map.
                /// </summary>

                if (enabled) {
                    $(this.map.elem).removeClass("hideLabels");
                } else {
                    $(this.map.elem).addClass("hideLabels");
                }
            };

            this.enableTrails = function (enabled) {
                /// <summary>
                ///     Set visibility of trails (links between points) when mapping trips.
                /// </summary>

                if (enabled) {
                    this.historyLayer.show();
                } else {
                    this.historyLayer.hide();
                }
            };

            // TODO: to be removed
            this.map.Click.append(function (e) {
                if (e.shiftKey) {
                    //$("#coordBox").innerHTML = $("#coordBox").innerHTML + map.mouseLatLon(e) + "<br/>";
                    //console.log(local.map.mouseLatLon(e));
                }
            });


            this.drawingContext = {
                formFactor: Sentinel.Constants.GEOZONE_FORM_FACTOR.RECTANGLE,
                points: [],
                moveCallback: null,
                clear: function () { this.points = []; },
                isEditing: false,
                editingGeozoneId: null
            };

            this._initDrawingLayer = function () {
                if (local.drawingContext.formFactor == Sentinel.Constants.GEOZONE_FORM_FACTOR.RECTANGLE) {

                    var points = [];
                    if (local.drawingContext.points.length == 2) {
                        var nw = local.drawingContext.points[0];
                        var se = local.drawingContext.points[1];
                        points.push(nw);
                        points.push(new LatLon(nw.lat, se.lon));
                        points.push(se);
                        points.push(new LatLon(se.lat, nw.lon));
                    }
                    this.drawingLayer = new PolygonFence({
                        parent: local.map,
                        vertexRadius: 4,
                        points: points
                    });

                } else if (local.drawingContext.formFactor == Sentinel.Constants.GEOZONE_FORM_FACTOR.CIRCLE) {

                    var centerLatLon = null;
                    var radius = null;
                    if (local.drawingContext.points.length == 2) {
                        centerLatLon = local.drawingContext.points[0];
                        radius = MathUtil.distance(centerLatLon, local.drawingContext.points[1], DistanceUnit.METERS);
                    }
                    local.drawingLayer = new CircleFence({
                        parent: local.map,
                        center: centerLatLon,
                        radius: radius,
                        units: DistanceUnit.METERS
                    });

                } else if (local.drawingContext.formFactor == Sentinel.Constants.GEOZONE_FORM_FACTOR.POLYGON) {

                    var points = [];
                    if (local.drawingContext.points.length > 3) { // need at least 3 points to form a polygon
                        points = points.concat(local.drawingContext.points);
                    }
                    local.drawingLayer = new PolygonFence({
                        parent: local.map,
                        vertexRadius: 4,
                        points: points
                    });
                }
            };

            this.editGeozone = function (type, maximunPoints) {

                if (type == Sentinel.Constants.GEOZONE_FORM_FACTOR.CIRCLE) {
                    var appendCircle = function (e) {
                        // add point only if ctrl key is pressed
                        try {
                            if (local.map.drawingContext.formFactor == Sentinel.Constants.GEOZONE_FORM_FACTOR.CIRCLE
                            && local.drawingLayer != null
                          ) return;
                        }
                        catch (err) { }
                        if (!(e.ctrlKey || e.metaKey)) {
                            local.map.Click.replace(appendCircle);
                            //this.drawingContext.formFactor = Sentinel.Constants.GEOZONE_FORM_FACTOR.CIRCLE;
                            return;
                        }
                        //local.map.disableUI();
                        local.geozoneEraseClicked();
                        var mouseLoc = new LatLon();
                        mouseLoc = local.map.mouseLatLon(e);

                        local.drawingLayer = new CircleFence({
                            parent: local.map,
                            center: mouseLoc,
                            radius: 0,
                            units: DistanceUnit.METERS
                        });


                        var landmarkObj = new ImageObject({
                            anchorPoint: new Point(0.5, 1.0),
                            dragEnabled: false,
                            layer: local.pushpinLayer,
                            location: mouseLoc,
                            src: "../Bigicons/NewLandmark.ico"
                        });

                        var updateCircle = function (e) {
                            mouseLoc = local.map.mouseLatLon(e);
                            var dist = mouseLoc.distanceTo(local.drawingLayer.getCenter(), DistanceUnit.METERS);
                            local.drawingLayer.setRadius(dist, DistanceUnit.METERS);
                        };

                        local.map.MouseMove.append(updateCircle);
                        local.map.MouseUp.replace(function () {

                            local.map.MouseMove.remove(updateCircle);
                            local.map.enableUI();
                        });
                        local.map.Click.replace(appendCircle);
                    }
                    //this._initDrawingLayer();

                    //local.map.disableUI();
                    //var oldCursor = Maps.main.setCursor('crosshair');
                    this.map.Click.replace(appendCircle);
                    this.drawingContext.formFactor = Sentinel.Constants.GEOZONE_FORM_FACTOR.CIRCLE;
                    return;
                }
                this.pushpinLayer.clear();
                this.drawingContext.points = [];
                this.drawingContext.moveCallback = null;
                if (type == Sentinel.Constants.GEOZONE_FORM_FACTOR.RECTANGLE)
                    this.drawingContext.formFactor = Sentinel.Constants.GEOZONE_FORM_FACTOR.RECTANGLE;
                if (type == Sentinel.Constants.GEOZONE_FORM_FACTOR.POLYGON)
                    this.drawingContext.formFactor = Sentinel.Constants.GEOZONE_FORM_FACTOR.POLYGON;

                this._initDrawingLayer();
                var addPoint = function (e) {

                    var loc_1 = local.map.mouseLatLon(e);

                    // add point only if ctrl key is pressed
                    if (!(e.ctrlKey || e.metaKey)) {
                        local.map.Click.replace(addPoint);
                        return;
                    }

                    if (type == Sentinel.Constants.GEOZONE_FORM_FACTOR.RECTANGLE) {
                        if (local.drawingContext.points.length == 0) {

                            local.drawingContext.points.push(loc_1);
                        } else if (local.drawingContext.points.length == 1) {
                            // reorganize points so that the points are [NW,SE]
                            bb = new BoundingBox(local.drawingContext.points[0], loc_1);
                            var nw = new LatLon(bb.getNE().lat, bb.getSW().lon);
                            var se = new LatLon(bb.getSW().lat, bb.getNE().lon);
                            local.drawingContext.points = [nw, se];
                            // draw rectangle
                            local.drawingLayer.appendPoint(new LatLon(nw.lat, nw.lon));
                            local.drawingLayer.appendPoint(new LatLon(nw.lat, se.lon));
                            local.drawingLayer.appendPoint(new LatLon(se.lat, se.lon));
                            local.drawingLayer.appendPoint(new LatLon(se.lat, nw.lon));
                        }

                    } else if (local.drawingContext.formFactor == Sentinel.Constants.GEOZONE_FORM_FACTOR.POLYGON) {
                        if (local.drawingContext.points.length < maximunPoints) {
                            local.drawingContext.points.push(loc_1);
                            local.drawingLayer.appendPoint(loc_1);
                        } else {
                            alert(Dictionary["GeozoneMaxPointsReached"]);
                        }
                    }

                    local.map.Click.replace(addPoint);
                };

                this.map.Click.replace(addPoint);

                this.drawingContext.isEditing = true;

            };

            this.geozoneSaveClicked = function () {

                // gather results
                var type = this.drawingContext.formFactor;
                var coordinates = null;
                var lat = null;
                var lon = null;
                var radius = null;
                if (local.drawingContext.formFactor == Sentinel.Constants.GEOZONE_FORM_FACTOR.CIRCLE) {
                    // circular fence
                    lat = this.drawingContext.points[0].lat;
                    lon = this.drawingContext.points[0].lon;
                    coordinates = [this.drawingContext.points[1]];
                    radius = MathUtil.distance(this.drawingContext.points[0], this.drawingContext.points[1], DistanceUnit.METERS);
                } else {
                    // rectangular or polygonal fence
                    coordinates = this.drawingContext.points;
                    // calculate center by fitting all points in a bounding box, and then use center
                    var bb = new BoundingBox();
                    for (var k in coordinates) {
                        bb.add(coordinates[k]);
                    }
                    lat = bb.getCenter().lat;
                    lon = bb.getCenter().lon;
                    radius = null;
                }

                // reset drawing support
                this.drawingContext.clear();
                this.drawingLayer.destroy();
                // hide drawing tools
                $("#drawingToolsPanel").hide();
                // notify listeners of drawing cancellation
                eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.GEOZONE_EDIT_SAVED, {
                    type: type,
                    coordinates: coordinates,
                    latitude: lat,
                    longitude: lon,
                    radius: radius
                });

                this.drawingContext.isEditing = false;
                this.drawingContext.editingGeozoneId = null;

            };

            this.geozoneCancelClicked = function () {

                // reset drawing support
                this.drawingContext.clear();
                this.drawingLayer.destroy();
                // hide drawing tools
                $("#drawingToolsPanel").hide();
                // notify listeners of drawing cancellation
                eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.GEOZONE_EDIT_CANCELLED, null);

                this.drawingContext.isEditing = false;
                this.drawingContext.editingGeozoneId = null;

            };

            this.geozoneEraseClicked = function () {
                if (this.drawingContext != null) this.drawingContext.points = [];
                if (this.drawingLayer != null) {
                    this.drawingLayer.destroy();
                    this._initDrawingLayer();
                }
                this.pushpinLayer.clear();
            };

            this.geozoneFormFactorChanged = function () {

                if (this.drawingLayer != null) {
                    this.drawingLayer.destroy();
                }
                this.drawingContext.clear();
                this.drawingContext.formFactor = $("#formFactorSelect").val();
                this._initDrawingLayer();

                var instructions = Dictionary["editGeozoneInstructions"] + "<br>" +
                    Dictionary["editGeozoneInstructions_" + this.drawingContext.formFactor];
                $('#drawingToolsInstructions').html(instructions);
            };

            // ************************************************************************************
            // event dispatch bindings

            eventDispatcher.register(Sentinel.Constants.EVENTS.SHOW_ASSET, this, function (args) {
                local.showAsset(args.fleet);
            });

            eventDispatcher.register(Sentinel.Constants.EVENTS.SHOW_ADDRESS, this, function (args) {
                local.showAddress(args.addr);
            });


            eventDispatcher.register(Sentinel.Constants.EVENTS.SHOW_TRIP, this, function (args) {
                local.showTrip(args.trip);
            });
            //Devin
            eventDispatcher.register(Sentinel.Constants.EVENTS.CREATE_LANDMARK, this, function (args) {
                var mypage = "../GeoZone_Landmarks/frmlandmark.aspx?create=1&x=" + createlandmarkloc.lon.toString() + "&y=" + createlandmarkloc.lat.toString();
                parent.window.location.href = mypage;
                //                var myname = '';
                //                var w = 700;
                //                var h = 350;
                //                var winl = (screen.width - w) / 2;
                //                var wint = (screen.height - h) / 2;
                //                winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
                //                win = window.open(mypage, myname, winprops)
                //                if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            });
            eventDispatcher.register(Sentinel.Constants.EVENTS.SEARCH, this, function (args) {
                var url = window.location.href;
                var i_wz1 = url.lastIndexOf("/");
                var i_wz2 = url.lastIndexOf(".aspx");
                //alert(url.substring(i_wz1 + 1, i_wz2 + 5));
                //var mypage = "../Map/frmMapSearch.aspx?frm='../MapNew/" + url.substring(i_wz1 + 1, i_wz2 + 5) + "'";
                var mypage = "../Map/frmMapSearch.aspx?frm='../MapNew/" + url.substring(i_wz1 + 1) + "'";
                var myname = '';
                var w = 580;
                var h = 450;
                var winl = (screen.width - w) / 2;
                var wint = (screen.height - h) / 2;
                winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
                win = window.open(mypage, myname, winprops)
                if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            });

            // ************************************************************************************
            // final init stage

            // fetch data for initial view
            this.refreshGeoAssetCache();

            //ByDevin
            this.ShowOneLandMark = function (landmark) {
                try {
                    map.setZoomIndex(Sentinel.Constants.ASSET_DEFAULT_ZOOM_INDEX, false);
                    var balloonInfo =
                            "<span class=\"balloonBlock\">" +
                            "<p><B>" + MapLandMarkText + "</B>" + landmark.desc + "</p>" +
                            "<p><B>" + MapLandMarkRadiusText + "</B>" + landmark.rad + "</p>" +
                            "</span>";

                    //var iconAndLabel = "<div class=\"elementIcon\"><img src=\"" +
                    //    Sentinel.Constants.LANDMARK_ICONS[landmark.type] + "\"/><span class=\"text\">" + landmark.desc + "</span></div>";
                    //devin
                    var landmarklatLon = new LatLon(parseFloat(landmark.lat), parseFloat(landmark.lon));
                    var iconAndLabel = "";
                    if (landmark.desc != '')
                       iconAndLabel = "<div class=\"elementIcon\"><table cellspacing='0' cellpadding='0' ><tr  ><td><img src=\"../Bigicons/" +
                            landmark.icon + "\"/></td><td style='color:black'><b>-</b></td><td><span class=\"text\">" + landmark.desc + "</span></td></tr><table></div>";
                    else
                        iconAndLabel = "<div class=\"elementIcon\"><img src=\"../Bigicons/" +
                            landmark.icon + "\"/></div>";

                    var obj = new DivObject({
                        id: "landmarkLayer_" + landmark.id,
                        anchorPoint: new Point(0.5, 0.5),
                        dragEnabled: false,
                        layer: this.landmarkLayer,
                        location: landmarklatLon,
                        balloonConfig: {
                            content: balloonInfo,
                            skin: BalloonSkin.standard,
                            behavior: Balloon.HOVER_ACTIVE
                        },
                        innerHTML: iconAndLabel,
                        size: new Size(16, 16) // size of the icon only
                    });

                    try {
                        if (landmark.rad != '0' && landmark.rad != '-1') {
                            var newZone = null;


                            newZone = new CircleFence({
                                id: "landmark_circle_" + landmark.id,
                                parent: this.map,
                                center: landmarklatLon,
                                radius: parseFloat(landmark.rad),
                                units: DistanceUnit.METERS,
                                zIndex: Sentinel.Constants.LAYER_ZINDEX.LANDMARK_CIRCLE
                            });


                            if (newZone != null) {
                                newZone.setVisibility(1);
                                //this.landmarkFenceCollection.add(newZone);
                                this.map.setHeight(this.map.getHeight() + 1);
                                this.map.setHeight(this.map.getHeight() - 1);

                            }
                        }
                        else {
                            if (landmark.rad == '-1') {
                                try {
                                    if (landmark.coords != null && landmark.coords.length > 2) {
                                        var points = [];
                                        landmark.coords = eval(landmark.coords);
                                        for (j in landmark.coords) {
                                            points.push(new LatLon(parseFloat(landmark.coords[j][0]), parseFloat(landmark.coords[j][1])));
                                        }

                                        newZone = new PolygonFence({
                                            id: "landmark_polygon_" + landmark.id,
                                            parent: this.map,
                                            points: points,
                                            zIndex: Sentinel.Constants.LAYER_ZINDEX.LANDMARK_CIRCLE
                                        });
                                        if (newZone != null) {
                                            newZone.setVisibility(1);
                                            this.centerMap(points);
                                        }
                                    }
                                }
                                catch (ex) { }
                            }
                        }
                    }
                    catch (err) { }
                    var latLonLocations = new Array();
                    latLonLocations.push(landmarklatLon);
                    this.centerMap(latLonLocations);
                } catch (e) {
                    console.error("Failed to display landmark, cause: " + e.message);
                }
            }

            this.ShowOneGeozone = function (geozones) {
                this.showGeozone(geozones);
                if (geozones[0].coords != null) {
                    var points = new Array();
                    for (j in geozones[0].coords) {
                        points.push(new LatLon(parseFloat(geozones[0].coords[j][0]), parseFloat(geozones[0].coords[j][1])));
                    }
                    this.centerMap(points);
                }
            }


            this.ShowSingleTrip = function (point, i) {
                if (point == null) {
                    return;
                }

                var isTrail = false;

                try {

                    var headingStr = Dictionary['heading_' + MathUtil.bearingToCompass(point.head)];
                    var statStr = '';
                    var balloonInfo =
                            "<span class=\"balloonBlock\">" + point.toolTip + "</span>";


                    var latLon = new LatLon(parseFloat(point.lat), parseFloat(point.lon));
                    this.loc.push(latLon);

                    var iconAndLabel = "<div class=\"elementIcon\"><img src=\"../Bigicons/" +
                            point.icon + "\"/></div>";

                    if (point.trail == 1) isTrail = true;

                    new DivObject({
                        id: "assetLayer_" + point.id + "_" + i,
                        anchorPoint: new Point(0.5, 0.5),
                        dragEnabled: false,
                        layer: this.assetLayer,
                        location: latLon,
                        balloonConfig: {
                            content: balloonInfo,
                            skin: BalloonSkin.standard,
                            behavior: Balloon.HOVER_ACTIVE
                        },
                        innerHTML: iconAndLabel,
                        size: new Size(16, 16) // size of the icon only
                    });

                } catch (e) {
                    console.error("Failed to display asset, cause: " + e.message);
                }

            };

            this.BeginShowLandmark = function () {
                this.landmarkLayer.clear();
                this.landmarkFenceCollection.clear();
            };


            this.BeginShowGeozone = function () {
                this.geozoneFenceCollection.clear();
            };

            this.showSingleLandmark = function (landmark) {

                try {
                    var balloonInfo =
                            "<span class=\"balloonBlock\">" +
                            "<p><B>" + MapLandMarkText + "  </B>" + landmark.desc + "</p>" +
                            "</span>";

                    var landmarklatLon = new LatLon(parseFloat(landmark.lat), parseFloat(landmark.lon));
                    var iconAndLabel = "";
                    //                    if (!isLabel)
                    //                        iconAndLabel = "<div class=\"elementIcon\"><table cellspacing='0' cellpadding='0' ><tr  ><td><img src=\"" +
                    //                            landmark.icon + "\"/></td><td style='color:black;visibility: hidden;' id='lank_mark_label' ><b>-</b></td><td id='lank_mark_label'  style='visibility: hidden;'><span class=\"text\">" + landmark.desc + "</span></td></tr><table></div>";
                    //                    else
                    iconAndLabel = "<table cellspacing='0' cellpadding='0' class=\"elementIcon\" ><tr  ><td><img src=\"" +
                            landmark.icon + "\"/></td><td style='color:black' class='landmarkdash'  ><b>-</b></td><td ><span class=\"landmarktext\">" + landmark.desc + "</span></td></tr><table>";

                    var obj = new DivObject({
                        id: "landmarkLayer_" + landmark.id,
                        anchorPoint: new Point(0.5, 0.5),
                        dragEnabled: false,
                        layer: this.landmarkLayer,
                        location: landmarklatLon,
                        balloonConfig: {
                            content: balloonInfo,
                            skin: BalloonSkin.standard,
                            behavior: Balloon.HOVER_ACTIVE
                        },
                        innerHTML: iconAndLabel,
                        size: new Size(16, 16) // size of the icon only
                    });
                    //                    try {
                    //                        if (landmark.rad != '0') {
                    //                            var newZone = null;


                    //                            newZone = new CircleFence({
                    //                                id: "landmark_circle_" + landmark.id,
                    //                                //parent: this.map,
                    //                                layer: this.landmarkLayer,
                    //                                center: landmarklatLon,
                    //                                radius: parseFloat(landmark.rad),
                    //                                units: DistanceUnit.METERS,
                    //                                zIndex: Sentinel.Constants.LAYER_ZINDEX.LANDMARK_CIRCLE
                    //                            });


                    //                            if (newZone != null) {
                    //                                newZone.setVisibility(this._landmarkVisible ? 1 : 0);
                    //                                this.landmarkFenceCollection.add(newZone);
                    //                            }
                    //                        }
                    //                    }
                    //                    catch (err) { }
                    //                    this.landmarkPoints.push(landmarklatLon);

                } catch (e) {
                    console.error("Failed to display landmark, cause: " + e.message);
                }

            };

            this.showSingleGeozone = function (geozone) {
                var newZone = null;
                // prevent from showing a geozone that is being edited
                if (geozone.lat != 'N' && geozone.lon != 'N') {
                    var centerLatLon = new LatLon(parseFloat(geozone.lat), parseFloat(geozone.lon));

                    var balloonInfo =
                        "<span class=\"balloonBlock\">" +
                        "<B>Geozone: </B>" + geozone.desc + "" +
                        "</span>";
                    var iconAndLabel = "<img src=\"" +
                            Sentinel.Constants.GEOZONE_ICONS[geozone.type] + "\"/>";
                    var label = new DivObject({
                        id: "geozoneLabelLayer_" + geozone.id,
                        anchorPoint: new Point(0.5, 0.5),
                        dragEnabled: false,
                        layer: this.geozoneLabelLayer,
                        location: centerLatLon,
                        balloonConfig: {
                            content: balloonInfo,
                            skin: BalloonSkin.standard,
                            behavior: Balloon.HOVER_ACTIVE
                        },
                        innerHTML: iconAndLabel,
                        size: new Size(16, 16) // size of the icon only
                    });
                }

                return;
                geozone.coords = eval(geozone.coords);
                if (geozone.coords.length == 1) {
                    // circular geozone


                    var radius = MathUtil.distance(centerLatLon,
                            new LatLon(parseFloat(geozone.coords[0][0]), parseFloat(geozone.coords[0][1])), DistanceUnit.METERS);

                    newZone = new CircleFence({
                        id: "geozone_" + geozone.id,
                        parent: this.map,
                        center: centerLatLon,
                        radius: radius,
                        units: DistanceUnit.METERS,
                        zIndex: Sentinel.Constants.LAYER_ZINDEX.GEOZONE
                    });

                } else if (geozone.coords.length == 2) {
                    // rectangular geozone

                    newZone = new RegionShadeLayer({
                        id: "geozone_" + geozone.id,
                        parent: this.map,
                        nw: new LatLon(parseFloat(geozone.coords[0][0]), parseFloat(geozone.coords[0][1])),
                        se: new LatLon(parseFloat(geozone.coords[1][0]), parseFloat(geozone.coords[1][1])),
                        zIndex: Sentinel.Constants.LAYER_ZINDEX.GEOZONE
                    });

                } else if (geozone.coords.length > 2) {
                    // polygonal geozone

                    var points = [];
                    for (j in geozone.coords) {
                        var mypoint = new LatLon(parseFloat(geozone.coords[j][0]), parseFloat(geozone.coords[j][1]));
                        points.push(mypoint);
                        this.geozonePoints.push(mypoint);
                    }

                    newZone = new PolygonFence({
                        id: "geozone_" + geozone.id,
                        parent: this.map,
                        points: points,
                        zIndex: Sentinel.Constants.LAYER_ZINDEX.GEOZONE
                    });
                }
                // set the initial visibility of the geozone
                if (newZone != null) {
                    newZone.setVisibility(this._geozoneVisible ? 1 : 0);

                    this.geozoneFenceCollection.add(newZone);
                }
            };

        } // end Map class

    }

} ();
