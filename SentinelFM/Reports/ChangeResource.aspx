<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangeResource.aspx.cs" Inherits="ChangeResource" 
    Culture="en-US" meta:resourcekey="PageResource1"
    UICulture="auto" 
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <asp:Button ID="btnbyckey" runat ="server" text ="Change by key" onclick="btnbyckey_Click" 
              />
       <asp:Button ID="btnbyValue" runat ="server" text ="Change by value" onclick="btnbyValue_Click" 
             />
       
    </div>
    </form>
</body>
</html>
