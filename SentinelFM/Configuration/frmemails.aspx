<%@ Page language="c#" Inherits="SentinelFM.frmEmails" CodeFile="frmEmails.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>frmEmails</title>
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
        
    function onOrganizationHierarchyNodeCodeClick()
    {
        var mypage='../../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
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
		<form id="Form1" method="post" runat="server">

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
																				<TD><asp:button id="cmdEmails" runat="server" Width="117px" CausesValidation="False" CssClass="selectedbutton"
																						Text="Email Addresses" CommandName="3" meta:resourcekey="cmdEmailsResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetMng" runat="server" Text="Fleet Management" CssClass="confbutton" CausesValidation="False"
																						Width="141px" CommandName="9" onclick="cmdFleetMng_Click" meta:resourcekey="cmdFleetMngResource1"></asp:button></TD>
																				<TD style="WIDTH: 166px">
																					<asp:button id="cmdFleetVehicle" runat="server" Text="Fleet-Vehicle Assignment" CssClass="confbutton"
																						CausesValidation="False" Width="168px" CommandName="7" onclick="cmdFleetVehicle_Click" meta:resourcekey="cmdFleetVehicleResource1"></asp:button></TD>
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
																	<TD >
																		<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="657" align="center" border="0">
																			<TR>
																				<TD>
																					<TABLE id="Table7" class=table WIDTH="960px" HEIGHT="500px"
																						border="0">
																						<TR>
																							<TD class="configTabBackground" valign=top  >
																								<TABLE id="Table1" style="WIDTH: 679px; HEIGHT: 444px" cellSpacing="0" cellPadding="0"
																									width="679" align="center" border="0">
																									<TR>
																										<TD class="tableheading" align="left" height="5" style="width: 669px"></TD>
																									</TR>
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
																									<tr id="trBasedOnNormalFleet" runat="server">
																										<TD style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: gray; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: gray; BORDER-TOP-COLOR: gray; HEIGHT: 30px; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: gray; width: 669px;"
																											align="center" colSpan="1">
																											<TABLE id="Table3" cellSpacing="0" cellPadding="0" align="center" border="0">
																												<TR>
																													<TD><asp:label id="lblFleet" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Fleet:"></asp:label></TD>
																													<TD><asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" AutoPostBack="True" DataValueField="FleetId"
																															DataTextField="FleetName" onselectedindexchanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1"></asp:dropdownlist></TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
                                                                                                    <tr id="trBasedOnHierarchyFleet" visible="false" runat="server">
                                                                                                        <TD style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: gray; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: gray; BORDER-TOP-COLOR: gray; HEIGHT: 30px; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: gray; width: 669px;"
																											align="center" colSpan="1">
																											<TABLE id="Table11" cellSpacing="0" cellPadding="0" align="center" border="0">
																												<tr>
                                                                                                                      <td class="style1">
                                                                                                                            <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" 
                                                                                                                                Text=" Hierarchy Node:" meta:resourcekey="lblOhTitleResource1"  /></td>
                                                                                                                     <td>
                                                                                                                            <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                                                                                                                CssClass="combutton" Width="200px" 
                                                                                                                                OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                                                                                                meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                                                                                                     </td>
                                                                                                                  </tr>
																											</TABLE>
																										</TD>
                                                                                                    </tr>
																									<TR>
																										<TD style="WIDTH: 669px; HEIGHT: 217px" align="center">
																											<TABLE id="Table4" style="HEIGHT: 210px"  cellSpacing="0" cellPadding="0" border="0">
																												<TR>
																													<TD><asp:datagrid id="dgEmails" runat="server" Visible="False" PageSize="7" AllowPaging="True" DataKeyField="Email"
																															AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px"
																															BackColor="White" GridLines="None" CellSpacing="1" meta:resourcekey="dgEmailsResource1" OnDeleteCommand="dgEmails_DeleteCommand" OnCancelCommand="dgEmails_CancelCommand" OnEditCommand="dgEmails_EditCommand" OnUpdateCommand="dgEmails_UpdateCommand" OnPageIndexChanged="dgEmails_PageIndexChanged" OnItemCreated="dgEmails_ItemCreated">
																															<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
																															<AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
																															<ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
																															<HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
																															<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
																															<Columns>
																																<asp:TemplateColumn Visible="False" HeaderText="EmailOriginal">
																																	<ItemTemplate>
																																		<asp:Label Visible=False ID="lblEmailOriginal" text='<%# DataBinder.Eval(Container.DataItem,"Email") %>' Runat=server Width=75px meta:resourcekey="lblEmailOriginalResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox Visible=False ID="txtEmailOriginal" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"Email") %>' Width=75px meta:resourcekey="txtEmailOriginalResource1"></asp:TextBox>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='<%$ Resources:EmailAddressHeader %>'>
																																	<ItemTemplate>
																																		<asp:Label ID="LabelEmail" text='<%# DataBinder.Eval(Container.DataItem,"Email") %>' Runat=server Width=200px meta:resourcekey="LabelEmailResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox ID="txtEmail" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"Email") %>' Width=200px meta:resourcekey="txtEmailResource1"></asp:TextBox>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																
																																
																																
																																
																																
																																<asp:TemplateColumn HeaderText='<%$ Resources:TimeZoneHeader %>'>
																																	<ItemStyle Wrap="False"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="lblTimeZone" text='<%# DataBinder.Eval(Container.DataItem,"TimeZoneName") %>' Runat=server meta:resourcekey="lblTimeZoneResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:DropDownList ID="cboTimeZone" DataSource='<%# dsTimeZone %>' DataValueField="TimeZoneId" DataTextField="TimeZoneName" SelectedIndex='<%# GetTimeZone(Convert.ToInt16(DataBinder.Eval(Container.DataItem,"TimeZone"))) %>' Runat=server meta:resourcekey="cboTimeZoneResource1">
																																		</asp:DropDownList>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='<%$ Resources:DaylightHeader %>' >
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkDaylight1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "AutoAdjustDayLightSaving")) %>' runat="server" meta:resourcekey="chkDaylight1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkDaylight" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "AutoAdjustDayLightSaving")) %>' runat="server" meta:resourcekey="chkDaylightResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='<%$ Resources:CriticalHeader %>'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkCritical1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Critical")) %>' runat="server" meta:resourcekey="chkCritical1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkCritical" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Critical")) %>' runat="server" meta:resourcekey="chkCriticalResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='<%$ Resources:WarningHeader %>'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkWarning1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Warning")) %>' runat="server" meta:resourcekey="chkWarning1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkWarning" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Warning")) %>' runat="server" meta:resourcekey="chkWarningResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='<%$ Resources:NotifyHeader %>'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkNotify1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Notify")) %>' runat="server" meta:resourcekey="chkNotify1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkNotify" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Notify")) %>' runat="server" meta:resourcekey="chkNotifyResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>


                                                                                                                                <asp:TemplateColumn HeaderText='<%$ Resources:MaintenanceHeader %>'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkMaintenance" Enabled="False" 
                                                                                                                                            Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Maintenance")) %>' 
                                                                                                                                            runat="server" meta:resourcekey="chkMaintenanceResource1"/>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkMaintenance" 
                                                                                                                                            Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Maintenance")) %>' 
                                                                                                                                            runat="server" meta:resourcekey="chkMaintenanceResource2"  />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
                                                                                                                                
                                                                                                                                <asp:TemplateColumn HeaderText='<%$ Resources:MessageForwardHeader %>'>
