<%@ Page language="c#" Inherits="SentinelFM.frmMenu" CodeFile="frmMenu.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<title>frmMenu</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		
		<script language="javascript">
		<!--

		
		 

			function OpenHelpWindow()
			{
				var newWindow = window.open("Help.aspx","","HEIGHT = 520, WIDTH=550")
			}
			
	
		function LoadFrames(menu,main)
			{
				if (menu !="")
				parent.menu.window.location=menu;
				if (main !="")
				parent.main.window.location=main;
			}
			
		//-->
		</script>
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body bgColor="#dedfde" onload="javascript:document.frmMenu.cmdHome.style.color='black';">
		<form name="frmMenu" method="post" runat="server">
			<table id="Table1" style="Z-INDEX: 101; LEFT: 6px; WIDTH: 99px; POSITION: absolute; TOP: 6px; HEIGHT: 592px" cellSpacing="0" cellPadding="0" align="left" border="0">
				<TR>
					<TD class="formtext" align="middle">
						<asp:image id="imgProdLogo" runat="server" ImageUrl="images/ProdLogo.gif" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" Width="111px" Height="36px"></asp:image></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 103px; HEIGHT: 24px" align="left" height="24"></TD>
				</TR>
				<tr>
					<td class="formtext" style="WIDTH: 103px; HEIGHT: 10px" align="left" height="10">
						&nbsp;&nbsp;
					</td>
				</tr>
				<TR>
					<TD class="specialText" style="WIDTH: 103px" align="left" height="10"></TD>
				</TR>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdHome" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='#003366'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='#003366';cmdMessaging.style.color='#003366';cmdHome.style.color='black'; cmdLandmarks.style.color='#003366';cmdAutomaticTasks.style.color='#003366'; LoadFrames('','Home/frmMainHome.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" maxLength="0" value="Home"></td>
				</tr>
				<TR>
					<TD style="WIDTH: 103px" height="10"></TD>
				</TR>
				<tr>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdMap" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px" onclick="javascript:cmdMap.style.color='black'; cmdHistory.style.color='#003366'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='#003366';cmdMessaging.style.color='#003366';cmdHome.style.color='#003366'; cmdLandmarks.style.color='#003366';cmdAutomaticTasks.style.color='#003366'; LoadFrames('','Map/frmMain.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="Map" name="cmdMap"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px" height="10"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdHistory" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='black'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='#003366';cmdMessaging.style.color='#003366';cmdHome.style.color='#003366'; cmdLandmarks.style.color='#003366' ;cmdAutomaticTasks.style.color='#003366'; LoadFrames('','History/frmHistMain.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="History" name="cmdHistory"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px" height="10"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdMessaging" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px;DISPLAY:<%=MessagesView%>" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='#003366'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='#003366';cmdMessaging.style.color='black';cmdHome.style.color='#003366' ;cmdLandmarks.style.color='#003366' ;cmdAutomaticTasks.style.color='#003366'; LoadFrames('','Messages/frmMessages.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="Messages" name="cmdMessaging"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px" height="10"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdAutomaticTasks" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='#003366'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='#003366';cmdMessaging.style.color='#003366';cmdHome.style.color='#003366' ;cmdLandmarks.style.color='#003366' ;cmdAutomaticTasks.style.color='black'; LoadFrames('','AutomaticTask/frmAutomaticTasks.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="Automatic Tasks" name="cmdAutomaticTasks"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px" height="10"></td>
				</tr>
				<TR>
					<TD style="WIDTH: 103px"><INPUT class="menubutton" id="cmdLandmarks" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='#003366'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='#003366';cmdMessaging.style.color='#003366';cmdHome.style.color='#003366';cmdLandmarks.style.color='black';cmdAutomaticTasks.style.color='#003366';LoadFrames('','GeoZone_Landmarks/frmLandmark.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="Landmarks" name="cmdLandmarks"></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 103px" height="10"></TD>
				</TR>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdReports" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='#003366'; cmdReports.style.color='black'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='#003366';cmdMessaging.style.color='#003366';cmdHome.style.color='#003366'; cmdLandmarks.style.color='#003366' ; cmdAutomaticTasks.style.color='#003366'; LoadFrames('','Reports/frmReports.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="Reports" name="cmdReports"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px" height="10"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdConfiguration" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px;DISPLAY:<%=ConfigurationView%>" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='#003366'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='black';cmdHelp.style.color='#003366';cmdMessaging.style.color='#003366';cmdHome.style.color='#003366'; cmdLandmarks.style.color='#003366';cmdAutomaticTasks.style.color='#003366'; LoadFrames('','Configuration/frmEmails.aspx')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="Configuration" name="cmdConfiguration"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px" height="10"></td>
				</tr>
				<tr>
					<td style="WIDTH: 103px"><INPUT class="menubutton" id="cmdHelp" onmouseover="javascript:this.style.backgroundColor='mintcream';" style="WIDTH: 114px; HEIGHT: 30px" onclick="javascript:cmdMap.style.color='#003366'; cmdHistory.style.color='#003366'; cmdReports.style.color='#003366'; cmdConfiguration.style.color='#003366';cmdHelp.style.color='black';cmdMessaging.style.color='#003366';cmdHome.style.color='#003366'; cmdLandmarks.style.color='#003366';cmdAutomaticTasks.style.color='#003366'; LoadFrames('','Help/Help.htm')" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" type="button" value="Help" name="cmdHelp"></td>
				</tr>
				<TR>
					<TD style="WIDTH: 114px" height="10"></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 114px"><asp:button id="cmdLogout" onmouseover="javascript:this.style.backgroundColor='mintcream';" onmouseout="javascript:this.style.backgroundColor='whitesmoke';" runat="server" Width="114" Height="30" CssClass="menubutton" Text="Logout" onclick="cmdLogout_Click"></asp:button></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 103px; COLOR: gainsboro"><label>ver: 2.2.6</label>
					</TD>
				</TR>
				<TR>
					<TD style="WIDTH: 103px" align="middle" height="30"><FONT size="1" class="specilatext">Copyright 
							© 2003-2004 BSM ®</FONT></TD>
				</TR>
			</table>
		</form>
	</body>
</HTML>
