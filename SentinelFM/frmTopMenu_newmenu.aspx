<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmTopMenu_newmenu.aspx.cs" Inherits="SentinelFM.frmTopMenu_newmenu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height:100%">
<head id="Head1" runat="server">
    <title></title> 
    <meta content="C#" name="CODE_LANGUAGE"/>
    <meta content="JavaScript" name="vs_defaultClientScript"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />

    <script language="javascript" type="text/javascript">
	<!--
        function Contact() {
            var mypage = 'frmContactUs.aspx';
            var myname = 'ContactUs';
            var w = 300;
            var h = 200;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function keepMeAlive(imgName) {
            myImg = document.getElementById(imgName);
            if (myImg) myImg.src = myImg.src.replace(/\?.*$/, '?' + Math.random());
        }

        window.setInterval("keepMeAlive('keepAliveIMG')", 100000); 


    -->
    </script>
    
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" type="text/javascript"></script>    
    <link rel="stylesheet" href="./Styles/SentinelFM/css/sentinel_menu.css" type="text/css" />

</head>
<body style="margin-top: 0px; margin-left: 0px; margin-bottom:0px;  margin-right: 0px; width: 100%; height: 100%">
    <form id="frmMenu" runat="server" method="post" style="margin-bottom:0;" >
        <div id="mainmenu" class="head">
            <div class="header" role="navigation">
                <div class="page">
                    <div class="line nav files_view">
                        <ul class="unit allow_right_click tabs_unit">
                            <li class="logo">
                                <a href="javascript:void(0);" onclick="gotoPage(this, 'Home/frmMainHome.aspx');">
                                    <img id="Img2" alt="home" src="images/Sentinel FM Logo BETA.png" border="0" />
                                </a>
                            </li>

                            <%if (IsHydroQuebec)
                              { %>
                              <li id="Li8">
                                <a id="A8" class="" onclick="gotoPage(this, 'Map/frmMain.aspx');" onmousedown="return false;" href="javascript:void(0);">Map</a>
                            </li>
                            <%}
                              else
                              { %>
                            <li id="files_tab">
                                <a id="files_tab_link" class="" onclick="gotoPage(this, 'Map/frmMain.aspx');" onmousedown="return false;" href="javascript:void(0);">Map</a>
                            </li>
                            <%if (ShowNewMap)
                              { %>
                            <li id="contacts_tab" class="">
                                <a id="contacts_tab_link" class="" onclick="gotoPage(this, '<%=NewMapUrl %>');" onmousedown="return false;" href="javascript:void(0);"><%=NewMapCaption%></sup></a>
                            </li>
                            <%} %>
                            <%} %>

                            
                            <li id="tools_menu" class="">
                                <a id="a15" class="drop_link" onclick="gotoDropdown(this);" onmousedown="return false;" href="javascript:void(0);">Tools<span class="arrow"></span></a>                                
                            </li>

                            <li id="notification_menu" class="">
                                <a id="a11" class="drop_link" onclick="gotoDropdown(this);" onmousedown="return false;" href="javascript:void(0);">Notifications<span class="arrow"></span></a>                                
                            </li>

                            <li id="reports_menu" class="">
                                <a id="a16" class="drop_link" onclick="gotoDropdown(this);" onmousedown="return false;" href="javascript:void(0);">Reports<span class="arrow"></span></a>                                
                            </li>
                            <%if (HosEnabled)
                              { %>
                            <li id="hos_menu" class="">
                                <a id="a13" class="drop_link" onclick="gotoDropdown(this);" onmousedown="return false;" href="javascript:void(0);">HOS<span class="arrow"></span></a>                                
                            </li>
                            <%} %>

                            <li id="gear_menu" class="gear">
                                <a id="a18" class="drop_link" onclick="gotoDropdown(this);" onmousedown="return false;" href="javascript:void(0);"><div style="opacity: 0.5;"><img src="images/gear.png" border="0" /></div><span class="arrow"></span></a>                                
                            </li>
                        </ul>
                        
                    </div>
                </div>
            </div>
        </div>

        <div id="notification_menu_dropdown" class="dropmenu">
            <ul>
                <%if (ShowAlarms)
                  { %>
                <li id="Li1" class="">
                    <a id="a1" class="" onclick="gotoPage(this, 'Messages/frmAlarms.aspx');" onmousedown="return false;" href="javascript:void(0);">Alarms</a>
                </li>
                <%} %>

                <%if (MessagesMenu)
                    { %>
                <li id="Li2" class="">
                    <a id="a2" class="" onclick="gotoPage(this, '<%=MessagesLocation %>');" onmousedown="return false;" href="javascript:void(0);">Messages</a>
                </li>
                <%} %>
            </ul>
        </div>

        <div id="tools_menu_dropdown" class="dropmenu">
            <ul>
                <% if (HistoryMenu) { %>
                <li id="apps_tab" class="">
                    <a id="apps_tab_link" class="" onclick="gotoPage(this, 'History/frmhistmain_new.aspx');" onmousedown="return false;" href="javascript:void(0);">History</a>
                </li>
                    <% } %>

                    <%if(MaintenanceMenu) { %>
                <li id="current_user_tab" class="current_user_tab">
                    <a id="" class="" onclick="gotoPage(this, 'Maintenance/frmMaintenanceGrid.aspx');" href="javascript:void(0);">Maintenance</a>
                </li>
                <%} %>

                <% if (LandmarkGeozonesMenu)
                    { %>
                <li id="Li3">
                    <a id="A3" class="drop_link mhs" onclick="gotoPage(this, 'GeoZone_Landmarks/frmLandmark.aspx');" onmousedown="return false;" href="javascript:void(0);">Landmarks &amp; Geozones</a>
                </li>
                <%} %>                
                <!-- Devin -->
                <% if (IsAnt == "1")
                    { %>
                <li id="Li4">
                    <a id="A4" class="drop_link mhs" onclick="gotoPage(this, 'ant/ant.html');" onmousedown="return false;" href="javascript:void(0);">Dispatch</a>
                </li>
                <%} %>                

            </ul>
        </div>

        <div id="reports_menu_dropdown" class="dropmenu">
            <ul>
                <%if (ReportsMenu)
                    { %>
                <li id="Li4">
                    <a id="A4" class="drop_link mhs" onclick="gotoPage(this, '<%=ReportsLocation %>');" onmousedown="return false;" href="javascript:void(0);">Reports</a>
                </li>
                <%} %>
                
                <% if (!IsHydroQuebec)
                   { %>
                <li id="Li9">
                    <a id="A9" class="drop_link mhs" onclick="gotoPage(this, 'DashBoard/portal.aspx');" onmousedown="return false;" href="javascript:void(0);">DashBoard</a>
                </li>
                <%} %>
            </ul>
        </div>

        <%if (HosEnabled)
          { %>
        <div id="hos_menu_dropdown" class="dropmenu">
            <ul>
                <li id="Li6">
                    <a id="A6" class="drop_link mhs" onclick="gotoPage(this, 'HOS/frmManagingHOS.aspx');" onmousedown="return false;" href="javascript:void(0);">HOS</a>
                </li>
                <li id="Li11">
                    <a id="A17" class="drop_link mhs" onclick="gotoPage(this, 'HOS/HOSDashBoard.aspx');" onmousedown="return false;" href="javascript:void(0);">HOS Dashboard</a>
                </li>
            </ul>
        </div>
        <%} %>

        <div id="gear_menu_dropdown" class="dropmenu">
            <div class="info">
                <div class="username" style="font-weight:bold;"><%=UserName %></div>
                <div class="organizationname" style="color:#666666;"><%=OrganizationName %></div>
            </div>
            <ul>
                <%if (AdministrationMenu)
                    { %>
                <li id="Li7">
                    <a id="A7" class="drop_link mhs" onclick="gotoPage(this, 'Configuration/frmEmails.aspx');" onmousedown="return false;" href="javascript:void(0);">Administration</a>
                </li>
                <%} %>

                            
                <li id="Li10">
                    <a id="A10" class="drop_link mhs" onclick="gotoPage(this, 'Configuration/frmpreference.aspx');" onmousedown="return false;" href="javascript:void(0);">User Preferences</a>
                </li>

                <% if (Convert.ToInt32(Session["superOrganizationId"]) == sn.SuperOrganizationId)
                   { %>
                <li id="Li19">
                    <a id="A22" class="drop_link mhs" onclick="gotoPage(this, 'frmSuperOrganizationMenu.aspx');" onmousedown="return false;" href="javascript:void(0);"><%if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                                                                                                                                                                          { %>Menu organisationnel <% }
                                                                                                                                                                          else { %>Organizations Menu <%} %></a>
                </li>
                <% } %>
                <li id="Li20">
                    <a id="A23" class="drop_link mhs" onclick="gotoPage(this, 'frmMain.htm');" onmousedown="return false;" href="javascript:void(0);">Switch To Old Menu</a>
                </li>
                <%if (sn.User.OrganizationId == 999728 || sn.User.OrganizationId == 480)
                  { %>
				<li id="Li20">
                    <a id="A23" class="drop_link mhs" onclick="gotoMobile();;" onmousedown="return false;" href="javascript:void(0);">Mobile</a>
                </li>
                <%} %>
                <li id="Li13">
                    <hr />
                </li>
                <li id="Li15">
                    <a id="A19" class="drop_link mhs" onclick="gotoPage(this, 'Help/frmHelp.aspx');" onmousedown="return false;" href="javascript:void(0);">Help</a>
                </li>
                <%if (ShowGuide)
                      { %>
                <li id="Li21">                    
                      <a id="A24" class="drop_link mhs" onclick="downlandGuide();" onmousedown="return false;" href="javascript:void(0);">(Guide de l'usager Sentinel)</a>                    
                </li>
                <%} %>
                <li id="Li16">
                    <a id="A20" class="drop_link mhs" onclick="javascript:Contact();" onmousedown="return false;" href="javascript:void(0);">Contact Us</a>
                </li>
                <%if (LiteMenu)
                  { %>
                <li id="Li17">
                    <a id="A21" class="drop_link mhs" onclick="gotoLite('<%=LiteLocation %>');" onmousedown="return false;" href="javascript:void(0);">Lite</a>                    
                </li>
                <%} %>
                <li id="Li18">
                    <asp:LinkButton ID="lnkLogout" class="drop_link mhs" runat="server" OnClick="lnkLogout_Click" meta:resourcekey="lnkLogoutResource1">Log Out</asp:LinkButton>
                </li>
            </ul>
        </div>

        <div class="menuhandler"><a href="javascript:void(0)" onclick="togglemenu();"><img src="images/menuhandler.png" alt="hide/show menu" title="hide/show menu" /></a></div>
        
    </form>   
    <iframe src="Home/frmMainHome.aspx" name="main" id="main" style="width:100%;  border:0;margin:0px"></iframe>    
</body>
</html>

<script type="text/javascript">
    var closeDropdownMenu;
    var dropdowmenuId;
    var menuExpanded = true;
    var originalHeight;
    var menuHeight = 36;
    function gotoPage(o, p) {
               
        $('.currentmenu').removeClass('currentmenu');

        var d = $(o).parents('.dropmenu').attr('id');
        if (d == undefined)
            $(o).parent().addClass('currentmenu');
        else {
            d = d.replace('_dropdown','');
            $('#' + d).addClass('currentmenu');
        }

        if (p == "frmSuperOrganizationMenu.aspx") {
            top.document.all('TopFrame').cols = '0,*';
            window.open('frmSuperOrganizationMenu.aspx', '_top');
        }
        else if (p == "frmMainOld.htm") {
            //top.document.all('TopFrame').cols = '0,*';
            //window.open('frmMainOld.htm', '_top');
            window.parent.location = p;
        }
        else
            document.getElementById('main').src = p;

        //close all drop down menu
        $('.dropmenu').slideUp();
    }
	
	function gotoMobile()
	{
		window.top.location.href = '/gotomobile.aspx';
	}

    function gotoLite(p) {
        $.ajax({
            url: 'clearsession.aspx',
            success: function (data) {
                window.open(p, '_top');
            }
        });       
                 
    }

    $(document).ready(function () {
        f = document.getElementById('main');
        var h = $(window).height();
        //f.style.height = (document.body.scrollHeight - menuHeight) + "px";
        //originalHeight = document.body.scrollHeight - menuHeight;
		f.style.height = (h - menuHeight) + "px";
        originalHeight = h - menuHeight;

        $('.drop_link').mouseleave(function () {
            closeDropdownMenu = true;
            var id = $(this).parent().attr('id') + '_dropdown';
            setTimeout(hidedropdownmenu, 100)
        });


        $('.drop_link').mouseenter(function () {
            var moid = $(this).parent().attr('id') + '_dropdown';
            if (dropdowmenuId != moid) return;
            closeDropdownMenu = false;
        });

        $('.dropmenu').mouseover(function () {
            closeDropdownMenu = false;
        });

        $('.dropmenu').mouseout(function () {
            closeDropdownMenu = true;
            var id = $(this).attr('id');
            setTimeout(hidedropdownmenu, 100)
        });

    });

    function gotoDropdown(o) {
        var id = $(o).parent().attr('id');
        $('#' + id + '_dropdown').css('left', $('#' + id).offset().left - ($('#' + id).hasClass('gear') ? 100 : 0)).css('top', $('#' + id).offset().top + menuHeight + 1).toggle();
        dropdowmenuId = $('#' + id + '_dropdown').is(":visible") ? $(o).parent().attr('id') + '_dropdown' : '';
    }

    
    function hidedropdownmenu() {

        if (dropdowmenuId == '') return;
        if (closeDropdownMenu)
            $('#' + dropdowmenuId).hide();
    }

    function Contact() {
        var mypage = 'frmContactUs.aspx';
        var myname = 'ContactUs';
        var w = 300;
        var h = 200;
        var winl = (screen.width - w) / 2;
        var wint = (screen.height - h) / 2;
        winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
        win = window.open(mypage, myname, winprops)
        if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
    }

    function togglemenu() {
        if (menuExpanded) {
            $('#mainmenu').slideUp("slow", function () {
                    f = document.getElementById('main');
                    f.style.height = (originalHeight + menuHeight) + "px";
                  }
            );
            
            menuExpanded = false;
        }
        else {
            $('#mainmenu').slideDown("slow", function () {
                f = document.getElementById('main');
                f.style.height = (originalHeight) + "px";                
            });
            
            menuExpanded = true;
        }
    }

    function downlandGuide() {
        var mypage = './Help/Guide de l\'usager Sentinel v.3.pdf';
        var myname = '';
        var w = 1000;
        var h = screen.height - 100;
        var winl = (screen.width - w) / 2;
        var wint = (screen.height - h) / 2;
        winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + ',location=0,status=0,scrollbars=0,toolbar=1,menubar=1'
        win = window.open(mypage, myname, winprops)
        if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
    }

</script>