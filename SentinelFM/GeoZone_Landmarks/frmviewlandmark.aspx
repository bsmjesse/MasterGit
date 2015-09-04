<%@ Page Language="c#" Inherits="SentinelFM.GeoZone_Landmarks.frmViewLandMark" CodeFile="frmViewLandMark.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Landmark</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <img id="imgMap" class="tableDoubleBorder"  style="z-index: 101; left: 8px;position: absolute; top: 8px" height="361" src="<%=sn.Map.ImgPath%>" width="655"
            border="0">
        
        <asp:Button ID="cmdCLoseWindow"  class="combutton" 
            OnClientClick="window.close()"  style="z-index: 102; left: 562px; width: 106px;
            position: absolute; top: 381px; height: 19px" runat="server" Text="Close" 
            meta:resourcekey="cmdCLoseWindowResource1" />
    </form>
</body>
</html>
