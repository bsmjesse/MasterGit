<%@ Page Language="c#" Inherits="SentinelFM.frmVehicleMap" CodeFile="frmVehicleMap.aspx.cs"
    Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title>frmVehicleMap</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script src="../Scripts/prototype.js" type="text/javascript"></script>

    <script src="../Scripts/map.js?id=20100323" type="text/javascript"></script>

    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <style type="text/css">
        #cache
        {
            z-index: 10;
            left: 10px;
            visibility: hidden;
            position: absolute;
            top: 200px;
        }
    </style>
    <style type="text/css">
        .trans
        {
            filter: alpha(opacity=50);
            background-color: whitesmoke;
            -moz-opacity: 0.5;
            opacity: 0.5;
        }
        .id
        {
            font-weight: bold;
            font-size: 9px;
            color: white;
            font-family: verdana,atial,helvetica,san-serif;
        }
        .deadlink
        {
            padding-right: 2px;
            padding-left: 2px;
            font-size: 10px;
            color: #000000;
            font-family: arial,helvetica,san-serif;
            text-decoration: none;
        }
        .skin0
        {
            border-right: black 1px solid;
            border-top: black 1px solid;
            font-size: 11px;
            z-index: 120;
            visibility: hidden;
            border-left: black 1px solid;
            width: 155px;
            cursor: default;
            line-height: 20px;
            border-bottom: black 1px solid;
            font-family: Arial, Helvetica, sans-serif;
            position: absolute;
            background-color: menu;
        }
        .menuitems
        {
            padding-right: 5px;
            padding-left: 5px;
        }
    </style>

    <script language="JavaScript">
