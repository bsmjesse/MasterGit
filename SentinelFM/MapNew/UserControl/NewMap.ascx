<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewMap.ascx.cs" Inherits="SentinelFM.Map_UserControl_Map" %>
   <div >
    <script type="text/javascript" language="javascript">

        var circleMarkers = null;
        var landmarkPolygonMarkers = null;
        var mapCurrentView = null;
        var mapCurrentViewGeo = null;
        var geozoneMarkers = null;

        var recursiveObject =null;
        var recursiveIndex =null;
        var recursiveObject_l =null;
        var recursiveIndex_l =null;
        var recursiveObject_g =null;
        var recursiveIndex_g =null;
        var mapLatLonLoc = null;
        var eventDispatcher = new EventDispatcher();
        var myMap;

        var finishLankMark = true;
        var finishGeozone = true;
        var finishHistory = true;
        var service = eval("<% GetService (); %>");
        var authTokenTuple = eval("[<% GetToken (); %>]");
        var hasDrawGeozone = false;
        var lsdtileLayer = null;
        $(document).ready(function () {

            // Set the internal GeoBase authentication token
           $("#panControlPanel").dblclick(function (event){ try{event.stopPropagation();} catch(err){}});

           $("#divMapLoadingText").hide();

            if (<%=isBigMap %> == 0 && <%= isFixSize %> == 0)
            {
               $("#vehicleMapfrmVehicleMap").width($(window).width());
               $("#vehicleMapfrmVehicleMap").height($(window).height());
            }
            Telogis.GeoBase.setService(service);
            Telogis.GeoBase.setAuthToken(authTokenTuple[0], authTokenTuple[1]);

            var options = {
                initialPosition: [42.423457, -99.887695]
            };

            if ('<%=IsFrench %>' == '1')
            {
               IsFrenchCulture = true;
               $('#recenterButton').html('Recentrer');
               $('#roadsLink').html('Routes');
               $('#printmap').html('Imprimer');
               $('#spanlabelsOption').html('Nom du véhicule');
               $('#spanassetLayerOption').html('Actifs/Historique');
               $('#spantrailsOption').html('Sentiers');
               $('#spanlandmarkLayerOption').html('Sites');
               $('#spanshowLandmarkRadius').html('Rayon du site');
               $('#spanshowLandmarkName').html('Nom du site');
               $('#spanlsdMapping').html('LSD Carte');
            }

            myMap = new Sentinel.Map($('#mapContainer')[0], eventDispatcher, options);
            
            if (<%= IsShowLandmarkname %> == '1')
            {
               $("#showLandmarkName").attr("checked", true);
               $(myMap.map.elem).removeClass("hide_landmark_label");
            }
            else $(myMap.map.elem).addClass("hide_landmark_label");
            
            if (<%= IsShowLandmarkRadius %> == '1') $("#showLandmarkRadius").attr('checked', true);
            if (<%= IsShowVehicleName %> == '1') 
            {
               $("#labelsOption").attr('checked', true);
               $(myMap.map.elem).removeClass("hide_vehicle_label");
            }
            else
            {
               $("#labelsOption").attr('checked', false);
               $(myMap.map.elem).addClass("hide_vehicle_label");
            }
            
            if ('<%= isShowLandMark %>' == '1')
            {
              myMap.enableLayer("landmarkLayer", true);
              $("#landmarkLayerOption").attr('checked', true);
            }
            else myMap.enableLayer("landmarkLayer", false);

            if (<%=isBigMap %> != 0 &&  <%= isFixSize %> == 0)
            {
                SetMapSize($(window).height()- 10 , $(window).width()- 10);
            }

            if ('<%= isFixSize %>' == '1')
            {
                 var mapWidth = $("#vehicleMapfrmVehicleMap").width();
                 if ('<%= MapWidth %>' != '0' )
                    mapWidth = <%= MapWidth %>;
                 if ('<%= MapHeight %>' == '0')
                    SetMapSize($("#vehicleMapfrmVehicleMap").height(), mapWidth);
                 else 
                    SetMapSize(<%= MapHeight %>, mapWidth);
            }         
            getVehicles();         
                     

            if ('<%= hasLandmarks %>' != '0' ) {
                eval('recursiveObject_l = <%= LandMarkData %>');
                myMap.map.Zoom.append(function () {
                    DrawLandmarkcirclesByZoomLevel();
                })
                myMap.map.Drag.append(function () {
                    //DrawLandmarkcirclesByZoomLevel();
                })
                myMap.map.Pan.append(function () {
                    //DrawLandmarkcirclesByZoomLevel();
                })
                myMap.map.Resize.append(function () {
                    DrawLandmarkcirclesByZoomLevel();
                })
                myMap.map.MouseUp.append(function (e) {
                       //if (e.button == 0)
                       DrawLandmarkcirclesByZoomLevel();
                })
            }

            myMap.map.setZoomIndex(1);
            if ('<%= MapSearchData %>' != '') {
                var mapSearchData = '';
                eval('mapSearchData = <%= MapSearchData %>');
                ShowMapAddress(eval(mapSearchData));
            }
            
            if ('<%= MapData %>' != '') {
                var mapAata = '';
                eval('mapAata = <%= MapData %>');
                ShowMultipleAssets(eval(mapAata));
            }
            //myMap.enableLayer("landmarkLayer", false);
            //myMap.setGeozoneVisibility(false);
            //if ('<%= isLandmark_GeoZone %>' == '1')
            {
                 if ('<%= isShowGeoZone %>' == '1')
                 {
                    myMap.setGeozoneVisibility(true);
                    $("#geozoneLayerOption").attr('checked', true);
                 }
                 else myMap.setGeozoneVisibility(false);
            }

            myMap.landmarkloc = new Array();
            myMap.landmarkPoints = new Array();
            myMap.geozonePoints = new Array();
            if ('<%= hasLandmarks %>' != '0' ) {
                finishLankMark = false;
                recursiveIndex_l = 0;
                ShowMapLoadingText();
                myMap.BeginShowLandmark();
                RecursiveShowLandmarks();
            }

            if ('<%= hasGeozone %>' != '0' ) {
               if (finishLankMark) ShowGeoZoneInMap();
            }            

            <%= StartupScript %>


            if ('<%= OneLandmark %>' != '') {
                myMap.map.RightClick.remove();
                var oneLandmark = '';
                eval('oneLandmark = <%= OneLandmark %>');
                myMap.ShowOneLandMark(oneLandmark);
                myMap.enableLayer("landmarkLayer", true);
                if ('<%= isFixSize %>' == '0') SetMapSize($(window).height() , $(window).width() - 10);
                $(".dropdown").hide();
            }  
            
            if ('<%= OneGeoZone %>' != '' ) {
                myMap.map.RightClick.remove();
                var oneGeoZone = '';
                eval('oneGeoZone = <%= OneGeoZone %>');
                myMap.ShowOneGeozone(oneGeoZone);
                myMap.setGeozoneVisibility(true);

                //SetMapSize($(window).height() , $(window).width() - 10);
                $(".dropdown").hide();
            }      
            
            if ('<%= isDrawGeozoneMap %>' == '1')
            {
                 $(".dropdown").hide();
                 myMap.map.RightClick.remove();
                 if ('<%= DrawGeozoneData %>' != '')
                 {
                    var drawGeozoneData = '';
                    eval('drawGeozoneData = <%= DrawGeozoneData %>');
                    if (drawGeozoneData[0].coords == '') 
                    {
                       myMap.ShowOneLandMark(drawGeozoneData[0]);
                    }
                    else
                    {
                       myMap.ShowOneGeozone(drawGeozoneData);
                    }
                 }
                 myMap.enableLayer("landmarkLayer", true);
                 myMap.setGeozoneVisibility(true);
            }
            if ('<%= isDrawMap %>' == '1')
            {
                $(".dropdown").hide();
//                var pushpinLayer = new ObjectLayer ({id: "pushpin_layer", map: myMap.map});
//                pushpinLayer.clear();
//                myMap.map.RightClick.disable = true;
//                myMap.map.RightClick.enable = false;
//                myMap.map.RightClick.remove();
//                myMap.map.RightClick.append(function (e){
//                    pushpinLayer.clear();
//                    var pin = new ImageObject ({
//                        anchorPoint: new Point (0.5, 1.0),
//                        dragEnabled: true,
//                        layer:       pushpinLayer,
//                        location:    myMap.map.mouseLatLon (e),
//                        src:        "../Bigicons/NewLandmark.ico"
//                    });

//                    mapLatLonLoc = myMap.map.mouseLatLon(e);
//                      
//                });
            }

            var loadingY = $("#vehicleMapfrmVehicleMap").offset().top;
            $("#divMapLoadingText").css("top", loadingY + 35);

            $('#topControlArea').addClass("NoPrintStyle");
            $('#panControlPanel').addClass("NoPrintStyle");
            $('#topMenu').addClass("NoPrintStyle");
            $('#divMapLoadingText').addClass("NoPrintStyle");

          
        });


        $(window).resize(function () {
            if (<%= isFixSize %> == 0 )
            {
                if (<%=isBigMap %> == 0)
                   SetMapSize($(window).height(), $(window).width());
                   //SetMapSize($("#vehicleMapfrmVehicleMap").height(), $(window).width());
                else 
                  SetMapSize($(window).height() - 10, $(window).width() -10);
            }
        });

        function getVehicles()
        {
                var brVersion=msieversion();
                     if(brVersion==0 || brVersion>7)
                     {                        
                    
                        var parent = window.parent;
                        if(parent)
                        {
                            var AllVehicles=parent.GetTelogisMapData();                        
                            if(AllVehicles)
                            {
                                if(AllVehicles!="" && AllVehicles!=null)
                                {
                                    ShowMultipleAssets(AllVehicles);
                                }
                                else
                                {
                                setTimeout("getVehicles()",10000);
                                }
                            }
                            else
                            {
                                setTimeout("getVehicles()",10000);
                            }
                        }
                    }
                    else
                    {
                      // Need to do in future
                    }
        }

        function SetMapSize(width, height) 
        {
           //var Size    = Telogis.GeoBase.Size;
           myMap.setSizeXY(height, width);
        }

            function zoomMap(fleet)
            {
                if (fleet != null ) {
                    fleet = eval(fleet);
                    eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.ZOOM_MAP,{ fleet: fleet });                
                    //Sentinel.Constants.EVENTS.SHOW_ASSET, { fleet: fleet });
                    //DrawLandmarkcirclesByZoomLevel();
                }            
            }

               function msieversion()
                {
                    var ua = window.navigator.userAgent
                    var msie = ua.indexOf ( "MSIE " )
                    if ( msie > 0 )      // If Internet Explorer, return version number
                        return parseInt (ua.substring (msie+5, ua.indexOf (".", msie )))
                    else                 // If another browser, return 0
                        return 0

               }

       function UpdateMultipleAssets(fleet) {
            if (fleet != null ) {
                fleet = eval(fleet);
                eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.UPDATE_ASSET,{ fleet: fleet });                
                //Sentinel.Constants.EVENTS.SHOW_ASSET, { fleet: fleet });
                //DrawLandmarkcirclesByZoomLevel();
            }
        }
        
        function ShowMultipleAssets(fleet) {
            if (fleet != null ) {
                fleet = eval(fleet);
                eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.SHOW_ASSET, { fleet: fleet });
                //(Sentinel.Constants.EVENTS.UPDATE_ASSET,{ fleet: fleet });
                //Sentinel.Constants.EVENTS.SHOW_ASSET, { fleet: fleet });
                DrawLandmarkcirclesByZoomLevel();
            }
        }
        
        var loadingLabel = document.createElement("div");
        function BeginShowMapTrip()
        {
           try
           {
              myMap.BeginshowTrip();
           }
           catch(e) {}
        }

        function ShowMapLoadingText()
        {
           var loadingText = '';
           if (!finishLankMark ) loadingText = " Landmarks";
           if (!finishGeozone )
           {
               if (loadingText == "") loadingText = " Geozones";
               else  loadingText = loadingText + ", Geozones"
           }
           if (!finishHistory )
           {
               if (loadingText == "") loadingText = " History";
               else  loadingText = loadingText + ", History"
           }

           $("#divMapLoadingText").show();
           if (loadingText != '')  $("#divMapLoadingText").text("Loading" + loadingText + "...");
        }

        function EndShowMapTrip()
        {
           try
           {
              myMap.EndshowTrip();
           }
           catch(e) {}
        }

        function ShowHistoryMap(fleet) {
            recursiveObject = eval(fleet);
            recursiveIndex = 0;
            finishHistory = false;
            ShowMapLoadingText();
            BeginShowMapTrip();
            RecursiveShowHistoryMap();
        }

        function RecursiveShowHistoryMap() {
            if (recursiveObject != null)
            {
                if (recursiveIndex <= recursiveObject.length - 1)
                {
                       //eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.SHOW_TRIP, {trip: recursiveObject[recursiveIndex]});
                       myMap.ShowSingleTrip(recursiveObject[recursiveIndex], recursiveIndex);
                }
                if (recursiveIndex == recursiveObject.length - 1)
                {
                   finishHistory = true; 
                   EndShowMapTrip();
                   if (finishLankMark && finishGeozone && finishHistory)
                           $("#divMapLoadingText").hide();

                   DrawLandmarkcirclesByZoomLevel();
                 }
                else
                {
                    recursiveIndex = recursiveIndex + 1;
                    setTimeout("RecursiveShowHistoryMap()", 1);
                }

            }
            else finishHistory = true;
        }

        function ShowMapAddress(addr) {
            if (addr != null ) {
                addr = eval(addr);
                eventDispatcher.raiseEvent(Sentinel.Constants.EVENTS.SHOW_ADDRESS, { addr: addr });
            }
        }

        function RecursiveShowLandmarks() {
//                for (var inde = 0; inde<= recursiveObject_l.length;inde++ )
//                {
//                debugger
//                   myMap.showSingleLandmark(recursiveObject_l[inde]);
//                }

//                        finishLankMark = true;
//                        ShowGeoZoneInMap();
//                        if (finishLankMark && finishGeozone && finishHistory)
//                           $("#divMapLoadingText").hide();
//                        SetLandmarkAndGeozoneCenter();
//                return;
//                
                if (recursiveObject_l != null)
                {
                    if (recursiveIndex_l <= recursiveObject_l.length - 1)
                    {
                           myMap.showSingleLandmark(recursiveObject_l[recursiveIndex_l]);
                    }
                    if (recursiveIndex_l == recursiveObject_l.length - 1)
                    {
                        finishLankMark = true;
                        ShowGeoZoneInMap();
                        if (finishLankMark && finishGeozone && finishHistory)
                           $("#divMapLoadingText").hide();
                        SetLandmarkAndGeozoneCenter();
                    }
                    else
                    {
                        recursiveIndex_l = recursiveIndex_l + 1;
                        setTimeout("RecursiveShowLandmarks()", 10);
                    }

                }
            }

            function ShowGeoZoneInMap()
            {
                if ('<%= hasGeozone %>' != '0' )
                {
                    hasDrawGeozone = true;
                    var geoData = '';
                    eval('recursiveObject_g = <%= GeoData %>');
                    recursiveIndex_g = 0;
                    finishGeozone = false;
                    ShowMapLoadingText();
                    myMap.BeginShowGeozone();
                    RecursiveShowGeozones();
                }
            }

            function RecursiveShowGeozones() {
                if (recursiveObject_g != null)
                {
                    if (recursiveIndex_g <= recursiveObject_g.length - 1)
                    {
                        try
                        {
                           myMap.showSingleGeozone(recursiveObject_g[recursiveIndex_g]);
                        }
                        catch(err){}
                    }
                    if (recursiveIndex_g == recursiveObject_g.length - 1)
                    {
                        finishGeozone = true;
                        if (finishLankMark && finishGeozone && finishHistory)
                           $("#divMapLoadingText").hide();
                        SetLandmarkAndGeozoneCenter();
                    }
                    else
                    {
                        recursiveIndex_g = recursiveIndex_g + 1;
                        setTimeout("RecursiveShowGeozones()", 10);
                    }

                }
            }

            function SetLandmarkAndGeozoneCenter()
            {
                var isShowLandMark = false;
                var isShowGeoZone = false;
                if ($("#landmarkLayerOption").attr('checked') == true) isShowLandMark = true;
                if ($("#geozoneLayerOption").attr('checked') == true) isShowGeoZone = true;

                             if (isShowLandMark && !isShowGeoZone)
                             {
                                 myMap.enableLayer("landmarkLayer", true);
                             }
                             if (!isShowLandMark && isShowGeoZone)
                             {
                                 myMap.setGeozoneVisibility(true);
                             }
                             if (isShowLandMark && isShowGeoZone)
                             {
                                 myMap.enableLayer("landmarkLayer", true);
                                 myMap.setGeozoneVisibility(true);

                             }                         

                try
                {
                    if ('<%= isLandmark_GeoZone %>' == '1')
                    {
                       if (finishLankMark && finishGeozone)
                       {
                             if (isShowLandMark && !isShowGeoZone)
                             {
                                 myMap.centerMap(myMap.landmarkPoints);
                             }
                             if (!isShowLandMark && isShowGeoZone)
                             {
                                 myMap.centerMap(myMap.geozonePoints);
                             }
                             if (isShowLandMark && isShowGeoZone)
                             {
                                 myMap.centerMap(myMap.landmarkPoints.concat(myMap.geozonePoints));

                             }                         
                       }
                    }
                }
                catch(err){};

            }

            function DrawLandmarkcirclesByZoomLevel()
            {
               myMap.landmarkFenceCollection.clear();
               if (recursiveObject_l != null && myMap.map.getZoomIndex() >= 10 && $("#landmarkLayerOption").attr('checked') == true && 
                   $("#showLandmarkRadius").attr('checked') == true )
               {
                  var isDraw = false;
                  var isDrawPolygon = false;
                  var mapSize = myMap.map.getSize();
                  var mapBox = new Telogis.GeoBase.BoundingBox(myMap.map.getLatLon(new Telogis.GeoBase.Point(0,0)), myMap.map.getLatLon(new Telogis.GeoBase.Point(mapSize.width,mapSize.height)));
                  if (mapCurrentView != null)
                  { 
                      var locNE_1 = mapCurrentView.getNE();
                      var locNE_2 = mapBox.getNE();
                      var locSW_1 = mapCurrentView.getSW();
                      var locSW_2 = mapBox.getSW();
                      if (locNE_1.lon == locNE_2.lon && locNE_1.lat == locNE_2.lat && locSW_1.lon == locSW_2.lon && locSW_1.lat == locSW_2.lat)    
                        return;
                  }
                  
                  if (landmarkPolygonMarkers != null) 
                  {
                      landmarkPolygonMarkers.destroy();
                  }
                  isUpdateLandmarkCanvas = false;
                  landmarkPolygonMarkers  = new LandmarkPolygonMarkerLayer  ({id: "LandmarkPolygonMarkerLayer", map: myMap.map, zIndex:Sentinel.Constants.LAYER_ZINDEX.LANDMARK_CIRCLE});
                  isUpdateLandmarkCanvas = true;
                  landmarkPolygonMarkers._points = [];
                  landmarkPolygonMarkers.setZIndex(20);

                  if (circleMarkers != null) 
                  {
                      circleMarkers.destroy();
                  }
                  isUpdateCanvas = false;
                  circleMarkers = new CircleMarkerLayer ({id: "circleMarkers", map: myMap.map, zIndex:Sentinel.Constants.LAYER_ZINDEX.LANDMARK_CIRCLE});
                  isUpdateCanvas = true;
                  circleMarkers._points = [];
                  circleMarkers._radius = [];
                  circleMarkers.setZIndex(20);
                  var screen_X = mapSize.width;
                  var screen_Y = mapSize.height;
                  for (var landmarkIndex =0; landmarkIndex < recursiveObject_l.length; landmarkIndex++) {
                       var landmark = recursiveObject_l[landmarkIndex];
                       var landmarklatLon = new LatLon(parseFloat(landmark.lat), parseFloat(landmark.lon));
                       //if (landmark.desc =='dimrud1' ) {debugger ;}

                       if (landmark.rad != '0' && landmark.rad != '-1'&&
                           (mapBox.contains(landmarklatLon) || 
                           CheckCircleIsInRetangle(landmarklatLon, parseInt(landmark.rad), screen_X, screen_Y, mapBox)))
                       {
                            try {
                                isDraw = true;
                                var newZone = null;
                                circleMarkers.addRadius(parseFloat(landmark.rad));
                               circleMarkers.addPoint(landmarklatLon);
                            }
                            catch (err) { }
                       }
                       else
                       {
                           //Polygon Begin
                                   if (landmark.rad == '-1')
                                   {
                                       if (mapBox.contains(landmarklatLon))
                                       {
                                            try {
                                                landmark.coords = eval(landmark.coords);
                                                if (landmark.coords.length >= 3) {
                                                   var  landmarkCoordsXy = CheckGeozoneIsInRetangle(landmark.coords, mapBox, screen_X, screen_Y, true)
                                                   if (landmarkCoordsXy.length > 0)
                                                   {
                                                      isDrawPolygon = true;
                                                      landmarkPolygonMarkers.addPoint(landmarkCoordsXy);
                                                   }
                                                }
                                            }
                                            catch (err) { }
                                       }
                                       else
                                       {
                                            try {
                                                landmark.coords = eval(landmark.coords);
                                                if (landmark.coords.length >= 3) {
                                                   var  landmarkCoordsXy = CheckGeozoneIsInRetangle(landmark.coords, mapBox, screen_X, screen_Y, false)
                                                   if (landmarkCoordsXy.length > 0)
                                                   {
                                                      isDrawPolygon = true;
                                                      landmarkPolygonMarkers.addPoint(landmarkCoordsXy);
                                                   }
                                                }
                                            }
                                            catch (err) { }
                                       }
                                   }

                           //Polygon End
                       }

                  }
                  if (isDraw) {  circleMarkers.setFillColor (new Color (0xff, 0xff, 0xff, 0.6));
                   }
                  if (isDrawPolygon) {  landmarkPolygonMarkers.setFillColor (new Color (0xff, 0xff, 0xff, 0.6));
                   }
               }
               else {if (circleMarkers != null) circleMarkers.destroy();if (landmarkPolygonMarkers != null) landmarkPolygonMarkers.destroy();}


               //DrawGeoZone
               if (recursiveObject_g != null && myMap.map.getZoomIndex() >= 7 && $("#geozoneLayerOption").attr('checked') == true )
               {
                  var isDraw = false;
                  var mapSize = myMap.map.getSize();
                  var mapBox = new Telogis.GeoBase.BoundingBox(myMap.map.getLatLon(new Telogis.GeoBase.Point(0,0)), myMap.map.getLatLon(new Telogis.GeoBase.Point(mapSize.width,mapSize.height)));
                  if (mapCurrentViewGeo != null)
                  { 
                      var locNE_1 = mapCurrentViewGeo.getNE();
                      var locNE_2 = mapBox.getNE();
                      var locSW_1 = mapCurrentViewGeo.getSW();
                      var locSW_2 = mapBox.getSW();
                      if (locNE_1.lon == locNE_2.lon && locNE_1.lat == locNE_2.lat && locSW_1.lon == locSW_2.lon && locSW_1.lat == locSW_2.lat)    
                        return;
                  }
                  if (geozoneMarkers != null) 
                  {
                      geozoneMarkers.destroy();
                  }
                  isUpdateGeozoneCanvas = false;
                  geozoneMarkers  = new PolygonMarkerLayer  ({id: "polygonMarker", map: myMap.map, zIndex:Sentinel.Constants.LAYER_ZINDEX.GEOZONE});
                  isUpdateGeozoneCanvas = true;
                  geozoneMarkers._points = [];
                  geozoneMarkers.setZIndex(20);
                  var screen_X = mapSize.width;
                  var screen_Y = mapSize.height;
                  for (var geozoneIndex =0; geozoneIndex < recursiveObject_g.length; geozoneIndex++) {
                       var geozoneMap = recursiveObject_g[geozoneIndex];
                       var geozonelatLon = new LatLon(parseFloat(geozoneMap.lat), parseFloat(geozoneMap.lon));
                       if (mapBox.contains(geozonelatLon))
                       {
                            try {
                                geozoneMap.coords = eval(geozoneMap.coords);
                                if (geozoneMap.coords.length >= 3) {
                                   var  geoCoordsXy = CheckGeozoneIsInRetangle(geozoneMap.coords, mapBox, screen_X, screen_Y, true)
                                   if (geoCoordsXy.length > 0)
                                   {
                                      isDraw = true;
                                      geozoneMarkers.addPoint(geoCoordsXy);
                                   }
                                }
                            }
                            catch (err) { }
                       }
                       else
                       {
                            try {
                                geozoneMap.coords = eval(geozoneMap.coords);
                                if (geozoneMap.coords.length >= 3) {
                                   var  geoCoordsXy = CheckGeozoneIsInRetangle(geozoneMap.coords, mapBox, screen_X, screen_Y, false)
                                   if (geoCoordsXy.length > 0)
                                   {
                                      isDraw = true;
                                      geozoneMarkers.addPoint(geoCoordsXy);
                                   }
                                }
                            }
                            catch (err) { }
                       }
                  }
                  if (isDraw) {  geozoneMarkers.setFillColor (new Color (0xff, 0xff, 0xff, 0.6));
                   }
               }
               else {if (geozoneMarkers != null) geozoneMarkers.destroy();}
            }

            function CheckCircleIsInRetangle(circleCenter,circleRadius,screen_X, screen_Y, mapBox )
            {
               var isInSize = false;
               var twocircleRadius = circleRadius*circleRadius;
               var circleRadius_1 = Math.sqrt(twocircleRadius + twocircleRadius );
               var southwestPoint = myMap.map.getXY(Telogis.GeoBase.MathUtil.displace (circleCenter, circleRadius_1, 135, Telogis.GeoBase.DistanceUnit.METERS));
               var northeastPoint = myMap.map.getXY(Telogis.GeoBase.MathUtil.displace (circleCenter, circleRadius_1, 315, Telogis.GeoBase.DistanceUnit.METERS));
//               if (mapBox.contains(Telogis.GeoBase.MathUtil.displace (circleCenter, circleRadius_1, 45, Telogis.GeoBase.DistanceUnit.METERS)))
//               {
//                  var s = "1";
//               }
              if ((northeastPoint.x <= screen_X && southwestPoint.x >= 0) &&  (northeastPoint.y <= screen_Y && southwestPoint.y >= 0))
               {
                     isInSize = true;
               }
               return isInSize;
            }     

            function CheckGeozoneIsInRetangle(geozoneCoords, mapBox, screen_X, screen_Y, isContained)
            {
               var coords = [];
               var minX = 999999;
               var minY = 999999;
               var maxX = -999999;
               var maxY = -999999;
               var isInSize = isContained;
               for (var coordIndex = 0; coordIndex < geozoneCoords.length; coordIndex++) 
               {
                  try
                  {
                    var lat_1 = parseFloat(geozoneCoords[coordIndex][0]);
                    var lon_1 = parseFloat(geozoneCoords[coordIndex][1]);
                    var LatLon_1 = new LatLon(lat_1, lon_1);
                    var coorsXy = myMap.map.getXY(LatLon_1);
                    if ( !isInSize && mapBox.contains(LatLon_1)) isInSize = true;
                    if (coorsXy.x < minX) minX = coorsXy.x;
                    if (coorsXy.y < minY) miny = coorsXy.y;
                    if (coorsXy.x > maxX) maxX = coorsXy.x;
                    if (coorsXy.y > maxY) maxY = coorsXy.y;
                    coords.push(coorsXy);
                  }
                  catch(err){}
               }
               if (!isInSize)
               {
                  if ((minX <= screen_X && maxX >= 0) &&  (minY <= screen_Y && maxY >= 0))
                  {
                     isInSize = true;
                  }
               }

               if (!isInSize)  coords = [];
               return coords;

            }

            function landmark_Click(ctl)
            {
                if ($(ctl).attr("checked"))
                   mapCurrentView = null;
                DrawLandmarkcirclesByZoomLevel();
                myMap.enableLayer($(ctl).attr("value"), $(ctl).attr("checked"));
                SetShowLandmarkOpt(ctl);
                return true;
            }

            function VehicleName_Click(ctl)
            {
                if ($(ctl).attr("checked"))
                {
                     $(myMap.map.elem).removeClass("hide_vehicle_label");
                }
                else
                {
                     $(myMap.map.elem).addClass("hide_vehicle_label");
                }

                var postData =  "{'ChkShowVehicleName':'" + $(ctl).attr("checked") + "'}";
                    $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "SetMapOption.asmx/SetShowVehicleName",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {

                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= failedSetValue%>");
                        }
                    },
                    error: function (request, status, error) {
                        alert("<%= failedSetValue%>");
                        return false;
                    }

                    });

            }

            function lsdMapping_click(ctl)
            {
                if ($(ctl).attr("checked"))
                {
                    this.tileConfig = {
					    serverPage: "renderers/LSDRenderer.aspx",
					    args: {
					    }
				    }
				
				    lsdtileLayer = new TileLayer({
											    id: 'main_map_auxiliary',
											    map: myMap.map,
											    tileConfig: this.tileConfig
											    });
                }
                else 
                {
                    if (lsdtileLayer != null )
                        lsdtileLayer.destroy();
                }

            }

            function LandmarkName_Click(ctl)
            {
                if ($(ctl).attr("checked"))
                {
                     $(myMap.map.elem).removeClass("hide_landmark_label");
                }
                else
                {
                     $(myMap.map.elem).addClass("hide_landmark_label");
                }

//                finishLankMark = false;
//                recursiveIndex_l = 0;
//                ShowMapLoadingText();
//                $(myMap.map.elem).find("#lank_mark_label")
//                RecursiveShowLandmarks();

                 var postData =  "{'ChkShowLandmarkname':'" + $(ctl).attr("checked") + "'}";
                    $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "SetMapOption.asmx/SetShowLandmarkName",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {

                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= failedSetValue%>");
                        }
                    },
                    error: function (request, status, error) {
                        alert("<%= failedSetValue%>");
                        return false;
                    }

                    });

            }

            function geozone_Click(ctl)
            {
                myMap.setGeozoneVisibility($(ctl).attr("checked"));
                SetShowGeoZonesOpt(ctl);
            }
 
            function LandmarkRadius_Click(ctl)
            {
               DrawLandmarkcirclesByZoomLevel();
                    var postData =  "{'ChkShowLandmarkRadius':'" + $(ctl).attr("checked") + "'}";
                    $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "SetMapOption.asmx/SetShowLandmarkRadius",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {

                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= failedSetValue%>");
                        }
                    },
                    error: function (request, status, error) {
                        alert("<%= failedSetValue%>");
                        return false;
                    }

                  });
            }

            function SetShowGeoZonesOpt(ctl)
            {
               if (!hasDrawGeozone && $(ctl).attr("checked"))
               {
                    var postData =  "{'ChkShowGeoZones':'" + $(ctl).attr("checked") + "','isQueryData':'1'}";
                    $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "SetMapOption.asmx/SetShowGeoZones",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                           if (data.d == '') return;
                           hasDrawGeozone = true;
                           var geoData = '';
                           recursiveObject_g = eval(data.d);
                           recursiveIndex_g = 0;
                           finishGeozone = false;
                           ShowMapLoadingText();
                           myMap.BeginShowGeozone();
                           RecursiveShowGeozones();

                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= failedSetValue%>");
                        }
                    },
                    error: function (request, status, error) {
                        alert("<%= failedSetValue%>");
                        return false;
                    }

                  });
                  return;
               }
               else
               {
                    var postData =  "{'ChkShowGeoZones':'" + $(ctl).attr("checked") + "','isQueryData':'0'}";
                    $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "SetMapOption.asmx/SetShowGeoZones",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= failedSetValue%>");
                        }
                    },
                    error: function (request, status, error) {
                        alert("<%= failedSetValue%>");
                        return false;
                    }
                  });
                  return;
               }
            }

            function SetShowLandmarkOpt(ctl)
            {
                    var postData =  "{'ChkShowLandmark':'" + $(ctl).attr("checked") + "'}";
                    $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "SetMapOption.asmx/SetShowLandmark",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            alert("<%= failedSetValue%>");
                        }
                    },
                    error: function (request, status, error) {
                        alert("<%= failedSetValue%>");
                        return false;
                    }

                  });
            }
     </script>

      <div id="mapContainer" style="border: 1px solid black; height:337px;width:100%" >
    </div>

    <div id="topControlArea" ></div>

    <!-- TODO: translation for tooltips -->
    <div id="panControlPanel" >
        <map id="panImageMap" name="panImageMap">
            <area alt="Pan Up"    shape="poly" coords="0,0, 23,23, 46,0" nohref="true" onclick="myMap.panUp();"  title="Pan Up" />
            <area alt="Pan Left" id="areaLeft"  shape="poly" coords="0,0, 23,23, 0,46" nohref="true" onclick="myMap.panLeft();"  title="Pan Left" />
            <area alt="Pan Right" shape="poly" coords="46,0, 23,23, 46,46" nohref="true" onclick="myMap.panRight();"  title="Pan Right" />
            <area alt="Pan Down"  shape="poly" coords="0,46, 23,23, 46,46" nohref="true" onclick="myMap.panDown();"  title="Pan Down" />
        </map>
        <img src="images/pan.png" ismap="true" usemap="#panImageMap" class="panImage" width="46px" height="46px" />
    </div>

 <div id="topMenu" class="mapControlPanel" >
        <!-- dropdown menu author: http://javascript-array.com/scripts/jquery_simple_drop_down_menu/ -->
        <!-- <input id="recenterButton" type="button" onclick="myMap.recenter()" value="Recenter" disabled="disabled" /> -->

        <ul id="jsddm">
            <li class="toggle">
                <span id="recenterButton" onclick="myMap.recenter();DrawLandmarkcirclesByZoomLevel();" >Recenter</span>
            </li>
            <li class="toggle">
                <span id="roadsLink" onclick="myMap.satelliteImagery(false);$('#satelliteLink').removeClass('selected');$('#roadsLink').addClass('selected');" class="selected">Roads</span>
            </li>
            <li class="toggle">
                <span id="satelliteLink"  onclick="myMap.satelliteImagery(true);$('#roadsLink').removeClass('selected');$('#satelliteLink').addClass('selected');">Satellite</span>
            </li>
            <li class="dropdown">
                <span>Options</span>
                <ul>
                    <li>
                        <input id="labelsOption" type="checkbox" onchange="return VehicleName_Click(this);" value="labels"  /><span id="spanlabelsOption">Vehicle Name</span>
                    </li>
                    <li>
                        <input id="assetLayerOption" type="checkbox" onchange="myMap.enableLayer(this.value, this.checked);" value="assetLayer" checked="checked" /><span id="spanassetLayerOption">Assets / History </span>
                    </li>
                    <li class="subitem">
                        <input id="trailsOption" type="checkbox" onchange="myMap.enableTrails(this.checked);" value="trails" checked="checked" /><span id="spantrailsOption"> Trails</span>
                    </li>
                    <li>
                        <input id="landmarkLayerOption" type="checkbox" onclick="return landmark_Click(this)" value="landmarkLayer"  /><span id="spanlandmarkLayerOption">Landmarks</span>
                    </li>
                    <li>
                        <input id="geozoneLayerOption" type="checkbox" onclick="return geozone_Click(this)"  value="geozoneLayer"  /><span id="spangeozoneLayerOption">Geozones</span>
                    </li>
                    <li>
                        <input id="showLandmarkRadius" type="checkbox" onchange="return LandmarkRadius_Click(this)"   /><span id="spanshowLandmarkRadius">Landmark Radius</span>
                    </li>
                    <li>
                        <input id="showLandmarkName" type="checkbox" onchange="return LandmarkName_Click(this)"   /><span id="spanshowLandmarkName">Landmark Name</span>
                    </li>
                    <li>
                        <input id="lsdMapping" type="checkbox" onchange="return lsdMapping_click(this)"   /><span id="spanlsdMapping">LSD Mapping</span>
                    </li>

                </ul>
            </li>
            <li class="toggle">
                <span id="printmap" onclick="window.print();" >Print</span>
            </li>
        </ul>
    </div>

    <%--<div id="divMapLoadingText" style="
	position: absolute;
	left: 90px;
	top: 26px;
	z-index: 150;
	background-color: White;
	color: Black;
	font-size: large;
	padding: 3px 10px 3px 10px;"   ><span>Loading...</span></div>
    </div>--%>