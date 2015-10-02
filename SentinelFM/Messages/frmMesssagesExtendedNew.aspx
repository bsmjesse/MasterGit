<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMesssagesExtendedNew.aspx.cs" Inherits="SentinelFM.Messages_frmMesssagesExtendedNew"  Culture="en-US"  UICulture="auto" meta:resourcekey="PageResource2" %>
<% if (QueryType != "export")
   {  %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik"%>
<%--<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>

<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
   TagPrefix="ISWebInput" %>--%>

<%--<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>--%>

<%--<%@ Register assembly="ISNet.WebUI.WebCombo" namespace="ISNet.WebUI.WebCombo" tagprefix="ISWebCombo" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>--%>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>frmMessages</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <link rel="stylesheet" href="//cdn.datatables.net/1.10.7/css/jquery.dataTables.css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <%--<link rel="stylesheet" href="../maps/style.css" type="text/css">--%>
    <link rel="stylesheet" href="../Scripts/jquery-plugin/jquery.timepicker/jquery.timepicker.css" type="text/css">
    <link rel="stylesheet" href="../Scripts/jquery-plugin/jquery.loadmask/jquery.loadmask.css" type="text/css">
    <link rel="stylesheet" href="../Scripts/jquery-plugin/dropit/dropit.css" type="text/css">    
    
    <script type="text/javascript" src="//code.jquery.com/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="//cdn.datatables.net/1.10.7/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="//code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-plugin/jquery.timepicker/jquery.timepicker.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-plugin/jquery.loadmask/jquery.loadmask.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-plugin/dropit/dropit.js"></script>    

    <script language="javascript">
		<!--
        var iTableCounter = 1;
        var currentMessageRow = null;

        function ScrollColor() {
            with (document.body.style) {
                scrollbarDarkShadowColor = "003366";
                scrollbar3dLightColor = "gray";
                scrollbarArrowColor = "gray";
                scrollbarBaseColor = "FFFFFF";
                scrollbarFaceColor = "FFFFFF";
                scrollbarHighlightColor = "gray";
                scrollbarShadowColor = "black";
                scrollbarTrackColor = "whitesmoke";
            }
        }


        function MessageInfoWindow(MsgKey) {
            var mypage = 'frmMessageInfo.aspx?MsgKey=' + MsgKey
            var myname = '';
            var w = 335;
            var h = 360;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }


        function NewDriverMsg() {

            var combo = ISGetObject("cboDrivers");
            var driverId = combo.Value;
            var mypage = 'frmNewDriverMsg.aspx?driverId=' + driverId;
            var myname = '';
            var w = 735;
            var h = 480;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }


        function NewMessageWindow() {
            var mypage = 'frmNewMessageMain.aspx'
            var myname = '';
            var w = 540;
            var h = 610;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
            return false;
        }



        function NewLocationsWindow() {
            var mypage = 'frmNewLocation.aspx'
            var myname = '';
            var w = 685;
            var h = 500;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }



        function MsgDetails(mt, id, mode) {
            var mypage = 'MessageDetail.aspx?mt=' + mt + '&id=' + id + '&mode=' + mode;
            var myname = '';
            var w = 400;
            var h = 600;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=yes,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        var $j = jQuery.noConflict();        

        function getMessageDetails(MsgId, BoxID) {
            $j.ajax({
                type: "POST",
                //create a method for search the data and show in datatable
                url: "./frmMesssagesExtendedNew.aspx/GetMessageDetails",
                contentType: "application/json; charset=utf-8",
                data: '{MsgId: "' + MsgId + '", BoxID: "' + BoxID + '"}',
                dataType: "json",
                success: getMessageDetailsSucceeded,
                error: getMessageDetailsFailed
            });
        }

        function getDestinationDetails(MsgId){
            $j.ajax({
                type: "POST",
                //create a method for search the data and show in datatable
                url: "./frmMesssagesExtendedNew.aspx/GetDestinationDetails",
                contentType: "application/json; charset=utf-8",
                data: '{MsgId: "' + MsgId + '"}',
                dataType: "json",
                success: getDestinationDetailsSucceeded,
                error: getMessageDetailsFailed
            });
        }

        function getMessageDetailsSucceeded(result) {
            //$("body").unmask();
            if (result.d != "[]") {
                var jdetails = result.d;
                dataTab = $.parseJSON(jdetails);
                
                odatatable.fnOpen(currentMessageRow, fnFormatDetails(iTableCounter, $("#detailsTable").html()), 'details');
                oInnerTable = $j("#messageDetailsTable_" + iTableCounter).dataTable({
                    "data": dataTab.data,
                    "bJQueryUI": true,
                    "bFilter": false,
                    "bPaginate": false,
                    "bSort" : false,                        
                    "sDom": 'lfrtip',
                    "order": [[0, "asc"]]
                });
                iTableCounter = iTableCounter + 1;
            }
        }

        function getDestinationDetailsSucceeded(result) {
            //$("body").unmask();
            if (result.d != "[]") {
                var jdetails = result.d;
                dataTab = $.parseJSON(jdetails);
                
                odatatable.fnOpen(currentMessageRow, fnFormatDetails(iTableCounter, $("#destinationDetailsTable").html()), 'details');
                oInnerTable = $j("#messageDetailsTable_" + iTableCounter).dataTable({
                    "data": dataTab.data,
                    "bJQueryUI": true,
                    "bFilter": false,
                    "bPaginate": false,
                    "bSort" : false,                        
                    "sDom": 'lfrtip',
                    "order": [[0, "asc"]]
                });
                iTableCounter = iTableCounter + 1;
            }
        }

        function getMessageDetailsFailed(result) {
            alert(result.status + ' ' + result.statusText);
        }

        function fnFormatDetails(table_id, html) {
            var sOut = "<table id=\"messageDetailsTable_" + table_id + "\">";
            sOut += html;
            sOut += "</table>";
            return sOut;
        }

        function exportGrid(exportType, format) {            

            var selectedMsgKeys = '';
            var filteredRows = odatatable._('tr', {"filter":"applied"});		    
            for (var i = 0; i < filteredRows.length; i++)
            {
                var columnIndex = exportType == 'message' ? 13 : 10;
                selectedMsgKeys += (selectedMsgKeys == '' ? '' : ',') + "'" + filteredRows[i][columnIndex] + "'";
            }
            if(selectedMsgKeys == '')
                selectedMsgKeys = "'-1'";
            $j('#MsgKeysForExport').val(selectedMsgKeys);

            $('#exportType').val(exportType);
            $('#exportformat').val(format);
            $("#exportForm").submit();
        }

        //-->
    </script>

    <style type="text/css">
        .style1
        {
            width: 83px;
        }
        .style2
        {
            width: 100%;
        }
        .WG5-Row img
        {
            left: 0 !important;
        }
        
        .WG5-Row span
        {
            display: inline-block !important;
        }
        #tblMessages span, #tblDestinations span {
            display:none; 
        }

        tr.details table thead, tr.details div.fg-toolbar {
            display:none;
        }

        table.dataTable tbody tr.odd {
            background-color: #f1efe2;
        }
        table.dataTable tbody tr.selected {
            background-color: #b0bed9 !important;
        }

        .menu ul { display: none; } /* Hide before plugin loads */
        .menu ul.dropit-submenu {
            background-color: #fff;
            border: 1px solid #b2b2b2;
            padding: 6px 0;
            margin: 3px 0 0 1px;
            -webkit-border-radius: 3px;
               -moz-border-radius: 3px;
                    border-radius: 3px;
            -webkit-box-shadow: 0px 1px 3px rgba(0,0,0,0.15);
               -moz-box-shadow: 0px 1px 3px rgba(0,0,0,0.15);
                    box-shadow: 0px 1px 3px rgba(0,0,0,0.15);
        }          
        .menu ul.dropit-submenu a {
            display: block;
            font-size: 12px;
            line-height: 25px;
            color: black;
            padding: 0 18px;
            text-align: left;
            text-decoration: none;
            font-family: verdana;
            /*width: 100%;*/
        }
        .menu ul.dropit-submenu a:hover {
            background: #248fc1;
            color: #fff;
            text-decoration: none;
        }
        
        table.dataTable tbody th, table.dataTable tbody td, table.dataTable thead th, .dataTables_wrapper {
            font-family: verdana;
            font-size: 11px;
        }

        table.dataTable tbody th, table.dataTable tbody td {
            padding: 4px 5px;
        }

        table.dataTable thead th, table.dataTable thead td {
            padding: 5px 9px;
        }
        
        table.dataTable tbody td.details {
            padding-left:20px;
        }

    </style>

</head>
<body  >
    
    <script type="text/javascript">
        var MESSAGE_DATA = <%=MESSAGE_DATA %>;   
        var DESTINATION_DATA = <%=DESTINATION_DATA%>;
        var CurrentPage = "<% = (tblMessagesButtons.Visible == true) ? "message" : "destination" %>";
        var odatatable;
        var SelectedLanguage = '<%=((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en")%>';
        $j(document).ready(function () {
            if(CurrentPage == 'message') {
                odatatable = $j('#tblMessages').dataTable({
                    "data": MESSAGE_DATA.data,
                    "order": [[1, "desc"]],
                    "aoColumnDefs": [
                        { 'bSortable': false, 'aTargets': [ 0, 10 ] }
                    ]
                    ,"fnRowCallback": function( nRow, aData, iDisplayIndex ) {
                        nRow.className += " selectable ";
                        return nRow;
                    }
                    ,"language": {
                        "url": SelectedLanguage == 'fr' ? "//cdn.datatables.net/plug-ins/1.10.7/i18n/French.json" : ''
                    }
                });

                $('#tblMessages tbody td img.expandable').live('click', function (e) {
                
                    var nTr = $(this).parents('tr')[0];
                    var nTds = this;

                    currentMessageRow = nTr;
            
                    if (odatatable.fnIsOpen(nTr)) {
                        /* This row is already open - close it */
                        this.src = "images/SD7Dz.png";
                        odatatable.fnClose(nTr);
                    }
                    else {
                        /* Open this row */
                        $j('.expandthisrow').removeClass('expandthisrow');
                        $j(nTds).closest('tr').addClass('expandthisrow');

                        //var rowIndex = odatatable.fnGetPosition( $j(nTds).closest('tr')[0] );                     
                        this.src = "images/d4ICC.png";

                        getMessageDetails(odatatable.api().rows('.expandthisrow').data()[0][11], odatatable.api().rows('.expandthisrow').data()[0][12]);                    
                    }                

                    return false;
                });

                $j('#tblMessages tbody').on('click', 'tr.selectable', function (e) {
                    if(e.target.tagName == 'IMG')
                        return;
                    $j(this).toggleClass('selected');

                    var selectedMessages = '';
                    for (var i = 0; i < odatatable.api().rows('.selected').data().length; i++)
                    {
                        selectedMessages += (selectedMessages == '' ? '' : '|') + odatatable.api().rows('.selected').data()[i][13];
                    }
                    $j('#SelectedKeyValues').val(selectedMessages);
                });
            }
            else {
                odatatable = $j('#tblDestinations').dataTable({
                    "data": DESTINATION_DATA.data,
                    "order": [[1, "asc"]],
                    "aoColumnDefs": [
                        { 'bSortable': false, 'aTargets': [ 0 ] }
                    ]
                    ,"fnRowCallback": function( nRow, aData, iDisplayIndex ) {
                        nRow.className += " selectable ";
                        return nRow;
                    }
                    ,"language": {
                        "url": SelectedLanguage == 'fr' ? "//cdn.datatables.net/plug-ins/1.10.7/i18n/French.json" : ''
                    }
                });

                $('#tblDestinations tbody td img.expandable').live('click', function (e) {
                
                    var nTr = $(this).parents('tr')[0];
                    var nTds = this;

                    currentMessageRow = nTr;
            
                    if (odatatable.fnIsOpen(nTr)) {
                        /* This row is already open - close it */
                        this.src = "images/SD7Dz.png";
                        odatatable.fnClose(nTr);
                    }
                    else {
                        /* Open this row */
                        $j('.expandthisrow').removeClass('expandthisrow');
                        $j(nTds).closest('tr').addClass('expandthisrow');

                        //var rowIndex = odatatable.fnGetPosition( $(nTds).closest('tr')[0] );
                        //getDestinationDetails(odatatable.api().rows().data()[rowIndex][9]);                    
                        this.src = "images/d4ICC.png";
                        getDestinationDetails(odatatable.api().rows('.expandthisrow').data()[0][9]);                    
                    }                

                    return false;
                });

                //$j('#tblDestinations tbody').on('click', 'tr.selectable', function (e) {
                //    if(e.target.tagName == 'IMG')
                //        return;
                //    $j(this).toggleClass('selected');

                //    var selectedMessages = '';
                //    for (var i = 0; i < odatatable.api().rows('.selected').data().length; i++)
                //    {
                //        selectedMessages += (selectedMessages == '' ? '' : '|') + odatatable.api().rows('.selected').data()[i][13];
                //    }
                //    $j('#SelectedKeyValues').val(selectedMessages);
                //});
            }

            $j('.menu').dropit();
        });
    </script>

        <%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
  {%>
  
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>     

    <script language="javascript">
		<!--
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = '<%=DefaultOrganizationHierarchyFleetId %>';
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

        var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0&loadVehicle=0";
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

        function OrganizationHierarchyNodeSelected(nodecode, fleetId)
        {            
            var myVal = document.getElementById('<%=valVehicle.ClientID %>');
            ValidatorEnable(myVal, false); 

            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
        }
            
			//-->
    </script>
<%} %>


    <form id="frmMessagesForm" method="post" runat="server">

        <input type="hidden" id="SelectedKeyValues" name="SelectedKeyValues" value="" runat ="server" />

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

                    return;
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
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery.cookie.js" />
                <asp:ScriptReference Path="~/Reports/jqueryFileTree.js" />
                <asp:ScriptReference Path="~/Reports/splitter.js" />
                <asp:ScriptReference Path="~/scripts/tablesorter/jquery.tablesorter.min.js" />
                <asp:ScriptReference Path="~/scripts/colResizable-1.3.min.js" />
                <asp:ScriptReference Path="~/scripts/json2.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Transparency="0" Skin="Hay">
        </telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1">
        </telerik:RadAjaxManager>
    
        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

    <%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
          {%>        
            <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" 
        Text="Button" style="display:none;" AutoPostBack="True"
            OnClick="hidOrganizationHierarchyPostBack_Click" 
        meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />
        <%} %>
    
    
    <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px" cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                               <asp:Button ID="cmdMessages" runat="server" CausesValidation="False" CssClass="selectedbutton" Text="Messages" 
                                 Width="181px" onclick="cmdMessages_Click" meta:resourcekey="cmdMessagesResource2"/>
                            </td>
                            <td>
                                   <asp:Button ID="cmdDestinations" runat="server" CausesValidation="False" CssClass="confbutton" Text="Destinations" 
                                   Width="181px" onclick="cmdDestinations_Click"  meta:resourcekey="cmdDestinationsResource2" />
                            </td>                            
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td valign="top" >
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="679" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" height="550px" width="990px" class=table  border="0">
                                    <tr>
                                        <td class="configTabBackground" valign=top  >
                                
                <fieldset>
                    <table class="formtext" style="width: 820px" >
                                <tr>
                                    <td align="left" >
                                        <table border="0px" cellpadding="0" cellspacing="0" >
                                          <tr>
                                              <td>
                                        <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1" Text="From :"></asp:Label>
                                    </td>
                                              <td style="width: 100px">
                                        <telerik:RadDatePicker ID="txtFrom" runat="server" Width="100px" DateInput-EmptyMessage=""
                                                    MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US" >
                                                    <Calendar>
                                                        <SpecialDays>
                                                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                                        </SpecialDays>
                                                    </Calendar>
                                                    <DateInput Culture="en-US" DateFormat='<%# sn.User.DateFormat %>' DisplayDateFormat='<%# sn.User.DateFormat %>' LabelCssClass="" />
                                        </telerik:RadDatePicker>
                                    </td>
                                              <td style="width: 100px">
                                        <telerik:RadTimePicker ID="cboHoursFrom" runat="server"  valign="top" meta:resourcekey="cboHoursFromResource1" Skin="Hay" />
                                    </td>
                                              <td>
                                        <asp:RequiredFieldValidator ID="valFromDate" runat="server" ControlToValidate="txtFrom" ValidationGroup="vgSubmit" ErrorMessage="Please select a From" meta:resourcekey="valFromDateResource1" Text="*"> </asp:RequiredFieldValidator>
                                    </td> 
                                              <td>
                                                <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1" Text="To:"></asp:Label>
                                              </td>
                                              <td style="width: 100px">
                                                <telerik:RadDatePicker ID="txtTo" runat="server" Width="100px" DateInput-EmptyMessage=""
                                                            Skin="Hay" MinDate="01/01/1900" MaxDate="01/01/3000"  Culture="en-US">
                                                            <Calendar>
                                                                <SpecialDays>
                                                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                                                </SpecialDays>
                                                            </Calendar>
                                                            <DateInput Culture="en-US" DateFormat='<%# sn.User.DateFormat %>' DisplayDateFormat='<%# sn.User.DateFormat %>' LabelCssClass="" />
                                                        </telerik:RadDatePicker>
                                             </td>
                                              <td>
                                                <asp:RequiredFieldValidator ID="valToDate" runat="server" ControlToValidate="txtTo" ValidationGroup="vgSubmit" ErrorMessage="Please select a To" meta:resourcekey="valToDateResource1" Text="*"></asp:RequiredFieldValidator>
                                            </td>
                                              <td style="width: 100px" valign="top">
                                                <telerik:RadTimePicker ID="cboHoursTo" runat="server" valign="top" meta:resourcekey="cboHoursToResource1" Skin="Hay" />
                                            </td>
                                         </tr>
                                      </table>
                                   </td>                                   
                                </tr>

                                <tr>
                                <td align="left" >
                                <table border="0px" cellpadding="0" cellspacing="0" >
                                <tr>
                                <td>
                                <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                                    Text="Fleet:" Width ="100px"></asp:Label>
                                    <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" Text=" Hierarchy Node:" 
                                           Visible="False" meta:resourcekey="lblOhTitleResource1"  Width ="100px"  />
                                </td>
                                <td>
                                <asp:RangeValidator ID="valFleet" runat="server" MaximumValue="999999999999999" MinimumValue="1"
                                    ErrorMessage="Please select a Fleet" ControlToValidate="cboFleet" meta:resourcekey="valFleetResource1"
                                    Text="*"></asp:RangeValidator>
                                </td>
                                <td style="width: 100px">
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="257px"
                                    DataValueField="FleetId" DataTextField="FleetName" AutoPostBack="True" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                    meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList>
                                </td>
                                <td>
                                <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" CssClass="combutton" 
                                           Visible="False" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                           meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />                                
                                </td>
                                <td>&nbsp;</td>
                                <td>                            
                                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Width="50px" 
                                    Visible="False" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:"></asp:Label>
                                </td>
                                <td>
                                <asp:RequiredFieldValidator ID="valVehicle" runat="server" ControlToValidate="cboVehicle"
                                    ErrorMessage="Please select a Vehicle" meta:resourcekey="RequiredFieldValidator1Resource1"
                                    Text="*"></asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 100px">
                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="256px"
                                    DataValueField="BoxId" DataTextField="Description" Visible="False"
                                     meta:resourcekey="cboVehicleResource1">
                                </asp:DropDownList>
                                </td>
                                </tr>
                                </table>
                                </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                       <table border="0px" cellpadding="0" cellspacing="0" >
                                           <tr>
                                            <td align="left" valign="top">                                
                                                <asp:Label ID="lblFolderListTitle" runat="server" CssClass="formtext" 
                                                meta:resourcekey="lblFolderListTitleResource1" Text="Display:" Width ="110px"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <asp:DropDownList ID="cboDirection" runat="server" CssClass="RegularText" 
                                                meta:resourcekey="cboDirectionResource1" Width="256px">
                                                <asp:ListItem meta:resourcekey="ListItemResource1" Text="All" Value="0"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource2" Text="Incoming" Value="1"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource3" Text="Outgoing" Value="2"></asp:ListItem>
                                                </asp:DropDownList>
                                             </td>
                                           </tr>
                                       </table>
                                    </td>
                                </tr>
                                <tr>
                                
                                      
                                   <td  align="left">
                            
                                      </td>
                                   <td valign="top">
                                       <table width="100%" cellpadding=0 cellspacing=0 border="0"  >
                                           <tr>
                                               <td valign="top">
                                                   <table id="tblDestinationsButtons"  runat=server   >
                                                       <tr>
                                                           <td>
                                        <asp:Button ID="cmdShowDestinations" runat="server" CssClass="combutton"  
                                             
                                            Text="<%$ Resources:cmdShowDestinations %>"  Width="180px" onclick="cmdShowDestinations_Click"  meta:resourcekey="cmdShowDestinationsResource2"  />
                                                           </td>
                                                           <td>
                                        <asp:Button ID="cmdNewDestination" runat="server" CausesValidation="False" 
                                            CommandName="25" CssClass="combutton"  
                                            Text="<%$ Resources:dgMessages_cmdNewDestination %>"  Width="136px" OnClientClick="NewLocationsWindow();return false;"  meta:resourcekey="cmdNewDestinationResource2"  />
                                                           </td>
                                                       </tr>
                                                       <tr>
                                                           <td colspan="2" align="left">
                                                               <div style="position: relative; z-index: 3000">
                                                               <ul class="menu">
                                                                    <li>
                                                                        <a href="#" style="text-decoration:none;">
                                                                        <asp:Button ID="Button1" runat="server" CausesValidation="False" 
                                                                            CommandName="25" CssClass="combutton" meta:resourcekey="cmdExportMessageResource1" 
                                                                            OnClientClick="return false;"  Text="Export" Width="136px" />
                                                                        </a>
                                                                        <ul>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('destination','csv');">CSV</a></li>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('destination','excel2003');">Excel 2003</a></li>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('destination','excel2007');">Excel 2007</a></li>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('destination','pdf');">PDF</a></li>
                                                                        </ul>
                                                                    </li>
                                                                </ul>
                                                                   </div>
                                                           </td>
                                                       </tr>
                                                   </table>
                                                   <table id="tblMessagesButtons" runat=server   >
                                                       <tr>
                                                           <td>
                                        <asp:Button ID="cmdShowMessages" runat="server" CssClass="combutton" 
                                            meta:resourcekey="cmdShowMessagesResource1" OnClick="cmdShowMessages_Click"
                                            Text="Refresh" Width="136px" />
                                                           </td>
                                                           <td>
                                                                                                            
                                                             <asp:Button ID="cmdMarkAsRead" runat="server" CssClass="combutton" 
                                            meta:resourcekey="cmdMarkAsReadResource1" OnClick="cmdMarkAsRead_Click" 
                                            Text="Mark as read" Width="136px" />

                                      
                                                           </td>
                                                       </tr>
                                                       <tr>
                                                           <td>
                                                                <asp:CheckBox ID="chkAuto" runat="server" CssClass="formtext" Font-Bold="False" 
                                                                    meta:resourcekey="chkAutoResource1" Text="Auto Time Refresh " Visible="False" />
                                                                                                            
                                                                <asp:Button ID="cmdNewMessage" runat="server" CausesValidation="False" 
                                                                    CommandName="25" CssClass="combutton" meta:resourcekey="cmdNewMessageResource1" 
                                                                    OnClientClick="NewMessageWindow();return false;"  Text="New Text Message" Width="136px" />
                                                            </td>
                                                           <td>
                                                               <div style="position: relative; z-index: 3000">
                                                               <ul class="menu">
                                                                    <li>
                                                                        <a href="#" style="text-decoration:none;">
                                                                        <asp:Button ID="cmdExportMessage" runat="server" CausesValidation="False" 
                                                                            CommandName="25" CssClass="combutton" meta:resourcekey="cmdExportMessageResource1" 
                                                                            OnClientClick="return false;"  Text="Export" Width="136px" />
                                                                        </a>
                                                                        <ul>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('message','csv');">CSV</a></li>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('message','excel2003');">Excel 2003</a></li>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('message','excel2007');">Excel 2007</a></li>
                                                                            <li><a href="javascript:void(0)" onclick="exportGrid('message','pdf');">PDF</a></li>
                                                                        </ul>
                                                                    </li>
                                                                </ul>
                                                                </div>                                                               
                                                           </td>
                                                       </tr>
                                                   </table>
                                               </td>
                                           </tr>
                                       </table>
                                        </td>
                                </tr>
                                <tr>
                                   <td align="left" valign=top colspan="4" >
                             
                                       <%--<busyboxdotnet:busybox
                                       id="BusyReport" runat="server" anchorcontrol="" compressscripts="False" gzipcompression="False"
                                       meta:resourcekey="BusyReportResource1" slideeasing="BackIn"
                                       text=""></busyboxdotnet:busybox>--%>
                                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="formtext" 
                                           meta:resourcekey="valSummaryResource1">
                                </asp:ValidationSummary>
                              
                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" 
                                    Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                </tr>
                             </table>
                </fieldset> 
                    
                    
                     <asp:MultiView ID="MultiviewMessages" runat="server" ActiveViewIndex="0">
                        <asp:View ID="TextMessages" runat="server">
                            <div style="height:450px;width:100%;vertical-align:top;padding-top:15px;">
                                <table id="tblMessages" style="width:100%;">
                                    <thead> 
                                        <tr>
                                            <th width="10">&nbsp;</th>
                                            <th width="100"><%=(string)base.GetLocalResourceObject("dgMessages_MsgDateTime")%></th>
                                            <th width="150"><%=(string)base.GetLocalResourceObject("dgMessages_Address")%></th>
                                            <th width="70"><%=(string)base.GetLocalResourceObject("dgMessages_From")%></th>
                                            <th width="70"><%=(string)base.GetLocalResourceObject("dgMessages_To")%></th>
                                            <th width="35"><%=(string)base.GetLocalResourceObject("dgMessages_MsgDirection")%></th>
                                            <th width="200"><%=(string)base.GetLocalResourceObject("dgMessages_MsgBody")%></th>
                                            <th width="70"><%=(string)base.GetLocalResourceObject("dgMessages_Status")%></th>
                                            <th width="35"><%=(string)base.GetLocalResourceObject("dgMessages_Acknowledged")%></th>
                                            <th width="50"><%=(string)base.GetLocalResourceObject("dgMessages_UserName")%></th>                                        
                                            <th width="15">&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>  
                                    
                        </asp:View>
                        
                          <asp:View ID="Destinations" runat="server">
                              <div style="height:450px;width:100%;vertical-align:top;padding-top:15px;">
                                <table id="tblDestinations" style="width:100%;">
                                    <thead> 
                                        <tr>
                                            <th width="10">&nbsp;</th>
                                            <th width="100"><%=(string)base.GetLocalResourceObject("dgMessages_MsgDateTime")%></th>
                                            <th width="70"><%=(string)base.GetLocalResourceObject("dgMessages_From")%></th>
                                            <th width="70"><%=(string)base.GetLocalResourceObject("dgMessages_To")%></th>
                                            <th width="45"><%=(string)base.GetLocalResourceObject("dgGarminHist_Address")%></th>
                                            <th width="215"><%=(string)base.GetLocalResourceObject("dgMessages_MsgBody")%></th>
                                            <th width="80"><%=(string)base.GetLocalResourceObject("dgMessages_Status")%></th>
                                            <th width="35"><%=(string)base.GetLocalResourceObject("dgMessages_Acknowledged")%></th>
                                            <th width="50"><%=(string)base.GetLocalResourceObject("dgMessages_UserName")%></th>                                            
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                            
                           
                           
                           <%--<ISWebGrid:WebGrid ID="dgDestinations" runat="server" Height="450px" 
                                UseDefaultStyle="True" 
                                ViewStateStorage="Session" Width="100%" 
                                  oninitializedatasource="dgDestinations_InitializeDataSource" 
                                  oninitializelayout="dgDestinations_InitializeLayout" 
                                  oninitializerow="dgDestinations_InitializeRow">
                                    <RootTable DataKeyField="MsgId" DataMember="Master">
                                   <Columns>
                                <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" 
                                            Bound="False" Caption="chkBox" ColumnType="CheckBox" DataMember="chkBox" 
                                            EditType="NoEdit" IsRowChecker="True" Name="Select Row" 
                                            ShowInSelectColumns="No" Width="25px"></ISWebGrid:WebGridColumn>
                                            <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgDateTime %>" 
                                            DataMember="MsgDate" DataType="System.DateTime" Name="MsgDate" Width="100px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_From %>" 
                                            DataMember="From" Name="From" Width="70px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_To %>" 
                                            DataMember="To" Name="To" Width="70px"></ISWebGrid:WebGridColumn>
                                             <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_Address %>' DataMember="location" Name="location">
                                                        </ISWebGrid:WebGridColumn>
                                            
                                            <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgBody %>" 
                                            DataMember="MsgBody" Name="MsgBody" Width="200px"></ISWebGrid:WebGridColumn>
                                            
                                             <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_Status %>" 
                                            DataMember="Status" Name="Status" Width="100px">
                                            </ISWebGrid:WebGridColumn>
                                            
                                            <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_Acknowledged %>" 
                                            DataMember="Acknowledged" Name="Acknowledged" Width="35px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_UserName %>" 
                                            DataMember="UserName" Name="UserName" Width="50px"></ISWebGrid:WebGridColumn>
                                            
                                            
                                              <ISWebGrid:WebGridColumn Caption="MsgId" 
                                            DataMember="MsgId" Name="MsgId" Visible=false  >
                                            </ISWebGrid:WebGridColumn>
                                            
                                            </Columns>
                                            
                                            
                                              <ChildTables   >
                       
                            
                                      <ISWebGrid:WebGridTable ColumnHeaders=No    Caption="Details" DataKeyField="RowId" DataMember="Details">
                                           
                                          <Columns>
                                              
                                              
                                                    <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgDateTime %>" 
                                            DataMember="MsgDate" DataType="System.DateTime" Name="MsgDate" Width="100px"></ISWebGrid:WebGridColumn>
                                            <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_From %>" 
                                            DataMember="From" Name="From" Width="70px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_To %>" 
                                            DataMember="To" Name="To" Width="70px"></ISWebGrid:WebGridColumn>
                                            
                                             
                                            
                                            <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgBody %>" 
                                            DataMember="MsgBody" Name="MsgBody" Width="200px"></ISWebGrid:WebGridColumn>
                                            
                                            <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_Status %>" 
                                            DataMember="Status" Name="Status" Width="100px">
                                            </ISWebGrid:WebGridColumn>
                                            <ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_UserName %>" 
                                            DataMember="UserName" Name="UserName" Width="50px"></ISWebGrid:WebGridColumn>
                     
                                                                                   
                                          </Columns>
                                         
                                      </ISWebGrid:WebGridTable>
                                  </ChildTables>
                                  
                                  
                                  
                                         </RootTable><LayoutSettings AllowColumnMove="Yes" AllowContextMenu="False"  Hierarchical="True" 
                                    AllowExport="Yes" AllowFilter="Yes" AllowSorting="Yes" 
                                    AutoFilterSuggestion="True" AutoFitColumns="True" ColumnSetHeaders="Default" 
                                    DisplayDetailsOnUnhandledError="False" HideColumnsWhenGrouped="Default" 
                                    PersistRowChecker="True" ResetNewRowValuesOnError="False" 
                                     RowHeaders="Default" 
                                    RowHeightDefault="25px" RowLostFocusAction="NeverUpdate"><FreezePaneSettings AbsoluteScrolling="True" /></LayoutSettings></ISWebGrid:WebGrid>--%>
                                    
                                    
                             <table width=100% >
                <tr>
                    
                    <td align=left>
                                        &nbsp;</td>
                </tr>
              </table>
                        </asp:View>
                        
                    </asp:MultiView>  
                                        </td> 
                                    </tr>
                                    </table>
                    
                    
                                </td>        
                    
                    </tr>
                    </table>
                </td>
            </tr>
    </table>     
    </form>

    <div style="display:none">    
        <table id="detailsTable">
            <thead> 
                <tr>
                    <th width="100">MsgDate</th>
                    <th width="150">StreetAddress</th>
                    <th width="70">From</th>
                    <th width="70">To</th>
                    <th width="35">Direction</th>
                    <th width="200">MsgBody</th>
                    <th width="70">Status</th>
                    <th width="50">Read</th>
                    <th width="15"></th>
                </tr>
            </thead>
            <tbody></tbody>
        </table>
    </div>

    <div style="display:none">    
        <table id="destinationDetailsTable">
            <thead> 
                <tr>
                    <th width="100">MsgDate</th>
                    <th width="70">From</th>
                    <th width="70">To</th>
                    <th width="200">MsgBody</th>
                    <th width="70">Status</th>
                    <th width="50">Read</th>                    
                </tr>
            </thead>
            <tbody></tbody>
        </table>
    </div>

    <form id="exportForm" name="exportForm" action="frmMesssagesExtendedNew.aspx" method="post" target="frmExport" style="display:none;">
        <input type="hidden" id="QueryType" name="QueryType" value="export" />
        <input type="hidden" id="exportType" name="exportType" value="message" />
        <input type="hidden" id="exportformat" name="exportformat" value="csv" />
        <input type="hidden" id="MsgKeysForExport" name="MsgKeysForExport" value="" />
    </form>
     <iframe id="frmExport" name="frmExport" width="0" height="0"></iframe> 


</body>
</html>
<% } %>