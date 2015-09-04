<%@ Page language="c#" Inherits="SentinelFM.Admin.frmBoxSentCommands" CodeFile="frmBoxIncomingCmds.aspx.cs" %>
<%@ Register assembly="ISNet.WebUI.WebGrid" namespace="ISNet.WebUI.WebGrid" tagprefix="ISWebGrid" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Box Commands Received</title>
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
									<TABLE class="formtext" id="Table1" style="WIDTH: 307px; HEIGHT: 130px" cellSpacing="0"
										cellPadding="0" border="0">
										<TR>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
											<TD style="HEIGHT: 12px"></TD>
										</TR>
										<TR>
											<TD height="5"></TD>
											<TD height="5"><asp:label id="lblOrganization" runat="server">Organization:</asp:label></TD>
											<TD height="5">
                                                <asp:dropdownlist id="cboOrganization" runat="server" 
                                                    DataTextField="OrganizationName" DataValueField="OrganizationId"
													AutoPostBack="True" Width="215px" CssClass="RegularText" 
                                                    onselectedindexchanged="cboOrganization_SelectedIndexChanged"></asp:dropdownlist></TD>
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
                                        <tr>
											<TD class="style1">&nbsp;</TD>
											<TD class="style1">Message Type:</TD>
											<TD class="style1">
                                                <asp:DropDownList ID="CmdTypeList" runat="server" Width="215px">
                                                    <asp:ListItem Value="-1">All</asp:ListItem>
                                                    <asp:ListItem Value="13">Firmware Version</asp:ListItem>
                                                </asp:DropDownList>
                                            </TD>
										</tr>
										<TR>
											<TD height="10"></TD>
											<TD height="10"></TD>
											<TD height="10"></TD>
										</TR>
										<TR>
											<TD></TD>
											<TD>&nbsp;</TD>
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
						<TABLE id="tblFW" cellSpacing="0" cellPadding="0" border="0" runat="server" style="width: 605px">
							<TR>
								<TD style="width: 533px; height: 546px">
									<TABLE id="Table3" cellSpacing="0" cellPadding="0" border="0" style="width: 606px">
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="76" style="HEIGHT: 76px; width: 587px;">
												&nbsp;</TD>
										</TR>
										<TR>
											<TD vAlign="top" align="center" colSpan="2" style="HEIGHT: 370px; width: 587px;">
												
                                                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" 
                                                    BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
                                                    CellPadding="3" Font-Size="Small" GridLines="Vertical">
                                                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                                    <Columns>
                                                        <asp:BoundField DataField="BoxId" HeaderText="Box Id" />
                                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                                        <asp:BoundField DataField="BoxMsgInTypeName" HeaderText="Command Type" />
                                                        <asp:BoundField DataField="Timestamp" HeaderText="Timestamp" />
                                                        <asp:BoundField DataField="CustomProp" HeaderText="Custom Prop" />
                                                    </Columns>
                                                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                                                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                                                    <AlternatingRowStyle BackColor="Gainsboro" />
                                                </asp:GridView>
                                                
                                            </TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="23" style="HEIGHT: 23px; width: 587px;">
												&nbsp;
												</TD>
										</TR>
										<TR>
											<TD vAlign="middle" align="center" colSpan="2" height="40" style="width: 587px">
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
