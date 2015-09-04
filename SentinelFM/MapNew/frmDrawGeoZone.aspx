<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmDrawGeoZone.aspx.cs" Inherits="SentinelFM.MapNew_frmDrawGeoZone" meta:resourcekey="PageResource1"  UICulture="auto"   %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="UserControl/Map.ascx" TagName="Map" TagPrefix="uc1" %>
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
    <script type="text/javascript" src="../Scripts/sentinel.telogis.map.js?ver=1.4" language="javascript"></script>
</head>
<body topmargin="0px" leftmargin="0px">
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" EnablePageMethods ="true">
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" >
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1"  
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

    <div style="vertical-align: top">
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
            <tr valign="top" >
                <td id="frmVehicleMapfrmVehicleMap" style="width: 800px; height: 450px">
                    <uc1:Map ID="Map1" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:Panel ID="pnlAll" runat="server"  >
                    <table width="700px" >
                        <tr>
                            <td>
                                <table id="tblGeoZone" style="
                                     height: 14px" cellspacing="0" cellpadding="0" width="311" border="0"
                                    runat="server">
                                    <tr runat="server">
                                        <td class="formtext" style="width: 308px; height: 5px" height="5" 
                                            runat="server">
                                                <table id="Table6" style="width: 301px; height: 47px" cellspacing="1" cellpadding="1"
                                                    width="301" border="0">
                                                    <tr>
                                                        <td class="formtext" style="height: 11px">
                                                            <asp:Label ID="lblMapToTitle" runat="server" CssClass="formtext" 
                                                                meta:resourcekey="lblMapToTitleResource1" Text="Map To:"></asp:Label>&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="cboLandmarks" runat="server" CssClass="formtext" DataTextField="LandmarkName"
                                                                DataValueField="LandmarkName" AutoPostBack="True" Width="294px" OnSelectedIndexChanged="cboLandmarks_SelectedIndexChanged"
                                                                meta:resourcekey="cboLandmarksResource1">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                        </td>
                                    </tr>
                                    <tr runat="server">
                                        <td class="formtext" style="width: 308px" runat="server">
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
                                                        <asp:RadioButtonList ID="GeoZoneOptions" runat="server" CssClass="formtext" 
                                                            Width="150px" Height="48px" BorderWidth="1px" onclick="return SelectDrawType();"
                                                            meta:resourcekey="GeoZoneOptionsResource1">
                                                            <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource1" 
                                                                Text="Rectangle"></asp:ListItem>
                                                            <asp:ListItem Value="2" meta:resourcekey="ListItemResource2" Text="Polygon"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr runat="server">
                                        <td class="formtext" style="width: 308px" runat="server" >
                                            <asp:Label ID="lblAddGeoZoneMsg" runat="server" Visible="False" 
                                                meta:resourcekey="lblAddGeoZoneMsgResource1" 
                                                Text="Navigate map to geozone poition."></asp:Label>
                                        </td>
                                    </tr>
                                </table>
        <asp:Button ID="cmdShowEditGeoZone" Style="" runat="server" CssClass="combutton" Text="Edit GeoZone" OnClick="cmdShowEditGeoZone_Click"
            meta:resourcekey="cmdShowEditGeoZoneResource1"></asp:Button>

                            </td>
                            <td align="right" valign="top">
                                <asp:Button ID="cmdCancel" runat="server" CssClass="combutton" Text="Close" OnClientClick="window.close();"
                                    meta:resourcekey="cmdCancelResource1"></asp:Button>
                                <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save"  ValidationGroup="vgcmdSave"
                                    meta:resourcekey="cmdSaveResource1" onclick="cmdSave_Click"></asp:Button>
                                <asp:CustomValidator ID="cvcmdSave" runat="server" ClientValidationFunction="CustomValidateDate"
                                                  EnableClientScript="true" ValidationGroup="vgcmdSave" Display="None" ErrorMessage="" />

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
     <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >

        
        function StartDrawGeoZone(ctl) {
            $("#<%= hidStartDraw.ClientID %>").val("1");
            if (ctl != null) $(ctl).attr("disabled", "true");
            var maximunPoints = <%= MaximunPoints%>;
            var drawType = $('#GeoZoneOptions').find("input:checked").val();
            if (drawType == "1") {
               myMap.editGeozone(Sentinel.Constants.GEOZONE_FORM_FACTOR.RECTANGLE, maximunPoints);
               $("#<%= lblMessage.ClientID %>").text('<%= drawRectangleText %>');
            }
            else{
              myMap.editGeozone(Sentinel.Constants.GEOZONE_FORM_FACTOR.POLYGON, maximunPoints);
              $("#<%= lblMessage.ClientID %>").text('<%= drawPolygon %>');
            }

            $("#<%= HidDrawType.ClientID %>").val(drawType);
            return false;
        }

        function SelectDrawType()
        {
            if  ($("#<%= hidStartDraw.ClientID %>").val() == '') return;
            var drawType = $('#GeoZoneOptions').find("input:checked").val();
            if ($("#<%= HidDrawType.ClientID %>").val() != drawType)
            {
               CleanDrawGeoZone();
               StartDrawGeoZone(null);
            }
           return true;
        }
        function CleanDrawGeoZone()
        {
            myMap.geozoneEraseClicked();
            return false;
        }

        function CustomValidateDate(sender, args)  {
            var coordinates = null;
            var drawType = $('#GeoZoneOptions').find("input:checked").val();
            coordinates = myMap.drawingContext.points;
            var ret = false;
            if (drawType == 1)
            {
               if (coordinates.length <= 1)
               {
                  alert('<%= MinimumPointsRectangleError %>');
               }
               else
               {
                   ret = true;
               }
            }
            if (drawType == 2)
            {
               if (coordinates.length <= 2)
               {
                  alert('<%= MinimumPointsPolygonError %>');
               }
               else
               {
                  ret = true;
               }
            }
            if (ret) 
            {
                var mapPoints = '';
                for (var k in coordinates) {
                      if (mapPoints != '' ) mapPoints =mapPoints + ",";
                      mapPoints = mapPoints + coordinates[k].lat.toString() + "|" + coordinates[k].lon.toString();
                }
                $('#<%= hidPoints.ClientID %>').val(mapPoints);
            }
            args.IsValid = ret;


        }


    </script>
    </telerik:RadCodeBlock>
    </form>
    
</body>
</html>
