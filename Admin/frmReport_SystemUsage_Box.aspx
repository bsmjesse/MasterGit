<%@ Page language="c#" Inherits="SentinelFM.Admin.frmReport_SystemUsage_Box" CodeFile="frmReport_SystemUsage_Box.aspx.cs" %>
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
		<FORM id="frmReport_SystemUsage" method="post" runat="server">
			<asp:label id="lblToDate" style="Z-INDEX: 106; LEFT: 346px; POSITION: absolute; TOP: 38px"
				runat="server" CssClass="tableheading" Font-Underline="True"></asp:label>
			<asp:datagrid id="dgData" style="Z-INDEX: 107; LEFT: 17px; POSITION: absolute; TOP: 66px" runat="server"
				ForeColor="Black" CellPadding="4" BackColor="White" BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE"
				Width="626px" AutoGenerateColumns="False" Visible=false  >
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
					<asp:BoundColumn DataField="TotalInMsgs" HeaderText="Messages In "></asp:BoundColumn>
					<asp:BoundColumn DataField="TotalOutMsgs" HeaderText="Messages Out"></asp:BoundColumn>
					<asp:BoundColumn DataField="TotalInTxtBytes" HeaderText="Text Msgs Bytes In"></asp:BoundColumn>
					<asp:BoundColumn DataField="TotalInTxtMsgs" HeaderText="Text Msgs In"></asp:BoundColumn>
					<asp:BoundColumn DataField="TotalOutTxtBytes" HeaderText="Text Msgs Bytes Out"></asp:BoundColumn>
					<asp:BoundColumn DataField="TotalOutTxtMsgs" HeaderText="Text Msgs Out"></asp:BoundColumn>
					<asp:BoundColumn DataField="TotalBytes" HeaderText="Total Bytes"></asp:BoundColumn>
					<asp:BoundColumn DataField="TotalMsgs" HeaderText="Total Messages">
						<HeaderStyle Width="70px"></HeaderStyle>
					</asp:BoundColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			
			
			
			
			
			
			
			
			
			
			<asp:datagrid id="dgDataNew" style="Z-INDEX: 107; LEFT: 17px; POSITION: absolute; TOP: 66px" runat="server"
				ForeColor="Black" CellPadding="4" BackColor="White" BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE"
				Width="626px" AutoGenerateColumns="False">
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
								
					<asp:BoundColumn DataField="BoxId" HeaderText="Box Id"/>
					<asp:BoundColumn DataField="Description" HeaderText="Description"/>
					<asp:BoundColumn DataField="TotalBytes" HeaderText="Total Bytes"/>
					<asp:BoundColumn DataField="TotalMsgs" HeaderText="Total Messages"/>
					<asp:BoundColumn DataField="TotalTxtBytes" HeaderText="Total Text Bytes"/>
					<asp:BoundColumn DataField="TotalTxtMsgs" HeaderText="Total Text Messages"/>
						
					
				</Columns>
				<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			
			
			
			<asp:label id="Label3" style="Z-INDEX: 105; LEFT: 325px; POSITION: absolute; TOP: 38px" runat="server"
				CssClass="tableheading">To:</asp:label>
			<asp:label id="Label1" style="Z-INDEX: 101; LEFT: 130px; POSITION: absolute; TOP: 5px" runat="server"
				CssClass="heading" Font-Size="Larger">System Usage Report for Box:</asp:label>
			<asp:label id="lblBoxId" style="Z-INDEX: 102; LEFT: 413px; POSITION: absolute; TOP: 5px" runat="server"
				CssClass="heading" Font-Underline="True" Font-Size="Larger"></asp:label>
			<asp:label id="Label2" style="Z-INDEX: 103; LEFT: 147px; POSITION: absolute; TOP: 38px" runat="server"
				CssClass="tableheading">From:</asp:label>
			<asp:label id="lblFromDate" style="Z-INDEX: 104; LEFT: 186px; POSITION: absolute; TOP: 37px"
				runat="server" CssClass="tableheading" Font-Underline="True"></asp:label></FORM>
	</body>
</HTML>
