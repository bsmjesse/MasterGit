<%@ Page language="c#" Inherits="SentinelFM.frmMain" CodeFile="frmMain.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat=server  >
		<TITLE>Map</TITLE>
		<meta content="Microsoft Visual Studio.NET 7.0" name="GENERATOR">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<frameset id="MiddleFrame" border="0" frameSpacing="0" borderColor="#0066cc" rows="60%,*" frameBorder="0">
		<frameset border="0" frameSpacing="0" borderColor="#0066cc" frameBorder="0" cols="<%=mainFrameWidth%>">
			<frame name="frmVehicleMap" borderColor="#0066cc" src="<%=strMapForm%>" frameBorder="0" scrolling="no" noresize>
			<frameset border="0" frameSpacing="0" rows="<%=AlarmsScrollingHight%>,<%=MDTMessagesScrollingHight%>,<%=MDTMessageButtonHight%>" frameBorder="0">
				<!--<frame name="frmAlarmRotating" src="frmAlarmMainFrame.aspx" scrolling="no" noresize>-->
				<frame name="frmAlarmRotating"  src="<%=alarmPage%>" noresize>
				<!--<frame name="frmMessageRotating" src="frmMessageMainFrame.aspx" scrolling="no" noresize>-->
				<!--<frame name="frmMessageRotating" src="frmMessageRotatingCallBack.aspx" scrolling="no" noresize>-->
				<frame name="frmMessageRotating" src="frmMessageRotatingClient.aspx" noresize>
				<frame name="frmMessage" src="frmMessage.aspx" noresize>
			</frameset>
		</frameset>
		<frameset cols="100%,*">
			<frame name="frmFleetInfo" borderColor="#0066cc" src="frmFleetInfoNew.aspx" scrolling="no" noresize>
			<frame name="frmStatus" src="frmTimerPosition.aspx" scrolling="no" noresize>
		</frameset>
	</frameset>
</HTML>
