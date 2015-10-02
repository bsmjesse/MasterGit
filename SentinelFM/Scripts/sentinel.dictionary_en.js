
Dictionary:Dictionary = function () {

    return {

        // compass descriptions
        heading_N: "N",
        heading_NE: "NE",
        heading_E: "E",
        heading_SE: "SE",
        heading_S: "S",
        heading_SW: "SW",
        heading_W: "W",
        heading_NW: "NW",

        // misc labels
        heading: "Heading",
        status: "Status",
        vehicle: "Vehicle",
        speed: "Speed",
        time: "Time",
        assets: "Assets",
        landmark: "Landmark:",
        radius: "Radius:",

        landmark_fr: "Site:",
        radius_fr: "Rayon:",
        heading_fr: "Direction",
        status_fr: "Statut",
        vehicle_fr: "Véhicule",
        speed_fr: "Vitesse",
        time_fr: "Heure",
        assets_fr: "Assets",

        loading: "Loading",
        createLandmark: "Create landmark",
        search: "Search",

        createLandmark_fr: "ajouter un site",
        search_fr: "requête",


        drawNewGeozone: "Drawing a new geozone",
        editGeozone: "Editing geozone: ",
        editGeozoneInstructions: "Holding down the control (Ctrl) key, click to draw the geozone. Release the control key to pan the map.",
        editGeozoneInstructions_CIRCLE: "Click once to mark the center, then move the cursor and click again to define the radius.",
        editGeozoneInstructions_RECTANGLE: "Click at each of two corners, following the diagonal of the rectangle.",
        editGeozoneInstructions_POLYGON: "Click at each corner of the polygon, clockwise.",

        // messages, warnings and errors
        finishEditingGeozone: "A geozone is being edited. \nThis task must be completed before another geozone can be edited.",
        GeozoneMaxPointsReached: "A polygonal geozone cannot have more than 10 corners."

    };

} ();
