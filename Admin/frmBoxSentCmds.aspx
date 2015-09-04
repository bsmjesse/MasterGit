<%@ Page language="c#" Inherits="SentinelFM.Admin.frmBoxSentCommands" CodeFile="frmBoxSentCmds.aspx.cs" %>
<%@ Register assembly="ISNet.WebUI.WebGrid" namespace="ISNet.WebUI.WebGrid" tagprefix="ISWebGrid" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Box Commands Sent</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	    <style type="text/css">
            .style1
            {
                height: 3px;
            }
            .style2
            {
                height: 76px;
                width: 589px;
            }
            .style3
            {
                height: 370px;
                width: 589px;
            }
            .style4
            {
                height: 23px;
                width: 589px;
            }
            .style5
            {
                width: 589px;
            }
        </style>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<TABLE id="Table2" style="WIDTH: 709px; HEIGHT: 163px; border-right: gray 4px double; border-top: gray 4px double; border-left: gray 4px double; border-bottom: gray 4px double;"
				cellSpacing="0" cellPadding="0" border="0">
				<TR>
					<TD align="center" vAlign="top" style="width: 632px">
						<TABLE id="Table6"
							cellSpacing="0" cellPadding="0" width="541" border="0">
							<TR>
								<TD vAlign="middle" align="center" style="height: 153px">
									<TABLE class="formtext" id="Table1" 
                                        style="WIDTH: 316px; HEIGHT: 130px; margin-right: 13px;" cellSpacing="0"
										cellPadding="0" border="0">
										<TR>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
										</TR>
										<TR>
											<TD height="5"></TD>
											<TD height="5"><asp:label id="lblOrganization" runat="server">Organization:</asp:label></TD>
											<TD height="5"><asp:dropdownlist id="cboOrganization" runat="server" 
                                                    DataTextField="OrganizationName" DataValueField="OrganizationId"
													AutoPostBack="True" Width="215px" CssClass="RegularText" 
                                                    onselectedindexchanged="cboOrganization_SelectedIndexChanged" 
                                                    Height="16px"></asp:dropdownlist></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD></TD>
											<TD></TD>
										</TR>
										<TR>
											<TD style="HEIGHT: 21px"></TD>
											<TD style="HEIGHT: 21px"><asp:label id="lblFleets" runat="server">Fleet:</asp:label></TD>
											<TD style="HEIGHT: 21px"><asp:dropdownlist id="cboFleet" runat="server" 
                                                    DataTextField="FleetName" DataValueField="FleetId"
													AutoPostBack="True" Width="215px" CssClass="RegularText" ></asp:dropdownlist></TD>
										</TR>
										<tr>
											<TD height="10"></TD>
											<TD height="10"></TD>
											<TD height="10"></TD>
										</tr>
										<TR>
											<TD class="style1">&nbsp;</TD>
											<TD class="style1">Command Type:</TD>
											<TD class="style1">
                                                <asp:TextBox ID="txtCommand" runat="server" Width="215px"></asp:TextBox>
                                            </TD>
										</TR>
										<TR>
											<TD></TD>
											<TD></TD>
											<TD align="right">
												<asp:button id="cmdView" runat="server" CssClass="combutton" Text="View" onclick="cmdView_Click"></asp:button>
												<asp:button id="cmdToExcel" runat="server" CssClass="combutton" Text="To Excel" 
                                                    onclick="cmdToExcel_Click" Visible="False"></asp:button></TD>
										</TR>
									</TABLE>
                                    </TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD vAlign="top" align="center" style="width: 632px">
						<TABLE id="tblFW" cellSpacing="0" cellPadding="0" border="0" runat="server" 
                            style="width: 667px; margin-right: 62px;">
							<TR>
								<TD style="width: 533px; height: 546px">
									<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0" 
                                        style="width: 660px">
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="76" class="style2">
												&nbsp;</TD>
										</TR>
										<TR>
											<TD vAlign="top" align="center" colSpan="2" class="style3">
                                                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
                                                    BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
                                                    CellPadding="3" Font-Size="Small" GridLines="Vertical">
                                                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                                    <Columns>
                                                        <asp:BoundField DataField="BoxId" HeaderText="Box Id" />
                                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                                        <asp:BoundField DataField="BoxCmdOutTypeName" HeaderText="Command Type" />
                                                        <asp:BoundField DataField="DateTimeSent" HeaderText="Sent" />
                                                        <asp:BoundField DataField="DateTimeAck" HeaderText="Ack" />
                                                        <asp:BoundField DataField="Username" HeaderText="User Name" />
                                                        <asp:BoundField DataField="CustomProp" HeaderText="Custom Prop" />
                                                    </Columns>
                                                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                                                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                                                    <AlternatingRowStyle BackColor="#DCDCDC" />
                                                </asp:GridView>
                                            </TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="23" class="style4">
												&nbsp;
												</TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="40" class="style5">
												<asp:label id="lblMessage" runat="server" CssClass="regulartext" ForeColor="Red"></asp:label></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
