<%@ Page language="c#" Inherits="SentinelFM.LoginPopUp" CodeFile="frmLoginPopUp.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<title>Login</title>
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
					LoginPopUp.elements["txtHash"].value = hex_md5(LoginPopUp.txtPassword.value+LoginPopUp.txtRnd.value);
					//LoginPopUp.txtHash.value = hex_md5(LoginPopUp.txtPassword.value+LoginPopUp.txtRnd.value);
					LoginPopUp.submit(); 
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
		<SCRIPT language="JavaScript">
			ns4=document.layers
			ie4=document.all
			ns6=document.getElementById&&!document.all
			var xWin=null; var ticks=6;

			function ChangeLabel0(txt){
			if(ns4){document.layers.label0.innerHTML=txt}
			if(ie4){document.all.label0.innerHTML=txt}
			if(ns6){document.getElementById("label0").innerHTML=txt}
			}

			function StartT(){
			StartPopup();
			window.focus();
			ChangeLabel0("");
			} 
			function StartPopup(){
			xWin=window.open("PopPupBlockerTest.htm","","width=1,height=1");
			setTimeout("test_xWin()",700);
			}
			function test_xWin(){
			//  alert("typeof(xWin)="+typeof(xWin));
			//  alert("typeof(xWin.location.href)="+typeof(xWin.location.href));
			//  alert("xWin="+xWin);
			//  alert("xWin.location.hash="+xWin.location.hash);
			if ( (xWin==null)
				||(typeof(xWin)=="undefined")
				||(typeof(xWin.location.hash)!="string")
			//     ||(xWin.location.hash!="#abc")
				){sMsg="<font color=#FF0000><b>A popup killer is detected</b></font>";ChangeLabel0(sMsg);}
			else{xWin.close();};
			//alert("xWin="+xWin+" type="+typeof(xWin)+" typeloc="+typeof(xWin.location.hash)+" hash="+xWin.location.hash);
			window.focus();
			
	}
		</SCRIPT>
	</HEAD>
	<body bgColor="#d4d0c8" onload="StartT();document.forms[0].txtUserName.focus();">
		<form name="LoginPopUp" method="post" runat="server" AUTOCOMPLETE=OFF>
			<TABLE class="td_green" id="Table1" style="Z-INDEX: 101; LEFT: 14px; WIDTH: 275px; POSITION: absolute; TOP: 44px; HEIGHT: 151px"
				cellSpacing="0" cellPadding="0" width="275" border="0">
				<TR>
					<TD class="formtext" style="WIDTH: 114px" height="10"></TD>
					<TD style="WIDTH: 189px" align="left" height="10"></TD>
				</TR>
				<TR height="25">
					<TD class="formtext" style="WIDTH: 114px">User Name:
						<asp:requiredfieldvalidator id="valUserName" runat="server" ControlToValidate="txtUserName" ErrorMessage="Please enter a user name!">*</asp:requiredfieldvalidator></TD>
					<TD style="WIDTH: 189px" align="left"><asp:textbox id="txtUserName" tabIndex="2" runat="server" CssClass="formtext" Width="180px"></asp:textbox></TD>
				</TR>
				<TR height="25">
					<TD class="formtext" style="WIDTH: 114px; HEIGHT: 24px">Password:
						<asp:requiredfieldvalidator id="valPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Please enter a password!">*</asp:requiredfieldvalidator></TD>
					<TD style="WIDTH: 189px; HEIGHT: 24px" align="left"><asp:textbox id="txtPassword" tabIndex="3" runat="server" CssClass="formtext" Width="180px" TextMode="Password"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="RegularText" style="WIDTH: 114px"></TD>
					<TD style="WIDTH: 189px" align="left"><asp:validationsummary id="valSummary" runat="server" CssClass="errortext" Width="202px" Height="27px"></asp:validationsummary><asp:label id="lblMessage" runat="server" CssClass="errortext" Visible="False"></asp:label></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 114px"></TD>
					<TD style="WIDTH: 189px" align="center">
						<TABLE id="Table2" style="WIDTH: 199px; HEIGHT: 24px" cellSpacing="0" cellPadding="0" width="199"
							border="0">
							<TR>
								<TD><INPUT id="cmdOk" style="FONT-SIZE: 12px; WIDTH: 81px; HEIGHT: 24px" onclick="javascript:Encrypt()"
										tabIndex="4" type="button" value="OK" name="cmdOk"></TD>
								<TD align="left"><INPUT id="cmdCancel" style="FONT-SIZE: 12px; WIDTH: 81px; HEIGHT: 24px" onclick="javascript:window.close()"
										tabIndex="5" type="button" value="Cancel" name="cmdOk"></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
			<TABLE id="Table4" style="Z-INDEX: 106; LEFT: 14px; WIDTH: 100%; POSITION: absolute; TOP: 195px"
				cellSpacing="0" cellPadding="0" width="275" border="0">
				<tr><td><div id="label0" align=center  ><b><font color="#ff0000"></font></b></div></td> </tr>
				<TR>
					<TD class="formtext">¹To maintain client privacy and confidentiality, client 
						account information is protected by 128-bit encryption.
					</TD>
				</TR>
				<tr>
					<td height="15"></td>
				</tr>
				<TR>
					<td class="formtext">For best viewing of this website, please use Internet Explorer 
						version 5.0 or higher and monitor resolution of 1024x768.
					</td>
				</TR>
				<tr>
					<td height="15"></td>
				</tr>
				<TR>
					<td class="formtext" style="FONT-WEIGHT: bold">ALL POP UP BLOCKING software must be 
						disabled before starting the Sentinel-FM application.
					</td>
				</TR>
			</TABLE>
			<INPUT id=txtRnd 
style="Z-INDEX: 103; LEFT: 357px; POSITION: absolute; TOP: 70px" type=hidden 
value='<%=ViewState["auth_seed"]%>' name=txtRnd>
			<TABLE id="Table3" style="Z-INDEX: 102; LEFT: 12px; WIDTH: 302px; POSITION: absolute; TOP: 2px; HEIGHT: 45px"
				cellSpacing="0" cellPadding="0" width="302" border="0">
				<TR>
					<TD><IMG src="images/key.jpg"></TD>
					<TD><asp:label id="Label1" runat="server" CssClass="formtext">Please type your user name and password.</asp:label>¹</TD>
				</TR>
			</TABLE>
			<INPUT id="txtHash" style="Z-INDEX: 104; LEFT: 356px; POSITION: absolute; TOP: 42px" type="hidden"
				name="txtHash">
			
		</form>
	</body>
</HTML>
