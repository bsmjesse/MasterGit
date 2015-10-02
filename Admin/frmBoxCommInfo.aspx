<%@ Page language="c#" Inherits="SentinelFM.Admin.frmBoxCommInfo" CodeFile="frmBoxCommInfo.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Box Communication Info</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<asp:DataGrid id="dgData" runat="server" AutoGenerateColumns="False" Width="426px" BorderColor="#DEDFDE"
							BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4" ForeColor="Black">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<Columns>
								<asp:BoundColumn DataField="CommAddressTypeName" HeaderText="Type"></asp:BoundColumn>
								<asp:BoundColumn DataField="CommAddressValue" HeaderText="Address"></asp:BoundColumn>
							</Columns> 
			</asp:DataGrid>
		</form>
	</body>
</HTML>
