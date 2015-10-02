<%@ Page Language="c#" Inherits="SentinelFM.Map.frmBigDetails" CodeFile="frmBigDetails.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Details</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script language="javascript">
		<!--
			function VehicleInfoWindow(VehicleId) { 
					var mypage='frmVehicleDescription.aspx?VehicleId='+VehicleId
					var myname='';
					var w=300;
					var h=327;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
	
	
	function SensorsInfo(LicensePlate) { 
					var mypage='frmSensorMain.aspx?LicensePlate='+LicensePlate
					var myname='Sensors';
					var w=450;
					var h=555;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=yes' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 
				
	function AutoReloadDetails()
    	{
    	
    		window.location.href="frmBigDetails.aspx" ; 
    	}
    	
    	
    	function Search() { 
					var mypage='frmBigDetailsVehicleSearch.aspx'
					var myname='Search';
					var w=840;
					var h=500;
					
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 		
	
		//-->
    </script>

</head>
<body>
    <form id="frmBigDetailsForm" method="post" runat="server">
        <div id="divGrid" style="border-right: gray 4px double; border-top: gray 4px double;
            z-index: 101; left: 10px; border-left: gray 4px double; width: 989px; border-bottom: gray 4px double;
            position: relative; top: 15px; height: 650px" align="center">
            <div id="divTop" style="left: 50px; position: relative; top: 3px">
                <table id="Table1" style="width: 571px; height: 22px" cellspacing="0" cellpadding="0"
                    border="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblSelect" runat="server" CssClass="formtext" meta:resourcekey="lblSelectResource1"> Fleet:</asp:Label></td>
                        <td>
                            <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" AutoPostBack="True"
                                DataTextField="FleetName" DataValueField="FleetId" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                meta:resourcekey="cboFleetResource1">
                            </asp:DropDownList></td>
                        <td>
                            <asp:Button runat="server" CssClass="combutton" ID="cmdSearch" Width="88px" Height="19px"
                                OnClientClick="Search()" Text="Search" meta:resourcekey="cmdSearchResource1" /><%--<input class="combutton" id="cmdSearch" style="width: 88px; height: 19px"
                                    onclick="Search()" type="button" value="Search" name="cmdSearch">--%>
                        </td>
                        <td>
                            <asp:Button ID="cmdUpdatePosition" runat="server" CssClass="combutton" CommandName="35"
                                Text="Update Position" Width="107px" OnClick="cmdUpdatePosition_Click" meta:resourcekey="cmdUpdatePositionResource1">
                            </asp:Button></td>
                        <td>
                            <asp:Button ID="cmdSelectAll" runat="server" CssClass="combutton" Text="Select All"
                                Width="107px" OnClick="cmdSelectAll_Click" meta:resourcekey="cmdSelectAllResource1">
                            </asp:Button></td>
                        <td>
                            <asp:Button ID="cmdUnselect" runat="server" CssClass="combutton" Text="Deselect All"
                                Width="107px" OnClick="cmdUnselect_Click" meta:resourcekey="cmdUnselectResource1">
                            </asp:Button></td>
                        <td style="width:200px" >
                            <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="parent.close()"
                                Text="Close Window" meta:resourcekey="cmdCloseResource1" /><%--<input class="combutton" id="cmdClose" onclick="parent.close();"
                                    type="button" value="Close Window" name="cmdClose">--%></td>
                    </tr>
                </table>
            </div>
            <div id="DIV1" style="z-index: 102; left: 366px; overflow: auto; width: 330px; position: absolute;
                top: 134px; height: 168px" runat="server">
                <table id="tblWait" style="width: 296px; height: 139px" runat="server">
                    <tr>
                        <td style="font-size: 15pt; height: 15px" align="center">
                            <asp:Label ID="lblPleaseWaitMessage" runat="server" meta:resourcekey="lblPleaseWaitMessageResource1">Please wait...</asp:Label></td>
                    </tr>
                    <tr>
                        <td style="font-size: 15pt; height: 15px" align="center">
                            <asp:Label ID="lblUpdatingPositionMessage" runat="server" meta:resourcekey="lblUpdatingPositionMessageResource1">Updating Position...</asp:Label></td>
                    </tr>
                    <tr>
                        <td style="height: 10px" align="center">
                            <asp:Image ID="imgWait" runat="server" Width="105px" ImageUrl="../images/prgBar.gif"
                                Height="5px" meta:resourcekey="imgWaitResource1"></asp:Image></td>
                    </tr>
                    <tr>
                        <td style="height: 22px" align="center">
                            <asp:Button ID="cmdCancelUpdatePos" runat="server" CssClass="combutton" Text="Cancel"
                                OnClick="cmdCancelUpdatePos_Click" meta:resourcekey="cmdCancelUpdatePosResource1">
                            </asp:Button></td>
                    </tr>
                </table>
            </div>
            <asp:DataGrid ID="dgFleetInfo" runat="server" Width="916px" AllowPaging="True" GridLines="None"
                CellSpacing="1" PageSize="19" AutoGenerateColumns="False" BorderColor="Gray"
                BorderWidth="1px" BackColor="White" CellPadding="3" OnPageIndexChanged="dgFleetInfo_PageIndexChanged"
                meta:resourcekey="dgFleetInfoResource1">
                <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                <Columns>
                    <asp:BoundColumn Visible="False" DataField="VehicleId" HeaderText='<%$ Resources:dgFleetInfo_VehicleId %>'>
                    </asp:BoundColumn>
                    <asp:TemplateColumn>
                        <HeaderStyle Width="20px"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkCheckBox" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBoxShow") %>'
                                runat="server" meta:resourcekey="chkCheckBoxResource1" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:HyperLinkColumn DataNavigateUrlField="LicensePlate" DataNavigateUrlFormatString="javascript:var w =SensorsInfo('{0}')"
                        DataTextField="Description" SortExpression="Description" HeaderText='<%$ Resources:dgFleetInfo_Description %>'
                        DataTextFormatString="{0:c}" meta:resourcekey="HyperLinkColumnResource1">
                        <HeaderStyle Width="100px"></HeaderStyle>
                        <ItemStyle Wrap="False" ForeColor="Black"></ItemStyle>
                    </asp:HyperLinkColumn>
                    <asp:BoundColumn DataField="StreetAddress" ReadOnly="True" HeaderText='<%$ Resources:dgFleetInfo_StreetAddress %>'>
                        <HeaderStyle Width="335px"></HeaderStyle>
                        <ItemStyle Width="335px"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="OriginDateTime" HeaderText='<%$ Resources:dgFleetInfo_MyDateTime %>'>
                        <HeaderStyle Width="200px"></HeaderStyle>
                        <ItemStyle Width="200px"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="CustomSpeed" ReadOnly="True" HeaderText='<%$ Resources:dgFleetInfo_CustomSpeed %>'>
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
                </Columns>
                <PagerStyle HorizontalAlign="Right" ForeColor="gray" BackColor="White" Mode="NumericPages">
                </PagerStyle>
            </asp:DataGrid>
        </div>
    </form>
</body>
</html>
