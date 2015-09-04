
var layerURL = "http://services.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer";
var map = null;
var mycontrols = [];
var strategy = null;
var depotLayer = null;
var stationLayer = null;
var DepotsMarkers = null;
var StationMarkers = null
var SearchMarkers = null;
var selectedFeature = null; 
var selectedPopup = null;

var StationCtrl = function ($scope, $modal, $http, $timeout, saService) {
    var EditDepotContent = "<iframe src='frmEditStationMap.aspx' width='100%' height='100%'></iframe>";
    $scope.InitController = function () {
        $scope.DepotList = [];
        $scope.StationList = [];
        InitMap();
        GetDepots();
        GetStations();
        $scope.DepotStatus = "Init";
        $scope.StationStatus = "Init";
        $scope.Search = { Depot: "", Station: "" };
        $scope.SearchMarker = null;
    };

    $scope.InitData = function (depot) {
        $scope.InitController();
        saService.SetSchedule(null);
        if (depot != null)
            $scope.GetGroupListByDepotId(depot.StationId);
    };

    $scope.SelectedItem = function (itemId) {
        var item = GetItemById(itemId);
        if (item == null) return;
        var center = transformCoords(item.longitude, item.latitude);
        if (!map.getExtent().containsLonLat(center))
            map.setCenter(center);

        angular.forEach($scope.DepotList, function (row) {
            row.IsSelected = false;
        });
        angular.forEach($scope.StationList, function (row) {
            row.IsSelected = false;
        });
        item.IsSelected = true;
    };

    $scope.EditCancel = function () {
        ClearOldNewFeature();
    };

    $scope.SaveDepot = function () {
        ClearOldNewFeature();
        $scope.DepotStatus = "Loading";
        $timeout(function () {
            GetDepots();
        }, 15000);
    };

    $scope.DeleteDepot = function () {
        ClearOldNewFeature();
        GetDepots();
    };

    $scope.SaveStation = function () {
        ClearOldNewFeature();
        $scope.StationStatus = "Loading";
        $timeout(function () {
            GetStations();
        }, 15000); //delay 8 seconds
    };

    $scope.DeleteStation = function () {
        ClearOldNewFeature();
        GetStations();
    };

    $scope.DepotCompare = function (item) {
        if ($scope.Search.Depot == '') return true;
        var des = $scope.Search.Depot.toLowerCase();
        if (des != '' && item.Name.toLowerCase().indexOf(des) < 0 && item.StationNumber.indexOf(des) < 0) return false;
        return true;
    };

    $scope.StationCompare = function (item) {
        if ($scope.Search.Station == '') return true;
        var des = $scope.Search.Station.toLowerCase();
        if (des != '' && item.Name.toLowerCase().indexOf(des) < 0 && item.StationNumber.indexOf(des) < 0) return false;
        return true;
    };

    function GetItemById(itemId) {
        var item = null;
        angular.forEach($scope.DepotList, function (row) {
            if (row.StationId == itemId)
                item = row;
        });
        angular.forEach($scope.StationList, function (row) {
            if (row.StationId == itemId)
                item = row;
        });
        return item;
    };

    function GetDepots() {
        $scope.DepotStatus = "Loading";
        $http.post('ScheduleData.ashx?Req=GetDeptList').success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                $scope.DepotStatus = "NoData";
                return;
            }
            $scope.DepotList = data.Object;
            $scope.DepotStatus = "Data";
            var features = [];
            DepotsMarkers.clearMarkers();
            angular.forEach($scope.DepotList, function (row) {
                BuildDepotPoint(row);
                DepotsMarkers.addMarker(row.Marker);
                features.push(row.Vector);
            });
            depotLayer.removeAllFeatures();
            depotLayer.addFeatures(features);
        });
    };

    function GetStations() {
        $scope.StationStatus = "Loading";
        $http.post('ScheduleData.ashx?Req=GetStationList').success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                $scope.StationStatus = "NoData";
                return;
            }
            $scope.StationList = data.Object;
            $scope.StationStatus = "Data";
            var markers = [];
            var features = [];
            StationMarkers.clearMarkers();
            angular.forEach($scope.StationList, function (row) {
                BuildStationPoint(row);
                StationMarkers.addMarker(row.Marker);
                features.push(row.Vector);
            });
            stationLayer.removeAllFeatures();
            stationLayer.addFeatures(features);
        });
    };

    function AddSearchMarker(lon, lat) {
        if ($scope.SearchMarker != null)
            SearchMarkers.removeMarker($scope.SearchMarker);
        var point = transformCoords(lon, lat);
        var size = new OpenLayers.Size(56, 58);
        var offset = new OpenLayers.Pixel(-10, -33);
        var icon = new OpenLayers.Icon('../images/locator.png', size, offset);
        $scope.SearchMarker = new OpenLayers.Marker(point, icon);
        SearchMarkers.addMarker($scope.SearchMarker);
        map.setCenter(point, 16);
    };

    $scope.SearchAddress = function () {
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ "address": $('#txtSearchAddress').val() }, function (results, status) {
            //addressTryTimes++;
            if (status == google.maps.GeocoderStatus.OK) {
                searchLat = results[0].geometry.location.lat();
                searchLon = results[0].geometry.location.lng();
                AddSearchMarker(searchLon, searchLat);
            }
        });
    };

    function InitMapControl() {
        strategy = new OpenLayers.Strategy.Cluster();
        strategy.distance = 40;
        strategy.threshold = 3;
        depotLayer = InitVectorLayer("Depots");
        depotLayer.events.register("visibilitychanged", null, function () {
            if (DepotsMarkers == null) return;
            DepotsMarkers.setVisibility(depotLayer.getVisibility());
        });
        stationLayer = InitVectorLayer("Stations");
        stationLayer.events.register("visibilitychanged", null, function () {
            if (StationMarkers == null) return;
            StationMarkers.setVisibility(stationLayer.getVisibility());
        });
        map.addLayer(depotLayer);
        map.addLayer(stationLayer);

        selectControl = new OpenLayers.Control.SelectFeature([depotLayer, stationLayer], {
            clickout: true, toggle: false,
            multiple: false, hover: false, onSelect: onFeatureSelect, onUnselect: onFeatureUnselect
        });
        mycontrols = {
            select: selectControl,
            depot: new OpenLayers.Control.DrawFeature(depotLayer, OpenLayers.Handler.RegularPolygon, { handlerOptions: { sides: 40} }),
            station: new OpenLayers.Control.DrawFeature(stationLayer, OpenLayers.Handler.RegularPolygon, { handlerOptions: { sides: 40} })
        };
        for (var key in mycontrols) {
            map.addControl(mycontrols[key]);
        }
        mycontrols.select.activate();
        mycontrols.depot.deactivate();
        mycontrols.station.deactivate();

        DepotsMarkers = new OpenLayers.Layer.Markers("Depots Markers", { displayInLayerSwitcher: false });
        map.addLayer(DepotsMarkers);
        StationMarkers = new OpenLayers.Layer.Markers("Stations Markers", { displayInLayerSwitcher: false });
        map.addLayer(StationMarkers);
        SearchMarkers = new OpenLayers.Layer.Markers("Search Markers", { displayInLayerSwitcher: false });
        //SearchMarkers = new OpenLayers.Layer.Markers("Search Markers");
        map.addLayer(SearchMarkers);

        depotLayer.events.on({
            "sketchcomplete": CreateDepot
        });
        stationLayer.events.on({
            "sketchcomplete": CreateStation
        });
    };

    function onFeatureSelect(feature) {
        var type;
        if (feature.type == "depot")
            type = 2;
        else
            type = 1;
        ShowPopup(feature);
        if (feature.stationId != -1)
            $scope.$apply(function () {
                $scope.SelectedItem(feature.stationId);
            });
    };

    function onFeatureUnselect(feature) {
        ClearOldNewFeature();
    };

    function CreateStation(event) {
        if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {
            var feature = event.feature;
            feature.type = "station";
            feature.action = "add";
            ShowPopup(feature);
        }
    };

    function CreateDepot(event) {
        if (event.feature != null && event.feature.geometry != null && event.feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Polygon") {
            var feature = event.feature;
            feature.type = "depot";
            feature.action = "add";
            ShowPopup(feature);
        }
    }
    function ShowPopup(feature) {
        ClearOldNewFeature();
        var latlon = feature.geometry.getBounds().getCenterLonLat();
        latlon.transform(new OpenLayers.Projection("EPSG:900913"), new OpenLayers.Projection("EPSG:4326"));
        var centerPoint = latlon.lat + "," + latlon.lon;
        txtX = latlon.lon;
        txtY = latlon.lat;
        latlon.transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913"));

        var c = feature.geometry.components[0];
        var pointSets = "";
        var mapSearchPointSets = "";
        var centerPoint = "";
        var polygonRadius = 0;
        var point1 = new OpenLayers.Geometry.Point(latlon.lon, latlon.lat);
        for (var i = 0; i < c.components.length - 1; i++) {
            var point2 = new OpenLayers.Geometry.Point(c.components[i].x, c.components[i].y);
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

        var popupheight = 330;
        popupheight = 300;
        var area = feature.geometry.getGeodesicArea(new OpenLayers.Projection("EPSG:900913"));
        var radius = Math.floor(0.565352 * Math.sqrt(area));
        var type;
        if (feature.type == "depot")
            type = 2;
        else
            type = 1;
        var content;
        if (feature.action == "add")
            content = "<iframe src='frmEditStationMap.aspx?type=" + type + "&Action=add&radius=" + radius + "&lon=" + txtX + "&lat=" + txtY + "' width='100%' height='100%'></iframe>";
        else
            content = "<iframe src='frmEditStationMap.aspx?type=" + type + "&Action=edit&stationId=" + feature.stationId + "&radius=" + radius + "&lon=" + txtX + "&lat=" + txtY + "' width='100%' height='100%'></iframe>";
        selectedFeature = feature;

        popup = new OpenLayers.Popup.FramedCloud("polygonformpopup",
                                feature.geometry.getBounds().getCenterLonLat(),
                                null,
                                content,
                                null, false, OnCreatePopupClosed);
        feature.popup = popup;
        selectedPopup = popup;
        map.addPopup(popup);

        popup.setSize(new OpenLayers.Size(350, popupheight));

        currentEditFeature = selectedFeature;
        currentEditPopup = popup;
    };

    function ClearOldNewFeature() {
        if (selectedPopup != null) {
            map.removePopup(selectedPopup);
            selectedPopup = null;
        }
        if (selectedFeature != null && selectedFeature.action == "add") {
            if (selectedFeature.type == "depot")
                depotLayer.removeFeatures(selectedFeature);
            else
                stationLayer.removeFeatures(selectedFeature);
            selectedFeature = null;
        }
        mycontrols.select.unselectAll();
    };

    function OnCreatePopupClosed(evt) {
        ClearOldNewFeature();
    };

    function InitDepot() {
        return { StationId: -1, Name: "Test new", LandmarkId: -1, StationNumber: 'fff' };
    };

    function InitGoogleAutoComplete() {
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

        google.maps.event.addListener(autocomplete, 'place_changed', function () {
            var place = autocomplete.getPlace();

            if (!place.geometry) {
                searchLat = 0;
                searchLon = 0;
                return;
            }
            var txt = $('#txtSearchAddress').val();
            var numb = txt.match(/\d/g);
            $scope.SearchLonLat = new OpenLayers.LonLat(place.geometry.location.lng(), place.geometry.location.lat());
            AddSearchMarker(place.geometry.location.lng(), place.geometry.location.lat());
        });
    }

    function InitMap() {
        var renderer;
        try {
            if (ifShowArcgis && !ISSECURE) {

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
            }),
            new OpenLayers.Control.ScaleLine({ 'geodesic': true }),
            new OpenLayers.Control.MousePosition(),
            new OpenLayers.Control.KeyboardDefaults()
          ]
        };

            // create OSM layer
            if (ifShowTheme1 && !ISSECURE)
                var mapnik = new OpenLayers.Layer.OSM("Theme 1");

            if (ifShowTheme2 && !ISSECURE) {
                var cloudmade = new OpenLayers.Layer.CloudMade("Theme 2",
              {
                  key: '8ee2a50541944fb9bcedded5165f09d9'
              });
            }

            if (ifShowGoogleStreets) {
                var gmap = new OpenLayers.Layer.Google(
              "Google Streets",
              {
                  sphericalMercator: true
              });
            }

            if (ifShowGoogleHybrid) {
                var ghyb = new OpenLayers.Layer.Google(
          "Google Hybrid",
          {
              type: google.maps.MapTypeId.HYBRID, sphericalMercator: true, numZoomLevels: 20, disableTilt: true
          }
          );
            }

            // create Virtual Earth layers
            if (ifShowBingRoads && !ISSECURE) {
                var veroad = new OpenLayers.Layer.VirtualEarth(
          "Bing Roads",
          {
              'type': VEMapStyle.Road, sphericalMercator: true
          });
            }

            if (ifShowBingHybrid && !ISSECURE) {
                var vehyb = new OpenLayers.Layer.VirtualEarth(
          "Bing Hybrid",
          {
              'type': VEMapStyle.Hybrid, sphericalMercator: true
          });
            }

            map = new OpenLayers.Map('stationMap', options);

            var apiKey = "AjfXbh4yW0IPCW0jQaXRxfZwLdAIGJjXk0v_OhoZo2gs6rygaryV1aCihwPQ4F9l";

            highlightLayer = new OpenLayers.Layer.Vector("Highlighted Features",
        {
            displayInLayerSwitcher: false,
            isBaseLayer: false
        });


            if (ifShowTheme1 && !ISSECURE)
                map.addLayer(mapnik);
            if (ifShowTheme2 && !ISSECURE)
                map.addLayer(cloudmade);
            if (ifShowArcgis && !ISSECURE)
                map.addLayer(arcgisLayer);
            if (ifShowGoogleStreets)
                map.addLayer(gmap);
            if (ifShowGoogleHybrid)
                map.addLayer(ghyb);
            if (ifShowBingRoads && !ISSECURE)
                map.addLayer(veroad);
            if (ifShowBingHybrid && !ISSECURE)
                map.addLayer(vehyb);

            if (ifShowNavteq) {
                var Navteq = new OpenLayers.Layer.Navteq("Navteq", "normal.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
                map.addLayer(Navteq);
            }

            if (ifShowNavteqHybrid) {
                var NavteqHybrid = new OpenLayers.Layer.Navteq("Navteq Hybrid", "hybrid.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw");
                map.addLayer(NavteqHybrid);
            }

            if (map.layers.length > 1 && map.layers[0].name.toLowerCase().indexOf("google") != -1) {
                map.setBaseLayer(map.layers[1]);
                map.setBaseLayer(map.layers[0]);
            }
        }
        catch (err) {
        }
        if (DefaultMapCenter != null &&
                 DefaultMapZoomLevel != null) {
            map.setCenter(DefaultMapCenter, DefaultMapZoomLevel);
        }
        else
            map.zoomTo(4);
        InitMapControl();
        InitGoogleAutoComplete();
    };

    $scope.InitController();
};



