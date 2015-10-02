<%@ Page language="c#" Inherits="SentinelFM.frmAlarms" CodeFile="frmAlarms.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<title>frmAlarms</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<!--<!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->-->
	</HEAD>
	<body>
    <script language="javascript">
	<!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
    var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
    var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
    var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

    var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        
    function onOrganizationHierarchyNodeCodeClick()
    {
        var mypage='../../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
        if (MutipleUserHierarchyAssignment) {
            mypage = mypage + "&m=1&f=0";
        }
		var myname='OrganizationHierarchy';
		var w=740;
		var h=440;
		var winl = (screen.width - w) / 2; 
		var wint = (screen.height - h) / 2; 
		winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
		win = window.open(mypage, myname, winprops) 
		if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }  
        return false;
    }

    function OrganizationHierarchyNodeSelected(nodecode, fleetId)
    {            
        $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
        $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
        $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
    }
            
		//-->
</script>
		<form id="frmAlarmsMain" method="post" runat="server">

        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

        <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" 
            style="display:none;" AutoPostBack="True"
                    OnClick="hidOrganizationHierarchyPostBack_Click" 
            meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />

			<table id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellspacing="0"
				cellpadding="0" width="300" border="0">
				<tr>
					<td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicles"  />
					</td>
				</tr>
				<tr>
					<td>
						<table id="tblBody" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<tr>
								<td>
									<table id="tblForm" class="table"  style="HEIGHT: 550px"
										cellSpacing="0" cellPadding="0" width="990" border="0">
										<tr>
											<td class="configTabBackground">
												<table id="Table2" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<tr>
														<td>
															<table id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<tr>
																	<td>
																		<table id="Table5" style="Z-INDEX: 101; WIDTH: 190px; POSITION: relative; TOP: 0px; HEIGHT: 22px"
																			cellSpacing="0" cellPadding="0" border="0">
																			<tr>
																				<td><asp:button id="cmdVehicleInfo" runat="server" CausesValidation="False" CssClass="confbutton"
																						Text="Vehicle Info" CommandName="4" Width="112px" onclick="cmdVehicleInfo_Click" meta:resourcekey="cmdVehicleInfoResource1"></asp:button></td>
																				<td><asp:button id="cmdAlarms" runat="server" CausesValidation="False" CssClass="selectedbutton"
																						Text="Sensors/Alarms/Messages" CommandName="5" Width="176px" meta:resourcekey="cmdAlarmsResource1"></asp:button></td>
																				<td><asp:button id="cmdOutputs" runat="server" CausesValidation="False" CssClass="confbutton" Text="Outputs"
																						CommandName="6" onclick="cmdOutputs_Click" meta:resourcekey="cmdOutputsResource1"></asp:button></td>
																				<td><asp:button id="cmdFleetVehicle" runat="server" CausesValidation="False" CssClass="confbutton"
																						Text="Fleet-Vehicle Assignment" CommandName="7" Width="169px" onclick="cmdFleetVehicle_Click" meta:resourcekey="cmdFleetVehicleResource1"></asp:button></td>
																				<td>
                                                                                <asp:Button ID="btnEquipmentAssignment" runat="server" Width="168px" CommandName="77" CausesValidation="False"
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
																		<table id="Table6" cellSpacing="0" cellPadding="0" width="760" align="center" border="0">
																			<tr>
																				<td>
																					<table id="Table7" class=table  WIDTH="960px" HEIGHT="500px"
																						cellSpacing="0" cellPadding="0" border="0">
																						<tr>
																							<td class="configTabBackground">
																								<table id="Table1" style="WIDTH: 98%; HEIGHT: 444px" cellSpacing="0" cellPadding="0" width="679"
																									align="center" border="0">
																									<tr>
																										<td class="tableheading" align="left" height="5"></td>
																									</tr>
                                                                                                    <tr id="trFleetSelectOption" visible="false" runat="server">
                                                                                                        <td align="center" class="configTabBackground">
                                                                                                            <table class="formtext" runat="server" id="optBaseTable"  cellpadding=0 cellspacing=0  >
                                                                                                                <tr>
                                                                                                                    <td> 
                                                                                                                        <asp:Label ID="Label11" runat="server" Text="Based On:" meta:resourcekey="Label11Resource1"></asp:Label> 
                                                                                                                    </td>
                                                                                                                    <td>
                                                                                                                        <asp:RadioButtonList ID="optAssignBased" name="AssignBased" runat="server"  class="formtext" 
                                                                                                                                    RepeatDirection="Horizontal" AutoPostBack="True"
                                                                                                                                    onselectedindexchanged="optAssignBased_SelectedIndexChanged" 
                                                                                                                                    meta:resourcekey="optAssignBasedResource1" >
                                                                                                                            <asp:ListItem id="Radio2" type="radio" name="raFleetSelectOption" value="0" checked
                                                                                                                                runat="server" Selected="True" meta:resourcekey="ListItemResource1" >Fleet</asp:ListItem> 
                                                                                                                            <asp:ListItem id="Radio1" name="raFleetSelectOption" value="1" runat="server" 
                                                                                                                                        meta:resourcekey="ListItemResource2" >Organization Hierarchy Fleet</asp:ListItem>
                                                                                                                        </asp:RadioButtonList>                                                         
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
																									<tr>
																										<td style="BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; HEIGHT: 30px" align="center">
																											<table id="Table4" style="WIDTH: 764px; HEIGHT: 19px" cellSpacing="0" cellPadding="0" width="564"
																												align="center" border="0">
																												<tr>
																													<td style="WIDTH: 200px; HEIGHT: 17px" align="right"><asp:label id="lblFleet" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Fleet:"></asp:label></td>
																													<td style="HEIGHT: 17px">
                                                                                                                        <asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" Width="200px" DataTextField="FleetName"
																															DataValueField="FleetId" DESIGNTIMEDRAGDROP="79" AutoPostBack="True" onselectedindexchanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1"></asp:dropdownlist>
                                                                                                                        <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" Visible="false"
                                                                                                                                CssClass="combutton" Width="200px" 
                                                                                                                                OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                                                                                                meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                                                                                                    </td>
																													<td style="WIDTH: 38px; HEIGHT: 17px" width="38"></td>
																													<td style="WIDTH: 96px; HEIGHT: 17px"><asp:label id="lblVehicleName" runat="server" CssClass="formtext" Width="76px" Visible="False" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:"></asp:label></td>
																													<td style="WIDTH: 93px; HEIGHT: 17px"><asp:dropdownlist id="cboVehicle" runat="server" CssClass="RegularText" Width="200px" DataTextField="Description"
																															DataValueField="LicensePlate" DESIGNTIMEDRAGDROP="79" AutoPostBack="True" Visible="False" onselectedindexchanged="cboVehicle_SelectedIndexChanged" meta:resourcekey="cboVehicleResource1"></asp:dropdownlist></td>
																												</tr>
																											</table>
																										</td>
																									</tr>
																									<tr>
																										<td style="WIDTH: 100%; HEIGHT: 217px" vAlign="top" align="left">
																											<table id="Table3" cellSpacing="0" cellPadding="0" width="300" border="0">
																												<tr>
																													<td style="WIDTH: 680px; height: 233px;" vAlign="top">
																														<table id="tblHeader" class="DataHeaderStyle"  style="BORDER-RIGHT: white 1px solid; BORDER-TOP: white 1px solid; FONT-WEIGHT: bold; FONT-SIZE: 11px; BORDER-LEFT: white 1px solid; COLOR: white; BORDER-BOTTOM: white 1px solid; FONT-FAMILY: Arial,Helvetica,sans-serif;" 
																															cellSpacing="0" cellPadding="0" width="632" border="1" runat="server">
																															<tr>
																																<td style="BORDER-RIGHT: white 1px solid; WIDTH: 19px; HEIGHT: 15px" align="center"
																																	width="19">
                                                                                                                                    <asp:Label ID="lblIP" runat="server" Text="I/P" meta:resourcekey="lblIPResource1"></asp:Label></td>
																																<td style="BORDER-RIGHT: white 1px solid; BORDER-BOTTOM: white 1px solid; HEIGHT: 13px"
																																	align="center" width="188">&nbsp;<asp:Label ID="lblSensor" runat="server" Text="Sensor" meta:resourcekey="lblSensorResource1"></asp:Label></td>
																																<td style="BORDER-RIGHT: white 1px solid; BORDER-BOTTOM: white 1px solid; HEIGHT: 15px"
																																	align="center" colSpan="2">
                                                                                                                                    <asp:Label ID="lblActionOn" runat="server" Text="Action On" meta:resourcekey="lblActionOnResource1"></asp:Label></td>
																																<td style="BORDER-RIGHT: white 1px solid; BORDER-BOTTOM: white 1px solid; HEIGHT: 15px"
																																	align="center" colSpan="2">
                                                                                                                                    <asp:Label ID="lblActionOff" runat="server" Text="Action Off" meta:resourcekey="lblActionOffResource1"></asp:Label></td>
																																<td style="HEIGHT: 15px" align="center" width="20">&nbsp;</td>
																															</tr>
																															<tr>
																																<td style="BORDER-RIGHT: white 1px solid; WIDTH: 19px">&nbsp;</td>
																																<td style="BORDER-RIGHT: white 1px solid">&nbsp;</td>
																																<td style="BORDER-RIGHT: white 1px solid" align="center" width="82">
                                                                                                                                    <asp:Label ID="lblActionOnName" runat="server" Text="Name" meta:resourcekey="lblActionOnNameResource1"></asp:Label></td>
																																<td style="BORDER-RIGHT: white 1px solid" align="center" width="94">
                                                                                                                                    <asp:Label ID="lblActionOnSeverity" runat="server" Text="Severity" meta:resourcekey="lblActionOnSeverityResource1"></asp:Label></td>
																																<td style="BORDER-RIGHT: white 1px solid" align="center" width="82">
                                                                                                                                    <asp:Label ID="lblActionOffName" runat="server" Text="Name" meta:resourcekey="lblActionOffNameResource1"></asp:Label></td>
																																<td style="BORDER-RIGHT: white 1px solid; width: 93px;" align="center">
                                                                                                                                    <asp:Label ID="lblActionOffSeverity" runat="server" Text="Severity" meta:resourcekey="lblActionOffSeverityResource1"></asp:Label></td>
																																<td>&nbsp;</td>
																															</tr>
																														</table>
																														<asp:datagrid id="dgSensors" runat="server" Width="630px" DataKeyField="SensorId" ShowHeader="False"
																															PageSize="8" GridLines="None" CellPadding="3" BackColor="White" BorderWidth="2px" CellSpacing="1"
																															BorderColor="White" BorderStyle="Ridge" AllowPaging="True" AutoGenerateColumns="False" meta:resourcekey="dgSensorsResource1" OnCancelCommand="dgSensors_CancelCommand" OnEditCommand="dgSensors_EditCommand" OnPageIndexChanged="dgSensors_PageIndexChanged" OnUpdateCommand="dgSensors_UpdateCommand">
																															<FooterStyle ForeColor="White" BackColor="#C6C3C6"></FooterStyle>
																															<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
																															<AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
																															<ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
																															<HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
																															<Columns>
																																<asp:BoundColumn DataField="SensorId" ReadOnly="True" HeaderText="Id">
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle Width="20px"></ItemStyle>
																																</asp:BoundColumn>
																																<asp:TemplateColumn HeaderText="Sensor">
																																	<HeaderStyle Width="200px"></HeaderStyle>
																																	<ItemStyle Width="200px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="lblSensorName" text='<%# DataBinder.Eval(Container.DataItem,"SensorName") %>' Runat=server Width=200px meta:resourcekey="lblSensorNameResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox ID="txtSensorName" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"SensorName") %>' Width=200px meta:resourcekey="txtSensorNameResource1"></asp:TextBox>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText="Action On - Name">
																																	<ItemStyle Width="100px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="lblSensorActionOn" text='<%# DataBinder.Eval(Container.DataItem,"ActionOn") %>' Runat=server Width=75px meta:resourcekey="lblSensorActionOnResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox ID="txtSensorActionOn" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"ActionOn") %>' Width=75px meta:resourcekey="txtSensorActionOnResource1"></asp:TextBox>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText="Action On - Severity">
																																	<ItemStyle Wrap="False" Width="100px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="lblcboSensorActionOn" text='<%# DataBinder.Eval(Container.DataItem,"SeverityNameOn") %>' Runat=server meta:resourcekey="lblcboSensorActionOnResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:DropDownList ID="cboSensorActionsOn" DataSource='<%# dsAlarmSeverity %>' DataValueField="SeverityId" DataTextField="SeverityName" SelectedIndex='<%# GetAlarmSeverity(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"AlarmLevelOn"))) %>' Runat=server meta:resourcekey="cboSensorActionsOnResource1">
																																		</asp:DropDownList>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText="Action Off">
																																	<ItemStyle Width="120px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="lblSensorActionOff" text='<%# DataBinder.Eval(Container.DataItem,"ActionOff") %>' Runat=server Width=75px meta:resourcekey="lblSensorActionOffResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox ID="txtSensorActionOff" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"ActionOff") %>' Width=75px meta:resourcekey="txtSensorActionOffResource1"></asp:TextBox>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText="Action Off - Severity">
																																	<ItemStyle Wrap="False" Width="100px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="lblSensorActionsOff" text='<%# DataBinder.Eval(Container.DataItem,"SeverityNameOff") %>' Runat=server meta:resourcekey="lblSensorActionsOffResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:DropDownList ID="cboSensorActionsOff" DataSource='<%# dsAlarmSeverity %>' DataValueField="SeverityId" DataTextField="SeverityName" SelectedIndex='<%# GetAlarmSeverity(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"alarmLevelOff"))) %>' Runat=server meta:resourcekey="cboSensorActionsOffResource1">
																																		</asp:DropDownList>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:EditCommandColumn UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;"
																																	EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource1"></asp:EditCommandColumn>
																															</Columns>
																															<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
																														</asp:datagrid></td>
																													<td style="WIDTH: 33px; height: 233px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
																														&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
																													<td style="height: 233px"><asp:datagrid id="dgMessages" runat="server" Width="250px" DataKeyField="BoxMsgInTypeId" PageSize="8"
																															GridLines="None" CellPadding="3" BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White"
																															BorderStyle="Ridge" AllowPaging="True" AutoGenerateColumns="False" meta:resourcekey="dgMessagesResource1" OnCancelCommand="dgMessages_CancelCommand" OnEditCommand="dgMessages_EditCommand" OnPageIndexChanged="dgMessages_PageIndexChanged" OnUpdateCommand="dgMessages_UpdateCommand">
																															<FooterStyle ForeColor="White" BackColor="#C6C3C6"></FooterStyle>
																															<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
																															<AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
																															<ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
																															<HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
																															<Columns>
																																<asp:BoundColumn Visible="False" DataField="BoxMsgInTypeId" HeaderText="BoxMsgInTypeId"></asp:BoundColumn>
																																<asp:BoundColumn DataField="BoxMsgInTypeName" ReadOnly="True" HeaderText='<%$ Resources:MessageHeader %>'></asp:BoundColumn>
																																<asp:TemplateColumn HeaderText='<%$ Resources:SeverityHeader %>' >
																																	<ItemStyle Wrap="False" Width="100px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="SeverityName" text='<%# DataBinder.Eval(Container.DataItem,"SeverityName") %>' Runat=server meta:resourcekey="SeverityNameResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:DropDownList ID="cboMessageSeverity" DataSource='<%# dsAlarmSeverity %>' DataValueField="SeverityId" DataTextField="SeverityName" SelectedIndex='<%# GetAlarmSeverity(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"alarmLevel"))) %>' Runat=server meta:resourcekey="cboMessageSeverityResource1">
																																		</asp:DropDownList>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:EditCommandColumn UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;"
																																	EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource2"></asp:EditCommandColumn>
																															</Columns>
																															<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
																														</asp:datagrid>&nbsp;&nbsp;&nbsp;</td>
																												</tr>
																											</table>
																										</td>
																									</tr>
																									<tr>
																										<td style="HEIGHT: 15px" align="center">
																											<asp:label id="lblMessage" runat="server" CssClass="errortext" Width="270px" Visible="False"
																												Height="20px" meta:resourcekey="lblMessageResource1"></asp:label></td>
																									</tr>
																									<tr>
																										<td class="formtext" style="WIDTH: 535px; HEIGHT: 15px" align="left">
																											<P><U><B></B></U>&nbsp;</P>
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
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
