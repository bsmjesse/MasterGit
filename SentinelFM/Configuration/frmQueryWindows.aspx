<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmQueryWindows.aspx.cs" Inherits="SentinelFM.Configuration_frmQueryWindows" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register src="Components/ctlOrganizationSubMenuTabs.ascx" tagname="ctlOrganizationSubMenuTabs" tagprefix="ucSubmenu" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" /> 
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>    
</head>
<body>
    <script type="text/javascript" src="../scripts/configuration_querywindows.js?v=<%=LastUpdatedQueryWindowsJs %>"></script>
    <form id="formQueryWindows" runat="server">
        
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
									<TABLE id="tblForm" class=table width="1060" border="0" style="height:auto;overflow:hidden;">
										<TR>
											<TD class="configTabBackground">
												<TABLE id="Table3" style="LEFT: 10px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<TR>
														<TD>
															<TABLE id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<TR>
																	<TD>
																		<ucSubmenu:ctlOrganizationSubMenuTabs ID="ctlSubMenuTabs" runat="server" SelectedControl="btnQueryWindows" />
																	</TD>
																</TR>
																<TR>
																	<TD  >
																		<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="657" align="center" border="0">                                                                        
																			<tr>
                                                                                <td>
                                                                                    <table id="tblBody" border="0" align="center" width="960" cellspacing="0" cellpadding="0">
                                                                                    <tr><td>
                                                                                        <div id="searchCriteriaPanel" style="margin-top:20px;"></div>
                                                                                    </td></tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>

                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table1" border="0" align="center" width="960" cellspacing="0" cellpadding="0">
                                                                                    <tr><td>
                                                                                        <div id="vehicleEmailPanel" style="margin:10px 0;"></div>
                                                                                    </td></tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            
                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table4" border="0" align="center" width="960" cellspacing="0" cellpadding="0">
                                                                                    <tr><td>
                                                                                        <div id="costCenterEmailPanel" style="margin:10px 0;"></div>
                                                                                    </td></tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>

                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table5" border="0" align="center" width="960" cellspacing="0" cellpadding="0">
                                                                                    <tr><td>
                                                                                        <div id="scheduledReportsPanel" style="margin:10px 0;"></div>
                                                                                    </td></tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            
                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table7" border="0" align="center" width="960" cellspacing="0" cellpadding="0">
                                                                                    <tr><td>
                                                                                        <div id="btnSave" style="margin:0 0 20px 0;"></div>
                                                                                    </td></tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>                                                                                    
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
            
        </table>
    </form>
</body>
</html>
