<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="RouterBuilder_Main" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!--
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
-->
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <!-- metatags -->
<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
<title>SentinelFM Route Builder</title>
<meta name="description" content="OSRM Website"/>
<meta name="author" content="Dennis Schieferdecker" />

<!-- stylesheets -->
<link rel="stylesheet" href="leaflet/leaflet.css" type="text/css"/>
<link rel="stylesheet" href="gh-buttons.css" />
<link rel="stylesheet" href="../ServiceAssignment/Scripts/bootstrap/css/bootstrap.css" />
<!--[if lte IE 8]>
    <link rel="stylesheet" href="leaflet/leaflet.ie.css" type="text/css"/>
<![endif]-->
<link rel="stylesheet" href="main.css" type="text/css"/>
<link rel="stylesheet" href="js/media/smoothness/jquery-ui.css" />
<link rel="stylesheet" href="js/media/css/demo_table.css" />
<link rel="stylesheet" href="js/media/css/demo_page.css" />
    <link rel="stylesheet" href="js/media/css/TableTools.css" />  
    <link href="js/leafletmarker/font-awesome.css" rel="stylesheet" />
<link rel="stylesheet" href="js/leafletmarker/leaflet.awesome-markers.css" />
    <link rel="stylesheet" href="leaflet/leaflet.css" />
<!-- scripts -->
  
<script src="leaflet/leaflet-src.js" type="text/javascript"></script>
<script src="js/leafletmarker/leaflet.awesome-markers.js" type="text/javascript"></script>
<script src="base/leaflet/L.Bugfixes.js" type="text/javascript"></script>
<script src="base/leaflet/L.LabelMarker.js" type="text/javascript"></script>
<script src="base/leaflet/L.LabelMarkerIcon.js" type="text/javascript"></script>
<script src="base/leaflet/L.BingLayer.js" type="text/javascript"></script>
<script src="OSRM.base.js" type="text/javascript"></script>
<script src="OSRM.config.js" type="text/javascript"></script>
<script src="utils/OSRM.browsers.js" type="text/javascript"></script>
<script src="utils/OSRM.classes.js" type="text/javascript"></script>

<script src="main.js" type="text/javascript"></script>
<!-- <script defer="defer" src="OSRM.debug.js" type="text/javascript"></script> -->

<script src="base/osrm/OSRM.Control.Layers.js" type="text/javascript"></script>
<script src="base/osrm/OSRM.Control.Locations.js" type="text/javascript"></script>
<script src="base/osrm/OSRM.Control.Zoom.js" type="text/javascript"></script>
<script src="base/osrm/OSRM.Control.Map.js" type="text/javascript"></script>
<script src="base/osrm/OSRM.Marker.js" type="text/javascript"></script>
<script src="base/osrm/OSRM.Route.js" type="text/javascript"></script>

<script src="base/OSRM.Map.js?_v=<%=DateTime.Now.Ticks %>" type="text/javascript"></script>
<script src="base/OSRM.Markers.js" type="text/javascript"></script>
<script src="base/OSRM.Routes.js" type="text/javascript"></script>
<script src="base/OSRM.HistoryRoutes.js" type="text/javascript"></script>
<script src="gui/OSRM.GUI.js" type="text/javascript"></script>
<script src="gui/OSRM.GUIBoxGroup.js" type="text/javascript"></script>
<script src="gui/OSRM.GUIBoxHandle.js" type="text/javascript"></script>
<script src="gui/OSRM.Selector.js" type="text/javascript"></script>
<script src="gui/OSRM.MainGUI.js" type="text/javascript"></script>
<script src="gui/OSRM.Notifications.js" type="text/javascript"></script>
<script src="routing/OSRM.Routing.js" type="text/javascript"></script>
<script src="routing/OSRM.RoutingAlternatives.js" type="text/javascript"></script>
<script src="routing/OSRM.RoutingDescription.js" type="text/javascript"></script>
<script src="routing/OSRM.RoutingGeometry.js" type="text/javascript"></script>
<script src="gui/OSRM.RoutingGUI.js" type="text/javascript"></script>
<script src="gui/OSRM.RoutingEngineGUI.js" type="text/javascript"></script>
<script src="routing/OSRM.RoutingNoNames.js" type="text/javascript"></script>
<script src="base/OSRM.Via.js" type="text/javascript"></script>
<script src="base/OSRM.Geocoder.js" type="text/javascript"></script>

