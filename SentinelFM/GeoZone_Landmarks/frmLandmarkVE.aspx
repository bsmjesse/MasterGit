<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmLandmarkVE.aspx.cs" Inherits="SentinelFM.GeoZone_Landmarks_frmLandmarkVE" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>GeoZone</title>
    <link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />
    <link href="../vemapstyle.css" type="text/css" rel="stylesheet" />
    <script charset="UTF-8" type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2&mkt=en-us">    </script>
    <script id="vexscript" charset="UTF-8" type="text/javascript" src="../Scripts/vex.js" language="javascript"></script>
    <script id="vexdrawscript" charset="UTF-8" type="text/javascript" src="../Scripts/vexdraw.js" language="javascript"></script>
</head>
<body onload="GetMap('Landmark')" >
    

  <script type="text/javascript"   language="JavaScript">
    <!--
        
        var avls =null;
        var geozones = null;
        var geozonesactive = false;
        var landmarks = null;
        var landmarksactive = false;
        var org ='<%=sn.User.OrganizationId%>';
        var uid ='<%=sn.UserID%>';
        var sid ='<%=sn.SecId%>';
        var unitOfMes='<%=strUnitOfMes %>';
        var logo='bsm_logo.gif';
        var token='0925690829068902360921';
            
    //-->
 </script>

   <form id="MapForm" name="MapForm" method="post" runat="server">
   
         
        <input id="browserType" name="browserType"  type="hidden"  value="0" />
        <input id="mapheight" name="mapheight" type="hidden" value="400x"  />
        <input id="Points" name="Points"  type="hidden"  value="0" />
        <div id='vex' class="mapdiv"  style="width:800px;height:500px;"></div>
        
    </form>    


</body>
</html>
