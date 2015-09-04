<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMap_Landmark_GeoZone.aspx.cs" Inherits="SentinelFM.MapNew_frmMap_Landmark_GeoZone" meta:resourcekey="PageResource1" %>
<%@ Register src="UserControl/Map.ascx" tagname="Map" tagprefix="uc1" %>
<%@ Register src="../GeoZone_Landmarks/Components/ctlGeozoneLandmarksMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/css/Map/sentinel.telogis.css?ver=3.0" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.4.1.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.geobase.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.dictionary_en.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.skins.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.map.js?ver=3.0" language="javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div>

        <table id="tblCommands" style=" left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc2:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdMap"  />
                </td>
            </tr>
            <tr>
                <td style="height: 588px" valign="top"  >

                    <table id="tblBody" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" height="550px" width="990px" class=table  border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table4" style="" cellspacing="0" cellpadding="0" width="261" border="0">
                                                <tr>
                                                    <td id="frmVehicleMapfrmVehicleMap" width="655" height="400px">
                                                        <uc1:Map ID="Map1" runat="server" />
                                                   </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>


                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
