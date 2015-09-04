<%@ Page language="c#" Inherits="SentinelFM.Admin.frmFWResults" CodeFile="frmFWResults.aspx.cs" %>

<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>FW Results</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
		
	</HEAD>
	<body>
		<FORM id="Form1" method="post" runat="server">
			<TABLE id="Table2" style="WIDTH: 712px; HEIGHT: 163px; border-right: gray 4px double; border-top: gray 4px double; border-left: gray 4px double; border-bottom: gray 4px double;"
				cellSpacing="0" cellPadding="0" border="0">
				<TR>
					<TD vAlign="top" align="center" style="HEIGHT: 149px; width: 570px;">
						<TABLE id="Table6"
							cellSpacing="0" cellPadding="0" width="520" border="0">
							<TR>
								<TD vAlign="middle" align="center" style="height: 192px">
									<TABLE class="formtext" id="Table1" style="WIDTH: 212px; HEIGHT: 83px" cellSpacing="0"
										cellPadding="0" width="212" border="0">
										<TR>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
										</TR>
										<TR>
											<TD height="5"></TD>
											<TD height="5">
												<asp:label id="lblOrganization" runat="server">Organization:</asp:label></TD>
											<TD height="5">
												<asp:dropdownlist id="cboOrganization" runat="server" DataTextField="OrganizationName" DataValueField="OrganizationId"
													AutoPostBack="True" Width="203px" CssClass="RegularText" Enabled="False" onselectedindexchanged="cboOrganization_SelectedIndexChanged"></asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD></TD>
											<TD></TD>
										</TR>
										<TR>
											<TD style="HEIGHT: 21px"></TD>
											<TD style="HEIGHT: 21px">
												<asp:label id="lblFleets" runat="server">Fleet:</asp:label></TD>
											<TD style="HEIGHT: 21px">
												<asp:dropdownlist id="cboFleet" runat="server" DataTextField="FleetName" DataValueField="FleetId"
													AutoPostBack="True" Width="203px" CssClass="RegularText" onselectedindexchanged="cboFleet_SelectedIndexChanged"></asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD></TD>
											<TD></TD>
										</TR>
										<TR>
											<TD style="HEIGHT: 1px"></TD>
											<TD style="HEIGHT: 1px">
												<asp:label id="lblVehicles" runat="server">Vehicles:</asp:label></TD>
											<TD style="HEIGHT: 1px">
												<asp:dropdownlist id="cboVehicle" runat="server" DataTextField="Description" DataValueField="VehicleId"
													AutoPostBack="True" Width="203px" CssClass="RegularText" onselectedindexchanged="cboVehicle_SelectedIndexChanged"></asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD height="10"></TD>
											<TD height="10"></TD>
											<TD></TD>
										</TR>
										<TR>
											<TD height="10"></TD>
											<TD height="10">Result within</TD>
											<TD height="10">
												<asp:dropdownlist id="cboDays" runat="server" CssClass="RegularText" Width="203px" AutoPostBack="True">
													<asp:ListItem Value="2" Selected="True">2 Hours</asp:ListItem>
													<asp:ListItem Value="12">12 Hours</asp:ListItem>
													<asp:ListItem Value="24">1 Day</asp:ListItem>
													<asp:ListItem Value="48">2 Days</asp:ListItem>
													<asp:ListItem Value="168">7 Days</asp:ListItem>
                                       <asp:ListItem Value="240">10 Days</asp:ListItem>
                                       <asp:ListItem Value="288">12 Days</asp:ListItem>
                                       <asp:ListItem Value="336">14 Days</asp:ListItem>
												</asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD align="center" colSpan="3" height="20"></TD>
										</TR>
										<TR>
											<TD colspan="3" align="right">
												<asp:button id="cmdView" runat="server" CssClass="combutton" Text="View" onclick="cmdView_Click"></asp:button></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
                  <busyboxdotnet:busybox id="BusyBox1" runat="server"></busyboxdotnet:busybox>
					</TD>
				</TR>
				<TR>
					<TD vAlign="top" align="center" style="width: 570px">
						<TABLE id="tblFW" cellSpacing="0" cellPadding="0" border="0" runat="server" style="width: 121%">
							<TR>
								<TD style="width: 100%; height: 317px">
									<TABLE id="Table3"cellSpacing="0" cellPadding="0"
										border="0">
										<TR>
											<TD style="height: 5px"></TD>
											<TD align="center" style="height: 5px"></TD>
										</TR>
										<TR>
											<TD style="HEIGHT: 20px" height="20"></TD>
											<TD style="HEIGHT: 20px; width: 100%;" align="center" height="20">
												<asp:button id="cmdRefresh" runat="server" CssClass="combutton" Text="Refresh Data" onclick="cmdRefresh_Click"></asp:button>&nbsp;&nbsp;&nbsp;
												<asp:button id="cmdFWUpdate" runat="server" CssClass="combutton" Text="FW Update" onclick="cmdFWUpdate_Click"></asp:button>&nbsp; &nbsp;<asp:Button ID="cmdSubmitToCS" runat="server" CssClass="combutton"
                                                    OnClick="cmdSubmitToCS_Click" Text="Submit to CS System" Width="130px" Visible="False" />&nbsp;&nbsp;
                                                <asp:Button ID="cmdSetAllSensors" runat="server"
                                                            CssClass="combutton" OnClick="cmdSetAllSensors_Click" Text="Select All" Width="92px" Visible="False" />&nbsp; &nbsp;<asp:Button
                                                        ID="cmdUnselectAllSensors" runat="server" CssClass="combutton" OnClick="cmdUnselectAllSensors_Click"
                                                        Text="Deselect All" Width="92px" Visible="False" /></TD>
										</TR>
										<TR>
											<TD style="HEIGHT: 20px" height="20"></TD>
											<TD style="HEIGHT: 20px" align="center" height="20"></TD>
										</TR>
										<TR>
											<TD vAlign="top" align="center" colSpan="2">
												<asp:datagrid id="dgData" runat="server" AutoGenerateColumns="False" ForeColor="Black" CellPadding="4"
													BackColor="White" BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE" Width="100%" OnSelectedIndexChanged="dgData_SelectedIndexChanged" OnItemDataBound="dgData_ItemDataBound">
													<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
													<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
													<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
													<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="Black" BackColor="White"></ItemStyle>
													<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
														VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>

													<Columns>
													   <asp:TemplateColumn HeaderText="">
															<HeaderStyle ></HeaderStyle>
															<ItemStyle HorizontalAlign="Center" ></ItemStyle>
															<ItemTemplate>
																<asp:CheckBox ID="chkBox" Enabled="True" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBox") %>' Visible=false runat="server" />
															</ItemTemplate>
														</asp:TemplateColumn>
														
														<asp:BoundColumn DataField="BoxID" HeaderText="Box ID"  ></asp:BoundColumn>
														<asp:BoundColumn DataField="Description" ReadOnly="True" HeaderText="Description "></asp:BoundColumn>
														<asp:BoundColumn DataField="FwName" ReadOnly="True" HeaderText="Firmware"></asp:BoundColumn>
														<asp:BoundColumn DataField="DateTime" HeaderText="Date"></asp:BoundColumn>
														<asp:BoundColumn DataField="Acknowledged" HeaderText="Status"></asp:BoundColumn>
                                                      <asp:ButtonColumn CommandName="Select" Text="Cancel OTA"></asp:ButtonColumn>
													</Columns>
													<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
                                       <EditItemStyle BackColor="#999999" />
												</asp:datagrid></TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="40">
                                                <asp:Label ID="lblMessage" runat="server" CssClass="formtext"></asp:Label></TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="40"></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		</FORM>
	</body>
</HTML>
