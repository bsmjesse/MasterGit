/*
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU AFFERO General Public License as published by
the Free Software Foundation; either version 3 of the License, or
any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
or see http://www.gnu.org/licenses/agpl.txt.
*/

// OSRM routing
// [management of routing requests and processing of responses]

// some variables
OSRM.GLOBALS.route = null;
OSRM.GLOBALS.markers = null;

OSRM.GLOBALS.dragging = null;
OSRM.GLOBALS.dragid = null;
OSRM.GLOBALS.pending = false;


OSRM.Routing = {

    // init routing data structures
    init: function () {
        // init variables	
        OSRM.GUI.setRoutingEngine(OSRM.DEFAULTS.ROUTING_ENGINE);

        OSRM.G.markers = new OSRM.Markers();
        OSRM.G.route = new OSRM.Route();
        OSRM.G.response = { via_points: [] };
        OSRM.G.cacheStart = null;
        OSRM.RoutingDescription.init();
    },


    // -- JSONP processing -- 

    // process JSONP response of routing server
    timeoutRoute: function () {
        OSRM.RoutingGeometry.showNA();
        OSRM.RoutingNoNames.showNA();
        OSRM.RoutingDescription.showNA(OSRM.loc("TIMED_OUT"));
        OSRM.Routing._snapRoute();
    },
    timeoutRoute_Dragging: function () {
        OSRM.RoutingGeometry.showNA();
        OSRM.RoutingDescription.showNA(OSRM.loc("TIMED_OUT"));
    },
    timeoutRoute_Reversed: function () {
        OSRM.G.markers.reverseMarkers();
        OSRM.Routing.timeoutRoute();
    },
    showRoute: function (response, parameters) {
        if (!response)
            return;
        if (parameters.keepAlternative != true)
            OSRM.G.active_alternative = 0;

        OSRM.G.response = response; // needed for printing & history routes!
        if (response.status == 207) {
            OSRM.RoutingGeometry.showNA();
            OSRM.RoutingNoNames.showNA();
            OSRM.RoutingDescription.showNA(OSRM.loc("NO_ROUTE_FOUND"));
            OSRM.Routing._snapRoute();
        } else {
            OSRM.RoutingAlternatives.prepare(OSRM.G.response);
            OSRM.RoutingGeometry.show(OSRM.G.response);
            OSRM.RoutingNoNames.show(OSRM.G.response);
            OSRM.RoutingDescription.show(OSRM.G.response);
            OSRM.Routing._snapRoute();
        }
        OSRM.Routing._updateHints(response);
        if (parameters.recenter == true) {		// allow recentering when the route is first shown
            var bounds = new L.LatLngBounds(OSRM.G.route._current_route.getPositions());
            OSRM.G.map.setViewBoundsUI(bounds);
        }
    },
    showNavTeqRoute: function () {        
        OSRM.Routing.getNavTeqRoute();       
    },
    displayNavTeqRoute: function(data) {                
        var points = [];
        var instructions = [];
        var summary = null;
        g.each(data, function (key, val) {            
            if (val.route.routeId == undefined) {
                g.each(val.route, function (mykey, myval) {
                    OSRM.G.NavTeqRouteId = myval.routeId;
                    g.each(myval.shape, function (mykey1, myval1) {
                        var latlons = myval1.split(',');
                        points.push(new L.LatLng(latlons[0], latlons[1]));
                        if (OSRM.G.NavTeqResponse == null) {
                            OSRM.G.NavTeqResponse = latlons[1] + ' ' + latlons[0];
                        } else {
                            OSRM.G.NavTeqResponse += ',' + latlons[1] + ' ' + latlons[0];
                        }
                    });

                    g.each(myval.leg, function (mykey2, myval2) {
                        g.each(myval2.maneuver, function (key3, val3) {
                            var newItem = {
                                Position: {
                                    Latitude: val3.position.latitude,
                                    Longitude: val3.position.longitude
                                },
                                Instruction: val3.instruction
                            };
                            instructions.push(newItem);
                        });
                        OSRM.G.NavTEQManeuver = myval2.maneuver;
                    });
                    summary = myval.summary;
                    OSRM.G.NavTEQSummary = myval.summary;
                });
            } else {
                OSRM.G.NavTeqRouteId = val.route.routeId;
                g.each(val.route.shape, function (mykey1, myval1) {
                    var latlons = myval1.split(',');
                    points.push(new L.LatLng(latlons[0], latlons[1]));
                    if (OSRM.G.NavTeqResponse == null) {
                        OSRM.G.NavTeqResponse = latlons[1] + ' ' + latlons[0];
                    } else {
                        OSRM.G.NavTeqResponse += ',' + latlons[1] + ' ' + latlons[0];
                    }


                    g.each(val.route.leg, function (mykey2, myval2) {
                        g.each(myval2.maneuver, function (key3, val3) {
                            var newItem = {
                                Position: {
                                    Latitude: val3.position.latitude,
                                    Longitude: val3.position.longitude
                                },
                                Instruction: val3.instruction
                            };
                            instructions.push(newItem);
                        });
                        OSRM.G.NavTEQManeuver = myval2.maneuver;
                    });
                    summary = val.route.summary;
                    OSRM.G.NavTEQSummary = val.route.summary;
                });
            }
        });
        OSRM.G.NavTeqGeoMetry = points;
        OSRM.RoutingGeometry.showNavTeqRoute(points);
        //OSRM.RoutingDescription.showNavTeq(instructions, summary);
        OSRM.RoutingDescription.showNavTEQDetail();
        OSRM.Routing._snapRoute();
        //OSRM.Routing.renderWayPointBoxes();
        OSRM.G.pending = false;        
    },
    
    getPointDistance: function (point1, point2) {        
        if (point1 == undefined) {
            point1 = OSRM.G.cacheStart;
        }
        var xs = 0;
        var ys = 0;                
        xs = point2.getLat().toFixed(6) - point1.getLat().toFixed(6);
        xs = xs * xs;                
        ys = point2.getLng().toFixed(6) - point1.getLng().toFixed(6);
        ys = ys * ys;        
        return Math.sqrt( xs + ys );                
  
    },

    getNavTeqRoute: function () {        
        var routeUrl = OSRM.Routing.getNavTeqRouteUrl();
        //console.trace();
        //console.log(routeUrl);
        //console.trace();
        OSRM.G.NavTeqResponse = null;
        g.ajax({
            dataType: "json",
            url: routeUrl,
            type: "GET",
            async: true,
            success: function (data) {                
                if (data.status != "failed") {
                    OSRM.G.NavTEQRawResponse = data;
                    OSRM.Routing.displayNavTeqRoute(data);
                } else {
                    if (data.Reason != undefined) {
                        alert(data.Reason);
                    }
                    
                }
                
            },
            
        });
    },

    renderWayPointBoxes: function () {
        var myPoints = OSRM.Routing.getSortedPoints();
        var allPoints = '';
        for (var i = 0; i < myPoints.length; i++) {
            if (allPoints == '') {
                allPoints = myPoints[i].getLat() + ',' + myPoints[i].getLng();
            }
            else {
                allPoints += ';' + myPoints[i].getLat() + ',' + myPoints[i].getLng();
            }
            
        }        
        ReRenderWayPoints(allPoints);
    },
    
    getSortedPoints: function () {
        var markers = OSRM.G.markers.route;
        if (markers.length < 1) {
            return null;
        }       
        var points = Array();
        for (var a = 0; a < markers.length; a++) {
            if (markers[a] == undefined) {
                continue;
            }
            points.push(markers[a]);
        }
        OSRM.G.cacheStart = points[0];
        /****No sorted points anymore by MTO**/
        //points.sort(function (p1, p2) {            
        //    var d1 = OSRM.Routing.getPointDistance(points[0], p1);
        //    var d2 = OSRM.Routing.getPointDistance(points[0], p2);
        //    //alert(d1 + ',' + d2);
        //    if (d1 < d2) {                
        //        return -1;
        //    } else return 1;           
        //});
        /**********************/
        return points;
    },

    getNavTeqRouteUrl: function () {        
        var points = OSRM.Routing.getSortedPoints();
        OSRM.G.SortedPoints = points;        
        var routeUrl = "SentinelService.aspx?";
        for (var i = 0; i < points.length; i++) {
            if (i == 0) {
                routeUrl += 'waypoint' + i.toString() + '=geo!' + points[i].getLat().toFixed(6) + ',' + points[i].getLng().toFixed(6);
            }
            else if (i == points.length - 1) {                
                routeUrl += '&waypoint' + i.toString() + '=geo!' + points[i].getLat().toFixed(6) + ',' + points[i].getLng().toFixed(6);
            } else {                
                routeUrl += '&waypoint' + i.toString() + '=geo!passThrough!' + points[i].getLat().toFixed(6) + ',' + points[i].getLng().toFixed(6);
            }            
        }        
        var now = new Date();               
        routeUrl += "&departure=" + encodeURIComponent(OSRM.Routing.getIsoDateTime(now));
        if (OSRM.G.NavTEQPreferences == null || OSRM.G.NavTEQPreferences == "") {
            OSRM.G.NavTEQPreferences = "fastest;car;traffic:disabled;";
        }
        switch(OSRM.G.active_routing_metric) {
            case 0:
                metricSystem = "metric";
                break;
            case 1:
                metricSystem = "imperial";
                break;
        }
        
        //routeUrl += "&language=" + OSRM.G.UserLanguage + "&routeattributes=all&maneuverattributes=all&legattributes=all&linkattributes=all&instructionformat=html&MetricSystem=" + metricSystem + "&mode=" + OSRM.G.NavTEQPreferences;
        routeUrl += "&language=" + OSRM.G.UserLanguage + "&routeattributes=all&maneuverattributes=all&instructionformat=html&MetricSystem=" + metricSystem + "&mode=" + OSRM.G.NavTEQPreferences;
        routeUrl += "&action=queryRoute";
        if (OSRM.G.NavTeqRouteId != "" && OSRM.G.NavTeqRouteId != null) {
            routeUrl += "&routeId=" + OSRM.G.NavTeqRouteId;
        }        
        //console.log(routeUrl);
        return routeUrl;
    },
    
    getIsoDateTime:function(d) {        
            // padding function
            var s = function(a,b){return(1e15+a+"").slice(-b)};

            // default date parameter
            if (typeof d === 'undefined'){
                d = new Date();
            };

            // return ISO datetime
            return d.getFullYear() + '-' +
                s(d.getMonth()+1,2) + '-' +
                s(d.getDate(),2) + 'T' +
                s(d.getHours(),2) + ':' +
                s(d.getMinutes(),2) + ':' +
                s(d.getSeconds(),2);
          
    },

    showRoute_Dragging: function (response) {
        if (!response)
            return;
        if (!OSRM.G.dragging)		// prevent simple routing when not dragging (required as there can be drag events after a dragstop event!)
            return;

        OSRM.G.response = response; // needed for history routes!
        if (response.status == 207) {
            OSRM.RoutingGeometry.showNA();
            OSRM.RoutingDescription.showNA(OSRM.loc("YOUR_ROUTE_IS_BEING_COMPUTED"));
        } else {
            OSRM.RoutingGeometry.show(response);
            OSRM.RoutingDescription.showSimple(response);
        }
        OSRM.Routing._updateHints(response);

        if (OSRM.G.pending)
            setTimeout(OSRM.Routing.draggingTimeout, 1);
    },
    
    showNavTeqRoute_Dragging: function () {
        if (!OSRM.G.dragging && OSRM.G.RouteProvider == 'NavTEQ')		// prevent simple routing when not dragging (required as there can be drag events after a dragstop event!)
            return;
        OSRM.Routing.getNavTeqRoute();        
    },
    showRoute_Redraw: function (response, parameters) {
        if (!response)
            return;
        if (parameters.keepAlternative == false)
            OSRM.G.active_alternative = 0;

        OSRM.G.response = response; // not needed, even harmful as important information is not stored! ==> really ????
        if (response.status != 207) {
            OSRM.RoutingAlternatives.prepare(OSRM.G.response);
            OSRM.RoutingGeometry.show(OSRM.G.response);
            OSRM.RoutingNoNames.show(OSRM.G.response);
        }
        OSRM.Routing._updateHints(response);
    },

    showNavTeqRoute_Redraw: function(parameters) {
        if (parameters.keepAlternative == false)
            OSRM.G.active_alternative = 0;
    },

    //-- main function --

    //generate server calls to query routes
    getRoute: function (parameters) {
        // if source or target are not set -> hide route
        if (OSRM.G.markers.route.length < 2) {
            OSRM.G.route.hideRoute();
            return;
        }

        parameters = parameters || {};

        OSRM.JSONP.clear('dragging');
        OSRM.JSONP.clear('redraw');
        OSRM.JSONP.clear('route');
        if (OSRM.G.RouteProvider != 'NavTEQ') {
            OSRM.JSONP.call(OSRM.Routing._buildCall() + '&instructions=true', OSRM.Routing.showRoute, OSRM.Routing.timeoutRoute, OSRM.DEFAULTS.JSONP_TIMEOUT, 'route', parameters);
        } else {
            OSRM.Routing.showNavTeqRoute();
        }
    },
    getRoute_Reversed: function () {
        if (OSRM.G.markers.route.length < 2)
            return;

        OSRM.JSONP.clear('dragging');
        OSRM.JSONP.clear('redraw');
        OSRM.JSONP.clear('route');
        if (OSRM.G.RouteProvider != 'NavTEQ') {
            OSRM.JSONP.call(OSRM.Routing._buildCall() + '&instructions=true', OSRM.Routing.showRoute, OSRM.Routing.timeoutRoute_Reversed, OSRM.DEFAULTS.JSONP_TIMEOUT, 'route', {});
        } else {
            OSRM.Routing.showNavTeqRoute();
        }
    },
    getRoute_Redraw: function (parameters) {
        if (OSRM.G.markers.route.length < 2)
            return;

        parameters = parameters || {};

        OSRM.JSONP.clear('dragging');
        OSRM.JSONP.clear('redraw');
        if (OSRM.G.RouteProvider != 'NavTEQ') {
            OSRM.JSONP.call(OSRM.Routing._buildCall() + '&instructions=true', OSRM.Routing.showRoute_Redraw, OSRM.Routing.timeoutRoute, OSRM.DEFAULTS.JSONP_TIMEOUT, 'redraw', parameters);
        } else {
            OSRM.Routing.showNavTeqRoute_Redraw(parameters);
        }
    },
    getRoute_Dragging: function () {
        OSRM.G.NavTeqRouteId = "";
        if (OSRM.G.RouteProvider != 'NavTEQ') {
            OSRM.G.pending = !OSRM.JSONP.call(OSRM.Routing._buildCall() + '&instructions=false', OSRM.Routing.showRoute_Dragging, OSRM.Routing.timeoutRoute_Dragging, OSRM.DEFAULTS.JSONP_TIMEOUT, 'dragging');            
        } else {
            OSRM.G.pending = true;
            OSRM.Routing.showNavTeqRoute_Dragging();            
            //OSRM.Routing.getNavTeqRoute();
        }
    },
    draggingTimeout: function () {
        OSRM.G.markers.route[OSRM.G.dragid].hint = null;
        OSRM.Routing.getRoute_Dragging();
    },

    _buildCall: function () {
        var source = OSRM.G.active_routing_server_url;
        source += '?z=' + OSRM.G.map.getZoom() + '&output=json&jsonp=%jsonp';
        if (OSRM.G.markers.checksum)
            source += '&checksum=' + OSRM.G.markers.checksum;
        var markers = OSRM.G.markers.route;
        for (var i = 0, size = markers.length; i < size; i++) {
            source += '&loc=' + markers[i].getLat().toFixed(6) + ',' + markers[i].getLng().toFixed(6);
            if (markers[i].hint)
                source += '&hint=' + markers[i].hint;
        }
        return source;
    },


    //-- helper functions --

    // update hints of all markers
    _updateHints: function (response) {
        var hint_locations = response.hint_data.locations;
        OSRM.G.markers.checksum = response.hint_data.checksum;
        for (var i = 0; i < hint_locations.length; i++)
            OSRM.G.markers.route[i].hint = hint_locations[i];
    },

    // snap all markers to the received route
    _snapRoute: function () {
        var markers = OSRM.G.markers.route;
        var via_points = OSRM.G.response.via_points;

        for (var i = 0; i < via_points.length; i++) {
            //markers[i].setPosition(new L.LatLng(via_points[i][0], via_points[i][1]));
        }

        OSRM.Geocoder.updateAddress(OSRM.C.SOURCE_LABEL);
        OSRM.Geocoder.updateAddress(OSRM.C.TARGET_LABEL);

        OSRM.G.markers.relabelViaMarkers();
    }

};
