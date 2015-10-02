<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MapNavigation.ascx.cs" Inherits="UserControl_MapNavigation" %>
<table>
   <tr>
      <td>
         <table cellpadding=0 cellspacing=0 border=0   >
            <tr>
               <td >
               </td>
               <td >
                  <asp:ImageButton ID="ImageButton1"  runat="server" ImageUrl="~/images/icon_go_up.gif" /></td>
               <td >
               </td>
            </tr>
            <tr>
               <td >
                  <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/icon_go_left.gif" /></td>
               <td >
               </td>
               <td >
                  <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/images/icon_go_right.gif" /></td>
            </tr>
            <tr>
               <td >
               </td>
               <td >
                  <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/icon_go_down.gif" /></td>
               <td >
               </td>
            </tr>
         </table>
      </td>
      <td>
         &nbsp;&nbsp;</td>
      <td >
       <table  >
            <tr>
               <td ><asp:ImageButton ID="cmdZoomOut" runat="server" ImageUrl="~/images/iconMinus.gif" OnClick="cmdZoomOut_Click" /></td>
               <td >
                  <asp:RadioButtonList ID="optMapLevel" Font-Size=0  runat="server" RepeatDirection="Horizontal">
                     <asp:ListItem Value=0></asp:ListItem>
                     <asp:ListItem Value=1></asp:ListItem>
                     <asp:ListItem Value=2></asp:ListItem>
                     <asp:ListItem Value=3></asp:ListItem>
                     <asp:ListItem Value=4></asp:ListItem>
                     <asp:ListItem Value=5 Selected="True"></asp:ListItem>
                  </asp:RadioButtonList></td>
               <td ><asp:ImageButton ID="cmdZoomIn" runat="server" ImageUrl="~/images/iconPlus.gif" OnClick="cmdZoomIn_Click" /></td>
            </tr>
         </table>
      </td>
      <td >&nbsp;
      </td>
      <td >
      </td>
      <td >
      </td>
   </tr>
</table>