<HeaderStyle Width="20px"></HeaderStyle>
<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
<ItemTemplate>
<asp:CheckBox ID="chkDriverMessage" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DriverMessage")) %>'
runat="server" meta:resourcekey="chkMessageForwardResource2" />
</ItemTemplate>
<EditItemTemplate>
<asp:CheckBox ID="chkDriverMessage" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DriverMessage")) %>'
runat="server" meta:resourcekey="chkMessageForwardResource2" />
</EditItemTemplate>
</asp:TemplateColumn>
                                                                                                                                <asp:TemplateColumn HeaderText='<%$ Resources:AutosubscriptionHeader %>'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkAutosubscription1" Enabled="False" 
                                                                                                                                            Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "autosubscription")) %>' 
                                                                                                                                            runat="server" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkAutosubscription" 
                                                                                                                                            Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "autosubscription")) %>' 
                                                                                                                                            runat="server"  />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>

                                                                                                                                <asp:TemplateColumn HeaderText='<%$ Resources:ReminderHeader %>'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkReminder1" Enabled="False" 
                                                                                                                                            Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Reminder")) %>' 
                                                                                                                                            runat="server" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkReminder" 
                                                                                                                                            Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Reminder")) %>' 
                                                                                                                                            runat="server"  />
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
																													<TD align="center" height="20"></TD>
																												</TR>
																												<TR>
																													<TD align="center">
                                                                                                                        <asp:button id="cmdAddEmail" runat="server" Width="127px" CssClass="combutton" Text="Add Email Address"
																															CommandName="23" onclick="cmdAddEmail_Click" meta:resourcekey="cmdAddEmailResource1"></asp:button>
                                                                                                                        <asp:Button ID="btnExportFleetEmailReport" runat="server" CssClass="combutton" onclick="Button1_Click" Text="Export" />

																													</TD>

																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
																									<TR>
																										<TD style="WIDTH: 669px;" align="center">
																											<TABLE id="tblEmailAdd" cellSpacing="0" cellPadding="0" border="0" runat="server">
																												<TR>
																													<TD class="formtext" valign="top">
                                                                                                                        <asp:Label ID="lblEmailAddr" runat="server" Text="Email Address:" meta:resourcekey="lblEmailAddrResource1"></asp:Label><asp:regularexpressionvalidator id="valEmail" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
																															ErrorMessage="Please enter a correct email address" ControlToValidate="txtNewEmail" meta:resourcekey="valEmailResource1" Text="*"></asp:regularexpressionvalidator><asp:requiredfieldvalidator id="valReqEmail" runat="server" Width="1px" ErrorMessage="Please enter an email address"
																															ControlToValidate="txtNewEmail" meta:resourcekey="valReqEmailResource1" Text="*"></asp:requiredfieldvalidator></TD>
																													<TD valign="top"><asp:textbox id="txtNewEmail" runat="server" Width="261px" CssClass="formtext" meta:resourcekey="txtNewEmailResource1"></asp:textbox></TD>
																													<TD style="WIDTH: 35px"></TD>
																													<TD valign="top"><asp:button id="cmdSaveEmail" runat="server" CssClass="combutton" Text="Save" onclick="cmdSaveEmail_Click" meta:resourcekey="cmdSaveEmailResource1"></asp:button></TD>
																												</TR>
																												<TR>
																													<TD class="formtext" valign="top">
                                                                                                                        <asp:Label ID="lblPhone" runat="server" Text="Phone:" Visible="False" 
                                                                                                                            meta:resourcekey="lblPhoneResource1"></asp:Label>
                                                                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                                                                                            ControlToValidate="txtPhone" CssClass="formtext" 
                                                                                                                            ErrorMessage="Invalid Phone Number:" 
                                                                                                                            
                                                                                                                            
                                                                                                                            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$" 
                                                                                                                            Enabled="False" meta:resourcekey="RegularExpressionValidator1Resource1">*</asp:RegularExpressionValidator>
                                                                                                                    </TD>
																													<TD valign="top">
                                                                                                                        <asp:textbox id="txtPhone" runat="server" Width="261px" 
                                                                                                                            CssClass="formtext" Enabled="False" Visible="False" 
                                                                                                                            meta:resourcekey="txtPhoneResource1"></asp:textbox></TD>
																													<TD style="WIDTH: 35px">&nbsp;</TD>
																													<TD valign="top">&nbsp;</TD>
																												</TR>
																												<TR>
																													<TD class="formtext" align="left" valign="top">
                                                                                                                        <asp:Label ID="lblTimeZone" runat="server" Text="Time Zone:" meta:resourcekey="lblTimeZoneResource2"></asp:Label></TD>
																													<TD align="left" valign="top"><asp:dropdownlist id="cboTimeZoneAdd" runat="server" Width="261px" CssClass="RegularText" DataValueField="TimeZoneId"
																															DataTextField="TimeZoneName" meta:resourcekey="cboTimeZoneAddResource1"></asp:dropdownlist></TD>
																													<TD style="WIDTH: 35px"></TD>
																													<TD valign="top"><asp:button id="cmdCancelAddEmal" runat="server" CausesValidation="False" CssClass="combutton"
																															Text="Cancel" onclick="cmdCancelAddEmal_Click" meta:resourcekey="cmdCancelAddEmalResource1"></asp:button></TD>
																												</TR>
																												<TR>
																													<TD class="formtext" align="center"></TD>
																													<TD colspan="3" align="left" valign="top">
																														<TABLE id="Table9" cellSpacing="0" cellPadding="0" border="0">
																															<TR>
																																<TD></TD>
																															</TR>
																															<TR>
																																<TD><asp:checkbox id="chkAddDayLight" runat="server" CssClass="formtext" Text="Automatically adjust for daylight savings time" meta:resourcekey="chkAddDayLightResource1"></asp:checkbox></TD>
																															</TR>
																															<TR>
																																<TD class="formtext" style="height: 10px;"></TD>
																															</TR>
																															<TR>
																																<TD class="formtext">&nbsp;<asp:Label ID="lblEmailNotification" runat="server" meta:resourcekey="lblEmailNotificationResource1"
                                                                                                                                        Text="Email notification in case of:"></asp:Label></TD>
																															</TR>
																															<TR>
																																<TD>
																																	<TABLE class="formtext" id="Table10" cellSpacing="0" cellPadding="0" border="0">
																																		<TR>
																																			<TD><asp:checkbox id="chkAddCritical" runat="server" Text="Critical Alarm" meta:resourcekey="chkAddCriticalResource1"></asp:checkbox></TD>
																																			<TD><asp:checkbox id="chkAddWarning" runat="server" Text="Warning Alarm" meta:resourcekey="chkAddWarningResource1"></asp:checkbox></TD>
																																			<TD><asp:checkbox id="chkAddNotify" runat="server" Text="Notify Alarm" meta:resourcekey="chkAddNotifyResource1"></asp:checkbox></TD>
                                                                                                                                            <TD><asp:checkbox id="chkMaintenance" runat="server" Text="Maintenance" meta:resourcekey="chkAddNotifyMaintenance1" ></asp:checkbox></TD>
																																		</TR>
                                                                                                                                        <tr>
