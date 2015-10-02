<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CurrentValue.ascx.cs" Inherits="Maintenance_CurrentValue" %>
<table width="100%">
   <tr valign="top">
     <td  style="width:150px">
        <asp:Label ID="lblHrs" runat="server" CssClass="formtext" Text="Current Engine Hours:" meta:resourcekey="lblHrsResource1"  ></asp:Label>
     </td>
     <td align="left">
        <asp:Label ID="lblHrsValue" runat="server"  CssClass="formtext" meta:resourcekey="lblHrsValueResource1"    ></asp:Label>
     </td>
    </tr>
    <tr>
     <td style="width:150px">
        <asp:Label ID="lblOdo" runat="server" CssClass="formtext" Text="Current Odometer:" meta:resourcekey="lblOdoResource1"  ></asp:Label>
     </td>
     <td align="left">
        <asp:Label ID="lblOdoValue" CssClass="formtext" runat="server"   meta:resourcekey="lblOdoValueResource1"    ></asp:Label>
     </td>

   </tr>
   <tr>
      <td align="center" colspan="2"></td>
   </tr>
   <tr>
      <td align="center" colspan="2">
         <asp:Button ID="btnCLose" runat="server" Text="Close" 
              OnClientClick="return CloseCurrentValueWin();" CssClass="combutton" 
              meta:resourcekey="btnCLoseResource1"  />
      </td>
   </tr>
</table>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >
        function ShowCurrentOdoandEngHrs(hrs, meters) {
            $telerik.$("#<%= lblHrsValue.ClientID%>").html(hrs);
            $telerik.$("#<%= lblOdoValue.ClientID%>").html(meters);
        }

    </script>
</telerik:RadCodeBlock>