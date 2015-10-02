<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMain.aspx.cs" Inherits="frmMain" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
	<head runat="server">
		<TITLE>Fleet Management & Security</TITLE>
		<meta name="GENERATOR" content="Microsoft Visual Studio.NET 7.0">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</head>
	
   <frameset rows="100%,*"  frameSpacing="0" frameborder=0 >
      <frameset cols="100%,*" frameSpacing="0" frameborder=0> 
	        <frameset id="TopFrame" rows="10px,*"  frameSpacing="0" frameborder=0  >
		        <!--<frame name="menu" src="frmTopMenu.aspx" width=1024px  scrolling="no" frameborder=0   noresize >-->
		        <frame name="menu" src="frmTopMenu.aspx" width="100%"  scrolling="no" frameborder=0   noresize >
		        <frame name="main" src="Home/frmMainHome.aspx" width="100%"  frameborder="1"  noresize style="border-bottom: gray 1px solid;border-right: gray 1px solid;">
	        </frameset>
	        <frame name="empty" src="EmptySpace.htm"  scrolling="no"  frameborder="0"   /> 
	   </frameset>
	   <frame name="empty" src="EmptySpace.htm"  scrolling="no" frameborder="0"   /> 
	</frameset> 
	
</html>
