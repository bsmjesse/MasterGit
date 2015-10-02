<%@ Page language="c#" Inherits="SentinelFM.Admin.frmReport_MapUsage" CodeFile="frmReport_MapUsage.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Map Usage Report</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<FORM id="frmReport_SystemUsage" method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 72px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD class="tableheading">Mapoint:</TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 155px" align="right" valign="top">
						<asp:datagrid id="dgMapPoint" runat="server" ForeColor="Black" CellPadding="4" BackColor="White"
							BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE" Width="626px" AutoGenerateColumns="False">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
							<Columns>
								<asp:BoundColumn DataField="UserName_BoxId" HeaderText="User/Box"></asp:BoundColumn>
								<asp:BoundColumn DataField="Map" HeaderText="Map"></asp:BoundColumn>
								<asp:BoundColumn DataField="StreetAddress" HeaderText="Street Address"></asp:BoundColumn>
								<asp:BoundColumn DataField="Totals" HeaderText="Total"></asp:BoundColumn>
							</Columns>
						</asp:datagrid>
						<asp:Label id="lblTotalMap" runat="server" CssClass="tableheading"></asp:Label>&nbsp;&nbsp;&nbsp; 
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</TD>
				</TR>
				<TR>
					<TD class="tableheading">GeoMicro</TD>
				</TR>
				<TR>
					<TD align="right" valign="top">
						<asp:datagrid id="dgGeo" runat="server" AutoGenerateColumns="False" Width="626px" BorderColor="#DEDFDE"
							BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4" ForeColor="Black">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
							<Columns>
								<asp:BoundColumn DataField="UserName_BoxId" HeaderText="User/Box"></asp:BoundColumn>
								<asp:BoundColumn DataField="Map" HeaderText="Map"></asp:BoundColumn>
								<asp:BoundColumn DataField="StreetAddress" HeaderText="Street Address"></asp:BoundColumn>
								<asp:BoundColumn DataField="Totals" HeaderText="Total"></asp:BoundColumn>
							</Columns>
						</asp:datagrid>
						<asp:Label id="lblTotalGeo" runat="server" CssClass="tableheading"></asp:Label>&nbsp;&nbsp;&nbsp; 
						&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</TD>
				</TR>
			</TABLE>
			<asp:label id="lblOranizationName" style="Z-INDEX: 106; LEFT: 448px; POSITION: absolute; TOP: 7px"
				runat="server" CssClass="heading" Font-Size="Larger" Font-Underline="True"></asp:label>
			<asp:label id="Label1" style="Z-INDEX: 100; LEFT: 130px; POSITION: absolute; TOP: 5px" runat="server"
				Font-Size="Larger" CssClass="heading">Map Usage Report for Organization:</asp:label>
			<asp:label id="Label2" style="Z-INDEX: 101; LEFT: 195px; POSITION: absolute; TOP: 36px" runat="server"
				CssClass="tableheading">For Month:</asp:label>
			<asp:Label id="lblMonth" style="Z-INDEX: 103; LEFT: 260px; POSITION: absolute; TOP: 37px" runat="server"
				CssClass="tableheading"></asp:Label></FORM>
	</body>
</HTML>
