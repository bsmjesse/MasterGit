<%@ Page language="c#" Inherits="SentinelFM.Admin.frmServicesUpdates" CodeFile="frmServicesUpdates.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Administration</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="/SentinelFM/GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 103; LEFT: 204px; POSITION: absolute; TOP: 13px" cellSpacing="0" cellPadding="0" width="300" border="0">
				<TR>
					<TD>
						<asp:datagrid id="dgFeatures" runat="server" AutoGenerateColumns="False" CellPadding="3" BorderColor="Black" BorderStyle="Ridge" BorderWidth="1px" BackColor="White" GridLines="None" CellSpacing="1" DataKeyField="MsgId" Width="362px">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="8pt" Font-Names="Arial, Helvetica, sans-serif" ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
							<HeaderStyle Font-Size="8pt" Font-Names="Arial, Helvetica, sans-serif" Font-Bold="True" ForeColor="White" BackColor="#000066"></HeaderStyle>
							<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
							<Columns>
								<asp:BoundColumn DataField="Msg" HeaderText="New Features:" ItemStyle-Width="300"></asp:BoundColumn>
								<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="<img src=../images/ok.gif border=0>" CancelText="<img src=../images/cancel.gif border=0>" EditText="<img src=../images/edit.gif border=0>"></asp:EditCommandColumn>
								<asp:ButtonColumn Text="<img src=../images/delete.gif border=0>" CommandName="Delete"></asp:ButtonColumn>
							</Columns>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD></TD>
				</TR>
				<TR>
					<TD></TD>
				</TR>
			</TABLE>
			<asp:datagrid id="dgSystem" style="Z-INDEX: 101; LEFT: 206px; POSITION: absolute; TOP: 222px" runat="server" AutoGenerateColumns="False" CellPadding="3" BorderColor="Black" BorderStyle="Ridge" BorderWidth="1px" BackColor="White" GridLines="None" CellSpacing="1" DataKeyField="MsgId" Width="363px">
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
				<ItemStyle Font-Size="8pt" Font-Names="Arial, Helvetica, sans-serif" ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
				<HeaderStyle Font-Size="8pt" Font-Names="Arial, Helvetica, sans-serif" Font-Bold="True" ForeColor="White" BackColor="#000066"></HeaderStyle>
				<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
				<Columns>
					<asp:BoundColumn DataField="Msg" HeaderText="System Status:" ItemStyle-Width="300"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="Severity" HeaderText="Severity"></asp:BoundColumn>
					<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="<img src=../images/ok.gif border=0>" CancelText="<img src=../images/cancel.gif border=0>" EditText="<img src=../images/edit.gif border=0>"></asp:EditCommandColumn>
					<asp:ButtonColumn Text="<img src=../images/delete.gif border=0>" CommandName="Delete"></asp:ButtonColumn>
				</Columns>
			</asp:datagrid>
		</form>
	</body>
</HTML>
