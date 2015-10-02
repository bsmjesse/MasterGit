
<%@ Page Language="c#"  Inherits="SentinelFM.frmFleetInfo" CodeFile="frmFleetInfo.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmFleetInfo</title>

    <script language="javascript">
<!--
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
					var w=500;
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
    			parent.frmFleetInfo.location.href="frmFleetInfo.aspx" ; 
    		}
    
    
//-->	
    </script>

    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <!--<body onload="ScrollColor()" MS_POSITIONING="GridLayout">-->
</head>
<body bgcolor="white" onload="ScrollColor()" style="height: 60%;">
    <form  id="frmFleet"  method="post" runat="server">
        <table id="Table1" style="left: 4px; width: 100%; height: 100%; position:  absolute; top: 0px;" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <td valign="top" align="center">
                    <table id="tblFleetsInfo" style="border-top-width: 2px; border-left-width: 2px; border-left-color: gray;
                        left: 40px; border-bottom-width: 2px; border-bottom-color: gray; width: 100%;height: 100%;
                        border-top-color: gray; border-right-width: 2px; border-right-color: gray" cellspacing="1"
                        cellpadding="1" align="left" border="0">
                        <tr>
                            <td height="25">
                                <asp:Menu ID="mnuFleetActions" AutoPostBack="True"  OnMenuItemClick="mnuFleetActions_MenuItemClick"  runat="server" 
                               
                               Font-Names="Verdana" Font-Size="0.8em" ForeColor="#666666" CssClass="formtext"  Orientation="Horizontal" meta:resourcekey="mnuFleetActionsResource1"
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
                            <td  height="25">
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="242px"
                                    AutoPostBack="True" DataTextField="FleetName" DataValueField="FleetId" BackColor="White"
                                    OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList></td>
                           <td align="left" height="25" style="width:20px">
                              &nbsp;&nbsp;
                           </td>
                            <td style="width: 100%" align=left   height="25">
                            
                            <asp:Menu ID="mnuOptions" AutoPostBack="True"   runat="server"  BackColor=WhiteSmoke
                               Font-Names="Verdana" Font-Size="0.8em" ForeColor="Black"   Orientation="Horizontal" OnMenuItemClick="mnuOptions_MenuItemClick" meta:resourcekey="mnuOptionsResource1"
                              >
                               <StaticMenuItemStyle  VerticalPadding="2px" BackColor=WhiteSmoke  Font-Underline=True   HorizontalPadding="10px" />
                               <DynamicHoverStyle BackColor="White" ForeColor="Black" />
                               <DynamicMenuItemStyle  BackColor="WhiteSmoke" BorderStyle="Solid" BorderWidth="1px"   HorizontalPadding="10px" ForeColor="Black" VerticalPadding="2px" />
                               <Items>
                                  <asp:MenuItem Text="Update Position" Value="UpdatePosition" meta:resourcekey="MenuItemResource4"></asp:MenuItem>
                                  <asp:MenuItem Text="Map It" Value="MapIt" meta:resourcekey="MenuItemResource5"></asp:MenuItem>
                                  <asp:MenuItem Text="Tools" Value="Tools" meta:resourcekey="MenuItemResource12">
                                     <asp:MenuItem Text="Search"  NavigateUrl="javascript: Search()" Value="Search" meta:resourcekey="MenuItemResource6"></asp:MenuItem>
                                     <asp:MenuItem meta:resourcekey="MenuItemResource16" Text="Sort by" Value="Sort by">
                                        <asp:MenuItem meta:resourcekey="MenuItemResource13" Text="Date/Time" Value="SortDateTime">
                                        </asp:MenuItem>
                                        <asp:MenuItem meta:resourcekey="MenuItemResource14" Text="Vehicle" Value="SortVehicle">
                                        </asp:MenuItem>
                                        <asp:MenuItem meta:resourcekey="MenuItemResource15" Text="Status" Value="SortStatus">
                                        </asp:MenuItem>
                                     </asp:MenuItem>
                                     <asp:MenuItem Text="Select All [ ]" Value="SelectAll" meta:resourcekey="MenuItemResource7"></asp:MenuItem>
                                     <asp:MenuItem Text="Full Screen" Value="Full Screen" meta:resourcekey="MenuItemResource11">
                                        <asp:MenuItem Text="Map" Value="FullMap" meta:resourcekey="MenuItemResource9" Enabled="False"></asp:MenuItem>
                                        <asp:MenuItem Text="Grid" Value="FullGrid" meta:resourcekey="MenuItemResource10"></asp:MenuItem>
                                     </asp:MenuItem>
                                  </asp:MenuItem>
                                     <asp:MenuItem Text="Auto Refresh [ ]" Value="AutoRefresh" meta:resourcekey="MenuItemResource8"></asp:MenuItem>
                               </Items>
                            </asp:Menu>
                               <%--<input class="Commands" id="cmdSearch" style="width: 116px;
                                        height: 19px" onclick="Search()" type="button" value="Search" name="cmdCancel">--%></td>
                           
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td style=" width: 100%; height: 100% ;">
                    <div id="divDetails"  style="overflow: auto; width: 100%; height: 100% ;">
                        <asp:DataGrid ID="dgFleetInfo" runat="server" Width="100%"   BackColor="White" BorderColor="Gray"
                            BorderWidth="1px" PageSize="15" ShowHeader="False" AllowSorting="True" AllowPaging="True"
                            AutoGenerateColumns="False" CellPadding="3" CellSpacing="1" GridLines="None"
                            OnPageIndexChanged="dgFleetInfo_PageIndexChanged" meta:resourcekey="dgFleetInfoResource1" OnSelectedIndexChanged="dgFleetInfo_SelectedIndexChanged">
                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                            <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                            <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                            <Columns >
                                <asp:BoundColumn Visible="False" DataField="VehicleId" HeaderText='<%$ Resources:dgFleetInfo_VehicleId %>'>
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText='<%$ Resources:dgFleetInfo_chkBoxShow %>'>
                                    <HeaderStyle Width="20px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" Width="20px"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkCheckBox" AutoPostBack='<%# chkShowAutoPostBack %>' OnCheckedChanged="SaveShowChecks"
                                            Checked='<%# DataBinder.Eval(Container.DataItem, "chkBoxShow") %>' runat="server"
                                            meta:resourcekey="chkCheckBoxResource1" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="BoxId" ReadOnly="True" HeaderText='BoxId'>
                                    <HeaderStyle Width="50px"></HeaderStyle>
                                    <ItemStyle Width="50px"></ItemStyle>
                                </asp:BoundColumn>
                                
                                <asp:HyperLinkColumn DataNavigateUrlField="LicensePlate" DataNavigateUrlFormatString="javascript:var w =SensorsInfo('{0}')"
                                    DataTextField="Description" SortExpression="Description" HeaderText='<%$ Resources:dgFleetInfo_Description %>'
                                    DataTextFormatString="{0:c}" meta:resourcekey="HyperLinkColumnResource1">
                                    <HeaderStyle Width="60px"></HeaderStyle>
                                    <ItemStyle Wrap="False" ForeColor="Black"></ItemStyle>
                                </asp:HyperLinkColumn>
                                <asp:BoundColumn DataField="StreetAddress" ReadOnly="True" HeaderText='<%$ Resources:dgFleetInfo_StreetAddress %>'>
                                    <HeaderStyle Width="400px"></HeaderStyle>
                                    <ItemStyle Width="400px"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="MyDateTime" HeaderText='<%$ Resources:dgFleetInfo_MyDateTime %>'>
                                    <HeaderStyle Width="160px"></HeaderStyle>
                                    <ItemStyle Width="160px"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="CustomSpeed" ReadOnly="True" HeaderText="<%$ Resources:dgFleetInfo_CustomSpeed %>">
                                    <HeaderStyle Width="65px"></HeaderStyle>
                                    <ItemStyle Wrap="False" Width="65px"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText='<%$ Resources:dgFleetInfo_BoxArmed %>'>
                                    <HeaderStyle Width="40px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkArmed" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "BoxArmed")) %>'
                                            runat="server" meta:resourcekey="chkArmedResource1" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn Visible="False" DataField="OriginDateTime" HeaderText='<%$ Resources:dgFleetInfo_OriginDateTime %>'>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="MyHeading" HeaderText='<%$ Resources:dgFleetInfo_MyHeading %>'>
                                    <HeaderStyle Width="50px"></HeaderStyle>
                                    <ItemStyle Width="50px"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="SensorMask" HeaderText='<%$ Resources:dgFleetInfo_SensorMask %>'>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="VehicleStatus" HeaderText='<%$ Resources:dgFleetInfo_VehicleStatus %>'>
                                    <HeaderStyle Width="70px"></HeaderStyle>
                                    <ItemStyle Width="70px"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="BoxId" HeaderText='<%$ Resources:dgFleetInfo_BoxId %>'>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="LicensePlate" HeaderText='<%$ Resources:dgFleetInfo_LicensePlate %>'>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="CurrentStatus" HeaderText='<%$ Resources:dgFleetInfo_CurrentStatus %>'>
                                    <ItemStyle Width="80px"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="LastCommunicatedDateTime" HeaderText='<%$ Resources:dgFleetInfo_LastCommunicatedDateTime %>'>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="FwTypeId" HeaderText='<%$ Resources:dgFleetInfo_FwTypeId %>'>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="Dormant" HeaderText='<%$ Resources:dgFleetInfo_Dormant %>'>
                                </asp:BoundColumn>
                        
                               <asp:ButtonColumn ButtonType="PushButton" CommandName="Select"     Text='<%$ Resources:dgFleetInfo_ViewHistory %>'>
                                <ItemStyle Width="40px" Font-Size="5px" CssClass=combutton  ></ItemStyle>
                               </asp:ButtonColumn>
                                
                            </Columns>
                            <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                Mode="NumericPages"    NextPageText="&gt;" Position="TopAndBottom" PrevPageText="&lt;"></PagerStyle>
                        </asp:DataGrid></div>
                </td>
            </tr>
        </table>
        <div id="DIV1" style="z-index: 102; left: 305px; overflow: auto; width: 317px; position: absolute;
            top: 55px; height: 134px" runat="server">
            <table id="tblWait" style="width: 309px; height: 108px" runat="server">
                <tr>
                    <td class="RegularText" style="height: 15px" align="center">
                        <asp:Label ID="lblPleaseWaitMessage" runat="server" meta:resourcekey="lblPleaseWaitMessageResource1" Text="Please wait..."></asp:Label></td>
                </tr>
                <tr>
                    <td class="formtext" style="height: 15px" align="center">
                        <asp:Label ID="lblUpdatingPositionMessage" runat="server" meta:resourcekey="lblUpdatingPositionMessageResource1" Text="Updating Position..."></asp:Label></td>
                </tr>
                <tr>
                    <td style="height: 21px" align="center">
                        <asp:Image ID="imgWait" runat="server" Width="105px" Height="5px" ImageUrl="../images/prgBar.gif"
                            meta:resourcekey="imgWaitResource1"></asp:Image></td>
                </tr>
                <tr>
                    <td style="height: 22px" align="center">
                        <asp:Button ID="cmdCancelUpdatePos" runat="server" CssClass="combutton" Text="Cancel"
                            OnClick="cmdCancelUpdatePos_Click" meta:resourcekey="cmdCancelUpdatePosResource1">
                        </asp:Button></td>
                </tr>
                <tr>
                    <td style="height: 22px" align="center">
                        <asp:Label ID="lblUpdatePosition" runat="server" CssClass="ErrorText" meta:resourcekey="lblUpdatePositionResource1"></asp:Label></td>
                </tr>
            </table>
        </div>

    </form>
</body>
</html>
