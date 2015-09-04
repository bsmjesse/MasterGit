
<%@ Page Language="c#" Inherits="SentinelFM.frmVehicleInfo" CodeFile="frmVehicleInfo.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmVehicleInfo</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->

    <script type="text/javascript" language="javascript">
		<!--
		
				function VehicleInfoWindow(LicensePlate) { 
					var mypage='frmVehicle_Add_Edit.aspx?LicensePlate='+LicensePlate
					var myname='';
					var w=550;
					var h=631;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}				


				function clickButton(e, buttonid) {
				    var evt = e ? e : window.event;
				    var bt = document.getElementById(buttonid);				   
				    if (bt) {
				        if (evt.keyCode == 13) {
				            bt.click();
				            return false;
				        }
				    }
				}

		//-->
    </script>

</head>
<body>
    <form id="frmEmails" method="post" runat="server" >
        <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicles" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" height="550px" class=table 
                                    cellspacing="0" cellpadding="0" width="990" border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" style="left: 10px; position: relative; top: 0px" cellspacing="0"
                                                cellpadding="0" width="300" border="0">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" style="left: 10px; position: relative; top: 0px" cellspacing="0"
                                                            cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table5" style="z-index: 101; width: 190px; position: relative; top: 0px;
                                                                        height: 22px" cellspacing="0" cellpadding="0" border="0">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Button ID="cmdVehicleInfo" runat="server" Width="112px" CausesValidation="False"
                                                                                    CssClass="selectedbutton" Text="Vehicle Info" CommandName="4" meta:resourcekey="cmdVehicleInfoResource1">
                                                                                </asp:Button></td>
                                                                            <td style="width: 167px">
                                                                                <asp:Button ID="cmdAlarms" runat="server" Width="176px" CausesValidation="False"
                                                                                    CssClass="confbutton" Text="Sensors/Alarms/Messages" CommandName="5" OnClick="cmdAlarms_Click"
                                                                                    meta:resourcekey="cmdAlarmsResource1"></asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="cmdOutputs" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                                    Text="Outputs" CommandName="6" OnClick="cmdOutputs_Click" meta:resourcekey="cmdOutputsResource1">
                                                                                </asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="cmdFleetVehicle" runat="server" Width="168px" CausesValidation="False"
                                                                                    CssClass="confbutton" Text="Fleet-Vehicle Assignment" CommandName="7" OnClick="cmdFleetVehicle_Click"
                                                                                    meta:resourcekey="cmdFleetVehicleResource1"></asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="cmdBoxFirmware" runat="server" Width="168px" CausesValidation="False"
                                                                                    CssClass="confbutton" Text="Box Firmware Status" CommandName="8" OnClick="cmdBoxFirmware_Click"
                                                                                    meta:resourcekey="cmdBoxFirmware"></asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="btnEquipmentAssignment" runat="server" Width="168px" CommandName="77"  CausesValidation="False"
                                                                                    CssClass="confbutton" Text="Equipment Assignment" PostBackUrl="~/Configuration/Equipment/frmAssignmentList.aspx"
                                                                                    meta:resourcekey="btnEquipmentAssignmentResource1"></asp:Button>

                                                                            </td>
                                                                            <td>
                                                                                <asp:Button ID="btnFuelCategory" runat="server" Width="168px" Visible= "false" CausesValidation="False" CssClass="confbutton"
                                                                                     Text="Fuel Category" PostBackUrl="~/Configuration/FuelCategory/frmFuelCategory.aspx"
                                                                                    meta:resourcekey="btnFuelCategoryResource1"></asp:Button>

                                                                            </td>

                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table id="Table6" cellspacing="0" cellpadding="0" width="617" align="center" border="0">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" style="width: 960px;height: 500px" class="tableDoubleBorder"  border="0">
                                                                                    <tr>
                                                                                        <td class="configTabBackground">
                                                                                            <table id="Table1" style="width: 823px; height: 477px" cellspacing="0" cellpadding="0"
                                                                                                width="823" align="center" border="0">
                                                                                                <tr>
                                                                                                    <td style="width: 100%; height: 360px" align="center">
                                                                                                       <table>
                                                                                                          <tr>
                                                                                                             <td style=" height: 10px" align="center">
                                                                                                                <table>
                                                                                                                   <tr>
                                                                                                                      <td style="width: 100px">
                                                                                                                      </td>
                                                                                                                      <td style="width: 100px">
                                                                                                                      </td>
                                                                                                                      <td style="width: 100px">
                                                                                                                      </td>
                                                                                                                      <td style="width: 100px">
                                                                                                                      </td>
                                                                                                                   </tr>
                                                                                                                   <tr>
                                                                                                                      <td style="width: 100px">
                                                                                                                      
                                                                                                                
                                                                                                                <asp:DropDownList ID="cboSearchType" runat="server" AutoPostBack="True" CssClass="RegularText" OnSelectedIndexChanged="cboSearchType_SelectedIndexChanged" meta:resourcekey="cboSearchTypeResource1">
                                                                                                                   <asp:ListItem  Value="0" meta:resourcekey="ListItemResource1" Text="Description"></asp:ListItem>
                                                                                                                   <asp:ListItem  Value="1" meta:resourcekey="ListItemResource2" Text="License Plate"></asp:ListItem>
                                                                                                                   <asp:ListItem  Value="2" meta:resourcekey="ListItemResource3" Text="Box ID"></asp:ListItem>
                                                                                                                   <asp:ListItem  Value="3" meta:resourcekey="ListItemResource4" Text="VIN Number"></asp:ListItem>
                                                                                                                
                                                                                                                
                                                                                                                     <asp:ListItem Value="4" meta:resourcekey="ListItemResource5" Text="Equip Number" Enabled="False"></asp:ListItem>
                                                                                                                     <asp:ListItem Value="5" meta:resourcekey="ListItemResource6" Text="Legacy Equip#" Enabled="False"></asp:ListItem>
                                                                                                                     <asp:ListItem Value="6" meta:resourcekey="ListItemResource7" Text="SAP Equip#" Enabled="False"></asp:ListItem>
                                                                                                                     <asp:ListItem Value="7" meta:resourcekey="ListItemResource8" Text="Object Type" Enabled="False"></asp:ListItem>
                                                                                                                     <asp:ListItem Value="8" meta:resourcekey="ListItemResource9" Text="DOT Number" Enabled="False"></asp:ListItem>
                                                                                                                     <asp:ListItem Value="9" meta:resourcekey="ListItemResource10" Text="Project Nbr" Enabled="False"></asp:ListItem>
                                                                                                                
                                                                                                                
                                                                                                                </asp:DropDownList> 
                                                                                                                
                                                                                                                </td>
                                                                                                                      <td >
                                                                                                                         <asp:TextBox ID="txtSearchParam" runat="server"   CssClass="formtext" Width="200px" meta:resourcekey="txtSearchParamResource1"  onkeypress="return clickButton(event,'cmdSearch')" ></asp:TextBox></td>
                                                                                                                      <td >
                                                                                                                         <asp:Button ID="cmdSearch" runat="server" CssClass="combutton" meta:resourcekey="cmdSearchResource1"
                                                                                                                            OnClick="cmdSearch_Click" Text="Search" Width="121px"/></td>
                                                                                                                            <td style="width: 100px"><asp:Button ID="cmdClear" runat="server" CssClass="combutton" meta:resourcekey="cmdClearResource1"
                                                                                                                            OnClick="cmdClear_Click" Text="Clear" Width="121px" /></td>
                                                                                                                   </tr>
                                                                                                                </table>
                                                                                                             </td>
                                                                                                          </tr>
                                                                                                          <tr>
                                                                                                             <td style="width: 100px">
                                                                                                        <asp:DataGrid ID="dgVehicles" runat="server" CellSpacing="1" GridLines="None" BackColor="White"
                                                                                                            BorderWidth="2px" BorderStyle="Ridge" BorderColor="White" CellPadding="3" AutoGenerateColumns="False"
                                                                                                            DataKeyField="LicensePlate" AllowPaging="True" PageSize="12" Width="866px" OnDeleteCommand="dgVehicles_DeleteCommand"
                                                                                                            OnPageIndexChanged="dgVehicles_PageIndexChanged" OnItemCreated="dgVehicles_ItemCreated"
                                                                                                            meta:resourcekey="dgVehiclesResource1">
                                                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                                            <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                                                            <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                                                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                                            <Columns>
                                                                                                                <asp:BoundColumn DataField="Description" HeaderText='<%$ Resources:dgVehicles_Description %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="VinNum" HeaderText='<%$ Resources:dgVehicles_VINNumber %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="LicensePlate" HeaderText='<%$ Resources:dgVehicles_LicensePlate %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="VehicleTypeName" HeaderText='<%$ Resources:dgVehicles_VehicleType %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="MakeName" HeaderText='<%$ Resources:dgVehicles_Make %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="ModelName" HeaderText='<%$ Resources:dgVehicles_Model %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="ModelYear" HeaderText='<%$ Resources:dgVehicles_Year %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="Color" HeaderText='<%$ Resources:dgVehicles_Color %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="BoxId" HeaderText='<%$ Resources:dgVehicles_Box %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="Email" HeaderText='<%$ Resources:dgVehicles_Email %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="IconTypeName" HeaderText='<%$ Resources:dgVehicles_IconType %>'>
                                                                                                                </asp:BoundColumn>
                                                                                                                <asp:HyperLinkColumn Text="&lt;img src=../images/edit.gif border=0&gt;" DataNavigateUrlField="LicensePlate"
                                                                                                                    DataNavigateUrlFormatString="javascript:var w =VehicleInfoWindow('{0}')" meta:resourcekey="HyperLinkColumnResource1">
                                                                                                                </asp:HyperLinkColumn>
                                                                                                                <asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete"  
                                                                                                                    meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
                                                                                                            </Columns>
                                                                                                            <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                                                                Mode="NumericPages"></PagerStyle>
                                                                                                        </asp:DataGrid></td>
                                                                                                          </tr>
                                                                                                       </table>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td style="height: 10px" align="center">
                                                                                                        <asp:Label ID="lblMessage" runat="server" Width="270px" CssClass="errortext" Visible="False"
                                                                                                            Height="8px" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td style="height: 25px" align="center">
                                                                                                        <asp:Button runat="server" CssClass="combutton" ID="cmdAddVehicle" OnClientClick="VehicleInfoWindow()"
                                                                                                            Text="Add Vehicle" meta:resourcekey="cmdAddVehicleResource1" /><%--<input class="combutton" id="cmdAddVehicle" onclick="VehicleInfoWindow()"
                                                                                                                type="button" value="Add Vehicle">--%></td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
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
</body>
</html>
