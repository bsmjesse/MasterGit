<%@ Page Language="C#" AutoEventWireup="true" CodeFile="reportViewer.aspx.cs" Inherits="SentinelFM.DashBoard_reportViewer" %>

<%@ Register assembly="BusyBoxDotNet" namespace="BusyBoxDotNet" tagprefix="busyboxdotnet" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        
    </div>
                                                <busyboxdotnet:BusyBox ID="BusyReport" runat="server" AnchorControl="" 
                                                    ShowBusyBox=OnLoad  
        SlideEasing="BackBoth" Text="Preparing the Report" CompressScripts="False"
                                                    GZipCompression="False"></busyboxdotnet:BusyBox>
    </form>
</body>
</html>