<script src="utils/OSRM.CSS.js" type="text/javascript"></script>
<script src="utils/OSRM.JSONP.js" type="text/javascript"></script>
<script src="localization/OSRM.Localization.js" type="text/javascript"></script>
<script src="printing/OSRM.Printing.js" type="text/javascript"></script>
<script src="utils/OSRM.Utils.js" type="text/javascript"></script>





  <script type="text/javascript" src="js/ui/jquery-1.9.1.min.js"></script>
  <script type="text/javascript" src="js/ui/jquery-ui.js"></script>
        <script src="js/media/js/jquery.dataTables.js" type="text/javascript"></script>
    <script type="text/javascript" src="js/ckeditor/ckeditor.js"></script>        
        <link rel="stylesheet" href="leaflet-cluster/MarkerCluster.css" />
    <link rel="stylesheet" href="leaflet-cluster/MarkerCluster.Default.css" />
    <link rel="stylesheet" href="js/bootstrap/css/bootstrap.css" />
    <script src="js/bootstrap/js/bootstrap.js" type="text/javascript"></script>
    <script type="text/javascript" src="leaflet-cluster/leaflet.markercluster.js"></script>   
    <script type="text/javascript" charset="utf-8" src="js/media/js/ZeroClipboard.js"></script>
	<script type="text/javascript" charset="utf-8" src="js/media/js/TableTools.js"></script>    
    <script type="text/javascript" src="js/SpeedDispute.js"></script>
    <script type="text/javascript" src="js/Route.js?_v=<%=DateTime.Now.Ticks %>"></script>
    <script type="text/javascript" src="js/SpeedDispute.js?_v=<%=DateTime.Now.Ticks %>"></script>
    <%
        if (DisputeLinkAdmin)
        {
    %>

    <script type="text/javascript" src="js/Dispute.js"></script>
    <%
        }
    %>
 <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places" type="text/javascript"></script>
    <script type="text/javascript">
        var customizedUrl = '';
        var addressesCollection = '<%=WayPints%>';
        var metricSystem = "metric";
        var modifiedDisputeLink = {};
        OSRM.G.NavTEQPreferences = "<%=Preferences%>";
        OSRM.G.RouteProvider = 'NavTEQ';       
        OSRM.G.IsHgiUser = "<%=HgiUser%>" == "True";
        OSRM.G.UserLanguage = "<%=UserLanguage%>";
        OSRM.G.OrganizationId = "<%=sn.User.OrganizationId%>";
        OSRM.G.IsDblClicked = true;
        OSRM.G.NavTeqRouteId = "<%=RouteId%>";
    //var RouteProvider = 'OSRM';
    </script>       
        <style type="text/css">
            body {
                font-size: 62.5%;
            }

            label, input {
                display: inline;
            }

                input.text {
                    margin-bottom: 12px;
                    width: 95%;
                    padding: .4em;
                }

            fieldset {
                padding: 0;
                border: 0;
                margin-top: 25px;
            }

            h1 {
                font-size: 1.2em;
                margin: .6em 0;
            }

            div#users-contain {
                width: 350px;
                margin: 20px 0;
            }

                div#users-contain table {
                    margin: 1em 0;
                    border-collapse: collapse;
                    width: 100%;
                }

                    div#users-contain table td, div#users-contain table th {
                        border: 1px solid #eee;
                        padding: .6em 10px;
                        text-align: left;
                    }

            .ui-dialog .ui-state-error {
                padding: .3em;
            }

            .validateTips {
                border: 1px solid transparent;
                padding: 0.3em;
            }

            table#OrganizationSkillsTable {
                font-family: Verdana,Arial;
                font-size: 12px;
            }

            .ui-autocomplete-loading {
                background: white url('http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.2/themes/smoothness/images/ui-anim_basic_16x16.gif') right center no-repeat;
            }

            .distance-description {
                display: none;
            }

            .street {
                font-weight: 600;
            }
            .corrected {
                color: green;
            }

            .correct {
                color: orange;
            }

            .dismissed {
                color: red;
            }
        </style>
