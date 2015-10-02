<%@ Page language="c#" Inherits="SentinelFM.frmHistoryMsgs"  CodeFile="frmHistoryMsgs.aspx.cs" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat=server  >
		<title>Messages</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form method="post" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 103; LEFT: 9px; POSITION: absolute; TOP: 7px" cellSpacing="0" cellPadding="0" width="300" border="0">
				<TR>
					<TD>
						<TABLE id="Table2" cellSpacing="0" cellPadding="0"  border="0">
							<TR>
								<TD style="height: 45px">
									<asp:label id="Label3" runat="server" CssClass="formtext" >Last Messages:</asp:label></TD>
								<TD style="height: 45px">
									<asp:textbox id="txtLastMessages" runat="server" CssClass="RegularText" Width="53px">10</asp:textbox></TD>
                        <td style="height: 45px">
                           &nbsp;</td>
                        <td style="height: 45px">
                           <asp:Label ID="Label1" runat="server" CssClass="formtext" >Organization (optional):</asp:Label></td>
								<TD style="height: 45px">
                           <asp:DropDownList ID="cboOrganization" runat="server" AutoPostBack="True" CssClass="RegularText"
                              DataTextField="OrganizationName" DataValueField="OrganizationId"
                              OnSelectedIndexChanged="cboOrganization_SelectedIndexChanged" Width="203px" Enabled="False">
                           </asp:DropDownList></TD>
                        <td style="height: 45px">
                           &nbsp;</td>
								<TD style="height: 45px">
                           <asp:Label ID="Label4" runat="server" CssClass="formtext" >Fleet (optional):</asp:Label></TD>
								<TD style="height: 45px">
                           <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText"
                              DataTextField="FleetName" DataValueField="FleetId" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                              Width="203px">
                           </asp:DropDownList></TD>
                        <td style="height: 45px">
                           &nbsp;</td>
								<TD style="height: 45px" >
									<asp:label id="lblBoxId" runat="server" CssClass="formtext">Box Id:</asp:label></TD>
								<TD style="height: 45px" >
									<asp:textbox id="txtBoxId" runat="server" CssClass="RegularText" Width="67px"></asp:textbox></TD>
                        <td style="height: 45px">
                           &nbsp; &nbsp; &nbsp;&nbsp;
                        </td>
								
								<TD style="height: 45px">
									<asp:button id="btnRetrieveLastMessages" onmouseover="javascript:this.style.backgroundColor='#FFFFFF'; &#13;&#10;&#9;&#9;&#9;&#9;&#9;&#9;javascript:this.style.color='#FFCC00';&#13;&#10;&#9;&#9;&#9;&#9;&#9;&#9;" tabIndex="4"  runat="server" CssClass="combutton" Text="Retrieve" onclick="btnRetrieveLastMessages_Click"></asp:button></TD>
							</TR>
                     <tr>
                        <td style="height: 45px">
									<asp:label id="lblFromCalendar" runat="server" CssClass="formtext">From:</asp:label></td>
                        <td style="height: 45px">
									<asp:textbox id="txtFormDT" runat="server" CssClass="RegularText" Width="140px">mm/dd/yyyy 00:00:00 AM</asp:textbox></td>
                        <td style="height: 45px">
                        </td>
                        <td style="height: 45px">
									<asp:label id="lblToCalendar" runat="server" CssClass="formtext">To:</asp:label></td>
                        <td style="height: 45px">
									<asp:textbox id="txtToDT" runat="server" CssClass="RegularText" Width="140px">mm/dd/yyyy 00:00:00 AM</asp:textbox></td>
                        <td style="height: 45px">
                        </td>
                        <td style="height: 45px">
									<asp:label id="Label2" runat="server" CssClass="formtext" >Message Type:</asp:label></td>
                        <td style="height: 45px">
									<asp:dropdownlist id="lstMessageType" runat="server" CssClass="RegularText" Width="146px">
										<asp:ListItem Value="-1" Selected="True">No Filter</asp:ListItem>
										<asp:ListItem Value="0">Ack</asp:ListItem>
										<asp:ListItem Value="1">Coordinate</asp:ListItem>
										<asp:ListItem Value="2">Sensor</asp:ListItem>
										<asp:ListItem Value="3">Speed</asp:ListItem>
										<asp:ListItem Value="4">GeoFence</asp:ListItem>
										<asp:ListItem Value="5">KeyFobArm</asp:ListItem>
										<asp:ListItem Value="6">KeyFobDisarm</asp:ListItem>
										<asp:ListItem Value="7">Identification</asp:ListItem>
										<asp:ListItem Value="8">PictureDownloadComplete</asp:ListItem>
										<asp:ListItem Value="9">PositionUpdate</asp:ListItem>
										<asp:ListItem Value="10">IPUpdate</asp:ListItem>
										<asp:ListItem Value="11">WaypointData</asp:ListItem>
										<asp:ListItem Value="12">WaypointDownloadEnd</asp:ListItem>
										<asp:ListItem Value="13">BoxStatus</asp:ListItem>
										<asp:ListItem Value="14">BoxSetup</asp:ListItem>
										<asp:ListItem Value="15">KeyFobPanic</asp:ListItem>
										<asp:ListItem Value="16">ExternalData</asp:ListItem>
										<asp:ListItem Value="17">NAck</asp:ListItem>
										<asp:ListItem Value="18">Alarm</asp:ListItem>
										<asp:ListItem Value="19">MBAlarm</asp:ListItem>
										<asp:ListItem Value="20  ">MDTResponse  </asp:ListItem>
										<asp:ListItem Value="21">MDTSpecialMessage</asp:ListItem>
										<asp:ListItem Value="22">MDTTextMessage</asp:ListItem>
										<asp:ListItem Value="23">MDTAck </asp:ListItem>
										<asp:ListItem Value="24">StoredPosition</asp:ListItem>
										<asp:ListItem Value="25">Idling    </asp:ListItem>
									</asp:dropdownlist></td>
                      
                        <td style="height: 45px">
									</td>
                        <td style="height: 45px">
                        </td>
                        <td style="height: 45px; text-align: center;" colspan="3">
									<asp:RadioButtonList id="lstMessages" runat="server" CssClass="formtext" RepeatDirection="Horizontal" OnSelectedIndexChanged="lstMessages_SelectedIndexChanged" AutoPostBack="True">
										<asp:ListItem Value="0" Selected="True">Incoming</asp:ListItem>
										<asp:ListItem Value="1">Outgoing</asp:ListItem>
									</asp:RadioButtonList></td>
                     </tr>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD height="5"></TD>
				</TR>
				<TR>
					<TD>
						<asp:datagrid id="dgrHistoryIn" runat="server" AllowSorting="True" AutoGenerateColumns="False" BorderColor="#CCCCCC" BorderWidth="1px" BackColor="White" BorderStyle="None" CellPadding="3" Visible="False">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#669999"></SelectedItemStyle>
							<ItemStyle Font-Size="10px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="#000066"></ItemStyle>
							<HeaderStyle Font-Size="10px" Font-Names="Arial,Helvetica,sans-serif" Font-Bold="True" ForeColor="White" BackColor="#006699"></HeaderStyle>
							<FooterStyle ForeColor="#000066" BackColor="White"></FooterStyle>
							<Columns>
								<asp:BoundColumn DataField="BoxId" HeaderText="Box Id">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="OriginDateTime" HeaderText="Originated Date/Time">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="DateTimeReceived" HeaderText="Received Date/Time ">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="DclId" HeaderText="DCL">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="BoxMsgInTypeName" HeaderText="Msg Type">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="BoxProtocolTypeName" HeaderText="Protocol">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="CommInfo1" HeaderText="From Ip">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="CommInfo2" HeaderText="Port">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="ValidGps" HeaderText="Valid GPS">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="StreetAddress" HeaderText="Street Address">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Latitude" HeaderText="Latitude">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Longitude" HeaderText="Longitude">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Speed" HeaderText="Speed">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Heading" HeaderText="Heading">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="CustomProp" HeaderText="Custom Prop">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="SequenceNum" HeaderText="Seq Num">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
							</Columns>
                      <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" Mode="NumericPages" />
						</asp:datagrid>
						<asp:datagrid id="dgrHistoryOut" runat="server" BorderWidth="1px" BorderColor="#CCCCCC" AutoGenerateColumns="False" AllowSorting="True" BackColor="White" BorderStyle="None" CellPadding="3">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#669999"></SelectedItemStyle>
							<ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="#000066"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" Font-Bold="True" ForeColor="White" BackColor="#006699"></HeaderStyle>
							<FooterStyle ForeColor="#000066" BackColor="White"></FooterStyle>
							<Columns>
								<asp:BoundColumn DataField="BoxId" HeaderText="Box Id">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="DateTime" HeaderText="Date/Time">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="DclId" HeaderText="DCL">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="AslId" HeaderText="ASL"></asp:BoundColumn>
								<asp:BoundColumn DataField="BoxCmdOutTypeName" HeaderText="Msg Type">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="BoxProtocolTypeName" HeaderText="Protocol">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="CommInfo1" HeaderText="From Ip">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="CommInfo2" HeaderText="Port">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="CustomProp" HeaderText="Custom Prop">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="SequenceNum" HeaderText="Seq Num">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemStyle Wrap="False"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Acknowledged" HeaderText="Acknowledged"></asp:BoundColumn>
							</Columns>
                      <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" Mode="NumericPages" />
						</asp:datagrid>&nbsp;
                  <ISWebGrid:WebGrid ID="dgHistIn" runat="server" Height="450px" UseDefaultStyle="True" OnInitializeDataSource="dgHistIn_InitializeDataSource" Width="1075px"><RootTable>
                     <Columns>
                        <ISWebGrid:WebGridColumn Caption="BoxId" DataMember="BoxId" Name="BoxId" Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Originated DateTime" DataMember="OriginDateTime"
                           IsAutoWidth="True" Name="OriginDateTime" Width="200px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Received DateTime" DataMember="DateTimeReceived"
                           Name="DateTimeReceived" Width="200px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="DCL" DataMember="DclId" Name="DclId" Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Msg Type" DataMember="BoxMsgInTypeName" Name="BoxMsgInTypeName"
                           Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Protocol" DataMember="BoxProtocolTypeName" Name="BoxProtocolTypeName"
                           Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Valid Gps" DataMember="ValidGps" Name="ValidGps"
                           Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Address" DataMember="StreetAddress" Name="StreetAddress"
                           Width="300px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Latitude" DataMember="Latitude" Name="Latitude"
                           Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Longitude" DataMember="Longitude" Name="Longitude"
                           Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Speed" DataMember="Speed" Name="Speed" Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Heading" DataMember="Heading" Name="Heading" Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="CustomProp" DataMember="CustomProp" Name="CustomProp"
                           Width="100px">
                        </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="SequenceNum" DataMember="SequenceNum" Name="SequenceNum"
                           Width="100px">
                        </ISWebGrid:WebGridColumn>
                     </Columns>
                  </RootTable>
                     <LayoutSettings AllowExport="Yes" AllowFilter="Yes" AllowSorting="Yes" PagingLoadMode="Custom">
                     </LayoutSettings>
                  </ISWebGrid:WebGrid>
                  <ISWebGrid:WebGrid ID="dgHistOut" runat="server" Height="450px" UseDefaultStyle="True" OnInitializeDataSource="dgHistOut_InitializeDataSource" Width="1075px" Visible="False">
                     <RootTable>
                        <Columns>
                           <ISWebGrid:WebGridColumn Caption="BoxId" DataMember="BoxId" Name="BoxId" Width="100px">
                           </ISWebGrid:WebGridColumn>
                           <ISWebGrid:WebGridColumn Caption="Date/Time" DataMember="DateTime" Name="DateTime"
                              Width="200px">
                           </ISWebGrid:WebGridColumn>
                           <ISWebGrid:WebGridColumn Caption="DCL" DataMember="DclId" Name="DclId" Width="100px">
                           </ISWebGrid:WebGridColumn>
                           <ISWebGrid:WebGridColumn Caption="Protocol" DataMember="BoxCmdOutTypeName" Name="BoxCmdOutTypeName"
                              Width="100px">
                           </ISWebGrid:WebGridColumn>
                           <ISWebGrid:WebGridColumn Caption="CustomProp" DataMember="CustomProp" Name="CustomProp"
                              Width="100px">
                           </ISWebGrid:WebGridColumn>
                           <ISWebGrid:WebGridColumn Caption="SequenceNum" DataMember="SequenceNum" Name="SequenceNum"
                              Width="100px">
                           </ISWebGrid:WebGridColumn>
                           <ISWebGrid:WebGridColumn Caption="Acknowledged" DataMember="Acknowledged" Name="Acknowledged"
                              Width="100px">
                           </ISWebGrid:WebGridColumn>
                        </Columns>
                     </RootTable>
                     <LayoutSettings AllowExport="Yes" AllowFilter="Yes" AllowSorting="Yes">
                     </LayoutSettings>
                  </ISWebGrid:WebGrid>
               </TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
