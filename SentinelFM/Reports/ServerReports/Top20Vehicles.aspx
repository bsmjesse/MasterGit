<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Top20Vehicles.aspx.cs" Inherits="Top20Vehicles" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Top 20 Vehicles Chart Reprot</title>
    <style type="text/css"> 
        html, body, form { width: 100%; height: 100%; margin: 0; padding: 0 } 
    </style> 
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align:center;">
        <table style="text-align:center;">
            <tr><td style="text-align: center;"><h1>Top 20 Vehicle Chart Report</h1></td></tr>
            <tr><td style="text-align: right;"><a id="EXIT" href="Default.aspx">Home</a></td></tr>
            <tr><td style="text-align: center;"><rsweb:ReportViewer ID="ServerReportViewer" runat="server"></rsweb:ReportViewer></td></tr>
            <tr><td style="visibility: hidden;"><asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager></td></tr>
        </table>
    </div>
    </form>
</body>
</html>
