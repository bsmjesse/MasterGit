<%@ Page language="c#" Inherits="SentinelFM.Admin.frmDiagNotValidGPS" CodeFile="frmDiagNotValidGPS.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Diagnostic Results</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 19px; POSITION: absolute; TOP: 28px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD class="formtext" style="HEIGHT: 13px">&nbsp;
						<asp:Label id="lblNotValidGPS" runat="server" Visible="False"> Not Valid GPS Statistic:</asp:Label></TD>
				</TR>
				<TR>
					<TD>
						<asp:datagrid id="dgNotValidGPS" runat="server" AutoGenerateColumns="False" Width="626px" BorderColor="#DEDFDE"
							BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4" ForeColor="Black">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Left" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Left" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="BoxId" HeaderText="Box"></asp:BoundColumn>
								<asp:BoundColumn DataField="LicensePlate" HeaderText="License Plate"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Vehicle"></asp:BoundColumn>
								<asp:BoundColumn DataField="OrganizationName" HeaderText="Organization"></asp:BoundColumn>
								<asp:BoundColumn DataField="InvalidMsgs" HeaderText="Invalid Msgs"></asp:BoundColumn>
								<asp:BoundColumn DataField="ValidMsgs" HeaderText="Valid Msgs"></asp:BoundColumn>
								<asp:BoundColumn DataField="PercentInvalidMsgs" HeaderText="%"></asp:BoundColumn>
							</Columns>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD></TD>
				</TR>
				<TR>
					<TD class="formtext">&nbsp;
						<asp:Label id="lblBoxIpUpdates" runat="server" Visible="False"> No communications</asp:Label></TD>
				</TR>
				<TR>
					<TD>
						<asp:datagrid id="dgNoIPUpdates" runat="server" ForeColor="Black" CellPadding="4" BackColor="White"
							BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE" Width="626px" AutoGenerateColumns="False">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Left" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Left" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="BoxId" HeaderText="Box"></asp:BoundColumn>
								<asp:BoundColumn DataField="LicensePlate" HeaderText="License Plate"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Vehicle"></asp:BoundColumn>
								<asp:BoundColumn DataField="OrganizationName" HeaderText="Organization"></asp:BoundColumn>
							</Columns>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 13px">
						<asp:Label id="lblReported" runat="server" Visible="False" CssClass="formtext"> Reported Messages</asp:Label></TD>
				</TR>
				<TR>
					<TD>
						<asp:datagrid id="dgReportedFrq" runat="server" ForeColor="Black" CellPadding="4" BackColor="White"
							BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE" Width="626px" AutoGenerateColumns="False">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Left" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Left" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="BoxId" HeaderText="Box"></asp:BoundColumn>
								<asp:BoundColumn DataField="OrganizationName" HeaderText="Organization"></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalMessages" HeaderText="Total Messages"></asp:BoundColumn>
							</Columns>
						</asp:datagrid></TD>
				</TR>
			</TABLE>
			<asp:Label id="lblDate" style="Z-INDEX: 101; LEFT: 22px; POSITION: absolute; TOP: 10px" runat="server"
				CssClass="formtext"></asp:Label>
			<asp:Label id="Label3" style="Z-INDEX: 103; LEFT: 312px; POSITION: absolute; TOP: 6px" runat="server"
				CssClass="heading">Diagnostic Results</asp:Label>
			<asp:Label id="lblHours" style="Z-INDEX: 104; LEFT: 144px; POSITION: absolute; TOP: 11px" runat="server"
				CssClass="formtext"></asp:Label>
		</form>
	</body>
</HTML>
