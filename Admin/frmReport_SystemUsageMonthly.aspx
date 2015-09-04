<%@ Page language="c#" Inherits="SentinelFM.Admin.frmReport_SystemUsageMonthly" CodeFile="frmReport_SystemUsageMonthly.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>System Usage Report</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
		
		<script language=javascript>
		<!--
			function BoxInfo(BoxId) { 
					var mypage='frmBoxCommInfo.aspx?BoxId='+BoxId
					var myname='BoxCommInfo';
					var w=480;
					var h=395;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=yes' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 
		//-->
	</script>
	
	</HEAD>
	<body>
		<form method="post" runat="server">
			<asp:label id="Label1" style="Z-INDEX: 101; LEFT: 120px; POSITION: absolute; TOP: 3px" runat="server"
				Font-Size="Larger" CssClass="heading">System Usage Report for Organization:</asp:label>
			<asp:label id="lblMonth" style="Z-INDEX: 105; LEFT: 342px; POSITION: absolute; TOP: 32px" runat="server"
				CssClass="tableheading" Font-Underline="True"></asp:label>
			<asp:label id="Label2" style="Z-INDEX: 104; LEFT: 277px; POSITION: absolute; TOP: 32px" runat="server"
				CssClass="tableheading">For Month:</asp:label>
			<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 11px; POSITION: absolute; TOP: 94px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
						<asp:datagrid id="dgData" runat="server" AutoGenerateColumns="False" Width="1049px" BorderColor="#DEDFDE"
							BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="4" ForeColor="Black">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<Columns>
								<asp:HyperLinkColumn ItemStyle-HorizontalAlign=Left   DataNavigateUrlField="BoxId" DataNavigateUrlFormatString="javascript:var w =BoxInfo('{0}')"
									DataTextField="Description" SortExpression="Description" HeaderText="Vehicle Description" DataTextFormatString="{0:c}">
									<HeaderStyle Width="100px"></HeaderStyle>
									<ItemStyle Wrap="False" ForeColor="Black"></ItemStyle>
								</asp:HyperLinkColumn>
					
								<asp:BoundColumn DataField="BoxId" HeaderText="Box Id">
									<HeaderStyle Width="60px"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ProtocolTypeName" HeaderText="Protocol Type"></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalInBytes" HeaderText="Bytes In">
									<HeaderStyle HorizontalAlign="Center" Width="40px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="TotalOutBytes" HeaderText="Bytes Out"></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalInMsgs" HeaderText="Msg In "></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalOutMsgs" HeaderText="Msg Out"></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalInTxtMsgs" HeaderText="Text Msg In"></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalOutTxtMsgs" HeaderText="Text Msg Out"></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalBytes" HeaderText="Total Bytes"></asp:BoundColumn>
								<asp:BoundColumn DataField="TotalMsgs" HeaderText="Total Msg">
									<HeaderStyle Width="70px"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="TotalTxtMsgs" HeaderText="Total Text Msg"></asp:BoundColumn>
								<asp:BoundColumn DataField="MaxMsgs" HeaderText="Limit Msg"></asp:BoundColumn>
								<asp:BoundColumn DataField="MaxTxtMsgs" HeaderText="Limit Text Msg"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD class="formtext" style="HEIGHT: 16px" align="right">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</TD>
				</TR>
				<TR>
					<TD align="right"></TD>
				</TR>
			</TABLE>
			<asp:label id="lblOranizationName" style="Z-INDEX: 100; LEFT: 475px; POSITION: absolute; TOP: 5px"
				runat="server" Font-Size="Larger" CssClass="heading" Font-Underline="True"></asp:label>
		</form>
	</body>
</HTML>
