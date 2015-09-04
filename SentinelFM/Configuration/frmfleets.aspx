<%@ Page language="c#" Inherits="SentinelFM.frmFleets" CodeFile="frmFleets.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>frmFleets</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<!--<LINK href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
  </HEAD>
	<body>
	<form id="Form1" method="post" runat="server">
			<TABLE id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdFleets"  />
					</TD>
				</TR>
				<TR>
					<TD>
						<TABLE id="tblBody" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<TR>
								<TD>
									<TABLE id="tblForm" class=table HEIGHT="550px"
										width="990" border="0">
										<TR>
											<TD class="configTabBackground">
												<TABLE id="Table2" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<TR>
														<TD>
															<TABLE id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<TR>
																	<TD>
																		<TABLE id="Table5" style="Z-INDEX: 101; WIDTH: 190px; POSITION: relative; TOP: 0px; HEIGHT: 22px"
																			cellSpacing="0" cellPadding="0" border="0">
																			<TR>
																				<TD>
																					<asp:button id="cmdEmails" runat="server" Width="117px" CausesValidation="False" CssClass="confbutton"
																						Text="Email Addresses" CommandName="3" onclick="cmdEmails_Click" meta:resourcekey="cmdEmailsResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetMng" runat="server" Text="Fleet Management" CssClass="selectedbutton"
																						CausesValidation="False" Width="141px" CommandName="9" meta:resourcekey="cmdFleetMngResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetVehicle" runat="server" Text="Fleet-Vehicle Assignment" CssClass="confbutton"
																						CausesValidation="False" Width="160px" CommandName="7" onclick="cmdFleetVehicle_Click" meta:resourcekey="cmdFleetVehicleResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetUsers" runat="server" CommandName="15" Text="Fleet-User Assignment" CssClass="confbutton"
																						CausesValidation="False" Width="160px" onclick="cmdFleetUsers_Click" meta:resourcekey="cmdFleetUsersResource1"></asp:button></TD>
                                                                                 <TD>
																					<asp:button id="cmdDriverSkill" runat="server" CommandName="16" Text="Driver-Skill Assignment" CssClass="confbutton"
																						CausesValidation="False" Width="160px" onclick="cmdDriverSkill_Click" meta:resourcekey="cmdDriverSkillResource1"></asp:button></TD>
																			</TR>
																		</TABLE>
																	</TD>
																</TR>
																<TR>
																	<TD>
																		<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="617" align="center" border="0">
																			<TR>
																				<TD>
																					<TABLE id="Table7" class=table WIDTH="960px" HEIGHT="500px"
																						border="0">
																						<TR>
																							<TD class="configTabBackground" valign="top">
																								<TABLE id="Table1" style="WIDTH: 679px; HEIGHT: 444px" cellSpacing="0" cellPadding="0"
																									width="679" align="center" border="0">
																									<TR>
																										<TD class="tableheading" align="left" height="5"></TD>
																									</TR>
                                                                                                    <TR>
																										<TD style="WIDTH: 100%; HEIGHT: auto" align="center"> 
                                                                                                          
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
                                                                                                                   <asp:ListItem  Value="0" meta:resourcekey="ListItemResource0" Text="Fleet"></asp:ListItem>
                                                                                                                   <asp:ListItem  Value="1" meta:resourcekey="ListItemResource1" Text="Description"></asp:ListItem>
                                                                                                                  
                                                                                                                
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
                                                                                                            </TD>
                                                                                                        </TR>
																									<TR>
																										<TD style="WIDTH: 100%; HEIGHT: 217px" align="center">

																											<TABLE id="Table4" style="HEIGHT: 210px" height="210" cellSpacing="0" cellPadding="0" border="0">
																												<TR>
																													<TD>
																														<asp:datagrid id="dgFleets" runat="server" PageSize="7" AllowPaging="True" DataKeyField="FleetId"
																															AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px"
																															BackColor="White" GridLines="None" CellSpacing="1" meta:resourcekey="dgFleetsResource1" OnCancelCommand="dgFleets_CancelCommand" OnDeleteCommand="dgFleets_DeleteCommand" OnEditCommand="dgFleets_EditCommand" OnPageIndexChanged="dgFleets_PageIndexChanged" OnUpdateCommand="dgFleets_UpdateCommand" OnItemCreated="dgFleets_ItemCreated">
																															<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
																															<AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
																															<ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
																															<HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
																															<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
																															<Columns>
																																<asp:TemplateColumn HeaderText='<%$ Resources:dgFleets_Fleet %>'>
																																	<ItemTemplate>
																																		<asp:Label ID="lblFleetName" text='<%# DataBinder.Eval(Container.DataItem,"FleetName") %>' Runat=server Width=200px meta:resourcekey="lblFleetNameResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox ID="txtFleetName" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"FleetName") %>' Width=200px meta:resourcekey="txtFleetNameResource1"></asp:TextBox>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='<%$ Resources:dgFleets_Description %>'>
																																	<ItemTemplate>
																																		<asp:Label ID="lblDescription" text='<%# DataBinder.Eval(Container.DataItem,"Description") %>' Runat=server Width=200px meta:resourcekey="lblDescriptionResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox ID="txtDescription" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"Description") %>' Width=200px meta:resourcekey="txtDescriptionResource1"></asp:TextBox>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:EditCommandColumn UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;" 
 EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource1"></asp:EditCommandColumn>
																																<asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
																															</Columns>
																															<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
																														</asp:datagrid></TD>
																												</TR>
																												<TR>
																													<TD align="center" height="15"></TD>
																												</TR>
																												<TR>
																													<TD align="center" style="height: 4px">
																														<asp:button id="cmdAddFleet" runat="server" Width="115px" CssClass="combutton" Text="Add Fleet"
																															CommandName="10" onclick="cmdAddFleet_Click" meta:resourcekey="cmdAddFleetResource1"></asp:button></TD>
																												</TR>
																											</TABLE>
																											<asp:label id="lblMessage" runat="server" CssClass="errortext" Width="270px" Height="8px" Visible="False" meta:resourcekey="lblMessageResource1"></asp:label>
																										</TD>
																									</TR>
																									<TR>
																										<TD align="center">
																											<TABLE id="tblFleetAdd" cellSpacing="0" cellPadding="0" border="0" runat="server">
																												<TR>
																													<TD class="formtext" align="right">
                                                                                                                        <asp:requiredfieldvalidator id="valReqFleet" runat="server" ControlToValidate="txtNewFleetName"
																															ErrorMessage="Please enter a fleet." meta:resourcekey="valReqFleetResource1" Text="*"></asp:requiredfieldvalidator>
                                                                                                                        <asp:Label ID="lblFleetName" runat="server" meta:resourcekey="lblFleetNameResource2"
                                                                                                                            Text="Fleet Name:"></asp:Label></TD>
																													<TD>
																														<asp:textbox id="txtNewFleetName" runat="server" CssClass="formtext" Width="261px" MaxLength="50" meta:resourcekey="txtNewFleetNameResource1"></asp:textbox></TD>
																													<TD style="WIDTH: 35px;"></TD>
																													<TD>
																														<asp:button id="cmdSaveFleet" runat="server" Text="Save" CssClass="combutton" onclick="cmdSaveFleet_Click" meta:resourcekey="cmdSaveFleetResource1"></asp:button></TD>
																												</TR>
																												<TR>
																													<TD class="formtext" align="right">
                                                                                                                        <asp:Label ID="lblDesc" runat="server" meta:resourcekey="lblDescResource1" Text="Description:"></asp:Label></TD>
																													<TD align="left">
																														<asp:textbox id="txtNewFleetDescription" runat="server" CssClass="formtext" Width="261px" meta:resourcekey="txtNewFleetDescriptionResource1"></asp:textbox></TD>
																													<TD style="WIDTH: 35px;"></TD>
																													<TD>
																														<asp:button id="cmdCancelAddFleet" runat="server" Text="Cancel" CssClass="combutton" CausesValidation="False" onclick="cmdCancelAddFleet_Click" meta:resourcekey="cmdCancelAddFleetResource1"></asp:button></TD>
																												</TR>
																												<tr>
																												    <td colspan="4">
																												    </td>
																												</tr>
																												<%--<TR>
																													<TD class="formtext" style="WIDTH: 116px" align="center"></TD>
																													<TD style="WIDTH: 337px" align="left"></TD>
																													<TD style="WIDTH: 35px"></TD>
																													<TD style="WIDTH: 109px"></TD>
																												</TR>--%>
                                                                                                                <tr>
                                                                                                            <td colspan="2" class="formtext">
                                                                                                                <asp:label ID="Label1" runat="server" text="Assign vehicles to the new fleet (Excel 2003 .xls):"></asp:label>                                                                                                            
                                                                                                            </td>
                                                                                                            <td colspan="2" class="formtext">
                                                                                                                    <asp:FileUpload ID="newFleetVehicleAssignment" runat="server" 
                                                                                                                    CssClass="RegularText" Width="200px" style="margin: 20px 0px 20px 0px;" />
                                                                                                            </td>
                                                                                                        </tr>
																												<TR>
																													<TD></TD>
																													<TD colspan="3">
																														<asp:validationsummary id="ValidationSummary1" runat="server" meta:resourcekey="ValidationSummary1Resource1"></asp:validationsummary></TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
																									<TR>
																										<TD style="WIDTH: 100%; HEIGHT: 40px" align="left" class="formtext">
																											<P><U><B></B></U>&nbsp;</P>
																										</TD>
																									</TR>
																								</TABLE>
																							</TD>
																						</TR>
																					</TABLE>
																				</TD>
																			</TR>
																		</TABLE>
																	</TD>
																</TR>
															</TABLE>
														</TD>
													</TR>
												</TABLE>
											</TD>
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