function InitVectorLayer(layerName)
{
    var renderer = OpenLayers.Util.getParameters(window.location.href).renderer;
    renderer = (renderer) ? [renderer] : OpenLayers.Layer.Vector.prototype.renderers;
    var vectorLayer = new OpenLayers.Layer.Vector(layerName, {
        styleMap: new OpenLayers.StyleMap({
            "default": GetFeatureStyle(),
            "select": {
                fillColor: "#8aeeef",
                strokeColor: "#32a8a9"
            }
        }),
        renderers: renderer
    });

    return vectorLayer;
};

function BuildDepotPoint(depot){
    var point = transformCoords(depot.longitude, depot.latitude);
    var size = new OpenLayers.Size(12,20);
    var offset = new OpenLayers.Pixel(-(size.w/2), -size.h);
    var icon = new OpenLayers.Icon('img/mm_20_red.png',size,offset);
    depot.Marker = new OpenLayers.Marker(point,icon);
    var vector = CircleToVector(depot);
    vector.type = "depot";
    vector.action = "edit";
    vector.stationId = depot.StationId
    depot.Vector = vector;
    depot.IsSelected = false;
};

function BuildStationPoint(station){
    var point = transformCoords(station.longitude, station.latitude);
    var size = new OpenLayers.Size(12,20);
    var offset = new OpenLayers.Pixel(-(size.w/2), -size.h);
    var icon = new OpenLayers.Icon('img/mm_20_blue.png',size,offset);
    station.Marker = new OpenLayers.Marker(point,icon);
    var vector = CircleToVector(station);
    vector.type = "station";
    vector.action = "edit";
    vector.stationId = station.StationId
    station.Vector = vector;
    station.IsSelected = false;
};

