<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmOrganizationPushSettings.aspx.cs" Inherits="SentinelFM.Configuration_frmOrganizationPushSettings" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Assembly="obout_ColorPicker" Namespace="OboutInc.ColorPicker" TagPrefix="obout" %>

<%@ Register Src="../UserControl/ColorPicker.ascx" TagName="ColorPicker" TagPrefix="uc1" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register src="Components/ctlOrganizationSubMenuTabs.ascx" tagname="ctlOrganizationSubMenuTabs" tagprefix="ucSubmenu" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>frmEmails</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		
		
		<script type="text/JavaScript">

function OnColorPicked(sender){
  if(typeof ob_post == "object"){
    ob_post.AddParam("mColor", sender.getColor());     
    ob_post.post(null, "OnColorSelect", function(){});
  }
}

</script>

        <style type="text/css">
            .style1
            {
                width: 100%;
            }
            .combutton
            {
                width: 37px;
            }
            .style3
            {
                width: 208px;
            }
        </style>

        </HEAD>
	<body>
		<form id="Form1" method="post" runat="server" enctype="multipart/form-data">
			<TABLE id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdOrganization" HasOrgCommandName="false" />
				    </TD>
				</TR>
				<TR>
					<TD>
						<TABLE id="tblBody" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<TR>
								<TD>
									<TABLE id="tblForm" class=table HEIGHT="550px"
										width="1060" border="0">
										<TR>
											<TD class="configTabBackground">
												<TABLE id="Table2" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<TR>
														<TD>
															<TABLE id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<TR>
																	<TD>
																		<ucSubmenu:ctlOrganizationSubMenuTabs ID="ctlSubMenuTabs" runat="server" SelectedControl="cmdPushSettings" />
																	</TD>
																</TR>
																<TR>
																	<TD  >
																		<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="657" align="center" border="0">
																			<TR>
																				<TD>
																					<TABLE id="Table7" class=table WIDTH="1030px" HEIGHT="500px"
																						border="0">
																						<TR>
																							<TD class="configTabBackground" valign=middle align=center    >
																				                
																		                                          <table class="style1">
                                                                                                    <tr>
                                                                                                        <td align="center">
																				                
																		                        <asp:datagrid id="dgPushConfiguration" runat="server" PageSize="12" 
                                                                                                    AllowPaging="True" DataKeyField="PushId"
																									 AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px"
																									 BackColor="White" GridLines="None" CellSpacing="1" onselectedindexchanged="dgPushConfiguration_SelectedIndexChanged" 
                                                                                                                ondeletecommand="dgPushConfiguration_DeleteCommand" 
                                                                                                                onitemcreated="dgPushConfiguration_ItemCreated" 
                                                                                                                meta:resourcekey="dgPushConfigurationResource1">
																									 <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
																									 <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
																									 <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
																									 <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
																									<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
																									<Columns>
																									
																									<asp:BoundColumn DataField="Configuration"  HeaderText="Configuration" Visible=false  ReadOnly=true ></asp:BoundColumn>
                                																	<asp:BoundColumn DataField="PushName" meta:resourcekey="BoundFieldPushConfigurationType"  ReadOnly=true ></asp:BoundColumn>
                                																	<asp:BoundColumn DataField="PushTypeId" Visible=false   ReadOnly=true ></asp:BoundColumn>
																									
																			    				    <asp:ButtonColumn CommandName="Select" 
                                                                                                            Text="&lt;img src=../images/edit.gif border=0&gt;" 
                                                                                                            meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
 																									    <asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" 
                                                                                                            CommandName="Delete" meta:resourcekey="ButtonColumnResource2" ></asp:ButtonColumn>
 																									
																									</Columns>
																									<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
																								</asp:datagrid>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="center">
                                                                                                            <asp:Button ID="cmdAdd" runat="server" Text="Add" onclick="cmdAdd_Click" 
                                                                                                                CssClass="combutton" Width="100px" meta:resourcekey="cmdAddResource1" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="center">
                                                                                                                            &nbsp;</td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="center">
                                                                                                                            <asp:Label ID="lblMessage" runat="server" Visible="False" CssClass="formtext" 
                                                                                                                                meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="center">
                                                                                                            <table id="tblAddUpdate" runat="server" class="formtext">
                                                                                                                <tr>
                                                                                                                   <td  colspan="2" align=left >
                                                                                                                     <table class="formtext"><tr>
                                                                                                                    <td align=left>
                                                                                                                        <asp:Label ID="lblPushType" runat="server" BorderStyle="None" Text="Push Type:" 
                                                                                                                            meta:resourcekey="lblPushTypeResource1"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td align=left>&nbsp;
                                                                                                                    <asp:Label ID="lblPushTypeName"   runat="server" Font-Bold="True" Visible="False" 
                                                                                                                            meta:resourcekey="lblPushTypeNameResource1"></asp:Label>
                                                                                                                       <asp:DropDownList ID="cboPushType" runat="server"    DataTextField="PushName" 
                                                                                                                            DataValueField="PushTypeId" 
                                                                                                                            onselectedindexchanged="cboPushType_SelectedIndexChanged" 
                                                                                                                            AutoPostBack="True" meta:resourcekey="cboPushTypeResource1">
                                                                                                                        </asp:DropDownList>
                                                                                                                        
                                                                                                                    </td>
                                                                                                                     </tr></table>
                                                                                                                    </td>
                                                                                                                    
                                                                                                                    
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td  colspan="2" align=left>
                                                                                                                        <asp:Label ID="lblPushId" runat="server" Visible="False" 
                                                                                                                            meta:resourcekey="lblPushIdResource1"></asp:Label>
                                                                                                                        <asp:Label ID="lblPushTypeId" runat="server" Visible="False" 
                                                                                                                            meta:resourcekey="lblPushTypeIdResource1"></asp:Label>
                                                                                                                    </td>
                                                                                                                   
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="2" align=left >
                                                                                                                        <table id="tblFTP" runat=server   class="formtext">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUrlFTP" runat="server" Text="Url:" 
                                                                                                                                        meta:resourcekey="lblUrlFTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUrlFTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUrlFTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUserNameFTP" runat="server" Text="User Name:" 
                                                                                                                                        meta:resourcekey="lblUserNameFTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUserNameFTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUserNameFTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblPasswordFTP" runat="server" Text="Password:" 
                                                                                                                                        meta:resourcekey="lblPasswordFTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtPasswordFTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtPasswordFTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        
                                                                                                                        </table>
                                                                                                                        
                                                                                                                        <table id="tblSMTP" runat=server  class="formtext" >
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblEmailSMTP" runat="server" Text="Email:" 
                                                                                                                                        meta:resourcekey="lblEmailSMTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtEmailSMTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtEmailSMTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblSubjSMTP" runat="server" Text="Subject:" 
                                                                                                                                        meta:resourcekey="lblSubjSMTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtSubjSMTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtSubjSMTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            
                                                                                                                            
                                                                                                                             <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUserNameSMTP" runat="server" Text="User Name:" 
                                                                                                                                        meta:resourcekey="lblUserNameSMTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUserNameSMTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUserNameSMTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblPasswordSMTP" runat="server" Text="Password:" 
                                                                                                                                        meta:resourcekey="lblPasswordSMTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtPasswordSMTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtPasswordSMTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                         
                                                                                                                        </table>
                                                                                                                        
                                                                                                                        
                                                                                                                         <table id="tblHTTP" runat=server   class="formtext">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUrlHTTP" runat="server" Text="Url:" 
                                                                                                                                        meta:resourcekey="lblUrlHTTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUrlHTTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUrlHTTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUserNameHTTP" runat="server" Text="User Name:" 
                                                                                                                                        meta:resourcekey="lblUserNameHTTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUserNameHTTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUserNameHTTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblPasswordHTTP" runat="server" Text="Password:" 
                                                                                                                                        meta:resourcekey="lblPasswordHTTPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtPasswordHTTP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtPasswordHTTPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        
                                                                                                                        </table>
                                                                                                                        
                                                                                                                        
                                                                                                                        
                                                                                                                        
                                                                                                                        <table id="tblHTTPS" runat=server   class="formtext">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUrlHTTPS" runat="server" Text="Url:" 
                                                                                                                                        meta:resourcekey="lblUrlHTTPSResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUrlHTTPS" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUrlHTTPSResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUserNameHTTPS" runat="server" Text="User Name:" 
                                                                                                                                        meta:resourcekey="lblUserNameHTTPSResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUserNameHTTPS" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUserNameHTTPSResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblPasswordHTTPS" runat="server" Text="Password:" 
                                                                                                                                        meta:resourcekey="lblPasswordHTTPSResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtPasswordHTTPS" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtPasswordHTTPSResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        
                                                                                                                        </table>
                                                                                                                        
                                                                                                                        
                                                                                                                        
                                                                                                                          <table id="tblTCP_IP" runat=server   class="formtext">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblIpTCP" runat="server" Text="IP:" 
                                                                                                                                        meta:resourcekey="lblIpTCPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtIpTCP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtIpTCPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            
                                                                                                                            
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblPortTCP" runat="server" Text="Port:" 
                                                                                                                                        meta:resourcekey="lblPortTCPResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtPortTCP" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtPortTCPResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            
                                                                                                                        </table>
                                                                                                                        
                                                                                                                        
                                                                                                                          <table id="tblWebService" runat=server   class="formtext">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUrlWebService" runat="server" Text="URL:" 
                                                                                                                                        meta:resourcekey="lblUrlWebServiceResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUrlWebService" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUrlWebServiceResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            
                                                                                                                            
                                                                                                                             <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblUserNameWebService" runat="server" Text="User Name:" 
                                                                                                                                        meta:resourcekey="lblUserNameWebServiceResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtUserNameWebService" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtUserNameWebServiceResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            
                                                                                                                            
                                                                                                                              <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Label ID="lblPasswordWebService" runat="server" Text="Password:" 
                                                                                                                                        meta:resourcekey="lblPasswordWebServiceResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:TextBox ID="txtPasswordWebService" runat="server" Width="250px" 
                                                                                                                                        meta:resourcekey="txtPasswordWebServiceResource1"></asp:TextBox>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            
                                                                                                                        </table>
                                                                                                                        
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                              
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    
                                                                                                       <tr>
                                                                                                        <td align="center">
                                                                                                            &nbsp;</td>
                                                                                                    </tr>
                                                                                                    
                                                                                                       <tr>
                                                                                                        <td align="center">
                                                                                                         <table id="tblUpdateButtons" runat=server>
                                                                                                            <tr><td class="style3">
                                                                                                            <asp:Button ID="cmdCancel" runat="server" Text="Cancel" 
                                                                                                                onclick="cmdCancel_Click" CssClass="combutton" Width="100px" 
                                                                                                                    meta:resourcekey="cmdCancelResource1" /> &nbsp;&nbsp;<asp:Button 
                                                                                                                ID="cmdSave" runat="server" Text="Save" CssClass="combutton" 
                                                                                                                    onclick="cmdSave_Click" Width="100px" 
                                                                                                                    meta:resourcekey="cmdSaveResource1" />
                                                                                                                </td></tr>
                                                                                                                </table> 
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    
                                                                                                </table>
                                                                                              </TD>
																						</TR>
																					</TABLE>
																				</TD>
																			</TR>
																		</TABLE>
																	</TD>
																</TR>
															</TABLE>
														</TD>
													</TR>
												</TABLE>
											</TD>
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
