<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test1.aspx.cs" Inherits="SentinelFM.Map_Test1" %>

<%@ Register Src="CommandsControls/BoxSetup.ascx" TagName="BoxSetup" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:BoxSetup ID="BoxSetup1" runat="server" />
    
    </div>
    </form>
</body>
</html>
