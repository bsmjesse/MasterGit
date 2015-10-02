<%@ Page language="c#" Inherits="SentinelFM.ServicesUpdates.frmMainHome" CodeFile="frmMainHome.aspx.cs" %>

<HTML>
  <HEAD runat=server ><TITLE>Home</TITLE>
<meta content="Microsoft Visual Studio 7.0" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
  </HEAD>
  
  <frameset border=0   frameSpacing=0 borderColor=gray frameBorder=0 cols="75%,25%">
	<frame name=Home src="<%=mainURL%>" frameBorder=0 noResize scrolling=no>
	<frame  style="BORDER-LEFT: black 1px solid; "  name=ServicesUpdates src="frmServicesUpdates.aspx" 
noResize scrolling=no></FRAMESET>
</HTML>
