
var map, marker, popup;
    OpenLayers.Control.Click = OpenLayers.Class(OpenLayers.Control, {
        defaultHandlerOptions: {
            'single': true,
            'double': false,
            'pixelTolerance': 0,
            'stopSingle': false,
            'stopDouble': false
        },

        initialize: function (options) {
            this.handlerOptions = OpenLayers.Util.extend(
                {}, this.defaultHandlerOptions
            );
            OpenLayers.Control.prototype.initialize.apply(
                this, arguments
            );
            this.handler = new OpenLayers.Handler.Click(
                this, {
                    'click': this.trigger
                }, this.handlerOptions
            );
        },

        trigger: function (e) {
            var lonlat = map.getLonLatFromPixel(e.xy);
            alert("You clicked near " + lonlat.lat + " N, " +
                +lonlat.lon + " E");
        }

    });
    function init() {
        map = new OpenLayers.Map('map');

        var ol_wms = new OpenLayers.Layer.WMS("OpenLayers WMS",
            "http://vmap0.tiles.osgeo.org/wms/vmap0?", { layers: 'basic' });

        var jpl_wms = new OpenLayers.Layer.WMS("NASA Global Mosaic",
            "http://t1.hypercube.telascience.org/cgi-bin/landsat7",
            { layers: "landsat7" });

        jpl_wms.setVisibility(false);

        map.addLayers([ol_wms, jpl_wms]);
        map.addControl(new OpenLayers.Control.LayerSwitcher());
        map.setCenter(new OpenLayers.LonLat(-79.571944,43.721944), 10);
        //map.zoomToMaxExtent();

        //var click = new OpenLayers.Control.Click();
        //map.addControl(click);
        //click.activate();

    }
    
    $(document).ready(function () {

        $('#iButton').click(function () {
            
            //map.panTo(lonlat);
            var markers = new OpenLayers.Layer.Markers("Markers");
            map.addLayer(markers);

            var size = new OpenLayers.Size(21, 25);            
            var offset = new OpenLayers.Pixel(-(size.w / 2), -size.h);
            var icon = new OpenLayers.Icon('http://www.openlayers.org/dev/img/marker.png', size, offset);
            var lonlat = new OpenLayers.LonLat(-77.0063747406006, 38.8801287024414 );
            markers.addMarker(new OpenLayers.Marker(lonlat, icon));

            var halfIcon = icon.clone();
            markers.addMarker(new OpenLayers.Marker(new OpenLayers.LonLat(0, 45), halfIcon));

            marker = new OpenLayers.Marker(new OpenLayers.LonLat(90, 10), icon.clone());
            marker.setOpacity(0.2);
            marker.events.register('mousedown', marker, function (evt) { alert(this.icon.url); OpenLayers.Event.stop(evt); });
            markers.addMarker(marker);


            var newl = new OpenLayers.Layer.Text("text", { location: "/Content/textfile.txt" });
            map.addLayer(newl);

            popup = new OpenLayers.Popup("chicken",
                       new OpenLayers.LonLat(5, 40),
                       new OpenLayers.Size(200, 200),
                       "example popup",
                       true);

            map.addPopup(popup);
            
            map.addControl(new OpenLayers.Control.LayerSwitcher());
            map.zoomToMaxExtent();
        });
    }
    );