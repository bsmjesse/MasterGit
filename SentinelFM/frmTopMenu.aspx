<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmTopMenu.aspx.cs" Inherits="SentinelFM.frmTopMenu" meta:resourcekey="PageResource2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title> 
    <meta content="C#" name="CODE_LANGUAGE"/>
    <meta content="JavaScript" name="vs_defaultClientScript"/>
     
</head>
<body style="margin-top: 0px; margin-left: 0px; margin-bottom:0px;  margin-right: 0px; width: 100%;">
    <script language="javascript" type="text/javascript">

        function keepMeAlive(imgName) {
            myImg = document.getElementById(imgName);
            if (myImg) myImg.src = myImg.src.replace(/\?.*$/, '?' + Math.random());
        }

        
            function Contact() { 
                var mypage='<%=strContactUs%>';
		    var myname='ContactUs';
		    var w=300;
		    var h=200;
		    var winl = (screen.width - w) / 2; 
		    var wint = (screen.height - h) / 2;
		    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
		    win = window.open(mypage, myname, winprops) 
		    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
		} 


        function downlandGuide() {
            var mypage = './Help/Guide de l\'usager Sentinel v.3.pdf';
            var myname = '';
            var w = 1000;
            var h = screen.height-100;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + ',location=0,status=0,scrollbars=0,toolbar=1,menubar=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function GotoMobile() {
            window.top.location.href = '/gotomobile.aspx';
        }

    
    </script>     
    <form id="frmMenu" runat="server" method="post"  >
        <table border="0" class="hbg"  cellpadding="0" cellspacing="0"  style="width: 100%;">
            <tr>
                <td style="background-color: white;"  valign="top" align="left" colspan="2">
                    <table width="100%"   border="0" cellpadding="0" cellspacing="0">
                        <tr>
                             <td  align="left" >
				                <img id="keepAliveIMG" width="1" height="1" src="images/ProdLogo.gif" /> 
                                <asp:Image ID="imgProdLogo" runat="server"  
                                   ></asp:Image>
                            </td>
                            <td align="right" valign="bottom" style="height: 28px">                        
                                <asp:Label ID="lblUserTitle" runat="server" Font-Names="verdana" Font-Size="11px" Text="Welcome:" meta:resourcekey="lblUserTitleResource2"/>
                                &nbsp; 
                                <asp:Label ID="lnkUser" runat="server" Font-Names="verdana" Font-Size="11px" meta:resourcekey="lnkUserResource2"/>
                                &nbsp;&nbsp;<br />
                                <asp:LinkButton ID="lnkHelp" runat="server" ForeColor="Black" Font-Names="verdana" Font-Size="9pt" Font-Underline="False" OnClick="lnkHelp_Click" meta:resourcekey="lnkHelpResource1">Help</asp:LinkButton>
                                <asp:LinkButton ID="lnkGuide" runat="server" ForeColor="Black" Font-Names="verdana" Font-Size="9pt" Font-Underline="False" OnClientClick="javascript:downlandGuide();">(Guide de l'usager Sentinel)</asp:LinkButton>
                                &nbsp; |&nbsp;
                                <asp:LinkButton ID="lnkMobile" runat="server" Visible="false"  Font-Names="verdana" Font-Size="9pt" Font-Underline="False" ForeColor="Black"  OnClientClick="javascript:GotoMobile();return false;" meta:resourcekey="MobileButton1Resource1">Mobile</asp:LinkButton>                                        
                                <asp:Label ID="lnkMobileSepertor" runat="server" Visible="false" Text="&nbsp;|&nbsp;"></asp:Label>
                                <asp:LinkButton ID="lnkCotactUs" runat="server"  Font-Names="verdana" Font-Size="9pt" Font-Underline="False" ForeColor="Black"  OnClientClick="javascript:Contact();" meta:resourcekey="LinkButton1Resource1">Contact Us</asp:LinkButton>                                        
                               
                                <asp:LinkButton ID="lnkLite" runat="server"  Font-Names="verdana" Font-Size="9pt" Font-Underline="False" ForeColor="Black" Visible="False" onclick="lnkLite_Click">Lite</asp:LinkButton>
                                <asp:Label ID="lnkLiteSepertor" runat="server" Text="&nbsp;|&nbsp;"></asp:Label>
                                <asp:LinkButton ID="lnkLogout" runat="server"  Font-Names="verdana" Font-Size="9pt" Font-Underline="False" ForeColor="Black" OnClick="lnkLogout_Click" meta:resourcekey="lnkLogoutResource1">Log Out</asp:LinkButton>
                                &nbsp;&nbsp;                                                                   
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 754px;height:27px" valign="middle">
                    <asp:Menu id="mnuMenu" runat="server" Font-Overline="False" Font-Names="verdana,11px" Font-Italic="False" Font-Bold="False" meta:resourcekey="mnuMenuResource1" OnMenuItemClick="mnuMenu_MenuItemClick" Orientation="Horizontal">
                        <StaticSelectedStyle ForeColor="Yellow"/>
                        <StaticMenuItemStyle HorizontalPadding="7px" Font-Bold="False" Font-Names="Verdana" Font-Size="11px" ForeColor="White"/>
                        <StaticHoverStyle Font-Underline="True"/>
                        <Items>
                            <asp:MenuItem Selected="True"  Text="Home" Value="Home/frmMainHome.aspx" meta:resourceKey="MenuItemResource1"/>
                            <asp:MenuItem Text="Map" Value="Map/frmMain.aspx" meta:resourceKey="MenuItemResource2"/>
                            <asp:MenuItem Text="Map<sup>new</sup>" Value="NewMap.aspx" meta:resourceKey="MenuItemResource15"/>
                            <asp:MenuItem Text="History" Value="History/frmhistmain_new.aspx" meta:resourceKey="MenuItemResource3"/>
                            <asp:MenuItem Text="Maintenance" Value="Maintenance/frmMaintenanceGrid.aspx" meta:resourceKey="MenuItemResource4"  />
                            <asp:MenuItem Text="Alarms" Value="Messages/frmAlarms.aspx" meta:resourceKey="MenuItemResource14" />
                            <asp:MenuItem Text="Messages" Value="Messages/frmMessages.aspx" meta:resourceKey="MenuItemResource5"/>
                            <asp:MenuItem Text="Landmarks &amp; Geozones" Value="GeoZone_Landmarks/frmLandmark.aspx" meta:resourceKey="MenuItemResource6"/>
                            <asp:MenuItem Text="Reports" Value="Reports/frmReportMaster.aspx" meta:resourceKey="MenuItemResource7"/>
                            <asp:MenuItem Text="Events" Value="EventViewer.aspx"/>
                            <asp:MenuItem Text="HOS" ToolTip="Hours of Service" Value="HOS/frmManagingHOS.aspx" meta:resourcekey="MenuItemResource11"/>
                            <asp:MenuItem Text="Administration" Value="Configuration/frmEmails.aspx" meta:resourceKey="MenuItemResource8"/>
                            <asp:MenuItem Text="User Preferences" Value="Configuration/frmpreference.aspx" meta:resourceKey="MenuItemResource13"/>                           
                           <asp:MenuItem Text="HOS Dashboard" Value="HOS/HOSDashBoard.aspx" meta:resourcekey="MenuItemResource22" />
			 <asp:MenuItem Text="Dashboard" Value="DashBoard/portal.aspx" meta:resourceKey="MenuItemResource20"/>
                            <asp:MenuItem Text="Directions" Value="routing/Main.aspx"></asp:MenuItem>
							<asp:MenuItem Text="Schedule Adherence" Value="ScheduleAdherence/frmReport.aspx"></asp:MenuItem>
                        </Items>
                    </asp:Menu>  
                </td>
            </tr>
        </table>
    </form>   
</body>
</html>
