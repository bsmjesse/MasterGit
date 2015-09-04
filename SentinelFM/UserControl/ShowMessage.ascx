<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShowMessage.ascx.cs" Inherits="UserControl_ShowMessage" %>	
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table>
   <tr>
      <td align="left" valign="top" >
         <asp:Image ID="img"  runat="server" ImageUrl= "~/images/caution.gif" />
      </td>
      <td>
          <asp:TextBox id="txtMessage"  runat="server" CssClass="errortext" ForeColor="Red" Width="405px" Height="190px" TextMode="MultiLine" ReadOnly="True" BorderStyle="Solid" BackColor="White" BorderColor="#E0E0E0" Wrap="true" ></asp:TextBox>
      </td>
   </tr>
   <tr>
      <td colspan="2" align="center">
         <input class="combutton" id="cmdOk"   onclick="ClosefrmShowMessage()"   type="button" value="Ok" name="cmdOk">
      </td>
   </tr>
</table>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
     <script type="text/javascript">
         function ShowMessage_SetTxtMessage(msg) {
             $telerik.$("#<%= txtMessage.ClientID %>").text(msg);
         }
     </script>
</telerik:RadCodeBlock>  
			

