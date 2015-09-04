<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VE.aspx.cs" Inherits="SentinelFM.VE" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VEX TEST</title>

    <script charset="UTF-8" type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2&mkt=en-us">    </script>

    <script src="scripts/prototype.js?ver=<%=jsversion%>" type="text/javascript"></script>

    <script id="vexscript" type="text/javascript" src="scripts/vex.js?ver=<%=jsversion%>"></script>

    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/yahoo/yahoo-min.js"></script>

    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/event/event-min.js"></script>

    <link type="text/css" rel="Stylesheet" href="ve.css" />

    <script type="text/javascript" language="JavaScript" src="scripts/interframe.js?ver=<%=jsversion%>"></script>
    <script type="text/javascript" language="JavaScript" src="scripts/p_ajax.js?ver=<%=jsversion%>"></script>

    <script type="text/javascript" language="JavaScript">

        var mapscreen = null;

        function frameResize() {
            if (mapscreen == null)
                return;
            mapscreen.frameResize();
        }

	if (BsmVE_Version) BsmVE_Version = '<%=jsversion%>';

    </script>

</head>
<body onresize="frameResize();">
    <form id="VEForm" name="VEForm" runat="server">
    <asp:Literal ID="VEScriptLiteral" runat="server" />
    <input type="hidden" id="Points" name="Points" value="" />
    <input type="hidden" id="MapType" name="MapType" value="" />
    <div id='vex' class="mapdiv" style="width: 600px; height: 400px;">
    </div>
    </form>
</body>
</html>
