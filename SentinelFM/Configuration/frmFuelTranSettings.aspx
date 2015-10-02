<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmFuelTranSettings.aspx.cs" Inherits="SentinelFM.Configuration_frmFuelTranSettings" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register src="Components/ctlOrganizationSubMenuTabs.ascx" tagname="ctlOrganizationSubMenuTabs" tagprefix="ucSubmenu" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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
		<form id="Form1" method="post" runat="server">
			<TABLE id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdOrganization" HasOrgCommandName="false" />
					</TD>
				</TR>
				<TR>
					<TD>
						<TABLE id="tblBody" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<TR>
								<TD>
									<TABLE id="tblForm" class=table HEIGHT="550px"
										width="1060" border="0">
										<TR>
											<TD class="configTabBackground">
												<TABLE id="Table2" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<TR>
														<TD>
															<TABLE id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<TR>
																	<TD>
																		<ucSubmenu:ctlOrganizationSubMenuTabs ID="ctlSubMenuTabs" runat="server" SelectedControl="cmdFuel" />
																	</TD>
																</TR>
																<TR>
																	<TD  >
																		<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="657" align="center" border="0">
																			<TR>
																				<TD>
																					<TABLE id="Table7" class=table WIDTH="1030px" HEIGHT="500px"
																						border="0">
																						<TR>
																							<TD class="configTabBackground" valign=middle align=center    >
                                                                                                &nbsp;<table class="formtext" width="500">
                                                                                                                    <tr>
                                                                                                                        <td style="width: 100px" align="left">
                                                                                                                            <asp:Label ID="lblNotificationEmailAddress" runat="server" Text="Notification Email Address:"
                                                                                                                                Width="200px" meta:resourcekey="lblNotificationEmailAddressResource1"></asp:Label></td>
                                                                                                                        <td style="width: 100px">
                                                                                                                            <asp:TextBox ID="txtNotificationEmailAddress" runat="server" CssClass="formtext" Width="225px" meta:resourcekey="txtNotificationEmailAddressResource1"></asp:TextBox></td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td style="width: 100px; height: 18px;" align="left">
                                                                                                                            <asp:Label ID="lblGPSRadius" runat="server" Text="GPS Radius:" Width="200px" meta:resourcekey="lblGPSRadiusResource1"></asp:Label>(Meters)</td>
                                                                                                                        <td style="width: 100px; height: 18px;">
                                                                                                                            <asp:TextBox ID="txtGPSRadius" runat="server" CssClass="formtext" Width="225px" meta:resourcekey="txtGPSRadiusResource1"></asp:TextBox></td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td style="width: 100px" align="left">
                                                                                                                            <asp:Label ID="lblMaximumReportingInterval" runat="server" Text="Maximum Reporting Interval:"
                                                                                                                                Width="200px" meta:resourcekey="lblMaximumReportingIntervalResource1"></asp:Label>(Min)</td>
                                                                                                                        <td style="width: 100px">
                                                                                                                            <asp:TextBox ID="txtMaximumReportingInterval" runat="server" CssClass="formtext" Width="225px" meta:resourcekey="txtMaximumReportingIntervalResource1"></asp:TextBox></td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td style="width: 100px" align="left">
                                                                                                                            <asp:Label ID="lblHistoryTimeRange" runat="server" Text="History Time Range:" Width="200px" meta:resourcekey="lblHistoryTimeRangeResource1"></asp:Label>(Min)</td>
                                                                                                                        <td style="width: 100px">
                                                                                                                            <asp:TextBox ID="txtHistoryTimeRange" runat="server" CssClass="formtext" Width="225px" meta:resourcekey="txtHistoryTimeRangeResource1"></asp:TextBox></td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td style="width: 100px" align="left">
                                                                                                                            <asp:Label ID="lblWaitingPeriodToGetMessages" runat="server" Text="Waiting Period To Get Messages:"
                                                                                                                                Width="200px" meta:resourcekey="lblWaitingPeriodToGetMessagesResource1"></asp:Label>(Hrs)</td>
                                                                                                                        <td style="width: 100px">
                                                                                                                            <asp:TextBox ID="txtWaitingPeriodToGetMessages" runat="server" CssClass="formtext" Width="225px" meta:resourcekey="txtWaitingPeriodToGetMessagesResource1"></asp:TextBox></td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td style="width: 100px; height: 21px;" align="left">
                                                                                                                            <asp:Label ID="lblTimeZone" runat="server" Text="Timezone:" Width="200px" meta:resourcekey="lblTimeZoneResource1"></asp:Label></td>
                                                                                                                        <td style="width: 100px; height: 21px;">
                                                                                                                            <asp:DropDownList ID="cboTimeZone" runat="server" CssClass="RegularText" meta:resourcekey="cboTimeZoneResource1"
                                                                                                                                Width="225px">
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource1" Text="GMT-12 Eniwetok,Kwajalein"
                                                                                                                                    Value="-12"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource2" Text="GMT-11 Midway Island" Value="-11"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource3" Text="GMT-10 Hawaii" Value="-10"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource4" Text="GMT-9 Alaska" Value="-9"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource5" Text="GMT-8 Pacific Time (USA&amp;Canada)"
                                                                                                                                    Value="-8"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource6" Text="GMT-7 Mountain Time (USA&amp;Canada)"
                                                                                                                                    Value="-7"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource7" Text="GMT-6 Central Time (USA&amp;Canada)"
                                                                                                                                    Value="-6"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource8" Text="GMT-5 Eastern Time (USA&amp;Canada)"
                                                                                                                                    Value="-5"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource9" Text="GMT-4 Atlantic Time (Canada)"
                                                                                                                                    Value="-4"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource10" Text="GMT-3 Brasilia,Greenland"
                                                                                                                                    Value="-3"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource11" Text="GMT-2 Mid-Atlantic" Value="-2"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource12" Text="GMT-1 Azores,Cape Verde Is."
                                                                                                                                    Value="-1"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource13" Selected="True" Text="GMT Dublin,London"
                                                                                                                                    Value="0"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource14" Text="GMT+1 Amsterdam,Berlin,Bern,Rome"
                                                                                                                                    Value="1"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource15" Text="GMT+2 Jerusalem,Riga,Tallinn"
                                                                                                                                    Value="2"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource16" Text="GMT+3 Moscow,St. Petersburg"
                                                                                                                                    Value="3"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource17" Text="GMT+4 Abu Dhabi,Baku,Tbilisi"
                                                                                                                                    Value="4"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource18" Text="GMT+5 Islamabad,Karachi"
                                                                                                                                    Value="5"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource19" Text="GMT+6 Astana,Dhaka" Value="6"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource20" Text="GMT+7 Bangkok,Hanoi,Jakarta"
                                                                                                                                    Value="7"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource21" Text="GMT+8 Beijing,Hong Kong"
                                                                                                                                    Value="8"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource22" Text="GMT+9 Osaka,Tokyo,Seoul"
                                                                                                                                    Value="9"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource23" Text="GMT+10 Sydney,Melbourne"
                                                                                                                                    Value="10"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource24" Text="GMT+11 Magadan" Value="11"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource25" Text="GMT+12 Wellington,Fiji"
                                                                                                                                    Value="12"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourceKey="ListItemResource26" Text="GMT+13 Nuku'alofa" Value="13"></asp:ListItem>
                                                                                                                            </asp:DropDownList></td>
                                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="center" colspan="2">
                                                                                                            <asp:Label ID="lblMessage" runat="server" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="width: 100px">
                                                                                                        </td>
                                                                                                        <td style="width: 100px">
                                                                                                            <asp:Button ID="cmdUpdate" runat="server" CssClass="combutton" meta:resourcekey="cmdSaveFleetResource1"
                                                                                                                OnClick="cmdSave" Text="Update" /></td>
                                                                                                    </tr>
                                                                                                                </table>
                                                                                                                
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