function CircleToVector(circle){
    var proj = new OpenLayers.Projection("EPSG:4326");
    var radius = parseInt(circle.radius);
    var point = new OpenLayers.LonLat(circle.longitude, circle.latitude);
    point.transform(proj, map.getProjectionObject());
    var origin = new OpenLayers.Geometry.Point(point.lon, point.lat);
    var vector = OpenLayers.Geometry.Polygon.createRegularPolygon(origin, radius / Math.cos(circle.latitude * (Math.PI / 180)), 40, 0);
    var feature = new OpenLayers.Feature.Vector(vector);
    return feature;
};
function transformCoords(lon, lat) {
    return new OpenLayers.LonLat(lon, lat)
   .transform(
   new OpenLayers.Projection("EPSG:4326"),
   map.getProjectionObject()
   );
}
function toggleControl(controlId, action) {
    if (controlId == 'none') {
        //strategy.activate();
//        vectors.removeFeatures(geoLandmarkFeatures.features);
//        vectors.addFeatures(geoLandmarkFeatures);

//        dblclick.deactivate();
//        dblclick.activate();

        var c = map.getControlsByClass("OpenLayers.Control.KeyboardDefaults");
        c[0].activate();
        FireClick('bt_EditCancel');
        mycontrols.select.activate();
        mycontrols.depot.deactivate();
        mycontrols.station.deactivate();
    }
    else {
        //strategy.deactivate();

//        vectors.removeAllFeatures();
//        vectors.addFeatures(geoLandmarkFeatures);

        var c = map.getControlsByClass("OpenLayers.Control.KeyboardDefaults");
        c[0].deactivate();
        FireClick('bt_EditCancel');
        for (key in mycontrols) {
            var control = mycontrols[key];
            if (controlId == key && action) {
                control.activate();
            } else {
                control.deactivate();
            }
        }
    }
};

function GetFeatureStyle() {
    return new OpenLayers.Style({
        pointRadius: "${radius}",
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
                return (feature.type == "depot") ? '#FF271C' : '#3A3DFF';
            },
            opacity: function (feature) {
                return feature.cluster ? 0.8 : 0.3;
            },
            strokeDashstyle: function (feature) {
                return "solid";    //res tbd                
            }
        }
    });
};

