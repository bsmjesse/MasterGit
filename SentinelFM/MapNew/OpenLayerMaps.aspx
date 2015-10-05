<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OpenLayerMaps.aspx.cs" Inherits="SentinelFM.OpenLayerMaps" meta:resourcekey="PageResource1" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <title>Track vehicles on map</title>

    <link rel="stylesheet" href="../Openlayers/OpenLayers-2.13.1/theme/default/style.css?v=20140812" type="text/css">
    <link rel="stylesheet" href="../maps/style.css" type="text/css">
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" href="<%=ISSECURE ? "https" : "http" %>://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />

    


</head>
<body onload="init()">
    <div id="toolbar" class="olbsmtoolbar"></div>

    <div id="mapsearchtoolbar" class="mapsearchtoolbar">
        <input type="text" style="width: 300px; height: 26px;" class="formtext bsmforminput" id="txtSearchAddress" name="txtSearchAddress" placeholder="" onfocus="$('#searchAddressOptions').hide();" />

        <div id="searchAddressBtnW">


            <button id="searchAddressBtn" class="searchAddressBtn" onclick="searchAddress();">
                <span class="searchAddressBtnI"></span>
            </button>
        </div>
    </div>
    <div id="searchAddressOptions">
        <div id="searchAddressOptionsBody">

            <input type="checkbox" checked name="SearchAddressBy[]" value="GoogleAddress" />Search Address
            <br />
            <br />
            Searchable Overlays:
            <div id="searchAddressOptionsOverlays" style="margin-left: 20px;">
            </div>
        </div>
        <div id="searchAddressOptionsFoot">
            <a href="javascript:void(0);" onclick="$('#searchAddressOptions').hide();">Close</a>
        </div>
    </div>

    <div id="undobar" style="border: 0; z-index: 1000; position: absolute; top: 10px; left: 220px; width: 350px; display: none" class="message">The shape has been changed. <span id="undolink"><a href="javascript:void(0)" onclick="undopolygon();">Undo</a></span> (<span id="undonum">1</span>)<span id="undosaving" style="margin-left: 10px;">Saving...</span><span id="savechanges"><a href="javascript:void(0)" onclick="savechanges();" style="margin-left: 10px;">Save</a> <a href="javascript:void(0)" onclick="cancelchanges();">Cancel</a></span></div>
    <div id="redrawbar" style="border: 0; z-index: 1000; position: absolute; top: 10px; left: 220px; width: 350px; display: none" class="message">Redraw Landmark: <span id="redrawLandmarkName" style="font-weight: bold; font-style: italic;"></span>. <span id="saveredrawchanges" style="display: none"><a href="javascript:void(0)" onclick="saveredrawchanges();" style="margin-left: 10px;">Save</a></span> <a href="javascript:void(0)" onclick="cancelredraw();">Cancel</a></div>
    <div id="messagebar" style="border: 0; z-index: 1002; position: absolute; top: 43px; left: 220px; width: 300px; display: none" class="message"></div>
    <div id="searchmessage" style="border: 0; z-index: 1000; position: absolute; top: 40px; left: 125px; width: 300px; display: none" class="searchmessage"></div>
    <% if (ShowAlarmPage)
       { %>
    <div id="alarmpage">
        <iframe src="../Map/frm_Alarms.aspx?s=newmap" style="width: 100%; height: 100%"></iframe>
    </div>
    <%} %>
    <div id="map" class="largemap"></div>

    <div id="controls" style="display: none">
        <ul id="controlToggle">
            <li>
                <input type="radio" name="type" value="none" id="noneToggle" onclick="toggleControl(this);" checked="checked" />
                <label for="noneToggle">navigate</label>
            </li>
            <li>
                <input type="radio" name="type" value="select" id="selectToggle" onclick="toggleControl(this);" />
                <label for="selectToggle">select polygon on click</label>
            </li>
            <li>
                <input type="radio" name="type" value="circle" id="circleToggle" onclick="toggleControl(this);" />
                <label for="pointToggle">draw circle</label>
            </li>
            <li>
                <input type="radio" name="type" value="polygon" id="polygonToggle" onclick="toggleControl(this);" />
                <label for="polygonToggle">draw polygon</label>
            </li>
            <li>
                <input type="radio" name="type" value="modify" id="modifyToggle" onclick="toggleControl(this);" />
                <label for="modifyToggle">modify feature</label>

            </li>
        </ul>
    </div>
    <div id="map_canvas"></div>

    <script type="text/javascript">
        var userDate ='<%=sn.User.DateFormat %>';
        var userTime ='<%=sn.User.TimeFormat %>';
        var DefaultUserDate = '<%=sn.User.DateFormat %>';
        
        function getDatePickerDateFormat()
        {
            if(userDate == 'dd/MM/yyyy')
                userDate = 'dd/mm/yy';            
            else if(userDate == 'MM/dd/yyyy')
                userDate ='mm/dd/yy';
            else
                userDate = 'dd/mm/yy';
            return userDate;
            //if(userTime =="hh:mm:ss tt")
            //    userTime="hh:mm:ss tt";
            //else
            //    userTime="hh:mm:ss tt";
            //return userDate+" "+userTime;
        }
        DefaultUserDate = getDatePickerDateFormat();
        var SelectedLanguage = "<%=SelectedLanguage %>";
           var tooltip_Pan_Map = "<%=tooltip_Pan_Map %>";
        var tooltip_Add_a_landmark_Circle = "<%=tooltip_Add_a_landmark_Circle %>";
        var tooltip_Draw_a_Polygon = "<%=tooltip_Draw_a_Polygon %>";
        var tooltip_Modify_feature = "<%=tooltip_Modify_feature %>";
        var tooltip_UserPreference = "<%=tooltip_UserPreference %>"; //Salman, FR translation
        var tooltip_PrintMap = "<%=tooltip_PrintMap %>"; //Salman (Mantis# 30??)
        var ResEdit = "<%=ResEdit %>";

        var alandmarkformmoreoptionsText = "<%=Res_alandmarkformmoreoptionsText %>";
        var alertDeletePolygonFailedText = "<%=Res_alertDeletePolygonFailedText %>";
        var alertEditPolygonFailedText = "<%=Res_alertEditPolygonFailedText%>";
        var alertErrorText = "<%=Res_alertErrorText%>";
        var alertGeozoneLoadFailedText = "<%=Res_alertGeozoneLoadFailedText %>";
        var alertLandmarkLoadFailedText = "<%=Res_alertLandmarkLoadFailedText %>";
        var btnDeletePolygonText = "<%=Res_btnDeletePolygonText %>";
        var CancelText = "<%=Res_CancelText %>";
        var cboDirection_DisableText = "<%=Res_cboDirection_DisableText %>";
        var cboDirection_InOutText = "<%=Res_cboDirection_InOutText %>";
        var cboDirection_InText = "<%=Res_cboDirection_InText %>";
        var cboDirection_OutText = "<%=Res_cboDirection_OutText %>";
        var cboGeoZoneSeverity_CriticalText = "<%=Res_cboGeoZoneSeverity_CriticalText %>";
        var cboGeoZoneSeverity_NoAlarmText = "<%=Res_cboGeoZoneSeverity_NoAlarmText %>";
        var cboGeoZoneSeverity_NotifyText = "<%=Res_cboGeoZoneSeverity_NotifyText %>";
        var cboGeoZoneSeverity_WarningText = "<%=Res_cboGeoZoneSeverity_WarningText %>";
        var cboTimeZoneGMT = "<%=Res_cboTimeZoneGMT %>";
        var chkCriticalText = "<%=Res_chkCriticalText %>";
        var chkDayLightText = "<%=Res_chkDayLightText %>";
        var chkNotifyText = "<%=Res_chkNotifyText %>";
        var chkWarningText = "<%=Res_chkWarningText %>";
        var ClosestVehiclesNumOfVehiclesText = "<%=Res_ClosestVehiclesNumOfVehicles %>";
        var closeText = "<%=Res_closeText %>";
        var cvCustomSpeedText = "<%=Res_cvCustomSpeedText %>";
        var cvDriverText = "<%=Res_cvDriverText %>";
        var cvEquipmentText = "<%=Res_cvEquipmentText %>";
        var cvHistoryInfoIdTitle = "<%=Res_cvHistoryInfoIdTitle %>";
        var cvListNameText = "<%=Res_cvListNameText %>";
        var cvLocationTitle = "<%=Res_cvLocationTitle %>";
        var cvMyHeadingText = "<%=Res_cvMyHeadingText %>";
        var cvTeamLeaderNameText = "<%=Res_cvTeamLeaderNameText %>";
        var cvVehicleStatusText = "<%=Res_cvVehicleStatusText %>";
        var errorContentFailedDataFetchText = "<%=Res_errorContentFailedDataFetchText %>";
        var geozoneDefaultFormTitle = "<%=Res_geozoneDefaultFormTitle %>";
        var geozoneDefaultInfoTitle = "<%=Res_geozoneDefaultInfoTitle %>";
        var geozoneLandmarkFormTitle = "<%=Res_geozoneLandmarkFormTitle %>";
        var geozoneLandmarkInfoTitle = "<%=Res_geozoneLandmarkInfoTitle %>";
        var h6CircleContentText = "<%=Res_h6CircleContentText %>";
        var h6featureVehiclesText = "<%=Res_h6featureVehiclesText %>";
        var h6NewGeozoneLandmarlText = "<%=Res_h6NewGeozoneLandmarlText %>";
        var lblCallTimerTitle = "<%=Res_lblCallTimerTitle %>";
        var lblContactNameTitle = "<%=Res_lblContactNameTitle %>";
        var lblDefaultSeverityTitle = "<%=Res_lblDefaultSeverityTitle %>";
        var lblDirectionTitle = "<%=Res_lblDirectionTitle %>";
        var lblEmailTitle = "<%=Res_lblEmailTitle %>";
        var lblGeozoneDescriptionTitle = "<%=Res_lblGeozoneDescriptionTitle %>";
        var lblGeozoneNameTitle = "<%=Res_lblGeozoneNameTitle %>";
        var lblLandmarkDescriptionTitle = "<%=Res_lblLandmarkDescriptionTitle %>";
        var lblLandmarkNameTitle = "<%=Res_lblLandmarkNameTitle %>";
        var lbllstAddOptionsGeozoneText = "<%=Res_lbllstAddOptionsGeozoneText %>";
        var lblMultipleEmailsText = "<%=Res_lblMultipleEmailsText %>";
        var lblPhoneTitle = "<%=Res_lblPhoneTitle %>";
        var lblRadiusTitle = "<%=Res_lblRadiusTitle %>";
        var lblTimeZoneTitle = "<%=Res_lblTimeZoneTitle %>";
        var lnkAssignGeozoneToFleetText = "<%=Res_lnkAssignGeozoneToFleetText %>";
        var lnkBackText = "<%=Res_lnkBackText %>";
        var lnkClosestVehicleText = "<%=Res_lnkClosestVehicleText %>";
        var lnklandmarkhistorysearchText = "<%=Res_lnklandmarkhistorysearchText %>";
        var lnkloadPopupsendmessageText = "<%=Res_lnkloadPopupsendmessageText %>";
        var lnkRouteAssignmentText = "<%=Res_lnkRouteAssignmentText %>";
        var lnkShowMapHistoryText = "<%=Res_lnkShowMapHistoryText %>";
        var lstAddOptionsLandmarkText = "<%=Res_lstAddOptionsLandmarkText %>";
        var lstPublicPrivate_PrivateText = "<%=Res_lstPublicPrivate_PrivateText %>";
        var lstPublicPrivate_PublicText = "<%=Res_lstPublicPrivate_PublicText %>";
        var messagebarEditLandmarkCircleText = "<%=Res_messagebarEditLandmarkCircleText %>";
        var messagebarGeoLandZoomInText = "<%=Res_messagebarGeoLandZoomInText %>";
        var messagebarGeozoneNonEditableText = "<%=Res_messagebarGeozoneNonEditableText %>";
        var messagebarSaveChangesText = "<%=Res_messagebarSaveChangesText %>";
        var messagebarSavePolygonFailedText = "<%=Res_messagebarSavePolygonFailedText %>";
        var onFeatureSelectContentText = "<%=Res_onFeatureSelectContentText %>";
        var SaveText = "<%=Res_SaveText %>";
        var searchAddressMessageText = "<%=Res_searchAddressMessageText %>";
        var SearchHistoryDateText = "<%=Res_SearchHistoryDateText %>";
        var SearchHistoryMinutesText = "<%=Res_SearchHistoryMinutesText %>";
        var SearchHistoryTimeRangeText = "<%=Res_SearchHistoryTimeRangeText %>";
        var SearchHistoryTimeText = "<%=Res_SearchHistoryTimeText %>";
        var SearchHistoryTitle = "<%=Res_SearchHistoryTitle %>";
        var searchmessageHtmlText = "<%=Res_searchmessageHtmlText %>";
        var SearchText = "<%=Res_SearchText %>";
        var vehicleText = "<%=Res_vehicleText %>";
        var vehicleViewMoreLinkText = "<%=Res_vehicleViewMoreLinkText %>";
        var WMSInfoControlTitle = "<%=Res_WMSInfoControlTitle %>";
        var Res_DefaultViewGroupWindowText = "<%=Res_DefaultViewGroupWindowText %>";
        var Res_DefaultViewGroupBoxLabelUserText = "<%=Res_DefaultViewGroupBoxLabelUserText %>";
        var Res_DefaultViewGroupBoxLabelOrganizationText = "<%=Res_DefaultViewGroupBoxLabelOrganizationText %>";
        var Res_DefaultViewGroupButtonCancelText = "<%=Res_DefaultViewGroupButtonCancelText %>";
        var Res_DefaultViewGroupButtonSaveText = "<%=Res_DefaultViewGroupButtonSaveText %>";
        var Res_By = "<%=Res_By %>";
        var Res_All = "<%=Res_All %>";
        var Res_SearchHistoryFleetHierarchy = "<%=Res_SearchHistoryFleetHierarchy %>";
        var Res_SelectedVehicles = "<%=Res_SelectedVehicles %>";
        var Res_CategoryTitle = "<%=Res_CategoryTitle%>";
        var Res_DeleteConfirmation = "<%=Res_DeleteConfirmation%>";
        var ShowMultiColor = <%= ShowMultiColor.ToString().ToLower() %>; //Added by Rohit Mittal
        var isExtended = <%=isExtended.ToString().ToLower() %>; // Added by Salman (Mantis# 3620; Milton's 24H search; MAr 25, 2014)
        var overlaysettings = <%=overlays %>;
        var basemapSettings = <%=basemapSettings %>;
        var ifShowClusteredVehicleLabel = false;
        var showAlarmPage = <% = ShowAlarmPage.ToString().ToLower() %>;
           if (overlaysettings.vehicleDrivers) {
               if(overlaysettings.vehicleDriversVisibility) ifShowClusteredVehicleLabel = true;        
           }
           else if (overlaysettings.vehiclenames) {
               if(overlaysettings.vehiclenamesVisibility) ifShowClusteredVehicleLabel = true;
           }

           var geoserver = '<%=GeoServer%>';
           var CategoryList = <%=CategoryList %>;
        if (!showAlarmPage) {
            $('#map').addClass('noAlarmPage');
        }
    </script>

    <script type="text/javascript">
        var orgid = '<%=sn.User.OrganizationId %>';
        var interval = '<%=sn.User.GeneralRefreshFrequency %>';
        var GoogleAddressService;
    </script>
    <script src="../scripts/json2.js"></script>
    <%--      <script src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.3&mkt=en-us" type="text/javascript"></script>  --%>

    <script type="text/javascript" src="../sencha/extjs-4.1.0/bootstrap.js"></script>

    <%if (ifShowGoogleStreets || ifShowGoogleHybrid)
      { %>
    <%-- <script src="<%=ISSECURE ? "https" : "http" %>://maps.google.com/maps/api/js?v=3.5&amp;sensor=false"></script>--%>
    <%--<script src="<%=ISSECURE ? "https" : "http" %>://maps.googleapis.com/maps/api/js?v=3&client=gme-bsmwirelessinc&sensor=true"></script>--%>
    <%} %>

    <script type="text/javascript" src="<%=ISSECURE ? "https" : "http" %>://maps.googleapis.com/maps/api/js?v=3&client=gme-bsmwirelessinc&sensor=true&libraries=places"></script>


    <%if (!ISSECURE && (ifShowBingRoads || ifShowBingHybrid))
      { %>
    <script src='http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.1'></script>
    <%} %>

    <%--<script type="text/javascript" src="../Openlayers/OpenLayers-2.11/lib/OpenLayers.js"></script>--%>
    <script src="../Openlayers/OpenLayers-2.13.1/OpenLayers-20140819.js"></script>
    <%--<script src="../Openlayers/OpenLayers-2.12/lib/deprecated.js"></script>--%>
    <script src="../Openlayers/OpenLayers-2.13.1/lib/deprecated.js"></script>
    <script src="../Openlayers/navteqlayer.js?v=20140408"></script>
    <script type="text/javascript" src="../Scripts/OpenLayerMap/utils/cloudmade.js"></script>
    <script type="text/javascript" src="../Scripts/OpenLayerMap/OpenLayerMap.js?v=<%=LastUpdatedOpenLayerMapJs %>"></script>


    <%if (ifShowArcgis)
      { %>
    <script type="text/javascript" src="../Openlayers/OpenLayers-2.13.1/lib/OpenLayers/Layer/ArcGISCache.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/OpenLayerMap/utils/arcgislayers.js"></script>
    <%} %>


    <script type="text/javascript" src="<%=ISSECURE ? "https" : "http" %>://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="<%=ISSECURE ? "https" : "http" %>://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>


    <script type="text/javascript" src="mapmenus.js?v=2015070702"></script>

    <script type="text/javascript">
        var ISSECURE = <%=ISSECURE.ToString().ToLower() %>;   
        var ShowAssignToFleet = <%=ShowAssignToFleet.ToString().ToLower() %>;
        var ShowMapHistorySearch = <%=ShowMapHistorySearch.ToString().ToLower() %>;
        var ShowRouteAssignment = <%=ShowRouteAssignment.ToString().ToLower() %>;
        var ShowCallTimer = <%=ShowCallTimer.ToString().ToLower() %> ;
        var ShowFields = <%=ShowFields.ToString().ToLower() %> ;
        /*Devin Added*/
        var IsAdmin =  '<%=IsAdmin %>';
        var defaultViewWin;
        var DefaultMapCenter = null;
        var DefaultMapZoomLevel = null;

        var ShowPublicLandmarkOption = <%=ShowPublicLandmarkOption.ToString().ToLower() %>;
        var ShowPublicGeoZoneOption  = <%=ShowPublicGeoZoneOption.ToString().ToLower() %>;

        

         <%if (ShowCallTimer)
           { %>
        var CallTimerSelections = "<%=CallTimerSelections %>";
         <%} %>
         
        function assignGeozoneToFleet(geozoneId) {
            var url = "Widgets/FleetAssignment.aspx?objectName=geozoneid&objectId=" + geozoneId;
            var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
            parent.openPopupWindow("Assign to fleet", urlToLoad, 520, 320);
        }    
        $(document).ready(function() {
            GoogleAddressService = new IniGoogleAutoComplete();
            //$( "#txtSearchAddress" ).datepicker();

            $('#txtSearchAddress').focus(function(){
                $('#clear').toggle();
            });

            //Devin Added for default Map View
            <%
        if (sn.MapCenter != null && sn.MapCenter != "" && sn.MapZoomLevel != null && sn.MapZoomLevel != "")
        {
            %>
            DefaultMapCenter = new OpenLayers.LonLat(<% = sn.MapCenter %>);
            DefaultMapZoomLevel = <% = sn.MapZoomLevel %>;
            <%} %>
        });

        //Devin Added for setting default map view for user ference or organization 
        function SettingDefaultMap() {
            
            var mapCenter = map.getCenter().lon + "," + map.getCenter().lat;
            var mapZoom = map.getZoom();
            if (IsAdmin == "1")
            {
                if (!defaultViewWin) {
                    var defaultViewConfirmForm = Ext.widget('form', {
                        layout: {
                            type: 'vbox',
                            align: 'stretch'
                        },
                        border: false,
                        bodyPadding: 10,

                        fieldDefaults: {
                            labelAlign: 'top',
                            labelWidth: 100,
                            labelStyle: 'font-weight:bold'
                        },
                        defaults: {
                            margins: '0 0 10 0'
                        },

                        items: [{
                            xtype: "radiogroup",
                            //fieldLabel: "DefaultView",
                            id: "DefaultViewgroup",
                            defaults: {xtype: "radio"},
                            items: [
                                {
                                    boxLabel: Res_DefaultViewGroupBoxLabelUserText, //"For User",
                                    inputValue: "User",
                                    name:"defaultViewselection",
                                    checked:true
                                },
                                {
                                    boxLabel: Res_DefaultViewGroupBoxLabelOrganizationText, //"For Organization",
                                    name:"defaultViewselection",
                                    inputValue: "Organization",
                                }
                            ]
                        }],
                        buttonAlign: 'center',
                        buttons: [{
                            text: Res_DefaultViewGroupButtonCancelText, //'Cancel',
                            handler: function() {
                                this.up('form').getForm().reset();
                                this.up('window').hide();
                            }
                        }, {
                            text: Res_DefaultViewGroupButtonSaveText, //'Save',
                            handler: function() {
                                if (this.up('form').getForm().isValid()) {
                                    // In a real application, this would submit the form to the configured url
                                    // this.up.getForm()('form').getForm().submit();
                                    var selectionType = this.up('form').getForm().getValues()['defaultViewselection'];
                                    var postData = "{'type':'" + selectionType + 
                                         "','center':'"  +  mapCenter + 
                                         "','zoomlevel':'"  +  mapZoom + 
                                         "'}";
                                    SetMapDefaultView(postData);
                                    this.up('form').getForm().reset();
                                    this.up('window').hide();
                                }
                            }
                        }]
                    });

                    defaultViewWin = Ext.widget('window', {
                        title: Res_DefaultViewGroupWindowText, //'Default map view setting',
                        closeAction: 'hide',
                        width: 400,
                        height: 150,
                        minHeight: 150,
                        layout: 'fit',
                        resizable: false,
                        modal: true,
                        items: defaultViewConfirmForm
                    });
                }
                defaultViewWin.show();
            }
            else
            {
                if (confirm("<%= Res_Sure_Set_Default%>"))
                {
                    var postData = "{'type':'user"  + 
                                   "','center':'"  +  mapCenter + 
                                   "','zoomlevel':'"  +  mapZoom + 
                                    "'}";
                    SetMapDefaultView(postData);

                }
            }
        }

        //Salman
        function printMap() {
            window.print(); /* @media print in style/css controlling printing behaviour*/
        }

        function SetMapDefaultView(postData)
        {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "OpenLayerMaps.aspx/SetMapDefaultView",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.d != '-1' && data.d != "0") {
                        alert("<%= Res_SaveSuccessfully%>");
                           //AutoReloadDetails();
                       }

                       if (data.d == '-1') {
                           top.document.all('TopFrame').cols = '0,*';
                           window.open('../Login.aspx', '_top')
                       }
                       if (data.d == '0') {
                           alert("<%= Res_ErrorSave%>");
                            return false;
                        }

                   },
                   error: function (request, status, error) {
                       alert("<%= Res_ErrorSave%>");
                  return false;
              }

               });        
          }

          function getSearchAddressOptions(){
              var selectedlayers = "";
              $.each($("input[name='SearchAddressBy[]']:checked"), function() {
                  selectedlayers += $(this).val() + ";";                                  
              });
              return selectedlayers;
          }

          function GotoMapPosition(lat, lon){
              $('#txtSearchAddress').val(lat + "," + lon);
              $('#searchAddressBtn').click();
          }

    </script>

    <link rel="stylesheet" type="text/css" href="menus.css" />
    <script type="text/javascript" src="../scripts/NewMap/landmarkEditForm.js"></script>
</body>
</html>


