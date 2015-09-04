<%@ Page language="c#" CodeFile="frmExtras.aspx.cs" Inherits="SentinelFM.Configuration_frmExtras" Culture="en-US" UICulture="auto" EnableTheming="true" Theme="Default" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>frmOutputs</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="Configuration.css" type="text/css" rel="stylesheet" />
		<!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
	</head>
	<body>
	<form id="frmEmails" method="post" runat="server">
		<div id="tblCommands" style="left: 8px; position: absolute; top: 4px; width: 960px; height: 550px; padding: 10px 10px 10px 10px">
			<div id="subDiv1" style="width: 100%; height: 100%;">
				<div id="divButtons1" style="height: auto; width: auto; position: relative; top: 0px; left: 0px;">
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicles" IsPolicy="true" FleetUrl="frmfleets.aspx"  
                    VehicleUrl="frmfleetvehicles.aspx" />
				</div>
				<div id="tblForm" class="frame" style="padding: 10px 10px 10px 10px; width: 97%; height:91%">
					<div id="Table2" style="width: 100%; height:100%">
					    <div id="divButtons2" style="position: relative; top: 0px; left:0px; width: auto; height: auto;">
							<table style="border-style:none;" cellSpacing="0" cellPadding="0">
								<tr>
									<td>
										<asp:button id="cmdVehicleInfo" runat="server" CausesValidation="False" CssClass="confbutton"
											Text="Vehicle Info" Width="112px" CommandName="4" PostBackUrl="~/Configuration/frmvehicleinfo.aspx" meta:resourcekey="cmdVehicleInfoResource1"></asp:button>
									</td>
									<td>
									    <asp:button id="cmdAlarms" runat="server" Text="Sensors/Alarms/Messages" CssClass="confbutton"
											CausesValidation="False" Width="176px" CommandName="5" PostBackUrl="~/Configuration/frmalarms.aspx" meta:resourcekey="cmdAlarmsResource1"></asp:button></td>
									<td>
									    <asp:button id="cmdOutputs" runat="server" Text="Outputs" CssClass="confbutton" CausesValidation="False" PostBackUrl="~/Configuration/frmoutputs.aspx"
											CommandName="6" meta:resourcekey="cmdOutputsResource1"></asp:button>
									</td>
									<td>
                                        <asp:Button ID="btnExtras" runat="server" Width="168px" CausesValidation="False" CssClass="selectedbutton" Text="Extras"></asp:Button>
                                    </td>
									<td>
									    <asp:button id="cmdFleetVehicle" runat="server" CausesValidation="False" CssClass="confbutton"
									        Text="Fleet-Vehicle Assignment" Width="168px" CommandName="7" PostBackUrl="~/Configuration/frmvehiclefleet.aspx" meta:resourcekey="cmdFleetVehicleResource1"></asp:button>
									</td>
								</tr>
							</table>
						</div>	
						<div class="frame" style="width: 97%; height:91%; padding: 10px 10px 10px 10px;">
							<div id="divButtons3" style="position: relative; top: 0px; left:0px; width: auto; height:auto;">
                                <table cellpadding="0" cellspacing="0" style="border-style: none;">
                                    <tr>
						    	        <td>
						    	            <asp:Button ID="btnReefer" runat="server" CssClass="selectedbutton" Text="Reefer" Width="104px" />
					    	            </td>
						    	    </tr>
                                </table>
                            </div>
                            <div class="frame" style="width: auto; height:500; margin: 0px 0px 10px 0px; background-color:#fffff0">
                                <div style="position:relative; top: 20px; left: 20px; text-align:center;">
                                    <div style="position:relative; top:0px">
                                        <table style="WIDTH: 564px; HEIGHT: 19px" id="Table4" cellSpacing=0 cellPadding=0 width=564 align=center border=0>
                                            <tr>
                                                <td style="WIDTH: 56px; HEIGHT: 17px">
                                                    <asp:label id="lblFleet" runat="server" CssClass="formtext">Fleet:</asp:label></td>
                                                <td style="HEIGHT: 17px">
                                                    <asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" Width="200px" onselectedindexchanged="cboFleet_SelectedIndexChanged" AutoPostBack="True" DataValueField="FleetId" DataTextField="FleetName"></asp:dropdownlist>
                                                </td>
                                                <td style="WIDTH: 38px; HEIGHT: 17px" width=38>
                                                </td>
                                                <td style="WIDTH: 96px; HEIGHT: 17px">
                                                    <asp:label id="lblVehicleName" runat="server" CssClass="formtext" Width="76px" Visible="False">Vehicle:</asp:label>
                                                </td>
                                                <td style="WIDTH: 93px; HEIGHT: 17px">
                                                    <asp:dropdownlist id="cboVehicle" runat="server" CssClass="RegularText" Width="200px" onselectedindexchanged="cboVehicle_SelectedIndexChanged" AutoPostBack="True" DataValueField="LicensePlate" DataTextField="Description" Visible="False"></asp:dropdownlist>
                                                </td>
                                           </tr>
                                       </table>
                                    </div>
                                    <div id="tempSensors" style="border:solid 1px green; width: 700px; position:relative; top: 20px">
                                        <br />
                                        <asp:GridView ID="gdSensors" runat="server" EnableTheming="True" AutoGenerateColumns="False" Caption="<b>Temperature Zones</b>">
                                            <Columns>
                                                <asp:BoundField DataField="SensorId" HeaderText="Id" ReadOnly="True" />
                                                <asp:TemplateField HeaderText="Sensor">
                                                    <ItemTemplate>
														<asp:Label ID="lblSensorName" text='<%# Eval("SensorName") %>' Runat="server"></asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox ID="txtSensorName" Runat=server Text='<%# Eval("SensorName") %>' Width=200px></asp:TextBox>
													</EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action On - Name">
                                                    <ItemTemplate>
														<asp:Label ID="lblSensorActionOn" text='<%# Eval("ActionOn") %>' Runat="server" Width=75px ></asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox ID="txtSensorActionOn" Runat=server Text='<%# Eval("ActionOn") %>' Width=75px ></asp:TextBox>
													</EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action On - Severity">
                                                    <ItemTemplate>
														<asp:Label ID="lblcboSensorActionOn" text='<%# DataBinder.Eval(Container.DataItem,"SeverityNameOn") %>' Runat="server" ></asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:DropDownList ID="cboSensorActionsOn" DataSource='<%# dsAlarmSeverity %>' DataValueField="SeverityId" DataTextField="SeverityName" SelectedIndex='<%# GetAlarmSeverity(Convert.ToInt32(Eval("AlarmLevelOn"))) %>' Runat="server" >
														</asp:DropDownList>
													</EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action Off - Name">
                                                    <ItemTemplate>
														<asp:Label ID="lblSensorActionOff" text='<%# Eval("ActionOff") %>' Runat=server Width="75px"></asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:TextBox ID="txtSensorActionOff" Runat=server Text='<%# Eval("ActionOff") %>' Width="75px"></asp:TextBox>
													</EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action Off - Severity">
                                                    <ItemTemplate>
													    <asp:Label ID="lblSensorActionsOff" text='<%# Eval("SeverityNameOff") %>' Runat="server"></asp:Label>
													</ItemTemplate>
													<EditItemTemplate>
														<asp:DropDownList ID="cboSensorActionsOff" DataSource='<%# dsAlarmSeverity %>' DataValueField="SeverityId" DataTextField="SeverityName" SelectedIndex='<%# GetAlarmSeverity(Convert.ToInt32(Eval("alarmLevelOff"))) %>' Runat="server" >
													    </asp:DropDownList>
													</EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:CommandField ButtonType="Image" CancelImageUrl="~/images/Cancel.gif" EditImageUrl="~/images/Edit.gif"
                                                    ShowEditButton="True" UpdateImageUrl="~/images/OK.gif" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <div id="fuelSensor" style="border:solid 1px green; width: 700px; position:relative; top: 40px">
                                    </div>
                                    <div id="statusSensors" style="border:solid 1px green; width: 700px; position:relative; top: 60px">
                                    </div>
                                </div>
						    </div>
					    </div>
				    </div>
				</div>
			</div>
		</div>
	</form>
	</body>
</html>
