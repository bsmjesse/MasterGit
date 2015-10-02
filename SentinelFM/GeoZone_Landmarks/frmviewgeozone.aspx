<%@ Page Language="c#" Inherits="SentinelFM.GeoZone_Landmarks.frmViewGeoZone" CodeFile="frmViewGeoZone.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>GeoZone</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <img id="imgMap" class="tableDoubleBorder"  style="z-index: 101; left: 8px;position: absolute; top: 8px" height="361" src="<%=sn.GeoZone.ImgPath%>" width="655"
            border="0">
          
            
             <asp:Button ID="cmdCLoseWindow"  class="combutton" 
            OnClientClick="window.close()"  style="z-index: 102; left: 562px; width: 106px;
            position: absolute; top: 381px; height: 19px" runat="server" Text="Close" meta:resourcekey="cmdCLoseWindowResource1" 
             />
            
        <asp:Label ID="lblMessage" Style="position: absolute; top: 379px" runat="server"
            CssClass="errorText" Height="29px" Width="535px" meta:resourcekey="lblMessageResource1"></asp:Label>
    </form>
</body>
</html>
