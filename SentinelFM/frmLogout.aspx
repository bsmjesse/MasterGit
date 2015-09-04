<%@ Page Language="c#" Inherits="SentinelFM.CaptchaLogout" ErrorPage="frmLoginError.aspx"
    CodeFile="frmLogout.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <meta http-equiv="Pragma" content="no-cache" name="prevent_caching1" />
    <meta http-equiv="Cache-Control" content="no-cache" name="prevent_caching2" />
    <meta http-equiv="Expires" content="0" name="prevent_caching3" />
    <link href="GlobalStyle.css" type="text/css" rel="stylesheet" />
    <script language="javascript" src="Scripts/md5.js"></script>

    <script language="javascript">
			//<!--


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
			
					document.forms[0].elements["txtHash"].value = hex_md5(hex_md5(document.forms[0].txtPassword.value)+document.forms[0].txtRnd.value);
					//Login.txtHash.value = hex_md5(Login.txtPassword.value+Login.txtRnd.value);
					
					//document.forms[0].elements["txtWidth"].value=screen.width;
					//document.forms[0].elements["txtHeight"].value=screen.height;
					if (ie)
					{
					   document.forms[0].elements["txtWidth"].value=document.body.clientWidth;
					   document.forms[0].elements["txtHeight"].value=document.body.clientHeight;
					}
					else
					{
					   document.forms[0].elements["txtWidth"].value=window.innerWidth;
					   document.forms[0].elements["txtHeight"].value=window.innerHeight;
					}
					
					document.forms[0].submit(); 
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
			

        function VaildateUserNameInput()
        {
             var iChars = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";

              for (var i = 0; i < document.forms[0].txtUserName.value.length; i++) {
  	            if (iChars.indexOf(document.forms[0].txtUserName.value.charAt(i)) != -1) {
  	            alert ("Your username has special characters. \nThese are not allowed.\n Please remove them and try again.");
  	            return false;
  	            }
            }
         }


      function VaildatePswInput()
        {
             var iChars = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";

              for (var i = 0; i < document.forms[0].txtPassword.value.length; i++) {
  	            if (iChars.indexOf(document.forms[0].txtPassword.value.charAt(i)) != -1) {
  	            alert ("Your password has special characters. \nThese are not allowed.\n Please remove them and try again.");
  	            return false;
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

</head>
<body onload="document.forms[0].txtUserName.focus();LoadOnTop();" onunload="VaildateUserNameInput();">
    <form id="Login" name="Login" method="post" runat="server" autocomplete="OFF">
        <center>
            <div id="PageHeader" style="height: 100px;">
                <asp:Label ID="lblThankYou" runat="server" meta:resourcekey="lblThankYouResource1"
                    Text="Thank you for using our system." Font-Bold="True"></asp:Label><br /><asp:Label ID="lblFinished" runat="server"
                        meta:resourcekey="lblFinishedResource1" Text="If you are finished , please close your browser to protect your privacy."></asp:Label><br />
                &nbsp;<asp:Label ID="lblRelogin" runat="server" meta:resourcekey="lblReloginResource1"
                    Text="Please relogin to begin a new session."></asp:Label></div>
            <div class="formtext" id="PageContent">
               
                
                
                
                   
                     <table id="Table1" class="tableLoginBorder"  style="height: 165px; width: 380px;">
                <tr>
                    <td style="width: 370px">
                    
                    <div class="divLogin" >
                        <asp:Label ID="lblCustLogin" runat="server" meta:resourcekey="lblCustLoginResource1"
                            Text="Customer Login" Font-Size="Small"></asp:Label></div>
                    <table>
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:Image ID="imgProdLogo" runat="server"  ImageUrl="images/ProdLogo.gif"
                                    meta:resourcekey="imgProdLogoResource1"></asp:Image>
                                             </td>
                        </tr>
                        <tr>
                            <td class="tableheading">
                                <asp:Label ID="lblUserName" runat="server" CssClass="tableheading" meta:resourcekey="lblUserNameResource1"
                                    Text="User Name:"></asp:Label>
                                <asp:RequiredFieldValidator ID="valUserName" runat="server" ControlToValidate="txtUserName"
                                    ErrorMessage="Please enter a user name!" meta:resourcekey="valUserNameResource1"
                                    Text="*"></asp:RequiredFieldValidator></td>
                            <td>
                                <asp:TextBox ID="txtUserName" TabIndex="2" runat="server" CssClass="formtext" meta:resourcekey="txtUserNameResource1"
                                    Width="180px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tableheading">
                                <asp:Label ID="lblPassword" runat="server" CssClass="tableheading" meta:resourcekey="lblPasswordResource1"
                                    Text="Password:"></asp:Label>¹<asp:RequiredFieldValidator ID="valPassword" runat="server"
                                        ControlToValidate="txtPassword" ErrorMessage="Please enter a password!" meta:resourcekey="valPasswordResource1"
                                        Text="*" Enabled="False"></asp:RequiredFieldValidator></td>
                            <td>
                                <asp:TextBox ID="txtPassword" TabIndex="3" runat="server" CssClass="formtext" TextMode="Password"
                                    meta:resourcekey="txtPasswordResource1" Width="180px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tableheading" align="left">
                                <asp:Label ID="lblDatabase" runat="server" CssClass="tableHeading" 
                                    Text="Database" Visible="False"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="cboDataBaseName" Width="180px" runat="server" 
                                    Visible="False">
                                    <asp:ListItem Selected=True Text="SentinelFM" Value="SentinelFM" ></asp:ListItem>   
                                </asp:DropDownList >
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:ValidationSummary ID="valSummary" runat="server" Width="100%" CssClass="errortext"
                                    Height="41px" meta:resourcekey="valSummaryResource1"></asp:ValidationSummary>
                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                        <asp:Panel ID="CaptchaPanel" runat="Server" Visible="false">
                            <asp:Image ID="imgCaptcha" runat="server" Height="80px" Width="211px" /><br />
                            <asp:TextBox ID="txtCaptcha" runat="server" Width="150px"></asp:TextBox>
                            <asp:Button ID="btnRegen" runat="server" Text="Next" OnClick="btnRegen_Click" CssClass="commands" /></asp:Panel>
                            </td>
                        </tr>
                        <tr>
                        <td></td>
                            <td >
                            
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="cmdLogin" runat="server" CommandName="25" CssClass="commands" Font-Size="10px"
                                        OnClientClick="javascript:Encrypt()" Text="Login" meta:resourcekey="cmdLoginResource2"
                                        Width="180px" UseSubmitBehavior="False" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 209px; height: 6px" align="center" class="td_green">
                                    <asp:LinkButton ID="lnkEnglish" runat="server" CausesValidation="False" OnClick="lnkEnglish_Click"
                                        meta:resourcekey="lnkEnglishResource1">ENGLISH</asp:LinkButton>
                                    &nbsp; &nbsp;<asp:LinkButton ID="lnkFrench" runat="server" CausesValidation="False"
                                        OnClick="lnkFrench_Click" meta:resourcekey="lnkFrenchResource1" Text="FRANÇAIS"></asp:LinkButton></td>
                            </tr>
                        </table>
                            </td>
                        </tr>
                    </table>
                    <div id="CaptchaDivision"  >
                        &nbsp;</div>
                    <div>
                        &nbsp;</div>
               
                    
                    </td>
                </tr>
            </table> 
            </div>
            
          <input id="txtHash" type="hidden" name="txtHash">
        <input id="txtRnd" type="hidden" value='<%=ViewState["auth_seed"]%>' name="txtRnd">&nbsp;&nbsp;
        <input id="txtWidth" type="hidden" name="txtWidth">
        <input id="txtHeight" type="hidden" name="txtHeight">
        
            <div id="PageFooter" class="formtext" style="padding-top: 70px; width:70%">
                <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1" Text="¹To maintain client privacy and confidentiality, client account information is protected by 128-bit encryption. "></asp:Label><br />
                <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1" Text="For best viewing of this website, please use Internet Explorer ver 7.0 or higher and monitor resolution of 1024x768."></asp:Label><br />
                <asp:Label ID="lblPopup" runat="server" Style="font-weight: bold;" meta:resourcekey="lblPopupResource1"
                    Text="ALL POP UP BLOCKING software must be disabled before starting the Sentinel-FM application."></asp:Label>
            </div>
        </center>
    </form>
</body>
</html>
