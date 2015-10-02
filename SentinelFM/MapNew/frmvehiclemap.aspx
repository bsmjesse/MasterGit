﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmvehiclemap.aspx.cs" Inherits="SentinelFM.Map_frmvehiclemap" %>

<%@ Register src="UserControl/Map.ascx" tagname="Map" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/css/Map/sentinel.telogis.css?ver=3.1" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.4.1.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.geobase.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.dictionary_en.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.skins.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.map.js?ver=3.2" language="javascript"></script>


</head>
<body>
    <form id="frmVehicleMapForm" runat="server">

            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; height:100%">
                <tr>
                    <td id="frmVehicleMapfrmVehicleMap" style="height: 100%; width: 100%">
                        <uc1:Map ID="Map1" runat="server" />
                    </td>
                </tr>
            </table>

    </form>
    <script type="text/javascript" >

        function AutoReloadMap() {
            parent.frmVehicleMap.location.href = "frmVehicleMap.aspx";
        }

    </script>
</body>
</html>
