<%@ Page Language="c#" Inherits="SentinelFM.Reports.frmReportBack" CodeFile="frmReportBack.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmReportBack</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body class="configTabBackground">
    <form id="Form1" method="post" runat="server">
        <table width="100%" style="z-index: 101; left: 3px; position: absolute; top: 0px; border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none;" border=0 cellpadding=0 cellspacing=0     >
            <tr>
                <td align="right">
                    <asp:Button runat="server" CssClass="combutton" ID="cmdClose" Text="BACK" OnClientClick="parent.parent.location.href='frmReportsNav.aspx'"
                        Font-Bold="True" ForeColor="White" BackColor="gray" meta:resourcekey="cmdCloseResource1" /><%--<input class="combutton"
                            id="cmdClose" type="button" value="Back" onclick="javascript:history.go(-2)"
                            name="cmdCancel" style="font-weight: bold; text-transform: uppercase; color: white;
                            background-color: gray">--%></td>
            </tr>
        </table>
    </form>
</body>
</html>
