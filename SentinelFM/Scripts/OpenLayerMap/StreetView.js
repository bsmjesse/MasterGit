var mapPanel;

var streetViewPanel;

var viewport;

var layer;

var DirectZoom=19;
var timer_is_on = false;
var finishedLastTimer = false;
var timers;
Ext.onReady(function()
{

   var qsParm = new Array();

function geocodePosition(pos) {
  var geocoder = new google.maps.Geocoder();
  geocoder.geocode({
    latLng: pos
  }, function(responses) {
    if (responses && responses.length > 0) {
      updateMarkerAddress(responses[0].formatted_address);
    } else {
      updateMarkerAddress('Cannot determine address at this location.');
    }
  });
}

   function qs()
   {
      var query = window.location.search.substring(1);
      if (query.indexOf("Id") != - 1)
      {
         if (query.indexOf('&') != - 1)
         {
            var parms = query.split('&');
            for (var i = 0; i < parms.length; i ++ )
            {
               var pos = parms[i].indexOf('=');
               if (pos > 0)
               {
                  var key = parms[i].substring(0, pos);
                  var val = parms[i].substring(pos + 1);
                  qsParm[key] = val;
               }
            }
         }
         else
         {
            var pos = query.indexOf('=');
            // alert(pos);
            if (pos > 0)
            {
               var key = query.substring(0, pos);
               // alert(key);
               var val = query.substring(pos + 1);
               // alert(val);
               qsParm[key] = val;
            }
         }
      }
   }



   try
   {
      qs();
      myWinID = qsParm["WinId"];
      // var p = parent;
      var winparent = window.opener;
      if (winparent == null)
      {
         winparent = window.parent;
      }
      var didntLoadFrmParent = false;
      if (winparent != null)
      {
         if (myWinID && myWinID != "")
         {
            allVehicles = winparent.GetWinInitialTrackData(myWinID);            
            //console.log(allVehicles[0].BoxId);
         }
      }
   }
   catch(err)
   {
     // console.log("Error " + err.message);
   }

   if(allVehicles[0].BoxId)
   {

       var featurePosition = new google.maps.LatLng(allVehicles[0].Latitude, allVehicles[0].Longitude);

     
      viewport = new Ext.Viewport(
      {
         layout : "border",
         id : 'mainViewport',
         items : [
         {
            region : "center",
            layout : 'fit',
            id : "mappanel",
            title : "Google Map",
            width : '50%',
            maxWidth : window.screen.width/2,
            //xtype : "gx_mappanel",            
            
			xtype: 'gmappanel',
            zoomLevel: DirectZoom,
            gmapType: 'map',            
			border: true,
            mapConfOpts: ['enableScrollWheelZoom','enableDoubleClickZoom','enableDragging'],
            mapControls: ['GSmallMapControl','GMapTypeControl'],
             setCenter: {
                        lat: allVehicles[0].Latitude,
                        lng: allVehicles[0].Longitude,
                        marker: {
                          //title: allVehicles[0].BoxId,
                          draggable: true,
                          clear: true, //Clear other markers
                          listeners:
                          {
                            dragend : function()
                            {
                                //geocodePosition(this.getPosition());
                                var streetPanel=Ext.getCmp('streetviewpanel');//document.getElementById('streetviewpanel');
                                streetPanel.getStreetViewMap(streetPanel.getMap().getStreetView(),this.getPosition());
                                //streetMap.setPosition(this.getPosition()); //This pointer here is the marker reference.
                                //streetMap.setVisible(true);
                            }
                          }
                        }
                    },
      
            split : true,
            listeners :
            {
//                 'afterrender' : function ()
//                 {
//                     console.log(' I am done with map');
//                   // fleetstore.load();
//                 }
            }
            //tbar : toolbar
         }
         ,
         {
            region : "east",
            layout : 'fit',
            width : '50%',
            id : "streetviewpanel",
            maxWidth : window.screen.width/2,
            title : 'Street View',
            zoomLevel: DirectZoom,
            pitch: 0,
            //closeAction : 'hide',            
            xtype: 'gmappanel',
		    gmapType: 'panorama',
    		setCenter: {
	       		lat: allVehicles[0].Latitude,
	   	       	'long': allVehicles[0].Longitude
        		},    
                split : true           
         }
         ]
      }
      );
      

      viewport.doLayout();
      
//       var vehicletask =
//   {
//      run : function ()
//      {
//         CheckAndUpdateMap();
//      }
//      ,
//      interval : interval // 5 second
//   }
//   var vehiclerunner = new Ext.util.TaskRunner();

//   vehiclerunner.start(vehicletask);      

   }
}
);
