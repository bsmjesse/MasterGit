var map;

function init() {

    $('.olLayerGooglePoweredBy ').remove();

    map = new OpenLayers.Map({
        div: "map",
        allOverlays: true
    });

    var osm = new OpenLayers.Layer.OSM();
    var gmap = new OpenLayers.Layer.Google("Google Streets", {visibility: true});

    // note that first layer must be visible
    map.addLayers([gmap]);

    map.addControl(new OpenLayers.Control.LayerSwitcher());
    map.zoomToMaxExtent();
    
    google.maps.event.addListener(gmap, 'zoom_changed', function() {
        $('.olLayerGooglePoweredBy ').remove();
  });

}
