<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleMapVE.aspx.cs" Inherits="SentinelFM.Map_frmVehicleMapVE" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    
    
   
    <link href="../vemapstyle.css" type="text/css" rel="stylesheet" />
    <script charset="UTF-8" type="text/javascript" src="http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2&mkt=en-us">    </script>
    <script id="vexscript" charset="UTF-8" type="text/javascript" src="../Scripts/vex.js" language="javascript"></script>
</head>
<body onload="GetMap('Map');frameSize();"  onresize="frameSize();">
    <form id="MapForm"    >
        <input id="browserType" name="browserType"  type="hidden"  value="0" />
        <input id="mapheight" name="mapheight" type="hidden" value="325px"  />
        <div id='vex' class="mapdiv"  style="width:600px;height:400px;"></div>
    </form>    


  <script type="text/javascript"   language="JavaScript">
    <!--
        
        var avls ='<%=strVehicleData%>';
        var geozones = '<%=strGeoZoneData %>';
        var geozonesactive = '<%=sn.Map.ShowGeoZone %>';
        var landmarks = '<%=strLandmarkData %>';
        var landmarksactive = '<%=sn.Map.ShowLandmark %>';
        var org ='<%=sn.User.OrganizationId%>';
        var uid ='<%=sn.UserID%>';
        var sid ='<%=sn.SecId%>';
        var unitOfMes='<%=strUnitOfMes %>';
        var logo='bsm_logo.gif';
        var token='<%=token%>';
        
        function frameSize()
        {

		FrameResize(); //(-300,0);
		 //top.frames[0].window,0,0);
        }



    //-->
 </script>

</body>
</html>
