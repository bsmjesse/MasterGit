// API key for http://openlayers.org. Please get your own at
// http://bingmapsportal.com/ and use that instead.
var apiKey = "AqTGBsziZHIJYYxgivLBf0hVdrAk9mWO5cQcb8Yux8sW5M8c8opEC2lZqKR1ZZXf";

// initialize map when page ready
var map;
var sprintersLayer;
var followLayer;
var selectControl;
var gg = new OpenLayers.Projection("EPSG:4326");
var sm = new OpenLayers.Projection("EPSG:900913");


var init = function (onSelectFeatureFunction) {

    $('#map').html('');

    var vector = new OpenLayers.Layer.Vector(ResourceLandmarkGeozone, { displayInLayerSwitcher: false});
    sprintersLayer = new OpenLayers.Layer.Vector(decodeURI(ResourceVehicles), {
        styleMap: new OpenLayers.StyleMap({
            //externalGraphic: "img/mobile-loc.png",
            externalGraphic: "/mobile/content/images/CircleSelected.png",
            graphicOpacity: 1.0,
            graphicWidth: 32,
            graphicHeight: 32
            //,graphicYOffset: 0
        })
    });

    followLayer = new OpenLayers.Layer.Vector(decodeURI(ResourceFollow), {
        styleMap: new OpenLayers.StyleMap({
            //externalGraphic: "img/mobile-loc.png",
            externalGraphic: "${icon}",//"/mobile/content/images/CircleHistory.png",
            graphicOpacity: 1.0,
            graphicWidth: 32,
            graphicHeight: 32
            //,graphicYOffset: 0
        })
    });

    //var sprinters = getFeatures();
    //sprintersLayer.addFeatures(sprinters);

    selectControl = new OpenLayers.Control.SelectFeature([sprintersLayer, followLayer], {
        autoActivate:true,
        onSelect: onSelectFeatureFunction});

    var geolocate = new OpenLayers.Control.Geolocate({
        id: 'locate-control',
        geolocationOptions: {
            enableHighAccuracy: false,
            maximumAge: 0,
            timeout: 7000
        }
    });
    
        // create map
        map = new OpenLayers.Map({
            div: "map",
            theme: null,
            projection: sm,
            numZoomLevels: 18,
            //tileManager: new OpenLayers.TileManager(),
            controls: [
                new OpenLayers.Control.Attribution(),
                //new OpenLayers.Control.ZoomPanel(),
                new OpenLayers.Control.TouchNavigation({
                    dragPanOptions: {
                        enableKinetic: true
                    }
                }),
                geolocate//,
                //selectControl
            ],
            layers: [
                //new OpenLayers.Layer.Bing({
                //    key: apiKey,
                //    type: "Road",
                //    // custom metadata parameter to request the new map style - only useful
                //    // before May 1st, 2011
                //    metadataParams: {
                //        mapVersion: "v1"
                //    },
                //    name: "Bing Road",
                //    transitionEffect: 'resize'
                //}),
                //new OpenLayers.Layer.Bing({
                //    key: apiKey,
                //    type: "Aerial",
                //    name: "Bing Aerial",
                //    transitionEffect: 'resize'
                //}),
                //new OpenLayers.Layer.Bing({
                //    key: apiKey,
                //    type: "AerialWithLabels",
                //    name: "Bing Aerial + Labels",
                //    transitionEffect: 'resize'
                //}),
                new OpenLayers.Layer.Navteq("Map View", "normal.day", "v5HljYEynPujgUUkNmny", "x14y1MmQaoSerjNQKGsABw"),
                new OpenLayers.Layer.Google(
                      ResourceGoogleStreets,//"Google Streets",
                      {
                          sphericalMercator: true,
                          transitionEffect: 'resize'
                      }
                ),
                new OpenLayers.Layer.Google(
                    ResourceGoogleHybrid,//"Google Hybrid",
                    {
                        type: google.maps.MapTypeId.HYBRID, sphericalMercator: true, numZoomLevels: 20, disableTilt: true,
                        transitionEffect: 'resize'
                    }
                ),
                new OpenLayers.Layer.Google(
                    ResourceGoogleSatellite,//"Google Satellite",
                    {
                        type: google.maps.MapTypeId.SATELLITE, sphericalMercator: true, numZoomLevels: 20, disableTilt: true,
                    }
                    ),
                new OpenLayers.Layer.OSM("OpenStreetMap", null, {
                    transitionEffect: 'resize'
                }),
                vector,
                followLayer,
                sprintersLayer                
            ],
            center: new OpenLayers.LonLat(-8859313, 5415727),
            zoom: 4
        });

        map.addControl(selectControl);

        //followLayer.events.fallThrough = true;
        //sprintersLayer.events.fallThrough = true;
        
        if (iniVehicle != null && iniVehicle != undefined) {
            mapVehicle(iniVehicle.lon, iniVehicle.lat, 
                { 
                    BoxId: iniVehicle.attributes.BoxId, 
                    OriginDateTime: iniVehicle.attributes.OriginDateTime,
                    Speed: iniVehicle.attributes.Speed,
                    Status: iniVehicle.attributes.Status,
                    Location: iniVehicle.attributes.Location,
                    Description: iniVehicle.attributes.Description,
                    MyHeading: iniVehicle.attributes.MyHeading
            });
        }
        
    var style = {
        fillOpacity: 0.1,
        fillColor: '#000',
        strokeColor: '#f00',
        strokeOpacity: 0.6
    };
    geolocate.events.register("locationupdated", this, function(e) {
        vector.removeAllFeatures();
        vector.addFeatures([
            new OpenLayers.Feature.Vector(
                e.point,
                {},
                {
                    graphicName: 'cross',
                    strokeColor: '#f00',
                    strokeWidth: 2,
                    fillOpacity: 0,
                    pointRadius: 10
                }
            ),
            new OpenLayers.Feature.Vector(
                OpenLayers.Geometry.Polygon.createRegularPolygon(
                    new OpenLayers.Geometry.Point(e.point.x, e.point.y),
                    e.position.coords.accuracy / 2,
                    50,
                    0
                ),
                {},
                style
            )
        ]);
        map.zoomToExtent(vector.getDataExtent());        
    });    

};
