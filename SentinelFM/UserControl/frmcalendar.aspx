<%@ Page language="c#" Inherits="SentinelFM.UserControl.frmCalendar" CodeFile="frmCalendar.aspx.cs" Culture="en-US" UICulture="auto"  %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>Calendar</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
	<body>
		<form method="post" runat="server">
			<asp:calendar id="calDate" style="Z-INDEX: 101; LEFT: 5px; POSITION: absolute; TOP: 1px" runat="server" CellSpacing="1" BorderColor="Black" BorderStyle="Solid" Font-Bold="True" Font-Size="11px" NextPrevFormat="ShortMonth" Height="195px" Width="240px" onselectionchanged="cldTo_SelectionChanged">
				<TodayDayStyle ForeColor="White" BackColor="#999999"></TodayDayStyle>
				<DayStyle BackColor="#CCCCCC"></DayStyle>
				<NextPrevStyle Font-Size="10px" Font-Bold="True" ForeColor="White"></NextPrevStyle>
				<DayHeaderStyle Font-Size="12px" Font-Bold="True" Height="10px" ForeColor="#333333"></DayHeaderStyle>
				<SelectedDayStyle ForeColor="White" BackColor="gray"></SelectedDayStyle>
				<TitleStyle Font-Size="12px" Font-Bold="True" Height="12px" ForeColor="White" BackColor="gray"></TitleStyle>
				<OtherMonthDayStyle ForeColor="#999999"></OtherMonthDayStyle>
			</asp:calendar>
			<asp:Label id="lblControlName" style="Z-INDEX: 102; LEFT: 5px; POSITION: absolute; TOP: 199px" runat="server" Visible="False"></asp:Label>
		</form>
	</body>
</HTML>
