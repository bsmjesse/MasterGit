<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHistExtended.aspx.cs" Inherits="SentinelFM.History_frmHistExtended" Culture="en-US" meta:resourcekey="PageResource2" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop" TagPrefix="ISWebDesktop" %>
<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>

<html>
<head runat="server">
    <title>History</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR"/>
    <meta content="C#" name="CODE_LANGUAGE"/>
    <meta content="JavaScript" name="vs_defaultClientScript"/>
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <style type="text/css">.trans { FILTER: alpha(opacity=50); BACKGROUND-COLOR: #dbeaf5; moz-opacity: 0.5 }
	    .id { FONT-WEIGHT: bold; FONT-SIZE: 9px; COLOR: white; FONT-FAMILY: verdana,atial,helvetica,san-serif }
	    .deadlink { PADDING-RIGHT: 2px; PADDING-LEFT: 2px; FONT-SIZE: 10px; COLOR: #000000; FONT-FAMILY: arial,helvetica,san-serif; TEXT-DECORATION: none }
	    .skin0 { BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; FONT-SIZE: 11px; Z-INDEX: 120; VISIBILITY: hidden; BORDER-LEFT: black 1px solid; WIDTH: 155px; CURSOR: default; LINE-HEIGHT: 20px; BORDER-BOTTOM: black 1px solid; FONT-FAMILY: Arial, Helvetica, sans-serif; POSITION: absolute; BACKGROUND-COLOR: menu }
	    .menuitems { PADDING-RIGHT: 5px; PADDING-LEFT: 5px }
    </style>

    <script language="JavaScript" type="text/javascript">
        <!--
        function ResizeMap()
        {
            location.href="?clientWidth="+document.body.clientWidth+"&clientHeight="+document.body.clientHeight;
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

		ns = (document.layers) ? true : false;
		ie = (document.all) ? true : false;

    	var sImageName = "imgMap";
		var oImageInfo     = { x : -1, y : -1, w : -1, h : -1 };
		var oSelectInfo    = { x1 : -1, y1 : -1, x2 : -1, y2 : -1 };
		var bDown          = false;
		var bMustSetValues = false;

		function findObjectPosition(oTmp){
			var oPosition = { x : 0, y : 0 };
		    while (oTmp.offsetParent){
		        oPosition.x += oTmp.offsetLeft;
		        oPosition.y += oTmp.offsetTop;
				oTmp = oTmp.offsetParent;
			}
			return oPosition;
		}

		function getMouse(e){
			if(document.all){
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
			document.frmHistory.imagex.value = (oSelectInfo.x1 < oSelectInfo.x2 ? oSelectInfo.x1 : oSelectInfo.x2) - oImageInfo.x;
			document.frmHistory.imagey.value = (oSelectInfo.y1 < oSelectInfo.y2 ? oSelectInfo.y1 : oSelectInfo.y2) - oImageInfo.y;
			document.frmHistory.imageW.value = Math.abs(oSelectInfo.x2 - oSelectInfo.x1);
			document.frmHistory.imageH.value = Math.abs(oSelectInfo.y2 - oSelectInfo.y1);
		    oTmp.style.top    = (oSelectInfo.y1 < oSelectInfo.y2 ? oSelectInfo.y1 : oSelectInfo.y2) - (document.all ? 0 : 1);
		    oTmp.style.left   = (oSelectInfo.x1 < oSelectInfo.x2 ? oSelectInfo.x1 : oSelectInfo.x2) - (document.all ? 0 : 1);
		    oTmp.style.width  = Math.abs(oSelectInfo.x2 - oSelectInfo.x1);
		    oTmp.style.height = Math.abs(oSelectInfo.y2 - oSelectInfo.y1);
		}

		function down(e) {
		    //clear
		    document.frmHistory.imagex.value=0;
		    document.frmHistory.imagey.value=0;
		    document.frmHistory.imageW.value=0;
		    document.frmHistory.imageH.value=0;
		    //
			document.frmHistory.imgMap.onmousemove=move;
			document.frmHistory.imgMap.useMap=null;

			document.frmHistory.imagex.value=event.offsetX;
			document.frmHistory.imagey.value=event.offsetY;

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
			
			document.frmHistory.imageEndx.value =parseFloat(document.frmHistory.imagex.value)+parseFloat(document.frmHistory.imageW.value);
			document.frmHistory.imageEndy.value =parseFloat(document.frmHistory.imagey.value)+parseFloat(document.frmHistory.imageH.value); 
			
			if (event.button==1)
				window.location.href="frmHistExtended.aspx?CoordInX="+document.frmHistory.imagex.value+"&CoordInY="+document.frmHistory.imagey.value+"&CoordEndX="+document.frmHistory.imageEndx.value+"&CoordEndY="+document.frmHistory.imageEndy.value ; 
			
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
  				document.frmHistory.imgMap.onmousedown = down;
  				//document.frmHistory.imgMap.onmousemove = move;
  				document.frmHistory.imgMap.onmouseup = up;
			} else {
			    alert("Your browser is not campatible with this script");
			}
		}
        //-->
    </script>
    <script language="javascript" type="text/javascript">
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
<body onload="init();initToolTips();" style="height: 100%;" topmargin="0"; leftmargin="5px;">
    <form id="frmHistory" method="post" runat="server">
     
            
            <table style="z-index: 107; left: 0px; overflow: auto; width: 100%;
            height:350px;position:absolute ; top: 0px " cellpadding=0 cellspacing=0   >
               <tr>
                  <td align="left">
                        
      
        
                     <table>
                        <tr>
                           <td valign=top  >
          
       
       
        
       
        <table id="Table3" style="z-index: 115;  background-color: white" height="320" cellspacing="0"
            cellpadding="0" width="63" bgcolor="white" border="0" >
            <tr>
                <td  align="center" valign=top  >
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
                                  <asp:ImageButton ID="btnMoveNorth" runat="server" border="0" ImageUrl="~/images/icon_go_up.gif" OnClick="btnMoveNorth_Click" ToolTip="Move map north" meta:resourcekey="btnMoveNorthResource2" /></td>
                               <td style="height: 19px">
                               </td>
                            </tr>
                            <tr>
                               <td>
                                  <asp:ImageButton ID="btnMoveWest" runat="server" border="0" ImageUrl="~/images/icon_go_left.gif" OnClick="btnMoveWest_Click" ToolTip="Move map west" meta:resourcekey="btnMoveWestResource2" /></td>
                               <td style="background-color: white">
                               </td>
                               <td>
                                  <asp:ImageButton ID="btnMoveEast" runat="server" border="0" ImageUrl="~/images/icon_go_right.gif" OnClick="btnMoveEast_Click" ToolTip="Move map east" meta:resourcekey="btnMoveEastResource2" /></td>
                            </tr>
                            <tr>
                               <td>
                               </td>
                               <td>
                                  <asp:ImageButton ID="btnMoveSouth" runat="server" border="0" ImageUrl="~/images/icon_go_down.gif" OnClick="btnMoveSouth_Click" ToolTip="Move map south" meta:resourcekey="btnMoveSouthResource2" /></td>
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
                                           <asp:ImageButton ID="btnZoomIn" runat="server" border="0" ImageUrl="../images/plusImg.gif" OnClick="btnZoomIn_Click" ToolTip="Zoom In" meta:resourcekey="btnZoomInResource2" /></td>
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
                                           <asp:ImageButton ID="btnZoomOut" runat="server" border="0" ImageUrl="../images/minusImg.gif" OnClick="btnZoomOut_Click" ToolTip="Zoom Out" meta:resourcekey="btnZoomOutResource2" /></td>
                                     </tr>
                                     <tr>
                                        <td style="width: 61px; height: 21px">
                                        </td>
                                     </tr>
                                  </table>
                      </td>
                   </tr>
                   
                   <tr>
                      <td align="center"  valign=top >
                      
                      
                      <table style="border-right: 1px outset; border-top: 1px outset; border-left: 1px outset;
                            border-bottom: 1px outset">
                            <tr>
                               <td style="width: 100px">
                               </td>
                            </tr>
                            <tr>
                               <td style="width: 100px" align="center">
                                  <asp:ImageButton ID="cmdRecenter" runat="server" border="0" ImageUrl="~/images/rctf.gif" OnClick="cmdRecenter_Click" ToolTip="Re-Center" meta:resourcekey="cmdRecenterResource2" /></td>
                            </tr>
                            <tr>
                               <td style="width: 100px">
                               </td>
                            </tr>
                            <tr>
                               <td style="width: 100px" align="center">
                                  <asp:ImageButton ID="cmdMapOpt" runat="server" border="0" ImageUrl="~/images/mapOpts.gif" OnClientClick="MapOptions()" ToolTip="Map Options" meta:resourcekey="cmdMapOptResource2"  /></td>
                            </tr>
                            <tr>
                               <td style="width: 100px">
                               </td>
                            </tr>
                            <tr>
                               <td align="center" style="width: 100px">
                                   <img id="Img1" src="../images/resizemp.gif" onclick="ResizeMap()" runat=server   alt="<%$ Resources:ResizeMap %>"  /></td>
                            </tr>
                            <tr>
                               <td align="center" style="width: 100px">
                               </td>
                            </tr>
                            <tr>
                               <td style="width: 100px" align="center">
                                  <asp:ImageButton ID="cmdStopLegends" runat="server" border="0" ImageUrl="~/images/imgHistoryStopLegends.gif"  OnClientClick="StopLegends()" ToolTip="Legend" meta:resourcekey="cmdStopLegendsResource2" Visible="False"  /></td>
                            </tr>
                         </table>
                         
                         
                      </td>
                   </tr>
                </table>   
                </td>
            </tr>
           
        </table>
                           </td>
                           <td width=100% valign=top >
                   
                   
        <img id="imgMap" class="tableDoubleBorder"  style="z-index: 101;height: 325px" height="325px"  src="<%=sn.History.ImgPath%>" width="<%=imageW%>"
            usemap="#locations" border="0">
            
            </td>
                        </tr>
                       
                     </table>
                    
             
      <table border="0">
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
                  </td>
                  <td align="left" valign=top  >
                        <iframe src=frmhistorycrt.aspx  style="width: 200px; height: 340px" frameborder=no  ></iframe>  
                  </td>
               </tr>
             <tr>
                <td align="left" valign=top  >
                  <table>
                      <tr>
                         <td >
                            <asp:Label ID="lblVisibleRows" runat="server" CssClass="formtext" Text="Visible Rows" Visible="False"></asp:Label></td>
                         <td   > 
                            <asp:DropDownList ID="cboRows" runat="server" AutoPostBack="True" CssClass="RegularText"
                               meta:resourcekey="cboGridPagingResource1" OnSelectedIndexChanged="cboRows_SelectedIndexChanged" Visible="False">
                               <asp:ListItem>1</asp:ListItem>
                               <asp:ListItem>2</asp:ListItem>
                               <asp:ListItem Selected="True">5</asp:ListItem>
                               <asp:ListItem>7</asp:ListItem>
                               <asp:ListItem>10</asp:ListItem>
                            </asp:DropDownList></td>
                         <td >
                            <asp:Label ID="lblPageSize" runat="server" CssClass="formtext" Text="Items per Page:" meta:resourcekey="lblPageSizeResource1"></asp:Label></td>
                         <td >
                            <asp:DropDownList ID="cboGridPaging" runat="server" AutoPostBack="True" CssClass="RegularText"
                               OnSelectedIndexChanged="cboGridPaging_SelectedIndexChanged" meta:resourcekey="cboGridPagingResource1">
                               <asp:ListItem Value="999" meta:resourcekey="ListItemResource1">All</asp:ListItem>
                               <asp:ListItem meta:resourcekey="ListItemResource2">15</asp:ListItem>
                               <asp:ListItem Selected="True" meta:resourcekey="ListItemResource3">25</asp:ListItem>
                               <asp:ListItem meta:resourcekey="ListItemResource4">50</asp:ListItem>
                            </asp:DropDownList>
                         </td>
                         <td width=10px >
                            <asp:Button ID="cmdMapSelected" runat="server" Text="Map It" CssClass="combutton" OnClick="cmdMapSelected_Click" /></td>
                      </tr>
                   </table>
                </td>
                <td align="left">
                </td>
             </tr>
             
          </table>
          
          <div id=HistoryGrid style="position:relative; top:370px; " >
             <ISWebGrid:WebGrid ID="dgHistoryDetails"   runat="server"  Width="99%"  Height="250px"   UseDefaultStyle="True" 
             OnInitializeDataSource="dgHistoryDetails_InitializeDataSource" OnRowChanged="dgHistoryDetails_RowChanged"   OnButtonClick="dgHistoryDetails_ButtonClick" meta:resourcekey="dgHistoryDetailsResource2" OnInitializeRow="dgHistoryDetails_InitializeRow" OnInitializeLayout="dgHistoryDetails_InitializeLayout"   >
               <RootTable DataKeyField="dgKey">
                   <Columns>
                   
                     <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                            ColumnType="CheckBox"  EditType="NoEdit" DataMember="chkBoxShow"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                         </ISWebGrid:WebGridColumn>
                         
                     <ISWebGrid:WebGridColumn Caption="dgKey" DataMember="dgKey" DataType="System.Int32"
                        Name="dgKey" Visible="False">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_DateTime %>' DataMember="OriginDateTime" DataType="System.DateTime"
                        Name="MyDateTime" Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Address %>' DataMember="StreetAddress" Name="StreetAddress"
                        Width="200px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Speed %>' DataMember="Speed" DataType="System.Int32"
                        Name="Speed" Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_MsgTypeName %>' ColumnType="Template" DataMember="BoxMsgInTypeName"
                        Name="BoxMsgInTypeName" Width="150px">
                        <CellTemplate>
                         <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>' Text='<%# DataBinder.Eval(Container.DataItem, "BoxMsgInTypeName") %>' meta:resourcekey="HyperLink_SiteResource1" ></asp:HyperLink>
                        </CellTemplate>
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_MsgDetails %>' DataMember="MsgDetails" Name="MsgDetails"
                        Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Ack %>' DataMember="Acknowledged" Name="Ack" Width="40px">
                     </ISWebGrid:WebGridColumn>
                      <ISWebGrid:WebGridColumn  DataMember="MyHeading" Name="MyHeading"  Visible=false Width="0px"  >
                     </ISWebGrid:WebGridColumn>
                    <%--  <ISWebGrid:WebGridColumn ButtonAutoPostback="True" Visible=false ButtonPostbackMode="FullPagePostback" Caption= '<%$ Resources:dgHist_MapIt %>'
                                                  ButtonText='<%$ Resources:dgHist_MapIt %>' ColumnType="Button" EditType="NoEdit" Name="MapIt" Width="70px">
                                               </ISWebGrid:WebGridColumn>--%>
                     <ISWebGrid:WebGridColumn Caption=" " Width="10px"  DataMember="Latitude" Name="Latitude"
                        Visible="false">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption="Longitude" DataMember="Longitude" Name="Longitude"
                        Visible="False">
                     </ISWebGrid:WebGridColumn>
                   
                  </Columns> 
                   
                 
               </RootTable>
           
   <LayoutSettings AllowColumnMove="Yes" AllowContextMenu="False" AllowFilter="Yes"  AutoFitColumns=true  
                      AllowSorting="Yes"    AutoFilterSuggestion="True"  RowChangedAction="OnTheFlyPostback"
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" RowHeightDefault="25px">
                      <FreezePaneSettings AbsoluteScrolling="True" />
                   </LayoutSettings>
                      
                                    
                   
                   
          </ISWebGrid:WebGrid>
     
                                      <ISWebGrid:WebGrid ID="dgStops" runat="server" Width="99%"   Height="250px"  OnButtonClick="dgStops_ButtonClick"
                                         OnInitializeDataSource="dgStops_InitializeDataSource" OnRowChanged="dgStops_RowChanged"
                                         UseDefaultStyle="True"  meta:resourcekey="dgStopsResource2" OnInitializeRow="dgStops_InitializeRow" OnInitializeLayout="dgStops_InitializeLayout">
                                         <RootTable DataKeyField="StopIndex">
                                            <Columns>
                                            <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                            ColumnType="CheckBox" DataMember="chkBoxShow"  EditType="NoEdit"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                         </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption="StopIndex" DataMember="StopIndex" Name="StopIndex"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Arrival %>' DataMember="ArrivalDateTime" DataType="System.DateTime"
                                                  Name="ArrivalDateTime" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Address %>' DataMember="Location" Name="Location"
                                                  Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Departure %>' DataMember="DepartureDateTime" DataType="System.DateTime"
                                                  Name="DepartureDateTime" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Duration %>' DataMember="StopDuration" Name="StopDuration"
                                                  Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Status %>' DataMember="Remarks" Name="Remarks" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption="Latitude" DataMember="Latitude" Name="Latitude"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption="Longitude" DataMember="Longitude" Name="Longitude"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption="StopDurationVal" DataMember="StopDurationVal" Name="StopDurationVal"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                              <%-- <ISWebGrid:WebGridColumn ButtonAutoPostback="True" ButtonPostbackMode="FullPagePostback" Caption= '<%$ Resources:dgHist_MapIt %>'
                                                  ButtonText='<%$ Resources:dgHist_MapIt %>' Visible=false  ColumnType="Button" EditType="NoEdit" Name="MapIt">
                                               </ISWebGrid:WebGridColumn>--%>
                                            </Columns>
                                               
                                         </RootTable>
                                         <LayoutSettings AllowColumnMove="Yes" AllowContextMenu="False" AllowFilter="Yes" 
                      AllowSorting="Yes"    AutoFilterSuggestion="True"  RowChangedAction="OnTheFlyPostback" AutoFitColumns=true
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" RowHeightDefault="25px">
                      <FreezePaneSettings AbsoluteScrolling="True" />
                   </LayoutSettings>
                                      </ISWebGrid:WebGrid>
          </div> 
    </form>

    <div class="trans" id="selectLayer" style="border-right: black 1px dashed; border-top: black 1px dashed;
        font-size: 0px; z-index: 112; visibility: hidden; border-left: black 1px dashed;
        border-bottom: black 1px dashed; position: absolute">
    </div>
    
     <div id="toolTipLayer" style="font-weight: normal; font-size: small; z-index: 113;
            visibility: hidden; color: gray; font-family: Tahoma, Arial; position: absolute">
        </div>

    <script language="javascript" type="text/javascript">
				<!--
						//if (ie)
						//	document.body.onmousedown=new Function("if (event.button==2||event.button==3)alert('The right mouse button has been disabled')")
						if (ns)
							document.body.onmousedown=new Function("if (event.which==2||event.which==3)alert('The right mouse button has been disabled')")							
				-->
    </script>

    <script language="JavaScript"><!--
			var textArray = new Array("<%=sn.Map.VehiclesToolTip %>");
			var ns4 = document.layers;
			var ns6 = document.getElementById && !document.all;
			var ie4 = document.all;
			offsetX = 0;
			offsetY = 20;
			var toolTipSTYLE="";
			function initToolTips()
			{
			if(ns4||ns6||ie4)
			{
				if(ns4) toolTipSTYLE = document.toolTipLayer;
				else if(ns6) toolTipSTYLE = document.getElementById("toolTipLayer").style;
				else if(ie4) toolTipSTYLE = document.all.toolTipLayer.style;
				if(ns4) {
				document.captureEvents(Event.MOUSEMOVE);
				} else {
				toolTipSTYLE.visibility = "visible";
				toolTipSTYLE.display = "none";
				}
				
				document.frmHistory.imgMap.onmousemove=moveToMouseLoc;
			}
			}

			function hide() {
				if(ns4) {
				toolTipSTYLE.visibility = "hidden";
				} else {
				toolTipSTYLE.display = "none";
				}
			}
			function toolTip(indexList)
			{
			    
				fg = "black";
				bg = "white"; 
				var content =
				'<table border="1" cellspacing="0" cellpadding="1" bgcolor="' + bg + '" width="270" bordercolor="black"><tr>' +
				'<td align="left" ><font face="Arial, Helvetica, sans-serif" color="' + fg +
				'" size=1px >' ;
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
			function moveToMouseLoc(e) 
			{
				if(ns4||ns6)
			{
				width = window.innerWidth;
				//width = page;
				x = e.pageX;
				y = e.pageY;
			}
			else
			{
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
			//--></script>

    <map name="locations">
        <%=sn.Map.VehiclesMappings%>
    </map>
    

</body>
</html>
