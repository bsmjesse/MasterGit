<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmLandmarkGeoZoneVE.aspx.cs" Inherits="SentinelFM.frmLandmarkGeoZoneVE" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%=sn.Landmark.RefreshFormName%></title>
     <META HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    <link href="../vemapstyle.css" type="text/css" rel="stylesheet" />
    <script charset="UTF-8" type="text/javascript" src="http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2&mkt=en-us">    </script>
    <script id="vexscript" charset="UTF-8" type="text/javascript" src="../Scripts/vex.js" language="javascript"></script>
    <script id="vexdrawscript" charset="UTF-8" type="text/javascript" src="../Scripts/vexdraw.js" language="javascript"></script>
</head>
<body onload="GetMap('<%=sn.Landmark.RefreshFormName%>')" >
    

  <script type="text/javascript"   language="JavaScript">
    <!--
        
        var avls =null;
        var geozones =  '<%=strGeoZoneData %>';
        var geozonesactive = <%=flagShowGeoZones%>;
        var landmarks =  '<%=strLandmarkData %>';
        var landmarksactive = <%=flagShowLandmarks%>;
        var org ='<%=sn.User.OrganizationId%>';
        var uid ='<%=sn.UserID%>';
        var sid ='<%=sn.SecId%>';
        var unitOfMes='<%=strUnitOfMes %>';
        var logo='bsm_logo.gif';
        var token='<%=token%>';
        var edit=<%=flagEditData %>;
            
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
