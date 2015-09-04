<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmOrganizationHierarchyImport.aspx.cs" Inherits="SentinelFM.Configuration_frmOrganizationHierarchyImport" meta:resourcekey="PageResource1" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register src="Components/ctlOrganizationSubMenuTabs.ascx" tagname="ctlOrganizationSubMenuTabs" tagprefix="ucSubmenu" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />

    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" /> 
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>   		       
</head>
<body>
    <form id="form1" runat="server">
    <table id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				    cellPadding="0" width="300" border="0">
	        <tr>
		        <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdOrganization" />
		        </td>
	        </tr>
            <TR>
					<TD>
						<TABLE id="TABLE2" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<TR>
								<TD>
									<TABLE id="tblForm" class=table width="1060" border="0" style="height:400px;overflow:hidden;">
										<TR>
											<TD class="configTabBackground" valign="top">
												<TABLE id="Table3" style="LEFT: 10px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<TR>
														<TD>
															<TABLE id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<TR>
																	<TD>
																		<ucSubmenu:ctlOrganizationSubMenuTabs ID="ctlSubMenuTabs" runat="server" SelectedControl="cmdHierarchyImport" />
																	</TD>
																</TR>
																<TR>
																	<TD  >
																		<table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                                                                            
                                                                            
                                                                            <tr>
                                                                                <td>
                                                                                    <fieldset id="tblUploadOrganizationHierarchy" runat="server" style="margin-top:50px; width: 674px; text-align: center; border: double 4px gray;padding:10px;" visible="true">
                                                                                        <legend style="text-align:center">
                                                                                            <asp:Label ID="Label1" runat="server" CssClass="RegularText" Font-Bold="True" 
                                                                                                text="Organization Hierarchy Import" meta:resourcekey="Label1Resource1"></asp:Label>
                                                                                        </legend>
                                    
                                                                                        <br />
                                                                                        <table border="0">
                                                                                            <tr>
                                                                                                <td style="text-align:left;">
                                                                                                    <asp:FileUpload ID="fileOrganizationHierarchy" runat="server" 
                                                                                                        CssClass="RegularText" Width="500px" style="margin: 20px 0px 20px 0px;" 
                                                                                                        meta:resourcekey="fileOrganizationHierarchyResource1" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="color:#999999;font-size:14px;">
                                                                                                    <asp:Label ID="Label2" runat="server" 
                                                                                                        Text="Your upload will overwrite the existing data, please backup database if it's necessary." 
                                                                                                        meta:resourcekey="Label2Resource1"></asp:Label>                                                                                                    
                                                                                                </td>
                                                                                            </tr>                                        
                                                                                            <tr>
                                                                                                <td height="40" valign="bottom" align="center">
                                                                                                    <asp:Button ID="Button1"
                                                                                                    runat="server" CssClass="combutton"
                                                                                                    Text="Submit" meta:resourcekey="cmdSaveDriverResource1" 
                                                                                                    onclick="cmdSaveUploadOrganizationHierarchy_Click" />
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                    
                                    
                                                                                        <br />
                                    
                                                                                    </fieldset>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left" style="height: 15px">
                                                                                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="20px" Width="615px" meta:resourcekey="lblMessageResource1"></asp:Label>
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
    </table>
    </form>
</body>
</html>
