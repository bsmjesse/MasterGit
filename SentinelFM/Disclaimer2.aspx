<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Disclaimer2.aspx.cs" Inherits="SentinelFM.Disclaimer2" Culture="en-US" UICulture="auto" meta:resourcekey="PageResource1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Disclaimer</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="JavaScript" name="vs_defaultClientScript" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <meta http-equiv="Pragma" content="no-cache" name="prevent_caching1" />
    <meta http-equiv="Cache-Control" content="no-cache" name="prevent_caching2" />
    <meta http-equiv="Expires" content="0" name="prevent_caching3" />
 <link href="Styles/styles.css" rel="stylesheet" />
    <script src="Styles/css-pop.js"></script>

    <script type="text/javascript" >

        function disagree_ClientClick() {

            document.forms[0].elements["txtEmail"].value = '';

            document.forms[0].elements["lblRequireEmail"].value = '';

        }


        function popup(windowname) {
            //blanket_size(windowname);
            //window_pos(windowname);
            toggle('blanket');
            toggle(windowname);
        }
        </script>
</head>

<body>
    <form id="form1" runat="server">
        <asp:scriptmanager runat="server"></asp:scriptmanager>
      
        <div id="blanket" style="display:none;"></div>
	<div id="popUpDiv" style="display:none;" class="smart-green" runat="server" defaultfocus="txtEmail">
        <div  class="head" >
        <div style="text-align:right; padding-right:10px;">
            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Styles/SentinelFM/images/close.png" OnClick="btnDisagree_Click" meta:resourcekey="ImageButton1Resource1"/>

        </div>
             <div >
              <asp:Label ID="Label5" runat="server" Text="Disclaimertest" meta:resourcekey="lblDisclaimer" Font-Size="24px"/>   
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
                <asp:TextBox ID="txtEmail" runat="server"  width="95%" ValidationGroup="emailGroup" placeholder="<%$ Resources:PHEmail %>" meta:resourcekey="txtEmailResource1"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" style="display:none"  ErrorMessage="Invalid Email Id"  ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                    ControlToValidate="txtEmail" ValidationGroup="emailGroup" SetFocusOnError="True" meta:resourcekey="InvalidEmailResource1"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" style="display:none" meta:resourcekey="valEmailResource1" ErrorMessage='Email cannot be left blank'   ValidationGroup="emailGroup" ControlToValidate="txtEmail" SetFocusOnError="True" ></asp:RequiredFieldValidator>
            </div>           
        </div>
    	<div style="text-align:right;width:100%;float:left;margin-top:10px">
           
            <asp:Button ID="btnDisagree" runat="server" Text="Disagree" OnClick="btnDisagree_Click" CssClass="button" meta:resourcekey="btnDisagreeTxt" CausesValidation="False"/>
            <asp:Button ID="btnAgree" runat="server" Text="Agree" OnClick="btnSaveEmail_Click"  CssClass="button" meta:resourcekey="btnAgreeTxt" ValidationGroup="emailGroup"/>
   
                
    	</div>
                <span>
            <asp:Label ID="lblRequireEmail" runat="server" ForeColor="Red" meta:resourcekey="lblRequireEmailResource1"></asp:Label>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" style="display:none"  ValidationGroup="emailGroup" meta:resourcekey="ValidationSummary1Resource1"/>

            </span>
         
       </ContentTemplate>
                </asp:UpdatePanel>
        
 	</div>
    </form>
</body>
</html>
