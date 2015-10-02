<%@ Page Language="c#" Inherits="SentinelFM.frmviewSearchMap" CodeFile="frmviewSearchMap.aspx.cs" Culture="en-US"  UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Map</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <img id="imgMap" style="border-right: gray 3px double; border-top: gray 3px double;
            z-index: 101; left: 8px; border-left: gray 3px double; border-bottom: gray 3px double;
            position: absolute; top: 8px" height="361" src="<%=sn.Map.ImgPath%>" width="655"
            border="0">
        <input class="combutton" id="cmdClose" style="z-index: 102; left: 562px; width: 106px;
            position: absolute; top: 381px; height: 19px" onclick="window.close()" type="button"
            value="Close" name="cmdCancel"><!-- NEED TO LOCALIZE INPUT BUTTON -->
    </form>
</body>
</html>
