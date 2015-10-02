<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mao.aspx.cs" Inherits="MapNew_mao" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/sentinel.telogis.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.4.1.js" language="javascript"></script>

    <script type="text/javascript" src="Scripts/telogis.geobase.js" language="javascript"></script>
    <script type="text/javascript" src="Scripts/telogis.util.js" language="javascript"></script>

    <script type="text/javascript" src="Scripts/sentinel.util.js" language="javascript"></script>
    <script type="text/javascript" src="Scripts/sentinel.dictionary_en.js" language="javascript"></script>
    <script type="text/javascript" src="Scripts/sentinel.telogis.skins.js" language="javascript"></script>
    <script type="text/javascript" src="Scripts/sentinel.telogis.map.js" language="javascript"></script>

</head>
<body >
    <form id="form1" runat="server">
 <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <div >
    <script type="text/javascript" language="javascript">

        var eventDispatcher = new EventDispatcher();
        var myMap;

        var service = eval("<% GetService (); %>");
        var authTokenTuple = eval("[<% GetToken (); %>]");

        $(document).ready(function () {

            // Set the internal GeoBase authentication token
            Telogis.GeoBase.setService(service);
            Telogis.GeoBase.setAuthToken(authTokenTuple[0], authTokenTuple[1]);

            var options = {
                initialPosition: [34.0, -118.0]
            };

            myMap = new Sentinel.Map($('#mapContainer')[0], eventDispatcher, options);

        });
     </script>

    <div id="mapContainer" style="border: 1px solid black; height:300px; width:100px;" >
    </div>

    <div id="topControlArea"></div>

    <!-- TODO: translation for tooltips -->
    <div id="panControlPanel">
        <map id="panImageMap" name="panImageMap">
            <area alt="Pan Up"    shape="poly" coords="0,0, 35,35, 70,0" nohref="true" onclick="myMap.panUp();" title="Pan Up" />
            <area alt="Pan Left"  shape="poly" coords="0,0, 35,35, 0,70" nohref="true" onclick="myMap.panLeft();" title="Pan Left" />
            <area alt="Pan Right" shape="poly" coords="70,0, 35,35, 70,70" nohref="true" onclick="myMap.panRight();" title="Pan Right" />
            <area alt="Pan Down"  shape="poly" coords="0,70, 35,35, 70,70" nohref="true" onclick="myMap.panDown();" title="Pan Down" />
        </map>
        <img src="images/pan.png" ismap="true" usemap="#panImageMap" class="panImage" />
    </div>

    <div id="topMenu" style="display:none" >
    </div>
    </div>
    </form>
</body>
</html>
