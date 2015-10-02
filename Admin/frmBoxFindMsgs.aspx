<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmBoxFindMsgs.aspx.cs" Inherits="frmBoxFindMsgs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Find Missing Messages for a Box</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding: 20px 20px 20px 20px; font-family: Verdana; font-size: small">
      <table>
         <tr>
            <td>Box</td>
            <td>
               <asp:TextBox runat="server" ID="BoxId"></asp:TextBox>
            </td>
            <td>Date From</td>
            <td>
               <asp:TextBox runat="server" ID="DateFrom"></asp:TextBox>
            </td>
            <td>Date To</td>
            <td>
               <asp:TextBox runat="server" ID="DateTo"></asp:TextBox>
            </td>
         </tr>
      </table>
      <br />
      <asp:Button runat="server" ID="Load" Text="LoadMessages" OnClick="Load_Click" />
      <hr />
      <asp:Label runat="server" ID="LabelMessage" ForeColor="red" Font-Size="Small"></asp:Label>
      <div style="overflow: auto; height: 400px; width:98%; padding: 20px 20px 20px 0px">
         <span runat="server" id="Messages"></span>
      </div>
    </div>
    </form>
</body>
</html>
