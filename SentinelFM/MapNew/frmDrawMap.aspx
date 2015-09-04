<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmDrawMap.aspx.cs" Inherits="SentinelFM.MapNew_frmDrawMap" meta:resourcekey="PageResource1"  UICulture="auto"  %>

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
    <script type="text/javascript" >
        function SaveClick() {
            if (mapLatLonLoc != null) {
                window.opener.document.forms[0].txtX.value = mapLatLonLoc.lon;
                window.opener.document.forms[0].txtY.value = mapLatLonLoc.lat;
            }
            window.close();
            return false;
        }
    </script>
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" 
        EnablePageMethods ="True">
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" 
        meta:resourcekey="LoadingPanel1Resource1" >
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"  
       >
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID ="cboLandmarks" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID ="cmdShowEditGeoZone" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID ="cmdSave" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div  style="vertical-align:top">
          <table border="0" cellpadding="0" cellspacing="0" style="width:100%; height:100%">
                <tr valign="top">
                    <td id="frmVehicleMapfrmVehicleMap"  style="width:800px; height:500px"  >
                        <uc1:Map ID="Map1" runat="server"  />
                    </td>

                </tr>

  <tr>
                <td align="left">
                    <asp:Panel ID="pnlAll" runat="server" meta:resourcekey="pnlAllResource1"  >
                    <table width="700px" >
                        <tr>
                            <td>
                                <table id="tblGeoZone" style="
                                     height: 14px" cellspacing="0" cellpadding="0" width="311" border="0"
                                    runat="server">
                                    <tr id="Tr2" runat="server">
                                        <td id="Td2" class="formtext" style="width: 308px" runat="server">
                                            <table id="Table4" cellspacing="1" cellpadding="1" width="300" border="0">
                                                <tr>
                                                    <td>
                                                        <table id="Table7" style="width: 198px; height: 60px" cellspacing="1" cellpadding="1"
                                                            width="198" border="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdDrawGeoZone" runat="server" CssClass="combutton" Text="Start Drawing"
                                                                        Width="189px" Height="22px"  OnClientClick ="return StartDrawGeoZone(this);" meta:resourcekey="cmdDrawGeoZoneResource1">
                                                                    </asp:Button>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdClearMap" runat="server" CssClass="combutton" Text="Clear Map"  OnClientClick ="return CleanDrawGeoZone();"
                                                                        Width="189px" meta:resourcekey="cmdClearMapResource1">
                                                                    </asp:Button>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <asp:RadioButtonList ID="LandmarkOptions" runat="server" CssClass="formtext" 
                                                            Width="150px" Height="48px" BorderWidth="1px" onclick="return SelectDrawType();"
                                                            meta:resourcekey="GeoZoneOptionsResource1" RepeatDirection="Horizontal" >
                                                            <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource3" Text="Circle"></asp:ListItem>
                                                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource1" 
                                                                Text="Rectangle"></asp:ListItem>
                                                            <asp:ListItem Value="2" meta:resourcekey="ListItemResource2" Text="Polygon"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="Tr3" runat="server">
                                        <td id="Td3" class="formtext" style="width: 308px" runat="server" >
                                            <asp:Label ID="lblAddGeoZoneMsg" runat="server" 
                                                meta:resourcekey="lblAddGeoZoneMsgResource1" 
                                                Text="Navigate map to landmark position."></asp:Label>
                                        </td>
                                    </tr>
                                </table>
        <asp:Button ID="cmdShowEditGeoZone" Style="" runat="server" CssClass="combutton" Text="Edit Landmark" 
            meta:resourcekey="cmdShowEditLandmarkResource1" onclick="cmdShowEditGeoZone_Click"></asp:Button>

                            </td>
                            <td align="right" valign="top">
                              <asp:Button ID="cmdCancel" runat="server" CssClass="combutton" Text="Close" OnClick="cmdCancel_Click" 
            meta:resourcekey="cmdCancelResource1"></asp:Button>
        <asp:Button ID="cmdSave"  ValidationGroup="vgcmdSave"
            runat="server" CssClass="combutton" Text="Save" 
                                    meta:resourcekey="cmdSaveResource1" onclick="cmdSave_Click">
        </asp:Button>
                                <asp:CustomValidator ID="cvcmdSave" runat="server" 
                                    ClientValidationFunction="CustomValidateDate" ValidationGroup="vgcmdSave" 
                                    Display="None" meta:resourcekey="cvcmdSaveResource1" />


                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                    <asp:Label ID="lblMessage" Style="" runat="server" CssClass="errortext"  Font-Bold="True"
            meta:resourcekey="lblMessageResource1"></asp:Label>

                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                </td>
            </tr>
            </table>

    </div>
    <asp:HiddenField id="hidStartDraw" runat="server" />
    <asp:HiddenField id="HidDrawType" runat="server" />
    <asp:HiddenField id="hidPoints" runat="server" />
    <asp:HiddenField id="hidCenter" runat="server" />

     <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >

        
        function StartDrawGeoZone(ctl) {
            $("#<%= hidStartDraw.ClientID %>").val("1");
            if (ctl != null) $(ctl).attr("disabled", "true");
            var maximunPoints = <%= MaximunPoints%>;
            var drawType = $('#LandmarkOptions').find("input:checked").val();
            if (drawType == "1") {
               myMap.editGeozone(Sentinel.Constants.GEOZONE_FORM_FACTOR.RECTANGLE, maximunPoints);
               $("#<%= lblMessage.ClientID %>").text('<%= drawRectangleText %>');
            }
            if (drawType == "2") {
              myMap.editGeozone(Sentinel.Constants.GEOZONE_FORM_FACTOR.POLYGON, maximunPoints);
              $("#<%= lblMessage.ClientID %>").text('<%= drawPolygon %>');
            }

            if (drawType == "0") {
              myMap.editGeozone(Sentinel.Constants.GEOZONE_FORM_FACTOR.CIRCLE, maximunPoints);
              $("#<%= lblMessage.ClientID %>").text('<%= drawCircleText %>');
            }


            $("#<%= HidDrawType.ClientID %>").val(drawType);
            return false;
        }

        function SelectDrawType()
        {
            if  ($("#<%= hidStartDraw.ClientID %>").val() == '') return;
            var drawType = $('#LandmarkOptions').find("input:checked").val();
            if ($("#<%= HidDrawType.ClientID %>").val() != drawType)
            {
               CleanDrawGeoZone();
            }
           return true;
        }
        function CleanDrawGeoZone()
        {
            myMap.geozoneEraseClicked();
            StartDrawGeoZone(null);
            return false;
        }
        function CustomValidateDate(sender, args)  {
            var coordinates = null;
            var drawType = $('#LandmarkOptions').find("input:checked").val();
            coordinates = myMap.drawingContext.points;
            var ret = false;
            if (drawType == 1)
            {
               if (myMap.drawingContext.points != null && coordinates.length <= 1)
               {
                  alert('<%= MinimumPointsRectangleError %>');
               }
               else
               {
                   ret = true;
               }
               var mapPoints = '';

               mapPoints = coordinates[0].lat.toString() + "|" + coordinates[0].lon.toString() + "," +
                           coordinates[0].lat.toString() + "|" + coordinates[1].lon.toString() + "," +
                           coordinates[1].lat.toString() + "|" + coordinates[1].lon.toString() + "," +
                           coordinates[1].lat.toString() + "|" + coordinates[0].lon.toString() ;
               $('#<%= hidPoints.ClientID %>').val(mapPoints);
            }
            if (drawType == 2)
            {
               if (myMap.drawingContext.points != null && coordinates.length <= 2)
               {
                  alert('<%= MinimumPointsPolygonError %>');
               }
               else
               {
                  ret = true;
                  var mapPoints = '';
                  for (var k in coordinates) {
                      if (mapPoints != '' ) mapPoints =mapPoints + ",";
                      mapPoints = mapPoints + coordinates[k].lat.toString() + "|" + coordinates[k].lon.toString();
                  }
                  $('#<%= hidPoints.ClientID %>').val(mapPoints);
               }
            }

            if (drawType == 0)
            {
                try {
                        var mapPoints = '';
                        if (myMap.drawingLayer != null) 
                        { 
                            var centerPoint = myMap.drawingLayer.getCenter();
                            mapPoints = centerPoint.lat.toString() + "|" + centerPoint.lon.toString();
                            var radius = myMap.drawingLayer.getDistance();
                            var unit = myMap.drawingLayer.getUnits() ;
                            radius = Telogis.GeoBase.MathUtil.convertUnits (radius, unit, Telogis.GeoBase.DistanceUnit.METERS);
                            mapPoints = mapPoints +"|" + radius.toString();
                        }
                    }
                catch (err) { mapPoints = '';}
                $('#<%= hidPoints.ClientID %>').val(mapPoints);
                if (mapPoints != '')
                {
                     ret = true;
                }
                else alert('<%= errorCircle %>');
            }
            args.IsValid = ret;
        }
        </script>
        </telerik:RadCodeBlock>
    </form>
</body>
</html>

