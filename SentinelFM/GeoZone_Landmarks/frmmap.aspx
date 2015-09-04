<%@ Page Language="c#" Inherits="SentinelFM.Map_Geo_Land_Zone.frmMap" CodeFile="frmMap.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Map</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <style type="text/css">#cache { Z-INDEX: 10; LEFT: 10px; VISIBILITY: hidden; POSITION: absolute; TOP: 200px }
		</style>
    <style type="text/css">.trans { BORDER-RIGHT: black 1px dashed; BORDER-TOP: black 1px dashed; FONT-SIZE: 0px; Z-INDEX: 105; FILTER: alpha(opacity=50); VISIBILITY: hidden; BORDER-LEFT: black 1px dashed; BORDER-BOTTOM: black 1px dashed; POSITION: absolute; BACKGROUND-COLOR: whitesmoke; moz-opacity: 0.5 }
	.geozone { BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; FONT-SIZE: 0px; Z-INDEX: 105; FILTER: alpha(opacity=50); VISIBILITY: hidden; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid; POSITION: absolute; BACKGROUND-COLOR: #c0e7ff; moz-opacity: 0.5 }
	.id { FONT-WEIGHT: bold; FONT-SIZE: 9px; COLOR: white; FONT-FAMILY: verdana,atial,helvetica,san-serif }
	.deadlink { PADDING-RIGHT: 2px; PADDING-LEFT: 2px; FONT-SIZE: 10px; COLOR: #000000; FONT-FAMILY: arial,helvetica,san-serif; TEXT-DECORATION: none }
	.skin0 { BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; FONT-SIZE: 11px; Z-INDEX: 120; VISIBILITY: hidden; BORDER-LEFT: black 1px solid; WIDTH: 155px; CURSOR: default; LINE-HEIGHT: 20px; BORDER-BOTTOM: black 1px solid; FONT-FAMILY: Arial, Helvetica, sans-serif; POSITION: absolute; BACKGROUND-COLOR: menu }
	.menuitems { PADDING-RIGHT: 5px; PADDING-LEFT: 5px }
		</style>

    <script language="JavaScript">
		<!--
		
		
		

		ns = (document.layers)? true:false
		ie = (document.all)? true:false

		var sImageName = "imgMap";

		var oImageInfo     = { x : -1, y : -1, w : -1, h : -1 };
		var oSelectInfo    = { x1 : -1, y1 : -1, x2 : -1, y2 : -1 };
		var bDown          = false;
		var bMustSetValues = false
		var mouseX=0;
		var mouseY=0;

		function findObjectPosition(oTmp){
			var oPosition = { x : 0, y : 0 };
		while (oTmp.offsetParent){
				oPosition.x += oTmp.offsetLeft
				oPosition.y += oTmp.offsetTop
				oTmp = oTmp.offsetParent;
			}
			return oPosition;
		}

		function getMouse(e){
		if (ie)
			{
			//	mouseY = event.clientY + document.body.scrollTop;
			//	mouseX = event.clientX + document.body.scrollLeft;
				mouseY = event.clientY ;
				mouseX = event.clientX ;
			}


		if (ns)
			{
				mouseY = e.pageY;
				mouseX = e.pageX;
			}
		}	
		function showSelect() {
		document.getElementById("selectLayer").style.visibility="visible";
		}

		function hideSelect() {
		document.getElementById("selectLayer").style.visibility="hidden";
		}

		function redrawSelect() {
		var oTmp = document.getElementById("selectLayer");
			document.myform.imagex.value = (oSelectInfo.x1 < oSelectInfo.x2 ? oSelectInfo.x1 : oSelectInfo.x2) - oImageInfo.x;
			document.myform.imagey.value = (oSelectInfo.y1 < oSelectInfo.y2 ? oSelectInfo.y1 : oSelectInfo.y2) - oImageInfo.y;
			document.myform.imageW.value = Math.abs(oSelectInfo.x2 - oSelectInfo.x1);
			document.myform.imageH.value = Math.abs(oSelectInfo.y2 - oSelectInfo.y1);
		oTmp.style.top    = (oSelectInfo.y1 < oSelectInfo.y2 ? oSelectInfo.y1 : oSelectInfo.y2) - (document.all ? 0 : 1);
		oTmp.style.left   = (oSelectInfo.x1 < oSelectInfo.x2 ? oSelectInfo.x1 : oSelectInfo.x2) - (document.all ? 0 : 1);
		oTmp.style.width  = Math.abs(oSelectInfo.x2 - oSelectInfo.x1);
		oTmp.style.height = Math.abs(oSelectInfo.y2 - oSelectInfo.y1);
		}

		function down(e){
		//clear
		document.myform.imgMap.onmousemove=move;
		document.myform.imgMap.useMap=null;

		 
		document.myform.imagex.value=0;
		document.myform.imagey.value=0;
		document.myform.imageW.value=0;
		document.myform.imageH.value=0;
		//

			

			if (ie)
			{
				document.myform.imagex.value=event.offsetX;
				document.myform.imagey.value=event.offsetY;
			}
			
			if (ns)
			{
				document.myform.imagex.value=e.layerX;
				document.myform.imagey.value =e.layerY;
			}



			if((document.layers && e.which!=1) || (document.all && event.button!=1)) return true;
			bDown = true;
			bMustSetValues = true;
			getMouse(e);
			if (mouseX < oImageInfo.x) mouseX = oImageInfo.x;
			if (mouseX > (oImageInfo.x + oImageInfo.w)) mouseX = (oImageInfo.x + oImageInfo.w);
			if (mouseY < oImageInfo.y) mouseY = oImageInfo.y;
			if (mouseY > (oImageInfo.y + oImageInfo.h)) mouseY = (oImageInfo.y + oImageInfo.h);
		return false;
		}

		function move(e){
		if (bDown) {
			if (bMustSetValues) {
			oSelectInfo.x1 = mouseX;
				oSelectInfo.y1 = mouseY;
				bMustSetValues = false;
			}
			getMouse(e);
			if (mouseX < oImageInfo.x) mouseX = oImageInfo.x;
			if (mouseX > (oImageInfo.x + oImageInfo.w)) mouseX = (oImageInfo.x + oImageInfo.w);
			if (mouseY < oImageInfo.y) mouseY = oImageInfo.y;
			if (mouseY > (oImageInfo.y + oImageInfo.h)) mouseY = (oImageInfo.y + oImageInfo.h);
				oSelectInfo.x2 = mouseX;
 				oSelectInfo.y2 = mouseY;
 				redrawSelect();
 				showSelect();
		}
			if(document.all) return false;
		}
				
		
		function up(e){

			//check for right click
			if (typeof(mouseX)=="undefined")
			{
				return false;
			}
		//


		bDown = false;
		var oTmp = { x : mouseX, y : mouseY};
		getMouse(e);
			if (mouseX == oTmp.x && mouseY == oTmp.y) hideSelect;
			
			document.myform.imageEndx.value =parseFloat(document.myform.imagex.value)+parseFloat(document.myform.imageW.value);
			document.myform.imageEndy.value =parseFloat(document.myform.imagey.value)+parseFloat(document.myform.imageH.value); 
			
			if (event.button==1)
				window.location.href="frmMap.aspx?CoordInX="+document.myform.imagex.value+"&CoordInY="+document.myform.imagey.value+"&CoordEndX="+document.myform.imageEndx.value+"&CoordEndY="+document.myform.imageEndy.value; 
				 
				
			return true;
		}

		function init() {
		if (document.getElementById) {
			var oTmp = document.getElementById(sImageName)
			var oPosition = findObjectPosition(oTmp);
			oImageInfo.x = oPosition.x;
			oImageInfo.y = oPosition.y;
			oImageInfo.w = oTmp.width;
			oImageInfo.h = oTmp.height;
  			document.myform.imgMap.onmousedown = down;
  			//document.myform.imgMap.onmousemove = move;
  			document.myform.imgMap.onmouseup = up;
		  	
  			//clear
			document.myform.imagex.value=0;
			document.myform.imagey.value=0;
			document.myform.imageW.value=0;
			document.myform.imageH.value=0;
			//
		  	
		} else {
			alert("Your browser is not campatible with this script")
		}
		}
		//-->
    </script>

    <script language="javascript">
				<!--

				

				ver = navigator.appVersion.substring(0,1)
				if (ie)
    			{
    			//document.write('<DIV ID="cache" style="Z-INDEX: 104; LEFT: 500px; POSITION: absolute; TOP: 150px"><TABLE WIDTH=500 BGCOLOR=black BORDER=0 CELLPADDING=2 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><TABLE WIDTH=100% BGCOLOR=#FFFFFF BORDER=0 CELLPADDING=0 CELLSPACING=0><TR><TD ALIGN=center VALIGN=middle><FONT FACE="Arial, Verdana" SIZE=4 color="black"><B><BR>PLEASE WAIT ... <BR><BR></B></FONT></TD> </TR></TABLE></TD> </TR></TABLE></DIV>');
    			var navi = (navigator.appName == "Netscape" && parseInt(navigator.appVersion) >= 4);
    			var HIDDEN = (navi) ? 'hide' : 'hidden';
    			var VISIBLE = (navi) ? 'show' : 'visible';
    			///var cache = (navi) ? document.cache : document.all.cache.style;
    			largeur = screen.width;
    			//cache.left = Math.round(100);
    			//cache.visibility = VISIBLE;
    			}
			function cacheOff()
    			{
    			if (ie)
    			{
    				if (ver >= 4)
    					{
    					cache.visibility = HIDDEN;
    					}
    				}
    			}
		    	
		    	
    			function AutoReloadMap()
    			{
    				window.location.href="frmMap.aspx?CoordInX=0&CoordInY=0&CoordEndX=0&CoordEndY=0" ; 
    			}
		    	
    			function ReloadMap(CoordX,CoordY)
    			{
    				window.location.href="frmMap.aspx?CoordX="+CoordX+"&CoordY="+CoordY ; 
    			}
		    
    			function MapOptions() 
    			{ 
							var mypage='frmMapOptions.aspx'
							var myname='';
							var w=220;
							var h=200;
							var winl = (screen.width - w) / 2; 
							var wint = (screen.height - h) / 2; 
							winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
							win = window.open(mypage, myname, winprops) 
							if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
						}

				//-->
    </script>

</head>
<body onload="init();">
    <form id="frmMapForm" method="post" runat="server">
        <table id="Table1" style="z-index: 102; left: 0px; position: absolute; top: 0px">
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
        </table>
        <asp:Button ID="cmdCancel" Style="z-index: 106; left: 640px; position: absolute;
            top: 392px" runat="server" CssClass="combutton" Text="Close" OnClick="cmdCancel_Click"
            meta:resourcekey="cmdCancelResource1"></asp:Button>
        <asp:Button ID="cmdSave" Style="z-index: 103; left: 527px; position: absolute; top: 392px"
            runat="server" CssClass="combutton" Text="Save" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1">
        </asp:Button>
        <table id="tblGeoZone" style="z-index: 110; left: 92px; width: 311px; position: absolute;
            top: 387px; height: 14px" cellspacing="0" cellpadding="0" width="311" border="0"
            runat="server">
            <tr>
                <td class="formtext" style="width: 308px; height: 5px" height="5">
                    <p>
                        <table id="Table6" style="width: 301px; height: 47px" cellspacing="1" cellpadding="1"
                            width="301" border="0">
                            <tr>
                                <td class="formtext" style="height: 11px">
                                    <asp:Label ID="lblMapToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMapToTitleResource1">Map To:</asp:Label>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="cboLandmarks" runat="server" CssClass="formtext" DataTextField="LandmarkName"
                                        DataValueField="LandmarkName" AutoPostBack="True" Width="294px" OnSelectedIndexChanged="cboLandmarks_SelectedIndexChanged"
                                        meta:resourcekey="cboLandmarksResource1">
                                    </asp:DropDownList></td>
                            </tr>
                        </table>
                    </p>
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 308px">
                    <table id="Table4" cellspacing="1" cellpadding="1" width="300" border="0">
                        <tr>
                            <td>
                                <table id="Table7" style="width: 198px; height: 60px" cellspacing="1" cellpadding="1"
                                    width="198" border="0">
                                    <tr>
                                        <td>
                                            <asp:Button ID="cmdDrawGeoZone" runat="server" CssClass="combutton" Text="Start Drawing"
                                                Width="189px" Height="22px" OnClick="cmdDrawGeoZone_Click" meta:resourcekey="cmdDrawGeoZoneResource1">
                                            </asp:Button></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Button ID="cmdClearMap" runat="server" CssClass="combutton" Text="Clear Map"
                                                Width="189px" OnClick="cmdClearMap_Click" meta:resourcekey="cmdClearMapResource1">
                                            </asp:Button></td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <asp:RadioButtonList ID="GeoZoneOptions" runat="server" CssClass="formtext" AutoPostBack="True"
                                    Width="83px" Height="48px" BorderWidth="1px" OnSelectedIndexChanged="GeoZoneOptions_SelectedIndexChanged"
                                    meta:resourcekey="GeoZoneOptionsResource1">
                                    <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource1">Rectangle</asp:ListItem>
                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource2">Polygon</asp:ListItem>
                                </asp:RadioButtonList></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 308px" height="30">
                    <asp:Label ID="lblAddGeoZoneMsg" runat="server" Visible="False" meta:resourcekey="lblAddGeoZoneMsgResource1">Navigate map to geozone poition.</asp:Label></td>
            </tr>
        </table>
        <table id="Table2" style="z-index: 111; left: 5px; position: absolute; top: 5px;"  height="370" cellspacing="0"
            cellpadding="0" width="63" bgcolor="white" border="0">
            <tr>
                <td style="height: 8px" align="center" background="verticalgray">
                
                 <table>
                   <tr>
                      <td align="center" valign=top   style="width: 53px">
                                  <table style="border-right: 1px outset; border-top: 1px outset; border-left: 1px outset; border-bottom: 1px outset">
                                     <tr>
                                        <td style="width: 61px">
                         <table border="0" cellpadding="0" cellspacing="0">
                            <tr>
                               <td style="height: 19px">
                               </td>
                               <td style="height: 19px">
                                  <asp:ImageButton ID="btnMoveNorth" runat="server" border="0" ImageUrl="~/images/icon_go_up.gif" OnClick="btnMoveNorth_Click" /></td>
                               <td style="height: 19px">
                               </td>
                            </tr>
                            <tr>
                               <td>
                                  <asp:ImageButton ID="btnMoveWest" runat="server" border="0" ImageUrl="~/images/icon_go_left.gif" OnClick="btnMoveWest_Click" /></td>
                               <td style="background-color: white">
                               </td>
                               <td>
                                  <asp:ImageButton ID="btnMoveEast" runat="server" border="0" ImageUrl="~/images/icon_go_right.gif" OnClick="btnMoveEast_Click" /></td>
                            </tr>
                            <tr>
                               <td>
                               </td>
                               <td>
                                  <asp:ImageButton ID="btnMoveSouth" runat="server" border="0" ImageUrl="~/images/icon_go_down.gif" OnClick="btnMoveSouth_Click" /></td>
                               <td>
                               </td>
                            </tr>
                         </table>
                                        </td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px">
                                        </td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:ImageButton ID="btnZoomIn" runat="server" border="0" ImageUrl="../images/plusImg.gif" OnClick="btnZoomIn_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnMaxZoom" runat="server" Height="10px" Width="20px" ForeColor="Silver" OnClick="btnMaxZoom_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnStreetLevel1" runat="server" Height="10px" Width="20px" OnClick="btnStreetLevel1_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnStreetLevel2" runat="server" Height="10px" Width="20px" OnClick="btnStreetLevel2_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnRegionLevel1" runat="server" Height="10px" Width="20px" OnClick="btnRegionLevel1_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px; height: 12px;" align="center">
                                           <asp:Button ID="btnRegionLevel2" runat="server" Height="10px" Width="20px" OnClick="btnRegionLevel2_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnCountryLevel" runat="server" Height="10px" Width="20px" OnClick="btnCountryLevel_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:ImageButton ID="btnZoomOut" runat="server" border="0" ImageUrl="../images/minusImg.gif" OnClick="btnZoomOut_Click" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px; height: 21px">
                                        </td>
                                     </tr>
                                  </table>
                      </td>
                   </tr>
                   <tr>
                      <td style="width: 53px">
                      </td>
                   </tr>
                   <tr>
                      <td align="center" style="width: 53px; height: 21px;">
                      </td>
                   </tr>
                </table>   
                 
                </td>
            </tr>
            <tr>
                <td align="center" background="verticalgray">
                </td>
            </tr>
        </table>
        <asp:Button ID="cmdShowEditGeoZone" Style="z-index: 115; left: 412px; position: absolute;
            top: 392px" runat="server" CssClass="combutton" Text="Edit GeoZone" OnClick="cmdShowEditGeoZone_Click"
            meta:resourcekey="cmdShowEditGeoZoneResource1"></asp:Button>
        <asp:Label ID="lblMessage" Style="z-index: 114; left: 457px; position: absolute;
            top: 434px" runat="server" CssClass="errortext" Width="273px" Height="73px" Font-Bold="True"
            meta:resourcekey="lblMessageResource1"></asp:Label>
       &nbsp; &nbsp;&nbsp;
    </form>
    <form name="myform">
        <div id="toolTipLayer" style="font-weight: normal; font-size: small; z-index: 113;
            visibility: hidden; color: black; font-family: Tahoma, Arial; position: absolute">
        </div>
        <img id="imgMap" style="border-right: gray 3px double; border-top: gray 3px double;
            z-index: 101; left: 90px; border-left: gray 3px double; border-bottom: gray 3px double;
            position: absolute; top: 5px" height="361" src="<%=sn.GeoZone.ImgConfPath%>"
            width="655" usemap="#locations" border="0">
        <table id="Table3" border="0">
            <tr>
                <td>
                    <input type="hidden" size="4" value="-1" name="imagex"></td>
                <td>
                    <input type="hidden" size="4" value="-1" name="imagey"></td>
                <td>
                    <input type="hidden" size="4" value="-1" name="imageEndx"></td>
                <td>
                    <input type="hidden" size="4" value="-1" name="imageEndy"></td>
                <td>
                    <input type="hidden" size="4" value="-1" name="imageW"></td>
                <td>
                    <input type="hidden" size="4" value="-1" name="imageH"></td>
            </tr>
        </table>
    </form>
    <div class="<%=sn.GeoZone.SelectLayer%>" id="selectLayer" style="border-right: black 1px dashed;
        border-top: black 1px dashed; font-size: 0px; z-index: 112; visibility: hidden;
        border-left: black 1px dashed; border-bottom: black 1px dashed; position: absolute">
    </div>

    <script language="javascript" type="text/javascript">
				<!--
						//if (ie)
						//	document.body.onmousedown=new Function("if (event.button==2||event.button==3)alert('The right mouse button has been disabled')")
						if (ns)
							document.body.onmousedown=new Function("if (event.which==2||event.which==3)alert('The right mouse button has been disabled')")							
				-->
    </script>

    <div class="skin0" id="ie5menu" onmouseover="highlightie5(event)" style="z-index: 109;
        left: 166px; top: 21px" onclick="jumptoie5(event)" onmouseout="lowlightie5(event)"
        display:none>
        <div class="menuitems" jszoom="True">
            <asp:Label ID="lblMenuZoomIn" runat="server" meta:resourcekey="lblMenuZoomInResource1">Zoom In</asp:Label></div>
        <div class="menuitems" jszoom="False">
            <asp:Label ID="lblMenuZoomOut" runat="server" meta:resourcekey="lblMenuZoomOutResource1">Zoom Out</asp:Label></div>
        <div class="menuitems" jsprintmap="True">
            <asp:Label ID="lblMenuPrintMap" runat="server" meta:resourcekey="lblMenuPrintMapResource1">Print Map</asp:Label></div>
        <hr>
        <div class="menuitems" jscancel="True">
            <asp:Label ID="lblMenuCancel" runat="server" meta:resourcekey="lblMenuCancelResource1">Cancel</asp:Label></div>
    </div>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>

    <script language="JavaScript">

			//set this variable to 1 if you wish the URLs of the highlighted menu to be displayed in the status bar
			var display_url=0

			var ie5=document.all&&document.getElementById
			var ns6=document.getElementById&&!document.all
			if (ie5||ns6)
			var menuobj=document.getElementById("ie5menu")

			function showmenuie5(e){
			//Find out how close the mouse is to the corner of the window
			var rightedge=ie5? document.body.clientWidth-event.clientX : window.innerWidth-e.clientX
			var bottomedge=ie5? document.body.clientHeight-event.clientY : window.innerHeight-e.clientY

			//if the horizontal distance isn't enough to accomodate the width of the context menu
			if (rightedge<menuobj.offsetWidth)
			//move the horizontal position of the menu to the left by it's width
			menuobj.style.left=ie5? document.body.scrollLeft+event.clientX-menuobj.offsetWidth : window.pageXOffset+e.clientX-menuobj.offsetWidth
			else
			//position the horizontal position of the menu where the mouse was clicked
			menuobj.style.left=ie5? document.body.scrollLeft+event.clientX : window.pageXOffset+e.clientX

			//same concept with the vertical position
			if (bottomedge<menuobj.offsetHeight)
			menuobj.style.top=ie5? document.body.scrollTop+event.clientY-menuobj.offsetHeight : window.pageYOffset+e.clientY-menuobj.offsetHeight
			else
			menuobj.style.top=ie5? document.body.scrollTop+event.clientY : window.pageYOffset+e.clientY

			menuobj.style.visibility="visible"
			return false
			}

			function hidemenuie5(e){
			menuobj.style.visibility="hidden"
			}

			function highlightie5(e)
			{
				var firingobj=ie5? event.srcElement : e.target
				if (firingobj.className=="menuitems"||ns6&&firingobj.parentNode.className=="menuitems")
				{
					if (ns6&&firingobj.parentNode.className=="menuitems") firingobj=firingobj.parentNode //up one node
						firingobj.style.backgroundColor="highlight"
						firingobj.style.color="white"
					if (display_url==1)
						window.status=event.srcElement.url
				}
		    }

			function lowlightie5(e)
			{
				var firingobj=ie5? event.srcElement : e.target
				if (firingobj.className=="menuitems"||ns6&&firingobj.parentNode.className=="menuitems")
				{
					if (ns6&&firingobj.parentNode.className=="menuitems") firingobj=firingobj.parentNode //up one node
					firingobj.style.backgroundColor=""
					firingobj.style.color="black"
					window.status=''
				}
			}

			function jumptoie5(e)
			{
				var firingobj=ie5? event.srcElement : e.target
				if (firingobj.className=="menuitems"||ns6&&firingobj.parentNode.className=="menuitems")
				{
					if (ns6&&firingobj.parentNode.className=="menuitems") firingobj=firingobj.parentNode
					if 	(firingobj.getAttribute("jsZoom"))
					{
						frmMap.location.href="frmMap.aspx?ZoomIn="+firingobj.getAttribute("jsZoom");
					}
					
					if 	(firingobj.getAttribute("jsPrintMap"))
					{
						hidemenuie5();
						window.print(); 
					}
					
					if 	(firingobj.getAttribute("jsCancel"))
					{
						hidemenuie5();
					}
					
					
					//else if (firingobj.getAttribute("target"))
					//		window.open(firingobj.getAttribute("url"),firingobj.getAttribute("target"))
					//	else (firingobj.getAttribute("target"))
					 //		window.location=firingobj.getAttribute("url")
							
				}
			}

			if (ie5||ns6){
			menuobj.style.display=''
			document.oncontextmenu=showmenuie5
			document.onclick=hidemenuie5
			}

    </script>

</body>
</html>
