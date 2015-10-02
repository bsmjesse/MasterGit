<%@ Page Language="c#" Inherits="SentinelFM.frmHist"  CodeFile="frmHist.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>History</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR"/>
    <meta content="C#" name="CODE_LANGUAGE"/>
    <meta content="JavaScript" name="vs_defaultClientScript"/>
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
    
    <style type="text/css">
        .trans { FILTER: alpha(opacity=50); BACKGROUND-COLOR: #dbeaf5; moz-opacity: 0.5 }
	    .id { FONT-WEIGHT: bold; FONT-SIZE: 9px; COLOR: white; FONT-FAMILY: verdana,atial,helvetica,san-serif }
	    .deadlink { PADDING-RIGHT: 2px; PADDING-LEFT: 2px; FONT-SIZE: 10px; COLOR: #000000; FONT-FAMILY: arial,helvetica,san-serif; TEXT-DECORATION: none }
	    .skin0 { BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; FONT-SIZE: 11px; Z-INDEX: 120; VISIBILITY: hidden; BORDER-LEFT: black 1px solid; WIDTH: 155px; CURSOR: default; LINE-HEIGHT: 20px; BORDER-BOTTOM: black 1px solid; FONT-FAMILY: Arial, Helvetica, sans-serif; POSITION: absolute; BACKGROUND-COLOR: menu }
	    .menuitems { PADDING-RIGHT: 5px; PADDING-LEFT: 5px }
	</style>
	
    <script language="JavaScript" type ="text/javascript">
        <!--
        function ResizeMap() {
            parent.frmHis.location.href="frmhist.aspx?clientWidth="+document.body.clientWidth+"&clientHeight="+document.body.clientHeight; 
        }

    	function MapOptions() {
    	    var mypage = 'frmHistoryMapOptions.aspx';
			var myname='';
			var w=230;
			var h=200;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2;
			winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
			win = window.open(mypage, myname, winprops); 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
		}
				
		function StopLegends() {
		    var mypage = 'frmhistorylegend.aspx';
			var myname='';
			var w=230;
			var h=200;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2;
			winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,';
			win = window.open(mypage, myname, winprops); 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
		}

	    function HistoryInfo(dgKey) {
	        var mypage = 'frmHistDetails.aspx?dgKey=' + dgKey;
			var myname='';
			var w=580;
			var h=660;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2;
			winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=yes,';
			win = window.open(mypage, myname, winprops); 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
		}

	    ns = (document.layers)? true:false
	    ie = (document.all)? true:false	

		var sImageName = "imgMap";
		var oImageInfo     = { x : -1, y : -1, w : -1, h : -1 };
		var oSelectInfo    = { x1 : -1, y1 : -1, x2 : -1, y2 : -1 };
		var bDown          = false;
		var bMustSetValues = false

		function findObjectPosition(oTmp) {
			var oPosition = { x : 0, y : 0 };
    		while (oTmp.offsetParent) {
    		    oPosition.x += oTmp.offsetLeft;
    		    oPosition.y += oTmp.offsetTop;
				oTmp = oTmp.offsetParent;
			}
			return oPosition;
		}

		function getMouse(e) {
			if(document.all) {
				mouseY = event.clientY + document.body.scrollTop;
				mouseX = event.clientX + document.body.scrollLeft;
			} else {
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

		function down(e) {
		    //clear
		    document.myform.imagex.value=0;
		    document.myform.imagey.value=0;
		    document.myform.imageW.value=0;
		    document.myform.imageH.value=0;
		    //
			document.myform.imgMap.onmousemove=move;
			document.myform.imgMap.useMap=null;

			document.myform.imagex.value=event.offsetX;
			document.myform.imagey.value=event.offsetY;

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

		function move(e) {
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

		function up(e) {
			//check for right click
			if (typeof(mouseX)=="undefined") {
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
				window.location.href="frmHist.aspx?CoordInX="+document.myform.imagex.value+"&CoordInY="+document.myform.imagey.value+"&CoordEndX="+document.myform.imageEndx.value+"&CoordEndY="+document.myform.imageEndy.value ; 
			
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
			} else {
			    alert("Your browser is not campatible with this script");
			}
		}
        //-->
    </script>
    <script language="javascript" type ="text/javascript">
		<!--
		function ScrollColor() {
		    with(document.body.style) {
				scrollbarDarkShadowColor="003366";
				scrollbar3dLightColor="gray";
				scrollbarArrowColor="gray";
				scrollbarBaseColor="FFFFFF";
				scrollbarFaceColor="FFFFFF";
				scrollbarHighlightColor="gray";
				scrollbarShadowColor="black";
				scrollbarTrackColor="whitesmoke";
			}
		}
		//-->
    </script>
</head>
<body onload="ScrollColor();init();initToolTips();" style="height: 100%;">
    <form id="frmHistory" method="post" runat="server">
        <div id="divHist" style="z-index: 107; left: 10px; overflow: auto; width: 100%; position: absolute; top: 390px; height:<%=divHistoryH%>%">
            <asp:DataGrid ID="dgStops" runat="server" Width="100%" ShowHeader="False" AutoGenerateColumns="False"
                CellPadding="3" AllowPaging="True" PageSize="20" BorderStyle="Ridge" BorderWidth="2px"
                BorderColor="White" BackColor="White" CellSpacing="1" GridLines="None" OnSelectedIndexChanged="dgStops_SelectedIndexChanged" meta:resourcekey="dgStopsResource1">
                <FooterStyle ForeColor="Black" BackColor="#C6C3C6"/>
                <SelectedItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="White" BackColor="#99CC66"/>
                <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"/>
                <ItemStyle CssClass="gridtext" BackColor="White"/>
                <HeaderStyle CssClass="DataHeaderStyle"/>
                <Columns>
                    <asp:BoundColumn DataField="StopIndex" HeaderText="No">
                        <ItemStyle Width="20px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="ArrivalDateTime" HeaderText="Arrival">
                        <ItemStyle Width="160px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Location" HeaderText="Address">
                        <ItemStyle Width="290px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="DepartureDateTime" HeaderText="Departure">
                        <ItemStyle Width="160px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="StopDuration" HeaderText="Duration">
                        <ItemStyle Width="50px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Remarks" HeaderText="Status">
                        <HeaderStyle Width="50px"/>
                        <ItemStyle Width="50px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="Latitude" HeaderText="Latitude"/>
                    <asp:BoundColumn Visible="False" DataField="Longitude" HeaderText="Longitude"/>
                    <asp:BoundColumn Visible="False" DataField="StopDurationVal" HeaderText="StopDurationVal"/>
                    <asp:ButtonColumn Text="Map" CommandName="Select" meta:resourcekey="ButtonColumnResource1"/>
                </Columns>
                <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"/>
            </asp:DataGrid>
            <asp:DataGrid ID="dgHistoryDetails" runat="server" Width="100%" ShowHeader="False" AutoGenerateColumns="False" CellPadding="3" AllowPaging="True" PageSize="100" BorderStyle="Ridge" BorderWidth="2px" 
                BorderColor="White" BackColor="White" CellSpacing="1" GridLines="None" OnSelectedIndexChanged="dgHistoryDetails_SelectedIndexChanged" meta:resourcekey="dgHistoryDetailsResource1">
                <FooterStyle ForeColor="Black" BackColor="#C6C3C6"/>
                <SelectedItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black" BackColor="#99CC66"/>
                <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"/>
                <ItemStyle CssClass="gridtext" BackColor="White"/>
                <HeaderStyle CssClass="DataHeaderStyle"/>
                <Columns>
                    <asp:BoundColumn DataField="MyDateTime" HeaderText="Date/Time">
                        <ItemStyle Wrap="False"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="OriginDateTime" HeaderText="OriginDateTime"/>
                    <asp:BoundColumn DataField="StreetAddress" HeaderText="Address">
                        <ItemStyle Width="250px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Speed" HeaderText="Speed">
                        <ItemStyle Wrap="False" HorizontalAlign="Center"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="Latitude" HeaderText="Latitude"/>
                    <asp:BoundColumn Visible="False" DataField="Longitude" HeaderText="Longitude"/>
                    <asp:BoundColumn Visible="False" DataField="OriginDateTime" HeaderText="OriginDateTime"/>
                    <asp:HyperLinkColumn DataNavigateUrlField="dgKey" DataNavigateUrlFormatString="javascript:var w =HistoryInfo('{0}')" DataTextField="BoxMsgInTypeName" HeaderText="Message Type" meta:resourcekey="HyperLinkColumnResource1">
                        <ItemStyle Wrap="False"/>
                    </asp:HyperLinkColumn>
                    <asp:BoundColumn DataField="MsgDetails" HeaderText="Message">
                        <ItemStyle Width="100px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Acknowledged" HeaderText="Ack">
                        <ItemStyle Width="25px"/>
                    </asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="MyHeading" HeaderText="Heading"/>
                    <asp:ButtonColumn Text="Map" CommandName="Select" meta:resourcekey="ButtonColumnResource2">
                        <ItemStyle Width="10px"/>
                    </asp:ButtonColumn>
                </Columns>
                <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="White" BackColor="#C6C3C6" Mode="NumericPages"/>
            </asp:DataGrid>
        </div>
        <table id="Table1" style="z-index: 111; left: 0px; position: absolute; top: 0px">
            <tr><td></td></tr>
        </table>
        <table id="tblNoData" style="border-right: gray 1px outset; border-top: gray 1px outset;
            z-index: 109; left: 240px; border-left: gray 1px outset; border-bottom: gray 1px outset;
            position: absolute; top: 507px" cellspacing="0" cellpadding="0" width="400" border="0"
            runat="server">
            <tr>
                <td valign="middle" align="center">
                    <table cellspacing="0" cellpadding="0" width="100%" bgcolor="#ffffff" border="0">
                        <tr>
                            <td class="tableheading" valign="middle" align="center">
                                <b>
                                    <br/>
                                    <asp:Label ID="lblNoHistoryMessage" runat="server" CssClass="tableheading" meta:resourcekey="lblNoHistoryMessageResource1" Text="No history data matching the selected criteria."/>
                                    <br/>
                                    <br/>
                                </b>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table id="tblOverflow" style="border-right: gray 1px outset; border-top: gray 1px outset;
            z-index: 103; left: 240px; border-left: gray 1px outset; border-bottom: gray 1px outset;
            position: absolute; top: 469px" cellspacing="0" cellpadding="0" width="400" bgcolor="gray"
            border="0" runat="server">
            <tr>
                <td valign="middle" align="center">
                    <table cellspacing="0" cellpadding="0" width="100%" bgcolor="#ffffff" border="0">
                        <tr>
                            <td style="height: 124px" valign="middle" align="center">
                                <font face="Arial, Verdana" color="gray" size="4">
                                    <b>
                                        <br/>
                                        <asp:Label ID="lblTooManyRecordsMessage1" runat="server" meta:resourcekey="lblTooManyRecordsMessage1Resource1" Text="Too many records requested."/>
                                        <br/>
                                        <br/>
                                        <asp:Label ID="lblTooManyRecordsMessage2" runat="server" meta:resourcekey="lblTooManyRecordsMessage2Resource1" Text="Try to decrease history period ..."/>
                                    </b>
                                    <br/>
                                    <br/>
                                </font>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table id="Table3" style="z-index: 115; left: 0px; position: absolute; top: 5px; background-color: white;height:359" cellspacing="0" cellpadding="0" width="63" bgcolor="white" border="0">
            <tr>
                <td style="height: 8px" align="center" valign="top">
                    <table>
                        <tr>
                            <td align="center" valign=top   style="width: 53px">
                                <table style="border-right: 1px outset; border-top: 1px outset; border-left: 1px outset; border-bottom: 1px outset">
                                    <tr>
                                        <td style="width: 61px">
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td style="height: 19px"></td>
                                                    <td style="height: 19px">
                                                        <asp:ImageButton ID="btnMoveNorth" runat="server" border="0" ImageUrl="~/images/icon_go_up.gif" OnClick="btnMoveNorth_Click" ToolTip="Move map north"/>
                                                    </td>
                                                    <td style="height: 19px"></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:ImageButton ID="btnMoveWest" runat="server" border="0" ImageUrl="~/images/icon_go_left.gif" OnClick="btnMoveWest_Click" ToolTip="Move map west"/>
                                                    </td>
                                                    <td style="background-color: white"></td>
                                                    <td>
                                                        <asp:ImageButton ID="btnMoveEast" runat="server" border="0" ImageUrl="~/images/icon_go_right.gif" OnClick="btnMoveEast_Click" ToolTip="Move map east"/>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:ImageButton ID="btnMoveSouth" runat="server" border="0" ImageUrl="~/images/icon_go_down.gif" OnClick="btnMoveSouth_Click" ToolTip="Move map south"/>
                                                    </td>
                                                    <td></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px"></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:ImageButton ID="btnZoomIn" runat="server" border="0" ImageUrl="../images/plusImg.gif" OnClick="btnZoomIn_Click" ToolTip="Zoom In"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnMaxZoom" runat="server" Height="10px" Width="20px" ForeColor="Silver" OnClick="btnMaxZoom_Click"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnStreetLevel1" runat="server" Height="10px" Width="20px" OnClick="btnStreetLevel1_Click"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnStreetLevel2" runat="server" Height="10px" Width="20px" OnClick="btnStreetLevel2_Click"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:Button ID="btnRegionLevel1" runat="server" Height="10px" Width="20px" OnClick="btnRegionLevel1_Click"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px; height: 12px;" align="center">
                                            <asp:Button ID="btnRegionLevel2" runat="server" Height="10px" Width="20px" OnClick="btnRegionLevel2_Click"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px" align="center">
                                            <asp:Button ID="btnCountryLevel" runat="server" Height="10px" Width="20px" OnClick="btnCountryLevel_Click"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px" align="center">
                                           <asp:ImageButton ID="btnZoomOut" runat="server" border="0" ImageUrl="../images/minusImg.gif" OnClick="btnZoomOut_Click" ToolTip="Zoom Out"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 61px; height: 21px"></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 53px"></td>
                        </tr>
                        <tr>
                            <td align="center" style="width: 53px; height: 21px;">
                                <table style="border-right: 1px outset; border-top: 1px outset; border-left: 1px outset; border-bottom: 1px outset">
                                    <tr>
                                        <td style="width: 100px"></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100px" align="center">
                                            <asp:ImageButton ID="cmdRecenter" runat="server" border="0" ImageUrl="~/images/rctf.gif" OnClick="cmdRecenter_Click" ToolTip="Re-Center"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100px"></td>
                                    </tr>   
                                    <tr>
                                        <td style="width: 100px" align="center">
                                            <asp:ImageButton ID="cmdMapOpt" runat="server" border="0" ImageUrl="~/images/mapOpts.gif" OnClientClick="MapOptions()" ToolTip="Map Options"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100px"></td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 100px">
                                            <img id="Img1" src="../images/resizemp.gif" onclick="ResizeMap()" runat=server   alt="<%$ Resources:ResizeMap %>"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 100px"></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100px" align="center">
                                            <asp:ImageButton ID="cmdStopLegends" runat="server" border="0" ImageUrl="~/images/imgHistoryStopLegends.gif"  OnClientClick="StopLegends()" ToolTip="Legend"/>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>   
                </td>
            </tr>
        </table>
    </form>
    <form name="myform">
        <table border="0">
            <tr>
                <td>
                    <input type="hidden" style ="size:4" value="-1" name="imagex"/>
                </td>
                <td>
                    <input type="hidden" style ="size:4" value="-1" name="imagey"/>
                </td>
                <td>
                    <input type="hidden" style ="size:4" value="-1" name="imageEndx"/>
                </td>
                <td>
                    <input type="hidden" style ="size:4" value="-1" name="imageEndy"/>
                </td>
                <td>
                    <input type="hidden" style ="size:4" value="-1" name="imageW"/>
                </td>
                <td>
                    <input type="hidden" style ="size:4" value="-1" name="imageH"/>
                </td>
            </tr>
        </table>
        <div id="toolTipLayer" style="font-weight: normal; font-size: small; z-index: 113;visibility: hidden; color: gray; font-family: Tahoma, Arial; position: absolute"></div>
        <img id="imgMap" alt="" style="border-right: gray 3px double; border-top: gray 3px double;
            z-index: 101; left: 75px; border-left: gray 3px double; border-bottom: gray 3px double;
            position: absolute; top: 5px; height: 375px" height="375px"  src="<%=sn.History.ImgPath%>" width="<%=imageW%>"
            usemap="#locations" border="0"/>
    </form>
    <div class="trans" id="selectLayer" style="border-right: black 1px dashed; border-top: black 1px dashed;
        font-size: 0px; z-index: 112; visibility: hidden; border-left: black 1px dashed;
        border-bottom: black 1px dashed; position: absolute">
    </div>
    
    <script language="javascript" type="text/javascript">
	    <!--
		//if (ie)
		//	document.body.onmousedown=new Function("if (event.button==2||event.button==3)alert('The right mouse button has been disabled')")
		if (ns)
			document.body.onmousedown=new Function("if (event.which==2||event.which==3)alert('The right mouse button has been disabled')")							
		-->
    </script>
    <script language="JavaScript" type="text/javascript">
        <!--
		var textArray = new Array("<%=sn.Map.VehiclesToolTip %>");
		var ns4 = document.layers;
		var ns6 = document.getElementById && !document.all;
		var ie4 = document.all;
		offsetX = 0;
		offsetY = 20;
		var toolTipSTYLE = "";
		
		function initToolTips()	{
			if(ns4||ns6||ie4) {
				if(ns4) toolTipSTYLE = document.toolTipLayer;
				else if(ns6) toolTipSTYLE = document.getElementById("toolTipLayer").style;
				else if(ie4) toolTipSTYLE = document.all.toolTipLayer.style;
				if(ns4) {
				    document.captureEvents(Event.MOUSEMOVE);
				} else {
				    toolTipSTYLE.visibility = "visible";
				    toolTipSTYLE.display = "none";
				}
				document.myform.imgMap.onmousemove=moveToMouseLoc;
		    }
	    }

		function hide() {
			if(ns4) {
				toolTipSTYLE.visibility = "hidden";
			} else {
				toolTipSTYLE.display = "none";
			}
		}
		
		function toolTip(indexList)	{
    		fg = "black";
			bg = "white";
			var content = '<table border="1" cellspacing="0" cellpadding="1" bgcolor="' + bg + '" width="270" bordercolor="black"><tr>' +
				          '<td align="left" ><font face="Arial, Helvetica, sans-serif" color="' + fg + '" size=1px >';
			var indexArray = indexList.split('|');
			
			for (var i =0;i<indexList.length;i++) {
				if (indexArray[i] != null) {
					content += textArray[indexArray[i] - 1];
					if (indexList.length>1) 
						content +='<hr>';
				}		
			}
			content += '</font></td></tr></table>';
			
			if(ns4) {
			    toolTipSTYLE.document.write(content);
				toolTipSTYLE.document.close();
				toolTipSTYLE.visibility = "visible";
			}
			if(ns6) {
				document.getElementById("toolTipLayer").innerHTML = content;
				toolTipSTYLE.display='block'
			}
			if(ie4) {
				document.all("toolTipLayer").innerHTML=content;
				toolTipSTYLE.display='block'
			}
        }
		
		function moveToMouseLoc(e) {
		    if(ns4||ns6) {
				width = window.innerWidth;
				//width = page;
				x = e.pageX;
				y = e.pageY;
			} else {
				width =648; //document.body.clientWidth;
				height =424; //document.body.clientHeight;
				x = event.x ;//+ document.body.scrollLeft;
				y = event.y ;//+ document.body.scrollTop;
				if (y + toolTipLayer.offsetHeight+100> height) {
					y = y - (toolTipLayer.offsetHeight)-100;
				}
				if (y < 0) {
				    y = 0;
				}
            }
			
			if (x + 250 > width) {
				x = x -260;
			}
			toolTipSTYLE.left = x + offsetX;
			toolTipSTYLE.top = y + offsetY;
			return true;
		}
		//-->
	</script>
    <map name="locations">
        <%=sn.Map.VehiclesMappings%>
    </map>
    <div class="skin0" id="ie5menu" onmouseover="highlightie5(event)" style="z-index: 134;left: 156px; top: 81px" onclick="jumptoie5(event)" onmouseout="lowlightie5(event)" display:none>
        <div class="menuitems" jszoom="True">
            <asp:Label ID="lblMenuZoomIn" runat="server" meta:resourcekey="lblMenuZoomInResource1" Text="Zoom In"/>
        </div>
        <div class="menuitems" jszoom="False">
            <asp:Label ID="lblMenuZoomOut" runat="server" meta:resourcekey="lblMenuZoomOutResource1" Text="Zoom Out"/>
        </div>
        <div class="menuitems" jsprintmap="True">
            <asp:Label ID="lblMenuPrintMap" runat="server" meta:resourcekey="lblMenuPrintMapResource1" Text="Print Map"/>
        </div>
        <hr>
        <div class="menuitems" jscancel="True">
            <asp:Label ID="lblMenuCancel" runat="server" meta:resourcekey="lblMenuCancelResource1" Text="Cancel"/>
        </div>
    </div>
    <table id="tblHistoryGridHeader" style="border-right: white 2px outset; border-top: white 2px outset;
        z-index: 101; left: 8px; border-left: white 2px outset; width: 797px; border-bottom: white 2px outset;
        position: absolute; top: 410px; height: 18px" cellspacing="0" cellpadding="0" border="0" runat="server" visible="false">
        <tr>
            <td style="font-weight: bold; font-size: 11px; width: 160px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray" align="left">
                <asp:Label ID="lblDateTime" runat="server" meta:resourcekey="lblDateTimeResource1" Text="Date/Time"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 224px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray">
                <asp:Label ID="lblAddress" runat="server" meta:resourcekey="lblAddressResource1" Text="Address"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 85px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray">
                <asp:Label ID="lblSpeed" runat="server" Text="Speed" meta:resourcekey="lblSpeedResource1"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 123px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray">
                <asp:Label ID="lblMessageType" runat="server" meta:resourcekey="lblMessageTypeResource1" Text="Message Type"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 100px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray">
                <asp:Label ID="lblMessage" runat="server" meta:resourcekey="lblMessageResource1" Text="Message"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 30px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray">
                <asp:Label ID="lblAck" runat="server" meta:resourcekey="lblAckResource1" Text="Ack"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 25px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray" bgcolor="gray">
            </td>
        </tr>
    </table>
    <table id="tblStopGridHeader" style="border-right: white 2px outset; border-top: white 2px outset;
        z-index: 102; left: 8px; border-left: white 2px outset; width: 797px; border-bottom: white 2px outset;
        position: absolute; top: 410px; height: 18px" cellspacing="0" cellpadding="0" border="0" runat="server" visible="false">
        <tr>
            <td style="font-weight: bold; font-size: 11px; width: 29px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray" align="left">
                &nbsp;<asp:Label ID="lblNo" runat="server" meta:resourcekey="lblNoResource1" Text="No"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 140px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray">
                &nbsp;<asp:Label ID="lblArrival" runat="server" meta:resourcekey="lblArrivalResource1" Text="Arrival"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 251px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray">
                &nbsp;<asp:Label ID="lblAddress2" runat="server" meta:resourcekey="lblAddress2Resource1" Text="Address"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 135px; color: white; font-family: Arial,Helvetica,sans-serif;
                background-color: gray">
                &nbsp;<asp:Label ID="lblDeparture" runat="server" meta:resourcekey="lblDepartureResource1" Text="Departure"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 53px; color: white; font-family: Arial,Helvetica,sans-serif;
                background-color: gray">
                &nbsp;<asp:Label ID="lblDuration" runat="server" meta:resourcekey="lblDurationResource1" Text="Duration"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 54px; color: white; font-family: Arial,Helvetica,sans-serif;
                background-color: gray">
                &nbsp;<asp:Label ID="lblStatus" runat="server" meta:resourcekey="lblStatusResource1" Text="Status"/>
            </td>
            <td style="font-weight: bold; font-size: 11px; width: 10px; color: white; font-family: Arial,Helvetica,sans-serif;background-color: gray" bgcolor="gray">
            </td>
        </tr>
    </table>
    <script language="JavaScript" type="text/javascript">
    	//set this variable to 1 if you wish the URLs of the highlighted menu to be displayed in the status bar
        var display_url = 0;
        var ie5 = document.all && document.getElementById;
        var ns6 = document.getElementById && !document.all;

        if (ie5 || ns6)
            var menuobj = document.getElementById("ie5menu");

		function showmenuie5(e){
		    //Find out how close the mouse is to the corner of the window
		    var rightedge = ie5 ? document.body.clientWidth - event.clientX : window.innerWidth - e.clientX;
		    var bottomedge = ie5 ? document.body.clientHeight - event.clientY : window.innerHeight - e.clientY;

			//if the horizontal distance isn't enough to accomodate the width of the context menu
		    if (rightedge < menuobj.offsetWidth)
		    //move the horizontal position of the menu to the left by it's width
		        menuobj.style.left = ie5 ? document.body.scrollLeft + event.clientX - menuobj.offsetWidth : window.pageXOffset + e.clientX - menuobj.offsetWidth;
		    else
		    //position the horizontal position of the menu where the mouse was clicked
		        menuobj.style.left = ie5 ? document.body.scrollLeft + event.clientX : window.pageXOffset + e.clientX;
			//same concept with the vertical position
		    if (bottomedge < menuobj.offsetHeight)
		        menuobj.style.top = ie5 ? document.body.scrollTop + event.clientY - menuobj.offsetHeight : window.pageYOffset + e.clientY - menuobj.offsetHeight;
		    else
		        menuobj.style.top = ie5 ? document.body.scrollTop + event.clientY : window.pageYOffset + e.clientY;

		    menuobj.style.visibility = "visible";
			return false
		}

		function hidemenuie5(e) {
		    menuobj.style.visibility = "hidden";
		}

		function highlightie5(e) {
		    var firingobj = ie5 ? event.srcElement : e.target;
			if (firingobj.className=="menuitems"||ns6&&firingobj.parentNode.className=="menuitems")	{
			    if (ns6 && firingobj.parentNode.className == "menuitems") firingobj = firingobj.parentNode;  //up one node
			        firingobj.style.backgroundColor = "highlight";
			        firingobj.style.color = "white";
			        if (display_url == 1)
			            window.status = event.srcElement.url;
			}
		}

		function lowlightie5(e)	{
		    var firingobj = ie5 ? event.srcElement : e.target;
			if (firingobj.className=="menuitems"||ns6&&firingobj.parentNode.className=="menuitems")	{
			    if (ns6 && firingobj.parentNode.className == "menuitems") firingobj = firingobj.parentNode;  //up one node
			        firingobj.style.backgroundColor = "";
			        firingobj.style.color = "black";
			        window.status = '';
			}
		}

		function jumptoie5(e) {
		    var firingobj = ie5 ? event.srcElement : e.target;
			if (firingobj.className=="menuitems"||ns6&&firingobj.parentNode.className=="menuitems")	{
			    if (ns6&&firingobj.parentNode.className=="menuitems") firingobj=firingobj.parentNode
				    if 	(firingobj.getAttribute("jsZoom")) {
						window.location.href="frmHist.aspx?ZoomIn="+firingobj.getAttribute("jsZoom");
					}
					
					if 	(firingobj.getAttribute("jsPrintMap")) {
						hidemenuie5();
						//document.all.divHist.style.visibility="hidden";
						window.print(); 
						//document.all.divHist.style.visibility="visible";
					}
					
					if 	(firingobj.getAttribute("jsCancel")) {
						hidemenuie5();
					}
					
					//else if (firingobj.getAttribute("target"))
					//		window.open(firingobj.getAttribute("url"),firingobj.getAttribute("target"))
					//	else (firingobj.getAttribute("target"))
					 //		window.location=firingobj.getAttribute("url")	
			}
		}

		if (ie5||ns6){
		    menuobj.style.display = '';
		    document.oncontextmenu = showmenuie5;
		    document.onclick = hidemenuie5;
		}
    </script>
</body>
</html>
