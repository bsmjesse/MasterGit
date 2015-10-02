<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DriverControlTest.aspx.cs" Inherits="SentinelFM.UserControl_DriverControlTest" %>
<%@ Register src="../UserControl/DriverSearch.ascx" tagname="ctlDriverSearch" tagprefix="ucDriverSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
    <script type="text/javascript">
        function driver1Select(driverId) {
            $('#msg').html("Driver1 Selected: " + driverId);
        }

        function driver2Select(driverId) {
            $('#msg').html("Driver2 Selected: " + driverId);
        }
    </script>
</head>
<body>
    <h1>Driver Search Control Test Page</h1>
    <br /><br />
    <form id="form1" runat="server">
    <div>
        Driver1: <ucDriverSearch:ctlDriverSearch ID="ctlDriverSearch1" onDriverSelect="driver1Select" placeholder="Search..." runat="server" />
    </div>

   <div style="margin-top:20px;">
        Driver2: <ucDriverSearch:ctlDriverSearch ID="ctlDriverSearch2" loadResources="false" onDriverSelect="driver2Select" runat="server" />
    </div>
    </form>
    <div id="msg" style="margin-top:30px;"></div>
</body>
</html>
