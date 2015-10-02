<%@ Page language="c#" Inherits="SentinelFM.AutomaticTask.frmAutomaticTasks" CodeFile="frmAutomaticTasks.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<title>frmAutomaticTasks</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form method="post" runat="server">
			<TABLE id="Table2" style="BORDER-RIGHT: black 4px double; BORDER-TOP: black 4px double; Z-INDEX: 103; LEFT: 2px; BORDER-LEFT: black 4px double; BORDER-BOTTOM: black 4px double; POSITION: absolute; TOP: 5px"
				cellSpacing="1" cellPadding="1" width="1005" border="1" height="550">
				<TR>
					<TD align="center">
						<asp:datagrid id="dgAutomaticTasks" runat="server" CellSpacing="1" GridLines="None" BackColor="White"
							BorderWidth="2px" BorderStyle="Ridge" BorderColor="White" CellPadding="3" AutoGenerateColumns="False"
							DataKeyField="TaskId" PageSize="4" Width="666px">
							<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
							<AlternatingItemStyle CssClass="gridtext" BackColor="beige"></AlternatingItemStyle>
							<ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" Font-Bold="True" ForeColor="Black"
								BackColor="black"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="RequestDateTime" ReadOnly="True" HeaderText="Date/Time"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" ReadOnly="True" HeaderText="Vehicle"></asp:BoundColumn>
								<asp:BoundColumn DataField="LicensePlate" ReadOnly="True" HeaderText="License Plate"></asp:BoundColumn>
								<asp:BoundColumn DataField="BoxCmdOutTypeName" ReadOnly="True" HeaderText="Task"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Transmission Period">
									<ItemStyle Wrap="False"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="lblPeriod" text='<%#  DataBinder.Eval(Container.DataItem,"PeriodText") %>' Runat=server>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList ID="cboPeriod" DataSource='<%# dsSchPeriod%>' DataValueField="PeriodValue" DataTextField="PeriodText" SelectedIndex='<%# GetTransmissionPeriod(Convert.ToString(DataBinder.Eval(Container.DataItem,"TransmissionPeriod")))%>' Runat=server>
										</asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Transmission Interval">
									<ItemStyle Wrap="False"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="lblInterval" text='<%#  DataBinder.Eval(Container.DataItem,"IntervalText") %>' Runat=server>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList ID="cboInterval" DataSource='<%# dsSchInterval%>' DataValueField="IntervalValue" DataTextField="IntervalText" SelectedIndex='<%# GetTransmissionInterval(Convert.ToString(DataBinder.Eval(Container.DataItem,"TransmissionInterval")))%>' Runat=server>
										</asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;"
									EditText="&lt;img src=../images/edit.gif border=0&gt;"></asp:EditCommandColumn>
								<asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete"></asp:ButtonColumn>
							</Columns>
							<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
						<TABLE id="tblNoData" style="BORDER-RIGHT: black 1px outset; BORDER-TOP: black 1px outset; BORDER-LEFT: black 1px outset; BORDER-BOTTOM: black 1px outset"
							cellSpacing="0" cellPadding="0" width="400" border="0" runat="server">
							<TR>
								<TD vAlign="middle" align="center">
									<TABLE id="Table1" cellSpacing="0" cellPadding="0" width="100%" bgColor="#ffffff" border="0">
										<TR>
											<TD class="tableheading" vAlign="middle" align="center"><B><BR>
													No&nbsp; automatic tasks.
													<BR>
													<BR>
												</B></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
						<asp:label id="lblMessage" runat="server" Width="270px" Height="8px" Visible="False" CssClass="errortext"></asp:label>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
