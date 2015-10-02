<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewMap.aspx.cs" Inherits="SentinelFM.NewVehicleList" Async="true" EnableViewState="false" meta:resourcekey="PageResource1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Vehicle List</title>

    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/examples/shared/example.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/<%=ExtjsVersion %>/examples/ux/grid/css/RangeMenu.css" />
    <link href="./bootstrap/css/bootstrapscope.css" rel="stylesheet" media="screen" />

    <style type="text/css">
        .x-menu-item img.preview-right, .preview-right {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-right.gif);
        }

        .x-menu-item img.preview-bottom, .preview-bottom {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-bottom.gif);
        }

        .x-menu-item img.preview-hide, .preview-hide {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-hide.gif);
        }

        .x-menu-item img.preview-top, .preview-top {
            background-image: url(./sencha/<%=ExtjsVersion %>/examples/feed-viewer/images/preview-top.gif);
        }

        #reading-menu .x-menu-item-checked {
            border: 1px dotted #a3bae9 !important;
            background: #DFE8F6;
            padding: 0;
            margin: 0;
        }


        .grid-row-red {
            background-color: red;
        }

        .grid-row-yellow {
            background-color: yellow;
        }

        .x-grid3-scroller {
            overflow-y: scroll;
        }

        .cmbLabel {
            float: left;
            z-index: 2;
            position: relative;
            font: normal 12px tahoma, arial, verdana, sans-serif !important;
            -moz-user-select: none;
            -khtml-user-select: none;
            -webkit-user-select: ignore;
            cursor: default;
        }

        .x-panel-body {
            border-top: 0px solid #fafad2 !important;
            border-bottom: 0px solid #fafad2 !important;
            font: normal 12px tahoma, arial, verdana, sans-serif !important;
        }

        .cmbfonts {
            font: normal 12px tahoma, arial, verdana, sans-serif !important;
        }

        .map {
            background: url('./Styles/SentinelFM/resources/icons/map.png') no-repeat 0 0 !important;
        }

        .x-livesearch-match {
            font-weight: bold;
            background-color: yellow;
        }

        .x-toolbar .x-form-cb-wrap {
            line-height: 22px;
        }

        .x-toolbar .x-form-checkbox {
            vertical-align: 0;
        }

        .x-grid-with-col-lines .x-grid-cell {
            border-right: 0;
        }

        .withinlastday .x-date-time {
            background-color: #7BB273 !important;
        }

        .withinlast2days .x-date-time {
            background-color: #EFD700 !important;
        }

        .withinlast3days .x-date-time {
            background-color: #FFA64A !important;
        }

        .withinlast7days .x-date-time {
            background-color: #DE7973 !important;
        }

        .morethan7days .x-date-time {
            background-color: #637DA5 !important;
        }

        .latereceived .x-date-time {
            background-color: #6A5ACD !important;
            color: White !important;
        }

        .collapsebutton {
            position: absolute;
            float: right;
            margin-right: 20px;
            margin-top: 0;
            z-index: 1000;
        }

        .togglemap {
            position: absolute;
            float: right;
            margin-right: 120px;
            margin-top: 0;
            z-index: 1000;
        }

        .expandgridbutton {
            position: absolute;
            float: right;
            margin-right: 50px;
            margin-top: 0;
            z-index: 1000;
        }

        .history-icons {
            background-image: url('./Styles/SentinelFM/resources/icons/ui-icons_ef8c08_256x240.png') !important;
            background-repeat: no-repeat;
            background-color: #F6F6F6;
        }

        .history-icons-hover, .history-icons-hover .history-icons {
            background-color: #FDF5CE;
        }

        .replayPlay {
            background-position: 0 -160px;
        }

        .replayPause {
            background-position: -16px -160px;
        }

        .replayClose {
            background-position: -80px -128px;
        }

        .replayIncrease {
            background-position: 0 -48px; /*0 -16px;*/
        }

        .replayDecrease {
            background-position: -64px -48px; /*-64px -16px;*/
        }

        /*.dashboardView {
            display: none;
        }*/
    </style>



    <link rel="stylesheet" type="text/css" href="Scripts/jqchart/css/jquery.jqChart.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/jqchart/css/jquery.jqRangeSlider.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/jqchart/themes/smoothness/jquery-ui-1.8.21.css" />

    <%if (LoadVehiclesBasedOn == "hierarchy")
      {%>

    <link rel="stylesheet" href="scripts/tablesorter/themes/report/style.css" type="text/css" />


    <style type="text/css">
        .style1 {
            width: 45px;
        }

        #vehicledetails {
            /*width:450px;*/
            margin-right: 16px;
        }



        .kd-button {
            -moz-transition: all 0.218s ease 0s;
            background-color: #F5F5F5;
            background-image: -moz-linear-gradient(center top, #F5F5F5, #F1F1F1);
            /*border: 1px solid rgba(0, 0, 0, 0.1);*/
            border: 1px solid #D8D8D8;
            border-radius: 2px 2px 2px 2px;
            color: #444444;
            display: inline-block;
            font-size: 100%;
            font-family: arial,​sans-serif;
            font-weight: bold;
            height: 27px;
            line-height: 27px;
            min-width: 54px;
            padding: 0 8px;
            text-align: center;
        }

            .kd-button:hover {
                -moz-transition: all 0s ease 0s;
                background-color: #F8F8F8;
                background-image: -moz-linear-gradient(center top, #F8F8F8, #F1F1F1);
                border: 1px solid #C6C6C6;
                box-shadow: 0 1px 1px rgba(0, 0, 0, 0.1);
                color: #333333;
                text-decoration: none;
            }

        .kd-button-disabled, .kd-button-disabled:hover {
            -moz-transition: all 0.218s ease 0s !important;
            background-color: #F5F5F5 !important;
            background-image: -moz-linear-gradient(center top, #F5F5F5, #F1F1F1) !important;
            border: 1px solid #D8D8D8 !important;
            border-radius: 2px 2px 2px 2px !important;
            color: #cccccc !important;
            font-weight: normal !important;
            box-shadow: none !important;
        }
    </style>


    <%} %>
</head>
<body>

    <%if (LoadVehiclesBasedOn == "hierarchy")
      {%>
    <div id="organizationHierarchyTree" style="display: none;">
        <div id="ohsearchbar" class="formtext">
            <asp:Label ID="Label8" runat="server"
                CssClass="tableheading" Text="Search: " meta:resourcekey="Label8Resource1"></asp:Label>
            <input type="text" id="ohsearchbox" class="ohsearch" />
            <a href="javascript:void(0);" onclick="onsearchbtnclicked('reports/vehicleListTree.asmx/SearchOrganizationHierarchy');">
                <img src="../images/searchicon.png" border="0" /></a>
            <asp:Label ID="Label10" runat="server" Style="color: #666666; display: none"
                Text="(Type in at least 3 characters to search)"
                meta:resourcekey="Label10Resource1"></asp:Label>
        </div>
        <div id="ohsearchresult">
            <div id="ohsearchresulttitle">
                Search Result: <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">Close</a>
            </div>
            <div id="ohsearchresultlist">
                <ul></ul>
            </div>
        </div>
        <div id="MySplitter">

            <div id="LeftPane">
                <div id="vehicletreeview" class="demo"></div>
            </div>
            <div id="RightPane">
                <div id="vehicledetails">
                    <table cellspacing="0" class="vehiclelisttbl tablesorter" id="vehiclelisttbl">
                        <thead>
                            <tr>
                                <th>Vehicle</th>

                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>
        <div>
            <input type="button" class="kd-button" onclick="applyOrganizationHierarchy();" id="Button1" value="OK" />
            <input type="button" class="kd-button" onclick="cancelOrganizationHierarchy();" id="Button2" value="Cancel" />
        </div>
    </div>
    <%} %>

    <iframe id="exportframe" name="exportframe" style="display: none"></iframe>
    <form id="exportform" method="post" target="exportframe" action="./MapNew/frmExportData.aspx">
        <input type="hidden" id="exportdata" name="exportdata" value="" />
        <input type="hidden" id="filename" name="filename" value="" />
        <input type="hidden" id="formatter" name="formatter" value="" />
    </form>

    <div id="vehicleInfoPopover"></div>

    <script type="text/javascript">
        if (!Array.prototype.indexOf) {
            Array.prototype.indexOf = function (obj, start) {
                for (var i = (start || 0), j = this.length; i < j; i++) {
                    if (this[i] === obj) { return i; }
                }
                return -1;
            }
        }
        var userDate ='<%=sn.User.DateFormat %>';
        var userTime ='<%=sn.User.TimeFormat %>';
        var userDefaultDate = '<%=sn.User.DateFormat %>';
        var showsensor = '<%=sn.History.DsAssSensors%>';
        function getSenchaDateFormat()
        {
            if(userDate == 'dd/MM/yyyy')
                userDate = 'd/m/Y';
            else if(userDate == 'd/M/yyyy')
                userDate ='j/n/Y';
            else if(userDate == 'dd/MM/yy')
                userDate ='d/m/y';
            else if(userDate == 'd/M/yy')
                userDate ='j/n/y';
            else if(userDate == 'd MMM yyyy')
                userDate ='j M Y';
            else if(userDate == 'MM/dd/yyyy')
                userDate ='m/d/Y';
            else if(userDate == 'M/d/yyyy')
                userDate = 'n/j/Y';
            else if(userDate == 'MM/dd/yy')
                userDate = 'm/d/y';
            else if(userDate == 'M/d/yy')
                userDate = 'n/j/y';
            else if(userDate == 'MMMM d yy')
                userDate = 'M j y';
            else if(userDate == 'yyyy/MM/dd')
                userDate = 'Y/m/d';
            if(userTime =="hh:mm:ss tt")
                userTime="h:i:s A";
            else
                userTime="H:i:s";
            return userDate+" "+userTime;
        }
        var userdateformat = getSenchaDateFormat();
      
        var ResVehicles = "<%=ResVehicles %>";
        var ResAlarms = "<%=ResAlarms %>";
        var ResMessages = "<%=ResMessages %>";
        var ResGeozoneLandmarks = "<%=ResGeozoneLandmarks %>";
        var ResDispatch = "<%=ResDispatch %>";
        var ResVehiclePagerDisplayMsg = "<%=ResVehiclePagerDisplayMsg %>";
        var ResVehiclePagerEmptyMsg = "<%=ResVehiclePagerEmptyMsg %>";

        var ResUpdatePositionCommandStatus = "<%=ResUpdatePositionCommandStatus %>";
        var ResGeozones = "<%=ResGeozones %>";
        var ResLandmarks = "<%=ResLandmarks %>";
        var ResLocation = "<%=ResLocation %>";
        var ResTrip = "<%=ResTrip %>";
        var ResHistory = "<%=ResHistory %>";
        var ResSearchResult = "<%=ResSearchResult %>";
        var ResTrackVehicles = "<%=ResTrackVehicles %>";
        var ResSearch = "<%=ResSearch %>";
        var ResMaptheselectedvehicle = "<%=ResMaptheselectedvehicle %>";
        var ResTrackSelectedVehicleOnSeparateMap = "<%=ResTrackSelectedVehicleOnSeparateMap %>";
        var ResGoogleStreetView = "<%=ResGoogleStreetView %>";
        var ResMapSelectedVehicleOnMap = "<%=ResMapSelectedVehicleOnMap %>";
        var ResRefreshTheMapAndGridAutomatically = "<%=ResRefreshTheMapAndGridAutomatically %>";
        var ResVehicleGeozoneAssignment = "<%=ResVehicleGeozoneAssignment %>";
        var ResSearchHistory = "<%=ResSearchHistory %>";
        var ResMapTheSelectedHistoryRecords = "<%=ResMapTheSelectedHistoryRecords %>";
        var ResSendCMDHistoryRecords = "<%=ResSendCMDHistoryRecords %>";
        var ResSelectVehicles = "<%=ResSelectVehicles %>";
        var ResSelectCommMode = "<%=ResSelectCommMode %>";
        var ResClearAllSelectedVehicles = "<%=ResClearAllSelectedVehicles %>";
        var ResExport = "<%=ResExport %>";
        var ResMapItButMapTheSelectedVehicle = "<%=ResMapItButMapTheSelectedVehicle %>";
        var ResUpdatePositionButtonToolTip = "<%=ResUpdatePositionButtonToolTip %>";
        var ResWantToImproveOurMap = "<%=ResWantToImproveOurMap %>";
        var ResClearAllButtonToolTip = "<%=ResClearAllButtonToolTip %>";
        var ResFindVehiclesDrivers = "<%=ResFindVehiclesDrivers %>";
        var ResLegendDateTimeColor = "<%=ResLegendDateTimeColor %>";
        var ResOrganizationHierarchy = "<%=ResOrganizationHierarchy %>";
        var ResSelectFleet = "<%=ResSelectFleet %>";
        var ResHistoryOrganizationHierarchyButtonToolTip = "<%=ResHistoryOrganizationHierarchyButtonToolTip %>";
        var ResHistoryFleetButtonSelectfleet = "<%=ResHistoryFleetButtonSelectfleet %>";
        var ResLabelonoffButtonHideShowLabel = "<%=ResLabelonoffButtonHideShowLabel %>";
        var ResClearSearchBtnHideShowLabel = "<%=ResClearSearchBtnHideShowLabel %>";
        var ResSendmessageButtonSendMessage = "<%=ResSendmessageButtonSendMessage %>";
        var ResStreetViewButtonGoogleStreetView = "<%=ResStreetViewButtonGoogleStreetView %>";
        var ResTrackitButtonToolTip = "<%=ResTrackitButtonToolTip %>";
        var ResDemomapButtonToolTip = "<%=ResDemomapButtonToolTip %>";

        var ResClearAllButtonText = "<%=ResClearAllButtonText %>";
        var ResFindVehiclesDriversButtonText = "<%=ResFindVehiclesDriversButtonText %>";
        var ResLegendButtonText = "<%=ResLegendButtonText %>";
        //Added by Rohit Mittal for Trip Colors
        var ResHisTripColorsButtonText = "<%=ResHisTripColorsButtonText %>";
        var ResHisTripColors = "<%=ResHisTripColors %>";
        var ShowMultiColor = <%= ShowMultiColor.ToString().ToLower() %>;
        var ResClearSearchBtnText = "<%=ResClearSearchBtnText %>";
        var ResSendmessageButtonText = "<%=ResSendmessageButtonText %>";
        var ResStreetViewButtonText = "<%=ResStreetViewButtonText %>";
        var ResStreetviewAlertMessage = "<%=ResStreetviewAlertMessage %>";
        var ResStreetviewAlertTitle = "<%=ResStreetviewAlertTitle %>";
        var ResTrackitButtonText = "<%=ResTrackitButtonText %>";
        var ResDemomapButtonText = "<%=ResDemomapButtonText %>";
        var ResSearchMapButtonText = "<%=ResSearchMapButtonText %>";
        var ResAlarmsPagerdisplayMsg = "<%=ResAlarmsPagerdisplayMsg %>";
        var ResAlarmsPageremptyMsg = "<%=ResAlarmsPageremptyMsg %>";
        var ResMessagesPagerdisplayMsg = "<%=ResMessagesPagerdisplayMsg %>";
        var ResMessagesPageremptyMsg = "<%=ResMessagesPageremptyMsg %>";
        var ResHistoryPagerdisplayMsg = "<%=ResHistoryPagerdisplayMsg %>";
        var ResHistoryPageremptyMsg = "<%=ResHistoryPageremptyMsg %>";
        var ResHistoryPageralert = "<%=ResHistoryPageralert %>";
        var ResHistorygridalertMessage = "<%=ResHistorygridalertMessage %>";
        var ResHistorygridalertTitle = "<%=ResHistorygridalertTitle %>";
        var ResFinditmenuText = "<%=ResFinditmenuText %>";
        var ResTrackitmenuText = "<%=ResTrackitmenuText %>";
        var ResStreetViewMenuText = "<%=ResStreetViewMenuText %>";
        var ResStreetViewMenualertMessage = "<%=ResStreetViewMenualertMessage %>";
        var ResStreetViewMenualertTitle = "<%=ResStreetViewMenualertTitle %>";
        var ResUpdatePositionMenuText = "<%=ResUpdatePositionMenuText %>";
        var ResClearAllMenuText = "<%=ResClearAllMenuText %>";
        var ResExportToCsvButtonText = "<%=ResExportToCsvButtonText %>";
        var ResExportToExcel2003ButtonText = "<%=ResExportToExcel2003ButtonText %>";
        var ResExportToExcel2007ButtonText = "<%=ResExportToExcel2007ButtonText %>";
        var ResExportHistoryToCsvButtonText = "<%=ResExportHistoryToCsvButtonText %>";
        var ResExportHistoryToExcel2003ButtonText = "<%=ResExportHistoryToExcel2003ButtonText %>";
        var ResExportHistoryToExcel2007ButtonText = "<%=ResExportHistoryToExcel2007ButtonText %>";
        var ResExportHistoryStopToCsvButtonText = "<%=ResExportHistoryStopToCsvButtonText %>";
        var ResExportHistoryStopToExcel2003ButtonText = "<%=ResExportHistoryStopToExcel2003ButtonText %>";
        var ResExportHistoryStopToExcel2007ButtonText = "<%=ResExportHistoryStopToExcel2007ButtonText %>";
        var ResExportHistoryTripToCsvButtonText = "<%=ResExportHistoryTripToCsvButtonText %>";
        var ResExportHistoryTripToExcel2003ButtonText = "<%=ResExportHistoryTripToExcel2003ButtonText %>";
        var ResExportHistoryTripToExcel2007ButtonText = "<%=ResExportHistoryTripToExcel2007ButtonText %>";
        var ResExportHisotryAddressToCsvButtonText = "<%=ResExportHisotryAddressToCsvButtonText %>";
        var ResExportHistoryAddressToExcel2003ButtonText = "<%=ResExportHistoryAddressToExcel2003ButtonText %>";
        var ResExportHistoryAddressToExcel2007ButtonText = "<%=ResExportHistoryAddressToExcel2007ButtonText %>";
        var ResvgUnitIDText = "<%=ResvgUnitIDText %>";
        var ResvgDriverText = "<%=ResvgDriverText %>";
        var ResvgDescriptionText = "<%=ResvgDescriptionText %>";
        var ResvgStatusText = "<%=ResvgStatusText %>";
        var ResvgSpeedText = "<%=ResvgSpeedText %>";
        var ResvgDateTimeText = "<%=ResvgDateTimeText %>";
        var ResvgAddressText = "<%=ResvgAddressText %>";
        var ResvgArmedText = "<%=ResvgArmedText %>";
        var ResvgHistoryText = "<%=ResvgHistoryText %>";
        var ResvgDriverCardNumberText = "<%=ResvgDriverCardNumberText %>";
        var ResvgField1Text = "<%=ResvgField1Text %>";
        var ResvgField2Text = "<%=ResvgField2Text %>";
        var ResvgField3Text = "<%=ResvgField3Text %>";
        var ResvgField4Text = "<%=ResvgField4Text %>";
        var ResvgField5Text = "<%=ResvgField5Text %>";
        var ResvgModelYearText = "<%=ResvgModelYearText %>";
        var ResvgMakeNameText = "<%=ResvgMakeNameText %>";
        var ResvgModelNameText = "<%=ResvgModelNameText %>";
        var ResvgVehicleTypeNameText = "<%=ResvgVehicleTypeNameText %>";
        var ResvgLicensePlateText = "<%=ResvgLicensePlateText %>";
        var ResvgVinNumText = "<%=ResvgVinNumText %>";
        var ResvgManagerNameText = "<%=ResvgManagerNameText %>";
        var ResvgManagerEmployeeIdText = "<%=ResvgManagerEmployeeIdText %>";
        var ResvgStateProvinceText = "<%=ResvgStateProvinceText %>";
        var ResvgEngineHoursText = "<%=ResvgEngineHoursText %>";
        var ResvgOdometerText = "<%=ResvgOdometerText %>";
        var ResvgIsRouteAssignedText = "<%=ResvgIsRouteAssignedText %>";
        var ResdockedItemsActionsText = "<%=ResdockedItemsActionsText %>";
        var ResdockedItemsExportText = "<%=ResdockedItemsExportText %>";
        var ResalarmgridEmptyText = "<%=ResalarmgridEmptyText %>";
        var ResalarmgridcolumnsAlarmId = "<%=ResalarmgridcolumnsAlarmId %>";
        var ResalarmgridcolumnsTimeCreated = "<%=ResalarmgridcolumnsTimeCreated %>";
        var ResalarmgridcolumnsAlarmLevel = "<%=ResalarmgridcolumnsAlarmLevel %>";
        var ResalarmgridcolumnsAlarmDescription = "<%=ResalarmgridcolumnsAlarmDescription %>";
        var ResalarmgridcolumnsvehicleDescription = "<%=ResalarmgridcolumnsvehicleDescription %>";
        var ResmessagegridemptyText = "<%=ResmessagegridemptyText %>";
        var ResmessagegridcolumnsMessageId = "<%=ResmessagegridcolumnsMessageId %>";
        var ResmessagegridcolumnsMsgDateTime = "<%=ResmessagegridcolumnsMsgDateTime %>";
        var ResmessagegridcolumnsDescription = "<%=ResmessagegridcolumnsDescription %>";
        var ResmessagegridcolumnsMsgBody = "<%=ResmessagegridcolumnsMsgBody %>";
        var ResmessagegridcolumnsAcknowledged = "<%=ResmessagegridcolumnsAcknowledged %>";
        var ResgeolandmarkgridemptyText = "<%=ResgeolandmarkgridemptyText %>";
        var ResvehiclegeozoneassignmentButtonText = "<%=ResvehiclegeozoneassignmentButtonText %>";
        var ResgeozonegridemptyText = "<%=ResgeozonegridemptyText %>";
        var ResgeozonegridcolumnsGeozone = "<%=ResgeozonegridcolumnsGeozone %>";
        var Resgeozonegridcolumnsdesc = "<%=Resgeozonegridcolumnsdesc %>";
        var ResgeozonegridcolumnsDirection = "<%=ResgeozonegridcolumnsDirection %>";
        var ResgeozonegridcolumnsSeverityName = "<%=ResgeozonegridcolumnsSeverityName %>";
        var ReslandmarkgridemptyText = "<%=ReslandmarkgridemptyText %>";
        var Reslandmarkgridname = "<%=Reslandmarkgridname %>";
        var Reslandmarkgriddesc = "<%=Reslandmarkgriddesc %>";
        var ReslandmarkgridStreetAddress = "<%=ReslandmarkgridStreetAddress %>";
        var ReslandmarkgridEmail = "<%=ReslandmarkgridEmail %>";
        var ReslandmarkgridContactPhoneNum = "<%=ReslandmarkgridContactPhoneNum %>";
        var Reslandmarkgridradius = "<%=Reslandmarkgridradius %>";
        var ReslandmarkgridCategoryName = "<%=ReslandmarkgridCategoryName %>";
        var ResbtmHistorySearchText = "<%=ResbtmHistorySearchText %>";
        var ResbtnHistoryMapitText = "<%=ResbtnHistoryMapitText %>";
        var ResbtnHistoryLegendText = "<%=ResbtnHistoryLegendText %>";
        var ResbtnHistoryMapAllText = "<%=ResbtnHistoryMapAllText %>";
        var ResbtnHistorySendCMDText = "<%=ResbtnHistorySendCMDText %>";
        var ReshistoryTypeemptyText = "<%=ReshistoryTypeemptyText %>";
        var ResHistoryVehicleListDescription1 = "<%=ResHistoryVehicleListDescription1 %>";
        var ResHistoryVehicleListDescription2 = "<%=ResHistoryVehicleListDescription2 %>";
        var ReshistoryVehiclesdisplayField = "<%=ReshistoryVehiclesdisplayField %>";
        var ReshistoryVehiclesfieldLabel = "<%=ReshistoryVehiclesfieldLabel %>";
        var ReshistoryVehiclesEmptyText = "<%=ReshistoryVehiclesEmptyText %>";
        var ReshistoryCommModesEmptyText = "<%=ReshistoryCommModesEmptyText %>";
        var ReshistoryMessageCheckBoxboxLabel = "<%=ReshistoryMessageCheckBoxboxLabel %>";
        var ReshistoryTripRadioshtml = "<%=ReshistoryTripRadioshtml %>";
        var ReshistoryTripRadiosboxLabel1 = "<%=ReshistoryTripRadiosboxLabel1 %>";
        var ReshistoryTripRadiosboxLabel2 = "<%=ReshistoryTripRadiosboxLabel2 %>";
        var ReshistoryTripRadiosboxLabel3 = "<%=ReshistoryTripRadiosboxLabel3 %>";
        var ResbtnSubmitMessage = "<%=ResbtnSubmitMessage %>";
        var ResbtnSubmitAlert = "<%=ResbtnSubmitAlert %>";
        var ReshistorygridEmptyText = "<%=ReshistorygridEmptyText %>";
        var ReshistorygridBoxId = "<%=ReshistorygridBoxId %>";
        var ReshistorygridDescription = "<%=ReshistorygridDescription %>";
        var ReshistorygridOriginDateTime = "<%=ReshistorygridOriginDateTime %>";
        var ReshistorygridStreetAddress = "<%=ReshistorygridStreetAddress %>";
        var ReshistorygridSpeed = "<%=ReshistorygridSpeed %>";
        var ReshistorygridBoxMsgInTypeName = "<%=ReshistorygridBoxMsgInTypeName %>";
        var ReshistorygridMsgDetails = "<%=ReshistorygridMsgDetails %>";
        var ResAcknowledged = "<%=ResAcknowledged %>";
        var ReshistoryStopGridEmptyText = "<%=ReshistoryStopGridEmptyText %>";
        var ReshistoryStopGridArrivalDateTime = "<%=ReshistoryStopGridArrivalDateTime %>";
        var ReshistoryStopGridLocation = "<%=ReshistoryStopGridLocation %>";
        var ReshistoryStopGridDepartureDateTime = "<%=ReshistoryStopGridDepartureDateTime %>";
        var ReshistoryStopGridStopDuration = "<%=ReshistoryStopGridStopDuration %>";
        var ReshistoryStopGridRemarks = "<%=ReshistoryStopGridRemarks %>";
        var ReshistoryTripGridEmptyText = "<%=ReshistoryTripGridEmptyText %>";
        var ReshistoryTripGridDescription = "<%=ReshistoryTripGridDescription %>";
        var ReshistoryTripGridDepartureTime = "<%=ReshistoryTripGridDepartureTime %>";
        var ReshistoryTripGridArrivalTime = "<%=ReshistoryTripGridArrivalTime %>";
        var ReshistoryTripGrid_From = "<%=ReshistoryTripGrid_From %>";
        var ReshistoryTripGrid_To = "<%=ReshistoryTripGrid_To %>";
        var ReshistoryTripGridDuration = "<%=ReshistoryTripGridDuration %>";
        var ReshistoryTripGridFuelConsumed = "<%=ReshistoryTripGridFuelConsumed %>";
        var ReshistoryAddressGridhaUnitID = "<%=ReshistoryAddressGridhaUnitID %>";
        var ReshistoryAddressGridhaDescription = "<%=ReshistoryAddressGridhaDescription %>";
        var ReshistoryAddressGridhaDateTime = "<%=ReshistoryAddressGridhaDateTime %>";
        var ReshistoryAddressGridhaDetails = "<%=ReshistoryAddressGridhaDetails %>";
        var ReshidtoryDetailsGridEmptyText = "<%=ReshidtoryDetailsGridEmptyText %>";
        var ReshidtoryDetailsGridBoxId = "<%=ReshidtoryDetailsGridBoxId %>";
        var ReshidtoryDetailsGridDescription = "<%=ReshidtoryDetailsGridDescription %>";
        var ReshidtoryDetailsGridOriginDateTime = "<%=ReshidtoryDetailsGridOriginDateTime %>";
        var ReshidtoryDetailsGridStreetAddress = "<%=ReshidtoryDetailsGridStreetAddress %>";
        var ReshidtoryDetailsGridSpeed = "<%=ReshidtoryDetailsGridSpeed %>";
        var ReshidtoryDetailsGridBoxMsgInTypeName = "<%=ReshidtoryDetailsGridBoxMsgInTypeName %>";
        var ReshidtoryDetailsGridMsgDetails = "<%=ReshidtoryDetailsGridMsgDetails %>";
        var ReshidtoryDetailsGridAcknowledged = "<%=ReshidtoryDetailsGridAcknowledged %>";
        var ReshistoryFormEmptyText = "<%=ReshistoryFormEmptyText %>";

        var ResfleetDefaultText = "<%=ResfleetDefaultText %>";
        var ResloadingMaskMessage = "<%=ResloadingMaskMessage %>";
        var RessearchingMaskMessage = "<%=RessearchingMaskMessage %>";
        var ResmapitButtonText = "<%=ResmapitButtonText %>";
        var ResupdatePositionButtonText = "<%=ResupdatePositionButtonText %>";
        var ResFeedbackButtonText = "<%=ResFeedbackButtonText %>";
        var ResfleetButtonOpenwindowMessage = "<%=ResfleetButtonOpenwindowMessage %>";
        var ReshistoryFleetButtonOpenwindowMessage = "<%=ReshistoryFleetButtonOpenwindowMessage %>";
        var ReslabelonoffButtonlabelonoffShowLabel = "<%=ReslabelonoffButtonlabelonoffShowLabel %>";
        var ReslabelonoffButtonlabelonoffHideLabel = "<%=ReslabelonoffButtonlabelonoffHideLabel %>";
        var ResstreetViewButtonOpenwindowMessage = "<%=ResstreetViewButtonOpenwindowMessage %>";
        var RestrackitButtonOpenwindowMessage = "<%=RestrackitButtonOpenwindowMessage %>";
        var ReshistorygridemptyText2 = "<%=ReshistorygridemptyText2 %>";
        var RestrackitmenuOpenwindowMessage = "<%=RestrackitmenuOpenwindowMessage %>";
        var ResstreetViewMenuOpenwindowMessage = "<%=ResstreetViewMenuOpenwindowMessage %>";
        var Resgeolandmarkgridcolumnsname = "<%=Resgeolandmarkgridcolumnsname %>";
        var ResgeolandmarkgridcolumnsType = "<%=ResgeolandmarkgridcolumnsType %>";
        var ResgeolandmarkgridsetTitle = "<%=ResgeolandmarkgridsetTitle %>";
        var ReshistoryDateFromfieldLabel = "<%=ReshistoryDateFromfieldLabel %>";
        var ReshistoryDateTofieldLabel = "<%=ReshistoryDateTofieldLabel %>";
        var ReshistoryTypefieldLabel = "<%=ReshistoryTypefieldLabel %>";
        var ResbtnSubmitAlertTitle = "<%=ResbtnSubmitAlertTitle %>";
        var ReshistoryFleetButtonsetText = "<%=ReshistoryFleetButtonsetText %>";
        var ReshistoryFormAlertTitle = "<%=ReshistoryFormAlertTitle %>";
        var ReshistoryFormAlertMessage = "<%=ReshistoryFormAlertMessage %>";
        var ResMultipleHierarchy = '<%=ResMultipleHierarchy %>';
        var ResAllRoutesBtnText = "<%=ResAllRoutesBtnText%>";
        var ResSubmitButtonText = "<%=ResSubmitButtonText %>";
        var ResHistoryLinkText = "<%=ResHistoryLinkText %>";
        var ResHistoryLocationText = "<%=ResHistoryLocationText %>";
        var ResHistoryTypeValues_0 = "<%=ResHistoryTypeValues_0 %>";
        var ResHistoryTypeValues_1 = "<%=ResHistoryTypeValues_1 %>";
        var ResHistoryTypeValues_2 = "<%=ResHistoryTypeValues_2 %>";
        var ResHistoryTypeValues_3 = "<%=ResHistoryTypeValues_3 %>";
        var ResHistoryTypeValues_4 = "<%=ResHistoryTypeValues_4 %>";
        var ResFilterText = "<%=ResFilterText %>";
        var ResbtnVehicleonoffShowDetailsText = "<%=ResbtnVehicleonoffShowDetailsText %>";
        var ResbtnVehicleonoffHideDetailsText = "<%=ResbtnVehicleonoffHideDetailsText %>";
        var ResbtnVehicleonoffTooltipText = "<%=ResbtnVehicleonoffTooltipText %>";
        var ResTxtButtonTitleFleetText = "<%=ResTxtButtonTitleFleetText %>";
        var ResTxtButtonTitleHierarchyText = "<%=ResTxtButtonTitleHierarchyText %>";
        var ResHistoryCommModeNameText = "<%=ResHistoryCommModeNameText %>";
        var ResRouteAssignedYes = "<%=ResRouteAssignedYes %>";
        var ResRouteAssignedNo = "<%=ResRouteAssignedNo %>";
        var ResgeozonegridcolumnsCurrentAssignment = "<%=ResgeozonegridcolumnsCurrentAssignment %>";
        var Res_cvColorText = "<%=Res_cvColorText %>";
        var vgrid = '<%=sn.User.VGrid%>';
        var hgrid = '<%=sn.User.HGrid%>';
        var VGridActive = '<%=sn.User.VGridActive%>';
        var hgridactive = '<%=sn.User.HGridActive%>';
        var Res_HistoryReplayReplay = '<%=ResHistoryReplayReplay %>';
        var Res_historyReplayStop = '<%=ResHistoryReplayStop %>';
        var Res_historyReplayTooltip = '<%=ResHistoryReplayTooltip %>';
        var ResBatteryTrendingBtnText = 'Battery Report';
        var ResAddressresolutioninprogress = "<%=ResAddressresolutioninprogress%>";
        var ResResolveAddressToolTips = "<%=ResResolveAddressToolTips%>";
        var ResResolving = "<%=ResResolving%>";
        var ResValidGPS = "<%=ResValidGPS%>";
        var ResTextTrue = "<%=ResTextTrue%>";
        var ResTextFalse = "<%=ResTextFalse%>";
        var ResTextNA = "<%=ResTextNA%>";
        var alarminterval = '<%=sn.User.AlarmRefreshFrequency%>';
        var messageinterval = '<%=sn.User.AlarmRefreshFrequency%>';
        var ShowRetiredVehicles = <%=sn.User.ShowRetiredVehicles.ToString().ToLower()%>;
    </script>

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script type="text/javascript" src="//code.jquery.com/jquery-migrate-1.2.1.js"></script>
    <script type="text/javascript" src="./sencha/<%=ExtjsVersion %>/ext-all.js"></script>
    <% if (sn.SelectedLanguage.Contains("fr-CA"))
       { %>
    <script type="text/javascript" src="./sencha/<%=ExtjsVersion %>/locale/ext-lang-fr_CA.js"></script>
    <%} %>
    <script type="text/javascript" src="./sencha/rowexpander.js"></script>
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/downloadify.min.js"></script>
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/Button.js"></script>
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="./sencha/Ext.ux.Exporter/ExportMenu.js"></script>
    <script type="text/javascript" src="./bootstrap/js/bootstrap.min.js"></script>

    <script src="Scripts/jqchart/js/jquery.mousewheel.js" type="text/javascript"></script>
    <script src="Scripts/jqchart/js/jquery.jqChart.min.js" type="text/javascript"></script>
    <script src="Scripts/jqchart/js/jquery.jqRangeSlider.min.js" type="text/javascript"></script>
    <!--[if IE]><script lang="javascript" type="text/javascript" src="Scripts/jqchart/js/excanvas.js?v=2014032107"></script><![endif]-->

    <% if (LoadVehiclesBasedOn == "hierarchy")
       { %>
    <script type="text/javascript" src="scripts/jquery.cookie.js"></script>
    <script src="scripts/tablesorter/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script src="scripts/json2.js" type="text/javascript"></script>

    <script type="text/javascript" language="javascript">
			<!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var vehicletreeviewIni = false;
        var selectedOrganizationHierarchyNodeCode = '';
        var selectedOrganizationHierarchyFleetName = ''
        var hierarchyBtnReference = 'vehiclelist';
            
        function onOrganizationHierarchyNodeCodeClick() {
            var mypage = 'Widgets/OrganizationHierarchy.aspx?nodecode=' + DefaultOrganizationHierarchyNodeCode + '&loadVehicle=0';
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0&rootNodecode=";//  + PreferOrganizationHierarchyNodeCode;
            }
            var myname = 'OrganizationHierarchy';
            var w = 740;
            var h = 440;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            hierarchyBtnReference = 'vehiclelist';
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName) {
            if (hierarchyBtnReference == 'vehiclelist') {
                selectedOrganizationHierarchyNodeCode = nodecode;
                selectedOrganizationHierarchyFleetName = fleetName;
                TempSelectedOrganizationHierarchyFleetId = fleetId;

                applyOrganizationHierarchy();
            }
            else if (hierarchyBtnReference == 'history') {
                HistoryOrganizationHierarchyNodeCode = nodecode;                    
                historyHiddenFleet.setValue(fleetId);

                historyOrganizationHierarchy.setText(fleetName);
                //loadingMask.show();
                historyVehicleStore.load(
                {
                    params:
                    {
                        fleetID: fleetId
                    }
                });
            }
                
        }

            

        function applyOrganizationHierarchy() {
            organizationHierarchy.setText(selectedOrganizationHierarchyFleetName);
            //$('#organizationHierarchyTree').hide();
            DefaultOrganizationHierarchyFleetId = TempSelectedOrganizationHierarchyFleetId;
            DefaultOrganizationHierarchyNodeCode = selectedOrganizationHierarchyNodeCode;
            loadingMask.show();
            mainstore.load(
                {
                    params:
                    {
                        QueryType: 'GetfleetPosition',
                        fleetID: DefaultOrganizationHierarchyFleetId,
                        start: 0,
                        limit: VehicleListPagesize
                    }
                }
            );
        }

            
            
        //-->
    </script>
    <% } %>

    <script type="text/javascript">
        var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        var mapAssets = <%=mapAssets.ToString().ToLower() %>;
        var maxVehiclesOnMap = <%=maxVehiclesOnMap%>;
        var selectedVehicleBoxId = -1;
        var selectedVehicleData;
        var firstLoad = true;
     
        var overlaysettings = <%=overlays %>;
        var defaultMapView = "<%=defaultMapView %>";
        var VehicleClustering = <%=VehicleClustering.ToString().ToLower() %>;
        var VehicleClusteringDistance = <%=VehicleClusteringDistance %>;
        var VehicleClusteringThreshold = <%=VehicleClusteringThreshold %>;
        var showDriverFinderButton = <%=showDriverFinderButton.ToString().ToLower() %>;
        var showDriverColumn = <%=showDriverColumn.ToString().ToLower() %>;
        var showDriverCardNumberText =true;
        var showField1Text = true;
        var showField2Text = true;
        var showField3Text = true;
        var showField4Text = true;
        var showField5Text = true;
        var showModelYearText = true;
        var showMakeNameText = true;
        var showModelNameText = true;
        var showVehicleTypeNameText =true;
        var showLicensePlateText = true;
        var showVinNumText = true;
        var showManagerNameText = true;
        var showManagerEmployeeIdText = true;
        var showStateProvinceText = true;
        var ShowEngineHours = <%=ShowEngineHours.ToString().ToLower() %>;
        var ShowOdometer = <%=ShowOdometer.ToString().ToLower() %>;
        var ShowPTO = <%=ShowPTO.ToString().ToLower() %>;
        var vehiclegrid;
        var zoomtogeozone = true;
        var ifShowClusteredVehicleLabel = <%=sn.Map.ShowVehicleName.ToString().ToLower()%>;
        var ifShowVehicleIcon=true;
        var OriginIfShowVehicleIcon = true;
        var HistoryPlayPaused = true;

        var VehicleListPagesize = <%=PageSize %>; 
     var HistoryPagesize = <%=HistoryPageSize.ToString() %>;
        //var HistoryPagesize = 10;
        var IE8orUnder = <%=IE8orUnder.ToString().ToLower() %>;

        //Devin Added
        var DispatchOrganizationId = <%=sn.User.OrganizationId %>;

        var ShowAlarmTab = <%=ShowAlarmTab.ToString().ToLower() %>;
        var ShowMessageTab = <%=ShowMessageTab.ToString().ToLower() %>;
        var ShowRouteAssignment = <%=ShowRouteAssignment.ToString().ToLower() %>;
        var ShowScheduleAdherence = <%=ShowScheduleAdherence.ToString().ToLower() %>;
        var ShowHistoryDetails = <%=ShowHistoryDetails.ToString().ToLower() %>;

        var BatteryTredingEnabled = <%=BatteryTredingEnabled.ToString().ToLower() %>;
        var ShowDashboardView = <%=ShowDashboardView.ToString().ToLower() %>;
        //     if (overlaysettings.vehicleDrivers) {
        //        if(overlaysettings.vehicleDriversVisibility) ifShowClusteredVehicleLabel = true;        
        //     }
        //     else if (overlaysettings.vehiclenames) {
        //        if(overlaysettings.vehiclenamesVisibility) ifShowClusteredVehicleLabel = true;
        //     }

        if(defaultMapView == "north")
            defaultMapView = "south";
        else if(defaultMapView == "south")
            defaultMapView = "north";
        else if(defaultMapView =="east")
            defaultMapView = "west";
        else if(defaultMapView == "west")
            defaultMapView = "east";

        var hideMapByDefault = false;
        if (defaultMapView == "none")
        {
            defaultMapView = "south";
            hideMapByDefault = true;
        }

        var LoadVehiclesBasedOn = '<%=LoadVehiclesBasedOn %>';
        var DefaultOrganizationHierarchyFleetId = '<%=DefaultOrganizationHierarchyFleetId %>';
        var DefaultOrganizationHierarchyFleetName = '<%=DefaultOrganizationHierarchyFleetName %>';
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var PreferOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        var HistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
        var HistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

        var HistoryEnabled = <%=HistoryEnabled.ToString().ToLower() %>;

        var HGI = <%=HGI.ToString().ToLower() %>;

        var CategoryList = <%=CategoryList %>;

        //if(DefaultOrganizationHierarchyFleetName.split(',').length > 1 || DefaultOrganizationHierarchyNodeCode.split(',').length > 1)
        // DefaultOrganizationHierarchyFleetName = ResMultipleHierarchy;
     
        var currentMapFrame='';

        var vehinterval ='<%=sn.User.GeneralRefreshFrequency %>';
        var PositionExpiredTime= '<%=sn.User.PositionExpiredTime %>';
        var DefaultFleetID='<%=sn.User.DefaultFleet %>';  
        var DefaultFleetName = '<%=DefaultFleetName %>';  
         
        var SelectedFleetId = DefaultFleetID;
        var SelectedFleetName = DefaultFleetName; 
        //var mapReloadedFromSearch='<%=sn.Map.ReloadMap%>';

        var IsSyncOn = false;
        var allVehicles;
        var allHistories;
        var windowsUpdating = false;
        var allTrackerWindows = new Array();
        var wincounter = 0;
        var openMapURL="./OpenLayerMap.aspx?WinId=";

        if (vehinterval > 0) {
            IsSyncOn = true;
        }

        function GetWinInitialTrackData(winID) {
            if (allTrackerWindows[winID])
                return allTrackerWindows[winID].winData;
            else
                return "";
        }

        function GetWinTrackData(vehicleID) {
            var result;
            var j = 0;             
            for (j = 0; j < allVehicles.length; j++) {
                if (allVehicles[j].BoxId == vehicleID) {
                    //.id == vehicleID) {
                    result=allVehicles[j];
                    break;
                }
            }
            return result;
        }

        function GetTelogisMapData() {
            /*while (true) {
                if (allVehicles) {
                    if (allVehicles != "" && allVehicles != null) {
                        break;
                    }
                }
            }*/
            return allVehicles;
        }

        function SetWinTrackData(data) {
            winTracker = new Object();
            winTracker.winId = "Tracker:" + wincounter;
            winTracker.winData = data;
            allTrackerWindows[wincounter] = winTracker;
            NewWindow(openMapURL,wincounter);
            wincounter++;
        }

        //         function updateVehiclePosition(VehIds)
        //         {
        //             alert("You cliecked " + VehIds);
        //         }
        function SetWinTrackData2(data) {
            winTracker = new Object();
            winTracker.winId = "Tracker:" + wincounter;
            winTracker.winData = data;
            allTrackerWindows[wincounter] = winTracker;
            //NewWindow(openMapURL,wincounter);
            wincounter++;
        }

        function searchwindow(mapframe) {
            currentMapFrame=mapframe;
            var mypage='./Map/mapSearch.aspx'
            var myname='Search';
            var w=840;
            var h=500;
            var winl = (screen.width - w) / 2; 
            var wint = (screen.height - h) / 2; 
            winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,toolbar=0,menubar=0,scrollbars=1,' 
            win = window.open(mypage, myname, winprops) 
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
        }
        //         function ZoomVehicles(mapData) {             
        //             var el = document.getElementById("mapframe");
        //             var IframeLoaded = false;
        //             var maxAttempt = 10;
        //             var currentAttempt = 0;
        //             while (!IframeLoaded && currentAttempt < maxAttempt) {
        //                 try {
        //                     currentAttempt = currentAttempt + 1;
        //                     if (el.contentWindow) {
        //                         el.contentWindow.zoomMap(mapData);
        //                     }
        //                     else if (el.contentDocument) {
        //                         el.contentDocument.zoomMap(mapData);
        //                     }
        //                     IframeLoaded = true;
        //                 } catch (err) {
        //                     IframeLoaded = false;
        //                 }
        //             }
        //         }

        function mapSelecteds(selectedBoxs,mapframe)
        {               
            var el = document.getElementById(mapframe);             
            if(el)
            {
                var IframeLoaded = false;
                var maxAttempt = 5;
                var currentAttempt = 0;
                while (!IframeLoaded && currentAttempt < maxAttempt) {
                    try {
                        currentAttempt = currentAttempt + 1;
                        if (el.contentWindow) {
                            el.contentWindow.zoomMap(selectedBoxs);
                        }
                        else if (el.contentDocument) {
                            el.contentDocument.zoomMap(selectedBoxs);
                        }
                        IframeLoaded = true;
                    } catch (err) {
                        IframeLoaded = false;
                    }
                }  
            }      
        }

        function mapSelectedVehIDs(vehID)
        {             
            var vehIDs = [];
            vehID=new String(vehID);
            //var vehIDs=selectedVehIDs.split(',');                
            try
            {
                var boxids=new Array(); 
                if(vehID.indexOf(',')>0)
                {
                    vehIDs=vehID.split(',');
                    selectSelectedVehIDs(vehIDs);
                    var i = 0, j = 0;
                    for (i = 0; i < vehIDs.length; i++) {
                        for (j = 0; j < allVehicles.length; j++) {
                            if (allVehicles[j].VehicleId == vehIDs[i]) { 
                                boxids.push(allVehicles[j]);
                                break;
                            }
                        }
                    }
                }
                else
                {  
                    vehIDs.push(vehID);
                    selectSelectedVehIDs(vehIDs);       
                    var i = 0, j = 0;
                    
                    for (j = 0; j < allVehicles.length; j++) {
                        if (allVehicles[j].VehicleId == vehID) { 
                            boxids.push(allVehicles[j]);
                            break;
                        }
                    }
                    
                }
                if(currentMapFrame!='')
                {
                    mapSelecteds(boxids,currentMapFrame);
                }
                else
                {
                    mapSelecteds(boxids,'nmapframe');
                }
            }
            catch(err)
            {
                console.log("Error: " + err.message);
            }
        }

        function selectSelectedVehIDs(vehIDs) {
            try {
                mapLoading = true;

                for (i = 0; i < vehIDs.length; i++) {
                    var gridindex = 0;
                    vehiclegrid.getStore().each(function (record) {                            
                        if (record.data.VehicleId == vehIDs[i]) {                                
                            if (!vehiclegrid.getSelectionModel().isSelected(gridindex))
                                vehiclegrid.getSelectionModel().select(gridindex, true, false);                               
                        }
                        gridindex++;

                    });
                }
                   
                mapLoading = false;
            }
            catch (err) {
                alert(err);
            }               
        }

        function MapAlarm(vehicles)
        {
            var el = document.getElementById("nmapframe");
            var IframeLoaded = false;
            var maxAttempt = 5;
            var currentAttempt = 0;
            while (!IframeLoaded && currentAttempt < maxAttempt) {
                try {
                    currentAttempt = currentAttempt + 1;
                    if (el.contentWindow) {
                        el.contentWindow.zoomMapAlarms(vehicles);
                    }
                    else if (el.contentDocument) {
                        el.contentDocument.zoomMapAlarms(vehicles);
                    }
                    IframeLoaded = true;
                } catch (err) {
                    //console.log("Error in mapping alarms " + err.message);
                    IframeLoaded = false;
                }
            }

            el = document.getElementById("smapframe");
            IframeLoaded = false;
            currentAttempt = 0;
            while (!IframeLoaded && currentAttempt < maxAttempt) {
                try {
                    currentAttempt = currentAttempt + 1;
                    if (el.contentWindow) {
                        el.contentWindow.zoomMapAlarms(vehicles);
                    }
                    else if (el.contentDocument) {
                        el.contentDocument.zoomMapAlarms(vehicles);
                    }
                    IframeLoaded = true;
                } catch (err) {
                    //console.log("Error in mapping alarms " + err.message);
                    IframeLoaded = false;
                }
            }

            el = document.getElementById("emapframe");
            IframeLoaded = false;
            currentAttempt = 0;
            while (!IframeLoaded && currentAttempt < maxAttempt) {
                try {
                    currentAttempt = currentAttempt + 1;
                    if (el.contentWindow) {
                        el.contentWindow.zoomMapAlarms(vehicles);
                    }
                    else if (el.contentDocument) {
                        el.contentDocument.zoomMapAlarms(vehicles);
                    }
                    IframeLoaded = true;
                } catch (err) {
                    //console.log("Error in mapping alarms " + err.message);
                    IframeLoaded = false;
                }
            }
        }

        function ShowMapFrameData(mapData, isInitialMapRequest,mapframe, zoomVehicles) {        
            zoomVehicles = typeof zoomVehicles !== 'undefined' ? zoomVehicles : true;
            
            if (isInitialMapRequest) {
                allVehicles = mapData;
            }
            else {
                var i = 0, j = 0;
                for (i = 0; i < mapData.length; i++) {
                    for (j = 0; j < allVehicles.length; j++) {
                        if (allVehicles[j].BoxId == mapData[i].BoxId) {
                            //(allVehicles[j].id == mapData[i].id) {
                            allVehicles[j] = mapData[i];                             
                            break;
                        }
                    }
                }
            }
            var el = document.getElementById(mapframe);
            var IframeLoaded = false;
            var maxAttempt = 5;
            var currentAttempt = 0;
            while (!IframeLoaded && currentAttempt < maxAttempt) {
                try {
                    currentAttempt = currentAttempt + 1;
                    if (el.contentWindow) {
                        if (isInitialMapRequest) {
                            el.contentWindow.ShowMultipleAssets(mapData, zoomVehicles);                                     
                        }
                        else {
                            el.contentWindow.UpdateMultipleAssets(mapData);
                        }
                    }
                    else if (el.contentDocument) {
                        if (isInitialMapRequest) {
                            el.contentDocument.ShowMultipleAssets(mapData, zoomVehicles);                                  
                        }
                        else {
                            el.contentDocument.UpdateMultipleAssets(mapData);
                        }
                    }
                    IframeLoaded = true;
                } catch (err) {
                    IframeLoaded = false;
                }
            }
             
        }

        function ShowHistoryMapFrameData(mapData,isInitialMapRequest,mapframe, zoomHistories) {        
            zoomHistories = typeof zoomHistories !== 'undefined' ? zoomHistories : true; 
             
            allHistories = mapData;
             
            var el = document.getElementById(mapframe);
            var IframeLoaded = false;
            var maxAttempt = 5;
            var currentAttempt = 0;
            while (!IframeLoaded && currentAttempt < maxAttempt) {
                try {
                    currentAttempt = currentAttempt + 1;
                    if (el.contentWindow) {
                        if (isInitialMapRequest) {
                            el.contentWindow.ShowHistories(mapData, zoomHistories);                                     
                        }
                        else {
                            el.contentWindow.UpdateHistories(mapData);
                        }
                        el.contentWindow.removeAllPopups();
                    }
                    else if (el.contentDocument) {
                        if (isInitialMapRequest) {
                            el.contentDocument.ShowHistories(mapData, zoomHistories);                                  
                        }
                        else {
                            el.contentDocument.UpdateHistories(mapData);
                        }
                        el.contentDocument.removeAllPopups();
                    }
                    IframeLoaded = true;
                } catch (err) {
                    IframeLoaded = false;
                }
            }
             
        }



        function removeMapWindow(winID) {
            try {
                allTrackerWindows.splice(winID, 1);
            } catch (err) {
            }
        }

        function NewWindow(url,data) {
            var mypage = url + data;
            win = window.open(mypage)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function NewAlarmWindow(AlarmId) {
            var target = "<%=alarmDetailPage%>";
            var mypage = target + '?AlarmId=' + AlarmId;                    
            var myname='AlarmInfo';
            var w=<%=windowWidth%>;
               var h=<%=windowHeight%>;
            var winl = (screen.width - w) / 2; 
            var wint = (screen.height - h) / 2; 
            winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
            win = window.open(mypage, myname, winprops) 
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function NewMessageWindow(MsgKey) {
            var target = "<%=messageDetailPage%>";
               var mypage = target + '?MsgKey=' + MsgKey;                    
               var myname='MessageInfo';
               var w=426;
               var h=400;
               var winl = (screen.width - w) / 2; 
               var wint = (screen.height - h) / 2; 
               winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
               win = window.open(mypage, myname, winprops) 
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
           }

           function SensorInfoWindow(LicensePlate) {
               var mypage='./Map/frmSensorMain.aspx?LicensePlate='+LicensePlate
               var myname='Sensors';
               var w=525;
               var h=720;
               var winl = (screen.width - w) / 2; 
               var wint = (screen.height - h) / 2; 
               winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
               win = window.open(mypage, myname, winprops) 
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
           }

           function RouteWindow(vehicleDescription) {
               var mypage='./ServiceAssignment/AssignmentForm.aspx?objectname=' + encodeURIComponent(vehicleDescription.replace(/&singlequote;/g, "\'")) + '&routeName=&service=route';
               //var mypage='http://preprod.sentinelfm.com/ServiceAssignment/AssignmentForm.aspx?vehicleName=' + encodeURIComponent(vehicleDescription.replace(/&singlequote;/g, "\'")) + '&routeName=';
               var myname='Route';
               var w=825;
               var h=520;
               var winl = (screen.width - w) / 2; 
               var wint = (screen.height - h) / 2; 
               winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
               win = window.open(mypage, myname, winprops) 
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
           }
           function removeMarkersOnMap()
           {
               var el = document.getElementById("mapframe");
               var IframeLoaded = false;
               var maxAttempt = 5;
               var currentAttempt = 0;
               while (!IframeLoaded && currentAttempt < maxAttempt) {
                   try {
                       currentAttempt = currentAttempt + 1;
                       if (el.contentWindow) {                                
                           el.contentWindow.removeMarkersOnMap();                                
                       }
                       else if (el.contentDocument) {
                           el.contentDocument.removeMarkersOnMap();                             
                       }
                       IframeLoaded = true;
                   } catch (err) {
                       IframeLoaded = false;
                   }
               }
           }

           function removeHistoriesOnMap(mf)
           {
               var el = document.getElementById(mf);
               var IframeLoaded = false;
               var maxAttempt = 5;
               var currentAttempt = 0;
               while (!IframeLoaded && currentAttempt < maxAttempt) {
                   try {
                       currentAttempt = currentAttempt + 1;
                       if (el.contentWindow) {                                
                           el.contentWindow.removeHistoriesOnMap(); 
                           el.contentWindow.removeAllPopups();
                       }
                       else if (el.contentDocument) {
                           el.contentDocument.removeHistoriesOnMap();
                           el.contentDocument.removeAllPopups();
                       }
                       $('#historiescount').html('0');
                       IframeLoaded = true;
                   } catch (err) {
                       IframeLoaded = false;
                   }
               }
           }
           
           var geozoneCurrentAssignmentWin;
           function openNewWindow(wintitle, winURL, winWidth, winHeight) {
               win = new Ext.Window(
                 {
                     title: wintitle,
                     width: winWidth,
                     height: winHeight,
                     layout: 'fit',
                     maxWidth: window.screen.width,
                     maxHeight: window.screen.height,
                     maximizable: 'true',
                     minimizable: 'true',
                     resizable: 'true',
                     closable: true,
                     border: false,
                     html: winURL
                 }
                 );

               win.show();        
           }

           function blinks(hide) {
               if(hide==1) {
                   $('.blinking').show();
                   hide = 0;
               }
               else { 
                   $('.blinking').hide();
                   hide = 1;
               }
               setTimeout("blinks("+hide+")",1000);
           }

           function GetGeozoneCurrentAssignment(geozoneId)
           {
               if(geozoneCurrentAssignmentWin)
                   geozoneCurrentAssignmentWin.close();

               var url = "./GeoZone_Landmarks/frmViewVehicleGeozones.aspx?geozoneId=" + geozoneId;
               var urlToLoad = '<iframe id="geozoneCurrentAssignmentWin" name="geozoneCurrentAssignmentWin" width="100%" height="100%" frameborder="0" scrolling="yes" src="' + url + '"></iframe>';
               
               geozoneCurrentAssignmentWin = new Ext.Window(
                 {
                     title: 'Current Assignment',
                     width: 500,
                     height: 320,
                     layout: 'fit',
                     maxWidth: window.screen.width,
                     maxHeight: window.screen.height,
                     maximizable: 'true',
                     minimizable: 'true',
                     resizable: 'true',
                     closable: true,
                     border: false,
                     html: urlToLoad
                 }
                 );

               geozoneCurrentAssignmentWin.show();
           }

           function redrawVehicleMarkers(){
               var el = document.getElementById(mapframe).contentWindow;

               el.markers.removeAllFeatures();
               if (el.parent.VehicleClustering) {
                   el.markerstrategy.clearCache();
               }
               el.markers.addFeatures(el.vehicleFeatures);
           }

           function redrawHistoryVehicleMarkers(){
               var el = document.getElementById(mapframe).contentWindow;

               el.ShowHistoryVehicleIcons = !el.ShowHistoryVehicleIcons;
               el.histories.removeAllFeatures();
        
               el.ShowHistories(el.AllHistoryRecords, false);
           }

           function replayHistoryVehicleMarkers(currentIdex){
               if(currentIdex == undefined) currentIdex = -1;
               var el = document.getElementById(mapframe).contentWindow;

               el.HistoryPlayPaused = HistoryPlayPaused;

               if(currentIdex>0)
               {
                   el.clearHistoryReplayTimeout();
                   el.CurrentHistoryPlayIndex = currentIdex - 1;
                   el.ReplayHistories(el.AllHistoryRecords);            
               }
               else
               {
                   if(!HistoryPlayPaused)
                   {
                       el.ReplayHistories(el.AllHistoryRecords);                
                   }
                   else
                       el.clearHistoryReplayTimeout();
               }
           }

           function setHistoryReplayDelayTime(x)
           {
               var el = document.getElementById(mapframe).contentWindow;
               el.HistoryReplayDelayTime = el.HistoryReplayDelayBaseTime * x;
           }

           function adjustHistoryReplayDelayTime(x)
           {
               var el = document.getElementById(mapframe).contentWindow;
               if(x > 0 && el.HistoryReplayDelayTime + x <= 1000)
               {
                   el.HistoryReplayDelayTime = el.HistoryReplayDelayTime + x;
                   txtHistoryReplayDelayTime.setValue(el.HistoryReplayDelayTime + 'ms');
               }
               else if(x < 0 && el.HistoryReplayDelayTime + x >= 50)
               {
                   el.HistoryReplayDelayTime = el.HistoryReplayDelayTime + x;
                   txtHistoryReplayDelayTime.setValue(el.HistoryReplayDelayTime + 'ms');
               }
           }

           function ResetHistoryReplay(){
               var el = document.getElementById(mapframe).contentWindow;
               el.HistoryPlayPaused = true;
               el.CurrentHistoryPlayIndex = 0;
               btnHistoryReplay.setText(Res_HistoryReplayReplay); //'Replay'
               ifShowVehicleIcon = OriginIfShowVehicleIcon;
               el.ShowHistoryVehicleIcons = ifShowVehicleIcon;

               historyReplaySlider.setValue(1, false);
               replayPanel.hide();
               btnHistoryReplay.setDisabled(false);

               if (ifShowVehicleIcon) {
                   btnVehicleonoff.setText(ResbtnVehicleonoffHideDetailsText/*'Hide Details'*/);            
               }
               else {
                   btnVehicleonoff.setText(ResbtnVehicleonoffShowDetailsText/*'Show Details'*/);
               }

               //Reset History Graph as well
               historyGraphWindow.hide();
               dataHistoryGraphSpeed = [];
               dataHistoryGraphRPM = [];
               dataHistoryGraphRoadSpeed = [];
           }

           function GetHistoryCurrentDateTime(currentIndex)
           {
               var el = document.getElementById(mapframe).contentWindow;
               var currentHistory = el.AllHistoryRecords[el.AllHistoryRecords.length - currentIndex];
               return currentHistory.OriginDateTime;
           }

           function GetHistoryGraphData()
           {
               var _d = [];
               var el = document.getElementById(mapframe).contentWindow;
               for (i = el.AllHistoryRecords.length - 1; i >= 0; i--) {
                   var h = el.AllHistoryRecords[i];
                   var speed = h.Speed;
                   if (speed >= 0)
                       speed = speed * 1;
                   else
                       speed = 0;
                   var p = [h.convertedDisplayDate, speed];
                   _d.push(p);
               }
               return _d;
           }

           function GetHistoryGraphRPM()
           {
               var _d = [];
               var el = document.getElementById(mapframe).contentWindow;
               for (i = el.AllHistoryRecords.length - 1; i >= 0; i--) {
                   var h = el.AllHistoryRecords[i];
                   var rpm = h.RPM;
                   if (rpm >= 0)
                       rpm = rpm * 1;
                   else
                       rpm = 0;
                   var p = [h.convertedDisplayDate, rpm];
                   _d.push(p);
               }
               return _d;
           }

           function GetHistoryGraphRoadSpeed()
           {
               var _d = [];
               var el = document.getElementById(mapframe).contentWindow;
               for (i = el.AllHistoryRecords.length - 1; i >= 0; i--) {
                   var h = el.AllHistoryRecords[i];
                   var roadSpeed = h.RoadSpeed;
                   if (roadSpeed >= 0)
                       roadSpeed = roadSpeed * 1;
                   else
                       roadSpeed = 0;
                   var p = [h.convertedDisplayDate, roadSpeed];
                   _d.push(p);
               }
               return _d;
           }

           function SetHistoryGraphHighlight(ic)
           {
               if(dataHistoryGraphSpeed.length > ic)
               {
                   var axes = $('#divHistoryGraph').jqChart('option', 'axes');

                   if (axes[0].visibleMaximum != undefined) {
                       var m = 0;
                       var p = axes[0].visibleMaximum - axes[0].visibleMinimum;
                       if(axes[0].visibleMaximum < ic)
                       {
                           //m = dataHistoryGraphSpeed.length - ic  >= p ? p : dataHistoryGraphSpeed.length - ic;
                           m = dataHistoryGraphSpeed.length - axes[0].visibleMaximum  >= p ? Math.ceil((ic - axes[0].visibleMaximum)/p) * p : dataHistoryGraphSpeed.length - axes[0].visibleMaximum;
                           axes[0].visibleMaximum = axes[0].visibleMaximum + m;
                           axes[0].visibleMinimum = axes[0].visibleMinimum + m;
                           $('#divHistoryGraph').jqChart('update');
                       }                
                       else if(axes[0].visibleMinimum > ic)
                       {
                           m = axes[0].visibleMinimum - ic;
                           axes[0].visibleMaximum = axes[0].visibleMaximum - m;
                           axes[0].visibleMinimum = axes[0].visibleMinimum - m;
                           $('#divHistoryGraph').jqChart('update');
                       }
                
                   }

                   $('#divHistoryGraph').jqChart('highlightData', [dataHistoryGraphSpeed[ic], dataHistoryGraphRPM[ic], dataHistoryGraphRoadSpeed[ic]]);
               }
           }

           var hasVehiclePopover = false;

           var IfClosevehicleInfopopover = true;
           function closevehicleInfopopover(e)
           {
               if(IfClosevehicleInfopopover && e.attr('data-popup') == 'false')
               {
                   e.popover('hide');
               }
           }
           function closeAllVehicleInfoPopover()
           {
               $('#vehicleInfoPopover .popover').remove();
           }
           function bindVehicleInfoPopover()
           {
               $('#vehicleInfoPopover .popover').bind({
                   'mouseenter':function(el) {    
                       IfClosevehicleInfopopover = false;                                    
                   },
                   'mouseleave':function(el) {    
                       $(this).closest(".popover").remove();    
                   }
               });
           }

           function iniVehicleGridPopup() {
               return;//we disable this feature for Chrome will flash. Chrome has this bug when bootstrap's popover is in frame's frame
               $('[rel=bootstrapvehiclegridpopover]').bind({
                   'mouseenter':function(el) {        
                       //alert(el.clientY + ', ' + $(document).height());
                       IfClosevehicleInfopopover = false;
                
                       var clientY = el.clientY;
                       var documentHeight = $(document).height();
                       var placement = 'right';
                       if(clientY < documentHeight / 3)
                           placement = 'bottom';
                       else if(clientY > documentHeight * 2 / 3)
                           placement = 'top';

                       var e=$(this);
                       //alert(hasVehiclePopover);

                       closeAllVehicleInfoPopover();
                
                       if(e.attr('data-popup') == 'true')
                           return;

                       e.attr('data-popup', 'true');
                
                       if(e.attr('data-content') == undefined || e.attr('data-content') == 'loading...')
                       {
                           e.popover({
                               content: "loading...",
                               placement: placement
                           }).popover('show');
                           $.get(e.data('poload'),function(d) {
                               e.attr('data-content', d);
                               //e.attr('data-title', title);
                        
                               e.popover('destroy');

                               if(e.attr('data-popup') == 'false')
                                   return;

                               e.popover({
                                   //title: title,
                                   content: e.attr('data-content'),
                                   placement: placement
                               }).popover('show');
                               //e.popover('show');

                               bindVehicleInfoPopover();
                    
                           });
                       }
                       else
                       {
                           e.popover({
                               //title: title,
                               content: e.attr('data-content'),
                               placement: placement
                           }).popover('show');
                           //e.popover('show');

                           bindVehicleInfoPopover();
                       }

                
                   },
                   'mouseleave': function(e) {
                       IfClosevehicleInfopopover = true;
                       var e=$(this);
                       e.attr('data-popup', 'false');
                       setTimeout(function(){ closevehicleInfopopover(e);}, 300);
                       //e.popover('hide');
                                
                   }
               });
               
           }

           function iniBootstrapHoverPopover() {
               return;//we disable this feature for Chrome will flash. Chrome has this bug when bootstrap's popover is in frame's frame
               $('[rel=bootstrapHoverPopover]').popover({ 
                   trigger: "hover"
                  ,placement: function(pop, el) {
                      var placement = 'top';

                      try {
                          var clientY = $(el).offset().top;
                          var documentHeight = $(document).height();
    
                          if(clientY < documentHeight / 2)
                              placement = 'bottom';
                    
                      }
                      catch(err){}
    
                      return placement;
                  },
               });         
           }

           function closebootstrappopover(o)
           {
               var popover = $(o).parent().parent().remove();
           }

           function VideoViewer(boxId, dt) {
               var mypage='./Widgets/VideoViewer.aspx?boxid='+boxId + '&dt=' + dt;
               var myname='VideoViewer';
               var w=550;
               var h=420;
               var winl = (screen.width - w) / 2; 
               var wint = (screen.height - h) / 2; 
               winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
               win = window.open(mypage, myname, winprops) 
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
           }

           function GotoMapPosition(lat, lon){
               var el = document.getElementById(mapframe);
               var IframeLoaded = false;
               var maxAttempt = 5;
               var currentAttempt = 0;
               while (!IframeLoaded && currentAttempt < maxAttempt) {
                   try {
                       currentAttempt = currentAttempt + 1;
                       if (el.contentWindow) {                                
                           el.contentWindow.GotoMapPosition(lat, lon);                             
                       }
                       else if (el.contentDocument) {
                           el.contentDocument.GotoMapPosition(lat, lon);
                       }
                       IframeLoaded = true;
                   } catch (err) {
                       IframeLoaded = false;
                   }
               }
           }

    </script>
    <script src="Scripts/NewMap/VehicleList.js?v=<%=LastUpdatedVehicleListJs %>" type="text/javascript"></script>

    <script type="text/javascript">

        blinks(1);

        function MapRoute(routeId, serviceConfigId) {
         
            var el = document.getElementById(mapframe);
            var IframeLoaded = false;
            var maxAttempt = 5;
            var currentAttempt = 0;
            while (!IframeLoaded && currentAttempt < maxAttempt) {
                try {
                    currentAttempt = currentAttempt + 1;
                    if (el.contentWindow) {

                        el.contentWindow.mapRoute(routeId, serviceConfigId);
                    }
                    else if (el.contentDocument) {

                        el.contentDocument.mapRoute(routeId, serviceConfigId);
                    }
                    IframeLoaded = true;
                } catch (err) {
                    IframeLoaded = false;
                }
            }   
        
        }

    </script>



    <!-- Google Tag Manager -->
    <noscript>
        <iframe src="//www.googletagmanager.com/ns.html?id=GTM-K49R9G"
            height="0" width="0" style="display: none; visibility: hidden"></iframe>
    </noscript>
    <script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
})(window,document,'script','dataLayer','GTM-K49R9G');</script>
    <!-- End Google Tag Manager -->

</body>
</html>
