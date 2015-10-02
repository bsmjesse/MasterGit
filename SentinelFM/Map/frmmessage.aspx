<%@ Reference Page="~/frmMessage.aspx" %>

<%@ Page Language="c#" Inherits="SentinelFM.Map.frmMessage" CodeFile="frmMessage.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmMessage</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    
    
    
       <script language="javascript">
		<!--

				
				function NewMessageWindow() { 
					var mypage='../Messages/frmNewMessageMain.aspx'
					var myname='';
					var w=560;
					var h=560;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
				

		//-->
    </script>



</head>
<body bgcolor="white">
    <form method="post" runat="server">
     <table style="z-index: 101; left: 0px; position: absolute; width:200px; top: 7px" ><tr><td align=center  >
        <asp:Button ID="cmdNewMessage" runat="server" Font-Size="10px" CssClass="commands" Text="New Text Message"
            Width="208px" CommandName="25" OnClientClick="javascript:NewMessageWindow()"  meta:resourcekey="cmdNewMessageResource1"></asp:Button>
            </td></tr></table> 
    </form>
</body>
</html>
