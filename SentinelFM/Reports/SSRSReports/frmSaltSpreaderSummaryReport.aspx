<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmSaltSpreaderSummaryReport.aspx.cs" Inherits="SentinelFM.Reports_SSRSReports_frmSaltSpreaderSummaryReport" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
          <rsweb:ReportViewer ID="ServerReportViewer" runat="server"  Visible= "true"  
            AsyncRendering="true" 
            SizeToReportContent="true" 
            ProcessingMode="Remote"
            ShowParameterPrompts="false"
            ZoomMode="FullPage"
            ShowZoomControl="false"  
            ShowReportBody="true" Height="29px" Width="100%"  
            >
            <ServerReport ReportServerUrl="" />
        </rsweb:ReportViewer>
    
    </div>

       <div id="divScriptManager" style="visibility:hidden;height:0px;width:0px;">
        <asp:Label ID="lblReportID" runat="server" Visible="false" /> 
        <asp:scriptmanager ID="Scriptmanager1" runat="server" />
    </div>

    </form>
</body>
</html>