<td>
<asp:CheckBox ID="chkDriverMessage" runat="server" 
   Text="Driver Message Forward to Email" 
   meta:resourcekey="chkAddMessageForwardResource1">
</asp:CheckBox>
</td>
<td><asp:CheckBox ID="chkAddAutoSubscription" runat="server" 
   Text="Auto Subscription" 
   meta:resourcekey="chkAddAutoSubscriptionResource1">
</asp:CheckBox></td>
<td><asp:CheckBox ID="chkAddReminder" runat="server" 
   Text="Reminder" 
   meta:resourcekey="chkAddReminderResource1">
</asp:CheckBox></td>
<td></td>
</tr>
																																	</TABLE>
																																</TD>
																															</TR>
																														</TABLE>
																													</TD>
																													<%--<TD style="WIDTH: 35px"></TD>
																													<TD></TD>--%>
																												</TR>
																											</TABLE>
																											<asp:label id="lblMessage" runat="server" Width="270px" CssClass="errortext" Visible="False"
																												Height="8px" meta:resourcekey="lblMessageResource1"></asp:label><asp:validationsummary id="ValidationSummary1" runat="server" Width="332px" Height="23px" meta:resourcekey="ValidationSummary1Resource1"></asp:validationsummary>
																											<TABLE id="Table8" style="WIDTH: 100%; HEIGHT: 64px" cellSpacing="1" cellPadding="1" width="701"
																												border="0">
																												<TR>
																													<TD class="formtext" align="left"><U>&nbsp;<asp:Label ID="lblNote" runat="server" Font-Bold="True" meta:resourcekey="lblNoteResource1"
                                                                                                                            Text="Note:"></asp:Label></U><BR>
                                                                                                                        <asp:Label ID="lblNoteDesc" runat="server" meta:resourcekey="lblNoteDescResource1"
                                                                                                                            Text="In order to receive email notifications on alarms, you must enter a valid email address. In addition, you have to select the type of severity, that is Critical, Warning or Notify."></asp:Label></TD>
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
					</TD>
				</TR>
			</TABLE>
		
		</form>
	</body>
</HTML>
