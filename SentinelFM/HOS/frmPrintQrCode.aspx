<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmPrintQrCode.aspx.cs" Inherits="SentinelFM.HOS_frmPrintQrCode" %>

<%@ Register Src="../Configuration/Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="HosTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css">
    <link rel="stylesheet" href="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
    <link href="../reports/jqueryFileTree.css" rel="stylesheet" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../Styles/SentinelFM/css/simplePagination.css" />
    <style type="text/css">
        .style1 {
            width: 45px;
        }


        .style2 {
            width: 120px;
        }

        .style3 {
            width: 119px;
        }

        .style4 {
            width: 28px;
        }

        .style5 {
            height: 24px;
            width: 28px;
        }

        .style6 {
            height: 14px;
            width: 28px;
        }

        .style7 {
            height: 25px;
            width: 28px;
        }

        .style8 {
            height: 9px;
            width: 28px;
        }
    </style>


    <style type="text/css">
        .loading {
            background-color: #dff3ff;
            border: 1px solid #c6e1f2;
        }
    </style>
</head>
<body>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
    <script type="text/javascript" src="../scripts/jquery.simplePagination.js"></script>
    <script type="text/javascript" src="../Reports/jqueryFileTree.js?v=20140220"></script>
    <script type="text/javascript" src="../Reports/splitter.js"></script>
    <script type="text/javascript" src="../scripts/tablesorter2145/js/jquery.tablesorter.js?v=20140113"></script>
    <script type="text/javascript" src="../scripts/tablesorter2145/js/jquery.tablesorter.widgets.min.js"></script>
    <script type="text/javascript" src="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.js"></script>
    <script type="text/javascript" src="../scripts/colResizable-1.3.min.js"></script>
    <script type="text/javascript" src="../scripts/json2.js"></script>

    <script language="javascript" type="text/javascript">
	<!--

    var browserTypes = { "MSIE8": 0, "MSIE7": 1, "MSIE6": 2, "Gecko": 3, "WebKit": 4 };
    var browserType = browserTypes.MSIE;


    var OrganizationHierarchyPath = "";
    var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
    var PreferOrganizationHierarchyNodeCode = "<%=PreferOrganizationHierarchyNodeCode %>";

    var var_inspectionJson = "";
    var var_scannableImage = "<img src='../images/qrcode.png' id ='camera' style='width:16px; height:16px;margin-top:0px;margin-right:5px;margin-left:5px' />";

    var inspectionAssignedFormsJson = "";
    var IniHierarchyPath = <%=IniHierarchyPath.ToString().ToLower() %>;
        var rex = /AppleWebKit\/\d{1,3}\.\d{1,3}/
        var match = navigator.userAgent.match(rex);
        var isBox = false;
        var isDot = false;
        var currentSelectionFleet = "";
        var currentSelectionVehicleDesc= "";
        var OrganizationDOT = "<%=OrganizationDOT%>";
        var curWaitId = null;
        if (match) browserType = browserTypes.WebKit;

        rex = /Gecko\/\d*\s/
        match = navigator.userAgent.match(rex);
        if (match) browserType = browserTypes.Gecko;

        rex = /MSIE\s*\d{1,2}\.\d{1,3};/
        match = navigator.userAgent.match(rex);
        if (match && match[0]) {
            if (parseInt(match[0].split(' ')[1]) === 8) browserType = browserTypes.MSIE8;
            if (parseInt(match[0].split(' ')[1]) === 7) browserType = browserTypes.MSIE7;
            if (parseInt(match[0].split(' ')[1]) === 6) browserType = browserTypes.MSIE6;
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

            $('#vehicletreeview').fileTree({ root: PreferOrganizationHierarchyNodeCode, script: '../Reports/vehicleListTree.asmx/FetchVehicleList', expanded: inipath, expandSpeed: 200, collapseSpeed: 200, vehicledetails: 'vehicledetails'
                                            , highlightVehicleSelection: <%=OrganizationHierarchySelectVehicle.ToString().ToLower() %>
                                            , MutipleUserHierarchyAssignment: MutipleUserHierarchyAssignment
                                            , PreferOrganizationHierarchyNodeCode: PreferOrganizationHierarchyNodeCode
                                            , scriptForPreferNodecodes: '../Reports/vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                                            , FullTreeView: false 
                                            ,searchScript: '../reports/vehicleListTree.asmx/SearchOrganizationHierarchy'
                                            , VehicleListPagingBarId: 'vehiclelistPageBar'
                                            , scriptForFetchVehicleByPage: '../Reports/vehicleListTree.asmx/GetVehicleListByFilterByFleetIdForDOT'
                                            , VehiclePageSize: <%=VehiclePageSize %>
                                            , SelectedFolders: selectedFolders//IniHierarchyPath ? $('#OrganizationHierarchyNodeCode').val() : ''
                                            , SelectedFleetIds: (MutipleUserHierarchyAssignment && appendpath) ? $('#OrganizationHierarchyFleetId').val() : ''
            },
            /*
            * Call back function when you click left pane tree folder.
            */
                        function (NodeCode, FleetId, fleetName) {
                            $('#OrganizationHierarchyNodeCode').val(NodeCode);
                            $('#OrganizationHierarchyFleetId').val(FleetId);
                            $('#OrganizationHierarchyBoxId').val('');
                            $('#OrganizationHierarchyVehicleDescription').val('');
                            isBox = false;
                            isDot = false;
                            currentSelectionFleet = NodeCode;
                        },

            /*
            * Call back function when you click right pane vehicle list.
            */
                        function (BoxId, vehicleDescription) {
                            //alert('BoxId: ' + BoxId);
                            $('#OrganizationHierarchyBoxId').val(BoxId);
                            $('#OrganizationHierarchyVehicleDescription').val(vehicleDescription);
                            isBox = true;
                            isDot = false;
                            currentSelectionFleet = BoxId;
                            currentSelectionVehicleDesc = vehicleDescription;
                        },

                        function (selectedNodecodes, selectedFleetIds, fleetName)
                        {   
                            $('#OrganizationHierarchyNodeCode').val(selectedNodecodes);
                            $('#OrganizationHierarchyFleetId').val(selectedFleetIds);
                        }
                    );

                                        if (OrganizationDOT.length > 0)
                                        {
                                            $(OrganizationDOT).appendTo($('#vehicletreeview'));
                                        }

                                    }
                                    $(document).ready(function () {
                                        try {
                                            $("#MySplitter").width(780);
                                            $("#MySplitter").splitter({
                                                type: "v",
                                                outline: true,
                                                //minLeft: 100, sizeLeft: 359, minRight: 100,
                                                minLeft: 450, sizeLeft: 450, minRight: 450, sizeRight: 450,
                                                resizeToWidth: true,
                                                //cookie: "vsplitter13",
                                                accessKey: 'I'
                                            });
                                            inifiletree(OrganizationHierarchyPath);
                                            $('#vehiclelisttbl').tablesorter();
                                            $('#vehiclelisttbl').colResizable({ headerOnly: true });

                                        }
                                        catch (ex) { 
                                            alert(ex.message);
                                        }
                                    });

                                    //-->
                                    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        


            function showLoadingImage(btnId, isEnable) {
                if (isEnable == true) {
                    if (btnId != "")
                        $('#divLoading').css('left', $('#' + btnId).offset().left + $('#' + btnId).width() + 20).css('top', $('#' + btnId).offset().top + 2).show();
                    $("input[tag='dynamicInspec']").attr("disabled", "enabled");
                }
                else {
                    $('#divLoading').hide();
                    $("[tag='dynamicInspec']").attr("disabled", "");
                }
            }

            function printQRCode()
            {
                $('#lblErrorMessage').html('');
                //$('#OrganizationHierarchyBoxId').val(BoxId);

                if (isBox != true || currentSelectionFleet == "" )
                {
                    if($('#hidSelectedFleet')[0].defaultValue =="")
                    {
                        $('#lblErrorMessage').html('<%= msgSelectVehicle%>');
                        return;
                    }
                    else
                    {
                        currentSelectionFleet = $('#hidSelectedFleet')[0].defaultValue;
                        currentSelectionFleet = parseInt(currentSelectionFleet);
                        currentSelectionVehicleDesc = $('#hidSelectedText')[0].defaultValue;
                    }
                }             

                    showLoadingImage('<%= btnShowQRCode.ClientID%>', true);
                    $.ajax({
                        type: 'POST',
                        contentType: "application/json; charset=utf-8",
                        url: "frmPrintQrCode.aspx/GetInspectionCategory",
                        data: JSON.stringify({
                            boxId: currentSelectionFleet,
                            isPrint:true
                        }),
                        dataType: "json",
                        success: function (data) {
                            if (data.d != '-1' && data.d != "0") {
                                if (data.d.length > 0)
                                {
                                    OpenQRcodeWindows(data.d);
                                }
                                else 
                                {
                                    $('#lblErrorMessage').html('<%= msgNQRcodeDefined%>');
                                }
                            }
                            if (data.d == '-1') {
                                //top.document.all('TopFrame').cols = '0,*';
                                window.open('../Login.aspx', '_top')
                            }
                            if (data.d == '0') {
                                $('#lblErrorMessage').html('<%= msgFailedtoLoadData%>');
                            }
                            showLoadingImage('<%= btnShowQRCode.ClientID%>', false);
                        },
                        error: function (request, status, error) {
                            $('#lblErrorMessage').html('<%= msgFailedtoLoadData%>');
                            showLoadingImage('<%= btnShowQRCode.ClientID%>', false);
                        }
                    });
                }

                function showQRCode()
                {
                    $('#lblErrorMessage').html('');
                    //$('#OrganizationHierarchyBoxId').val(BoxId);
                    if (isBox != true || currentSelectionFleet == "" )
                    {
                        if($('#hidSelectedFleet')[0].defaultValue =="")
                        {
                            $('#lblErrorMessage').html('<%= msgSelectVehicle%>');
                            return;
                        }
                        else
                        {
                            currentSelectionFleet = $('#hidSelectedFleet')[0].defaultValue;
                            currentSelectionFleet = parseInt(currentSelectionFleet);
                            currentSelectionVehicleDesc = $('#hidSelectedText')[0].defaultValue;
                        }
                    }
               

                    showLoadingImage('<%= btnShowQRCode.ClientID%>', true);
                    $.ajax({
                        type: 'POST',
                        contentType: "application/json; charset=utf-8",
                        url: "frmPrintQrCode.aspx/GetInspectionCategory",
                        data: JSON.stringify({
                            boxId: currentSelectionFleet,
                            isPrint:false
                        }),
                        dataType: "json",
                        success: function (data) {
                            if (data.d != '-1' && data.d != "0") {
                                if (data.d.length > 0)
                                {
                                    var var_categoryList = JSON.parse(data.d);
                                    showQuestionSet(var_categoryList[0], var_categoryList[1]);
                                    $('#inspectiontreeview').show();
                                    $('#<%= lblAssignedtoVehicle.ClientID %>').text("<%= msgVehicleInspectionForm %>" + currentSelectionVehicleDesc);
                                
                                }
                                else 
                                {
                                    $('#ulInspections').empty();
                                    $('#lblErrorMessage').html('<%= msgNodataFound%>');
                                    $('#inspectiontreeview').hide();
                                }
                            }
                            if (data.d == '-1') {
                                //top.document.all('TopFrame').cols = '0,*';
                                window.open('../Login.aspx', '_top')
                            }
                            if (data.d == '0') {
                                $('#lblErrorMessage').html('<%= msgFailedtoLoadData%>');
                            }
                            showLoadingImage('<%= btnShowQRCode.ClientID%>', false);
                        },
                        error: function (request, status, error) {
                            $('#lblErrorMessage').html('<%= msgFailedtoLoadData%>');
                            showLoadingImage('<%= btnShowQRCode.ClientID%>', false);
                        }
                    });
                }

                function showQuestionSet(var_categoryJson, inspectionJson) {
                    $('#ulInspections').empty();
                    var_inspectionJson = inspectionJson;
                    if (var_categoryJson.length > 0) {
                        for (var index = 0; index < var_categoryJson.length ; index++) {
                            var eleId = var_categoryJson[index]["ID"];
                            var element = '<li class="directory collapsed" id = "li_' + eleId + '">' + var_categoryJson[index]["Defect"];

                            if (var_categoryJson[index]["BarCode"] != "") {
                                element = element + var_scannableImage + "<span id='BarCode' style='color:blue;margin-top:-5px'>" + var_categoryJson[index]["BarCode"] + "</span>";
                            }

                            element = element + '</li>';
                            $(element).css('margin-left', '25px').appendTo($('#ulInspections'));
                            findInsepctions(eleId, true);

                        }
                    }
                }

                function findInsepctions(inspectionId, isCategory) {
                    if (var_inspectionJson.length > 0) {
                        for (var index = 0; index < var_inspectionJson.length ; index++) {
                            var_parentItemID = var_inspectionJson[index]["ParentItemID"];
                            var var_categoryId = var_inspectionJson[index]["CategoryID"];
                            if ((!isCategory && var_parentItemID == inspectionId) || (isCategory && var_parentItemID == "" && var_categoryId == inspectionId)) {
                                var eleId = var_inspectionJson[index]["ID"];
                                var element = '<li class="directory collapsed" id = "li_' + eleId + '" >' + var_inspectionJson[index]["Defect"];

                                //if (var_inspectionJson[index]["Scannable"] == "1") {
                                //    element = element + var_scannableImage + "<span id='loc'>" + var_inspectionJson[index]["Location"] + "</span>";
                                // }

                                element = element + '</li>';

                                $(element).css('left', $("#li_" + inspectionId).offset().left + 25).appendTo($('#li_' + inspectionId));
                                findInsepctions(eleId, false);
                            }
                        }
                    }
                }

                function OpenQRcodeWindows(qrcodes)
                {
                    var mypage='frmPrintQrCodeP.aspx?num='+qrcodes
                    var myname='';
                    var w=800;
                    var h=700;
                    var winl = (screen.width - w) / 2; 
                    var wint = (screen.height - h) / 2; 
                    winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,' 
                    win = window.open(mypage, myname, winprops) 
                    //if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
                }

    </script>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetName" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyNodeCode" Value="" runat="server" />
        <input type="hidden" name="OrganizationHierarchyFleetId" id="OrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyBoxId" Value="" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyVehicleDescription" Value="" runat="server" />
        <asp:HiddenField ID="hidSelectedFleet" runat="server" />
        <asp:HiddenField ID="hidSelectedText" runat="server" />
        <div>
            <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300">
                <tr align="left">
                    <td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" selectedcontrol="btnHOS" />
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                            <tr>
                                <td>
                                    <uc2:HosTabs ID="HosTabs1" runat="server" selectedcontrol="cmdQRCode" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width: 1200px;">
                                        <tr>
                                            <td class="configTabBackground">
                                                <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                    <tr>
                                                        <td>
                                                            <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 1170px; margin-top: 10px; margin-bottom: 10px" class="tableDoubleBorder">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0"
                                                                                                    style="height: 700px">
                                                                                                    <tr valign="top">
                                                                                                        <td>

                                                                                                            <table id="Table8" cellspacing="0" cellpadding="0" width="870" align="center" border="0">
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <table id="Table9" class="table" width="1170px" height="700px"
                                                                                                                            border="0">
                                                                                                                            <tr>
                                                                                                                                <td class="configTabBackground" style="vertical-align: top; width: 100%">
                                                                                                                                    <table style="vertical-align: top; width: 100%">
                                                                                                                                        <tr>
                                                                                                                                                  <td colspan="10" align="left">
                                                                                                                                                         <table id="fleetTable" runat="server"> 
                                                                                                                                                            <tr>
                                                                                                                                                              <td>                                                                                                         
                                                                                                                                                                   <div id="ohfleetbar" class="formtext" runat="server" visible ="true">                                                                                                                                                                
                                                                                                                                                                     <asp:label id="lblFleet" runat="server" cssclass="tableheading" width="33px" meta:resourcekey="lblFleetResource1"
                                                                                                                                                                        text="Fleet:"></asp:label>
                                                                                                                                                                     <asp:dropdownlist id="cboFleet" runat="server" autopostback="True" cssclass="RegularText"
                                                                                                                                                                        width="258px" datatextfield="FleetName" datavaluefield="FleetId" 
                                                                                                                                                                        meta:resourcekey="cboFleetResource1" onselectedindexchanged="cboFleet_SelectedIndexChanged" >
                                                                                                                                                                     </asp:dropdownlist>                                                                                                                                                                    
                                                                                                                                                                     <asp:label id="lblVehicleName" runat="server" cssclass="tableheading" width="53px"
                                                                                                                                                                        meta:resourcekey="lblVehicleNameResource1" text="Vehicle:"></asp:label>                                                                                                                                                               
                                                                                                                                                                     <asp:dropdownlist id="cboVehicle" runat="server" cssclass="RegularText" width="258px" autopostback="True"
                                                                                                                                                                        datatextfield="Description" datavaluefield="BoxId" meta:resourcekey="cboVehicleResource1"  onselectedindexchanged="cboVehicle_SelectedIndexChanged">
                                                                                                                                                                     </asp:dropdownlist>                                                                                                                                                              
                                                                                                                                                                   </div>
                                                                                                                                                                 </td>
                                                                                                                                                            </tr>
                                                                                                                                                         </table>
                                                                                                                                                                                                                                                                                                                  
                                                                                                                                                         <table id="OrganizationHierarchyTable" runat ="server">                                                                                                                                             
                                                                                                                                                           <tr>
                                                                                                                                                              <td colspan="10" align="left">
                                                                                                                                                                <div id="ohsearchbar" class="formtext">
                                                                                                                                                                <asp:Label ID="lblSearchOrganizationHierarchy" runat="server" CssClass="formtextGreen" Text="Search Organization Hierarchy:" meta:resourcekey="lblSearchOrganizationHierarchyResource1"></asp:Label>
                                                                                                                                                                <input type="text" id="ohsearchbox" class="ohsearch" />
                                                                                                                                                                <a href="javascript:void(0);" onclick="onsearchbtnclicked('../reports/vehicleListTree.asmx/SearchOrganizationHierarchy');">
                                                                                                                                                                    <img src="../images/searchicon.png" border="0" /></a>
                                                                                                                                                                <asp:Label ID="Label10" runat="server" Style="color: #666666;" Text="(Type in at least 3 characters to search)"></asp:Label>                                                                                                                                                               
                                                                                                                                                            </div>
                                                                                                                                                              </td>
                                                                                                                                                               </tr>
                                                                                                                                                           <tr>
                                                                                                                                                              <td colspan="10" align="left">
                                                                                                                                                                  <div id="ohsearchresult">
                                                                                                                                                                <div id="ohsearchresulttitle">
                                                                                                                                                                    <%=msgSearchResult %>: <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">Close</a>
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
                                                                                                                                                                                    <th><%=msgVehicle %></th>

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
                                                                                                                                                                                    <span class="pagedisplay"></span>
                                                                                                                                                                                    <!-- this can be any element, including an input -->
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
                                                                                                                                                        </table>
                                                                                                                                                  </td>
                                                                                                                                            </tr>
                                                                                                                                        <tr>
                                                                                                                                            <td>
                                                                                                                                                <div>
                                                                                                                                                              <asp:Label ID="lblErrorMessage" runat="server" CssClass="errortext" Width="615px" meta:resourcekey="lblErrorMessageResource1"></asp:Label>
                                                                                                                                                      </div>  </td>
                                                                                                                                            
                                                                                                                                        </tr>
                                                                                                                                         <tr>
                                                                                                                                            
                                                                                                                                                  <td>
                                                                                                                                                      <div>
                                                                                                                                                        <asp:Button ID="btnShowQRCode" runat="server" Text="View Form" meta:resourcekey="btnShowQRCodeResource1" OnClientClick="javascript:showQRCode(); return false;" />
                                                                                                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                                                        <asp:Button ID="btnPrintQRCode" runat="server" Text="Print QR Code" meta:resourcekey="btnPrintQRCodeResource1" OnClientClick="javascript:printQRCode(); return false;" />
                                                                                                                                                       </div>
                                                                                                                                                  </td>
                                                                                                                                        </tr> 
                                                                                                                                        <tr>
                                                                                                                                                  <td colspan="10" align="left">
                                                                                                                                                            <asp:Label ID="lblAssignedtoVehicle" runat="server" CssClass="formtextGreen" Font-Bold="true" Text="" ForeColor="White" BackColor="Blue"></asp:Label>
                                                                                                                                                   </td>
                                                                                                                                        </tr>
                                                                                                                                        <tr>
                                                                                                                                                  <td colspan="10" align="left">
                                                                                                                                                     <!-- <div id="inspectiontreeview" class="demo"></div> -->
                                                                                                                                                       <div style="width: 100%; height: 600px; overflow: scroll; display: none" id="inspectiontreeview">
                                                                                                                                                                <ul style="border:solid;border-width:1px" id="ulInspections" class="jqueryFileTree">
                                                                                                                                                                    <li class="directory collapsed" id="li_0"><a id="lia_0" href="javascript:click_inspection('0')"><%= msgQuestionSet%></a></li>
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
                                </td>
                            </tr>
                        </table>                   
        </div>
    </form>
    <div id="divLoading" class="loadingdiv" border="0" style="display: none; z-index: 1000; position: absolute; width: 20px; height: 20px">
        <image src="../images/loading2.gif"></image>
    </div>
</body>
</html>
