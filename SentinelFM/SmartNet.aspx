<%@ Page %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<title>SmartNet</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language=javascript>
		<!--
				function Submit()
				{
					frmSmartNetLogin.submit(); 	
				}
		//-->
		</script>

	</HEAD>
	<body onload="Submit()">
	  <form id="frmSmartNetLogin" name="frmSmartNetLogin" method=post action="Login.aspx"  > 
		<input type=hidden id="Logo" name="Logo" value="SmartNet.gif">    
		<input type=hidden id="URL" name="URL" value="frmHomeSmartNet.aspx">    
		</form> 
	</body>
</HTML>
