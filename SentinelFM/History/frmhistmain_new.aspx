<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmhistmain_new.aspx.cs" Inherits="SentinelFM.History_frmhistmain_new" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head id="Head1" runat="server">
		<TITLE>frmHistMain</TITLE>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<frameset rows="340px,*" frameSpacing="0" border="0" bordercolor="#006699" frameBorder="0">
		<frameset  cols="*,265px"  frameSpacing="0" border="0" bordercolor="#006699"  frameBorder="0">
	   	<frame name="frmHisMap"  src="../MapNew/frmHistMap.aspx" noresize scrolling="no">
	   	<frame name="frmHisCrt" src="New/frmHistoryCrt.aspx" noresize scrolling="no">
      </frameset>
   	<frame name="frmHis" src="New/frmHistDataGridExtended.aspx<%= FromMapScreen %>" noresize scrolling="no">	
	</frameset>
</HTML>