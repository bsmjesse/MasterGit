<%@ Page Language="c#" Inherits="SentinelFM.CaptchaLogin" CodeFile="Login.aspx.cs" Culture="en-US" 
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
    
   
    <link href="Scripts/css/login.css" type="text/css" rel="stylesheet" />
    <link href="Styles/styles.css" rel="stylesheet" />
    <script src="Styles/css-pop.js"></script>
    <script language="javascript" type="text/javascript" src="Scripts/prototype.js"></script>
    <script language="javascript" type="text/javascript" src="Scripts/md5.js"></script>
    <script language="javascript" type="text/javascript" src="Scripts/d5c.js"></script>
    <script language="javascript" type="text/javascript" src="Scripts/auth.js?v=20130703"></script>
   
   
    <script type="text/javascript" language="javascript">
        //<!--
        function disagree_ClientClick()
        {
			
            document.forms[0].elements["txtEmail"].value ='';
           
            document.forms[0].elements["lblRequireEmail"].value ='';
            
        }


        function popup(windowname) {
            blanket_size(windowname);
            window_pos(windowname);
            toggle('blanket');
            toggle(windowname);		
        }

        
      
			ns = (document.layers)? true:false
			ie = (document.all)? true:false
			var AuthenticationFailed = <%=AuthenticationFailed %>;
			
			
			
			
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
					
					///document.forms[0].elements["txtWidth"].value=screen.width;
					// document.forms[0].elements["txtHeight"].value=screen.height;
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
			    
				if(e)
				{			   
					if (e.keyCode == 13) 
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
			
		

        var lcookie = null;
        function setupLogin() {

            try {
                lcookie = readCookie("sfm_login");
	            }
	        catch(e) {}

	        if ((lcookie !== null) && $("rblSFM_" + lcookie) != null)
                $("rblSFM_" + lcookie).checked = true;
                
             var wdim = document.viewport.getDimensions();
             
             var x = $("sf_login_frame");
            var xdim = Element.getDimensions(x);
            var xl = (wdim.width - xdim.width - 30) / 2;
            var xt = (wdim.height - xdim.height) / 4;
            Element.setStyle(x, { left: xl + "px", top: xt + "px" });


             var xf = $("sf_login_footer");
            var xfdim = Element.getDimensions(xf);
            var xfl = (wdim.width - xfdim.width) / 2;
            var xft = wdim.height - xfdim.height - 20;
            Element.setStyle(xf, { left: xfl + "px", top: xft + "px" });
        };
		
		
		function prepareLogin() {
        try
        {
        window.external.AutoCompleteSaveForm(document.forms[0]);
        }
        catch(err){}
        $('cmdLogin').disable();

        var val = ($("rblSFM_1").checked) ? 1 : 0;
        if ((lcookie === null) || ($("rblSFM_" + lcookie)==null))
            lcookie = createCookie("sfm_login", val, false);
        else {
            if ($("rblSFM_" + lcookie).checked === false) {
                 eraseCookie("sfm_login");
                createCookie("sfm_login", val, false);
                lcookie = readCookie("sfm_login");
            }
        }
    
         Encrypt();
       };


			
Event.observe(window, "resize", setupLogin);
Event.observe(document, "keydown", function(event) { press(event) });

    function prefocuslogin() {
          if (AuthenticationFailed == 0) {
              $('txtUserName').focus();
              $('txtEmail').focus(); 
          }
          else {            
              $('txtPassword').focus();
              setTimeout(function(){
                document.getElementById("txtPassword").value = '';
               },100)
              
          }
      }

    function prefocusPopup() {
        
            $('txtEmail').focus();            
    }
			
			//-->
		
    </script>
    
    <style type="text/css">
        body
        {
            margin: 0px;
            font-family: Tahoma;
            font-size: 9pt;
        }
        #sf_login_frame
        {
            position: absolute;
            top: -1000px;
            left: -1000px;
        }
        #sf_login_control_frame
        {
            height: 220px;
            position: relative;
            top: -30px;
            left: 74px;
        }
        .sf_login_control
        {
            width: 166px;
        }
        #sf_login_footer
        {
            text-align: center;
            position: absolute;
            top: -1000px;
            left: -1000px;
        }
        a
        {
            color: #009800;
        }
    </style>

</head>
<body onload="setupLogin(); prefocuslogin(); LoadOnTop();" >
    <form id="Login" name="Login" method="post" runat="server" autocomplete="on" >
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <center>
          <div id="sf_login_frame">
 
                     <table id="Table1"  style="height: 165px; width: 380px;">
                <tr>
                    <td align="center">
                    
                  
                    <table  style="width: 300px;">
                        <tr>
                            <td colspan="2">
                                <asp:Image ID="imgProdLogo" runat="server" 
                                   ></asp:Image>
                            </td>
                        </tr>
                        <tr>
                            <td class="tableheading" align="right">
                                <asp:Label ID="lblUserName" runat="server" CssClass="tableHeading" meta:resourcekey="lblUserNameResource1"
                                    Text="User Name:"></asp:Label>
                                <asp:RequiredFieldValidator ID="valUserName" runat="server" ControlToValidate="txtUserName"
                                    ErrorMessage="Please enter a user name!" meta:resourcekey="valUserNameResource1"
                                    Text="*"></asp:RequiredFieldValidator></td>
                            <td align="left">
                                <asp:TextBox ID="txtUserName" TabIndex="2" runat="server" CssClass="formtext" meta:resourcekey="txtUserNameResource1"
                                    Width="166px" onblur="iBlur(this);" onfocus="iFocus(this);"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tableheading" align="right">
                                <asp:Label ID="lblPassword" runat="server" CssClass="tableHeading" meta:resourcekey="lblPasswordResource1"
                                    Text="Password:"></asp:Label>¹<asp:RequiredFieldValidator ID="valPassword" runat="server"
                                        ControlToValidate="txtPassword" ErrorMessage="Please enter a password!" meta:resourcekey="valPasswordResource1"
                                        Text="*" Enabled="False"></asp:RequiredFieldValidator></td>
                            <td align="left">
                                <asp:TextBox ID="txtPassword" TabIndex="3" runat="server" CssClass="formtext" TextMode="Password"  onkeyup="iKey(event, this);"
                                    meta:resourcekey="txtPasswordResource1" Width="166px"></asp:TextBox>
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
                           
                            <td colspan=2 >
                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
                                <asp:ValidationSummary ID="valSummary" runat="server" Width="100%" CssClass="errortext"
                                    Height="100%" meta:resourcekey="valSummaryResource1"></asp:ValidationSummary>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" >
                        <asp:Panel ID="CaptchaPanel" runat="Server" Visible="false">
                            <asp:Image ID="imgCaptcha" runat="server" Height="80px" Width="211px" />
                            <br />
                            <asp:TextBox ID="txtCaptcha" runat="server" Width="150px"></asp:TextBox>
                            <asp:Button ID="btnRegen" runat="server" Text="Next" OnClick="btnRegen_Click" CssClass="commands" />

                        </asp:Panel>
                            </td>
                        </tr>
                        
                    </table>
                    <div id="CaptchaDivision" >
                        <table>
                            <tr>
                                <td >
                                    <div id="optionContainer" class="CollapseContainer" >
                                    <asp:RadioButtonList ID="rblSFM" runat="server"  RepeatDirection="Horizontal">
                                        <asp:ListItem   Text="Standard" Selected="True" Value="0"></asp:ListItem>
                                        <asp:ListItem  Text="Lite [BETA]" Value="1"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                </td>
                            </tr>
                            <tr>
                                <td  align=center  >
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate> 
                                    <asp:Button ID="cmdLogin" runat="server" CommandName="25" CssClass="commands" Font-Size="10px"
                                        OnClientClick="javascript:prepareLogin(); javascript:prefocusPopup(); " Text="Login" meta:resourcekey="cmdLoginResource2"
                                         CausesValidation="false" UseSubmitBehavior="false"/>
                                   </ContentTemplate>
                                        </asp:UpdatePanel>
                                    <%--<asp:Button ID="cmdLogin" runat="server" Text="Button" OnClick="cmdLogin_Click" meta:resourcekey="cmdLoginResource2" OnClientClick="javascript:prepareLogin()"/>--%>
                                   
                                    <asp:HiddenField ID="HdDestinationUrl" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td  align=center  >
                                    &nbsp;</td>
                            </tr>
                            <tr>
                                <td  align=center  >
                                    <asp:LinkButton ID="lnkEnglish" runat="server" CausesValidation="False" OnClick="lnkEnglish_Click"
                                        meta:resourcekey="lnkEnglishResource1">ENGLISH</asp:LinkButton>
                                    &nbsp; &nbsp;<asp:LinkButton ID="lnkFrench" runat="server" CausesValidation="False"
                                        OnClick="lnkFrench_Click" meta:resourcekey="lnkFrenchResource1" Text="FRANÇAIS"></asp:LinkButton></td>
                            </tr>
                            </table>
                            </div>
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
        
            <div id="sf_login_footer" class="formtext" >
            <p>
	        	<strong><asp:Label ID="lblCustomerSupport" runat="server"   meta:resourcekey="lblCustomerSupport" Text="Customer Support:"></asp:Label> </strong><asp:Label ID="lblCustomerSupportPhone"  meta:resourcekey="lblCustomerSupportPhone" runat="server"  Text=" Toll Free Direct Line:"></asp:Label>  <asp:Label ID="lblTollFreeNumber" runat="server" Text="1-866-578-4315" ></asp:Label>  <asp:Label ID="lblEmail" Text="Email:" runat="server"   meta:resourcekey="lblEmail"  ></asp:Label>   <asp:HyperLink NavigateUrl="mailto:customercare@bsmwireless.com" runat="server" ID="lblEmailurl" Text="customercare@bsmwireless.com" meta:resourcekey="lblEmailurl"  ></asp:HyperLink>  
                <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
    		</p>		
                <asp:Label ID="lblMessageError" runat="server" Text="Your account has been deactivated, please contact with your administrator." Visible="false" ForeColor="Red"></asp:Label>

                <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1" Text="¹To maintain client privacy and confidentiality, client account information is protected by 128-bit encryption. "></asp:Label><br />
                <asp:Label ID="Label1" runat="server" meta:resourcekey="Label1Resource1" Text="For best viewing of this website, please use Internet Explorer ver 5.0 or higher and monitor resolution of 1024x768."></asp:Label><br />
                <asp:Label ID="lblPopup" runat="server" Style="font-weight: bold;" meta:resourcekey="lblPopupResource1"
                    Text="ALL POP UP BLOCKING software must be disabled before starting the application."></asp:Label>
            </div>

           

            <asp:HiddenField ID="SNuserid" runat="server" />
             <asp:HiddenField ID="SNsecId" runat="server" />
            <asp:HiddenField ID="HDCulture" runat="server" />

        </center>

        <div id="blanket" style="display:none;"></div>
	<div id="popUpDiv" style="display:none;" class="smart-green" runat="server" defaultfocus="txtEmail">
        <div  class="head">
        <div style="text-align:right; padding-right:10px; ">
            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Styles/SentinelFM/images/close.png" OnClientClick="javascript:popup(popUpDiv)"/>

        </div>
             <div>
              <asp:Label ID="Label5" runat="server" Text="Disclaimer" meta:resourcekey="lblDisclaimer" Font-Size="24px"/>   
        </div>
    </div>
        	 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate> 
 
        <div style="overflow-y: auto; height:70px;" class="bod" >
            <asp:Label ID="Label4" runat="server" Text="You are required this by state Law of California. we are a produce store." meta:resourcekey="lblDisclaimerTxt"></asp:Label>
       
        </div>
      
        <div style="padding-top:12px">
            
            <div style="width:20%;float: left;padding-top:12px"><asp:Label ID="Label6" runat="server" Text="Email" meta:resourcekey="lblEmailtxt" /> </div>
            <div style="width:80%;float: left; ">
                <asp:TextBox ID="txtEmail" runat="server"  width="95%" ValidationGroup="emailGroup" placeHolder="jone@bswireless.com"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" style="display:none"  ErrorMessage="Invalid Email Id"  ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                    ControlToValidate="txtEmail" ValidationGroup="emailGroup" SetFocusOnError="true" meta:resourcekey="InvalidEmailResource1"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" style="display:none" meta:resourcekey="valEmailResource1" ErrorMessage='Email cannot be left blank'   ValidationGroup="emailGroup" ControlToValidate="txtEmail" SetFocusOnError="true" ></asp:RequiredFieldValidator>
            </div>           ,
        </div>
    	<div style="text-align:right;">
           
            <asp:Button ID="btnDisagree" runat="server" Text="Disagree" OnClick="btnDisagree_Click" CssClass="button" meta:resourcekey="btnDisagreeTxt" CausesValidation="false"/>
            <asp:Button ID="btnAgree" runat="server" Text="Agree" OnClick="btnSaveEmail_Click"  CssClass="button" meta:resourcekey="btnAgreeTxt" ValidationGroup="emailGroup"/>
   
                
    	</div>
                <span>
            <asp:Label ID="lblRequireEmail" runat="server" Text="" ForeColor="Red" Visible="true"></asp:Label>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" style="display:none" ShowMessageBox="false" DisplayMode="BulletList" ShowSummary="true"  ValidationGroup="emailGroup"/>

            </span>
         
       </ContentTemplate>
                </asp:UpdatePanel>
        
 	</div>	
  
       <%-- <asp:Button ID="click" runat="server" Text="Button" OnClick="click_Click" CausesValidation="false"/>--%>

    </form>
<!-- Google Tag Manager -->
<noscript><iframe src="//www.googletagmanager.com/ns.html?id=GTM-K49R9G"
height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>
<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
})(window,document,'script','dataLayer','GTM-K49R9G');</script>
<!-- End Google Tag Manager -->


     
</body>
</html>
