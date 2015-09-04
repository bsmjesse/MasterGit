<%@ Page language="c#" Inherits="SentinelFM.Admin.frmReport_CommDiagnostic" CodeFile="frmReport_CommDiagnostic.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Network Latency Report</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<asp:label id="lblToDate" style="Z-INDEX: 106; LEFT: 264px; POSITION: absolute; TOP: 32px"
				runat="server" CssClass="tableheading" Font-Underline="True"></asp:label>
			<TABLE id="Table1" style="Z-INDEX: 109; LEFT: 48px; POSITION: absolute; TOP: 56px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
						<asp:datagrid id="dgCommDiag" runat="server" ForeColor="Black" CellPadding="4" BackColor="White"
							BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE" Width="626px" AutoGenerateColumns="False">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="OrganizationName" HeaderText="Organization"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Description"></asp:BoundColumn>
								<asp:BoundColumn DataField="BoxId" HeaderText="BoxId"></asp:BoundColumn>
								<asp:BoundColumn DataField="CommModeName" HeaderText="Communication Mode"></asp:BoundColumn>
								<asp:BoundColumn DataField="NumOfMsgs" HeaderText="Number of  Messages"></asp:BoundColumn>
								<asp:BoundColumn DataField="DiffInSec" HeaderText="Latency (Sec)"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD vAlign="bottom" align="center">
						<TABLE class="formtext" id="tblNotes" style="WIDTH: 600px; HEIGHT: 111px" cellSpacing="0"
							cellPadding="0" width="600" border="0">
							<TR>
								<TD style="FONT-WEIGHT: bold" height="20">Notes:</TD>
							</TR>
							<TR>
								<TD>1. Current Communication mode for the box was not changed during report period.</TD>
							</TR>
							<TR>
								<TD height="5"></TD>
							</TR>
							<TR>
								<TD>2. Current Box Vehicle Assignment was not chnaged during report period.</TD>
							</TR>
							<TR>
								<TD height="5"></TD>
							</TR>
							<TR>
								<TD>3. Only Boxes assigned to Vehicles are diplayed.</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
			<asp:label id="Label3" style="Z-INDEX: 105; LEFT: 240px; POSITION: absolute; TOP: 32px" runat="server"
				CssClass="tableheading">To:</asp:label>
			<asp:label id="Label1" style="Z-INDEX: 101; LEFT: 48px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="heading" Font-Size="Larger"> Network Latency Report for Organization:</asp:label>
			<asp:label id="lblOranizationName" style="Z-INDEX: 102; LEFT: 424px; POSITION: absolute; TOP: 8px"
				runat="server" CssClass="heading" Font-Underline="True" Font-Size="Larger"></asp:label>
			<asp:label id="Label2" style="Z-INDEX: 103; LEFT: 48px; POSITION: absolute; TOP: 32px" runat="server"
				CssClass="tableheading">From:</asp:label>
			<asp:label id="lblFromDate" style="Z-INDEX: 104; LEFT: 88px; POSITION: absolute; TOP: 32px"
				runat="server" CssClass="tableheading" Font-Underline="True"></asp:label></form>
	</body>
</HTML>
