<%@ Page language="c#" Inherits="SentinelFM.frmFleetUsers" CodeFile="frmFleetUsers.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register TagPrefix="uc1" TagName="ctlFleetUsers" Src="Components/ctlFleetUsers.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ctlOrganizationHierarchyFleetUsers" Src="Components/ctlOrganizationHierarchyFleetUsers.ascx" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>frmFleetUsers</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<!--<LINK href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
  </HEAD>
	<body>
		<form id="frmFleetUsersForm" method="post" runat="server">
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
																					<asp:button id="cmdEmails" runat="server" Text="Email Addresses" CssClass="confbutton" CausesValidation="False"
																						CommandName="3" Width="117px" onclick="cmdEmails_Click" meta:resourcekey="cmdEmailsResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetMng" runat="server" Text="Fleet Management" CssClass="confbutton" CausesValidation="False"
																						CommandName="9" Width="141px" onclick="cmdFleetMng_Click" meta:resourcekey="cmdFleetMngResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetVehicle" runat="server" Text="Fleet-Vehicle Assignment" CssClass="confbutton"
																						CausesValidation="False" CommandName="7" Width="168px" onclick="cmdFleetVehicle_Click" meta:resourcekey="cmdFleetVehicleResource1"></asp:button></TD>
																				<TD><asp:button id="cmdFleetUsers" runat="server" Text="Fleet-User Assignment" CssClass="selectedbutton"
																						CausesValidation="False" CommandName="15" Width="160px" meta:resourcekey="cmdFleetUsersResource1"></asp:button></TD>
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
																					<TABLE id="Table7" class=table HEIGHT="500px" WIDTH="960px"
																						border="0">
                                                                                        <tr id="trFleetSelectOption" visible="false" runat="server"><td align="center" class="configTabBackground">
                                                                                            <table class="formtext" runat="server" id="optBaseTable"  cellpadding=0 cellspacing=0  >
                                                                                                <tr>
                                                                                                    <td> 
                                                                                                        <asp:Label ID="Label11" runat="server" Text="Based On:" 
                                                                                                            meta:resourcekey="Label11Resource1"></asp:Label> </td>
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
                                                                                        </td></tr>
																						<TR id="trBasedOnNormalFleet" runat="server">
																							<TD align="center" class="configTabBackground">
																								<uc1:ctlFleetUsers id="CtlFleetUsers1" runat="server" ></uc1:ctlFleetUsers></TD>
																						</TR>
                                                                                        <tr id="trBasedOnHierarchyFleet" visible="false" runat="server">
                                                                                            <TD align="center" class="configTabBackground">
																								<uc2:ctlOrganizationHierarchyFleetUsers id="CtlFleetUsers2" runat="server" ></uc2:ctlOrganizationHierarchyFleetUsers></TD>
                                                                                        </tr>
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
		
		</form>
	</body>
</HTML>
