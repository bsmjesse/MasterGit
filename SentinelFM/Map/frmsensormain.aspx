<%@ Page language="c#" Inherits="SentinelFM.Map.frmSensorMain" CodeFile="frmSensorMain.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<TITLE>Sensors-Commands-Vehicle Info</TITLE>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<frameset border="0" frameSpacing="0" borderColor="#006699" frameBorder="0" cols="100%,0%">
		<frame  name=frmSensorsInfo src="frmSensorsWait.aspx?LicensePlate=<%=LicensePlate%>" noResize scrolling=auto>
		<frame name="frmSensorTimer" src="frmSensorTimer.aspx" scrolling="no" noresize>
	</frameset>
</HTML>
