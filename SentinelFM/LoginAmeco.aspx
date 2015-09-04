<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoginAmeco.aspx.cs" Inherits="SentinelFM.CaptchaLogin" Culture="en-US" UICulture="auto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
 
<head runat="server">
    <title>Login</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <meta http-equiv="Pragma" content="no-cache" name="prevent_caching1" />
    <meta http-equiv="Cache-Control" content="no-cache" name="prevent_caching2" />
    <meta http-equiv="Expires" content="0" name="prevent_caching3" />
   <%--<link href="GlobalStyle.css" type="text/css" rel="stylesheet" />--%>
    <%--<link href="Scripts/css/login.css" type="text/css" rel="stylesheet" />--%>
    <link href="Scripts/css/style.css" rel="stylesheet" />

    
    <link href="GlobalStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/prototype.js"></script>
    <script  type="text/javascript" src="Scripts/md5.js"></script>
    <script  type="text/javascript" src="Scripts/d5c.js"></script>
    <script  type="text/javascript" src="Scripts/auth.js?v=20130703"></script>
     

    <script type="text/javascript" src="Scripts/css/scripts/jquery-1.9.0.min.js"></script>
    <script type="text/javascript" src="Scripts/css/scripts/jquery_slide.js"></script>
   
     <script type="text/javascript">
         $(window).load(function () {
             $('.slideshow').cycle({
                 fx: 'fade',
                 speed: 2200
             });

         });

         ns = (document.layers)? true:false
         ie = (document.all)? true:false
         var AuthenticationFailed = <%=AuthenticationFailed %>;
         var lcookie = null;

         function Encrypt() {
            
             document.forms[0].elements["txtHash"].value = hex_md5(hex_md5(document.forms[0].txtPassword.value) + document.forms[0].txtRnd.value);
             
             //Login.txtHash.value = hex_md5(Login.txtPassword.value+Login.txtRnd.value);

             //document.forms[0].elements["txtWidth"].value=screen.width;
             //document.forms[0].elements["txtHeight"].value=screen.height;
             if (ie) {
                 document.forms[0].elements["txtWidth"].value = document.body.clientWidth;
                 document.forms[0].elements["txtHeight"].value = document.body.clientHeight;
             }
             else {
                 document.forms[0].elements["txtWidth"].value = window.innerWidth;
                 document.forms[0].elements["txtHeight"].value = window.innerHeight;
             }

             document.forms[0].submit();
         }

         function press(e) {
             if (ie) {
                 if (event.keyCode == 13) {
                     Encrypt();
                 }
             }
         }

         function setupLogin() {

             try {
                 lcookie = readCookie("sfm_login");
             }
             catch (e) { }

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
             try {
                 window.external.AutoCompleteSaveForm(document.forms[0]);
             }
             catch (err) { }

              try {
                $('cmdLogin').disable();
             }
             catch (err) { }
             

             var val = ($("rblSFM_1").checked) ? 1 : 0;
             if ((lcookie === null) || ($("rblSFM_" + lcookie) == null))
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
         Event.observe(document, "keydown", function (event) { press(event) });

         function prefocuslogin() {
             if (AuthenticationFailed == 0) {
                 $('txtUserName').focus();
             }
             else {
                 $('txtPassword').focus();
                 setTimeout(function () {
                     document.getElementById("txtPassword").value = '';
                 }, 100)

             }
         }

    </script>
</head>

<body>
    
   <div id="wrapper">
  <div id="inner_wrapper">
    <header>
      <div class="header">
      	<div class="logo">
              <asp:Image ID="imgProdLogo" runat="server" Visible="false"></asp:Image>                                   
      	</div>
      </div>
    </header>
    <section>
      <div class="leftPnl">
      	<div class="left_section">
        	<div class="row">
            	<div class="col6">


                     <div class="banner">
    <div style="position:relative;" class="slideshow">
       <div class="banner_01"></div>
       <div class="banner_02"></div>
       <div class="banner_03"></div>
       
   </div>
  </div>


            	</div>                              
            </div>
            
        </div>
      </div>
      <div class="rightPnl">
      	<div class="login">
        	<div class="login_header" >
                
        	</div>
               <div style="text-align:center"><b>Telematics Portal</b></div>
        	
            <div class="login_section">
            	<form id="form1" runat="server" class="form-signin">
                    
        <h2 class="form-signin-heading">Please Sign In</h2>

                    <div class="formL">
                        
                        <div class="lableB">
                             <asp:Label ID="lblUserName" runat="server" meta:resourcekey="lblUserNameResource1" cssClass="sr-only"
                                    Text="User Name:">
                                <asp:RequiredFieldValidator ID="valUserName" runat="server" ControlToValidate="txtUserName"
                                    ErrorMessage="Please enter a user name!" meta:resourcekey="valUserNameResource1"
                                    Text="*"></asp:RequiredFieldValidator> 

                           </asp:Label>

                        </div>
                       <div class="txtBx"> 
                           <asp:TextBox ID="txtUserName" runat="server" meta:resourcekey="txtUserNameResource1" class="form-control" placeholder="Username"></asp:TextBox>
                        </div>
                    </div>
                    
        <div class="formL">
              <div class="lableB">
                  <asp:Label ID="lblPassword" runat="server" CssClass="sr-only" meta:resourcekey="lblPasswordResource1"
                                    Text="Password:">
                 
                      <asp:RequiredFieldValidator ID="valPassword" runat="server"
                                        ControlToValidate="txtPassword" ErrorMessage="Please enter a password!" meta:resourcekey="valPasswordResource1"
                                        Text="*" ></asp:RequiredFieldValidator> 

                  </asp:Label>
                 
              </div>
              <div class="txtBx"> <asp:TextBox ID="txtPassword" runat="server" meta:resourcekey="txtPasswordResource1" class="form-control" placeholder="Password" TextMode="Password"></asp:TextBox> 
               </div>
              </div>
                  <div style="display:inline-block; width:100%; text-align:center; margin-bottom:5px;">
                    
                         <%--<asp:LinkButton ID="lnkEnglish" runat="server" CausesValidation="False" OnClick="lnkEnglish_Click"
                                        meta:resourcekey="lnkEnglishResource1">ENGLISH</asp:LinkButton>
                               |
                         <asp:LinkButton ID="lnkFrench" runat="server" CausesValidation="False"
                                        OnClick="lnkFrench_Click" meta:resourcekey="lnkFrenchResource1" Text="FRANÇAIS"></asp:LinkButton>      --%>
                           
                            
                  </div>                      
       
                           
       <asp:Button ID="cmdLogin" runat="server" CommandName="25" OnClientClick="javascript:prepareLogin()" CausesValidation="true" 
           Text="Sign In" class="btn btn-lg btn-primary btn-block" meta:resourcekey="cmdLoginResource2" UseSubmitBehavior="True"/>                                                                               
        <div style="clear:both"></div>
                 
                    <div style="display:inline-block; width:100%; text-align:center;">
                        <asp:Label ID="lblMessage" runat="server" Visible="False" meta:resourcekey="lblMessageResource1" ForeColor="#FF3300"></asp:Label>
                <asp:ValidationSummary ID="valSummary" runat="server" meta:resourcekey="valSummaryResource1" ></asp:ValidationSummary>
                        </div>
                <asp:Panel ID="CaptchaPanel" runat="Server" Visible="false">
                            <asp:Image ID="imgCaptcha" runat="server" />
                            <br />
                            <asp:TextBox ID="txtCaptcha" runat="server" ></asp:TextBox>
                            <asp:Button ID="btnRegen" runat="server" Text="Next"  CssClass="commands" />

                </asp:Panel>
                  <asp:Label ID="lblDatabase" runat="server" Text="Database" Visible="False"></asp:Label>
                     <asp:DropDownList ID="cboDataBaseName" Width="180px" runat="server"  Visible="False">               
                           <asp:ListItem Selected="True" Text="SentinelFM" Value="SentinelFM" ></asp:ListItem>   
                     </asp:DropDownList >  
                     <input id="txtHash" type="hidden" name="txtHash" runat="server"/>
         <input id="txtRnd" type="hidden" value='<%=ViewState["auth_seed"]%>' name="txtRnd">&nbsp;&nbsp;
      <input id="txtWidth" type="hidden" name="txtWidth" runat="server"/>
        <input id="txtHeight" type="hidden" name="txtHeight" runat="server"/>
                </form>
            </div>
             
        </div>
      </div>
    </section>
    <footer>
    <div style="clear:both"></div>
      <div class="footer">
      	<%--<p> The Ultimate CSS Gradient Editor was created by Alex Sirota (iosart). If you like this tool, check out ColorZilla for more advanced tools such as eyedroppers, color pickers, palette editors and website analyzers. As you might know, HTML5 introduced many exciting features for Web developers. One of the features is the ability to specify gradients using pure CSS3, without having to create any images and use them as repeating backgrounds for gradient effects. </p>
        <p> The Ultimate CSS Gradient Editor was created by Alex Sirota (iosart). If you like this tool, check out ColorZilla for more advanced tools such as eyedroppers, color pickers, palette editors and website analyzers. As you might know, HTML5 introduced many exciting features for Web developers. One of the features is the ability to specify gradients using pure CSS3, without having to create any images and use them as repeating backgrounds for gradient effects. </p>--%>
      <p>          
	        	<asp:Label ID="lblCustomerSupport" runat="server"   meta:resourcekey="lblCustomerSupport" Text="Customer Support:" Visible="false"></asp:Label></p>
              <p> <asp:Label ID="lblCustomerSupportPhone"  meta:resourcekey="lblCustomerSupportPhone" runat="server"  Text=" " Visible="false" ></asp:Label> </p>
                 <p>  <asp:Label ID="lblTollFreeNumber" runat="server" Text="" ></asp:Label>  <asp:Label ID="lblEmail" Text="" runat="server"   meta:resourcekey="lblEmail" Visible="false"  ></asp:Label>  </p>
                   <p>   <asp:HyperLink NavigateUrl="" runat="server" ID="lblEmailurl" Text="" meta:resourcekey="lblEmailurl"  Visible="false" ></asp:HyperLink>  </p>
               <p> <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label></p>
    		
     <p><asp:Label ID="lblMessageError" runat="server" Text="Your account has been deactivated, please contact with your administrator." Visible="false" ForeColor="Red"></asp:Label></p>
      <p> <asp:Label ID="Label2" runat="server" meta:resourcekey="Label2Resource1" Text="This computer system is the property of Fluor Corporation and its subsidiaries (Fluor), or of its designated agents, and is intended for authorized users only. The computer system must be used in accordance with Fluor's company policies, including the Fluor IT Asset Usage Guidelines and User ID and Password Controls Policies, copies of which are available at www.fdnet.com/security/policies.htm. Unauthorized access and improper use of the system, and any information or materials made available through the system, is prohibited. All users of this system are subject to having their activities on the system monitored and recorded by Fluor in accordance with its policies. Accordingly, and except where stated in those policies, and except where prohibited by applicable local law, users of the system should have no expectation of privacy. Use of the system, and any information or materials made available through the system, contrary to Fluor's policies or applicable law, or other than for the purpose for which they were provided, may result in the immediate termination or suspension of access privileges. Further, where required or allowed by law or Fluor's policies, Fluor may disclose the results of such monitoring to third parties, including without limitation, law enforcement officials. This computer system and all information and materials made available through the system are provided 'as is' and 'when available' and without warranties of any kind. Fluor shall not be liable for any direct, indirect, special, consequential, or any other damages in connection with or arising out of the furnishing, performance or use of its computer system and the information and materials made available through such system."></asp:Label></p>
                <p><asp:Label ID="Label4"  runat="server" meta:resourcekey="Label4Resource1" Text="Legal | Trademarks | Privacy Policy | © 2014 Fluor Corporation. All Rights Reserved. | Need Assistance? Contact BSM Support Toll Free at 1-866-578-4315. Support Hours: 8:00 AM - 5:00 PM ET Monday – Sunday, and after hours on call. You may also e-Mail us at customercare@bsmwireless.com."></asp:Label></p>
                <p><asp:Label ID="Label1"  runat="server"  Text=" "></asp:Label></p>
                <p><asp:Label ID="lblPopup"  runat="server" Style="font-weight: bold;" meta:resourcekey="lblPopupResource1"
                    Text=""></asp:Label></p>
         
      </div>
    </footer>
  </div>
</div>
    
</body>
</html>
