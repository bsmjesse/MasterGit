<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmviewgeozone.aspx.cs" Inherits="SentinelFM.MapNew_frmviewgeozone" %>
<%@ Register src="UserControl/Map.ascx" tagname="Map" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Landmark</title>
    <link href="../Scripts/css/Map/sentinel.telogis.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.4.1.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.geobase.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.dictionary_en.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.skins.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.map.js?ver=1.1" language="javascript"></script>

</head>
<body topmargin="0px" leftmargin="0px" >
    <form id="form1" runat="server">
    <div  style="vertical-align:top">
    <div  style="vertical-align:top">
          <table border="0" cellpadding="0" cellspacing="0" style="width:100%; height:100%">
                <tr valign="top">
                    <td id="frmVehicleMapfrmVehicleMap"    >
                        <uc1:Map ID="Map1" runat="server" />
                    </td>
                </tr>
                <tr align="center">
                    <td>
                    <br />
<asp:Button ID="cmdCLoseWindow"  class="combutton" 
            OnClientClick="window.close()"  runat="server" Text="Close" 
            meta:resourcekey="cmdCLoseWindowResource1" />
                    </td>
                </tr>
            </table>

    </div>
    </div>
    </form>
</body>
</html>