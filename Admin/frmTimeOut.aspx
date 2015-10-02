<%@ Page language="c#" Inherits="SentinelFM.frmTimeOut" CodeFile="frmTimeOut.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>TimeOut</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="Pragma" content="no-cache" name="prevent_caching1">
		<meta http-equiv="Cache-Control" content="no-cache" name="prevent_caching2">
		<meta http-equiv="Expires" content="0" name="prevent_caching3">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="Scripts/md5.js"></script>
		<script language="javascript">
			<!--

			ns = (document.layers)? true:false
			ie = (document.all)? true:false

			
			function LoadFrames(menu,main)
			{
				if (menu !="")
				parent.menu.window.location=menu;
				if (main !="")
				parent.main.window.location=main;
			}
			
			
			function Encrypt()
			{
				with(document)
				{
					//TimeOut.elements["txtHash"].value = hex_md5(TimeOut.txtPassword.value+TimeOut.txtRnd.value);
						TimeOut.elements["txtHash"].value = hex_md5(hex_md5(TimeOut.txtPassword)+TimeOut.txtRnd.value);
					//TimeOut.txtHash.value = hex_md5(TimeOut.txtPassword.value+TimeOut.txtRnd.value);
					TimeOut.submit(); 
				}
			}
			
			function press(e) 
			{
				if(ie)
				{
					if (event.keyCode == 13) 
					{
			 			Encrypt();
					}
				}
		   }
			
			
			function LoadOnTop()
			{
				if (window != window.top)
				{
					window.top.location.href = window.location.href;
				}
			 }
			
			document.onkeydown=press;


			//function TopWindow()
			//{
			//	if (window != window.top)
			//	{
			//	    window.top.location.href = window.location.href;
			//	}
			//}
			
			
			//-->
		
		</script>
	</HEAD>
	<body onload="document.forms[0].txtUserName.focus();LoadOnTop();">
		<form id="TimeOut" name="TimeOut" method="post" runat="server">
			<table id="Table2" cellSpacing="0" cellPadding="0" align="center" border="0">
				<TR>
					<TD align="center" height="50"></TD>
				</TR>
				<TR>
					<TD align="center"></TD>
				</TR>
				<tr>
					<td align="center"><br>
						<br>
						<br>
						<br>
						<font face="Verdana, Arial, Helvetica, sans-serif" size="2" class="formtext">Sorry, 
							for security reasons your session time has expired and you have been 
							automatically logged out.<br>
							<br>
						</font>
					</td>
				</tr>
				<tr>
					<td align="center" class="formtext">Please relogin&nbsp;to begin a new 
						session.</td>
				</tr>
				<TR>
					<TD align="center" height="40"></TD>
				</TR>
				<TR>
					<TD align="center">
						<TABLE class="td_green" id="Table1" style="BORDER-RIGHT: 3px double; BORDER-TOP: 3px double; BORDER-LEFT: 3px double; WIDTH: 316px; BORDER-BOTTOM: 3px double"
							cellSpacing="0" cellPadding="0" width="316" border="0">
							<TR height="25">
								<TD style="FONT-WEIGHT: bold; TEXT-TRANSFORM: uppercase; WIDTH: 189px; COLOR: white"
									align="left" colSpan="4">&nbsp;&nbsp;&nbsp;Customer Login</TD>
							</TR>
							<TR>
								<TD class="regulartext" style="WIDTH: 115px" height="10"></TD>
								<TD class="regulartext" style="WIDTH: 180px" height="10"></TD>
								<TD style="WIDTH: 211px" align="center" height="10"></TD>
								<TD style="WIDTH: 189px" align="left" height="10"></TD>
							</TR>
							<TR>
								<TD class="regulartext" style="WIDTH: 115px" height="10"></TD>
								<TD class="regulartext" style="WIDTH: 180px" height="10"></TD>
								<TD style="WIDTH: 211px" align="center" height="10"></TD>
								<TD style="WIDTH: 189px" align="left" height="10"></TD>
							</TR>
							<TR height="25">
								<TD class="tableheading" style="WIDTH: 115px"></TD>
								<TD class="tableheading" style="WIDTH: 180px; COLOR: #006600">User Name:
									<asp:requiredfieldvalidator id="valUserName" runat="server" ErrorMessage="Please enter a user name!" ControlToValidate="txtUserName">*</asp:requiredfieldvalidator></TD>
								<TD style="WIDTH: 211px" align="left">
									<asp:textbox id="txtUserName" tabIndex="2" runat="server" Width="100%" CssClass="formtext"></asp:textbox></TD>
								<TD style="WIDTH: 189px" align="left"></TD>
							</TR>
							<TR height="25">
								<TD class="tableheading" style="WIDTH: 115px; HEIGHT: 24px"></TD>
								<TD class="tableheading" style="WIDTH: 180px; COLOR: #006600; HEIGHT: 24px">Password:¹
									<asp:requiredfieldvalidator id="valPassword" runat="server" ErrorMessage="Please enter a password!" ControlToValidate="txtPassword">*</asp:requiredfieldvalidator></TD>
								<TD style="WIDTH: 211px; HEIGHT: 24px" align="left">
									<asp:textbox id="txtPassword" tabIndex="3" runat="server" Width="100%" CssClass="formtext" TextMode="Password"></asp:textbox></TD>
								<TD style="WIDTH: 189px; HEIGHT: 24px" align="left"></TD>
							</TR>
							<TR>
								<TD class="RegularText" style="WIDTH: 115px"></TD>
								<TD class="RegularText" style="WIDTH: 180px"></TD>
								<TD style="WIDTH: 211px" align="left">
									<asp:validationsummary id="valSummary" runat="server" Width="202px" CssClass="errortext" Height="41px"></asp:validationsummary>
									<asp:label id="lblMessage" runat="server" CssClass="errortext" Visible="False"></asp:label></TD>
								<TD style="WIDTH: 189px" align="left"></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 115px" height="30"></TD>
								<TD style="WIDTH: 180px" height="30"></TD>
								<TD style="WIDTH: 211px" align="right" height="30"><IMG id="cmdLogin" style="CURSOR: hand" onclick="javascript:Encrypt()" alt="Login" src="images/Login1.gif"></TD>
								<TD align="center" height="30"></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 115px; HEIGHT: 6px"></TD>
								<TD style="WIDTH: 180px; HEIGHT: 6px"></TD>
								<TD style="WIDTH: 209px; HEIGHT: 6px" align="left"></TD>
								<TD style="WIDTH: 189px; HEIGHT: 6px" align="left"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD align="center" height="20"></TD>
				</TR>
				<TR>
					<TD align="center">
						<asp:Label id="Label2" runat="server" CssClass="formtext" Height="9px" ForeColor="Black">¹Your account information is protected by 128-bit 
			encryption to maintain your privacy and confidentiality.</asp:Label></TD>
				</TR>
			</table>
			<input id="txtHash" type="hidden" name="txtHash"> <input id=txtRnd 
type=hidden value='<%=ViewState["auth_seed"]%>' name=txtRnd>&nbsp;&nbsp;
		</form>
	</body>
</HTML>
