<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmFullScreenGrid.aspx.cs" Inherits="SentinelFM.frmFullScreenGrid" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Data</title>

    <script language="javascript">
<!--

	   ns = (document.layers)? true:false
			ie = (document.all)? true:false
			
			
		function ScrollColor()
	{
		with(document.body.style)
		{
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
	
	
	
	
		function VehicleInfoWindow(VehicleId) { 
					var mypage='frmVehicleDescription.aspx?VehicleId='+VehicleId
					var myname='';
					var w=330;
					var h=450;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
	
	
	function SensorsInfo(LicensePlate) { 
					var mypage='frmSensorMain.aspx?LicensePlate='+LicensePlate
					var myname='Sensors';
					var w=460;
					var h=720;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=yes' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 
				
		function Search() { 
					var mypage='frmMapVehicleSearch.aspx'
					var myname='Search';
					var w=840;
					var h=500;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 		
				
		
		 	
		function AutoReloadDetails()
    		{
    			window.location.href="frmFullScreenGrid.aspx" ; 
    		}
    

            var winMap;
            
            function FullMap() { 
					var mypage='frmbigmap.aspx'
					var myname='Map';
					var w=screen.width-50;
					var h=screen.height-50;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1' 
					winMap = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { winMap.window.focus(); } 
				}		
				
				 function FullVEMap() { 
					var mypage='../MapVE/VEMap.aspx'
					var myname='Map';
					var w=1074;
					var h=768;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,resizable=0,toolbar=0,menubar=0,scrollbars=1' 
					winMap = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { winMap.window.focus(); } 
				}		



        function CloseWindow()
        {
            
           winMap.close();
           top.close(); 
        }
    
//-->	
    </script>

    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">


    <!--<link href="http://www.sentinelfm.com/GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <!--<body onload="ScrollColor()" MS_POSITIONING="GridLayout">-->
</head>

<body  id="body" style="overflow:hidden"  leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0" onload="<%=strMapForm %>"   >
    <form  id="FleetForm"  method="post" runat="server">
        <table id="Table1" style="left: 4px; width: 100%; height: 100%;" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <td valign="bottom" align="left" height=25px >
                   <table height=25px >
                      <tr>
                       
                         <td valign=bottom>
                            <table id="tblFleetActions" runat="server">
                               <tr>
                                  <td valign=bottom>
                           
                                <asp:Menu ID="mnuFleetActions"   OnMenuItemClick="mnuFleetActions_MenuItemClick"  runat="server" 
                               
                               Font-Names="Verdana" Font-Size="12px" ForeColor="#666666" CssClass="formtext"  Orientation="Horizontal" meta:resourcekey="mnuFleetActionsResource1"
                              >
                               <StaticMenuItemStyle  VerticalPadding="2px" />
                               <DynamicHoverStyle BackColor="WhiteSmoke" ForeColor="White" />
                               
                               <DynamicMenuItemStyle  BorderStyle="Solid" BorderWidth="1px"   HorizontalPadding="5px" ForeColor="White" VerticalPadding="2px" />
                               <Items>
                                  <asp:MenuItem Text="Fleet:" Value="Fleet:" meta:resourcekey="MenuItemResource3">
                                     <asp:MenuItem   Text="Add Vehicles to Fleet" Value="1" meta:resourcekey="MenuItemResource1"></asp:MenuItem>
                                     <asp:MenuItem Text="Remove Vehicles from Fleet" Value="2" meta:resourcekey="MenuItemResource2">
                                     </asp:MenuItem>
                                  </asp:MenuItem>
                               </Items>
                                   
                               </asp:Menu>
                                  </td>
                                  <td valign=bottom>
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="242px"
                                    AutoPostBack="True" DataTextField="FleetName" DataValueField="FleetId" BackColor="White"
                                    OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList></td>
                                  <td valign=bottom>
                                      &nbsp;<asp:Menu ID="mnuOptions" runat="server" BackColor="WhiteSmoke" Font-Names="Verdana"
                                          Font-Size="12px" ForeColor="Black" meta:resourcekey="mnuOptionsResource1" OnMenuItemClick="mnuOptions_MenuItemClick"
                                          Orientation="Horizontal">
                                          <StaticMenuItemStyle BackColor="WhiteSmoke" Font-Underline="True" HorizontalPadding="10px"
                                              VerticalPadding="2px" />
                                          <DynamicHoverStyle BackColor="White" ForeColor="Black" />
                                          <DynamicMenuItemStyle BackColor="WhiteSmoke" BorderStyle="Solid" BorderWidth="1px"
                                              ForeColor="Black" HorizontalPadding="10px" VerticalPadding="2px" />
                                          <Items>
                                              <asp:MenuItem meta:resourceKey="MenuItemResource4" Text="Update Position" Value="UpdatePosition">
                                              </asp:MenuItem>
                                              <asp:MenuItem meta:resourceKey="MenuItemResource5" Text="Map It" Value="MapIt"></asp:MenuItem>
                                              <asp:MenuItem meta:resourceKey="MenuItemResource8" Text="Auto Refresh [ ]" Value="AutoRefresh">
                                              </asp:MenuItem>
                                          </Items>
                                      </asp:Menu>
                                  </td>
                                   <td  align="center" valign=bottom>&nbsp;&nbsp;<asp:Label ID="lblVisibleRows" runat="server" CssClass="formtext" Text="Visible Rows" Visible="False"></asp:Label>&nbsp;
                        </td>
                                  <td align="center" valign=bottom>
                                     <asp:DropDownList ID="cboRows" CssClass="RegularText" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboRows_SelectedIndexChanged" meta:resourcekey="cboGridPagingResource1" Visible="False">
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                      <asp:ListItem Selected="True">5</asp:ListItem>
                                        <asp:ListItem>7</asp:ListItem>
                                      <asp:ListItem>10</asp:ListItem>
                                   </asp:DropDownList></td>
                                  <td valign=bottom  align=right>
                                     <asp:Label ID="lblPageSize" runat="server" CssClass="formtext" Text="Items per Page:" meta:resourcekey="lblPageSizeResource1"></asp:Label></td>
                                  <td valign=bottom>
                                     <asp:DropDownList ID="cboGridPaging" CssClass="RegularText" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboGridPaging_SelectedIndexChanged" meta:resourcekey="cboGridPagingResource1">
                                        <asp:ListItem Value="999" meta:resourcekey="ListItemResource1">All</asp:ListItem>
                                        <asp:ListItem meta:resourcekey="ListItemResource2">15</asp:ListItem>
                                        <asp:ListItem Selected="True" meta:resourcekey="ListItemResource3">25</asp:ListItem>
                                        <asp:ListItem meta:resourcekey="ListItemResource4">50</asp:ListItem>
                                     </asp:DropDownList></td>
                                   <td valign="bottom">
                                       <ISWebDesktop:WebButton ID="WebButton1" runat="server" Height="20px" OnClientClick="CloseWindow"
                                           Text="Close" Width="80px">
                                       </ISWebDesktop:WebButton>
                                       </td>
                                  
                               </tr>
                            </table>
                         </td>
                           <td valign=bottom  >
                               <table id="tblWait" width=100% height=25px   runat="server" style="border-right: thin solid; border-top: thin solid; border-left: thin solid; border-bottom: thin solid">
              
                <tr>
                    
                         <td  align="center">&nbsp;
                        </td>
                    <td style="width: 100px" align="left" class=formtext ><asp:Label ID="lblUpdatingPositionMessage" runat="server" meta:resourcekey="lblUpdatingPositionMessageResource1"
                          Text="Updating Position..." Width="200px"></asp:Label>
                        </td>
                    <td style="height: 21px" align="center">
                        <asp:Image ID="imgWait" runat="server" Width="145px" Height="5px" ImageUrl="../images/prgBar.gif"
                            meta:resourcekey="imgWaitResource1"></asp:Image></td>
                            <td style="width: 22px" align="center">&nbsp;
                        </td>
                    <td style="height: 22px" align="center">
                        <asp:Button ID="cmdCancelUpdatePos" runat="server" CssClass="combutton" Text="Cancel"
                            OnClick="cmdCancelUpdatePos_Click" meta:resourcekey="cmdCancelUpdatePosResource1">
                        </asp:Button></td>
                        
                         <td style="width: 22px" align="center">&nbsp;
                        </td>
                    <td style="height: 22px" align="center">
                        <asp:Label ID="lblUpdatePosition" runat="server" CssClass="formtext" meta:resourcekey="lblUpdatePositionResource1" ForeColor="Red"></asp:Label></td>
                   <td align="center" style="height: 22px">
                   </td>
                </tr>
            </table>
                           </td>
                      </tr>
                   </table>
           
                </td>
            </tr>
            
            <tr>
                <td  valign=top align=left  >
                 <div id="divDetails" style=" width: 100%; height: 100% ;">
                     <ISWebGrid:WebGrid ID="dgFleetInfo" runat="server" OnInitializeDataSource="dgFleetInfo_InitializeDataSource" 
                   UseDefaultStyle="True"  Height="90px"   Width="100%"    OnRowChanged="dgFleetInfo_RowChanged" OnButtonClick="dgFleetInfo_ButtonClick" OnInitializeRow="dgFleetInfo_InitializeRow" OnInitializeLayout="dgFleetInfo_InitializeLayout" OnInitializePostBack="dgFleetInfo_InitializePostBack"   >
                   <RootTable DataKeyField="VehicleId" Caption="VehiclesLastKnownPositionInformation" DataMember="VehiclesLastKnownPositionInformation">
                      <Columns>
                     
                     
                           <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                            Caption="chkBoxShow" ColumnType="CheckBox" DataMember="chkBoxShow" EditType="NoEdit"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgFleetInfo_BoxId %>' DataMember="BoxId" DataType="System.Int32"
                            Name="BoxId" Width="50px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgFleetInfo_Description %>' ColumnType="Template" DataMember="Description"
                            Name="Description" ShowInSelectColumns="No" Width="130px">
                            <CellTemplate>
                         <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>' Text='<%# DataBinder.Eval(Container.DataItem, "Description") %>' meta:resourcekey="HyperLink_SiteResource1" ></asp:HyperLink>
                            </CellTemplate>
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgFleetInfo_StreetAddress %>' DataMember="StreetAddress" Name="StreetAddress"
                            Width="200px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgFleetInfo_MyDateTime %>' DataMember="OriginDateTime"    DataType="System.DateTime"
                            Name="MyDateTime" Width="100px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgFleetInfo_CustomSpeed %>' DataMember="CustomSpeed" Name="CustomSpeed"
                            Width="70px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgFleetInfo_BoxArmed %>'  DataMember="BoxArmed"  ColumnType="CheckBox" DataType="System.Boolean"
                            EditType="NoEdit" Name="chkArmed" Width="50px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgFleetInfo_VehicleStatus %>' DataMember="VehicleStatus" Name="VehicleStatus"
                            Width="80px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption="chkBoxShow" DataMember="chkBoxShow" Name="chkBoxShow"
                            Visible="False" Width="100px">
                         </ISWebGrid:WebGridColumn>
                         <ISWebGrid:WebGridColumn Caption="VehicleId" DataMember="VehicleId" Name="VehicleId"
                            Visible="False" Width="100px">
                         </ISWebGrid:WebGridColumn>
                        
                      
                       
                      
                      </Columns>
                     
                       <SelfReferencingSettings FitColumnOnExpand="True" />
                   </RootTable>
                   <LayoutSettings AutoHeight=true AutoWidth=true AutoColMinWidth=50   AutoFitColumnsBuffering=false AutoFitColumns=true      ShowRefreshButton=False  RowChangedAction=OnTheFlyPostback 
                      AllowSorting="Yes"  RowHeightDefault=25px
                      PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" />
                                    
                   
                </ISWebGrid:WebGrid>
                </div> 
                </td>
            </tr>
        </table>

      
       
    </form>
</body>
</html>