</head>
<body>
  <!-- old browser warning -->
<![if IE]> 
<script type="text/javascript">
    /*****/
    var disableLoading = false;

    var agentStr = navigator.userAgent;
    
    var mode;
    
    if (agentStr.indexOf("Trident/6.0") > -1) {
        if (agentStr.indexOf("MSIE 7.0") > -1) {
            mode = "IE8 Compatibility View";
            disableLoading = true;
        }            
        else
            mode = "IE8";
    }
    else if (agentStr.indexOf("Trident/5.0") > -1) {
        if (agentStr.indexOf("MSIE 7.0") > -1) {
            mode = "IE9 Compatibility View";
            disableLoading = true;
        }    
        else
            mode = "IE9";
    }
    else if (agentStr.indexOf("Trident/4.0") > -1) {
        if (agentStr.indexOf("MSIE 7.0") > -1) {
            mode = "IE8 Compatibility View";
            disableLoading = true;
        }            
        else
            mode = "IE8";
    }    
    else
        mode = "IE7";
    if (disableLoading) {
        document.title = "Browser Mode:\t" + mode;
        document.write("<h3>You are using a compatibility mode or browser that is not supported by the SentinelFM website. Please switch back to normal mode or upgrade to a more recent browser.</h3>");
        document.execCommand('Stop');
    }
    /***/
</script>
<![endif]>

    <!-- map -->
<div id="map"></div>

<!-- exclusive notification -->
<div id="exclusive-notification-blanket">
<div id="exclusive-notification-wrapper" class="box-wrapper not-selectable">
<div id="exclusive-notification-content" class="box-content">
	<!-- header -->
	<div id="exclusive-notification-toggle" class="iconic-button cancel-marker top-right-button"></div>
	<div id="exclusive-notification-label" class="box-label">Notification</div>

	<!-- notification text -->
	<div id="exclusive-notification-box"></div>
</div>
</div>
</div>

<!-- tooltip notification -->
<div id="tooltip-notification-wrapper" class="box-wrapper not-selectable">
<div id="tooltip-notification-content" class="box-content">
	<!-- header -->
	<div id="tooltip-notification-toggle" class="iconic-button cancel-marker top-right-button"></div>
	<div class="quad top-right-button"></div>
	<div id="tooltip-notification-resize" class="iconic-button cancel-marker top-right-button"></div>
	<div id="tooltip-notification-label" class="box-label">Notification</div>

	<!-- notification text -->
	<div id="tooltip-notification-box"></div>
</div>
</div>

<!-- config gui -->
<div id="config-wrapper" class="box-wrapper">
<div id="config-content" class="box-content">
	<!-- header -->
	<div id="config-toggle" class="iconic-button cancel-marker top-right-button"></div>
	<div id="gui-config-label" class="box-label">Configuraion</div>

	<!-- config options -->
	<div class="full">
	<div class="row">
	<div class="left fixed"><span id="gui-language-2-label" class="config-label">Language:</span></div>
	<div class="left stretch"><select id="gui-language-2-toggle" class="config-select"></select></div>
	</div>
	<div class="row">
	<div class="left fixed"><span id="gui-units-label" class="config-label">Units:</span></div>
	<div class="left stretch"><select id="gui-units-toggle" class="config-select"></select></div>
	</div>
	</div>
	
	<!-- gui & data timestamps -->
	<div id="config-timestamps">
	<div class="row">
	<div class="right small-font"><span id="gui-timestamp-label">gui:</span></div>
	<div class="right small-font"><span id="gui-timestamp">v0.0.0 010180</span></div>
	</div>
	<div class="row">
	<div class="right small-font"><span id="gui-data-timestamp-label">data:</span></div>
	<div class="right small-font"><span id="gui-data-timestamp">n/a</span></div>
	</div>	
	</div>
</div>
</div>

