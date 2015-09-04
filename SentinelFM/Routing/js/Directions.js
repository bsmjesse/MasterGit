/*	Set authentication token and appid 
*	WARNING: this is a demo-only key
*	please register on http://api.developer.nokia.com/ 
*	and obtain your own API key 
*/
nokia.Settings.set("appId", "_peU-uCkp-j8ovkzFGNU");
nokia.Settings.set("authenticationToken", "gBoUkAMoxoqIWfxWA5DuMQ");
(document.location.protocol == "https:") && nokia.Settings.set("secureConnection", "force");

// Get the DOM node to which we will append the map
var mapContainer = document.getElementById("mapContainer");
// Create a map inside the map container DOM node
var map = new nokia.maps.map.Display(mapContainer, {
    // Initial center and zoom level of the map
    center: [52.51676875, 13.39201495],
    zoomLevel: 18,
    // We add the behavior component to allow panning / zooming of the map
    components: [new nokia.maps.map.component.Behavior()]
}),
	router = new nokia.maps.routing.Manager(); // create a route manager;

// The function onRouteCalculated  will be called when a route was calculated
var onRouteCalculated = function (observedRouter, key, value) {
    if (value == "finished") {
        var routes = observedRouter.getRoutes(),
            container = new nokia.maps.map.Container(),
            route = routes[0],
            waypoints = route.waypoints,
            i, length = waypoints.length;

        // Add route polyline to the container
        container.objects.add(new nokia.maps.map.Polyline(route.shape, {
            pen: new nokia.maps.util.Pen({
                lineWidth: 5,
                strokeColor: "#AB7A8C"
            })
        }));

        // Add container to the map
        map.objects.add(container);

        // Iterate through all waypoints and add them to the container
        for (i = 0; i < length; i++) {
            //
            container.objects.add(new nokia.maps.map.StandardMarker(waypoints[i].originalPosition, {
                text: String.fromCharCode(65 + i) //65 is a char code for "A"
            }));
        }
        //Zoom to the bounding box of the route
        map.zoomTo(container.getBoundingBox(), false, "default");
    } else if (value == "failed") {
        alert("The routing request failed.");
    }
};

/* We create on observer on router's "state" property so the above created
 * onRouteCalculated we be called once the route is calculated
 */
router.addObserver("state", onRouteCalculated);

// Create waypoints
var waypoints = new nokia.maps.routing.WaypointParameterList();
waypoints.addCoordinate(new nokia.maps.geo.Coordinate(43.657922, -79.399892));
waypoints.addCoordinate(new nokia.maps.geo.Coordinate(43.678006, -79.443151));
waypoints.addCoordinate(new nokia.maps.geo.Coordinate(43.709222, -79.505593));

/* Properties such as type, transportModes, options, trafficMode can be
 * specified as second parameter in performing the routing request.
 * 
 * See for the mode options the "nokia.maps.routing.Mode" section in the developer's guide
 */
var modes = [{
    type: "fastest",
    transportModes: ["car"],
    trafficMode: "disabled",
    options: ""
}];

// Trigger route calculation after the map emmits the "displayready" event
map.addListener("displayready", function () {
    router.calculateRoute(waypoints, modes);
}, false);

/* We create a UI notecontainer for example description
 * NoteContainer is a UI helper function and not part of the Nokia Maps API
 * See exampleHelpers.js for implementation details 
 */
var noteContainer = new NoteContainer({
    id: "routingUi",
    parent: document.getElementById("uiContainer"),
    title: "A to C via B routing",
    content:
		'<p>This example shows how a route manager can be used to calculate a route from: <br/>' +
		'<b>Westin Grand hotel, Berlin</b> <br />(52.516222,13.388900) <br/>' +
		'to: <br/><b>Staatsoper, Berlin</b> <br /> (52.517175, 13.395129)' +
		'<br />via <b>Deutsche Guggenheim</b> (52.516650, 13.391368) and push routing result to the map.</p>' +
		'<p>Routing manager supports various options in making a routing request like ' +
		'setting type, transport mode and routes to avoid.</p>'
});