<!--

    var vm = null;


        function ResizeMap() {
            parent.frmVehicleMap.location.href = "frmVehicleMap.aspx?clientWidth=" + document.body.clientWidth + "&clientHeight=" + document.body.clientHeight;
            parent.frmFleetInfo.location.href = "frmFleetInfoNew.aspx?clientWidth=" + document.body.clientWidth + "&clientHeight=" + document.body.clientHeight;
        }


        ns = (document.layers) ? true : false
        ie = (document.all) ? true : false


        var sImageName = "imgMap";
        var imWidth = "<%=imageW%>" + 6;

        var oImageInfo = { x: -1, y: -1, w: -1, h: -1 };
        var oSelectInfo = { x1: -1, y1: -1, x2: -1, y2: -1 };
        var bDown = false;
        var bMustSetValues = false
        var mouseX = 0;
        var mouseY = 0;



        function findObjectPosition(oTmp) {
            var oPosition = { x: 0, y: 0 };
            while (oTmp.offsetParent) {
                oPosition.x += oTmp.offsetLeft
                oPosition.y += oTmp.offsetTop
                oTmp = oTmp.offsetParent;
            }
            return oPosition;
        }

        /*

function getMouse(e){
        if (ie)
        {
        mouseY = event.clientY + document.body.scrollTop;
        mouseX = event.clientX + document.body.scrollLeft;
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
        var oTmp = $("selectLayer");
	
        document.myform.imagex.value = (oSelectInfo.x1 < oSelectInfo.x2 ? oSelectInfo.x1 : oSelectInfo.x2) - oImageInfo.x;
        document.myform.imagey.value = (oSelectInfo.y1 < oSelectInfo.y2 ? oSelectInfo.y1 : oSelectInfo.y2) - oImageInfo.y;
        document.myform.imageW.value = Math.abs(oSelectInfo.x2 - oSelectInfo.x1);
        document.myform.imageH.value = Math.abs(oSelectInfo.y2 - oSelectInfo.y1);

	Element.setStyle(oTmp, {
top: (oSelectInfo.y1 < oSelectInfo.y2 ? oSelectInfo.y1 : oSelectInfo.y2) - (document.all ? 0 : 1),
left:  (oSelectInfo.x1 < oSelectInfo.x2 ? oSelectInfo.x1 : oSelectInfo.x2) - (document.all ? 0 : 1),
width: Math.abs(oSelectInfo.x2 - oSelectInfo.x1),
height: Math.abs(oSelectInfo.y2 - oSelectInfo.y1)
	});
        //oTmp.style.top    = (oSelectInfo.y1 < oSelectInfo.y2 ? oSelectInfo.y1 : oSelectInfo.y2) - (document.all ? 0 : 1);
        //oTmp.style.left   = (oSelectInfo.x1 < oSelectInfo.x2 ? oSelectInfo.x1 : oSelectInfo.x2) - (document.all ? 0 : 1);
        //oTmp.style.width  = Math.abs(oSelectInfo.x2 - oSelectInfo.x1);
        //oTmp.style.height = Math.abs(oSelectInfo.y2 - oSelectInfo.y1);
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
	
	if (ie)
        {
        if (event.button==1)
        parent.frmVehicleMap.location.href="frmVehicleMap.aspx?CoordInX="+document.myform.imagex.value+"&CoordInY="+document.myform.imagey.value+"&CoordEndX="+document.myform.imageEndx.value+"&CoordEndY="+document.myform.imageEndy.value; 
        }
        else
        {
        if (e.which==1)
        parent.frmVehicleMap.location.href="frmVehicleMap.aspx?CoordInX="+document.myform.imagex.value+"&CoordInY="+document.myform.imagey.value+"&CoordEndX="+document.myform.imageEndx.value+"&CoordEndY="+document.myform.imageEndy.value; 
        }	
		
	return true;
        }
        */

        function init() {

            vm = new vMap(window, "frmVehicleMap");

            if (document.getElementById) {
                var oTmp = document.getElementById(sImageName)
                var oPosition = findObjectPosition(oTmp);
                oImageInfo.x = oPosition.x;
                oImageInfo.y = oPosition.y;
                oImageInfo.w = oTmp.width;
                oImageInfo.h = oTmp.height;
                //document.myform.imgMap.onmousedown = down;
                //document.myform.imgMap.onmousemove = move;
                //document.myform.imgMap.onmouseup = up;

                //clear
                document.myform.imagex.value = 0;
                document.myform.imagey.value = 0;
                document.myform.imageW.value = 0;
                document.myform.imageH.value = 0;
                //

            } else {
                alert("Your browser is not campatible with this script")
            }
        }
