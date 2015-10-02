<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReports_new.aspx.cs" Inherits="SentinelFM.Reports_frmReports_new"
    Culture="en-US" UICulture="auto" %>

<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register src="UserControl/frmRepository.ascx" tagname="frmRepository" tagprefix="uc1" %>
<%@ Register src="UserControl/ViewReport.ascx" tagname="ViewReport" tagprefix="uc2" %>
<%@ Register src="~/UserControl/DriverSearch.ascx" tagname="DriverSearch" tagprefix="ucDriverSearch" %>
<%@ Register Src="~/Reports/UserControl/frmScheduleReportList.ascx" TagPrefix="uc1" TagName="frmScheduleReportList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css">
    <link rel="stylesheet" href="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
	<link href="jqueryFileTree.css?v=20140220" rel="stylesheet" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../Styles/SentinelFM/css/simplePagination.css" />

    <style type="text/css">
        .style1
        {
            width: 45px;
        }
        
        
        .style2
        {
            width: 120px;
        }
        .style3
        {
            width: 119px;
        }
        .style4
        {
            width: 28px;
        }
        .style5
        {
            height: 24px;
            width: 28px;
        }
        .style6
        {
            height: 14px;
            width: 28px;
        }
        .style7
        {
            height: 25px;
            width: 28px;
        }
        .style8
        {
            height: 9px;
            width: 28px;
        }
    </style>
    

    <style type="text/css">
        .loading
        {
            background-color: #dff3ff;
            border: 1px solid #c6e1f2;
        }
       
        
    </style>

       <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
       <%--<script src="../scripts/jquery.simplePagination.js" type="text/javascript"></script>--%>
       
