<%@ Page language="c#" Inherits="SentinelFM.Admin.frmFWUpdate" CodeFile="frmFWUpdate.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>FW Update</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	    <style type="text/css">
            .style1
            {
                width: 100%;
            }
        </style>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table2" style="WIDTH: 709px; HEIGHT: 163px; border-right: gray 4px double; border-top: gray 4px double; border-left: gray 4px double; border-bottom: gray 4px double;"
				cellSpacing="0" cellPadding="0" border="0">
				<TR>
					<TD align="center" vAlign="top">
						<TABLE id="Table6"
							cellSpacing="0" cellPadding="0" width="541" border="0">
							<TR>
								<TD vAlign="middle" align="center" style="height: 153px">
									<TABLE class="formtext" id="Table1" style="WIDTH: 288px; HEIGHT: 130px" cellSpacing="0"
										cellPadding="0" width="288" border="0">
										<TR>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
										</TR>
										<TR>
											<TD height="5"></TD>
											<TD height="5"><asp:label id="lblOrganization" runat="server">Organization:</asp:label></TD>
											<TD height="5"><asp:dropdownlist id="cboOrganization" runat="server" DataTextField="OrganizationName" DataValueField="OrganizationId"
													AutoPostBack="True" Width="203px" CssClass="RegularText" Enabled="False" onselectedindexchanged="cboOrganization_SelectedIndexChanged"></asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD></TD>
											<TD></TD>
										</TR>
										<TR>
											<TD style="HEIGHT: 21px"></TD>
											<TD style="HEIGHT: 21px"><asp:label id="lblFleets" runat="server">Fleet:</asp:label></TD>
											<TD style="HEIGHT: 21px"><asp:dropdownlist id="cboFleet" runat="server" DataTextField="FleetName" DataValueField="FleetId"
													AutoPostBack="True" Width="203px" CssClass="RegularText" onselectedindexchanged="cboFleet_SelectedIndexChanged"></asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD></TD>
											<TD></TD>
										</TR>
										<TR>
											<TD style="HEIGHT: 1px"></TD>
											<TD style="HEIGHT: 1px"><asp:label id="lblVehicles" runat="server">Vehicles:</asp:label></TD>
											<TD style="HEIGHT: 1px"><asp:dropdownlist id="cboVehicle" runat="server" DataTextField="Description" DataValueField="VehicleId"
													AutoPostBack="True" Width="203px" CssClass="RegularText" onselectedindexchanged="cboVehicle_SelectedIndexChanged"></asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD height="10"></TD>
											<TD height="10"></TD>
											<TD height="10"></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD></TD>
											<TD align="right">
												<asp:button id="cmdView" runat="server" CssClass="combutton" Text="View" onclick="cmdView_Click"></asp:button></TD>
										</TR>
									</TABLE>
                                    </TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD vAlign="top" align="center">
						<TABLE id="tblFW" cellSpacing="0" cellPadding="0" border="0" runat="server" style="width: 605px">
							<TR>
								<TD style="width: 533px; height: 546px">
									<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0">
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="76" style="HEIGHT: 76px; width: 587px;">
												<TABLE id="Table4" cellSpacing="0" cellPadding="0" border="0" style="width: 103%; height: 89px">
													<TR>
														<TD align="center" colSpan="2" vAlign="middle">
															<TABLE id="Table5" style="WIDTH: 84%; HEIGHT: 40px" cellSpacing="0" cellPadding="0"
																border="0">
																<TR>
																	<TD class="formtext" style="WIDTH: 100%" align="center">
                                                      <fieldset>
                                                      <table>
                                                         <tr>
                                                            <td style="width: 100px">
																		<asp:button id="cmdUnselectAllSensors" runat="server" CssClass="combutton" Width="92px" Text="Deselect All" onclick="cmdUnselectAllSensors_Click"></asp:button></td>
                                                            <td style="width: 100px">
																		<asp:button id="cmdSetAllSensors" runat="server" CssClass="combutton" Width="92px" Text="Select All" onclick="cmdSetAllSensors_Click"></asp:button></td>
                                                            <td style="width: 100px">
                                                               <asp:Button ID="cmdGetBoxFirmware" runat="server" CssClass="combutton" OnClick="cmdGetBoxFirmware_Click"
                                                         Text="Send Box Status Cmd" Width="146px" /></td>
                                                            <td style="width: 100px">
                                                               <asp:Button ID="cmdRefreshBoxFirmware" runat="server" CssClass="combutton" 
                                                         Text="Get Box Status Results" Width="144px" OnClick="cmdRefreshBoxFirmware_Click" /></td>
                                                           <td>
												<asp:button id="cmdUpdate" runat="server" CssClass="combutton" Width="151px" Text="Update" onclick="cmdUpdate_Click"></asp:button></td>
                                                           <td>
												<asp:button id="cmdViewResults" runat="server" CssClass="combutton" Width="151px" Text="View Results" onclick="cmdViewResults_Click"></asp:button></td>
                                                         </tr>
                                                      </table>
                                                      </fieldset> 
                                                   </TD>
																</TR>
                                                                <tr>
                                                                    <td align="left" class="formtext" height="20" >
                                                                        <fieldset>
                                                                        <table >
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="Label1" runat="server" CssClass="formtext" 
                                                                                        Text="Filter Old Firmware:"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtFirmwareFilter" runat="server" Width="200px" 
                                                                                        CssClass="formtext"></asp:TextBox>
                                                                                </td>
                                                                                <td>
												<asp:button id="cmdFilterRecords" runat="server" CssClass="combutton" Width="151px" Text="Filter" 
                                                                                        onclick="cmdFilterRecords_Click"></asp:button>
                                                                                &nbsp;<asp:button id="cmdClearFilter" runat="server" CssClass="combutton" 
                                                                                        Width="151px" Text="Clear Filter" 
                                                                                        onclick="cmdClearFilter_Click"></asp:button>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        </fieldset> 
                                                                    </td>
                                                                </tr>
																<TR>
																    <td>
																    <fieldset>
																    <table style="WIDTH: 100%">
																        <tr>
																            <TD class="formtext" style="WIDTH: 237px" height="10" align="left">Update&nbsp;all selected Boxes&nbsp;to 
																		Firmware:</TD>
																            
																        </tr>
																        <TR>
																	<TD class="formtext" style="WIDTH: 100%">
																		<asp:dropdownlist id="cboFirmware" runat="server" CssClass="RegularText" Width="100%" AutoPostBack="True"
																			DataValueField="FwId" DataTextField="FwName_Port" onselectedindexchanged="cboFirmware_SelectedIndexChanged"></asp:dropdownlist></TD>
																		
																
																    </table>
																    	</fieldset>
																	</td>
																</TR>
																
															</TABLE>
														</TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
										<TR>
											<TD vAlign="top" align="center" colSpan="2" style="HEIGHT: 370px; width: 587px;"><asp:datagrid id="dgData" runat="server" CellPadding="4" ForeColor="#333333" AutoGenerateColumns="False"
													DataKeyField="BoxID" onselectedindexchanged="dgData_SelectedIndexChanged" GridLines="None" PageSize="9999">
													<FooterStyle ForeColor="White" BackColor="#5D7B9D" Font-Bold="True"></FooterStyle>
													<SelectedItemStyle Font-Bold="True" ForeColor="#333333" BackColor="#E2DED6"></SelectedItemStyle>
													<AlternatingItemStyle BackColor="White" ForeColor="#284775"></AlternatingItemStyle>
													<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="#333333" BackColor="#F7F6F3"></ItemStyle>
													<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="White"
														VerticalAlign="Middle" BackColor="#5D7B9D"></HeaderStyle>
													<Columns>
														<asp:BoundColumn Visible="False" DataField="BoxId" HeaderText="BoxId"></asp:BoundColumn>
														<asp:TemplateColumn HeaderText="Update">
															<HeaderStyle Width="20px"></HeaderStyle>
															<ItemStyle HorizontalAlign="Center" Width="20px"></ItemStyle>
															<ItemTemplate>
																<asp:CheckBox ID="chkBox" Enabled="True" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBox") %>' runat="server" />
															</ItemTemplate>
														</asp:TemplateColumn>
														<asp:BoundColumn DataField="BoxId" HeaderText="BoxId" ReadOnly="True"></asp:BoundColumn>
														<asp:BoundColumn DataField="Description" ReadOnly="True" HeaderText="Description "></asp:BoundColumn>
														<asp:BoundColumn DataField="FwName" ReadOnly="True" HeaderText="Old Firmware"></asp:BoundColumn>
														<asp:TemplateColumn HeaderText="New Firmware">
															<ItemStyle Wrap="False" Width="100px"></ItemStyle>
															<ItemTemplate>
																<asp:Label ID="lblNewFirmware" text='<%#  DataBinder.Eval(Container.DataItem,"NewFirmware") %>' Runat=server>
																</asp:Label>
															</ItemTemplate>
															<EditItemTemplate>
																<asp:DropDownList ID="cboNewFirmware" DataSource='<%# dsFirmware%>' DataValueField="FwId" DataTextField="FwName" SelectedIndex='<%# GetFirmware(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"NewFirmwareId")))%>' Runat=server>
																</asp:DropDownList>
															</EditItemTemplate>
														</asp:TemplateColumn>
														<asp:TemplateColumn HeaderText="Scheduled">
															<HeaderStyle Width="20px"></HeaderStyle>
															<ItemStyle HorizontalAlign="Center" Width="20px"></ItemStyle>
															<ItemTemplate>
																<asp:CheckBox ID="chkBoxScheduled" Enabled="True" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBoxScheduled") %>' runat="server" />
															</ItemTemplate>
															
															<EditItemTemplate>
																<asp:CheckBox ID="chkBoxScheduled" Enabled="True" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBoxScheduled") %>' runat="server" />
															</EditItemTemplate>
															
														</asp:TemplateColumn>
														
														<asp:BoundColumn DataField="DateTimeReceived" ReadOnly="True" HeaderText="DateTime"></asp:BoundColumn>
														<asp:BoundColumn DataField="BoxFirmwareStatus" ReadOnly="True" HeaderText="Get Box Status Flag"></asp:BoundColumn>
														<asp:BoundColumn DataField="BoxFirmware" ReadOnly="True" HeaderText="Current Firmware"></asp:BoundColumn>
														<asp:BoundColumn DataField="OAPPort" ReadOnly="True" HeaderText="Port"></asp:BoundColumn>
														<asp:EditCommandColumn UpdateText="&lt;img src=images/ok.gif border=0&gt;" CancelText="&lt;img src=images/cancel.gif border=0&gt;"
															EditText="&lt;img src=images/edit.gif border=0&gt;"></asp:EditCommandColumn>
													</Columns>
													<PagerStyle HorizontalAlign="Right" ForeColor="White" BackColor="#284775" Mode="NumericPages"></PagerStyle>
                                  
												</asp:datagrid></TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="23" style="HEIGHT: 23px; width: 587px;">
												&nbsp;
												</TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="40" style="width: 587px">
												<asp:label id="lblMessage" runat="server" CssClass="regulartext" ForeColor="Red"></asp:label></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
