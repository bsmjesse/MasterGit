<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="frmInspectionsAssignment.aspx.cs" Inherits="SentinelFM.HOS_frmInspectionsAssignment" %>
<%@ Register Src="../UserControl/FleetOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc3" %>
<%@ Register Src="../Configuration/Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="HosTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
<body onload="LoadOnFleetChange();">
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
    var selectedPreference ='<%=sn.User.LoadVehiclesBasedOn %>';
    var inspectionFormsJson = <%=inspectionFormsJson%>;
        var inspectionAssignedFormsJson = "";
        var IniHierarchyPath = <%=IniHierarchyPath.ToString().ToLower() %>;
        var rex = /AppleWebKit\/\d{1,3}\.\d{1,3}/
        var match = navigator.userAgent.match(rex);
        var isBox = false;
        var isDot = false;
        var dotType = 0;
        var currentSelectionFleet = "";
        var currentFleet = "";
        var OrganizationDOT = "<%=OrganizationDOT%>";
        var OrganizationType = "<%=OrganizationType%>";
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
        function LoadOnVehicleChange() 
        {
            if(document.getElementById('hdnVehicleDescription').value != "")
            {
                var selectedVehicleBoxId = document.getElementById('hdnVehicleDescription').value;
                $('#<%= lblAssignedtoFleet.ClientID%>').text(selectedVehicleBoxId);               
                isBox = true;
                isDot = false;                
                currentSelectionFleet = document.getElementById('hdnBoxId').value;
                currentFleet = 0;
                document.getElementById('hdnFleetId').value = "";
                
                getAssignedQuestionSet();
                document.getElementById('hdnBoxId').value = "";
                document.getElementById('hdnVehicleDescription').value = ""
                return true;
            }
        }
        function LoadOnFleetChange() 
        {
            LoadOnVehicleChange();
            LoadForms(document.getElementById('hdnFleetId').value,document.getElementById('hdnFleetName').value);
            
        }
        function LoadForms(FleetId, fleetName) 
        {
            if(FleetId != "")
            {
                $('#<%= lblAssignedtoFleet.ClientID%>').text(fleetName);
                isBox = false;
                isDot = false;
                currentFleet = FleetId;
                getAssignedQuestionSet();
                return true;
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
                            $('#<%= lblAssignedtoFleet.ClientID%>').text(fleetName);
                            isBox = false;
                            isDot = false;
                            currentFleet = 0;
                            currentSelectionFleet = NodeCode;
                            getAssignedQuestionSet();
                        },

            /*
            * Call back function when you click right pane vehicle list.
            */
                        function (BoxId, vehicleDescription) {
                            //alert('BoxId: ' + BoxId);
                            $('#<%= lblAssignedtoFleet.ClientID%>').text(vehicleDescription);
                            $('#OrganizationHierarchyBoxId').val(BoxId);
                            $('#OrganizationHierarchyVehicleDescription').val(vehicleDescription);
                            isBox = true;
                            isDot = false;
                            currentSelectionFleet = BoxId;
                            getAssignedQuestionSet();
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
		              if(selectedPreference != "fleet")
                            { 
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

                            FillQuestionSetList();
                            $('#txtSearchQuestionSet').keyup(function(){
                                FillQuestionSetList();
                            });
                            if (OrganizationDOT.length > 0)
                            {
                                $(OrganizationDOT).appendTo($('#vehicledotiew'));
                                $('#ul_dot_all').css("margin-left", $('#vehicletreeview').find(".jqueryFileTree" ).position().left);
                                
                            }
            
                            if (OrganizationType.length > 0)
                            {
                                $(OrganizationType).insertBefore($('#vehicletypeview'));
                                $('#ul_type_all').css("margin-left", $('#vehicletreeview').find(".jqueryFileTree" ).position().left);
                            }

                            //if (OrganizationDOT.length > 0)
                            //{
                                //$(OrganizationDOT).appendTo($('#vehicletreeview'));
                                //}
                                //}
                            }
                            else{
                                $('#txtSearchQuestionSet').keyup(function(){
                                    FillQuestionSetList(); });
                                FillQuestionSetList();
                            }
                        }
                        catch (ex) { 
                            alert(ex.message);
                        }
                    });
                    function FillQuestionSetList()
                    {
                        $("#<%= lstForms.ClientID %>").empty()
                        $("#<%= lstassignedForms.ClientID %>").empty()
                        if (inspectionFormsJson.length > 0)
                        {
                            var var_groupIds = "";
                            if (inspectionAssignedFormsJson.length > 0)
                            {
                                for(var index=0; index < inspectionAssignedFormsJson.length; index++)
                                {
                                    $("#<%= lstassignedForms.ClientID %>").append($('<option></option>').attr('value', inspectionAssignedFormsJson[index]["GroupId"]).text(inspectionAssignedFormsJson[index]["Name"]));
                                    var_groupIds = var_groupIds + "," + inspectionAssignedFormsJson[index]["GroupId"];
                                }
                            }
                            var_groupIds = var_groupIds + "," ;
                            for(var index=0; index < inspectionFormsJson.length; index++)
                            {
                                var searchMatch = $.trim($("#txtSearchQuestionSet").val());
                                if ((searchMatch == "" || inspectionFormsJson[index]["Name"].toLowerCase().indexOf(searchMatch.toLowerCase()) == 0) && var_groupIds.indexOf(inspectionFormsJson[index]["GroupId"]) < 0 )
                                {
                                    $("#<%= lstForms.ClientID %>").append($('<option></option>').attr('value', inspectionFormsJson[index]["GroupId"]).text(inspectionFormsJson[index]["Name"]));
                                }
                            }
                        }

                    }
                    //-->
                    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        
        function addCurrentSelectionToList(curGroupId, curGroupName, isAdd)
        {
            if (isAdd)
            {
                $("#<%= lstassignedForms.ClientID %>").append($('<option></option>').attr('value', curGroupId).text(curGroupName));
                $("#<%= lstForms.ClientID %> ").find("option[value='" + curGroupId + "']").remove();
            }
            else
            {
                $("#<%= lstForms.ClientID %>").append($('<option></option>').attr('value', curGroupId).text(curGroupName));
                $("#<%= lstassignedForms.ClientID %> ").find("option[value='" + curGroupId + "']").remove();
            }
        }

        function getAssignedQuestionSet()
        {
            $('#lblErrorMessage').html('');
            if (currentSelectionFleet == "" && currentFleet == "")
            {
                $('#lblErrorMessage').html('<%= msgSelectAfleet %>');
                return;
            }
            if (currentFleet == "") currentFleet = -1;
            showLoadingImage("", true);
            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: "frmInspectionsAssignment.aspx/GetInspectionGroupByHierarchy",
                data: JSON.stringify({
                    currentSelection: currentSelectionFleet,
                    isBox: isBox,
                    isDOT: isDot,
                    dotType: dotType,
                    fleetId : currentFleet


                }),
                dataType: "json",
                success: function (data) {
                    if (data.d != '-1' && data.d != "0") {
                        if (data.d.length > 0)
                        {
                            inspectionAssignedFormsJson = JSON.parse(data.d);
                        }
                        else inspectionAssignedFormsJson = "";
                    }
                    FillQuestionSetList();
                    if (data.d == '-1') {
                        //top.document.all('TopFrame').cols = '0,*';
                        window.open('../Login.aspx', '_top')
                    }
                    if (data.d == '0') {
                        $('#lblErrorMessage').html('<%= msgFailedtoLoadData%>');
                }
                    showLoadingImage("", false);
                },
                error: function (request, status, error) {
                    $('#lblErrorMessage').html('<%= msgFailedtoLoadData%>');
                    showLoadingImage("", false);
                }
            });

            }

            function AddOrUpdateLogData_InspectionGroupAssignment(isAdd)
            {
                $('#lblErrorMessage').html('');
                if (currentSelectionFleet == "" && currentFleet == "")
                {
                    $('#lblErrorMessage').html('<%= msgSelectAfleet %>');
                return;
            }
            if (currentFleet == "") currentFleet = -1;
            var var_groupid = "";
            var var_groupName = "";
            var var_id = "";
            if (isAdd)
            {
                var_groupid = $("#<%= lstForms.ClientID %> option:selected").val();
                var_groupName = $("#<%= lstForms.ClientID %> option:selected").text();
                var_id = "<%= btnAssign.ClientID %>";
            }
            else 
            {
                var_groupid = $("#<%= lstassignedForms.ClientID %> option:selected").val();
                var_groupName = $("#<%= lstassignedForms.ClientID %> option:selected").text();
                var_id = "<%= btnUnassign.ClientID %>";
            }

            if (var_groupid == undefined || var_groupid.length =="" )
            {
                $('#lblErrorMessage').html('<%= msgSelectQuestionet %>');
                return;
            }
                
            showLoadingImage(var_id, true);
            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: "frmInspectionsAssignment.aspx/AddOrUpdateLogData_InspectionGroupAssignment",
                data: JSON.stringify({
                    groupId:var_groupid,
                    currentSelection: currentSelectionFleet,
                    isBox: isBox,
                    isAdd:isAdd,
                    isDOT:isDot,
                    dotType: dotType,
                    fleetId : currentFleet

                }),
                dataType: "json",
                success: function (data) {
                    if (data.d != '-1' && data.d != "0") {
                        addCurrentSelectionToList(var_groupid, var_groupName, isAdd)
                    }
                    if (data.d == '-1') {
                        //top.document.all('TopFrame').cols = '0,*';
                        window.open('../Login.aspx', '_top')
                    }
                    if (data.d == '0') {
                        $('#lblErrorMessage').html('<%= msgFailedtoSave%>');
                }
                    showLoadingImage(var_id, false);
                },
                error: function (request, status, error) {
                    $('#lblErrorMessage').html('<%= msgFailedtoSave%>');
                    showLoadingImage(var_id, false);
                }
            });
                return false;
            }
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
            function btnDot_Click(dotId)
            {
                $('#lblErrorMessage').html('');
                $('UL.jqueryFileTree A').removeClass('current');
                $("#" + dotId).addClass('current');
                isDot = true;
                isBox = false;
                dotType = "1";
                var dotselection = $("#" + dotId).text();
                curWaitId = $("#" + dotId).parent();
                curWaitId.addClass('wait');
                $('#<%= lblAssignedtoFleet.ClientID%>').text(dotselection);
                currentFleetId = "@@DOT_" + $("#" + dotId).text();
                currentSelectionFleet = dotselection;
                BindDOTVehicles();
                getAssignedQuestionSet();
            }

        function btnObjectType_Click(typeId)
        {   
            $('#lblErrorMessage').html('');
            $('UL.jqueryFileTree A').removeClass('current');
            $("#" + typeId).addClass('current');
            isDot = true;
            isBox = false;
            dotType = "2";
            var typeselection = $("#" + typeId).text();
            curWaitId = $("#" + typeId).parent();
            curWaitId.addClass('wait');
            $('#<%= lblAssignedtoFleet.ClientID%>').text(typeselection);
            currentFleetId = "@@Typ_" + $("#" + typeId).text();
            currentSelectionFleet = typeselection;
                BindTypeVehicles();
                getAssignedQuestionSet();
        }

        function BindTypeVehicles()
        {           
            $("#vehicledetails table").tablesorter({
                theme: 'blue',

                headers: {
                    // disable sorting
                    0: {
                        // disable it by setting the property sorter to false
                        sorter: false
                    },
                    1: {
                        // disable it by setting the property sorter to false
                        sorter: false
                    }
                },

                // hidden filter input/selects will resize the columns, so try to minimize the change
                widthFixed: true,

                // initialize zebra striping and filter widgets
                widgets: ["filter"]
            })
            .tablesorterPager({

                // **********************************
                //  Description of ALL pager options
                // **********************************

                // target the pager markup - see the HTML block below
                //container: $('#' + o.VehicleListPagingBarId), //$(".pager"),
                container: $(".pager"),

                // use this format: "http:/mydatabase.com?page={page}&size={size}&{sortList:col}"
                // where {page} is replaced by the page number (or use {page+1} to get a one-based index),
                // {size} is replaced by the number of records to show,
                // {sortList:col} adds the sortList to the url into a "col" array, and {filterList:fcol} adds
                // the filterList to the url into an "fcol" array.
                // So a sortList = [[2,0],[3,0]] becomes "&col[2]=0&col[3]=0" in the url
                // and a filterList = [[2,Blue],[3,13]] becomes "&fcol[2]=Blue&fcol[3]=13" in the url

                //ajaxUrl: '../reports/vehicleListTree.asmx/FetchVehicleListFilterByPage?fleetid=' + currentFleetId + '&page={page}&{filterList:filter}', //'http://mottie.github.io/tablesorter/docs/assets/City{page}.json?{filterList:filter}&{sortList:column}',
                ajaxUrl: '../Reports/vehicleListTree.asmx/GetVehicleListByFilterByFleetIdForType?fleetid=' + escape(currentFleetId) + '&manager=false&page={page}&{filterList:filter}', //'http://mottie.github.io/tablesorter/docs/assets/City{page}.json?{filterList:filter}&{sortList:column}',

                // modify the url after all processing has been applied
                customAjaxUrl: function (table, url) {
                    // manipulate the url string as you desire
                    // url += '&cPage=' + window.location.pathname;
                    // trigger my custom event
                    $(table).trigger('changingUrl', url);
                    // send the server the current page
                    return url;
                },

                // add more ajax settings here
                // see http://api.jquery.com/jQuery.ajax/#jQuery-ajax-settings
                ajaxObject: {
                    dataType: 'json'
                },

                // process ajax so that the following information is returned:
                // [ total_rows (number), rows (array of arrays), headers (array; optional) ]
                // example:
                // [
                //   100,  // total rows
                //   [
                //     [ "row1cell1", "row1cell2", ... "row1cellN" ],
                //     [ "row2cell1", "row2cell2", ... "row2cellN" ],
                //     ...
                //     [ "rowNcell1", "rowNcell2", ... "rowNcellN" ]
                //   ],
                //   [ "header1", "header2", ... "headerN" ] // optional
                // ]
                // OR
                // return [ total_rows, $rows (jQuery object; optional), headers (array; optional) ]
                ajaxProcessing: function (data) {
                   
                    //$('#vehicledetails table tr.vehicleitem').remove();
                    $('#vehicledetails table').find('tbody').empty()
                    //$('#vehicledetails table tr.current').remove();
                    $('#vehicledetails table tbody').append(data.vehiclelist);
                  
                    //$("#vehiclelisttbl").trigger("update");
                    $("#vehicledetails table").trigger("update");

                    bindVehiclelist($('#vehicledetails table'));
                    highlightVehicleList();
                    if (curWaitId != null)
                        curWaitId.removeClass('wait');
                    return [data.totalVehicles, null];
                },

                // output string - default is '{page}/{totalPages}'; possible variables: {page}, {totalPages}, {startRow}, {endRow} and {totalRows}
                //output: '{startRow} to {endRow} ({totalRows})',
                output: '{page}/{totalPages}',

                // apply disabled classname to the pager arrows when the rows at either extreme is visible - default is true
                updateArrows: true,

                // starting page of the pager (zero based index)
                page: 0,

                // Number of visible rows - default is 10
                size: 10,

                // if true, the table will remain the same height no matter how many records are displayed. The space is made up by an empty
                // table row set to a height to compensate; default is false
                fixedHeight: false,

                // remove rows from the table to speed up the sort of large tables.
                // setting this to false, only hides the non-visible rows; needed if you plan to add/remove rows with the pager enabled.
                removeRows: false,

                // css class names of pager arrows
                cssNext: '.next',  // next page arrow
                cssPrev: '.prev',  // previous page arrow
                cssFirst: '.first', // go to first page arrow
                cssLast: '.last',  // go to last page arrow
                cssPageDisplay: '.pagedisplay', // location of where the "output" is displayed
                cssPageSize: '.pagesize', // page size selector - select dropdown that sets the "size" option
                cssErrorRow: 'tablesorter-errorRow', // error information row

                // class added to arrows when at the extremes (i.e. prev/first arrows are "disabled" when on the first page)
                cssDisabled: 'disabled' // Note there is no period "." in front of this class name

            });

            //$('#vehiclelisttbl').colResizable({ headerOnly: true });
            $("#vehicledetails table").colResizable({ headerOnly: true });
        }

        function BindDOTVehicles()
        {
            $("#vehicledetails table").tablesorter({
                theme: 'blue',

                headers: {
                    // disable sorting
                    0: {
                        // disable it by setting the property sorter to false
                        sorter: false
                    },
                    1: {
                        // disable it by setting the property sorter to false
                        sorter: false
                    }
                },

                // hidden filter input/selects will resize the columns, so try to minimize the change
                widthFixed: true,

                // initialize zebra striping and filter widgets
                widgets: ["filter"]
            })
            .tablesorterPager({

                // **********************************
                //  Description of ALL pager options
                // **********************************

                // target the pager markup - see the HTML block below
                //container: $('#' + o.VehicleListPagingBarId), //$(".pager"),
                container: $(".pager"),

                // use this format: "http:/mydatabase.com?page={page}&size={size}&{sortList:col}"
                // where {page} is replaced by the page number (or use {page+1} to get a one-based index),
                // {size} is replaced by the number of records to show,
                // {sortList:col} adds the sortList to the url into a "col" array, and {filterList:fcol} adds
                // the filterList to the url into an "fcol" array.
                // So a sortList = [[2,0],[3,0]] becomes "&col[2]=0&col[3]=0" in the url
                // and a filterList = [[2,Blue],[3,13]] becomes "&fcol[2]=Blue&fcol[3]=13" in the url

                //ajaxUrl: '../reports/vehicleListTree.asmx/FetchVehicleListFilterByPage?fleetid=' + currentFleetId + '&page={page}&{filterList:filter}', //'http://mottie.github.io/tablesorter/docs/assets/City{page}.json?{filterList:filter}&{sortList:column}',
                ajaxUrl: '../Reports/vehicleListTree.asmx/GetVehicleListByFilterByFleetIdForDOT?fleetid=' + escape(currentFleetId) + '&manager=false&page={page}&{filterList:filter}', //'http://mottie.github.io/tablesorter/docs/assets/City{page}.json?{filterList:filter}&{sortList:column}',

                // modify the url after all processing has been applied
                customAjaxUrl: function (table, url) {
                    // manipulate the url string as you desire
                    // url += '&cPage=' + window.location.pathname;
                    // trigger my custom event
                    $(table).trigger('changingUrl', url);
                    // send the server the current page
                    return url;
                },

                // add more ajax settings here
                // see http://api.jquery.com/jQuery.ajax/#jQuery-ajax-settings
                ajaxObject: {
                    dataType: 'json'
                },

                // process ajax so that the following information is returned:
                // [ total_rows (number), rows (array of arrays), headers (array; optional) ]
                // example:
                // [
                //   100,  // total rows
                //   [
                //     [ "row1cell1", "row1cell2", ... "row1cellN" ],
                //     [ "row2cell1", "row2cell2", ... "row2cellN" ],
                //     ...
                //     [ "rowNcell1", "rowNcell2", ... "rowNcellN" ]
                //   ],
                //   [ "header1", "header2", ... "headerN" ] // optional
                // ]
                // OR
                // return [ total_rows, $rows (jQuery object; optional), headers (array; optional) ]
                ajaxProcessing: function (data) {

                        //$('#vehicledetails table tr.vehicleitem').remove();
                          $('#vehicledetails table').find('tbody').empty()
                        //$('#vehicledetails table tr.current').remove();
                          $('#vehicledetails table tbody').append(data.vehiclelist);                       

                        //$("#vehiclelisttbl").trigger("update");
                        $("#vehicledetails table").trigger("update");                      

                        bindVehiclelist($('#vehicledetails table'));
                        highlightVehicleList();
                        if (curWaitId != null)
                           curWaitId.removeClass('wait');
                    return [data.totalVehicles, null];
                },

                // output string - default is '{page}/{totalPages}'; possible variables: {page}, {totalPages}, {startRow}, {endRow} and {totalRows}
                //output: '{startRow} to {endRow} ({totalRows})',
                output: '{page}/{totalPages}',

                // apply disabled classname to the pager arrows when the rows at either extreme is visible - default is true
                updateArrows: true,

                // starting page of the pager (zero based index)
                page: 0,

                // Number of visible rows - default is 10
                size: 10,

                // if true, the table will remain the same height no matter how many records are displayed. The space is made up by an empty
                // table row set to a height to compensate; default is false
                fixedHeight: false,

                // remove rows from the table to speed up the sort of large tables.
                // setting this to false, only hides the non-visible rows; needed if you plan to add/remove rows with the pager enabled.
                removeRows: false,

                // css class names of pager arrows
                cssNext: '.next',  // next page arrow
                cssPrev: '.prev',  // previous page arrow
                cssFirst: '.first', // go to first page arrow
                cssLast: '.last',  // go to last page arrow
                cssPageDisplay: '.pagedisplay', // location of where the "output" is displayed
                cssPageSize: '.pagesize', // page size selector - select dropdown that sets the "size" option
                cssErrorRow: 'tablesorter-errorRow', // error information row

                // class added to arrows when at the extremes (i.e. prev/first arrows are "disabled" when on the first page)
                cssDisabled: 'disabled' // Note there is no period "." in front of this class name

            });

            //$('#vehiclelisttbl').colResizable({ headerOnly: true });
            $("#vehicledetails table").colResizable({ headerOnly: true });
           
        }
        function bindVehiclelist(v) {            
            $(v).find('tbody tr td').bind('click', function () {
                    $('#vehicledetails table tbody tr').removeClass('current');
                    $(this).parent().addClass('current');
                    var var_BoxId = $(this).parent().attr('rel');
                    var var_vehicleDescription = $(this).html();
                    $('#<%= lblAssignedtoFleet.ClientID%>').text(var_vehicleDescription);

                    $('#OrganizationHierarchyBoxId').val(var_BoxId);
                    $('#OrganizationHierarchyVehicleDescription').val(var_vehicleDescription);
                    isBox = true;
                    isDot = false;
                    currentSelectionFleet = var_BoxId;
                    getAssignedQuestionSet();

            });
        }
        function highlightVehicleList() {
            $("#vehiclelisttbl tr").click(function () {
                var row = $(this);

                $('#vehiclelisttbl tr').removeClass('current');
                $('#vehiclelisttbl tr').addClass("vehicleitem");

                var VehicleClass = row.hasClass("vehicleitem");
                var SelectVehicleClass = row.hasClass("current");

                if (VehicleClass) {
                    row.removeClass("vehicleitem");
                    row.addClass("current");
                }
            });
        }

        function btnDot_All_Click()
        {
            var controls =$('#ul_dot_all').find('[id^="li_dot_"]');
            if (controls.length > 0)
            {
                var isVisible = false;
                if ($(controls[0]).css('display') == "none")
                {
                    isVisible = true;
                }
                controls.each(function() {
                    if (isVisible) 
                        $( this ).show();
                    else 
                        $( this ).hide();
                });
            }
        }

        function btnObjectType_All_Click()
        {
            var controls =$('#ul_type_all').find('[id^="li_type_"]');
            if (controls.length > 0)
            {
                var isVisible = false;
                if ($(controls[0]).css('display') == "none")
                {
                    isVisible = true;
                }
                controls.each(function() {
                    if (isVisible) 
                        $( this ).show();
                    else 
                        $( this ).hide();
                });
            }
        }

    </script>


    <form id="form1" runat="server">
        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hdnNodeCode" runat="server" />
        <asp:HiddenField ID="hdnFleetId" runat="server" />
        <asp:HiddenField ID="hdnFleetName" runat="server" />
        <asp:HiddenField ID="hdnBoxId" runat="server" />
        <asp:HiddenField ID="hdnVehicleDescription" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetName" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyNodeCode" Value="" runat="server" />
        <input type="hidden" name="OrganizationHierarchyFleetId" id="OrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyBoxId" Value="" runat="server" />
        <asp:HiddenField ID="OrganizationHierarchyVehicleDescription" Value="" runat="server" />

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
                                    <uc2:HosTabs ID="HosTabs1" runat="server" selectedcontrol="cmdQuestionSetAssignment" />
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
                                                                                                                                            <td>
                                                                                                                                                <table>
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
                                                                                                                                                                        datatextfield="Description" datavaluefield="BoxId" meta:resourcekey="cboVehicleResource1"  onselectedindexchanged="cboVehicle_SelectedIndexChanged" onChange="javascript:LoadOnVehicleChange()">
                                                                                                                                                                     </asp:dropdownlist>                                                                                                                                                              
                                                                                                                                                                   </div>
                                                                                                                                                                 </td>
                                                                                                                                                                </tr>
                                                                                                                                                           </table>
                                                                                                                                                            <table id="OrganizationHierarchyTable" runat ="server">
                                                                                                                                                               <tr>
                                                                                                                                                                 <td> 
                                                                                                                                                                  <div id="ohsearchbar"   class="formtext">
                                                                                                                                                                  <asp:Label ID="lblSearchOrganizationHierarchy" runat="server" CssClass="formtextGreen" Text="Search Organization Hierarchy:" meta:resourcekey="lblSearchOrganizationHierarchyResource1"></asp:Label>
                                                                                                                                                                  <input type="text" id="ohsearchbox" class="ohsearch" />
                                                                                                                                                                  <a href="javascript:void(0);" onclick="onsearchbtnclicked('../reports/vehicleListTree.asmx/SearchOrganizationHierarchy');">
                                                                                                                                                                    <img src="../images/searchicon.png" border="0" /></a>
                                                                                                                                                                <asp:Label ID="Label10" runat="server" Style="color: #666666;" Text="(Type in at least 3 characters to search)"></asp:Label>
                                                                                                                                                            </div>
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
                                                                                                                                                                    <div id="vehicletypeview" class="demo"></div>
                                                                                                                                                                    <div id="vehicledotiew" class="demo"></div>
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
                                                                                                                                                        <td style="text-align: center">
                                                                                                                                                            <asp:Label ID="lblErrorMessage" runat="server" CssClass="errortext" Width="615px" meta:resourcekey="lblErrorMessageResource1"></asp:Label>
                                                                                                                                                        </td>
                                                                                                                                                    </tr>
                                                                                                                                                    <tr>
                                                                                                                                                        <td>
                                                                                                                                                            <table>
                                                                                                                                                                <tr>
                                                                                                                                                                    <td>
                                                                                                                                                                        <asp:Label ID="lblAssignedto" runat="server" CssClass="formtextGreen" Font-Bold="true" Text="Select a question form assign to:" meta:resourcekey="lblAssigntoResource1"></asp:Label>
                                                                                                                                                                    </td>
                                                                                                                                                                    <td>
                                                                                                                                                                        <asp:Label ID="lblAssignedtoFleet" runat="server" CssClass="formtextGreen" Font-Bold="true" Text="" ForeColor="White" BackColor="Blue"></asp:Label>
                                                                                                                                                                    </td>
                                                                                                                                                                </tr>
                                                                                                                                                            </table>
                                                                                                                                                        </td>
                                                                                                                                                    </tr>
                                                                                                                                                    <tr>
                                                                                                                                                        <td>
                                                                                                                                                            <table>
                                                                                                                                                                <tr>
                                                                                                                                                                    <td>
                                                                                                                                                                        <table>
                                                                                                                                                                            <tr>
                                                                                                                                                                                <div class="formtext">
                                                                                                                                                                                    <asp:Label ID="lblSearchQuestionSet" runat="server" CssClass="formtextGreen" Text="Search:" meta:resourcekey="lblSearchQuestionSetSetResource1"></asp:Label>
                                                                                                                                                                                    <input placeholder="" id="txtSearchQuestionSet" type="text" />
                                                                                                                                                                                </div>
                                                                                                                                                                            </tr>
                                                                                                                                                                            <tr>

                                                                                                                                                                                <td>
                                                                                                                                                                                    <asp:ListBox ID="lstForms" runat="server" Width="350px" Height="300px"></asp:ListBox>
                                                                                                                                                                                </td>
                                                                                                                                                                            </tr>

                                                                                                                                                                        </table>
                                                                                                                                                                    </td>
                                                                                                                                                                    <td>
                                                                                                                                                                        <table>
                                                                                                                                                                            <tr>
                                                                                                                                                                                <td>
                                                                                                                                                                                    <asp:Button Width="100px" ID="btnAssign" runat="server" Text="Assign" meta:resourcekey="btnAssignResource1" OnClientClick="javascript:AddOrUpdateLogData_InspectionGroupAssignment(true);return false;" />
                                                                                                                                                                                </td>
                                                                                                                                                                            </tr>
                                                                                                                                                                            <tr>
                                                                                                                                                                                <td>&nbsp;</td>
                                                                                                                                                                            </tr>
                                                                                                                                                                            <tr>
                                                                                                                                                                                <td>
                                                                                                                                                                                    <asp:Button Width="100px" ID="btnUnassign" runat="server" Text="UnAssign" meta:resourcekey="btnUnassignResource1" OnClientClick="javascript:AddOrUpdateLogData_InspectionGroupAssignment(false);return false;" />
                                                                                                                                                                                </td>
                                                                                                                                                                            </tr>
                                                                                                                                                                        </table>
                                                                                                                                                                    </td>
                                                                                                                                                                    <td>
                                                                                                                                                                        <table>
                                                                                                                                                                            <tr>
                                                                                                                                                                                <td>
                                                                                                                                                                                    <div class="formtext">
                                                                                                                                                                                        <asp:Label ID="lblAssignedQuestionSet" runat="server" CssClass="formtextGreen" Text="Assigned Question Form" meta:resourcekey="lblAssignedQuestionSetResource1"></asp:Label>
                                                                                                                                                                                    </div>
                                                                                                                                                                                </td>
                                                                                                                                                                            </tr>
                                                                                                                                                                            <tr>
                                                                                                                                                                                <td>
                                                                                                                                                                                    <asp:ListBox ID="lstassignedForms" runat="server" Width="350px" Height="300px"></asp:ListBox>
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
        <div id="divLoading" class="loadingdiv" border="0" style="display: none; z-index: 1000; position: absolute; width: 20px; height: 20px">
            <image src="../images/loading2.gif"></image>
        </div>
        
    </form>
</body>
</html>