</head>
<body>
    <script id="localScript" language="javascript" type="text/javascript">
	<!--

        var browserTypes = { "MSIE8": 0, "MSIE7": 1, "MSIE6": 2, "Gecko": 3, "WebKit": 4 };
        var browserType = browserTypes.MSIE;


        var OrganizationHierarchyPath = "";
        var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        var PreferOrganizationHierarchyNodeCode = "<%=PreferOrganizationHierarchyNodeCode %>";
        var IniHierarchyPath = <%=IniHierarchyPath.ToString().ToLower() %>;
        var rex = /AppleWebKit\/\d{1,3}\.\d{1,3}/
        var match = navigator.userAgent.match(rex);
        if (match)
            browserType = browserTypes.WebKit;

        rex = /Gecko\/\d*\s/
        match = navigator.userAgent.match(rex);
        if (match)
            browserType = browserTypes.Gecko;


        rex = /MSIE\s*\d{1,2}\.\d{1,3};/
        match = navigator.userAgent.match(rex);
        if (match && match[0]) {
            if (parseInt(match[0].split(' ')[1]) === 8) browserType = browserTypes.MSIE8;
            if (parseInt(match[0].split(' ')[1]) === 7) browserType = browserTypes.MSIE7;
            if (parseInt(match[0].split(' ')[1]) === 6) browserType = browserTypes.MSIE6;
        }


        function CalendarView(hideFields) {

            if (hideFields) {
                document.getElementById("cboHoursFrom").style.display = "none";
                document.getElementById("cboHoursTo").style.display = "none";
                document.getElementById("cboVehicle").style.display = "none";
                document.getElementById("cboFleet").style.display = "none";
                document.getElementById("cboVehicle").style.display = "none";
                document.getElementById("cboFormat").style.display = "none";
            }
            else {

                document.getElementById("cboHoursFrom").style.display = "inline-block";
                document.getElementById("cboHoursTo").style.display = "inline-block";
                document.getElementById("cboVehicle").style.display = "inline-block";
                document.getElementById("cboFleet").style.display = "inline-block";
                document.getElementById("cboVehicle").style.display = "inline-block";
                document.getElementById("cboFormat").style.display = "inline-block";
            }

        };

        function calendarClick(hideFields) {

            switch (browserType) {
                /*
                case browserTypes.Gecko:
                break;
                case browserTypes.WebKit:
                Element.setStyle($("obcall"), { left: 87 + "px", top: -22 + "px" });
                break;
                case browserTypes.MSIE7:
                Element.setStyle($("obcall"), { left: 87 + "px", top: -21 + "px" });
                break;
                */ 
                case browserTypes.MSIE6:
                    CalendarView(hideFields);
                    break;
            }


        }

        <%if(ShowOrganizationHierarchy && (trMaintenanceHierarchy.Visible || organizationHierarchy.Visible)) { %>

        function inifiletree(inipath, appendpath) {
            if(appendpath == undefined)
            {
                appendpath = false;
            }
            var selectedFolders = '';
            if(MutipleUserHierarchyAssignment && (IniHierarchyPath || appendpath))
            {
                selectedFolders = $('#OrganizationHierarchyNodeCode').val();
                if(appendpath)
                {
                    var x_ps = inipath.split('/');
                    selectedFolders = selectedFolders + ',' + x_ps[x_ps.length-1];
                }
            }
            $('#vehicletreeview').fileTree({ root: PreferOrganizationHierarchyNodeCode, script: 'vehicleListTree.asmx/FetchVehicleList', expanded: inipath, expandSpeed: 200, collapseSpeed: 200, vehicledetails: 'vehicledetails'
                                            , highlightVehicleSelection: <%=OrganizationHierarchySelectVehicle.ToString().ToLower() %>
                                            , MutipleUserHierarchyAssignment: MutipleUserHierarchyAssignment
                                            , PreferOrganizationHierarchyNodeCode: PreferOrganizationHierarchyNodeCode
                                            , scriptForPreferNodecodes: 'vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                                            , FullTreeView: false 
                                            , VehicleListPagingBarId: 'vehiclelistPageBar'
                                            , scriptForFetchVehicleByPage: 'vehicleListTree.asmx/FetchVehicleListFilterByPage'
                                            , VehiclePageSize: <%=VehiclePageSize %>
                                            , SelectedFolders: selectedFolders//IniHierarchyPath ? $('#OrganizationHierarchyNodeCode').val() : ''
                                            , SelectedFleetIds: (MutipleUserHierarchyAssignment && appendpath) ? $('#OrganizationHierarchyFleetId').val() : ''
                                           },
            /*
            * Call back function when you click left pane tree folder.
            */
                        function (NodeCode, FleetId) {
                            if(!MutipleUserHierarchyAssignment)
                            {
                                $('#OrganizationHierarchyNodeCode').val(NodeCode);
                                $('#OrganizationHierarchyFleetId').val(FleetId);
                                $('#OrganizationHierarchyBoxId').val('');
                                $('#OrganizationHierarchyVehicleDescription').val('');
                            }
                        },

            /*
            * Call back function when you click right pane vehicle list.
            */
                        function (BoxId, vehicleDescription) {
                            //alert('BoxId: ' + BoxId);
                            $('#OrganizationHierarchyBoxId').val(BoxId);
                            $('#OrganizationHierarchyVehicleDescription').val(vehicleDescription);
                        },

                        function (selectedNodecodes, selectedFleetIds, fleetName)
                        {   
//                            selectedOrganizationHierarchyNodeCode = selectedNodecodes;
//                            selectedOrganizationHierarchyFleetId = selectedFleetIds;
//                            selectedOrganizationHierarchyFleetName = fleetName;
                            $('#OrganizationHierarchyNodeCode').val(selectedNodecodes);
                            $('#OrganizationHierarchyFleetId').val(selectedFleetIds);
                            //$('#OrganizationHierarchyBoxId').val('');
                            //$('#OrganizationHierarchyVehicleDescription').val('');                            
                        }
                    );
        }
        $(document).ready(function () {
            try {
                $("#MySplitter").splitter({
                    type: "v",
                    outline: true,
                    minLeft: 100, sizeLeft: 390, minRight: 100,
                    resizeToWidth: true,
                    //cookie: "vsplitter13",
                    accessKey: 'I'
                });

                inifiletree(OrganizationHierarchyPath);

                /*$('#vehiclelisttbl').dataTable({
                "bPaginate": false,
                "bLengthChange": false,
                "bFilter": false,
                "bInfo": false
                });*/
                //$('#vehiclelisttbl').tablesorter();
                //$('#vehiclelisttbl').colResizable({ headerOnly: true });
            }
            catch (ex) { }
        });
        
	    <% } %>	
			//-->

        $(document).ready(function () {
            // Clean driver entry box if All Driver checked
            $("#<%= chkAllDrivers.ClientID %>").change(function() {
                if(this.checked){
                    $("#<%= ucReportDriver.Input_DriverName %>").val("");
                    $("#<%= ucMantainDrivers.Input_DriverName %>").val("");
                }
            })
            // Uncheck All Driver box if Driver Box has keypressed 
            $('#<%= ucReportDriver.Input_DriverName %>').keypress(function(){
                if ($("#<%= chkAllDrivers.ClientID %>") != null){
                    $("#<%= chkAllDrivers.ClientID %>").attr('checked', false);  
                }
            });
            // Uncheck All Driver Checkbox if Driver Box has typed
            $('#<%= ucMantainDrivers.Input_DriverName %>').keypress(function(){
                if ($("#<%= chkAllDrivers.ClientID %>") != null){
                    $("#<%= chkAllDrivers.ClientID %>").attr('checked', false);  
                }
            });

        });
    </script>
    <script type="text/javascript">
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
    </script>


    <script language="javascript">
	    <!--
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
		    var myname='OrganizationHierarchy';
		    var w=740;
		    var h=440;
		    var winl = (screen.width - w) / 2; 
		    var wint = (screen.height - h) / 2; 
		    winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
		    win = window.open(mypage, myname, winprops) 
		    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }  
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName)
        {            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyFleetName.ClientID %>').val(fleetName);
            $('#<%=btnOrganizationHierarchyNodeCode.ClientID %>').val(fleetName);
            
        }
            
		    //-->
    </script>

    <div style="height: 100%">
        <form id="frmReports_new" runat="server" method="post">

        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetName" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            <script type="text/javascript">

                function onTabSelected(sender, args) {
                    if (args.get_tab().get_value() == "2") {
                        RefreshPage_ViewReport();
                    }
                    if (args.get_tab().get_value() == "1") rebindGrid(false);
                    if (args.get_tab().get_value() == "3") frmScheduleReportListToolBar();
                    //Devin Added
                    if (args.get_tab().get_value() == "0") {
                        //user click create
                        if ($telerik.$('#<%= hidAfterCreate.ClientID %>').val() == "1") {
                            $telerik.$('#<%= hidAfterCreate.ClientID %>').val("");
                            $telerik.$('#vehicledetails').html('');
                            $telerik.$('#<%= btnAfterCreate.ClientID%>').click();
                        }
                    }
   

                }

                var hideButtonID = '';
                function OpenCreateWindow(validationGroup, hideButton) {
                    var cboReports = $find("<%= cboReports.ClientID %>");
                    var selectedReport = cboReports.get_value();
                    
                    hideButtonID = ''
                    $telerik.$('#<%= hidSubmitType.ClientID%>').val('');
                    $telerik.$('#<%= lblMessage.ClientID %>').text('');
                    if (validationGroup != null && !Page_ClientValidate(validationGroup)) {
                        Page_BlockSubmit = false;
                        return false; //not valid return false
                    }
                    var oWnd = window.radopen(null, "UserListDialog");
                    var url = "../ReportsScheduling/frmReportScheduler_new.aspx?rnd=" + Math.random();
                    //User Logins Report and Hours of Service Audit Report
                    if (selectedReport == "60") //hideButton == '<%//= cmdPreviewFleetMaintenanceReportHide.ClientID%>' || 
                        url = url + "&hideSchedule=1";
                    window.radopen(url, "UserListDialog");
                    hideButtonID = hideButton;
                    Page_BlockSubmit = false;

                    //Devin Added
                    $telerik.$('#<%= hidAfterCreate.ClientID %>').val("1");

                    return false;
                }

                //Parameter
                //val: 1 means one time report, 2 means schedule report, 3 means my report
                function PopupSubmit(val) {
                    $telerik.$('#<%= hidSubmitType.ClientID%>').val(val);
                    if (hideButtonID != '') {

                        $telerik.$('#' + hideButtonID).click();
                        hideButtonID = '';
                    }
                }
                function PopupLogin() {
                    if (hideButtonID != '') {
                        $telerik.$('#<%= btnLogin.ClientID %>').click();
                        hideButtonID = '';
                    }
                }


                //Custom Client Validate 
                function CustomValidateDate(sender, args) {
                    args.IsValid = true;

                    try {
                        var timePickerfrom = $find("<%= cboHoursFrom.ClientID %>");
                        var timeViewfrom = timePickerfrom.get_timeView();
                        var fromTime = timeViewfrom.getTime();
                        var datePickerfrom = $find("<%= txtFrom.ClientID %>");
                        var fromDate = datePickerfrom.get_selectedDate();
                        fromDate.setHours(fromTime.getHours());
                        fromDate.setMinutes(fromTime.getMinutes());


                        var timePickerto = $find("<%= cboHoursTo.ClientID %>");
                        var timeViewto = timePickerto.get_timeView();
                        var toTime = timeViewto.getTime();
                        var datePickerto = $find("<%= txtTo.ClientID %>");
                        var toDate = datePickerto.get_selectedDate();
                        toDate.setHours(toTime.getHours());
                        toDate.setMinutes(toTime.getMinutes());
                        if (fromDate >= toDate) {
                            sender.errormessage = "<%= errlblMessage_Text_InvalidDate%>";
                            args.IsValid = false;
                            return;
                        }
                    } catch (err) { }


                    var cboReports = $find("<%= cboReports.ClientID %>");
                    var selectedReport = cboReports.get_value();
                    if (selectedReport == "40" || selectedReport == "63") {
                        var cboVehicle = $find("<%= cboVehicle.ClientID %>");
                        if (cboVehicle != null) {
                            if (cboVehicle.get_selectedIndex() == 0) {
                                sender.errormessage = "<%= errvalSelectVehicle%>";
                                args.IsValid = false;
                                return;
                            }
                        }
                    }

                    if (selectedReport == "21" || selectedReport == "23" || selectedReport == "41" || selectedReport == "10066" || selectedReport == "10067") {
                        var ddlLandmarks = $find("<%= ddlLandmarks.ClientID %>");
                        if (ddlLandmarks != null) {
                            if (ddlLandmarks.get_selectedIndex() == 0) {
                                sender.errormessage = "<%= errvalSelectLandmark%>";
                                args.IsValid = false;
                                return;
                            }
                        }
                    }
                    
                    if (selectedReport != "19" && selectedReport != "25" && selectedReport != "38") {
                        var cboFleet = $find("<%= cboFleet.ClientID %>");
                        var ucDriverForReport = $('#<%= ucReportDriver.Input_DriverName %>');
                        var ucDriverForManten = $('#<%= ucMantainDrivers.Input_DriverName %>');
                        var chkAllDriversBox = $("#<%= chkAllDrivers.ClientID %>");

                        if (cboFleet != null) {
                            if ($(cboFleet).is(':enabled')) {
                                if (cboFleet.get_selectedIndex() == 0) {
                                    sender.errormessage = "<%= errvalFleetMessage%>";
                                    args.IsValid = false;
                                    return;
                                }
                            }
                        }
                        else if (ucDriverForManten.length > 0){
                            if ($("#<%= ucMantainDrivers.Input_DriverID %>").val() == "")
                            {
                                if (chkAllDriversBox == null)
                                {
                                    sender.errormessage = "<%= errDriverMessage%>";
                                    args.IsValid = false;
                                    return;
                                }
                            }
                        }
                        else if (ucDriverForReport.val() != null)
                        {
                            if ($("#<%= ucReportDriver.Input_DriverID %>").val() == "")
                            {
                                if (chkAllDriversBox == null) 
                                {
                                    sender.errormessage = "<%= errDriverMessage%>";
                                    args.IsValid = false;
                                    return;
                                }
                            }
                        }
                        else 
                        {
                            var vehicleSelectOption = $("#<%=vehicleSelectOption.ClientID %>");
                            if (vehicleSelectOption.html() != null && vehicleSelectOption.html() != '') {
                                if ($('#OrganizationHierarchyFleetId').val() == null || $('#OrganizationHierarchyFleetId').val() == '') {
                                    sender.errormessage = "<%= errvalHierarchyMessage%>";
                                    args.IsValid = false;
                                    return;
                                }
                            }
                        }
                    }

                    if (selectedReport == "25" || selectedReport == "26" || selectedReport == "27" || selectedReport == "28" || selectedReport == "53") {
                        var ddlDrivers = $find("<%= ddlDrivers.ClientID %>");
                        if (ddlDrivers != null) {
                            if (ddlDrivers.get_selectedIndex() == 0) {
                                sender.errormessage = "<%= errvalDriver%>";
                                args.IsValid = false;
                                return;
                            }
                        }
                    }

                    if (selectedReport == "22") {
                        var ddlGeozones = $find("<%= ddlGeozones.ClientID %>");
                        if (ddlGeozones != null) {
                            if (ddlGeozones.get_selectedIndex() == 0) {
                                sender.errormessage = "<%= errddlGeozones_Item_0%>";
                                args.IsValid = false;
                                return;
                            }
                        }
                    }

                    var cboVehicle = $find("<%= cboVehicle.ClientID %>");
                    if (cboVehicle != null) {
                        if ((cboVehicle.get_selectedIndex() == 0 && selectedReport == "3") ||
                         (cboVehicle.get_selectedIndex() == -1)) {
                            sender.errormessage = "<%= errlblMessage_Text_SelectVehicle%>";
                            args.IsValid = false;
                            return;
                        }
                    }


                    return;
                }

                //for Checkbox chkweekend click event
                function clickchkWeekend() {
                    var ctl = "'#<%= chkWeekend.ClientID %>:checked'";
                    if ($telerik.$(ctl).val() != null) {
                        var calendar = $find("<%= cboWeekEndToH.ClientID %>");
                        var dt = new Date();
                        dt.setHours(0, 0, 0, 0);

                        calendar.set_selectedDate(dt);
                        calendar.set_enabled(false);
                        var calendar = $find("<%= cboWeekEndFromH.ClientID %>");
                        calendar.set_selectedDate(dt);
                        calendar.set_enabled(false);

                    }
                    else {
                        var calendar = $find("<%= cboWeekEndToH.ClientID %>");
                        calendar.set_enabled(true);
                        var calendar = $find("<%= cboWeekEndFromH.ClientID %>");
                        calendar.set_enabled(true);

                    }

                }


                //************************************************************************************************************
                //                                    Refresh repository page
                //************************************************************************************************************
                var reportIntervalID = setInterval(reflashRepository, 20000);
                function reflashRepository() {
                    var pageView = $find("<%= RadMultiPage1.ClientID %>").findPageViewByID("Repository");
                    if (pageView.get_selected() == true) { rebindGrid(false); };

                }
                function refreshAfterSubmit() {
                    //var tabStrip = $find("<%= RadTabStrip1.ClientID %>");
                    //tabStrip.findTabByValue("0").select();
                    //setTimeout("pageLoad_repository()", 500);
                    pageLoad_repository();
                }

            </script>
        </telerik:RadCodeBlock>
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
          <Services>
                <asp:ServiceReference Path="ReportWebService.asmx" />
          </Services>
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery.cookie.js" />
                <asp:ScriptReference Path="~/scripts/jquery.simplePagination.js" />
                <asp:ScriptReference Path="~/Reports/jqueryFileTree.js?v=20140220" />
                <asp:ScriptReference Path="~/Reports/splitter.js" />
                <asp:ScriptReference Path="~/scripts/tablesorter2145/js/jquery.tablesorter.js?v=20140113" />
                <asp:ScriptReference Path="~/scripts/tablesorter2145/js/jquery.tablesorter.widgets.min.js" />
                <asp:ScriptReference Path="~/scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.js" />
                <asp:ScriptReference Path="~/scripts/colResizable-1.3.min.js" />
                <asp:ScriptReference Path="~/scripts/json2.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Transparency="0" Skin="Hay">
        </telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1">

        </telerik:RadAjaxManager>
        <script type="text/javascript">

            function onTabSelecting(sender, args) {
                if (args.get_tab().get_pageViewID()) {
                    args.get_tab().set_postBack(false);
                }
            }

            function DateSelected(sender, args) {
            }  


        </script>
        <asp:HiddenField ID="OrganizationHierarchyNodeCode" Value="" runat="server" />
        <input type="hidden" name="OrganizationHierarchyFleetId" id="OrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyBoxId" Value="" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyVehicleDescription" Value="" runat="server" />

        <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
            Skin="Hay" meta:resourcekey="RadTabStrip1Resource1" OnClientTabSelected="onTabSelected" >
            <Tabs>
                <telerik:RadTab runat="server" Text="Create Report" Value= "0" PageViewID="Report" Selected ="true" meta:resourcekey="RadTabResource1">
                </telerik:RadTab>
                <telerik:RadTab runat="server" Text="Repository" Value= "1" PageViewID="Repository" meta:resourcekey="RadTabResource2">
                </telerik:RadTab>
                <telerik:RadTab runat="server" Text="Scheduled Report" Value= "3" PageViewID="ScheduledReport" meta:resourcekey="RadTabResource4" >
                </telerik:RadTab>

                <telerik:RadTab runat="server" Text="View" Value= "2" PageViewID="View" meta:resourcekey="RadTabResource3" Enabled ="false">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0"  CssClass="multiPage" >
            <telerik:RadPageView ID="Report" runat="server" PageViewID="Report" meta:resourcekey="RadPageView1Resource1" 
                Selected="True">
                <center>
                    <table width="95%" >
                        <asp:Panel id="pnlddlReport" runat ="server" >
                        <tr align="center" style="height: 40px;">
                            <td >
                                <telerik:RadComboBox ID="ddlReport" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged"
                                    Skin="Hay">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="Standard Reports" meta:resourcekey="ddlReportItemResource1"
                                            Value="0" Selected="True"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Text="Extended Reports" Visible ="false" meta:resourcekey="ddlReportItemResource2"
                                            Value="1"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem  Text="My Reports" Visible ="true" meta:resourcekey="ddlReportItemResource3"
                                            Value="2"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem  Text="Processed Standard Reports" Visible ="true" 
                                            Value="3"></telerik:RadComboBoxItem>
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        </asp:Panel>
                        <tr align="center">
                            <td align="center">
                                <asp:Panel ID="pnlStandard" runat="server" Width="100%" >
                                <fieldset>
                                    <table width="100%">
                                        <tr align="center">
                                            <td  align="center">
                                                <table   style="width:<%= tblWidth %> ;" cellspacing="0" cellpadding="0" border="0">
                                                    <tr style="height: 30px">
                                                        <td  align="right"   >
                                                            <asp:Label ID="lblReportTitle" runat="server"   CssClass="formtextGreen" meta:resourcekey="lblReportTitleResource1" Font-Bold="true"
                                                                Text="Report:" style="padding-right:3px"></asp:Label>
                                                       </td>
                                                       <td align ="left">
                                                            <telerik:RadComboBox ID="cboReports" runat="server" AutoPostBack="True" DataTextField="GuiName"
                                                                DataValueField="GuiId" OnSelectedIndexChanged="cboReports_SelectedIndexChanged"
                                                                meta:resourcekey="cboReportsResource1" Width="358px" Filter="Contains" MarkFirstMatch="true"
                                                                EnableScreenBoundaryDetection="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay"
                                                                MaxHeight="400px" CausesValidation = "false">
                                                            </telerik:RadComboBox> 
                                                        </td>
                                                    </tr>
                                                    <tr id="trReportLayout" runat="server" style="height: 40px; vertical-align:top;padding-top:8px;">
                                                        <td style="text-align:left; padding-right:3px;">
                                                        </td>
                                                        <td class="formtext" >
                                                            <asp:Label ID="lblReportlayout" runat="server" Text="Layout: " CssClass="formtextGreen" />
                                                            <asp:RadioButtonList ID="rblReportLayout" runat="server" TextAlign="Right" RepeatLayout="Flow" RepeatDirection="Horizontal">
                                                                <asp:ListItem Text="Normarl" Selected="True" Value="1"/>
                                                                <asp:ListItem Text="Drill down" Value="2"/>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td  align="right"  style="width: 50px" >
                                                        </td>
                                                        <td colspan="4" align="center" >
              <!-- Audit Report By Devin 2014.01.27 Begin-->
                                                            <table id ="tbAuditReport" runat = "server" visible="false" width="100%" >
                                                            <tr >
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkWorkShiftViolation" runat="server" CssClass="formtext" Text="Include Work Shift Violation" Checked = "true"
                                                                            meta:resourcekey="chkWorkShiftViolationResource1"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkDailyViolation" runat="server" CssClass="formtext" Text="Include Daily Violation" Checked = "true"
                                                                            meta:resourcekey="chkDailyViolationResource1"></asp:CheckBox>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkOffDutyViolation" runat="server" CssClass="formtext" Text="Include Off Duty Violation" Checked = "true"
                                                                            meta:resourcekey="chkOffDutyViolationResource1"></asp:CheckBox>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkCycleViolation" runat="server" CssClass="formtext" Text="Include Cycle Violation" Checked = "true"
                                                                            meta:resourcekey="chkCycleViolationResource1"></asp:CheckBox>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkPreTripNotDone" runat="server" CssClass="formtext" Text="Include Pre Trip Inspection Not Completed" Checked = "true"
                                                                            meta:resourcekey="chkPreTripNotDoneResource1"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkPostTripNotDone" runat="server" CssClass="formtext" Text="Include Post Trip Inspection Not Completed" Checked = "true"
                                                                            meta:resourcekey="chkPostTripNotDoneResource1"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkDrivingWithDefect" runat="server" CssClass="formtext" Text="Include Driving With Major Defect" Checked = "true"
                                                                            meta:resourcekey="chkDrivingWithDefectResource1"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                  <td align="left">
                                                                   <asp:CheckBox ID="chkDriverWithoutSigned" runat="server" CssClass="formtext" Text="Driven without signed in" Checked = "false"
                                                                           meta:resourcekey="chkDriverWithoutSignedResource1"></asp:CheckBox>
                                                                   </td>
                                                            </tr>
                                                            <tr> 
                                                                  <td align="left">
                                                                   <asp:CheckBox ID="chkLogsNotReceive" runat="server" CssClass="formtext" Text="Logs are not received after 1 week" Checked = "false"
                                                                           meta:resourcekey="chkLogsNotReceiveResource1"></asp:CheckBox>
                                                                   </td>

                                                             </tr>

                                                            </table>
                                                            <!-- End -->
                                                            <table border="0" cellpadding="0" cellspacing="0" width="100%" class="formtext">
                                                                <tr>
                                                                    <td colspan="2" class="formtext" align="left" valign="top">
                                                                        <table id="tblException" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td height="3">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext">
                                                                                    <table id="Table3" cellspacing="0" cellpadding="0" border="0">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:CheckBox ID="chkDoor" runat="server" CssClass="formtext" Width="203px" Text="No"
                                                                                                    Visible="False" meta:resourcekey="chkDoorResource1"></asp:CheckBox>
                                                                                                <table id="Table5" style=" padding:0px; border-right: gray 1px inset; border-top: gray 1px inset;
                                                                                                    border-left: gray 1px inset; border-bottom: gray 1px inset" cellspacing="0" cellpadding="0"
                                                                                                    border="0"    >
                                                                                                    <tr>
                                                                                                        <td class="formtext" align="left" height="4">
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td class="formtext" align="left" colspan ="2">
                                                                                                            <asp:Label ID="Label3" runat="server" CssClass="formtextGreen" meta:resourcekey="Label3Resource1"
                                                                                                                Text="No opening/closing within:" ></asp:Label>
                                                                                                            <telerik:RadComboBox ID="cboDoorPeriod" runat="server" CssClass="RegularText" meta:resourcekey="cboDoorPeriodResource1"
                                                                                                                Filter="Contains" MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false"
                                                                                                                Skin="Hay">
                                                                                                                <Items>
                                                                                                                    <telerik:RadComboBoxItem Value="1" meta:resourcekey="ListItemResource1" Text="1 Hour">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="2" meta:resourcekey="ListItemResource2" Text="2 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="3" meta:resourcekey="ListItemResource3" Text="3 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="4" meta:resourcekey="ListItemResource4" Text="4 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="5" meta:resourcekey="ListItemResource5" Text="5 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="6" meta:resourcekey="ListItemResource6" Text="6 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="7" meta:resourcekey="ListItemResource7" Text="7 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="8" meta:resourcekey="ListItemResource8" Text="8 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="9" meta:resourcekey="ListItemResource9" Text="9 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="18" meta:resourcekey="ListItemResource10" Text="18 Hours">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="24" meta:resourcekey="ListItemResource11" Text="1 Day">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="48" meta:resourcekey="ListItemResource12" Text="2 Days">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="72" meta:resourcekey="ListItemResource13" Text="3 Days">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="168" meta:resourcekey="ListItemResource14" Text="Week">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="730" meta:resourcekey="ListItemResource15" Text="1 Month">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                    <telerik:RadComboBoxItem Value="1460" meta:resourcekey="ListItemResource16" Text="2 Month">
                                                                                                                    </telerik:RadComboBoxItem>
                                                                                                                </Items>
                                                                                                            </telerik:RadComboBox>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="left">
                                                                                                            <asp:CheckBox ID="chkDriverDoorExc" runat="server" CssClass="formtext" Text="Driver Door"
                                                                                                                meta:resourcekey="chkDriverDoorExcResource1"></asp:CheckBox>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker4" Visible="false" runat="server" Text="Locker 4" CssClass="formtext" meta:resourcekey="chkLocker4Resource" />                            
                                                                                                        </td>

                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="left">
                                                                                                            <asp:CheckBox ID="chkPassengerDoorExc" runat="server" CssClass="formtext" Text="Passenger Door"
                                                                                                                meta:resourcekey="chkPassengerDoorExcResource1"></asp:CheckBox>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker5" Visible="false" runat="server" Text="Locker 5" CssClass="formtext" meta:resourcekey="chkLocker5Resource" />                            
                                                                                                        </td>

                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="left">
                                                                                                            <asp:CheckBox ID="chkSideHopperDoorExc" runat="server" CssClass="formtext" Text="Side Hopper Door"
                                                                                                                meta:resourcekey="chkSideHopperDoorExcResource1"></asp:CheckBox>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker6" Visible="false" runat="server" Text="Locker 6" CssClass="formtext" meta:resourcekey="chkLocker6Resource" />                            
                                                                                                        </td>

                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="left">
                                                                                                            <asp:CheckBox ID="chkRearHopperDoorExc" runat="server" CssClass="formtext" Text="Rear Hopper Door"
                                                                                                                meta:resourcekey="chkRearHopperDoorExcResource1"></asp:CheckBox>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker7" Visible="false" runat="server" Text="Locker 7" CssClass="formtext" meta:resourcekey="chkLocker7Resource" />                            
                                                                                                        </td>

                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker1" Visible="false" runat="server" Text="Locker 1" CssClass="formtext" meta:resourcekey="chkLocker1Resource" />                            
                                                                                                        </td>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker8" Visible="false" runat="server" Text="Locker 8" CssClass="formtext" meta:resourcekey="chkLocker8Resource" />                            
                                                                                                        </td>

                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker2" Visible="false" runat="server" Text="Locker 2" CssClass="formtext" meta:resourcekey="chkLocker2Resource" />                            
                                                                                                        </td>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker9" Visible="false" runat="server" Text="Locker 9" CssClass="formtext" meta:resourcekey="chkLocker9Resource" />                            
                                                                                                        </td>

                                                                                                    </tr>

                                                                                                    <tr>
                                                                                                        <td>
                                                                                                          <asp:CheckBox ID="chkLocker3" Visible="false" runat="server" Text="Locker 3" CssClass="formtext" meta:resourcekey="chkLocker3Resource" />                            
                                                                                                        </td>
                                                                                                        <td></td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="left" height="6">
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </td>
                                                                                            <td>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height:5px">
                                                                              <td colspan ="3" ></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table2" cellspacing="0" cellpadding="0" border="0">
                                                                                        <tr>
                                                                                            <td valign="top">
                                                                                                <asp:CheckBox ID="chkSOSMode" runat="server" CssClass="formtext" Width="114px" Text="More than"
                                                                                                    meta:resourcekey="chkSOSModeResource1"></asp:CheckBox>
                                                                                            </td>
                                                                                            <td valign="bottom">
                                                                                                <telerik:RadComboBox ID="cboSOSLimit" runat="server" CssClass="RegularText" meta:resourcekey="cboSOSLimitResource1"
                                                                                                    Filter="Contains" MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false"
                                                                                                    Skin="Hay">
                                                                                                    <Items>
                                                                                                        <telerik:RadComboBoxItem Value="3" meta:resourcekey="ListItemResource17" Text="3">
                                                                                                        </telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="6" meta:resourcekey="ListItemResource18" Text="6">
                                                                                                        </telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="9" meta:resourcekey="ListItemResource19" Text="9">
                                                                                                        </telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="18" meta:resourcekey="ListItemResource20" Text="18">
                                                                                                        </telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="24" meta:resourcekey="ListItemResource21" Text="24">
                                                                                                        </telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="48" meta:resourcekey="ListItemResource22" Text="48">
                                                                                                        </telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="72" meta:resourcekey="ListItemResource23" Text="72">
                                                                                                        </telerik:RadComboBoxItem>
                                                                                                    </Items>
                                                                                                </telerik:RadComboBox>
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                            </td>
                                                                                            <td class="formtext" valign="middle" >
                                                                                                <asp:Label ID="lblSOSModesTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSOSModesTitleResource1"
                                                                                                    Text="SOS modes"></asp:Label>
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkTAR" runat="server" CssClass="formtext" Width="165px" Text="Any TAR mode events"
                                                                                        meta:resourcekey="chkTARResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkImmobilization" runat="server" CssClass="formtext" Width="211px"
                                                                                        Text="Vehicle immobilization events" meta:resourcekey="chkImmobilizationResource1">
                                                                                    </asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkDriverDoor" runat="server" CssClass="formtext" Width="292px"
                                                                                        Text="15 seconds driver/passenger door violation" meta:resourcekey="chkDriverDoorResource1">
                                                                                    </asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkLeash" runat="server" CssClass="formtext" Width="270px" Text="50% of Leash Event"
                                                                                        meta:resourcekey="chkLeashResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <table id="tblHistoryOptions" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkHistIncludeCoordinate" runat="server" CssClass="formtext" Text="Include Coordinates"
                                                                                        Checked="True" meta:resourcekey="chkHistIncludeCoordinateResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkHistIncludeSensors" runat="server" CssClass="formtext" Width="196px"
                                                                                        Text="Include Sensors" Checked="True" meta:resourcekey="chkHistIncludeSensorsResource1">
                                                                                    </asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkHistIncludeInvalidGPS" runat="server"  CssClass="formtext"
                                                                                        Width="280px" Text="Include Invalid GPS for coordinate messages" Checked="True"
                                                                                        meta:resourcekey="chkHistIncludeInvalidGPSResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkHistIncludePositions" runat="server" CssClass="formtext" Width="261px"
                                                                                        Text="Include Position" Checked="True" Visible="False" meta:resourcekey="chkHistIncludePositionsResource1">
                                                                                    </asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <table id="tblOptions1" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkIncludeStreetAddress" runat="server" CssClass="formtext" Text="Include Street Address"
                                                                                        Checked="True" meta:resourcekey="chkIncludeStreetAddressResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkIncludeSensors" runat="server" CssClass="formtext" Width="198px"
                                                                                        Text="Include Sensors" Checked="True" meta:resourcekey="chkIncludeSensorsResource1">
                                                                                    </asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:CheckBox ID="chkIncludePosition" runat="server" CssClass="formtext" Width="211px"
                                                                                        Text="Include Position" Checked="True" meta:resourcekey="chkIncludePositionResource1">
                                                                                    </asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <asp:CheckBox ID="chkShowStorePosition" runat="server" CssClass="formtext" Width="257px"
                                                                            Text="Show Stored Position" meta:resourcekey="chkShowStorePositionResource1">
                                                                        </asp:CheckBox>
                                                                        <asp:CheckBox ID="chkShowDriver" runat="server" CssClass="formtext" Width="257px" Text="Display Driver"></asp:CheckBox>
                                                                        <asp:CheckBox ID="chkShowOdometer" runat="server" CssClass="formtext" Width="257px" Text="Display Odometer"></asp:CheckBox>

                                                                        <table id="tblStopReport" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td class="formtext">
                                                                                    <asp:Label ID="lblStopDurationTitle" runat="server" CssClass="formtextGreen" meta:resourcekey="lblStopDurationTitleResource1"
                                                                                        Text="Duration : "></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <telerik:RadComboBox ID="cboStopSequence" runat="server" CssClass="formtext" meta:resourcekey="cboStopSequenceResource1"
                                                                                        Filter="Contains" MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false"
                                                                                        Skin="Hay">
                                                                                        <Items>
                                                                                            <telerik:RadComboBoxItem Value="0" meta:resourcekey="ListItemResource24" Text="Not Filtered">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="300" meta:resourcekey="ListItemResource25" Text="Longer than 5 Min">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="600" meta:resourcekey="ListItemResource26" Text="Longer than 10 Min">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="900" meta:resourcekey="ListItemResource27" Text="Longer than 15 Min">
                                                                                            </telerik:RadComboBoxItem>

                                                                                            <telerik:RadComboBoxItem Value="1200" Text="Longer than 20 Min">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="1500" Text="Longer than 25 Min">
                                                                                            </telerik:RadComboBoxItem>

                                                                                            <telerik:RadComboBoxItem Value="1800" meta:resourcekey="ListItemResource28" Text="Longer than 30 Min">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="2700" meta:resourcekey="ListItemResource29" Text="Longer than 45 Min">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="3600" meta:resourcekey="ListItemResource30" Text="Longer than 1 Hour">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="7200" meta:resourcekey="ListItemResource31" Text="Longer than 2 Hours">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="43200" meta:resourcekey="ListItemResource32" Text="Longer than 12 Hours">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="86400" meta:resourcekey="ListItemResource33" Text="Longer than 24 Hours">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="172800" meta:resourcekey="ListItemResource34" Text="Longer than 48 Hours">
                                                                                            </telerik:RadComboBoxItem>
                                                                                        </Items>
                                                                                    </telerik:RadComboBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan="2" class="formtext">
                                                                                    <asp:RadioButtonList ID="optStopFilter" runat="server" CssClass="formtext" meta:resourcekey="optStopFilterResource1">
                                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource35" Text="Show Stops Only"></asp:ListItem>
                                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource36" Text="Show Idlings Only"></asp:ListItem>
                                                                                        <asp:ListItem Selected="True" Value="2" meta:resourcekey="ListItemResource37" Text="Show Stops and Idlings"></asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </td>
                                                                            </tr>
                                                                        </table>

                                                                        <table id="tblViolationReport" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td colspan="2" class="formtext">
                                                                                    <table class="formtext" border="0" cellpadding="0" cellspacing="0">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:CheckBox ID="chkSpeedViolation" runat="server" Text="Speed Violation" Checked="True"
                                                                                                    meta:resourcekey="chkSpeedViolationResource1" />
                                                                                            </td>
                                                                                            <td>
                                                                                                &nbsp;&nbsp;&nbsp;
                                                                                            </td>
                                                                                            <td>
                                                                                                 <telerik:RadComboBox ID="cboViolationSpeed" runat="server" CssClass="formtext" Filter="Contains"
                                                                                                    MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay">
                                                                                                    <Items>
                                                                                                       <telerik:RadComboBoxItem Value="1" Text="100 kph (62 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="2" Text="105 kph (65 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="3" Text="110 kph (68 mph)"></telerik:RadComboBoxItem>
                                                                        <telerik:RadComboBoxItem Value="4" Text="115 kph (71 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="5" Text="120 kph (75 mph)"></telerik:RadComboBoxItem>
<telerik:RadComboBoxItem Value="6" Text="125 kph (77 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="7" Text="130 kph (80 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="8" Text="140 kph (90 mph)"></telerik:RadComboBoxItem>
                                                                                                    </Items>
                                                                                                </telerik:RadComboBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                    <table border="0" cellpadding="0" cellspacing="0"   >
                                                                                        <tr>
                                                                                            <td>
                                                                                                 <asp:CheckBox ID="chkHarshAcceleration" runat="server" Text="Harsh Acceleration"
                                                                                        Checked="True" meta:resourcekey="chkHarshAccelerationResource1" />
                                                                                            </td>

                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                 
                                                                                    <asp:CheckBox ID="chkHarshBraking" runat="server" Text="Harsh Braking" Checked="True"
                                                                                        meta:resourcekey="chkHarshBrakingResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                
                                                                                    <asp:CheckBox ID="chkExtremeAcceleration" runat="server" Text="Extreme Acceleration"
                                                                                        Checked="True" meta:resourcekey="chkExtremeAccelerationResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                  <asp:CheckBox ID="chkExtremeBraking" runat="server" Text="Extreme Braking" Checked="True"
                                                                                        meta:resourcekey="chkExtremeBrakingResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                 <asp:CheckBox ID="chkSeatBeltViolation" runat="server" Text="SeatBelt Violation"
                                                                                        Checked="True" meta:resourcekey="chkSeatBeltViolationResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                  <asp:CheckBox ID="chkReverseSpeed" runat="server" Text="Reverse Excess Speed"
                                                                                        Checked="False" Visible=false   />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                    <asp:CheckBox ID="chkReverseDistance" runat="server" Text="Reverse Excess Distance"
                                                                                        Checked="False" Visible=false />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                  <asp:CheckBox ID="chkHighRail" runat="server" Text="HyRail"
                                                                                        Checked="False" Visible=false />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                  
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <table id="tblViolationReport_Extended" cellspacing="0" cellpadding="0" border="0" runat="server" visible=false >
                                                                            <tr>
                                                                                <td colspan="2" class="formtext">
                                                                                    <table class="formtext" border="0" cellpadding="0" cellspacing="0">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:CheckBox ID="chkSpeedViolation_Extended" runat="server" Text="Speed Violation" Checked="True"
                                                                                                    />
                                                                                            </td>
                                                                                            <td>
                                                                                                &nbsp;&nbsp;&nbsp;
                                                                                            </td>
                                                                                            <td>
                                                                                                <telerik:RadComboBox ID="cboViolationSpeed_Extended" visible=false runat="server" CssClass="formtext" Filter="Contains"
                                                                                                    MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay">
                                                                                                    <Items>
                                                                                                        <telerik:RadComboBoxItem Value="1" Text="100 kph (62 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="2" Text="105 kph (65 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="3" Text="110 kph (68 mph)"></telerik:RadComboBoxItem>
                                                                        <telerik:RadComboBoxItem Value="4" Text="115 kph (71 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="5" Text="120 kph (75 mph)"></telerik:RadComboBoxItem>
<telerik:RadComboBoxItem Value="6" Text="125 kph (77 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="7" Text="130 kph (80 mph)"></telerik:RadComboBoxItem>
                                                                                                        <telerik:RadComboBoxItem Value="8" Text="140 kph (90 mph)"></telerik:RadComboBoxItem>
                                                                                                    </Items>
                                                                                                </telerik:RadComboBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                      <asp:CheckBox ID="chkPostedSpeed_Extended" runat="server" Text="Posted Speed Violation"
                                                                                        Checked="True" /><br />

                                                                                    <asp:CheckBox ID="chkAcceleration_Extended" runat="server" Text="Acceleration"
                                                                                        Checked="True" /><br />
                                                                                    <asp:CheckBox ID="chkBraking_Extended" runat="server" Text="Braking" Checked="True"
                                                                                        /><br />
                                                                                    <asp:CheckBox ID="chkSeatBelt_Extended" runat="server" Text="SeatBelt"
                                                                                        Checked="True"  /><br />
                                                                                        <asp:CheckBox ID="chkHyRailSpeed_Extended" runat="server" Text="HyRail Speed"
                                                                                        Checked="True" /><br />
                                                                                             <asp:CheckBox ID="chkReverseSpeed_Extended" runat="server" Text="Reverse Speed"
                                                                                        Checked="True"   /><br />
                                                                                        <asp:CheckBox ID="chkHyRailReverseSpeed_Extended" runat="server" Text="HyRail Reverse Speed"
                                                                                        Checked="True" /><br />
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </table>




                                                                        <table id="tblViolationReport_ExtendedSummary" cellspacing="0" cellpadding="0" border="0" runat="server" visible=false >
                                                                            <tr>
                                                                                <td class="formtext" style="width:170px" >
                                                                                                <asp:CheckBox ID="SpeedViolation_ExtendedSummary" runat="server" Text="Speed Violation" Checked="True" />
                                                                                    <br />
                                                                                    <asp:CheckBox ID="chkPostedSpeed_ExtendedSummary" runat="server" Text="Posted Speed Violation"
                                                                                        Checked="True" /><br />
                                                                                    <asp:CheckBox ID="Acceleration_ExtendedSummary" runat="server" Text="Acceleration"
                                                                                        Checked="True" /><br />
                                                                                    <asp:CheckBox ID="Braking_ExtendedSummary" runat="server" Text="Braking" Checked="True"
                                                                                        /><br />
                                                                                    <asp:CheckBox ID="SeatBelt_ExtendedSummary" runat="server" Text="SeatBelt"
                                                                                        Checked="True"  /><br />
                                                                                    <asp:CheckBox ID="HyRailSpeed_ExtendedSummary" runat="server" Text="HyRail Speed"
                                                                                        Checked="True" /><br />
                                                                                    <asp:CheckBox ID="ReverseSpeed_ExtendedSummary" runat="server" Text="Reverse Speed"
                                                                                        Checked="True"   /><br />
                                                                                    <asp:CheckBox ID="HyRailReverseSpeed_ExtendedSummary" runat="server" Text="HyRail Reverse Speed"
                                                                                        Checked="True" /><br />
                                                                                    <br />
                                                                                </td>
                                                                                <td>
                                                                                          <fieldset runat="server" id="ViolationReport_ExtendedSummaryPoints" style="width: 400px">
                                                                                        <table id="Table4" runat="server" class="formtext"  style="width: 380px">
                                                                                <tr>
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label12" runat="server" CssClass="formtext" Text="Speed > 65 MPH" ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSpeed_65_MPH_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>


                                                                                      <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label15" runat="server" CssClass="formtext" Text="Speed > 80 MPH"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSpeed_75_MPH_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                        Text="30"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>


                                                                                       <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label18" runat="server" Text="Speed > 85 MPH" CssClass="formtext" ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSpeed_80_MPH_ExtSummary" runat="server" CssClass="formtext" Width="30px"
                                                                                                        Text="50"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                             
                                                                                </tr>


                                                                                  <tr>
                                                                                           <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label13" runat="server" CssClass="formtext" Text="Speed 4+ Over Posted" ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSpeed4_OverPosted_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label14" runat="server" CssClass="formtext" Text="Speed 10+ Over Posted"
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txt_Speed10_OverPosted_ExtSummary" runat="server" CssClass="formtext" Width="30px"
                                                                                                         Text="30"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>


                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label23" runat="server" CssClass="formtext" Text="Speed 15+ Over Posted"
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txt_Speed15_OverPosted_ExtSummary" runat="server" CssClass="formtext" Width="30px"
                                                                                                         Text="50"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                
                                                                                </tr>


                                                                                <tr>
                                                                                  
                                                                                    <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label16" runat="server" Text="Acceleration" CssClass="formtext"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtAcceleration_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                        Text="20"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label17" runat="server" Text="Seat Belt"   CssClass="formtext" ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSeatBelt_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                        Text="50"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>

                                                                                      <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label19" runat="server" CssClass="formtext" Text="Braking"
                                                                                                       ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtBraking_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                       Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                               

                                                                               <tr>
                                                                                 
                                                                                  

                                                                            

                                                                                    <td align="center" valign="top">


                                                                                       <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label24" runat="server" CssClass="formtext" Text="Reverse Speed" 
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtReverseSpeed_ExtSummary" runat="server" CssClass="formtext"  Width="30px" 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>

                                                                               
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label21" runat="server" CssClass="formtext" Text="HyRail Rev. Speed"  
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtHyRail_Reverse_Speed_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>

                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label22" runat="server" CssClass="formtext" Text="HyRail Speed" 
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtHyRailSpeed_ExtSummary" runat="server" CssClass="formtext" Width="30px" 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>

                                                                                    
                                                                                </tr>

                                                                            </table>
                                                                        </fieldset>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <table id="tblRoadSpeed" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td colspan=2 >
                                                                                    <asp:CheckBox ID="chkIsPostedOnly"  runat="server" 
                                                                                    Text="Posted Road Speed Only" Checked="True" Enabled ="false"  class="formtext" TextAlign="Left"
                                                                               /></td>
                                                                             </tr>
                                                                             <tr>
                                                                                <td>
                                                                                     <asp:Label ID="lblRoadSpeedDelta" runat="server" Text="Speed Over Road Speed:" CssClass="formtext"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <telerik:RadComboBox ID="cboRoadSpeedDelta" runat="server" CssClass="formtext" Filter="Contains"
                                                                                                    MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay">
                                                                                       <Items>
                                                                                        <telerik:RadComboBoxItem Value="10" Text ="11 kph (7 mph)"></telerik:RadComboBoxItem>
                                                                                        <telerik:RadComboBoxItem Value="20" Text ="20 kph (13 mph)"></telerik:RadComboBoxItem>
                                                                                        <telerik:RadComboBoxItem Value="25" Text ="25 kph (16 mph)"></telerik:RadComboBoxItem>
                                                                                        <telerik:RadComboBoxItem Value="30" Text ="30 kph (19 mph)"></telerik:RadComboBoxItem>
                                                                                        <telerik:RadComboBoxItem Value="35" Text ="35 kph (22 mph)"></telerik:RadComboBoxItem>
                                                                                        <telerik:RadComboBoxItem Value="40" Text = "125 kph (25 mph)"></telerik:RadComboBoxItem>
                                                                                       </Items>
                                                                                    </telerik:RadComboBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <div id="tblLandmarkOptions" runat="server" style="padding: 10px 10px 10px 0px">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblLandmarkCaption" runat="server" Text="Landmark:" CssClass="formtextGreen"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <telerik:RadComboBox ID="ddlLandmarks" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                                            DataTextField="LandmarkName" DataValueField="LandmarkName" Width="342px" Filter="Contains"
                                                                                            MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="400px">
                                                                                        </telerik:RadComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="trServiceLandmarks" style="margin-top:60px;">
                                                                                    <td>
                                                                                        <asp:Label ID="lblServiceLandmark" runat="server" Text="Landmark:" CssClass="formtextGreen"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <telerik:RadComboBox ID="ddlServiceLandmarks" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                                            DataTextField="LandmarkName" DataValueField="LandmarkID" Width="260px" Filter="Contains"
                                                                                            MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="400px">
                                                                                        </telerik:RadComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="trMasterDelta" style="height:40px; margin-top: 20px;" runat="server" visible="false">
                                                                                    <td style="text-align:right; ">
                                                                                        <asp:Label ID="lblMasterDelta" runat="server" Text="Delta(min): " CssClass="formtextGreen" />
                                                                                    </td>
                                                                                    <td style="text-align:left; font-size: 12px; ">
                                                                                        <asp:TextBox ID="txtMasterDetal" runat="server" Text="31" CssClass="RegularText" Width="40" Height="14" />
                                                                                        <asp:RangeValidator ID="vldMasterDetal" runat="server" ControlToValidate="txtMasterDetal" 
                                                                                                Type="Integer" MinimumValue="0" MaximumValue="100" ErrorMessage="Value must be integer between 0 and 100" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                        <div id="tblLandmarkCategory" runat="server" style="padding: 10px 10px 10px 0px">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 80px;">
                                                                                    <asp:Label ID="lblLandmarkCategory" runat="server" Text="Category : " CssClass="formtextGreen"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <telerik:RadComboBox ID="rcbLandmarkCategory" runat="server" CssClass="RegularText" EnableViewState="true"  AutoPostBack="True"
                                                                                        DataTextField="CategoryName" DataValueField="CategoryID" Width="342px" Filter="Contains" OnSelectedIndexChanged="LandmarkCategorySelection_Changed"
                                                                                        MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="400px">
                                                                                    </telerik:RadComboBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        </div>
                                                                        <div id="tblServiceLandmarks" runat="server" style="padding: 10px 10px 10px 0px;">
                                                                        <table>
                                                                            <tr id="trCategoryServiceLandmarks" style="margin-top:60px;">
                                                                                <td style="width: 80px;">
                                                                                    <asp:Label ID="lblServiceLandmarkList" runat="server" Text="Landmark : " CssClass="formtextGreen"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <telerik:RadComboBox ID="rcbServiceLandmarkList" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                                        DataTextField="LandmarkName" DataValueField="LandmarkID" Width="260px" Filter="Contains"
                                                                                        MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="400px">
                                                                                    </telerik:RadComboBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        </div>
                                                                        <div id="tblLandmarkListOption" runat="server" style="padding: 10px 10px 10px 0px;">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 80px;">
                                                                                    <asp:Label ID="lblLandmarkListOption" Text="List : " CssClass="formtextGreen" runat="server" />
                                                                                </td>
                                                                                <td>
                                                                                     <asp:RadioButtonList ID="rblLandmarkListOption" CssClass="formtext" RepeatDirection="Horizontal" runat="server" >
                                                                                        <asp:ListItem Value="0" Text="Public Landmarks" Selected="True"></asp:ListItem>
                                                                                        <asp:ListItem Value="1" Text="Include Private Landmarks"></asp:ListItem>
                                                                                        <asp:ListItem Value="2" Text="All Landmarks"></asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        </div>
                                                                        <div id="tblLandmarkReportGroup" runat="server" style="padding: 10px 10px 10px 0px;">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 80px;">
                                                                                    <asp:Label ID="lblLandmarkReportGroup" Text="Group by : " CssClass="formtextGreen" runat="server" />
                                                                                </td>
                                                                                <td>
                                                                                     <asp:RadioButtonList ID="rblLandmarkReportGroup" CssClass="formtext" RepeatDirection="Horizontal" runat="server" >
                                                                                        <asp:ListItem Value="0" Text="Landmark" Selected="True"></asp:ListItem>
                                                                                        <asp:ListItem Value="1" Text="Vehicle"></asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        </div>
                                                                        <div id="tblReportFieldOption" runat="server" style="padding: 10px 10px 10px 0px;">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 80px;">
                                                                                    <asp:Label ID="lblReportFieldOption" Text="Include : " CssClass="formtextGreen" runat="server" />
                                                                                </td>
                                                                                <td style="width: 200px;">
                                                                                    <asp:CheckBox ID="chkIdleTimeOption" Text="Idling in Landmark" class="formtext" TextAlign="right"  runat="server" />
                                                                                </td>
                                                                                <td style="width: 200px;">
                                                                                    <asp:CheckBox ID="chkPTOTimeOption"  Text="PTO in Landmark" class="formtext" TextAlign="right" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        </div>
                                                                        <div id="tblGeozoneOptions" runat="server" style="padding: 10px 10px 10px 0px">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblGeozoneCaption" runat="server" Text="Geozone:" CssClass="formtextGreen"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <telerik:RadComboBox ID="ddlGeozones" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                                            DataTextField="GeozoneName" DataValueField="GeozoneNo" Width="355px" Filter="Contains"
                                                                                            MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="400px">
                                                                                        </telerik:RadComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                        <div id="tblDriverOptions" runat="server" style="padding: 10px 10px 10px 0px">
                                                                            <table>
                                                                                <tr>
                                                                                    <td class="style1">
                                                                                        <asp:Label ID="lblDriverCaption" runat="server" Text="Driver:" CssClass="formtextGreen"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <%--<telerik:RadComboBox ID="ddlDrivers" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                                            DataTextField="FullName" DataValueField="DriverId" Width="358px" Filter="Contains"
                                                                                            MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="300px">
                                                                                        </telerik:RadComboBox>--%>
                                                                                        <asp:DropDownList ID="ddlDrivers" runat="server" CssClass="RegularText" EnableViewState="true" AppendDataBoundItems="true"
                                                                                             DataTextField="FullName" DataValueField="DriverId" Width="358px" SkinID="Hay" >
                                                                                        </asp:DropDownList>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                        <table id="tblFleetMaintenance" runat="server" class="formtext" border="0" cellpadding="0" width ="100%" cellspacing="0">
                                                                            <tr id="maintenanceVehicleSelectOption" runat="server" visible="false">
                                                                                <td  class="formtext"   colspan="2">
                                                                                    <table class="formtext" cellpadding=0 cellspacing=0  >
                                                                                        <tr>
                                                                                            <td> Create Report Based On: </td>
                                                                                            <td>
                                                                                               <asp:RadioButtonList ID="optMaintenanceBased" name="ReportBased" runat="server" class="formtext" 
                                                                                                   RepeatDirection="Horizontal" AutoPostBack="true" onselectedindexchanged="optMaintenanceBased_SelectedIndexChanged" >
                                                                                                   <asp:ListItem id="maintenanceOH" name="raVehicleSelectOption" value="0"  Selected="True" runat="server">Organization Hierarchy</asp:ListItem> 
                                                                                                   <asp:ListItem id="maintenanceFleet" type="radio" name="raVehicleSelectOption" value="1" runat="server" Selected="False" >Fleet</asp:ListItem> 
                                                                                                   <asp:ListItem id="maintenanceDriver" type="radio" name="raVehicleSelectOption" value="2" runat="server" Selected="False" Enabled="false">Driver</asp:ListItem> 
                                                                                               </asp:RadioButtonList> 
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>                                               
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 2px">
                                                                                <td class="tableheading" style="width: 63px"></td>
                                                                                <td class="tableheading"></td>
                                                                            </tr>
                                                                            <tr id="trMaintenanceFleet" style="height: 30px" runat="server">
                                                                                <td align="left" valign="top" style="width: 63px">
                                                                                    <asp:Label ID="Label6" runat="server" CssClass="formtextGreen" meta:resourcekey="Label6Resource1"
                                                                                        Text="Fleet:"></asp:Label>
                                                                                </td>
                                                                                <td align="left" valign="top">
                                                                                    <telerik:RadComboBox ID="cboMaintenanceFleet" runat="server" AutoPostBack="True"
                                                                                        CssClass="RegularText" DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboMaintenanceFleetResource1"
                                                                                        Filter="Contains" MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false"
                                                                                        Skin="Hay" MaxHeight="300px">
                                                                                    </telerik:RadComboBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr id="trMaintenanceHierarchy" style="height: 30px" runat="server" visible="false">
                                                                                <td align="left" valign="top" style="width: 63px">
                                                                                    <asp:Label ID="Label11" runat="server" CssClass="formtextGreen" 
                                                                                        Text="Hierarchy Node:"></asp:Label>
                                                                                </td>
                                                                                <td align="left" vaign="top">
                                                                                    <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                                                                        CssClass="combutton" Width="300px" 
                                                                                        OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                                                    />
                                                                                </td>
                                                                            </tr>
                                                                            <tr id="trMaintenanceDriver" style="height: 30px" runat="server" visible="false">
                                                                                <td style="text-align: left; vertical-align: top;">
                                                                                    <asp:Label ID="Label25" runat="server" CssClass="formtextGreen" meta:resourcekey="lblDriverCaptionResource1"
                                                                                        Text="Drivers:" ></asp:Label>
                                                                                </td>
                                                                                <td style="text-align: left; vertical-align: top;">
                                                                                    <ucDriverSearch:DriverSearch ID="ucMantainDrivers" runat="server" Visible="false" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 30px">
                                                                                <td align="left" valign="top" style="width: 63px">
                                                                                    <asp:Label ID="Label7" runat="server" CssClass="formtextGreen" meta:resourcekey="Label7Resource1"
                                                                                        Text="Format:"></asp:Label>
                                                                                </td>
                                                                                <td align="left" valign="top">
                                                                                    <telerik:RadComboBox ID="cboFleetReportFormat" runat="server" 
                                                                                        CssClass="RegularText" meta:resourcekey="cboFleetReportFormatResource1" Filter="Contains"
                                                                                        MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="300px">
                                                                                        <Items>
                                                                                            <telerik:RadComboBoxItem Selected="True" Value="1" meta:resourcekey="ListItemResource54"
                                                                                                Text="PDF"></telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="2" meta:resourcekey="ListItemResource55" Text="Excel">
                                                                                            </telerik:RadComboBoxItem>
                                                                                            <telerik:RadComboBoxItem Value="3" meta:resourcekey="ListItemResource56" Text="Word">
                                                                                            </telerik:RadComboBoxItem>
                                                                                        </Items>
                                                                                    </telerik:RadComboBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="height: 30px">
                                                                                <td align="Center" colspan ="2" >
                                                                                    <asp:Button ID="cmdPreviewFleetMaintenanceReport" runat="server" CausesValidation="False"
                                                                                        ValidationGroup="vgSubmit" OnClick="cmdPreviewFleetMaintenanceReport_Click" CssClass="combutton"
                                                                                        Text="Preview" meta:resourcekey="cmdPreviewFleetMaintenanceReportResource1" />
                                                                                    <asp:Button ID="cmdPreviewFleetMaintenanceReportHide" runat="server" CausesValidation="False" OnClick="cmdPreviewFleetMaintenanceReport_Click" Style="visibility: hidden"
                                                                                        Width="0" Height="0" />
                                                                                        
                                                                                    <asp:Button ID="cmdFleetMaintenanceReportMyReport" runat="server" CssClass="combutton" Text="Execute" Width="130px"
                                                                                        OnClick="cmdFleetMaintenanceReportMyReport_Click" Visible="false" meta:resourcekey="cmdShowMyReportResource1" ValidationGroup="vgSubmit">
                                                                                    </asp:Button>
                                                                                    <asp:Button ID="cmdFleetMaintenanceReportMyReportUpdate" runat="server" CssClass="combutton" Text="Execute and Update" Width="130px"
                                                                                        OnClick="cmdFleetMaintenanceReportMyReportUpdate_Click" Visible="false" meta:resourcekey="cmdShowMyReportUpdateResource1" ValidationGroup="vgSubmit">
                                                                                    </asp:Button>

                                                                                    &nbsp;&nbsp;
                                                                                </td>
                                                                            </tr>
                                                                        </table>

                                                                    </td>

                                                                    <td align="left" valign="top">
                                                                        <table id="tblOptions2" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkIncludeIdleTime" runat="server" CssClass="formtext" Text="Include Idle Time"
                                                                                        Checked="True" meta:resourcekey="chkIncludeIdleTimeResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkIncludeSummary" runat="server" CssClass="formtext" Text="Include Summary"
                                                                                        Checked="True" meta:resourcekey="chkIncludeSummaryResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <fieldset id="tblOffHours" runat="server" style="padding: 5px 5px 5px 5px; width: 90%">
                                                                            <legend>
                                                                                <asp:Label ID="lblWorkingHours" runat="server" CssClass="formtextGreen" Text="Regular Business Hours"></asp:Label>
                                                                            </legend>
                                                                            <table class="formtext" id="Table6" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                                <tr>
                                                                                    <td align="right" valign="top">
                                                                                        &nbsp;
                                                                                    </td>
                                                                                    <td align="right" valign="top">
                                                                                        &nbsp;
                                                                                    </td>
                                                                                    <td colspan="3" align="left" valign="top">
                                                                                        &nbsp;
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" valign="top">
                                                                                        <asp:Label ID="lblMonToFriTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMonToFriTitleResource1"
                                                                                            Text="Monday-Friday:"></asp:Label>
                                                                                    </td>
                                                                                    <td align="right" valign="top">
                                                                                        <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1"
                                                                                            Text="From:"></asp:Label>
                                                                                    </td>
                                                                                    <td colspan="3" align="left" valign="top">
                                                                                        <telerik:RadTimePicker ID="cboFromDayH" runat="server" valign="top" meta:resourcekey="cboFromDayHResource1"
                                                                                            isReadOnly="true" Skin="Hay">
                                                                                            <ClientEvents OnDateSelected="DateSelected" />
                                                                                        </telerik:RadTimePicker>
                                                                                        <asp:Panel ID="pnlOffDateTimeFrom" runat="server" Visible="false">
                                                                                            :<telerik:RadComboBox ID="cboFromDayM" runat="server" CssClass="RegularText" meta:resourcekey="cboFromDayMResource1"
                                                                                                Filter="Contains" MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false"
                                                                                                Skin="Hay">
                                                                                                <Items>
                                                                                                    <telerik:RadComboBoxItem Value="00" meta:resourcekey="ListItemResource38" Text="00">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="15" meta:resourcekey="ListItemResource39" Text="15">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="30" meta:resourcekey="ListItemResource40" Text="30">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="45" meta:resourcekey="ListItemResource41" Text="45">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                </Items>
                                                                                            </telerik:RadComboBox>
                                                                                        </asp:Panel>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                    </td>
                                                                                    <td align="right" valign="top">
                                                                                        <asp:Label ID="lblToTitle" runat="server" CssClass="formtext"  meta:resourcekey="lblToTitleResource1"
                                                                                            Text="To:"></asp:Label>
                                                                                    </td>
                                                                                    <td colspan="3" align="left" valign="top">
                                                                                        <telerik:RadTimePicker ID="cboToDayH" runat="server" valign="top" meta:resourcekey="cboToDayHResource1"
                                                                                            isReadOnly="true" Skin="Hay">
                                                                                        </telerik:RadTimePicker>
                                                                                        <asp:Panel ID="pnlOffDateTimeTo" runat="server" Visible="false">
                                                                                            :<telerik:RadComboBox ID="cboToDayM" runat="server" CssClass="RegularText" meta:resourcekey="cboToDayMResource1"
                                                                                                Filter="Contains" MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false"
                                                                                                Skin="Hay">
                                                                                                <Items>
                                                                                                    <telerik:RadComboBoxItem Value="00" meta:resourcekey="ListItemResource42" Text="00">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="15" meta:resourcekey="ListItemResource43" Text="15">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="30" meta:resourcekey="ListItemResource44" Text="30">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="45" meta:resourcekey="ListItemResource45" Text="45">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                </Items>
                                                                                            </telerik:RadComboBox>
                                                                                        </asp:Panel>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="3">
                                                                                        <asp:CheckBox ID="chkWeekend" runat="server"  Width="184px" Text="Not operated on weekends"
                                                                                            TextAlign="Left" onClick="clickchkWeekend()"  meta:resourcekey="chkWeekendResource1">
                                                                                        </asp:CheckBox>
                                                                                    </td>
                                                                                    <td>
                                                                                    </td>
                                                                                    <td>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" valign="top">
                                                                                        <asp:Label ID="lblWeekendTitle" CssClass="formtext" runat="server" meta:resourcekey="lblWeekendTitleResource1"
                                                                                            Text="Weekend:"></asp:Label>
                                                                                    </td>
                                                                                    <td align="right" valign="top">
                                                                                        <asp:Label ID="lblFromTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitle2Resource1"
                                                                                            Text="From:"></asp:Label>
                                                                                    </td>
                                                                                    <td colspan="3" align="left" valign="top">
                                                                                        <telerik:RadTimePicker ID="cboWeekEndFromH" runat="server" valign="top" meta:resourcekey="cboWeekEndFromHResource1"
                                                                                            Skin="Hay">
                                                                                        </telerik:RadTimePicker>
                                                                                        <asp:Panel ID="pnlcboWeekEndFromHFrom" runat="server" Visible="false">
                                                                                            :<telerik:RadComboBox ID="cboWeekEndFromM" runat="server" CssClass="RegularText"
                                                                                                meta:resourcekey="cboWeekEndFromMResource1" Filter="Contains" MarkFirstMatch="true"
                                                                                                ChangeTextOnKeyBoardNavigation="false" Skin="Hay">
                                                                                                <Items>
                                                                                                    <telerik:RadComboBoxItem Value="00" meta:resourcekey="ListItemResource46" Text="00">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="15" meta:resourcekey="ListItemResource47" Text="15">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="30" meta:resourcekey="ListItemResource48" Text="30">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="45" meta:resourcekey="ListItemResource49" Text="45">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                </Items>
                                                                                            </telerik:RadComboBox>
                                                                                        </asp:Panel>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                    </td>
                                                                                    <td align="right" valign="top">
                                                                                        <asp:Label ID="lblToTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblToTitle2Resource1"
                                                                                            Text="To:"></asp:Label>
                                                                                    </td>
                                                                                    <td colspan="3" align="left" valign="top">
                                                                                        <telerik:RadTimePicker ID="cboWeekEndToH" runat="server" valign="top" meta:resourcekey="cboWeekEndToHResource1"
                                                                                            Skin="Hay">
                                                                                        </telerik:RadTimePicker>
                                                                                        <asp:Panel ID="pnlcboWeekEndFromHTo" runat="server" Visible="false">
                                                                                            :<telerik:RadComboBox ID="cboWeekEndToM" runat="server" CssClass="RegularText" meta:resourcekey="cboWeekEndToMResource1"
                                                                                                Filter="Contains" MarkFirstMatch="true" ChangeTextOnKeyBoardNavigation="false"
                                                                                                Skin="Hay">
                                                                                                <Items>
                                                                                                    <telerik:RadComboBoxItem Value="00" meta:resourcekey="ListItemResource50" Text="00">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="15" meta:resourcekey="ListItemResource51" Text="15">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="30" meta:resourcekey="ListItemResource52" Text="30">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                    <telerik:RadComboBoxItem Value="45" meta:resourcekey="ListItemResource53" Text="45">
                                                                                                    </telerik:RadComboBoxItem>
                                                                                                </Items>
                                                                                            </telerik:RadComboBox>
                                                                                        </asp:Panel>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </fieldset>
                                                                        <table id="tblException1" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkExcBattery" runat="server" CssClass="formtext" Text="Main battery and backup battery"
                                                                                        meta:resourcekey="chkExcBatteryResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkExcTamper" runat="server" CssClass="formtext" Text="Tamper events"
                                                                                        meta:resourcekey="chkExcTamperResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkExcPanic" runat="server" CssClass="formtext" Text="Any panic events"
                                                                                        meta:resourcekey="chkExcPanicResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkExcKeypad" runat="server" CssClass="formtext" Text="3 keypad/card attempts incorrect"
                                                                                        meta:resourcekey="chkExcKeypadResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkExcGPS" runat="server" CssClass="formtext" Text="Alt GPS antenna"
                                                                                        meta:resourcekey="chkExcGPSResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkExcAVL" runat="server" CssClass="formtext" Text="Controller Status"
                                                                                        meta:resourcekey="chkExcAVLResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkExcLeash" runat="server" CssClass="formtext" Text="Leash broken"
                                                                                        meta:resourcekey="chkExcLeashResource1"></asp:CheckBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkIncCurTARMode" runat="server" CssClass="formtext" Text="Include current TAR mode status"
                                                                                        meta:resourcekey="chkIncCurTARModeResource1" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <fieldset runat="server" id="tblPoints" style="width: 300px">
                                                                            <table id="Table1" runat="server" class="formtext">
                                                                                <tr>
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblSpeed120" runat="server" CssClass="formtext" Text="Speed >" meta:resourcekey="lblSpeed120Resource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSpeed120" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed120Resource1"
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="Label9" runat="server" CssClass="formtext" Text="Acc. Harsh" meta:resourcekey="Label9Resource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtAccHarsh" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccHarshResource1"
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblBrakingExtreme" runat="server" CssClass="formtext" Text="Braking Extreme"
                                                                                                        meta:resourcekey="lblBrakingExtremeResource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtBrakingExtreme" runat="server" CssClass="formtext" Width="30px"
                                                                                                        meta:resourcekey="txtBrakingExtremeResource1" Text="20"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblSpeed130" runat="server" CssClass="formtext" Text="Speed >" meta:resourcekey="lblSpeed130Resource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSpeed130" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed130Resource1"
                                                                                                        Text="20"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblAccExtreme" runat="server" Text="Acc. Extreme" CssClass="formtext" meta:resourcekey="lblAccExtremeResource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtAccExtreme" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccExtremeResource1"
                                                                                                        Text="20"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblSeatBelt" runat="server" Text="Seat Belt" CssClass="formtext" meta:resourcekey="lblSeatBeltResource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSeatBelt" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSeatBeltResource1"
                                                                                                        Text="50"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="center" valign="top">
                                                                                        <table class="formtext">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblSpeed140" runat="server" Text="Speed >" CssClass="formtext" meta:resourcekey="lblSpeed140Resource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSpeed140" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed140Resource1"
                                                                                                        Text="50"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblBrakingHarsh" runat="server" CssClass="formtext" Text="Braking Harsh"
                                                                                                        meta:resourcekey="lblBrakingHarshResource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtBrakingHarsh" runat="server" CssClass="formtext" Width="30px"
                                                                                                        meta:resourcekey="txtBrakingHarshResource1" Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>

                                                                                     <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblReverseSpeed" runat="server" CssClass="formtext" Text="Reverse Speed" Visible=false  
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtReverseSpeed" runat="server" CssClass="formtext" Width="30px" Visible=false 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                               </tr>
                                                                               <tr>
                                                                                    <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblHihgRail" runat="server" CssClass="formtext" Text="HyRail" Visible=false  
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtHighRail" runat="server" CssClass="formtext" Width="30px" Visible=false 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                     <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblOver10" runat="server" CssClass="formtext" Text="Over 10" Visible=false  
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtOver10" runat="server" CssClass="formtext" Width="30px" Visible=false 
                                                                                                        Text="1"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>

                                                                                       <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblReverseDistance" runat="server" CssClass="formtext" Text="Reverse Distance" Visible=false  
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtReverseDistance" runat="server" CssClass="formtext" Width="30px" Visible=false 
                                                                                                        Text="10"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>

                                                                                    </tr>
                                                                                    <tr>
                                                                                       <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblOver4" runat="server" CssClass="formtext" Text="Over 4" Visible=false  
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtOver4" runat="server" CssClass="formtext" Width="30px" Visible=false 
                                                                                                        Text="1"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>


                                                                                      


                                                                                       <td align="center" valign="top">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblOver15" runat="server" CssClass="formtext" Text="Over 15" Visible=false  
                                                                                                        ></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtOver15" runat="server" CssClass="formtext" Width="30px" Visible=false 
                                                                                                        Text="1"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    </tr>
                                                                                
                                                                            </table>
                                                                        </fieldset>
                                                                        <asp:Panel ID="tblIgnition" runat="server">
                                                                            <table>
                                                                                <tr >
                                                                                    <td class="formtext"   >
                                                                                        <asp:Label ID="lblIgnition"  CssClass="formtextGreen" runat="server" meta:resourcekey="lblIgnitionResource1"
                                                                                            Text="Calculate Trips based on:" ></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="left">
                                                                                        <asp:RadioButtonList ID="optEndTrip"  runat="server" RepeatDirection="Horizontal"
                                                                                            CssClass="formtext" meta:resourcekey="optEndTripResource1">
                                                                                            <asp:ListItem Selected="True" Text="Ignition" Value="3" meta:resourcekey="ListItemResource60"></asp:ListItem>
                                                                                            <asp:ListItem Text="Tractor Power" Value="11" meta:resourcekey="ListItemResource61"></asp:ListItem>
                                                                                            <asp:ListItem Value="8" meta:resourcekey="ListItemResource62">PTO</asp:ListItem>
                                                                                        </asp:RadioButtonList>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </asp:Panel>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr id="trObservationTime" style= "height:50px; vertical-align: bottom;" runat="server">
                                                        <td colspan ="3">
                                                            <table id="tbObservationTime">
                                                                <tr>
                                                                    <td style="width: 160px; text-align: right;">
                                                                        <asp:Label ID="Label26" Text="Observation Time : " CssClass="formtextGreen" runat="server" /> 
                                                                    </td>
                                                                    <td style="width: 200px">
                                                                        <telerik:RadTimePicker ID="cboObservationTime" runat="server" valign="top" meta:resourcekey="cboHoursToResource1" Skin="Hay" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:RequiredFieldValidator ID="rfvObserveTime" runat="server" ControlToValidate="cboObservationTime"
                                                                                            ValidationGroup="vgSubmit" ErrorMessage="Please specify Observation Time." Text="*">
                                                                        </asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td  align="right"  style="width: 50px" >
                                                        </td>
                                                        <td colspan="5">
                                                            <table width="720px" id="tblGeneralCriteria" runat="server">
                                                                <tr id="trSaturdays"  style="height: 48px; vertical-align: middle;" runat="server">
                                                                    <td class="tableheading" style="width: 52px; text-align: left;" >
                                                                        <asp:Label ID="Label20" Text="To Date: " CssClass="formtextGreen" runat="server" />
                                                                    </td>
                                                                    <td colspan="2" style="text-align: left;">
                                                                        <telerik:RadComboBox ID="cboSaturdays" runat="server" CssClass="RegularText" Width="258px" ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="300px">
                                                                        </telerik:RadComboBox>
                                                                    </td>
                                                                    <td class="tableheading" style="width: 52px;"></td>
                                                                </tr>
                                                                <tr id="DateTimeLabel" runat="server">
                                                                	<td align="left" class="tableheading" style="width: 52px;"></td>
                                                                    <td style="width: 312px;" align="left">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 100px;">
                                                                                    <asp:Label ID="Label1" runat="server" CssClass="formtext" Width="172px" Font-Bold="True"
                                                                                        meta:resourcekey="Label1Resource1" Text="Day"></asp:Label>
                                                                                </td>
                                                                                <td style="width: 24px;">
                                                                                </td>
                                                                                <td style="width: 100px;" align="left">
                                                                                    <asp:Label ID="Label2" runat="server" CssClass="formtext" Width="44px" Font-Bold="True"
                                                                                        meta:resourcekey="Label2Resource1" Text="Time"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td align="left" class="tableheading" style="width: 52px;"></td>
                                                                    <td align="left" >
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 100px;">
                                                                                    <asp:Label ID="Label4" runat="server" CssClass="formtext" Font-Bold="True" Width="172px"
                                                                                        meta:resourcekey="Label4Resource1" Text="Day"></asp:Label>
                                                                                </td>
                                                                                <td style="width: 24px;">
                                                                                </td>
                                                                                <td style="width: 100px;" align="left">
                                                                                    <asp:Label ID="Label5" runat="server" CssClass="formtext" Font-Bold="True" Width="44px"
                                                                                        meta:resourcekey="Label5Resource1" Text="Time"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr id="DateTimeEntry" runat="server">
                                                                    <td class="tableheading" style="width: 52px" align="left" >
                                                                        <asp:Label ID="lblFromTitle3" runat="server" CssClass="formtextGreen" meta:resourcekey="lblFromTitle3Resource1"
                                                                            Text="From:"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 312px" align="left" valign="top">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 100px">
                                                                                    <telerik:RadDatePicker ID="txtFrom" runat="server" Width="182px" DateInput-EmptyMessage=""
                                                                                        MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US" >
                                                                                        <Calendar>
                                                                                            <SpecialDays>
                                                                                                <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                                                                            </SpecialDays>
                                                                                        </Calendar>
                                                                                        <DateInput Culture="en-US" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" />
                                                                                    </telerik:RadDatePicker>
                                                                                </td>
                                                                                <td>
                                                                                        <asp:RequiredFieldValidator ID="valFromDate" runat="server" ControlToValidate="txtFrom"
                                                                                            ValidationGroup="vgSubmit" ErrorMessage="Please select a From" meta:resourcekey="valFromDateResource1"
                                                                                            Text="*"></asp:RequiredFieldValidator>
                                                                                </td>
                                                                                <td style="width: 100px">
                                                                                    <telerik:RadTimePicker ID="cboHoursFrom" runat="server" valign="top" meta:resourcekey="cboHoursFromResource1" Skin="Hay" />
                                                                                </td>
                                                                                <td>
                                                                                    <asp:CompareValidator ID="valCompareDates"
                                                                                runat="server" CssClass="errortext" ControlToValidate="txtTo" ErrorMessage="The From Date should be earlier than the To Date!" Enabled ="false" 
                                                                                Type="Date" Operator="GreaterThanEqual" ControlToCompare="txtFrom" meta:resourcekey="valCompareDatesResource1" 
                                                                                ValidationGroup="vgSubmit" Text="*"></asp:CompareValidator>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td class="tableheading" style="width: 52px" align="left">
                                                                        <asp:Label ID="lblToTitle3" runat="server" CssClass="formtextGreen" meta:resourcekey="lblToTitle3Resource1"
                                                                            Text="To:"></asp:Label>
                                                                    </td>
                                                                    <td align="left">
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 100px">
                                                                                    <telerik:RadDatePicker ID="txtTo" runat="server" Width="182px" DateInput-EmptyMessage=""
                                                                                        Skin="Hay" MinDate="01/01/1900" MaxDate="01/01/3000"  Culture="en-US">
                                                                                        <Calendar>
                                                                                            <SpecialDays>
                                                                                                <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                                                                            </SpecialDays>
                                                                                        </Calendar>
                                                                                        <DateInput Culture="en-US" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" />
                                                                                    </telerik:RadDatePicker>
                                                                                </td>
                                                                                <td>
                                                                                     <asp:RequiredFieldValidator ID="valToDate" runat="server" ControlToValidate="txtTo"
                                                                                            ValidationGroup="vgSubmit" ErrorMessage="Please select a To" meta:resourcekey="valToDateResource1"
                                                                                            Text="*"></asp:RequiredFieldValidator>

                                                                                </td>
                                                                                <td style="width: 100px" valign="top">
                                                                                    <telerik:RadTimePicker ID="cboHoursTo" runat="server" valign="top" meta:resourcekey="cboHoursToResource1"
                                                                                        Skin="Hay" />
                                                                                </td>
                                                                                
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                        <tr id="vehicleSelectOption" class="formtext"  runat="server">
                                            <td  class="formtext"  style="height: 24px"></td>
                                            <td  class="formtext"   colspan="4">
                                                <table class="formtext" runat="server" id="optBaseTable"  cellpadding=0 cellspacing=0  >
                                                    <tr>
                                                        <td> Create Report Based On: </td>
                                                        <td>
                                                            <asp:RadioButtonList ID="optReportBased" name="ReportBased" runat="server"  class="formtext" RepeatDirection="Horizontal" AutoPostBack="true" onselectedindexchanged="optReportBased_SelectedIndexChanged" >
                                                                <asp:ListItem id="Radio1" type="radio" name="raVehicleSelectOption" value="0" runat="server" Selected="True">Organization Hierarchy</asp:ListItem> 
                                                                <asp:ListItem id="Radio2" type="radio" name="raVehicleSelectOption" value="1" runat="server" Selected="False">Fleet</asp:ListItem> 
                                                                <asp:ListItem id="Radio3" type="radio" name="raVehicleSelectOption" value="2" runat="server" Selected="False" Enabled="false">Driver</asp:ListItem> 
                                                            </asp:RadioButtonList> 
                                                        </td>
                                                    </tr>
                                                </table>

                                               <%-- <input id="Radio1" type="radio" name="raVehicleSelectOption" value="1" checked runat="server" onclick="$('#trFleet').hide();$('#organizationHierarchy').show();">Organization Hierarchy
                                                <input id="Radio2" type="radio" name="raVehicleSelectOption" value="2" runat="server" onclick="$('#trFleet').show();$('#organizationHierarchy').hide();">Fleet                                                --%>

                                            </td>
                                        </tr>
                                        <tr id="trReportDriver" runat="server">
                                            <td class="tableheading" style="width: 52px;" align="left">
                                                <asp:Label ID="LabelDriver" runat="server" CssClass="formtextGreen" Width="33px" meta:resourcekey="lblDriverCaptionResource1" Text="Driver : "></asp:Label>
                                            </td>
                                            <td style="width: 312px;" align="left">
                                                <ucDriverSearch:DriverSearch ID="ucReportDriver" runat="server" Visible="true" />
                                            </td>
                                            <td class="formtext" style="width: 52px">
                                                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtextGreen" Visible="False"></asp:Label>
                                            </td>
                                            <td style="width: 300px" align="left">
                                                <asp:CheckBox ID="chkAllDrivers" runat="server" Text="All Drivers"  CssClass="formtext" TextAlign="Right" Checked="false" />
                                            </td>
                                        </tr>
                                        <tr id="trFleet" runat="server">
                                            <td class="tableheading" style="width: 52px;" align="left">
                                                <asp:Label ID="lblFleet" runat="server" CssClass="formtextGreen" Width="33px" meta:resourcekey="lblFleetResource1"
                                                    Text="Fleet:"></asp:Label><asp:RangeValidator ID="valFleet" runat="server" ControlToValidate="cboFleet"
                                                        ErrorMessage="Please select a Fleet" MinimumValue="1" MaximumValue="999999999999999"
                                                        meta:resourcekey="valFleetResource1" Text="*" Enabled="False" ValidationGroup="vgSubmit"></asp:RangeValidator>
                                            </td>
                                            <td style="width: 312px;" align="left">
                                                <telerik:RadComboBox ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                    Width="258px" DataTextField="FleetName" DataValueField="FleetId" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                                    meta:resourcekey="cboFleetResource1" Filter="Contains" MarkFirstMatch="true"
                                                    ChangeTextOnKeyBoardNavigation="false" Skin="Hay" MaxHeight="300px">
                                                </telerik:RadComboBox>
                                            </td>
                                            <td class="formtext" style="width: 52px"><nobr>
                                                <asp:Label ID="lblVehicleList" runat="server" CssClass="formtextGreen"
                                                    Visible="False" meta:resourcekey="lblVehicleNameResource1" Text="Vehicle:"></asp:Label></nobr>
                                            </td>
                                            <td style="width: 300px" align="left">
                                                <telerik:RadComboBox ID="cboVehicle" runat="server" CssClass="RegularText" Width="258px"
                                                    DataTextField="Description" DataValueField="LicensePlate" Visible="False" meta:resourcekey="cboVehicleResource1"
                                                    Filter="Contains" MarkFirstMatch="true" EnableScreenBoundaryDetection="False"
                                                    Skin="Hay" MaxHeight="300px">
                                                </telerik:RadComboBox>
                                            </td>
                                        </tr>
                                        <tr ID="organizationHierarchy" runat="server">
                                            <td colspan="10" align="left">
                                                <div id="ohsearchbar" class="formtext"><asp:Label ID="Label8" runat="server" CssClass="formtextGreen" Text="Search Cost Center: "></asp:Label>
                                                    <input type="text" id="ohsearchbox" class="ohsearch" />
                                                    <a href="javascript:void(0);" onclick="onsearchbtnclicked();"><img src="../images/searchicon.png" border="0" /></a>
                                                    <asp:Label ID="Label10" runat="server" style="color:#666666;" Text="(Type in at least 3 characters to search)"></asp:Label>
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
                                                        <div id="vehiclelistPageBar">
                                                            <table>		            
		                                                        <tr>
			                                                        <td class="pager">
				                                                        <img src="../scripts/tablesorter2145/addons/pager/icons/first.png" class="first" alt="First" />
				                                                        <img src="../scripts/tablesorter2145/addons/pager/icons/prev.png" class="prev" alt="Prev" />
				                                                        <span class="pagedisplay"></span> <!-- this can be any element, including an input -->
				                                                        <img src="../scripts/tablesorter2145/addons/pager/icons/next.png" class="next" alt="Next" />
				                                                        <img src="../scripts/tablesorter2145/addons/pager/icons/last.png" class="last" alt="Last" />				            
			                                                        </td>
		                                                        </tr>
	                                                        </table>
                                                        </div>
                                                   </div>
                                                 </div>
                                                
                                            </td>
                                        </tr>


                                                                <tr>
                                                                    <td class="tableheading" style="width: 52px; height: 9px" align="left">
                                                                        <asp:Label ID="lblFormatTitle" runat="server" CssClass="formtextGreen" meta:resourcekey="lblFormatTitleResource1"
                                                                            Text="Format:"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 312px; height: 9px" align="left">
                                                                        <telerik:RadComboBox ID="cboFormat" runat="server"  CssClass="RegularText"
                                                                            Width="258px" meta:resourcekey="cboFormatResource1" Filter="Contains" MarkFirstMatch="true"
                                                                            ChangeTextOnKeyBoardNavigation="false" Skin="Hay">
                                                                            <Items>
                                                                                <telerik:RadComboBoxItem Value="1" Selected="True" meta:resourcekey="ListItemResource57"
                                                                                    Text="PDF"></telerik:RadComboBoxItem>
                                                                                <telerik:RadComboBoxItem Value="2" meta:resourcekey="ListItemResource58" Text="Excel">
                                                                                </telerik:RadComboBoxItem>
                                                                                <telerik:RadComboBoxItem Value="3" meta:resourcekey="ListItemResource59" Text="Word">
                                                                                </telerik:RadComboBoxItem>
                                                                            </Items>
                                                                        </telerik:RadComboBox>
                                                                        <br />
                                                                        <asp:Label ID="lblReportFormat" runat="server" CssClass="formtext" meta:resourcekey="lblReportFormatResource1"
                                                                            Visible="False"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 52px">
                                                                    </td>
                                                                    <td style="width: 300px; height: 9px" align="left">
                                                                        &nbsp;
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="center" colspan="4">
                                                                        <table style="width: 100%">
                                                                            <tr>
                                                                                <td style="width: 100px">
                                                                                    <asp:CustomValidator ID="cvDate" runat="server" ClientValidationFunction="CustomValidateDate"
                                                                                        EnableClientScript="true" ValidationGroup="vgSubmit" Display="None" ErrorMessage=""   />
                                                                                    <asp:Button ID="cmdShowHide" runat="server" OnClick="cmdShow_Click" Style="visibility: hidden"
                                                                                        Width="0" Height="0" />
                                                                                    <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Width="0" Height="0"
                                                                                        Style="visibility: hidden" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr align="center">
                                                                                <td style="width: 100%">
                                                                                    <asp:Button ID="cmdShow" runat="server" CssClass="combutton" Text="Preview" Width="120px"
                                                                                        OnClick="cmdShow_Click" meta:resourcekey="cmdShowResource1" ValidationGroup="vgSubmit">
                                                                                    </asp:Button>
                                                                                    <asp:Button ID="cmdShowMyReport" runat="server" CssClass="combutton" Text="Execute" Width="130px"
                                                                                        OnClick="cmdShowMyReport_Click" Visible="false" meta:resourcekey="cmdShowMyReportResource1" ValidationGroup="vgSubmit">
                                                                                    </asp:Button>
                                                                                    <asp:Button ID="cmdShowMyReportUpdate" runat="server" CssClass="combutton" Text="Execute and Update" Width="130px"
                                                                                        OnClick="cmdShowMyReportUpdate_Click" Visible="false" meta:resourcekey="cmdShowMyReportUpdateResource1" ValidationGroup="vgSubmit">
                                                                                    </asp:Button>

                                                                                </td>

                                                                            </tr>

                                                                            <tr align="center">
                                                                                <td style="width: 100%">
                                                                                    <table >
                                                                                        <tr>
                                                                                            <td    >
                                                                                                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1"  
                                                                                                    ValidationGroup="vgSubmit"></asp:ValidationSummary>
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
                                                    <tr>
                                                        <td  align="right"  style="width: 50px" >
                                                        </td>
                                                        <td align="center" colspan="5">
                                                            <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                            &nbsp;&nbsp;&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td  align="right"  style="width: 50px" >
                                                        </td>
                                                        <td align="left" colspan="5" height="25" style="width: 100%">
                                                            <table id="tblDesc" align="left" width="761px" cellspacing="0" cellpadding="0" border="0">
                                                                <tr>
                                                                    <td class="tableheading" style="width: 6px">
                                                                    </td>
                                                                    <td class="tableheading" align="left">
                                                                        <b>
                                                                            <asp:Label ID="lblReportDescTitle" runat="server" CssClass="formtextGreen" meta:resourcekey="lblReportDescTitleResource1"
                                                                                Text="Report Description:"></asp:Label></b>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 6px">
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="LabelReportDescription" runat="server" CssClass="formtext"></asp:Label>
                                                                        <!--
                                            <asp:Label ID="lblTripReportDesc" runat="server" CssClass="formtext" meta:resourcekey="lblTripReportDescResource1" Text="This report provides details about vehicle trips in a specified period of time. Each trip is determined by ignition on/off showing trip start, trip end, vehicle position info and any sensors that have been triggered. This report can be customized to include/exclude street address, sensor triggers, scheduled position reports, idling time, stored position and trip summary. It can also be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblHistoryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblHistoryReportDescResource1" Text="This report summarizes all the activities occurred in the system for a particular vehicle during the selected period of time, including IP address updates, sensor triggers, commands, outputs, position updates, scheduled position reports, MDT text messages and Geo Fence violations. This report can be customized to include/exclude sensor triggers, scheduled GPS coordinates and invalid GPS positions. This report can be filtered by date/time and generated for a single vehicle only."></asp:Label>
                                            <asp:Label ID="lblMessageReportDescription" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblMessageReportDescriptionResource1" Text="This report provides list of text messages sent and received by the system in the selected period of time, including From, To, direction of a message, message text and responses. It can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblTripSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblTripSummaryReportDescResource1" Text="This report summarizes each trip determined by ignition on/off showing departure address and time, arrival address and time, distance traveled, trip time, idling time and stop time for each trip. It also totals the number of trips, trip time, idling time, stop time, and distance traveled in the selected period of time. This report can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblAlarmReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblAlarmReportDescResource1" Text="This report summarizes all the security alarms that occurred in the system during the selected period of time. It can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblStopReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblStopReportDescResource1" Text="This report lists all the stops including idling showing arrival time, street address, departure time and stop duration in the selected period of time. It also totals the number of stops and stop time. This report can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblLandmarkActivityReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblLandmarkActivityReportDescResource1" Text="This report provides  a summary of total time spent by a vehicle at every landmark, if any"></asp:Label>
                                            <asp:Label ID="lblOffHoursReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblOffHoursReportDescResource1" Text="This report list all the vehicles that were used after hours."></asp:Label>                    
                                            <asp:Label ID="lblFleetMaintenanceReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetMaintenanceReportDescResource1" Text="Provides a report on vehicle maintenance including current odometer readings, and whether maintenance has been recently performed, is due or is overdue." ></asp:Label>
                                            <asp:Label ID="lblFleetViolationDetailsReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetViolationDetailsReportDescResource1" Text="Provides a report on the occurrence of various driving violations including:  Speed Violation, Harsh Acceleration, Harsh Braking, Extreme Acceleration, Extreme Braking and Seat Belt Violation.  The User can choose which violations and period of time on which to report.  " ></asp:Label>
                                            <asp:Label ID="lblFleetViolationSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetViolationSummaryReportDescResource1" Text="This report provides a summary of violations that have occurred for a specified period of time by assigning configurable demerit point values for each type of violation including: Speed Violation, Harsh Acceleration, Harsh Braking, Extreme Acceleration, Extreme Braking and Seat Belt Violation.  The type of violations to be reported on and the number of demerit points for different types of violations can be specified by the User in the report screen.  The report multiplies the number of violation occurrences by the assigned demerit point value to generate a total violation demerit score for each vehicle.  The total score is colour coded to indicated the severity of the violation demerit score. " ></asp:Label>
                                            <asp:Label ID="lblIdlingDetailsReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblIdlingDetailsReportDescResource1" Text="This report summarizes the total number of hours that a vehicle ignition is on, the number of hours the vehicle ignition is on and engine is idling (no vehicle movement) and the percentage of time the vehicle is idling (idling time divided by total ignition on time).  The User can choose the period of time on which to report." ></asp:Label>
                                            <asp:Label ID="lblIdlingSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblIdlingSummaryReportDescResource1" Text="A summary report on Engine Idling details created for a fleet of vehicles for a selected period.  The report displays the total number of hours for the entire fleet when vehicle ignition is on, total number of hours when vehicle ignition is on and engine is idling (no vehicle movement), the percentage of time all vehicles are idling (idling time divided by total ignition on time) and the average idling time per vehicle." ></asp:Label>
                                            -->
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 20px">
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <asp:Panel ID="pnlPDF" runat="server" >
                                        <tr>
                                            <td  align="center">
                                                <a href="http://www.adobe.com/products/acrobat/readermain.html" target="top">
                                                    <img height="31" src="../images/get_adobe_reader.gif" align="right" alt="Adobe Reader"
                                                        border="0" />
                                                </a>
                                            </td>
                                        </tr>
                                        </asp:Panel>
                                    </table>
                                    </fieldset> 
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </center>
                <asp:HiddenField ID="hidSubmitType" runat = "server" />
            </telerik:RadPageView>
            <telerik:RadPageView ID="Repository" runat="server" PageViewID="Repository" meta:resourcekey="RadPageView2Resource1" Height="92%">
                <center>
                <uc1:frmRepository ID="frmRepository1" runat="server" />
                </center>
            </telerik:RadPageView>
            <telerik:RadPageView ID="View" runat="server" PageViewID="View" meta:resourcekey="RadPageView3Resource1">
                <center>
                <uc2:ViewReport ID="ViewReport1" runat="server"  />
                </center>
            </telerik:RadPageView>
 <telerik:RadPageView ID="ScheduledReport" runat="server" PageViewID="ScheduledReport" meta:resourcekey="RadPageView4Resource1" Height="92%">
                <center>
                <uc1:frmScheduleReportList runat="server" id="frmScheduleReportList" />
                </center>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
            <Windows>
                <telerik:RadWindow ID="UserListDialog" runat="server" Skin="Hay" DestroyOClose="true"
                    ReloadOnShow="false" ShowContentDuringLoad="false" Modal="true" VisibleStatusbar="false"
                    VisibleTitlebar="false" Animation="Fade" AnimationDuration="1"
                      />
            </Windows>
        </telerik:RadWindowManager>
        <telerik:RadInputManager ID="RadInputManager1" runat="server">
            <telerik:NumericTextBoxSetting BehaviorID="NumericBehavior1" EmptyMessage="day"
                Type="Number" DecimalDigits="0">
                <TargetControls>
                    <telerik:TargetInput ControlID="txtStartDay"  />
                    <telerik:TargetInput ControlID="txtDurationDay"  />
                </TargetControls>
            </telerik:NumericTextBoxSetting>
            <telerik:NumericTextBoxSetting BehaviorID="NumericBehavior2" EmptyMessage="hour"
                Type="Number" DecimalDigits="0" >
                <TargetControls>
                    <telerik:TargetInput ControlID="txtStartHour"  />
                    <telerik:TargetInput ControlID="txtDurationHour"  />
                </TargetControls>
            </telerik:NumericTextBoxSetting>

        </telerik:RadInputManager>
         <!-- Devin Added -->
        <asp:Button ID="btnAfterCreate" runat = "server" style="display:none" OnClick="btnAfterCreate_Click" />
        <asp:HiddenField ID="hidAfterCreate" runat = "server"  />

        </form>
    </div>
    <script type ="text/javascript" >

    </script>
    
</body>
</html>
