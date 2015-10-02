<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HOSDashBoard.aspx.cs" Inherits="SentinelFM.HOS_HOSDashBoard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
    <title>Charts</title>
    <%--<link rel="stylesheet" type="text/css" href="../ExtJS/resources/css/ext-all.css" />
    <link rel="stylesheet" type="text/css" href="../ExtJS/examples/shared/example.css" />--%>
    <%--<script type="text/javascript" src="../ExtJS/bootstrap.js"></script>--%>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />   
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>    
    <%--<script type="text/javascript" src="../Map/ExtJS/HOSDriverStatus.js"></script>--%>
    <script type="text/javascript" src="../Map/ExtJS/HOSDriverStatus.js?v=<%=LastUpdatedHOSDriverStatusJS %>"></script>
    <script type="text/javascript" src="../sencha/Ext.ux.Exporter/Exporter.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>

    <script type="text/javascript">
        var LoadVehiclesBasedOn = '<%=sn.User.LoadVehiclesBasedOn%>';
        var MutipleUserHierarchyAssignment = false;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';         
        var DefaultOrganizationHierarchyFleetId = '<%=DefaultOrganizationHierarchyFleetId %>';
        var DefaultOrganizationHierarchyFleetName = '<%=DefaultOrganizationHierarchyFleetName %>';
        var PreferOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        var HistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
        

        var selectedOrganizationHierarchyNodeCode = '';
        var selectedOrganizationHierarchyFleetName = '';
        var hierarchyBtnReference = 'vehiclelist';
     
        var userDate = '<%=sn.User.DateFormat %>';
        var userTime = '<%=sn.User.TimeFormat %>';
        function getSenchaDateFormat() {
            if (userDate == 'dd/MM/yyyy')
                userDate = 'd/m/Y';
            else if (userDate == 'd/M/yyyy')
                userDate = 'j/n/Y';
            else if (userDate == 'dd/MM/yy')
                userDate = 'd/m/y';
            else if (userDate == 'd/M/yy')
                userDate = 'j/n/y';
            else if (userDate == 'd MMM yyyy')
                userDate = 'j M Y';
            else if (userDate == 'MM/dd/yyyy')
                userDate = 'm/d/Y';
            else if (userDate == 'M/d/yyyy')
                userDate = 'n/j/Y';
            else if (userDate == 'MM/dd/yy')
                userDate = 'm/d/y';
            else if (userDate == 'M/d/yy')
                userDate = 'n/j/y';
            else if (userDate == 'MMMM d yy')
                userDate = 'M j y';
            else if (userDate == 'yyyy/MM/dd')
                userDate = 'Y/m/d';
            if (userTime == "hh:mm:ss tt")
                userTime = "h:i:s A";
            else
                userTime = "H:i:s";
            return userDate + " " + userTime;
        }
        var userdateformat = getSenchaDateFormat();
       


        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage = '../Widgets/OrganizationHierarchy.aspx?nodecode=' + DefaultOrganizationHierarchyNodeCode + '&loadVehicle=0&sl=1';
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0&rootNodecode=";
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
            $('#organizationHierarchyTree').hide();
            DefaultOrganizationHierarchyFleetId = TempSelectedOrganizationHierarchyFleetId;
            DefaultOrganizationHierarchyNodeCode = selectedOrganizationHierarchyNodeCode;
            loadingMask.show();
            mainstore.load(
                    {
                        params:
                        {
                            FleetId: DefaultOrganizationHierarchyFleetId,
                            start: 0,
                            limit: DriverListPagesize
                        }
                    });
        }

        function ShowFrmManagingHOSWindow(DriverName) {
            var mypage = 'frmManagingHOS.aspx?DriverName=' + DriverName;
					var myname='ManagingHOS';
					var w=909;
					var h=755;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
            }
     
        var DefaultFleetID = '<%=sn.User.DefaultFleet %>'; 
        var DefaultFleetName = '<%=DefaultFleetName %>';

        var SelectedFleetId = DefaultFleetID;
        var SelectedFleetName = DefaultFleetName;

        var LoadVehiclesBasedOn = '<%=LoadVehiclesBasedOn %>';

        var ResfleetButtonOpenwindowMessage = "<%=ResfleetButtonOpenwindowMessage %>";
        var DriverListPagesize = '35';
         
        </script>

     <style type="text/css">
         .map {
            background: url("../Styles/SentinelFM/resources/icons/map.png") no-repeat scroll 0 0 transparent !important;
        }
        
        .x-btn-icon 
        {
        	margin: 0 auto !important;
        }
     </style>
</head>
    <body id="docbody">
        <h1>Driver's hours of service</h1>
	    <div style="margin: 10px;">
        </div> <%if (LoadVehiclesBasedOn == "hierarchy")
  {%>
<div id="organizationHierarchyTree" style="display:none;">
    <div id="ohsearchbar" class="formtext"><asp:Label ID="Label8" runat="server" 
            CssClass="tableheading" Text="Search: " meta:resourcekey="Label8Resource1"></asp:Label>
        <input type="text" id="ohsearchbox" class="ohsearch" />
        <a href="javascript:void(0);" onclick="onsearchbtnclicked('reports/vehicleListTree.asmx/SearchOrganizationHierarchy');"><img src="../images/searchicon.png" border="0" /></a>
        <asp:Label ID="Label10" runat="server" style="color:#666666;display:none" 
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


        <iframe id="exportframe" name="exportframe" style="display:none"></iframe>
        <form id="exportform" method="post" target="exportframe" action="../MapNew/frmExportData.aspx">
            <input type="hidden" id="exportdata" name="exportdata" value="" />
            <input type="hidden" id="filename" name="filename" value="" />
            <input type="hidden" id="formatter" name="formatter" value="" />
        </form>
    </body>
</html>
