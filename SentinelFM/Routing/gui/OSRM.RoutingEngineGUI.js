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

// OSRM RoutingEngineGUI
// [handles all GUI aspects that deals with switching the routing engine]


OSRM.GUI.extend( {
		
// init
init: function() {
	// gather routing engines
	var options = OSRM.GUI.getRoutingEngines();

    // generate selectors
	var selectedValue = 0;
	if (OSRM.G.RouteProvider == "NavTEQ") {
	    selectedValue = g('#gui-engine-toggle').val();
	    if (selectedValue == null) {
	        selectedValue = 0;
	    }
	}
	OSRM.GUI.selectorInit("gui-engine-toggle", options, selectedValue, OSRM.GUI._onRoutingEngineChanged);
},

// change active routing engine
setRoutingEngine: function(engine) {
	if( engine == OSRM.G.active_routing_engine )
		return;
	
	OSRM.GUI.selectorChange( 'gui-engine-toggle', engine );
	
	OSRM.G.active_routing_engine = engine;
    var routingengines = OSRM.DEFAULTS.ROUTING_ENGINES;
	if (OSRM.G.RouteProvider == "NavTEQ") {
	    routingengines = OSRM.DEFAULTS.NAVTEQ_ROUTING_ENGINES;
	}
	OSRM.G.active_routing_metric = routingengines[OSRM.G.active_routing_engine].metric;;
	OSRM.G.active_routing_server_url = routingengines[OSRM.G.active_routing_engine].url;
	OSRM.G.active_routing_timestamp_url = routingengines[OSRM.G.active_routing_engine].timestamp;
	
    // requery data timestamp
	if (OSRM.G.RouteProvider == 'OSRM') {
	    OSRM.GUI.queryDataTimestamp();
	    OSRM.JSONP.call(routingengines[OSRM.G.active_routing_engine].timestamp + "?jsonp=%jsonp", OSRM.GUI.setDataTimestamp, OSRM.GUI.setDataTimestampTimeout, OSRM.DEFAULTS.JSONP_TIMEOUT, 'data_timestamp');
	}	
},
_onRoutingEngineChanged: function(engine) {
	if( engine == OSRM.G.active_routing_engine )
		return;
	
	OSRM.GUI.setRoutingEngine( engine );
	
	// requery route
	if (OSRM.G.markers.route.length > 1 && OSRM.G.RouteProvider != "NavTEQ")
		OSRM.Routing.getRoute();
},

// change language of routing engine entries
setRoutingEnginesLanguage: function() {
	// gather routing engines
	var options = OSRM.GUI.getRoutingEngines();
	
	// change dropdown menu names
	OSRM.GUI.selectorRenameOptions( "gui-engine-toggle", options );
},

// gather routing engines
getRoutingEngines: function() {
    var engines = OSRM.DEFAULTS.ROUTING_ENGINES;
    if (OSRM.G.RouteProvider == "NavTEQ") {
        engines = OSRM.DEFAULTS.NAVTEQ_ROUTING_ENGINES;
    }
	var options = [];
	for(var i=0, size=engines.length; i<size; i++) {
		options.push( {display:OSRM.loc(engines[i].label), value:i} );
	}
	
	return options;
}

});
