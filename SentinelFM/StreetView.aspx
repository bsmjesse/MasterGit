<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StreetView.aspx.cs" Inherits="SentinelFM.StreetView" %>

<html>
<head>
    <title>Sentinel Google street view</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>

          <script type="text/javascript">
              var orgid = '<%=sn.User.OrganizationId %>';
              var interval = '<%=sn.User.GeneralRefreshFrequency %>';
      </script>

    <link rel="stylesheet" type="text/css" href="./sencha/ext-3.4.0/resources/css/ext-all.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/ext-3.4.0/examples/shared/examples.css" />

    <script type="text/javascript" src="./sencha/ext-3.4.0/adapter/ext/ext-base.js"></script>
    <script type="text/javascript" src="./sencha/ext-3.4.0/ext-all.js"></script>
  <script src="https://maps.googleapis.com/maps/api/js?sensor=false" type="text/javascript"></script>

   <script type="text/javascript" src="./Scripts/utils/ExtGmapPanel/src/Ext.ux.GMapPanel3.js"></script>
     <script type="text/javascript" src="./Scripts/OpenLayerMap/StreetView.js"></script>

</head>
<body> 
  <div id="map_canvas" style="width: 400px; height: 300px"></div>
  <div id="pano" style="position:absolute; left:410px; top: 8px; width: 400px; height: 300px;"></div>
</body>
</html>
