<%@ Page language="c#" Inherits="SentinelFM.frmSystemUpdate" ValidateRequest="false"  CodeFile="frmSystemUpdate.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>System Update</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="GlobalStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="form1" runat="server">
			<TABLE id="Table1" style="Z-INDEX: 102;"cellSpacing="0" cellPadding="0" width="98%" border="0">
				<TR>
					<TD><asp:datagrid id="dgSystem" runat="server" AutoGenerateColumns="False" DataKeyField="MsgId"
							Width="100%" PageSize="999" OnEditCommand="dgSystem_EditCommand" OnUpdateCommand="dgSystem_UpdateCommand" OnCancelCommand="dgSystem_CancelCommand">
							<ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif"></HeaderStyle>
							<Columns>
							<asp:TemplateColumn  HeaderText="Visible">
																																	<HeaderStyle Width="20px" ></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
								<ItemTemplate>
																																		<asp:CheckBox ID="chkVisible1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Visible")) %>' runat="server"  />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkVisible" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Visible")) %>' runat="server"  />
																																	</EditItemTemplate>
	</asp:TemplateColumn>										
	
								<asp:TemplateColumn HeaderText="Message">
									<HeaderStyle Width="400px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="lblMsg" text='<%# DataBinder.Eval(Container.DataItem,"Msg") %>' Runat=server  Width=400>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="txtMsg" Runat=server Text='<%#  DataBinder.Eval(Container.DataItem,"Msg") %>' TextMode=MultiLine    Width=400>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								
								
								<asp:TemplateColumn HeaderText="Message Fr">
									<HeaderStyle Width="400px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="lblMsgFr" text='<%# DataBinder.Eval(Container.DataItem,"MsgFr") %>' Runat=server  Width=400>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="txtMsgFr" Runat=server Text='<%#  DataBinder.Eval(Container.DataItem,"MsgFr") %>' TextMode=MultiLine    Width=400>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Color">
									<ItemStyle Wrap="False"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="lblFontColor" text='<%#  DataBinder.Eval(Container.DataItem,"FontColor") %>' Runat=server>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:DropDownList ID="cboFontColor" DataSource='<%# dsFontColor%>' DataValueField="FontColor" DataTextField="FontColor" SelectedIndex='<%# GetFontColor(Convert.ToString(DataBinder.Eval(Container.DataItem,"FontColor")))%>' Runat=server>
										</asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Bold">
																																	<HeaderStyle Width="20px" ></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
								<ItemTemplate>
																																		<asp:CheckBox ID="chkFontBold1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "FontBold")) %>' runat="server"  />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkFontBold" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "FontBold")) %>' runat="server"  />
																																	</EditItemTemplate>
	</asp:TemplateColumn>																																	
														<asp:EditCommandColumn UpdateText="&lt;img src=images/ok.gif border=0&gt;" CancelText="&lt;img src=images/cancel.gif border=0&gt;"  EditText="&lt;img src=images/edit.gif border=0&gt;" ></asp:EditCommandColumn>
								<asp:ButtonColumn Text="&lt;img src=images/delete.gif border=0&gt;" CommandName="Delete"></asp:ButtonColumn>
							</Columns>
							<PagerStyle Font-Size="11px"></PagerStyle>
						</asp:datagrid></TD>
				</TR>
				<TR>
					<TD></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 14px" align="center"><asp:label id="lblMessage" runat="server" Width="270px" Visible="False" Height="8px" CssClass="errortext"></asp:label></TD>
				</TR>
				<TR>
					<TD></TD>
				</TR>
				<TR>
					<TD>
						<TABLE id="Table2" cellSpacing="0" cellPadding="0" width="100%" border="0" style="HEIGHT: 24px" class="formtext">
                            <tr>
                                <td class="formtext" style="height: 24px">
                                </td>
                                <td style="height: 24px">
                                </td>
                                <td style="width: 4px; height: 24px">
                                    &nbsp;Visible</td>
                                <td style="width: 4px; height: 24px">
                                    &nbsp;</td>
                                <td style="height: 24px">
                                    Message
									:</td>
                                <td class="formtext" style="height: 24px">
                                    &nbsp;</td>
                                <td style="height: 24px">
                                    Message French</td>
                                <td class="formtext" style="height: 24px">
                                    &nbsp;</td>
                                <td style="height: 24px">
                                    Color</td>
                                <td style="height: 24px">
                                    &nbsp;</td>
                                <td style="height: 24px">
                                    Bold</td>
                                <td>
                                </td>
                            </tr>
							<TR>
								<TD class="formtext" style="height: 24px"></TD>
                                <td style="height: 24px">
                                </td>
                                <td style="width: 4px; height: 24px">
                                    <asp:CheckBox ID="chkVisibleMsg" runat="server" CssClass="formtext" /></td>
                                <td style="width: 4px; height: 24px">
                                </td>
								<TD style="height: 24px">
									<asp:TextBox id="txtMsg" runat="server" Width="403px" CssClass="formtext"></asp:TextBox></TD>
                                <td class="formtext" style="height: 24px">
                                    </td>
                                <td style="height: 24px">
                                    <asp:TextBox ID="txtMsgFr" runat="server" CssClass="formtext" Width="403px"></asp:TextBox></td>
								<TD class="formtext" style="height: 24px">
                                    </TD>
								<TD style="height: 24px">
									<asp:DropDownList id="cboFontColorAdd" runat="server" Width="81px" CssClass="formtext" DataTextField="FontColor"
										DataValueField="FontColor"></asp:DropDownList></TD>
                                <td style="height: 24px">
                                </td>
                                <td style="height: 24px">
                                    <asp:CheckBox ID="chkBold" runat="server" CssClass="formtext" /></td>
								<TD align="right">
									<asp:button id="cmdAddMsg" runat="server" Text="Add" CssClass="combutton" onclick="cmdAddMsg_Click"></asp:button></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD align="right">
						<INPUT class="combutton" id="cmdClose" onclick="window.close()" type="button" value="Close"
							name="cmdClose"/>
					</TD>
				</TR>
			</TABLE>
			
		</form>
	</body>
</HTML>
