<%@ Page language="c#" Inherits="SentinelFM.Admin.frmReport_Logins" CodeFile="frmReport_Login.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Report Logins</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<FORM id="frmReport_SystemUsage" method="post" runat="server">
			<asp:label id="lblToDate" style="Z-INDEX: 106; LEFT: 357px; POSITION: absolute; TOP: 33px"
				runat="server" CssClass="tableheading" Font-Underline="True"></asp:label><asp:label id="Label3" style="Z-INDEX: 105; LEFT: 336px; POSITION: absolute; TOP: 33px" runat="server"
				CssClass="tableheading">To:</asp:label><asp:label id="Label1" style="Z-INDEX: 101; LEFT: 130px; POSITION: absolute; TOP: 5px" runat="server"
				CssClass="heading" Font-Size="Larger">User Login Report for Organization:</asp:label><asp:label id="lblOranizationName" style="Z-INDEX: 102; LEFT: 475px; POSITION: absolute; TOP: 5px"
				runat="server" CssClass="heading" Font-Underline="True" Font-Size="Larger"></asp:label><asp:label id="Label2" style="Z-INDEX: 103; LEFT: 158px; POSITION: absolute; TOP: 33px" runat="server"
				CssClass="tableheading">From:</asp:label><asp:label id="lblFromDate" style="Z-INDEX: 104; LEFT: 197px; POSITION: absolute; TOP: 32px"
				runat="server" CssClass="tableheading" Font-Underline="True"></asp:label>
			<asp:datagrid id="dgData" runat="server" AutoGenerateColumns="False" Width="626px" BorderColor="#DEDFDE"
				BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4" ForeColor="Black" style="Z-INDEX: 107; LEFT: 37px; POSITION: absolute; TOP: 64px">
				<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
				<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="Black" BackColor="White"></ItemStyle>
				<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
					VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="LoginId" HeaderText="LoginId "></asp:BoundColumn>
					<asp:BoundColumn DataField="UserId" HeaderText="UserId"></asp:BoundColumn>
					<asp:BoundColumn DataField="IP" HeaderText="IP Address"></asp:BoundColumn>
					<asp:BoundColumn DataField="LoginDateTime" HeaderText="Date/Time "></asp:BoundColumn>
					<asp:BoundColumn DataField="UserName" HeaderText="User Name "></asp:BoundColumn>
					<asp:BoundColumn DataField="FirstName" HeaderText="First Name "></asp:BoundColumn>
					<asp:BoundColumn DataField="LastName" HeaderText="Last Name "></asp:BoundColumn>
				</Columns>
			</asp:datagrid></FORM>
	</body>
</HTML>
