<%@ Page language="c#" Inherits="SentinelFM.Reports.frmReportMain" CodeFile="frmReportMain.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head runat="server">
    <title>frmReportMain</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
   
	<frameset id="TopFrame" rows="*,24px" frameSpacing="0" border="0" bordercolor="gray" frameBorder="0">
		<frame name="report" src="<%=ReportDestinationUrl%>" scrolling=auto  frameborder="0" noresize>
		<frame name="reportback" src="frmReportBack.aspx" scrolling="no" frameborder="1" noresize>
	</frameset>
   
	
  
</html>
