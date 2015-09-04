<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSReport.aspx.cs" Inherits="SentinelFM.HOS_frmHOSReport" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>




<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <link href="/aspnet_client/System_Web/2_0_50727/CrystalReportWebFormViewer3/css/default.css"
        rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cr:crystalreportviewer id="crReportViewer" runat="server" autodatabind="True" height="50px"
            reportsourceid="repHOS" width="350px" DisplayToolbar="False"></cr:crystalreportviewer>
        <cr:crystalreportsource id="repHOS" runat="server">
<Report FileName="HOSReport.rpt"></Report>
</cr:crystalreportsource>
    
    </div>
    </form>
</body>
</html>
