<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportMasterExtended_new.aspx.cs"
    Inherits="SentinelFM.Reports_frmReportMasterExtended_new" Culture="en-US" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register src="UserControl/frmRepository.ascx" tagname="frmRepository" tagprefix="uc1" %>
<%@ Register src="UserControl/ViewReport.ascx" tagname="ViewReport" tagprefix="uc2" %>
<%@ Register Src="~/Reports/UserControl/frmScheduleReportList.ascx" TagPrefix="uc1" TagName="frmScheduleReportList" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css">
    <link rel="stylesheet" href="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
	<link href="jqueryFileTree.css?v=20140220" rel="stylesheet" type="text/css" media="screen" />
    

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
        .style9
        {
            width: 268435456px;
        }
    </style>
    
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

              
			//-->
    </script>

</head>
<body>
<script type="text/javascript">
    <%if(ShowOrganizationHierarchy && organizationHierarchy.Visible) { %>
            var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
              var PreferOrganizationHierarchyNodeCode = "<%=PreferOrganizationHierarchyNodeCode %>";
              var IniHierarchyPath = <%=IniHierarchyPath.ToString().ToLower() %>;

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
                    , highlightVehicleSelection: false 
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
                    $('#OrganizationHierarchyNodeCode').val(NodeCode);
                    $('#OrganizationHierarchyFleetId').val(FleetId);
                    $('#OrganizationHierarchyBoxId').val('');
                },

                  /*
                  * Call back function when you click right pane vehicle list.
                  */
                function (BoxId) {
                    //$('#OrganizationHierarchyBoxId').val(BoxId);
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
                  try {
                      $("#MySplitter").splitter({
                          type: "v",
                          outline: true,
                          minLeft: 100, sizeLeft: 360, minRight: 100,
                          resizeToWidth: true,
                          //cookie: "vsplitter14",
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
        
    <%} %>

    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
    </script>

    <div style="height: 100%">
        <form id="frmReportMaster" runat="server">
         <input type="hidden" name="OrganizationHierarchyNodeCode" id="OrganizationHierarchyNodeCode" runat="server"/>
         <input type="hidden" name="OrganizationHierarchyFleetId" id="OrganizationHierarchyFleetId" runat="server"/>
         <input type="hidden" name="OrganizationHierarchyBoxId" id="OrganizationHierarchyBoxId" runat="server"/>

        <telerik:RadScriptManager runat="server" ID="RadScriptManager1"  >
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

   
 
    

        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource2">
        </telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1">
     
        </telerik:RadAjaxManager>
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

            function onTabSelecting(sender, args) {
                if (args.get_tab().get_pageViewID()) {
                    args.get_tab().set_postBack(false);
                }
            }

            function DateSelected(sender, args) {
                //alert("Selected Time  : " + args.get_newValue());  
            }  


        </script>
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            <script type="text/javascript">
                var hideButtonID = '';
                function OpenCreateWindow(validationGroup, hideButton) {
                    hideButtonID = ''
                    $telerik.$('#<%= hidSubmitType.ClientID%>').val('');
                    $telerik.$('#<%= lblMessage.ClientID %>').text('');
                    if (!Page_ClientValidate(validationGroup)) {
                        Page_BlockSubmit = false;
                        return false; //not valid return false
                    }
                    var oWnd = window.radopen(null, "UserListDialog");
                    var url = "../ReportsScheduling/frmReportScheduler_new.aspx?rnd=" + Math.random();
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
                    var cboReports = $find("<%= cboReports.ClientID %>");
                    var selectedReport = cboReports.get_value();
                    var cboVehicle = $find("<%= cboVehicle.ClientID %>");
                    if (cboVehicle != null) {
                        if ((cboVehicle.get_selectedIndex() == 0 && selectedReport == "3") ||
                         (cboVehicle.get_selectedIndex() == -1)) {
                            sender.errormessage = "<%= errlblMessage_Text_SelectVehicle%>"
                            args.IsValid = false;
                            return;
                        }
                    }
                    
                    var vehicleSelectOption = $("#<%=vehicleSelectOption.ClientID %>");
                    if (vehicleSelectOption.html() != null && vehicleSelectOption.html() != '') {
                        //var cboFleet = $find("<%= cboFleet.ClientID %>");
                        
                        if (<%=optReportBased.SelectedIndex %>==0) {
                            if ($('#OrganizationHierarchyFleetId').val() == null || $('#OrganizationHierarchyFleetId').val() == '') {
                                sender.errormessage = "<%= errvalHierarchyMessage%>";
                                args.IsValid = false;
                                return;
                            }
                        }
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
        <asp:Panel ID="pnlAll" runat="server" meta:resourcekey="pnlAllResource2">
            <telerik:RadTabStrip ID="RadTabStrip1" SelectedIndex="0" runat="server" MultiPageID="RadMultiPage1"
                 Skin="Hay" meta:resourcekey="RadTabStrip1Resource1" OnClientTabSelected="onTabSelected" >
                <Tabs>
                    <telerik:RadTab runat="server" Text="Create Report" PageViewID="Report" Value ="0" meta:resourcekey="RadTabResource1"
                        Owner="" Selected="True">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="Repository" PageViewID="Repository" Value ="1" meta:resourcekey="RadTabResource2"
                        Owner="">
                    </telerik:RadTab>
                <telerik:RadTab runat="server" Text="Scheduled Report" Value= "3" PageViewID="ScheduledReport" meta:resourcekey="RadTabResource4" >
                </telerik:RadTab>
                    <telerik:RadTab runat="server" Text="View" PageViewID="View" Value ="2" meta:resourcekey="RadTabResource3" Enabled ="false"
                        Owner="">
                    </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" meta:resourcekey="RadMultiPage1Resource2" CssClass="multiPage">
                <telerik:RadPageView ID="Report" runat="server" PageViewID="Report" meta:resourcekey="RadPageView1Resource1"
                    Selected="True">
                    <center>
                        <table width="95%">
                            <asp:Panel id="pnlddlReport" runat ="server" >
                            <tr align="center" style="height: 40px;" >
                                <td colspan="10">
                                    <telerik:RadComboBox ID="ddlReport" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged"
                                        Skin="Hay" meta:resourcekey="ddlReportResource2">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Standard Reports" meta:resourcekey="ddlReportItemResource1"
                                                Value="0" runat="server" Owner=""></telerik:RadComboBoxItem>
                                            <telerik:RadComboBoxItem Text="Extended Reports" meta:resourcekey="ddlReportItemResource2"
                                                Value="1" Selected="True" runat="server" Owner=""></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem  Text="My Reports" Visible ="true" meta:resourcekey="ddlReportItemResource3"
                                            Value="2"></telerik:RadComboBoxItem>
                                            <telerik:RadComboBoxItem  Text="Process Standard Reports" Visible ="true" 
                                            Value="3"></telerik:RadComboBoxItem>
                                        </Items>
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            </asp:Panel>
                            <tr align="center">
                                <td>
                                    <asp:Panel ID="pnlExtended" runat="server" meta:resourcekey="pnlExtendedResource2">
                                     <fieldset>
                                        <table id="Table4" width="100%" cellspacing="0" cellpadding="0" border="0">
                                            <tr>
                                                <td style="width: 100%;" align="center">
                                                    <table id="Table1" style="width: 30%;" cellspacing="0" cellpadding="0" >
                                                        <tr>
                                                            <td colspan="5" style="">
                                                                <table class="formtext">
                                                                    <tr>
                                                                        <td align="right">
                                                                            &nbsp;</td>
                                                                        <td style="width: 100px" align="left" colspan="4">
                                                                            &nbsp;</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right">
                                                                            <asp:Label ID="lblReportTitle" runat="server" CssClass="formtextGreen" 
                                                                                Font-Bold="true" meta:resourcekey="lblReportTitleResource1" 
                                                                                Style="padding-right: 3px" Text="Report:"></asp:Label>
                                                                        </td>
                                                                        <td align="left" colspan="4" style="width: 100px">
                                                                            <telerik:RadComboBox ID="cboReports" runat="server" AutoPostBack="True" 
                                                                                CssClass="RegularText" DataTextField="GuiName" DataValueField="GuiId" 
                                                                                EnableScreenBoundaryDetection="False" MaxHeight="400px" 
                                                                                meta:resourcekey="cboReportsResource1" 
                                                                                OnSelectedIndexChanged="cboReports_SelectedIndexChanged" Skin="Hay" 
                                                                                Width="258px">
                                                                            </telerik:RadComboBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" style="width: 50px">
                                                                        </td>
                                                                        <td colspan="5" align="left">
                                                                            <table cellpadding="2" id="tblSpeedViolation" runat="server"  >
                                                                                <tr id="Tr1" runat="server">
                                                                                    <td id="Td1" runat="server">
                                                                                        <asp:Label ID="lblSpeedViolation" runat="server" Text="Speed:" CssClass="formtextGreen"
                                                                                            meta:resourcekey="lblSpeedViolationResource1"></asp:Label>
                                                                                    </td>
                                                                                    <td id="Td2" runat="server">
                                                                                        <telerik:RadComboBox ID="cboViolationSpeed" runat="server" CssClass="formtext" Skin="Hay">
                                                                                            <Items>
                                                                                                <telerik:RadComboBoxItem Value="1" Text="For Canada" runat="server" meta:resourcekey="cboViolationSpeedItem1Resource1"
                                                                                                    Owner="" />
                                                                                                <telerik:RadComboBoxItem Value="2" Text="For USA" runat="server" Owner="" meta:resourcekey="cboViolationSpeedItem2Resource1" />
                                                                                            </Items>
                                                                                        </telerik:RadComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table id="tblCost" runat="server">
                                                                                <tr id="Tr2" runat="server">
                                                                                    <td id="Td3" runat="server">
                                                                                        <asp:Label ID="lblCost" runat="server" CssClass="formtextGreen" Text="Cost of Unnecessary Idling ($ per Hr):"
                                                                                            meta:resourcekey="lblCostResource1"></asp:Label>
                                                                                    </td>
                                                                                    <td id="Td4" style="width: 20px" runat="server">
                                                                                        <asp:TextBox ID="txtCost" runat="server" CssClass="formtext" Width="30px">1</asp:TextBox>
                                                                                        <asp:RegularExpressionValidator ID="retxtCost" runat="server" ControlToValidate="txtCost"
                                                                                            Display="None" ValidationGroup="vgSubmit" Text="*" ValidationExpression="^\s*-?\d*\.?\d*$"
                                                                                            meta:resourcekey="retxtCostResource1">
                                                                                        </asp:RegularExpressionValidator>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table id="tblFilter" runat="server">
                                                                                <tr id="Tr3" runat="server">
                                                                                    <td id="Td5" runat="server">
                                                                                        <asp:Label ID="Label1" runat="server" CssClass="formtextGreen" Text="Color:" meta:resourcekey="LabelColorResource1"></asp:Label>
                                                                                    </td>
                                                                                    <td id="Td6" style="width: 20px" runat="server">
                                                                                        <asp:TextBox ID="txtColorFilter" runat="server" CssClass="formtext"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table id="tblIgnition" runat="server" border="0" cellpadding="0" cellspacing="0">
                                                                                <tr id="Tr4" runat="server">
                                                                                    <td id="Td7" class="formtext" colspan="2" align="left" runat="server">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td class="formtext">
                                                                                                    <asp:Label ID="lblIgnition" runat="server" meta:resourcekey="lblIgnitionResource1"
                                                                                                        CssClass="formtextGreen" Text="Calculate Trips based on:"></asp:Label>
                                                                                                </td>
                                                                                                <td align="left">
                                                                                                    <asp:RadioButtonList ID="optEndTrip" runat="server" CssClass="formtext" meta:resourcekey="optEndTripResource1"
                                                                                                        RepeatDirection="Horizontal">
                                                                                                        <asp:ListItem Selected="True" Text="Ignition" Value="3" meta:resourcekey="ListItemResource60"></asp:ListItem>
                                                                                                        <asp:ListItem Text="Tractor Power" Value="11" meta:resourcekey="ListItemResource61"></asp:ListItem>
                                                                                                        <asp:ListItem Value="8" meta:resourcekey="ListItemResource62" Text="PTO"></asp:ListItem>
                                                                                                    </asp:RadioButtonList>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table id="tblViolationReport" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                                <tr id="Tr5" runat="server">
                                                                                    <td id="Td8" colspan="2" class="formtext" runat="server">
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
                                                                                                    <telerik:RadComboBox ID="DropDownList1" runat="server" CssClass="formtext" Skin="Hay">
                                                                                                        <Items>
                                                                                                            <telerik:RadComboBoxItem Value="1" Text="100 kph (62 mph)" runat="server" Owner="" />
                                                                                                            <telerik:RadComboBoxItem Value="2" Text="105 kph (65 mph)" runat="server" Owner="" />
                                                                                                            <telerik:RadComboBoxItem Value="3" Text="110 kph (68 mph)" runat="server" Owner="" />
                                                                                                            <telerik:RadComboBoxItem Value="4" Text="120 kph (75 mph)" runat="server" Owner="" />
                                                                                                            <telerik:RadComboBoxItem Value="5" Text="130 kph (80 mph)" runat="server" Owner="" />
                                                                                                            <telerik:RadComboBoxItem Value="6" Text="140 kph (90 mph)" runat="server" Owner="" />
                                                                                                        </Items>
                                                                                                    </telerik:RadComboBox>
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
                                                                                    <td>&nbsp;&nbsp;</td>
                                                                                    <td >
                                                                                       
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table>
                                                                            <tr>
                                                                            <td>
                                                                            <fieldset runat="server" id="tblPoints" style="width: 300px;height: 150px;vertical-align:top; ">
                                                                            <table id="tblViolationPoints" runat="server" class="formtext">
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
                                                                                    <td></td>
                                                                                </tr>
                                                                            </table>
                                                                            </fieldset>
                                                                            </td>
                                                                            <td>&nbsp</td>
                                                                            <td>
                                                                            <fieldset runat=server id="fldScoreCategory" style="width: 300px; height: 150px; vertical-align:top;">
                                                                            <table id="tblScoreCategories" runat="server" class="formtext">

                                                                                <tr>
                                                                                    <td>
                                                                                        
                                                                                          <asp:Label ID="Label2" runat="server" CssClass="formtext" Text="Mileage divider"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtMileageDivider" CssClass="formtext" Text="100"  Width="60px" runat="server" />
                                                                                    </td>
                                                                                </tr>


                                                                                <tr>
                                                                                 <td>
                                                                                        
                                                                                    <asp:Label ID="lblScoreCategory" runat="server" Text="Score Categories"></asp:Label>
                                                                                    </td>

                                                                                    <td valign="top">
                                                                                        
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtScoreA" runat="server" CssClass="formtext" Width="60px" Text="2.99"></asp:TextBox>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Button ID="btnScoreA" BackColor="#00aa00" Width="20px" Height="20px" Enabled="false" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtScoreB" runat="server" CssClass="formtext" Width="60px" Text="4.99"></asp:TextBox>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Button ID="btnScoreB" BackColor="#ff8800" Width="20px" Height="20px" Enabled="false" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtScoreC" runat="server" CssClass="formtext" Width="60px" Text="5+"></asp:TextBox>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Button ID="btnScoreC" BackColor="#ee0000" Width="20px" Height="20px" Enabled="false" runat="server" />
                                                                                     </td>
                                                                                </tr>
                                                                            </table>
                                                                            </fieldset>
                                                                            </td>
                                                                            </tr>
                                                                            </table>
                                                                            <table id="tblDriverOptions" runat="server">
                                                                                <tr>
                                                            <td >
                                                                <asp:Label ID="lblDriverCaption" runat="server" Text="Driver:" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlDrivers" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                    DataTextField="FullName" DataValueField="DriverId" Width="200px"  >
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table id="tblIdlingThreshold" runat="server">
                                                        <tr>
                                                            <td >
                                                                <asp:Label ID="Label3" runat="server" Text="Idling Threshold:" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td>
                                                                 <asp:DropDownList ID="cboIdlingThreshold" runat="server" CssClass="formtext" 
                                                                     Width="140px">
                                                                            <asp:ListItem Value="-1">All</asp:ListItem>
                                                                            <asp:ListItem Value="1">More than 1 Hour</asp:ListItem>
                                                                            <asp:ListItem Value="2">More than 2 Hours</asp:ListItem>
                                                                            <asp:ListItem Value="3">More than 3 Hours</asp:ListItem>
                                                                        </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table id="tblSpeedThreshold" runat="server">
                                                        <tr>
                                                            <td >
                                                                <asp:Label ID="Label4" runat="server" Text="Speed Threshold:" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td>
                                                                 <asp:DropDownList ID="cboSpeedThreshold" runat="server" CssClass="formtext" 
                                                                     Width="140px">
                                                                              <asp:ListItem Value="100">100 kph (62 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="105">105 kph (65 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="110">110 kph (68 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="120">120 kph (75 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="125">125 kph (77 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="130">130 kph (80 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="140">140 kph (90 mph)</asp:ListItem>
                                                                        </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table id="tblMediaType" runat=server class=formtext  ><tr><td valign=top  >
                                                                                            <asp:Label ID="lblMedia" runat="server" Text="Media Type:" ></asp:Label>
                                                                                            </td><td>
                                                                                          
                                                                                                 <asp:RadioButtonList ID="cboMediaType" runat="server" CssClass="formtext" >
        <asp:ListItem Selected="True" Value="93">Rock Salt</asp:ListItem>
        <asp:ListItem Value="99">50:50 Salt/Sand</asp:ListItem>
        <asp:ListItem Value="100">Pelletized Mag / Calc</asp:ListItem>
    </asp:RadioButtonList>

                                                                                            </td></tr></table>
                                                                            <asp:Panel id="pnlOperationalLogs" runat ="server" >
                                                                            <table id="tbOperationalLogs" class="formtext">
                                                                                <tr style="height: 4px;"><td colspan="2"></td></tr>
                                                                                <tr id="trOprLogLabel">
                                                                                    <td id="td21" style="width: 64px; text-align: left; padding-left: 4px;">Modules: </td>
                                                                                    <td style="text-align: left; padding-left: 4px;">
                                                                                        <asp:CheckBox ID="chkFleet" runat="server" CssClass="formtext" Checked="true" AccessKey="2" Text="Fleet" />
                                                                                        <asp:CheckBox ID="chkVehicle" runat="server" CssClass="formtext" Checked="true" AccessKey="3" Text="Vehicle" />
                                                                                        <asp:CheckBox ID="chkDriver" runat="server" CssClass="formtext" Checked="true" AccessKey="4" Text="Driver" />
                                                                                        <asp:CheckBox ID="chkUser" runat="server" CssClass="formtext" Checked="true" AccessKey="1" Text="User" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td id="td22" style="width: 64px; text-align: left; padding-left: 4px;">Actions: </td>
                                                                                    <td style="text-align: left; padding-left: 4px;">
                                                                                        <asp:CheckBox ID="chkUpdate" runat="server" CssClass="formtext" Checked="true" Text="Update" />
                                                                                        <asp:CheckBox ID="chkAssign" runat="server" CssClass="formtext" Checked="true" Text="Add" />
                                                                                        <asp:CheckBox ID="chkDelete" runat="server" CssClass="formtext" Checked="true" Text="Delete" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td id="td23" style="width: 64px; text-align: left; padding-left: 4px;">Updated By: </td>
                                                                                    <td style="text-align: left; padding-left: 4px;">
                                                                                        <asp:DropDownList ID="ddlUpdateUsers" runat="server" Width="128px" AutoPostBack="false">
                                                                                            <asp:ListItem Text="-- All Users --" Value="0" Selected="True"></asp:ListItem>
                                                                                        </asp:DropDownList>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr style="height: 4px;"><td colspan="2"></td></tr>
                                                                            </table>
                                                                            </asp:Panel>
                                                                        </td>
                                                                    </tr>

                                                                    <tr>
                                                                        <td align="left" >
                                                                                            <asp:Label ID="lblLandmarkCaption" runat="server" 
                                                                                Text="Landmark:" CssClass="tableheading"></asp:Label>
                                                                                        </td>
                                                                        <td >

                                                                            <telerik:RadComboBox ID="ddlLandmarks" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                                                DataTextField="LandmarkName" DataValueField="LandmarkName" 
                                                                                Width="258px" EnableScreenBoundaryDetection="False"
                                                                                Skin="Hay" MaxHeight="400px">
                                                                            </telerik:RadComboBox>

                                                                        </td>
                                                                        <td align="left" style="width: 100px">
                                                                            </td>
                                                                        <td align="left" style="width: 100px">
                                                                           </td>
                                                                        <td >
                                                                            </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="left" >
                                                                                            <asp:Label ID="lblGeozoneCaption" runat="server" Text="Geozone:" CssClass="formtext"></asp:Label>
                                                                                        </td>
                                                                        <td >
                                                                            <telerik:RadComboBox ID="ddlGeozones" runat="server" AutoPostBack="True" CssClass="RegularText" EnableViewState="true"
                                                                                DataTextField="GeozoneName" DataValueField="GeozoneNo" 
                                                                                Width="258px" EnableScreenBoundaryDetection="False"
                                                                                Skin="Hay" MaxHeight="400px">
                                                                            </telerik:RadComboBox>

                                                                        </td>
                                                                        <td align="left" style="width: 100px">
                                                                            &nbsp;</td>
                                                                        <td align="left" style="width: 100px">
                                                                            &nbsp;</td>
                                                                        <td >
                                                                            &nbsp;</td>
                                                                    </tr>
                                                                    <tr id="trVoltageThreshold" >
                                                                        <td align="left" ></td>
                                                                        <td colspan="4">
                                                                            <table id="tbVoltageThreshold" style="width: 100%; vertical-align: top;" runat="server">
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lbVoltageThreshold" runat="server" Text="Voltage Thres : " CssClass="RegularText"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:RadioButtonList ID="rbVoltageThreshold" RepeatDirection="Horizontal" runat="server">
                                                                                            <asp:ListItem Text="No special" Value="0" Selected="True" />
                                                                                            <asp:ListItem Text="< 11" Value="1" />
                                                                                            <asp:ListItem Text="11 - 12.5" Value="2" />
                                                                                            <asp:ListItem Text="> 12.5" Value="3" />
                                                                                        </asp:RadioButtonList>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" style="width: 50px">
                                                                        </td>
                                                                        <td colspan="5" align="left" >
                                                                            <table width ="700px">
                                                                                <tr>
                                                                                    <td align="left" style="width: 50px">
                                                                                        <asp:Label ID="lblFromTitle3" runat="server" CssClass="formtextGreen" meta:resourcekey="lblFromTitle3Resource1"
                                                                                            Text="From:"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <telerik:RadDatePicker ID="txtFrom" runat="server" Width="182px"  Skin="Hay" Culture="en-US" meta:resourcekey="txtFromResource2">
                                                                                            <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" ViewSelectorText="x">
                                                                                                <SpecialDays>
                                                                                                    <telerik:RadCalendarDay Repeatable="Today" Date="" meta:resourcekey="RadCalendarDayResource3">
                                                                                                        <ItemStyle CssClass="rcToday" />
                                                                                                    </telerik:RadCalendarDay>
                                                                                                </SpecialDays>
                                                                                            </Calendar>
                                                                                            <DateInput DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="">
                                                                                            </DateInput>
                                                                                            <DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
                                                                                        </telerik:RadDatePicker>
                                                                                        </td>
                                                                                        <td>
                                                                                        <asp:RequiredFieldValidator ID="valFromDate" runat="server" ControlToValidate="txtFrom"
                                                                                            ValidationGroup="vgSubmit" ErrorMessage="Please select a From" meta:resourcekey="valFromDateResource1"
                                                                                            Text="*"></asp:RequiredFieldValidator>

                                                                                    </td>
                                                                                    <td align="left" style="width: 100px">
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:Label ID="lblToTitle3" runat="server" CssClass="formtextGreen" meta:resourcekey="lblToTitle3Resource1"
                                                                                            Text="To:"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <telerik:RadDatePicker ID="txtTo" runat="server" Width="182px" 
                                                                                             Skin="Hay"  Culture="en-US" meta:resourcekey="txtToResource2" >
                                                                                            <Calendar UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" ViewSelectorText="x">
                                                                                                <SpecialDays>
                                                                                                    <telerik:RadCalendarDay Repeatable="Today" Date="" meta:resourcekey="RadCalendarDayResource4">
                                                                                                        <ItemStyle CssClass="rcToday" />
                                                                                                    </telerik:RadCalendarDay>
                                                                                                </SpecialDays>
                                                                                            </Calendar>
                                                                                            <DateInput Culture="en-US" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="">
                                                                                            </DateInput>
                                                                                            <DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
                                                                                            
                                                                                        </telerik:RadDatePicker>
                                                                                        </td>
                                                                                        <td>
                                                                                        <asp:RequiredFieldValidator ID="valToDate" runat="server" ControlToValidate="txtTo"
                                                                                            ValidationGroup="vgSubmit" ErrorMessage="Please select a To" meta:resourcekey="valToDateResource1"
                                                                                            Text="*"></asp:RequiredFieldValidator>

                                                                                        </td>
                                                                                        <td>
                                                                                        <asp:CompareValidator ID="valCompareDates" runat="server" CssClass="errortext" ControlToValidate="txtTo"
                                                                                            ValidationGroup="vgSubmit" ErrorMessage="The From Date should be earlier than the To Date!"
                                                                                            Enabled="true" Type="Date" Operator="GreaterThanEqual" ControlToCompare="txtFrom"
                                                                                            meta:resourcekey="valCompareDatesResource1" Text="*"></asp:CompareValidator>

                                                                                        </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="10">
                                                                                    </td>
                                                                                </tr>


                                                                                  <tr id="vehicleSelectOption" class="formtext"  runat="server">
                                         
                                            <td  class="formtext" colspan="9" align=center >
                                                <table class="formtext" runat=server id="optBaseTable"  cellpadding=0 cellspacing=0  >
                                                    <tr>
                                                        <td> Create Report Based On: </td>
                                                        <td>
                                                                 <asp:RadioButtonList ID="optReportBased" name="ReportBased" runat=server  class="formtext" 
                                                    RepeatDirection="Horizontal" AutoPostBack=true  
                                                    onselectedindexchanged="optReportBased_SelectedIndexChanged" >
                                                    <asp:ListItem id="Radio1" name="raVehicleSelectOption" value="0" 
                                                        runat="server" Selected="True">Organization Hierarchy</asp:ListItem> 
                                                    <asp:ListItem id="Radio2" type="radio" name="raVehicleSelectOption" value="1" 
                                                        runat="server" 
                                                        >Fleet</asp:ListItem> 

                                                </asp:RadioButtonList> 
                                                        
                                                        </td>
                                                    </tr>
                                                </table>
                                               
                                                </td>
                                        </tr>
                                        <tr id="trFleet" runat="server">
                                                                                    <td align="left" style="width: 50px">
                                                                                        <nobr><asp:Label ID="lblFleet" runat="server" CssClass="formtextGreen" 
                                                                                            Width="33px" meta:resourcekey="lblFleetResource1"
                                                                                            Text="Fleet:" Height="16px"></asp:Label><asp:RangeValidator ID="valFleet" runat="server" ControlToValidate="cboFleet"
                                                                                                ValidationGroup="vgSubmit" ErrorMessage="Please select a Fleet" MinimumValue="1"
                                                                                                MaximumValue="999999999999999" meta:resourcekey="valFleetResource1" Text="*"
                                                                                                Enabled="False"></asp:RangeValidator></nobr>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <telerik:RadComboBox ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                                                            Width="258px" DataTextField="FleetName" DataValueField="FleetId" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                                                                            meta:resourcekey="cboFleetResource1" Skin="Hay" MaxHeight="300px">
                                                                                        </telerik:RadComboBox>
                                                                                    </td>
                                                                                    <td align="left" style="width: 100px">
                                                                                    </td>
                                                                                    <td></td>
                                                                                    <td align="left" style="width: 100px"><nobr>
                                                                                        <asp:Label ID="lblVehicleName" runat="server" CssClass="formtextGreen" Width="53px"
                                                                                            Visible="False" meta:resourcekey="lblVehicleNameResource1" Text="Vehicle:"></asp:Label></nobr>
                                                                                    </td>
                                                                                    <td>
                                                                                        <telerik:RadComboBox ID="cboVehicle" runat="server" CssClass="RegularText" Width="258px"
                                                                                            DataTextField="Description" DataValueField="VehicleId" Visible="False" meta:resourcekey="cboVehicleResource1"
                                                                                            Skin="Hay" MaxHeight="300px">
                                                                                        </telerik:RadComboBox>
                                                                                    </td>
                                                                                    <td>
                                                                                    </td>
                                                                                    <td></td>
                                                                                </tr>
                                        <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td align=left   >
                                                                        <asp:CheckBox ID="chkActiveVehicles" runat="server" 
                                                    Text="Active Vehicles Only" Checked="True" CssClass="formtext"/>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                                   
                                                                                <tr>
                                                                                    <td colspan="10" >
                                                                                    </td>
                                                                                </tr>

                                                                                    <tr>
                                                                                    <td colspan="10" >
                                                                                     
                                                                                    </td>
                                                                                </tr>


                                                                                <tr>
                                                                                    <td colspan="10">
                                                                                                     


                                                                                    </td> 
                                                                                </tr>

                                                                                 <tr ID="organizationHierarchy" runat="server">
                                           
                                            <td colspan="10" align="left">
                                                <div id="ohsearchbar" class="formtext"><asp:Label ID="Label8" runat="server" CssClass="tableheading" Text="Search Cost Center: "></asp:Label>
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
                                                                                    <td colspan="10" class="style3">
                                                                                    </td>
                                                                                </tr>


                                                                                <tr>
                                                                                    <td align="right" style="width: 50px">
                                                                                    </td>
                                                                                    <td colspan="10">
                                                                                        <table id="tblFleets" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                                            <tr id="Tr6" runat="server">
                                                                                                <td id="Td9" colspan="2" class="formtext" style="width: 240px" runat="server">
                                                                                                    <asp:Label ID="lblAllFleets" CssClass="formtextGreen" meta:resourcekey="lblAllFleetsResource1"
                                                                                                        runat="server" Text="All Fleets"></asp:Label>
                                                                                                </td>
                                                                                                <td id="Td10" class="style1" runat="server">
                                                                                                    <asp:Label ID="lblSelectedFleets" CssClass="formtextGreen" meta:resourcekey="lblSelectedFleetsResource1"
                                                                                                        runat="server" Text="Selected fleets"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr id="Tr7" runat="server">
                                                                                                <td id="Td11" style="width: 110px" valign="top" runat="server">
                                                                                                    <asp:ListBox ID="lstUnAss" DataValueField="FleetId" DataTextField="FleetName" CssClass="formtext"
                                                                                                        Width="200px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssResource1">
                                                                                                    </asp:ListBox>
                                                                                                </td>
                                                                                                <td id="Td12" style="width: 130px" align="center" valign="top" runat="server">
                                                                                                    <table id="tblAddRemoveBtns" style="width: 75px; height: 99px" cellspacing="0" cellpadding="0"
                                                                                                        width="75" border="0" runat="server">
                                                                                                        <tr id="Tr8" runat="server">
                                                                                                            <td id="Td13" valign="middle" runat="server">
                                                                                                                <asp:Button ID="cmdAdd" CssClass="combutton" runat="server" Text="Add->" OnClick="cmdAdd_Click">
                                                                                                                </asp:Button>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                        <tr id="Tr9" runat="server">
                                                                                                            <td id="Td14" style="height: 20px" runat="server">
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                        <tr id="Tr10" runat="server">
                                                                                                            <td id="Td15" runat="server">
                                                                                                                <asp:Button ID="cmdAddAll" CssClass="combutton" runat="server" Text="Add All->" OnClick="cmdAddAll_Click">
                                                                                                                </asp:Button>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                        <tr id="Tr11" runat="server">
                                                                                                            <td id="TD16" style="height: 20px" runat="server">
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                        <tr id="Tr12" runat="server">
                                                                                                            <td id="Td17" runat="server">
                                                                                                                <asp:Button ID="cmdRemove" CssClass="combutton" runat="server" Text="<-Remove" OnClick="cmdRemove_Click">
                                                                                                                </asp:Button>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                        <tr id="Tr13" runat="server">
                                                                                                            <td id="Td18" style="height: 20px" runat="server">
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                        <tr id="Tr14" runat="server">
                                                                                                            <td id="Td19" runat="server">
                                                                                                                <asp:Button ID="cmdRemoveAll" CssClass="combutton" runat="server" Text="<-Remove All"
                                                                                                                    OnClick="cmdRemoveAll_Click"></asp:Button>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </td>
                                                                                                <td id="Td20" valign="top" runat="server" class="style2">
                                                                                                    <asp:ListBox ID="lstAss" DataValueField="FleetId" DataTextField="FleetName" CssClass="formtext"
                                                                                                        Width="200px" runat="server" SelectionMode="Multiple" Rows="15" DESIGNTIMEDRAGDROP="46">
                                                                                                    </asp:ListBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="left" style="width: 100px">
                                                                                        <asp:Label ID="lblFormatTitle" runat="server" CssClass="formtextGreen" meta:resourcekey="lblFormatTitleResource1"
                                                                                            Text="Format:"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <telerik:RadComboBox ID="cboFormat" runat="server"  CssClass="RegularText"
                                                                                            Width="258px" meta:resourcekey="cboFormatResource1" Skin="Hay">
                                                                                            <Items>
                                                                                                <telerik:RadComboBoxItem Value="1" Selected="True" meta:resourcekey="ListItemResource57"
                                                                                                    Text="PDF" runat="server" Owner="" />
                                                                                                <telerik:RadComboBoxItem Value="2" meta:resourcekey="ListItemResource58" Text="Excel"
                                                                                                    runat="server" Owner="" />
                                                                                                <telerik:RadComboBoxItem Value="3" meta:resourcekey="ListItemResource59" Text="Word"
                                                                                                    runat="server" Owner="" />
                                                                                            </Items>
                                                                                        </telerik:RadComboBox>
                                                                                    </td>
                                                                                    <td style="width: 100px">
                                                                                    </td>
                                                                                    <td style="width: 100px">
                                                                                    </td>
                                                                                    <td style="width: 100px">
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 100px">
                                                                            <asp:Button ID="cmdShowHide" runat="server" OnClick="cmdShow_Click" Style="visibility: hidden"
                                                                                Width="0" Height="0" />
                                                                            <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Width="0" Height="0"
                                                                                Style="visibility: hidden" />
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                        <td style="width: 100px">
                                                                        </td>
                                                                    </tr>
                                                                    <tr align="center">
                                                                        <td colspan="5">
                                                                            <asp:Button ID="cmdShow" OnClick="cmdShow_Click" runat="server" CssClass="combutton"
                                                                                meta:resourcekey="cmdShowResource1" Text="Preview" ValidationGroup="vgSubmit"
                                                                                Width="120px" />

                                                                                    <asp:Button ID="cmdShowMyReport" runat="server" CssClass="combutton" Text="Execute" Width="130px"
                                                                                        OnClick="cmdShowMyReport_Click" Visible="false" meta:resourcekey="cmdShowMyReportResource1" ValidationGroup="vgSubmit">
                                                                                    </asp:Button>

                                                                                    <asp:Button ID="cmdShowMyReportUpdate" runat="server" CssClass="combutton" Text="Execute and Update" Width="130px"
                                                                                        OnClick="cmdShowMyReportUpdate_Click" Visible="false" meta:resourcekey="cmdShowMyReportUpdateResource1" ValidationGroup="vgSubmit">
                                                                                    </asp:Button>


                                                                            <asp:CustomValidator ID="cvDate" runat="server" ClientValidationFunction="CustomValidateDate"
                                                                                EnableClientScript="true" ValidationGroup="vgSubmit" Display="None" ErrorMessage="" />
                                                                        </td>
                                                                        <asp:Panel ID="pnlSchedule" runat="server" Visible="False" meta:resourcekey="pnlScheduleResource1">
                                                                            <td style="width: 100px">
                                                                                <asp:Button ID="cmdSchedule" runat="server" CssClass="combutton" meta:resourcekey="cmdScheduleResource1"
                                                                                    Text="Schedule Report" Width="178px" ValidationGroup="vgSubmit" />
                                                                            </td>
                                                                            <td style="width: 100px">
                                                                                <asp:Button ID="cmdViewScheduled" runat="server" CausesValidation="False" CssClass="combutton"
                                                                                    meta:resourcekey="cmdViewScheduledResource1" Text="View Schedule Reports" Width="178px"
                                                                                    ValidationGroup="vgSubmit" />
                                                                            </td>
                                                                        </asp:Panel>
                                                                    </tr>
                                                                    <tr align="center">
                                                                        <td colspan="5">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1"
                                                                                            ValidationGroup="vgSubmit"></asp:ValidationSummary>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 100%" align="center" colspan="5">
                                                                            <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="240px" Visible="False"
                                                                                meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" style="width: 50px">
                                                                        </td>
                                                                        <td align="left" colspan="4" style="width: 100%">
                                                                            <table id="tblDesc" align="left" width="761px" cellspacing="0" cellpadding="0" border="0">
                                                                                <tr>
                                                                                    <td class="tableheading" align="left" height="20">
                                                                                        <b>
                                                                                            <asp:Label ID="lblReportDescTitle" runat="server" CssClass="formtextGreen" meta:resourcekey="lblReportDescTitleResource1"
                                                                                                Text="Report Description:"></asp:Label></b>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="LabelReportDescription" runat="server" CssClass="formtext" meta:resourcekey="LabelReportDescriptionResource1"></asp:Label>
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
                                                    </table>
                                                </td>
                                            </tr>
                                            <asp:Panel ID="pnlPDF" runat="server" >
                                            <tr>
                                                <td style="width: 90%;" align="right">
                                                    <a href="http://www.adobe.com/products/acrobat/readermain.html" target="top">
                                                        <img height="31" src="../images/get_adobe_reader.gif" align="right" alt="Adobe Reader" border="0" />
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
            <telerik:RadPageView ID="Repository" runat="server" PageViewID="Repository" meta:resourcekey="RadPageView2Resource1" Height="92%" >
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
                    <uc1:frmScheduleReportList runat="server" ID="frmScheduleReportList" />
                </center>
            </telerik:RadPageView>     
           </telerik:RadMultiPage>
        </asp:Panel>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
            <Windows>
                <telerik:RadWindow ID="UserListDialog" runat="server" Skin="Hay" DestroyOClose="true"
                    ReloadOnShow="false" ShowContentDuringLoad="false" Modal="true" VisibleStatusbar="false"
                    VisibleTitlebar="false"  Animation="Fade" AnimationDuration="1" />
            </Windows>
        </telerik:RadWindowManager>
        <!-- Devin Added -->
        <asp:Button ID="btnAfterCreate" runat = "server" style="display:none" OnClick="btnAfterCreate_Click" />
        <asp:HiddenField ID="hidAfterCreate" runat = "server"  />

        </form>
    </div>
    <script type ="text/javascript" >

    </script>
</body>
</html>
