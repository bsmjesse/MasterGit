<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OpenLayerMaps_Reefer.aspx.cs" Inherits="SentinelFM.MapNew_OpenLayerMaps_Reefer" %>

<html>
  <head>
      <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0">
    <meta name="apple-mobile-web-app-capable" content="yes">
<meta http-equiv="X-UA-Compatible" content="IE=8" />
    <title>Track vehicles on map</title>
<%--    <link rel="stylesheet" href="../openlayers/theme/default/style.css" type="text/css">
    <link rel="stylesheet" href="../maps/style.css" type="text/css">--%>
     <link rel="stylesheet" href="../openlayers/theme/default/style.css" type="text/css">
    <link rel="stylesheet" href="../maps/style.css" type="text/css">
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
   
      <style type="text/css">
          html, body {
              height: 100%;
              margin:0px 0px;
          }
          #map {
              width: 100%;
              height: 100%;
              border: 1px solid black;
          }
          .olPopup p { margin:0px; font-size: .9em;}
          .olPopup h2 { font-size:1.2em; }
          
        .cmbfonts
        {
            font:normal 12px tahoma, arial, verdana, sans-serif !important;margin-right:10px;
        }
        
        #toolbar .toolbar-transparent
        {
            border:0;
            background-color: #B5B2AE !important;
            background-image: none;                        
        }
        
        .message
        {
            background: none repeat scroll 0 0 #F9EDBE;
            border: 1px solid #F0C36D !important;
            border-radius: 2px 2px 2px 2px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
            font-size: 95%;
            line-height: 29px;
            padding-left: 16px;
            padding-right: 16px;
        }
        
        .searchmessage
        {
            background: none repeat scroll 0 0 #F9EDBE;
            border: 1px solid #F0C36D !important;
            border-radius: 2px 2px 2px 2px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
            font-size: 95%;
            line-height: 29px;
            padding-left: 16px;
            padding-right: 16px;
            color: Red;
        }
        
        .maptoolbarselected
        {
            background: none repeat scroll 0 0 #ffffff !important;
        }
        
        .olControlLayerSwitcher {
            top: 5px !important;
        }
        
        .olControlLayerSwitcher .maximizeDiv, .olControlLayerSwitcher .minimizeDiv {
            top: 0 !important;
        }
        
        .olControlPanZoomBar 
        {
            top: 28px !important;
            right: 50px !important;
            left: auto !important;
        }
        
        .olControlPanZoom
        {
            top: 28px !important;
            right: 50px !important;
            left: auto !important;
        }
        
        .olbsmtoolbar
        {
            border: 0; z-index: 1000; position: absolute; top: 10px; left: 15px; width: 100px;
        }
        
        .mapsearchtoolbar
        {
            border: 0; z-index: 1000; position: absolute; top: 8px; left: 120px; width: 450px; height: 35px;display:none;
        }
        .mapsearchtoolbar input
        {
            margin-top: 2px; margin-left: 5px;           
        }
        
        .olControlScaleLine 
        {
            bottom: 40px !important;
        }
        
        .olControlMousePosition
        {
            bottom: 15px !important;
        }
        
        .SearchAddressForms 
        {
            margin: 0;
        }
        
        #searchAddressBtnW 
        {
            display: inline-block;
            margin: 1px 15px 0 10px;
            vertical-align: top;
            padding: 0;
        }
        
        .searchAddressBtn 
        {
            background-color: #4D90FE;
            background-image: -moz-linear-gradient(center top , #4D90FE, #4787ED);
            border: 1px solid #3079ED;
            color: #FFFFFF !important;
            margin: 0;
            -moz-user-select: none;
            border-radius: 2px 2px 2px 2px;
            cursor: default !important;
            display: inline-block;
            font-weight: bold;
            height: 29px;
            line-height: 29px;
            min-width: 54px;
            padding: 0 8px;
            text-align: center;
            text-decoration: none !important;
        }
        
        /*.searchAddressBtn:focus {
            border: 1px solid #4D90FE;
            box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.5) inset;
            outline: medium none;
        }*/
        
        .searchAddressBtn:hover {
            background-color: #357AE8;
            background-image: -moz-linear-gradient(center top , #4D90FE, #357AE8);
        }
        
        .searchAddressBtnI 
        {
            display: inline-block;
            height: 13px;
            margin: 7px 19px;
            width: 14px;
            background-image: url("../images/search.png");
        }
        
        .ui-datepicker{ z-index: 9999 !important;}
        button.ui-datepicker-current { display: none; }
      </style>
         <script type="text/javascript">
             var orgid = '<%=sn.User.OrganizationId %>';
             var interval = '<%=sn.User.GeneralRefreshFrequency %>';
             var GoogleAddressService;
      </script>
       <script src="../scripts/json2.js"></script>
<%--      <script src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.3&mkt=en-us" type="text/javascript"></script>  --%> 
  
     <script type="text/javascript" src="../sencha/extjs-4.1.0/bootstrap.js"></script>
     
     <%if (ifShowGoogleStreets || ifShowGoogleHybrid)  { %>
        <%-- <script src="<%=ISSECURE ? "https" : "http" %>://maps.google.com/maps/api/js?v=3.5&amp;sensor=false"></script>--%> 
        <%--<script src="<%=ISSECURE ? "https" : "http" %>://maps.googleapis.com/maps/api/js?v=3&client=gme-bsmwirelessinc&sensor=true"></script>--%>
     <%} %>

     <script type="text/javascript" src="<%=ISSECURE ? "https" : "http" %>://maps.googleapis.com/maps/api/js?v=3&client=gme-bsmwirelessinc&sensor=true&libraries=places"></script>


     <%if (!ISSECURE && (ifShowBingRoads || ifShowBingHybrid))
       { %>
        <script src='http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.1'></script> 
     <%} %>

     <%if (!ISSECURE && ifShowAerial)
       { %>
     <script src="http://api.maps.yahoo.com/ajaxymap?v=3.0&appid=euzuro-openlayers"></script>
     <%} %>
     
    <%--<script type="text/javascript" src="../Openlayers/OpenLayers-2.11/lib/OpenLayers.js"></script>--%>
    <script src="../Openlayers/OpenLayers-2.11/OpenLayers.js"></script>
	<script src="../Openlayers/OpenLayers-2.11/lib/deprecated.js"></script>
    <script type="text/javascript" src="../Scripts/OpenLayerMap/utils/cloudmade.js"></script>
    <script src="../Scripts/OpenLayerMap/OpenLayerMap_Reefer.js?v=<%=LastUpdatedOpenLayerMapJs %>"></script>
    
        
    <%if (ifShowArcgis)  { %>
        <script src="../Openlayers/OpenLayers-2.11/lib/OpenLayers/Layer/ArcGISCache.js" type="text/javascript"></script>
        <script src="../Scripts/OpenLayerMap/utils/arcgislayers.js"></script>
    <%} %>

         
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>    
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    

     <script type="text/javascript" src="mapmenus.js"></script>

     <script type="text/javascript">
         var ISSECURE = <%=ISSECURE.ToString().ToLower() %>;   
         var ShowAssignToFleet = <%=ShowAssignToFleet.ToString().ToLower() %>;
         var ShowMapHistorySearch = <%=ShowMapHistorySearch.ToString().ToLower() %>;
         var ShowRouteAssignment = <%=ShowRouteAssignment.ToString().ToLower() %>;
         var ShowCallTimer = <%=ShowCallTimer.ToString().ToLower() %> ;
         <%if(ShowCallTimer){ %>
            var CallTimerSelections = "<%=CallTimerSelections %>";
         <%} %>
         
         function assignGeozoneToFleet(geozoneId) {
            var url = "Widgets/FleetAssignment.aspx?objectName=geozoneid&objectId=" + geozoneId;
            var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
            parent.openPopupWindow("Assign to fleet", urlToLoad, 520, 320);
        }    
        $(document).ready(function() {
            GoogleAddressService = new IniGoogleAutoComplete();
            //$( "#txtSearchAddress" ).datepicker();

            $('#txtSearchAddress').focus(function(){
                $('#clear').toggle();
            });
        });
     </script>

    <link rel="stylesheet" type="text/css" href="menus.css" />  
    <script type="text/javascript" src="../scripts/NewMap/landmarkEditForm.js"></script>   
  </head>
  <body onload="init()">
    <div id="toolbar" class="olbsmtoolbar"></div>
    <div id="mapsearchtoolbar" class="mapsearchtoolbar"><input type="text" style="width:300px;height:26px;" class="formtext bsmforminput" id="txtSearchAddress" name="txtSearchAddress" /> 
        <%--<a href="javascript:void(0)" onclick="searchAddress();"><img src="../images/searchicon.png" alt="search" /></a>--%>
        <div id="searchAddressBtnW">
            <button id="searchAddressBtn" class="searchAddressBtn" onclick="searchAddress();">
                <span class="searchAddressBtnI"></span>
            </button>
        </div>
    </div>
    <div id="undobar" style="border: 0; z-index: 1000; position: absolute; top: 10px; left: 220px; width: 300px; display:none" class="message">The shape has been changed. <span id="undolink"><a href="javascript:void(0)" onclick="undopolygon();">Undo</a></span> (<span id="undonum">1</span>)<span id="undosaving" style="margin-left:10px;">Saving...</span><span id="savechanges"><a href="javascript:void(0)" onclick="savechanges();" style="margin-left:10px;">Save</a> <a href="javascript:void(0)" onclick="cancelchanges();">Cancel</a></span></div>
    <div id="messagebar" style="border: 0; z-index: 1000; position: absolute; top: 10px; left: 220px; width: 300px; display:none" class="message"></div>
    <div id="searchmessage" style="border: 0; z-index: 1000; position: absolute; top: 40px; left: 125px; width: 300px; display:none" class="searchmessage"></div>
    <div id="map" class="largemap"></div>

     <div id="controls" style="display:none">
        <ul id="controlToggle">
            <li>
                <input type="radio" name="type" value="none" id="noneToggle" onclick="toggleControl(this);" checked="checked" />
                <label for="noneToggle">navigate</label>
            </li>
            <li>
                <input type="radio" name="type" value="select" id="selectToggle" onclick="toggleControl(this);" />
                <label for="selectToggle">select polygon on click</label>
            </li>            
            <li>
                <input type="radio" name="type" value="circle" id="circleToggle" onclick="toggleControl(this);" />
                <label for="pointToggle">draw circle</label>
            </li>
            <li>
                <input type="radio" name="type" value="polygon" id="polygonToggle" onclick="toggleControl(this);" />
                <label for="polygonToggle">draw polygon</label>
            </li>
            <li>
                <input type="radio" name="type" value="modify" id="modifyToggle" onclick="toggleControl(this);" />
                <label for="modifyToggle">modify feature</label>
                
            </li>
        </ul>
    </div>
    <div id="map_canvas"></div>
  </body>
</html>
