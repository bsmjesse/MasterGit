<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommentEdit.ascx.cs" Inherits="Maintenance_CommentEdit" %>
<table>
   <tr valign="top">
     <td>
        <asp:Label ID="lblComment" runat="server" Text="Comment:" meta:resourcekey="lblCommentResource1"  ></asp:Label>
     </td>
     <td>
        <asp:TextBox ID="txtComment" runat="server" Width ="360px" Height="100px" 
             MaxLength="250" TextMode="MultiLine"  ></asp:TextBox>
     </td>
   </tr>
   <tr>
      <td colspan="2" align="center">
         <asp:Button ID="btnSave" runat="server" Text ="Save"  
              meta:resourcekey="btnSaveResource1" OnClientClick="return SaveEditComment();" />
         &nbsp;
         <asp:Button ID="btnClose" runat="server" Text ="Close"  meta:resourcekey="btnCloseResource1" OnClientClick="return CloseEditComment();" />
      </td>
   </tr>
</table>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >
        function SetCommentTextField(comment) {
            $telerik.$("#<%= txtComment.ClientID%>").val(comment);
        }
        function GetCommentTextField(comment) {
            return $telerik.$("#<%= txtComment.ClientID%>").val(comment);
        }

    </script>
</telerik:RadCodeBlock>