<!-- mapping gui -->
<div id="mapping-wrapper" class="box-wrapper">
<div id="mapping-content" class="box-content">
	<!-- header -->
	<div id="mapping-toggle" class="iconic-button cancel-marker top-right-button"></div>
	<div id="gui-mapping-label" class="box-label">Mapping Tools</div>
	
	<!-- mapping options -->
	<form id="mapping-checkboxes" class="small-font">
	<div>
		<label id="gui-option-highlight-nonames-label">
			<input type="checkbox" id="option-highlight-nonames" value="show-previous-routes"/>
			Highlight unnamed streets
		</label>
		<label id="gui-option-show-previous-routes-label">
			<input type="checkbox" id="option-show-previous-routes" value="show-previous-routes"/>
			Show previous routes
		</label>
	</div>
	</form>
	
	<!-- off-site links -->
	<a class="button mapping-button" id="open-osmbugs">OSM Bugs</a><span class="quad mapping-button"></span><a class="button mapping-button" id="open-josm">JOSM</a>
</div>
</div>

<!-- main gui -->
<div id="main-wrapper" class="box-wrapper">

<!-- input box -->
<!----operation section begin----->
<div id="main-window" class="box-content">
<div id="main-operation">
	<!--  header -->
	<div id="input-mask-header">
	<select id="gui-language-toggle" class="top-left-button"></select>
	<div id="main-toggle" class="iconic-button cancel-marker top-right-button"></div>
	</div>
	
	<!--  input mask -->
	<div id="input-mask">
	<%
        string p1 = "";
        string p2 = "";
        if (!string.IsNullOrEmpty(WayPints))
        {
            string[] myWayPoints = WayPints.Split('|');
            p1 = myWayPoints[0];
            p2 = myWayPoints[1];
        }
	          
	%>
	<!-- source/target input -->
	<div id="input-source-group" class="full">
	<div id="input-source" class="input-marker">
	<div class="left"><span  id="gui-search-source-label" class="input-label">From:</span></div>
	<div class="center input-box"><input id="gui-input-source" class="input-box" type="text" maxlength="200" value="<%=p1 %>" title="Enter start" size="38" /></div>
	<div class="left"><div id="gui-delete-source" class="iconic-button cancel-marker input-delete"></div></div>
	<div class="right"><a class="button" id="gui-search-source">Show</a></div>
	</div>

	<div id="input-target" class="input-marker">
	<div class="left"><span id="gui-search-target-label" class="input-label">To:</span></div>
	<div class="center input-box"><input id="gui-input-target" class="input-box" type="text" maxlength="200" value="<%=p2 %>" title="Enter destination" size="38" /></div>	
	<div class="left"><div id="gui-delete-target" class="iconic-button cancel-marker input-delete"></div></div>
	<div class="right"><a class="button" id="gui-search-target">Show</a></div>	
	</div>        
	</div>
        <div style="display:inline;">
            <div class="center"><a href="#" onclick="AddMorePoints('')">Add more way points</a></div>
    <div class="center" id="divPreference"><a href="#" onclick="SetPreference()">&nbsp;|&nbsp;Set route preferences</a></div>
    <div class="right" style="float:right;">Provider: 
        <select id="EngineProvider">
            <option value="NavTEQ" selected="selected">NavTeq</option>
            <option value="OSRM">OSRM</option>
        </select>

    </div>
        </div>
	
    <div class="quad"></div>	
	
	<!-- action buttons -->
    <div class="full">
    <div class="row">
    <div class="left"><a class="button" id="gui-reset">Reset</a></div>
	<div class="center"><select id="gui-engine-toggle" class="engine-select"></select></div>
	<div class="right"><a class="button" id="gui-reverse">Reverse</a></div>
    </div>
    </div>
    </div>
    <!--
</div>
<div id="main-output" class="box-content">
        -->
    <br />
    <div align="center" style="height: 15px;">       
        <!-- test buttons -->
        <div class="actions button-container boostrap" style="display:inline;">
    <a href="#" id="routeView" class="button primary icon home">Routes</a>
    <%
        if (DisputeLinkAdmin)
        {
    %>
    
            <!--<a href="#" id="showDisputes" class="button primary icon home">Disputes Speed</a>-->
           <div class="btn-group">
                <a href="#" class="button primary icon home" data-toggle="dropdown">Disputes Speed</a>
                <ul class="dropdown-menu">
                  <li><a href="#" id="showAll">All</a></li>
                    <li><a href="#" id="showDisputed">Disputed</a></li>
                    <li><a href="#" id="showAccepted">Accepted</a></li>
                    <li><a href="#" id="showDismissed">Dismissed</a></li>
                 <li><a href="#" id="showSegements">Route segments</a></li>
                </ul>
              </div>
   <%
        }
   %>            
    <div id="showActions" class="button-group" style="display:none;">
        <a href="#" id="routeSave" class="button primary icon add">Save</a>
        <a href="#" id="routeExport" class="button">Export</a>
        <a href="#" id="routeEmail" class="button icon mail">Email</a>
    </div>
                        
    <br />
    </div>
        <!-- test buttons end-->
    </div>
    <br/>

	<div id="information-box-header"></div>
	<div id="information-box"></div>    
	
    </div>
  </div>
