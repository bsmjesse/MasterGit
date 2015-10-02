<%@ Page language="c#" Inherits="SentinelFM.Admin.frmReports" CodeFile="frmReports.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Reports</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form method="post" runat="server">
			<TABLE id="tblMain" style="Z-INDEX: 101; LEFT: 161px; POSITION: absolute; TOP: 95px" cellSpacing="0"
				cellPadding="0" width="546" border="0" runat="server">
				<TR>
					<TD class="heading" style="WIDTH: 88px; HEIGHT: 8px" align="right">Report:&nbsp;&nbsp;
					</TD>
					<TD class="heading" style="WIDTH: 203px; HEIGHT: 8px"><asp:dropdownlist id="cboReports" runat="server" AutoPostBack="True" Width="203px" CssClass="RegularText" onselectedindexchanged="cboReports_SelectedIndexChanged">
							
							<asp:ListItem Value="2">Box air usage report</asp:ListItem>
							<asp:ListItem Value="3">User Login report</asp:ListItem>
							<asp:ListItem Value="4">Exception Usage Report</asp:ListItem>
							<asp:ListItem Value="5">Map Usage Report</asp:ListItem>
							<asp:ListItem Value="6">Network Latency Report</asp:ListItem>
						</asp:dropdownlist></TD>
					<TD class="heading" style="WIDTH: 39px; HEIGHT: 8px"></TD>
					<TD class="heading" style="HEIGHT: 8px"></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 88px"></TD>
					<TD style="WIDTH: 203px"></TD>
					<TD style="WIDTH: 39px"></TD>
					<TD></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px; HEIGHT: 29px" align="right">
						<asp:Label id="lblOrganization" runat="server">Organization:</asp:Label>&nbsp;&nbsp;
					</TD>
					<TD style="WIDTH: 203px; HEIGHT: 29px"><asp:dropdownlist id="cboOrganization" runat="server" Width="203px" CssClass="RegularText" DataTextField="OrganizationName"
							DataValueField="OrganizationId" AutoPostBack="True" onselectedindexchanged="cboOrganization_SelectedIndexChanged"></asp:dropdownlist></TD>
					<TD style="WIDTH: 39px; HEIGHT: 29px"></TD>
					<TD style="HEIGHT: 29px"></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px" align="right">
						<asp:Label id="lblFrom" runat="server">From:</asp:Label>&nbsp;&nbsp;
					</TD>
					<TD style="WIDTH: 203px"><asp:textbox id="txtFrom" runat="server" Width="127px" CssClass="RegularText" ReadOnly="True"></asp:textbox><asp:dropdownlist id="cboHoursFrom" runat="server" Width="74px" CssClass="RegularText" Height="14px"></asp:dropdownlist></TD>
					<TD class="formtext" style="WIDTH: 39px">
						<asp:Label id="lblTo" runat="server">To:</asp:Label></TD>
					<TD align="center"><asp:textbox id="txtTo" runat="server" Width="127px" CssClass="RegularText" ReadOnly="True"></asp:textbox><asp:dropdownlist id="cboHoursTo" runat="server" Width="74px" CssClass="RegularText" Height="14px"></asp:dropdownlist></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px" align="right"></TD>
					<TD style="WIDTH: 203px"><asp:radiobuttonlist id="optRange" runat="server" AutoPostBack="True" CssClass="formtext" RepeatDirection="Horizontal" onselectedindexchanged="optRange_SelectedIndexChanged">
							<asp:ListItem Value="1" Selected="True">Any period</asp:ListItem>
							<asp:ListItem Value="2">Monthly</asp:ListItem>
						</asp:radiobuttonlist></TD>
					<TD class="formtext" style="WIDTH: 39px"></TD>
					<TD align="center"></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px; HEIGHT: 22px"></TD>
					<TD style="WIDTH: 204px; HEIGHT: 22px" align="center">
						<TABLE class="formtext" id="tblMonth" style="WIDTH: 100%; HEIGHT: 20px" cellSpacing="0"
							cellPadding="0" border="0" runat="server">
							<TR>
								<TD align="left"><asp:dropdownlist id="cboMonth" runat="server" Width="143px" CssClass="RegularText">
										<asp:ListItem Value="1" Selected="True">January</asp:ListItem>
										<asp:ListItem Value="2">February</asp:ListItem>
										<asp:ListItem Value="3">March</asp:ListItem>
										<asp:ListItem Value="4">April</asp:ListItem>
										<asp:ListItem Value="5">May</asp:ListItem>
										<asp:ListItem Value="6">June</asp:ListItem>
										<asp:ListItem Value="7">July</asp:ListItem>
										<asp:ListItem Value="8">August</asp:ListItem>
										<asp:ListItem Value="9">September</asp:ListItem>
										<asp:ListItem Value="10">October</asp:ListItem>
										<asp:ListItem Value="11">November</asp:ListItem>
										<asp:ListItem Value="12">December</asp:ListItem>
									</asp:dropdownlist></TD>
								<TD align="left"><asp:dropdownlist id="cboYear" runat="server" Width="56px" CssClass="RegularText">
										<asp:ListItem Value="2003">2003</asp:ListItem>
										<asp:ListItem Value="2004">2004</asp:ListItem>
										<asp:ListItem Value="2005">2005</asp:ListItem>
										<asp:ListItem Value="2006">2006</asp:ListItem>
										<asp:ListItem Value="2007">2007</asp:ListItem>
										<asp:ListItem Value="2008">2008</asp:ListItem>
										<asp:ListItem Value="2009">2009</asp:ListItem>
										<asp:ListItem Value="2010">2010</asp:ListItem>
									</asp:dropdownlist></TD>
							</TR>
						</TABLE>
					</TD>
					<TD style="WIDTH: 39px; HEIGHT: 22px"></TD>
					<TD style="HEIGHT: 22px" align="center"></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px"></TD>
					<TD style="WIDTH: 203px"><asp:calendar id="cldFrom" runat="server" Width="204px" Height="162px" CellSpacing="1" BorderColor="Black"
							BorderStyle="Solid" Font-Bold="True" Font-Size="11px" NextPrevFormat="ShortMonth" onselectionchanged="cldFrom_SelectionChanged">
							<TodayDayStyle ForeColor="White" BackColor="#999999"></TodayDayStyle>
							<DayStyle BackColor="#CCCCCC"></DayStyle>
							<NextPrevStyle Font-Size="10px" Font-Bold="True" ForeColor="white"></NextPrevStyle>
							<DayHeaderStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="#333333"></DayHeaderStyle>
							<SelectedDayStyle ForeColor="white" BackColor="#009933"></SelectedDayStyle>
							<TitleStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="white" BackColor="#009933"></TitleStyle>
							<OtherMonthDayStyle ForeColor="#999999"></OtherMonthDayStyle>
						</asp:calendar></TD>
					<TD style="WIDTH: 39px"></TD>
					<TD align="center"><asp:calendar id="cldTo" runat="server" Width="205px" Height="162px" CellSpacing="1" BorderColor="Black"
							BorderStyle="Solid" Font-Bold="True" Font-Size="11px" NextPrevFormat="ShortMonth" onselectionchanged="cldTo_SelectionChanged">
							<TodayDayStyle ForeColor="White" BackColor="#999999"></TodayDayStyle>
							<DayStyle BackColor="#CCCCCC"></DayStyle>
							<NextPrevStyle Font-Size="10px" Font-Bold="True" ForeColor="white"></NextPrevStyle>
							<DayHeaderStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="#333333"></DayHeaderStyle>
							<SelectedDayStyle ForeColor="white" BackColor="#009933"></SelectedDayStyle>
							<TitleStyle Font-Size="10px" Font-Bold="True" Height="10px" ForeColor="white" BackColor="#009933"></TitleStyle>
							<OtherMonthDayStyle ForeColor="#999999"></OtherMonthDayStyle>
						</asp:calendar></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px; HEIGHT: 15px"></TD>
					<TD style="WIDTH: 203px; HEIGHT: 15px">
						<TABLE id="tblBoxes" style="BORDER-RIGHT: #009933 1px outset; BORDER-TOP: #009933 1px outset; BORDER-LEFT: #009933 1px outset; WIDTH: 197px; BORDER-BOTTOM: #009933 1px outset"
							cellSpacing="0" cellPadding="0" border="0" runat="server">
							<TR>
								<TD>
									<asp:radiobuttonlist id="optSystemUsage" runat="server" CssClass="formtext" Width="168px" AutoPostBack="True"
										RepeatDirection="Horizontal" onselectedindexchanged="optSystemUsage_SelectedIndexChanged">
										<asp:ListItem Value="1" Selected="True">By Box</asp:ListItem>
										<asp:ListItem Value="2">By Phone</asp:ListItem>
									</asp:radiobuttonlist></TD>
							</TR>
							<TR>
								<TD>
									<TABLE class="formtext" id="Table1" style="WIDTH: 196px; HEIGHT: 43px" cellSpacing="0"
										cellPadding="0" width="196" border="0">
										<TR>
											<TD style="WIDTH: 14px"></TD>
											<TD style="WIDTH: 104px">Box Id:</TD>
											<TD style="WIDTH: 120px">
												<asp:textbox id="txtBoxId" runat="server" Width="107px"></asp:textbox></TD>
											<TD></TD>
										</TR>
										<TR>
											<TD style="WIDTH: 14px"></TD>
											<TD style="WIDTH: 104px">
												<asp:label id="lblPhone" runat="server" Visible="False">Phone:</asp:label></TD>
											<TD style="WIDTH: 120px">
												<asp:textbox id="txtPhone" runat="server" Width="107px" Visible="False"></asp:textbox></TD>
											<TD align="right"></TD>
										</TR>
										<TR>
											<TD style="WIDTH: 14px"></TD>
											<TD style="WIDTH: 104px"></TD>
											<TD style="WIDTH: 120px">
												<asp:button id="cmdFind" runat="server" CssClass="combutton" Width="47px" Text="Find"></asp:button></TD>
											<TD align="right"></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
							<TR>
								<TD>
									<asp:listbox id="lstBoxes" runat="server" CssClass="formtext" Width="195px" AutoPostBack="True"
										DataValueField="BoxId" DataTextField="Description" Height="72px" Visible="False"></asp:listbox></TD>
							</TR>
						</TABLE>
						<TABLE id="tblFleets" style="BORDER-RIGHT: 0px outset; BORDER-TOP: 0px outset; BORDER-LEFT: 0px outset; WIDTH: 197px; BORDER-BOTTOM: 0px outset"
							cellSpacing="0" cellPadding="0" border="0" runat="server">
							<TR>
								<TD></TD>
							</TR>
							<TR>
								<TD>
									<TABLE class="formtext" id="Table3" style="WIDTH: 196px; HEIGHT: 43px" cellSpacing="0"
										cellPadding="0" width="196" border="0">
										<TR>
											<TD style="WIDTH: 14px" height="20"></TD>
											<TD style="WIDTH: 104px" height="20"></TD>
											<TD style="WIDTH: 120px" height="20"></TD>
											<TD height="20"></TD>
										</TR>
										<TR>
											<TD style="WIDTH: 14px; HEIGHT: 9px"></TD>
											<TD style="WIDTH: 104px; HEIGHT: 9px">
												<asp:Label id="lblComFleets" runat="server">Fleets:</asp:Label></TD>
											<TD style="WIDTH: 120px; HEIGHT: 9px">
												<asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" Width="140px" AutoPostBack="True"
													DataValueField="FleetId" DataTextField="FleetName" onselectedindexchanged="cboFleet_SelectedIndexChanged"></asp:dropdownlist></TD>
											<TD style="HEIGHT: 9px"></TD>
										</TR>
										<TR>
											<TD style="WIDTH: 14px; HEIGHT: 13px"></TD>
											<TD style="WIDTH: 104px; HEIGHT: 13px">
												<asp:Label id="lblComVehicles" runat="server">Vehicles:</asp:Label></TD>
											<TD style="WIDTH: 120px; HEIGHT: 13px">
												<asp:dropdownlist id="cboVehicle" runat="server" CssClass="RegularText" Width="140px" AutoPostBack="True"
													DataValueField="VehicleId" DataTextField="Description"></asp:dropdownlist></TD>
											<TD style="HEIGHT: 13px" align="right"></TD>
										</TR>
										<TR>
											<TD style="WIDTH: 14px"></TD>
											<TD style="WIDTH: 104px"></TD>
											<TD style="WIDTH: 120px"></TD>
											<TD align="right"></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
							<TR>
								<TD align="center">
									<asp:listbox id="lstCommMode" runat="server" CssClass="formtext" Width="195px" DataValueField="CommModeId"
										DataTextField="CommModeName" Height="72px" SelectionMode="Multiple"></asp:listbox></TD>
							</TR>
						</TABLE>
					</TD>
					<TD style="WIDTH: 39px; HEIGHT: 15px"></TD>
					<TD style="HEIGHT: 15px" align="center"></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px; HEIGHT: 15px"></TD>
					<TD style="WIDTH: 203px; HEIGHT: 15px"><asp:label id="lblMessage" runat="server" CssClass="errortext"></asp:label></TD>
					<TD style="WIDTH: 39px; HEIGHT: 15px"></TD>
					<TD style="HEIGHT: 15px" align="center"></TD>
				</TR>
				<TR>
					<TD class="formtext" style="WIDTH: 88px"></TD>
					<TD style="WIDTH: 207px" align="center"></TD>
					<TD style="WIDTH: 39px"></TD>
					<TD align="center">
						<INPUT class="combutton" id="cmdClose" onclick="window.close()" type="button" value="Close"
							name="cmdClose">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:button id="cmdPreview" onmouseover="javascript:this.style.backgroundColor='#FFFFFF'; &#13;&#10;&#9;&#9;&#9;&#9;&#9;&#9;javascript:this.style.color='#FFCC00';&#13;&#10;&#9;&#9;&#9;&#9;&#9;&#9;"
							tabIndex="4" runat="server" CssClass="combutton" Width="94px" Text="Preview" onclick="cmdPreview_Click"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
