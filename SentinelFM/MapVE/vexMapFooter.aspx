<%@ Page Language="C#" AutoEventWireup="true" CodeFile="vexMapFooter.aspx.cs" Inherits="SentinelFM.MapVE_vexMapFooter" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VE Footer</title>
    <link type="text/css" rel="Stylesheet" href="ve.css" />

    <script src="scripts/util.js" type="text/javascript"></script>

    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/yahoo/yahoo-min.js"></script>

    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/event/event-min.js"></script>

    <script language="javascript" type="text/javascript">

        var _mode = "wait";
        var _wait = "images/egg-timer-icon-rw.gif";
        var _norm = "images/trans.gif";

        function initializeFooter() {
            document.getElementById("traffickey").style.display = "none";
        }

        function prepFooter(frame) {
            document.getElementById("ShowFleet").className = ("buttonselected_" + frame.mapscreen.fleetEnabled);
            frame.mapscreen.onFleetVisibilityChanged.subscribe(fleetVisibilityChangedHandler);
            frame.mapscreen.onTrafficVisibilityChanged.subscribe(trafficVisibilityChangedHandler);
            frame.mapscreen.onInsetMapVisibilityChanged.subscribe(insetMapVisibilityChangedHandler);
            frame.mapscreen.onLandmarksAvailableChange.subscribe(landmarksAvailableChangeHandler);
            frame.mapscreen.onGeozonesAvailableChange.subscribe(geozonesAvailableChangeHandler);

            _mode = "norm";
            getImage();
            frame.mapscreen.setUserInterface();
            trafficVisibilityChangedHandler();
            insetMapVisibilityChangedHandler();

            if (frame.mapscreen.maptype == "Map")
                document.getElementById("ShowTraffic").style.display = "inline-block";
                
        }

        function getImage() {
            if (_mode == "wait")
                var value = _wait;
            else
                var value = _norm;
            var waitcursor = document.getElementById("waitcursor")
            if (waitcursor != null)
                waitcursor.src = value;
        }

        function showFleet() {
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.toggleFleetVisibility();
        }

        function fleetVisibilityChangedHandler() {
            var fb = getFrameElement("framebody", parent);
            var value = fb.mapscreen.fleetEnabled;
            document.getElementById("ShowFleet").className = ("buttonselected_" + value);
        }



        function getLandmarks() {
            _mode = "wait";
            getImage();
            var fb = getFrameElement("framebody", parent);
            fb.getLandmarksProxy();
            document.getElementById("GetLandmarks").className = ("buttonselected_true");

        }


        function landmarksAvailableChangeHandler() {
            var fb = getFrameElement("framebody", parent);
            var enabled = fb.mapscreen.landmarkInBounds;
            var value = "disabled";
            if (enabled)
                value = "";
            document.getElementById("GetLandmarks").disabled = value;
        }

        function getGeoZones() {
            _mode = "wait";
            getImage();
            var fb = getFrameElement("framebody", parent);
            fb.getGeozonesProxy();
            document.getElementById("GetGeozones").className = ("buttonselected_true");
        }

        function geozonesAvailableChangeHandler() {
            var fb = getFrameElement("framebody", parent);
            var enabled = fb.mapscreen.geozonesInBounds;
            var value = "disabled";
            if (enabled)
                value = "";
            document.getElementById("GetGeozones").disabled = value;
        }

        function showTraffic() {
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.toggleTrafficVisibility();
        }


        function trafficVisibilityChangedHandler() {
            try {
                var fb = getFrameElement("framebody", parent);
                var value = fb.mapscreen.trafficEnabled;
                document.getElementById("ShowTraffic").className = ("buttonselected_" + value);
                showTrafficKey();
            }
            catch (e) { }
        }

        function showInsetMap() {
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.toggleInsetMapVisibility();
        }

        function insetMapVisibilityChangedHandler() {
            var fb = getFrameElement("framebody", parent);
            var value = fb.mapscreen.insetMapEnabled;
            document.getElementById("ShowMiniMap").className = ("buttonselected_" + value);
        }


        function find() {
            _mode = "wait";
            var value = document.getElementById("SearchValue");
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.conductFind(value.value);
        }

        function cancelWait() {
            _mode = "norm";
            getImage();
        }


        function showTrafficKey() {
            var fb = getFrameElement("framebody", parent);
            if (fb.mapscreen.trafficEnabled)
                document.getElementById("traffickey").style.display = "inline-block";
            else
                document.getElementById("traffickey").style.display = "none";
        }
        

    </script>

</head>
<body class="footerdiv" onload="initializeFooter();">
    <table cellpadding="2" cellspacing="0" width="100%">
        <tr>
            <td id="footercellleft" style="">
                <nobr>
                    <span style="display: none;">
                        <img alt="trans" id="trans1" src="images/trans.gif" width="10px" />
                        <img alt="wait cursor" id="waitcursor" src="images/egg-timer-icon-rw.gif" height="15px"
                            width="9px" />
                    </span>
                    <img alt="trans" id="trans2" src="images/trans.gif" width="10px" />
                    <input id="ShowFleet" type="button" class="buttonselected_true" value="Fleet" onclick="showFleet();" />
                    <input id="GetLandmarks" disabled="disabled" type="button" class="buttonselected_false"
                        value="Landmarks" onclick="getLandmarks();" />
                    <input id="GetGeoZones" disabled="disabled" type="button" class="buttonselected_false"
                        value="GeoZones" onclick="getGeoZones();" />
                    <input id="ShowTraffic" style="display: none;" class="buttonselected_false" type="button"
                        value="Traffic" onclick="showTraffic();" />
                    <span id="traffickey" style="font-size:8pt; vertical-align:middle;display: none;">
                        slow <img alt="traffic key" src="images/traffickey.png" /> fast</span>
                    <input id="ShowMiniMap" type="button" class="buttonselected_false" value="Inset Map"
                        onclick="showInsetMap();" />
                    <input id="Help" type="button" class="button" value="Help" onclick="window.open('help.htm','','status=0,location=0,menubar=0,width=400,height=400,scrollbars=1,resizable=1');" />
                    <input id="FindIt" type="button" class="button" value="Find" onclick="find();" />
                    <input id="SearchValue" type="text" value="" />
                </nobr>
            </td>
        </tr>
    </table>
</body>
</html>