<!----operation section end----->
</div>

<div class="demo">

<div id="dialog-form" title="Create new route">
<form id="routeForm" runat="server">
	<fieldset>
		<label for="routeName">Route Name*</label>
		<input type="text" name="routeName" id="routeName" value="<%=(LandmarkInfo.ContainsKey("landmarkname") ? LandmarkInfo["landmarkname"] : "") %>" class="text ui-widget-content ui-corner-all" />
        <label for="routeBuffer">Route Buffer(Meters)*</label>
        <input type="text" name="routeBuffer" id="routeBuffer" value="<%=(LandmarkInfo.ContainsKey("radius") ? LandmarkInfo["radius"] : "") %>" class="text ui-widget-content ui-corner-all" />
        <label for="txtName">Contact Person*</label>
        <input type="text" name="txtName" id="txtName" value="<%=(LandmarkInfo.ContainsKey("contactpersonname") ? LandmarkInfo["contactpersonname"] : "") %>" class="text ui-widget-content ui-corner-all" />
        <label for="txtEmail">Route Email*</label>
		<input type="text" name="txtEmail" id="txtEmail" value="<%=(LandmarkInfo.ContainsKey("email") ? LandmarkInfo["email"] : "") %>" class="text ui-widget-content ui-corner-all" />
        <label for="txtPhone">Contact Phone</label>
		<input type="text" name="txtPhone" id="txtPhone" value="<%=(LandmarkInfo.ContainsKey("phone") ? LandmarkInfo["phone"] : "") %>" class="text ui-widget-content ui-corner-all" />
        <!--<label for="timeZone">Time Zone</label>-->
		<asp:DropDownList ID="timeZone" name="timeZone" runat="server">
        </asp:DropDownList>
        <label for="chkEnableService">Active Route Service</label>
        <input type="checkbox" name="chkEnableService" id="chkEnableService" value="1" <%=(ServiceConfigId > 0 ? "CHECKED" : "") %> /> <br />
        <%
            if (DisputeLinkAdmin)
            {
        %>
        <label for="chkSpeed">Speed Correction</label>
        <input type="checkbox" name="chkSpeed" id="chkSpeed" value="1" /><br />
        <label for="correctSpeed" style="display:none;">Speed(<strong>mph</strong> in USA; <strong>km/h</strong> in Canada. *Only numberic value)</label>
        <input type="text" name="correctSpeed" id="correctSpeed" style="display:none;" class="text ui-widget-content ui-corner-all" />
        <br />
        <%
                if (HgiUser)
                {
        %>
                    <label for="gblSegment" style="display:none;">Global speed</label>
                    <input type="checkbox" name="gblSegment" id="gblSegment" value="1" style="display:none;" /><br />
        <%        
                }
            }
        %>
        <label for="txtDescription">Route Description</label>
        <textarea name="txtDescription" id="txtDescription" rows="2" cols="46" class="textarea ui-widget-content ui-corner-all">
<%=(LandmarkInfo.ContainsKey("description") ? LandmarkInfo["description"] : "")%>
</textarea>	
        <input type="hidden" id="txtEditLink" name="txtEditLink" value=""/>
        <input type="hidden" id="txtPreference" name="txtPreference" value=""/>
        <input type="hidden" id="txtWayPoints" name="txtWayPoints" value=""/>
        <input type="hidden" id="txtQueueString" name="txtQueueString" value=""/>
        <input type="hidden" id="LandmarkId" name="LandmarkId" value="<%=LandmarkId %>"/>
         <input type="hidden" id="NavTeqResponse" name="NavTeqResponse" value=""/>
        <input type="hidden" id="correctionid" name="correctionid" value=""/>
        <input type="hidden" id="disputeId" name="disputeId" value=""/>
        <input type="hidden" id="disputeComment" name="disputeComment" value=""/>
        <input type="hidden" id="gpxonly" name="gpxonly" value=""/>			
	</fieldset>	
    </form>