//-->

    </script>

    <script language="javascript">
		<!--


        function SearchWindow() {
            var mypage = "frmMapSearch.aspx?frm='frmvehiclemap.aspx'"
            var myname = '';
            var w = 580;
            var h = 450;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }





        function AddLandmarkWindow() {
            var mypage = "../GeoZone_Landmarks/frmLandmarkAddShortForm.aspx?FormName=VehicleMap"
            var myname = '';
            var w = 700;
            var h = 350;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }


        function AutoReloadMap() {
            parent.frmVehicleMap.location.href = "frmVehicleMap.aspx?CoordInX=0&CoordInY=0&CoordEndX=0&CoordEndY=0";
        }

        function ReloadMap(CoordX, CoordY) {
            parent.frmVehicleMap.location.href = "frmVehicleMap.aspx?CoordX=" + CoordX + "&CoordY=" + CoordY;
        }

        function MapOptions() {
            var mypage = 'frmMapOptions.aspx'
            var myname = '';
            var w = 230;
            var h = 200;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

		//-->
    </script>

</head>
<body bgcolor="white" onload="init();initToolTips();" style="width: 100%" rightmargin="0">
    <form id="frmVehicleMapForm" method="post" runat="server">
      <table id="Table3" style="z-index: 106; left: 0px; 
            position: absolute; top: 5px; background-color: white" height="369" cellspacing="0"
            cellpadding="0" width="63" bgcolor="white" border="0">
            <tr>
                <td align="center"  valign="top">
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
                                  <asp:ImageButton ID="btnMoveNorth" runat="server" border="0" ImageUrl="~/images/icon_go_up.gif" OnClick="btnMoveNorth_Click" meta:resourcekey="btnMoveNorthResource2" ToolTip="Move map north" /></td>
                               <td style="height: 19px">
                               </td>
                            </tr>
                            <tr>
                               <td>
                                  <asp:ImageButton ID="btnMoveWest" runat="server" border="0" ImageUrl="~/images/icon_go_left.gif" OnClick="btnMoveWest_Click" meta:resourcekey="btnMoveWestResource2" ToolTip="Move map west" /></td>
                               <td style="background-color: white">
                               </td>
                               <td>
                                  <asp:ImageButton ID="btnMoveEast" runat="server" border="0" ImageUrl="~/images/icon_go_right.gif" OnClick="btnMoveEast_Click" meta:resourcekey="btnMoveEastResource2" ToolTip="Move map east" /></td>
                            </tr>
                            <tr>
                               <td>
                               </td>
                               <td>
                                  <asp:ImageButton ID="btnMoveSouth" runat="server" border="0" ImageUrl="~/images/icon_go_down.gif" OnClick="btnMoveSouth_Click" meta:resourcekey="btnMoveSouthResource2" ToolTip="Move map south" CausesValidation="False" EnableViewState="False" /></td>
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
                                           <asp:ImageButton ID="cmdZoomInMain" runat="server" border="0" ImageUrl="../images/plusImg.gif" OnClick="cmdZoomInMain_Click" EnableViewState="False" CausesValidation="False" meta:resourcekey="cmdZoomInMainResource1" ToolTip="Zoom In" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnMaxZoom" runat="server" Height="10px" Width="20px" ForeColor="Silver" OnClick="btnMaxZoom_Click" meta:resourcekey="btnMaxZoomResource2" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnStreetLevel1" runat="server" Height="10px" Width="20px" OnClick="btnStreetLevel1_Click" meta:resourcekey="btnStreetLevel1Resource2" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnStreetLevel2" runat="server" Height="10px" Width="20px" OnClick="btnStreetLevel2_Click" meta:resourcekey="btnStreetLevel2Resource2" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnRegionLevel1" runat="server" Height="10px" Width="20px" OnClick="btnRegionLevel1_Click" meta:resourcekey="btnRegionLevel1Resource2" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px; height: 12px;" align="center">
                                           <asp:Button ID="btnRegionLevel2" runat="server" Height="10px" Width="20px" OnClick="btnRegionLevel2_Click" meta:resourcekey="btnRegionLevel2Resource2" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnCountryLevel" runat="server" Height="10px" Width="20px" OnClick="btnCountryLevel_Click" meta:resourcekey="btnCountryLevelResource2" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:ImageButton ID="cmdZoomOutMain" runat="server" border="0" ImageUrl="../images/minusImg.gif" OnClick="cmdZoomOutMain_Click" EnableViewState="False" CausesValidation="False" meta:resourcekey="cmdZoomOutMainResource1" ToolTip="Zoom Out" /></td>
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
                         <table style="border-right: 1px outset; border-top: 1px outset; border-left: 1px outset;
                            border-bottom: 1px outset">
                            <tr>
                               <td style="width: 100px">
                               </td>
                            </tr>
                            <tr>
                               <td style="width: 100px" align="center">
                                  <asp:ImageButton ID="cmdRecenter" runat="server" border="0" ImageUrl="~/images/rctf.gif" OnClick="cmdRecenter_Click" meta:resourcekey="cmdRecenterResource2" ToolTip="Re-Center" /></td>
                            </tr>
                            <tr>
                               <td style="width: 100px">
                               </td>
                            </tr>
                            <tr>
                               <td style="width: 100px" align="center">
                                  <asp:ImageButton ID="cmdMapOpt" runat="server" border="0" ImageUrl="~/images/mapOpts.gif" OnClientClick="MapOptions()" meta:resourcekey="cmdMapOptResource2" ToolTip="Map Options" CausesValidation="False"  /></td>
                            </tr>
                            <tr>
                               <td align="center" style="width: 100px">
                               </td>
                            </tr>
                            <tr>
                               <td style="width: 100px" align="center">
                                  <img src="../images/resizemp.gif" onclick="ResizeMap()" runat=server   alt="<%$ Resources:ResizeMap %>" id="IMG1"  /></td>
                            </tr>
                         </table>
                      </td>
                   </tr>
                </table>   
                </td>
            </tr>
            
        </table>
    <div class="skin0" id="ie5menu" onmouseover="highlightie5(event)" style="z-index: 135;
        left: 118px; position: absolute; top: 45px" onclick="jumptoie5(event)" onmouseout="lowlightie5(event)"
        display:none>
        <div class="menuitems">
            <asp:LinkButton ID="btnSearch" Text="Search" OnClientClick="SearchWindow();" runat="server"></asp:LinkButton>
        </div>
        <div class="menuitems">
            <asp:LinkButton ID="btnAddLandmark" Text="Add Landmark" OnClientClick="AddLandmarkWindow();"
                runat="server"></asp:LinkButton>
        </div>
        <div class="menuitems" jszoom="True">
            <asp:Label ID="lblMenuZoomIn" runat="server" meta:resourcekey="lblMenuZoomInResource1"
                Text="Zoom In"></asp:Label></div>
        <div class="menuitems" jszoom="False">
            <asp:Label ID="lblMenuZoomOut" runat="server" meta:resourcekey="lblMenuZoomOutResource1"
                Text="Zoom Out"></asp:Label></div>
        <div class="menuitems" jsprintmap="True">
            <asp:Label ID="lblMenuPrintMap" runat="server" meta:resourcekey="lblMenuPrintMapResource1"
                Text="Print Map"></asp:Label></div>
        <hr>
        <div class="menuitems" jscancel="True">
            <asp:Label ID="lblMenuCancel" runat="server" meta:resourcekey="lblMenuCancelResource1"
                Text="Cancel"></asp:Label></div>
    </div>
    </form>
    <form name="myform">
    <table border="0">
        <tr>
            <td style="height: 26px">
                <input type="hidden" size="4" value="-1" id="imagex" name="imagex">
            </td>
            <td style="height: 26px">
                <input type="hidden" size="4" value="-1" id="imagey" name="imagey">
            </td>
            <td style="height: 26px">
                <input type="hidden" size="4" value="-1" id="imageEndx" name="imageEndx">
            </td>
            <td style="height: 26px">
                <input type="hidden" size="4" value="-1" id="imageEndy" name="imageEndy">
            </td>
            <td style="height: 26px">
                <input type="hidden" size="4" value="-1" id="imageW" name="imageW">
            </td>
            <td style="height: 26px">
                <input type="hidden" size="4" value="-1" id="imageH" name="imageH">
            </td>
        </tr>
    </table>
    <div id="toolTipLayer" style="font-weight: normal; font-size: small; z-index: 104;
        visibility: hidden; width: 1px; color: gray; font-family: Tahoma, Arial; position: absolute;
        height: 17px">
    </div>
    <div id="rubberBandLayer" style="z-index: 102; left: 75px; position: absolute; top: 5px;
        height: 331px; width: 325px; background-image: url(../images/trans.png); background-repeat: repeat;">
    </div>
    <img id="imgMap" style="border-right: gray 3px double; border-top: gray 3px double;
        z-index: 101; left: 75px; border-left: gray 3px double; border-bottom: gray 3px double;
        position: absolute; top: 5px; height: 325px" height="325px" src="<%=sn.Map.ImgPath%>"
        width="<%=imageW%>" usemap="#locations" border="0">
    </form>
    <div class="trans" id="selectLayer" style="border-right: black 1px dashed; border-top: black 1px dashed;
        font-size: 0px; z-index: 103; visibility: hidden; border-left: black 1px dashed;
        width: 3px; border-bottom: black 1px dashed; position: absolute; height: 2px">
    </div>
     <script language="javascript" type="text/javascript">
				<!--
        //if (ie)
        //	document.body.onmousedown=new Function("if (event.button==2||event.button==3)alert('The right mouse button has been disabled')")
        if (ns)
            document.body.onmousedown = new Function("if (event.which==2||event.which==3)alert('The right mouse button has been disabled')")							
				-->
    </script>

    <script language="JavaScript"><!--
        var textArray = new Array("<%=sn.Map.VehiclesToolTip %>");
        var locationMap = {};
        var toolTipVisible = false;

        var ns4 = document.layers;
        var ns6 = document.getElementById && !document.all;
        var ie4 = document.all;
        offsetX = 0;
        offsetY = 20;
        var toolTipSTYLE = "";
        function initToolTips() {
            if (ns4 || ns6 || ie4) {
                if (ns4) toolTipSTYLE = document.toolTipLayer;
                else if (ns6) toolTipSTYLE = document.getElementById("toolTipLayer").style;
                else if (ie4) toolTipSTYLE = document.all.toolTipLayer.style;
                if (ns4) {
                    document.captureEvents(Event.MOUSEMOVE);
                } else {
                    toolTipSTYLE.visibility = "visible";
                    toolTipSTYLE.display = "none";
                }
                Event.observe($("rubberBandLayer"), "mousemove", moveToMouseLoc);
            }
        }

        function hide() {
            Element.setStyle($(document.body), { cursor: "default" });
            toolTipVisible = false;
            if (ns4) {
                toolTipSTYLE.visibility = "hidden";
            } else {
                toolTipSTYLE.display = "none";
            }
        }

        function toolTip(indexList) {
            toolTipVisible = true;
            Element.setStyle($(document.body), { cursor: "crosshair" });
            fg = "black";
            bg = "white";
            var content =
            '<table border="1" cellspacing="0" cellpadding="1" bgcolor="' + bg + '" width="270" bordercolor="black"><tr>' +
            '<td align="left" ><font face="Arial, Helvetica, sans-serif" color="' + fg +
            '" size=1px >';
            var indexArray = indexList.split('|');
            for (var i = 0; i < indexList.length; i++) {
                if (indexArray[i] != null) {
                    content += textArray[indexArray[i] - 1];
                    if (indexList.length > 1)
                        content += '<hr>';
                }
            }
            content += '</font></td></tr></table>';
            if (ns4) {
                toolTipSTYLE.document.write(content);
                toolTipSTYLE.document.close();
                toolTipSTYLE.visibility = "visible";
            }
            if (ns6) {
                document.getElementById("toolTipLayer").innerHTML = content;
                toolTipSTYLE.display = 'block'
            }
            if (ie4) {
                document.all("toolTipLayer").innerHTML = content;
                toolTipSTYLE.display = 'block'
            }

        }
        function moveToMouseLoc(e) {
            var x = e.clientX;
            var y = e.clientY;

            var element = e.element();
            var pos = element.positionedOffset();
            x -= pos[0];
            y -= pos[1];
x -= 8;
y -= 8;

            toolTipSTYLE.left = x + offsetX;
            toolTipSTYLE.top = y + offsetY;
            hitTest(x, y);
            return true;
        }

        function hitTest(x, y) {
            if (vm.bDown) {
                hide();    
                return;
            }
            var found = false;
            $H(locationMap).each(function(item) {
                if (x > item.value[0] && x < item.value[2] && y > item.value[1] && y < item.value[3]) {
                    found = true;
                    toolTip(item.key);
                }
            });
            if (!found & toolTipVisible)
                hide();
        }

			
			
				
			//--></script>

    <%=sn.Map.VehiclesMappings%>
    <map name="locations">
        <!--%=sn.Map.VehiclesMappings%//-->
    </map>

    <script language="JavaScript">

        //set this variable to 1 if you wish the URLs of the highlighted menu to be displayed in the status bar
        var display_url = 0

        var ie5 = document.all && document.getElementById
        var ns6 = document.getElementById && !document.all
        if (ie5 || ns6)
            var menuobj = document.getElementById("ie5menu")

        function showmenuie5(e) {
            //Find out how close the mouse is to the corner of the window
            var rightedge = ie5 ? document.body.clientWidth - event.clientX : window.innerWidth - e.clientX
            var bottomedge = ie5 ? document.body.clientHeight - event.clientY : window.innerHeight - e.clientY

            //if the horizontal distance isn't enough to accomodate the width of the context menu
            if (rightedge < menuobj.offsetWidth)
            //move the horizontal position of the menu to the left by it's width
                menuobj.style.left = ie5 ? document.body.scrollLeft + event.clientX - menuobj.offsetWidth : window.pageXOffset + e.clientX - menuobj.offsetWidth
            else
            //position the horizontal position of the menu where the mouse was clicked
                menuobj.style.left = ie5 ? document.body.scrollLeft + event.clientX : window.pageXOffset + e.clientX

            //same concept with the vertical position
            if (bottomedge < menuobj.offsetHeight)
                menuobj.style.top = ie5 ? document.body.scrollTop + event.clientY - menuobj.offsetHeight : window.pageYOffset + e.clientY - menuobj.offsetHeight
            else
                menuobj.style.top = ie5 ? document.body.scrollTop + event.clientY : window.pageYOffset + e.clientY

            menuobj.style.visibility = "visible"
            return false
        }

        function hidemenuie5(e) {
            menuobj.style.visibility = "hidden"
        }

        function highlightie5(e) {
            var firingobj = ie5 ? event.srcElement : e.target
            if (firingobj.className == "menuitems" || ns6 && firingobj.parentNode.className == "menuitems") {
                if (ns6 && firingobj.parentNode.className == "menuitems") firingobj = firingobj.parentNode //up one node
                firingobj.style.backgroundColor = "highlight"
                firingobj.style.color = "white"
                if (display_url == 1)
                    window.status = event.srcElement.url
            }
        }

        function lowlightie5(e) {
            var firingobj = ie5 ? event.srcElement : e.target
            if (firingobj.className == "menuitems" || ns6 && firingobj.parentNode.className == "menuitems") {
                if (ns6 && firingobj.parentNode.className == "menuitems") firingobj = firingobj.parentNode //up one node
                firingobj.style.backgroundColor = ""
                firingobj.style.color = "black"
                window.status = ''
            }
        }

        function jumptoie5(e) {
            var firingobj = ie5 ? event.srcElement : e.target
            if (firingobj.className == "menuitems" || ns6 && firingobj.parentNode.className == "menuitems") {
                if (ns6 && firingobj.parentNode.className == "menuitems") firingobj = firingobj.parentNode
                if (firingobj.getAttribute("jsZoom")) {
                    parent.frmVehicleMap.location.href = "frmVehicleMap.aspx?ZoomIn=" + firingobj.getAttribute("jsZoom");
                }

                if (firingobj.getAttribute("jsPrintMap")) {
                    hidemenuie5();
                    window.print();
                }

                if (firingobj.getAttribute("jsCancel")) {
                    hidemenuie5();
                }


                //else if (firingobj.getAttribute("target"))
                //		window.open(firingobj.getAttribute("url"),firingobj.getAttribute("target"))
                //	else (firingobj.getAttribute("target"))
                //		window.location=firingobj.getAttribute("url")

            }
        }

        if (ie5 || ns6) {
            menuobj.style.display = ''
            document.oncontextmenu = showmenuie5
            document.onclick = hidemenuie5
        }

    </script>

</body>
</html>
