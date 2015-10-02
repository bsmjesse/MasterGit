<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmGeozoneLandmarkMapFrame.aspx.cs" Inherits="SentinelFM.GeoZone_Landmarks_frmGeozoneLandmarkMapFrame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<META HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head runat="server">
    <title>Untitled Page</title>
       <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
       <link href="../vemapstyle.css" type="text/css" rel="stylesheet" />
       <script charset="UTF-8" type="text/javascript" src="http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2&mkt=en-us">    </script>
       <script id="vexscript" charset="UTF-8" type="text/javascript" src="../Scripts/vex.js" language="javascript"></script>
       <script id="vexdrawscript" charset="UTF-8" type="text/javascript" src="../Scripts/vexdraw.js" language="javascript"></script>
    
    
    
    
 
</head>
<body  onload="GetMap('Landmark_GeoZone');frameSize()">
    <form id="MapForm" runat="server">
    <div>
            <input id="browserType" name="browserType"  type="hidden"  value="0" />
                        <input id="mapheight" name="mapheight" type="hidden" value="490x"  />
                        <input id="Points" name="Points"  type="hidden"  value="0" />
                        <div id='vex' class="mapdiv"  style="width:800px;height:490px;"></div>
        
    </div>
    </form>
    
    
    <script type="text/javascript"   language="JavaScript">
    <!--
        
        var avls =null;
        var geozones =  '<%=strGeoZoneData %>';
        var geozonesactive = true;
        var landmarks =  '<%=strLandmarkData %>';
        var landmarksactive = true;
        var org ='<%=sn.User.OrganizationId%>';
        var uid ='<%=sn.UserID%>';
        var sid ='<%=sn.SecId%>';
        var unitOfMes='<%=strUnitOfMes %>';
        var logo='bsm_logo.gif';
        var token='<%=token%>';
        var edit=false;
        
        
        function frameSize()
        {
			FrameResize();
        }

            
    //-->
 </script>
</body>
</html>