</div>
</div><!-- End demo -->



<div id="dialog-modal" title="Routes" style="display: none;">
  <div align="center"><a href="Main.aspx">Build new route</a></div>
  <div id="dynamic">
  <table cellpadding="0" cellspacing="0" border="0" class="display" id="routesList">
      <thead>
		<tr>
			<th width="30%">Route Name</th>			
			<th width="20%">Created By</th>
            <th width="20%">Created On</th>
            <th width="30%">Modify</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="4" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>
	<tfoot>
		<tr>
			<th width="30%">Route Name</th>			
			<th width="20%">Created By</th>
            <th width="20%">Created On</th>
            <th width="30%">Modify</th>
		</tr>
	</tfoot>

  </table>
  </div>
</div>
    
    
    <div id="dialog-disputes" title="Dispute points" style="display: none;">
  <div align="center"><a href="Main.aspx">Dispute speed points</a></div>

  <table cellpadding="0" cellspacing="0" border="0" class="display" id="tblDisputePoints">
      <thead>
		<tr>
			<th>Ticket Id</th>			
			<th>Address</th>
            <th width="10%">Reported By</th>
            <th>Your speed</th>
            <th>Posted speed</th>
            <th>Claimed speed</th>
            <th>Notes</th>
            <th>Created</th>
            <th>Modified</th>
            <th>Status</th>
            <th width="10%">Operation</th>            
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="4" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>	

  </table>
  </div>
    
    
     <div id="dialog-routeSegments" title="Overwritten speed segments" style="display: none;">
  <div align="center"><a href="Main.aspx">Dispute speed segments</a></div>
         <div align="center"><input type="checkbox" id="chkDropSegments" name="chkDropSegments" />Drop all segments on map</div>
  <table cellpadding="0" cellspacing="0" border="0" class="display" id="tblRouteSegments">
      <thead>
		<tr>
			<th>Segment name</th>			
			<th>Created By</th>
            <th>Created At</th>
            <th>Expired At</th>
            <th>Speed(km/h in Canada, mph in USA)</th>            
            <th>Description</th>            
            <th>Contained points</th>
            <th width="10%">Operation</th>            
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="4" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>	

  </table>
  </div>

    
    <div id="preference-dialog" title="Route Preferences" style="display: none;">
    <div align="center">Add preferences</div>
        <table>
            <tr>
                <td>
                    <fieldset>
        <legend>Road options:</legend>
        <input type="checkbox" id="ckTollRoad" name="RouteFeatureType" value="tollroad" >Avoid Toll Road</input><br />
        <input type="checkbox" id="ckMotoway" name="RouteFeatureType" value="motorway" >Avoid Motoway</input>
        </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    <fieldset>
        <legend>Route options:</legend>
            <input type="radio" id="rdoFastest" name="routeOption" value="fastest" checked="checked" >Fastest</input><br />
            <input type="radio" id="rdoShortest" name="routeOption" value="shortest" >Shortest</input>          
        </fieldset>     
                </td>
            </tr>
        </table>
        
        
                   
    </div>  
    
    <div id="email-dialog" title="Email route" style="display: none;">
        <table>
            <tr>
                <td>Your Name:</td>
                <td><input type="text" id="emailname" name="emailname" /></td>
            </tr>
            <tr>
                <td> Email Address:</td>
                <td><input type="text" id="emailaddress" name="emailaddress" maxlength="200" size="35" /></td>
            </tr>
            <tr>
                <td>Message:</td>
                <td><textarea id="emailmessage" name="emailmessage" rows="3" cols="55"></textarea></td>
            </tr>
        </table>                                                
    </div>  
    
    <form id="gpxform" action="GenerateGpx.aspx?gpxonly=1" method="POST">
        <input type="hidden" value="" id="NavTeqResponseData" name="NavTeqResponseData"/>
    </form>
</body>
</html>
