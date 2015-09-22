<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmResetPassword.aspx.cs" Inherits="SentinelFM.frmResetPassword" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>



<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Reset Password</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    
    
</head>
<body>
    <form id="form1" runat="server" method=post >
    
       <table class="formtext" style="width: 500px">
          <tr>
             <td style="width: 175px">
                <asp:Label ID="lblNewPsw" runat="server" meta:resourcekey="lblNewPswResource1" Text="Enter a new password:"></asp:Label></td>
             <td colspan="2">
                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="formtext"  
                   meta:resourcekey="txtNewPasswordResource1" TabIndex="2" TextMode="Password" Width="140px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="valNewPassword" runat="server" ControlToValidate="txtNewPassword"
                   ErrorMessage="Enter a new Password" meta:resourcekey="valNewPasswordResource1"
                   Text="*"></asp:RequiredFieldValidator>&nbsp;<span id="strength" runat=server ></span></td>
          </tr>
          <tr>
             <td style="width: 175px">
                <asp:Label ID="lblReenterPsw" runat="server" 
                   Text="Re-enter the new password:" meta:resourcekey="lblReenterPswResource1"></asp:Label>
                   </td>
             <td colspan="2">
                <asp:TextBox ID="txtNewPassword1" runat="server" CssClass="formtext"
                    TabIndex="2" TextMode="Password" Width="140px" meta:resourcekey="txtNewPassword1Resource1"></asp:TextBox><asp:CompareValidator ID="vlComp" runat="server" ControlToCompare="txtNewPassword1"
                   ControlToValidate="txtNewPassword" ErrorMessage="Please re-enter the new password"
                    Text="*" meta:resourcekey="vlCompResource1"></asp:CompareValidator></td>
          </tr>
          <tr>
             <td colspan="3" align="center">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext"
                   meta:resourcekey="ValidationSummary1Resource1" />
                <asp:Label ID="lblPswMsg" runat="server" CssClass="errortext" meta:resourcekey="lblPswMsgResource1"
                   Visible="False"></asp:Label>                 
                <input id="txtPasswordStatus" type="hidden" name="txtPasswordStatus"></td>
          </tr>
          <tr>
             <td align="center" colspan="3">
                &nbsp;
                <asp:Button ID="cmdCancel" runat="server" CssClass="combutton" Text="Cancel" OnClientClick="javascript:window.close();"  meta:resourcekey="cmdCancelResource1" />
                <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1" /></td>              
          </tr>
           <tr>
               <td align="center" colspan="3">
                   <asp:Label ID="lblPswMsgRule" runat="server" CssClass="tableheading" meta:resourcekey="lblPswMsgRuleResource1"
                   Visible="False"></asp:Label>
               </td>
           </tr>
       </table>
    
    
    </form>
    
        <script language="javascript">
         function passwordChanged() 
         {
	            var strength = document.getElementById('strength');
	            var strongRegex = new RegExp("^(?=.{9,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
	            var mediumRegex = new RegExp("^(?=.{8,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$", "g");
	            var enoughRegex = new RegExp("(?=.{7,}).*", "g");
	            var pwd = document.getElementById("txtNewPassword");
	            var txtPasswordStatus=document.forms[0].elements["txtPasswordStatus"];
	            
	            
	            if (pwd.value.length==0) {
		            strength.innerHTML ='<%=msgPsw_TypePassword%>';
	            } else if (false == enoughRegex.test(pwd.value)) {
		            strength.innerHTML ='<%=msgPsw_MoreCharacters%>';
		             txtPasswordStatus.value="0";
	            } else if (strongRegex.test(pwd.value)) {
		            strength.innerHTML = '<span style="color:green"><%=msgPsw_Strong%></span>';
		            txtPasswordStatus.value="1";
	            } else if (mediumRegex.test(pwd.value)) {
		            strength.innerHTML = '<span style="color:orange"><%=msgPsw_Medium%></span>';
		             txtPasswordStatus.value="1";
	            } else { 
		            strength.innerHTML = '<span style="color:red"><%=msgPsw_Weak%></span>';
		            txtPasswordStatus.value="0";
	            }
         }
</script>
</body>
</html>
