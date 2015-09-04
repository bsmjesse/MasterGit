<%@ Page language="c#" Inherits="SentinelFM.History.frmHistMain" CodeFile="frmHistMain.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>frmHistMain</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0"/>
		<meta name="CODE_LANGUAGE" Content="C#"/>
		<meta name="vs_defaultClientScript" content="JavaScript"/>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	</head>
	<frameset rows="340px,*" framespacing="0" border="0" bordercolor="#006699" frameborder="0">
		<frameset  cols="*,265px"  framespacing="0" border="0" bordercolor="#006699"  frameborder="0">
	   	<frame name="frmHisMap"  src="<%=strHistoryForm%>" noresize scrolling="no">
	   	<frame name="frmHisCrt" src="frmHistoryCrt.aspx" noresize scrolling="no">
    </frameset>
   	<frame name="frmHis" src="frmHistDataGridExtended.aspx" noresize scrolling="no"></frameset>
</html>
