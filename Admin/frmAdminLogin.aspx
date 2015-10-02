<%@ Page language="c#" Inherits="SentinelFM.Admin.frmAdminLogin" CodeFile="frmAdminLogin.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Admin Login</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="Scripts/md5.js"></script>
		<script language="javascript">
		<!--
		
		ns = (document.layers)? true:false
		ie = (document.all)? true:false

			
			function Encrypt()
			{
				with(document)
				{
					//Login.elements["txtHash"].value = hex_md5(Login.txtPassword.value+Login.txtRnd.value);
					
					document.forms[0].elements["txtHash"].value = hex_md5(hex_md5(document.forms[0].txtPassword.value)+document.forms[0].txtRnd.value);
					
					//Login.txtHash.value = hex_md5(Login.txtPassword.value+Login.txtRnd.value);
					Login.submit(); 
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
		   
		   document.onkeydown=press;
		   
		//-->
		</script>
	</HEAD>
	<body onload="document.forms[0].txtUserName.focus();">
		<DIV style="Z-INDEX: 101; LEFT: 8px; WIDTH: 10px; POSITION: absolute; TOP: 8px; HEIGHT: 10px">
			<FORM id="Login" name="Login" method="post" runat="server">
				<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 307px; WIDTH: 280px; POSITION: absolute; TOP: 230px; HEIGHT: 151px"
					cellSpacing="0" cellPadding="0" width="280" border="0">
					<TR height="25">
						<TD class="regulartext" style="WIDTH: 115px"></TD>
						<TD style="WIDTH: 189px" align="left"></TD>
					</TR>
					<TR height="25">
						<TD class="tableheading" style="WIDTH: 115px">User Name:
							<asp:requiredfieldvalidator id="valUserName" runat="server" ControlToValidate="txtUserName" ErrorMessage="Please enter a user name!">*</asp:requiredfieldvalidator></TD>
						<TD style="WIDTH: 189px" align="left"><asp:textbox id="txtUserName" tabIndex="2" runat="server" CssClass="formtext" Width="140px"></asp:textbox></TD>
					</TR>
					<TR height="25">
						<TD class="tableheading" style="WIDTH: 115px; HEIGHT: 26px">Password:
							<asp:requiredfieldvalidator id="valPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Please enter a password!">*</asp:requiredfieldvalidator></TD>
						<TD style="WIDTH: 189px; HEIGHT: 26px" align="left"><asp:textbox id="txtPassword" tabIndex="3" runat="server" CssClass="formtext" Width="140px" TextMode="Password"></asp:textbox></TD>
					</TR>
					<TR>
						<TD class="RegularText" style="WIDTH: 115px"></TD>
						<TD style="WIDTH: 189px" align="left"><asp:validationsummary id="valSummary" runat="server" CssClass="errortext" Width="202px" Height="41px"></asp:validationsummary><asp:label id="lblMessage" runat="server" CssClass="errortext" Visible="False"></asp:label></TD>
					</TR>
					<TR>
						<TD style="WIDTH: 115px"></TD>
						<TD style="WIDTH: 189px" align="left">&nbsp;&nbsp;&nbsp; &nbsp; <INPUT class="combutton" id="cmdLogin" onclick="javascript:Encrypt()" tabIndex="4" type="button"
								value="Login" name="cmdLogin"></TD>
					</TR>
					<TR>
						<TD style="WIDTH: 115px; HEIGHT: 6px"></TD>
						<TD style="WIDTH: 189px; HEIGHT: 6px" align="left"></TD>
					</TR>
				</TABLE>
				<INPUT id="txtHash" type="hidden" name="txtHash"> <INPUT id=txtRnd type=hidden 
value='<%=Session["auth_seed"]%>' name=txtRnd>&nbsp;
			</FORM>
		</DIV>
		<asp:label id="Label3" style="Z-INDEX: 102; LEFT: 311px; POSITION: absolute; TOP: 198px" runat="server"
			Width="347px" Font-Size="14pt" Font-Bold="True">SentinelFM administration System</asp:label>
		<IMG style="Z-INDEX: 105; LEFT: 384px; POSITION: absolute; TOP: 40px" src="images/sentinel_large.gif">
	</body>
</HTML>
