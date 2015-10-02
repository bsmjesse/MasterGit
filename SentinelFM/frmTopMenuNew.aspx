<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmTopMenuNew.aspx.cs" Inherits="SentinelFM.frmTopMenuNew"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<meta content="C#" name="CODE_LANGUAGE">
 <meta content="JavaScript" name="vs_defaultClientScript">
    
    <title></title>

       <link href="GlobalStyle.css" type="text/css" rel="stylesheet" />
</head>
<body style="margin-top: 2px; margin-left: 2px; margin-bottom:0px;  margin-right: 0px; width: 100%;" >
 
    <form id="frmMenu" runat="server" method="post"  >
        <table border="0" cellpadding="0" cellspacing="0" class="hbg" style="width: 100%;">
            <tr>
                <td style="min-width: 754px;" valign=top><nobr><asp:Menu id="mnuMenu" runat="server" Font-Overline="False" Font-Names="verdana,11px" Font-Italic="False" Font-Bold="False" meta:resourcekey="mnuMenuResource1" OnMenuItemClick="mnuMenu_MenuItemClick" Orientation="Horizontal">
<StaticSelectedStyle ForeColor="Yellow"></StaticSelectedStyle>

<StaticMenuItemStyle HorizontalPadding="7px" Font-Bold="False" Font-Names="Verdana" Font-Size="11px" ForeColor="White"></StaticMenuItemStyle>

<StaticHoverStyle Font-Underline="True"></StaticHoverStyle>
<Items>
<asp:MenuItem Selected="True"  Text="Home" Value="Home/frmMainHome.aspx" meta:resourceKey="MenuItemResource1"></asp:MenuItem>
<asp:MenuItem Text="Map" Value="Map/frmMain.aspx" meta:resourceKey="MenuItemResource2"></asp:MenuItem>
<asp:MenuItem Text="History" Value="History/frmhistmain.aspx" meta:resourceKey="MenuItemResource3"></asp:MenuItem>
<asp:MenuItem Text="Maintenance" Value="Maintenance/frmMaintenance.aspx" meta:resourceKey="MenuItemResource4"></asp:MenuItem>
<asp:MenuItem Text="Messages" Value="Messages/frmMessages.aspx" meta:resourceKey="MenuItemResource5"></asp:MenuItem>
<asp:MenuItem Text="Landmarks &amp; Geozones" Value="GeoZone_Landmarks/frmLandmark.aspx" meta:resourceKey="MenuItemResource6"></asp:MenuItem>
<asp:MenuItem Text="Reports" Value="Reports/frmReportMaster.aspx" meta:resourceKey="MenuItemResource7"></asp:MenuItem>
<asp:MenuItem Text="HOS" ToolTip="Hours of Service" Value="HOS/frmManagingHOS.aspx" meta:resourcekey="MenuItemResource11"></asp:MenuItem>
<asp:MenuItem Text="Configuration" Value="Configuration/frmEmails.aspx" meta:resourceKey="MenuItemResource8"></asp:MenuItem>
<asp:MenuItem Text="Dashboard" Value="Dashboard" meta:resourcekey="MenuItemResource12"></asp:MenuItem>
<asp:MenuItem Text="Help" Value="Help/frmHelp.aspx" meta:resourceKey="MenuItemResource9"></asp:MenuItem>
<asp:MenuItem Text="Logout" Value="Logout" meta:resourceKey="MenuItemResource10"></asp:MenuItem>
</Items>
</asp:Menu> </nobr>
                </td>
                <td valign=top align="center"  style=""   >
                   <table >
                      <tr>
                         <td >
                    <asp:Label ID="lblUserTitle" runat="server" Text="User:"  ForeColor="White"
                        Font-Names="Verdana" Font-Size="11px" meta:resourcekey="lblUserTitleResource1"></asp:Label></td>
                         <td >
                    <asp:LinkButton ID="lnkUser" runat="server" Font-Bold="False" Font-Names="verdana,11px"
                        Font-Size="11px" OnClick="lnkUser_Click" meta:resourcekey="lnkUserResource1"
                        ForeColor="White" Font-Italic="False"></asp:LinkButton></td>
                      </tr>
                   </table>
                </td>
            </tr>
        </table>
       
    
        <iframe visible=false   src="frmSessionValidate.aspx"  height="0px"  width="0px"   ></iframe>    
    </form>
    
</body>
</html>
