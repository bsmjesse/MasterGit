<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportMaster_hr.aspx.cs"
    Inherits="SentinelFM.Reports_frmReportMaster" Culture="en-US" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik"%>
<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls" TagPrefix="ISWebInput" %>
<%@ Register src="UserControl/frmRepository.ascx" tagname="frmRepository" tagprefix="uc1" %>
<%@ Register src="UserControl/ViewReport.ascx" tagname="ViewReport" tagprefix="uc2" %>
<%@ Register src="UserControl/frmScheduleReportList.ascx" tagname="ViewReport" tagprefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
    <script src="jqueryFileTree.js?v=20140220" type="text/javascript"></script>
    <script src="splitter.js" type="text/javascript"></script>
    <script src="../scripts/colResizable-1.3.min.js" type="text/javascript"></script>
    <script src="../scripts/json2.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css" type="text/css" />
    <link rel="stylesheet" href="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
	<link href="jqueryFileTree.css?v=20140220" rel="stylesheet" type="text/css" media="screen" />

    <style type="text/css">
        .style1
        {
            width: 45px;
        }
        
        .LeftPane {
            background: none repeat scroll 0 0 #FFFFFF;
            border-color: #BBBBBB #FFFFFF #FFFFFF #BBBBBB;
            border-style: solid;
            border-width: 1px;
            height: 300px;
            overflow: scroll;
            padding: 5px;
            width: 300px;
        }
        
        #vehicletreeview, #vehicledetails
        {
            padding: 0 5px;
        }
        
        #vehicledetails
        {
            border-bottom: 1px solid #CCCCCC;            
            height: 270px;
            margin-bottom: 0;
            margin-right: 0;
            overflow: auto; 
        }
        
        #vehiclelistPageBar
        {
            padding: 5px 0 0 5px;
            height: 26px;  
            overflow: hidden;                  
        }
        
        /*
         * Splitter container. Set this to the desired width and height
         * of the combined left and right panes. In this example, the
         * height is fixed and the width is the full width of the body,
         * less the margin on the splitter itself.
         */
        #MySplitter {
	        height: 300px;
	        margin: 5px 40px 5px 0;
	        border: 4px solid #bdb;
	        /* No padding allowed */
        }
        /*
         * Left-side element of the splitter. Use pixel units for the
         * min-width and max-width; the splitter plugin parses them to
         * determine the splitter movement limits. Set the width to
         * the desired initial width of the element; the plugin changes
         * the width of this element dynamically.
         */
        #LeftPane {
	        background: #efe;
	        overflow: auto;
	        /* No margin or border allowed */
        }
        /*
         * Right-side element of the splitter.
         */
        #RightPane {
	        background: #f8fff8;
	        overflow: hidden;
	        /* No margin or border allowed */
        }
        /* 
         * Splitter bar style; the .active class is added when the
         * mouse is over the splitter or the splitter is focused
         * via the keyboard taborder or an accessKey. 
         */
        #MySplitter .vsplitbar {
	        width: 6px;
	        background: #aca url(images/vgrabber.gif) no-repeat center;
        }
        #MySplitter .vsplitbar.active {
	        background: #da8 url(images/vgrabber.gif) no-repeat center;
	        opacity: 0.7;
        }
        
        ul.jqueryFileTree A
        {
            text-align:left;
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
    
    
    
</head>
<body>
    <script language="javascript">
			<!--

           var browserTypes = { "MSIE8": 0, "MSIE7": 1, "MSIE6": 2, "Gecko": 3, "WebKit": 4 };
           var browserType = browserTypes.MSIE;


           var OrganizationHierarchyPath = "";
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

               $('#vehicletreeview').fileTree({ root: PreferOrganizationHierarchyNodeCode, script: 'vehicleListTree.asmx/FetchVehicleList', expanded: inipath, expandSpeed: 200, collapseSpeed: 200
                    , vehicledetails: 'vehicledetails', highlightVehicleSelection: <%=OrganizationHierarchySelectVehicle.ToString().ToLower() %> 
                    , scriptForFetchVehicleByPage: 'vehicleListTree.asmx/FetchVehicleListFilterByPage'
                    , MutipleUserHierarchyAssignment: MutipleUserHierarchyAssignment
                    , PreferOrganizationHierarchyNodeCode: PreferOrganizationHierarchyNodeCode
                    , scriptForPreferNodecodes: 'vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                    , VehiclePageSize: <%=VehiclePageSize %>
                    , SelectedFolders: selectedFolders//IniHierarchyPath ? $('#OrganizationHierarchyNodeCode').val(): ''
                    , SelectedFleetIds: (MutipleUserHierarchyAssignment && appendpath) ? $('#OrganizationHierarchyFleetId').val() : ''
               },
               /*
               * Call back function when you click left pane tree folder.
               */
                        function (NodeCode, FleetId, fleetName, fleetPath) {
                            if(!MutipleUserHierarchyAssignment)
                            {
                                $('#OrganizationHierarchyNodeCode').val(NodeCode);
                                $('#OrganizationHierarchyFleetId').val(FleetId);                                 
                            }
                            $('#OrganizationHierarchyBoxId').val("");
                            $('#fleetPath').html(fleetPath);
                        },

               /*
               * Call back function when you click right pane vehicle list.
               */
                        function (BoxId) {
                            //alert('BoxId: ' + BoxId);
                            $('#OrganizationHierarchyBoxId').val(BoxId);
                        },

                        function (selectedNodecodes, selectedFleetIds, fleetName)
                        {   
                            $('#OrganizationHierarchyNodeCode').val(selectedNodecodes);
                            $('#OrganizationHierarchyFleetId').val(selectedFleetIds);
                        }
                    );
           }

           $(document).ready(function () {

               $("#MySplitter").splitter({
                   type: "v",
                   outline: true,
                   minLeft: 100, sizeLeft: 425, minRight: 100,
                   resizeToWidth: true,
                   cookie: "sfmsplitter6",
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

           });
        
				
			//-->
    </script>

    <script type="text/javascript">
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        var PreferOrganizationHierarchyNodeCode = "<%=PreferOrganizationHierarchyNodeCode %>";
        var IniHierarchyPath = <%=IniHierarchyPath.ToString().ToLower() %>;
    </script>

    <script language="javascript">
	    <!--
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0";
            }
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

    <form id="frmReportMaster" method="post" runat="server">

        <input type="hidden" name="OrganizationHierarchyNodeCode" id="OrganizationHierarchyNodeCode" runat="server"/>
        <input type="hidden" name="OrganizationHierarchyFleetId" id="OrganizationHierarchyFleetId" />
        <input type="hidden" name="OrganizationHierarchyBoxId" id="OrganizationHierarchyBoxId" />

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
                }

                var hideButtonID = '';
                function OpenCreateWindow(validationGroup, hideButton) {
                    hideButtonID = ''

                    return false;
                }

                //Parameter
                //val: 1 means one time report, 2 means schedule report, 3 means my report
                function PopupSubmit(val) {
                    if (hideButtonID != '') {
                        $telerik.$('#' + hideButtonID).click();
                        hideButtonID = '';
                    }
                }
                function PopupLogin() {
                    if (hideButtonID != '') {

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

                                args.IsValid = false;
                                return;
                            }
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

                }
                function refreshAfterSubmit() {
               
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
        
           
        <div   style=" z-index: 101;left: 12px;position: absolute; top: 4px; height: 97%; width: 98%; background-color: white">
            
            <table id="Table4" width="100%" border="0">
                
                <tr>
                    <td height="25px">&nbsp;</td>
                </tr>

                <tr>
                    <td style="height: 80%; width: 90%;" align="center">

                        <table id="Table1" class="tableDoubleBorder"  style="width: 800px;height: 350px;" cellspacing="0" cellpadding="0" border="0">

                            <tr>
                                <td rowspan="4" class="heading" style="width: 10px">
                                </td>
                                <td class="heading" height="20">
                                </td>
                                <td class="formtext" height="20">
                                </td>
                                <td style="width: 10px">
                                </td>
                                <td height="20">
                                </td>
                            </tr>

                            <tr>
                                <%--<td class="heading" style="width: 28px; height: 18px">
                            </td>--%>
                                <td colspan="4" class="heading" align="left" valign="top" style="height: 19px">
                                    &nbsp; &nbsp;<asp:Label ID="lblReportTitle" runat="server" CssClass="heading"
                                        meta:resourcekey="lblReportTitleResource1" Text="Report:"></asp:Label>&nbsp;
                                    <asp:DropDownList ID="cboReports" runat="server" AutoPostBack="True" CssClass="RegularText"
                                        DataTextField="GuiName" DataValueField="GuiId" OnSelectedIndexChanged="cboReports_SelectedIndexChanged"
                                        meta:resourcekey="cboReportsResource1" Width="558px">
                                    </asp:DropDownList></td>
                                <%--<td class="formtext" align="left" valign="top">
                                </td>--%><%--<td style="width: 48px">
                            </td>--%><%--<td>
                            </td>--%>
                            </tr>

                            <tr>
                                <td colspan="4" align="center" style="padding-left: 15px;">
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%"  class="formtext">
                                        <tr>
                                            <%--<td class="formtext" style="width: 28px;">
                                        </td>--%><%--<td class="formtext">
                                        </td>--%>
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
                                                                        <table id="Table5" style="border-right: gray 1px inset; border-top: gray 1px inset;
                                                                            border-left: gray 1px inset; border-bottom: gray 1px inset" cellspacing="0" cellpadding="0"
                                                                            border="0">
                                                                            <tr>
                                                                                <td class="formtext" align="left" height="4">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" align="left">
                                                                                    &nbsp;<asp:Label ID="Label3" runat="server" CssClass="formtext" meta:resourcekey="Label3Resource1"
                                                                                        Text="No opening/closing within:"></asp:Label>
                                                                                    <asp:DropDownList ID="cboDoorPeriod" runat="server" CssClass="RegularText" meta:resourcekey="cboDoorPeriodResource1">
                                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource1" Text="1 Hour"></asp:ListItem>
                                                                                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource2" Text="2 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource3" Text="3 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource4" Text="4 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="5" meta:resourcekey="ListItemResource5" Text="5 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="6" meta:resourcekey="ListItemResource6" Text="6 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="7" meta:resourcekey="ListItemResource7" Text="7 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="8" meta:resourcekey="ListItemResource8" Text="8 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="9" meta:resourcekey="ListItemResource9" Text="9 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="18" meta:resourcekey="ListItemResource10" Text="18 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="24" meta:resourcekey="ListItemResource11" Text="1 Day"></asp:ListItem>
                                                                                        <asp:ListItem Value="48" meta:resourcekey="ListItemResource12" Text="2 Days"></asp:ListItem>
                                                                                        <asp:ListItem Value="72" meta:resourcekey="ListItemResource13" Text="3 Days"></asp:ListItem>
                                                                                        <asp:ListItem Value="168" meta:resourcekey="ListItemResource14" Text="Week"></asp:ListItem>
                                                                                        <asp:ListItem Value="730" meta:resourcekey="ListItemResource15" Text="1 Month"></asp:ListItem>
                                                                                        <asp:ListItem Value="1460" meta:resourcekey="ListItemResource16" Text="2 Month"></asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkDriverDoorExc" runat="server" CssClass="formtext" Text="Driver Door"
                                                                                        meta:resourcekey="chkDriverDoorExcResource1"></asp:CheckBox></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkPassengerDoorExc" runat="server" CssClass="formtext" Text="Passenger Door"
                                                                                        meta:resourcekey="chkPassengerDoorExcResource1"></asp:CheckBox></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkSideHopperDoorExc" runat="server" CssClass="formtext" Text="Side Hopper Door"
                                                                                        meta:resourcekey="chkSideHopperDoorExcResource1"></asp:CheckBox></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:CheckBox ID="chkRearHopperDoorExc" runat="server" CssClass="formtext" Text="Rear Hopper Door"
                                                                                        meta:resourcekey="chkRearHopperDoorExcResource1"></asp:CheckBox></td>
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

                                                            <table class="formtext" cellpadding=0 cellspacing=0   >
                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker1" Visible=False   runat="server" Text="Locker 1" 
                                                                        meta:resourcekey="chkLocker1Resource1" />
                                                                    </td>
                                                                
                                                                </tr>
                                                                
                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker2" Visible=False runat="server" Text="Locker 2" 
                                                                        meta:resourcekey="chkLocker2Resource1" />
                                                                    </td>
                                                                
                                                                </tr>

                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker3" Visible=False runat="server" Text="Locker 3" 
                                                                        meta:resourcekey="chkLocker3Resource1" />
                                                                    </td>
                                                                
                                                                </tr>

                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker4" Visible=False runat="server" Text="Locker 4" 
                                                                        meta:resourcekey="chkLocker4Resource1" />
                                                                    </td>
                                                                
                                                                </tr>

                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker5" Visible=False runat="server" Text="Locker 5" 
                                                                        meta:resourcekey="chkLocker5Resource1" />
                                                                    </td>
                                                                
                                                                </tr>

                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker6" Visible=False runat="server" Text="Locker 6" 
                                                                        meta:resourcekey="chkLocker6Resource1" />
                                                                    </td>
                                                                
                                                                </tr>

                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker7" Visible=False runat="server" Text="Locker 7" 
                                                                        meta:resourcekey="chkLocker7Resource1" />
                                                                    </td>
                                                                
                                                                </tr>

                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker8" Visible=False runat="server" Text="Locker 8" 
                                                                        meta:resourcekey="chkLocker8Resource1" />
                                                                    </td>
                                                                
                                                                </tr>

                                                                <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkLocker9" Visible=False runat="server" Text="Locker 9" 
                                                                        meta:resourcekey="chkLocker9Resource1" />
                                                                    </td>
                                                                
                                                                </tr>
                                                            </table> 
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table id="Table2" cellspacing="0" cellpadding="0" border="0">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <asp:CheckBox ID="chkSOSMode" runat="server" CssClass="formtext" Width="114px" Text="More than"
                                                                            meta:resourcekey="chkSOSModeResource1"></asp:CheckBox></td>
                                                                    <td valign="top">
                                                                        <asp:DropDownList ID="cboSOSLimit" runat="server" CssClass="RegularText" meta:resourcekey="cboSOSLimitResource1">
                                                                            <asp:ListItem Value="3" meta:resourcekey="ListItemResource17" Text="3"></asp:ListItem>
                                                                            <asp:ListItem Value="6" meta:resourcekey="ListItemResource18" Text="6"></asp:ListItem>
                                                                            <asp:ListItem Value="9" meta:resourcekey="ListItemResource19" Text="9"></asp:ListItem>
                                                                            <asp:ListItem Value="18" meta:resourcekey="ListItemResource20" Text="18"></asp:ListItem>
                                                                            <asp:ListItem Value="24" meta:resourcekey="ListItemResource21" Text="24"></asp:ListItem>
                                                                            <asp:ListItem Value="48" meta:resourcekey="ListItemResource22" Text="48"></asp:ListItem>
                                                                            <asp:ListItem Value="72" meta:resourcekey="ListItemResource23" Text="72"></asp:ListItem>
                                                                        </asp:DropDownList></td>
                                                                    <td class="formtext">
                                                                    </td>
                                                                    <td class="formtext">
                                                                    </td>
                                                                    <td class="formtext" valign="top">
                                                                        <asp:Label ID="lblSOSModesTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSOSModesTitleResource1"
                                                                            Text="SOS modes"></asp:Label></td>
                                                                    <td class="formtext">
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkTAR" runat="server" CssClass="formtext" Width="165px" Text="Any TAR mode events"
                                                                meta:resourcekey="chkTARResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkImmobilization" runat="server" CssClass="formtext" Width="211px"
                                                                Text="Vehicle immobilization events" meta:resourcekey="chkImmobilizationResource1">
                                                            </asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkDriverDoor" runat="server" CssClass="formtext" Width="292px"
                                                                Text="15 seconds driver/passenger door violation" meta:resourcekey="chkDriverDoorResource1">
                                                            </asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkLeash" runat="server" CssClass="formtext" Width="270px" Text="50% of Leash Event"
                                                                meta:resourcekey="chkLeashResource1"></asp:CheckBox></td>
                                                    </tr>
                                                </table>
                                                <table id="tblHistoryOptions" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkHistIncludeCoordinate" runat="server" CssClass="formtext" Text="Include Coordinates"
                                                                Checked="True" meta:resourcekey="chkHistIncludeCoordinateResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkHistIncludeSensors" runat="server" CssClass="formtext" Width="196px"
                                                                Text="Include Sensors" Checked="True" meta:resourcekey="chkHistIncludeSensorsResource1">
                                                            </asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkHistIncludeInvalidGPS" runat="server" AutoPostBack="True" CssClass="formtext"
                                                                Width="280px" Text="Include Invalid GPS for coordinate messages" Checked="True"
                                                                meta:resourcekey="chkHistIncludeInvalidGPSResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkHistIncludePositions" runat="server" CssClass="formtext" Width="261px"
                                                                Text="Include Position" Checked="True" Visible="False" meta:resourcekey="chkHistIncludePositionsResource1">
                                                            </asp:CheckBox></td>
                                                    </tr>
                                                </table>
                                                <table id="tblOptions1" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkIncludeStreetAddress" runat="server" CssClass="formtext" Text="Include Street Address"
                                                                Checked="True" meta:resourcekey="chkIncludeStreetAddressResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkIncludeSensors" runat="server" CssClass="formtext" Width="198px"
                                                                Text="Include Sensors" Checked="True" meta:resourcekey="chkIncludeSensorsResource1">
                                                            </asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkIncludePosition" runat="server" CssClass="formtext" Width="211px"
                                                                Text="Include Position" Checked="True" meta:resourcekey="chkIncludePositionResource1">
                                                            </asp:CheckBox></td>
                                                    </tr>
                                                </table>
                                                <asp:CheckBox ID="chkShowStorePosition" runat="server" CssClass="formtext" Width="257px"
                                                    Text="Show Stored Position" meta:resourcekey="chkShowStorePositionResource1"></asp:CheckBox>

                                                 <asp:CheckBox ID="chkShowDriver" runat="server" CssClass="formtext" Width="257px"
                                                    Text="Display Driver" meta:resourcekey="chkShowDriverResource1"></asp:CheckBox>

                                                <table id="tblStopReport" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        <td class="formtext">
                                                            &nbsp;<asp:Label ID="lblStopDurationTitle" runat="server" CssClass="formtext" meta:resourcekey="lblStopDurationTitleResource1"
                                                                Text="Stop Duration:"></asp:Label></td>
                                                        <td>
                                                            <asp:DropDownList ID="cboStopSequence" runat="server" CssClass="formtext" meta:resourcekey="cboStopSequenceResource1">
                                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource24" Text="Not Filtered"></asp:ListItem>
                                                                <asp:ListItem Value="300" meta:resourcekey="ListItemResource25" Text="Longer than 5 Min"></asp:ListItem>
                                                                <asp:ListItem Value="600" meta:resourcekey="ListItemResource26" Text="Longer than 10 Min"></asp:ListItem>
                                                                <asp:ListItem Value="900" meta:resourcekey="ListItemResource27" Text="Longer than 15 Min"></asp:ListItem>
                                                                <asp:ListItem Value="1200"  Text="Longer than 20 Min" 
                                                                    meta:resourcekey="ListItemResource63"></asp:ListItem>
                                                                <asp:ListItem Value="1500"  Text="Longer than 25 Min" 
                                                                    meta:resourcekey="ListItemResource64"></asp:ListItem>
                                                                <asp:ListItem Value="1800" meta:resourcekey="ListItemResource28" Text="Longer than 30 Min"></asp:ListItem>
                                                                <asp:ListItem Value="2700" meta:resourcekey="ListItemResource29" Text="Longer than 45 Min"></asp:ListItem>
                                                                <asp:ListItem Value="3600" meta:resourcekey="ListItemResource30" Text="Longer than 1 Hour"></asp:ListItem>
                                                                <asp:ListItem Value="7200" meta:resourcekey="ListItemResource31" Text="Longer than 2 Hours"></asp:ListItem>
                                                                <asp:ListItem Value="43200" meta:resourcekey="ListItemResource32" Text="Longer than 12 Hours"></asp:ListItem>
                                                                <asp:ListItem Value="86400" meta:resourcekey="ListItemResource33" Text="Longer than 24 Hours"></asp:ListItem>
                                                                <asp:ListItem Value="172800" meta:resourcekey="ListItemResource34" Text="Longer than 48 Hours"></asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" class="formtext">
                                                            <asp:RadioButtonList ID="optStopFilter" runat="server" CssClass="formtext" meta:resourcekey="optStopFilterResource1">
                                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource35" Text="Show Stops Only"></asp:ListItem>
                                                                <asp:ListItem Value="1" meta:resourcekey="ListItemResource36" Text="Show Idlings Only"></asp:ListItem>
                                                                <asp:ListItem Selected="True" Value="2" meta:resourcekey="ListItemResource37" Text="Show Stops and Idlings"></asp:ListItem>
                                                            </asp:RadioButtonList></td>
                                                    </tr>
                                                </table>
                                                <table id="tblViolationReport" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        <td colspan="2" class="formtext">
                                                            <table class="formtext" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <asp:CheckBox ID="chkSpeedViolation" runat="server" Text="Speed Violation" Checked="True"
                                                                            meta:resourcekey="chkSpeedViolationResource1" /></td>
                                                                    <td>
                                                                        &nbsp;&nbsp;&nbsp;
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="cboViolationSpeed" runat="server" CssClass="formtext" 
                                                                            meta:resourcekey="cboViolationSpeedResource1">
                                                                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource65">100 kph (62 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="2" meta:resourcekey="ListItemResource66">105 kph (65 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="3" meta:resourcekey="ListItemResource67">110 kph (68 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="4" meta:resourcekey="ListItemResource68">120 kph (75 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="5" meta:resourcekey="ListItemResource69">125 kph (77 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="6" meta:resourcekey="ListItemResource70">130 kph (80 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="7" meta:resourcekey="ListItemResource71">140 kph (90 mph)</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <asp:CheckBox ID="chkHarshAcceleration" runat="server" Text="Harsh Acceleration"
                                                                Checked="True" meta:resourcekey="chkHarshAccelerationResource1" /><br />
                                                            <asp:CheckBox ID="chkHarshBraking" runat="server" Text="Harsh Braking" Checked="True"
                                                                meta:resourcekey="chkHarshBrakingResource1" /><br />
                                                            <asp:CheckBox ID="chkExtremeAcceleration" runat="server" Text="Extreme Acceleration"
                                                                Checked="True" meta:resourcekey="chkExtremeAccelerationResource1" /><br />
                                                            <asp:CheckBox ID="chkExtremeBraking" runat="server" Text="Extreme Braking" Checked="True"
                                                                meta:resourcekey="chkExtremeBrakingResource1" /><br />
                                                            <asp:CheckBox ID="chkSeatBeltViolation" runat="server" Text="SeatBelt Violation"
                                                                Checked="True" meta:resourcekey="chkSeatBeltViolationResource1" /><br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                </table>



                                                 <table id="tblRoadSpeed" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        
                                                                    <td colspan=2 >
                                                                        <asp:CheckBox ID="chkIsPostedOnly"  runat="server" 
                                                                            Text="Posted Road Speed Only" class="formtext" TextAlign="Left" meta:resourcekey="chkIsPostedOnlyResource1"
                                                                           /></td>
                                                      </tr>
                                                      <tr>
                                                                    <td>

                                                                         <asp:Label ID="lblRoadSpeedDelta" runat="server" Text="Speed Over Road Speed:" 
                                                                             CssClass="formtext" meta:resourcekey="lblRoadSpeedDeltaResource1"></asp:Label>
                                                                    </td>
                                                                    <td class="style3">
                                                                        <asp:DropDownList ID="cboRoadSpeedDelta" runat="server" CssClass="formtext" 
                                                                            meta:resourcekey="cboRoadSpeedDeltaResource1">
                                                                            <asp:ListItem Value="10" meta:resourcekey="ListItemResource72">10 kph (6 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="20" meta:resourcekey="ListItemResource73">20 kph (13 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="25" meta:resourcekey="ListItemResource74">25 kph (16 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="30" meta:resourcekey="ListItemResource75">30 kph (19 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="35" meta:resourcekey="ListItemResource76">35 kph (22 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="40" meta:resourcekey="ListItemResource77">40 kph (25 mph)</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                             
                                                    </tr>
                                                </table>


                                                <div id="tblLandmarkOptions" runat="server" style="padding: 10px 10px 10px 0px">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblLandmarkCaption" runat="server" Text="Landmark:" 
                                                                    CssClass="heading" meta:resourcekey="lblLandmarkCaptionResource1"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlLandmarks" runat="server" CssClass="RegularText"
                                                                    DataTextField="LandmarkName" DataValueField="LandmarkName" Width="342px" 
                                                                    meta:resourcekey="ddlLandmarksResource1">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div id="tblGeozoneOptions" runat="server" style="padding: 10px 10px 10px 0px">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblGeozoneCaption" runat="server" Text="Geozone:" 
                                                                    CssClass="formtext" meta:resourcekey="lblGeozoneCaptionResource1"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlGeozones" runat="server" CssClass="RegularText"
                                                                    DataTextField="GeozoneName" DataValueField="GeozoneNo" Width="355px" 
                                                                    meta:resourcekey="ddlGeozonesResource1">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div id="tblDriverOptions" runat="server" style="padding: 10px 10px 10px 0px">
                                                    <table>
                                                        <tr>
                                                            <td class="style1">
                                                                <asp:Label ID="lblDriverCaption" runat="server" Text="Driver:" 
                                                                    CssClass="formtext" meta:resourcekey="lblDriverCaptionResource1"></asp:Label>
                                                            </td>
                                                            <td>
                                                                &nbsp;<asp:DropDownList ID="ddlDrivers" runat="server" CssClass="RegularText"
                                                                    DataTextField="FullName" DataValueField="DriverId" Width="358px" 
                                                                    meta:resourcekey="ddlDriversResource1" >
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>




                                    <table id="tblFleetMaintenance" runat="server" class="formtext" border="0" cellpadding="0"
                                        cellspacing="0">
                                        <tr id="maintenanceVehicleSelectOption" runat="server" visible="false">
                                            <td  class="formtext"   colspan="4">
                                                <table class="formtext" cellpadding=0 cellspacing=0  >
                                                    <tr>
                                                        <td> <asp:Label ID="Label11" runat="server" 
                                                                meta:resourcekey="lblCreateReportBasedOnResource1" 
                                                                Text="Create Report Based On: "></asp:Label> </td>
                                                        <td>
                                                                    <asp:RadioButtonList ID="optMaintenanceBased" name="ReportBased" runat="server"  class="formtext" 
                                                    RepeatDirection="Horizontal" AutoPostBack="true"
                                                    onselectedindexchanged="optMaintenanceBased_SelectedIndexChanged" >
                                                    <asp:ListItem id="maintenanceOH" name="raVehicleSelectOption" value="0"  Selected="True"  meta:resourcekey="ListItemResource78"
                                                        runat="server">Organization Hierarchy</asp:ListItem> 
                                                    <asp:ListItem id="maintenanceFleet" type="radio" name="raVehicleSelectOption" value="1" 
                                                        runat="server" Selected="False" meta:resourcekey="ListItemResource79" 
                                                        >Fleet</asp:ListItem> 

                                                </asp:RadioButtonList> 
                                                        
                                                        </td>
                                                    </tr>
                                                </table>                                               
                                                                                   
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tableheading" style="width: 63px">
                                            </td>
                                            <td class="tableheading">
                                            </td>
                                            <td class="tableheading">
                                                &nbsp;</td>
                                            <td class="style2">
                                                &nbsp;</td>
                                        </tr>
                                        <tr id="trMaintenanceFleet" runat="server">
                                            <td align="left" valign="top" style="width: 63px">
                                                <asp:Label ID="Label6" runat="server" CssClass="tableheading" meta:resourcekey="Label6Resource1"
                                                    Text="Fleet:"></asp:Label></td>
                                            <td align="left" valign="top">
                                                <asp:DropDownList ID="cboMaintenanceFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                    DataTextField="FleetName" DataValueField="FleetId"
                                                    meta:resourcekey="cboMaintenanceFleetResource1">
                                                </asp:DropDownList></td>
                                            <td align="left" valign="top">
                                                &nbsp;</td>
                                            <td align="left" valign="top" class="style2">
                                                &nbsp;</td>
                                        </tr>
                                        <tr id="trMaintenanceHierarchy" style="height: 30px" runat="server" visible="false">
                                            <td align="left" valign="top" style="width: 63px">
                                                <asp:Label ID="Label12" runat="server" CssClass="formtextGreen" meta:resourcekey="ListItemResource78" 
                                                    Text="Organization Hierarchy:"></asp:Label>
                                            </td>
                                            <td align="left" vaign="top">
                                                <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                                    CssClass="combutton" Width="300px" 
                                                    OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 13px; width: 63px;">
                                            </td>
                                            <td style="height: 13px">
                                            </td>
                                            <td style="height: 13px">
                                                &nbsp;</td>
                                            <td class="style3">
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td align="left" valign="top" style="width: 63px">
                                                <asp:Label ID="Label7" runat="server" CssClass="tableheading" meta:resourcekey="Label7Resource1"
                                                    Text="Format:"></asp:Label></td>
                                            <td align="left" valign="top">
                                                <asp:DropDownList ID="cboFleetReportFormat" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                    meta:resourcekey="cboFleetReportFormatResource1">
                                                    <asp:ListItem Selected="True" Value="1" meta:resourcekey="ListItemResource54" Text="PDF"></asp:ListItem>
                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource55" Text="Excel"></asp:ListItem>
                                                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource56" Text="Word"></asp:ListItem>
                                                </asp:DropDownList></td>
                                            <td align="left" valign="top">
                                                &nbsp;</td>
                                            <td align="left" valign="top" class="style2">
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 14px; width: 63px;">
                                            </td>
                                            <td align="left" style="height: 14px">
                                            </td>
                                            <td align="left" style="height: 14px">
                                                &nbsp;</td>
                                            <td align="left" class="style4">
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td align="right" style="width: 63px">
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="cmdPreviewFleetMaintenanceReport" runat="server" CausesValidation="False"
                                                    CssClass="combutton" OnClick="cmdPreviewFleetMaintenanceReport_Click" Text="Preview"
                                                    meta:resourcekey="cmdPreviewFleetMaintenanceReportResource1" />
                                                &nbsp;&nbsp;<asp:Button ID="cmdSchedule0" runat="server" CssClass="combutton" OnClick="cmdSchedule_Click"
                                                                Text="Schedule Report" Width="178px" 
                                                    meta:resourcekey="cmdScheduleResource1" />&nbsp;</td>
                                            <td align="right">
                                                &nbsp;</td>
                                            <td align="right" class="style2">
                                                            <asp:Button ID="cmdViewScheduled0" runat="server" 
                                                    CssClass="combutton" OnClick="cmdViewScheduled_Click"
                                                                Text="View Schedule Reports" Width="178px" meta:resourcekey="cmdViewScheduledResource1"
                                                                CausesValidation="False" />
                                            </td>
                                        </tr>
                                    </table>
                                            </td>
                                            <td style="width: 30px;">
                                            </td>
                                            <td align="left" valign="top">
                                                <table id="tblOptions2" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkIncludeIdleTime" runat="server" CssClass="formtext" Text="Include Idle Time"
                                                                Checked="True" meta:resourcekey="chkIncludeIdleTimeResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkIncludeSummary" runat="server" CssClass="formtext" Text="Include Summary"
                                                                Checked="True" meta:resourcekey="chkIncludeSummaryResource1"></asp:CheckBox></td>
                                                    </tr>
                                                </table>
                                                
                                                       <fieldset id="tblOffHours" runat="server" style="padding: 5px 5px 5px 5px;">
                                                            <legend>
                                                                <asp:Label ID="lblWorkingHours" runat="server" CssClass="formtext" 
                                                                Text="Regular Business Hours" meta:resourcekey="lblWorkingHoursResource1"></asp:Label>
                                                            </legend> 
                                                            
                                                                   <table class="formtext" id="Table6" cellspacing="0" cellpadding="0" border="0"
                                                    runat="server">
                                                    
                                                                                                         
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            &nbsp;</td>
                                                        <td align="right" valign="top">
                                                            &nbsp;</td>
                                                        <td colspan="3" align="left" valign="top">
                                                            &nbsp;</td>
                                                    </tr>
                                                    
                                                                                                         
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <asp:Label ID="lblMonToFriTitle" runat="server" meta:resourcekey="lblMonToFriTitleResource1"
                                                                Text="Monday-Friday:"></asp:Label></td>
                                                        <td align="right" valign="top">
                                                            <asp:Label ID="lblFromTitle" runat="server" meta:resourcekey="lblFromTitleResource1"
                                                                Text="From:"></asp:Label></td>
                                                        <td colspan="3" align="left" valign="top">
                                                            <asp:DropDownList ID="cboFromDayH" runat="server" CssClass="RegularText" meta:resourcekey="cboFromDayHResource1">
                                                            </asp:DropDownList>:<asp:DropDownList ID="cboFromDayM" runat="server" CssClass="RegularText"
                                                                meta:resourcekey="cboFromDayMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource38" Text="00"></asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource39" Text="15"></asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource40" Text="30"></asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource41" Text="45"></asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                        </td>
                                                        <td align="right" valign="top">
                                                            <asp:Label ID="lblToTitle" runat="server" meta:resourcekey="lblToTitleResource1"
                                                                Text="To:"></asp:Label></td>
                                                        <td colspan="3" align="left" valign="top">
                                                            <asp:DropDownList ID="cboToDayH" runat="server" CssClass="RegularText" meta:resourcekey="cboToDayHResource1">
                                                            </asp:DropDownList>:<asp:DropDownList ID="cboToDayM" runat="server" CssClass="RegularText"
                                                                meta:resourcekey="cboToDayMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource42" Text="00"></asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource43" Text="15"></asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource44" Text="30"></asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource45" Text="45"></asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3">
                                                            <asp:CheckBox ID="chkWeekend" runat="server" AutoPostBack="True" Width="184px" Text="Not operated on weekends"
                                                                TextAlign="Left" OnCheckedChanged="chkWeekend_CheckedChanged" meta:resourcekey="chkWeekendResource1">
                                                            </asp:CheckBox></td>
                                                        <td>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td height="2">
                                                            &nbsp;</td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                        <td height="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <asp:Label ID="lblWeekendTitle" runat="server" meta:resourcekey="lblWeekendTitleResource1"
                                                                Text="Weekend:"></asp:Label></td>
                                                        <td align="right" valign="top">
                                                            <asp:Label ID="lblFromTitle2" runat="server" meta:resourcekey="lblFromTitle2Resource1"
                                                                Text="From:"></asp:Label></td>
                                                        <td colspan="3" align="left" valign="top">
                                                            <asp:DropDownList ID="cboWeekEndFromH" runat="server" CssClass="RegularText" meta:resourcekey="cboWeekEndFromHResource1">
                                                            </asp:DropDownList>:<asp:DropDownList ID="cboWeekEndFromM" runat="server" CssClass="RegularText"
                                                                meta:resourcekey="cboWeekEndFromMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource46" Text="00"></asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource47" Text="15"></asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource48" Text="30"></asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource49" Text="45"></asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                        </td>
                                                        <td align="right" valign="top">
                                                            <asp:Label ID="lblToTitle2" runat="server" meta:resourcekey="lblToTitle2Resource1" Text="To:"></asp:Label>
                                                        </td>
                                                        <td colspan="3" align="left" valign="top">
                                                            <asp:DropDownList ID="cboWeekEndToH" runat="server" CssClass="RegularText" meta:resourcekey="cboWeekEndToHResource1">
                                                            </asp:DropDownList>:<asp:DropDownList ID="cboWeekEndToM" runat="server" CssClass="RegularText"
                                                                meta:resourcekey="cboWeekEndToMResource1">
                                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource50" Text="00"></asp:ListItem>
                                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource51" Text="15"></asp:ListItem>
                                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource52" Text="30"></asp:ListItem>
                                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource53" Text="45"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                                
                                                            </fieldset> 
                                                         
                                                <table id="tblException1" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkExcBattery" runat="server" CssClass="formtext" Text="Main battery and backup battery"
                                                                meta:resourcekey="chkExcBatteryResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkExcTamper" runat="server" CssClass="formtext" Text="Tamper events"
                                                                meta:resourcekey="chkExcTamperResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkExcPanic" runat="server" CssClass="formtext" Text="Any panic events"
                                                                meta:resourcekey="chkExcPanicResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkExcKeypad" runat="server" CssClass="formtext" Text="3 keypad/card attempts incorrect"
                                                                meta:resourcekey="chkExcKeypadResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkExcGPS" runat="server" CssClass="formtext" Text="Alt GPS antenna"
                                                                meta:resourcekey="chkExcGPSResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkExcAVL" runat="server" CssClass="formtext" Text="Controller Status"
                                                                meta:resourcekey="chkExcAVLResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkExcLeash" runat="server" CssClass="formtext" Text="Leash broken"
                                                                meta:resourcekey="chkExcLeashResource1"></asp:CheckBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkIncCurTARMode" runat="server" CssClass="formtext" Text="Include current TAR mode status"
                                                                meta:resourcekey="chkIncCurTARModeResource1" /></td>
                                                    </tr>
                                                </table>
                                                <fieldset runat=server id="tblPoints" style="width: 300px"    >
                                                <table runat="server" class="formtext">
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed120" runat="server" CssClass="formtext" Text="Speed >" meta:resourcekey="lblSpeed120Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed120" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed120Resource1"
                                                                            Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="Label9" runat="server" CssClass="formtext" Text="Acc. Harsh" meta:resourcekey="Label9Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAccHarsh" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccHarshResource1"
                                                                            Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblBrakingExtreme" runat="server" CssClass="formtext" Text="Braking Extreme"
                                                                            meta:resourcekey="lblBrakingExtremeResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBrakingExtreme" runat="server" CssClass="formtext" Width="30px"
                                                                            meta:resourcekey="txtBrakingExtremeResource1" Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed130" runat="server" Text="Speed >" meta:resourcekey="lblSpeed130Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed130" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed130Resource1"
                                                                            Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblAccExtreme" runat="server" Text="Acc. Extreme" meta:resourcekey="lblAccExtremeResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAccExtreme" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccExtremeResource1"
                                                                            Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSeatBelt" runat="server" Text="Seat Belt" meta:resourcekey="lblSeatBeltResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSeatBelt" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSeatBeltResource1"
                                                                            Text="50"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed140" runat="server" Text="Speed >" meta:resourcekey="lblSpeed140Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed140" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed140Resource1"
                                                                            Text="50"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblBrakingHarsh" runat="server" CssClass="formtext" Text="Braking Harsh"
                                                                            meta:resourcekey="lblBrakingHarshResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBrakingHarsh" runat="server" CssClass="formtext" Width="30px"
                                                                            meta:resourcekey="txtBrakingHarshResource1" Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                                </fieldset> 
                                                <fieldset id="tblIgnition" runat="server" style="width:200px " >
                                                    <legend>
                                                         <asp:Label ID="lblIgnition" CssClass="formtext"  runat="server" meta:resourcekey="lblIgnitionResource1"
                                                                            Text="Calculate Trips based on:"></asp:Label>
                                                    </legend> 
                                                
                                                          <table  border="0" cellpadding="0" cellspacing="0"
                                                    visible="true">
                                                    <tr>
                                                        <td class="formtext" colspan="2" align="left">
                                                            <table>
                                                                <tr>
                                                                    <td class="formtext">
                                                                       </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left">
                                                                        <asp:RadioButtonList ID="optEndTrip" runat="server" CssClass="formtext" meta:resourcekey="optEndTripResource1">
                                                                            <asp:ListItem Selected="True" Text="Ignition" Value="3" meta:resourcekey="ListItemResource60"></asp:ListItem>
                                                                            <asp:ListItem Text="Tractor Power" Value="11" meta:resourcekey="ListItemResource61"></asp:ListItem>
                                                                            <asp:ListItem Value="8" meta:resourcekey="ListItemResource62">PTO</asp:ListItem>
                                                                        </asp:RadioButtonList>
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
                                </td>
                            </tr>

                            <tr>
                                <%--<td class="RegularText" style="width: 28px">
                            </td>--%>
                                <td colspan="2" align="left" valign="top" style="padding-left: 15px;">
                                </td>
                                <td style="width: 10px">
                                </td>
                                <td>
                                </td>
                            </tr>
                            
                            <tr>
                                <td colspan="5">
                                    <table width="100%" id="tblGeneralCriteria" runat="server">

                                        <tr>
                                            <td align="left" class="style4">
                                            </td>
                                            <td align="left" class="tableheading" style="width: 52px;">
                                            </td>
                                            <td style="width: 280px;" align="left">
                                                <table>
                                                    <tr>
                                                        <td style="width: 100px; height: 21px;">
                                                            <asp:Label ID="Label1" runat="server" CssClass="formtext" Width="172px" Font-Bold="True"
                                                                meta:resourcekey="Label1Resource1" Text="Day"></asp:Label></td>
                                                        <td style="width: 24px; height: 21px;">
                                                        </td>
                                                        <td style="width: 100px; height: 21px;" align="left">
                                                            <asp:Label ID="Label2" runat="server" CssClass="formtext" Width="44px" Font-Bold="True"
                                                                meta:resourcekey="Label2Resource1" Text="Time"></asp:Label></td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="left" class="tableheading" style="width: 80px;">
                                            </td>
                                            <td align="left">
                                                <table>
                                                    <tr>
                                                        <td style="width: 100px; height: 21px">
                                                            <asp:Label ID="Label4" runat="server" CssClass="formtext" Font-Bold="True" Width="172px"
                                                                meta:resourcekey="Label4Resource1" Text="Day"></asp:Label></td>
                                                        <td style="width: 24px; height: 21px">
                                                        </td>
                                                        <td style="width: 100px; height: 21px" align="left">
                                                            <asp:Label ID="Label5" runat="server" CssClass="formtext" Font-Bold="True" Width="44px"
                                                                meta:resourcekey="Label5Resource1" Text="Time"></asp:Label></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        
                                        <tr id="datetime">
                                            <td class="style4"></td>
                                            <td class="tableheading" style="width: 52px" align="left" >
                                                <asp:Label ID="lblFromTitle3" runat="server" CssClass="formtextGreen" meta:resourcekey="lblFromTitle3Resource1"
                                                    Text="From:"></asp:Label>
                                            </td>
                                            <td style="width: 280px" align="left" valign="top">
                                                <table>
                                                    <tr>
                                                        <td style="width: 100px">
                                                            <telerik:RadDatePicker ID="txtFrom" runat="server" Width="162px" DateInput-EmptyMessage=""
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
                                                            <telerik:RadTimePicker ID="cboHoursFrom" runat="server" valign="top" meta:resourcekey="cboHoursFromResource1"
                                                                Skin="Hay" />

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
                                            <td class="tableheading" style="width: 80px" align="right">
                                                <asp:Label ID="lblToTitle3" runat="server" CssClass="formtextGreen" meta:resourcekey="lblToTitle3Resource1"
                                                    Text="To:"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <table>
                                                    <tr>
                                                        <td style="width: 100px">
                                                            <telerik:RadDatePicker ID="txtTo" runat="server" Width="162px" DateInput-EmptyMessage=""
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
                                                            <telerik:RadTimePicker ID="cboHoursTo" runat="server" valign="top" meta:resourcekey="cboHoursFromResource1"
                                                                Skin="Hay" />
                                                        </td>
                                                                                
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
  
                                        <tr>
                                            <td class="style4"></td>
                                            <td style="width: 52px"></td>
                                            <td style="width: 280px"></td>
                                            <td style="width: 80px"></td>
                                            <td style="width: 280px; height: 25px"></td>
                                        </tr>

                                        <tr id="vehicleSelectOption" class="formtext"  runat="server">
                                            <td  class="style5"></td>
                                            <td  class="formtext"   colspan="4">
                                                <table class="formtext" runat=server id="optBaseTable"  cellpadding=0 cellspacing=0  >
                                                    <tr>
                                                        <td> 
                                                            <asp:Label ID="lblCreateReportBasedOn" runat="server" 
                                                                meta:resourcekey="lblCreateReportBasedOnResource1" 
                                                                Text="Create Report Based On: "></asp:Label>
                                                        </td>
                                                        <td>
                                                                 <asp:RadioButtonList ID="optReportBased" name="ReportBased" runat=server  class="formtext" 
                                                    RepeatDirection="Horizontal" AutoPostBack=True  
                                                    onselectedindexchanged="optReportBased_SelectedIndexChanged" 
                                                                     meta:resourcekey="optReportBasedResource1" >
                                                    <asp:ListItem id="Radio1" name="raVehicleSelectOption" value="0" checked 
                                                        runat="server" meta:resourcekey="ListItemResource78">Organization Hierarchy</asp:ListItem> 
                                                    <asp:ListItem id="Radio2" type="radio" name="raVehicleSelectOption" value="1" 
                                                        runat="server" Selected="True" meta:resourcekey="ListItemResource79" 
                                                        >Fleet</asp:ListItem> 

                                                </asp:RadioButtonList> 
                                                        
                                                        </td>
                                                    </tr>
                                                </table>
                                               
                                               <%-- <input id="Radio1" type="radio" name="raVehicleSelectOption" value="1" checked runat="server" onclick="$('#trFleet').hide();$('#organizationHierarchy').show();">Organization Hierarchy
                                                <input id="Radio2" type="radio" name="raVehicleSelectOption" value="2" runat="server" onclick="$('#trFleet').show();$('#organizationHierarchy').hide();">Fleet                                                --%>
                                               
                                            </td>
                                        </tr>

                                        <tr ID="trFleet" runat="server">
                                            <td class="style6">
                                            </td>
                                            <td class="tableheading" style="width: 52px; height: 14px" align="left">
                                                <asp:Label ID="lblFleet" runat="server" CssClass="tableheading" Width="33px" meta:resourcekey="lblFleetResource1"
                                                    Text="Fleet:"></asp:Label><asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="cboFleet"
                                                        ErrorMessage="Please select a Fleet" MinimumValue="1" MaximumValue="999999999999999"
                                                        meta:resourcekey="valFleetResource1" Text="*" Enabled="False"></asp:RangeValidator>
                                            </td>
                                            <td style="width: 280px; height: 14px" align="left">
                                                <asp:DropDownList ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                    Width="258px" DataTextField="FleetName" DataValueField="FleetId"
                                                    OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="formtext" style="text-align: right; padding-right: 4px; width: 80px;">
                                                <asp:Label ID="lblVehicleName" runat="server" CssClass="tableheading"
                                                    Visible="False" meta:resourcekey="lblVehicleNameResource1" Text="Vehicle:"></asp:Label>
                                            </td>
                                            <td style="width: 280px" align="left">
                                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="258px"
                                                    DataTextField="Description" DataValueField="LicensePlate" Visible="False" 
                                                    meta:resourcekey="cboVehicleResource1">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr ID="organizationHierarchy" runat="server">
                                            <td class="style6">
                                            </td>
                                            <td colspan="4" align="left">
                                                <div id="ohsearchbar" class="formtext"><asp:Label ID="Label8" runat="server" 
                                                        CssClass="tableheading" Text="Search Cost Center: " 
                                                        meta:resourcekey="Label8Resource1"></asp:Label>
                                                    <input type="text" id="ohsearchbox" class="ohsearch" />
                                                    <a href="javascript:void(0);" onclick="onsearchbtnclicked();"><img src="../images/searchicon.png" border="0" /></a>
                                                    <asp:Label ID="Label10" runat="server" style="color:#666666;" 
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
                                                                    <th><asp:Label ID="lblHrVehicle" Text='<%$ Resources:lblHrVehicle %>'  runat=server  ></asp:Label>  </th>
                                                                                                                                                                                                     
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

                                                 <div style="font-family: Verdana,sans-serif; font-size: 12px; margin-bottom: 5px;">
                                                    <table border="0">
                                                    <tr><td width="30" valign="top"><asp:Label ID="Label400" runat="server" Text="Path:" class="formtext" meta:resourcekey="Label400Resource1"></asp:Label>
                                                    </td>
                                                    <td id="fleetPath" width="600" height="35" valign="top">
                                                    </td></tr>
                                                    </table>
                                                    <!--<div style="display: inline-block; width: 620px; height: 35px; vertical-align: top; overflow: hidden;" id="Div1"></div>-->
                                                </div>
                                                
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="style4">
                                            </td>
                                            <td style="width: 52px">
                                            </td>
                                            <td style="width: 280px">
                                            </td>
                                            <td style="width: 80px">
                                            </td>
                                            <td style="width: 280px; height: 25px">
                                            </td>
                                        </tr>

                                       
                                        <tr>
                                            <td class="style8">
                                            </td>
                                            <td class="tableheading" style="width: 52px; height: 9px" align="left">
                                                <asp:Label ID="lblFormatTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblFormatTitleResource1"
                                                    Text="Format:"></asp:Label></td>
                                            <td style="width: 280px; height: 9px" align="left">
                                                <asp:DropDownList ID="cboFormat" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                    Width="258px" meta:resourcekey="cboFormatResource1">
                                                    <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource57" Text="PDF"></asp:ListItem>
                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource58" Text="Excel"></asp:ListItem>
                                                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource59" Text="Word"></asp:ListItem>
                                                </asp:DropDownList><br />
                                                <asp:Label ID="lblReportFormat" runat="server" CssClass="formtext" 
                                                    meta:resourcekey="lblReportFormatResource1" Visible="False"></asp:Label></td>
                                            <td style="width: 80px">
                                            </td>
                                            <td style="width: 280px; height: 9px" align="left">
                                                &nbsp;
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="center" colspan="5">
                                                <table style="width: 73%">
                                                    <tr>
                                                        <td style="width: 100px">
                                                        </td>
                                                        <td style="width: 100px">
                                                        </td>
                                                        <td style="width: 100px">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 100px">
                                                            <asp:Button ID="cmdShow" runat="server" CssClass="combutton" Text="Preview" OnClick="cmdShow_Click"
                                                                Width="178px" meta:resourcekey="cmdShowResource1"></asp:Button></td>
                                                        <td style="width: 100px">
                                                            <asp:Button ID="cmdSchedule" runat="server" CssClass="combutton" OnClick="cmdSchedule_Click"
                                                                Text="Schedule Report" Width="178px" meta:resourcekey="cmdScheduleResource1" /></td>
                                                        <td style="width: 100px">
                                                            <asp:Button ID="cmdViewScheduled" runat="server" CssClass="combutton" OnClick="cmdViewScheduled_Click"
                                                                Text="View Schedule Reports" Width="178px" meta:resourcekey="cmdViewScheduledResource1"
                                                                CausesValidation="False" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td style="width: 100%" height="25" align="center" colspan="5">
                                    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1">
                                    </asp:ValidationSummary>
                                                <busyboxdotnet:BusyBox ID="BusyReport" runat="server" AnchorControl="" meta:resourcekey="BusyReportResource1"
                                                    ShowBusyBox="Custom" SlideEasing="BackBoth" Text="Preparing the Report" CompressScripts="False"
                                                    GZipCompression="False"></busyboxdotnet:BusyBox>
                                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="100%" Visible="False"
                                        meta:resourcekey="lblMessageResource1"></asp:Label></td>
                            </tr>

                            <tr>
                                <td align="left" colspan="5" height="25" style="width: 100%">
                                    <table id="tblDesc" align="left" width="761px" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td class="tableheading" style="width: 6px" height="30">
                                            </td>
                                            <td class="tableheading" align="left" height="30">
                                                <b>
                                                    <asp:Label ID="lblReportDescTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblReportDescTitleResource1"
                                                        Text="Report Description:"></asp:Label></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 6px">
                                            </td>
                                            <td>
                                                <asp:Label ID="LabelReportDescription" runat="server" CssClass="formtext" 
                                                    meta:resourcekey="LabelReportDescriptionResource1"></asp:Label>
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
               
                <tr>
                    <td align="left" height="100" style="width: 50%;">
                        <a href="http://www.adobe.com/products/acrobat/readermain.html" target="top">
                            <img height="31" src="../images/get_adobe_reader.gif" align="right" alt="Adobe Reader"
                                border="0" />
                        </a>
                    </td>
                </tr>
           
            </table>
        </div>
    </form>
</body>
</html>
