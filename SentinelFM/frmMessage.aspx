<%@ Page language="c#" Inherits="SentinelFM.frmMessage" CodeFile="frmMessage.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>SentinelFM Info</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript">
	<!--
			function onblur(event) 
			{
		window.focus();
		}


	//-->
		</script>
</HEAD>
	<body onBlur="window.focus()" class="configTabBackground">
		<form method="post" runat="server">
			<INPUT class="combutton" id="cmdOk"  style="Z-INDEX: 102; LEFT: 250px; POSITION: absolute; TOP: 75px" onclick="window.close()" tabIndex="4"  type="button" value="Ok" name="cmdOk">
			<IMG style="Z-INDEX: 101; LEFT: 6px; POSITION: absolute; TOP: 4px" src="images/caution.gif">
			<asp:TextBox id="txtMessage" style="Z-INDEX: 104; LEFT: 43px; POSITION: absolute; TOP: 6px" runat="server" CssClass="errortext" Width="313px" Height="62px" TextMode="MultiLine" ReadOnly="True" BorderStyle="Solid" BackColor="White" BorderColor="#E0E0E0"></asp:TextBox>
            &nbsp;
		</form>
	</body>
</HTML>
