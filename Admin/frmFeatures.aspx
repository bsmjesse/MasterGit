<%@ Page language="c#" Inherits="SentinelFM.frmFeatures" CodeFile="frmFeatures.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Features</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 34px; WIDTH: 509px; POSITION: absolute; TOP: 13px; HEIGHT: 250px"
				cellSpacing="0" cellPadding="0" width="509" border="0">
				<TR>
					<TD>
						<asp:datagrid id="dgSystem" runat="server" AutoGenerateColumns="False" CellPadding="3" BorderColor="Black"
							BorderStyle="Ridge" BorderWidth="1px" BackColor="White" GridLines="None" CellSpacing="1" DataKeyField="MsgId"
							Width="505px" PageSize="8" AllowPaging="True">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" Font-Bold="True" ForeColor="White"
								BackColor="#009933"></HeaderStyle>
							<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Message">
									<HeaderStyle Width="450px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="lblMsg" text='<%# DataBinder.Eval(Container.DataItem,"Msg") %>' Runat=server Width=200>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="txtMsg" Runat=server Text='<%#  DataBinder.Eval(Container.DataItem,"Msg") %>' Width=200>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete"></asp:ButtonColumn>
							</Columns>
							<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 14px">
						<asp:label id="lblMessage" runat="server" Width="270px" Visible="False" Height="8px" CssClass="errortext"></asp:label></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 18px"></TD>
				</TR>
				<TR>
					<TD>
						<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD class="formtext" style="WIDTH: 434px">Message
									<asp:RequiredFieldValidator id="valMsg" runat="server" ErrorMessage="Please enter a Message" ControlToValidate="txtNewMsg">*</asp:RequiredFieldValidator>:</TD>
								<TD>
									<asp:TextBox id="txtNewMsg" runat="server" Width="224px" CssClass="formtext"></asp:TextBox></TD>
								<TD style="WIDTH: 106px"></TD>
								<TD align="right">
									<asp:button id="cmdAddMsg" runat="server" CssClass="combutton" Text="Add" onclick="cmdAddMsg_Click"></asp:button></TD>
							</TR>
							<TR>
								<TD class="formtext" style="WIDTH: 434px" height="2"></TD>
								<TD height="2"></TD>
								<TD style="WIDTH: 106px" height="2"></TD>
								<TD height="2"></TD>
							</TR>
							<TR>
								<TD class="formtext" style="WIDTH: 434px"></TD>
								<TD></TD>
								<TD style="WIDTH: 106px"></TD>
								<TD align="right">
									<INPUT class="combutton" id="cmdClose" onclick="window.close()" type="button" value="Close"
										name="cmdClose"></TD>
							</TR>
						</TABLE>
						<asp:ValidationSummary id="ValidationSummary1" runat="server"></asp:ValidationSummary></TD>
				</TR>
				<TR>
					<TD align="right">&nbsp;
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
