<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Message.ascx.cs" Inherits="SentinelFM.MapNew_UserControl_Message" %>
    
       <script type="text/javascript" language="javascript">
		<!--


           function MessageWindow() {
               var mypage = 'Messages/frmNewMessageMain.aspx'
               var myname = '';
               var w = 560;
               var h = 590;
               NewWindow(mypage, myname, w, h);
               //parent.parent.frmMain_Top_ViewWindow(mypage, w, h);
           }
				
				
				

		//-->
    </script>
     <table  ><tr><td align=center  >
        <asp:Button ID="cmdNewMessage" runat="server" Font-Size="10px" CssClass="commands" Text="New Text Message"
            Width="208px" CommandName="25" OnClientClick="javascript:MessageWindow(); return false;"  meta:resourcekey="cmdNewMessageResource1"></asp:Button>
            </td></tr></table> 