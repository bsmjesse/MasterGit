<%@ Page Language="C#" AutoEventWireup="true" CodeFile="error.aspx.cs" Inherits="error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Get Data Error</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin: 20px 20px 20px 20px; padding: 20px 20px 20px 20px; border: double 2px Red">
      <asp:Label ID="ErrorMessage" runat="server" ForeColor="Red" Font-Names="Verdana,Arial" Font-Size="Large"></asp:Label>
    </div>
    </form>
</body>
</html>
