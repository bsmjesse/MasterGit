<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EventViewer.aspx.cs" Inherits="SentinelFM.EventViewer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>EventViewer</title>
    <!-- ExtJS StyleSheet-->
    <link rel="stylesheet" type="text/css" href="./sencha/extjs-4.1.0/examples/shared/example.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="./sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />
    <link href="font-awesome-4.3.0/css/font-awesome.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="Scripts/css/EventStyle.css" type="text/css" id="ssgray"/>
   
    

    <!--Jquery-->
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.maskedinput.js"></script>
    <!-- ExtJS -->
    <script type="text/javascript" src="./sencha/extjs-4.1.0/ext-all.js"></script>
    <!-- UI Control JS -->
    <script type="text/javascript" src="./Scripts/NewMap/event.js"></script>
    <script type="text/javascript">
        if (!Array.prototype.indexOf) {
            Array.prototype.indexOf = function (obj, start) {
                for (var i = (start || 0), j = this.length; i < j; i++) {
                    if (this[i] === obj) { return i; }
                }
                return -1;
            }
        }
        var LoadVehiclesBasedOn = '<%=sn.User.LoadVehiclesBasedOn %>';
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var DefaultOrganizationHierarchyFleetName = '<%=DefaultOrganizationHierarchyFleetName%>';
        var DefaultOrganizationHierarchyFleetId = '<%=DefaultOrganizationHierarchyFleetId%>';
        var MutipleUserHierarchyAssignment = '<%=MutipleUserHierarchyAssignment.ToString().ToLower() %>';
        var vehicletreeviewIni = false;
        var selectedOrganizationHierarchyNodeCode = '';
        var selectedOrganizationHierarchyFleetName = ''
        var hierarchyBtnReference = 'vehiclelist';
        var userDate = '<%=sn.User.DateFormat %>';
        var userTime = '<%=sn.User.TimeFormat %>'; 
        var organizationColor= '<%=sn.User.MenuColor %>';
        var eventCol = '<%=sn.User.EventColumns %>';
        var violationCol = '<%=sn.User.ViolationColumns %>';
        var ScheventId = '<%= ScheventId %>';       
        var recordsToFetch = '<%= sn.User.RecordsToFetch %>'; 
        var defaultFleet = '<%= sn.User.DefaultFleet %>'; 
       

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
            {
                userTime = "h:i:s A";
              
            }
            else
            {
                userTime = "H:i:s";
               
            }
          
            return userDate + " " + userTime;
        }
        
       
        var userdateformat = getSenchaDateFormat();
        var defaultSelectedFleetid = '<%=sn.User.DefaultFleet %>';
        var defaultSelectedEvents = 'all';
    </script>


</head>
<body>
    <form id="form1" runat="server">
        <div id="form-ct" style="width: 100%">
        </div>
    </form>
</body>
</html>
