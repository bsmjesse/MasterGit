<%@ Control Language="c#" Inherits="SentinelFM.Components.ctlFleetUsers" CodeFile="ctlFleetUsers.ascx.cs" %>
<TABLE id="Table1" style="WIDTH: 679px; HEIGHT: 444px" cellSpacing="0" cellPadding="0"
	width="679" align="center" border="0">
	<TR>
		<TD class="tableheading" style="BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; HEIGHT: 30px"
			align="center" valign="top">
			<TABLE id="Table3" style="WIDTH: 257px; HEIGHT: 20px" cellSpacing="0" cellPadding="0" width="257"
				align="center" border="0">
				<TBODY>
					<TR>
						<TD class="formtext" style="WIDTH: 100%" align="center" valign="top">
                      &nbsp;
                      <table>
                          <tr>
                              <td align="right" style="width: 100px">
                      <asp:Label ID="lblFleet" runat="server" meta:resourcekey="lblFleetResource1" Text="Fleet:" CssClass="formtext"></asp:Label></td>
                              <td style="width: 100px">
							<asp:dropdownlist id="cboToFleet" AutoPostBack="True" DataValueField="FleetId" DataTextField="FleetName"
								CssClass="RegularText" Width="200px" runat="server" onselectedindexchanged="cboToFleet_SelectedIndexChanged" meta:resourcekey="cboToFleetResource1"></asp:dropdownlist></td>
                          </tr>
                      </table>
                  </TD>
						<TD>
							</TD>
<TR>
	<TD style="WIDTH: 959px; HEIGHT: 217px" align="center">
		<TABLE id="Table8" style="WIDTH: 586px; HEIGHT: 284px" cellSpacing="0" cellPadding="0" width="586" border="0">
			<TR>
				<TD style="HEIGHT: 270px" align="center">
					<TABLE id="tblUsers" style="WIDTH: 548px; HEIGHT: 265px" cellSpacing="0" cellPadding="0"
						width="548" border="0" runat="server">
						<TR>
							<TD colspan="2" class="formtext" style="WIDTH: 240px">
                         <asp:Label ID="lblUnUsers" runat="server" meta:resourcekey="lblUnUsersResource1"
                             Text="Unassigned users"></asp:Label></TD>
							<%--<TD style="WIDTH: 130px" align="center"></TD>--%>
							<TD class="formtext">
                         <asp:Label ID="lblAssUsers" runat="server" meta:resourcekey="lblAssUsersResource1"
                             Text="Assigned users"></asp:Label></TD>
						</TR>
						<TR>
							<TD style="WIDTH: 110px" vAlign=top>
								<asp:listbox id="lstUnAss" DataValueField="UserId" DataTextField="UserFullName" CssClass="formtext"
									Width="200px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssResource1"></asp:listbox></TD>
							<TD style="WIDTH: 130px" align="center" vAlign=top>
								<TABLE id="tblAddRemoveBtns" style="WIDTH: 75px; HEIGHT: 99px" cellSpacing="0" cellPadding="0"
									width="75" border="0" runat="server">
									<TR>
										<TD vAlign=middle>
											<asp:button id="cmdAdd" CssClass="combutton" runat="server" Text="Add->" CommandName="39" onclick="cmdAdd_Click" meta:resourcekey="cmdAddResource1"></asp:button></TD>
									</TR>
									<TR>
										<TD style="HEIGHT: 20px"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdAddAll" CssClass="combutton" runat="server" Text="Add All->" CommandName="39" onclick="cmdAddAll_Click" meta:resourcekey="cmdAddAllResource1"></asp:button></TD>
									</TR>
									<TR>
										<TD id="TD1" style="HEIGHT: 20px" runat="server"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdRemove" CssClass="combutton" runat="server" Text="<-Remove" CommandName="40" onclick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:button></TD>
									</TR>
									<TR>
										<TD style="HEIGHT: 20px"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdRemoveAll" CssClass="combutton" runat="server" Text="<-Remove All" CommandName="40" onclick="cmdRemoveAll_Click" meta:resourcekey="cmdRemoveAllResource1"></asp:button></TD>
									</TR>
								</TABLE>
							</TD>
							<TD vAlign=top>
								<asp:listbox id="lstAss" DataValueField="UserId" DataTextField="UserFullName" CssClass="formtext"
									Width="200px" runat="server" SelectionMode="Multiple" Rows="15" DESIGNTIMEDRAGDROP="46" meta:resourcekey="lstAssResource1"></asp:listbox></TD>
						</TR>
					</TABLE>
				</TD>
			</TR>
		</TABLE>
		<asp:label id="lblMessage" CssClass="errortext" Width="270px" runat="server" Height="8px" Visible="False" meta:resourcekey="lblMessageResource1"></asp:label></TD>
</TR>
		</TD>
	</TR>
</TABLE>

