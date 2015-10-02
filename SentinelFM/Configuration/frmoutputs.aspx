<%@ Page language="c#" Inherits="SentinelFM.frmOutputs" CodeFile="frmOutputs.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<title>frmOutputs</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<!--<LINK href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
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
		<FORM id="frmEmails" method="post" runat="server">
        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

        <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" 
            style="display:none;" AutoPostBack="True"
                    OnClick="hidOrganizationHierarchyPostBack_Click" 
            meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />

			<TABLE id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicles" />
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
																					<asp:button id="cmdVehicleInfo" runat="server" CausesValidation="False" CssClass="confbutton"
																						Text="Vehicle Info" Width="112px" CommandName="4" onclick="cmdVehicleInfo_Click" meta:resourcekey="cmdVehicleInfoResource1"></asp:button>
																				</TD>
																				<TD><asp:button id="cmdAlarms" runat="server" Text="Sensors/Alarms/Messages" CssClass="confbutton"
																						CausesValidation="False" Width="176px" CommandName="5" onclick="cmdAlarms_Click" meta:resourcekey="cmdAlarmsResource1"></asp:button></TD>
																				<TD><asp:button id="cmdOutputs" runat="server" Text="Outputs" CssClass="selectedbutton" CausesValidation="False"
																						CommandName="6" meta:resourcekey="cmdOutputsResource1"></asp:button></TD>
																				<TD><asp:button id="cmdFleetVehicle" runat="server" CausesValidation="False" CssClass="confbutton"
																						Text="Fleet-Vehicle Assignment" Width="168px" CommandName="7" onclick="cmdFleetVehicle_Click" meta:resourcekey="cmdFleetVehicleResource1"></asp:button></TD>
																				<TD>
                                                                                <asp:Button ID="btnEquipmentAssignment" runat="server" Width="168px" CommandName="77"  CausesValidation="False"
                                                                                    CssClass="confbutton" Text="Equipment Assignment" PostBackUrl="~/Configuration/Equipment/frmAssignmentList.aspx"
                                                                                    meta:resourcekey="btnEquipmentAssignmentResource1"></asp:Button>

                                                                                </TD>
                                                                            <td>
                                                                                <asp:Button ID="btnFuelCategory" runat="server" Width="168px" Visible= "false" CausesValidation="False" CssClass="confbutton"
                                                                                     Text="Fuel Category" PostBackUrl="~/Configuration/FuelCategory/frmFuelCategory.aspx"
                                                                                    meta:resourcekey="btnFuelCategoryResource1"></asp:Button>

                                                                            </td>
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
																						cellSpacing="0" cellPadding="0" border="0">
																						<TR>
																							<TD class="configTabBackground">
																								<TABLE id="Table1" style="WIDTH: 679px; HEIGHT: 444px" cellSpacing="0" cellPadding="0"
																									width="679" align="center" border="0">
																									<TR>
																										<TD class="tableheading" align="left" height="5" style="width: 692px"></TD>
																									</TR>
                                                                                                    <tr id="trFleetSelectOption" visible="false" runat="server">
                                                                                                        <td align="center" class="configTabBackground" height="30">
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
																									<TR>
																										<TD style="BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; HEIGHT: 30px; width: 832px;" align="center">
																											<TABLE id="Table4" style="WIDTH: 771px; HEIGHT: 25px" cellSpacing="0" cellPadding="0" width="631"
																												align="center" border="0">
																												<TR>
																													<TD style="WIDTH: 296px; HEIGHT: 17px" align="right"><asp:label id="lblFleet" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Fleet:"></asp:label></TD>
																													<TD style="WIDTH: 219px; HEIGHT: 17px">
                                                                                                                        <asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" DataTextField="FleetName" DataValueField="FleetId"
																															DESIGNTIMEDRAGDROP="79" AutoPostBack="True" Width="200px" onselectedindexchanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1"></asp:dropdownlist>
                                                                                                                        <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" Visible="false"
                                                                                                                                CssClass="combutton" Width="200px" 
                                                                                                                                OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                                                                                                meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                                                                                                    </TD>
																													<TD style="WIDTH: 18px; HEIGHT: 17px" width="18"></TD>
																													<TD style="WIDTH: 106px; HEIGHT: 17px"><asp:label id="lblVehicleName" runat="server" CssClass="formtext" Width="13px" Visible="False" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:"></asp:label></TD>
																													<TD style="WIDTH: 93px; HEIGHT: 17px"><asp:dropdownlist id="cboVehicle" runat="server" CssClass="RegularText" DataTextField="Description"
																															DataValueField="LicensePlate" DESIGNTIMEDRAGDROP="79" AutoPostBack="True" Visible="False" Width="247px" onselectedindexchanged="cboVehicle_SelectedIndexChanged" meta:resourcekey="cboVehicleResource1"></asp:dropdownlist></TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
																									<TR>
																										<TD style="WIDTH: 692px; HEIGHT: 217px;padding-left:150px;" align="center"><asp:datagrid id="dgOutputs" runat="server" Width="614px" BorderStyle="Ridge" AllowPaging="True"
																												DataKeyField="OutputId" AutoGenerateColumns="False" BorderColor="White" CellSpacing="1" BorderWidth="2px" BackColor="White" CellPadding="3"
																												GridLines="None" PageSize="8" OnCancelCommand="dgOutputs_CancelCommand" OnEditCommand="dgOutputs_EditCommand" OnPageIndexChanged="dgOutputs_PageIndexChanged" OnUpdateCommand="dgOutputs_UpdateCommand" meta:resourcekey="dgOutputsResource1">
																												<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
																												<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
																												<AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
																												<ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
																												<HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
																												<Columns>
																													<asp:BoundColumn DataField="OutputId" ReadOnly="True" HeaderText="<%$ Resources:dgOutput_OP %>"  >
																														<HeaderStyle Width="50px"></HeaderStyle>
																													</asp:BoundColumn>
																													<asp:TemplateColumn HeaderText="<%$ Resources:dgOutput_Output %>" >
																														<HeaderStyle Width="300px"></HeaderStyle>
																														<ItemTemplate>
																															<asp:Label ID="lblOutputName" text='<%# DataBinder.Eval(Container.DataItem,"OutputName") %>' Runat=server Width=75px meta:resourcekey="lblOutputNameResource1"></asp:Label>
																														</ItemTemplate>
																														<EditItemTemplate>
																															<asp:TextBox ID="txtOutputName" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"OutputName") %>' Width=75px meta:resourcekey="txtOutputNameResource1"></asp:TextBox>
																														</EditItemTemplate>
																													</asp:TemplateColumn>
																													<asp:TemplateColumn HeaderText="<%$ Resources:dgOutput_ActionOn %>">
																														<ItemTemplate>
																															<asp:Label ID="lblOutputActionOn" text='<%# DataBinder.Eval(Container.DataItem,"ActionOn") %>' Runat=server Width=75px meta:resourcekey="lblOutputActionOnResource1"></asp:Label>
																														</ItemTemplate>
																														<EditItemTemplate>
																															<asp:TextBox ID="txtOutputActionOn" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"ActionOn") %>' Width=75px meta:resourcekey="txtOutputActionOnResource1"></asp:TextBox>
																														</EditItemTemplate>
																													</asp:TemplateColumn>
																													<asp:TemplateColumn HeaderText="<%$ Resources:dgOutput_ActionOff %>">
																														<ItemTemplate>
																															<asp:Label ID="lblOutputActionOff" text='<%# DataBinder.Eval(Container.DataItem,"ActionOff") %>' Runat=server Width=75px meta:resourcekey="lblOutputActionOffResource1"></asp:Label>
																														</ItemTemplate>
																														<EditItemTemplate>
																															<asp:TextBox ID="txtOutputActionOff" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"ActionOff") %>' Width=75px meta:resourcekey="txtOutputActionOffResource1"></asp:TextBox>
																														</EditItemTemplate>
																													</asp:TemplateColumn>
																													<asp:EditCommandColumn UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;"
																														EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource1"></asp:EditCommandColumn>
																												</Columns>
																												<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
																											</asp:datagrid>
																											<asp:label id="lblMessage" runat="server" CssClass="errortext" Width="270px" Visible="False"
																												Height="8px" meta:resourcekey="lblMessageResource1"></asp:label></TD>
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
			&nbsp;
		</FORM>
		<DIV></DIV>
	</body>
</HTML>
